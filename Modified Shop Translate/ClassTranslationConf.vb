
Public Class ClassTranslationConf

#Region "Static"
    Public Shared Function GetTranslation(TextToTranslate As String, Optional Context As String = Nothing) As String
        For Each SourceConf As ClassTranslationConf In FormMain.Settings.LanguageSource.TranslationsConf
            If TextToTranslate Is SourceConf.Value Then
                ' Search target
                For Each TargetConf As ClassTranslationConf In FormMain.Settings.LanguageTarget.TranslationsConf
                    If SourceConf.Section = TargetConf.Section AndAlso SourceConf.Key = TargetConf.Key AndAlso TargetConf.Value IsNot "" Then
                        Return TargetConf.Value
                    End If
                Next
            End If
        Next

        Return ""
    End Function
#End Region

#Region "Public"
    Public Property Section As String = ""
    Public Property Key As String
    Public Property Value As String = ""

    Public Function GetContext() As String
        Return "[" & Me.Section & "] " & Me.Key
    End Function
#End Region
End Class
