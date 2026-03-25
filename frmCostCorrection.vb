Imports System.Data.SqlClient


Public Class frmCostCorrection
    Inherits AABaseOperationForm
    Private IsInventoryPosted As Boolean = False
    Private IsCancelMode As Boolean = False
    Private CurrentOperationType As String = "PUR"
    Private IsLoading As Boolean = False
    Private _revaluationService As CostCorrectionService
    Private _lastSimulationLedger As DataTable
    Private _lastSimulationLinks As DataTable
    Private _originalProductionQty As Decimal = 0D
    Private _originalProductionBaseValue As Decimal = 0D
    Private _originalProductionUnitCost As Decimal = 0D
    Private CurrentSubCategoryID As Integer = 0
    Dim TotalChemicalQty As Decimal = 0D
    Dim totalChemicalCost As Decimal = 0D
    Private _originalProductionVolume As Decimal = 0D
    Private _currentCutBaseProductID As Integer = 0
    Private _editedRowsSnapshots As New Dictionary(Of Integer, Dictionary(Of String, Object))
    Private ReadOnly _allowedEditProps As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "NewQty",
        "NewUnitPrice"
    }

    Private ReadOnly _allowedEditColumnNames As HashSet(Of String) =
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "colQty",        ' عمود الكمية في designer
            "colUnitPrice"   ' عمود السعر في designer
        }


    'General FORM

    Private Class SimulationCorrection
        Public Property LedgerID As Long
        Public Property ProductID As Integer
        Public Property StoreID As Integer?
        Public Property NewQty As Decimal
        Public Property NewUnitCost As Decimal
    End Class
    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "COR"
        End Get
    End Property
    Private Function ToInt(val As Object) As Integer
        If val Is Nothing OrElse IsDBNull(val) Then
            Return 0
        End If

        Dim result As Integer
        If Integer.TryParse(val.ToString(), result) Then
            Return result
        End If

        Return 0
    End Function
    Private Sub dgvMain_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
        e.ThrowException = False
    End Sub
    Protected Sub LoadVATRateCombo()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                TaxTypeID,
                TaxName,
                TaxRate
            FROM Master_TaxType
            WHERE IsActive = 1
            ORDER BY TaxTypeID
        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using
        End Using

        cboVATRate.DataSource = Nothing
        cboVATRate.Items.Clear()

        cboVATRate.DataSource = dt
        cboVATRate.DisplayMember = "TaxName"
        cboVATRate.ValueMember = "TaxTypeID"

        ' ✅ الافتراضي ID=1 (بدون SelectedIndex=-1)
        If dt.Rows.Count > 0 Then
            cboVATRate.SelectedValue = 1
        End If

    End Sub
    Private Sub cboPartnerCode_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboPartnerCode.SelectedIndexChanged

        If IsLoading Then Exit Sub

        Dim drv As DataRowView =
        TryCast(cboPartnerCode.SelectedItem, DataRowView)

        If drv Is Nothing Then
            ClearPartnerFields()
            Exit Sub
        End If

        OnPartnerChanged(drv)

    End Sub
    Private Sub ClearPartnerFields()
        txtPartnerName.Text = ""
    End Sub
    Protected Sub OnPartnerChanged(drv As DataRowView)

        ' اسم / كود الشريك
        txtPartnerName.Text = drv("PartnerName").ToString()

        ' الرقم الضريبي
        drv("VATRegistrationNumber").ToString()

        ' الهاتف / العنوان / المدينة
        Dim phone As String, address As String, city As String
        GetPartnerInfo(drv, phone, address, city)

        txtPhone.Text = phone

    End Sub
    Protected Sub GetPartnerInfo(
drv As DataRowView,
ByRef phone As String,
ByRef address As String,
ByRef city As String
)

        If drv Is Nothing Then
            phone = ""
            address = ""
            city = ""
            Exit Sub
        End If

        phone = drv("Phone").ToString()
        address = drv("Address").ToString()
        city = drv("City").ToString()

    End Sub
    Protected Sub LoadPaymentMethodCombo()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
        "SELECT PaymentMethodID, NameAr FROM Master_PaymentMethod WHERE IsActive = 1", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

    End Sub
    Protected Sub LoadPaymentTermCombo()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "
            SELECT 
                PaymentTermID,
                NameAr,
                DueDays
            FROM Master_PaymentTerm
            WHERE IsActive = 1
            ORDER BY PaymentTermID
            ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        cboPaymentTerm.DataSource = dt
        cboPaymentTerm.DisplayMember = "NameAr"
        cboPaymentTerm.ValueMember = "PaymentTermID"
        cboPaymentTerm.SelectedIndex = -1

    End Sub
    Protected Sub LoadTargetStoreCombo()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
        SELECT
            StoreID,
            StoreName
        FROM Master_Store
        WHERE IsActive = 1
        ORDER BY StoreName
        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        cboTargetStore.DataSource = dt
        cboTargetStore.DisplayMember = "StoreName"
        cboTargetStore.ValueMember = "StoreID"
        cboTargetStore.SelectedIndex = -1

    End Sub
    Protected Sub LoadPartnerComboBox()
        Try
            cboPartnerCode.DataSource = Nothing
            cboPartnerCode.Items.Clear()

            Dim dt As New DataTable()

            Using con As New SqlConnection(ConnStr)
                Dim query As String =
"
SELECT 
   p.PartnerID,
   p.PartnerCode,
   p.PartnerName,
   p.VATRegistrationNumber,
   p.Phone,
   p.Address,
   a.City
FROM Master_Partner p
LEFT JOIN Master_PartnerAddress a 
    ON a.PartnerID = p.PartnerID
   AND a.IsDefault = 1
WHERE p.IsActive = 1
  AND p.PartnerCode LIKE '%CUS%'
ORDER BY p.PartnerName
"
                Using cmd As New SqlCommand(query, con)
                    con.Open()
                    dt.Load(cmd.ExecuteReader())
                End Using
            End Using

            ' 🔴 الترتيب الصحيح (مهم)
            cboPartnerCode.DisplayMember = "PartnerCode"
            cboPartnerCode.ValueMember = "PartnerID"
            cboPartnerCode.DataSource = dt
            cboPartnerCode.SelectedIndex = -1

            cboPartnerCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            cboPartnerCode.AutoCompleteSource = AutoCompleteSource.ListItems

        Catch ex As Exception
            MessageBox.Show("خطأ في تحميل قائمة الموردين: " & ex.Message)

        End Try
    End Sub
    Private Sub rbtnEditMode_CheckedChanged(sender As Object, e As EventArgs) Handles rbtnEditMode.CheckedChanged
        If rbtnEditMode.Checked Then
            IsCancelMode = False

            If CurrentDocumentID > 0 Then

                Select Case CurrentOperationType
                    Case "CUT"
                        LoadCuttingForRevaluation(CurrentDocumentID)
                    Case "PRO"
                        LoadProductionForRevaluation(CurrentDocumentID)
                    Case "PUR", "SRT"
                        LoadOriginalDocumentForRevaluation(CurrentDocumentID)


                    Case Else
                        LoadOriginalDocumentForRevaluation(CurrentDocumentID)
                End Select

            End If

            SetModeUI()

            ' ✅ إعادة الحساب
            RecalculateCuttingTotals()
            RefreshAdjustmentDeltaView()

        End If
    End Sub
    Private Sub rbtnCancelMode_CheckedChanged(sender As Object, e As EventArgs) Handles rbtnCancelMode.CheckedChanged

        If Not rbtnCancelMode.Checked Then Return

        IsCancelMode = True

        '========================================
        ' 1) إعادة تحميل البيانات حسب العملية
        '========================================
        If CurrentDocumentID > 0 Then

            Select Case CurrentOperationType

                Case "CUT"
                    LoadCuttingForRevaluation(CurrentDocumentID)

                Case "PRO"
                    LoadProductionForRevaluation(CurrentDocumentID)

                Case "PUR", "SRT"
                    LoadOriginalDocumentForRevaluation(CurrentDocumentID)

                Case Else
                    LoadOriginalDocumentForRevaluation(CurrentDocumentID)

            End Select

        End If

        '========================================
        ' 2) تصفير القيم حسب نوع العملية
        '========================================
        Select Case CurrentOperationType

            Case "PRO"
                ResetProductionValuesForCancel()

            Case "CUT"
                ResetCuttingValuesForCancel()

            Case "PUR", "SRT"
                ' 🔥 سننشئها لاحقًا
                ResetPurchaseValuesForCancel()

        End Select

        '========================================
        ' 3) خصائص خاصة بالإنتاج
        '========================================
        If CurrentOperationType = "PRO" Then
            SetProductionReadOnly()
        End If

        '========================================
        ' 4) تحديث الواجهة
        '========================================
        SetModeUI()

        '========================================
        ' 5) إعادة الحساب
        '========================================
        RecalculateCuttingTotals()
        RefreshAdjustmentDeltaView()

    End Sub
    Private Sub frmRevaluation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IsLoading Then Return
        IsLoading = True

        Try
            rbtnEditMode.Checked = True
            IsCancelMode = False
            _revaluationService = New CostCorrectionService(ConnStr)
            dgvMain.AutoGenerateColumns = False
            dgvMain.EditMode = DataGridViewEditMode.EditOnEnter

            dgvMain.DataSource = Nothing

            LoadPartnerComboBox()
            LoadPaymentMethodCombo()
            LoadPaymentTermCombo()
            LoadTargetStoreCombo()
            LoadVATRateCombo()

            InitializeAdjustmentDeltaGrid()
            InitializeAffectedOperationsGrid()
            InitializeSimulationGrid()
            InitializeCuttingGrid()

            If cboVATRate.DataSource IsNot Nothing Then
                cboVATRate.SelectedValue = 1
            End If

            colProductCode.DataPropertyName = "ProductCode"
            colProductID.DataPropertyName = "ProductID"
            colProductName.DataPropertyName = "ProductName"
            colProductType.DataPropertyName = "ProductTypeID"

            colUnitID.DataPropertyName = "UnitID"
            colQty.DataPropertyName = "NewQty"
            colUnitPrice.DataPropertyName = "NewUnitPrice"
            colVATRate.DataPropertyName = "TaxRate"
            colVATAmount.DataPropertyName = "TaxAmount"
            colTaxableAmount.DataPropertyName = "TaxableAmount"
            colTotalAmount.DataPropertyName = "LineTotal"

            SetModeUI()
            _revaluationService = New CostCorrectionService(ConnStr)
        Finally
            IsLoading = False
        End Try
    End Sub
    Private Function ToDecSafe(r As DataRow, ParamArray cols() As String) As Decimal

        For Each c In cols
            If r.Table.Columns.Contains(c) AndAlso Not IsDBNull(r(c)) Then
                Return Convert.ToDecimal(r(c))
            End If
        Next

        Return 0D

    End Function
    Private Sub SetModeUI()

        dgvMain.AllowUserToAddRows = False
        dgvMain.AllowUserToDeleteRows = False
        dgvMain.EditMode = DataGridViewEditMode.EditOnEnter

        If IsCancelMode Then
            dgvMain.ReadOnly = True

            ResetEditGuard()
            RefreshAdjustmentDeltaView()
            RefreshDisplayedTotals()
            dgvMain.Refresh()
            Return
        End If

        ' === Edit mode ===
        dgvMain.ReadOnly = False

        For Each col As DataGridViewColumn In dgvMain.Columns
            Dim isAllowed As Boolean =
            String.Equals(col.Name, "colQty", StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(col.Name, "colUnitPrice", StringComparison.OrdinalIgnoreCase)

            col.ReadOnly = Not isAllowed
        Next

        ResetEditGuard()
        cboVATRate.Enabled = True
        cboTargetStore.Enabled = False
        txtPartnerName.ReadOnly = True
        txtPhone.ReadOnly = True
        txtSubTotal.ReadOnly = True
        txtVATTotal.ReadOnly = True
        txtGrandTotal.ReadOnly = True

        If CurrentOperationType = "PRO" Then
            ApplyProductionEditRules()
        End If
        If IsCancelMode Then

            dgvMain.ReadOnly = True
            dgvProduced.ReadOnly = True
            dgvProductionCalculations.ReadOnly = True

            txtProductionAmount.ReadOnly = True

            ResetProductionValuesForCancel()

            RecalculateProductionTotals()
            ResetEditGuard()
            dgvMain.Refresh()

            Return
        End If
        If CurrentOperationType = "CUT" Then

            dgvOutPut.ReadOnly = False

            For Each col As DataGridViewColumn In dgvOutPut.Columns
                col.ReadOnly = (col.DataPropertyName <> "Quantity")
            Next
            If IsCancelMode Then
                dgvOutPut.ReadOnly = True
                ResetCuttingValuesForCancel()
                Return
            End If

        End If

    End Sub
    Private Sub ApplyArabicHeaders(grid As DataGridView, headers As Dictionary(Of String, String))
        If grid Is Nothing OrElse grid.Columns Is Nothing Then Return

        For Each col As DataGridViewColumn In grid.Columns
            Dim key = col.DataPropertyName
            If String.IsNullOrWhiteSpace(key) Then key = col.Name

            If headers.ContainsKey(key) Then
                col.HeaderText = headers(key)
            End If
        Next
    End Sub
    Private Sub ResetEditGuard()
        _editedRowsSnapshots.Clear()
    End Sub
    Private Sub RestoreAllEditedRows()

        If _editedRowsSnapshots Is Nothing OrElse _editedRowsSnapshots.Count = 0 Then Return

        For Each kvp In _editedRowsSnapshots

            Dim rowIndex As Integer = kvp.Key
            Dim snapshot As Dictionary(Of String, Object) = kvp.Value

            If rowIndex < 0 OrElse rowIndex >= dgvMain.Rows.Count Then Continue For

            Dim drv As DataRowView = TryCast(dgvMain.Rows(rowIndex).DataBoundItem, DataRowView)
            If drv Is Nothing Then Continue For

            Dim r As DataRow = drv.Row

            For Each col In snapshot
                If r.Table.Columns.Contains(col.Key) Then
                    r(col.Key) = col.Value
                End If
            Next

        Next

        ' تحديث الواجهات بعد الاسترجاع
        RefreshAdjustmentDeltaView()
        RefreshDisplayedTotals()
        dgvMain.Refresh()

        ' تنظيف بعد الاسترجاع
        _editedRowsSnapshots.Clear()

    End Sub
    Private Sub ClearDependentResults()
        ' Affected Operations
        Dim dtAffected As DataTable = TryCast(dgvAffectedOperations.DataSource, DataTable)
        If dtAffected IsNot Nothing Then
            dtAffected.Rows.Clear()
        End If

        ' Simulation
        Dim dtSim As DataTable = TryCast(dgvSimulation.DataSource, DataTable)
        If dtSim IsNot Nothing Then
            dtSim.Rows.Clear()
        End If

        ' (اختياري) لو عندك جريد روابط/نتائج أخرى امسحها هنا أيضًا
        _lastSimulationLedger = Nothing
        _lastSimulationLinks = Nothing
    End Sub
    Private Sub SetProductionReadOnly()
        dgvProduced.ReadOnly = True
        ' TextBoxes
        For Each ctrl As Control In Me.Controls
            If TypeOf ctrl Is TextBox Then
                CType(ctrl, TextBox).ReadOnly = True
            End If
        Next

        ' افتح فقط هذا
        txtProductionAmount.ReadOnly = False
        For Each col As DataGridViewColumn In dgvProduced.Columns

            Dim name = col.Name.ToLower()

            If col.DataPropertyName = "Length" OrElse
               col.DataPropertyName = "Width" OrElse
               col.DataPropertyName = "Height" OrElse
               col.DataPropertyName = "Quantity" Then

                col.ReadOnly = False
            Else
                col.ReadOnly = True
            End If

        Next
        dgvProductionCalculations.ReadOnly = True
        If CurrentSubCategoryID = 11 Then
            dgvProduced.ReadOnly = True
            Return
        End If
        dgvProductionCalculations.ReadOnly = True

    End Sub
    Private Sub tabMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabMain.SelectedIndexChanged
        '========================================
        ' 1) تحديث نوع العملية
        '========================================
        Select Case tabMain.SelectedTab.Name

            Case "tabPUR"
                CurrentOperationType = "PUR"

            Case "tabSRT"
                CurrentOperationType = "SRT"

            Case "tabPRO"
                CurrentOperationType = "PRO"

            Case "tabCUT"
                CurrentOperationType = "CUT"

            Case "tabSCR"
                CurrentOperationType = "SCR"

        End Select

        '========================================
        ' 2) نقل الكنترول أولاً
        '========================================
        If tabMain.SelectedTab.Name = "tabSRT" Then

            dgvMain.Parent = tabSRT
            Panel3.Parent = tabSRT

        ElseIf tabMain.SelectedTab.Name = "tabPUR" Then

            dgvMain.Parent = tabPUR
            Panel3.Parent = tabPUR

        End If

        '========================================
        ' 3) ثم Reset
        '========================================
        ResetFormForNewTab()

        '========================================
        ' 4) إعادة ربط الحدث (اختياري)
        '========================================
        RemoveHandler dgvMain.CellEndEdit, AddressOf dgvMain_CellEndEdit
        AddHandler dgvMain.CellEndEdit, AddressOf dgvMain_CellEndEdit
    End Sub
    Private Sub ResetFormForNewTab()

        '========================================
        ' 1) Reset الحالة العامة
        '========================================
        CurrentDocumentID = 0
        IsCancelMode = False

        rbtnCancelMode.Checked = False
        rbtnEditMode.Checked = True

        '========================================
        ' 2) تفريغ الجريدات
        '========================================
        dgvMain.DataSource = Nothing
        dgvProductionCalculations.DataSource = Nothing
        dgvProduced.DataSource = Nothing
        dgvOutPut.DataSource = Nothing

        dgvAdjustResult.DataSource = Nothing
        dgvAffectedOperations.DataSource = Nothing
        dgvSimulation.DataSource = Nothing

        '========================================
        ' 3) تفريغ الحقول (EMPTY وليس 0)
        '========================================

        ' TextBoxes
        txtTotalProductionCost.Clear()
        txtTotalProductionQTY.Clear()
        txtProductUnitCost.Clear()
        txtAvailableQTY.Clear()
        txtProductionCode.Clear()
        txtProductID.Clear()
        txtProductionAmount.Clear()
        txtProductionUnit.Clear()
        txtTotalProductionVolume.Clear()
        txtTotalChemicalQty.Clear()
        txtPastProductAverageCost.Clear()

        txtSubTotal.Clear()
        txtVATTotal.Clear()
        txtGrandTotal.Clear()

        txtPartnerName.Clear()
        txtPhone.Clear()

        txtCuttingCode.Clear()
        txtSourceStore.Clear()
        txtProductCode.Clear()
        TotalPcsOutPut.Clear()
        txtTotalVolumeOutPut.Clear()

        '========================================
        ' 4) ComboBoxes (إلغاء الاختيار)
        '========================================
        cboDocumentID.SelectedIndex = -1
        cboPaymentTerm.SelectedIndex = -1
        cboPartnerCode.SelectedIndex = -1
        cboTargetStore.SelectedIndex = -1
        cboVATRate.SelectedIndex = -1

        '========================================
        ' 5) Reset الكاش
        '========================================
        _lastSimulationLedger = Nothing
        _lastSimulationLinks = Nothing

        '========================================
        ' 6) تحديث الواجهة
        '========================================
        SetModeUI()

    End Sub


    ' CONTROLS
    Private Sub btnLoadOriginalDocument_Click(
    sender As Object,
    e As EventArgs
) Handles btnLoadOriginalDocument.Click

        Dim tabName As String = tabMain.SelectedTab.Name

        Select Case tabName

            Case "tabPUR"
                Using f As New frmPurchaseSearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedDocumentID <= 0 Then Exit Sub
                    LoadOriginalDocumentForRevaluation(f.SelectedDocumentID)
                    SetModeUI()
                End Using
            Case "tabSRT"
                Using f As New frmSalesReturnsearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedDocumentID <= 0 Then Exit Sub
                    LoadOriginalDocumentForRevaluation(f.SelectedDocumentID)
                    SetModeUI()
                End Using


            Case "tabPRO", "tabProduction"
                Using f As New frmProductionSearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedProductionID <= 0 Then Exit Sub

                    LoadProductionForRevaluation(f.SelectedProductionID)

                    SetModeUI()
                    '                SetProductionReadOnly()
                End Using

            Case "tabCUT", "tabCut"
                Using f As New frmCuttingSearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedCuttingID <= 0 Then Exit Sub

                    LoadCuttingForRevaluation(f.SelectedCuttingID)

                    SetModeUI()
                End Using

            Case "tabSCR", "tabScrap"
                Using f As New frmCuttingWasteCalculatorSearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedWasteID <= 0 Then Exit Sub
                    LoadOriginalDocumentForRevaluation(f.SelectedWasteID)
                    SetModeUI()
                End Using

            Case Else
                MessageBox.Show("اسم التاب غير معروف: " & tabName)

        End Select

    End Sub
    Private Sub btnAdvanceAnalysis_Click(sender As Object, e As EventArgs) _
    Handles btnAdvanceAnalysis.Click

        If _lastSimulationLedger Is Nothing Then
            MessageBox.Show("قم بتشغيل Preview أولاً")
            Exit Sub
        End If

        Dim f As New frmCostCorrectionAdvanceAnalysis

        f.LoadSimulation(_lastSimulationLedger, _lastSimulationLinks)

        f.Show()

    End Sub
    Private Sub btnAffectedOperations_Click(
    sender As Object,
    e As EventArgs
) Handles btnAffectedOperations.Click

        Dim dtAdj As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)

        If dtAdj Is Nothing OrElse dtAdj.Rows.Count = 0 Then
            MessageBox.Show("لا يوجد تعديل.")
            Exit Sub
        End If
        Dim detailIDs = GetChangedDetailIDs()

        If detailIDs Is Nothing OrElse detailIDs.Count = 0 Then
            MessageBox.Show("لا يوجد تغيير فعلي.")
            Exit Sub
        End If

        Dim result As DataTable =
        _revaluationService.GetAffectedCostDependencies(detailIDs)

        FillAffectedOperations(result)

    End Sub
    Private Sub btnPreveiw_Click(sender As Object, e As EventArgs) Handles btnPreveiw.Click

        If dgvAffectedOperations.DataSource Is Nothing Then
            MessageBox.Show("قم بجلب العمليات المتأثرة أولاً")
            Exit Sub
        End If

        RunPreviewSimulationFull()

    End Sub






    ' GENERAL OPERATIONS
    Private Sub InitializeAdjustmentDeltaGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("ProductCode", GetType(String))
        dt.Columns.Add("ProductName", GetType(String))

        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("QtyDelta", GetType(Decimal))

        dt.Columns.Add("OldUnitCost", GetType(Decimal))
        dt.Columns.Add("NewUnitCost", GetType(Decimal))
        dt.Columns.Add("CostDelta", GetType(Decimal))
        dt.Columns.Add("RowType", GetType(String))     ' IN / MIX / OUT
        dt.Columns.Add("UnitName", GetType(String))    ' حبة / م3
        dgvAdjustResult.DataSource = dt

        dgvAdjustResult.ReadOnly = True
        dgvAdjustResult.AllowUserToAddRows = False
        dgvAdjustResult.AllowUserToDeleteRows = False

        For Each col As DataGridViewColumn In dgvAdjustResult.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next

    End Sub
    Private Sub RefreshAdjustmentDeltaView()

        Select Case CurrentOperationType
            Case "PUR", "SRT"
                BuildAdjustmentDelta_PUR_SRT()

            Case "PRO"

                BuildAdjustmentDelta_PRO()
            Case "CUT"
                BuildAdjustmentDelta_CUT()
            Case "SCR"
                ' لاحقًا

        End Select

    End Sub
    Private Sub RefreshDisplayedTotals()

        Dim subTotal As Decimal = 0D          ' Net before VAT (after discount)
        Dim vatTotal As Decimal = 0D
        Dim grandTotal As Decimal = 0D

        Dim isTaxInclusive As Boolean = chkIsTaxInclusive.Checked
        Dim dt As DataTable = TryCast(dgvMain.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            txtSubTotal.Text = "0.00"
            txtVATTotal.Text = "0.00"
            txtGrandTotal.Text = "0.00"
            Exit Sub
        End If
        For Each row In dt.Rows
            If row.RowState = DataRowState.Deleted Then Continue For

            Dim qty As Decimal = ToDec(row("NewQty"))
            Dim unitPrice As Decimal = ToDec(row("NewUnitPrice"))

            Dim discountRate As Decimal = ToDec(row("DiscountRate"))
            If discountRate < 0D Then discountRate = 0D
            If discountRate > 100D Then discountRate = 100D

            Dim taxRate As Decimal = ToDec(row("TaxRate"))
            If taxRate < 0D Then taxRate = 0D

            Dim includeTaxLine As Boolean = True

            If dt.Columns.Contains("IncludeTax") Then
                If Not IsDBNull(row("IncludeTax")) Then
                    includeTaxLine = CBool(row("IncludeTax"))
                End If
            End If
            Dim grossAmount As Decimal = qty * unitPrice
            Dim discountAmount As Decimal = grossAmount * discountRate / 100D

            ' Base amount for tax calc depends on inclusive/exclusive
            Dim taxableAmount As Decimal = grossAmount - discountAmount
            Dim taxAmount As Decimal = 0D
            Dim netAmount As Decimal = 0D
            Dim lineTotal As Decimal = 0D

            If Not includeTaxLine OrElse taxRate <= 0D Then
                ' no tax
                netAmount = taxableAmount
                taxAmount = 0D
                lineTotal = netAmount
            ElseIf isTaxInclusive Then
                ' Price already includes tax => extract tax portion
                ' taxableAmount here is "price incl tax after discount"
                Dim divisor As Decimal = 1D + (taxRate / 100D)
                If divisor <= 0D Then divisor = 1D

                netAmount = taxableAmount / divisor
                taxAmount = taxableAmount - netAmount
                lineTotal = taxableAmount
            Else
                ' Tax exclusive => add tax on top
                netAmount = taxableAmount
                taxAmount = netAmount * taxRate / 100D
                lineTotal = netAmount + taxAmount
            End If

            ' rounding (adjust if you use 3 decimals etc.)
            grossAmount = Math.Round(grossAmount, 2, MidpointRounding.AwayFromZero)
            discountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero)
            taxableAmount = Math.Round(taxableAmount, 2, MidpointRounding.AwayFromZero)
            netAmount = Math.Round(netAmount, 2, MidpointRounding.AwayFromZero)
            taxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero)
            lineTotal = Math.Round(lineTotal, 2, MidpointRounding.AwayFromZero)

            row("GrossAmount") = grossAmount
            row("DiscountAmount") = discountAmount
            row("TaxableAmount") = taxableAmount
            row("NetAmount") = netAmount
            row("TaxAmount") = taxAmount
            row("LineTotal") = lineTotal

            subTotal += netAmount
            vatTotal += taxAmount
            grandTotal += lineTotal
        Next

        txtSubTotal.Text = subTotal.ToString("N2")
        txtVATTotal.Text = vatTotal.ToString("N2")
        txtGrandTotal.Text = grandTotal.ToString("N2")

    End Sub
    'AffectedOperationTable
    Private Sub InitializeAffectedOperationsGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("InQty", GetType(Decimal))
        dt.Columns.Add("OutQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("InUnitCost", GetType(Decimal))
        dt.Columns.Add("OutUnitCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("RootLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))


        dgvAffectedOperations.Columns.Clear()
        dgvAffectedOperations.AutoGenerateColumns = True
        dgvAffectedOperations.DataSource = dt
        dgvAffectedOperations.Columns("LedgerID").DisplayIndex = 0
        dgvAffectedOperations.Columns("TransactionID").DisplayIndex = 1
        dgvAffectedOperations.Columns("SourceDetailID").DisplayIndex = 2
        dgvAffectedOperations.Columns("ProductID").DisplayIndex = 3
        dgvAffectedOperations.Columns("BaseProductID").DisplayIndex = 4
        dgvAffectedOperations.Columns("StoreID").DisplayIndex = 5
        dgvAffectedOperations.Columns("OperationTypeID").DisplayIndex = 6

        dgvAffectedOperations.Columns("LocalOldQty").DisplayIndex = 7
        dgvAffectedOperations.Columns("OldQty").DisplayIndex = 8
        dgvAffectedOperations.Columns("InQty").DisplayIndex = 9
        dgvAffectedOperations.Columns("OutQty").DisplayIndex = 10
        dgvAffectedOperations.Columns("NewQty").DisplayIndex = 11
        dgvAffectedOperations.Columns("LocalNewQty").DisplayIndex = 12

        dgvAffectedOperations.Columns("OldAvgCost").DisplayIndex = 13
        dgvAffectedOperations.Columns("InUnitCost").DisplayIndex = 14
        dgvAffectedOperations.Columns("OutUnitCost").DisplayIndex = 15
        dgvAffectedOperations.Columns("NewAvgCost").DisplayIndex = 16

        dgvAffectedOperations.Columns("LedgerSequence").DisplayIndex = 17
        dgvAffectedOperations.Columns("SourceLedgerID").DisplayIndex = 18
        dgvAffectedOperations.Columns("RootLedgerID").DisplayIndex = 19
        dgvAffectedOperations.Columns("SupersededByLedgerID").DisplayIndex = 20
        dgvAffectedOperations.Columns("RootTransactionID").DisplayIndex = 21

        dgvAffectedOperations.Columns("PostingDate").DisplayIndex = 22

        For Each col As DataGridViewColumn In dgvAffectedOperations.Columns
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next
    End Sub
    Private Sub FillAffectedOperations(source As DataTable)

        '========================================
        ' 1) تأكد أن DataSource موجود
        '========================================
        Dim dt As DataTable = TryCast(dgvAffectedOperations.DataSource, DataTable)

        If dt Is Nothing Then
            dt = CreateAffectedOperationsTable() ' 🔥 ننشئه من جديد
            dgvAffectedOperations.DataSource = dt
        Else
            dt.Rows.Clear()
        End If

        If source Is Nothing Then Exit Sub

        '========================================
        ' 2) تعبئة البيانات
        '========================================
        For Each r As DataRow In source.Rows

            Dim row = dt.NewRow()

            row("LedgerID") = r("LedgerID")
            row("TransactionID") = r("TransactionID")
            row("SourceDetailID") = If(IsDBNull(r("SourceDetailID")), DBNull.Value, r("SourceDetailID"))

            row("ProductID") = r("ProductID")
            row("BaseProductID") = If(source.Columns.Contains("BaseProductID"), r("BaseProductID"), r("ProductID"))
            row("StoreID") = r("StoreID")
            row("OperationTypeID") = r("OperationTypeID")

            row("LocalOldQty") = r("LocalOldQty")
            row("OldQty") = r("OldQty")

            row("InQty") = r("InQty")
            row("OutQty") = r("OutQty")

            row("NewQty") = r("NewQty")
            row("LocalNewQty") = r("LocalNewQty")

            row("OldAvgCost") = r("OldAvgCost")

            row("InUnitCost") = If(source.Columns.Contains("InUnitCost"), r("InUnitCost"), 0D)
            row("OutUnitCost") = If(source.Columns.Contains("OutUnitCost"), r("OutUnitCost"), 0D)

            row("NewAvgCost") = r("NewAvgCost")

            row("LedgerSequence") = r("LedgerSequence")
            row("SourceLedgerID") = r("SourceLedgerID")

            row("RootLedgerID") = If(source.Columns.Contains("RootLedgerID") AndAlso Not IsDBNull(r("RootLedgerID")), r("RootLedgerID"), DBNull.Value)
            row("SupersededByLedgerID") = If(source.Columns.Contains("SupersededByLedgerID") AndAlso Not IsDBNull(r("SupersededByLedgerID")), r("SupersededByLedgerID"), DBNull.Value)
            row("RootTransactionID") = If(source.Columns.Contains("RootTransactionID") AndAlso Not IsDBNull(r("RootTransactionID")), r("RootTransactionID"), DBNull.Value)

            row("PostingDate") = r("PostingDate")

            dt.Rows.Add(row)

        Next

    End Sub
    Private Function CreateAffectedOperationsTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("InQty", GetType(Decimal))
        dt.Columns.Add("OutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("InUnitCost", GetType(Decimal))
        dt.Columns.Add("OutUnitCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Long))
        dt.Columns.Add("SourceLedgerID", GetType(Long))

        dt.Columns.Add("RootLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(DateTime))

        Return dt

    End Function
    Private Function GetChangedDetailIDs() As List(Of Integer)

        Dim dt As DataTable = Nothing

        Select Case CurrentOperationType

            Case "PRO"
                dt = _revaluationService.GetChangedDetails_PRO(CurrentDocumentID)

                If dt Is Nothing Then Return New List(Of Integer)
            Case "CUT"
                dt = _revaluationService.GetChangedDetails_CUT(CurrentDocumentID)

            Case "PUR", "SRT"
                dt = _revaluationService.GetChangedDetails_PUR_SRT(CurrentDocumentID)

            Case "SCR"
                MessageBox.Show("not yet")

            Case Else
                Return New List(Of Integer)

        End Select

        ' 🔥 تحويل إلى List
        Dim result As New List(Of Integer)

        If dt Is Nothing Then Return result

        For Each r As DataRow In dt.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            Dim id As Integer = ToInt(r("DetailID"))

            If id > 0 Then result.Add(id)

        Next

        Return result.Distinct().ToList()

    End Function
    Private Sub ApplyLinks_TransferAndProduction(simLedgers As DataTable)

        If simLedgers Is Nothing OrElse simLedgers.Rows.Count = 0 Then Exit Sub

        Dim links As DataTable = _revaluationService.GetSimulationLinks(simLedgers)
        _lastSimulationLinks = links

        ' Build lookup for ledgers
        Dim ledgerLookup =
    simLedgers.AsEnumerable().
    Where(Function(r) Not IsDBNull(r("LedgerID"))).
    ToLookup(Function(r) CLng(r("LedgerID")))

        Dim getLedgerRow =
        Function(id As Long) As DataRow
            Dim rows = ledgerLookup(id)
            If rows IsNot Nothing AndAlso rows.Any() Then
                Return rows.First()
            End If
            Return Nothing
        End Function

        ' Get a reliable unit cost for "source OUT"
        Dim getOutUnitCost =
        Function(row As DataRow) As Decimal
            If row Is Nothing Then Return 0D

            Dim c As Decimal = ToDec(row("CorrectOutUnitCost"))
            If c > 0D Then Return c

            c = ToDec(row("OldAvgCost"))
            If c > 0D Then Return c

            Return ToDec(row("NewAvgCost"))
        End Function

        '=========================================================
        ' 1) TRANSFER (LinkType=1): Target IN cost = Source OUT cost
        '=========================================================
        For Each link As DataRow In links.Rows

            If CInt(link("LinkType")) <> 1 Then Continue For

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            Dim targetRow As DataRow = getLedgerRow(targetID)

            If sourceRow Is Nothing OrElse targetRow Is Nothing Then Continue For

            If ToDec(targetRow("CorrectInQty")) > 0D Then
                targetRow("CorrectInUnitCost") = getOutUnitCost(sourceRow)
            End If

        Next

        '=========================================================
        ' 2) CONSUME (LinkType=2): اجمع تكلفة المكونات حسب OperationGroupID
        '    poolCost(group) = sum(flowQty * sourceOutUnitCost)
        '=========================================================
        Dim consumePoolByGroup As New Dictionary(Of String, Decimal)()

        For Each link As DataRow In links.Rows

            If CInt(link("LinkType")) <> 2 Then Continue For

            Dim groupId As String = ""
            If links.Columns.Contains("OperationGroupID") AndAlso Not IsDBNull(link("OperationGroupID")) Then
                groupId = link("OperationGroupID").ToString()
            End If

            If String.IsNullOrWhiteSpace(groupId) Then
                ' بدون OperationGroupID لا يمكن ربط الاستهلاك بالمخرجات بشكل صحيح
                Continue For
            End If

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim flowQty As Decimal = ToDec(link("FlowQty"))

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            If sourceRow Is Nothing Then Continue For

            Dim cost As Decimal = flowQty * getOutUnitCost(sourceRow)

            If Not consumePoolByGroup.ContainsKey(groupId) Then
                consumePoolByGroup(groupId) = 0D
            End If

            consumePoolByGroup(groupId) += cost

        Next

        '=========================================================
        ' 3) OUTPUTS (LinkType=3): وزّع poolCost على مخرجات نفس OperationGroupID
        '    توزيع حسب FlowQty للمخرجات
        '=========================================================
        For Each groupId As String In consumePoolByGroup.Keys

            Dim poolCost As Decimal = consumePoolByGroup(groupId)
            If poolCost <= 0D Then Continue For

            ' احصل روابط المخرجات لهذا الجروب
            Dim outputLinks As List(Of DataRow) =
            links.AsEnumerable().
            Where(Function(l)
                      If CInt(l("LinkType")) <> 3 Then Return False
                      If Not links.Columns.Contains("OperationGroupID") Then Return False
                      If IsDBNull(l("OperationGroupID")) Then Return False
                      Return String.Equals(l("OperationGroupID").ToString(), groupId, StringComparison.OrdinalIgnoreCase)
                  End Function).
            ToList()

            If outputLinks.Count = 0 Then Continue For

            ' مجموع كميات المخرجات
            Dim sumFlowQty As Decimal = 0D
            For Each l As DataRow In outputLinks
                sumFlowQty += ToDec(l("FlowQty"))
            Next
            If sumFlowQty <= 0D Then Continue For

            ' وزّع تكلفة الاستهلاك على كل target ledger output
            For Each l As DataRow In outputLinks

                Dim targetID As Long = CLng(l("TargetLedgerID"))
                Dim flowQty As Decimal = ToDec(l("FlowQty"))

                Dim targetRow As DataRow = getLedgerRow(targetID)
                If targetRow Is Nothing Then Continue For

                Dim allocatedCost As Decimal = poolCost * (flowQty / sumFlowQty)

                Dim targetInQty As Decimal = ToDec(targetRow("CorrectInQty"))
                If targetInQty <= 0D Then targetInQty = flowQty

                If targetInQty > 0D Then
                    targetRow("CorrectInUnitCost") = allocatedCost / targetInQty
                End If

                ' خزّن معلومات الجروب داخل السيميوليشن (للتشخيص)
                If simLedgers.Columns.Contains("OperationGroupID") Then
                    targetRow("OperationGroupID") = groupId
                End If
                If simLedgers.Columns.Contains("GroupSeq") AndAlso links.Columns.Contains("GroupSeq") Then
                    If Not IsDBNull(l("GroupSeq")) Then targetRow("GroupSeq") = CInt(l("GroupSeq"))
                End If

            Next

        Next

        '=========================================================
        ' 4) SCRAP (LinkType=9): لا نوزع عليه تكلفة (يظل 0)
        '=========================================================
        ' لا شيء هنا intentionally

    End Sub
    Private Function CaptureRowSnapshot(rowIndex As Integer) As Dictionary(Of String, Object)
        Dim result As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

        If rowIndex < 0 OrElse rowIndex >= dgvMain.Rows.Count Then Return result

        Dim drv As DataRowView = TryCast(dgvMain.Rows(rowIndex).DataBoundItem, DataRowView)
        If drv Is Nothing Then Return result

        Dim r As DataRow = drv.Row
        For Each c As DataColumn In r.Table.Columns
            result(c.ColumnName) = If(IsDBNull(r(c.ColumnName)), DBNull.Value, r(c.ColumnName))
        Next

        Return result
    End Function
    Private Function ExtractAffectedPreviewInputsFromUI() As List(Of RevalAffectedInputDto)

        Dim list As New List(Of RevalAffectedInputDto)

        Select Case CurrentOperationType

            Case "PUR"

                Dim dt As DataTable = TryCast(dgvMain.DataSource, DataTable)
                If dt Is Nothing Then Return list

                For Each r As DataRow In dt.Rows

                    If r.RowState = DataRowState.Deleted Then Continue For

                    Dim item As New RevalAffectedInputDto()

                    item.DetailID = ToInt(r("DetailID"))
                    item.ProductID = ToInt(r("ProductID"))

                    item.ProductCode = r("ProductCode").ToString()
                    item.ProductName = r("ProductName").ToString()
                    item.ProductTypeID = r("ProductTypeID").ToString()
                    item.UnitID = r("UnitID").ToString()

                    item.SourceStoreID =
    If(IsDBNull(r("SourceStoreID")), CType(Nothing, Integer?), ToInt(r("SourceStoreID")))

                    item.TargetStoreID =
    If(IsDBNull(r("TargetStoreID")), CType(Nothing, Integer?), ToInt(r("TargetStoreID")))

                    item.OldQty = ToDec(r("OldQty"))
                    item.NewQty = ToDec(r("NewQty"))

                    item.OldUnitPrice = ToDec(r("OldUnitPrice"))
                    item.NewUnitPrice = ToDec(r("NewUnitPrice"))

                    item.GrossAmount = ToDec(r("GrossAmount"))
                    item.DiscountRate = ToDec(r("DiscountRate"))
                    item.DiscountAmount = ToDec(r("DiscountAmount"))

                    item.IncludeTax = False
                    item.TaxRate = ToDec(r("TaxRate"))

                    item.TaxTypeID =
    If(IsDBNull(r("TaxTypeID")), CType(Nothing, Integer?), ToInt(r("TaxTypeID")))

                    item.TaxableAmount = ToDec(r("TaxableAmount"))
                    item.TaxAmount = ToDec(r("TaxAmount"))

                    item.NetAmount = ToDec(r("NetAmount"))
                    item.LineTotal = ToDec(r("LineTotal"))

                    list.Add(item)

                Next

        End Select

        Return list

    End Function



    'PUR,SRT

    Protected Sub LoadOriginalDocumentForRevaluation(documentID As Integer)

        If documentID <= 0 Then Exit Sub

        EnterUIGuard()
        Try
            Dim header = _revaluationService.GetDocumentHeaderForRevaluation(documentID)
            BindDocumentHeaderToForm(header)

            ' تحميل البيانات فقط (بدون Reval)
            Dim data =
            _revaluationService.GetDocumentDetailsForRevaluation(documentID)

            dgvMain.DataSource = data

        Finally
            ExitUIGuard()
        End Try

    End Sub
    Private Sub BindDocumentHeaderToForm(header As RevaluationDocumentHeaderDto)

        If header Is Nothing Then Exit Sub

        CurrentDocumentID = header.DocumentID
        IsInventoryPosted = header.IsInventoryPosted

        cboDocumentID.Text = header.DocumentNo
        dtpDocumentDate.Value = header.DocumentDate

        If header.PartnerID.HasValue Then
            cboPartnerCode.SelectedValue = header.PartnerID.Value
        Else
            cboPartnerCode.SelectedIndex = -1
        End If

        chkIsTaxInclusive.Checked = header.IsTaxInclusive

        txtSubTotal.Text = header.TotalAmount.ToString("N2")
        txtVATTotal.Text = header.TotalTax.ToString("N2")
        txtGrandTotal.Text = header.TotalNetAmount.ToString("N2")


        If header.PaymentTermID.HasValue Then
            cboPaymentTerm.SelectedValue = header.PaymentTermID.Value
        Else
            cboPaymentTerm.SelectedIndex = -1
        End If


        RefreshFormStatus(CurrentDocumentID)

    End Sub
    Private Sub dgvMain_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
        If IsLoading Then Return
        If IsCancelMode Then Return
        RefreshAdjustmentDeltaView()
        RefreshDisplayedTotals()
    End Sub
    Private Sub dgvMain_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvMain.CellValidating
        Dim colName = dgvMain.Columns(e.ColumnIndex).Name

        If colName = "colQty" OrElse colName = "colUnitPrice" Then
            If Not String.IsNullOrEmpty(e.FormattedValue.ToString()) Then
                Dim val As Decimal
                If Decimal.TryParse(e.FormattedValue.ToString(), val) Then
                    If val < 0 Then
                        MessageBox.Show("القيم السالبة ممنوعة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        e.Cancel = True
                        Exit Sub
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub dgvMain_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellValueChanged

        If IsLoading Then Return
        If IsCancelMode Then Return

        ' 🔥 هذا أهم سطر
        dgvMain.EndEdit()

        ' 🔥 تأكيد إضافي
        Me.Validate()

        ' الآن نحسب
        BuildAdjustmentDelta_PUR_SRT()

    End Sub
    Private Sub ResetPurchaseValuesForCancel()

        Dim dt As DataTable = TryCast(dgvMain.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        For Each r As DataRow In dt.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            '========================================
            ' 1) الكمية
            '========================================
            If dt.Columns.Contains("NewQty") Then
                r("NewQty") = 0D
            End If

            '========================================
            ' 2) السعر
            '========================================
            If dt.Columns.Contains("NewUnitPrice") Then
                r("NewUnitPrice") = 0D
            End If

            '========================================
            ' 3) إعادة ضبط القيم المالية (اختياري لكن صحيح)
            '========================================
            If dt.Columns.Contains("GrossAmount") Then
                r("GrossAmount") = 0D
            End If

            If dt.Columns.Contains("DiscountAmount") Then
                r("DiscountAmount") = 0D
            End If

            If dt.Columns.Contains("TaxableAmount") Then
                r("TaxableAmount") = 0D
            End If

            If dt.Columns.Contains("TaxAmount") Then
                r("TaxAmount") = 0D
            End If

            If dt.Columns.Contains("NetAmount") Then
                r("NetAmount") = 0D
            End If

            If dt.Columns.Contains("LineTotal") Then
                r("LineTotal") = 0D
            End If

        Next

        dgvMain.Refresh()

    End Sub
    Private Sub BuildAdjustmentDelta_PUR_SRT()

        '========================================
        ' 0) تجهيز جدول النتائج
        '========================================
        Dim dtResult As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)

        If dtResult Is Nothing Then
            InitializeAdjustmentDeltaGrid()
            dtResult = TryCast(dgvAdjustResult.DataSource, DataTable)
            If dtResult Is Nothing Then Exit Sub
        End If

        dtResult.Rows.Clear()

        If Not dtResult.Columns.Contains("SourceDetailID") Then
            dtResult.Columns.Add("SourceDetailID", GetType(Long))
        End If

        '========================================
        ' 1) جلب الصفوف من dgvMain
        '========================================
        Dim dtMain As DataTable = TryCast(dgvMain.DataSource, DataTable)

        If dtMain Is Nothing OrElse dtMain.Rows.Count = 0 Then Exit Sub

        '========================================
        ' 2) استخراج DetailIDs
        '========================================
        Dim detailIDs As New List(Of Long)

        For Each r As DataRow In dtMain.Rows

            If r.RowState = DataRowState.Deleted Then Continue For
            If Not dtMain.Columns.Contains("DetailID") Then Continue For
            If IsDBNull(r("DetailID")) Then Continue For

            detailIDs.Add(CLng(r("DetailID")))

        Next

        If detailIDs.Count = 0 Then Exit Sub

        '========================================
        ' 3) جلب الترانزكشن
        '========================================
        Dim dtTrans As DataTable =
        _revaluationService.GetTransactionDetailsByDetailIDs(detailIDs)

        If dtTrans Is Nothing OrElse dtTrans.Rows.Count = 0 Then Exit Sub

        '========================================
        ' 4) Dictionary
        '========================================
        Dim dict As New Dictionary(Of Long, DataRow)

        For Each tr As DataRow In dtTrans.Rows

            If IsDBNull(tr("SourceDocumentDetailID")) Then Continue For

            Dim key As Long = CLng(tr("SourceDocumentDetailID"))

            If Not dict.ContainsKey(key) Then
                dict.Add(key, tr)
            End If

        Next

        If dict.Count = 0 Then Exit Sub

        '========================================
        ' 5) المقارنة
        '========================================
        For Each r As DataRow In dtMain.Rows

            If r.RowState = DataRowState.Deleted Then Continue For
            If IsDBNull(r("DetailID")) Then Continue For

            Dim sourceID As Long = CLng(r("DetailID"))

            If Not dict.ContainsKey(sourceID) Then Continue For

            Dim tr As DataRow = dict(sourceID)

            '========================================
            ' القيم القديمة
            '========================================
            Dim oldQty As Decimal = ToDec(tr("Quantity"))
            Dim oldCost As Decimal = ToDec(tr("UnitCost"))

            '========================================
            ' القيم الجديدة (من الجريد)
            '========================================
            Dim newQty As Decimal = ToDecSafe(r, "Quantity", "NewQty")
            Dim newCost As Decimal = ToDecSafe(r, "UnitPrice", "NewUnitPrice")

            '========================================
            ' الفروقات
            '========================================
            Dim deltaQty As Decimal = newQty - oldQty
            Dim deltaCost As Decimal = (newCost) - (oldCost)

            If deltaQty = 0D AndAlso deltaCost = 0D Then Continue For

            '========================================
            ' إضافة النتيجة
            '========================================
            Dim row As DataRow = dtResult.NewRow()

            row("SourceDetailID") = sourceID
            row("ProductID") = ToInt(tr("ProductID"))
            row("ProductCode") = tr("ProductCode").ToString()
            row("ProductName") = tr("ProductName").ToString()

            row("OldQty") = oldQty
            row("NewQty") = newQty
            row("QtyDelta") = deltaQty

            row("OldUnitCost") = oldCost
            row("NewUnitCost") = newCost
            row("CostDelta") = deltaCost

            dtResult.Rows.Add(row)
        Next

    End Sub

    'PRODUCTION

    Private Sub LoadProductionForRevaluation(productionID As Integer)

        Dim data = _revaluationService.GetProductionForRevaluation(productionID)
        CurrentDocumentID = productionID
        CurrentOperationType = "PRO"
        If data Is Nothing Then Exit Sub
        IsLoading = True
        '========================================
        ' 1) Inputs Grid
        '========================================
        dgvProductionCalculations.DataSource = data.Inputs

        Dim dt As DataTable = CType(dgvProductionCalculations.DataSource, DataTable)

        ' إضافة OriginalQuantity مرة واحدة فقط
        If Not dt.Columns.Contains("OriginalQuantity") Then
            dt.Columns.Add("OriginalQuantity", GetType(Decimal))
        End If

        For Each r As DataRow In dt.Rows
            r("OriginalQuantity") = r("Quantity")
        Next

        ' تنسيق الأرقام
        For Each col As DataGridViewColumn In dgvProductionCalculations.Columns
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next

        '========================================
        ' 2) Outputs Grid
        '========================================
        Dim dtOut As DataTable = data.Outputs
        ' 🔥 فك القفل عن الأعمدة المطلوبة
        If dtOut.Columns.Contains("Quantity") Then
            dtOut.Columns("Quantity").ReadOnly = False
        End If

        If dtOut.Columns.Contains("Length") Then
            dtOut.Columns("Length").ReadOnly = False
        End If

        If dtOut.Columns.Contains("Width") Then
            dtOut.Columns("Width").ReadOnly = False
        End If

        If dtOut.Columns.Contains("Height") Then
            dtOut.Columns("Height").ReadOnly = False
        End If

        dtOut.Columns("VolumeM3").ReadOnly = False
        ' حساب الحجم
        If dtOut.Columns.Contains("VolumeM3") Then
            dtOut.Columns("VolumeM3").ReadOnly = False
        End If

        '========================================
        ' 3) Setup Grid Columns (مرة واحدة فقط)
        '========================================
        dgvProduced.AutoGenerateColumns = False
        dgvProduced.Columns.Clear()

        ' Length
        Dim colL As New DataGridViewTextBoxColumn()
        colL.Name = "colLength"
        colL.HeaderText = "Length"
        colL.DataPropertyName = "Length"
        colL.DefaultCellStyle.Format = "N2"
        dgvProduced.Columns.Add(colL)

        ' Width
        Dim colW As New DataGridViewTextBoxColumn()
        colW.Name = "colWidth"
        colW.HeaderText = "Width"
        colW.DataPropertyName = "Width"
        colW.DefaultCellStyle.Format = "N2"
        dgvProduced.Columns.Add(colW)

        ' Height
        Dim colH As New DataGridViewTextBoxColumn()
        colH.Name = "colHeight"
        colH.HeaderText = "Height"
        colH.DataPropertyName = "Height"
        colH.DefaultCellStyle.Format = "N2"
        dgvProduced.Columns.Add(colH)

        ' Quantity
        Dim colQ As New DataGridViewTextBoxColumn()
        colQ.Name = "colQuantity"
        colQ.HeaderText = "Qty"
        colQ.DataPropertyName = "Quantity"
        colQ.DefaultCellStyle.Format = "N2"
        dgvProduced.Columns.Add(colQ)

        ' Volume (ReadOnly 🔥)
        Dim colV As New DataGridViewTextBoxColumn()
        colV.Name = "colVolume"
        colV.HeaderText = "Volume (m3)"
        colV.DataPropertyName = "VolumeM3"
        colV.DefaultCellStyle.Format = "N3"
        colV.ReadOnly = True
        dgvProduced.Columns.Add(colV)

        ' ربط البيانات بعد تعريف الأعمدة
        dgvProduced.DataSource = dtOut
        dgvProduced.ReadOnly = False
        ' 🔥 حفظ الكمية الأصلية للإنتاج
        If Not dtOut.Columns.Contains("OriginalQuantity") Then
            dtOut.Columns.Add("OriginalQuantity", GetType(Decimal))
        End If

        For Each r As DataRow In dtOut.Rows
            r("OriginalQuantity") = r("Quantity")
        Next
        '========================================
        ' 4) TextBoxes
        '========================================
        txtProductionAmount.Text = data.ProductionBaseValue.ToString("N2")
        _originalProductionBaseValue = data.ProductionBaseValue
        txtProductID.Text = data.ProductCode
        txtProductID.Tag = data.ProductID
        txtProductionCode.Text = data.ProductionCode
        txtTotalChemicalQty.Text = data.TotalChemicalQty.ToString("N2")

        txtTotalProductionQTY.Text = data.TotalProductionQty.ToString("N2")
        _originalProductionQty = data.TotalProductionQty
        _originalProductionUnitCost = data.UnitCost
        _originalProductionVolume = data.TotalProductionVolume
        CurrentSubCategoryID = data.SubCategoryID

        txtTotalProductionCost.Text = data.TotalChemicalCost.ToString("N2")
        txtProductUnitCost.Text = data.UnitCost.ToString("N2")
        txtTotalProductionVolume.Text = data.TotalProductionVolume.ToString("N2")
        txtPastProductAverageCost.Text = data.PastAvgCost.ToString("N2")
        txtProductionUnit.Text = data.UnitName
        '========================================
        ' 5) Fix TotalCost Column (Inputs)
        '========================================
        dt.Columns("TotalCost").ReadOnly = False
        For Each r As DataRow In dt.Rows
            r("TotalCost") = ToDec(r("Quantity")) * ToDec(r("UnitCost"))
        Next
        If dgvProduced.Columns.Contains("DetailID") Then
            dgvProduced.Columns("DetailID").Visible = False
        End If
        dgvProductionCalculations.Refresh()

        IsLoading = False
    End Sub
    Private Sub RecalculateConsumption()

        If _originalProductionQty <= 0 Then Exit Sub

        Dim newBaseValue As Decimal = ToDec(txtProductionAmount.Text)

        If _originalProductionBaseValue <= 0 Then Exit Sub
        If newBaseValue <= 0 Then Exit Sub

        Dim factor As Decimal = newBaseValue / _originalProductionBaseValue
        '========================================
        ' 🔥 تعديل الإنتاج للحالة 11
        '========================================
        If CurrentSubCategoryID = 11 Then

            Dim dtProduced As DataTable =
        CType(dgvProduced.DataSource, DataTable)

            If dtProduced IsNot Nothing Then
                For Each r As DataRow In dtProduced.Rows

                    Dim originalQty As Decimal = ToDec(r("OriginalQuantity"))
                    r("Quantity") = originalQty * factor

                Next
            End If

        End If
        Dim dt As DataTable =
        CType(dgvProductionCalculations.DataSource, DataTable)

        ' 🔥 الحل هنا
        If dt.Columns.Contains("TotalCost") Then
            dt.Columns("TotalCost").ReadOnly = False
        End If

        For Each r As DataRow In dt.Rows

            Dim originalQty As Decimal = ToDec(r("OriginalQuantity"))
            Dim newChemicalQty As Decimal = originalQty * factor

            r("Quantity") = newChemicalQty
            r("TotalCost") = newChemicalQty * ToDec(r("UnitCost"))

        Next

        dgvProductionCalculations.Refresh()
        RecalculateProductionTotals()

    End Sub
    Private Sub RecalculateProductionTotals()
        IsLoading = True
        dgvProduced.EndEdit()
        dgvProductionCalculations.EndEdit()

        Dim totalProductionQty As Decimal = 0D
        Dim totalChemicalAmount As Decimal = 0D
        Dim totalProductionCost As Decimal = 0D
        Dim totalProductionVolume As Decimal = 0D
        Dim unitCost As Decimal = 0D

        '========================================
        ' 1) إجمالي الإنتاج
        '========================================
        Dim dtProduced As DataTable = TryCast(dgvProduced.DataSource, DataTable)
        If dtProduced IsNot Nothing AndAlso dtProduced.Rows.Count > 0 Then

            totalProductionQty =
            dtProduced.AsEnumerable().
            Sum(Function(r) ToDec(r("Quantity")))

            totalProductionVolume =
            dtProduced.AsEnumerable().
            Sum(Function(r) ToDec(r("VolumeM3")))
        End If

        '========================================
        ' 2) إجمالي تكلفة المواد الخام
        '========================================
        Dim dtInputs As DataTable = TryCast(dgvProductionCalculations.DataSource, DataTable)
        If dtInputs IsNot Nothing AndAlso dtInputs.Rows.Count > 0 Then
            For Each r As DataRow In dtInputs.Rows
                r("TotalCost") = ToDec(r("Quantity")) * ToDec(r("UnitCost"))
            Next
            TotalChemicalQty =
    dtInputs.AsEnumerable().
    Sum(Function(r) ToDec(r("Quantity")))

            totalChemicalCost =
    dtInputs.AsEnumerable().
    Sum(Function(r) ToDec(r("TotalCost")))

        End If

        '========================================
        ' 3) إجمالي تكلفة الإنتاج
        ' نفس منطق frmProduction:
        ' totalProductionCost = مجموع تكلفة المواد فقط
        '========================================
        totalProductionCost = totalChemicalCost


        '========================================
        ' 4) حساب تكلفة الوحدة حسب نوع الإنتاج
        ' 9,10 = على الفوليوم
        ' 11   = على الكمية
        '========================================
        Select Case CurrentSubCategoryID

            Case 9, 10
                If totalProductionVolume > 0D Then
                    unitCost = totalProductionCost / totalProductionVolume
                Else
                    unitCost = 0D
                End If

            Case 11
                If totalProductionQty > 0D Then
                    unitCost = totalProductionCost / totalProductionQty
                Else
                    unitCost = 0D
                End If

            Case Else
                If totalProductionVolume > 0 Then
                    unitCost = totalProductionCost / totalProductionVolume
                ElseIf totalProductionQty > 0 Then
                    unitCost = totalProductionCost / totalProductionQty
                End If

        End Select

        '========================================
        ' 5) تحديث الحقول
        '========================================
        txtTotalChemicalQty.Text = TotalChemicalQty.ToString("N2")
        txtTotalProductionCost.Text = totalChemicalCost.ToString("N2")
        txtTotalProductionVolume.Text = totalProductionVolume.ToString("N2")
        txtTotalProductionQTY.Text = totalProductionQty.ToString("N2")
        txtProductUnitCost.Text = unitCost.ToString("N2")
        IsLoading = False
    End Sub
    Private Sub dgvProductionCalculations_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProductionCalculations.CellEndEdit

        Dim dt As DataTable = CType(dgvProductionCalculations.DataSource, DataTable)

        If e.RowIndex < 0 Then Return

        Dim r As DataRow = dt.Rows(e.RowIndex)

        Dim qty As Decimal = ToDec(r("Quantity"))
        Dim cost As Decimal = ToDec(r("UnitCost"))

        r("TotalCost") = qty * cost

        dgvProductionCalculations.Refresh()
        RecalculateProductionTotals()
        RefreshAdjustmentDeltaView()
    End Sub
    Private Sub dgvProductionCalculations_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProductionCalculations.CellValueChanged

        If IsLoading Then Return

        RecalculateProductionTotals()

    End Sub
    Private Sub ApplyProductionEditRules()

        If CurrentOperationType <> "PRO" Then Exit Sub
        If Not IsCancelMode Then
            dgvProduced.ReadOnly = False
            For Each col As DataGridViewColumn In dgvProduced.Columns
                If col.Name = "colVolume" Then
                    col.ReadOnly = True ' لا يمكن التعديل على الحجم المحسوب
                Else
                    col.ReadOnly = False
                End If
            Next
            txtProductionAmount.ReadOnly = False
        Else
            dgvProduced.ReadOnly = True
            txtProductionAmount.ReadOnly = True
            ' إذا أردت تصفير القيم في الإلغاء: مرر على الصفوف وضع Quantity و VolumeM3 = 0
        End If

        Select Case CurrentSubCategoryID

        '========================================
        ' CASE 9
        '========================================
            Case 9
                ' مسموح الاثنين
                dgvProduced.ReadOnly = False
                txtProductionAmount.ReadOnly = False

        '========================================
        ' CASE 10
        '========================================
            Case 10
                ' الجريد فقط
                dgvProduced.ReadOnly = False
                txtProductionAmount.ReadOnly = True

        '========================================
        ' CASE 11
        '========================================
            Case 11
                ' التكست فقط
                dgvProduced.ReadOnly = True
                txtProductionAmount.ReadOnly = False

        End Select

    End Sub
    Private Sub ApplyProductionAmountToProducedGrid()

        Dim dt As DataTable = TryCast(dgvProduced.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            txtProductionAmount.Text = "0.00"
            Exit Sub
        End If

        Dim totalVolume As Decimal =
            dt.AsEnumerable().Sum(Function(r) ToDec(r("VolumeM3")))

        txtProductionAmount.Text = totalVolume.ToString("N2")

    End Sub
    Private Sub dgvProduced_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProduced.CellEndEdit

        If IsLoading Then Return

        Dim dt As DataTable = CType(dgvProduced.DataSource, DataTable)
        Dim r As DataRow = dt.Rows(e.RowIndex)

        Dim l = ToDec(r("Length"))
        Dim w = ToDec(r("Width"))
        Dim h = ToDec(r("Height"))
        Dim q = ToDec(r("Quantity"))

        r("VolumeM3") = l * w * h * q / 1000000D
        If CurrentOperationType <> "PRO" Then Exit Sub
        If IsLoading Then Exit Sub
        If IsCancelMode Then Exit Sub

        If CurrentSubCategoryID = 10 Then
            ApplyProductionAmountToProducedGrid()


        End If
        dgvProduced.Refresh()
        RecalculateProductionTotals()
        RefreshAdjustmentDeltaView()
    End Sub
    Private Sub txtProductionAmount_TextChanged(
    sender As Object,
    e As EventArgs
) Handles txtProductionAmount.TextChanged

        If IsLoading Then Exit Sub
        If IsCancelMode Then Exit Sub

        ' 🔥 الحل هنا
        dgvProductionCalculations.EndEdit()
        dgvProduced.EndEdit()

        RecalculateConsumption()
        RecalculateProductionTotals()
        RefreshAdjustmentDeltaView()
    End Sub
    Private Sub ResetProductionValuesForCancel()

        '========================================
        ' 1) تصفير TextBox
        '========================================
        txtProductionAmount.Text = "0"

        '========================================
        ' 2) تصفير OUTPUT (dgvProduced)
        '========================================
        Dim dtProduced As DataTable = TryCast(dgvProduced.DataSource, DataTable)

        If dtProduced IsNot Nothing Then
            For Each r As DataRow In dtProduced.Rows

                If r.RowState = DataRowState.Deleted Then Continue For

                If dtProduced.Columns.Contains("Length") Then r("Length") = 0D
                If dtProduced.Columns.Contains("Width") Then r("Width") = 0D
                If dtProduced.Columns.Contains("Height") Then r("Height") = 0D
                If dtProduced.Columns.Contains("Quantity") Then r("Quantity") = 0D
                If dtProduced.Columns.Contains("VolumeM3") Then r("VolumeM3") = 0D

            Next
        End If

        dgvProduced.EndEdit()
        dgvProduced.Refresh()

        '========================================
        ' 3) تصفير INPUT (المواد الخام)
        '========================================
        Dim dtInputs As DataTable = TryCast(dgvProductionCalculations.DataSource, DataTable)

        If dtInputs IsNot Nothing Then
            For Each r As DataRow In dtInputs.Rows

                If r.RowState = DataRowState.Deleted Then Continue For

                If dtInputs.Columns.Contains("Quantity") Then r("Quantity") = 0D

                ' مهم: لا تعبث بأي أعمدة أصلية أخرى
                ' ولا تخزن Old هنا — لأن old يأتي من DB

            Next
        End If

        dgvProductionCalculations.EndEdit()
        dgvProductionCalculations.Refresh()

        '========================================
        ' 4) إعادة الحساب
        '========================================
        RecalculateProductionTotals()

    End Sub
    Private Sub BuildAdjustmentDelta_PRO()

        '========================================
        ' 0) تجهيز جدول النتائج
        '========================================
        Dim dtResult As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)

        If dtResult Is Nothing Then
            InitializeAdjustmentDeltaGrid()
            dtResult = TryCast(dgvAdjustResult.DataSource, DataTable)
            If dtResult Is Nothing Then Exit Sub
        End If

        dtResult.Rows.Clear()

        If Not dtResult.Columns.Contains("SourceDetailID") Then
            dtResult.Columns.Add("SourceDetailID", GetType(Long))
        End If

        '========================================
        ' 1) جمع الصفوف من الجريد
        '========================================
        Dim rows As New List(Of DataRow)

        Select Case CurrentOperationType

            Case "PRO"

                Dim dtOut As DataTable = TryCast(dgvProduced.DataSource, DataTable)
                Dim dtIn As DataTable = TryCast(dgvProductionCalculations.DataSource, DataTable)

                If dtOut IsNot Nothing Then
                    For Each r As DataRow In dtOut.Rows
                        If r.RowState <> DataRowState.Deleted Then rows.Add(r)
                    Next
                End If

                If dtIn IsNot Nothing Then
                    For Each r As DataRow In dtIn.Rows
                        If r.RowState <> DataRowState.Deleted Then rows.Add(r)
                    Next
                End If

            Case Else
                Exit Sub

        End Select

        If rows.Count = 0 Then Exit Sub

        '========================================
        ' 2) استخراج SourceDocumentDetailID
        '========================================
        Dim detailIDs As New List(Of Long)

        For Each r As DataRow In rows

            If Not r.Table.Columns.Contains("SourceDocumentDetailID") Then Continue For
            If IsDBNull(r("SourceDocumentDetailID")) Then Continue For

            detailIDs.Add(CLng(r("SourceDocumentDetailID")))

        Next

        If detailIDs.Count = 0 Then Exit Sub

        '========================================
        ' 3) جلب بيانات الترانزكشن
        '========================================
        Dim dtTrans As DataTable =
        _revaluationService.GetTransactionDetailsByDetailIDs(detailIDs)

        If dtTrans Is Nothing OrElse dtTrans.Rows.Count = 0 Then Exit Sub

        '========================================
        ' 4) بناء Dictionary
        '========================================
        Dim dict As New Dictionary(Of Long, DataRow)

        For Each tr As DataRow In dtTrans.Rows

            If IsDBNull(tr("SourceDocumentDetailID")) Then Continue For

            Dim key As Long = CLng(tr("SourceDocumentDetailID"))

            If Not dict.ContainsKey(key) Then
                dict.Add(key, tr)
            End If

        Next

        If dict.Count = 0 Then Exit Sub

        '========================================
        ' 5) المقارنة وبناء الدلتا
        '========================================
        For Each r As DataRow In rows

            If Not r.Table.Columns.Contains("SourceDocumentDetailID") Then Continue For
            If IsDBNull(r("SourceDocumentDetailID")) Then Continue For

            Dim sourceID As Long = CLng(r("SourceDocumentDetailID"))

            If Not dict.ContainsKey(sourceID) Then Continue For

            Dim tr As DataRow = dict(sourceID)

            ' القيم القديمة
            Dim oldQty As Decimal = ToDec(tr("Quantity"))
            Dim oldCost As Decimal = ToDec(tr("UnitCost"))

            ' القيم الجديدة
            Dim newQty As Decimal = 0D
            If dgvProduced.DataSource IsNot Nothing AndAlso
   r.Table Is CType(dgvProduced.DataSource, DataTable) Then
                If CurrentSubCategoryID = 9 OrElse CurrentSubCategoryID = 10 Then
                    newQty = ToDecSafe(r, "VolumeM3", "TotalVolume")
                ElseIf CurrentSubCategoryID = 11 Then
                    newQty = ToDecSafe(r, "Quantity", "NewQty")
                End If
            ElseIf dgvProductionCalculations.DataSource IsNot Nothing AndAlso
                   r.Table Is CType(dgvProductionCalculations.DataSource, DataTable) Then

                ' 🟢 هذا INPUT (الداخل)
                newQty = ToDecSafe(r, "Quantity", "NewQty")

            End If
            Dim newCost As Decimal = oldCost

            ' الفروقات
            Dim deltaQty As Decimal = newQty - oldQty
            Dim deltaCost As Decimal = newCost - oldCost

            If deltaQty = 0D AndAlso deltaCost = 0D Then Continue For

            ' إضافة للنتيجة
            Dim row As DataRow = dtResult.NewRow()

            row("SourceDetailID") = sourceID
            row("ProductID") = ToInt(tr("ProductID"))
            row("ProductCode") = tr("ProductCode").ToString()
            row("ProductName") = tr("ProductName").ToString()

            row("OldQty") = oldQty
            row("NewQty") = newQty
            row("QtyDelta") = deltaQty

            row("OldUnitCost") = oldCost
            row("NewUnitCost") = newCost
            row("CostDelta") = deltaCost

            dtResult.Rows.Add(row)
        Next

    End Sub



    'CUTTING
    Private Sub dgvMain_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit

        If IsLoading Then Return
        If IsCancelMode Then
            e.Cancel = True
            Return
        End If

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            e.Cancel = True
            Return
        End If

        Dim col As DataGridViewColumn = dgvMain.Columns(e.ColumnIndex)
        Dim prop As String = col.DataPropertyName
        Dim name As String = col.Name

        Dim allowed As Boolean =
        _allowedEditProps.Contains(prop) OrElse _allowedEditColumnNames.Contains(name)

        If Not allowed Then
            e.Cancel = True
            MessageBox.Show("غير مسموح تعديل هذا الحقل. المسموح فقط: الكمية أو السعر.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        '========================================
        ' 🔥 Multi-row Snapshot
        '========================================
        If Not _editedRowsSnapshots.ContainsKey(e.RowIndex) Then
            _editedRowsSnapshots(e.RowIndex) = CaptureRowSnapshot(e.RowIndex)
        End If

        '========================================
        ' 🔥 أي تعديل جديد يلغي النتائج السابقة
        '========================================
        ClearDependentResults()

    End Sub
    Private Sub InitializeCuttingGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("Length", GetType(Decimal))
        dt.Columns.Add("Width", GetType(Decimal))
        dt.Columns.Add("Height", GetType(Decimal))
        dt.Columns.Add("Quantity", GetType(Decimal))

        dt.Columns.Add("ProductCode", GetType(String))
        dt.Columns.Add("ProductType", GetType(String))

        dt.Columns.Add("PieceVolume", GetType(Decimal))
        dt.Columns.Add("TotalVolume", GetType(Decimal))

        dgvOutPut.AutoGenerateColumns = True
        dgvOutPut.DataSource = dt

        For Each col As DataGridViewColumn In dgvOutPut.Columns
            col.ReadOnly = (col.DataPropertyName <> "Quantity")

            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N3"
            End If
        Next

    End Sub
    Private Sub RecalculateCuttingTotals()

        Dim dt As DataTable = TryCast(dgvOutPut.DataSource, DataTable)

        Dim totalQty As Decimal = 0D
        Dim totalVolume As Decimal = 0D

        If dt IsNot Nothing Then

            totalQty =
                dt.AsEnumerable().
                Sum(Function(r) ToDec(r("Quantity")))

            totalVolume =
                dt.AsEnumerable().
                Sum(Function(r) ToDec(r("TotalVolume")))

        End If

        TotalPcsOutPut.Text = totalQty.ToString("N2")
        txtTotalVolumeOutPut.Text = totalVolume.ToString("N3")

    End Sub
    Private Sub ValidateCuttingBalance(changedRow As DataRow)

        Dim dtSource As DataTable = CType(dgvOutPut.DataSource, DataTable)

        ' مجموع الكميات من الجريد
        Dim totalQty As Decimal =
        dtSource.AsEnumerable().
        Sum(Function(x) ToDec(x("Quantity")))

        ' الكمية المتاحة
        Dim availableQty As Decimal = ToDec(txtAvailableQTY.Tag)

        ' الكمية الأصلية (قبل التعديل)

    End Sub
    Private Sub dgvOutPut_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvOutPut.CurrentCellDirtyStateChanged

        If dgvOutPut.IsCurrentCellDirty Then
            dgvOutPut.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub
    Private Sub dgvOutPut_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvOutPut.CellEndEdit

        If IsLoading Then Return
        dgvOutPut.EndEdit()
        Dim dtSource As DataTable = CType(dgvOutPut.DataSource, DataTable)

        '========================================
        ' VALIDATION (CUT)
        '========================================
        Dim availableQty As Decimal = ToDec(txtAvailableQTY.Tag)

        Dim totalNewQty As Decimal =
        dtSource.AsEnumerable().
        Sum(Function(x) ToDec(x("Quantity")))

        If totalNewQty > availableQty Then
            MessageBox.Show("الكمية المطلوبة تتجاوز المتاح", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)

            ' رجّع القيمة القديمة (بدون كسر)
            Dim rowEdit As DataRow = dtSource.Rows(e.RowIndex)

            If dtSource.Columns.Contains("OriginalQuantity") Then
                rowEdit("Quantity") = ToDec(rowEdit("OriginalQuantity"))
            End If

            dgvOutPut.Refresh()
            Exit Sub
        End If

        '========================================
        ' الحسابات
        '========================================
        Dim rowCalc As DataRow = dtSource.Rows(e.RowIndex)

        Dim l As Decimal = ToDec(rowCalc("Length_cm"))
        Dim w As Decimal = ToDec(rowCalc("Width_cm"))
        Dim h As Decimal = ToDec(rowCalc("Height_cm"))
        Dim q As Decimal = ToDec(rowCalc("Quantity"))

        ' تحويل من cm³ إلى m³
        Dim pieceVolume As Decimal = (l * w * h) / 1000000D
        Dim totalVolume As Decimal = pieceVolume * q

        ' تحديث الأعمدة الصحيحة
        rowCalc("PieceVolume") = pieceVolume
        rowCalc("TotalVolume") = totalVolume

        dgvOutPut.Refresh()

        RecalculateCuttingTotals()
        RefreshAdjustmentDeltaView()
        ValidateCuttingBalance(rowCalc)
    End Sub
    Private Sub ResetCuttingValuesForCancel()

        Dim dt As DataTable = TryCast(dgvOutPut.DataSource, DataTable)

        If dt IsNot Nothing Then
            For Each r As DataRow In dt.Rows
                r("Quantity") = 0D
                r("TotalVolume") = 0D
            Next
        End If

        RecalculateCuttingTotals()

    End Sub
    Private Sub LoadCuttingForRevaluation(cuttingID As Integer)

        IsLoading = True
        CurrentDocumentID = cuttingID
        CurrentOperationType = "CUT"

        Try

            Dim data = _revaluationService.GetCuttingForRevaluation(cuttingID)
            If data Is Nothing Then Exit Sub

            '========================================
            ' 1) Header
            '========================================
            txtCuttingCode.Text = data.CuttingCode
            txtProductCode.Text = data.BaseProductCode

            '========================================
            ' 2) Outputs
            '========================================
            Dim dt As DataTable = data.Outputs
            '========================================
            ' 🔥 توحيد المفتاح (مهم جداً)
            '========================================
            If Not dt.Columns.Contains("SourceDocumentDetailID") Then
                dt.Columns.Add("SourceDocumentDetailID", GetType(Long))
            End If

            For Each row As DataRow In dt.Rows

                If dt.Columns.Contains("CutOutputID") AndAlso
       Not IsDBNull(row("CutOutputID")) Then

                    row("SourceDocumentDetailID") = CLng(row("CutOutputID"))

                End If

            Next
            ' حساب الطلب
            Dim requestQty As Decimal =
            dt.AsEnumerable().
            Sum(Function(x) ToDec(x("Quantity")))

            ' المتاح
            txtAvailableQTY.Text =
            (data.AvailableQty + requestQty).ToString("N2")

            '========================================
            ' 3) ربط الجريد
            '========================================
            dgvOutPut.DataSource = dt

            ' عناوين
            If dgvOutPut.Columns.Contains("Length_cm") Then
                dgvOutPut.Columns("Length_cm").HeaderText = "الطول (سم)"
            End If

            If dgvOutPut.Columns.Contains("Width_cm") Then
                dgvOutPut.Columns("Width_cm").HeaderText = "العرض (سم)"
            End If

            If dgvOutPut.Columns.Contains("Height_cm") Then
                dgvOutPut.Columns("Height_cm").HeaderText = "الارتفاع (سم)"
            End If

            ' السماح بالتعديل
            If dgvOutPut.Columns.Contains("Quantity") Then
                dgvOutPut.Columns("Quantity").ReadOnly = False
            End If

            '========================================
            ' 🔥 حفظ الكمية الأصلية
            '========================================
            If Not dt.Columns.Contains("OriginalQuantity") Then
                dt.Columns.Add("OriginalQuantity", GetType(Decimal))
            End If

            For Each row As DataRow In dt.Rows
                row("OriginalQuantity") = row("Quantity")
            Next

            '========================================
            ' 🔥 إخفاء DetailID
            '========================================
            If dgvOutPut.Columns.Contains("DetailID") Then
                dgvOutPut.Columns("DetailID").Visible = False
            End If

            '========================================
            ' 4) تعبئة المخزن
            '========================================
            If dt.Rows.Count > 0 AndAlso dt.Columns.Contains("StoreName") Then
                txtSourceStore.Text = dt.Rows(0)("StoreName").ToString()
            End If

            '========================================
            ' 5) حفظ المتاح
            '========================================
            txtAvailableQTY.Tag = txtAvailableQTY.Text

            '========================================
            ' 6) حساب الإجماليات
            '========================================
            RecalculateCuttingTotals()

        Finally
            IsLoading = False
        End Try

    End Sub
    Private Sub BuildAdjustmentDelta_CUT()
        '========================================
        ' 0) تجهيز جدول النتائج
        '========================================
        Dim dtResult As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)

        If dtResult Is Nothing Then
            InitializeAdjustmentDeltaGrid()
            dtResult = TryCast(dgvAdjustResult.DataSource, DataTable)
            If dtResult Is Nothing Then Exit Sub

        End If

        dtResult.Rows.Clear()

        If Not dtResult.Columns.Contains("SourceDetailID") Then
            dtResult.Columns.Add("SourceDetailID", GetType(Long))
        End If

        '========================================
        ' 1) الجريد
        '========================================
        Dim dt As DataTable = TryCast(dgvOutPut.DataSource, DataTable)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        '========================================
        ' 2) IDs
        '========================================
        Dim detailIDs As New List(Of Long)

        For Each r As DataRow In dt.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            If IsDBNull(r("SourceDocumentDetailID")) Then Continue For

            detailIDs.Add(CLng(r("SourceDocumentDetailID")))
        Next

        If detailIDs.Count = 0 Then Exit Sub

        '========================================
        ' 3) الترانزكشن
        '========================================
        Dim dtTrans As DataTable =
        _revaluationService.GetTransactionDetailsByDetailIDs(detailIDs)

        If dtTrans Is Nothing OrElse dtTrans.Rows.Count = 0 Then Exit Sub

        '========================================
        ' 4) Dictionary
        '========================================
        Dim dict As New Dictionary(Of Long, DataRow)

        For Each tr As DataRow In dtTrans.Rows
            If IsDBNull(tr("SourceDocumentDetailID")) Then Continue For

            Dim key As Long = CLng(tr("SourceDocumentDetailID"))

            If Not dict.ContainsKey(key) Then
                dict.Add(key, tr)
            End If
        Next

        If dict.Count = 0 Then Exit Sub

        '========================================
        ' 5) تجميع الخارج
        '========================================

        Dim totalOldVolume As Decimal = 0D
        Dim totalNewVolume As Decimal = 0D

        Dim cuttingSourceID As Long = 0
        Dim cuttingOldCost As Decimal = 0D

        '========================================
        ' 6) الداخل
        '========================================
        For Each r As DataRow In dt.Rows
            Dim sourceID As Long = CLng(r("SourceDocumentDetailID"))

            If r.RowState = DataRowState.Deleted Then Continue For
            If IsDBNull(r("SourceDocumentDetailID")) Then Continue For


            If Not dict.ContainsKey(sourceID) Then Continue For

            Dim tr As DataRow = dict(sourceID)

            Dim oldQty As Decimal = ToDec(tr("Quantity"))
            Dim oldCost As Decimal = ToDec(tr("UnitCost"))

            '========================================
            ' MIX؟
            '========================================
            Dim isMix As Boolean = False
            If dt.Columns.Contains("IsMix") Then
                If Not IsDBNull(r("IsMix")) Then
                    isMix = CBool(r("IsMix"))
                End If
            End If

            Dim newQty As Decimal = 0D
            Dim oldCompare As Decimal = 0D
            Dim newCompare As Decimal = 0D

            If isMix Then
                ' 🔴 MIX → بالمتر المكعب

                Dim length As Decimal = ToDecSafe(r, "Length_cm")
                Dim width As Decimal = ToDecSafe(r, "Width_cm")
                Dim height As Decimal = ToDecSafe(r, "Height_cm")

                Dim oldVolume As Decimal = oldQty
                Dim newVolume As Decimal =
                                    ToDecSafe(r, "Quantity") * length * width * height / 1000000d

                oldCompare = oldVolume
                newCompare = newVolume

                totalOldVolume += oldVolume
                totalNewVolume += newVolume

            Else
                ' 🟢 عادي → بالكمية

                newQty = ToDecSafe(r, "Quantity")

                oldCompare = oldQty
                newCompare = newQty

                ' نحول للحجم للخارج
                Dim length As Decimal = ToDecSafe(r, "Length_cm")
                Dim width As Decimal = ToDecSafe(r, "Width_cm")
                Dim height As Decimal = ToDecSafe(r, "Height_cm")

                totalOldVolume += oldQty * length * width * height / 1000000
                totalNewVolume += newQty * length * width * height / 1000000

            End If

            Dim deltaQty As Decimal = newCompare - oldCompare
            Dim deltaCost As Decimal = oldCost - oldCost ' صفر فعلياً

            If cuttingSourceID = 0 Then
                cuttingSourceID = sourceID
                cuttingOldCost = oldCost
            End If

            If deltaQty = 0D Then Continue For

            ' RESULT (الداخل)
            Dim row As DataRow = dtResult.NewRow()
            row("ProductID") = ToInt(r("ProductID"))
            row("ProductCode") = r("ProductCode").ToString()
            row("ProductName") = r("ProductName").ToString()
            row("OldQty") = oldCompare
            row("NewQty") = newCompare
            row("QtyDelta") = deltaQty

            row("OldUnitCost") = oldCost
            row("NewUnitCost") = oldCost
            row("CostDelta") = 0D

            dtResult.Rows.Add(row)

        Next

        '========================================
        ' 7) الخارج (Volume فقط)
        '========================================
        If cuttingSourceID <> 0 Then

            Dim oldOut As Decimal = -totalOldVolume
            Dim newOut As Decimal = -totalNewVolume

            Dim deltaOut As Decimal = newOut - oldOut

            If deltaOut <> 0D Then

                Dim row As DataRow = dtResult.NewRow()
                Dim trOut As DataRow = dict(cuttingSourceID)

                row("SourceDetailID") = cuttingSourceID
                row("ProductID") = ToInt(trOut("ProductID"))
                row("ProductCode") = trOut("ProductCode").ToString()
                row("ProductName") = trOut("ProductName").ToString()
                row("OldQty") = oldOut
                row("NewQty") = newOut
                row("QtyDelta") = deltaOut

                row("OldUnitCost") = cuttingOldCost
                row("NewUnitCost") = cuttingOldCost
                row("CostDelta") = 0D

                dtResult.Rows.Add(row)

            End If

        End If

    End Sub


    'SIMULATION


    Private Function CreateSimulationTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("OperationGroupID", GetType(String))
        dt.Columns.Add("GroupSeq", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("CorrectInQty", GetType(Decimal))
        dt.Columns.Add("CorrectOutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))

        dt.Columns.Add("CorrectInUnitCost", GetType(Decimal))
        dt.Columns.Add("CorrectOutUnitCost", GetType(Decimal))

        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("RootLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))

        Return dt

    End Function
    Private Sub InitializeSimulationGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("OperationGroupID", GetType(String))
        dt.Columns.Add("GroupSeq", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("CorrectInQty", GetType(Decimal))
        dt.Columns.Add("CorrectOutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))

        dt.Columns.Add("CorrectInUnitCost", GetType(Decimal))
        dt.Columns.Add("CorrectOutUnitCost", GetType(Decimal))

        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("RootLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))

        dgvSimulation.AutoGenerateColumns = True
        dgvSimulation.DataSource = dt

        dgvSimulation.ReadOnly = True
        dgvSimulation.AllowUserToAddRows = False
        dgvSimulation.AllowUserToDeleteRows = False

        For Each col As DataGridViewColumn In dgvSimulation.Columns

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If

        Next

    End Sub
    Private Sub FillSimulationGrid()

        Dim source As DataTable =
    CType(dgvAffectedOperations.DataSource, DataTable)

        Dim dt As DataTable =
    CType(dgvSimulation.DataSource, DataTable)

        dt.Rows.Clear()

        For Each r As DataRow In source.Rows

            Dim row = dt.NewRow()

            row("LedgerID") = r("LedgerID")
            row("TransactionID") = r("TransactionID")
            row("SourceDetailID") = r("SourceDetailID")

            row("ProductID") = r("ProductID")
            row("BaseProductID") = r("BaseProductID")
            row("StoreID") = r("StoreID")
            row("OperationTypeID") = r("OperationTypeID")

            row("LocalOldQty") = r("LocalOldQty")
            row("OldQty") = r("OldQty")

            '================================
            ' Correct Quantities
            '================================

            Dim inQty As Decimal = Convert.ToDecimal(r("InQty"))
            Dim outQty As Decimal = Convert.ToDecimal(r("OutQty"))

            If inQty > 0 Then
                row("CorrectInQty") = inQty
                row("CorrectOutQty") = 0D
            ElseIf outQty > 0 Then
                row("CorrectInQty") = 0D
                row("CorrectOutQty") = outQty
            Else
                row("CorrectInQty") = 0D
                row("CorrectOutQty") = 0D
            End If

            row("NewQty") = r("NewQty")
            row("LocalNewQty") = r("LocalNewQty")

            row("OldAvgCost") = r("OldAvgCost")

            '================================
            ' Correct Costs
            '================================

            If inQty > 0 Then
                row("CorrectInUnitCost") = r("InUnitCost")
                row("CorrectOutUnitCost") = 0D
            ElseIf outQty > 0 Then
                row("CorrectInUnitCost") = 0D
                row("CorrectOutUnitCost") = r("OutUnitCost")
            Else
                row("CorrectInUnitCost") = 0D
                row("CorrectOutUnitCost") = 0D
            End If

            row("NewAvgCost") = r("NewAvgCost")

            row("LedgerSequence") = r("LedgerSequence")
            row("SourceLedgerID") = r("SourceLedgerID")
            row("RootLedgerID") = r("RootLedgerID")

            row("SupersededByLedgerID") =
        If(IsDBNull(r("SupersededByLedgerID")),
           DBNull.Value,
           r("SupersededByLedgerID"))

            row("RootTransactionID") =
        If(IsDBNull(r("RootTransactionID")),
           DBNull.Value,
           r("RootTransactionID"))

            row("PostingDate") = r("PostingDate")

            dt.Rows.Add(row)

        Next

    End Sub
    Private Function GetSimulationCorrections() As Dictionary(Of Long, (NewQty As Decimal, NewCost As Decimal))

        Dim result As New Dictionary(Of Long, (Decimal, Decimal))

        Dim adjust As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)
        If adjust Is Nothing OrElse adjust.Rows.Count = 0 Then Return result

        For Each r As DataRow In adjust.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            If IsDBNull(r("SourceDetailID")) Then Continue For

            Dim detailId As Long = CLng(r("SourceDetailID"))
            Dim newQty As Decimal = ToDec(r("NewQty"))
            Dim newCost As Decimal = ToDec(r("NewUnitCost"))

            result(detailId) = (newQty, newCost)

        Next

        Return result

    End Function
    Private Sub BuildSimulationTable()

        Dim source As DataTable = TryCast(dgvAffectedOperations.DataSource, DataTable)
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        Dim sim As DataTable = TryCast(dgvSimulation.DataSource, DataTable)
        If sim Is Nothing Then Exit Sub
        sim.Rows.Clear()

        Dim corrections = GetSimulationCorrections()
        If corrections.Count = 0 Then Exit Sub

        Dim orderedSource = source.AsEnumerable().
        OrderBy(Function(r) CDate(r("PostingDate"))).
        ThenBy(Function(r) CLng(r("LedgerSequence"))).
        ThenBy(Function(r) CLng(r("LedgerID")))

        For Each r As DataRow In orderedSource

            Dim row = sim.NewRow()

            For Each col As DataColumn In source.Columns
                If sim.Columns.Contains(col.ColumnName) Then
                    row(col.ColumnName) = r(col.ColumnName)
                End If
            Next

            Dim inQty As Decimal = ToDec(r("InQty"))
            Dim outQty As Decimal = ToDec(r("OutQty"))

            row("CorrectInQty") = inQty
            row("CorrectOutQty") = outQty
            row("CorrectInUnitCost") = ToDec(r("InUnitCost"))
            row("CorrectOutUnitCost") = ToDec(r("OutUnitCost"))

            '========================================
            ' 🔥 الربط الصحيح (DocumentDetail → Ledger)
            '========================================
            If Not IsDBNull(r("SourceDetailID")) Then

                Dim detailId As Long = CLng(r("SourceDetailID"))

                If corrections.ContainsKey(detailId) Then

                    Dim corr = corrections(detailId)

                    row("CorrectInQty") = corr.NewQty
                    row("CorrectOutQty") = 0D
                    row("CorrectInUnitCost") = corr.NewCost
                    row("CorrectOutUnitCost") = 0D

                End If
            End If

            sim.Rows.Add(row)

        Next

    End Sub
    Private Sub RunPreviewSimulationFull()

        BuildSimulationTable()

        Dim sim As DataTable = TryCast(dgvSimulation.DataSource, DataTable)
        If sim Is Nothing OrElse sim.Rows.Count = 0 Then Exit Sub

        ' ترتيب
        Dim ordered = sim.AsEnumerable().
            OrderBy(Function(r) CDate(r("PostingDate"))).
            ThenBy(Function(r) CLng(r("LedgerSequence"))).
            ThenBy(Function(r) CLng(r("LedgerID"))).
            ToList()

        Dim temp As DataTable = sim.Clone()
        For Each r In ordered
            temp.ImportRow(r)
        Next

        dgvSimulation.DataSource = temp
        sim = temp

        Dim maxIter As Integer = 10

        For i As Integer = 1 To maxIter
            RunSimulation()
            ApplyLinks_TransferAndProduction(sim)
        Next

        RunSimulation()

        _lastSimulationLedger = sim.Copy()

    End Sub
    Private Sub RunSimulation()

        Dim dt As DataTable = CType(dgvSimulation.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub
        For Each r As DataRow In dt.Rows
            r("NewQty") = 0D
            r("LocalNewQty") = 0D
            r("NewAvgCost") = 0D
        Next
        ' Global balances per ProductID
        Dim globalQtyByProduct As New Dictionary(Of Integer, Decimal)()
        Dim globalAvgByProduct As New Dictionary(Of Integer, Decimal)()

        ' Local balances per (ProductID, StoreID)
        Dim localQtyByProductStore As New Dictionary(Of String, Decimal)()

        For i As Integer = 0 To dt.Rows.Count - 1

            Dim r As DataRow = dt.Rows(i)

            Dim productID As Integer = CInt(r("ProductID"))
            Dim storeID As Integer = If(IsDBNull(r("StoreID")), Integer.MinValue, CInt(r("StoreID")))

            Dim inQty As Decimal = ToDec(r("CorrectInQty"))
            Dim outQty As Decimal = ToDec(r("CorrectOutQty"))

            ' Old global for this product
            Dim oldQty As Decimal
            Dim oldAvgCost As Decimal

            If Not globalQtyByProduct.ContainsKey(productID) Then
                oldQty = ToDec(r("OldQty"))
                oldAvgCost = ToDec(r("OldAvgCost"))
                globalQtyByProduct(productID) = oldQty
                globalAvgByProduct(productID) = oldAvgCost
            Else
                oldQty = globalQtyByProduct(productID)
                oldAvgCost = globalAvgByProduct(productID)
            End If

            ' Old local for this product+store
            Dim localKey As String = productID.ToString() & "|" & storeID.ToString()

            Dim localOldQty As Decimal
            If Not localQtyByProductStore.ContainsKey(localKey) Then
                localOldQty = ToDec(r("LocalOldQty"))
                localQtyByProductStore(localKey) = localOldQty
            Else
                localOldQty = localQtyByProductStore(localKey)
            End If

            ' Incoming unit cost
            ' Incoming unit cost
            Dim inUnitCost As Decimal = 0D
            If inQty > 0D Then
                Dim specified As Decimal = ToDec(r("CorrectInUnitCost"))
                If specified > 0D Then
                    inUnitCost = specified
                Else
                    ' إذا ما فيه تكلفة دخول مصححة -> اعتبرها متوسط ما قبل الحركة
                    inUnitCost = oldAvgCost
                End If
            End If
            ' New qty
            Dim newQty As Decimal = oldQty + inQty - outQty
            Dim localNewQty As Decimal = localOldQty + inQty - outQty

            ' New avg (WAC per ProductID)
            Dim newAvgCost As Decimal = oldAvgCost
            If inQty > 0D Then
                If inUnitCost > 0D Then
                    Dim denom As Decimal = oldQty + inQty
                    If denom > 0D Then
                        newAvgCost = ((oldQty * oldAvgCost) + (inQty * inUnitCost)) / denom
                    Else
                        newAvgCost = inUnitCost
                    End If
                Else
                    ' تكلفة الدخول غير معروفة بعد -> لا تغيّر المتوسط الآن
                    newAvgCost = oldAvgCost
                End If
            End If

            ' Out unit cost = avg before out
            If outQty > 0D Then
                r("CorrectOutUnitCost") = oldAvgCost
            End If

            ' Update row
            r("OldQty") = oldQty
            r("OldAvgCost") = oldAvgCost
            r("LocalOldQty") = localOldQty

            r("NewQty") = newQty
            r("LocalNewQty") = localNewQty
            r("NewAvgCost") = newAvgCost

            ' Persist state
            globalQtyByProduct(productID) = newQty
            globalAvgByProduct(productID) = newAvgCost
            localQtyByProductStore(localKey) = localNewQty

        Next

    End Sub



End Class

