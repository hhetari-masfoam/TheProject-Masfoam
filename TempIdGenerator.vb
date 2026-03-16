Public NotInheritable Class TempIdGenerator


    Private Shared _ledgerId As Long = 900000000

    Public Shared Function NextLedgerID() As Long

        _ledgerId += 1
        Return _ledgerId

    End Function


End Class
