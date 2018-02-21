Public Class dbfk
    Private _name As String
    Private _col As String
    Private _table As String
    Public Sub New(ByVal name As String, ByVal col As String, ByVal table As String)
        Me._col = col
        Me._name = name
        Me._table = table
    End Sub

    Public ReadOnly Property FOREIGNKEY() As String
        Get
            Return Me._col
        End Get
    End Property

    Public ReadOnly Property FOREIGNKEY_NAME() As String
        Get
            Return Me._name
        End Get
    End Property

    Public ReadOnly Property FK_TABLE() As String
        Get
            Return Me._table
        End Get
    End Property
End Class
