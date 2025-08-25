Imports System.Data.OleDb
Imports System.IO
Imports System.Reflection.Metadata
Imports Guna.UI2.WinForms
Imports Org.BouncyCastle.Asn1.Ocsp

Public Class AdminEmployee
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateDepartmentAndJob()
        LoadEmployeeData()
        PopulateDepartments()
    End Sub

    Public Sub PopulateDepartmentAndJob()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim departmentCmd As New OleDbCommand("SELECT DISTINCT Department FROM Job", con)
            Dim departmentReader As OleDbDataReader = departmentCmd.ExecuteReader()

            cbDepartment.Items.Clear()
            While departmentReader.Read()
                cbDepartment.Items.Add(departmentReader("Department").ToString())
            End While
            departmentReader.Close()

            cbJob.Items.Clear()
            cbJob.Enabled = False
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub cbDepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDepartment.SelectedIndexChanged
        If cbDepartment.SelectedItem IsNot Nothing Then
            Dim selectedDepartment = cbDepartment.SelectedItem.ToString
            PopulateJobComboBox(selectedDepartment)
            cbJob.Enabled = True
        Else
            cbJob.Enabled = False
            cbJob.Items.Clear()
        End If
    End Sub

    Private Sub dgEmployeelist_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgEmployeelist.CellClick

        btnedit.Visible = True
        If btnedit.Text = "Cancel" Then
            btnedit.PerformClick()
        End If
        cbDepartment.Enabled = True
        cbJob.Enabled = True
        If e.RowIndex >= 0 Then
            Dim selectedRow = dgEmployeelist.Rows(e.RowIndex)
            Dim selectedEmployeeID = If(selectedRow.Cells("Employee_ID").Value?.ToString(), "")

            DisplaySchedule(selectedEmployeeID)

            Try
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                Dim query = "SELECT First_Name, Last_Name, Department, Job, Photo, Account_Num, Phone_Num, Email " &
                        "FROM Employee WHERE Employee_ID = @EmployeeID"
                Dim command As New OleDbCommand(query, con)
                command.Parameters.AddWithValue("@EmployeeID", selectedEmployeeID)

                Using reader = command.ExecuteReader()
                    If reader.Read() Then

                        txtid.Text = selectedEmployeeID
                        txtfname.Text = If(reader("First_Name").ToString(), "")
                        txtlname.Text = If(reader("Last_Name").ToString(), "")
                        txtaccnum.Text = If(reader("Account_Num").ToString(), "")
                        txtphone.Text = If(reader("Phone_Num").ToString(), "")
                        txtemail.Text = If(reader("Email").ToString(), "")

                        If Not IsDBNull(reader("Photo")) AndAlso reader("Photo") IsNot Nothing Then
                            Dim photoBytes = CType(reader("Photo"), Byte())
                            Using ms As New MemoryStream(photoBytes)
                                PictureBox1.Image = Image.FromStream(ms)
                                PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                            End Using
                        Else
                            PictureBox1.Image = Nothing
                        End If

                        Dim department As String = reader("Department").ToString()
                        Dim job As String = reader("Job").ToString()

                        cbDepartment.SelectedItem = department
                        PopulateJobComboBox(department)

                        cbJob.SelectedItem = job

                        cbDepartment.Enabled = False
                        cbJob.Enabled = False
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show($"An error occurred Employee List: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If
    End Sub


    Private Sub PopulateJobComboBox(department As String)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim cmd As New OleDbCommand("SELECT Job FROM Job WHERE Department = @Department", con)
            cmd.Parameters.AddWithValue("@Department", department)
            Dim adapter As New OleDbDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            cbJob.Items.Clear()
            For Each row As DataRow In dt.Rows
                cbJob.Items.Add(row("Job").ToString())
            Next
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
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
            If dgEmployeelist.Rows.Count > 0 Then
                dgEmployeelist.Rows(0).Selected = False
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred while populating the employee list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DisplaySchedule(employeeID As String)
        Try

            dgvschedule.Columns.Clear()
            dgvschedule.Rows.Clear()

            Dim daysOfWeek As String() = {"S", "M", "T", "W", "T", "F", "S"}
            For Each day As String In daysOfWeek
                dgvschedule.Columns.Add(day, day)
            Next

            dgvschedule.Rows.Add()

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim scheduleQuery As String = "
        SELECT DayOfWeek, StartTime, EndTime 
        FROM Schedule 
        WHERE Employee_ID = @employeeID"

            Using command As New OleDbCommand(scheduleQuery, con)
                command.Parameters.AddWithValue("@employeeID", employeeID)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim dayOfWeek As String = reader("DayOfWeek").ToString()
                        Dim startTime As String = DateTime.Parse(reader("StartTime").ToString()).ToShortTimeString()
                        Dim endTime As String = DateTime.Parse(reader("EndTime").ToString()).ToShortTimeString()

                        Dim timeRange As String = $"{startTime}{vbCrLf}-{vbCrLf}{endTime}"

                        Dim columnIndex As Integer = GetDayColumnIndex(dayOfWeek)
                        If columnIndex >= 0 Then
                            dgvschedule.Rows(0).Cells(columnIndex).Value = timeRange
                        End If
                    End While
                End Using
            End Using

            dgvschedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvschedule.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dgvschedule.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dgvschedule.DefaultCellStyle.WrapMode = DataGridViewTriState.True


        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function GetDayColumnIndex(dayOfWeek As String) As Integer
        Select Case dayOfWeek.ToLower()
            Case "sunday"
                Return 0
            Case "monday"
                Return 1
            Case "tuesday"
                Return 2
            Case "wednesday"
                Return 3
            Case "thursday"
                Return 4
            Case "friday"
                Return 5
            Case "saturday"
                Return 6
            Case Else
                Return -1
        End Select
    End Function

    Private originalFName As String
    Private originalLName As String
    Private originalAccNum As String
    Private originalEmail As String
    Private originalPhone As String
    Private originalDepartment As String
    Private originalJob As String

    Private originalSunStart As DateTime
    Private originalSunEnd As DateTime
    Private originalSunChecked As Boolean

    Private originalMonStart As DateTime
    Private originalMonEnd As DateTime
    Private originalMonChecked As Boolean

    Private originalTuesStart As DateTime
    Private originalTuesEnd As DateTime
    Private originalTuesChecked As Boolean

    Private originalWedStart As DateTime
    Private originalWedEnd As DateTime
    Private originalWedChecked As Boolean

    Private originalThursStart As DateTime
    Private originalThursEnd As DateTime
    Private originalThursChecked As Boolean

    Private originalFriStart As DateTime
    Private originalFriEnd As DateTime
    Private originalFriChecked As Boolean

    Private originalSatStart As DateTime
    Private originalSatEnd As DateTime
    Private originalSatChecked As Boolean
    Private originalimage As Image

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnedit.Click
        If btnedit.Text = "Edit" Then

            originalFName = txtfname.Text
            originalLName = txtlname.Text
            originalAccNum = txtaccnum.Text
            originalEmail = txtemail.Text
            originalPhone = txtphone.Text
            originalDepartment = cbDepartment.Text
            originalJob = cbJob.Text

            originalSunStart = DTPstartsun.Value
            originalSunEnd = DTPendsun.Value
            originalSunChecked = chkboxsun.Checked

            originalMonStart = DTPstartmon.Value
            originalMonEnd = DTPendmon.Value
            originalMonChecked = chkboxmon.Checked

            originalTuesStart = DTPstarttues.Value
            originalTuesEnd = DTPendtues.Value
            originalTuesChecked = chkboxtues.Checked

            originalWedStart = DTPstartwed.Value
            originalWedEnd = DTPendwed.Value
            originalWedChecked = chkboxwed.Checked

            originalThursStart = DTPstartthurs.Value
            originalThursEnd = DTPendthurs.Value
            originalThursChecked = chkboxthurs.Checked

            originalFriStart = DTPstartfri.Value
            originalFriEnd = DTPendfri.Value
            originalFriChecked = chkboxfri.Checked

            originalSatStart = DTPstartsat.Value
            originalSatEnd = DTPendsat.Value
            originalSatChecked = chkboxsat.Checked

            originalimage = PictureBox1.Image


            txtfname.ReadOnly = False
            txtlname.ReadOnly = False
            txtaccnum.ReadOnly = False
            txtemail.ReadOnly = False
            txtphone.ReadOnly = False
            cbDepartment.Enabled = True
            cbJob.Enabled = True

            chkboxPass.Visible = True
            chkboxPass.Checked = False

            dgvschedule.Visible = False
            btnsave.Visible = True


            DTPstartsun.Visible = chkboxsun.Checked
            DTPendsun.Visible = chkboxsun.Checked
            DTPstartmon.Visible = chkboxmon.Checked
            DTPendmon.Visible = chkboxmon.Checked
            DTPstarttues.Visible = chkboxtues.Checked
            DTPendtues.Visible = chkboxtues.Checked
            DTPstartwed.Visible = chkboxwed.Checked
            DTPendwed.Visible = chkboxwed.Checked
            DTPstartthurs.Visible = chkboxthurs.Checked
            DTPendthurs.Visible = chkboxthurs.Checked
            DTPstartfri.Visible = chkboxfri.Checked
            DTPendfri.Visible = chkboxfri.Checked
            DTPstartsat.Visible = chkboxsat.Checked
            DTPendsat.Visible = chkboxsat.Checked


            chkboxsun.Visible = True
            chkboxmon.Visible = True
            chkboxtues.Visible = True
            chkboxwed.Visible = True
            chkboxthurs.Visible = True
            chkboxfri.Visible = True
            chkboxsat.Visible = True

            Dim employeeID As String = txtid.Text
            Dim query As String = "SELECT StartTime, EndTime, DayOfWeek FROM Schedule WHERE Employee_ID = @employee_id"

            Try
                con.Open()
                Using command As New OleDb.OleDbCommand(query, con)
                    command.Parameters.AddWithValue("@employee_id", employeeID)
                    Using reader As OleDb.OleDbDataReader = command.ExecuteReader()
                        While reader.Read()
                            Dim dayOfWeek As String = reader("DayOfWeek").ToString().Trim().ToLower()
                            Dim startTime As DateTime
                            Dim endTime As DateTime

                            If DateTime.TryParse(reader("StartTime").ToString(), startTime) AndAlso DateTime.TryParse(reader("EndTime").ToString(), endTime) Then

                                Select Case dayOfWeek
                                    Case "sunday"
                                        DTPstartsun.Value = startTime
                                        DTPendsun.Value = endTime
                                        chkboxsun.Checked = True
                                        DTPstartsun.Visible = True
                                        DTPendsun.Visible = True
                                    Case "monday"
                                        DTPstartmon.Value = startTime
                                        DTPendmon.Value = endTime
                                        chkboxmon.Checked = True
                                        DTPstartmon.Visible = True
                                        DTPendmon.Visible = True
                                    Case "tuesday"
                                        DTPstarttues.Value = startTime
                                        DTPendtues.Value = endTime
                                        chkboxtues.Checked = True
                                        DTPstarttues.Visible = True
                                        DTPendtues.Visible = True
                                    Case "wednesday"
                                        DTPstartwed.Value = startTime
                                        DTPendwed.Value = endTime
                                        chkboxwed.Checked = True
                                        DTPstartwed.Visible = True
                                        DTPendwed.Visible = True
                                    Case "thursday"
                                        DTPstartthurs.Value = startTime
                                        DTPendthurs.Value = endTime
                                        chkboxthurs.Checked = True
                                        DTPstartthurs.Visible = True
                                        DTPendthurs.Visible = True
                                    Case "friday"
                                        DTPstartfri.Value = startTime
                                        DTPendfri.Value = endTime
                                        chkboxfri.Checked = True
                                        DTPstartfri.Visible = True
                                        DTPendfri.Visible = True
                                    Case "saturday"
                                        DTPstartsat.Value = startTime
                                        DTPendsat.Value = endTime
                                        chkboxsat.Checked = True
                                        DTPstartsat.Visible = True
                                        DTPendsat.Visible = True
                                End Select
                            Else
                                MessageBox.Show("Invalid time format in the database.")
                            End If
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try

            btnedit.Text = "Cancel"
        ElseIf btnedit.Text = "Cancel" Then
            txtfname.Text = originalFName
            txtlname.Text = originalLName
            txtaccnum.Text = originalAccNum
            txtemail.Text = originalEmail
            txtphone.Text = originalPhone
            cbDepartment.Text = originalDepartment
            cbJob.Text = originalJob

            DTPstartsun.Value = originalSunStart
            DTPendsun.Value = originalSunEnd
            chkboxsun.Checked = originalSunChecked

            DTPstartmon.Value = originalMonStart
            DTPendmon.Value = originalMonEnd
            chkboxmon.Checked = originalMonChecked

            DTPstarttues.Value = originalTuesStart
            DTPendtues.Value = originalTuesEnd
            chkboxtues.Checked = originalTuesChecked

            DTPstartwed.Value = originalWedStart
            DTPendwed.Value = originalWedEnd
            chkboxwed.Checked = originalWedChecked

            DTPstartthurs.Value = originalThursStart
            DTPendthurs.Value = originalThursEnd
            chkboxthurs.Checked = originalThursChecked

            DTPstartfri.Value = originalFriStart
            DTPendfri.Value = originalFriEnd
            chkboxfri.Checked = originalFriChecked

            DTPstartsat.Value = originalSatStart
            DTPendsat.Value = originalSatEnd
            chkboxsat.Checked = originalSatChecked

            PictureBox1.Image = originalimage

            txtfname.ReadOnly = True
            txtlname.ReadOnly = True
            txtaccnum.ReadOnly = True
            txtemail.ReadOnly = True
            txtphone.ReadOnly = True
            cbDepartment.Enabled = False
            cbJob.Enabled = False

            DTPstartsun.Visible = False
            DTPendsun.Visible = False
            chkboxsun.Visible = False
            DTPstartmon.Visible = False
            DTPendmon.Visible = False
            chkboxmon.Visible = False
            DTPstarttues.Visible = False
            DTPendtues.Visible = False
            chkboxtues.Visible = False
            DTPstartwed.Visible = False
            DTPendwed.Visible = False
            chkboxwed.Visible = False
            DTPstartthurs.Visible = False
            DTPendthurs.Visible = False
            chkboxthurs.Visible = False
            DTPstartfri.Visible = False
            DTPendfri.Visible = False
            chkboxfri.Visible = False
            DTPstartsat.Visible = False
            DTPendsat.Visible = False
            chkboxsat.Visible = False
            chkboxPass.Checked = False
            chkboxPass.Visible = False


            dgvschedule.Visible = True
            btnsave.Visible = False


            btnedit.Text = "Edit"
        End If
    End Sub

    Private Sub chkbox_CheckedChanged(sender As Object, e As EventArgs) Handles chkboxsun.CheckedChanged, chkboxmon.CheckedChanged, chkboxtues.CheckedChanged, chkboxwed.CheckedChanged, chkboxthurs.CheckedChanged, chkboxfri.CheckedChanged, chkboxsat.CheckedChanged
        Select Case CType(sender, CheckBox).Name
            Case "chkboxsun"
                DTPstartsun.Visible = chkboxsun.Checked
                DTPendsun.Visible = chkboxsun.Checked
            Case "chkboxmon"
                DTPstartmon.Visible = chkboxmon.Checked
                DTPendmon.Visible = chkboxmon.Checked
            Case "chkboxtues"
                DTPstarttues.Visible = chkboxtues.Checked
                DTPendtues.Visible = chkboxtues.Checked
            Case "chkboxwed"
                DTPstartwed.Visible = chkboxwed.Checked
                DTPendwed.Visible = chkboxwed.Checked
            Case "chkboxthurs"
                DTPstartthurs.Visible = chkboxthurs.Checked
                DTPendthurs.Visible = chkboxthurs.Checked
            Case "chkboxfri"
                DTPstartfri.Visible = chkboxfri.Checked
                DTPendfri.Visible = chkboxfri.Checked
            Case "chkboxsat"
                DTPstartsat.Visible = chkboxsat.Checked
                DTPendsat.Visible = chkboxsat.Checked
        End Select
    End Sub


    Private Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to overwrite the data?", "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        Dim imageBytes As Byte() = Nothing
        If result = DialogResult.Yes Then
            Dim updateEmployeeQuery As String = "UPDATE employee SET [first_name] = @fname, [last_name] = @lname, " &
                                            "[account_num] = @accnum, [department] = @department, [job] = @job, " &
                                            "[phone_num] = @phone, [email] = @email"

            If chkboxPass.Checked Then
                updateEmployeeQuery &= ", [password] = @password"
            End If

            If PictureBox1.Image IsNot originalimage Then
                If PictureBox1.Image IsNot Nothing Then
                    Dim ms As New IO.MemoryStream()
                    PictureBox1.Image.Save(ms, PictureBox1.Image.RawFormat)
                    imageBytes = ms.ToArray()
                End If
                updateEmployeeQuery &= ", [photo] = @photo"
            End If

            updateEmployeeQuery &= " WHERE [employee_id] = @eid"

            Try
                con.Open()
                Dim department As String = If(cbDepartment.SelectedItem?.ToString(), "")
                Dim job As String = If(cbJob.SelectedItem?.ToString(), "")

                Using command As New OleDb.OleDbCommand(updateEmployeeQuery, con)

                    command.Parameters.AddWithValue("@fname", txtfname.Text)
                    command.Parameters.AddWithValue("@lname", txtlname.Text)
                    command.Parameters.AddWithValue("@accnum", txtaccnum.Text)
                    command.Parameters.AddWithValue("@department", department)
                    command.Parameters.AddWithValue("@job", job)
                    command.Parameters.AddWithValue("@phone", txtphone.Text)
                    command.Parameters.AddWithValue("@email", txtemail.Text)
                    command.Parameters.AddWithValue("@photo", If(imageBytes IsNot Nothing, imageBytes, DBNull.Value))


                    If chkboxPass.Checked Then
                        command.Parameters.AddWithValue("@password", txtid.Text)
                    End If

                    command.Parameters.AddWithValue("@eid", txtid.Text)


                    command.ExecuteNonQuery()
                End Using


                Dim deleteScheduleQuery As String = "DELETE FROM Schedule WHERE Employee_ID = @eid"
                Using deleteCommand As New OleDb.OleDbCommand(deleteScheduleQuery, con)
                    deleteCommand.Parameters.AddWithValue("@eid", txtid.Text)
                    deleteCommand.ExecuteNonQuery()
                End Using

                Dim insertScheduleQuery As String = "INSERT INTO Schedule (Employee_ID, DayOfWeek, StartTime, EndTime) VALUES (@eid, @day, @start, @end)"
                Using insertCommand As New OleDb.OleDbCommand(insertScheduleQuery, con)
                    Dim days As New Dictionary(Of String, (Guna2DateTimePicker, Guna2DateTimePicker, CheckBox)) From {
                            {"Sunday", (DTPstartsun, DTPendsun, chkboxsun)},
                            {"Monday", (DTPstartmon, DTPendmon, chkboxmon)},
                            {"Tuesday", (DTPstarttues, DTPendtues, chkboxtues)},
                            {"Wednesday", (DTPstartwed, DTPendwed, chkboxwed)},
                            {"Thursday", (DTPstartthurs, DTPendthurs, chkboxthurs)},
                            {"Friday", (DTPstartfri, DTPendfri, chkboxfri)},
                            {"Saturday", (DTPstartsat, DTPendsat, chkboxsat)}
                        }

                    For Each day In days
                        If day.Value.Item3.Checked Then
                            insertCommand.Parameters.Clear()
                            insertCommand.Parameters.AddWithValue("@eid", txtid.Text)
                            insertCommand.Parameters.AddWithValue("@day", day.Key)
                            insertCommand.Parameters.AddWithValue("@start", day.Value.Item1.Value)
                            insertCommand.Parameters.AddWithValue("@end", day.Value.Item2.Value)
                            insertCommand.ExecuteNonQuery()
                        End If
                    Next
                End Using
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try


            txtfname.ReadOnly = True
            txtlname.ReadOnly = True
            txtaccnum.ReadOnly = True
            txtemail.ReadOnly = True
            txtphone.ReadOnly = True
            cbDepartment.Enabled = False
            cbJob.Enabled = False

            DTPstartsun.Visible = False
            DTPendsun.Visible = False
            chkboxsun.Visible = False
            DTPstartmon.Visible = False
            DTPendmon.Visible = False
            chkboxmon.Visible = False
            DTPstarttues.Visible = False
            DTPendtues.Visible = False
            chkboxtues.Visible = False
            DTPstartwed.Visible = False
            DTPendwed.Visible = False
            chkboxwed.Visible = False
            DTPstartthurs.Visible = False
            DTPendthurs.Visible = False
            chkboxthurs.Visible = False
            DTPstartfri.Visible = False
            DTPendfri.Visible = False
            chkboxfri.Visible = False
            DTPstartsat.Visible = False
            DTPendsat.Visible = False
            chkboxsat.Visible = False
            chkboxPass.Checked = False
            chkboxPass.Visible = False


            dgvschedule.Visible = True
            btnsave.Visible = False

            btnedit.Text = "Edit"
        End If
        Dim selectedemployeeid As String = txtid.Text
        DisplaySchedule(selectedemployeeid)

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query = "SELECT First_Name, Last_Name, Department, Job, Photo, Account_Num, Phone_Num, Email " &
                    "FROM Employee WHERE Employee_ID = @EmployeeID"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@EmployeeID", selectedemployeeid)

            Using reader = command.ExecuteReader()
                If reader.Read() Then

                    txtid.Text = selectedemployeeid
                    txtfname.Text = If(reader("First_Name").ToString(), "")
                    txtlname.Text = If(reader("Last_Name").ToString(), "")
                    txtaccnum.Text = If(reader("Account_Num").ToString(), "")
                    txtphone.Text = If(reader("Phone_Num").ToString(), "")
                    txtemail.Text = If(reader("Email").ToString(), "")

                    If Not IsDBNull(reader("Photo")) AndAlso reader("Photo") IsNot Nothing Then
                        Dim photoBytes = CType(reader("Photo"), Byte())
                        Using ms As New MemoryStream(photoBytes)
                            PictureBox1.Image = Image.FromStream(ms)
                            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                        End Using
                    Else
                        PictureBox1.Image = Nothing
                    End If

                    Dim department As String = reader("Department").ToString()
                    Dim job As String = reader("Job").ToString()

                    cbDepartment.SelectedItem = department
                    PopulateJobComboBox(department)

                    cbJob.SelectedItem = job

                    cbDepartment.Enabled = False
                    cbJob.Enabled = False
                    LoadEmployeeData()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"An error occurred Employee List: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub chkboxPass_CheckedChanged(sender As Object, e As EventArgs) Handles chkboxPass.CheckedChanged
        If chkboxPass.Checked Then

            Dim result As DialogResult = MessageBox.Show("Password will be reset to the employee's ID. Do you want to proceed?", "Confirm Password Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If result = DialogResult.No Then
                chkboxPass.Checked = False
            End If
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If btnedit.Text = "Cancel" Then
            Try
                Dim openFileDialog As New OpenFileDialog
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"

                If openFileDialog.ShowDialog = DialogResult.OK Then
                    PictureBox1.Image = Image.FromFile(openFileDialog.FileName)
                    PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage

                End If
            Catch ex As Exception
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If
    End Sub
    Private PayrollTable As DataTable
    Private Sub txtsearchnameid_TextChanged(sender As Object, e As EventArgs) Handles txtsearchnameid.TextChanged
        ApplyFilters()
    End Sub
    Private Sub cbsearchDepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbsearchdepartment.SelectedIndexChanged
        ApplyFilters()
    End Sub
    Private Sub ApplyFilters()
        If dgEmployeelist.Rows.Count = 0 Then Exit Sub

        Dim filterText As String = txtsearchnameid.Text.Trim().ToLower()
        Dim departmentFilter As String = cbsearchdepartment.SelectedItem?.ToString()

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

        cbsearchdepartment.DataSource = Nothing
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

        cbsearchdepartment.DataSource = sortedDepartments
    End Sub
End Class
