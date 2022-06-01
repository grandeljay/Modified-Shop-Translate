Imports System.IO
Imports System.ComponentModel

Public Class ClassSettings
    Public BackgroundWorkerGetFiles, BackgroundWorkerSetLanguage As New BackgroundWorker

    Private Sub BackgroundWorkerGetFiles_DoWork(sender As Object, e As DoWorkEventArgs)

    End Sub

    Private Sub BackgroundWorkerSetLanguage_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        ' Beep()
    End Sub

    Private Sub BackgroundWorkerSetLanguage_DoWork(sender As Object, e As DoWorkEventArgs)
        ' Set language confs
        Dim LanguagePaths As String() = Directory.GetDirectories(Me.DirectoryLang, "*", SearchOption.TopDirectoryOnly)
        Dim LanguageDirectories As New List(Of String)

        For Each LanguagePath As String In LanguagePaths
            Dim LanguageName As String = Path.GetFileName(LanguagePath)
            Dim LanguageConf As String = LanguagePath & "\lang_" & LanguageName & ".conf"

            If File.Exists(LanguageConf) Then
                LanguageDirectories.Add(LanguagePath)
            End If
        Next

        ' Add languages
        Me.Languages = New List(Of ClassLanguage)

        For I = 0 To LanguageDirectories.Count - 1
            Dim ProgressPercent As Integer = Convert.ToInt32(I / (LanguageDirectories.Count - 1) * 100)
            BackgroundWorkerSetLanguage.ReportProgress(ProgressPercent)

            Dim LanguageDirectory As String = LanguageDirectories.Item(I)
            Me.Languages.Add(New ClassLanguage(LanguageDirectory))
        Next
    End Sub

    Public Property DirectoryShop As String
    Public Property DirectoryLang As String

    Public Property Languages As List(Of ClassLanguage)
    Public Property LanguageSource As ClassLanguage
    Public Property LanguageTarget As ClassLanguage

    Sub New(ShopRoot As String)
        ' Root
        Me.DirectoryShop = ShopRoot

        ' Lang
        Dim DirectoryLang As String = Me.DirectoryShop & "\lang"

        If Directory.Exists(DirectoryLang) Then
            Me.DirectoryLang = DirectoryLang
        End If

        ' Filepaths Define
        BackgroundWorkerGetFiles.WorkerReportsProgress = True
        BackgroundWorkerSetLanguage.WorkerReportsProgress = True

        AddHandler BackgroundWorkerGetFiles.DoWork, AddressOf Me.BackgroundWorkerGetFiles_DoWork
        AddHandler BackgroundWorkerSetLanguage.DoWork, AddressOf Me.BackgroundWorkerSetLanguage_DoWork
        AddHandler BackgroundWorkerSetLanguage.ProgressChanged, AddressOf Me.BackgroundWorkerSetLanguage_ProgressChanged
    End Sub
End Class
