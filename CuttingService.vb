Imports System.Data.SqlClient

Public Class CuttingService

    Private ReadOnly ConnStr As String

    Public Sub New(connectionString As String)
        ConnStr = connectionString
    End Sub

    Public Function SaveCuttingWITHMIX(
                                                                                ByRef CuttingID As Integer,
                                                                                CuttingCode As String,
                                                                                CutDate As Date,
                                                                                BaseProductID As Integer,
                                                                                ConsumedVolume_m3 As Decimal,
                                                                                SourceStoreID As Integer,
                                                                                Notes As String,
                                                                                UserID As Integer,
                                                                                Outputs As DataTable
                                                                            ) As Boolean

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim OLDQTY As Decimal = 0
                    Dim NEWQTY As Decimal = 0
                    Dim AvgCostPerM3 As Decimal = 0

                    ' =========================
                    ' (1) Header
                    ' =========================
                    If CuttingID = 0 Then

                        Dim cmdInsert As New SqlCommand("
                                                                        INSERT INTO prod.CuttingHeader
                                                                        (CuttingCode,CutDate,Notes,StatusID,CreatedBy,CreatedAt,IsInventoryPosted,
                                                                         BaseProductID,ConsumedVolume_m3,SourceStoreID)
                                                                        VALUES
                                                                        (@CuttingCode,@CutDate,@Notes,2,@UserID,GETDATE(),0,
                                                                         @BaseProductID,@ConsumedVolume_m3,@SourceStoreID);
                                                                        SELECT SCOPE_IDENTITY();
                                                                        ", con, tran)

                        cmdInsert.Parameters.AddWithValue("@CuttingCode", CuttingCode)
                        cmdInsert.Parameters.AddWithValue("@CutDate", CutDate)
                        cmdInsert.Parameters.AddWithValue("@Notes", Notes)
                        cmdInsert.Parameters.AddWithValue("@UserID", UserID)
                        cmdInsert.Parameters.AddWithValue("@BaseProductID", BaseProductID)
                        cmdInsert.Parameters.AddWithValue("@ConsumedVolume_m3", ConsumedVolume_m3)
                        cmdInsert.Parameters.AddWithValue("@SourceStoreID", SourceStoreID)

                        CuttingID = Convert.ToInt32(cmdInsert.ExecuteScalar())

                    Else

                        Dim cmdUpdate As New SqlCommand("
                                                                        UPDATE prod.CuttingHeader
                                                                        SET CutDate=@CutDate,
                                                                            Notes=@Notes,
                                                                            BaseProductID=@BaseProductID,
                                                                            ConsumedVolume_m3=@ConsumedVolume_m3,
                                                                            SourceStoreID=@SourceStoreID
                                                                        WHERE CuttingID=@CuttingID
                                                                        ", con, tran)

                        cmdUpdate.Parameters.AddWithValue("@CutDate", CutDate)
                        cmdUpdate.Parameters.AddWithValue("@Notes", Notes)
                        cmdUpdate.Parameters.AddWithValue("@BaseProductID", BaseProductID)
                        cmdUpdate.Parameters.AddWithValue("@ConsumedVolume_m3", ConsumedVolume_m3)
                        cmdUpdate.Parameters.AddWithValue("@SourceStoreID", SourceStoreID)
                        cmdUpdate.Parameters.AddWithValue("@CuttingID", CuttingID)

                        cmdUpdate.ExecuteNonQuery()

                    End If

                    ' =========================
                    ' حذف القديم
                    ' =========================
                    Dim cmdDel As New SqlCommand("
                                                                        DELETE FROM prod.CuttingOutput WHERE CutID=@ID
                                                                        ", con, tran)
                    cmdDel.Parameters.AddWithValue("@ID", CuttingID)
                    cmdDel.ExecuteNonQuery()

                    ' =========================
                    ' إدخال النواتج
                    ' =========================
                    For Each r As DataRow In Outputs.Rows

                        Dim cmd As New SqlCommand("
                                                                        INSERT INTO prod.CuttingOutput
                                                                        (CutID,ProductID,QtyPieces,IsMix,Notes,
                                                                         Length_cm,Width_cm,Height_cm,
                                                                         PieceVolume_m3,TotalVolume_m3,
                                                                         ProductTypeID,OutProductCode,
                                                                         UnitCost,TotalCost,
                                                                         SourceStoreID,TargetStoreID,IsInventoryPosted)
                                                                        VALUES
                                                                        (@CutID,@ProductID,@QtyPieces,@IsMix,@Notes,
                                                                         @L,@W,@H,@PV,@TV,
                                                                         @TypeID,@Code,
                                                                         0,0,
                                                                         @SourceStoreID,
                                                                         CASE WHEN @IsMix=1 THEN @SourceStoreID ELSE @TargetStoreID END,
                                                                         0)
                                                                        ", con, tran)

                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@ProductID", r("ProductID"))
                        cmd.Parameters.AddWithValue("@QtyPieces", r("QtyPieces"))
                        cmd.Parameters.AddWithValue("@IsMix", r("IsMix"))
                        cmd.Parameters.AddWithValue("@Notes", If(r("Notes"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@L", If(r("Length_cm"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@W", If(r("Width_cm"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@H", If(r("Height_cm"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@PV", If(r("PieceVolume_m3"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@TV", If(r("TotalVolume_m3"), DBNull.Value))
                        cmd.Parameters.AddWithValue("@TypeID", r("ProductTypeID"))
                        cmd.Parameters.AddWithValue("@Code", r("OutProductCode"))
                        cmd.Parameters.AddWithValue("@SourceStoreID", SourceStoreID)
                        cmd.Parameters.AddWithValue("@TargetStoreID", r("TargetStoreID"))

                        cmd.ExecuteNonQuery()
                    Next

                    ' =========================
                    ' AvgCost
                    ' =========================
                    Dim cmdAvg As New SqlCommand("
                                                                        SELECT AvgCostPerM3 FROM inv.FinalProductAvgCost WHERE BaseProductID=@ID
                                                                        ", con, tran)

                    cmdAvg.Parameters.AddWithValue("@ID", BaseProductID)
                    Dim obj = cmdAvg.ExecuteScalar()

                    If obj Is Nothing Then
                        Throw New Exception("Missing AvgCostPerM3")
                    End If

                    AvgCostPerM3 = Convert.ToDecimal(obj)

                    ' =========================
                    ' حساب الكميات
                    ' =========================
                    Dim cmdQty As New SqlCommand("
                                                                        SELECT 
                                                                        SUM(TotalVolume_m3),
                                                                        SUM(CASE WHEN IsMix=0 THEN TotalVolume_m3 ELSE 0 END)
                                                                        FROM prod.CuttingOutput WHERE CutID=@ID
                                                                        ", con, tran)

                    cmdQty.Parameters.AddWithValue("@ID", CuttingID)

                    Using rd = cmdQty.ExecuteReader()
                        If rd.Read() Then
                            OLDQTY = If(IsDBNull(0), 0, rd.GetDecimal(0))
                            NEWQTY = If(IsDBNull(1), 0, rd.GetDecimal(1))
                        End If
                    End Using

                    Dim OutAvgCost As Decimal = If(NEWQTY = 0, 0, (OLDQTY * AvgCostPerM3) / NEWQTY)

                    ' =========================
                    ' تحديث التكلفة
                    ' =========================
                    Dim cmdCost As New SqlCommand("
                                                                        UPDATE prod.CuttingOutput
                                                                        SET UnitCost = TotalVolume_m3*@Cost/NULLIF(QtyPieces,0),
                                                                            TotalCost = TotalVolume_m3*@Cost
                                                                        WHERE CutID=@ID AND IsMix=0
                                                                        ", con, tran)

                    cmdCost.Parameters.AddWithValue("@Cost", OutAvgCost)
                    cmdCost.Parameters.AddWithValue("@ID", CuttingID)
                    cmdCost.ExecuteNonQuery()

                    Dim cmdMix As New SqlCommand("
                                                                        UPDATE prod.CuttingOutput
                                                                        SET UnitCost=0,TotalCost=0
                                                                        WHERE CutID=@ID AND IsMix=1
                                                                        ", con, tran)

                    cmdMix.Parameters.AddWithValue("@ID", CuttingID)
                    cmdMix.ExecuteNonQuery()

                    ' =========================
                    ' Flags
                    ' =========================
                    Dim cmdFlag As New SqlCommand("
                                                                        UPDATE prod.CuttingHeader
                                                                        SET IsInventoryPosted=0
                                                                        WHERE CuttingID=@ID
                                                                        ", con, tran)

                    cmdFlag.Parameters.AddWithValue("@ID", CuttingID)
                    cmdFlag.ExecuteNonQuery()

                    tran.Commit()
                    Return True

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Public Function GetCuttingStatus(cuttingID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                                                                        SELECT StatusID 
                                                                        FROM prod.CuttingHeader 
                                                                        WHERE CuttingID=@ID
                                                                        ", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                con.Open()
                Dim obj = cmd.ExecuteScalar()

                If obj Is Nothing Then Return 0

                Return Convert.ToInt32(obj)
            End Using
        End Using

    End Function

    Public Sub SendCutting(CuttingID As Integer, UserID As Integer)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim NowDate As DateTime = DateTime.Now
                    Dim TransactionID As Integer = 0
                    Dim BaseProductID As Integer
                    Dim SourceStoreID As Integer
                    Dim ConsumedVolume As Decimal
                    Dim BaseAvgCostPerM3 As Decimal
                    Dim OperationTypeID As Integer
                    Dim PeriodID As Integer

                    ' (1) قراءة بيانات القص
                    Using cmd As New SqlCommand("
SELECT BaseProductID, SourceStoreID, ConsumedVolume_m3
FROM prod.CuttingHeader
WHERE CuttingID=@ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CuttingID)

                        Using rd = cmd.ExecuteReader()
                            If rd.Read() Then
                                BaseProductID = rd.GetInt32(0)
                                SourceStoreID = rd.GetInt32(1)
                                ConsumedVolume = rd.GetDecimal(2)
                            Else
                                Throw New Exception("بيانات القص غير مكتملة")
                            End If
                        End Using
                    End Using

                    ' (2) تكلفة الخام
                    Using cmd As New SqlCommand("
SELECT ISNULL(AvgCostPerM3,0)
FROM inv.FinalProductAvgCost
WHERE BaseProductID=@ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", BaseProductID)
                        BaseAvgCostPerM3 = Convert.ToDecimal(cmd.ExecuteScalar())
                    End Using

                    ' (3) نوع العملية
                    Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM wf.OperationType
WHERE OperationCode='CUT' AND IsActive=1
", con, tran)

                        Dim obj = cmd.ExecuteScalar()
                        If obj Is Nothing Then Throw New Exception("OperationType غير معرف")
                        OperationTypeID = Convert.ToInt32(obj)
                    End Using

                    ' (4) الفترة المالية
                    Using cmd As New SqlCommand("
SELECT PeriodID
FROM cfg.FiscalPeriod
WHERE CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate
AND IsOpen=1
", con, tran)

                        Dim obj = cmd.ExecuteScalar()
                        If obj Is Nothing Then Throw New Exception("لا توجد فترة مفتوحة")
                        PeriodID = Convert.ToInt32(obj)
                    End Using

                    ' (5) إنشاء الهيدر
                    Using cmd As New SqlCommand("
INSERT INTO inv.TransactionHeader
(TransactionDate,SourceDocumentID,OperationTypeID,PeriodID,StatusID,
 IsFinancialPosted,CreatedBy,CreatedAt,SentAt,SentBy,IsInventoryPosted)
VALUES
(@Now,@CuttingID,@Op,@Period,5,0,@User,@Now,@Now,@User,0);
SELECT SCOPE_IDENTITY();
", con, tran)

                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@CuttingID", CuttingID)
                        cmd.Parameters.AddWithValue("@Op", OperationTypeID)
                        cmd.Parameters.AddWithValue("@Period", PeriodID)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        TransactionID = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    ' (6) RAW
                    Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(
TransactionID,ProductID,Quantity,UnitID,UnitCost,CostAmount,
SourceStoreID,TargetStoreID,SourceDocumentDetailID,ReferenceDetailID,
CreatedAt,CreatedBy,CorrectionReferenceDetailID,IsCorrection,CorrectionRunID
)
VALUES
(@TID,@PID,@Q,
 (SELECT StorageUnitID FROM md.Product WHERE ProductID=@PID),
 @Cost,@Amount,
 @Source,NULL,@CutID,NULL,@Now,@User,NULL,0,NULL)
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", TransactionID)
                        cmd.Parameters.AddWithValue("@PID", BaseProductID)
                        cmd.Parameters.AddWithValue("@Q", ConsumedVolume)
                        cmd.Parameters.AddWithValue("@Cost", BaseAvgCostPerM3)
                        cmd.Parameters.AddWithValue("@Amount", ConsumedVolume * BaseAvgCostPerM3)
                        cmd.Parameters.AddWithValue("@Source", SourceStoreID)
                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (7) GOOD
                    Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(
TransactionID,ProductID,Quantity,UnitID,UnitCost,CostAmount,
SourceStoreID,TargetStoreID,SourceDocumentDetailID,ReferenceDetailID,
CreatedAt,CreatedBy,CorrectionReferenceDetailID,IsCorrection,CorrectionRunID
)
SELECT
@TID,
o.ProductID,
o.QtyPieces,
p.StorageUnitID,
o.UnitCost,
o.QtyPieces * o.UnitCost,
NULL,
o.TargetStoreID,
o.CutOutputID,
NULL,
@Now,
@User,
NULL,
0,
NULL
FROM prod.CuttingOutput o
INNER JOIN md.Product p ON p.ProductID=o.ProductID
WHERE o.CutID=@CutID AND o.IsMix=0
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", TransactionID)
                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (8) MIX
                    Using cmd As New SqlCommand("
INSERT INTO inv.TransactionDetails
(
TransactionID,ProductID,Quantity,UnitID,UnitCost,CostAmount,
SourceStoreID,TargetStoreID,SourceDocumentDetailID,ReferenceDetailID,
CreatedAt,CreatedBy,CorrectionReferenceDetailID,IsCorrection,CorrectionRunID
)
SELECT
@TID,
o.ProductID,
o.TotalVolume_m3,
p.StorageUnitID,
o.UnitCost,
o.TotalVolume_m3 * o.UnitCost,
NULL,
o.SourceStoreID,
o.CutOutputID,
NULL,
@Now,
@User,
NULL,
0,
NULL
FROM prod.CuttingOutput o
INNER JOIN md.Product p ON p.ProductID=o.ProductID
WHERE o.CutID=@CutID AND o.IsMix=1
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", TransactionID)
                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (9) تحديث الحالة
                    Using cmd As New SqlCommand("
UPDATE prod.CuttingHeader
SET StatusID=5, IsInventoryPosted=0
WHERE CuttingID=@ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CuttingID)
                        cmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub

    Public Sub ReserveCutting(CuttingID As Integer, UserID As Integer)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    ' 1- حذف الحجز القديم
                    Using cmd As New SqlCommand("
DELETE FROM inv.Reservation
WHERE SourceID=@ID AND SourceOperationTypeID=11
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CuttingID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' 2- إدخال الحجز الجديد (الخام فقط)
                    Using cmd As New SqlCommand("
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
    h.BaseProductID,
    h.SourceStoreID,
    h.ConsumedVolume_m3,
    11,
    h.CuttingID,
    ISNULL(f.AvgCostPerM3,0),
    SYSDATETIME(),
    @UserID,
    1,
    h.CuttingID
FROM prod.CuttingHeader h
LEFT JOIN inv.FinalProductAvgCost f
    ON f.BaseProductID = h.BaseProductID
WHERE h.CuttingID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CuttingID)
                        cmd.Parameters.AddWithValue("@UserID", UserID)
                        cmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

    Public Function GetAvailableQty(ProductID As Integer, StoreID As Integer) As Decimal

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    ISNULL(SUM(InQty - OutQty),0)
    -
    ISNULL((
        SELECT SUM(ReservedQty)
        FROM inv.Reservation
        WHERE ProductID=@P AND SourceStoreID=@S
          AND ReservationStatusID=1
    ),0)
FROM inv.CostLedger
WHERE ProductID=@P AND StoreID=@S
", con)

                cmd.Parameters.AddWithValue("@P", ProductID)
                cmd.Parameters.AddWithValue("@S", StoreID)

                con.Open()
                Return Convert.ToDecimal(cmd.ExecuteScalar())
            End Using
        End Using

    End Function
    Private Sub BuildCuttingCorrectionQueue(
    oldTable As DataTable,
    newTable As DataTable,
    scopeCode As String,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim eps As Decimal = 0.000001D

        For i As Integer = 0 To newTable.Rows.Count - 1

            Dim oldRow = oldTable.Rows(i)
            Dim newRow = newTable.Rows(i)

            Dim oldQty As Decimal = CDec(oldRow("Qty"))
            Dim newQty As Decimal = CDec(newRow("Qty"))

            If Math.Abs(oldQty - newQty) <= eps AndAlso newQty <> 0 Then Continue For

            Dim productID As Integer = CInt(newRow("ProductID"))

            ' 🔥 الربط الصحيح من oldTable
            Dim transactionDetailID As Integer = CInt(oldRow("TransactionDetailID"))
            Dim startLedgerID As Integer = CInt(oldRow("LedgerID"))
            Dim documentDetailID As Integer = CInt(oldRow("DocumentDetailID"))

            Using cmd As New SqlCommand("
INSERT INTO inv.CorrectionQueue
(TransactionDetailID, DocumentDetailID, StartLedgerID,
 ProductID, ChangeType, StatusID, NewQuantity, ScopeCode, CreatedAt)
VALUES
(@TDID, @DocID, @LedgerID,
 @ProductID, 'EDIT', 22, @Qty, @Scope, GETDATE())
", con, tran)

                cmd.Parameters.AddWithValue("@TDID", transactionDetailID)
                cmd.Parameters.AddWithValue("@DocID", documentDetailID)
                cmd.Parameters.AddWithValue("@LedgerID", startLedgerID)
                cmd.Parameters.AddWithValue("@ProductID", productID)
                cmd.Parameters.AddWithValue("@Qty", newQty)
                cmd.Parameters.AddWithValue("@Scope", scopeCode)

                cmd.ExecuteNonQuery()
            End Using

        Next

    End Sub


    Public Sub HandlePostedCuttingEdit(
    cuttingID As Integer,
    oldTable As DataTable,
    newTable As DataTable
)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    ' 🔥 استخراج CuttingID

                    ' 🔥 بناء Queue
                    BuildCuttingCorrectionQueue(oldTable, newTable, "CUT", con, tran)

                    ' 🔥 تحديث الحجز

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub
    Public Function IsCuttingInCorrectionQueue(cuttingID As Integer) As Boolean

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT TOP 1 1
FROM inv.CorrectionQueue q
INNER JOIN inv.TransactionDetails d
    ON d.DetailID = q.TransactionDetailID
INNER JOIN inv.TransactionHeader h
    ON h.TransactionID = d.TransactionID
WHERE h.SourceDocumentID = @CuttingID
  AND h.OperationTypeID = 11
  AND q.StatusID IN (22, 23)
", con)

                cmd.Parameters.AddWithValue("@CuttingID", cuttingID)

                con.Open()
                Dim obj = cmd.ExecuteScalar()
                Return obj IsNot Nothing
            End Using
        End Using

    End Function
    Public Sub ValidatePostedCuttingEdit(
    cuttingID As Integer,
    oldTable As DataTable,
    newTable As DataTable
)

        Dim newConsumed As Decimal = 0D
        Dim oldConsumed As Decimal = 0D
        Dim productID As Integer = 0
        Dim storeID As Integer = 0

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT BaseProductID, SourceStoreID, ConsumedVolume_m3
FROM prod.CuttingHeader
WHERE CuttingID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)
                con.Open()

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        productID = rd.GetInt32(0)
                        storeID = rd.GetInt32(1)
                        oldConsumed = rd.GetDecimal(2) ' 🔥 هذا هو الصح
                    Else
                        Throw New Exception("لم يتم العثور على سند القص")
                    End If
                End Using
            End Using
        End Using

        For Each r As DataRow In newTable.Rows
            newConsumed += Convert.ToDecimal(r("Qty"))
        Next

        Dim extraRawNeeded As Decimal = newConsumed - oldConsumed
        If extraRawNeeded <= 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT BaseProductID, SourceStoreID
FROM prod.CuttingHeader
WHERE CuttingID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                con.Open()


                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        productID = rd.GetInt32(0)
                        storeID = rd.GetInt32(1)
                    Else
                        Throw New Exception("لم يتم العثور على سند القص")
                    End If
                End Using

                Dim available As Decimal = GetAvailableQty(productID, storeID)

                If available < extraRawNeeded Then
                    Throw New Exception(
                        "لا يمكن حفظ التعديل على السند المرحل لأن الزيادة المطلوبة في الخام (" &
                        extraRawNeeded.ToString("N3") &
                        ") أكبر من المتاح (" &
                        available.ToString("N3") & ")."
                    )
                End If

            End Using
        End Using

    End Sub
End Class