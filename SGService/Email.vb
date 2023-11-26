Imports System.Net.Mail
Imports System.Runtime.Remoting
Imports SGService.Settings
Public Class Email
    Private Email As SGService.Settings.Email
    Public Sub New(settings As SGService.Settings.Email)
        Email = New Settings.Email
        Email.ServerName = settings.ServerName
        Email.ServerPort = settings.ServerPort
        Email.SslEnabled = settings.SslEnabled
        Email.AccountName = settings.AccountName
        Email.AccountPassword = settings.AccountPassword
        Email.SenderAddress = settings.SenderAddress
    End Sub
    ' get name part of a mail address
    Public Function NameOfMailAddress(address As String) As String
        Dim name As String = address
        If address.IndexOf("@") > 1 Then name = address.Substring(0, address.IndexOf("@"))
        Return name
    End Function
    ' send email
    Public Function sendEmail(addressList As String, subject As String, messageContent As String) As String
        Try
            If (addressList Is Nothing) Then
                Return ""
            End If
            Dim sender As System.Net.Mail.MailAddress = New MailAddress(Email.SenderAddress, Email.SenderName)
            ' recipients:
            Dim recipientlist As String() = addressList.Split(",")
            If (recipientlist.Length = 0) Then Return ""
            Dim Recipient0 As String = recipientlist(0).Trim
            Dim recipient As MailAddress = New MailAddress(Recipient0, NameOfMailAddress(Recipient0))
            Dim message As MailMessage = New MailMessage(sender, recipient)
            For Each recipientString As String In recipientlist
                recipientString = recipientString.Trim
                If (recipientString <> Recipient0) Then
                    recipient = New MailAddress(recipientString, NameOfMailAddress(recipientString))
                    message.To.Add(recipient)
                End If
            Next
            ' muut mail-parametrit:
            message.Subject = subject
            Dim client As New SmtpClient(Email.ServerName)
            client.Credentials = New System.Net.NetworkCredential(Email.AccountName, Email.AccountPassword)
            If Email.ServerPort > "" Then client.Port = Email.ServerPort
            If Email.SslEnabled Then client.EnableSsl = True
            message.Body = messageContent & Environment.NewLine
            message.IsBodyHtml = False

            ' Send:
            client.Send(message)
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function


End Class
