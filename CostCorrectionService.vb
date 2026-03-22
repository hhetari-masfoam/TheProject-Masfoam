

Imports System.Data
Imports System.Data.SqlClient
Imports THE_PROJECT.ProductAverageDTO

Public Class CostCorrectionService

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub


    '====================================================
    ' HEADER
    '====================================================

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


    '====================================================
    ' DETAILS
    '====================================================

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



    Public Function GetAffectedCostDependenciesForPreview(
    details As DataTable,
    documentID As Integer
) As DataTable

        Dim result As New DataTable()

        result.Columns.Add("LedgerID", GetType(Long))
        result.Columns.Add("TransactionID", GetType(Integer))
        result.Columns.Add("SourceDetailID", GetType(Integer))

        result.Columns.Add("ProductID", GetType(Integer))
        result.Columns.Add("BaseProductID", GetType(Integer))
        result.Columns.Add("StoreID", GetType(Integer))
        result.Columns.Add("OperationTypeID", GetType(Integer))

        result.Columns.Add("PostingDate", GetType(DateTime))

        result.Columns.Add("OldQty", GetType(Decimal))
        result.Columns.Add("NewQty", GetType(Decimal))

        result.Columns.Add("LocalOldQty", GetType(Decimal))
        result.Columns.Add("LocalNewQty", GetType(Decimal))

        result.Columns.Add("InQty", GetType(Decimal))
        result.Columns.Add("OutQty", GetType(Decimal))

        result.Columns.Add("OldAvgCost", GetType(Decimal))
        result.Columns.Add("NewAvgCost", GetType(Decimal))

        result.Columns.Add("InUnitCost", GetType(Decimal))
        result.Columns.Add("OutUnitCost", GetType(Decimal))
        result.Columns.Add("LedgerSequence", GetType(Long))
        result.Columns.Add("SourceLedgerID", GetType(Long))
        result.Columns.Add("RootLedgerID", GetType(Long))
        result.Columns.Add("RootTransactionID", GetType(Long))
        result.Columns.Add("SupersededByLedgerID", GetType(Long))

        ' -----------------------------
        ' 1) Extract actually-changed DetailIDs
        ' -----------------------------
        Dim changedIds As New List(Of Integer)()

        If details IsNot Nothing AndAlso details.Rows.Count > 0 Then

            Dim hasDetailID As Boolean = details.Columns.Contains("DetailID")
            Dim hasOldQty As Boolean = details.Columns.Contains("OldQty")
            Dim hasNewQty As Boolean = details.Columns.Contains("NewQty")
            Dim hasOldPrice As Boolean = details.Columns.Contains("OldUnitPrice")
            Dim hasNewPrice As Boolean = details.Columns.Contains("NewUnitPrice")

            If hasDetailID AndAlso hasOldQty AndAlso hasNewQty AndAlso hasOldPrice AndAlso hasNewPrice Then
                For Each r As DataRow In details.Rows
                    If r.RowState = DataRowState.Deleted Then Continue For

                    Dim oldQty As Decimal = ToDec(r("OldQty"))
                    Dim newQty As Decimal = ToDec(r("NewQty"))
                    Dim oldPrice As Decimal = ToDec(r("OldUnitPrice"))
                    Dim newPrice As Decimal = ToDec(r("NewUnitPrice"))

                    If oldQty <> newQty OrElse oldPrice <> newPrice Then
                        changedIds.Add(CInt(r("DetailID")))
                    End If
                Next
            End If
        End If

        If changedIds.Count = 0 Then
            Return result
        End If

        changedIds = changedIds.Distinct().ToList()
        Dim changedIdsCsv As String = String.Join(",", changedIds)

        Using con As New SqlConnection(_connectionString)

            con.Open()

            Dim sql As String = "
;WITH ChangedDetails AS
(
    SELECT TRY_CONVERT(INT, value) AS DetailID
    FROM string_split(@ChangedDetailIDs, ',')
    WHERE TRY_CONVERT(INT, value) IS NOT NULL
),

StartLedgers AS
(
    -- same start logic you had (document -> transaction details -> cost ledger)
    SELECT cl.*
    FROM Inventory_CostLedger cl
    WHERE cl.SourceDetailID IN
    (
        SELECT td.DetailID
        FROM Inventory_TransactionDetails td
        WHERE td.SourceDocumentDetailID IN (SELECT DetailID FROM ChangedDetails)
    )
),

AffectedLedgers AS
(
    -- seed
    SELECT *
    FROM StartLedgers

    UNION ALL

    -- 1) Dependency expansion: anything whose cost depends on a previous ledger
    SELECT l.*
    FROM Inventory_CostLedger l
    JOIN AffectedLedgers a
        ON l.DependsOnLedgerID = a.LedgerID
    WHERE l.IsActive = 1
      AND l.IsReversed = 0

    UNION ALL

    -- 2) Dependency expansion via links, but ONLY by BACKWARD direction (depends-on)
    -- LinkDirectionID = 2 (BACKWARD) based on your table Inventory_CostLinkDirection
    SELECT l.*
    FROM Inventory_CostLedgerLink link
    JOIN AffectedLedgers a
        ON link.SourceLedgerID = a.LedgerID
    JOIN Inventory_CostLedger l
        ON l.LedgerID = link.TargetLedgerID
    WHERE link.IsActive = 1
      AND l.IsActive = 1
      AND l.IsReversed = 0
      AND link.LinkDirection = 2
)

SELECT *
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
            ORDER BY cl.LedgerID
        ) AS rn

    FROM AffectedLedgers cl
) x
WHERE rn = 1
ORDER BY
    PostingDate,
    LedgerSequence,
    LedgerID
"

            Using cmd As New SqlCommand(sql, con)

                cmd.Parameters.AddWithValue("@ChangedDetailIDs", changedIdsCsv)

                Using reader = cmd.ExecuteReader()

                    While reader.Read()

                        Dim row = result.NewRow()

                        row("LedgerID") = reader("LedgerID")
                        row("TransactionID") = reader("TransactionID")

                        row("SourceDetailID") =
                        If(IsDBNull(reader("SourceDetailID")),
                           DBNull.Value,
                           reader("SourceDetailID"))

                        row("ProductID") = reader("ProductID")
                        row("StoreID") = reader("StoreID")

                        row("OperationTypeID") = reader("OperationTypeID")
                        row("PostingDate") = reader("PostingDate")

                        row("OldQty") = reader("OldQty")
                        row("NewQty") = reader("NewQty")

                        row("LocalOldQty") = reader("LocalOldQty")
                        row("LocalNewQty") = reader("LocalNewQty")

                        row("InQty") = reader("InQty")
                        row("OutQty") = reader("OutQty")

                        row("OldAvgCost") = reader("OldAvgCost")
                        row("NewAvgCost") = reader("NewAvgCost")

                        row("InUnitCost") = reader("InUnitCost")
                        row("OutUnitCost") = reader("OutUnitCost")
                        row("BaseProductID") = reader("BaseProductID")

                        row("LedgerSequence") = reader("LedgerSequence")

                        row("SourceLedgerID") =
                        If(IsDBNull(reader("SourceLedgerID")), DBNull.Value, reader("SourceLedgerID"))

                        row("RootLedgerID") =
                        If(IsDBNull(reader("RootLedgerID")), DBNull.Value, reader("RootLedgerID"))

                        row("SupersededByLedgerID") =
                        If(IsDBNull(reader("SupersededByLedgerID")), DBNull.Value, reader("SupersededByLedgerID"))

                        row("RootTransactionID") =
                        If(IsDBNull(reader("RootTransactionID")), DBNull.Value, reader("RootTransactionID"))

                        result.Rows.Add(row)

                    End While

                End Using

            End Using

        End Using

        Return result

    End Function


    Private Function D(val As Object) As Decimal
        If IsDBNull(val) OrElse val Is Nothing Then
            Return 0D
        End If
        Return CDec(val)
    End Function


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
        ON u.UnitID = p.StorageUnitID
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
    d.ProductID,
    p.ProductCode,
    p.ProductName,
    d.Quantity,
    d.UnitCost,
    (d.Quantity * d.UnitCost) AS TotalCost
FROM Inventory_TransactionDetails d
INNER JOIN Master_Product p
    ON p.ProductID = d.ProductID
WHERE d.TransactionID=@T
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
    ProductID,
    Quantity,
    Length,
    Width,
    Height,
    VolumeM3
FROM Production_Output
WHERE ProductionID=@P
        ", con)

                cmd.Parameters.AddWithValue("@P", productionID)
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


End Class
