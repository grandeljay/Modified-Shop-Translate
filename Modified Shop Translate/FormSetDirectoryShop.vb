Imports System.IO

Public Class FormSetDirectoryShop
    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        If Directory.Exists(TextBoxDirectory.Text) Then
            FormMain.Settings.DirectoryShop = TextBoxDirectory.Text

            Close()
        Else
            MessageBox.Show("Der angegebene Pfad scheint kein Verzeichnis zu sein.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Close()
    End Sub
End Class