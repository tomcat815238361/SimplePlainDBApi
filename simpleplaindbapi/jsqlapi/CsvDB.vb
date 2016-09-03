Imports System.Data

Public Class CsvDB : Inherits AbstractDB

    Public Sub New(ByVal _dbPath)
        MyBase.New(_dbPath)
        '.........
    End Sub

    Public Overrides Function GetColumns(tn As String) As String()
        Return Nothing
    End Function

    Public Overrides Function GetTableData(tn As String) As System.Data.DataTable
        Return Nothing
    End Function

    Public Overrides Function GetTableNums() As Integer
        Return Nothing
    End Function

    Public Overrides Function GetTables() As String()
        Return Nothing
    End Function
End Class
