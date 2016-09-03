Imports System.Data
Imports System.IO

Public Class TxtDB : Inherits AbstractDB

    Private Shared regexSplit As String = vbTab

    Public Sub New(ByVal _dbPath)

        MyBase.New(_dbPath)

        If Not Directory.Exists(_dbPath) Then
            Throw New DBException(_dbPath + " DB NOT EXISTS")
        End If

        tbns = Directory.GetFiles(_dbPath)

        stbns = GetSimpleTbns(tbns, DBType.TXT)

        If tbns.Length = 0 Then
            Throw New DBException("THE DB NO HAVE TABLES")
        End If

        Dim columns As String
        Dim temp() As String
        Dim column As DataColumn

        Dim ttbs(tbns.Length - 1) As DataTable

        For i As Integer = 0 To tbns.Length - 1
            Dim dt As New DataTable(SPTbns(i))

            Using sr As StreamReader = New StreamReader(tbns(i), FileMode.Open)
                columns = sr.ReadLine()

                temp = columns.Split(regexSplit)

                For Each c As String In temp
                    column = New DataColumn(c, Type.GetType("System.String"))
                    dt.Columns.Add(column)
                Next

                columns = sr.ReadLine()
                While columns <> Nothing
                    dt.Rows.Add(columns.Split(regexSplit))
                    columns = sr.ReadLine()
                End While

            End Using

            ttbs(i) = dt
        Next

        tbs = ttbs
    End Sub

    Public Overrides ReadOnly Property PTbns() As String()
        Get
            Return GetSimpleTbns(tbns, DBType.TXT)
        End Get
    End Property

    Public Overrides ReadOnly Property PTbs() As DataTable()
        Get
            Return tbs
        End Get
    End Property

    Public Overrides Function GetTableData(tn As String) As DataTable
        For Each dt As DataTable In tbs
            If dt.TableName = tn Then
                Return dt
            End If
        Next

        Return Nothing

    End Function

    Public Overrides Function GetTableNums() As Integer
        Return tbns.Length
    End Function

    Public Overrides Function GetTables() As String()
        Return tbns
    End Function

    Public Overrides Function GetColumns(tn As String) As String()

        Dim dt As DataTable = GetTableData(tn)

        Dim colNames(dt.Columns.Count - 1) As String

        For i As Integer = 0 To dt.Columns.Count - 1
            colNames(i) = dt.Columns.Item(i).ColumnName
        Next

        Return colNames

    End Function
End Class
