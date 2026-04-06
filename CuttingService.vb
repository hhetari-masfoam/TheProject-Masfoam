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
INSERT INTO Production_CuttingHeader
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
UPDATE Production_CuttingHeader
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
DELETE FROM Production_CuttingOutput WHERE CutID=@ID
", con, tran)
                    cmdDel.Parameters.AddWithValue("@ID", CuttingID)
                    cmdDel.ExecuteNonQuery()

                    ' =========================
                    ' إدخال النواتج
                    ' =========================
                    For Each r As DataRow In Outputs.Rows

                        Dim cmd As New SqlCommand("
INSERT INTO Production_CuttingOutput
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
SELECT AvgCostPerM3 FROM Master_FinalProductAvgCost WHERE BaseProductID=@ID
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
FROM Production_CuttingOutput WHERE CutID=@ID
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
UPDATE Production_CuttingOutput
SET UnitCost = TotalVolume_m3*@Cost/NULLIF(QtyPieces,0),
    TotalCost = TotalVolume_m3*@Cost
WHERE CutID=@ID AND IsMix=0
", con, tran)

                    cmdCost.Parameters.AddWithValue("@Cost", OutAvgCost)
                    cmdCost.Parameters.AddWithValue("@ID", CuttingID)
                    cmdCost.ExecuteNonQuery()

                    Dim cmdMix As New SqlCommand("
UPDATE Production_CuttingOutput
SET UnitCost=0,TotalCost=0
WHERE CutID=@ID AND IsMix=1
", con, tran)

                    cmdMix.Parameters.AddWithValue("@ID", CuttingID)
                    cmdMix.ExecuteNonQuery()

                    ' =========================
                    ' Flags
                    ' =========================
                    Dim cmdFlag As New SqlCommand("
UPDATE Production_CuttingHeader
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
FROM Production_CuttingHeader 
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

                    ' (1)
                    Using cmd As New SqlCommand("
SELECT BaseProductID, SourceStoreID, ConsumedVolume_m3
FROM Production_CuttingHeader
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

                    ' (2)
                    Using cmd As New SqlCommand("
SELECT ISNULL(AvgCostPerM3,0)
FROM Master_FinalProductAvgCost
WHERE BaseProductID=@ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", BaseProductID)
                        BaseAvgCostPerM3 = Convert.ToDecimal(cmd.ExecuteScalar())
                    End Using

                    ' (3)
                    Using cmd As New SqlCommand("
SELECT OperationTypeID
FROM Workflow_OperationType
WHERE OperationCode='CUT' AND IsActive=1
", con, tran)

                        Dim obj = cmd.ExecuteScalar()
                        If obj Is Nothing Then Throw New Exception("OperationType غير معرف")
                        OperationTypeID = Convert.ToInt32(obj)
                    End Using

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

                    ' (4) Header
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionHeader
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

                    ' (5) RAW
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionDetails
(TransactionID,ProductID,Quantity,UnitID,UnitCost,CostAmount,
 SourceStoreID,TargetStoreID,SourceDocumentDetailID,ReferenceDetailID,
 CreatedAt,CreatedBy,CorrectionReferenceDetailID)
VALUES
(@TID,@PID,@Q,
 (SELECT StorageUnitID FROM Master_Product WHERE ProductID=@PID),
 @Cost,@Amount,
 @Source,NULL,@CutID,NULL,@Now,@User,NULL)
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

                    ' (6) GOOD
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionDetails
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
NULL
FROM Production_CuttingOutput o
INNER JOIN Master_Product p ON p.ProductID=o.ProductID
WHERE o.CutID=@CutID AND o.IsMix=0
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", TransactionID)
                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (7) MIX
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionDetails
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
NULL
FROM Production_CuttingOutput o
INNER JOIN Master_Product p ON p.ProductID=o.ProductID
WHERE o.CutID=@CutID AND o.IsMix=1
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", TransactionID)
                        cmd.Parameters.AddWithValue("@CutID", CuttingID)
                        cmd.Parameters.AddWithValue("@Now", NowDate)
                        cmd.Parameters.AddWithValue("@User", UserID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (8)
                    Using cmd As New SqlCommand("
UPDATE Production_CuttingHeader
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
DELETE FROM Inventory_Reservation
WHERE SourceID=@ID AND SourceOperationTypeID=11
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CuttingID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' 2- إدخال الحجز الجديد (الخام فقط)
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_Reservation
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
FROM Production_CuttingHeader h
LEFT JOIN Master_FinalProductAvgCost f
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
        FROM Inventory_Reservation
        WHERE ProductID=@P AND SourceStoreID=@S
          AND ReservationStatusID=1
    ),0)
FROM Inventory_CostLedger
WHERE ProductID=@P AND StoreID=@S
", con)

                cmd.Parameters.AddWithValue("@P", ProductID)
                cmd.Parameters.AddWithValue("@S", StoreID)

                con.Open()
                Return Convert.ToDecimal(cmd.ExecuteScalar())
            End Using
        End Using

    End Function

End Class