Public Module sharedModule
    Public Enum TableFieldType
        NORMAL_COLUMN = 0
        PRIMARY_KEY = 1
        FOREIGN_KEY = 2
    End Enum

    Public Enum RestrictionName
        Tables
        Columns
        PrimaryKeys
        ForeignKeys
        ForeignKeyColumns
        IndexColumns
    End Enum

    Public Enum RelationType
        IS_A_RELATION
        PART_OF_RELATION
    End Enum

    Public Function insertWithoutReplication(ByVal item As String, ByRef list As List(Of String)) As Boolean
        For Each listItem As String In list
            If listItem = item Then
                Return False
            End If
        Next
        list.Add(item)
        Return True
    End Function

    Public Function Generate_IRI(ByVal path As String, ByVal db As String, ByVal table As String, ByVal ky As String, ByVal vl As String) As String
        If Not ky Is Nothing And Not vl Is Nothing Then
            Return Generate_IRI(path, db, table, New String() {ky}, New String() {vl})
        ElseIf ky Is Nothing Then
            Return Generate_IRI(path, db, table, Nothing, New String() {vl})
        Else
            Return Generate_IRI(path, db, table, New String() {ky}, Nothing)
        End If
        

    End Function
    Public Function Generate_IRI(ByVal path As String, ByVal db As String, ByVal table As String, ByVal kys() As String, ByVal vls() As String) As String
        Try

            Dim len As Integer
            If Not kys Is Nothing Then
                len = kys.Count
            End If

            If Not vls Is Nothing Then
                len = vls.Count
            End If

            path = If(path.EndsWith("/"), path, path + "/")
            If Not vls Is Nothing And Not kys Is Nothing Then
                Dim token As String = ""
                For i As Integer = 0 To len - 1
                    token = token + kys(i) + "=" + vls(i) + "&"
                Next
                path = path + db + "/" + table + "?" + token.Substring(0, token.Length - 1)
            ElseIf Not kys Is Nothing And vls Is Nothing Then
                Dim token As String = ""
                For i As Integer = 0 To len - 1
                    token = token + kys(i) + "."
                Next
                path = path + db + "/" + table + "?" + token.Substring(0, token.Length - 1)
            Else
                Dim token As String = ""
                For i As Integer = 0 To len - 1
                    token = token + vls(i) + "."
                Next
                path = path + db + "/" + table + "?obj=" + token.Substring(0, token.Length - 1)
            End If

            Return path

        Catch ex As Exception
            Return ""
        End Try
    End Function
End Module
