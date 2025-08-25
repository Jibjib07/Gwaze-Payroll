<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AdminBonusData
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
        components = New ComponentModel.Container()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        dgdisplay = New Guna.UI2.WinForms.Guna2DataGridView()
        btnAdd = New Guna.UI2.WinForms.Guna2Button()
        btnDelete = New Guna.UI2.WinForms.Guna2Button()
        txtAmount = New TextBox()
        txtReason = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Shapes1 = New Guna.UI2.WinForms.Guna2Shapes()
        Guna2Shapes2 = New Guna.UI2.WinForms.Guna2Shapes()
        btnExit = New FontAwesome.Sharp.IconButton()
        CType(dgdisplay, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgdisplay
        ' 
        DataGridViewCellStyle1.BackColor = Color.White
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle1.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        DataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        dgdisplay.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.White
        DataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgdisplay.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        dgdisplay.ColumnHeadersHeight = 30
        dgdisplay.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = Color.White
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        DataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.False
        dgdisplay.DefaultCellStyle = DataGridViewCellStyle3
        dgdisplay.GridColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        dgdisplay.Location = New Point(12, 47)
        dgdisplay.Name = "dgdisplay"
        dgdisplay.ReadOnly = True
        dgdisplay.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = Color.White
        DataGridViewCellStyle4.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle4.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = Color.White
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.True
        dgdisplay.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        dgdisplay.RowHeadersVisible = False
        dgdisplay.Size = New Size(777, 295)
        dgdisplay.TabIndex = 0
        dgdisplay.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White
        dgdisplay.ThemeStyle.AlternatingRowsStyle.Font = Nothing
        dgdisplay.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty
        dgdisplay.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty
        dgdisplay.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty
        dgdisplay.ThemeStyle.BackColor = Color.White
        dgdisplay.ThemeStyle.GridColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        dgdisplay.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(CByte(100), CByte(88), CByte(255))
        dgdisplay.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None
        dgdisplay.ThemeStyle.HeaderStyle.Font = New Font("Segoe UI", 9F)
        dgdisplay.ThemeStyle.HeaderStyle.ForeColor = Color.White
        dgdisplay.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        dgdisplay.ThemeStyle.HeaderStyle.Height = 30
        dgdisplay.ThemeStyle.ReadOnly = True
        dgdisplay.ThemeStyle.RowsStyle.BackColor = Color.White
        dgdisplay.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgdisplay.ThemeStyle.RowsStyle.Font = New Font("Segoe UI", 9F)
        dgdisplay.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        dgdisplay.ThemeStyle.RowsStyle.Height = 25
        dgdisplay.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(CByte(231), CByte(229), CByte(255))
        dgdisplay.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(CByte(71), CByte(69), CByte(94))
        ' 
        ' btnAdd
        ' 
        btnAdd.CustomizableEdges = CustomizableEdges1
        btnAdd.DisabledState.BorderColor = Color.DarkGray
        btnAdd.DisabledState.CustomBorderColor = Color.DarkGray
        btnAdd.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnAdd.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnAdd.FillColor = Color.SteelBlue
        btnAdd.Font = New Font("Segoe UI", 9F)
        btnAdd.ForeColor = Color.White
        btnAdd.Location = New Point(359, 364)
        btnAdd.Name = "btnAdd"
        btnAdd.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        btnAdd.Size = New Size(180, 45)
        btnAdd.TabIndex = 1
        btnAdd.Text = "ADD"
        ' 
        ' btnDelete
        ' 
        btnDelete.CustomizableEdges = CustomizableEdges3
        btnDelete.DisabledState.BorderColor = Color.DarkGray
        btnDelete.DisabledState.CustomBorderColor = Color.DarkGray
        btnDelete.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnDelete.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnDelete.FillColor = Color.SteelBlue
        btnDelete.Font = New Font("Segoe UI", 9F)
        btnDelete.ForeColor = Color.White
        btnDelete.Location = New Point(564, 364)
        btnDelete.Name = "btnDelete"
        btnDelete.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btnDelete.Size = New Size(180, 45)
        btnDelete.TabIndex = 2
        btnDelete.Text = "DELETE"
        ' 
        ' txtAmount
        ' 
        txtAmount.Location = New Point(137, 364)
        txtAmount.Name = "txtAmount"
        txtAmount.Size = New Size(187, 23)
        txtAmount.TabIndex = 4
        ' 
        ' txtReason
        ' 
        txtReason.Location = New Point(137, 393)
        txtReason.Name = "txtReason"
        txtReason.Size = New Size(187, 23)
        txtReason.TabIndex = 5
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(36, 367)
        Label1.Name = "Label1"
        Label1.Size = New Size(51, 15)
        Label1.TabIndex = 6
        Label1.Text = "Amount"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(36, 396)
        Label2.Name = "Label2"
        Label2.Size = New Size(45, 15)
        Label2.TabIndex = 7
        Label2.Text = "Reason"
        Label2.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.TargetControl = Me
        ' 
        ' Guna2Shapes1
        ' 
        Guna2Shapes1.BackColor = Color.Transparent
        Guna2Shapes1.BorderColor = Color.Empty
        Guna2Shapes1.FillColor = Color.PowderBlue
        Guna2Shapes1.LineStartCap = Drawing2D.LineCap.Round
        Guna2Shapes1.Location = New Point(344, 164)
        Guna2Shapes1.Name = "Guna2Shapes1"
        Guna2Shapes1.PolygonSkip = 1
        Guna2Shapes1.Rotate = 0F
        Guna2Shapes1.RoundedEdges = CustomizableEdges5
        Guna2Shapes1.Shape = Guna.UI2.WinForms.Enums.ShapeType.Ellipse
        Guna2Shapes1.Size = New Size(534, 477)
        Guna2Shapes1.TabIndex = 8
        Guna2Shapes1.Text = "Guna2Shapes1"
        Guna2Shapes1.UseTransparentBackground = True
        Guna2Shapes1.Zoom = 80
        ' 
        ' Guna2Shapes2
        ' 
        Guna2Shapes2.BackColor = Color.Transparent
        Guna2Shapes2.BorderColor = Color.Empty
        Guna2Shapes2.FillColor = Color.SteelBlue
        Guna2Shapes2.LineStartCap = Drawing2D.LineCap.Round
        Guna2Shapes2.Location = New Point(-172, -212)
        Guna2Shapes2.Name = "Guna2Shapes2"
        Guna2Shapes2.PolygonSkip = 1
        Guna2Shapes2.Rotate = 0F
        Guna2Shapes2.RoundedEdges = CustomizableEdges6
        Guna2Shapes2.Shape = Guna.UI2.WinForms.Enums.ShapeType.Ellipse
        Guna2Shapes2.Size = New Size(534, 477)
        Guna2Shapes2.TabIndex = 9
        Guna2Shapes2.Text = "Guna2Shapes2"
        Guna2Shapes2.UseTransparentBackground = True
        Guna2Shapes2.Zoom = 80
        ' 
        ' btnExit
        ' 
        btnExit.BackColor = Color.DodgerBlue
        btnExit.IconChar = FontAwesome.Sharp.IconChar.Close
        btnExit.IconColor = Color.Black
        btnExit.IconFont = FontAwesome.Sharp.IconFont.Auto
        btnExit.IconSize = 30
        btnExit.Location = New Point(733, 12)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(56, 29)
        btnExit.TabIndex = 10
        btnExit.UseVisualStyleBackColor = True
        ' 
        ' AdminBonusData
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.WhiteSmoke
        ClientSize = New Size(801, 453)
        Controls.Add(btnExit)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(txtReason)
        Controls.Add(txtAmount)
        Controls.Add(btnDelete)
        Controls.Add(btnAdd)
        Controls.Add(dgdisplay)
        Controls.Add(Guna2Shapes1)
        Controls.Add(Guna2Shapes2)
        FormBorderStyle = FormBorderStyle.None
        Name = "AdminBonusData"
        StartPosition = FormStartPosition.CenterScreen
        Text = "AdminBonusData"
        CType(dgdisplay, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents dgdisplay As Guna.UI2.WinForms.Guna2DataGridView
    Friend WithEvents btnAdd As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnDelete As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents txtAmount As TextBox
    Friend WithEvents txtReason As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Shapes1 As Guna.UI2.WinForms.Guna2Shapes
    Friend WithEvents Guna2Shapes2 As Guna.UI2.WinForms.Guna2Shapes
    Friend WithEvents btnExit As FontAwesome.Sharp.IconButton
End Class
