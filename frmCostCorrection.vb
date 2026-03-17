Imports System.Data.SqlClient


Public Class frmCostCorrection
    Inherits AABaseOperationForm
    Private IsInventoryPosted As Boolean = False
    Private IsCancelMode As Boolean = False
    Private CurrentOperationType As String = "PUR"
    Private RevalDetailsTable As DataTable
    Private IsLoading As Boolean = False
    Private _revaluationService As CostCorrectionService
    Private _lastSimulationLedger As DataTable
    Private _lastSimulationLinks As DataTable
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



    Private Sub CreateEditableDocumentDetailsTable()
        RevalDetailsTable = New DataTable()

        RevalDetailsTable.Columns.Add("DetailID", GetType(Integer))
        RevalDetailsTable.Columns.Add("ProductID", GetType(Integer))
        RevalDetailsTable.Columns.Add("ProductCode", GetType(String))
        RevalDetailsTable.Columns.Add("ProductName", GetType(String))
        RevalDetailsTable.Columns.Add("ProductTypeID", GetType(String))
        RevalDetailsTable.Columns.Add("UnitID", GetType(String))
        RevalDetailsTable.Columns.Add("OldQty", GetType(Decimal))
        RevalDetailsTable.Columns.Add("NewQty", GetType(Decimal))

        RevalDetailsTable.Columns.Add("SourceStoreID", GetType(Integer))
        RevalDetailsTable.Columns.Add("TargetStoreID", GetType(Integer))

        RevalDetailsTable.Columns.Add("OldUnitPrice", GetType(Decimal))
        RevalDetailsTable.Columns.Add("NewUnitPrice", GetType(Decimal))
        RevalDetailsTable.Columns.Add("GrossAmount", GetType(Decimal))

        RevalDetailsTable.Columns.Add("DiscountRate", GetType(Decimal))
        RevalDetailsTable.Columns.Add("DiscountAmount", GetType(Decimal))

        RevalDetailsTable.Columns.Add("IncludeTax", GetType(Boolean))
        RevalDetailsTable.Columns.Add("TaxRate", GetType(Decimal))
        RevalDetailsTable.Columns.Add("TaxTypeID", GetType(Integer))
        RevalDetailsTable.Columns.Add("TaxableAmount", GetType(Decimal))
        RevalDetailsTable.Columns.Add("TaxAmount", GetType(Decimal))

        RevalDetailsTable.Columns.Add("NetAmount", GetType(Decimal))
        RevalDetailsTable.Columns.Add("LineTotal", GetType(Decimal))

    End Sub




    Protected Sub LoadOriginalDocumentForRevaluation(documentID As Integer)

        If documentID <= 0 Then Exit Sub

        EnterUIGuard()
        Try
            Dim header = _revaluationService.GetDocumentHeaderForRevaluation(documentID)
            BindDocumentHeaderToForm(header)

            RevalDetailsTable =
                _revaluationService.GetDocumentDetailsForRevaluation(documentID)

            dgvMain.DataSource = RevalDetailsTable
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
                    LoadOriginalDocumentForRevaluation(f.SelectedProductionID)
                    SetModeUI()
                End Using

            Case "tabCUT", "tabCut"
                Using f As New frmCuttingSearch()
                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedCuttingID <= 0 Then Exit Sub
                    LoadOriginalDocumentForRevaluation(f.SelectedCuttingID)
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
                LoadOriginalDocumentForRevaluation(CurrentDocumentID)
            End If
            SetModeUI()
        End If
    End Sub

    Private Sub rbtnCancelMode_CheckedChanged(sender As Object, e As EventArgs) Handles rbtnCancelMode.CheckedChanged
        If rbtnCancelMode.Checked Then
            IsCancelMode = True
            If CurrentDocumentID > 0 Then
                LoadOriginalDocumentForRevaluation(CurrentDocumentID)
            End If
            SetModeUI()
        End If
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

            CreateEditableDocumentDetailsTable()
            dgvMain.DataSource = RevalDetailsTable

            LoadPartnerComboBox()
            LoadPaymentMethodCombo()
            LoadPaymentTermCombo()
            LoadTargetStoreCombo()
            LoadVATRateCombo()

            InitializeAdjustmentDeltaGrid()
            InitializeAffectedOperationsGrid()
            InitializeSimulationGrid()

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

        If CurrentOperationType = "PUR" Then
            dgvMain.DataSource = RevalDetailsTable
        End If

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

    Private Sub BuildPurchaseAdjustmentDelta()

        Dim dtResult As DataTable = CType(dgvAdjustResult.DataSource, DataTable)
        dtResult.Rows.Clear()

        For Each r As DataRow In RevalDetailsTable.Rows

            Dim oldQty As Decimal = ToDec(r("OldQty"))
            Dim newQty As Decimal = ToDec(r("NewQty"))

            Dim oldCost As Decimal = ToDec(r("OldUnitPrice"))
            Dim newCost As Decimal = ToDec(r("NewUnitPrice"))

            Dim deltaQty As Decimal = newQty - oldQty
            Dim deltaCost As Decimal = (newQty * newCost) - (oldQty * oldCost)

            If deltaQty <> 0 OrElse deltaCost <> 0 Then

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

    End Sub


    Private Sub RefreshAdjustmentDeltaView()

        Select Case CurrentOperationType

            Case "PUR"
                BuildPurchaseAdjustmentDelta()

            Case "PRO"
            ' لاحقًا

            Case "CUT"
            ' لاحقًا

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
        If RevalDetailsTable Is Nothing Then Exit Sub

        dgvMain.AllowUserToAddRows = False
        dgvMain.AllowUserToDeleteRows = False
        dgvMain.EditMode = DataGridViewEditMode.EditOnEnter

        If IsCancelMode Then
            dgvMain.ReadOnly = True

            For Each row As DataRow In RevalDetailsTable.Rows
                If row.RowState <> DataRowState.Deleted Then
                    row("NewQty") = 0D
                    row("NewUnitPrice") = 0D
                End If
            Next

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
    End Sub

    Private Sub RefreshDisplayedTotals()

        Dim subTotal As Decimal = 0D          ' Net before VAT (after discount)
        Dim vatTotal As Decimal = 0D
        Dim grandTotal As Decimal = 0D

        Dim isTaxInclusive As Boolean = chkIsTaxInclusive.Checked

        For Each row As DataRow In RevalDetailsTable.Rows
            If row.RowState = DataRowState.Deleted Then Continue For

            Dim qty As Decimal = ToDec(row("NewQty"))
            Dim unitPrice As Decimal = ToDec(row("NewUnitPrice"))

            Dim discountRate As Decimal = ToDec(row("DiscountRate"))
            If discountRate < 0D Then discountRate = 0D
            If discountRate > 100D Then discountRate = 100D

            Dim taxRate As Decimal = ToDec(row("TaxRate"))
            If taxRate < 0D Then taxRate = 0D

            Dim includeTaxLine As Boolean = True
            If RevalDetailsTable.Columns.Contains("IncludeTax") Then
                If Not IsDBNull(row("IncludeTax")) Then includeTaxLine = CBool(row("IncludeTax"))
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
                result = _revaluationService.GetAffectedPurchaseOperationsForPreview(
                RevalDetailsTable,
                CurrentDocumentID
            )

            Case "PRO"
                MessageBox.Show("استخراج العمليات المتأثرة للإنتاج لم يُبن بعد.")
                Exit Sub

            Case "CUT"
                MessageBox.Show("استخراج العمليات المتأثرة للقص لم يُبن بعد.")
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

        ' 1) لازم يكون في تعديل (سطر واحد حسب القيد عندكم)
        If adjust Is Nothing OrElse adjust.Rows.Count = 0 Then
            ' لا يوجد تعديل -> لا نبني محاكاة
            Exit Sub
        End If

        Dim correctedProductID As Integer = CInt(adjust.Rows(0)("ProductID"))
        Dim correctedQty As Decimal = ToDec(adjust.Rows(0)("NewQty"))
        Dim correctedUnitCost As Decimal = ToDec(adjust.Rows(0)("NewUnitCost"))

        ' 2) نحاول نلتقط StoreID من تفاصيل المستند (اختياري لتحسين الدقة)
        Dim correctedStoreID As Integer? = Nothing
        If RevalDetailsTable IsNot Nothing Then
            Dim dr = RevalDetailsTable.AsEnumerable().
            FirstOrDefault(Function(r)
                               If IsDBNull(r("ProductID")) Then Return False
                               Return CInt(r("ProductID")) = correctedProductID AndAlso
                                      (ToDec(r("OldQty")) <> ToDec(r("NewQty")) OrElse ToDec(r("OldUnitPrice")) <> ToDec(r("NewUnitPrice")))
                           End Function)

            If dr IsNot Nothing AndAlso RevalDetailsTable.Columns.Contains("SourceStoreID") AndAlso Not IsDBNull(dr("SourceStoreID")) Then
                correctedStoreID = CInt(dr("SourceStoreID"))
            End If
        End If

        ' 3) تحديد Ledger البداية الصحيح: أول IN لنفس Product (ولنفس Store إن توفر)
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

        ' لو ما وجدنا نفس المخزن، نرجع لأول IN لنفس المنتج بدون شرط مخزن
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

        If startLedgerRow Is Nothing Then
            ' لا يوجد IN لهذا المنتج ضمن المتأثرين
            Exit Sub
        End If

        Dim startLedgerID As Long = CLng(startLedgerRow("LedgerID"))

        ' 4) ترتيب المصدر
        Dim orderedSource =
        source.AsEnumerable().
        OrderBy(Function(r) CDate(r("PostingDate"))).
        ThenBy(Function(r) CLng(r("LedgerSequence"))).
        ThenBy(Function(r) CLng(r("LedgerID")))

        ' 5) بناء جدول المحاكاة: نسخ + إعداد Correct* + تطبيق تصحيح المستخدم فقط على startLedgerID
        For Each r As DataRow In orderedSource

            Dim row = sim.NewRow()

            ' نسخ الأعمدة المشتركة
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

            ' تطبيق تصحيح المستخدم على Ledger البداية فقط
            If CLng(r("LedgerID")) = startLedgerID Then
                row("CorrectInQty") = correctedQty
                row("CorrectOutQty") = 0D
                row("CorrectInUnitCost") = correctedUnitCost
                ' CorrectOutUnitCost يبقى 0 لأنه IN
            End If

            sim.Rows.Add(row)

        Next

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
            Dim inUnitCost As Decimal = 0D
            If inQty > 0D Then
                Dim specified As Decimal = ToDec(r("CorrectInUnitCost"))
                If specified > 0D Then
                    inUnitCost = specified
                Else
                    inUnitCost = oldAvgCost
                    inUnitCost = 0D
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

        Dim sim As DataTable = CType(dgvSimulation.DataSource, DataTable)
        If sim Is Nothing OrElse sim.Rows.Count = 0 Then Exit Sub

        ' Ensure stable ordering
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

        ' Iterate (bounded) to propagate between chains and links
        Dim maxIter As Integer = 10

        For iter As Integer = 1 To maxIter
            RunSimulation()
            ApplyLinks_TransferAndProduction(sim)
        Next

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

        ' Map ledgers by ID for fast lookup
        Dim ledgerById As Dictionary(Of Long, DataRow) =
        simLedgers.AsEnumerable().ToDictionary(Function(r) CLng(r("LedgerID")))

        Dim getLedgerRow =
        Function(id As Long) As DataRow
            If ledgerById.ContainsKey(id) Then Return ledgerById(id)
            Return Nothing
        End Function

        ' Stable unit-cost for SOURCE when propagating through links:
        ' Prefer CorrectOutUnitCost, else OutUnitCost, else OldAvgCost.
        ' DO NOT use NewAvgCost as fallback (may be temporary during iterations).
        Dim getSourceOutUnitCost =
        Function(row As DataRow) As Decimal
            If row Is Nothing Then Return 0D

            Dim c As Decimal = ToDec(row("CorrectOutUnitCost"))
            If c > 0D Then Return c

            c = ToDec(row("OutUnitCost"))
            If c > 0D Then Return c

            c = ToDec(row("OldAvgCost"))
            If c > 0D Then Return c

            Return 0D
        End Function

        '=========================================================
        ' 1) TRANSFER (LinkType=1)
        '=========================================================
        For Each link As DataRow In links.Rows

            If CInt(link("LinkType")) <> 1 Then Continue For

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            Dim targetRow As DataRow = getLedgerRow(targetID)
            If sourceRow Is Nothing OrElse targetRow Is Nothing Then Continue For

            Dim srcCost As Decimal = getSourceOutUnitCost(sourceRow)
            If srcCost <= 0D Then Continue For

            If ToDec(targetRow("CorrectInQty")) > 0D Then
                targetRow("CorrectInUnitCost") = srcCost
            End If

        Next

        '=========================================================
        ' 2) PRODUCTION CONSUME (LinkType=2): many OUT raw -> one IN final
        '    Target IN unit cost = Sum(flowQty * sourceOutUnitCost) / targetInQty
        '=========================================================
        Dim prodTotalCostByTarget As New Dictionary(Of Long, Decimal)()

        For Each link As DataRow In links.Rows

            If CInt(link("LinkType")) <> 2 Then Continue For

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))
            Dim flowQty As Decimal = ToDec(link("FlowQty"))

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            If sourceRow Is Nothing Then Continue For

            Dim srcCost As Decimal = getSourceOutUnitCost(sourceRow)
            If srcCost <= 0D Then Continue For

            Dim cost As Decimal = flowQty * srcCost

            If Not prodTotalCostByTarget.ContainsKey(targetID) Then
                prodTotalCostByTarget(targetID) = 0D
            End If
            prodTotalCostByTarget(targetID) += cost

        Next

        For Each kvp In prodTotalCostByTarget

            Dim targetID As Long = kvp.Key
            Dim totalCost As Decimal = kvp.Value

            Dim targetRow As DataRow = getLedgerRow(targetID)
            If targetRow Is Nothing Then Continue For

            Dim inQty As Decimal = ToDec(targetRow("CorrectInQty"))
            If inQty <= 0D Then inQty = ToDec(targetRow("InQty"))

            If inQty > 0D Then
                targetRow("CorrectInUnitCost") = totalCost / inQty
            End If

        Next

        '=========================================================
        ' 3) MULTI-OUTPUT DISTRIBUTION (LinkType=3) + SCRAP (LinkType=9)
        '    One SOURCE -> many targets (e.g., Cutting, Waste->Scrap).
        '    Scrap targets (9) = cost 0, but included as movement qty.
        '
        '    poolCost = sourceOutQty * sourceOutUnitCost
        '    distribute poolCost across LinkType=3 targets by FlowQty proportion.
        '=========================================================
        Dim outputsBySource As Dictionary(Of Long, List(Of DataRow)) =
        links.AsEnumerable().
        Where(Function(l) CInt(l("LinkType")) = 3 OrElse CInt(l("LinkType")) = 9).
        GroupBy(Function(l) CLng(l("SourceLedgerID"))).
        ToDictionary(Function(g) g.Key, Function(g) g.ToList())

        For Each sourceID In outputsBySource.Keys

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            If sourceRow Is Nothing Then Continue For

            Dim outQty As Decimal = ToDec(sourceRow("CorrectOutQty"))
            If outQty <= 0D Then outQty = ToDec(sourceRow("OutQty"))

            Dim outCost As Decimal = getSourceOutUnitCost(sourceRow)

            Dim poolCost As Decimal = outQty * outCost
            If poolCost <= 0D Then Continue For

            ' Sum FlowQty for LinkType=3 only (exclude scrap 9 from cost distribution)
            Dim sumFlowQty As Decimal = 0D
            For Each l As DataRow In outputsBySource(sourceID)
                If CInt(l("LinkType")) = 3 Then
                    sumFlowQty += ToDec(l("FlowQty"))
                End If
            Next
            If sumFlowQty <= 0D Then Continue For

            For Each l As DataRow In outputsBySource(sourceID)

                Dim linkType As Integer = CInt(l("LinkType"))
                Dim targetID As Long = CLng(l("TargetLedgerID"))

                Dim targetRow As DataRow = getLedgerRow(targetID)
                If targetRow Is Nothing Then Continue For

                If linkType = 9 Then
                    ' Scrap: force zero cost if it's an IN movement
                    If ToDec(targetRow("CorrectInQty")) > 0D Then
                        targetRow("CorrectInUnitCost") = 0D
                    End If
                    Continue For
                End If

                If linkType <> 3 Then Continue For

                Dim flowQty As Decimal = ToDec(l("FlowQty"))
                Dim allocatedCost As Decimal = poolCost * (flowQty / sumFlowQty)

                Dim targetInQty As Decimal = ToDec(targetRow("CorrectInQty"))
                If targetInQty <= 0D Then targetInQty = ToDec(targetRow("InQty"))
                If targetInQty <= 0D Then targetInQty = flowQty

                If targetInQty > 0D Then
                    targetRow("CorrectInUnitCost") = allocatedCost / targetInQty
                End If

            Next

        Next

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

End Class