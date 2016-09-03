Imports System.Collections
Imports System.Collections.Generic

Public Class KeyValue(Of V)
    Private keys As New List(Of String)

    Private values As New List(Of V)

    Public Sub Add(ByVal key As String, ByVal val As V)

        For i As Integer = 0 To keys.Count - 1
            If keys.Item(i) = key Then
                keys.RemoveAt(i)
                values.RemoveAt(i)
                Exit For
            End If
        Next

        keys.Add(key)
        values.Add(val)

    End Sub

    Public Function Item(index As Integer) As V
        Return values.Item(index)
    End Function

    Public Function Item(key As String) As V

        For i As Integer = 0 To keys.Count - 1
            If keys.Item(i) = key Then
                Return values.Item(i)
            End If
        Next

        Return Nothing

    End Function

End Class
