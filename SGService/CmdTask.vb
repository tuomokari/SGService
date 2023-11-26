Imports System.Diagnostics
Imports System.IO
Imports SGService.Settings
Public Class CmdTask
	Private ReadOnly cmdExe As New Process()
	Public Sub New(InitFileName As String, task As SGService.Settings.Task)
		Dim exeOut As String
		Dim exeError As String
		Try
			cmdExe.StartInfo.FileName = InitFileName
			cmdExe.StartInfo.Arguments = task.Command

			'exe.StartInfo.WorkingDirectory = "D:\SystemsGarden\server\healthcheck\wwwroot\Control"
			cmdExe.StartInfo.UseShellExecute = False
			cmdExe.StartInfo.RedirectStandardInput = True
			cmdExe.StartInfo.RedirectStandardOutput = True
			cmdExe.StartInfo.RedirectStandardError = True
			cmdExe.StartInfo.CreateNoWindow = True
			cmdExe.Start()
			cmdExe.StandardInput.Close()
			exeError = cmdExe.StandardError.ReadToEnd()
			exeOut = cmdExe.StandardOutput.ReadToEnd()
			task.Result.ProcessLog = exeError & Environment.NewLine & exeOut
			cmdExe.WaitForExit()
			Dim ExitCode As String
			ExitCode = cmdExe.ExitCode.ToString()
			' after task processing:
			If ExitCode <> "0" Then
				task.Result.Success = False
				task.Result.ProcessError = "Error running cmd:" & exeError
			Else
				task.Result.Success = True
				task.Result.ProcessInfo = exeOut
			End If
		Catch ex As Exception
			task.Result.Success = False
			task.Result.ProcessError = ex.Message & Environment.NewLine & ex.StackTrace.ToString
		End Try
	End Sub

End Class
