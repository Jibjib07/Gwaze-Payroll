Imports System.Data.OleDb

Public Class Admin13thMonth
    Public Sub Load13thMonthData()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "
            SELECT employee_id, first_name, last_name, Department, Job, date_hired, last_bonus 
            FROM Employee 
            WHERE last_bonus IS NULL OR last_bonus < DateAdd('m', -13, Date())"
            Dim da As New OleDbDataAdapter(query, con)
            Dim dt As New DataTable
            da.Fill(dt)

            dt.Columns.Add("Name", GetType(String))
            For Each row As DataRow In dt.Rows
                row("Name") = row("first_name").ToString() & " " & row("last_name").ToString()
            Next

            dt.Columns.Remove("first_name")
            dt.Columns.Remove("last_name")

            dt.Columns("employee_id").ColumnName = "ID"
            dt.Columns("date_hired").ColumnName = "Date Hired"
            dt.Columns("last_bonus").ColumnName = "Last Bonus"

            dgv13thmonth.DataSource = dt

            dgv13thmonth.Columns("Last Bonus").Visible = False

            If dgv13thmonth.Columns("btnGiveBonus") Is Nothing Then
                Dim btnColumn As New DataGridViewButtonColumn()
                btnColumn.HeaderText = "Action"
                btnColumn.Name = "btnGiveBonus"
                btnColumn.Text = "Give Bonus"
                btnColumn.UseColumnTextForButtonValue = True
                dgv13thmonth.Columns.Add(btnColumn)
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub dgv13thmonth_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv13thmonth.CellContentClick
        If e.ColumnIndex = dgv13thmonth.Columns("btnGiveBonus").Index AndAlso e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = dgv13thmonth.Rows(e.RowIndex)
            Dim employeeID As String = selectedRow.Cells("ID").Value.ToString()

            Try
                Dim todayDate As String = Date.Now.ToString("MM/dd/yyyy")

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim updateCmd As New OleDbCommand("UPDATE Employee SET Last_Bonus = @LastBonus WHERE Employee_ID = @EmployeeID", con)
                updateCmd.Parameters.AddWithValue("@LastBonus", todayDate)
                updateCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                updateCmd.ExecuteNonQuery()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim payrollCmd As New OleDbCommand("SELECT Payroll_ID FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'", con)
                payrollCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                Dim payrollID As String = payrollCmd.ExecuteScalar()?.ToString()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If String.IsNullOrEmpty(payrollID) Then
                    MessageBox.Show("No unreleased payroll found for this employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim jobDeptCmd As New OleDbCommand("SELECT Job, Department FROM Employee WHERE Employee_ID = @EmployeeID", con)
                jobDeptCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                Dim reader As OleDbDataReader = jobDeptCmd.ExecuteReader()
                Dim job As String = String.Empty
                Dim department As String = String.Empty

                If reader.Read() Then
                    job = reader("Job").ToString()
                    department = reader("Department").ToString()
                End If
                reader.Close()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If String.IsNullOrEmpty(job) OrElse String.IsNullOrEmpty(department) Then
                    MessageBox.Show("Employee job or department not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim salaryCmd As New OleDbCommand("SELECT Salary FROM Job WHERE Job = @Job AND Department = @Department", con)
                salaryCmd.Parameters.AddWithValue("@Job", job)
                salaryCmd.Parameters.AddWithValue("@Department", department)
                Dim salary As String = salaryCmd.ExecuteScalar()?.ToString()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If String.IsNullOrEmpty(salary) Then
                    MessageBox.Show("Salary not found for the given job and department.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim insertCmd As New OleDbCommand("INSERT INTO Bonus (Payroll_ID, Employee_ID, Amount, Reason, Selected) VALUES (@PayrollID, @EmployeeID, @Amount, @Reason, @Selected)", con)
                insertCmd.Parameters.AddWithValue("@PayrollID", payrollID)
                insertCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                insertCmd.Parameters.AddWithValue("@Amount", salary)
                insertCmd.Parameters.AddWithValue("@Reason", "13th Month Pay")
                insertCmd.Parameters.AddWithValue("@Selected", True)
                insertCmd.ExecuteNonQuery()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                MessageBox.Show("13th-month bonus successfully granted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Load13thMonthData()

            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub btngive_Click(sender As Object, e As EventArgs) Handles btngive.Click
        Try
            Dim todayDate As String = Date.Now.ToString("MM/dd/yyyy")

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim fetchEligibleCmd As New OleDbCommand("SELECT Employee.Employee_ID, Employee.Job, Employee.Department, Employee.Last_Bonus 
                                                  FROM Employee 
                                                  WHERE DateDiff('m', Last_Bonus, Date()) >= 13 OR Last_Bonus IS NULL", con)
            Dim reader As OleDbDataReader = fetchEligibleCmd.ExecuteReader()

            Dim eligibleEmployees As New List(Of Dictionary(Of String, String))

            While reader.Read()
                Dim employeeData As New Dictionary(Of String, String)
                employeeData("Employee_ID") = reader("Employee_ID").ToString()
                employeeData("Job") = reader("Job").ToString()
                employeeData("Department") = reader("Department").ToString()
                eligibleEmployees.Add(employeeData)
            End While
            reader.Close()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If eligibleEmployees.Count = 0 Then
                MessageBox.Show("No eligible employees for 13th-month pay.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            For Each employee In eligibleEmployees
                Dim employeeID = employee("Employee_ID")
                Dim job = employee("Job")
                Dim department = employee("Department")
                Dim salary As String = ""
                Dim payrollID As String = ""

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim salaryCmd As New OleDbCommand("SELECT Salary FROM Job WHERE Job = @Job AND Department = @Department", con)
                salaryCmd.Parameters.AddWithValue("@Job", job)
                salaryCmd.Parameters.AddWithValue("@Department", department)
                salary = salaryCmd.ExecuteScalar()?.ToString()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If String.IsNullOrEmpty(salary) Then
                    MessageBox.Show("Salary not found for Employee_ID: " & employeeID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Continue For
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim payrollCmd As New OleDbCommand("SELECT Payroll_ID FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'", con)
                payrollCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                payrollID = payrollCmd.ExecuteScalar()?.ToString()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If String.IsNullOrEmpty(payrollID) Then
                    MessageBox.Show("No unreleased payroll found for Employee_ID: " & employeeID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Continue For
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim insertBonusCmd As New OleDbCommand("INSERT INTO Bonus (Payroll_ID, Employee_ID, Amount, Reason, Selected) 
                                                    VALUES (@PayrollID, @EmployeeID, @Amount, @Reason, @Selected)", con)
                insertBonusCmd.Parameters.AddWithValue("@PayrollID", payrollID)
                insertBonusCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                insertBonusCmd.Parameters.AddWithValue("@Amount", salary)
                insertBonusCmd.Parameters.AddWithValue("@Reason", "13th Month Pay")
                insertBonusCmd.Parameters.AddWithValue("@Selected", False) ' Default Selected to No
                insertBonusCmd.ExecuteNonQuery()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim updateBonusCmd As New OleDbCommand("UPDATE Employee SET Last_Bonus = @LastBonus WHERE Employee_ID = @EmployeeID", con)
                updateBonusCmd.Parameters.AddWithValue("@LastBonus", todayDate)
                updateBonusCmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                updateBonusCmd.ExecuteNonQuery()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            Next

            MessageBox.Show("13th-month bonus successfully granted to all eligible employees.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Load13thMonthData()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

End Class