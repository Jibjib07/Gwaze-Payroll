Imports System.Data.OleDb
Imports System.IO
Imports System.Security.Policy

Public Class ClientPayroll
    Private Sub ClientPayroll_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RetrievePayrollData()
        lblWorkinghour_timer.Enabled = True
        lbldate.Text = DateTime.Now.ToString("MM/dd/yyyy")
        PopulateDeductionData()
        GetBonusData()
    End Sub
    Private Sub ClientPayroll_GotFocus(sender As Object, e As EventArgs) Handles MyBase.GotFocus
        lblWorkinghour_timer.Enabled = True
    End Sub
    Private Sub ClientPayroll_LostFocus(sender As Object, e As EventArgs) Handles MyBase.LostFocus
        lblWorkinghour_timer.Enabled = False
    End Sub
    Private Sub lblWorkinghour_timer_Tick(sender As Object, e As EventArgs) Handles lblWorkinghour_timer.Tick
        StatusUpdate()
        If lblactivestatus.Text = "Active" Then
            lblactivestatus.ForeColor = Color.Lime
        ElseIf lblactivestatus.Text = "Break" Then
            lblactivestatus.ForeColor = Color.Orange
        ElseIf lblactivestatus.Text = "Recorded" Then
            lblactivestatus.ForeColor = Color.White
        End If
    End Sub
    Private Sub TotalOT(sender As Object, e As EventArgs) Handles txtotrate.TextChanged, txtotworked.TextChanged
        Dim otTotal = Val(txtotrate.Text) * Val(txtotworked.Text)
        txtott.Text = otTotal.ToString("0.00")
    End Sub

    Private Sub TotalPay(sender As Object, e As EventArgs) Handles txtpayrate.TextChanged, txtHoursworked.TextChanged
        Dim paytotal = Val(txtpayrate.Text) * Val(txtHoursworked.Text)
        txtpayrateT.Text = paytotal.ToString("0.00")

    End Sub

    Private Sub TotalContribution(sender As Object, e As EventArgs) Handles txtincometax.TextChanged, txtphil.TextChanged, txtsss.TextChanged
        txtcontribution.Text = Val(txtincometax.Text) + Val(txtphil.Text) + Val(txtsss.Text)
    End Sub

    Private Sub TotalGross(sender As Object, e As EventArgs) Handles txtott.TextChanged, txtpayrateT.TextChanged
        Dim totalgross = Val(txtott.Text) + Val(txtpayrateT.Text)
        txtgrosssalary.Text = totalgross.ToString("0.00")
    End Sub
    Private Sub TotalDeduction(sender As Object, e As EventArgs) Handles txtdeduct.TextChanged, txtcontribution.TextChanged, txttotaldeduct.TextChanged, txtbonust.TextChanged

        Dim totalDeductions = Val(txtcontribution.Text) + Val(txtdeduct.Text)
        Dim net = Val(txtbonust.Text) + Val(txtgrosssalary.Text) - Val(txttotaldeduct.Text)
        txtnetsalary.Text = net.ToString("0.00")
    End Sub
    Private Sub txtGrossSal_TextChanged(sender As Object, e As EventArgs) Handles txtgrosssalary.TextChanged, txttotaldeduct.TextChanged, txtdeduct.TextChanged

        Dim grossSalary = Val(txtgrosssalary.Text)
        Dim withholdingTax = 0.0
        Dim philHealth = 0.0
        Dim sss = 0.0
        Dim adddeduct = Val(txtdeduct.Text)

        If grossSalary <= 10416.5 Then
            withholdingTax = 0
        ElseIf grossSalary <= 16666 Then
            withholdingTax = 0.15 * (grossSalary - 10416.5)
        ElseIf grossSalary <= 33333 Then
            withholdingTax = 937.5 + 0.2 * (grossSalary - 16666.5)
        ElseIf grossSalary <= 83333 Then
            withholdingTax = 4270.9 + 0.25 * (grossSalary - 33333.5)
        ElseIf grossSalary <= 333333 Then
            withholdingTax = 16770.9 + 0.3 * (grossSalary - 83333.5)
        Else
            withholdingTax = 91770.9 + 0.35 * (grossSalary - 333333.5)
        End If


        Dim philHealthRate = 0.025
        philHealth = grossSalary * philHealthRate

        Dim sssRate = 0.045
        Dim maxSssSalary As Double = 30_000
        If grossSalary > maxSssSalary Then
            sss = maxSssSalary * sssRate
        Else
            sss = grossSalary * sssRate
        End If

        txtincometax.Text = withholdingTax.ToString("0.00")
        txtphil.Text = philHealth.ToString("0.00")
        txtsss.Text = sss.ToString("0.00")

        Dim totalDeductions = withholdingTax + philHealth + sss + adddeduct
        txttotaldeduct.Text = totalDeductions.ToString("0.00")
        Dim net = Val(txtgrosssalary.Text) - Val(txttotaldeduct.Text)
        txtnetsalary.Text = net.ToString("0.00")
    End Sub

    Private Sub StatusUpdate()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim selectedID As String = MainInterface.lblID.Text
            Dim currentdate As String = lbldate.Text

            Dim query As String = "SELECT Active_Time, Status FROM CLocking_Log WHERE Employee_ID = @Employee_ID and Work_Date = @date"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@EmployeeID", selectedID)
            command.Parameters.AddWithValue("Work_Date", currentdate)

            Dim reader As OleDbDataReader = command.ExecuteReader()

            If reader.HasRows Then
                reader.Read()
                lblWorkingHours.Text = reader("Active_Time").ToString()
                lblactivestatus.Text = reader("Status").ToString()

                reader.Close()
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Public Sub RetrievePayrollData()

        Dim employeeid As Integer = CInt(MainInterface.lblID.Text)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Photo FROM Employee WHERE Employee_ID = @EmployeeID"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@EmployeeID", employeeid)

            Dim photo As Object = command.ExecuteScalar()

            If photo IsNot DBNull.Value AndAlso photo IsNot Nothing Then
                Dim photoBytes As Byte() = CType(photo, Byte())
                Using ms As New MemoryStream(photoBytes)
                    PictureBox1.Image = Image.FromStream(ms)
                    PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                End Using
            Else
                PictureBox1.Image = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Payroll_ID, Employee_Name, Employee_ID, Department, Job, Pay_rate, Hours_Worked, OT_rate, OT_Worked, Philhealth, tax, sss
                               FROM Payroll 
                               WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'"

            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID)

                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        lblPayrollID.Text = reader("Payroll_ID").ToString()
                        txtJob.Text = reader("Job").ToString()
                        txtDepartment.Text = reader("Department").ToString()
                        txtname.Text = reader("Employee_Name").ToString()
                        txtid.Text = reader("Employee_ID").ToString()
                        txtpayrate.Text = reader("Pay_rate").ToString()
                        txtHoursworked.Text = reader("Hours_Worked").ToString()
                        txtotrate.Text = reader("OT_Rate").ToString()
                        txtotworked.Text = reader("OT_Worked").ToString()

                        txtotrate.Text = Val(txtpayrate.Text) * 1.25
                    Else
                        MessageBox.Show("No unreleased payroll found for this employee.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using

        Catch ex As Exception

            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub PopulateDeductionData()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim payrollID As String = lblPayrollID.Text

            Dim query As String = "SELECT Deduction_ID, Payroll_ID, Amount, Reason, Selected FROM Deductions WHERE Payroll_ID = @PayrollID"

            Dim dataTable As New DataTable()

            Using adapter As New OleDbDataAdapter(query, con)
                adapter.SelectCommand.Parameters.AddWithValue("@PayrollID", payrollID)
                adapter.Fill(dataTable)
            End Using

            dgdeduction.DataSource = dataTable
            If dgdeduction.Columns("Deduction_ID") IsNot Nothing Then
                dgdeduction.Columns("Deduction_ID").Visible = False
            End If
            If dgdeduction.Columns("Payroll_ID") IsNot Nothing Then
                dgdeduction.Columns("Payroll_ID").Visible = False
            End If

            If dgdeduction.Columns("Selected") Is Nothing Then
                Dim checkBoxColumn As New DataGridViewCheckBoxColumn()
                checkBoxColumn.DataPropertyName = "Selected"
                checkBoxColumn.HeaderText = "Select"
                checkBoxColumn.Name = "Selected"
                checkBoxColumn.Width = 50
                dgdeduction.Columns.Add(checkBoxColumn)
            End If

            For Each column As DataGridViewColumn In dgdeduction.Columns
                If column.Name <> "Selected" Then
                    column.ReadOnly = True
                End If
            Next

            dgdeduction.AllowUserToAddRows = False

            CalculateTotalDeduction(dataTable)
        Catch ex As Exception
            MessageBox.Show("An error occurred in GetDeductionData: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub CalculateTotalDeduction(dataTable As DataTable)
        Dim totalDeduction As Decimal = 0
        Dim grossSalary As Decimal = Convert.ToDecimal(txtgrosssalary.Text)
        Dim deductionLimit As Decimal = grossSalary * 0.2

        Dim sortedRows = dataTable.Select("", "Amount ASC")

        For Each row As DataRow In sortedRows
            If Not IsDBNull(row("Selected")) AndAlso Convert.ToBoolean(row("Selected")) Then
                Dim amount As Decimal = 0
                If Not IsDBNull(row("Amount")) Then
                    amount = Convert.ToDecimal(row("Amount"))
                End If

                If totalDeduction + amount > deductionLimit Then
                    Dim remainingLimit As Decimal = deductionLimit - totalDeduction

                    If remainingLimit > 0 Then
                        totalDeduction += remainingLimit
                    End If

                    Exit For
                Else
                    totalDeduction += amount
                End If
            End If
        Next
        txtdeduct.Text = totalDeduction.ToString("F2")
    End Sub

    Public Sub GetBonusData()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim payrollID As String = lblPayrollID.Text

            Dim query As String = "SELECT Bonus_ID, Payroll_ID, Amount, Reason, Selected FROM Bonus WHERE Payroll_ID = @PayrollID"

            Dim dataTable As New DataTable()

            Using adapter As New OleDbDataAdapter(query, con)
                adapter.SelectCommand.Parameters.AddWithValue("@PayrollID", payrollID)
                adapter.Fill(dataTable)
            End Using

            dgbonus.DataSource = dataTable

            If dgbonus.Columns("Bonus_ID") IsNot Nothing Then
                dgbonus.Columns("Bonus_ID").Visible = False
            End If
            If dgbonus.Columns("Payroll_ID") IsNot Nothing Then
                dgbonus.Columns("Payroll_ID").Visible = False
            End If

            If dgbonus.Columns("Selected") Is Nothing Then
                Dim checkBoxColumn As New DataGridViewCheckBoxColumn()
                checkBoxColumn.DataPropertyName = "Selected"
                checkBoxColumn.HeaderText = "Select"
                checkBoxColumn.Name = "Selected"
                checkBoxColumn.Width = 50
                dgbonus.Columns.Add(checkBoxColumn)
            End If

            For Each column As DataGridViewColumn In dgbonus.Columns
                If column.Name <> "Selected" Then
                    column.ReadOnly = True
                End If
            Next

            dgBonus.AllowUserToAddRows = False

            CalculateTotalBonus(dataTable)
        Catch ex As Exception
            MessageBox.Show("An error occurred in GetBonusData: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub CalculateTotalBonus(dataTable As DataTable)
        Dim totalBonus As Decimal = 0

        For Each row As DataRow In dataTable.Rows
            If Not IsDBNull(row("Selected")) AndAlso Convert.ToBoolean(row("Selected")) Then
                If Not IsDBNull(row("Amount")) Then
                    totalBonus += Convert.ToDecimal(row("Amount"))
                End If
            End If
        Next

        txtbonust.Text = totalBonus.ToString("F2")
    End Sub


End Class