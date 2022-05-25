
Public Class ClassTranslationDefine
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
        Return Me.Name
    End Function
#End Region

End Class
