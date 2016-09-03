
Public Class JSQLTableColumn
    Private _columnname As String
    Private _aliascolumnname As String

    Public Property ColumnName() As String
        Get
            Return _columnname
        End Get
        Set(ByVal value As String)
            _columnname = value
        End Set
    End Property

    Public Property AliasColumnName() As String
        Get
            Return _aliascolumnname
        End Get
        Set(ByVal value As String)
            _aliascolumnname = value
        End Set
    End Property

    Public ReadOnly Property RealColumnName() As String
        Get
            Return _columnname.Substring(_columnname.IndexOf(".") + 1)
        End Get
    End Property

End Class



