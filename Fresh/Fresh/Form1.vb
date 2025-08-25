Imports System.Collections.ObjectModel
Imports System.Data.OleDb
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ApplicationServices


Public Class Form1
    Dim Client As New ClientDashboard

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        UpdatePendingStatus()
    End Sub
    Public Sub UpdatePendingStatus()
        Try
            Dim query As String = "UPDATE Payroll SET Payroll_Status = 'Pending' " &
                           "WHERE Date_Due < @CurrentDate AND Payroll_Status = 'Unreleased'"

            Dim currentDate As Date = DateTime.Now.Date


            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.Add("@CurrentDate", OleDbType.Date).Value = currentDate

                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MsgBox($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub txtUsernameandPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtEID.KeyDown, txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.KeyCode = Keys.Enter Then
                e.Handled = True

                Login_Click(sender, e)
            End If
        End If

    End Sub

    Private Sub txtEID_GotFocus(sender As Object, e As EventArgs) Handles txtEID.GotFocus
        If txtEID.Text = "Employee ID" Then
            txtEID.Text = ""
            txtEID.ForeColor = Color.Black
        End If
    End Sub

    Private Sub txtPassword_GotFocus(sender As Object, e As EventArgs) Handles txtPassword.GotFocus
        If txtPassword.Text = "Password" Then
            txtPassword.Text = ""
            txtPassword.ForeColor = Color.Black
            txtPassword.PasswordChar = "*"
        End If
    End Sub
    Private Sub txtEID_LostFocus(sender As Object, e As EventArgs) Handles txtEID.LostFocus
        If txtEID.Text = "" Then
            txtEID.Text = "Employee ID"
            txtEID.ForeColor = Color.FromArgb(200, 200, 200)
        End If
    End Sub
    Private Sub txtPassword_LostFocus(sender As Object, e As EventArgs) Handles txtPassword.LostFocus
        If txtPassword.Text = "" Then
            txtPassword.Text = "Password"
            txtPassword.ForeColor = Color.FromArgb(200, 200, 200)
            txtPassword.PasswordChar = ""
        End If
    End Sub
    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged
        If txtPassword.Text = "Password" Then
            btnshow.Visible = False
        Else
            btnshow.Visible = True
        End If
    End Sub
    Private Sub btnShow_Click(sender As Object, e As EventArgs) Handles btnshow.Click
        If txtPassword.PasswordChar = "*" Then
            btnshow.IconChar = FontAwesome.Sharp.IconChar.Eye
            txtPassword.PasswordChar = ControlChars.NullChar
        ElseIf txtPassword.PasswordChar = ControlChars.NullChar Then
            btnshow.IconChar = FontAwesome.Sharp.IconChar.EyeSlash
            txtPassword.PasswordChar = "*"
        End If
    End Sub

    Private Sub Login_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        If con.State = ConnectionState.Closed Then
            con.Open
        End If

        If txtEID.Text = "Employee ID" Or txtPassword.Text = "Password" Then
            lblerroruser.Text = "Invalid Employee ID or Password."
            lblerroruser.Show
        Else
            Dim EID = txtEID.Text
            Dim password = txtPassword.Text

            If CredentialCheck(EID, password) Then
                If IsAdmin(EID) Then
                    txtEID.Text = "Employee ID"
                    txtEID.ForeColor = Color.FromArgb(200, 200, 200)
                    txtPassword.Text = "Password"
                    txtPassword.ForeColor = Color.FromArgb(200, 200, 200)
                    txtPassword.PasswordChar = ""
                    btnshow.IconChar = FontAwesome.Sharp.IconChar.EyeSlash

                    lblerroruser.Hide
                    Admininterface.Show
                    Hide

                Else
                    Dim name = nameget(EID, password)
                    Dim ID = IDGet(EID, password)

                    Maininterface.lblname.Text = name
                    Maininterface.lblID.Text = ID

                    txtEID.Text = "Employee ID"
                    txtEID.ForeColor = Color.FromArgb(200, 200, 200)
                    txtPassword.Text = "Password"
                    txtPassword.ForeColor = Color.FromArgb(200, 200, 200)
                    txtPassword.PasswordChar = ""
                    btnshow.IconChar = FontAwesome.Sharp.IconChar.EyeSlash

                    lblerroruser.Hide
                    Maininterface.Show
                    Hide

                End If
                Exit Sub
            Else
                lblerroruser.Text = "Wrong username or password."
                lblerroruser.Show
            End If

            con.Close
        End If
    End Sub
    Private Function CredentialCheck(eid As String, password As String) As Boolean
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Using command As New OleDbCommand("SELECT [PASSWORD] FROM Employee WHERE [Employee_ID] = @eid", con)
            command.Parameters.AddWithValue("@eid", eid)

            Dim dbPassword As Object = command.ExecuteScalar()

            If dbPassword IsNot Nothing Then
                Return String.Compare(password, dbPassword.ToString(), StringComparison.Ordinal) = 0
            End If

            Return False
        End Using
    End Function
    Private Sub btnexit_Click(sender As Object, e As EventArgs) Handles btnexit.Click
        System.Windows.Forms.Application.Exit()
    End Sub

    Private Sub btnminimize_Click(sender As Object, e As EventArgs) Handles btnminimize.Click
        WindowState = FormWindowState.Minimized
    End Sub


    'NAME DB GET
    Private Function nameget(eid As String, password As String) As String
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Using command As New OleDbCommand("SELECT First_Name FROM Employee WHERE [Employee_ID] = @eid AND [PASSWORD] = @password", con)
            command.Parameters.AddWithValue("@eid", eid)
            command.Parameters.AddWithValue("@password", password)

            Dim name As String = CStr(command.ExecuteScalar())
            Return name
            con.Close()
        End Using
    End Function
    'ID DB GET
    Private Function IDGet(eid As String, password As String) As String
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Using command As New OleDbCommand("SELECT Employee_ID FROM Employee WHERE [Employee_ID] = @eid AND [PASSWORD] = @password", con)
            command.Parameters.AddWithValue("@eid", eid)
            command.Parameters.AddWithValue("@password", password)

            Dim ID As String = CStr(command.ExecuteScalar())
            Return ID
            con.Close()
        End Using
    End Function
    Private Function IsAdmin(username As String) As Boolean
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Using command As New OleDbCommand("SELECT Type FROM Employee WHERE [Employee_ID] = @eid", con)
            command.Parameters.AddWithValue("@eid", username)

            Dim result As String = Convert.ToString(command.ExecuteScalar())

            Return result = "Admin"
            con.Close()
        End Using
    End Function
End Class
