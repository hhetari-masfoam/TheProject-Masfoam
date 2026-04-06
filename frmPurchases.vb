Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class frmPurchases
    Inherits AABaseOperationForm

    ' =========================
    ' دالة تحميل الكمبو الخاصة بالمشتريات (الموردين فقط)
    Private _purchaseService As New PurchaseApplicationService(AppConfig.MainConnectionString)
    Private CurrentStatusID As Integer = 0
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
    Private _isPostedCorrectionEdit As Boolean = False
    Private _deletedOriginalDetailIDs As New List(Of Integer)

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

    Private Function DumpRow(i As Integer, title As String) As String
        If InvoiceDetailsTable Is Nothing Then Return title & ": InvoiceDetailsTable = Nothing"
        If i < 0 OrElse i >= InvoiceDetailsTable.Rows.Count Then Return title & ": row out of range"

        Dim r = InvoiceDetailsTable.Rows(i)
        If r.RowState = DataRowState.Deleted Then Return title & ": row is Deleted"

        Dim s As New System.Text.StringBuilder()
        s.AppendLine("==== " & title & " (Row " & i & ") ====")
        s.AppendLine("IsUIGuarded=" & IsUIGuarded.ToString())
        s.AppendLine("Quantity=" & ToDec(r("Quantity")).ToString("N2") &
                 " UnitPrice=" & ToDec(r("UnitPrice")).ToString("N2"))
        s.AppendLine("TaxRate=" & ToDec(r("TaxRate")).ToString("N2") &
                 " IncludeTax=" & ToBool(r("IncludeTax")).ToString())
        s.AppendLine("NetAmount=" & ToDec(r("NetAmount")).ToString("N2"))
        s.AppendLine("TaxAmount=" & ToDec(r("TaxAmount")).ToString("N2"))
        s.AppendLine("LineTotal=" & ToDec(r("LineTotal")).ToString("N2"))
        s.AppendLine("HeaderTotals: Sub=" & txtSubTotal.Text &
                 " VAT=" & txtVATTotal.Text &
                 " Grand=" & txtGrandTotal.Text)
        Return s.ToString()
    End Function


    Private Sub frmPurchases_Load(
    sender As Object,
    e As EventArgs
) Handles Me.Load

        If IsLoading Then Return
        IsLoading = True

        Try
            dgvInvoiceDetails.AutoGenerateColumns = False
            dgvInvoiceDetails.EditMode = DataGridViewEditMode.EditOnEnter

            LoadUnitsForGrid()
            InitInvoiceDetailsTable()
            LoadProductTypesForGrid()
            dgvInvoiceDetails.DataSource = InvoiceDetailsTable
            LoadProductsForGrid()
            LoadProductTypeFilterCombo()

            RemoveHandler InvoiceDetailsTable.RowChanged, AddressOf InvoiceDetailsTable_RowChanged
            AddHandler InvoiceDetailsTable.RowChanged, AddressOf InvoiceDetailsTable_RowChanged

            LoadPartnerComboBox()
            LoadPaymentMethodCombo()
            LoadPaymentTermCombo()
            LoadSourceCombo()
            LoadTargetStoreCombo()
            LoadVATRateCombo()

            If cboVATRate.DataSource IsNot Nothing Then
                cboVATRate.SelectedValue = 1
            End If

            colProductCode.DataPropertyName = "ProductCode"  ' ✅ الكود يُخزن في ProductCode
            colProductID.DataPropertyName = "ProductID"      ' ✅ يبقى كما هو
            colProductName.DataPropertyName = "ProductName"
            colProductType.DataPropertyName = "ProductTypeID"

            colUnitID.DataPropertyName = "UnitID"
            colQty.DataPropertyName = "Quantity"
            colUnitPrice.DataPropertyName = "UnitPrice"
            colVATRate.DataPropertyName = "TaxRate"
            colVATAmount.DataPropertyName = "TaxAmount"
            colTaxableAmount.DataPropertyName = "NetAmount"
            colTotalAmount.DataPropertyName = "LineTotal"

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
            Dim exists As Boolean = False

            If comboCell.DataSource IsNot Nothing Then
                Dim dt As DataTable = CType(comboCell.DataSource, DataTable)

                For Each r As DataRow In dt.Rows
                    If r("ProductCode").ToString() = f.SelectedProductCode Then
                        exists = True
                        Exit For
                    End If
                Next

                ' إذا غير موجود، نضيفه مؤقتًا
                If Not exists Then
                    Dim newRow = dt.NewRow()
                    newRow("ProductCode") = f.SelectedProductCode
                    newRow("ProductID") = f.SelectedProductID
                    dt.Rows.Add(newRow)
                End If
            End If

            ' =========================
            ' (3) تعيين الكود
            ' =========================
            dgvInvoiceDetails.CurrentCell = row.Cells("colProductCode")
            row.Cells("colProductCode").Value = f.SelectedProductID
            ' تثبيت التغيير
            dgvInvoiceDetails.EndEdit()
            dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

            ' =========================
            ' (4) تحميل بقية البيانات
            ' =========================
            FillProductRow(e.RowIndex)

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
            FROM Master_ProductType
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
            If _isPostedCorrectionEdit Then
                e.Row("IsChanged") = True
            End If
        Finally
            ExitUIGuard()
        End Try

    End Sub
    Private Sub ApplyHeaderStoreToDetails()

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If cboTargetStore.SelectedValue Is Nothing _
   OrElse Not IsNumeric(cboTargetStore.SelectedValue) Then Exit Sub

        Dim storeID As Integer = CInt(cboTargetStore.SelectedValue)

        For Each r As DataRow In InvoiceDetailsTable.Rows
            If r.RowState <> DataRowState.Deleted Then
                r("TargetStoreID") = storeID
            End If
        Next

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
        FROM Master_TaxType
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
            cboSource.Enabled = True
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

        '       If IsUIGuarded Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        ' ⛔ لا تعديل بعد الترحيل
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)

        If mode = EditModeType.NoEdit Then Exit Sub

        Dim colName As String =
        dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        Select Case colName

            Case "colProductCode", "colProductType"

                EnterUIGuard()
                Try
                    FillProductRow(e.RowIndex)

                    RecalculatePreview(
                        PreviewRecalcScope.RowOnly,
                        e.RowIndex
                    )

                Finally
                    ExitUIGuard()
                End Try
            Case "colQty", "colUnitPrice", "colVATRate"

                ' ⭐ تثبيت القيمة قبل الحساب
                dgvInvoiceDetails.CommitEdit(
                DataGridViewDataErrorContexts.Commit
            )

                ' 🔁 حساب Preview للسطر + الإجمالي
                RecalculatePreview(
                PreviewRecalcScope.RowOnly,
                e.RowIndex
            )

        End Select

    End Sub

    Protected Sub NormalizeInvoiceGrid()

        If InvoiceDetailsTable Is Nothing Then Exit Sub

        For i As Integer = InvoiceDetailsTable.Rows.Count - 1 To 0 Step -1

            Dim r As DataRow = InvoiceDetailsTable.Rows(i)

            ' 🟢 تجاهل المحذوف
            If r.RowState = DataRowState.Deleted Then Continue For

            ' 🟢 حذف الصفوف الفارغة بالكامل
            If IsDBNull(r("ProductID")) _
           OrElse CInt(r("ProductID")) <= 0 _
           OrElse ToDec(r("Quantity")) <= 0D Then

                InvoiceDetailsTable.Rows.RemoveAt(i)
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


    ' ========================================
    ' تعديل InitInvoiceDetailsTable
    ' ========================================
    Protected Sub InitInvoiceDetailsTable()

        InvoiceDetailsTable = New DataTable()

        ' معلومات الصنف
        InvoiceDetailsTable.Columns.Add("OriginalDetailID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("DetailID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductCode", GetType(String))
        InvoiceDetailsTable.Columns.Add("ProductTypeID", GetType(Integer))
        InvoiceDetailsTable.Columns.Add("ProductName", GetType(String))

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
        InvoiceDetailsTable.Columns.Add("IsChanged", GetType(Boolean))
    End Sub
    ' =========================
    ' frmPurchases
    ' =========================
    Protected Sub ApplyEditPermissionByStatus()
        Dim currentStatus = GetDocumentStatusID(CurrentDocumentID)
        Dim mode = GetEditMode(currentStatus)
        If CurrentDocumentID = 0 Then
            dgvInvoiceDetails.ReadOnly = False
            dgvInvoiceDetails.Enabled = True
            btnSaveDraft.Enabled = True
            btnCancel.Enabled = True
            Exit Sub
        End If
        Select Case mode

            Case EditModeType.DirectEdit

                dgvInvoiceDetails.ReadOnly = False
                dgvInvoiceDetails.Enabled = True

                btnSaveDraft.Enabled = True
                btnCancel.Enabled = True

            Case EditModeType.EngineEdit

                ' 👇 نسمح بالتعديل في الشاشة
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

    Private Sub EnableEditForNewDocument()

        ' 🆕 هذه الدالة تُستخدم فقط لسند جديد
        If CurrentDocumentID <> 0 Then Exit Sub

        dgvInvoiceDetails.Enabled = True
        dgvInvoiceDetails.ReadOnly = False
        dgvInvoiceDetails.AllowUserToAddRows = True
        dgvInvoiceDetails.AllowUserToDeleteRows = True

        btnSend.Enabled = True

        dtpDocumentDate.Enabled = True
        cboPartnerCode.Enabled = True
        cboSource.Enabled = True
        cboPaymentMethod.Enabled = True
        cboPaymentTerm.Enabled = True
        cboTargetStore.Enabled = True
        txtNote.ReadOnly = False

        ' 🔒 الحالة دائمًا مقفلة في UI


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
        If String.IsNullOrWhiteSpace(txtSubTotal.Text) _
       OrElse Val(txtSubTotal.Text) <= 0 Then
            MessageBox.Show("إجمالي الأصناف غير صحيح")
            txtSubTotal.Focus()
            Return False
        End If

        ' =========================
        ' 7️⃣ VAT Total
        ' =========================
        If String.IsNullOrWhiteSpace(txtVATTotal.Text) _
       OrElse Val(txtVATTotal.Text) < 0 Then
            MessageBox.Show("قيمة الضريبة غير صحيحة")
            txtVATTotal.Focus()
            Return False
        End If

        ' =========================
        ' 8️⃣ Grand Total
        ' =========================
        If String.IsNullOrWhiteSpace(txtGrandTotal.Text) _
       OrElse Val(txtGrandTotal.Text) <= 0 Then
            MessageBox.Show("الإجمالي النهائي غير صحيح")
            txtGrandTotal.Focus()
            Return False
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

            If IsDBNull(r("Quantity")) OrElse Val(r("Quantity")) <= 0 Then
                MessageBox.Show("الكمية يجب أن تكون أكبر من صفر")
                Return False
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
            FROM Inventory_DocumentHeader
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
        "SELECT PaymentMethodID, NameAr FROM Master_PaymentMethod WHERE IsActive = 1", con)

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
    Protected Sub LoadSourceCombo()

        Dim dt As New DataTable
        dt.Columns.Add("ID", GetType(Boolean))
        dt.Columns.Add("Name", GetType(String))

        dt.Rows.Add(False, "داخلي")
        dt.Rows.Add(True, "خارجي")

        cboSource.DataSource = dt
        cboSource.DisplayMember = "Name"   ' داخلي / خارجي
        cboSource.ValueMember = "ID"       ' False / True
        cboSource.SelectedIndex = -1

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

    Protected Sub LoadProductsForGrid()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    ProductID,
    ProductCode,
    ProductName,
    StorageUnitID,
    ProductTypeID
FROM dbo.Master_Product
WHERE IsActive = 1
ORDER BY ProductCode
", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        Dim col =
        CType(dgvInvoiceDetails.Columns("colProductCode"),
              DataGridViewComboBoxColumn)

        col.DataSource = dt
        col.DisplayMember = "ProductCode"
        col.ValueMember = "ProductCode"

    End Sub

    Private Sub LoadUnitsForGrid()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                UnitID,
                UnitName
            FROM Master_Unit
            WHERE IsActive = 1
            ORDER BY UnitName
        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using
        End Using

        Dim col =
        CType(dgvInvoiceDetails.Columns("colUnitID"),
              DataGridViewComboBoxColumn)

        col.DataSource = dt
        col.DisplayMember = "UnitName"
        col.ValueMember = "UnitID"

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
            _isPostedCorrectionEdit = False
            _deletedOriginalDetailIDs.Clear()
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
            btnSaveDraft.Text = "حفظ"
            cboPaymentMethod.SelectedIndex = -1
            cboPaymentTerm.SelectedIndex = -1
            cboSource.SelectedIndex = -1
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


    Private Function GetNullableInt(value As Object) As Integer?

        If value Is Nothing OrElse IsDBNull(value) Then
            Return Nothing
        End If

        Dim s As String = value.ToString().Trim()

        If s = "" Then
            Return Nothing
        End If

        Dim i As Integer
        If Integer.TryParse(s, i) Then
            Return i
        End If

        Return Nothing

    End Function

    ' ========================================
    ' شرط النوع عند الحفظ
    ' ========================================
    Protected Function ValidateDocumentLines() As Boolean

        dgvInvoiceDetails.EndEdit()
        dgvInvoiceDetails.CommitEdit(
        DataGridViewDataErrorContexts.Commit
    )

        NormalizeInvoiceGrid()

        If InvoiceDetailsTable Is Nothing _
        OrElse InvoiceDetailsTable.Rows.Count = 0 Then
            MessageBox.Show("لا توجد أصناف صالحة في الفاتورة.")
            Return False
        End If

        For Each r As DataRow In InvoiceDetailsTable.Rows
            If IsDBNull(r("ProductTypeID")) Then
                MessageBox.Show("يجب تحديد نوع الصنف")
                Return False
            End If
        Next

        Return True

    End Function
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


    ' =========================
    ' إعادة حساب كل سطور الفاتورة بعد التحميل
    ' =========================
    ' ========================================
    ' تعديل FillProductRow
    ' ========================================
    Protected Sub FillProductRow(rowIndex As Integer)

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If rowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        Dim productCodeObj = row.Cells("colProductCode").Value   ' ✅ الآن هي ProductCode
        Dim productTypeObj = row.Cells("colProductType").Value

        If productCodeObj Is Nothing OrElse IsDBNull(productCodeObj) Then Exit Sub
        If productTypeObj Is Nothing OrElse IsDBNull(productTypeObj) Then Exit Sub

        Dim productCode As String = productCodeObj.ToString()
        Dim productTypeID As Integer = CInt(productTypeObj)
        Dim col = CType(dgvInvoiceDetails.Columns("colProductCode"), DataGridViewComboBoxColumn)
        Dim src As DataTable = CType(col.DataSource, DataTable)

        Dim safeCode = productCode.Replace("'", "''")
        Dim found() As DataRow =
    src.Select("ProductCode = '" & safeCode & "' AND ProductTypeID = " & productTypeID)

        If found.Length = 0 Then Exit Sub

        Dim productID As Integer = CInt(found(0)("ProductID"))

        drv("ProductID") = productID
        drv("ProductName") = found(0)("ProductName").ToString()
        drv("UnitID") = CInt(found(0)("StorageUnitID"))
        drv.EndEdit()
        dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
        dgvInvoiceDetails.Refresh()

    End Sub
    Private Sub dgvInvoiceDetails_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvInvoiceDetails.CurrentCellDirtyStateChanged

        If dgvInvoiceDetails.CurrentCell Is Nothing Then Exit Sub

        Dim colName As String =
        dgvInvoiceDetails.Columns(dgvInvoiceDetails.CurrentCell.ColumnIndex).Name

        If colName = "colProductCode" OrElse colName = "colProductType" Then
            dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
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
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()
                Using r = cmd.ExecuteReader()
                    If Not r.Read() Then Exit Sub

                    CurrentDocumentID = CInt(r("DocumentID"))

                    CurrentDocumentID = CInt(r("DocumentID"))
                    txtDocumentID.Text = r("DocumentNo").ToString()
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
d.CorrectionReferenceDetailID,
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

FROM Inventory_DocumentDetails d
INNER JOIN Master_Product p
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
            If IsDBNull(r("CorrectionReferenceDetailID")) Then
                newRow("OriginalDetailID") = r("DetailID") ' أول مرة
            Else
                newRow("OriginalDetailID") = r("CorrectionReferenceDetailID") ' تصحيح
            End If
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
            newRow("IsChanged") = False
            InvoiceDetailsTable.Rows.Add(newRow)

        Next

        InvoiceDetailsTable.AcceptChanges()

        dgvInvoiceDetails.Refresh()

    End Sub
    Protected Sub InferTargetStoreFromDetails()

        If InvoiceDetailsTable Is Nothing Then Exit Sub
        If InvoiceDetailsTable.Rows.Count = 0 Then Exit Sub

        For Each r As DataRow In InvoiceDetailsTable.Rows
            If r.RowState <> DataRowState.Deleted AndAlso
           Not IsDBNull(r("TargetStoreID")) Then

                cboTargetStore.SelectedValue = CInt(r("TargetStoreID"))

                ' 🔹 تحميل المنتجات (لم تعد تعتمد على المستودع)
                LoadProductsForGrid()

                Exit For
            End If
        Next

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

    Private Sub ApplyViewMode()

        ' تعطيل الهيدر
        dtpDocumentDate.Enabled = False
        cboPartnerCode.Enabled = False
        cboSource.Enabled = False
        cboPaymentMethod.Enabled = False
        cboPaymentTerm.Enabled = False
        txtNote.ReadOnly = True
        txtDocumentID.Enabled = False


        ' تعطيل الجريد
        ' الأزرار
        btnSend.Enabled = False

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
    Protected Sub btnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
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
    Protected Sub LoadDocument(documentID As Integer)
        _isPostedCorrectionEdit = False
        _deletedOriginalDetailIDs.Clear()
        EnterUIGuard()
        Try
            ' 🔎 تحميل الهيدر + التفاصيل
            LoadDocumentHeader(documentID)
            LoadDocumentDetails(documentID)
            LoadDocumentStatus(documentID)
            ApplyEditPermissionByStatus()
            InferTargetStoreFromDetails()

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

        ' مطابق تمامًا لجدول Inventory_DocumentDetails
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
        tvp.Columns.Add("OriginalDetailID", GetType(Integer))
        tvp.Columns.Add("DetailID", GetType(Integer))
        tvp.Columns.Add("IsChanged", GetType(Boolean))
        For Each r As DataRow In InvoiceDetailsTable.Select("", "", DataViewRowState.CurrentRows)
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
            row("OriginalDetailID") =
    If(IsDBNull(r("OriginalDetailID")), DBNull.Value, r("OriginalDetailID"))
            row("DetailID") =
If(IsDBNull(r("DetailID")), DBNull.Value, r("DetailID"))
            row("IsChanged") =
    If(r.Table.Columns.Contains("IsChanged") AndAlso Not IsDBNull(r("IsChanged")),
       CBool(r("IsChanged")),
       False)
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

        Dim Quantity As Decimal = ToDec(r("Quantity"))
        Dim unitPrice As Decimal = ToDec(r("UnitPrice"))
        Dim discountRate As Decimal = ToDec(r("DiscountRate"))
        Dim vatRatePct As Decimal = ToDec(r("TaxRate"))
        Dim includeTax As Boolean = ToBool(r("IncludeTax"))

        Dim rate As Decimal = vatRatePct / 100D

        ' 1️⃣ Gross
        Dim gross As Decimal = Quantity * unitPrice
        r("GrossAmount") = Math.Round(gross, 6)

        ' 2️⃣ Discount
        Dim discountAmount As Decimal = gross * (discountRate / 100D)
        r("DiscountAmount") = Math.Round(discountAmount, 6)

        ' 3️⃣ Taxable
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
            FROM Inventory_DocumentHeader h
            INNER JOIN Workflow_OperationStatusPolicy p
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
            If IsCorrectionDocument(CurrentDocumentID) Then

                service.SendCorrectionPurchase(
    CurrentDocumentID,
    transactionCode,
    CurrentUserID,
    _deletedOriginalDetailIDs
)
            Else

                service.SendPurchase(CurrentDocumentID, transactionCode, CurrentUserID)

            End If
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

        If mode = EditModeType.NoEdit Then
            MessageBox.Show("لا يمكن تعديل هذا السند")
            Exit Sub
        End If

        Try
            ' =========================
            ' Validation
            ' =========================
            If Not ValidateDocument() Then Exit Sub

            ' ✅ تنظيف الجريد قبل التحقق
            NormalizeInvoiceGrid()

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
            ' =========================
            ' استدعاء السيرفس للحفظ
            ' =========================
            Dim service As New PurchaseApplicationService(ConnStr)
            If _isPostedCorrectionEdit Then

                Dim newDocID As Integer =
        service.EditPostedPurchase(
            oldDocumentID:=CurrentDocumentID,
            documentNo:=txtDocumentID.Text,
            documentDate:=dtpDocumentDate.Value,
            partnerID:=CInt(cboPartnerCode.SelectedValue),
            taxTypeID:=CInt(cboVATRate.SelectedValue),
            paymentMethodID:=CInt(cboPaymentMethod.SelectedValue),
            paymentTermID:=CInt(cboPaymentTerm.SelectedValue),
            notes:=txtNote.Text,
            isTaxInclusive:=chkIsTaxInclusive.Checked,
            details:=BuildDocumentDetailsTVP(),
            deletedOriginalDetailIDs:=_deletedOriginalDetailIDs,
            userID:=CurrentUserID
        )

                _isPostedCorrectionEdit = False
                _deletedOriginalDetailIDs.Clear()

                MessageBox.Show("تم حفظ سند التصحيح بنجاح", "تم")
                LoadDocument(newDocID)
                Exit Sub

            End If
            If CurrentDocumentID = 0 Then

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


                If mode = EditModeType.DirectEdit Then

                    ' 🟢 تعديل مباشر
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

                ElseIf mode = EditModeType.EngineEdit Then

                    ' 🔵 تعديل عبر المحرك
                    service.UpdatePurchaseWithTransactionSync(
            documentID:=CurrentDocumentID,
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
                    UPDATE Inventory_DocumentHeader
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

            MessageBox.Show("تم حفظ الفاتورة بنجاح", "تم")
            btnSaveDraft.Text = "تعديل"
        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Function GetDocumentStatusID(documentID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)

            Dim sql As String = "
SELECT StatusID 
FROM Inventory_DocumentHeader
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

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    ProductTypeID,
    TypeCode,
    TypeName
FROM dbo.Master_ProductType
WHERE IsActive = 1
ORDER BY TypeName
", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        Dim col =
        CType(dgvInvoiceDetails.Columns("colProductType"),
              DataGridViewComboBoxColumn)

        col.DataSource = dt
        col.DisplayMember = "TypeName"
        col.ValueMember = "ProductTypeID"

    End Sub
    ' ========================================
    ' فلترة الأنواع حسب الكود
    ' ========================================
    Private Sub FilterProductTypeByCode(rowIndex As Integer)

        Dim row = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim productIDObj = row.Cells("colProductCode").Value
        If productIDObj Is Nothing OrElse IsDBNull(productIDObj) Then Exit Sub

        Dim src =
        CType(CType(dgvInvoiceDetails.Columns("colProductCode"),
              DataGridViewComboBoxColumn).DataSource, DataTable)

        Dim productID As Integer = CInt(productIDObj)

        Dim found = src.Select("ProductID = " & productID)
        If found.Length = 0 Then Exit Sub

        Dim typeID As Integer = CInt(found(0)("ProductTypeID"))

        row.Cells("colProductType").Value = typeID

    End Sub
    ' ========================================
    ' منع الخروج من السطر بدون نوع
    ' ========================================
    Private Sub dgvInvoiceDetails_RowValidating(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvInvoiceDetails.RowValidating

        If IsLoading OrElse IsUIGuarded Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If InvoiceDetailsTable Is Nothing Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        ' 🟢 تجاهل الصفوف الفارغة (مهم)
        Dim productVal = row.Cells("colProductCode").Value
        If productVal Is Nothing OrElse IsDBNull(productVal) Then Exit Sub

        Dim typeVal = row.Cells("colProductType").Value

        If typeVal Is Nothing OrElse IsDBNull(typeVal) Then
            MessageBox.Show("يجب اختيار نوع الصنف")
            e.Cancel = True
        End If

    End Sub
    Private Enum EditModeType
        DirectEdit
        EngineEdit
        NoEdit
    End Enum

    Private Function GetEditMode(statusID As Integer) As EditModeType

        If _isPostedCorrectionEdit Then
            Return EditModeType.DirectEdit
        End If

        Select Case statusID
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
            SELECT StatusID
            FROM Inventory_DocumentHeader
            WHERE DocumentID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    FormStatusID = CInt(result)
                Else
                    Throw New Exception("لم يتم العثور على حالة السند")
                End If

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

        If MessageBox.Show("هل تريد حذف السطر؟", "تأكيد",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        If rowIndex < 0 OrElse rowIndex >= dt.Rows.Count Then Exit Sub

        Dim r As DataRow = dt.Rows(rowIndex)

        If _isPostedCorrectionEdit Then
            If Not IsDBNull(r("OriginalDetailID")) Then
                Dim originalID As Integer = CInt(r("OriginalDetailID"))
                If Not _deletedOriginalDetailIDs.Contains(originalID) Then
                    _deletedOriginalDetailIDs.Add(originalID)
                End If
            End If
        End If

        dt.Rows.RemoveAt(rowIndex)

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

    Private Sub btnEditPostedPurchase_Click(sender As Object, e As EventArgs) Handles btnEditPostedPurchase.Click

        If CurrentDocumentID <= 0 Then
            MessageBox.Show("لا يوجد سند محدد")
            Exit Sub
        End If

        If FormStatusID <> 6 Then
            MessageBox.Show("لا يمكن تعديل سند غير مستلم")
            Exit Sub
        End If

        If MessageBox.Show("سيتم فتح السند بوضع التصحيح، هل تريد المتابعة؟",
                       "تأكيد",
                       MessageBoxButtons.YesNo) <> DialogResult.Yes Then Exit Sub

        Try
            _isPostedCorrectionEdit = True
            _deletedOriginalDetailIDs.Clear()

            ApplyEditPermissionByStatus()
            btnSaveDraft.Text = "حفظ التصحيح"

            MessageBox.Show("تم فتح السند بوضع التصحيح. عدّل ثم اضغط حفظ التصحيح.")

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Sub btnDeletePostedPurchase_Click(sender As Object, e As EventArgs) Handles btnDeletePostedPurchase.Click
        If CurrentDocumentID <= 0 Then
            MessageBox.Show("لا يوجد سند محدد")
            Exit Sub
        End If

        If FormStatusID <> 6 Then
            MessageBox.Show("لا يمكن إلغاء سند غير مستلم")
            Exit Sub
        End If

        If MessageBox.Show("هل تريد إلغاء السند؟",
                       "تأكيد",
                       MessageBoxButtons.YesNo) <> DialogResult.Yes Then Exit Sub

        Try

            _purchaseService.CancelPostedPurchase(
            CurrentDocumentID,
            CurrentUserID
        )

            MessageBox.Show("تم الإلغاء")

            LoadDocument(CurrentDocumentID)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Function IsCorrectionDocument(documentID As Integer) As Boolean

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT ISNULL(IsCorrection,0)
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", documentID)

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then Return False

                Return CBool(result)

            End Using
        End Using

    End Function

End Class



