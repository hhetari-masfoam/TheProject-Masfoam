Imports System.Data.SqlClient

Public Class frmGoodsIssue
    Inherits AABaseOperationForm
    Private SourceLOID As Integer = 0
    Private SourceSRID As Integer = 0

    Private GoodsIssueDetailsDT As DataTable
    Private Sub frmGoodsIssue_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' جريد التحميل
        SetupLoadingGrid()

        ' جريد التفاصيل
        InitGoodsIssueDetailsTable()
        SetupGoodsIssueGrid()
        dgvSRD.DataSource = GoodsIssueDetailsDT

    End Sub
    Private Sub SetupLoadingGrid()

        dgvLO.Columns.Clear()
        dgvLO.AutoGenerateColumns = False

        ' LOID (مخفي)
        Dim colLOID As New DataGridViewTextBoxColumn()
        colLOID.Name = "colLOID"
        colLOID.HeaderText = "LOID"
        colLOID.DataPropertyName = "LOID"
        colLOID.Visible = False
        dgvLO.Columns.Add(colLOID)

        ' LO Code
        Dim colLOCode As New DataGridViewTextBoxColumn()
        colLOCode.Name = "colLOCode"
        colLOCode.HeaderText = "رقم أمر التحميل"
        colLOCode.DataPropertyName = "LOCode"
        dgvLO.Columns.Add(colLOCode)

        ' Driver
        Dim colDriver As New DataGridViewTextBoxColumn()
        colDriver.Name = "colDriverName"
        colDriver.HeaderText = "السائق"
        colDriver.DataPropertyName = "DriverName"
        dgvLO.Columns.Add(colDriver)

        ' Vehicle
        Dim colVehicle As New DataGridViewTextBoxColumn()
        colVehicle.Name = "colVehicleCode"
        colVehicle.HeaderText = "السيارة"
        colVehicle.DataPropertyName = "VehicleCode"
        dgvLO.Columns.Add(colVehicle)

        ' SRID (مخفي)
        Dim colSRID As New DataGridViewTextBoxColumn()
        colSRID.Name = "colSRID"
        colSRID.HeaderText = "SRID"
        colSRID.DataPropertyName = "SRID"
        colSRID.Visible = False
        dgvLO.Columns.Add(colSRID)

        ' SR Code
        Dim colSRCode As New DataGridViewTextBoxColumn()
        colSRCode.Name = "colSRCode"
        colSRCode.HeaderText = "طلب المبيعات"
        colSRCode.DataPropertyName = "SRCode"
        dgvLO.Columns.Add(colSRCode)

    End Sub

    Private Sub btnImportSRWaitingIssue_Click(
    sender As Object,
    e As EventArgs
) Handles btnImportSRWaitingIssue.Click

        Dim f As New frmLoadingBoard()

        f.CurrentMode = frmLoadingBoard.LoadingBoardMode.GoodsIssueSelection
        f.IsOpenedFromInvoice = True   ' نستخدم نفس الفلترة الحالية مؤقتًا

        If f.ShowDialog() <> DialogResult.OK Then Exit Sub

        If f.SelectedLOID <= 0 OrElse f.SelectedSRID <= 0 Then
            MessageBox.Show("لم يتم اختيار أمر تحميل وطلب مبيعات صالحين.", "تنبيه")
            Exit Sub
        End If

        SourceLOID = f.SelectedLOID
        SourceSRID = f.SelectedSRID

        LoadGoodsIssueFromLoading(SourceLOID, SourceSRID)

    End Sub
    Private Sub LoadGoodsIssueFromLoading(loID As Integer, srID As Integer)

        If loID <= 0 Then Throw New Exception("LOID غير صالح.")
        If srID <= 0 Then Throw New Exception("SRID غير صالح.")

        LoadGoodsIssueHeader_FromDB(loID, srID)
        LoadGoodsIssueDetails_FromDB(loID, srID)

    End Sub
    Private Sub LoadGoodsIssueHeader_FromDB(loID As Integer, srID As Integer)

        dgvLO.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    LO.LOID,
    LO.LOCode,
    SR.SRID,
    SR.SRCode,
    ISNULL(E.EmpName, '')     AS DriverName,
    ISNULL(V.VehicleCode, '') AS VehicleCode
FROM dbo.Logistics_LoadingOrder LO
INNER JOIN dbo.Logistics_LoadingOrderSR LOS
    ON LOS.LOID = LO.LOID
INNER JOIN dbo.Business_SR SR
    ON SR.SRID = LOS.SRID
LEFT JOIN dbo.Security_Employee E
    ON E.EmployeeID = LO.DriverEmployeeID
LEFT JOIN dbo.Master_Vehicle V
    ON V.VehicleID = LO.VehicleID
WHERE LO.LOID = @LOID
  AND SR.SRID = @SRID
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)
                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim r As Integer = dgvLO.Rows.Add()

                        dgvLO.Rows(r).Cells("colLOID").Value = CInt(rd("LOID"))
                        dgvLO.Rows(r).Cells("colLOCode").Value = rd("LOCode").ToString()
                        dgvLO.Rows(r).Cells("colDriverName").Value = rd("DriverName").ToString()
                        dgvLO.Rows(r).Cells("colVehicleCode").Value = rd("VehicleCode").ToString()
                        dgvLO.Rows(r).Cells("colSRID").Value = CInt(rd("SRID"))
                        dgvLO.Rows(r).Cells("colSRCode").Value = rd("SRCode").ToString()
                    End If
                End Using
            End Using
        End Using

    End Sub
    Private Sub InitGoodsIssueDetailsTable()

        GoodsIssueDetailsDT = New DataTable()

        GoodsIssueDetailsDT.Columns.Add("LoadingOrderDetailID", GetType(Integer))
        GoodsIssueDetailsDT.Columns.Add("SRDID", GetType(Integer))
        GoodsIssueDetailsDT.Columns.Add("ProductID", GetType(Integer))

        GoodsIssueDetailsDT.Columns.Add("ProductCode", GetType(String))
        GoodsIssueDetailsDT.Columns.Add("RequiredQty", GetType(Decimal))
        GoodsIssueDetailsDT.Columns.Add("LoadedQty", GetType(Decimal))
        GoodsIssueDetailsDT.Columns.Add("UnitName", GetType(String))

    End Sub
    Private Sub SetupGoodsIssueGrid()

        dgvSRD.Columns.Clear()
        dgvSRD.AutoGenerateColumns = False

        ' ProductCode
        Dim colProductCode As New DataGridViewTextBoxColumn()
        colProductCode.Name = "colProductCode"
        colProductCode.HeaderText = "كود الصنف"
        colProductCode.DataPropertyName = "ProductCode"
        dgvSRD.Columns.Add(colProductCode)

        ' RequiredQty
        Dim colRequiredQty As New DataGridViewTextBoxColumn()
        colRequiredQty.Name = "colRequiredQty"
        colRequiredQty.HeaderText = "الكمية المطلوبة"
        colRequiredQty.DataPropertyName = "RequiredQty"
        dgvSRD.Columns.Add(colRequiredQty)

        ' LoadedQty
        Dim colLoadedQty As New DataGridViewTextBoxColumn()
        colLoadedQty.Name = "colLoadedQty"
        colLoadedQty.HeaderText = "الكمية المحملة"
        colLoadedQty.DataPropertyName = "LoadedQty"
        dgvSRD.Columns.Add(colLoadedQty)

        ' UnitName
        Dim colUnitName As New DataGridViewTextBoxColumn()
        colUnitName.Name = "colUnitName"
        colUnitName.HeaderText = "الوحدة"
        colUnitName.DataPropertyName = "UnitName"
        dgvSRD.Columns.Add(colUnitName)

    End Sub

    Private Sub BindGoodsIssueDetailsGrid()

        If GoodsIssueDetailsDT Is Nothing Then
            InitGoodsIssueDetailsTable()
        End If

        dgvSRD.AutoGenerateColumns = False
        dgvSRD.DataSource = GoodsIssueDetailsDT

        dgvSRD.Columns("colLoadingOrderDetailID").DataPropertyName = "LoadingOrderDetailID"
        dgvSRD.Columns("colSRDID").DataPropertyName = "SRDID"
        dgvSRD.Columns("colProductID").DataPropertyName = "ProductID"

        dgvSRD.Columns("colProductCode").DataPropertyName = "ProductCode"
        dgvSRD.Columns("colRequiredQty").DataPropertyName = "RequiredQty"
        dgvSRD.Columns("colLoadedQty").DataPropertyName = "LoadedQty"
        dgvSRD.Columns("colUnitName").DataPropertyName = "UnitName"

    End Sub
    Private Sub LoadGoodsIssueDetails_FromDB(loID As Integer, srID As Integer)

        If GoodsIssueDetailsDT Is Nothing Then
            InitGoodsIssueDetailsTable()
        End If

        If dgvSRD.DataSource Is Nothing Then
            BindGoodsIssueDetailsGrid()
        End If

        GoodsIssueDetailsDT.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    LOD.LoadingOrderDetailID,
    LOD.SourceDetailID AS SRDID,
    LOD.ProductID,
    P.ProductCode,
    ISNULL(SRD.Quantity, 0)  AS RequiredQty,
    ISNULL(LOD.LoadedQty, 0) AS LoadedQty,
    ISNULL(U.UnitName, '')   AS UnitName
FROM dbo.Logistics_LoadingOrderDetail LOD
INNER JOIN dbo.Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
INNER JOIN dbo.Master_Product P
    ON P.ProductID = LOD.ProductID
LEFT JOIN dbo.Master_Unit U
    ON U.UnitID = P.StorageUnitID
WHERE LOD.LOID = @LOID
  AND LOD.SourceHeaderID = @SRID
  AND ISNULL(LOD.LoadedQty, 0) > 0
ORDER BY P.ProductCode
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)
                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim dr As DataRow = GoodsIssueDetailsDT.NewRow()

                        dr("LoadingOrderDetailID") = CInt(rd("LoadingOrderDetailID"))
                        dr("SRDID") = CInt(rd("SRDID"))
                        dr("ProductID") = CInt(rd("ProductID"))
                        dr("ProductCode") = rd("ProductCode").ToString()
                        dr("RequiredQty") = CDec(rd("RequiredQty"))
                        dr("LoadedQty") = CDec(rd("LoadedQty"))
                        dr("UnitName") = rd("UnitName").ToString()

                        GoodsIssueDetailsDT.Rows.Add(dr)
                    End While
                End Using
            End Using
        End Using

    End Sub

    Private Sub btnIssue_Click(sender As Object, e As EventArgs) Handles btnIssue.Click

        Try
            If SourceLOID <= 0 OrElse SourceSRID <= 0 Then
                MessageBox.Show("البيانات غير مكتملة")
                Exit Sub
            End If

            Dim svc As New GoodsIssueService(ConnStr)

            svc.Issue(SourceLOID, SourceSRID, CurrentUser.EmployeeID)

            MessageBox.Show("تم الفسح وترحيل المخزون بنجاح")

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Function GetLoadingTransactionID(loID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT TOP 1 TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @LOID
  AND OperationTypeID = 4
ORDER BY TransactionID DESC
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0

                Return CInt(obj)
            End Using
        End Using

    End Function
End Class