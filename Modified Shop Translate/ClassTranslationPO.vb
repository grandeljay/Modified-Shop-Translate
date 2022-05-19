Imports System.Net

Public Class ClassTranslationPO

#Region "Static"
    Enum StringType As Integer
        NONE
        MSGCTXT
        MSGID
        MSGSTR
    End Enum
    Public Shared Function WriteLine(LineToWrite As String, Optional Prefix As StringType = StringType.NONE) As String
        Dim ManipulatedLine As String = ""

        If Not Prefix = StringType.NONE Then
            ManipulatedLine = Prefix.ToString.ToLower & " "
        End If

        If LineToWrite.Contains("\n") Then
            Dim Multilines As String()

            If LineToWrite.EndsWith("\n") Then
                Multilines = LineToWrite.Substring(0, LineToWrite.Length - 2).Split("\n")
            Else
                Multilines = LineToWrite.Split("\n")
            End If

            If Multilines.Length >= 2 Then
                ManipulatedLine &= Chr(34) & Chr(34) & vbCrLf
            End If

            For I = 0 To Multilines.Length - 1
                ManipulatedLine &= Chr(34) & Multilines(I) & "\n" & Chr(34)

                If Multilines.Length >= 2 AndAlso I < Multilines.Length - 1 Then
                    ManipulatedLine &= vbCrLf
                End If
            Next
        Else
            ManipulatedLine &= Chr(34) & LineToWrite & Chr(34)
        End If

        Return ManipulatedLine
    End Function

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
