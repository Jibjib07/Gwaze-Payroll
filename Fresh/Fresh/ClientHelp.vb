Imports System.Net.Mail

Public Class ClientHelp
    Private Sub btnsubmit_Click(sender As Object, e As EventArgs) Handles btnsubmit.Click
        Try

            Dim recipient As String = "gwazecompany@gmail.com"
            Dim senderEmail As String = "gwazecompany@gmail.com"
            Dim senderPassword As String = "tdhn vwpb jwdk dvjf"


            Dim senderName As String = txtname.Text
            Dim messageContent As String = txtmessagebox.Text
            Dim contactNumber As String = txtcontact.Text


            Dim formattedMessage As String = $"Good Day Ma'am/Sir, I am {senderName}," & vbCrLf & vbCrLf &
                                             $"My Concern is: " & vbCrLf & vbCrLf &
                                             $"({messageContent})" & vbCrLf & vbCrLf &
                                             $"({contactNumber})"


            Dim mail As New MailMessage()
            mail.From = New MailAddress(senderEmail, "Company Employee")
            mail.To.Add(recipient)
            mail.Subject = $"Message from {senderName}"
            mail.Body = formattedMessage


            Dim smtp As New SmtpClient("smtp.gmail.com")
            smtp.Port = 587
            smtp.Credentials = New Net.NetworkCredential(senderEmail, senderPassword)
            smtp.EnableSsl = True


            smtp.Send(mail)
            MessageBox.Show("Email sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtname.Text = ""
            txtcontact.Text = ""
            txtmessagebox.Text = ""
        Catch ex As Exception
            MessageBox.Show("Failed to send email. Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
