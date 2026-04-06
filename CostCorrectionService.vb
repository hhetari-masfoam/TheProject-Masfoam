

Imports System.Data
Imports System.Data.SqlClient
Imports THE_PROJECT.ProductAverageDTO

Public Class CostCorrectionService

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Private Function SafeDec(val As Object) As Decimal
        If IsDBNull(val) OrElse val Is Nothing Then Return 0D
        Return Convert.ToDecimal(val)
    End Function
    Private Function SafeInt(val As Object) As Integer
        If IsDBNull(val) OrElse val Is Nothing Then Return 0
        Return Convert.ToInt32(val)
    End Function
    Private Function SafeLong(val As Object) As Long
        If IsDBNull(val) OrElse val Is Nothing Then Return 0
        Return Convert.ToInt64(val)
    End Function
    'GENERAL
    Private Function D(val As Object) As Decimal
        If IsDBNull(val) OrElse val Is Nothing Then
            Return 0D
        End If
        Return CDec(val)
    End Function







    'GENERAL OPERATION
    Private Function BuildEditMap(edits As DataTable) As Dictionary(Of Integer, (NewQty As Decimal, NewPrice As Decimal))

        Dim map As New Dictionary(Of Integer, (Decimal, Decimal))

        For Each r As DataRow In edits.Rows

            Dim detailID As Integer = CInt(r("DetailID"))

            Dim newQty As Decimal = CDec(r("NewQty"))
            Dim newPrice As Decimal = CDec(r("NewUnitPrice"))

            map(detailID) = (newQty, newPrice)

        Next

        Return map

    End Function
    Public Function GetAffectedCostDependencies(
    detailIDs As List(Of Integer)
) As DataTable

        Dim result As New DataTable()

        result.Columns.Add("LedgerID", GetType(Long))
        result.Columns.Add("TransactionID", GetType(Long))
        result.Columns.Add("SourceDetailID", GetType(Long))

        result.Columns.Add("ProductID", GetType(Integer))
        result.Columns.Add("BaseProductID", GetType(Integer))
        result.Columns.Add("StoreID", GetType(Integer))
        result.Columns.Add("OperationTypeID", GetType(Integer))

        result.Columns.Add("LocalOldQty", GetType(Decimal))
        result.Columns.Add("OldQty", GetType(Decimal))
        result.Columns.Add("InQty", GetType(Decimal))
        result.Columns.Add("OutQty", GetType(Decimal))
        result.Columns.Add("NewQty", GetType(Decimal))
        result.Columns.Add("LocalNewQty", GetType(Decimal))

        result.Columns.Add("OldAvgCost", GetType(Decimal))
        result.Columns.Add("InUnitCost", GetType(Decimal))
        result.Columns.Add("OutUnitCost", GetType(Decimal))
        result.Columns.Add("NewAvgCost", GetType(Decimal))

        result.Columns.Add("LedgerSequence", GetType(Long))
        result.Columns.Add("SourceLedgerID", GetType(Long))
        result.Columns.Add("SupersededByLedgerID", GetType(Long))
        result.Columns.Add("RootTransactionID", GetType(Long))

        result.Columns.Add("PostingDate", GetType(DateTime))

        If detailIDs Is Nothing OrElse detailIDs.Count = 0 Then
            Return result
        End If

        Dim cleanIds As List(Of Integer) =
        detailIDs.
        Where(Function(x) x > 0).
        Distinct().
        ToList()

        If cleanIds.Count = 0 Then
            Return result
        End If

        Dim idsCsv As String = String.Join(",", cleanIds)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Dim sql As String = "
;WITH ChangedDetails AS
(
    SELECT TRY_CONVERT(INT, value) AS DetailID
    FROM STRING_SPLIT(@DetailIDs, ',')
    WHERE TRY_CONVERT(INT, value) IS NOT NULL
),

StartLedgers AS
(
    SELECT cl.*
    FROM Inventory_CostLedger cl
    WHERE cl.SourceDetailID IN (SELECT DetailID FROM ChangedDetails)
      AND cl.IsActive = 1
      AND cl.IsReversed = 0
),

AffectedLedgers AS
(
    -- 1️⃣ البداية
    SELECT *
    FROM StartLedgers

    UNION ALL

    -- 2️⃣ نفس المنتج (المتوسط)
    SELECT cl.*
    FROM Inventory_CostLedger cl
    INNER JOIN AffectedLedgers a
        ON cl.PrevLedgerID = a.LedgerID
    WHERE cl.IsActive = 1
      AND cl.IsReversed = 0

    UNION ALL

    -- 3️⃣ الروابط (قص / إنتاج / تحويل)
    SELECT l.*
    FROM Inventory_CostLedgerLink link
    INNER JOIN AffectedLedgers a
        ON link.SourceLedgerID = a.LedgerID
    INNER JOIN Inventory_CostLedger l
        ON l.LedgerID = link.TargetLedgerID
    WHERE link.IsActive = 1
      AND link.LinkDirection = 2
      AND l.IsActive = 1
      AND l.IsReversed = 0
)
SELECT
    LedgerID,
    TransactionID,
    SourceDetailID,
    ProductID,
    BaseProductID,
    StoreID,
    OperationTypeID,
    LocalOldQty,
    OldQty,
    InQty,
    OutQty,
    NewQty,
    LocalNewQty,
    OldAvgCost,
    InUnitCost,
    OutUnitCost,
    NewAvgCost,
    LedgerSequence,
    SourceLedgerID,
    SupersededByLedgerID,
    RootTransactionID,
    PostingDate
FROM
(
    SELECT
        cl.LedgerID,
        cl.TransactionID,
        cl.SourceDetailID,
        cl.ProductID,
        cl.BaseProductID,
        cl.StoreID,
        cl.OperationTypeID,
        cl.LocalOldQty,
        cl.OldQty,
        cl.InQty,
        cl.OutQty,
        cl.NewQty,
        cl.LocalNewQty,
        cl.OldAvgCost,
        cl.InUnitCost,
        cl.OutUnitCost,
        cl.NewAvgCost,
        cl.LedgerSequence,
        cl.SourceLedgerID,
        cl.SupersededByLedgerID,
        cl.RootTransactionID,
        cl.PostingDate,
        ROW_NUMBER() OVER
        (
            PARTITION BY cl.LedgerID
            ORDER BY
                cl.PostingDate,
                cl.LedgerSequence,
                cl.LedgerID
        ) AS rn
    FROM AffectedLedgers cl
) x
WHERE rn = 1
ORDER BY
    PostingDate,
    LedgerSequence,
    LedgerID
OPTION (MAXRECURSION 32767);
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@DetailIDs", idsCsv)

                Using r = cmd.ExecuteReader()

                    While r.Read()

                        Dim row = result.NewRow()

                        row("LedgerID") = r("LedgerID")
                        row("TransactionID") = r("TransactionID")

                        row("SourceDetailID") =
                        If(IsDBNull(r("SourceDetailID")),
                           DBNull.Value,
                           r("SourceDetailID"))

                        row("ProductID") = r("ProductID")

                        row("BaseProductID") =
                        If(IsDBNull(r("BaseProductID")),
                           DBNull.Value,
                           r("BaseProductID"))

                        row("StoreID") = r("StoreID")
                        row("OperationTypeID") = r("OperationTypeID")

                        row("LocalOldQty") = r("LocalOldQty")
                        row("OldQty") = r("OldQty")
                        row("InQty") = r("InQty")
                        row("OutQty") = r("OutQty")
                        row("NewQty") = r("NewQty")
                        row("LocalNewQty") = r("LocalNewQty")

                        row("OldAvgCost") = r("OldAvgCost")

                        row("InUnitCost") =
                        If(IsDBNull(r("InUnitCost")), 0D, r("InUnitCost"))

                        row("OutUnitCost") =
                        If(IsDBNull(r("OutUnitCost")), 0D, r("OutUnitCost"))

                        row("NewAvgCost") = r("NewAvgCost")
                        row("LedgerSequence") = r("LedgerSequence")

                        row("SourceLedgerID") =
                        If(IsDBNull(r("SourceLedgerID")),
                           DBNull.Value,
                           r("SourceLedgerID"))


                        row("SupersededByLedgerID") =
                        If(IsDBNull(r("SupersededByLedgerID")),
                           DBNull.Value,
                           r("SupersededByLedgerID"))

                        row("RootTransactionID") =
                        If(IsDBNull(r("RootTransactionID")),
                           DBNull.Value,
                           r("RootTransactionID"))

                        row("PostingDate") = r("PostingDate")

                        result.Rows.Add(row)

                    End While

                End Using
            End Using

        End Using

        Return result

    End Function
    Public Function CreateAffectedPreviewRevalTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("DetailID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("OldUnitPrice", GetType(Decimal))
        dt.Columns.Add("NewUnitPrice", GetType(Decimal))

        Return dt

    End Function
    Public Function GetTransactionDetailsByDetailIDs(detailIDs As List(Of Long)) As DataTable

        Dim dt As New DataTable()

        If detailIDs Is Nothing OrElse detailIDs.Count = 0 Then Return dt

        Using con As New SqlConnection(_connectionString)

            Dim ids As String = String.Join(",", detailIDs)

            Dim sql As String = "
      SELECT 
    d.DetailID,
    d.ProductID,
    p.ProductCode,
    p.ProductName,
    d.Quantity,
    d.UnitCost,
    d.CostAmount,
    d.SourceDocumentDetailID
FROM Inventory_TransactionDetails d
INNER JOIN Master_Product p
    ON p.ProductID = d.ProductID
WHERE d.SourceDocumentDetailID IN (" & ids & ")
        "

            Using cmd As New SqlCommand(sql, con)
                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using

        End Using

        Return dt

    End Function
    Public Function GetAffectedLedgers(detailIDs As List(Of Integer)) As List(Of AvgCostData)

        Dim result As New List(Of AvgCostData)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            ' 1) تحميل كل اللدجرات
            Dim allLedgers As New List(Of AvgCostData)

            Using cmd As New SqlCommand("SELECT * FROM Inventory_CostLedger WHERE IsActive = 1 AND IsReversed = 0", con)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim l As New AvgCostData With {
                            .LedgerID = CLng(rd("LedgerID")),
                            .TransactionID = CInt(rd("TransactionID")),
                            .SourceDetailID = If(IsDBNull(rd("SourceDetailID")), 0, CInt(rd("SourceDetailID"))),
                            .ProductID = CInt(rd("ProductID")),
                            .BaseProductID = CInt(rd("BaseProductID")),
                            .StoreID = CInt(rd("StoreID")),
                            .OperationTypeID = CInt(rd("OperationTypeID")),
                            .PrevLedgerID = If(IsDBNull(rd("PrevLedgerID")), Nothing, CLng(rd("PrevLedgerID"))),
                            .SourceLedgerID = If(IsDBNull(rd("SourceLedgerID")), Nothing, CLng(rd("SourceLedgerID"))),
                            .LocalOldQty = SafeDec(rd("LocalOldQty")),
                            .LocalNewQty = SafeDec(rd("LocalNewQty")),
                        .InQty = SafeDec(rd("InQty")),
                            .OutQty = SafeDec(rd("OutQty")),
                            .NewQty = SafeDec(rd("NewQty")),
                            .OldQty = SafeDec(rd("OldQty")),
                            .InUnitCost = SafeDec(rd("InUnitCost")),
                            .OutUnitCost = SafeDec(rd("OutUnitCost")),
                            .NewAvgCost = SafeDec(rd("NewAvgCost")),
                            .OldAvgCost = SafeDec(rd("OldAvgCost")),
                            .PostingDate = CDate(rd("PostingDate")),
                            .LedgerSequence = CInt(rd("LedgerSequence"))
                        }
                        allLedgers.Add(l)
                    End While
                End Using
            End Using

            ' 2) تحميل اللينك
            Dim links As New List(Of Tuple(Of Long, Long))

            Using cmd As New SqlCommand("SELECT SourceLedgerID, TargetLedgerID FROM Inventory_CostLedgerLink WHERE IsActive = 1", con)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        If Not IsDBNull(rd("SourceLedgerID")) AndAlso Not IsDBNull(rd("TargetLedgerID")) Then
                            links.Add(Tuple.Create(
                                CLng(rd("SourceLedgerID")),
                                CLng(rd("TargetLedgerID"))
                            ))
                        End If
                    End While
                End Using
            End Using

            ' 3) Indexes (مهم للأداء)
            Dim prevIndex = allLedgers.
                Where(Function(x) x.PrevLedgerID.HasValue).
                GroupBy(Function(x) x.PrevLedgerID.Value).
                ToDictionary(Function(g) g.Key, Function(g) g.ToList())

            Dim sourceIndex = allLedgers.
                Where(Function(x) x.SourceLedgerID.HasValue).
                GroupBy(Function(x) x.SourceLedgerID.Value).
                ToDictionary(Function(g) g.Key, Function(g) g.ToList())

            Dim linkIndex = links.
                GroupBy(Function(x) x.Item1).
                ToDictionary(Function(g) g.Key, Function(g) g.Select(Function(x) x.Item2).ToList())

            Dim ledgerMap = allLedgers.ToDictionary(Function(x) x.LedgerID)

            ' 4) نقطة البداية
            Dim start = allLedgers.
                Where(Function(x) detailIDs.Contains(x.SourceDetailID)).
                ToList()

            Dim visited As New HashSet(Of Long)
            Dim queue As New Queue(Of AvgCostData)

            For Each s In start
                queue.Enqueue(s)
                visited.Add(s.LedgerID)
            Next

            ' 5) BFS Traversal
            While queue.Count > 0

                Dim current = queue.Dequeue()
                result.Add(current)

                ' 1) Prev
                If prevIndex.ContainsKey(current.LedgerID) Then

                    For Each nxt In prevIndex(current.LedgerID)

                        ' 🔥 شرط المنتج
                        If nxt.BaseProductID <> current.BaseProductID Then Continue For
                        If nxt.ProductID <> current.ProductID Then Continue For
                        If Not visited.Contains(nxt.LedgerID) Then
                            visited.Add(nxt.LedgerID)
                            queue.Enqueue(nxt)
                        End If
                    Next
                End If

                ' 2) Source (returns)
                If sourceIndex.ContainsKey(current.LedgerID) Then

                    For Each nxt In sourceIndex(current.LedgerID)
                        If nxt.ProductID <> current.ProductID Then Continue For
                        If Not visited.Contains(nxt.LedgerID) Then
                            visited.Add(nxt.LedgerID)
                            queue.Enqueue(nxt)
                        End If
                    Next
                End If

                ' 3) Link (flow)
                If linkIndex.ContainsKey(current.LedgerID) Then
                    For Each targetID In linkIndex(current.LedgerID)

                        If ledgerMap.ContainsKey(targetID) Then
                            Dim nxt = ledgerMap(targetID)
                            If Not visited.Contains(nxt.LedgerID) Then
                                visited.Add(nxt.LedgerID)
                                queue.Enqueue(nxt)
                            End If
                        End If
                    Next
                End If

            End While

            ' 6) ترتيب
            Return result.
                OrderBy(Function(x) x.PostingDate).
                ThenBy(Function(x) x.LedgerSequence).
                ThenBy(Function(x) x.LedgerID).
                ToList()

        End Using

    End Function


    'Purchases
    Public Sub EnqueueCorrection(documentID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    '========================
                    ' جلب TransactionDetails
                    '========================
                    Dim dt As New DataTable()

                    Using da As New SqlDataAdapter("
SELECT 
    td.DetailID AS TransactionDetailID,
    td.SourceDocumentDetailID AS DocumentDetailID,
    MIN(l.LedgerID) AS StartLedgerID
FROM Inventory_TransactionDetails td
LEFT JOIN Inventory_CostLedger l
    ON l.SourceDetailID = td.DetailID
    AND l.IsActive = 1
    AND l.IsReversed = 0
WHERE td.TransactionID IN (
    SELECT TransactionID 
    FROM Inventory_TransactionHeader
    WHERE SourceDocumentID = @DocID
)
GROUP BY td.DetailID, td.SourceDocumentDetailID
", con)

                        da.SelectCommand.Parameters.AddWithValue("@DocID", documentID)
                        da.SelectCommand.Transaction = tran
                        da.Fill(dt)

                    End Using

                    '========================
                    ' Upsert Queue
                    '========================
                    For Each r As DataRow In dt.Rows

                        Dim trnDetailID As Long = CLng(r("TransactionDetailID"))
                        Dim docDetailID As Long = CLng(r("DocumentDetailID"))
                        Dim ledgerID As Object =
                            If(IsDBNull(r("StartLedgerID")), DBNull.Value, r("StartLedgerID"))

                        ' UPDATE
                        Using cmd As New SqlCommand("
UPDATE Inventory_CorrectionQueue
SET 
    DocumentDetailID = @DocDetailID,
    StartLedgerID = @LedgerID,
    StatusID = 22,
    LastUpdatedAt = SYSDATETIME(),
    ErrorMessage = NULL
WHERE TransactionDetailID = @TrnDetailID
", con, tran)

                            cmd.Parameters.AddWithValue("@TrnDetailID", trnDetailID)
                            cmd.Parameters.AddWithValue("@DocDetailID", docDetailID)
                            cmd.Parameters.AddWithValue("@LedgerID", ledgerID)

                            Dim rows = cmd.ExecuteNonQuery()

                            ' INSERT إذا غير موجود
                            If rows = 0 Then

                                Using cmdInsert As New SqlCommand("
INSERT INTO Inventory_CorrectionQueue
(TransactionDetailID, DocumentDetailID, StartLedgerID, StatusID, ScopeCode)
VALUES
(@TrnDetailID, @DocDetailID, @LedgerID, 22, 'COR')
", con, tran)

                                    cmdInsert.Parameters.AddWithValue("@TrnDetailID", trnDetailID)
                                    cmdInsert.Parameters.AddWithValue("@DocDetailID", docDetailID)
                                    cmdInsert.Parameters.AddWithValue("@LedgerID", ledgerID)

                                    cmdInsert.ExecuteNonQuery()

                                End Using

                            End If

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

    Public Function GetCorrectionQueue() As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(_connectionString)

            Dim sql As String = "
SELECT 
    q.ID,
    q.TransactionDetailID,
    q.DocumentDetailID,
    q.StartLedgerID,
td.CorrectionReferenceDetailID,
    th.OperationTypeID,
    ot.OperationName,

    q.StatusID,
    s.StatusName,

    q.CreatedAt,
    q.LastUpdatedAt,
    q.ErrorMessage

FROM Inventory_CorrectionQueue q

LEFT JOIN Inventory_TransactionDetails td
    ON td.DetailID = q.TransactionDetailID

LEFT JOIN Inventory_TransactionHeader th
    ON th.TransactionID = td.TransactionID

LEFT JOIN Workflow_OperationType ot
    ON ot.OperationTypeID = th.OperationTypeID

LEFT JOIN Workflow_Status s
    ON s.StatusID = q.StatusID


ORDER BY q.ID DESC
"

            Using cmd As New SqlCommand(sql, con)
                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using

        End Using

        Return dt

    End Function

End Class
