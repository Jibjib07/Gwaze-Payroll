Imports System.Data.OleDb
Imports System.IO

Public Class AdminDashboard

    Private holidayToolTip As New ToolTip()

    Private holidayDict As New Dictionary(Of DateTime, String)()
    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblDate.Text = DateTime.Now.ToString("MM/dd/yyyy")

    End Sub

    Public Sub FillEmployeeCounts()
        Try

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Department, COUNT(*) AS EmployeeCount FROM Employee GROUP BY Department"

            Dim employeeCounts As New Dictionary(Of String, Integer)
            Dim totalEmployees As Integer = 0

            Using cmd As New OleDbCommand(query, con)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim department As String = reader("Department").ToString()
                        Dim count As Integer = Convert.ToInt32(reader("EmployeeCount"))
                        employeeCounts(department) = count

                        totalEmployees += count
                    End While
                End Using
            End Using

            lblLobbyCount.Text = $"{If(employeeCounts.ContainsKey("Lobby"), employeeCounts("Lobby"), 0)}"
            lblPayrollCount.Text = $"{If(employeeCounts.ContainsKey("Payroll"), employeeCounts("Payroll"), 0)}"
            lblKitchenCount.Text = $"{If(employeeCounts.ContainsKey("Kitchen"), employeeCounts("Kitchen"), 0)}"
            lblSecurityCount.Text = $"{If(employeeCounts.ContainsKey("Security"), employeeCounts("Security"), 0)}"

            lbltotal.Text = $"{totalEmployees}"
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Public Sub UpdateProgressAndTotalTime()

        Dim today = lblDate.Text
        Dim recordedCount As Integer = 0
        Dim totalEmployees As Integer = 0

        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try
            Dim queryRecorded As String = "SELECT COUNT(*) AS RecordedCount FROM Clocking_Log WHERE Status = 'Recorded' AND Work_Date = ?"
            Using commandRecorded As New OleDbCommand(queryRecorded, con)
                commandRecorded.Parameters.AddWithValue("?", today)
                recordedCount = Convert.ToInt32(commandRecorded.ExecuteScalar())
            End Using

            Dim queryTotalEmployees As String = "SELECT COUNT(*) AS TotalEmployees FROM Employee"
            Using commandTotalEmployees As New OleDbCommand(queryTotalEmployees, con)
                totalEmployees = Convert.ToInt32(commandTotalEmployees.ExecuteScalar())
            End Using

            lbltotaltimedin.Text = $"{recordedCount}"

            Dim progress As Integer = Math.Min((recordedCount / totalEmployees) * 100, 100)
            pbattendance.Value = progress

        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Public Sub HighlightHolidays()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim query As String = "SELECT Holiday_Date, Holiday_Name FROM Holidays"
            Dim cmd As New OleDbCommand(query, con)

            Dim reader As OleDbDataReader = cmd.ExecuteReader()
            While reader.Read()
                Dim holidayDate As DateTime
                Dim holidayName As String = reader("Holiday_Name").ToString()

                If DateTime.TryParse(reader("Holiday_Date").ToString(), holidayDate) Then
                    holidayDict(holidayDate) = holidayName
                End If
            End While

            MonthCalendar1.BoldedDates = holidayDict.Keys.ToArray()
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub MonthCalendar1_MouseMove(sender As Object, e As MouseEventArgs) Handles MonthCalendar1.MouseMove
        Dim hitTestInfo As MonthCalendar.HitTestInfo = MonthCalendar1.HitTest(e.Location)

        If hitTestInfo.HitArea = MonthCalendar.HitArea.Date Then
            Dim hoveredDate As DateTime = hitTestInfo.Time

            If holidayDict.ContainsKey(hoveredDate) Then
                holidayToolTip.SetToolTip(MonthCalendar1, holidayDict(hoveredDate))
            Else
                holidayToolTip.SetToolTip(MonthCalendar1, String.Empty)
            End If
        End If
    End Sub


    Public Sub loadclockinlog()
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try
            Dim query As String = "SELECT Clocking_ID, Work_Date, Employee_ID, Start_Time, End_Time, Hours_Worked, OT_worked, Status " &
                          "FROM Clocking_Log"

            Dim cmd As New OleDbCommand(query, con)

            Dim adapter As New OleDbDataAdapter(cmd)
            Dim table As New DataTable()

            adapter.Fill(table)

            dgvEmployee.Rows.Clear()
            dgvEmployee.Columns.Clear()

            dgvEmployee.Columns.Add("Employee_ID", "Employee ID")
            dgvEmployee.Columns.Add("Work_Date", "Work Date")

            dgvEmployee.Columns.Add("Start_Time", "Start Time")
            dgvEmployee.Columns.Add("End_Time", "End Time")

            Dim colClockingID As New DataGridViewTextBoxColumn()
            colClockingID.Name = "Clocking_ID"
            colClockingID.HeaderText = "Clocking ID"
            colClockingID.Visible = False
            dgvEmployee.Columns.Add(colClockingID)

            Dim colHoursWorked As New DataGridViewTextBoxColumn()
            colHoursWorked.Name = "Hours_Worked"
            colHoursWorked.HeaderText = "Hours Worked"
            colHoursWorked.Visible = False
            dgvEmployee.Columns.Add(colHoursWorked)

            Dim colOTWorked As New DataGridViewTextBoxColumn()
            colOTWorked.Name = "OT_worked"
            colOTWorked.HeaderText = "OT Worked"
            colOTWorked.Visible = False
            dgvEmployee.Columns.Add(colOTWorked)

            Dim colStatus As New DataGridViewTextBoxColumn()
            colStatus.Name = "Status"
            colStatus.HeaderText = "Status"
            colStatus.Visible = False
            dgvEmployee.Columns.Add(colStatus)

            For Each row As DataRow In table.Rows
                dgvEmployee.Rows.Add(row("Employee_ID"), row("Work_Date"),
                                     row("Start_Time"), row("End_Time"),
                                     row("Clocking_ID"), row("Hours_Worked"),
                                     row("OT_worked"), row("Status"))
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading clocking log: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub
End Class