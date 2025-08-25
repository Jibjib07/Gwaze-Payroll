Imports System.Data.OleDb
Imports System.IO
Imports System.Windows.Media.Animation

Public Class ClientDashboard
    Private holidayToolTip As New ToolTip()

    Private holidayDict As New Dictionary(Of DateTime, String)()
    Private Sub ClientDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateEmployeeDetails()
        lbldate.Text = DateTime.Now.ToString("MM/dd/yyyy")
        CheckRecord()
        PopulateDataGrid()
        HighlightHolidays()
    End Sub
    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles lblsys_timer.Tick
        lblcurrenttime.Text = DateTime.Now.ToString("hh:mm:ss tt")
    End Sub
    Private elapsedTime As Integer = 0
    Private Sub lblworkingtime_timer_Tick(sender As Object, e As EventArgs) Handles lblworkingtime_timer.Tick
        elapsedTime += 1
        lblworkingtime.Text = TimeSpan.FromSeconds(elapsedTime).ToString("hh\:mm\:ss")
        UpdateActivetime()
        PopulateDataGrid()

    End Sub
    Private Sub btntimein_Click(sender As Object, e As EventArgs) Handles btntimein.Click
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim selectedID As String = MainInterface.lblID.Text
            Dim selectedName As String = lblname.Text
            Dim currentTime As String = lblcurrenttime.Text
            Dim currentDate As String = lbldate.Text
            Dim activeTime As String = lblworkingtime.Text

            'SECONDS For Presentation
            Dim timeSpan As TimeSpan = TimeSpan.Parse(activeTime)
            Dim totalSeconds As Integer = CInt(timeSpan.TotalSeconds)

            'HOURS Normal
            'Dim timeSpan As TimeSpan = TimeSpan.Parse(activeTime)
            'Dim totalSeconds As Integer = CInt(timeSpan.TotalHours)

            If btntimein.Text = "Time In" Then
                lblactivestatus.Text = "Active"
                lblactivestatus.ForeColor = Color.Lime
                btntimein.Text = "Time out"
                btntimein.FillColor = Color.Red
                btnbreak.Enabled = True
                btnbreak.FillColor = Color.Orange
                elapsedTime = 0
                lblworkingtime_timer.Enabled = True

                Dim query As String = "INSERT INTO Clocking_Log([Employee_ID], [Start_Time], [Status], [Work_Date]) VALUES (@Id, @Start, @Status, @date)"
                Using command As New OleDbCommand(query, con)
                    command.Parameters.AddWithValue("@Id", selectedID)
                    command.Parameters.AddWithValue("@Start", currentTime)
                    command.Parameters.AddWithValue("@Status", "Active")
                    command.Parameters.AddWithValue("@date", currentDate)
                    command.ExecuteNonQuery()
                End Using

                PopulateDataGrid()
            Else
                btntimein.Text = "Recorded"
                lblactivestatus.Text = "Recorded"
                lblactivestatus.ForeColor = Color.DodgerBlue
                btnbreak.Enabled = False
                btntimein.Enabled = False
                lblworkingtime_timer.Enabled = False

                Dim timeOutQuery As String = "UPDATE Clocking_log SET [End_Time] = ?, [Status] = ?, [Active_Time] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
                Using command As New OleDbCommand(timeOutQuery, con)
                    command.Parameters.AddWithValue("?", currentTime)
                    command.Parameters.AddWithValue("?", "Recorded")
                    command.Parameters.AddWithValue("?", activeTime)
                    command.Parameters.AddWithValue("?", selectedID)
                    command.Parameters.AddWithValue("?", currentDate)
                    command.ExecuteNonQuery()
                End Using

                If totalSeconds > 8 Then
                    Dim overtime As Double = totalSeconds - 8
                    Dim convertedSeconds As Double = 8

                    Dim workQuery As String = "UPDATE Clocking_log SET [Hours_Worked] = ?, [OT_Worked] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
                    Using command As New OleDbCommand(workQuery, con)
                        command.Parameters.AddWithValue("@HoursWorked", convertedSeconds)
                        command.Parameters.AddWithValue("@OTWorked", overtime)
                        command.Parameters.AddWithValue("@EmployeeID", selectedID)
                        command.Parameters.AddWithValue("@WorkDate", currentDate)
                        command.ExecuteNonQuery()
                    End Using

                    Dim updatePayQuery As String = "UPDATE Payroll SET [Hours_Worked] = [Hours_Worked] + ?, [OT_Worked] = [OT_Worked] + ? WHERE [Employee_ID] = ?"
                    Using command As New OleDbCommand(updatePayQuery, con)
                        command.Parameters.AddWithValue("@HoursWorked", convertedSeconds)
                        command.Parameters.AddWithValue("@OTWorked", overtime)
                        command.Parameters.AddWithValue("@EmployeeID", selectedID)
                        command.ExecuteNonQuery()
                    End Using


                Else
                    Dim workQuery As String = "UPDATE Clocking_log SET [Hours_Worked] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
                    Using command As New OleDbCommand(workQuery, con)
                        command.Parameters.AddWithValue("@HoursWorked", totalSeconds)
                        command.Parameters.AddWithValue("@EmployeeID", selectedID)
                        command.Parameters.AddWithValue("@WorkDate", currentDate)
                        command.ExecuteNonQuery()
                    End Using

                    Dim updatePayQuery As String = "UPDATE Payroll SET [Hours_Worked] = [Hours_Worked] + ? WHERE [Employee_ID] = ?"
                    Using command As New OleDbCommand(updatePayQuery, con)
                        command.Parameters.AddWithValue("@HoursWorked", totalSeconds)
                        command.Parameters.AddWithValue("@EmployeeID", selectedID)
                        command.ExecuteNonQuery()
                    End Using


                End If
                CalculateAndInsertBonus()
            End If
            PopulateDataGrid()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub btnbreak_Click(sender As Object, e As EventArgs) Handles btnbreak.Click
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Dim selectedID As String = MainInterface.lblID.Text
        Dim currentDate As String = lbldate.Text

        If btnbreak.Text = "Break Time" Then
            btntimein.Enabled = False
            lblactivestatus.Text = "Break"
            lblactivestatus.ForeColor = Color.Orange
            btnbreak.Text = "Resume"
            btnbreak.FillColor = Color.Lime
            lblworkingtime_timer.Enabled = False
            Try
                Dim query As String = "UPDATE Clocking_log SET [Status] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
                Using command As New OleDbCommand(query, con)
                    command.Parameters.AddWithValue("@Status", "Break")
                    command.Parameters.AddWithValue("@EmployeeID", selectedID)
                    command.Parameters.AddWithValue("@WorkDate", currentDate)

                    command.ExecuteNonQuery()
                End Using

                PopulateDataGrid()
            Catch ex As Exception
                MessageBox.Show($"Error Break: {ex.Message}")
            End Try
        Else
            lblactivestatus.Text = "Active"
            lblactivestatus.ForeColor = Color.Lime
            btntimein.Enabled = True
            btnbreak.Text = "Resume"
            btnbreak.FillColor = Color.Gray
            btnbreak.Enabled = False
            lblworkingtime_timer.Enabled = True
            Try
                Dim query As String = "UPDATE Clocking_log SET [Status] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
                Using command As New OleDbCommand(query, con)
                    command.Parameters.AddWithValue("@Status", "Active")
                    command.Parameters.AddWithValue("@EmployeeID", selectedID)
                    command.Parameters.AddWithValue("@WorkDate", currentDate)

                    command.ExecuteNonQuery()
                End Using

                PopulateDataGrid()
            Catch ex As Exception
                MessageBox.Show($"Error Resume: {ex.Message}")
            End Try
        End If
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
    End Sub

    Private Sub PopulateEmployeeDetails()

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim employeeID As String = MainInterface.lblID.Text

            Dim employeeQuery As String = "SELECT Employee_ID, First_name, Last_name, email, Department, Job FROM Employee WHERE Employee_ID = @EmployeeID"
            Dim employeeCommand As New OleDbCommand(employeeQuery, con)
            employeeCommand.Parameters.AddWithValue("@EmployeeID", employeeID)

            Dim reader As OleDbDataReader = employeeCommand.ExecuteReader()

            If reader.HasRows Then
                reader.Read()

                Dim Department As String = reader("Department").ToString()
                Dim job As String = reader("job").ToString()
                lblname.Text = $"{reader("First_name")} {reader("Last_name")}"
                Dim employeename As String = $"{reader("First_name")} {reader("Last_name")}"
                lblemail.Text = reader("email").ToString()
                lblDepartment.Text = Department
                lbljob.Text = job

                reader.Close()

                Dim jobQuery As String = "SELECT Job, Salary FROM Job WHERE Job = @Job AND Department = @Department"
                Dim jobCommand As New OleDbCommand(jobQuery, con)
                jobCommand.Parameters.AddWithValue("@JobID", job)
                jobCommand.Parameters.AddWithValue("@Department", Department)

                Dim jobReader As OleDbDataReader = jobCommand.ExecuteReader()
                If jobReader.HasRows Then
                    jobReader.Read()
                    lblmonthlypayroll.Text = jobReader("Salary").ToString()
                End If

                jobReader.Close()
                CheckPayrollRecord()
            End If

            reader.Close()
        Catch ex As Exception
            MessageBox.Show($"Error Populateemployeedetails: {ex.Message}")
        End Try
    End Sub
    Private Sub UpdateLabels()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim payrollID As Integer = lblpayrollid.Text
            If payrollID > 0 Then
                Dim query As String = "SELECT DATE_DUE FROM Payroll WHERE Payroll_ID = @PayrollID"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.Add("@PayrollID", OleDbType.Integer).Value = payrollID
                Dim dateDue As Object = cmd.ExecuteScalar()

                If dateDue IsNot Nothing AndAlso dateDue IsNot DBNull.Value Then
                    Dim daysWithin As Integer = DateDiff(DateInterval.Day, DateTime.Now.Date, CDate(dateDue))
                    lblnotif.Text = $"Your payroll cutoff is within {daysWithin} day(s)"
                    lblupcomingdate.Text = CDate(dateDue).ToString("MMMM d, yyyy")
                End If
            End Using
            End If
        Catch ex As Exception
            MessageBox.Show($"Error in UpdateLabels: {ex.Message}")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function CheckPayrollRecord() As Integer
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim employeeID As String = MainInterface.lblID.Text
            Dim query As String = "SELECT Payroll_ID FROM Payroll WHERE Employee_ID = @EmployeeID AND Payroll_Status = 'Unreleased'"
            Using cmd As New OleDbCommand(query, con)
                cmd.Parameters.Add("@EmployeeID", OleDbType.Integer).Value = employeeID
                Dim result As Object = cmd.ExecuteScalar()

                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    lblpayrollid.Text = CStr(result)
                    UpdateLabels()
                Else
                    CreateNewPayrollRecord()
                    Return CheckPayrollRecord()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error in CheckPayrollRecord: {ex.Message}")
            Return -1
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Function
    Private Sub CreateNewPayrollRecord()
        Dim employeeID As String = MainInterface.lblID.Text
        Dim employeename As String = lblname.Text
        Dim job As String = lbljob.Text
        Dim department As String = lblDepartment.Text
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

            Dim salary As Decimal = 0, anual As Decimal = 0, payRate As Decimal = 0

            Dim queryPayRate As String = "SELECT Salary, Anual, Pay_rate FROM Job WHERE Job = @Job AND Department = @Department"
            Using commandPayRate As New OleDbCommand(queryPayRate, con)
                commandPayRate.Parameters.Add("@Job", OleDbType.VarChar).Value = job
                commandPayRate.Parameters.Add("@Department", OleDbType.VarChar).Value = department

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
                commandInsert.Parameters.Add("@EmployeeName", OleDbType.VarChar).Value = employeename
                commandInsert.Parameters.Add("@Department", OleDbType.VarChar).Value = department
                commandInsert.Parameters.Add("@Job", OleDbType.VarChar).Value = job
                commandInsert.Parameters.Add("@PayRate", OleDbType.Decimal).Value = payRate
                commandInsert.Parameters.Add("@Created", OleDbType.Date).Value = currentDate
                commandInsert.Parameters.Add("@Due", OleDbType.Date).Value = dateDue
                commandInsert.ExecuteNonQuery()
            End Using


        Catch ex As Exception
            MessageBox.Show($"An error occurred while creating a new payroll record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub CheckRecord()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim selectedID As String = MainInterface.lblID.Text
            Dim currentDay As String = DateTime.Now.ToString("dddd")
            Dim currentDate As String = DateTime.Now.ToString("MM/dd/yyyy")
            Dim currentTime As DateTime = DateTime.Now

            Dim recordCheckQuery As String = "
        SELECT COUNT(*)
        FROM Clocking_Log
        WHERE EMPLOYEE_ID = @selectedID AND Work_Date = @currentDate"

            Dim hasRecord As Boolean = False
            Using recordCheckCommand As New OleDbCommand(recordCheckQuery, con)
                recordCheckCommand.Parameters.AddWithValue("@selectedID", selectedID)
                recordCheckCommand.Parameters.AddWithValue("@currentDate", currentDate)
                Dim recordResult = recordCheckCommand.ExecuteScalar()

                If recordResult IsNot Nothing AndAlso Not IsDBNull(recordResult) AndAlso CInt(recordResult) > 0 Then
                    hasRecord = True
                End If
            End Using

            If hasRecord Then
                btntimein.Text = "Recorded"
                lblactivestatus.Text = "Recorded"
                lblactivestatus.ForeColor = Color.DodgerBlue
                btntimein.Enabled = False
                btnbreak.Enabled = False
                lblworkingtime_timer.Enabled = False
                Exit Sub
            End If

            Dim scheduleQuery As String = "
        SELECT COUNT(*) 
        FROM Schedule
        WHERE EMPLOYEE_ID = @selectedID 
        AND dayofweek = @currentDay 
        AND @currentTime >= starttime 
        AND @currentTime <= endtime"

            Dim isScheduled As Boolean = False
            Using command As New OleDbCommand(scheduleQuery, con)
                command.Parameters.AddWithValue("@selectedID", selectedID)
                command.Parameters.AddWithValue("@currentDay", currentDay)
                command.Parameters.AddWithValue("@currentTime", currentTime.ToString("HH:mm:ss"))
                Dim result = command.ExecuteScalar()

                If result IsNot Nothing AndAlso Not IsDBNull(result) AndAlso CInt(result) > 0 Then
                    isScheduled = True
                End If
            End Using

            If isScheduled Then
                btntimein.Enabled = True
                lblactivestatus.Text = "Scheduled"
                lblactivestatus.ForeColor = Color.Green
            Else
                btntimein.Text = "Out of Schedule"
                lblactivestatus.Text = "Out of Schedule"
                lblactivestatus.ForeColor = Color.Red
                btntimein.Enabled = False
            End If

            Dim activeTimeQuery As String = "SELECT TOP 1 ACTIVE_TIME FROM Clocking_Log WHERE EMPLOYEE_ID = @selectedID ORDER BY Clocking_ID DESC"
            Using command As New OleDbCommand(activeTimeQuery, con)
                command.Parameters.AddWithValue("@selectedID", selectedID)
                Dim activeTimeResult = command.ExecuteScalar()

                lblworkingtime.Text = If(activeTimeResult IsNot Nothing AndAlso Not IsDBNull(activeTimeResult), CStr(activeTimeResult), "00:00:00")
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub PopulateDataGrid()
        Try
            DataGridView1.DataSource = Nothing
            Dim dataTable As New DataTable()

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim selectedID As String = MainInterface.lblID.Text
            Dim query As String = "SELECT * FROM Clocking_Log WHERE Employee_ID = @selectedID ORDER BY Work_Date DESC"

            Using adapter As New OleDbDataAdapter(query, con)
                adapter.SelectCommand.Parameters.AddWithValue("@selectedID", selectedID)
                adapter.Fill(dataTable)
            End Using

            DataGridView1.DataSource = dataTable

            If DataGridView1.Columns.Contains("Clocking_ID") Then DataGridView1.Columns.Remove("Clocking_ID")
            If DataGridView1.Columns.Contains("Hours_Worked") Then DataGridView1.Columns.Remove("Hours_Worked")
            If DataGridView1.Columns.Contains("OT_Worked") Then DataGridView1.Columns.Remove("OT_Worked")
            If DataGridView1.Columns.Contains("Employee_ID") Then DataGridView1.Columns.Remove("Employee_ID")


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Private Sub UpdateActivetime()
        Try
            Dim selectedID As String = MainInterface.lblID.Text
            Dim currentDate As String = lbldate.Text
            Dim activeTime As String = lblworkingtime.Text
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim query As String = "UPDATE Clocking_log SET [Active_Time] = ? WHERE [Employee_ID] = ? AND [Work_Date] = ?"
            Using command As New OleDbCommand(query, con)
                command.Parameters.AddWithValue("@ActiveTime", activeTime)
                command.Parameters.AddWithValue("@EmployeeID", selectedID)
                command.Parameters.AddWithValue("@WorkDate", currentDate)
                command.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub

    Sub CalculateAndInsertBonus()
        Try

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If


            Dim holidayRate As Decimal
            Dim holidayName As String
            Dim hoursWorked As Decimal
            Dim otWorked As Decimal
            Dim payRate As Decimal
            Dim otRate As Decimal
            Dim payrollId As Integer

            Dim holidayQuery As String = "SELECT Rate, Holiday_Name FROM Holidays WHERE Holiday_Date = ?"
            Using holidayCmd As New OleDbCommand(holidayQuery, con)
                holidayCmd.Parameters.AddWithValue("@HolidayDate", lbldate.Text)
                Using reader As OleDbDataReader = holidayCmd.ExecuteReader()
                    If reader.Read() Then
                        holidayRate = Convert.ToDecimal(reader("Rate"))
                        holidayName = reader("Holiday_Name").ToString()
                    Else
                        MessageBox.Show("No matching holiday found.")
                        Exit Sub
                    End If
                End Using
            End Using

            Dim clockingQuery As String = "SELECT hours_worked, ot_worked FROM Clocking_Log WHERE Employee_ID = ? AND work_Date = ?"
            Using clockingCmd As New OleDbCommand(clockingQuery, con)
                clockingCmd.Parameters.AddWithValue("@EmployeeID", MainInterface.lblID.Text)
                clockingCmd.Parameters.AddWithValue("@workDate", lbldate.Text)
                Using reader As OleDbDataReader = clockingCmd.ExecuteReader()
                    If reader.Read() Then
                        hoursWorked = Convert.ToDecimal(reader("hours_worked"))
                        otWorked = Convert.ToDecimal(reader("ot_worked"))
                    Else
                        MessageBox.Show("No clocking log found for the selected holiday.")
                        Exit Sub
                    End If
                End Using
            End Using

            Dim payrollQuery As String = "SELECT Payroll_ID, Pay_Rate, OT_Rate FROM Payroll WHERE Employee_ID = ? AND Payroll_Status = 'Unreleased'"
            Using payrollCmd As New OleDbCommand(payrollQuery, con)
                payrollCmd.Parameters.AddWithValue("@EmployeeID", MainInterface.lblID.Text)
                Using reader As OleDbDataReader = payrollCmd.ExecuteReader()
                    If reader.Read() Then
                        payrollId = Convert.ToInt32(reader("Payroll_ID"))
                        payRate = Convert.ToDecimal(reader("Pay_Rate"))
                        otRate = Convert.ToDecimal(reader("OT_Rate"))
                    Else
                        MessageBox.Show("No unreleased payroll found.")
                        Exit Sub
                    End If
                End Using
            End Using

            Dim variable1 As Decimal = (payRate * (holidayRate - 1)) * hoursWorked
            Dim variable2 As Decimal = (otRate * (1.3D - 1)) * otWorked
            Dim totalBonus As Decimal = variable1 + variable2

            Dim insertQuery As String = "INSERT INTO Bonus (Payroll_ID, Employee_ID, Amount, Reason, Selected) VALUES (?, ?, ?, ?, True)"
            Using insertCmd As New OleDbCommand(insertQuery, con)
                insertCmd.Parameters.AddWithValue("@PayrollID", payrollId)
                insertCmd.Parameters.AddWithValue("@EmployeeID", MainInterface.lblID.Text)
                insertCmd.Parameters.AddWithValue("@Amount", totalBonus)
                insertCmd.Parameters.AddWithValue("@Reason", holidayName)
                insertCmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub HighlightHolidays()
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

End Class