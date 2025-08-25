Imports System.Data.OleDb
Imports System.Globalization

Public Class AdminSchedule
    Private Sub CalendarForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Configure the DataGridView for the calendar
        ConfigureDataGridView()

        ' Populate the month ComboBox with month names
        For i As Integer = 1 To 12
            ComboBoxMonth.Items.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i))
        Next

        ' Set the ComboBoxMonth to the current month
        ComboBoxMonth.SelectedIndex = DateTime.Now.Month - 1 ' Month is 1-based, ComboBox index is 0-based

        ' Populate the calendar with the current month's dates
        FillCalendar(DateTime.Now.Year, DateTime.Now.Month)
    End Sub


    Private Sub ConfigureDataGridView()
        ' Set up column headers for days of the week
        Dim daysOfWeek As String() = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}
        For Each day As String In daysOfWeek
            datagridview.Columns.Add(day, day)
        Next

        ' Configure the appearance and behavior of the DataGridView
        With datagridview
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .ReadOnly = True
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .RowTemplate.Height = 50 ' Set row height for better visibility
            .RowHeadersVisible = False ' Hide row headers
        End With
    End Sub

    Private Sub FillCalendar(year As Integer, month As Integer)
        ' Clear the DataGridView
        datagridview.Rows.Clear()

        ' Get the first day of the month and the number of days in the month
        Dim firstDayOfMonth As New DateTime(year, month, 1)
        Dim daysInMonth As Integer = DateTime.DaysInMonth(year, month)
        Dim startDayIndex As Integer = CInt(firstDayOfMonth.DayOfWeek)

        ' Fill the calendar
        Dim currentDay As Integer = 1
        Dim weekRow As DataGridViewRow = Nothing

        While currentDay <= daysInMonth
            If weekRow Is Nothing Then weekRow = New DataGridViewRow()
            weekRow.CreateCells(datagridview)

            For i As Integer = 0 To 6 ' Loop through all days of the week
                If (currentDay = 1 AndAlso i < startDayIndex) OrElse currentDay > daysInMonth Then
                    weekRow.Cells(i).Value = "" ' Empty cell for days outside the month
                Else
                    weekRow.Cells(i).Value = currentDay.ToString() ' Set the date
                    currentDay += 1
                End If
            Next

            ' Add the week row to the DataGridView
            datagridview.Rows.Add(weekRow)
            weekRow = Nothing ' Reset the row for the next week
        End While
    End Sub

    Private Sub ComboBoxMonth_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxMonth.SelectedIndexChanged
        ' Update the calendar when the user selects a different month
        Dim selectedMonth As Integer = ComboBoxMonth.SelectedIndex + 1
        Dim currentYear As Integer = DateTime.Now.Year ' You can adjust this for year selection
        FillCalendar(currentYear, selectedMonth)
    End Sub


    Private Sub datagridview_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagridview.CellClick
        ' Check if the clicked cell is valid
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            Dim cellValue As String = datagridview.Rows(e.RowIndex).Cells(e.ColumnIndex).Value?.ToString()

            ' Ensure the cell contains a date
            If Not String.IsNullOrEmpty(cellValue) AndAlso IsNumeric(cellValue) Then
                ' Get the selected day
                Dim selectedDay As Integer = CInt(cellValue)
                Dim selectedMonth As Integer = ComboBoxMonth.SelectedIndex + 1
                Dim selectedYear As Integer = DateTime.Now.Year ' Adjust for a year selector if needed

                ' Create a DateTime object for the selected date
                Dim selectedDate As New DateTime(selectedYear, selectedMonth, selectedDay)

                ' Get the day of the week
                Dim dayOfWeek As String = selectedDate.DayOfWeek.ToString()

                ' Update the label with the day of the week
                lbldayofweek.Text = $"Selected Date: {selectedDate.ToShortDateString()} - Day: {dayOfWeek}"

                ' Populate the schedule DataGridView
                PopulateSchedule(dayOfWeek)
            End If
        End If
    End Sub


    Private Sub PopulateSchedule(dayOfWeek As String)
        ' Use the shared connection from the SystemModule
        Dim query As String = "
    SELECT 
        e.Employee_ID, 
        e.First_Name, 
        e.Last_Name, 
        s.startTime, 
        s.endTime
    FROM 
        schedule AS s
    INNER JOIN 
        employee AS e
    ON 
        s.Employee_ID = e.Employee_ID
    WHERE 
        s.dayOfWeek = @dayOfWeek"

        Using command As New OleDbCommand(query, con)
            ' Add parameter for dayOfWeek
            command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek)

            ' Create a DataTable to hold the data
            Dim dataTable As New DataTable()

            Try
                ' Ensure the connection is open
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                ' Execute the query and fill the DataTable
                Dim adapter As New OleDbDataAdapter(command)
                adapter.Fill(dataTable)

                ' Clear any existing columns to ensure fresh data binding
                showsched.Columns.Clear()

                ' Bind the DataTable to the DataGridView
                showsched.DataSource = dataTable

                ' Customize column headers
                If showsched.Columns.Contains("Employee_ID") Then
                    showsched.Columns("Employee_ID").HeaderText = "Employee ID"
                End If
                If showsched.Columns.Contains("First_Name") Then
                    showsched.Columns("First_Name").HeaderText = "First Name"
                End If
                If showsched.Columns.Contains("Last_Name") Then
                    showsched.Columns("Last_Name").HeaderText = "Last Name"
                End If
                If showsched.Columns.Contains("startTime") Then
                    showsched.Columns("startTime").HeaderText = "Start Time"
                    showsched.Columns("startTime").DefaultCellStyle.Format = "hh:mm tt" ' Format time
                End If
                If showsched.Columns.Contains("endTime") Then
                    showsched.Columns("endTime").HeaderText = "End Time"
                    showsched.Columns("endTime").DefaultCellStyle.Format = "hh:mm tt" ' Format time
                End If

                ' Ensure dayOfWeek column is removed
                If showsched.Columns.Contains("dayOfWeek") Then
                    showsched.Columns.Remove("dayOfWeek")
                End If
            Catch ex As Exception
                MessageBox.Show($"Error fetching schedule: {ex.Message}")
            Finally
                ' Optional: Close the connection if it's not shared for longer use
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End Using
    End Sub


End Class