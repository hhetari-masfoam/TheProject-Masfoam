Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class frmPurchases
    Private _isInternalChange As Boolean = False
    ' =========================
    ' دالة تحميل الكمبو الخاصة بالمشتريات (الموردين فقط)
    Protected CurrentUserID As Integer = 1
    Protected IsSaved As Boolean = False
    Protected InvoiceDetailsTable As DataTable
    Protected CurrentMode As FormMode = FormMode.NewMode
    ' =========================
    ' Document State
    ' =========================
    ' =========================
    ' Document Constants
    ' =========================
    Private _suspendDueSync As Boolean = False
    ' =========================
    ' Inventory Posting State
    ' =========================
    Protected IsInventoryPosted As Boolean = False
    ' وضع تعديل سند مرحل
    Private IsPostedEditMode As Boolean = False

    ' لتتبع السطور المحذوفة
    Private DeletedDetailIDs As New List(Of Integer)
    Private OriginalDocumentID As Integer = 0
    Private OriginalDetailsTable As DataTable
    Private _allProducts As DataTable
    Private _allProductTypes As DataTable
    Private _allUnits As DataTable
    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "PUR"
        End Get
    End Property
    Public Enum CancelActionType
        None
        Delete
        Zero
    End Enum
    Protected Enum FormMode
        NewMode
        ViewMode
    End Enum

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
    p.PartnerTypeID,
   a.City
FROM md.Partner p
LEFT JOIN md.PartnerAddress a 
    ON a.PartnerID = p.PartnerID
   AND a.IsDefault = 1
WHERE p.IsActive = 1
  AND p.PartnerTypeID =2 
ORDER BY p.PartnerName
"
                Using cmd As New SqlCommand(query, con)
                    con.Open()
                    dt.Load(cmd.ExecuteReader())
                End Using
            End Using

            ' 🔴 الترتيب الصحيح (مهم)
            cboPartnerCode.DisplayMember = "PartnerName"
            cboPartnerCode.ValueMember = "PartnerID"
            cboPartnerCode.DataSource = dt
            cboPartnerCode.SelectedIndex = -1

            cboPartnerCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            cboPartnerCode.AutoCompleteSource = AutoCompleteSource.ListItems

        Catch ex As Exception
            MessageBox.Show("خطأ في تحميل قائمة الموردين: " & ex.Message)
        End Try
    End Sub
    Private Function ToDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim s = v.ToString().Trim()
        If s = "" Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(s, d) Then Return d
        Return 0D
    End Function
    Private Function ToBool(v As Object) As Boolean
        If v Is Nothing OrElse IsDBNull(v) Then Return False
        If TypeOf v Is Boolean Then Return CBool(v)
        Dim s = v.ToString().Trim().ToLower()
        Return (s = "true" OrElse s = "1" OrElse s = "yes")
    End Function


    Private Sub LoadUnits()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    UnitID,
    UnitName
FROM md.Unit
WHERE IsActive = 1
", con)

                con.Open()
                _allUnits = New DataTable()
                _allUnits.Load(cmd.ExecuteReader())

            End Using
        End Using

    End Sub

    Private Sub frmPurchases_Load(
    sender As Object,
    e As EventArgs
) Handles Me.Load

        If IsLoading Then Return
        IsLoading = True

        Try
            dgvInvoiceDetails.AutoGenerateColumns = False
            dgvInvoiceDetails.EditMode = DataGridViewEditMode.EditOnEnter

    
            InitInvoiceDetailsTable()
            LoadProductTypesForGrid()
            dgvInvoiceDetails.DataSource = InvoiceDetailsTable
            LoadProductsForGrid()
            LoadProductTypeFilterCombo()
            LoadUnits()
            RemoveHandler InvoiceDetailsTable.RowChanged, AddressOf InvoiceDetailsTable_RowChanged
            AddHandler InvoiceDetailsTable.RowChanged, AddressOf InvoiceDetailsTable_RowChanged

            LoadPartnerComboBox()
            LoadPaymentMethodCombo()
            LoadPaymentTermCombo()
            LoadTargetStoreCombo()
            LoadVATRateCombo()
            FormatInvoiceGrid(dgvInvoiceDetails)

            If cboVATRate.DataSource IsNot Nothing Then
                cboVATRate.SelectedValue = 1
            End If

            colProductCode.DataPropertyName = "ProductID"
            colProductID.DataPropertyName = "ProductID"
            colProductName.DataPropertyName = "ProductName"
            colProductType.DataPropertyName = "ProductTypeID"

            colUnitID.DataPropertyName = "UnitName"
            colQty.DataPropertyName = "Quantity"
            colUnitPrice.DataPropertyName = "UnitPrice"
            colVATRate.DataPropertyName = "TaxRate"
            colTaxableAmount.DataPropertyName = "TaxableAmount"
            colVATAmount.DataPropertyName = "TaxAmount"
            colTotalAmount.DataPropertyName = "LineTotal"
            colProductType.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            Dim colCode = CType(dgvInvoiceDetails.Columns("colProductCode"), DataGridViewComboBoxColumn)
            colCode.DisplayMember = "ProductCode"
            colCode.ValueMember = "ProductID"

            Dim colType = CType(dgvInvoiceDetails.Columns("colProductType"), DataGridViewComboBoxColumn)
            colType.DisplayMember = "TypeName"
            colType.ValueMember = "ProductTypeID"
            OpenNewMode()

        Finally
            IsLoading = False
        End Try


        '       ResolveFormOperationType()
        '      Dim colCode = CType(dgvInvoiceDetails.Columns("colProductCode"), DataGridViewComboBoxColumn)
        '     Dim colType = CType(dgvInvoiceDetails.Columns("colProductType"), DataGridViewComboBoxColumn)


    End Sub

    Protected Sub ApplyHeaderVATToGrid()

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit Then Exit Sub

        EnterUIGuard()
        Try
            Dim TaxRate As Decimal = GetSelectedVATRate()
            Dim includeTax As Boolean = chkIsTaxInclusive.Checked

            For Each r As DataRow In InvoiceDetailsTable.Rows
                If r.RowState = DataRowState.Deleted Then Continue For
                r("TaxRate") = TaxRate
                r("IncludeTax") = includeTax
            Next
        Finally
            ExitUIGuard()   ' ⬅️ مهم جداً
        End Try

        ' 2️⃣ الآن الحساب مسموح
        RecalculatePreview(PreviewRecalcScope.AllRows)

        ' 3️⃣ تحديث العرض
        dgvInvoiceDetails.Refresh()

    End Sub
    Private Sub dgvInvoiceDetails_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        ' التأكد أن الضغط على زر البحث
        If dgvInvoiceDetails.Columns(e.ColumnIndex).Name <> "colProductSearch" Then Exit Sub

        Using f As New frmProductSearch()

            ' تمرير فلترة النوع
            If cboProductFilter.SelectedValue IsNot Nothing _
           AndAlso IsNumeric(cboProductFilter.SelectedValue) _
           AndAlso CInt(cboProductFilter.SelectedValue) > 0 Then

                f.SearchFilter = frmProductSearch.ProductSearchFilter.ByType
                f.FilterValueID = CInt(cboProductFilter.SelectedValue)

            Else
                f.SearchFilter = frmProductSearch.ProductSearchFilter.None
                f.FilterValueID = 0
            End If

            If f.ShowDialog() <> DialogResult.OK Then Exit Sub

            Dim row = dgvInvoiceDetails.Rows(e.RowIndex)

            ' =========================
            ' (1) تعيين النوع أولاً
            ' =========================
            EnterUIGuard()
            Try
                row.Cells("colProductType").Value = f.SelectedProductTypeID
            Finally
                ExitUIGuard()
            End Try

            ' =========================
            ' (2) التأكد أن الكود موجود في الكمبوا
            ' =========================
            Dim comboCell = CType(row.Cells("colProductCode"), DataGridViewComboBoxCell)


            ' =========================
            ' (3) تعيين الكود
            ' =========================
            dgvInvoiceDetails.CurrentCell = row.Cells("colProductCode")
            row.Cells("colProductCode").Value = f.SelectedProductCode

            ' تثبيت التغيير
            dgvInvoiceDetails.EndEdit()
            dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

            ' =========================
            ' (5) حساب السطر
            ' =========================
            RecalculatePreview(PreviewRecalcScope.RowOnly, e.RowIndex)

        End Using

    End Sub
    Private Sub LoadProductTypeFilterCombo()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                ProductTypeID,
                TypeName
            FROM md.ProductType
            WHERE IsActive = 1
            ORDER BY TypeName
        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        ' صف (الكل)
        Dim allRow As DataRow = dt.NewRow()
        allRow("ProductTypeID") = 0
        allRow("TypeName") = "كل الأنواع"
        dt.Rows.InsertAt(allRow, 0)

        cboProductFilter.DataSource = dt
        cboProductFilter.DisplayMember = "TypeName"
        cboProductFilter.ValueMember = "ProductTypeID"
        cboProductFilter.SelectedIndex = 0

    End Sub

    Private Sub cboVATRate_SelectionChangeCommitted(
    sender As Object,
    e As EventArgs
) Handles cboVATRate.SelectionChangeCommitted

        If IsUIGuarded Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit Then Exit Sub
        ' 1️⃣ تحديث السطور فقط (مع Guard)
        EnterUIGuard()
        Try
            Dim TaxRate As Decimal = GetSelectedVATRate()

            For Each r As DataRow In InvoiceDetailsTable.Rows
                If r.RowState = DataRowState.Deleted Then Continue For
                r("TaxRate") = TaxRate
            Next

        Finally
            ExitUIGuard()
        End Try

        ' 2️⃣ الحساب خارج الحارس
        RecalculatePreview(PreviewRecalcScope.AllRows)

        ' 3️⃣ تحديث العرض
        dgvInvoiceDetails.Refresh()

    End Sub

    Private Sub InvoiceDetailsTable_RowChanged(
    sender As Object,
    e As DataRowChangeEventArgs
)
        If e.Action <> DataRowAction.Add Then Exit Sub
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit Then Exit Sub



        EnterUIGuard()
        Try

            ' =========================
            ' VAT الافتراضي من الهيدر
            ' =========================
            Dim TaxRate As Decimal = GetSelectedVATRate()
            If IsDBNull(e.Row("TaxRate")) OrElse ToDec(e.Row("TaxRate")) = 0D Then
                e.Row("TaxRate") = TaxRate

            End If
            ' =========================
            ' شامل الضريبة من الهيدر
            ' =========================
            e.Row("IncludeTax") = chkIsTaxInclusive.Checked
            e.Row("TaxTypeID") = CInt(cboVATRate.SelectedValue)
            ' =========================
            ' المخازن (الحل هنا)
            ' =========================
            If cboTargetStore.SelectedValue IsNot Nothing _
           AndAlso IsNumeric(cboTargetStore.SelectedValue) Then

                e.Row("TargetStoreID") = CInt(cboTargetStore.SelectedValue)
                e.Row("SourceStoreID") = DBNull.Value   ' مشتريات دائمًا

            End If

        Finally
            ExitUIGuard()
        End Try

    End Sub

    Protected Function GetSelectedVATRate() As Decimal

        If cboVATRate.SelectedValue Is Nothing Then
            Return 0D
        End If

        Dim vatID As Integer

        ' ✅ معالجة DataRowView
        If TypeOf cboVATRate.SelectedValue Is DataRowView Then
            vatID = CInt(CType(cboVATRate.SelectedValue, DataRowView)("ID"))
        ElseIf IsNumeric(cboVATRate.SelectedValue) Then
            vatID = CInt(cboVATRate.SelectedValue)
        Else
            Return 0D
        End If

        Dim result = ExecuteScalarValue(
        "
        SELECT TaxRate
        FROM md.TaxType
        WHERE TaxTypeID = @ID
        ",
        Sub(cmd)
            cmd.Parameters.AddWithValue("@ID", vatID)
        End Sub
    )

        If result Is Nothing OrElse IsDBNull(result) Then
            Return 0D
        End If

        Return CDec(result)

    End Function


    Protected Sub OpenNewMode()

        IsLoading = True
        EnterUIGuard()
        Try


            ' الحالة من المصدر المركزي
            RefreshFormStatus(0)
            ' =========================
            ' فتح الفورم للإدخال
            ' =========================
            dgvInvoiceDetails.Enabled = True
            dgvInvoiceDetails.ReadOnly = False
            dgvInvoiceDetails.AllowUserToAddRows = True
            dgvInvoiceDetails.AllowUserToDeleteRows = True

            btnSend.Enabled = True

            dtpDocumentDate.Enabled = True
            cboPartnerCode.Enabled = True
            cboPaymentMethod.Enabled = True
            cboPaymentTerm.Enabled = True
            cboTargetStore.Enabled = True
            txtNote.ReadOnly = False

            ' =========================
            ' الحالة من المصدر المركزي
            ' =========================
            RefreshFormStatus(0)

            ' =========================
            ' تطبيق الصلاحيات حسب الحالة
            ' =========================
            ApplyEditPermissionByStatus()

        Finally
            ExitUIGuard()
            IsLoading = False
        End Try

    End Sub
    Private Sub dgvInvoiceDetails_CellBeginEdit(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvInvoiceDetails.CellBeginEdit

        ' ⛔ ممنوع التعديل بعد الترحيل مهما كانت الحالة

        ' ⛔ لا نثق بالكمبو


    End Sub

    Private Sub dgvInvoiceDetails_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellValueChanged

        If _isInternalChange Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim colName As String = dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        Try
            _isInternalChange = True

            Select Case colName

                Case "colProductCode"

                    Dim codeObj = row.Cells("colProductCode").Value
                    If codeObj Is Nothing OrElse IsDBNull(codeObj) Then Exit Sub

                    Dim selectedCodeProductID As Integer = CInt(codeObj)

                    Dim drv As DataRowView = CType(row.DataBoundItem, DataRowView)

                    drv("ProductID") = DBNull.Value
                    drv("ProductTypeID") = DBNull.Value
                    drv("ProductName") = ""
                    drv("UnitID") = DBNull.Value
                    drv("UnitName") = ""

                    Dim typesTable = GetTypesByProductCode(selectedCodeProductID)
                    If typesTable Is Nothing Then Exit Sub

                    Dim typeIds = typesTable.AsEnumerable().
                    Select(Function(r) r.Field(Of Integer)("ProductTypeID")).
                    ToList()

                    If typeIds.Count = 0 Then Exit Sub

                    Dim combo = CType(row.Cells("colProductType"), DataGridViewComboBoxCell)
                    Dim view As New DataView(_allProductTypes)

                    Dim filter As String =
                    String.Join(" OR ", typeIds.Select(Function(id) "ProductTypeID = " & id))

                    view.RowFilter = filter

                    combo.DataSource = view
                    combo.DisplayMember = "TypeName"
                    combo.ValueMember = "ProductTypeID"

                    row.Cells("colProductType").Value = DBNull.Value

                    If typeIds.Count = 1 Then
                        Dim singleTypeID As Integer = typeIds(0)
                        row.Cells("colProductType").Value = singleTypeID

                        ApplyProductSelection(row, selectedCodeProductID, singleTypeID, e.RowIndex)
                    Else
                        dgvInvoiceDetails.CurrentCell = row.Cells("colProductType")
                    End If

                Case "colProductType"

                    Dim codeObj = row.Cells("colProductCode").Value
                    Dim typeObj = row.Cells("colProductType").Value

                    If codeObj Is Nothing OrElse IsDBNull(codeObj) Then Exit Sub
                    If typeObj Is Nothing OrElse IsDBNull(typeObj) Then Exit Sub

                    Dim selectedCodeProductID As Integer = CInt(codeObj)
                    Dim typeID As Integer = CInt(typeObj)

                    ApplyProductSelection(row, selectedCodeProductID, typeID, e.RowIndex)

                Case "colQty", "colUnitPrice", "colVATRate"

                    dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

                    RecalculatePreview(
                    PreviewRecalcScope.RowOnly,
                    e.RowIndex
                )

            End Select

        Finally
            _isInternalChange = False
        End Try

    End Sub
    Private Sub ApplyProductSelection(row As DataGridViewRow, codeProductID As Integer, typeID As Integer, rowIndex As Integer)

        If row Is Nothing OrElse row.IsNewRow Then Exit Sub
        If rowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim codeRow = _allProducts.AsEnumerable().
        FirstOrDefault(Function(r) CInt(r("ProductID")) = codeProductID)

        If codeRow Is Nothing Then Exit Sub

        Dim productCode As String = codeRow("ProductCode").ToString()

        Dim selected = _allProducts.AsEnumerable().
        FirstOrDefault(Function(r) _
            r("ProductCode").ToString() = productCode AndAlso
            CInt(r("ProductTypeID")) = typeID)

        If selected Is Nothing Then Exit Sub

        Dim finalProductID As Integer = CInt(selected("ProductID"))

        If IsDuplicateProduct(finalProductID, rowIndex) Then
            MessageBox.Show("الصنف مكرر")
            row.Cells("colProductType").Value = DBNull.Value
            dgvInvoiceDetails.CurrentCell = row.Cells("colProductType")
            Exit Sub
        End If

        Dim drv As DataRowView = CType(row.DataBoundItem, DataRowView)

        drv("ProductID") = finalProductID
        drv("ProductTypeID") = typeID
        drv("ProductName") = selected("ProductName")

        Dim unitID As Integer = CInt(selected("StorageUnitID"))
        drv("UnitID") = unitID

        Dim unitRow = _allUnits.AsEnumerable().
        FirstOrDefault(Function(u) CInt(u("UnitID")) = unitID)

        If unitRow IsNot Nothing Then
            drv("UnitName") = unitRow("UnitName").ToString()
        Else
            drv("UnitName") = ""
        End If

        drv.EndEdit()

        RecalculatePreview(PreviewRecalcScope.RowOnly, rowIndex)

        dgvInvoiceDetails.CurrentCell = row.Cells("colQty")

    End Sub



    Protected Sub NormalizeInvoiceGrid()

        For i As Integer = InvoiceDetailsTable.Rows.Count - 1 To 0 Step -1

            Dim r As DataRow = InvoiceDetailsTable.Rows(i)

            If r.RowState = DataRowState.Deleted Then Continue For

            ' ❌ حذف إذا الصنف غير صالح
            If IsDBNull(r("ProductID")) OrElse CInt(r("ProductID")) <= 0 Then
                InvoiceDetailsTable.Rows.RemoveAt(i)
                Continue For
            End If

            ' 🔥 الكمية صفر
            If ToDec(r("Quantity")) <= 0D Then

                If IsPostedEditMode Then
                    ' ✅ لا نحذف → نخليه صفر (إلغاء)
                    r("Quantity") = 0D

                Else
                    ' 🟢 قبل الترحيل → نحذف
                    InvoiceDetailsTable.Rows.RemoveAt(i)
                End If

            End If

        Next

    End Sub
    Protected Sub LoadVATRateCombo()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                TaxTypeID,
                TaxName,
                TaxRate
            FROM md.TaxType
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


    ' ========================================
    ' تعديل InitInvoiceDetailsTable
    ' ========================================
    Protected Sub InitInvoiceDetailsTable()

        InvoiceDetailsTable = New DataTable()

        ' معلومات الصنف
        InvoiceDetailsTable.Columns.Add("DetailID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductCode", GetType(String))
        InvoiceDetailsTable.Columns.Add("ProductTypeID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductName", GetType(String))
        InvoiceDetailsTable.Columns.Add("UnitName", GetType(String))
        ' الكميات
        InvoiceDetailsTable.Columns.Add("Quantity", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("UnitID", GetType(Integer))

        ' المخازن
        InvoiceDetailsTable.Columns.Add("SourceStoreID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("TargetStoreID", GetType(Integer))

        ' الأسعار
        InvoiceDetailsTable.Columns.Add("UnitPrice", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("GrossAmount", GetType(Decimal))

        ' الخصم
        InvoiceDetailsTable.Columns.Add("DiscountRate", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("DiscountAmount", GetType(Decimal))

        ' الضريبة
        InvoiceDetailsTable.Columns.Add("IncludeTax", GetType(Boolean))
        InvoiceDetailsTable.Columns.Add("TaxRate", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("TaxTypeID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("TaxableAmount", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("TaxAmount", GetType(Decimal))

        ' المجاميع
        InvoiceDetailsTable.Columns.Add("NetAmount", GetType(Decimal))
        InvoiceDetailsTable.Columns.Add("LineTotal", GetType(Decimal))

    End Sub
    ' =========================
    ' frmPurchases
    ' =========================
    Protected Sub ApplyEditPermissionByStatus()
        Dim statusID = GetDocumentStatusID(CurrentDocumentID)

        If statusID = 6 AndAlso Not IsPostedEditMode Then

            dgvInvoiceDetails.ReadOnly = True
            dgvInvoiceDetails.Enabled = False

            btnSaveDraft.Enabled = False
            btnCancel.Enabled = False

            Exit Sub
        End If
        ' 🔥 أهم شرط
        If IsPostedEditMode Then
            dgvInvoiceDetails.ReadOnly = False
            dgvInvoiceDetails.Enabled = True

            btnSaveDraft.Enabled = True
            btnCancel.Enabled = True

            Exit Sub

        End If

        ' =========================
        ' الوضع الطبيعي
        ' =========================
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        Select Case mode

            Case EditModeType.DirectEdit

                dgvInvoiceDetails.ReadOnly = False
                dgvInvoiceDetails.Enabled = True

                btnSaveDraft.Enabled = True
                btnCancel.Enabled = True

            Case EditModeType.EngineEdit

                dgvInvoiceDetails.ReadOnly = False
                dgvInvoiceDetails.Enabled = True

                btnSaveDraft.Enabled = True
                btnCancel.Enabled = True

            Case EditModeType.NoEdit

                dgvInvoiceDetails.ReadOnly = True
                dgvInvoiceDetails.Enabled = False

                btnSaveDraft.Enabled = False
                btnCancel.Enabled = False

        End Select

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
    Protected Function ValidateDocument() As Boolean

        ' =========================
        ' 1️⃣ Payment Term
        ' =========================
        If cboPaymentTerm.SelectedIndex = -1 Then
            MessageBox.Show("يجب اختيار شرط الدفع")
            cboPaymentTerm.Focus()
            Return False
        End If

        ' =========================
        ' 2️⃣ Payment Method
        ' =========================
        If cboPaymentMethod.SelectedIndex = -1 Then
            MessageBox.Show("يجب اختيار طريقة الدفع")
            cboPaymentMethod.Focus()
            Return False
        End If

        ' =========================
        ' 3️⃣ VAT Rate
        ' =========================
        If cboVATRate.SelectedIndex = -1 Then
            MessageBox.Show("يجب اختيار نسبة الضريبة")
            cboVATRate.Focus()
            Return False
        End If

        ' =========================
        ' 4️⃣ Supplier (Partner)
        ' =========================
        If cboPartnerCode.SelectedIndex = -1 Then
            MessageBox.Show("يجب اختيار المورد")
            cboPartnerCode.Focus()
            Return False
        End If

        ' =========================
        ' 5️⃣ Target Store
        ' =========================
        If cboTargetStore.SelectedIndex = -1 Then
            MessageBox.Show("يجب اختيار المستودع")
            cboTargetStore.Focus()
            Return False
        End If

        ' =========================
        ' 6️⃣ Sub Total
        ' =========================
        If Not IsPostedEditMode Then

            If String.IsNullOrWhiteSpace(txtSubTotal.Text) _
       OrElse Val(txtSubTotal.Text) <= 0 Then
                MessageBox.Show("إجمالي الأصناف غير صحيح")
                txtSubTotal.Focus()
                Return False
            End If
        End If

        ' =========================
        ' 7️⃣ VAT Total
        ' =========================
        If Not IsPostedEditMode Then

            If String.IsNullOrWhiteSpace(txtVATTotal.Text) _
       OrElse Val(txtVATTotal.Text) < 0 Then
                MessageBox.Show("قيمة الضريبة غير صحيحة")
                txtVATTotal.Focus()
                Return False
            End If
        End If

        ' =========================
        ' 8️⃣ Grand Total
        ' =========================
        If Not IsPostedEditMode Then

            If String.IsNullOrWhiteSpace(txtGrandTotal.Text) _
       OrElse Val(txtGrandTotal.Text) <= 0 Then
                MessageBox.Show("الإجمالي النهائي غير صحيح")
                txtGrandTotal.Focus()
                Return False
            End If
        End If

        ' =========================
        ' 9️⃣ Grid Details Validation
        ' =========================
        If InvoiceDetailsTable Is Nothing _
       OrElse InvoiceDetailsTable.Rows.Count = 0 Then
            MessageBox.Show("لا توجد أصناف في الفاتورة")
            Return False
        End If

        For Each r As DataRow In InvoiceDetailsTable.Rows

            If IsDBNull(r("ProductID")) OrElse Val(r("ProductID")) <= 0 Then
                MessageBox.Show("يوجد صنف غير محدد في التفاصيل")
                Return False
            End If

            If Not IsPostedEditMode Then

                If IsDBNull(r("Quantity")) OrElse Val(r("Quantity")) <= 0 Then
                    MessageBox.Show("الكمية يجب أن تكون أكبر من صفر")
                    Return False
                End If

            End If
            If IsDBNull(r("UnitPrice")) OrElse Val(r("UnitPrice")) < 0 Then
                MessageBox.Show("سعر الوحدة غير صحيح")
                Return False
            End If

        Next

        Return True

    End Function

    Protected Sub LoadPreviousDocumentNumbers(documentTypeID As Integer)

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                DocumentID,
                DocumentNo
            FROM inv.DocumentHeader
            WHERE DocumentTypeID = @DT
            ORDER BY DocumentDate DESC
        ", con)

                cmd.Parameters.AddWithValue("@DT", documentTypeID)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

    End Sub
    Protected Sub LoadPaymentMethodCombo()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
        "SELECT PaymentMethodID, NameAr FROM md.PaymentMethod WHERE IsActive = 1", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        cboPaymentMethod.DataSource = dt
        cboPaymentMethod.DisplayMember = "NameAr"   ' الاسم العربي
        cboPaymentMethod.ValueMember = "PaymentMethodID" ' المفتاح
        cboPaymentMethod.SelectedIndex = -1         ' بدون اختيار افتراضي

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
            FROM md.PaymentTerm
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
        FROM md.Store
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

    Protected Sub LoadProductsForGrid()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    ProductID,
    ProductCode,
    ProductName,
    StorageUnitID,
    ProductTypeID
FROM md.Product
WHERE IsActive = 1
ORDER BY ProductCode
", con)

                con.Open()
                _allProducts = New DataTable()
                _allProducts.Load(cmd.ExecuteReader())

            End Using
        End Using

        ' أول تحميل للجريد
        BindProductCodeGrid(_allProducts)

    End Sub
    Private Sub BindProductCodeGrid(dt As DataTable)

        Dim col =
    CType(dgvInvoiceDetails.Columns("colProductCode"),
          DataGridViewComboBoxColumn)

        col.DataSource = dt
        col.DisplayMember = "ProductCode"
        col.ValueMember = "ProductID"

    End Sub
    Private Sub cboProductFilter_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboProductFilter.SelectedIndexChanged

        If _allProducts Is Nothing Then Exit Sub
        If cboProductFilter.SelectedValue Is Nothing Then Exit Sub

        Dim typeID As Integer

        If TypeOf cboProductFilter.SelectedValue Is DataRowView Then
            typeID = CInt(CType(cboProductFilter.SelectedValue, DataRowView)("ProductTypeID"))
        ElseIf IsNumeric(cboProductFilter.SelectedValue) Then
            typeID = CInt(cboProductFilter.SelectedValue)
        Else
            Exit Sub
        End If

        If typeID = 0 Then
            BindProductCodeGrid(_allProducts)
            Exit Sub
        End If

        Dim filtered = _allProducts.AsEnumerable().
        Where(Function(r) CInt(r("ProductTypeID")) = typeID)

        If filtered.Any() Then
            BindProductCodeGrid(filtered.CopyToDataTable())
        Else
            BindProductCodeGrid(_allProducts.Clone())
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
    Protected Sub OnPartnerChanged(drv As DataRowView)

        ' اسم / كود الشريك
        txtPartnerCode.Text = drv("PartnerName").ToString()

        ' الرقم الضريبي
        txtVATRegistrationNumber.Text =
        drv("VATRegistrationNumber").ToString()

        ' الهاتف / العنوان / المدينة
        Dim phone As String, address As String, city As String
        GetPartnerInfo(drv, phone, address, city)

        txtPhone.Text = phone
        txtAddress.Text = address
        txtCity.Text = city

    End Sub
    Private Sub ClearPartnerFields()
        txtPartnerCode.Text = ""
        txtVATRegistrationNumber.Text = ""
        txtPhone.Text = ""
        txtAddress.Text = ""
        txtCity.Text = ""
    End Sub

    ' =========================
    ' زر جديد – الرجوع لوضع الإدخال
    ' =========================
    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click

        If IsLoading Then Exit Sub

        EnterUIGuard()
        Try
            IsLoading = True

            ' =========================
            ' تصفير حالة المستند
            ' =========================
            CurrentDocumentID = 0
            IsInventoryPosted = False
            IsSaved = False
            CurrentMode = FormMode.NewMode

            ' =========================
            ' تصفير الهيدر
            ' =========================
            txtDocumentID.Text = ""
            dtpDocumentDate.Value = Date.Today
            dtpDueDate.Value = Date.Today

            cboPartnerCode.SelectedIndex = -1
            txtPartnerCode.Clear()
            txtVATRegistrationNumber.Clear()
            txtPhone.Clear()
            txtAddress.Clear()
            txtCity.Clear()
            txtStatusName.Clear()
            btnSaveDraft.Text = "حفظ"
            cboPaymentMethod.SelectedIndex = -1
            cboPaymentTerm.SelectedIndex = -1
            cboTargetStore.SelectedIndex = -1

            txtNote.Clear()

            chkIsTaxInclusive.Checked = False
            cboVATRate.SelectedValue = 1

            ' =========================
            ' تصفير المجاميع
            ' =========================
            txtSubTotal.Text = "0.00"
            txtVATTotal.Text = "0.00"
            txtGrandTotal.Text = "0.00"

            ' =========================
            ' تصفير التفاصيل
            ' =========================
            If InvoiceDetailsTable IsNot Nothing Then
                InvoiceDetailsTable.Clear()
                InvoiceDetailsTable.AcceptChanges()
            End If

            dgvInvoiceDetails.Enabled = True
            dgvInvoiceDetails.ReadOnly = False
            dgvInvoiceDetails.AllowUserToAddRows = True
            dgvInvoiceDetails.AllowUserToDeleteRows = True

            ' =========================
            ' إعادة الحالة الابتدائية
            ' =========================
            RefreshFormStatus(0)

            ' =========================
            ' تطبيق الصلاحيات
            ' =========================
            ApplyEditPermissionByStatus()

        Finally
            IsLoading = False
            ExitUIGuard()
        End Try

    End Sub



    ' ========================================
    ' شرط النوع عند الحفظ
    ' ========================================
    Private Sub dgvInvoiceDetails_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs
) Handles dgvInvoiceDetails.DataError

        e.ThrowException = False

    End Sub
    Protected Function ExecuteScalarValue(
    sql As String,
    parameters As Action(Of SqlCommand),
    Optional con As SqlConnection = Nothing,
    Optional tran As SqlTransaction = Nothing
) As Object

        Dim ownConnection As Boolean = (con Is Nothing)

        If ownConnection Then
            con = New SqlConnection(ConnStr)
            con.Open()
        End If

        Using cmd As New SqlCommand(sql, con)
            If tran IsNot Nothing Then
                cmd.Transaction = tran
            End If

            parameters(cmd)

            Dim result = cmd.ExecuteScalar()

            If ownConnection Then
                con.Close()
            End If

            Return result
        End Using

    End Function


    Private Sub dgvInvoiceDetails_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellEndEdit

        If IsUIGuarded Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim colName As String =
        dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        Select Case colName
            Case "colQty", "colUnitPrice", "colVATRate"

                dgvInvoiceDetails.EndEdit()
                dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

                RecalculatePreview(PreviewRecalcScope.RowOnly, e.RowIndex)
                dgvInvoiceDetails.Refresh()
        End Select

    End Sub
    Private Sub dgvInvoiceDetails_DefaultValuesNeeded(
    sender As Object,
    e As DataGridViewRowEventArgs
) Handles dgvInvoiceDetails.DefaultValuesNeeded

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If InvoiceDetailsTable.Rows.Count = 0 Then Exit Sub

        Dim lastIndex = InvoiceDetailsTable.Rows.Count - 1
        Dim r = InvoiceDetailsTable.Rows(lastIndex)

        If r.RowState = DataRowState.Deleted Then Exit Sub

        ' 🔴 تحقق من اكتمال السطر
        If IsDBNull(r("ProductID")) OrElse
       IsDBNull(r("ProductTypeID")) OrElse
       ToDec(r("Quantity")) <= 0 OrElse
       ToDec(r("UnitPrice")) <= 0 Then

            MessageBox.Show("يجب إكمال السطر الحالي أولاً")

            ' 🔥 منع إنشاء السطر الجديد
            dgvInvoiceDetails.CancelEdit()
        End If

    End Sub

    ' =========================
    ' تحميل هيدر فاتورة المشتريات
    ' =========================
    Protected Sub LoadDocumentHeader(documentID As Integer)

        If documentID <= 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
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
FROM inv.DocumentHeader
WHERE DocumentID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()
                Using r = cmd.ExecuteReader()
                    If Not r.Read() Then Exit Sub

                    CurrentDocumentID = CInt(r("DocumentID"))

                    txtDocumentID.Text = r("DocumentNo").ToString()
                    dtpDocumentDate.Value = CDate(r("DocumentDate"))

                    cboPartnerCode.SelectedValue = CInt(r("PartnerID"))
                    IsInventoryPosted = CBool(r("IsInventoryPosted"))
                    Dim includeTaxValue As Object = r("IsTaxInclusive")

                    If IsDBNull(includeTaxValue) Then
                        chkIsTaxInclusive.Checked = False
                    ElseIf TypeOf includeTaxValue Is Boolean Then
                        chkIsTaxInclusive.Checked = CBool(includeTaxValue)
                    Else
                        chkIsTaxInclusive.Checked = (includeTaxValue.ToString().Trim() = "1")
                    End If
                    txtSubTotal.Text = ToDec(r("TotalAmount")).ToString("N2")
                    txtVATTotal.Text = ToDec(r("TotalTax")).ToString("N2")
                    txtGrandTotal.Text = ToDec(r("TotalNetAmount")).ToString("N2")

                    cboPaymentMethod.SelectedValue = CInt(r("PaymentMethodID"))
                    cboPaymentTerm.SelectedValue = CInt(r("PaymentTermID"))

                    txtNote.Text = r("Notes").ToString()
                End Using
            End Using
        End Using
        RefreshFormStatus(CurrentDocumentID)
    End Sub

    ' =========================
    ' تحميل تفاصيل فاتورة المشتريات
    ' =========================
    Protected Sub LoadDocumentDetails(documentID As Integer)

        If documentID <= 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    d.DetailID,
    d.ProductID,
    p.ProductCode,
    p.ProductName,
    p.ProductTypeID,

    d.Quantity,
    d.UnitID,
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

FROM inv.DocumentDetails d
INNER JOIN md.Product p
    ON p.ProductID = d.ProductID
WHERE d.DocumentID = @DocumentID
ORDER BY d.DetailID
", con)

                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        ' =========================
        ' تعبئة InvoiceDetailsTable
        ' =========================
        InvoiceDetailsTable.Clear()

        For Each r As DataRow In dt.Rows

            Dim newRow As DataRow = InvoiceDetailsTable.NewRow()

            ' ===== معلومات الصنف =====
            newRow("DetailID") = r("DetailID")
            newRow("ProductID") = r("ProductID")
            newRow("ProductCode") = r("ProductCode").ToString()
            newRow("ProductName") = r("ProductName").ToString()
            newRow("ProductTypeID") = r("ProductTypeID")

            ' ===== الكميات =====
            newRow("Quantity") = r("Quantity")
            newRow("UnitID") = r("UnitID")

            ' ===== الأسعار =====
            newRow("UnitPrice") = r("UnitPrice")
            newRow("GrossAmount") = r("GrossAmount")

            ' ===== الخصم =====
            newRow("DiscountRate") = r("DiscountRate")
            newRow("DiscountAmount") = r("DiscountAmount")

            ' ===== الضريبة =====
            newRow("TaxTypeID") = r("TaxTypeID")
            newRow("TaxRate") = r("TaxRate")
            newRow("TaxableAmount") = r("TaxableAmount")
            newRow("TaxAmount") = r("TaxAmount")

            ' ===== المجاميع =====
            newRow("NetAmount") = r("NetAmount")
            newRow("LineTotal") = r("LineTotal")

            ' ===== المخازن =====
            newRow("SourceStoreID") =
            If(IsDBNull(r("SourceStoreID")), DBNull.Value, r("SourceStoreID"))

            newRow("TargetStoreID") =
            If(IsDBNull(r("TargetStoreID")), DBNull.Value, r("TargetStoreID"))

            ' ===== IncludeTax =====
            ' لا نفترض – نستنتج منطقيًا من العلاقة
            ' إذا LineTotal = GrossAmount - DiscountAmount → شامل
            ' إذا LineTotal = NetAmount + TaxAmount → غير شامل

            Dim grossMinusDisc As Decimal =
            CDec(r("GrossAmount")) - CDec(r("DiscountAmount"))

            Dim netPlusTax As Decimal =
            CDec(r("NetAmount")) + CDec(r("TaxAmount"))

            If Math.Round(CDec(r("LineTotal")), 6) =
           Math.Round(grossMinusDisc, 6) Then

                newRow("IncludeTax") = Me.chkIsTaxInclusive.Checked
            Else
                newRow("IncludeTax") = False
            End If

            InvoiceDetailsTable.Rows.Add(newRow)

        Next

        InvoiceDetailsTable.AcceptChanges()

        dgvInvoiceDetails.Refresh()

    End Sub

    Private Sub cboTargetStore_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboTargetStore.SelectedIndexChanged
        If IsUIGuarded Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If cboTargetStore.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboTargetStore.SelectedValue) Then Exit Sub
        ApplyTargetStoreToDetails(CInt(cboTargetStore.SelectedValue))

    End Sub
    ' =========================
    ' منع التعديل في وضع العرض
    ' =========================
    Private Sub chkIsTaxInclusive_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles chkIsTaxInclusive.CheckedChanged

        If IsUIGuarded Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        ' ⛔ لا تعديل بعد الترحيل
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit Then Exit Sub

        ' ⛔ لا تعديل إن لم تسمح السياسة



        ApplyHeaderVATToGrid()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        If CurrentDocumentID <= 0 Then Exit Sub

        Try
            Dim service As New PurchaseApplicationService(ConnStr)

            ' تأكيد للمستخدم
            If MessageBox.Show("هل تريد إلغاء السند؟",
                           "تأكيد",
                           MessageBoxButtons.YesNo) <> DialogResult.Yes Then Exit Sub

            service.CancelPurchase(CurrentDocumentID, 1)

            MessageBox.Show("تم الإلغاء بنجاح")

            LoadDocument(CurrentDocumentID)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub

    Private Sub btnSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnSearch.Click

        Using f As New frmPurchaseSearch()

            If f.ShowDialog() <> DialogResult.OK Then Exit Sub
            If f.SelectedDocumentID <= 0 Then Exit Sub

            LoadDocument(f.SelectedDocumentID)

        End Using

    End Sub
    Private Sub ApplyTargetStoreToDetails(storeID As Integer)

        If InvoiceDetailsTable Is Nothing Then Exit Sub

        For Each r As DataRow In InvoiceDetailsTable.Rows
            If r.RowState <> DataRowState.Deleted Then
                r("TargetStoreID") = storeID
            End If
        Next

    End Sub
    Protected Sub LoadDocument(documentID As Integer)

        EnterUIGuard()
        Try
            ' 🔎 تحميل الهيدر + التفاصيل
            LoadDocumentHeader(documentID)
            LoadDocumentDetails(documentID)
            LoadTargetStoreCombo()

            ' 🔥 توزيع المستودع من الهيدر إلى التفاصيل
            ' 🔥 استخراج المستودع من أول سطر في التفاصيل
            Dim firstRow = InvoiceDetailsTable.AsEnumerable().
    FirstOrDefault(Function(r) _
        r.RowState <> DataRowState.Deleted AndAlso
        Not IsDBNull(r("TargetStoreID"))
    )


            If firstRow IsNot Nothing Then

                Dim storeID As Integer = CInt(firstRow("TargetStoreID"))

                ' 🔥 مهم: إعادة تعيين قبل التعيين
                cboTargetStore.SelectedIndex = -1

                cboTargetStore.SelectedValue = storeID

                ' 🔥 الأهم: لا تعتمد على الحدث
                ApplyTargetStoreToDetails(storeID)

            End If
            LoadDocumentStatus(documentID)
            ApplyEditPermissionByStatus()
            ' 🔎 مزامنة الحالة الحقيقية (StatusID)
            ' 🔎 تحميل حالة الترحيل من DB

            ' 🔐 تطبيق الصلاحيات حسب الحالة والترحيل
            ApplyEditPermissionByStatus()

            btnSaveDraft.Text = "تعديل"
            btnSaveDraft.Enabled = True

        Finally
            ExitUIGuard()
        End Try

    End Sub



    Protected Function BuildDocumentDetailsTVP() As DataTable

        Dim tvp As New DataTable()

        tvp.Columns.Add("ProductID", GetType(Integer))
        tvp.Columns.Add("UnitID", GetType(Integer))
        tvp.Columns.Add("Quantity", GetType(Decimal))
        tvp.Columns.Add("UnitPrice", GetType(Decimal))
        tvp.Columns.Add("GrossAmount", GetType(Decimal))
        tvp.Columns.Add("DiscountRate", GetType(Decimal))
        tvp.Columns.Add("DiscountAmount", GetType(Decimal))
        tvp.Columns.Add("NetAmount", GetType(Decimal))
        tvp.Columns.Add("TaxRate", GetType(Decimal))
        tvp.Columns.Add("TaxAmount", GetType(Decimal))
        tvp.Columns.Add("LineTotal", GetType(Decimal))
        tvp.Columns.Add("SourceStoreID", GetType(Integer))
        tvp.Columns.Add("TargetStoreID", GetType(Integer))
        tvp.Columns.Add("TaxTypeID", GetType(Integer))
        tvp.Columns.Add("TaxableAmount", GetType(Decimal))
        tvp.Columns.Add("DetailID", GetType(Integer))

        ' 🔥 مهم جدا
        tvp.Columns.Add("OriginalDetailID", GetType(Integer))

        For Each r As DataRow In InvoiceDetailsTable.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            If IsDBNull(r("TargetStoreID")) Then
                Throw New ApplicationException("TargetStoreID غير محدد")
            End If

            Dim row As DataRow = tvp.NewRow()

            row("ProductID") = CInt(r("ProductID"))
            row("UnitID") = CInt(r("UnitID"))
            row("Quantity") = ToDec(r("Quantity"))
            row("UnitPrice") = ToDec(r("UnitPrice"))
            row("GrossAmount") = ToDec(r("GrossAmount"))
            row("DiscountRate") = ToDec(r("DiscountRate"))
            row("DiscountAmount") = ToDec(r("DiscountAmount"))
            row("NetAmount") = ToDec(r("NetAmount"))
            row("TaxRate") = ToDec(r("TaxRate"))
            row("TaxAmount") = ToDec(r("TaxAmount"))
            row("LineTotal") = ToDec(r("LineTotal"))

            row("SourceStoreID") =
            If(IsDBNull(r("SourceStoreID")), DBNull.Value, CInt(r("SourceStoreID")))

            row("TargetStoreID") = CInt(r("TargetStoreID"))
            row("TaxTypeID") = CInt(r("TaxTypeID"))
            row("TaxableAmount") = ToDec(r("TaxableAmount"))

            row("DetailID") =
            If(IsDBNull(r("DetailID")), DBNull.Value, r("DetailID"))

            row("OriginalDetailID") =
            If(r.Table.Columns.Contains("OriginalDetailID") AndAlso
               Not IsDBNull(r("OriginalDetailID")),
               r("OriginalDetailID"),
               DBNull.Value)

            tvp.Rows.Add(row)

        Next

        Return tvp

    End Function
    Private Sub cboPaymentTerm_SelectionChangeCommitted(
    sender As Object,
    e As EventArgs
) Handles cboPaymentTerm.SelectionChangeCommitted

        If _suspendDueSync Then Exit Sub

        ' ⛔ كاش = لا نحسب DueDate
        If IsCashPayment_Local() Then Exit Sub

        If cboPaymentTerm.SelectedItem Is Nothing Then Exit Sub

        Dim drv As DataRowView = TryCast(cboPaymentTerm.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        Dim dueDays As Integer = CInt(drv("DueDays"))

        _suspendDueSync = True
        Try
            dtpDueDate.Value = dtpDocumentDate.Value.Date.AddDays(dueDays)
        Finally
            _suspendDueSync = False
        End Try

    End Sub
    Private Sub dtpDueDate_ValueChanged(
    sender As Object,
    e As EventArgs
) Handles dtpDueDate.ValueChanged

        If _suspendDueSync Then Exit Sub
        If cboPaymentTerm.DataSource Is Nothing Then Exit Sub

        Dim baseDate As Date = dtpDocumentDate.Value.Date
        Dim selectedDate As Date = dtpDueDate.Value.Date

        Dim diffDays As Integer =
        DateDiff(DateInterval.Day, baseDate, selectedDate)

        If diffDays < 0 Then diffDays = 0

        Dim bestPaymentTermID As Integer? = Nothing
        Dim bestDays As Integer = -1

        Dim dt As DataTable = CType(cboPaymentTerm.DataSource, DataTable)

        For Each r As DataRow In dt.Rows
            Dim d As Integer = CInt(r("DueDays"))

            If d <= diffDays AndAlso d > bestDays Then
                bestDays = d
                bestPaymentTermID = CInt(r("PaymentTermID"))
            End If
        Next

        If bestPaymentTermID.HasValue Then
            _suspendDueSync = True
            Try
                cboPaymentTerm.SelectedValue = bestPaymentTermID.Value
            Finally
                _suspendDueSync = False
            End Try
        End If

    End Sub
    Private Function IsCashPayment_Local() As Boolean
        If cboPaymentMethod.SelectedItem Is Nothing Then Return False

        Dim drv = TryCast(cboPaymentMethod.SelectedItem, DataRowView)
        If drv Is Nothing Then Return False

        If drv.Row.Table.Columns.Contains("IsCash") Then
            Return Not IsDBNull(drv("IsCash")) AndAlso CBool(drv("IsCash"))
        End If

        Return False
    End Function

    Private Sub cboPaymentMethod_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPaymentMethod.SelectedIndexChanged
        If IsCashPayment_Local() Then
            dtpDueDate.Value = dtpDocumentDate.Value
            dtpDueDate.Enabled = False
        Else
            dtpDueDate.Enabled = True
        End If

    End Sub

    Private Sub btnFindPartner_Click(sender As Object, e As EventArgs) Handles btnFindPartner.Click

        Dim f As New frmPartnerSearch()

        ' لو المشتريات فقط موردين (اختياري)
        ' f.PartnerCodePrefix = "SUP-"

        If f.ShowDialog() <> DialogResult.OK Then Exit Sub

        If f.SelectedPartnerID <= 0 Then Exit Sub

        ' =========================
        ' تحميل الشريك في الهيدر
        ' =========================
        cboPartnerCode.SelectedValue = f.SelectedPartnerID

    End Sub

    Private Sub btnClose_Click_1(sender As Object, e As EventArgs) Handles btnClose.Click

        ' في حال وجود تعديلات غير محفوظة
        If IsSaved = False AndAlso CurrentMode <> FormMode.ViewMode Then

            Dim r = MessageBox.Show(
                "هناك تعديلات غير محفوظة، هل تريد الخروج بدون حفظ؟",
                "تأكيد الإغلاق",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            )

            If r = DialogResult.No Then Exit Sub
        End If

        Me.Close()

    End Sub

    Protected Enum PreviewRecalcScope
        RowOnly
        AllRows
        TotalsOnly
    End Enum

    Protected Sub RecalculatePreview(
    scope As PreviewRecalcScope,
    Optional rowIndex As Integer = -1
)

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If IsUIGuarded Then Exit Sub

        EnterUIGuard()
        Try
            Select Case scope

                Case PreviewRecalcScope.RowOnly
                    If rowIndex >= 0 AndAlso rowIndex < InvoiceDetailsTable.Rows.Count Then
                        RecalculateRowPreview(rowIndex)
                    End If
                    RecalculateTotalsPreview()

                Case PreviewRecalcScope.AllRows
                    For i As Integer = 0 To InvoiceDetailsTable.Rows.Count - 1
                        RecalculateRowPreview(i)
                    Next
                    RecalculateTotalsPreview()

                Case PreviewRecalcScope.TotalsOnly
                    RecalculateTotalsPreview()

            End Select

        Finally
            ExitUIGuard()
        End Try

    End Sub
    Private Sub RecalculateRowPreview(rowIndex As Integer)

        Dim r As DataRow = InvoiceDetailsTable.Rows(rowIndex)
        If r.RowState = DataRowState.Deleted Then Exit Sub

        ' =========================
        ' 🔴 تحقق من الصنف
        ' =========================
        If IsDBNull(r("ProductID")) Then Exit Sub

        If IsDBNull(r("ProductTypeID")) Then
            MessageBox.Show("يجب اختيار نوع الصنف")
            Exit Sub
        End If

        Dim productID As Integer = CInt(r("ProductID"))
        Dim typeID As Integer = CInt(r("ProductTypeID"))


        ' =========================
        ' الحسابات (كما هي)
        ' =========================
        Dim Quantity As Decimal = ToDec(r("Quantity"))
        Dim unitPrice As Decimal = ToDec(r("UnitPrice"))
        Dim discountRate As Decimal = ToDec(r("DiscountRate"))
        Dim vatRatePct As Decimal = ToDec(r("TaxRate"))
        Dim includeTax As Boolean = ToBool(r("IncludeTax"))

        Dim rate As Decimal = vatRatePct / 100D

        Dim gross As Decimal = Quantity * unitPrice
        r("GrossAmount") = Math.Round(gross, 6)

        Dim discountAmount As Decimal = gross * (discountRate / 100D)
        r("DiscountAmount") = Math.Round(discountAmount, 6)

        Dim taxable As Decimal = gross - discountAmount
        r("TaxableAmount") = Math.Round(taxable, 6)

        Dim vat As Decimal
        Dim total As Decimal

        If Not includeTax Then
            vat = taxable * rate
            total = taxable + vat
        Else
            If rate > 0D Then
                taxable = taxable / (1D + rate)
            End If
            vat = (gross - discountAmount) - taxable
            total = gross - discountAmount
        End If

        r("NetAmount") = Math.Round(taxable, 6)
        r("TaxAmount") = Math.Round(vat, 6)
        r("LineTotal") = Math.Round(total, 6)
        r("TaxTypeID") = CInt(cboVATRate.SelectedValue)

    End Sub
    Private Sub RecalculateTotalsPreview()

        Dim subTotal As Decimal = 0D
        Dim vatTotal As Decimal = 0D
        Dim TotalAmount As Decimal = 0D

        For Each r As DataRow In InvoiceDetailsTable.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            subTotal += ToDec(r("NetAmount"))
            vatTotal += ToDec(r("TaxAmount"))
            TotalAmount += ToDec(r("LineTotal"))
        Next

        txtSubTotal.Text = subTotal.ToString("N2")
        txtVATTotal.Text = vatTotal.ToString("N2")
        txtGrandTotal.Text = TotalAmount.ToString("N2")

    End Sub

    Protected Function CanPostDocument(documentID As Integer) As Boolean

        If documentID <= 0 Then Return False

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT COUNT(1)
            FROM inv.DocumentHeader h
            INNER JOIN wf.OperationStatusPolicy p
                ON p.StatusID = h.StatusID
            WHERE h.DocumentID = @ID
              AND p.OperationTypeID = @OperationTypeID
              AND p.AllowPost = 1
              AND p.IsActive = 1
              AND h.IsInventoryPosted = 0
        ", con)

                cmd.Parameters.AddWithValue("@ID", documentID)
                cmd.Parameters.AddWithValue("@OperationTypeID", FormOperationTypeID)

                con.Open()
                Return CInt(cmd.ExecuteScalar()) > 0
            End Using
        End Using

    End Function

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        If IsLoading Then Exit Sub
        If CurrentDocumentID <= 0 Then Exit Sub

        If IsInventoryPosted Then
            MessageBox.Show("السند مرحّل مسبقًا", "مرفوض",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            Dim transactionCode As String = ""

            Using con As New SqlConnection(ConnStr)
                con.Open()

                Using cmdCode As New SqlCommand("cfg.GetNextCode", con)
                    cmdCode.CommandType = CommandType.StoredProcedure
                    cmdCode.Parameters.AddWithValue("@CodeType", "TRN")

                    Dim pOut As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                    pOut.Direction = ParameterDirection.Output
                    cmdCode.Parameters.Add(pOut)

                    cmdCode.ExecuteNonQuery()
                    transactionCode = pOut.Value.ToString()
                End Using
            End Using

            Dim service As New PurchaseApplicationService(ConnStr)
            service.SendPurchase(CurrentDocumentID, transactionCode, CurrentUserID)

            MessageBox.Show("تم إرسال السند وترحيله بنجاح", "تم")

            LoadDocument(CurrentDocumentID)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Sub btnSaveDraft_Click(sender As Object, e As EventArgs) _
    Handles btnSaveDraft.Click

        If IsLoading Then Exit Sub

        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit AndAlso Not IsPostedEditMode Then
            MessageBox.Show("لا يمكن تعديل هذا السند")
            Exit Sub
        End If

        Try
            ' =========================
            ' Validation
            ' =========================
            If Not ValidateDocument() Then Exit Sub
            If Not ValidateDocumentLines() Then Exit Sub


            Using con As New SqlConnection(ConnStr)
                con.Open()

                ' 🆕 توليد رقم عند أول حفظ فقط (كما كان)
                If CurrentDocumentID = 0 Then

                    Using cmdCode As New SqlCommand("cfg.GetNextCode", con)
                        cmdCode.CommandType = CommandType.StoredProcedure
                        cmdCode.Parameters.AddWithValue("@CodeType", FormScopeCode)

                        Dim pNextCode As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                        pNextCode.Direction = ParameterDirection.Output
                        cmdCode.Parameters.Add(pNextCode)

                        cmdCode.ExecuteNonQuery()
                        txtDocumentID.Text = pNextCode.Value.ToString()
                    End Using

                End If

            End Using
            Me.Validate()
            dgvInvoiceDetails.EndEdit()
            ' =========================
            ' استدعاء السيرفس للحفظ
            ' =========================
            Dim service As New PurchaseApplicationService(ConnStr)
            If CurrentDocumentID = 0 Then

                ' 🆕 إنشاء سند جديد
                CurrentDocumentID =
        service.SaveDraftDirect(
            documentID:=0,
            documentNo:=txtDocumentID.Text,
            documentDate:=dtpDocumentDate.Value,
            partnerID:=CInt(cboPartnerCode.SelectedValue),
            taxTypeID:=CInt(cboVATRate.SelectedValue),
            paymentMethodID:=CInt(cboPaymentMethod.SelectedValue),
            paymentTermID:=CInt(cboPaymentTerm.SelectedValue),
            notes:=txtNote.Text,
            isTaxInclusive:=chkIsTaxInclusive.Checked,
            details:=BuildDocumentDetailsTVP()
        )

            Else

                ' =========================================
                ' 🔥 1) وضع تعديل سند مرحل (الأولوية القصوى)
                ' =========================================
                Dim statusID = GetDocumentStatusID(CurrentDocumentID)

                ' =========================
                ' 🔴 الحالة 6 (Received)
                ' =========================
                If statusID = 6 Then

                    If Not IsPostedEditMode Then
                        MessageBox.Show("لا يمكن تعديل سند مستلم إلا عبر وضع تعديل السند المرحل")
                        Exit Sub
                    End If

                    ' ✅ تعديل عبر المحرك فقط
                    CurrentDocumentID =
                        service.SavePostedDocumentWithQueue(
                            documentID:=CurrentDocumentID,
                            documentNo:=txtDocumentID.Text,
                            documentDate:=dtpDocumentDate.Value,
                            partnerID:=CInt(cboPartnerCode.SelectedValue),
                            taxTypeID:=CInt(cboVATRate.SelectedValue),
                            paymentMethodID:=CInt(cboPaymentMethod.SelectedValue),
                            paymentTermID:=CInt(cboPaymentTerm.SelectedValue),
                            notes:=txtNote.Text,
                            isTaxInclusive:=chkIsTaxInclusive.Checked,
                            details:=BuildDocumentDetailsTVP(),
                            originalDetails:=OriginalDetailsTable,
                            scopeCode:=FormScopeCode
                        )

                    ' =========================
                    ' 🟡 الحالة 5 (Sent)
                    ' =========================
                ElseIf statusID = 5 Then
                    ' ✔ تعديل طبيعي لكن يشمل الترانسكشن
                    CurrentDocumentID =
                        service.SaveSentDocument(
                            documentID:=CurrentDocumentID,
                            documentNo:=txtDocumentID.Text,
                            documentDate:=dtpDocumentDate.Value,
                            partnerID:=CInt(cboPartnerCode.SelectedValue),
                            taxTypeID:=CInt(cboVATRate.SelectedValue),
                            paymentMethodID:=CInt(cboPaymentMethod.SelectedValue),
                            paymentTermID:=CInt(cboPaymentTerm.SelectedValue),
                            notes:=txtNote.Text,
                            isTaxInclusive:=chkIsTaxInclusive.Checked,
                            details:=BuildDocumentDetailsTVP()
                        )

                    ' =========================
                    ' 🟢 باقي الحالات
                    ' =========================
                Else

                    CurrentDocumentID =
                        service.SaveDraftDirect(
                            documentID:=CurrentDocumentID,
                            documentNo:=txtDocumentID.Text,
                            documentDate:=dtpDocumentDate.Value,
                            partnerID:=CInt(cboPartnerCode.SelectedValue),
                            taxTypeID:=CInt(cboVATRate.SelectedValue),
                            paymentMethodID:=CInt(cboPaymentMethod.SelectedValue),
                            paymentTermID:=CInt(cboPaymentTerm.SelectedValue),
                            notes:=txtNote.Text,
                            isTaxInclusive:=chkIsTaxInclusive.Checked,
                            details:=BuildDocumentDetailsTVP()
                        )

                End If
            End If
            ' =========================
            ' تحويل الحالة عند أول حفظ فقط (كما كان)
            ' =========================
            If FormStatusID = 1 Then

                Using con As New SqlConnection(ConnStr)
                    Using cmd As New SqlCommand("
                    UPDATE inv.DocumentHeader
                    SET StatusID = 2
                    WHERE DocumentID = @DocumentID
                ", con)

                        cmd.Parameters.AddWithValue("@DocumentID", CurrentDocumentID)
                        con.Open()
                        cmd.ExecuteNonQuery()

                    End Using
                End Using

                RefreshFormStatus(CurrentDocumentID)

            End If
            IsPostedEditMode = False
            MessageBox.Show("تم حفظ الفاتورة بنجاح", "تم")
            btnSaveDraft.Text = "تعديل"
            btnSaveDraft.Enabled = False
            LoadDocument(CurrentDocumentID)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Function GetDocumentStatusID(documentID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)

            Dim sql As String = "
SELECT StatusID 
FROM inv.DocumentHeader
WHERE DocumentID = @ID
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then Return 0
                Return CInt(result)

            End Using
        End Using

    End Function







    ' ========================================
    ' تحميل أنواع المنتجات للجريد
    ' ========================================
    Protected Sub LoadProductTypesForGrid()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    ProductTypeID,
    TypeCode,
    TypeName
FROM md.ProductType
WHERE IsActive = 1
ORDER BY TypeName
", con)

                con.Open()
                _allProductTypes = New DataTable()
                _allProductTypes.Load(cmd.ExecuteReader())

            End Using
        End Using

        ' 🔥 مهم: ربط مبدئي للجريد (optional)
        Dim col =
    CType(dgvInvoiceDetails.Columns("colProductType"),
          DataGridViewComboBoxColumn)

        col.DataSource = _allProductTypes
        col.DisplayMember = "TypeName"
        col.ValueMember = "ProductTypeID"

    End Sub
    ' ========================================
    ' فلترة الأنواع حسب الكود
    ' ========================================
    ' ========================================
    ' منع الخروج من السطر بدون نوع
    ' ========================================
    Private Sub dgvInvoiceDetails_RowValidating(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvInvoiceDetails.RowValidating

        If IsLoading OrElse IsUIGuarded Then Exit Sub

        ' 🔴 حماية أساسية
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If e.RowIndex >= InvoiceDetailsTable.Rows.Count Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim r As DataRow = InvoiceDetailsTable.Rows(e.RowIndex)

        ' 🔥 الحماية الحقيقية (هذه هي المشكلة)
        If r Is Nothing Then Exit Sub
        If r.RowState = DataRowState.Deleted Then Exit Sub
        If r.RowState = DataRowState.Detached Then Exit Sub

        ' =========================
        ' التحقق
        ' =========================

        If r.IsNull("ProductID") OrElse CInt(r("ProductID")) <= 0 Then

            ' 🔴 إذا الكود موجود → المشكلة في النوع
            If Not r.IsNull("ProductCode") Then
                dgvInvoiceDetails.CurrentCell = row.Cells("colProductType")
            Else
                dgvInvoiceDetails.CurrentCell = row.Cells("colProductCode")
            End If

            e.Cancel = True
            Exit Sub
        End If
        If r.IsNull("ProductTypeID") Then
            dgvInvoiceDetails.CurrentCell = row.Cells("colProductType")
            e.Cancel = True
            Exit Sub
        End If

        If r.IsNull("Quantity") OrElse ToDec(r("Quantity")) <= 0 Then
            dgvInvoiceDetails.CurrentCell = row.Cells("colQty")
            e.Cancel = True
            Exit Sub
        End If

    End Sub
    Private Sub dgvInvoiceDetails_CellValidating(
    sender As Object,
    e As DataGridViewCellValidatingEventArgs
) Handles dgvInvoiceDetails.CellValidating

        If _isInternalChange Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        If e.RowIndex >= InvoiceDetailsTable.Rows.Count Then Exit Sub

        Dim r As DataRow = InvoiceDetailsTable.Rows(e.RowIndex)

        If r Is Nothing Then Exit Sub
        If r.RowState = DataRowState.Deleted OrElse r.RowState = DataRowState.Detached Then Exit Sub

        Dim colName As String = dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        ' 🔴 منع الخروج من النوع إذا المنتج غير مكتمل
        If colName = "colProductType" Then

            If r.IsNull("ProductID") OrElse CInt(r("ProductID")) <= 0 Then
                e.Cancel = True
            End If

        End If

    End Sub
    Private Sub dgvInvoiceDetails_CurrentCellChanged(
    sender As Object,
    e As EventArgs
) Handles dgvInvoiceDetails.CurrentCellChanged

        If _isInternalChange Then Exit Sub
        If dgvInvoiceDetails.CurrentCell Is Nothing Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim rowIndex As Integer = dgvInvoiceDetails.CurrentCell.RowIndex
        If rowIndex < 0 OrElse rowIndex >= InvoiceDetailsTable.Rows.Count Then Exit Sub

        Dim r As DataRow = InvoiceDetailsTable.Rows(rowIndex)

        If r Is Nothing Then Exit Sub
        If r.RowState = DataRowState.Deleted OrElse r.RowState = DataRowState.Detached Then Exit Sub

        ' 🔴 إذا المنتج غير مكتمل
        If r.IsNull("ProductID") OrElse CInt(r("ProductID")) <= 0 Then

            _isInternalChange = True
            Try
                If Not r.IsNull("ProductCode") Then
                    dgvInvoiceDetails.CurrentCell = dgvInvoiceDetails.Rows(rowIndex).Cells("colProductType")
                Else
                    dgvInvoiceDetails.CurrentCell = dgvInvoiceDetails.Rows(rowIndex).Cells("colProductCode")
                End If
            Finally
                _isInternalChange = False
            End Try

        End If

    End Sub


    Private Enum EditModeType
        DirectEdit
        EngineEdit
        NoEdit
    End Enum

    Private Function GetEditMode(statusID As Integer) As EditModeType

        Select Case FormStatusID

            Case 1, 2
                Return EditModeType.DirectEdit

            Case 5
                Return EditModeType.EngineEdit

            Case 10, 11, 6
                Return EditModeType.NoEdit

            Case Else
                Return EditModeType.NoEdit

        End Select

    End Function
    Private Sub LoadDocumentStatus(documentID As Integer)

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT s.StatusID, s.StatusName
            FROM inv.DocumentHeader h
            INNER JOIN wf.Status s ON s.StatusID = h.StatusID
            WHERE h.DocumentID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()

                Using r = cmd.ExecuteReader()

                    If Not r.Read() Then
                        Throw New Exception("لم يتم العثور على حالة السند")
                    End If

                    FormStatusID = CInt(r("StatusID"))
                    txtStatusName.Text = r("StatusName").ToString()

                End Using
            End Using
        End Using

    End Sub
    Private Sub dgvInvoiceDetails_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoiceDetails.CellClick

        If e.RowIndex < 0 Then Exit Sub

        If dgvInvoiceDetails.Columns(e.ColumnIndex).Name = "colDelete" Then

            DeleteRow(e.RowIndex)

        End If

    End Sub
    Private Sub DeleteRow(rowIndex As Integer)

        Dim dt As DataTable = CType(dgvInvoiceDetails.DataSource, DataTable)

        If dt Is Nothing Then Exit Sub

        ' 👇 تأكيد
        If MessageBox.Show("هل تريد حذف السطر؟", "تأكيد",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        ' 👇 حذف منطقي من الـ DataTable فقط
        dt.Rows(rowIndex).Delete()

    End Sub
    Private Function GetCancelActionFromStatus(statusID As Integer) As CancelActionType

        Select Case statusID

        ' 🟢 delete
            Case 1, 2 ' مسودة / جديد
                Return CancelActionType.Delete

        ' 🟡 zero
            Case 5 ' تم الإرسال / تم الاستلام / مرتجع / ملغي / مغلق
                Return CancelActionType.Zero

                ' 🔴 غير معروف
            Case 6, 9, 10, 11
                Return CancelActionType.None

        End Select

    End Function

    Private Sub btnEditPostedPurchase_Click(
    sender As Object,
    e As EventArgs
) Handles btnEditPostedPurchase.Click

        If CurrentDocumentID <= 0 Then
            MessageBox.Show("لا يوجد سند مفتوح")
            Exit Sub
        End If

        ' =========================
        ' تفعيل وضع تعديل سند مرحل
        ' =========================
        IsPostedEditMode = True
        OriginalDetailsTable = InvoiceDetailsTable.Copy()
        ' =========================
        ' تجهيز جدول جديد
        ' =========================
        Dim newTable As DataTable = InvoiceDetailsTable.Clone()

        ' إضافة عمود الربط
        If Not newTable.Columns.Contains("OriginalDetailID") Then
            newTable.Columns.Add("OriginalDetailID", GetType(Integer))
        End If

        ' =========================
        ' نسخ البيانات مع الربط
        ' =========================
        For Each r As DataRow In InvoiceDetailsTable.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            Dim newRow As DataRow = newTable.NewRow()

            ' نسخ كل الأعمدة
            For Each col As DataColumn In InvoiceDetailsTable.Columns
                newRow(col.ColumnName) = r(col.ColumnName)
            Next

            ' ربط السطر بالقديم
            If Not IsDBNull(r("DetailID")) Then
                newRow("OriginalDetailID") = r("DetailID")
            End If

            ' ❗ لا تصفر DetailID
            ' نحتاجه للتحديث

            newTable.Rows.Add(newRow)

        Next

        ' =========================
        ' استبدال الجدول
        ' =========================
        InvoiceDetailsTable = newTable
        dgvInvoiceDetails.DataSource = InvoiceDetailsTable
        dgvInvoiceDetails.Columns("colDelete").Visible = False
        ' =========================
        ' 🔥 فك القفل عن الفورم
        ' =========================
        SetFormEditable(True)
        ApplyEditPermissionByStatus()
        MessageBox.Show("تم تفعيل وضع تعديل السند المرحل")

    End Sub
    Private Sub SetFormEditable(isEditable As Boolean)

        ' TextBoxes
        txtNote.ReadOnly = Not isEditable

        ' Combos
        cboPartnerCode.Enabled = isEditable
        cboVATRate.Enabled = isEditable
        cboPaymentMethod.Enabled = isEditable
        cboPaymentTerm.Enabled = isEditable

        ' Grid
        dgvInvoiceDetails.ReadOnly = Not isEditable
        dgvInvoiceDetails.AllowUserToAddRows = isEditable
        dgvInvoiceDetails.AllowUserToDeleteRows = isEditable

    End Sub
    Private Sub btnDeletePostedPurchase_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnDeletePostedPurchase.Click

        If CurrentDocumentID <= 0 Then Exit Sub

        Dim result = MessageBox.Show(
            "سيتم إلغاء المستند بالكامل، هل أنت متأكد؟",
            "تأكيد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

        If result <> DialogResult.Yes Then Exit Sub

        ' 🔥 ندخل وضع التعديل
        btnEditPostedPurchase.PerformClick()

        ' 🔥 نحول كل الكميات إلى صفر
        For Each row As DataRow In InvoiceDetailsTable.Rows
            row("Quantity") = 0D
        Next

        ' 🔥 تحديث الحساب
        btnEditPostedPurchase.Enabled = False
    End Sub

    Private Sub FormatInvoiceGrid(dgv As DataGridView)

        If dgv.Columns.Count = 0 Then Exit Sub

        ' 🔥 إيقاف التحديث مؤقتًا
        dgv.SuspendLayout()

        ' =========================
        ' 🎯 إعداد عام
        ' =========================
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        dgv.RowHeadersVisible = False
        dgv.AllowUserToResizeRows = False

        ' =========================
        ' 🧾 تسميات الأعمدة
        ' =========================
        dgv.Columns("colProductCode").HeaderText = "كود الصنف"
        dgv.Columns("colProductType").HeaderText = "النوع"
        dgv.Columns("colProductName").HeaderText = "اسم الصنف"
        dgv.Columns("colUnitID").HeaderText = "الوحدة"
        dgv.Columns("colQty").HeaderText = "الكمية"
        dgv.Columns("colUnitPrice").HeaderText = "السعر"
        dgv.Columns("colTaxableAmount").HeaderText = "المبلغ"
        dgv.Columns("colVATRate").HeaderText = "الضريبة %"
        dgv.Columns("colVATAmount").HeaderText = "مبلغ الضريبة"
        dgv.Columns("colTotalAmount").HeaderText = "الإجمالي"
        dgv.Columns("colDelete").HeaderText = "حذف"

        ' =========================
        ' 📏 عرض الأعمدة
        ' =========================
        dgv.Columns("colProductSearch").Width = 60

        dgv.Columns("colProductCode").Width = 140
        dgv.Columns("colProductType").Width = 80
        dgv.Columns("colProductName").Width = 160
        dgv.Columns("colUnitID").Width = 90
        dgv.Columns("colQty").Width = 80
        dgv.Columns("colUnitPrice").Width = 80
        dgv.Columns("colTaxableAmount").Width = 100
        dgv.Columns("colVATRate").Width = 70
        dgv.Columns("colVATAmount").Width = 100
        dgv.Columns("colTotalAmount").Width = 110
        dgv.Columns("colDelete").Width = 80

        ' =========================
        ' 🔢 تنسيق الأرقام
        ' =========================
        Dim numericCols = {
        "colQty",
        "colUnitPrice",
        "colTaxableAmount",
        "colVATRate",
        "colVATAmount",
        "colTotalAmount"
    }

        For Each colName In numericCols
            If dgv.Columns.Contains(colName) Then
                dgv.Columns(colName).DefaultCellStyle.Format = "N2"
                dgv.Columns(colName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next

        ' =========================
        ' 🔤 محاذاة النصوص
        ' =========================
        dgv.Columns("colProductName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgv.Columns("colProductCode").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv.Columns("colProductType").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv.Columns("colUnitID").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        ' =========================
        ' 🙈 إخفاء الأعمدة التقنية
        ' =========================
        If dgv.Columns.Contains("colProductID") Then
            dgv.Columns("colProductID").Visible = False
        End If

        ' =========================
        ' 🎨 تحسين الشكل
        ' =========================
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgv.EnableHeadersVisualStyles = False
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 9, FontStyle.Bold)

        dgv.Columns("colProductSearch").Visible = False
        dgv.Columns("colProductName").ReadOnly = True
        dgv.Columns("colUnitID").ReadOnly = True
        ' =========================
        ' 🔥 تشغيل التحديث
        ' =========================
        dgv.ResumeLayout()

    End Sub
    Private Function HasDuplicateProduct(
    productID As Integer,
    productTypeID As Integer,
    currentRowIndex As Integer
) As Boolean

        If InvoiceDetailsTable Is Nothing Then Return False

        For i As Integer = 0 To InvoiceDetailsTable.Rows.Count - 1

            If i = currentRowIndex Then Continue For

            Dim r As DataRow = InvoiceDetailsTable.Rows(i)

            If r.RowState = DataRowState.Deleted Then Continue For

            If IsDBNull(r("ProductID")) OrElse IsDBNull(r("ProductTypeID")) Then Continue For

            If CInt(r("ProductID")) = productID _
        AndAlso CInt(r("ProductTypeID")) = productTypeID Then

                Return True
            End If

        Next

        Return False

    End Function
    Private Function FinalizeRowAfterProductChange(rowIndex As Integer) As Boolean

        If rowIndex < 0 OrElse rowIndex >= dgvInvoiceDetails.Rows.Count Then Return False

        Dim row = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Return False

        Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)
        If drv Is Nothing Then Return False

        ' 🔴 لا تكمل إذا الصنف غير موجود
        If IsDBNull(drv("ProductID")) Then Return False

        ' =========================
        ' منع التكرار (بعد اكتمال النوع)
        ' =========================
        If Not IsDBNull(drv("ProductID")) AndAlso
       Not IsDBNull(drv("ProductTypeID")) Then

            Dim productID As Integer = CInt(drv("ProductID"))
            Dim typeID As Integer = CInt(drv("ProductTypeID"))

            If HasDuplicateProduct(productID, typeID, rowIndex) Then

                MessageBox.Show("لا يمكن إدخال نفس الصنف (نفس النوع) أكثر من مرة")

                EnterUIGuard()
                Try
                    row.Cells("colProductType").Value = DBNull.Value

                    drv("ProductTypeID") = DBNull.Value
                    drv("ProductName") = ""
                    drv("UnitID") = DBNull.Value
                    drv("UnitName") = ""

                    drv("GrossAmount") = 0D
                    drv("TaxableAmount") = 0D
                    drv("TaxAmount") = 0D
                    drv("NetAmount") = 0D
                    drv("LineTotal") = 0D

                    drv.EndEdit()
                Finally
                    ExitUIGuard()
                End Try

                dgvInvoiceDetails.CurrentCell = row.Cells("colProductType")

                dgvInvoiceDetails.Refresh()
                RecalculatePreview(PreviewRecalcScope.TotalsOnly)

                Return False

            End If
        End If

        ' =========================
        ' الحساب
        ' =========================
        If Not IsDBNull(drv("ProductTypeID")) Then
            RecalculatePreview(PreviewRecalcScope.RowOnly, rowIndex)
        End If

        dgvInvoiceDetails.Refresh()
        Return True

    End Function
    Protected Function ValidateDocumentLines() As Boolean

        dgvInvoiceDetails.EndEdit()
        dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

        NormalizeInvoiceGrid()

        If InvoiceDetailsTable Is Nothing _
           OrElse InvoiceDetailsTable.Rows.Count = 0 Then
            MessageBox.Show("لا توجد أصناف صالحة في الفاتورة.")
            Return False
        End If

        Dim seen As New HashSet(Of Integer)

        For Each r As DataRow In InvoiceDetailsTable.Rows

            If r.RowState = DataRowState.Deleted Then Continue For

            If IsDBNull(r("ProductTypeID")) Then
                MessageBox.Show("يجب تحديد نوع الصنف")
                Return False
            End If

            If IsDBNull(r("ProductID")) Then
                MessageBox.Show("يوجد صنف غير محدد")
                Return False
            End If

            Dim productID As Integer = CInt(r("ProductID"))

            If seen.Contains(productID) Then
                MessageBox.Show("لا يمكن تكرار نفس الصنف داخل التفاصيل")
                Return False
            End If

            seen.Add(productID)
        Next

        Return True

    End Function
    Private Function GetTypesByProductCode(productID As Integer) As DataTable

        Dim selected = _allProducts.AsEnumerable().
        FirstOrDefault(Function(r) CInt(r("ProductID")) = productID)

        If selected Is Nothing Then Return Nothing

        Dim code As String = selected("ProductCode").ToString()

        Dim typeIds = _allProducts.AsEnumerable().
        Where(Function(r) r("ProductCode").ToString() = code).
        Select(Function(r) CInt(r("ProductTypeID"))).
        Distinct().
        ToList()

        Dim result As DataTable = _allProductTypes.Clone()

        For Each typeId As Integer In typeIds
            Dim typeRow = _allProductTypes.AsEnumerable().
            FirstOrDefault(Function(r) CInt(r("ProductTypeID")) = typeId)

            If typeRow IsNot Nothing Then
                result.ImportRow(typeRow)
            End If
        Next

        Return result

    End Function
    Private Function IsDuplicateProduct(productID As Integer, currentRow As Integer) As Boolean

        For i = 0 To InvoiceDetailsTable.Rows.Count - 1

            If i = currentRow Then Continue For

            Dim r = InvoiceDetailsTable.Rows(i)
            If r.RowState = DataRowState.Deleted Then Continue For

            If Not IsDBNull(r("ProductID")) Then
                If CInt(r("ProductID")) = productID Then
                    Return True
                End If
            End If

        Next

        Return False


    End Function
End Class



