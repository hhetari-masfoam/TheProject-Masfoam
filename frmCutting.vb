Imports System.Data
Imports System.Data.SqlClient

Public Class frmCutting
    Inherits AABaseOperationForm
#Region "================= المتغيرات العامة ================="
    ' جدول أنواع المنتجات (يستخدم في ComboBoxColumn)
    Private dtProductTypes As DataTable
    ' متغير على مستوى الفورم
    Private LastProductID As Integer = 0
    ' الصف المسموح العمل عليه في الجريد العلوي
    Private AllowedInputRowIndex As Integer = 0
    ' العملية الحالية (0 = جديد)
    Private CurrentCuttingID As Integer = 0
    Private IsLoadingCutting As Boolean = False
    ' =========================
    ' إدارة صف التعديل (رجوع تلقائي عند اختيار صف آخر)
    ' =========================
    Private PendingEdit As Boolean = False
    Private PendingOutIndex As Integer = -1
    Private PendingOutData As Dictionary(Of String, Object) = Nothing
    ' =========================
    ' وضع تعديل صف إخراج
    ' =========================
    Private IsEditingOutput As Boolean = False
    Private EditingOutRowIndex As Integer = -1
    Private IsNewCutting As Boolean = True

    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "CUT"
        End Get
    End Property

#End Region

#Region "================= تحميل الفورم ================="

    Private Sub frmCutting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadWorkflowPolicy(0)
        txtCuttingStatus.Text = GetStatusNameByID(FormStatusID)


        ' منع الأعمدة التلقائية
        dgvInPut.AutoGenerateColumns = False
        dgvOutPut.AutoGenerateColumns = False
        LoadProductsForCutting()
        CurrentCuttingID = 0
        btnSend.Enabled = False
        ' =========================
        ' منع التنقل بين الصفوف في الجريد العلوي
        ' =========================
        With dgvInPut
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
        End With

        ' تهيئة أزرار الجريد
        SetupInputGridButtons()

        ' تحميل كود القص
        LoadCuttingCodes()

        ' تحميل المنتجات الأساسية
        '        LoadProductsForCutting()

        ' جعل حقول العرض ReadOnly
        SetReadOnlyFields()

        ' تحميل أنواع المنتجات وربطها بالكمبو
        LoadProductTypes()
        BindProductTypeComboColumn()
        ' =========================
        ' الجريد السفلي عرض فقط
        ' =========================
        dgvOutPut.ReadOnly = True
        dgvOutPut.AllowUserToAddRows = False
        dgvOutPut.AllowUserToDeleteRows = False
        dgvOutPut.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvOutPut.MultiSelect = False
        ClearProductDisplay()
        ' =========================
        ' تعطيل الجريدات عند فتح الفورم
        ' =========================
        dgvInPut.Enabled = False
        dgvOutPut.Enabled = False
        ' الافتراضي
        cboSourceStore.SelectedValue = 3
        cboTargetedStore.SelectedValue = 4
        cboSourceStore.Enabled = True
        cboTargetedStore.Enabled = True

    End Sub

#End Region

#Region "================= تحميل البيانات ================="

    ' تحميل كود القص
    Private Sub LoadCuttingCodes()

        Dim dt As New DataTable
        dt.Columns.Add("CuttingCode", GetType(String))
        dt.Rows.Add(GenerateCuttingCode())

        cboCuttingCode.DataSource = dt
        cboCuttingCode.DisplayMember = "CuttingCode"
        cboCuttingCode.ValueMember = "CuttingCode"
        cboCuttingCode.SelectedIndex = 0


    End Sub
    ' =========================
    ' تحميل المخازن
    ' =========================
    Private Sub LoadSourceStores()

        Dim dt As New DataTable

        Dim sql As String =
        "SELECT StoreID, StoreName
         FROM Master_Store
         WHERE IsActive = 1
         ORDER BY StoreName;"

        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dt)
            End Using
        End Using

        cboSourceStore.DataSource = dt
        cboSourceStore.DisplayMember = "StoreName"
        cboSourceStore.ValueMember = "StoreID"
        cboSourceStore.SelectedIndex = -1



    End Sub
    Private Sub LoadTargetedStores()

        Dim dt As New DataTable

        Dim sql As String =
        "SELECT StoreID, StoreName
         FROM Master_Store
         WHERE IsActive = 1
         ORDER BY StoreName;"

        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dt)
            End Using
        End Using



        cboTargetedStore.DataSource = dt
        cboTargetedStore.DisplayMember = "StoreName"
        cboTargetedStore.ValueMember = "StoreID"
        cboTargetedStore.SelectedIndex = -1

    End Sub


    ' تحميل المنتجات
    Private Sub LoadProductsForCutting()

        LoadSourceStores()
        LoadTargetedStores()
        Dim dt As New DataTable

        Dim sql As String =
"SELECT
    p.ProductID,
    p.ProductCode,
    p.ProductName,
    p.ProductEnglishName,
    c.CategoryName,
    pg.GroupName AS ProductGroupName,
    clr.ColorName,
    pt.TypeName AS ProductTypeName,
    mt.ProductCode AS MixTypeName,
    p.Description,
    p.Density
FROM Master_Product p
INNER JOIN Master_ProductCategory c
    ON p.ProductCategoryID = c.CategoryID
LEFT JOIN Master_ProductGroup pg
    ON p.ProductGroupID = pg.ProductGroupID
LEFT JOIN Master_ProductColor clr
    ON p.ProductColorID = clr.ColorID
LEFT JOIN Master_ProductType pt
    ON p.ProductTypeID = pt.ProductTypeID
LEFT JOIN Master_Product mt
    ON p.MixTypeID = mt.ProductID
INNER JOIN Inventory_Balance ib
    ON ib.ProductID = p.ProductID
WHERE p.IsActive = 1
  AND p.ProductCategoryID = 2
  AND ib.QtyOnHand > 0
ORDER BY p.ProductCode;
"


        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dt)
            End Using
        End Using

        cboProductCode.DataSource = dt
        cboProductCode.DisplayMember = "ProductCode"
        cboProductCode.ValueMember = "ProductID"
        cboProductCode.SelectedIndex = -1

    End Sub


    ' تحميل أنواع المنتجات
    Private Sub LoadProductTypes()

        dtProductTypes = New DataTable

        Dim sql As String =
            "SELECT ProductTypeID, TypeName
             FROM Master_ProductType
             WHERE IsActive = 1
             ORDER BY TypeName;"

        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dtProductTypes)
            End Using
        End Using

    End Sub

#End Region

#Region "================= واجهة المستخدم ================="

    ' جعل الحقول للعرض فقط
    Private Sub SetReadOnlyFields()

        txtEnglishName.ReadOnly = True
        txtProductCategory.ReadOnly = True
        txtProductGroup.ReadOnly = True
        txtProductColor.ReadOnly = True
        txtProductType.ReadOnly = True
        txtProductMixType.ReadOnly = True
        txtProductNote.ReadOnly = True

    End Sub


    ' مسح بيانات المنتج
    Private Sub ClearProductDisplay()

        txtEnglishName.Clear()
        txtProductCategory.Clear()
        txtProductGroup.Clear()
        txtProductColor.Clear()
        txtProductType.Clear()
        txtProductMixType.Clear()
        txtProductNote.Clear()

    End Sub


    ' تهيئة أزرار Add / Delete
    Private Sub SetupInputGridButtons()

        With CType(dgvInPut.Columns("colAdd"), DataGridViewButtonColumn)
            .Text = "Add"
            .UseColumnTextForButtonValue = True
        End With

        With CType(dgvInPut.Columns("colDelete"), DataGridViewButtonColumn)
            .Text = "Delete"
            .UseColumnTextForButtonValue = True
        End With

    End Sub


    ' ربط عمود نوع المنتج بالكمبو
    Private Sub BindProductTypeComboColumn()

        Dim col = CType(dgvInPut.Columns("colProductType"), DataGridViewComboBoxColumn)

        col.DataSource = dtProductTypes
        col.DisplayMember = "TypeName"
        col.ValueMember = "ProductTypeID"
        col.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        col.FlatStyle = FlatStyle.Standard

    End Sub

#End Region

#Region "================= دوال مساعدة ================="
    ' =========================================================
    ' الحصول على ProductID لناتج القص
    ' إن لم يوجد → فتح فورم الترميز
    ' =========================================================

    ' =========================
    ' بيانات ناتج القص لتمريرها للترميز
    ' =========================


    Private Function SnapshotOutRow(outRow As DataGridViewRow) As Dictionary(Of String, Object)

        Dim d As New Dictionary(Of String, Object)

        ' نخزن كل القيم المهمة حسب أعمدتك الحالية
        d("colOutProductCode") = outRow.Cells("colOutProductCode").Value
        d("colOutProductTypeID") = outRow.Cells("colOutProductTypeID").Value
        d("colOutProductType") = outRow.Cells("colOutProductType").Value
        d("colOutLength") = outRow.Cells("colOutLength").Value
        d("colOutWidth") = outRow.Cells("colOutWidth").Value
        d("colOutHeight") = outRow.Cells("colOutHeight").Value
        d("colOutQty") = outRow.Cells("colOutQty").Value
        d("colOutPieceVolume") = outRow.Cells("colOutPieceVolume").Value
        d("colOutTotalVolume") = outRow.Cells("colOutTotalVolume").Value

        Return d

    End Function

    ' =========================
    ' رجوع الصف القديم للإخراج قبل البدء بتعديل صف جديد
    ' =========================
    Private Sub RestorePendingOutRow()

        If Not PendingEdit Then Exit Sub
        If PendingOutData Is Nothing Then Exit Sub

        ' نرجعه في نفس مكانه قدر الإمكان
        Dim insertAt As Integer = PendingOutIndex
        If insertAt < 0 Then insertAt = 0
        If insertAt > dgvOutPut.Rows.Count Then insertAt = dgvOutPut.Rows.Count

        ' نضيف صف جديد ونعبئه بنفس القيم الأصلية
        Dim idx As Integer
        If insertAt = dgvOutPut.Rows.Count Then
            idx = dgvOutPut.Rows.Add()
        Else
            dgvOutPut.Rows.Insert(insertAt, 1)
            idx = insertAt
        End If

        With dgvOutPut.Rows(idx)
            .Cells("colOutProductCode").Value = PendingOutData("colOutProductCode")
            .Cells("colOutProductTypeID").Value = PendingOutData("colOutProductTypeID")
            .Cells("colOutProductType").Value = PendingOutData("colOutProductType")
            .Cells("colOutLength").Value = PendingOutData("colOutLength")
            .Cells("colOutWidth").Value = PendingOutData("colOutWidth")
            .Cells("colOutHeight").Value = PendingOutData("colOutHeight")
            .Cells("colOutQty").Value = PendingOutData("colOutQty")
            .Cells("colOutPieceVolume").Value = PendingOutData("colOutPieceVolume")
            .Cells("colOutTotalVolume").Value = PendingOutData("colOutTotalVolume")
        End With

        ' تنظيف صف الإدخال السابق (لأننا ألغينا تعديله تلقائيًا)
        dgvInPut.Rows.Clear()
        AllowedInputRowIndex = -1

        ' تصفير حالة التعديل
        PendingEdit = False
        PendingOutIndex = -1
        PendingOutData = Nothing

        UpdateOutPutTotals()

    End Sub

    ' =========================================================
    ' تحديث إجماليات نواتج القص (حجم + كمية)
    ' =========================================================
    Private Sub UpdateOutPutTotals()

        Dim totalVolume As Decimal = 0D
        Dim totalQty As Decimal = 0D

        ' المرور على كل صف في الجريد السفلي
        For Each r As DataGridViewRow In dgvOutPut.Rows

            If r.IsNewRow Then Continue For

            ' جمع الحجم الإجمالي
            totalVolume += GetDec(r.Cells("colOutTotalVolume").Value)

            ' جمع الكمية
            totalQty += GetDec(r.Cells("colOutQty").Value)

        Next

        ' عرض النتائج
        txtTotalVolumeOutPut.Text = totalVolume.ToString("0.###")
        TotalPcsOutPut.Text = totalQty.ToString("0.###")

    End Sub

    ' تحويل Decimal آمن
    Private Function GetDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal
        Decimal.TryParse(v.ToString(), d)
        Return d
    End Function


    ' حساب حجم الحبة بالمتر المكعب
    Private Function PieceVolM3(lCM As Decimal, wCM As Decimal, hCM As Decimal) As Decimal
        Return (lCM / 100D) * (wCM / 100D) * (hCM / 100D)
    End Function


    ' جلب اسم نوع المنتج
    Private Function GetTypeName(typeID As Integer) As String
        If dtProductTypes Is Nothing Then Return ""
        Dim r = dtProductTypes.Select("ProductTypeID=" & typeID)
        If r.Length = 0 Then Return ""
        Return r(0)("TypeName").ToString()
    End Function

#End Region

    Private Sub dgvInPut_SelectionChanged(
    sender As Object,
    e As EventArgs
) Handles dgvInPut.SelectionChanged

        ' ✅ لو ما حددنا صف مسموح بعد، لا تتدخل
        If AllowedInputRowIndex < 0 Then Exit Sub

        ' ✅ لو الجريد فاضي (أثناء حذف/نقل)، اخرج
        If dgvInPut.Rows.Count = 0 Then Exit Sub

        ' ✅ لو AllowedInputRowIndex صار خارج النطاق بسبب حذف الصف، اخرج
        If AllowedInputRowIndex >= dgvInPut.Rows.Count Then Exit Sub

        ' ✅ لو لا توجد خلية حالية، اخرج
        If dgvInPut.CurrentCell Is Nothing Then Exit Sub

        ' ✅ منع الانتقال لصف غير مسموح
        If dgvInPut.CurrentCell.RowIndex <> AllowedInputRowIndex Then

            Dim colIdx As Integer = dgvInPut.CurrentCell.ColumnIndex
            If colIdx < 0 Then colIdx = 0
            If colIdx >= dgvInPut.Columns.Count Then colIdx = 0

            dgvInPut.CurrentCell = dgvInPut.Rows(AllowedInputRowIndex).Cells(colIdx)

        End If

    End Sub

    ' عند اختيار منتج
    Private Sub cboProductCode_SelectedIndexChanged(
    sender As Object, e As EventArgs
) Handles cboProductCode.SelectedIndexChanged

        Dim cbo = DirectCast(sender, ComboBox)

        Dim drv As DataRowView = TryCast(cbo.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        Dim newProductID As Integer = CInt(drv("ProductID"))

        ' =========================
        ' تحميل خصائص المنتج (دائمًا)
        ' =========================
        txtEnglishName.Text = drv("ProductEnglishName").ToString()
        txtProductCategory.Text = drv("CategoryName").ToString()
        txtProductGroup.Text = drv("ProductGroupName").ToString()
        txtProductColor.Text = drv("ColorName").ToString()
        txtProductType.Text = drv("ProductTypeName").ToString()
        txtProductMixType.Text = drv("MixTypeName").ToString()
        txtProductNote.Text = drv("Description").ToString()


        ' =========================
        ' ⛔ تحذير فقط عند تغيير يدوي
        ' =========================
        If cbo.Focused Then

            If LastProductID <> 0 AndAlso
           newProductID <> LastProductID AndAlso
           dgvOutPut.Rows.Count > 0 Then

                Dim res = MessageBox.Show(
                "تغيير المنتج سيؤدي إلى حذف جميع نواتج القص الحالية." & vbCrLf &
                "هل تريد المتابعة؟",
                "تنبيه",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)

                If res = DialogResult.No Then
                    cbo.SelectedValue = LastProductID
                    Exit Sub
                End If

                dgvOutPut.Rows.Clear()
                dgvInPut.Rows.Clear()
            End If
        End If
        ' =========================
        ' تفعيل الجريد وتجهيز سطر إدخال
        ' =========================
        If dgvInPut.Enabled = False Then
            dgvInPut.Enabled = True
            dgvOutPut.Enabled = True
        End If

        PrepareNewInputRow()

        LastProductID = newProductID
        RefreshAvailableQty()

    End Sub

    Private Sub PrepareNewInputRow()

        dgvInPut.Rows.Clear()

        Dim idx As Integer = dgvInPut.Rows.Add()
        AllowedInputRowIndex = idx

        dgvInPut.Enabled = True

        dgvInPut.CurrentCell = dgvInPut.Rows(idx).Cells("colLength")
        dgvInPut.BeginEdit(True)

    End Sub

    ' منع أخطاء الكمبو
    Private Sub dgvInPut_DataError(
        sender As Object,
        e As DataGridViewDataErrorEventArgs
    ) Handles dgvInPut.DataError

        e.ThrowException = False

    End Sub


    ' زر Add / Delete
    Private Sub dgvInPut_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInPut.CellContentClick

        Try
            ' 1) منع الضغط على الهيدر أو خارج الأعمدة
            If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

            ' 2) منع أي Index خارج نطاق الصفوف/الأعمدة
            If e.RowIndex >= dgvInPut.Rows.Count Then Exit Sub
            If e.ColumnIndex >= dgvInPut.Columns.Count Then Exit Sub

            Dim row As DataGridViewRow = dgvInPut.Rows(e.RowIndex)
            If row Is Nothing OrElse row.IsNewRow Then Exit Sub

            ' 3) منع التعديل خارج الصف المسموح
            If AllowedInputRowIndex <> e.RowIndex Then
                MessageBox.Show("هذا السطر غير مخصص للتعديل")
                Exit Sub
            End If

            Dim colName As String = dgvInPut.Columns(e.ColumnIndex).Name

            ' حذف
            If colName = "colDelete" Then
                dgvInPut.Rows.Remove(row)
                AllowedInputRowIndex = -1
                Exit Sub
            End If

            If colName <> "colAdd" Then Exit Sub

            dgvInPut.CommitEdit(DataGridViewDataErrorContexts.Commit)
            dgvInPut.EndEdit()

            ' =========================
            ' قراءة القيم
            ' =========================
            Dim l As Decimal = GetDec(row.Cells("colLength").Value)
            Dim w As Decimal = GetDec(row.Cells("colWidth").Value)
            Dim h As Decimal = GetDec(row.Cells("colHeight").Value)
            Dim Quantity As Decimal = GetDec(row.Cells("colQty").Value)

            If l <= 0 OrElse w <= 0 OrElse h <= 0 OrElse Quantity <= 0 Then
                MessageBox.Show("يرجى إدخال القيم بشكل صحيح")
                Exit Sub
            End If

            If row.Cells("colProductType").Value Is Nothing Then
                MessageBox.Show("يرجى اختيار نوع المنتج")
                Exit Sub
            End If

            Dim typeID As Integer = CInt(row.Cells("colProductType").Value)
            Dim typeName As String = GetTypeName(typeID)

            ' توليد كود الناتج
            Dim baseCode As String = cboProductCode.Text.Trim()
            Dim outCode As String

            If typeID = 4 Then
                outCode = txtProductMixType.Text.Trim()
            Else
                outCode = GenOutCode(baseCode, l, w, h)
            End If

            ' منع التكرار
            For Each r As DataGridViewRow In dgvOutPut.Rows
                If r.IsNewRow Then Continue For
                If IsEditingOutput AndAlso r.Index = EditingOutRowIndex Then Continue For

                If r.Cells("colOutProductCode").Value.ToString() = outCode AndAlso
               CInt(r.Cells("colOutProductTypeID").Value) = typeID Then
                    MessageBox.Show("المنتج المقصوص مضاف مسبقًا")
                    Exit Sub
                End If
            Next

            Dim pv As Decimal = PieceVolM3(l, w, h)
            Dim tv As Decimal = pv * Quantity

            ' إضافة / تعديل الإخراج
            If IsEditingOutput AndAlso EditingOutRowIndex >= 0 AndAlso EditingOutRowIndex < dgvOutPut.Rows.Count Then

                Dim outRow As DataGridViewRow = dgvOutPut.Rows(EditingOutRowIndex)

                outRow.Cells("colOutProductCode").Value = outCode
                outRow.Cells("colOutLength").Value = l
                outRow.Cells("colOutWidth").Value = w
                outRow.Cells("colOutHeight").Value = h
                outRow.Cells("colOutQty").Value = Quantity
                outRow.Cells("colOutProductTypeID").Value = typeID
                outRow.Cells("colOutProductType").Value = typeName
                outRow.Cells("colOutPieceVolume").Value = pv
                outRow.Cells("colOutTotalVolume").Value = tv

                IsEditingOutput = False
                EditingOutRowIndex = -1

            Else
                Dim outIdx As Integer = dgvOutPut.Rows.Add()
                With dgvOutPut.Rows(outIdx)
                    .Cells("colOutProductCode").Value = outCode
                    .Cells("colOutLength").Value = l
                    .Cells("colOutWidth").Value = w
                    .Cells("colOutHeight").Value = h
                    .Cells("colOutQty").Value = Quantity
                    .Cells("colOutProductTypeID").Value = typeID
                    .Cells("colOutProductType").Value = typeName
                    .Cells("colOutPieceVolume").Value = pv
                    .Cells("colOutTotalVolume").Value = tv
                End With
            End If

            ' إعادة تجهيز الإدخال
            dgvInPut.Rows.Clear()
            Dim newIdx As Integer = dgvInPut.Rows.Add()
            AllowedInputRowIndex = newIdx

            If dgvInPut.Columns.Contains("colLength") Then
                dgvInPut.CurrentCell = dgvInPut.Rows(newIdx).Cells("colLength")
                dgvInPut.BeginEdit(True)
            End If

            UpdateOutPutTotals()

        Catch ex As Exception
            ' لا تجعل التطبيق ينهار؛ اعرض رسالة مفهومة
            MessageBox.Show("حدث خطأ أثناء تنفيذ العملية:" & vbCrLf & ex.Message)
        End Try

    End Sub

    ' الضغط على صف الإخراج لإرجاعه للتعديل
    Private Sub dgvOutPut_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvOutPut.CellClick

        If e.RowIndex < 0 Then Exit Sub

        Dim outRow As DataGridViewRow = dgvOutPut.Rows(e.RowIndex)
        If outRow.IsNewRow Then Exit Sub

        ' =========================
        ' قفل أي تحرير سابق
        ' =========================
        dgvInPut.EndEdit()
        dgvOutPut.EndEdit()

        ' =========================
        ' دخول وضع تعديل إخراج
        ' =========================
        IsEditingOutput = True
        EditingOutRowIndex = e.RowIndex

        ' =========================
        ' 🔴 تفعيل جريد الإدخال مؤقتًا للتعديل
        ' =========================
        dgvInPut.Enabled = True

        ' =========================
        ' تجهيز جريد الإدخال (صف واحد فقط)
        ' =========================
        dgvInPut.Rows.Clear()

        Dim idx As Integer = dgvInPut.Rows.Add()
        AllowedInputRowIndex = idx
        Dim inRow As DataGridViewRow = dgvInPut.Rows(idx)

        ' تعبئة القيم
        inRow.Cells("colLength").Value = outRow.Cells("colOutLength").Value
        inRow.Cells("colWidth").Value = outRow.Cells("colOutWidth").Value
        inRow.Cells("colHeight").Value = outRow.Cells("colOutHeight").Value
        inRow.Cells("colQty").Value = outRow.Cells("colOutQty").Value

        ' تعبئة نوع المنتج
        Dim typeID As Integer = CInt(outRow.Cells("colOutProductTypeID").Value)
        Dim cmb As DataGridViewComboBoxCell =
        CType(inRow.Cells("colProductType"), DataGridViewComboBoxCell)

        cmb.DataSource = dtProductTypes
        cmb.DisplayMember = "TypeName"
        cmb.ValueMember = "ProductTypeID"
        cmb.Value = typeID

        ' زر Edit
        inRow.Cells("colAdd").Value = "Edit"

        ' =========================
        ' تثبيت المؤشر للكتابة
        ' =========================
        dgvInPut.CurrentCell = inRow.Cells("colLength")
        dgvInPut.BeginEdit(True)

    End Sub

#Region "================= توليد الأكواد ================="

    ' توليد كود الناتج
    Private Function GenOutCode(
        baseCode As String,
        l As Decimal,
        w As Decimal,
        h As Decimal
    ) As String

        Return $"{baseCode}-{CInt(l):000}{CInt(w):000}{CInt(h):000}"

    End Function


    ' توليد كود القص CT-00001
    Private Function GenerateCuttingCode() As String

        Dim maxNo As Integer = 0

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT CuttingCode FROM Production_CuttingHeader WHERE CuttingCode LIKE 'CUT-%';", con)

                con.Open()
                Using dr = cmd.ExecuteReader()
                    While dr.Read()
                        Dim n As Integer
                        If Integer.TryParse(dr("CuttingCode").ToString().Replace("CUT-", ""), n) Then
                            maxNo = Math.Max(maxNo, n)
                        End If
                    End While
                End Using
            End Using
        End Using

        Return "CUT-" & (maxNo + 1).ToString("000000")

    End Function


#End Region
    ' =========================
    ' خطوة 1: New
    ' =========================
    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        CurrentCuttingID = 0
        IsNewCutting = True

        dgvInPut.Rows.Clear()
        dgvOutPut.Rows.Clear()

        ' =========================
        ' 3️⃣ مسح بيانات عرض المنتج
        ' =========================
        ClearProductDisplay()

        ' =========================
        ' 4️⃣ تصفير الإجماليات
        ' =========================
        txtTotalVolumeOutPut.Text = "0"
        TotalPcsOutPut.Text = "0"
        txtCuttingStatus.Text = ""
        ' =========================
        ' 5️⃣ إعادة توليد كود القص
        ' =========================
        LoadCuttingCodes()
        cboCuttingCode.Enabled = True

        ' =========================
        ' 6️⃣ إعادة ضبط اختيار المنتج
        ' =========================
        LastProductID = 0
        cboProductCode.Visible = True
        cboProductCode.Enabled = True
        cboProductCode.SelectedIndex = -1
        cboSourceStore.Enabled = True
        cboTargetedStore.Enabled = True

        ' =========================
        ' 7️⃣ تفعيل زر البحث
        ' =========================
        btnFind.Enabled = True

        ' =========================
        ' 8️⃣ تعطيل الجريدات لحين اختيار منتج
        ' =========================
        dgvInPut.Enabled = False
        dgvOutPut.Enabled = False

        ' =========================
        ' 9️⃣ ضبط المؤشرات
        ' =========================
        AllowedInputRowIndex = -1
        dtpCuttingDate.Value = Date.Today

        ' =========================
        ' 🔟 زر الحفظ
        ' =========================
        btnSave.Text = "Save"
        btnSave.Enabled = True

    End Sub
    ' =========================
    ' خطوة 2: Save
    ' =========================

    Private Function GetProductIDByCode(productCode As String) As Integer

        If String.IsNullOrWhiteSpace(productCode) Then
            Return 0
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT TOP 1 ProductID
FROM Master_Product
WHERE ProductCode = @Code
  AND IsActive = 1
", con)

                cmd.Parameters.AddWithValue("@Code", productCode.Trim())

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse IsDBNull(result) Then
                    Return 0
                End If

                Return CInt(result)
            End Using
        End Using

    End Function

    Private Sub btnFind_Click(sender As Object, e As EventArgs) Handles btnFind.Click

        Using f As New frmProductSearch()

            f.SourceStoreID = CInt(cboSourceStore.SelectedValue)
            f.OnlyWithBalance = True        ' فلترة انفرتري
            f.ProductCategoryID = 2         ' فئة
            ' f.ProductTypeID = 0           ' غير مستخدم

            If f.ShowDialog() = DialogResult.OK Then
                cboProductCode.SelectedValue = f.SelectedProductID
            End If

        End Using


    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        Using f As New frmCuttingSearch()
            If f.ShowDialog() = DialogResult.OK Then

                If f.SelectedCuttingID > 0 Then
                    LoadCutting(f.SelectedCuttingID)
                End If

            End If
        End Using

    End Sub
    Private Sub LoadBaseProductFromCuttingInput(cuttingID As Integer)

        Dim baseProductID As Integer = 0

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
        "SELECT TOP 1 ProductID
         FROM Production_CuttingInput
         WHERE CutID = @ID", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)
                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    baseProductID = CInt(obj)
                End If

            End Using
        End Using

        If baseProductID > 0 Then
            cboProductCode.SelectedValue = baseProductID
        End If

    End Sub

    Private Sub LoadCutting(cuttingID As Integer)

        CurrentCuttingID = cuttingID
        IsNewCutting = False
        Dim cuttingCode As String = ""
        Dim cutDate As Date
        Dim notes As String = ""
        Dim sourceStoreID As Integer = -1
        Dim targetStoreID As Integer = -1
        Dim statusName As String = ""
        Dim statusID As Integer = 0
        dgvInPut.Rows.Clear()
        dgvOutPut.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =========================
            ' 1) تحميل الهيدر (بدون مستودعات)
            ' =========================
            Using cmd As New SqlCommand(
        "SELECT 
    h.CuttingCode,
    h.CutDate,
    h.Notes,
    h.StatusID,
    s.StatusName
FROM Production_CuttingHeader h
LEFT JOIN Workflow_Status s
    ON s.StatusID = h.StatusID
WHERE h.CuttingID = @ID",
        con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then
                        cuttingCode = dr("CuttingCode").ToString()
                        cutDate = CDate(dr("CutDate"))
                        notes = dr("Notes").ToString()
                        statusName = dr("StatusName").ToString()
                        statusID = CInt(dr("StatusID"))
                    End If
                End Using
            End Using

            ' =========================
            ' 2) تحميل نواتج القص + المستودعات
            ' =========================
            Using cmd As New SqlCommand(
        "SELECT
            ProductID,
            OutProductCode,
            ProductTypeID,
            MAX(Length_cm) AS Length_cm,
            MAX(Width_cm) AS Width_cm,
            MAX(Height_cm) AS Height_cm,
            SUM(QtyPieces) AS QtyPieces,
            MAX(PieceVolume_m3) AS PieceVolume_m3,
            SUM(TotalVolume_m3) AS TotalVolume_m3,
            MAX(SourceStoreID) AS SourceStoreID,
            MAX(TargetStoreID) AS TargetStoreID
         FROM Production_CuttingOutput
         WHERE CutID = @ID
         GROUP BY ProductID, OutProductCode, ProductTypeID", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)

                    For Each r As DataRow In dt.Rows
                        Dim idx As Integer = dgvOutPut.Rows.Add()
                        With dgvOutPut.Rows(idx)
                            .Cells("colProductID").Value = r("ProductID")
                            .Cells("colOutProductCode").Value = r("OutProductCode")
                            .Cells("colOutProductTypeID").Value = r("ProductTypeID")
                            .Cells("colOutProductType").Value =
                            GetTypeName(CInt(r("ProductTypeID")))
                            .Cells("colOutLength").Value = r("Length_cm")
                            .Cells("colOutWidth").Value = r("Width_cm")
                            .Cells("colOutHeight").Value = r("Height_cm")
                            .Cells("colOutQty").Value = r("QtyPieces")
                            .Cells("colOutPieceVolume").Value = r("PieceVolume_m3")
                            .Cells("colOutTotalVolume").Value = r("TotalVolume_m3")
                        End With

                        ' أخذ أول مستودع من التفاصيل
                        If sourceStoreID = -1 AndAlso Not IsDBNull(r("SourceStoreID")) Then
                            sourceStoreID = CInt(r("SourceStoreID"))
                        End If

                        If targetStoreID = -1 AndAlso Not IsDBNull(r("TargetStoreID")) Then
                            targetStoreID = CInt(r("TargetStoreID"))
                        End If
                    Next
                End Using
            End Using

            ' =========================
            ' 3) تحميل المنتج الأب
            ' =========================
            Using cmd As New SqlCommand(
        "
        SELECT TOP 1 p.BaseProductID
        FROM Production_CuttingOutput co
        INNER JOIN Master_Product p
            ON p.ProductID = co.ProductID
        WHERE co.CutID = @ID
          AND p.BaseProductID IS NOT NULL
        ", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    cboProductCode.SelectedValue = CInt(obj)
                End If
            End Using

        End Using

        ' =========================
        ' تعبئة الهيدر
        ' =========================
        cboCuttingCode.DataSource = Nothing
        cboCuttingCode.Items.Clear()
        cboCuttingCode.Text = cuttingCode
        cboCuttingCode.Enabled = False

        ' المستودعات من التفاصيل
        cboSourceStore.SelectedValue = sourceStoreID
        cboTargetedStore.SelectedValue = targetStoreID
        txtCuttingStatus.Text = statusName
        dtpCuttingDate.Value = cutDate
        txtNotes.Text = notes
        txtCuttingStatus.Text = statusName
        txtCuttingStatus.Text = statusName

        If statusID = 2 Then
            btnSend.Enabled = True
        Else
            btnSend.Enabled = False
        End If
        ' =========================
        ' تثبيت المنتج الحالي
        ' =========================
        If cboProductCode.SelectedValue IsNot Nothing Then
            LastProductID = CInt(cboProductCode.SelectedValue)
        Else
            LastProductID = 0
        End If

        UpdateOutPutTotals()
        btnSave.Text = "Update"
        cboSourceStore.Enabled = False
        cboTargetedStore.Enabled = False
    End Sub
    Private Sub LoadCuttingInput(cuttingID As Integer)

        IsLoadingCutting = True

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand(
            "SELECT TOP 1 ProductID
             FROM Production_CuttingInput
             WHERE CutID = @ID", con)

                cmd.Parameters.AddWithValue("@ID", cuttingID)

                Dim obj = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    Dim productID As Integer = CInt(obj)

                    ' ✅ نخلي IsLoadingCutting = True إلى أن يتم تعيين الكمبو فعلياً
                    Me.BeginInvoke(Sub()
                                       cboProductCode.SelectedValue = productID
                                       IsLoadingCutting = False
                                   End Sub)
                Else
                    IsLoadingCutting = False
                End If

            End Using
        End Using

    End Sub

    Private Sub SetEditMode()

        ' =========================
        ' الهيدر (وضع تعديل)
        ' =========================

        ' ❌ منع تغيير المنتج
        cboProductCode.Enabled = False

        ' ❌ منع البحث عن منتج
        btnFind.Enabled = False

        ' ❌ منع تغيير المخزن والتاريخ
        cboSourceStore.Enabled = False
        dtpCuttingDate.Enabled = False

        ' ملاحظات (للعرض فقط حالياً)
        txtNotes.Enabled = True

        ' =========================
        ' الجريدات
        ' =========================

        ' ❌ لا إدخال جديد
        dgvInPut.Enabled = False

        ' ✔ يسمح بتعديل نواتج القص فقط
        dgvOutPut.Enabled = True

        ' =========================
        ' زر الحفظ
        ' =========================
        btnSave.Text = "Update"
        btnSave.Enabled = True

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub LoadAvailableQty(productID As Integer, storeID As Integer)

        If productID <= 0 OrElse storeID <= 0 Then
            txtAvailableQTY.Text = "0"
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT ISNULL(QtyOnHand, 0)
             FROM Inventory_Balance
             WHERE ProductID = @ProductID
               AND StoreID = @StoreID", con)

                cmd.Parameters.AddWithValue("@ProductID", productID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)

                con.Open()
                Dim Quantity = cmd.ExecuteScalar()

                txtAvailableQTY.Text =
                    If(Quantity Is Nothing OrElse IsDBNull(Quantity),
                       "0",
                       Convert.ToDecimal(Quantity).ToString("0.###"))
            End Using
        End Using

    End Sub
    Private Sub cboSourceStoreID_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboSourceStore.SelectedIndexChanged, cboTargetedStore.SelectedIndexChanged

        If cboSourceStore.SelectedIndex = -1 Then
            txtAvailableQTY.Text = "0"
            Exit Sub
        End If

        If cboProductCode.SelectedIndex = -1 Then
            txtAvailableQTY.Text = "0"
            Exit Sub
        End If
        If cboSourceStore.SelectedIndex <> -1 Then
        End If


        If IsLoadingCutting Then Exit Sub
        RefreshAvailableQty()

    End Sub
    Private Function GetTotalOutputVolume() As Decimal

        Dim total As Decimal = 0D

        For Each row As DataGridViewRow In dgvOutPut.Rows
            If row.IsNewRow Then Continue For

            total += GetDec(row.Cells("colOutTotalVolume").Value)
        Next

        Return total

    End Function
    Private Sub RefreshAvailableQty()

        If cboProductCode.SelectedValue Is Nothing Then Exit Sub
        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub

        If cboProductCode.SelectedIndex = -1 Then Exit Sub
        If cboSourceStore.SelectedIndex = -1 Then Exit Sub

        Dim productID As Integer = CInt(cboProductCode.SelectedValue)
        Dim storeID As Integer = CInt(cboSourceStore.SelectedValue)

        If productID <= 0 OrElse storeID <= 0 Then Exit Sub

        LoadAvailableQty(productID, storeID)

    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        If CurrentCuttingID <= 0 Then
            MessageBox.Show("لا يوجد سند قص للحفظ أولاً")
            Exit Sub
        End If

        Dim res = MessageBox.Show(
        "هل تريد إرسال عملية القص؟",
        "تأكيد الإرسال",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question)

        If res <> DialogResult.Yes Then Exit Sub

        Try
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("prod.SendCutting", con)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@CuttingID", CurrentCuttingID)
                    cmd.Parameters.AddWithValue("@UserID", 1)

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("تم إرسال عملية القص بنجاح")

            ' تحديث الحالة في الفورم
            RefreshFormStatus(CurrentCuttingID)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        btnSend.Enabled = False
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' =========================
        ' تحقق مبدئي
        ' =========================
        If cboProductCode.SelectedIndex = -1 Then
            MessageBox.Show("يرجى اختيار المنتج")
            Exit Sub
        End If

        If cboSourceStore.SelectedIndex = -1 Then
            MessageBox.Show("يرجى اختيار مخزن القص")
            Exit Sub
        End If

        If cboTargetedStore.SelectedIndex = -1 Then
            MessageBox.Show("يرجى اختيار مخزن المنتجات")
            Exit Sub
        End If

        If dgvOutPut.Rows.Count = 0 Then
            MessageBox.Show("لا توجد نواتج قص للحفظ")
            Exit Sub
        End If

        Dim availableQty As Decimal = GetDec(txtAvailableQTY.Text)
        Dim totalOutputQty As Decimal = GetTotalOutputVolume()

        If totalOutputQty > availableQty Then
            MessageBox.Show(
            "إجمالي الكمية المقصوصة أكبر من المتوفر",
            "تنبيه",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            Exit Sub
        End If

        ' =========================
        ' منع القص بدون ناتج صالح
        ' =========================
        Dim hasNonMixOutput As Boolean = False

        For Each r As DataGridViewRow In dgvOutPut.Rows
            If r.IsNewRow Then Continue For

            Dim typeID As Integer = CInt(r.Cells("colOutProductTypeID").Value)
            If typeID <> 4 Then
                hasNonMixOutput = True
                Exit For
            End If
        Next

        If Not hasNonMixOutput Then
            MessageBox.Show(
            "لا يمكن حفظ عملية قص بدون ناتج صالح." & vbCrLf &
            "كل النواتج مكس (Mix)." & vbCrLf &
            "يرجى إضافة منتج واحد على الأقل غير مكس.",
            "منع الحفظ",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            Exit Sub
        End If

        btnSave.Enabled = False
        Dim savedOk As Boolean = False

        Try
            ' =================================================
            ' 1) تجهيز DataTable للنواتج
            ' =================================================
            Dim dtOutputs As New DataTable()

            dtOutputs.Columns.Add("ProductID", GetType(Integer))
            dtOutputs.Columns.Add("QtyPieces", GetType(Integer))
            dtOutputs.Columns.Add("IsMix", GetType(Boolean))
            dtOutputs.Columns.Add("Notes", GetType(String))
            dtOutputs.Columns.Add("Length_cm", GetType(Decimal))
            dtOutputs.Columns.Add("Width_cm", GetType(Decimal))
            dtOutputs.Columns.Add("Height_cm", GetType(Decimal))
            dtOutputs.Columns.Add("PieceVolume_m3", GetType(Decimal))
            dtOutputs.Columns.Add("TotalVolume_m3", GetType(Decimal))
            dtOutputs.Columns.Add("ProductTypeID", GetType(Integer))
            dtOutputs.Columns.Add("OutProductCode", GetType(String))
            dtOutputs.Columns.Add("TargetStoreID", GetType(Integer))


            ' تعبئة النواتج من الجريد
            For Each r As DataGridViewRow In dgvOutPut.Rows
                If r.IsNewRow Then Continue For

                Dim outCode As String = r.Cells("colOutProductCode").Value.ToString()
                Dim typeID As Integer = CInt(r.Cells("colOutProductTypeID").Value)

                Dim l As Decimal = GetDec(r.Cells("colOutLength").Value)
                Dim w As Decimal = GetDec(r.Cells("colOutWidth").Value)
                Dim h As Decimal = GetDec(r.Cells("colOutHeight").Value)

                Dim outProductID As Integer =
                    GetOrCreateOutputProductID(outCode, typeID, l, w, h)

                If outProductID = 0 Then
                    MessageBox.Show("تم إلغاء ترميز المنتج")
                    Exit Sub
                End If

                Dim qtyPieces As Integer = CInt(GetDec(r.Cells("colOutQty").Value))
                Dim pv As Decimal = GetDec(r.Cells("colOutPieceVolume").Value)
                Dim tv As Decimal = GetDec(r.Cells("colOutTotalVolume").Value)

                Dim targetStoreID As Integer = CInt(cboTargetedStore.SelectedValue)

                dtOutputs.Rows.Add(
    outProductID,          ' ProductID
    qtyPieces,             ' QtyPieces
    (typeID = 4),           ' IsMix
    DBNull.Value,           ' Notes
    If(l > 0, l, DBNull.Value),
    If(w > 0, w, DBNull.Value),
    If(h > 0, h, DBNull.Value),
    If(pv > 0, pv, DBNull.Value),
    If(tv > 0, tv, DBNull.Value),
    typeID,                 ' ProductTypeID
    outCode,                ' OutProductCode
    targetStoreID           ' TargetStoreID
)

            Next


            ' =========================
            ' توليد كود القص من الإجراء
            ' =========================
            Dim cuttingCode As String = ""

            Using conCode As New SqlConnection(ConnStr)
                Using cmdCode As New SqlCommand("cfg.GetNextCode", conCode)
                    cmdCode.CommandType = CommandType.StoredProcedure
                    cmdCode.Parameters.AddWithValue("@CodeType", Me.FormScopeCode)

                    Dim pNextCode As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                    pNextCode.Direction = ParameterDirection.Output
                    cmdCode.Parameters.Add(pNextCode)

                    conCode.Open()
                    cmdCode.ExecuteNonQuery()
                    cuttingCode = pNextCode.Value.ToString()
                End Using
            End Using

            ' =================================================
            ' 2) استدعاء الإجراء المخزن
            ' =================================================
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("prod.SaveCuttingWITHMIX", con)
                    cmd.CommandType = CommandType.StoredProcedure

                    Dim pCutID As New SqlParameter("@CuttingID", SqlDbType.Int)
                    pCutID.Direction = ParameterDirection.InputOutput
                    If IsNewCutting Then
                        pCutID.Value = 0      ' إجبار Insert
                    Else
                        pCutID.Value = CurrentCuttingID   ' Update
                    End If

                    pCutID.Value = CurrentCuttingID
                    cmd.Parameters.Add(pCutID)
                    cmd.Parameters.AddWithValue("@CuttingCode", cuttingCode)

                    cmd.Parameters.AddWithValue("@CutDate", dtpCuttingDate.Value.Date)
                    cmd.Parameters.AddWithValue("@BaseProductID", CInt(cboProductCode.SelectedValue))
                    Dim consumedVolume As Decimal = GetTotalOutputVolume()
                    cmd.Parameters.AddWithValue("@ConsumedVolume_m3", consumedVolume)
                    cmd.Parameters.AddWithValue("@SourceStoreID", CInt(cboSourceStore.SelectedValue))
                    cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim())
                    cmd.Parameters.AddWithValue("@UserID", 1)

                    Dim p As New SqlParameter("@Outputs", SqlDbType.Structured)
                    p.TypeName = " prod.TVP_CuttingOutput"
                    p.Value = dtOutputs
                    cmd.Parameters.Add(p)

                    con.Open()
                    cmd.ExecuteNonQuery()

                    CurrentCuttingID = CInt(pCutID.Value)
                    IsNewCutting = False
                End Using
            End Using


            savedOk = True

            MessageBox.Show("تم حفظ عملية القص بنجاح")

        Catch ex As Exception
            MessageBox.Show(ex.Message)

        Finally
            btnSave.Enabled = Not savedOk

            If savedOk Then
                cboProductCode.Enabled = False
            End If
        End Try
        btnSave.Enabled = False
        btnSend.Enabled = True
    End Sub
    Private Function GetOrCreateOutputProductID(
    outCode As String,
    productTypeID As Integer,
    lengthCM As Decimal,
    widthCM As Decimal,
    heightCM As Decimal
) As Integer

        ' =========================
        ' 1) البحث عن المنتج بالكود + النوع
        ' =========================
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT ProductID
            FROM Master_Product
            WHERE ProductCode = @Code
              AND ProductTypeID = @TypeID
              AND IsActive = 1
        ", con)

                cmd.Parameters.AddWithValue("@Code", outCode)
                cmd.Parameters.AddWithValue("@TypeID", productTypeID)

                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    Return CInt(obj)
                End If
            End Using
        End Using

        ' =========================
        ' 2) المنتج غير موجود
        ' =========================

        ' إذا كان مكس → هذا خطأ في البيانات
        If productTypeID = 4 Then
            MessageBox.Show(
            "خطأ في تعريف نوع المكس للمنتج الأب." & vbCrLf &
            "كود المكس غير موجود في النظام.",
            "خطأ بيانات",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        )
            Return 0
        End If

        ' =========================
        ' 3) منتج سليم → فتح فورم الترميز
        ' =========================
        Using f As New frmProduct

            f.PrefillFromCutting(
            outCode,
            productTypeID,
            "",
            "",
            cboProductCode.Text,
            lengthCM,
            widthCM,
            heightCM,
            CInt(cboTargetedStore.SelectedValue)
        )

            If f.ShowDialog() = DialogResult.OK AndAlso f.SavedProductID > 0 Then
                Return f.SavedProductID
            Else
                Return 0
            End If
        End Using

    End Function


End Class
