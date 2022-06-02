Imports System.ComponentModel

Public Class ClassLoader
    Private ReadOnly Loader As New FormLoader

    Private BackgroundWorkerLoader As BackgroundWorker

    Public Sub Show()
        Me.Loader.Show()
    End Sub

    Public Sub Hide()
        Me.Loader.Hide()
    End Sub

    Private Function IsActive() As Boolean
        Return Loader.Visible
    End Function

    Public Sub SetBackgroundWorker(ByRef BackgroundWorkerLoader As BackgroundWorker)
        Me.BackgroundWorkerLoader = BackgroundWorkerLoader
    End Sub

    Private Sub _SetText(Text As String)
        Me.Loader.LabelText.Text = Text
    End Sub
    Public Sub SetText(Text As String)
        If Not Me.IsActive Then Exit Sub

        If Me.BackgroundWorkerLoader Is Nothing Then
            Me._SetText(Text)
        Else
            Me.BackgroundWorkerLoader.ReportProgress(-1, Text)
        End If
    End Sub

    Private Sub _SetMaximum(Maximum As Integer)
        Me.Loader.ProgressBarLoading.Value = Me.Loader.ProgressBarLoading.Minimum
        Me.Loader.ProgressBarLoading.Style = ProgressBarStyle.Blocks
        Me.Loader.ProgressBarLoading.Maximum = Maximum
    End Sub

    Public Sub SetMaximum(Maximum As Integer)
        If Not Me.IsActive Then Exit Sub

        If Me.BackgroundWorkerLoader Is Nothing Then
            Me._SetMaximum(Maximum)
        Else
            Me.BackgroundWorkerLoader.ReportProgress(Maximum, "maximum")
        End If
    End Sub

    Private Sub _PerformStep()
        Me.Loader.ProgressBarLoading.PerformStep()
    End Sub

    Public Sub PerformStep()
        If Not Me.IsActive Then Exit Sub

        If Me.BackgroundWorkerLoader Is Nothing Then
            Me._PerformStep()
        Else
            Me.BackgroundWorkerLoader.ReportProgress(1, "step")
        End If
    End Sub

    Public Sub BackgroundWorkerLoader_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        Select Case e.ProgressPercentage
            Case -1
                Me._SetText(e.UserState.ToString)
        End Select

        Select Case e.UserState.ToString
            Case "maximum"
                Me._SetMaximum(e.ProgressPercentage)

            Case "step"
                Me._PerformStep()
        End Select
    End Sub
End Class
