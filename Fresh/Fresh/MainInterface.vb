Imports System.Data.OleDb
Imports System.Net.Mail
Imports System.Runtime.InteropServices
Imports System.Windows.Automation

Public Class MainInterface
    Dim Dashboard As New ClientDashboard()
    Dim Payroll As New ClientPayroll()
    Dim TaxAdvise As New ClientTaxAdvice
    Dim Help As New ClientHelp()
    Dim Setting As New ClientSetting()


    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr

    End Function
    Private Sub ClientDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dashboard.TopLevel = False
        Dashboard.Visible = True
        LoadChildForm(ClientDashboard)
    End Sub
    Private Sub LoadChildForm(childForm As Form)
        pnlDisplay.Controls.Clear()

        childForm.TopLevel = False
        childForm.FormBorderStyle = FormBorderStyle.None
        childForm.Dock = DockStyle.Fill

        pnlDisplay.Controls.Add(childForm)
        pnlDisplay.Tag = childForm
        childForm.BringToFront()
        childForm.Show()

        For Each ctrl As Control In childForm.Controls
            ctrl.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Next
    End Sub
    Private Sub btnpayroll_Click(sender As Object, e As EventArgs) Handles btnpayroll.Click
        Payroll.TopLevel = False
        Payroll.Visible = True
        LoadChildForm(ClientPayroll)


        Payroll.RetrievePayrollData
    End Sub

    Private Sub btndashboard_Click(sender As Object, e As EventArgs) Handles btndashboard.Click
        Dashboard.TopLevel = False
        Dashboard.Visible = True
        LoadChildForm(ClientDashboard)

    End Sub


    Private Sub btnhelp_Click(sender As Object, e As EventArgs) Handles btnhelp.Click
        Help.TopLevel = False
        Help.Visible = True
        LoadChildForm(ClientHelp)

    End Sub

    Private Sub btntaxadvice_Click(sender As Object, e As EventArgs) Handles btntaxadvice.Click
        TaxAdvise.TopLevel = False
        TaxAdvise.Visible = True
        LoadChildForm(TaxAdvise)

    End Sub

    Private Sub btnsetting_Click(sender As Object, e As EventArgs) Handles btnsetting.Click
        Setting.TopLevel = False
        Setting.Visible = True
        LoadChildForm(ClientSetting)

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

    Private Sub btnexit_Click(sender As Object, e As EventArgs) Handles btnexit.Click
        Application.Exit()
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
    Private Sub btnminimize_Click(sender As Object, e As EventArgs) Handles btnminimize.Click
        WindowState = FormWindowState.Minimized
    End Sub

End Class