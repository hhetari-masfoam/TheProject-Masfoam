Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ListView

Public Class FinPostingService

    Private ReadOnly _repo As FinPostingRepository

    Public Sub New()
        _repo = New FinPostingRepository()
    End Sub

    Public Sub Post(
    transactionID As Integer,
    documentID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        ' 👇 استدعاء الدالة القديمة
        Post(transactionID, userID, con, tran)
    End Sub
    Public Sub Post(
    transactionID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction,
    Optional documentID As Integer = 0
)
        Dim docID As Integer = documentID

        If docID = 0 Then
            Using cmd As New SqlCommand("
SELECT SourceDocumentID
FROM inv.TransactionHeader
WHERE TransactionID = @T
", con, tran)

                cmd.Parameters.AddWithValue("@T", transactionID)

                Dim obj = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    docID = CInt(obj)
                End If
            End Using
        End If

        ' =========================================
        ' 🔒 منع الترحيل المكرر
        ' =========================================
        If docID > 0 Then
            If _repo.IsDocumentFinancialPosted(docID, con, tran) Then Exit Sub
        Else
            If _repo.IsFinancialPosted(transactionID, con, tran) Then Exit Sub
        End If

        ' =========================================
        ' 1️⃣ تحديد نوع العملية
        ' =========================================
        Dim opTypeID = _repo.GetOperationTypeID(transactionID, con, tran)

        Dim isInvoiceFlow As Boolean = False
        Dim docType As String = ""

        If docID > 0 Then
            Try
                docType = _repo.GetDocumentType(docID, con, tran)

                Select Case docType
                    Case "SAL"
                        opTypeID = 9
                        isInvoiceFlow = True

                    Case "SRT"
                        opTypeID = 12
                        isInvoiceFlow = False

                    Case "PRT"
                        opTypeID = 14
                        isInvoiceFlow = True
                End Select

            Catch ex As Exception
                ' ليس Document مالي → تجاهل
            End Try
        End If

        ' =========================================
        ' 🔥 مسار الفواتير (SAL / SRT / PRT)
        ' =========================================
        If isInvoiceFlow Then
            PostSalesDocument(docID, transactionID, userID, opTypeID, con, tran)
            Return
        End If

        ' =========================================
        ' 🧠 Shipping Layer
        ' =========================================
        Dim ship = _repo.GetShippingData(docID, con, tran)

        Dim net = ship.Net
        Dim tax = ship.Tax
        Dim delNet = ship.DelNet
        Dim delTax = ship.DelTax
        Dim mode = ship.Mode
        Dim remaining = ship.Remaining

        If mode = 4 Then
            net -= delNet
            tax -= delTax
        End If
        ' =========================================
        ' 🎯 Override Values
        ' =========================================
        Dim overrideValues As New Dictionary(Of Integer, Decimal)
        overrideValues(2) = net
        overrideValues(3) = tax
        overrideValues(11) = remaining

        If mode = 4 Then
            overrideValues(12) = 0D
            overrideValues(13) = 0D
        Else
            overrideValues(12) = delNet
            overrideValues(13) = delTax
        End If

        If mode = 4 Then
            overrideValues(14) = net + tax + delNet + delTax
        Else
            overrideValues(14) = remaining
        End If
        Dim totalTax As Decimal = _repo.GetTotalTaxByDocument(docID, con, tran)
        ' =========================================
        ' 2️⃣ Posting Rules
        ' =========================================
        Dim rules As New List(Of PostingRuleHeader)

        Dim rule = _repo.GetPostingRuleHeader(opTypeID, con, tran)

        If rule IsNot Nothing Then rules.Add(rule)

        If rules.Count = 0 Then Exit Sub

        ' =========================================
        ' 3️⃣ تجميع القيود
        ' =========================================
        Dim lines As New List(Of JournalLine)
        Dim transDetails = _repo.GetTransactionDetails(transactionID, con, tran)

        For Each rRule In rules

            Dim details = _repo.GetPostingRuleDetails(rRule.PostingRuleHeaderID, con, tran)

            If details Is Nothing OrElse details.Count = 0 Then Continue For

            For Each d In details
                If Not d.IsDistributed Then

                    Dim amount = ResolveHeaderAmount(
                    transactionID,
                    d.SourceAmountFieldID,
                    con,
                    tran,
                    overrideValues
                )
                    If amount = 0 Then Continue For

                    Dim accountID = ResolveAccount(transactionID, d, con, tran)
                    If accountID <= 0 Then Continue For

                    Dim line As New JournalLine With {
                    .AccountID = accountID
                }

                    If d.AccountSourceTypeID = 3 Then
                        Dim partnerId = _repo.GetPartnerID(transactionID, con, tran)
                        If partnerId > 0 Then line.PartnerID = partnerId
                    End If

                    If d.SourceAmountFieldID = 9 Then ' 🔥 فرق الهالك

                        If Math.Round(amount, 5) = 0D Then Continue For

                        If amount > 0 Then
                            line.DebitAmount = amount
                            line.CreditAmount = 0
                        Else
                            line.DebitAmount = 0
                            line.CreditAmount = Math.Abs(amount)
                        End If

                    Else
                        If d.EntrySideID = 1 Then
                            line.DebitAmount = amount
                        Else
                            line.CreditAmount = amount
                        End If
                    End If

                    lines.Add(line)

                Else

                    For Each r As DataRow In transDetails.Rows

                        Dim amount = ResolveAmountPerDetail(r, d.SourceAmountFieldID, con, tran)
                        If amount = 0 Then Continue For

                        Dim accountID As Integer

                        If d.AccountSourceTypeID = 2 Then

                            accountID = _repo.GetInventoryAccountByProduct(CInt(r("ProductID")), con, tran)

                        ElseIf d.AccountSourceTypeID = 4 Then

                            accountID = _repo.GetCOGSAccountByProduct(CInt(r("ProductID")), con, tran)

                        ElseIf d.AccountSourceTypeID = 5 Then

                            accountID = _repo.GetGoodsAtCustomerAccountByProduct(CInt(r("ProductID")), con, tran)

                        Else

                            accountID = ResolveAccount(transactionID, d, con, tran)

                        End If

                        If accountID <= 0 Then Continue For

                        Dim line As New JournalLine With {
                        .AccountID = accountID
                    }

                        If d.AccountSourceTypeID = 3 Then
                            Dim partnerId = _repo.GetPartnerID(transactionID, con, tran)
                            If partnerId > 0 Then line.PartnerID = partnerId
                        End If

                        If d.SourceAmountFieldID = 9 Then ' 🔥 فرق الهالك

                            If Math.Round(amount, 5) = 0D Then Continue For

                            If amount > 0 Then
                                line.DebitAmount = amount
                                line.CreditAmount = 0
                            Else
                                line.DebitAmount = 0
                                line.CreditAmount = Math.Abs(amount)
                            End If

                        Else
                            If d.EntrySideID = 1 Then
                                line.DebitAmount = amount
                            Else
                                line.CreditAmount = amount
                            End If
                        End If

                        If Not IsDBNull(r("ProductID")) Then
                            line.ProductID = CInt(r("ProductID"))
                        End If
                        line.SourceDetailID = If(IsDBNull(r("SourceDocumentDetailID")), Nothing, CInt(r("SourceDocumentDetailID")))
                        line.TransactionDetailID = If(IsDBNull(r("DetailID")), Nothing, CInt(r("DetailID")))
                        If Not IsDBNull(r("TargetStoreID")) Then
                            line.StoreID = CInt(r("TargetStoreID"))
                        ElseIf Not IsDBNull(r("SourceStoreID")) Then
                            line.StoreID = CInt(r("SourceStoreID"))
                        Else
                            line.StoreID = Nothing
                        End If
                        lines.Add(line)

                    Next

                End If

            Next

        Next
        If opTypeID = 12 AndAlso totalTax <> 0 Then

            Dim taxAccount As Integer = 19 ' أو من rule

            Dim taxLine As New JournalLine With {
        .AccountID = taxAccount,
        .DebitAmount = totalTax
    }

            lines.Add(taxLine)

        End If
        ' =========================================
        ' 4️⃣ التحقق من التوازن
        ' =========================================
        ValidateJournalBalance(lines)

        Dim totalDebit = lines.Sum(Function(x) x.DebitAmount)
        Dim totalCredit = lines.Sum(Function(x) x.CreditAmount)

        ' =========================================
        ' 5️⃣ إنشاء Journal
        ' =========================================
        Dim journalTypeID = _repo.GetJournalTypeByOperation(opTypeID, con, tran)

        Dim result = _repo.InsertJournalHeader(
        transactionID,
        journalTypeID,
        userID,
        totalDebit,
        totalCredit,
        opTypeID,
        docID,
        con,
        tran
    )
        Dim journalID = result.JournalID
        Dim refNo = result.RefNo
        Dim periodID = _repo.GetPeriodID_ByJournal(journalID, con, tran)
        Dim fiscalYearID = _repo.GetFiscalYearID(periodID, con, tran)
        Dim operationTypeID = opTypeID
        ' =========================================
        ' 6️⃣ التفاصيل
        ' =========================================
        Dim lineNo As Integer = 1

        For Each l In lines
            Dim sourceDetailID As Integer? = Nothing
            Dim transactionDetailID As Integer? = Nothing
            Dim storeID As Integer? = Nothing

            If transDetails.Rows.Count = 1 Then
                Dim r = transDetails.Rows(0)

                sourceDetailID = If(IsDBNull(r("SourceDocumentDetailID")), Nothing, CInt(r("SourceDocumentDetailID")))
                transactionDetailID = If(IsDBNull(r("DetailID")), Nothing, CInt(r("DetailID")))
                storeID = If(IsDBNull(r("TargetStoreID")), Nothing, CInt(r("TargetStoreID")))
            End If



            If Math.Round(l.DebitAmount, 5) = 0D AndAlso Math.Round(l.CreditAmount, 5) = 0D Then
                Continue For
            End If
            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("خطأ: لا يمكن أن يكون السطر مدين ودائن معاً")
            End If
            If l.DebitAmount <= 0 AndAlso l.CreditAmount <= 0 Then
                Throw New Exception("سطر صفر - AccountID: " & l.AccountID)
            End If

            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("سطر مزدوج - AccountID: " & l.AccountID)
            End If
            If l.SourceDetailID Is Nothing Then
                l.SourceDetailID = l.ReferenceDetailID
            End If
            Dim detailID = _repo.InsertJournalDetail(
                    journalID,
                    l.AccountID,
                    l.DebitAmount,
                    l.CreditAmount,
                    lineNo,
                    l.PartnerID,
                    l.SourceDetailID,        ' 🔥
                    Nothing,
                    l.ProductID,
                    l.TransactionDetailID,   ' 🔥
                    userID,
                    con,
                    tran,
                    l.StoreID               ' 🔥
                )

            lineNo += 1

            _repo.InsertAccountLedger(
            journalID,
            detailID,
            l.AccountID,
            transactionID,
            l.DebitAmount,
            l.CreditAmount,
            userID,
            con,
            tran,
            docID,
            opTypeID
        )
            _repo.UpdateAccountBalance(
                l.AccountID,
                periodID,
                fiscalYearID,
                l.DebitAmount,
                l.CreditAmount,
                con,
                tran
            )
        Next

        _repo.MarkJournalAsPosted(journalID, userID, con, tran)
        _repo.LinkCostLedgerToJournal(transactionID, journalID, con, tran)
        _repo.MarkTransactionAsFinancialPosted(transactionID, con, tran)
        _repo.LinkTransactionToJournal(transactionID, journalID, con, tran)

    End Sub
    Public Sub PostPayment(
    transactionID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim opTypeID As Integer = _repo.GetOperationType_FromPayment(transactionID, con, tran)



        Dim rule = _repo.GetPostingRuleHeader(opTypeID, con, tran)

        If rule Is Nothing Then
            Throw New Exception("لا توجد قاعدة ترحيل مفعلة للعملية PAY_GEN.")
        End If
        If _repo.IsFinancialPosted(transactionID, con, tran) Then
            Throw New Exception("⚠️ السند مرحل مسبقاً")
        End If

        Dim transDetails = _repo.GetCashTransactionDetails(transactionID, con, tran)

        If transDetails.Rows.Count = 0 Then
            Throw New Exception("❌ لا توجد تفاصيل للسند")
        End If

        Dim lines As New List(Of JournalLine)

        For Each r As DataRow In transDetails.Rows

            Dim debit = If(IsDBNull(r("DebitAmount")), 0D, CDec(r("DebitAmount")))
            Dim credit = If(IsDBNull(r("CreditAmount")), 0D, CDec(r("CreditAmount")))

            If Math.Round(debit, 5) = 0D AndAlso Math.Round(credit, 5) = 0D Then Continue For

            Dim accountID As Integer = CInt(r("AccountID"))

            If accountID <= 0 Then
                Throw New Exception("❌ AccountID غير صحيح في التفاصيل")
            End If

            Dim line As New JournalLine With {
        .AccountID = accountID,
        .DebitAmount = debit,
        .CreditAmount = credit
    }

            ' 🔵 Partner
            If Not IsDBNull(r("PartnerID")) Then
                line.PartnerID = CInt(r("PartnerID"))
            End If

            lines.Add(line)

        Next






        ValidateJournalBalance(lines)

        Dim totalDebit As Decimal = lines.Sum(Function(x) x.DebitAmount)
        Dim totalCredit As Decimal = lines.Sum(Function(x) x.CreditAmount)

        Dim journalTypeID As Integer = _repo.GetJournalTypeByOperation(opTypeID, con, tran)

        Dim result = _repo.InsertJournalHeader(
            transactionID,
            journalTypeID,
            userID,
            totalDebit,
            totalCredit,
            opTypeID,
            0,
            con,
            tran
        )
        Dim journalID = result.JournalID
        Dim refNo = result.RefNo
        Dim periodID As Integer = _repo.GetPeriodID_ByJournal(journalID, con, tran)
        Dim fiscalYearID As Integer = _repo.GetFiscalYearID(periodID, con, tran)
        Dim lineNo As Integer = 1

        For Each l In lines
            Debug.Print("LINE AccountID: " & l.AccountID &
                " Debit=" & l.DebitAmount &
                " Credit=" & l.CreditAmount)
            If Not lines.Any(Function(x) x.AccountID = _repo.GetCashAccount(transactionID, con, tran)) Then
                Throw New Exception("❌ لا يوجد حساب كاش في القيود")
            End If

            If Math.Round(l.DebitAmount, 5) = 0D AndAlso Math.Round(l.CreditAmount, 5) = 0D Then
                Continue For
            End If
            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("خطأ: لا يمكن أن يكون السطر مدين ودائن معاً")
            End If
            If l.DebitAmount <= 0 AndAlso l.CreditAmount <= 0 Then
                Throw New Exception("سطر صفر - AccountID: " & l.AccountID)
            End If

            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("سطر مزدوج - AccountID: " & l.AccountID)
            End If
            Dim detailID = _repo.InsertJournalDetail(
                journalID,
                l.AccountID,
                l.DebitAmount,
                l.CreditAmount,
                lineNo,
                If(l.PartnerID.HasValue AndAlso l.PartnerID.Value > 0,
                   l.PartnerID,
                   Nothing),
                Nothing,
                Nothing,
                Nothing,
                Nothing,
                userID,
                con,
                tran
            )

            _repo.InsertAccountLedger(
                journalID,
                detailID,
                l.AccountID,
                transactionID,
                l.DebitAmount,
                l.CreditAmount,
                userID,
                con,
                tran,
                Nothing,
                opTypeID ' 🔥 هذا هو المطلوب
            )

            _repo.UpdateAccountBalance(
                l.AccountID,
                periodID,
                fiscalYearID,
                l.DebitAmount,
                l.CreditAmount,
                con,
                tran
            )

            lineNo += 1

        Next

        _repo.MarkJournalAsPosted(journalID, userID, con, tran)
        _repo.MarkTransactionAsFinancialPosted(transactionID, con, tran)
        _repo.LinkCashTransactionToJournal(transactionID, journalID, con, tran)
    End Sub



    Private Function ResolveAmount(
    transactionID As Integer,
    sourceAmountFieldID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Dim opTypeID As Integer = _repo.GetOperationType_FromPayment(transactionID, con, tran)
        Dim opCode As String = _repo.GetOperationCode(opTypeID, con, tran)

        If opCode.Trim().ToUpper() = "PAY_GEN" Then

            Select Case sourceAmountFieldID

                Case 1 ' TOTAL
                    Return _repo.GetPaymentAmount(transactionID, con, tran)

                Case 2 ' NET
                    Return _repo.GetPaymentNetAmount(transactionID, con, tran)

                Case 3 ' TAX
                    Return _repo.GetPaymentTaxAmount(transactionID, con, tran)

            End Select

        End If

        Select Case sourceAmountFieldID

            Case 1
                Return _repo.GetTotal(transactionID, con, tran)

            Case 2
                Return _repo.GetNet(transactionID, con, tran)

            Case 3
                Return _repo.GetTax(transactionID, con, tran)

            Case 8
                Return _repo.GetScrapTotal(transactionID, con, tran)

            Case 9
                Return _repo.GetWasteDifference(transactionID, con, tran)

            Case 11
                Return _repo.GetGrandTotal(transactionID, con, tran)

            Case Else
                Throw New Exception("Unsupported SourceAmountFieldID: " & sourceAmountFieldID)

        End Select

    End Function

    Public Enum AccountSourceType

        Fixed = 1
        ProductInventory = 2
        Partner = 3

        ' 🔥 هذا المهم
        Payment_Beneficiary = 6
        Payment_Cash = 7
        Payment_InputTax = 8

    End Enum
    Private Function ResolveAccount(
    transactionID As Integer,
    d As PostingRuleDetail,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Select Case CType(d.AccountSourceTypeID, AccountSourceType)

            Case AccountSourceType.Fixed
                Return d.FixedAccountID

            Case AccountSourceType.ProductInventory
                Return _repo.GetInventoryAccount(transactionID, con, tran)

            Case AccountSourceType.Partner
                Return _repo.GetPartnerAccount(transactionID, con, tran)

            Case AccountSourceType.Payment_Beneficiary
                Return _repo.GetBeneficiaryAccount_FromPayment(transactionID, con, tran)

            Case AccountSourceType.Payment_Cash
                Return _repo.GetCashAccount(transactionID, con, tran)

            Case AccountSourceType.Payment_InputTax
                Return _repo.GetInputTaxAccount_FromPayment(transactionID, con, tran)

            Case Else
                Throw New Exception("Unsupported AccountSourceTypeID")

        End Select

    End Function
    Private Sub ValidateJournalBalance(lines As List(Of JournalLine))

        Dim totalDebitAmount = lines.Sum(Function(x) x.DebitAmount)
        Dim totalCreditAmount = lines.Sum(Function(x) x.CreditAmount)


        If Math.Round(totalDebitAmount, 2) <> Math.Round(totalCreditAmount, 2) Then
            Throw New Exception("القيد غير متوازن")
        End If

    End Sub
    Private Function ResolveAmountPerDetail(
    r As DataRow,
    sourceFieldID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Select Case sourceFieldID

            Case 1 ' Quantity
                Return CDec(r("Quantity"))

            Case 2 ' Net / Cost
                Return CDec(r("CostAmount"))

            Case 3 ' Tax

                ' فقط إذا موجود في الترانسكشن
                If r.Table.Columns.Contains("TaxAmount") AndAlso Not IsDBNull(r("TaxAmount")) Then
                    Return CDec(r("TaxAmount"))
                End If

                Return 0D


            Case 7 ' OUT فقط
                If Not IsDBNull(r("SourceStoreID")) Then
                    Return CDec(r("CostAmount"))
                Else
                    Return 0D
                End If
            Case 8 ' Scrap IN فقط
                If Not IsDBNull(r("TargetStoreID")) Then
                    Return CDec(r("CostAmount"))
                Else
                    Return 0D
                End If
            Case Else
                Return 0

        End Select

    End Function
    Private Function ResolveHeaderAmount(
    transactionID As Integer,
    sourceFieldID As Integer,
    con As SqlConnection,
    tran As SqlTransaction,
    overrideValues As Dictionary(Of Integer, Decimal)
) As Decimal

        ' ✅ 1) override أولاً
        If overrideValues IsNot Nothing AndAlso overrideValues.ContainsKey(sourceFieldID) Then
            Return overrideValues(sourceFieldID)
        End If

        ' ✅ 2) fallback من الفاتورة
        Select Case sourceFieldID

            Case 2 ' NET
                Return _repo.GetNet(transactionID, con, tran)

            Case 3 ' TAX
                Return _repo.GetTax(transactionID, con, tran)
            Case 9

                Dim opTypeID As Integer = _repo.GetOperationTypeID(transactionID, con, tran)

                ' 🔴 SCR (الهالك)
                If opTypeID = 13 Then

                    Dim totalOut As Decimal = 0D
                    Dim totalIn As Decimal = 0D

                    Dim transDetails = _repo.GetTransactionDetails(transactionID, con, tran)

                    For Each r As DataRow In transDetails.Rows

                        Dim cost = If(IsDBNull(r("CostAmount")), 0D, CDec(r("CostAmount")))

                        If Not IsDBNull(r("SourceStoreID")) Then
                            totalOut += cost
                        End If

                        If Not IsDBNull(r("TargetStoreID")) Then
                            totalIn += cost
                        End If

                    Next

                    Return totalOut - totalIn

                End If


                ' 🔵 COR (تصحيح التكلفة)
                If opTypeID = 2 Then

                    Dim totalOut As Decimal = 0D
                    Dim totalIn As Decimal = 0D

                    Dim transDetails = _repo.GetTransactionDetails(transactionID, con, tran)

                    For Each r As DataRow In transDetails.Rows

                        Dim cost = If(IsDBNull(r("CostAmount")), 0D, CDec(r("CostAmount")))

                        If Not IsDBNull(r("SourceStoreID")) Then
                            totalOut += cost
                        End If

                        If Not IsDBNull(r("TargetStoreID")) Then
                            totalIn += cost
                        End If

                    Next

                    Return totalOut - totalIn

                End If

                ' 🔴 أي عملية أخرى
                Throw New Exception("FieldID=9 غير مدعوم لهذه العملية")
            Case Else
                Return 0D

        End Select

    End Function
    Public Class JournalLine
        Public Property AccountID As Integer
        Public Property DebitAmount As Decimal
        Public Property CreditAmount As Decimal

        Public Property PartnerID As Integer?
        Public Property ProductID As Integer?        ' 🔥 جديد
        Public Property SourceDetailID As Integer?   ' 🔥 جديد
        Public Property ReferenceDetailID As Integer?
        Public Property TransactionDetailID As Integer?
        Public Property StoreID As Integer?
    End Class
    Private Sub PostSalesDocument(
    documentID As Integer,
    transactionID As Integer,
    userID As Integer,
    opTypeID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        ' =========================================
        ' 🔒 منع التكرار (Document Level)
        ' =========================================
        If _repo.IsDocumentFinancialPosted(documentID, con, tran) Then Exit Sub

        ' =========================================
        ' 🧠 Shipping
        ' =========================================
        Dim ship = _repo.GetShippingData(documentID, con, tran)

        Dim net = ship.Net
        Dim tax = ship.Tax
        Dim delNet = ship.DelNet
        Dim delTax = ship.DelTax
        Dim mode = ship.Mode
        Dim remaining = ship.Remaining



        ' 🔥 Mode 4: الشحن ضمن الأصناف
        ' نفصل الشحن من قيمة البضاعة والضريبة
        If mode = 4 Then
            net -= delNet
            tax -= delTax
        End If

        ' =========================================
        ' 🎯 Override Values
        ' =========================================
        Dim overrideValues As New Dictionary(Of Integer, Decimal)

        overrideValues(2) = net        ' إيراد البضاعة بعد فصل الشحن
        overrideValues(3) = tax        ' ضريبة البضاعة بعد فصل ضريبة الشحن
        overrideValues(12) = delNet    ' إيراد الشحن
        overrideValues(13) = delTax    ' ضريبة الشحن

        If mode = 4 Then
            ' العميل يتحمل الإجمالي الحقيقي بعد التوزيع، لا Remaining
            overrideValues(11) = net + tax + delNet + delTax
            overrideValues(14) = net + tax + delNet + delTax
        Else
            overrideValues(11) = remaining
            overrideValues(14) = remaining
        End If

        ' =========================================
        ' 📜 Rules (SAL + DEL)
        ' =========================================
        Dim rules As New List(Of PostingRuleHeader)

        Dim docType = _repo.GetDocumentType(documentID, con, tran)
        Dim rule = _repo.GetPostingRuleHeaderByCode(docType, con, tran)
        If rule IsNot Nothing Then rules.Add(rule)

        If mode <> 1 Then
            Dim delRule = _repo.GetPostingRuleHeaderByCode("DEL", con, tran)
            If delRule IsNot Nothing Then rules.Add(delRule)
        End If

        If rules.Count = 0 Then Exit Sub

        ' =========================================
        ' 🧾 Lines
        ' =========================================
        Dim lines As New List(Of JournalLine)

        For Each rRule In rules

            Dim details = _repo.GetPostingRuleDetails(rRule.PostingRuleHeaderID, con, tran)

            For Each d In details
                If d.AccountSourceTypeID = 3 Then
                End If
                Dim amount As Decimal

                If d.IsDistributed Then

                    ' 🔥 توزيع على تفاصيل الفاتورة
                    Dim detailRows As DataTable

                    If d.AccountSourceTypeID = 4 OrElse d.AccountSourceTypeID = 5 Then
                        detailRows = _repo.GetTransactionDetails(transactionID, con, tran)
                    Else
                        detailRows = _repo.GetDocumentDetails(documentID, con, tran)
                    End If

                    For Each r As DataRow In detailRows.Rows

                        Dim detailAmount = ResolveAmountPerDetail(r, d.SourceAmountFieldID, con, tran)

                        If detailAmount = 0 Then Continue For

                        Dim detailAccountID As Integer = 0

                        If d.AccountSourceTypeID = 4 Then

                            detailAccountID = _repo.GetCOGSAccountByProduct(CInt(r("ProductID")), con, tran)

                        ElseIf d.AccountSourceTypeID = 5 Then

                            detailAccountID = _repo.GetGoodsAtCustomerAccountByProduct(CInt(r("ProductID")), con, tran)

                        Else

                            detailAccountID = ResolveAccount(documentID, d, con, tran)

                        End If
                        If detailAccountID <= 0 Then Continue For

                        Dim detailLine As New JournalLine With {
            .AccountID = detailAccountID
        }

                        If d.AccountSourceTypeID = 3 Then
                            Dim p = _repo.GetPartnerID_ByDocument(documentID, con, tran)
                            If p > 0 Then detailLine.PartnerID = p
                        End If

                        If d.EntrySideID = 1 Then
                            detailLine.DebitAmount = detailAmount
                        Else
                            detailLine.CreditAmount = detailAmount
                        End If

                        lines.Add(detailLine)

                    Next

                    Continue For

                Else
                    amount = ResolveHeaderAmount(
        documentID,
        d.SourceAmountFieldID,
        con,
        tran,
        overrideValues
    )
                End If
                If d.AccountSourceTypeID = 3 Then
                End If
                If amount = 0 Then Continue For

                Dim accountID As Integer = 0

                If d.AccountSourceTypeID = 3 Then

                    accountID = _repo.GetAccountID_ByDocument(documentID, con, tran)

                    If accountID <= 0 Then
                        Throw New Exception("❌ لا يوجد حساب للعميل مربوط بالفاتورة DocumentID=" & documentID)
                    End If

                ElseIf d.FixedAccountID > 0 Then

                    accountID = d.FixedAccountID

                End If

                If accountID <= 0 Then Continue For

                Dim line As New JournalLine With {
                    .AccountID = accountID
                }

                If d.AccountSourceTypeID = 3 Then
                    Dim p = _repo.GetPartnerID_ByDocument(documentID, con, tran)
                    If p > 0 Then line.PartnerID = p
                End If
                If d.EntrySideID = 1 Then
                    line.DebitAmount = amount
                Else
                    line.CreditAmount = amount
                End If

                lines.Add(line)

            Next
        Next

        ' =========================================
        ' ✅ Validate
        ' =========================================
        ValidateJournalBalance(lines)

        Dim totalDebit = lines.Sum(Function(x) x.DebitAmount)
        Dim totalCredit = lines.Sum(Function(x) x.CreditAmount)

        ' =========================================
        ' 🧾 Header
        ' =========================================
        Dim journalTypeID = _repo.GetJournalTypeByOperation(opTypeID, con, tran)

        Dim result = _repo.InsertJournalHeader(
            transactionID,
            journalTypeID,
            userID,
            totalDebit,
            totalCredit,
            opTypeID,
            documentID,
            con,
            tran
        )
        Dim journalID = Result.JournalID
        Dim refNo = Result.RefNo
        Dim periodID = _repo.GetPeriodID_ByJournal(journalID, con, tran)
        Dim fiscalYearID = _repo.GetFiscalYearID(periodID, con, tran)
        ' =========================================
        ' 🧾 Details
        ' =========================================
        Dim lineNo As Integer = 1

        For Each l In lines
            If Math.Round(l.DebitAmount, 5) = 0D AndAlso Math.Round(l.CreditAmount, 5) = 0D Then
                Continue For
            End If
            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("خطأ: لا يمكن أن يكون السطر مدين ودائن معاً")
            End If
            If l.DebitAmount <= 0 AndAlso l.CreditAmount <= 0 Then
                Throw New Exception("سطر صفر - AccountID: " & l.AccountID)
            End If

            If l.DebitAmount > 0 AndAlso l.CreditAmount > 0 Then
                Throw New Exception("سطر مزدوج - AccountID: " & l.AccountID)
            End If
            Dim detailID = _repo.InsertJournalDetail(
                journalID,
                l.AccountID,
                l.DebitAmount,
                l.CreditAmount,
                lineNo,
                l.PartnerID,
                Nothing,
                Nothing,
                Nothing,
                Nothing,
                userID,
                con,
                tran
            )

            lineNo += 1

            _repo.InsertAccountLedger(
                journalID,
                detailID,
                l.AccountID,
                transactionID,
                l.DebitAmount,
                l.CreditAmount,
                userID,
                con,
                tran
            )
            _repo.UpdateAccountBalance(
    l.AccountID,
    periodID,
    fiscalYearID,
    l.DebitAmount,
    l.CreditAmount,
    con,
    tran
)
        Next

        _repo.MarkJournalAsPosted(journalID, userID, con, tran)

    End Sub
    Private Function GetOperationTypeFromDocument(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM inv.DocumentHeader
WHERE DocumentID = @D
", con, tran)

            cmd.Parameters.AddWithValue("@D", documentID)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("❌ Document has no OperationTypeID")
            End If

            Return CInt(obj)

        End Using
    End Function
    Private Function ResolveAmountPerDocumentDetail(
    r As DataRow,
    sourceFieldID As Integer
) As Decimal

        Select Case sourceFieldID

            Case 1 ' Quantity
                Return If(IsDBNull(r("Quantity")), 0D, CDec(r("Quantity")))

            Case 2 ' Net
                Return If(IsDBNull(r("NetAmount")), 0D, CDec(r("NetAmount")))

            Case 3 ' Tax
                Return If(IsDBNull(r("TaxAmount")), 0D, CDec(r("TaxAmount")))

            Case 11, 14 ' Total / Customer
                Return If(IsDBNull(r("GrossAmount")), 0D, CDec(r("GrossAmount")))

            Case Else
                Return 0D

        End Select

    End Function
    Private Function GetBeneficiaryAccount(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Return _repo.GetBeneficiaryAccount_FromPayment(transactionID, con, tran)

    End Function






End Class