Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Net

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

    Public Shared Function GetTranslationForPO(TextToTranslate As String, Optional Context As String = Nothing) As String
        If TextToTranslate = "" Then
            Return ""
        End If

        ' Search Conf
        Dim TranslationConf As String = ClassTranslationConf.GetTranslation(TextToTranslate)

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        ' Search Defines
        Dim TranslationDefine As String = ClassTranslationDefine.GetTranslation(TextToTranslate, Context)

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        ' Search PO
        Dim TranslationPO As String = ClassTranslationPO.GetTranslation(TextToTranslate, Context)

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        Return ""
    End Function

    Public Shared Function GetTranslationForDefine(TextToTranslate As String, Optional Context As String = Nothing) As String
        ' Search PO
        Dim TranslationPO As String = ClassTranslationPO.FromPO(ClassTranslationPO.GetTranslation(TextToTranslate, Context))

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        ' Search Conf
        Dim TranslationConf As String = ClassTranslationPO.FromConf(ClassTranslationConf.GetTranslation(TextToTranslate))

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        ' Search Define
        Dim TranslationDefine As String = ClassTranslationPO.FromDefine(ClassTranslationDefine.GetTranslation(TextToTranslate, Context))

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        Return TextToTranslate
    End Function

    Public Shared Function GetTranslationForConf(TextToTranslate As String, Optional Context As String = Nothing) As String
        ' Search PO
        Dim TranslationPO As String = ClassTranslationPO.GetTranslation(TextToTranslate, Context)

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        ' Search Define
        Dim TranslationDefine As String = ClassTranslationDefine.GetTranslation(TextToTranslate, Context)

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        ' Search Conf
        Dim TranslationConf As String = ClassTranslationConf.GetTranslation(TextToTranslate)

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        Return TextToTranslate
    End Function

    Public Shared Function GetFilepathSource(FilepathTarget As String, Optional LanguageSource As ClassLanguage = Nothing, Optional LanguageTarget As ClassLanguage = Nothing) As String
        If LanguageSource Is Nothing Then LanguageSource = FormMain.Settings.LanguageSource
        If LanguageTarget Is Nothing Then LanguageTarget = FormMain.Settings.LanguageTarget

        Return FilepathTarget.Replace(LanguageTarget.Name, LanguageSource.Name)
    End Function

    Public Shared Function GetFilepathTarget(FilepathSource As String, Optional LanguageSource As ClassLanguage = Nothing, Optional LanguageTarget As ClassLanguage = Nothing) As String
        If LanguageSource Is Nothing Then LanguageSource = FormMain.Settings.LanguageSource
        If LanguageTarget Is Nothing Then LanguageTarget = FormMain.Settings.LanguageTarget

        Return FilepathSource.Replace(LanguageSource.Name, LanguageTarget.Name)
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

        ' Regexes

        ' Context
        ' msgctxt \"(.*?)\"
        ' msgctxt " & Chr(34) & "(.*?) " & Chr(34)
        Dim RegexContext As New Regex("msgctxt " & Chr(34) & "(.*?)" & Chr(34))

        ' ID
        ' ^msgid \"(.*?)\"$
        ' "^msgid " & Chr(34) & "(.*?)" & Chr(34) & "$"
        Dim RegexID As New Regex("^msgid " & Chr(34) & "(.*?)" & Chr(34) & "$")

        ' ^\"(.*?)\"$
        ' "^" & Chr(34) & "(.*?)" & Chr(34) & "$
        Dim RegexID_Multiline As New Regex("^" & Chr(34) & "(.*?)" & Chr(34) & "$")

        ' Translations
        ' ^msgstr \"(.*?)\"$
        ' "^msgstr " & Chr(34) & "(.*?)" & Chr(34) & "$"
        Dim RegexTranslation As New Regex("^msgstr " & Chr(34) & "(.*?)" & Chr(34) & "$")
        Dim RegexTranslation_Multiline As Regex = RegexID_Multiline

        ' Matches
        Dim MatchContext As Match
        Dim MatchID As Match
        Dim MatchID_Multiline As Match
        Dim MatchTranslation As Match
        Dim MatchTranslation_Multiline As Match

        ' Parse
        Dim FilepathPO_Lines As String() = File.ReadAllLines(FilepathPO)
        Dim TranslationPO As New ClassTranslationPO
        Dim LastType As ClassTranslationPO.StringType = ClassTranslationPO.StringType.NONE

        For Each Line As String In FilepathPO_Lines
            ' Context
            MatchContext = RegexContext.Match(Line)

            If MatchContext.Success Then
                TranslationPO.Context = MatchContext.Groups(1).Value

                LastType = ClassTranslationPO.StringType.MSGCTXT
                Continue For
            End If

            ' ID
            MatchID = RegexID.Match(Line)

            If MatchID.Success Then
                TranslationPO.ID = MatchID.Groups(1).Value

                LastType = ClassTranslationPO.StringType.MSGID
                Continue For
            End If

            MatchID_Multiline = RegexID_Multiline.Match(Line)

            If MatchID_Multiline.Success AndAlso LastType = ClassTranslationPO.StringType.MSGID Then
                TranslationPO.ID &= MatchID_Multiline.Groups(1).Value

                LastType = ClassTranslationPO.StringType.MSGID
                Continue For
            End If

            ' Translation
            MatchTranslation = RegexTranslation.Match(Line)

            If MatchTranslation.Success Then
                TranslationPO.Translation = MatchTranslation.Groups(1).Value

                LastType = ClassTranslationPO.StringType.MSGSTR
                Continue For
            End If

            MatchTranslation_Multiline = RegexTranslation_Multiline.Match(Line)

            If MatchTranslation_Multiline.Success AndAlso LastType = ClassTranslationPO.StringType.MSGSTR Then
                TranslationPO.Translation &= MatchTranslation_Multiline.Groups(1).Value

                LastType = ClassTranslationPO.StringType.MSGSTR
                Continue For
            End If

            ' End Translation
            If String.IsNullOrEmpty(TranslationPO.ID) AndAlso String.IsNullOrEmpty(TranslationPO.Translation) AndAlso TranslationsPO.Count = 0 Then
                Continue For
            End If

            If String.IsNullOrEmpty(Line.Trim()) AndAlso TranslationPO.Context IsNot Nothing AndAlso TranslationPO.ID IsNot Nothing Then
                TranslationsPO.Add(TranslationPO)
                TranslationPO = New ClassTranslationPO

                Continue For
            End If

            LastType = ClassTranslationPO.StringType.NONE
        Next

        Return TranslationsPO
    End Function

    Private Function GetTranslationsDefine() As List(Of ClassTranslationDefine)
        Dim TranslationsDefine As New List(Of ClassTranslationDefine)

        ' Regex
        Dim RegexPattern As New Regex(ClassTranslationDefine.REGEX_DEFINE, RegexOptions.Multiline)

        ' Match
        Dim MatchesDefine As MatchCollection

        ' Wait for index
        Do While FormMain.Settings.BackgroundWorkerGetFiles.IsBusy
            Threading.Thread.Sleep(1000)
        Loop

        For Each FilepathDefine As String In Me.FilepathsDefine
            ' Lines
            Dim FilepathDefineContents As String = File.ReadAllText(FilepathDefine)

            MatchesDefine = RegexPattern.Matches(FilepathDefineContents)

            For Each MatchDefine As Match In MatchesDefine
                If MatchDefine.Success Then
                    Dim Define As String() = ClassTranslationDefine.GetRegexDefineGroup(MatchDefine)

                    Dim TranslationDefine As New ClassTranslationDefine With {
                        .Filepath = FilepathDefine,
                        .Original = Define(0),
                        .Name = Define(1),
                        .Value = Define(2)
                    }

                    If TranslationDefine.IsSuitedForPO Then
                        TranslationsDefine.Add(TranslationDefine)
                    End If
                End If
            Next

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

    Public Property FilepathsDefine As New List(Of String)

    Public Sub New(Optional LanguageDirectory As String = "")
        If LanguageDirectory Is "" AndAlso Not Directory.Exists(LanguageDirectory) Then
            Exit Sub
        End If

        ' Directories
        Me.DirectoryPath = LanguageDirectory
        Me.Name = Path.GetFileName(Me.DirectoryPath)
        Me.FilepathsDefine = Directory.GetFiles(LanguageDirectory, "*.php", SearchOption.AllDirectories).ToList

        'For Each Culture As CultureInfo In CultureInfo.GetCultures(CultureTypes.AllCultures)
        '    If Culture.NativeName.ToLower().Contains(Me.Name.ToLower()) _
        '    Or Culture.EnglishName.ToLower().Contains(Me.Name.ToLower()) _
        '    Or Culture.DisplayName.ToLower().Contains(Me.Name.ToLower()) _
        '    Then
        '        Debug.WriteLine(Culture)
        '    End If
        'Next

        Select Case Me.Name.ToLower()
            Case "bulgarian" : Me.Locale = "bg_BG"
            Case "english" : Me.Locale = "en_GB"
            Case "french" : Me.Locale = "fr_FR"
            Case "german" : Me.Locale = "de_DE"
            Case "italian" : Me.Locale = "it_IT"
            Case "polish" : Me.Locale = "pl_PL"
            Case "spanish" : Me.Locale = "es_ES"
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

#End Region

End Class
