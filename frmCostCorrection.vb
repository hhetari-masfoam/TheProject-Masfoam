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
    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "COR"
        End Get
    End Property
    ' --- Single-row edit guard ---
    Private _editedRowIndex As Integer = -1
    Private _editedRowSnapshot As Dictionary(Of String, Object) = Nothing
    Private _snapshotCapturedForRowIndex As Integer = -1
    ' --- single-row edit guard ---





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

    Private Sub btnLoadOriginalDocument_Click(
    sender As Object,
    e As EventArgs
) Handles btnLoadOriginalDocument.Click

        Dim tabName As String = tabMain.SelectedTab.Name

        Select Case tabName

            Case "tabPUR", "tabPurchase"
                Using f As New frmPurchaseSearch()
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

        If CurrentDocumentID > 0 Then

            Select Case CurrentOperationType
                Case "CUT"
                    LoadCuttingForRevaluation(CurrentDocumentID)
                Case "PRO"
                    LoadProductionForRevaluation(CurrentDocumentID)
                Case Else
                    LoadOriginalDocumentForRevaluation(CurrentDocumentID)
            End Select

        End If

        ' 🔥 تصفير القيم
        ResetProductionValuesForCancel()
        If CurrentOperationType = "CUT" Then
            ResetCuttingValuesForCancel()
        End If

        ' فقط في حالة PRO، اجعل الإنتاج للقراءة فقط
        If CurrentOperationType = "PRO" Then
            SetProductionReadOnly()
        End If

        SetModeUI()

        ' ✅ إعادة الحساب بعد الإلغاء
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

        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub tabMain_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles tabMain.SelectedIndexChanged

        Dim tabName As String = tabMain.SelectedTab.Name

        Select Case tabName

        ' Purchase
            Case "tabPUR", "tabPurchase"
                CurrentOperationType = "PUR"

        ' Production
            Case "tabPRO", "tabProduction"
                CurrentOperationType = "PRO"

        ' Cut
            Case "tabCUT", "tabCut"
                CurrentOperationType = "CUT"

        ' Scrap
            Case "tabSCR", "tabScrap"
                CurrentOperationType = "SCR"

            Case Else
                ' fallback safe default
                CurrentOperationType = "PUR"
        End Select

        RefreshAdjustmentDeltaView()


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

    Private Sub BuildAdjustmentDelta()

        Dim dtResult As DataTable = TryCast(dgvAdjustResult.DataSource, DataTable)

        If dtResult Is Nothing Then
            InitializeAdjustmentDeltaGrid()
            dtResult = TryCast(dgvAdjustResult.DataSource, DataTable)
            If dtResult Is Nothing Then Exit Sub
        End If

        dtResult.Rows.Clear()

        If CurrentOperationType = "PUR" Then
            Dim dt As DataTable = TryCast(dgvMain.DataSource, DataTable)
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

            For Each r As DataRow In dt.Rows
                If r.RowState = DataRowState.Deleted Then Continue For
                Dim oldQty As Decimal = ToDec(r("OldQty"))
                Dim newQty As Decimal = ToDec(r("NewQty"))
                Dim oldCost As Decimal = ToDec(r("OldUnitPrice"))
                Dim newCost As Decimal = ToDec(r("NewUnitPrice"))

                Dim deltaQty As Decimal = newQty - oldQty
                Dim deltaCost As Decimal = (newQty * newCost) - (oldQty * oldCost)

                If deltaQty <> 0D OrElse deltaCost <> 0D Then
                    Dim row As DataRow = dtResult.NewRow()

                    row("ProductID") = r("ProductID")
                    row("ProductCode") = r("ProductCode")
                    row("ProductName") = r("ProductName")
                    row("OldQty") = oldQty
                    row("NewQty") = newQty
                    row("QtyDelta") = deltaQty
                    row("OldUnitCost") = oldCost
                    row("NewUnitCost") = newCost
                    row("CostDelta") = deltaCost

                    dtResult.Rows.Add(row)
                End If
            Next
        End If
        '========================================
        ' PRO (الإنتاج)
        '========================================
        If CurrentOperationType = "PRO" Then

            Dim oldQty As Decimal
            Dim newQty As Decimal

            Select Case CurrentSubCategoryID

                Case 9, 10
                    oldQty = ToDec(_originalProductionVolume)
                    newQty = ToDec(txtTotalProductionVolume.Text)

                Case 11
                    oldQty = _originalProductionQty
                    newQty = ToDec(txtTotalProductionQTY.Text)

                Case Else
                    oldQty = _originalProductionQty
                    newQty = ToDec(txtTotalProductionQTY.Text)

            End Select
            Dim oldUnitCost As Decimal = _originalProductionUnitCost
            Dim newUnitCost As Decimal = ToDec(txtProductUnitCost.Text)

            Dim deltaQty As Decimal = newQty - oldQty
            Dim deltaCost As Decimal =
            (newQty * newUnitCost) - (oldQty * oldUnitCost)
            Dim hasUserChange As Boolean =
    Math.Abs(newQty - oldQty) > 0.0001D OrElse
    Math.Abs(newUnitCost - oldUnitCost) > 0.0001D

            If hasUserChange Then
                Dim row As DataRow = dtResult.NewRow()

                row("ProductID") = 0
                row("ProductCode") = txtProductID.Text
                row("ProductName") = "Production"

                row("OldQty") = oldQty
                row("NewQty") = newQty
                row("QtyDelta") = deltaQty

                row("OldUnitCost") = oldUnitCost
                row("NewUnitCost") = newUnitCost
                row("CostDelta") = deltaCost

                dtResult.Rows.Add(row)

            End If

        End If

        '========================================
        ' CUT
        '========================================
        If CurrentOperationType = "CUT" Then

            Dim dtCut As DataTable = TryCast(dgvOutPut.DataSource, DataTable)
            If dtCut Is Nothing OrElse dtCut.Rows.Count = 0 Then Exit Sub

            Dim totalDeltaVolume As Decimal = 0D

            If Not dtCut.Columns.Contains("OriginalQuantity") _
       OrElse Not dtCut.Columns.Contains("QtyPieces") Then
                Exit Sub
            End If

            '========================================
            ' 1) GOOD + MIX (بالحبة)
            '========================================
            For Each r As DataRow In dtCut.Rows

                Dim oldQty As Decimal = ToDec(r("OriginalQuantity"))
                Dim newQty As Decimal = ToDec(r("QtyPieces"))
                Dim deltaQty As Decimal = newQty - oldQty

                If Math.Abs(deltaQty) > 0.0001D Then

                    Dim row As DataRow = dtResult.NewRow()

                    row("ProductID") = If(dtCut.Columns.Contains("ProductID"), r("ProductID"), 0)
                    row("ProductCode") = If(dtCut.Columns.Contains("ProductCode"), r("ProductCode"), "")
                    row("ProductName") = If(dtCut.Columns.Contains("ProductName"), r("ProductName"), "Cutting")

                    row("OldQty") = oldQty
                    row("NewQty") = newQty
                    row("QtyDelta") = deltaQty

                    '========================================
                    ' 🔥 التكلفة من TransactionDetails
                    '========================================
                    Dim unitCost As Decimal = 0D

                    If dtCut.Columns.Contains("UnitCost") Then
                        unitCost = ToDec(r("UnitCost"))
                    End If

                    Dim deltaCost As Decimal = deltaQty * unitCost

                    row("OldUnitCost") = unitCost
                    row("NewUnitCost") = unitCost
                    row("CostDelta") = deltaCost

                    dtResult.Rows.Add(row)

                    '========================================
                    ' الحجم
                    '========================================
                    Dim pieceVolume As Decimal = 0D

                    If dtCut.Columns.Contains("PieceVolume") AndAlso Not IsDBNull(r("PieceVolume")) Then
                        pieceVolume = ToDec(r("PieceVolume"))
                    End If

                    totalDeltaVolume -= pieceVolume * deltaQty

                End If

            Next

            '========================================
            ' 2) OUT (بالمتر المكعب)
            '========================================
            If Math.Abs(totalDeltaVolume) > 0.0001D Then

                Dim rowOut As DataRow = dtResult.NewRow()

                rowOut("ProductID") = _currentCutBaseProductID
                rowOut("ProductCode") = txtProductCode.Text
                rowOut("ProductName") = "Output Volume (m3)"

                rowOut("OldQty") = 0D
                rowOut("NewQty") = totalDeltaVolume
                rowOut("QtyDelta") = totalDeltaVolume

                Dim totalCostDelta As Decimal =
dtResult.AsEnumerable().
Where(Function(x) x("ProductName").ToString() <> "Output Volume (m3)").
Sum(Function(x) ToDec(x("CostDelta")))

                rowOut("OldUnitCost") = 0D
                rowOut("NewUnitCost") = 0D
                rowOut("CostDelta") = -totalCostDelta

                dtResult.Rows.Add(rowOut)

            End If

        End If
        '========================================
        ' RAW MATERIALS (Inputs)
        '========================================
        Dim dtInputs As DataTable = TryCast(dgvProductionCalculations.DataSource, DataTable)

        If dtInputs IsNot Nothing Then

            For Each r As DataRow In dtInputs.Rows

                Dim oldQtyInput As Decimal = ToDec(r("OriginalQuantity"))
                Dim newQtyInput As Decimal = ToDec(r("Quantity"))

                Dim unitCost As Decimal = ToDec(r("UnitCost"))

                Dim deltaQtyInput As Decimal = newQtyInput - oldQtyInput
                Dim deltaCostInput As Decimal =
                    (newQtyInput * unitCost) - (oldQtyInput * unitCost)

                ' 🔥 فقط إذا تغير فعلي
                If Math.Abs(deltaQtyInput) > 0.0001D Then

                    Dim row As DataRow = dtResult.NewRow()

                    row("ProductID") = If(r.Table.Columns.Contains("ProductID"), r("ProductID"), 0)
                    row("ProductCode") = If(r.Table.Columns.Contains("ProductCode"), r("ProductCode"), "")
                    row("ProductName") = If(r.Table.Columns.Contains("ProductName"), r("ProductName"), "")
                    row("OldQty") = oldQtyInput
                    row("NewQty") = newQtyInput
                    row("QtyDelta") = deltaQtyInput

                    row("OldUnitCost") = unitCost
                    row("NewUnitCost") = unitCost
                    row("CostDelta") = deltaCostInput

                    dtResult.Rows.Add(row)

                End If

            Next

        End If
    End Sub


    Private Sub RefreshAdjustmentDeltaView()

        Select Case CurrentOperationType

            Case "PUR"
                BuildAdjustmentDelta()

            Case "PRO"
                BuildAdjustmentDelta()
            Case "CUT"
                BuildAdjustmentDelta()
            Case "SCR"
                ' لاحقًا

        End Select

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
    Private Sub SetModeUI()

        dgvMain.AllowUserToAddRows = False
        dgvMain.AllowUserToDeleteRows = False
        dgvMain.EditMode = DataGridViewEditMode.EditOnEnter

        If IsCancelMode Then
            dgvMain.ReadOnly = True

            ResetSingleRowEditGuard()

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

        ResetSingleRowEditGuard()

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

            ResetSingleRowEditGuard()
            dgvMain.Refresh()

            Return
        End If
        If CurrentOperationType = "CUT" Then

            dgvOutPut.ReadOnly = False

            For Each col As DataGridViewColumn In dgvOutPut.Columns
                col.ReadOnly = (col.DataPropertyName <> "QtyPieces")
            Next
            If IsCancelMode Then
                dgvOutPut.ReadOnly = True
                ResetCuttingValuesForCancel()
                Return
            End If

        End If

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

        Dim dt As DataTable = CType(dgvAffectedOperations.DataSource, DataTable)
        dt.Rows.Clear()

        If source Is Nothing Then Exit Sub

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

            If source.Columns.Contains("InUnitCost") Then
                row("InUnitCost") = r("InUnitCost")
            Else
                row("InUnitCost") = 0D
            End If

            If source.Columns.Contains("OutUnitCost") Then
                row("OutUnitCost") = r("OutUnitCost")
            Else
                row("OutUnitCost") = 0D
            End If

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
    Private Sub btnAffectedOperations_Click(sender As Object, e As EventArgs) Handles btnAffectedOperations.Click

        If CType(dgvAdjustResult.DataSource, DataTable).Rows.Count = 0 Then
            MessageBox.Show("لا يوجد تعديل على الكمية أو السعر، لذلك لا توجد عمليات متأثرة.", "تنبيه")
            Exit Sub
        End If

        Dim result As DataTable = Nothing

        Select Case CurrentOperationType

            Case "PUR"

                Dim previewInputs = ExtractAffectedPreviewInputsFromUI()

                Dim reval = _revaluationService.BuildAffectedPreviewReval(previewInputs)

                result = _revaluationService.GetAffectedCostDependenciesForPreview(
                    reval,
                    CurrentDocumentID
                )

            Case "PRO"
                Dim productionData = _revaluationService.GetProductionForRevaluation(CurrentDocumentID)

                Dim changedDetailIDs = _revaluationService.GetProductionChangedDetailIDs(
    CurrentDocumentID,
    productionData.TransactionID,
    dgvProductionCalculations.DataSource,
    ToDec(txtTotalProductionQTY.Text),
    ToDec(txtProductUnitCost.Text)
)

                If changedDetailIDs.Count = 0 Then
                    MessageBox.Show("لا يوجد تغيير فعلي.")
                    Exit Sub
                End If

                result = _revaluationService.GetAffectedCostDependenciesForProduction(
    changedDetailIDs
)

                FillAffectedOperations(result)

            Case "CUT"
                MessageBox.Show("استخراج العمليات المتأثرة للإنتاج لم يُبن بعد.")
                Exit Sub

            Case "SCR"
                MessageBox.Show("استخراج العمليات المتأثرة للسكراب لم يُبن بعد.")
                Exit Sub

            Case Else
                MessageBox.Show("نوع العملية غير مدعوم.")
                Exit Sub

        End Select

        FillAffectedOperations(result)

    End Sub

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

    Private Sub BuildSimulationTable()

        Dim source As DataTable = CType(dgvAffectedOperations.DataSource, DataTable)
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        Dim adjust As DataTable = CType(dgvAdjustResult.DataSource, DataTable)
        Dim sim As DataTable = CType(dgvSimulation.DataSource, DataTable)
        sim.Rows.Clear()

        ' 1) لازم يكون في تعديل
        If adjust Is Nothing OrElse adjust.Rows.Count = 0 Then Exit Sub

        Dim correctedProductID As Integer = CInt(adjust.Rows(0)("ProductID"))
        Dim correctedQty As Decimal = ToDec(adjust.Rows(0)("NewQty"))
        Dim correctedUnitCost As Decimal = ToDec(adjust.Rows(0)("NewUnitCost"))

        ' 2) بناء Reval الجديد
        Dim previewInputs = ExtractAffectedPreviewInputsFromUI()
        Dim reval = _revaluationService.BuildAffectedPreviewReval(previewInputs)

        ' 3) استخراج StoreID من Reval
        Dim correctedStoreID As Integer? = Nothing

        Dim dr = reval.AsEnumerable().
        FirstOrDefault(Function(r)
                           If IsDBNull(r("ProductID")) Then Return False
                           Return CInt(r("ProductID")) = correctedProductID AndAlso
                   (ToDec(r("OldQty")) <> ToDec(r("NewQty")) OrElse
                    ToDec(r("OldUnitPrice")) <> ToDec(r("NewUnitPrice")))
                       End Function)

        If dr IsNot Nothing AndAlso
       reval.Columns.Contains("SourceStoreID") AndAlso
       Not IsDBNull(dr("SourceStoreID")) Then

            correctedStoreID = CInt(dr("SourceStoreID"))
        End If

        ' 4) تحديد أول Ledger (IN)
        Dim startLedgerRow As DataRow =
        source.AsEnumerable().
        Where(Function(r)
                  If IsDBNull(r("ProductID")) Then Return False
                  If CInt(r("ProductID")) <> correctedProductID Then Return False
                  If ToDec(r("InQty")) <= 0D Then Return False

                  If correctedStoreID.HasValue Then
                      If IsDBNull(r("StoreID")) Then Return False
                      If CInt(r("StoreID")) <> correctedStoreID.Value Then Return False
                  End If

                  Return True
              End Function).
        OrderBy(Function(r) CDate(r("PostingDate"))).
        ThenBy(Function(r) CLng(r("LedgerSequence"))).
        ThenBy(Function(r) CLng(r("LedgerID"))).
        FirstOrDefault()

        ' fallback بدون شرط المخزن
        If startLedgerRow Is Nothing Then
            startLedgerRow =
            source.AsEnumerable().
            Where(Function(r)
                      If IsDBNull(r("ProductID")) Then Return False
                      If CInt(r("ProductID")) <> correctedProductID Then Return False
                      If ToDec(r("InQty")) <= 0D Then Return False
                      Return True
                  End Function).
            OrderBy(Function(r) CDate(r("PostingDate"))).
            ThenBy(Function(r) CLng(r("LedgerSequence"))).
            ThenBy(Function(r) CLng(r("LedgerID"))).
            FirstOrDefault()
        End If

        If startLedgerRow Is Nothing Then Exit Sub

        Dim startLedgerID As Long = CLng(startLedgerRow("LedgerID"))

        ' 5) ترتيب المصدر
        Dim orderedSource =
        source.AsEnumerable().
        OrderBy(Function(r) CDate(r("PostingDate"))).
        ThenBy(Function(r) CLng(r("LedgerSequence"))).
        ThenBy(Function(r) CLng(r("LedgerID")))

        ' 6) بناء المحاكاة
        For Each r As DataRow In orderedSource

            Dim row = sim.NewRow()

            ' نسخ الأعمدة
            For Each col As DataColumn In source.Columns
                If sim.Columns.Contains(col.ColumnName) Then
                    row(col.ColumnName) = r(col.ColumnName)
                End If
            Next

            Dim inQty As Decimal = ToDec(r("InQty"))
            Dim outQty As Decimal = ToDec(r("OutQty"))

            If inQty > 0D Then
                row("CorrectInQty") = inQty
                row("CorrectOutQty") = 0D
                row("CorrectInUnitCost") = ToDec(r("InUnitCost"))
                row("CorrectOutUnitCost") = 0D

            ElseIf outQty > 0D Then
                row("CorrectInQty") = 0D
                row("CorrectOutQty") = outQty
                row("CorrectInUnitCost") = 0D
                row("CorrectOutUnitCost") = ToDec(r("OutUnitCost"))

            Else
                row("CorrectInQty") = 0D
                row("CorrectOutQty") = 0D
                row("CorrectInUnitCost") = 0D
                row("CorrectOutUnitCost") = 0D
            End If

            ' تطبيق التصحيح على أول Ledger فقط
            If CLng(r("LedgerID")) = startLedgerID Then
                row("CorrectInQty") = correctedQty
                row("CorrectOutQty") = 0D
                row("CorrectInUnitCost") = correctedUnitCost
            End If

            sim.Rows.Add(row)

        Next

        ' 7) ربط الجروبات
        Dim links As DataTable = _revaluationService.GetSimulationLinks(sim)
        _lastSimulationLinks = links

        If links IsNot Nothing AndAlso links.Rows.Count > 0 Then

            Dim map As New Dictionary(Of Long, (GroupId As String, Seq As Integer))()

            For Each l As DataRow In links.Rows

                Dim groupId As String =
                If(IsDBNull(l("OperationGroupID")), "", l("OperationGroupID").ToString())

                If String.IsNullOrWhiteSpace(groupId) Then Continue For

                Dim seq As Integer = 0
                If Not IsDBNull(l("GroupSeq")) Then seq = CInt(l("GroupSeq"))

                Dim srcId As Long = CLng(l("SourceLedgerID"))
                Dim tgtId As Long = CLng(l("TargetLedgerID"))

                If Not map.ContainsKey(srcId) Then map(srcId) = (groupId, seq)
                If Not map.ContainsKey(tgtId) Then map(tgtId) = (groupId, seq)

            Next

            For Each r As DataRow In sim.Rows

                Dim ledgerId As Long = CLng(r("LedgerID"))

                If map.ContainsKey(ledgerId) Then
                    r("OperationGroupID") = map(ledgerId).GroupId
                    r("GroupSeq") = map(ledgerId).Seq
                End If

            Next

        End If

    End Sub

    Private Sub RunSimulation()

        Dim dt As DataTable = CType(dgvSimulation.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

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

    Private Sub btnPreveiw_Click(sender As Object, e As EventArgs) Handles btnPreveiw.Click

        If dgvAffectedOperations.DataSource Is Nothing Then
            MessageBox.Show("قم بجلب العمليات المتأثرة أولاً")
            Exit Sub
        End If

        RunPreviewSimulationFull()

    End Sub
    Private Sub RunPreviewSimulationFull()

        BuildSimulationTable()

        Dim sim As DataTable = TryCast(dgvSimulation.DataSource, DataTable)
        If sim Is Nothing OrElse sim.Rows.Count = 0 Then Exit Sub

        ' 1) Ensure stable ordering ONCE at start (same instance used afterwards)
        Dim ordered = sim.AsEnumerable().
        OrderBy(Function(r) CDate(r("PostingDate"))).
        ThenBy(Function(r) CLng(r("LedgerSequence"))).
        ThenBy(Function(r) CLng(r("LedgerID"))).
        ToList()

        Dim temp As DataTable = sim.Clone()
        For Each r In ordered
            temp.ImportRow(r)
        Next

        ' Replace datasource and keep working on the SAME instance
        dgvSimulation.DataSource = temp
        sim = temp

        ' 2) Iterate (bounded) to propagate between chains and links
        Dim maxIter As Integer = 10

        For iter As Integer = 1 To maxIter

            ' (A) First compute avg/qty based on current CorrectInUnitCost/Qty
            RunSimulation()

            ' (B) Then apply link rules and update CorrectInUnitCost where needed
            ApplyLinks_TransferAndProduction(sim)

            ' Optional: if you want to debug stability, you can break early
            ' if nothing changes, but we keep it simple for now.
        Next

        ' 3) Final run to reflect last link application in NewAvgCost/NewQty
        RunSimulation()

        _lastSimulationLedger = sim.Copy()

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


    Private Sub ApplyLinks_TransferAndProduction(simLedgers As DataTable)

        If simLedgers Is Nothing OrElse simLedgers.Rows.Count = 0 Then Exit Sub

        Dim links As DataTable = _revaluationService.GetSimulationLinks(simLedgers)
        _lastSimulationLinks = links

        ' Build lookup for ledgers
        Dim ledgerById As Dictionary(Of Long, DataRow) =
        simLedgers.AsEnumerable().
        Where(Function(r) Not IsDBNull(r("LedgerID"))).
        ToDictionary(Function(r) CLng(r("LedgerID")))

        Dim getLedgerRow =
        Function(id As Long) As DataRow
            If ledgerById.ContainsKey(id) Then Return ledgerById(id)
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
        ' 2) منع تعديل أكثر من صف
        If _editedRowIndex <> -1 AndAlso e.RowIndex <> _editedRowIndex Then

            Dim msg As String =
        "لا يمكن تعديل أكثر من صنف في نفس العملية." & vbCrLf &
        "هل تريد تعديل الصف الحالي؟" & vbCrLf &
        "إذا اخترت نعم سيتم إلغاء تعديل الصف السابق وإرجاعه كما كان، وسيتم مسح نتائج العمليات المتأثرة/المحاكاة."

            Dim res = MessageBox.Show(msg, "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If res = DialogResult.Yes Then
                RestoreEditedRowSnapshot()
                ResetSingleRowEditGuard()

                ' مهم: النتائج القديمة لم تعد صالحة لأن الصف المعدّل تغيّر
                ClearDependentResults()

            Else
                e.Cancel = True
                Return
            End If

        End If
        ' 3) أول مرة نلمس صف للتعديل: نسجل أنه هو الصف المعتمد + نلتقط Snapshot قبل أي تغيير
        If _editedRowIndex = -1 Then
            _editedRowIndex = e.RowIndex
        End If

        If _editedRowIndex = e.RowIndex AndAlso _snapshotCapturedForRowIndex <> e.RowIndex Then
            _editedRowSnapshot = CaptureRowSnapshot(e.RowIndex)
            _snapshotCapturedForRowIndex = e.RowIndex
        End If

    End Sub


    Private Sub ResetSingleRowEditGuard()
        _editedRowIndex = -1
        _editedRowSnapshot = Nothing
        _snapshotCapturedForRowIndex = -1
    End Sub


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

    Private Sub RestoreEditedRowSnapshot()
        If _editedRowIndex < 0 Then Return
        If _editedRowSnapshot Is Nothing Then Return
        If _editedRowIndex >= dgvMain.Rows.Count Then Return

        Dim drv As DataRowView = TryCast(dgvMain.Rows(_editedRowIndex).DataBoundItem, DataRowView)
        If drv Is Nothing Then Return

        Dim r As DataRow = drv.Row
        For Each kvp In _editedRowSnapshot
            If r.Table.Columns.Contains(kvp.Key) Then
                r(kvp.Key) = kvp.Value
            End If
        Next

        RefreshAdjustmentDeltaView()
        RefreshDisplayedTotals()
        dgvMain.Refresh()
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
        Dim dtOut As DataTable = data.Outputs.Copy()
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

        If CurrentSubCategoryID = 9 OrElse CurrentSubCategoryID = 10 Then


        End If
        dgvProduced.Refresh()
        ApplyProductionAmountToProducedGrid()
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
    Private Sub ResetProductionValuesForCancel()

        '========================================
        ' 1) تصفير TextBox
        '========================================
        txtProductionAmount.Text = "0"

        '========================================
        ' 2) تصفير dgvProduced (قيم = 0 وليس NULL)
        '========================================
        Dim dtProduced As DataTable = TryCast(dgvProduced.DataSource, DataTable)

        If dtProduced IsNot Nothing Then
            For Each r As DataRow In dtProduced.Rows

                r("Length") = 0D
                r("Width") = 0D
                r("Height") = 0D
                r("Quantity") = 0D
                r("VolumeM3") = 0D

            Next
        End If

        dgvProduced.EndEdit()
        dgvProduced.Refresh()

        '========================================
        ' 3) تصفير مواد الإنتاج (اختياري حسب منطقك)
        '========================================
        Dim dtInputs As DataTable = TryCast(dgvProductionCalculations.DataSource, DataTable)

        If dtInputs IsNot Nothing Then
            For Each r As DataRow In dtInputs.Rows
                r("Quantity") = 0D
                r("TotalCost") = 0D
            Next
        End If

        dgvProductionCalculations.EndEdit()
        dgvProductionCalculations.Refresh()

        '========================================
        ' 4) إعادة الحساب
        '========================================
        RecalculateProductionTotals()

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
            col.ReadOnly = (col.DataPropertyName <> "QtyPieces")

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
                Sum(Function(r) ToDec(r("QtyPieces")))

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
        Sum(Function(x) ToDec(x("QtyPieces")))

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
        Sum(Function(x) ToDec(x("QtyPieces")))

        If totalNewQty > availableQty Then
            MessageBox.Show("الكمية المطلوبة تتجاوز المتاح", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)

            ' رجّع القيمة القديمة (بدون كسر)
            Dim rowEdit As DataRow = dtSource.Rows(e.RowIndex)

            If dtSource.Columns.Contains("OriginalQuantity") Then
                rowEdit("QtyPieces") = ToDec(rowEdit("OriginalQuantity"))
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
        Dim q As Decimal = ToDec(rowCalc("QtyPieces"))

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
                r("QtyPieces") = 0D
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
            Using con As New SqlConnection(ConnStr)
                con.Open()

                '========================================
                ' 1) Header (الرصيد من Inventory_Balance)
                '========================================
                Dim availableQty As Decimal = 0D
                Dim cuttingTransactionID As Integer = 0
                Using cmd As New SqlCommand("
SELECT 
    h.CuttingID,
    h.BaseProductID,
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
                            txtCuttingCode.Text = r("CuttingCode").ToString()
                            txtProductCode.Text = r("BaseProductCode").ToString()

                            availableQty = ToDec(r("AvailableQty"))
                            _currentCutBaseProductID = ToInt(r("BaseProductID"))

                        End If
                    End Using

                End Using
                '========================================
                ' 2) Output (الطلب الحالي)
                '========================================
                Dim dt As New DataTable()

                Using cmd As New SqlCommand("
SELECT 
    o.ProductID,
    p.ProductCode,
    t.TypeName AS ProductType,
    o.Length_cm,
    o.Width_cm,
    o.Height_cm,
    o.QtyPieces,
    o.SourceStoreID,
    s.StoreName,

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
    WHERE td.SourceDocumentDetailID = o.CutOutputID  -- 🔥 الربط الصحيح
    ORDER BY td.DetailID
) td

WHERE o.CutID = @ID
", con)

                    cmd.Parameters.AddWithValue("@ID", cuttingID)

                    dt.Load(cmd.ExecuteReader())

                End Using
                '========================================
                ' 3) حساب كمية الطلب
                '========================================
                Dim requestQty As Decimal =
                    dt.AsEnumerable().
                    Sum(Function(x) ToDec(x("QtyPieces")))

                '========================================
                ' 4) المتاح الحقيقي = الرصيد + الطلب القديم
                '========================================
                txtAvailableQTY.Text =
                    (availableQty + requestQty).ToString("N2")

                '========================================
                ' 5) حساب الأحجام
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
                    Dim q = ToDec(row("QtyPieces"))

                    Dim pieceVolume As Decimal = (l * w * h) / 1000000D
                    Dim totalVolume As Decimal = pieceVolume * q

                    row("PieceVolume") = pieceVolume
                    row("TotalVolume") = totalVolume

                Next

                '========================================
                ' 6) ربط الجريد
                '========================================
                dgvOutPut.DataSource = dt
                If dgvOutPut.Columns.Contains("Length_cm") Then
                    dgvOutPut.Columns("Length_cm").HeaderText = "الطول (سم)"
                End If

                If dgvOutPut.Columns.Contains("Width_cm") Then
                    dgvOutPut.Columns("Width_cm").HeaderText = "العرض (سم)"
                End If

                If dgvOutPut.Columns.Contains("Height_cm") Then
                    dgvOutPut.Columns("Height_cm").HeaderText = "الارتفاع (سم)"
                End If
                ' السماح بالتعديل على QtyPieces
                If dgvOutPut.Columns.Contains("QtyPieces") Then
                    dgvOutPut.Columns("QtyPieces").ReadOnly = False
                End If
                If Not dt.Columns.Contains("OriginalQuantity") Then
                    dt.Columns.Add("OriginalQuantity", GetType(Decimal))
                End If

                For Each row As DataRow In dt.Rows
                    row("OriginalQuantity") = row("QtyPieces")
                Next

                '========================================
                ' 7) تعبئة المنتج
                '========================================
                If dt.Rows.Count > 0 Then
                    If dt.Columns.Contains("StoreName") Then
                        txtSourceStore.Text = dt.Rows(0)("StoreName").ToString()
                    End If
                End If
                '========================================
                ' 8) حفظ المتاح
                '========================================
                txtAvailableQTY.Tag = txtAvailableQTY.Text

            End Using

            '========================================
            ' 9) حساب الإجماليات
            '========================================
            RecalculateCuttingTotals()

        Finally
            IsLoading = False
        End Try

    End Sub

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
End Class