Imports System.Data.SqlClient

Public Class PurchaseApplicationService

    Private ReadOnly _connStr As String

    Public Sub New(connStr As String)
        _connStr = connStr
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

                    For Each row As DataRow In details.Rows

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
StatusID=2,
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
                    For Each row As DataRow In details.Rows

                        If row.RowState = DataRowState.Deleted Then Continue For

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
                                If(row.IsNull("SourceStoreID"), CType(DBNull.Value, Object), row("SourceStoreID")))
                            cmd.Parameters.AddWithValue("@Target",
                                If(row.IsNull("TargetStoreID"), CType(DBNull.Value, Object), row("TargetStoreID")))

                            cmd.Parameters.AddWithValue("@TaxType", row("TaxTypeID"))
                            cmd.Parameters.AddWithValue("@Taxable", row("TaxableAmount"))

                            cmd.ExecuteNonQuery()
                        End Using

                    Next

                    tran.Commit()
                    Return newID

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function


    Public Sub SendPurchase(documentID As Integer,
                            transactionCode As String,
                            userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

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

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub
End Class