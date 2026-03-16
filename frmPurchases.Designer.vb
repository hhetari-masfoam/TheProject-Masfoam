<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPurchases
    Inherits AABaseOperationForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tabOldInvoices = New System.Windows.Forms.TabPage()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.cboTargetStore = New System.Windows.Forms.ComboBox()
        Me.cboPaymentMethod = New System.Windows.Forms.ComboBox()
        Me.btnAddPartner = New System.Windows.Forms.Button()
        Me.btnFindPartner = New System.Windows.Forms.Button()
        Me.cboSource = New System.Windows.Forms.ComboBox()
        Me.cboPaymentTerm = New System.Windows.Forms.ComboBox()
        Me.dtpDueDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpDocumentDate = New System.Windows.Forms.DateTimePicker()
        Me.txtGrandTotal = New System.Windows.Forms.TextBox()
        Me.txtVATTotal = New System.Windows.Forms.TextBox()
        Me.txtSubTotal = New System.Windows.Forms.TextBox()
        Me.cboPartnerCode = New System.Windows.Forms.ComboBox()
        Me.txtNote = New System.Windows.Forms.TextBox()
        Me.dgvInvoiceDetails = New System.Windows.Forms.DataGridView()
        Me.colProductSearch = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitID = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colTaxableAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVATRate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVATAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTotalAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.btnSaveDraft = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.tab1 = New System.Windows.Forms.TabControl()
        Me.TabInvoice = New System.Windows.Forms.TabPage()
        Me.cboProductFilter = New System.Windows.Forms.ComboBox()
        Me.txtExchangeRate = New System.Windows.Forms.TextBox()
        Me.txtCurrencyID = New System.Windows.Forms.TextBox()
        Me.cboDocumentID = New System.Windows.Forms.ComboBox()
        Me.cboVATRate = New System.Windows.Forms.ComboBox()
        Me.chkIsTaxInclusive = New System.Windows.Forms.CheckBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtVATRegistrationNumber = New System.Windows.Forms.TextBox()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.txtPartnerCode = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.tab1.SuspendLayout()
        Me.TabInvoice.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabOldInvoices
        '
        Me.tabOldInvoices.Location = New System.Drawing.Point(4, 34)
        Me.tabOldInvoices.Margin = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Name = "tabOldInvoices"
        Me.tabOldInvoices.Padding = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Size = New System.Drawing.Size(1317, 921)
        Me.tabOldInvoices.TabIndex = 1
        Me.tabOldInvoices.Text = "فواتير سابقة"
        Me.tabOldInvoices.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.SystemColors.Control
        Me.btnNew.Location = New System.Drawing.Point(21, 452)
        Me.btnNew.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(85, 29)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "New"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnSend
        '
        Me.btnSend.BackColor = System.Drawing.SystemColors.Control
        Me.btnSend.Location = New System.Drawing.Point(21, 489)
        Me.btnSend.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(85, 29)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "ارسال"
        Me.btnSend.UseVisualStyleBackColor = False
        '
        'cboTargetStore
        '
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(8, 77)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(189, 24)
        Me.cboTargetStore.TabIndex = 17
        '
        'cboPaymentMethod
        '
        Me.cboPaymentMethod.FormattingEnabled = True
        Me.cboPaymentMethod.Location = New System.Drawing.Point(865, 74)
        Me.cboPaymentMethod.Name = "cboPaymentMethod"
        Me.cboPaymentMethod.Size = New System.Drawing.Size(172, 24)
        Me.cboPaymentMethod.TabIndex = 15
        '
        'btnAddPartner
        '
        Me.btnAddPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnAddPartner.Location = New System.Drawing.Point(454, 7)
        Me.btnAddPartner.Name = "btnAddPartner"
        Me.btnAddPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnAddPartner.TabIndex = 14
        Me.btnAddPartner.Text = "Add"
        Me.btnAddPartner.UseVisualStyleBackColor = False
        '
        'btnFindPartner
        '
        Me.btnFindPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnFindPartner.Location = New System.Drawing.Point(508, 7)
        Me.btnFindPartner.Name = "btnFindPartner"
        Me.btnFindPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnFindPartner.TabIndex = 14
        Me.btnFindPartner.Text = "Find"
        Me.btnFindPartner.UseVisualStyleBackColor = False
        '
        'cboSource
        '
        Me.cboSource.FormattingEnabled = True
        Me.cboSource.Location = New System.Drawing.Point(865, 208)
        Me.cboSource.Name = "cboSource"
        Me.cboSource.Size = New System.Drawing.Size(172, 24)
        Me.cboSource.TabIndex = 13
        '
        'cboPaymentTerm
        '
        Me.cboPaymentTerm.FormattingEnabled = True
        Me.cboPaymentTerm.Location = New System.Drawing.Point(865, 122)
        Me.cboPaymentTerm.Name = "cboPaymentTerm"
        Me.cboPaymentTerm.Size = New System.Drawing.Size(172, 24)
        Me.cboPaymentTerm.TabIndex = 11
        '
        'dtpDueDate
        '
        Me.dtpDueDate.Location = New System.Drawing.Point(865, 163)
        Me.dtpDueDate.Name = "dtpDueDate"
        Me.dtpDueDate.Size = New System.Drawing.Size(172, 24)
        Me.dtpDueDate.TabIndex = 10
        '
        'dtpDocumentDate
        '
        Me.dtpDocumentDate.Location = New System.Drawing.Point(865, 47)
        Me.dtpDocumentDate.Name = "dtpDocumentDate"
        Me.dtpDocumentDate.Size = New System.Drawing.Size(172, 24)
        Me.dtpDocumentDate.TabIndex = 9
        '
        'txtGrandTotal
        '
        Me.txtGrandTotal.Enabled = False
        Me.txtGrandTotal.Location = New System.Drawing.Point(8, 265)
        Me.txtGrandTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGrandTotal.Name = "txtGrandTotal"
        Me.txtGrandTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtGrandTotal.TabIndex = 7
        '
        'txtVATTotal
        '
        Me.txtVATTotal.Enabled = False
        Me.txtVATTotal.Location = New System.Drawing.Point(8, 230)
        Me.txtVATTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATTotal.Name = "txtVATTotal"
        Me.txtVATTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtVATTotal.TabIndex = 7
        '
        'txtSubTotal
        '
        Me.txtSubTotal.Enabled = False
        Me.txtSubTotal.Location = New System.Drawing.Point(8, 195)
        Me.txtSubTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSubTotal.Name = "txtSubTotal"
        Me.txtSubTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtSubTotal.TabIndex = 7
        '
        'cboPartnerCode
        '
        Me.cboPartnerCode.FormattingEnabled = True
        Me.cboPartnerCode.Location = New System.Drawing.Point(564, 7)
        Me.cboPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.cboPartnerCode.Name = "cboPartnerCode"
        Me.cboPartnerCode.Size = New System.Drawing.Size(157, 24)
        Me.cboPartnerCode.TabIndex = 6
        '
        'txtNote
        '
        Me.txtNote.Location = New System.Drawing.Point(454, 208)
        Me.txtNote.Margin = New System.Windows.Forms.Padding(4)
        Me.txtNote.Multiline = True
        Me.txtNote.Name = "txtNote"
        Me.txtNote.Size = New System.Drawing.Size(267, 77)
        Me.txtNote.TabIndex = 5
        '
        'dgvInvoiceDetails
        '
        Me.dgvInvoiceDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvInvoiceDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInvoiceDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductSearch, Me.colProductCode, Me.colProductType, Me.colProductID, Me.colProductName, Me.colUnitPrice, Me.colQty, Me.colUnitID, Me.colTaxableAmount, Me.colVATRate, Me.colVATAmount, Me.colTotalAmount})
        Me.dgvInvoiceDetails.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.dgvInvoiceDetails.Location = New System.Drawing.Point(4, 455)
        Me.dgvInvoiceDetails.Margin = New System.Windows.Forms.Padding(4)
        Me.dgvInvoiceDetails.Name = "dgvInvoiceDetails"
        Me.dgvInvoiceDetails.RowHeadersWidth = 51
        Me.dgvInvoiceDetails.RowTemplate.Height = 26
        Me.dgvInvoiceDetails.Size = New System.Drawing.Size(1309, 462)
        Me.dgvInvoiceDetails.TabIndex = 3
        '
        'colProductSearch
        '
        Me.colProductSearch.HeaderText = "بحث"
        Me.colProductSearch.MinimumWidth = 6
        Me.colProductSearch.Name = "colProductSearch"
        Me.colProductSearch.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductSearch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colProductCode
        '
        Me.colProductCode.HeaderText = "كود الصنف"
        Me.colProductCode.MinimumWidth = 6
        Me.colProductCode.Name = "colProductCode"
        '
        'colProductType
        '
        Me.colProductType.HeaderText = "النوع"
        Me.colProductType.MinimumWidth = 6
        Me.colProductType.Name = "colProductType"
        Me.colProductType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colProductID
        '
        Me.colProductID.HeaderText = "رقم الصنف"
        Me.colProductID.MinimumWidth = 6
        Me.colProductID.Name = "colProductID"
        Me.colProductID.ReadOnly = True
        Me.colProductID.Visible = False
        '
        'colProductName
        '
        Me.colProductName.HeaderText = "اسم الصنف"
        Me.colProductName.MinimumWidth = 6
        Me.colProductName.Name = "colProductName"
        Me.colProductName.ReadOnly = True
        '
        'colUnitPrice
        '
        Me.colUnitPrice.HeaderText = "السعر"
        Me.colUnitPrice.MinimumWidth = 6
        Me.colUnitPrice.Name = "colUnitPrice"
        '
        'colQty
        '
        Me.colQty.HeaderText = "الكمية"
        Me.colQty.MinimumWidth = 6
        Me.colQty.Name = "colQty"
        '
        'colUnitID
        '
        Me.colUnitID.HeaderText = "الوحدة"
        Me.colUnitID.MinimumWidth = 6
        Me.colUnitID.Name = "colUnitID"
        Me.colUnitID.ReadOnly = True
        '
        'colTaxableAmount
        '
        Me.colTaxableAmount.HeaderText = "المبلغ"
        Me.colTaxableAmount.MinimumWidth = 6
        Me.colTaxableAmount.Name = "colTaxableAmount"
        Me.colTaxableAmount.ReadOnly = True
        '
        'colVATRate
        '
        Me.colVATRate.DataPropertyName = "TaxRate"
        Me.colVATRate.HeaderText = "الضريبة"
        Me.colVATRate.MinimumWidth = 6
        Me.colVATRate.Name = "colVATRate"
        Me.colVATRate.ReadOnly = True
        '
        'colVATAmount
        '
        Me.colVATAmount.HeaderText = "مبلغ الضريبة"
        Me.colVATAmount.MinimumWidth = 6
        Me.colVATAmount.Name = "colVATAmount"
        Me.colVATAmount.ReadOnly = True
        '
        'colTotalAmount
        '
        Me.colTotalAmount.HeaderText = "الاجمالي"
        Me.colTotalAmount.MinimumWidth = 6
        Me.colTotalAmount.Name = "colTotalAmount"
        Me.colTotalAmount.ReadOnly = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(740, 177)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 17)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "الرقم الضريبي"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(761, 135)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 17)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "رقم الهاتف"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(783, 94)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 17)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "العنوان"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(270, 265)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(98, 17)
        Me.Label14.TabIndex = 1
        Me.Label14.Text = "المبلغ الاجمالي"
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Location = New System.Drawing.Point(21, 524)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(85, 29)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Location = New System.Drawing.Point(21, 572)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(85, 29)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "Find"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.Info
        Me.Panel2.Controls.Add(Me.txtStatusName)
        Me.Panel2.Controls.Add(Me.btnSaveDraft)
        Me.Panel2.Controls.Add(Me.btnNew)
        Me.Panel2.Controls.Add(Me.btnSend)
        Me.Panel2.Controls.Add(Me.btnCancel)
        Me.Panel2.Controls.Add(Me.btnSearch)
        Me.Panel2.Controls.Add(Me.btnPrint)
        Me.Panel2.Controls.Add(Me.btnClose)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(1325, 0)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(124, 959)
        Me.Panel2.TabIndex = 3
        '
        'txtStatusName
        '
        Me.txtStatusName.Enabled = False
        Me.txtStatusName.Location = New System.Drawing.Point(5, 12)
        Me.txtStatusName.Name = "txtStatusName"
        Me.txtStatusName.Size = New System.Drawing.Size(114, 24)
        Me.txtStatusName.TabIndex = 22
        '
        'btnSaveDraft
        '
        Me.btnSaveDraft.Location = New System.Drawing.Point(21, 701)
        Me.btnSaveDraft.Name = "btnSaveDraft"
        Me.btnSaveDraft.Size = New System.Drawing.Size(85, 29)
        Me.btnSaveDraft.TabIndex = 1
        Me.btnSaveDraft.Text = "حفظ"
        Me.btnSaveDraft.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Location = New System.Drawing.Point(21, 609)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(85, 29)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Location = New System.Drawing.Point(21, 644)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(85, 29)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(268, 230)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(96, 17)
        Me.Label13.TabIndex = 1
        Me.Label13.Text = "اجمالي الضريبة"
        '
        'tab1
        '
        Me.tab1.Controls.Add(Me.TabInvoice)
        Me.tab1.Controls.Add(Me.tabOldInvoices)
        Me.tab1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tab1.ItemSize = New System.Drawing.Size(150, 30)
        Me.tab1.Location = New System.Drawing.Point(0, 0)
        Me.tab1.Margin = New System.Windows.Forms.Padding(4)
        Me.tab1.Name = "tab1"
        Me.tab1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tab1.RightToLeftLayout = True
        Me.tab1.SelectedIndex = 0
        Me.tab1.Size = New System.Drawing.Size(1325, 959)
        Me.tab1.TabIndex = 0
        Me.tab1.Tag = ""
        '
        'TabInvoice
        '
        Me.TabInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TabInvoice.Controls.Add(Me.cboProductFilter)
        Me.TabInvoice.Controls.Add(Me.txtExchangeRate)
        Me.TabInvoice.Controls.Add(Me.txtCurrencyID)
        Me.TabInvoice.Controls.Add(Me.cboDocumentID)
        Me.TabInvoice.Controls.Add(Me.cboVATRate)
        Me.TabInvoice.Controls.Add(Me.chkIsTaxInclusive)
        Me.TabInvoice.Controls.Add(Me.cboTargetStore)
        Me.TabInvoice.Controls.Add(Me.cboPaymentMethod)
        Me.TabInvoice.Controls.Add(Me.btnAddPartner)
        Me.TabInvoice.Controls.Add(Me.btnFindPartner)
        Me.TabInvoice.Controls.Add(Me.cboSource)
        Me.TabInvoice.Controls.Add(Me.cboPaymentTerm)
        Me.TabInvoice.Controls.Add(Me.dtpDueDate)
        Me.TabInvoice.Controls.Add(Me.dtpDocumentDate)
        Me.TabInvoice.Controls.Add(Me.txtGrandTotal)
        Me.TabInvoice.Controls.Add(Me.txtVATTotal)
        Me.TabInvoice.Controls.Add(Me.txtSubTotal)
        Me.TabInvoice.Controls.Add(Me.cboPartnerCode)
        Me.TabInvoice.Controls.Add(Me.txtNote)
        Me.TabInvoice.Controls.Add(Me.dgvInvoiceDetails)
        Me.TabInvoice.Controls.Add(Me.Label6)
        Me.TabInvoice.Controls.Add(Me.Label5)
        Me.TabInvoice.Controls.Add(Me.Label4)
        Me.TabInvoice.Controls.Add(Me.Label14)
        Me.TabInvoice.Controls.Add(Me.Label13)
        Me.TabInvoice.Controls.Add(Me.Label17)
        Me.TabInvoice.Controls.Add(Me.Label12)
        Me.TabInvoice.Controls.Add(Me.Label3)
        Me.TabInvoice.Controls.Add(Me.Label15)
        Me.TabInvoice.Controls.Add(Me.Label2)
        Me.TabInvoice.Controls.Add(Me.Label19)
        Me.TabInvoice.Controls.Add(Me.Label1)
        Me.TabInvoice.Controls.Add(Me.Label18)
        Me.TabInvoice.Controls.Add(Me.Label16)
        Me.TabInvoice.Controls.Add(Me.Label11)
        Me.TabInvoice.Controls.Add(Me.Label10)
        Me.TabInvoice.Controls.Add(Me.Label9)
        Me.TabInvoice.Controls.Add(Me.Label8)
        Me.TabInvoice.Controls.Add(Me.Label7)
        Me.TabInvoice.Controls.Add(Me.txtVATRegistrationNumber)
        Me.TabInvoice.Controls.Add(Me.txtPhone)
        Me.TabInvoice.Controls.Add(Me.txtCity)
        Me.TabInvoice.Controls.Add(Me.txtAddress)
        Me.TabInvoice.Controls.Add(Me.txtPartnerCode)
        Me.TabInvoice.Location = New System.Drawing.Point(4, 34)
        Me.TabInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.Name = "TabInvoice"
        Me.TabInvoice.Padding = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.TabInvoice.Size = New System.Drawing.Size(1317, 921)
        Me.TabInvoice.TabIndex = 0
        Me.TabInvoice.Text = "الفاتورة"
        '
        'cboProductFilter
        '
        Me.cboProductFilter.FormattingEnabled = True
        Me.cboProductFilter.Location = New System.Drawing.Point(865, 293)
        Me.cboProductFilter.Name = "cboProductFilter"
        Me.cboProductFilter.Size = New System.Drawing.Size(172, 24)
        Me.cboProductFilter.TabIndex = 22
        '
        'txtExchangeRate
        '
        Me.txtExchangeRate.Location = New System.Drawing.Point(25, 147)
        Me.txtExchangeRate.Name = "txtExchangeRate"
        Me.txtExchangeRate.Size = New System.Drawing.Size(100, 24)
        Me.txtExchangeRate.TabIndex = 21
        Me.txtExchangeRate.Visible = False
        '
        'txtCurrencyID
        '
        Me.txtCurrencyID.Location = New System.Drawing.Point(25, 117)
        Me.txtCurrencyID.Name = "txtCurrencyID"
        Me.txtCurrencyID.Size = New System.Drawing.Size(100, 24)
        Me.txtCurrencyID.TabIndex = 21
        Me.txtCurrencyID.Visible = False
        '
        'cboDocumentID
        '
        Me.cboDocumentID.FormattingEnabled = True
        Me.cboDocumentID.Location = New System.Drawing.Point(865, 9)
        Me.cboDocumentID.Name = "cboDocumentID"
        Me.cboDocumentID.Size = New System.Drawing.Size(172, 24)
        Me.cboDocumentID.TabIndex = 20
        '
        'cboVATRate
        '
        Me.cboVATRate.FormattingEnabled = True
        Me.cboVATRate.Location = New System.Drawing.Point(953, 263)
        Me.cboVATRate.Name = "cboVATRate"
        Me.cboVATRate.Size = New System.Drawing.Size(84, 24)
        Me.cboVATRate.TabIndex = 19
        '
        'chkIsTaxInclusive
        '
        Me.chkIsTaxInclusive.AutoSize = True
        Me.chkIsTaxInclusive.Location = New System.Drawing.Point(865, 267)
        Me.chkIsTaxInclusive.Name = "chkIsTaxInclusive"
        Me.chkIsTaxInclusive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsTaxInclusive.TabIndex = 18
        Me.chkIsTaxInclusive.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(247, 77)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(120, 17)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "المستودع المستلم"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(290, 38)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(72, 17)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "حالة الطلب"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(241, 195)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 17)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "اجمالي قبل الضريبة"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(761, 11)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(66, 17)
        Me.Label15.TabIndex = 1
        Me.Label15.Text = "كود المورد"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(759, 53)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 17)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "إسم المورد"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(897, 268)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(43, 17)
        Me.Label19.TabIndex = 1
        Me.Label19.Text = "شامل"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1113, 300)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(116, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "الية فلترة المنتجات"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(1113, 272)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(49, 17)
        Me.Label18.TabIndex = 1
        Me.Label18.Text = "الضريبة"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(1083, 80)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(79, 17)
        Me.Label16.TabIndex = 1
        Me.Label16.Text = "طريقة الدفع "
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(1058, 214)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(104, 17)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "مصدر المشتريات"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(1088, 128)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(75, 17)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "طبيعة الدفع"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(1069, 168)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(103, 17)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "تاريخ الاستحقاق"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(1081, 51)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(83, 17)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = "تاريخ الفاتورة "
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(1093, 11)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(73, 17)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "رقم الفاتورة"
        '
        'txtVATRegistrationNumber
        '
        Me.txtVATRegistrationNumber.Enabled = False
        Me.txtVATRegistrationNumber.Location = New System.Drawing.Point(454, 174)
        Me.txtVATRegistrationNumber.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATRegistrationNumber.Name = "txtVATRegistrationNumber"
        Me.txtVATRegistrationNumber.Size = New System.Drawing.Size(267, 24)
        Me.txtVATRegistrationNumber.TabIndex = 0
        '
        'txtPhone
        '
        Me.txtPhone.Enabled = False
        Me.txtPhone.Location = New System.Drawing.Point(454, 132)
        Me.txtPhone.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(267, 24)
        Me.txtPhone.TabIndex = 0
        '
        'txtCity
        '
        Me.txtCity.Enabled = False
        Me.txtCity.Location = New System.Drawing.Point(613, 91)
        Me.txtCity.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(109, 24)
        Me.txtCity.TabIndex = 0
        '
        'txtAddress
        '
        Me.txtAddress.Enabled = False
        Me.txtAddress.Location = New System.Drawing.Point(454, 91)
        Me.txtAddress.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(151, 24)
        Me.txtAddress.TabIndex = 0
        '
        'txtPartnerCode
        '
        Me.txtPartnerCode.Enabled = False
        Me.txtPartnerCode.Location = New System.Drawing.Point(455, 50)
        Me.txtPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPartnerCode.Name = "txtPartnerCode"
        Me.txtPartnerCode.Size = New System.Drawing.Size(267, 24)
        Me.txtPartnerCode.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel1.Controls.Add(Me.tab1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1325, 959)
        Me.Panel1.TabIndex = 2
        '
        'frmPurchases
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1449, 959)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmPurchases"
        Me.Text = "frmPurchases"
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.tab1.ResumeLayout(False)
        Me.TabInvoice.ResumeLayout(False)
        Me.TabInvoice.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabOldInvoices As TabPage
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSend As Button
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents cboPaymentMethod As ComboBox
    Friend WithEvents btnAddPartner As Button
    Friend WithEvents btnFindPartner As Button
    Friend WithEvents cboSource As ComboBox
    Friend WithEvents cboPaymentTerm As ComboBox
    Friend WithEvents dtpDueDate As DateTimePicker
    Friend WithEvents dtpDocumentDate As DateTimePicker
    Friend WithEvents txtGrandTotal As TextBox
    Friend WithEvents txtVATTotal As TextBox
    Friend WithEvents txtSubTotal As TextBox
    Friend WithEvents cboPartnerCode As ComboBox
    Friend WithEvents txtNote As TextBox
    Friend WithEvents dgvInvoiceDetails As DataGridView
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents Label13 As Label
    Friend WithEvents tab1 As TabControl
    Friend WithEvents TabInvoice As TabPage
    Friend WithEvents Label17 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents txtVATRegistrationNumber As TextBox
    Friend WithEvents txtPhone As TextBox
    Friend WithEvents txtCity As TextBox
    Friend WithEvents txtAddress As TextBox
    Friend WithEvents txtPartnerCode As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents chkIsTaxInclusive As CheckBox
    Friend WithEvents Label18 As Label
    Friend WithEvents cboVATRate As ComboBox
    Friend WithEvents Label19 As Label
    Friend WithEvents cboDocumentID As ComboBox
    Friend WithEvents txtExchangeRate As TextBox
    Friend WithEvents txtCurrencyID As TextBox
    Friend WithEvents btnSaveDraft As Button
    Friend WithEvents txtStatusName As TextBox
    Friend WithEvents cboProductFilter As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents colProductSearch As DataGridViewButtonColumn
    Friend WithEvents colProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colProductType As DataGridViewComboBoxColumn
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colProductName As DataGridViewTextBoxColumn
    Friend WithEvents colUnitPrice As DataGridViewTextBoxColumn
    Friend WithEvents colQty As DataGridViewTextBoxColumn
    Friend WithEvents colUnitID As DataGridViewComboBoxColumn
    Friend WithEvents colTaxableAmount As DataGridViewTextBoxColumn
    Friend WithEvents colVATRate As DataGridViewTextBoxColumn
    Friend WithEvents colVATAmount As DataGridViewTextBoxColumn
    Friend WithEvents colTotalAmount As DataGridViewTextBoxColumn
End Class
