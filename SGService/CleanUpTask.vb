Imports System.IO

Public Class CleanUpTask
    Public Sub New(task As SGService.Settings.Task)
        Try
            Dim getsOldAt As Date = Now
            If (Not task.CleanUpSetting.RetainDays = Nothing) Then getsOldAt = DateAdd(DateInterval.Day, 0 - task.CleanUpSetting.RetainDays, getsOldAt)
            If (Not task.CleanUpSetting.RetainHours = Nothing) Then getsOldAt = DateAdd(DateInterval.Hour, 0 - task.CleanUpSetting.RetainHours, getsOldAt)
            Dim filter As String = task.CleanUpSetting.FileMask
            Dim SOption As SearchOption = SearchOption.TopDirectoryOnly

            If task.CleanUpSetting.SearchSubDirectories Then
                SOption = SearchOption.AllDirectories
            End If

            Dim flist As String() = task.CleanUpSetting.FolderList.Split(",")
            Dim removedCnt As Integer
            For Each folder As String In flist
                folder = folder.Trim
                task.Result.ProcessInfo += "Folder " & folder & Environment.NewLine
                removedCnt = 0
                Dim tiedostot() As String = Directory.GetFiles(folder, filter, SOption)
                For Each tiedosto As String In tiedostot
                    Dim fi As FileInfo = New FileInfo(tiedosto)
                    If fi.LastWriteTime < getsOldAt Then
                        Try
                            fi.Delete()
                            removedCnt += 1
                        Catch ex As Exception
                            task.Result.ProcessWarning += ex.Message + Environment.NewLine
                        End Try
                    End If
                Next
                If removedCnt > 0 Then
                    task.Result.ProcessInfo += "Removed " & CStr(removedCnt) & " files in " & folder & Environment.NewLine
                End If
            Next
            task.Result.Success = True
        Catch ex As Exception
            task.Result.Success = False
            task.Result.ProcessError = ex.Message & Environment.NewLine & ex.StackTrace.ToString
        End Try
    End Sub
End Class
