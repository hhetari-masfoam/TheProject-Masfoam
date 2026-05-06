Imports System.Data.SqlClient
Imports System.Runtime.Remoting
Imports THE_PROJECT.FinPostingService

Public Class frmfinPaymentVouchers
    Private IsLoading As Boolean = False
    Private Const VAT_RATE As Decimal = 0.15D
    Private CurrentOperationMode As String = ""
    Private CurrentOperationTypeID As Integer = 0
    Private PaymentAccountsTable As DataTable
    Private IsSaved As Boolean = False
    Private CurrentTransactionID As Integer = 0
    Private CurrentStatusID As Integer = 1
    Private AccountsTable As DataTable

    Private Sub SetupGrid()

        With dgvDetails

            .Columns.Clear()
            .AutoGenerateColumns = False
            .AllowUserToAddRows = True
            .AllowUserToDeleteRows = True

            ' 🔵 زر البحث
            Dim colSearch As New DataGridViewButtonColumn()
            colSearch.Name = "btnSearch"
            colSearch.HeaderText = ""
            colSearch.Text = "..."
            colSearch.UseColumnTextForButtonValue = True
            colSearch.Width = 40
            .Columns.Add(colSearch)

            ' 🔵 كود الحساب
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "AccountID",
                .HeaderText = "كود الحساب",
                .Width = 80
            })

            ' 🔵 اسم الحساب
            Dim colAccount As New DataGridViewComboBoxColumn()
            colAccount.Name = "AccountName"
            colAccount.HeaderText = "اسم الحساب"
            colAccount.DisplayMember = "AccountNameAr"
            colAccount.ValueMember = "AccountID"
            colAccount.Width = 200
            .Columns.Add(colAccount)

            ' 🔵 المبلغ
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "Amount",
                .HeaderText = "المبلغ",
                .Width = 120
            })

            ' 🔵 ضريبي
            .Columns.Add(New DataGridViewCheckBoxColumn With {
                .Name = "IsTaxable",
                .HeaderText = "خاضع للضريبة"
            })

            ' 🔵 شامل ضريبة
            .Columns.Add(New DataGridViewCheckBoxColumn With {
                .Name = "IsTaxIncluded",
                .HeaderText = "شامل الضريبة"
            })

            ' 🔵 نسبة الضريبة
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "TaxRate",
                .HeaderText = "نسبة الضريبة",
                .ReadOnly = True
            })

            ' 🔵 مبلغ الضريبة
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "TaxAmount",
                .HeaderText = "مبلغ الضريبة",
                .ReadOnly = True
            })

            ' 🔵 الصافي
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "NetAmount",
                .HeaderText = "الصافي",
                .ReadOnly = True
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
    .Name = "TotalAmount",
    .HeaderText = "الإجمالي",
    .ReadOnly = True
})
            ' 🔵 البيان
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "Description",
                .HeaderText = "البيان",
                .Width = 200
            })

            ' 🔴 حذف
            Dim colDelete As New DataGridViewButtonColumn()
            colDelete.Name = "btnDelete"
            colDelete.HeaderText = ""
            colDelete.Text = "X"
            colDelete.UseColumnTextForButtonValue = True
            colDelete.Width = 40
            .Columns.Add(colDelete)

        End With

    End Sub

    Private Sub dgvDetails_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDetails.CellContentClick

        If e.RowIndex < 0 Then Exit Sub
        If IsLoading Then Exit Sub
        Dim colName = dgvDetails.Columns(e.ColumnIndex).Name

        ' 🔵 بحث
        If colName = "btnSearch" Then

            Dim frm As New frmfinAccountsSearch()

            ' 🔵 مرر نوع العملية للفورم
            If cboOperationType.SelectedValue IsNot Nothing AndAlso
       Not TypeOf cboOperationType.SelectedValue Is DataRowView Then

                frm.OperationTypeID = CurrentOperationTypeID

            End If

            ' 🔵 افتح الفورم أولاً
            If frm.ShowDialog() = DialogResult.OK Then

                Dim selectedID As Integer = frm.SelectedAccountID

                Dim row = dgvDetails.Rows(e.RowIndex)

                row.Cells("AccountID").Value = selectedID

                dgvDetails.BeginEdit(True)
                row.Cells("AccountName").Value = selectedID
                dgvDetails.EndEdit()


            End If

        End If

        ' 🔴 حذف
        If colName = "btnDelete" Then
            If Not dgvDetails.Rows(e.RowIndex).IsNewRow Then
                dgvDetails.Rows.RemoveAt(e.RowIndex)
            End If
        End If

    End Sub


    Private Sub RecalculateRow(rowIndex As Integer)

        If rowIndex < 0 Then Exit Sub

        Dim row = dgvDetails.Rows(rowIndex)
        If row.IsNewRow Then Exit Sub

        Dim amount As Decimal = ToDec(row.Cells("Amount").Value)
        Dim isTaxable As Boolean = ToBool(row.Cells("IsTaxable").Value)
        Dim isIncluded As Boolean = ToBool(row.Cells("IsTaxIncluded").Value)

        Dim net As Decimal = amount
        Dim tax As Decimal = 0

        If isTaxable Then

            If isIncluded Then
                net = Math.Round(amount / (1 + VAT_RATE), 2)
                tax = amount - net
            Else
                net = amount
                tax = Math.Round(amount * VAT_RATE, 2)
            End If

        End If

        row.Cells("TaxRate").Value = VAT_RATE * 100
        row.Cells("TaxAmount").Value = tax
        row.Cells("NetAmount").Value = net
        row.Cells("TotalAmount").Value = net + tax
    End Sub

    Private Sub dgvDetails_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDetails.CellValueChanged

        If e.RowIndex < 0 Then Exit Sub
        If IsLoading Then Exit Sub
        Dim col = dgvDetails.Columns(e.ColumnIndex).Name

        If col = "Amount" Or col = "IsTaxable" Or col = "IsTaxIncluded" Then
            RecalculateRow(e.RowIndex)
            CalculateTotals()
        End If
        If col = "IsTaxable" Then

            Dim isTaxable = ToBool(dgvDetails.Rows(e.RowIndex).Cells("IsTaxable").Value)

            If Not isTaxable Then
                dgvDetails.Rows(e.RowIndex).Cells("IsTaxIncluded").Value = False
            End If

        End If


        If col = "IsTaxIncluded" Then

            Dim isIncluded = ToBool(dgvDetails.Rows(e.RowIndex).Cells("IsTaxIncluded").Value)

            If isIncluded Then
                dgvDetails.Rows(e.RowIndex).Cells("IsTaxable").Value = True
            End If

        End If
        If dgvDetails.Columns(e.ColumnIndex).Name = "AccountName" Then

            Dim row = dgvDetails.Rows(e.RowIndex)

            Dim val = row.Cells("AccountName").Value

            row.Cells("AccountID").Value = val

        End If


    End Sub
    Private Sub cboPaymentMethod_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboPaymentMethod.SelectionChangeCommitted

        If IsLoading Then Exit Sub

        FilterAccountsByPaymentMethod()

        ' 🔥 تصفير الحساب
        cboCashAccount.SelectedIndex = -1

    End Sub
    Private Sub CalculateTotals()

        Dim total As Decimal = 0
        Dim tax As Decimal = 0
        Dim net As Decimal = 0
        Dim TotalAmount As Decimal = 0

        For Each row As DataGridViewRow In dgvDetails.Rows

            If row.IsNewRow Then Continue For

            total += ToDec(row.Cells("TotalAmount").Value)
            tax += ToDec(row.Cells("TaxAmount").Value)
            net += ToDec(row.Cells("NetAmount").Value)
            TotalAmount += ToDec(row.Cells("TotalAmount").Value)

        Next

        txtTotalAmount.Text = total.ToString("N2")
        txtTotalTax.Text = tax.ToString("N2")
        txtTotalNet.Text = net.ToString("N2")

    End Sub
    Private Function BuildModel() As finCashTransactionModel

        Dim model As New finCashTransactionModel

        ' 🔵 الهيدر
        model.TransactionNo = txtTransactionNo.Text
        model.TransactionDate = dtpTransactionDate.Value

        model.OperationTypeID = CInt(cboOperationType.SelectedValue)
        model.Direction = GetDirection(model.OperationTypeID)

        model.CashAccountID = CInt(cboCashAccount.SelectedValue)

        model.CurrencyID = 1 ' حالياً ثابت
        model.ExchangeRate = 1

        model.PaymentMethodID = CInt(cboPaymentMethod.SelectedValue)

        model.Description = txtDescription.Text
        model.ReferenceNo = txtTransactionNo.Text

        model.TotalAmount = ToDec(txtTotalAmount.Text)

        ' 🔵 التفاصيل
        model.Details = BuildDetailsList(dgvDetails)

        Return model

    End Function
    Private Function GetDirection(opID As Integer) As Integer

        ' عدل حسب جدول العمليات عندك
        ' مثال:
        ' 1 = In
        ' 2 = Out

        Select Case opID
            Case 16, 18 ' صرف / دفع
                Return 2

            Case 17, 19 ' قبض
                Return 1

            Case Else
                Return 2
        End Select

    End Function
    Private Function BuildDetailsList(dgv As DataGridView) As List(Of finCashTransactionDetailModel)

        Dim list As New List(Of finCashTransactionDetailModel)

        For Each row As DataGridViewRow In dgv.Rows

            If row.IsNewRow Then Continue For

            Dim acc = ToInt(row.Cells("AccountID").Value)
            Dim amount = ToDec(row.Cells("Amount").Value)

            If acc <= 0 OrElse amount <= 0 Then Continue For

            Dim item As New finCashTransactionDetailModel With {
                .AccountID = acc,
                .Amount = amount,
                .IsTaxable = ToBool(row.Cells("IsTaxable").Value),
                .IsTaxIncluded = ToBool(row.Cells("IsTaxIncluded").Value),
                .TaxRate = ToDec(row.Cells("TaxRate").Value),
                .TaxAmount = ToDec(row.Cells("TaxAmount").Value),
                .NetAmount = ToDec(row.Cells("NetAmount").Value),
                .Description = ToStr(row.Cells("Description").Value)
            }

            list.Add(item)

        Next

        Return list

    End Function

    Private Sub ClearForm()

        txtTransactionNo.Clear()
        txtDescription.Clear()
        txtTransactionNo.Clear()

        dgvDetails.Rows.Clear()

        txtTotalAmount.Text = "0.00"
        txtTotalTax.Text = "0.00"
        txtTotalNet.Text = "0.00"

    End Sub
    Private Function ToDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal = 0
        Decimal.TryParse(v.ToString(), d)
        Return d
    End Function

    Private Function ToInt(v As Object) As Integer
        If v Is Nothing OrElse IsDBNull(v) Then Return 0
        Return Convert.ToInt32(v)
    End Function

    Private Function ToBool(v As Object) As Boolean
        If v Is Nothing OrElse IsDBNull(v) Then Return False
        Return Convert.ToBoolean(v)
    End Function

    Private Function ToStr(v As Object) As String
        If v Is Nothing OrElse IsDBNull(v) Then Return ""
        Return v.ToString()
    End Function
    Private Sub frmCashTransaction_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IsLoading = True
        SetupGrid()
        dgvDetails.Columns("TaxRate").DefaultCellStyle.Format = "0.##'%'"
        LoadPaymentMethods()
        LoadOperationTypes()
        LoadCashAccounts() ' 🔥 مهم جداً
        LoadAccountsForGrid()

        InitForm()
        dtpTransactionDate.Format = DateTimePickerFormat.Custom
        dtpTransactionDate.CustomFormat = "yyyy-MM-dd"

        IsLoading = False
    End Sub
    Private Sub LoadPaymentMethods()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable

            Using da As New SqlDataAdapter("
SELECT PaymentMethodID, NameAr
FROM md.PaymentMethod
WHERE IsActive = 1
ORDER BY PaymentMethodID
", con)

                da.Fill(dt)
            End Using

            cboPaymentMethod.DataSource = dt
            cboPaymentMethod.DisplayMember = "NameAr"
            cboPaymentMethod.ValueMember = "PaymentMethodID"

        End Using

    End Sub


    Private Sub LoadCashAccounts()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable

            Using da As New SqlDataAdapter("
SELECT 
    p.PaymentAccountID,
    p.AccountID,
    p.DisplayName,
    m.PaymentMethodID
FROM cfg.PaymentAccounts p
JOIN cfg.PaymentAccountMethods m 
    ON m.PaymentAccountID = p.PaymentAccountID
WHERE p.IsActive = 1
", con)

                da.Fill(dt)
            End Using
            cboCashAccount.DataSource = Nothing
            cboCashAccount.DisplayMember = "DisplayName"
            cboCashAccount.ValueMember = "AccountID"

            PaymentAccountsTable = dt


        End Using

    End Sub
    Private Sub FilterAccountsByPaymentMethod()

        If IsLoading Then Exit Sub
        If PaymentAccountsTable Is Nothing Then Exit Sub
        If cboPaymentMethod.SelectedValue Is Nothing Then Exit Sub
        If TypeOf cboPaymentMethod.SelectedValue Is DataRowView Then Exit Sub

        Dim methodID As Integer
        If Not Integer.TryParse(cboPaymentMethod.SelectedValue.ToString(), methodID) Then Exit Sub

        Dim dv As New DataView(PaymentAccountsTable)

        dv.RowFilter = "PaymentMethodID = " & methodID

        cboCashAccount.DataSource = Nothing
        cboCashAccount.DataSource = dv
        cboCashAccount.DisplayMember = "DisplayName"
        cboCashAccount.ValueMember = "AccountID"

        cboCashAccount.SelectedIndex = -1

    End Sub
    Private Sub LoadOperationTypes()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable

            Using da As New SqlDataAdapter("
SELECT OperationTypeID, OperationName
FROM wf.OperationType
WHERE AffectsFinancials = 1 AND IsActive = 1
ORDER BY OperationTypeID
", con)

                da.Fill(dt)
            End Using

            cboOperationType.DataSource = dt
            cboOperationType.DisplayMember = "OperationName"
            cboOperationType.ValueMember = "OperationTypeID"

            cboOperationType.SelectedIndex = -1

        End Using

    End Sub
    Private Sub LoadAccountsForGrid()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable

            Using da As New SqlDataAdapter("
SELECT 
    a.AccountID,
    a.AccountNameAr,
    r.OperationTypeID,
    r.RuleType
FROM gl.Accounts a
JOIN cfg.AccountOperationRules r
    ON r.AccountID = a.AccountID
WHERE a.IsActive = 1
  AND r.IsAllowed = 1
", con)

                da.Fill(dt)
            End Using

            AccountsTable = dt

        End Using

    End Sub
    Private Sub FilterGridAccounts()

        If cboOperationType.SelectedValue Is Nothing Then Exit Sub
        If TypeOf cboOperationType.SelectedValue Is DataRowView Then Exit Sub
        If AccountsTable Is Nothing Then Exit Sub

        Dim opID As Integer = CInt(cboOperationType.SelectedValue)

        Dim dv As New DataView(AccountsTable)
        dv.RowFilter = "OperationTypeID = " & opID & " AND RuleType <> 'CASH'"

        Dim col = CType(dgvDetails.Columns("AccountName"), DataGridViewComboBoxColumn)
        col.DataSource = Nothing
        col.DataSource = dv
        col.DisplayMember = "AccountNameAr"
        col.ValueMember = "AccountID"

    End Sub
    Private Sub cboOperationType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboOperationType.SelectionChangeCommitted
        If IsLoading Then Exit Sub

        If cboOperationType.SelectedValue Is Nothing Then Exit Sub
        If TypeOf cboOperationType.SelectedValue Is DataRowView Then Exit Sub

        CurrentOperationTypeID = CInt(cboOperationType.SelectedValue)

        ApplyOperationBehavior()


    End Sub
    Private Sub dgvDetails_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvDetails.CurrentCellDirtyStateChanged
        If IsLoading Then Exit Sub
        If dgvDetails.IsCurrentCellDirty Then
            dgvDetails.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub
    Private Sub InitForm()
        Dim currentStatusID As Integer
        ' 🔵 تعيين القيم الافتراضية
        If cboPaymentMethod.Items.Count > 0 Then
            cboPaymentMethod.SelectedIndex = -1
        End If

        If cboCashAccount.Items.Count > 0 Then
            cboCashAccount.SelectedIndex = -1
        End If
        If PaymentAccountsTable IsNot Nothing Then
            cboCashAccount.DataSource = PaymentAccountsTable
            cboCashAccount.DisplayMember = "DisplayName"
            cboCashAccount.ValueMember = "AccountID"
        End If
        ' 🔵 إضافة سطر أول للجريد
        dgvDetails.Rows.Add()
        txtStatus.Text = "مسودة"
        currentStatusID = 1

    End Sub
    Private Function GetOperationMode(opID As Integer) As String

        Select Case opID

            Case 17 ' دفع مورد
                Return "SUPPLIER_PAYMENT"

            Case 16 ' قبض عميل
                Return "CUSTOMER_RECEIPT"

            Case 18, 19 ' سند عام
                Return "GENERAL"

            Case Else
                Return "LOCKED"

        End Select

    End Function
    Private Sub ApplyOperationBehavior()

        If cboOperationType.SelectedValue Is Nothing Then Exit Sub
        If TypeOf cboOperationType.SelectedValue Is DataRowView Then Exit Sub

        Dim opID As Integer = CInt(cboOperationType.SelectedValue)
        CurrentOperationTypeID = opID
        CurrentOperationMode = GetOperationMode(opID)

        ' تنظيف الجريد عند تغيير نوع العملية
        dgvDetails.Rows.Clear()

        ' إظهار / إخفاء إجمالي الفاتورة
        Dim showInvoiceTotal As Boolean = (CurrentOperationMode = "GENERAL")

        txtInvoiceTotal.Visible = showInvoiceTotal
        lblTotalInvoiceAmount.Visible = showInvoiceTotal
        txtTotalNet.Visible = showInvoiceTotal
        txtTotalTax.Visible = showInvoiceTotal
        lblTotalTax.Visible = showInvoiceTotal
        lblTotalNet.Visible = showInvoiceTotal

        Select Case CurrentOperationMode

            Case "SUPPLIER_PAYMENT", "CUSTOMER_RECEIPT"

                dgvDetails.Enabled = True
                dgvDetails.AllowUserToAddRows = False

                dgvDetails.Columns("IsTaxable").Visible = False
                dgvDetails.Columns("IsTaxIncluded").Visible = False
                dgvDetails.Columns("TaxRate").Visible = False
                dgvDetails.Columns("TaxAmount").Visible = False
                dgvDetails.Columns("TotalAmount").Visible = False
                dgvDetails.Columns("NetAmount").Visible = False
                dgvDetails.Columns("btnDelete").Visible = False

                dgvDetails.Rows.Add()

            Case "GENERAL"

                dgvDetails.Enabled = True
                dgvDetails.AllowUserToAddRows = True

                dgvDetails.Columns("IsTaxable").Visible = True
                dgvDetails.Columns("IsTaxIncluded").Visible = True
                dgvDetails.Columns("TaxRate").Visible = True
                dgvDetails.Columns("TaxAmount").Visible = True
                dgvDetails.Columns("TotalAmount").Visible = True
                dgvDetails.Columns("NetAmount").Visible = True
                dgvDetails.Columns("btnDelete").Visible = True

                dgvDetails.Rows.Add()

            Case Else

                dgvDetails.Enabled = False
                dgvDetails.AllowUserToAddRows = False

        End Select

        FilterGridAccounts()
        CalculateTotals()

    End Sub

    Private Sub dgvDetails_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvDetails.DataError
        e.ThrowException = False
    End Sub
    Private Sub dgvDetails_UserAddedRow(sender As Object, e As DataGridViewRowEventArgs) Handles dgvDetails.UserAddedRow

        If CurrentOperationMode = "SUPPLIER_PAYMENT" OrElse CurrentOperationMode = "CUSTOMER_RECEIPT" Then

            Dim realRows = dgvDetails.Rows.Cast(Of DataGridViewRow)().
            Count(Function(r) Not r.IsNewRow)

            If realRows > 1 Then
                dgvDetails.Rows.Remove(e.Row)
                MessageBox.Show("هذه العملية تسمح بسطر واحد فقط")
            End If

        End If

    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        Dim frm As New frmfinPaymentsSearch()

        frm.OperationTypeID = CurrentOperationTypeID ' 🔥 مهم

        If frm.ShowDialog() = DialogResult.OK Then

            If frm.SelectedTransactionID > 0 Then

                LoadTransaction(frm.SelectedTransactionID)

            End If

        End If

    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click

        ' 🔒 إيقاف الأحداث مؤقتاً
        IsLoading = True

        ' =========================
        ' 🧹 تصفير الحقول
        ' =========================

        txtDescription.Clear()
        txtTransactionNo.Clear()
        txtInvoiceTotal.Clear()

        txtTotalAmount.Text = "0.00"
        txtTotalNet.Text = "0.00"
        txtTotalTax.Text = "0.00"

        ' =========================
        ' 📅 التاريخ
        ' =========================
        dtpTransactionDate.Value = DateTime.Now

        ' =========================
        ' 🔽 الكمبوهات
        ' =========================

        cboOperationType.SelectedIndex = -1
        cboPaymentMethod.SelectedIndex = -1
        cboCashAccount.DataSource = Nothing

        ' =========================
        ' 📊 الجريد
        ' =========================

        dgvDetails.Rows.Clear()
        dgvDetails.Enabled = False

        ' =========================
        ' 📌 الحالة
        ' =========================

        txtStatus.Text = "مسودة"
        CurrentStatusID = 1

        ' =========================
        ' 🧠 متغيرات النظام
        ' =========================

        CurrentOperationTypeID = 0
        CurrentOperationMode = ""

        ' =========================
        ' 🔓 إعادة تفعيل الأحداث
        ' =========================

        IsLoading = False

    End Sub
    Private Sub dgvDetails_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDetails.CellClick

        If e.RowIndex < 0 Then Exit Sub

        If dgvDetails.Columns(e.ColumnIndex).Name = "AccountName" Then

            dgvDetails.BeginEdit(True)

            Dim cmb = TryCast(dgvDetails.EditingControl, ComboBox)

            If cmb IsNot Nothing Then
                cmb.DroppedDown = True
            End If

        End If

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' =========================
        ' 🔴 تحقق
        ' =========================
        ' 🔴 التحقق من تطابق إجمالي الفاتورة
        If CurrentOperationTypeID = 18 Or CurrentOperationTypeID = 19 Then

            Dim invoiceTotal As Decimal = ToDec(txtInvoiceTotal.Text)
            Dim entriesTotal As Decimal = ToDec(txtTotalAmount.Text)

            If Not Decimal.TryParse(txtInvoiceTotal.Text, invoiceTotal) Then
                MessageBox.Show("❌ أدخل رقم صحيح لإجمالي الفاتورة")
                txtInvoiceTotal.Focus()
                Exit Sub
            End If
            If Math.Abs(invoiceTotal - entriesTotal) > 0.001D Then

                MessageBox.Show("❌ إجمالي الفاتورة لا يساوي إجمالي القيود")

                txtInvoiceTotal.Focus()
                Exit Sub

            End If

        End If
        If IsSaved AndAlso Not (CurrentStatusID = 1 Or CurrentStatusID = 2 Or CurrentStatusID = 5) Then
            MessageBox.Show("⚠️ لا يمكن تعديل هذا السند في هذه الحالة")
            Exit Sub
        End If
        If cboCashAccount.SelectedValue Is Nothing Then
            MessageBox.Show("اختر حساب الكاش")
            Exit Sub
        End If

        If cboPaymentMethod.SelectedValue Is Nothing Then
            MessageBox.Show("اختر طريقة الدفع")
            Exit Sub
        End If

        If CurrentOperationTypeID = 0 Then
            MessageBox.Show("اختر نوع العملية")
            Exit Sub
        End If

        ' =========================
        ' 🔵 إنشاء الموديل
        ' =========================
        Dim model As New finCashTransactionModel()

        Dim codeType = GetOperationCode(CurrentOperationTypeID)
        Dim newNo = GetNextTransactionNo(codeType)

        model.TransactionNo = newNo
        txtTransactionNo.Text = newNo

        model.TransactionDate = dtpTransactionDate.Value
        model.OperationTypeID = CurrentOperationTypeID
        model.Direction = If(CurrentOperationTypeID = 16 Or CurrentOperationTypeID = 18, 1, 2)

        model.CashAccountID = CInt(cboCashAccount.SelectedValue)
        model.PaymentMethodID = CInt(cboPaymentMethod.SelectedValue)

        model.Description = txtDescription.Text
        model.ReferenceNo = ""

        model.CurrencyID = 1
        model.ExchangeRate = 1

        model.StatusID = 2
        model.CreatedBy = 1

        model.Details = New List(Of finCashTransactionDetailModel)

        ' =========================
        ' 🟢 قراءة الجريد
        ' =========================
        For Each row As DataGridViewRow In dgvDetails.Rows

            If row.IsNewRow Then Continue For
            If row.Cells("AccountID").Value Is Nothing _
   OrElse Not IsNumeric(row.Cells("AccountID").Value) _
   OrElse CInt(row.Cells("AccountID").Value) = 0 Then

                MessageBox.Show("❌ يوجد سطر بدون حساب صحيح")
                Exit Sub

            End If

            Dim d As New finCashTransactionDetailModel()
            d.AccountID = CInt(row.Cells("AccountID").Value)
            Dim entrySide = GetEntrySide(d.AccountID, model.OperationTypeID)

            If entrySide Is Nothing Then
                MessageBox.Show("⚠️ الحساب " & d.AccountID & " ليس له تعريف اتجاه")
                Exit Sub
            End If

            Dim amount As Decimal = ToDec(row.Cells("Amount").Value)
            Dim tax As Decimal = ToDec(row.Cells("TaxAmount").Value)
            Dim net As Decimal = ToDec(row.Cells("NetAmount").Value)
            Dim isTaxIncluded As Boolean = ToBool(row.Cells("IsTaxIncluded").Value)

            ' 🔥 معالجة الضريبة
            If isTaxIncluded Then
                d.Amount = net
            Else
                d.Amount = amount
            End If

            d.TaxAmount = tax
            d.NetAmount = net
            d.IsTaxIncluded = isTaxIncluded
            d.IsTaxable = ToBool(row.Cells("IsTaxable").Value)

            ' 🔥 هذا السطر الجديد
            d.TaxRate = VAT_RATE * 100

            d.Description = row.Cells("Description").Value?.ToString()
            ' 🔵 جلب البارتنر
            Dim partnerData = GetPartnerByAccount(d.AccountID)

            If partnerData.Item1.HasValue Then
                d.PartnerID = partnerData.Item1
                d.PartnerTypeID = partnerData.Item2
            End If

            model.Details.Add(d)

        Next

        ' =========================
        ' 🔵 إضافة الضريبة كسطر مستقل
        ' =========================
        Dim totalTax As Decimal = model.Details.Sum(Function(x) x.TaxAmount)

        If totalTax > 0 Then

            Dim firstTaxLine = model.Details.
        FirstOrDefault(Function(x) x.TaxAmount > 0)

            If firstTaxLine Is Nothing Then Exit Sub
            Dim taxTypeId As Integer

            If model.Direction = 2 Then
                taxTypeId = 1 ' Input VAT
            Else
                taxTypeId = 4 ' Output VAT
            End If

            Dim taxInfo = GetTaxInfo(taxTypeId)
            Dim taxLine As New finCashTransactionDetailModel()

            taxLine.AccountID = taxInfo.AccountID
            taxLine.Amount = totalTax

            If taxInfo.Direction = 1 Then
                taxLine.DebitAmount = totalTax
                taxLine.CreditAmount = 0
            Else
                taxLine.DebitAmount = 0
                taxLine.CreditAmount = totalTax
            End If

            taxLine.Description = "ضريبة"

            model.Details.Add(taxLine)

        End If
        ' =========================
        ' 🔵 تحديد Debit / Credit من الجدول
        ' =========================
        For Each d In model.Details

            Dim entrySide = GetEntrySide(d.AccountID, model.OperationTypeID)

            If entrySide = 1 Then
                d.DebitAmount = d.Amount
                d.CreditAmount = 0
            Else
                d.DebitAmount = 0
                d.CreditAmount = d.Amount
            End If

        Next

        ' =========================
        ' 🔵 إضافة الكاش (موازنة)
        ' =========================
        Dim totalDebit As Decimal = model.Details.Sum(Function(x) x.DebitAmount)
        Dim totalCredit As Decimal = model.Details.Sum(Function(x) x.CreditAmount)

        Dim cashLine As New finCashTransactionDetailModel()

        cashLine.AccountID = model.CashAccountID

        cashLine.DebitAmount = totalCredit
        cashLine.CreditAmount = totalDebit

        cashLine.Amount = Math.Abs(totalDebit - totalCredit)
        cashLine.Description = "كاش"

        model.Details.Add(cashLine)

        ' =========================
        ' 🔵 إجمالي السند
        ' =========================
        model.TotalAmount =
    model.Details.
    Where(Function(x) x.AccountID <> model.CashAccountID).
    Sum(Function(x) x.Amount)

        ' =========================
        ' 🔵 الحفظ
        ' =========================
        Dim service As New finCashTransactionService()

        Dim id As Integer

        If IsSaved Then
            ' 🔵 تعديل
            model.TransactionID = CurrentTransactionID
            id = service.UpdateTransaction(model)
        Else
            ' 🔵 جديد
            id = service.SaveTransaction(model)
        End If

        CurrentTransactionID = id
        IsSaved = True
        CurrentStatusID = 2

        LoadTransaction(id)

        UpdateButtons()
        btnSave.Enabled = False
        MessageBox.Show("✅ تم الحفظ بنجاح")

    End Sub
    Private Function GetNextTransactionNo(codeType As String) As String

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("cfg.GetNextCode", con)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@CodeType", codeType)

                Dim outParam As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                outParam.Direction = ParameterDirection.Output
                cmd.Parameters.Add(outParam)

                cmd.ExecuteNonQuery()

                Return outParam.Value.ToString()
            End Using

        End Using

    End Function
    Private Function GetOperationCode(opID As Integer) As String

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
SELECT OperationCode 
FROM wf.OperationType 
WHERE OperationTypeID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", opID)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then
                    Throw New Exception("لم يتم العثور على نوع العملية")
                End If

                Return result.ToString()

            End Using
        End Using

    End Function
    Private Function GetPartnerByAccount(accountId As Integer) As (Integer?, Integer?)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
SELECT PartnerID, PartnerTypeID
FROM md.Partner
WHERE AccountID = @AccountID
", con)

                cmd.Parameters.AddWithValue("@AccountID", accountId)

                Using rd = cmd.ExecuteReader()

                    If rd.Read() Then

                        Dim partnerId = If(rd("PartnerID") Is DBNull.Value, Nothing, CType(rd("PartnerID"), Integer?))
                        Dim partnerType = If(rd("PartnerTypeID") Is DBNull.Value, Nothing, CType(rd("PartnerTypeID"), Integer?))

                        Return (partnerId, partnerType)

                    End If

                End Using

            End Using
        End Using

        Return (Nothing, Nothing)

    End Function
    Private Function GetAccountNature(accountId As Integer) As Integer

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
SELECT AccountNatureID
FROM gl.Accounts
WHERE AccountID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", accountId)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then Return 0

                Return CInt(result)

            End Using
        End Using

    End Function
    Private Function GetEntrySide(accountId As Integer, opID As Integer) As Integer?

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
SELECT EntrySideID
FROM cfg.AccountEntryRules
WHERE AccountID = @AccountID
AND OperationTypeID = @OperationTypeID
AND IsActive = 1
", con)

                cmd.Parameters.AddWithValue("@AccountID", accountId)
                cmd.Parameters.AddWithValue("@OperationTypeID", opID)

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then
                    Return Nothing ' 👈 بدل الانهيار
                End If

                Return CInt(result)

            End Using
        End Using

    End Function

    Private Function GetTaxInfo(taxTypeId As Integer) As (AccountID As Integer, Direction As Integer)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
SELECT TaxAccountID, TaxDirection
FROM md.TaxType
WHERE TaxTypeID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", taxTypeId)

                Using dr = cmd.ExecuteReader()

                    If dr.Read() Then
                        Return (CInt(dr("TaxAccountID")), CInt(dr("TaxDirection")))
                    Else
                        Throw New Exception("❌ نوع الضريبة غير معرف")
                    End If

                End Using

            End Using
        End Using

    End Function

    Private Sub LoadTransaction(id As Integer)

        Dim service As New finCashTransactionService()

        ' =========================
        ' 🔵 الهيدر
        ' =========================
        Dim header = service.GetTransactionHeader(id)

        If header Is Nothing Then Exit Sub

        txtTransactionNo.Text = header("TransactionNo").ToString()
        dtpTransactionDate.Value = CDate(header("TransactionDate"))

        cboPaymentMethod.SelectedValue = header("PaymentMethodID")

        ' 🔥 هنا بالضبط
        If cboPaymentMethod.SelectedValue IsNot Nothing Then
            FilterAccountsByPaymentMethod()
        End If

        cboCashAccount.SelectedValue = header("CashAccountID")


        txtDescription.Text = header("Description").ToString()

        CurrentStatusID = CInt(header("StatusID"))
        CurrentTransactionID = id

        txtStatus.Text = GetStatusName(CurrentStatusID)

        ' =========================
        ' 🔵 التفاصيل
        ' =========================
        Dim dtDetails = service.GetTransactionDetails(id)

        dgvDetails.Rows.Clear()

        For Each r As DataRow In dtDetails.Rows

            ' ❌ تجاهل الكاش والضريبة (اختياري)
            If r("Description").ToString() = "كاش" Then Continue For
            If r("Description").ToString() = "ضريبة" Then Continue For
            '
            Dim i = dgvDetails.Rows.Add()

            With dgvDetails.Rows(i)

                .Cells("AccountID").Value = r("AccountID")
                .Cells("AccountName").Value = r("AccountID")

                .Cells("Amount").Value = r("Amount")
                .Cells("IsTaxable").Value = r("IsTaxable")
                .Cells("IsTaxIncluded").Value = r("IsTaxIncluded")
                .Cells("TaxRate").Value = r("TaxRate")
                .Cells("TaxAmount").Value = r("TaxAmount")
                .Cells("NetAmount").Value = r("NetAmount")

                .Cells("TotalAmount").Value =
        CDec(r("NetAmount")) + CDec(r("TaxAmount"))

                .Cells("Description").Value = r("Description")

            End With

        Next

        UpdateButtons()

    End Sub
    Private Sub SendTransaction()

        Dim service As New finCashTransactionService()
        service.UpdateStatus(CurrentTransactionID, 5)

        CurrentStatusID = 5
        txtStatus.Text = "مرسل"

        UpdateButtons()

    End Sub
    Private Sub RejectTransaction()

        Dim service As New finCashTransactionService()
        service.UpdateStatus(CurrentTransactionID, 27)

        CurrentStatusID = 27
        txtStatus.Text = "مرفوض"

        UpdateButtons()

    End Sub
    Private Sub CancelTransaction()

        Dim service As New finCashTransactionService()
        service.UpdateStatus(CurrentTransactionID, 10)

        CurrentStatusID = 10
        txtStatus.Text = "ملغي"

        UpdateButtons()

    End Sub
    Private Sub UpdateButtons()

        btnSave.Enabled = False
        btnSend.Enabled = False
        btnPost.Enabled = False
        btnReject.Enabled = False
        btnCancel.Enabled = False

        Select Case CurrentStatusID

            Case 1 ' DRAFT
                btnSave.Enabled = True
                btnCancel.Enabled = True

            Case 2 ' NEW
                btnSave.Enabled = True
                btnSave.Text = "تعديل"
                btnSend.Enabled = True
                btnCancel.Enabled = True

            Case 5 ' SENT
                btnPost.Enabled = True
                btnReject.Enabled = True
                btnCancel.Enabled = True

            Case 27 ' REJECTED
                btnSave.Enabled = True
                btnSend.Enabled = True
                btnCancel.Enabled = True

        End Select
        ' 🔥 قفل التعديل بعد الإرسال
        Dim isEditable As Boolean = (CurrentStatusID = 1 Or CurrentStatusID = 2)

        cboPaymentMethod.Enabled = isEditable
        cboCashAccount.Enabled = isEditable
        txtTotalAmount.ReadOnly = Not isEditable
        txtDescription.ReadOnly = Not isEditable
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        If CurrentTransactionID = 0 Then
            MessageBox.Show("❌ يجب حفظ السند أولاً")
            Exit Sub
        End If

        If CurrentStatusID <> 2 Then
            MessageBox.Show("❌ لا يمكن إرسال السند في هذه الحالة")
            Exit Sub
        End If

        SendTransaction()
        MessageBox.Show("تم ارسال السند للمراجعة المالية")

    End Sub
    Private Sub btnPost_Click(sender As Object, e As EventArgs) Handles btnPost.Click

        If CurrentTransactionID = 0 Then
            MessageBox.Show("❌ لا يوجد سند")
            Exit Sub
        End If

        If CurrentStatusID <> 5 Then
            MessageBox.Show("❌ يجب أن يكون السند مرسل قبل الترحيل")
            Exit Sub
        End If

        If Not CurrentUser.IsLoggedIn() Then
            MessageBox.Show("❌ المستخدم غير مسجل")
            Exit Sub
        End If

        Try

            Using con As New SqlConnection(AppConfig.MainConnectionString)
                con.Open()

                Using tran = con.BeginTransaction()

                    Dim service As New FinPostingService()

                    service.PostPayment(
                    CurrentTransactionID,
                    CurrentUser.EmployeeID,
                    con,
                    tran
                )

                    tran.Commit()

                End Using
            End Using

            ' 🔵 تحديث الحالة
            CurrentStatusID = 24
            txtStatus.Text = "مرحل"

            UpdateButtons()

            MessageBox.Show("✅ تم ترحيل السند مالياً")

        Catch ex As Exception

            MessageBox.Show("❌ خطأ أثناء الترحيل: " & ex.Message)

        End Try

    End Sub
    Private Sub btnReject_Click(sender As Object, e As EventArgs) Handles btnReject.Click

        If CurrentTransactionID = 0 Then
            MessageBox.Show("❌ لا يوجد سند")
            Exit Sub
        End If

        If CurrentStatusID <> 5 Then
            MessageBox.Show("❌ لا يمكن رفض السند إلا إذا كان مرسل")
            Exit Sub
        End If

        If MessageBox.Show("هل تريد رفض السند؟", "تأكيد", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            RejectTransaction()
        End If
        MessageBox.Show("تم رفض السند ويستطيع منشئ السند مراجعته واعادة ارساله او الغائه")

    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        If CurrentTransactionID = 0 Then
            MessageBox.Show("❌ لا يوجد سند")
            Exit Sub
        End If

        ' 🔴 السماح فقط في هذه الحالات
        If Not (CurrentStatusID = 1 Or CurrentStatusID = 2 Or CurrentStatusID = 5) Then
            MessageBox.Show("❌ لا يمكن إلغاء السند في هذه الحالة")
            Exit Sub
        End If

        If MessageBox.Show("هل تريد إلغاء السند؟", "تأكيد", MessageBoxButtons.YesNo) = DialogResult.Yes Then

            CancelTransaction()

            MessageBox.Show("✅ تم إلغاء السند")

        End If

    End Sub
End Class
