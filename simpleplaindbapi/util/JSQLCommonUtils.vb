Public Module JSQLCommonUtils
    Public Function checkValueIsInAr(ByVal val As String, ByVal check() As String) As Boolean
        For Each c As String In check
            If c = val Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetSimpleTbns(ByRef tbs() As String, type As DBType) As String()

        Dim rtbs(tbs.Length - 1) As String

        Dim temp As String

        If type = DBType.TXT Then
            For i As Integer = 0 To tbs.Length - 1
                temp = tbs(i)
                rtbs(i) = temp.Substring(temp.LastIndexOf("\") + 1, temp.LastIndexOf(".") - temp.LastIndexOf("\") - 1).ToUpper()
            Next
            Return rtbs
        End If
        Return tbs
    End Function

    Public Function GetSimpleTbn(ByVal tbn As String, type As DBType) As String

        If type = DBType.TXT Then
            Return tbn.Substring(tbn.LastIndexOf("\") + 1, tbn.LastIndexOf(".") - tbn.LastIndexOf("\"))
        End If

        Return tbn

    End Function

    Public Function GetSpecVals(ByVal str As String, ByVal sp As String, ParamArray ByVal sep() As String) As String()
        Dim sepL As New List(Of String)
        Dim temp() As String = str.Split(sp)
        For Each c As String In temp
            If checkValueIsInAr(c, sep) Then
                sepL.Add(c)
            End If
        Next
        Return sepL.ToArray
    End Function

    Public Function SubArray(Of T)(ByVal dis() As T, ByVal startIndex As Integer, ByVal length As Integer) As T()
        Dim temps(length - 1) As T

        For i As Integer = startIndex To startIndex + length - 1
            temps(i - startIndex) = dis(i)
        Next

        Return temps

    End Function
End Module
