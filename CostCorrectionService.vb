

Imports System.Data
Imports System.Data.SqlClient

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



    ' Purchase
    ' Purchase
    ' Purchase
    Public Function GetAffectedPurchaseOperationsForPreview(
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
        ' 1) استخراج DetailIDs التي تغيّرت فعلاً (بدون أي تكرار دوال في الفورم)
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

        ' لو لا يوجد أي تعديل فعلي -> رجّع فاضي (بدل ما نجيب كل الليدجرات)
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
    SELECT *
    FROM StartLedgers

    UNION ALL

    SELECT l.*
    FROM Inventory_CostLedger l
    JOIN AffectedLedgers a
        ON l.PrevLedgerID = a.LedgerID
    WHERE l.IsActive = 1

    UNION ALL

    SELECT l.*
    FROM Inventory_CostLedgerLink link
    JOIN AffectedLedgers a
        ON link.SourceLedgerID = a.LedgerID
    JOIN Inventory_CostLedger l
        ON l.LedgerID = link.TargetLedgerID
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
        links.Columns.Add("SourceLedgerID", GetType(Long))
        links.Columns.Add("TargetLedgerID", GetType(Long))
        links.Columns.Add("FlowQty", GetType(Decimal))
        links.Columns.Add("FlowUnitCost", GetType(Decimal))
        links.Columns.Add("LinkType", GetType(Integer))

        Dim ids As String =
        String.Join(",", affected.AsEnumerable().
        Select(Function(r) r("LedgerID").ToString()))

        Using con As New SqlConnection(_connectionString)

            Using cmd As New SqlCommand("
SELECT
LinkID,
SourceLedgerID,
TargetLedgerID,
FlowQty,
FlowUnitCost,
LinkType
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

                        row("FlowQty") = r("FlowQty")
                        row("FlowUnitCost") = r("FlowUnitCost")
                        row("LinkType") = r("LinkType")

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


End Class
