Public Class EventLog
    ReadOnly EventLogName As String
    ReadOnly EventlogSourceName As String
    Public Sub Logita(messu As String, Optional type As String = "information")
        If (Not System.Diagnostics.EventLog.SourceExists(EventlogSourceName)) Then
            System.Diagnostics.EventLog.CreateEventSource(
            EventlogSourceName, EventLogName)
        End If
        Dim eventLog As New Diagnostics.EventLog
        eventLog.Source = EventlogSourceName
        Select Case LCase(type)
            Case "error", "e"
                eventLog.WriteEntry(messu, EventLogEntryType.Error)
            Case "warning", "w"
                eventLog.WriteEntry(messu, EventLogEntryType.Warning)
            Case Else
                eventLog.WriteEntry(messu, EventLogEntryType.Information)
        End Select

    End Sub
    Public Sub New(InitEventLogName As String, InitEventlogSourceName As String)
        EventLogName = InitEventLogName
        EventlogSourceName = InitEventlogSourceName
        'EventLog.DeleteEventSource("2") ' tämä holdasi koko palvelun
    End Sub
End Class
