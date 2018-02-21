Public Class IntegrateHash
    Private entities As Dictionary(Of String, List(Of IntegrateEntity))
    Public Sub New()
        If Me.entities Is Nothing Then
            Me.entities = New Dictionary(Of String, List(Of IntegrateEntity))
        End If
    End Sub

    Public Sub addEntity(ByVal entity As IntegrateEntity)
        Dim h As List(Of IntegrateEntity) = Nothing
        If Me.entities.ContainsKey(entity.SCHEMA) Then
            h = Me.entities(entity.SCHEMA)
        End If
        Dim cond As Boolean = h Is Nothing

        If cond Then
            h = New List(Of IntegrateEntity)
            h.Add(entity)
        Else
            h.Add(entity)
        End If

        If Not Me.entities.ContainsKey(entity.SCHEMA) Then
            Me.entities.Add(entity.SCHEMA, h)
        End If


    End Sub

    Public Sub load(ByVal filename As String)
        Try
            Dim r As New IO.StreamReader(filename)
            While Not r.EndOfStream
                Dim line As String = r.ReadLine
                If line.StartsWith("/*") Then
                    Continue While
                End If
                Dim pts() As String = line.Split(",")
                Dim entity As New IntegrateEntity(pts(0), pts(1), pts(2), pts(3))
                Me.addEntity(entity)
            End While
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Function GET_ALL_INTEGRATE_ENTITIES() As Dictionary(Of String, List(Of IntegrateEntity))
        Return Me.entities
    End Function


    Public Function setDatabaseSchemaAndIntegrateIt(ByRef dbs As dbSchema) As Boolean
        If Me.entities.Count = 0 Then
            Return False
        End If

        For Each entity As IntegrateEntity In Me.entities(dbs.DATABASE_NAME)
            Dim table As dbtable = dbs.GetTable(entity.TABLE)
            table.changeFieldValue(entity.FIELD, entity.NEW_FIELD)
        Next

        Return True
    End Function

End Class
