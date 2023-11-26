Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class Settings
    Public AllSettings As General
    Public Class General
        <DefaultValue(30)>
        Public Property MainLoopFrequenceSeconds As Integer
        Public Property Email As Email
        Public Property EventLog As EventLog
        Public Property TaskErrorEmailAddresslist As String
        Public Property FileLog As FileLog
        <DefaultValue("C:\\Windows\\system32\\cmd.exe")>
        Public Property WindowsCmdExecutablePath As String
        Public Property NasComm As NasComm
        Public Property TaskList() As Task()
    End Class
    Public Class Email
        Public Property TaskErrorAddresslist As String
        Public Property ServerName As String
        Public Property ServerPort As String
        Public Property SslEnabled As Boolean
        Public Property AccountName As String
        Public Property AccountPassword As String
        <DefaultValue("noreply@systemsgarden.com")>
        Public Property SenderAddress As String
        <DefaultValue("SGService")>
        Public Property SenderName As String
    End Class
    Public Class EventLog
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        <DefaultValue("SystemsGarden")>
        Public Property Name As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        <DefaultValue("SystemsGardenService")>
        Public Property SourceName As String
    End Class
    Public Class FileLog
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FolderPath As String
        <DefaultValue(True)>
        Public Property OnlyErrors As Boolean
    End Class
    Public Class ProcessResult
        Public Property Success As Boolean
        Public Property ProcessLog As String
        Public Property ProcessError As String
        Public Property ProcessWarning As String
        Public Property ProcessInfo As String
        Public Sub New()
            Me.Success = False
            Me.ProcessLog = ""
            Me.ProcessError = ""
            Me.ProcessWarning = ""
        End Sub
    End Class
    Public Class Task
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Name As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Description As String
        <DefaultValue(True)>
        Public Property Enabled As Boolean
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Program As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Command As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property CommandParameter As String
        <JsonProperty(Required:=True)>
        Public Property CleanUpSetting As CleanupSetting
        Public Property Schedule As Schedule
        Public Property NextRun As DateTime
        Public Property Result As ProcessResult
    End Class
    Public Class Schedule
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Type As String
        Public Property FileSetting As FileSetting
        Public Property TimeSetting As TimeSetting
    End Class
    Public Class CleanupSetting
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FileMask As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FolderList As String
        Public Property RetainDays As Integer
        Public Property RetainHours As Integer
        Public Property SearchSubDirectories As Boolean
    End Class
    Public Class FileSetting
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property SpoolFolder As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FileMask As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ProcessFolder As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ProcessedFolder As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FailedFolder As String
        Public Property FileName As String
    End Class
    Public Class TimeSetting
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property DayOfMonthList As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property DayOfWeekList As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property HourList As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property MinuteList As String
    End Class
    Public Class NasComm
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property AssemblyFileName As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClientType As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClusterPath As String
        <RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClientName As String

    End Class
    Sub New(folder As String, fileName As String)
        Dim settingFileName As String = folder & "\" & fileName
        Dim json As Object = File.ReadAllText(settingFileName)

        AllSettings = JsonConvert.DeserializeObject(Of General)(json)

        ' check fields in settings to make informative logging in case of a fault
        checkSettings(AllSettings)
    End Sub
    ' check fields in settings to make informative logging in case of a fault
    Sub checkSettings(settings As General)
        If settings.EventLog Is Nothing Then
            Throw New Exception("Eventlog information missing in settings.")
        End If
    End Sub
End Class
