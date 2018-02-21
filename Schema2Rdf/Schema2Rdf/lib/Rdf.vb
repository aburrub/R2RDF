Public Class Rdf
    Private _subject As String
    Private _predicate As String
    Private _obj As String

    Public Sub New(ByVal subject As String, ByVal predicate As String, ByVal obj As String)
        Me._subject = subject
        Me._predicate = predicate
        Me._obj = obj
    End Sub

    Public Property SUBJECT() As String
        Get
            Return Me._subject
        End Get
        Set(ByVal value As String)
            Me._subject = value
        End Set
    End Property

    Public Property PREDICATE() As String
        Get
            Return Me._predicate
        End Get
        Set(ByVal value As String)
            Me._predicate = value
        End Set
    End Property

    Public Property OBJ() As String
        Get
            Return Me._obj
        End Get
        Set(ByVal value As String)
            Me._obj = value
        End Set
    End Property

    Public Sub printRdfEntry()

        MsgBox("subject" + Me._subject + Environment.NewLine + "predicate" + Me._predicate + Environment.NewLine + "object" + Me._obj)
    End Sub
End Class
