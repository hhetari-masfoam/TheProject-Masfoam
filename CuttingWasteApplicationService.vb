Imports System.Data
Imports System.Data.SqlClient

Public Class CuttingWasteApplicationService

    Private ReadOnly _connStr As String

    ' Operation types
    Private Const OP_SCRAP As Integer = 13 ' SCR
    Private Const OP_CUT As Integer = 11   ' CUT

    ' Status IDs
    Private Const STATUS_NEW As Integer = 2
    Private Const STATUS_SENT As Integer = 5
    Private Const STATUS_RECEIVED As Integer = 6
    Private Const STATUS_CANCELLED As Integer = 10

    ' Reservation
    Private Const RES_STATUS_ACTIVE As Integer = 1

    Private Const UNIT_M3 As Integer = 8
    Private Const UNIT_KG As Integer = 11

    Public Sub New(connStr As String)
        _connStr = connStr
    End Sub

#Region "DTOs"

    Public Class WasteHeaderDraft
        Public Property WasteID As Integer
        Public Property WasteCode As String

        Public Property PeriodStart As DateTime
        Public Property PeriodEnd As DateTime

        Public Property WasteTypeCode As String
        Public Property CalculationTypeCode As String
        Public Property WasteReason As String
        Public Property WastePercent As Decimal?

        Public Property SourceStoreID As Integer
        Public Property TargetStoreID As Integer
        Public Property ScrapProductID As Integer

        ' Required snapshots (per your rule)
        Public Property CurrentScrapStockQty As Decimal
        Public Property CurrentScrapAvgCost As Decimal
        Public Property WasteAvgCost As Decimal      ' = txtWasteAvgCost
        Public Property NewScrapAvgCost As Decimal
    End Class

    Public Class WasteTotals
        Public Property TotalWasteWeightKG As Decimal
        Public Property TotalWasteVolumeM3 As Decimal
        Public Property TotalWasteValue As Decimal
        Public Property TotalScrapValue As Decimal
    End Class
    Public Class ExecuteWasteResult
        Public Property WasteID As Integer
        Public Property WasteCode As String
        Public Property TransactionID As Integer
    End Class
#End Region

#Region "Public API"

    Public Function SaveDraftAndReserve(
        header As WasteHeaderDraft,
        totals As WasteTotals,
        details As DataTable,
        userID As Integer
    ) As Integer

        Using con As New SqlConnection(_connStr)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    ValidateDraftInputs(header, details)

                    Dim scrapUnitID As Integer = GetProductStorageUnitID(con, tran, header.ScrapProductID)
                    If scrapUnitID <> UNIT_KG AndAlso scrapUnitID <> UNIT_M3 Then
                        Throw New Exception("هذه النسخة تدعم ال��كراب بوحدة KG أو M3 فقط.")
                    End If

                    ' Validate current status if existing
                    If header.WasteID > 0 Then
                        Dim st = GetWasteStatus(con, tran, header.WasteID)
                        If st <> STATUS_NEW Then Throw New Exception("لا يمكن الحفظ: المستند ليس في حالة NEW.")
                        Dim existingTrnID = GetWasteTransactionID(con, tran, header.WasteID)
                        If existingTrnID <> 0 Then Throw New Exception("لا يمكن الحفظ: تم إنشاء حركة بالفعل.")
                    End If
                    ' PeriodStart rules:
                    ' - For NEW document (WasteID=0): must match auto computed start (and no open document exists)
                    ' - For existing document: PeriodStart must remain exactly as saved in DB
                    If header.WasteID = 0 Then
                        Dim expectedStart As DateTime = GetNextPeriodStartForStore(con, tran, header.SourceStoreID, currentWasteID:=0)

                        If header.PeriodStart <> expectedStart Then
                            Throw New Exception(
            "بداية الفترة يتم تحديدها تلقائياً ولا يمكن تغييرها." & Environment.NewLine &
            $"البداية الصحيحة: {expectedStart:yyyy-MM-dd HH:mm:ss}"
        )
                        End If
                    Else
                        ' Existing document: PeriodStart must not change
                        Using cmd As New SqlCommand("
SELECT PeriodStartDate
FROM dbo.Inventory_WasteHeader
WHERE WasteID=@W
", con, tran)
                            cmd.Parameters.AddWithValue("@W", header.WasteID)
                            Dim dbStartObj = cmd.ExecuteScalar()
                            If dbStartObj Is Nothing OrElse IsDBNull(dbStartObj) Then Throw New Exception("المستند غير موجود.")
                            Dim dbStart As DateTime = CDate(dbStartObj)

                            If header.PeriodStart <> dbStart Then
                                Throw New Exception("لا يمكن تغيير بداية الفترة لمستند محفوظ.")
                            End If
                        End Using

                        ' Also: ensure no OTHER open doc exists for same store (excluding itself)
                        Call GetNextPeriodStartForStore(con, tran, header.SourceStoreID, currentWasteID:=header.WasteID)
                    End If
                    ' Re-check availability at save time (DB truth)
                    ValidateAvailabilityAtSave(con, tran, header.SourceStoreID, details)

                    ' Ensure WasteCode
                    If header.WasteID = 0 AndAlso String.IsNullOrWhiteSpace(header.WasteCode) Then
                        header.WasteCode = GetNextCode(con, tran, "SCR")
                    End If

                    Dim newWasteID As Integer = header.WasteID

                    ' Insert / Update Header
                    If header.WasteID = 0 Then
                        newWasteID = InsertWasteHeader(con, tran, header, totals, scrapUnitID, userID)
                    Else
                        UpdateWasteHeader(con, tran, header, totals, scrapUnitID, userID)
                    End If

                    ' Replace details (simple & safe)
                    DeleteWasteDetails(con, tran, newWasteID)

                    ' Replace reservations: release all for this waste then insert current
                    DeleteAllReservationsForWaste(con, tran, newWasteID)

                    For Each row As DataRow In details.Rows
                        If row.RowState = DataRowState.Deleted Then Continue For

                        Dim productID = CInt(row("ProductID"))
                        Dim unitID = CInt(row("UnitID"))
                        Dim consumptionQty = CDec(row("ConsumptionQty"))
                        Dim wastePercent = CDec(row("WastePercent"))
                        Dim wasteQty = CDec(row("WasteQty"))
                        Dim availableSnapshot = If(details.Columns.Contains("AvailableQty"), CDec(row("AvailableQty")), 0D)
                        Dim density = If(details.Columns.Contains("Density"), CDec(row("Density")), 0D)
                        Dim notes = If(details.Columns.Contains("Notes"), Convert.ToString(row("Notes")), "")

                        Dim detailID As Integer = InsertWasteDetail(con, tran,
                                           newWasteID, productID, unitID,
                                           consumptionQty, wastePercent, wasteQty,
                                           availableSnapshot, density, notes,
                                           userID)

                        Dim unitCostAtReserve As Decimal = GetWasteDetailUnitCost(con, tran, detailID)

                        InsertReservation(con, tran,
                  productID:=productID,
                  sourceStoreID:=header.SourceStoreID,
                  reservedQty:=wasteQty,
                  sourceOperationTypeID:=OP_SCRAP,
                  sourceID:=detailID,
                  sourceDetailID:=detailID,
                  costAtReserve:=unitCostAtReserve,
                  userID:=userID)

                    Next

                    tran.Commit()
                    Return newWasteID

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Function
    Public Function ExecuteWasteInstant(
    header As WasteHeaderDraft,
    totals As WasteTotals,
    details As DataTable,
    userID As Integer
) As ExecuteWasteResult

        Using con As New SqlConnection(_connStr)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    ' =========================
                    ' 1) Validate
                    ' =========================
                    ValidateDraftInputs(header, details)

                    Dim scrapUnitID As Integer = GetProductStorageUnitID(con, tran, header.ScrapProductID)
                    If scrapUnitID <> UNIT_KG AndAlso scrapUnitID <> UNIT_M3 Then
                        Throw New Exception("هذه النسخة تدعم السكراب بوحدة KG أو M3 فقط.")
                    End If

                    ' PeriodStart must match system computed start
                    Dim expectedStart As DateTime = GetNextPeriodStartForStore(con, tran, header.SourceStoreID, currentWasteID:=0)
                    If header.PeriodStart <> expectedStart Then
                        Throw New Exception(
                        "بداية الفترة يتم تحديدها تلقائياً ولا يمكن تغييرها." & Environment.NewLine &
                        $"البداية الصحيحة: {expectedStart:yyyy-MM-dd HH:mm:ss}"
                    )
                    End If

                    ' Validate availability at execution time (NO reservations logic)
                    ValidateAvailabilityAtExecute(con, tran, header.SourceStoreID, details)

                    ' Ensure WasteCode
                    If String.IsNullOrWhiteSpace(header.WasteCode) Then
                        header.WasteCode = GetNextCode(con, tran, "SCR")
                    End If

                    ' =========================
                    ' 2) Insert WasteHeader + WasteDetails  (NO reservation)
                    ' =========================
                    Dim wasteID As Integer = InsertWasteHeader(con, tran, header, totals, scrapUnitID, userID)

                    ' Replace details (safe)
                    DeleteWasteDetails(con, tran, wasteID)

                    For Each row As DataRow In details.Rows
                        If row.RowState = DataRowState.Deleted Then Continue For

                        Dim productID = CInt(row("ProductID"))
                        Dim unitID = CInt(row("UnitID"))
                        Dim consumptionQty = CDec(row("ConsumptionQty"))
                        Dim wastePercent = CDec(row("WastePercent"))
                        Dim wasteQty = CDec(row("WasteQty"))
                        Dim availableSnapshot = If(details.Columns.Contains("AvailableQty"), CDec(row("AvailableQty")), 0D)
                        Dim density = If(details.Columns.Contains("Density"), CDec(row("Density")), 0D)
                        Dim notes = If(details.Columns.Contains("Notes"), Convert.ToString(row("Notes")), "")

                        InsertWasteDetail(
                        con, tran,
                        wasteID, productID, unitID,
                        consumptionQty, wastePercent, wasteQty,
                        availableSnapshot, density, notes,
                        userID
                    )
                    Next

                    ' =========================
                    ' 3) Create TransactionHeader/Details (SCR)
                    ' =========================
                    Dim nowDt As DateTime = DateTime.Now
                    Dim periodID As Integer = GetOpenFiscalPeriodID(con, tran, header.PeriodEnd)

                    ' Create TransactionHeader
                    Dim transactionID As Integer = InsertTransactionHeader(
                    con, tran,
                    transactionDate:=nowDt,
                    sourceDocumentID:=wasteID,
                    operationTypeID:=OP_SCRAP,
                    periodID:=periodID,
                    statusID:=STATUS_SENT,
                    userID:=userID
                )

                    ' OUT details
                    InsertTransactionOutDetailsFromWaste(
                    con, tran,
                    transactionID:=transactionID,
                    wasteID:=wasteID,
                    sourceStoreID:=header.SourceStoreID,
                    nowDt:=nowDt,
                    userID:=userID
                )

                    ' IN detail (scrap) with UnitCost = WasteAvgCost ✅
                    Dim scrapQty As Decimal = If(scrapUnitID = UNIT_KG, totals.TotalWasteWeightKG, totals.TotalWasteVolumeM3)
                    InsertTransactionInDetail(
                    con, tran,
                    transactionID:=transactionID,
                    scrapProductID:=header.ScrapProductID,
                    qty:=scrapQty,
                    unitID:=scrapUnitID,
                    unitCost:=header.WasteAvgCost,
                    targetStoreID:=header.TargetStoreID,
                    nowDt:=nowDt,
                    userID:=userID
                )

                    ' Link waste to transaction + set SentAt/SentBy (truth)
                    Using cmd As New SqlCommand("
UPDATE dbo.Inventory_WasteHeader
SET TransactionID=@T,
    StatusID=@StatusID,
    SentAt=@Now,
    SentBy=@UserID
WHERE WasteID=@W
", con, tran)
                        cmd.Parameters.AddWithValue("@T", transactionID)
                        cmd.Parameters.AddWithValue("@StatusID", STATUS_SENT)
                        cmd.Parameters.AddWithValue("@Now", nowDt)
                        cmd.Parameters.AddWithValue("@UserID", userID)
                        cmd.Parameters.AddWithValue("@W", wasteID)
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New SqlCommand("
UPDATE Inventory_TransactionHeader
SET PostingDate = SYSDATETIME()
WHERE TransactionID = @T
", con, tran)

                        cmd.Parameters.AddWithValue("@T", transactionID)
                        cmd.ExecuteNonQuery()

                    End Using

                    tran.Commit()

                    Return New ExecuteWasteResult With {
                    .WasteID = wasteID,
                    .WasteCode = header.WasteCode,
                    .TransactionID = transactionID
                }

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function
    Public Class SendWasteResult
        Public Property WasteID As Integer
        Public Property WasteCode As String
        Public Property TransactionID As Integer
    End Class
    Public Function SendWaste(wasteID As Integer, userID As Integer) As SendWasteResult
        Using con As New SqlConnection(_connStr)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    Dim nowDt As DateTime = DateTime.Now

                    Dim hdr = LoadWasteForSend(con, tran, wasteID)

                    If hdr.StatusID <> STATUS_NEW Then Throw New Exception("لا يمكن الإرسال: الحالة ليست NEW.")
                    If hdr.TransactionID <> 0 Then Throw New Exception("تم إنشاء حركة مسبقًا لهذا المستند.")

                    Dim periodID As Integer = GetOpenFiscalPeriodID(con, tran, hdr.PeriodEndDate)

                    ' Create TransactionHeader (SCR)
                    Dim transactionID As Integer = InsertTransactionHeader(
                    con, tran,
                    transactionDate:=nowDt,
                    sourceDocumentID:=wasteID,
                    operationTypeID:=OP_SCRAP,
                    periodID:=periodID,
                    statusID:=STATUS_SENT,
                    userID:=userID
                )

                    ' OUT lines
                    InsertTransactionOutDetailsFromWaste(
                    con, tran,
                    transactionID:=transactionID,
                    wasteID:=wasteID,
                    sourceStoreID:=hdr.SourceStoreID,
                    nowDt:=nowDt,
                    userID:=userID
                )

                    ' IN line (scrap)
                    Dim scrapUnitID As Integer = GetProductStorageUnitID(con, tran, hdr.ScrapProductID)

                    Dim scrapQty As Decimal
                    If scrapUnitID = UNIT_KG Then
                        scrapQty = hdr.TotalWasteWeightKG
                    ElseIf scrapUnitID = UNIT_M3 Then
                        scrapQty = hdr.TotalWasteVolumeM3
                    Else
                        Throw New Exception("سكراب مدعوم فقط KG أو M3.")
                    End If

                    ' ✅ WasteAvgCost must be stored in TransactionDetails.UnitCost to allow ledger/avg calc
                    Dim wasteAvgCost As Decimal = GetWasteAvgCost(con, tran, wasteID)

                    InsertTransactionInDetail(
                    con, tran,
                    transactionID:=transactionID,
                    scrapProductID:=hdr.ScrapProductID,
                    qty:=scrapQty,
                    unitID:=scrapUnitID,
                    unitCost:=wasteAvgCost,
                    targetStoreID:=hdr.TargetStoreID,
                    nowDt:=nowDt,
                    userID:=userID
                )

                    ' Update waste to SENT + link transaction
                    Using cmd As New SqlCommand("
UPDATE dbo.Inventory_WasteHeader
SET StatusID=@NewStatus,
    SentAt=@Now,
    SentBy=@UserID,
    TransactionID=@TransactionID
WHERE WasteID=@WasteID
  AND StatusID=@OldStatus
", con, tran)
                        cmd.Parameters.AddWithValue("@NewStatus", STATUS_SENT)
                        cmd.Parameters.AddWithValue("@Now", nowDt)
                        cmd.Parameters.AddWithValue("@UserID", userID)
                        cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        cmd.Parameters.AddWithValue("@OldStatus", STATUS_NEW)

                        Dim affected = cmd.ExecuteNonQuery()
                        If affected = 0 Then Throw New Exception("فشل تحديث حالة الهالك إلى SENT.")
                    End Using
                    Dim result As New SendWasteResult With {
    .WasteID = wasteID,
    .WasteCode = hdr.WasteCode,
    .TransactionID = transactionID
}
                    tran.Commit()
                    Return result
                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function
    Private Function GetWasteAvgCost(con As SqlConnection, tran As SqlTransaction, wasteID As Integer) As Decimal
        Using cmd As New SqlCommand("
SELECT CAST(ISNULL(WasteAvgCost,0) AS DECIMAL(18,6))
FROM dbo.Inventory_WasteHeader
WHERE WasteID=@W
", con, tran)
            cmd.Parameters.AddWithValue("@W", wasteID)
            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
            Return CDec(obj)
        End Using
    End Function

    Public Sub CancelWaste(wasteID As Integer, cancelReason As String, userID As Integer)
        Using con As New SqlConnection(_connStr)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    Dim statusID As Integer
                    Dim trnID As Integer
                    Dim sentAtObj As Object

                    Using cmd As New SqlCommand("
SELECT StatusID, ISNULL(TransactionID,0) AS TransactionID, SentAt
FROM dbo.Inventory_WasteHeader
WHERE WasteID=@WasteID
", con, tran)
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then Throw New Exception("مستند الهالك غير موجود.")
                            statusID = CInt(rd("StatusID"))
                            trnID = CInt(rd("TransactionID"))
                            sentAtObj = rd("SentAt")
                        End Using
                    End Using

                    ' Allowed only in SENT or RECEIVED
                    If statusID <> STATUS_SENT AndAlso statusID <> STATUS_RECEIVED Then
                        Throw New Exception("الإلغاء مسموح فقط في حالة تم الإرسال أو تم الاستلام.")
                    End If

                    ' Must have SentAt (to satisfy CK_InvWasteHdr_SentAt)
                    If sentAtObj Is Nothing OrElse IsDBNull(sentAtObj) Then
                        Throw New Exception("بيانات غير صحيحة: SentAt فارغ رغم أن الحالة ليست NEW.")
                    End If

                    ' Cancel linked transaction if exists and not posted
                    If trnID <> 0 Then
                        Dim trnPosted As Boolean
                        Using cmd As New SqlCommand("
SELECT ISNULL(IsInventoryPosted,0)
FROM dbo.Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)
                            cmd.Parameters.AddWithValue("@T", trnID)
                            trnPosted = CBool(cmd.ExecuteScalar())
                        End Using

                        If trnPosted Then
                            Throw New Exception("لا يمكن إلغاء الهالك بعد الترحيل. يتم العكس من المستودع.")
                        End If

                        Using cmd As New SqlCommand("
UPDATE dbo.Inventory_TransactionHeader
SET StatusID=@NewStatus
WHERE TransactionID=@T
", con, tran)
                            cmd.Parameters.AddWithValue("@NewStatus", STATUS_CANCELLED) '10
                            cmd.Parameters.AddWithValue("@T", trnID)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If

                    ' Delete reservations for this document
                    DeleteAllReservationsForWaste(con, tran, wasteID)

                    ' Cancel waste header
                    Using cmd As New SqlCommand("
UPDATE dbo.Inventory_WasteHeader
SET StatusID=@StatusID,
    CancelledAt=SYSDATETIME(),
    CancelledBy=@UserID,
    CancelReason=@Reason,
    ModifiedAt=SYSDATETIME(),
    ModifiedBy=@UserID
WHERE WasteID=@WasteID
  AND StatusID IN (@Sent, @Received)
", con, tran)
                        cmd.Parameters.AddWithValue("@StatusID", STATUS_CANCELLED) '10
                        cmd.Parameters.AddWithValue("@UserID", userID)
                        cmd.Parameters.AddWithValue("@Reason", If(cancelReason, ""))
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        cmd.Parameters.AddWithValue("@Sent", STATUS_SENT)           '5
                        cmd.Parameters.AddWithValue("@Received", STATUS_RECEIVED)   '6

                        Dim affected = cmd.ExecuteNonQuery()
                        If affected = 0 Then Throw New Exception("فشل الإلغاء: تغيرت الحالة أثناء العملية.")
                    End Using

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub
#End Region

#Region "Header/Details DB"

    Private Function InsertWasteHeader(con As SqlConnection, tran As SqlTransaction,
                                      header As WasteHeaderDraft, totals As WasteTotals,
                                      scrapUnitID As Integer, userID As Integer) As Integer

        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_WasteHeader
(
 WasteCode, OperationTypeID,
 PeriodStartDate, PeriodEndDate,
 CalculatedAt, ReservedAt,
 ConsumptionOperationTypeID, ConsumptionStatusID, ConsumptionPostedFlag,
 WasteTypeCode, CalculationTypeCode,
 WasteReason, WastePercent,
 SourceStoreID, TargetStoreID, ScrapProductID,
 TotalWasteVolume_m3, TotalWasteWeight_kg,
 TotalWasteValue, TotalScrapValue,
CurrentScrapStockQty, CurrentScrapAvgCost, WasteAvgCost, NewScrapAvgCost,
 StatusID, CreatedBy, CreatedAt
)
OUTPUT INSERTED.WasteID
VALUES
(
 @WasteCode, @OpType,
 @From, @To,
 @CalcAt, @ResAt,
 @ConsOpType, @ConsStatus, @ConsPosted,
 @WasteTypeCode, @CalcTypeCode,
 @Reason, @WastePercent,
 @SourceStoreID, @TargetStoreID, @ScrapProductID,
 @TotalM3, @TotalKG,
 @TotalWasteValue, @TotalScrapValue,
@CurrentScrapStockQty, @CurrentScrapAvgCost, @WasteAvgCost, @NewScrapAvgCost,
 @StatusID, @UserID, SYSDATETIME()
)
", con, tran)

            cmd.Parameters.AddWithValue("@WasteCode", header.WasteCode)
            cmd.Parameters.AddWithValue("@OpType", OP_SCRAP)
            cmd.Parameters.AddWithValue("@From", header.PeriodStart)
            cmd.Parameters.AddWithValue("@To", header.PeriodEnd)
            cmd.Parameters.AddWithValue("@CalcAt", DateTime.Now)
            cmd.Parameters.AddWithValue("@ResAt", DateTime.Now)
            cmd.Parameters.AddWithValue("@ConsOpType", OP_CUT)
            cmd.Parameters.AddWithValue("@ConsStatus", STATUS_RECEIVED)
            cmd.Parameters.AddWithValue("@ConsPosted", 1)
            cmd.Parameters.AddWithValue("@WasteTypeCode", header.WasteTypeCode)
            cmd.Parameters.AddWithValue("@CalcTypeCode", header.CalculationTypeCode)
            cmd.Parameters.AddWithValue("@Reason", If(header.WasteReason, ""))
            cmd.Parameters.AddWithValue("@WastePercent", If(header.WastePercent.HasValue, CType(header.WastePercent.Value, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@SourceStoreID", header.SourceStoreID)
            cmd.Parameters.AddWithValue("@TargetStoreID", header.TargetStoreID)
            cmd.Parameters.AddWithValue("@ScrapProductID", header.ScrapProductID)

            cmd.Parameters.AddWithValue("@TotalM3", totals.TotalWasteVolumeM3)
            cmd.Parameters.AddWithValue("@TotalKG", totals.TotalWasteWeightKG)
            cmd.Parameters.AddWithValue("@TotalWasteValue", totals.TotalWasteValue)
            cmd.Parameters.AddWithValue("@CurrentScrapStockQty", header.CurrentScrapStockQty)
            cmd.Parameters.AddWithValue("@CurrentScrapAvgCost", header.CurrentScrapAvgCost)
            cmd.Parameters.AddWithValue("@WasteAvgCost", header.WasteAvgCost)
            cmd.Parameters.AddWithValue("@NewScrapAvgCost", header.NewScrapAvgCost)
            cmd.Parameters.AddWithValue("@TotalScrapValue", totals.TotalScrapValue)

            cmd.Parameters.AddWithValue("@StatusID", STATUS_NEW)
            cmd.Parameters.AddWithValue("@UserID", userID)

            Return CInt(cmd.ExecuteScalar())
        End Using
    End Function

    Private Sub UpdateWasteHeader(con As SqlConnection, tran As SqlTransaction,
                                 header As WasteHeaderDraft, totals As WasteTotals,
                                 scrapUnitID As Integer, userID As Integer)

        Using cmd As New SqlCommand("
UPDATE dbo.Inventory_WasteHeader
SET
 PeriodStartDate=@From,
 PeriodEndDate=@To,
 ReservedAt=@ResAt,

 WasteTypeCode=@WasteTypeCode,
 CalculationTypeCode=@CalcTypeCode,
 WasteReason=@Reason,
 WastePercent=@WastePercent,

 SourceStoreID=@SourceStoreID,
 TargetStoreID=@TargetStoreID,
 ScrapProductID=@ScrapProductID,

 TotalWasteVolume_m3=@TotalM3,
 TotalWasteWeight_kg=@TotalKG,
 TotalWasteValue=@TotalWasteValue,
 TotalScrapValue=@TotalScrapValue,

CurrentScrapStockQty=@CurrentScrapStockQty,
CurrentScrapAvgCost=@CurrentScrapAvgCost,
WasteAvgCost=@WasteAvgCost,
NewScrapAvgCost=@NewScrapAvgCost,

 ModifiedAt=SYSDATETIME(),
 ModifiedBy=@UserID
WHERE WasteID=@WasteID
  AND StatusID=@StatusID
", con, tran)

            cmd.Parameters.AddWithValue("@WasteID", header.WasteID)
            cmd.Parameters.AddWithValue("@From", header.PeriodStart)
            cmd.Parameters.AddWithValue("@To", header.PeriodEnd)
            cmd.Parameters.AddWithValue("@ResAt", DateTime.Now)
            cmd.Parameters.AddWithValue("@WasteTypeCode", header.WasteTypeCode)
            cmd.Parameters.AddWithValue("@CalcTypeCode", header.CalculationTypeCode)
            cmd.Parameters.AddWithValue("@Reason", If(header.WasteReason, ""))
            cmd.Parameters.AddWithValue("@WastePercent", If(header.WastePercent.HasValue, CType(header.WastePercent.Value, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@SourceStoreID", header.SourceStoreID)
            cmd.Parameters.AddWithValue("@TargetStoreID", header.TargetStoreID)
            cmd.Parameters.AddWithValue("@ScrapProductID", header.ScrapProductID)

            cmd.Parameters.AddWithValue("@TotalM3", totals.TotalWasteVolumeM3)
            cmd.Parameters.AddWithValue("@TotalKG", totals.TotalWasteWeightKG)
            cmd.Parameters.AddWithValue("@TotalWasteValue", totals.TotalWasteValue)
            cmd.Parameters.AddWithValue("@TotalScrapValue", totals.TotalScrapValue)

            cmd.Parameters.AddWithValue("@CurrentScrapStockQty", header.CurrentScrapStockQty)
            cmd.Parameters.AddWithValue("@CurrentScrapAvgCost", header.CurrentScrapAvgCost)
            cmd.Parameters.AddWithValue("@WasteAvgCost", header.WasteAvgCost)
            cmd.Parameters.AddWithValue("@NewScrapAvgCost", header.NewScrapAvgCost)

            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.Parameters.AddWithValue("@StatusID", STATUS_NEW)

            Dim affected = cmd.ExecuteNonQuery()
            If affected = 0 Then Throw New Exception("فشل تحديث الهيدر (ربما تغيرت الحالة).")
        End Using
    End Sub

    Private Function GetWasteDetailUnitCost(con As SqlConnection, tran As SqlTransaction, wasteDetailID As Integer) As Decimal
        Using cmd As New SqlCommand("
SELECT CAST(ISNULL(UnitCost,0) AS DECIMAL(18,6))
FROM dbo.Inventory_WasteDetails
WHERE WasteDetailID=@D
", con, tran)
            cmd.Parameters.AddWithValue("@D", wasteDetailID)
            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
            Return CDec(obj)
        End Using
    End Function

    Private Sub DeleteWasteDetails(con As SqlConnection, tran As SqlTransaction, wasteID As Integer)
        Using cmd As New SqlCommand("DELETE FROM dbo.Inventory_WasteDetails WHERE WasteID=@WasteID", con, tran)
            cmd.Parameters.AddWithValue("@WasteID", wasteID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function InsertWasteDetail(con As SqlConnection, tran As SqlTransaction,
                                  wasteID As Integer,
                                  productID As Integer,
                                  unitID As Integer,
                                  consumptionQty As Decimal,
                                  wastePercent As Decimal,
                                  wasteQty As Decimal,
                                  availableSnapshot As Decimal,
                                  density As Decimal,
                                  notes As String,
                                  userID As Integer) As Integer

        Dim wasteWeightKg As Decimal = 0D
        Dim wasteVolumeM3 As Decimal = 0D

        If unitID = UNIT_KG Then
            wasteWeightKg = wasteQty
            wasteVolumeM3 = 0D
        ElseIf unitID = UNIT_M3 Then
            wasteVolumeM3 = wasteQty
            If density <= 0D Then Throw New Exception("Density مطلوب لوحدة M3.")
            wasteWeightKg = wasteQty * density
        Else
            Throw New Exception("Unit غير مدعوم (فقط KG أو M3).")
        End If

        wasteWeightKg = Math.Round(wasteWeightKg, 6)
        wasteVolumeM3 = Math.Round(wasteVolumeM3, 6)

        Dim unitCost As Decimal = Math.Round(GetProductAvgCost(con, tran, productID, unitID), 6)
        Dim costAmount As Decimal = Math.Round(unitCost * wasteQty, 6)

        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_WasteDetails
(
 WasteID, ProductID, UnitID,
 ConsumptionQty, WastePercent, WasteQty,
 AvailableQty, Density,
 WasteWeight_kg, WasteVolume_m3,
 UnitCost, CostAmount,
 Notes,
 CreatedBy, CreatedAt
)
OUTPUT INSERTED.WasteDetailID
VALUES
(
 @WasteID, @ProductID, @UnitID,
 @ConsumptionQty, @WastePercent, @WasteQty,
 @AvailableQty, @Density,
 @WasteWeightKG, @WasteVolumeM3,
 @UnitCost, @CostAmount,
 @Notes,
 @UserID, SYSDATETIME()
)
", con, tran)

            cmd.Parameters.AddWithValue("@WasteID", wasteID)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@UnitID", unitID)
            cmd.Parameters.AddWithValue("@ConsumptionQty", consumptionQty)
            cmd.Parameters.AddWithValue("@WastePercent", wastePercent)
            cmd.Parameters.AddWithValue("@WasteQty", wasteQty)
            cmd.Parameters.AddWithValue("@AvailableQty", availableSnapshot)
            cmd.Parameters.AddWithValue("@Density", density)

            cmd.Parameters.AddWithValue("@WasteWeightKG", wasteWeightKg)
            cmd.Parameters.AddWithValue("@WasteVolumeM3", wasteVolumeM3)
            cmd.Parameters.AddWithValue("@UnitCost", unitCost)
            cmd.Parameters.AddWithValue("@CostAmount", costAmount)

            cmd.Parameters.AddWithValue("@Notes", If(notes, ""))
            cmd.Parameters.AddWithValue("@UserID", userID)

            Return CInt(cmd.ExecuteScalar())
        End Using
    End Function
#End Region

#Region "Transaction DB"

    Private Function InsertTransactionHeader(con As SqlConnection, tran As SqlTransaction,
                                            transactionDate As DateTime,
                                            sourceDocumentID As Integer,
                                            operationTypeID As Integer,
                                            periodID As Integer,
                                            statusID As Integer,
                                            userID As Integer) As Integer
        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_TransactionHeader
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
 @SourceDocumentID,
 @OperationTypeID,
 @PeriodID,
 @StatusID,
 0,
 @UserID,
 @Now,
 @Now,
 @UserID,
 0
);
SELECT SCOPE_IDENTITY();
", con, tran)

            cmd.Parameters.AddWithValue("@Now", transactionDate)
            cmd.Parameters.AddWithValue("@SourceDocumentID", sourceDocumentID)
            cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
            cmd.Parameters.AddWithValue("@PeriodID", periodID)
            cmd.Parameters.AddWithValue("@StatusID", statusID)
            cmd.Parameters.AddWithValue("@UserID", userID)

            Return CInt(cmd.ExecuteScalar())
        End Using
    End Function

    Private Sub InsertTransactionOutDetailsFromWaste(
    con As SqlConnection,
    tran As SqlTransaction,
    transactionID As Integer,
    wasteID As Integer,
    sourceStoreID As Integer,
    nowDt As DateTime,
    userID As Integer
)
        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_TransactionDetails
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
 d.WasteQty,
 d.UnitID,
 CAST(ISNULL(d.UnitCost,0) AS DECIMAL(18,6)) AS UnitCost,
 CAST(ISNULL(d.CostAmount,0) AS DECIMAL(18,6)) AS CostAmount,
 @SourceStoreID,
 NULL,
 d.WasteDetailID,
 NULL,
 @Now,
 @UserID
FROM dbo.Inventory_WasteDetails d
WHERE d.WasteID = @WasteID
  AND d.WasteQty > 0
", con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@WasteID", wasteID)
            cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)
            cmd.Parameters.AddWithValue("@Now", nowDt)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertTransactionInDetail(
    con As SqlConnection,
    tran As SqlTransaction,
    transactionID As Integer,
    scrapProductID As Integer,
    qty As Decimal,
    unitID As Integer,
    unitCost As Decimal,
    targetStoreID As Integer,
    nowDt As DateTime,
    userID As Integer
)
        Dim costAmount As Decimal = Math.Round(qty * unitCost, 6, MidpointRounding.AwayFromZero)

        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_TransactionDetails
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
VALUES
(
 @TransactionID,
 @ScrapProductID,
 @Qty,
 @UnitID,
 @UnitCost,
 @CostAmount,
 NULL,
 @TargetStoreID,
 0,
 NULL,
 @Now,
 @UserID
)
", con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@ScrapProductID", scrapProductID)
            cmd.Parameters.AddWithValue("@Qty", qty)
            cmd.Parameters.AddWithValue("@UnitID", unitID)

            cmd.Parameters.AddWithValue("@UnitCost", unitCost)
            cmd.Parameters.AddWithValue("@CostAmount", costAmount)

            cmd.Parameters.AddWithValue("@TargetStoreID", targetStoreID)
            cmd.Parameters.AddWithValue("@Now", nowDt)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
#End Region

#Region "Availability / Reservation"
    Private Sub ValidateAvailabilityAtExecute(
    con As SqlConnection,
    tran As SqlTransaction,
    storeID As Integer,
    details As DataTable
)
        Dim productIDs As New List(Of Integer)
        For Each row As DataRow In details.Rows
            If row.RowState = DataRowState.Deleted Then Continue For
            productIDs.Add(CInt(row("ProductID")))
        Next
        productIDs = productIDs.Distinct().ToList()
        If productIDs.Count = 0 Then Throw New Exception("لا توجد منتجات في التفاصيل.")

        Dim inParams As New List(Of String)
        Using cmd As New SqlCommand()
            cmd.Connection = con
            cmd.Transaction = tran

            For i = 0 To productIDs.Count - 1
                Dim pName = "@p" & i
                inParams.Add(pName)
                cmd.Parameters.AddWithValue(pName, productIDs(i))
            Next
            cmd.Parameters.AddWithValue("@StoreID", storeID)

            cmd.CommandText =
$"
SELECT
    b.ProductID,
    CAST(ISNULL(b.QtyOnHand,0) AS DECIMAL(18,6)) AS AvailableQty
FROM dbo.Inventory_Balance b
WHERE b.StoreID = @StoreID
  AND b.ProductID IN ({String.Join(",", inParams)});
"

            Dim availableMap As New Dictionary(Of Integer, Decimal)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    availableMap(CInt(rd("ProductID"))) = CDec(rd("AvailableQty"))
                End While
            End Using

            For Each row As DataRow In details.Rows
                If row.RowState = DataRowState.Deleted Then Continue For
                Dim productID As Integer = CInt(row("ProductID"))
                Dim wasteQty As Decimal = CDec(row("WasteQty"))
                Dim avail As Decimal = If(availableMap.ContainsKey(productID), availableMap(productID), 0D)
                If wasteQty > avail Then
                    Throw New Exception($"الرصيد تغير: المنتج {productID} المتاح الآن ({avail}) أقل من الهالك ({wasteQty}). أعد الحسابات.")
                End If
            Next
        End Using
    End Sub
    Private Sub ValidateAvailabilityAtSave(con As SqlConnection, tran As SqlTransaction, storeID As Integer, details As DataTable)
        Dim productIDs As New List(Of Integer)
        For Each row As DataRow In details.Rows
            If row.RowState = DataRowState.Deleted Then Continue For
            productIDs.Add(CInt(row("ProductID")))
        Next
        productIDs = productIDs.Distinct().ToList()
        If productIDs.Count = 0 Then Throw New Exception("لا توجد منتجات في التفاصيل.")

        Dim inParams As New List(Of String)
        Using cmd As New SqlCommand()
            cmd.Connection = con
            cmd.Transaction = tran

            For i = 0 To productIDs.Count - 1
                Dim pName = "@p" & i
                inParams.Add(pName)
                cmd.Parameters.AddWithValue(pName, productIDs(i))
            Next
            cmd.Parameters.AddWithValue("@StoreID", storeID)

            cmd.CommandText =
$"
WITH Res AS
(
    SELECT ProductID, SourceStoreID AS StoreID, SUM(ReservedQty) AS ReservedQty
    FROM dbo.Inventory_Reservation
    WHERE ReleasedAt IS NULL
      AND ReservationStatusID = {RES_STATUS_ACTIVE}
      AND SourceStoreID = @StoreID
      AND ProductID IN ({String.Join(",", inParams)})
    GROUP BY ProductID, SourceStoreID
)
SELECT
    b.ProductID,
    CAST(ISNULL(b.QtyOnHand,0) - ISNULL(r.ReservedQty,0) AS DECIMAL(18,6)) AS AvailableQty
FROM dbo.Inventory_Balance b
LEFT JOIN Res r
    ON r.StoreID = b.StoreID AND r.ProductID = b.ProductID
WHERE b.StoreID = @StoreID
  AND b.ProductID IN ({String.Join(",", inParams)});
"

            Dim availableMap As New Dictionary(Of Integer, Decimal)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    availableMap(CInt(rd("ProductID"))) = CDec(rd("AvailableQty"))
                End While
            End Using

            For Each row As DataRow In details.Rows
                If row.RowState = DataRowState.Deleted Then Continue For
                Dim productID As Integer = CInt(row("ProductID"))
                Dim wasteQty As Decimal = CDec(row("WasteQty"))
                Dim avail As Decimal = If(availableMap.ContainsKey(productID), availableMap(productID), 0D)
                If wasteQty > avail Then
                    Throw New Exception($"الرصيد تغير: المنتج {productID} المتاح الآن ({avail}) أقل من الهالك ({wasteQty}). أعد الحسابات.")
                End If
            Next
        End Using
    End Sub

    Private Sub InsertReservation(con As SqlConnection, tran As SqlTransaction,
                                  productID As Integer,
                                  sourceStoreID As Integer,
                                  reservedQty As Decimal,
                                  sourceOperationTypeID As Integer,
                                  sourceID As Integer,
                                  sourceDetailID As Integer,
                                  costAtReserve As Decimal,
                                  userID As Integer)
        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_Reservation
(
 ProductID, SourceStoreID, ReservedQty,
 SourceOperationTypeID, SourceID, CostAtReserve,
 ReservedAt, ReleasedAt, CreatedBy, ReservationStatusID, SourceDetailID
)
VALUES
(
 @ProductID, @SourceStoreID, @ReservedQty,
 @SourceOperationTypeID, @SourceDetailID, @CostAtReserve,
 SYSDATETIME(), NULL, @UserID, @ResStatus, @SourceDetailID
)
", con, tran)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)
            cmd.Parameters.AddWithValue("@ReservedQty", reservedQty)
            cmd.Parameters.AddWithValue("@SourceOperationTypeID", sourceOperationTypeID)
            cmd.Parameters.AddWithValue("@SourceID", sourceDetailID)
            cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
            cmd.Parameters.AddWithValue("@CostAtReserve", costAtReserve)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.Parameters.AddWithValue("@ResStatus", RES_STATUS_ACTIVE)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

#End Region

#Region "Validation / Loaders"

    Private Sub ValidateDraftInputs(header As WasteHeaderDraft, details As DataTable)
        If header Is Nothing Then Throw New Exception("header = Nothing")
        If details Is Nothing Then Throw New Exception("details = Nothing")
        If details.Rows.Count = 0 Then Throw New Exception("لا توجد تفاصيل.")
        If header.PeriodEnd < header.PeriodStart Then Throw New Exception("نهاية الفترة يجب أن تكون أكبر أو تساوي بداية الفترة.")
        If header.SourceStoreID <= 0 Then Throw New Exception("SourceStoreID غير صحيح.")
        If header.TargetStoreID <= 0 Then Throw New Exception("TargetStoreID غير صحيح.")
        If header.ScrapProductID <= 0 Then Throw New Exception("ScrapProductID غير صحيح.")
        If String.IsNullOrWhiteSpace(header.WasteTypeCode) Then Throw New Exception("نوع الهالك مطلوب.")
        If String.IsNullOrWhiteSpace(header.CalculationTypeCode) Then Throw New Exception("طريقة الحساب مطلوبة.")
    End Sub

    Private Function GetWasteStatus(con As SqlConnection, tran As SqlTransaction, wasteID As Integer) As Integer
        Using cmd As New SqlCommand("SELECT StatusID FROM dbo.Inventory_WasteHeader WHERE WasteID=@W", con, tran)
            cmd.Parameters.AddWithValue("@W", wasteID)
            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Throw New Exception("مستند الهالك غير موجود.")
            Return CInt(obj)
        End Using
    End Function

    Private Function GetWasteTransactionID(con As SqlConnection, tran As SqlTransaction, wasteID As Integer) As Integer
        Using cmd As New SqlCommand("SELECT ISNULL(TransactionID,0) FROM dbo.Inventory_WasteHeader WHERE WasteID=@W", con, tran)
            cmd.Parameters.AddWithValue("@W", wasteID)
            Return CInt(cmd.ExecuteScalar())
        End Using
    End Function

    Private Class WasteForSend
        Public Property StatusID As Integer
        Public Property SourceStoreID As Integer
        Public Property TargetStoreID As Integer
        Public Property ScrapProductID As Integer
        Public Property PeriodEndDate As DateTime
        Public Property TransactionID As Integer
        Public Property TotalWasteWeightKG As Decimal
        Public Property TotalWasteVolumeM3 As Decimal
        Public Property WasteCode As String

    End Class

    Private Function LoadWasteForSend(con As SqlConnection, tran As SqlTransaction, wasteID As Integer) As WasteForSend
        Using cmd As New SqlCommand("
SELECT
 StatusID,
 SourceStoreID,
 TargetStoreID,
 ScrapProductID,
 PeriodEndDate,
 ISNULL(TransactionID,0) AS TransactionID,
 ISNULL(TotalWasteWeight_kg,0) AS TotalWasteWeightKG,
 ISNULL(TotalWasteVolume_m3,0) AS TotalWasteVolumeM3
FROM dbo.Inventory_WasteHeader
WHERE WasteID=@W
", con, tran)
            cmd.Parameters.AddWithValue("@W", wasteID)
            Using rd = cmd.ExecuteReader()
                If Not rd.Read() Then Throw New Exception("مستند الهالك غير موجود.")
                Return New WasteForSend With {
                    .StatusID = CInt(rd("StatusID")),
                    .SourceStoreID = CInt(rd("SourceStoreID")),
                    .TargetStoreID = CInt(rd("TargetStoreID")),
                    .ScrapProductID = CInt(rd("ScrapProductID")),
                    .PeriodEndDate = CDate(rd("PeriodEndDate")),
                    .TransactionID = CInt(rd("TransactionID")),
                    .TotalWasteWeightKG = CDec(rd("TotalWasteWeightKG")),
                    .TotalWasteVolumeM3 = CDec(rd("TotalWasteVolumeM3"))
                }
            End Using
        End Using
    End Function

    Private Function GetProductStorageUnitID(con As SqlConnection, tran As SqlTransaction, productID As Integer) As Integer
        Using cmd As New SqlCommand("
SELECT StorageUnitID
FROM dbo.Master_Product
WHERE ProductID=@P
", con, tran)
            cmd.Parameters.AddWithValue("@P", productID)
            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Throw New Exception("لم يتم العثور على وحدة الصنف.")
            Return CInt(obj)
        End Using
    End Function

    Private Function GetOpenFiscalPeriodID(con As SqlConnection, tran As SqlTransaction, docDate As DateTime) As Integer
        Using cmd As New SqlCommand("
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE @DocDate >= StartDate
  AND @DocDate <= EndDate
  AND IsOpen = 1
ORDER BY StartDate DESC
", con, tran)
            cmd.Parameters.AddWithValue("@DocDate", docDate.Date)
            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then
                Throw New Exception("لا يوجد فترة مالية مفتوحة لهذا التاريخ.")
            End If
            Return CInt(result)
        End Using
    End Function

    Private Function GetNextCode(con As SqlConnection, tran As SqlTransaction, codeType As String) As String
        Using cmd As New SqlCommand("cfg.GetNextCode", con, tran)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@CodeType", codeType)
            Dim pOut As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
            pOut.Direction = ParameterDirection.Output
            cmd.Parameters.Add(pOut)
            cmd.ExecuteNonQuery()
            Return pOut.Value.ToString()
        End Using
    End Function

#End Region
    Private Function GetProductAvgCost(con As SqlConnection, tran As SqlTransaction, productID As Integer, unitID As Integer) As Decimal
        If unitID = UNIT_KG Then
            Using cmd As New SqlCommand("
SELECT CAST(ISNULL(AvgCost,0) AS DECIMAL(18,6))
FROM dbo.Master_Product
WHERE ProductID=@P
", con, tran)
                cmd.Parameters.AddWithValue("@P", productID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
                Return CDec(obj)
            End Using

        ElseIf unitID = UNIT_M3 Then
            Dim baseID As Integer
            Using cmdBase As New SqlCommand("
SELECT ISNULL(BaseProductID, ProductID)
FROM dbo.Master_Product
WHERE ProductID=@P
", con, tran)
                cmdBase.Parameters.AddWithValue("@P", productID)
                Dim objBase = cmdBase.ExecuteScalar()
                If objBase Is Nothing OrElse IsDBNull(objBase) Then baseID = productID Else baseID = CInt(objBase)
            End Using

            Using cmd As New SqlCommand("
SELECT CAST(ISNULL(AvgCostPerM3,0) AS DECIMAL(18,6))
FROM dbo.Master_FinalProductAvgCost
WHERE BaseProductID=@B
", con, tran)
                cmd.Parameters.AddWithValue("@B", baseID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
                Return CDec(obj)
            End Using
        End If

        Return 0D
    End Function



    Private Sub DeleteAllReservationsForWaste(con As SqlConnection, tran As SqlTransaction, wasteID As Integer)
        Using cmd As New SqlCommand("
DELETE R
FROM dbo.Inventory_Reservation R
INNER JOIN dbo.Inventory_WasteDetails D
    ON R.SourceID = D.WasteDetailID
WHERE D.WasteID = @WasteID
  AND R.SourceOperationTypeID = @Op
", con, tran)
            cmd.Parameters.AddWithValue("@Op", OP_SCRAP)
            cmd.Parameters.AddWithValue("@WasteID", wasteID)

            Dim affected = cmd.ExecuteNonQuery()
            ' (اختياري للتشخيص مؤقتاً)
            ' If affected = 0 Then Throw New Exception("لم يتم العثور على حجوزات للحذف لهذا المستند.")
        End Using
    End Sub
    Public Sub DeleteWasteDraft(wasteID As Integer, userID As Integer)
        Using con As New SqlConnection(_connStr)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    Dim statusID As Integer
                    Dim trnID As Integer

                    Using cmd As New SqlCommand("
SELECT StatusID, ISNULL(TransactionID,0) AS TransactionID
FROM dbo.Inventory_WasteHeader
WHERE WasteID=@WasteID
", con, tran)
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then Throw New Exception("المستند غير موجود.")
                            statusID = CInt(rd("StatusID"))
                            trnID = CInt(rd("TransactionID"))
                        End Using
                    End Using

                    If statusID <> STATUS_NEW Then Throw New Exception("الحذف مسموح فقط في حالة NEW.")
                    If trnID <> 0 Then Throw New Exception("لا يمكن الحذف: يوجد حركة مرتبطة بالمستند.")

                    ' 1) Delete reservations
                    DeleteAllReservationsForWaste(con, tran, wasteID)

                    ' 2) Delete details
                    Using cmd As New SqlCommand("
DELETE FROM dbo.Inventory_WasteDetails
WHERE WasteID=@WasteID
", con, tran)
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' 3) Delete header
                    Using cmd As New SqlCommand("
DELETE FROM dbo.Inventory_WasteHeader
WHERE WasteID=@WasteID
  AND StatusID=@StatusID
", con, tran)
                        cmd.Parameters.AddWithValue("@WasteID", wasteID)
                        cmd.Parameters.AddWithValue("@StatusID", STATUS_NEW)

                        Dim affected = cmd.ExecuteNonQuery()
                        If affected = 0 Then Throw New Exception("فشل الحذف: تغيرت الحالة أثناء العملية.")
                    End Using

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub
    Public Function GetNextPeriodStartForStore(sourceStoreID As Integer, Optional currentWasteID As Integer = 0) As DateTime
        Using con As New SqlConnection(_connStr)
            con.Open()
            Return GetNextPeriodStartForStore(con, Nothing, sourceStoreID, currentWasteID)
        End Using
    End Function

    Private Function GetNextPeriodStartForStore(con As SqlConnection, tran As SqlTransaction, sourceStoreID As Integer, currentWasteID As Integer) As DateTime
        ' 1) منع وجود مستند مفتوح آخر لنفس المخزن (2/5/6) غير المستند الحالي
        Using cmdOpen As New SqlCommand("
SELECT TOP 1 WasteID, WasteCode, StatusID, PeriodStartDate, PeriodEndDate
FROM dbo.Inventory_WasteHeader
WHERE SourceStoreID = @StoreID
  AND StatusID IN (2,5)
  AND WasteID <> @CurrentWasteID
ORDER BY CreatedAt DESC
", con, tran)
            cmdOpen.Parameters.AddWithValue("@StoreID", sourceStoreID)
            cmdOpen.Parameters.AddWithValue("@CurrentWasteID", currentWasteID)

            Using rd = cmdOpen.ExecuteReader()
                If rd.Read() Then
                    Dim wasteCode = Convert.ToString(rd("WasteCode"))
                    Dim statusID = CInt(rd("StatusID"))
                    Dim ps = CDate(rd("PeriodStartDate"))
                    Dim pe = CDate(rd("PeriodEndDate"))

                    Dim statusText As String =
                        If(statusID = STATUS_NEW, "محفوظ (NEW) ومحجوز",
                        If(statusID = STATUS_SENT, "تم الإرسال (SENT)", "تم الاستلام (RECEIVED)"))

                    Throw New Exception(
                        "لا يمكن بدء/حفظ فترة جديدة لهذا المخزن." & Environment.NewLine &
                        "يوجد مستند سابق غير منتهٍ:" & Environment.NewLine &
                        $"رقم المستند: {wasteCode}" & Environment.NewLine &
                        $"الحالة: {statusText}" & Environment.NewLine &
                        $"الفترة: {ps:yyyy-MM-dd HH:mm:ss}  إلى  {pe:yyyy-MM-dd HH:mm:ss}" & Environment.NewLine &
                        "يرجى إنهاء المستند السابق (إرسال/استلام) أو حذفه/إلغاؤه ثم المحاولة مرة أخرى."
                    )
                End If
            End Using
        End Using

        ' 2) آخر نهاية لفترة مكتملة (5/6) ثم +1 ثانية
        Dim lastEndObj As Object
        Using cmdLast As New SqlCommand("
SELECT MAX(PeriodEndDate)
FROM dbo.Inventory_WasteHeader
WHERE SourceStoreID = @StoreID
  AND StatusID IN (5,6)
", con, tran)
            cmdLast.Parameters.AddWithValue("@StoreID", sourceStoreID)
            lastEndObj = cmdLast.ExecuteScalar()
        End Using

        If lastEndObj IsNot Nothing AndAlso Not IsDBNull(lastEndObj) Then
            Dim lastEnd = CDate(lastEndObj)
            Return DateAdd(DateInterval.Second, 1, lastEnd)
        End If

        ' 3) أول عملية قص CUT لهذا المخزن
        ' 3) أول عملية قص CUT لهذا المخزن (من تفاصيل الحركة لأن الهيدر لا يحتوي مخزن)
        Dim firstCutObj As Object
        Using cmdFirstCut As New SqlCommand("
SELECT MIN(h.TransactionDate)
FROM dbo.Inventory_TransactionHeader h
WHERE h.OperationTypeID = @OpCut
  AND EXISTS
  (
      SELECT 1
      FROM dbo.Inventory_TransactionDetails d
      WHERE d.TransactionID = h.TransactionID
        AND d.SourceStoreID = @StoreID
  )
", con, tran)
            cmdFirstCut.Parameters.AddWithValue("@OpCut", OP_CUT)
            cmdFirstCut.Parameters.AddWithValue("@StoreID", sourceStoreID)
            firstCutObj = cmdFirstCut.ExecuteScalar()
        End Using

        If firstCutObj Is Nothing OrElse IsDBNull(firstCutObj) Then
            Throw New Exception("لا يمكن تحديد بداية الفترة: لا توجد أي عملية قص (CUT) لهذا المخزن.")
        End If

        Return CDate(firstCutObj)

    End Function


End Class