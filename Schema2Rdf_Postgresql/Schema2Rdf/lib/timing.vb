Public Class timing    
    Private Sub timing_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Timer1.Enabled = True
        Me.Label2.Text = ""
        Me.Label3.Text = ""
        Me.Label4.Text = ""
        Me.Label1.Text = "Extraction Feature..."
    End Sub

    Public Sub setValues() Handles Timer1.Tick
        If Not RdfMapping.SH_FLG Then
            Me.Label1.Text = "Transforming of "
        End If
        Me.Label2.Text = "Database: " + RdfMapping.SH_DATABASE
        Me.Label3.Text = "TABLE: " + RdfMapping.SH_TABLE
        Me.Label4.Text = "Subject: " + RdfMapping.SH_SUBJECT
        Me.Label6.Text = "Predicat: " + RdfMapping.SH_PREDICATE
        Me.Label5.Text = "Object: " + RdfMapping.SH_OBJ

    End Sub

End Class