<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Admin13thMonth
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        btngive = New Guna.UI2.WinForms.Guna2Button()
        Guna2HtmlLabel1 = New Guna.UI2.WinForms.Guna2HtmlLabel()
        Guna2CustomGradientPanel11 = New Guna.UI2.WinForms.Guna2CustomGradientPanel()
        dgv13thmonth = New Guna.UI2.WinForms.Guna2DataGridView()
        Guna2GradientPanel1.SuspendLayout()
        Guna2CustomGradientPanel11.SuspendLayout()
        CType(dgv13thmonth, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Guna2GradientPanel1.Controls.Add(Guna2HtmlLabel1)
        Guna2GradientPanel1.Controls.Add(Guna2CustomGradientPanel11)
        Guna2GradientPanel1.Controls.Add(btngive)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges5
        Guna2GradientPanel1.FillColor = Color.DodgerBlue
        Guna2GradientPanel1.GradientMode = Drawing2D.LinearGradientMode.Vertical
        Guna2GradientPanel1.Location = New Point(0, 0)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        Guna2GradientPanel1.Size = New Size(1450, 768)
        Guna2GradientPanel1.TabIndex = 0
        ' 
        ' btngive
        ' 
        btngive.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btngive.CustomizableEdges = CustomizableEdges3
        btngive.DisabledState.BorderColor = Color.DarkGray
        btngive.DisabledState.CustomBorderColor = Color.DarkGray
        btngive.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btngive.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btngive.Font = New Font("Segoe UI", 9F)
        btngive.ForeColor = Color.White
        btngive.Location = New Point(1274, 711)
        btngive.Name = "btngive"
        btngive.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btngive.Size = New Size(145, 45)
        btngive.TabIndex = 56
        btngive.Text = "Give All"
        ' 
        ' Guna2HtmlLabel1
        ' 
        Guna2HtmlLabel1.BackColor = Color.Transparent
        Guna2HtmlLabel1.Font = New Font("Nirmala UI", 24F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Guna2HtmlLabel1.ForeColor = Color.White
        Guna2HtmlLabel1.Location = New Point(34, 12)
        Guna2HtmlLabel1.Name = "Guna2HtmlLabel1"
        Guna2HtmlLabel1.Size = New Size(282, 47)
        Guna2HtmlLabel1.TabIndex = 4
        Guna2HtmlLabel1.Text = "13th Month Bonus"
        ' 
        ' Guna2CustomGradientPanel11
        ' 
        Guna2CustomGradientPanel11.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Guna2CustomGradientPanel11.BackColor = Color.Transparent
        Guna2CustomGradientPanel11.BorderColor = Color.Transparent
        Guna2CustomGradientPanel11.BorderRadius = 20
        Guna2CustomGradientPanel11.Controls.Add(dgv13thmonth)
        Guna2CustomGradientPanel11.CustomizableEdges = CustomizableEdges1
        Guna2CustomGradientPanel11.Location = New Point(31, 69)
        Guna2CustomGradientPanel11.Name = "Guna2CustomGradientPanel11"
        Guna2CustomGradientPanel11.ShadowDecoration.BorderRadius = 20
        Guna2CustomGradientPanel11.ShadowDecoration.Color = Color.DimGray
        Guna2CustomGradientPanel11.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        Guna2CustomGradientPanel11.ShadowDecoration.Depth = 6
        Guna2CustomGradientPanel11.ShadowDecoration.Enabled = True
        Guna2CustomGradientPanel11.ShadowDecoration.Shadow = New Padding(8)
        Guna2CustomGradientPanel11.Size = New Size(1388, 623)
        Guna2CustomGradientPanel11.TabIndex = 23
        ' 
        ' dgv13thmonth
        ' 
        dgv13thmonth.AllowUserToAddRows = False
        dgv13thmonth.AllowUserToDeleteRows = False
        dgv13thmonth.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = Color.White
        DataGridViewCellStyle1.Font = New Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle1.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = Color.White
        DataGridViewCellStyle1.SelectionForeColor = Color.Black
        dgv13thmonth.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        dgv13thmonth.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.White
        DataGridViewCellStyle2.Font = New Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle2.ForeColor = Color.Black
        DataGridViewCellStyle2.SelectionBackColor = Color.White
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgv13thmonth.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        dgv13thmonth.ColumnHeadersHeight = 40
        dgv13thmonth.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = Color.White
        DataGridViewCellStyle3.Font = New Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle3.ForeColor = Color.Black
        DataGridViewCellStyle3.SelectionBackColor = Color.White
        DataGridViewCellStyle3.SelectionForeColor = Color.Black
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.False
        dgv13thmonth.DefaultCellStyle = DataGridViewCellStyle3
        dgv13thmonth.GridColor = Color.FromArgb(CByte(187), CByte(222), CByte(251))
        dgv13thmonth.Location = New Point(3, 24)
        dgv13thmonth.Name = "dgv13thmonth"
        dgv13thmonth.ReadOnly = True
        dgv13thmonth.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = Color.White
        DataGridViewCellStyle4.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle4.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = Color.White
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.True
        dgv13thmonth.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        dgv13thmonth.RowHeadersVisible = False
        dgv13thmonth.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
        dgv13thmonth.RowTemplate.Height = 35
        dgv13thmonth.Size = New Size(1382, 580)
        dgv13thmonth.TabIndex = 10
        dgv13thmonth.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Blue
        dgv13thmonth.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White
        dgv13thmonth.ThemeStyle.AlternatingRowsStyle.Font = New Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        dgv13thmonth.ThemeStyle.AlternatingRowsStyle.ForeColor = SystemColors.ControlText
        dgv13thmonth.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.White
        dgv13thmonth.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Black
        dgv13thmonth.ThemeStyle.BackColor = Color.White
        dgv13thmonth.ThemeStyle.GridColor = Color.FromArgb(CByte(187), CByte(222), CByte(251))
        dgv13thmonth.ThemeStyle.HeaderStyle.BackColor = Color.White
        dgv13thmonth.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None
        dgv13thmonth.ThemeStyle.HeaderStyle.Font = New Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        dgv13thmonth.ThemeStyle.HeaderStyle.ForeColor = Color.Black
        dgv13thmonth.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        dgv13thmonth.ThemeStyle.HeaderStyle.Height = 40
        dgv13thmonth.ThemeStyle.ReadOnly = True
        dgv13thmonth.ThemeStyle.RowsStyle.BackColor = Color.White
        dgv13thmonth.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv13thmonth.ThemeStyle.RowsStyle.Font = New Font("Century Gothic", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        dgv13thmonth.ThemeStyle.RowsStyle.ForeColor = Color.Black
        dgv13thmonth.ThemeStyle.RowsStyle.Height = 35
        dgv13thmonth.ThemeStyle.RowsStyle.SelectionBackColor = Color.White
        dgv13thmonth.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black
        ' 
        ' Admin13thMonth
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1450, 768)
        Controls.Add(Guna2GradientPanel1)
        FormBorderStyle = FormBorderStyle.None
        Name = "Admin13thMonth"
        Text = "Admin13thMonth"
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        Guna2CustomGradientPanel11.ResumeLayout(False)
        CType(dgv13thmonth, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Guna2CustomGradientPanel11 As Guna.UI2.WinForms.Guna2CustomGradientPanel
    Friend WithEvents Guna2HtmlLabel1 As Guna.UI2.WinForms.Guna2HtmlLabel
    Friend WithEvents dgv13thmonth As Guna.UI2.WinForms.Guna2DataGridView
    Friend WithEvents btngive As Guna.UI2.WinForms.Guna2Button
End Class
