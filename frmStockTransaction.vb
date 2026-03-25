Imports System.Data.SqlClient
Imports THE_PROJECT.frmProductSearch

Public Class frmStockTransaction
    Inherits AABaseOperationForm
    Private IsEditMode As Boolean = False
    '=========================================
    ' اتصال قاعدة البيانات
    '=========================================

    '=========================================
    ' متغيرات الحالة
    '=========================================
    Private CurrentTransactionID As Integer = 0
    Private CurrentUserID As Integer = 1   ' مؤقتًا
    Private EditingTransactionID As Integer = 0
    ' =========================
    ' يحدد وضع الفورم: إرسال أو استقبال
    ' =========================
    Private _isLoadingStores As Boolean = False
    Private CurrentMode As StockTransactionMode
    Private CurrentListMode As TransfersListMode = TransfersListMode.NormalList
    Private _isLoadingTransaction As Boolean = False
    Private _allowLeaveGrid As Boolean = False
    Private CurrentProductionID As Integer = 0
    Private WithEvents txtCancelReason As New TextBox()
    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "TRN"
        End Get
    End Property
    Public Enum StockTransactionMode
        TransferSend = 1
        TransferReceive = 2
        PurchaseReceive = 3
        ProductionReceive = 4
        CuttingReceive = 5
        SalesReturnReceive = 6
        PurchaseReturnSend = 7
    End Enum
    Enum TransfersListMode
        NormalList
        SingleHeader
    End Enum

    '=========================================
    ' تحميل الفورم
    '=========================================
    Private Sub frmStockTransaction_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        rdoSendTransaction.Checked = True
        CurrentMode = StockTransactionMode.TransferSend

        PrepareTransferDetailsGrid()
        PositionStoreCombos()
        LoadStoresByUserPermission()
        LoadProductTypesForGrid_Transfer()
        PrepareTransfersListGrid()
        PrepareNewTransaction()
        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()
        With dgvTransfersList
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvTransferDetails.EditMode = DataGridViewEditMode.EditOnEnter
            .TabStop = False
            dgvTransferDetails.TabStop = (CurrentMode = StockTransactionMode.TransferSend)
        End With
        ' إعداد مربع نص سبب الإلغاء
        With txtCancelReason
            .Multiline = True
            .Height = 60
            .Width = 300
            .Location = New Point(Me.Width \ 2 - 150, Me.Height \ 2 - 30)
            .Visible = False
            .BorderStyle = BorderStyle.FixedSingle
            .Font = New Font("Segoe UI", 10)

            ' نص توجيهي
            .Text = "أدخل سبب إلغاء الاستلام..."
            .ForeColor = Color.Gray

            ' ✅ أولاً: أضف الـ Control إلى الفورم
            Me.Controls.Add(txtCancelReason)

            ' ✅ ثانياً: ارفعه للأمام (هذا سطر منفصل)
            .BringToFront()
        End With

        ' إضافة معالج حدث للدخول والخروج من النص
        AddHandler txtCancelReason.Enter, AddressOf TxtCancelReason_Enter
        AddHandler txtCancelReason.Leave, AddressOf TxtCancelReason_Leave
    End Sub
    Private Sub TxtCancelReason_Enter(sender As Object, e As EventArgs)
        If txtCancelReason.Text = "أدخل سبب إلغاء الاستلام..." Then
            txtCancelReason.Text = ""
            txtCancelReason.ForeColor = Color.Black
        End If
    End Sub

    Private Sub TxtCancelReason_Leave(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtCancelReason.Text) Then
            txtCancelReason.Text = "أدخل سبب إلغاء الاستلام..."
            txtCancelReason.ForeColor = Color.Gray
        End If
    End Sub
    Private Sub rdoSendTransaction_CheckedChanged(sender As Object, e As EventArgs) Handles rdoSendTransaction.CheckedChanged
        If Not rdoSendTransaction.Checked Then Exit Sub
        If _isLoadingTransaction Then Exit Sub

        CurrentMode = StockTransactionMode.TransferSend
        If CurrentTransactionID > 0 Then
            PrepareNewTransaction()
        Else
            SyncUIWithMode()
            LoadTransfersList()
        End If
        btnSend.Enabled = True

        ApplyDetailsGridAccess()
    End Sub
    

    Private Sub rdoReceiveTransaction_CheckedChanged(sender As Object, e As EventArgs) Handles rdoReceiveTransaction.CheckedChanged
        If Not rdoReceiveTransaction.Checked Then Exit Sub


        If _isLoadingTransaction Then Exit Sub
        PrepareNewTransaction()
        CurrentMode = StockTransactionMode.TransferReceive
        If CurrentTransactionID > 0 Then
            PrepareNewTransaction()
        Else
            SyncUIWithMode()
            LoadTransfersList()
        End If

        ApplyDetailsGridAccess()
    End Sub
    Private Sub dgvTransfersList_CellBeginEdit(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvTransfersList.CellBeginEdit

        e.Cancel = True

    End Sub
    Private Sub chkIsProduction_CheckedChanged(
    sender As Object,
    e As EventArgs
)


    End Sub

    Private Sub chkReceivePurchase_CheckedChanged(
    sender As Object,
    e As EventArgs
)



    End Sub
    Private Sub SyncUIWithMode()

        Select Case CurrentMode
            Case StockTransactionMode.ProductionReceive
                btnSend.Text = "Receive PROD"
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = False
                dgvTransferDetails.TabStop = False
            Case StockTransactionMode.TransferSend
                If IsEditMode Then
                    btnSend.Text = "تعديل"
                Else
                    btnSend.Text = "Send"
                End If
                cboSourceStore.Enabled = True
                cboTargetStore.Enabled = True
                dgvTransferDetails.TabStop = True
            Case StockTransactionMode.TransferReceive
                btnSend.Text = "Receive"
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = False
                dgvTransferDetails.TabStop = False
            Case StockTransactionMode.PurchaseReceive
                btnSend.Text = "Receive PR"
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = False
                dgvTransferDetails.TabStop = False
            Case StockTransactionMode.CuttingReceive
                btnSend.Text = "Receive CUT"
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = False
                dgvTransferDetails.TabStop = False
            Case StockTransactionMode.SalesReturnReceive
                btnSend.Text = "Receive SRT"
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = False
                dgvTransferDetails.TabStop = False
            Case StockTransactionMode.PurchaseReturnSend
                btnSend.Text = "Send PRT"
                cboSourceStore.Enabled = True
                cboTargetStore.Enabled = False
                dgvTransferDetails.ReadOnly = True
        End Select

        ApplyDetailsGridAccess()

    End Sub

    '=========================================
    ' تحميل أكواد المنتجات الموجودة في مخزن المصدر
    '=========================================
    Private Function LoadProductCodesForSourceStore(sourceStoreID As Integer) As DataTable

        Dim dt As New DataTable()
        dt.Columns.Add("ProductCode", GetType(String))
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT DISTINCT
    p.ProductCode
FROM Inventory_Balance b
INNER JOIN Master_Product p ON p.ProductID = b.ProductID
WHERE b.StoreID = @StoreID
  AND b.QtyOnHand > 0
ORDER BY p.ProductCode
", con)

                cmd.Parameters.AddWithValue("@StoreID", sourceStoreID)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt

    End Function


    '=========================================
    ' تحميل أنواع المنتجات المرتبطة بالكود والموجودة في المخزن
    '=========================================
    Private Function LoadProductTypesForCode(
    sourceStoreID As Integer,
    productCode As String
) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT DISTINCT
    p.ProductTypeID,
    pt.TypeName
FROM Inventory_Balance b
INNER JOIN Master_Product p ON p.ProductID = b.ProductID
INNER JOIN Master_ProductType pt ON pt.ProductTypeID = p.ProductTypeID
WHERE b.StoreID = @StoreID
  AND b.QtyOnHand > 0
  AND p.ProductCode = @Code
ORDER BY pt.TypeName
", con)

                cmd.Parameters.AddWithValue("@StoreID", sourceStoreID)
                cmd.Parameters.AddWithValue("@Code", productCode)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt

    End Function


    '=========================================
    ' تحديد ProductID بناءً على الكود والنوع والمخزن
    '=========================================
    Private Function ResolveProductIDFromCodeAndType(
    sourceStoreID As Integer,
    productCode As String,
    productTypeID As Integer
) As Integer

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT TOP 1 p.ProductID
FROM Inventory_Balance b
INNER JOIN Master_Product p ON p.ProductID = b.ProductID
WHERE b.StoreID = @StoreID
  AND b.QtyOnHand > 0
  AND p.ProductCode = @Code
  AND p.ProductTypeID = @TypeID
", con)

                cmd.Parameters.AddWithValue("@StoreID", sourceStoreID)
                cmd.Parameters.AddWithValue("@Code", productCode)
                cmd.Parameters.AddWithValue("@TypeID", productTypeID)

                con.Open()
                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then Return 0
                Return CInt(result)
            End Using
        End Using

    End Function

    '=========================================
    ' تهيئة جريد تفاصيل التحويل
    '=========================================
    Private Sub PrepareTransferDetailsGrid()

        dgvTransferDetails.AutoGenerateColumns = False
        dgvTransferDetails.AllowUserToAddRows = True
        dgvTransferDetails.AllowUserToDeleteRows = False
        dgvTransferDetails.ReadOnly = False

        '=========================
        ' Product Code Combo
        '=========================
        Dim colCode =
    CType(dgvTransferDetails.Columns("colProductCode"), DataGridViewComboBoxColumn)

        colCode.DisplayMember = "ProductCode"
        colCode.ValueMember = "ProductCode"
        colCode.DataSource = Nothing
        colCode.ValueMember = "ProductCode"
        colCode.ValueType = GetType(String)
        colCode.DataPropertyName = "ProductCode"

        '=========================
        ' Product Type Combo
        '=========================
        Dim colType = CType(dgvTransferDetails.Columns("colProductType"), DataGridViewComboBoxColumn)
        colType.DisplayMember = "TypeName"
        colType.ValueMember = "ProductTypeID"
        colType.DataSource = Nothing
        colType.DataPropertyName = "ProductTypeID"

        '=========================
        ' ربط الأعمدة مع الـ DataTable (DataPropertyName)
        '=========================
        dgvTransferDetails.Columns("colProductID").DataPropertyName = "ProductID"
        dgvTransferDetails.Columns("colProductCode").DataPropertyName = "ProductCode"
        dgvTransferDetails.Columns("colProductName").DataPropertyName = "ProductName"
        dgvTransferDetails.Columns("colSourceOnHand").DataPropertyName = "SourceOnHand"
        dgvTransferDetails.Columns("colTransferQty").DataPropertyName = "TransferQty"
        dgvTransferDetails.Columns("colTargetAfter").DataPropertyName = "TargetAfter"
        dgvTransferDetails.Columns("colLineNotes").DataPropertyName = "LineNotes"

        '=========================
        ' ProductID (Hidden)
        '=========================
        dgvTransferDetails.Columns("colProductID").ReadOnly = True

        '=========================
        ' Unit (Text Only)
        '=========================
        dgvTransferDetails.Columns("colUnit").DataPropertyName = "UnitName"
        dgvTransferDetails.Columns("colUnit").ReadOnly = True

        With dgvTransferDetails.Columns("colTransferQty")
            .ValueType = GetType(Decimal)
            .DefaultCellStyle.Format = "N2"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        End With

        '=========================
        ' الشكل العام (كما كان)
        '=========================
        With dgvTransferDetails
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .EnableHeadersVisualStyles = False

            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 248)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
            .ColumnHeadersHeight = 36

            .DefaultCellStyle.Font = New Font("Segoe UI", 9.5)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 252)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            .RowTemplate.Height = 30
            .GridColor = Color.FromArgb(230, 230, 230)

            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None
        End With

        '=========================
        ' ✅ أزرار الجريد - التهيئة الصحيحة
        '=========================
        Dim btnSearch As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colProductSearch"), DataGridViewButtonColumn)
        btnSearch.Text = "🔍 بحث"  ' ✅ ضبط النص
        btnSearch.UseColumnTextForButtonValue = True
        btnSearch.FlatStyle = FlatStyle.Standard
        btnSearch.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        btnSearch.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)  ' لون خلفية
        btnSearch.DefaultCellStyle.ForeColor = Color.Black  ' لون النص

        Dim btnDelete As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colDelete"), DataGridViewButtonColumn)
        btnDelete.Text = "🗑️ حذف"  ' ✅ ضبط النص
        btnDelete.UseColumnTextForButtonValue = True
        btnDelete.FlatStyle = FlatStyle.Standard
        btnDelete.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        btnDelete.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200)  ' لون خلفية أحمر فاتح
        btnDelete.DefaultCellStyle.ForeColor = Color.Black  ' لون النص

        '=========================
        ' المقاسات
        '=========================
        With dgvTransferDetails.Columns
            .Item("colProductSearch").Width = 80
            .Item("colProductID").Width = 60
            .Item("colProductCode").Width = 160
            .Item("colProductType").Width = 90
            .Item("colProductName").Width = 160
            .Item("colSourceOnHand").Width = 80
            .Item("colTransferQty").Width = 80
            .Item("colUnit").Width = 70
            .Item("colTargetAfter").Width = 80
            .Item("colLineNotes").Width = 150
            .Item("colDelete").Width = 80
        End With

        ' =========================
        ' تلوين أعمدة الكميات المتاحة فقط
        ' =========================
        Dim beigeColor As Color = Color.FromArgb(255, 248, 220)

        With dgvTransferDetails.Columns("colSourceOnHand")
            .DefaultCellStyle.BackColor = beigeColor
            .DefaultCellStyle.SelectionBackColor = beigeColor
        End With

        With dgvTransferDetails.Columns("colTargetAfter")
            .DefaultCellStyle.BackColor = beigeColor
            .DefaultCellStyle.SelectionBackColor = beigeColor
        End With

        ' =========================
        ' تلوين هيدر أعمدة الكميات المتاحة فقط
        ' =========================
        With dgvTransferDetails.Columns("colSourceOnHand")
            .HeaderCell.Style.BackColor = beigeColor
            .HeaderCell.Style.SelectionBackColor = beigeColor
        End With

        With dgvTransferDetails.Columns("colTargetAfter")
            .HeaderCell.Style.BackColor = beigeColor
            .HeaderCell.Style.SelectionBackColor = beigeColor
        End With

        ' ✅ قوة إضافية: فرض رسم الأزرار
        dgvTransferDetails.Invalidate()

    End Sub
    Private Sub PositionStoreCombos()

        If dgvTransferDetails.Columns.Count = 0 Then Exit Sub

        Dim beigeColor As Color = Color.FromArgb(255, 248, 220)
        Dim darkRed As Color = Color.FromArgb(120, 20, 20)

        ' =========================
        ' المصدر
        ' =========================
        Dim srcCol = dgvTransferDetails.Columns("colSourceOnHand")
        Dim srcRect As Rectangle =
        dgvTransferDetails.GetCellDisplayRectangle(srcCol.Index, -1, True)

        ' Label المصدر
        With lblSourceStore
            .AutoSize = False
            .TextAlign = ContentAlignment.MiddleCenter
            .Width = srcRect.Width
            .Height = 24
            .Location = New Point(
            dgvTransferDetails.Left + srcRect.Left,
            dgvTransferDetails.Top - .Height - cboSourceStore.Height - 6
        )
            .BackColor = beigeColor
            .ForeColor = darkRed
            .Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
            .BorderStyle = BorderStyle.None
        End With

        ' Combo المصدر
        cboSourceStore.SetBounds(
        dgvTransferDetails.Left + srcRect.Left,
        lblSourceStore.Bottom + 2,
        srcRect.Width,
        cboSourceStore.Height
    )
        cboSourceStore.BackColor = beigeColor
        cboSourceStore.FlatStyle = FlatStyle.Flat

        ' =========================
        ' الهدف
        ' =========================
        Dim trgCol = dgvTransferDetails.Columns("colTargetAfter")
        Dim trgRect As Rectangle =
        dgvTransferDetails.GetCellDisplayRectangle(trgCol.Index, -1, True)

        ' Label الهدف
        With lblTargetStore
            .AutoSize = False
            .TextAlign = ContentAlignment.MiddleCenter
            .Width = trgRect.Width
            .Height = 24
            .Location = New Point(
            dgvTransferDetails.Left + trgRect.Left,
            dgvTransferDetails.Top - .Height - cboTargetStore.Height - 6
        )
            .BackColor = beigeColor
            .ForeColor = darkRed
            .Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
            .BorderStyle = BorderStyle.None
        End With

        ' Combo الهدف
        cboTargetStore.SetBounds(
        dgvTransferDetails.Left + trgRect.Left,
        lblTargetStore.Bottom + 2,
        trgRect.Width,
        cboTargetStore.Height
    )
        cboTargetStore.BackColor = beigeColor
        cboTargetStore.FlatStyle = FlatStyle.Flat

        ' تأكيد الظهور
        lblSourceStore.BringToFront()
        cboSourceStore.BringToFront()
        lblTargetStore.BringToFront()
        cboTargetStore.BringToFront()

    End Sub
    Private Sub frmStockTransaction_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        PositionStoreCombos()
    End Sub
    Private Sub dgvTransferDetails_Scroll(sender As Object, e As ScrollEventArgs) _
    Handles dgvTransferDetails.Scroll

        If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
            PositionStoreCombos()
        End If
    End Sub


    Private Sub dgvTransferDetails_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvTransferDetails.CellValueChanged

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = dgvTransferDetails.Rows(e.RowIndex)
        Dim colName As String = dgvTransferDetails.Columns(e.ColumnIndex).Name
        Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)

        '=========================
        ' تغيير كود المنتج
        '=========================
        If colName = "colProductCode" Then

            ClearRowUI(row)
            row.Cells("colProductType").Value = Nothing
            row.Cells("colProductID").Value = Nothing

            If drv IsNot Nothing Then
                drv("ProductTypeID") = DBNull.Value
                drv("ProductID") = DBNull.Value
            End If

            If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
            If row.Cells("colProductCode").Value Is Nothing Then Exit Sub

            Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
            Dim productCode As String = row.Cells("colProductCode").Value.ToString()

            Dim dtTypes As DataTable =
            LoadProductTypesForCode(sourceStoreID, productCode)

            ' اختيار تلقائي إذا نوع واحد
            If dtTypes.Rows.Count = 1 Then
                row.Cells("colProductType").Value =
                CInt(dtTypes.Rows(0)("ProductTypeID"))
            End If

            Exit Sub
        End If

        '=========================
        ' تغيير النوع
        '=========================
        If colName = "colProductType" Then

            If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
            If row.Cells("colProductCode").Value Is Nothing Then Exit Sub
            If row.Cells("colProductType").Value Is Nothing Then Exit Sub

            Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
            Dim productCode As String = row.Cells("colProductCode").Value.ToString()
            Dim productTypeID As Integer = CInt(row.Cells("colProductType").Value)

            Dim productID As Integer =
            ResolveProductIDFromCodeAndType(sourceStoreID, productCode, productTypeID)

            If productID <= 0 Then Exit Sub

            If IsDuplicateProductID(e.RowIndex, productID) Then
                MessageBox.Show("هذا الصنف مضاف مسبقًا", "تنبيه")
                ClearRowUI(row)
                row.Cells("colProductType").Value = Nothing
                row.Cells("colProductID").Value = Nothing
                If drv IsNot Nothing Then
                    drv("ProductID") = DBNull.Value
                    drv("ProductTypeID") = DBNull.Value
                End If
                Exit Sub
            End If

            row.Cells("colProductID").Value = productID
            If drv IsNot Nothing Then drv("ProductID") = productID

            FillProductRowByProductID(row, productID)
            row.Cells("colTransferQty").Tag = 0D

            Exit Sub
        End If

    End Sub
    Private Sub dgvTransferDetails_CellBeginEdit(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvTransferDetails.CellBeginEdit

        If e.RowIndex < 0 Then Exit Sub

        ' ✅ منع التحرير في وضع الاستلام
        If CurrentMode <> StockTransactionMode.TransferSend Then
            e.Cancel = True
            Exit Sub
        End If

        Dim row = dgvTransferDetails.Rows(e.RowIndex)
        Dim colName As String = dgvTransferDetails.Columns(e.ColumnIndex).Name

        Select Case colName

            Case "colProductCode"

                If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
                If Not TypeOf cboSourceStore.SelectedValue Is Integer Then Exit Sub

                Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)

                Dim colCode =
            CType(dgvTransferDetails.Columns("colProductCode"),
                  DataGridViewComboBoxColumn)

                colCode.DataSource = LoadProductCodesForSourceStore(sourceStoreID)
                colCode.DisplayMember = "ProductCode"
                colCode.ValueMember = "ProductCode"

            Case "colProductType"

                If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
                If row.Cells("colProductCode").Value Is Nothing Then Exit Sub

                Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
                Dim productCode As String = row.Cells("colProductCode").Value.ToString()

                Dim dtTypes As DataTable =
            LoadProductTypesForCode(sourceStoreID, productCode)

                Dim cellCombo As DataGridViewComboBoxCell =
            CType(row.Cells("colProductType"), DataGridViewComboBoxCell)

                cellCombo.DataSource = dtTypes
                cellCombo.DisplayMember = "TypeName"
                cellCombo.ValueMember = "ProductTypeID"

            Case "colProductSearch", "colTransferQty", "colDelete"
                ' مسموح

            Case Else
                e.Cancel = True
        End Select

    End Sub
    Private Sub LoadProductTypesForGrid_Transfer()

        Dim colType =
    CType(dgvTransferDetails.Columns("colProductType"),
          DataGridViewComboBoxColumn)

        colType.DataSource = LoadAllProductTypes_Transfer()
        colType.DisplayMember = "TypeName"
        colType.ValueMember = "ProductTypeID"

    End Sub
    Private Function LoadAllProductTypes_Transfer() As DataTable

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

        Return dt

    End Function
    Private Sub ValidateSourceTargetStores()

        ' ✅ تجاهل القص
        If CurrentMode = StockTransactionMode.CuttingReceive Then Exit Sub

        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
        If cboTargetStore.SelectedValue Is Nothing Then Exit Sub

        Dim src As Integer = CInt(cboSourceStore.SelectedValue)
        Dim trg As Integer = CInt(cboTargetStore.SelectedValue)

        If src = trg Then

            MessageBox.Show(
            "لا يمكن أن يكون مستودع المصدر والهدف متطابقين",
            "تنبيه",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )

            If cboSourceStore.Focused Then
                cboSourceStore.SelectedIndex = -1
            ElseIf cboTargetStore.Focused Then
                cboTargetStore.SelectedIndex = -1
            End If

        End If

    End Sub
    '----------------------------------------------------------------------------------------
    Private Sub dgvTransferDetails_RowValidating(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvTransferDetails.RowValidating

        If _allowLeaveGrid Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        Dim row = dgvTransferDetails.Rows(e.RowIndex)
        If row.IsNewRow Then Exit Sub

        ' ✅ التحقق: إذا كان الصف فارغاً تماماً (لا يوجد كود ولا نوع) نسمح بالخروج
        If (row.Cells("colProductCode").Value Is Nothing OrElse
        String.IsNullOrEmpty(row.Cells("colProductCode").Value.ToString())) AndAlso
       (row.Cells("colProductType").Value Is Nothing OrElse
        IsDBNull(row.Cells("colProductType").Value)) AndAlso
       (row.Cells("colProductID").Value Is Nothing OrElse
        IsDBNull(row.Cells("colProductID").Value)) Then

            ' صف فارغ - نسمح بالخروج
            Exit Sub
        End If

        ' إذا كان فيه كود ولكن بدون نوع
        If row.Cells("colProductCode").Value IsNot Nothing AndAlso
       Not String.IsNullOrEmpty(row.Cells("colProductCode").Value.ToString()) AndAlso
       (row.Cells("colProductType").Value Is Nothing OrElse
        IsDBNull(row.Cells("colProductType").Value)) Then

            e.Cancel = True
            MessageBox.Show("يجب اختيار نوع المنتج", "تنبيه")
            Exit Sub
        End If

        ' إذا كان فيه نوع ولكن بدون كود
        If (row.Cells("colProductCode").Value Is Nothing OrElse
        String.IsNullOrEmpty(row.Cells("colProductCode").Value.ToString())) AndAlso
       row.Cells("colProductType").Value IsNot Nothing AndAlso
       Not IsDBNull(row.Cells("colProductType").Value) Then

            e.Cancel = True
            MessageBox.Show("يجب اختيار كود المنتج أولاً", "تنبيه")
            Exit Sub
        End If

        ' التحقق من الكمية فقط إذا كان الصف مكتمل
        If row.Cells("colProductCode").Value IsNot Nothing AndAlso
       Not String.IsNullOrEmpty(row.Cells("colProductCode").Value.ToString()) AndAlso
       row.Cells("colProductType").Value IsNot Nothing AndAlso
       Not IsDBNull(row.Cells("colProductType").Value) AndAlso
       row.Cells("colProductID").Value IsNot Nothing AndAlso
       Not IsDBNull(row.Cells("colProductID").Value) Then

            Dim qty As Decimal = 0D
            If row.Cells("colTransferQty").Value IsNot Nothing Then
                Decimal.TryParse(row.Cells("colTransferQty").Value.ToString(), qty)
            End If

            If qty <= 0D Then
                MessageBox.Show("يجب إدخال كمية أكبر من صفر", "تنبيه")
                e.Cancel = True
                Exit Sub
            End If
        End If

    End Sub
    Private Sub dgvTransferDetails_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs
) Handles dgvTransferDetails.DataError

        e.ThrowException = False
        e.Cancel = True

    End Sub

    '=========================================
    ' تحميل المستودعات
    '=========================================
    '=========================================
    ' تحميل المستودعات (للهيدر)
    '=========================================
    Protected Sub LoadStoresByUserPermission()

        _isLoadingStores = True
        Try
            Dim dtSource As New DataTable()
            Dim dtTarget As New DataTable()

            Using con As New SqlConnection(ConnStr)

                ' Source
                Using cmd As New SqlCommand("
                SELECT
                    s.StoreID,
                    s.StoreName,
                    us.IsDefaultSend
                FROM Master_Store s
                INNER JOIN Security_UserStore us ON us.StoreID = s.StoreID
                WHERE s.IsActive = 1
                  AND us.IsActive = 1
                  AND us.EmployeeID = @UserID
                  AND us.CanSend = 1
                ORDER BY us.IsDefaultSend DESC, s.StoreName
            ", con)

                    cmd.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dtSource)
                    End Using
                End Using

                ' Target
                Using cmd As New SqlCommand("
                SELECT
                    s.StoreID,
                    s.StoreName,
                    us.IsDefaultReceive
                FROM Master_Store s
                INNER JOIN Security_UserStore us ON us.StoreID = s.StoreID
                WHERE s.IsActive = 1
                  AND us.IsActive = 1
                  AND us.EmployeeID = @UserID
                  AND us.CanReceive = 1
                ORDER BY us.IsDefaultReceive DESC, s.StoreName
            ", con)

                    cmd.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dtTarget)
                    End Using
                End Using

            End Using

            ' Bind Source
            cboSourceStore.DataSource = dtSource
            cboSourceStore.DisplayMember = "StoreName"
            cboSourceStore.ValueMember = "StoreID"
            cboSourceStore.SelectedIndex = -1

            For Each r As DataRow In dtSource.Rows
                If Convert.ToInt32(r("IsDefaultSend")) = 1 Then
                    cboSourceStore.SelectedValue = CInt(r("StoreID"))
                    Exit For
                End If
            Next

            ' Bind Target
            cboTargetStore.DataSource = dtTarget
            cboTargetStore.DisplayMember = "StoreName"
            cboTargetStore.ValueMember = "StoreID"
            cboTargetStore.SelectedIndex = -1

            For Each r As DataRow In dtTarget.Rows
                If Convert.ToInt32(r("IsDefaultReceive")) = 1 Then
                    cboTargetStore.SelectedValue = CInt(r("StoreID"))
                    Exit For
                End If
            Next

        Finally
            _isLoadingStores = False
        End Try

    End Sub


    '=========================================
    ' تحميل قائمة التحويلات
    '=========================================

    '=========================================
    Private Sub LoadTransfersList()

        ' =========================
        ' تحديد نوع العملية حسب الوضع
        ' =========================
        Dim operationTypeID As Integer = 0
        Dim filterByTargetStore As Boolean = False
        Dim filterBySourceStore As Boolean = False

        Select Case CurrentMode

            Case StockTransactionMode.TransferSend
                operationTypeID = 10   ' عدل حسب رقم TRN عندك
                filterBySourceStore = True

            Case StockTransactionMode.TransferReceive
                operationTypeID = 10   ' TRN
                filterByTargetStore = True

            Case StockTransactionMode.PurchaseReceive
                operationTypeID = 7   ' PUR
                filterByTargetStore = True

            Case StockTransactionMode.ProductionReceive
                operationTypeID = 6   ' PRO
                filterByTargetStore = True

            Case StockTransactionMode.CuttingReceive
                operationTypeID = 11  ' CUT
                filterByTargetStore = True

            Case StockTransactionMode.SalesReturnReceive
                operationTypeID = 12   ' SRT
                filterByTargetStore = True
            Case StockTransactionMode.PurchaseReturnSend
                operationTypeID = 14
                filterBySourceStore = True

            Case Else
                dgvTransfersList.DataSource = Nothing
                Exit Sub

        End Select


        ' =========================
        ' الاستعلام الجديد المتوافق مع الهيكل الحالي
        ' =========================
        Dim sql As String =
"
SELECT
    h.TransactionID AS RefID,
    h.TransactionID AS TransactionID,
    h.TransactionDate,
    s.StatusName AS TransactionStatus,

    src.StoreName AS SourceStoreName,
    tgt.StoreName AS TargetStoreName

FROM Inventory_TransactionHeader h

INNER JOIN Workflow_Status s
    ON s.StatusID = h.StatusID

OUTER APPLY
(
    SELECT TOP 1 st.StoreName
    FROM Inventory_TransactionDetails d
    INNER JOIN Master_Store st
        ON st.StoreID = d.SourceStoreID
    WHERE d.TransactionID = h.TransactionID
) src

OUTER APPLY
(
    SELECT TOP 1 st.StoreName
    FROM Inventory_TransactionDetails d
    INNER JOIN Master_Store st
        ON st.StoreID = d.TargetStoreID
    WHERE d.TransactionID = h.TransactionID
) tgt

WHERE
    h.StatusID = 5
    AND h.IsInventoryPosted = 0
    AND h.OperationTypeID = @OperationTypeID
"


        ' =========================
        ' فلترة المستودعات
        ' =========================
        If filterBySourceStore AndAlso IsNumeric(cboSourceStore.SelectedValue) Then

            sql &= "
AND EXISTS
(
    SELECT 1
    FROM Inventory_TransactionDetails d
    WHERE d.TransactionID = h.TransactionID
      AND d.SourceStoreID = @SourceStoreID
)"

        End If


        If filterByTargetStore AndAlso IsNumeric(cboTargetStore.SelectedValue) Then

            sql &= "
AND EXISTS
(
    SELECT 1
    FROM Inventory_TransactionDetails d
    WHERE d.TransactionID = h.TransactionID
      AND d.TargetStoreID = @TargetStoreID
)"

        End If


        sql &= "
ORDER BY h.TransactionDate DESC
"


        ' =========================
        ' التنفيذ
        ' =========================
        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(sql, con)

                cmd.Parameters.Clear()

                cmd.Parameters.Add("@OperationTypeID", SqlDbType.Int).Value = operationTypeID

                If filterBySourceStore AndAlso IsNumeric(cboSourceStore.SelectedValue) Then
                    cmd.Parameters.Add("@SourceStoreID", SqlDbType.Int).Value = CInt(cboSourceStore.SelectedValue)
                End If

                If filterByTargetStore AndAlso IsNumeric(cboTargetStore.SelectedValue) Then
                    cmd.Parameters.Add("@TargetStoreID", SqlDbType.Int).Value = CInt(cboTargetStore.SelectedValue)
                End If

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using
        End Using


        dgvTransfersList.DataSource = dt

        If dgvTransfersList.Columns.Contains("RefID") Then
            dgvTransfersList.Columns("RefID").Visible = False
        End If

        ' تغيير أسماء الأعمدة إلى العربية
        If dgvTransfersList.Columns.Contains("TransactionID") Then
            dgvTransfersList.Columns("TransactionID").HeaderText = "رقم السند"
        End If

        If dgvTransfersList.Columns.Contains("TransactionDate") Then
            dgvTransfersList.Columns("TransactionDate").HeaderText = "التاريخ"
        End If

        If dgvTransfersList.Columns.Contains("TransactionStatus") Then
            dgvTransfersList.Columns("TransactionStatus").HeaderText = "الحالة"
        End If

        If dgvTransfersList.Columns.Contains("SourceStoreName") Then
            dgvTransfersList.Columns("SourceStoreName").HeaderText = "مستودع المصدر"
        End If

        If dgvTransfersList.Columns.Contains("TargetStoreName") Then
            dgvTransfersList.Columns("TargetStoreName").HeaderText = "مستودع الهدف"
        End If
    End Sub
    Private Sub FilterTransfersByWorkflow(dt As DataTable)

    End Sub


    '=========================================
    ' تهيئة تحويل جديد
    '=========================================
    Protected Sub PrepareNewTransaction()

        _isLoadingTransaction = True

        Try
            ' 🔒 تفريغ الجريد بأمان
            SafeResetGrid(dgvTransferDetails)

            ' =========================
            CurrentTransactionID = 0
            EditingTransactionID = 0
            IsEditMode = False
            txtTransactionCode.Clear()
            dtpTransactionDate.Value = Date.Today
            btnSend.Enabled = True

            LoadStoresByUserPermission()

            ' 🔹 ربط جدول جديد
            dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
            Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
            If dt.Rows.Count = 0 Then
                dt.Rows.Add(dt.NewRow())
            End If
            dgvTransferDetails.ReadOnly = False

        Finally
            _isLoadingTransaction = False
        End Try

        LoadTransfersList()
        RefreshWorkflowUI()

    End Sub


    '=========================================
    ' DataTable فارغ لجريد التفاصيل
    '=========================================
    '=========================================
    ' DataTable فارغ لجريد التفاصيل
    '=========================================
    Protected Function BuildEmptyDetailsTable() As DataTable

        Dim dt As New DataTable

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("ProductCode", GetType(String))
        dt.Columns.Add("ProductTypeID", GetType(Integer))

        dt.Columns.Add("ProductName", GetType(String))
        dt.Columns.Add("SourceOnHand", GetType(Decimal))
        dt.Columns.Add("TransferQty", GetType(Decimal))
        dt.Columns.Add("UnitID", GetType(Integer))
        dt.Columns.Add("UnitName", GetType(String))
        dt.Columns.Add("TargetAfter", GetType(Decimal))
        dt.Columns.Add("LineNotes", GetType(String))

        ' ✅ مهم جداً للمشتريات:
        ' LoadTransferForEdit يعتمد على هذا العمود لضبط cboTargetStore عند فتح السند
        dt.Columns.Add("TargetStoreID", GetType(Integer))
        dt.Columns.Add("SourceStoreID", GetType(Integer))

        Return dt

    End Function
    '=========================================
    ' جلب رصيد الصنف في مخزن محدد
    '=========================================
    Protected Function GetProductOnHand(productID As Integer, storeID As Integer) As Decimal

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT QtyOnHand 
             FROM Inventory_Balance 
             WHERE ProductID = @P AND StoreID = @S", con)

                cmd.Parameters.AddWithValue("@P", productID)
                cmd.Parameters.AddWithValue("@S", storeID)

                con.Open()
                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse IsDBNull(result) Then
                    Return 0D
                Else
                    Return Convert.ToDecimal(result)
                End If

            End Using
        End Using

    End Function

    '=========================================
    '=========================================

    '=========================================
    ' تحميل الأصناف حسب مخزن المصدر
    '=========================================

    '=========================================
    ' عند تغيير مخزن المصدر
    '=========================================
    Private Sub cboSourceStore_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboSourceStore.SelectedValueChanged

        If _isLoadingTransaction Then Exit Sub
        If _isLoadingStores Then Exit Sub
        If CurrentMode = StockTransactionMode.PurchaseReceive Then Exit Sub
        If cboSourceStore.SelectedValue Is Nothing Then Exit Sub
        If Not TypeOf cboSourceStore.SelectedValue Is Integer Then Exit Sub

        Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)

        Dim colCode =
        CType(dgvTransferDetails.Columns("colProductCode"),
              DataGridViewComboBoxColumn)
        colCode.ValueType = GetType(String)

        Dim dtCodes As DataTable = LoadProductCodesForSourceStore(sourceStoreID)
        colCode.DataSource = dtCodes
        colCode.DisplayMember = "ProductCode"
        colCode.ValueMember = "ProductCode"
        If dgvTransferDetails.IsCurrentCellInEditMode Then
            dgvTransferDetails.EndEdit()
        End If
        _allowLeaveGrid = True

        If dgvTransferDetails.IsCurrentCellInEditMode Then
            dgvTransferDetails.EndEdit()
        End If

        dgvTransferDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
        dgvTransferDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

        dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
        _allowLeaveGrid = False

        If CurrentMode = StockTransactionMode.TransferSend Then
            LoadTransfersList()
        End If
        Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
        If dt.Rows.Count = 0 Then
            dt.Rows.Add(dt.NewRow())
        End If

        ValidateSourceTargetStores()

    End Sub

    Private Sub FillProductRowByProductID(
    row As DataGridViewRow,
    productID As Integer
)

        ' 🔴 (A) ضع الرسالة هنا بالضبط – أول سطر داخل الدالة

        If productID <= 0 Then Exit Sub

        Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
        Dim targetStoreID As Integer = CInt(cboTargetStore.SelectedValue)

        Dim productName As String = ""
        Dim unitID As Integer = 0
        Dim unitName As String = ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    p.ProductName,
    p.StorageUnitID,
    u.UnitName
FROM Master_Product p
INNER JOIN Master_Unit u ON u.UnitID = p.StorageUnitID
WHERE p.ProductID = @PID
", con)

                cmd.Parameters.AddWithValue("@PID", productID)
                con.Open()

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        productName = rd("ProductName").ToString()
                        unitID = CInt(rd("StorageUnitID"))
                        unitName = rd("UnitName").ToString()
                    End If
                End Using

            End Using
        End Using

        ' 🔴 (B) ضع الرسالة هنا بالضبط – بعد انتهاء جلب البيانات من DB

        Dim sourceQty As Decimal = 0D
        Dim targetQty As Decimal = 0D

        If cboSourceStore.SelectedValue IsNot Nothing AndAlso
   TypeOf cboSourceStore.SelectedValue Is Integer Then
            sourceQty = GetProductOnHand(productID, CInt(cboSourceStore.SelectedValue))
        End If

        If cboTargetStore.SelectedValue IsNot Nothing AndAlso
   TypeOf cboTargetStore.SelectedValue Is Integer Then
            targetQty = GetProductOnHand(productID, CInt(cboTargetStore.SelectedValue))
        End If


        ' 🔴 (C) ضع الرسالة هنا بالضبط – قبل أول سطر يكتب في الجريد

        row.Cells("colProductName").Value = productName
        row.Cells("colUnit").Value = unitName   ' نص فقط

        row.Cells("colSourceOnHand").Value = sourceQty
        row.Cells("colTransferQty").Value = 0D
        row.Cells("colTransferQty").Tag = 0D
        row.Cells("colTargetAfter").Value = targetQty

        ' 🔴 (D) ضع الرسالة هنا بالضبط – آخر سطر قبل End Sub

    End Sub


    '=========================================
    ' التحقق من وجود الصنف مسبقًا في الجريد
    '=========================================
    Private Function IsProductAlreadyAdded(productID As Integer) As Boolean

        If productID <= 0 Then Return False

        Dim dt = TryCast(dgvTransferDetails.DataSource, DataTable)
        If dt Is Nothing Then Return False

        For Each r As DataRow In dt.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            If IsDBNull(r("ProductID")) Then Continue For
            If CInt(r("ProductID")) = productID Then
                Return True
            End If
        Next

        Return False

    End Function

    '=========================================
    ' تحديث الرصيد المعروض للمصدر بعد إدخال الكمية
    '=========================================
    '=========================================
    ' تحديث الأرصدة المعروضة (عرض فقط)
    '=========================================
    Private Sub dgvTransferDetails_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvTransferDetails.CellEndEdit

        If e.RowIndex < 0 Then Exit Sub
        If dgvTransferDetails.Columns(e.ColumnIndex).Name <> "colTransferQty" Then Exit Sub

        Dim row As DataGridViewRow = dgvTransferDetails.Rows(e.RowIndex)

        ' ⛔ تأمين القيم قبل أي حساب
        If row.Cells("colSourceOnHand").Value Is Nothing _
       OrElse IsDBNull(row.Cells("colSourceOnHand").Value) Then Exit Sub

        If row.Cells("colTargetAfter").Value Is Nothing _
       OrElse IsDBNull(row.Cells("colTargetAfter").Value) Then Exit Sub

        Dim lastQty As Decimal = 0D
        If row.Cells("colTransferQty").Tag IsNot Nothing _
       AndAlso Not IsDBNull(row.Cells("colTransferQty").Tag) Then
            lastQty = CDec(row.Cells("colTransferQty").Tag)
        End If

        Dim originalSource As Decimal =
        CDec(row.Cells("colSourceOnHand").Value) + lastQty

        Dim originalTarget As Decimal =
        CDec(row.Cells("colTargetAfter").Value) - lastQty

        Dim transferQty As Decimal = 0D
        If row.Cells("colTransferQty").Value IsNot Nothing _
       AndAlso Not IsDBNull(row.Cells("colTransferQty").Value) Then
            Decimal.TryParse(
            row.Cells("colTransferQty").Value.ToString(),
            transferQty
        )
        End If

        ' منع السالب
        If transferQty < 0D Then transferQty = 0D

        ' منع أكبر من الرصيد
        If transferQty > originalSource Then
            MessageBox.Show(
            "الكمية أكبر من رصيد المستودع المصدر",
            "تنبيه",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            transferQty = 0D
        End If

        ' تحديث العرض
        row.Cells("colSourceOnHand").Value = originalSource - transferQty
        row.Cells("colTargetAfter").Value = originalTarget + transferQty

        ' حفظ آخر كمية صحيحة
        row.Cells("colTransferQty").Value = transferQty
        row.Cells("colTransferQty").Tag = transferQty

    End Sub
    '=========================================



    '=========================================
    ' حساب الكمية المحجوزة (Sent ولم تُستلم)
    '=========================================
    Protected Function GetReservedQty(productID As Integer, storeID As Integer) As Decimal

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT ISNULL(SUM(d.Quantity), 0)
FROM Inventory_TransactionDetails d
INNER JOIN Inventory_TransactionHeader h
    ON h.TransactionID = d.TransactionID
WHERE d.ProductID = @PID
  AND d.SourceStoreID = @StoreID
  AND h.IsInventoryPosted = 0
", con)

                cmd.Parameters.AddWithValue("@PID", productID)
                cmd.Parameters.AddWithValue("@STOREID", storeID)

                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then
                    Return 0D
                End If

                Return CDec(result)

            End Using
        End Using

    End Function
    '=========================================
    ' الكمية المتاحة = OnHand - Reserved
    '=========================================
    Protected Function GetAvailableQty(productID As Integer, storeID As Integer) As Decimal

        Dim onHand As Decimal =
        GetProductOnHand(productID, storeID)

        Dim reserved As Decimal =
        GetReservedQty(productID, storeID)

        Return onHand

    End Function
    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        _allowLeaveGrid = True

        If IsEditMode Then
            UpdateCurrentTransaction()
        Else
            Select Case CurrentMode
                Case StockTransactionMode.TransferSend

                    CreateTransferTransaction2()

                Case Else
                    ReceiveInventoryTransaction()
            End Select
        End If

    End Sub
    Private Sub UpdateCurrentTransaction()
        If CurrentTransactionID <= 0 Then Exit Sub
        If dgvTransferDetails.Rows.Count = 0 Then Exit Sub

        ' تحويل الجريد لـ DataTable
        Dim details As New DataTable()
        details.Columns.Add("ProductID", GetType(Integer))
        details.Columns.Add("Qty", GetType(Decimal))
        details.Columns.Add("UnitID", GetType(Integer))
        details.Columns.Add("UnitPrice", GetType(Decimal))
        details.Columns.Add("SourceStoreID", GetType(Integer))
        details.Columns.Add("TargetStoreID", GetType(Integer))

        Dim sourceStoreID As Integer = 0
        Dim targetStoreID As Integer = 0

        If cboSourceStore.SelectedValue IsNot Nothing AndAlso
           TypeOf cboSourceStore.SelectedValue Is Integer Then
            sourceStoreID = CInt(cboSourceStore.SelectedValue)
        End If

        If cboTargetStore.SelectedValue IsNot Nothing AndAlso
           TypeOf cboTargetStore.SelectedValue Is Integer Then
            targetStoreID = CInt(cboTargetStore.SelectedValue)
        End If

        For Each r As DataGridViewRow In dgvTransferDetails.Rows
            If r.IsNewRow Then Continue For

            Dim qty As Decimal = Convert.ToDecimal(r.Cells("colTransferQty").Value)
            If qty <= 0 Then Continue For

            Dim productID As Integer = CInt(r.Cells("colProductID").Value)

            ' جلب UnitID
            Dim unitID As Integer = 0
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT StorageUnitID 
                FROM Master_Product 
                WHERE ProductID = @ProductID", con)

                    cmd.Parameters.AddWithValue("@ProductID", productID)
                    con.Open()
                    unitID = CInt(cmd.ExecuteScalar())
                End Using
            End Using

            details.Rows.Add(
                productID,
                qty,
                unitID,
                0D,
                sourceStoreID,
                targetStoreID
            )
        Next

        ' استدعاء UpdateTransaction في الـ Engine
        Dim engine As New TransactionEngine(ConnStr)
        engine.UpdateTransaction(CurrentTransactionID, CurrentUserID, details)

        MessageBox.Show("تم تعديل السند بنجاح", "تم",
                       MessageBoxButtons.OK, MessageBoxIcon.Information)

        ' إلغاء وضع التعديل
        IsEditMode = False
        CurrentTransactionID = 0
        PrepareNewTransaction()
        SyncUIWithMode()
    End Sub

    Private Sub CreateTransferTransaction2()
        If CurrentTransactionID <> 0 Then Exit Sub
        If dgvTransferDetails.Rows.Count = 0 Then Exit Sub

        ' 🔴 تحقق إلزامي في وضع الإرسال فقط
        If CurrentMode = StockTransactionMode.TransferSend Then

            If cboSourceStore.SelectedValue Is Nothing _
        OrElse Not TypeOf cboSourceStore.SelectedValue Is Integer Then

                MessageBox.Show("يجب اختيار مستودع المصدر", "تنبيه")
                Exit Sub
            End If

            If cboTargetStore.SelectedValue Is Nothing _
        OrElse Not TypeOf cboTargetStore.SelectedValue Is Integer Then

                MessageBox.Show("يجب اختيار مستودع الهدف", "تنبيه")
                Exit Sub
            End If

            If CInt(cboSourceStore.SelectedValue) = CInt(cboTargetStore.SelectedValue) Then
                MessageBox.Show("لا يمكن أن يكون المصدر والهدف متطابقين", "تنبيه")
                Exit Sub
            End If
        End If

        ' تحويل الجريد لـ List(Of TransactionDetailDTO)
        Dim detailsList As New List(Of TransactionDetailDTO)()

        Dim sourceStoreID As Integer = CInt(cboSourceStore.SelectedValue)
        Dim targetStoreID As Integer = CInt(cboTargetStore.SelectedValue)

        For Each r As DataGridViewRow In dgvTransferDetails.Rows
            If r.IsNewRow Then Continue For

            Dim qty As Decimal = Convert.ToDecimal(r.Cells("colTransferQty").Value)
            If qty <= 0 Then Continue For

            Dim productID As Integer = CInt(r.Cells("colProductID").Value)
            Dim unitID As Integer = 0

            ' جلب UnitID من قاعدة البيانات
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT StorageUnitID 
                FROM Master_Product 
                WHERE ProductID = @ProductID", con)

                    cmd.Parameters.AddWithValue("@ProductID", productID)
                    con.Open()
                    unitID = CInt(cmd.ExecuteScalar())
                End Using
            End Using

            ' إنشاء DTO وإضافته للقائمة
            Dim dto As New TransactionDetailDTO()
            dto.ProductID = productID
            dto.Quantity = qty
            dto.UnitID = unitID
            dto.UnitCost = 0D  ' UnitPrice -> UnitCost
            dto.SourceStoreID = sourceStoreID
            dto.TargetStoreID = targetStoreID

            detailsList.Add(dto)
        Next

        ' ✅ استدعاء CreateTransfer في TransactionEngine
        Try
            Dim engine As New TransactionEngine(ConnStr)
            Dim newID As Integer = engine.CreateTransfer(
            dtpTransactionDate.Value,
            CurrentUserID,
            detailsList
        )

            CurrentTransactionID = newID

            MessageBox.Show(
            "تم حفظ التحويل بنجاح" & vbCrLf & "رقم السند: " & CurrentTransactionID,
            "تم الحفظ",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )

            ' تحديث الفورم بعد الحفظ
            RefreshFormStatus(CurrentTransactionID)
            LoadTransfersList()
            ApplyDetailsGridAccess()
            RefreshWorkflowUI()

        Catch ex As Exception
            MessageBox.Show("خطأ في حفظ التحويل: " & ex.Message, "خطأ",
                       MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub ReceiveInventoryTransaction()

        If CurrentTransactionID <= 0 Then Exit Sub

        ' تحقق إضافي قبل الاستلام
        Dim canReceive As Boolean = False
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT COUNT(*) 
            FROM Inventory_TransactionHeader 
            WHERE TransactionID = @ID 
              AND StatusID = 5 
              AND IsInventoryPosted = 0
        ", con)
                cmd.Parameters.AddWithValue("@ID", CurrentTransactionID)
                con.Open()
                canReceive = (CInt(cmd.ExecuteScalar()) > 0)
            End Using
        End Using

        If Not canReceive Then
            MessageBox.Show("لا يمكن استلام هذا السند في حالته الحالية", "تنبيه")
            Exit Sub
        End If

        Try
            Dim engine As New TransactionEngine(ConnStr)
            engine.Receive(CurrentTransactionID, CurrentUserID)

            MessageBox.Show("تم الاستلام بنجاح", "تم")

            RefreshFormStatus(CurrentTransactionID)
            LoadTransfersList()
            ApplyDetailsGridAccess()
            RefreshWorkflowUI()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ أثناء الاستلام")
        End Try

    End Sub
    Protected Sub PostInventory(transactionID As Integer)

        If transactionID <= 0 Then
            MessageBox.Show("لا يوجد سند صالح", "تنبيه")
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("dbo.sp_PostStockTransaction", con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                Try
                    con.Open()
                    cmd.ExecuteNonQuery()

                    MessageBox.Show("تم الترحيل بنجاح", "تم")

                    PrepareNewTransaction()
                    LoadTransfersList()

                Catch ex As Exception
                    MessageBox.Show(ex.Message, "خطأ")
                End Try
            End Using
        End Using

    End Sub
    Protected Function IsTransactionEditable(tid As Integer) As Boolean

        If tid <= 0 Then Return False

        Dim isPosted As Boolean = True

        Try
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
SELECT IsInventoryPosted
FROM Inventory_TransactionHeader
WHERE TransactionID = @ID
", con)

                    cmd.Parameters.AddWithValue("@ID", tid)
                    con.Open()

                    Dim result = cmd.ExecuteScalar()
                    If result Is Nothing Then Return False

                    isPosted = CBool(result)
                End Using
            End Using
        Catch
            Return False
        End Try

        Return Not isPosted

    End Function



    ' =========================
    ' تنفيذ الاستقبال
    ' =========================
    ' =========================
    ' تنفيذ الاستقبال (آمن)
    ' =========================

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click

        _allowLeaveGrid = True
        CurrentListMode = TransfersListMode.NormalList

        If dgvTransferDetails.IsCurrentCellInEditMode Then
            dgvTransferDetails.EndEdit()
        End If

        CurrentMode = StockTransactionMode.TransferSend
        dgvTransferDetails.ReadOnly = False
        EditingTransactionID = 0
        CurrentTransactionID = 0
        IsEditMode = False
        PrepareNewTransaction()
        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()
    End Sub

    Private Sub dgvTransfersList_CellMouseDoubleClick(
    sender As Object,
    e As DataGridViewCellMouseEventArgs
) Handles dgvTransfersList.CellMouseDoubleClick

        If e.RowIndex < 0 Then Exit Sub

        If Not dgvTransfersList.Columns.Contains("RefID") Then Exit Sub

        Dim transactionID As Integer =
    CInt(dgvTransfersList.Rows(e.RowIndex).Cells("RefID").Value)

        If transactionID <= 0 Then Exit Sub

        ' ✅ فقط نمرر رقم السند - لا نغير المود
        LoadTransferByTransactionCode(transactionID.ToString())

    End Sub
    Protected Sub LoadTransferByTransactionCode(transactionCode As String)

        _isLoadingTransaction = True
        ResetHeaderControls()

        If dgvTransferDetails.DataSource IsNot Nothing Then
            DetachGridBinding(dgvTransferDetails)
        End If

        Try
            ' =====================================
            ' (1) جلب TransactionID
            ' =====================================
            Dim transactionID As Integer = 0

            If Not Integer.TryParse(transactionCode, transactionID) Then
                MessageBox.Show("رقم السند غير صالح", "تنبيه")
                Exit Sub
            End If

            CurrentTransactionID = transactionID
            EditingTransactionID = transactionID

            ' ✅ لا نغير CurrentMode - نحتفظ بالوضع الحالي

            ' =====================================
            ' (2) تحديث الحالة
            ' =====================================
            RefreshFormStatus(transactionID)

            ' =====================================
            ' (3) تحميل Header
            ' =====================================
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT TransactionID, TransactionDate
                FROM Inventory_TransactionHeader
                WHERE TransactionID = @ID
            ", con)

                    cmd.Parameters.AddWithValue("@ID", transactionID)
                    con.Open()

                    Using rd = cmd.ExecuteReader()
                        If rd.Read() Then
                            txtTransactionCode.Text = rd("TransactionID").ToString()
                            dtpTransactionDate.Value = CDate(rd("TransactionDate"))
                        End If
                    End Using
                End Using
            End Using

            ' =====================================
            ' (4) تحميل Details (باقي الكود كما هو)
            ' =====================================
            Dim dt As DataTable = BuildEmptyDetailsTable()

            Dim sourceStoreID As Integer = 0
            Dim targetStoreID As Integer = 0

            Using con As New SqlConnection(ConnStr)

                Dim sql As String =
"
SELECT
    d.ProductID,
    p.ProductCode,
    p.ProductName,
    p.ProductTypeID,
    d.Quantity,
    p.StorageUnitID AS UnitID,
    u.UnitName,
    d.SourceStoreID,
    d.TargetStoreID
FROM Inventory_TransactionDetails d
INNER JOIN Master_Product p ON p.ProductID = d.ProductID
INNER JOIN Master_Unit u ON u.UnitID = p.StorageUnitID
WHERE d.TransactionID = @ID
"

                ' ✅ نستخدم CurrentMode الحالي للشروط
                If CurrentMode = StockTransactionMode.ProductionReceive _
           OrElse CurrentMode = StockTransactionMode.CuttingReceive Then

                    sql &= " AND d.SourceStoreID IS NULL"

                End If

                Using cmd As New SqlCommand(sql, con)

                    cmd.Parameters.AddWithValue("@ID", transactionID)

                    Using da As New SqlDataAdapter(cmd)
                        Dim tmp As New DataTable()
                        da.Fill(tmp)

                        For Each r As DataRow In tmp.Rows

                            Dim pid As Integer = CInt(r("ProductID"))
                            Dim qty As Decimal = CDec(r("Quantity"))

                            If sourceStoreID = 0 AndAlso Not IsDBNull(r("SourceStoreID")) Then
                                sourceStoreID = CInt(r("SourceStoreID"))
                            End If

                            If targetStoreID = 0 AndAlso Not IsDBNull(r("TargetStoreID")) Then
                                targetStoreID = CInt(r("TargetStoreID"))
                            End If

                            Dim targetOnHand As Decimal =
                        If(targetStoreID > 0,
                           GetProductOnHand(pid, targetStoreID),
                           0D)

                            Dim row = dt.NewRow()
                            row("ProductID") = pid
                            row("ProductCode") = r("ProductCode").ToString()
                            row("ProductTypeID") = CInt(r("ProductTypeID"))
                            row("ProductName") = r("ProductName").ToString()
                            row("UnitName") = r("UnitName").ToString()
                            row("SourceOnHand") = 0D
                            row("TransferQty") = qty
                            row("UnitID") = CInt(r("UnitID"))
                            row("TargetAfter") = targetOnHand
                            row("LineNotes") = ""
                            row("SourceStoreID") =
                        If(sourceStoreID > 0, sourceStoreID, DBNull.Value)
                            row("TargetStoreID") =
                        If(targetStoreID > 0, targetStoreID, DBNull.Value)

                            dt.Rows.Add(row)

                        Next
                    End Using

                End Using

            End Using

            ' =====================================
            ' (5) باقي الكود كما هو
            ' =====================================
            If CurrentMode = StockTransactionMode.PurchaseReceive Then
                Dim colCode = CType(dgvTransferDetails.Columns("colProductCode"),
                    DataGridViewComboBoxColumn)
                colCode.DataSource = LoadAllProductCodes()
                colCode.DisplayMember = "ProductCode"
                colCode.ValueMember = "ProductCode"
            End If

            SyncUIWithMode() ' هذا يضبط حالة الكمبوهات فقط

            ' تعيين المصدر إذا كان موجوداً
            If sourceStoreID > 0 Then
                For Each item As DataRowView In cboSourceStore.Items
                    If Convert.ToInt32(item("StoreID")) = sourceStoreID Then
                        cboSourceStore.SelectedItem = item
                        Exit For
                    End If
                Next
            End If

            ' تعيين الهدف إذا كان موجوداً
            If targetStoreID > 0 Then
                For Each item As DataRowView In cboTargetStore.Items
                    If Convert.ToInt32(item("StoreID")) = targetStoreID Then
                        cboTargetStore.SelectedItem = item
                        Exit For
                    End If
                Next
            End If

            ' تعبئة كمبوا كود الصنف
            If sourceStoreID > 0 Then
                Dim colCode = CType(dgvTransferDetails.Columns("colProductCode"), DataGridViewComboBoxColumn)
                colCode.DataSource = LoadProductCodesForSourceStore(sourceStoreID)
                colCode.DisplayMember = "ProductCode"
                colCode.ValueMember = "ProductCode"
                colCode.ValueType = GetType(String)
            End If

            ' ربط الجريد
            dgvTransferDetails.DataSource = dt

            For Each r As DataGridViewRow In dgvTransferDetails.Rows
                If r.IsNewRow Then Continue For
                If r.DataBoundItem Is Nothing Then Continue For

                Dim drv As DataRowView = CType(r.DataBoundItem, DataRowView)
                If IsDBNull(drv("ProductID")) Then Continue For

                Dim pid As Integer = CInt(drv("ProductID"))

                If Not IsDBNull(drv("SourceStoreID")) Then
                    drv("SourceOnHand") =
                GetProductOnHand(pid, CInt(drv("SourceStoreID")))
                End If

                If Not IsDBNull(drv("TargetStoreID")) Then
                    drv("TargetAfter") =
                GetProductOnHand(pid, CInt(drv("TargetStoreID")))
                End If

                r.Cells("colTransferQty").Tag = CDec(drv("TransferQty"))
            Next
            ' ✅ تفعيل وضع التعديل فقط إذا كنا في وضع الإرسال
            If CurrentMode = StockTransactionMode.TransferSend Then
                IsEditMode = True
            Else
                IsEditMode = False
            End If
            ' ✅ لا نستدعي LoadTransfersList() هنا لأنها ستغير القائمة
            ' فقط نحدث الواجهة
            SyncUIWithMode()
            ApplyDetailsGridAccess()
            RefreshWorkflowUI()

        Finally
            _isLoadingTransaction = False
        End Try

    End Sub
    Protected Sub LoadTransferForEdit(tid As Integer)

        CurrentTransactionID = tid

        ' =========================
        ' تحديث الحالة
        ' =========================
        RefreshFormStatus(tid)

        ' =========================
        ' Header (متوافق مع الجداول الجديدة)
        ' =========================
        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    TransactionID,
    TransactionDate,
    Notes
FROM Inventory_TransactionHeader
WHERE TransactionID = @TID
", con)

                cmd.Parameters.AddWithValue("@TID", tid)
                con.Open()

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        txtTransactionCode.Text = rd("TransactionID").ToString()
                        dtpTransactionDate.Value = CDate(rd("TransactionDate"))
                    End If
                End Using
            End Using
        End Using

        ' =========================
        ' Details (متوافق مع Quantity + Master_Product)
        ' =========================
        Dim dt As DataTable = BuildEmptyDetailsTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    d.ProductID,
    p.ProductCode,
    p.ProductName,
    p.ProductTypeID,
    d.Quantity,
    p.StorageUnitID AS UnitID,
    u.UnitName,
    d.SourceStoreID,
    d.TargetStoreID
FROM Inventory_TransactionDetails d
INNER JOIN Master_Product p ON p.ProductID = d.ProductID
INNER JOIN Master_Unit u ON u.UnitID = p.StorageUnitID
WHERE d.TransactionID = @TID
ORDER BY d.ProductID
", con)

                cmd.Parameters.AddWithValue("@TID", tid)

                Using da As New SqlDataAdapter(cmd)
                    Dim tmp As New DataTable()
                    da.Fill(tmp)

                    For Each r As DataRow In tmp.Rows

                        Dim pid As Integer = CInt(r("ProductID"))
                        Dim qty As Decimal = CDec(r("Quantity"))

                        Dim srcStoreID As Integer =
                        If(IsDBNull(r("SourceStoreID")), 0, CInt(r("SourceStoreID")))

                        Dim trgStoreID As Integer =
                        If(IsDBNull(r("TargetStoreID")), 0, CInt(r("TargetStoreID")))

                        Dim row = dt.NewRow()
                        row("ProductID") = pid
                        row("ProductCode") = r("ProductCode").ToString()
                        row("ProductTypeID") = CInt(r("ProductTypeID"))
                        row("ProductName") = r("ProductName").ToString()
                        row("SourceOnHand") =
                        If(srcStoreID > 0, GetProductOnHand(pid, srcStoreID), 0D)
                        row("TransferQty") = qty
                        row("UnitID") = CInt(r("UnitID"))
                        row("UnitName") = r("UnitName").ToString()
                        row("TargetAfter") =
                        If(trgStoreID > 0, GetProductOnHand(pid, trgStoreID), 0D)
                        row("LineNotes") = ""
                        row("SourceStoreID") =
                        If(srcStoreID > 0, srcStoreID, DBNull.Value)
                        row("TargetStoreID") =
                        If(trgStoreID > 0, trgStoreID, DBNull.Value)

                        dt.Rows.Add(row)

                    Next
                End Using
            End Using
        End Using

        dgvTransferDetails.DataSource = dt

        For Each r As DataGridViewRow In dgvTransferDetails.Rows
            If r.IsNewRow Then Continue For
            r.Cells("colTransferQty").Tag =
            CDec(r.Cells("colTransferQty").Value)
        Next

        LoadTransfersList()
        RefreshWorkflowUI()
        ApplyDetailsGridAccess()

    End Sub
    Private Function LoadAllProductCodes() As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT DISTINCT ProductCode
            FROM Master_Product
            WHERE IsActive = 1
            ORDER BY ProductCode
        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        Return dt

    End Function
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        _allowLeaveGrid = True

        If dgvTransferDetails.IsCurrentCellInEditMode Then
            dgvTransferDetails.EndEdit()
        End If

        Me.Close()
    End Sub
    Private Sub frmStockTransaction_FormClosing(
sender As Object,
e As FormClosingEventArgs) Handles Me.FormClosing

        ' لا يوجد قفل دائم في قاعدة البيانات
        EditingTransactionID = 0
        CurrentTransactionID = 0

    End Sub
    Private Sub btnTransactionSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnTransactionSearch.Click
        _allowLeaveGrid = True

        Using frm As New frmStockTransactionSearch()

            If frm.ShowDialog() <> DialogResult.OK Then Exit Sub

            If frm.SelectedTransactionID <= 0 Then Exit Sub

            CurrentTransactionID = frm.SelectedTransactionID
            CurrentListMode = TransfersListMode.SingleHeader

            LoadTransfersList()
            LoadTransferByTransactionCode(GetTransactionCode(CurrentTransactionID))

        End Using

    End Sub

    Private Sub cboTargetStore_SelectedValueChanged(
    sender As Object,
    e As EventArgs
) Handles cboTargetStore.SelectedValueChanged

        If _isLoadingStores Then Exit Sub
        If cboTargetStore.SelectedValue Is Nothing Then Exit Sub
        If Not TypeOf cboTargetStore.SelectedValue Is Integer Then Exit Sub

        ' استثناء المشتريات
        If CurrentMode = StockTransactionMode.PurchaseReceive Then
            LoadTransfersList()
            Exit Sub
        End If

        ' منع نفس المصدر والهدف
        If cboSourceStore.SelectedValue IsNot Nothing AndAlso
       TypeOf cboSourceStore.SelectedValue Is Integer Then

            If CInt(cboSourceStore.SelectedValue) = CInt(cboTargetStore.SelectedValue) Then
                MessageBox.Show("لا يمكن اختيار نفس المستودع كمصدر وهدف", "تنبيه")
                cboTargetStore.SelectedIndex = -1
                Exit Sub
            End If
        End If

        ' فلترة فقط في وضع الاستلام
        If CurrentMode = StockTransactionMode.TransferReceive Then
            LoadTransfersList()
            ValidateSourceTargetStores()
        End If

    End Sub

    Private Sub btnSwip_Click(sender As Object, e As EventArgs)

        _allowLeaveGrid = True
        CurrentListMode = TransfersListMode.NormalList

        ' لا سويب في وضع المشتريات
        If CurrentMode = StockTransactionMode.PurchaseReceive Then Exit Sub

        If CurrentMode = StockTransactionMode.TransferSend Then
            CurrentMode = StockTransactionMode.TransferReceive

        ElseIf CurrentMode = StockTransactionMode.TransferReceive Then
            CurrentMode = StockTransactionMode.TransferSend
        End If

        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()

    End Sub
    '------------------------------------------------------------
    '========================================================
    ' تعبئة الترانسكشن من عملية قص
    '========================================================
    Public Sub LoadFromCutting(
    sourceStoreID As Integer,
    targetStoreID As Integer,
    cuttingCode As String,
    cuttingOutputGrid As DataGridView
)

        ' 1️⃣ ضبط المخازن
        cboSourceStore.SelectedValue = sourceStoreID
        cboTargetStore.SelectedValue = targetStoreID

        ' 2️⃣ إنشاء جدول التفاصيل
        Dim dt As DataTable = BuildEmptyDetailsTable()

        ' 3️⃣ المرور على نواتج القص
        For Each r As DataGridViewRow In cuttingOutputGrid.Rows

            If r.IsNewRow Then Continue For
            Dim typeID As Integer = CInt(r.Cells("colOutProductTypeID").Value)
            If typeID = 4 Then Continue For
            Dim productID As Integer = CInt(r.Cells("colProductID").Value)
            Dim productCode As String = r.Cells("colOutProductCode").Value.ToString()
            Dim transferQty As Decimal = CDec(r.Cells("colOutQty").Value)

            ' اسم المنتج
            Dim productName As String = ""
            Dim unitID As Integer = 0

            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand(
            "SELECT ProductName, StorageUnitID 
             FROM Master_Product 
             WHERE ProductID = @ID", con)

                    cmd.Parameters.AddWithValue("@ID", productID)
                    con.Open()

                    Using rd = cmd.ExecuteReader()
                        If rd.Read() Then
                            productName = rd("ProductName").ToString()
                            unitID = CInt(rd("StorageUnitID"))
                        End If
                    End Using
                End Using
            End Using

            ' الرصيد
            Dim sourceAvailable As Decimal =
            GetAvailableQty(productID, sourceStoreID)

            Dim targetOnHand As Decimal =
            GetProductOnHand(productID, targetStoreID)

            ' 4️⃣ إضافة صف
            Dim row = dt.NewRow()

            row("ProductID") = productID
            row("ProductCode") = productCode
            row("ProductName") = productName
            row("SourceOnHand") = sourceAvailable
            row("TransferQty") = transferQty
            row("UnitID") = unitID
            row("TargetAfter") = targetOnHand + transferQty
            row("LineNotes") = ""

            dt.Rows.Add(row)

        Next

        ' 5️⃣ ربط الجدول بالجريد
        dgvTransferDetails.DataSource = dt

        ' 6️⃣ تهيئة Tag للكمية
        For i As Integer = 0 To dgvTransferDetails.Rows.Count - 1
            dgvTransferDetails.Rows(i).Cells("colTransferQty").Tag =
            CDec(dgvTransferDetails.Rows(i).Cells("colTransferQty").Value)
        Next

    End Sub

    Private Sub CancelTransaction(transactionID As Integer)

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("dbo.sp_CancelStockTransaction", con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@UserID", CurrentUserID)

                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub
    Private Sub RefreshWorkflowUI()

        ' 🟢 حالة السند الجديد (إرسال فقط)
        If CurrentTransactionID = 0 Then
            If CurrentMode = StockTransactionMode.TransferSend Then
                btnSend.Enabled = True
                btnSend.Text = "إرسال"
            End If
            Exit Sub
        End If

        ' تحديث الحالة من قاعدة البيانات
        RefreshFormStatus(CurrentTransactionID)

        ' جلب حالة السند الحالية
        Dim isPosted As Boolean = False
        Dim currentStatusID As Integer = 0

        Try
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT 
                    IsInventoryPosted,
                    StatusID
                FROM Inventory_TransactionHeader
                WHERE TransactionID = @ID", con)

                    cmd.Parameters.AddWithValue("@ID", CurrentTransactionID)
                    con.Open()

                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            isPosted = reader.GetBoolean(0)
                            currentStatusID = reader.GetInt32(1)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Exit Sub
        End Try

        ' ===========================================
        ' ✅ تفعيل زر الاستلام (btnSend) في modes الاستلام
        ' الشرط: StatusID = 5 AND IsInventoryPosted = False
        ' ===========================================
        Select Case CurrentMode
            Case StockTransactionMode.TransferReceive
                If currentStatusID = 5 AndAlso Not isPosted Then
                    btnSend.Enabled = True
                    btnCancel.Enabled = False
                End If


            Case StockTransactionMode.PurchaseReceive,
          StockTransactionMode.ProductionReceive,
          StockTransactionMode.CuttingReceive,
          StockTransactionMode.SalesReturnReceive

                ' الشرط الصحيح: السند مرسل (StatusID = 5) وغير مرحل (IsPosted = False)
                If currentStatusID = 5 AndAlso Not isPosted Then
                    btnSend.Enabled = True
                Else
                    btnSend.Enabled = False
                    ' تحديد سبب عدم التفعيل للتصحيح (يمكن إزالته في الإنتاج)
                    Debug.WriteLine($"استلام غير مفعل: StatusID={currentStatusID}, IsPosted={isPosted}")
                End If

            Case StockTransactionMode.TransferSend

                If currentStatusID = 5 AndAlso Not isPosted Then
                    btnSend.Enabled = True
                    btnCancel.Enabled = True
                Else
                    btnSend.Enabled = False
                End If

        End Select

        ' ===========================================
        ' ✅ تفعيل أزرار أخرى
        ' ===========================================

        ' زر إلغاء الاستلام (إلغاء الترحيل)
        btnCancelReceive.Enabled = isPosted AndAlso currentStatusID = 6


    End Sub
    Public Sub LoadTransaction(transactionID As Integer)

        CurrentTransactionID = transactionID
        EditingTransactionID = transactionID

        LoadTransfersList()
    End Sub
    Private Function GetTransactionCode(tid As Integer) As String

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT TransactionID
            FROM Inventory_TransactionHeader
            WHERE TransactionID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", tid)
                con.Open()

                Dim r = cmd.ExecuteScalar()
                If r Is Nothing Then Return ""

                Return r.ToString()

            End Using
        End Using

    End Function
    Private Sub DetachGridBinding(dgv As DataGridView)

        Dim cm As CurrencyManager =
        CType(Me.BindingContext(dgv.DataSource), CurrencyManager)

        If cm IsNot Nothing Then
            cm.EndCurrentEdit()
            cm.SuspendBinding()
        End If

        dgv.DataSource = Nothing

        If cm IsNot Nothing Then
            cm.ResumeBinding()
        End If

    End Sub
    Protected Sub ResetHeaderControls()

        txtTransactionCode.Clear()
        dtpTransactionDate.Value = Date.Today

        ' 🟡 مهم: تصفير الكمبوهات صراحة
        cboSourceStore.SelectedIndex = -1
        cboTargetStore.SelectedIndex = -1

        cboSourceStore.Enabled = True
        cboTargetStore.Enabled = True

    End Sub
    Private Sub ApplyDetailsGridAccess()
        ' ✅ إنهاء أي تعديل جاري
        If dgvTransferDetails.IsCurrentCellInEditMode Then
            dgvTransferDetails.EndEdit()
        End If

        ' إبقاء جميع الأعمدة مرئية
        dgvTransferDetails.Columns("colDelete").Visible = True
        dgvTransferDetails.Columns("colProductSearch").Visible = True

        Select Case CurrentMode
            Case StockTransactionMode.TransferReceive,
             StockTransactionMode.PurchaseReceive,
             StockTransactionMode.ProductionReceive,
             StockTransactionMode.CuttingReceive,
             StockTransactionMode.SalesReturnReceive

                ' في جميع مودات الاستلام
                dgvTransferDetails.ReadOnly = True
                dgvTransferDetails.Enabled = True
                dgvTransferDetails.TabStop = False ' ❌ لا يدخل بالتاب

                ' تعطيل الأزرار
                Dim btnDelete As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colDelete"), DataGridViewButtonColumn)
                btnDelete.DefaultCellStyle.BackColor = Color.LightGray
                btnDelete.DefaultCellStyle.ForeColor = Color.DarkGray

                Dim btnSearch As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colProductSearch"), DataGridViewButtonColumn)
                btnSearch.DefaultCellStyle.BackColor = Color.LightGray
                btnSearch.DefaultCellStyle.ForeColor = Color.DarkGray

                ' تعطيل الأعمدة القابلة للتحرير
                dgvTransferDetails.Columns("colProductCode").ReadOnly = True
                dgvTransferDetails.Columns("colProductType").ReadOnly = True
                dgvTransferDetails.Columns("colTransferQty").ReadOnly = True
                dgvTransferDetails.Columns("colLineNotes").ReadOnly = True

            Case StockTransactionMode.TransferSend
                ' ✅ في مود الإرسال
                dgvTransferDetails.ReadOnly = False
                dgvTransferDetails.Enabled = True
                dgvTransferDetails.TabStop = True ' ✅ يدخل بالتاب

                ' إعادة الأزرار للونها الطبيعي
                Dim btnDelete As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colDelete"), DataGridViewButtonColumn)
                btnDelete.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200)
                btnDelete.DefaultCellStyle.ForeColor = Color.Black

                Dim btnSearch As DataGridViewButtonColumn = CType(dgvTransferDetails.Columns("colProductSearch"), DataGridViewButtonColumn)
                btnSearch.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240)
                btnSearch.DefaultCellStyle.ForeColor = Color.Black

                ' تفعيل الأعمدة القابلة للتحرير
                dgvTransferDetails.Columns("colProductCode").ReadOnly = False
                dgvTransferDetails.Columns("colProductType").ReadOnly = False
                dgvTransferDetails.Columns("colTransferQty").ReadOnly = False
                dgvTransferDetails.Columns("colLineNotes").ReadOnly = False

        End Select
    End Sub
    Private Sub dgvTransferDetails_CellMouseDown(
    sender As Object,
    e As DataGridViewCellMouseEventArgs
) Handles dgvTransferDetails.CellMouseDown

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String =
        dgvTransferDetails.Columns(e.ColumnIndex).Name

        Select Case colName
            Case "colProductSearch",
             "colProductCode",
             "colProductType",
             "colTransferQty",
             "colDelete"
                ' مسموح
            Case Else
                dgvTransferDetails.ClearSelection()
                Exit Sub
        End Select

    End Sub
    Private Sub dgvTransferDetails_EditingControlShowing(
    sender As Object,
    e As DataGridViewEditingControlShowingEventArgs
) Handles dgvTransferDetails.EditingControlShowing

        If TypeOf e.Control Is ComboBox Then
            Dim cb As ComboBox = CType(e.Control, ComboBox)
            cb.DropDownStyle = ComboBoxStyle.DropDownList
            cb.DroppedDown = False   ' ⛔ لا فتح تلقائي
        End If

    End Sub
    Private Sub dgvTransferDetails_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvTransferDetails.CurrentCellDirtyStateChanged

        If dgvTransferDetails.IsCurrentCellDirty Then
            dgvTransferDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        ' إذا كان TAB
        If keyData = Keys.Tab Then
            ' إذا كان التركيز على الجريد العلوي
            If dgvTransfersList.Focused Then
                ' في وضع الإرسال: ننتقل للجريد السفلي
                If CurrentMode = StockTransactionMode.TransferSend Then
                    If dgvTransferDetails.Rows.Count > 0 AndAlso dgvTransferDetails.Enabled Then
                        dgvTransferDetails.Focus()
                        If dgvTransferDetails.Rows(0).Cells("colProductCode").Visible Then
                            dgvTransferDetails.CurrentCell = dgvTransferDetails.Rows(0).Cells("colProductCode")
                        End If
                    End If
                Else
                    ' في باقي المودات: ننتقل للعنصر التالي بعد الجريد العلوي
                    Me.SelectNextControl(dgvTransfersList, True, True, True, True)
                End If
                Return True ' منع المعالجة الافتراضية

                ' إذا كان التركيز على الجريد السفلي
            ElseIf dgvTransferDetails.Focused Then
                ' في وضع الإرسال: نسمح بالتنقل داخل الجريد
                If CurrentMode = StockTransactionMode.TransferSend Then
                    ' التنقل الطبيعي داخل الجريد
                    Return MyBase.ProcessDialogKey(keyData)
                Else
                    ' في باقي المودات: نخرج من الجريد السفلي
                    Me.SelectNextControl(dgvTransferDetails, True, True, True, True)
                    Return True
                End If
            End If
        End If

        ' إذا كان Shift+Tab
        If keyData = (Keys.Shift Or Keys.Tab) Then
            ' إذا كان التركيز على الجريد السفلي
            If dgvTransferDetails.Focused Then
                ' في وضع الإرسال: نسمح بالتنقل العكسي داخل الجريد
                If CurrentMode = StockTransactionMode.TransferSend Then
                    Return MyBase.ProcessDialogKey(keyData)
                Else
                    ' في باقي المودات: نرجع للجريد العلوي
                    dgvTransfersList.Focus()
                    Return True
                End If
            End If
        End If

        Return MyBase.ProcessDialogKey(keyData)
    End Function

    Private Function IsDuplicateProductID(
    currentRowIndex As Integer,
    productID As Integer
) As Boolean

        If productID <= 0 Then Return False

        For Each r As DataGridViewRow In dgvTransferDetails.Rows
            If r.Index = currentRowIndex Then Continue For
            If r.IsNewRow Then Continue For

            If r.Cells("colProductID").Value IsNot Nothing AndAlso
           Not IsDBNull(r.Cells("colProductID").Value) AndAlso
           CInt(r.Cells("colProductID").Value) = productID Then

                Return True
            End If
        Next

        Return False
    End Function

    Private Sub dgvTransferDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvTransferDetails.KeyDown
        ' إذا كان في وضع غير الإرسال
        If CurrentMode <> StockTransactionMode.TransferSend Then
            ' نمنع أي حركة بالأسهم داخل الجريد
            If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down OrElse
           e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right OrElse
           e.KeyCode = Keys.Enter Then
                e.Handled = True
                ' ننقل التركيز للعنصر التالي
                Me.SelectNextControl(dgvTransferDetails, True, True, True, True)
            End If
        End If
    End Sub

    Private Sub dgvTransferDetails_Enter(sender As Object, e As EventArgs) Handles dgvTransferDetails.Enter
        ' عند الدخول للجريد السفلي
        If CurrentMode <> StockTransactionMode.TransferSend Then
            ' إذا لم يكن في وضع الإرسال، نخرج فوراً
            Me.ActiveControl = Nothing
            Me.SelectNextControl(dgvTransferDetails, True, True, True, True)
        End If
    End Sub

    Private Sub dgvTransferDetails_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvTransferDetails.CellContentClick

        _allowLeaveGrid = True

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        ' ✅ منع النقر على الأزرار في وضع الاستلام
        If CurrentMode <> StockTransactionMode.TransferSend Then
            Exit Sub
        End If

        Dim colName As String = dgvTransferDetails.Columns(e.ColumnIndex).Name
        Dim row As DataGridViewRow = dgvTransferDetails.Rows(e.RowIndex)
        If row Is Nothing Then Exit Sub

        '=========================
        ' زر الحذف
        '=========================
        If colName = "colDelete" Then

            If dgvTransferDetails.IsCurrentCellInEditMode Then
                dgvTransferDetails.EndEdit()
            End If

            ' إذا الصف الوحيد → فرّغه بدل الحذف
            Dim realRowsCount As Integer =
        dgvTransferDetails.Rows.Cast(Of DataGridViewRow)().
        Count(Function(r) Not r.IsNewRow)

            If realRowsCount <= 1 Then
                row.Cells("colProductCode").Value = Nothing
                row.Cells("colProductType").Value = Nothing
                row.Cells("colProductID").Value = Nothing
                ClearRowUI(row)
                Exit Sub
            End If

            ' حذف كامل
            dgvTransferDetails.Rows.RemoveAt(e.RowIndex)
            SelectPreviousRowAfterDelete(e.RowIndex)
            Exit Sub
        End If

        '=========================
        ' زر البحث
        '=========================
        If colName = "colProductSearch" Then

            If cboSourceStore.SelectedValue Is Nothing Then
                MessageBox.Show("يجب اختيار مستودع المصدر أولاً", "تنبيه")
                Exit Sub
            End If

            If dgvTransferDetails.IsCurrentCellInEditMode Then
                dgvTransferDetails.EndEdit()
            End If

            Using frm As New frmProductSearch()
                frm.SearchFilter = ProductSearchFilter.InSourceStoreWithBalance
                frm.SourceStoreID = CInt(cboSourceStore.SelectedValue)
                frm.OnlyWithBalance = True
                If frm.ShowDialog() <> DialogResult.OK Then Exit Sub
                FillRowFromProductSearch_Tr(e.RowIndex, frm)
            End Using

            Exit Sub
        End If

    End Sub
    Private Sub FillRowFromProductSearch_Tr(
rowIndex As Integer,
frm As frmProductSearch
)

        Dim row As DataGridViewRow = dgvTransferDetails.Rows(rowIndex)

        ' منع التكرار
        If IsProductAlreadyAdded(frm.SelectedProductID) Then
            MessageBox.Show("هذا الصنف مضاف مسبقًا", "تنبيه")
            Exit Sub
        End If

        row.Cells("colProductID").Value = frm.SelectedProductID
        row.Cells("colProductCode").Value = frm.SelectedProductCode
        row.Cells("colProductType").Value = frm.SelectedProductTypeID

        ' تعبئة باقي البيانات من النظام
        FillProductRowByProductID(row, frm.SelectedProductID)

    End Sub
    Private Function LoadUnits() As DataTable

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

        Return dt

    End Function
    Private Sub PrepareTransfersListGrid()

        With dgvTransfersList
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .EnableHeadersVisualStyles = False

            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 248)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
            .ColumnHeadersHeight = 36

            .DefaultCellStyle.Font = New Font("Segoe UI", 9.5)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 252)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            .RowTemplate.Height = 30
            .GridColor = Color.FromArgb(230, 230, 230)

            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .TabStop = False
            dgvTransferDetails.ColumnHeadersDefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleCenter

            dgvTransfersList.ColumnHeadersDefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleCenter
            dgvTransferDetails.DefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleCenter

            dgvTransfersList.DefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleCenter
            dgvTransferDetails.Columns("colTransferQty").DefaultCellStyle.Format = "N2"
            dgvTransferDetails.Columns("colSourceOnHand").DefaultCellStyle.Format = "N2"
            dgvTransferDetails.Columns("colTargetAfter").DefaultCellStyle.Format = "N2"
            dgvTransferDetails.Columns("colTransferQty").DefaultCellStyle.Alignment =
    DataGridViewContentAlignment.MiddleCenter

        End With

    End Sub

    Private Sub ClearRowUI(row As DataGridViewRow)

        If row Is Nothing Then Exit Sub

        row.Cells("colProductID").Value = Nothing
        row.Cells("colProductName").Value = Nothing
        row.Cells("colUnit").Value = Nothing
        row.Cells("colSourceOnHand").Value = Nothing
        row.Cells("colTransferQty").Value = Nothing
        row.Cells("colTransferQty").Tag = 0D
        row.Cells("colTargetAfter").Value = Nothing
        row.Cells("colLineNotes").Value = Nothing

    End Sub

    Private Sub SelectPreviousRowAfterDelete(deletedRowIndex As Integer)

        If dgvTransferDetails.Rows.Count = 0 Then Exit Sub

        Dim idx As Integer = deletedRowIndex - 1
        If idx < 0 Then idx = 0

        If idx >= dgvTransferDetails.Rows.Count Then idx = dgvTransferDetails.Rows.Count - 1

        If dgvTransferDetails.Rows(idx).IsNewRow Then
            If idx > 0 Then idx -= 1
        End If

        If idx >= 0 AndAlso idx < dgvTransferDetails.Rows.Count Then
            dgvTransferDetails.CurrentCell = dgvTransferDetails.Rows(idx).Cells("colProductCode")
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        If CurrentTransactionID <= 0 Then Exit Sub

        If MessageBox.Show(
        "هل أنت متأكد من إلغاء هذا السند؟",
        "تأكيد الإلغاء",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    ) <> DialogResult.Yes Then Exit Sub

        Try
            ' استدعاء TransactionEngine مباشرة (وليس الإجراء المخزن)
            Dim engine As New TransactionEngine(ConnStr)
            engine.CancelTransaction(CurrentTransactionID, CurrentUserID)

            MessageBox.Show("تم إلغاء السند بنجاح", "تم")

            RefreshFormStatus(CurrentTransactionID)
            LoadTransfersList()
            ApplyDetailsGridAccess()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ أثناء الإلغاء")
        End Try

    End Sub

    Private Sub rdoProductionReceive_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rdoProductionReceive.CheckedChanged

        _allowLeaveGrid = True
        If _isLoadingTransaction Then Exit Sub
        PrepareNewTransaction()
        ' لا حاجة لإطفاء المشتريات يدويًا مع RadioButton
        ' لأن النظام يفعل ذلك تلقائيًا

        CurrentListMode = TransfersListMode.NormalList
        CurrentTransactionID = 0
        EditingTransactionID = 0

        _isLoadingTransaction = True
        Try
            SafeResetGrid(dgvTransferDetails)

            If rdoProductionReceive.Checked Then
                CurrentMode = StockTransactionMode.ProductionReceive

                cboSourceStore.SelectedIndex = -1
                cboSourceStore.Enabled = False
                cboTargetStore.Enabled = True
            Else
                CurrentMode = StockTransactionMode.TransferSend

                cboSourceStore.Enabled = True
                cboTargetStore.Enabled = True
            End If

            dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
            Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
            If dt.Rows.Count = 0 Then
                dt.Rows.Add(dt.NewRow())
            End If

        Finally
            _isLoadingTransaction = False
        End Try

        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()

    End Sub
    Private Sub rdoPurchaseReceive_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rdoPurchaseReceive.CheckedChanged

        _allowLeaveGrid = True
        PrepareNewTransaction()
        If _isLoadingTransaction Then Exit Sub
        If Not rdoPurchaseReceive.Checked Then Exit Sub

        CurrentListMode = TransfersListMode.NormalList
        CurrentTransactionID = 0
        EditingTransactionID = 0

        _isLoadingTransaction = True
        Try
            SafeResetGrid(dgvTransferDetails)

            CurrentMode = StockTransactionMode.PurchaseReceive

            cboSourceStore.SelectedIndex = -1
            cboSourceStore.Enabled = False
            cboTargetStore.Enabled = True

            dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
            dgvTransferDetails.ReadOnly = False
            Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
            If dt.Rows.Count = 0 Then
                dt.Rows.Add(dt.NewRow())
            End If

        Finally
            _isLoadingTransaction = False
        End Try

        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()




    End Sub
    Private Sub rdoCuttingReceive_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rdoCuttingReceive.CheckedChanged

        _allowLeaveGrid = True
        PrepareNewTransaction()
        If Not rdoCuttingReceive.Checked Then Exit Sub
        If _isLoadingTransaction Then Exit Sub

        ' إطفاء بقية الأنواع
        rdoPurchaseReceive.Checked = False
        rdoProductionReceive.Checked = False
        rdoReturnReceive.Checked = False

        CurrentListMode = TransfersListMode.NormalList
        CurrentTransactionID = 0
        EditingTransactionID = 0

        _isLoadingTransaction = True
        Try
            SafeResetGrid(dgvTransferDetails)

            ' وضع استلام القص
            CurrentMode = StockTransactionMode.CuttingReceive

            ' القص لا يملك مخزن مصدر
            cboSourceStore.SelectedIndex = -1
            cboSourceStore.Enabled = False

            ' المخزن الهدف فقط
            cboTargetStore.Enabled = True

            dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
            dgvTransferDetails.ReadOnly = False
            Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
            If dt.Rows.Count = 0 Then
                dt.Rows.Add(dt.NewRow())
            End If

        Finally
            _isLoadingTransaction = False
        End Try

        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()

    End Sub


    Private Sub rdoReturnReceive_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rdoReturnReceive.CheckedChanged

        _allowLeaveGrid = True
        PrepareNewTransaction()
        If Not rdoReturnReceive.Checked Then Exit Sub
        If _isLoadingTransaction Then Exit Sub

        CurrentListMode = TransfersListMode.NormalList
        CurrentTransactionID = 0
        EditingTransactionID = 0

        _isLoadingTransaction = True
        Try
            SafeResetGrid(dgvTransferDetails)

            CurrentMode = StockTransactionMode.SalesReturnReceive

            cboSourceStore.SelectedIndex = -1
            cboSourceStore.Enabled = False
            cboTargetStore.Enabled = True

            dgvTransferDetails.DataSource = BuildEmptyDetailsTable()
            dgvTransferDetails.ReadOnly = False
            Dim dt = CType(dgvTransferDetails.DataSource, DataTable)
            If dt.Rows.Count = 0 Then
                dt.Rows.Add(dt.NewRow())
            End If

        Finally
            _isLoadingTransaction = False
        End Try

        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()

    End Sub
    Private Sub btnCancelReceive_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnCancelReceive.Click

        _allowLeaveGrid = True

        ' التحقق من وجود سند
        If CurrentTransactionID <= 0 Then
            MessageBox.Show("الرجاء تحميل سند أولاً", "تنبيه",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim canReverse As Boolean = False

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
        SELECT COUNT(*)
        FROM Inventory_TransactionHeader
        WHERE TransactionID = @ID
          AND StatusID = 6
          AND IsInventoryPosted = 1
    ", con)

                cmd.Parameters.AddWithValue("@ID", CurrentTransactionID)
                con.Open()
                canReverse = (CInt(cmd.ExecuteScalar()) > 0)
            End Using
        End Using

        If Not canReverse Then
            MessageBox.Show("لا يمكن إلغاء هذا السند في حالته الحالية.", "تنبيه")
            Exit Sub
        End If
        ' طلب سبب الإلغاء مباشرة
        Dim reason As String = InputBox("الرجاء إدخال سبب إلغاء الاستلام:", "سبب الإلغاء", "")

        ' التحقق من الإدخال
        If String.IsNullOrWhiteSpace(reason) Then
            MessageBox.Show("لم يتم إدخال سبب الإلغاء. تم إلغاء العملية.", "تنبيه",
                           MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' تأكيد نهائي
        If MessageBox.Show(
            "هل أنت متأكد من إلغاء استلام هذا السند؟" & vbCrLf &
            "السبب: " & reason & vbCrLf &
            "هذا الإجراء لا يمكن التراجع عنه.",
            "تأكيد نهائي",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        ) <> DialogResult.Yes Then
            Exit Sub
        End If

        ' استدعاء الدالة في الـ Engine
        Try
            Dim engine As New TransactionEngine(ConnStr)
            engine.ReverseReceiveTransaction(
                CurrentTransactionID,
                CurrentUserID,
                reason
            )

            MessageBox.Show("تم إلغاء الاستلام بنجاح", "نجاح",
                           MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' تحديث الفورم
            RefreshFormStatus(CurrentTransactionID)
            LoadTransfersList()
            ApplyDetailsGridAccess()
            RefreshWorkflowUI()

            ' تفريغ الجريد السفلي
            LoadTransaction(CurrentTransactionID)
        Catch ex As Exception
            MessageBox.Show("خطأ في إلغاء الاستلام: " & ex.Message, "خطأ",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub rdoPRTSend_CheckedChanged(sender As Object, e As EventArgs) Handles rdoPRTSend.CheckedChanged

        If Not rdoPRTSend.Checked Then Return

        CurrentMode = StockTransactionMode.PurchaseReturnSend

        PrepareNewTransaction()
        SyncUIWithMode()
        LoadTransfersList()
        ApplyDetailsGridAccess()

    End Sub


End Class
