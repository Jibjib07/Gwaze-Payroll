Imports System.Data.OleDb

Public Class ClientSetting
    Private connectionString As String

    Private Sub ClientSetting(sender As Object, e As EventArgs) Handles MyBase.Load
        txtemployeeid.ReadOnly = True

        txtemployeeid.Text = MainInterface.lblID.Text
        UpdateEmployee(txtemployeeid.Text)
    End Sub
    Private Sub UpdateEmployee(employeeid As String)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT First_Name, Last_Name, [Password], Phone_Num, Email FROM Employee WHERE Employee_ID = @Employee_ID"
            Using command As New OleDbCommand(query, con)
                command.Parameters.AddWithValue("@Employee_ID", employeeid)

                Using reader As OleDbDataReader = command.ExecuteReader()
                    If reader.Read() Then

                        txtfirstname.Text = reader("First_Name").ToString()
                        txtlastname.Text = reader("Last_Name").ToString()
                        txtcontact.Text = reader("Phone_Num").ToString()
                        txtemail.Text = reader("Email").ToString()
                    Else
                        MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading employee details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        If String.IsNullOrWhiteSpace(txtoldpassword.Text) OrElse
      String.IsNullOrWhiteSpace(txtnewpassword.Text) OrElse
      String.IsNullOrWhiteSpace(txtconfirmpassword.Text) Then
            MessageBox.Show("All password fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If txtnewpassword.Text <> txtconfirmpassword.Text Then
            MessageBox.Show("New Password and Confirm Password do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim queryCheck As String = "SELECT [Password] FROM Employee WHERE Employee_ID = @Employee_ID"
            Using commandCheck As New OleDbCommand(queryCheck, con)
                commandCheck.Parameters.AddWithValue("@Employee_ID", txtemployeeid.Text)

                Dim currentPassword As String = commandCheck.ExecuteScalar()?.ToString()
                If currentPassword Is Nothing Then
                    MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                If txtoldpassword.Text <> currentPassword Then
                    MessageBox.Show("Old Password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End Using


            Dim queryUpdate As String = "UPDATE Employee SET [Password] = @NewPassword WHERE Employee_ID = @Employee_ID"
            Using commandUpdate As New OleDbCommand(queryUpdate, con)
                commandUpdate.Parameters.AddWithValue("@NewPassword", txtnewpassword.Text)
                commandUpdate.Parameters.AddWithValue("@EmployeeID", txtemployeeid.Text)

                Dim rowsAffected As Integer = commandUpdate.ExecuteNonQuery()
                If rowsAffected > 0 Then
                    MessageBox.Show("Password updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    txtoldpassword.Clear()
                    txtnewpassword.Clear()
                    txtconfirmpassword.Clear()
                Else
                    MessageBox.Show("Failed to update password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating password: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub txtemployeeid_TextChanged(sender As Object, e As EventArgs) Handles txtemployeeid.TextChanged

    End Sub
End Class