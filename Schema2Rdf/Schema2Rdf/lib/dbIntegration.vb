Imports System.Xml
Public Class dbIntegration
    Private conversionFilePath As String = "../../conversion.xml"
    Private fields As New List(Of conversionItem)

    Public Sub New(ByVal filePath As String)
        Me.conversionFilePath = filePath
        If Me.fields Is Nothing Then
            Me.fields = New List(Of conversionItem)
        End If
    End Sub

    Public Sub loadCSVFile()
        Try

            Dim sr As New IO.StreamReader(Me.conversionFilePath)
            Dim line As String
            Dim pts() As String
            While Not sr.EndOfStream
                line = sr.ReadLine()
                pts = line.Split(",")
                Me.addItem(pts(0), pts(1), pts(2), pts(3))
            End While


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub addItem(ByVal schema As String, ByVal table As String, ByVal fieldName As String, ByVal newFieldName As String)
        Me.fields.Add(New conversionItem(schema, table, fieldName, newFieldName))
    End Sub
End Class
