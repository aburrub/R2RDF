Public Class test

    Private Sub test_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' test integrate flow
        Dim file As String = "../../MappingTable.txt"
        Dim integ As New IntegrateHash
        integ.load(file)
        integ.setDatabaseSchemaAndIntegrateIt(New dbSchema("sdf"))

    End Sub
End Class