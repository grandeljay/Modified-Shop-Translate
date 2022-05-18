Imports System.Net

Public Class ClassTranslationPO

#Region "Static"
    ''' <summary>
    ''' Prepares the input string by escaping and unescaping values as well as decoding html entities.
    ''' </summary>
    ''' <param name="StringToConvert">The input string to manipulate.</param>
    ''' <returns>String</returns>
    Public Shared Function ToPo(StringToConvert As String) As String
        Const EscapeChracter As String = "\"

        ' Remove slashes
        Dim ChractersToUnescape As New List(Of String) From {
            "'"
        }

        For Each ChracterToUnescape As String In ChractersToUnescape
            StringToConvert = StringToConvert.Replace(EscapeChracter & ChracterToUnescape, ChracterToUnescape)
        Next

        ' Add slashes
        Dim ChractersToEscape As New List(Of String) From {
            Chr(34)
        }

        For Each ChracterToEscape As String In ChractersToEscape
            StringToConvert = StringToConvert.Replace(ChracterToEscape, EscapeChracter & ChracterToEscape)
        Next

        ' Convert HTML entities      
        StringToConvert = WebUtility.HtmlDecode(StringToConvert)

        Return StringToConvert
    End Function

    ''' <summary>
    ''' Prepares the input string by escaping and unescaping values as well as decoding html entities.
    ''' </summary>
    ''' <param name="StringToConvert">The input string to manipulate.</param>
    ''' <returns>String</returns>
    Public Shared Function ToConf(StringToConvert As String) As String
        Const EscapeChracter As String = "\"

        ' Remove slashes
        Dim ChractersToUnescape As New List(Of String) From {
            Chr(34)
        }

        For Each ChracterToUnescape As String In ChractersToUnescape
            StringToConvert = StringToConvert.Replace(EscapeChracter & ChracterToUnescape, ChracterToUnescape)
        Next

        ' Add slashes
        Dim ChractersToEscape As New List(Of String) From {
            "'"
        }

        For Each ChracterToEscape As String In ChractersToEscape
            StringToConvert = StringToConvert.Replace(ChracterToEscape, EscapeChracter & ChracterToEscape)
        Next

        Return StringToConvert
    End Function
#End Region

#Region "Public"
    Public Property Context As String
    Public Property ID As String
    Public Property Translation As String = ""
#End Region

End Class
