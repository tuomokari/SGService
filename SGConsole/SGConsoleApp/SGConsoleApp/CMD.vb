Imports System.Diagnostics
Imports System.IO
Public Class CMD
	Private log As SGConsoleApp.Log
	Private ReadOnly cmdExe As New Process()
	Public Sub New(InitFileName As String, Initlog As Log, task As SGConsoleApp.Settings.Task)
		Dim exeOut As String = ""
		Dim exeError As String = ""
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
			exeOut = cmdExe.StandardOutput.ReadToEnd()
			exeError = cmdExe.StandardError.ReadToEnd()
			cmdExe.WaitForExit()
			Dim ExitCode As String
			ExitCode = cmdExe.ExitCode.ToString()
		Catch ex As Exception
			Dim hui As String = ex.Message
			Dim hai As String = exeError
		End Try
	End Sub

End Class
