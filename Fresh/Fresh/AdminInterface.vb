Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class AdminInterface

    Dim Admin As New AdminDashboard()
    Public AdminPayrollInstance As New AdminPayroll()
    Dim Register As New AdminRegister()
    Dim History As New AdminHistory()
    Dim Holiday As New AdminHolidays()
    Dim thirteen As New Admin13thMonth()
    Dim employee As New AdminEmployee()
    Dim sched As New AdminSchedule()

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btndashboard.PerformClick()
    End Sub

    Private Sub LoadChildForm(childForm As Form)
        pnlaDisplay.Controls.Clear()

        childForm.TopLevel = False
        childForm.FormBorderStyle = FormBorderStyle.None
        childForm.Dock = DockStyle.Fill

        pnlaDisplay.Controls.Add(childForm)
        pnlaDisplay.Tag = childForm
        childForm.BringToFront()
        childForm.Show()

        For Each ctrl As Control In childForm.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Next
    End Sub

    Private Sub btndashboard_Click(sender As Object, e As EventArgs) Handles btndashboard.Click
        LoadChildForm(Admin)

        Admin.FillEmployeeCounts()
        Admin.UpdateProgressAndTotalTime()
        Admin.HighlightHolidays()
        Admin.loadclockinlog()
    End Sub

    Private Sub btnpayroll_Click(sender As Object, e As EventArgs) Handles btnpayroll.Click
        If AdminPayrollInstance Is Nothing Then
            AdminPayrollInstance = New AdminPayroll
        End If
        LoadChildForm(AdminPayrollInstance)

        AdminPayrollInstance.LoadEmployeeData
        AdminPayrollInstance.UpdateUnpaidLabel
    End Sub

    Private Sub btnregister_Click(sender As Object, e As EventArgs) Handles btnregister.Click
        LoadChildForm(Register)

    End Sub

    Private Sub btnihistory_Click(sender As Object, e As EventArgs) Handles btnihistory.Click
        LoadChildForm(History)

        History.LoadPayrollHistory()
    End Sub

    Private Sub btnHolidays_Click(sender As Object, e As EventArgs) Handles btnHolidays.Click
        LoadChildForm(Holiday)

        Holiday.LoadData
    End Sub

    Private Sub btn13thmonth_Click(sender As Object, e As EventArgs) Handles btn13thmonth.Click
        LoadChildForm(thirteen)

        thirteen.Load13thMonthData()
    End Sub
    Private Sub btnEmployee_Click(sender As Object, e As EventArgs) Handles btnEmployee.Click
        LoadChildForm(employee)

        employee.LoadEmployeeData()
    End Sub
    Private Sub btnSched_Click(sender As Object, e As EventArgs) Handles btnSched.Click
        LoadChildForm(sched)

    End Sub
    Private Sub btnsignout_Click(sender As Object, e As EventArgs) Handles btnsignout.Click
        For Each frm As Form In My.Application.OpenForms.OfType(Of Form).ToArray()
            If Not frm Is Form1 Then
                frm.Close()
            End If
        Next

        Form1.lblerroruser.Hide()
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub AdminPayroll_EnabledChanged(sender As Object, e As EventArgs) Handles MyBase.EnabledChanged
        If Me.Enabled = True Then
            AdminPayrollInstance.GetBonusData()
            AdminPayrollInstance.GetDeductionData()
        End If
    End Sub

    Private Sub AdminInterface_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not IsNothing(AdminPayrollInstance) Then
            Try
                AdminPayrollInstance.UpdatePayroll()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub AdminInterface_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim blueSectionWidth As Integer = 150
        pnlaDisplay.Left = blueSectionWidth
        pnlaDisplay.Width = Me.ClientSize.Width - blueSectionWidth
        pnlaDisplay.Height = Me.ClientSize.Height

        If pnlaDisplay.Controls.Count > 0 Then
            Dim childForm As Form = CType(pnlaDisplay.Controls(0), Form)
            childForm.Dock = DockStyle.Fill
        End If
    End Sub

    Private Sub btnMaximize_Click(sender As Object, e As EventArgs) Handles btnMaximize.Click
        If btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize Then
            WindowState = FormWindowState.Normal
            Dim workingArea As Rectangle = Screen.PrimaryScreen.WorkingArea
            Me.Bounds = workingArea
            btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowRestore
        Else
            WindowState = FormWindowState.Normal
            Me.Size = New Size(1604, 805)

            Me.Location = New Point(
            (Screen.PrimaryScreen.WorkingArea.Width - Me.Width) \ 2,
            (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) \ 2)

            btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize
        End If
    End Sub

    Private Sub btnexit_Click(sender As Object, e As EventArgs) Handles btnexit.Click
        Application.Exit()
    End Sub

    Private Sub btnminimize_Click(sender As Object, e As EventArgs) Handles btnminimize.Click
        WindowState = FormWindowState.Minimized
    End Sub


End Class
