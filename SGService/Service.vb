Imports System.Configuration
Imports System.IO
Imports System.Threading
Imports SGService.Settings
Imports SGService.Email

Public Class Service
    Public settings As Settings
    Friend eventLog As SGService.EventLog
    Friend nc As SGService.NasCommTask
    Protected stopping As New ManualResetEvent(False)
    Protected RunningThread As Thread
    Public errorCount As Integer
    Private errorMaxCount As Integer = 10
    Private runInFolder As String
    Private settingsFileName As String
    Private LoopFrequenceSeconds As Integer
    Private logFileNumber As Integer = 0

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            GetCommandParameters()
            ChDir(runInFolder)
            settings = New Settings(runInFolder, settingsFileName)
            eventLog = New SGService.EventLog(settings.AllSettings.EventLog.Name, settings.AllSettings.EventLog.SourceName)
            Directory.CreateDirectory(settings.AllSettings.FileLog.FolderPath) ' general logs directory

            LoopFrequenceSeconds = settings.AllSettings.MainLoopFrequenceSeconds

            eventLog.Logita("Service Started at " & runInFolder & ", settingsFileName=" & settingsFileName)

            ' Update NextRun for all tasks with a timesetting
            Dim timer As Timer = New SGService.Timer(eventLog)
            For Each task As Task In settings.AllSettings.TaskList
                If task.Enabled Then
                    timer.createDirecotries(task)
                    timer.SetNextRun(task)
                    eventLog.Logita("Initial Time Setting of Task " & task.Name & ", next run=" & task.NextRun.ToString("dd.MM.yyyy HH.mm"))
                End If
            Next

            ' Service Loop: =======================================
            RunningThread = New Thread(AddressOf RunService)
            RunningThread.Start(timer)
            ' =====================================================
        Catch ex As Exception
            eventLog.Logita("Error OnStart: " & ex.Message, "e")
            eventLog.Logita("Error at RunService, Stack: " & ex.StackTrace.ToString, "e")
            eventLog.Logita("Error OnStart: Service Self-Stopping")
            Environment.Exit(1)
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Try
            stopping.Set()
            eventLog.Logita("Service Stopping...")
        Catch ex As Exception
            eventLog.Logita("Error OnStop:" & ex.Message, "e")
        End Try
    End Sub
    Private Sub RunService(timer As SGService.Timer)
        While Not stopping.WaitOne(0)
            If stopping.WaitOne(LoopFrequenceSeconds * 1000) Then
                Exit While
            End If
            Try
                For Each task As Settings.Task In settings.AllSettings.TaskList
                    Dim fileList As New List(Of String)()
                    If task.Enabled AndAlso timer.isTimeToRun(task, fileList) Then
                        task.Result = New ProcessResult
                        Select Case LCase(task.Program)
                            Case "nascomm"
                                nc = New NasCommTask(settings.AllSettings.NasComm.AssemblyFileName, settings.AllSettings.NasComm.ClientType, settings.AllSettings.NasComm.ClusterPath, settings.AllSettings.NasComm.ClientName)
                                If fileList.Count > 0 Then
                                    For Each f In fileList
                                        nc.connectAndRun(task, f)
                                    Next
                                Else
                                    nc.connectAndRun(task, "")
                                End If
                                nc = Nothing
                            Case "cmd"
                                Dim cmd As SGService.CmdTask = New CmdTask(settings.AllSettings.WindowsCmdExecutablePath, task)
                            Case "cleanup"
                                Dim cleanUpTask As SGService.CleanUpTask = New CleanUpTask(task)
                        End Select
                        handleExecutedTask(task)
                        timer.SetNextRun(task)
                    End If
                Next
            Catch ex As Exception
                ' Email on error:
                Dim email As Email = New SGService.Email(settings.AllSettings.Email)
                Dim errorMessage As String = email.sendEmail(settings.AllSettings.Email.TaskErrorAddresslist, "Service Task Process Error", ex.Message)
                If (errorMessage > "") Then
                    eventLog.Logita("Error sending Email: " & errorMessage, "error")
                End If
                ' Log:
                eventLog.Logita("Error at RunService: " & ex.Message, "e")
                eventLog.Logita("Error at RunService, Stack: " & ex.StackTrace.ToString, "e")
                errorCount += 1
                If errorCount >= errorMaxCount Then
                    stopping.Set()
                    eventLog.Logita("Stopping Service because Max Error Count Was Reached (" & errorMaxCount.ToString & ")", "w")
                End If
            End Try
        End While
        eventLog.Logita("Service Stopped")

    End Sub
    ' Move schedule-file from spool to ProcessedFolder OR to FailedFolder
    ' Log to eventLog
    ' Log to Disk
    Sub handleExecutedTask(task As Settings.Task)
        ' move schedule-file from spool:
        If Not (task.Schedule.FileSetting Is Nothing) AndAlso Not (task.Schedule.FileSetting.FileName = Nothing) Then
            Dim fileTarget As String
            If task.Result.Success Then
                fileTarget = Replace(task.Schedule.FileSetting.FileName, task.Schedule.FileSetting.ProcessFolder, task.Schedule.FileSetting.ProcessedFolder)
            Else
                fileTarget = Replace(task.Schedule.FileSetting.FileName, task.Schedule.FileSetting.ProcessFolder, task.Schedule.FileSetting.FailedFolder)
            End If
            File.Move(task.Schedule.FileSetting.FileName, fileTarget)
        End If
        ' Log to eventLog:
        Dim logHeader As String = "Task " & task.Name
        If task.Result.ProcessError > "" Or Not task.Result.Success Then
            eventLog.Logita(logHeader & " Prosess Error: " & task.Result.ProcessError, "error")
        End If
        If task.Result.ProcessWarning > "" Then
            eventLog.Logita(logHeader & " Warning:" & task.Result.ProcessWarning, "warning")
        End If
        If task.Result.ProcessInfo > "" Then
            eventLog.Logita(logHeader & ":" & task.Result.ProcessInfo, "info")
        End If
        ' Email on error:
        If task.Result.ProcessError > "" Or Not task.Result.Success Then
            Dim email As Email = New SGService.Email(settings.AllSettings.Email)
            Dim errorMessage As String = email.sendEmail(settings.AllSettings.Email.TaskErrorAddresslist, "Error in task " & task.Name, task.Result.ProcessError)
            If (errorMessage > "") Then
                eventLog.Logita(logHeader & " Error sending Email: " & errorMessage, "error")
            End If
        End If
        ' Log to Disk:
        If settings.AllSettings.FileLog IsNot Nothing Then
            Dim folderPath As String = settings.AllSettings.FileLog.FolderPath
            If folderPath > "" And (task.Result.ProcessError.Length > 3 Or (task.Result.ProcessLog.Length > 3 AndAlso Not settings.AllSettings.FileLog.OnlyErrors)) Then
                Dim processLog As String = logHeader & Environment.NewLine
                If task.Result.ProcessError.Length > 1 Then
                    processLog += "Error:" & task.Result.ProcessError & Environment.NewLine & "=======================" & Environment.NewLine
                End If
                processLog += task.Result.ProcessLog
                Dim filename As String = Now.ToString("yyyyMMdd_HHmmss_ms_") & logHeader & ".txt"
                Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
                System.IO.File.WriteAllBytes(folderPath & "\\" & filename, enc.GetBytes(processLog))
            End If
        End If
    End Sub
    Sub GetCommandParameters()
        runInFolder = Service.serviceParameters(1)
        settingsFileName = Service.serviceParameters(2)
    End Sub

End Class
