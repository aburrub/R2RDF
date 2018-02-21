Public Class association
    Private tbl1 As String
    Private tbl2 As String

    Public Sub New(ByVal t1 As String, ByVal t2 As String)
        Me.tbl1 = t1
        Me.tbl2 = t2
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

