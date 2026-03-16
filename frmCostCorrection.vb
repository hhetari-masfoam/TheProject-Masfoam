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

        Select Case tabMain.SelectedTab.Name

        '================================
        ' PURCHASE
        '================================
            Case "tabPUR"

                Using f As New frmPurchaseSearch()

                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedDocumentID <= 0 Then Exit Sub

                    LoadOriginalDocumentForRevaluation(f.SelectedDocumentID)
                    SetModeUI()

                End Using


        '================================
        ' PRODUCTION
        '================================
            Case "tabPRO"

                Using f As New frmProductionSearch()

                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedProductionID <= 0 Then Exit Sub

                    LoadOriginalDocumentForRevaluation(f.SelectedProductionID)
                    SetModeUI()

                End Using


        '================================
        ' CUT
        '================================
            Case "tabCUT"

                Using f As New frmCuttingSearch()

                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedCuttingID <= 0 Then Exit Sub

                    LoadOriginalDocumentForRevaluation(f.SelectedCuttingID)
                    SetModeUI()

                End Using


        '================================
        ' SCRAP
        '================================
            Case "tabSCR"

                Using f As New frmCuttingWasteCalculatorSearch()

                    If f.ShowDialog() <> DialogResult.OK Then Exit Sub
                    If f.SelectedWasteID <= 0 Then Exit Sub

                    LoadOriginalDocumentForRevaluation(f.SelectedWasteID)
                    SetModeUI()

                End Using

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

        Select Case tabMain.SelectedTab.Name

            Case "tabPurchase"
                CurrentOperationType = "PUR"

            Case "tabProduction"
                CurrentOperationType = "PRO"

            Case "tabCut"
                CurrentOperationType = "CUT"

            Case "tabScrap"
                CurrentOperationType = "SCR"

        End Select

        ' عند تغيير التاب نعيد بناء الجريد السفلي
        RefreshAdjustmentDeltaView()
        If CurrentOperationType = "PUR" Then dgvMain.DataSource = RevalDetailsTable
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
        If Not IsLoading Then
            RefreshAdjustmentDeltaView()
            RefreshDisplayedTotals()
        End If
    End Sub
    Private Sub dgvMain_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvMain.CellValidating
        Dim colName = dgvMain.Columns(e.ColumnIndex).Name
        If colName = "NewQty" OrElse colName = "NewUnitPrice" Then
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
        If IsCancelMode Then
            ' وضع الإلغاء: يصير الجريد فقط للعرض ويصفر الكمية والسعر ثم يعيد الحساب والمجاميع تلقائياً
            dgvMain.ReadOnly = True

            For Each row As DataRow In RevalDetailsTable.Rows
                If row.RowState <> DataRowState.Deleted Then
                    row("NewQty") = 0D
                    row("NewUnitPrice") = 0D
                End If
            Next

            RefreshAdjustmentDeltaView()
            RefreshDisplayedTotals() ' ��تعيد الحسابات لكل صف وكل الاجماليات تلقائياً
            dgvMain.Refresh()
        Else
            dgvMain.ReadOnly = False
            For Each col As DataGridViewColumn In dgvMain.Columns

                Dim prop = col.DataPropertyName

                If prop = "NewQty" OrElse
       prop = "NewUnitPrice" OrElse
       prop = "TaxTypeID" OrElse
       prop = "TaxRate" Then

                    col.ReadOnly = False
                Else
                    col.ReadOnly = True
                End If

            Next

            cboVATRate.Enabled = True
            cboTargetStore.Enabled = False
            txtPartnerName.ReadOnly = True
            txtPhone.ReadOnly = True
            txtSubTotal.ReadOnly = True
            txtVATTotal.ReadOnly = True
            txtGrandTotal.ReadOnly = True
        End If
    End Sub

    Private Sub RefreshDisplayedTotals()
        Dim subTotal As Decimal = 0D
        Dim vatTotal As Decimal = 0D
        Dim grandTotal As Decimal = 0D

        For Each row As DataRow In RevalDetailsTable.Rows
            If row.RowState <> DataRowState.Deleted Then
                ' إعادة الحساب لكل صف بناءً على الكمية والسعر
                Dim qty As Decimal = ToDec(row("NewQty"))
                Dim price As Decimal = ToDec(row("NewUnitPrice"))
                Dim grossAmount As Decimal = qty * price

                Dim discountRate As Decimal = ToDec(row("DiscountRate"))
                Dim discountAmount As Decimal = grossAmount * discountRate / 100D

                Dim taxableAmount As Decimal = grossAmount - discountAmount
                Dim taxRate As Decimal = ToDec(row("TaxRate"))
                Dim taxAmount As Decimal = taxableAmount * taxRate / 100D

                Dim netAmount As Decimal = taxableAmount
                Dim lineTotal As Decimal = netAmount + taxAmount

                row("GrossAmount") = grossAmount
                row("DiscountAmount") = discountAmount
                row("TaxableAmount") = taxableAmount
                row("TaxAmount") = taxAmount
                row("NetAmount") = netAmount
                row("LineTotal") = lineTotal

                subTotal += netAmount
                vatTotal += taxAmount
                grandTotal += lineTotal
            End If
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

        Dim dt As DataTable =
        CType(dgvAffectedOperations.DataSource, DataTable)

        dt.Rows.Clear()

        For Each r As DataRow In source.Rows

            Dim row = dt.NewRow()

            row("LedgerID") = r("LedgerID")
            row("TransactionID") = r("TransactionID")
            row("SourceDetailID") = If(IsDBNull(r("SourceDetailID")), DBNull.Value, r("SourceDetailID"))

            row("ProductID") = r("ProductID")
            row("BaseProductID") = r("BaseProductID")
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
                row("InUnitCost") = r("InUnitCost")
            End If

            row("OutUnitCost") = r("OutUnitCost")
            row("NewAvgCost") = r("NewAvgCost")

            row("LedgerSequence") = r("LedgerSequence")
            row("SourceLedgerID") = r("SourceLedgerID")
            row("RootLedgerID") = r("RootLedgerID")

            row("SupersededByLedgerID") = If(IsDBNull(r("SupersededByLedgerID")), DBNull.Value, r("SupersededByLedgerID"))

            row("RootTransactionID") = r("RootTransactionID")
            row("PostingDate") = r("PostingDate")
            row("BaseProductID") = r("BaseProductID")


            row("RootLedgerID") = If(IsDBNull(r("RootLedgerID")), DBNull.Value, r("RootLedgerID"))

            row("SupersededByLedgerID") = If(IsDBNull(r("SupersededByLedgerID")), DBNull.Value, r("SupersededByLedgerID"))

            row("RootTransactionID") = If(IsDBNull(r("RootTransactionID")), DBNull.Value, r("RootTransactionID"))
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

        If RevalDetailsTable Is Nothing OrElse RevalDetailsTable.Rows.Count = 0 Then
            MessageBox.Show("لا يوجد بيانات لإعادة التقييم.", "تنبيه")
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

        Dim hasPrev As Boolean = source.Columns.Contains("PrevLedgerID")

        Dim startLedgerRow As DataRow =
            source.AsEnumerable().
            Where(Function(r)
                      If IsDBNull(r("SourceDetailID")) Then Return False
                      If ToDec(r("InQty")) <= 0D Then Return False
                      If hasPrev AndAlso Not IsDBNull(r("PrevLedgerID")) Then Return False
                      Return True
                  End Function).
            OrderBy(Function(r) CDate(r("PostingDate"))).
            ThenBy(Function(r) CLng(r("LedgerSequence"))).
            ThenBy(Function(r) CLng(r("LedgerID"))).
            FirstOrDefault()

        If startLedgerRow Is Nothing Then
            startLedgerRow =
                source.AsEnumerable().
                OrderBy(Function(r) CDate(r("PostingDate"))).
                ThenBy(Function(r) CLng(r("LedgerSequence"))).
                ThenBy(Function(r) CLng(r("LedgerID"))).
                First()
        End If

        Dim startLedgerID As Long = CLng(startLedgerRow("LedgerID"))

        Dim correctedQty As Decimal? = Nothing
        Dim correctedUnitCost As Decimal? = Nothing

        If adjust IsNot Nothing AndAlso adjust.Rows.Count > 0 Then
            correctedQty = ToDec(adjust.Rows(0)("NewQty"))
            correctedUnitCost = ToDec(adjust.Rows(0)("NewUnitCost"))
        End If

        Dim orderedSource =
            source.AsEnumerable().
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

            If CLng(r("LedgerID")) = startLedgerID Then
                If correctedQty.HasValue Then
                    row("CorrectInQty") = correctedQty.Value
                    row("CorrectOutQty") = 0D
                End If
                If correctedUnitCost.HasValue Then
                    row("CorrectInUnitCost") = correctedUnitCost.Value
                End If
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
                    r("CorrectInUnitCost") = inUnitCost
                End If
            End If

            ' New qty
            Dim newQty As Decimal = oldQty + inQty - outQty
            Dim localNewQty As Decimal = localOldQty + inQty - outQty

            ' New avg (WAC per ProductID)
            Dim newAvgCost As Decimal = oldAvgCost
            If inQty > 0D Then
                Dim denom As Decimal = oldQty + inQty
                If denom > 0D Then
                    newAvgCost = ((oldQty * oldAvgCost) + (inQty * inUnitCost)) / denom
                Else
                    newAvgCost = inUnitCost
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
    Private Sub ApplyTransferLinksOnly(simLedgers As DataTable)

        If simLedgers Is Nothing OrElse simLedgers.Rows.Count = 0 Then Exit Sub

        Dim links As DataTable = _revaluationService.GetSimulationLinks(simLedgers)
        _lastSimulationLinks = links

        For Each link As DataRow In links.Rows

            Dim linkType As Integer = CInt(link("LinkType"))

            ' TRANSFER only (LinkTypeID=1)
            If linkType <> 1 Then Continue For

            Dim sourceLedgerID As Long = CLng(link("SourceLedgerID"))
            Dim targetLedgerID As Long = CLng(link("TargetLedgerID"))

            Dim sourceRow As DataRow =
            simLedgers.AsEnumerable().
            FirstOrDefault(Function(r) CLng(r("LedgerID")) = sourceLedgerID)

            Dim targetRow As DataRow =
            simLedgers.AsEnumerable().
            FirstOrDefault(Function(r) CLng(r("LedgerID")) = targetLedgerID)

            If sourceRow Is Nothing OrElse targetRow Is Nothing Then Continue For

            ' Rule: In cost = Out cost (transfer)
            Dim sourceOutCost As Decimal = ToDec(sourceRow("CorrectOutUnitCost"))
            If sourceOutCost <= 0D Then
                sourceOutCost = ToDec(sourceRow("OldAvgCost"))
                If sourceOutCost <= 0D Then sourceOutCost = ToDec(sourceRow("NewAvgCost"))
            End If

            If ToDec(targetRow("CorrectInQty")) > 0D Then
                targetRow("CorrectInUnitCost") = sourceOutCost
            End If

        Next

    End Sub

    Private Sub RecalculateLinkedCosts()

        Dim dt As DataTable =
    CType(dgvSimulation.DataSource, DataTable)

        Dim links As DataTable =
    _revaluationService.GetSimulationLinks(dt)

        For Each link As DataRow In links.Rows

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))

            Dim sourceRow =
        dt.AsEnumerable().
        FirstOrDefault(Function(r) CLng(r("LedgerID")) = sourceID)

            Dim targetRow =
        dt.AsEnumerable().
        FirstOrDefault(Function(r) CLng(r("LedgerID")) = targetID)

            If sourceRow Is Nothing OrElse targetRow Is Nothing Then Continue For

            Dim avgCost As Decimal =
        CDec(sourceRow("NewAvgCost"))

            ' إذا كان الهدف حركة دخول
            If CDec(targetRow("CorrectInQty")) > 0 Then

                targetRow("CorrectInUnitCost") = avgCost

            End If

            ' إذا كان الهدف حركة خروج
            If CDec(targetRow("CorrectOutQty")) > 0 Then

                targetRow("CorrectOutUnitCost") = avgCost

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

        ' Helper lambdas to get ledger rows quickly
        Dim ledgerById As Dictionary(Of Long, DataRow) =
            simLedgers.AsEnumerable().ToDictionary(Function(r) CLng(r("LedgerID")))

        Dim getLedgerRow =
            Function(id As Long) As DataRow
                If ledgerById.ContainsKey(id) Then Return ledgerById(id)
                Return Nothing
            End Function

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

            Dim linkType As Integer = CInt(link("LinkType"))
            If linkType <> 1 Then Continue For

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
        ' 2) Build "consume total cost" per TARGET ledger (LinkType=2)
        '    TotalConsumeCost(target) = sum(flowQty * sourceOutUnitCost)
        '=========================================================
        Dim consumeTotalCostByTarget As New Dictionary(Of Long, Decimal)()

        For Each link As DataRow In links.Rows

            Dim linkType As Integer = CInt(link("LinkType"))
            If linkType <> 2 Then Continue For

            Dim sourceID As Long = CLng(link("SourceLedgerID"))
            Dim targetID As Long = CLng(link("TargetLedgerID"))
            Dim flowQty As Decimal = ToDec(link("FlowQty"))

            Dim sourceRow As DataRow = getLedgerRow(sourceID)
            If sourceRow Is Nothing Then Continue For

            Dim cost As Decimal = flowQty * getOutUnitCost(sourceRow)

            If Not consumeTotalCostByTarget.ContainsKey(targetID) Then
                consumeTotalCostByTarget(targetID) = 0D
            End If
            consumeTotalCostByTarget(targetID) += cost

        Next

        '=========================================================
        ' 3) If a TARGET ledger receives consume links and is an IN movement,
        '    set its IN unit cost = totalConsumeCost / its IN qty
        '    (This covers "production: out one, in many" if target is the product ledger itself)
        '=========================================================
        For Each kvp In consumeTotalCostByTarget

            Dim targetID As Long = kvp.Key
            Dim totalCost As Decimal = kvp.Value

            Dim targetRow As DataRow = getLedgerRow(targetID)
            If targetRow Is Nothing Then Continue For

            Dim inQty As Decimal = ToDec(targetRow("CorrectInQty"))
            If inQty > 0D Then
                targetRow("CorrectInUnitCost") = totalCost / inQty
            End If

        Next

        '=========================================================
        ' 4) Outputs distribution (LinkType=3) from a SOURCE to many targets,
        '    excluding SCRAP (LinkType=9) which has zero cost.
        '
        '    We distribute based on FlowQty proportion among LinkType=3 targets.
        '
        '    Cost base:
        '      - If the SOURCE ledger itself has an OUT movement, use its OUT cost.
        '      - Otherwise, if the SOURCE has a "consume total cost" (as a target in step 2),
        '        use that total cost as the pool to distribute.
        '=========================================================
        Dim linksBySource As Dictionary(Of Long, List(Of DataRow)) =
            links.AsEnumerable().
            Where(Function(l) CInt(l("LinkType")) = 3 OrElse CInt(l("LinkType")) = 9).
            GroupBy(Function(l) CLng(l("SourceLedgerID"))).
            ToDictionary(Function(g) g.Key, Function(g) g.ToList())

        For Each src In linksBySource.Keys

            Dim srcRow As DataRow = getLedgerRow(src)

            ' Determine the cost pool to distribute
            Dim poolCost As Decimal = 0D

            If srcRow IsNot Nothing AndAlso ToDec(srcRow("CorrectOutQty")) > 0D Then
                poolCost = ToDec(srcRow("CorrectOutQty")) * getOutUnitCost(srcRow)
            ElseIf consumeTotalCostByTarget.ContainsKey(src) Then
                poolCost = consumeTotalCostByTarget(src)
            ElseIf srcRow IsNot Nothing Then
                ' fallback: if nothing else, use OUT cost * OutQty (could be 0)
                poolCost = ToDec(srcRow("OutQty")) * getOutUnitCost(srcRow)
            End If

            If poolCost <= 0D Then Continue For

            ' Sum FlowQty for LinkType=3 only (exclude scrap type=9)
            Dim sumFlowQty As Decimal = 0D
            For Each l As DataRow In linksBySource(src)
                If CInt(l("LinkType")) = 3 Then
                    sumFlowQty += ToDec(l("FlowQty"))
                End If
            Next
            If sumFlowQty <= 0D Then Continue For

            ' Allocate to each output target of type=3
            For Each l As DataRow In linksBySource(src)

                Dim linkType As Integer = CInt(l("LinkType"))
                Dim targetID As Long = CLng(l("TargetLedgerID"))
                Dim flowQty As Decimal = ToDec(l("FlowQty"))

                If linkType = 9 Then
                    ' SCRAP / trace-only: keep as-is (cost 0)
                    Continue For
                End If

                If linkType <> 3 Then Continue For

                Dim targetRow As DataRow = getLedgerRow(targetID)
                If targetRow Is Nothing Then Continue For

                Dim allocatedCost As Decimal = poolCost * (flowQty / sumFlowQty)

                Dim targetInQty As Decimal = ToDec(targetRow("CorrectInQty"))
                If targetInQty <= 0D Then
                    ' fallback: use flow qty if ledger in qty not available
                    targetInQty = flowQty
                End If

                If targetInQty > 0D Then
                    targetRow("CorrectInUnitCost") = allocatedCost / targetInQty
                End If

            Next

        Next

    End Sub


End Class