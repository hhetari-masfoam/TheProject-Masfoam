Public Module EditFieldMapper

    Public Function ResolveEditKind(
        fieldName As String,
        quantityFields As HashSet(Of String),
        costFields As HashSet(Of String)
    ) As EditKind

        If quantityFields.Contains(fieldName) Then
            Return EditKind.Quantity
        End If

        If costFields.Contains(fieldName) Then
            Return EditKind.Cost
        End If

        Return EditKind.Data

    End Function

End Module
