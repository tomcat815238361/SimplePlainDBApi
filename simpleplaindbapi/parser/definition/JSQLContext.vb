Public Class JSQLContext

    Private _jsql As String

    Private _tables() As JSQLTable

    Private _columns() As JSQLTableColumn

    Private _allColumns As Boolean

    Private _tbwithcolumns As New KeyValue(Of List(Of String))

    Public Property JSQL() As String
        Get
            Return _jsql
        End Get
        Set(ByVal value As String)
            _jsql = value
        End Set
    End Property

    Public Property AllColumns() As Boolean
        Get
            Return _allColumns
        End Get
        Set(ByVal value As Boolean)
            _allColumns = value
        End Set
    End Property

    Public Property Tables As JSQLTable()
        Get
            Return _tables
        End Get
        Set(ByVal value() As JSQLTable)
            _tables = value
        End Set
    End Property

    Public Property Columns As JSQLTableColumn()
        Get
            Return _columns
        End Get
        Set(ByVal value() As JSQLTableColumn)
            _columns = value
        End Set
    End Property

End Class
