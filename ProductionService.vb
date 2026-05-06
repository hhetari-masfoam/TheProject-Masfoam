Imports System.Data.SqlClient
Imports THE_PROJECT.frmPurchases

Public Class ProductionService
    Private ReadOnly _connStr As String

    Public Sub New(connStr As String)
        _connStr = connStr
    End Sub


    Public Sub SaveProduction(
    ProductionDate As Date,
    BOMID As Integer,
    ProductID As Integer,
    SourceProductionStoreID As Integer,
    TargetProductionStoreID As Integer,
    Notes As String,
    ProductionBaseValue As Decimal,
    CreatedByUserID As Integer,
    BatchAvgCost As Decimal,
    ProductionOutput As DataTable,
    ProductionConsumption As DataTable,
    ByRef NewProductionID As Integer,
    ByRef NewProductionCode As String
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' (1) توليد الكود
                    Using cmdCode As New SqlCommand("cfg.GetNextCode", con, tran)
                        cmdCode.CommandType = CommandType.StoredProcedure
                        cmdCode.Parameters.AddWithValue("@CodeType", "PRO")

                        Dim pOut As New SqlParameter("@NextCode", SqlDbType.NVarChar, 20)
                        pOut.Direction = ParameterDirection.Output
                        cmdCode.Parameters.Add(pOut)

                        cmdCode.ExecuteNonQuery()
                        NewProductionCode = pOut.Value.ToString()
                    End Using

                    ' (2) Header
                    Using cmd As New SqlCommand("
INSERT INTO prod.ProductionHeader
(ProductionDate,BOMID,ProductID,SourceProductionStoreID,TargetProductionStoreID,
Notes,CreatedAt,ProductionBaseValue,StatusID,IsInventoryPosted,ProductionCode,CreatedByUserID)
VALUES
(@ProductionDate,@BOMID,@ProductID,@Source,@Target,
@Notes,SYSDATETIME(),@BaseValue,2,0,@Code,@UserID);
SELECT SCOPE_IDENTITY();
", con, tran)

                        cmd.Parameters.AddWithValue("@ProductionDate", ProductionDate)
                        cmd.Parameters.AddWithValue("@BOMID", BOMID)
                        cmd.Parameters.AddWithValue("@ProductID", ProductID)
                        cmd.Parameters.AddWithValue("@Source", SourceProductionStoreID)
                        cmd.Parameters.AddWithValue("@Target", TargetProductionStoreID)
                        cmd.Parameters.AddWithValue("@Notes", If(String.IsNullOrWhiteSpace(Notes), DBNull.Value, Notes))
                        cmd.Parameters.AddWithValue("@BaseValue", ProductionBaseValue)
                        cmd.Parameters.AddWithValue("@Code", NewProductionCode)
                        cmd.Parameters.AddWithValue("@UserID", CreatedByUserID)

                        NewProductionID = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    ' (3) Output
                    For Each r As DataRow In ProductionOutput.Rows
                        Using cmd As New SqlCommand("
INSERT INTO prod.ProductionOutPut
(ProductionID,ProductID,Length,Width,Height,Quantity,IsInventoryPosted,BatchAvgCost)
VALUES
(@ID,@ProductID,@L,@W,@H,@Q,0,@Cost)", con, tran)

                            cmd.Parameters.AddWithValue("@ID", NewProductionID)
                            cmd.Parameters.AddWithValue("@ProductID", r("ProductID"))
                            cmd.Parameters.AddWithValue("@L", r("Length"))
                            cmd.Parameters.AddWithValue("@W", r("Width"))
                            cmd.Parameters.AddWithValue("@H", r("Height"))
                            cmd.Parameters.AddWithValue("@Q", r("Quantity"))
                            cmd.Parameters.AddWithValue("@Cost", BatchAvgCost)

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    ' (4) Consumption
                    For Each r As DataRow In ProductionConsumption.Rows
                        Using cmd As New SqlCommand("
INSERT INTO prod.ProductionConsumption
(ProductionID,ComponentProductID,BOMQty,ActualConsumedQty,StockQtyAtTime,AvgCost,IsInventoryPosted)
SELECT
@ID,
@Component,
@BOM,
@Actual,
@Stock,
p.AvgCost,
0
FROM md.Product p
WHERE p.ProductID = @Component
", con, tran)

                            cmd.Parameters.AddWithValue("@ID", NewProductionID)
                            cmd.Parameters.AddWithValue("@Component", r("ComponentProductID"))
                            cmd.Parameters.AddWithValue("@BOM", r("BOMQty"))
                            cmd.Parameters.AddWithValue("@Actual", r("ActualConsumedQty"))
                            cmd.Parameters.AddWithValue("@Stock", r("StockQtyAtTime"))

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub
    Public Sub UpdateProduction(
        ProductionID As Integer,
        ProductionDate As Date,
        BOMID As Integer,
        ProductID As Integer,
        SourceProductionStoreID As Integer,
        TargetProductionStoreID As Integer,
        Notes As String,
        ProductionBaseValue As Decimal,
        CreatedByUserID As Integer,
        BatchAvgCost As Decimal,
        ProductionOutput As DataTable,
        ProductionConsumption As DataTable
    )

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' (1) تحقق
                    Dim status As Integer = 0
                    Dim posted As Boolean = False

                    Using cmd As New SqlCommand("
SELECT StatusID, IsInventoryPosted 
FROM prod.ProductionHeader WHERE ProductionID=@ID", con, tran)

                        cmd.Parameters.AddWithValue("@ID", ProductionID)

                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then
                                Throw New Exception("السند غير موجود")
                            End If

                            status = rd("StatusID")
                            posted = rd("IsInventoryPosted")
                        End Using
                    End Using

                    If status = 6 Then
                        Throw New Exception("لا يمكن التعديل بعد الاستلام")
                    End If
                    ' (2) Header
                    Using cmd As New SqlCommand("
UPDATE prod.ProductionHeader SET
ProductionDate=@Date,
BOMID=@BOM,
ProductID=@Product,
SourceProductionStoreID=@Source,
TargetProductionStoreID=@Target,
Notes=@Notes,
ProductionBaseValue=@Base,
CreatedAt=SYSDATETIME(),
CreatedByUserID=@User
WHERE ProductionID=@ID", con, tran)

                        cmd.Parameters.AddWithValue("@Date", ProductionDate)
                        cmd.Parameters.AddWithValue("@BOM", BOMID)
                        cmd.Parameters.AddWithValue("@Product", ProductID)
                        cmd.Parameters.AddWithValue("@Source", SourceProductionStoreID)
                        cmd.Parameters.AddWithValue("@Target", TargetProductionStoreID)
                        cmd.Parameters.AddWithValue("@Notes", If(String.IsNullOrWhiteSpace(Notes), DBNull.Value, Notes))
                        cmd.Parameters.AddWithValue("@Base", ProductionBaseValue)
                        cmd.Parameters.AddWithValue("@User", CreatedByUserID)
                        cmd.Parameters.AddWithValue("@ID", ProductionID)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' (3) حذف
                    Using cmd As New SqlCommand("DELETE FROM prod.ProductionOutPut WHERE ProductionID=@ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", ProductionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SqlCommand("DELETE FROM prod.ProductionConsumption WHERE ProductionID=@ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", ProductionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' (4) إعادة الإدخال (نفس الحفظ)
                    For Each r As DataRow In ProductionOutput.Rows
                        Using cmd As New SqlCommand("
INSERT INTO prod.ProductionOutPut
(ProductionID,ProductID,Length,Width,Height,Quantity,IsInventoryPosted,BatchAvgCost)
VALUES (@ID,@Product,@L,@W,@H,@Q,0,@Cost)", con, tran)

                            cmd.Parameters.AddWithValue("@ID", ProductionID)
                            cmd.Parameters.AddWithValue("@Product", r("ProductID"))
                            cmd.Parameters.AddWithValue("@L", r("Length"))
                            cmd.Parameters.AddWithValue("@W", r("Width"))
                            cmd.Parameters.AddWithValue("@H", r("Height"))
                            cmd.Parameters.AddWithValue("@Q", r("Quantity"))
                            cmd.Parameters.AddWithValue("@Cost", BatchAvgCost)

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    For Each r As DataRow In ProductionConsumption.Rows
                        Using cmd As New SqlCommand("
INSERT INTO prod.ProductionConsumption
(ProductionID,ComponentProductID,BOMQty,ActualConsumedQty,StockQtyAtTime,AvgCost,IsInventoryPosted)
SELECT @ID,@Component,@BOM,@Actual,@Stock,p.AvgCost,0
FROM md.Product p
WHERE p.ProductID=@Component", con, tran)

                            cmd.Parameters.AddWithValue("@ID", ProductionID)
                            cmd.Parameters.AddWithValue("@Component", r("ComponentProductID"))
                            cmd.Parameters.AddWithValue("@BOM", r("BOMQty"))
                            cmd.Parameters.AddWithValue("@Actual", r("ActualConsumedQty"))
                            cmd.Parameters.AddWithValue("@Stock", r("StockQtyAtTime"))

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub




    Private Enum CancelActionType
        Delete
        Zero
        None
    End Enum

    Private Function GetProductionCancelAction(statusID As Integer) As CancelActionType

        Select Case statusID

            Case 1, 2
                Return CancelActionType.Delete

            Case 5
                Return CancelActionType.Zero

            Case 10, 11
                Return CancelActionType.None

            Case Else
                Return CancelActionType.None

        End Select

    End Function

    Public Sub CancelProduction(productionID As Integer, userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim statusID As Integer

                    ' 1) قراءة الحالة
                    Using cmd As New SqlCommand("
SELECT StatusID
FROM prod.ProductionHeader
WHERE ProductionID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", productionID)

                        Dim result = cmd.ExecuteScalar()

                        If result Is Nothing Then
                            Throw New Exception("السند غير موجود")
                        End If

                        statusID = CInt(result)
                    End Using

                    Dim action = GetProductionCancelAction(statusID)

                    '==================================
                    ' DELETE
                    '==================================
                    If action = CancelActionType.Delete Then

                        Using cmd As New SqlCommand("
DELETE FROM prod.ProductionConsumption WHERE ProductionID=@ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using

                        Using cmd As New SqlCommand("
DELETE FROM prod.ProductionOutPut WHERE ProductionID=@ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using

                        Using cmd As New SqlCommand("
DELETE FROM prod.ProductionHeader WHERE ProductionID=@ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using

                        '==================================
                        ' ZERO
                        '==================================
                    ElseIf action = CancelActionType.Zero Then


                        ' تحديث الحالة → ملغي
                        Using cmd As New SqlCommand("
UPDATE prod.ProductionHeader
SET StatusID = 10
WHERE ProductionID = @ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using
                        Using cmd As New SqlCommand("
                    UPDATE inv.TransactionHeader
                    SET StatusID = 10
                    WHERE SourceDocumentID = @ID
                    ", con, tran)

                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()

                        End Using

                    Else
                        Throw New Exception("لا يمكن إلغاء هذا السند")
                    End If

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub

    Public Sub SavePostedProductionEdit(
    productionID As Integer,
    productionDate As Date,
    notes As String,
    productionBaseValue As Decimal,
    batchAvgCost As Decimal,
    outputTable As DataTable,
    consumptionTable As DataTable,
    userID As Integer
)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    Dim productID As Integer = 0
                    Dim sourceStoreID As Integer = 0
                    Dim targetStoreID As Integer = 0
                    Dim statusID As Integer = 0
                    Dim stockTransactionID As Integer = 0

                    Using cmd As New SqlCommand("
SELECT ProductID, SourceProductionStoreID, TargetProductionStoreID, StatusID, ISNULL(StockTransactionID,0)
FROM prod.ProductionHeader
WHERE ProductionID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", productionID)

                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then
                                Throw New Exception("سند الإنتاج غير موجود.")
                            End If

                            productID = CInt(rd("ProductID"))
                            sourceStoreID = CInt(rd("SourceProductionStoreID"))
                            targetStoreID = CInt(rd("TargetProductionStoreID"))
                            statusID = CInt(rd("StatusID"))
                            stockTransactionID = CInt(rd(4))
                        End Using
                    End Using

                    If statusID <> 6 Then
                        Throw New Exception("هذه الدالة مخصصة لتعديل السند المرحل فقط.")
                    End If
                    Dim oldTotalVolume As Decimal = GetOldTotalVolume(productionID, con, tran)
                    Dim oldConsumptionDict = GetOldConsumptionDict(productionID, con, tran)
                    Dim testVol As Decimal = 0D

                    For Each r As DataRow In outputTable.Rows
                        testVol += (CDec(r("Length")) *
                CDec(r("Width")) *
                CDec(r("Height")) *
                CDec(r("Quantity"))) / 1000000D
                    Next

                    ValidatePostedProductionEdit(
    productionID,
    productID,
    sourceStoreID,
    targetStoreID,
    outputTable,
    consumptionTable,
    oldTotalVolume,
    oldConsumptionDict,
    con,
    tran
)
                    BuildProductionCorrectionQueue(
    productionID,
    stockTransactionID,
    productID,
    outputTable,
    consumptionTable,
    oldTotalVolume,
    oldConsumptionDict,
    con,
    tran
)

                    UpdatePostedProductionHeader(
                        productionID,
                        productionDate,
                        notes,
                        productionBaseValue,
                        con,
                        tran
                    )

                    UpdatePostedProductionOutputRows(
                        outputTable,
                        batchAvgCost,
                        con,
                        tran
                    )

                    UpdatePostedProductionConsumptionRows(
                        consumptionTable,
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
    Private Sub ValidatePostedProductionEdit(
    productionID As Integer,
    finalProductID As Integer,
    sourceStoreID As Integer,
    targetStoreID As Integer,
    outputTable As DataTable,
    consumptionTable As DataTable,
    oldTotalVolume As Decimal,
    oldConsumptionDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        ' 1) تحقق من الزيادة في الخام
        For Each r As DataRow In consumptionTable.Rows

            Dim consumptionID As Integer = CInt(r("ConsumptionID"))
            Dim productID As Integer = CInt(r("ComponentProductID"))
            Dim newQty As Decimal = CDec(r("ActualConsumedQty"))

            Dim oldQty As Decimal = 0D

            If oldConsumptionDict.ContainsKey(consumptionID) Then
                oldQty = oldConsumptionDict(consumptionID)
            Else
                Throw New Exception("تعذر قراءة صف استهلاك رقم " & consumptionID)
            End If

            Dim extraNeeded As Decimal = newQty - oldQty

            If extraNeeded > 0D Then
                Dim availableQty As Decimal = GetAvailableStock(productID, sourceStoreID, con, tran)

                If availableQty < extraNeeded Then
                    Throw New Exception("لا يوجد رصيد كاف للصنف الخام " & productID & " والفرق المطلوب = " &
                                        extraNeeded.ToString("N2"))
                End If
            End If
        Next

        ' 2) تحقق من إنقاص المنتج النهائي


        Dim newTotalVolume As Decimal = 0D

        For Each r As DataRow In outputTable.Rows

            Dim length As Decimal = CDec(r("Length"))
            Dim width As Decimal = CDec(r("Width"))
            Dim height As Decimal = CDec(r("Height"))
            Dim qty As Decimal = CDec(r("Quantity"))

            Dim volume As Decimal = 0D

            If length > 0 AndAlso width > 0 AndAlso height > 0 AndAlso qty > 0 Then
                volume = (length * width * height * qty) / 1000000D
            End If

            newTotalVolume += volume

        Next

        Dim reduceVolume As Decimal = oldTotalVolume - newTotalVolume

        If reduceVolume > 0D Then
            Dim availableFG As Decimal = GetAvailableStock(finalProductID, targetStoreID, con, tran)

            If availableFG < reduceVolume Then
                Throw New Exception("لا توجد كمية متاحة كافية من المنتج النهائي في مخزن الهدف. الفرق المطلوب = " &
                                    reduceVolume.ToString("N2"))
            End If
        End If

    End Sub
    Private Function GetAvailableStock(
    productID As Integer,
    storeID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT ISNULL(SUM(InQty - OutQty),0)
FROM inv.CostLedger
WHERE ProductID = @ProductID
  AND StoreID = @StoreID
  AND IsActive = 1
  AND IsReversed = 0
", con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@StoreID", storeID)

            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then Return 0D

            Return CDec(result)
        End Using

    End Function
    Private Sub UpdatePostedProductionHeader(
    productionID As Integer,
    productionDate As Date,
    notes As String,
    productionBaseValue As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
UPDATE prod.ProductionHeader
SET ProductionDate = @Date,
    Notes = @Notes,
    ProductionBaseValue = @BaseValue
WHERE ProductionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@Date", productionDate)
            cmd.Parameters.AddWithValue("@Notes", If(String.IsNullOrWhiteSpace(notes), DBNull.Value, notes))
            cmd.Parameters.AddWithValue("@BaseValue", productionBaseValue)
            cmd.Parameters.AddWithValue("@ID", productionID)

            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Private Sub UpdatePostedProductionOutputRows(
    outputTable As DataTable,
    batchAvgCost As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
)

        For Each r As DataRow In outputTable.Rows
            Using cmd As New SqlCommand("
UPDATE prod.ProductionOutPut
SET Length = @L,
    Width = @W,
    Height = @H,
    Quantity = @Q,
    BatchAvgCost = @Cost
WHERE OutputID = @OutputID
", con, tran)

                cmd.Parameters.AddWithValue("@L", CDec(r("Length")))
                cmd.Parameters.AddWithValue("@W", CDec(r("Width")))
                cmd.Parameters.AddWithValue("@H", CDec(r("Height")))
                cmd.Parameters.AddWithValue("@Q", CDec(r("Quantity")))
                cmd.Parameters.AddWithValue("@Cost", batchAvgCost)
                cmd.Parameters.AddWithValue("@OutputID", CInt(r("OutputID")))

                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub
    Private Sub UpdatePostedProductionConsumptionRows(
    consumptionTable As DataTable,
    con As SqlConnection,
    tran As SqlTransaction
)

        For Each r As DataRow In consumptionTable.Rows
            Using cmd As New SqlCommand("
UPDATE prod.ProductionConsumption
SET BOMQty = @BOMQty,
    ActualConsumedQty = @ActualQty,
    StockQtyAtTime = @StockQty,
    AvgCost = @AvgCost
WHERE ConsumptionID = @ConsumptionID
", con, tran)

                cmd.Parameters.AddWithValue("@BOMQty", CDec(r("BOMQty")))
                cmd.Parameters.AddWithValue("@ActualQty", CDec(r("ActualConsumedQty")))
                cmd.Parameters.AddWithValue("@StockQty", CDec(r("StockQtyAtTime")))
                cmd.Parameters.AddWithValue("@AvgCost", CDec(r("AvgCost")))
                cmd.Parameters.AddWithValue("@ConsumptionID", CInt(r("ConsumptionID")))

                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub
    Private Sub BuildProductionCorrectionQueue(
    productionID As Integer,
    stockTransactionID As Integer,
    finalProductID As Integer,
    outputTable As DataTable,
    consumptionTable As DataTable,
    oldTotalVolume As Decimal,
    oldConsumptionDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        ' خام: سجل فقط ما تغير
        For Each r As DataRow In consumptionTable.Rows

            Dim consumptionID As Integer = CInt(r("ConsumptionID"))
            Dim productID As Integer = CInt(r("ComponentProductID"))
            Dim newQty As Decimal = CDec(r("ActualConsumedQty"))

            Dim oldQty As Decimal = 0D

            If oldConsumptionDict.ContainsKey(consumptionID) Then
                oldQty = oldConsumptionDict(consumptionID)
            Else
                Continue For
            End If
            If oldQty <> newQty Then
                Dim transactionDetailID As Object = DBNull.Value
                Dim startLedgerID As Object = DBNull.Value

                Using cmd As New SqlCommand("
SELECT TOP 1 
    td.DetailID,
    cl.LedgerID
FROM inv.TransactionDetails td
LEFT JOIN inv.CostLedger cl
    ON cl.SourceDetailID = td.DetailID
WHERE td.SourceDocumentDetailID = @DocID
ORDER BY cl.LedgerID DESC
", con, tran)

                    cmd.Parameters.AddWithValue("@DocID", consumptionID)

                    Using rd = cmd.ExecuteReader()
                        If rd.Read() Then

                            If Not IsDBNull(rd(0)) Then
                                transactionDetailID = rd(0)
                            End If

                            If Not IsDBNull(rd(1)) Then
                                startLedgerID = rd(1)
                            End If

                        End If
                    End Using
                End Using
                Using cmd As New SqlCommand("
INSERT INTO inv.CorrectionQueue
(TransactionDetailID, DocumentDetailID, StartLedgerID, StatusID, CreatedAt,
 ScopeCode, ChangeType, ProductID, NewQuantity, NewUnitCost, CostGroupID)
VALUES
(@TDID, @DocumentDetailID, @StartLedgerID, 22, SYSDATETIME(),
 'PRO', 'EDIT', @ProductID, @NewQuantity, 0, NULL)
", con, tran)

                    cmd.Parameters.AddWithValue("@TDID", transactionDetailID)
                    cmd.Parameters.AddWithValue("@StartLedgerID", startLedgerID)
                    cmd.Parameters.AddWithValue("@DocumentDetailID", consumptionID)
                    cmd.Parameters.AddWithValue("@ProductID", productID)
                    cmd.Parameters.AddWithValue("@NewQuantity", newQty)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        Next

        ' داخل: سجل واحد فقط إذا تغير مجموع الحجم

        Dim newTotalVolume As Decimal = 0D

        For Each r As DataRow In outputTable.Rows

            Dim length As Decimal = CDec(r("Length"))
            Dim width As Decimal = CDec(r("Width"))
            Dim height As Decimal = CDec(r("Height"))
            Dim qty As Decimal = CDec(r("Quantity"))

            Dim volume As Decimal = 0D

            If length > 0 AndAlso width > 0 AndAlso height > 0 AndAlso qty > 0 Then
                volume = (length * width * height * qty) / 1000000D
            End If

            newTotalVolume += volume

        Next

        Dim diff As Decimal = oldTotalVolume - newTotalVolume

        If diff <> 0D Then
            Dim transactionDetailID As Object = DBNull.Value
            Dim startLedgerID As Object = DBNull.Value

            Using cmd As New SqlCommand("
SELECT TOP 1 
    td.DetailID,
    cl.LedgerID
FROM inv.TransactionDetails td
LEFT JOIN inv.CostLedger cl
    ON cl.SourceDetailID = td.DetailID
WHERE td.TransactionID = @TransID
  AND td.ProductID = @ProductID
ORDER BY cl.LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@TransID", stockTransactionID)
                cmd.Parameters.AddWithValue("@ProductID", finalProductID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        If Not IsDBNull(rd(0)) Then
                            transactionDetailID = rd(0)
                        End If

                        If Not IsDBNull(rd(1)) Then
                            startLedgerID = rd(1)
                        End If

                    End If
                End Using
            End Using
            Using cmd As New SqlCommand("
INSERT INTO inv.CorrectionQueue
(TransactionDetailID, DocumentDetailID, StartLedgerID, StatusID, CreatedAt,
 ScopeCode, ChangeType, ProductID, NewQuantity, NewUnitCost, CostGroupID)
VALUES
(@TDID, @DocumentDetailID, @StartLedgerID, 22, SYSDATETIME(),
 'PRO', 'EDIT', @ProductID, @NewQuantity, 0, NULL)
", con, tran)
                cmd.Parameters.AddWithValue("@TDID", transactionDetailID)
                cmd.Parameters.AddWithValue("@StartLedgerID", startLedgerID)
                cmd.Parameters.AddWithValue("@DocumentDetailID", productionID)
                cmd.Parameters.AddWithValue("@ProductID", finalProductID)
                cmd.Parameters.AddWithValue("@NewQuantity", newTotalVolume)

                cmd.ExecuteNonQuery()
            End Using
        End If

    End Sub
    Private Sub DeleteProductionReservations(
    productionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
DELETE r
FROM inv.Reservation r
INNER JOIN prod.ProductionConsumption c
    ON r.SourceID = c.ConsumptionID
WHERE c.ProductionID = @ProductionID
", con, tran)

            cmd.Parameters.AddWithValue("@ProductionID", productionID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Private Sub RebuildProductionReservations(
    productionID As Integer,
    sourceStoreID As Integer,
    operationTypeID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
INSERT INTO inv.Reservation
(ProductID, SourceStoreID, ReservedQty, SourceOperationTypeID, SourceID,
 CostAtReserve, ReservedAt, CreatedBy, ReservationStatusID, SourceDetailID)
SELECT
    c.ComponentProductID,
    @SourceStoreID,
    c.ActualConsumedQty,
    @OperationTypeID,
    c.ConsumptionID,
    c.TotalCost,
    SYSDATETIME(),
    1,
    1,
    c.ConsumptionID
FROM prod.ProductionConsumption c
WHERE c.ProductionID = @ProductionID
  AND c.ActualConsumedQty > 0
", con, tran)

            cmd.Parameters.AddWithValue("@ProductionID", productionID)
            cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)
            cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Private Function GetOldTotalVolume(
    productionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT ISNULL(SUM(VolumeM3),0)
FROM prod.ProductionOutPut
WHERE ProductionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", productionID)
            Return CDec(cmd.ExecuteScalar())
        End Using

    End Function
    Private Function GetOldConsumptionDict(
    productionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Dictionary(Of Integer, Decimal)

        Dim dict As New Dictionary(Of Integer, Decimal)

        Using cmd As New SqlCommand("
SELECT ConsumptionID, ActualConsumedQty
FROM prod.ProductionConsumption
WHERE ProductionID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", productionID)

            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    dict(rd.GetInt32(0)) = rd.GetDecimal(1)
                End While
            End Using
        End Using

        Return dict

    End Function
    Public Function IsProductionInCorrectionQueue(productionID As Integer) As Boolean

        Using con As New SqlConnection(_connStr)
            Using cmd As New SqlCommand("
SELECT TOP 1 1
FROM inv.CorrectionQueue q
WHERE q.ScopeCode = 'PRO'
  AND q.StatusID IN (22, 23)
  AND (
        q.DocumentDetailID IN (
            SELECT ConsumptionID
            FROM prod.ProductionConsumption
            WHERE ProductionID = @ProductionID
        )
        OR
        q.DocumentDetailID = @ProductionID
      )
", con)

                cmd.Parameters.AddWithValue("@ProductionID", productionID)

                con.Open()
                Dim obj = cmd.ExecuteScalar()
                Return obj IsNot Nothing
            End Using
        End Using

    End Function

End Class




