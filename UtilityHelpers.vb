Public Module UtilityHelpers

    Public Function ToDec(value As Object) As Decimal

        If value Is Nothing OrElse IsDBNull(value) Then
            Return 0D
        End If

        Dim result As Decimal

        If Decimal.TryParse(value.ToString(), result) Then
            Return result
        End If

        Return 0D

    End Function


End Module
