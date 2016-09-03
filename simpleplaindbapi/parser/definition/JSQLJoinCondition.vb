Public Class JSQLJoinCondition

    Private _tableA As String
    Private _columnA As String

    Private _tableB As String
    Private _ColumnB As String

    Private _condition As String

    Public ReadOnly Property TableColumnName(ByVal tableName As String) As String
        Get
            Return IIf(tableName = _tableA, _tableA + "." + _columnA, _tableB + "." + _ColumnB)
        End Get
    End Property

    Public ReadOnly Property ColumnName(ByVal tableName As String) As String
        Get
            Return IIf(tableName = _tableA, _columnA, _ColumnB)
        End Get
    End Property

    Public Property TableA() As String
        Get
            Return _tableA
        End Get
        Set(ByVal value As String)
            _tableA = value
        End Set
    End Property

    Public Property ColumnA() As String
        Get
            Return _columnA
        End Get
        Set(ByVal value As String)
            _columnA = value
        End Set
    End Property

    Public Property TableB() As String
        Get
            Return _tableB
        End Get
        Set(ByVal value As String)
            _tableB = value
        End Set
    End Property

    Public Property ColumnB() As String
        Get
            Return _ColumnB
        End Get
        Set(ByVal value As String)
            _ColumnB = value
        End Set
    End Property

    Public Property Condition() As String
        Get
            Return _condition
        End Get
        Set(ByVal value As String)
            _condition = value
        End Set
    End Property
End Class
