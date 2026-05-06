Imports System.Data.SqlClient

Public Class FinPostingRepository
    Public Class CashOperationInfo
        Public Property OperationTypeID As Integer
        Public Property OperationCode As String
        Public Property CashDirection As Integer
    End Class
    Public Function GetOperationTypeID(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        ' 🔴 1. حاول من المخزون
        Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM inv.TransactionHeader
WHERE TransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                Return CInt(obj)
            End If

        End Using

        ' 🔵 2. حاول من المالية
        Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                Return CInt(obj)
            End If

        End Using


        Return 0

    End Function


    Public Function GetPostingRuleHeader(
        opTypeID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As PostingRuleHeader

        Using cmd As New SqlCommand("
SELECT TOP 1 *
FROM gl.PostingRuleHeader
WHERE OperationTypeID = @OP
  AND IsActive = 1
", con, tran)

            cmd.Parameters.AddWithValue("@OP", opTypeID)

            Using rd = cmd.ExecuteReader()

                If rd.Read() Then
                    Dim r As New PostingRuleHeader()
                    r.PostingRuleHeaderID = CInt(rd("PostingRuleHeaderID"))
                    Return r
                End If

            End Using
        End Using

        Return Nothing

    End Function
    Public Function GetPartnerID(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer

        Using cmd As New SqlCommand("
SELECT d.PartnerID
FROM inv.TransactionHeader t
JOIN inv.DocumentHeader d ON d.DocumentID = t.SourceDocumentID
WHERE t.TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)
            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetPartnerID_ByDocument(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT PartnerID
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                Return CInt(obj)
            End If
        End Using

        Return 0
    End Function
    Public Function GetPartnerID_FromPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT PartnerID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 0
            End If

            Return CInt(obj)

        End Using

    End Function

    Public Function GetPostingRuleDetails(
        headerID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As List(Of PostingRuleDetail)

        Dim list As New List(Of PostingRuleDetail)

        Using cmd As New SqlCommand("
SELECT *
FROM gl.PostingRuleDetails
WHERE PostingRuleHeaderID = @ID
  AND IsActive = 1
ORDER BY LineNumber
", con, tran)

            cmd.Parameters.AddWithValue("@ID", headerID)

            Using rd = cmd.ExecuteReader()

                While rd.Read()

                    Dim d As New PostingRuleDetail()

                    d.PostingRuleDetailID = CInt(rd("PostingRuleDetailID"))
                    d.EntrySideID = CInt(rd("EntrySideID"))
                    d.FixedAccountID =
                        If(IsDBNull(rd("FixedAccountID")), 0, CInt(rd("FixedAccountID")))
                    d.AccountSourceTypeID =
                        If(IsDBNull(rd("AccountSourceTypeID")), 0, CInt(rd("AccountSourceTypeID")))
                    d.SourceAmountFieldID = CInt(rd("SourceAmountFieldID"))
                    d.IsDistributed = CBool(rd("IsDistributed"))
                    list.Add(d)

                End While

            End Using
        End Using

        Return list

    End Function
    Public Function GetCurrencyID(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer

        Using cmd As New SqlCommand("
SELECT d.CurrencyID
FROM inv.DocumentHeader d
JOIN inv.TransactionHeader t ON t.SourceDocumentID = d.DocumentID
WHERE t.TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 1 ' 🔥 Default = SAR
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function InsertJournalHeader(
    transactionID As Integer,
    journalTypeID As Integer,
    userID As Integer,
    totalDebit As Decimal,
    totalCredit As Decimal,
    operationTypeID As Integer,
    sourceDocumentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction,
    Optional description As String = Nothing   ' 🔥 الجديد
) As (JournalID As Integer, RefNo As String)

        Dim journalDate As Date = Date.Today
        Dim periodID As Integer = 0

        Using cmdPeriod As New SqlCommand("
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE @D BETWEEN StartDate AND EndDate
  AND IsOpen = 1
", con, tran)

            cmdPeriod.Parameters.AddWithValue("@D", journalDate)

            Dim obj = cmdPeriod.ExecuteScalar()

            If obj Is Nothing Then
                Throw New Exception("لا توجد فترة محاسبية مفتوحة لهذا التاريخ")
            End If

            periodID = CInt(obj)
        End Using
        Dim journalNo = GenerateJournalNumber(con, tran)


        Dim currencyID = GetCurrencyID(transactionID, con, tran)
        Using cmd As New SqlCommand("
INSERT INTO gl.JournalHeader
(
    JournalNo,
    TransactionID,
    JournalTypeID,
    JournalDate,
    PeriodID,
    StatusID,
    CurrencyID,
    OperationTypeID,
    SourceDocumentID,
    SourceType,
    ReferenceNo,      -- 🔥 جديد
    SourceModule,     -- 🔥 جديد
    TotalDebit,
    TotalCredit,
    Description,
    CreatedBy,
    CreatedAt
)
VALUES
(
    @No,
    @T,               -- 🔥 هنا
    @JT,
    @JD,
    @PID,
    @S,
    @C,
    @OP,
    @SD,
    @SourceType,
    @RefNo,           -- 🔥
    @Module,          -- 🔥
    @TD,
    @TC,
    @DESC,
    @U,
    SYSDATETIME()
);

SELECT SCOPE_IDENTITY();
", con, tran)

            cmd.Parameters.AddWithValue("@No", journalNo)
            cmd.Parameters.AddWithValue("@T", transactionID)
            Dim refNo As String
            Dim moduleName As String
            Dim sourceType As String

            ' 🔴 العمليات المالية
            If operationTypeID = 16 OrElse operationTypeID = 17 OrElse operationTypeID = 18 OrElse operationTypeID = 19 Then

                ' حاول تجيب رقم السند الحقيقي
                Dim paymentNo As String = ""

                Using cmdRef As New SqlCommand("
SELECT TransactionNo 
FROM fin.CashTransactionHeader 
WHERE CashTransactionID = @ID
", con, tran)

                    cmdRef.Parameters.AddWithValue("@ID", transactionID)

                    Dim obj = cmdRef.ExecuteScalar()
                    If obj IsNot Nothing Then
                        paymentNo = obj.ToString()
                    End If
                End Using

                refNo = If(String.IsNullOrEmpty(paymentNo), "PAY-" & transactionID, paymentNo)
                moduleName = "FIN"
                sourceType = "FIN"

            Else
                ' 🔵 العمليات المخزنية
                refNo = "DOC-" & sourceDocumentID
                moduleName = "INV"
                sourceType = "INV"
            End If

            cmd.Parameters.AddWithValue("@RefNo", refNo)
            cmd.Parameters.AddWithValue("@Module", moduleName)
            cmd.Parameters.AddWithValue("@SourceType", sourceType)
            cmd.Parameters.AddWithValue("@TD", totalDebit)
            cmd.Parameters.AddWithValue("@TC", totalCredit)

            cmd.Parameters.AddWithValue("@JT", journalTypeID)
            cmd.Parameters.AddWithValue("@JD", journalDate)
            cmd.Parameters.AddWithValue("@PID", periodID)   ' 🔥
            cmd.Parameters.AddWithValue("@S", 24)
            cmd.Parameters.AddWithValue("@C", currencyID)
            cmd.Parameters.AddWithValue("@U", userID)
            cmd.Parameters.AddWithValue("@OP", operationTypeID)
            cmd.Parameters.AddWithValue("@SD", sourceDocumentID)
            Dim descValue As String

            If String.IsNullOrEmpty(description) Then
                descValue = refNo
            Else
                descValue = description
            End If

            cmd.Parameters.AddWithValue("@DESC", descValue)
            Dim journalID = CInt(cmd.ExecuteScalar())
            Return (journalID, refNo)
        End Using

    End Function

    Public Function InsertJournalDetail(
    journalID As Integer,
    accountID As Integer,
    DebitAmount As Decimal,
    CreditAmount As Decimal,
    lineNumber As Integer,
    partnerID As Integer?,
    sourceDetailID As Integer?,
    referenceDetailID As Integer?,   ' 🔥 موجود
    productID As Integer?,
    transactionDetailID As Integer?, ' 🔥 موجود
    userID As Integer,
    con As SqlConnection,
   tran As SqlTransaction,
    Optional storeID As Integer? = Nothing,
    Optional description As String = Nothing
    ) As Long

        Using cmd As New SqlCommand("
INSERT INTO gl.JournalDetails
(
    JournalID,
    LineNumber,
    AccountID,
    DebitAmount,
    CreditAmount,
    CreatedBy,
    CreatedAt,
    RelatedPartnerID,
    SourceDetailID,
    ReferenceDetailID,
    ProductID,
    TransactionDetailID,
 Description,
    StoreID
)
VALUES
(
    @J,
    @LN,
    @A,
    @D,
    @C,
    @U,
    SYSDATETIME(),
    @P,
    @SD,
    @RD,
    @PR,
    @TD,
@DESC,
    @StoreID
);

SELECT SCOPE_IDENTITY();
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@LN", lineNumber)
            cmd.Parameters.AddWithValue("@A", accountID)
            cmd.Parameters.AddWithValue("@D", DebitAmount)
            cmd.Parameters.AddWithValue("@C", CreditAmount)
            cmd.Parameters.AddWithValue("@U", userID)

            cmd.Parameters.AddWithValue("@P",
            If(partnerID.HasValue, CType(partnerID.Value, Object), DBNull.Value))

            cmd.Parameters.AddWithValue("@SD",
            If(sourceDetailID.HasValue, CType(sourceDetailID.Value, Object), DBNull.Value))

            cmd.Parameters.AddWithValue("@RD",
            If(referenceDetailID.HasValue, CType(referenceDetailID.Value, Object), DBNull.Value))

            cmd.Parameters.AddWithValue("@PR",
            If(productID.HasValue, CType(productID.Value, Object), DBNull.Value))

            cmd.Parameters.AddWithValue("@TD",
            If(transactionDetailID.HasValue, CType(transactionDetailID.Value, Object), DBNull.Value))

            ' 🔥 الجديد (قبل التنفيذ)
            cmd.Parameters.AddWithValue("@StoreID",
            If(storeID.HasValue, CType(storeID.Value, Object), DBNull.Value))
            Dim descValue As String

            If String.IsNullOrEmpty(description) Then

                If sourceDetailID.HasValue Then
                    descValue = "DOC-" & sourceDetailID.Value

                ElseIf transactionDetailID.HasValue Then
                    descValue = "TRX-" & transactionDetailID.Value

                ElseIf partnerID.HasValue Then
                    descValue = "PARTNER-" & partnerID.Value

                Else
                    descValue = "JOURNAL-" & journalID
                End If

            Else
                descValue = description
            End If

            cmd.Parameters.AddWithValue("@DESC", descValue)

            Return CLng(cmd.ExecuteScalar())

        End Using

    End Function

    Public Sub MarkTransactionAsFinancialPosted(
        transactionID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    )

        Using cmd As New SqlCommand("
UPDATE inv.TransactionHeader
SET IsFinancialPosted = 1
WHERE TransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)
            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Function GetNet(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalNetAmount
FROM inv.DocumentHeader
WHERE DocumentID = (
    SELECT SourceDocumentID FROM inv.TransactionHeader WHERE TransactionID = @T
)
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)
            Return CDec(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetTax(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalTax
FROM inv.DocumentHeader
WHERE DocumentID = (
    SELECT SourceDocumentID FROM inv.TransactionHeader WHERE TransactionID = @T
)
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)
            Return CDec(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetTotal(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalAmount
FROM inv.DocumentHeader
WHERE DocumentID = (
    SELECT SourceDocumentID
    FROM inv.TransactionHeader
    WHERE TransactionID = @T
)
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                Return 0D
            End If

            Return CDec(result)

        End Using

    End Function

    Public Function GetGrandTotal(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Decimal

        Using cmd As New SqlCommand("
SELECT GrandTotal
FROM inv.DocumentHeader
WHERE DocumentID = (
    SELECT SourceDocumentID FROM inv.TransactionHeader WHERE TransactionID = @T
)
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)
            Return CDec(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetInventoryAccount(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 fg.DefaultInventoryAccountID
FROM inv.TransactionDetails d
JOIN md.Product p ON p.ProductID = d.ProductID
JOIN gl.FinancialProductGroup fg ON fg.FinancialGroupID = p.FinancialGroupID
WHERE d.TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetPartnerAccount(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 p.AccountID
FROM inv.TransactionHeader t
JOIN inv.DocumentHeader d ON d.DocumentID = t.SourceDocumentID
JOIN md.Partner p ON p.PartnerID = d.PartnerID
WHERE t.TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Sub InsertAccountLedger(
    journalID As Long,
    journalDetailID As Long,
    accountID As Integer,
    transactionID As Integer?,
    debit As Decimal,
    credit As Decimal,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction,
    Optional description As String = Nothing,
    Optional sourceOperationTypeID As Integer = 0   ' 🔥 الجديد
)
        Dim lastBalance As Decimal = 0D

        Using cmdBal As New SqlCommand("
SELECT TOP 1 RunningBalance
FROM gl.AccountLedger
WHERE AccountID = @A
ORDER BY LedgerDate DESC, LedgerID DESC
", con, tran)

            cmdBal.Parameters.AddWithValue("@A", accountID)

            Dim obj = cmdBal.ExecuteScalar()
            If obj IsNot Nothing Then
                lastBalance = CDec(obj)
            End If
        End Using
        Dim newBalance As Decimal = lastBalance + debit - credit
        If transactionID.HasValue AndAlso sourceOperationTypeID > 0 Then

            Dim opType = sourceOperationTypeID

            If opType = 16 OrElse opType = 17 OrElse opType = 18 OrElse opType = 19 Then

                ' 🔵 Cash
                If Not ExistsCashTransaction(transactionID.Value, con, tran) Then
                    Throw New Exception("CashTransactionID غير موجود")
                End If

            Else

                ' 🔴 Inventory
                If Not ExistsInventoryTransaction(transactionID.Value, con, tran) Then
                    Throw New Exception("TransactionID غير موجود")
                End If

            End If

        End If
        Using cmd As New SqlCommand("
INSERT INTO gl.AccountLedger
(
    JournalID,
    JournalDetailID,
    AccountID,
    TransactionID,
    LedgerDate,
    DebitAmount,
    CreditAmount,
    RunningBalance,
    Description,
    CreatedAt,
    CreatedBy,
SourceOperationTypeID
)
VALUES
(
    @J,
    @JD,
    @A,
    @T,
    @LD,
    @D,
    @C,
    @RB,      -- 🔥 هنا
    @DESC,
    SYSDATETIME(),
    @U,
@SourceOperationTypeID
)
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@JD", journalDetailID)
            cmd.Parameters.AddWithValue("@A", accountID)
            cmd.Parameters.Add("@T", SqlDbType.Int).Value =
    If(transactionID.HasValue,
       CType(transactionID.Value, Object),
       DBNull.Value)
            cmd.Parameters.AddWithValue("@D", debit)
            cmd.Parameters.AddWithValue("@C", credit)
            cmd.Parameters.AddWithValue("@U", userID)
            cmd.Parameters.AddWithValue("@RB", newBalance)
            If String.IsNullOrEmpty(description) Then

                If transactionID.HasValue Then
                    cmd.Parameters.AddWithValue("@DESC", "TRX-" & transactionID.Value)
                Else
                    cmd.Parameters.AddWithValue("@DESC", "JOURNAL-" & journalID)
                End If

            Else
                cmd.Parameters.AddWithValue("@DESC", "DOC-" & description)
            End If
            cmd.Parameters.AddWithValue("@LD", Date.Today)
            cmd.Parameters.AddWithValue("@SourceOperationTypeID", sourceOperationTypeID)
            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Function IsFinancialPosted(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT IsFinancialPosted
FROM inv.TransactionHeader
WHERE TransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                Return False
            End If

            Return CBool(result)

        End Using

    End Function

    Public Function IsDocumentFinancialPosted(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT COUNT(1)
FROM gl.JournalHeader
WHERE SourceDocumentID = @D
  AND OperationTypeID = 9
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Return CInt(cmd.ExecuteScalar()) > 0

        End Using

    End Function

    Public Function GetJournalTypeByOperation(opTypeID As Integer,
                                          con As SqlConnection,
                                          tran As SqlTransaction) As Integer

        Dim code As String = Nothing

        ' 1️⃣ خذ الكود من OperationType
        Using cmd As New SqlCommand("
SELECT JournalTypeCode
FROM wf.OperationType
WHERE OperationTypeID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", opTypeID)
            code = cmd.ExecuteScalar()?.ToString()

        End Using

        If String.IsNullOrEmpty(code) Then
            Throw New Exception("Operation has no JournalTypeCode")
        End If

        ' 2️⃣ ابحث عنه في JournalTypes
        Using cmd As New SqlCommand("
SELECT JournalTypeID
FROM gl.JournalTypes
WHERE JournalTypeCode = @C
", con, tran)

            cmd.Parameters.AddWithValue("@C", code)

            Dim obj = cmd.ExecuteScalar()

            ' 🔥 3️⃣ إذا غير موجود → أنشئه تلقائيًا
            If obj Is Nothing Then

                Using insertCmd As New SqlCommand("
INSERT INTO gl.JournalTypes
(JournalTypeCode, JournalTypeNameAr, JournalTypeNameEn, IsSystemGenerated, IsActive)
VALUES
(@C, @C, @C, 1, 1);

SELECT SCOPE_IDENTITY();
", con, tran)

                    insertCmd.Parameters.AddWithValue("@C", code)
                    Return CInt(insertCmd.ExecuteScalar())

                End Using

            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GenerateJournalNumber(
    con As SqlConnection,
    tran As SqlTransaction
) As String

        Using cmd As New SqlCommand("gl.GenerateJournalNumber", con, tran)
            cmd.CommandType = CommandType.StoredProcedure

            Dim outParam As New SqlParameter("@NewNumber", SqlDbType.NVarChar, 50)
            outParam.Direction = ParameterDirection.Output
            cmd.Parameters.Add(outParam)

            cmd.ExecuteNonQuery()

            Return outParam.Value.ToString()
        End Using

    End Function
    Public Sub MarkJournalAsPosted(
    journalID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
UPDATE gl.JournalHeader
SET IsPosted = 1,
    PostedAt = SYSDATETIME(),
    PostedBy = @U
WHERE JournalID = @J
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@U", userID)

            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Sub UpdateAccountBalance(
    accountID As Integer,
    periodID As Integer,
    fiscalYearID As Integer,
    debit As Decimal,
    credit As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
MERGE gl.AccountBalances AS T
USING (SELECT @A AS AccountID, @P AS PeriodID) AS S
ON T.AccountID = S.AccountID AND T.PeriodID = S.PeriodID

WHEN MATCHED THEN
    UPDATE SET 
        PeriodDebit = PeriodDebit + @D,
        PeriodCredit = PeriodCredit + @C,
        ClosingDebit = ClosingDebit + @D,
        ClosingCredit = ClosingCredit + @C,
        ClosingBalance = ClosingBalance + (@D - @C),
        LastUpdatedAt = SYSDATETIME()

WHEN NOT MATCHED THEN
    INSERT (
        AccountID,
        FiscalYearID,
        PeriodID,
        OpeningBalance,
        PeriodDebit,
        PeriodCredit,
        ClosingDebit,
        ClosingCredit,
        ClosingBalance,
        LastUpdatedAt
    )
    VALUES (
        @A,
        @FY,
        @P,
        0,
        @D,
        @C,
        @D,
        @C,
        (@D - @C),
        SYSDATETIME()
    );
", con, tran)

            cmd.Parameters.AddWithValue("@A", accountID)
            cmd.Parameters.AddWithValue("@P", periodID)
            cmd.Parameters.AddWithValue("@FY", fiscalYearID)
            cmd.Parameters.AddWithValue("@D", debit)
            cmd.Parameters.AddWithValue("@C", credit)

            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Function GetFiscalYearID(
    periodID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT FiscalYearID
FROM cfg.FiscalPeriod
WHERE PeriodID = @P
", con, tran)

            cmd.Parameters.AddWithValue("@P", periodID)
            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetTransactionDetails(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim dt As New DataTable()

        Using cmd As New SqlCommand("
SELECT 
DetailID, 
    ProductID,
    SourceDocumentDetailID,
    Quantity,
    UnitCost,
    CostAmount,  -- 🔥 هذا الناقص
SourceStoreID,
    TargetStoreID,
    SourceDocumentDetailID,
    ReferenceDetailID
FROM inv.TransactionDetails
WHERE TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

        End Using

        Return dt

    End Function
    Public Function GetScrapTotal(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Decimal

        Using cmd As New SqlCommand("
SELECT SUM(CostAmount)
FROM inv.TransactionDetails
WHERE TransactionID = @T
  AND TargetStoreID IS NOT NULL
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing Then Return 0
            Return CDec(obj)

        End Using

    End Function
    Public Function GetWasteDifference(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As Decimal

        Using cmd As New SqlCommand("
SELECT w.RevaluationDiffValue
FROM inv.WasteHeader w
JOIN inv.TransactionHeader t ON t.SourceDocumentID = w.WasteID
WHERE t.TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing Then Return 0
            Return CDec(obj)

        End Using

    End Function
    Public Function GetInventoryAccountByProduct(
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT g.DefaultInventoryAccountID
FROM md.Product p
JOIN gl.FinancialProductGroup g
    ON g.FinancialGroupID = p.FinancialGroupID
WHERE p.ProductID = @P
", con, tran)

            cmd.Parameters.AddWithValue("@P", productID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("لا يوجد حساب مخزون مربوط بالصنف ProductID=" & productID)
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GetShippingData(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As (Mode As Integer,
      DelNet As Decimal,
      DelTax As Decimal,
      Net As Decimal,
      Tax As Decimal,
      Remaining As Decimal)

        Using cmd As New SqlCommand("
SELECT 
    ShippingMode,
    DeliveryNet,
    DeliveryTax,
    TotalNetAmount,
    TotalTax,
    RemainingAmount
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then

                    Dim mode As Integer = If(IsDBNull(rd("ShippingMode")), 1, CInt(rd("ShippingMode")))
                    Dim delNet As Decimal = If(IsDBNull(rd("DeliveryNet")), 0D, CDec(rd("DeliveryNet")))
                    Dim delTax As Decimal = If(IsDBNull(rd("DeliveryTax")), 0D, CDec(rd("DeliveryTax")))
                    Dim net As Decimal = If(IsDBNull(rd("TotalNetAmount")), 0D, CDec(rd("TotalNetAmount")))
                    Dim tax As Decimal = If(IsDBNull(rd("TotalTax")), 0D, CDec(rd("TotalTax")))
                    Dim remaining As Decimal = If(IsDBNull(rd("RemainingAmount")), 0D, CDec(rd("RemainingAmount")))

                    Return (mode, delNet, delTax, net, tax, remaining)

                End If
            End Using
        End Using

        ' 🔥 fallback آمن
        Return (1, 0D, 0D, 0D, 0D, 0D)

    End Function
    Public Function GetPostingRuleHeaderByCode(
    code As String,
    con As SqlConnection,
    tran As SqlTransaction
) As PostingRuleHeader

        Using cmd As New SqlCommand("
SELECT TOP 1 *
FROM gl.PostingRuleHeader
WHERE RuleCode = @C
  AND IsActive = 1
", con, tran)

            cmd.Parameters.AddWithValue("@C", code)

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    Dim r As New PostingRuleHeader()
                    r.PostingRuleHeaderID = CInt(rd("PostingRuleHeaderID"))
                    Return r
                End If
            End Using
        End Using

        Return Nothing


    End Function
    Public Function GetAccountID_ByDocument(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT p.AccountID
FROM inv.DocumentHeader d
JOIN md.Partner p ON d.PartnerID = p.PartnerID
WHERE d.DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                Return CInt(obj)
            End If
        End Using

        Return 0
    End Function
    Public Function GetDocumentType(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As String

        Using cmd As New SqlCommand("
SELECT DocumentType
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("❌ DocumentType غير موجود للوثيقة رقم " & documentID)
            End If

            Return obj.ToString().Trim().ToUpper()

        End Using

    End Function
    Public Function GetDocumentDetails(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim dt As New DataTable()

        Using cmd As New SqlCommand("
SELECT
    DetailID,
    ProductID,
    Quantity,
    UnitPrice,
    LineTotal,
    DiscountAmount,
    NetAmount,
    TaxAmount,
    GrossAmount
FROM inv.DocumentDetails
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt

    End Function
    Public Function GetTotalTaxByDocument(
        documentID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalTax
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 0D
            End If

            Return CDec(obj)

        End Using

    End Function
    Public Function GetDocumentNet(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalNetAmount
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 0D
            End If

            Return CDec(obj)

        End Using

    End Function
    Public Function GetGoodsAtCustomerAccountByProduct(
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT g.GoodsInTransitAccountID
FROM md.Product p
JOIN gl.FinancialProductGroup g
    ON g.FinancialGroupID = p.FinancialGroupID
WHERE p.ProductID = @P
", con, tran)

            cmd.Parameters.AddWithValue("@P", productID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("لا يوجد حساب بضاعة لدى العميل مربوط بالصنف ProductID=" & productID)
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GetCOGSAccountByProduct(
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT g.DefaultCOGSAccountID
FROM md.Product p
JOIN gl.FinancialProductGroup g
    ON g.FinancialGroupID = p.FinancialGroupID
WHERE p.ProductID = @P
", con, tran)

            cmd.Parameters.AddWithValue("@P", productID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("لا يوجد حساب تكلفة مربوط بالصنف ProductID=" & productID)
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GetPeriodID_ByJournal(
    journalID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT PeriodID
FROM gl.JournalHeader
WHERE JournalID = @J
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetPaymentAmount(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT TotalAmount
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 0D
            End If

            Return CDec(obj)

        End Using

    End Function
    Public Function GetCashAccount(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT CashAccountID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)
            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetPartnerAccount_FromPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT 
    t.PartnerID,
    p.PartnerName,
    p.AccountID
FROM fin.CashTransactionHeader t
LEFT JOIN md.Partner p ON p.PartnerID = t.PartnerID
WHERE t.CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Using rd = cmd.ExecuteReader()

                If Not rd.Read() Then
                    Throw New Exception("السند المالي غير موجود.")
                End If

                If IsDBNull(rd("PartnerID")) Then
                    Throw New Exception("لا يوجد شريك مرتبط بهذا السند.")
                End If

                If IsDBNull(rd("AccountID")) Then
                    Dim partnerName As String =
                    If(IsDBNull(rd("PartnerName")), "غير معروف", rd("PartnerName").ToString())

                    Throw New Exception(
                    "الشريك [" & partnerName & "] لا يوجد له حساب محاسبي مرتبط." &
                    vbCrLf &
                    "يجب إنشاء/ربط حساب الشريك أولاً من شاشة العملاء/الموردين."
                )
                End If

                Return CInt(rd("AccountID"))

            End Using

        End Using

    End Function
    Public Function GetPaymentDirection(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT Direction
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Return 2 ' default OUT
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GetOperationCode(
        opTypeID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As String

        Using cmd As New SqlCommand("
    SELECT OperationCode
    FROM wf.OperationType
    WHERE OperationTypeID = @ID
    ", con, tran)

            cmd.Parameters.AddWithValue("@ID", opTypeID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("OperationType غير موجود")
            End If

            Return obj.ToString()

        End Using

    End Function
    Public Function GetCashOperationByCode(
    operationCode As String,
    con As SqlConnection,
    tran As SqlTransaction
) As CashOperationInfo

        Using cmd As New SqlCommand("
SELECT 
    OperationTypeID,
    OperationCode,
    CashDirection
FROM wf.OperationType
WHERE OperationCode = @Code
  AND IsActive = 1
", con, tran)

            cmd.Parameters.AddWithValue("@Code", operationCode)

            Using rd = cmd.ExecuteReader()

                If rd.Read() Then

                    If IsDBNull(rd("CashDirection")) Then
                        Throw New Exception("CashDirection غير محدد للعملية: " & operationCode)
                    End If

                    Return New CashOperationInfo With {
                        .OperationTypeID = CInt(rd("OperationTypeID")),
                        .OperationCode = rd("OperationCode").ToString(),
                        .CashDirection = CInt(rd("CashDirection"))
                    }

                End If

            End Using

        End Using

        Throw New Exception("OperationCode غير موجود أو غير فعال: " & operationCode)

    End Function
    Public Function GetOperationType_FromPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("❌ نوع العملية غير موجود في السند")
            End If

            Return CInt(obj)

        End Using

    End Function
    Public Function GetPaymentNetAmount(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT NetAmount
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D

            Return CDec(obj)
        End Using

    End Function
    Public Function GetPaymentTaxAmount(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT TaxAmount
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D

            Return CDec(obj)
        End Using

    End Function
    Public Function GetBeneficiaryAccount_FromPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT BeneficiaryAccountID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("لم يتم تحديد الحساب المستفيد في سند الصرف.")
            End If

            Return CInt(obj)
        End Using

    End Function
    Public Function GetInputTaxAccount_FromPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT tt.TaxAccountID
FROM fin.CashTransactionHeader h
JOIN md.TaxType tt ON tt.TaxTypeID = h.TaxTypeID
WHERE h.CashTransactionID = @ID
  AND tt.TaxDirection = 1
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("لم يتم تحديد حساب ضريبة المدخلات في جدول TaxType.")
            End If

            Return CInt(obj)
        End Using

    End Function
    Public Function GetJournalTypeID_ByCode(code As String,
                                        con As SqlConnection,
                                        tran As SqlTransaction) As Integer

        Using cmd As New SqlCommand("
        SELECT JournalTypeID
        FROM gl.JournalTypes
        WHERE JournalTypeCode = @Code
    ", con, tran)

            cmd.Parameters.AddWithValue("@Code", code)

            Return CInt(cmd.ExecuteScalar())
        End Using

    End Function
    Public Function GetRevaluationData(runId As Long,
                                   con As SqlConnection,
                                   tran As SqlTransaction) As DataTable

        Dim dt As New DataTable()

        Using cmd As New SqlCommand("
        SELECT
            ProductID,
            StoreID,
            SUM(InventoryDifference) AS TotalDiff
        FROM inv.CostEngineResult
        WHERE RunID = @RunID
          AND IsChanged = 1
        GROUP BY ProductID, StoreID
        HAVING ABS(SUM(InventoryDifference)) > 0.0001
    ", con, tran)

            cmd.Parameters.AddWithValue("@RunID", runId)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

        End Using

        Return dt

    End Function
    Public Sub LinkCostLedgerToJournal(
    transactionID As Integer,
    journalID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        Using cmd As New SqlCommand("
UPDATE inv.CostLedger
SET FinancialJournalID = @J,
    GLPostedCost = InTotalCost
WHERE TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@T", transactionID)

            cmd.ExecuteNonQuery()
        End Using
    End Sub
    Public Sub LinkTransactionToJournal(
    transactionID As Integer,
    journalID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
UPDATE inv.TransactionHeader
SET FinancialJournalID = @J
WHERE TransactionID = @T
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@T", transactionID)

            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Sub LinkCashTransactionToJournal(
    cashTransactionID As Integer,
    journalID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
UPDATE fin.CashTransactionHeader
SET JournalID = @J
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@J", journalID)
            cmd.Parameters.AddWithValue("@ID", cashTransactionID)

            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Public Function ExistsInventoryTransaction(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT 1
FROM inv.TransactionHeader
WHERE TransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Return cmd.ExecuteScalar() IsNot Nothing

        End Using

    End Function
    Public Function ExistsCashTransaction(
    cashTransactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT 1
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", cashTransactionID)

            Return cmd.ExecuteScalar() IsNot Nothing

        End Using

    End Function
    Public Function GetOperationTypeID_ForPayment(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Return CInt(cmd.ExecuteScalar())

        End Using

    End Function
    Public Function GetCashTransactionDetails(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Using cmd As New SqlCommand("
        SELECT *
        FROM fin.CashTransactionDetails
        WHERE CashTransactionID = @ID
        ORDER BY LineNumber
    ", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Dim dt As New DataTable()
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

            Return dt

        End Using

    End Function



End Class
