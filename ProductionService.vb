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
INSERT INTO Production_Header
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
INSERT INTO Production_Output
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
INSERT INTO Production_Consumption
(ProductionID,ComponentProductID,BOMQty,ActualConsumedQty,StockQtyAtTime,AvgCost,IsInventoryPosted)
SELECT
@ID,
@Component,
@BOM,
@Actual,
@Stock,
p.AvgCost,
0
FROM Master_Product p
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
FROM Production_Header WHERE ProductionID=@ID", con, tran)

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
UPDATE Production_Header SET
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
                    Using cmd As New SqlCommand("DELETE FROM Production_Output WHERE ProductionID=@ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", ProductionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SqlCommand("DELETE FROM Production_Consumption WHERE ProductionID=@ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", ProductionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' (4) إعادة الإدخال (نفس الحفظ)
                    For Each r As DataRow In ProductionOutput.Rows
                        Using cmd As New SqlCommand("
INSERT INTO Production_Output
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
INSERT INTO Production_Consumption
(ProductionID,ComponentProductID,BOMQty,ActualConsumedQty,StockQtyAtTime,AvgCost,IsInventoryPosted)
SELECT @ID,@Component,@BOM,@Actual,@Stock,p.AvgCost,0
FROM Master_Product p
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
FROM Production_Header
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
DELETE FROM Production_Consumption WHERE ProductionID=@ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using

                        Using cmd As New SqlCommand("
DELETE FROM Production_Output WHERE ProductionID=@ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using

                        Using cmd As New SqlCommand("
DELETE FROM Production_Header WHERE ProductionID=@ID
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
UPDATE Production_Header
SET StatusID = 10
WHERE ProductionID = @ID
", con, tran)
                            cmd.Parameters.AddWithValue("@ID", productionID)
                            cmd.ExecuteNonQuery()
                        End Using
                        Using cmd As New SqlCommand("
                    UPDATE Inventory_TransactionHeader
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

End Class




