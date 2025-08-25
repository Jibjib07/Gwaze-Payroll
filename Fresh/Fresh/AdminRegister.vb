Imports System.Data.OleDb
Imports Guna.UI2.WinForms

Public Class AdminRegister
    Private Sub AdminRegister_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateDepartmentComboBox()
        cbJob.Enabled = False
        LoadNewEmployeeID()
    End Sub
    Private Sub cbjob_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbJob.SelectedIndexChanged
        If cbJob.SelectedValue IsNot Nothing AndAlso IsNumeric(cbJob.SelectedValue) Then
            UpdateAgreedSalary()
        End If
    End Sub
    Private Sub PopulateDepartmentComboBox()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim cmd As New OleDbCommand("SELECT DISTINCT Department FROM Job", con)
            Dim reader As OleDbDataReader = cmd.ExecuteReader()

            cbDepartment.Items.Clear()
            While reader.Read()
                cbDepartment.Items.Add(reader("Department").ToString())
            End While
            reader.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub PopulateJobComboBox(department As String)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim cmd As New OleDbCommand("SELECT Job_ID, Job FROM Job WHERE Department = @Department", con)
            cmd.Parameters.AddWithValue("@Department", department)
            Dim adapter As New OleDbDataAdapter(cmd)
            Dim dt As New DataTable()

            adapter.Fill(dt)

            cbJob.DataSource = Nothing
            cbJob.Items.Clear()
            cbJob.DataSource = dt
            cbJob.DisplayMember = "Job"
            cbJob.ValueMember = "Job_ID"
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
            Dim selectedDepartment As String = cbDepartment.SelectedItem.ToString()
            cbJob.Enabled = True
            PopulateJobComboBox(selectedDepartment)
        Else
            cbJob.Enabled = False
            cbJob.Items.Clear()
        End If
    End Sub


    Private Sub PictureBox_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Try
            Dim openFileDialog As New OpenFileDialog
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"

            If openFileDialog.ShowDialog = DialogResult.OK Then
                PictureBox1.Image = Image.FromFile(openFileDialog.FileName)
                PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage

                MessageBox.Show("Photo uploaded successfully!")
            End If
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub btncreate_Click(sender As Object, e As EventArgs) Handles btncreate.Click
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Dim eid As Integer = CInt(txtemployeeid.Text)
        Dim firstName As String = txtfirstname.Text
        Dim lastName As String = txtlastname.Text
        Dim birthDate As Date = txtdob.Value
        Dim accountNumber As String = txtaccountnumber.Text
        Dim department As String = cbDepartment.Text
        Dim job As String = cbJob.Text
        Dim dateHired As Date = txtdh.Value
        Dim phoneNum As String = txtcono.Text
        Dim email As String = txtemailadd.Text
        Dim password As String = txtpassword.Text


        Dim inputFields As New Dictionary(Of String, Object) From {
        {"First Name", firstName},
        {"Last Name", lastName},
        {"Birth Date", birthDate},
        {"Account Number", accountNumber},
        {"Date Hired", dateHired},
        {"Phone Number", phoneNum},
        {"Email", email},
        {"Password", password}
    }

        For Each field In inputFields
            If String.IsNullOrEmpty(field.Value.ToString()) OrElse (TypeOf field.Value Is Integer AndAlso CInt(field.Value) = -1) Then
                MessageBox.Show($"{field.Key} is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        Next


        If Not IsValidEmail(email) Then
            MessageBox.Show("Invalid email address format. Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtemailadd.Focus()
            Exit Sub
        End If


        Dim imageBytes As Byte() = Nothing

        If PictureBox1.Image IsNot Nothing Then
            Dim ms As New IO.MemoryStream()
            PictureBox1.Image.Save(ms, PictureBox1.Image.RawFormat)
            imageBytes = ms.ToArray()
        Else
            MessageBox.Show("Please upload a picture for the employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Try

            Dim query As String = "INSERT INTO employee([employee_id], [first_name], [last_name], [birthdate], [account_num], [Department], [job], [date_hired], [phone_num], [email], [password], [type], [photo]) VALUES (@eid, @fname, @lname, @birth, @accnum, @department, @job, @date_hired, @phonem, @email, @pass, @type, @photo)"
            Dim command As New OleDbCommand(query, con)
            Dim type As String = "Client"

            command.Parameters.AddWithValue("@eid", eid)
            command.Parameters.AddWithValue("@fname", firstName)
            command.Parameters.AddWithValue("@lname", lastName)
            command.Parameters.AddWithValue("@birth", birthDate)
            command.Parameters.AddWithValue("@accnum", accountNumber)
            command.Parameters.AddWithValue("@department", department)
            command.Parameters.AddWithValue("@job", job)
            command.Parameters.AddWithValue("@date_hired", dateHired)
            command.Parameters.AddWithValue("@phonem", phoneNum)
            command.Parameters.AddWithValue("@email", email)
            command.Parameters.AddWithValue("@pass", password)
            command.Parameters.AddWithValue("@type", type)
            command.Parameters.AddWithValue("@photo", If(imageBytes IsNot Nothing, imageBytes, DBNull.Value))

            command.ExecuteNonQuery()

            Dim insertScheduleQuery As String = "INSERT INTO Schedule (employee_id, dayofweek, starttime, endtime) VALUES (@eid, @dayofweek, @starttime, @endtime)"
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
                    Using scheduleCmd As New OleDbCommand(insertScheduleQuery, con)
                        scheduleCmd.Parameters.AddWithValue("@eid", eid)
                        scheduleCmd.Parameters.AddWithValue("@dayofweek", day.Key)
                        scheduleCmd.Parameters.AddWithValue("@starttime", day.Value.Item1.Value)
                        scheduleCmd.Parameters.AddWithValue("@endtime", day.Value.Item2.Value)
                        scheduleCmd.ExecuteNonQuery()
                    End Using
                End If
            Next

            MessageBox.Show("Employee and schedule added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


            LoadNewEmployeeID()
            txtfirstname.Text = ""
            txtlastname.Text = ""
            txtaccountnumber.Text = ""
            txtdh.Value = DateTime.Now
            txtdob.Value = DateTime.Now
            txtcono.Text = ""
            txtemailadd.Text = ""
            txtpassword.Text = ""
            cbJob.SelectedIndex = -1
            DTPstartsun.Value = DateTime.Now
            DTPendsun.Value = DateTime.Now.AddHours(8)
            PictureBox1.Image = My.Resources.blank_pfp
            chkboxsun.Checked = False
            chkboxmon.Checked = False

            For Each day In days
                day.Value.Item1.Value = DateTime.Today.AddHours(9)
                day.Value.Item2.Value = DateTime.Today.AddHours(17)
                day.Value.Item3.Checked = False
            Next

        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
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

    Private Function IsValidEmail(email As String) As Boolean
        Dim emailRegex As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Dim regex As New System.Text.RegularExpressions.Regex(emailRegex)
        Return regex.IsMatch(email)
    End Function


    Private Sub UpdateAgreedSalary()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Salary, Anual, Pay_rate FROM Job WHERE Job_ID = @JobID"
            Dim command As New OleDbCommand(query, con)
            command.Parameters.AddWithValue("@JobID", CInt(cbJob.SelectedValue))

            Dim reader As OleDbDataReader = command.ExecuteReader()

            If reader.HasRows Then
                While reader.Read()
                    txtagreedsalary.Text = reader("Salary").ToString()
                    txtAnual.Text = reader("Anual").ToString()
                    txtHourly.Text = reader("Pay_rate").ToString()
                End While
            Else
                txtagreedsalary.Text = "0"
                txtAnual.Text = "0"
                txtHourly.Text = "0"
            End If

            reader.Close()
        Catch ex As Exception
            MessageBox.Show($"An error occurred while fetching the data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub LoadNewEmployeeID()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT MAX(Employee_ID) FROM Employee"
            Dim command As New OleDbCommand(query, con)
            Dim result As Object = command.ExecuteScalar()
            txtemployeeid.Text = (CInt(result) + 1).ToString()
            txtpassword.Text = txtemployeeid.Text

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

End Class