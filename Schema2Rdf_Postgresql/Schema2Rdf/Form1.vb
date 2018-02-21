Imports Devart.Data.PostgreSql
Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load        

        'Dim dbschema1 As New dbSchema("postgres")
        ' 'Dim dbschema2 As New dbSchema("hr")
        'Dim schemas As New List(Of dbSchema)
        'schemas.Add(dbschema1)

        'Dim rdf As New RdfMapping("dmpdata", schemas)
        'rdf.Mapping_AllSchemas()
    End Sub

    Private Sub prnt(ByRef dt As DataTable, ByRef txt As TextBox)
        Dim res As String = ""
        For j As Integer = 0 To dt.Columns.Count - 1
            res = res + dt.Columns(j).ColumnName + " , "
        Next
        res = res + Environment.NewLine
        For i As Integer = 0 To dt.Rows.Count - 1
            For j As Integer = 0 To dt.Columns.Count - 1
                res = res + dt.Rows(i).ItemArray(j).ToString + " , "
            Next
            res = res + Environment.NewLine
        Next
        txt.Text = res
    End Sub

    Private Sub addSchemaButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles addSchemaButton.Click
        If Not (Me.ListBox1.Items.Contains(Me.TextBox1.Text) Or Me.TextBox1.Text = "") Then
            Me.ListBox1.Items.Add(Me.TextBox1.Text)
            Me.TextBox1.Text = ""
        End If

    End Sub

    Private Sub remove_schemaButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles remove_schemaButton.Click
        If Me.ListBox1.SelectedIndices.Count >= 1 Then
            Me.ListBox1.Items.RemoveAt(Me.ListBox1.SelectedIndices(0))
        End If

    End Sub

    Private Sub TextBox1_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyValue = Keys.Enter Then
            addSchemaButton_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub Mapping_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Mapping_Button.Click
        Dim schemaObj As dbSchema
        Dim schemas As New List(Of dbSchema)
        For Each schema As String In Me.ListBox1.Items
            schemaObj = New dbSchema(schema)
            schemas.Add(schemaObj)
        Next
        Dim mappingObj As New RdfMapping("rdfdata", schemas)
        mappingObj.Mapping_AllSchemas()
        Me.title_Label.Text = "mapping is done"
    End Sub
End Class
