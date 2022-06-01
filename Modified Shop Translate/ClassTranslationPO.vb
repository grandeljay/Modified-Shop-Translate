Imports System.Net
Imports System.Text.RegularExpressions

Public Class ClassTranslationPO
    Public Const REGEX_CONTEXT_TO_KEY As String = "^\[([a-z_]+)\] ([a-zA-Z_0-9]+)$"

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

        If LineToWrite.Contains("\n") AndAlso Prefix <> StringType.MSGCTXT Then
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

    Public Shared Function FromPO(StringToConvert As String) As String
        Const EscapeChracter As String = "\"

        ' Remove slashes
        Dim ChractersToUnescape As New List(Of String) From {
            Chr(34)
        }

        For Each ChracterToUnescape As String In ChractersToUnescape
            StringToConvert = StringToConvert.Replace(EscapeChracter & ChracterToUnescape, ChracterToUnescape)
        Next

        Return StringToConvert
    End Function

    ''' <summary>
    ''' Prepares the input string by escaping and unescaping values as well as decoding html entities.
    ''' </summary>
    ''' <param name="StringToConvert">The input string to manipulate.</param>
    ''' <returns>String</returns>
    Public Shared Function ToPo(StringToConvert As String) As String
        Const EscapeChracter As String = "\"

        ' Convert HTML entities      
        StringToConvert = WebUtility.HtmlDecode(StringToConvert)

        ' Remove slashes
        Dim ChractersToUnescape As New List(Of String) From {
            Chr(34),
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

        Return StringToConvert
    End Function

    Public Shared Function FromConf(StringToConvert As String) As String
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
            Chr(34),
            "'"
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

    Public Shared Function FromDefine(StringToConvert As String) As String
        Return StringToConvert
    End Function

    Public Shared Function ToDefine(StringToConvert As String) As String
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
            "'"
        }

        For Each ChracterToEscape As String In ChractersToEscape
            StringToConvert = StringToConvert.Replace(ChracterToEscape, EscapeChracter & ChracterToEscape)
        Next

        Return StringToConvert
    End Function

    Public Shared Function GetTranslation(TextToTranslate As String, Optional Context As String = Nothing) As String
        Dim TextToTranslatePO As String = WebUtility.HtmlDecode(TextToTranslate)

        For Each TargetPO As ClassTranslationPO In FormMain.Settings.LanguageTarget.TranslationsPO
            If Context Is Nothing Then
                If TextToTranslatePO = TargetPO.ID Then
                    Return TargetPO.Translation
                End If
            Else
                If TextToTranslatePO = TargetPO.ID AndAlso Context = TargetPO.Context Then
                    Return TargetPO.Translation
                End If
            End If
        Next

        Return ""
    End Function
#End Region

#Region "Public"
    Public Property Context As String
    Public Property ID As String
    Public Property Translation As String = ""

    Public Function IsFromConf() As Boolean
        Dim RegexConf As New Regex(REGEX_CONTEXT_TO_KEY)
        Dim MatchConf As Match = RegexConf.Match(Me.Context)

        Return MatchConf.Success
    End Function

    Public Function IsFromDefine() As Boolean
        Dim RegexDefine As New Regex("^[A-Z_]+$")
        Dim MatchDefine As Match = RegexDefine.Match(Me.Context)

        Return MatchDefine.Success
    End Function

    Public Function GetSection() As String
        Dim Section As String = ""

        Dim RegexSection As New Regex("^\[([a-z_]+)\]")
        Dim MatchSection As Match = RegexSection.Match(Me.Context)

        If MatchSection.Success Then
            Section = MatchSection.Groups(1).Value
        End If

        Return Section
    End Function

    Public Function GetKey() As String
        Dim Key As String = ""

        Dim RegexSection As New Regex(REGEX_CONTEXT_TO_KEY)
        Dim MatchSection As Match = RegexSection.Match(Me.Context)

        If MatchSection.Success Then
            Key = MatchSection.Groups(2).Value
        End If

        Return Key
    End Function
#End Region

End Class
