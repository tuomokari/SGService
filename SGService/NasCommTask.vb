Imports System.Reflection
Imports System.IO
Imports SGService.Settings

Public Class NasCommTask
    ReadOnly assemblyFileName As String
    ReadOnly clientType As String
    ReadOnly clusterPath As String
    ReadOnly clientName As String
    Private assy As Assembly

    Public Sub New(InitassemblyFileName As String, InitclientType As String, InitclusterPath As String, InitclientName As String)
        assemblyFileName = InitassemblyFileName
        clientType = InitclientType
        clusterPath = InitclusterPath
        clientName = InitclientName
    End Sub
    Sub connectAndRun(task As SGService.Settings.Task, filePath As String)
        Dim ProcessStep As String = "init"
        Try
            If assy = Nothing Then
                assy = Assembly.LoadFrom(assemblyFileName)
                ProcessStep = "NasComm Assembly loaded."
            End If
            If assy = Nothing Then Throw New Exception("Error in NasComm Assembly LoadFrom.")
            Dim NasCommClient As Object = assy.CreateInstance(clientType)
            ProcessStep = "NasComm Client Instance Created."
            NasCommClient.connect(clientName, clusterPath)
            ProcessStep = "NasComm Client Connected."
            Dim NCTask As Object = NasCommClient.NewTask
            ProcessStep = "NasComm Task created."
            Dim procedureName As String = Replace(task.Command, "/", ".")
            procedureName = Replace(procedureName, "\", ".")
            If task.CommandParameter Is Nothing Then
                NCTask("CommandParameter") = ""
            Else
                NCTask("CommandParameter") = Replace(task.CommandParameter, "{FileName}", filePath)
            End If
            ProcessStep = "NasComm Task " & task.Name & ", start running NasComm procedure " & procedureName & ", CommandParameter=" & NCTask("CommandParameter")
            NCTask.RunProcedure(procedureName)
            ' after task processing:
            If NCTask("process.success") <> "Y" Then
                task.Result.Success = False
            Else
                task.Result.Success = True
            End If
            task.Result.ProcessError = NCTask("eventlog.error")
            task.Result.ProcessWarning = NCTask("eventlog.warning")
            task.Result.ProcessInfo = NCTask("eventlog.info")
            task.Result.ProcessLog = NCTask.ContentView(10000)
        Catch ex As Exception
            task.Result.Success = False
            task.Result.ProcessError = ex.Message & Environment.NewLine & ex.StackTrace.ToString
        End Try

    End Sub

End Class
