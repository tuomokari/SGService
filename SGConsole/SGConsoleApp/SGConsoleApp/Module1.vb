Imports Microsoft.VisualBasic.Logging
Imports System.Configuration
Imports System.IO
Imports System.Runtime
Imports System.Threading

Module Module1

    Friend log As SGConsoleApp.Log
    Friend nc As SGConsoleApp.NC
    Private LoopFrequenceSeconds As Integer
    Public settings As Settings
    Private settingsFileName As String = "SGServiceSettings.json"
    Private logFileNumber As Integer = 0
    Sub Main()
        Try
            'Dim fileList As New List(Of String)()
            'fileList.Add("hui")
            'fileList.Add("hai")
            'For Each f As String In fileList
            '    Console.WriteLine(f)
            'Next
            'Console.WriteLine(fileList.Count)
            'Console.ReadLine()
            'Exit Sub

            Dim runInFolder As String = "C:\A\SG\projects\SGService\SGConsole\SGConsoleApp\SGConsoleApp"
            settings = New Settings(runInFolder, settingsFileName)
            log = New SGConsoleApp.Log(settings.AllSettings.EventLogName, settings.AllSettings.EventlogSourceName)

            Directory.CreateDirectory(settings.AllSettings.FileLogFolder) ' general logs directory
            Console.WriteLine(Environment.CurrentDirectory)
            LoopFrequenceSeconds = settings.AllSettings.LoopFrequenceSeconds

            log.Logita("ConsoleApp Started at " & runInFolder & ", settingsFileName=" & settingsFileName)



            ' Update NextRun for all tasks with a timesetting
            Dim timer As Timer = New SGConsoleApp.Timer(log)
            ' Update NextRun for all tasks with a timesetting
            For Each task As Settings.Task In settings.AllSettings.TaskList 'settings.AllSettings.TaskList
                timer.createDirecotries(task)
                timer.SetNextRun(task)
            Next
            Dim email As Email = New SGConsoleApp.Email(settings.AllSettings.Email)
            Dim errorMessage As String = email.sendEmail(settings.AllSettings.Email.TaskErrorAddresslist, "Error in task", "Hui!")


            ' eipä nyt ajeta:
            If (1 = 0) Then
                For Each task As Settings.Task In settings.AllSettings.TaskList
                    Dim fileList As New List(Of String)()
                    If timer.isTimeToRun(task, fileList) Then
                        Select Case LCase(task.Program)
                            Case "nascomm"
                                If fileList.Count > 0 Then
                                    For Each f In fileList
                                        nc = New NC(settings.AllSettings.NasComm.AssemblyFileName, settings.AllSettings.NasComm.ClientType, settings.AllSettings.NasComm.ClusterPath, settings.AllSettings.NasComm.ClientName, log)
                                        nc.connectAndRun(task, f)
                                        logWrite(task)
                                        nc = Nothing
                                    Next
                                Else
                                    nc = New NC(settings.AllSettings.NasComm.AssemblyFileName, settings.AllSettings.NasComm.ClientType, settings.AllSettings.NasComm.ClusterPath, settings.AllSettings.NasComm.ClientName, log)
                                    nc.connectAndRun(task, "")
                                    logWrite(task)
                                    nc = Nothing
                                End If
                            Case "cmd"
                                Dim cmd As SGConsoleApp.CMD = New CMD(settings.AllSettings.CMD, log, task)
                        End Select
                        timer.SetNextRun(task)
                    End If
                Next
            End If


        Catch ex As Exception
            Console.WriteLine("Error OnStart: " & ex.Message, "e")
        End Try
        Console.WriteLine("Enter NEW-LINE")
        Console.Read()

        End

    End Sub
    Sub logWrite(task As Settings.Task)
        Dim folderPath As String = settings.AllSettings.FileLogFolder
        If folderPath = "" Then Exit Sub

        'logFileNumber += 1
        'Dim filename As String = Now.Year.ToString & "n" & Right("000000" & CStr(logFileNumber), 7) & ".txt"
        'Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
        'System.IO.File.WriteAllBytes(folderPath & "\\" & filename, enc.GetBytes(task.ProcessOuput))
    End Sub

End Module
