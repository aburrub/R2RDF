Imports System.Data.OleDb
Imports System.Data.OracleClient
Public Class dbSchema
    Private associationRelations As New List(Of association) ' save in it is-a or part-of relations
    Private associationTables As New List(Of String) ' save entity tables that is made an association
    Private AbstractionsRelations As New List(Of relation)
    Private connectionString As String

    Private _connection As OleDbConnection = Nothing
    Private tables As New Hashtable
    Private db As String

    Public Function get_is_a_and_part_of_relations() As List(Of relation)
        Return Me.AbstractionsRelations
    End Function

    Public Sub New(ByVal db As String)
        Me.db = db
    End Sub

    Public Sub extractDataFromThisSchema()
        Me.setConnection(Me.db)
        Me.openConnection()
        Me.GenerateTablesDetails()
        Me.findAllAssociations()
        Me.closeConnection()
    End Sub
    'postgres,postgres, 127.0.0.1, 5432, hr
    Public Sub New(ByVal db_user_id As String, ByVal db_user_password As String, ByVal db_host As String, ByVal db_port As String)
        Me.connectionString = "User ID=" + db_user_id + ";Password=" + db_user_password + ";Host=" + db_host + ";Port=" + db_port + ";Database=" + Me.db + "; Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;Unicode=True"
    End Sub

    Public Sub setConnection(ByVal pathfile As String)
        Me.connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathfile
    End Sub
    Public Sub closeConnection()
        Me._connection.Close()
    End Sub
    Public Function openConnection() As Boolean
        Try
            Me._connection = New OleDbConnection()
            Me._connection.ConnectionString = Me.connectionString
            Me._connection.Open()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Private Function getPrimaryKeyColumnFromPrimaryKeyName(ByRef primaryKeyRow As DataRow) As List(Of String)
        Dim PrimaryKeyColumnNameFromSchemaIndex As Integer = 3
        Dim PrimaryKeyNameFromSchemaIndex As Integer = 0
        Dim result As New List(Of String)
        Dim IndexColumnsFromSchema As DataTable
        Dim primaryKeyName As String
        Dim primaryKeyColumnName As String
        Dim IndexColumnsRestriction(2) As String
        primaryKeyName = primaryKeyRow.ItemArray(PrimaryKeyNameFromSchemaIndex).ToString
        IndexColumnsRestriction(2) = primaryKeyName
        IndexColumnsFromSchema = Me._connection.GetSchema(RestrictionName.IndexColumns.ToString, IndexColumnsRestriction)
        For Each row As DataRow In IndexColumnsFromSchema.Rows
            primaryKeyColumnName = row.ItemArray(PrimaryKeyColumnNameFromSchemaIndex).ToString()
            result.Add(primaryKeyColumnName.ToLower)
        Next


        Return result
    End Function
    Private Function getForeinKeyColumnFromPrimaryKeyName(ByRef foreignKeyRow As DataRow) As dbfk
        Dim foreignKeyName As String
        Dim foreignKeyColumnName As String
        Dim foreignKeyTableName As String

        Dim rest(2) As String

        Dim foreignKeyNameFromSchemaIndex As Integer = 9
        Dim foreignKeyTableNameFromSchemaIndex As Integer = 2
        Dim foreignKeyColumnNameFromSchemaIndex As Integer = 9

        foreignKeyName = foreignKeyRow.ItemArray(foreignKeyNameFromSchemaIndex).ToString()
        foreignKeyTableName = foreignKeyRow.ItemArray(foreignKeyTableNameFromSchemaIndex).ToString()
        Try
            Dim foreignKeyColSchema As DataTable
            rest(2) = foreignKeyName
            foreignKeyColSchema = Me._connection.GetSchema(RestrictionName.ForeignKeyColumns.ToString, rest)
            foreignKeyColumnName = foreignKeyColSchema.Rows(0).ItemArray(foreignKeyColumnNameFromSchemaIndex).ToString

        Catch ex As Exception

        End Try
        
        Return New dbfk(foreignKeyName.ToLower, foreignKeyName.ToLower, foreignKeyTableName.ToLower)
    End Function

    Public Function GenerateTablesDetails() As Boolean
        Try
            Dim TableFieldFromSchemaIndex As Integer = 2

            Dim tableName As String
            Dim tableRestrictions(3) As String
            Dim tablesFromSchema As DataTable
            Dim primaryKeysFromSchema As DataTable
            Dim foreignKeysFromSchema As DataTable

            ' primary key related
            Dim primaryKeyRestriction(4) As String
            Dim foreignKeyRestriction(4) As String

            tableRestrictions(3) = "TABLE"
            tablesFromSchema = Me._connection.GetSchema("tables", tableRestrictions)

            For Each tableRow As DataRow In tablesFromSchema.Rows

                tableName = tableRow.Item(2).ToString()
                Dim tableObj As New dbtable(tableName.ToLower)



                primaryKeyRestriction(4) = tableName ' for each table we should get primary keys
                primaryKeysFromSchema = Me._connection.GetSchema("indexes", primaryKeyRestriction)
                ' save primarykey columns
                For Each primaryKeyRow As DataRow In primaryKeysFromSchema.Rows
                    MsgBox(primaryKeyRow.ItemArray(17).ToString())
                    If (primaryKeyRow.ItemArray(6).ToString.ToLower = "true") Then
                        tableObj.appendKey(TableFieldType.PRIMARY_KEY, primaryKeyRow.ItemArray(17).ToString())
                    End If

                Next

                foreignKeyRestriction(4) = tableName ' for each table we should get primary keys
                foreignKeysFromSchema = _connection.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Foreign_Keys, Nothing)

                For Each foreignRow As DataRow In foreignKeysFromSchema.Rows
                    If (foreignRow.Item(8).ToString.ToLower = tableName.ToLower) Then
                        tableObj.appendFK(TableFieldType.FOREIGN_KEY, getForeinKeyColumnFromPrimaryKeyName(foreignRow))
                    End If

                Next

                ' set other fields
                Dim columnsSchema As DataTable
                Dim colIndex As Integer = 2
                Dim colName As String
                Dim columRest(3) As String
                columRest(2) = tableName
                columnsSchema = _connection.GetSchema("Columns", columRest)
                For Each row As DataRow In columnsSchema.Rows
                    colName = row.ItemArray(3).ToString
                    If Not (tableObj.hasField(TableFieldType.PRIMARY_KEY, colName) Or tableObj.hasField(TableFieldType.FOREIGN_KEY, colName)) Then
                        tableObj.appendKey(TableFieldType.NORMAL_COLUMN, colName)
                    End If
                Next

                tableObj.SetEntityTable()
                tables.Add(tableObj.NAME, tableObj)
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function GetTable(ByRef tableName As String) As dbtable
        Return CType(Me.tables(tableName), dbtable)
    End Function

    Public Function GetTables() As Hashtable
        Return Me.tables
    End Function

    Public ReadOnly Property CONNECTION() As OleDbConnection
        Get
            Return Me._connection
        End Get
    End Property

    Public Sub findAllAssociations()

        Dim pk_length As Integer = 0
        Dim associationFlag As Boolean
        Dim e2e As Boolean
        Dim fkAsPkFlag As Boolean
        For Each tbl As dbtable In Me.tables.Values

            ' establish an association between an entity and the entity corresponding to each foreign key, unless it also happens to be a part of the primary key
            If tbl.IS_ENTITY_TABLE And Not tbl.FKs Is Nothing Then
                For Each fk As dbfk In tbl.FKs
                    e2e = True
                    For Each pk As String In tbl.PKs
                        If fk.FOREIGNKEY = pk Then
                            e2e = False
                            Exit For
                        End If
                    Next
                    If e2e Then
                        sharedModule.insertWithoutReplication(tbl.NAME, Me.associationTables)
                        sharedModule.insertWithoutReplication(fk.FK_TABLE, Me.associationTables)
                        addNewAssociation(New association(tbl.NAME, fk.FK_TABLE)) 'Me.relations.Add(New association(tbl.NAME, fk.FK_TABLE))
                    End If
                Next
            End If
            ' end of entity-2-entity association

            associationFlag = True
            pk_length = tbl.COUNT(TableFieldType.PRIMARY_KEY)
            If pk_length > 1 Then
                ' check if all parts of primaryKey is also in foreignKey
                For Each pk_part As String In tbl.PKs
                    If Not tbl.hasField(TableFieldType.FOREIGN_KEY, pk_part) Then
                        associationFlag = False
                    End If
                Next

                If associationFlag Then
                    For Each fk As dbfk In tbl.FKs  ' every table whose primary key has more than one attribute and is entirely composed of foreign key represents an association between all the entities corresponding to the foreign keys
                        For Each s_t As dbtable In Me.tables.Values
                            If Not (s_t.NAME = tbl.NAME) And s_t.IS_ENTITY_TABLE Then
                                If s_t.hasField(TableFieldType.FOREIGN_KEY, fk.FOREIGNKEY) Then
                                    sharedModule.insertWithoutReplication(fk.FK_TABLE, Me.associationTables)
                                    sharedModule.insertWithoutReplication(s_t.NAME, Me.associationTables)
                                    Me.addNewAssociation(New association(fk.FK_TABLE, s_t.NAME)) 'relations.Add(New association(fk.FK_TABLE, s_t.NAME))
                                End If
                            End If
                        Next
                    Next
                End If
                Dim x333 As String = ""

                ' here we will check part-of relation
                If tbl.et Then
                    Dim fks_in_pk As String() = tbl.getAllForeignKeysInPrimaryKey2
                    Dim fk_in_pk_flag As Boolean = If(fks_in_pk Is Nothing, False, True)
                    If fk_in_pk_flag And tbl.IS_ENTITY_TABLE And tbl.PKs.Count >= 2 Then
                        For Each t1 As dbtable In Me.tables.Values
                            If Not (t1.NAME = tbl.NAME) Then
                                For Each fkValue As String In fks_in_pk
                                    fkAsPkFlag = False
                                    For Each pkValue As String In t1.PKs
                                        If fkValue = pkValue Then
                                            fkAsPkFlag = True
                                            Exit For
                                        End If
                                    Next
                                    If fkAsPkFlag Then
                                        Me.AbstractionsRelations.Add(New relation(tbl.NAME, t1.NAME, RelationType.PART_OF_RELATION))
                                    End If
                                Next

                            End If
                        Next
                    End If
                End If
                

            Else



                ' here we will check is-a relaton
                If Not tbl.PKs Is Nothing Then
                    Dim pk As String = tbl.PKs(0)
                    '   MsgBox("table[" + tbl.NAME + "] pk[" + pk + "]")
                    Dim fc As Integer = 0


                    For Each t1 As dbtable In Me.tables.Values
                        fc = If(t1.FKs Is Nothing, 0, t1.FKs.Count)
                        If Not (t1.NAME = tbl.NAME) And fc = 1 Then
                            If t1.hasExactField(TableFieldType.PRIMARY_KEY, pk) And t1.hasExactField(TableFieldType.FOREIGN_KEY, t1.FKs(0).FOREIGNKEY_NAME) And t1.checkForeignKeyReferenceTable(t1.FKs(0).FOREIGNKEY_NAME, tbl.NAME) And t1.IS_ENTITY_TABLE Then
                                Me.AbstractionsRelations.Add(New relation(tbl.NAME, t1.NAME, RelationType.IS_A_RELATION))
                                Continue For
                            End If
                        End If
                    Next

                    ' here we will check part-of relation
                    If tbl.et Then
                        Dim fks_in_pk As String() = tbl.getAllForeignKeysInPrimaryKey2
                        Dim fk_in_pk_flag As Boolean = If(fks_in_pk Is Nothing, False, True)
                        If fk_in_pk_flag And tbl.IS_ENTITY_TABLE And tbl.PKs.Count >= 2 Then
                            For Each t1 As dbtable In Me.tables.Values
                                If Not (t1.NAME = tbl.NAME) And t1.IS_ENTITY_TABLE Then
                                    For Each fkValue As String In fks_in_pk
                                        fkAsPkFlag = False
                                        For Each pkValue As String In t1.PKs
                                            If fkValue = pkValue Then
                                                fkAsPkFlag = True
                                                Exit For
                                            End If
                                        Next
                                        If fkAsPkFlag Then
                                            Me.AbstractionsRelations.Add(New relation(tbl.NAME, t1.NAME, RelationType.PART_OF_RELATION))
                                        End If
                                    Next

                                End If
                            Next
                        End If
                    End If
                End If
                    


            End If

        Next

        ' Here we will find 1-M  association - problem in [1-1]
        Dim atObj1, atObj2 As dbtable
        For Each at1 As String In Me.associationTables
            atObj1 = Me.GetTable(at1)
            For Each at2 As String In Me.associationTables
                If Not at1 = at2 Then
                    atObj2 = Me.GetTable(at2)
                    If atObj1.COUNT(TableFieldType.PRIMARY_KEY) = 1 And atObj1.COUNT(TableFieldType.FOREIGN_KEY) >= 1 Then
                        For Each fk As dbfk In atObj1.FKs
                            For Each pk As String In atObj2.PKs
                                If fk.FOREIGNKEY = pk And atObj2.COUNT(TableFieldType.FOREIGN_KEY) = 0 Then
                                    'MsgBox("(" + at2 + ")=>(" + at1 + ")")
                                End If
                            Next
                        Next



                    End If
                    ' here is logic


                End If

            Next
        Next
    End Sub
    ' check if relation entry is already in relations

    Private Function addNewAssociation(ByVal assParam As association) As Boolean
        Dim flg As Boolean
        For Each assObj As association In Me.associationRelations
            flg = (assObj.TABLE1 = assParam.TABLE1) And (assObj.TABLE2 = assParam.TABLE2)
            If Not flg Then
                flg = (assObj.TABLE1 = assParam.TABLE2) And (assObj.TABLE2 = assParam.TABLE1)
            End If
            If flg Then
                Exit For
            End If
        Next
        If Not flg Then
            Me.associationRelations.Add(assParam)
            Return True
        Else
            Return False
        End If
    End Function
    Public ReadOnly Property DATABASE_NAME As String
        Get
            Return Me.db
        End Get
    End Property
End Class
