Imports System.IO
Imports System.Text.RegularExpressions

Public Class ClassLanguage

#Region "Static"
    Public Shared Function GetFromName(LanguageNameToFind As String) As ClassLanguage
        For Each Language As ClassLanguage In FormMain.Settings.Languages
            If Language.Name = LanguageNameToFind Then
                Return Language
            End If
        Next

        Throw New System.Exception("Unable to find language " & Chr(34) & LanguageNameToFind & Chr(34) & ".")
    End Function

    Public Shared Function GetTranslation(TextToTranslate As String, Optional LanguageSource As ClassLanguage = Nothing, Optional LanguageTarget As ClassLanguage = Nothing) As String
        If LanguageSource Is Nothing Then LanguageSource = FormMain.Settings.LanguageSource
        If LanguageTarget Is Nothing Then LanguageTarget = FormMain.Settings.LanguageTarget

        ' Search PO
        For Each SourcePO As ClassTranslationPO In LanguageSource.TranslationsPO
            If TextToTranslate Is SourcePO.Translation Then
                ' Search target
                For Each TargetPO As ClassTranslationPO In LanguageTarget.TranslationsPO
                    If SourcePO.Context Is TargetPO.Context AndAlso SourcePO.ID Is TargetPO.ID AndAlso TargetPO.Translation IsNot "" Then
                        Return TargetPO.Translation
                    End If
                Next
            End If
        Next

        ' Search Conf
        For Each SourceConf As ClassTranslationConf In LanguageSource.TranslationsConf
            If TextToTranslate Is SourceConf.Value Then
                ' Search target
                For Each TargetConf As ClassTranslationConf In LanguageTarget.TranslationsConf
                    If SourceConf.Section = TargetConf.Section AndAlso SourceConf.Key = TargetConf.Key AndAlso TargetConf.Value IsNot "" Then
                        Return TargetConf.Value
                    End If
                Next
            End If
        Next

        ' Search Define
        For Each SourceDefine As ClassTranslationDefine In LanguageSource.TranslationsDefine
            If TextToTranslate Is SourceDefine.Value Then
                ' Search target
                For Each TargetDefine As ClassTranslationDefine In LanguageTarget.TranslationsDefine
                    If SourceDefine.Name = TargetDefine.Name AndAlso TargetDefine.Value IsNot "" Then
                        Return TargetDefine.Value
                    End If
                Next
            End If
        Next

        Return ""
    End Function
#End Region

#Region "Private"
    Private Property DirectoryPath As String

    Private Function GetTranslationsConf() As List(Of ClassTranslationConf)
        Dim TranslationsConf As New List(Of ClassTranslationConf)

        ' Filepaths
        Dim FilepathConf As String = Me.GetFilepathConf()

        If Not File.Exists(FilepathConf) Then
            Return TranslationsConf
        End If

        Dim FilepathConf_Lines As String() = File.ReadAllLines(FilepathConf)

        ' Regexes
        Dim RegexSection As New Regex("^\[([a-z_]+)\]")
        Dim RegexTranslation As New Regex("^([A-Za-z_0-9]+) += +'(.*?)'$")

        ' Matches
        Dim MatchSection As Match
        Dim MatchTranslation As Match

        ' Process
        Dim CurrentSection As String = ""

        For Each LineConf As String In FilepathConf_Lines
            ' Section
            MatchSection = RegexSection.Match(LineConf)

            If MatchSection.Success Then
                CurrentSection = MatchSection.Groups(1).Value
            End If

            If String.IsNullOrEmpty(LineConf.Trim()) Then
                CurrentSection = ""
            End If

            ' Translation
            MatchTranslation = RegexTranslation.Match(LineConf)

            If MatchTranslation.Success Then
                Dim Key As String = MatchTranslation.Groups(1).Value
                Dim Value As String = MatchTranslation.Groups(2).Value

                Dim TranslationConf As New ClassTranslationConf With {
                    .Section = CurrentSection,
                    .Key = Key,
                    .Value = Value
                }

                ' Check for duplicates
                Dim DuplicateExists As Boolean = False

                For Each PotentialDuplicate As ClassTranslationConf In TranslationsConf
                    If PotentialDuplicate.Section = TranslationConf.Section AndAlso PotentialDuplicate.Key = TranslationConf.Key Then
                        DuplicateExists = True

                        Exit For
                    End If
                Next

                If Not DuplicateExists Then
                    TranslationsConf.Add(TranslationConf)
                End If
            End If
        Next

        Return TranslationsConf
    End Function

    Private Function GetTranslationsPO() As List(Of ClassTranslationPO)
        Dim TranslationsPO As New List(Of ClassTranslationPO)

        ' Filepaths
        Dim FilepathPO As String = Me.GetFilepathPO()

        If Not File.Exists(FilepathPO) Then
            Return TranslationsPO
        End If

        Dim FilepathPO_Text As String = File.ReadAllText(FilepathPO)

        ' Regexes
        Dim RegexPattern As New Regex(
            "^msgctxt " & Chr(34) & "\[([a-z_0-9]+)\] ([a-zA-Z_0-9]+)" & Chr(34) & "\s+.+?" & Chr(34) & ".*?" & Chr(34) & "\s+msgstr " & Chr(34) & "(.*?)" & Chr(34) & "\s\s",
            RegexOptions.Multiline
        )

        ' Matches
        Dim Matches As MatchCollection = RegexPattern.Matches(FilepathPO_Text)

        ' Process
        For Each Match As Match In Matches
            If Match.Success Then
                Dim TranslationConf As New ClassTranslationConf With {
                    .Section = Match.Groups(1).Value,
                    .Key = Match.Groups(2).Value,
                    .Value = Match.Groups(3).Value
                }

                Dim TranslationPO As New ClassTranslationPO With {
                    .Context = TranslationConf.GetContext(),
                    .ID = TranslationConf.Key,
                    .Translation = TranslationConf.Value
                }

                TranslationsPO.Add(TranslationPO)
            End If
        Next

        Return TranslationsPO
    End Function

    Private Function GetTranslationsDefine() As List(Of ClassTranslationDefine)
        Dim TranslationsDefine As New List(Of ClassTranslationDefine)

        ' Filepaths
        Dim FilepathDefine As String = Me.GetFilepathDefine()

        If Not File.Exists(FilepathDefine) Then
            Return TranslationsDefine
        End If

        Dim FilepathDefine_Lines As String() = File.ReadAllLines(FilepathDefine)

        ' Regexes
        Dim RegexPattern As New Regex("define\('([A-Z_0-9]+)', *'(.*?)'\);")
        Dim RegexQuoteSingle As New Regex("'")
        Dim RegexQuoteSingleEscaped As New Regex("\\'")

        ' Matches
        Dim MatchDefine As Match
        Dim MatchQuoteSingle As MatchCollection
        Dim MatchQuoteSingleEscaped As MatchCollection

        ' Process
        For Each LineDefine As String In FilepathDefine_Lines
            MatchDefine = RegexPattern.Match(LineDefine)

            If MatchDefine.Success Then
                Dim Define As String = MatchDefine.Groups(0).Value
                Dim Key As String = MatchDefine.Groups(1).Value
                Dim Value As String = MatchDefine.Groups(2).Value

                MatchQuoteSingle = RegexQuoteSingle.Matches(Value)
                MatchQuoteSingleEscaped = RegexQuoteSingleEscaped.Matches(Value)

                ' Use this match
                If MatchQuoteSingle.Count = MatchQuoteSingleEscaped.Count Then
                    Dim TranslationDefine As New ClassTranslationDefine With {
                        .Original = Define,
                        .Name = Key,
                        .Value = Value
                    }

                    TranslationsDefine.Add(TranslationDefine)
                End If
            End If
        Next

        Return TranslationsDefine
    End Function
#End Region

#Region "Public"
    Public Property Name As String
    Public Property Locale As String

    Public Property TranslationsConf As New List(Of ClassTranslationConf)
    Public Property TranslationsPO As New List(Of ClassTranslationPO)
    Public Property TranslationsDefine As New List(Of ClassTranslationDefine)

    Public Sub New(Optional LanguageDirectory As String = "")
        If LanguageDirectory Is "" AndAlso Not Directory.Exists(LanguageDirectory) Then
            Exit Sub
        End If

        ' Directories
        Me.DirectoryPath = LanguageDirectory
        Me.Name = Path.GetFileName(Me.DirectoryPath)

        Select Case Me.Name.ToLower()
            Case "bulgarian" : Me.Locale = "bg-BG"
            Case "english" : Me.Locale = "en-GB"
            Case "french" : Me.Locale = "fr-FR"
            Case "german" : Me.Locale = "de-DE"
            Case "italian" : Me.Locale = "it-IT"
            Case "polish" : Me.Locale = "pl-PL"
            Case "spanish" : Me.Locale = "es-ES"
            Case Else
                Throw New System.Exception("There is no known locale for language " & Me.Name & ". Please improve the source code.")
        End Select

        ' Translations
        Me.TranslationsConf = Me.GetTranslationsConf
        Me.TranslationsPO = Me.GetTranslationsPO
        Me.TranslationsDefine = Me.GetTranslationsDefine
    End Sub

    Public Function GetFilepathConf() As String
        Return Me.DirectoryPath & "\lang_" & Me.Name & ".conf"
    End Function

    Public Function GetFilepathPO() As String
        Return Me.DirectoryPath & "\" & Me.Name & ".po"
    End Function

    Public Function GetFilepathDefine() As String
        Return Me.DirectoryPath & "\" & Me.Name & ".php"
    End Function
#End Region

End Class
