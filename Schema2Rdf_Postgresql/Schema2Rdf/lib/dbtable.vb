
Public Class dbtable
    Private _tableName As String
    Private _fields As New Hashtable
    Private _primaryKey As New Hashtable
    Private _foreignKey As New Hashtable ' should save referenced table
    Private _entityTable As Boolean
    Public Sub New(ByVal tableName As String)
        Me._tableName = tableName
    End Sub

    Public Sub appendKey(ByVal type As TableFieldType, ByVal value As String)
        Select Case type
            Case TableFieldType.NORMAL_COLUMN
                Me._fields.Add(value, value)

            Case TableFieldType.PRIMARY_KEY
                Me._primaryKey.Add(value, value)

        End Select
    End Sub

    Public Sub appendFK(ByVal type As TableFieldType, ByVal value As dbfk)
        Select Case type
            Case TableFieldType.FOREIGN_KEY
                Me._foreignKey.Add(value.FOREIGNKEY_NAME, value) ' zzzzzzzzzzzzzzzzzzzzzzzzzzz
        End Select
    End Sub

    Public Sub removeKey(ByVal type As TableFieldType, ByVal value As String)
        Select Case type
            Case TableFieldType.NORMAL_COLUMN
                Me._fields.Remove(value)

            Case TableFieldType.PRIMARY_KEY
                Me._primaryKey.Remove(value)

            Case TableFieldType.FOREIGN_KEY
                Me._foreignKey.Remove(value)
        End Select
    End Sub

    Public ReadOnly Property COUNT(ByVal type As TableFieldType) As Integer        
        Get
            Dim FieldsCount As Integer = 0
            Try
                Select Case type
                    Case TableFieldType.NORMAL_COLUMN
                        FieldsCount = Me._fields.Count

                    Case TableFieldType.PRIMARY_KEY
                        FieldsCount = Me._primaryKey.Count

                    Case TableFieldType.FOREIGN_KEY
                        FieldsCount = Me._foreignKey.Count
                End Select
            Catch ex As Exception
                Return FieldsCount
            End Try

            Return FieldsCount
        End Get
    End Property

    ' this function should be used once we create the table or modify it
    Public Sub SetEntityTable()
        Dim result As Boolean = False
        result = If(COUNT(TableFieldType.PRIMARY_KEY) = 1, True, False)
        For Each pk As String In Me._primaryKey.Keys
            If Me._foreignKey(pk) Is Nothing Then
                result = True
                Exit For
            End If
        Next
        Me._entityTable = result
    End Sub
    Public ReadOnly Property IS_ENTITY_TABLE() As Boolean
        Get
            Dim result As Boolean = False
            result = If(COUNT(TableFieldType.PRIMARY_KEY) = 1, True, False)
            For Each pk As String In Me._primaryKey.Values
                If Me._foreignKey(pk) Is Nothing Then
                    result = True
                    Exit For
                End If
            Next
            Me._entityTable = result
            Return result
        End Get
    End Property

    Public Function hasField(ByVal type As TableFieldType, ByVal name As String) As Boolean
        Select Case type
            Case TableFieldType.FOREIGN_KEY
                Return If(Me._foreignKey(name) Is Nothing, False, True)

            Case TableFieldType.PRIMARY_KEY
                Return If(Me._primaryKey(name) Is Nothing, False, True)
        End Select
        Return False
    End Function
    Public Function checkForeignKeyReferenceTable(ByVal fk As String, ByVal TableName As String) As Boolean
        Dim ob As dbfk = Me._foreignKey(fk)
        If ob Is Nothing Then
            Return False
        Else
            If ob.FK_TABLE = TableName Then
                Return True
            Else
                Return False
            End If
        End If
    End Function
    Public Function hasExactField(ByVal type As TableFieldType, ByVal name As String) As Boolean
        Select Case type
            Case TableFieldType.FOREIGN_KEY
                If Me._foreignKey.Keys.Count >= 2 Then
                    Return False
                End If
                Return If(Me._foreignKey(name) Is Nothing, False, True)

            Case TableFieldType.PRIMARY_KEY
                If Me._primaryKey.Keys.Count >= 2 Then
                    Return False
                End If
                Return If(Me._primaryKey(name) Is Nothing, False, True)
        End Select
        Return False
    End Function
    Public ReadOnly Property NAME() As String
        Get
            Return Me._tableName
        End Get
    End Property

    Public ReadOnly Property ENTITY_TABLE() As Boolean
        Get
            Return Me._entityTable
        End Get
    End Property

    Public ReadOnly Property PKs() As String()
        Get
            Dim result As New List(Of String)
            For Each pk_str As String In Me._primaryKey.Values
                result.Add(pk_str)
            Next
            Return If(result.Count = 0, Nothing, result.ToArray)
        End Get
    End Property

    Public ReadOnly Property FKs() As List(Of dbfk)
        Get
            Dim result As New List(Of dbfk)
            For Each fko As dbfk In Me._foreignKey.Values
                result.Add(fko)
            Next
            Return If(result.Count = 0, Nothing, result)
        End Get
    End Property

    Public Function printPkObjs() As String
        Dim res As String = ""
        For Each obj As String In Me._primaryKey.Values
            res = res + obj + ", " + Environment.NewLine
        Next
        Return res
    End Function

    Public Function checkAtLeastOneForeignKey() As Boolean
        For Each fkey As dbfk In Me._foreignKey.Values
            For Each pkey As String In Me._primaryKey.Values
                If pkey = fkey.FOREIGNKEY Then
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    ' get all fkeys that are in primary key
    Public Function getAllForeignKeysInPrimaryKey() As String()
        Dim res As New List(Of String)
        For Each fkey As dbfk In Me._foreignKey.Values
            For Each pkey As String In Me._primaryKey.Values
                If pkey = fkey.FOREIGNKEY Then
                    res.Add(pkey)
                    Exit For
                End If
            Next
        Next
        Return If(res.Count = 0, Nothing, res.ToArray)
    End Function
    Public Function getAllFields() As String()
        Dim allfields As New List(Of String)
        If Not Me.PKs Is Nothing Then
            For Each pk As String In Me.PKs
                sharedModule.insertWithoutReplication(pk, allfields)
            Next
        End If

        If Not Me.FKs Is Nothing Then
            For Each fk As dbfk In Me.FKs
                sharedModule.insertWithoutReplication(fk.FOREIGNKEY, allfields)
            Next
        End If

        For Each field As String In Me.FIELDS.Keys
            sharedModule.insertWithoutReplication(field, allfields)
        Next
        Return If(allfields.Count = 0, Nothing, allfields.ToArray)
    End Function

    Public Function getAllFieldsExceptKeys() As String()
        Dim allfields As New List(Of String)
        
        For Each field As String In Me.FIELDS.Keys
            sharedModule.insertWithoutReplication(field, allfields)
        Next
        Return If(allfields.Count = 0, Nothing, allfields.ToArray)
    End Function
    Public ReadOnly Property FIELDS As Hashtable
        Get
            Return Me._fields
        End Get
    End Property
End Class
