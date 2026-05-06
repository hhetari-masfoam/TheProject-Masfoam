Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq

Public Class CostCorrectionEngine

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub
#Region "DTOs"

    Private Class LedgerNode
        Public Property LedgerID As Long
        Public Property LedgerSequence As Long
        Public Property TransactionID As Integer
        Public Property SourceDetailID As Integer?
        Public Property DocumentDetailID As Integer?
        Public Property ProductID As Integer
        Public Property BaseProductID As Integer?
        Public Property StoreID As Integer
        Public Property OperationTypeID As Integer
        Public Property LocalOldQty As Decimal
        Public Property OldQty As Decimal
        Public Property InQty As Decimal
        Public Property OutQty As Decimal
        Public Property NewQty As Decimal
        Public Property LocalNewQty As Decimal
        Public Property OldAvgCost As Decimal
        Public Property InUnitCost As Decimal
        Public Property InTotalCost As Decimal
        Public Property OutUnitCost As Decimal
        Public Property OutTotalCost As Decimal
        Public Property NewAvgCost As Decimal
        Public Property SourceLedgerID As Long?
        Public Property PrevLedgerID As Long?
        Public Property PostingDate As DateTime?
        Public Property IsActive As Boolean
        Public Property IsReversed As Boolean
        Public Property OperationGroupID As Guid?
        Public Property GroupSeq As Integer?
        Public Property CostPoolOldQty As Decimal
        Public Property CostPoolInQty As Decimal
        Public Property CostPoolOutQty As Decimal
        Public Property CostPoolNewQty As Decimal
        Public Property CostPoolOldAvgCost As Decimal
        Public Property CostPoolNewAvgCost As Decimal
    End Class

    Private Class LinkEdge
        Public Property LinkID As Long
        Public Property TargetLedgerID As Long?
        Public Property SourceLedgerID As Long?
        Public Property LinkType As Short
        Public Property FlowQty As Decimal
        Public Property FlowUnitCost As Decimal
        Public Property FlowTotalCost As Decimal
        Public Property SourceTransactionDetailID As Integer?
        Public Property TargetTransactionDetailID As Integer?
        Public Property PostingDate As DateTime?
        Public Property OperationGroupID As Guid?
        Public Property GroupSeq As Integer?
        Public Property LinkDirection As Byte?
        Public Property IsActive As Boolean
    End Class

    Private Class CorrectionEntry
        Public Property TransactionDetailID As Integer
        Public Property NewQuantity As Decimal?
        Public Property NewUnitCost As Decimal?
    End Class

    Private Class SimLedgerState
        Public Property LedgerID As Long
        Public Property ProductID As Integer
        Public Property StoreID As Integer

        Public Property OldQty As Decimal
        Public Property NewQty As Decimal

        Public Property InQty As Decimal
        Public Property OutQty As Decimal

        Public Property LocalOldQty As Decimal
        Public Property LocalNewQty As Decimal

        Public Property OldAvgCost As Decimal
        Public Property NewAvgCost As Decimal

        Public Property InUnitCost As Decimal
        Public Property OutUnitCost As Decimal
        Public Property CostPoolOldQty As Decimal?
        Public Property CostPoolInQty As Decimal?
        Public Property CostPoolOutQty As Decimal?
        Public Property CostPoolNewQty As Decimal?

        Public Property CostPoolOldAvgCost As Decimal?
        Public Property CostPoolNewAvgCost As Decimal?

    End Class

    Private Class RebuildContext
        Public Property AllLedgers As Dictionary(Of Long, LedgerNode)
        Public Property ActiveLedgers As Dictionary(Of Long, LedgerNode)
        Public Property AllLinks As List(Of LinkEdge)

        Public Property IncomingLinksByTarget As Dictionary(Of Long, List(Of LinkEdge))
        Public Property OutgoingLinksBySource As Dictionary(Of Long, List(Of LinkEdge))

        Public Property CorrectionsByTransactionDetail As Dictionary(Of Integer, CorrectionEntry)
        Public Property SortedLedgerIds As List(Of Long)
    End Class

#End Region

#Region "Public API"

    ' يشغّل المحرك على كل التصحيحات المعلقة في الطابور
    Public Function SimulatePendingCorrections() As DataTable

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Dim startLedgerIds = LoadPendingStartLedgerIds(con)

            If startLedgerIds.Count = 0 Then
                Return CreateResultTable()
            End If

            Dim ctx = LoadRebuildContext(startLedgerIds, con)
            Dim states = SimulateContext(ctx)

            Return BuildResultTable(ctx, states)

        End Using

    End Function

    ' يشغّل المحرك على ليدجرات بداية محددة
    Public Function SimulateByStartLedgerIds(startLedgerIds As IEnumerable(Of Long)) As DataTable

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Dim cleaned = startLedgerIds.
                Where(Function(x) x > 0).
                Distinct().
                ToList()

            If cleaned.Count = 0 Then
                Return CreateResultTable()
            End If

            Dim ctx = LoadRebuildContext(cleaned, con)
            Dim states = SimulateContext(ctx)

            Return BuildResultTable(ctx, states)

        End Using

    End Function

    Public Function GetCorrectionQueue() As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(_connectionString)

            Dim sql As String = "
SELECT 
    q.ID,
    q.DocumentDetailID,
    q.StartLedgerID,
    q.StatusID,
    s.StatusName,
    q.CreatedAt,
    q.ErrorMessage,
    q.ScopeCode,
    q.ChangeType,
    q.ProductID AS QueueProductID,
    q.NewQuantity,
    q.NewUnitCost,

    dd.UnitID AS DocumentUnitID,
    dd.DetailID,

    CASE 
    WHEN ISNULL(cl.InQty,0) > 0 THEN cl.InQty
    WHEN ISNULL(cl.OutQty,0) > 0 THEN cl.OutQty
    ELSE 0
END AS OldQuantity,
    CASE 
    WHEN ISNULL(cl.InUnitCost,0) > 0 THEN cl.InUnitCost
    WHEN ISNULL(cl.OutUnitCost,0) > 0 THEN cl.OutUnitCost
    ELSE 0
END AS OldCost,


    td.DetailID AS TransactionDetailID,
    td.TransactionID,
    td.SourceDocumentDetailID,

    th.OperationTypeID,
    mp.ProductCode,
    ms.UnitName

FROM inv.CorrectionQueue q

LEFT JOIN inv.DocumentDetails dd
    ON dd.DetailID = q.DocumentDetailID

LEFT JOIN inv.TransactionDetails td
    ON td.SourceDocumentDetailID = q.DocumentDetailID

LEFT JOIN inv.TransactionHeader th
    ON th.TransactionID = td.TransactionID

LEFT JOIN inv.CostLedger cl
    ON cl.LedgerID = q.StartLedgerID

LEFT JOIN wf.OperationType ot
    ON ot.OperationTypeID = th.OperationTypeID

LEFT JOIN md.Product mp
    ON mp.ProductID = q.ProductID
LEFT JOIN md.Unit ms
    ON ms.UnitID = dd.UnitID
LEFT JOIN wf.Status s
    ON s.StatusID = q.StatusID
WHERE q.StatusID = 22
ORDER BY q.ID DESC
"

            Using cmd As New SqlCommand(sql, con)
                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using

        End Using

        Return dt

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
    FROM inv.CostLedger cl
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
    FROM inv.CostLedger cl
    INNER JOIN AffectedLedgers a
        ON cl.PrevLedgerID = a.LedgerID
    WHERE cl.IsActive = 1
      AND cl.IsReversed = 0

    UNION ALL

    -- 3️⃣ الروابط (قص / إنتاج / تحويل)
    SELECT l.*
    FROM inv.CostLedgerLink link
    INNER JOIN AffectedLedgers a
        ON link.SourceLedgerID = a.LedgerID
    INNER JOIN inv.CostLedger l
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
    Public Function GetUnifiedAffectedGraph(detailIDs As List(Of Integer)) As DataTable

        Dim result As New DataTable()

        ' نفس الأعمدة
        result.Columns.Add("LedgerID", GetType(Long))
        result.Columns.Add("TransactionID", GetType(Long))
        result.Columns.Add("SourceDetailID", GetType(Long))
        result.Columns.Add("ProductID", GetType(Integer))
        result.Columns.Add("StoreID", GetType(Integer))
        result.Columns.Add("LocalOldQty", GetType(Decimal))
        result.Columns.Add("LocalNewQty", GetType(Decimal))
        result.Columns.Add("OldQty", GetType(Decimal))
        result.Columns.Add("InQty", GetType(Decimal))
        result.Columns.Add("OutQty", GetType(Decimal))
        result.Columns.Add("NewQty", GetType(Decimal))
        result.Columns.Add("OldAvgCost", GetType(Decimal))
        result.Columns.Add("InUnitCost", GetType(Decimal))
        result.Columns.Add("OutUnitCost", GetType(Decimal))
        result.Columns.Add("NewAvgCost", GetType(Decimal))
        result.Columns.Add("PostingDate", GetType(DateTime))
        result.Columns.Add("LedgerSequence", GetType(Integer))
        result.Columns.Add("PrevLedgerID", GetType(Long))
        result.Columns.Add("OperationGroupID", GetType(String))
        result.Columns.Add("GroupSeq", GetType(Integer))
        result.Columns.Add("DocumentDetailID", GetType(Integer))

        If detailIDs Is Nothing OrElse detailIDs.Count = 0 Then
            Return result
        End If

        Dim idsCsv As String = String.Join(",", detailIDs)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Dim sql As String = "
;WITH ChangedDetails AS
(
    SELECT TRY_CONVERT(INT, value) AS DetailID
    FROM STRING_SPLIT(@DetailIDs, ',')
),
StartLedgers AS
(
    SELECT *
    FROM inv.CostLedger
    WHERE SourceDetailID IN (SELECT DetailID FROM ChangedDetails)
      AND IsActive = 1 AND IsReversed = 0
),
Affected AS
(
    SELECT * FROM StartLedgers

    UNION ALL

    -- نفس المنتج (Prev)
    SELECT cl.*
    FROM inv.CostLedger cl
    INNER JOIN Affected a
        ON cl.PrevLedgerID = a.LedgerID
    WHERE cl.IsActive = 1 AND cl.IsReversed = 0

    UNION ALL

    -- عبر الروابط
    SELECT l.*
    FROM inv.CostLedgerLink link
    INNER JOIN Affected a
        ON link.SourceLedgerID = a.LedgerID
    INNER JOIN inv.CostLedger l
        ON l.LedgerID = link.TargetLedgerID
    WHERE link.IsActive = 1
      AND l.IsActive = 1
      AND l.IsReversed = 0
)

SELECT DISTINCT
    td.SourceDocumentDetailID AS DocumentDetailID,
    cl.LedgerID,
    cl.TransactionID,
    cl.SourceDetailID,
    cl.ProductID,
    cl.StoreID,
    cl.LocalOldQty,
    cl.LocalNewQty,
    cl.OldQty,
    cl.InQty,
    cl.OutQty,
    cl.NewQty,
    cl.OldAvgCost,
    cl.InUnitCost,
    cl.OutUnitCost,
    cl.NewAvgCost,
    cl.PostingDate,
    cl.GroupSeq,
    cl.OperationGroupID,
    cl.PrevLedgerID,
    cl.LedgerSequence
FROM Affected cl
LEFT JOIN inv.TransactionDetails td
    ON td.DetailID = cl.SourceDetailID
ORDER BY 
    cl.PostingDate,
    cl.OperationGroupID,
    cl.GroupSeq,
    cl.LedgerSequence,
    cl.LedgerID
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@DetailIDs", idsCsv)

                Using r = cmd.ExecuteReader()
                    result.Load(r)
                End Using
            End Using

        End Using

        Return result

    End Function


#End Region

#Region "Context Loading"

    Private Function LoadRebuildContext(startLedgerIds As IEnumerable(Of Long),
                                        con As SqlConnection) As RebuildContext

        Dim allLedgers = LoadAllActiveLedgers(con)
        Dim allLinks = LoadAllActiveLinks(con)
        Dim corrections = LoadPendingCorrections(con)

        Dim closure = ExpandCostClosure(startLedgerIds, allLedgers, allLinks)

        Dim activeLedgers = allLedgers.
            Where(Function(kv) closure.Contains(kv.Key)).
            ToDictionary(Function(kv) kv.Key, Function(kv) kv.Value)

        Dim sortedLedgerIds = activeLedgers.Values.
            OrderBy(Function(x) If(x.PostingDate.HasValue, x.PostingDate.Value, Date.MinValue)).
            ThenBy(Function(x) If(x.OperationGroupID.HasValue, x.OperationGroupID.Value.ToString(), "")).
            ThenBy(Function(x) If(x.GroupSeq.HasValue, x.GroupSeq.Value, Integer.MinValue)).
            ThenBy(Function(x) x.LedgerSequence).
            ThenBy(Function(x) x.LedgerID).
            Select(Function(x) x.LedgerID).
            ToList()

        Dim incoming = BuildIncomingLinksByTarget(allLinks)
        Dim outgoing = BuildOutgoingLinksBySource(allLinks)

        Return New RebuildContext With {
            .AllLedgers = allLedgers,
            .ActiveLedgers = activeLedgers,
            .AllLinks = allLinks,
            .IncomingLinksByTarget = incoming,
            .OutgoingLinksBySource = outgoing,
            .CorrectionsByTransactionDetail = corrections,
            .SortedLedgerIds = sortedLedgerIds
        }

    End Function

    Private Function LoadPendingStartLedgerIds(con As SqlConnection) As List(Of Long)

        Dim result As New List(Of Long)

        Using cmd As New SqlCommand("
SELECT DISTINCT StartLedgerID
FROM inv.CorrectionQueue
WHERE StatusID = 22
  AND StartLedgerID IS NOT NULL
", con)

            Using rd = cmd.ExecuteReader()
                While rd.Read()
                    result.Add(CLng(rd("StartLedgerID")))
                End While
            End Using

        End Using

        Return result

    End Function

    Private Function LoadPendingCorrections(con As SqlConnection) As Dictionary(Of Integer, CorrectionEntry)

        Dim result As New Dictionary(Of Integer, CorrectionEntry)

        Using cmd As New SqlCommand("
SELECT
    TransactionDetailID,
    NewQuantity,
    NewUnitCost
FROM inv.CorrectionQueue
WHERE StatusID = 22
  AND TransactionDetailID IS NOT NULL
", con)

            Using rd = cmd.ExecuteReader()
                While rd.Read()

                    Dim docId As Integer = CInt(rd("TransactionDetailID"))

                    Dim entry As New CorrectionEntry With {
                        .TransactionDetailID = docId,
                        .NewQuantity = If(IsDBNull(rd("NewQuantity")), CType(Nothing, Decimal?), CDec(rd("NewQuantity"))),
                        .NewUnitCost = If(IsDBNull(rd("NewUnitCost")), CType(Nothing, Decimal?), CDec(rd("NewUnitCost")))
                    }

                    result(docId) = entry

                End While
            End Using

        End Using

        Return result

    End Function

    Private Function LoadAllActiveLedgers(con As SqlConnection) As Dictionary(Of Long, LedgerNode)

        Dim result As New Dictionary(Of Long, LedgerNode)

        Using cmd As New SqlCommand("
SELECT
    cl.LedgerID,
    cl.LedgerSequence,
    cl.TransactionID,
    cl.SourceDetailID,
    td.SourceDocumentDetailID AS DocumentDetailID,
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
    cl.InTotalCost,
    cl.OutUnitCost,
    cl.OutTotalCost,
    cl.NewAvgCost,
    cl.SourceLedgerID,
    cl.PrevLedgerID,
    cl.PostingDate,
    ISNULL(cl.IsActive, 0) AS IsActive,
    ISNULL(cl.IsReversed, 0) AS IsReversed,
    cl.OperationGroupID,
    cl.CostPoolOldQty,
    cl.CostPoolInQty,
    cl.CostPoolOutQty,
    cl.CostPoolNewQty,
    cl.CostPoolOldAvgCost,
    cl.CostPoolNewAvgCost,
    cl.GroupSeq
FROM inv.CostLedger cl
LEFT JOIN inv.TransactionDetails td
    ON td.DetailID = cl.SourceDetailID
WHERE ISNULL(cl.IsActive, 0) = 1
  AND ISNULL(cl.IsReversed, 0) = 0
", con)

            Using rd = cmd.ExecuteReader()
                While rd.Read()

                    Dim item As New LedgerNode With {
                        .LedgerID = CLng(rd("LedgerID")),
                        .LedgerSequence = If(IsDBNull(rd("LedgerSequence")), 0L, CLng(rd("LedgerSequence"))),
                        .TransactionID = If(IsDBNull(rd("TransactionID")), 0, CInt(rd("TransactionID"))),
                        .SourceDetailID = If(IsDBNull(rd("SourceDetailID")), CType(Nothing, Integer?), CInt(rd("SourceDetailID"))),
                        .DocumentDetailID = If(IsDBNull(rd("DocumentDetailID")), CType(Nothing, Integer?), CInt(rd("DocumentDetailID"))),
                        .ProductID = If(IsDBNull(rd("ProductID")), 0, CInt(rd("ProductID"))),
                        .BaseProductID = If(IsDBNull(rd("BaseProductID")), CType(Nothing, Integer?), CInt(rd("BaseProductID"))),
                        .StoreID = If(IsDBNull(rd("StoreID")), 0, CInt(rd("StoreID"))),
                        .OperationTypeID = If(IsDBNull(rd("OperationTypeID")), 0, CInt(rd("OperationTypeID"))),
                        .LocalOldQty = SafeDec(rd("LocalOldQty")),
                        .OldQty = SafeDec(rd("OldQty")),
                        .InQty = SafeDec(rd("InQty")),
                        .OutQty = SafeDec(rd("OutQty")),
                        .NewQty = SafeDec(rd("NewQty")),
                        .LocalNewQty = SafeDec(rd("LocalNewQty")),
                        .OldAvgCost = SafeDec(rd("OldAvgCost")),
                        .InUnitCost = SafeDec(rd("InUnitCost")),
                        .InTotalCost = SafeDec(rd("InTotalCost")),
                        .OutUnitCost = SafeDec(rd("OutUnitCost")),
                        .OutTotalCost = SafeDec(rd("OutTotalCost")),
                        .NewAvgCost = SafeDec(rd("NewAvgCost")),
                        .SourceLedgerID = If(IsDBNull(rd("SourceLedgerID")), CType(Nothing, Long?), CLng(rd("SourceLedgerID"))),
                        .PrevLedgerID = If(IsDBNull(rd("PrevLedgerID")), CType(Nothing, Long?), CLng(rd("PrevLedgerID"))),
                        .PostingDate = If(IsDBNull(rd("PostingDate")), CType(Nothing, DateTime?), CDate(rd("PostingDate"))),
                        .IsActive = CBool(rd("IsActive")),
                        .IsReversed = CBool(rd("IsReversed")),
                        .OperationGroupID = If(IsDBNull(rd("OperationGroupID")), CType(Nothing, Guid?), CType(rd("OperationGroupID"), Guid)),
                        .CostPoolOldQty = SafeDec(rd("CostPoolOldQty")),
                        .CostPoolInQty = SafeDec(rd("CostPoolInQty")),
                        .CostPoolOutQty = SafeDec(rd("CostPoolOutQty")),
                        .CostPoolNewQty = SafeDec(rd("CostPoolNewQty")),
                        .CostPoolOldAvgCost = SafeDec(rd("CostPoolOldAvgCost")),
                        .CostPoolNewAvgCost = SafeDec(rd("CostPoolNewAvgCost")),
                        .GroupSeq = If(IsDBNull(rd("GroupSeq")), CType(Nothing, Integer?), CInt(rd("GroupSeq")))
                    }

                    result(item.LedgerID) = item

                End While
            End Using

        End Using

        Return result

    End Function

    Private Function LoadAllActiveLinks(con As SqlConnection) As List(Of LinkEdge)

        Dim result As New List(Of LinkEdge)

        Using cmd As New SqlCommand("
SELECT
    LinkID,
    TargetLedgerID,
    SourceLedgerID,
    LinkType,
    FlowQty,
    FlowUnitCost,
    FlowTotalCost,
    SourceTransactionDetailID,
    TargetTransactionDetailID,
    PostingDate,
    OperationGroupID,
    GroupSeq,
    LinkDirection,
    IsActive
FROM inv.CostLedgerLink
WHERE IsActive = 1
", con)

            Using rd = cmd.ExecuteReader()
                While rd.Read()

                    Dim item As New LinkEdge With {
                        .LinkID = CLng(rd("LinkID")),
                        .TargetLedgerID = If(IsDBNull(rd("TargetLedgerID")), CType(Nothing, Long?), CLng(rd("TargetLedgerID"))),
                        .SourceLedgerID = If(IsDBNull(rd("SourceLedgerID")), CType(Nothing, Long?), CLng(rd("SourceLedgerID"))),
                        .LinkType = CShort(rd("LinkType")),
                        .FlowQty = SafeDec(rd("FlowQty")),
                        .FlowUnitCost = SafeDec(rd("FlowUnitCost")),
                        .FlowTotalCost = SafeDec(rd("FlowTotalCost")),
                        .SourceTransactionDetailID = If(IsDBNull(rd("SourceTransactionDetailID")), CType(Nothing, Integer?), CInt(rd("SourceTransactionDetailID"))),
                        .TargetTransactionDetailID = If(IsDBNull(rd("TargetTransactionDetailID")), CType(Nothing, Integer?), CInt(rd("TargetTransactionDetailID"))),
                        .PostingDate = If(IsDBNull(rd("PostingDate")), CType(Nothing, DateTime?), CDate(rd("PostingDate"))),
                        .OperationGroupID = If(IsDBNull(rd("OperationGroupID")), CType(Nothing, Guid?), CType(rd("OperationGroupID"), Guid)),
                        .GroupSeq = If(IsDBNull(rd("GroupSeq")), CType(Nothing, Integer?), CInt(rd("GroupSeq"))),
                        .LinkDirection = If(IsDBNull(rd("LinkDirection")), CType(Nothing, Byte?), CByte(rd("LinkDirection"))),
                        .IsActive = CBool(rd("IsActive"))
                    }

                    result.Add(item)

                End While
            End Using

        End Using

        Return result

    End Function

#End Region

#Region "Closure Graph"

    Private Function ExpandCostClosure(startLedgerIds As IEnumerable(Of Long),
                                       allLedgers As Dictionary(Of Long, LedgerNode),
                                       allLinks As List(Of LinkEdge)) As HashSet(Of Long)

        Dim closure As New HashSet(Of Long)(
            startLedgerIds.Where(Function(x) allLedgers.ContainsKey(x))
        )

        Dim nextByPrev As New Dictionary(Of Long, List(Of Long))
        For Each led In allLedgers.Values
            If led.PrevLedgerID.HasValue Then
                If Not nextByPrev.ContainsKey(led.PrevLedgerID.Value) Then
                    nextByPrev(led.PrevLedgerID.Value) = New List(Of Long)
                End If
                nextByPrev(led.PrevLedgerID.Value).Add(led.LedgerID)
            End If
        Next

        Dim incomingByTarget = BuildIncomingLinksByTarget(allLinks)
        Dim outgoingBySource = BuildOutgoingLinksBySource(allLinks)

        Dim q As New Queue(Of Long)(closure)

        While q.Count > 0

            Dim currentId = q.Dequeue()

            ' 1) امتداد السلسلة للأمام
            If nextByPrev.ContainsKey(currentId) Then
                For Each nxt In nextByPrev(currentId)
                    If closure.Add(nxt) Then q.Enqueue(nxt)
                Next
            End If

            ' 2) الأهداف الخارجة من هذا المصدر
            If outgoingBySource.ContainsKey(currentId) Then
                For Each lk In outgoingBySource(currentId)
                    If lk.TargetLedgerID.HasValue AndAlso allLedgers.ContainsKey(lk.TargetLedgerID.Value) Then
                        If closure.Add(lk.TargetLedgerID.Value) Then
                            q.Enqueue(lk.TargetLedgerID.Value)
                        End If
                    End If
                Next
            End If

            ' 3) كل المصادر الداخلة لهذا الهدف
            ' 3) كل المصادر الداخلة لهذا الهدف
            ' نضيفها إلى الـ closure فقط كـ support nodes
            ' لكن لا نسمح لها بفتح سلسلة forward مستقلة
            If incomingByTarget.ContainsKey(currentId) Then
                For Each lk In incomingByTarget(currentId)
                    If lk.SourceLedgerID.HasValue AndAlso allLedgers.ContainsKey(lk.SourceLedgerID.Value) Then
                        If closure.Add(lk.SourceLedgerID.Value) Then
                            q.Enqueue(lk.SourceLedgerID.Value)
                        End If
                    End If
                Next
            End If
        End While

        Return closure

    End Function

    Private Function BuildIncomingLinksByTarget(links As IEnumerable(Of LinkEdge)) As Dictionary(Of Long, List(Of LinkEdge))

        Dim result As New Dictionary(Of Long, List(Of LinkEdge))

        For Each lk In links
            If lk.TargetLedgerID.HasValue Then
                If Not result.ContainsKey(lk.TargetLedgerID.Value) Then
                    result(lk.TargetLedgerID.Value) = New List(Of LinkEdge)
                End If
                result(lk.TargetLedgerID.Value).Add(lk)
            End If
        Next

        Return result

    End Function

    Private Function BuildOutgoingLinksBySource(links As IEnumerable(Of LinkEdge)) As Dictionary(Of Long, List(Of LinkEdge))

        Dim result As New Dictionary(Of Long, List(Of LinkEdge))

        For Each lk In links
            If lk.SourceLedgerID.HasValue Then
                If Not result.ContainsKey(lk.SourceLedgerID.Value) Then
                    result(lk.SourceLedgerID.Value) = New List(Of LinkEdge)
                End If
                result(lk.SourceLedgerID.Value).Add(lk)
            End If
        Next

        Return result

    End Function

#End Region

#Region "Simulation"
    Private Class CutRow
        Public Property Ledger As LedgerNode
        Public Property PrevQty As Decimal
        Public Property PrevAvg As Decimal
        Public Property LocalOldQty As Decimal
        Public Property InQty As Decimal
        Public Property OutQty As Decimal
        Public Property IsMix As Boolean
        Public Property InUnitCost As Decimal
    End Class
    Private Function ComputeCutSourceOutQty(
    groupId As Guid,
    ctx As RebuildContext
) As Decimal

        Dim total As Decimal = 0D

        Dim rows = ctx.ActiveLedgers.Values.
        Where(Function(x) x.OperationTypeID = 11 _
            AndAlso x.OperationGroupID.HasValue _
            AndAlso x.OperationGroupID.Value = groupId _
            AndAlso x.InQty > 0D).
        ToList()

        For Each led In rows

            Dim inQty As Decimal = led.InQty
            Dim outQty As Decimal = led.OutQty

            Dim corr As CorrectionEntry = Nothing
            If led.SourceDetailID.HasValue Then
                ctx.CorrectionsByTransactionDetail.TryGetValue(led.SourceDetailID.Value, corr)
            End If

            ApplyQuantityCorrection(led, corr, inQty, outQty)

            total += inQty

        Next

        Return total

    End Function

    Private Function SimulateContext(ctx As RebuildContext) As Dictionary(Of Long, SimLedgerState)

        Dim state As New Dictionary(Of Long, SimLedgerState)
        Dim localState As New Dictionary(Of String, Decimal)
        Dim processedCutGroups As New HashSet(Of Guid)

        For Each ledgerId In ctx.SortedLedgerIds

            Dim led = ctx.ActiveLedgers(ledgerId)
            ' =========================
            ' 🔥 فصل منطق القص (CUT)
            ' =========================
            If led.OperationTypeID = 11 _
               AndAlso led.OperationGroupID.HasValue _
               AndAlso led.InQty > 0D Then
                Dim g = led.OperationGroupID.Value

                If Not processedCutGroups.Contains(g) Then
                    SimulateCutGroup(g, ctx, state, localState)
                    processedCutGroups.Add(g)
                End If

                Continue For
            End If
            Dim incomingLinks As List(Of LinkEdge) = Nothing
            If Not ctx.IncomingLinksByTarget.TryGetValue(ledgerId, incomingLinks) Then
                incomingLinks = New List(Of LinkEdge)
            End If

            Dim isMixTarget As Boolean = incomingLinks.Any(Function(x) x.LinkType = 9)

            Dim currentInQty As Decimal = led.InQty
            Dim currentOutQty As Decimal = led.OutQty

            Dim corr As CorrectionEntry = Nothing
            If led.SourceDetailID.HasValue Then
                ctx.CorrectionsByTransactionDetail.TryGetValue(led.SourceDetailID.Value, corr)
            End If

            Dim isCutSource As Boolean =
    led.OperationTypeID = 11 _
    AndAlso led.OperationGroupID.HasValue _
    AndAlso led.OutQty > 0D _
    AndAlso led.InQty = 0D

            If isCutSource Then
                currentOutQty = ComputeCutSourceOutQty(led.OperationGroupID.Value, ctx)
            Else
                ApplyQuantityCorrection(led, corr, currentInQty, currentOutQty)
            End If
            Dim prevQty As Decimal = led.OldQty
            Dim prevAvg As Decimal = led.OldAvgCost

            If led.PrevLedgerID.HasValue AndAlso state.ContainsKey(led.PrevLedgerID.Value) Then
                prevQty = state(led.PrevLedgerID.Value).NewQty
                Dim prevState = state(led.PrevLedgerID.Value)

                If prevState.NewAvgCost > 0 Then
                    prevAvg = prevState.NewAvgCost
                Else
                    prevAvg = led.OldAvgCost ' fallback
                End If
            End If

            Dim localKey As String = led.StoreID.ToString() & "|" & led.ProductID.ToString()
            Dim localOldQty As Decimal = led.LocalOldQty
            If localState.ContainsKey(localKey) Then
                localOldQty = localState(localKey)
            End If

            ' =========================
            ' 🔥 تحديد نوع العملية (قص)
            ' =========================

            ' =========================
            ' تكلفة الدخول
            ' =========================
            Dim currentInUnitCost As Decimal = led.InUnitCost

            If corr IsNot Nothing AndAlso corr.NewUnitCost.HasValue AndAlso led.OperationTypeID <> 6 Then

                currentInUnitCost = corr.NewUnitCost.Value

            ElseIf isMixTarget Then

                currentInUnitCost = 0D

            ElseIf currentInQty > 0D Then

                ' 🔥 حالة خاصة: إنتاج سكراب من الوست (Operation 13)
                If led.OperationTypeID = 13 Then

                    If corr IsNot Nothing AndAlso corr.NewUnitCost.HasValue Then
                        currentInUnitCost = corr.NewUnitCost.Value
                    Else
                        currentInUnitCost = led.InUnitCost
                    End If

                Else

                    If led.SourceLedgerID.HasValue Then

                        currentInUnitCost = GetSourceLinkedInboundCost(
                sourceLedgerId:=led.SourceLedgerID.Value,
                ctx:=ctx,
                state:=state
            )

                        ' 🔥 التعديل هنا فقط
                    ElseIf incomingLinks.Any(Function(x) x.SourceLedgerID.HasValue) Then

                        currentInUnitCost = ComputeLinkedInputUnitCost(
                targetLedger:=led,
                targetInQty:=currentInQty,
                ctx:=ctx,
                state:=state
            )

                    Else
                        currentInUnitCost = led.InUnitCost
                    End If

                End If

            Else
                currentInUnitCost = led.InUnitCost
            End If
            ' =========================
            ' تكلفة الخروج
            ' =========================
            Dim currentOutUnitCost As Decimal = 0D
            Dim newAvg As Decimal = prevAvg

            If currentOutQty > 0D Then

                currentOutUnitCost = prevAvg
            End If
            Dim newQty As Decimal = prevQty + currentInQty - currentOutQty
            Dim totalCost As Decimal = (prevQty * prevAvg) + (currentInQty * currentInUnitCost) - (currentOutQty * currentOutUnitCost)


            If isMixTarget Then
                newAvg = 0D


            ElseIf newQty <> 0D Then
                newAvg = totalCost / newQty
            End If

            ' =========================
            ' 🔥 منطق القص
            ' =========================

            Dim localNewQty As Decimal = localOldQty + currentInQty - currentOutQty

            Dim s As New SimLedgerState With {
            .LedgerID = led.LedgerID,
            .ProductID = led.ProductID,
            .StoreID = led.StoreID,
            .OldQty = prevQty,
            .NewQty = newQty,
            .InQty = currentInQty,
            .OutQty = currentOutQty,
            .LocalOldQty = localOldQty,
            .LocalNewQty = localNewQty,
            .OldAvgCost = prevAvg,
            .NewAvgCost = newAvg,
            .InUnitCost = currentInUnitCost,
            .OutUnitCost = currentOutUnitCost
        }
            state(ledgerId) = s
            localState(localKey) = localNewQty

        Next
        ' =========================
        ' 🔥 إعادة حساب متوسط القص بعد اكتمال الحالة
        ' =========================

        ' =========================
        ' 🔥 حساب متوسط القص النهائي
        ' =========================
        Return state

    End Function
    Private Sub SimulateCutGroup(
    groupId As Guid,
    ctx As RebuildContext,
    state As Dictionary(Of Long, SimLedgerState),
    localState As Dictionary(Of String, Decimal)
)

        Dim rows = ctx.ActiveLedgers.Values.
    Where(Function(x) x.OperationTypeID = 11 _
        AndAlso x.OperationGroupID.HasValue _
        AndAlso x.OperationGroupID.Value = groupId _
        AndAlso x.InQty > 0D).
        OrderBy(Function(x) If(x.GroupSeq.HasValue, x.GroupSeq.Value, Integer.MaxValue)).
        ThenBy(Function(x) x.LedgerSequence).
        ThenBy(Function(x) x.LedgerID).
        ToList()

        If rows.Count = 0 Then Exit Sub

        Dim poolOldQty As Decimal = 0D
        Dim poolOldCost As Decimal = 0D
        Dim poolInQty As Decimal = 0D
        Dim poolInCost As Decimal = 0D
        Dim poolOutQty As Decimal = 0D
        Dim poolOldAvg As Decimal = 0D
        Dim prepared As New List(Of CutRow)

        For Each led In rows

            Dim incomingLinks As List(Of LinkEdge) = Nothing
            If Not ctx.IncomingLinksByTarget.TryGetValue(led.LedgerID, incomingLinks) Then
                incomingLinks = New List(Of LinkEdge)
            End If

            Dim isMix As Boolean = incomingLinks.Any(Function(x) x.LinkType = 9)

            Dim currentInQty As Decimal = led.InQty
            Dim currentOutQty As Decimal = led.OutQty

            Dim corr As CorrectionEntry = Nothing

            If led.SourceDetailID.HasValue Then
                ctx.CorrectionsByTransactionDetail.TryGetValue(led.SourceDetailID.Value, corr)
            End If
            ApplyQuantityCorrection(led, corr, currentInQty, currentOutQty)

            Dim prevQty As Decimal = led.OldQty
            Dim prevAvg As Decimal = led.OldAvgCost

            If led.PrevLedgerID.HasValue AndAlso state.ContainsKey(led.PrevLedgerID.Value) Then
                prevQty = state(led.PrevLedgerID.Value).NewQty
                prevAvg = state(led.PrevLedgerID.Value).NewAvgCost
            End If

            Dim localKey As String = led.StoreID.ToString() & "|" & led.ProductID.ToString()
            Dim localOldQty As Decimal = led.LocalOldQty
            If localState.ContainsKey(localKey) Then
                localOldQty = localState(localKey)
            End If

            Dim inUnitCost As Decimal = 0D

            If corr IsNot Nothing AndAlso corr.NewUnitCost.HasValue Then
                inUnitCost = corr.NewUnitCost.Value
            ElseIf Not isMix AndAlso currentInQty > 0D Then
                inUnitCost = ComputeLinkedInputUnitCost(led, currentInQty, ctx, state)
            End If

            If Not isMix Then

                ' 🔥 نجمع كل oldQty وليس أول سطر فقط
                poolOldQty += prevQty
                poolOldCost += prevQty * prevAvg

                poolInQty += currentInQty
                poolInCost += currentInQty * inUnitCost

            End If
            prepared.Add(New CutRow With {
    .Ledger = led,
    .PrevQty = prevQty,
    .PrevAvg = prevAvg,
    .LocalOldQty = localOldQty,
    .InQty = currentInQty,
    .OutQty = currentOutQty,
    .IsMix = isMix,
    .InUnitCost = inUnitCost
})
        Next
        If poolOldQty <> 0D Then
            poolOldAvg = poolOldCost / poolOldQty
        Else
            poolOldAvg = 0D
        End If
        Dim poolNewQty As Decimal = poolOldQty + poolInQty
        Dim poolNewAvg As Decimal = 0D

        If poolNewQty <> 0D Then
            poolNewAvg = (poolOldCost + poolInCost) / poolNewQty
        End If

        For Each item In prepared

            Dim led = item.Ledger
            Dim prevQty = item.PrevQty
            Dim prevAvg = item.PrevAvg
            Dim localOldQty = item.LocalOldQty
            Dim inQty = item.InQty
            Dim outQty = item.OutQty
            Dim isMix = item.IsMix
            Dim inUnitCost = item.InUnitCost

            Dim newAvg As Decimal = If(isMix, 0D, poolNewAvg)
            Dim outCost As Decimal = If(outQty > 0, prevAvg, 0D)
            If Not isMix Then
                poolOutQty += outQty
            End If
            Dim newQty = prevQty + inQty - outQty
            Dim localNewQty = localOldQty + inQty - outQty
            Dim isInRow As Boolean = inQty > 0D
            Dim cpOldQty As Decimal?
            Dim cpInQty As Decimal?
            Dim cpOutQty As Decimal?
            Dim cpNewQty As Decimal?
            Dim cpOldAvg As Decimal?
            Dim cpNewAvg As Decimal?

            If isMix OrElse Not isInRow Then

                cpOldQty = Nothing
                cpInQty = Nothing
                cpOutQty = Nothing
                cpNewQty = Nothing
                cpOldAvg = Nothing
                cpNewAvg = Nothing

            Else

                cpOldQty = poolOldQty
                cpInQty = poolInQty
                cpOutQty = poolOutQty
                cpNewQty = poolNewQty
                cpOldAvg = poolOldAvg
                cpNewAvg = poolNewAvg

            End If
            state(led.LedgerID) = New SimLedgerState With {
                    .LedgerID = led.LedgerID,
                    .ProductID = led.ProductID,
                    .StoreID = led.StoreID,
                    .OldQty = prevQty,
                    .NewQty = newQty,
                    .InQty = inQty,
                    .OutQty = outQty,
                    .LocalOldQty = localOldQty,
                    .LocalNewQty = localNewQty,
                    .OldAvgCost = prevAvg,
                    .NewAvgCost = newAvg,
                    .InUnitCost = If(isMix, 0D, inUnitCost),
                    .OutUnitCost = outCost,
                    .CostPoolOldQty = cpOldQty,
                    .CostPoolInQty = cpInQty,
                    .CostPoolOutQty = cpOutQty,
                    .CostPoolNewQty = cpNewQty,
                    .CostPoolOldAvgCost = cpOldAvg,
                    .CostPoolNewAvgCost = cpNewAvg
                }

            Dim localKey As String = led.StoreID.ToString() & "|" & led.ProductID.ToString()
            localState(localKey) = localNewQty

        Next

    End Sub
    Private Function ComputeCutPoolNewAvg(
    currentLedger As LedgerNode,
    currentLedgerId As Long,
    currentInQty As Decimal,
    currentInUnitCost As Decimal,
    ctx As RebuildContext,
    state As Dictionary(Of Long, SimLedgerState)
) As Decimal

        If Not currentLedger.OperationGroupID.HasValue Then
            Return currentLedger.CostPoolNewAvgCost
        End If

        Dim groupId = currentLedger.OperationGroupID.Value

        ' كل سطور القص الداخلة non-mix في نفس العملية
        Dim groupRows = ctx.ActiveLedgers.Values.
        Where(Function(x) x.OperationTypeID = 11 _
            AndAlso x.OperationGroupID.HasValue _
            AndAlso x.OperationGroupID.Value = groupId _
            AndAlso x.InQty > 0D).
        OrderBy(Function(x) x.LedgerSequence).
        ThenBy(Function(x) x.LedgerID).
        ToList()

        If groupRows.Count = 0 Then
            Return currentLedger.CostPoolNewAvgCost
        End If

        Dim poolOldQty As Decimal = 0D
        Dim poolOldAvg As Decimal = 0D
        Dim foundPoolSeed As Boolean = False

        Dim poolInQty As Decimal = 0D
        Dim poolInCost As Decimal = 0D

        For Each led In groupRows

            Dim incomingLinks As List(Of LinkEdge) = Nothing
            If Not ctx.IncomingLinksByTarget.TryGetValue(led.LedgerID, incomingLinks) Then
                incomingLinks = New List(Of LinkEdge)
            End If

            Dim isMixTarget As Boolean = incomingLinks.Any(Function(x) x.LinkType = 9)
            If isMixTarget Then Continue For

            ' خذ أساس الحاوية القديمة من أول سطر non-mix يحمل CostPool
            If Not foundPoolSeed Then

                If ctx.IncomingLinksByTarget.TryGetValue(led.LedgerID, incomingLinks) Then

                    Dim srcLink = incomingLinks.FirstOrDefault(Function(x) x.LinkType <> 9)

                    If srcLink IsNot Nothing AndAlso srcLink.SourceLedgerID.HasValue Then

                        Dim srcId = srcLink.SourceLedgerID.Value

                        If state.ContainsKey(srcId) Then
                            poolOldQty = state(srcId).OutQty
                            poolOldAvg = state(srcId).OutUnitCost
                        ElseIf ctx.ActiveLedgers.ContainsKey(srcId) Then
                            poolOldQty = ctx.ActiveLedgers(srcId).OutQty
                            poolOldAvg = ctx.ActiveLedgers(srcId).OutUnitCost
                        End If

                        foundPoolSeed = True

                    End If

                End If

            End If
            Dim rowInQty As Decimal
            Dim rowInUnitCost As Decimal

            If led.LedgerID = currentLedgerId Then
                rowInQty = currentInQty
                rowInUnitCost = currentInUnitCost
            Else
                rowInQty = led.InQty

                ' إن كان هذا السطر أعيدت محاكاته بالفعل واختلفت تكلفته خذ الجديدة
                If state.ContainsKey(led.LedgerID) Then
                    rowInUnitCost = state(led.LedgerID).InUnitCost
                    rowInQty = state(led.LedgerID).InQty
                Else
                    rowInUnitCost = led.InUnitCost
                End If

                ' طبّق تصحيح الكمية إن وجد على هذا السطر
                Dim corr As CorrectionEntry = Nothing

                If led.SourceDetailID.HasValue Then
                    ctx.CorrectionsByTransactionDetail.TryGetValue(led.SourceDetailID.Value, corr)
                End If
                Dim tmpIn As Decimal = rowInQty
                Dim tmpOut As Decimal = led.OutQty
                ApplyQuantityCorrection(led, corr, tmpIn, tmpOut)
                rowInQty = tmpIn

                ' طبّق تصحيح التكلفة المباشرة إن وجد
                If corr IsNot Nothing AndAlso corr.NewUnitCost.HasValue Then
                    rowInUnitCost = corr.NewUnitCost.Value
                End If
            End If

            poolInQty += rowInQty
            poolInCost += rowInQty * rowInUnitCost
        Next

        Dim poolNewQty As Decimal = poolOldQty + poolInQty
        If poolNewQty <= 0D Then Return 0D

        Return ((poolOldQty * poolOldAvg) + poolInCost) / poolNewQty

    End Function

    Private Sub ApplyQuantityCorrection(led As LedgerNode,
                                        corr As CorrectionEntry,
                                        ByRef currentInQty As Decimal,
                                        ByRef currentOutQty As Decimal)

        If corr Is Nothing OrElse Not corr.NewQuantity.HasValue Then
            Exit Sub
        End If

        ' نطبق الكمية الجديدة على الطرف الفعلي للحركة
        If led.InQty > 0D AndAlso led.OutQty = 0D Then
            currentInQty = corr.NewQuantity.Value
        ElseIf led.OutQty > 0D AndAlso led.InQty = 0D Then
            currentOutQty = corr.NewQuantity.Value
        ElseIf led.InQty > 0D Then
            currentInQty = corr.NewQuantity.Value
        ElseIf led.OutQty > 0D Then
            currentOutQty = corr.NewQuantity.Value
        End If

    End Sub

    Private Function ComputeLinkedInputUnitCost(targetLedger As LedgerNode,
                                                targetInQty As Decimal,
                                                ctx As RebuildContext,
                                                state As Dictionary(Of Long, SimLedgerState)) As Decimal

        If targetInQty <= 0D Then Return 0D

        Dim incomingLinks As List(Of LinkEdge) = Nothing
        If Not ctx.IncomingLinksByTarget.TryGetValue(targetLedger.LedgerID, incomingLinks) Then
            Return 0D
        End If

        If incomingLinks.Any(Function(x) x.LinkType = 9) Then
            Return 0D
        End If

        Dim totalAllocatedCost As Decimal = 0D

        For Each lk In incomingLinks
            totalAllocatedCost += AllocateSourceCostToTarget(lk, ctx, state)
        Next

        If totalAllocatedCost <= 0D Then
            Return 0D
        End If

        Return totalAllocatedCost / targetInQty

    End Function

    Private Function AllocateSourceCostToTarget(incomingLink As LinkEdge,
                                                ctx As RebuildContext,
                                                state As Dictionary(Of Long, SimLedgerState)) As Decimal

        If Not incomingLink.SourceLedgerID.HasValue OrElse Not incomingLink.TargetLedgerID.HasValue Then
            Return 0D
        End If

        Dim sourceLedgerId = incomingLink.SourceLedgerID.Value
        Dim targetLedgerId = incomingLink.TargetLedgerID.Value

        If Not ctx.AllLedgers.ContainsKey(sourceLedgerId) OrElse Not ctx.AllLedgers.ContainsKey(targetLedgerId) Then
            Return 0D
        End If

        Dim sourceOutQty As Decimal
        Dim sourceOutUnitCost As Decimal

        If state.ContainsKey(sourceLedgerId) Then
            sourceOutQty = state(sourceLedgerId).OutQty
            sourceOutUnitCost = state(sourceLedgerId).OutUnitCost
        Else
            Dim src = ctx.AllLedgers(sourceLedgerId)
            sourceOutQty = src.OutQty
            sourceOutUnitCost = src.OutUnitCost
        End If

        Dim sourceTotalOutCost As Decimal = sourceOutQty * sourceOutUnitCost
        If sourceTotalOutCost = 0D Then Return 0D

        Dim outgoingLinks As List(Of LinkEdge) = Nothing
        If Not ctx.OutgoingLinksBySource.TryGetValue(sourceLedgerId, outgoingLinks) Then
            Return 0D
        End If

        ' نستبعد الـ MIX من التحميل
        Dim nonMixTargets = outgoingLinks.
            Where(Function(x) x.TargetLedgerID.HasValue AndAlso x.LinkType <> 9).
            ToList()

        If nonMixTargets.Count = 0 Then
            Return 0D
        End If

        If nonMixTargets.Count = 1 Then
            If nonMixTargets(0).TargetLedgerID.Value = targetLedgerId Then
                Return sourceTotalOutCost
            Else
                Return 0D
            End If
        End If

        Dim totalWeight As Decimal = 0D
        For Each lk In nonMixTargets
            totalWeight += GetTargetWeight(lk.TargetLedgerID.Value, ctx)
        Next

        If totalWeight <= 0D Then
            Return 0D
        End If

        Dim myWeight As Decimal = GetTargetWeight(targetLedgerId, ctx)
        If myWeight <= 0D Then
            Return 0D
        End If

        Return sourceTotalOutCost * (myWeight / totalWeight)

    End Function

    Private Function GetTargetWeight(targetLedgerId As Long,
                                     ctx As RebuildContext) As Decimal

        If Not ctx.AllLedgers.ContainsKey(targetLedgerId) Then
            Return 0D
        End If

        Dim led = ctx.AllLedgers(targetLedgerId)

        Dim inQty As Decimal = led.InQty
        Dim outQty As Decimal = led.OutQty

        Dim corr As CorrectionEntry = Nothing

        If led.SourceDetailID.HasValue Then
            ctx.CorrectionsByTransactionDetail.TryGetValue(led.SourceDetailID.Value, corr)
        End If
        ApplyQuantityCorrection(led, corr, inQty, outQty)

        If inQty > 0D Then Return inQty
        If outQty > 0D Then Return outQty

        Return 0D

    End Function

    Private Function GetSourceLinkedInboundCost(sourceLedgerId As Long,
                                                ctx As RebuildContext,
                                                state As Dictionary(Of Long, SimLedgerState)) As Decimal

        If state.ContainsKey(sourceLedgerId) Then
            Return state(sourceLedgerId).OutUnitCost
        End If

        If ctx.AllLedgers.ContainsKey(sourceLedgerId) Then
            Return ctx.AllLedgers(sourceLedgerId).OutUnitCost
        End If

        Return 0D

    End Function

#End Region

#Region "Result"

    Private Function CreateResultTable() As DataTable

        Dim dt As New DataTable()

        ' الهوية
        dt.Columns.Add("NewLedgerID", GetType(Long))
        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Integer))
        dt.Columns.Add("SourceDetailID", GetType(Integer))
        dt.Columns.Add("DocumentDetailID", GetType(Integer))

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))

        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("OperationGroupID", GetType(Guid))
        dt.Columns.Add("GroupSeq", GetType(Integer))

        dt.Columns.Add("LedgerSequence", GetType(Long))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("PrevLedgerID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(DateTime))

        ' الكميات
        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("InQty", GetType(Decimal))
        dt.Columns.Add("OutQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        ' التكاليف
        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("InUnitCost", GetType(Decimal))
        dt.Columns.Add("OutUnitCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("OldOutTotalCost", GetType(Decimal))
        dt.Columns.Add("NewOutTotalCost", GetType(Decimal))
        dt.Columns.Add("CostDifference", GetType(Decimal))
        dt.Columns.Add("IsChanged", GetType(Boolean))
        dt.Columns.Add("ChangeImpactValue", GetType(Decimal))
        dt.Columns.Add("OldInventoryValue", GetType(Decimal))
        dt.Columns.Add("NewInventoryValue", GetType(Decimal))
        dt.Columns.Add("InventoryDifference", GetType(Decimal))

        Dim col As DataColumn

        col = dt.Columns.Add("CostPoolOldQty", GetType(Decimal))
        col.AllowDBNull = True

        col = dt.Columns.Add("CostPoolInQty", GetType(Decimal))
        col.AllowDBNull = True

        col = dt.Columns.Add("CostPoolOutQty", GetType(Decimal))
        col.AllowDBNull = True

        col = dt.Columns.Add("CostPoolNewQty", GetType(Decimal))
        col.AllowDBNull = True

        col = dt.Columns.Add("CostPoolOldAvgCost", GetType(Decimal))
        col.AllowDBNull = True

        col = dt.Columns.Add("CostPoolNewAvgCost", GetType(Decimal))
        col.AllowDBNull = True

        Return dt

    End Function
    Private Function BuildResultTable(ctx As RebuildContext,
                                 states As Dictionary(Of Long, SimLedgerState)) As DataTable

        Dim dt = CreateResultTable()

        For Each ledgerId In ctx.SortedLedgerIds

            Dim led = ctx.ActiveLedgers(ledgerId)
            Dim st = states(ledgerId)

            Dim row = dt.NewRow()
            Dim oldOutTotal As Decimal = led.OutTotalCost
            Dim newOutTotal As Decimal = st.OutQty * st.OutUnitCost
            Dim oldInvValue = led.NewQty * led.OldAvgCost
            Dim newInvValue = st.NewQty * st.NewAvgCost
            Dim invDiff = newInvValue - oldInvValue


            Dim diff As Decimal = newOutTotal - oldOutTotal
            ' الهوية
            row("LedgerID") = led.LedgerID
            row("TransactionID") = led.TransactionID
            row("SourceDetailID") = If(led.SourceDetailID.HasValue, CType(led.SourceDetailID.Value, Object), DBNull.Value)
            row("DocumentDetailID") = If(led.DocumentDetailID.HasValue, CType(led.DocumentDetailID.Value, Object), DBNull.Value)

            row("ProductID") = led.ProductID
            row("BaseProductID") = If(led.BaseProductID.HasValue, CType(led.BaseProductID.Value, Object), DBNull.Value)
            row("StoreID") = led.StoreID

            row("OperationTypeID") = led.OperationTypeID
            row("OperationGroupID") = If(led.OperationGroupID.HasValue, CType(led.OperationGroupID.Value, Object), DBNull.Value)
            row("GroupSeq") = If(led.GroupSeq.HasValue, CType(led.GroupSeq.Value, Object), DBNull.Value)

            row("LedgerSequence") = led.LedgerSequence
            row("SourceLedgerID") = If(led.SourceLedgerID.HasValue, CType(led.SourceLedgerID.Value, Object), DBNull.Value)
            row("PrevLedgerID") = If(led.PrevLedgerID.HasValue, CType(led.PrevLedgerID.Value, Object), DBNull.Value)

            row("PostingDate") = If(led.PostingDate.HasValue, CType(led.PostingDate.Value, Object), DBNull.Value)

            ' الكميات
            row("LocalOldQty") = st.LocalOldQty
            row("OldQty") = st.OldQty
            row("InQty") = st.InQty
            row("OutQty") = st.OutQty
            row("NewQty") = st.NewQty
            row("LocalNewQty") = st.LocalNewQty

            ' التكاليف
            row("OldAvgCost") = st.OldAvgCost
            row("InUnitCost") = st.InUnitCost
            row("OutUnitCost") = st.OutUnitCost
            row("NewAvgCost") = st.NewAvgCost

            row("CostPoolOldQty") = If(st.CostPoolOldQty.HasValue, st.CostPoolOldQty.Value, DBNull.Value)
            row("CostPoolInQty") = If(st.CostPoolInQty.HasValue, st.CostPoolInQty.Value, DBNull.Value)
            row("CostPoolOutQty") = If(st.CostPoolOutQty.HasValue, st.CostPoolOutQty.Value, DBNull.Value)
            row("CostPoolNewQty") = If(st.CostPoolNewQty.HasValue, st.CostPoolNewQty.Value, DBNull.Value)
            row("CostPoolOldAvgCost") = If(st.CostPoolOldAvgCost.HasValue, st.CostPoolOldAvgCost.Value, DBNull.Value)
            row("CostPoolNewAvgCost") = If(st.CostPoolNewAvgCost.HasValue, st.CostPoolNewAvgCost.Value, DBNull.Value)

            row("OldOutTotalCost") = oldOutTotal
            row("NewOutTotalCost") = newOutTotal
            row("CostDifference") = diff
            row("IsChanged") = If(Math.Abs(invDiff) > 0.0001D, True, False)
            row("ChangeImpactValue") = invDiff
            row("OldInventoryValue") = oldInvValue
            row("NewInventoryValue") = newInvValue
            row("InventoryDifference") = invDiff
            dt.Rows.Add(row)

        Next

        Return dt

    End Function
    Public Sub ExecuteFullCorrection(startLedgerIds As List(Of Long), userID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()
                Dim runId As Long = 0
                Try

                    ' 1) Simulation
                    Using cmd As New SqlCommand("
INSERT INTO inv.CostEngineRun
(
    StartLedgerID,
    StatusCode,
    StartedAt,
    StartedBy,
    StatusID,
    EngineVersion
)
VALUES
(
    @StartLedgerID,
    'RUNNING',
    SYSDATETIME(),
    @UserID,
    23,
    1
);
SELECT SCOPE_IDENTITY();
", con, tran)

                        cmd.Parameters.AddWithValue("@StartLedgerID", startLedgerIds.First())
                        cmd.Parameters.AddWithValue("@UserID", userID)

                        runId = CLng(cmd.ExecuteScalar())

                    End Using
                    Dim dtResult = SimulateByStartLedgerIds(startLedgerIds)

                    ' 2) Rebuild Cost
                    SaveEngineResults(dtResult, con, tran, runId)
                    RebuildCostLedger(dtResult, con, tran, userID)
                    RebuildCostLinks(dtResult, con, tran, userID)

                    ' 3) Update Transactions
                    RebuildTransactions(dtResult, con, tran)

                    ' 4) Update Headers
                    UpdateTransactionHeadersAsCorrected(dtResult, con, tran)

                    Dim srv As New CostRevaluationService()
                    srv.PostRevaluation(runId, userID, con, tran)

                    ' 5) Update Queue → SUCCESS
                    UpdateCorrectionQueue(startLedgerIds, 24, con, tran)
                    Using cmd As New SqlCommand("
UPDATE inv.CostEngineRun
SET
    StatusCode = 'DONE',
    EndedAt = SYSDATETIME(),
    TotalLedgers = @Total,
    AffectedCount = @Affected,
    ErrorCount = 0,
    Notes = @Notes,
    StatusID = 24,
    DurationMs = DATEDIFF(MILLISECOND, StartedAt, SYSDATETIME())
WHERE RunID = @RunID
", con, tran)

                        cmd.Parameters.AddWithValue("@RunID", runId)
                        cmd.Parameters.AddWithValue("@Total", dtResult.Rows.Count)

                        Dim affected = dtResult.AsEnumerable().
        Count(Function(r) r("OldAvgCost") <> r("NewAvgCost") _
                       OrElse r("OldQty") <> r("NewQty"))

                        cmd.Parameters.AddWithValue("@Affected", affected)
                        cmd.Parameters.AddWithValue("@Notes", "Auto rebuild")

                        cmd.ExecuteNonQuery()

                    End Using



                    tran.Commit()

                Catch ex As Exception

                    tran.Rollback()

                    ' 🔥 تحديث الفشل
                    Using con2 As New SqlConnection(_connectionString)
                        con2.Open()

                        Using cmd As New SqlCommand("
UPDATE inv.CorrectionQueue
SET 
    StatusID = 25,
    LastUpdatedAt = GETDATE(),
    ErrorMessage = @Err
WHERE StatusID = 22
", con2)

                            cmd.Parameters.AddWithValue("@Err", ex.Message)
                            cmd.ExecuteNonQuery()

                        End Using
                    End Using
                    Using con3 As New SqlConnection(_connectionString)
                        con3.Open()

                        Using cmd As New SqlCommand("
UPDATE inv.CostEngineRun
SET
    StatusCode = 'FAILED',
    EndedAt = SYSDATETIME(),
    ErrorCount = 1,
    Notes = @Err,
    StatusID = 25,
    DurationMs = DATEDIFF(MILLISECOND, StartedAt, SYSDATETIME())
WHERE RunID = @RunID
", con3)

                            cmd.Parameters.AddWithValue("@RunID", runId)
                            cmd.Parameters.AddWithValue("@Err", ex.Message)

                            cmd.ExecuteNonQuery()

                        End Using
                    End Using
                    Throw

                End Try

            End Using
        End Using

    End Sub

    Private Sub SaveEngineResults(
    dt As DataTable,
    con As SqlConnection,
    tran As SqlTransaction,
    runId As Long
)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        ' =========================
        ' 1) إنشاء Run
        ' =========================


        ' =========================
        ' 2) Bulk Insert
        ' =========================
        Using bulk As New SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran)

            bulk.DestinationTableName = "inv.CostEngineResult"

            ' =========================
            ' Mapping
            ' =========================
            bulk.ColumnMappings.Add("RunID", "RunID")
            bulk.ColumnMappings.Add("TransactionID", "TransactionID")
            bulk.ColumnMappings.Add("DocumentDetailID", "DocumentDetailID")
            bulk.ColumnMappings.Add("BaseProductID", "BaseProductID")
            bulk.ColumnMappings.Add("LedgerSequence", "LedgerSequence")
            bulk.ColumnMappings.Add("LedgerID", "LedgerID")
            bulk.ColumnMappings.Add("SourceDetailID", "SourceDetailID")
            bulk.ColumnMappings.Add("ProductID", "ProductID")
            bulk.ColumnMappings.Add("StoreID", "StoreID")
            bulk.ColumnMappings.Add("OperationTypeID", "OperationTypeID")

            bulk.ColumnMappings.Add("PostingDate", "PostingDate")

            bulk.ColumnMappings.Add("PrevLedgerID", "PrevLedgerID")
            bulk.ColumnMappings.Add("SourceLedgerID", "SourceLedgerID")

            bulk.ColumnMappings.Add("OldQty", "OldQty")
            bulk.ColumnMappings.Add("InQty", "InQty")
            bulk.ColumnMappings.Add("OutQty", "OutQty")
            bulk.ColumnMappings.Add("NewQty", "NewQty")

            bulk.ColumnMappings.Add("LocalOldQty", "LocalOldQty")
            bulk.ColumnMappings.Add("LocalNewQty", "LocalNewQty")

            bulk.ColumnMappings.Add("OldAvgCost", "OldAvgCost")
            bulk.ColumnMappings.Add("InUnitCost", "InUnitCost")
            bulk.ColumnMappings.Add("OutUnitCost", "OutUnitCost")
            bulk.ColumnMappings.Add("NewAvgCost", "NewAvgCost")

            bulk.ColumnMappings.Add("OperationGroupID", "OperationGroupID")
            bulk.ColumnMappings.Add("GroupSeq", "GroupSeq")

            bulk.ColumnMappings.Add("OldOutTotalCost", "OldOutTotalCost")
            bulk.ColumnMappings.Add("NewOutTotalCost", "NewOutTotalCost")
            bulk.ColumnMappings.Add("CostDifference", "CostDifference")
            bulk.ColumnMappings.Add("IsChanged", "IsChanged")
            bulk.ColumnMappings.Add("ChangeImpactValue", "ChangeImpactValue")
            bulk.ColumnMappings.Add("OldInventoryValue", "OldInventoryValue")
            bulk.ColumnMappings.Add("NewInventoryValue", "NewInventoryValue")
            bulk.ColumnMappings.Add("InventoryDifference", "InventoryDifference")

            ' =========================
            ' إضافة RunID لكل الصفوف
            ' =========================
            For Each col As DataColumn In dt.Columns
                Debug.Print(col.ColumnName)
            Next
            Dim dtWithRun As DataTable = dt.Copy()

            If Not dtWithRun.Columns.Contains("RunID") Then
                dtWithRun.Columns.Add("RunID", GetType(Long))
            End If

            For Each r As DataRow In dtWithRun.Rows
                r("RunID") = runId
            Next

            ' =========================
            ' إدخال البيانات
            ' =========================
            bulk.WriteToServer(dtWithRun)

        End Using

    End Sub
    Private Sub RebuildCostLedger(
    dt As DataTable,
    con As SqlConnection,
    tran As SqlTransaction,
    userID As Integer
)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        ' =========================
        ' 1) تحديد المتأثر فقط
        ' =========================
        Dim affected = dt.AsEnumerable().
        Where(Function(r) r("OldAvgCost") <> r("NewAvgCost") _
                        OrElse r("OldQty") <> r("NewQty") _
                        OrElse r("InUnitCost") <> r("OutUnitCost")).
        ToList()

        If affected.Count = 0 Then Exit Sub

        ' =========================
        ' 2) تعليق القديم
        ' =========================
        Dim ids = String.Join(",", affected.Select(Function(r) r("LedgerID").ToString()))

        Using cmd As New SqlCommand($"
UPDATE inv.CostLedger
SET IsActive = 0,
    IsReversed = 1
WHERE LedgerID IN ({ids})
", con, tran)
            cmd.ExecuteNonQuery()
        End Using

        ' =========================
        ' 3) إدخال الجديد + Mapping
        ' =========================
        Dim map As New Dictionary(Of Long, Long)

        For Each r In affected

            Dim oldLedgerID As Long = CLng(r("LedgerID"))

            Dim newLedgerID As Long

            Using cmd As New SqlCommand("
INSERT INTO inv.CostLedger
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
    CostPoolOldQty,
    CostPoolInQty,
    CostPoolOutQty,
    CostPoolNewQty,
    CostPoolOldAvgCost,
    CostPoolNewAvgCost,
    SourceLedgerID,
    PrevLedgerID,

    RootTransactionID,
    CreatedBy,
    CreatedAt,

    OperationGroupID,
    GroupSeq,

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
@OutQty,
@NewQty,

@OldAvgCost,
@InUnitCost,
@InTotalCost,
@OutUnitCost,
@OutTotalCost,
@NewAvgCost,
@CostPoolOldQty,
@CostPoolInQty,
@CostPoolOutQty,
@CostPoolNewQty,
@CostPoolOldAvgCost,
@CostPoolNewAvgCost,
NULL,
NULL,

@TransactionID,
@UserID,
SYSDATETIME(),

@OperationGroupID,
@GroupSeq,

@LedgerSequence,
1
)
", con, tran)

                cmd.Parameters.AddWithValue("@TransactionID", r("TransactionID"))
                cmd.Parameters.AddWithValue("@SourceDetailID", r("SourceDetailID"))
                cmd.Parameters.AddWithValue("@ProductID", r("ProductID"))
                cmd.Parameters.AddWithValue("@BaseProductID", r("BaseProductID"))
                cmd.Parameters.AddWithValue("@StoreID", r("StoreID"))
                cmd.Parameters.AddWithValue("@OperationTypeID", r("OperationTypeID"))
                cmd.Parameters.AddWithValue("@PostingDate", r("PostingDate"))

                cmd.Parameters.AddWithValue("@LocalOldQty", r("LocalOldQty"))
                cmd.Parameters.AddWithValue("@LocalNewQty", r("LocalNewQty"))

                cmd.Parameters.AddWithValue("@OldQty", r("OldQty"))
                cmd.Parameters.AddWithValue("@InQty", r("InQty"))
                cmd.Parameters.AddWithValue("@OutQty", r("OutQty"))
                cmd.Parameters.AddWithValue("@NewQty", r("NewQty"))

                cmd.Parameters.AddWithValue("@OldAvgCost", r("OldAvgCost"))
                cmd.Parameters.AddWithValue("@InUnitCost", r("InUnitCost"))
                cmd.Parameters.AddWithValue("@OutUnitCost", r("OutUnitCost"))

                Dim inTotal = CDec(r("InQty")) * CDec(r("InUnitCost"))
                Dim outTotal = CDec(r("OutQty")) * CDec(r("OutUnitCost"))

                cmd.Parameters.AddWithValue("@InTotalCost", inTotal)
                cmd.Parameters.AddWithValue("@OutTotalCost", outTotal)

                cmd.Parameters.AddWithValue("@NewAvgCost", r("NewAvgCost"))

                cmd.Parameters.AddWithValue("@OperationGroupID", If(IsDBNull(r("OperationGroupID")), DBNull.Value, r("OperationGroupID")))
                cmd.Parameters.AddWithValue("@GroupSeq", If(IsDBNull(r("GroupSeq")), DBNull.Value, r("GroupSeq")))

                cmd.Parameters.AddWithValue("@LedgerSequence", r("LedgerSequence"))
                cmd.Parameters.AddWithValue("@UserID", userID)
                If CInt(r("OperationTypeID")) = 11 Then

                    cmd.Parameters.AddWithValue("@CostPoolOldQty", r("CostPoolOldQty"))
                    cmd.Parameters.AddWithValue("@CostPoolInQty", r("CostPoolInQty"))
                    cmd.Parameters.AddWithValue("@CostPoolOutQty", r("CostPoolOutQty"))
                    cmd.Parameters.AddWithValue("@CostPoolNewQty", r("CostPoolNewQty"))

                    cmd.Parameters.AddWithValue("@CostPoolOldAvgCost", r("CostPoolOldAvgCost"))
                    cmd.Parameters.AddWithValue("@CostPoolNewAvgCost", r("CostPoolNewAvgCost"))

                Else

                    cmd.Parameters.AddWithValue("@CostPoolOldQty", DBNull.Value)
                    cmd.Parameters.AddWithValue("@CostPoolInQty", DBNull.Value)
                    cmd.Parameters.AddWithValue("@CostPoolOutQty", DBNull.Value)
                    cmd.Parameters.AddWithValue("@CostPoolNewQty", DBNull.Value)

                    cmd.Parameters.AddWithValue("@CostPoolOldAvgCost", DBNull.Value)
                    cmd.Parameters.AddWithValue("@CostPoolNewAvgCost", DBNull.Value)

                End If
                newLedgerID = CLng(cmd.ExecuteScalar())
                r("NewLedgerID") = newLedgerID
            End Using

            map(oldLedgerID) = newLedgerID

        Next

        ' =========================
        ' 4) تحديث الروابط
        ' =========================
        For Each r In affected

            Dim newLedgerID = map(CLng(r("LedgerID")))

            Dim prevOld = If(IsDBNull(r("PrevLedgerID")), Nothing, CType(r("PrevLedgerID"), Long?))
            Dim srcOld = If(IsDBNull(r("SourceLedgerID")), Nothing, CType(r("SourceLedgerID"), Long?))

            Dim prevNew As Object = DBNull.Value
            Dim srcNew As Object = DBNull.Value

            If prevOld.HasValue AndAlso map.ContainsKey(prevOld.Value) Then
                prevNew = map(prevOld.Value)
            ElseIf prevOld.HasValue Then
                prevNew = prevOld.Value
            End If

            If srcOld.HasValue AndAlso map.ContainsKey(srcOld.Value) Then
                srcNew = map(srcOld.Value)
            ElseIf srcOld.HasValue Then
                srcNew = srcOld.Value
            End If

            Using cmd As New SqlCommand("
UPDATE inv.CostLedger
SET PrevLedgerID = @Prev,
    SourceLedgerID = @Src
WHERE LedgerID = @ID
", con, tran)

                cmd.Parameters.AddWithValue("@Prev", prevNew)
                cmd.Parameters.AddWithValue("@Src", srcNew)
                cmd.Parameters.AddWithValue("@ID", newLedgerID)

                cmd.ExecuteNonQuery()
            End Using

        Next

    End Sub
    Private Sub RebuildCostLinks(
    dt As DataTable,
    con As SqlConnection,
    tran As SqlTransaction,
    userID As Integer
)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        ' =========================
        ' 1) Mapping LedgerID -> NewLedgerID
        ' =========================
        Dim map As New Dictionary(Of Long, Long)

        For Each r As DataRow In dt.Rows
            If Not IsDBNull(r("NewLedgerID")) Then
                map(CLng(r("LedgerID"))) = CLng(r("NewLedgerID"))
            End If
        Next

        If map.Count = 0 Then Exit Sub

        ' =========================
        ' 2) جلب اللينكات المتأثرة
        ' =========================
        Dim links As New DataTable()

        Using cmd As New SqlCommand("
SELECT *
FROM inv.CostLedgerLink
WHERE IsActive = 1
", con, tran)

            Using da As New SqlDataAdapter(cmd)
                da.Fill(links)
            End Using

        End Using

        Dim affected = links.AsEnumerable().
        Where(Function(r) _
            (Not IsDBNull(r("TargetLedgerID")) AndAlso map.ContainsKey(CLng(r("TargetLedgerID")))) _
         OrElse (Not IsDBNull(r("SourceLedgerID")) AndAlso map.ContainsKey(CLng(r("SourceLedgerID"))))
        ).ToList()

        If affected.Count = 0 Then Exit Sub

        ' =========================
        ' 3) تعطيل القديم
        ' =========================
        For Each r In affected

            Using cmd As New SqlCommand("
UPDATE inv.CostLedgerLink
SET IsActive = 0
WHERE LinkID = @ID
", con, tran)

                cmd.Parameters.AddWithValue("@ID", r("LinkID"))
                cmd.ExecuteNonQuery()

            End Using

        Next

        ' =========================
        ' 4) إنشاء الجديد
        ' =========================
        For Each r In affected

            Dim oldTarget = If(IsDBNull(r("TargetLedgerID")), Nothing, CType(r("TargetLedgerID"), Long?))
            Dim oldSource = If(IsDBNull(r("SourceLedgerID")), Nothing, CType(r("SourceLedgerID"), Long?))

            Dim newTarget As Object = DBNull.Value
            Dim newSource As Object = DBNull.Value

            If oldTarget.HasValue Then
                newTarget = If(map.ContainsKey(oldTarget.Value), map(oldTarget.Value), oldTarget.Value)
            End If

            If oldSource.HasValue Then
                newSource = If(map.ContainsKey(oldSource.Value), map(oldSource.Value), oldSource.Value)
            End If

            ' =========================
            ' جلب Ledger الجديد
            ' =========================
            Dim srcRow As DataRow = Nothing
            Dim tgtRow As DataRow = Nothing

            If Not IsDBNull(newSource) Then
                srcRow = dt.AsEnumerable().
                FirstOrDefault(Function(x) CLng(x("NewLedgerID")) = CLng(newSource))
            End If

            If Not IsDBNull(newTarget) Then
                tgtRow = dt.AsEnumerable().
                FirstOrDefault(Function(x) CLng(x("NewLedgerID")) = CLng(newTarget))
            End If

            ' =========================
            ' تحديد نوع العملية من Ledger
            ' =========================
            Dim opType As Integer = 0

            If srcRow IsNot Nothing Then
                opType = CInt(srcRow("OperationTypeID"))
            ElseIf tgtRow IsNot Nothing Then
                opType = CInt(tgtRow("OperationTypeID"))
            End If

            ' =========================
            ' حساب القيم من Ledger الجديد فقط
            ' =========================
            Dim newQty As Decimal = 0D
            Dim newUnitCost As Decimal = 0D

            ' 🟨 شراء / استلام
            If opType = 7 Or opType = 12 Then

                If tgtRow IsNot Nothing Then
                    newQty = CDec(tgtRow("InQty"))
                    newUnitCost = CDec(tgtRow("InUnitCost"))
                End If

                ' 🟩 قص
            ElseIf opType = 11 Then

                If tgtRow IsNot Nothing Then
                    newQty = CDec(tgtRow("InQty"))
                End If

                If srcRow IsNot Nothing Then
                    newUnitCost = CDec(srcRow("NewAvgCost"))
                End If

                ' 🟥 باقي العمليات (خروج)
            Else

                If srcRow IsNot Nothing Then
                    newQty = CDec(srcRow("OutQty"))
                    newUnitCost = CDec(srcRow("NewAvgCost"))
                End If

            End If

            ' =========================
            ' إدخال اللينك الجديد
            ' =========================
            Using cmd As New SqlCommand("
INSERT INTO inv.CostLedgerLink
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
    CreatedAt,
    CreatedBy,
    LinkDirection
)
VALUES
(
    @Target,
    @Source,
    @Type,
    @Qty,
    @UnitCost,
    @SrcDet,
    @TgtDet,
    @SrcStore,
    @TgtStore,
    @Prod,
    @BaseProd,
    @Date,
    @GroupID,
    @Seq,
    1,
    SYSDATETIME(),
    @UserID,
    @Dir
)
", con, tran)

                cmd.Parameters.AddWithValue("@Target", newTarget)
                cmd.Parameters.AddWithValue("@Source", newSource)
                cmd.Parameters.AddWithValue("@Type", r("LinkType"))

                cmd.Parameters.AddWithValue("@Qty", newQty)
                cmd.Parameters.AddWithValue("@UnitCost", newUnitCost)

                cmd.Parameters.AddWithValue("@SrcDet", If(IsDBNull(r("SourceTransactionDetailID")), DBNull.Value, r("SourceTransactionDetailID")))
                cmd.Parameters.AddWithValue("@TgtDet", If(IsDBNull(r("TargetTransactionDetailID")), DBNull.Value, r("TargetTransactionDetailID")))

                cmd.Parameters.AddWithValue("@SrcStore", If(IsDBNull(r("SourceStoreID")), DBNull.Value, r("SourceStoreID")))
                cmd.Parameters.AddWithValue("@TgtStore", If(IsDBNull(r("TargetStoreID")), DBNull.Value, r("TargetStoreID")))

                cmd.Parameters.AddWithValue("@Prod", r("ProductID"))
                cmd.Parameters.AddWithValue("@BaseProd", If(IsDBNull(r("BaseProductID")), DBNull.Value, r("BaseProductID")))

                cmd.Parameters.AddWithValue("@Date", r("PostingDate"))
                cmd.Parameters.AddWithValue("@GroupID", If(IsDBNull(r("OperationGroupID")), DBNull.Value, r("OperationGroupID")))
                cmd.Parameters.AddWithValue("@Seq", If(IsDBNull(r("GroupSeq")), DBNull.Value, r("GroupSeq")))

                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@Dir", r("LinkDirection"))

                cmd.ExecuteNonQuery()

            End Using

        Next

    End Sub
    Private Function BuildTransactionRebuildPreview(
    dt As DataTable,
    con As SqlConnection,
    tran As SqlTransaction
) As DataTable

        Dim result As New DataTable()

        result.Columns.Add("LedgerID", GetType(Long))
        result.Columns.Add("NewLedgerID", GetType(Long))
        result.Columns.Add("SourceDetailID", GetType(Integer))

        result.Columns.Add("OldQuantity", GetType(Decimal))
        result.Columns.Add("OldUnitCost", GetType(Decimal))
        result.Columns.Add("OldCostAmount", GetType(Decimal))


        result.Columns.Add("NewQuantity", GetType(Decimal))
        result.Columns.Add("NewUnitCost", GetType(Decimal))
        result.Columns.Add("NewCostAmount", GetType(Decimal))

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return result
        End If

        For Each r As DataRow In dt.Rows

            Dim sourceDetailIdObj As Object = r("SourceDetailID")
            If IsDBNull(sourceDetailIdObj) Then Continue For

            Dim sourceDetailId As Integer = CInt(sourceDetailIdObj)

            Dim oldQty As Decimal = 0D
            Dim oldUnitCost As Decimal = 0D
            Dim oldCostAmount As Decimal = 0D
            Dim operationTypeId As Integer = 0
            Dim transactionId As Integer = 0
            Dim documentDetailId As Integer = 0

            Using cmd As New SqlCommand("
SELECT TOP 1
    td.Quantity,
    td.UnitCost,
    td.CostAmount,
    th.OperationTypeID,
    td.TransactionID,
    td.SourceDocumentDetailID
FROM inv.TransactionDetails td
INNER JOIN inv.TransactionHeader th
    ON th.TransactionID = td.TransactionID
WHERE td.DetailID = @DetailID
", con, tran)

                cmd.Parameters.AddWithValue("@DetailID", sourceDetailId)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        oldQty = If(IsDBNull(rd("Quantity")), 0D, CDec(rd("Quantity")))
                        oldUnitCost = If(IsDBNull(rd("UnitCost")), 0D, CDec(rd("UnitCost")))
                        oldCostAmount = If(IsDBNull(rd("CostAmount")), 0D, CDec(rd("CostAmount")))
                        operationTypeId = If(IsDBNull(rd("OperationTypeID")), 0, CInt(rd("OperationTypeID")))
                        transactionId = If(IsDBNull(rd("TransactionID")), 0, CInt(rd("TransactionID")))
                        documentDetailId = If(IsDBNull(rd("SourceDocumentDetailID")), 0, CInt(rd("SourceDocumentDetailID")))
                    End If
                End Using
            End Using

            Dim row = result.NewRow()

            row("LedgerID") = CLng(r("LedgerID"))
            row("NewLedgerID") = If(dt.Columns.Contains("NewLedgerID") AndAlso Not IsDBNull(r("NewLedgerID")), CLng(r("NewLedgerID")), 0L)
            row("SourceDetailID") = sourceDetailId

            row("OldQuantity") = oldQty
            row("OldUnitCost") = oldUnitCost
            row("OldCostAmount") = oldCostAmount


            Dim inQty As Decimal = If(IsDBNull(r("InQty")), 0D, CDec(r("InQty")))
            Dim outQty As Decimal = If(IsDBNull(r("OutQty")), 0D, CDec(r("OutQty")))

            Dim inCost As Decimal = If(IsDBNull(r("InUnitCost")), 0D, CDec(r("InUnitCost")))
            Dim outCost As Decimal = If(IsDBNull(r("OutUnitCost")), 0D, CDec(r("OutUnitCost")))

            Dim newQty As Decimal = 0D
            Dim newCost As Decimal = 0D

            If inQty > 0D Then
                newQty = inQty
                newCost = inCost
            ElseIf outQty > 0D Then
                newQty = outQty
                newCost = outCost
            End If

            row("NewQuantity") = newQty
            row("NewUnitCost") = newCost
            row("NewCostAmount") = newQty * newCost
            result.Rows.Add(row)

        Next

        Return result

    End Function
    Public Function GetTransactionCorrectionPreview(startLedgerIds As List(Of Long)) As DataTable

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    ' 1) Simulation
                    Dim dtResult = SimulateByStartLedgerIds(startLedgerIds)

                    ' 2) Build Preview
                    Dim dtPreview = BuildTransactionRebuildPreview(dtResult, con, tran)

                    tran.Commit()

                    Return dtPreview

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Public Sub RebuildTransactions(dt As DataTable,
                               con As SqlConnection,
                               tran As SqlTransaction)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        ' 🔹 رقم عملية التصحيح
        Dim correctionRunID As Guid = Guid.NewGuid()

        ' 🔹 لتجنب تكرار نفس DetailID
        Dim processed As New HashSet(Of Integer)

        For Each r As DataRow In dt.Rows

            ' =========================
            ' 1) استخراج DetailID
            ' =========================
            If IsDBNull(r("SourceDetailID")) Then Continue For

            Dim detailId As Integer = CInt(r("SourceDetailID"))

            ' منع التكرار (احتياط)
            If processed.Contains(detailId) Then Continue For
            processed.Add(detailId)

            ' =========================
            ' 2) قراءة القيم من المحاكاة
            ' =========================
            Dim inQty As Decimal = If(IsDBNull(r("InQty")), 0D, CDec(r("InQty")))
            Dim outQty As Decimal = If(IsDBNull(r("OutQty")), 0D, CDec(r("OutQty")))

            Dim inCost As Decimal = If(IsDBNull(r("InUnitCost")), 0D, CDec(r("InUnitCost")))
            Dim outCost As Decimal = If(IsDBNull(r("OutUnitCost")), 0D, CDec(r("OutUnitCost")))

            ' =========================
            ' 3) تحديد القيم النهائية
            ' =========================
            Dim newQty As Decimal = 0D
            Dim newCost As Decimal = 0D

            If inQty > 0D Then
                newQty = inQty
                newCost = inCost
            ElseIf outQty > 0D Then
                newQty = outQty
                newCost = outCost
            Else
                ' لا يوجد حركة → تجاهل
                Continue For
            End If

            ' =========================
            ' 4) تنفيذ التحديث
            ' =========================
            Using cmd As New SqlCommand("
UPDATE inv.TransactionDetails
SET
    Quantity = @Qty,
    UnitCost = @Cost,
    CostAmount = @Qty * @Cost,
    CorrectionRunID = @RunID,
    CorrectionReferenceDetailID = @RefID,
    IsCorrection = 1
WHERE DetailID = @ID
", con, tran)

                cmd.Parameters.AddWithValue("@Qty", newQty)
                cmd.Parameters.AddWithValue("@Cost", newCost)
                cmd.Parameters.AddWithValue("@RunID", correctionRunID)
                cmd.Parameters.AddWithValue("@RefID", detailId)
                cmd.Parameters.AddWithValue("@ID", detailId)

                cmd.ExecuteNonQuery()

            End Using

        Next

    End Sub
    Private Sub UpdateTransactionHeadersAsCorrected(dt As DataTable,
                                                con As SqlConnection,
                                                tran As SqlTransaction)

        Dim transactionIds As New HashSet(Of Integer)

        For Each r As DataRow In dt.Rows
            If IsDBNull(r("TransactionID")) Then Continue For
            transactionIds.Add(CInt(r("TransactionID")))
        Next

        For Each transId In transactionIds

            Using cmd As New SqlCommand("
UPDATE inv.TransactionHeader
SET 
    IsCorrection = 1
WHERE TransactionID = @ID
", con, tran)

                cmd.Parameters.AddWithValue("@ID", transId)
                cmd.ExecuteNonQuery()

            End Using

        Next

    End Sub
    Private Sub UpdateCorrectionQueue(startLedgerIds As List(Of Long),
                                  statusId As Integer,
                                  con As SqlConnection,
                                  tran As SqlTransaction,
                                  Optional errorMessage As String = Nothing)

        If startLedgerIds Is Nothing OrElse startLedgerIds.Count = 0 Then Exit Sub

        Dim idsCsv As String = String.Join(",", startLedgerIds)

        Using cmd As New SqlCommand($"
UPDATE inv.CorrectionQueue
SET 
    StatusID = @StatusID,
    LastUpdatedAt = GETDATE(),
    ErrorMessage = @Error
WHERE StartLedgerID IN ({idsCsv})
", con, tran)

            cmd.Parameters.AddWithValue("@StatusID", statusId)

            If String.IsNullOrEmpty(errorMessage) Then
                cmd.Parameters.AddWithValue("@Error", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@Error", errorMessage)
            End If

            cmd.ExecuteNonQuery()

        End Using

    End Sub
#End Region

#Region "Helpers"

    Private Function SafeDec(val As Object) As Decimal
        If val Is Nothing OrElse IsDBNull(val) Then Return 0D
        Return CDec(val)
    End Function

#End Region

End Class