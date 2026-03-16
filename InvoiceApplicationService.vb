Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.Configuration

Public Class InvoiceApplicationService

    Public Function SaveInvoiceDraft(
    header As InvoiceHeaderInput,
    details As DataTable,
    executedByEmployeeID As Integer
) As Integer

        Using con As New SqlConnection(
        ConfigurationManager.ConnectionStrings("MainDB").ConnectionString)

            con.Open()

            Using tran = con.BeginTransaction()

                Dim invoiceNo As String

                ' توليد رقم الفاتورة
                Using cmdNo As New SqlCommand("cfg.GenerateInvoiceNumber", con, tran)
                    cmdNo.CommandType = CommandType.StoredProcedure
                    cmdNo.Parameters.AddWithValue("@InvoiceType", "SAL")

                    Dim outParam As New SqlParameter("@NewNumber", SqlDbType.NVarChar, 50)
                    outParam.Direction = ParameterDirection.Output
                    cmdNo.Parameters.Add(outParam)

                    cmdNo.ExecuteNonQuery()
                    invoiceNo = outParam.Value.ToString()
                End Using

                Try

                    Dim documentID As Integer

                    ' ======================================
                    ' إدخال الهيدر (متوافق مع الجدول)
                    ' ======================================
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_DocumentHeader
(
    DocumentType,
    DocumentNo,
    DocumentDate,
    DueDate,
    PartnerID,
    CurrencyID,
    ExchangeRate,
    TaxReasonID,
    TotalAmount,
    TotalDiscount,
    TotalTax,
    TotalTaxableAmount,
    PaidAmount,
    RemainingAmount,
    DeliveryDate,
    DriverName,
    VehicleNo,
    Notes,
    PaymentMethodID,
    TaxTypeID,
    PaymentTermID,
    StatusID,
    IsTaxInclusive,
    IsOutbound,
    GrandTotal,
    IsInventoryPosted,
    IsZatcaReported,
    CreatedAt
)
OUTPUT INSERTED.DocumentID
VALUES
(
    @DocumentType,
    @DocumentNo,
    @DocumentDate,
    @DueDate,
    @PartnerID,
    @CurrencyID,
    @ExchangeRate,
    @TaxReasonID,
    @TotalAmount,
    @TotalDiscount,
    @TotalTax,
    @TotalTaxableAmount,
    @PaidAmount,
    @RemainingAmount,
    NULL,
    @DriverName,
    @VehicleNo,
    @Notes,
    @PaymentMethodID,
    @TaxTypeID,
    @PaymentTermID,
    @StatusID,
    @IsTaxInclusive,
    @IsOutbound,
    @GrandTotal,
    0,
    0,
    GETDATE()
)", con, tran)

                        cmd.Parameters.AddWithValue("@DocumentType", header.DocumentType)
                        cmd.Parameters.AddWithValue("@DocumentNo", invoiceNo)
                        cmd.Parameters.AddWithValue("@DocumentDate", header.DocumentDate)
                        cmd.Parameters.AddWithValue("@DueDate", header.DueDate)

                        cmd.Parameters.AddWithValue("@PartnerID",
                        If(header.PartnerID.HasValue, header.PartnerID.Value, DBNull.Value))

                        cmd.Parameters.AddWithValue("@CurrencyID", header.CurrencyID)
                        cmd.Parameters.AddWithValue("@ExchangeRate", header.ExchangeRate)

                        cmd.Parameters.AddWithValue("@TaxReasonID", DBNull.Value)

                        cmd.Parameters.AddWithValue("@TotalAmount", header.TotalAmount)
                        cmd.Parameters.AddWithValue("@TotalDiscount", header.TotalDiscount)
                        cmd.Parameters.AddWithValue("@TotalTax", header.TotalTax)
                        cmd.Parameters.AddWithValue("@TotalTaxableAmount", header.TotalTaxableAmount)
                        cmd.Parameters.AddWithValue("@PaidAmount", header.PaidAmount)
                        cmd.Parameters.AddWithValue("@RemainingAmount", header.RemainingAmount)

                        cmd.Parameters.AddWithValue("@DriverName",
                        If(String.IsNullOrWhiteSpace(header.DriverName),
                           DBNull.Value, header.DriverName))

                        cmd.Parameters.AddWithValue("@VehicleNo",
                        If(String.IsNullOrWhiteSpace(header.VehicleNo),
                           DBNull.Value, header.VehicleNo))

                        cmd.Parameters.AddWithValue("@Notes", header.Notes)
                        cmd.Parameters.AddWithValue("@PaymentMethodID", header.PaymentMethodID)
                        cmd.Parameters.AddWithValue("@TaxTypeID", header.TaxTypeID)

                        cmd.Parameters.AddWithValue("@PaymentTermID",
                        If(header.PaymentTermID.HasValue,
                           header.PaymentTermID.Value,
                           DBNull.Value))

                        cmd.Parameters.AddWithValue("@StatusID", header.StatusID)
                        cmd.Parameters.AddWithValue("@IsTaxInclusive", header.IsTaxInclusive)
                        cmd.Parameters.AddWithValue("@IsOutbound", header.IsOutbound)
                        cmd.Parameters.AddWithValue("@GrandTotal", header.GrandTotal)

                        documentID = CInt(cmd.ExecuteScalar())
                    End Using

                    ' ======================================
                    ' إدخال التفاصيل
                    ' ======================================
                    For Each r As DataRow In details.Rows

                        Using cmd As New SqlCommand("
INSERT INTO Inventory_DocumentDetails
(
    DocumentID,
    ProductID,
    UnitID,
    Quantity,
    UnitPrice,
    GrossAmount,
    DiscountRate,
    DiscountAmount,
    NetAmount,
    TaxRate,
    TaxAmount,
    TaxableAmount,
    LineTotal,
    TaxTypeID,
    SourceStoreID,
    SourceLoadingOrderDetailID
)
VALUES
(
    @DocumentID,
    @ProductID,
    @UnitID,
    @Quantity,
    @UnitPrice,
    @GrossAmount,
    @DiscountRate,
    @DiscountAmount,
    @NetAmount,
    @TaxRate,
    @TaxAmount,
    @TaxableAmount,
    @LineTotal,
    @TaxTypeID,
    @SourceStoreID,
    @SourceLoadingOrderDetailID
)", con, tran)

                            cmd.Parameters.AddWithValue("@DocumentID", documentID)
                            cmd.Parameters.AddWithValue("@ProductID", r("ProductID"))
                            cmd.Parameters.AddWithValue("@UnitID", r("UnitID"))
                            cmd.Parameters.AddWithValue("@Quantity", r("Quantity"))
                            cmd.Parameters.AddWithValue("@UnitPrice", r("UnitPrice"))
                            cmd.Parameters.AddWithValue("@GrossAmount", r("GrossAmount"))
                            cmd.Parameters.AddWithValue("@DiscountRate", r("DiscountRate"))
                            cmd.Parameters.AddWithValue("@DiscountAmount", r("DiscountAmount"))
                            cmd.Parameters.AddWithValue("@NetAmount", r("NetAmount"))
                            cmd.Parameters.AddWithValue("@TaxRate", r("TaxRate"))
                            cmd.Parameters.AddWithValue("@TaxAmount", r("TaxAmount"))
                            cmd.Parameters.AddWithValue("@TaxableAmount", r("TaxableAmount"))
                            cmd.Parameters.AddWithValue("@LineTotal", r("LineTotal"))
                            cmd.Parameters.AddWithValue("@TaxTypeID", r("TaxTypeID"))
                            cmd.Parameters.AddWithValue("@SourceStoreID", r("SourceStoreID"))

                            cmd.Parameters.AddWithValue("@SourceLoadingOrderDetailID",
                            If(r.Table.Columns.Contains("SourceLoadingOrderDetailID") AndAlso
                               Not IsDBNull(r("SourceLoadingOrderDetailID")),
                               r("SourceLoadingOrderDetailID"),
                               DBNull.Value))

                            cmd.ExecuteNonQuery()
                        End Using

                    Next

                    tran.Commit()
                    Return documentID

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Private Sub ShowUserError(message As String)
        MessageBox.Show(
        message,
        "تنبيه",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    )
    End Sub
    Public Function UpdateInvoiceDraft(
    header As InvoiceHeaderInput,
    details As DataTable,
    executedByEmployeeID As Integer
) As Integer

        Using con As New SqlConnection(
        ConfigurationManager.ConnectionStrings("MainDB").ConnectionString)

            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    ' ======================================
                    ' 1️⃣ التحقق من إمكانية التعديل
                    ' ======================================
                    Using chk As New SqlCommand("
SELECT StatusID, IsZatcaReported
FROM Inventory_DocumentHeader
WHERE DocumentID = @DocumentID
", con, tran)

                        chk.Parameters.AddWithValue("@DocumentID", header.DocumentID)

                        Using rd = chk.ExecuteReader()

                            If Not rd.Read() Then
                                Throw New Exception("الفاتورة غير موجودة.")
                            End If

                            If CBool(rd("IsZatcaReported")) Then
                                Throw New Exception(
                                "لا يمكن تعديل الفاتورة." & Environment.NewLine &
                                "تم إرسالها إلى هيئة الزكاة."
                            )
                            End If

                        End Using
                    End Using

                    ' ======================================
                    ' 2️⃣ تحديث الهيدر (متوافق مع الجدول)
                    ' ======================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader SET
    DocumentDate = @DocumentDate,
    DueDate = @DueDate,
    PartnerID = @PartnerID,
    CurrencyID = @CurrencyID,
    ExchangeRate = @ExchangeRate,
    TaxReasonID = @TaxReasonID,
    TotalAmount = @TotalAmount,
    TotalDiscount = @TotalDiscount,
    TotalTax = @TotalTax,
    TotalTaxableAmount = @TotalTaxableAmount,
    PaidAmount = @PaidAmount,
    RemainingAmount = @RemainingAmount,
    DriverName = @DriverName,
    VehicleNo = @VehicleNo,
    Notes = @Notes,
    PaymentMethodID = @PaymentMethodID,
    TaxTypeID = @TaxTypeID,
    PaymentTermID = @PaymentTermID,
    IsTaxInclusive = @IsTaxInclusive,
    IsOutbound = @IsOutbound,
    GrandTotal = @GrandTotal
WHERE DocumentID = @DocumentID
", con, tran)

                        cmd.Parameters.AddWithValue("@DocumentID", header.DocumentID)
                        cmd.Parameters.AddWithValue("@DocumentDate", header.DocumentDate)
                        cmd.Parameters.AddWithValue("@DueDate", header.DueDate)

                        cmd.Parameters.AddWithValue("@PartnerID",
                        If(header.PartnerID.HasValue,
                           header.PartnerID.Value,
                           DBNull.Value))

                        cmd.Parameters.AddWithValue("@CurrencyID", header.CurrencyID)
                        cmd.Parameters.AddWithValue("@ExchangeRate", header.ExchangeRate)

                        cmd.Parameters.AddWithValue("@TaxReasonID",
                        If(header.TaxReasonID.HasValue,
                           header.TaxReasonID.Value,
                           DBNull.Value))

                        cmd.Parameters.AddWithValue("@TotalAmount", header.TotalAmount)
                        cmd.Parameters.AddWithValue("@TotalDiscount", header.TotalDiscount)
                        cmd.Parameters.AddWithValue("@TotalTax", header.TotalTax)
                        cmd.Parameters.AddWithValue("@TotalTaxableAmount", header.TotalTaxableAmount)
                        cmd.Parameters.AddWithValue("@PaidAmount", header.PaidAmount)
                        cmd.Parameters.AddWithValue("@RemainingAmount", header.RemainingAmount)

                        cmd.Parameters.AddWithValue("@DriverName",
                        If(String.IsNullOrWhiteSpace(header.DriverName),
                           DBNull.Value, header.DriverName))

                        cmd.Parameters.AddWithValue("@VehicleNo",
                        If(String.IsNullOrWhiteSpace(header.VehicleNo),
                           DBNull.Value, header.VehicleNo))

                        cmd.Parameters.AddWithValue("@Notes", header.Notes)
                        cmd.Parameters.AddWithValue("@PaymentMethodID", header.PaymentMethodID)
                        cmd.Parameters.AddWithValue("@TaxTypeID", header.TaxTypeID)

                        cmd.Parameters.AddWithValue("@PaymentTermID",
                        If(header.PaymentTermID.HasValue,
                           header.PaymentTermID.Value,
                           DBNull.Value))

                        cmd.Parameters.AddWithValue("@IsTaxInclusive", header.IsTaxInclusive)
                        cmd.Parameters.AddWithValue("@IsOutbound", header.IsOutbound)
                        cmd.Parameters.AddWithValue("@GrandTotal", header.GrandTotal)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' ======================================
                    ' 3️⃣ حذف التفاصيل القديمة
                    ' ======================================
                    Using delCmd As New SqlCommand("
DELETE FROM Inventory_DocumentDetails
WHERE DocumentID = @DocumentID
", con, tran)

                        delCmd.Parameters.AddWithValue("@DocumentID", header.DocumentID)
                        delCmd.ExecuteNonQuery()

                    End Using

                    ' ======================================
                    ' 4️⃣ إعادة إدخال التفاصيل
                    ' ======================================
                    For Each r As DataRow In details.Rows

                        Using cmd As New SqlCommand("
INSERT INTO Inventory_DocumentDetails
(
    DocumentID,
    ProductID,
    UnitID,
    Quantity,
    UnitPrice,
    GrossAmount,
    DiscountRate,
    DiscountAmount,
    NetAmount,
    TaxRate,
    TaxAmount,
    TaxableAmount,
    LineTotal,
    TaxTypeID,
    SourceStoreID,
    SourceLoadingOrderDetailID
)
VALUES
(
    @DocumentID,
    @ProductID,
    @UnitID,
    @Quantity,
    @UnitPrice,
    @GrossAmount,
    @DiscountRate,
    @DiscountAmount,
    @NetAmount,
    @TaxRate,
    @TaxAmount,
    @TaxableAmount,
    @LineTotal,
    @TaxTypeID,
    @SourceStoreID,
    @SourceLoadingOrderDetailID
)", con, tran)

                            cmd.Parameters.AddWithValue("@DocumentID", header.DocumentID)
                            cmd.Parameters.AddWithValue("@ProductID", r("ProductID"))
                            cmd.Parameters.AddWithValue("@UnitID", r("UnitID"))
                            cmd.Parameters.AddWithValue("@Quantity", r("Quantity"))
                            cmd.Parameters.AddWithValue("@UnitPrice", r("UnitPrice"))
                            cmd.Parameters.AddWithValue("@GrossAmount", r("GrossAmount"))
                            cmd.Parameters.AddWithValue("@DiscountRate", r("DiscountRate"))
                            cmd.Parameters.AddWithValue("@DiscountAmount", r("DiscountAmount"))
                            cmd.Parameters.AddWithValue("@NetAmount", r("NetAmount"))
                            cmd.Parameters.AddWithValue("@TaxRate", r("TaxRate"))
                            cmd.Parameters.AddWithValue("@TaxAmount", r("TaxAmount"))
                            cmd.Parameters.AddWithValue("@TaxableAmount", r("TaxableAmount"))
                            cmd.Parameters.AddWithValue("@LineTotal", r("LineTotal"))
                            cmd.Parameters.AddWithValue("@TaxTypeID", r("TaxTypeID"))
                            cmd.Parameters.AddWithValue("@SourceStoreID", r("SourceStoreID"))

                            cmd.Parameters.AddWithValue("@SourceLoadingOrderDetailID",
                            If(r.Table.Columns.Contains("SourceLoadingOrderDetailID") AndAlso
                               Not IsDBNull(r("SourceLoadingOrderDetailID")),
                               r("SourceLoadingOrderDetailID"),
                               DBNull.Value))

                            cmd.ExecuteNonQuery()

                        End Using

                    Next

                    tran.Commit()
                    Return header.DocumentID

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function


    Public Sub SendAndPostInvoice(
    documentID As Integer,
    executedByEmployeeID As Integer,
    isSimplified As Boolean,
    autoClearInSimulation As Boolean
)

        Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("MainDB").ConnectionString)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' =========================================================
                    ' 1) Validate Invoice (exists, Outbound, NEW only)
                    ' =========================================================
                    Dim isOutbound As Boolean
                    Dim currentStatus As Integer
                    Dim docNo As String
                    Dim totalAmount As Decimal

                    Using cmd As New SqlCommand("
SELECT IsOutbound, StatusID, DocumentNo, TotalAmount
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)
                        cmd.Parameters.AddWithValue("@ID", documentID)

                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then Throw New Exception("الفاتورة غير موجودة")
                            isOutbound = CBool(rd("IsOutbound"))
                            currentStatus = CInt(rd("StatusID"))
                            docNo = Convert.ToString(rd("DocumentNo"))
                            totalAmount = Convert.ToDecimal(rd("TotalAmount"))
                        End Using
                    End Using

                    If Not isOutbound Then Throw New Exception("لا يتم إرسال هذا المستند للهيئة (ليس Outbound)")
                    If currentStatus <> 2 Then Throw New Exception("لا يمكن الإرسال: الفاتورة يجب أن تكون NEW فقط")

                    ' =========================================================
                    ' 2) Get LOID from invoice details (single LO per invoice)
                    ' =========================================================
                    Dim loID As Integer

                    Using cmd As New SqlCommand("
SELECT TOP 1 LOD.LOID
FROM Inventory_DocumentDetails D
INNER JOIN Logistics_LoadingOrderDetail LOD
    ON LOD.LoadingOrderDetailID = D.SourceLoadingOrderDetailID
WHERE D.DocumentID = @DocID
GROUP BY LOD.LOID
", con, tran)
                        cmd.Parameters.AddWithValue("@DocID", documentID)

                        Dim r = cmd.ExecuteScalar()
                        If r Is Nothing OrElse IsDBNull(r) Then Throw New Exception("لا يوجد تحميل مرتبط بهذه الفاتورة")
                        loID = Convert.ToInt32(r)
                    End Using

                    ' تأكيد أن الفاتورة لا تحتوي LOID مختلف (سلامة)
                    Using cmd As New SqlCommand("
IF EXISTS (
    SELECT 1
    FROM (
        SELECT DISTINCT LOD.LOID
        FROM Inventory_DocumentDetails D
        INNER JOIN Logistics_LoadingOrderDetail LOD
            ON LOD.LoadingOrderDetailID = D.SourceLoadingOrderDetailID
        WHERE D.DocumentID = @DocID
    ) X
    HAVING COUNT(*) > 1
)
    THROW 50001, N'الفاتورة مرتبطة بأكثر من أمر تحميل، غير مسموح', 1;
", con, tran)
                        cmd.Parameters.AddWithValue("@DocID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' =========================================================
                    ' 2.2) Validate SR rules (ONE SR per invoice + FULL loaded qty for that SR)
                    ' =========================================================
                    Dim sqlValidate As String = "
/* (A) Invoice must contain ONE SR only */
IF EXISTS
(
    SELECT 1
    FROM
    (
        SELECT DISTINCT SRD.SRID
        FROM Inventory_DocumentDetails D
        INNER JOIN Logistics_LoadingOrderDetail LOD
            ON LOD.LoadingOrderDetailID = D.SourceLoadingOrderDetailID
        INNER JOIN Business_SRD SRD
            ON SRD.SRDID = LOD.SourceDetailID
        WHERE D.DocumentID = @DocID
    ) X
    HAVING COUNT(*) > 1
)
    THROW 50030, N'غير مسموح: الفاتورة تحتوي أكثر من طلب مبيعات (SR). يجب أن تكون الفاتورة لطلب واحد فقط.', 1;

/* (B) Full loaded qty for that SR within this LO must be invoiced */
DECLARE @SRID INT;

SELECT @SRID = MIN(SRD.SRID)
FROM Inventory_DocumentDetails D
INNER JOIN Logistics_LoadingOrderDetail LOD
    ON LOD.LoadingOrderDetailID = D.SourceLoadingOrderDetailID
INNER JOIN Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
WHERE D.DocumentID = @DocID;

IF @SRID IS NULL
    THROW 50031, N'لا توجد أسطر مفوترة مرتبطة بأمر تحميل.', 1;

DECLARE @LoadedSum DECIMAL(18,6);
DECLARE @InvoicedSum DECIMAL(18,6);

SELECT
    @LoadedSum = ISNULL(SUM(LOD.LoadedQty),0)
FROM Logistics_LoadingOrderDetail LOD
INNER JOIN Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
  AND SRD.SRID = @SRID;

SELECT
    @InvoicedSum = ISNULL(SUM(D.Quantity),0)
FROM Inventory_DocumentDetails D
INNER JOIN Logistics_LoadingOrderDetail LOD
    ON LOD.LoadingOrderDetailID = D.SourceLoadingOrderDetailID
INNER JOIN Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
WHERE D.DocumentID = @DocID
  AND SRD.SRID = @SRID;

IF ABS(@LoadedSum - @InvoicedSum) > 0.000001
    THROW 50032, N'لا يسمح بفوترة جزئية لطلب المبيعات داخل أمر التحميل. يجب فوترة كامل الكمية المحملة لهذا الطلب.', 1;

/* (C) Safety: SR in invoice must belong to this LO */
IF NOT EXISTS
(
    SELECT 1
    FROM Logistics_LoadingOrderSR LOSR
    WHERE LOSR.LOID = @LOID
      AND LOSR.SRID = @SRID
)
    THROW 50033, N'خطأ ترابط: الطلب (SR) المفوتر غير مرتبط بأمر التحميل.', 1;
"

                    Using cmdValidate As New SqlCommand(sqlValidate, con, tran)
                        cmdValidate.Parameters.AddWithValue("@DocID", documentID)
                        cmdValidate.Parameters.AddWithValue("@LOID", loID)
                        cmdValidate.ExecuteNonQuery()
                    End Using

                    ' =========================================================
                    ' 3) Get TransactionID for this LO (exactly one)
                    ' =========================================================
                    Dim transactionID As Integer
                    Using cmd As New SqlCommand("
SELECT TOP 1 TH.TransactionID
FROM Inventory_TransactionHeader TH
WHERE TH.SourceDocumentID = @LOID
ORDER BY TH.TransactionID DESC
", con, tran)
                        cmd.Parameters.AddWithValue("@LOID", loID)

                        Dim r = cmd.ExecuteScalar()
                        If r Is Nothing OrElse IsDBNull(r) Then
                            Throw New Exception("لا يوجد ترانسكشن مخزني مرتبط بأمر التحميل (LOD)")
                        End If
                        transactionID = Convert.ToInt32(r)
                    End Using

                    ' =========================================================
                    ' 4) Create Document_Link (SAL ↔ LOD) if missing
                    ' =========================================================
                    Using cmdLink As New SqlCommand("
IF NOT EXISTS (
    SELECT 1
    FROM Document_Link
    WHERE SourceDocumentID = @SAL
      AND SourceType = 'SAL'
      AND TargetDocumentID = @LOD
      AND TargetType = 'LOD'
      AND LinkType = 'INVOICED'
)
BEGIN
    INSERT INTO Document_Link
    (
        SourceDocumentID, SourceType,
        TargetDocumentID, TargetType,
        LinkType, CreatedAt
    )
    VALUES
    (
        @SAL, 'SAL',
        @LOD, 'LOD',
        'INVOICED', SYSDATETIME()
    )
END
", con, tran)
                        cmdLink.Parameters.AddWithValue("@SAL", documentID)
                        cmdLink.Parameters.AddWithValue("@LOD", loID)
                        cmdLink.ExecuteNonQuery()
                    End Using

                    ' =========================================================
                    ' 5) ZATCA record (kept as your current approach)
                    ' =========================================================
                    Dim invoiceTypeCode As String = "388"

                    Dim previousHash As String = Nothing
                    Using cmd As New SqlCommand("
SELECT TOP 1 InvoiceHash
FROM Inventory_ZatcaDocument
WHERE InvoiceHash IS NOT NULL
  AND LTRIM(RTRIM(InvoiceHash)) <> ''
ORDER BY CreatedAt DESC, ZatcaID DESC
", con, tran)
                        Dim r = cmd.ExecuteScalar()
                        If r IsNot Nothing AndAlso Not IsDBNull(r) Then
                            previousHash = Convert.ToString(r)
                        End If
                    End Using

                    If String.IsNullOrWhiteSpace(previousHash) Then
                        previousHash = ComputeSha256Base64("0")
                    End If

                    Dim experimentalPayload As String =
                    $"DOC={documentID}|NO={docNo}|AMT={totalAmount}|UTC={DateTime.UtcNow:O}"
                    Dim invoiceHash As String = ComputeSha256Base64(experimentalPayload)

                    Dim zatcaID As Integer
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_ZatcaDocument
(
    DocumentID,
    UUID,
    InvoiceTypeCode,
    IsSimplified,
    InvoiceHash,
    PreviousInvoiceHash,
    ZatcaStatus,
    CreatedAt,
    ReportedAt
)
VALUES
(
    @DocID,
    NEWID(),
    @InvoiceTypeCode,
    @IsSimplified,
    @InvoiceHash,
    @PrevHash,
    17,
    SYSDATETIME(),
    SYSDATETIME()
);

SELECT SCOPE_IDENTITY();
", con, tran)
                        cmd.Parameters.AddWithValue("@DocID", documentID)
                        cmd.Parameters.AddWithValue("@InvoiceTypeCode", invoiceTypeCode)
                        cmd.Parameters.AddWithValue("@IsSimplified", If(isSimplified, 1, 0))
                        cmd.Parameters.AddWithValue("@InvoiceHash", invoiceHash)
                        cmd.Parameters.AddWithValue("@PrevHash", previousHash)
                        zatcaID = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    Using cmd As New SqlCommand("
INSERT INTO Inventory_ZatcaTaxTotals
(
    ZatcaID,
    TaxableAmount,
    TaxAmount,
    TaxCategoryCode,
    TaxRate
)
SELECT
    @ZID,
    SUM(D.TaxableAmount),
    SUM(D.TaxAmount),
    TT.TaxCategoryCode,
    MAX(D.TaxRate)
FROM Inventory_DocumentDetails D
INNER JOIN Master_TaxType TT
    ON TT.TaxTypeID = D.TaxTypeID
WHERE D.DocumentID = @DocID
GROUP BY TT.TaxCategoryCode
", con, tran)
                        cmd.Parameters.AddWithValue("@ZID", zatcaID)
                        cmd.Parameters.AddWithValue("@DocID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SqlCommand("
UPDATE H
SET TotalNetAmount = X.SumNet
FROM Inventory_DocumentHeader H
CROSS APPLY (
    SELECT ISNULL(SUM(NetAmount),0) AS SumNet
    FROM Inventory_DocumentDetails
    WHERE DocumentID = H.DocumentID
) X
WHERE H.DocumentID = @ID
", con, tran)
                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' =========================================================
                    ' 6) Update Document status to REPORTED (17)
                    ' =========================================================
                    Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 17,
    IsZatcaReported = 1
WHERE DocumentID = @ID
", con, tran)
                        cmd.Parameters.AddWithValue("@ID", documentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' =========================================================
                    ' 7) Simulation-only auto-clear (optional)
                    ' =========================================================
                    If autoClearInSimulation Then
                        Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 18
WHERE DocumentID = @ID;

UPDATE Inventory_ZatcaDocument
SET ZatcaStatus = 18,
    ClearedAt = SYSDATETIME()
WHERE ZatcaID = @ZID;
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", documentID)
                            cmd.Parameters.AddWithValue("@ZID", zatcaID)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If

                    ' =========================================================
                    ' 8) Update LoadingOrder status (Fully/Partially invoiced) - CLEARED only (18)
                    ' =========================================================
                    Dim totalLoadedQty As Decimal = 0D
                    Dim totalInvoicedQty As Decimal = 0D

                    Using cmd As New SqlCommand("
SELECT ISNULL(SUM(LOD.LoadedQty),0)
FROM Logistics_LoadingOrderDetail LOD
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
", con, tran)
                        cmd.Parameters.AddWithValue("@LOID", loID)
                        totalLoadedQty = Convert.ToDecimal(cmd.ExecuteScalar())
                    End Using

                    Using cmd As New SqlCommand("
SELECT ISNULL(SUM(D.Quantity),0)
FROM Inventory_DocumentDetails D
INNER JOIN Inventory_DocumentHeader H
    ON H.DocumentID = D.DocumentID
WHERE H.DocumentType = 'SAL'
  AND H.StatusID = 18              -- ✅ CLEARED فقط
  AND D.SourceLoadingOrderDetailID IN
  (
      SELECT LoadingOrderDetailID
      FROM Logistics_LoadingOrderDetail
      WHERE LOID = @LOID
        AND LoadedQty > 0
  )
", con, tran)
                        cmd.Parameters.AddWithValue("@LOID", loID)
                        totalInvoicedQty = Convert.ToDecimal(cmd.ExecuteScalar())
                    End Using

                    Dim newLoadingStatusID As Integer = 0
                    If totalLoadedQty > 0D Then
                        If totalInvoicedQty >= totalLoadedQty Then
                            newLoadingStatusID = 7 ' INVOICED
                        ElseIf totalInvoicedQty > 0D Then
                            newLoadingStatusID = 8 ' PARTIALLY_INVOICED
                        End If
                    End If

                    If newLoadingStatusID <> 0 Then
                        Using cmd As New SqlCommand("
UPDATE Logistics_LoadingOrder
SET LoadingStatusID = @StatusID,
    ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)
                            cmd.Parameters.AddWithValue("@StatusID", newLoadingStatusID)
                            cmd.Parameters.AddWithValue("@UserID", executedByEmployeeID)
                            cmd.Parameters.AddWithValue("@LOID", loID)
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

    Public Sub ReopenRejectedInvoice(documentID As Integer, executedByEmployeeID As Integer)

        Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("MainDB").ConnectionString)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    Using cmd As New SqlCommand("
IF NOT EXISTS (SELECT 1 FROM Inventory_DocumentHeader WHERE DocumentID = @ID)
    THROW 50100, N'الفاتورة غير موجودة', 1;

IF EXISTS (SELECT 1 FROM Inventory_DocumentHeader WHERE DocumentID = @ID AND StatusID <> 19)
    THROW 50101, N'لا يمكن إعادة فتح الفاتورة إلا إذا كانت مرفوضة (REJECTED)', 1;

UPDATE Inventory_DocumentHeader
SET StatusID = 2,          -- NEW
    IsZatcaReported = 0
WHERE DocumentID = @ID;
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

    ' =========================================================
    ' Helper: Base64(SHA256(UTF8(text)))
    ' =========================================================
    Private Function ComputeSha256Base64(input As String) As String
        Using sha = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(If(input, ""))
            Dim hash = sha.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function
    Private Function GetLastZatcaHash(
    con As SqlConnection,
    tran As SqlTransaction
) As String
        Return ""
    End Function


    Private Function ComputeRealInvoiceHash(
    con As SqlConnection,
    tran As SqlTransaction,
    documentID As Integer,
    previousHash As String
) As String
        Return Guid.NewGuid().ToString()
    End Function


    Private Sub UpdateLODInvoiceStatus(
    con As SqlConnection,
    tran As SqlTransaction,
    loID As Integer,
    executedByEmployeeID As Integer
)

        Dim invoicedCount As Integer = 0
        Dim totalCount As Integer = 0

        Using cmd As New SqlCommand("
;WITH L AS (
    SELECT
        LOD.LoadingOrderDetailID,
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Inventory_DocumentDetails D
                INNER JOIN Inventory_DocumentHeader H
                    ON H.DocumentID = D.DocumentID
                WHERE D.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
                  AND H.DocumentType = 'SAL'
                  AND H.StatusID = 18  -- ✅ مفوتر فقط إذا CLEARED
            )
            THEN 1 ELSE 0
        END AS IsInvoiced
    FROM Logistics_LoadingOrderDetail LOD
    WHERE LOD.LOID = @LOID
      AND ISNULL(LOD.LoadedQty,0) > 0
)
SELECT
    SUM(IsInvoiced) AS InvoicedCount,
    COUNT(*) AS TotalCount
FROM L
", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    invoicedCount = If(IsDBNull(rd("InvoicedCount")), 0, CInt(rd("InvoicedCount")))
                    totalCount = If(IsDBNull(rd("TotalCount")), 0, CInt(rd("TotalCount")))
                End If
            End Using
        End Using

        Dim newStatusID As Integer
        If totalCount = 0 OrElse invoicedCount = 0 Then
            newStatusID = 15 ' WAITING_INVOICE
        ElseIf invoicedCount < totalCount Then
            newStatusID = 8  ' PARTIALLY_INVOICED
        Else
            newStatusID = 7  ' INVOICED (Fully Invoiced)
        End If

        Using upd As New SqlCommand("
UPDATE Logistics_LoadingOrder
SET LoadingStatusID = @StatusID,
    ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)

            upd.Parameters.AddWithValue("@StatusID", newStatusID)
            upd.Parameters.AddWithValue("@UserID", executedByEmployeeID)
            upd.Parameters.AddWithValue("@LOID", loID)
            upd.ExecuteNonQuery()
        End Using

    End Sub
    Public Sub RecalculateLOStatusAfterInvoiceDelete(
        documentID As Integer,
        executedByEmployeeID As Integer
    )

        Using con As New SqlConnection(
            ConfigurationManager.ConnectionStrings("MainDB").ConnectionString)

            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    ' 1) استخراج LOID المرتبط بالفاتورة (إن وجد)
                    Dim loIDObj As Object

                    Using cmdGetLO As New SqlCommand("
SELECT TOP 1 SourceDocumentID
FROM Document_Link
WHERE TargetDocumentID = @DocID
  AND TargetType = 'SAL'
  AND SourceType = 'LO'
", con, tran)

                        cmdGetLO.Parameters.AddWithValue("@DocID", documentID)
                        loIDObj = cmdGetLO.ExecuteScalar()
                    End Using

                    If loIDObj Is Nothing OrElse IsDBNull(loIDObj) Then
                        tran.Commit()
                        Exit Sub
                    End If

                    Dim loID As Integer = CInt(loIDObj)

                    ' 2) ✅ لا نحذف Document_Link هنا
                    ' - في سياسة الإلغاء الجديدة نحتاج الاحتفاظ بالسجلات للأثر (Audit).
                    ' - وإظهار/إخفاء الطلبات في البورد يجب أن يعتمد على Inventory_DocumentDetails
                    '   مع استثناء الفواتير الملغاة StatusID=10 (كما اتفقنا).

                    ' 3) إعادة احتساب حالة LO/LOD
                    UpdateLODInvoiceStatus(con, tran, loID, executedByEmployeeID)

                    tran.Commit()

                Catch
                    Try : tran.Rollback() : Catch : End Try
                    Throw
                End Try

            End Using
        End Using

    End Sub

End Class
