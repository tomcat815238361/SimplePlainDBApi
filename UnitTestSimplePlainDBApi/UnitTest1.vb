Imports SimplePlainDBApi
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class UnitTest

    <TestMethod()> Public Sub TestMethod()
        Dim jsa As JSQLApi = JSQLApi.CreateJSQLApi("F:\DB", DBType.TXT)
        'Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P select *")
        Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P select P.NAME,P.AGE")
    End Sub

    <TestMethod()> Public Sub TestMethod1()
        Dim re As New Regex("(AND|OR)")
        Dim str As String = "WHERE XXX>10 AND XXX<10 OR YYY>5"
        Dim strS() As String = re.Split(str)
        Dim split() As String = {"AND", "OR"}
        Dim str1() As String = str.Split(split, StringSplitOptions.RemoveEmptyEntries)
    End Sub

    <TestMethod()> Public Sub TestTowTable()
        Dim jsa As JSQLApi = JSQLApi.CreateJSQLApi("F:\DB", DBType.TXT)
        'Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P select *")
        Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P,Teacher T select * Join P.NAME=T.NAME")
    End Sub

    <TestMethod()> Public Sub TestThreeTable()
        Dim jsa As JSQLApi = JSQLApi.CreateJSQLApi("F:\DB", DBType.TXT)
        'Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P select *")
        Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P,Teacher T,Book B select P.NAME NAME,B.BOOK BOOK Join P.NAME=T.NAME AND T.AGE = B.AGE")
    End Sub

    <TestMethod()> Public Sub TestThreeTableWhere()
        Dim jsa As JSQLApi = JSQLApi.CreateJSQLApi("F:\DB", DBType.TXT)
        'Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P select *")
        'wrong where 
        'Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P,Teacher T,Book B select P.NAME NAME,B.BOOK BOOK where NAME>80 AND BOOK='Runner' JOIN P.NAME=T.NAME AND T.AGE = B.AGE")
        'current where
        Dim dt As DataTable = jsa.QueryWithJSQL("from  Person  P,Teacher T,Book B select P.NAME NAME,B.BOOK BOOK where PERSON.NAME='jack' AND BOOK.BOOK='Runner' JOIN P.NAME=T.NAME AND T.AGE = B.AGE")
    End Sub
End Class