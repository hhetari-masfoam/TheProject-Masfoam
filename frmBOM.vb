Imports System.Data.SqlClient

Public Class frmBOM
    Private CurrentBOMID As Integer = 0
    Private IsBOMDirty As Boolean = False
    Private IsLoadingBOMVersions As Boolean = False
    Private SelectedStoreID As Integer = 0
    Private SelectedCategoryID As Integer = 0
    Private SelectedSubCategoryID As Integer = 0
    Private SelectedProductGroupID As Integer = 0
    Private SelectedBaseUnitID As Integer = 0
    Private CurrentProductDensity As Decimal = 0D  'للحساب

    '   Private SelectedProductDensityID As Integer = 0

    ' =========================
    ' دالة مركزية لتفعيل الحفظ
    ' =========================

    Private Sub MarkAsDirty()

        If CurrentMode = FormMode.ViewMode Then
            IsBOMDirty = True
            btnSave.Enabled = True
        End If

    End Sub

    ' =========================
    ' دالة مركزية لتحميل قيم من الجداول
    ' =========================
    Private Sub LoadProductCore(productID As Integer)

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("core.GetProduct", con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID

                con.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If Not dr.Read() Then Exit Sub

                    ' =========================
                    ' العرض فقط (Display)
                    ' =========================
                    txtProductName.Text = dr("ProductName").ToString()

                    txtBaseUnitID.Text = dr("StorageUnitName").ToString()

                    txtDensity.Text =
                    If(IsDBNull(dr("Density")), "", dr("Density").ToString())

                    txtCategoryID.Text =
                    If(IsDBNull(dr("CategoryName")), "", dr("CategoryName").ToString())

                    txtSubCategoryID.Text =
                    If(IsDBNull(dr("SubCategoryName")), "", dr("SubCategoryName").ToString())

                    txtProductGroupID.Text =
                    If(IsDBNull(dr("GroupName")), "", dr("GroupName").ToString())

                    ' =========================
                    ' التخزين الداخلي (Internal)
                    ' =========================
                    If Not IsDBNull(dr("StorageUnitID")) Then
                        SelectedBaseUnitID = CInt(dr("StorageUnitID"))
                    Else
                        SelectedBaseUnitID = 0
                    End If

                    CurrentProductDensity =
                    If(IsDBNull(dr("Density")), 0D, CDec(dr("Density")))

                    ' =========================
                    ' Production Unit (قراءة فقط)
                    ' =========================
                    If Not IsDBNull(dr("ProductionUnitID")) Then
                        cboProductionUnit.SelectedValue = CInt(dr("ProductionUnitID"))
                    Else
                        cboProductionUnit.SelectedIndex = -1
                    End If

                    cboProductionUnit.Enabled = False

                End Using
            End Using
        End Using

    End Sub


    ' =========================
    ' تفريغ كمبو الصنف (cboProductID)
    ' =========================
    Private Sub ClearProductCombo()

        cboProductID.SelectedIndex = -1
        cboProductID.SelectedItem = Nothing
        cboProductID.Text = ""

    End Sub


    ' =========================
    ' تفريغ كمبو وحدة الإنتاج
    ' =========================
    Private Sub ClearProductionCombo()

        cboProductionUnit.SelectedIndex = -1
        cboProductionUnit.SelectedItem = Nothing

    End Sub


    ' =========================
    ' إعادة ضبط تاريخ الإنشاء
    ' =========================
    Private Sub ResetCreatedDate()

        dtpCreatedDate.Value = Date.Today

    End Sub



    ' =========================
    ' إعادة ضبط الفورم لحالة New
    ' =========================
    Private Sub ResetForNewBOM()

        IsFormLoading = True   ' ⛔ امنع الأحداث

        SelectedProductID = 0

        ClearProductCombo()
        ClearProductionCombo()
        ResetCreatedDate()
        txtDensity.Text = ""
        dgvProductSelect.Enabled = False
        dgvBOMDetails.Enabled = False

        IsFormLoading = False  ' ✅ أعد التفعيل

    End Sub



    ' =========================
    ' Connection String
    ' =========================
    Private ReadOnly ConnStr As String =
            "Data Source=DESKTOP-9FR2UBF\SQLEXPRESS01;Initial Catalog=MSDP;Integrated Security=True"
    Private SelectedProductID As Integer = 0

    ' حالات الفورم
    Private Enum FormMode
        NewMode
        ViewMode
        EditMode
    End Enum
    Private IsFormLoading As Boolean = False

    Private CurrentMode As FormMode = FormMode.NewMode


    ' تغيير حالة الفورم
    Private Sub SetFormMode(mode As FormMode)

        CurrentMode = mode

        Select Case mode

            Case FormMode.NewMode

                dgvProductSelect.Enabled = False
                dgvBOMDetails.Enabled = False
                dgvBOMDetails.Rows.Clear()

                txtBOMCode.Clear()
                txtProductionBaseQTY.Clear()
                txtCustomerID.Clear()
                txtNotes.Clear()
                txtProductGroupID.Clear()
                txtSubCategoryID.Clear()
                txtCategoryID.Clear()
                txtStoreID.Clear()
                txtProductName.Clear()
                txtBaseUnitID.Clear()

                cboProductionUnit.SelectedIndex = -1

                ' ✅ الحل هنا
                cboBOMVersion.DataSource = Nothing
                cboBOMVersion.Items.Clear()

                SelectedProductID = 0


            Case FormMode.ViewMode
                dgvProductSelect.Enabled = True
                dgvBOMDetails.Enabled = True

            Case FormMode.EditMode
                dgvProductSelect.Enabled = True
                dgvBOMDetails.Enabled = True

        End Select
        btnSave.Enabled = True
    End Sub



    ' =========================
    ' Form Load
    ' =========================

    ' =========================
    ' Load Units
    ' =========================
    Private Sub LoadProductionUnitID()
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT UnitID, UnitName FROM Master_Unit ORDER BY UnitID", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductionUnit.DataSource = dt
        cboProductionUnit.DisplayMember = "UnitName"
        cboProductionUnit.ValueMember = "UnitID"
        cboProductionUnit.SelectedIndex = -1



    End Sub
    Private Function ValidateProduct() As Boolean
        If cboProductionUnit.SelectedIndex = -1 Then

            MessageBox.Show("يرجى تعبئة جميع الحقول المطلوبة")
            Return False
        End If

        Return True

    End Function
    Private Sub frmBOM_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim screenW As Integer = Screen.PrimaryScreen.WorkingArea.Width
        Dim screenH As Integer = Screen.PrimaryScreen.WorkingArea.Height

        Me.Width = CInt(screenW * 0.85)
        Me.Height = CInt(screenH * 0.85)

        ' توسيط الفورم
        Me.StartPosition = FormStartPosition.CenterScreen

        IsFormLoading = True

        LoadProducts()
        LoadProductionUnitID()

        dgvProductSelect.Rows.Clear()
        dgvProductSelect.Rows.Add()

        dtpCreatedDate.Value = Date.Today

        ApplyStandardGridStyle(dgvProductSelect)
        ApplyStandardGridStyle(dgvBOMDetails)

        FormatMoneyColumn(dgvProductSelect, "colSelAvgCost", "التكلفة")
        FormatMoneyColumn(dgvBOMDetails, "colDetAvgCost", "التكلفة")

        ResetForNewBOM()

        IsFormLoading = False

    End Sub


    ' تفريغ بيانات الصنف
    Private Sub ClearProductFields()

        RemoveHandler cboProductID.SelectedValueChanged,
        AddressOf cboProductID_SelectedValueChanged

        SelectedProductID = 0
        cboProductID.SelectedIndex = -1

        txtProductName.Clear()
        txtBaseUnitID.Clear()
        txtStoreID.Clear()
        txtCategoryID.Clear()
        txtSubCategoryID.Clear()
        txtProductGroupID.Clear()

        AddHandler cboProductID.SelectedValueChanged,
        AddressOf cboProductID_SelectedValueChanged

    End Sub
    Private Sub LoadBOMVersions(productID As Integer)

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
        "SELECT
            BOMID,
            VersionNo,
            CustomerID,
            Notes,
            BOMCode,
            IsActive
         FROM Production_BOMHeader
         WHERE ProductID = @ProductID
         ORDER BY VersionNo ASC", con)

                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        IsLoadingBOMVersions = True

        If dt.Rows.Count = 0 Then
            cboBOMVersion.DataSource = Nothing
            cboBOMVersion.Items.Clear()
            CurrentBOMID = 0
            IsLoadingBOMVersions = False
            Exit Sub
        End If

        ' بعد ربط الـ DataSource
        cboBOMVersion.DataSource = dt
        cboBOMVersion.DisplayMember = "VersionNo"
        cboBOMVersion.ValueMember = "BOMID"
        cboBOMVersion.SelectedIndex = -1

        ' =========================
        ' اختيار آخر إصدار نشط
        ' =========================
        Dim lastActiveIndex As Integer = -1

        For i As Integer = 0 To dt.Rows.Count - 1
            If Not IsDBNull(dt.Rows(i)("IsActive")) AndAlso CBool(dt.Rows(i)("IsActive")) Then
                lastActiveIndex = i
            End If
        Next

        If lastActiveIndex >= 0 Then
            cboBOMVersion.SelectedIndex = lastActiveIndex
        End If


        IsLoadingBOMVersions = False
        RecalcTotalAvgCost()
    End Sub



    ' تحميل آخر BOM للصنف (يرجع True إذا وُجد BOM)
    Private Sub LoadBOMByID(bomID As Integer)
        CurrentBOMID = bomID
        dgvBOMDetails.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' تحميل الهيدر
            Using cmd As New SqlCommand(
            "SELECT
                BOMCode,
                VersionNo,
                ProductionUnitID,
                CustomerID,
            IsActive,
                Notes
             FROM Production_BOMHeader
             WHERE BOMID = @BOMID", con)

                cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = bomID

                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then
                        txtBOMCode.Text = dr("BOMCode").ToString()
                        '                       txtProductionBaseQTY.Text = CDec(dr("ProductionBaseQty")).ToString("N2")
                        cboProductionUnit.SelectedValue = CInt(dr("ProductionUnitID"))
                        txtCustomerID.Text = If(IsDBNull(dr("CustomerID")), "", dr("CustomerID").ToString())
                        txtNotes.Text = If(IsDBNull(dr("Notes")), "", dr("Notes").ToString())
                        Dim isActive As Boolean = CBool(dr("IsActive"))

                        chkIsActive.Checked = isActive

                        If isActive Then
                            lblBOMStatus.Text = "نشطة"
                            lblBOMStatus.ForeColor = Color.Green
                            btnDeactivateBOM.Text = "تعطيل BOM"
                        Else
                            lblBOMStatus.Text = "غير نشطة"
                            lblBOMStatus.ForeColor = Color.Red
                            btnDeactivateBOM.Text = "تنشيط BOM"
                        End If

                    End If
                End Using
            End Using

            ' تحميل التفاصيل
            Using cmd As New SqlCommand(
            "SELECT
                d.ComponentProductID,
                p.ProductCode,
                p.ProductName,
                d.Quantity,
                d.UnitID,
                u.UnitName        AS BaseUnitName,
                p.AvgCost,
                p.IsActive
            FROM Production_BOMDetails d
            JOIN Master_Product p ON p.ProductID = d.ComponentProductID
            LEFT JOIN Master_Unit u ON u.UnitID = d.UnitID
            WHERE d.BOMID = @BOMID
            ORDER BY d.LineNumber
            ", con)

                cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = bomID

                Using dr = cmd.ExecuteReader()
                    While dr.Read()
                        Dim i = dgvBOMDetails.Rows.Add()
                        With dgvBOMDetails.Rows(i)
                            .Cells("colDetProductID").Value = CInt(dr("ComponentProductID"))
                            .Cells("colDetProductCode").Value = dr("ProductCode").ToString()
                            .Cells("colDetProductName").Value = dr("ProductName").ToString()
                            .Cells("colDetProductQty").Value = dr("Quantity")
                            .Cells("colDetBaseUnitID").Value = dr("UnitID")
                            .Cells("colDetBaseUnitName").Value = dr("BaseUnitName").ToString()
                            .Cells("colDetAvgCost").Value = dr("AvgCost")
                            .Cells("colDetIsActive").Value = CBool(dr("IsActive"))
                        End With


                    End While
                End Using
            End Using

        End Using
        IsBOMDirty = False
        RecalcTotalAvgCost()
    End Sub


    Private Sub cboBOMVersion_SelectedValueChanged(
    sender As Object, e As EventArgs
) Handles cboBOMVersion.SelectedValueChanged

        If IsLoadingBOMVersions Then Exit Sub
        If IsFormLoading Then Exit Sub
        If cboBOMVersion.SelectedValue Is Nothing Then Exit Sub
        If Not Integer.TryParse(cboBOMVersion.SelectedValue.ToString(), Nothing) Then Exit Sub

        LoadBOMByID(CInt(cboBOMVersion.SelectedValue))

        btnNewBOM.Enabled = True

    End Sub

    '========================================
    ' تحميل الأصناف (كود + اسم)
    '========================================
    Private Sub LoadProducts()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductID, ProductCode, ProductName, StorageUnitID, ProductCategoryID, ProductSubCategoryID,ProductGroupID
             FROM Master_Product
             WHERE IsActive = 1
             ORDER BY ProductCode", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using
        End Using

        ' ربط الكمبو الرئيسي
        cboProductID.DataSource = dt
        cboProductID.DisplayMember = "ProductCode"
        cboProductID.ValueMember = "ProductID"
        cboProductID.SelectedIndex = -1

        ' ربط كمبو الجريد
        Dim col As DataGridViewComboBoxColumn =
        CType(dgvProductSelect.Columns("colSelProductCode"),
              DataGridViewComboBoxColumn)

        col.DataSource = dt.Copy()
        col.DisplayMember = "ProductCode"
        col.ValueMember = "ProductID"

    End Sub

    '========================================
    ' عند اختيار صنف من الكمبو
    '========================================
    Private Sub cboProductID_SelectedValueChanged(
    sender As Object, e As EventArgs
) Handles cboProductID.SelectedValueChanged

        ' 🔒 حمايات أساسية
        If IsFormLoading Then Exit Sub
        If cboProductID.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboProductID.SelectedValue) Then Exit Sub

        SelectedProductID = CInt(cboProductID.SelectedValue)

        ' =====================================================
        ' 1️⃣ اسم الصنف فقط من نفس DataSource (مسموح)
        ' =====================================================
        Dim drv As DataRowView = TryCast(cboProductID.SelectedItem, DataRowView)
        If drv IsNot Nothing Then
            txtProductName.Text = drv("ProductName").ToString()
        End If

        ' =====================================================
        ' 2️⃣ تحميل باقي البيانات من الجداول المرتبطة (الصحيح)
        ' =====================================================
        LoadProductCore(SelectedProductID)

        ' =====================================================
        ' 3️⃣ تحميل الـ BOM
        ' =====================================================
        SetFormMode(FormMode.ViewMode)
        LoadBOMVersions(SelectedProductID)
        cboProductionUnit.Enabled = False
        '        txtProductionBaseQTY.ReadOnly = True

        '        If cboBOMVersion.Items.Count > 0 Then
        '       cboBOMVersion.SelectedIndex = 0   ' آخر Version تلقائيًا
        '      End If

    End Sub

    Private Sub btnProductSearch_Click(sender As Object, e As EventArgs) Handles btnProductSearch.Click

        Using f As New frmProductSearch()

            If f.ShowDialog() = DialogResult.OK Then

                IsFormLoading = True   ' ⛔ امنع أي أحداث

                ' 1️⃣ رجوع كامل لوضع New
                ResetForNewBOM()
                SetFormMode(FormMode.NewMode)

                ' 2️⃣ تثبيت الصنف الجديد
                SelectedProductID = f.SelectedProductID
                cboProductID.SelectedValue = SelectedProductID

                IsFormLoading = False  ' ✅ أعد تفعيل الأحداث

            End If

        End Using

    End Sub





    Private Sub btnSearchBOM_Click(sender As Object, e As EventArgs) Handles btnSearchBOM.Click

        Using f As New frmBOMSearch()

            If f.ShowDialog() <> DialogResult.OK Then Exit Sub

            IsFormLoading = True   ' ⛔ إيقاف أحداث الفورم العامة

            ' =================================================
            ' 1️⃣ إعادة الفورم لوضع New (كما كان)
            ' =================================================
            ResetForNewBOM()
            SetFormMode(FormMode.NewMode)

            ' =================================================
            ' 2️⃣ جلب ProductID المرتبط بالـ BOM المختار
            ' =================================================
            Dim productID As Integer

            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand(
                "SELECT ProductID FROM Production_BOMHeader WHERE BOMID = @BOMID", con)

                    cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = f.SelectedBOMID
                    con.Open()

                    Dim r = cmd.ExecuteScalar()
                    If r Is Nothing OrElse IsDBNull(r) Then
                        IsFormLoading = False
                        Exit Sub
                    End If

                    productID = CInt(r)
                End Using
            End Using

            ' =================================================
            ' 3️⃣ تحديد الصنف (يشغّل منطق التحميل الطبيعي)
            ' =================================================
            SelectedProductID = productID
            cboProductID.SelectedValue = productID

            IsFormLoading = False  ' ✅ إعادة تفعيل الأحداث

            ' =================================================
            ' 4️⃣ تحميل BOM المختار صراحة (لا نعتمد على Events)
            ' =================================================
            CurrentBOMID = f.SelectedBOMID
            LoadBOMByID(CurrentBOMID)

            SetFormMode(FormMode.ViewMode)

        End Using

    End Sub





    ' =========================
    ' تفعيل التغيير الفوري للكمبو
    ' =========================
    Private Sub dgvProductSelect_CurrentCellDirtyStateChanged(
    sender As Object, e As EventArgs
        ) Handles dgvProductSelect.CurrentCellDirtyStateChanged


        If dgvProductSelect.IsCurrentCellDirty Then
            dgvProductSelect.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    ' =========================
    ' عند اختيار المنتج
    ' =========================
    Private Sub dgvProductSelect_CellValueChanged(
    sender As Object, e As DataGridViewCellEventArgs
) Handles dgvProductSelect.CellValueChanged

        If e.RowIndex < 0 Then Exit Sub

        ' نتحقق مرة واحدة فقط
        If dgvProductSelect.Columns(e.ColumnIndex).Name <> "colSelProductCode" Then Exit Sub

        Dim cell = dgvProductSelect.Rows(e.RowIndex).Cells("colSelProductCode")

        ' حمايات مهمة
        If cell.Value Is Nothing Then Exit Sub
        If Not IsNumeric(cell.Value) Then Exit Sub

        Dim productID As Integer = CInt(cell.Value)

        LoadProductCoreForGrid(e.RowIndex, productID)
        FocusQtyCell(e.RowIndex)

    End Sub

    Private Sub LoadProductCoreForGrid(rowIndex As Integer, productID As Integer)

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("core.GetProduct", con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID

                con.Open()
                Using dr = cmd.ExecuteReader()
                    If Not dr.Read() Then Exit Sub

                    With dgvProductSelect.Rows(rowIndex)

                        ' 👁️ العرض
                        .Cells("colSelProductName").Value =
                        dr("ProductName").ToString()

                        .Cells("colSelBaseUnitID").Value =
                        dr("StorageUnitID")        ' ID لأن العمود ComboBox
                        .Cells("colSelBaseUnitName").Value = dr("StorageUnitName")

                        .Cells("colSelAvgCost").Value = dr("AvgCost")

                        .Cells("colSelIsActive").Value =
                        CBool(dr("IsActive"))
                        With dgvProductSelect.Rows(rowIndex)




                        End With

                    End With
                End Using
            End Using
        End Using

    End Sub



    ' =========================
    ' تحميل خصائص المنتج
    ' =========================


    Private Sub dgvProductSelect_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs) _


        ' تجاهل أخطاء الكمبوبكس غير الصالحة
        e.ThrowException = False

    End Sub
    Private OriginalRowBackup As Dictionary(Of String, Object) = Nothing

    Private Sub dgvProductSelect_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvProductSelect.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        Dim colName As String = dgvProductSelect.Columns(e.ColumnIndex).Name
        Dim srcRow As DataGridViewRow = dgvProductSelect.Rows(e.RowIndex)

        ' =========================
        ' 🔍 زر البحث عن صنف
        If colName = "colSelSearch" Then
            Using f As New frmProductSearch()
                If f.ShowDialog() = DialogResult.OK Then

                    If cboProductID.SelectedValue IsNot Nothing AndAlso
               CInt(cboProductID.SelectedValue) = f.SelectedProductID Then

                        MessageBox.Show("لا يمكن اختيار المنتج المصنع كمكوّن", "تنبيه")
                        Exit Sub
                    End If

                    srcRow.Cells("colSelProductCode").Value = f.SelectedProductID
                    ' سيُستكمل التحميل تلقائيًا عبر CellValueChanged
                End If
            End Using
            Exit Sub
        End If

        ' =========================
        ' ❌ زر تفريغ سطر الإدخال
        ' =========================
        If colName = "colSelDelete" Then

            srcRow.Cells("colSelProductCode").Value = Nothing
            srcRow.Cells("colSelProductName").Value = Nothing

            srcRow.Cells("colSelBaseUnitID").Value = Nothing
            srcRow.Cells("colSelBaseUnitName").Value = Nothing

            srcRow.Cells("colSelProductQty").Value = Nothing
            srcRow.Cells("colSelAvgCost").Value = Nothing
            srcRow.Cells("colSelIsActive").Value = Nothing

            Exit Sub
        End If

        ' =========================
        ' ➕ زر الإضافة إلى تفاصيل BOM
        ' =========================
        If colName = "colSelAdd" Then

            ' تحقق أساسي
            If srcRow.Cells("colSelProductCode").Value Is Nothing Then
                MessageBox.Show("اختر الصنف أولاً")
                Exit Sub
            End If

            ' تحقق الكمية
            Dim Quantity As Decimal
            If Not Decimal.TryParse(
            srcRow.Cells("colSelProductQty").Value?.ToString(), Quantity
        ) OrElse Quantity <= 0 Then

                MessageBox.Show("أدخل كمية صحيحة")

                Me.BeginInvoke(Sub()
                                   dgvProductSelect.CurrentCell =
                                   srcRow.Cells("colSelProductQty")
                                   dgvProductSelect.BeginEdit(True)
                               End Sub)
                Exit Sub
            End If

            Dim productID As Integer =
            CInt(srcRow.Cells("colSelProductCode").Value)

            ' منع التكرار
            For Each r As DataGridViewRow In dgvBOMDetails.Rows
                If r.IsNewRow Then Continue For
                If CInt(r.Cells("colDetProductID").Value) = productID Then
                    MessageBox.Show("الصنف مضاف مسبقًا")
                    Exit Sub
                End If
            Next

            ' إضافة سطر جديد للتفاصيل
            Dim newIndex As Integer = dgvBOMDetails.Rows.Add()

            With dgvBOMDetails.Rows(newIndex)

                ' 🔒 التخزين الداخلي
                .Cells("colDetProductID").Value = productID

                .Cells("colDetProductCode").Value =
srcRow.Cells("colSelProductCode").FormattedValue

                .Cells("colDetBaseUnitID").Value =
                srcRow.Cells("colSelBaseUnitID").Value
                ' 👁️ العرض
                .Cells("colDetProductName").Value =
                srcRow.Cells("colSelProductName").Value

                .Cells("colDetBaseUnitName").Value =
                srcRow.Cells("colSelBaseUnitName").Value

                .Cells("colDetProductQty").Value = Quantity

                .Cells("colDetAvgCost").Value =
                srcRow.Cells("colSelAvgCost").Value

                .Cells("colDetIsActive").Value =
                srcRow.Cells("colSelIsActive").Value

            End With

            MarkAsDirty()

            ' تفريغ سطر الإدخال
            srcRow.Cells("colSelProductCode").Value = Nothing
            srcRow.Cells("colSelProductName").Value = Nothing
            srcRow.Cells("colSelBaseUnitID").Value = Nothing
            srcRow.Cells("colSelBaseUnitName").Value = Nothing
            srcRow.Cells("colSelProductQty").Value = Nothing
            srcRow.Cells("colSelAvgCost").Value = Nothing
            srcRow.Cells("colSelIsActive").Value = Nothing

            Exit Sub
        End If


        RecalcTotalAvgCost()
    End Sub

    Private Sub BackupOriginalRow(srcRow As DataGridViewRow)

        OriginalRowBackup = New Dictionary(Of String, Object) From {
            {"ProductCode", srcRow.Cells("colDetProductCode").Value},
            {"ProductName", srcRow.Cells("colDetProductName").Value},
            {"StorageUnitID", srcRow.Cells("colDetBaseUnitID").Value},
            {"StorageUnitName", srcRow.Cells("colDetBaseUnitName").Value},
            {"Quantity", srcRow.Cells("colDetProductQty").Value},
              {"AvgCost", srcRow.Cells("colDetAvgCost").Value},
               {"IsActive", srcRow.Cells("colDetIsActive").Value}
            }
        RecalcTotalAvgCost()
    End Sub

    Private Sub RestoreOriginalRowToDetails()

        If OriginalRowBackup Is Nothing Then Exit Sub

        Dim idx As Integer = dgvBOMDetails.Rows.Add()

        With dgvBOMDetails.Rows(idx)
            .Cells("colDetProductCode").Value = OriginalRowBackup("ProductCode")
            .Cells("colDetProductName").Value = OriginalRowBackup("ProductName")
            .Cells("colDetBaseUnitID").Value = OriginalRowBackup("StorageUnitID")
            .Cells("colDetProductQty").Value = OriginalRowBackup("Quantity")
            .Cells("colDetAvgCost").Value = OriginalRowBackup("AvgCost")
            .Cells("colDetIsActive").Value = OriginalRowBackup("IsActive")
        End With


        OriginalRowBackup = Nothing
        RecalcTotalAvgCost()
    End Sub
    Private Sub dgvBOMDetails_CellDoubleClick(
    sender As Object, e As DataGridViewCellEventArgs
) Handles dgvBOMDetails.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub
        If dgvBOMDetails.Rows(e.RowIndex).IsNewRow Then Exit Sub

        ' إذا كان هناك صف سابق قيد التعديل → أعده كما كان
        If OriginalRowBackup IsNot Nothing Then
            RestoreOriginalRowToDetails()
        End If

        ' خذ الصف الجديد
        Dim srcRow = dgvBOMDetails.Rows(e.RowIndex)

        ' خزّن النسخة الأصلية فقط
        BackupOriginalRow(srcRow)

        ' انقله للأعلى
        Dim editRow = dgvProductSelect.Rows(0)

        editRow.Cells("colSelProductCode").Value = srcRow.Cells("colDetProductID").Value ' ✅ الأهم
        editRow.Cells("colSelProductName").Value = srcRow.Cells("colDetProductName").Value
        editRow.Cells("colSelBaseUnitID").Value = srcRow.Cells("colDetBaseUnitID").Value
        editRow.Cells("colSelProductQty").Value = srcRow.Cells("colDetProductQty").Value
        editRow.Cells("colSelAvgCost").Value = srcRow.Cells("colDetAvgCost").Value
        editRow.Cells("colSelIsActive").Value = srcRow.Cells("colDetIsActive").Value


        ' احذف الصف من الأسفل
        dgvBOMDetails.Rows.RemoveAt(e.RowIndex)


        RecalcTotalAvgCost()
    End Sub
    Private Sub dgvProductSelect_EditingControlShowing(
    sender As Object, e As DataGridViewEditingControlShowingEventArgs
) Handles dgvProductSelect.EditingControlShowing

        If dgvProductSelect.CurrentCell.ColumnIndex =
        dgvProductSelect.Columns("colSelProductQty").Index Then

            Dim tb As TextBox = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                RemoveHandler tb.KeyPress, AddressOf Qty_KeyPress
                AddHandler tb.KeyPress, AddressOf Qty_KeyPress
            End If
        End If

    End Sub

    Private Sub Qty_KeyPress(sender As Object, e As KeyPressEventArgs)

        ' السماح بالأرقام + Backspace + فاصلة عشرية
        If Not Char.IsDigit(e.KeyChar) AndAlso
       e.KeyChar <> "."c AndAlso
       e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If

        ' منع أكثر من فاصلة عشرية
        Dim tb As TextBox = CType(sender, TextBox)
        If e.KeyChar = "."c AndAlso tb.Text.Contains(".") Then
            e.Handled = True
        End If

    End Sub
    Private Sub dgvProductSelect_CellValidating(
       sender As Object, e As DataGridViewCellValidatingEventArgs
   ) Handles dgvProductSelect.CellValidating


        If dgvProductSelect.Columns(e.ColumnIndex).Name <> "colSelProductQty" Then Exit Sub

        Dim Quantity As Decimal
        If Not Decimal.TryParse(e.FormattedValue.ToString(), Quantity) OrElse Quantity <= 0 Then
            MessageBox.Show("الكمية يجب أن تكون رقمًا أكبر من صفر")
            e.Cancel = True
            dgvProductSelect.CurrentCell =
       dgvProductSelect.Rows(e.RowIndex).Cells("colSelProductQty")

            dgvProductSelect.BeginEdit(True)
        End If

    End Sub
    ' =====================================================
    ' نقل المؤشر تلقائيًا إلى خلية الكمية وبدء الكتابة
    ' =====================================================
    Private Sub FocusQtyCell(rowIndex As Integer)

        ' تحديد خلية الكمية كسيل حالية
        dgvProductSelect.CurrentCell =
        dgvProductSelect.Rows(rowIndex).Cells("colSelProductQty")

        ' إدخال وضع التحرير مباشرة
        dgvProductSelect.BeginEdit(True)

    End Sub
    '  ==========================================================================================================
    Private Sub ApplyStandardGridStyle(dgv As DataGridView)

        With dgv
            .EnableHeadersVisualStyles = False

            ' عناوين الأعمدة – وسط
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment =
            DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeight = 34

            ' الخلايا
            .DefaultCellStyle.Font = New Font("Segoe UI", 9)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            ' الشبكة والصفوف
            .GridColor = Color.Gainsboro
            .RowTemplate.Height = 28
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)

            ' السلوك
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        End With

    End Sub
    Private Sub FormatMoneyColumn(
    dgv As DataGridView,
    columnName As String,
    Optional headerText As String = Nothing
)

        If Not dgv.Columns.Contains(columnName) Then Exit Sub

        With dgv.Columns(columnName)
            .DefaultCellStyle.Format = "N2" ' فاصلة آلاف + رقمين عشريين
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' تحسين شكلي
            .DefaultCellStyle.ForeColor = Color.DarkBlue
            .DefaultCellStyle.Font = New Font(dgv.Font, FontStyle.Bold)

            If headerText IsNot Nothing Then
                .HeaderText = headerText
            End If
        End With

    End Sub
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' =========================
        ' 1️⃣ Validation (كما كان)
        ' =========================
        If SelectedProductID = 0 Then
            MessageBox.Show("اختر الصنف أولاً")
            Exit Sub
        End If

        If cboProductionUnit.SelectedIndex = -1 Then
            MessageBox.Show("اختر وحدة الإنتاج")
            Exit Sub
        End If

        If dgvBOMDetails.Rows.Count = 0 Then
            MessageBox.Show("لا توجد مكونات في BOM")
            Exit Sub
        End If


        ' =========================
        ' 2️⃣ تحديد SaveMode (جديد)
        ' =========================
        Dim saveMode As String = "NEWVER"

        If CurrentBOMID > 0 Then
            Dim r = MessageBox.Show(
            "هل تريد تعديل الفرجن الحالي؟" & vbCrLf &
            "نعم = تعديل الفرجن الحالي" & vbCrLf &
            "لا = إنشاء فرجن جديد",
            "حفظ BOM",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question
        )

            If r = DialogResult.Cancel Then Exit Sub
            If r = DialogResult.Yes Then saveMode = "UPDATE"
            If r = DialogResult.No Then saveMode = "NEWVER"
        End If

        ' =========================
        ' 3️⃣ تجهيز تفاصيل BOM (كما كان + TVP)
        ' =========================
        Dim dtDetails As New DataTable()
        dtDetails.Columns.Add("ComponentProductID", GetType(Integer))
        dtDetails.Columns.Add("Quantity", GetType(Decimal))
        dtDetails.Columns.Add("UnitID", GetType(Integer))
        dtDetails.Columns.Add("LineNumber", GetType(Integer))

        Dim lineNo As Integer = 1

        For Each r As DataGridViewRow In dgvBOMDetails.Rows
            If r.IsNewRow Then Continue For

            dtDetails.Rows.Add(
            CInt(r.Cells("colDetProductID").Value),
            CDec(r.Cells("colDetProductQty").Value),
            CInt(r.Cells("colDetBaseUnitID").Value),
            lineNo
        )

            lineNo += 1
        Next

        ' =========================
        ' 4️⃣ استدعاء الإجراء
        ' =========================
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("core.SaveBOM", con)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@SaveMode", saveMode)

                cmd.Parameters.AddWithValue("@BOMID",
                If(saveMode = "UPDATE",
                   CurrentBOMID,
                   CType(DBNull.Value, Object)))

                cmd.Parameters.AddWithValue("@ProductID", SelectedProductID)
                '                cmd.Parameters.AddWithValue("@ProductionBaseQty", CDec(txtProductionBaseQTY.Text))
                cmd.Parameters.AddWithValue("@ProductionUnitID", CInt(cboProductionUnit.SelectedValue))

                Dim customerIDValue As Object = DBNull.Value

                If Integer.TryParse(txtCustomerID.Text.Trim(), Nothing) Then
                    customerIDValue = CInt(txtCustomerID.Text.Trim())
                End If

                cmd.Parameters.AddWithValue("@CustomerID", customerIDValue)

                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim())
                cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked)

                cmd.Parameters.AddWithValue("@CreatedDate", dtpCreatedDate.Value)
                cmd.Parameters.AddWithValue("@CreatedBy", CurrentUser.LoginName)

                Dim pDetails = cmd.Parameters.AddWithValue("@Details", dtDetails)
                pDetails.SqlDbType = SqlDbType.Structured
                pDetails.TypeName = "core.BOMDetailType"


                cmd.Parameters.Add("@OutBOMID", SqlDbType.Int).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@OutBOMCode", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output
                cmd.Parameters.Add("@OutVersionNo", SqlDbType.Int).Direction = ParameterDirection.Output

                con.Open()
                cmd.ExecuteNonQuery()

                ' =========================
                ' 5️⃣ معالجة النتائج
                ' =========================
                CurrentBOMID = CInt(cmd.Parameters("@OutBOMID").Value)
                txtBOMCode.Text = cmd.Parameters("@OutBOMCode").Value.ToString()

                MessageBox.Show(
                "تم حفظ BOM بنجاح" & vbCrLf &
                "رقم الفرجن: " & cmd.Parameters("@OutVersionNo").Value.ToString(),
                "نجاح",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            End Using
        End Using

        ' =========================
        ' 6️⃣ تحديث وضع الفورم
        ' =========================
        SetFormMode(FormMode.EditMode)

    End Sub

    Private Sub btnDeactivateBOM_Click(
     sender As Object, e As EventArgs) Handles btnDeactivateBOM.Click

        ' لا يوجد BOM محمّل
        If CurrentBOMID = 0 Then Exit Sub

        ' لا نسمح بالتغيير في وضع New
        If CurrentMode = FormMode.NewMode Then Exit Sub

        Dim actionText As String

        If chkIsActive.Checked Then
            actionText = "هل تريد تعطيل هذا الإصدار؟"
        Else
            actionText = "هل تريد تنشيط هذا الإصدار؟"
        End If

        If MessageBox.Show(
        actionText,
        "تأكيد",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    ) = DialogResult.No Then Exit Sub
        Dim newState As Boolean = Not chkIsActive.Checked

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand(
            "UPDATE Production_BOMHeader
             SET IsActive = @IsActive
             WHERE BOMID = @BOMID", con)

                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = newState
                cmd.Parameters.Add("@BOMID", SqlDbType.Int).Value = CurrentBOMID

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' تحديث الواجهة فقط
        chkIsActive.Checked = newState

        If newState Then
            btnDeactivateBOM.Text = "تعطيل BOM"
            lblBOMStatus.Text = "نشطة"
            lblBOMStatus.ForeColor = Color.Green
        Else
            btnDeactivateBOM.Text = "تنشيط BOM"
            lblBOMStatus.Text = "غير نشطة"
            lblBOMStatus.ForeColor = Color.Red
        End If


    End Sub

    Private Sub btnNewBOM_Click(sender As Object, e As EventArgs) Handles btnNewBOM.Click

        IsFormLoading = True   ' ⛔ امنع الأحداث مؤقتًا

        ResetForNewBOM()
        SetFormMode(FormMode.NewMode)

        IsFormLoading = False  ' ✅ أعد تفعيل الأحداث

    End Sub
    'Private Sub txtProductionBaseQTY_TextChanged(
    'sender As Object, e As EventArgs) Handles txtProductionBaseQTY.TextChanged
    '   MarkAsDirty()
    'End Sub

    Private Sub cboProductionUnit_SelectedValueChanged(
    sender As Object, e As EventArgs) Handles cboProductionUnit.SelectedValueChanged
        If IsFormLoading Then Exit Sub
        MarkAsDirty()
    End Sub

    Private Sub txtCustomerID_TextChanged(
    sender As Object, e As EventArgs) Handles txtCustomerID.TextChanged, txtDensity.TextChanged
        MarkAsDirty()
    End Sub

    Private Sub txtNotes_TextChanged(
    sender As Object, e As EventArgs) Handles txtNotes.TextChanged
        MarkAsDirty()
    End Sub
    Private Sub LoadProductByID(productID As Integer)
        ' مؤقتًا فقط للتجربة
        MessageBox.Show("ProductID = " & productID.ToString())
    End Sub
    Private Sub frmBOM_FormClosing(
    sender As Object,
    e As FormClosingEventArgs
) Handles Me.FormClosing

        ' إذا الفورم في NewMode ولا يوجد BOM محمّل → خروج مباشر
        If CurrentMode = FormMode.NewMode AndAlso CurrentBOMID = 0 Then Exit Sub

        Dim result = MessageBox.Show(
        "توجد تغييرات غير محفوظة، هل تريد الحفظ قبل الإغلاق؟",
        "تأكيد الإغلاق",
        MessageBoxButtons.YesNoCancel,
        MessageBoxIcon.Question
    )

        Select Case result
            Case DialogResult.Yes
                btnSave.PerformClick()

            Case DialogResult.No
            ' تجاهل

            Case DialogResult.Cancel
                e.Cancel = True
        End Select

    End Sub

    Private Sub btnCloseBOM_Click(sender As Object, e As EventArgs) Handles btnCloseBOM.Click
        Me.Close()
    End Sub
    Private Sub dgvBOMDetails_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvBOMDetails.CellValueChanged

        If IsFormLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        ' استخدم نفس الجريد الذي أطلق الحدث
        Dim grid = dgvBOMDetails

        ' حماية من الخروج عن النطاق
        If e.RowIndex >= grid.Rows.Count Then Exit Sub
        If e.ColumnIndex >= grid.Columns.Count Then Exit Sub

        If grid.Columns(e.ColumnIndex).Name <> "colSelProductID" Then Exit Sub

        Dim selectedIDObj = grid.Rows(e.RowIndex).Cells("colSelProductID").Value
        If selectedIDObj Is Nothing OrElse cboProductID.SelectedValue Is Nothing Then Exit Sub

        Dim selectedID As Integer
        If Not Integer.TryParse(selectedIDObj.ToString(), selectedID) Then Exit Sub

        Dim headerID As Integer
        If Not Integer.TryParse(cboProductID.SelectedValue.ToString(), headerID) Then Exit Sub

        If selectedID = headerID Then
            MessageBox.Show("لا يمكن اختيار نفس منتج الهيدر كمكوّن", "تنبيه")
            grid.Rows(e.RowIndex).Cells("colSelProductID").Value = Nothing
            Exit Sub
        End If

        RecalcTotalAvgCost()
        MarkAsDirty()
    End Sub

    Private Sub RecalcTotalAvgCost()

        Dim total As Decimal = 0D

        For Each r As DataGridViewRow In dgvBOMDetails.Rows
            If r.IsNewRow Then Continue For

            Dim Quantity As Decimal = CDec(r.Cells("colDetProductQty").Value)
            Dim avgCost As Decimal = CDec(r.Cells("colDetAvgCost").Value)

            total += Quantity * avgCost
        Next

        lblTotalAvgCost.Text = total.ToString("N2")

    End Sub
    Private Sub dgvProductSelect_CellPainting(
    sender As Object,
    e As DataGridViewCellPaintingEventArgs
) Handles dgvProductSelect.CellPainting

        If e.RowIndex < 0 Then Exit Sub

        If TypeOf dgvProductSelect.Columns(e.ColumnIndex) Is DataGridViewButtonColumn Then

            e.PaintBackground(e.ClipBounds, True)

            Dim rect As Rectangle = e.CellBounds
            rect.Inflate(-4, -4)

            ' 🎨 لون رمادي رسمي (قريب جدًا من الويندوز)
            Dim backColor As Color = Color.FromArgb(240, 240, 240)
            Dim borderColor As Color = Color.FromArgb(180, 180, 180)
            Dim textColor As Color = Color.Black

            ' خلفية الزر
            Using br As New SolidBrush(backColor)
                e.Graphics.FillRectangle(br, rect)
            End Using

            ' إطار الزر
            Using pen As New Pen(borderColor)
                e.Graphics.DrawRectangle(pen, rect)
            End Using

            ' النص
            TextRenderer.DrawText(
            e.Graphics,
            e.FormattedValue.ToString(),
            dgvProductSelect.Font,
            rect,
            textColor,
            TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter
        )

            e.Handled = True
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

    End Sub
End Class
