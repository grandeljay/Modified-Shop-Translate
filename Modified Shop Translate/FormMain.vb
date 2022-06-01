Imports System.IO
Imports System.Text.RegularExpressions

Public Class FormMain
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

        Me.CreateConf()
        Me.CreateDefines()
    End Sub

    Private Sub ButtonUpdateTranslationsAll_Click(sender As Object, e As EventArgs) Handles ButtonUpdateTranslationsAll.Click
        If Settings.LanguageSource Is Nothing Then
            MessageBox.Show("Please select a source language.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If

        For Each LanguageTarget As ClassLanguage In Settings.Languages
            If Settings.LanguageSource.Locale = LanguageTarget.Locale Then
                Continue For
            End If

            Settings.LanguageTarget = LanguageTarget

            Me.ButtonUpdateTranslations_Click(sender, e)
        Next

        Me.ComboBoxLanguageSource_SelectedIndexChanged(sender, e)
        Me.ComboBoxLanguageTarget_SelectedIndexChanged(sender, e)
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
                .Translation = ClassTranslationPO.ToPo(ClassLanguage.GetTranslationForPO(TranslationConf.Value, TranslationConf.GetContext()))
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
                .Translation = ClassTranslationPO.ToPo(ClassLanguage.GetTranslationForPO(TranslationDefine.Value, TranslationDefine.GetContext()))
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

        For Each TranslationPO As ClassTranslationPO In Settings.LanguageTarget.TranslationsPO
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
        Dim Filepath As String = Settings.LanguageTarget.GetFilepathConf()

        File.WriteAllLines(Filepath, Lines)

        MessageBox.Show(Filepath & " has been created.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub CreateDefines()
        For Each FilepathDefine As String In Settings.LanguageTarget.FilepathsDefine
            Dim FilecontentsDefine As String = File.ReadAllText(FilepathDefine)

            For Each DefineSource As ClassTranslationDefine In Settings.LanguageSource.TranslationsDefine
                Dim DefineTranslation As String = ClassLanguage.GetTranslationForDefine(DefineSource.Value, DefineSource.GetContext)

                Dim RegexPattern As String = ClassTranslationDefine.REGEX_DEFINE.Replace(ClassTranslationDefine.REGEX_DEFINE_CONSTANT, DefineSource.Name)
                Dim RegexOriginal As New Regex(RegexPattern, RegexOptions.Multiline)
                Dim MatchOriginal As Match = RegexOriginal.Match(FilecontentsDefine)

                If MatchOriginal.Success AndAlso DefineSource.IsSuitedForPO AndAlso FilepathDefine = DefineSource.GetFilepathForTarget(Settings.LanguageSource, Settings.LanguageTarget) Then
                    Dim Define As String() = ClassTranslationDefine.GetRegexDefineGroup(MatchOriginal)

                    Dim Original As String = Define(0)
                    Dim Value As String = Define(2)

                    If "" = Value Then
                        Continue For
                    End If

                    Dim Translation As String = Original.Replace(Value, ClassTranslationPO.ToDefine(DefineTranslation))

                    FilecontentsDefine = FilecontentsDefine.Replace(Original, Translation)
                End If
            Next

            ' Complete
            File.WriteAllText(FilepathDefine, FilecontentsDefine)

            ' MessageBox.Show(FilepathDefine & " has been created.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Next

        MessageBox.Show("All translation files updated.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
