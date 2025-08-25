Imports System.Data.OleDb

Public Class AdminHolidays

    Public Sub LoadData()
        Dim adapter As OleDbDataAdapter
        Dim dtOriginal As DataTable
        Dim dtDisplay As New DataTable()

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            ' Load original data
            dtOriginal = New DataTable()
            adapter = New OleDbDataAdapter("SELECT * FROM Holidays", con)
            adapter.Fill(dtOriginal)

            ' Build new display table
            dtDisplay.Columns.Add("Holiday_ID", GetType(Integer))
            dtDisplay.Columns.Add("Description", GetType(String))
            dtDisplay.Columns.Add("Date", GetType(String))
            dtDisplay.Columns.Add("Type", GetType(String))
            dtDisplay.Columns.Add("RowType", GetType(String)) ' "Name" or "Date"

            For Each r As DataRow In dtOriginal.Rows
                ' Row 1: Holiday name (ID included)
                dtDisplay.Rows.Add(r("Holiday_ID"),
                               r("Holiday_Name").ToString(),
                               "",
                               r("Type").ToString(),
                               "Name")

                ' Row 2: Date (no ID here 👇)
                dtDisplay.Rows.Add(DBNull.Value,
                               "",
                               CDate(r("Holiday_Date")).ToShortDateString(),
                               "",
                               "Date")
            Next

            dgvholiday.DataSource = dtDisplay

            ' Hide RowType
            dgvholiday.Columns("RowType").Visible = False

            ' Adjust headers
            dgvholiday.Columns("Description").HeaderText = "Description"
            dgvholiday.Columns("Date").HeaderText = "Date"

            ' Make rows bigger
            dgvholiday.RowTemplate.Height = 40
            dgvholiday.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            For Each row As DataGridViewRow In dgvholiday.Rows
                row.Height = 40
            Next

            ' Add Edit button if not exists
            If dgvholiday.Columns("Edit") Is Nothing Then
                Dim btnEdit As New DataGridViewButtonColumn()
                btnEdit.Name = "Edit"
                btnEdit.Text = "Edit"
                btnEdit.UseColumnTextForButtonValue = True
                dgvholiday.Columns.Add(btnEdit)
            End If

            ' Add Delete button if not exists
            If dgvholiday.Columns("Delete") Is Nothing Then
                Dim btnDelete As New DataGridViewButtonColumn()
                btnDelete.Name = "Delete"
                btnDelete.Text = "Delete"
                btnDelete.UseColumnTextForButtonValue = True
                dgvholiday.Columns.Add(btnDelete)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
        End Try

        LoadTypes()
    End Sub
    Private Sub dgvholiday_RowPrePaint(sender As Object, e As DataGridViewRowPrePaintEventArgs) Handles dgvholiday.RowPrePaint
        Dim dgv = DirectCast(sender, DataGridView)
        Dim rowType = dgv.Rows(e.RowIndex).Cells("RowType").Value.ToString()

        ' Alternate backcolor by pairs (Name + Date)
        If rowType = "Name" Then
            dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.WhiteSmoke
        Else
            dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
        End If
    End Sub




    Private Sub dgvholiday_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvholiday.CellFormatting
        If dgvholiday.Columns(e.ColumnIndex).Name = "Edit" OrElse dgvholiday.Columns(e.ColumnIndex).Name = "Delete" Then
            Dim rowType As String = dgvholiday.Rows(e.RowIndex).Cells("RowType").Value.ToString()
            If rowType = "Date" Then
                e.Value = "" ' Remove button text for Date rows
                e.FormattingApplied = True
            End If
        End If
    End Sub

    Private Sub dgvholiday_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvholiday.CellPainting
        If e.RowIndex >= 0 AndAlso dgvholiday.Columns.Contains("RowType") Then
            Dim rowType = dgvholiday.Rows(e.RowIndex).Cells("RowType").Value.ToString()

            ' Draw bottom border only for Date rows
            If rowType = "Date" AndAlso e.ColumnIndex >= 0 Then
                e.Paint(e.CellBounds, DataGridViewPaintParts.All And Not DataGridViewPaintParts.Border)

                Dim gridLinePen As New Pen(Color.White, 2) ' 2px thick border
                Dim rect = e.CellBounds
                Dim y As Integer = rect.Bottom - 1
                e.Graphics.DrawLine(gridLinePen, rect.Left, y, rect.Right, y)

                e.Handled = True
            End If
        End If
    End Sub


    Private Sub dgvHoliday_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvHoliday.CellContentClick
        If e.ColumnIndex = dgvholiday.Columns("Edit").Index Then
            txtid.Text = dgvholiday.Rows(e.RowIndex).Cells("Holiday_ID").Value.ToString()
            txtname.Text = dgvholiday.Rows(e.RowIndex).Cells("Holiday_Name").Value.ToString()
            datetimepicker1.Value = Convert.ToDateTime(dgvholiday.Rows(e.RowIndex).Cells("Holiday_Date").Value)
            cbtype.Text = dgvholiday.Rows(e.RowIndex).Cells("Type").Value.ToString()
        ElseIf e.ColumnIndex = dgvholiday.Columns("Delete").Index Then
            Dim holidayID As Integer = Convert.ToInt32(dgvHoliday.Rows(e.RowIndex).Cells("Holiday_ID").Value)
            DeleteHoliday(holidayID)
        End If
    End Sub

    Private Sub LoadTypes()
        cbtype.Items.Clear()
        cbaddtype.Items.Clear()
        cbtype.Items.Add("Regular Holiday")
        cbtype.Items.Add("Special Non-Working")
        cbaddtype.Items.Add("Regular Holiday")
        cbaddtype.Items.Add("Special Non-Working")
    End Sub

    Private Sub DeleteHoliday(holidayID As Integer)

        Dim cmd As OleDbCommand
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim result = MessageBox.Show("Are you sure you want to delete this holiday? This action cannot be undone.",
                                     "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                     MessageBoxDefaultButton.Button2)

            If result = DialogResult.Yes Then
                cmd = New OleDbCommand("DELETE FROM Holidays WHERE Holiday_ID = @HolidayID", con)
                cmd.Parameters.AddWithValue("@HolidayID", holidayID)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Holiday deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

                LoadData()
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred while deleting the holiday: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        Try
            If con.State = ConnectionState.Closed Then
                con.Open
            End If
            If String.IsNullOrWhiteSpace(txtname.Text) Then
                MessageBox.Show("Holiday Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtname.Focus
                Exit Sub
            End If

            Dim result = MessageBox.Show("Are you sure you want to edit this holiday? This action cannot be undone.",
                                     "Editing Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                     MessageBoxDefaultButton.Button2)

            If result = DialogResult.No Then
                Exit Sub
            End If

            Dim holidayID = Convert.ToInt32(txtid.Text)
            Dim holidayName = txtname.Text
            Dim holidayDate = datetimepicker1.Value
            Dim holidayType = cbtype.SelectedItem.ToString
            Dim rate As Decimal

            If holidayType = "Regular Holiday" Then
                rate = 2
            ElseIf holidayType = "Special Non-Working" Then
                rate = 1.3
            Else
                MessageBox.Show("Invalid holiday type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim cmd As OleDbCommand
            cmd = New OleDbCommand("UPDATE Holidays SET Holiday_Name = @HolidayName, Holiday_Date = @HolidayDate, Type = @HolidayType, Rate = @Rate WHERE Holiday_ID = @HolidayID", con)
            cmd.Parameters.AddWithValue("@HolidayName", holidayName)
            cmd.Parameters.AddWithValue("@HolidayDate", holidayDate)
            cmd.Parameters.AddWithValue("@HolidayType", holidayType)
            cmd.Parameters.AddWithValue("@Rate", rate)
            cmd.Parameters.AddWithValue("@HolidayID", holidayID)

            cmd.ExecuteNonQuery
            MessageBox.Show("Holiday updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ClearFields

            LoadData
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close
            End If
        End Try
    End Sub

    Private Sub ClearFields()
        txtid.Clear()
        txtname.Clear()
        cbtype.SelectedIndex = -1
        datetimepicker1.Value = DateTime.Now
    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click
        Dim cmd As OleDbCommand

        Try
            If String.IsNullOrWhiteSpace(txtaddname.Text) Then
                MessageBox.Show("Holiday Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtaddname.Focus()
                Exit Sub
            End If

            If cbaddtype.SelectedIndex = -1 Then
                MessageBox.Show("Please select a holiday type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                cbaddtype.Focus()
                Exit Sub
            End If

            Dim holidayName = txtaddname.Text
            Dim holidayDate = cbadddate.Value
            Dim holidayType = cbaddtype.SelectedItem.ToString()
            Dim rate As Decimal

            If holidayType = "Regular Holiday" Then
                rate = 2
            ElseIf holidayType = "Special Non-Working" Then
                rate = 1.3
            Else
                MessageBox.Show("Invalid holiday type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim result = MessageBox.Show("Are you sure you want to add this holiday?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then
                Exit Sub
            End If

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OleDbCommand("INSERT INTO Holidays (Holiday_Name, Holiday_Date, Type, Rate) VALUES (@HolidayName, @HolidayDate, @HolidayType, @Rate)", con)
            cmd.Parameters.AddWithValue("@HolidayName", holidayName)
            cmd.Parameters.AddWithValue("@HolidayDate", holidayDate)
            cmd.Parameters.AddWithValue("@HolidayType", holidayType)
            cmd.Parameters.AddWithValue("@Rate", rate)

            cmd.ExecuteNonQuery()
            MessageBox.Show("Holiday added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ClearAddFields()

            LoadData()
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub ClearAddFields()
        txtaddname.Clear()
        cbaddtype.SelectedIndex = -1
        cbadddate.Value = DateTime.Now
    End Sub

    Private Sub cbaddtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbaddtype.SelectedIndexChanged

    End Sub
End Class