Imports System.Data.SqlClient

Public Class PurchaseApplicationService

    Private ReadOnly _connStr As String

    Public Sub New(connStr As String)
        _connStr = connStr
    End Sub

    Public Sub SendPurchase(
    documentID As Integer,
    transactionCode As String,
    userID As Integer
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    SendPurchase_Internal(
                    documentID,
                    transactionCode,
                    userID,
                    con,
                    tran
                )

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub
    Private Sub SendPurchase_Internal(
    documentID As Integer,
    transactionCode As String,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)


        Try
            Dim nowDate As DateTime = DateTime.Now
            Dim currentStatusID As Integer
            Dim documentDate As Date
            Dim periodID As Integer
            Dim transactionID As Integer
            Dim operationTypeID As Integer = 7 ' PUR
            Dim isTaxInclusive As Boolean

            '==================================================
            ' (1) قراءة حالة المستند + التاريخ + شامل ضريبة؟
            '==================================================
            Using cmd As New SqlCommand("
SELECT StatusID, DocumentDate, IsTaxInclusive
FROM inv.DocumentHeader
WHERE DocumentID = @DocumentID
", con, tran)

                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                Using rd = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        Throw New Exception("السند غير موجود")
                    End If

                    currentStatusID = CInt(rd("StatusID"))
                    documentDate = CDate(rd("DocumentDate"))
                    isTaxInclusive = CBool(rd("IsTaxInclusive"))
                End Using
            End Using

            If currentStatusID <> 2 Then
                Throw New Exception("السند غير قابل للإرسال (الحالة ليست NEW)")
            End If

            '==================================================
            ' (2) تحديد PeriodID
            '==================================================
            Using cmd As New SqlCommand("
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE @DocDate >= StartDate
AND @DocDate < DATEADD(DAY, 1, EndDate)
AND IsOpen = 1
ORDER BY StartDate DESC
", con, tran)

                cmd.Parameters.AddWithValue("@DocDate", documentDate)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then
                    Throw New Exception("لا يوجد فترة مالية مفتوحة لهذا التاريخ")
                End If

                periodID = CInt(result)

            End Using
            '==================================================
            ' (3) إنشاء Transaction Header
            '==================================================
            Using cmd As New SqlCommand("
INSERT INTO inv.TransactionHeader
(
TransactionDate,
SourceDocumentID,
OperationTypeID,
PeriodID,
StatusID,
IsFinancialPosted,
CreatedBy,
CreatedAt,
SentAt,
SentBy,
IsInventoryPosted
)
VALUES
(
@Now,
@DocumentID,
@OperationTypeID,
@PeriodID,
5,
0,
@UserID,
@Now,
@Now,
@UserID,
0
);

SELECT SCOPE_IDENTITY();
", con, tran)

                cmd.Parameters.AddWithValue("@Now", nowDate)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PeriodID", periodID)
                cmd.Parameters.AddWithValue("@UserID", userID)

                transactionID = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            If transactionID <= 0 Then
                Throw New Exception("فشل إنشاء TransactionHeader")
            End If

            '==================================================
            ' (4) إنشاء Transaction Details
            '     ✔ UnitCost دائماً بدون ضريبة
            '     ✔ CostAmount = NetAmount
            '==================================================
            Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(
TransactionID,
ProductID,
Quantity,
UnitID,
UnitCost,
CostAmount,
SourceStoreID,
TargetStoreID,
SourceDocumentDetailID,
ReferenceDetailID,
CreatedAt,
CreatedBy
)
SELECT
@TransactionID,
d.ProductID,
d.Quantity,
d.UnitID,

-- UnitCost بدون ضريبة دائماً
CASE 
    WHEN @IsTaxInclusive = 1 AND d.Quantity <> 0
        THEN (d.NetAmount / d.Quantity)
    WHEN d.Quantity <> 0
        THEN (d.NetAmount / d.Quantity)
    ELSE 0
END,

-- CostAmount = NetAmount
d.NetAmount,

d.SourceStoreID,
d.TargetStoreID,
d.DetailID,
NULL,
@Now,
@UserID
FROM inv.DocumentDetails d
WHERE d.DocumentID = @DocumentID
", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@Now", nowDate)
                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)
                cmd.Parameters.AddWithValue("@IsTaxInclusive", isTaxInclusive)

                cmd.ExecuteNonQuery()
            End Using

            '==================================================
            ' (5) تحديث حالة المستند فقط
            '     ✔ لا نغير IsInventoryPosted
            '==================================================
            Using cmd As New SqlCommand("
UPDATE inv.DocumentHeader
SET StatusID = 5,
    SentAt = @Now,
    SentBy = @UserID
WHERE DocumentID = @DocumentID
", con, tran)

                cmd.Parameters.AddWithValue("@Now", nowDate)
                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                cmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            Throw
        End Try


    End Sub


    Public Sub UpdatePurchaseWithTransactionSync(
        documentID As Integer,
        documentDate As Date,
        partnerID As Integer,
        taxTypeID As Integer,
        paymentMethodID As Integer,
        paymentTermID As Integer,
        notes As String,
        isTaxInclusive As Boolean,
        details As DataTable
    )





        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    '========================
                    ' 1) تحديث الهيدر
                    '========================
                    Using cmd As New SqlCommand("
UPDATE inv.DocumentHeader SET
DocumentDate=@Date,
PartnerID=@Partner,
PaymentMethodID=@PayMethod,
PaymentTermID=@PayTerm,
Notes=@Notes,
TaxTypeID=@TaxType,
IsTaxInclusive=@Inclusive
WHERE DocumentID=@ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.Parameters.AddWithValue("@Date", documentDate)
                        cmd.Parameters.AddWithValue("@Partner", partnerID)
                        cmd.Parameters.AddWithValue("@PayMethod", paymentMethodID)
                        cmd.Parameters.AddWithValue("@PayTerm", paymentTermID)
                        cmd.Parameters.AddWithValue("@Notes", notes)
                        cmd.Parameters.AddWithValue("@TaxType", taxTypeID)
                        cmd.Parameters.AddWithValue("@Inclusive", isTaxInclusive)

                        cmd.ExecuteNonQuery()
                    End Using

                    '========================
                    ' 2) تحميل DB
                    '========================
                    Dim dtDB As New DataTable()

                    Using da As New SqlDataAdapter("
SELECT * FROM inv.DocumentDetails
WHERE DocumentID = @ID
", con)
                        da.SelectCommand.Parameters.AddWithValue("@ID", documentID)
                        da.SelectCommand.Transaction = tran
                        da.Fill(dtDB)
                    End Using

                    Dim dictDB As New Dictionary(Of Integer, DataRow)

                    For Each r As DataRow In dtDB.Rows
                        dictDB(CInt(r("DetailID"))) = r
                    Next

                    '========================
                    ' 3) تحميل TRN
                    '========================
                    Dim transactionID As Integer

                    Using cmd As New SqlCommand("
SELECT TransactionID
FROM inv.TransactionHeader
WHERE SourceDocumentID = @Doc
", con, tran)

                        cmd.Parameters.AddWithValue("@Doc", documentID)
                        transactionID = CInt(cmd.ExecuteScalar())
                    End Using

                    Dim dtTRN As New DataTable()

                    Using da As New SqlDataAdapter("
SELECT * FROM inv.TransactionDetails
WHERE TransactionID = @TID
", con)
                        da.SelectCommand.Parameters.AddWithValue("@TID", transactionID)
                        da.SelectCommand.Transaction = tran
                        da.Fill(dtTRN)
                    End Using

                    Dim dictTRN As New Dictionary(Of Integer, DataRow)

                    For Each r As DataRow In dtTRN.Rows
                        If Not IsDBNull(r("SourceDocumentDetailID")) Then
                            dictTRN(CInt(r("SourceDocumentDetailID"))) = r
                        End If
                    Next

                    '========================
                    ' 4) LOOP على التفاصيل
                    '========================
                    For Each row As DataRow In details.Rows

                        If row.RowState = DataRowState.Deleted Then Continue For

                        Dim hasID As Boolean = details.Columns.Contains("DetailID") _
                                              AndAlso Not IsDBNull(row("DetailID"))

                        Dim detailID As Integer = If(hasID, CInt(row("DetailID")), 0)

                        If hasID AndAlso dictDB.ContainsKey(detailID) Then

                            '========================
                            ' UPDATE Document
                            '========================
                            Using cmd As New SqlCommand("
UPDATE inv.DocumentDetails SET
ProductID=@Prod,
UnitID=@Unit,
Quantity=@Qty,
UnitPrice=@Price,
GrossAmount=@Gross,
DiscountRate=@Rate,
DiscountAmount=@Disc,
NetAmount=@Net,
TaxRate=@TaxRate,
TaxAmount=@Tax,
LineTotal=@Line,
SourceStoreID=@Source,
TargetStoreID=@Target,
TaxTypeID=@TaxType,
TaxableAmount=@Taxable
WHERE DetailID=@ID
", con, tran)

                                cmd.Parameters.AddWithValue("@ID", detailID)
                                cmd.Parameters.AddWithValue("@Prod", row("ProductID"))
                                cmd.Parameters.AddWithValue("@Unit", row("UnitID"))
                                cmd.Parameters.AddWithValue("@Qty", row("Quantity"))
                                cmd.Parameters.AddWithValue("@Price", row("UnitPrice"))
                                cmd.Parameters.AddWithValue("@Gross", row("GrossAmount"))
                                cmd.Parameters.AddWithValue("@Rate", row("DiscountRate"))
                                cmd.Parameters.AddWithValue("@Disc", row("DiscountAmount"))
                                cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                                cmd.Parameters.AddWithValue("@TaxRate", row("TaxRate"))
                                cmd.Parameters.AddWithValue("@Tax", row("TaxAmount"))
                                cmd.Parameters.AddWithValue("@Line", row("LineTotal"))
                                cmd.Parameters.AddWithValue("@Source",
                                    If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))
                                cmd.Parameters.AddWithValue("@Target", row("TargetStoreID"))
                                cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                                cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))

                                cmd.ExecuteNonQuery()
                            End Using

                            '========================
                            ' UPDATE TRN
                            '========================
                            If dictTRN.ContainsKey(detailID) Then

                                Using cmd As New SqlCommand("
UPDATE inv.TransactionDetails
SET Quantity=@Qty,
    UnitCost = CASE WHEN @Qty<>0 THEN @Net/@Qty ELSE 0 END,
    CostAmount=@Net,
    ProductID=@Prod,
    UnitID=@Unit,
    SourceStoreID=@Source,
    TargetStoreID=@Target
WHERE DetailID=@DetailID
", con, tran)

                                    cmd.Parameters.AddWithValue("@Qty", row("Quantity"))
                                    cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                                    cmd.Parameters.AddWithValue("@Prod", row("ProductID"))
                                    cmd.Parameters.AddWithValue("@Unit", row("UnitID"))
                                    cmd.Parameters.AddWithValue("@Source",
                                        If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))
                                    cmd.Parameters.AddWithValue("@Target", row("TargetStoreID"))
                                    cmd.Parameters.AddWithValue("@DetailID",
                                        dictTRN(detailID)("DetailID"))

                                    cmd.ExecuteNonQuery()
                                End Using

                            End If

                            dictDB.Remove(detailID)

                        Else

                            '========================
                            ' INSERT Document
                            '========================
                            Dim newDetailID As Integer

                            Using cmd As New SqlCommand("
INSERT INTO inv.DocumentDetails
(DocumentID, ProductID, UnitID, Quantity,
 UnitPrice, GrossAmount, DiscountRate,
 DiscountAmount, NetAmount,
 TaxRate, TaxAmount, LineTotal,
 SourceStoreID, TargetStoreID,
 TaxTypeID, TaxableAmount)
VALUES
(@Doc, @Prod, @Unit, @Qty,
 @Price, @Gross, @Rate,
 @Disc, @Net,
 @TaxRate, @Tax, @Line,
 @Source, @Target,
 @TaxType, @Taxable);
SELECT SCOPE_IDENTITY();
", con, tran)

                                cmd.Parameters.AddWithValue("@Doc", documentID)
                                cmd.Parameters.AddWithValue("@Prod", row("ProductID"))
                                cmd.Parameters.AddWithValue("@Unit", row("UnitID"))
                                cmd.Parameters.AddWithValue("@Qty", row("Quantity"))
                                cmd.Parameters.AddWithValue("@Price", row("UnitPrice"))
                                cmd.Parameters.AddWithValue("@Gross", row("GrossAmount"))
                                cmd.Parameters.AddWithValue("@Rate", row("DiscountRate"))
                                cmd.Parameters.AddWithValue("@Disc", row("DiscountAmount"))
                                cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                                cmd.Parameters.AddWithValue("@TaxRate", row("TaxRate"))
                                cmd.Parameters.AddWithValue("@Tax", row("TaxAmount"))
                                cmd.Parameters.AddWithValue("@Line", row("LineTotal"))
                                cmd.Parameters.AddWithValue("@Source",
                                    If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))
                                cmd.Parameters.AddWithValue("@Target", row("TargetStoreID"))
                                cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                                cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))

                                newDetailID = CInt(cmd.ExecuteScalar())
                            End Using

                            '========================
                            ' INSERT TRN
                            '========================
                            Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(TransactionID, ProductID, Quantity, UnitID,
 UnitCost, CostAmount,
 SourceStoreID, TargetStoreID,
 SourceDocumentDetailID,
 CreatedAt, CreatedBy)
VALUES
(@TID, @Prod, @Qty, @Unit,
 CASE WHEN @Qty<>0 THEN @Net/@Qty ELSE 0 END,
 @Net,
 @Source, @Target,
 @DocDetailID,
 GETDATE(), 1)
", con, tran)

                                cmd.Parameters.AddWithValue("@TID", transactionID)
                                cmd.Parameters.AddWithValue("@Prod", row("ProductID"))
                                cmd.Parameters.AddWithValue("@Qty", row("Quantity"))
                                cmd.Parameters.AddWithValue("@Unit", row("UnitID"))
                                cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                                cmd.Parameters.AddWithValue("@Source",
                                    If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))
                                cmd.Parameters.AddWithValue("@Target", row("TargetStoreID"))
                                cmd.Parameters.AddWithValue("@DocDetailID", newDetailID)

                                cmd.ExecuteNonQuery()
                            End Using

                        End If

                    Next


                    'تحديث المجاميع في الهدر
                    Using cmd As New SqlCommand("
UPDATE inv.DocumentHeader
SET 
    TotalAmount = ISNULL(S.Gross,0),
    TotalDiscount = ISNULL(S.Discount,0),
    TotalNetAmount = ISNULL(S.Net,0),
    TotalTax = ISNULL(S.Tax,0),
    TotalTaxableAmount = ISNULL(S.Taxable,0),
    GrandTotal = ISNULL(S.Total,0)
FROM inv.DocumentHeader H
OUTER APPLY (
    SELECT 
        SUM(GrossAmount) AS Gross,
        SUM(DiscountAmount) AS Discount,
        SUM(NetAmount) AS Net,
        SUM(TaxAmount) AS Tax,
        SUM(TaxableAmount) AS Taxable,
        SUM(LineTotal) AS Total
    FROM inv.DocumentDetails
    WHERE DocumentID = @ID
) S
WHERE H.DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.ExecuteNonQuery()

                    End Using
                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub
    Public Sub CancelPurchase(documentID As Integer, userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim statusID As Integer

                    '========================
                    ' 1) قراءة الحالة
                    '========================
                    Using cmd As New SqlCommand("
SELECT StatusID
FROM inv.DocumentHeader
WHERE DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", documentID)

                        Dim result = cmd.ExecuteScalar()

                        If result Is Nothing Then
                            Throw New Exception("السند غير موجود")
                        End If

                        statusID = CInt(result)
                    End Using

                    '========================
                    ' 2) تحديد نوع الإلغاء
                    '========================
                    Dim isNotDelete As Boolean = (statusID = 6)
                    If isNotDelete Then
                        Throw New Exception("لا يمكن الالغاء بعد الاستلام")
                    End If
                    Dim isDelete As Boolean = (statusID = 1 OrElse statusID = 2)

                    '========================
                    ' 3) DELETE (مسودة / جديد)
                    '========================
                    If isDelete Then

                        ' حذف التفاصيل
                        Using cmd As New SqlCommand("
DELETE FROM inv.DocumentDetails
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using

                        ' حذف الهيدر
                        Using cmd As New SqlCommand("
DELETE FROM inv.DocumentHeader
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using

                    Else



                        ' تحديث الحالة → ملغي (10)
                        Using cmd As New SqlCommand("
UPDATE inv.DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using



                        Using cmd As New SqlCommand("
                    UPDATE inv.TransactionHeader
                    SET StatusID = 10
                    WHERE SourceDocumentID = @DocID
                    ", con, tran)

                            cmd.Parameters.AddWithValue("@DocID", documentID)
                            cmd.ExecuteNonQuery()

                        End Using
                    End If

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub


    Public Function SaveDraftDirect(
    documentID As Integer,
    documentNo As String,
    documentDate As Date,
    partnerID As Integer,
    taxTypeID As Integer,
    paymentMethodID As Integer,
    paymentTermID As Integer,
    notes As String,
    isTaxInclusive As Boolean,
    details As DataTable
) As Integer

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    Dim result = SaveDraftDirect_Internal(
                    documentID,
                    documentNo,
                    documentDate,
                    partnerID,
                    taxTypeID,
                    paymentMethodID,
                    paymentTermID,
                    notes,
                    isTaxInclusive,
                    details,
                    con,
                    tran
                )

                    tran.Commit()
                    Return result

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Function

    Public Function SavePostedDocumentWithQueue(
    documentID As Integer,
    documentNo As String,
    documentDate As Date,
    partnerID As Integer,
    taxTypeID As Integer,
    paymentMethodID As Integer,
    paymentTermID As Integer,
    notes As String,
    isTaxInclusive As Boolean,
    details As DataTable,
    originalDetails As DataTable,
    scopeCode As String
) As Integer
        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    If IsDocumentInCorrectionQueue(documentID, con, tran) Then
                        Throw New Exception("لا يمكن تعديل السند لأنه قيد التصحيح")
                    End If

                    ' 1) حفظ الدكمنت
                    Dim newID = SaveDraftDirect_Internal(
                        documentID,
                        documentNo,
                        documentDate,
                        partnerID,
                        taxTypeID,
                        paymentMethodID,
                        paymentTermID,
                        notes,
                        isTaxInclusive,
                        details,
                        con,
                        tran
                    )

                    ' 2) بناء Queue
                    BuildQueueFromTables(
                        originalDetails,
                        details,
                        scopeCode,
                        con,
                        tran
                            )

                    tran.Commit()
                    Return newID
                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Private Function SaveDraftDirect_Internal(
    documentID As Integer,
    documentNo As String,
    documentDate As Date,
    partnerID As Integer,
    taxTypeID As Integer,
    paymentMethodID As Integer,
    paymentTermID As Integer,
    notes As String,
    isTaxInclusive As Boolean,
    details As DataTable,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Try

            '======================================================
            ' 0) تأكيد وجود التفاصيل
            '======================================================
            If details Is Nothing Then
                Throw New Exception("details = Nothing")
            End If

            '======================================================
            ' 1) تصحيح/توحيد القيم المحسوبة في details قبل الحفظ
            '    لمنع أي عدم اتساق (TaxableAmount/NetAmount/TaxAmount/LineTotal...)
            '======================================================
            Dim totalAmount As Decimal = 0D
            Dim totalDiscount As Decimal = 0D
            Dim totalTax As Decimal = 0D
            Dim totalTaxable As Decimal = 0D
            Dim totalNet As Decimal = 0D

            For Each row As DataRow In details.Select("", "", DataViewRowState.CurrentRows)
                If row.RowState = DataRowState.Deleted Then Continue For

                Dim qty As Decimal = 0D
                Dim unitPrice As Decimal = 0D
                Dim discRate As Decimal = 0D
                Dim taxRatePct As Decimal = 0D

                If Not row.IsNull("Quantity") Then qty = Convert.ToDecimal(row("Quantity"))
                If Not row.IsNull("UnitPrice") Then unitPrice = Convert.ToDecimal(row("UnitPrice"))
                If Not row.IsNull("DiscountRate") Then discRate = Convert.ToDecimal(row("DiscountRate"))
                If Not row.IsNull("TaxRate") Then taxRatePct = Convert.ToDecimal(row("TaxRate"))

                Dim rate As Decimal = taxRatePct / 100D

                ' 1) GrossAmount = Qty * UnitPrice
                Dim gross As Decimal = qty * unitPrice

                ' 2) DiscountAmount = Gross * (DiscountRate/100)
                Dim discAmt As Decimal = gross * (discRate / 100D)

                ' 3) Base = Gross - Discount
                Dim baseAfterDisc As Decimal = gross - discAmt

                Dim taxable As Decimal
                Dim taxAmt As Decimal
                Dim netAmt As Decimal
                Dim lineTotal As Decimal

                If isTaxInclusive = False Then
                    ' غير شامل:
                    taxable = baseAfterDisc
                    taxAmt = taxable * rate
                    netAmt = taxable
                    lineTotal = taxable + taxAmt
                Else
                    ' شامل:
                    lineTotal = baseAfterDisc

                    If rate > 0D Then
                        taxable = lineTotal / (1D + rate)
                    Else
                        taxable = lineTotal
                    End If

                    taxAmt = lineTotal - taxable
                    netAmt = taxable
                End If

                ' تقريب 6 منازل (مثل الحسابات عندك)
                gross = Math.Round(gross, 6)
                discAmt = Math.Round(discAmt, 6)
                taxable = Math.Round(taxable, 6)
                taxAmt = Math.Round(taxAmt, 6)
                netAmt = Math.Round(netAmt, 6)
                lineTotal = Math.Round(lineTotal, 6)

                ' تحديث القيم داخل DataTable (مهم حتى تكون المجاميع صحيحة)
                If details.Columns.Contains("GrossAmount") Then row("GrossAmount") = gross
                If details.Columns.Contains("DiscountAmount") Then row("DiscountAmount") = discAmt
                If details.Columns.Contains("TaxableAmount") Then row("TaxableAmount") = taxable
                If details.Columns.Contains("TaxAmount") Then row("TaxAmount") = taxAmt
                If details.Columns.Contains("NetAmount") Then row("NetAmount") = netAmt
                If details.Columns.Contains("LineTotal") Then row("LineTotal") = lineTotal

                ' تجميع الهيدر
                totalAmount += gross
                totalDiscount += discAmt
                totalTax += taxAmt
                totalTaxable += taxable
                totalNet += netAmt

            Next

            totalAmount = Math.Round(totalAmount, 6)
            totalDiscount = Math.Round(totalDiscount, 6)
            totalTax = Math.Round(totalTax, 6)
            totalTaxable = Math.Round(totalTaxable, 6)
            totalNet = Math.Round(totalNet, 6)

            Dim grandTotal As Decimal
            If isTaxInclusive Then
                ' شامل: مجموع LineTotal = مجموع (Gross - Discount)
                ' grandTotal = totalTaxable + totalTax = totalLineTotal
                grandTotal = Math.Round(totalTaxable + totalTax, 6)
            Else
                ' غير شامل: grandTotal = net + tax
                grandTotal = Math.Round(totalNet + totalTax, 6)
            End If

            Dim remainingAmount As Decimal = grandTotal

            Dim newID As Integer = documentID

            '======================================================
            ' 2) Insert/Update Header
            '======================================================
            If documentID = 0 Then

                Using cmd As New SqlCommand("
INSERT INTO inv.DocumentHeader
(DocumentType, DocumentNo, DocumentDate,
 PartnerID, CurrencyID, ExchangeRate,
 TotalAmount, TotalDiscount, TotalTax,
 TotalTaxableAmount, PaidAmount, RemainingAmount,
 PaymentMethodID, PaymentTermID, Notes,
 TaxTypeID, StatusID, CreatedAt,
 IsTaxInclusive, IsInventoryPosted,
 GrandTotal, TotalNetAmount)
OUTPUT INSERTED.DocumentID
VALUES
('PUR', @No, @Date,
 @Partner, 1, 1,
 @Total, @Disc, @Tax,
 @Taxable, 0, @Remain,
 @PayMethod, @PayTerm, @Notes,
 @TaxType, 2, GETDATE(),
 @Inclusive, 0,
 @Grand, @Net)
", con, tran)

                    cmd.Parameters.AddWithValue("@No", documentNo)
                    cmd.Parameters.AddWithValue("@Date", documentDate)
                    cmd.Parameters.AddWithValue("@Partner", partnerID)
                    cmd.Parameters.AddWithValue("@Total", totalAmount)
                    cmd.Parameters.AddWithValue("@Disc", totalDiscount)
                    cmd.Parameters.AddWithValue("@Tax", totalTax)
                    cmd.Parameters.AddWithValue("@Taxable", totalTaxable)
                    cmd.Parameters.AddWithValue("@Remain", remainingAmount)
                    cmd.Parameters.AddWithValue("@PayMethod", paymentMethodID)
                    cmd.Parameters.AddWithValue("@PayTerm", paymentTermID)
                    cmd.Parameters.AddWithValue("@Notes", notes)
                    cmd.Parameters.AddWithValue("@TaxType", taxTypeID)
                    cmd.Parameters.AddWithValue("@Inclusive", isTaxInclusive)
                    cmd.Parameters.AddWithValue("@Grand", grandTotal)
                    cmd.Parameters.AddWithValue("@Net", totalNet)

                    newID = CInt(cmd.ExecuteScalar())
                End Using

            Else

                Using cmd As New SqlCommand("
UPDATE inv.DocumentHeader SET
DocumentDate=@Date,
PartnerID=@Partner,
TotalAmount=@Total,
TotalDiscount=@Disc,
TotalTax=@Tax,
TotalTaxableAmount=@Taxable,
RemainingAmount=@Remain,
PaymentMethodID=@PayMethod,
PaymentTermID=@PayTerm,
Notes=@Notes,
TaxTypeID=@TaxType,
IsTaxInclusive=@Inclusive,
GrandTotal=@Grand,
TotalNetAmount=@Net
WHERE DocumentID=@ID
", con, tran)

                    cmd.Parameters.AddWithValue("@ID", documentID)
                    cmd.Parameters.AddWithValue("@Date", documentDate)
                    cmd.Parameters.AddWithValue("@Partner", partnerID)
                    cmd.Parameters.AddWithValue("@Total", totalAmount)
                    cmd.Parameters.AddWithValue("@Disc", totalDiscount)
                    cmd.Parameters.AddWithValue("@Tax", totalTax)
                    cmd.Parameters.AddWithValue("@Taxable", totalTaxable)
                    cmd.Parameters.AddWithValue("@Remain", remainingAmount)
                    cmd.Parameters.AddWithValue("@PayMethod", paymentMethodID)
                    cmd.Parameters.AddWithValue("@PayTerm", paymentTermID)
                    cmd.Parameters.AddWithValue("@Notes", notes)
                    cmd.Parameters.AddWithValue("@TaxType", taxTypeID)
                    cmd.Parameters.AddWithValue("@Inclusive", isTaxInclusive)
                    cmd.Parameters.AddWithValue("@Grand", grandTotal)
                    cmd.Parameters.AddWithValue("@Net", totalNet)

                    cmd.ExecuteNonQuery()
                End Using


            End If

            '======================================================
            ' 3) Insert Details (بعد تصحيح القيم)
            '======================================================
            For Each row As DataRow In details.Select("", "", DataViewRowState.CurrentRows)

                If row.RowState = DataRowState.Deleted Then Continue For

                If documentID = 0 Then
                    ' =========================
                    ' 🟢 إنشاء جديد → INSERT
                    ' =========================
                    Using cmd As New SqlCommand("
INSERT INTO inv.DocumentDetails
(DocumentID, ProductID, UnitID, Quantity,
 UnitPrice, GrossAmount, DiscountRate,
 DiscountAmount, NetAmount,
 TaxRate, TaxAmount, LineTotal,
 SourceStoreID, TargetStoreID,
 TaxTypeID, TaxableAmount)
VALUES
(@Doc, @Prod, @Unit, @Qty,
 @Price, @Gross, @Rate,
 @Disc, @Net,
 @TaxRate, @Tax, @Line,
 @Source, @Target,
 @TaxType, @Taxable)
", con, tran)

                        cmd.Parameters.AddWithValue("@Doc", newID)
                        cmd.Parameters.AddWithValue("@Prod", row("ProductID"))
                        cmd.Parameters.AddWithValue("@Unit", row("UnitID"))
                        cmd.Parameters.AddWithValue("@Qty", row("Quantity"))

                        cmd.Parameters.AddWithValue("@Price", row("UnitPrice"))
                        cmd.Parameters.AddWithValue("@Gross", row("GrossAmount"))
                        cmd.Parameters.AddWithValue("@Rate", row("DiscountRate"))
                        cmd.Parameters.AddWithValue("@Disc", row("DiscountAmount"))
                        cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                        cmd.Parameters.AddWithValue("@TaxRate", row("TaxRate"))
                        cmd.Parameters.AddWithValue("@Tax", row("TaxAmount"))
                        cmd.Parameters.AddWithValue("@Line", row("LineTotal"))

                        cmd.Parameters.AddWithValue("@Source",
                If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))

                        cmd.Parameters.AddWithValue("@Target",
                If(row.IsNull("TargetStoreID"), DBNull.Value, row("TargetStoreID")))

                        cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                        cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))

                        cmd.ExecuteNonQuery()
                    End Using

                Else
                    ' =========================
                    ' 🔵 تعديل → UPDATE
                    ' =========================
                    ' ✔ فقط في حالة التعديل على سند مرحل
                    If details.Columns.Contains("OriginalDetailID") Then

                        ' نحدد هل نحن في وضع تعديل مرحل
                        Dim isPostedEdit As Boolean = details.AsEnumerable().
        Any(Function(r) Not IsDBNull(r("OriginalDetailID")))

                        If isPostedEdit Then
                            If row.IsNull("OriginalDetailID") Then
                                Throw New Exception("OriginalDetailID مفقود في وضع التعديل")
                            End If
                        End If

                    End If
                    Using cmd As New SqlCommand("
UPDATE inv.DocumentDetails SET
    Quantity = @Qty,
    UnitPrice = @Price,
    GrossAmount = @Gross,
    DiscountRate = @Rate,
    DiscountAmount = @Disc,
    NetAmount = @Net,
    TaxRate = @TaxRate,
    TaxAmount = @Tax,
    LineTotal = @Line,
    SourceStoreID = @Source,
    TargetStoreID = @Target,
    TaxTypeID = @TaxType,
    TaxableAmount = @Taxable
WHERE DetailID = @DetailID
", con, tran)

                        Dim detailID As Object

                        If Not row.IsNull("OriginalDetailID") Then
                            detailID = row("OriginalDetailID")   ' حالة 6
                        Else
                            detailID = row("DetailID")           ' حالة 5
                        End If

                        cmd.Parameters.AddWithValue("@DetailID", detailID)
                        cmd.Parameters.AddWithValue("@Qty", row("Quantity"))
                        cmd.Parameters.AddWithValue("@Price", row("UnitPrice"))
                        cmd.Parameters.AddWithValue("@Gross", row("GrossAmount"))
                        cmd.Parameters.AddWithValue("@Rate", row("DiscountRate"))
                        cmd.Parameters.AddWithValue("@Disc", row("DiscountAmount"))
                        cmd.Parameters.AddWithValue("@Net", row("NetAmount"))
                        cmd.Parameters.AddWithValue("@TaxRate", row("TaxRate"))
                        cmd.Parameters.AddWithValue("@Tax", row("TaxAmount"))
                        cmd.Parameters.AddWithValue("@Line", row("LineTotal"))

                        cmd.Parameters.AddWithValue("@Source",
                If(row.IsNull("SourceStoreID"), DBNull.Value, row("SourceStoreID")))

                        cmd.Parameters.AddWithValue("@Target",
                If(row.IsNull("TargetStoreID"), DBNull.Value, row("TargetStoreID")))

                        cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                        cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))

                        cmd.ExecuteNonQuery()
                    End Using

                End If

            Next

            Return newID

        Catch ex As Exception
            Throw
        End Try

    End Function



    Public Sub BuildQueueFromTables(
    oldTable As DataTable,
    newTable As DataTable,
    scopeCode As String,
    con As SqlConnection,
    tran As SqlTransaction
)
        Dim dictOld As New Dictionary(Of Integer, DataRow)

        '========================
        ' 1) بناء القديم
        '========================
        For Each r As DataRow In oldTable.Rows

            If IsDBNull(r("DetailID")) Then Continue For

            dictOld(CInt(r("DetailID"))) = r

        Next

        Dim usedOld As New HashSet(Of Integer)
        Dim map As New Dictionary(Of Integer, Tuple(Of Integer, Integer))

        Using cmd As New SqlCommand("
SELECT 
    D.DetailID,
    TD.DetailID AS TransactionDetailID,
    CL.LedgerID
FROM inv.DocumentDetails D
LEFT JOIN inv.TransactionDetails TD
    ON TD.SourceDocumentDetailID = D.DetailID
LEFT JOIN inv.CostLedger CL
    ON CL.SourceDetailID = TD.DetailID
WHERE D.DetailID IN (" & String.Join(",", dictOld.Keys) & ")
", con, tran)

            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim docID = CInt(rd("DetailID"))
                    Dim td = If(IsDBNull(rd("TransactionDetailID")), 0, CInt(rd("TransactionDetailID")))
                    Dim led = If(IsDBNull(rd("LedgerID")), 0, CInt(rd("LedgerID")))

                    map(docID) = Tuple.Create(td, led)
                End While
            End Using

        End Using
        '========================
        ' 2) EDIT
        '========================

        For Each r As DataRow In newTable.Rows

            If IsDBNull(r("OriginalDetailID")) Then Continue For

            Dim oldID As Integer = CInt(r("OriginalDetailID"))

            If Not dictOld.ContainsKey(oldID) Then Continue For

            usedOld.Add(oldID)

            Dim oldRow = dictOld(oldID)

            Dim oldQty As Decimal = CDec(oldRow("Quantity"))
            Dim newQty As Decimal = CDec(r("Quantity"))

            Dim oldNet As Decimal = CDec(oldRow("NetAmount"))
            Dim newNet As Decimal = CDec(r("NetAmount"))

            Dim oldCost As Decimal = 0D
            Dim newCost As Decimal = 0D

            If oldQty <> 0D Then oldCost = oldNet / oldQty
            If newQty <> 0D Then newCost = newNet / newQty

            Dim productID As Integer = CInt(r("ProductID"))

            ' 🔥 سماحية للأرقام العشرية
            Dim eps As Decimal = 0.000001D

            Dim qtyChanged As Boolean = Math.Abs(oldQty - newQty) > eps
            Dim costChanged As Boolean = Math.Abs(oldCost - newCost) > eps

            If qtyChanged OrElse costChanged Then
                Dim transactionDetailID As Integer = 0
                Dim startLedgerID As Integer = 0

                If map.ContainsKey(oldID) Then
                    transactionDetailID = map(oldID).Item1
                    startLedgerID = map(oldID).Item2
                End If
                Using cmd As New SqlCommand("
INSERT INTO inv.CorrectionQueue
(DocumentDetailID, TransactionDetailID, StartLedgerID, ProductID, ChangeType, StatusID, NewQuantity, NewUnitCost, ScopeCode, CreatedAt)
VALUES
(@ID, @TDID, @LedgerID, @ProductID, 'EDIT', 22, @Qty, @Cost, @Scope, GETDATE())
", con, tran)

                    cmd.Parameters.AddWithValue("@ID", oldID)
                    cmd.Parameters.AddWithValue("@Qty", newQty)
                    cmd.Parameters.AddWithValue("@Cost", newCost) ' 👈 صح
                    cmd.Parameters.AddWithValue("@Scope", scopeCode)
                    cmd.Parameters.AddWithValue("@ProductID", productID)
                    cmd.Parameters.AddWithValue("@TDID", If(transactionDetailID = 0, DBNull.Value, transactionDetailID))
                    cmd.Parameters.AddWithValue("@LedgerID", If(startLedgerID = 0, DBNull.Value, startLedgerID))
                    cmd.ExecuteNonQuery()
                End Using

            End If

        Next

    End Sub
    Private Function IsDocumentInCorrectionQueue(
    documentID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT COUNT(1)
FROM inv.CorrectionQueue Q
INNER JOIN inv.DocumentDetails D
    ON D.DetailID = Q.DocumentDetailID
WHERE D.DocumentID = @ID
  AND Q.StatusID = 22
", con, tran)

            cmd.Parameters.AddWithValue("@ID", documentID)
            Return CInt(cmd.ExecuteScalar()) > 0

        End Using

    End Function


    Public Function SaveSentDocument(
    documentID As Integer,
    documentNo As String,
    documentDate As Date,
    partnerID As Integer,
    taxTypeID As Integer,
    paymentMethodID As Integer,
    paymentTermID As Integer,
    notes As String,
    isTaxInclusive As Boolean,
    details As DataTable
) As Integer

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                ' =========================
                ' 0) تأكد من وجود ترانسكشن سابق
                ' =========================
                Dim transactionID As Object

                Using cmd As New SqlCommand("
SELECT TOP 1 TransactionID
FROM inv.TransactionHeader
WHERE SourceDocumentID = @DocID
", con, tran)

                    cmd.Parameters.AddWithValue("@DocID", documentID)
                    transactionID = cmd.ExecuteScalar()

                End Using
                Try
                    ' =========================
                    ' 1) تحديث الدوكمنت (نفس SaveDraft)
                    ' =========================
                    Dim newID = SaveDraftDirect_Internal(
                        documentID,
                        documentNo,
                        documentDate,
                        partnerID,
                        taxTypeID,
                        paymentMethodID,
                        paymentTermID,
                        notes,
                        isTaxInclusive,
                        details,
                        con,
                        tran
                    )
                    For Each r As DataRow In details.Rows
                        MessageBox.Show("Qty From UI: " & r("Quantity").ToString())
                    Next
                    Using cmd As New SqlCommand("
UPDATE TD
SET 
    TD.Quantity = D.Quantity,
    TD.UnitID = D.UnitID,
    TD.UnitCost = 
        CASE 
            WHEN D.Quantity <> 0 THEN (D.NetAmount / D.Quantity)
            ELSE 0
        END,
    TD.CostAmount = D.NetAmount,
    TD.SourceStoreID = D.SourceStoreID,
    TD.TargetStoreID = D.TargetStoreID
FROM inv.TransactionDetails TD
INNER JOIN inv.DocumentDetails D
    ON TD.SourceDocumentDetailID = D.DetailID
INNER JOIN inv.TransactionHeader TH
    ON TH.TransactionID = TD.TransactionID
WHERE TH.SourceDocumentID = @DocID
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", newID)
                        cmd.ExecuteNonQuery()
                        Dim affected = cmd.ExecuteNonQuery()
                        MessageBox.Show("Updated Rows: " & affected)
                    End Using

                    Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(
TransactionID,
ProductID,
Quantity,
UnitID,
UnitCost,
CostAmount,
SourceStoreID,
TargetStoreID,
SourceDocumentDetailID,
ReferenceDetailID,
CreatedAt,
CreatedBy
)
SELECT
TH.TransactionID,
D.ProductID,
D.Quantity,
D.UnitID,
CASE 
    WHEN D.Quantity <> 0 THEN (D.NetAmount / D.Quantity)
    ELSE 0
END,
D.NetAmount,
D.SourceStoreID,
D.TargetStoreID,
D.DetailID,
NULL,
GETDATE(),
1
FROM inv.DocumentDetails D
INNER JOIN inv.TransactionHeader TH
    ON TH.SourceDocumentID = D.DocumentID
WHERE D.DocumentID = @DocID
AND NOT EXISTS (
    SELECT 1 FROM inv.TransactionDetails TD
    WHERE TD.SourceDocumentDetailID = D.DetailID
)
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", newID)
                        cmd.ExecuteNonQuery()

                    End Using

                    tran.Commit()
                    Return newID

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function

    Private Function GetTransactionDetailID(
        documentDetailID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 DetailID
FROM inv.TransactionDetails
WHERE SourceDocumentDetailID = @DocDetailID
ORDER BY DetailID
", con, tran)

            cmd.Parameters.AddWithValue("@DocDetailID", documentDetailID)

            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then Return 0

            Return CInt(result)
        End Using

    End Function
    Private Function GetStartLedgerID(
    transactionDetailID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 LedgerID
FROM inv.CostLedger
WHERE SourceDetailID = @TDID
ORDER BY LedgerID
", con, tran)

            cmd.Parameters.AddWithValue("@TDID", transactionDetailID)

            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then Return 0

            Return CInt(result)
        End Using

    End Function


End Class