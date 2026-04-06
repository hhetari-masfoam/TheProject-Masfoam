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
FROM Inventory_DocumentHeader
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
WHERE @DocDate BETWEEN StartDate AND EndDate
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
INSERT INTO Inventory_TransactionHeader
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
INSERT INTO Inventory_TransactionDetails
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
FROM Inventory_DocumentDetails d
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
UPDATE Inventory_DocumentHeader
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
INSERT INTO Inventory_DocumentHeader
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
UPDATE Inventory_DocumentHeader SET
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

                Using cmd As New SqlCommand("
DELETE FROM Inventory_DocumentDetails
WHERE DocumentID=@ID
", con, tran)

                    cmd.Parameters.AddWithValue("@ID", documentID)
                    cmd.ExecuteNonQuery()
                End Using

            End If

            '======================================================
            ' 3) Insert Details (بعد تصحيح القيم)
            '======================================================
            For Each row As DataRow In details.Select("", "", DataViewRowState.CurrentRows)
                If row.RowState = DataRowState.Deleted Then Continue For

                Using cmd As New SqlCommand("
INSERT INTO Inventory_DocumentDetails
(DocumentID, ProductID, UnitID, Quantity,
 UnitPrice, GrossAmount, DiscountRate,
 DiscountAmount, NetAmount,
 TaxRate, TaxAmount, LineTotal,
 SourceStoreID, TargetStoreID,
 TaxTypeID, TaxableAmount,CorrectionReferenceDetailID)
VALUES
(@Doc, @Prod, @Unit, @Qty,
 @Price, @Gross, @Rate,
 @Disc, @Net,
 @TaxRate, @Tax, @Line,
 @Source, @Target,
 @TaxType, @Taxable,@OriginalDetailID)
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
                                If(row.IsNull("SourceStoreID"), CType(DBNull.Value, Object), row("SourceStoreID")))
                    cmd.Parameters.AddWithValue("@Target",
                                If(row.IsNull("TargetStoreID"), CType(DBNull.Value, Object), row("TargetStoreID")))

                    cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                    cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))
                    cmd.Parameters.AddWithValue("@OriginalDetailID",
    If(row.Table.Columns.Contains("OriginalDetailID") AndAlso
       Not IsDBNull(row("OriginalDetailID")),
       row("OriginalDetailID"),
       DBNull.Value))
                    cmd.ExecuteNonQuery()
                End Using

            Next

            Return newID

        Catch ex As Exception
            Throw
        End Try

    End Function

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
UPDATE Inventory_DocumentHeader SET
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
SELECT * FROM Inventory_DocumentDetails
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
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @Doc
", con, tran)

                        cmd.Parameters.AddWithValue("@Doc", documentID)
                        transactionID = CInt(cmd.ExecuteScalar())
                    End Using

                    Dim dtTRN As New DataTable()

                    Using da As New SqlDataAdapter("
SELECT * FROM Inventory_TransactionDetails
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
UPDATE Inventory_DocumentDetails SET
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
UPDATE Inventory_TransactionDetails
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
INSERT INTO Inventory_DocumentDetails
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
INSERT INTO Inventory_TransactionDetails
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
UPDATE Inventory_DocumentHeader
SET 
    TotalAmount = ISNULL(S.Gross,0),
    TotalDiscount = ISNULL(S.Discount,0),
    TotalNetAmount = ISNULL(S.Net,0),
    TotalTax = ISNULL(S.Tax,0),
    TotalTaxableAmount = ISNULL(S.Taxable,0),
    GrandTotal = ISNULL(S.Total,0)
FROM Inventory_DocumentHeader H
OUTER APPLY (
    SELECT 
        SUM(GrossAmount) AS Gross,
        SUM(DiscountAmount) AS Discount,
        SUM(NetAmount) AS Net,
        SUM(TaxAmount) AS Tax,
        SUM(TaxableAmount) AS Taxable,
        SUM(LineTotal) AS Total
    FROM Inventory_DocumentDetails
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
FROM Inventory_DocumentHeader
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
DELETE FROM Inventory_DocumentDetails
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using

                        ' حذف الهيدر
                        Using cmd As New SqlCommand("
DELETE FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using

                    Else



                        ' تحديث الحالة → ملغي (10)
                        Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.ExecuteNonQuery()
                        End Using



                        Using cmd As New SqlCommand("
                    UPDATE Inventory_TransactionHeader
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

    Public Function EditPostedPurchase(
    oldDocumentID As Integer,
    documentNo As String,
    documentDate As Date,
    partnerID As Integer,
    taxTypeID As Integer,
    paymentMethodID As Integer,
    paymentTermID As Integer,
    notes As String,
    isTaxInclusive As Boolean,
    details As DataTable,
    deletedOriginalDetailIDs As List(Of Integer),
    userID As Integer
) As Integer

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim oldTransactionID As Integer
                    Dim newDocumentID As Integer
                    Dim newTransactionID As Integer

                    '==================================================
                    ' 1) التحقق من أن السند مستلم
                    '==================================================
                    Using cmd As New SqlCommand("
SELECT TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @DocID
AND StatusID = 6
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", oldDocumentID)

                        Dim result = cmd.ExecuteScalar()

                        If result Is Nothing Then
                            Throw New Exception("لا يمكن تعديل السند (ليس مستلم)")
                        End If

                        oldTransactionID = CInt(result)
                    End Using

                    '==================================================
                    ' 2) تعليق السند القديم
                    '==================================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", oldDocumentID)
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New SqlCommand("
                    UPDATE Inventory_TransactionHeader
                    SET StatusID = 10
                    WHERE SourceDocumentID = @DocID
                    ", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", oldDocumentID)
                        cmd.ExecuteNonQuery()

                    End Using

                    '==================================================
                    ' 3) إنشاء مستند جديد
                    '==================================================
                    Dim changedOnly As DataTable = BuildChangedOnlyTable(details)

                    If changedOnly.Rows.Count = 0 AndAlso
   (deletedOriginalDetailIDs Is Nothing OrElse deletedOriginalDetailIDs.Count = 0) Then
                        Throw New Exception("لا يوجد أي تعديل للحفظ")
                    End If
                    newDocumentID = SaveDraftDirect_Internal(
    0,
                                    documentNo,
                                    documentDate,
                                    partnerID,
                                    taxTypeID,
                                    paymentMethodID,
                                    paymentTermID,
                                    notes,
                                    isTaxInclusive,
                                    changedOnly,
                                    con,
                                    tran
                                )

                    '==================================================
                    ' 4) ربط الجديد بالقديم (Header)
                    '==================================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET 
    IsCorrection = 1,
    CorrectionReferenceDocumentID = @OldID
WHERE DocumentID = @NewID
", con, tran)

                        cmd.Parameters.AddWithValue("@OldID", oldDocumentID)
                        cmd.Parameters.AddWithValue("@NewID", newDocumentID)
                        cmd.ExecuteNonQuery()
                    End Using



                    tran.Commit()

                    Return newDocumentID

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Public Sub CancelPostedPurchase(
    documentID As Integer,
    userID As Integer
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim transactionID As Integer

                    '==================================================
                    ' 1) التحقق أن السند مرحّل (مستلم)
                    '==================================================
                    Using cmd As New SqlCommand("
SELECT TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @DocID
AND StatusID = 6
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", documentID)

                        Dim result = cmd.ExecuteScalar()

                        If result Is Nothing Then
                            Throw New Exception("لا يمكن إلغاء السند (ليس مستلم)")
                        End If

                        transactionID = CInt(result)
                    End Using

                    '==================================================
                    ' 2) تغيير حالة المستند إلى ملغي
                    '==================================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    '==================================================
                    ' 3) تغيير حالة الترانسكشن
                    '==================================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_TransactionHeader
SET StatusID = 10
WHERE TransactionID = @TID
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", transactionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    '==================================================
                    ' 4) إدخال في Correction Queue (نقطة البداية)
                    '==================================================
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_CorrectionQueue
(
TransactionDetailID,
DocumentDetailID,
StartLedgerID,
StatusID,
CreatedAt,
ScopeCode
)
SELECT
t.DetailID,
d.DetailID,
NULL,
22,
GETDATE(),
'PUR'
FROM Inventory_TransactionDetails t
JOIN Inventory_DocumentDetails d
    ON d.DetailID = t.SourceDocumentDetailID

-- 👇 أهم جزء: نجيب الليدجر السابق

WHERE t.TransactionID = @TID
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", transactionID)
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
    Public Sub SendCorrectionPurchase(
    documentID As Integer,
    transactionCode As String,
    userID As Integer,
    deletedOriginalDetailIDs As List(Of Integer)
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    ' 1) إنشاء Transaction (نفس الدالة)
                    SendPurchase_Internal(
                        documentID,
                        transactionCode,
                        userID,
                        con,
                        tran
                    )

                    ' 2) تحويل مباشرة إلى مستلم
                    Using cmd As New SqlCommand("
UPDATE Inventory_TransactionHeader
SET StatusID = 6,
    IsInventoryPosted = 1
WHERE SourceDocumentID = @DocID
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 6,
    IsInventoryPosted = 1
WHERE DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' 3) جلب TransactionID
                    Dim newTransactionID As Integer

                    Using cmd As New SqlCommand("
SELECT TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @DocID
", con, tran)

                        cmd.Parameters.AddWithValue("@DocID", documentID)
                        newTransactionID = CInt(cmd.ExecuteScalar())
                    End Using

                    ' 4) ربط بالقديم
                    Using cmd As New SqlCommand("
UPDATE t
SET t.CorrectionReferenceTransactionID = oldT.TransactionID,
    t.IsCorrection = 1
FROM Inventory_TransactionHeader t
JOIN Inventory_DocumentHeader d
    ON d.DocumentID = t.SourceDocumentID
JOIN Inventory_TransactionHeader oldT
    ON oldT.SourceDocumentID = d.CorrectionReferenceDocumentID
WHERE t.TransactionID = @NewTID
", con, tran)

                        cmd.Parameters.AddWithValue("@NewTID", newTransactionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    Dim oldTransactionID As Integer

                    Using cmd As New SqlCommand("
SELECT TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = (
    SELECT CorrectionReferenceDocumentID
    FROM Inventory_DocumentHeader
    WHERE DocumentID = @NewDocID
)
", con, tran)

                        cmd.Parameters.AddWithValue("@NewDocID", documentID)

                        oldTransactionID = CInt(cmd.ExecuteScalar())
                    End Using
                    Using cmd As New SqlCommand("
UPDATE newTD
SET newTD.CorrectionReferenceDetailID = oldTD.DetailID
FROM Inventory_TransactionDetails newTD
INNER JOIN Inventory_DocumentDetails newDD
    ON newDD.DetailID = newTD.SourceDocumentDetailID
INNER JOIN Inventory_TransactionDetails oldTD
    ON oldTD.TransactionID = @OldTID
   AND oldTD.SourceDocumentDetailID = newDD.CorrectionReferenceDetailID
WHERE newTD.TransactionID = @NewTID
", con, tran)

                        cmd.Parameters.AddWithValue("@OldTID", oldTransactionID)
                        cmd.Parameters.AddWithValue("@NewTID", newTransactionID)
                        cmd.ExecuteNonQuery()

                    End Using
                    ' 5) Correction Queue
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_CorrectionQueue
(
    TransactionDetailID,
    DocumentDetailID,
    ChangeType,
    ProductID,
    NewQuantity,
    NewUnitCost,
    CostGroupID,
    StatusID,
    CreatedAt,
    ScopeCode
)
SELECT
    oldTD.DetailID,   -- لو موجود = EDIT
    newDD.DetailID,

    CASE 
        WHEN newDD.CorrectionReferenceDetailID IS NULL THEN 'ADD'
        ELSE 'EDIT'
    END,

    newTD.ProductID,
    newTD.Quantity,
    newTD.UnitCost,

    ISNULL(oldTD.CostGroupID, NEWID()),

    22,
    GETDATE(),
    'PUR'

FROM Inventory_TransactionDetails newTD
INNER JOIN Inventory_DocumentDetails newDD
    ON newDD.DetailID = newTD.SourceDocumentDetailID

LEFT JOIN Inventory_TransactionDetails oldTD
    ON oldTD.SourceDocumentDetailID = newDD.CorrectionReferenceDetailID
", con, tran)

                        cmd.Parameters.AddWithValue("@NewTID", newTransactionID)
                        cmd.ExecuteNonQuery()
                    End Using
                    '========================================
                    ' 🔥 الصفوف المحذوفة (Reverse)
                    '========================================
                    Dim deletedIDs As String = ""

                    If deletedOriginalDetailIDs IsNot Nothing AndAlso deletedOriginalDetailIDs.Count > 0 Then
                        deletedIDs = String.Join(",", deletedOriginalDetailIDs)
                    Else
                        deletedIDs = "0" ' عشان ما يجيب شيء
                    End If
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_CorrectionQueue
(
    TransactionDetailID,
    DocumentDetailID,
    ChangeType,
    ProductID,
    NewQuantity,
    NewUnitCost,
    CostGroupID,
    StatusID,
    CreatedAt,
    ScopeCode
)
SELECT
    tOld.DetailID,
    NULL,
    'DELETE',
    tOld.ProductID,
    0,
    tOld.UnitCost,
    tOld.CostGroupID,
    22,
    GETDATE(),
    'PUR'
FROM Inventory_TransactionDetails tOld
WHERE tOld.TransactionID = @OldTID
AND tOld.SourceDocumentDetailID IN (" & deletedIDs & ")
", con, tran)

                        cmd.Parameters.AddWithValue("@OldTID", oldTransactionID)
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
    Private Function BuildChangedOnlyTable(details As DataTable) As DataTable

        Dim dtChanges As DataTable = details.Clone()

        For Each row As DataRow In details.Rows

            If row.RowState = DataRowState.Deleted Then Continue For

            Dim isChanged As Boolean = False

            If details.Columns.Contains("IsChanged") AndAlso
               Not IsDBNull(row("IsChanged")) Then
                isChanged = CBool(row("IsChanged"))
            End If

            Dim isNew As Boolean =
                (Not details.Columns.Contains("DetailID")) OrElse
                IsDBNull(row("DetailID")) OrElse
                CInt(If(IsDBNull(row("DetailID")), 0, row("DetailID"))) = 0

            If isChanged OrElse isNew Then
                dtChanges.ImportRow(row)
            End If

        Next

        Return dtChanges

    End Function
End Class