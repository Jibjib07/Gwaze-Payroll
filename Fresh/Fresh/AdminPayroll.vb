Imports System.Data.OleDb
Imports System.IO
Imports System.IO.Packaging
Imports System.Runtime.Intrinsics.X86
Imports System.Windows.Media
Imports Guna.UI2.WinForms

Public Class AdminPayroll

    Private Sub AdminPayroll_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub
    Private Sub TotalOT(sender As Object, e As EventArgs) Handles txtotrate.TextChanged, txtotworked.TextChanged
        Dim otTotal = Val(txtotrate.Text) * Val(txtotworked.Text)
        txtott.Text = otTotal.ToString("0.00")
    End Sub

    Private Sub TotalPay(sender As Object, e As EventArgs) Handles txtpayrate.TextChanged, txtHoursworked.TextChanged
        Dim paytotal = Val(txtpayrate.Text) * Val(txtHoursworked.Text)
        txtpayrateT.Text = paytotal.ToString("0.00")

        txtotrate.Text = Val(txtpayrate.Text) * 1.25
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
        txttotaldeduct.Text = totalDeductions.ToString("0.00")
        txtnetsalary.Text = Nothing
        Dim net = Val(txtbonust.Text) + Val(txtgrosssalary.Text) - Val(txttotaldeduct.Text)
        txtnetsalary.Text = net.ToString("0.00")
    End Sub

    Private Sub txtGrossSal_TextChanged(sender As Object, e As EventArgs) Handles txtgrosssalary.TextChanged

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

    Private Sub dgemployeelist_SelectionChanged(sender As Object, e As EventArgs) Handles dgEmployeelist.SelectionChanged
        If btnCheck.Text = "Check" Then
            txtHoursworked.Text = "0"
            txtotworked.Text = "0"
            If dgEmployeelist.SelectedRows.Count > 0 Then
                GetEmployeeData()
                GetBonusData()
                GetDeductionData()
                UpdatePayroll()
            End If
        Else
            txtHoursworked.Text = "0"
            txtotworked.Text = "0"
            If dgEmployeelist.SelectedRows.Count > 0 Then
                GetPendingData()
                GetBonusData()
                GetDeductionData()
                UpdatePayroll()
            End If
        End If

    End Sub
    Private Sub GetEmployeeData()
        Dim selectedEmployeeID As Integer = CInt(dgEmployeelist.SelectedRows(0).Cells("Employee_ID").Value)
        Dim selectedEmployeeName As String = dgEmployeelist.SelectedRows(0).Cells("FullName").Value.ToString()
        Dim selectedJob As String = dgEmployeelist.SelectedRows(0).Cells("Job").Value.ToString()
        Dim selectedDepartment As String = dgEmployeelist.SelectedRows(0).Cells("Department").Value.ToString()
        txtJob.Text = selectedJob
        txtdepartment.Text = selectedDepartment

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Photo FROM Employee WHERE Employee_ID = @EmployeeID"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID)

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

            Dim queryCheckPayroll As String = "SELECT * FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'"
            Dim commandCheckPayroll As New OleDbCommand(queryCheckPayroll, con)
            commandCheckPayroll.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID)

            Dim payrollReader As OleDbDataReader = commandCheckPayroll.ExecuteReader()

            If payrollReader.HasRows Then

                While payrollReader.Read()
                    lblPayrollID.Text = payrollReader("Payroll_ID").ToString()
                    txtName.Text = payrollReader("Employee_Name").ToString()
                    txtid.Text = payrollReader("Employee_ID").ToString()
                    txtpayrate.Text = payrollReader("Pay_rate").ToString()
                    txtHoursworked.Text = payrollReader("Hours_Worked").ToString()
                    txtotworked.Text = payrollReader("OT_Worked").ToString()
                    lbldatecreated.Text = Convert.ToDateTime(payrollReader("Date_Created")).ToString("MM/dd/yyyy")
                    lbldatedue.Text = Convert.ToDateTime(payrollReader("Date_Due")).ToString("MM/dd/yyyy")
                End While
                payrollReader.Close()

                Dim salary As Decimal = 0
                Dim anual As Decimal = 0
                Dim payRate As Decimal = 0
                Dim jobTitle As String = String.Empty

                Dim queryPayRate As String = "SELECT Job, Salary, Anual, Pay_rate FROM Job WHERE Job = @Job AND Department = @Department"
                Dim commandPayRate As New OleDbCommand(queryPayRate, con)
                commandPayRate.Parameters.AddWithValue("@Job", selectedJob)
                commandPayRate.Parameters.AddWithValue("@Department", selectedDepartment)

                Dim jobReader As OleDbDataReader = commandPayRate.ExecuteReader()

                If jobReader.HasRows Then
                    While jobReader.Read()
                        jobTitle = If(IsDBNull(jobReader("Job")), String.Empty, jobReader("Job").ToString())
                        salary = If(IsDBNull(jobReader("Salary")), 0, CDec(jobReader("Salary")))
                        anual = If(IsDBNull(jobReader("Anual")), 0, CDec(jobReader("Anual")))
                        payRate = If(IsDBNull(jobReader("Pay_rate")), 0, CDec(jobReader("Pay_rate")))
                    End While

                    Dim monthly As Decimal = If(anual > 0, anual / 12, 0)
                    Dim bimonthly As Decimal = monthly / 2

                    txtdepartment.Text = selectedDepartment
                    txtJob.Text = jobTitle
                    lblMonthly.Text = salary.ToString
                    lblAnually.Text = anual.ToString
                    lblBimonthly.Text = bimonthly.ToString
                    txtpayrate.Text = payRate.ToString

                End If
                jobReader.Close()
            Else
                CreateNewPayrollRecord(selectedEmployeeID, selectedEmployeeName)
            End If

        Catch ex As Exception
            MessageBox.Show($"An error occurred while checking payroll records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private PayrollTable As DataTable
    Private Sub txtsearchnameid_TextChanged(sender As Object, e As EventArgs) Handles txtsearchnameid.TextChanged
        ApplyFilters()
    End Sub
    Private Sub cbDepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged
        ApplyFilters()
    End Sub
    Private Sub ApplyFilters()
        If dgEmployeelist.Rows.Count = 0 Then Exit Sub

        Dim filterText As String = txtsearchnameid.Text.Trim().ToLower()
        Dim departmentFilter As String = cbDepartment.SelectedItem?.ToString()

        For Each row As DataGridViewRow In dgEmployeelist.Rows

            Dim fullName As String = If(row.Cells("FullName").Value, "").ToString().ToLower()
            Dim employeeID As String = If(row.Cells("Employee_ID").Value, "").ToString().ToLower()
            Dim department As String = If(row.Cells("Department").Value, "").ToString()

            Dim matchesText As Boolean = String.IsNullOrEmpty(filterText) OrElse fullName.StartsWith(filterText) OrElse employeeID.StartsWith(filterText)
            Dim matchesDepartment As Boolean = departmentFilter = "All" OrElse departmentFilter Is Nothing OrElse department = departmentFilter

            row.Visible = matchesText AndAlso matchesDepartment
        Next
    End Sub
    Private Sub PopulateDepartments()

        cbdepartment.DataSource = Nothing
        Dim departments As New HashSet(Of String)()

        For Each row As DataGridViewRow In dgEmployeelist.Rows
            If Not row.IsNewRow Then
                Dim department As String = If(row.Cells("Department").Value?.ToString(), "").Trim()
                If Not String.IsNullOrEmpty(department) Then
                    departments.Add(department)
                End If
            End If
        Next

        Dim sortedDepartments = departments.ToList()
        sortedDepartments.Sort()

        sortedDepartments.Insert(0, "All")

        cbdepartment.DataSource = sortedDepartments
    End Sub

    Public Sub LoadEmployeeData()
        dgEmployeelist.DataSource = Nothing
        dgEmployeelist.Rows.Clear()
        dgEmployeelist.Columns.Clear()
        dgEmployeelist.ClearSelection()

        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try
            Dim query As String = "SELECT Photo, Employee_ID, (First_Name & ' ' & Last_Name) AS [FullName], Department, Job " &
                          "FROM Employee " &
                          "WHERE Department <> 'Payroll'"
            Dim adapter As New OleDbDataAdapter(query, con)
            Dim table As New DataTable()

            adapter.Fill(table)

            Dim imgCol As New DataGridViewImageColumn()
            imgCol.HeaderText = "Photo"
            imgCol.Name = "Photo"
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom
            dgEmployeelist.Columns.Add(imgCol)

            dgEmployeelist.Columns.Add("Employee_ID", "Employee ID")
            dgEmployeelist.Columns.Add("FullName", "Full Name")
            dgEmployeelist.Columns.Add("Job", "Job")
            dgEmployeelist.Columns.Add("Department", "Department")


            For Each row As DataRow In table.Rows
                Dim img As Byte() = If(IsDBNull(row("Photo")), Nothing, DirectCast(row("Photo"), Byte()))
                Dim resizedPhoto As Image = Nothing

                If img IsNot Nothing Then
                    Using ms As New IO.MemoryStream(img)
                        Dim originalPhoto As Image = Image.FromStream(ms)
                        resizedPhoto = New Bitmap(originalPhoto, New Size(40, 40))
                    End Using
                End If

                dgEmployeelist.Rows.Add(resizedPhoto, row("Employee_ID"), row("FullName"), row("Job"), row("Department"))
            Next

            dgEmployeelist.RowTemplate.Height = 40
            dgEmployeelist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgEmployeelist.AllowUserToAddRows = False
            dgEmployeelist.Columns("Photo").Width = 80

            For Each column As DataGridViewColumn In dgEmployeelist.Columns
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Next
            dgEmployeelist.ClearSelection()
            dgEmployeelist.Rows(0).Selected = False
            PopulateDepartments()
        Catch ex As Exception
            MessageBox.Show($"An error occurred while populating the employee list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Public Sub GetDeductionData()
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

            dgDeduction.DataSource = dataTable

            If dgDeduction.Columns("Deduction_ID") IsNot Nothing Then
                dgDeduction.Columns("Deduction_ID").Visible = False
            End If
            If dgDeduction.Columns("Payroll_ID") IsNot Nothing Then
                dgDeduction.Columns("Payroll_ID").Visible = False
            End If

            If dgDeduction.Columns("Selected") Is Nothing Then
                Dim checkBoxColumn As New DataGridViewCheckBoxColumn()
                checkBoxColumn.DataPropertyName = "Selected"
                checkBoxColumn.HeaderText = "Select"
                checkBoxColumn.Name = "Selected"
                checkBoxColumn.Width = 50
                dgDeduction.Columns.Add(checkBoxColumn)
            End If

            For Each column As DataGridViewColumn In dgDeduction.Columns
                If column.Name <> "Selected" Then
                    column.ReadOnly = True
                End If
            Next

            dgDeduction.AllowUserToAddRows = False

            CalculateTotalDeduction(dataTable)
        Catch ex As Exception
            MessageBox.Show("An error occurred in GetDeductionData: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub dgDeduction_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgDeduction.CellValueChanged
        If e.ColumnIndex = dgDeduction.Columns("Selected").Index Then
            Dim row As DataGridViewRow = dgDeduction.Rows(e.RowIndex)
            Dim deductionID As Integer = Convert.ToInt32(row.Cells("Deduction_ID").Value)
            Dim isSelected As Boolean = Convert.ToBoolean(row.Cells("Selected").Value)

            UpdateDeductionSelection(deductionID, isSelected)

            CalculateTotalDeduction(CType(dgDeduction.DataSource, DataTable))
        End If
    End Sub
    Private Sub dgDeduction_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgDeduction.CurrentCellDirtyStateChanged
        If dgDeduction.CurrentCell.ColumnIndex = dgDeduction.Columns("Selected").Index AndAlso dgDeduction.IsCurrentCellDirty Then
            dgDeduction.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub UpdateDeductionSelection(deductionID As Integer, isSelected As Boolean)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "UPDATE Deductions SET Selected = @Selected WHERE Deduction_ID = @DeductionID"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@Selected", isSelected)
                cmd.Parameters.AddWithValue("@DeductionID", deductionID)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while updating the database: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

            dgBonus.DataSource = dataTable

            If dgBonus.Columns("Bonus_ID") IsNot Nothing Then
                dgBonus.Columns("Bonus_ID").Visible = False
            End If
            If dgBonus.Columns("Payroll_ID") IsNot Nothing Then
                dgBonus.Columns("Payroll_ID").Visible = False
            End If

            If dgBonus.Columns("Selected") Is Nothing Then
                Dim checkBoxColumn As New DataGridViewCheckBoxColumn()
                checkBoxColumn.DataPropertyName = "Selected"
                checkBoxColumn.HeaderText = "Select"
                checkBoxColumn.Name = "Selected"
                checkBoxColumn.Width = 50
                dgBonus.Columns.Add(checkBoxColumn)
            End If

            For Each column As DataGridViewColumn In dgBonus.Columns
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
    Private Sub dgBonus_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgBonus.CurrentCellDirtyStateChanged
        If dgBonus.CurrentCell.ColumnIndex = dgBonus.Columns("Selected").Index AndAlso dgBonus.IsCurrentCellDirty Then
            dgBonus.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub dgBonus_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgBonus.CellValueChanged
        If e.ColumnIndex = dgBonus.Columns("Selected").Index Then
            Dim row As DataGridViewRow = dgBonus.Rows(e.RowIndex)
            Dim bonusID As Integer = Convert.ToInt32(row.Cells("Bonus_ID").Value)
            Dim isSelected As Boolean = Convert.ToBoolean(row.Cells("Selected").Value)

            UpdateBonusSelection(bonusID, isSelected)

            CalculateTotalBonus(CType(dgBonus.DataSource, DataTable))
        End If
    End Sub

    Private Sub UpdateBonusSelection(bonusID As Integer, isSelected As Boolean)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "UPDATE Bonus SET Selected = @Selected WHERE Bonus_ID = @BonusID"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@Selected", isSelected)
                cmd.Parameters.AddWithValue("@BonusID", bonusID)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while updating the database: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub btnbonus_Click(sender As Object, e As EventArgs) Handles btnbonus.Click
        AdminBonusData.Show()
    End Sub

    Private Sub btnDeduction_Click(sender As Object, e As EventArgs) Handles btnDeduction.Click
        AdminDeductionData.Show()
        AdminInterface.Enabled = False
    End Sub

    Public Sub UpdatePayroll()
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Dim updateQuery As String = "
 UPDATE Payroll 
    SET 
        Pay_rate = @PayRate, 
        OT_rate = @OTRate, 
        Bonus = @Bonus, 
        Deductions = @Deductions, 
        Philhealth = @Philhealth, 
        Tax = @Tax, 
        SSS = @SSS, 
        Gross_Salary = @GrossSalary,
        Total_Deduction = @TotalDeduction, 
        Net_salary = @NetSalary
    WHERE Payroll_ID = @PayrollID"

        Dim command As New OleDbCommand(updateQuery, con)

        command.Parameters.AddWithValue("@PayRate", Convert.ToDecimal(txtpayrate.Text.Trim()))
        command.Parameters.AddWithValue("@OTRate", Convert.ToDecimal(txtotrate.Text.Trim()))
        command.Parameters.AddWithValue("@Bonus", Convert.ToDecimal(txtbonust.Text.Trim()))
        command.Parameters.AddWithValue("@Deductions", Convert.ToDecimal(txtdeduct.Text.Trim()))
        command.Parameters.AddWithValue("@Philhealth", Convert.ToDecimal(txtphil.Text.Trim()))
        command.Parameters.AddWithValue("@Tax", Convert.ToDecimal(txtincometax.Text.Trim()))
        command.Parameters.AddWithValue("@SSS", Convert.ToDecimal(txtsss.Text.Trim()))
        command.Parameters.AddWithValue("@GrossSalary", Convert.ToDecimal(txtgrosssalary.Text.Trim()))
        command.Parameters.AddWithValue("@TotalDeduction", Convert.ToDecimal(txttotaldeduct.Text.Trim()))
        command.Parameters.AddWithValue("@NetSalary", Convert.ToDecimal(txtnetsalary.Text.Trim()))
        command.Parameters.AddWithValue("@PayrollID", Convert.ToInt32(lblPayrollID.Text.Trim()))

        Try
            Dim rowsAffected As Integer = command.ExecuteNonQuery()

            If rowsAffected > 0 Then
            Else
                MessageBox.Show("No records were updated. Please check the Payroll_ID.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error during payroll update: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    'RELEASE PAYROLLLLL
    Private Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        If btnCheck.Text = "Check" Then
            LoadPendingData()
            UpdateUnpaidLabel()
            btnCheck.Text = "Back"
        Else
            btnCheck.Text = "Check"
            LoadEmployeeData()
        End If
    End Sub


    Private Sub btnRelease_Click(sender As Object, e As EventArgs) Handles btnRelease.Click
        ReleasePayroll()
        If btnCheck.Text = "Back" Then
            LoadPendingData()
        Else
            LoadEmployeeData()
        End If
    End Sub

    Private Sub ReleasePayroll()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim oldPayrollID As Integer = CInt(lblPayrollID.Text)

            Dim updateStatusQuery As String = "UPDATE Payroll SET Payroll_Status = 'Released' WHERE Payroll_ID = @PayrollID"
            Using updateStatusCommand As New OleDbCommand(updateStatusQuery, con)
                updateStatusCommand.Parameters.Add("@PayrollID", OleDbType.Integer).Value = oldPayrollID
                updateStatusCommand.ExecuteNonQuery()
            End Using

            Dim employeeID As Integer = CInt(txtid.Text)
            Dim employeeName As String = txtName.Text
            Dim newPayrollID As Integer

            Dim query As String = "SELECT Payroll_ID FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.Add("@EmployeeID", OleDbType.Integer).Value = employeeID
                Dim result = cmd.ExecuteScalar()

                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    newPayrollID = CInt(result)
                Else
                    newPayrollID = CreateNewPayrollRecord(employeeID, employeeName)
                End If
            End Using

            ProcessDeductionsForRelease(oldPayrollID, newPayrollID)

            Dim updateBonusQuery As String = "UPDATE Bonus SET Payroll_ID = @NewPayrollID WHERE Payroll_ID = @OldPayrollID AND Selected = False"
            Using updateBonusCommand As New OleDbCommand(updateBonusQuery, con)
                updateBonusCommand.Parameters.Add("@NewPayrollID", OleDbType.Integer).Value = newPayrollID
                updateBonusCommand.Parameters.Add("@OldPayrollID", OleDbType.Integer).Value = oldPayrollID
                updateBonusCommand.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            MessageBox.Show($"Error releasing payroll: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub ProcessDeductionsForRelease(oldPayrollID As Integer, newPayrollID As Integer)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim dataTable As DataTable = CType(dgDeduction.DataSource, DataTable)
            Dim grossSalary As Decimal = Convert.ToDecimal(txtgrosssalary.Text)
            Dim deductionLimit As Decimal = grossSalary * 0.2
            Dim totalDeduction As Decimal = 0

            Dim sortedRows = dataTable.Select("", "Amount ASC")

            For Each row As DataRow In sortedRows
                Dim amount As Decimal = Convert.ToDecimal(row("Amount"))
                Dim deductionID As Integer = Convert.ToInt32(row("Deduction_ID"))

                If totalDeduction + amount > deductionLimit Then
                    Dim remainingLimit As Decimal = deductionLimit - totalDeduction

                    If remainingLimit > 0 Then
                        UpdatePartialDeduction(row, remainingLimit)

                        InsertRemainingDeduction(row, amount - remainingLimit, newPayrollID)
                    End If

                    Exit For
                Else
                    totalDeduction += amount
                End If
            Next

            txtdeduct.Text = totalDeduction.ToString("F2")
        Catch ex As Exception
            MessageBox.Show($"Error processing deductions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
        End Try
    End Sub
    Private Sub UpdatePartialDeduction(row As DataRow, partialAmount As Decimal)
        Try
            Dim deductionID As Integer = Convert.ToInt32(row("Deduction_ID"))
            Dim query As String = "UPDATE Deductions SET Amount = @PartialAmount, Selected = True WHERE Deduction_ID = @DeductionID"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@PartialAmount", partialAmount)
                cmd.Parameters.AddWithValue("@DeductionID", deductionID)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating partial deduction: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub InsertRemainingDeduction(row As DataRow, remainingAmount As Decimal, newPayrollID As Integer)
        Try
            Dim employeeID As Integer = txtid.Text
            Dim reason As String = row("Reason").ToString()
            Dim query As String = "INSERT INTO Deductions (Payroll_ID, Employee_ID, Amount, Reason, Selected) VALUES (@NewPayrollID, @EmployeeID, @Amount, @Reason, True)"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.AddWithValue("@NewPayrollID", newPayrollID)
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID)
                cmd.Parameters.AddWithValue("@Amount", remainingAmount)
                cmd.Parameters.AddWithValue("@Reason", reason)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error inserting remaining deduction: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function CreateNewPayrollRecord(employeeID As Integer, employeeName As String) As Integer
        Dim newPayrollID As Integer = -1
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim queryMaxID As String = "SELECT MAX(Payroll_ID) FROM Payroll"
            Using commandMaxID As New OleDbCommand(queryMaxID, con)
                Dim maxID As Object = commandMaxID.ExecuteScalar()
                newPayrollID = If(IsDBNull(maxID), 1, CInt(maxID) + 1)
            End Using

            Dim currentDate As DateTime = DateTime.Now.Date
            Dim dateDue As DateTime = If(currentDate.Day <= 15,
                                     New DateTime(currentDate.Year, currentDate.Month, 15),
                                     New DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)))

            Dim selectedJob As String = dgEmployeelist.SelectedRows(0).Cells("Job").Value.ToString()
            Dim selectedDepartment As String = dgEmployeelist.SelectedRows(0).Cells("Department").Value.ToString()
            Dim salary As Decimal = 0, anual As Decimal = 0, payRate As Decimal = 0

            Dim queryPayRate As String = "SELECT Salary, Anual, Pay_rate FROM Job WHERE Job = @Job AND Department = @Department"
            Using commandPayRate As New OleDbCommand(queryPayRate, con)
                commandPayRate.Parameters.Add("@Job", OleDbType.VarChar).Value = selectedJob
                commandPayRate.Parameters.Add("@Department", OleDbType.VarChar).Value = selectedDepartment

                Using reader As OleDbDataReader = commandPayRate.ExecuteReader()
                    If reader.Read() Then
                        salary = If(IsDBNull(reader("Salary")), 0, CDec(reader("Salary")))
                        anual = If(IsDBNull(reader("Anual")), 0, CDec(reader("Anual")))
                        payRate = If(IsDBNull(reader("Pay_rate")), 0, CDec(reader("Pay_rate")))
                    End If
                End Using
            End Using

            Dim queryInsert As String = "INSERT INTO Payroll (Payroll_ID, Employee_ID, Employee_name, Department, Job, Pay_rate, Hours_worked, OT_rate, OT_worked, Bonus, Deductions, Philhealth, Tax, SSS, Gross_Salary, Total_Deduction, Net_Salary, Date_Created, Date_Due, Payroll_Status) " &
                                     "VALUES (@PayrollID, @EmployeeID, @EmployeeName, @Department, @Job, @PayRate, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, @Created, @Due, 'Unreleased')"
            Using commandInsert As New OleDbCommand(queryInsert, con)
                commandInsert.Parameters.Add("@PayrollID", OleDbType.Integer).Value = newPayrollID
                commandInsert.Parameters.Add("@EmployeeID", OleDbType.Integer).Value = employeeID
                commandInsert.Parameters.Add("@EmployeeName", OleDbType.VarChar).Value = employeeName
                commandInsert.Parameters.Add("@Department", OleDbType.VarChar).Value = selectedDepartment
                commandInsert.Parameters.Add("@Job", OleDbType.VarChar).Value = selectedJob
                commandInsert.Parameters.Add("@PayRate", OleDbType.Decimal).Value = payRate
                commandInsert.Parameters.Add("@Created", OleDbType.Date).Value = currentDate
                commandInsert.Parameters.Add("@Due", OleDbType.Date).Value = dateDue
                commandInsert.ExecuteNonQuery()
            End Using

            Return newPayrollID

        Catch ex As Exception
            MessageBox.Show($"An error occurred while creating a new payroll record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Function


    Private Sub lbldatedue_TextChanged(sender As Object, e As EventArgs) Handles lbldatedue.TextChanged
        Dim dueDate As DateTime
        If DateTime.TryParse(lbldatedue.Text, dueDate) Then
            If dueDate < DateTime.Now Then
                btnRelease.Enabled = True
            Else
                btnRelease.Enabled = False
            End If

        End If
    End Sub


    Public Sub LoadPendingData()
        dgEmployeelist.DataSource = Nothing
        dgEmployeelist.Rows.Clear()
        dgEmployeelist.Columns.Clear()
        dgEmployeelist.ClearSelection()

        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try

            Dim query As String = "SELECT e.Photo, e.Employee_ID, (e.First_Name & ' ' & e.Last_Name) AS [FullName], e.Department, e.Job " &
                              "FROM Employee e " &
                              "INNER JOIN Payroll p ON e.Employee_ID = p.Employee_ID " &
                              "WHERE p.Payroll_Status = 'Pending' AND e.Department <> 'Payroll'"
            Dim adapter As New OleDbDataAdapter(query, con)
            Dim table As New DataTable()

            adapter.Fill(table)

            Dim imgCol As New DataGridViewImageColumn()
            imgCol.HeaderText = "Photo"
            imgCol.Name = "Photo"
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom
            dgEmployeelist.Columns.Add(imgCol)

            dgEmployeelist.Columns.Add("Employee_ID", "Employee ID")
            dgEmployeelist.Columns.Add("FullName", "Full Name")
            dgEmployeelist.Columns.Add("Job", "Job")
            dgEmployeelist.Columns.Add("Department", "Department")

            For Each row As DataRow In table.Rows
                Dim img As Byte() = If(IsDBNull(row("Photo")), Nothing, DirectCast(row("Photo"), Byte()))
                Dim resizedPhoto As Image = Nothing

                If img IsNot Nothing Then
                    Using ms As New IO.MemoryStream(img)
                        Dim originalPhoto As Image = Image.FromStream(ms)
                        resizedPhoto = New Bitmap(originalPhoto, New Size(40, 40))
                    End Using
                End If

                dgEmployeelist.Rows.Add(resizedPhoto, row("Employee_ID"), row("FullName"), row("Job"), row("Department"))
            Next

            For Each dgvRow As DataGridViewRow In dgEmployeelist.Rows
                If dgvRow.Cells("Department").Value IsNot Nothing AndAlso dgvRow.Cells("Department").Value.ToString() = "Payroll" Then
                    dgvRow.Visible = False
                End If
            Next

            dgEmployeelist.RowTemplate.Height = 40
            dgEmployeelist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgEmployeelist.AllowUserToAddRows = False
            dgEmployeelist.Columns("Photo").Width = 80

            For Each column As DataGridViewColumn In dgEmployeelist.Columns
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            Next
            dgEmployeelist.ClearSelection()
            dgEmployeelist.Rows(0).Selected = False

            PopulateDepartments()
        Catch ex As Exception
            MessageBox.Show($"An error occurred while populating the employee list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Public Sub UpdateUnpaidLabel()
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try
            Dim query As String = "SELECT COUNT(*) FROM Payroll WHERE Payroll_Status = 'Pending'"
            Using cmd As New OleDbCommand(query, con)
                Dim unpaidCount As Integer = CInt(cmd.ExecuteScalar())
                lblunpaid.Text = $"{unpaidCount}"
            End Using
        Catch ex As Exception
            MessageBox.Show($"An error occurred while updating unpaid label: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    'Check

    Private Sub GetPendingData()
        Dim selectedEmployeeID As Integer = CInt(dgEmployeelist.SelectedRows(0).Cells("Employee_ID").Value)
        Dim selectedEmployeeName As String = dgEmployeelist.SelectedRows(0).Cells("FullName").Value.ToString()
        Dim selectedJob As String = dgEmployeelist.SelectedRows(0).Cells("Job").Value.ToString()
        Dim selectedDepartment As String = dgEmployeelist.SelectedRows(0).Cells("Department").Value.ToString()
        txtJob.Text = selectedJob
        txtdepartment.Text = selectedDepartment

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Photo FROM Employee WHERE Employee_ID = @EmployeeID"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID)

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

            Dim queryCheckPayroll As String = "SELECT * FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Pending'"
            Dim commandCheckPayroll As New OleDbCommand(queryCheckPayroll, con)
            commandCheckPayroll.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID)

            Dim payrollReader As OleDbDataReader = commandCheckPayroll.ExecuteReader()

            If payrollReader.HasRows Then

                While payrollReader.Read()
                    lblPayrollID.Text = payrollReader("Payroll_ID").ToString()
                    txtName.Text = payrollReader("Employee_Name").ToString()
                    txtid.Text = payrollReader("Employee_ID").ToString()
                    txtpayrate.Text = payrollReader("Pay_rate").ToString()
                    txtHoursworked.Text = payrollReader("Hours_Worked").ToString()
                    txtotworked.Text = payrollReader("OT_Worked").ToString()
                    lbldatecreated.Text = Convert.ToDateTime(payrollReader("Date_Created")).ToString("MM/dd/yyyy")
                    lbldatedue.Text = Convert.ToDateTime(payrollReader("Date_Due")).ToString("MM/dd/yyyy")
                End While
                payrollReader.Close()

                Dim salary As Decimal = 0
                Dim anual As Decimal = 0
                Dim payRate As Decimal = 0
                Dim jobTitle As String = String.Empty

                Dim queryPayRate As String = "SELECT Job, Salary, Anual, Pay_rate FROM Job WHERE Job = @Job AND Department = @Department"
                Dim commandPayRate As New OleDbCommand(queryPayRate, con)
                commandPayRate.Parameters.AddWithValue("@Job", selectedJob)
                commandPayRate.Parameters.AddWithValue("@Department", selectedDepartment)

                Dim jobReader As OleDbDataReader = commandPayRate.ExecuteReader()

                If jobReader.HasRows Then
                    While jobReader.Read()
                        jobTitle = If(IsDBNull(jobReader("Job")), String.Empty, jobReader("Job").ToString())
                        salary = If(IsDBNull(jobReader("Salary")), 0, CDec(jobReader("Salary")))
                        anual = If(IsDBNull(jobReader("Anual")), 0, CDec(jobReader("Anual")))
                        payRate = If(IsDBNull(jobReader("Pay_rate")), 0, CDec(jobReader("Pay_rate")))
                    End While

                    Dim monthly As Decimal = If(anual > 0, anual / 12, 0)
                    Dim bimonthly As Decimal = monthly / 2

                    txtdepartment.Text = selectedDepartment
                    txtJob.Text = jobTitle
                    lblMonthly.Text = salary.ToString
                    lblAnually.Text = anual.ToString
                    lblBimonthly.Text = bimonthly.ToString
                    txtpayrate.Text = payRate.ToString

                End If
                jobReader.Close()

            End If

        Catch ex As Exception
            MessageBox.Show($"An error occurred while checking payroll records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

End Class