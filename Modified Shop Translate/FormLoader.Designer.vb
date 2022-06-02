<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLoader
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
        Me.ProgressBarLoading = New System.Windows.Forms.ProgressBar()
        Me.LabelText = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ProgressBarLoading
        '
        Me.ProgressBarLoading.Location = New System.Drawing.Point(13, 43)
        Me.ProgressBarLoading.Name = "ProgressBarLoading"
        Me.ProgressBarLoading.Size = New System.Drawing.Size(400, 23)
        Me.ProgressBarLoading.Step = 1
        Me.ProgressBarLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.ProgressBarLoading.TabIndex = 0
        '
        'LabelText
        '
        Me.LabelText.Location = New System.Drawing.Point(13, 10)
        Me.LabelText.Name = "LabelText"
        Me.LabelText.Size = New System.Drawing.Size(400, 30)
        Me.LabelText.TabIndex = 1
        Me.LabelText.Text = "Initializing..."
        '
        'FormLoader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(429, 79)
        Me.Controls.Add(Me.LabelText)
        Me.Controls.Add(Me.ProgressBarLoading)
        Me.Font = New System.Drawing.Font("Raleway", 9.749999!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Name = "FormLoader"
        Me.Padding = New System.Windows.Forms.Padding(10)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Loading"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ProgressBarLoading As ProgressBar
    Friend WithEvents LabelText As Label
End Class
