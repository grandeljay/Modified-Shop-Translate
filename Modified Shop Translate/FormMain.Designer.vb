﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.LabelLanguageSource = New System.Windows.Forms.Label()
        Me.LabelLanguageTarget = New System.Windows.Forms.Label()
        Me.ComboBoxLanguageSource = New System.Windows.Forms.ComboBox()
        Me.ComboBoxLanguageTarget = New System.Windows.Forms.ComboBox()
        Me.ButtonCreatePO = New System.Windows.Forms.Button()
        Me.ButtonUpdateTranslations = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'LabelLanguageSource
        '
        Me.LabelLanguageSource.AutoSize = True
        Me.LabelLanguageSource.Location = New System.Drawing.Point(12, 9)
        Me.LabelLanguageSource.Name = "LabelLanguageSource"
        Me.LabelLanguageSource.Size = New System.Drawing.Size(112, 15)
        Me.LabelLanguageSource.TabIndex = 0
        Me.LabelLanguageSource.Text = "Source language"
        '
        'LabelLanguageTarget
        '
        Me.LabelLanguageTarget.AutoSize = True
        Me.LabelLanguageTarget.Location = New System.Drawing.Point(168, 9)
        Me.LabelLanguageTarget.Name = "LabelLanguageTarget"
        Me.LabelLanguageTarget.Size = New System.Drawing.Size(107, 15)
        Me.LabelLanguageTarget.TabIndex = 1
        Me.LabelLanguageTarget.Text = "Target language"
        '
        'ComboBoxLanguageSource
        '
        Me.ComboBoxLanguageSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxLanguageSource.FormattingEnabled = True
        Me.ComboBoxLanguageSource.Location = New System.Drawing.Point(12, 27)
        Me.ComboBoxLanguageSource.Name = "ComboBoxLanguageSource"
        Me.ComboBoxLanguageSource.Size = New System.Drawing.Size(150, 23)
        Me.ComboBoxLanguageSource.TabIndex = 2
        '
        'ComboBoxLanguageTarget
        '
        Me.ComboBoxLanguageTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxLanguageTarget.FormattingEnabled = True
        Me.ComboBoxLanguageTarget.Location = New System.Drawing.Point(168, 27)
        Me.ComboBoxLanguageTarget.Name = "ComboBoxLanguageTarget"
        Me.ComboBoxLanguageTarget.Size = New System.Drawing.Size(150, 23)
        Me.ComboBoxLanguageTarget.TabIndex = 3
        '
        'ButtonCreatePO
        '
        Me.ButtonCreatePO.AutoSize = True
        Me.ButtonCreatePO.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonCreatePO.Location = New System.Drawing.Point(12, 56)
        Me.ButtonCreatePO.MinimumSize = New System.Drawing.Size(0, 24)
        Me.ButtonCreatePO.Name = "ButtonCreatePO"
        Me.ButtonCreatePO.Size = New System.Drawing.Size(79, 25)
        Me.ButtonCreatePO.TabIndex = 4
        Me.ButtonCreatePO.Text = "Create PO"
        Me.ButtonCreatePO.UseVisualStyleBackColor = True
        '
        'ButtonUpdateTranslations
        '
        Me.ButtonUpdateTranslations.AutoSize = True
        Me.ButtonUpdateTranslations.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonUpdateTranslations.Location = New System.Drawing.Point(97, 56)
        Me.ButtonUpdateTranslations.MinimumSize = New System.Drawing.Size(0, 24)
        Me.ButtonUpdateTranslations.Name = "ButtonUpdateTranslations"
        Me.ButtonUpdateTranslations.Size = New System.Drawing.Size(130, 25)
        Me.ButtonUpdateTranslations.TabIndex = 6
        Me.ButtonUpdateTranslations.Text = "Update Traslations"
        Me.ButtonUpdateTranslations.UseVisualStyleBackColor = True
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(333, 89)
        Me.Controls.Add(Me.ButtonUpdateTranslations)
        Me.Controls.Add(Me.ButtonCreatePO)
        Me.Controls.Add(Me.ComboBoxLanguageTarget)
        Me.Controls.Add(Me.ComboBoxLanguageSource)
        Me.Controls.Add(Me.LabelLanguageTarget)
        Me.Controls.Add(Me.LabelLanguageSource)
        Me.Font = New System.Drawing.Font("Raleway", 9.749999!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Modified Shop Translate"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LabelLanguageSource As Label
    Friend WithEvents LabelLanguageTarget As Label
    Friend WithEvents ComboBoxLanguageSource As ComboBox
    Friend WithEvents ComboBoxLanguageTarget As ComboBox
    Friend WithEvents ButtonCreatePO As Button
    Friend WithEvents ButtonUpdateTranslations As Button
End Class