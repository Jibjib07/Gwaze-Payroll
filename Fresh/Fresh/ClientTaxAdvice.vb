Public Class ClientTaxAdvice
    Private Sub Guna2Panel4_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel4.Paint

    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        PictureBox1.BackgroundImage = My.Resources.Monthly_Tax_Table
        taxlbl.Text = "Monthly Tax Table"
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        PictureBox1.BackgroundImage = My.Resources.Anual_Tax_Table
        taxlbl.Text = "Anual Tax Table"
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        PictureBox1.BackgroundImage = My.Resources.Contribution_Schedule
        taxlbl.Text = "Contribution"
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        PictureBox1.BackgroundImage = My.Resources.titii
        taxlbl.Text = "Tax Exemptions"
    End Sub

End Class