
Imports System.Data.SqlClient
    Imports System.Text

Public Class TransactionRepository

    Public Function GetNextTransactionCode(
    codeType As String,
    con As SqlConnection,
    tran As SqlTransaction
) As String

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

    Public Function InsertTransferHeader(
    transactionDate As DateTime,
    transactionCode As String,
    operationTypeID As Integer,
    userId As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Dim periodID As Integer = GetCurrentPeriodID(transactionDate, con, tran)

        Using cmd As New SqlCommand("
DECLARE @Now DATETIME2(7) = SYSDATETIME();

INSERT INTO Inventory_TransactionHeader
(
    TransactionDate,
    SourceDocumentID,
    OperationTypeID,
    PeriodID,
    StatusID,
    IsFinancialPosted,
    IsInventoryPosted,
    CreatedBy,
    CreatedAt,
    SentAt,
    SentBy
)
VALUES
(
    @TransactionDate,
    0,
    @OperationTypeID,
    @PeriodID,
    5,              -- مرسل
    0,
    0,
    @UserID,
    @Now,
    @Now,           -- ✅ SentAt = CreatedAt
    @UserID         -- ✅ SentBy = CreatedBy
);

SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tran)

            cmd.Parameters.AddWithValue("@TransactionDate", transactionDate)
            cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
            cmd.Parameters.AddWithValue("@PeriodID", periodID)
            cmd.Parameters.AddWithValue("@UserID", userId)

            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using

    End Function
    Public Function ResolveUnitCostForPosting(
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Const M3UnitID As Integer = 8

        Using cmd As New SqlCommand("
SELECT
    UnitCost =
    CASE
        -- (1) وحدة المنتج م3 وليس له BaseProductID
        WHEN p.StorageUnitID = @M3UnitID AND ISNULL(p.BaseProductID,0) = 0 THEN
            ISNULL(fpSelf.AvgCostPerM3, 0)

        -- (2) وحدة المنتج ليست م3 وله BaseProductID ووحدة الأب = م3
        WHEN p.StorageUnitID <> @M3UnitID
             AND ISNULL(p.BaseProductID,0) <> 0
             AND baseP.StorageUnitID = @M3UnitID THEN
            ISNULL(fpBase.AvgCostPerM3forFG, 0)

        -- (3) وحدة المنتج ليست م3 وليس له BaseProductID
        WHEN p.StorageUnitID <> @M3UnitID AND ISNULL(p.BaseProductID,0) = 0 THEN
            ISNULL(p.AvgCost, 0)

        ELSE 0
    END
FROM dbo.Master_Product p
LEFT JOIN dbo.Master_Product baseP
    ON baseP.ProductID = p.BaseProductID
OUTER APPLY (
    SELECT TOP (1) f.AvgCostPerM3
    FROM dbo.Master_FinalProductAvgCost f
    WHERE f.BaseProductID = p.ProductID
    ORDER BY f.LastUpdated DESC
) fpSelf
OUTER APPLY (
    SELECT TOP (1) f.AvgCostPerM3forFG
    FROM dbo.Master_FinalProductAvgCost f
    WHERE f.BaseProductID = p.BaseProductID
    ORDER BY f.LastUpdated DESC
) fpBase
WHERE p.ProductID = @ProductID
", con, tran)

            cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
            cmd.Parameters.Add("@M3UnitID", SqlDbType.Int).Value = M3UnitID

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
            Return Convert.ToDecimal(obj)
        End Using
    End Function

    Private Function GetCurrentPeriodID(transactionDate As DateTime, con As SqlConnection, tran As SqlTransaction) As Integer
        ' جلب الفترة المالية المناسبة للتاريخ
        Dim sql = "
        SELECT TOP 1 PeriodID 
        FROM cfg.FiscalPeriod 
        WHERE @TransactionDate BETWEEN StartDate AND EndDate 
          AND IsOpen = 1
        ORDER BY PeriodID"

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionDate", transactionDate)
            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                ' ✅ لا نعيد استخدام cmd، ننشئ كائن جديد في Using منفصل
                Dim sql2 = "SELECT TOP 1 PeriodID FROM cfg.FiscalPeriod ORDER BY PeriodID DESC"
                Using cmd2 As New SqlCommand(sql2, con, tran)
                    result = cmd2.ExecuteScalar()
                End Using

                If result Is Nothing Then
                    Throw New Exception("لا توجد فترات مالية في النظام")
                End If
            End If

            Return Convert.ToInt32(result)
        End Using
    End Function
    Public Sub InsertTransferDetails(
    transactionID As Integer,
    details As List(Of TransactionDetailDTO),
    con As SqlConnection,
    tran As SqlTransaction
)

        For Each d In details
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
                CreatedAt,
                CreatedBy
            )
            VALUES
            (
                @TransactionID,
                @ProductID,
                @Quantity,
                @UnitID,
                @UnitCost,
                @CostAmount,
                @SourceStoreID,
                @TargetStoreID,
                0,                          -- SourceDocumentDetailID
                SYSDATETIME(),
                @CreatedBy
            )", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@ProductID", d.ProductID)
                cmd.Parameters.AddWithValue("@Quantity", d.Quantity)
                cmd.Parameters.AddWithValue("@UnitID", d.UnitID)
                cmd.Parameters.AddWithValue("@UnitCost", d.UnitCost)
                cmd.Parameters.AddWithValue("@CostAmount", d.Quantity * d.UnitCost)
                cmd.Parameters.AddWithValue("@SourceStoreID", If(d.SourceStoreID > 0, d.SourceStoreID, DBNull.Value))
                cmd.Parameters.AddWithValue("@TargetStoreID", If(d.TargetStoreID > 0, d.TargetStoreID, DBNull.Value))
                cmd.Parameters.AddWithValue("@CreatedBy", 1) ' CurrentUserID

                cmd.ExecuteNonQuery()
            End Using
        Next

    End Sub
    Public Sub UpdateSelfReference(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Using cmd As New SqlCommand("
        UPDATE Inventory_TransactionHeader
        SET SourceDocumentID = @TransactionID
        WHERE TransactionID = @TransactionID", con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.ExecuteNonQuery()

        End Using

    End Sub

    Public Sub ExecutePostTransaction(
    transactionID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim sql As New StringBuilder()

        sql.AppendLine("
/* =========================
   Basic validation
   ========================= */
IF NOT EXISTS (
    SELECT 1
    FROM dbo.Inventory_TransactionHeader
    WHERE TransactionID = @TransactionID
)
    THROW 60000, 'Invalid Transaction', 1;

IF EXISTS (
    SELECT 1
    FROM dbo.Inventory_TransactionHeader
    WHERE TransactionID = @TransactionID
      AND IsInventoryPosted = 1
)
BEGIN
    THROW 50001, N'تم استلام/ترحيل السند مسبقًا', 1;
END

/* =========================
   ✅ Fill posting cost into Inventory_TransactionDetails BEFORE ledger
   UnitID(M3)=8
   ========================= */
DECLARE @M3UnitID INT = 8;

;WITH CostRule AS
(
    SELECT
        d.DetailID,
        UnitCost =
        CASE
            -- (1) المنتج وحدته م3 وليس له BaseProductID
            WHEN p.StorageUnitID = @M3UnitID
                 AND ISNULL(p.BaseProductID,0) = 0
            THEN ISNULL(fpSelfTop.AvgCostPerM3, 0)

            -- (2) المنتج وحدته ليست م3 وله BaseProductID ووحدة الأب = م3
            WHEN p.StorageUnitID <> @M3UnitID
                 AND ISNULL(p.BaseProductID,0) <> 0
                 AND baseP.StorageUnitID = @M3UnitID
            THEN ISNULL(fpBaseTop.AvgCostPerM3forFG, 0)

            -- (3) المنتج وحدته ليست م3 وليس له BaseProductID
            WHEN p.StorageUnitID <> @M3UnitID
                 AND ISNULL(p.BaseProductID,0) = 0
            THEN ISNULL(p.AvgCost, 0)

            ELSE 0
        END
    FROM dbo.Inventory_TransactionDetails d
    INNER JOIN dbo.Master_Product p
        ON p.ProductID = d.ProductID
    LEFT JOIN dbo.Master_Product baseP
        ON baseP.ProductID = p.BaseProductID
    OUTER APPLY (
        SELECT TOP (1) f.AvgCostPerM3
        FROM dbo.Master_FinalProductAvgCost f
        WHERE f.BaseProductID = p.ProductID
        ORDER BY f.LastUpdated DESC
    ) fpSelfTop
    OUTER APPLY (
        SELECT TOP (1) f.AvgCostPerM3forFG
        FROM dbo.Master_FinalProductAvgCost f
        WHERE f.BaseProductID = p.BaseProductID
        ORDER BY f.LastUpdated DESC
    ) fpBaseTop
    WHERE d.TransactionID = @TransactionID
)
UPDATE d
SET
    d.UnitCost = c.UnitCost,
    d.CostAmount = d.Quantity * c.UnitCost
FROM dbo.Inventory_TransactionDetails d
INNER JOIN CostRule c
    ON c.DetailID = d.DetailID
WHERE d.TransactionID = @TransactionID
  AND (ISNULL(d.UnitCost,0) = 0 OR ISNULL(d.CostAmount,0) = 0);

/* ✅ Safety: prevent posting with zero cost */
IF EXISTS (
    SELECT 1
    FROM dbo.Inventory_TransactionDetails
    WHERE TransactionID = @TransactionID
      AND (ISNULL(UnitCost,0) = 0 OR ISNULL(CostAmount,0) = 0)
)
BEGIN
    THROW 51001, N'لا يمكن الترحيل: يوجد سطور بدون تكلفة بعد تطبيق قواعد المتوسطات.', 1;
END

/* =========================================================
   👇 الصق هنا باقي SQL الخاص بالترحيل (Ledger/Links/Balances)
   بدون BEGIN TRAN/COMMIT/TRY لأن الترانزاكشن مُدارة من VB
   ========================================================= */
------------------------------------------------------------
-- باقي SQL كما هو عندك
------------------------------------------------------------
")

        Using cmd As New SqlCommand(sql.ToString(), con, tran)
            cmd.Parameters.Add("@TransactionID", SqlDbType.Int).Value = transactionID
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Public Function GetTransactionDetails(transactionID As Integer, con As SqlConnection, tran As SqlTransaction) As List(Of TransactionDetailDTO)
        Dim details As New List(Of TransactionDetailDTO)()

        Dim sql = "
        SELECT 
            d.DetailID,
            d.TransactionID,
            d.ProductID,
            d.Quantity,
            d.UnitID,
            d.UnitCost,
            d.SourceStoreID,
            d.TargetStoreID,
            p.BaseProductID,
            p.StorageUnitID,
            h.SourceDocumentID as SourceDocumentID,
            h.OperationTypeID
        FROM Inventory_TransactionDetails d
        INNER JOIN Master_Product p ON p.ProductID = d.ProductID
        INNER JOIN Inventory_TransactionHeader h ON h.TransactionID = d.TransactionID
        WHERE d.TransactionID = @TransactionID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

        Dim reader = cmd.ExecuteReader()
        While reader.Read()
            Dim dto As New TransactionDetailDTO()
            dto.DetailID = reader.GetInt32(0)
            dto.TransactionID = reader.GetInt32(1)
            dto.ProductID = reader.GetInt32(2)
            dto.Quantity = reader.GetDecimal(3)
            dto.UnitID = If(reader.IsDBNull(4), 0, reader.GetInt32(4))
            dto.UnitCost = reader.GetDecimal(5)
            dto.SourceStoreID = If(reader.IsDBNull(6), 0, reader.GetInt32(6))
            dto.TargetStoreID = If(reader.IsDBNull(7), 0, reader.GetInt32(7))
            dto.BaseProductID = If(reader.IsDBNull(8), 0, reader.GetInt32(8))
            dto.StorageUnitID = reader.GetInt32(9)
            dto.SourceDocumentID = If(reader.IsDBNull(10), 0, reader.GetInt32(10))
            dto.OperationTypeID = reader.GetInt32(11)

            ' تحديد المخزن المستخدم (Target للمشتريات، Source للتحويلات)
            If dto.TargetStoreID > 0 Then
                dto.StoreID = dto.TargetStoreID
            Else
                dto.StoreID = dto.SourceStoreID
            End If

            details.Add(dto)
        End While
        reader.Close()

        Return details
    End Function

End Class
