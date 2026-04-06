Imports System.Data.SqlClient
Imports THE_PROJECT.frmProductSearch

Public Class frmProduction
    Inherits AABaseOperationForm



    ' =========================
    ' BOM المختار
    ' =========================
    Public SelectedBOMID As Integer = 0
    ' =====================================================
    ' Connection String
    ' =====================================================
    Private IsCleaningChemicalInitializing As Boolean = False
    Private LastCleaningProductCode As String = ""
    ' =========================
    ' Variables (Production State)
    ' =========================
    Private CurrentStatusID As Integer = 0
    Private CurrentProductionID As Integer = 0
    Private IsInventoryPosted As Boolean = False
    ' =========================
    ' User Context (مؤقت)
    ' =========================
    Private CurrentUserID As Integer = 1
    Private CurrentProductionStatusID As Integer = 0
    Private IsSaved As Boolean = True
    Private CurrentSubCategoryID As Integer = 0
    Private CurrentMode As FormMode = FormMode.NewMode
    Private IsCleaningManualChange As Boolean = False
    ' =========================
    ' Workflow Context
    ' =========================
    Private Const OPERATION_TYPE_CODE As String = "PRO"
    Private OperationTypeID As Integer = 0
    Private CurrentPolicy As EditPolicy
    Private Sub InitWorkflowContext()

        ' جلب OperationTypeID من الكود (مرة واحدة)
        If OperationTypeID = 0 Then
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT OperationTypeID
                FROM Workflow_OperationType
                WHERE OperationCode = @Code
                  AND IsActive = 1
            ", con)
                    cmd.Parameters.AddWithValue("@Code", OPERATION_TYPE_CODE)
                    con.Open()

                    Dim v = cmd.ExecuteScalar()
                    If v Is Nothing OrElse IsDBNull(v) Then
                        Throw New ApplicationException("OperationType PRO غير معرف")
                    End If

                    OperationTypeID = CInt(v)
                End Using
            End Using
        End If

    End Sub
    Private Sub RefreshWorkflowPolicy()

        If OperationTypeID <= 0 OrElse CurrentStatusID <= 0 Then
            CurrentPolicy = New EditPolicy()
            Exit Sub
        End If

        CurrentPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            OperationTypeID,
            CurrentStatusID
        )

        ApplyUIByWorkflow()

    End Sub

    Private Sub ApplyUIByWorkflow()

        If CurrentProductionID <= 0 Then
            ' سند جديد
            EnableEditing(True)
            btnSave.Enabled = True
            btnExecuteProduction.Enabled = True
            btnCancelProduction.Enabled = False
            Exit Sub
        End If

        ' --- Editing ---
        EnableEditing(CurrentPolicy.AllowEditData)

        ' --- Buttons ---
        btnSave.Enabled = CurrentPolicy.AllowEditData
        btnExecuteProduction.Enabled = CurrentPolicy.IsPostable
        btnCancelProduction.Enabled = CurrentPolicy.IsCancelable

    End Sub
    ' ===== دالة: ApplySubCategoryRules =====
    Private Sub ApplySubCategoryRules()

        ' =========================
        ' حمايات أساسية
        ' =========================
        If CurrentSubCategoryID <= 0 Then Exit Sub

        ' =========================
        ' الإعداد الافتراضي (إسفنج / عام)
        ' =========================
        dgvProduced.Visible = True
        dgvProduced.Enabled = True

        txtProductionAmount.Enabled = True
        txtProductionAmount.ReadOnly = False



        ' =========================
        ' تطبيق حسب SubCategory
        ' =========================
        Select Case CurrentSubCategoryID

            Case 9   ' إسفنج
            ' لا تغيير عن الافتراضي

            Case 10  ' مضغوط
                ' الإنتاج يعتمد على الحجم فقط
                txtProductionAmount.Clear()
                txtProductionAmount.Enabled = False
                txtProductionAmount.ReadOnly = True

            Case 11  ' مراتب
                ' لا نستخدم الجريد السفلي
                dgvProduced.Visible = False
                dgvProduced.Enabled = False

                ' الإنتاج يعتمد على العدد فقط
                txtProductionAmount.Enabled = True
                txtProductionAmount.ReadOnly = False

                ' لا مواد تنظيف


        End Select

    End Sub

    Private Sub UpdateCleaningChemicalInGrid()
        ' =========================
        ' الحماية من التنفيذ أثناء تحميل الفورم
        ' =========================
        If IsFormLoading Then Exit Sub

        ' =========================
        ' تحديد معامل الإنتاج (Factor)
        ' =========================
        Dim factor As Decimal = 0D
        Select Case CurrentSubCategoryID
            Case 9, 11 ' إسفنج أو مراتب
                factor = GetDec(txtProductionAmount.Text)
            Case 10    ' مضغوط
                Dim totalVolume As Decimal = 0D
                For Each rr As DataGridViewRow In dgvProduced.Rows
                    If rr.IsNewRow Then Continue For
                    totalVolume += GetDec(rr.Cells("colManTotalProductVolume").Value)
                Next
                factor = totalVolume
        End Select

        ' =========================
        ' تحديد ID مادة التنظيف الحالية (إن وجدت)
        ' =========================
        Dim currentCleaningProductID As Integer = -1
        If ChkIsCleaningUsed.Checked AndAlso cboCleaningChemical.SelectedValue IsNot Nothing AndAlso IsNumeric(cboCleaningChemical.SelectedValue) Then
            currentCleaningProductID = CInt(cboCleaningChemical.SelectedValue)
        End If

        ' =========================
        ' الخطوة 1: البحث عن أي صف كان يمثل مادة التنظيف سابقاً
        ' =========================
        Dim cleaningRow As DataGridViewRow = Nothing
        Dim cleaningProductIDInGrid As Integer = -1

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            If r.Cells("colCalProductID").Value Is Nothing Then Continue For

            ' هل هذا الصف موسوم بأنه مادة تنظيف؟
            Dim tagValue As String = If(r.Cells("colCalActualQTY").Tag, "").ToString()

            If tagValue.Contains("CLEANING") Then
                ' وجدنا مادة التنظيف القديمة
                cleaningRow = r
                cleaningProductIDInGrid = CInt(r.Cells("colCalProductID").Value)
                Exit For
            End If
        Next

        ' =========================
        ' الخطوة 2: إزالة/إعادة المادة القديمة إلى حالتها الطبيعية
        ' =========================
        If cleaningRow IsNot Nothing Then
            ' هل هذه المادة موجودة أصلاً في BOM؟
            Dim bomQtyBase As Decimal = If(cleaningRow.Cells("colCalBOMQTY").Tag IsNot Nothing,
                                       GetDec(cleaningRow.Cells("colCalBOMQTY").Tag),
                                       GetDec(cleaningRow.Cells("colCalBOMQTY").Value))

            If bomQtyBase > 0D Then
                ' موجودة في BOM: نعيدها إلى حالتها الطبيعية (بدون تنظيف)
                cleaningRow.Cells("colCalActualQTY").Value = bomQtyBase * factor
                cleaningRow.Cells("colCalActualQTY").Tag = Nothing  ' إزالة وسم التنظيف
            Else
                ' غير موجودة في BOM: نحذفها بالكامل
                dgvProductionCalculations.Rows.Remove(cleaningRow)
            End If
        End If

        ' =========================
        ' الخطوة 3: إضافة المادة الجديدة (إذا كان التشك مفعل)
        ' =========================
        If ChkIsCleaningUsed.Checked AndAlso currentCleaningProductID > 0 AndAlso IsNumeric(txtCleaningChemicalQTY.Text) Then

            Dim cleaningQty As Decimal = GetDec(txtCleaningChemicalQTY.Text)
            If cleaningQty <= 0D Then Exit Sub

            ' هل المادة الجديدة موجودة في الجريد (ربما من BOM)؟
            Dim targetRow As DataGridViewRow = Nothing
            For Each r As DataGridViewRow In dgvProductionCalculations.Rows
                If r.IsNewRow Then Continue For
                If CInt(r.Cells("colCalProductID").Value) = currentCleaningProductID Then
                    targetRow = r
                    Exit For
                End If
            Next

            If targetRow IsNot Nothing Then
                ' موجودة: نضيف كمية التنظيف إلى كميتها
                Dim bomQtyBase As Decimal = If(targetRow.Cells("colCalBOMQTY").Tag IsNot Nothing,
                                           GetDec(targetRow.Cells("colCalBOMQTY").Tag),
                                           GetDec(targetRow.Cells("colCalBOMQTY").Value))

                Dim baseActual As Decimal = bomQtyBase * factor
                targetRow.Cells("colCalActualQTY").Value = baseActual + cleaningQty
                targetRow.Cells("colCalActualQTY").Tag = "CLEANING_ADDED"
            Else
                ' غير موجودة: نضيف سطر جديد
                Dim drv As DataRowView = TryCast(cboCleaningChemical.SelectedItem, DataRowView)
                If drv Is Nothing Then Exit Sub

                Dim rowIndex As Integer = dgvProductionCalculations.Rows.Add()
                With dgvProductionCalculations.Rows(rowIndex)
                    .Cells("colCalProductID").Value = currentCleaningProductID
                    .Cells("colCalProductCode").Value = drv("ProductCode").ToString()
                    .Cells("colCalProductName").Value = drv("ProductName").ToString()
                    .Cells("colCalProductUnit").Value = txtCleaningChemicalUnit.Text
                    .Cells("colCalBOMQTY").Value = 0D
                    .Cells("colCalBOMQTY").Tag = 0D
                    .Cells("colCalActualQTY").Value = cleaningQty
                    .Cells("colCalActualQTY").Tag = "CLEANING_ONLY"
                    .Cells("colCalAvailableStock").Value = Nothing
                    .Cells("colCalCost").Value = Nothing
                End With
            End If
        End If

        ' =========================
        ' الخطوة 4: تحديث التكاليف والمخزون
        ' =========================
        FillAvailableStockInGrid()
        RecalculateChemicalCost()
        RecalculateProductionTotals()
        MarkAsDirty()

    End Sub
    ' ===== خطأ 1: حدث CheckBox لا يضبط IsCleaningManualChange =====
    Private Sub ChkIsCleaningUsed_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles ChkIsCleaningUsed.CheckedChanged

        If IsFormLoading Then Exit Sub

        ' تطبيق حالة التمكين للعناصر
        cboCleaningChemical.Enabled = ChkIsCleaningUsed.Checked
        txtCleaningChemicalQTY.Enabled = ChkIsCleaningUsed.Checked
        txtCleaningChemicalUnit.Enabled = False  ' للعرض فقط

        ' إذا تم إلغاء التشك، ننظف الحقول
        If Not ChkIsCleaningUsed.Checked Then
            cboCleaningChemical.SelectedIndex = -1
            txtCleaningChemicalQTY.Clear()
            txtCleaningChemicalUnit.Clear()
        End If

        ' استدعاء المدير المركزي لتحديث الجريد
        ResetManualOverridesToAuto()
        UpdateCleaningChemicalInGrid()

    End Sub
    Private Sub ApplyCleaningUIState()

        ' منع التنظيف للمراتب (SubCategoryID = 11)
        Dim allowCleaning As Boolean = (CurrentSubCategoryID <> 11)

        ' إذا كان مسموحاً بالتنظيف، نعتمد على حالة التشك
        If allowCleaning Then
            cboCleaningChemical.Enabled = ChkIsCleaningUsed.Checked
            txtCleaningChemicalQTY.Enabled = ChkIsCleaningUsed.Checked
            txtCleaningChemicalUnit.Enabled = False   ' عرض فقط
        Else
            ' غير مسموح بالتنظيف إطلاقاً
            cboCleaningChemical.Enabled = False
            txtCleaningChemicalQTY.Enabled = False
            txtCleaningChemicalUnit.Enabled = False

            ' إذا كان المنتج من نوع مراتب، نلغي أي تنظيف سابق
            If CurrentSubCategoryID = 11 AndAlso ChkIsCleaningUsed.Checked Then
                ChkIsCleaningUsed.Checked = False
                cboCleaningChemical.SelectedIndex = -1
                txtCleaningChemicalQTY.Clear()
                txtCleaningChemicalUnit.Clear()
                UpdateCleaningChemicalInGrid()  ' إزالة من الجريد
            End If
        End If

    End Sub
    Private Sub ApplyCleaningUsageState()

        If ChkIsCleaningUsed.Checked Then

            cboCleaningChemical.Enabled = True
            txtCleaningChemicalQTY.Enabled = True
            txtCleaningChemicalUnit.Enabled = True

            If cboCleaningChemical.SelectedValue Is Nothing Then
                LoadLastCleaningChemical()
            End If


        Else
            cboCleaningChemical.SelectedIndex = -1
            txtCleaningChemicalQTY.Clear()
            txtCleaningChemicalUnit.Clear()

            cboCleaningChemical.Enabled = False
            txtCleaningChemicalQTY.Enabled = False
            txtCleaningChemicalUnit.Enabled = False


        End If

    End Sub
    Private Sub ClearProductionTotals()

        txtTotalProductionVolume.Text = "0"
        txtTotalProductionQTY.Text = "0"
        txtTotalChemicalConsumption.Text = "0"
        txtTotalProductionCost.Text = "0"
        txtProductUnitCost.Text = "0"
        txtDeviation.Text = "0"
        txtPastProductAverageCost.Text = "0"

    End Sub


    ' ===== دالة: ResetForNewProduction =====
    Private Sub ResetForNewProduction()

        IsFormLoading = True

        ' =========================
        ' 🔥 تصفير الحقيقة الداخلية
        ' =========================
        CurrentProductionID = 0
        CurrentProductionStatusID = 0
        CurrentStatusID = 0
        IsInventoryPosted = False
        IsSaved = True

        SelectedProductID = 0
        CurrentBOMID = 0
        CurrentSubCategoryID = 0

        ' =========================
        ' تصفير حقول الهيدر
        ' =========================
        txtProductionCode.Clear()
        txtNotes.Clear()
        txtProductionAmount.Clear()

        cboProductID.SelectedIndex = -1
        cboBOMVersion.DataSource = Nothing
        cboBOMVersion.Items.Clear()

        cboProductionStatus.SelectedIndex = -1
        SetDefaultProductionStatus()
        txtProductName.Clear()
        txtBaseUnitID.Clear()
        txtCategoryID.Clear()
        txtProductGroupID.Clear()
        txtSubCategory.Clear()
        txtProductionUnit.Clear()
        txtPastProductAverageCost.Text = "0"
        txtProductUnitCost.Text = "0"
        txtDeviation.Text = "0"

        ' =========================
        ' الجريدات
        ' =========================
        dgvProduced.Rows.Clear()
        dgvProductionCalculations.Rows.Clear()

        ' =========================
        ' مواد التنظيف (نظيف – بدون تحميل تاريخي)
        ' =========================
        ChkIsCleaningUsed.Checked = False
        cboCleaningChemical.SelectedIndex = -1
        txtCleaningChemicalQTY.Clear()
        txtCleaningChemicalUnit.Clear()

        ' =========================
        ' إعادة تمكين التحرير
        ' =========================
        EnableEditing(True)
        ApplyCleaningUIState()

        IsFormLoading = False

    End Sub
    Private Sub ApplyGridVisualStyle()

        ' =========================
        ' إعدادات عامة
        ' =========================
        Dim altBackColor As Color = Color.FromArgb(245, 246, 248)   ' رمادي فاتح رسمي
        Dim headerBackColor As Color = Color.FromArgb(230, 232, 235)
        Dim gridBackColor As Color = Color.White
        Dim gridLineColor As Color = Color.FromArgb(210, 210, 210)

        ' =========================
        ' تنسيق dgvProduced
        ' =========================
        With dgvProduced
            .EnableHeadersVisualStyles = False
            .BackgroundColor = gridBackColor
            .GridColor = gridLineColor
            .AlternatingRowsDefaultCellStyle.BackColor = altBackColor

            .ColumnHeadersDefaultCellStyle.BackColor = headerBackColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font(.Font, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        ' أرقام الإنتاج (عرض برقم عشري واحد)
        Dim producedNumericCols As String() = {
        "colManLength",
        "colManWidth",
        "colManHeight",
        "colManProductVolume",
        "colManTotalProductVolume",
        "colManQTY"
    }

        For Each colName In producedNumericCols
            If dgvProduced.Columns.Contains(colName) Then
                With dgvProduced.Columns(colName)
                    .DefaultCellStyle.Format = "N2"
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End With
            End If
        Next

        ' =========================
        ' تنسيق dgvProductionCalculations
        ' =========================
        With dgvProductionCalculations
            .EnableHeadersVisualStyles = False
            .BackgroundColor = gridBackColor
            .GridColor = gridLineColor
            .AlternatingRowsDefaultCellStyle.BackColor = altBackColor

            .ColumnHeadersDefaultCellStyle.BackColor = headerBackColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font(.Font, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        ' أعمدة الحسابات (عرض برقم عشري واحد – الحساب الداخلي يبقى 6)
        Dim calcNumericCols As String() = {
        "colCalBOMQTY",
        "colCalActualQTY",
        "colCalAvailableStock",
        "colCalCost"
    }

        For Each colName In calcNumericCols
            If dgvProductionCalculations.Columns.Contains(colName) Then
                With dgvProductionCalculations.Columns(colName)
                    .DefaultCellStyle.Format = "N2"
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End With
            End If
        Next

    End Sub
    ' ===== دالة: InitOnLoad =====
    Private Sub InitOnLoad()

        ' =========================
        ' هذه الدالة أصبحت مجرد غلاف
        ' لا تقوم بأي تحميل أو Reset
        ' الاعتماد الآن على frmProductionNew_Load فقط
        ' =========================

        ' ❌ تم إلغاء أي منطق سابق هنا عمداً
        ' ❌ لا تحميل بيانات
        ' ❌ لا Reset
        ' ❌ لا تغيير حالة

        ' إبقاؤها فارغة لتجنب كسر أي استدعاء قديم
        ' وسيتم حذفها لاحقًا بعد اكتمال التنقيح

    End Sub
    ' ===== دالة: LoadProductionSafe =====
    Private Sub LoadProductionSafe(productionID As Integer)

        ' =========================
        ' حمايات أساسية
        ' =========================
        If productionID <= 0 Then Exit Sub

        ' =========================
        ' قفل الأحداث
        ' =========================
        IsFormLoading = True

        ' =========================
        ' تحميل الإنتاج من مصدر واحد
        ' =========================
        LoadProduction(productionID)

        ' =========================
        ' فتح الأحداث
        ' =========================
        IsFormLoading = False

    End Sub
    ' ===== دالة: MarkAsDirty =====
    Private Sub MarkAsDirty()

        If IsFormLoading Then Exit Sub
        IsSaved = False

    End Sub
    Private Sub AnyTextBox_TextChanged(sender As Object, e As EventArgs) _
    Handles txtNotes.TextChanged,
            txtProductionAmount.TextChanged

        MarkAsDirty()

    End Sub
    Private Sub AnyCombo_SelectedValueChanged(sender As Object, e As EventArgs) _
    Handles cboProductID.SelectedValueChanged,
            cboBOMVersion.SelectedValueChanged,
            cboSourceStore.SelectedValueChanged,
            cboTargetStore.SelectedValueChanged

        MarkAsDirty()

    End Sub
    Private Sub dgvProductionCalculations_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProductionCalculations.CellValueChanged

        If IsFormLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName = dgvProductionCalculations.Columns(e.ColumnIndex).Name
        If colName = "colCalActualQTY" Then
            Dim cell = dgvProductionCalculations.Rows(e.RowIndex).Cells("colCalActualQTY")
            Dim tagStr As String = If(cell.Tag, "AUTO").ToString()

            If Not tagStr.StartsWith("CLEANING") Then
                cell.Tag = "MANUAL"
            End If
        End If
        MarkAsDirty()
    End Sub
    ' ===== دالة: EnableEditing =====
    Private Sub EnableEditing(canEdit As Boolean)

        ' =========================
        ' حمايات أساسية
        ' =========================
        If IsFormLoading Then Exit Sub

        ' ⭐⭐⭐ في وضع التعديل، نسمح بالتحرير إذا كان canEdit = True والحالة تسمح ⭐⭐⭐
        Dim finalCanEdit As Boolean = canEdit AndAlso (Not IsEditMode OrElse (IsEditMode AndAlso CurrentStatusID = 2))
        ' =========================
        ' حقول الهيدر
        ' =========================
        cboProductID.Enabled = canEdit
        cboBOMVersion.Enabled = canEdit
        cboSourceStore.Enabled = canEdit
        cboTargetStore.Enabled = canEdit

        txtProductionAmount.ReadOnly = Not canEdit
        txtNotes.ReadOnly = Not canEdit

        ' =========================
        ' الجريدات (ReadOnly فقط)
        ' =========================
        dgvProduced.ReadOnly = Not canEdit
        dgvProductionCalculations.ReadOnly = Not canEdit


    End Sub

    Private Sub frmProductionNew_Load(
    sender As Object,
    e As EventArgs
) Handles MyBase.Load

        Dim screenW As Integer = Screen.PrimaryScreen.WorkingArea.Width
        Dim screenH As Integer = Screen.PrimaryScreen.WorkingArea.Height

        Dim newWidth As Integer = CInt(screenW * 0.95)
        Dim newHeight As Integer = CInt(screenH * 0.95)

        ' حساب موقع المركز
        Dim newX As Integer = (screenW - newWidth) \ 2
        Dim newY As Integer = (screenH - newHeight) \ 2

        ' تعيين الموقع والحجم مباشرة
        Me.SetBounds(newX, newY, newWidth, newHeight)

        IsFormLoading = True

        ' =========================
        ' تحميل البيانات الثابتة (مرة واحدة فقط)
        ' =========================
        LoadProducts()
        LoadStores()
        LoadCleaningMaterials()
        LoadProductionStatusCombo()
        btnSave.Text = "حفظ"
        ' =========================
        ' تنسيق الجريدات
        ' =========================
        ApplyGridVisualStyle()
        FormatProductionGridColumns()
        FormatFormNumbers()
        ' =========================
        ' تهيئة فورم إنتاج جديد
        ' =========================
        ResetForNewProduction()
        InitWorkflowContext()

        ' =========================
        ' ربط الأحداث (بعد اكتمال التحميل)
        ' =========================
        RemoveHandler cboProductID.SelectedValueChanged, AddressOf cboProductID_SelectedValueChanged
        AddHandler cboProductID.SelectedValueChanged, AddressOf cboProductID_SelectedValueChanged

        RemoveHandler cboBOMVersion.SelectedValueChanged, AddressOf cboBOMVersion_SelectedValueChanged
        AddHandler cboBOMVersion.SelectedValueChanged, AddressOf cboBOMVersion_SelectedValueChanged

        RemoveHandler btnProductSearch.Click, AddressOf btnProductSearch_Click
        AddHandler btnProductSearch.Click, AddressOf btnProductSearch_Click

        RemoveHandler btnSearchBOM.Click, AddressOf btnSearchBOM_Click
        AddHandler btnSearchBOM.Click, AddressOf btnSearchBOM_Click

        RemoveHandler btnClose.Click, AddressOf btnClose_Click
        AddHandler btnClose.Click, AddressOf btnClose_Click

        ' =========================
        ' تحميل آخر إعدادات تنظيف للمستخدم
        ' =========================
        LoadLastCleaningChemical()

        ' =========================
        ' فتح الأحداث
        ' =========================
        IsFormLoading = False

        ' =========================
        ' تثبيت وضع الفورم
        ' =========================
        SetFormMode(FormMode.NewMode)

        ' =========================
        ' تركيز مبدئي
        ' =========================
        cboProductID.Focus()

    End Sub

    ' =========================
    ' Variables
    ' =========================
    Private SelectedProductID As Integer = 0
    Private CurrentBOMID As Integer = 0
    Private IsFormLoading As Boolean = False

    ' =========================
    ' Form Mode
    ' =========================
    Private Enum FormMode
        NewMode
        ViewMode
    End Enum
    Private Sub SetDefaultProductionStatus()

        ' PRO_NEW = 37 (حسب ما أدخلناه)
        cboProductionStatus.SelectedValue = 37

    End Sub


    ' =====================================================
    ' إدارة وضع الفورم (New / View)
    ' - التحكم بسلوك العناصر حسب الحالة
    ' =====================================================
    ' ===== دالة: SetFormMode =====
    Private Sub SetFormMode(mode As FormMode)

        ' =========================
        ' تثبيت وضع الفورم فقط
        ' بدون تحميل / Reset / منطق جانبي
        ' =========================
        CurrentMode = mode

        Select Case mode

            Case FormMode.NewMode
                cboProductID.Enabled = True
                cboBOMVersion.Enabled = True

            Case FormMode.ViewMode
                cboProductID.Enabled = True
                cboBOMVersion.Enabled = True

        End Select

    End Sub
    ' =====================================================
    ' تحميل الأصناف الفعّالة وربطها مع cboProductID
    ' =====================================================
    Private Sub LoadProducts()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT 
            ProductID,
            ProductCode,
            ProductName,
            StorageUnitID,
            ProductGroupID,
            ProductCategoryID,
            ProductSubCategoryID
         FROM Master_Product
         WHERE IsActive = 1
         ORDER BY ProductCode", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductID.DataSource = dt
        cboProductID.DisplayMember = "ProductCode"
        cboProductID.ValueMember = "ProductID"
        cboProductID.SelectedIndex = -1

    End Sub
    Private Sub LoadStores()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT StoreID, StoreName
             FROM Master_Store
             WHERE IsActive = 1
             ORDER BY StoreName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        ' =========================
        ' مستودع المواد الخام
        ' =========================
        cboSourceStore.DataSource = dt.Copy()
        cboSourceStore.DisplayMember = "StoreName"
        cboSourceStore.ValueMember = "StoreID"
        cboSourceStore.SelectedIndex = -1

        ' ✅ افتراضي: Source = 1
        If dt.Select("StoreID = 1").Length > 0 Then
            cboSourceStore.SelectedValue = 1
        End If

        ' =========================
        ' مستودع المنتج النهائي
        ' =========================
        cboTargetStore.DataSource = dt
        cboTargetStore.DisplayMember = "StoreName"
        cboTargetStore.ValueMember = "StoreID"
        cboTargetStore.SelectedIndex = -1

        ' ✅ افتراضي: Target = 3
        If dt.Select("StoreID = 3").Length > 0 Then
            cboTargetStore.SelectedValue = 3
        End If

    End Sub

    ' =====================================================
    ' تحميل وحدة مادة التنظيف بناءً على الصنف المختار
    ' (Product.BaseUnitID → Unit.ID)
    ' =====================================================
    Private Sub LoadCleaningChemicalUnit(productID As Integer)

        If productID <= 0 Then
            txtCleaningChemicalUnit.Clear()
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT u.UnitName
         FROM Master_Product p
         INNER JOIN Master_Unit u
             ON u.UnitID = p.StorageUnitID
         WHERE p.ProductID = @ProductID", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result IsNot Nothing Then
                    txtCleaningChemicalUnit.Text = result.ToString()
                Else
                    txtCleaningChemicalUnit.Clear()
                End If

            End Using
        End Using

    End Sub


    ' =====================================================
    ' عند اختيار منتج من cboProductID
    ' =====================================================
    ' ===== دالة: cboProductID_SelectedValueChanged =====
    ' ===== دالة: cboProductID_SelectedValueChanged =====
    Private Sub cboProductID_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboProductID.SelectedValueChanged

        If IsFormLoading Then Exit Sub
        If cboProductID.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboProductID.SelectedValue) Then Exit Sub

        SelectedProductID = CInt(cboProductID.SelectedValue)

        Dim drv As DataRowView = TryCast(cboProductID.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        txtProductName.Text = drv("ProductName").ToString()
        txtBaseUnitID.Text = GetUnitNameByID(CInt(drv("StorageUnitID")))

        txtCategoryID.Text = GetCategoryName(CInt(drv("ProductCategoryID")))
        txtProductGroupID.Text = GetProductGroupName(CInt(drv("ProductGroupID")))

        ' ✅ متوسط سعر سابق للمنتج المنتج (المادة المنتجة)
        LoadPastProductAverageCost(SelectedProductID)

        ' ✅ تحميل BOM (آخر Active) + جريد الحسابات
        LoadBOMVersions(SelectedProductID)

        ' ✅ تحديث المخزون في الجريد
        FillAvailableStockInGrid()

        ' ✅ وحدة الإنتاج + SubCategory
        LoadProductionUnitFromProduct(SelectedProductID)
        LoadSubCategoryFromProduct(SelectedProductID)

        ' ✅ ضبط واجهة مادة التنظيف (بعد تطبيق SubCategory)
        ApplyCleaningUIState()
        ' داخل cboProductID_SelectedValueChanged بعد LoadSubCategoryFromProduct
        RecalculateTotalProductionValue()

        RecalculateActualQuantities()

        RecalculateChemicalCost()
        RecalculateProductionTotals()
        UpdateDeviation()

    End Sub
    Private Sub LoadPastProductAverageCost(productID As Integer)

        If productID <= 0 Then
            txtPastProductAverageCost.Text = "0"
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT TOP 1 
                AvgCostPerM3
            FROM dbo.Master_FinalProductAvgCost
            WHERE BaseProductID = @ProductID
            ORDER BY LastUpdated DESC
        ", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso IsNumeric(result) Then
                    txtPastProductAverageCost.Text = CDec(result).ToString("N2")
                Else
                    txtPastProductAverageCost.Text = "0"
                End If
            End Using
        End Using

    End Sub
    ' ===== دالة: LoadSubCategoryFromProduct =====
    Private Sub LoadSubCategoryFromProduct(productID As Integer)

        CurrentSubCategoryID = 0
        txtSubCategory.Clear()

        If productID <= 0 Then
            ApplySubCategoryRules()
            Exit Sub
        End If
        ApplyCleaningUIState()
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT 
                sc.SubcategoryID,
                sc.SubcategoryName
            FROM Master_Product p
            INNER JOIN Master_SubCategory sc
                ON sc.SubcategoryID = p.ProductSubCategoryID
            WHERE p.ProductID = @ProductID
        ", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                con.Open()

                Using r = cmd.ExecuteReader()
                    If r.Read() Then
                        CurrentSubCategoryID = CInt(r("SubCategoryID"))
                        txtSubCategory.Text = r("SubcategoryName").ToString()
                    End If
                End Using

            End Using
        End Using

        ' =========================
        ' تطبيق القواعد من مكان واحد فقط
        ' =========================
        ApplySubCategoryRules()

    End Sub


    ' ===== دالة: LoadBOMVersions =====
    ' ===== دالة: LoadBOMVersions =====
    Private Sub LoadBOMVersions(productID As Integer)

        If productID <= 0 Then Exit Sub

        IsFormLoading = True   ' 🔒 إيقاف الأحداث مؤقتًا

        Try

            CurrentBOMID = 0
            txtBOMID.Clear()

            cboBOMVersion.DataSource = Nothing
            cboBOMVersion.Items.Clear()

            Dim dt As New DataTable()

            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand(
"
SELECT TOP 1
    BOMID,
    VersionNo,
    BOMCode,
    IsActive
FROM Production_BOMHeader
WHERE ProductID = @ProductID
  AND IsActive = 1
ORDER BY VersionNo DESC
", con)

                    cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID

                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using

                End Using
            End Using

            ' ❌ لا يوجد BOM Active
            If dt.Rows.Count = 0 Then
                Exit Sub
            End If

            ' ✅ ربط الكمبو
            cboBOMVersion.DataSource = dt
            cboBOMVersion.DisplayMember = "VersionNo"
            cboBOMVersion.ValueMember = "BOMID"

            ' ✅ اختيار آخر BOM Active صراحة
            Dim row As DataRow = dt.Rows(0)
            CurrentBOMID = CInt(row("BOMID"))

            cboBOMVersion.SelectedValue = CurrentBOMID
            txtBOMID.Text = row("BOMCode").ToString()

            ' ✅ تحميل الجريد يدويًا (لا نعتمد على الحدث)
            LoadBOMCalculationGrid(CurrentBOMID)
            FillAvailableStockInGrid()

            ' ✅ تحميل حالة Active مباشرة
            RecalculateTotalProductionValue()
            RecalculateActualQuantities()
            RecalculateChemicalCost()
            RecalculateProductionTotals()

        Finally
            IsFormLoading = False  ' 🔓 إعادة تفعيل الأحداث
        End Try

    End Sub
    ' ===== دالة: cboBOMVersion_SelectedValueChanged =====
    Private Sub cboBOMVersion_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboBOMVersion.SelectedValueChanged

        ' =========================
        ' حمايات أساسية
        ' =========================
        If IsFormLoading Then Exit Sub
        If cboBOMVersion.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboBOMVersion.SelectedValue) Then Exit Sub

        ' =========================
        ' تثبيت الـ BOM المختار
        ' =========================
        CurrentBOMID = CInt(cboBOMVersion.SelectedValue)

        Dim drv As DataRowView = TryCast(cboBOMVersion.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        ' =========================
        ' عرض بيانات الـ BOM
        ' =========================
        txtBOMID.Text = drv("BOMCode").ToString()
        ChkBOMIsActive.Checked = CBool(drv("IsActive"))

        ' =========================
        ' تحميل الحسابات والمخزون
        ' =========================
        LoadBOMCalculationGrid(CurrentBOMID)
        FillAvailableStockInGrid()
        RecalculateTotalProductionValue()
        RecalculateActualQuantities()
        RecalculateChemicalCost()
        RecalculateProductionTotals()

    End Sub

    ' =====================================================
    ' عند تغيير إصدار BOM
    ' =====================================================
    ' ===== دالة: cboBOMVersion_SelectedValueChanged =====

    ' =====================================================
    ' البحث عن منتج وربطه بفورم الإنتاج
    ' =====================================================
    ' ===== دالة: btnProductSearch_Click =====
    Private Sub btnProductSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnProductSearch.Click

        Using f As New frmProductSearch()

            ' 👇 التعديل هنا
            f.SearchFilter = ProductSearchFilter.HasBOM

            If f.ShowDialog() <> DialogResult.OK Then Exit Sub

            BindProductForProduction(f.SelectedProductID)
            SetFormMode(FormMode.NewMode)

        End Using

    End Sub


    Private Sub BindProductForProduction(productID As Integer)

        ' =========================
        ' حمايات أساسية
        ' =========================
        If productID <= 0 Then Exit Sub

        ' =========================
        ' قفل الأحداث
        ' =========================
        IsFormLoading = True

        ' =========================
        ' تصفير الفورم بالكامل
        ' (مصدر واحد للحقيقة)
        ' =========================
        ResetForNewProduction()

        ' =========================
        ' تثبيت المنتج المختار
        ' =========================
        SelectedProductID = productID

        ' =========================
        ' تحميل المنتجات (لضمان وجوده في الكمبو)
        ' =========================
        LoadProducts()

        ' =========================
        ' اختيار المنتج
        ' =========================
        cboProductID.SelectedValue = productID

        ' =========================
        ' فتح الأحداث
        ' =========================
        IsFormLoading = False

        ' =========================
        ' تشغيل منطق اختيار المنتج يدويًا
        ' =========================
        cboProductID_SelectedValueChanged(cboProductID, EventArgs.Empty)

        ' =========================
        ' تثبيت وضع الفورم
        ' =========================
        SetFormMode(FormMode.NewMode)

        IsSaved = False

    End Sub

    ' =====================================================
    ' البحث عن BOM وربطه مباشرة بفورم الإنتاج
    ' =====================================================
    ' ===== دالة: btnSearchBOM_Click =====
    Private Sub btnSearchBOM_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnSearchBOM.Click

        Using f As New frmBOMSearch()

            If f.ShowDialog() <> DialogResult.OK Then Exit Sub

            Dim productID As Integer = 0

            ' جلب ProductID المرتبط بالـ BOM المختار
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand(
                    "SELECT ProductID
                 FROM Production_BOMHeader
                 WHERE BOMID = @BOMID", con)

                    cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = f.SelectedBOMID
                    con.Open()

                    Dim result = cmd.ExecuteScalar()
                    If result Is Nothing OrElse Not IsNumeric(result) Then Exit Sub

                    productID = CInt(result)
                End Using
            End Using

            ' ربط المنتج المختار مع فورم الإنتاج
            BindProductForProduction(productID)
            LoadProductionUnitFromProduct(SelectedProductID)

            ' تثبيت الـ BOM المختار
            CurrentBOMID = f.SelectedBOMID
            cboBOMVersion.SelectedValue = CurrentBOMID

            ' تثبيت وضع العرض
            SetFormMode(FormMode.ViewMode)

            FillAvailableStockInGrid()

        End Using

    End Sub

    Private Sub LoadCleaningMaterials()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT 
                ProductID,
                ProductCode,
                ProductName,
                StorageUnitID
             FROM Master_Product
             WHERE IsActive = 1
             ORDER BY ProductCode", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboCleaningChemical.DataSource = dt
        cboCleaningChemical.DisplayMember = "ProductName"
        cboCleaningChemical.ValueMember = "ProductID"
        cboCleaningChemical.SelectedIndex = -1

        ' ✅ الافتراضي: كمية التنظيف = 20
        If String.IsNullOrWhiteSpace(txtCleaningChemicalQTY.Text) Then
            txtCleaningChemicalQTY.Text = "20"
        End If

        ' ✅ الافتراضي: مادة التنظيف ProductID = 164
        If dt.Select("ProductID = 164").Length > 0 Then
            cboCleaningChemical.SelectedValue = 164
            LoadCleaningChemicalUnit(164)
        End If

    End Sub

    Private ReadOnly AllowedTabColumns As String() = {
    "colCalBOMQTY",
    "colCalActualQTY"
}

    ' ===== دالة: dgvProductionCalculations_CellEnter =====
    Private Sub dgvProductionCalculations_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProductionCalculations.CellEndEdit

        If IsFormLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String = dgvProductionCalculations.Columns(e.ColumnIndex).Name
        If colName = "colCalActualQTY" Then

            Dim cell = dgvProductionCalculations.Rows(e.RowIndex).Cells("colCalActualQTY")
            Dim tagStr As String = If(cell.Tag, "AUTO").ToString()

            ' ✅ إذا ليس تنظيف: اعتبره تعديل يدوي وثبّته
            If Not tagStr.StartsWith("CLEANING") Then
                cell.Tag = "MANUAL"
            End If
        End If

        ' ✅ بعد التحرير: نعيد حساب التكلفة/الإجمالي فقط (بدون أن يغيّر MANUAL)
        RecalculateChemicalCost()
        RecalculateProductionTotals()
        UpdateDeviation()
        MarkAsDirty()

    End Sub
    Private Sub ResetManualOverridesToAuto()
        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            Dim cell = r.Cells("colCalActualQTY")
            Dim tagStr As String = If(cell.Tag, "AUTO").ToString()

            ' لا نلمس سطور التنظيف
            If tagStr.StartsWith("CLEANING") Then Continue For

            ' رجّعها AUTO لتُحسب من BOM
            cell.Tag = "AUTO"
        Next
    End Sub

    ' ===== خطأ 4: LoadLastCleaningChemical يغير الكمبو بدون حماية =====
    Private Sub LoadLastCleaningChemical()

        Try
            IsFormLoading = True

            Using con As New SqlConnection(ConnStr)
                ' نجلب آخر استخدام للمواد في Production_Consumption
                Using cmd As New SqlCommand("
                SELECT TOP 1
                    c.ComponentProductID,
                    c.ActualConsumedQty,
                    p.ProductCode,
                    p.ProductName,
                    p.StorageUnitID
                FROM Production_Consumption c
                INNER JOIN Production_Header h ON h.ProductionID = c.ProductionID
                INNER JOIN Master_Product p ON p.ProductID = c.ComponentProductID
                WHERE h.CreatedByUserID = @UserID  -- آخر استخدام لهذا المستخدم
                ORDER BY c.ConsumptionID DESC
            ", con)

                    cmd.Parameters.AddWithValue("@UserID", CurrentUserID)
                    con.Open()

                    Using dr = cmd.ExecuteReader()
                        If dr.Read() Then
                            ' ✅ وجدنا آخر مادة استخدمها
                            Dim productID As Integer = CInt(dr("ComponentProductID"))
                            Dim qty As Decimal = CDec(dr("ActualConsumedQty"))

                            ' تعيين القيم في الواجهة
                            cboCleaningChemical.SelectedValue = productID
                            txtCleaningChemicalQTY.Text = qty.ToString("N0")

                            ' تحميل الوحدة
                            LoadCleaningChemicalUnit(productID)

                            ' تفعيل CheckBox (سيؤدي هذا إلى استدعاء UpdateCleaningChemicalInGrid)
                            ChkIsCleaningUsed.Checked = True
                        Else
                            ' ❌ لا يوجد استخدام سابق → نستخدم القيم الافتراضية
                            SetDefaultCleaningChemical()
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            ' في حالة حدوث خطأ، نستخدم الافتراضي
            SetDefaultCleaningChemical()
        Finally
            IsFormLoading = False
        End Try

    End Sub
    Private Sub SetDefaultCleaningChemical()

        ' ✅ الافتراضي: كمية التنظيف = 20
        txtCleaningChemicalQTY.Text = "20"

        ' ✅ الافتراضي: مادة التنظيف ProductID = 164
        Dim defaultProductID As Integer = 164

        ' التأكد من وجود المنتج 164 في مصدر البيانات
        Dim found As Boolean = False
        For Each item In cboCleaningChemical.Items
            Dim drv As DataRowView = TryCast(item, DataRowView)
            If drv IsNot Nothing AndAlso CInt(drv("ProductID")) = defaultProductID Then
                cboCleaningChemical.SelectedValue = defaultProductID
                LoadCleaningChemicalUnit(defaultProductID)
                found = True
                Exit For
            End If
        Next

        ' إذا لم يوجد 164، اختر أول منتج في القائمة
        If Not found AndAlso cboCleaningChemical.Items.Count > 0 Then
            Dim firstItem As DataRowView = TryCast(cboCleaningChemical.Items(0), DataRowView)
            If firstItem IsNot Nothing Then
                cboCleaningChemical.SelectedValue = CInt(firstItem("ProductID"))
                LoadCleaningChemicalUnit(CInt(firstItem("ProductID")))
            End If
        End If

        ' تفعيل CheckBox (سيؤدي إلى استدعاء UpdateCleaningChemicalInGrid)
        ChkIsCleaningUsed.Checked = True

    End Sub

    ' =====================================================
    ' تحميل مكونات BOM المرتبطة بالمنتج المختار
    ' (عرض فقط – بدون حسابات)
    ' =====================================================
    Private Sub LoadBOMCalculationGrid(bomID As Integer)

        dgvProductionCalculations.Rows.Clear()

        If bomID <= 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT
                p.ProductID,
                p.ProductCode,
                p.ProductName,
                u.UnitName,
                d.Quantity
             FROM Production_BOMDetails d
             INNER JOIN Master_Product p
                 ON p.ProductID = d.ComponentProductID
             INNER JOIN Master_Unit u
                 ON u.UnitID = d.UnitID
             WHERE d.BOMID = @BOMID
             ORDER BY d.LineNumber", con)

                cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = bomID
                con.Open()

                Using dr = cmd.ExecuteReader()
                    While dr.Read()

                        Dim bomQty As Decimal = CDec(dr("Quantity"))
                        Dim rowIndex As Integer = dgvProductionCalculations.Rows.Add()

                        With dgvProductionCalculations.Rows(rowIndex)
                            .Cells("colCalProductID").Value = CInt(dr("ProductID"))
                            .Cells("colCalProductCode").Value = dr("ProductCode").ToString()
                            .Cells("colCalProductName").Value = dr("ProductName").ToString()
                            .Cells("colCalProductUnit").Value = dr("UnitName").ToString()

                            ' ⭐ كمية BOM الأساسية
                            .Cells("colCalBOMQTY").Value = bomQty
                            ' ⭐⭐ الأهم: حفظها في Tag كمرجع ⭐⭐
                            .Cells("colCalBOMQTY").Tag = bomQty

                            ' ⭐ الكمية الفعلية الافتراضية = BOM
                            .Cells("colCalActualQTY").Value = bomQty
                            ' ⭐ Tag التنظيف يكون Nothing لأنها ليست مادة تنظيف
                            .Cells("colCalActualQTY").Tag = "AUTO"

                            .Cells("colCalAvailableStock").Value = Nothing
                            .Cells("colCalCost").Value = Nothing
                        End With
                    End While
                End Using
            End Using
        End Using
        RecalculateTotalProductionValue()
        RecalculateActualQuantities()
        RecalculateChemicalCost()
        RecalculateProductionTotals()

    End Sub

    Private Function GetProductionFactor() As Decimal
        Select Case CurrentSubCategoryID
            Case 9, 11
                Return GetDec(txtProductionAmount.Text)

            Case 10
                Return GetDec(txtTotalProductionVolume.Text)
        End Select
        Return 0D
    End Function
    ' ===== خطأ 2: SelectedValueChanged مقفول دائمًا بسبب IsCleaningManualChange =====
    Private Sub cboCleaningChemical_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboCleaningChemical.SelectedValueChanged

        If IsFormLoading Then Exit Sub
        If Not ChkIsCleaningUsed.Checked Then Exit Sub
        If cboCleaningChemical.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboCleaningChemical.SelectedValue) Then Exit Sub

        ' تحميل وحدة المادة الجديدة
        LoadCleaningChemicalUnit(CInt(cboCleaningChemical.SelectedValue))

        ' استدعاء المدير المركزي لتحديث الجريد
        UpdateCleaningChemicalInGrid()
        ResetManualOverridesToAuto()
    End Sub    ' ===== خطأ 3: TextChanged غير مربوط بالحدث =====
    Private Sub txtCleaningChemicalQTY_TextChanged(
    sender As Object,
    e As EventArgs
) Handles txtCleaningChemicalQTY.TextChanged

        If IsFormLoading Then Exit Sub
        If Not ChkIsCleaningUsed.Checked Then Exit Sub

        ' التحقق من أن النص رقم صحيح
        Dim qty As Decimal
        If Not Decimal.TryParse(txtCleaningChemicalQTY.Text, qty) Then Exit Sub
        If qty < 0 Then Exit Sub

        ' استدعاء المدير المركزي لتحديث الجريد
        UpdateCleaningChemicalInGrid()
        ResetManualOverridesToAuto()

    End Sub
    Private Sub txtProductionAmount_TextChanged(
    sender As Object,
    e As EventArgs
) Handles txtProductionAmount.TextChanged

        If IsFormLoading Then Exit Sub
        ResetManualOverridesToAuto()
        RecalculateTotalProductionValue()
        RecalculateActualQuantities()
        FillAvailableStockInGrid()
        RecalculateChemicalCost()

        RecalculateProductionTotals()

        MarkAsDirty()

    End Sub

    Private Sub dgvProduced_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProduced.CellValueChanged

        If IsFormLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String = dgvProduced.Columns(e.ColumnIndex).Name

        If colName <> "colManLength" AndAlso
       colName <> "colManWidth" AndAlso
       colName <> "colManHeight" AndAlso
       colName <> "colManQTY" Then Exit Sub
        For Each r As DataGridViewRow In dgvProduced.Rows
            RecalculateProducedRow(r)
        Next
        ResetManualOverridesToAuto()
        RecalculateTotalProductionValue()
        RecalculateActualQuantities()
        FillAvailableStockInGrid()
        RecalculateChemicalCost()

        RecalculateProductionTotals()

        MarkAsDirty()

    End Sub


    Private Sub LoadProductionUnitFromProduct(productID As Integer)

        ' حماية أساسية
        If productID <= 0 Then
            txtProductionUnit.Clear()
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT 
                u.UnitName
            FROM Master_Product p
            INNER JOIN Master_Unit u
                ON u.UnitID = p.ProductionUnitID
            WHERE p.ProductID = @ProductID
        ", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result IsNot Nothing Then
                    txtProductionUnit.Text = result.ToString()
                Else
                    txtProductionUnit.Clear()
                End If

            End Using
        End Using

    End Sub

    Private Function GetUnitName(unitID As Integer) As String

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT UnitName FROM Master_Unit WHERE UnitID = @UnitID", con)

                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = unitID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                End If
            End Using
        End Using

        Return ""

    End Function
    Private Function GetAverageCostByProductID(productID As Integer) As Decimal

        If productID <= 0 Then Return 0D

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT AvgCost
             FROM Master_Product
             WHERE ProductID = @ProductID", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso IsNumeric(result) Then
                    Return CDec(result)
                End If
            End Using
        End Using

        Return 0D

    End Function
    Private Function GetAvailableStock(productID As Integer, storeID As Integer) As Decimal

        If productID <= 0 OrElse storeID <= 0 Then Return 0D

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT QtyOnHand
             FROM Inventory_Balance
             WHERE ProductID = @ProductID
               AND StoreID = @StoreID", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID
                cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = storeID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso IsNumeric(result) Then
                    Return CDec(result)
                End If
            End Using
        End Using

        Return 0D

    End Function
    ' ===== دالة: FillAvailableStockInGrid =====
    Private Sub FillAvailableStockInGrid()

        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboSourceStore.SelectedValue) Then Exit Sub

        Dim storeID As Integer = CInt(cboSourceStore.SelectedValue)

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            If r.Cells("colCalProductID").Value Is Nothing Then Continue For

            Dim productID As Integer = CInt(r.Cells("colCalProductID").Value)

            r.Cells("colCalAvailableStock").Value =
            GetAvailableStock(productID, storeID)
        Next

    End Sub
    ' ===== دالة: cboRawMaterialStore_SelectedValueChanged =====
    Private Sub cboRawMaterialStore_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboSourceStore.SelectedValueChanged

        ' =========================
        ' حمايات أساسية
        ' =========================
        If IsFormLoading Then Exit Sub
        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboSourceStore.SelectedValue) Then Exit Sub

        ' =========================
        ' تحديث الكميات المتاحة فقط
        ' =========================
        FillAvailableStockInGrid()

    End Sub
    ' ===== دالة: EnableAllControlsRecursive =====
    Private Sub EnableAllControlsRecursive(parent As Control)
        For Each c As Control In parent.Controls
            c.Enabled = True
            If c.HasChildren Then
                EnableAllControlsRecursive(c)
            End If
        Next
    End Sub

    ' ===== دالة: btnClose_Click =====
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

        ' =========================
        ' الاعتماد على IsSaved كمصدر واحد
        ' =========================
        If Not IsSaved Then

            Dim res = MessageBox.Show(
            "يوجد بيانات غير محفوظة، هل تريد الخروج؟",
            "تأكيد الخروج",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

            If res = DialogResult.No Then Exit Sub

        End If

        ' =========================
        ' إغلاق الفورم
        ' =========================
        Me.Close()

    End Sub


    Private Function GetCategoryName(categoryID As Integer) As String

        If categoryID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT CategoryName
             FROM Master_ProductCategory
             WHERE CategoryID = @CategoryID", con)

                cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                End If
            End Using
        End Using

        Return ""

    End Function
    Private Function GetSubCategoryName(subCategoryID As Integer) As String

        If subCategoryID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT SubcategoryName
             FROM Master_Subcategory
             WHERE SubcategoryID = @SubcategoryID", con)

                cmd.Parameters.Add("@SubcategoryID", SqlDbType.Int).Value = subCategoryID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                End If
            End Using
        End Using

        Return ""

    End Function

    Private Function GetUnitNameByID(unitID As Integer) As String

        If unitID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT UnitName
             FROM Master_Unit
             WHERE UnitID = @UnitID", con)

                cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = unitID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                End If
            End Using
        End Using

        Return ""

    End Function

    Private Function GetProductGroupName(groupID As Integer) As String

        If groupID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT GroupName
             FROM Master_ProductGroup
             WHERE ProductGroupID = @ProductGroupID", con)

                cmd.Parameters.Add("@ProductGroupID", SqlDbType.Int).Value = groupID
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                End If
            End Using
        End Using

        Return ""

    End Function
    Private Sub FormatProductionGridColumns()

        Dim numericCols As String() = {
        "colCalBOMQTY",
        "colCalActualQTY",
        "colCalAvailableStock",
        "colCalCost"
    }

        For Each colName In numericCols
            If dgvProductionCalculations.Columns.Contains(colName) Then
                With dgvProductionCalculations.Columns(colName)
                    .DefaultCellStyle.Format = "N2"
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End With
            End If
        Next
        txtProductionAmount.Text = GetDec(txtProductionAmount).ToString("N2")
    End Sub
    Private Sub FormatFormNumbers()
        txtProductionAmount.Text = GetDec(txtProductionAmount.Text).ToString("N2")
        txtCleaningChemicalQTY.Text = GetDec(txtCleaningChemicalQTY.Text).ToString("N2")

    End Sub

    Private Sub txtProductionAmount_Leave(sender As Object, e As EventArgs) Handles txtProductionAmount.Leave
        txtProductionAmount.Text = GetDec(txtProductionAmount.Text).ToString("N2")
    End Sub
    Private Sub txtCleaningChemicalQTY_Leave(sender As Object, e As EventArgs) Handles txtCleaningChemicalQTY.Leave
        txtCleaningChemicalQTY.Text = GetDec(txtCleaningChemicalQTY.Text).ToString("N2")
    End Sub
    Private Function GenerateManufacturedProductCode(
    baseProductCode As String,
    l As Decimal,
    w As Decimal,
    h As Decimal
) As String

        Dim lStr As String = CInt(l).ToString("000")
        Dim wStr As String = CInt(w).ToString("000")
        Dim hStr As String = CInt(h).ToString("000")

        Return $"{baseProductCode}-{lStr}{wStr}{hStr}"

    End Function
    Private Function GetDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(v.ToString(), d) Then Return d
        Return 0D
    End Function
    Private Function GetProductCodeByID(productID As Integer) As String
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductCode FROM Master_Product WHERE ProductID=@ID", con)
                cmd.Parameters.AddWithValue("@ID", productID)
                con.Open()
                Dim r = cmd.ExecuteScalar()
                If r IsNot Nothing Then Return r.ToString()
            End Using
        End Using
        Return ""
    End Function
    Private Sub RecalculateProducedRow(row As DataGridViewRow)

        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim l As Decimal = GetDec(row.Cells("colManLength").Value)
        Dim w As Decimal = GetDec(row.Cells("colManWidth").Value)
        Dim h As Decimal = GetDec(row.Cells("colManHeight").Value)
        Dim Quantity As Decimal = GetDec(row.Cells("colManQTY").Value)

        If l <= 0 OrElse w <= 0 OrElse h <= 0 Then
            row.Cells("colManProductVolume").Value = 0
            row.Cells("colManTotalProductVolume").Value = 0
            Exit Sub
        End If
        Dim unitVolume As Decimal = l * w * h / 1000000D
        row.Cells("colManProductVolume").Value = unitVolume

        If Quantity > 0 Then
            row.Cells("colManTotalProductVolume").Value = unitVolume * Quantity
        Else
            row.Cells("colManTotalProductVolume").Value = unitVolume
        End If

    End Sub


    ' ===== دالة: dgvProduced_CellValueChanged =====

    Private Sub RefreshProducedGridAfterLoad()

        For Each r As DataGridViewRow In dgvProduced.Rows
            RecalculateProducedRow(r)
        Next
        RecalculateChemicalCost()

        RecalculateProductionTotals()

    End Sub
    ' ===== دالة: RecalculateProductionTotals =====
    Private Sub UpdateDeviation()

        Dim unitCost As Decimal = GetDec(txtProductUnitCost.Text)
        Dim pastAvg As Decimal = GetDec(txtPastProductAverageCost.Text)

        txtDeviation.Text = (unitCost - pastAvg).ToString("N2")

    End Sub
    ' ===== دالة: txtProductionAmount_KeyPress =====
    Private Sub txtProductionAmount_KeyPress(
    sender As Object,
    e As KeyPressEventArgs
) Handles txtProductionAmount.KeyPress

        If CurrentSubCategoryID = 10 Then
            e.Handled = True
        End If

    End Sub

    ' ===== دالة: btnNew_Click =====
    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click

        IsFormLoading = True

        ResetForNewProduction()
        ClearProductionTotals()
        SetFormMode(FormMode.NewMode)

        IsFormLoading = False

        ' ✅ إعادة تقييم الواجهة حسب الحالة الجديدة
        ApplyUIByWorkflow()
        RefreshWorkflowPolicy()

        cboProductID.Focus()
        btnSave.Text = "حفظ"
    End Sub
    Private Sub LoadProductionStatusCombo()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
    s.StatusID,
    s.StatusCode,
    s.StatusName
FROM Workflow_StatusScope ss
INNER JOIN Workflow_Status s
    ON s.StatusID = ss.StatusID
WHERE ss.ScopeCode = 'PRO'
  AND ss.IsActive = 1
ORDER BY
    ss.IsInitial DESC,
    s.StatusID;

        ", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductionStatus.DataSource = Nothing
        cboProductionStatus.Items.Clear()

        cboProductionStatus.DataSource = dt
        cboProductionStatus.DisplayMember = "StatusName"
        cboProductionStatus.ValueMember = "StatusID"
        cboProductionStatus.SelectedIndex = -1

    End Sub
    Private Function GetCurrentProductionStatusID() As Integer

        If cboProductionStatus.SelectedValue Is Nothing Then Return 0
        If Not IsNumeric(cboProductionStatus.SelectedValue) Then Return 0

        Return CInt(cboProductionStatus.SelectedValue)

    End Function
    Private IsEditMode As Boolean = False
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        If IsFormLoading Then Exit Sub

        ' =========================
        ' Validations أساسية
        ' =========================
        If SelectedProductID <= 0 Then
            MessageBox.Show("اختر المنتج أولاً")
            Exit Sub
        End If

        If CurrentBOMID <= 0 Then
            MessageBox.Show("اختر BOM أولاً")
            Exit Sub
        End If

        If cboSourceStore.SelectedValue Is Nothing OrElse Not IsNumeric(cboSourceStore.SelectedValue) Then
            MessageBox.Show("اختر مستودع المواد الخام")
            Exit Sub
        End If

        If cboTargetStore.SelectedValue Is Nothing OrElse Not IsNumeric(cboTargetStore.SelectedValue) Then
            MessageBox.Show("اختر مستودع المنتج النهائي")
            Exit Sub
        End If

        ' ⭐⭐⭐ تحديد ما إذا كنا في وضع الإدراج أو التعديل ⭐⭐⭐
        Dim currentStatus = GetProductionStatusFromDB(CurrentProductionID)

        Dim mode = GetProductionEditMode(currentStatus)

        Dim isUpdate As Boolean = (CurrentProductionID > 0)

        ' =========================
        ' تجهيز TVP – ProductionOutput
        ' =========================
        Dim tvpOutput As New DataTable()
        tvpOutput.Columns.Add("ProductID", GetType(Integer))
        tvpOutput.Columns.Add("Length", GetType(Decimal))
        tvpOutput.Columns.Add("Width", GetType(Decimal))
        tvpOutput.Columns.Add("Height", GetType(Decimal))
        tvpOutput.Columns.Add("Quantity", GetType(Integer))

        For Each r As DataGridViewRow In dgvProduced.Rows
            If r.IsNewRow Then Continue For

            Dim Quantity As Integer = CInt(GetDec(r.Cells("colManQTY").Value))
            If Quantity <= 0 Then Continue For

            Dim l As Decimal = GetDec(r.Cells("colManLength").Value)
            Dim w As Decimal = GetDec(r.Cells("colManWidth").Value)
            Dim h As Decimal = GetDec(r.Cells("colManHeight").Value)

            If l <= 0 OrElse w <= 0 OrElse h <= 0 Then
                MessageBox.Show("يوجد صف إنتاج أبعاده غير صحيحة (Length/Width/Height).")
                Exit Sub
            End If

            tvpOutput.Rows.Add(
            SelectedProductID,
            l,
            w,
            h,
            Quantity
        )
        Next
        If CurrentSubCategoryID <> 11 Then
            ' إسفنج / مضغوط
            If tvpOutput.Rows.Count = 0 Then
                MessageBox.Show("أدخل صف واحد على الأقل في جدول الإنتاج.")
                Exit Sub
            End If
        Else
            ' مراتب → إنشاء Output افتراضي
            Dim Quantity As Decimal = GetDec(txtProductionAmount.Text)
            If Quantity <= 0D Then
                MessageBox.Show("أدخل كمية إنتاج صحيحة للمراتب.")
                Exit Sub
            End If

            ' ✅ صف Output إلزامي للمراتب
            tvpOutput.Rows.Add(
        SelectedProductID,
        0D,   ' Length
        0D,   ' Width
        0D,   ' Height
        Quantity   ' Quantity
    )
        End If


        ' =========================
        ' تجهيز TVP – ProductionConsumption
        ' =========================
        Dim tvpCons As New DataTable()
        tvpCons.Columns.Add("ComponentProductID", GetType(Integer))
        tvpCons.Columns.Add("BOMQty", GetType(Decimal))
        tvpCons.Columns.Add("ActualConsumedQty", GetType(Decimal))
        tvpCons.Columns.Add("StockQtyAtTime", GetType(Decimal))

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            If r.Cells("colCalProductID").Value Is Nothing Then Continue For

            Dim actualQty As Decimal = GetDec(r.Cells("colCalActualQTY").Value)
            If actualQty <= 0D Then Continue For

            tvpCons.Rows.Add(
            CInt(r.Cells("colCalProductID").Value),
            GetDec(r.Cells("colCalBOMQTY").Value),
            actualQty,
            GetDec(r.Cells("colCalAvailableStock").Value)
        )
        Next

        ' =========================
        ' قيم الهيدر
        ' =========================
        Dim isValidProduction As Boolean = False

        Select Case CurrentSubCategoryID

            Case 9, 11   ' إسفنج / مراتب
                isValidProduction = (GetDec(txtProductionAmount.Text) > 0D)

            Case 10      ' مضغوط
                isValidProduction = (GetDec(txtTotalProductionVolume.Text) > 0D)

        End Select

        If Not isValidProduction Then
            MessageBox.Show("كمية الإنتاج غير صحيحة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim baseValue As Decimal = 0D

        Select Case CurrentSubCategoryID
            Case 9, 11   ' إسفنج / مراتب
                baseValue = GetDec(txtProductionAmount.Text)

            Case 10      ' مضغوط
                baseValue = GetDec(txtTotalProductionVolume.Text)
        End Select

        ' =========================
        ' استدعاء الإجراء
        ' =========================
        Try
            Dim service As New ProductionService(ConnStr)

            If isUpdate Then

                service.UpdateProduction(
            CurrentProductionID,
            dtpProductionDate.Value.Date,
            CurrentBOMID,
            SelectedProductID,
            CInt(cboSourceStore.SelectedValue),
            CInt(cboTargetStore.SelectedValue),
            txtNotes.Text,
            baseValue,
            CurrentUserID,
            GetDec(txtProductUnitCost.Text),
            tvpOutput,
            tvpCons
        )

                MessageBox.Show("تم تحديث الإنتاج بنجاح", "تم",
                MessageBoxButtons.OK, MessageBoxIcon.Information)

                LoadProduction(CurrentProductionID)
                IsSaved = True

            Else

                Dim newID As Integer = 0
                Dim newCode As String = ""

                service.SaveProduction(
            dtpProductionDate.Value.Date,
            CurrentBOMID,
            SelectedProductID,
            CInt(cboSourceStore.SelectedValue),
            CInt(cboTargetStore.SelectedValue),
            txtNotes.Text,
            baseValue,
            CurrentUserID,
            GetDec(txtProductUnitCost.Text),
            tvpOutput,
            tvpCons,
            newID,
            newCode
        )

                CurrentProductionID = newID
                txtProductionCode.Text = newCode

                MessageBox.Show("تم حفظ الإنتاج بنجاح", "تم",
                MessageBoxButtons.OK, MessageBoxIcon.Information)

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ",
            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        RefreshWorkflowPolicy()
        LoadProduction(CurrentProductionID)
    End Sub
    ' ===== دالة: btnSearch_Click =====


    Private Sub dgvdgvProduced_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProduced.CellClick

        If e.RowIndex < 0 Then Exit Sub

        If dgvProduced.Columns(e.ColumnIndex).Name = "colDelete" Then

            DeleteRow(e.RowIndex)

        End If

    End Sub
    Private Sub DeleteRow(rowIndex As Integer)

        Dim dt As DataTable = CType(dgvProduced.DataSource, DataTable)

        If dt Is Nothing Then Exit Sub

        ' 👇 تأكيد
        If MessageBox.Show("هل تريد حذف السطر؟", "تأكيد",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        ' 👇 حذف منطقي من الـ DataTable فقط
        dt.Rows(rowIndex).Delete()

    End Sub

    Private Sub btnSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnSearch.Click

        Using f As New frmProductionSearch
            f.ShowDialog()

            If f.SelectedProductionID <= 0 Then Exit Sub

            ' =========================
            ' تحميل الإنتاج بطريقة آمنة
            ' =========================
            LoadProductionSafe(f.SelectedProductionID)
        End Using

    End Sub
    ' ===== دالة: LoadProduction =====
    Public Sub LoadProduction(productionID As Integer)

        If productionID <= 0 Then Exit Sub

        IsFormLoading = True

        ' =========================
        ' تحميل البيانات من المصدر
        ' =========================
        LoadProductionHeader(productionID)
        LoadProductionOutput(productionID)
        LoadProductionConsumption(productionID)

        ' =========================
        ' تثبيت وضع العرض
        ' =========================
        SetFormMode(FormMode.ViewMode)
        cboProductionStatus.SelectedValue = CurrentProductionStatusID
        cboProductionStatus.Refresh()

        ' =========================
        ' تطبيق القواعد
        ' =========================
        ApplySubCategoryRules()
        ApplyCleaningUIState()

        ApplyUIByWorkflow()
        FormatFormNumbers()
        IsSaved = True
        IsFormLoading = False

    End Sub
    ' ===== دالة: LoadProductionHeader =====
    Private Sub LoadProductionHeader(productionID As Integer)

        ' =========================
        ' حمايات أساسية
        ' =========================
        If productionID <= 0 Then Exit Sub

        ' =========================
        ' قفل الأحداث
        ' =========================
        IsFormLoading = True

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                ProductionID,
                ProductionCode,
                ProductionDate,
                BOMID,
                ProductID,
                SourceProductionStoreID,
                TargetProductionStoreID,
                Notes,
                ProductionBaseValue,
                StatusID,
                IsInventoryPosted
            FROM Production_Header
            WHERE ProductionID = @ProductionID
        ", con)

                cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID
                con.Open()

                Using dr = cmd.ExecuteReader()
                    If Not dr.Read() Then
                        IsFormLoading = False
                        Exit Sub
                    End If

                    ' =========================
                    ' المتغيرات الأساسية
                    ' =========================
                    CurrentProductionID = productionID
                    CurrentProductionStatusID = CInt(dr("StatusID"))
                    CurrentStatusID = CurrentProductionStatusID
                    IsInventoryPosted = CBool(dr("IsInventoryPosted"))
                    CurrentStatusID = CInt(dr("StatusID"))
                    IsEditMode = (CurrentStatusID = 2)
                    RefreshWorkflowPolicy()

                    ' =========================
                    ' حقول الهيدر
                    ' =========================
                    txtProductionCode.Text = dr("ProductionCode").ToString()
                    dtpProductionDate.Value = CDate(dr("ProductionDate"))
                    txtNotes.Text = dr("Notes").ToString()
                    txtProductionAmount.Text = dr("ProductionBaseValue").ToString()

                    ' =========================
                    ' المستودعات
                    ' =========================
                    cboSourceStore.SelectedValue = CInt(dr("SourceProductionStoreID"))
                    cboTargetStore.SelectedValue = CInt(dr("TargetProductionStoreID"))

                    ' =========================
                    ' المنتج
                    ' =========================
                    SelectedProductID = CInt(dr("ProductID"))
                    LoadProducts()
                    cboProductID.SelectedValue = SelectedProductID
                    IsFormLoading = False
                    cboProductID_SelectedValueChanged(cboProductID, EventArgs.Empty)
                    IsFormLoading = True

                    ' =========================
                    ' BOM
                    ' =========================
                    CurrentBOMID = CInt(dr("BOMID"))
                    LoadBOMVersions(SelectedProductID)
                    cboBOMVersion.SelectedValue = CurrentBOMID

                End Using
            End Using
        End Using

        ' =========================
        ' تطبيق القواعد من مصدر واحد
        ' =========================
        LoadProductionUnitFromProduct(SelectedProductID)
        LoadSubCategoryFromProduct(SelectedProductID)
        ApplySubCategoryRules()
        ApplyUIByWorkflow()

        ' =========================
        ' فتح الأحداث
        ' =========================
        IsFormLoading = False
        btnSave.Text = "تعديل"
    End Sub
    ' ===== دالة: LoadProductionOutput =====
    Private Sub LoadProductionOutput(productionID As Integer)

        If productionID <= 0 Then Exit Sub

        IsFormLoading = True

        dgvProduced.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                Length,
                Width,
                Height,
                Quantity
            FROM Production_Output
            WHERE ProductionID = @ProductionID
            ORDER BY OutputID
        ", con)

                cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID
                con.Open()

                Using dr = cmd.ExecuteReader()
                    While dr.Read()

                        Dim i As Integer = dgvProduced.Rows.Add()

                        dgvProduced.Rows(i).Cells("colManLength").Value = dr("Length")
                        dgvProduced.Rows(i).Cells("colManWidth").Value = dr("Width")
                        dgvProduced.Rows(i).Cells("colManHeight").Value = dr("Height")
                        dgvProduced.Rows(i).Cells("colManQTY").Value = dr("Quantity")

                    End While
                End Using

            End Using
        End Using

        IsFormLoading = False

        ' ✅ حساب الأحجام بعد التحميل
        RefreshProducedGridAfterLoad()

    End Sub
    ' ===== دالة: LoadProductionConsumption =====
    Private Sub LoadProductionConsumption(productionID As Integer)

        If productionID <= 0 Then Exit Sub

        IsFormLoading = True

        dgvProductionCalculations.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT
                pc.ComponentProductID,
                p.ProductCode,
                p.ProductName,
                u.UnitName,
                pc.BOMQty,
                pc.ActualConsumedQty,
                pc.StockQtyAtTime,
                pc.TotalCost
            FROM Production_Consumption pc
            INNER JOIN Master_Product p
                ON p.ProductID = pc.ComponentProductID
            INNER JOIN Master_Unit u
                ON u.UnitID = p.StorageUnitID
            WHERE pc.ProductionID = @ProductionID
            ORDER BY pc.ConsumptionID
        ", con)

                cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value = productionID
                con.Open()

                Using dr = cmd.ExecuteReader()
                    While dr.Read()
                        Dim i As Integer = dgvProductionCalculations.Rows.Add()

                        With dgvProductionCalculations.Rows(i)
                            .Cells("colCalProductID").Value = dr("ComponentProductID")
                            .Cells("colCalProductCode").Value = dr("ProductCode").ToString()
                            .Cells("colCalProductName").Value = dr("ProductName").ToString()
                            .Cells("colCalProductUnit").Value = dr("UnitName").ToString()
                            .Cells("colCalBOMQTY").Value = dr("BOMQty")
                            .Cells("colCalBOMQTY").Tag = dr("BOMQty")  ' ⭐ حفظ كمية BOM في Tag

                            .Cells("colCalActualQTY").Value = dr("ActualConsumedQty")

                            ' ⭐⭐ هل هذه المادة هي مادة تنظيف؟ ⭐⭐
                            ' لا يمكننا الجزم من قاعدة البيانات، لكن نفترض أنها ليست تنظيف
                            ' عند الحفظ سنقوم بتعيين Tag مناسب للتنظيف
                            .Cells("colCalActualQTY").Tag = "MANUAL"

                            .Cells("colCalAvailableStock").Value = dr("StockQtyAtTime")
                            .Cells("colCalCost").Value = dr("TotalCost")
                        End With
                    End While
                End Using

            End Using
        End Using
        ' ⭐ بعد تحميل البيانات، نحاول استنتاج ما إذا كانت هناك مادة تنظيف
        ' هذا يعتمد على أن السجل الوحيد الذي BOMQty = 0 قد يكون مادة تنظيف
        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For

            Dim bomQty As Decimal = GetDec(r.Cells("colCalBOMQTY").Value)
            Dim actualQty As Decimal = GetDec(r.Cells("colCalActualQTY").Value)

            ' إذا كانت BOMQty = 0 والكمية الفعلية > 0، فهي غالباً مادة تنظيف
            If bomQty = 0 AndAlso actualQty > 0 Then
                r.Cells("colCalActualQTY").Tag = "CLEANING_ONLY"

                ' محاولة تعيينها في واجهة التنظيف
                Dim productID As Integer = CInt(r.Cells("colCalProductID").Value)

                ' البحث في مصدر الكمبو عن هذا المنتج
                For Each item In cboCleaningChemical.Items
                    Dim drv As DataRowView = TryCast(item, DataRowView)
                    If drv IsNot Nothing AndAlso CInt(drv("ProductID")) = productID Then
                        cboCleaningChemical.SelectedValue = productID
                        LoadCleaningChemicalUnit(productID)
                        txtCleaningChemicalQTY.Text = actualQty.ToString()
                        ChkIsCleaningUsed.Checked = True
                        Exit For
                    End If
                Next
            End If
        Next
        FillAvailableStockInGrid()
        RecalculateChemicalCost()

        RecalculateProductionTotals()

        IsFormLoading = False

    End Sub

    Private Sub btnExecuteProduction_Click(
    sender As Object,
    e As EventArgs
) Handles btnExecuteProduction.Click

        ' =========================
        ' حمايات أساسية
        ' =========================
        If CurrentProductionID <= 0 Then
            MessageBox.Show("يجب حفظ الإنتاج أولاً.", "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim statusID As Integer = 0

        ' =========================
        ' جلب الحالة الفعلية من DB
        ' =========================
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT StatusID
            FROM Production_Header
            WHERE ProductionID = @ProductionID
        ", con)

                cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value = CurrentProductionID
                con.Open()

                Using r = cmd.ExecuteReader()
                    If Not r.Read() Then
                        MessageBox.Show("الإنتاج غير موجود.", "خطأ",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error)
                        Exit Sub
                    End If

                    statusID = CInt(r("StatusID"))
                End Using
            End Using
        End Using

        ' =========================
        ' منع إعادة التنفيذ
        ' =========================
        If statusID = 5 Then
            MessageBox.Show("تم تنفيذ هذا الإنتاج مسبقًا.", "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' =========================
        ' تأكيد المستخدم
        ' =========================
        Dim res = MessageBox.Show(
        "سيتم تنفيذ الإنتاج وإنشاء حركة مخزون بدون ترحيل." &
        vbCrLf & "سيتم الترحيل لاحقًا عند الاستلام." &
        vbCrLf & "هل تريد المتابعة؟",
        "تأكيد التنفيذ",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If dgvProductionCalculations.Rows.
        Cast(Of DataGridViewRow)().
        Any(Function(r) Not r.IsNewRow AndAlso
            GetDec(r.Cells("colCalActualQTY").Value) <= 0D) Then

            MessageBox.Show("يوجد مواد خام بدون كمية استهلاك فعلية.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            Exit Sub
        End If

        If res <> DialogResult.Yes Then Exit Sub

        ' =========================
        ' تنفيذ الإنتاج (بدون ترحيل)
        ' =========================
        Try
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("prod.Post", con)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value =
                    CurrentProductionID

                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value =
                    CurrentUserID

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' =========================
            ' إعادة تحميل الإنتاج
            ' =========================
            LoadProduction(CurrentProductionID)

            MessageBox.Show("تم تنفيذ الإنتاج بنجاح، بانتظار الاستلام.",
                        "تم",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

            IsSaved = True

        Catch ex As SqlException
            MessageBox.Show(ex.Message,
                        "خطأ SQL",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)

        Catch ex As Exception
            MessageBox.Show(ex.Message,
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
        End Try

        RefreshWorkflowPolicy()

    End Sub
    ' ===== دالة: UpdateProductionButtons =====
    Private Sub UpdateProductionButtons()

        ' =========================
        ' المصدر الوحيد للتحكم بالواجهة
        ' =========================
        ApplyUIByWorkflow()

    End Sub
    ' ===== دالة: frmProduction_FormClosing =====
    Private Sub frmProduction_FormClosing(
    sender As Object,
    e As FormClosingEventArgs
) Handles Me.FormClosing

        ' =========================
        ' المصدر الوحيد للتحقق
        ' =========================
        If IsSaved Then Exit Sub

        Dim res = MessageBox.Show(
        "هل تريد حفظ التعديلات قبل الإغلاق؟",
        "تنبيه",
        MessageBoxButtons.YesNoCancel,
        MessageBoxIcon.Warning
    )

        Select Case res
            Case DialogResult.Cancel
                e.Cancel = True

            Case DialogResult.Yes
                btnSave.PerformClick()
                If Not IsSaved Then
                    e.Cancel = True
                End If

            Case DialogResult.No
                ' السماح بالإغلاق
        End Select

    End Sub

    Private Sub btnCancelProduction_Click(sender As Object, e As EventArgs) _
     Handles btnCancelProduction.Click

        ' =========================
        ' حمايات أساسية
        ' =========================
        If IsFormLoading Then Exit Sub

        If CurrentProductionID <= 0 Then
            MessageBox.Show("لا يوجد سند إنتاج محدد.", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' =========================
        ' تأكيد المستخدم
        ' =========================
        Dim msg As String =
            If(IsInventoryPosted,
               "سيتم عكس أثر المخزون وإلغاء الإنتاج." & vbCrLf &
               "لن يتم تعديل متوسط التكلفة." & vbCrLf &
               "هل أنت متأكد؟",
               "سيتم إلغاء الإنتاج (غير مرحّل)." & vbCrLf &
               "هل أنت متأكد؟")

        Dim res = MessageBox.Show(
            msg,
            "تأكيد إلغاء الإنتاج",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)

        If res <> DialogResult.Yes Then Exit Sub

        ' =========================
        ' تنفيذ الإلغاء
        ' =========================
        Try
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("dbo.   ", con)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add("@ProductionID", SqlDbType.Int).Value = CurrentProductionID
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' =========================
            ' تحديث الحالة محليًا
            ' =========================
            IsInventoryPosted = False
            IsSaved = True

            ' إعادة تحميل السند من قاعدة البيانات
            LoadProduction(CurrentProductionID)

            ApplyUIByWorkflow()

            MessageBox.Show("تم إلغاء الإنتاج بنجاح.",
                            "تم",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

        Catch ex As SqlException
            MessageBox.Show(ex.Message,
                            "خطأ SQL",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)

        Catch ex As Exception
            MessageBox.Show(ex.Message,
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
        RefreshWorkflowPolicy()

    End Sub
    Private Sub RecalculateProductionTotals()

        Dim totalProductionVolume As Decimal = 0D
        Dim totalProductionQty As Decimal = 0D
        Dim totalChemicalQty As Decimal = 0D
        Dim totalProductionCost As Decimal = 0D

        If CurrentSubCategoryID <> 11 Then
            For Each r As DataGridViewRow In dgvProduced.Rows
                If r.IsNewRow Then Continue For
                totalProductionVolume += GetDec(r.Cells("colManTotalProductVolume").Value)
                totalProductionQty += GetDec(r.Cells("colManQTY").Value)
            Next
        Else
            totalProductionQty = GetDec(txtProductionAmount.Text)
            totalProductionVolume = 0D
        End If

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            totalChemicalQty += GetDec(r.Cells("colCalActualQTY").Value)
            totalProductionCost += GetDec(r.Cells("colCalCost").Value)
        Next

        txtTotalProductionVolume.Text = totalProductionVolume.ToString("N2")
        txtTotalProductionQTY.Text = totalProductionQty.ToString("N2")
        txtTotalChemicalConsumption.Text = totalChemicalQty.ToString("N2")
        txtTotalProductionCost.Text = totalProductionCost.ToString("N2")


        Select Case CurrentSubCategoryID
            Case 9, 10
                txtProductUnitCost.Text =
                If(totalProductionVolume > 0D,
                   (totalProductionCost / totalProductionVolume).ToString("N2"),
                   "0.0")
            Case 11
                txtProductUnitCost.Text =
                If(totalProductionQty > 0D,
                   (totalProductionCost / totalProductionQty).ToString("N2"),
                   "0.0")
        End Select

        UpdateDeviation()

    End Sub

    Private Sub RecalculateChemicalCost()

        If IsFormLoading Then Exit Sub

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For

            Dim productID As Integer = CInt(r.Cells("colCalProductID").Value)
            Dim actualQty As Decimal = GetDec(r.Cells("colCalActualQTY").Value)
            Dim avgCost As Decimal = GetAverageCostByProductID(productID)

            r.Cells("colCalCost").Value = actualQty * avgCost
        Next

    End Sub
    Private Sub RecalculateTotalProductionValue()

        If IsFormLoading Then Exit Sub

        Dim totalVolume As Decimal = 0D

        ' المضغوط يعتمد على الجريد العلوي فقط
        If CurrentSubCategoryID = 10 Then

            For Each r As DataGridViewRow In dgvProduced.Rows
                If r.IsNewRow Then Continue For

                totalVolume += GetDec(r.Cells("colManTotalProductVolume").Value)
            Next

        Else
            totalVolume = 0D
        End If

        txtTotalProductionVolume.Text = totalVolume.ToString("N2")

    End Sub

    Private Sub RecalculateActualQuantities()

        If IsFormLoading Then Exit Sub

        Dim factor As Decimal = GetProductionFactor()

        ' ✅ مهم: لا نحسب إذا factor=0 حتى لا نصفر
        If factor <= 0D Then Exit Sub

        Dim cleaningProductID As Integer = -1
        If ChkIsCleaningUsed.Checked AndAlso cboCleaningChemical.SelectedValue IsNot Nothing AndAlso IsNumeric(cboCleaningChemical.SelectedValue) Then
            cleaningProductID = CInt(cboCleaningChemical.SelectedValue)
        End If

        For Each r As DataGridViewRow In dgvProductionCalculations.Rows
            If r.IsNewRow Then Continue For
            If r.Cells("colCalProductID").Value Is Nothing Then Continue For

            Dim qtyTag As String = If(r.Cells("colCalActualQTY").Tag, "AUTO").ToString()

            ' التنظيف لا نحسبه هنا (يُدار بدالة UpdateCleaningChemicalInGrid)
            If qtyTag.StartsWith("CLEANING") Then Continue For

            Dim productID As Integer = CInt(r.Cells("colCalProductID").Value)
            If productID = cleaningProductID Then Continue For

            ' ✅ نحسب فقط AUTO — اليدوي يبقى كما هو
            If qtyTag <> "AUTO" Then Continue For

            Dim bomBase As Decimal =
            If(r.Cells("colCalBOMQTY").Tag IsNot Nothing,
               GetDec(r.Cells("colCalBOMQTY").Tag),
               GetDec(r.Cells("colCalBOMQTY").Value))

            r.Cells("colCalActualQTY").Value = bomBase * factor
        Next

    End Sub
    Private Enum EditModeType
        DirectEdit
        EngineEdit
        NoEdit
    End Enum

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        If IsFormLoading Then Exit Sub

        If CurrentProductionID <= 0 Then
            MessageBox.Show("لا يوجد سند إنتاج محدد.", "تنبيه",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' =========================
        ' تحديد نوع التعديل حسب الحالة
        ' =========================
        Dim mode = GetProductionEditMode(CurrentStatusID)

        If mode = EditModeType.NoEdit Then
            MessageBox.Show("لا يمكن إلغاء هذا السند.",
                       "عملية غير مسموحة",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If mode = EditModeType.EngineEdit Then
            MessageBox.Show("لا يمكن الغاء سند تم استلامه يجب الغاء الاستلام اولا .",
                       "تنبيه",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' =========================
        ' تأكيد المستخدم
        ' =========================
        Dim result = MessageBox.Show(
        "هل أنت متأكد من إلغاء هذا الإنتاج؟" & vbCrLf &
        "السند: " & txtProductionCode.Text,
        "تأكيد الإلغاء",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question)

        If result <> DialogResult.Yes Then Exit Sub

        ' =========================
        ' تنفيذ الإلغاء عبر Service
        ' =========================
        Try

            Dim service As New ProductionService(ConnStr)

            service.CancelProduction(CurrentProductionID, CurrentUserID)

            ' تحديث الحالة بعد الإلغاء
            CurrentStatusID = 10

            LoadProduction(CurrentProductionID)

            MessageBox.Show("تم إلغاء الإنتاج بنجاح.", "تم",
               MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ",
               MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        RefreshWorkflowPolicy()
        LoadProduction(CurrentProductionID)
    End Sub
    Private Function GetProductionEditMode(statusID As Integer) As EditModeType

        Select Case statusID

            Case 1, 2, 5
                Return EditModeType.DirectEdit

            Case 6
                Return EditModeType.EngineEdit

            Case 10, 11
                Return EditModeType.NoEdit

            Case Else
                Return EditModeType.NoEdit

        End Select

    End Function
    Private Function GetProductionStatusFromDB(productionID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT StatusID 
FROM Production_Header
WHERE ProductionID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", productionID)

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then Return 0

                Return CInt(result)

            End Using
        End Using

    End Function
End Class
