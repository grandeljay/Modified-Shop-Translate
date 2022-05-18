
Public Class ClassTranslationConf

#Region "Static"

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
