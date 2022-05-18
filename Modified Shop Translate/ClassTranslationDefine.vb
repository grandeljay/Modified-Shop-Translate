
Public Class ClassTranslationDefine
    Public Property Original As String
    Public Property Name As String
    Public Property Value As String

    ''' <summary>
    ''' Returns the original define statement with the value subsituted with the supplied translation.
    ''' </summary>
    ''' <param name="Translation">The translation to subsitute the original value with</param>
    ''' <returns>String</returns>
    Public Function GetOriginalTranslated(TranslationToUse As String) As String
        If Me.Value Is "" Then
            Return ""
        End If

        Dim Translation As String = Me.Original

        Translation = Translation.Replace(Me.Value, TranslationToUse)

        Return Translation
    End Function

    Public Function GetContext() As String
        Return Me.Name
    End Function
End Class
