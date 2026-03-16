Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Public Class frmCuttingWasteCalculator
    Inherits AABaseOperationForm

    Private Const UNIT_M3 As Integer = 8
    Private Const UNIT_KG As Integer = 11

    ' Grid column names (from your designer)
    Private Const COL_DetailID As String = "colDetailID"
    Private Const COL_ProductID As String = "colProductID"
    Private Const COL_ProductCode As String = "colProductCode"
    Private Const COL_Consumption As String = "colConsumption"
    Private Const COL_ProductUnit As String = "colProductUnit"
    Private Const COL_WastePercent As String = "colWastePercent"
    Private Const COL_WasteValue As String = "colWasteValue"
    Private Const COL_WasteQty As String = "colWasteQTY"
    Private Const COL_Available As String = "colProductAvailableQty"
    Private Const COL_Density As String = "colProductDensity"
    Private Const COL_TotalWaste As String = "colTotalWaste"
    Private Const COL_Notes As String = "colNotes"
    Private _avgCostCache As Dictionary(Of String, Decimal) = New Dictionary(Of String, Decimal)()
    Private _isLoading As Boolean = False
    Private _isGridRecalc As Boolean = False
    Private _totalWasteVolumeM3 As Decimal = 0D
    Private _totalWasteWeightKG As Decimal = 0D
    Private _totalWasteValue As Decimal = 0D
    Private _totalScrapValue As Decimal = 0D
    Private _wasteAvgCost As Decimal = 0D     ' 6 decimals truth
    Private _newScrapAvgCost As Decimal = 0D  ' 6 decimals truth

    Private _statusID As Integer = 0
    Private _statusCode As String = ""
    Private _statusName As String = ""

    Private _isSaving As Boolean = False
    Private _hasUnsavedChanges As Boolean = False

    Private Const STATUS_NEW As Integer = 2
    Private _wasteID As Integer = 0
    Private _wasteCode As String = ""
    Private _scrapUnitID As Integer = 0
    Private Const STATUS_NEW_ID As Integer = 2
    ' على مستوى الفورم
    Private _isViewOnly As Boolean = False
    ' TODO: replace with your actual session/global user
    Private ReadOnly Property CurrentUserID As Integer
        Get
            Return 1
        End Get
    End Property

    Private ReadOnly Property Service As CuttingWasteApplicationService
        Get
            Return New CuttingWasteApplicationService(ConnStr)
        End Get
    End Property

#Region "Form lifecycle"
    Private Sub MarkDirty()
        If _isLoading Then Exit Sub
        _hasUnsavedChanges = True
    End Sub

    Private Sub MarkClean()
        _hasUnsavedChanges = False
    End Sub
    Private Sub frmCuttingWasteCalculator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _isLoading = True
        dtpPeriodStart.Format = DateTimePickerFormat.Custom
        dtpPeriodStart.CustomFormat = "yyyy-MM-dd HH:mm:ss"

        dtpPeriodEnd.Format = DateTimePickerFormat.Custom
        dtpPeriodEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss"
        ' ✅ السياسة الجديدة: لا حفظ ولا إرسال منفصل ولا إلغاء
        btnSave.Enabled = False
        btnCancel.Enabled = False

        ' زر الإرسال يصبح زر التنفيذ
        btnSend.Text = "تنفيذ"
        btnSend.Enabled = True
        ' PeriodStart always read-only (auto)
        dtpPeriodStart.Enabled = False

        ' If this is a NEW screen, compute start
        Try
            dtpPeriodStart.Enabled = False

            ' إذا كان المخزن محدد مسبقًا
            If cboSourceStore.SelectedValue IsNot Nothing AndAlso CInt(cboSourceStore.SelectedValue) > 0 Then
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        Try
            LoadCombos()
            ResetFormToNew()
        Finally
            _isLoading = False
        End Try
        SetDefaultStores()
        SetAutoPeriodStart()
        btnSave.Text = "حفظ"

    End Sub
    Private Sub txtWastePercent_MouseUp(sender As Object, e As MouseEventArgs) Handles txtWastePercent.MouseUp
        ' لو المستخدم دخل بالماوس، نخلي SelectAll يشتغل وما ينلغي بسبب MouseUp
        If txtWastePercent.SelectionLength = 0 Then
            txtWastePercent.SelectAll()
        End If
    End Sub
    Private Sub cboSourceStore_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboSourceStore.SelectedValueChanged
        If _isLoading Then Exit Sub
        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
        If CInt(cboSourceStore.SelectedValue) <= 0 Then Exit Sub

        SetAutoPeriodStart()
    End Sub

    Private Sub frmCuttingWasteCalculator_Shown(sender As Object, e As EventArgs) Handles Me.Shown
    End Sub
    Private Sub cboWasteType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboWasteType.SelectionChangeCommitted
        If _isLoading Then Exit Sub
        MarkDirty()
    End Sub
    Private Sub cboCalculationType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboCalculationType.SelectionChangeCommitted
        If _isLoading Then Exit Sub
        MarkDirty()
    End Sub
    Private Sub dtpPeriodEnd_ValueChanged(sender As Object, e As EventArgs) Handles dtpPeriodEnd.ValueChanged
        If _isLoading Then Exit Sub
        MarkDirty()
    End Sub

    Private Sub cboSourceStore_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboSourceStore.SelectionChangeCommitted
        SetAutoPeriodStart(silent:=False)
        MarkDirty()
    End Sub
    Private Sub cboTargetStore_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboTargetStore.SelectionChangeCommitted
        If _isLoading Then Exit Sub
        MarkDirty()
    End Sub

    Private Sub SetDefaultStores()
        If _isLoading Then Exit Sub
        If _wasteID <> 0 Then Exit Sub ' لا تغيّر في وضع تعديل مستند

        Const DEFAULT_SOURCE_STORE_ID As Integer = 3
        Const DEFAULT_TARGET_STORE_ID As Integer = 1

        _isLoading = True
        Try
            cboSourceStore.SelectedValue = DEFAULT_SOURCE_STORE_ID
            cboTargetStore.SelectedValue = DEFAULT_TARGET_STORE_ID
        Finally
            _isLoading = False
        End Try
    End Sub
    Private Sub SetAutoPeriodStart(Optional silent As Boolean = False)
        If _isLoading Then Exit Sub
        If cboSourceStore.SelectedValue Is Nothing OrElse CInt(cboSourceStore.SelectedValue) <= 0 Then Exit Sub
        If _wasteID <> 0 Then Exit Sub ' لا تغيّر لمستند محفوظ

        Try
            Dim storeID As Integer = CInt(cboSourceStore.SelectedValue)
            Dim nextStart As DateTime = Service.GetNextPeriodStartForStore(storeID, currentWasteID:=0)

            _isLoading = True
            dtpPeriodStart.Enabled = False
            dtpPeriodStart.Value = nextStart
            _isLoading = False

        Catch ex As Exception
            _isLoading = False
            If Not silent Then
                MessageBox.Show(ex.Message, "لا يمكن إنشاء فترة جديدة", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            ' silent=True => لا شيء (بدون رسالة)
        End Try
    End Sub
#End Region

#Region "Combos"

    Private Sub LoadCombos()
        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' Stores
            ' Stores
            Using cmd As New SqlCommand("SELECT StoreID, StoreName FROM dbo.Master_Store WHERE IsActive=1 ORDER BY StoreName", con)
                Dim dtStores As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtStores)
                End Using

                cboSourceStore.DisplayMember = "StoreName"
                cboSourceStore.ValueMember = "StoreID"
                cboSourceStore.DataSource = dtStores.Copy()

                cboTargetStore.DisplayMember = "StoreName"
                cboTargetStore.ValueMember = "StoreID"
                cboTargetStore.DataSource = dtStores.Copy()
            End Using
            ' Scrap products (CategoryID=4 assumed; adjust if needed)
            Using cmd As New SqlCommand("
SELECT ProductID, ProductCode + N' - ' + ProductName AS DisplayName
FROM dbo.Master_Product
WHERE IsActive=1 AND ProductCategoryID=4
ORDER BY ProductCode
", con)
                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
                cboScrapCode.DisplayMember = "DisplayName"
                cboScrapCode.ValueMember = "ProductID"
                cboScrapCode.DataSource = dt
            End Using

            ' Waste types
            Using cmd As New SqlCommand("SELECT WasteTypeCode, WasteTypeNameAr FROM dbo.System_WasteType WHERE IsActive=1 ORDER BY SortOrder, WasteTypeNameAr", con)
                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
                cboWasteType.DisplayMember = "WasteTypeNameAr"
                cboWasteType.ValueMember = "WasteTypeCode"
                cboWasteType.DataSource = dt
            End Using

            ' Calculation types
            Using cmd As New SqlCommand("SELECT CalculationTypeCode, CalculationTypeNameAr FROM dbo.System_WasteCalculationType WHERE IsActive=1 ORDER BY SortOrder, CalculationTypeNameAr", con)
                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
                cboCalculationType.DisplayMember = "CalculationTypeNameAr"
                cboCalculationType.ValueMember = "CalculationTypeCode"
                cboCalculationType.DataSource = dt
            End Using

        End Using
    End Sub

#End Region

#Region "Buttons"

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        If _isLoading Then Exit Sub
        ResetFormToNew()
        btnSave.Text = "حفظ"

    End Sub

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        If _isLoading Then Exit Sub

        Try
            CalculateAndFillGrid()
            MarkDirty()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If _isLoading Then Exit Sub

        Try
            Dim hdr As New CuttingWasteApplicationService.WasteHeaderDraft With {
    .WasteID = _wasteID,
    .WasteCode = _wasteCode,
    .PeriodStart = dtpPeriodStart.Value,
    .PeriodEnd = dtpPeriodEnd.Value,
    .WasteTypeCode = CStr(cboWasteType.SelectedValue),
    .CalculationTypeCode = CStr(cboCalculationType.SelectedValue),
    .WasteReason = txtWasteReason.Text,
    .WastePercent = If(txtWastePercent.Visible, CType(GetDec(txtWastePercent.Text), Decimal?), Nothing),
    .SourceStoreID = CInt(cboSourceStore.SelectedValue),
    .TargetStoreID = CInt(cboTargetStore.SelectedValue),
    .ScrapProductID = CInt(cboScrapCode.SelectedValue),
    .CurrentScrapStockQty = Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapStock.Text)),
    .CurrentScrapAvgCost = Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapAvgCost.Text)),
    .WasteAvgCost = Round6(_wasteAvgCost),
    .NewScrapAvgCost = Round6(_newScrapAvgCost)
}

            Dim totals = BuildTotalsFromGrid()
            Dim detailsDt = BuildDetailsDataTableFromGrid()

            Dim newID = Service.SaveDraftAndReserve(hdr, totals, detailsDt, CurrentUserID)
            _wasteID = newID
            _wasteCode = hdr.WasteCode

            txtStatusCode.Text = "NEW"
            MessageBox.Show("تم الحفظ والحجز بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)
            MarkClean()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtWasteReason_TextChanged(sender As Object, e As EventArgs) Handles txtWasteReason.TextChanged
        MarkDirty()
    End Sub
    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        If _isLoading Then Exit Sub
        If _isSaving Then Exit Sub

        Try
            _isSaving = True

            ' 1️⃣ حساب القيم النهائية قبل الترحيل
            CalculateAndFillGrid()

            ' 2️⃣ تجهيز بيانات الهالك
            Dim hdr As New CuttingWasteApplicationService.WasteHeaderDraft With {
            .WasteID = 0,
            .WasteCode = "",
            .PeriodStart = dtpPeriodStart.Value,
            .PeriodEnd = dtpPeriodEnd.Value,
            .WasteTypeCode = CStr(cboWasteType.SelectedValue),
            .CalculationTypeCode = CStr(cboCalculationType.SelectedValue),
            .WasteReason = txtWasteReason.Text,
            .WastePercent = If(txtWastePercent.Visible, CType(GetDec(txtWastePercent.Text), Decimal?), Nothing),
            .SourceStoreID = CInt(cboSourceStore.SelectedValue),
            .TargetStoreID = CInt(cboTargetStore.SelectedValue),
            .ScrapProductID = CInt(cboScrapCode.SelectedValue),
            .CurrentScrapStockQty = Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapStock.Text)),
            .CurrentScrapAvgCost = Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapAvgCost.Text)),
            .WasteAvgCost = Round6(_wasteAvgCost),
            .NewScrapAvgCost = Round6(_newScrapAvgCost)
        }

            ' 3️⃣ بناء التفاصيل من الجريد
            Dim totals = BuildTotalsFromGrid()
            Dim detailsDt = BuildDetailsDataTableFromGrid()

            ' 4️⃣ تنفيذ العملية الكاملة
            ' هذه الدالة يجب أن تنشئ Transaction بنوع:
            ' OperationTypeID = 13 (SCRAP)
            ' ثم تستدعي Receive(transactionID)
            Dim result = Service.ExecuteWasteInstant(hdr, totals, detailsDt, CurrentUserID)

            Dim engine As New TransactionEngine(DbConfig.ConnectionString)

            engine.Receive(result.TransactionID, CurrentUserID)
            ' 5️⃣ تحديث الواجهة
            _wasteID = result.WasteID
            _wasteCode = result.WasteCode

            txtStatusCode.Text = "RECEIVED"

            MessageBox.Show(
            "تم تنفيذ وترحيل الهالك بنجاح." & vbCrLf &
            "رقم المستند: " & _wasteCode & vbCrLf &
            "رقم الحركة: " & result.TransactionID,
            "تم",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )

            MarkClean()

            ' بعد الترحيل يصبح المستند للقراءة فقط
            ApplyDocumentMode()
            ConfigureGridRules()

        Catch ex As Exception

            MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally

            _isSaving = False

        End Try

    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If _isLoading Then Exit Sub
        If _wasteID <= 0 Then Exit Sub

        Try
            If _statusID = 2 Then
                If MessageBox.Show("هل أنت متأكد من حذف المستند؟", "تأكيد الحذف",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Exit Sub

                Service.DeleteWasteDraft(_wasteID, CurrentUserID)

                ResetFormToNew()
                ApplyDocumentMode()
                MessageBox.Show("تم حذف المستند.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ElseIf _statusID = 5 OrElse _statusID = 6 Then
                Dim reason = InputBox("سبب الإلغاء:", "إلغاء")
                If String.IsNullOrWhiteSpace(reason) Then
                    MessageBox.Show("يجب إدخال سبب الإلغاء.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Service.CancelWaste(_wasteID, reason, CurrentUserID)

                LoadWasteDocument(_wasteID)
                ApplyDocumentMode()
                MessageBox.Show("تم الإلغاء.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Else
                MessageBox.Show("هذه الحالة لا تسمح بالحذف/الإلغاء.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvCalculations_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCalculations.CellContentClick
        If _isLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If dgvCalculations.Columns(e.ColumnIndex).Name <> "colDelete" Then Exit Sub
        If Not (_statusID = 2 OrElse _wasteID = 0) Then Exit Sub

        Dim row = dgvCalculations.Rows(e.RowIndex)
        If row.IsNewRow Then Exit Sub

        If MessageBox.Show("حذف السطر؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        dgvCalculations.Rows.RemoveAt(e.RowIndex)

        UpdateTotalsToUI()
        RefreshScrapInfoAndNewAvg()
        MarkDirty()
    End Sub
    Private Function ToDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim s = Convert.ToString(v).Trim()
        If s = "" Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(s, d) Then Return d
        Return 0D
    End Function

    Private Sub UpdateDeleteColumnVisibility()
        Dim visible As Boolean = (_wasteID = 0) OrElse (_statusID = 2)
        If dgvCalculations.Columns.Contains("colDelete") Then
            dgvCalculations.Columns("colDelete").Visible = visible
        End If
    End Sub
#End Region

#Region "Grid edit recalculation"
    Private Enum GridMode
        None = 0
        KG = 1
        M3 = 2
    End Enum

    Private Function DetectGridMode() As GridMode
        Dim hasM3 As Boolean = False
        Dim hasKG As Boolean = False

        For Each r As DataGridViewRow In dgvCalculations.Rows
            If r.IsNewRow Then Continue For
            Dim unitID As Integer = CInt(r.Cells(COL_ProductUnit).Value)
            If unitID = UNIT_M3 Then hasM3 = True
            If unitID = UNIT_KG Then hasKG = True
        Next

        If hasM3 Then Return GridMode.M3
        If hasKG Then Return GridMode.KG
        Return GridMode.None
    End Function

    Private Sub dgvCalculations_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCalculations.CellEndEdit
        If _isGridRecalc OrElse _isLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim row = dgvCalculations.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim colName = dgvCalculations.Columns(e.ColumnIndex).Name

        Try
            _isGridRecalc = True

            If colName = COL_WastePercent Then
                RecalcRowFromPercent(row)
            ElseIf colName = COL_WasteQty Then
                RecalcRowFromQty(row)
            End If

            UpdateTotalsToUI()
            RefreshScrapInfoAndNewAvg()
            MarkDirty()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            _isGridRecalc = False
        End Try
    End Sub

#End Region

#Region "Calculate"
    Private _unitsById As Dictionary(Of Integer, String)

    Private Sub EnsureUnitsCacheLoaded()
        If _unitsById IsNot Nothing Then Exit Sub

        _unitsById = New Dictionary(Of Integer, String)()

        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
SELECT UnitID, UnitName
FROM dbo.Master_Unit
", con)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim id As Integer = CInt(rd("UnitID"))
                        Dim name As String = Convert.ToString(rd("UnitName"))
                        If Not _unitsById.ContainsKey(id) Then _unitsById.Add(id, name)
                    End While
                End Using
            End Using
        End Using
    End Sub
    Private Sub dgvCalculations_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvCalculations.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String = dgvCalculations.Columns(e.ColumnIndex).Name

        ' existing unit-name formatting for product unit
        If colName = COL_ProductUnit Then
            If e.Value Is Nothing OrElse IsDBNull(e.Value) Then Exit Sub
            EnsureUnitsCacheLoaded()

            Dim unitId As Integer
            If Integer.TryParse(e.Value.ToString(), unitId) Then
                Dim unitName As String = Nothing
                If _unitsById.TryGetValue(unitId, unitName) Then
                    e.Value = unitName
                    e.FormattingApplied = True
                End If
            End If
            Exit Sub
        End If

        ' percent formatting inside the cell
        If colName = COL_WastePercent Then
            If e.Value Is Nothing OrElse IsDBNull(e.Value) Then Exit Sub

            Dim p As Decimal
            If Decimal.TryParse(e.Value.ToString(), p) Then
                e.Value = p.ToString("0.00") & "%"
                e.FormattingApplied = True
            End If
            Exit Sub
        End If
    End Sub
    Private Sub CalculateAndFillGrid()
        ValidateForCalculate()

        Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
        Dim scrapProductID As Integer = CInt(cboScrapCode.SelectedValue)

        _scrapUnitID = GetProductStorageUnitID(scrapProductID)
        If _scrapUnitID <> UNIT_KG AndAlso _scrapUnitID <> UNIT_M3 Then
            Throw New Exception("هذه النسخة تدعم السكراب بوحدة KG أو M3 فقط.")
        End If
        ConfigureGridRules()
        Dim fromDt As DateTime = dtpPeriodStart.Value
        Dim toDt As DateTime = dtpPeriodEnd.Value

        Dim globalPercent As Decimal = GetDec(txtWastePercent.Text)
        If globalPercent < 0D OrElse globalPercent > 100D Then Throw New Exception("النسبة يجب أن تكون بين 0 و 100.")

        Dim dt As DataTable = LoadConsumptionGrid(sourceStoreID, fromDt, toDt)

        dgvCalculations.Rows.Clear()
        If _avgCostCache IsNot Nothing Then _avgCostCache.Clear()
        _isGridRecalc = True
        Try
            For Each dr As DataRow In dt.Rows
                Dim productID = CInt(dr("ProductID"))
                Dim productCode = Convert.ToString(dr("ProductCode"))
                Dim consumption = CDec(dr("ConsumptionQty"))
                Dim available = CDec(dr("AvailableQty"))
                Dim unitID = CInt(dr("ProductUnitID"))
                Dim unitname = Convert.ToString(dr("ProductUnitName"))
                Dim density = CDec(dr("Density"))

                ' Support rules
                If _scrapUnitID = UNIT_M3 AndAlso unitID <> UNIT_M3 Then
                    Throw New Exception($"الصنف {productCode}: السكراب M3 يتطلب أن تكون وحدة الصنف M3.")
                End If
                If _scrapUnitID = UNIT_KG AndAlso unitID = UNIT_M3 AndAlso density <= 0D Then
                    Throw New Exception($"الصنف {productCode}: لا يمكن التحويل من M3 إلى KG بدون Density.")
                End If
                Dim idx = dgvCalculations.Rows.Add()
                Dim row = dgvCalculations.Rows(idx)
                row.Cells(COL_DetailID).Value = 0
                row.Cells(COL_ProductID).Value = productID
                row.Cells(COL_ProductCode).Value = productCode
                row.Cells(COL_Consumption).Value = consumption
                row.Cells(COL_ProductUnit).Value = unitID
                row.Cells(COL_WastePercent).Value = globalPercent
                row.Cells(COL_Available).Value = available
                row.Cells(COL_Density).Value = density
                row.Cells(COL_Notes).Value = ""

                RecalcRowFromPercent(row)
            Next
        Finally
            _isGridRecalc = False
        End Try

        dtpPeriodStart.Enabled = False
        dtpPeriodEnd.Enabled = False
        UpdateGridColumnHeadersForUnits()
        ConfigureGridRules()
        UpdateTotalsToUI()
        RefreshScrapInfoAndNewAvg()
    End Sub

    Private Function LoadConsumptionGrid(sourceStoreID As Integer, fromDt As DateTime, toDt As DateTime) As DataTable
        Dim dt As New DataTable()

       Const sql As String =
"
WITH Cons AS
(
    SELECT
        d.ProductID,
        SUM(d.Quantity) AS ConsumptionQty
    FROM dbo.Inventory_TransactionHeader h
    JOIN dbo.Inventory_TransactionDetails d
        ON d.TransactionID = h.TransactionID
    WHERE
        h.OperationTypeID = 11
        AND h.StatusID = 6
        AND h.IsInventoryPosted = 1
        AND h.PostingDate >= @From
        AND h.PostingDate <= @To
        AND d.SourceStoreID = @SourceStoreID
    GROUP BY
        d.ProductID
)
SELECT
    c.ProductID,
    p.ProductCode,
    p.ProductName,
    p.StorageUnitID AS ProductUnitID,
    u.UnitName AS ProductUnitName,
    CAST(ISNULL(p.Density,0) AS DECIMAL(18,6)) AS Density,
    CAST(c.ConsumptionQty AS DECIMAL(18,6)) AS ConsumptionQty,
    CAST(ISNULL(b.QtyOnHand,0) AS DECIMAL(18,6)) AS AvailableQty
FROM Cons c
JOIN dbo.Master_Product p ON p.ProductID = c.ProductID
LEFT JOIN dbo.Master_Unit u ON u.UnitID = p.StorageUnitID
LEFT JOIN dbo.Inventory_Balance b ON b.StoreID=@SourceStoreID AND b.ProductID=c.ProductID
WHERE c.ConsumptionQty > 0
ORDER BY p.ProductCode;

"

        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)
                cmd.Parameters.AddWithValue("@From", fromDt)
                cmd.Parameters.AddWithValue("@To", toDt)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Private Sub RefreshScrapInfoAndNewAvg(Optional useSnapshotOnly As Boolean = False)
        If cboScrapCode.SelectedValue Is Nothing OrElse CInt(cboScrapCode.SelectedValue) <= 0 Then Exit Sub
        If _scrapUnitID <> UNIT_KG AndAlso _scrapUnitID <> UNIT_M3 Then Exit Sub

        Dim scrapUnitText As String = If(_scrapUnitID = UNIT_KG, "كجم", "متر مكعب")

        If useSnapshotOnly Then
            ' ✅ لا تعيد قراءة من DB ولا تعيد حساب: اعرض السناب شوت فقط
            SetValueWithUnit(txtCurrentScrapStock, Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapStock.Text)), scrapUnitText, 2)
            SetValueWithUnit(txtCurrentScrapAvgCost, Round6(ParseDecimalFromTextWithUnit(txtCurrentScrapAvgCost.Text)), "ريال", 2)
            SetValueWithUnit(txtScrapAvgCost, Round6(_newScrapAvgCost), "ريال", 2)
            Exit Sub
        End If

        Dim scrapProductID As Integer = CInt(cboScrapCode.SelectedValue)

        ' 1) Current scrap stock (ALL stores)
        Dim currentStock As Decimal = GetScrapStockAllStores(scrapProductID)

        ' 2) Current scrap avg cost (depends on unit)
        Dim currentAvgCost As Decimal = GetScrapCurrentAvgCost(scrapProductID, _scrapUnitID)

        ' 3) Compute new avg cost using numeric cached totals
        Dim inQty As Decimal = If(_scrapUnitID = UNIT_KG, _totalWasteWeightKG, _totalWasteVolumeM3)
        Dim inAvg As Decimal = _wasteAvgCost

        Dim newAvg As Decimal
        If (currentStock + inQty) <= 0D Then
            newAvg = currentAvgCost
        Else
            newAvg = ((currentStock * currentAvgCost) + (inQty * inAvg)) / (currentStock + inQty)
        End If

        SetValueWithUnit(txtCurrentScrapStock, currentStock, scrapUnitText)
        SetValueWithUnit(txtCurrentScrapAvgCost, currentAvgCost, "ريال")
        _newScrapAvgCost = Round6(newAvg)
        SetValueWithUnit(txtScrapAvgCost, _newScrapAvgCost, "ريال", 2)
    End Sub
    Private Function GetScrapStockAllStores(scrapProductID As Integer) As Decimal
        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlClient.SqlCommand("
SELECT CAST(ISNULL(SUM(QtyOnHand),0) AS DECIMAL(18,6))
FROM dbo.Inventory_Balance
WHERE ProductID=@ProductID
", con)
                cmd.Parameters.AddWithValue("@ProductID", scrapProductID)
                Return Convert.ToDecimal(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    Private Function GetScrapCurrentAvgCost(scrapProductID As Integer, scrapUnitID As Integer) As Decimal
        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            If scrapUnitID = UNIT_KG Then
                Using cmd As New SqlClient.SqlCommand("
SELECT CAST(ISNULL(AvgCost,0) AS DECIMAL(18,6))
FROM dbo.Master_Product
WHERE ProductID=@ProductID
", con)
                    cmd.Parameters.AddWithValue("@ProductID", scrapProductID)
                    Return Convert.ToDecimal(cmd.ExecuteScalar())
                End Using

            ElseIf scrapUnitID = UNIT_M3 Then
                ' Need BaseProductID for lookup
                Dim baseID As Integer
                Using cmdBase As New SqlClient.SqlCommand("
SELECT ISNULL(BaseProductID, ProductID)
FROM dbo.Master_Product
WHERE ProductID=@ProductID
", con)
                    cmdBase.Parameters.AddWithValue("@ProductID", scrapProductID)
                    baseID = Convert.ToInt32(cmdBase.ExecuteScalar())
                End Using

                Using cmd As New SqlClient.SqlCommand("
SELECT CAST(ISNULL(AvgCostPerM3,0) AS DECIMAL(18,6))
FROM dbo.Master_FinalProductAvgCost
WHERE BaseProductID=@BaseProductID
", con)
                    cmd.Parameters.AddWithValue("@BaseProductID", baseID)
                    Return Convert.ToDecimal(cmd.ExecuteScalar())
                End Using
            Else
                Return 0D
            End If
        End Using
    End Function


#End Region

#Region "Row recalculation"

    Private Sub RecalcRowFromPercent(row As DataGridViewRow)
        Dim consumption = GetDec(row.Cells(COL_Consumption).Value)
        Dim available = GetDec(row.Cells(COL_Available).Value)
        Dim p = GetDec(row.Cells(COL_WastePercent).Value)

        If p < 0D OrElse p > 100D Then Throw New Exception("النسبة يجب أن تكون بين 0 و 100.")

        Dim wasteQty = Round6(consumption * p / 100D)

        If wasteQty > available Then
            wasteQty = available
            If consumption > 0D Then
                p = Round4(wasteQty / consumption * 100D)
            Else
                p = 0D
            End If
        End If

        row.Cells(COL_WasteQty).Value = wasteQty
        row.Cells(COL_WastePercent).Value = p

        ComputeTotalWaste(row)
    End Sub

    Private Sub RecalcRowFromQty(row As DataGridViewRow)
        Dim consumption = GetDec(row.Cells(COL_Consumption).Value)
        Dim available = GetDec(row.Cells(COL_Available).Value)
        Dim wasteQty = GetDec(row.Cells(COL_WasteQty).Value)

        If wasteQty < 0D Then Throw New Exception("الكمية لا يمكن أن تكون سالبة.")
        If wasteQty > available Then wasteQty = available

        Dim p As Decimal = 0D
        If consumption > 0D Then p = Round4(wasteQty / consumption * 100D)

        row.Cells(COL_WasteQty).Value = wasteQty
        row.Cells(COL_WastePercent).Value = p

        ComputeTotalWaste(row)
    End Sub

    Private Sub ConfigureGridRules()
        dgvCalculations.AllowUserToAddRows = False
        dgvCalculations.AllowUserToDeleteRows = False
        dgvCalculations.MultiSelect = False
        dgvCalculations.SelectionMode = DataGridViewSelectionMode.CellSelect

        Dim editable As Boolean = IsDocEditable()

        For Each c As DataGridViewColumn In dgvCalculations.Columns
            c.ReadOnly = True
        Next

        If Not editable Then Exit Sub

        ' Editable columns in NEW only
        If dgvCalculations.Columns.Contains(COL_WastePercent) Then
            dgvCalculations.Columns(COL_WastePercent).ReadOnly = False
            dgvCalculations.Columns(COL_WastePercent).DefaultCellStyle.Format = "0.00"
        End If

        If dgvCalculations.Columns.Contains(COL_WasteQty) Then
            dgvCalculations.Columns(COL_WasteQty).ReadOnly = False
            dgvCalculations.Columns(COL_WasteQty).DefaultCellStyle.Format = "0.00"
        End If

        If dgvCalculations.Columns.Contains(COL_Notes) Then
            dgvCalculations.Columns(COL_Notes).ReadOnly = False
        End If

        If dgvCalculations.Columns.Contains(COL_TotalWaste) Then
            dgvCalculations.Columns(COL_TotalWaste).DefaultCellStyle.Format = "0.00"
        End If

        If dgvCalculations.Columns.Contains(COL_WasteValue) Then
            dgvCalculations.Columns(COL_WasteValue).DefaultCellStyle.Format = "0.00"
        End If
    End Sub

    ' Requires:
    ' Private Const COL_WasteValue As String = "colWasteValue"

    ' Requires:
    ' Private Const COL_WasteValue As String = "colWasteValue"

    ' Put this field at class level (once):
    ' Private _avgCostCache As New Dictionary(Of String, Decimal)()
    ' Cache key format: "{ProductID}|{UnitID}"
    Private Sub ComputeTotalWaste(row As DataGridViewRow)
        ' - COL_TotalWaste: quantity in scrap unit (KG or M3)  [output qty]
        ' - COL_WasteValue: actual waste value based on product avg cost * WasteQty (product unit)
        '
        ' IMPORTANT:
        ' - COL_ProductUnit is the product's original unit (KG/M3) and is used for calculations.
        ' - COL_WasteUnit is output unit (display only) and must NOT be used for calculations.

        Dim productID As Integer = CInt(row.Cells(COL_ProductID).Value)
        Dim wasteQty As Decimal = GetDec(row.Cells(COL_WasteQty).Value) ' in product unit
        Dim productUnitID As Integer = GetUnitIdFromCellValue(row.Cells(COL_ProductUnit).Value)
        Dim density As Decimal = GetDec(row.Cells(COL_Density).Value)

        If productUnitID <= 0 Then
            Throw New Exception("وحدة المنتج غير معروفة. تحقق من colProductUnit.")
        End If

        ' 1) Output qty (COL_TotalWaste) in scrap unit
        Dim scrapQty As Decimal

        If _scrapUnitID = UNIT_KG Then
            If productUnitID = UNIT_KG Then
                scrapQty = Round6(wasteQty)
            ElseIf productUnitID = UNIT_M3 Then
                If density <= 0D Then Throw New Exception("Density مطلوب للتحويل من M3 إلى KG.")
                scrapQty = Round6(wasteQty * density)
            Else
                Throw New Exception("وحدة المنتج غير مدعومة (فقط KG أو M3).")
            End If

        ElseIf _scrapUnitID = UNIT_M3 Then
            If productUnitID <> UNIT_M3 Then Throw New Exception("السكراب M3 يتطلب أن تكون وحدة المنتج M3 في هذه المرحلة.")
            scrapQty = Round6(wasteQty)

        Else
            Throw New Exception("Scrap unit غير معروف. قم بإجراء الحسابات أولاً.")
        End If

        row.Cells(COL_TotalWaste).Value = scrapQty

        ' 2) Actual waste value = ProductAvgCost * wasteQty (in product unit)
        If dgvCalculations.Columns.Contains(COL_WasteValue) Then
            Dim productAvgCost As Decimal = GetProductAvgCostCached(productID, productUnitID)
            row.Cells(COL_WasteValue).Value = Round6(productAvgCost * wasteQty)
        End If
    End Sub

    Private Function GetUnitIdFromCellValue(v As Object) As Integer
        If v Is Nothing OrElse IsDBNull(v) Then Return 0

        ' Numeric UnitID?
        Dim tmp As Integer
        If Integer.TryParse(v.ToString().Trim(), tmp) Then Return tmp

        ' String cases (UnitCode/Arabic display)
        Dim s As String = v.ToString().Trim().ToUpperInvariant()

        If s = "KG" OrElse s = "KGS" OrElse s = "كجم" OrElse s = "كيلو" OrElse s.Contains("كجم") Then Return UNIT_KG
        If s = "M3" OrElse s = "م3" OrElse s.Contains("متر") OrElse s.Contains("مكعب") Then Return UNIT_M3

        Return 0
    End Function



    Private Function GetProductAvgCostCached(productID As Integer, unitID As Integer) As Decimal
        Dim key As String = productID.ToString() & "|" & unitID.ToString()
        If _avgCostCache IsNot Nothing AndAlso _avgCostCache.ContainsKey(key) Then
            Return _avgCostCache(key)
        End If

        Dim cost As Decimal = 0D

        Using con As New SqlConnection(ConnStr)
            con.Open()

            If unitID = UNIT_KG Then
                Using cmd As New SqlCommand("
SELECT CAST(ISNULL(AvgCost,0) AS DECIMAL(18,6))
FROM dbo.Master_Product
WHERE ProductID=@P
", con)
                    cmd.Parameters.AddWithValue("@P", productID)
                    cost = Convert.ToDecimal(cmd.ExecuteScalar())
                End Using

            ElseIf unitID = UNIT_M3 Then
                Dim baseID As Integer
                Using cmdBase As New SqlCommand("
SELECT ISNULL(BaseProductID, ProductID)
FROM dbo.Master_Product
WHERE ProductID=@P
", con)
                    cmdBase.Parameters.AddWithValue("@P", productID)
                    baseID = Convert.ToInt32(cmdBase.ExecuteScalar())
                End Using

                Using cmd As New SqlCommand("
SELECT CAST(ISNULL(AvgCostPerM3,0) AS DECIMAL(18,6))
FROM dbo.Master_FinalProductAvgCost
WHERE BaseProductID=@B
", con)
                    cmd.Parameters.AddWithValue("@B", baseID)
                    cost = Convert.ToDecimal(cmd.ExecuteScalar())
                End Using

            Else
                cost = 0D
            End If
        End Using

        If _avgCostCache Is Nothing Then _avgCostCache = New Dictionary(Of String, Decimal)()
        _avgCostCache(key) = cost

        Return cost
    End Function

#End Region

#Region "Totals"

    Private Function BuildTotalsFromGrid() As CuttingWasteApplicationService.WasteTotals
        Dim tot As New CuttingWasteApplicationService.WasteTotals()

        ' use cached totals computed in UpdateTotalsToUI
        tot.TotalWasteWeightKG = Round6(_totalWasteWeightKG)
        tot.TotalWasteVolumeM3 = Round6(_totalWasteVolumeM3)
        tot.TotalWasteValue = Round6(_totalWasteValue)
        tot.TotalScrapValue = Round6(_totalScrapValue)

        Return tot
    End Function
    Private Sub UpdateTotalsToUI()
        Dim sumWasteQty As Decimal = 0D        ' SUM(colWasteQTY) (product unit)
        Dim sumTotalWaste As Decimal = 0D      ' SUM(colTotalWaste) => converted/output qty (KG in your current logic)
        Dim sumWasteValue As Decimal = 0D      ' SUM(colWasteValue)

        For Each r As DataGridViewRow In dgvCalculations.Rows
            If r.IsNewRow Then Continue For
            sumWasteQty += GetDec(r.Cells(COL_WasteQty).Value)
            sumTotalWaste += GetDec(r.Cells(COL_TotalWaste).Value)

            If dgvCalculations.Columns.Contains(COL_WasteValue) Then
                sumWasteValue += GetDec(r.Cells(COL_WasteValue).Value)
            End If
        Next

        sumWasteQty = Round6(sumWasteQty)
        sumTotalWaste = Round6(sumTotalWaste)
        sumWasteValue = Round6(sumWasteValue)

        Dim mode As GridMode = DetectGridMode()

        ' Cache numeric truth
        If mode = GridMode.M3 Then
            _totalWasteVolumeM3 = sumWasteQty
            _totalWasteWeightKG = sumTotalWaste
        ElseIf mode = GridMode.KG Then
            _totalWasteVolumeM3 = 0D
            _totalWasteWeightKG = sumTotalWaste
        Else
            _totalWasteVolumeM3 = 0D
            _totalWasteWeightKG = 0D
        End If

        _totalWasteValue = sumWasteValue

        ' Display with units inside the textbox
        If mode = GridMode.M3 Then
            SetValueWithUnit(txtTotalWasteVolume, _totalWasteVolumeM3, "متر مكعب")
        Else
            ' hidden/zero (your SetUnitsMode controls visibility)
            SetValueWithUnit(txtTotalWasteVolume, _totalWasteVolumeM3, "متر مكعب", 2)
        End If

        SetValueWithUnit(txtTotalWasteWeight, _totalWasteWeightKG, "كجم", 2)
        SetValueWithUnit(txtTotalWasteValue, _totalWasteValue, "ريال", 2)

        ' Total scrap output value: output qty depends on scrap unit
        Dim newScrapAvgCost As Decimal = _wasteAvgCost
        Dim scrapQty As Decimal = If(_scrapUnitID = UNIT_KG, _totalWasteWeightKG, _totalWasteVolumeM3)

        _totalScrapValue = Round6(scrapQty * newScrapAvgCost)
        SetValueWithUnit(txtTotalScrapValue, _totalScrapValue, "ريال", 2)

        SetValueWithUnit(txtWasteAvgCost, _wasteAvgCost, "ريال", 2)



        ' Apply visibility rules
    End Sub

#End Region

#Region "Build Details DataTable"

    Private Function BuildDetailsDataTableFromGrid() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("UnitID", GetType(Integer))
        dt.Columns.Add("ConsumptionQty", GetType(Decimal))
        dt.Columns.Add("WastePercent", GetType(Decimal))
        dt.Columns.Add("WasteValue", GetType(Decimal))
        dt.Columns.Add("WasteQty", GetType(Decimal))
        dt.Columns.Add("AvailableQty", GetType(Decimal))
        dt.Columns.Add("Density", GetType(Decimal))
        dt.Columns.Add("Notes", GetType(String))

        For Each r As DataGridViewRow In dgvCalculations.Rows
            If r.IsNewRow Then Continue For

            Dim dr = dt.NewRow()
            dr("ProductID") = CInt(r.Cells(COL_ProductID).Value)
            dr("UnitID") = CInt(r.Cells(COL_ProductUnit).Value)
            dr("ConsumptionQty") = GetDec(r.Cells(COL_Consumption).Value)
            dr("WastePercent") = GetDec(r.Cells(COL_WastePercent).Value)
            dr("WasteValue") = GetDec(r.Cells(COL_WasteValue).Value)
            dr("WasteQty") = GetDec(r.Cells(COL_WasteQty).Value)
            dr("AvailableQty") = GetDec(r.Cells(COL_Available).Value)
            dr("Density") = GetDec(r.Cells(COL_Density).Value)
            dr("Notes") = Convert.ToString(r.Cells(COL_Notes).Value)
            dt.Rows.Add(dr)
        Next

        Return dt
    End Function

#End Region

#Region "Utilities"
    Private Sub ResetFormToNew()
        If _isLoading Then Exit Sub

        _isLoading = True
        Try
            ' reset state
            _wasteID = 0
            _wasteCode = ""
            _scrapUnitID = 0

            _statusID = 0
            _statusCode = "NEW"
            txtStatusCode.Text = "NEW"

            txtWasteReason.Text = ""
            txtWastePercent.Text = "0%"

            dtpPeriodStart.Enabled = False
            dtpPeriodEnd.Enabled = True
            dtpPeriodEnd.Value = DateTime.Now

            dgvCalculations.Rows.Clear()

            txtTotalWasteWeight.Text = "0"
            txtTotalWasteVolume.Text = "0"
            txtTotalWasteValue.Text = "0"
            txtTotalScrapValue.Text = "0"

            UpdateDeleteColumnVisibility()
            ApplyDocumentMode()

            ' defaults
            cboSourceStore.SelectedValue = 3
            cboTargetStore.SelectedValue = 1
        Finally
            _isLoading = False
        End Try

        ' compute start silently (no popup)
        SetAutoPeriodStart(silent:=True)
        MarkClean()
    End Sub

    Private Sub ValidateForCalculate()
        If cboSourceStore.SelectedValue Is Nothing OrElse CInt(cboSourceStore.SelectedValue) <= 0 Then
            Throw New Exception("يرجى اختيار مخزن الصرف.")
        End If

        If cboTargetStore.SelectedValue Is Nothing OrElse CInt(cboTargetStore.SelectedValue) <= 0 Then
            Throw New Exception("يرجى اختيار مخزن الاستلام.")
        End If

        If cboScrapCode.SelectedValue Is Nothing OrElse CInt(cboScrapCode.SelectedValue) <= 0 Then
            Throw New Exception("يرجى اختيار صنف السكراب.")
        End If

        If cboWasteType.SelectedValue Is Nothing OrElse String.IsNullOrWhiteSpace(CStr(cboWasteType.SelectedValue)) Then
            Throw New Exception("يرجى اختيار نوع الهالك.")
        End If

        If cboCalculationType.SelectedValue Is Nothing OrElse String.IsNullOrWhiteSpace(CStr(cboCalculationType.SelectedValue)) Then
            Throw New Exception("يرجى اختيار طريقة الحس��ب.")
        End If

        If dtpPeriodEnd.Value < dtpPeriodStart.Value Then
            Throw New Exception("نهاية الفترة يجب أن تكون أكبر أو تساوي بداية الفترة.")
        End If

        ' Required inputs before calculate
        Dim p As Decimal = GetDec(txtWastePercent.Text)
        If p <= 0D OrElse p > 100D Then
            Throw New Exception("يرجى إدخال نسبة الهالك (0-100).")
        End If

        ' IMPORTANT: txtWasteAvgCost now may contain "ريال"
        Dim cost As Decimal = Round6(ParseDecimalFromTextWithUnit(txtWasteAvgCost.Text))
        If cost <= 0D Then
            Throw New Exception("يرجى إدخال سعر الوحدة للهالك.")
        End If
        _wasteAvgCost = cost

        ' Business rules (current phase)
        Dim calcType As String = CStr(cboCalculationType.SelectedValue).Trim().ToUpperInvariant()
        If calcType = "AUTO" Then
            Throw New Exception("طريقة الحساب MANUAL غير متوفرة حالياً. الرجاء اختيار AUTO .")
        End If

        Dim wasteType As String = CStr(cboWasteType.SelectedValue).Trim().ToUpperInvariant()
        If wasteType = "ABNORMAL" Then
            Throw New Exception("نوع الهالك ABNORMAL يتطلب إضافة الأصناف يدوياً (سيتم تفعيلها في الخطوة القادمة).")
        End If
    End Sub

    Private Function GetProductStorageUnitID(productID As Integer) As Integer
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("SELECT StorageUnitID FROM dbo.Master_Product WHERE ProductID=@P", con)
                cmd.Parameters.AddWithValue("@P", productID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Throw New Exception("لم يتم العثور على وحدة الصنف.")
                Return CInt(obj)
            End Using
        End Using
    End Function

    Private Shared Function GetDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D

        Dim s As String = v.ToString().Trim()

        ' remove percent if present
        If s.EndsWith("%") Then
            s = s.Substring(0, s.Length - 1).Trim()
        End If

        ' try direct parse first
        Dim d As Decimal
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, d) Then
            Return d
        End If

        ' fallback: parse numeric token from text with units (e.g., "12.56 متر مكعب")
        Return ParseDecimalFromTextWithUnit(s)
    End Function

    Private Shared Function Round6(x As Decimal) As Decimal
        Return Math.Round(x, 6, MidpointRounding.AwayFromZero)
    End Function

    Private Shared Function Round4(x As Decimal) As Decimal
        Return Math.Round(x, 4, MidpointRounding.AwayFromZero)
    End Function

#End Region
    Private Sub txtWastePercent_Enter(sender As Object, e As EventArgs) Handles txtWastePercent.Enter


        Dim t As String = If(txtWastePercent.Text, "").Trim()
        If t.EndsWith("%") Then
            t = t.Substring(0, t.Length - 1).Trim()
            txtWastePercent.Text = t
            txtWastePercent.SelectAll()
        End If
    End Sub

    Private Sub txtWastePercent_Leave(sender As Object, e As EventArgs) Handles txtWastePercent.Leave
        ' Add % after editing (keep number only in internal parsing via GetDec)
        Dim v As Decimal = 0D
        Dim t As String = If(txtWastePercent.Text, "").Trim().Replace("%", "").Trim()

        If t = "" Then
            txtWastePercent.Text = ""
            Exit Sub
        End If

        If Not Decimal.TryParse(t, v) Then
            MessageBox.Show("يرجى إدخال نسبة صحيحة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtWastePercent.Focus()
            Exit Sub
        End If

        If v < 0D OrElse v > 100D Then
            MessageBox.Show("النسبة يجب أن تكون بين 0 و 100.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtWastePercent.Focus()
            Exit Sub
        End If

        txtWastePercent.Text = v.ToString("0.####") & "%"
        MarkDirty()
    End Sub

    Private Sub txtWastePercent_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtWastePercent.KeyPress
        ' Allow digits, backspace, and decimal separator only
        If Char.IsControl(e.KeyChar) Then Return

        Dim decSep As Char = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator(0)

        If Char.IsDigit(e.KeyChar) Then Return
        If e.KeyChar = decSep Then
            ' Only one decimal separator
            If txtWastePercent.Text.Contains(decSep) Then e.Handled = True
            Return
        End If

        ' Block everything else (including %)
        e.Handled = True
    End Sub
    Private Sub cboScrapCode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboScrapCode.SelectedIndexChanged
        If _isLoading Then Exit Sub
        If cboScrapCode.SelectedValue Is Nothing OrElse CInt(cboScrapCode.SelectedValue) <= 0 Then Exit Sub

        Try
            _scrapUnitID = GetProductStorageUnitID(CInt(cboScrapCode.SelectedValue))
            UpdateTotalsToUI()
            RefreshScrapInfoAndNewAvg()
            MarkDirty()
        Catch
            ' ignore during binding
        End Try
    End Sub
    Private Sub UpdateGridColumnHeadersForUnits()
        ' Column titles only; no need for a separate WasteUnit column.

        Dim mode As GridMode = DetectGridMode()

        Dim qtyUnitText As String = ""
        If mode = GridMode.M3 Then
            qtyUnitText = "م³"
        ElseIf mode = GridMode.KG Then
            qtyUnitText = "كجم"
        Else
            ' before calculate
            qtyUnitText = ""
        End If

        If dgvCalculations.Columns.Contains(COL_WasteQty) Then
            dgvCalculations.Columns(COL_WasteQty).HeaderText = If(qtyUnitText = "", "الهالك", $"الهالك ({qtyUnitText})")
        End If

        If dgvCalculations.Columns.Contains(COL_Available) Then
            dgvCalculations.Columns(COL_Available).HeaderText = If(qtyUnitText = "", "المتاح", $"المتاح ({qtyUnitText})")
        End If

        If dgvCalculations.Columns.Contains(COL_Consumption) Then
            dgvCalculations.Columns(COL_Consumption).HeaderText = If(qtyUnitText = "", "الاستهلاك", $"الاستهلاك ({qtyUnitText})")
        End If

        If dgvCalculations.Columns.Contains(COL_Density) Then
            dgvCalculations.Columns(COL_Density).HeaderText = "الكثافة (كجم/م³)"
        End If

        If dgvCalculations.Columns.Contains(COL_TotalWaste) Then
            dgvCalculations.Columns(COL_TotalWaste).HeaderText = "الهالك المحول (كجم)"
        End If

        If dgvCalculations.Columns.Contains(COL_WasteValue) Then
            dgvCalculations.Columns(COL_WasteValue).HeaderText = "قيمة الهالك (ريال)"
        End If
    End Sub

    Private Sub SetValueWithUnit(tb As TextBox, value As Decimal, unitText As String, Optional decimalsToShow As Integer = 2)
        Dim fmt As String = "0." & New String("0"c, Math.Max(0, decimalsToShow))
        Dim num As String = Math.Round(value, 6).ToString(fmt)
        tb.Text = If(String.IsNullOrWhiteSpace(unitText), num, num & " " & unitText.Trim())
    End Sub
    Private Sub dgvCalculations_CellParsing(sender As Object, e As DataGridViewCellParsingEventArgs) Handles dgvCalculations.CellParsing
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String = dgvCalculations.Columns(e.ColumnIndex).Name
        If colName <> COL_WastePercent Then Exit Sub

        If e.Value Is Nothing Then Exit Sub

        Dim s As String = e.Value.ToString().Trim()
        s = s.Replace("%", "").Trim()

        Dim p As Decimal
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, p) Then
            e.Value = p
            e.ParsingApplied = True
        End If
    End Sub

    Private Shared Function ParseDecimalFromTextWithUnit(s As String) As Decimal
        If String.IsNullOrWhiteSpace(s) Then Return 0D
        s = s.Trim()

        ' Extract the first numeric token (supports "12.5 كجم", "12,5", "-3.2%", etc.)
        Dim sb As New System.Text.StringBuilder()
        Dim started As Boolean = False

        For Each ch As Char In s
            If Char.IsDigit(ch) OrElse ch = "."c OrElse ch = ","c OrElse ch = "-"c Then
                sb.Append(ch)
                started = True
            ElseIf started Then
                Exit For
            End If
        Next

        Dim token As String = sb.ToString().Trim()
        If token.Length = 0 Then Return 0D

        Dim value As Decimal

        ' Try current culture
        If Decimal.TryParse(token, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, value) Then
            Return value
        End If

        ' Fallback: invariant culture normalization
        token = token.Replace(",", ".")
        If Decimal.TryParse(token, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, value) Then
            Return value
        End If

        Return 0D
    End Function
    Private Sub txtWasteAvgCost_Enter(sender As Object, e As EventArgs) Handles txtWasteAvgCost.Enter
        ' If it contains unit text like "6.00 ريال", show number only for editing
        Dim v As Decimal = ParseDecimalFromTextWithUnit(txtWasteAvgCost.Text)

        ' Only overwrite text if there is a meaningful number OR the textbox is not empty
        If Not String.IsNullOrWhiteSpace(txtWasteAvgCost.Text) Then
            txtWasteAvgCost.Text = Math.Round(v, 2).ToString("0.00")
        End If

        ' Select all after focus is fully applied (more reliable)
        Me.BeginInvoke(New Action(Sub() txtWasteAvgCost.SelectAll()))
    End Sub
    Private Sub txtWasteAvgCost_Leave(sender As Object, e As EventArgs) Handles txtWasteAvgCost.Leave
        _wasteAvgCost = Round6(ParseDecimalFromTextWithUnit(txtWasteAvgCost.Text))
        SetValueWithUnit(txtWasteAvgCost, _wasteAvgCost, "ريال", 2)

        UpdateTotalsToUI()
        RefreshScrapInfoAndNewAvg()
    End Sub
    Private Function IsDocEditable() As Boolean
        Return (_wasteID = 0) OrElse (_statusCode.Trim().ToUpperInvariant() = "NEW")
    End Function

    Private Sub ApplyDocumentMode()
        ' ... أي منطق عندك ...

        ' Dynamic Cancel/Delete button + grid delete column
        If _wasteID <= 0 Then
            btnCancel.Enabled = False
            btnCancel.Text = "حذف/إلغاء"
            dgvCalculations.Columns("colDelete").Visible = True
            Exit Sub
        End If

        If _statusID = 2 Then
            btnCancel.Enabled = True
            btnCancel.Text = "حذف"
            dgvCalculations.Columns("colDelete").Visible = True
        ElseIf _statusID = 5 OrElse _statusID = 6 Then
            btnCancel.Enabled = True
            btnCancel.Text = "إلغاء"
            dgvCalculations.Columns("colDelete").Visible = False
        Else
            btnCancel.Enabled = False
            btnCancel.Text = "حذف/إلغاء"
            dgvCalculations.Columns("colDelete").Visible = False
        End If
        UpdateDeleteColumnVisibility()
    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        If _isLoading Then Exit Sub

        Using f As New frmCuttingWasteCalculatorSearch()
            If f.ShowDialog(Me) <> DialogResult.OK Then Exit Sub
            If f.SelectedWasteID <= 0 Then Exit Sub

            Try
                LoadWasteDocument(f.SelectedWasteID)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub
    Private Sub LoadWasteDocument(wasteID As Integer)
        _isLoading = True
        Try
            Using con As New SqlConnection(ConnStr)
                con.Open()

                ' 1) Load header
                Dim hdr As New DataTable()
                Using cmd As New SqlCommand("
SELECT
    WasteID,
    WasteCode,
    PeriodStartDate,
    PeriodEndDate,
    WasteTypeCode,
    CalculationTypeCode,
    WasteReason, 
    WastePercent,
    SourceStoreID, 
    TargetStoreID, 
    ScrapProductID,
    CurrentScrapStockQty,
    CurrentScrapAvgCost,
    WasteAvgCost,
    NewScrapAvgCost,
    TotalWasteVolume_m3, 
    TotalWasteWeight_kg, 
    TotalWasteValue,
    TotalScrapValue,
    h.StatusID,
    ws.StatusCode,
    ws.StatusName
FROM dbo.Inventory_WasteHeader h
LEFT JOIN dbo.Workflow_Status ws ON ws.StatusID = h.StatusID
WHERE h.WasteID=@WasteID
", con)
                    cmd.Parameters.AddWithValue("@WasteID", wasteID)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(hdr)
                    End Using
                End Using

                If hdr.Rows.Count = 0 Then Throw New Exception("لم يتم العثور على المستند.")

                Dim r = hdr.Rows(0)

                _wasteID = CInt(r("WasteID"))
                _wasteCode = Convert.ToString(r("WasteCode"))

                ' === Status fields (DB truth) ===
                _statusID = CInt(r("StatusID"))
                _statusCode = Convert.ToString(r("StatusCode"))

                ' للعرض فقط (عربي)
                txtStatusCode.Text = Convert.ToString(r("StatusName"))

                ' ... ثم أكمل تعبئة باقي الحقول ...                _wasteID = CInt(r("WasteID"))
                _wasteCode = Convert.ToString(r("WasteCode"))
                txtStatusCode.Text = Convert.ToString(r("StatusID"))

                dtpPeriodStart.Value = CDate(r("PeriodStartDate"))
                dtpPeriodEnd.Value = CDate(r("PeriodEndDate"))
                dtpPeriodStart.Enabled = False
                dtpPeriodEnd.Enabled = False

                cboSourceStore.SelectedValue = CInt(r("SourceStoreID"))
                cboTargetStore.SelectedValue = CInt(r("TargetStoreID"))
                cboScrapCode.SelectedValue = CInt(r("ScrapProductID"))

                cboWasteType.SelectedValue = Convert.ToString(r("WasteTypeCode"))
                cboCalculationType.SelectedValue = Convert.ToString(r("CalculationTypeCode"))
                txtWasteReason.Text = Convert.ToString(r("WasteReason"))

                Dim statusId As Integer = CInt(r("StatusID"))
                txtStatusCode.Text = StatusTextFromId(statusId)

                ' percent textbox (you are already formatting it with % in Leave; here set numeric only then let user see it)
                Dim wp As Decimal = If(IsDBNull(r("WastePercent")), 0D, CDec(r("WastePercent")))
                txtWastePercent.Text = wp.ToString("0.00") & "%"

                ' load cached scrap unit for correct display
                _scrapUnitID = GetProductStorageUnitID(CInt(r("ScrapProductID")))

                ' Costs snapshots
                _wasteAvgCost = If(IsDBNull(r("WasteAvgCost")), 0D, CDec(r("WasteAvgCost")))
                _newScrapAvgCost = If(IsDBNull(r("NewScrapAvgCost")), 0D, CDec(r("NewScrapAvgCost")))

                SetValueWithUnit(txtWasteAvgCost, _wasteAvgCost, "ريال", 2)
                SetValueWithUnit(txtScrapAvgCost, _newScrapAvgCost, "ريال", 2)

                Dim currentStock As Decimal = If(IsDBNull(r("CurrentScrapStockQty")), 0D, CDec(r("CurrentScrapStockQty")))
                Dim currentAvg As Decimal = If(IsDBNull(r("CurrentScrapAvgCost")), 0D, CDec(r("CurrentScrapAvgCost")))
                Dim scrapUnitText As String = If(_scrapUnitID = UNIT_KG, "كجم", "متر مكعب")
                SetValueWithUnit(txtCurrentScrapStock, currentStock, scrapUnitText, 2)
                SetValueWithUnit(txtCurrentScrapAvgCost, currentAvg, "ريال", 2)

                ' Totals snapshots
                _totalWasteVolumeM3 = If(IsDBNull(r("TotalWasteVolume_m3")), 0D, CDec(r("TotalWasteVolume_m3")))
                _totalWasteWeightKG = If(IsDBNull(r("TotalWasteWeight_kg")), 0D, CDec(r("TotalWasteWeight_kg")))
                _totalWasteValue = If(IsDBNull(r("TotalWasteValue")), 0D, CDec(r("TotalWasteValue")))
                _totalScrapValue = If(IsDBNull(r("TotalScrapValue")), 0D, CDec(r("TotalScrapValue")))

                SetValueWithUnit(txtTotalWasteVolume, _totalWasteVolumeM3, "متر مكعب", 2)
                SetValueWithUnit(txtTotalWasteWeight, _totalWasteWeightKG, "كجم", 2)
                SetValueWithUnit(txtTotalWasteValue, _totalWasteValue, "ريال", 2)
                SetValueWithUnit(txtTotalScrapValue, _totalScrapValue, "ريال", 2)

                ' 2) Load details
                Dim dtDet As New DataTable()
                Using cmd As New SqlCommand("
SELECT
    d.WasteDetailID,
    d.ProductID,
    p.ProductCode,
    d.UnitID,
    d.ConsumptionQty,
    d.WastePercent,
    d.WasteQty,
    d.AvailableQty,
    d.Density,
    d.Notes
FROM dbo.Inventory_WasteDetails d
LEFT JOIN dbo.Master_Product p ON p.ProductID = d.ProductID
WHERE d.WasteID=@WasteID
ORDER BY d.WasteDetailID
", con)
                    cmd.Parameters.AddWithValue("@WasteID", wasteID)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dtDet)
                    End Using
                End Using

                dgvCalculations.Rows.Clear()
                _avgCostCache.Clear()

                _isGridRecalc = True
                Try
                    For Each dr As DataRow In dtDet.Rows
                        Dim idx = dgvCalculations.Rows.Add()
                        Dim rowGrid = dgvCalculations.Rows(idx)

                        rowGrid.Cells(COL_DetailID).Value = CInt(dr("WasteDetailID"))
                        rowGrid.Cells(COL_ProductID).Value = CInt(dr("ProductID"))

                        ' ProductCode is not stored in details; optional to load via join if needed
                        rowGrid.Cells(COL_ProductCode).Value = ""

                        rowGrid.Cells(COL_ProductUnit).Value = CInt(dr("UnitID"))
                        rowGrid.Cells(COL_Consumption).Value = CDec(dr("ConsumptionQty"))
                        rowGrid.Cells(COL_WastePercent).Value = CDec(dr("WastePercent"))
                        rowGrid.Cells(COL_WasteQty).Value = CDec(dr("WasteQty"))
                        rowGrid.Cells(COL_Available).Value = CDec(dr("AvailableQty"))
                        rowGrid.Cells(COL_Density).Value = CDec(dr("Density"))
                        rowGrid.Cells(COL_Notes).Value = Convert.ToString(dr("Notes"))
                        rowGrid.Cells(COL_ProductID).Value = CInt(dr("ProductID"))
                        rowGrid.Cells(COL_ProductCode).Value = Convert.ToString(dr("ProductCode"))
                        ' recompute computed columns in UI (TotalWaste/WasteValue)
                        ComputeTotalWaste(rowGrid)
                    Next
                Finally
                    _isGridRecalc = False
                End Try

                UpdateGridColumnHeadersForUnits()
                ConfigureGridRules()

                ' Recalculate totals from grid to ensure UI sync (optional)
                UpdateTotalsToUI()
                RefreshScrapInfoAndNewAvg(useSnapshotOnly:=True)
                ApplyDocumentMode()
                UpdateDeleteColumnVisibility()
                MarkClean()
            End Using
            btnSave.Text = "تعديل"
            _isViewOnly = True
            ApplyViewOnlyMode()
        Finally
            _isLoading = False
        End Try
    End Sub

    Private Function StatusTextFromId(id As Integer) As String
        Select Case id
            Case 2 : Return "NEW"
            Case 5 : Return "SENT"
            Case 6 : Return "RECEIVED"
            Case 10 : Return "CANCELLED"
            Case Else : Return id.ToString()
        End Select
    End Function
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        TryCloseForm()
    End Sub
    Private Sub TryCloseForm()
        ' 1) لا تغلق أثناء عمليات حساسة
        If _isLoading OrElse _isSaving Then
            MessageBox.Show("يرجى الانتظار حتى تنتهي العملية الحالية.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 2) تحذير من فقدان التعديلات
        If _hasUnsavedChanges Then
            Dim msg =
    "يوجد تغييرات غير محفوظة.
هل تريد الإغلاق بدون حفظ؟"

            Dim r = MessageBox.Show(msg, "تأكيد الإغلاق", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
            If r <> DialogResult.Yes Then Exit Sub
        End If

        ' 3) إغلاق نظيف
        Me.Close()
    End Sub
    Private Sub frmCuttingWasteCalculator_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.UserClosing Then Exit Sub

        If _isLoading OrElse _isSaving Then
            MessageBox.Show("يرجى الانتظار حتى تنتهي العملية الحالية.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
            Exit Sub
        End If

        If _hasUnsavedChanges Then
            Dim r = MessageBox.Show("يوجد تغييرات غير محفوظة. هل تريد الإغلاق بدون حفظ؟",
                                    "تأكيد الإغلاق", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
            If r <> DialogResult.Yes Then
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub
    Private Sub ApplyViewOnlyMode()
        ' ==== Header controls ====
        dtpPeriodStart.Enabled = False
        dtpPeriodEnd.Enabled = False

        cboSourceStore.Enabled = False
        cboTargetStore.Enabled = False
        cboScrapCode.Enabled = False
        cboWasteType.Enabled = False
        cboCalculationType.Enabled = False

        txtWasteReason.ReadOnly = True
        txtWastePercent.ReadOnly = True

        txtCurrentScrapStock.ReadOnly = True
        txtCurrentScrapAvgCost.ReadOnly = True
        txtWasteAvgCost.ReadOnly = True
        txtScrapAvgCost.ReadOnly = True

        txtTotalWasteVolume.ReadOnly = True
        txtTotalWasteWeight.ReadOnly = True
        txtTotalWasteValue.ReadOnly = True
        txtTotalScrapValue.ReadOnly = True

        ' لو txtStatusCode عندك TextBox
        txtStatusCode.ReadOnly = True

        ' ==== Buttons (حسب رغبتك) ====
        btnSave.Enabled = False
        btnSend.Enabled = False
        btnCalculate.Enabled = False ' إذا عندك زر حساب/تحديث

        ' ==== Grid بالكامل ReadOnly ====
        dgvCalculations.ReadOnly = True
        dgvCalculations.AllowUserToAddRows = False
        dgvCalculations.AllowUserToDeleteRows = False
        dgvCalculations.EditMode = DataGridViewEditMode.EditProgrammatically

        ' لو عندك عمود حذف (زر/Checkbox) أخفه
        UpdateDeleteColumnVisibility() ' عندك أصلاً، خليها تخفيه لما ViewOnly
    End Sub
End Class