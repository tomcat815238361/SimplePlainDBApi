

Public Class JSELECTSQLContext : Inherits JSQLContext

    Private _tbwithcolumns As New KeyValue(Of List(Of String))

    Private _joinCondtions As New List(Of JSQLJoinCondition)

    Private _strWhere As String

    Public ReadOnly Property TbNameWithAliasName(ByVal aliasName As String)
        Get
            For Each c As JSQLTable In Tables
                If c.AliasTableName = aliasName Then
                    Return c.TableName
                End If
            Next
            Return Nothing
        End Get
    End Property
    Public Property Where() As String
        Get
            Return _strWhere
        End Get
        Set(value As String)
            _strWhere = value
        End Set
    End Property

    Public ReadOnly Property TbWithColumns() As KeyValue(Of List(Of String))
        Get
            Return _tbwithcolumns
        End Get
    End Property

    Public ReadOnly Property JoinCondtions() As IList(Of JSQLJoinCondition)
        Get
            Return _joinCondtions
        End Get
    End Property

    Public Sub AddJoinConditions(ByVal jtc As JSQLJoinCondition)
        _joinCondtions.Add(jtc)
    End Sub

    Public Sub InitTbWithColumns()

        If Not (Columns Is Nothing) And Not AllColumns Then
            For Each column As JSQLTableColumn In Columns

                Dim temp() As String = column.ColumnName.Split(".")
                If _tbwithcolumns.Item(temp(0)) Is Nothing Then
                    Dim al As New List(Of String)
                    al.Add(temp(1))
                    _tbwithcolumns.Add(temp(0), al)
                Else
                    CType(_tbwithcolumns.Item(temp(0)), List(Of String)).Add(temp(1))
                End If
            Next
        End If

    End Sub

    Public Function ChooseJoinCondtion(ByVal t1 As JSQLTable, ByVal t2 As JSQLTable) As JSQLJoinCondition

        For Each j As JSQLJoinCondition In JoinCondtions
            If (j.TableA = t1.TableName And j.TableB = t2.TableName) Or (j.TableA = t2.TableName And j.TableB = t1.TableName) Then
                Return j
            End If
        Next

        Return Nothing

    End Function

    Public Function GetJSQLTableColumnByName(ByVal cn As String) As JSQLTableColumn
        If Not AllColumns Then
            For Each c As JSQLTableColumn In Columns
                If TbNameWithAliasName((c.ColumnName).Split(".")(0)) + "." + (c.ColumnName).Split(".")(1) = cn Then
                    Return c
                End If
            Next
        End If
        Return Nothing
    End Function
End Class

