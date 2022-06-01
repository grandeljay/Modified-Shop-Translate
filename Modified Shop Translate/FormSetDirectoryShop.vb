Imports System.ComponentModel
Imports System.IO

Public Class FormSetDirectoryShop
    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        If Directory.Exists(Me.TextBoxDirectory.Text) Then
            FormMain.Settings = New ClassSettings(Me.TextBoxDirectory.Text)

            Me.Close()
        Else
            MessageBox.Show("Der angegebene Pfad scheint kein Verzeichnis zu sein.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.Close()
    End Sub

End Class