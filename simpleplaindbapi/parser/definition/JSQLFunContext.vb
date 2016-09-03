

Public MustInherit Class JSQLFunContext

    Private _funname As String

    Private _jsc As JSELECTSQLContext

    Private _tbtotbcol As New Hashtable

    Public ReadOnly Property TbToTbCol(ByVal index As Integer) As Hashtable
        Get
            Return _tbtotbcol
        End Get

    End Property

    Public Property FunctionName() As String
        Get
            Return _funname
        End Get
        Set(ByVal Value As String)
            _funname = Value
        End Set
    End Property

    Public Sub JSQLFunContext(ByVal jsc As JSELECTSQLContext)

        _jsc = jsc

    End Sub

    Public Sub AddTbColRelation(ByVal tbName As String, ByVal colName As String)

    End Sub

End Class
