

Imports System.Data
Imports System.Data.SqlClient
Imports THE_PROJECT.ProductAverageDTO

Public Class CostCorrectionService

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub



    'GENERAL
    Private Function D(val As Object) As Decimal
        If IsDBNull(val) OrElse val Is Nothing Then
            Return 0D
        End If
        Return CDec(val)
    End Function




    'SIMULATION
    Public Function GetSimulationLinks(affected As DataTable) As DataTable

        Dim links As New DataTable()

        links.Columns.Add("LinkID", GetType(Long))
        links.Columns.Add("TargetLedgerID", GetType(Long))
        links.Columns.Add("SourceLedgerID", GetType(Long))
        links.Columns.Add("LinkType", GetType(Integer))

        links.Columns.Add("FlowQty", GetType(Decimal))
        links.Columns.Add("FlowUnitCost", GetType(Decimal))
        links.Columns.Add("FlowTotalCost", GetType(Decimal))

        links.Columns.Add("ProductID", GetType(Integer))
        links.Columns.Add("BaseProductID", GetType(Integer))

        links.Columns.Add("PostingDate", GetType(DateTime))
        links.Columns.Add("OperationGroupID", GetType(String))
        links.Columns.Add("GroupSeq", GetType(Integer))
        links.Columns.Add("LinkDirection", GetType(Integer))

        If affected Is Nothing OrElse affected.Rows.Count = 0 Then
            Return links
        End If

        Dim ids As String =
        String.Join(",", affected.AsEnumerable().
        Select(Function(r) r("LedgerID").ToString()))

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
SELECT
    LinkID,
    TargetLedgerID,
    SourceLedgerID,
    LinkType,
    FlowQty,
    FlowUnitCost,
    FlowTotalCost,
    ProductID,
    BaseProductID,
    PostingDate,
    OperationGroupID,
    GroupSeq,
    LinkDirection
FROM Inventory_CostLedgerLink
WHERE SourceLedgerID IN (" & ids & ")
   OR TargetLedgerID IN (" & ids & ")
", con)

                con.Open()

                Using r = cmd.ExecuteReader()

                    While r.Read()

                        Dim row = links.NewRow()

                        row("LinkID") = r("LinkID")
                        row("SourceLedgerID") = r("SourceLedgerID")
                        row("TargetLedgerID") = r("TargetLedgerID")
                        row("LinkType") = r("LinkType")

                        row("FlowQty") = r("FlowQty")
                        row("FlowUnitCost") = r("FlowUnitCost")

                        ' FlowTotalCost قد يكون NULL
                        row("FlowTotalCost") = If(IsDBNull(r("FlowTotalCost")), 0D, r("FlowTotalCost"))

                        ' Product / BaseProduct قد تكون NULL
                        row("ProductID") = If(IsDBNull(r("ProductID")), 0, r("ProductID"))
                        row("BaseProductID") = If(IsDBNull(r("BaseProductID")), 0, r("BaseProductID"))

                        row("PostingDate") = If(IsDBNull(r("PostingDate")), DateTime.MinValue, r("PostingDate"))

                        ' OperationGroupID غالبًا UNIQUEIDENTIFIER -> نخليه String لتفادي مشاكل النوع
                        row("OperationGroupID") = If(IsDBNull(r("OperationGroupID")), "", r("OperationGroupID").ToString())

                        row("GroupSeq") = If(IsDBNull(r("GroupSeq")), 0, CInt(r("GroupSeq")))
                        row("LinkDirection") = If(IsDBNull(r("LinkDirection")), 0, CInt(r("LinkDirection")))

                        links.Rows.Add(row)

                    End While

                End Using

            End Using

        End Using

        Return links

    End Function
    Public Sub RecalculateSimulationFlows(
    simLedgers As DataTable,
    links As DataTable
)

        For Each link As DataRow In links.Rows

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))

            Dim sourceRow =
            simLedgers.Select("LedgerID=" & sourceID).FirstOrDefault()

            If sourceRow Is Nothing Then Continue For

            Dim avgCost As Decimal = CDec(sourceRow("SimAvgCost"))

            link("FlowUnitCost") = avgCost

        Next

    End Sub



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
        result.Columns.Add("RootLedgerID", GetType(Long))
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
    -- البداية
    SELECT *
    FROM StartLedgers

    UNION ALL

    -- 1) أي Ledger يعتمد مباشرة على Ledger سابق
    SELECT l.*
    FROM Inventory_CostLedger l
    INNER JOIN AffectedLedgers a
        ON l.DependsOnLedgerID = a.LedgerID
    WHERE l.IsActive = 1
      AND l.IsReversed = 0

    UNION ALL

    -- 2) التمدد عبر الروابط الفعلية للجراف
    -- مهم: في تصميمك الحالي الاتجاه المؤثر للأمام هو LinkDirection = 2
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
    RootLedgerID,
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
        cl.RootLedgerID,
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

                        row("RootLedgerID") =
                        If(IsDBNull(r("RootLedgerID")),
                           DBNull.Value,
                           r("RootLedgerID"))

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


    'PUR,SRT
    Public Function GetDocumentHeaderForRevaluation(documentID As Integer) _
        As RevaluationDocumentHeaderDto

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
SELECT
    DocumentID,
    DocumentNo,
    DocumentDate,
    PartnerID,
    TotalAmount,
    TotalTax,
    TotalNetAmount,
    PaymentMethodID,
    PaymentTermID,
    Notes,
    StatusID,
    IsInventoryPosted,
    IsTaxInclusive
FROM Inventory_DocumentHeader
WHERE DocumentID=@ID
", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()

                Using r = cmd.ExecuteReader()

                    If Not r.Read() Then Return Nothing

                    Dim dto As New RevaluationDocumentHeaderDto

                    dto.DocumentID = CInt(r("DocumentID"))
                    dto.DocumentNo = r("DocumentNo").ToString
                    dto.DocumentDate = CDate(r("DocumentDate"))

                    dto.PartnerID =
                        If(IsDBNull(r("PartnerID")),
                           CType(Nothing, Integer?),
                           CInt(r("PartnerID")))

                    dto.TotalAmount = ToDec(r("TotalAmount"))
                    dto.TotalTax = ToDec(r("TotalTax"))
                    dto.TotalNetAmount = ToDec(r("TotalNetAmount"))

                    dto.IsInventoryPosted =
                        Not IsDBNull(r("IsInventoryPosted")) _
                        AndAlso CBool(r("IsInventoryPosted"))

                    dto.IsTaxInclusive =
                        Not IsDBNull(r("IsTaxInclusive")) _
                        AndAlso CBool(r("IsTaxInclusive"))

                    Return dto

                End Using

            End Using

        End Using

    End Function
    ' COLUMNS FOR GRID PUR,SRT
    Public Function GetDocumentDetailsForRevaluation(documentID As Integer) As DataTable


        Dim dt As New DataTable()

        dt.Columns.Add("DetailID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("ProductCode", GetType(String))
        dt.Columns.Add("ProductName", GetType(String))
        dt.Columns.Add("ProductTypeID", GetType(String))
        dt.Columns.Add("UnitID", GetType(String))
        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))

        dt.Columns.Add("SourceStoreID", GetType(Integer))
        dt.Columns.Add("TargetStoreID", GetType(Integer))

        dt.Columns.Add("OldUnitPrice", GetType(Decimal))
        dt.Columns.Add("NewUnitPrice", GetType(Decimal))

        dt.Columns.Add("GrossAmount", GetType(Decimal))
        dt.Columns.Add("DiscountRate", GetType(Decimal))
        dt.Columns.Add("DiscountAmount", GetType(Decimal))

        dt.Columns.Add("IncludeTax", GetType(Boolean))
        dt.Columns.Add("TaxRate", GetType(Decimal))
        dt.Columns.Add("TaxTypeID", GetType(Integer))

        dt.Columns.Add("TaxableAmount", GetType(Decimal))
        dt.Columns.Add("TaxAmount", GetType(Decimal))

        dt.Columns.Add("NetAmount", GetType(Decimal))
        dt.Columns.Add("LineTotal", GetType(Decimal))

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("


SELECT
d.DetailID,
d.ProductID,
p.ProductCode,
p.ProductName,
t.TypeName,
d.Quantity,
u.UnitName,
d.UnitPrice,
d.GrossAmount,
d.DiscountRate,
d.DiscountAmount,
d.TaxTypeID,
d.TaxRate,
d.TaxableAmount,
d.TaxAmount,
d.NetAmount,
d.LineTotal,
d.SourceStoreID,
d.TargetStoreID
FROM Inventory_DocumentDetails d
INNER JOIN Master_Product p
ON p.ProductID = d.ProductID
INNER JOIN Master_ProductType t
ON t.ProductTypeID = p.ProductTypeID
LEFT JOIN Master_Unit u
ON u.UnitID = d.UnitID
WHERE d.DocumentID = @DocumentID
ORDER BY d.DetailID
", con)


                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                con.Open()

                Using r = cmd.ExecuteReader()

                    While r.Read()

                        Dim row = dt.NewRow()

                        row("DetailID") = r("DetailID")
                        row("ProductID") = r("ProductID")
                        row("ProductCode") = r("ProductCode").ToString()
                        row("ProductName") = r("ProductName").ToString()
                        row("ProductTypeID") = r("TypeName").ToString()
                        row("UnitID") = r("UnitName").ToString()


                        row("SourceStoreID") =
                    If(IsDBNull(r("SourceStoreID")), DBNull.Value, r("SourceStoreID"))

                        row("TargetStoreID") =
                    If(IsDBNull(r("TargetStoreID")), DBNull.Value, r("TargetStoreID"))

                        ' الكمية
                        row("OldQty") = r("Quantity")
                        row("NewQty") = r("Quantity")

                        ' السعر
                        row("OldUnitPrice") = r("UnitPrice")
                        row("NewUnitPrice") = r("UnitPrice")

                        row("GrossAmount") = r("GrossAmount")
                        row("DiscountRate") = r("DiscountRate")
                        row("DiscountAmount") = r("DiscountAmount")

                        row("TaxTypeID") = r("TaxTypeID")
                        row("TaxRate") = r("TaxRate")

                        row("TaxableAmount") = r("TaxableAmount")
                        row("TaxAmount") = r("TaxAmount")

                        row("NetAmount") = r("NetAmount")
                        row("LineTotal") = r("LineTotal")

                        row("IncludeTax") = False

                        dt.Rows.Add(row)

                    End While

                End Using

            End Using

        End Using

        Return dt


    End Function
    Public Function BuildAffectedPreviewReval(
        items As List(Of RevalAffectedInputDto)
    ) As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("DetailID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("ProductCode", GetType(String))
        dt.Columns.Add("ProductName", GetType(String))
        dt.Columns.Add("ProductTypeID", GetType(String))
        dt.Columns.Add("UnitID", GetType(String))

        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))

        dt.Columns.Add("SourceStoreID", GetType(Integer))
        dt.Columns.Add("TargetStoreID", GetType(Integer))

        dt.Columns.Add("OldUnitPrice", GetType(Decimal))
        dt.Columns.Add("NewUnitPrice", GetType(Decimal))
        dt.Columns.Add("GrossAmount", GetType(Decimal))

        dt.Columns.Add("DiscountRate", GetType(Decimal))
        dt.Columns.Add("DiscountAmount", GetType(Decimal))

        dt.Columns.Add("IncludeTax", GetType(Boolean))
        dt.Columns.Add("TaxRate", GetType(Decimal))
        dt.Columns.Add("TaxTypeID", GetType(Integer))

        dt.Columns.Add("TaxableAmount", GetType(Decimal))
        dt.Columns.Add("TaxAmount", GetType(Decimal))

        dt.Columns.Add("NetAmount", GetType(Decimal))
        dt.Columns.Add("LineTotal", GetType(Decimal))

        If items Is Nothing Then Return dt

        For Each x In items

            Dim row = dt.NewRow()

            row("DetailID") = x.DetailID
            row("ProductID") = x.ProductID
            row("ProductCode") = x.ProductCode
            row("ProductName") = x.ProductName
            row("ProductTypeID") = x.ProductTypeID
            row("UnitID") = x.UnitID

            row("OldQty") = x.OldQty
            row("NewQty") = x.NewQty

            row("SourceStoreID") =
                If(x.SourceStoreID.HasValue, x.SourceStoreID.Value, DBNull.Value)

            row("TargetStoreID") =
                If(x.TargetStoreID.HasValue, x.TargetStoreID.Value, DBNull.Value)

            row("OldUnitPrice") = x.OldUnitPrice
            row("NewUnitPrice") = x.NewUnitPrice
            row("GrossAmount") = x.GrossAmount

            row("DiscountRate") = x.DiscountRate
            row("DiscountAmount") = x.DiscountAmount

            row("IncludeTax") = x.IncludeTax
            row("TaxRate") = x.TaxRate

            row("TaxTypeID") =
                If(x.TaxTypeID.HasValue, x.TaxTypeID.Value, DBNull.Value)

            row("TaxableAmount") = x.TaxableAmount
            row("TaxAmount") = x.TaxAmount

            row("NetAmount") = x.NetAmount
            row("LineTotal") = x.LineTotal

            dt.Rows.Add(row)

        Next

        Return dt

    End Function

    Public Function GetChangedDetails_PUR_SRT(documentID As Integer) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
            SELECT t.DetailID
            FROM Inventory_TransactionDetails t
            INNER JOIN Inventory_DocumentDetails d
                ON t.SourceDocumentDetailID = d.DetailID
            WHERE d.DocumentID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using

        End Using

        Return dt

    End Function


    'PRODUCTION

    Public Function GetProductionForRevaluation(productionID As Integer) As ProductionRevaluationDto

        Dim result As New ProductionRevaluationDto()

        Using con As New SqlConnection(_connectionString)
            con.Open()

            '========================================
            ' 1) Header
            '========================================
            Using cmd As New SqlCommand("
    SELECT 

         h.ProductionCode,
        h.ProductionBaseValue,
        h.StockTransactionID,
        h.ProductID,
        p.ProductCode,
        u.UnitName,
        p.ProductSubCategoryID
    FROM Production_Header h
    INNER JOIN Master_Product p 
        ON p.ProductID = h.ProductID
    INNER JOIN Master_Unit u 
        ON u.UnitID = p.ProductionUnitID
    WHERE h.ProductionID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", productionID)

                Using r = cmd.ExecuteReader()

                    If r.Read() Then
                        result.ProductionCode =
            If(IsDBNull(r("ProductionCode")), "", r("ProductionCode").ToString())
                        result.ProductionBaseValue =
                If(IsDBNull(r("ProductionBaseValue")), 0D, Convert.ToDecimal(r("ProductionBaseValue")))

                        result.TransactionID =
                If(IsDBNull(r("StockTransactionID")), 0, Convert.ToInt32(r("StockTransactionID")))

                        result.ProductID =
                If(IsDBNull(r("ProductID")), 0, Convert.ToInt32(r("ProductID")))

                        result.ProductCode =
                If(IsDBNull(r("ProductCode")), "", r("ProductCode").ToString())

                        result.UnitName =
                If(IsDBNull(r("UnitName")), "", r("UnitName").ToString())
                        result.SubCategoryID =
                            If(IsDBNull(r("ProductSubCategoryID")), 0, Convert.ToInt32(r("ProductSubCategoryID")))
                    End If

                End Using
            End Using

            '========================================
            ' 2) Posting Date
            '========================================
            Using cmd As New SqlCommand("
        SELECT PostingDate
        FROM Inventory_TransactionHeader
        WHERE TransactionID=@T
        ", con)

                cmd.Parameters.AddWithValue("@T", result.TransactionID)
                result.PostingDate = Convert.ToDateTime(cmd.ExecuteScalar())
            End Using

            '========================================
            ' 3) Inputs
            '========================================
            Dim dtInputs As New DataTable()

            Using cmd As New SqlCommand("
SELECT 
    d.SourceDocumentDetailID,   -- 🔥 هذا هو المفتاح الحقيقي

    d.ProductID,
    p.ProductCode,
    p.ProductName,

    d.Quantity,
    d.UnitCost,
    (d.Quantity * d.UnitCost) AS TotalCost

FROM Inventory_TransactionDetails d

INNER JOIN Production_Consumption c
    ON c.ConsumptionID = d.SourceDocumentDetailID   -- 🔥 الربط الصحيح

INNER JOIN Master_Product p
    ON p.ProductID = d.ProductID

WHERE d.TransactionID = @T
  AND d.SourceStoreID IS NOT NULL

        ", con)

                cmd.Parameters.AddWithValue("@T", result.TransactionID)
                dtInputs.Load(cmd.ExecuteReader())
            End Using

            result.Inputs = dtInputs

            '========================================
            ' 4) Outputs
            '========================================

            Dim dtOutputs As New DataTable()

            Using cmd As New SqlCommand("
SELECT 
    d.SourceDocumentDetailID,   -- 🔥 مهم

    d.ProductID,
    p.ProductCode,
    p.ProductName,

    o.Quantity,
    o.Length,
    o.Width,
    o.Height,
    o.VolumeM3

FROM Inventory_TransactionDetails d

INNER JOIN Production_Output o
    ON o.OutputID = d.SourceDocumentDetailID   -- 🔥 الربط الصحيح

INNER JOIN Master_Product p
    ON p.ProductID = d.ProductID

WHERE d.TransactionID = @T
  AND d.TargetStoreID IS NOT NULL
", con)

                cmd.Parameters.AddWithValue("@T", result.TransactionID)

                dtOutputs.Load(cmd.ExecuteReader())

            End Using


            If dtOutputs.Columns.Contains("VolumeM3") Then
                dtOutputs.Columns("VolumeM3").ReadOnly = False
            End If
            result.Outputs = dtOutputs
            For Each r As DataRow In dtOutputs.Rows

                Dim l = ToDec(r("Length"))
                Dim w = ToDec(r("Width"))
                Dim h = ToDec(r("Height"))
                Dim q = ToDec(r("Quantity"))

                r("VolumeM3") = (l * w * h * q) / 1000000D

            Next
            '========================================
            ' 5) Calculations
            '========================================
            result.TotalChemicalCost =
                dtInputs.AsEnumerable().
                Sum(Function(r) ToDec(r("TotalCost")))

            result.TotalChemicalQty =
    dtInputs.AsEnumerable().
    Sum(Function(r) ToDec(r("Quantity")))

            result.TotalProductionVolume =
                dtOutputs.AsEnumerable().
                Sum(Function(r) ToDec(r("VolumeM3")))
            result.TotalProductionQty =
    dtOutputs.AsEnumerable().
    Sum(Function(r) ToDec(r("Quantity")))

            result.TotalChemicalQty =
                result.TotalChemicalQty

            Dim totalCost As Decimal = result.TotalChemicalCost
            Select Case result.SubCategoryID

                Case 9, 10
                    If result.TotalProductionVolume > 0 Then
                        result.UnitCost = totalCost / result.TotalProductionVolume
                    End If

                Case 11
                    If result.TotalProductionQty > 0 Then
                        result.UnitCost = totalCost / result.TotalProductionQty
                    End If
            End Select
            '========================================
            ' 6) Past Avg Cost
            '========================================
            Using cmd As New SqlCommand("
        SELECT TOP 1 NewAvgCost
        FROM Inventory_CostLedger
        WHERE ProductID=@P
          AND PostingDate < @D
        ORDER BY PostingDate DESC, LedgerID DESC
        ", con)

                cmd.Parameters.AddWithValue("@P", result.ProductID)
                cmd.Parameters.AddWithValue("@D", result.PostingDate)

                Dim v = cmd.ExecuteScalar()

                If v IsNot Nothing AndAlso Not IsDBNull(v) Then
                    result.PastAvgCost = Convert.ToDecimal(v)
                Else
                    result.PastAvgCost = 0D
                End If
            End Using

        End Using

        Return result

    End Function
    Public Function GetProductionChangedDetailIDs(
    productionID As Integer,
    transactionID As Integer,
    inputs As DataTable,
    newFgQty As Decimal,
    newFgCost As Decimal
) As List(Of Integer)

        Dim result As New List(Of Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            '========================================
            ' 1) Get TransactionDetails
            '========================================
            Dim dtDetails As New DataTable()

            Using cmd As New SqlCommand("
            SELECT *
            FROM Inventory_TransactionDetails
            WHERE TransactionID = @T
        ", con)

                cmd.Parameters.AddWithValue("@T", transactionID)
                dtDetails.Load(cmd.ExecuteReader())
            End Using

            '========================================
            ' 2) Inputs (Consumption)
            '========================================
            For Each r As DataRow In inputs.Rows

                Dim productID As Integer = CInt(r("ProductID"))
                Dim oldQty As Decimal = ToDec(r("OriginalQuantity"))
                Dim newQty As Decimal = ToDec(r("Quantity"))

                If Math.Abs(oldQty - newQty) < 0.0001D Then Continue For

                Dim match =
                    dtDetails.AsEnumerable().
                    FirstOrDefault(Function(x)
                                       Return CInt(x("ProductID")) = productID AndAlso
                                          Not IsDBNull(x("SourceStoreID"))
                                   End Function)

                If match IsNot Nothing Then
                    result.Add(CInt(match("DetailID")))
                End If

            Next

            '========================================
            ' 3) Output (FG)
            '========================================
            Dim fgRow =
                dtDetails.AsEnumerable().
                FirstOrDefault(Function(x)
                                   Return IsDBNull(x("SourceStoreID")) AndAlso
                                      Not IsDBNull(x("TargetStoreID"))
                               End Function)

            If fgRow IsNot Nothing Then

                Dim oldQty As Decimal = ToDec(fgRow("Quantity"))
                Dim oldCost As Decimal = ToDec(fgRow("UnitCost"))

                If Math.Abs(oldQty - newFgQty) > 0.0001D OrElse
                   Math.Abs(oldCost - newFgCost) > 0.0001D Then

                    result.Add(CInt(fgRow("DetailID")))
                End If

            End If

        End Using

        Return result

    End Function
    Public Function GetChangedDetails_PRO(productionID As Integer) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
            SELECT DetailID
            FROM Inventory_TransactionDetails
            WHERE SourceDocumentDetailID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", productionID)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using

        End Using

        Return dt

    End Function


    'CUT
    Public Function GetCuttingForRevaluation(cuttingID As Integer) As CuttingRevaluationDto

        Dim result As New CuttingRevaluationDto()

        Using con As New SqlConnection(_connectionString)
            con.Open()

            '========================================
            ' 1) Header
            '========================================
            Using cmd As New SqlCommand("
SELECT 
    h.CuttingCode,
    bp.ProductCode AS BaseProductCode,
    ISNULL(b.QtyOnHand, 0) AS AvailableQty
FROM Production_CuttingHeader h
LEFT JOIN Inventory_Balance b
    ON b.BaseProductID = h.BaseProductID
    AND b.StoreID = h.SourceStoreID
LEFT JOIN Master_Product bp
    ON bp.ProductID = h.BaseProductID
WHERE h.CuttingID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                Using r = cmd.ExecuteReader()
                    If r.Read() Then
                        result.CuttingCode = r("CuttingCode").ToString()
                        result.BaseProductCode = r("BaseProductCode").ToString()
                        result.AvailableQty = ToDec(r("AvailableQty"))
                    End If
                End Using
            End Using

            '========================================
            ' 2) Outputs (🔥 مع DetailID)
            '========================================
            Dim dt As New DataTable()

            Using cmd As New SqlCommand("
SELECT 
    o.CutOutputID AS SourceDocumentDetailID,   -- 🔥 أهم سطر
    o.ProductID,
    p.ProductCode,
 p.ProductName,
    t.TypeName AS ProductType,
    o.Length_cm,
    o.Width_cm,
    o.Height_cm,
    o.QtyPieces AS Quantity,
    o.SourceStoreID,
    s.StoreName,
    o.IsMix,
    ISNULL(td.UnitCost, 0) AS UnitCost

FROM Production_CuttingOutput o

INNER JOIN Master_Product p
    ON p.ProductID = o.ProductID

INNER JOIN Master_ProductType t
    ON t.ProductTypeID = p.ProductTypeID

LEFT JOIN Master_Store s
    ON s.StoreID = o.SourceStoreID

OUTER APPLY
(
    SELECT TOP 1 UnitCost
    FROM Inventory_TransactionDetails td
    WHERE td.SourceDocumentDetailID = o.CutOutputID
    ORDER BY td.DetailID
) td

WHERE o.CutID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)
                dt.Load(cmd.ExecuteReader())
            End Using

            '========================================
            ' 3) حساب الأحجام
            '========================================
            If Not dt.Columns.Contains("PieceVolume") Then
                dt.Columns.Add("PieceVolume", GetType(Decimal))
            End If

            If Not dt.Columns.Contains("TotalVolume") Then
                dt.Columns.Add("TotalVolume", GetType(Decimal))
            End If

            For Each row As DataRow In dt.Rows

                Dim l = ToDec(row("Length_cm"))
                Dim w = ToDec(row("Width_cm"))
                Dim h = ToDec(row("Height_cm"))
                Dim q = ToDec(row("Quantity"))

                Dim pieceVolume As Decimal = (l * w * h) / 1000000D
                Dim totalVolume As Decimal = pieceVolume * q

                row("PieceVolume") = pieceVolume
                row("TotalVolume") = totalVolume

            Next

            result.Outputs = dt

        End Using

        Return result

    End Function
    Public Function GetChangedDetails_CUT(cuttingID As Integer) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
            SELECT DetailID
            FROM Inventory_TransactionDetails
            WHERE SourceDocumentDetailID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using

        End Using

        Return dt

    End Function







End Class
