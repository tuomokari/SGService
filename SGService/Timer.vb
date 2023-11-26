Imports SGService.Settings
Imports System.Globalization
Imports System.IO

Public Class Timer
    Private log As SGService.EventLog
    Public Sub New(Initlog As EventLog)
        log = Initlog
    End Sub
    Friend Function isTimeToRun(task As Task, ByRef fileList As List(Of String)) As Boolean
        Dim isTime As Boolean = False
        If Not task.Enabled Then
            Return False
        End If
        If LCase(task.Schedule.Type) = "time" Then
            If (Not task.NextRun = Nothing) And task.NextRun < Now Then
                isTime = True
            End If
        ElseIf LCase(task.Schedule.Type) = "file" Then
            If task.Schedule.FileSetting.SpoolFolder > "" And task.Schedule.FileSetting.ProcessFolder > "" Then
                Dim fileNames As String() = System.IO.Directory.GetFiles(task.Schedule.FileSetting.SpoolFolder, task.Schedule.FileSetting.FileMask)
                For Each fileName In fileNames
                    Dim fileTarget = Replace(fileName, task.Schedule.FileSetting.SpoolFolder, task.Schedule.FileSetting.ProcessFolder)
                    log.Logita("Found Scheduling File " & fileName, "i")
                    If (File.Exists(fileTarget)) Then File.Delete(fileTarget)
                    File.Move(fileName, fileTarget)
                    fileList.Add(fileTarget)
                    isTime = True
                Next
            End If
        End If
        Return isTime
    End Function
    Friend Function createDirecotries(task As Task) As Boolean
        If LCase(task.Schedule.Type) = "file" Then
            If task.Schedule.FileSetting.SpoolFolder > "" Then
                Directory.CreateDirectory(task.Schedule.FileSetting.SpoolFolder)
            End If
            If task.Schedule.FileSetting.ProcessFolder > "" Then
                Directory.CreateDirectory(task.Schedule.FileSetting.ProcessFolder)
            End If
            If task.Schedule.FileSetting.ProcessedFolder > "" Then
                Directory.CreateDirectory(task.Schedule.FileSetting.ProcessedFolder)
            End If
            If task.Schedule.FileSetting.FailedFolder > "" Then
                Directory.CreateDirectory(task.Schedule.FileSetting.FailedFolder)
            End If
        End If
    End Function
    Public Sub SetNextRun(Task As Settings.Task)
        Try

            If LCase(Task.Schedule.Type) <> "time" Then
                Task.NextRun = Nothing
                Exit Sub
            End If
            Dim soon As DateTime = DateTime.Now.AddSeconds(20)
            Dim currentMday As Integer = soon.Day
            Dim maxDay As Integer = DateTime.DaysInMonth(soon.Year, soon.Month)
            Dim todayAt00 As DateTime = New DateTime(soon.Year, soon.Month, soon.Day, 0, 0, 0, 0, CultureInfo.CurrentCulture.Calendar)
            ' Setting date nextRun, and -in case that appears to be before now-  nextRunLater
            ' DayOfMonth and DayOfWeek set nextRun to:
            '   - if not set: = soon
            '   - if today is set: = today at 00:00
            '   - other days set: the earliest and the second earliest after today 00:00
            Dim nextRun As DateTime = soon
            Dim nextRunLater As DateTime = soon
            Dim monthDayList As String = ""
            Dim weekDayList As String = ""
            If Not Task.Schedule.TimeSetting.DayOfMonthList Is Nothing Then
                monthDayList = LCase(Task.Schedule.TimeSetting.DayOfMonthList.Trim)
            End If
            If Not Task.Schedule.TimeSetting.DayOfWeekList Is Nothing Then
                weekDayList = LCase(Task.Schedule.TimeSetting.DayOfWeekList.Trim)
            End If
            If monthDayList > "" And monthDayList <> "*" Then
                nextRun = Nothing
                nextRunLater = Nothing
                Dim daylist As String() = monthDayList.Split(",")
                For Each dayString As String In daylist
                    dayString = dayString.Trim
                    If dayString = "last" Then
                        dayString = maxDay.ToString
                    End If
                    If IsNumeric(dayString) Then
                        Dim monthDay As Integer = CInt(dayString)
                        If monthDay <= DateTime.DaysInMonth(soon.Year, soon.Month) And monthDay > 0 Then
                            Dim nextRunCandi As DateTime = New DateTime(soon.Year, soon.Month, monthDay, 0, 0, 0, 0, CultureInfo.CurrentCulture.Calendar)
                            If nextRunCandi < todayAt00 Then
                                nextRunCandi = nextRunCandi.AddMonths(1)
                            End If
                            If nextRun = Nothing Or nextRun > nextRunCandi Then
                                If nextRunLater = Nothing Or nextRunLater > nextRun Then
                                    nextRunLater = nextRun
                                End If
                                nextRun = nextRunCandi
                            ElseIf nextRunLater = Nothing Or nextRunLater > nextRunCandi Then
                                nextRunLater = nextRunCandi
                            End If
                        End If
                    End If
                Next
                If nextRunLater = Nothing Then
                    nextRunLater = nextRun.AddMonths(1)
                End If
            ElseIf weekDayList > "" And weekDayList <> "*" Then
                nextRun = Nothing
                nextRunLater = Nothing
                Dim daylist As String() = weekDayList.Split(",")
                For Each dayString As String In daylist
                    dayString = dayString.Trim
                    If IsNumeric(dayString) Then
                        Dim weekDay As Integer = CInt(dayString)
                        If weekDay <= 7 And weekDay > 0 Then
                            Dim nextRunCandi As DateTime = New DateTime(soon.Year, soon.Month, soon.Day, 0, 0, 0, 0, CultureInfo.CurrentCulture.Calendar)
                            If weekDay > CInt(nextRunCandi.DayOfWeek) Then
                                nextRunCandi = nextRunCandi.AddDays(weekDay - CInt(nextRunCandi.DayOfWeek))
                            ElseIf weekDay < CInt(nextRunCandi.DayOfWeek) Then
                                nextRunCandi = nextRunCandi.AddDays(7 + weekDay - CInt(nextRunCandi.DayOfWeek))
                            End If
                            If nextRun = Nothing Or nextRun > nextRunCandi Then
                                If nextRunLater = Nothing Or nextRunLater > nextRun Then
                                    nextRunLater = nextRun
                                End If
                                nextRun = nextRunCandi
                            ElseIf nextRunLater = Nothing Or nextRunLater > nextRunCandi Then
                                nextRunLater = nextRunCandi
                            End If
                        End If
                    End If
                Next
                If nextRunLater = Nothing Then
                    nextRunLater = nextRun.AddDays(7)
                End If
            Else
                nextRunLater = nextRun.AddDays(1)
            End If
            ' Set Hour and Minute:
            Dim hourList As String = Task.Schedule.TimeSetting.HourList.Trim()
            If hourList = "" Then
                hourList = "0"
            End If
            If hourList = "*" Then
                hourList = ""
                For i As Integer = 0 To 22
                    hourList += CStr(i) & ","
                Next
                hourList += "23"
            End If
            Dim minuteList As String = Task.Schedule.TimeSetting.MinuteList.Trim()
            If minuteList = "" Then
                minuteList = "0"
            End If
            If minuteList = "*" Then
                minuteList = ""
                For i As Integer = 0 To 58
                    minuteList += CStr(i) & ","
                Next
                minuteList += "59"
            End If
            Dim minuteIsSet As Boolean = False
            Dim hours As String() = hourList.Split(",")
            Dim minutes As String() = minuteList.Split(",")
            For Each hourString As String In hours
                hourString = hourString.Trim
                If CInt(hourString) < 24 Then
                    Dim nextRunCandi As DateTime = New DateTime(nextRun.Year, nextRun.Month, nextRun.Day, CInt(hourString), 0, 0, 0, CultureInfo.CurrentCulture.Calendar)
                    For Each minuteString As String In minutes
                        minuteString = minuteString.Trim
                        If CInt(minuteString) < 60 Then
                            Dim nextRunCandi2 As DateTime = nextRunCandi.AddMinutes(CInt(minuteString))
                            If nextRunCandi2 > soon Then
                                If Not (minuteIsSet) Then ' this because: nextRun may be originally > soon but still minute must be set from the list
                                    nextRun = nextRunCandi2
                                    minuteIsSet = True
                                ElseIf nextRunCandi2 < nextRun Then
                                    nextRun = nextRunCandi2
                                End If
                            End If
                        End If
                    Next
                End If
            Next
            If Not minuteIsSet Then
                For Each hourString As String In hours
                    hourString = hourString.Trim
                    If CInt(hourString) < 24 Then
                        Dim nextRunCandi As DateTime = New DateTime(nextRunLater.Year, nextRunLater.Month, nextRunLater.Day, CInt(hourString), 0, 0, 0, CultureInfo.CurrentCulture.Calendar)
                        For Each minuteString As String In minutes
                            minuteString = minuteString.Trim
                            If CInt(minuteString) < 60 Then
                                Dim nextRunCandi2 As DateTime = nextRunCandi.AddMinutes(CInt(minuteString))
                                If nextRunCandi2 > soon Then
                                    If (Not (minuteIsSet)) Then ' this because: nextRunLater may be originally > soon but still minute must be set from the list
                                        nextRun = nextRunCandi2
                                        minuteIsSet = True
                                    ElseIf nextRunCandi2 < nextRun Then
                                        nextRun = nextRunCandi2
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
            End If

            Task.NextRun = nextRun
        Catch ex As Exception
            Throw New Exception("Error in Timer:" & ex.Message & ". StackTrace=" & ex.StackTrace.ToString)
        End Try
    End Sub

End Class
