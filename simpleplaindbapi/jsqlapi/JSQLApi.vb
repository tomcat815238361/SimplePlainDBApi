
Public Enum DBType
    TXT
    CSV
End Enum

Public Class JSQLApi

    Private Shared _jsa As JSQLApi = Nothing
    Private Shared lock As Object = New Object()

    Private ddb As AbstractDB

    Private Shared sqltoJSELECTSQLResult As New KeyValue(Of DataTable)

    Private Sub New(ByVal db As String, ByVal dbType As DBType)
        If dbType = DBType.TXT Then
            ddb = New TxtDB(db)
        End If
        If dbType = DBType.CSV Then
            ddb = New CsvDB(db)
        End If
    End Sub

    Public Shared Function CreateJSQLApi(ByVal db As String, ByVal dbType As DBType) As JSQLApi
        Dim t As JSQLApi = Nothing

        If _jsa Is Nothing Then
            SyncLock lock
                If _jsa Is Nothing Then
                    _jsa = New JSQLApi(db, dbType)
                    t = _jsa
                End If
            End SyncLock
        End If

        Return t

    End Function

    Public Function QueryWithJSQL(ByVal jsql As String) As DataTable

        Dim rrDt As DataTable

        Dim jsc As JSELECTSQLContext = JSQLSELECTParser.ParserSQLContext(jsql)

        Dim obj As Object = sqltoJSELECTSQLResult.Item(jsc.JSQL)

        If Not (sqltoJSELECTSQLResult.Item(jsc.JSQL) Is Nothing) Then
            Return CType(obj, DataTable)
        End If

        For Each c As JSQLTable In jsc.Tables

            If Not checkValueIsInAr(c.TableName, ddb.PTbns) Then
                Throw New DBException(c.TableName + " TABLE NOT EXISTS")
            End If

        Next


        If jsc.Tables.Count = 1 Then

            Dim rdt As DataTable

            rdt = ddb.GetTableData(CType(jsc.Tables(0), JSQLTable).TableName)

            If Not （rdt Is Nothing） AndAlso rdt.Rows.Count > 0 Then
                If Not (jsc.Where Is Nothing) And jsc.Where <> "" Then
                    Dim temp As DataTable
                    temp = rdt.Clone
                    Dim tR As DataRow
                    For Each c As DataRow In rdt.Select(jsc.Where)
                        tR = temp.NewRow
                        tR.ItemArray = c.ItemArray
                    Next
                    rdt = temp
                End If
            End If

            If jsc.AllColumns = True Then
                rrDt = rdt
            Else
                Dim rcolumns() As String = ddb.GetColumns(CType(jsc.Tables(0), JSQLTable).TableName)

                Dim jccolumns() As String = CType(jsc.TbWithColumns.Item(CType(jsc.Tables(0), JSQLTable).AliasTableName), List(Of String)).ToArray

                For Each cl As String In jccolumns

                    If Not JSQLCommonUtils.checkValueIsInAr(cl, rcolumns) Then
                        Throw New DBException(cl + " COLUMN NOT EXISTS IN " + CType(jsc.Tables(0), JSQLTable).TableName + " TABLE")
                    End If

                Next

                Dim dtc As DataColumn

                For Each cl As String In rcolumns
                    If Not JSQLCommonUtils.checkValueIsInAr(cl, jccolumns) Then
                        rdt.Columns.Remove(cl)
                    End If
                Next

                For Each cl As JSQLTableColumn In jsc.Columns
                    dtc = rdt.Columns(cl.RealColumnName)
                    If cl.AliasColumnName Is Nothing Or cl.AliasColumnName = "" Then
                        dtc.ColumnName = cl.ColumnName
                    Else
                        dtc.ColumnName = cl.AliasColumnName
                    End If
                Next

                rrDt = rdt
            End If
        Else
            '
            Dim jstc As JSQLTableColumn
            Dim dtAll As DataTable = RecursionDataTable(jsc, jsc.Tables)

            Dim toBeRemColummn As New ArrayList

            If Not （dtAll Is Nothing） AndAlso dtAll.Rows.Count > 0 Then
                If Not (jsc.Where Is Nothing) And jsc.Where <> "" Then
                    Dim temp As DataTable
                    temp = dtAll.Clone
                    Dim tR As DataRow
                    For Each c As DataRow In dtAll.Select(jsc.Where)
                        tR = temp.NewRow
                        tR.ItemArray = c.ItemArray
                    Next
                    dtAll = temp
                End If
            End If

            If jsc.AllColumns Then
                rrDt = dtAll
            Else
                For Each c As DataColumn In dtAll.Columns
                    jstc = jsc.GetJSQLTableColumnByName(c.ColumnName)
                    If Not （jstc Is Nothing） Then
                        If jstc.AliasColumnName <> "" Then
                            c.ColumnName = jstc.AliasColumnName
                        End If
                    Else
                        toBeRemColummn.Add(c)
                    End If
                Next

                For Each c As DataColumn In toBeRemColummn
                    dtAll.Columns.Remove(c)
                Next

                rrDt = dtAll
            End If
        End If

        SyncLock sqltoJSELECTSQLResult
            sqltoJSELECTSQLResult.Add(jsc.JSQL, rrDt)
        End SyncLock

        Return rrDt
    End Function

    Private Function RecursionDataTable(ByVal jsc As JSELECTSQLContext, ParamArray dt() As JSQLTable) As DataTable
        If dt.Length = 2 Then
            Dim temp As DataTable = New DataTable()
            Dim tColumn As DataColumn

            Dim jtc As JSQLJoinCondition = jsc.ChooseJoinCondtion(dt(0), dt(1))
            Dim t1 As DataTable = ddb.GetTableData(jtc.TableA)
            Dim t2 As DataTable = ddb.GetTableData(jtc.TableB)

            For Each c1 As DataColumn In t1.Columns
                tColumn = New DataColumn
                tColumn.ColumnName = t1.TableName + "." + c1.ColumnName
                temp.Columns.Add(tColumn)
            Next

            For Each c2 As DataColumn In t2.Columns
                tColumn = New DataColumn
                tColumn.ColumnName = t2.TableName + "." + c2.ColumnName
                temp.Columns.Add(tColumn)
            Next

            Dim temp1 As String
            Dim temp2 As String

            Dim nr As DataRow

            For Each r1 As DataRow In t1.Rows

                For Each r2 As DataRow In t2.Rows
                    '以后可以考虑增加数据类型转换接口
                    temp1 = r1(jtc.ColumnA)
                    temp2 = r2（jtc.ColumnB)
                    If jtc.Condition = "=" Then
                        If temp1 = temp2 Then
                            nr = temp.NewRow()
                            nr.ItemArray = r1.ItemArray.Concat(r2.ItemArray).ToArray
                            temp.Rows.Add(nr)
                        End If
                    ElseIf jtc.Condition = "<>" Then
                        If temp1 <> temp2 Then
                            nr = temp.NewRow()
                            nr.ItemArray = r1.ItemArray.Concat(r2.ItemArray).ToArray
                            temp.Rows.Add(nr)
                        End If
                    Else
                        Throw New DBException("JSQLApi.RecursionDataTable:UNVALID JOIN OPER " + jtc.Condition)
                    End If
                Next

            Next

            Return temp
        Else

            Dim nr As DataRow

            Dim temp As DataTable = New DataTable()

            Dim tColumn As DataColumn

            Dim dtLast As DataTable = ddb.GetTableData(dt(dt.Length - 1).TableName)
            Dim dtPre As DataTable = RecursionDataTable(jsc, SubArray(dt, 0, dt.Length - 1))

            Dim jtc As JSQLJoinCondition = jsc.ChooseJoinCondtion(dt(dt.Length - 2), dt(dt.Length - 1))

            For Each c2 As DataColumn In dtPre.Columns
                tColumn = New DataColumn
                tColumn.ColumnName = c2.ColumnName
                temp.Columns.Add(tColumn)
            Next

            For Each c1 As DataColumn In dtLast.Columns
                tColumn = New DataColumn
                tColumn.ColumnName = dtLast.TableName + "." + c1.ColumnName
                temp.Columns.Add(tColumn)
            Next

            Dim temp1 As String
            Dim temp2 As String

            For Each r1 As DataRow In dtPre.Rows

                For Each r2 As DataRow In dtLast.Rows
                    '以后可以考虑增加数据类型转换接口
                    temp1 = r1(jtc.TableColumnName(dt(dt.Length - 2).TableName))
                    temp2 = r2（jtc.ColumnName(dt(dt.Length - 1).TableName))
                    If jtc.Condition = "=" Then
                        If temp1 = temp2 Then
                            nr = temp.NewRow()
                            nr.ItemArray = r1.ItemArray.Concat(r2.ItemArray).ToArray
                            temp.Rows.Add(nr)
                        End If
                    ElseIf jtc.Condition = "<>" Then
                        If temp1 <> temp2 Then
                            nr = temp.NewRow()
                            nr.ItemArray = r1.ItemArray.Concat(r2.ItemArray).ToArray
                            temp.Rows.Add(nr)
                        End If
                    Else
                        Throw New DBException("JSQLApi.RecursionDataTable:UNVALID JOIN OPER " + jtc.Condition)
                    End If
                Next

            Next

            Return temp

        End If
        Return Nothing

    End Function
End Class
