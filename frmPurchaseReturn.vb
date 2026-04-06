Imports System.Data.SqlClient

Public Class frmPurchaseReturn
    Inherits AABaseOperationForm

    ' =========================
    ' State
    ' =========================
    Private InvoiceDetailsDT As DataTable
    Private IsResolvingProduct As Boolean = False
    Private IsLoading As Boolean = False
    Private IsLoadingInvoiceDetails As Boolean = False

    Private CurrentEmployeeID As Integer = 1
    Private CurrentPRTDocumentID As Integer = 0      ' يمثل المرتجع فقط
    Private CurrentSourceSALDocumentID As Integer = 0 ' يمثل فاتورة المبيعات المصدر

    ' =========================
    ' Load
    ' =========================
    Private Sub frmSalesReturn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IsLoading Then Return
        IsLoading = True
        Try
            LoadAllCombos()
            InitInvoiceDetailsTable()
            BindInvoiceGrid()
            ApplyDecimalFormatting()

            InitNewDocument()
            ApplyInvoicePermission()
        Finally
            IsLoading = False
        End Try
    End Sub

    ' =========================
    ' Combos
    ' =========================
    Private Sub LoadAllCombos()
        LoadPaymentMethods()
        LoadPaymentTerms()

        LoadStores()
        LoadPartners()
        LoadProductTypes()
        LoadTaxTypes()
        LoadTaxReasonCombo()
        LoadInvoiceStatuses()
    End Sub

    Private Sub LoadStores()
        LoadCombo(
            cboTargetStoreID,
            "
            SELECT StoreID, StoreName
            FROM Master_Store
            WHERE IsActive = 1
            ORDER BY StoreName
            ",
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
            SELECT PartnerID, PartnerCode, PartnerName, VATRegistrationNumber
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

    Private Sub LoadTaxReasonCombo()
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
                SELECT TaxReasonID, ReasonCode, ReasonNameAr
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

    Private Sub LoadInvoiceStatuses()
        LoadCombo(
            cboStatusID,
            "SELECT StatusID, StatusName FROM Workflow_Status WHERE IsActive = 1 ORDER BY StatusID",
            "StatusName",
            "StatusID"
        )
    End Sub

    ' =========================
    ' Helpers (Combo)
    ' =========================
    Private Sub LoadCombo(cbo As ComboBox, sql As String, displayMember As String, valueMember As String)
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                Dim dt As New DataTable()
                da.Fill(dt)
                cbo.DataSource = dt
                cbo.DisplayMember = displayMember
                cbo.ValueMember = valueMember
                cbo.SelectedIndex = -1
            End Using
        End Using
    End Sub

    Private Sub LoadGridCombo(col As DataGridViewComboBoxColumn, sql As String, displayMember As String, valueMember As String)
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                Dim dt As New DataTable()
                da.Fill(dt)
                col.DataSource = dt
                col.DisplayMember = displayMember
                col.ValueMember = valueMember
            End Using
        End Using
    End Sub

    ' =========================
    ' Partner UI
    ' =========================
    Private Sub cboPartnerID_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboPartnerID.SelectionChangeCommitted
        Dim drv As DataRowView = TryCast(cboPartnerID.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        txtPartnerName.Text = drv("PartnerName").ToString()
        txtVATRegistrationNumber.Text =
            If(IsDBNull(drv("VATRegistrationNumber")), "", drv("VATRegistrationNumber").ToString())
    End Sub

    ' =========================
    ' Details Table (CLEAN)
    ' =========================
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
        InvoiceDetailsDT.Columns.Add("ReturnQty", GetType(Decimal))
        InvoiceDetailsDT.Columns("ReturnQty").DefaultValue = 0D
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
        InvoiceDetailsDT.Columns.Add("TargetStoreID", GetType(Integer))

        InvoiceDetailsDT.Columns.Add("Note", GetType(String))
        InvoiceDetailsDT.Columns.Add("Length", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("Width", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("Height", GetType(Decimal))
        InvoiceDetailsDT.Columns.Add("SourceDocumentDetailID", GetType(Integer))
        InvoiceDetailsDT.Columns("SourceDocumentDetailID").AllowDBNull = True

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

        colDetDiscountPercent.ReadOnly = True
        colDetDiscountAmount.ReadOnly = True


        ' ComboBox
        colDetProductCode.DataPropertyName = "ProductCode"
        colDetProductCode.ValueMember = "ProductCode"
        colDetProductCode.DisplayMember = "ProductCode"

        colProductName.DataPropertyName = "ProductName"
        colProductName.ReadOnly = True

        colDetProductType.DataPropertyName = "ProductTypeID"
        colDetReturnQty.DataPropertyName = "ReturnQty"

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
        colDetSourceDocumentDetailID.DataPropertyName = "SourceDocumentDetailID"
        colDetSourceDocumentDetailID.Visible = False

    End Sub

    Private Sub ApplyDecimalFormatting()
        Dim numFmt As String = "0.000"
        colDetQty.DefaultCellStyle.Format = numFmt
        colDetPieceSellPrice.DefaultCellStyle.Format = numFmt
        colDetLineAmount.DefaultCellStyle.Format = numFmt
        colDetDiscountAmount.DefaultCellStyle.Format = numFmt
        colDetTaxableAmount.DefaultCellStyle.Format = numFmt
        colDetTaxAmount.DefaultCellStyle.Format = numFmt
        colTotal.DefaultCellStyle.Format = numFmt
        colDetDiscountPercent.DefaultCellStyle.Format = "0'%'" ' عرض
    End Sub

    ' =========================
    ' New Document
    ' =========================
    Private Sub InitNewDocument()
        CurrentDocumentID = 0

        cboPartnerID.SelectedIndex = -1
        cboStatusID.SelectedValue = 1

        txtPartnerName.Clear()
        txtVATRegistrationNumber.Clear()
        txtInvoiceNote.Clear()

        chkIsIncludeVAT.Checked = False
        chkIsExport.Checked = False
        cboTaxReason.SelectedIndex = -1
        txtTotalAmount.Text = "0.000"
        txtTotalDiscount.Text = "0.000"
        txtTotalTaxableAmount.Text = "0.000"
        txtTotalTax.Text = "0.000"
        txtGrandTotal.Text = "0.000"

        InvoiceDetailsDT.Clear()
        InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())
        InvoiceDetailsDT.Rows(0)("ReturnQty") = 0D

    End Sub

    Private Sub btnNewInvoice_Click(sender As Object, e As EventArgs) Handles btnNewInvoice.Click
        InitNewDocument()

        ApplyInvoicePermission()
        btnSaveDraft.Enabled = True
    End Sub


    ' =========================
    ' Store -> Products
    ' =========================
    Private Sub cboTargetStoreID_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboTargetStoreID.SelectionChangeCommitted
        If IsLoading Then Exit Sub
        If cboTargetStoreID.SelectedValue Is Nothing OrElse Not IsNumeric(cboTargetStoreID.SelectedValue) Then Exit Sub

        Dim storeID As Integer = CInt(cboTargetStoreID.SelectedValue)

        ' مسح التفاصيل إن كان فيها بيانات
        Dim hasData =
            InvoiceDetailsDT.AsEnumerable().Any(Function(r)
                                                    Return r.RowState <> DataRowState.Deleted AndAlso
                                                           Not IsDBNull(r("ProductCode")) AndAlso
                                                           r("ProductCode").ToString().Trim() <> ""
                                                End Function)

        If hasData Then
            Dim res = MessageBox.Show(
                "تغيير المستودع سوف يؤدي إلى مسح تفاصيل المستند. هل أنت متأكد؟",
                "تحذير",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            )
            If res <> DialogResult.Yes Then Exit Sub

            InvoiceDetailsDT.Clear()
            InvoiceDetailsDT.Rows.Add(InvoiceDetailsDT.NewRow())
            RecalculateInvoiceTotals_FromGrid()
        End If

        ChangeTargetStore(storeID)
    End Sub

    Private Sub ChangeTargetStore(storeID As Integer)
        LoadProductsForGrid(storeID)
        For Each r As DataRow In InvoiceDetailsDT.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            r("TargetStoreID") = storeID
        Next
    End Sub

    Private Sub LoadProductsForGrid(storeID As Integer)

        Dim sql As String = "
        SELECT DISTINCT P.ProductCode
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
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    colDetProductCode.DataSource = dt
                    colDetProductCode.DisplayMember = "ProductCode"
                    colDetProductCode.ValueMember = "ProductCode"

                    ' 🔥 هذا السطر هو الحل
                    colDetProductCode.DataPropertyName = "ProductCode"

                End Using
            End Using
        End Using

    End Sub
    ' =========================
    ' Product resolve
    ' =========================
    Private Function GetProductVariantsByCode(code As String) As DataTable
        Dim dt As New DataTable()
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
                SELECT ProductID, ProductTypeID, ProductName, StorageUnitID
                FROM Master_Product
                WHERE ProductCode = @Code AND IsActive = 1
            ", con)
                cmd.Parameters.AddWithValue("@Code", code)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    Private Function GetUnitNameByID(unitID As Integer) As String
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("SELECT UnitName FROM Master_Unit WHERE UnitID = @ID", con)
                cmd.Parameters.AddWithValue("@ID", unitID)
                Dim res = cmd.ExecuteScalar()
                If res Is Nothing OrElse IsDBNull(res) Then Return ""
                Return res.ToString()
            End Using
        End Using
    End Function

    Private Sub TryResolveProduct(rowIndex As Integer)
        If IsResolvingProduct Then Exit Sub
        If rowIndex < 0 OrElse rowIndex >= dgvInvoiceDetails.Rows.Count Then Exit Sub

        Dim row = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim codeObj = row.Cells("colDetProductCode").Value
        If codeObj Is Nothing OrElse IsDBNull(codeObj) Then Exit Sub

        Dim productCode = codeObj.ToString().Trim()
        If productCode = "" Then Exit Sub

        Dim variants = GetProductVariantsByCode(productCode)
        If variants Is Nothing OrElse variants.Rows.Count = 0 Then Exit Sub

        Dim typeVal = row.Cells("colDetProductType").Value
        If typeVal Is Nothing OrElse IsDBNull(typeVal) Then
            If variants.Rows.Count = 1 Then
                row.Cells("colDetProductType").Value = CInt(variants.Rows(0)("ProductTypeID"))
            Else
                Exit Sub
            End If
        End If

        Dim productTypeID As Integer
        If Not Integer.TryParse(row.Cells("colDetProductType").Value.ToString(), productTypeID) Then Exit Sub

        Dim rows = variants.Select("ProductTypeID = " & productTypeID)
        If rows.Length <> 1 Then Exit Sub

        Dim p = rows(0)

        IsResolvingProduct = True
        Try
            Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)
            If drv Is Nothing Then Exit Sub

            drv("ProductID") = CInt(p("ProductID"))
            drv("ProductName") = p("ProductName").ToString()
            drv("UnitID") = CInt(p("StorageUnitID"))
            row.Cells("colProductName").Value = p("ProductName").ToString()

        Finally
            IsResolvingProduct = False
        End Try
    End Sub

    Private Sub dgvInvoiceDetails_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoiceDetails.CellEndEdit
        If e.RowIndex < 0 Then Exit Sub
        Dim colName = dgvInvoiceDetails.Columns(e.ColumnIndex).Name

        If colName = "colDetProductCode" OrElse colName = "colDetProductType" OrElse colName = "colDetReturnQty" Then
            TryResolveProduct(e.RowIndex)
        End If

        RecalculateRow(e.RowIndex)
        RecalculateInvoiceTotals_FromGrid()
    End Sub

    ' =========================
    ' Calculations
    ' =========================
    Private Function ToDec(val As Object) As Decimal
        If val Is Nothing OrElse IsDBNull(val) Then Return 0D
        Dim s = val.ToString().Trim()
        If s = "" Then Return 0D
        Dim d As Decimal
        Decimal.TryParse(s, d)
        Return d
    End Function

    Private Function GetVatPercentByTaxTypeID(taxTypeID As Integer) As Decimal
        If taxTypeID = 1 Then Return 15D
        Return 0D
    End Function
    Private Function GetPreviousReturnedQty(sourceDetailID As Integer, Optional excludeDocumentID As Integer = 0) As Decimal

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT ISNULL(SUM(D.Quantity),0)
FROM Inventory_DocumentDetails D
INNER JOIN Inventory_DocumentHeader H
    ON H.DocumentID = D.DocumentID
WHERE H.DocumentType = 'PRT'
  AND H.StatusID <> 10
  AND D.SourceDocumentDetailID = @SourceDetailID
  AND H.DocumentID <> @ExcludeDocumentID
        ", con)

                cmd.Parameters.AddWithValue("@SourceDetailID", sourceDetailID)
                cmd.Parameters.AddWithValue("@ExcludeDocumentID", excludeDocumentID)

                Return Convert.ToDecimal(cmd.ExecuteScalar())

            End Using
        End Using

    End Function


    Private Sub RecalculateRow(rowIndex As Integer)

        If rowIndex < 0 OrElse rowIndex >= dgvInvoiceDetails.Rows.Count Then Exit Sub
        Dim row = dgvInvoiceDetails.Rows(rowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim Quantity = ToDec(row.Cells("colDetReturnQty").Value)
        Dim originalQty = ToDec(row.Cells("colDetQty").Value)

        Dim previousReturned As Decimal = 0D

        If row.Cells("colDetSourceDocumentDetailID").Value IsNot Nothing _
   AndAlso Not IsDBNull(row.Cells("colDetSourceDocumentDetailID").Value) Then

            Dim sourceDetailID As Integer =
        Convert.ToInt32(row.Cells("colDetSourceDocumentDetailID").Value)

            previousReturned = GetPreviousReturnedQty(sourceDetailID, CurrentPRTDocumentID)

        End If

        Dim allowedQty As Decimal = originalQty - previousReturned

        If Quantity > allowedQty Then
            MessageBox.Show("الكمية المتبقية المسموح بها: " & allowedQty.ToString("0.000"))
            row.Cells("colDetReturnQty").Value = allowedQty
            Quantity = allowedQty
        End If

        Dim unitPrice = ToDec(row.Cells("colDetPieceSellPrice").Value)
        Dim DiscountRate = ToDec(row.Cells("colDetDiscountPercent").Value)

        Dim taxTypeID As Integer = 1
        If row.Cells("colDetTaxPercent").Value IsNot Nothing _
       AndAlso Not IsDBNull(row.Cells("colDetTaxPercent").Value) Then
            Integer.TryParse(row.Cells("colDetTaxPercent").Value.ToString(), taxTypeID)
        End If

        Dim vatPercent = GetVatPercentByTaxTypeID(taxTypeID)

        Dim baseAmount = Math.Round(Quantity * unitPrice, 6, MidpointRounding.AwayFromZero)
        Dim discountAmount = Math.Round(baseAmount * DiscountRate / 100D, 6, MidpointRounding.AwayFromZero)
        Dim afterDiscount = Math.Round(baseAmount - discountAmount, 6, MidpointRounding.AwayFromZero)

        Dim taxable As Decimal
        Dim vat As Decimal
        Dim total As Decimal

        If chkIsIncludeVAT.Checked Then
            total = afterDiscount
            taxable = If(vatPercent > 0D,
                     Math.Round(total / (1D + vatPercent / 100D), 6, MidpointRounding.AwayFromZero),
                     total)
            vat = Math.Round(total - taxable, 6, MidpointRounding.AwayFromZero)
        Else
            taxable = afterDiscount
            vat = Math.Round(taxable * vatPercent / 100D, 6, MidpointRounding.AwayFromZero)
            total = Math.Round(taxable + vat, 6, MidpointRounding.AwayFromZero)
        End If

        row.Cells("colDetLineAmount").Value = baseAmount
        row.Cells("colDetDiscountAmount").Value = discountAmount
        row.Cells("colDetTaxableAmount").Value = taxable
        row.Cells("colDetTaxAmount").Value = vat
        row.Cells("colTotal").Value = total

    End Sub

    Private Sub RecalculateInvoiceTotals_FromGrid()
        Dim GrossAmount As Decimal = 0D
        Dim totalDiscount As Decimal = 0D
        Dim totalTaxable As Decimal = 0D
        Dim totalTax As Decimal = 0D
        Dim TotalAmount As Decimal = 0D

        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows
            If row.IsNewRow Then Continue For
            GrossAmount += ToDec(row.Cells("colDetLineAmount").Value)
            totalDiscount += ToDec(row.Cells("colDetDiscountAmount").Value)
            totalTaxable += ToDec(row.Cells("colDetTaxableAmount").Value)
            totalTax += ToDec(row.Cells("colDetTaxAmount").Value)
            TotalAmount += ToDec(row.Cells("colTotal").Value)
        Next

        txtTotalAmount.Text = GrossAmount.ToString("0.000")
        txtTotalDiscount.Text = totalDiscount.ToString("0.000")
        txtTotalTaxableAmount.Text = totalTaxable.ToString("0.000")
        txtTotalTax.Text = totalTax.ToString("0.000")
        txtGrandTotal.Text = TotalAmount.ToString("0.000")
    End Sub

    Private Sub chkIsIncludeVAT_CheckedChanged(sender As Object, e As EventArgs) Handles chkIsIncludeVAT.CheckedChanged
        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows
            If row.IsNewRow Then Continue For
            RecalculateRow(row.Index)
        Next
        RecalculateInvoiceTotals_FromGrid()
    End Sub

    ' =========================
    ' Validation (NO PAYMENT)
    ' =========================
    Private Function ValidateInvoiceDraft() As Boolean
        If cboPartnerID.SelectedValue Is Nothing Then
            MessageBox.Show("يرجى اختيار العميل")
            Return False
        End If

        If cboTargetStoreID.SelectedValue Is Nothing Then
            MessageBox.Show("يرجى اختيار المستودع")
            Return False
        End If

        Dim hasValid =
            InvoiceDetailsDT.AsEnumerable().
            Any(Function(r)
                    If r.RowState = DataRowState.Deleted Then Return False
                    Return Not IsDBNull(r("ProductID")) AndAlso ToDec(r("ReturnQty")) > 0D
                End Function)

        If Not hasValid Then
            MessageBox.Show("لا توجد سطور صالحة (كمية/صنف)")
            Return False
        End If

        Return True
    End Function

    ' =========================
    ' Policy Permission
    ' =========================
    Private Sub ApplyInvoicePermission()
        Dim statusID As Integer
        If CurrentPRTDocumentID <= 0 Then
            statusID = Workflow_OperationPolicyHelper.GetInitialStatusByScope("PRT")
        Else
            Workflow_OperationPolicyHelper.GetEntityStatusByScope("PRT", CurrentPRTDocumentID, statusID)
        End If

        Dim opID = Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("PRT")
        Dim policy = Workflow_OperationPolicyHelper.GetEditPolicy(opID, statusID)

        btnSaveDraft.Enabled = (policy.Mode <> EditMode.None)
        btnPostReturnInvoice.Enabled = policy.IsPostable
        btnCancel.Enabled = policy.IsCancelable

        cboPartnerID.Enabled = policy.AllowEditData
        txtInvoiceNote.ReadOnly = Not policy.AllowEditData

        dgvInvoiceDetails.ReadOnly = (policy.Mode = EditMode.None)
        dgvInvoiceDetails.AllowUserToAddRows = policy.AllowEditQuantity
        dgvInvoiceDetails.AllowUserToDeleteRows = policy.IsCancelable

        cboStatusID.SelectedValue = statusID
        If policy.IsPostable = False AndAlso policy.AllowEditData = False Then

            dgvInvoiceDetails.ReadOnly = True
            dgvInvoiceDetails.AllowUserToAddRows = False
            dgvInvoiceDetails.AllowUserToDeleteRows = False

        End If

    End Sub

    Private Sub btnSaveDraft_Click(
    sender As Object,
    e As EventArgs
) Handles btnSaveDraft.Click

        If cboStatusID.SelectedValue IsNot Nothing Then
            Dim statusID As Integer = CInt(cboStatusID.SelectedValue)

            ' يسمح بالحفظ في 1 (Draft) و 2 (NEW)
            If statusID <> 1 AndAlso statusID <> 2 Then
                MessageBox.Show("لا يمكن تعديل مستند بعد ترحيله")
                Exit Sub
            End If
        End If

        If Not ValidateInvoiceDraft() Then Exit Sub

        RecalculateInvoiceTotals_FromGrid()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    Dim nowDate = DateTime.Now
                    Dim isUpdate As Boolean = (CurrentPRTDocumentID > 0)
                    Dim documentID As Integer = CurrentPRTDocumentID
                    Dim nextCode As String = ""

                    ' =====================================
                    ' (0) ✅ تحقق قبل أي إدخال:
                    ' لا يسمح بحفظ مرتجع إلا إذا كانت الفاتورة الأصلية SAL بحالة CLEARED (18)
                    ' يعتمد على SourceDocumentDetailID من الجريد
                    ' =====================================
                    Dim sourceDetailIDs As New List(Of Integer)

                    For Each row As DataGridViewRow In dgvInvoiceDetails.Rows
                        If row.IsNewRow Then Continue For

                        Dim currentQty As Decimal = ToDec(row.Cells("colDetReturnQty").Value)
                        If currentQty <= 0D Then Continue For

                        If row.Cells("colDetSourceDocumentDetailID").Value Is Nothing _
                        OrElse IsDBNull(row.Cells("colDetSourceDocumentDetailID").Value) Then
                            Continue For
                        End If

                        Dim sourceDetailID As Integer = Convert.ToInt32(row.Cells("colDetSourceDocumentDetailID").Value)
                        If sourceDetailID > 0 Then sourceDetailIDs.Add(sourceDetailID)
                    Next

                    If sourceDetailIDs.Count = 0 Then
                        MessageBox.Show("لا توجد كميات مرتجعة للحفظ", "تنبيه")
                        tran.Rollback()
                        Exit Sub
                    End If

                    Dim inList As String = String.Join(",", sourceDetailIDs.Distinct())

                    Using cmdChk As New SqlCommand("
IF EXISTS (
    SELECT 1
    FROM dbo.Inventory_DocumentDetails OD
    INNER JOIN dbo.Inventory_DocumentHeader OH
        ON OH.DocumentID = OD.DocumentID
    WHERE OD.DetailID IN (" & inList & ")
      AND OH.DocumentType = 'PUR'
      AND OH.StatusID <> 6
)
    THROW 51010, N'لا يمكن عمل مرتجع إلا على فاتورة مشتريات مستلمة (ZATCA_CLEARED).', 1;
", con, tran)
                        cmdChk.ExecuteNonQuery()
                    End Using

                    ' =====================================
                    ' توليد رقم فقط في حالة INSERT
                    ' =====================================
                    If Not isUpdate Then
                        Using cmdCode As New SqlCommand("cfg.GetNextCode", con, tran)
                            cmdCode.CommandType = CommandType.StoredProcedure
                            cmdCode.Parameters.AddWithValue("@CodeType", "PRT")

                            Dim pNextCode As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                            pNextCode.Direction = ParameterDirection.Output
                            cmdCode.Parameters.Add(pNextCode)

                            cmdCode.ExecuteNonQuery()
                            nextCode = pNextCode.Value.ToString()
                        End Using
                    End If

                    ' =====================================
                    ' HEADER
                    ' =====================================
                    If isUpdate Then

                        Using cmd As New SqlCommand("
UPDATE Inventory_DocumentHeader SET
    DocumentDate = @DocumentDate,
    PartnerID = @PartnerID,
    TotalAmount = @TotalAmount,
    TotalDiscount = @TotalDiscount,
    TotalTax = @TotalTax,
    TotalTaxableAmount = @TotalTaxableAmount,
    RemainingAmount = @GrandTotal,
    Notes = @Notes,
    PaymentMethodID = @PaymentMethodID,
    PaymentTermID = @PaymentTermID,
    IsTaxInclusive = @IsTaxInclusive,
    GrandTotal = @GrandTotal,
    StatusID = 2
WHERE DocumentID = @DocumentID
", con, tran)

                            cmd.Parameters.AddWithValue("@DocumentID", documentID)
                            cmd.Parameters.AddWithValue("@DocumentDate", dtpDocumentDate.Value)
                            cmd.Parameters.AddWithValue("@PartnerID", CInt(cboPartnerID.SelectedValue))
                            cmd.Parameters.AddWithValue("@TotalAmount", CDec(txtTotalAmount.Text))
                            cmd.Parameters.AddWithValue("@TotalDiscount", CDec(txtTotalDiscount.Text))
                            cmd.Parameters.AddWithValue("@TotalTax", CDec(txtTotalTax.Text))
                            cmd.Parameters.AddWithValue("@TotalTaxableAmount", CDec(txtTotalTaxableAmount.Text))
                            cmd.Parameters.AddWithValue("@GrandTotal", CDec(txtGrandTotal.Text))
                            cmd.Parameters.AddWithValue("@Notes", txtInvoiceNote.Text.Trim())
                            cmd.Parameters.AddWithValue("@PaymentMethodID", CInt(cboPaymentMethodID.SelectedValue))
                            cmd.Parameters.AddWithValue("@PaymentTermID",
                            If(cboPaymentTerm.SelectedValue Is Nothing, DBNull.Value, cboPaymentTerm.SelectedValue))
                            cmd.Parameters.AddWithValue("@IsTaxInclusive", chkIsIncludeVAT.Checked)

                            cmd.ExecuteNonQuery()
                        End Using

                        ' حذف التفاصيل القديمة
                        Using cmdDel As New SqlCommand("
DELETE FROM Inventory_DocumentDetails
WHERE DocumentID = @DocumentID
", con, tran)

                            cmdDel.Parameters.AddWithValue("@DocumentID", documentID)
                            cmdDel.ExecuteNonQuery()
                        End Using

                    Else

                        Using cmd As New SqlCommand("
INSERT INTO Inventory_DocumentHeader
(
    DocumentType,
    DocumentNo,
    DocumentDate,
    PartnerID,
    CurrencyID,
    ExchangeRate,
    TotalAmount,
    TotalDiscount,
    TotalTax,
    TotalTaxableAmount,
    PaidAmount,
    RemainingAmount,
    Notes,
    CreatedAt,
    PaymentMethodID,
    TaxTypeID,
    PaymentTermID,
    StatusID,
    IsZatcaReported,
    IsTaxInclusive,
    IsOutbound,
    GrandTotal,
    IsInventoryPosted
)
VALUES
(
    'PRT',
    @DocumentNo,
    @DocumentDate,
    @PartnerID,
    1,
    1,
    @TotalAmount,
    @TotalDiscount,
    @TotalTax,
    @TotalTaxableAmount,
    0,
    @GrandTotal,
    @Notes,
    @CreatedAt,
    @PaymentMethodID,
    1,
    @PaymentTermID,
    2,
    0,
    @IsTaxInclusive,
    0,
    @GrandTotal,
    0
);
SELECT SCOPE_IDENTITY();
", con, tran)

                            cmd.Parameters.AddWithValue("@DocumentNo", nextCode)
                            cmd.Parameters.AddWithValue("@DocumentDate", dtpDocumentDate.Value)
                            cmd.Parameters.AddWithValue("@PartnerID", CInt(cboPartnerID.SelectedValue))
                            cmd.Parameters.AddWithValue("@TotalAmount", CDec(txtTotalAmount.Text))
                            cmd.Parameters.AddWithValue("@TotalDiscount", CDec(txtTotalDiscount.Text))
                            cmd.Parameters.AddWithValue("@TotalTax", CDec(txtTotalTax.Text))
                            cmd.Parameters.AddWithValue("@TotalTaxableAmount", CDec(txtTotalTaxableAmount.Text))
                            cmd.Parameters.AddWithValue("@GrandTotal", CDec(txtGrandTotal.Text))
                            cmd.Parameters.AddWithValue("@Notes", txtInvoiceNote.Text.Trim())
                            cmd.Parameters.AddWithValue("@CreatedAt", nowDate)
                            cmd.Parameters.AddWithValue("@PaymentMethodID", CInt(cboPaymentMethodID.SelectedValue))
                            cmd.Parameters.AddWithValue("@PaymentTermID",
                            If(cboPaymentTerm.SelectedValue Is Nothing, DBNull.Value, cboPaymentTerm.SelectedValue))
                            cmd.Parameters.AddWithValue("@IsTaxInclusive", chkIsIncludeVAT.Checked)

                            documentID = Convert.ToInt32(cmd.ExecuteScalar())
                        End Using

                    End If

                    ' 🔥 جلب جميع المرتجعات السابقة مرة واحدة
                    Dim returnedMap As New Dictionary(Of Integer, Decimal)

                    Using cmdPrev As New SqlCommand("
SELECT D.SourceDocumentDetailID,
       SUM(D.Quantity) AS TotalReturned
FROM Inventory_DocumentDetails D
INNER JOIN Inventory_DocumentHeader H
    ON H.DocumentID = D.DocumentID
WHERE H.DocumentType = 'PRT'
  AND H.StatusID <> 10
  AND H.DocumentID <> @CurrentID
GROUP BY D.SourceDocumentDetailID
", con, tran)

                        cmdPrev.Parameters.AddWithValue("@CurrentID", CurrentPRTDocumentID)

                        Using rdPrev = cmdPrev.ExecuteReader()
                            While rdPrev.Read()
                                returnedMap(CInt(rdPrev("SourceDocumentDetailID"))) =
                                Convert.ToDecimal(rdPrev("TotalReturned"))
                            End While
                        End Using
                    End Using

                    ' =====================================
                    ' DETAILS
                    ' =====================================
                    For Each row As DataGridViewRow In dgvInvoiceDetails.Rows

                        If row.IsNewRow Then Continue For

                        Dim currentQty As Decimal = ToDec(row.Cells("colDetReturnQty").Value)
                        If currentQty <= 0D Then Continue For

                        If row.Cells("colDetSourceDocumentDetailID").Value Is Nothing _
                        OrElse IsDBNull(row.Cells("colDetSourceDocumentDetailID").Value) Then
                            Continue For
                        End If

                        Dim sourceDetailID As Integer =
                        Convert.ToInt32(row.Cells("colDetSourceDocumentDetailID").Value)

                        Dim originalQty As Decimal =
                        ToDec(row.Cells("colDetQty").Value)

                        Dim previousReturned As Decimal = 0D
                        If returnedMap.ContainsKey(sourceDetailID) Then
                            previousReturned = returnedMap(sourceDetailID)
                        End If

                        If previousReturned + currentQty > originalQty Then
                            MessageBox.Show("تجاوزت الكمية المسموح بها للصنف")
                            tran.Rollback()
                            Exit Sub
                        End If

                        Using cmdDet As New SqlCommand("
INSERT INTO Inventory_DocumentDetails
(
    DocumentID,
    ProductID,
    Quantity,
    UnitID,
    UnitPrice,
    GrossAmount,
    DiscountRate,
    DiscountAmount,
    TaxRate,
    TaxAmount,
    NetAmount,
    TaxableAmount,
    SourceDocumentDetailID,
    SourceStoreID,
    LineTotal,
    TaxTypeID
)
VALUES
(
    @DocumentID,
    @ProductID,
    @Quantity,
    @UnitID,
    @UnitPrice,
    @GrossAmount,
    @DiscountRate,
    @DiscountAmount,
    @TaxRate,
    @TaxAmount,
    @NetAmount,
    @TaxableAmount,
    @SourceDocumentDetailID,
    @SourceStoreID,
    @LineTotal,
    @TaxTypeID
)
", con, tran)

                            cmdDet.Parameters.AddWithValue("@DocumentID", documentID)
                            cmdDet.Parameters.AddWithValue("@ProductID", CInt(row.Cells("colDetProductID").Value))
                            cmdDet.Parameters.AddWithValue("@Quantity", CDec(row.Cells("colDetReturnQty").Value))
                            cmdDet.Parameters.AddWithValue("@UnitID", CInt(row.Cells("colDetUnitID").Value))
                            cmdDet.Parameters.AddWithValue("@UnitPrice", CDec(row.Cells("colDetPieceSellPrice").Value))
                            cmdDet.Parameters.AddWithValue("@GrossAmount", CDec(row.Cells("colDetLineAmount").Value))
                            cmdDet.Parameters.AddWithValue("@DiscountRate", CDec(row.Cells("colDetDiscountPercent").Value))
                            cmdDet.Parameters.AddWithValue("@DiscountAmount", CDec(row.Cells("colDetDiscountAmount").Value))
                            cmdDet.Parameters.AddWithValue("@TaxRate",
                            GetVatPercentByTaxTypeID(CInt(row.Cells("colDetTaxPercent").Value)))
                            cmdDet.Parameters.AddWithValue("@TaxAmount", CDec(row.Cells("colDetTaxAmount").Value))
                            cmdDet.Parameters.AddWithValue("@NetAmount", CDec(row.Cells("colDetTaxableAmount").Value))
                            cmdDet.Parameters.AddWithValue("@TaxableAmount", CDec(row.Cells("colDetTaxableAmount").Value))
                            cmdDet.Parameters.AddWithValue("@SourceDocumentDetailID", sourceDetailID)
                            Dim storeID As Integer = CInt(cboTargetStoreID.SelectedValue)
                            cmdDet.Parameters.AddWithValue("@SourceStoreID", storeID)
                            cmdDet.Parameters.AddWithValue("@LineTotal", CDec(row.Cells("colTotal").Value))
                            cmdDet.Parameters.AddWithValue("@TaxTypeID", CInt(row.Cells("colDetTaxPercent").Value))

                            cmdDet.ExecuteNonQuery()
                        End Using

                    Next

                    tran.Commit()

                    CurrentPRTDocumentID = documentID
                    MessageBox.Show("تم حفظ المرتجع بنجاح")

                    ApplyInvoicePermission()

                Catch ex As Exception
                    Try : tran.Rollback() : Catch : End Try
                    MessageBox.Show("خطأ أثناء الحفظ: " & ex.Message)
                End Try

            End Using
        End Using

    End Sub
    Private Function GetDetailsTVP() As DataTable

        Dim dt As New DataTable()

        ' =========================
        ' Columns Definition
        ' =========================
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("Quantity", GetType(Decimal))
        dt.Columns.Add("UnitID", GetType(Integer))
        dt.Columns.Add("UnitPrice", GetType(Decimal))
        dt.Columns.Add("GrossAmount", GetType(Decimal))
        dt.Columns.Add("DiscountRate", GetType(Decimal))
        dt.Columns.Add("DiscountAmount", GetType(Decimal))
        dt.Columns.Add("TaxRate", GetType(Decimal))
        dt.Columns.Add("TaxAmount", GetType(Decimal))
        dt.Columns.Add("NetAmount", GetType(Decimal))
        dt.Columns.Add("LineTotal", GetType(Decimal))

        dt.Columns.Add("SourceDocumentDetailID", GetType(Integer))
        dt.Columns.Add("SourceStoreID", GetType(Integer))
        dt.Columns.Add("TargetStoreID", GetType(Integer))
        dt.Columns.Add("TaxTypeID", GetType(Integer))

        ' =========================
        ' Fill Rows
        ' =========================
        For Each row As DataGridViewRow In dgvInvoiceDetails.Rows

            If row.IsNewRow Then Continue For

            Dim drv As DataRowView =
            TryCast(row.DataBoundItem, DataRowView)

            If drv Is Nothing Then Continue For

            If IsDBNull(drv("ProductID")) _
           OrElse IsDBNull(drv("SourceLoadingOrderDetailID")) Then Continue For

            Dim dr = dt.NewRow()

            dr("ProductID") = Convert.ToInt32(drv("ProductID"))
            dr("Quantity") = Convert.ToDecimal(drv("ReturnQty"))
            dr("UnitID") = Convert.ToInt32(drv("UnitID"))
            dr("UnitPrice") = Convert.ToDecimal(drv("PieceSellPrice"))
            dr("GrossAmount") = Convert.ToDecimal(drv("GrossAmount"))
            dr("DiscountRate") = Convert.ToDecimal(drv("DiscountRate"))
            dr("DiscountAmount") = Convert.ToDecimal(drv("DiscountAmount"))
            dr("TaxRate") = GetVatPercentByTaxTypeID(Convert.ToInt32(drv("TaxTypeID")))
            dr("TaxAmount") = Convert.ToDecimal(drv("TaxAmount"))
            dr("NetAmount") = Convert.ToDecimal(drv("NetAmount"))
            dr("LineTotal") = Convert.ToDecimal(drv("LineTotal"))

            ' 👇 الربط مع سطر الفاتورة الأصلية
            dr("SourceDocumentDetailID") =
    Convert.ToInt32(drv("SourceDocumentDetailID"))

            ' مرتجع = دخول مخزون
            dr("SourceStoreID") = CInt(cboTargetStoreID.SelectedValue)
            dr("TargetStoreID") = DBNull.Value

            dr("TaxTypeID") = Convert.ToInt32(drv("TaxTypeID"))

            dt.Rows.Add(dr)

        Next

        Return dt

    End Function
    Private Sub btnPostReturnInvoice_Click(sender As Object, e As EventArgs) Handles btnPostReturnInvoice.Click

        If CurrentPRTDocumentID <= 0 Then
            MessageBox.Show("لا يوجد سند مرتجع لحفظه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    '=========================
                    ' 1) قراءة حالة السند
                    '=========================
                    Dim currentStatusID As Integer
                    Dim isTaxInclusive As Boolean
                    Dim documentNo As String
                    Dim notes As String

                    Using cmd As New SqlCommand("
SELECT StatusID, IsTaxInclusive, DocumentNo, Notes
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CurrentPRTDocumentID)

                        Using rd = cmd.ExecuteReader()
                            If Not rd.Read() Then Throw New Exception("السند غير موجود")
                            currentStatusID = CInt(rd("StatusID"))
                            isTaxInclusive = CBool(rd("IsTaxInclusive"))
                            documentNo = rd("DocumentNo").ToString()
                            notes = If(IsDBNull(rd("Notes")), "", rd("Notes").ToString())
                        End Using
                    End Using

                    If currentStatusID <> 2 Then
                        Throw New Exception("السند ليس في حالة NEW")
                    End If
                    Using cmd As New SqlCommand("
IF NOT EXISTS (
    SELECT 1
    FROM Inventory_DocumentDetails
    WHERE DocumentID = @ID
      AND Quantity > 0
)
    THROW 50060, N'لا توجد كميات مرتجعة للإرسال', 1;
", con, tran)

                        cmd.Parameters.AddWithValue("@ID", CurrentPRTDocumentID)
                        cmd.ExecuteNonQuery()
                    End Using
                    '=========================
                    ' 2) استخراج الفاتورة الأصلية + UUID (سطرًا سطرًا يجب أن يكون من نفس فاتورة)
                    '=========================
                    Dim sourceInvoiceID As Integer = 0
                    Dim sourceInvoiceUUID As String = Nothing
                    Dim sourceInvoiceNo As String = Nothing

                    ' (2.1) تأكيد أن كل سطور المرتجع ترجع من نفس فاتورة
                    Using cmd As New SqlCommand("
IF EXISTS (
    SELECT 1
    FROM (
        SELECT DISTINCT SD.DocumentID
        FROM Inventory_DocumentDetails R
        INNER JOIN Inventory_DocumentDetails SD
            ON SD.DetailID = R.SourceDocumentDetailID
        WHERE R.DocumentID = @PRT
    ) X
    HAVING COUNT(*) > 1
)
    THROW 50051, N'سند المرتجع مرتبط بأكثر من فاتورة أصلية، غير مسموح', 1;
", con, tran)
                        cmd.Parameters.AddWithValue("@PRT", CurrentPRTDocumentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' (2.2) جلب InvoiceID
                    Using cmd As New SqlCommand("
SELECT TOP 1 SD.DocumentID
FROM Inventory_DocumentDetails R
INNER JOIN Inventory_DocumentDetails SD
    ON SD.DetailID = R.SourceDocumentDetailID
WHERE R.DocumentID = @PRT
", con, tran)
                        cmd.Parameters.AddWithValue("@PRT", CurrentPRTDocumentID)
                        Dim r = cmd.ExecuteScalar()
                        If r Is Nothing OrElse IsDBNull(r) Then
                            Throw New Exception("لا يوجد سطر مرجعي لفاتورة أصلية داخل المرتجع (SourceDocumentDetailID).")
                        End If
                        sourceInvoiceID = CInt(r)
                    End Using


                    '=========================
                    ' 3) منع الإرجاع الزائد سطريًا
                    '   (QtyReturn <= QtyInvoice - Sum(PreviousReturns))
                    '=========================
                    Using cmd As New SqlCommand("
IF EXISTS (
    SELECT 1
    FROM Inventory_DocumentDetails R
    WHERE R.DocumentID = @PRT
      AND R.SourceDocumentDetailID IS NOT NULL
      AND R.Quantity >
      (
        ISNULL((
            SELECT I.Quantity
            FROM Inventory_DocumentDetails I
            WHERE I.DetailID = R.SourceDocumentDetailID
        ),0)
        -
        ISNULL((
            SELECT SUM(R2.Quantity)
            FROM Inventory_DocumentDetails R2
            INNER JOIN Inventory_DocumentHeader H2
                ON H2.DocumentID = R2.DocumentID
            WHERE H2.DocumentType = 'PRT'
              AND H2.StatusID <> 10
              AND R2.SourceDocumentDetailID = R.SourceDocumentDetailID
              AND H2.DocumentID <> @PRT
        ),0)
      )
)
    THROW 50052, N'لا يمكن إرجاع كمية أكبر من المتبقي في سطر الفاتورة', 1;
", con, tran)
                        cmd.Parameters.AddWithValue("@PRT", CurrentPRTDocumentID)
                        cmd.ExecuteNonQuery()
                    End Using

                    '=========================
                    ' 4) إنشاء Transaction Header للمرتجع
                    '=========================
                    Dim transactionID As Integer
                    Dim transactionCode As String = "PRT-" & CurrentPRTDocumentID.ToString().PadLeft(6, "0"c)

                    Dim periodID As Integer

                    Using cmd As New SqlCommand("
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE IsOpen = 1
ORDER BY PeriodID DESC
", con, tran)

                        Dim r = cmd.ExecuteScalar()
                        If r Is Nothing OrElse IsDBNull(r) Then
                            Throw New Exception("لا توجد فترة مالية مفتوحة")
                        End If

                        periodID = CInt(r)
                    End Using


                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionHeader
(
    TransactionDate,
    SourceDocumentID,
    OperationTypeID,
    PeriodID,
    StatusID,
    IsFinancialPosted,
    CreatedBy,
    CreatedAt,
    SentAt,
    SentBy,
    IsInventoryPosted
)
OUTPUT INSERTED.TransactionID
VALUES
(
    SYSDATETIME(),
    @DocumentID,
    14,
    @PeriodID,
    5,
    0,
    @UserID,
    SYSDATETIME(),
    SYSDATETIME(),
    @UserID,
    0
)", con, tran)

                        cmd.Parameters.AddWithValue("@DocumentID", CurrentPRTDocumentID)
                        cmd.Parameters.AddWithValue("@UserID", CurrentEmployeeID)
                        cmd.Parameters.AddWithValue("@PeriodID", periodID)

                        transactionID = CInt(cmd.ExecuteScalar())
                    End Using

                    '=========================
                    ' 5) إدخال TransactionDetails للمرتجع
                    '    - ReferenceDetailID = InvoiceDetailID (R.SourceDocumentDetailID)
                    '    - CostSnapshot من حركة التحميل الأصلية عبر InvoiceDetail.SourceLoadingOrderDetailID
                    '=========================
                    Using cmd As New SqlCommand("
INSERT INTO Inventory_TransactionDetails
(
    TransactionID,
    ProductID,
    Quantity,
    UnitID,
    UnitCost,
    CostAmount,
    SourceStoreID,
    TargetStoreID,
    SourceDocumentDetailID,
    ReferenceDetailID,
    CreatedAt,
    CreatedBy
)
SELECT
    @TID,
    R.ProductID,
    R.Quantity,
    R.UnitID,
    R.UnitPrice,
    R.Quantity * R.UnitPrice,
    R.SourceStoreID,
   NULL,
    R.DetailID,
    R.SourceDocumentDetailID,
    SYSDATETIME(),
    @UserID
FROM Inventory_DocumentDetails R
WHERE R.DocumentID = @PRT
  AND R.Quantity > 0;
", con, tran)

                        cmd.Parameters.AddWithValue("@TID", transactionID)
                        cmd.Parameters.AddWithValue("@PRT", CurrentPRTDocumentID)
                        cmd.Parameters.AddWithValue("@UserID", CurrentEmployeeID)

                        cmd.ExecuteNonQuery()
                    End Using



                    tran.Commit()

                    MessageBox.Show("تم إرسال مرتجع المشتريات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using

    End Sub
    Private Sub btnSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnGetOriginalInvoice.Click

        Dim f As New frmPurchaseSearch()


        f.ShowDialog()

        If f.SelectedDocumentID > 0 Then

            LoadInvoiceDocument(f.SelectedDocumentID)

            ' هذا مرتجع جديد
            CurrentSourceSALDocumentID = f.SelectedDocumentID
            CurrentPRTDocumentID = 0

            ApplyInvoicePermission()

        End If



    End Sub
    Private Sub dgvInvoiceDetails_DataError(
    sender As Object,
    e As DataGridViewDataErrorEventArgs
) Handles dgvInvoiceDetails.DataError

        e.ThrowException = False

    End Sub

    Private Sub LoadInvoiceDocument(documentID As Integer)
        If documentID <= 0 Then Exit Sub
        If IsLoading OrElse IsResolvingProduct Then Return
        IsLoadingInvoiceDetails = True

        Using con As New SqlConnection(ConnStr)
            con.Open()
            ' تحميل بيانات الهيدر
            Using cmd As New SqlCommand("
SELECT
    H.*,
    P.PartnerName,
    P.VATRegistrationNumber AS TaxNumber,
    LO.LOCode,
    SR.SRCode
FROM Inventory_DocumentHeader H
LEFT JOIN Master_Partner P ON P.PartnerID = H.PartnerID
LEFT JOIN Document_Link L ON L.TargetDocumentID = H.DocumentID AND L.TargetType = 'SAL'
LEFT JOIN Logistics_LoadingOrder LO ON LO.LOID = L.SourceDocumentID
LEFT JOIN Logistics_LoadingOrderSR LOSR ON LOSR.LOID = LO.LOID
LEFT JOIN Business_SR SR ON SR.SRID = LOSR.SRID
WHERE H.DocumentID = @DocumentID
", con)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        txtInvoicCode.Text = rd("DocumentNo").ToString()
                        dtpDocumentDate.Value = CDate(rd("DocumentDate"))
                        cboPartnerID.SelectedValue = rd("PartnerID")
                        cboPaymentMethodID.SelectedValue = rd("PaymentMethodID")
                        cboPaymentTerm.SelectedValue = rd("PaymentTermID")
                        Dim initialStatus As Integer = Workflow_OperationPolicyHelper.GetInitialStatusByScope("PRT")
                        cboStatusID.SelectedValue = initialStatus
                        chkIsIncludeVAT.Checked = CBool(rd("IsTaxInclusive"))
                        txtInvoiceNote.Text = If(IsDBNull(rd("Notes")), "", rd("Notes").ToString())
                        txtPartnerName.Text = If(IsDBNull(rd("PartnerName")), "", rd("PartnerName").ToString())
                        If Not IsDBNull(rd("TaxReasonID")) Then cboTaxReason.SelectedValue = rd("TaxReasonID")
                        If Not IsDBNull(rd("TaxNumber")) Then txtVATRegistrationNumber.Text = rd("TaxNumber").ToString()
                        txtTotalAmount.Text = rd("TotalAmount").ToString()
                        txtTotalDiscount.Text = rd("TotalDiscount").ToString()
                        txtTotalTaxableAmount.Text = rd("TotalTaxableAmount").ToString()
                        txtTotalTax.Text = rd("TotalTax").ToString()
                        txtGrandTotal.Text = rd("GrandTotal").ToString()
                    End If
                End Using
            End Using

            ' تحميل التفاصيل
            InvoiceDetailsDT.Clear()
            dgvInvoiceDetails.SuspendLayout()
            Using cmd As New SqlCommand("
SELECT
    D.DetailID,
    D.Quantity AS OriginalQty,
    D.ProductID,
    P.ProductCode,
    P.ProductName,
    P.ProductTypeID,
    P.Length,
    P.Width,
    P.Height,
    D.UnitID,
    U.UnitName,
    U.UnitCode,
    D.UnitPrice,
    D.DiscountRate,
    D.TaxRate,
    D.TaxTypeID,
    D.TargetStoreID
FROM Inventory_DocumentDetails D
INNER JOIN Master_Product P ON P.ProductID = D.ProductID
LEFT JOIN Master_Unit U ON U.UnitID = D.UnitID
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
                        dr("Quantity") = rd("OriginalQty")
                        dr("ReturnQty") = 0D
                        dr("PieceSellPrice") = rd("UnitPrice")
                        dr("DiscountRate") = rd("DiscountRate")
                        dr("TaxRate") = rd("TaxRate")
                        dr("TaxTypeID") = rd("TaxTypeID")
                        ' الصحيحة: خزن مخزن السطر في TargetStoreID
                        If Not IsDBNull(rd("TargetStoreID")) Then
                            dr("TargetStoreID") = rd("TargetStoreID")
                        Else
                            dr("TargetStoreID") = DBNull.Value
                        End If
                        dr("SourceDocumentDetailID") = rd("DetailID")
                        InvoiceDetailsDT.Rows.Add(dr)
                    End While
                End Using
            End Using
            dgvInvoiceDetails.ResumeLayout()

            ' تعبئة ComboBox المستودع من أول سطر فعلي
            If InvoiceDetailsDT.Rows.Count > 0 Then
                Dim firstRow As DataRow = InvoiceDetailsDT.Rows(0)
                If Not IsDBNull(firstRow("TargetStoreID")) Then
                    Dim storeID As Integer = CInt(firstRow("TargetStoreID"))
                    If cboTargetStoreID.DataSource Is Nothing OrElse cboTargetStoreID.Items.Count = 0 Then LoadStores()
                    cboTargetStoreID.SelectedValue = storeID
                    ChangeTargetStore(storeID)
                End If
            End If

        End Using

        IsLoadingInvoiceDetails = False
        btnSaveDraft.Text = "حفظ"
        ApplyInvoicePermission()
    End Sub

    Private Sub LoadPRTDocument(documentID As Integer)
        If documentID <= 0 Then Exit Sub
        IsLoadingInvoiceDetails = True

        Using con As New SqlConnection(ConnStr)
            con.Open()
            ' تحميل الهيدر
            Using cmd As New SqlCommand("
SELECT
    H.*,
    P.PartnerName,
    P.VATRegistrationNumber AS TaxNumber
FROM Inventory_DocumentHeader H
LEFT JOIN Master_Partner P ON P.PartnerID = H.PartnerID
WHERE H.DocumentID = @DocumentID
", con)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        CurrentPRTDocumentID = CInt(rd("DocumentID"))
                        CurrentSourceSALDocumentID = 0
                        txtInvoicCode.Text = rd("DocumentNo").ToString()
                        dtpDocumentDate.Value = CDate(rd("DocumentDate"))
                        cboPartnerID.SelectedValue = rd("PartnerID")
                        cboPaymentMethodID.SelectedValue = rd("PaymentMethodID")
                        cboPaymentTerm.SelectedValue = rd("PaymentTermID")
                        cboStatusID.SelectedValue = CInt(rd("StatusID"))
                        chkIsIncludeVAT.Checked = CBool(rd("IsTaxInclusive"))
                        txtInvoiceNote.Text = If(IsDBNull(rd("Notes")), "", rd("Notes").ToString())
                        txtPartnerName.Text = If(IsDBNull(rd("PartnerName")), "", rd("PartnerName").ToString())
                        If Not IsDBNull(rd("TaxReasonID")) Then cboTaxReason.SelectedValue = rd("TaxReasonID")
                        If Not IsDBNull(rd("TaxNumber")) Then txtVATRegistrationNumber.Text = rd("TaxNumber").ToString()
                        txtTotalAmount.Text = rd("TotalAmount").ToString()
                        txtTotalDiscount.Text = rd("TotalDiscount").ToString()
                        txtTotalTaxableAmount.Text = rd("TotalTaxableAmount").ToString()
                        txtTotalTax.Text = rd("TotalTax").ToString()
                        txtGrandTotal.Text = rd("GrandTotal").ToString()
                    End If
                End Using
            End Using

            ' تحميل التفاصيل
            InvoiceDetailsDT.Clear()

            Using cmd As New SqlCommand("
SELECT
    D.DetailID,
    D.SourceDocumentDetailID,
    D.Quantity AS ReturnQty,
    SD.Quantity AS OriginalQty,
    D.ProductID,
    P.ProductCode,
    P.ProductName,
    P.ProductTypeID,
    P.Length,
    P.Width,
    P.Height,
    D.UnitID,
    U.UnitName,
    U.UnitCode,
    D.UnitPrice,
    D.DiscountRate,
    D.TaxRate,
    D.TaxTypeID,
    D.SourceStoreID
FROM Inventory_DocumentDetails D
LEFT JOIN Inventory_DocumentDetails SD ON SD.DetailID = D.SourceDocumentDetailID
INNER JOIN Master_Product P ON P.ProductID = D.ProductID
LEFT JOIN Master_Unit U ON U.UnitID = D.UnitID
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
                        dr("Quantity") = rd("OriginalQty")
                        dr("ReturnQty") = rd("ReturnQty")
                        dr("PieceSellPrice") = rd("UnitPrice")
                        dr("DiscountRate") = rd("DiscountRate")
                        dr("TaxRate") = rd("TaxRate")
                        dr("TaxTypeID") = rd("TaxTypeID")
                        If Not IsDBNull(rd("SourceStoreID")) Then
                            dr("TargetStoreID") = rd("SourceStoreID")
                        Else
                            dr("TargetStoreID") = DBNull.Value
                        End If
                        dr("SourceDocumentDetailID") = rd("SourceDocumentDetailID")
                        InvoiceDetailsDT.Rows.Add(dr)
                    End While
                End Using

                ' تعبئة الكمبو بعد تحميل البيانات
                If InvoiceDetailsDT.Rows.Count > 0 Then
                    Dim firstRow As DataRow = InvoiceDetailsDT.Rows(0)
                    If Not IsDBNull(firstRow("TargetStoreID")) Then
                        Dim storeID As Integer = CInt(firstRow("TargetStoreID"))
                        If cboTargetStoreID.DataSource Is Nothing OrElse cboTargetStoreID.Items.Count = 0 Then LoadStores()
                        cboTargetStoreID.SelectedValue = storeID
                        ChangeTargetStore(storeID)
                    End If
                End If
            End Using

        End Using

        IsLoadingInvoiceDetails = False
        btnSaveDraft.Text = "حفظ"
        ApplyInvoicePermission()
    End Sub

    Private Sub btnSearch_Click_1(
    sender As Object,
    e As EventArgs
) Handles btnSearch.Click

        Using frm As New frmPurchaseReturnsearch() ' 👈 اسم فورم البحث الجديد

            If frm.ShowDialog() = DialogResult.OK Then

                If frm.SelectedDocumentID > 0 Then
                    LoadPRTDocument(frm.SelectedDocumentID)
                    '                   RefreshWorkflowUI()
                End If

            End If

        End Using

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        If CurrentPRTDocumentID <= 0 Then
            MessageBox.Show("لا يوجد مرتجع لإلغائه", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim res = MessageBox.Show("هل أنت متأكد من إلغاء هذا المرتجع؟",
                              "تأكيد الإلغاء",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question)

        If res <> DialogResult.Yes Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' 1) قراءة الحالة الحالية
                    Dim currentStatusID As Integer = 0

                    Using cmdGet As New SqlCommand("
SELECT StatusID
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
", con, tran)

                        cmdGet.Parameters.AddWithValue("@ID", CurrentPRTDocumentID)

                        Dim obj = cmdGet.ExecuteScalar()
                        If obj Is Nothing OrElse IsDBNull(obj) Then
                            Throw New Exception("السند غير موجود")
                        End If

                        currentStatusID = CInt(obj)
                    End Using

                    ' 2) شرطك الأساسي: الإلغاء فقط إذا الحالة = 2
                    If currentStatusID <> 2 Then
                        Throw New Exception("لا يمكن إلغاء المرتجع إلا إذا كانت حالته 2")
                    End If

                    ' 3) تحديث الحالة إلى CANCELED = 10
                    Using cmdUpd As New SqlCommand("
UPDATE Inventory_DocumentHeader
SET StatusID = 10
WHERE DocumentID = @ID
", con, tran)

                        cmdUpd.Parameters.AddWithValue("@ID", CurrentPRTDocumentID)
                        cmdUpd.ExecuteNonQuery()
                    End Using

                    tran.Commit()

                    MessageBox.Show("تم إلغاء المرتجع بنجاح",
                                "نجاح",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information)

                    ' 4) اختياري: إعادة تهيئة الفورم لمستند جديد
                    CurrentPRTDocumentID = 0
                    CurrentSourceSALDocumentID = 0

                    InitNewDocument()
                    ApplyInvoicePermission()

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show(ex.Message, "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using

    End Sub
End Class
