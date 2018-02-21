Imports System.Data.OracleClient
Imports System.Data.OleDb
Public Class RdfMapping
    ' used for conversion
    'Private dbInt As dbIntegration

    Public Shared SH_DATABASE As String = ""
    Public Shared SH_TABLE As String = ""
    Public Shared SH_SUBJECT As String = ""
    Public Shared SH_PREDICATE As String = ""
    Public Shared SH_OBJ As String = ""
    Public Shared SH_FLG As Boolean = True
    ' xml name spaces
    Private xmlns_rdf As String = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
    Private xmlns_rdfs As String = "http://www.w3.org/2000/01/rdf-schema#"
    Private xmlns_owl As String = "http://www.w3.org/2002/07/owl#"

    Private domain As String = "http://mop-gov.ps/"
    Private nullValue As String = "NULL_DEFINED_VALUE"
    Private db_schemas As List(Of dbSchema)
    Private rdf_table As String
    Private rdf_sequence As String
    Private oracle_connection As OracleConnection
    Public Sub New(ByVal rdf_table As String, ByRef dbSchema As dbSchema, ByVal conversionPath As String)
        '  Me.dbInt = New dbIntegration(conversionPath)
        ' Me.dbInt.loadCSVFile()

        Dim schemas As New List(Of dbSchema)
        schemas.Add(dbSchema)

        Me.db_schemas = schemas
        Me.rdf_table = rdf_table
        Me.rdf_sequence = rdf_table + "_sequence"
    End Sub
    Public Sub New(ByVal rdf_table As String, ByRef db_schemas As List(Of dbSchema), ByVal conversionPath As String)
        'Me.dbInt = New dbIntegration(conversionPath)
        'Me.dbInt.loadCSVFile()

        Me.db_schemas = db_schemas
        Me.rdf_table = rdf_table
        Me.rdf_sequence = rdf_table + "_sequence"
    End Sub

    Public Function connect_to_oracle() As Boolean
        Return Me.connect_to_oracle("testuser", "testuser")
    End Function

    Public Sub create_RDF_Table_With_Sequence()
        Try
            Dim cmd As OracleCommand
            Dim create_sql_table As String = "CREATE TABLE " + Me.rdf_table + " (id NUMBER PRIMARY KEY, triple SDO_RDF_TRIPLE_S)"
            Dim create_sequence As String = "CREATE   SEQUENCE   " + Me.rdf_sequence + " INCREMENT   BY   1 START   WITH   1 ORDER"
            'Dim model_sql As String = "execute SEM_APIS.create_rdf_model('" + Me.rdf_table + "_model', '" + Me.rdf_table + "', 'triple')"

            For Each Sql As String In New String() {create_sql_table, create_sequence}
                cmd = New OracleCommand(Sql, Me.oracle_connection)
                cmd.ExecuteNonQuery()
            Next


            'string productSQL = "SELECT ProductID, ProductName, QuantityPerUnit FROM Products WHERE CategoryID = @CategoryID"
            'productCmd.Parameters.Add("@CategoryID", SqlDbType.Int)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub delete_RDF_Table_With_Sequence()
        Try
            Dim cmd As OracleCommand
            Dim delete_sql_table As String = "drop TABLE " + Me.rdf_table
            Dim delete_sequence As String = "drop   SEQUENCE   " + Me.rdf_sequence
            For Each Sql As String In New String() {delete_sql_table, delete_sequence}
                cmd = New OracleCommand(Sql, Me.oracle_connection)
                cmd.ExecuteNonQuery()
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


    End Sub

    Public Sub insertBatch(ByRef rdfs As List(Of Rdf), ByRef db As String, ByRef tableName As String, ByRef txtBoxControl As TextBox)
        Dim sql As String = ""
        Dim cmd As OracleCommand
        If Not rdfs Is Nothing Then
            If Not rdfs.Count = 0 Then
                For Each rdf_entry As Rdf In rdfs
                    Try
                        SH_DATABASE = db
                        SH_TABLE = tableName
                        SH_SUBJECT = rdf_entry.SUBJECT
                        SH_PREDICATE = rdf_entry.PREDICATE
                        SH_OBJ = rdf_entry.OBJ
                        sql = "INSERT INTO " + Me.rdf_table + " VALUES (" + Me.rdf_sequence + ".NextVal,SDO_RDF_TRIPLE_S('rdfdatamodel', '" + rdf_entry.SUBJECT + "','" + rdf_entry.PREDICATE + "','" + rdf_entry.OBJ.Replace("'", "").Replace("""", "") + "'))"
                        txtBoxControl.Text = txtBoxControl.Text + "{(" + rdf_entry.SUBJECT + "), (" + rdf_entry.PREDICATE + "), (" + rdf_entry.OBJ + ")}" + Environment.NewLine
                        cmd = New OracleCommand(sql, Me.oracle_connection)
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception
                        MsgBox(ex.Message)
                    End Try

                Next
            End If

        End If


    End Sub

    Public Function close_oracle_connection() As Boolean
        Try
            Me.oracle_connection.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function connect_to_oracle(ByVal username As String, ByVal password As String) As Boolean
        Try
            Me.oracle_connection = New OracleConnection("Data Source=orcl;User Id=" + username + ";Password=" + password + ";")
            Dim oo As New OracleConnection
            Me.oracle_connection.Open()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Sub runTh()
        timing.ShowDialog()
    End Sub
    ' this is the main execution function
    Public Sub Mapping_AllSchemas(ByRef txtBoxCtrl As TextBox)
        Dim th As New Threading.Thread(AddressOf runTh)
        th.Start()
        ' extract each schema
        Me.connect_to_oracle()

        ' if first time run
        If Microsoft.Win32.Registry.LocalMachine.GetValue("firstRun") Is Nothing Then
            Microsoft.Win32.Registry.LocalMachine.SetValue("firstRun", True)
            Me.create_RDF_Table_With_Sequence()
        End If


        For Each schema As dbSchema In Me.db_schemas


            RdfMapping.SH_FLG = True
            schema.extractDataFromThisSchema()
            RdfMapping.SH_FLG = False
            For Each tableName As String In schema.GetTables().Keys
                insertBatch(Me.MappingSingleTable(schema, tableName), schema.DATABASE_NAME, tableName, txtBoxCtrl)
            Next

            Dim rdfEntries As New List(Of Rdf)
            Dim relations As List(Of relation) = schema.get_is_a_and_part_of_relations
            For Each rel As relation In relations
                rdfEntries.Add(New Rdf(Me.domain + "/" + schema.DATABASE_NAME + "/" + rel.TABLE1, If(rel.isPartOfRelation, Me.xmlns_rdfs + "partOf", Me.xmlns_rdfs + "subClassOf"), Me.domain + "/" + schema.DATABASE_NAME + "/" + rel.TABLE1)) ' part of need change
                txtBoxCtrl.Text = txtBoxCtrl.Text + "{(" + Me.domain + "/" + schema.DATABASE_NAME + "/" + rel.TABLE1 + "), (" + If(rel.isPartOfRelation, Me.xmlns_rdfs + "partOf", Me.xmlns_rdfs + "subClassOf") + "), (" + Me.domain + "/" + schema.DATABASE_NAME + "/" + rel.TABLE2 + ")}" + Environment.NewLine
            Next

        Next
        th.Abort()
    End Sub

    Public Function MappingSingleTable(ByRef schema As dbSchema, ByVal table As String) As List(Of Rdf)
        Dim result As New List(Of Rdf)
        Dim subjectIRI As String = ""
        Dim typeIRI As String = ""
        Dim predicateIRI As String = ""
        Dim objectIRI As String = ""

        Dim tblObj As dbtable = schema.GetTable(table)
        Dim selectStatement As String = "SELECT "
        Dim fields() As String = tblObj.getAllFields
        Dim fieldsExceptFields() As String = tblObj.getAllFieldsExceptKeys
        For Each field As String In fields
            selectStatement = selectStatement + "[" + field + "]" + ", "
        Next
        selectStatement = selectStatement.Substring(0, selectStatement.LastIndexOf(", "))
        selectStatement = selectStatement + " FROM [" + table + "]"

        Dim pgsqlConnection As OleDbConnection = schema.CONNECTION
        pgsqlConnection.Open()

        Dim cmd As New OleDbCommand(selectStatement, pgsqlConnection)

        Dim reader As OleDbDataReader = cmd.ExecuteReader
        While reader.Read()
            ' generate subject IRI
            If Not tblObj.PKs Is Nothing Then
                Dim subjectValue As New List(Of String)
                For Each pk As String In tblObj.PKs
                    subjectValue.Add(reader(pk))
                Next
                subjectIRI = sharedModule.Generate_IRI(Me.domain, schema.DATABASE_NAME, table, tblObj.PKs, subjectValue.ToArray)
                result.Add(New Rdf(subjectIRI, Me.xmlns_rdf + "rdf:type", tblObj.NAME))
            End If

            For Each field As String In fieldsExceptFields
                predicateIRI = sharedModule.Generate_IRI(Me.domain, schema.DATABASE_NAME, table, field, Nothing)
                objectIRI = sharedModule.Generate_IRI(Me.domain, schema.DATABASE_NAME, table, Nothing, If(reader(field).ToString = "", Me.nullValue, reader(field).ToString))
                'MsgBox(subjectIRI + Environment.NewLine + predicateIRI + Environment.NewLine + objectIRI + Environment.NewLine)
                result.Add(New Rdf(subjectIRI, predicateIRI, objectIRI))
            Next

            ' this is for foreign keys
            If Not tblObj.FKs Is Nothing Then
                For Each fk As dbfk In tblObj.FKs
                    objectIRI = sharedModule.Generate_IRI(Me.domain, schema.DATABASE_NAME, table, Nothing, If(reader(fk.FOREIGNKEY).ToString = "", Me.nullValue, reader(fk.FOREIGNKEY).ToString))
                    result.Add(New Rdf(objectIRI, Me.xmlns_rdfs + "domain", tblObj.NAME))
                    result.Add(New Rdf(objectIRI, Me.xmlns_rdfs + "range", fk.FK_TABLE))
                    'result.Item(result.Count - 2).printRdfEntry()
                    'result.Item(result.Count - 1).printRdfEntry()
                Next
            End If


        End While
        pgsqlConnection.Close()
        Return If(result.Count = 0, Nothing, result)
    End Function

    Private Sub insertRdfRow(ByRef rdf As Rdf)

    End Sub
End Class
