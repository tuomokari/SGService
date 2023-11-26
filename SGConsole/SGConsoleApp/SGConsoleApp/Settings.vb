Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class Settings
    Public AllSettings As General
    Public Class General
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ServiceRunFolder As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property LoopFrequenceSeconds As Integer
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Email As Email
        Public Property EventLogName As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property EventlogSourceName As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FileLogFolder As String
        Public Property CMD As String

        Public Property NasComm As NasComm
        <JsonProperty(Required:=True)>
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
        Public Property SenderName As String
    End Class
    Public Class Task
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Name As String
        Public Property Description As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Program As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Command As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property CommandParameter As String
        <JsonProperty(Required:=True)>
        Public Property Schedule As Schedule
        Public Property NextRun As DateTime
        Public Property ProcessOuput As String


    End Class
    Public Class Schedule
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property Type As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FileSetting As FileSetting
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property TimeSetting As TimeSetting
    End Class
    Public Class FileSetting
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property SpoolFolder As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property FileMask As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ProcessFolder As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ProcessedFolder As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
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
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property AssemblyFileName As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClientType As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClusterPath As String
        <JsonProperty(Required:=True), RegularExpression("^[A-Za-zåÅäÄöÖ0-9_, \\-!:/.@\r\n?+*&€]+$")>
        Public Property ClientName As String

    End Class
    Sub New(folder As String, fileName As String)
        Dim settingFileName As String = folder & "\" & fileName
        Dim json As Object = File.ReadAllText(settingFileName)
        'Dim options As JsonSerializerOptions = New JsonSerializerOptions
        'options.PropertyNameCaseInsensitive = True

        AllSettings = JsonConvert.DeserializeObject(Of General)(json)
        'AllSettings = JsonSerializer.Deserialize(Of General)(json, options)

    End Sub

End Class
