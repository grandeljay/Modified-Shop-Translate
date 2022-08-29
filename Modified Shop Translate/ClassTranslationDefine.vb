
Imports System.Text.RegularExpressions

Public Class ClassTranslationDefine
    Public Const REGEX_DEFINE_CONSTANT As String = "[A-Z_0-9]+"
    Public Const REGEX_DEFINE_BASE As String = "(define *\('(" & REGEX_DEFINE_CONSTANT & ")' *, *'(.*?)'\);)"
    Public Const REGEX_DEFINE As String = REGEX_DEFINE_BASE & "[\n\r/]|" & REGEX_DEFINE_BASE & "$|" & REGEX_DEFINE_BASE

#Region "Static"
    Public Shared Function GetTranslation(TextToTranslate As String, Optional Context As String = Nothing) As String
        ' Find Source translations
        Dim TranslationSource As ClassTranslationDefine = Nothing

        For Each SourceDefine As ClassTranslationDefine In FormMain.Settings.LanguageSource.TranslationsDefine
            If SourceDefine.Value = TextToTranslate AndAlso SourceDefine.GetContext = Context Then
                TranslationSource = SourceDefine
                Exit For
            End If
        Next

        If TranslationSource Is Nothing Then
            Return ""
        End If

        ' Find Target translation
        For Each TranslationTarget As ClassTranslationDefine In FormMain.Settings.LanguageTarget.TranslationsDefine
            If TranslationSource.GetFilepathTarget <> TranslationTarget.Filepath Then
                Continue For
            End If

            If TranslationSource.Name = TranslationTarget.Name Then
                Return TranslationTarget.Value
            End If
        Next

        Return ""
    End Function

    Public Shared Function GetRegexDefineGroup(MatchDefine As Match) As String()
        Dim Group1Original As String = MatchDefine.Groups(1).Value
        Dim Group1Name As String = MatchDefine.Groups(2).Value
        Dim Group1Value As String = MatchDefine.Groups(3).Value

        Dim Group2Original As String = MatchDefine.Groups(4).Value
        Dim Group2Name As String = MatchDefine.Groups(5).Value
        Dim Group2Value As String = MatchDefine.Groups(6).Value

        Dim Group3Original As String = MatchDefine.Groups(7).Value
        Dim Group3Name As String = MatchDefine.Groups(8).Value
        Dim Group3Value As String = MatchDefine.Groups(9).Value

        Dim Original As String = ""
        Dim Name As String = ""
        Dim Value As String = ""

        If Group1Original.Length >= Original.Length Then Original = Group1Original
        If Group1Name.Length >= Name.Length Then Name = Group1Name
        If Group1Value.Length >= Value.Length Then Value = Group1Value

        If Group2Original.Length >= Original.Length Then Original = Group2Original
        If Group2Name.Length >= Name.Length Then Name = Group2Name
        If Group2Value.Length >= Value.Length Then Value = Group2Value

        If Group3Original.Length >= Original.Length Then Original = Group3Original
        If Group3Name.Length >= Name.Length Then Name = Group3Name
        If Group3Value.Length >= Value.Length Then Value = Group3Value

        Dim Define As String() = {
            Original,
            Name,
            Value
        }

        Return Define
    End Function
#End Region

#Region "Public"
    Public Property Original As String
    Public Property Name As String
    Public Property Value As String
    Public Property Filepath As String

    Public Function GetContext() As String
        Dim Context As String = String.Concat(Me.Filepath.AsSpan(FormMain.Settings.DirectoryShop.Length), " ", Me.Name).Replace("\", "\\")

        Return Context
    End Function

    Public Function IsSuitedForPO() As Boolean
        ' Regexes
        Dim RegexQuoteSingle As New Regex("'")
        Dim RegexQuoteSingleEscaped As New Regex("\\'")

        ' Matches
        Dim MatchQuoteSingle As MatchCollection = RegexQuoteSingle.Matches(Me.Value)
        Dim MatchQuoteSingleEscaped As MatchCollection = RegexQuoteSingleEscaped.Matches(Me.Value)

        Return MatchQuoteSingle.Count = MatchQuoteSingleEscaped.Count
    End Function

    Public Function GetFilepathTarget(Optional LanguageSource As ClassLanguage = Nothing, Optional LanguageTarget As ClassLanguage = Nothing) As String
        Return ClassLanguage.GetFilepathTarget(Me.Filepath, LanguageSource, LanguageTarget)
    End Function
#End Region

End Class
