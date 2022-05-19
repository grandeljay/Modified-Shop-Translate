Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions

Public Class FormMain
    Public Settings As New ClassSettings

#Region "FormEvents"
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set Shop Path
        Do While Not Directory.Exists(Settings.DirectoryShop)
            FormSetDirectoryShop.ShowDialog()
        Loop

        ' Set language confs
        Dim LanguagePaths As String() = Directory.GetDirectories(Settings.DirectoryShop & "\lang", "*", SearchOption.TopDirectoryOnly)
        Dim LanguageDirectories As New List(Of String)

        For Each LanguagePath As String In LanguagePaths
            Dim LanguageName As String = Path.GetFileName(LanguagePath)
            Dim LanguageConf As String = LanguagePath & "\lang_" & LanguageName & ".conf"

            If File.Exists(LanguageConf) Then
                LanguageDirectories.Add(LanguagePath)
            End If
        Next

        ' Create/Update PO files
        Settings.Languages = New List(Of ClassLanguage)

        For Each LanguageDirectory As String In LanguageDirectories
            Settings.Languages.Add(New ClassLanguage(LanguageDirectory))
        Next

        ' Languages (ComboBoxes)
        For Each Language As ClassLanguage In Settings.Languages
            Me.ComboBoxLanguageSource.Items.Add(Language.Name)
            Me.ComboBoxLanguageTarget.Items.Add(Language.Name)
        Next
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

    Private Sub ButtonUpdateTranslations_Click(sender As Object, e As EventArgs) Handles ButtonUpdateTranslations.Click
        If Not Me.FormIsValid Then
            Exit Sub
        End If

        Me.CreateConf()
        Me.CreateDefine()
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
        Dim Lines As New List(Of String)

        Lines.Add(ClassTranslationPO.WriteLine("", ClassTranslationPO.StringType.MSGID))
        Lines.Add(ClassTranslationPO.WriteLine("", ClassTranslationPO.StringType.MSGSTR))
        Lines.Add(ClassTranslationPO.WriteLine("Project-Id-Version: \n"))
        Lines.Add(ClassTranslationPO.WriteLine("POT-Creation-Date: \n"))
        Lines.Add(ClassTranslationPO.WriteLine("PO-Revision-Date: \n"))
        Lines.Add(ClassTranslationPO.WriteLine("Last-Translator: \n"))
        Lines.Add(ClassTranslationPO.WriteLine("Language-Team: \n"))
        Lines.Add(ClassTranslationPO.WriteLine("Language: en\n"))
        Lines.Add(ClassTranslationPO.WriteLine("MIME-Version: 1.0\n"))
        Lines.Add(ClassTranslationPO.WriteLine("Content-Type: text/plain; charset=UTF-8\n"))
        Lines.Add(ClassTranslationPO.WriteLine("Content-Transfer-Encoding: 8bit\n"))
        Lines.Add(ClassTranslationPO.WriteLine("X-Generator: Modified Shop Translate " & Application.ProductVersion))
        Lines.Add("")

        ' Conf
        For Each TranslationConf As ClassTranslationConf In Settings.LanguageSource.TranslationsConf
            Dim TranslationPO As New ClassTranslationPO With {
                .Context = ClassTranslationPO.ToPo(TranslationConf.GetContext()),
                .ID = ClassTranslationPO.ToPo(TranslationConf.Value),
                .Translation = ClassTranslationPO.ToPo(ClassLanguage.GetTranslation(TranslationConf.Value, TranslationConf.GetContext()))
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
                .Translation = ClassTranslationPO.ToPo(ClassLanguage.GetTranslation(TranslationDefine.Value, TranslationDefine.GetContext()))
            }

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

        For Each TranslationConf As ClassTranslationConf In Settings.LanguageSource.TranslationsConf
            If CurrentSection <> TranslationConf.Section Then
                Lines.Add("")
                Lines.Add("[" & TranslationConf.Section & "]")

                CurrentSection = TranslationConf.Section
            End If

            Lines.Add(TranslationConf.Key & " = '" & ClassTranslationPO.ToConf(TranslationConf.Value) & "'")
        Next

        ' Complete
        Dim Filepath As String = Settings.LanguageTarget.GetFilepathConf()

        File.WriteAllLines(Filepath, Lines)

        MessageBox.Show(Filepath & " has been created.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub CreateDefine()
        Dim FilecontentsDefine As String = File.ReadAllText(Settings.LanguageTarget.GetFilepathDefine())

        For Each DefineSource As ClassTranslationDefine In Settings.LanguageSource.TranslationsDefine
            ' Search for translation
            For Each DefineTarget As ClassTranslationDefine In Settings.LanguageTarget.TranslationsDefine
                If DefineSource.Name = DefineTarget.Name Then
                    FilecontentsDefine = FilecontentsDefine.Replace(
                        DefineSource.Original,
                        DefineSource.GetOriginalTranslated(DefineTarget.Value)
                    )

                    Exit For
                End If
            Next
        Next

        ' Complete
        Dim Define_Path As String = Settings.LanguageTarget.GetFilepathDefine()

        File.WriteAllText(Define_Path, FilecontentsDefine)

        MessageBox.Show(Define_Path & " has been created.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
