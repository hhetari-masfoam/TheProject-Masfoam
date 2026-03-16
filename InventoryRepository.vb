Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text
Public Class InventoryRepository

    Private ReadOnly _connectionString As String
    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Function GetOldProductCosts(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Dictionary(Of Integer, Decimal)

        Dim result As New Dictionary(Of Integer, Decimal)

        Dim sql As String = "
        SELECT
            p.ProductID,
            CASE
                WHEN p.StorageUnitID = @M3UnitID
                    THEN f.AvgCostPerM3
                ELSE p.AvgCost
            END AS OldAvgCost
        FROM Master_Product p
        JOIN Inventory_TransactionDetails d
            ON d.TransactionID = @TransactionID
           AND d.ProductID     = p.ProductID
        LEFT JOIN Master_FinalProductAvgCost f
            ON f.BaseProductID = ISNULL(p.BaseProductID, p.ProductID)
    "

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using rd = cmd.ExecuteReader()

                While rd.Read()

                    Dim productID As Integer = CInt(rd("ProductID"))

                    Dim cost As Decimal =
                    If(IsDBNull(rd("OldAvgCost")),
                       0D,
                       CDec(rd("OldAvgCost")))

                    result(productID) = cost

                End While

            End Using

        End Using

        Return result

    End Function
    Public Function GetPreviousAvgCostFromCostLedger(
        productID As Integer,
        storeID As Integer,
        currentTransactionID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As AvgCostData
        Dim result As New AvgCostData()

        ' جلب آخر متوسط قبل هذه الحركة
        Dim sql = "
 SELECT TOP 1
    l.LedgerID,
    l.OldAvgCost,
    l.BaseProductID
FROM Inventory_CostLedger l
WHERE l.ProductID = @ProductID
  AND l.StoreID = @StoreID
  AND l.TransactionID = @TransactionID
  AND l.IsReversed = 0
ORDER BY l.LedgerID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", productID)
        cmd.Parameters.AddWithValue("@StoreID", storeID)
        cmd.Parameters.AddWithValue("@TransactionID", currentTransactionID)

        Dim reader = cmd.ExecuteReader()
        If reader.Read() Then
            result.OriginalLedgerID = reader.GetInt32(0)
            result.OldAvgCost = reader.GetDecimal(1)
            result.BaseProductID = reader.GetInt32(2)

        Else
            ' إذا لم يجد، استخدم المتوسط الحالي من Master_Product
            reader.Close()
            sql = "
            SELECT 
                p.AvgCost,
                p.BaseProductID,
                f.AvgCostPerM3
            FROM Master_Product p
            LEFT JOIN Master_FinalProductAvgCost f ON f.BaseProductID = p.BaseProductID
            WHERE p.ProductID = @ProductID"

            cmd = New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@ProductID", productID)

            reader = cmd.ExecuteReader()
            If reader.Read() Then
                result.OldAvgCost = reader.GetDecimal(0)
                result.BaseProductID = If(reader.IsDBNull(1), 0, reader.GetInt32(1))
                result.OldAvgCostPerM3 = If(reader.IsDBNull(2), 0D, reader.GetDecimal(2))
                result.OriginalLedgerID = 0
            End If
        End If
        reader.Close()

        Return result
    End Function
    Public Function GetOldQtyAllStores(
    transactionID As Integer,
    operationGroupID As Guid,
    con As SqlConnection,
    tran As SqlTransaction
) As Dictionary(Of Integer, Decimal)

        Dim result As New Dictionary(Of Integer, Decimal)

        Dim sql As String = "
SELECT
    cl.ProductID,
    cl.NewQty
FROM Inventory_CostLedger cl
JOIN
(
    SELECT
        ProductID,
        MAX(LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger
    WHERE IsActive = 1
      AND IsReversed = 0
      AND OperationGroupID <> @OperationGroupID
      AND ProductID IN
      (
        SELECT DISTINCT ProductID
        FROM Inventory_TransactionDetails
        WHERE TransactionID = @TransactionID
      )
    GROUP BY ProductID
) x
ON x.ProductID = cl.ProductID
AND x.LastLedgerID = cl.LedgerID
"

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)

            Using rd = cmd.ExecuteReader()

                While rd.Read()

                    Dim productID As Integer = CInt(rd("ProductID"))
                    Dim qty As Decimal =
                    If(IsDBNull(rd("NewQty")), 0D, CDec(rd("NewQty")))

                    result(productID) = qty

                End While

            End Using

        End Using

        Return result

    End Function




    Public Function GetProductUnitID(productID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer
        Dim sql = "SELECT StorageUnitID FROM Master_Product WHERE ProductID = @ProductID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", productID)

        Return CInt(cmd.ExecuteScalar())
    End Function
    Public Function GetPrevLedgerID(
productID As Integer,
baseProductID As Integer,
con As SqlConnection,
tran As SqlTransaction) As Object

        Dim sql As String = "
SELECT TOP 1 LedgerID
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
"

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@BaseProductID", baseProductID)

            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                Return DBNull.Value
            End If

            Return CLng(result)

        End Using

    End Function
    Public Function GetProductsByTargetStore(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim dt As New DataTable()
        Dim sql As String = "
   SELECT 
    d.ProductID,
    p.ProductName,
p.StorageUnitID AS StorageUnitID,
bp.StorageUnitID AS BaseUnitID,
    d.TargetStoreID
FROM Inventory_TransactionDetails d
JOIN Master_Product p 
    ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp 
    ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID = @TransactionID
AND d.TargetStoreID IS NOT NULL
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt
    End Function
    Public Sub GetCostChainContext(
    productID As Integer,
    baseProductID As Object,
    operationGroupID As Guid,
    con As SqlConnection,
    tran As SqlTransaction,
    ByRef prevLedgerID As Object,
    ByRef rootLedgerID As Object,
    ByRef oldAvgCost As Decimal
)

        prevLedgerID = DBNull.Value
        rootLedgerID = DBNull.Value
        oldAvgCost = 0D

        Dim dt As New DataTable()

        '========================================
        ' تحديد هل المنتج له أب
        '========================================
        Dim useFamily As Integer = 0
        Dim baseID As Integer = productID

        If Not IsDBNull(baseProductID) AndAlso CInt(baseProductID) <> productID Then
            useFamily = 1
            baseID = CInt(baseProductID)
        End If

        '========================================
        ' الاستعلام
        '========================================
        Using cmd As New SqlCommand("
SELECT TOP 1
    LedgerID,
    RootLedgerID,
    NewAvgCost
FROM Inventory_CostLedger
WHERE
(
    (@UseFamily = 1 AND BaseProductID = @BaseID)
    OR
    (@UseFamily = 0 AND ProductID = @ProductID)
)
AND IsActive = 1
AND IsReversed = 0
AND OperationGroupID <> @G
ORDER BY LedgerID DESC
", con, tran)

            cmd.Parameters.AddWithValue("@UseFamily", useFamily)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@BaseID", baseID)
            cmd.Parameters.AddWithValue("@G", operationGroupID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

        End Using

        If dt.Rows.Count > 0 Then

            prevLedgerID = CLng(dt.Rows(0)("LedgerID"))

            If IsDBNull(dt.Rows(0)("RootLedgerID")) Then
                rootLedgerID = prevLedgerID
            Else
                rootLedgerID = CLng(dt.Rows(0)("RootLedgerID"))
            End If

            oldAvgCost = If(IsDBNull(dt.Rows(0)("NewAvgCost")), 0D, Convert.ToDecimal(dt.Rows(0)("NewAvgCost")))

        End If

    End Sub



    Public Sub RecalculateAverage_Regular(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim sql As String = "

        ;WITH InRows AS
        (
            SELECT
                d.ProductID,
                SUM(d.Quantity) AS InQty,
SUM(d.Quantity * d.UnitCost)
/ NULLIF(SUM(d.Quantity),0) AS InAvg
            FROM Inventory_TransactionDetails d
            WHERE d.TransactionID = @TransactionID
              AND d.TargetStoreID IS NOT NULL
            GROUP BY d.ProductID
        ),
OldQty AS
(
    SELECT
        b.ProductID,
        SUM(b.QtyOnHand) AS OldQty
    FROM Inventory_Balance b
    JOIN InRows i 
        ON i.ProductID = b.ProductID
    GROUP BY 
        b.ProductID
)
UPDATE p
        SET p.AvgCost =
            CASE
                WHEN ISNULL(pl.OldQty,0) + i.InQty = 0 THEN p.AvgCost
                WHEN ISNULL(pl.OldQty,0) = 0 THEN i.InAvg
                ELSE
                    (
                        ISNULL(pl.OldQty,0) * ISNULL(p.AvgCost,0)
                        + (i.InQty * i.InAvg)
                    )
                    / (ISNULL(pl.OldQty,0) + i.InQty)
            END
        FROM Master_Product p
        JOIN InRows i ON i.ProductID = p.ProductID
        LEFT JOIN OldQty o ON o.ProductID = p.ProductID
        WHERE p.StorageUnitID <> @M3UnitID
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@M3UnitID", m3UnitID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Public Sub RecalculateAverage_M3(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim sql As String = "

;WITH src AS
(
    SELECT
        d.ProductID,
        ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
        d.TargetStoreID AS StoreID,
        SUM(d.Quantity) AS Quantity,
        p.StorageUnitID,
        p.AvgCost,
        f.AvgCostPerM3forFG
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p 
        ON p.ProductID = d.ProductID
    LEFT JOIN Master_FinalProductAvgCost f 
        ON f.BaseProductID = ISNULL(p.BaseProductID, p.ProductID)
    WHERE d.TransactionID = @TransactionID
      AND d.TargetStoreID IS NOT NULL
    GROUP BY
        d.ProductID,
        ISNULL(p.BaseProductID, p.ProductID),
        d.TargetStoreID,
        p.StorageUnitID,
        p.AvgCost,
        f.AvgCostPerM3forFG
)

MERGE Inventory_Balance AS tgt
USING src
ON tgt.ProductID     = src.ProductID
AND tgt.BaseProductID = src.BaseProductID
AND tgt.StoreID      = src.StoreID

WHEN MATCHED THEN
    UPDATE SET
        tgt.QtyOnHand     = tgt.QtyOnHand + src.Quantity,
        tgt.LastUpdatedAt = SYSDATETIME()

WHEN NOT MATCHED THEN
    INSERT
    (
        ProductID,
        BaseProductID,
        StoreID,
        QtyOnHand,
        CreatedAt,
        LastUpdatedAt
    )
    VALUES
    (
        src.ProductID,
        src.BaseProductID,
        src.StoreID,
        src.Quantity,
        SYSDATETIME(),
        SYSDATETIME()
    );
"
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@M3UnitID", m3UnitID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub

    Public Sub RecalculateAverage_PUR_PRO_BySnapshot(
    transactionID As Integer,
    m3UnitID As Integer,
    productIds As List(Of Integer),
    con As SqlConnection,
    tran As SqlTransaction
)

        If productIds Is Nothing OrElse productIds.Count = 0 Then Return

        Dim sql As String = "
;WITH P AS
(
    SELECT
        p.ProductID,
        ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
        p.StorageUnitID
    FROM dbo.Master_Product p
    WHERE p.ProductID IN (" & String.Join(",", productIds) & ")
),
L AS
(
    SELECT
        cl.ProductID,
        MAX(CASE WHEN cl.InQty > 0 THEN cl.LedgerID END) AS MaxLedgerID
    FROM dbo.Inventory_CostLedger cl
WHERE cl.IsReversed = 0
      AND cl.ProductID IN (" & String.Join(",", productIds) & ")
    GROUP BY cl.ProductID
),
V AS
(
    SELECT
        cl.ProductID,
        cl.NewAvgCost
    FROM dbo.Inventory_CostLedger cl
    JOIN L
      ON L.ProductID = cl.ProductID
     AND L.MaxLedgerID = cl.LedgerID
)
SELECT
    P.ProductID,
    P.BaseProductID,
    P.StorageUnitID,
    ISNULL(V.NewAvgCost, 0) AS NewAvgCost
FROM P
LEFT JOIN V ON V.ProductID = P.ProductID
ORDER BY P.ProductID;
"

        Dim dt As New DataTable()
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        If dt.Rows.Count = 0 Then Return
        ' 2) نفّذ "نقل القيمة" حسب السياسة
        For Each row As DataRow In dt.Rows

            Dim productId As Integer = CInt(row("ProductID"))
            Dim baseProductId As Integer = CInt(row("BaseProductID"))
            Dim storageUnitId As Integer = 0

            If dt.Columns.Contains("StorageUnitID") Then
                storageUnitId = Convert.ToInt32(row("StorageUnitID"))
            End If
            Dim newAvgCost As Decimal = CDec(row("NewAvgCost"))

            Dim isM3 As Boolean = (storageUnitId = m3UnitID)
            Dim hasParent As Boolean = (baseProductId <> productId)

            If isM3 Then
                ' ========= (A) M3 =========
                ' Update then Insert (AvgCostPerM3 NOT NULL)
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_FinalProductAvgCost
SET AvgCostPerM3 = @Val,
    LastUpdated = SYSDATETIME()
WHERE BaseProductID = @BaseProductID;
SELECT @@ROWCOUNT;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@BaseProductID", baseProductId)

                    Dim affected As Integer = CInt(cmdUp.ExecuteScalar())
                    If affected = 0 Then
                        Using cmdIns As New SqlCommand("
INSERT INTO dbo.Master_FinalProductAvgCost
(
    BaseProductID,
    AvgCostPerM3,
    AvgCostPerM3forFG,
    LastUpdated
)
VALUES
(
    @BaseProductID,
    @AvgCostPerM3,
    NULL,
    SYSDATETIME()
);
", con, tran)
                            cmdIns.Parameters.AddWithValue("@BaseProductID", baseProductId)
                            cmdIns.Parameters.AddWithValue("@AvgCostPerM3", newAvgCost)
                            cmdIns.ExecuteNonQuery()
                        End Using
                    End If
                End Using

            ElseIf hasParent Then
                ' ========= (B) Non-M3 + Parent =========
                ' Update then Insert (AvgCostPerM3 NOT NULL => نعطيه 0 عند Insert)
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_FinalProductAvgCost
SET AvgCostPerM3forFG = @Val,
    LastUpdated = SYSDATETIME()
WHERE BaseProductID = @BaseProductID;
SELECT @@ROWCOUNT;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@BaseProductID", baseProductId)

                    Dim affected As Integer = CInt(cmdUp.ExecuteScalar())
                    If affected = 0 Then
                        Using cmdIns As New SqlCommand("
INSERT INTO dbo.Master_FinalProductAvgCost
(
    BaseProductID,
    AvgCostPerM3,
    AvgCostPerM3forFG,
    LastUpdated
)
VALUES
(
    @BaseProductID,
    0,
    @AvgCostPerM3forFG,
    SYSDATETIME()
);
", con, tran)
                            cmdIns.Parameters.AddWithValue("@BaseProductID", baseProductId)
                            cmdIns.Parameters.AddWithValue("@AvgCostPerM3forFG", newAvgCost)
                            cmdIns.ExecuteNonQuery()
                        End Using
                    End If
                End Using

            Else
                ' ========= (C) Non-M3 + No Parent =========
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_Product
SET AvgCost = @Val
WHERE ProductID = @ProductID;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@ProductID", productId)
                    cmdUp.ExecuteNonQuery()
                End Using
            End If

        Next

    End Sub
    Public Sub RecalculateAverage_M3_PUR_PRO_BySnapshot(
    transactionID As Integer,
    m3UnitID As Integer,
    productIds As List(Of Integer),
    oldQtyAll As Dictionary(Of Integer, Decimal),
    ledgerID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        If productIds Is Nothing OrElse productIds.Count = 0 Then Return

        ' ============================================
        ' 1️⃣ جلب InQty و InAvg للمنتجات M3
        ' ============================================
        Dim inRows As New List(Of Tuple(Of Integer, Decimal, Decimal))()
        Dim sqlIn As String = "
    SELECT
        d.ProductID,
        SUM(d.Quantity) AS InQty,
        SUM(d.CostAmount) / NULLIF(SUM(d.Quantity), 0) AS InAvg
    FROM dbo.Inventory_TransactionDetails d
    WHERE d.TransactionID = @TransactionID
      AND d.TargetStoreID IS NOT NULL
      AND d.ProductID IN (" & String.Join(",", productIds) & ")
    GROUP BY d.ProductID
    "

        Using cmd As New SqlCommand(sqlIn, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    Dim pid As Integer = CInt(rd("ProductID"))
                    Dim inQty As Decimal = CDec(rd("InQty"))
                    Dim inAvg As Decimal = If(IsDBNull(rd("InAvg")), 0D, CDec(rd("InAvg")))
                    inRows.Add(Tuple.Create(pid, inQty, inAvg))
                End While
            End Using
        End Using

        ' ============================================
        ' 2️⃣ معالجة كل منتج على حدة باستخدام MERGE
        ' ============================================
        For Each item In inRows
            Dim pid = item.Item1
            Dim inQty = item.Item2
            Dim inAvg = item.Item3

            ' الحصول على BaseProductID
            Dim baseProductID As Integer = 0
            Using cmd As New SqlCommand("
            SELECT ISNULL(BaseProductID, ProductID)
            FROM dbo.Master_Product
            WHERE ProductID = @PID
        ", con, tran)
                cmd.Parameters.AddWithValue("@PID", pid)
                baseProductID = CInt(cmd.ExecuteScalar())
            End Using

            ' OldQty من السناب شوت
            Dim oldQty As Decimal = 0D
            If oldQtyAll IsNot Nothing AndAlso oldQtyAll.ContainsKey(pid) Then
                oldQty = oldQtyAll(pid)
            End If

            ' OldAvg من جدول FinalProductAvgCost (إذا كان موجودًا)
            Dim oldAvg As Decimal = 0D
            Using cmd As New SqlCommand("
            SELECT AvgCostPerM3
            FROM dbo.Master_FinalProductAvgCost
            WHERE BaseProductID = @BID
        ", con, tran)
                cmd.Parameters.AddWithValue("@BID", baseProductID)
                Dim r = cmd.ExecuteScalar()
                oldAvg = If(r Is Nothing OrElse IsDBNull(r), 0D, CDec(r))
            End Using
            ' حساب NewAvg
            Dim totalQty = oldQty + inQty
            Dim newAvg As Decimal
            If totalQty = 0D Then
                newAvg = oldAvg
            ElseIf oldQty = 0D Then
                newAvg = inAvg
            Else
                newAvg = ((oldQty * oldAvg) + (inQty * inAvg)) / totalQty
            End If

            ' ============================================
            ' 3️⃣ MERGE: إدراج أو تحديث في Master_FinalProductAvgCost
            ' ============================================
            Dim mergeSql As String = "
    MERGE dbo.Master_FinalProductAvgCost AS target
    USING (SELECT @BID AS BaseProductID) AS source
    ON target.BaseProductID = source.BaseProductID
    WHEN MATCHED THEN
        UPDATE SET 
            AvgCostPerM3 = @NewAvg,
            LastUpdated = SYSDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (BaseProductID, AvgCostPerM3, LastUpdated)
        VALUES (@BID, @NewAvg, SYSDATETIME());"

            Using cmd As New SqlCommand(mergeSql, con, tran)
                cmd.Parameters.AddWithValue("@BID", baseProductID)
                cmd.Parameters.AddWithValue("@NewAvg", newAvg)
                cmd.ExecuteNonQuery()
            End Using

            ' ============================================
            ' 4️⃣ تحديث TotalCost و LastMovementLedgerID في Inventory_Balance
            ' ============================================
            Using cmdTotal As New SqlCommand("
    UPDATE Inventory_Balance
    SET 
        LastMovementLedgerID = @LedgerID,
        LastMovementDate = SYSDATETIME()
    WHERE ProductID = @ProductID
", con, tran)
                cmdTotal.Parameters.AddWithValue("@LedgerID", ledgerID)
                cmdTotal.Parameters.AddWithValue("@ProductID", pid)
                cmdTotal.ExecuteNonQuery()
            End Using

        Next
    End Sub

    Private Function UsesM3LedgerUnit(
    productID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Using cmd As New SqlCommand("
SELECT
    p.StorageUnitID AS ProductUnitID,
    bp.StorageUnitID AS BaseUnitID
FROM Master_Product p
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
WHERE p.ProductID = @ProductID
", con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)

            Using rd = cmd.ExecuteReader()
                If Not rd.Read() Then
                    Throw New Exception("Product not found. ProductID=" & productID.ToString())
                End If

                Dim productUnitID As Integer =
                If(IsDBNull(rd("ProductUnitID")), 0, Convert.ToInt32(rd("ProductUnitID")))

                Dim baseUnitID As Integer =
                If(IsDBNull(rd("BaseUnitID")), 0, Convert.ToInt32(rd("BaseUnitID")))

                Return (productUnitID = m3UnitID OrElse baseUnitID = m3UnitID)
            End Using
        End Using

    End Function
    Public Sub InsertCostLedger_IN(
transactionID As Integer,
operationGroupID As Guid,
con As SqlConnection,
tran As SqlTransaction)

        Dim details As New DataTable

        Using cmd As New SqlCommand("
SELECT
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
d.TargetStoreID,
SUM(d.Quantity) AS Qty,
SUM(d.Quantity*d.UnitCost) AS TotalCost,
MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID=d.ProductID
WHERE d.TransactionID=@T
AND d.TargetStoreID IS NOT NULL
GROUP BY
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID),
d.TargetStoreID
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(details)
            End Using

        End Using

        Dim operationTypeID As Integer
        Dim postingDate As DateTime
        Dim createdBy As Integer

        Using cmd As New SqlCommand("
SELECT OperationTypeID,PostingDate,CreatedBy
FROM Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Using r = cmd.ExecuteReader()

                If r.Read() Then
                    operationTypeID = r.GetInt32(0)
                    postingDate = r.GetDateTime(1)
                    createdBy = r.GetInt32(2)
                End If

            End Using

        End Using

        For Each row As DataRow In details.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("TargetStoreID"))
            Dim qty As Decimal = CDec(row("Qty"))
            Dim totalCost As Decimal = CDec(row("TotalCost"))
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvg As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvg
        )

            Dim oldQty As Decimal = 0D

            Using cmd As New SqlCommand("
SELECT TOP 1 NewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND StoreID=@S
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@P", prodID)
                cmd.Parameters.AddWithValue("@B", baseProdID)
                cmd.Parameters.AddWithValue("@S", storeID)

                Dim v = cmd.ExecuteScalar()
                oldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim inUnitCost As Decimal = 0D

            If qty <> 0D Then
                inUnitCost = totalCost / qty
            End If

            Dim newQty As Decimal = oldQty + qty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvg
            Else
                newAvgCost = ((oldQty * oldAvg) + totalCost) / newQty
            End If

            Dim newLedgerID As Long

            Using cmd As New SqlCommand("SELECT NEXT VALUE FOR Seq_CostLedger", con, tran)
                newLedgerID = CLng(cmd.ExecuteScalar())
            End Using

            Dim ledgerType As Integer = 2
            Dim rootTransactionID As Integer = transactionID

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
LedgerID,
TransactionID,
SourceDetailID,
ProductID,
BaseProductID,
IsReversed,
IsRevaluation,
StoreID,
OperationTypeID,
PostingDate,
OldQty,
InQty,
OutQty,
NewQty,
OldAvgCost,
InUnitCost,
InTotalCost,
OutUnitCost,
OutTotalCost,
NewAvgCost,
RootLedgerID,
CreatedBy,
CreatedAt,
OperationGroupID,
GroupSeq,
ScopeKeyType,
ScopeKeyID,
PrevLedgerID,
LedgerType,
RootTransactionID,
LedgerSequence,
IsActive
)
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@T,
@DetailID,
@ProductID,
@BaseProductID,
0,
0,
@StoreID,
@OpType,
@PostingDate,
@OldQty,
@InQty,
0,
@NewQty,
@OldAvg,
@InUnitCost,
@InTotalCost,
0,
0,
@NewAvgCost,
@RootLedgerID,
@CreatedBy,
SYSDATETIME(),
@GroupID,
2,
'PRODUCT',
@ProductID,
@PrevLedgerID,
@LedgerType,
@RootTransactionID,
@LedgerSequence,
1
)
", con, tran)

                cmd.Parameters.AddWithValue("@LedgerID", newLedgerID)
                cmd.Parameters.AddWithValue("@T", transactionID)
                cmd.Parameters.AddWithValue("@DetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OpType", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)
                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", qty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)
                cmd.Parameters.AddWithValue("@OldAvg", oldAvg)
                cmd.Parameters.AddWithValue("@InUnitCost", inUnitCost)
                cmd.Parameters.AddWithValue("@InTotalCost", totalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If
                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", newLedgerID)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If
                cmd.Parameters.AddWithValue("@LedgerType", ledgerType)
                cmd.Parameters.AddWithValue("@RootTransactionID", rootTransactionID)

                cmd.Parameters.AddWithValue("@GroupID", operationGroupID)

                cmd.ExecuteNonQuery()

            End Using

        Next

    End Sub

    Public Function InsertCostLedger_Regular_Production(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    MIN(d.TargetStoreID) AS TargetStoreID,
    SUM(d.Quantity) AS InQty,
    SUM(d.Quantity*d.UnitCost) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID=d.ProductID
WHERE d.TransactionID=@TransactionID
  AND d.TargetStoreID IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM Master_Product p2
    LEFT JOIN Master_Product bp ON bp.ProductID = p2.BaseProductID
    WHERE p2.ProductID = d.ProductID
      AND (
            p2.StorageUnitID = @M3UnitID
         OR bp.StorageUnitID = @M3UnitID
      )
)
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID)
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(productRows)
                ' =========================
                ' DIAGNOSTIC
                ' =========================
                If productRows.Rows.Count = 0 Then
                    Return 0
                End If

            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence

        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))
            Dim inQty As Decimal = Convert.ToDecimal(prodRow("InQty"))
            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' =====================================================
            ' GLOBAL oldQty (كل المخازن كأنها مستودع واحد)
            ' =====================================================
            Dim oldQty As Decimal = 0D

            If oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If


            ' =====================================================
            ' LOCAL oldQty/newQty (المخزن الهدف فقط)
            ' =====================================================
            Dim localOldQty As Decimal = 0D

            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND StoreID=@StoreID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)

                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal = 0D
            If inQty <> 0D Then
                inAvg = inTotalCost / inQty
            End If

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);


", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)

                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)

                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                ' ✅ Local
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()
                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function


    Public Sub InsertCostLedger_OUT(
    transactionID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
        oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim userID As Integer

        Using cmdUser As New SqlCommand("
SELECT CreatedBy
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdUser.Parameters.AddWithValue("@TransactionID", transactionID)

            Dim v = cmdUser.ExecuteScalar()
            If v Is Nothing OrElse IsDBNull(v) Then
                Throw New Exception("TransactionHeader not found")
            End If

            userID = Convert.ToInt32(v)
        End Using

        Dim postDate As DateTime
        Dim operationTypeID As Integer

        Using cmdHead As New SqlCommand("
SELECT PostingDate, OperationTypeID
FROM Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)

            cmdHead.Parameters.AddWithValue("@T", transactionID)

            Dim dtHead As New DataTable()
            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(dtHead)
            End Using

            If dtHead.Rows.Count = 0 Then
                Throw New Exception("TransactionHeader not found")
            End If

            postDate = Convert.ToDateTime(dtHead.Rows(0)("PostingDate"))
            operationTypeID = Convert.ToInt32(dtHead.Rows(0)("OperationTypeID"))
        End Using

        Dim outRows As New DataTable()

        Dim getOutRowsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
    d.SourceStoreID AS StoreID,
    SUM(d.Quantity) AS OutQty,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
WHERE d.TransactionID = @TransactionID
  AND d.SourceStoreID IS NOT NULL
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID),
    d.SourceStoreID
HAVING SUM(d.Quantity) <> 0
"

        Using cmdGet As New SqlCommand(getOutRowsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(outRows)
            End Using
        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In outRows.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("StoreID"))
            Dim trxQty As Decimal = CDec(row("OutQty"))
            Dim outQty As Decimal = ConvertQtyToLedgerUnit(prodID, trxQty, con, tran)
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            ' منع التكرار لنفس العملية
            Using cmdExists As New SqlCommand("
SELECT TOP 1 1
FROM Inventory_CostLedger
WHERE TransactionID=@T
  AND ProductID=@P
  AND BaseProductID=@B
  AND StoreID=@S
  AND OutQty > 0
  AND IsActive=1
  AND IsReversed=0
", con, tran)

                cmdExists.Parameters.AddWithValue("@T", transactionID)
                cmdExists.Parameters.AddWithValue("@P", prodID)
                cmdExists.Parameters.AddWithValue("@B", baseProdID)
                cmdExists.Parameters.AddWithValue("@S", storeID)

                Dim exv = cmdExists.ExecuteScalar()
                If exv IsNot Nothing Then
                    Continue For
                End If
            End Using


            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' ==============================
            ' 1) OLD QTY (STORE) للفحص + لتعبئة LocalOldQty/LocalNewQty
            ' ==============================
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdBalStore As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND StoreID=@S
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdBalStore.Parameters.AddWithValue("@P", prodID)

                cmdBalStore.Parameters.AddWithValue("@S", storeID)

                Dim v = cmdBalStore.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal = ConvertBalanceQtyToLedgerUnit(prodID, localOldQtyBalance, con, tran)

            If localOldQty < outQty Then
                Throw New Exception("Stock would become negative Product=" & prodID.ToString() & " Store=" & storeID.ToString())
            End If

            Dim localNewQty As Decimal = localOldQty - outQty

            ' ==========================================
            ' 2) OLD QTY (GLOBAL) لحساب المتوسط عالمي
            ' ==========================================
            Dim oldQtyGlobalBalance As Decimal = 0D
            Using cmdBalGlobal As New SqlCommand("
SELECT TOP 1 NewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdBalGlobal.Parameters.AddWithValue("@P", prodID)
                cmdBalGlobal.Parameters.AddWithValue("@B", baseProdID)

                Dim v = cmdBalGlobal.ExecuteScalar()

                oldQtyGlobalBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim oldQtyGlobal As Decimal =
    ConvertBalanceQtyToLedgerUnit(prodID, oldQtyGlobalBalance, con, tran)


            Dim sqlInsert As String = "
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    StoreID,
    OperationTypeID,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    OutUnitCost,
    OutTotalCost,
    NewAvgCost,
    PostingDate,
    IsReversed,
    IsRevaluation,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
VALUES
(
    NEXT VALUE FOR Seq_CostLedgerID,
    @TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    @StoreID,
    @OperationTypeID,

    @LocalOldQty,
    @LocalNewQty,

    @OldQtyGlobal,
    0,
    @OutQty,
    @OldQtyGlobal - @OutQty,
    @OldAvgCost,
    0,
    0,
    @OldAvgCost,
    @OutQty * @OldAvgCost,
    @OldAvgCost,
    @PostingDate,
    0,
    0,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    1,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
)
"

            Using cmd As New SqlCommand(sqlInsert, con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                ' ✅ الأعمدة الجديدة (Local)
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobal)
                cmd.Parameters.AddWithValue("@OutQty", outQty)
                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@PostingDate", postDate)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()
            End Using
            seq += 1
        Next

    End Sub

    Public Sub InsertCostLedger_OUTtransfer(
    transactionID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
        oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim userID As Integer

        Using cmdUser As New SqlCommand("
SELECT CreatedBy
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdUser.Parameters.AddWithValue("@TransactionID", transactionID)

            Dim v = cmdUser.ExecuteScalar()
            If v Is Nothing OrElse IsDBNull(v) Then
                Throw New Exception("TransactionHeader not found")
            End If

            userID = Convert.ToInt32(v)
        End Using

        Dim postDate As DateTime
        Dim operationTypeID As Integer

        Using cmdHead As New SqlCommand("
SELECT PostingDate, OperationTypeID
FROM Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)

            cmdHead.Parameters.AddWithValue("@T", transactionID)

            Dim dtHead As New DataTable()
            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(dtHead)
            End Using

            If dtHead.Rows.Count = 0 Then
                Throw New Exception("TransactionHeader not found")
            End If

            postDate = Convert.ToDateTime(dtHead.Rows(0)("PostingDate"))
            operationTypeID = Convert.ToInt32(dtHead.Rows(0)("OperationTypeID"))
        End Using

        Dim outRows As New DataTable()

        Dim getOutRowsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
    d.SourceStoreID AS StoreID,

    SUM(
        CASE
            WHEN pu.UnitCode = 'M3' THEN d.Quantity

            WHEN pu.UnitCode <> 'M3'
                 AND bu.UnitCode = 'M3'
            THEN d.Quantity * (
                ISNULL(p.Length,0) * ISNULL(p.Width,0) * ISNULL(p.Height,0)
            ) / 1000000.0

            ELSE d.Quantity
        END
    ) AS OutQty,

    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p
    ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
LEFT JOIN Master_Unit pu
    ON pu.UnitID = d.UnitID
LEFT JOIN Master_Unit bu
    ON bu.UnitID = bp.StorageUnitID
WHERE d.TransactionID = @TransactionID
  AND d.SourceStoreID IS NOT NULL
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID),
    d.SourceStoreID
HAVING
    SUM(
        CASE
            WHEN pu.UnitCode = 'M3' THEN d.Quantity

            WHEN pu.UnitCode <> 'M3'
                 AND bu.UnitCode = 'M3'
            THEN d.Quantity * (
                ISNULL(p.Length,0) * ISNULL(p.Width,0) * ISNULL(p.Height,0)
            ) / 1000000.0

            ELSE d.Quantity
        END
    ) <> 0
"

        Using cmdGet As New SqlCommand(getOutRowsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(outRows)
            End Using
        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In outRows.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("StoreID"))
            Dim outQty As Decimal = CDec(row("OutQty"))
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            ' منع التكرار لنفس العملية
            Using cmdExists As New SqlCommand("
SELECT TOP 1 1
FROM Inventory_CostLedger
WHERE TransactionID=@T
  AND ProductID=@P
  AND BaseProductID=@B
  AND StoreID=@S
  AND OutQty > 0
  AND IsActive=1
  AND IsReversed=0
", con, tran)

                cmdExists.Parameters.AddWithValue("@T", transactionID)
                cmdExists.Parameters.AddWithValue("@P", prodID)
                cmdExists.Parameters.AddWithValue("@B", baseProdID)
                cmdExists.Parameters.AddWithValue("@S", storeID)

                Dim exv = cmdExists.ExecuteScalar()
                If exv IsNot Nothing Then
                    Continue For
                End If
            End Using


            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' ==============================
            ' 1) OLD QTY (STORE) للفحص + لتعبئة LocalOldQty/LocalNewQty
            ' ==============================
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdBalStore As New SqlCommand("
;WITH LastLedger AS
(
    SELECT
        LocalNewQty,
        ROW_NUMBER() OVER
        (
            PARTITION BY ProductID, StoreID
            ORDER BY LedgerID DESC
        ) rn
    FROM Inventory_CostLedger
    WHERE ProductID=@P
    AND StoreID=@S
    AND IsActive=1
    AND IsReversed=0
)
SELECT LocalNewQty
FROM LastLedger
WHERE rn = 1
", con, tran)

                cmdBalStore.Parameters.AddWithValue("@P", prodID)

                cmdBalStore.Parameters.AddWithValue("@S", storeID)

                Dim v = cmdBalStore.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal = localOldQtyBalance

            Dim tolerance As Decimal = 0.000001D

            If (localOldQty - outQty) < -tolerance Then
                Throw New Exception("Stock would become negative Product=" & prodID.ToString() & " Store=" & storeID.ToString())
            End If

            Dim localNewQty As Decimal = localOldQty - outQty
            If Math.Abs(localNewQty) < tolerance Then
                localNewQty = 0D
            End If

            ' ==========================================
            ' 2) OLD QTY (GLOBAL) لحساب المتوسط عالمي
            ' ==========================================
            Dim oldQtyGlobalBalance As Decimal = 0D
            Using cmdBalGlobal As New SqlCommand("
SELECT TOP 1 NewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdBalGlobal.Parameters.AddWithValue("@P", prodID)
                cmdBalGlobal.Parameters.AddWithValue("@B", baseProdID)

                Dim v = cmdBalGlobal.ExecuteScalar()

                oldQtyGlobalBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim oldQtyGlobal As Decimal = oldQtyGlobalBalance

            Dim sqlInsert As String = "
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    StoreID,
    OperationTypeID,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    OutUnitCost,
    OutTotalCost,
    NewAvgCost,
    PostingDate,
    IsReversed,
    IsRevaluation,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
VALUES
(
    NEXT VALUE FOR Seq_CostLedgerID,
    @TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    @StoreID,
    @OperationTypeID,

    @LocalOldQty,
    @LocalNewQty,

    @OldQtyGlobal,
    0,
    @OutQty,
    @OldQtyGlobal - @OutQty,
    @OldAvgCost,
    0,
    0,
    @OldAvgCost,
    @OutQty * @OldAvgCost,
    @OldAvgCost,
    @PostingDate,
    0,
    0,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    1,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
)
"

            Using cmd As New SqlCommand(sqlInsert, con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                ' ✅ الأعمدة الجديدة (Local)
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobal)
                cmd.Parameters.AddWithValue("@OutQty", outQty)
                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@PostingDate", postDate)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()
            End Using
            seq += 1
        Next

    End Sub




    Public Sub InsertCostLedger_OUT_lOADING(
    transactionID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
        oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim userID As Integer

        Using cmdUser As New SqlCommand("
SELECT CreatedBy
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdUser.Parameters.AddWithValue("@TransactionID", transactionID)

            Dim v = cmdUser.ExecuteScalar()
            If v Is Nothing OrElse IsDBNull(v) Then
                Throw New Exception("TransactionHeader not found")
            End If

            userID = Convert.ToInt32(v)
        End Using

        Dim postDate As DateTime
        Dim operationTypeID As Integer

        Using cmdHead As New SqlCommand("
SELECT PostingDate, OperationTypeID
FROM Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)

            cmdHead.Parameters.AddWithValue("@T", transactionID)

            Dim dtHead As New DataTable()
            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(dtHead)
            End Using

            If dtHead.Rows.Count = 0 Then
                Throw New Exception("TransactionHeader not found")
            End If

            postDate = Convert.ToDateTime(dtHead.Rows(0)("PostingDate"))
            operationTypeID = Convert.ToInt32(dtHead.Rows(0)("OperationTypeID"))
        End Using

        Dim outRows As New DataTable()

        Dim getOutRowsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
    d.SourceStoreID AS StoreID,
    SUM(d.Quantity) AS OutQty,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
WHERE d.TransactionID = @TransactionID
  AND d.SourceStoreID IS NOT NULL
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID),
    d.SourceStoreID
HAVING SUM(d.Quantity) <> 0
"

        Using cmdGet As New SqlCommand(getOutRowsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(outRows)
            End Using
        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In outRows.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("StoreID"))
            Dim trxQty As Decimal = CDec(row("OutQty"))
            Dim outQty As Decimal = ConvertQtyToLedgerUnit(prodID, trxQty, con, tran)
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            ' منع التكرار لنفس العملية
            Using cmdExists As New SqlCommand("
SELECT TOP 1 1
FROM Inventory_CostLedger
WHERE TransactionID=@T
  AND ProductID=@P
  AND BaseProductID=@B
  AND StoreID=@S
  AND OutQty > 0
  AND IsActive=1
  AND IsReversed=0
", con, tran)

                cmdExists.Parameters.AddWithValue("@T", transactionID)
                cmdExists.Parameters.AddWithValue("@P", prodID)
                cmdExists.Parameters.AddWithValue("@B", baseProdID)
                cmdExists.Parameters.AddWithValue("@S", storeID)

                Dim exv = cmdExists.ExecuteScalar()
                If exv IsNot Nothing Then
                    Continue For
                End If
            End Using


            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' ==============================
            ' 1) OLD QTY (STORE) للفحص + لتعبئة LocalOldQty/LocalNewQty
            ' ==============================
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdBalStore As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@P

AND StoreID=@S
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdBalStore.Parameters.AddWithValue("@P", prodID)
                cmdBalStore.Parameters.AddWithValue("@S", storeID)

                Dim v = cmdBalStore.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal = localOldQtyBalance

            If localOldQty < outQty Then
                Throw New Exception("Stock would become negative Product=" & prodID.ToString() & " Store=" & storeID.ToString())
            End If

            Dim localNewQty As Decimal = localOldQty - outQty

            ' ==========================================
            ' 2) OLD QTY (GLOBAL) لحساب المتوسط عالمي
            ' ==========================================
            Dim oldQtyGlobalBalance As Decimal = 0D
            Using cmdBalGlobal As New SqlCommand("
SELECT TOP 1 NewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdBalGlobal.Parameters.AddWithValue("@P", prodID)
                cmdBalGlobal.Parameters.AddWithValue("@B", baseProdID)

                Dim v = cmdBalGlobal.ExecuteScalar()

                oldQtyGlobalBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim oldQtyGlobal As Decimal = oldQtyGlobalBalance

            Dim sqlInsert As String = "
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    StoreID,
    OperationTypeID,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    OutUnitCost,
    OutTotalCost,
    NewAvgCost,
    PostingDate,
    IsReversed,
    IsRevaluation,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
VALUES
(
    NEXT VALUE FOR Seq_CostLedgerID,
    @TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    @StoreID,
    @OperationTypeID,

    @LocalOldQty,
    @LocalNewQty,

    @OldQtyGlobal,
    0,
    @OutQty,
    @OldQtyGlobal - @OutQty,
    @OldAvgCost,
    0,
    0,
    @OldAvgCost,
    @OutQty * @OldAvgCost,
    @OldAvgCost,
    @PostingDate,
    0,
    0,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    1,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
)
"

            Using cmd As New SqlCommand(sqlInsert, con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                ' ✅ الأعمدة الجديدة (Local)
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobal)
                cmd.Parameters.AddWithValue("@OutQty", outQty)
                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@PostingDate", postDate)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()
            End Using
            seq += 1
        Next

    End Sub




    Public Sub ApplyInventoryOut(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        '=====================================================
        ' خصم الكميات من المستودع المصدر فقط
        '=====================================================

        Dim sqlUpdateQty As String = "

;WITH src AS
(
SELECT
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
d.SourceStoreID AS StoreID,
SUM(d.Quantity) AS Quantity
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
WHERE d.TransactionID = @TransactionID
AND d.SourceStoreID IS NOT NULL
GROUP BY
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID),
d.SourceStoreID
)

UPDATE b
SET
b.QtyOnHand = b.QtyOnHand - src.Quantity,
b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN src
ON src.ProductID = b.ProductID
AND src.BaseProductID = b.BaseProductID
AND src.StoreID = b.StoreID
"

        Dim sqlUpdateMovement As String = "

;WITH LastLedger AS
(
SELECT
cl.ProductID,
cl.BaseProductID,
cl.StoreID,
MAX(cl.LedgerID) AS LastLedgerID
FROM Inventory_CostLedger cl
WHERE cl.TransactionID = @TransactionID
AND cl.OutQty > 0
AND cl.IsReversed = 0
GROUP BY
cl.ProductID,
cl.BaseProductID,
cl.StoreID
)

UPDATE b
SET
b.LastMovementLedgerID = ll.LastLedgerID,
b.LastMovementDate = SYSDATETIME(),
b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN LastLedger ll
ON ll.ProductID = b.ProductID
AND ll.BaseProductID = b.BaseProductID
AND ll.StoreID = b.StoreID
"

        Try

            Using cmd As New SqlCommand(sqlUpdateQty, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using

            Using cmd As New SqlCommand(sqlUpdateMovement, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception

            MessageBox.Show("🔴 [ApplyInventoryOut] فشل التنفيذ:" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            Throw

        End Try

    End Sub
    Public Sub ApplyInventoryIn(
    transactionID As Integer,
    m3UnitID As Integer,
    ledgerID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        ' =====================================================
        ' تحديث رصيد المخزن الهدف فقط
        ' =====================================================

        Dim sqlMerge As String = "

;WITH src AS
(
    SELECT
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
        d.TargetStoreID AS StoreID,
        SUM(d.Quantity) AS Quantity
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
      AND d.TargetStoreID IS NOT NULL
    GROUP BY
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID),
        d.TargetStoreID
)

MERGE Inventory_Balance AS tgt
USING src
ON  tgt.ProductID      = src.ProductID
AND tgt.BaseProductID  = src.BaseProductID
AND tgt.StoreID        = src.StoreID

WHEN MATCHED THEN
UPDATE SET
    tgt.QtyOnHand     = tgt.QtyOnHand + src.Quantity,
    tgt.LastUpdatedAt = SYSDATETIME()

WHEN NOT MATCHED THEN
INSERT
(
    ProductID,
    BaseProductID,
    StoreID,
    QtyOnHand,
    CreatedAt,
    LastUpdatedAt
)
VALUES
(
    src.ProductID,
    src.BaseProductID,
    src.StoreID,
    src.Quantity,
    SYSDATETIME(),
    SYSDATETIME()
);
"

        Try

            Using cmd As New SqlCommand(sqlMerge, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using


            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN)
            ' ملاحظة: نربط بالحركة عن طريق SourceDetailID (DetailID) لتفادي تعميم أول LedgerID على الجميع
            ' =====================================================

            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN) - مثل أسلوب OUT
            ' =====================================================

            Dim sqlUpdateMovement As String = "

;WITH LastLedger AS
(
    SELECT
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID,
        MAX(cl.LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger cl
    WHERE cl.TransactionID = @TransactionID
      AND cl.InQty > 0
      AND cl.IsReversed = 0
    GROUP BY
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID
)
UPDATE b
SET
    b.LastMovementLedgerID = ll.LastLedgerID,
    b.LastMovementDate = SYSDATETIME(),
    b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN LastLedger ll
  ON ll.ProductID = b.ProductID
 AND ll.BaseProductID = b.BaseProductID
 AND ll.StoreID = b.StoreID;
"
            Using cmdFix As New SqlCommand(sqlUpdateMovement, con, tran)
                cmdFix.Parameters.AddWithValue("@TransactionID", transactionID)
                cmdFix.ExecuteNonQuery()
            End Using


        Catch ex As Exception
            MessageBox.Show("🔴 [ApplyInventoryIn] فشل التنفيذ:" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            Throw
        End Try

    End Sub





    Public Function GetReturnUnitCost(
    returnDocumentDetailID As Integer,
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Dim cost As Decimal = 0D

        Using cmd As New SqlCommand("
SELECT TOP 1 cl.OutUnitCost
FROM Inventory_DocumentDetails rd
JOIN Inventory_DocumentDetails inv
    ON inv.DetailID = rd.SourceDocumentDetailID
JOIN Logistics_LoadingOrderDetail lod
    ON lod.LoadingOrderDetailID = inv.SourceLoadingOrderDetailID
JOIN Inventory_TransactionHeader th
    ON th.SourceDocumentID = lod.LOID
JOIN Inventory_CostLedger cl
    ON cl.TransactionID = th.TransactionID
WHERE rd.DetailID = @ReturnDetailID
AND cl.ProductID = @ProductID
AND cl.OutQty > 0
AND cl.IsActive = 1
AND cl.IsReversed = 0
ORDER BY cl.LedgerID DESC
", con, tran)

            cmd.Parameters.AddWithValue("@ReturnDetailID", returnDocumentDetailID)
            cmd.Parameters.AddWithValue("@ProductID", productID)

            Dim v = cmd.ExecuteScalar()

            If v IsNot Nothing AndAlso Not IsDBNull(v) Then
                cost = Convert.ToDecimal(v)
            End If

        End Using

        Return cost

    End Function





    Public Function InsertCostLedger_RegularPurchase(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    MIN(d.TargetStoreID) AS TargetStoreID,
    SUM(d.Quantity) AS InQty,
    SUM(d.Quantity*d.UnitCost) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID=d.ProductID
WHERE d.TransactionID=@TransactionID
  AND d.TargetStoreID IS NOT NULL
AND p.StorageUnitID <> @M3UnitID
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID)
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(productRows)
                ' =========================
                ' DIAGNOSTIC
                ' =========================

            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence

        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))
            Dim inQty As Decimal = Convert.ToDecimal(prodRow("InQty"))
            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' =====================================================
            ' GLOBAL oldQty (كل المخازن كأنها مستودع واحد)
            ' =====================================================
            Dim oldQty As Decimal = 0D

            If oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If


            ' =====================================================
            ' LOCAL oldQty/newQty (المخزن الهدف فقط)
            ' =====================================================
            Dim localOldQty As Decimal = 0D

            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND StoreID=@StoreID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)

                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal = 0D
            If inQty <> 0D Then
                inAvg = inTotalCost / inQty
            End If

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);


", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)

                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)

                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                ' ✅ Local
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()
                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function




    Public Function InsertCostLedger_Regular(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    MIN(d.TargetStoreID) AS TargetStoreID,
    SUM(d.Quantity) AS InQty,
    SUM(d.Quantity*d.UnitCost) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID=d.ProductID
WHERE d.TransactionID=@TransactionID
  AND d.TargetStoreID IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM Master_Product p2
    LEFT JOIN Master_Product bp ON bp.ProductID = p2.BaseProductID
    WHERE p2.ProductID = d.ProductID
      AND (
            p2.StorageUnitID = @M3UnitID
         OR bp.StorageUnitID = @M3UnitID
      )
)
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID)
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(productRows)
                ' =========================
                ' DIAGNOSTIC
                ' =========================

            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence

        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))
            Dim inQty As Decimal = Convert.ToDecimal(prodRow("InQty"))
            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' =====================================================
            ' GLOBAL oldQty (كل المخازن كأنها مستودع واحد)
            ' =====================================================
            Dim oldQty As Decimal = 0D

            If oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If


            ' =====================================================
            ' LOCAL oldQty/newQty (المخزن الهدف فقط)
            ' =====================================================
            Dim localOldQty As Decimal = 0D

            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND StoreID=@StoreID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)

                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal = 0D
            If inQty <> 0D Then
                inAvg = inTotalCost / inQty
            End If

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);


", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)

                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)

                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                ' ✅ Local
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()
                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function
    Public Function InsertCostLedger_M3(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    d.TargetStoreID,
    SUM(d.Quantity) AS TrxQty,
    SUM(d.CostAmount) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID = @TransactionID
  AND d.TargetStoreID IS NOT NULL
  AND (
        p.StorageUnitID = @M3UnitID
        OR bp.StorageUnitID = @M3UnitID
      )
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID),
    d.TargetStoreID
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(productRows)








            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence
        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))

            Dim trxQty As Decimal = Convert.ToDecimal(prodRow("TrxQty"))
            Dim inQty As Decimal = ConvertProductQtyToLedgerQty(prodID, trxQty, m3UnitID, con, tran)



            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' GLOBAL oldQty من Balance لكن نحوله إلى Ledger Unit
            Dim oldQtyBalance As Decimal = 0D
            Dim oldQty As Decimal = 0D
            If oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If
            ConvertProductQtyToLedgerQty(prodID, oldQtyBalance, m3UnitID, con, tran)

            ' LOCAL oldQty من Balance لكن نحوله إلى Ledger Unit
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND StoreID=@StoreID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)



                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal =
            ConvertProductQtyToLedgerQty(prodID, localOldQtyBalance, m3UnitID, con, tran)

            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal = 0D
            If inQty <> 0D Then
                inAvg = inTotalCost / inQty
            End If

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);

", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()

                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function
    Public Sub InsertCostLedger_IN_TRN(
    transactionID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
        oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim operationTypeID As Integer
        Dim postingDate As DateTime
        Dim createdBy As Integer

        Using cmd As New SqlCommand("
SELECT
    OperationTypeID,
    PostingDate,
    CreatedBy
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)

            Using rdr = cmd.ExecuteReader()

                If rdr.Read() Then
                    operationTypeID = rdr.GetInt32(0)
                    postingDate = rdr.GetDateTime(1)
                    createdBy = rdr.GetInt32(2)
                Else
                    Throw New Exception("TransactionHeader not found")
                End If

            End Using

        End Using

        Dim details As New DataTable()

        Using cmd As New SqlCommand("
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
    d.TargetStoreID,
    d.SourceStoreID,

    SUM(
        CASE
            WHEN pu.UnitCode = 'M3' THEN d.Quantity

            WHEN pu.UnitCode <> 'M3'
                 AND bu.UnitCode = 'M3'
            THEN d.Quantity * (
                ISNULL(p.Length,0) * ISNULL(p.Width,0) * ISNULL(p.Height,0)
            ) / 1000000.0

            ELSE d.Quantity
        END
    ) AS Qty,

    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p
    ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
LEFT JOIN Master_Unit pu
    ON pu.UnitID = d.UnitID
LEFT JOIN Master_Unit bu
    ON bu.UnitID = bp.StorageUnitID
WHERE d.TransactionID = @TransactionID
  AND d.TargetStoreID IS NOT NULL
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID),
    d.TargetStoreID,
    d.SourceStoreID
", con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(details)
            End Using

        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In details.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim targetStoreID As Integer = CInt(row("TargetStoreID"))
            Dim sourceStoreID As Integer = CInt(row("SourceStoreID"))
            Dim qty As Decimal = CDec(row("Qty"))
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            Dim sourceLedgerID As Long
            Dim rootLedgerID As Long
            Dim outCost As Decimal

            '====================================================
            ' الحصول على Ledger OUT المصدر لنفس التحويل
            '====================================================

            Using cmd As New SqlCommand("
SELECT TOP 1
    LedgerID,
    ISNULL(RootLedgerID,LedgerID),
    OutUnitCost
FROM Inventory_CostLedger
WHERE TransactionID=@TransactionID
AND OperationGroupID=@GroupID
AND ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND StoreID=@SourceStoreID
AND OutQty>0
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@GroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)

                Using rdr = cmd.ExecuteReader()

                    If rdr.Read() Then

                        sourceLedgerID = CLng(rdr.GetValue(0))
                        rootLedgerID = CLng(rdr.GetValue(1))
                        outCost = Convert.ToDecimal(rdr.GetValue(2))

                    Else

                        Throw New Exception(
                        "Transfer OUT ledger not found. " &
                        "TransactionID=" & transactionID &
                        " Product=" & prodID &
                        " Store=" & sourceStoreID)

                    End If

                End Using

            End Using

            '====================================================
            ' Local Qty في المخزن الهدف (LocalOldQty/LocalNewQty)
            '====================================================

            Dim localOldQty As Decimal = 0D

            Using cmd As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND StoreID=@S
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@P", prodID)
                cmd.Parameters.AddWithValue("@B", baseProdID)
                cmd.Parameters.AddWithValue("@S", targetStoreID)

                Dim v = cmd.ExecuteScalar()
                localOldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim localNewQty As Decimal = localOldQty + qty

            '====================================================
            ' Global Qty (OldQty/NewQty) — لازم تكون SUM لكل المخازن دائمًا
            '====================================================

            Dim oldQtyGlobal As Decimal = 0D

            Using cmd As New SqlCommand("
SELECT TOP 1 NewQty
FROM Inventory_CostLedger
WHERE ProductID=@P
AND BaseProductID=@B
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@P", prodID)
                cmd.Parameters.AddWithValue("@B", baseProdID)

                Dim v = cmd.ExecuteScalar()
                oldQtyGlobal = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            '====================================================
            ' إنشاء Ledger جديد
            '====================================================


            '====================================================
            ' إدراج Transfer IN
            '====================================================

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
LedgerID,
TransactionID,
SourceDetailID,
ProductID,
BaseProductID,
IsReversed,
IsRevaluation,
StoreID,
OperationTypeID,
PostingDate,

LocalOldQty,
LocalNewQty,

OldQty,
InQty,
OutQty,
NewQty,
OldAvgCost,
InUnitCost,
InTotalCost,
OutUnitCost,
OutTotalCost,
NewAvgCost,
SourceLedgerID,
PrevLedgerID,
RootLedgerID,
DependsOnLedgerID,
CostSourceType,
RootTransactionID,
CreatedBy,
CreatedAt,
OperationGroupID,
GroupSeq,
ScopeKeyType,
ScopeKeyID,
LedgerSequence,
IsActive
)
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
@SourceDetailID,
@ProductID,
@BaseProductID,
0,
0,
@StoreID,
@OperationTypeID,
@PostingDate,

@LocalOldQty,
@LocalNewQty,

@OldQtyGlobal,
@InQty,
0,
@NewQtyGlobal,
@UnitCost,
@UnitCost,
@TotalCost,
0,
0,
@UnitCost,
@SourceLedgerID,
@SourceLedgerID,
@RootLedgerID,
@SourceLedgerID,
@OperationTypeID,
@TransactionID,
@CreatedBy,
SYSDATETIME(),
@OperationGroupID,
2,
'PRODUCT',
@ProductID,
@LedgerSequence,
1
)
", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                ' ✅ Local
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobal)
                cmd.Parameters.AddWithValue("@InQty", qty)
                cmd.Parameters.AddWithValue("@NewQtyGlobal", oldQtyGlobal + qty)

                cmd.Parameters.AddWithValue("@UnitCost", outCost)
                cmd.Parameters.AddWithValue("@TotalCost", qty * outCost)

                cmd.Parameters.AddWithValue("@SourceLedgerID", sourceLedgerID)
                cmd.Parameters.AddWithValue("@RootLedgerID", rootLedgerID)

                cmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()

            End Using
            seq += 1
        Next

    End Sub
    Public Sub FinalizeLedgerMetadata(operationGroupID As Guid,
                                  con As SqlConnection,
                                  tran As SqlTransaction)

        '===============================
        ' 1) LedgerType + CostSourceType
        '===============================
        Dim sqlType As String = "

UPDATE L
SET 
    L.LedgerType =
        CASE O.OperationCode
            WHEN 'PUR' THEN 4
            WHEN 'TRN' THEN 1
            WHEN 'SAL' THEN 5
            WHEN 'SRT' THEN 5
            WHEN 'SCR' THEN 9
            WHEN 'LOD' THEN 2
            WHEN 'PRO' THEN 
                CASE 
                    WHEN L.OutQty > 0 THEN 2
                    ELSE 3
                END
            WHEN 'CUT' THEN 
                CASE 
                    WHEN L.OutQty > 0 THEN 2
                    ELSE 3
                END
        END,
    L.CostSourceType = H.OperationTypeID

FROM Inventory_CostLedger L
JOIN Inventory_TransactionHeader H
    ON H.TransactionID = L.TransactionID
JOIN Workflow_OperationType O
    ON O.OperationTypeID = H.OperationTypeID

WHERE L.OperationGroupID = @OperationGroupID
AND L.LedgerType IS NULL
"

        Using cmd As New SqlCommand(sqlType, con, tran)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
            cmd.ExecuteNonQuery()
        End Using


        '===============================
        ' 2) CalcHash
        '===============================
        Dim sqlCalcHash As String = "
UPDATE L
SET CalcHash = HASHBYTES(
    'SHA2_256',
    CONCAT(
        -- Keys
        ISNULL(CONVERT(varchar(20), L.ProductID), ''), '|',
        ISNULL(CONVERT(varchar(20), L.BaseProductID), ''), '|',
        ISNULL(CONVERT(varchar(20), L.StoreID), ''), '|',

        -- Classification
        ISNULL(CONVERT(varchar(20), H.OperationTypeID), ''), '|',
        ISNULL(CONVERT(varchar(20), L.LedgerType), ''), '|',

        -- Time (deterministic ISO)
        ISNULL(CONVERT(varchar(33), L.PostingDate, 126), ''), '|',

        -- Quantities (fixed scale)
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.OldQty)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.InQty)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.OutQty)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.NewQty)), ''), '|',

        -- Costs
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.OldAvgCost)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.InTotalCost)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.OutTotalCost)), ''), '|',
        ISNULL(CONVERT(varchar(50), CONVERT(decimal(18,6), L.NewAvgCost)), '')
    )
)
FROM Inventory_CostLedger L
JOIN Inventory_TransactionHeader H
  ON H.TransactionID = L.TransactionID
WHERE L.OperationGroupID = @OperationGroupID
  AND L.CalcHash IS NULL
"

        Using cmd As New SqlCommand(sqlCalcHash, con, tran)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
            cmd.ExecuteNonQuery()
        End Using


        '===============================
        ' 3) ChainHash
        '===============================
        Dim sqlChainHash As String = "
UPDATE L
SET ChainHash = HASHBYTES(
    'SHA2_256',
    ISNULL(P.ChainHash, 0x) + L.CalcHash
)
FROM Inventory_CostLedger L
LEFT JOIN Inventory_CostLedger P
  ON P.LedgerID = L.PrevLedgerID
WHERE L.OperationGroupID = @OperationGroupID
  AND L.ChainHash IS NULL
  AND L.CalcHash IS NOT NULL
"

        Using cmd As New SqlCommand(sqlChainHash, con, tran)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Private Function ConvertProductQtyToLedgerQty(
    productID As Integer,
    qty As Decimal,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Using cmd As New SqlCommand("
SELECT
    p.StorageUnitID AS ProductUnitID,
    bp.StorageUnitID AS BaseUnitID,
    ISNULL(p.Length,0) AS Length,
    ISNULL(p.Width,0)  AS Width,
    ISNULL(p.Height,0) AS Height
FROM Master_Product p
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
WHERE p.ProductID = @ProductID
", con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)

            Using rd = cmd.ExecuteReader()
                If Not rd.Read() Then
                    Throw New Exception("Product not found. ProductID=" & productID.ToString())
                End If

                Dim productUnitID As Integer =
                If(IsDBNull(rd("ProductUnitID")), 0, Convert.ToInt32(rd("ProductUnitID")))

                Dim baseUnitID As Integer =
                If(IsDBNull(rd("BaseUnitID")), 0, Convert.ToInt32(rd("BaseUnitID")))

                Dim length As Decimal = If(IsDBNull(rd("Length")), 0D, Convert.ToDecimal(rd("Length")))
                Dim width As Decimal = If(IsDBNull(rd("Width")), 0D, Convert.ToDecimal(rd("Width")))
                Dim height As Decimal = If(IsDBNull(rd("Height")), 0D, Convert.ToDecimal(rd("Height")))

                ' إذا Ledger ليس M3 فلا تحويل
                If productUnitID <> m3UnitID AndAlso baseUnitID <> m3UnitID Then
                    Return qty
                End If

                ' إذا وحدة الصنف نفسها M3 فلا تحويل
                If productUnitID = m3UnitID Then
                    Return qty
                End If

                ' إذا الصنف ليس M3 لكن الأب M3 => نحول من الحبة إلى الحجم
                If length <= 0D OrElse width <= 0D OrElse height <= 0D Then
                    Throw New Exception("Invalid dimensions for ProductID=" & productID.ToString())
                End If

                Dim pieceVolume As Decimal =
                (length / 100D) * (width / 100D) * (height / 100D)

                Return qty * pieceVolume
            End Using
        End Using

    End Function

    Private Function ConvertQtyToLedgerUnit(
    productID As Integer,
    qty As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Dim productUnit As String = ""
        Dim baseUnit As String = ""
        Dim length As Decimal = 0D
        Dim width As Decimal = 0D
        Dim height As Decimal = 0D

        Using cmd As New SqlCommand("
SELECT
    pu.UnitCode AS ProductUnit,
    bu.UnitCode AS BaseUnit,
    ISNULL(p.Length,0) AS Length,
    ISNULL(p.Width,0)  AS Width,
    ISNULL(p.Height,0) AS Height
FROM Master_Product p
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
LEFT JOIN Master_Unit pu
    ON pu.UnitID = p.StorageUnitID
LEFT JOIN Master_Unit bu
    ON bu.UnitID = bp.StorageUnitID
WHERE p.ProductID = @ProductID
", con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)

            Using rd = cmd.ExecuteReader()
                If Not rd.Read() Then
                    Throw New Exception("Product not found. ProductID=" & productID.ToString())
                End If

                productUnit = If(rd("ProductUnit") Is DBNull.Value, "", rd("ProductUnit").ToString().Trim().ToUpper())
                baseUnit = If(rd("BaseUnit") Is DBNull.Value, "", rd("BaseUnit").ToString().Trim().ToUpper())
                length = Convert.ToDecimal(rd("Length"))
                width = Convert.ToDecimal(rd("Width"))
                height = Convert.ToDecimal(rd("Height"))
            End Using
        End Using

        ' الحالة 1: الصنف نفسه M3
        If productUnit = "M3" Then
            Return qty
        End If

        ' الحالة 2: الصنف ليس M3 لكن الأب M3 => Ledger يخزن بالمتر المكعب
        If productUnit <> "M3" AndAlso baseUnit = "M3" Then

            If length <= 0D OrElse width <= 0D OrElse height <= 0D Then
                Throw New Exception("Invalid dimensions for ProductID=" & productID.ToString())
            End If

            Dim pieceVolumeM3 As Decimal =
            (length / 100D) * (width / 100D) * (height / 100D)

            Return qty * pieceVolumeM3
        End If

        ' الحالة 3: لا الصنف ولا الأب M3 => يبقى بوحدة الصنف
        Return qty

    End Function

    Private Function ConvertBalanceQtyToLedgerUnit(
    productID As Integer,
    balanceQty As Decimal,
    con As SqlConnection,
    tran As SqlTransaction
) As Decimal

        Return ConvertQtyToLedgerUnit(productID, balanceQty, con, tran)

    End Function




    Public Sub InsertLoadingLedgerLinks_LOD(
    transactionID As Integer,
    operationGroupID As Guid,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        Const LINKTYPE_LOADING_OUT As Short = 20

        ' نحتاج PostingDate من TransactionHeader
        Dim postingDate As DateTime
        Using cmd As New SqlCommand("
SELECT PostingDate
FROM dbo.Inventory_TransactionHeader
WHERE TransactionID = @T
", con, tran)
            cmd.Parameters.AddWithValue("@T", transactionID)
            postingDate = CDate(cmd.ExecuteScalar())
        End Using

        ' إدراج Link لكل سطر OUT في CostLedger مربوط بسطر TransactionDetails
        ' ملاحظة: نفترض Ledger OUT للـ DetailID موجود (InsertCostLedger_OUT تم قبلها)
        Using cmd As New SqlCommand("
INSERT INTO dbo.Inventory_CostLedgerLink
(
    TargetLedgerID,
    SourceLedgerID,
    LinkType,
    FlowQty,
    FlowUnitCost,
    SourceTransactionDetailID,
    TargetTransactionDetailID,
    SourceStoreID,
    TargetStoreID,
    ProductID,
    BaseProductID,
    PostingDate,
    OperationGroupID,
    GroupSeq,
    IsActive,
    LinkHash,
    CreatedAt,
    CreatedBy,
    FlowTotalCost,
    LinkReason,
    RootLedgerID,
    LinkDirection
)
SELECT
    NULL AS TargetLedgerID,
    cl.LedgerID AS SourceLedgerID,
    @LinkType AS LinkType,
    cl.OutQty AS FlowQty,
    cl.OutUnitCost AS FlowUnitCost,
    d.DetailID AS SourceTransactionDetailID,
    NULL AS TargetTransactionDetailID,
    d.SourceStoreID,
    d.TargetStoreID,
    d.ProductID,
    ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
    @PostingDate,
    @GroupID,
    1 AS GroupSeq,
    1 AS IsActive,

    -- LinkHash: لو عندكم منطق محدد استبدله. هنا Hash بسيط deterministic
    CONVERT(varchar(64), HASHBYTES('SHA2_256',
        CONCAT('LOD|', @T, '|', d.DetailID, '|', cl.LedgerID, '|', cl.OutQty, '|', cl.OutUnitCost)
    ), 2) AS LinkHash,

    SYSDATETIME() AS CreatedAt,
    @UserID AS CreatedBy,
    cl.OutTotalCost AS FlowTotalCost,
    N'LOD OUT posting' AS LinkReason,
    ISNULL(cl.RootLedgerID, cl.LedgerID) AS RootLedgerID,
    'OUT' AS LinkDirection
FROM dbo.Inventory_CostLedger cl
INNER JOIN dbo.Inventory_TransactionDetails d
    ON d.DetailID = cl.SourceDetailID
INNER JOIN dbo.Master_Product p
    ON p.ProductID = d.ProductID
WHERE cl.TransactionID = @T
  AND cl.OutQty > 0
  AND cl.IsActive = 1
  AND cl.IsReversed = 0

  -- منع التكرار (Idempotent)
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Inventory_CostLedgerLink l
      WHERE l.SourceLedgerID = cl.LedgerID
        AND l.SourceTransactionDetailID = d.DetailID
        AND l.LinkType = @LinkType
        AND l.IsActive = 1
  );
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)
            cmd.Parameters.AddWithValue("@LinkType", LINKTYPE_LOADING_OUT)
            cmd.Parameters.AddWithValue("@PostingDate", postingDate)
            cmd.Parameters.AddWithValue("@GroupID", operationGroupID)
            cmd.Parameters.AddWithValue("@UserID", userID)

            cmd.ExecuteNonQuery()
        End Using

    End Sub



    Public Sub InsertCorrectionRecord(
        correctionCode As String,
        detail As TransactionDetailDTO,
        oldAvgData As AvgCostData,
        costLedgerID As Integer,
        userID As Integer,
        reason As String,
        con As SqlConnection,
        tran As SqlTransaction
    )
        ' جلب الكمية الحالية بعد العكس
        Dim currentQty As Decimal = 0
        Dim sql = "SELECT QtyOnHand FROM Inventory_Balance WHERE ProductID = @ProductID AND StoreID = @StoreID"
        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", detail.ProductID)
        cmd.Parameters.AddWithValue("@StoreID", detail.StoreID)
        currentQty = Convert.ToDecimal(cmd.ExecuteScalar())

        ' حساب الفروق
        Dim qtyDifference = currentQty - (currentQty + detail.Quantity)
        Dim costDifference = (currentQty * oldAvgData.OldAvgCost) - ((currentQty + detail.Quantity) * detail.UnitCost)

        sql = "
        INSERT INTO Inventory_AVG_QTY_Correction
        (CorrectionCode, TransactionID, SourceDocumentID, ProductID, StoreID, BaseProductID,
         OldQty, OldAvgCost, OldAvgCostPerM3,
         CorrectedQty, CorrectedAvgCost, CorrectedAvgCostPerM3,
         QtyDifference, CostDifference,
         OperationTypeID, SourceOperationTypeID, SourceUnitID,
         Reason, CreatedBy, CostLedgerID, IsPosted, CreatedAt)
        VALUES
        (@CorrectionCode, @TransactionID, @SourceDocumentID, @ProductID, @StoreID, @BaseProductID,
         @OldQty, @OldAvgCost, @OldAvgCostPerM3,
         @CorrectedQty, @CorrectedAvgCost, @CorrectedAvgCostPerM3,
         @QtyDifference, @CostDifference,
         @OperationTypeID, @SourceOperationTypeID, @SourceUnitID,
         @Reason, @CreatedBy, @CostLedgerID, 1, GETDATE())"

        cmd = New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@CorrectionCode", correctionCode)
        cmd.Parameters.AddWithValue("@TransactionID", detail.TransactionID)
        cmd.Parameters.AddWithValue("@SourceDocumentID", detail.SourceDocumentID)
        cmd.Parameters.AddWithValue("@ProductID", detail.ProductID)
        cmd.Parameters.AddWithValue("@StoreID", If(detail.StoreID > 0, detail.StoreID, DBNull.Value))
        cmd.Parameters.AddWithValue("@BaseProductID", If(oldAvgData.BaseProductID > 0, oldAvgData.BaseProductID, DBNull.Value))

        ' البيانات قبل التصحيح (الخاطئة)
        cmd.Parameters.AddWithValue("@OldQty", currentQty + detail.Quantity)
        cmd.Parameters.AddWithValue("@OldAvgCost", detail.UnitCost)
        cmd.Parameters.AddWithValue("@OldAvgCostPerM3", If(oldAvgData.BaseProductID > 0, oldAvgData.OldAvgCostPerM3, DBNull.Value))

        ' البيانات بعد التصحيح (الصحيحة)
        cmd.Parameters.AddWithValue("@CorrectedQty", currentQty)
        cmd.Parameters.AddWithValue("@CorrectedAvgCost", oldAvgData.OldAvgCost)
        cmd.Parameters.AddWithValue("@CorrectedAvgCostPerM3", If(oldAvgData.BaseProductID > 0, oldAvgData.OldAvgCostPerM3, DBNull.Value))

        ' الفروق
        cmd.Parameters.AddWithValue("@QtyDifference", qtyDifference)
        cmd.Parameters.AddWithValue("@CostDifference", costDifference)

        cmd.Parameters.AddWithValue("@OperationTypeID", 2) ' COR
        cmd.Parameters.AddWithValue("@SourceOperationTypeID", detail.OperationTypeID)
        cmd.Parameters.AddWithValue("@SourceUnitID", detail.StorageUnitID)

        cmd.Parameters.AddWithValue("@Reason", If(String.IsNullOrEmpty(reason), DBNull.Value, reason))
        cmd.Parameters.AddWithValue("@CreatedBy", userID)
        cmd.Parameters.AddWithValue("@CostLedgerID", costLedgerID)

        cmd.ExecuteNonQuery()
    End Sub

    Public Sub InsertLedgerLink(
    sourceLedgerID As Long,
    targetLedgerID As Long,
    linkType As Short,
    qty As Decimal,
    unitCost As Decimal,
    transactionID As Integer,   ' keep for now (used in hash calc / existing call sites)
    storeSource As Integer,
    storeTarget As Integer,
    productID As Integer,
    baseProductID As Integer?,  ' changed from Integer -> Integer?
    postingDate As DateTime,
    operationGroupID As Guid,
    groupSeq As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim linkDirection As Integer = If(storeSource = storeTarget, 1, 2)

        Dim linkHash As Byte() =
        CalculateLinkHash(
            sourceLedgerID,
            targetLedgerID,
            linkType,
            qty,
            unitCost,
            operationGroupID)

        ' RootLedgerID comes from source ledger (existing behavior)
        Dim rootLedgerID As Long
        Using cmdRoot As New SqlCommand("
SELECT RootLedgerID
FROM Inventory_CostLedger
WHERE LedgerID = @L", con, tran)

            cmdRoot.Parameters.Add("@L", SqlDbType.BigInt).Value = sourceLedgerID

            Dim obj = cmdRoot.ExecuteScalar()
            If obj Is DBNull.Value OrElse obj Is Nothing Then
                rootLedgerID = sourceLedgerID
            Else
                rootLedgerID = CLng(obj)
            End If
        End Using

        ' NEW: derive DetailIDs from ledgers (you confirmed this is always valid)
        Dim sourceTransactionDetailID As Integer
        Dim targetTransactionDetailID As Integer

        Using cmdDet As New SqlCommand("
SELECT
    (SELECT SourceDetailID FROM Inventory_CostLedger WHERE LedgerID=@SrcL) AS SrcDetailID,
    (SELECT SourceDetailID FROM Inventory_CostLedger WHERE LedgerID=@TgtL) AS TgtDetailID
", con, tran)
            cmdDet.Parameters.Add("@SrcL", SqlDbType.BigInt).Value = sourceLedgerID
            cmdDet.Parameters.Add("@TgtL", SqlDbType.BigInt).Value = targetLedgerID

            Using rdr = cmdDet.ExecuteReader()
                If Not rdr.Read() Then
                    Throw New Exception("Failed to derive SourceDetailID/TargetDetailID for ledger link. SrcLedger=" &
                                    sourceLedgerID.ToString() & ", TgtLedger=" & targetLedgerID.ToString())
                End If

                If rdr("SrcDetailID") Is Nothing OrElse rdr("SrcDetailID") Is DBNull.Value Then
                    Throw New Exception("Source ledger has NULL SourceDetailID. LedgerID=" & sourceLedgerID.ToString())
                End If
                If rdr("TgtDetailID") Is Nothing OrElse rdr("TgtDetailID") Is DBNull.Value Then
                    Throw New Exception("Target ledger has NULL SourceDetailID. LedgerID=" & targetLedgerID.ToString())
                End If

                sourceTransactionDetailID = Convert.ToInt32(rdr("SrcDetailID"))
                targetTransactionDetailID = Convert.ToInt32(rdr("TgtDetailID"))
            End Using
        End Using

        ' UPDATED: column names changed to ...DetailID
        Dim sql As String = "
INSERT INTO dbo.Inventory_CostLedgerLink
(
    SourceLedgerID,
    TargetLedgerID,
    LinkType,
    FlowQty,
    FlowUnitCost,
    SourceTransactionDetailID,
    TargetTransactionDetailID,
    SourceStoreID,
    TargetStoreID,
    ProductID,
    BaseProductID,
    PostingDate,
    OperationGroupID,
    GroupSeq,
    IsActive,
    CreatedAt,
    CreatedBy,
    RootLedgerID,
    LinkDirection,
    LinkHash
)
VALUES
(
    @SourceLedgerID,
    @TargetLedgerID,
    @LinkType,
    @FlowQty,
    @FlowUnitCost,
    @SourceTransactionDetailID,
    @TargetTransactionDetailID,
    @SourceStoreID,
    @TargetStoreID,
    @ProductID,
    @BaseProductID,
    @PostingDate,
    @OperationGroupID,
    @GroupSeq,
    1,
    SYSDATETIME(),
    @UserID,
    @RootLedgerID,
    @LinkDirection,
    @LinkHash
);"

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.Add("@SourceLedgerID", SqlDbType.BigInt).Value = sourceLedgerID
            cmd.Parameters.Add("@TargetLedgerID", SqlDbType.BigInt).Value = targetLedgerID
            cmd.Parameters.Add("@LinkType", SqlDbType.SmallInt).Value = linkType

            cmd.Parameters.Add("@FlowQty", SqlDbType.Decimal).Value = qty
            cmd.Parameters.Add("@FlowUnitCost", SqlDbType.Decimal).Value = unitCost

            cmd.Parameters.Add("@SourceTransactionDetailID", SqlDbType.Int).Value = sourceTransactionDetailID
            cmd.Parameters.Add("@TargetTransactionDetailID", SqlDbType.Int).Value = targetTransactionDetailID

            cmd.Parameters.Add("@SourceStoreID", SqlDbType.Int).Value = storeSource
            cmd.Parameters.Add("@TargetStoreID", SqlDbType.Int).Value = storeTarget
            cmd.Parameters.Add("@RootLedgerID", SqlDbType.BigInt).Value = rootLedgerID

            cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID

            Dim pBase = cmd.Parameters.Add("@BaseProductID", SqlDbType.Int)
            pBase.Value = If(baseProductID.HasValue, baseProductID.Value, CType(DBNull.Value, Object))

            cmd.Parameters.Add("@PostingDate", SqlDbType.DateTime2).Value = postingDate

            cmd.Parameters.Add("@OperationGroupID", SqlDbType.UniqueIdentifier).Value = operationGroupID
            cmd.Parameters.Add("@GroupSeq", SqlDbType.Int).Value = groupSeq

            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
            cmd.Parameters.Add("@LinkDirection", SqlDbType.Int).Value = linkDirection
            cmd.Parameters.Add("@LinkHash", SqlDbType.VarBinary, 32).Value = linkHash

            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Public Sub InsertProductionLedgerLinks(
    transactionID As Integer,
    operationGroupID As Guid,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction)

        Dim dt As New DataTable()

        Dim sql As String = "
SELECT
    LedgerID,
    ProductID,
    BaseProductID,
    StoreID,
    OutQty,
    InQty,
    OutUnitCost,
    PostingDate,
    GroupSeq
FROM dbo.Inventory_CostLedger
WHERE TransactionID = @TransactionID
  AND IsActive = 1
  AND IsReversed = 0
"

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.Add("@TransactionID", SqlDbType.Int).Value = transactionID
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Dim rawRows = dt.Select("OutQty > 0")
        Dim fgRows = dt.Select("InQty > 0")

        If rawRows.Length = 0 Then Exit Sub
        If fgRows.Length = 0 Then Exit Sub

        If fgRows.Length <> 1 Then
            Throw New Exception("PRO expects exactly 1 FG IN ledger, found " &
                            fgRows.Length & " for TransactionID=" & transactionID.ToString())
        End If

        Dim fg As DataRow = fgRows(0)
        Dim targetLedgerID As Long = CLng(fg("LedgerID"))
        Dim targetStore As Integer = CInt(fg("StoreID"))

        ' ✅ Policy: Link.ProductID = Target product (FG)
        Dim targetProductID As Integer = CInt(fg("ProductID"))

        For Each raw As DataRow In rawRows

            Dim sourceLedgerID As Long = CLng(raw("LedgerID"))
            Dim sourceStore As Integer = CInt(raw("StoreID"))

            Dim qty As Decimal = CDec(raw("OutQty"))
            Dim unitCost As Decimal = CDec(raw("OutUnitCost"))
            Dim postingDate As DateTime = CDate(raw("PostingDate"))
            Dim groupSeq As Integer = CInt(raw("GroupSeq"))

            InsertLedgerLink(
            sourceLedgerID:=sourceLedgerID,
            targetLedgerID:=targetLedgerID,
            linkType:=CShort(2),   ' PROD_CONSUME
            qty:=qty,
            unitCost:=unitCost,
            transactionID:=transactionID,
            storeSource:=sourceStore,
            storeTarget:=targetStore,
            productID:=targetProductID,   ' ✅ FG product
            baseProductID:=Nothing,       ' keep NULL policy (or pass CInt(raw("BaseProductID")) if you still want it)
            postingDate:=postingDate,
            operationGroupID:=operationGroupID,
            groupSeq:=groupSeq,
            userID:=userID,
            con:=con,
            tran:=tran
        )

        Next

    End Sub
    Public Sub InsertScrapProductionLinks(
    transactionID As Integer,
    operationGroupID As Guid,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction)

        Const LINK_PROD_CONSUME As Short = 2

        ' 1) Get scrap IN ledger (target) - must be exactly one
        Dim targetLedgerID As Long
        Dim targetStoreID As Integer
        Dim targetProductID As Integer
        Dim postingDate As DateTime

        Using cmd As New SqlCommand("
;WITH t AS
(
    SELECT LedgerID, StoreID, ProductID, PostingDate
    FROM dbo.Inventory_CostLedger
    WHERE TransactionID=@T
      AND InQty > 0
      AND IsActive=1 AND IsReversed=0
)
SELECT
    (SELECT COUNT(*) FROM t) AS Cnt,
    (SELECT TOP 1 LedgerID FROM t ORDER BY LedgerID DESC) AS LedgerID,
    (SELECT TOP 1 StoreID  FROM t ORDER BY LedgerID DESC) AS StoreID,
    (SELECT TOP 1 ProductID FROM t ORDER BY LedgerID DESC) AS ProductID,
    (SELECT TOP 1 PostingDate FROM t ORDER BY LedgerID DESC) AS PostingDate;
", con, tran)

            cmd.Parameters.Add("@T", SqlDbType.Int).Value = transactionID

            Using rdr = cmd.ExecuteReader()
                If Not rdr.Read() Then Throw New Exception("SCRAP target IN ledger query failed. T=" & transactionID)

                Dim cnt As Integer = CInt(rdr("Cnt"))
                If cnt <> 1 Then
                    Throw New Exception("SCRAP expects exactly 1 IN ledger, found " & cnt & " for TransactionID=" & transactionID)
                End If

                targetLedgerID = CLng(rdr("LedgerID"))
                targetStoreID = CInt(rdr("StoreID"))
                targetProductID = CInt(rdr("ProductID"))
                postingDate = CDate(rdr("PostingDate"))
            End Using
        End Using

        ' 2) Get OUT ledgers (sources) + WasteDetails (weight KG + cost amount)
        Dim src As New DataTable()
        Using cmd As New SqlCommand("
SELECT
    cl.LedgerID AS SourceLedgerID,
    cl.StoreID  AS SourceStoreID,
    wd.WasteWeight_kg,
    wd.CostAmount
FROM dbo.Inventory_CostLedger cl
JOIN dbo.Inventory_TransactionDetails td
  ON td.TransactionID = cl.TransactionID
 AND td.DetailID = cl.SourceDetailID
JOIN dbo.Inventory_WasteDetails wd
  ON wd.WasteDetailID = td.SourceDocumentDetailID
WHERE cl.TransactionID=@T
  AND cl.OutQty > 0
  AND cl.IsActive=1 AND cl.IsReversed=0
", con, tran)

            cmd.Parameters.Add("@T", SqlDbType.Int).Value = transactionID

            Using da As New SqlDataAdapter(cmd)
                da.Fill(src)
            End Using
        End Using

        If src.Rows.Count = 0 Then Exit Sub

        For Each r As DataRow In src.Rows

            Dim sourceLedgerID As Long = CLng(r("SourceLedgerID"))
            Dim sourceStoreID As Integer = CInt(r("SourceStoreID"))

            Dim qtyKg As Decimal = CDec(r("WasteWeight_kg"))
            Dim costAmount As Decimal = CDec(r("CostAmount"))

            If qtyKg <= 0D Then Continue For

            Dim unitCostKg As Decimal = costAmount / qtyKg  ' ✅ cost per KG (matches FlowQty unit)

            InsertLedgerLink(
            sourceLedgerID:=sourceLedgerID,
            targetLedgerID:=targetLedgerID,
            linkType:=LINK_PROD_CONSUME,
            qty:=qtyKg,                 ' ✅ KG
            unitCost:=unitCostKg,       ' ✅ cost/KG
            transactionID:=transactionID,
            storeSource:=sourceStoreID,
            storeTarget:=targetStoreID,
            productID:=targetProductID, ' ✅ Target product (scrap)
            baseProductID:=Nothing,
            postingDate:=postingDate,
            operationGroupID:=operationGroupID,
            groupSeq:=1,
            userID:=userID,
            con:=con,
            tran:=tran
        )
        Next
    End Sub


    Public Sub InsertTransferLedgerLinks_TRN(
    transactionID As Integer,
    operationGroupID As Guid,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Const LINK_TRANSFER As Short = 1

        Dim m3UnitID As Integer

        ' UnitID للمتر المكعب
        Using cmd As New SqlCommand("
        SELECT UnitID
        FROM Master_Unit
        WHERE UnitCode='M3'
    ", con, tran)

            m3UnitID = CInt(cmd.ExecuteScalar())

        End Using


        ' تفاصيل التحويل
        Dim dt As New DataTable()

        Using cmd As New SqlCommand("
SELECT
    d.DetailID,
    d.ProductID,
    d.Quantity,
    d.SourceStoreID,
    d.TargetStoreID,
    p.StorageUnitID,
    bp.StorageUnitID AS BaseStorageUnitID
FROM Inventory_TransactionDetails d
JOIN Master_Product p
    ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID=@T
  AND d.SourceStoreID IS NOT NULL
  AND d.TargetStoreID IS NOT NULL
ORDER BY d.DetailID
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using

        End Using

        If dt.Rows.Count = 0 Then Exit Sub


        ' PostingDate
        Dim postingDate As DateTime

        Using cmd As New SqlCommand("
SELECT PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@T
", con, tran)

            cmd.Parameters.AddWithValue("@T", transactionID)

            postingDate = CDate(cmd.ExecuteScalar())

        End Using


        For Each r As DataRow In dt.Rows

            Dim detailID As Integer = CInt(r("DetailID"))
            Dim productID As Integer = CInt(r("ProductID"))
            Dim trxQty As Decimal = CDec(r("Quantity"))

            If trxQty = 0D Then Continue For

            Dim sourceStore As Integer = CInt(r("SourceStoreID"))
            Dim targetStore As Integer = CInt(r("TargetStoreID"))

            Dim productUnit As Integer = CInt(r("StorageUnitID"))

            Dim baseUnit As Integer = 0
            If Not IsDBNull(r("BaseStorageUnitID")) Then
                baseUnit = CInt(r("BaseStorageUnitID"))
            End If


            ' ======================================
            ' تحديد وحدة التحويل
            ' ======================================

            Dim flowQty As Decimal

            If productUnit = m3UnitID Then

                ' المنتج نفسه بالمتر المكعب
                flowQty = trxQty

            ElseIf baseUnit = m3UnitID Then

                ' المنتج قطعة لكن الأب بالمتر المكعب
                flowQty = ConvertQtyToLedgerUnit(productID, trxQty, con, tran)

            Else

                ' لا تحويل
                flowQty = trxQty

            End If


            ' ======================================
            ' OUT Ledger
            ' ======================================

            Dim outLedgerID As Long
            Dim outUnitCost As Decimal

            Using cmd As New SqlCommand("
SELECT TOP 1
    LedgerID,
    ISNULL(OutUnitCost,0) AS OutUnitCost
FROM Inventory_CostLedger
WHERE TransactionID=@T
AND SourceDetailID=@D
AND StoreID=@S
AND OutQty>0
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@T", transactionID)
                cmd.Parameters.AddWithValue("@D", detailID)
                cmd.Parameters.AddWithValue("@S", sourceStore)

                Using rdr = cmd.ExecuteReader()

                    If Not rdr.Read() Then
                        Throw New Exception("TRN OUT ledger not found")
                    End If

                    outLedgerID = CLng(rdr("LedgerID"))
                    outUnitCost = CDec(rdr("OutUnitCost"))

                End Using

            End Using


            ' ======================================
            ' IN Ledger
            ' ======================================

            Dim inLedgerID As Long

            Using cmd As New SqlCommand("
SELECT TOP 1 LedgerID
FROM Inventory_CostLedger
WHERE TransactionID=@T
AND SourceDetailID=@D
AND StoreID=@S
AND InQty>0
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmd.Parameters.AddWithValue("@T", transactionID)
                cmd.Parameters.AddWithValue("@D", detailID)
                cmd.Parameters.AddWithValue("@S", targetStore)

                Dim v = cmd.ExecuteScalar()

                If v Is Nothing Then
                    Throw New Exception("TRN IN ledger not found")
                End If

                inLedgerID = CLng(v)

            End Using


            ' ======================================
            ' إدخال الرابط
            ' ======================================

            InsertLedgerLink(
            sourceLedgerID:=outLedgerID,
            targetLedgerID:=inLedgerID,
            linkType:=LINK_TRANSFER,
            qty:=flowQty,
            unitCost:=outUnitCost,
            transactionID:=transactionID,
            storeSource:=sourceStore,
            storeTarget:=targetStore,
            productID:=productID,
            baseProductID:=Nothing,
            postingDate:=postingDate,
            operationGroupID:=operationGroupID,
            groupSeq:=1,
            userID:=userID,
            con:=con,
            tran:=tran
        )

        Next

    End Sub


    Public Sub UpdateProductAvgCost(productID As Integer, avgCost As Decimal, con As SqlConnection, tran As SqlTransaction)
        Dim sql = "
        UPDATE Master_Product 
        SET AvgCost = @AvgCost
        WHERE ProductID = @ProductID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", productID)
        cmd.Parameters.AddWithValue("@AvgCost", avgCost)

        cmd.ExecuteNonQuery()
    End Sub
    Public Sub UpdateFinalProductAvgCost(baseProductID As Integer, avgCostPerM3 As Decimal, con As SqlConnection, tran As SqlTransaction)
        Dim sql = "
        UPDATE Master_FinalProductAvgCost 
        SET AvgCostPerM3 = @AvgCostPerM3,
            LastUpdated = GETDATE()
        WHERE BaseProductID = @BaseProductID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@BaseProductID", baseProductID)
        cmd.Parameters.AddWithValue("@AvgCostPerM3", avgCostPerM3)

        cmd.ExecuteNonQuery()
    End Sub
    Public Sub UpdateFinalStatuses(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim sql As String = "

/* 1) تحديث حالة الترانسكشن */
UPDATE Inventory_TransactionHeader
SET StatusID = 6,
    IsInventoryPosted = 1,
    ReceivedAt = ISNULL(ReceivedAt, SYSDATETIME())
WHERE TransactionID = @TransactionID;

/* 2) Snapshot تكلفة الأب وقت الـ Posting لعمليات القص فقط
      - نفترض OperationTypeID = 11 هو القص (غيّره إذا لزم)
      - نعبّي مرة واحدة فقط إذا القيم NULL
*/
UPDATE th
SET
    th.BaseAvgCostPerM3_AtPosting =
        CASE WHEN th.BaseAvgCostPerM3_AtPosting IS NULL THEN f.AvgCostPerM3 ELSE th.BaseAvgCostPerM3_AtPosting END,
    th.BaseAvgCostPerM3forFG_AtPosting =
        CASE WHEN th.BaseAvgCostPerM3forFG_AtPosting IS NULL THEN f.AvgCostPerM3forFG ELSE th.BaseAvgCostPerM3forFG_AtPosting END
FROM dbo.Inventory_TransactionHeader th
JOIN dbo.Production_CuttingHeader ch
    ON ch.CuttingID = th.SourceDocumentID
JOIN dbo.Master_FinalProductAvgCost f
    ON f.BaseProductID = ch.BaseProductID
WHERE th.TransactionID = @TransactionID
  AND th.OperationTypeID = 11
  AND th.IsInventoryPosted = 1
  AND th.StatusID = 6
  AND (
        th.BaseAvgCostPerM3_AtPosting IS NULL
     OR th.BaseAvgCostPerM3forFG_AtPosting IS NULL
  );

/* 3) تحديث حالة المستند المرتبط (DocumentHeader) */
UPDATE dh
SET dh.StatusID = 6,
    dh.IsInventoryPosted = 1
FROM Inventory_DocumentHeader dh
JOIN Inventory_TransactionHeader th
    ON th.SourceDocumentID = dh.DocumentID
WHERE th.TransactionID = @TransactionID;

/* 4) تحديث حالة الهالك (WasteHeader) المرتبط بهذه الحركة
      ملاحظة: هذا يعتمد على وجود ربط بين WasteHeader و TransactionHeader.
      - جرّبت أكثر ربط شائع: WasteHeader.TransactionID
      - إذا اسم العمود مختلف عندك عدّله.
*/
UPDATE wh
SET wh.StatusID = 6
FROM dbo.Inventory_WasteHeader wh
WHERE wh.TransactionID = @TransactionID;

"

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Public Sub UpdateDocumentStatus(transactionID As Integer, con As SqlConnection, tran As SqlTransaction)
        ' جلب DocumentID من TransactionHeader
        Dim sql = "SELECT SourceDocumentID FROM Inventory_TransactionHeader WHERE TransactionID = @TransactionID"
        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

        Dim SourceDocumentID = cmd.ExecuteScalar()
        If SourceDocumentID IsNot Nothing AndAlso Not IsDBNull(SourceDocumentID) Then
            ' تحديث حالة المستند في Inventory_DocumentHeader
            sql = "
            UPDATE Inventory_DocumentHeader 
            SET IsInventoryPosted = 0,
                StatusID = 2,
            Notes = 'مرتجع من الستودع بسبب الغاء الاستلام'
            WHERE DocumentID = @DocumentID"

            cmd = New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@DocumentID", SourceDocumentID)
            cmd.ExecuteNonQuery()
        End If
    End Sub


    Public Sub ReverseInventoryMovement(detail As TransactionDetailDTO, con As SqlConnection, tran As SqlTransaction)
        Dim sql = "
        UPDATE Inventory_Balance 
        SET QtyOnHand = QtyOnHand - @Quantity,
            LastUpdatedAt = GETDATE()
        WHERE ProductID = @ProductID AND StoreID = @StoreID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", detail.ProductID)
        cmd.Parameters.AddWithValue("@StoreID", detail.StoreID)
        cmd.Parameters.AddWithValue("@Quantity", detail.Quantity)

        cmd.ExecuteNonQuery()
    End Sub
    Public Function LogReverseInCostLedger(
        detail As TransactionDetailDTO,
        userID As Integer,
        oldAvgData As AvgCostData,
        con As SqlConnection,
        tran As SqlTransaction
    ) As Integer
        ' جلب الرصيد الحالي بعد العكس
        Dim currentQty As Decimal = 0
        Dim sql = "
SELECT QtyOnHand 
FROM Inventory_Balance 
WHERE ProductID = @ProductID 
  AND BaseProductID = @BaseProductID
  AND StoreID = @StoreID"
        Dim baseProductIDValue As Object

        If oldAvgData.BaseProductID > 0 Then
            baseProductIDValue = oldAvgData.BaseProductID
        Else
            baseProductIDValue = detail.ProductID
        End If
        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@ProductID", detail.ProductID)
        cmd.Parameters.AddWithValue("@StoreID", detail.StoreID)
        cmd.Parameters.AddWithValue("@BaseProductID", baseProductIDValue)
        currentQty = Convert.ToDecimal(cmd.ExecuteScalar())


        sql = "
INSERT INTO Inventory_CostLedger
(
ledgerid,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    StoreID,
    OperationTypeID,
    OldQty,
    InQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    PostingDate,
    IsReversed,
    CreatedBy,
    CreatedAt,
LedgerSequence,
    PrevLedgerID
)
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
    @TransactionID,
    NULL,
    @ProductID,
    @BaseProductID,
    @StoreID,
    @OperationTypeID,
    @OldQty,
    @InQty,
    @NewQty,
    @OldAvgCost,
    @InUnitCost,
    @InTotalCost,
    @NewAvgCost,
    SYSDATETIME(),
    1,
    @CreatedBy,
    SYSDATETIME(),
@LedgerSequence,
    @PrevLedgerID
);
SELECT SCOPE_IDENTITY();"

        cmd = New SqlCommand(sql, con, tran)

        cmd.Parameters.AddWithValue("@TransactionID", detail.TransactionID)
        cmd.Parameters.AddWithValue("@ProductID", detail.ProductID)
        cmd.Parameters.AddWithValue("@BaseProductID", baseProductIDValue)
        cmd.Parameters.AddWithValue("@StoreID", detail.StoreID)

        cmd.Parameters.AddWithValue("@OldQty", currentQty + detail.Quantity)
        cmd.Parameters.AddWithValue("@InQty", -detail.Quantity)
        cmd.Parameters.AddWithValue("@NewQty", currentQty)

        cmd.Parameters.AddWithValue("@OldAvgCost", detail.UnitCost)
        cmd.Parameters.AddWithValue("@InUnitCost", detail.UnitCost)
        cmd.Parameters.AddWithValue("@InTotalCost", -(detail.Quantity * detail.UnitCost))
        cmd.Parameters.AddWithValue("@NewAvgCost", oldAvgData.OldAvgCost)

        cmd.Parameters.AddWithValue("@OperationTypeID", -7)
        cmd.Parameters.AddWithValue("@CreatedBy", userID)

        cmd.Parameters.AddWithValue("@PrevLedgerID", oldAvgData.OriginalLedgerID)
        Return Convert.ToInt32(cmd.ExecuteScalar())
    End Function
    Public Sub ValidateNoLaterAverageUsage(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Dim sql As String = "

    IF EXISTS
    (
        SELECT 1
        FROM Inventory_CostLedger cl
        JOIN Inventory_CostLedger later
            ON later.BaseProductID = cl.BaseProductID
           AND later.LedgerID > cl.LedgerID
           AND later.IsReversed = 0
        WHERE cl.TransactionID = @TransactionID
          AND cl.IsReversed = 0
    )
    BEGIN
        THROW 50001, 
        N'لا يمكن عكس الاستلام: توجد حركة لاحقة استخدمت المتوسط الجديد.', 
        1;
    END
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.ExecuteNonQuery()
        End Using

    End Sub
    Public Function HasM3ProductsInTransaction(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Dim sql As String = "
    SELECT COUNT(*)
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)

            Dim count As Integer = CInt(cmd.ExecuteScalar())
            Return count > 0
        End Using

    End Function
    Public Function HasNonM3ProductsInTransaction(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        Dim sql As String = "
    SELECT COUNT(*)
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
      AND p.StorageUnitID <> @M3UnitID
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Dim count As Integer = CInt(cmd.ExecuteScalar())
            Return count > 0
        End Using

    End Function
    Public Function TestNonM3Products(
    transactionID As Integer,
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim dt As New DataTable()
        Dim sql As String = "
    SELECT 
        d.ProductID,
        p.ProductName,
        p.StorageUnitID,
        CASE 
            WHEN p.StorageUnitID = @M3UnitID THEN 'M3'
            ELSE 'غير M3'
        END AS UnitType
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt
    End Function
    Private Function CalculateLinkHash(
        sourceLedgerID As Long,
        targetLedgerID As Long,
        linkType As Short,
        qty As Decimal,
        unitCost As Decimal,
        operationGroupID As Guid
    ) As Byte()

        Dim raw As String =
            sourceLedgerID.ToString() & "|" &
            targetLedgerID.ToString() & "|" &
            linkType.ToString() & "|" &
            qty.ToString("0.########") & "|" &
            unitCost.ToString("0.########") & "|" &
            operationGroupID.ToString()

        Dim bytes = System.Text.Encoding.UTF8.GetBytes(raw)

        Using sha As Security.Cryptography.SHA256 = Security.Cryptography.SHA256.Create()
            Return sha.ComputeHash(bytes)
        End Using

    End Function

    Public Sub FinalizeSalesReturnAfterReceive(
        srtDocumentID As Integer,
        sourceInvoiceID As Integer,
        sourceInvoiceUUID As String,
        documentNo As String,
        userID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    )

        '========================================
        ' 1) تحديث حالة المرتجع
        '========================================
        Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 5,
    SentAt = SYSDATETIME(),
    SentBy = @UserID
WHERE DocumentID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", srtDocumentID)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()

        End Using


        '========================================
        ' 2) ربط SAL ↔ SRT
        '========================================
        Using cmdLink As New SqlCommand("
IF NOT EXISTS (
    SELECT 1 FROM dbo.Document_Link
    WHERE SourceDocumentID = @SourceID
      AND TargetDocumentID = @TargetID
      AND LinkType = 'RETURN'
)
BEGIN
    INSERT INTO dbo.Document_Link
    (
        SourceDocumentID, SourceType,
        TargetDocumentID, TargetType,
        LinkType, CreatedAt
    )
    VALUES
    (
        @SourceID, 'SAL',
        @TargetID, 'SRT',
        'RETURN', SYSDATETIME()
    )
END
", con, tran)

            cmdLink.Parameters.AddWithValue("@SourceID", sourceInvoiceID)
            cmdLink.Parameters.AddWithValue("@TargetID", srtDocumentID)
            cmdLink.ExecuteNonQuery()

        End Using


        '========================================
        ' 3) تحديد حالة الفاتورة الأصلية
        '========================================
        Dim isFullyReturned As Boolean = False
        Dim hasAnyReturn As Boolean = False

        Using cmd As New SqlCommand("
DECLARE @HasAny bit = 0, @IsFull bit = 1;

IF EXISTS (
    SELECT 1
    FROM Inventory_DocumentDetails R
    INNER JOIN Inventory_DocumentHeader H
        ON H.DocumentID = R.DocumentID
    WHERE H.DocumentType = 'SRT'
      AND H.StatusID <> 10
      AND R.SourceDocumentDetailID IN
        (SELECT DetailID FROM Inventory_DocumentDetails WHERE DocumentID = @INV)
)
    SET @HasAny = 1;

IF EXISTS (
    SELECT 1
    FROM Inventory_DocumentDetails I
    WHERE I.DocumentID = @INV
      AND ISNULL((
          SELECT SUM(R.Quantity)
          FROM Inventory_DocumentDetails R
          INNER JOIN Inventory_DocumentHeader H
              ON H.DocumentID = R.DocumentID
          WHERE H.DocumentType = 'SRT'
            AND H.StatusID <> 10
            AND R.SourceDocumentDetailID = I.DetailID
      ),0) < I.Quantity
)
    SET @IsFull = 0;

SELECT @HasAny AS HasAnyReturn, @IsFull AS IsFullyReturned;
", con, tran)

            cmd.Parameters.AddWithValue("@INV", sourceInvoiceID)

            Using rd = cmd.ExecuteReader()
                rd.Read()
                hasAnyReturn = CBool(rd("HasAnyReturn"))
                isFullyReturned = CBool(rd("IsFullyReturned"))
            End Using

        End Using


        Dim newStatus As Integer

        If Not hasAnyReturn Then
            newStatus = 18
        ElseIf Not isFullyReturned Then
            newStatus = 20
        Else
            newStatus = 9
        End If


        Using cmdUpdate As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = @StatusID
WHERE DocumentID = @InvoiceID
", con, tran)

            cmdUpdate.Parameters.AddWithValue("@StatusID", newStatus)
            cmdUpdate.Parameters.AddWithValue("@InvoiceID", sourceInvoiceID)
            cmdUpdate.ExecuteNonQuery()

        End Using


        '========================================
        ' 4) ZATCA Credit Note
        '========================================

        Dim prevHash As String = Nothing

        Using cmd As New SqlCommand("
SELECT TOP 1 InvoiceHash
FROM Inventory_ZatcaDocument
WHERE InvoiceHash IS NOT NULL 
AND LTRIM(RTRIM(InvoiceHash)) <> ''
ORDER BY CreatedAt DESC, ZatcaID DESC
", con, tran)

            Dim r = cmd.ExecuteScalar()

            If r IsNot Nothing AndAlso Not IsDBNull(r) Then
                prevHash = r.ToString()
            End If

        End Using

        If String.IsNullOrWhiteSpace(prevHash) Then
            prevHash = ComputeSha256Base64("0")
        End If


        Dim totalAmt As Decimal

        Using cmd As New SqlCommand("
SELECT ISNULL(TotalAmount,0)
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)

            cmd.Parameters.AddWithValue("@ID", srtDocumentID)
            totalAmt = Convert.ToDecimal(cmd.ExecuteScalar())

        End Using


        Dim payload As String =
            $"SRT={srtDocumentID}|NO={documentNo}|AMT={totalAmt}|UTC={DateTime.UtcNow:O}"

        Dim returnHash As String = ComputeSha256Base64(payload)


        Dim returnZatcaID As Integer

        Using cmd As New SqlCommand("
INSERT INTO Inventory_ZatcaDocument
(
    DocumentID,
    UUID,
    InvoiceTypeCode,
    IsSimplified,
    OriginalInvoiceUUID,
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
    '381',
    0,
    @OrigUUID,
    @Hash,
    @PrevHash,
    17,
    SYSDATETIME(),
    SYSDATETIME()
);

SELECT SCOPE_IDENTITY();
", con, tran)

            cmd.Parameters.AddWithValue("@DocID", srtDocumentID)
            cmd.Parameters.AddWithValue("@OrigUUID", sourceInvoiceUUID)
            cmd.Parameters.AddWithValue("@Hash", returnHash)
            cmd.Parameters.AddWithValue("@PrevHash", prevHash)

            returnZatcaID = Convert.ToInt32(cmd.ExecuteScalar())

        End Using


        '========================================
        ' 5) TaxTotals
        '========================================
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
    -SUM(D.TaxableAmount),
    -SUM(D.TaxAmount),
    TT.TaxCategoryCode,
    MAX(D.TaxRate)
FROM Inventory_DocumentDetails D
INNER JOIN Master_TaxType TT
    ON TT.TaxTypeID = D.TaxTypeID
WHERE D.DocumentID = @DocID
  AND D.Quantity > 0
GROUP BY TT.TaxCategoryCode
", con, tran)

            cmd.Parameters.AddWithValue("@ZID", returnZatcaID)
            cmd.Parameters.AddWithValue("@DocID", srtDocumentID)
            cmd.ExecuteNonQuery()

        End Using


        '========================================
        ' 6) Simulation Auto Clear
        '========================================
        Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 18,
    IsZatcaReported = 1
WHERE DocumentID = @ID;

UPDATE Inventory_ZatcaDocument
SET ZatcaStatus = 18,
    ClearedAt = SYSDATETIME()
WHERE ZatcaID = @ZID;
", con, tran)

            cmd.Parameters.AddWithValue("@ID", srtDocumentID)
            cmd.Parameters.AddWithValue("@ZID", returnZatcaID)
            cmd.ExecuteNonQuery()

        End Using


        '========================================
        ' 7) تحديث Totals
        '========================================
        Using cmd As New SqlCommand("
UPDATE H
SET
    TotalNetAmount = X.SumNet,
    TotalAmount = X.SumGross,
    TotalDiscount = X.SumDiscount,
    TotalTax = X.SumTax,
    TotalTaxableAmount = X.SumNet,
    GrandTotal = X.SumTotal
FROM Inventory_DocumentHeader H
CROSS APPLY
(
    SELECT
        SUM(GrossAmount) AS SumGross,
        SUM(DiscountAmount) AS SumDiscount,
        SUM(NetAmount) AS SumNet,
        SUM(TaxAmount) AS SumTax,
        SUM(LineTotal) AS SumTotal
    FROM Inventory_DocumentDetails
    WHERE DocumentID = @DocID
) X
WHERE H.DocumentID = @DocID
", con, tran)

            cmd.Parameters.AddWithValue("@DocID", srtDocumentID)
            cmd.ExecuteNonQuery()

        End Using

    End Sub



    Public Sub Receive_ScrapInsideTransaction(
    transactionID As Integer,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        '====================================================
        ' 1) Units
        '====================================================
        Dim m3UnitID As Integer
        Dim seq As Integer = 1
        Using cmd As New SqlCommand("
        SELECT UnitID
        FROM dbo.Master_Unit
        WHERE UnitCode = 'M3'
    ", con, tran)

            m3UnitID = CInt(cmd.ExecuteScalar())

        End Using


        '====================================================
        ' 2) Operation Group
        '====================================================
        Dim operationGroupID As Guid = Guid.NewGuid()
        Dim oldQtyDict =
GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

        '====================================================
        ' 3) Detect Products
        '====================================================
        Dim dt = GetProductsByTargetStore(transactionID, con, tran)

        Dim hasM3 As Boolean = False
        Dim hasNonM3 As Boolean = False
        Dim productIdsM3 As New List(Of Integer)
        Dim productIdsNonM3 As New List(Of Integer)

        For Each row As DataRow In dt.Rows

            If CInt(row("StorageUnitID")) = m3UnitID Then

                hasM3 = True
                productIdsM3.Add(CInt(row("ProductID")))

            Else

                hasNonM3 = True
                productIdsNonM3.Add(CInt(row("ProductID")))

            End If

        Next


        '====================================================
        ' 4) Ledger
        '====================================================
        ' أولاً الخروج
        InsertCostLedger_OUT(transactionID, operationGroupID, seq, oldQtyDict, con, tran)

        ' ثم الدخول
        If hasM3 Then

            InsertCostLedger_M3(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)
        End If

        Dim ledgerID As Integer =
InsertCostLedger_Regular(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)

        '====================================================
        ' 5) Links
        '====================================================
        InsertScrapProductionLinks(transactionID, operationGroupID, userID, con, tran)


        '====================================================
        ' 6) Inventory
        '====================================================
        ApplyInventoryOut(transactionID, con, tran)

        ApplyInventoryIn(transactionID, m3UnitID, ledgerID, con, tran)


        '====================================================
        ' 7) Cost Recalculate
        '====================================================
        Dim allInboundProducts As List(Of Integer) =
    productIdsM3.
    Concat(productIdsNonM3).
    Distinct().
    ToList()

        RecalculateAverage_PUR_PRO_BySnapshot(
    transactionID,
    m3UnitID,
    allInboundProducts,
    con,
    tran
)



        '====================================================
        ' 9) Finalize Ledger
        '====================================================
        FinalizeLedgerMetadata(operationGroupID, con, tran)

        UpdateFinalStatuses(transactionID, con, tran)

    End Sub
    Private Sub PostLoadingOrder_InsideTransaction(
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
        FROM Logistics_LoadingOrder
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
            1,
            SYSDATETIME()
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
FROM Master_Unit
WHERE UnitCode = 'M3'
", con, tran)

            m3UnitID = CInt(cmd.ExecuteScalar())

        End Using


        '====================================================
        ' 4) إدخال TransactionDetails
        '====================================================
        Using cmdInsert As New SqlCommand("
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

FROM Logistics_LoadingOrderDetail LOD

JOIN Master_Product P
    ON P.ProductID = LOD.ProductID

LEFT JOIN Master_Product BP
    ON BP.ProductID = P.BaseProductID

LEFT JOIN Master_FinalProductAvgCost FP
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

        Dim operationGroupID As Guid = Guid.NewGuid()
        Dim oldQtyDict =
GetOldQtyAllStores(transactionID, operationGroupID, con, tran)
        '====================================================
        ' 5) Ledger
        '====================================================


        InsertCostLedger_OUT_lOADING(transactionID, operationGroupID, seq, oldQtyDict, con, tran)

        '====================================================
        ' 6) Inventory
        '====================================================
        ApplyInventoryOut(transactionID, con, tran)


        '====================================================
        ' 7) Finalize Ledger
        '====================================================
        FinalizeLedgerMetadata(operationGroupID, con, tran)
        Using cmd As New SqlCommand("
UPDATE Logistics_LoadingOrder
SET
    LoadingStatusID = 15,      
    IsInventoryPosted = 1,
    PostedAt = SYSDATETIME(),
    PostedBy = @UserID
WHERE LOID = @LOID
", con, tran)

            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@UserID", userID)

            cmd.ExecuteNonQuery()

        End Using
        UpdateFinalStatuses(transactionID, con, tran)

    End Sub

    Public Sub PostLoadingOrder(loID As Integer, userID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    PostLoadingOrder_InsideTransaction(loID, userID, con, tran)

                    tran.Commit()

                Catch

                    tran.Rollback()
                    Throw

                End Try

            End Using
        End Using

    End Sub

    ''' Cut
    Public Function GetOldQtyAllStores_cut(
    transactionID As Integer,
    operationGroupID As Guid,
    con As SqlConnection,
    tran As SqlTransaction
) As Dictionary(Of Integer, Decimal)

        Dim result As New Dictionary(Of Integer, Decimal)

        Dim sql As String = "
SELECT
    cl.ProductID,
    cl.NewQty
FROM Inventory_CostLedger cl
JOIN
(
    SELECT
        ProductID,
        MAX(LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger
    WHERE IsActive = 1
      AND IsReversed = 0
      AND OperationGroupID <> @OperationGroupID
      AND ProductID IN
      (
        SELECT DISTINCT ProductID
        FROM Inventory_TransactionDetails
        WHERE TransactionID = @TransactionID
      )
    GROUP BY ProductID
) x
ON x.ProductID = cl.ProductID
AND x.LastLedgerID = cl.LedgerID
"

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)

            Using rd = cmd.ExecuteReader()

                While rd.Read()

                    Dim productID As Integer = CInt(rd("ProductID"))
                    Dim qty As Decimal =
                    If(IsDBNull(rd("NewQty")), 0D, CDec(rd("NewQty")))

                    result(productID) = qty

                End While

            End Using

        End Using

        Return result

    End Function
    Public Sub InsertCostLedger_OUTCut(
        transactionID As Integer,
        operationGroupID As Guid,
         ledgerSequence As Integer,
            oldQtyDict As Dictionary(Of Integer, Decimal),
        con As SqlConnection,
        tran As SqlTransaction
    )

        Dim userID As Integer

        Using cmdUser As New SqlCommand("
    SELECT CreatedBy
    FROM Inventory_TransactionHeader
    WHERE TransactionID=@TransactionID
    ", con, tran)

            cmdUser.Parameters.AddWithValue("@TransactionID", transactionID)

            Dim v = cmdUser.ExecuteScalar()
            If v Is Nothing OrElse IsDBNull(v) Then
                Throw New Exception("TransactionHeader not found")
            End If

            userID = Convert.ToInt32(v)
        End Using

        Dim postDate As DateTime
        Dim operationTypeID As Integer

        Using cmdHead As New SqlCommand("
    SELECT PostingDate, OperationTypeID
    FROM Inventory_TransactionHeader
    WHERE TransactionID=@T
    ", con, tran)

            cmdHead.Parameters.AddWithValue("@T", transactionID)

            Dim dtHead As New DataTable()
            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(dtHead)
            End Using

            If dtHead.Rows.Count = 0 Then
                Throw New Exception("TransactionHeader not found")
            End If

            postDate = Convert.ToDateTime(dtHead.Rows(0)("PostingDate"))
            operationTypeID = Convert.ToInt32(dtHead.Rows(0)("OperationTypeID"))
        End Using

        Dim outRows As New DataTable()

        Dim getOutRowsSql As String = "
    SELECT
        d.ProductID,
        ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
        d.SourceStoreID AS StoreID,
        SUM(d.Quantity) AS OutQty,
        MIN(d.DetailID) AS SourceDetailID
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
      AND d.SourceStoreID IS NOT NULL
    GROUP BY
        d.ProductID,
        ISNULL(p.BaseProductID, p.ProductID),
        d.SourceStoreID
    HAVING SUM(d.Quantity) <> 0
    "

        Using cmdGet As New SqlCommand(getOutRowsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(outRows)
            End Using
        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In outRows.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("StoreID"))
            Dim trxQty As Decimal = CDec(row("OutQty"))
            Dim outQty As Decimal = ConvertQtyToLedgerUnit(prodID, trxQty, con, tran)
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            ' منع التكرار لنفس العملية
            Using cmdExists As New SqlCommand("
    SELECT TOP 1 1
    FROM Inventory_CostLedger
    WHERE TransactionID=@T
      AND ProductID=@P
      AND BaseProductID=@B
      AND StoreID=@S
      AND OutQty > 0
      AND IsActive=1
      AND IsReversed=0
    ", con, tran)

                cmdExists.Parameters.AddWithValue("@T", transactionID)
                cmdExists.Parameters.AddWithValue("@P", prodID)
                cmdExists.Parameters.AddWithValue("@B", baseProdID)
                cmdExists.Parameters.AddWithValue("@S", storeID)

                Dim exv = cmdExists.ExecuteScalar()
                If exv IsNot Nothing Then
                    Continue For
                End If
            End Using


            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
                prodID,
                baseProdID,
                operationGroupID,
                con,
                tran,
                prevLedgerID,
                rootLedgerID,
                oldAvgCost
            )

            ' ==============================
            ' 1) OLD QTY (STORE) للفحص + لتعبئة LocalOldQty/LocalNewQty
            ' ==============================
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdBalStore As New SqlCommand("
    SELECT TOP 1 LocalNewQty
    FROM Inventory_CostLedger
    WHERE ProductID=@P
    AND StoreID=@S
    AND IsActive=1
    AND IsReversed=0
    ORDER BY LedgerID DESC
    ", con, tran)

                cmdBalStore.Parameters.AddWithValue("@P", prodID)

                cmdBalStore.Parameters.AddWithValue("@S", storeID)

                Dim v = cmdBalStore.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal = ConvertBalanceQtyToLedgerUnit(prodID, localOldQtyBalance, con, tran)

            If localOldQty < outQty Then
                Throw New Exception("Stock would become negative Product=" & prodID.ToString() & " Store=" & storeID.ToString())
            End If

            Dim localNewQty As Decimal = localOldQty - outQty

            ' ==========================================
            ' 2) OLD QTY (GLOBAL) لحساب المتوسط عالمي
            ' ==========================================
            Dim oldQtyGlobalBalance As Decimal = 0D
            Using cmdBalGlobal As New SqlCommand("
    SELECT TOP 1 NewQty
    FROM Inventory_CostLedger
    WHERE ProductID=@P
    AND BaseProductID=@B
    AND IsActive=1
    AND IsReversed=0
    ORDER BY LedgerID DESC
    ", con, tran)

                cmdBalGlobal.Parameters.AddWithValue("@P", prodID)
                cmdBalGlobal.Parameters.AddWithValue("@B", baseProdID)

                Dim v = cmdBalGlobal.ExecuteScalar()

                oldQtyGlobalBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))

            End Using

            Dim oldQtyGlobal As Decimal =
        ConvertBalanceQtyToLedgerUnit(prodID, oldQtyGlobalBalance, con, tran)


            Dim sqlInsert As String = "
    INSERT INTO Inventory_CostLedger
    (
        LedgerID,
        TransactionID,
        SourceDetailID,
        ProductID,
        BaseProductID,
        StoreID,
        OperationTypeID,

        LocalOldQty,
        LocalNewQty,

        OldQty,
        InQty,
        OutQty,
        NewQty,
        OldAvgCost,
        InUnitCost,
        InTotalCost,
        OutUnitCost,
        OutTotalCost,
        NewAvgCost,
        PostingDate,
        IsReversed,
        IsRevaluation,
        RootLedgerID,
        PrevLedgerID,
        DependsOnLedgerID,
        CostSourceType,
        RootTransactionID,
        CreatedBy,
        CreatedAt,
        OperationGroupID,
        GroupSeq,
        ScopeKeyType,
        ScopeKeyID,
        LedgerSequence,
        IsActive
    )
    VALUES
    (
        NEXT VALUE FOR Seq_CostLedgerID,
        @TransactionID,
        @SourceDetailID,
        @ProductID,
        @BaseProductID,
        @StoreID,
        @OperationTypeID,

        @LocalOldQty,
        @LocalNewQty,

        @OldQtyGlobal,
        0,
        @OutQty,
        @OldQtyGlobal - @OutQty,
        @OldAvgCost,
        0,
        0,
        @OldAvgCost,
        @OutQty * @OldAvgCost,
        @OldAvgCost,
        @PostingDate,
        0,
        0,
        @RootLedgerID,
        @PrevLedgerID,
        @PrevLedgerID,
        @OperationTypeID,
        @TransactionID,
        @UserID,
        SYSDATETIME(),
        @OperationGroupID,
        1,
        'PRODUCT',
        @ProductID,
        @LedgerSequence,
        1
    )
    "

            Using cmd As New SqlCommand(sqlInsert, con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                ' ✅ الأعمدة الجديدة (Local)
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobal)
                cmd.Parameters.AddWithValue("@OutQty", outQty)
                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@PostingDate", postDate)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()
            End Using
            seq += 1
        Next

    End Sub
    Public Sub InsertCostLedger_CUT(
                                transactionID As Integer,
                                userID As Integer,
                                m3UnitID As Integer,
                                operationGroupID As Guid,
                                 ledgerSequence As Integer,
                                    oldQtyDict As Dictionary(Of Integer, Decimal),
                                con As SqlConnection,
                                tran As SqlTransaction)

        '====================================================
        ' Header (كما كان)
        '====================================================
        Dim postDate As DateTime
        Dim operationTypeID As Integer

        Using cmdHead As New SqlCommand("
                            SELECT PostingDate, OperationTypeID
                            FROM Inventory_TransactionHeader
                            WHERE TransactionID=@T
                            ", con, tran)

            cmdHead.Parameters.AddWithValue("@T", transactionID)

            Dim dtHead As New DataTable()
            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(dtHead)
            End Using

            If dtHead.Rows.Count = 0 Then
                Throw New Exception("TransactionHeader not found")
            End If

            postDate = Convert.ToDateTime(dtHead.Rows(0)("PostingDate"))
            operationTypeID = Convert.ToInt32(dtHead.Rows(0)("OperationTypeID"))
        End Using

        '====================================================
        ' IN rows only (TargetStoreID NOT NULL)
        ' ProductID الحقيقي + BaseProductID = ISNULL(BaseProductID, ProductID)
        ' InQty محسوبة m3
        '====================================================
        Dim inRows As New DataTable()

        Dim getInRowsSql As String = "
                            SELECT
                                d.ProductID,
                                ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
                                d.TargetStoreID AS StoreID,

                                SUM(
                                    CASE
                                        WHEN d.UnitID = @M3UnitID THEN d.Quantity
                                        ELSE d.Quantity * (ISNULL(p.Length,0) * ISNULL(p.Width,0) * ISNULL(p.Height,0)) / 1000000.0
                                    END
                                ) AS InQtyM3,

                                SUM(d.Quantity * d.UnitCost) AS InTotalCost,
                                MIN(d.DetailID) AS SourceDetailID
                            FROM Inventory_TransactionDetails d
                            JOIN Master_Product p ON p.ProductID = d.ProductID
                            WHERE d.TransactionID = @TransactionID
                              AND d.TargetStoreID IS NOT NULL
                            GROUP BY
                                d.ProductID,
                                ISNULL(p.BaseProductID, p.ProductID),
                                d.TargetStoreID
                            HAVING SUM(d.Quantity) <> 0
                            "

        Using cmdGet As New SqlCommand(getInRowsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(inRows)
            End Using
        End Using
        Dim seq As Integer = ledgerSequence
        For Each row As DataRow In inRows.Rows

            Dim prodID As Integer = CInt(row("ProductID"))
            Dim baseProdID As Integer = CInt(row("BaseProductID"))
            Dim storeID As Integer = CInt(row("StoreID"))
            Dim inQtyM3 As Decimal = CDec(row("InQtyM3"))
            Dim inTotalCost As Decimal = CDec(row("InTotalCost"))
            Dim sourceDetailID As Integer = CInt(row("SourceDetailID"))

            ' منع التكرار (كما كان)
            Using cmdExists As New SqlCommand("
                            SELECT TOP 1 1
                            FROM Inventory_CostLedger
                            WHERE TransactionID=@T
                              AND ProductID=@P
                              AND BaseProductID=@B
                              AND StoreID=@S
                              AND InQty > 0
                              AND IsActive=1
                              AND IsReversed=0
                            ", con, tran)

                cmdExists.Parameters.AddWithValue("@T", transactionID)
                cmdExists.Parameters.AddWithValue("@P", prodID)
                cmdExists.Parameters.AddWithValue("@B", baseProdID)
                cmdExists.Parameters.AddWithValue("@S", storeID)

                Dim exv = cmdExists.ExecuteScalar()
                If exv IsNot Nothing Then
                    Continue For
                End If
            End Using


            ' سلسلة التكلفة (كما كان)
            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
                                        prodID,
                                        baseProdID,
                                        operationGroupID,
                                        con,
                                        tran,
                                        prevLedgerID,
                                        rootLedgerID,
                                        oldAvgCost
                                    )

            '====================================================
            ' ✅ Global OldQty (m3):
            ' مجموع آخر الكميات لكل المنتجات تحت نفس الأب
            ' عبر كل المستودعات
            ' مع استبعاد المنتج الأب نفسه فقط
            '====================================================
            '====================================================
            ' ✅ Global OldQty (m3) from oldQtyDict (correct, per ProductID)
            '====================================================
            Dim oldQtyGlobalM3 As Decimal = 0D
            If oldQtyDict IsNot Nothing AndAlso oldQtyDict.ContainsKey(prodID) Then
                oldQtyGlobalM3 = oldQtyDict(prodID)
            End If

            Dim newQtyGlobalM3 As Decimal = oldQtyGlobalM3 + inQtyM3

            '====================================================
            ' ✅ Local OldQty (m3):
            ' مجموع آخر الكميات لكل المنتجات تحت نفس الأب
            ' داخل نفس المخزن
            ' مع استبعاد المنتج الأب نفسه فقط
            '====================================================
            Dim localOldQtyM3 As Decimal = 0D

            Using cmdBalLocal As New SqlCommand("
                            SELECT TOP 1 LocalNewQty
                            FROM Inventory_CostLedger
                            WHERE ProductID = @ProductID
                            AND StoreID = @StoreID
                            AND IsActive = 1
                            AND IsReversed = 0
                            AND OperationGroupID <> @OperationGroupID
                            ORDER BY LedgerID DESC
                            ", con, tran)

                cmdBalLocal.Parameters.AddWithValue("@ProductID", prodID)
                cmdBalLocal.Parameters.AddWithValue("@StoreID", storeID)
                cmdBalLocal.Parameters.AddWithValue("@OperationGroupID", operationGroupID)

                Dim v = cmdBalLocal.ExecuteScalar()

                If v IsNot Nothing AndAlso v IsNot DBNull.Value Then
                    localOldQtyM3 = Convert.ToDecimal(v)
                End If

            End Using
            '====================================================
            ' InUnitCost و NewAvgCost (كما كان)
            '====================================================
            Dim inUnitCost As Decimal = 0D
            If inQtyM3 <> 0D Then
                inUnitCost = inTotalCost / inQtyM3
            End If

            Dim newAvgCost As Decimal
            If newQtyGlobalM3 = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQtyGlobalM3 = 0D Then
                newAvgCost = inUnitCost
            Else
                newAvgCost = ((oldQtyGlobalM3 * oldAvgCost) + (inQtyM3 * inUnitCost)) / newQtyGlobalM3
            End If
            Dim localNewQtyM3 As Decimal = localOldQtyM3 + inQtyM3
            '====================================================
            ' Insert Ledger (IN) - نفس شكل OUT عندكم
            ' لكن OldQty/InQty/NewQty/Local... كلها m3
            '====================================================
            Dim sqlInsert As String = "
                            INSERT INTO Inventory_CostLedger
                            (
                                LedgerID,
                                TransactionID,
                                SourceDetailID,
                                ProductID,
                                BaseProductID,
                                StoreID,
                                OperationTypeID,

                                LocalOldQty,
                                LocalNewQty,

                                OldQty,
                                InQty,
                                OutQty,
                                NewQty,
                                OldAvgCost,
                                InUnitCost,
                                InTotalCost,
                                OutUnitCost,
                                OutTotalCost,
                                NewAvgCost,
                                PostingDate,
                                IsReversed,
                                IsRevaluation,
                                RootLedgerID,
                                PrevLedgerID,
                                DependsOnLedgerID,
                                CostSourceType,
                                RootTransactionID,
                                CreatedBy,
                                CreatedAt,
                                OperationGroupID,
                                GroupSeq,
                                ScopeKeyType,
                                ScopeKeyID,
                                LedgerSequence,
                                IsActive
                            )
                            VALUES
                            (
                            NEXT VALUE FOR Seq_CostLedgerID,
                                @TransactionID,
                                @SourceDetailID,
                                @ProductID,
                                @BaseProductID,
                                @StoreID,
                                @OperationTypeID,

                                @LocalOldQty,
                                @LocalNewQty,

                                @OldQtyGlobal,
                                @InQty,
                                0,
                                @OldQtyGlobal + @InQty,
                                @OldAvgCost,
                                @InUnitCost,
                                @InTotalCost,
                                0,
                                0,
                                @NewAvgCost,
                                @PostingDate,
                                0,
                                0,
                                @RootLedgerID,
                                @PrevLedgerID,
                                @PrevLedgerID,
                                @OperationTypeID,
                                @TransactionID,
                                @UserID,
                                SYSDATETIME(),
                                @OperationGroupID,
                                2,
                                'PRODUCT',
                                @ProductID,
                                @LedgerSequence,
                                1
                            )
                            "

            Using cmd As New SqlCommand(sqlInsert, con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQtyM3)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQtyM3)

                cmd.Parameters.AddWithValue("@OldQtyGlobal", oldQtyGlobalM3)
                cmd.Parameters.AddWithValue("@InQty", inQtyM3)
                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)

                cmd.Parameters.AddWithValue("@InUnitCost", inUnitCost)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                cmd.Parameters.AddWithValue("@PostingDate", postDate)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                    cmd.Parameters.AddWithValue("@DependsOnLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                cmd.ExecuteNonQuery()
            End Using
            seq += 1
        Next

    End Sub
    Public Sub InsertCuttingLedgerLinks_M3Only(
    transactionID As Integer,
    operationGroupID As Guid,
    userID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        Const LINK_PROD_OUTPUT As Short = 3
        Const LINK_SCRAP As Short = 9

        ' 1) CuttingID + PostingDate
        Dim cuttingID As Integer
        Dim postingDate As DateTime

        Using cmd As New SqlCommand("
SELECT SourceDocumentID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@T AND OperationTypeID=11
", con, tran)
            cmd.Parameters.AddWithValue("@T", transactionID)
            Using rdr = cmd.ExecuteReader()
                If Not rdr.Read() Then Throw New Exception("CUT header not found for TransactionID=" & transactionID.ToString())
                cuttingID = Convert.ToInt32(rdr("SourceDocumentID"))
                postingDate = Convert.ToDateTime(rdr("PostingDate"))
            End Using
        End Using

        ' 2) Raw OUT detail (TargetStoreID IS NULL) tied to CuttingID
        Dim rawDetailID As Integer
        Dim rawStoreID As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 d.DetailID, d.SourceStoreID
FROM Inventory_TransactionDetails d
WHERE d.TransactionID=@T
  AND d.TargetStoreID IS NULL
  AND d.SourceDocumentDetailID=@CuttingID
  AND d.SourceStoreID IS NOT NULL
ORDER BY d.DetailID
", con, tran)
            cmd.Parameters.AddWithValue("@T", transactionID)
            cmd.Parameters.AddWithValue("@CuttingID", cuttingID)

            Using rdr = cmd.ExecuteReader()
                If Not rdr.Read() Then Throw New Exception("CUT raw OUT detail not found for CuttingID=" & cuttingID.ToString())
                rawDetailID = Convert.ToInt32(rdr("DetailID"))
                rawStoreID = Convert.ToInt32(rdr("SourceStoreID"))
            End Using
        End Using

        ' OUT ledger (source)
        Dim rawLedgerID As Long
        Dim rawOutUnitCost As Decimal
        Using cmd As New SqlCommand("
SELECT TOP 1 LedgerID, ISNULL(OutUnitCost,0) AS OutUnitCost
FROM Inventory_CostLedger
WHERE TransactionID=@T
  AND SourceDetailID=@D
  AND OutQty > 0
  AND IsActive=1
  AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)
            cmd.Parameters.AddWithValue("@T", transactionID)
            cmd.Parameters.AddWithValue("@D", rawDetailID)
            Using rdr = cmd.ExecuteReader()
                If Not rdr.Read() Then Throw New Exception("OUT ledger not found for raw DetailID=" & rawDetailID.ToString())
                rawLedgerID = CLng(rdr("LedgerID"))
                rawOutUnitCost = Convert.ToDecimal(rdr("OutUnitCost"))
            End Using
        End Using

        ' 3) Outputs (good + mix) from CuttingOutput
        Dim outputs As New DataTable()
        Using cmd As New SqlCommand("
SELECT
    co.CutOutputID,
    co.ProductID,
    co.TotalVolume_m3,
    co.IsMix,
    ISNULL(co.TargetStoreID, ISNULL(co.SourceStoreID, 0)) AS StoreID
FROM Production_CuttingOutput co
WHERE co.CutID=@CuttingID
", con, tran)
            cmd.Parameters.AddWithValue("@CuttingID", cuttingID)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(outputs)
            End Using
        End Using

        If outputs.Rows.Count = 0 Then
            Throw New Exception("No CuttingOutput rows found for CuttingID=" & cuttingID.ToString())
        End If

        ' 4) For each output: find IN detail + IN ledger then link with FlowQty=m3
        For Each o As DataRow In outputs.Rows

            Dim cutOutputID As Integer = CInt(o("CutOutputID"))
            Dim prodID As Integer = CInt(o("ProductID"))
            Dim volM3 As Decimal = CDec(o("TotalVolume_m3"))
            Dim isMix As Boolean = (Convert.ToInt32(o("IsMix")) = 1)
            Dim storeTarget As Integer = CInt(o("StoreID"))

            If volM3 = 0D Then Continue For

            ' IN detail row (product output)
            Dim inDetailID As Integer
            Using cmd As New SqlCommand("
SELECT TOP 1 DetailID
FROM Inventory_TransactionDetails
WHERE TransactionID=@T
  AND SourceStoreID IS NULL
  AND SourceDocumentDetailID=@CutOutputID
  AND ProductID=@P
ORDER BY DetailID
", con, tran)
                cmd.Parameters.AddWithValue("@T", transactionID)
                cmd.Parameters.AddWithValue("@CutOutputID", cutOutputID)
                cmd.Parameters.AddWithValue("@P", prodID)

                Dim v = cmd.ExecuteScalar()
                If v Is Nothing OrElse IsDBNull(v) Then
                    Throw New Exception("IN detail not found for CutOutputID=" & cutOutputID.ToString())
                End If
                inDetailID = Convert.ToInt32(v)
            End Using

            ' IN ledger row (target ledger)
            Dim inLedgerID As Long
            Using cmd As New SqlCommand("
SELECT TOP 1 LedgerID
FROM Inventory_CostLedger
WHERE TransactionID=@T
  AND SourceDetailID=@D
  AND InQty > 0
  AND IsActive=1
  AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)
                cmd.Parameters.AddWithValue("@T", transactionID)
                cmd.Parameters.AddWithValue("@D", inDetailID)

                Dim v = cmd.ExecuteScalar()
                If v Is Nothing OrElse IsDBNull(v) Then
                    Throw New Exception("IN ledger not found for IN DetailID=" & inDetailID.ToString())
                End If
                inLedgerID = CLng(v)
            End Using

            Dim linkType As Short = If(isMix, LINK_SCRAP, LINK_PROD_OUTPUT)
            Dim unitCost As Decimal = If(isMix, 0D, rawOutUnitCost)

            ' IMPORTANT: baseProductID now NULL (to be removed later)
            ' SourceTransactionDetailID = rawDetailID
            ' TargetTransactionDetailID = inDetailID
            InsertLedgerLink(
                sourceLedgerID:=rawLedgerID,
                targetLedgerID:=inLedgerID,
                linkType:=linkType,
                qty:=volM3,
                unitCost:=unitCost,
                transactionID:=transactionID,
                storeSource:=rawStoreID,
                storeTarget:=storeTarget,
                productID:=prodID,
                baseProductID:=Nothing,  ' سيُخزن NULL بعد تعديل InsertLedgerLink (baseProductID As Integer?)
                postingDate:=postingDate,
                operationGroupID:=operationGroupID,
                groupSeq:=1,
                userID:=userID,
                con:=con,
                tran:=tran
            )
        Next

    End Sub
    Public Sub ApplyInventoryOutCut(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)

        '=====================================================
        ' خصم الكميات من المستودع المصدر فقط
        '=====================================================

        Dim sqlUpdateQty As String = "

;WITH src AS
(
SELECT
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
d.SourceStoreID AS StoreID,
SUM(d.Quantity) AS Quantity
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
WHERE d.TransactionID = @TransactionID
AND d.SourceStoreID IS NOT NULL
GROUP BY
d.ProductID,
ISNULL(p.BaseProductID,p.ProductID),
d.SourceStoreID
)

UPDATE b
SET
b.QtyOnHand = b.QtyOnHand - src.Quantity,
b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN src
ON src.ProductID = b.ProductID
AND src.BaseProductID = b.BaseProductID
AND src.StoreID = b.StoreID
"

        Dim sqlUpdateMovement As String = "

;WITH LastLedger AS
(
SELECT
cl.ProductID,
cl.BaseProductID,
cl.StoreID,
MAX(cl.LedgerID) AS LastLedgerID
FROM Inventory_CostLedger cl
WHERE cl.TransactionID = @TransactionID
AND cl.OutQty > 0
AND cl.IsReversed = 0
GROUP BY
cl.ProductID,
cl.BaseProductID,
cl.StoreID
)

UPDATE b
SET
b.LastMovementLedgerID = ll.LastLedgerID,
b.LastMovementDate = SYSDATETIME(),
b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN LastLedger ll
ON ll.ProductID = b.ProductID
AND ll.BaseProductID = b.BaseProductID
AND ll.StoreID = b.StoreID
"

        Try

            Using cmd As New SqlCommand(sqlUpdateQty, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using

            Using cmd As New SqlCommand(sqlUpdateMovement, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception

            MessageBox.Show("🔴 [ApplyInventoryOut] فشل التنفيذ:" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            Throw

        End Try

    End Sub
    Public Sub ApplyInventoryInCut(
    transactionID As Integer,
    m3UnitID As Integer,
    ledgerID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        ' =====================================================
        ' تحديث رصيد المخزن الهدف فقط
        ' =====================================================

        Dim sqlMerge As String = "

;WITH src AS
(
    SELECT
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
        d.TargetStoreID AS StoreID,
        SUM(d.Quantity) AS Quantity
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
      AND d.TargetStoreID IS NOT NULL
    GROUP BY
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID),
        d.TargetStoreID
)

MERGE Inventory_Balance AS tgt
USING src
ON  tgt.ProductID      = src.ProductID
AND tgt.BaseProductID  = src.BaseProductID
AND tgt.StoreID        = src.StoreID

WHEN MATCHED THEN
UPDATE SET
    tgt.QtyOnHand     = tgt.QtyOnHand + src.Quantity,
    tgt.LastUpdatedAt = SYSDATETIME()

WHEN NOT MATCHED THEN
INSERT
(
    ProductID,
    BaseProductID,
    StoreID,
    QtyOnHand,
    CreatedAt,
    LastUpdatedAt
)
VALUES
(
    src.ProductID,
    src.BaseProductID,
    src.StoreID,
    src.Quantity,
    SYSDATETIME(),
    SYSDATETIME()
);
"

        Try

            Using cmd As New SqlCommand(sqlMerge, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using


            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN)
            ' ملاحظة: نربط بالحركة عن طريق SourceDetailID (DetailID) لتفادي تعميم أول LedgerID على الجميع
            ' =====================================================

            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN) - مثل أسلوب OUT
            ' =====================================================

            Dim sqlUpdateMovement As String = "

;WITH LastLedger AS
(
    SELECT
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID,
        MAX(cl.LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger cl
    WHERE cl.TransactionID = @TransactionID
      AND cl.InQty > 0
      AND cl.IsReversed = 0
    GROUP BY
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID
)
UPDATE b
SET
    b.LastMovementLedgerID = ll.LastLedgerID,
    b.LastMovementDate = SYSDATETIME(),
    b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN LastLedger ll
  ON ll.ProductID = b.ProductID
 AND ll.BaseProductID = b.BaseProductID
 AND ll.StoreID = b.StoreID;
"
            Using cmdFix As New SqlCommand(sqlUpdateMovement, con, tran)
                cmdFix.Parameters.AddWithValue("@TransactionID", transactionID)
                cmdFix.ExecuteNonQuery()
            End Using


        Catch ex As Exception
            MessageBox.Show("🔴 [ApplyInventoryIn] فشل التنفيذ:" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            Throw
        End Try

    End Sub
    Public Sub UpdateFinalProductAvgCostForFG_FromCutLedger(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        Dim sql As String = "
;WITH CutInfo AS
(
    SELECT ch.BaseProductID
    FROM dbo.Inventory_TransactionHeader th
    JOIN dbo.Production_CuttingHeader ch
        ON ch.CuttingID = th.SourceDocumentID
    WHERE th.TransactionID = @T
      AND th.OperationTypeID = 11
),
LastCutLedger AS
(
    SELECT TOP 1
        cl.BaseProductID,
        cl.NewAvgCost
    FROM dbo.Inventory_CostLedger cl
    JOIN CutInfo ci
        ON ci.BaseProductID = cl.BaseProductID
    WHERE cl.TransactionID = @T
      AND cl.IsReversed = 0
      AND cl.InQty > 0
    ORDER BY cl.LedgerID DESC
)
MERGE dbo.Master_FinalProductAvgCost AS tgt
USING LastCutLedger AS src
ON tgt.BaseProductID = src.BaseProductID
WHEN MATCHED THEN
    UPDATE SET
        tgt.AvgCostPerM3forFG = src.NewAvgCost,
        tgt.LastUpdated = SYSDATETIME()
WHEN NOT MATCHED THEN
    INSERT (BaseProductID, AvgCostPerM3forFG, LastUpdated)
    VALUES (src.BaseProductID, src.NewAvgCost, SYSDATETIME());
"
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@T", transactionID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub



    ''' Return

    Public Function GetOldQtyAllStoresReturn(
    transactionID As Integer,
    operationGroupID As Guid,
    con As SqlConnection,
    tran As SqlTransaction
) As Dictionary(Of Integer, Decimal)

        Dim result As New Dictionary(Of Integer, Decimal)

        Dim sql As String = "
SELECT
    cl.ProductID,
    cl.NewQty
FROM Inventory_CostLedger cl
JOIN
(
    SELECT
        ProductID,
        MAX(LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger
    WHERE IsActive = 1
      AND IsReversed = 0
      AND OperationGroupID <> @OperationGroupID
      AND ProductID IN
      (
        SELECT DISTINCT ProductID
        FROM Inventory_TransactionDetails
        WHERE TransactionID = @TransactionID
      )
    GROUP BY ProductID
) x
ON x.ProductID = cl.ProductID
AND x.LastLedgerID = cl.LedgerID
"

        Using cmd As New SqlCommand(sql, con, tran)

            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)

            Using rd = cmd.ExecuteReader()

                While rd.Read()

                    Dim productID As Integer = CInt(rd("ProductID"))
                    Dim qty As Decimal =
                    If(IsDBNull(rd("NewQty")), 0D, CDec(rd("NewQty")))

                    result(productID) = qty

                End While

            End Using

        End Using

        Return result

    End Function
    Public Function GetProductsByTargetStoreReturn(
    transactionID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim dt As New DataTable()
        Dim sql As String = "
   SELECT 
    d.ProductID,
    p.ProductName,
p.StorageUnitID AS StorageUnitID,
bp.StorageUnitID AS BaseUnitID,
    d.TargetStoreID
FROM Inventory_TransactionDetails d
JOIN Master_Product p 
    ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp 
    ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID = @TransactionID
AND d.TargetStoreID IS NOT NULL
    "

        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt
    End Function
    Public Function InsertCostLedger_M3Return(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    d.TargetStoreID,
    SUM(d.Quantity) AS TrxQty,
    SUM(d.CostAmount) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID = @TransactionID
  AND d.TargetStoreID IS NOT NULL
  AND (
        p.StorageUnitID = @M3UnitID
        OR bp.StorageUnitID = @M3UnitID
      )
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID),
    d.TargetStoreID
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)
                da.Fill(productRows)

                If productRows.Rows.Count > 0 Then
                    Dim r = productRows.Rows(0)
                End If

            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence
        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))

            Dim trxQty As Decimal = Convert.ToDecimal(prodRow("TrxQty"))
            Dim inQty As Decimal = ConvertProductQtyToLedgerQty(prodID, trxQty, m3UnitID, con, tran)



            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' GLOBAL oldQty من Balance لكن نحوله إلى Ledger Unit
            Dim oldQty As Decimal = 0D

            ' LOCAL oldQty من Balance لكن نحوله إلى Ledger Unit
            Dim localOldQtyBalance As Decimal = 0D
            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
  AND BaseProductID=@BaseProductID
  AND StoreID=@StoreID
  AND IsActive=1
  AND IsReversed=0
  AND SupersededByLedgerID IS NULL
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)



                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQtyBalance = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localOldQty As Decimal = localOldQtyBalance
            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal =
    GetReturnUnitCost(sourceDetailID, prodID, con, tran)

            If inAvg = 0D AndAlso inQty <> 0D Then
                inAvg = inTotalCost / inQty
            End If

            inTotalCost = inAvg * inQty

            If oldQtyDict IsNot Nothing AndAlso oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);

", con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)
                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()

                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function
    Public Function InsertCostLedger_RegularReturn(
    transactionID As Integer,
    userID As Integer,
    m3UnitID As Integer,
    operationGroupID As Guid,
     ledgerSequence As Integer,
    oldQtyDict As Dictionary(Of Integer, Decimal),
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Dim firstLedgerID As Integer = 0
        Dim productRows As New DataTable()

        Dim getProductsSql As String = "
SELECT
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
    MIN(d.TargetStoreID) AS TargetStoreID,
    SUM(d.Quantity) AS InQty,
    SUM(d.Quantity*d.UnitCost) AS InTotalCost,
    MIN(d.DetailID) AS SourceDetailID
FROM Inventory_TransactionDetails d
JOIN Master_Product p 
    ON p.ProductID=d.ProductID
LEFT JOIN Master_Product bp
    ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID=@TransactionID
AND d.TargetStoreID IS NOT NULL
AND p.StorageUnitID <> @M3UnitID
AND ISNULL(bp.StorageUnitID,0) <> @M3UnitID
GROUP BY
    d.ProductID,
    ISNULL(p.BaseProductID,p.ProductID)
"

        Using cmdGet As New SqlCommand(getProductsSql, con, tran)
            cmdGet.Parameters.AddWithValue("@TransactionID", transactionID)
            cmdGet.Parameters.AddWithValue("@M3UnitID", m3UnitID)

            Using da As New SqlDataAdapter(cmdGet)

            End Using
        End Using

        Dim headDt As New DataTable()

        Using cmdHead As New SqlCommand("
SELECT OperationTypeID, PostingDate
FROM Inventory_TransactionHeader
WHERE TransactionID=@TransactionID
", con, tran)

            cmdHead.Parameters.AddWithValue("@TransactionID", transactionID)

            Using da As New SqlDataAdapter(cmdHead)
                da.Fill(headDt)
            End Using
        End Using

        If headDt.Rows.Count = 0 Then
            Throw New Exception("TransactionHeader not found")
        End If

        Dim operationTypeID As Integer = Convert.ToInt32(headDt.Rows(0)("OperationTypeID"))
        Dim postingDate As DateTime = Convert.ToDateTime(headDt.Rows(0)("PostingDate"))
        Dim seq As Integer = ledgerSequence

        For Each prodRow As DataRow In productRows.Rows


            Dim prodID As Integer = Convert.ToInt32(prodRow("ProductID"))
            Dim baseProdID As Integer = Convert.ToInt32(prodRow("BaseProductID"))
            Dim targetStoreID As Integer = Convert.ToInt32(prodRow("TargetStoreID"))
            Dim inQty As Decimal = Convert.ToDecimal(prodRow("InQty"))
            Dim inTotalCost As Decimal = Convert.ToDecimal(prodRow("InTotalCost"))
            Dim sourceDetailID As Integer = Convert.ToInt32(prodRow("SourceDetailID"))

            Dim prevLedgerID As Object
            Dim rootLedgerID As Object
            Dim oldAvgCost As Decimal

            GetCostChainContext(
            prodID,
            baseProdID,
            operationGroupID,
            con,
            tran,
            prevLedgerID,
            rootLedgerID,
            oldAvgCost
        )

            ' =====================================================
            ' GLOBAL oldQty (كل المخازن كأنها مستودع واحد)
            ' =====================================================
            Dim oldQty As Decimal = 0D

            If oldQtyDict.ContainsKey(prodID) Then
                oldQty = oldQtyDict(prodID)
            End If


            ' =====================================================
            ' LOCAL oldQty/newQty (المخزن الهدف فقط)
            ' =====================================================
            Dim localOldQty As Decimal = 0D

            Using cmdLocalQty As New SqlCommand("
SELECT TOP 1 LocalNewQty
FROM Inventory_CostLedger
WHERE ProductID=@ProductID
AND BaseProductID=@BaseProductID
AND StoreID=@StoreID
AND IsActive=1
AND IsReversed=0
ORDER BY LedgerID DESC
", con, tran)

                cmdLocalQty.Parameters.AddWithValue("@ProductID", prodID)
                cmdLocalQty.Parameters.AddWithValue("@BaseProductID", baseProdID)
                cmdLocalQty.Parameters.AddWithValue("@StoreID", targetStoreID)

                Dim v = cmdLocalQty.ExecuteScalar()
                localOldQty = If(v Is Nothing OrElse IsDBNull(v), 0D, Convert.ToDecimal(v))
            End Using

            Dim localNewQty As Decimal = localOldQty + inQty

            Dim inAvg As Decimal =
    GetReturnUnitCost(sourceDetailID, prodID, con, tran)

            inTotalCost = inAvg * inQty

            Dim newQty As Decimal = oldQty + inQty
            Dim newAvgCost As Decimal

            If newQty = 0D Then
                newAvgCost = oldAvgCost
            ElseIf oldQty = 0D Then
                newAvgCost = inAvg
            Else
                newAvgCost = ((oldQty * oldAvgCost) + inTotalCost) / newQty
            End If

            Dim insertedLedgerID As Integer = 0

            Using cmd As New SqlCommand("
INSERT INTO Inventory_CostLedger
(
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    IsReversed,
    IsRevaluation,
    StoreID,
    OperationTypeID,
    PostingDate,

    LocalOldQty,
    LocalNewQty,

    OldQty,
    InQty,
    OutQty,
    NewQty,
    OldAvgCost,
    InUnitCost,
    InTotalCost,
    NewAvgCost,
    SourceLedgerID,
    RootLedgerID,
    PrevLedgerID,
    DependsOnLedgerID,
    CostSourceType,
    RootTransactionID,
    CreatedBy,
    CreatedAt,
    OperationGroupID,
    GroupSeq,
    ScopeKeyType,
    ScopeKeyID,
    LedgerSequence,
    IsActive
)
OUTPUT INSERTED.LedgerID
VALUES
(
NEXT VALUE FOR Seq_CostLedgerID,
@TransactionID,
    @SourceDetailID,
    @ProductID,
    @BaseProductID,
    0,
    0,
    @StoreID,
    @OperationTypeID,
    @PostingDate,

    @LocalOldQty,
    @LocalNewQty,

    @OldQty,
    @InQty,
    0,
    @NewQty,
    @OldAvgCost,
    @InAvg,
    @InTotalCost,
    @NewAvgCost,
    @PrevLedgerID,
    @RootLedgerID,
    @PrevLedgerID,
    @PrevLedgerID,
    @OperationTypeID,
    @TransactionID,
    @UserID,
    SYSDATETIME(),
    @OperationGroupID,
    2,
    'PRODUCT',
    @ProductID,
    @LedgerSequence,
    1
);


", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ProductID", prodID)
                cmd.Parameters.AddWithValue("@BaseProductID", baseProdID)

                cmd.Parameters.AddWithValue("@StoreID", targetStoreID)

                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                cmd.Parameters.AddWithValue("@PostingDate", postingDate)

                ' ✅ Local
                cmd.Parameters.AddWithValue("@LocalOldQty", localOldQty)
                cmd.Parameters.AddWithValue("@LocalNewQty", localNewQty)

                ' ✅ Global
                cmd.Parameters.AddWithValue("@OldQty", oldQty)
                cmd.Parameters.AddWithValue("@InQty", inQty)
                cmd.Parameters.AddWithValue("@NewQty", newQty)

                cmd.Parameters.AddWithValue("@OldAvgCost", oldAvgCost)
                cmd.Parameters.AddWithValue("@InAvg", inAvg)
                cmd.Parameters.AddWithValue("@InTotalCost", inTotalCost)
                cmd.Parameters.AddWithValue("@NewAvgCost", newAvgCost)

                If prevLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@PrevLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@PrevLedgerID", CLng(prevLedgerID))
                End If

                If rootLedgerID Is DBNull.Value Then
                    cmd.Parameters.AddWithValue("@RootLedgerID", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("@RootLedgerID", CLng(rootLedgerID))
                End If

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@OperationGroupID", operationGroupID)
                cmd.Parameters.AddWithValue("@LedgerSequence", seq)
                Dim v = cmd.ExecuteScalar()
                insertedLedgerID = If(v Is Nothing OrElse IsDBNull(v), 0, Convert.ToInt32(v))
            End Using
            seq += 1
            If firstLedgerID = 0 Then
                firstLedgerID = insertedLedgerID
            End If

        Next

        Return firstLedgerID

    End Function
    Public Sub ApplyInventoryInReturn(
    transactionID As Integer,
    m3UnitID As Integer,
    ledgerID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
)
        ' =====================================================
        ' تحديث رصيد المخزن الهدف فقط
        ' =====================================================

        Dim sqlMerge As String = "

;WITH src AS
(
    SELECT
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID) AS BaseProductID,
        d.TargetStoreID AS StoreID,
        SUM(d.Quantity) AS Quantity
    FROM Inventory_TransactionDetails d
    JOIN Master_Product p ON p.ProductID = d.ProductID
    WHERE d.TransactionID = @TransactionID
      AND d.TargetStoreID IS NOT NULL
    GROUP BY
        d.ProductID,
        ISNULL(p.BaseProductID,p.ProductID),
        d.TargetStoreID
)

MERGE Inventory_Balance AS tgt
USING src
ON  tgt.ProductID      = src.ProductID
AND tgt.BaseProductID  = src.BaseProductID
AND tgt.StoreID        = src.StoreID

WHEN MATCHED THEN
UPDATE SET
    tgt.QtyOnHand     = tgt.QtyOnHand + src.Quantity,
    tgt.LastUpdatedAt = SYSDATETIME()

WHEN NOT MATCHED THEN
INSERT
(
    ProductID,
    BaseProductID,
    StoreID,
    QtyOnHand,
    CreatedAt,
    LastUpdatedAt
)
VALUES
(
    src.ProductID,
    src.BaseProductID,
    src.StoreID,
    src.Quantity,
    SYSDATETIME(),
    SYSDATETIME()
);
"

        Try

            Using cmd As New SqlCommand(sqlMerge, con, tran)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.ExecuteNonQuery()
            End Using


            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN)
            ' ملاحظة: نربط بالحركة عن طريق SourceDetailID (DetailID) لتفادي تعميم أول LedgerID على الجميع
            ' =====================================================

            ' =====================================================
            ' تحديث معلومات آخر حركة لكل صنف ومستودع (IN) - مثل أسلوب OUT
            ' =====================================================

            Dim sqlUpdateMovement As String = "

;WITH LastLedger AS
(
    SELECT
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID,
        MAX(cl.LedgerID) AS LastLedgerID
    FROM Inventory_CostLedger cl
    WHERE cl.TransactionID = @TransactionID
      AND cl.InQty > 0
      AND cl.IsReversed = 0
    GROUP BY
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID
)
UPDATE b
SET
    b.LastMovementLedgerID = ll.LastLedgerID,
    b.LastMovementDate = SYSDATETIME(),
    b.LastUpdatedAt = SYSDATETIME()
FROM Inventory_Balance b
JOIN LastLedger ll
  ON ll.ProductID = b.ProductID
 AND ll.BaseProductID = b.BaseProductID
 AND ll.StoreID = b.StoreID;
"
            Using cmdFix As New SqlCommand(sqlUpdateMovement, con, tran)
                cmdFix.Parameters.AddWithValue("@TransactionID", transactionID)
                cmdFix.ExecuteNonQuery()
            End Using


        Catch ex As Exception
            MessageBox.Show("🔴 [ApplyInventoryIn] فشل التنفيذ:" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            Throw
        End Try

    End Sub
    Public Sub RecalculateAverage_PUR_PRO_BySnapshotReturn(
    transactionID As Integer,
    m3UnitID As Integer,
    productIds As List(Of Integer),
    con As SqlConnection,
    tran As SqlTransaction
)

        If productIds Is Nothing OrElse productIds.Count = 0 Then Return

        Dim sql As String = "
;WITH P AS
(
    SELECT
        p.ProductID,
        ISNULL(p.BaseProductID, p.ProductID) AS BaseProductID,
        p.StorageUnitID
    FROM dbo.Master_Product p
    WHERE p.ProductID IN (" & String.Join(",", productIds) & ")
),
L AS
(
    SELECT
        cl.ProductID,
        MAX(CASE WHEN cl.InQty > 0 THEN cl.LedgerID END) AS MaxLedgerID
    FROM dbo.Inventory_CostLedger cl
WHERE cl.IsReversed = 0
      AND cl.ProductID IN (" & String.Join(",", productIds) & ")
    GROUP BY cl.ProductID
),
V AS
(
    SELECT
        cl.ProductID,
        cl.NewAvgCost
    FROM dbo.Inventory_CostLedger cl
    JOIN L
      ON L.ProductID = cl.ProductID
     AND L.MaxLedgerID = cl.LedgerID
)
SELECT
    P.ProductID,
    P.BaseProductID,
    P.StorageUnitID,
    ISNULL(V.NewAvgCost, 0) AS NewAvgCost
FROM P
LEFT JOIN V ON V.ProductID = P.ProductID
ORDER BY P.ProductID;
"

        Dim dt As New DataTable()
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        If dt.Rows.Count = 0 Then Return
        ' 2) نفّذ "نقل القيمة" حسب السياسة
        For Each row As DataRow In dt.Rows

            Dim productId As Integer = CInt(row("ProductID"))
            Dim baseProductId As Integer = CInt(row("BaseProductID"))
            Dim storageUnitId As Integer = 0

            If dt.Columns.Contains("StorageUnitID") Then
                storageUnitId = Convert.ToInt32(row("StorageUnitID"))
            End If
            Dim newAvgCost As Decimal = CDec(row("NewAvgCost"))

            Dim isM3 As Boolean = (storageUnitId = m3UnitID)
            Dim hasParent As Boolean = (baseProductId <> productId)

            If isM3 Then
                ' ========= (A) M3 =========
                ' Update then Insert (AvgCostPerM3 NOT NULL)
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_FinalProductAvgCost
SET AvgCostPerM3 = @Val,
    LastUpdated = SYSDATETIME()
WHERE BaseProductID = @BaseProductID;
SELECT @@ROWCOUNT;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@BaseProductID", baseProductId)

                    Dim affected As Integer = CInt(cmdUp.ExecuteScalar())
                    If affected = 0 Then
                        Using cmdIns As New SqlCommand("
INSERT INTO dbo.Master_FinalProductAvgCost
(
    BaseProductID,
    AvgCostPerM3,
    AvgCostPerM3forFG,
    LastUpdated
)
VALUES
(
    @BaseProductID,
    @AvgCostPerM3,
    NULL,
    SYSDATETIME()
);
", con, tran)
                            cmdIns.Parameters.AddWithValue("@BaseProductID", baseProductId)
                            cmdIns.Parameters.AddWithValue("@AvgCostPerM3", newAvgCost)
                            cmdIns.ExecuteNonQuery()
                        End Using
                    End If
                End Using

            ElseIf hasParent Then
                ' ========= (B) Non-M3 + Parent =========
                ' Update then Insert (AvgCostPerM3 NOT NULL => نعطيه 0 عند Insert)
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_FinalProductAvgCost
SET AvgCostPerM3forFG = @Val,
    LastUpdated = SYSDATETIME()
WHERE BaseProductID = @BaseProductID;
SELECT @@ROWCOUNT;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@BaseProductID", baseProductId)

                    Dim affected As Integer = CInt(cmdUp.ExecuteScalar())
                    If affected = 0 Then
                        Using cmdIns As New SqlCommand("
INSERT INTO dbo.Master_FinalProductAvgCost
(
    BaseProductID,
    AvgCostPerM3,
    AvgCostPerM3forFG,
    LastUpdated
)
VALUES
(
    @BaseProductID,
    0,
    @AvgCostPerM3forFG,
    SYSDATETIME()
);
", con, tran)
                            cmdIns.Parameters.AddWithValue("@BaseProductID", baseProductId)
                            cmdIns.Parameters.AddWithValue("@AvgCostPerM3forFG", newAvgCost)
                            cmdIns.ExecuteNonQuery()
                        End Using
                    End If
                End Using

            Else
                ' ========= (C) Non-M3 + No Parent =========
                Using cmdUp As New SqlCommand("
UPDATE dbo.Master_Product
SET AvgCost = @Val
WHERE ProductID = @ProductID;
", con, tran)
                    cmdUp.Parameters.AddWithValue("@Val", newAvgCost)
                    cmdUp.Parameters.AddWithValue("@ProductID", productId)
                    cmdUp.ExecuteNonQuery()
                End Using
            End If

        Next

    End Sub


End Class

