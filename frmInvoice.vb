Imports System.Data.SqlClient
Imports System.Linq
Imports System.Security.Cryptography

Public Class frmInvoice
    Inherits AABaseOperationForm
    Public Property SelectedDocumentID As Integer = 0
    Public Property SourceLOID As Integer
    Private IsProcessingKey As Boolean = False
    ' داخل كلاس frmInvoice
    Private SourceLOModifiedAt As DateTime?
    Public Property SourceSRID As Integer
    Private InvoiceDetailsDT As DataTable
    Private LastStoreID As Integer? = Nothing
    Private IsResolvingProduct As Boolean = False
    Private PendingSRIDs As List(Of Integer)
    Private CurrentInvoiceIndex As Integer = 0
    Private IsLoadingInvoiceDetails As Boolean = False
    Private PendingSRIDs_FromLoading As List(Of Integer)


    Private IsInitializingInvoice As Boolean = False
    Private IsAutoAddingRow As Boolean = False

    Private Const MSG_INVALID_DISCOUNT As String =
    "قيمة الخصم غير صحيحة"
    ' =========================
    ' Invoice Context (Draft)
    ' =========================
    Private Const SOURCE_TYPE_INVOICE As String = "SAL"
    Private Const TRANSACTION_TYPE_SALE As String = "SAL"   ' TransactionType.Code
    Private Const DEFAULT_VAT_RATE As Decimal = 0.15D        ' 15%
    ' =========================
    ' Invoice Context (NOT UI)
    ' =========================
    Private CurrentTransactionTypeCode As String = "SAL"
    Private CurrentSourceType As String = "SAL"
    Private CurrentInvoiceDate As DateTime = DateTime.Now
    Private CurrentIssueDateTime As DateTime = DateTime.Now
    Private CurrentInvoiceID As Integer = 0
    ' =========================
    ' Context Variables
    ' =========================
    Private CurrentEmployeeID As Integer = 1   ' مؤقتًا، اربطه بالجلسة لاحقًا
    Private CurrentInvoiceType As String = ""
    Private CurrentEnvironmentMode As String = ""
    Private CurrentSourceDocumentID As Integer = 0
    Private CurrentSourceDocumentType As String = ""


    Private Sub LoadTaxReasonCombo()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT 
   TaxReasonID,
ReasonCode,
ReasonNameAr

FROM Master_TaxReason
WHERE IsActive = 1
ORDER BY ReasonNameAr
", con)

                Using da As New SqlDataAdapter(cmd)

                    Dim dt As New DataTable()
                    da.Fill(dt)

                    cboTaxReason.DataSource = dt
                    cboTaxReason.DisplayMember = "ReasonNameAr"
                    cboTaxReason.ValueMember = "TaxReasonID"

                    cboTaxReason.SelectedIndex = -1

                End Using
            End Using
        End Using

    End Sub




    ' =========================
    ' STEP 1 – Load Combos
    ' =========================

    Private Sub LoadAllCombos()

        LoadStores()
        LoadPaymentMethods()
        LoadPaymentTerms()
        LoadPartners()
        LoadProductTypes()
        LoadTaxTypes()
        LoadInvoiceStatuses()

    End Sub


    Private Sub LoadStores()

        LoadCombo(
        cboSourceStoreID,
        "SELECT StoreID, StoreName 
         FROM Master_Store 
         WHERE IsActive = 1 
           AND StoreID IN (1,4)
         ORDER BY StoreID",
        "StoreName",
        "StoreID"
    )

    End Sub

    Private Sub LoadPaymentMethods()
        LoadCombo(
        cboPaymentMethodID,
        "SELECT PaymentMethodID, NameAr FROM Master_PaymentMethod WHERE IsActive = 1",
        "NameAr",
        "PaymentMethodID"
    )
    End Sub

    Private Sub LoadPaymentTerms()
        LoadCombo(
        cboPaymentTerm,
        "SELECT PaymentTermID, NameAr FROM Master_PaymentTerm WHERE IsActive = 1",
        "NameAr",
        "PaymentTermID"
    )
    End Sub

    Private Sub LoadPartners()

        LoadCombo(
        cboPartnerID,
        "
        SELECT
            PartnerID,
            PartnerCode,
            PartnerName,
            VATRegistrationNumber,
            Phone,
            Address
        FROM Master_Partner
        WHERE IsActive = 1
          AND PartnerCode LIKE 'CUS-%'
        ORDER BY PartnerCode
        ",
        "PartnerCode",
        "PartnerID"
    )

    End Sub


    Private Sub LoadProductTypes()
        LoadGridCombo(
        colDetProductType,
        "SELECT ProductTypeID, TypeName FROM Master_ProductType WHERE IsActive = 1",
        "TypeName",
        "ProductTypeID"
    )
    End Sub


    Private Sub LoadTaxTypes()
        LoadGridCombo(
        colDetTaxPercent,
        "SELECT TaxTypeID, TaxName FROM Master_TaxType WHERE IsActive = 1",
        "TaxName",
        "TaxTypeID"
    )
    End Sub

    Private Sub LoadInvoiceStatuses()
        LoadCombo(
    cboStatusID,
    "
    SELECT
        StatusID,
        StatusCode,
        StatusName
    FROM Workflow_Status
    ",
    "StatusName",
    "StatusID"
)


    End Sub
    Private Sub cboPartnerID_SelectionChangeCommitted(
    sender As Object,
    e As EventArgs
) Handles cboPartnerID.SelectionChangeCommitted

        Dim drv As DataRowView =
        TryCast(cboPartnerID.SelectedItem, DataRowView)

        If drv Is Nothing Then Exit Sub

        txtPartnerName.Text =
        drv("PartnerName").ToString()

        txtVATRegistrationNumber.Text =
        If(IsDBNull(drv("VATRegistrationNumber")),
           "",
           drv("VATRegistrationNumber").ToString())

    End Sub

    Private Sub InitInvoiceDetailsTable()

        InvoiceDetailsDT = New DataTable
        InvoiceDetailsDT.Columns.Add("SourceLoadingOrderDetailID", GetType(Integer))
        InvoiceDetailsDT.Columns("SourceLoadingOrderDetailID").AllowDBNull = True

        InvoiceDetailsDT.Columns.Add("SRDID", GetType(Integer))
        InvoiceDetailsDT.Columns.Add("BaseProductCode", GetType(String))
        InvoiceDetailsDT.Columns.Add("ProductID", GetType(Integer))
        InvoiceDetailsDT.Columns.Add("ProductCode", GetType(String))
        InvoiceDetailsDT.Columns.Add("ProductName", GetType(String))
        InvoiceDetailsDT.Columns.Add("ProductTypeID", GetType(Integer))

        InvoiceDetailsDT.Columns.Add("Quantity", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("UnitID", GetType(Integer))
        InvoiceDetailsDT.Columns.Add("UnitName", GetType(String))
        InvoiceDetailsDT.Columns.Add("UnitCode", GetType(String))

        InvoiceDetailsDT.Columns.Add("PieceSellPrice", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("M3SellPrice", GetType(Decimal))

        InvoiceDetailsDT.Columns.Add("GrossAmount", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("DiscountRate", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("DiscountAmount", GetType(Decimal))

        InvoiceDetailsDT.Columns.Add("NetAmount", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("TaxRate", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("TaxTypeID", GetType(Integer)).DefaultValue = 1
        InvoiceDetailsDT.Columns.Add("TaxAmount", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("Total", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("LineTotal", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("SourceStoreID", GetType(Integer))

        InvoiceDetailsDT.Columns.Add("Note", GetType(String))
        InvoiceDetailsDT.Columns.Add("Length", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("Width", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("Height", GetType(Decimal))

    End Sub
    Private Sub BindInvoiceGrid()

        dgvInvoiceDetails.AutoGenerateColumns = False
        dgvInvoiceDetails.DataSource = InvoiceDetailsDT

        colDetSRDID.DataPropertyName = "SRDID"

        colDetBaseProductCode.DataPropertyName = "BaseProductCode"
        colDetProductID.DataPropertyName = "ProductID"
        colDetUnitID.DataPropertyName = "UnitID"
        colDetUnitID.Visible = False
        colDetSellUnit.ReadOnly = True



        ' ComboBox
        colDetProductCode.DataPropertyName = "ProductCode"
        colDetProductCode.ValueMember = "ProductCode"
        colDetProductCode.DisplayMember = "ProductCode"

        colProductName.DataPropertyName = "ProductName"
        colProductName.ReadOnly = True

        colDetProductType.DataPropertyName = "ProductTypeID"

        colDetQty.DataPropertyName = "Quantity"
        colDetSellUnit.DataPropertyName = "UnitName"

        colDetPieceSellPrice.DataPropertyName = "PieceSellPrice"
        colDetM3SellPrice.DataPropertyName = "M3SellPrice"

        colDetLineAmount.DataPropertyName = "GrossAmount"
        colDetDiscountPercent.DataPropertyName = "DiscountRate"
        colDetDiscountAmount.DataPropertyName = "DiscountAmount"
        colDetTaxPercent.ReadOnly = True

        colDetTaxableAmount.DataPropertyName = "NetAmount"
        colDetTaxPercent.DataPropertyName = "TaxTypeID"
        colDetTaxAmount.DataPropertyName = "TaxAmount"
        colTotal.DataPropertyName = "LineTotal"

        colDetNote.DataPropertyName = "Note"

    End Sub
    Private Sub ApplyDecimalFormatting()

        ' ===== أرقام عشرية =====
        Dim numFmt As String = "0.000"

        colDetQty.DefaultCellStyle.Format = numFmt
        colDetPieceSellPrice.DefaultCellStyle.Format = numFmt
        colDetM3SellPrice.DefaultCellStyle.Format = numFmt
        colDetLineAmount.DefaultCellStyle.Format = numFmt
        colDetDiscountAmount.DefaultCellStyle.Format = numFmt
        colDetTaxableAmount.DefaultCellStyle.Format = numFmt
        colDetTaxAmount.DefaultCellStyle.Format = numFmt
        colTotal.DefaultCellStyle.Format = numFmt

        ' ===== نسبة الخصم (عرض فقط) =====
        colDetDiscountPercent.DefaultCellStyle.Format = "0'%'"

    End Sub
    Private Sub dgvInvoiceDetails_CellValidating(
    sender As Object,
    e As DataGridViewCellValidatingEventArgs
) Handles dgvInvoiceDetails.CellValidating

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim dgv = dgvInvoiceDetails
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        Dim newValue As String = e.FormattedValue?.ToString().Trim()

        ' 🧠 معالجة خاصة للخصم %
        If colName = "colDetDiscountPercent" AndAlso newValue.EndsWith("%") Then
            newValue = newValue.Replace("%", "").Trim()
        End If

        ' =========================
        ' التحقق من القيم الرقمية
        ' =========================
        If colName = "colDetQty" OrElse
       colName = "colDetPieceSellPrice" OrElse
       colName = "colDetM3SellPrice" OrElse
       colName = "colDetDiscountPercent" Then

            ' الفارغ يعتبر صفر (مسموح)
            If newValue = "" Then Exit Sub

            Dim val As Decimal
            If Not Decimal.TryParse(newValue, val) Then
                MessageBox.Show(
                "القيمة المدخلة غير صحيحة",
                "تنبيه",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
                e.Cancel = True   ' 🔴 قرار فقط
                Exit Sub
            End If
        End If

        ' =========================
        ' كود الصنف
        ' =========================
        If colName = "colDetProductCode" Then

            If newValue = "" Then Exit Sub

            Dim dt As DataTable =
            TryCast(colDetProductCode.DataSource, DataTable)

            If dt Is Nothing Then Exit Sub

            Dim found() As DataRow =
            dt.Select("ProductCode = '" & newValue.Replace("'", "''") & "'")

            If found.Length = 0 Then
                MessageBox.Show(
                "كود الصنف غير موجود",
                "تنبيه",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
                e.Cancel = True   ' 🔴 قرار فقط
            End If
        End If

    End Sub
    Private Sub dgvInvoiceDetails_CellBeginEdit(
    sender As Object,
    e As DataGridViewCellCancelEventArgs
) Handles dgvInvoiceDetails.CellBeginEdit


        Dim colName As String =
        dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        If CurrentDocumentID > 0 Then

            Dim statusID As Integer = 0
            Workflow_OperationPolicyHelper.GetEntityStatusByScope(
        "SAL",
        CurrentDocumentID,
        statusID
    )

            Dim operationTypeID As Integer =
        Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("SAL")

            Dim policy As EditPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            operationTypeID,
            statusID
        )

            Select Case colName

                Case "colDetQty"
                    If Not policy.AllowEditQuantity Then
                        e.Cancel = True
                        Exit Sub
                    End If

                Case "colDetPieceSellPrice",
             "colDetDiscountPercent",
             "colDetDiscountAmount"
                    If Not policy.AllowEditCost Then
                        e.Cancel = True
                        Exit Sub
                    End If

                Case Else
                    If Not policy.AllowEditData Then
                        e.Cancel = True
                        Exit Sub
                    End If

            End Select

        End If

    End Sub

    Private Sub LoadProductsForGrid(storeID As Integer)

        Dim sql As String = "
SELECT DISTINCT
    P.ProductCode
FROM Master_Product P
INNER JOIN Inventory_Balance IB ON IB.ProductID = P.ProductID
WHERE IB.StoreID = @StoreID
AND P.IsActive = 1
ORDER BY P.ProductCode
"

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@StoreID", storeID)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)

                    ' ربط الكمبو (كود فقط)
                    colDetProductCode.DataSource = dt
                    colDetProductCode.DisplayMember = "ProductCode"
                    colDetProductCode.ValueMember = "ProductCode"

                    ' تجهيز AutoComplete
                End Using
            End Using
        End Using

    End Sub

    Private Function GetProductVariantsByCode(code As String) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT 
            ProductID,
            ProductTypeID,
            ProductName,
            StorageUnitID          
         FROM Master_Product
         WHERE ProductCode = @Code
         AND IsActive = 1"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@Code", code)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt

    End Function

    Private Sub TryResolveProduct(rowIndex As Integer)

        If IsResolvingProduct Then Exit Sub
        If rowIndex < 0 OrElse rowIndex >= dgvInvoiceDetails.Rows.Count Then Exit Sub

        Dim row As DataGridViewRow = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim codeObj = row.Cells("colDetProductCode").Value
        If codeObj Is Nothing OrElse IsDBNull(codeObj) Then Exit Sub

        Dim productCode As String = codeObj.ToString().Trim()
        If productCode = "" Then Exit Sub

        Dim variants As DataTable = GetProductVariantsByCode(productCode)
        Dim typeCol =
CType(dgvInvoiceDetails.Columns("colDetProductType"),
      DataGridViewComboBoxColumn)

        Dim oldDT As DataTable =
TryCast(typeCol.DataSource, DataTable)

        Dim oldCount As Integer = If(oldDT Is Nothing, -1, oldDT.Rows.Count)


        If variants Is Nothing OrElse variants.Rows.Count = 0 Then
            ClearProductCells(row)
            Exit Sub
        End If


        ' العمود يبقى مربوط بالمصدر الأصلي

        Dim fullDT As DataTable =
TryCast(typeCol.DataSource, DataTable)

        If fullDT Is Nothing Then Exit Sub

        ' استخراج الأنواع المسموحة من variants
        Dim allowedTypes =
variants.AsEnumerable().
Select(Function(r) CInt(r("ProductTypeID"))).
Distinct().
ToList()

        ' فلترة باستخدام DataView (بدون كسر العمود)
        Dim dv As New DataView(fullDT)

        If allowedTypes.Count > 0 Then
            dv.RowFilter =
    "ProductTypeID IN (" &
    String.Join(",", allowedTypes) &
    ")"
        Else
            dv.RowFilter = "1=0"
        End If

        ' 🔥 الفلترة على مستوى الصف فقط
        Dim cell =
CType(row.Cells("colDetProductType"),
      DataGridViewComboBoxCell)

        cell.DataSource = dv
        cell.DisplayMember = "TypeName"
        cell.ValueMember = "ProductTypeID"

        ' إذا نوع واحد اختره تلقائياً
        If allowedTypes.Count = 1 Then
            row.Cells("colDetProductType").Value = allowedTypes(0)
        End If
        Dim newDT As DataTable =
TryCast(typeCol.DataSource, DataTable)

        Dim newCount As Integer = If(newDT Is Nothing, -1, newDT.Rows.Count)

        If newDT IsNot Nothing Then
            Dim hasTypeName =
    newDT.Columns.Contains("TypeName")

        End If

        ' إذا نوع واحد اختره تلقائياً
        If variants.Rows.Count = 1 Then
            row.Cells("colDetProductType").Value =
        variants.Rows(0)("ProductTypeID")
        End If

        ' فلترة الانواع من القائمة العامة

        ' إذا نوع واحد اختره مباشرة
        If allowedTypes.Count = 1 Then
            row.Cells("colDetProductType").Value = allowedTypes(0)
        End If

        Dim typeVal = row.Cells("colDetProductType").Value
        If typeVal Is Nothing OrElse IsDBNull(typeVal) Then Exit Sub

        Dim productTypeID As Integer
        If Not Integer.TryParse(typeVal.ToString(), productTypeID) Then Exit Sub

        Dim rows() As DataRow =
        variants.Select("ProductTypeID = " & productTypeID)

        If rows.Length <> 1 Then
            ClearProductCells(row)
            Exit Sub
        End If

        Dim p As DataRow = rows(0)

        IsResolvingProduct = True

        Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        drv("ProductID") = CInt(p("ProductID"))
        drv("ProductName") = p("ProductName").ToString()
        drv("UnitID") = CInt(p("StorageUnitID"))
        drv("UnitName") = GetUnitNameByID(CInt(p("StorageUnitID")))

        row.Cells("colProductName").Value = p("ProductName").ToString()

        IsResolvingProduct = False

    End Sub
    Private Sub ClearProductCells(row As DataGridViewRow)

        row.Cells("colDetProductID").Value = DBNull.Value
        row.Cells("colProductName").Value = DBNull.Value
        row.Cells("colDetSellUnit").Value = DBNull.Value
        row.Cells("colDetPieceSellPrice").Value = DBNull.Value
        row.Tag = Nothing

    End Sub

    Private Sub dgvInvoiceDetails_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellEndEdit

        If e.RowIndex < 0 Then Exit Sub
        Dim colName = dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        If colName = "colDetProductCode" OrElse
       colName = "colDetProductType" Then

            TryResolveProduct(e.RowIndex)

        End If
        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)

        Dim m3PerPiece As Decimal = GetM3PerPiece(row)
        If m3PerPiece <= 0D Then GoTo CALC

        If colName = "colDetPieceSellPrice" Then
            ' من الحبة → م3
            Dim piecePrice As Decimal = ToDec(row.Cells("colDetPieceSellPrice").Value)
            row.Cells("colDetM3SellPrice").Value =
            Math.Round(piecePrice / m3PerPiece, 6, MidpointRounding.AwayFromZero)

        ElseIf colName = "colDetM3SellPrice" Then
            ' من م3 → الحبة
            Dim m3Price As Decimal = ToDec(row.Cells("colDetM3SellPrice").Value)
            row.Cells("colDetPieceSellPrice").Value =
            Math.Round(m3Price * m3PerPiece, 6, MidpointRounding.AwayFromZero)
        End If

CALC:
        RecalculateRow(e.RowIndex)
        RecalculateInvoiceTotals_FromGrid()

    End Sub
    Private Sub dgvInvoiceDetails_RowsAdded(
    sender As Object,
    e As DataGridViewRowsAddedEventArgs
) Handles dgvInvoiceDetails.RowsAdded

        If Not dgvInvoiceDetails.Columns.Contains("colDetTaxPercent") Then Exit Sub

        For i As Integer = 0 To e.RowCount - 1

            Dim rowIndex As Integer = e.RowIndex + i
            Dim row = dgvInvoiceDetails.Rows(rowIndex)

            If row.Cells("colDetTaxPercent").Value Is Nothing OrElse
           IsDBNull(row.Cells("colDetTaxPercent").Value) Then
                row.Cells("colDetTaxPercent").Value = 1
            End If

        Next

    End Sub
    Private Sub dgvInvoiceDetails_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String =
        dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        If colName <> "colDetProductCode" Then Exit Sub

        If dgvInvoiceDetails.Columns(colName).ReadOnly Then Exit Sub

        dgvInvoiceDetails.CurrentCell =
        dgvInvoiceDetails.Rows(e.RowIndex).Cells(e.ColumnIndex)

        dgvInvoiceDetails.BeginEdit(True)

        Dim cbo As ComboBox =
        TryCast(dgvInvoiceDetails.EditingControl, ComboBox)

        If cbo IsNot Nothing Then
            cbo.DroppedDown = True
        End If

    End Sub

    Private Sub dgvInvoiceDetails_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvInvoiceDetails.CurrentCellDirtyStateChanged

        If dgvInvoiceDetails.IsCurrentCellDirty Then

            ' لا نعمل Commit أثناء التنقل البرمجي
            If dgvInvoiceDetails.EditingControl Is Nothing Then Exit Sub

            dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

        End If

    End Sub

    Private Sub dgvInvoiceDetails_EditingControlShowing(
    sender As Object,
    e As DataGridViewEditingControlShowingEventArgs
) Handles dgvInvoiceDetails.EditingControlShowing

        RemoveHandler e.Control.KeyPress, AddressOf NumericOnly_KeyPress

        Dim colName As String =
    dgvInvoiceDetails.Columns(dgvInvoiceDetails.CurrentCell.ColumnIndex).Name

        ' =========================
        ' كود الصنف: يسمح بالكتابة + AutoComplete
        ' =========================
        If colName = "colDetProductCode" Then

            Dim cbo As ComboBox = TryCast(e.Control, ComboBox)
            If cbo IsNot Nothing Then

                ' ✅ لازم يكون DropDown (قابل للكتابة)
                cbo.DropDownStyle = ComboBoxStyle.DropDown

                ' ✅ AutoComplete من عناصر القائمة نفسها (بدون CustomSource)
                cbo.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                cbo.AutoCompleteSource = AutoCompleteSource.ListItems

            End If

            Exit Sub
        End If

        ' =========================
        ' أرقام فقط
        ' =========================
        If colName = "colDetQty" OrElse
       colName = "colDetPieceSellPrice" OrElse
       colName = "colDetM3SellPrice" Then

            Dim tb As TextBox = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                AddHandler tb.KeyPress, AddressOf NumericOnly_KeyPress
            End If

        End If

    End Sub
    Private Sub NumericOnly_KeyPress(
    sender As Object,
    e As KeyPressEventArgs
)

        ' أرقام – Backspace – فاصلة عشرية
        If Char.IsControl(e.KeyChar) Then Exit Sub

        If Char.IsDigit(e.KeyChar) Then Exit Sub

        If e.KeyChar = "."c OrElse e.KeyChar = ","c Then Exit Sub

        e.Handled = True

    End Sub

    Private Sub RecalculateRow(rowIndex As Integer)

        If rowIndex < 0 OrElse rowIndex >= dgvInvoiceDetails.Rows.Count Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        ' =========================
        ' المدخلات
        ' =========================
        Dim Quantity As Decimal = ToDec(row.Cells("colDetQty").Value)
        Dim unitPrice As Decimal = ToDec(row.Cells("colDetPieceSellPrice").Value)
        Dim DiscountRate As Decimal = ToDec(row.Cells("colDetDiscountPercent").Value)

        Dim taxTypeID As Integer = 0
        If row.Cells("colDetTaxPercent").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("colDetTaxPercent").Value) Then
            Integer.TryParse(row.Cells("colDetTaxPercent").Value.ToString(), taxTypeID)
        End If

        Dim vatPercent As Decimal = GetVatPercentByTaxTypeID(taxTypeID)

        ' =========================
        ' الحساب
        ' =========================
        Dim GrossAmount As Decimal =
        Math.Round(Quantity * unitPrice, 6, MidpointRounding.AwayFromZero)

        Dim discountAmount As Decimal =
        Math.Round(GrossAmount * DiscountRate / 100D, 6, MidpointRounding.AwayFromZero)

        Dim netAfterDiscount As Decimal =
        Math.Round(GrossAmount - discountAmount, 6, MidpointRounding.AwayFromZero)

        Dim TaxableAmount As Decimal
        Dim TaxAmount As Decimal
        Dim LineTotal As Decimal

        If chkIsIncludeVAT.Checked Then
            ' السعر شامل الضريبة: netAfterDiscount يمثل الإجمالي شامل VAT
            LineTotal = netAfterDiscount

            TaxableAmount =
            Math.Round(LineTotal / (1D + vatPercent / 100D), 6, MidpointRounding.AwayFromZero)

            TaxAmount =
            Math.Round(LineTotal - TaxableAmount, 6, MidpointRounding.AwayFromZero)
        Else
            ' السعر غير شامل الضريبة: netAfterDiscount يمثل الأساس قبل VAT
            TaxableAmount = netAfterDiscount

            TaxAmount =
            Math.Round(TaxableAmount * vatPercent / 100D, 6, MidpointRounding.AwayFromZero)

            LineTotal =
            Math.Round(TaxableAmount + TaxAmount, 6, MidpointRounding.AwayFromZero)
        End If

        Dim NetAmount As Decimal = TaxableAmount

        ' =========================
        ' تخزين النتائج
        ' =========================
        row.Cells("colDetLineAmount").Value = GrossAmount
        row.Cells("colDetDiscountAmount").Value = discountAmount
        row.Cells("colDetTaxableAmount").Value = TaxableAmount
        row.Cells("colDetTaxAmount").Value = TaxAmount
        row.Cells("colTotal").Value = LineTotal

        If dgvInvoiceDetails.Columns.Contains("colDetNetAmount") Then
            row.Cells("colDetNetAmount").Value = NetAmount
        End If

        UpdateTotalVolumeLabel()

    End Sub
    Private Sub chkIsExport_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles chkIsExport.CheckedChanged

        If InvoiceDetailsDT Is Nothing Then Exit Sub
        If InvoiceDetailsDT.Rows.Count = 0 Then Exit Sub

        ' =========================
        ' 1️⃣ إنهاء أي تحرير جاري
        ' =========================
        If dgvInvoiceDetails.IsCurrentCellInEditMode Then
            dgvInvoiceDetails.EndEdit()
        End If
        dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)

        ' =========================
        ' 2️⃣ تحديد نوع الضريبة
        ' =========================
        Dim exportTaxTypeID As Integer = 2   ' صفرية
        Dim normalTaxTypeID As Integer = 1   ' 15%
        Dim selectedTaxTypeID As Integer =
        If(chkIsExport.Checked, exportTaxTypeID, normalTaxTypeID)

        ' =========================
        ' 3️⃣ تحديث مصدر البيانات
        ' =========================
        For Each r As DataRow In InvoiceDetailsDT.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            r("TaxTypeID") = selectedTaxTypeID
        Next

        ' =========================
        ' 4️⃣ تحديث الجريد وإعادة الحساب
        ' =========================
        dgvInvoiceDetails.Refresh()

        For i As Integer = 0 To dgvInvoiceDetails.Rows.Count - 1
            If Not dgvInvoiceDetails.Rows(i).IsNewRow Then
                RecalculateRow(i)
            End If
        Next

        RecalculateInvoiceTotals_FromGrid()
        ' =========================
        ' 5️⃣ التحكم في سبب الإعفاء
        ' =========================
        If chkIsExport.Checked Then
            For Each row As DataRowView In cboTaxReason.Items
                If row("ReasonCode").ToString() = "VATEX-SA-32" Then
                    cboTaxReason.SelectedValue = row("TaxReasonID")
                    Exit For
                End If
            Next
        Else
            cboTaxReason.SelectedIndex = -1
        End If

        ' =========================
        ' 6️⃣ تحديث عرض الـ ComboBox
        ' =========================
        dgvInvoiceDetails.Invalidate()

        ' يمكن إزالة كود DIAG أو وضعه بعد التحديث الكامل
        ' MessageBox.Show("تم تحديث الضريبة لجميع السطور", "تأكيد")

    End Sub
    Private Sub dgvInvoiceDetails_RowsRemoved(
    sender As Object,
    e As DataGridViewRowsRemovedEventArgs
) Handles dgvInvoiceDetails.RowsRemoved

        '        RecalculateInvoiceTotals()

    End Sub

    Private Sub dgvInvoiceDetails_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellValueChanged
        If dgvInvoiceDetails.IsCurrentCellInEditMode Then Exit Sub

        If IsLoadingInvoiceDetails Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim colName As String =
        dgvInvoiceDetails.Columns(e.ColumnIndex).Name
        ' فقط تحديد الصنف
        '       If colName = "colDetProductCode" OrElse
        '      colName = "colDetProductType" Then
        '     TryResolveProduct(e.RowIndex)
        '    End If

    End Sub
    Private Sub dgvInvoiceDetails_RowValidated(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.RowValidated

        If IsLoadingInvoiceDetails Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        If dgvInvoiceDetails.Rows(e.RowIndex).IsNewRow Then Exit Sub

        RecalculateRow(e.RowIndex)
        RecalculateInvoiceTotals_FromGrid()

    End Sub
    Private Sub dgvInvoiceDetails_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs
) Handles dgvInvoiceDetails.DataError

        e.ThrowException = False

    End Sub
    Private Function GetDefaultStoreIDForUser() As Integer?

        ' 🔹 الآن: مؤقتًا نرجع 4
        ' 🔹 لاحقًا: نربطها بصلاحيات المستخدم

        Return 4

    End Function
    Private Sub frmInvoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' ===============================
        ' حجم الفورم 95%
        ' ===============================
        Me.StartPosition = FormStartPosition.Manual

        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea

        Me.Width = CInt(wa.Width * 0.95)
        Me.Height = CInt(wa.Height * 0.95)

        Me.Left = wa.Left + (wa.Width - Me.Width) \ 2
        Me.Top = wa.Top + (wa.Height - Me.Height) \ 2
        rbuStandard.Checked = True
        rbuDeveloper.Checked = True
        btnSaveDraft.Text = "حفظ"

        ' ======================================
        ' 🔗 ربط مصدر المستند
        ' ======================================
        If SourceLOID > 0 Then
            CurrentSourceDocumentID = SourceLOID
            CurrentSourceDocumentType = "LOADING_ORDER"
        End If

        ' ===============================
        ' تحميل البيانات أولاً
        ' ===============================
        LoadAllCombos()
        LoadTaxReasonCombo()
        ' ===============================
        ' 🔴 إضافات معمارية صحيحة
        ' ===============================
        InitInvoiceDetailsTable()
        BindInvoiceGrid()
        dgvInvoiceDetails.StandardTab = False

        ' المستودع الافتراضي
        cboSourceStoreID.SelectedValue = 4
        ChangeSourceStore(4)


        ApplyDecimalFormatting()
        If PendingSRIDs_FromLoading Is Nothing Then
            If SourceLOID > 0 AndAlso SourceSRID > 0 Then
                LoadInvoiceFromLoadingOrder(SourceLOID, SourceSRID, SourceLOModifiedAt)
            End If
        End If
        ApplyInvoiceGridColumnWidths()
        ApplyDecimalFormatting()
        InitTotalVolumeOverlay()
        ApplyInvoicePermission()
        ' ===============================
        ' ربط العرض والقيمة
        ' ===============================
        cboSourceStoreID.DisplayMember = "StoreName"
        cboSourceStoreID.ValueMember = "StoreID"

        ' لا تلمس DisplayMember هنا إطلاقًا
        cboPartnerID.ValueMember = "PartnerID"

        cboPaymentMethodID.DisplayMember = "NameAr"
        cboPaymentMethodID.ValueMember = "PaymentMethodID"

        cboPaymentTerm.DisplayMember = "NameAr"
        cboPaymentTerm.ValueMember = "PaymentTermID"

        cboStatusID.DisplayMember = "StatusName"
        cboStatusID.ValueMember = "StatusID"

        colDetProductType.DisplayMember = "TypeName"
        colDetProductType.ValueMember = "ProductTypeID"

        colDetTaxPercent.DisplayMember = "TaxName"
        colDetTaxPercent.ValueMember = "TaxTypeID"

        lblTotalVolume.ForeColor = Color.FromArgb(80, Color.Red) ' أوضح
        If PendingSRIDs_FromLoading IsNot Nothing AndAlso
   PendingSRIDs_FromLoading.Count > 0 Then

            LoadInvoiceForSR(PendingSRIDs_FromLoading(CurrentInvoiceIndex))
        End If
        ' ===============================
        ' بدء فاتورة جديدة عند فتح الفورم
        InitNewInvoice()
    End Sub

    Private Sub btnPostInvoice_Click(
    sender As Object,
    e As EventArgs
) Handles btnPostInvoice.Click

        Try
            If CurrentDocumentID <= 0 Then
                MessageBox.Show("لا توجد فاتورة محفوظة", "تنبيه")
                Exit Sub
            End If

            Dim statusID As Integer = 0
            Workflow_OperationPolicyHelper.GetEntityStatusByScope(
    "SAL",
    CurrentDocumentID,
    statusID
)

            Dim operationTypeID As Integer =
    Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("SAL")

            Dim policy As EditPolicy =
    Workflow_OperationPolicyHelper.GetEditPolicy(
        operationTypeID,
        statusID
    )

            If Not policy.IsPostable Then
                Throw New Exception("لا يسمح باعتماد هذه الفاتورة حسب سياسة النظام")
            End If

            Dim service As New InvoiceApplicationService()

            Dim isSimplified As Boolean = rbuSimplified.Checked
            Dim autoClear As Boolean = rbuSimulation.Checked Or rbuDeveloper.Checked

            service.SendAndPostInvoice(
    CurrentDocumentID,
    CurrentEmployeeID,
    isSimplified,
    autoClear
)

            Dim statusText As String = ""
            If cboStatusID.Text IsNot Nothing Then statusText = cboStatusID.Text



            LoadInvoiceDocument(CurrentDocumentID)
            ApplyInvoicePermission()
            UpdateReopenButtonVisibility()
            MessageBox.Show(
    "تم إرسال الفاتورة بنجاح." & vbCrLf &
    "الحالة الحالية: " & statusText,
    "تم",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Sub btnNewInvoice_Click(
    sender As Object,
    e As EventArgs
) Handles btnNewInvoice.Click


        ResetInvoiceHeaderAndTotals()
        InitNewInvoice()
        ApplyInvoicePermission()
    End Sub


    ' =========================
    ' Helpers
    ' =========================

    Private Sub LoadCombo(
    cbo As ComboBox,
    sql As String,
    displayMember As String,
    valueMember As String
)
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)
                cbo.DataSource = dt
                cbo.DisplayMember = displayMember
                cbo.ValueMember = valueMember
                cbo.SelectedIndex = -1
            End Using
        End Using
    End Sub


    Private Sub LoadGridCombo(
    col As DataGridViewComboBoxColumn,
    sql As String,
    displayMember As String,
    valueMember As String
)
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)

                col.DataSource = dt
                col.DisplayMember = displayMember
                col.ValueMember = valueMember
            End Using
        End Using
    End Sub
    Private Sub RecalculateInvoiceTotals_FromGrid()

        Dim subTotal As Decimal = 0D              ' GrossAmount
        Dim discountTotal As Decimal = 0D         ' DiscountAmount
        Dim taxableTotal As Decimal = 0D          ' TaxableAmount
        Dim vatTotal As Decimal = 0D              ' TaxAmount
        Dim grandTotal As Decimal = 0D            ' LineTotal

        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows

            If row.IsNewRow Then Continue For
            If row.Cells("colDetQty").Value Is Nothing Then Continue For

            Dim gross As Decimal = ToDec(row.Cells("colDetLineAmount").Value)
            Dim discount As Decimal = ToDec(row.Cells("colDetDiscountAmount").Value)
            Dim taxable As Decimal = ToDec(row.Cells("colDetTaxableAmount").Value)
            Dim vat As Decimal = ToDec(row.Cells("colDetTaxAmount").Value)
            Dim total As Decimal = ToDec(row.Cells("colTotal").Value)

            subTotal += gross
            discountTotal += discount
            taxableTotal += taxable
            vatTotal += vat
            grandTotal += total

        Next

        subTotal = Math.Round(subTotal, 6, MidpointRounding.AwayFromZero)
        discountTotal = Math.Round(discountTotal, 6, MidpointRounding.AwayFromZero)
        taxableTotal = Math.Round(taxableTotal, 6, MidpointRounding.AwayFromZero)
        vatTotal = Math.Round(vatTotal, 6, MidpointRounding.AwayFromZero)
        grandTotal = Math.Round(grandTotal, 6, MidpointRounding.AwayFromZero)

        txtTotalAmount.Text = subTotal.ToString("0.000")
        txtTotalDiscount.Text = discountTotal.ToString("0.000")
        txtTotalTaxableAmount.Text = taxableTotal.ToString("0.000")
        txtTotalTax.Text = vatTotal.ToString("0.000")
        txtGrandTotal.Text = grandTotal.ToString("0.000")

        UpdateTotalVolumeLabel()

    End Sub
    Private Sub LoadInvoiceTotals_FromDB()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    TotalNetAmount,
    TotalDiscount,
    TotalTax,
    LineTotal
FROM Inventory_DocumentHeader
WHERE DocumentID = @DocumentID
", con)

                cmd.Parameters.AddWithValue("@DocumentID", CurrentDocumentID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        txtTotalAmount.Text =
                        CDec(rd("TotalNetAmount")).ToString("0.000")

                        txtTotalDiscount.Text =
                        CDec(rd("TotalDiscount")).ToString("0.000")

                        txtTotalTaxableAmount.Text =
                        CDec(rd("TotalNetAmount")).ToString("0.000")

                        txtTotalTax.Text =
                        CDec(rd("TotalTax")).ToString("0.000")

                        txtGrandTotal.Text =
                        CDec(rd("LineTotal")).ToString("0.000")

                    End If
                End Using

            End Using
        End Using

        ' الحجم لا يُحسب بعد POST
        lblTotalVolume.Visible = False

    End Sub

    Private Function GetVatPercentByTaxTypeID(taxTypeID As Integer) As Decimal
        ' حسب نظامك الحالي:
        ' 1 = 15% ، 2 = 0%
        If taxTypeID = 1 Then Return 15D
        Return 0D
    End Function

    Private Function ToDec(val As Object) As Decimal
        If val Is Nothing Then Return 0D
        If IsDBNull(val) Then Return 0D

        Dim s As String = val.ToString()
        If String.IsNullOrWhiteSpace(s) Then Return 0D

        Dim d As Decimal = 0D
        Decimal.TryParse(s, d)
        Return d
    End Function
    Private Sub chkIsIncludeVAT_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles chkIsIncludeVAT.CheckedChanged

        ' 🔥 مهم جداً – إنهاء أي تحرير داخل الجريد
        If dgvInvoiceDetails.IsCurrentCellInEditMode Then
            dgvInvoiceDetails.EndEdit()
        End If

        dgvInvoiceDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
        dgvInvoiceDetails.Refresh()

        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows
            If row.IsNewRow Then Continue For
            RecalculateRow(row.Index)
        Next

        RecalculateInvoiceTotals_FromGrid()

    End Sub
    Private Sub InitNewInvoice()

        PendingSRIDs = Nothing
        ' 🔓 إعادة فتح الجريد بالكامل للوضع الافتراضي
        dgvInvoiceDetails.ReadOnly = False
        dgvInvoiceDetails.AllowUserToAddRows = True
        dgvInvoiceDetails.AllowUserToDeleteRows = True

        For Each col As DataGridViewColumn In dgvInvoiceDetails.Columns
            col.ReadOnly = False
        Next

        PendingSRIDs_FromLoading = Nothing
        CurrentInvoiceIndex = 0
        IsLoadingInvoiceDetails = False

        If InvoiceDetailsDT Is Nothing Then
            InitInvoiceDetailsTable()
            BindInvoiceGrid()
        Else
            InvoiceDetailsDT.Clear()
        End If

        ' 🔥 أضف صف واحد فقط
        If InvoiceDetailsDT.Rows.Count = 0 Then
            InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())

        End If

        cboStatusID.SelectedValue = 1
        ' المستودع الافتراضي
        cboSourceStoreID.SelectedValue = 4
        ChangeSourceStore(4)
        ApplyInvoicePermission()
    End Sub
    Private Sub ResetInvoiceHeaderAndTotals()

        ' ===== الهيدر =====
        cboSourceStoreID.SelectedIndex = -1
        cboPartnerID.SelectedIndex = -1
        cboPaymentMethodID.SelectedIndex = -1
        cboPaymentTerm.SelectedIndex = -1

        txtPartnerName.Clear()
        txtVATRegistrationNumber.Clear()
        txtSRCode.Clear()
        txtDriverName.Clear()
        txtVehicleCode.Clear()
        txtInvoiceNote.Clear()
        btnSaveDraft.Text = "حفظ"

        ' ===== الضريبة / التصدير =====
        chkIsIncludeVAT.Checked = False
        chkIsExport.Checked = False

        ' ===== الحسابات =====
        txtTotalAmount.Text = "0.000"
        txtTotalDiscount.Text = "0.000"
        txtTotalTaxableAmount.Text = "0.000"
        txtTotalTax.Text = "0.000"
        txtGrandTotal.Text = "0.000"

        ' ===== الحجم =====
        lblTotalVolume.Text = ""
        lblTotalVolume.Visible = False

    End Sub
    Private Sub dgvInvoiceDetails_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvInvoiceDetails.CellContentClick

        If e.RowIndex < 0 Then Exit Sub
        If dgvInvoiceDetails.Columns(e.ColumnIndex).Name <> "colDetDelete" Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(e.RowIndex)
        If row.IsNewRow Then Exit Sub

        Dim res = MessageBox.Show(
        "هل تريد حذف هذا السطر؟",
        "تأكيد الحذف",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If res <> DialogResult.Yes Then Exit Sub

        ' ✅ الحذف الصحيح مع Binding
        Dim drv = TryCast(row.DataBoundItem, DataRowView)
        If drv IsNot Nothing Then
            drv.Delete()
        End If

        ' ضمان وجود صف فارغ
        If InvoiceDetailsDT.Rows.Count = 0 Then
            InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())
        End If

        RecalculateInvoiceTotals_FromGrid()
        UpdateTotalVolumeLabel()

    End Sub
    Private Sub ApplyInvoiceGridColumnWidths()

        With dgvInvoiceDetails


            .Columns("colDetBaseProductCode").Visible = False
            .Columns("colDetProductCode").Width = 110
            .Columns("colProductName").Width = 150
            .Columns("colDetProductType").Width = 80

            .Columns("colDetQty").Width = 50
            .Columns("colDetSellUnit").Width = 50

            .Columns("colDetPieceSellPrice").Width = 50

            .Columns("colDetLineAmount").Width = 50
            .Columns("colDetDiscountPercent").Width = 50
            .Columns("colDetDiscountAmount").Width = 70

            .Columns("colDetTaxableAmount").Width = 70
            .Columns("colDetTaxPercent").Width = 100
            .Columns("colDetTaxAmount").Width = 70
            .Columns("colTotal").Width = 70

            .Columns("colDetNote").Width = 120

            Dim btnCol = TryCast(.Columns("colDetDelete"), DataGridViewButtonColumn)
            If btnCol IsNot Nothing Then
                btnCol.Width = 40
                btnCol.HeaderText = ""
                btnCol.UseColumnTextForButtonValue = True
                btnCol.Text = "✖"
            End If
        End With

    End Sub
    Private Function GetM3PerPiece(row As DataGridViewRow) As Decimal

        If row Is Nothing OrElse row.IsNewRow Then Return 0D

        Dim length As Decimal = 0D
        Dim width As Decimal = 0D
        Dim height As Decimal = 0D

        ' =========================
        ' السيناريو 1️⃣: فاتورة محمّلة من DB (Binding)
        ' =========================
        Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)

        If drv IsNot Nothing AndAlso
       drv.DataView.Table.Columns.Contains("Length") Then

            length = ToDec(drv("Length"))
            width = ToDec(drv("Width"))
            height = ToDec(drv("Height"))

        End If

        ' =========================
        ' السيناريو 2️⃣: فاتورة جديدة (إدخال يدوي – Tag)
        ' =========================
        If (length = 0D OrElse width = 0D OrElse height = 0D) AndAlso
       row.Tag IsNot Nothing Then

            Try
                Dim dims = row.Tag
                length = ToDec(dims.Length)
                width = ToDec(dims.Width)
                height = ToDec(dims.Height)
            Catch
                ' تجاهل أي كائن غير متوافق في Tag
            End Try

        End If

        ' =========================
        ' تحقق نهائي
        ' =========================
        If length <= 0D OrElse width <= 0D OrElse height <= 0D Then
            Return 0D
        End If

        ' =========================
        ' التحويل من سم³ إلى م³
        ' =========================
        Dim volumeCm3 As Decimal = length * width * height

        Return Math.Round(
        volumeCm3 / 1000000D,
        6,
        MidpointRounding.AwayFromZero
    )

    End Function
    Private Sub InitTotalVolumeOverlay()

        ' اجعل الليبل فوق الجريد
        lblTotalVolume.Parent = dgvInvoiceDetails

        ' الحجم والموقع (وسط الجريد تقريبًا)
        lblTotalVolume.Width = dgvInvoiceDetails.Width
        lblTotalVolume.Height = 120
        lblTotalVolume.Left = 0
        lblTotalVolume.Top = (dgvInvoiceDetails.Height \ 2) - 60

        ' الخط
        lblTotalVolume.Font = New Font(
        "Segoe UI",
        36,
        FontStyle.Bold
    )

        ' اللون الأحمر مع شفافية
        lblTotalVolume.ForeColor = Color.FromArgb(120, Color.Red)

        ' خلفية شفافة
        lblTotalVolume.BackColor = Color.Transparent

        ' محاذاة النص
        lblTotalVolume.TextAlign = ContentAlignment.MiddleCenter

        ' فوق كل شيء
        lblTotalVolume.BringToFront()

    End Sub
    Private Function CalculateTotalVolumeM3() As Decimal

        Dim total As Decimal = 0D

        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows

            If row.IsNewRow Then Continue For
            If row.Tag Is Nothing Then Continue For

            Dim Quantity As Decimal = ToDec(row.Cells("colDetQty").Value)
            If Quantity <= 0D Then Continue For

            Dim dims = row.Tag

            ' الحجم بالسنتيمتر المكعب
            Dim volumeCm3 As Decimal =
            CDec(dims.Length) *
            CDec(dims.Width) *
            CDec(dims.Height)

            ' التحويل إلى متر مكعب
            Dim volumeM3PerPiece As Decimal = volumeCm3 / 1000000D

            total += Quantity * volumeM3PerPiece

        Next

        Return Math.Round(total, 3, MidpointRounding.AwayFromZero)

    End Function
    Private Sub UpdateTotalVolumeLabel()

        Dim totalVolume As Decimal = CalculateTotalVolumeM3()

        If totalVolume > 0D Then
            lblTotalVolume.Text = totalVolume.ToString("0.0") & " متر مكعب"
            lblTotalVolume.Visible = True
        Else
            lblTotalVolume.Text = ""
            lblTotalVolume.Visible = False
        End If

    End Sub

    Private Sub btnImportSRWaitingInvoice_Click(
    sender As Object,
    e As EventArgs
) Handles btnImportSRWaitingInvoice.Click

        Dim f As New frmLoadingBoard()

        f.CurrentMode = frmLoadingBoard.LoadingBoardMode.InvoiceSelection
        f.IsOpenedFromInvoice = True

        f.ShowDialog()

        If f.SelectedLOID > 0 AndAlso f.SelectedSRID > 0 Then
            SourceLOID = f.SelectedLOID
            SourceSRID = f.SelectedSRID
            SourceLOModifiedAt = f.SelectedLOModifiedAt

            ' ✅ تحميل الفاتورة مع تحقق Snapshot/إلغاء
            LoadInvoiceFromLoadingOrder(SourceLOID, SourceSRID, SourceLOModifiedAt)
        End If

    End Sub
    Public Sub LoadSRsForInvoice(srIDs As List(Of Integer))

        If srIDs Is Nothing OrElse srIDs.Count = 0 Then Exit Sub

        PendingSRIDs_FromLoading = srIDs
        CurrentInvoiceIndex = 0
    End Sub

    Private Sub LoadInvoiceForSR(srID As Integer)

        If SourceLOID <= 0 Then
            Throw New Exception("LOID غير محدد للفوترة")
        End If

        Dim loID As Integer = SourceLOID
        LoadInvoiceHeader_FromDB(loID)
        LoadPartnerFromSR(srID)
        LoadInvoiceDetails_FromDB(loID, srID)

    End Sub
    Private Sub LoadPartnerFromSR(srID As Integer)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    P.PartnerID,
    P.PartnerCode,
    P.PartnerName,
    P.VATRegistrationNumber
FROM Business_SR SR
INNER JOIN Master_Partner P 
    ON P.PartnerID = SR.PartnerID
WHERE SR.SRID = @SRID
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        cboPartnerID.SelectedValue = CInt(rd("PartnerID"))

                        txtPartnerName.Text =
                        rd("PartnerName").ToString()

                        txtVATRegistrationNumber.Text =
                        If(IsDBNull(rd("VATRegistrationNumber")),
                           "",
                           rd("VATRegistrationNumber").ToString())

                    Else
                        ' أمان
                        cboPartnerID.SelectedIndex = -1
                        txtPartnerName.Clear()
                        txtVATRegistrationNumber.Clear()
                    End If
                End Using
            End Using
        End Using

    End Sub

    Private Sub ReloadPartnerFromInvoiceHeader()

        If cboPartnerID.SelectedValue IsNot Nothing Then
            cboPartnerID.SelectedValue = cboPartnerID.SelectedValue
        End If

    End Sub

    Private Sub LoadInvoiceHeader_FromDB(loID As Integer)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    SR.SRCode,
    E.EmpName,
    V.VehicleCode
FROM Logistics_LoadingOrder LO
INNER JOIN Logistics_LoadingOrderSR LOS 
    ON LOS.LOID = LO.LOID
INNER JOIN Business_SR SR 
    ON SR.SRID = LOS.SRID
LEFT JOIN Security_Employee E 
    ON E.EmployeeID = LO.DriverEmployeeID
LEFT JOIN Master_Vehicle V 
    ON V.VehicleID = LO.VehicleID
WHERE LO.LOID = @LOID
", con)


                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        txtSRCode.Text = rd("SRCode").ToString()

                        txtDriverName.Text =
                        If(IsDBNull(rd("EmpName")), "", rd("EmpName").ToString())

                        txtVehicleCode.Text =
                        If(IsDBNull(rd("VehicleCode")), "", rd("VehicleCode").ToString())

                    End If
                End Using
            End Using
        End Using

    End Sub

    Private Sub LoadInvoiceDetails_FromDB(loID As Integer, srID As Integer)

        IsLoadingInvoiceDetails = True
        If InvoiceDetailsDT Is Nothing Then
            InitInvoiceDetailsTable()
            BindInvoiceGrid()
        End If

        InvoiceDetailsDT.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    LOD.LoadingOrderDetailID,
    SRD.SRDID,
    SRD.SRID,
    SRD.ProductID,
    BP.ProductCode AS BaseProductCode,
    P.ProductCode,
    P.ProductName,
    P.ProductTypeID,
    P.StorageUnitID,
    U.UnitName,
    U.UnitCode,

    LOD.LoadedQty AS Quantity,
    ISNULL(SRD.UnitPrice, 0) AS UnitPrice,
    ISNULL(SRD.DiscountRate, 0) AS DiscountRate,
    SRD.TaxTypeID AS TaxTypeID,

    P.HasDimensions,
    P.Length,
    P.Width,
    P.Height

FROM Logistics_LoadingOrderDetail LOD

INNER JOIN Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID

INNER JOIN Master_Product P
    ON P.ProductID = SRD.ProductID

LEFT JOIN Master_Product BP
    ON BP.ProductID = P.BaseProductID

LEFT JOIN Master_Unit U
    ON U.UnitID = P.StorageUnitID

WHERE LOD.LOID = @LOID
  AND LOD.SourceHeaderID = @SRID
  AND ISNULL(LOD.LoadedQty,0) > 0

  -- ✅ الاتفاق: استبعد إذا مرتبط بأي فاتورة SAL غير ملغاة (StatusID <> 10)
  AND NOT EXISTS (
        SELECT 1
        FROM Inventory_DocumentDetails D
        INNER JOIN Inventory_DocumentHeader H
            ON H.DocumentID = D.DocumentID
        WHERE D.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
          AND H.DocumentType = 'SAL'
          AND H.StatusID <> 10
    )
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)
                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()

                        Dim dr As DataRow = InvoiceDetailsDT.NewRow()
                        dr("SourceLoadingOrderDetailID") =
                        If(IsDBNull(rd("LoadingOrderDetailID")), DBNull.Value, CInt(rd("LoadingOrderDetailID")))

                        dr("SRDID") = CInt(rd("SRDID"))
                        dr("ProductID") = CInt(rd("ProductID"))
                        dr("BaseProductCode") = If(rd("BaseProductCode") Is DBNull.Value, "", rd("BaseProductCode").ToString())
                        dr("ProductCode") = rd("ProductCode").ToString()
                        dr("ProductName") = rd("ProductName").ToString()
                        dr("ProductTypeID") = CInt(rd("ProductTypeID"))
                        dr("UnitID") = CInt(rd("StorageUnitID"))
                        dr("UnitName") = rd("UnitName").ToString()
                        dr("UnitCode") = rd("UnitCode").ToString()
                        dr("Quantity") = CDec(rd("Quantity"))
                        dr("PieceSellPrice") = CDec(rd("UnitPrice"))
                        dr("DiscountRate") = If(IsDBNull(rd("DiscountRate")), 0D, CDec(rd("DiscountRate")))
                        dr("TaxTypeID") = If(IsDBNull(rd("TaxTypeID")), 1, CInt(rd("TaxTypeID")))

                        InvoiceDetailsDT.Rows.Add(dr)

                        Dim rowIndex As Integer = InvoiceDetailsDT.Rows.Count - 1
                        If rowIndex >= 0 AndAlso rowIndex < dgvInvoiceDetails.Rows.Count Then
                            dgvInvoiceDetails.Rows(rowIndex).Tag = New With {
                            .Length = If(IsDBNull(rd("Length")), 0D, CDec(rd("Length"))),
                            .Width = If(IsDBNull(rd("Width")), 0D, CDec(rd("Width"))),
                            .Height = If(IsDBNull(rd("Height")), 0D, CDec(rd("Height")))
                        }
                        End If

                    End While
                End Using

            End Using
        End Using

        IsLoadingInvoiceDetails = False

        For i As Integer = 0 To InvoiceDetailsDT.Rows.Count - 1
            If i < dgvInvoiceDetails.Rows.Count AndAlso Not dgvInvoiceDetails.Rows(i).IsNewRow Then
                RecalculateRow(i)
            End If
        Next

        btnSaveDraft.Text = "حفظ"
        RecalculateInvoiceTotals_FromGrid()

    End Sub
    Private Sub AfterInvoiceSaved()

        CurrentInvoiceIndex += 1

        If CurrentInvoiceIndex >= PendingSRIDs.Count Then
            MessageBox.Show("تمت فوترة جميع الطلبات المختارة.", "تم")
            Me.Close()
            Exit Sub
        End If

        If MessageBox.Show(
        "تم حفظ الفاتورة." & vbCrLf &
        "هل تريد إدراج الطلب التالي؟",
        "متابعة",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    ) = DialogResult.Yes Then

            LoadInvoiceForSR(PendingSRIDs(CurrentInvoiceIndex))

        Else
            Me.Close()
        End If

    End Sub
    Private Function GetStoreIDFromSR(srID As Integer) As Integer?

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT TOP 1 LO.SourceStoreID
FROM Logistics_LoadingOrder LO
INNER JOIN Logistics_LoadingOrderSR LOS ON LOS.LOID = LO.LOID
WHERE LOS.SRID = @SRID
ORDER BY LO.LOID DESC
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Dim res = cmd.ExecuteScalar()
                If res Is Nothing OrElse IsDBNull(res) Then Return Nothing
                Return CInt(res)
            End Using
        End Using

    End Function

    Private Sub cboSourceStoreID_SelectionChangeCommitted(
    sender As Object,
    e As EventArgs
) Handles cboSourceStoreID.SelectionChangeCommitted

        If cboSourceStoreID.SelectedValue Is Nothing Then Exit Sub
        If Not IsNumeric(cboSourceStoreID.SelectedValue) Then Exit Sub

        Dim newStoreID As Integer = CInt(cboSourceStoreID.SelectedValue)

        ' 🔴 تحذير عند وجود تفاصيل
        If InvoiceDetailsDT IsNot Nothing AndAlso
       InvoiceDetailsDT.Rows.Count > 0 AndAlso
       InvoiceDetailsDT.AsEnumerable().
       Any(Function(r) r.RowState <> DataRowState.Deleted AndAlso
                       (Not IsDBNull(r("ProductID")))) Then

            Dim res = MessageBox.Show(
            "تغيير المستودع سوف يؤدي إلى مسح تفاصيل الفاتورة." &
            vbCrLf & "هل أنت متأكد؟",
            "تحذير",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

            If res = DialogResult.No Then
                If LastStoreID.HasValue Then
                    cboSourceStoreID.SelectedValue = LastStoreID.Value
                End If
                Exit Sub
            End If

            ' 🧹 مسح التفاصيل
            InvoiceDetailsDT.Clear()
            InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())
            RecalculateInvoiceTotals_FromGrid()

        End If

        ChangeSourceStore(newStoreID)

    End Sub
    ' ===== هذه هي نقطة الدخول الوحيدة من LoadingBoard =====
    ' =========================
    ' Entry point from LoadingBoard
    ' =========================
    Private Sub ChangeSourceStore(storeID As Integer)

        If LastStoreID.HasValue AndAlso LastStoreID.Value = storeID Then Exit Sub

        LastStoreID = storeID

        LoadProductsForGrid(storeID)
    End Sub
    ' =========================
    ' Navigation (Forward / Reverse)
    ' =========================



    Private Sub MoveToNextRowProductCode(currentRowIndex As Integer)

        ' 🔒 منع الإضافة المكررة
        If IsAutoAddingRow Then Exit Sub
        IsAutoAddingRow = True

        Try

            Dim currentRow = dgvInvoiceDetails.Rows(currentRowIndex)

            ' ❌ لا ننشئ سطر جديد إلا إذا السطر الحالي مكتمل
            If Not IsInvoiceRowComplete(currentRow) Then
                dgvInvoiceDetails.CurrentCell =
                currentRow.Cells("colDetProductCode")
                dgvInvoiceDetails.BeginEdit(True)
                Exit Sub
            End If

            Dim nextRowIndex As Integer = currentRowIndex + 1

            ' ✅ أضف فقط إذا فعلاً لا يوجد سطر بيانات بعده
            If nextRowIndex >= InvoiceDetailsDT.Rows.Count Then
                InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())

            End If

            dgvInvoiceDetails.CurrentCell =
    dgvInvoiceDetails.Rows(nextRowIndex).Cells("colDetProductCode")

        Finally
            IsAutoAddingRow = False
        End Try

    End Sub
    Private Function IsInvoiceRowComplete(row As DataGridViewRow) As Boolean

        If row Is Nothing OrElse row.IsNewRow Then Return False

        ' لازم يكون عندنا ProductID
        Dim productID As Integer = 0
        If row.Cells("colDetProductID").Value Is Nothing OrElse
       Not Integer.TryParse(row.Cells("colDetProductID").Value.ToString(), productID) Then
            Return False
        End If

        ' ولازم كمية > 0
        Dim Quantity As Decimal = 0D
        Decimal.TryParse(row.Cells("colDetQty").Value?.ToString(), Quantity)
        If Quantity <= 0D Then Return False

        Return True

    End Function
    Private Sub EnsureInvoiceContextDefaults()

        ' نوع العملية: بيع
        If String.IsNullOrWhiteSpace(CurrentTransactionTypeCode) Then
            CurrentTransactionTypeCode = "SAL"
        End If

        ' مصدر العملية
        If String.IsNullOrWhiteSpace(CurrentSourceType) Then
            CurrentSourceType = "LOD"
        End If

        ' تاريخ المستند (من UI)
        If dtpDocumentDate IsNot Nothing Then
            CurrentInvoiceDate = dtpDocumentDate.Value
        End If

        ' تاريخ الإصدار (مبدئيًا نفس تاريخ المستند)
        If CurrentIssueDateTime = Date.MinValue Then
            CurrentIssueDateTime = CurrentInvoiceDate
        End If

    End Sub
    Private Function ValidateInvoiceDraft() As Boolean
        If cboPartnerID.SelectedValue Is Nothing Then
            MessageBox.Show("يرجى اختيار العميل")
            Return False
        End If


        If cboPaymentMethodID.SelectedValue Is Nothing OrElse
   Not IsNumeric(cboPaymentMethodID.SelectedValue) Then

            MessageBox.Show("يرجى اختيار طريقة الدفع", "تنبيه")
            cboPaymentMethodID.Focus()
            Return False
        End If
        If cboPaymentTerm.SelectedValue Is Nothing OrElse
   Not IsNumeric(cboPaymentTerm.SelectedValue) Then

            MessageBox.Show("يرجى اختيار طبيعة الدفع", "تنبيه")
            cboPaymentMethodID.Focus()
            Return False
        End If

        If cboSourceStoreID.SelectedValue Is Nothing Then
            MessageBox.Show("يرجى اختيار المستودع")
            Return False
        End If

        If InvoiceDetailsDT.Rows.Count = 0 Then
            MessageBox.Show("لا يوجد أصناف في الفاتورة")
            Return False
        End If

        Dim hasValidRow =
        InvoiceDetailsDT.Rows.Cast(Of DataRow)().
        Any(Function(r) r.RowState <> DataRowState.Deleted AndAlso
                         CDec(r("Quantity")) > 0)

        If Not hasValidRow Then
            MessageBox.Show("الفاتورة لا تحتوي على كميات صحيحة")
            Return False
        End If

        Return True

    End Function
    Private Function BuildInvoiceHeader(details As DataTable) As InvoiceHeaderInput

        Dim header As New InvoiceHeaderInput()

        ' ======================================
        ' 1) Basic Info
        ' ======================================
        header.DocumentType = "SAL"
        header.DocumentNo = Guid.NewGuid().ToString().Substring(0, 20)
        header.DocumentDate = dtpDocumentDate.Value.Date
        header.DueDate = dtpDocumentDate.Value.Date

        ' ======================================
        ' 2) Partner
        ' ======================================
        If cboPartnerID.SelectedValue Is Nothing OrElse cboPartnerID.SelectedIndex = -1 Then
            header.PartnerID = Nothing
        Else
            header.PartnerID = CInt(cboPartnerID.SelectedValue)
        End If

        ' ======================================
        ' 3) Currency
        ' ======================================
        header.CurrencyID = 1
        header.ExchangeRate = 1D

        ' ======================================
        ' 4) Tax
        ' ======================================
        header.TaxReasonID = Nothing
        header.TaxTypeID = 0

        If details IsNot Nothing AndAlso details.Rows.Count > 0 Then
            header.TaxTypeID = CInt(details.Rows(0)("TaxTypeID"))
        End If

        header.IsTaxInclusive = chkIsIncludeVAT.Checked

        ' ======================================
        ' 5) Totals (Strict مطابق لأعمدة الجدول)
        ' ======================================
        Dim totalAmount As Decimal = 0D          ' GrossAmount
        Dim totalDiscount As Decimal = 0D        ' DiscountAmount
        Dim totalTaxable As Decimal = 0D         ' TaxableAmount
        Dim totalTax As Decimal = 0D             ' TaxAmount
        Dim grandTotal As Decimal = 0D           ' LineTotal

        If details IsNot Nothing Then
            For Each r As DataRow In details.Rows

                totalAmount += CDec(r("GrossAmount"))
                totalDiscount += CDec(r("DiscountAmount"))
                totalTaxable += CDec(r("TaxableAmount"))
                totalTax += CDec(r("TaxAmount"))
                grandTotal += CDec(r("LineTotal"))

            Next
        End If

        header.TotalAmount = Math.Round(totalAmount, 4)
        header.TotalDiscount = Math.Round(totalDiscount, 4)
        header.TotalTaxableAmount = Math.Round(totalTaxable, 4)
        header.TotalTax = Math.Round(totalTax, 4)
        header.GrandTotal = Math.Round(grandTotal, 4)

        header.PaidAmount = 0D
        header.RemainingAmount = header.GrandTotal

        ' ======================================
        ' 6) Delivery Info
        ' ======================================
        header.DeliveryDate = Nothing

        If Not String.IsNullOrWhiteSpace(txtDriverName.Text) Then
            header.DriverName = txtDriverName.Text.Trim()
        End If

        If Not String.IsNullOrWhiteSpace(txtVehicleCode.Text) Then
            header.VehicleNo = txtVehicleCode.Text.Trim()
        End If

        ' ======================================
        ' 7) Notes
        ' ======================================
        header.Notes = txtInvoiceNote.Text.Trim()

        ' ======================================
        ' 8) Payment
        ' ======================================
        If cboPaymentMethodID.SelectedValue IsNot Nothing AndAlso
       Not IsDBNull(cboPaymentMethodID.SelectedValue) Then
            header.PaymentMethodID = CInt(cboPaymentMethodID.SelectedValue)
        End If

        If cboPaymentTerm.SelectedValue IsNot Nothing AndAlso
       Not IsDBNull(cboPaymentTerm.SelectedValue) Then
            header.PaymentTermID = CInt(cboPaymentTerm.SelectedValue)
        End If

        ' ======================================
        ' 9) Status
        ' ======================================
        header.StatusID =
        Workflow_OperationPolicyHelper.GetInitialStatusByScope("SAL")

        ' ======================================
        ' 10) Inventory Flags
        ' ======================================
        header.IsOutbound = True
        header.IsInventoryPosted = False
        header.IsZatcaReported = False

        ' ======================================
        ' 11) System Fields
        ' ======================================
        header.CreatedAt = DateTime.Now

        If CurrentSourceDocumentID > 0 Then
            header.SourceDocumentID = CurrentSourceDocumentID
            header.SourceDocumentType = CurrentSourceDocumentType
        End If

        Return header

    End Function
    Private Sub btnCancel_Click(
    sender As Object,
    e As EventArgs
) Handles btnCancel.Click

        Try
            If CurrentDocumentID <= 0 Then
                MessageBox.Show("لا توجد فاتورة", "تنبيه")
                Exit Sub
            End If

            ' =========================
            ' 1) جلب الحالة الحالية
            ' =========================
            Dim statusID As Integer = 0
            If cboStatusID.SelectedValue IsNot Nothing AndAlso IsNumeric(cboStatusID.SelectedValue) Then
                statusID = CInt(cboStatusID.SelectedValue)
            Else
                Workflow_OperationPolicyHelper.GetEntityStatusByScope("SAL", CurrentDocumentID, statusID)
            End If

            ' =========================
            ' 2) السياسة الجديدة
            ' =========================
            ' 1  = DRAFT              => حذف كامل
            ' 2  = NEW / 16 NOT_SENT  => تحويل إلى CANCELLED (10)
            ' 17 REPORTED وما بعدها   => ممنوع (البديل: مرتجع)
            Dim actionName As String = ""
            Dim allow As Boolean = True

            Select Case statusID
                Case 1
                    actionName = "حذف المسودة"
                Case 2, 16
                    actionName = "إلغاء الفاتورة (تحويلها إلى ملغي)"
                Case 17, 18, 19
                    allow = False
                Case Else
                    ' الحالات الأخرى: نلتزم بسياسة النظام (لكن التنفيذ يكون Cancel وليس Delete)
                    Dim opTypeID As Integer = Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("SAL")
                    Dim policy As EditPolicy = Workflow_OperationPolicyHelper.GetEditPolicy(opTypeID, statusID)
                    If Not policy.IsCancelable Then
                        allow = False
                    Else
                        actionName = "إلغاء الفاتورة (تحويلها إلى ملغي)"
                    End If
            End Select

            If Not allow Then
                MessageBox.Show(
                "لا يمكن إلغاء فاتورة مرسلة للهيئة أو ما بعدها." & vbCrLf &
                "الإجراء الصحيح: عمل مرتجع على الفاتورة.",
                "غير مسموح",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
                Exit Sub
            End If

            If MessageBox.Show(
            "هل أنت متأكد من " & actionName & "؟",
            "تأكيد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        ) <> DialogResult.Yes Then Exit Sub

            Dim service As New InvoiceApplicationService()
            Dim oldDocumentID As Integer = CurrentDocumentID

            ' =========================
            ' 3) تنفيذ (Delete draft) أو (Cancel)
            ' =========================
            Using con As New SqlConnection(ConnStr)
                con.Open()

                Using tran = con.BeginTransaction()
                    Try
                        If statusID = 1 Then
                            ' ---------------------------------
                            ' DRAFT => حذف كامل
                            ' ملاحظة مهمة:
                            ' - لا نحذف Document_Link هنا قبل إعادة الحساب لأن الخدمة تحتاجه لاستخراج LOID.
                            ' - سنحذفه بعد إعادة الحساب خارج الترانزاكشن.
                            ' ---------------------------------
                            Using cmdDet As New SqlCommand("
DELETE FROM Inventory_DocumentDetails
WHERE DocumentID = @ID
", con, tran)
                                cmdDet.Parameters.AddWithValue("@ID", oldDocumentID)
                                cmdDet.ExecuteNonQuery()
                            End Using

                            Using cmdHdr As New SqlCommand("
DELETE FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)
                                cmdHdr.Parameters.AddWithValue("@ID", oldDocumentID)
                                cmdHdr.ExecuteNonQuery()
                            End Using

                        Else
                            ' ---------------------------------
                            ' NEW/NOT_SENT/مسموح => تحويل إلى CANCELLED (10)
                            ' لا نحذف التفاصيل ولا الروابط
                            ' ---------------------------------
                            Using cmdCancel As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)
                                cmdCancel.Parameters.AddWithValue("@ID", oldDocumentID)
                                cmdCancel.ExecuteNonQuery()
                            End Using
                        End If

                        tran.Commit()

                    Catch
                        Try : tran.Rollback() : Catch : End Try
                        Throw
                    End Try
                End Using
            End Using

            ' =========================
            ' 4) إعادة احتساب حالة LO/LOD
            ' =========================
            ' مهم: بعد تعديل الخدمة (إزالة حذف Document_Link) ستنجح في الحذف والإلغاء.
            Try
                service.RecalculateLOStatusAfterInvoiceDelete(oldDocumentID, CurrentEmployeeID)
            Catch ex As Exception
                ' لا نوقف الإجراء بسبب إعادة الحساب، لكن نُظهر تحذير
                MessageBox.Show("تم تنفيذ العملية، لكن فشل تحديث حالة أمر التحميل: " & ex.Message, "تحذير")
            End Try

            ' =========================
            ' 5) تنظيف Document_Link فقط في حالة الحذف (Draft)
            ' =========================
            If statusID = 1 Then
                Using con As New SqlConnection(ConnStr)
                    con.Open()
                    Using cmdLink As New SqlCommand("
DELETE FROM Document_Link
WHERE TargetDocumentID = @ID
  AND TargetType = 'SAL'
", con)
                        cmdLink.Parameters.AddWithValue("@ID", oldDocumentID)
                        cmdLink.ExecuteNonQuery()
                    End Using
                End Using
            End If

            MessageBox.Show("تم تنفيذ العملية بنجاح", "تم")

            ' =========================
            ' 6) تحديث الشاشة
            ' =========================
            CurrentDocumentID = 0
            ResetInvoiceHeaderAndTotals()
            InitNewInvoice()
            ApplyInvoicePermission()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Sub btnSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnSearch.Click

        Dim f As New frmInvoiceSearch()


        f.ShowDialog()

        If f.SelectedDocumentID > 0 Then

            LoadInvoiceDocument(f.SelectedDocumentID)

            CurrentDocumentID = f.SelectedDocumentID

            ApplyInvoicePermission()


        End If



    End Sub
    Private Sub LoadInvoiceDocument(documentID As Integer)

        If documentID <= 0 Then Exit Sub

        IsLoadingInvoiceDetails = True

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =========================================================
            ' 1️⃣ تحميل الهيدر
            ' =========================================================
            Using cmd As New SqlCommand("
SELECT
    H.*,
    P.PartnerName,
    P.VATRegistrationNumber AS TaxNumber,
    LO.LOCode,
    SR.SRCode
FROM Inventory_DocumentHeader H
LEFT JOIN Master_Partner P
    ON P.PartnerID = H.PartnerID
LEFT JOIN Document_Link L
    ON L.TargetDocumentID = H.DocumentID
    AND L.TargetType = 'SAL'
LEFT JOIN Logistics_LoadingOrder LO
    ON LO.LOID = L.SourceDocumentID
LEFT JOIN Logistics_LoadingOrderSR LOSR
    ON LOSR.LOID = LO.LOID
LEFT JOIN Business_SR SR
    ON SR.SRID = LOSR.SRID
WHERE H.DocumentID = @DocumentID
", con)

                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        CurrentDocumentID = CInt(rd("DocumentID"))

                        txtInvoicCode.Text = rd("DocumentNo").ToString()
                        dtpDocumentDate.Value = CDate(rd("DocumentDate"))

                        cboPartnerID.SelectedValue = rd("PartnerID")
                        cboPaymentMethodID.SelectedValue = rd("PaymentMethodID")
                        cboPaymentTerm.SelectedValue = rd("PaymentTermID")

                        ' ✅ تحميل الحالة الحقيقية من الفاتورة
                        cboStatusID.SelectedValue = CInt(rd("StatusID"))

                        chkIsIncludeVAT.Checked = CBool(rd("IsTaxInclusive"))

                        txtInvoiceNote.Text =
                        If(IsDBNull(rd("Notes")), "", rd("Notes").ToString())

                        txtDriverName.Text =
                        If(IsDBNull(rd("DriverName")), "", rd("DriverName").ToString())

                        txtPartnerName.Text =
                        If(IsDBNull(rd("PartnerName")), "", rd("PartnerName").ToString())

                        txtVehicleCode.Text =
                        If(IsDBNull(rd("VehicleNo")), "", rd("VehicleNo").ToString())

                        If Not IsDBNull(rd("TaxReasonID")) Then
                            cboTaxReason.SelectedValue = rd("TaxReasonID")
                        End If

                        If Not IsDBNull(rd("SRCode")) Then
                            txtSRCode.Text = rd("SRCode").ToString()
                        End If

                        If Not IsDBNull(rd("TaxNumber")) Then
                            txtVATRegistrationNumber.Text = rd("TaxNumber").ToString()
                        End If

                        ' ✅ تحميل الإجماليات الصحيحة من الهيدر
                        txtTotalAmount.Text = rd("TotalAmount").ToString()
                        txtTotalDiscount.Text = rd("TotalDiscount").ToString()
                        txtTotalTaxableAmount.Text = rd("TotalTaxableAmount").ToString()
                        txtTotalTax.Text = rd("TotalTax").ToString()
                        txtGrandTotal.Text = rd("GrandTotal").ToString()

                    End If
                End Using
            End Using

            ' =========================================================
            ' 2️⃣ تحميل التفاصيل
            ' =========================================================
            InvoiceDetailsDT.Clear()

            Using cmd As New SqlCommand("
SELECT
    D.*,
    P.ProductCode,
    P.ProductName,
    P.ProductTypeID,
    P.Length,
    P.Width,
    P.Height,
    U.UnitName,
    U.UnitCode
FROM Inventory_DocumentDetails D
INNER JOIN Master_Product P
    ON P.ProductID = D.ProductID
LEFT JOIN Master_Unit U
    ON U.UnitID = D.UnitID
WHERE D.DocumentID = @DocumentID
", con)

                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                Using rd = cmd.ExecuteReader()

                    While rd.Read()

                        Dim dr As DataRow = InvoiceDetailsDT.NewRow()

                        dr("ProductID") = rd("ProductID")
                        dr("ProductCode") = rd("ProductCode")
                        dr("ProductName") = rd("ProductName")
                        dr("ProductTypeID") = rd("ProductTypeID")

                        dr("UnitID") = rd("UnitID")
                        dr("UnitCode") = rd("UnitCode")
                        dr("UnitName") = rd("UnitName")

                        dr("Quantity") = rd("Quantity")
                        dr("PieceSellPrice") = rd("UnitPrice")
                        dr("GrossAmount") = rd("GrossAmount")
                        dr("DiscountRate") = rd("DiscountRate")
                        dr("DiscountAmount") = rd("DiscountAmount")
                        dr("TaxRate") = rd("TaxRate")
                        dr("TaxAmount") = rd("TaxAmount")
                        dr("NetAmount") = rd("NetAmount")
                        dr("LineTotal") = rd("LineTotal")
                        dr("TaxTypeID") = rd("TaxTypeID")

                        ' ✅ دعم المرتجعات مستقبلاً
                        If Not IsDBNull(rd("SourceLoadingOrderDetailID")) Then
                            dr("SourceLoadingOrderDetailID") =
                            rd("SourceLoadingOrderDetailID")
                        Else
                            dr("SourceLoadingOrderDetailID") = DBNull.Value
                        End If


                        If Not IsDBNull(rd("SourceStoreID")) Then
                            dr("SourceStoreID") = rd("SourceStoreID")
                        Else
                            dr("SourceStoreID") = DBNull.Value
                        End If

                        InvoiceDetailsDT.Rows.Add(dr)

                    End While

                End Using
            End Using

            ' =========================================================
            ' 3️⃣ ضبط المخزن من أول سطر
            ' =========================================================
            If InvoiceDetailsDT.Rows.Count > 0 Then

                If Not IsDBNull(InvoiceDetailsDT.Rows(0)("SourceStoreID")) Then

                    Dim storeID As Integer =
                    CInt(InvoiceDetailsDT.Rows(0)("SourceStoreID"))

                    cboSourceStoreID.SelectedValue = storeID

                    ' تحميل منتجات هذا المخزن
                    ChangeSourceStore(storeID)

                End If

            End If

        End Using
        UpdateReopenButtonVisibility()
        IsLoadingInvoiceDetails = False

        btnSaveDraft.Text = "تعديل"
        UpdateReopenButtonVisibility()
    End Sub

    ' =========================
    ' FORM: frmInvoice
    ' =========================

    Private Sub btnSaveDraft_Click(
    sender As Object,
    e As EventArgs
) Handles btnSaveDraft.Click

        Try
            EnsureInvoiceContextDefaults()

            If Not ValidateInvoiceDraft() Then Exit Sub
            Dim statusID As Integer

            If CurrentDocumentID <= 0 Then
                statusID =
        Workflow_OperationPolicyHelper.GetInitialStatusByScope("SAL")
            Else
                Workflow_OperationPolicyHelper.GetEntityStatusByScope(
        "SAL",
        CurrentDocumentID,
        statusID
    )
            End If

            Dim operationTypeID As Integer =
    Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("SAL")

            Dim policy As EditPolicy =
    Workflow_OperationPolicyHelper.GetEditPolicy(
        operationTypeID,
        statusID
    )

            If policy.Mode = EditMode.None Then
                Throw New Exception("لا يسمح بتعديل هذه الفاتورة حسب سياسة النظام")
            End If

            Dim details As DataTable = GetInvoiceDetailsTable()
            Dim header As InvoiceHeaderInput =
            BuildInvoiceHeader(details)

            Dim service As New InvoiceApplicationService()

            ' ======================================
            ' منع تعدد أنواع الضريبة
            ' ======================================
            Dim distinctTaxTypes =
            details.AsEnumerable().
            Select(Function(r) r.Field(Of Integer)("TaxTypeID")).
            Distinct().
            ToList()

            If distinctTaxTypes.Count > 1 Then
                Throw New Exception("لا يمكن وجود أكثر من نوع ضريبة في نفس الفاتورة")
            End If

            header.TaxTypeID = distinctTaxTypes(0)

            Dim documentID As Integer

            ' ======================================
            ' 🔁 تحديد نوع العملية
            ' ======================================
            If CurrentDocumentID > 0 Then

                ' تعديل
                header.DocumentID = CurrentDocumentID

                documentID = service.UpdateInvoiceDraft(
                header,
                details,
                CurrentEmployeeID
            )

                MessageBox.Show("تم تحديث الفاتورة بنجاح", "نجاح",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            Else
                ' بعد الحفظ تتحول إلى NEW (2)
                header.StatusID = 2

                ' إنشاء جديد
                documentID = service.SaveInvoiceDraft(
                header,
                details,
                CurrentEmployeeID
            )

                CurrentDocumentID = documentID
                LoadInvoiceDocument(documentID)
                ApplyInvoicePermission()
                MessageBox.Show("تم حفظ الفاتورة كمسودة", "نجاح",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            End If

        Catch ex As Exception

            MessageBox.Show(
            ex.Message,
            "خطأ",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )

        End Try

    End Sub
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' تحميل مسودة فاتورة محفوظة

    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' تحميل مسودة فاتورة محفوظة

    ' FORM: frmInvoice
    ' =========================
    ' التحقق قبل اعتماد الفاتورة

    Private Function ValidateInvoiceBeforePost() As Boolean

        If CurrentDocumentID <= 0 Then
            MessageBox.Show("لا توجد فاتورة محفوظة")
            Return False
        End If

        If cboStatusID.SelectedValue Is Nothing Then
            MessageBox.Show("حالة الفاتورة غير محددة")
            Return False
        End If

        Return True

    End Function
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' اعتماد الفاتورة (Post)

    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' إلغاء الفاتورة (Draft فقط)

    Private Sub btnDelete_Click(
    sender As Object,
    e As EventArgs
) Handles btnCancel.Click

    End Sub
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' طباعة الفاتورة (QR + Hash جاهزين)

    Private Sub btnPrint_Click(
    sender As Object,
    e As EventArgs
) Handles btnPrint.Click

        If CurrentDocumentID <= 0 Then Exit Sub

        Dim service As New InvoiceApplicationService()
        '     Dim zatcaDT As DataTable =
        '    service.PrepareZATCAReport(CurrentTransactionID)

        '    If zatcaDT.Rows.Count = 0 Then
        '   MessageBox.Show("لا توجد بيانات طباعة")
        '  Exit Sub
        ' End If

        '  Dim f As New frmInvoicePrint()
        ' f.LoadInvoice(CurrentTransactionID, zatcaDT)
        'f.ShowDialog()

    End Sub
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' زر فحص جاهزية الهيئة (قبل الإرسال)

    Private Sub btnCheckZATCA_Click(
    sender As Object,
    e As EventArgs
)

        If CurrentDocumentID <= 0 Then Exit Sub

        Dim service As New InvoiceApplicationService()

        '        If Not service.ValidateInvoiceChain(CurrentTransactionID) Then
        '       MessageBox.Show("تسلسل الفاتورة غير صالح", "تحذير")
        '      Exit Sub
        '     End If

        MessageBox.Show("الفاتورة جاهزة للإبلاغ للهيئة ✔", "ZATCA")

    End Sub
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' قفل الفاتورة بعد POST (منع أي تعديل)

    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' استدعاء القفل مباشرة بعد POST

    Private Sub AfterInvoicePosted()


        MessageBox.Show(
        "تم اعتماد الفاتورة ولا يمكن تعديلها",
        "معلومة",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
    )

    End Sub

    Private Sub ApplyInvoicePermission()

        Dim statusID As Integer

        If CurrentDocumentID <= 0 Then
            ' 🟢 فاتورة جديدة → الحالة الابتدائية
            statusID =
            Workflow_OperationPolicyHelper.GetInitialStatusByScope("SAL")
        Else
            ' 🔵 فاتورة محفوظة → اقرأ من DB
            Workflow_OperationPolicyHelper.GetEntityStatusByScope(
            "SAL",
            CurrentDocumentID,
            statusID
        )
        End If

        Dim operationTypeID As Integer =
        Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("SAL")

        Dim policy As EditPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            operationTypeID,
            statusID
        )

        btnPostInvoice.Enabled = policy.IsPostable
        btnCancel.Enabled = policy.IsCancelable
        btnSaveDraft.Enabled = (policy.Mode <> EditMode.None)

        dgvInvoiceDetails.AllowUserToAddRows = policy.AllowEditQuantity
        dgvInvoiceDetails.AllowUserToDeleteRows = policy.IsCancelable

        cboPartnerID.Enabled = policy.AllowEditData
        cboPaymentMethodID.Enabled = policy.AllowEditData
        cboPaymentTerm.Enabled = policy.AllowEditData
        txtInvoiceNote.ReadOnly = Not policy.AllowEditData

        cboStatusID.SelectedValue = statusID
        UpdateReopenButtonVisibility()
        dgvInvoiceDetails.Refresh()
        colDetSellUnit.ReadOnly = True
        colDetTaxPercent.ReadOnly = True
        colProductName.ReadOnly = True
        colDetLineAmount.ReadOnly = True
        colDetTaxableAmount.ReadOnly = True
        colDetTaxAmount.ReadOnly = True
        colTotal.ReadOnly = True
        colDetQty.ReadOnly = True





    End Sub
    ' =========================
    ' FORM: frmInvoice
    ' =========================
    ' استدعاء الصلاحية بعد Load Draft

    Private Sub LoadInvoiceDraftCompleted()

        ApplyInvoicePermission()

    End Sub
    Private Function GetInvoiceDetailsTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("SourceLoadingOrderDetailID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("Quantity", GetType(Decimal))
        dt.Columns.Add("UnitPrice", GetType(Decimal))
        dt.Columns.Add("GrossAmount", GetType(Decimal))
        dt.Columns.Add("DiscountRate", GetType(Decimal))
        dt.Columns.Add("DiscountAmount", GetType(Decimal))
        dt.Columns.Add("NetAmount", GetType(Decimal))
        dt.Columns.Add("TaxRate", GetType(Decimal))
        dt.Columns.Add("TaxAmount", GetType(Decimal))
        dt.Columns.Add("TaxableAmount", GetType(Decimal))   ' 🔥 مهم
        dt.Columns.Add("LineTotal", GetType(Decimal))
        dt.Columns.Add("SourceStoreID", GetType(Integer))
        dt.Columns.Add("UnitID", GetType(Integer))
        dt.Columns.Add("UnitCode", GetType(String))
        dt.Columns.Add("TaxTypeID", GetType(Integer))
        dt.Columns.Add("ClassifiedTaxCategory", GetType(String))
        dt.Columns.Add("TaxExemptionReasonCode", GetType(String))
        dt.Columns.Add("TaxExemptionReasonText", GetType(String))

        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows

            If row.IsNewRow Then Continue For

            Dim dr = dt.NewRow()

            ' Source LOD
            If dgvInvoiceDetails.Columns.Contains("colSourceLODDetailID") AndAlso
           row.Cells("colSourceLODDetailID").Value IsNot Nothing AndAlso
           Not IsDBNull(row.Cells("colSourceLODDetailID").Value) Then

                dr("SourceLoadingOrderDetailID") =
                CInt(row.Cells("colSourceLODDetailID").Value)
            Else
                dr("SourceLoadingOrderDetailID") = DBNull.Value
            End If

            Dim taxTypeID As Integer =
            CInt(row.Cells("colDetTaxPercent").Value)

            Dim taxRate As Decimal =
            GetVatPercentByTaxTypeID(taxTypeID)

            dr("ProductID") = CInt(row.Cells("colDetProductID").Value)
            dr("Quantity") = CDec(row.Cells("colDetQty").Value)
            dr("UnitPrice") = CDec(row.Cells("colDetPieceSellPrice").Value)
            dr("GrossAmount") = CDec(row.Cells("colDetLineAmount").Value)
            dr("DiscountRate") = CDec(row.Cells("colDetDiscountPercent").Value)
            dr("DiscountAmount") = CDec(row.Cells("colDetDiscountAmount").Value)
            dr("NetAmount") = CDec(row.Cells("colDetTaxableAmount").Value)
            dr("TaxRate") = taxRate
            dr("TaxAmount") = CDec(row.Cells("colDetTaxAmount").Value)

            ' 🔥 هذا هو المطلوب
            dr("TaxableAmount") = CDec(row.Cells("colDetTaxableAmount").Value)

            dr("LineTotal") = CDec(row.Cells("colTotal").Value)
            dr("SourceStoreID") = CInt(cboSourceStoreID.SelectedValue)
            dr("TaxTypeID") = taxTypeID

            dr("UnitID") = CInt(row.Cells("colDetUnitID").Value)
            dr("UnitCode") = row.Cells("colDetSellUnit").Value.ToString()

            ' ZATCA
            If taxTypeID = 1 Then
                dr("ClassifiedTaxCategory") = "S"
                dr("TaxExemptionReasonCode") = DBNull.Value
                dr("TaxExemptionReasonText") = DBNull.Value
            ElseIf taxTypeID = 2 Then
                dr("ClassifiedTaxCategory") = "Z"
                dr("TaxExemptionReasonCode") = "VATEX-SA-32"
                dr("TaxExemptionReasonText") = "Export of goods"
            End If

            dt.Rows.Add(dr)

        Next

        Return dt

    End Function

    Private Function ResolveInvoiceType() As String

        If rbuStandard.Checked Then
            Return "STANDARD"
        End If

        If rbuSimplified.Checked Then
            Return "SIMPLIFIED"
        End If

        Throw New Exception("يجب اختيار نوع الفاتورة (Standard أو Simplified)")

    End Function
    Private Function ResolveEnvironmentMode() As String

        If rbuProduction.Checked Then
            Return "PRODUCTION"
        End If

        If rbuSimulation.Checked Then
            Return "SIMULATION"
        End If

        If rbuDeveloper.Checked Then
            Return "DEVELOPER"
        End If

        Throw New Exception("يجب اختيار وضع التشغيل (Production / Simulation / Developer)")

    End Function
    Private Sub rbuStandard_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rbuStandard.CheckedChanged

        If rbuStandard.Checked Then
            CurrentInvoiceType = "STANDARD"
        End If

    End Sub
    Private Sub rbuSimplified_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rbuSimplified.CheckedChanged

        If rbuSimplified.Checked Then
            CurrentInvoiceType = "SIMPLIFIED"
        End If

    End Sub
    Private Sub rbuDeveloper_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rbuDeveloper.CheckedChanged

        If rbuDeveloper.Checked Then
            CurrentEnvironmentMode = "DEVELOPER"
        End If

    End Sub
    Private Sub rbuSimulation_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rbuSimulation.CheckedChanged

        If rbuSimulation.Checked Then
            CurrentEnvironmentMode = "SIMULATION"
        End If

    End Sub
    Private Sub rbuProduction_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles rbuProduction.CheckedChanged

        If rbuProduction.Checked Then
            CurrentEnvironmentMode = "PRODUCTION"
        End If

    End Sub

    Private Structure UnitInfo
        Public UnitID As Integer
        Public UnitCode As String
    End Structure
    Private Function GetUnitInfoByProductID(productID As Integer) As UnitInfo

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT P.StorageUnitID, U.UnitCode
            FROM Master_Product P
            INNER JOIN Master_Unit U
                ON U.UnitID = P.StorageUnitID
            WHERE P.ProductID = @PID", con)

                cmd.Parameters.AddWithValue("@PID", productID)

                Using rd = cmd.ExecuteReader()

                    If rd.Read() Then
                        Dim info As New UnitInfo
                        info.UnitID = CInt(rd("StorageUnitID"))
                        info.UnitCode = rd("UnitCode").ToString()
                        Return info
                    Else
                        Throw New Exception("لم يتم العثور على وحدة للمنتج رقم " & productID)
                    End If

                End Using
            End Using
        End Using

    End Function
    Private Function GetUnitNameByID(unitID As Integer) As String

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT UnitName
            FROM Master_Unit
            WHERE UnitID = @ID", con)

                cmd.Parameters.AddWithValue("@ID", unitID)

                Dim res = cmd.ExecuteScalar()

                If res Is Nothing OrElse IsDBNull(res) Then
                    Return ""
                End If

                Return res.ToString()
            End Using
        End Using

    End Function

    ' زر: btnReOpenRI
    ' يظهر فقط إذا كانت الفاتورة مرفوضة (StatusID = 19)
    ' ويعيدها إلى NEW (2) ثم يحدّث الشاشة

    Private Sub btnReOpenRI_Click(sender As Object, e As EventArgs) Handles btnReOpenRI.Click
        Try
            If CurrentDocumentID <= 0 Then
                MessageBox.Show("لا توجد فاتورة محفوظة", "تنبيه")
                Exit Sub
            End If

            Dim statusID As Integer = 0
            If cboStatusID.SelectedValue IsNot Nothing AndAlso IsNumeric(cboStatusID.SelectedValue) Then
                statusID = CInt(cboStatusID.SelectedValue)
            End If

            If statusID <> 19 Then
                MessageBox.Show("هذا الزر يعمل فقط للفواتير المرفوضة (REJECTED)", "تنبيه")
                Exit Sub
            End If

            If MessageBox.Show(
            "سيتم إعادة فتح الفاتورة (تحويلها إلى NEW) لإعادة إرسالها للهيئة." & vbCrLf &
            "هل تريد المتابعة؟",
            "تأكيد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        ) <> DialogResult.Yes Then
                Exit Sub
            End If

            Dim service As New InvoiceApplicationService()
            service.ReopenRejectedInvoice(CurrentDocumentID, CurrentEmployeeID)

            ' تحديث الشاشة
            LoadInvoiceDocument(CurrentDocumentID)
            ApplyInvoicePermission()

            MessageBox.Show("تمت إعادة فتح الفاتورة بنجاح (NEW). يمكنك الآن إعادة الإرسال.", "تم")

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try
    End Sub
    ' ضع هذا الإجراء داخل frmInvoice
    ' وظيفته: إظهار/إخفاء زر إعادة الفتح حسب الحالة

    Private Sub UpdateReopenButtonVisibility()
        Dim statusID As Integer = 0

        If cboStatusID.SelectedValue IsNot Nothing AndAlso IsNumeric(cboStatusID.SelectedValue) Then
            statusID = CInt(cboStatusID.SelectedValue)
        End If

        btnReOpenRI.Visible = (CurrentDocumentID > 0 AndAlso statusID = 19)
        btnReOpenRI.Enabled = btnReOpenRI.Visible
    End Sub
    Private Sub LoadInvoiceFromLoadingOrder(loID As Integer, srID As Integer, loSnapshotModifiedAt As DateTime?)

        ' 0) تحقق ومن ثم تحديث تلقائي إذا تغير
        Dim latestModAt As DateTime? = Nothing

        If Not EnsureLoadingOrderStillValidOrAutoRefresh(loID, loSnapshotModifiedAt, latestModAt) Then
            Exit Sub
        End If

        ' حدث snapshot إلى آخر قيمة (حتى لو كانت NULL)
        SourceLOModifiedAt = latestModAt

        ' 1) تحميل البيانات
        LoadInvoiceHeader_FromDB(loID)
        LoadPartnerFromSR(srID)
        LoadInvoiceDetails_FromDB(loID, srID)

    End Sub
    Private Function EnsureLoadingOrderStillValidOrAutoRefresh(
    loID As Integer,
    snapshotModifiedAt As DateTime?,
    ByRef latestModifiedAt As DateTime?
) As Boolean

        latestModifiedAt = Nothing

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT LoadingStatusID, ModifiedAt
FROM dbo.Logistics_LoadingOrder
WHERE LOID = @LOID
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()

                    If Not rd.Read() Then
                        MessageBox.Show("أمر التحميل غير موجود.", "تنبيه")
                        Return False
                    End If

                    Dim statusID As Integer = CInt(rd("LoadingStatusID"))
                    Dim dbModAt As DateTime? =
                        If(IsDBNull(rd("ModifiedAt")), CType(Nothing, DateTime?), CDate(rd("ModifiedAt")))

                    latestModifiedAt = dbModAt

                    If statusID = 10 Then
                        MessageBox.Show("تم إلغاء أمر التحميل من المستودع، لا يمكن سحبه.", "تنبيه")
                        Return False
                    End If

                    Dim changed As Boolean =
                        (snapshotModifiedAt.HasValue Xor dbModAt.HasValue) OrElse
                        (snapshotModifiedAt.HasValue AndAlso dbModAt.HasValue AndAlso snapshotModifiedAt.Value <> dbModAt.Value)

                    If changed Then
                        MessageBox.Show(
                            "تم تحديث بيانات أمر التحميل من قبل المستودع." & vbCrLf &
                            "سيتم تحديث البيانات الآن، يرجى مراجعة التفاصيل قبل السحب.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        )
                    End If

                    Return True

                End Using
            End Using
        End Using

    End Function
End Class

