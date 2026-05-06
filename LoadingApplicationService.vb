Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class LoadingApplicationService

    Private ReadOnly _connStr As String

Private ReadOnly _inventoryRepo As InventoryRepository

    Public Sub New(connectionString As String)
        _connStr = connectionString
        _inventoryRepo = New InventoryRepository(connectionString)
    End Sub
    Private Sub TouchLoadingOrderModifiedAt(
    con As SqlConnection,
    tran As SqlTransaction,
    loID As Integer,
    userID As Integer
)
        Using cmd As New SqlCommand("
UPDATE log.LoadingOrder
SET ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)
            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
    Private Sub SendLoadingOrder_InsideTransaction(
    loID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        '====================================================
        ' 1) جلب بيانات LO
        '====================================================
        Dim operationTypeID As Integer
        Dim storeID As Integer
        Dim seq As Integer = 1

        Using cmd As New SqlCommand("
        SELECT OperationTypeID, SourceStoreID
        FROM log.LoadingOrder
        WHERE LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)

            Using rd = cmd.ExecuteReader()
                rd.Read()
                operationTypeID = CInt(rd("OperationTypeID"))
                storeID = CInt(rd("SourceStoreID"))
            End Using

        End Using


        '====================================================
        ' 2) جلب PeriodID
        '====================================================
        Dim periodID As Integer

        Using cmd As New SqlCommand("
        SELECT TOP 1 PeriodID
        FROM cfg.FiscalPeriod
        WHERE IsOpen = 1
        ORDER BY StartDate
    ", con, tran)

            periodID = CInt(cmd.ExecuteScalar())

        End Using


        '====================================================
        ' 3) إنشاء TransactionHeader
        '====================================================
        Dim transactionID As Integer

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
            IsInventoryPosted,
            PostingDate
        )
        VALUES
        (
            SYSDATETIME(),
            @LOID,
            @OpType,
            @PeriodID,
            5,
            0,
            @UserID,
            SYSDATETIME(),
            SYSDATETIME(),
            @UserID,
            0,
            NULL
        );

        SELECT SCOPE_IDENTITY();
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@OpType", operationTypeID)
            cmd.Parameters.AddWithValue("@PeriodID", periodID)
            cmd.Parameters.AddWithValue("@UserID", userID)

            transactionID = Convert.ToInt32(cmd.ExecuteScalar())

        End Using
        Dim m3UnitID As Integer

        '========================================
        ' جلب وحدة المتر المكعب
        '========================================

        Using cmd As New SqlCommand("
SELECT UnitID
FROM md.Unit
WHERE UnitCode = 'M3'
", con, tran)

            m3UnitID = CInt(cmd.ExecuteScalar())

        End Using


        '====================================================
        ' 4) إدخال TransactionDetails
        '====================================================
        Using cmdInsert As New SqlCommand("
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
    @TID,
    LOD.ProductID,
    LOD.LoadedQty,
    P.StorageUnitID,

    CASE
        WHEN P.StorageUnitID = @M3UnitID
            THEN ISNULL(FP.AvgCostPerM3,0)

        WHEN BP.StorageUnitID = @M3UnitID
            THEN ISNULL(FP.AvgCostPerM3forFG,0)

        ELSE
            ISNULL(P.AvgCost,0)
    END AS UnitCost,

    LOD.LoadedQty *
   CASE
    WHEN P.StorageUnitID = @M3UnitID
        THEN ISNULL(FP.AvgCostPerM3,0)
    WHEN P.StorageUnitID <> @M3UnitID
         AND BP.StorageUnitID = @M3UnitID
        THEN ISNULL(FP.AvgCostPerM3forFG,0)
    ELSE
        ISNULL(P.AvgCost,0)

END AS CostAmount,

    @StoreID,
    NULL,
    LOD.LoadingOrderDetailID,
    NULL,
    SYSDATETIME(),
    @UserID

FROM log.LoadingOrderDetail LOD

JOIN md.Product P
    ON P.ProductID = LOD.ProductID

LEFT JOIN md.Product BP
    ON BP.ProductID = P.BaseProductID

LEFT JOIN inv.FinalProductAvgCost FP
    ON FP.BaseProductID = COALESCE(P.BaseProductID, P.ProductID)

WHERE LOD.LOID = @LOID
AND LOD.LoadedQty > 0
", con, tran)

            cmdInsert.Parameters.AddWithValue("@TID", transactionID)
            cmdInsert.Parameters.AddWithValue("@LOID", loID)
            cmdInsert.Parameters.AddWithValue("@StoreID", storeID)
            cmdInsert.Parameters.AddWithValue("@UserID", userID)
            cmdInsert.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            cmdInsert.ExecuteNonQuery()

        End Using


        '====================================================
        ' 7) Finalize Ledger
        '====================================================
        '       FinalizeLedgerMetadata(operationGroupID, con, tran)
        Using cmd As New SqlCommand("
UPDATE log.LoadingOrder
SET
    LoadingStatusID = 5,      
    IsInventoryPosted = 0,
    PostedAt = SYSDATETIME(),
    PostedBy = @UserID
WHERE LOID = @LOID
", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@UserID", userID)

            cmd.ExecuteNonQuery()

        End Using
        '       UpdateFinalStatuses(transactionID, con, tran)

    End Sub
    Public Sub SendLoadingOrder(loID As Integer, userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    SendLoadingOrder_InsideTransaction(loID, userID, con, tran)

                    tran.Commit()

                Catch

                    tran.Rollback()
                    Throw

                End Try

            End Using
        End Using

    End Sub
    Public Sub CancelLoadingOrder(loID As Integer, userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    CancelLoadingOrder_InsideTransaction(loID, userID, con, tran)

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub
    Private Sub CancelLoadingOrder_InsideTransaction(
    loID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        '====================================================
        ' 1) تحقق من الحالة
        '====================================================
        Dim statusID As Integer

        Using cmd As New SqlCommand("
        SELECT LoadingStatusID
        FROM log.LoadingOrder
        WHERE LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing Then Throw New Exception("LO غير موجود")

            statusID = CInt(obj)
        End Using

        ' الحالات المسموحة
        If Not (statusID = 0 OrElse statusID = 1 OrElse statusID = 2 OrElse statusID = 5 OrElse statusID = 14) Then
            Throw New Exception("لا يمكن إلغاء أمر التحميل في هذه الحالة")
        End If


        '====================================================
        ' 2) حذف TransactionDetails
        '====================================================
        Using cmd As New SqlCommand("
        DELETE FROM inv.TransactionDetails
        WHERE TransactionID IN (
            SELECT TransactionID
            FROM inv.TransactionHeader
            WHERE SourceDocumentID = @LOID
        )
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 3) حذف TransactionHeader
        '====================================================
        Using cmd As New SqlCommand("
        DELETE FROM inv.TransactionHeader
        WHERE SourceDocumentID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 4) حذف Reservation
        '====================================================
        Using cmd As New SqlCommand("
        DELETE IR
        FROM inv.Reservation IR
        INNER JOIN log.LoadingOrderDetail LOD
            ON IR.SourceID = LOD.LoadingOrderDetailID
        WHERE LOD.LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 5) إعادة حالة SRD
        '====================================================
        Using cmd As New SqlCommand("
        UPDATE SRD
        SET BusinessStatusID = 4
        FROM inv.SRD SRD
        INNER JOIN log.LoadingOrderDetail LOD
            ON LOD.SourceDetailID = SRD.SRDID
        WHERE LOD.LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 6) حذف تفاصيل LO
        '====================================================
        Using cmd As New SqlCommand("
        DELETE FROM log.LoadingOrderDetail
        WHERE LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 7) حذف ربط SR
        '====================================================
        Using cmd As New SqlCommand("
        DELETE FROM log.LoadingOrderSR
        WHERE LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


        '====================================================
        ' 8) حذف الهيدر
        '====================================================
        Using cmd As New SqlCommand("
        DELETE FROM log.LoadingOrder
        WHERE LOID = @LOID
    ", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using


    End Sub
    Private Function GetLoadingStatusID(loID As Integer) As Integer
        Using con As New SqlConnection(_connStr)
            con.Open()
            Using cmd As New SqlCommand("
SELECT LoadingStatusID
FROM log.LoadingOrder
WHERE LOID = @LOID
", con)
                cmd.Parameters.AddWithValue("@LOID", loID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0
                Return CInt(obj)
            End Using
        End Using
    End Function


    Public Sub SaveLoadingOrder(
    loID As Integer,
    userID As Integer,
    dgvLOs As DataGridView,
    dgvLoadingSRD As DataGridView,
    ByRef isSaved As Boolean,
    ByRef isDirty As Boolean,
    ByRef isLoading As Boolean,
    ByRef isSavingGrid As Boolean,
    ByRef currentSelectedStoreID As Integer,
    ByVal isPostedEditMode As Boolean,
    ByVal originalOutputTable As DataTable   ' 🔥 هذا الجديد
)


        If loID <= 0 Then
            MessageBox.Show("لم يتم اختيار أمر تحميل", "تنبيه")
            Exit Sub
        End If

        ' الحالات المسموح بها حسب جدولك الجديد (LOD Scope):
        ' 0,1,2  : يسمح حفظ كامل
        ' 14     : يسمح حفظ كامل
        ' 15     : يسمح تعديل الهيدر فقط
        ' غير ذلك: لا يسمح

        Dim statusID As Integer = GetLoadingStatusID(loID)
        ' =========================================
        ' 🔥 Edit Posted Mode (الحالة 15)
        ' =========================================
        If statusID = 15 AndAlso isPostedEditMode Then

            ' 🔥 1) بناء New Table من الجريد
            Dim newTable As DataTable = BuildNewLoadingTable(dgvLoadingSRD)

            ' 🔥 2) Validation
            ValidatePostedLoadingEdit(
        loID,
       oldTable:=originalOutputTable,
        newTable:=newTable
    )

            ' 🔥 3) إدخال Queue + إعادة بناء
            HandlePostedLoadingEdit(
        loID,
        oldTable:=originalOutputTable,
        newTable:=newTable
    )

            MessageBox.Show("تم حفظ التعديل على السند المرحل")

            isSaved = True
            isDirty = False
            Return

        End If
        ' ✅ Header-only في WAITING_INVOICE (15)
        If statusID = 15 AndAlso Not isPostedEditMode Then
            Using con As New SqlConnection(_connStr)
                con.Open()
                Using cmdHdr As New SqlCommand("
UPDATE log.LoadingOrder
SET
    DriverEmployeeID    = @DriverEmployeeID,
    LoadingSupervisorID = @LoadingSupervisorID,
    VehicleID           = @VehicleID,
    Notes               = @Notes,
    ModifiedAt          = SYSDATETIME(),
    ModifiedBy          = @UserID
WHERE LOID = @LOID
", con)

                    Dim row = dgvLOs.Rows(0)

                    cmdHdr.Parameters.AddWithValue("@LOID", loID)
                    cmdHdr.Parameters.AddWithValue("@UserID", userID)
                    cmdHdr.Parameters.AddWithValue("@DriverEmployeeID", If(row.Cells("colLOsDriverCode").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@LoadingSupervisorID", If(row.Cells("colLOsSupervisor").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@VehicleID", If(row.Cells("colLOsVehicale").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@Notes", If(row.Cells("colLOsNote").Value, DBNull.Value))

                    cmdHdr.ExecuteNonQuery()
                End Using

                Using cmdDet As New SqlCommand("
UPDATE log.LoadingOrderDetail
SET LoadedQty = @LoadedQty
WHERE LoadingOrderDetailID = @LODID
", con)

                    For Each row As DataGridViewRow In dgvLoadingSRD.Rows

                        Dim lodID As Integer = CInt(row.Cells("colLoadingOrderDetailID").Value)

                        Dim loadedSaved As Decimal = CDec(If(row.Cells("colLoadingSRDLoadedQTY").Value, 0D))

                        Dim loadedSession As Decimal = 0D

                        If Not isPostedEditMode Then
                            loadedSession = CDec(If(row.Cells("colLoadingSRDLoadedInThisLO").Value, 0D))
                        End If

                        Dim loadedQty As Decimal = loadedSaved + loadedSession

                        cmdDet.Parameters.Clear()
                        cmdDet.Parameters.Add("@LODID", SqlDbType.Int).Value = lodID

                        Dim p = cmdDet.Parameters.Add("@LoadedQty", SqlDbType.Decimal)
                        p.Precision = 18
                        p.Scale = 3
                        p.Value = loadedQty

                        cmdDet.ExecuteNonQuery()

                        row.Cells("colLoadingSRDLoadedQTY").Value = loadedQty
                        row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D
                    Next
                End Using

            End Using

            MessageBox.Show("تم حفظ بيانات الهيدر فقط (الحالة: WAITING_INVOICE).", "تم")
            isSaved = True
            isDirty = False
            Return
        End If

        ' ✅ السماح بالحفظ الكامل فقط في: 0,1,2,14
        Dim allowFullSave As Boolean = (statusID = 5 OrElse statusID = 1 OrElse statusID = 2 OrElse statusID = 14)

        If Not allowFullSave Then
            MessageBox.Show("لا يمكن الحفظ في هذه الحالة.", "تنبيه")
            Exit Sub
        End If


        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    '=========================================================
                    ' (1) حفظ بيانات الهيدر
                    '=========================================================
                    Using cmdHdr As New SqlCommand("
UPDATE log.LoadingOrder
SET
    DriverEmployeeID    = @DriverEmployeeID,
    LoadingSupervisorID = @LoadingSupervisorID,
    VehicleID           = @VehicleID,
    SourceStoreID       = @SourceStoreID,
    Notes               = @Notes,
    ModifiedAt          = SYSDATETIME(),
    ModifiedBy          = @UserID
WHERE LOID = @LOID
", con, tran)

                        Dim row = dgvLOs.Rows(0)

                        cmdHdr.Parameters.AddWithValue("@LOID", loID)
                        cmdHdr.Parameters.AddWithValue("@UserID", userID)

                        cmdHdr.Parameters.AddWithValue("@DriverEmployeeID", If(row.Cells("colLOsDriverCode").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@LoadingSupervisorID", If(row.Cells("colLOsSupervisor").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@VehicleID", If(row.Cells("colLOsVehicale").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@SourceStoreID", If(row.Cells("colLOsStoreID").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@Notes", If(row.Cells("colLOsNote").Value, DBNull.Value))

                        cmdHdr.ExecuteNonQuery()
                    End Using


                    If dgvLoadingSRD.IsCurrentCellDirty Then
                        dgvLoadingSRD.CommitEdit(DataGridViewDataErrorContexts.Commit)
                    End If
                    dgvLoadingSRD.EndEdit()

                    '=========================================================
                    ' (2-A) معالجة حذف الصفوف المحددة
                    '=========================================================
                    Dim rowsToDelete As New List(Of Integer)

                    For Each row As DataGridViewRow In dgvLoadingSRD.Rows

                        If row.IsNewRow Then Continue For
                        If row.Cells("colLoadingOrderDetailID").Value Is Nothing Then Continue For
                        If IsDBNull(row.Cells("colLoadingOrderDetailID").Value) Then Continue For

                        Dim isDeleted As Boolean =
        CBool(If(row.Cells("colLoadingSRDDeleteFromThisLoadingOrder").Value, False))

                        If isDeleted Then
                            rowsToDelete.Add(CInt(row.Cells("colLoadingOrderDetailID").Value))
                        End If

                    Next


                    ' ❗ منع حذف كل الصفوف
                    If rowsToDelete.Count > 0 AndAlso rowsToDelete.Count = dgvLoadingSRD.Rows.Cast(Of DataGridViewRow)().
    Count(Function(r) Not r.IsNewRow AndAlso r.Cells("colLoadingOrderDetailID").Value IsNot Nothing) Then

                        Throw New Exception("لا يمكن حذف جميع السطور، استخدم زر إلغاء أمر التحميل")

                    End If


                    '=========================================================
                    ' (2-B) تنفيذ الحذف حسب الحالة
                    '=========================================================
                    For Each lodID In rowsToDelete

                        '-----------------------------
                        ' 1) حذف TransactionDetails (فقط في الحالة 5)
                        '-----------------------------
                        If statusID = 5 Then

                            Using cmd As New SqlCommand("
DELETE TD
FROM inv.TransactionDetails TD
INNER JOIN inv.TransactionHeader TH
    ON TH.TransactionID = TD.TransactionID
WHERE TH.SourceDocumentID = @LOID
  AND TD.SourceDocumentDetailID = @LODID
", con, tran)

                                cmd.Parameters.AddWithValue("@LOID", loID)
                                cmd.Parameters.AddWithValue("@LODID", lodID)
                                cmd.ExecuteNonQuery()

                            End Using

                        End If

                        '-----------------------------
                        ' 2) حذف الحجز
                        '-----------------------------
                        Using cmd As New SqlCommand("
DELETE FROM inv.Reservation
WHERE SourceID = @LODID
", con, tran)

                            cmd.Parameters.AddWithValue("@LODID", lodID)
                            cmd.ExecuteNonQuery()

                        End Using


                        '-----------------------------
                        ' 3) إعادة SRD إلى 4
                        '-----------------------------
                        Using cmd As New SqlCommand("
UPDATE SRD
SET BusinessStatusID = 4
FROM inv.SRD SRD
INNER JOIN log.LoadingOrderDetail LOD
    ON LOD.SourceDetailID = SRD.SRDID
WHERE LOD.LoadingOrderDetailID = @LODID
", con, tran)

                            cmd.Parameters.AddWithValue("@LODID", lodID)
                            cmd.ExecuteNonQuery()

                        End Using


                        '-----------------------------
                        ' 4) حذف LOD
                        '-----------------------------
                        Using cmd As New SqlCommand("
DELETE FROM log.LoadingOrderDetail
WHERE LoadingOrderDetailID = @LODID
", con, tran)

                            cmd.Parameters.AddWithValue("@LODID", lodID)
                            cmd.ExecuteNonQuery()

                        End Using

                    Next
                    '=========================================================
                    ' (2-C) حذف الربط بين SR و LO إذا لم يعد هناك تفاصيل
                    '=========================================================
                    Using cmd As New SqlCommand("
DELETE LOS
FROM log.LoadingOrderSR LOS
WHERE LOS.LOID = @LOID
  AND NOT EXISTS (
        SELECT 1
        FROM log.LoadingOrderDetail LOD
        WHERE LOD.LOID = LOS.LOID
          AND LOD.SourceHeaderID = LOS.SRID
  )
", con, tran)

                        cmd.Parameters.AddWithValue("@LOID", loID)
                        cmd.ExecuteNonQuery()

                    End Using

                    '=========================================================
                    ' (2) حفظ LoadedQty في تفاصيل التحميل
                    '=========================================================
                    isSavingGrid = True
                    Try
                        Using cmdDet As New SqlCommand("
UPDATE log.LoadingOrderDetail
SET LoadedQty = @LoadedQty
WHERE LoadingOrderDetailID = @LODID
", con, tran)

                            For Each row As DataGridViewRow In dgvLoadingSRD.Rows
                                Dim isDeleted As Boolean =
    CBool(If(row.Cells("colLoadingSRDDeleteFromThisLoadingOrder").Value, False))

                                If isDeleted Then Continue For

                                If row.IsNewRow Then Continue For
                                If row.Cells("colLoadingOrderDetailID").Value Is Nothing Then Continue For
                                If IsDBNull(row.Cells("colLoadingOrderDetailID").Value) Then Continue For

                                Dim lodID As Integer = CInt(row.Cells("colLoadingOrderDetailID").Value)

                                Dim loadedSaved As Decimal = CDec(If(row.Cells("colLoadingSRDLoadedQTY").Value, 0D))

                                Dim loadedSession As Decimal = 0D

                                If Not isPostedEditMode Then
                                    loadedSession = CDec(If(row.Cells("colLoadingSRDLoadedInThisLO").Value, 0D))
                                End If

                                Dim loadedQty As Decimal = loadedSaved + loadedSession
                                cmdDet.Parameters.Clear()
                                cmdDet.Parameters.Add("@LODID", SqlDbType.Int).Value = lodID

                                Dim p = cmdDet.Parameters.Add("@LoadedQty", SqlDbType.Decimal)
                                p.Precision = 18
                                p.Scale = 3
                                p.Value = loadedQty

                                cmdDet.ExecuteNonQuery()

                                row.Cells("colLoadingSRDLoadedQTY").Value = loadedQty
                                row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D
                            Next
                        End Using
                    Finally
                        isSavingGrid = False
                    End Try

                    '=========================================================
                    ' (3) منطق الحفظ المركزي (بدون SP) - نسخة من log.SaveLoadingOrder
                    '=========================================================

                    ' (3.1) قراءة OperationTypeID + SourceStoreID (مهم للحجز)
                    Dim operationTypeID As Integer
                    Dim sourceStoreIDObj As Object

                    Using cmdOp As New SqlCommand("
SELECT OperationTypeID, SourceStoreID
FROM log.LoadingOrder
WHERE LOID = @LOID
", con, tran)
                        cmdOp.Parameters.AddWithValue("@LOID", loID)

                        Using rd = cmdOp.ExecuteReader()
                            If Not rd.Read() Then Throw New Exception("LO غير موجود")
                            If IsDBNull(rd("OperationTypeID")) Then Throw New Exception("OperationTypeID غير موجود لأمر التحميل")
                            operationTypeID = CInt(rd("OperationTypeID"))
                            sourceStoreIDObj = rd("SourceStoreID")
                        End Using
                    End Using

                    If sourceStoreIDObj Is Nothing OrElse IsDBNull(sourceStoreIDObj) Then
                        Throw New Exception("SourceStoreID غير محدد في أمر التحميل - لا يمكن إنشاء حجز")
                    End If


                    ' (3.2) تحديث Volume_m3
                    Using cmdVol As New SqlCommand("
UPDATE LOD
SET Volume_m3 =
    (ISNULL(Length_cm,0) * ISNULL(Width_cm,0) * ISNULL(Height_cm,0)* ISNULL(LoadedQty,0)) / 1000000.0
FROM log.LoadingOrderDetail LOD
WHERE LOD.LOID = @LOID
", con, tran)
                        cmdVol.Parameters.AddWithValue("@LOID", loID)
                        cmdVol.ExecuteNonQuery()
                    End Using


                    ' (3.3) تحديث الحجز الموجود (LoadedQty > 0)
                    Using cmdUpdRes As New SqlCommand("
UPDATE IR
SET
    IR.ReservedQty = LOD.LoadedQty,
    IR.ReservedAt  = SYSDATETIME(),
    IR.ReleasedAt  = NULL,
    IR.CreatedBy   = @UserID
FROM inv.Reservation IR
INNER JOIN log.LoadingOrderDetail LOD
   ON IR.SourceID = LOD.LoadingOrderDetailID
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
", con, tran)
                        cmdUpdRes.Parameters.AddWithValue("@UserID", userID)
                        cmdUpdRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                        cmdUpdRes.Parameters.AddWithValue("@LOID", loID)
                        cmdUpdRes.ExecuteNonQuery()
                    End Using


                    ' (3.4) حذف الحجز إذا LoadedQty = 0
                    Using cmdDelRes As New SqlCommand("
DELETE IR
FROM inv.Reservation IR
INNER JOIN log.LoadingOrderDetail LOD
   ON IR.SourceID = LOD.LoadingOrderDetailID
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty = 0
", con, tran)
                        cmdDelRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                        cmdDelRes.Parameters.AddWithValue("@LOID", loID)
                        cmdDelRes.ExecuteNonQuery()
                    End Using


                    ' (3.5) إنشاء حجز جديد (للأسطر بدون حجز)
                    Using cmdInsRes As New SqlCommand("
INSERT INTO inv.Reservation
(
    ProductID,
    SourceStoreID,
    ReservedQty,
    SourceOperationTypeID,
    SourceID,
    CostAtReserve,
    ReservedAt,
    CreatedBy,
    ReservationStatusID,
    SourceDetailID
)
SELECT
    LOD.ProductID,
    LO.SourceStoreID,
    LOD.LoadedQty,
    @OperationTypeID,
    LOD.LoadingOrderDetailID,
    0,
    SYSDATETIME(),
    @UserID,
    1,
    LOD.SourceDetailID
FROM log.LoadingOrderDetail LOD
INNER JOIN log.LoadingOrder LO
    ON LO.LOID = LOD.LOID
LEFT JOIN inv.Reservation IR
  ON IR.SourceID = LOD.LoadingOrderDetailID 
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
  AND LOD.SourceDetailID IS NOT NULL
  AND IR.ReservationID IS NULL
", con, tran)
                        cmdInsRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                        cmdInsRes.Parameters.AddWithValue("@UserID", userID)
                        cmdInsRes.Parameters.AddWithValue("@LOID", loID)
                        cmdInsRes.ExecuteNonQuery()
                    End Using


                    ' (3.6) تحديث حالة SRD حسب إجمالي التحميل
                    Using cmdUpdSRD As New SqlCommand("
;WITH TotalLoaded AS
(
    SELECT
        SourceDetailID,
        SUM(LoadedQty) AS TotalLoadedQty
    FROM log.LoadingOrderDetail
    GROUP BY SourceDetailID
)
UPDATE SRD
SET SRD.BusinessStatusID =
    CASE
        WHEN ISNULL(T.TotalLoadedQty, 0) >= SRD.Quantity THEN 13
        WHEN ISNULL(T.TotalLoadedQty, 0) > 0        THEN 12
        ELSE SRD.BusinessStatusID
    END
FROM inv.SRD SRD
LEFT JOIN TotalLoaded T
    ON T.SourceDetailID = SRD.SRDID
WHERE EXISTS
(
    SELECT 1
    FROM log.LoadingOrderDetail X
    WHERE X.SourceDetailID = SRD.SRDID
      AND X.LOID = @LOID
)
", con, tran)
                        cmdUpdSRD.Parameters.AddWithValue("@LOID", loID)
                        cmdUpdSRD.ExecuteNonQuery()
                    End Using


                    ' (3.7) تحديث حالة أمر التحميل
                    ' (3.7) تحديث حالة أمر التحميل - فقط في الحالات المسموحة
                    Using cmdUpdLO As New SqlCommand("
UPDATE log.LoadingOrder
SET
    LoadingStatusID = 14,
    ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)
                        cmdUpdLO.Parameters.AddWithValue("@LOID", loID)
                        cmdUpdLO.Parameters.AddWithValue("@UserID", userID)
                        cmdUpdLO.ExecuteNonQuery()
                    End Using
                    tran.Commit()
                    isSaved = True
                    isDirty = False

                Catch ex As Exception
                    Try : tran.Rollback() : Catch : End Try
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            End Using
        End Using


        ' =========================
        ' 6) إعادة تحميل البورد والتركيز
        ' =========================

        isSaved = True
        MessageBox.Show("تم حفظ أمر التحميل بنجاح")
        isDirty = False


    End Sub
    Public Function IsLOInCorrectionQueue(loID As Integer) As Boolean

        Using con As New SqlConnection(_connStr)
            Using cmd As New SqlCommand("
SELECT TOP 1 1
FROM inv.CorrectionQueue q
INNER JOIN inv.TransactionDetails d
    ON d.DetailID = q.TransactionDetailID
INNER JOIN inv.TransactionHeader h
    ON h.TransactionID = d.TransactionID
WHERE h.SourceDocumentID = @LOID
  AND h.OperationTypeID = 4
  AND q.StatusID IN (22, 23)
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                con.Open()
                Dim obj = cmd.ExecuteScalar()
                Return obj IsNot Nothing
            End Using
        End Using

    End Function
    Public Sub ValidatePostedLoadingEdit(
    loID As Integer,
    oldTable As DataTable,
    newTable As DataTable
)
        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim storeID As Integer

                    Using cmd As New SqlCommand("
SELECT SourceStoreID
FROM log.LoadingOrder
WHERE LOID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", loID)
                        Dim obj = cmd.ExecuteScalar()
                        If obj Is Nothing OrElse IsDBNull(obj) Then
                            Throw New Exception("لم يتم العثور على مخزن السند")
                        End If

                        storeID = CInt(obj)

                    End Using

                    For Each newRow As DataRow In newTable.Rows

                        Dim docID As Integer = CInt(newRow("DocumentDetailID"))
                        Dim productID As Integer = CInt(newRow("ProductID"))

                        ' =========================
                        ' 🔥 الكميات الجديدة
                        ' =========================
                        Dim newQtyUnit As Decimal = CDec(newRow("QtyUnit")) ' حبة
                        Dim newQtyM3 As Decimal = CDec(newRow("QtyM3"))     ' m3

                        ' =========================
                        ' 🔥 الكميات القديمة
                        ' =========================
                        Dim oldRow = oldTable.Select("DocumentDetailID = " & docID).FirstOrDefault()

                        Dim oldQtyUnit As Decimal = 0D
                        Dim oldQtyM3 As Decimal = 0D

                        If oldRow IsNot Nothing Then
                            oldQtyM3 = CDec(oldRow("Qty")) ' القديم كان بالحبة

                        End If

                        ' =========================
                        ' 🔥 الفرق (بالمتر المكعب)
                        ' =========================
                        Dim extraQtyM3 As Decimal = newQtyM3 - oldQtyM3


                        If extraQtyM3 <= 0 Then Continue For

                        ' =========================
                        ' 🔥 OnHand (m3)
                        ' =========================
                        Dim OnhandQty As Decimal = 0D

                        Using cmd As New SqlCommand("
SELECT TOP 1 CL.NewQty
FROM inv.CostLedger CL
WHERE CL.ProductID = @ProductID
  AND CL.StoreID = @StoreID
  AND CL.IsActive = 1
ORDER BY CL.LedgerID DESC
", con, tran)

                            cmd.Parameters.AddWithValue("@ProductID", productID)
                            cmd.Parameters.AddWithValue("@StoreID", storeID)

                            Dim obj = cmd.ExecuteScalar()
                            OnhandQty = If(obj Is Nothing OrElse IsDBNull(obj), 0D, CDec(obj))
                        End Using

                        ' =========================
                        ' 🔥 Reserved (حبة → m3)
                        ' =========================
                        Dim ReservedQtyUI As Decimal = 0D

                        Using cmd As New SqlCommand("
SELECT
    ISNULL(SUM(IR.ReservedQty),0)
FROM inv.Reservation IR
WHERE IR.ProductID=@ProductID
   AND IR.SourceStoreID = @StoreID
   AND IR.ReservationStatusID = 1
", con, tran)

                            cmd.Parameters.AddWithValue("@ProductID", productID)
                            cmd.Parameters.AddWithValue("@StoreID", storeID)

                            Dim obj = cmd.ExecuteScalar()
                            ReservedQtyUI = If(obj Is Nothing OrElse IsDBNull(obj), 0D, CDec(obj))
                        End Using

                        ' 🔥 التحويل إلى متر مكعب
                        Dim ReservedQty As Decimal =
                        ConvertLoadingQtyToQueueQty(productID, ReservedQtyUI, con, tran)

                        ' =========================
                        ' 🔥 Available (m3)
                        ' =========================
                        Dim AvailableQty As Decimal = OnhandQty - ReservedQty

                        ' =========================
                        ' 🔥 المقارنة النهائية
                        ' =========================
                        If extraQtyM3 > AvailableQty Then
                            Throw New Exception("الكمية الزائدة غير متوفرة في المخزون للصنف رقم " & productID)
                        End If

                    Next

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub
    Public Sub HandlePostedLoadingEdit(
    loID As Integer,
    oldTable As DataTable,
    newTable As DataTable
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    BuildLoadingCorrectionQueue(
                    loID,
                    oldTable,
                    newTable,
                    con,
                    tran
                )

                    UpdateLoadingOrderDetailsFromNewTable(
                    loID,
                    newTable,
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
    Private Sub BuildLoadingCorrectionQueue(
    loID As Integer,
    oldTable As DataTable,
    newTable As DataTable,
    con As SqlConnection,
    tran As SqlTransaction
)

        For Each newRow As DataRow In newTable.Rows

            Dim docID As Integer = CInt(newRow("DocumentDetailID"))
            Dim newQtyUnit As Decimal = CDec(newRow("QtyUnit"))   ' للحبة عند الحاجة
            Dim newQtyM3 As Decimal = CDec(newRow("QtyM3"))       ' للـ Queue
            Dim productID As Integer = CInt(newRow("ProductID"))

            Dim oldRow = oldTable.Select("DocumentDetailID = " & docID).FirstOrDefault()

            Dim oldQtyM3 As Decimal = 0D
            Dim detailID As Object = DBNull.Value
            Dim startLedgerID As Object = DBNull.Value

            If oldRow IsNot Nothing Then

                If oldRow.Table.Columns.Contains("TransactionDetailID") AndAlso
               Not IsDBNull(oldRow("TransactionDetailID")) Then
                    detailID = oldRow("TransactionDetailID")
                End If

                If oldRow.Table.Columns.Contains("LedgerID") AndAlso
               Not IsDBNull(oldRow("LedgerID")) Then
                    startLedgerID = oldRow("LedgerID")
                End If

                ' القديم هنا نعتبره m3 جاهز من الجدول الأصلي المرحل
                If oldRow.Table.Columns.Contains("Qty") AndAlso
               Not IsDBNull(oldRow("Qty")) Then
                    oldQtyM3 = CDec(oldRow("Qty"))
                End If

            End If

            If oldQtyM3 = newQtyM3 Then Continue For

            Using cmd As New SqlCommand("
INSERT INTO inv.CorrectionQueue
(
    TransactionDetailID,
    DocumentDetailID,
    StartLedgerID,
    StatusID,
    ScopeCode,
    ChangeType,
    ProductID,
    NewQuantity,
    NewUnitCost,
    CostGroupID
)
VALUES
(
    @TDID,
    @DocID,
    @StartLedgerID,
    22,
    'LOD',
    'EDIT',
    @ProductID,
    @Qty,
    NULL,
    @CostGroupID
)
", con, tran)

                cmd.Parameters.AddWithValue("@TDID", detailID)
                cmd.Parameters.AddWithValue("@DocID", docID)
                cmd.Parameters.AddWithValue("@StartLedgerID", startLedgerID)
                cmd.Parameters.AddWithValue("@ProductID", productID)

                Dim pQty = cmd.Parameters.Add("@Qty", SqlDbType.Decimal)
                pQty.Precision = 18
                pQty.Scale = 3
                pQty.Value = newQtyM3

                cmd.Parameters.AddWithValue("@CostGroupID", Guid.NewGuid())

                cmd.ExecuteNonQuery()

            End Using
        Next

    End Sub

    Private Sub UpdateLoadingReservation(
    loID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim operationTypeID As Integer

        Using cmdGetOp As New SqlCommand("
SELECT OperationTypeID
FROM log.LoadingOrder
WHERE LOID = @LOID
", con, tran)

            cmdGetOp.Parameters.AddWithValue("@LOID", loID)

            Dim obj = cmdGetOp.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("OperationTypeID غير موجود لأمر التحميل")
            End If

            operationTypeID = CInt(obj)
        End Using

        Using cmd As New SqlCommand("
UPDATE IR
SET IR.ReservedQty = LOD.LoadedQty
FROM inv.Reservation IR
INNER JOIN log.LoadingOrderDetail LOD
    ON IR.SourceID = LOD.LoadingOrderDetailID
   AND IR.ProductID = LOD.ProductID
   AND IR.SourceOperationTypeID = @OperationTypeID
WHERE LOD.LOID = @LOID
", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Private Function BuildNewLoadingTable(dgv As DataGridView) As DataTable

        Dim dt As New DataTable

        dt.Columns.Add("DocumentDetailID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("QtyUnit", GetType(Decimal)) ' الحبة
        dt.Columns.Add("QtyM3", GetType(Decimal))   ' المتر المكعب

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                For Each row As DataGridViewRow In dgv.Rows

                    If row.IsNewRow Then Continue For

                    Dim isDeleted As Boolean =
                    CBool(If(row.Cells("colLoadingSRDDeleteFromThisLoadingOrder").Value, False))

                    If isDeleted Then Continue For

                    Dim lodID As Integer =
                    CInt(row.Cells("colLoadingOrderDetailID").Value)

                    Dim productID As Integer =
                    CInt(row.Cells("colLoadingSRDProductID").Value)

                    Dim loadedSaved As Decimal =
                    CDec(If(row.Cells("colLoadingSRDLoadedQTY").Value, 0D))

                    Dim loadedSession As Decimal =
                    CDec(If(row.Cells("colLoadingSRDLoadedInThisLO").Value, 0D))

                    Dim finalUIQty As Decimal = loadedSaved + loadedSession

                    Dim finalM3Qty As Decimal =
                    ConvertLoadingQtyToQueueQty(productID, finalUIQty, con, tran)

                    Dim dr = dt.NewRow()
                    dr("DocumentDetailID") = lodID
                    dr("ProductID") = productID
                    dr("QtyUnit") = finalUIQty
                    dr("QtyM3") = finalM3Qty

                    dt.Rows.Add(dr)

                Next

                tran.Commit()

            End Using
        End Using

        Return dt

    End Function

    Private Sub UpdateLoadingOrderDetailsFromNewTable(
    loID As Integer,
    newTable As DataTable,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
UPDATE LOD
SET LOD.LoadedQty = @Qty,
LOD.Volume_m3 =
    (ISNULL(Length_cm,0) * ISNULL(Width_cm,0) * ISNULL(Height_cm,0)* @Qty) / 1000000.0
FROM log.LoadingOrderDetail LOD
WHERE LOD.LOID = @LOID
  AND LOD.LoadingOrderDetailID = @DocID
", con, tran)

            For Each r As DataRow In newTable.Rows
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@LOID", loID)
                cmd.Parameters.AddWithValue("@DocID", CInt(r("DocumentDetailID")))

                Dim p = cmd.Parameters.Add("@Qty", SqlDbType.Decimal)
                p.Precision = 18
                p.Scale = 3
                p.Value = CDec(r("QtyUnit")) ' بالحبة

                cmd.ExecuteNonQuery()
            Next
        End Using

        Using cmd As New SqlCommand("
;WITH TotalLoaded AS
(
    SELECT
        SourceDetailID,
        SUM(LoadedQty) AS TotalLoadedQty
    FROM log.LoadingOrderDetail
    GROUP BY SourceDetailID
)
UPDATE SRD
SET SRD.BusinessStatusID =
    CASE
        WHEN ISNULL(T.TotalLoadedQty, 0) >= SRD.Quantity THEN 13
        WHEN ISNULL(T.TotalLoadedQty, 0) > 0 THEN 12
        ELSE 4
    END
FROM inv.SRD SRD
LEFT JOIN TotalLoaded T
    ON T.SourceDetailID = SRD.SRDID
WHERE EXISTS
(
    SELECT 1
    FROM log.LoadingOrderDetail X
    WHERE X.SourceDetailID = SRD.SRDID
      AND X.LOID = @LOID
)
", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.ExecuteNonQuery()
        End Using

        UpdateLoadingReservation(loID, con, tran)

    End Sub
    Private Function ConvertLoadingQtyToQueueQty(
    productID As Integer,
    loadedQty As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Return _inventoryRepo.ConvertQtyToLedgerUnit_ForLoading(productID, loadedQty, con, tran)
    End Function

End Class