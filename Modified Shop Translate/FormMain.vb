﻿Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions

Public Class FormMain
    Private ReadOnly LoaderUpdateTranslations, LoaderUpdateTranslationsAll As New ClassLoader

    Public Shared Settings As ClassSettings

#Region "FormEvents"
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set Shop Path
        Do While Settings Is Nothing
            FormSetDirectoryShop.ShowDialog()
        Loop

        Settings.BackgroundWorkerGetFiles.RunWorkerAsync()
        Settings.BackgroundWorkerSetLanguage.RunWorkerAsync()
    End Sub

    Private Sub FormMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.ComboBoxLanguageSource.UseWaitCursor = True
        Me.ComboBoxLanguageTarget.UseWaitCursor = True

        Do While Settings.BackgroundWorkerSetLanguage.IsBusy
            Application.DoEvents()
        Loop

        ' Languages (ComboBoxes)
        For Each Language As ClassLanguage In Settings.Languages
            Me.ComboBoxLanguageSource.Items.Add(Language.Name)
            Me.ComboBoxLanguageTarget.Items.Add(Language.Name)
        Next

        Me.ComboBoxLanguageSource.UseWaitCursor = False
        Me.ComboBoxLanguageTarget.UseWaitCursor = False
    End Sub

    Private Sub ComboBoxLanguageSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxLanguageSource.SelectedIndexChanged
        If Me.ComboBoxLanguageSource.SelectedIndex = -1 Then
            Exit Sub
        End If

        Settings.LanguageSource = ClassLanguage.GetFromName(Me.ComboBoxLanguageSource.SelectedItem.ToString())
    End Sub

    Private Sub ComboBoxLanguageTarget_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxLanguageTarget.SelectedIndexChanged
        If Me.ComboBoxLanguageTarget.SelectedIndex = -1 Then
            Exit Sub
        End If

        Settings.LanguageTarget = ClassLanguage.GetFromName(Me.ComboBoxLanguageTarget.SelectedItem.ToString())
    End Sub

    Private Sub ButtonCreatePO_Click(sender As Object, e As EventArgs) Handles ButtonCreatePO.Click
        If Not Me.FormIsValid Then
            Exit Sub
        End If

        Me.CreatePO()
    End Sub

    Private Sub ButtonCreatePOAll_Click(sender As Object, e As EventArgs) Handles ButtonCreatePOAll.Click
        If Settings.LanguageSource Is Nothing Then
            MessageBox.Show("Please select a source language.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If

        For Each LanguageTarget As ClassLanguage In Settings.Languages
            If Settings.LanguageSource.Locale = LanguageTarget.Locale Then
                Continue For
            End If

            Settings.LanguageTarget = LanguageTarget

            Me.ButtonCreatePO_Click(sender, e)
        Next

        Me.ComboBoxLanguageSource_SelectedIndexChanged(sender, e)
        Me.ComboBoxLanguageTarget_SelectedIndexChanged(sender, e)
    End Sub

    Private Sub ButtonUpdateTranslations_Click(sender As Object, e As EventArgs) Handles ButtonUpdateTranslations.Click
        If Not Me.FormIsValid Then
            Exit Sub
        End If

        LoaderUpdateTranslations.SetBackgroundWorker(Me.BackgroundWorkerUpdateTranslations)
        LoaderUpdateTranslations.Show()

        Me.BackgroundWorkerUpdateTranslations.RunWorkerAsync()
    End Sub

    Private Sub ButtonUpdateTranslationsAll_Click(sender As Object, e As EventArgs) Handles ButtonUpdateTranslationsAll.Click
        If Settings.LanguageSource Is Nothing Then
            MessageBox.Show("Please select a source language.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If

        LoaderUpdateTranslationsAll.SetBackgroundWorker(Me.BackgroundWorkerUpdateTranslationsAll)
        LoaderUpdateTranslationsAll.Show()

        Me.BackgroundWorkerUpdateTranslationsAll.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorkerUpdateTranslations_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerUpdateTranslations.DoWork
        Me.CreateConf()
        Me.CreateDefines()
    End Sub

    Private Sub BackgroundWorkerUpdateTranslations_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorkerUpdateTranslations.ProgressChanged
        LoaderUpdateTranslations.BackgroundWorkerLoader_ProgressChanged(sender, e)
    End Sub

    Private Sub BackgroundWorkerUpdateTranslations_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorkerUpdateTranslations.RunWorkerCompleted
        LoaderUpdateTranslations.Hide()

        MessageBox.Show("Finished creating/updating language files.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BackgroundWorkerUpdateTranslationsAll_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorkerUpdateTranslationsAll.DoWork
        LoaderUpdateTranslationsAll.SetMaximum(Settings.Languages.Count)

        For Each LanguageTarget As ClassLanguage In Settings.Languages
            LoaderUpdateTranslationsAll.PerformStep()

            If Settings.LanguageSource.Locale = LanguageTarget.Locale Then
                Continue For
            End If

            LoaderUpdateTranslationsAll.SetText("Creating/updating language " & Chr(34) & LanguageTarget.Name & Chr(34))

            Settings.LanguageTarget = LanguageTarget

            Me.BackgroundWorkerUpdateTranslations_DoWork(sender, e)
        Next
    End Sub

    Private Sub BackgroundWorkerUpdateTranslationsAll_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorkerUpdateTranslationsAll.ProgressChanged
        LoaderUpdateTranslationsAll.BackgroundWorkerLoader_ProgressChanged(sender, e)
    End Sub

    Private Sub BackgroundWorkerUpdateTranslationsAll_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorkerUpdateTranslationsAll.RunWorkerCompleted
        Me.ComboBoxLanguageSource_SelectedIndexChanged(sender, e)
        Me.ComboBoxLanguageTarget_SelectedIndexChanged(sender, e)

        LoaderUpdateTranslationsAll.Hide()

        MessageBox.Show("Finished creating/updating language files.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
#End Region

    Private Function FormIsValid() As Boolean
        If Settings.LanguageSource Is Nothing Or Settings.LanguageTarget Is Nothing Then
            MessageBox.Show("Please select a source and target language.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End If

        Return True
    End Function

    Private Sub CreatePO()
        Dim Lines As New List(Of String) From {
            ClassTranslationPO.WriteLine("", ClassTranslationPO.StringType.MSGID),
            ClassTranslationPO.WriteLine("", ClassTranslationPO.StringType.MSGSTR),
            ClassTranslationPO.WriteLine("Project-Id-Version: \n"),
            ClassTranslationPO.WriteLine("POT-Creation-Date: \n"),
            ClassTranslationPO.WriteLine("PO-Revision-Date: \n"),
            ClassTranslationPO.WriteLine("Last-Translator: \n"),
            ClassTranslationPO.WriteLine("Language-Team: \n"),
            ClassTranslationPO.WriteLine("Language: " & Settings.LanguageTarget.Locale & "\n"),
            ClassTranslationPO.WriteLine("MIME-Version: 1.0\n"),
            ClassTranslationPO.WriteLine("Content-Type: text/plain; charset=UTF-8\n"),
            ClassTranslationPO.WriteLine("Content-Transfer-Encoding: 8bit\n"),
            ClassTranslationPO.WriteLine("X-Generator: " & Application.ProductName & " " & Application.ProductVersion),
            ""
        }

        ' Conf
        For Each TranslationConf As ClassTranslationConf In Settings.LanguageSource.TranslationsConf
            Dim TranslationPO As New ClassTranslationPO With {
                .Context = ClassTranslationPO.ToPo(TranslationConf.GetContext()),
                .ID = ClassTranslationPO.ToPo(TranslationConf.Value),
                .Translation = ClassTranslationPO.ToPo(ClassTranslation.GetTranslationForPO(TranslationConf.Value, TranslationConf.GetContext()))
            }

            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.Context, ClassTranslationPO.StringType.MSGCTXT))
            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.ID, ClassTranslationPO.StringType.MSGID))
            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.Translation, ClassTranslationPO.StringType.MSGSTR))
            Lines.Add("")
        Next

        ' Define
        For Each TranslationDefine As ClassTranslationDefine In Settings.LanguageSource.TranslationsDefine
            Dim TranslationPO As New ClassTranslationPO With {
                .Context = ClassTranslationPO.ToPo(TranslationDefine.GetContext()),
                .ID = ClassTranslationPO.ToPo(TranslationDefine.Value),
                .Translation = ClassTranslationPO.ToPo(ClassTranslation.GetTranslationForPO(TranslationDefine.Value, TranslationDefine.GetContext()))
            }

            Lines.Add("# " & TranslationDefine.Name)
            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.Context, ClassTranslationPO.StringType.MSGCTXT))
            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.ID, ClassTranslationPO.StringType.MSGID))
            Lines.Add(ClassTranslationPO.WriteLine(TranslationPO.Translation, ClassTranslationPO.StringType.MSGSTR))
            Lines.Add("")
        Next

        ' Complete
        Dim Filepath As String = Settings.LanguageTarget.GetFilepathPO()

        File.WriteAllLines(Filepath, Lines)

        MessageBox.Show(Filepath & " has been created.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub CreateConf()
        Dim Lines As New List(Of String)

        Dim CurrentSection As String = ""
        Dim Filepath As String = Settings.LanguageTarget.GetFilepathConf()

        LoaderUpdateTranslations.SetText("Creating/updating " & Chr(34) & Filepath & Chr(34) & "...")
        LoaderUpdateTranslations.SetMaximum(Settings.LanguageTarget.TranslationsPO.Count)

        For Each TranslationPO As ClassTranslationPO In Settings.LanguageTarget.TranslationsPO
            LoaderUpdateTranslations.PerformStep()

            If TranslationPO.IsFromConf Then
                Dim TranslationConf As New ClassTranslationConf With {
                    .Section = TranslationPO.GetSection,
                    .Key = TranslationPO.GetKey,
                    .Value = TranslationPO.Translation
                }

                If CurrentSection <> TranslationConf.Section Then
                    Lines.Add("")
                    Lines.Add("[" & TranslationConf.Section & "]")

                    CurrentSection = TranslationConf.Section
                End If

                Lines.Add(TranslationConf.Key & " = '" & ClassTranslationPO.ToConf(TranslationConf.Value) & "'")
            End If
        Next

        ' Complete
        File.WriteAllLines(Filepath, Lines)
    End Sub

    Private Sub CreateDefines()
        LoaderUpdateTranslations.SetMaximum(Settings.LanguageSource.FilepathsDefine.Count)

        ' Remove orphans
        For Each FilepathDefineTarget As String In Settings.LanguageTarget.FilepathsDefine
            ' Files
            Dim FilepathDefineSource As String = ClassLanguage.GetFilepathSource(FilepathDefineTarget)

            If Not File.Exists(FilepathDefineSource) Then
                File.Delete(FilepathDefineTarget)

                Continue For
            End If

            ' Defines
            Dim FilecontentsDefineSource As String = File.ReadAllText(FilepathDefineSource)
            Dim FilecontentsDefineTarget As String = File.ReadAllText(FilepathDefineTarget)

            For Each DefineTarget As ClassTranslationDefine In Settings.LanguageTarget.TranslationsDefine
                If DefineTarget.Filepath <> FilepathDefineTarget Then
                    Continue For
                End If

                Dim RegexPattern As String = ClassTranslationDefine.REGEX_DEFINE.Replace(ClassTranslationDefine.REGEX_DEFINE_CONSTANT, DefineTarget.Name)
                Dim RegexOriginal As New Regex(RegexPattern, RegexOptions.Multiline)
                Dim SourceMatch As Match = RegexOriginal.Match(FilecontentsDefineSource)
                Dim TargetMatch As Match = RegexOriginal.Match(FilecontentsDefineTarget)

                If Not SourceMatch.Success AndAlso TargetMatch.Success Then
                    Dim Define As String() = ClassTranslationDefine.GetRegexDefineGroup(TargetMatch)
                    Dim Original As String = Define(0)

                    FilecontentsDefineTarget = FilecontentsDefineTarget.Replace(Original & vbLf, "")
                    FilecontentsDefineTarget = FilecontentsDefineTarget.Replace(Original & vbCrLf, "")
                    FilecontentsDefineTarget = FilecontentsDefineTarget.Replace(Original, "")
                End If
            Next

            File.WriteAllText(FilepathDefineTarget, FilecontentsDefineTarget)
        Next

        ' Update translations
        For Each FilepathDefineSource As String In Settings.LanguageSource.FilepathsDefine
            Dim FilepathDefineTarget As String = ClassLanguage.GetFilepathTarget(FilepathDefineSource)

            Dim FilecontentsDefine As String

            If File.Exists(FilepathDefineTarget) Then
                FilecontentsDefine = File.ReadAllText(FilepathDefineTarget)
            Else
                FilecontentsDefine = File.ReadAllText(FilepathDefineSource)
            End If

            LoaderUpdateTranslations.SetText("Creating/updating " & Chr(34) & FilepathDefineTarget.Substring(Settings.DirectoryShop.Length) & Chr(34) & "...")
            LoaderUpdateTranslations.PerformStep()

            For Each DefineSource As ClassTranslationDefine In Settings.LanguageSource.TranslationsDefine
                If FilepathDefineTarget <> DefineSource.GetFilepathTarget OrElse Not DefineSource.IsSuitedForPO Then
                    Continue For
                End If

                Dim DefineTranslation As String = ClassTranslation.GetTranslationForDefine(DefineSource.Value, DefineSource.GetContext)

                Dim RegexPattern As String = ClassTranslationDefine.REGEX_DEFINE.Replace(ClassTranslationDefine.REGEX_DEFINE_CONSTANT, DefineSource.Name)
                Dim RegexOriginal As New Regex(RegexPattern, RegexOptions.Multiline)
                Dim MatchOriginal As Match = RegexOriginal.Match(FilecontentsDefine)

                If MatchOriginal.Success Then
                    ' Replace translation
                    Dim Define As String() = ClassTranslationDefine.GetRegexDefineGroup(MatchOriginal)

                    Dim Original As String = Define(0)
                    Dim Value As String = Define(2)

                    If "" = Value Then
                        Continue For
                    End If

                    Dim Translation As String = Original.Replace(Value, ClassTranslationPO.ToDefine(DefineTranslation))

                    FilecontentsDefine = FilecontentsDefine.Replace(Original, Translation)
                Else
                    ' Insert translation
                    Dim Define As String = "define('" & DefineSource.Name & "', '" & ClassTranslationPO.ToDefine(DefineTranslation) & "');"

                    If FilecontentsDefine.Trim.EndsWith("?>") Then
                        Dim PositionEndOfFile As Integer = FilecontentsDefine.LastIndexOf("?>")

                        FilecontentsDefine = FilecontentsDefine.Insert(PositionEndOfFile, Define & vbLf)
                    Else
                        FilecontentsDefine = FilecontentsDefine.TrimEnd & vbLf & Define & vbLf
                    End If
                End If
            Next

            ' Create directory
            Dim DirectoryDefineTarget As String = Directory.GetParent(FilepathDefineTarget).FullName

            If Not Directory.Exists(DirectoryDefineTarget) Then
                Directory.CreateDirectory(DirectoryDefineTarget)
            End If

            ' Complete
            File.WriteAllText(FilepathDefineTarget, FilecontentsDefine)
        Next
    End Sub

End Class
