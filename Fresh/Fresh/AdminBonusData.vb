Imports System.Data.OleDb

Public Class AdminBonusData

    Private Sub AdminBonusData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AdminInterface.Enabled = False

        LoadData()
    End Sub

    Private Sub LoadData()

        Dim EmployeeID As String = AdminInterface.AdminPayrollInstance.txtid.Text
        Dim payrollID As String = AdminInterface.AdminPayrollInstance.lblPayrollID.Text
        Dim query As String = "SELECT Bonus_ID, Payroll_ID, Amount, Reason FROM Bonus WHERE Payroll_ID = @payrollID"
        Dim dt As New DataTable()

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@payrollID", payrollID)

                Using adapter As New OleDbDataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using

            dgdisplay.DataSource = dt

            dgdisplay.Columns("Bonus_ID").Visible = False
            dgdisplay.Columns("Payroll_ID").Visible = False

        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        AdminInterface.Enabled = True

        Me.Dispose()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If String.IsNullOrWhiteSpace(txtAmount.Text) Or String.IsNullOrWhiteSpace(txtReason.Text) Then
            MessageBox.Show("Please enter both Amount and Reason.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim EmployeeID As String = AdminInterface.AdminPayrollInstance.txtid.Text
        Dim payrollID As String = AdminInterface.AdminPayrollInstance.lblPayrollID.Text
        Dim amount As Decimal = Convert.ToDecimal(txtAmount.Text)
        Dim reason As String = txtReason.Text
        Dim query As String = "INSERT INTO Bonus (Employee_ID, Payroll_ID, Amount, Reason, Selected) VALUES (@EmployeeID, @payrollID, @amount, @reason, True)"

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID)
                cmd.Parameters.AddWithValue("@payrollID", payrollID)
                cmd.Parameters.AddWithValue("@amount", amount)
                cmd.Parameters.AddWithValue("@reason", reason)

                cmd.ExecuteNonQuery()
            End Using

            txtReason.Text = ""
            txtAmount.Text = ""
            LoadData()

        Catch ex As Exception
            MessageBox.Show("Error saving data: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        If dgdisplay.SelectedRows.Count > 0 Then

            Dim BonusID As Integer = Convert.ToInt32(dgdisplay.SelectedRows(0).Cells("Bonus_ID").Value)
            Dim query As String = "DELETE FROM Bonus WHERE Bonus_ID = @BonusID"

            Try

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                Using cmd As New OleDbCommand(query, con)
                    cmd.Parameters.AddWithValue("@BonusID", BonusID)

                    cmd.ExecuteNonQuery()
                End Using

                LoadData()

            Catch ex As Exception
                MessageBox.Show("Error deleting data: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MessageBox.Show("Please select a row to delete.")
        End If
    End Sub

    Private Sub AdminBonusData_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        AdminInterface.Enabled = True
    End Sub
End Class
