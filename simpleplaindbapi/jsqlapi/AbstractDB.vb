Public MustInherit Class AbstractDB
    Protected tbns() As String

    Protected tbs() As DataTable

    Protected stbns() As String

    Private _dbPath As String

    Public Overridable ReadOnly Property PTbns() As String()
        Get
            Return tbns
        End Get
    End Property

    Public Overridable ReadOnly Property PTbs() As DataTable()
        Get
            Return tbs
        End Get
    End Property

    Public Overridable ReadOnly Property SPTbns() As String()
        Get
            Return stbns
        End Get
    End Property


    Protected ReadOnly Property DB() As String
        Get
            Return _dbPath
        End Get
    End Property

    Public Sub New(_tdb As String)
        _dbPath = _tdb
    End Sub

    Public MustOverride Function GetTableData(ByVal tn As String) As DataTable

    Public MustOverride Function GetTableNums() As Integer

    Public MustOverride Function GetTables() As String()

    Public MustOverride Function GetColumns(ByVal tn As String) As String()
End Class
