Imports System.Data.OleDb
Imports System.Drawing.Printing
Imports System.Drawing.Imaging
Imports System.IO
Imports iText.Kernel.Pdf
Imports iText.Layout
Imports iText.Layout.Element
Imports iText.IO.Image
Imports System.Diagnostics

Public Class AdminHistory
    Private Sub txtsearchnameid_TextChanged(sender As Object, e As EventArgs) Handles txtsearchnameid.TextChanged
        ApplyFilters()
    End Sub
    Private Sub ApplyFilters()
        If dgHistory.Rows.Count = 0 Then Exit Sub

        Dim filterText As String = txtsearchnameid.Text.Trim().ToLower()

        For Each row As DataGridViewRow In dgHistory.Rows
            Dim fullName As String = If(row.Cells("Employee_Name").Value, "").ToString().ToLower()
            Dim employeeID As String = If(row.Cells("Employee_ID").Value, "").ToString().ToLower()

            Dim matchesText As Boolean = String.IsNullOrEmpty(filterText) OrElse
                                     fullName.StartsWith(filterText) OrElse
                                     employeeID.StartsWith(filterText)

            row.Visible = matchesText
        Next
    End Sub
    Public Sub LoadPayrollHistory()
        dgHistory.DataSource = Nothing
        dgHistory.Rows.Clear()
        dgHistory.Columns.Clear()
        dgHistory.ClearSelection()

        Dim query As String = "SELECT Payroll_ID, Employee_ID, Employee_Name, Department, Job, Gross_Salary, Total_Deduction, Net_salary, Date_Due " &
                          "FROM Payroll WHERE Payroll_Status = 'Released'"

        If con.State = ConnectionState.Closed Then con.Open()

        Dim command As New OleDb.OleDbCommand(query, con)
        Dim adapter As New OleDb.OleDbDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)


        PayrollHistoryTable = table
        dgHistory.DataSource = table

        With dgHistory
            .Columns("Payroll_ID").HeaderText = "Payroll ID"
            .Columns("Employee_ID").HeaderText = "Employee ID"
            .Columns("Employee_Name").HeaderText = "Employee Name"
            .Columns("Department").HeaderText = "Department"
            .Columns("Job").HeaderText = "Job"
            .Columns("Gross_Salary").HeaderText = "Gross Salary"
            .Columns("Total_Deduction").HeaderText = "Total Deductions"
            .Columns("Net_salary").HeaderText = "Net Salary"
            .Columns("Date_Due").HeaderText = "Date Due"
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        End With
    End Sub
    Public Sub DisplayPayrollDetails()
        Dim selectedPayrollID As String = dgHistory.SelectedRows(0).Cells("Payroll_ID").Value.ToString()


        Dim query As String = "SELECT Payroll_ID, Employee_ID, Employee_Name, Department, Job, Pay_rate, Hours_Worked, OT_rate, OT_Worked, " &
                              "Gross_Salary, Tax, sss, philhealth, Total_Deduction, Net_salary, Date_created, Date_Due " &
                              "FROM Payroll WHERE Payroll_ID = @PayrollID"

        Dim command As New OleDb.OleDbCommand(query, con)

        command.Parameters.AddWithValue("@PayrollID", selectedPayrollID)

        Dim reader As OleDb.OleDbDataReader

        Try
            If con.State = ConnectionState.Closed Then con.Open()

            reader = command.ExecuteReader()

            If reader.HasRows Then
                reader.Read()

                lblID.Text = reader("Payroll_ID").ToString()
                lblEmployeeID.Text = reader("Employee_ID").ToString()
                lblName.Text = reader("Employee_Name").ToString()
                lbldepartment.Text = reader("Department").ToString()
                lblJob.Text = reader("Job").ToString()
                lblpayrate.Text = reader("Pay_rate").ToString()
                lblHoursWorked.Text = reader("Hours_Worked").ToString()
                lblOTrate.Text = reader("OT_rate").ToString()
                lblOTWorked.Text = reader("OT_Worked").ToString()
                lblgross.Text = reader("Gross_Salary").ToString()
                lbltax.Text = reader("Tax").ToString()
                lblsss.Text = reader("sss").ToString()
                lblphil.Text = reader("philhealth").ToString()
                lbldeduction.Text = reader("Total_Deduction").ToString()
                lblsalary.Text = reader("Net_salary").ToString()
                lblCreated.Text = DateTime.Parse(reader("Date_created").ToString()).ToString("MM/dd/yyyy")
                lbldate.Text = DateTime.Parse(reader("Date_Due").ToString()).ToString("MM/dd/yyyy")

                Dim Standardfee = Val(lblpayrate.Text) * Val(lblHoursWorked.Text)
                Dim OTfee = Val(lblOTrate.Text) * Val(lblOTWorked.Text)
                lbltotalpay.Text = Standardfee.ToString("0.00")
                lbltotalot.Text = OTfee.ToString("0.00")
            Else
                MessageBox.Show("No details found for the selected Payroll ID.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show("An error occurred while fetching payroll details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub
    Private Sub dghistory_SelectionChanged(sender As Object, e As EventArgs) Handles dgHistory.SelectionChanged
        If dgHistory.SelectedRows.Count > 0 Then
            Dim selectedRow = dgHistory.SelectedRows(0)
            txtpayrollid.Text = selectedRow.Cells("Payroll_ID").Value.ToString
            txtemployeeid.Text = selectedRow.Cells("Employee_ID").Value.ToString
            txtname.Text = selectedRow.Cells("Employee_Name").Value.ToString
            txtjob.Text = selectedRow.Cells("Job").Value.ToString
            txtpayrolldate.Text = selectedRow.Cells("Date_Due").Value.ToString
            DisplayPayrollDetails()
            GetDeductionData()
            GetBonusData()
        End If
    End Sub

    Private PayrollHistoryTable As DataTable



    Public Sub GetDeductionData()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim payrollID As String = txtpayrollid.Text

            Dim query As String = "SELECT Deduction_ID, Payroll_ID, Reason, Amount FROM Deductions WHERE Payroll_ID = @PayrollID"

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

            If dgdeduction.Columns("Reason") IsNot Nothing Then
                dgdeduction.Columns("Reason").DisplayIndex = 1
            End If
            If dgdeduction.Columns("Amount") IsNot Nothing Then
                dgdeduction.Columns("Amount").DisplayIndex = 2
            End If

            For Each column As DataGridViewColumn In dgdeduction.Columns
                If column.Name = "Reason" Or column.Name = "Amount" Then
                    column.DefaultCellStyle.Padding = New Padding(25, 0, 0, 0)
                    column.HeaderCell.Style.Padding = New Padding(25, 0, 0, 0)
                End If
                If column.Name = "Amount" Then
                    column.DefaultCellStyle.Padding = New Padding(175, 0, 0, 0)
                    column.HeaderCell.Style.Padding = New Padding(175, 0, 0, 0)
                End If

                column.ReadOnly = True
            Next

            dgdeduction.AllowUserToAddRows = False

        Catch ex As Exception
            MessageBox.Show("An error occurred in GetDeductionData: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Public Sub GetBonusData()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim payrollID As String = txtpayrollid.Text

            Dim query As String = "SELECT Bonus_ID, Payroll_ID, Amount, Reason FROM Bonus WHERE Payroll_ID = @PayrollID"

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

            If dgbonus.Columns("Reason") IsNot Nothing Then
                dgbonus.Columns("Reason").DisplayIndex = 1
            End If
            If dgbonus.Columns("Amount") IsNot Nothing Then
                dgbonus.Columns("Amount").DisplayIndex = 2
            End If

            For Each column As DataGridViewColumn In dgbonus.Columns
                If column.Name = "Reason" Or column.Name = "Amount" Then
                    column.DefaultCellStyle.Padding = New Padding(25, 0, 0, 0)
                    column.HeaderCell.Style.Padding = New Padding(25, 0, 0, 0)
                End If
                If column.Name = "Amount" Then
                    column.DefaultCellStyle.Padding = New Padding(175, 0, 0, 0)
                    column.HeaderCell.Style.Padding = New Padding(175, 0, 0, 0)
                End If
                column.ReadOnly = True
            Next

            dgbonus.AllowUserToAddRows = False

        Catch ex As Exception
            MessageBox.Show("An error occurred in GetBonusData: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnprint_Click(sender As Object, e As EventArgs) Handles btnprint.Click
        Dim bmp As Bitmap = CapturePanel(pnlPayslip)

        SavePanelAsPdf(bmp, "Payslip.pdf")

        OpenPdf("Payslip.pdf")
    End Sub


    Private Function CapturePanel(panel As Panel) As Bitmap

        Dim bmp As New Bitmap(panel.Width, panel.Height)
        panel.DrawToBitmap(bmp, New Rectangle(0, 0, panel.Width, panel.Height))
        Return bmp
    End Function

    Private Sub SavePanelAsPdf(bmp As Bitmap, fileName As String)
        Using imgStream As New MemoryStream()
            bmp.Save(imgStream, ImageFormat.Png)
            imgStream.Position = 0

            Dim imgData As ImageData = ImageDataFactory.Create(imgStream.ToArray())

            Using pdfWriter As New PdfWriter(fileName)
                Using pdfDoc As New PdfDocument(pdfWriter)
                    Dim document As New Document(pdfDoc)

                    Dim pageSize = pdfDoc.GetDefaultPageSize()
                    Dim pageWidth As Single = pageSize.GetWidth()
                    Dim pageHeight As Single = pageSize.GetHeight()

                    Dim image As New iText.Layout.Element.Image(imgData)

                    image.SetFixedPosition(0, 0)
                    image.SetWidth(pageWidth)
                    image.SetHeight(pageHeight)

                    document.Add(image)

                    document.Close()
                End Using
            End Using
        End Using
    End Sub

    Private Sub OpenPdf(fileName As String)
        Try
            Process.Start("explorer.exe", fileName)
        Catch ex As Exception
            MessageBox.Show("Could not open the PDF file: " & ex.Message)
        End Try
    End Sub

    Private Sub Guna2GradientPanel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2GradientPanel1.Paint

    End Sub
End Class