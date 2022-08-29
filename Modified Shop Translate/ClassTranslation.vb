Imports DeepL

Public Class ClassTranslation
    Public Shared Function GetTranslationForPO(TextToTranslate As String, Optional Context As String = Nothing) As String
        If TextToTranslate = "" Then
            Return ""
        End If

        '' Search Conf
        Dim TranslationConf As String = ClassTranslationPO.FromConf(ClassTranslationConf.GetTranslation(TextToTranslate, Context))

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        '' Search Define
        Dim TranslationDefine As String = ClassTranslationPO.FromDefine(ClassTranslationDefine.GetTranslation(TextToTranslate, Context))

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        '' Search PO
        Dim TranslationPO As String = ClassTranslationPO.FromPO(ClassTranslationPO.GetTranslation(TextToTranslate, Context))

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        '' Search Conf (without context)
        Dim TranslationConfAlt As String = ClassTranslationConf.GetTranslation(TextToTranslate)

        If TranslationConfAlt <> "" Then
            Return TranslationConfAlt
        End If

        '' Search Defines (without context)
        Dim TranslationDefineAlt As String = ClassTranslationDefine.GetTranslation(TextToTranslate)

        If TranslationDefineAlt <> "" Then
            Return TranslationDefineAlt
        End If

        '' Search PO (without context)
        Dim TranslationPOAlt As String = ClassTranslationPO.GetTranslation(TextToTranslate)

        If TranslationPOAlt <> "" Then
            Return TranslationPOAlt
        End If

        ' 3. Translate via DeepL
        '
        Using Client As DeepLClient = New DeepLClient(FormMain.Settings.DeepLAPIKey, useFreeApi:=True)
            Try
                Dim Translation As Translation = Client.TranslateAsync(
                    TextToTranslate,
                    FormMain.Settings.LanguageSource.DeepLLanguage,
                    FormMain.Settings.LanguageTarget.DeepLLanguage,
                    DeepL.Splitting.None,
                    True
                ).Result

                Return Translation.Text
            Catch exception As DeepLException
                Console.WriteLine($"An error occurred: {exception.Message}")
            End Try
        End Using

        ' 4. Fallback to Source
        '
        Return TextToTranslate
    End Function

    Public Shared Function GetTranslationForDefine(TextToTranslate As String, Optional Context As String = Nothing) As String
        If TextToTranslate = "" Then
            Return ""
        End If

        ' 1. Search with context
        '
        '' Search PO
        Dim TranslationPO As String = ClassTranslationPO.FromPO(ClassTranslationPO.GetTranslation(TextToTranslate, Context))

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        '' Search Conf
        Dim TranslationConf As String = ClassTranslationPO.FromConf(ClassTranslationConf.GetTranslation(TextToTranslate, Context))

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        '' Search Define
        Dim TranslationDefine As String = ClassTranslationPO.FromDefine(ClassTranslationDefine.GetTranslation(TextToTranslate, Context))

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        ' 2. Search without context
        '
        '' Search PO (without context)
        Dim TranslationPOAlt As String = ClassTranslationPO.GetTranslation(TextToTranslate)

        If TranslationPOAlt <> "" Then
            Return TranslationPOAlt
        End If

        '' Search Conf (without context)
        Dim TranslationConfAlt As String = ClassTranslationConf.GetTranslation(TextToTranslate)

        If TranslationConfAlt <> "" Then
            Return TranslationConfAlt
        End If

        '' Search Defines (without context)
        Dim TranslationDefineAlt As String = ClassTranslationDefine.GetTranslation(TextToTranslate)

        If TranslationDefineAlt <> "" Then
            Return TranslationDefineAlt
        End If

        ' 3. Translate via DeepL
        '
        Using Client As DeepLClient = New DeepLClient(FormMain.Settings.DeepLAPIKey, useFreeApi:=True)
            Try
                Dim Translation As Translation = Client.TranslateAsync(
                    TextToTranslate,
                    FormMain.Settings.LanguageSource.DeepLLanguage,
                    FormMain.Settings.LanguageTarget.DeepLLanguage,
                    DeepL.Splitting.None,
                    True
                ).Result

                Return Translation.Text
            Catch exception As DeepLException
                Console.WriteLine($"An error occurred: {exception.Message}")
            End Try
        End Using

        ' 4. Fallback to Source
        '
        Return TextToTranslate
    End Function

    Public Shared Function GetTranslationForConf(TextToTranslate As String, Optional Context As String = Nothing) As String
        If TextToTranslate = "" Then
            Return ""
        End If

        ' 1. Search with context
        '
        '' Search PO
        Dim TranslationPO As String = ClassTranslationPO.FromPO(ClassTranslationPO.GetTranslation(TextToTranslate, Context))

        If TranslationPO <> "" Then
            Return TranslationPO
        End If

        '' Search Define
        Dim TranslationDefine As String = ClassTranslationPO.FromDefine(ClassTranslationDefine.GetTranslation(TextToTranslate, Context))

        If TranslationDefine <> "" Then
            Return TranslationDefine
        End If

        '' Search Conf
        Dim TranslationConf As String = ClassTranslationPO.FromConf(ClassTranslationConf.GetTranslation(TextToTranslate, Context))

        If TranslationConf <> "" Then
            Return TranslationConf
        End If

        ' 2. Search without context
        '
        '' Search PO (without context)
        Dim TranslationPOAlt As String = ClassTranslationPO.GetTranslation(TextToTranslate)

        If TranslationPOAlt <> "" Then
            Return TranslationPOAlt
        End If

        '' Search Defines (without context)
        Dim TranslationDefineAlt As String = ClassTranslationDefine.GetTranslation(TextToTranslate)

        If TranslationDefineAlt <> "" Then
            Return TranslationDefineAlt
        End If

        '' Search Conf (without context)
        Dim TranslationConfAlt As String = ClassTranslationConf.GetTranslation(TextToTranslate)

        If TranslationConfAlt <> "" Then
            Return TranslationConfAlt
        End If

        ' 3. Translate via DeepL
        '
        Using Client As DeepLClient = New DeepLClient(FormMain.Settings.DeepLAPIKey, useFreeApi:=True)
            Try
                Dim Translation As Translation = Client.TranslateAsync(
                    TextToTranslate,
                    FormMain.Settings.LanguageSource.DeepLLanguage,
                    FormMain.Settings.LanguageTarget.DeepLLanguage,
                    DeepL.Splitting.None,
                    True
                ).Result

                Return Translation.Text
            Catch exception As DeepLException
                Console.WriteLine($"An error occurred: {exception.Message}")
            End Try
        End Using

        ' 4. Fallback to Source
        '
        Return TextToTranslate
    End Function

End Class
