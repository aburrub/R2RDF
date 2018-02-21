Public Class relation
    Private tbl1 As String
    Private tbl2 As String
    Private rel As RelationType
    Public Function isPartOfRelation() As Boolean
        Return If(Me.rel = RelationType.PART_OF_RELATION, True, False)
    End Function

    Public Sub New(ByVal t1 As String, ByVal t2 As String, ByVal relation As RelationType)
        Me.tbl1 = t1
        Me.tbl2 = t2
        Me.rel = relation
    End Sub

    Public Property TABLE1() As String
        Get
            Return Me.tbl1
        End Get
        Set(ByVal value As String)
            Me.tbl1 = value
        End Set
    End Property

    Public Property TABLE2() As String
        Get
            Return Me.tbl2
        End Get
        Set(ByVal value As String)
            Me.tbl2 = value
        End Set
    End Property
End Class
