
Imports System.Text.RegularExpressions

Public Class ClassTranslationDefine
    Public Const REGEX_DEFINE_CONSTANT As String = "[A-Z_0-9]+"
    Public Const REGEX_DEFINE As String = "define\('(" & REGEX_DEFINE_CONSTANT & ")', *'(.*?)'\);"

#Region "Static"
    Public Shared Function GetTranslation(TextToTranslate As String) As String
        For Each SourceDefine As ClassTranslationDefine In FormMain.Settings.LanguageSource.TranslationsDefine
            If TextToTranslate Is SourceDefine.Value Then
                ' Search target
                For Each TargetDefine As ClassTranslationDefine In FormMain.Settings.LanguageTarget.TranslationsDefine
                    If SourceDefine.Name = TargetDefine.Name AndAlso TargetDefine.Value IsNot "" Then
                        Return TargetDefine.Value
                    End If
                Next
            End If
        Next

        Return ""
    End Function
#End Region

#Region "Public"
    Public Property Original As String
    Public Property Name As String
    Public Property Value As String
    Public Property IsAdmin As Boolean = False

    ''' <summary>
    ''' Returns the original define statement with the value subsituted with the supplied translation.
    ''' </summary>
    ''' <param name="Translation">The translation to subsitute the original value with</param>
    ''' <returns>String</returns>
    Public Function GetOriginalTranslated(TranslationToUse As String) As String
        If Me.Value Is "" Then
            Return Me.Original
        End If

        Dim Translation As String = Me.Original

        Translation = Translation.Replace(Me.Value, TranslationToUse)

        Return Translation
    End Function

    Public Function GetContext() As String
        Dim Context As String = Me.Name

        If IsAdmin Then
            Context &= " (Admin)"
        End If

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
#End Region

End Class
