Imports System.Reflection

Public Class NC
    ReadOnly assemblyFileName As String
    ReadOnly clientType As String
    ReadOnly clusterPath As String
    ReadOnly clientName As String
    Private assy As Assembly
    Private log As SGConsoleApp.Log

    Public Sub New(InitassemblyFileName As String, InitclientType As String, InitclusterPath As String, InitclientName As String, Initlog As Log)
        log = Initlog
        assemblyFileName = InitassemblyFileName
        clientType = InitclientType
        clusterPath = InitclusterPath
        clientName = InitclientName
    End Sub
    Sub connectAndRun(task As SGConsoleApp.Settings.Task, filePath As String)
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
            ProcessStep = "NasComm Task " & task.Name & ", start running NasComm procedure " & procedureName & ", CommandPrameter=" & NCTask("CommandParameter")
            Console.WriteLine(ProcessStep)
            'log.Logita(ProcessStep)
            'NCTask.RunProcedure(procedureName)
            'If NCTask("process.success") <> "Y" Then
            '    log.Logita("NASComm script processing failed", "error")
            'End If
            'task.ProcessOuput = NCTask.ContentView(100000)
        Catch ex As Exception
            log.Logita("Error in connectAndRun, " & ProcessStep & ": " & ex.Message, "e")
            log.Logita("Error in connectAndRun, Inner Exception: " & ex.InnerException.Message, "e")
        End Try
    End Sub
    Sub connectAndRunOld(task As SGConsoleApp.Settings.Task)
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
            NCTask("CommandParameter") = task.CommandParameter
            ProcessStep = "NasComm Task " & task.Name & ", start running NasComm procedure " & procedureName & ", CommandPrameter=" & NCTask("CommandParameter")
            log.Logita(ProcessStep)
            NCTask.RunProcedure(procedureName)
            If NCTask("process.success") <> "Y" Then
                log.Logita("NASComm script processing failed", "error")
            End If
            task.ProcessOuput = NCTask.ContentView(100000)
        Catch ex As Exception
            log.Logita("Error in connectAndRun, " & ProcessStep & ": " & ex.Message, "e")
            log.Logita("Error in connectAndRun, Inner Exception: " & ex.InnerException.Message, "e")
        End Try

    End Sub

End Class
