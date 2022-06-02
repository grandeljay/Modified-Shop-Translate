Public Class ClassLoader
    Private ReadOnly Loader As New FormLoader

    Public Sub Open()
        Me.Loader.Show()
    End Sub

    Public Sub Close()
        Me.Loader.Close()
    End Sub

    Public Sub SetText(Text As String)
        Me.Loader.LabelText.Text = Text
    End Sub

    Public Sub SetMaximum(Maximum As Integer)
        Me.Loader.ProgressBarLoading.Value = Me.Loader.ProgressBarLoading.Minimum
        Me.Loader.ProgressBarLoading.Style = ProgressBarStyle.Blocks

        Me.Loader.ProgressBarLoading.Maximum = Maximum
    End Sub

    Public Sub PerformStep()
        Me.Loader.ProgressBarLoading.PerformStep()
    End Sub
End Class
