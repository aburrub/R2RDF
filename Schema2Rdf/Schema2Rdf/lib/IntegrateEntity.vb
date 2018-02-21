Public Class IntegrateEntity
    Private _schema As String
    Private _table As String
    Private _fieldName As String
    Private _newFieldName As String
    Public Sub New(ByVal schema As String, ByVal table As String, ByVal field As String, ByVal newField As String)
        Me._schema = schema
        Me._table = table
        Me._fieldName = field
        Me._newFieldName = newField
    End Sub

    Public ReadOnly Property SCHEMA As String
        Get
            Return Me._schema
        End Get
    End Property

    Public ReadOnly Property TABLE As String
        Get
            Return Me._table
        End Get
    End Property

    Public ReadOnly Property FIELD As String
        Get
            Return Me._fieldName
        End Get
    End Property

    Public ReadOnly Property NEW_FIELD As String
        Get
            Return Me._newFieldName
        End Get
    End Property

End Class
