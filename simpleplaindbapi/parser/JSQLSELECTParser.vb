Imports System.Collections
Imports System.Text.RegularExpressions


Public Class JSQLSELECTParser

    Private Const JSQLSELECT As String = "SELECT"
    Private Const JSQLFROM As String = "FROM"
    Private Const JSQLWHERE As String = "WHERE"
    Private Const JSQLJOIN As String = "JOIN"
    Private Const JSQLJOINON As String = "ON"

    Private Shared JSQLWHERE_SUPPORTCONDITONS() As String = {">", "<", ">=", "<=", "=", "<>", "LIKE", "IN"}
    Private Shared JSQLJOIN_SUPPORTCONDITONS() As String = {"=", "<>"}


    Private Shared sqltoJSELECTSQLResult As New KeyValue(Of JSQLContext)

    Shared Sub New()

        '后期的功能点,解析JSQL函数动态加载类来实现,要解析配置文件来加载相应的类

    End Sub

    Public Shared Function ParserSQLContext(ByVal JSQL As String) As JSELECTSQLContext

        '解析JSQL，比如将换行符替换为空格符,将JSQL转换成大写等,顺便提取出表字符串和列字符串
        Dim tableStr() As String
        Dim colStr() As String

        Dim whStr() As String
        Dim wStr As String

        Dim joinStr() As String

        Dim jS As String

        JSQL = ParsePlain(JSQL)

        Dim obj As Object = sqltoJSELECTSQLResult.Item(JSQL)

        If Not (sqltoJSELECTSQLResult.Item(JSQL) Is Nothing) Then
            Return CType(obj, JSELECTSQLContext)
        End If

        CheckJSQL(JSQL, tableStr, colStr, whStr, wStr, joinStr, jS)

        Dim jsc As JSELECTSQLContext = New JSELECTSQLContext()

        jsc.JSQL = JSQL

        ParseTable(jsc, tableStr)

        ParseColumn(jsc, colStr)

        If wStr <> "" Then
            ParseWhere(jsc, whStr, wStr)
        End If

        If jS <> "" Then
            ParseJoin(jsc, joinStr, jS)
        End If


        SyncLock sqltoJSELECTSQLResult
            sqltoJSELECTSQLResult.Add(JSQL, jsc)
        End SyncLock

        Return jsc

    End Function
    '以下方法暂时设置成public方便测试 ，测试ok就改为private

    Public Shared Function ParsePlain(ByVal JSQL As String) As String
        Dim rfs As New Regex("\s+")

        JSQL = Strings.Trim(JSQL).Replace(vbCrLf, " ").ToUpper

        Return rfs.Replace(JSQL, " ")

    End Function

    Public Shared Sub CheckJSQL(ByVal JSQL As String, ByRef tableStr() As String, ByRef colStr() As String,
                                ByRef whStr() As String, ByRef wStr As String, ByRef joinStr() As String, ByRef jS As String)
        '1.检查from表达式是否符合规范
        Dim rf As New Regex(JSQLFROM + "\s\w+")

        If Not JSQL.StartsWith(JSQLFROM) Or Not JSQL.Contains(JSQLFROM + " ") Or Not rf.IsMatch(JSQL) Then
            Throw New FormatException(JSQLFROM + " FORMAT ERROR!")
        End If

        '2.检查select表达式是否符合规范
        Dim rs As New Regex(JSQLSELECT + "\s[A-Za-z\*,\s\.]+")

        If JSQL.StartsWith(JSQLSELECT) Or Not JSQL.Contains(JSQLSELECT + " ") Or Not rs.IsMatch(JSQL) Then
            Throw New FormatException(JSQLSELECT + " FORMAT ERROR!")
        End If

        '3.检查FROM....SELECT....是否符合这个语句规则
        Dim rsr As New Regex("^" + JSQLFROM + "\s+[A-Za-z0-9,\s]+\s" + JSQLSELECT + "\s[A-Za-z0-9*.,\s]")
        If Not rsr.IsMatch(JSQL) Then
            Throw New FormatException("JSQL FORMAT ERROR!")
        End If
        '4.检查表是否有别名
        Dim si As Integer = Strings.InStr(JSQL, JSQLSELECT)
        Dim tablesStr As String = Strings.Mid(JSQL, JSQLFROM.Length + 2, si - JSQLFROM.Length - 3)

        tableStr = tablesStr.Split(",")

        Dim temp() As String

        For Each c As String In tableStr
            temp = c.Split(" ")
            If temp.Length = 1 Then
                Throw New FormatException("TABLENAME NO ALIASNAME " + tablesStr)
            End If
        Next

        '5.1 .检查列是否有别名前缀
        Dim wi As Integer = Strings.InStr(JSQL, JSQLWHERE)
        Dim ji As Integer = Strings.InStr(JSQL, JSQLJOIN)



        Dim colsStr As String

        If wi = 0 And ji = 0 Then
            colsStr = Strings.Mid(JSQL, si + JSQLSELECT.Length + 1)
        ElseIf wi = 0 And ji <> 0 Then
            colsStr = Strings.Mid(JSQL, si + JSQLSELECT.Length + 1, ji - si - JSQLSELECT.Length - 2)
        Else
            colsStr = Strings.Mid(JSQL, si + JSQLSELECT.Length + 1, wi - si - JSQLSELECT.Length - 2)
        End If

        colStr = colsStr.Split(",")

        Dim dr As New Regex("\w+\.{1}\w+")

        If colsStr <> "*" Then
            For Each c As String In colStr
                If Not dr.IsMatch(c) Then
                    Throw New FormatException("COLNAME MUST ADD THE TABLEALIAS WITH THE PREX " + colsStr)
                End If
            Next
        End If

        '6.如果有Where 检查Where表达式 .... and .... or .....
        ji = Strings.InStr(JSQL, JSQLJOIN)

        If JSQL.Contains(JSQLWHERE) Then

            If ji = 0 Then
                wStr = Strings.Mid(JSQL, wi + JSQLWHERE.Length + 1)
            Else
                wStr = Strings.Mid(JSQL, wi + JSQLWHERE.Length + 1, ji - wi - JSQLJOIN.Length - 3)
            End If

            Dim sp() As String = {" And ", " Or "}
            whStr = wStr.Split(sp, StringSplitOptions.RemoveEmptyEntries)

            If whStr.Length = 0 Then
                Throw New FormatException("WHERE MUST FORMAT COLUMN > VAL Or COLUMN <> VAL")
            End If

            For Each c As String In whStr
                temp = c.Split(" ")
                If temp.Length <> 3 Then
                    Throw New FormatException("WHERE MUST FORMAT CLOUMN >,<,=, <>,IN,LIKE) VALUE " + c)
                End If
            Next

        End If

        If JSQL.Contains(JSQLJOIN) Then

            jS = JSQL.Substring(ji + JSQLJOIN.Length)

            Dim sp = {"AND"}
            joinStr = jS.Split(sp, StringSplitOptions.RemoveEmptyEntries)

            If joinStr.Length = 0 Then
                Throw New FormatException("JOIN FORMAT  MUST LIKE xxx.xx=xxxx.xx AND xxx.x =xx.x")
            End If

            Dim spJoin() As String = {"=", "<>"}
            For Each c As String In joinStr
                temp = c.Split(spJoin, StringSplitOptions.RemoveEmptyEntries)
                If Not c.Contains("=") And Not c.Contains("<>") Then
                    Throw New FormatException("JOIN FORMAT CONDITION MUST BE IN " + JSQLJOIN_SUPPORTCONDITONS.ToString)
                End If
            Next
        End If
    End Sub

    Public Shared Sub ParseTable(ByVal jsc As JSELECTSQLContext, ByVal tableStr() As String)

        Dim tables(tableStr.Length - 1) As JSQLTable

        Dim temp() As String
        For i As Integer = 0 To tableStr.Length - 1
            Dim jst As New JSQLTable()
            temp = tableStr(i).Split(" ")
            jst.TableName = temp(0)
            jst.AliasTableName = temp(1)
            tables(i) = jst
        Next

        jsc.Tables = tables

    End Sub

    Public Shared Sub ParseColumn(ByVal jsc As JSELECTSQLContext, ByVal colStr() As String)

        Dim columns(colStr.Length - 1) As JSQLTableColumn
        Dim temp() As String

        If colStr.Length = 1 And colStr(0) = "*" Then
            jsc.AllColumns = True
        Else
            For i As Integer = 0 To colStr.Length - 1
                Dim jscc As New JSQLTableColumn()
                temp = colStr(i).Split(" ")
                If temp.Length = 1 Then
                    jscc.ColumnName = temp(0)
                Else
                    jscc.ColumnName = temp(0)
                    jscc.AliasColumnName = temp(1)
                End If

                columns(i) = jscc
            Next

            jsc.Columns = columns
        End If

        jsc.InitTbWithColumns()
    End Sub

    Public Shared Sub ParseWhere(ByVal jsc As JSELECTSQLContext, ByVal whStr() As String, ByVal wStr As String)
        '利用DataTable where

        jsc.Where = wStr

    End Sub

    Public Shared Sub ParseJoin(ByVal jsc As JSELECTSQLContext, ByVal joinStr() As String, ByVal jS As String)
        Dim jtc As JSQLJoinCondition

        Dim rgx As New Regex("(<>)|(=)")

        Dim temp() As String

        For Each c As String In joinStr
            jtc = New JSQLJoinCondition()
            temp = rgx.Split(c)

            jtc.TableA = jsc.TbNameWithAliasName(temp(0).Split(".")(0).Trim())
            jtc.ColumnA = temp(0).Split(".")(1).Trim()

            jtc.Condition = temp(1)

            jtc.TableB = jsc.TbNameWithAliasName(temp(2).Split(".")(0).Trim())
            jtc.ColumnB = temp(2).Split(".")(1).Trim()

            jsc.AddJoinConditions(jtc)
        Next
    End Sub

End Class