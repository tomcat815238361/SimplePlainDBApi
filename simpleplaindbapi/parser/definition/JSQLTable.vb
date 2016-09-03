

Public Class JSQLTable

    Private _tablename As String
    Private _aliastablename As String


    Public Property TableName() As String
        Get
            Return _tablename
        End Get
        Set(ByVal value As String)
            _tablename = value
        End Set
    End Property

    Public Property AliasTableName() As String
        Get
            Return _aliastablename
        End Get
        Set(ByVal value As String)
            _aliastablename = value
        End Set
    End Property

End Class
