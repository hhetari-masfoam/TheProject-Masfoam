Imports System.Data.SqlClient

Public Class CostRevaluationService

    Private _repo As New FinPostingRepository()

    Public Sub PostRevaluation(runId As Long,
                               userId As Integer,
                               con As SqlConnection,
                               tran As SqlTransaction)

        ' =====================================
        ' 1️⃣ تحقق هل تم الترحيل سابقًا
        ' =====================================
        Dim existing As Object

        Using cmd As New SqlCommand("
            SELECT FinancialJournalID
            FROM inv.CostEngineRun
            WHERE RunID = @R
        ", con, tran)

            cmd.Parameters.AddWithValue("@R", runId)
            existing = cmd.ExecuteScalar()

        End Using

        If existing IsNot Nothing AndAlso Not IsDBNull(existing) Then Exit Sub

        ' =====================================
        ' 2️⃣ جلب البيانات
        ' =====================================
        Dim dt = _repo.GetRevaluationData(runId, con, tran)
        If dt.Rows.Count = 0 Then Exit Sub

        ' =====================================
        ' 3️⃣ تجهيز القيد
        ' =====================================
        Dim journalTypeId = _repo.GetJournalTypeID_ByCode("ADJ", con, tran)

        Dim totalDebit As Decimal = 0
        Dim totalCredit As Decimal = 0
        Dim refNo As String = "REVAL-" & runId
        ' =====================================
        ' 4️⃣ إنشاء Journal
        ' =====================================
        Dim result = _repo.InsertJournalHeader(
                0,
                journalTypeId,
                userId,
                0,
                0,
                20,
                0,
                con,
                tran,
                refNo   ' 🔥 هنا السحر
            )
        Dim journalId = result.JournalID
        Dim headerRef = result.RefNo
        Dim lineNo As Integer = 1

        ' =====================================
        ' 5️⃣ إدخال Inventory Lines
        ' =====================================
        For Each r As DataRow In dt.Rows

            Dim productId = CInt(r("ProductID"))
            Dim storeId = CInt(r("StoreID"))
            Dim diff = CDec(r("TotalDiff"))

            Dim invAccount = _repo.GetInventoryAccountByProduct(productId, con, tran)

            If diff > 0 Then
                Dim descLine As String = headerRef &
    " | Product " & productId &
    " | Store " & storeId

                ' Dr Inventory
                _repo.InsertJournalDetail(
                    journalId,
                    invAccount,
                    diff,
                    0,
                    lineNo,
                    Nothing,
                    Nothing,
                    Nothing,
                    productId,
                    Nothing,
                    userId,
                    con,
                    tran,
                    storeId,
                    descLine
                )

                totalDebit += diff

            Else
                Dim descLine As String =
    "Cost Revaluation - Run " & runId &
    " | Product " & productId &
    " | Store " & storeId

                ' Cr Inventory
                _repo.InsertJournalDetail(
                    journalId,
                    invAccount,
                    0,
                    Math.Abs(diff),
                    lineNo,
                    Nothing,
                    Nothing,
                    Nothing,
                    productId,
                    Nothing,
                    userId,
                    con,
                    tran,
                    storeId,
                    descLine
                )

                totalCredit += Math.Abs(diff)
            End If

            lineNo += 1

        Next

        ' =====================================
        ' 6️⃣ سطر Cost Adjustment
        ' =====================================
        Dim adjustmentAccount As Integer = 67

        If totalDebit > totalCredit Then

            Dim descAdj As String = headerRef & " | Adjustment"

            _repo.InsertJournalDetail(
                    journalId,
                    adjustmentAccount,
                    0D,
                    totalDebit - totalCredit,
                    lineNo,
                    Nothing,   ' partnerID
                    Nothing,   ' sourceDetailID
                    Nothing,   ' referenceDetailID
                    Nothing,   ' productID
                    Nothing,   ' transactionDetailID
                    userId,
                    con,
                    tran,
                    Nothing,   ' storeID
                    descAdj    ' description
                )

            totalCredit = totalDebit

        ElseIf totalCredit > totalDebit Then

            Dim descAdj As String = headerRef & " | Adjustment"

            _repo.InsertJournalDetail(
                journalId,
                adjustmentAccount,
                totalCredit - totalDebit,
                0D,
                lineNo,
                Nothing,
                Nothing,
                Nothing,
                Nothing,
                Nothing,
                userId,
                con,
                tran,
                Nothing,
                descAdj
            )

            totalDebit = totalCredit

        End If

        ' =====================================
        ' 7️⃣ تحديث totals في الهيدر
        ' =====================================
        Using cmd As New SqlCommand("
            UPDATE gl.JournalHeader
            SET TotalDebit = @D,
                TotalCredit = @C,
                CostRunID = @R
            WHERE JournalID = @J
        ", con, tran)

            cmd.Parameters.AddWithValue("@D", totalDebit)
            cmd.Parameters.AddWithValue("@C", totalCredit)
            cmd.Parameters.AddWithValue("@R", runId)
            cmd.Parameters.AddWithValue("@J", journalId)

            cmd.ExecuteNonQuery()

        End Using

        ' =====================================
        ' 8️⃣ ربط Run
        ' =====================================
        Using cmd As New SqlCommand("
            UPDATE inv.CostEngineRun
            SET FinancialJournalID = @J
            WHERE RunID = @R
        ", con, tran)

            cmd.Parameters.AddWithValue("@J", journalId)
            cmd.Parameters.AddWithValue("@R", runId)
            cmd.ExecuteNonQuery()

        End Using

    End Sub

End Class