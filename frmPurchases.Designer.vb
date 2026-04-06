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
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnEnableEditing = New System.Windows.Forms.Button()
        Me.btnDeletePostedPurchase = New System.Windows.Forms.Button()
        Me.btnEditPostedPurchase = New System.Windows.Forms.Button()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.btnSaveDraft = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.tab1 = New System.Windows.Forms.TabControl()
        Me.TabInvoice = New System.Windows.Forms.TabPage()
        Me.txtExchangeRate = New System.Windows.Forms.TextBox()
        Me.txtCurrencyID = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.txtDocumentID = New System.Windows.Forms.TextBox()
        Me.cboProductFilter = New System.Windows.Forms.ComboBox()
        Me.txtPartnerCode = New System.Windows.Forms.TextBox()
        Me.cboVATRate = New System.Windows.Forms.ComboBox()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.chkIsTaxInclusive = New System.Windows.Forms.CheckBox()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.txtVATRegistrationNumber = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
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
        Me.colDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.tab1.SuspendLayout()
        Me.TabInvoice.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabOldInvoices
        '
        Me.tabOldInvoices.Location = New System.Drawing.Point(4, 34)
        Me.tabOldInvoices.Margin = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Name = "tabOldInvoices"
        Me.tabOldInvoices.Padding = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Size = New System.Drawing.Size(1450, 1017)
        Me.tabOldInvoices.TabIndex = 1
        Me.tabOldInvoices.Text = "فواتير سابقة"
        Me.tabOldInvoices.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNew.BackColor = System.Drawing.SystemColors.Control
        Me.btnNew.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNew.Location = New System.Drawing.Point(3, 428)
        Me.btnNew.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(116, 53)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnSend
        '
        Me.btnSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSend.BackColor = System.Drawing.SystemColors.Control
        Me.btnSend.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSend.Location = New System.Drawing.Point(3, 621)
        Me.btnSend.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(116, 53)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "ارسال"
        Me.btnSend.UseVisualStyleBackColor = False
        '
        'cboTargetStore
        '
        Me.cboTargetStore.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(68, 106)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(189, 29)
        Me.cboTargetStore.TabIndex = 17
        '
        'cboPaymentMethod
        '
        Me.cboPaymentMethod.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPaymentMethod.FormattingEnabled = True
        Me.cboPaymentMethod.Location = New System.Drawing.Point(925, 117)
        Me.cboPaymentMethod.Name = "cboPaymentMethod"
        Me.cboPaymentMethod.Size = New System.Drawing.Size(198, 29)
        Me.cboPaymentMethod.TabIndex = 15
        '
        'btnAddPartner
        '
        Me.btnAddPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnAddPartner.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddPartner.Location = New System.Drawing.Point(514, 36)
        Me.btnAddPartner.Name = "btnAddPartner"
        Me.btnAddPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnAddPartner.TabIndex = 14
        Me.btnAddPartner.Text = "Add"
        Me.btnAddPartner.UseVisualStyleBackColor = False
        '
        'btnFindPartner
        '
        Me.btnFindPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnFindPartner.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFindPartner.Location = New System.Drawing.Point(568, 36)
        Me.btnFindPartner.Name = "btnFindPartner"
        Me.btnFindPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnFindPartner.TabIndex = 14
        Me.btnFindPartner.Text = "Find"
        Me.btnFindPartner.UseVisualStyleBackColor = False
        '
        'cboSource
        '
        Me.cboSource.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSource.FormattingEnabled = True
        Me.cboSource.Location = New System.Drawing.Point(925, 236)
        Me.cboSource.Name = "cboSource"
        Me.cboSource.Size = New System.Drawing.Size(198, 29)
        Me.cboSource.TabIndex = 13
        '
        'cboPaymentTerm
        '
        Me.cboPaymentTerm.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPaymentTerm.FormattingEnabled = True
        Me.cboPaymentTerm.Location = New System.Drawing.Point(925, 157)
        Me.cboPaymentTerm.Name = "cboPaymentTerm"
        Me.cboPaymentTerm.Size = New System.Drawing.Size(198, 29)
        Me.cboPaymentTerm.TabIndex = 11
        '
        'dtpDueDate
        '
        Me.dtpDueDate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpDueDate.Location = New System.Drawing.Point(925, 197)
        Me.dtpDueDate.Name = "dtpDueDate"
        Me.dtpDueDate.Size = New System.Drawing.Size(198, 28)
        Me.dtpDueDate.TabIndex = 10
        '
        'dtpDocumentDate
        '
        Me.dtpDocumentDate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpDocumentDate.Location = New System.Drawing.Point(925, 78)
        Me.dtpDocumentDate.Name = "dtpDocumentDate"
        Me.dtpDocumentDate.Size = New System.Drawing.Size(198, 28)
        Me.dtpDocumentDate.TabIndex = 9
        '
        'txtGrandTotal
        '
        Me.txtGrandTotal.Enabled = False
        Me.txtGrandTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGrandTotal.Location = New System.Drawing.Point(68, 294)
        Me.txtGrandTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGrandTotal.Name = "txtGrandTotal"
        Me.txtGrandTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtGrandTotal.TabIndex = 7
        '
        'txtVATTotal
        '
        Me.txtVATTotal.Enabled = False
        Me.txtVATTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtVATTotal.Location = New System.Drawing.Point(68, 259)
        Me.txtVATTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATTotal.Name = "txtVATTotal"
        Me.txtVATTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtVATTotal.TabIndex = 7
        '
        'txtSubTotal
        '
        Me.txtSubTotal.Enabled = False
        Me.txtSubTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSubTotal.Location = New System.Drawing.Point(68, 224)
        Me.txtSubTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSubTotal.Name = "txtSubTotal"
        Me.txtSubTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtSubTotal.TabIndex = 7
        '
        'cboPartnerCode
        '
        Me.cboPartnerCode.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPartnerCode.FormattingEnabled = True
        Me.cboPartnerCode.Location = New System.Drawing.Point(624, 36)
        Me.cboPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.cboPartnerCode.Name = "cboPartnerCode"
        Me.cboPartnerCode.Size = New System.Drawing.Size(157, 29)
        Me.cboPartnerCode.TabIndex = 6
        '
        'txtNote
        '
        Me.txtNote.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNote.Location = New System.Drawing.Point(514, 237)
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
        Me.dgvInvoiceDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductSearch, Me.colProductCode, Me.colProductType, Me.colProductID, Me.colProductName, Me.colUnitPrice, Me.colQty, Me.colUnitID, Me.colTaxableAmount, Me.colVATRate, Me.colVATAmount, Me.colTotalAmount, Me.colDelete})
        Me.dgvInvoiceDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvInvoiceDetails.Location = New System.Drawing.Point(4, 384)
        Me.dgvInvoiceDetails.Margin = New System.Windows.Forms.Padding(4)
        Me.dgvInvoiceDetails.Name = "dgvInvoiceDetails"
        Me.dgvInvoiceDetails.RowHeadersWidth = 51
        Me.dgvInvoiceDetails.RowTemplate.Height = 26
        Me.dgvInvoiceDetails.Size = New System.Drawing.Size(1442, 629)
        Me.dgvInvoiceDetails.TabIndex = 3
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(800, 206)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(107, 21)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "الرقم الضريبي"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(821, 164)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(83, 21)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "رقم الهاتف"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(843, 123)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(56, 21)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "العنوان"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(330, 294)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(118, 21)
        Me.Label14.TabIndex = 1
        Me.Label14.Text = "المبلغ الاجمالي"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(3, 753)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(116, 53)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearch.Location = New System.Drawing.Point(3, 489)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(116, 53)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.Info
        Me.Panel2.Controls.Add(Me.btnEnableEditing)
        Me.Panel2.Controls.Add(Me.btnDeletePostedPurchase)
        Me.Panel2.Controls.Add(Me.btnEditPostedPurchase)
        Me.Panel2.Controls.Add(Me.txtStatusName)
        Me.Panel2.Controls.Add(Me.btnSaveDraft)
        Me.Panel2.Controls.Add(Me.btnNew)
        Me.Panel2.Controls.Add(Me.btnSend)
        Me.Panel2.Controls.Add(Me.btnCancel)
        Me.Panel2.Controls.Add(Me.btnSearch)
        Me.Panel2.Controls.Add(Me.btnPrint)
        Me.Panel2.Controls.Add(Me.btnClose)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(1458, 0)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(124, 1055)
        Me.Panel2.TabIndex = 3
        '
        'btnEnableEditing
        '
        Me.btnEnableEditing.Location = New System.Drawing.Point(7, 134)
        Me.btnEnableEditing.Name = "btnEnableEditing"
        Me.btnEnableEditing.Size = New System.Drawing.Size(105, 42)
        Me.btnEnableEditing.TabIndex = 24
        Me.btnEnableEditing.Text = "فتح التعديل"
        Me.btnEnableEditing.UseVisualStyleBackColor = True
        '
        'btnDeletePostedPurchase
        '
        Me.btnDeletePostedPurchase.Location = New System.Drawing.Point(7, 86)
        Me.btnDeletePostedPurchase.Name = "btnDeletePostedPurchase"
        Me.btnDeletePostedPurchase.Size = New System.Drawing.Size(105, 42)
        Me.btnDeletePostedPurchase.TabIndex = 23
        Me.btnDeletePostedPurchase.Text = "حذف مرحل"
        Me.btnDeletePostedPurchase.UseVisualStyleBackColor = True
        '
        'btnEditPostedPurchase
        '
        Me.btnEditPostedPurchase.Location = New System.Drawing.Point(7, 38)
        Me.btnEditPostedPurchase.Name = "btnEditPostedPurchase"
        Me.btnEditPostedPurchase.Size = New System.Drawing.Size(105, 42)
        Me.btnEditPostedPurchase.TabIndex = 23
        Me.btnEditPostedPurchase.Text = "تعديل مرحل"
        Me.btnEditPostedPurchase.UseVisualStyleBackColor = True
        '
        'txtStatusName
        '
        Me.txtStatusName.Enabled = False
        Me.txtStatusName.Location = New System.Drawing.Point(3, 3)
        Me.txtStatusName.Name = "txtStatusName"
        Me.txtStatusName.Size = New System.Drawing.Size(118, 24)
        Me.txtStatusName.TabIndex = 22
        '
        'btnSaveDraft
        '
        Me.btnSaveDraft.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSaveDraft.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveDraft.Location = New System.Drawing.Point(3, 555)
        Me.btnSaveDraft.Name = "btnSaveDraft"
        Me.btnSaveDraft.Size = New System.Drawing.Size(116, 53)
        Me.btnSaveDraft.TabIndex = 1
        Me.btnSaveDraft.Text = "حفظ"
        Me.btnSaveDraft.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(5, 687)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(116, 53)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Font = New System.Drawing.Font("Arial Rounded MT Bold", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(5, 819)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(116, 53)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "إغلاق"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(328, 259)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(117, 21)
        Me.Label13.TabIndex = 1
        Me.Label13.Text = "اجمالي الضريبة"
        '
        'tab1
        '
        Me.tab1.Controls.Add(Me.TabInvoice)
        Me.tab1.Controls.Add(Me.tabOldInvoices)
        Me.tab1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tab1.Font = New System.Drawing.Font("Arial Rounded MT Bold", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab1.ItemSize = New System.Drawing.Size(150, 30)
        Me.tab1.Location = New System.Drawing.Point(0, 0)
        Me.tab1.Margin = New System.Windows.Forms.Padding(4)
        Me.tab1.Name = "tab1"
        Me.tab1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tab1.RightToLeftLayout = True
        Me.tab1.SelectedIndex = 0
        Me.tab1.Size = New System.Drawing.Size(1458, 1055)
        Me.tab1.TabIndex = 0
        Me.tab1.Tag = ""
        '
        'TabInvoice
        '
        Me.TabInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TabInvoice.Controls.Add(Me.dgvInvoiceDetails)
        Me.TabInvoice.Controls.Add(Me.txtExchangeRate)
        Me.TabInvoice.Controls.Add(Me.txtCurrencyID)
        Me.TabInvoice.Controls.Add(Me.Panel3)
        Me.TabInvoice.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabInvoice.Location = New System.Drawing.Point(4, 34)
        Me.TabInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.Name = "TabInvoice"
        Me.TabInvoice.Padding = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.TabInvoice.Size = New System.Drawing.Size(1450, 1017)
        Me.TabInvoice.TabIndex = 0
        Me.TabInvoice.Text = "الفاتورة"
        '
        'txtExchangeRate
        '
        Me.txtExchangeRate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtExchangeRate.Location = New System.Drawing.Point(8, 410)
        Me.txtExchangeRate.Name = "txtExchangeRate"
        Me.txtExchangeRate.Size = New System.Drawing.Size(100, 28)
        Me.txtExchangeRate.TabIndex = 21
        Me.txtExchangeRate.Visible = False
        '
        'txtCurrencyID
        '
        Me.txtCurrencyID.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCurrencyID.Location = New System.Drawing.Point(8, 410)
        Me.txtCurrencyID.Name = "txtCurrencyID"
        Me.txtCurrencyID.Size = New System.Drawing.Size(100, 28)
        Me.txtCurrencyID.TabIndex = 21
        Me.txtCurrencyID.Visible = False
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.txtDocumentID)
        Me.Panel3.Controls.Add(Me.cboProductFilter)
        Me.Panel3.Controls.Add(Me.txtPartnerCode)
        Me.Panel3.Controls.Add(Me.cboVATRate)
        Me.Panel3.Controls.Add(Me.txtAddress)
        Me.Panel3.Controls.Add(Me.chkIsTaxInclusive)
        Me.Panel3.Controls.Add(Me.txtCity)
        Me.Panel3.Controls.Add(Me.cboTargetStore)
        Me.Panel3.Controls.Add(Me.txtPhone)
        Me.Panel3.Controls.Add(Me.cboPaymentMethod)
        Me.Panel3.Controls.Add(Me.txtVATRegistrationNumber)
        Me.Panel3.Controls.Add(Me.btnAddPartner)
        Me.Panel3.Controls.Add(Me.Label7)
        Me.Panel3.Controls.Add(Me.btnFindPartner)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.cboSource)
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Controls.Add(Me.cboPaymentTerm)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.dtpDueDate)
        Me.Panel3.Controls.Add(Me.Label11)
        Me.Panel3.Controls.Add(Me.dtpDocumentDate)
        Me.Panel3.Controls.Add(Me.Label16)
        Me.Panel3.Controls.Add(Me.txtGrandTotal)
        Me.Panel3.Controls.Add(Me.Label18)
        Me.Panel3.Controls.Add(Me.txtVATTotal)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.txtSubTotal)
        Me.Panel3.Controls.Add(Me.Label19)
        Me.Panel3.Controls.Add(Me.cboPartnerCode)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.txtNote)
        Me.Panel3.Controls.Add(Me.Label15)
        Me.Panel3.Controls.Add(Me.Label3)
        Me.Panel3.Controls.Add(Me.Label6)
        Me.Panel3.Controls.Add(Me.Label12)
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.Label17)
        Me.Panel3.Controls.Add(Me.Label4)
        Me.Panel3.Controls.Add(Me.Label13)
        Me.Panel3.Controls.Add(Me.Label14)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(4, 4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1442, 380)
        Me.Panel3.TabIndex = 23
        '
        'txtDocumentID
        '
        Me.txtDocumentID.Location = New System.Drawing.Point(925, 37)
        Me.txtDocumentID.Name = "txtDocumentID"
        Me.txtDocumentID.Size = New System.Drawing.Size(198, 28)
        Me.txtDocumentID.TabIndex = 23
        '
        'cboProductFilter
        '
        Me.cboProductFilter.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboProductFilter.FormattingEnabled = True
        Me.cboProductFilter.Location = New System.Drawing.Point(925, 316)
        Me.cboProductFilter.Name = "cboProductFilter"
        Me.cboProductFilter.Size = New System.Drawing.Size(198, 29)
        Me.cboProductFilter.TabIndex = 22
        '
        'txtPartnerCode
        '
        Me.txtPartnerCode.Enabled = False
        Me.txtPartnerCode.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPartnerCode.Location = New System.Drawing.Point(515, 79)
        Me.txtPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPartnerCode.Name = "txtPartnerCode"
        Me.txtPartnerCode.Size = New System.Drawing.Size(267, 28)
        Me.txtPartnerCode.TabIndex = 0
        '
        'cboVATRate
        '
        Me.cboVATRate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboVATRate.FormattingEnabled = True
        Me.cboVATRate.Location = New System.Drawing.Point(1013, 276)
        Me.cboVATRate.Name = "cboVATRate"
        Me.cboVATRate.Size = New System.Drawing.Size(110, 29)
        Me.cboVATRate.TabIndex = 19
        '
        'txtAddress
        '
        Me.txtAddress.Enabled = False
        Me.txtAddress.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddress.Location = New System.Drawing.Point(514, 120)
        Me.txtAddress.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(151, 28)
        Me.txtAddress.TabIndex = 0
        '
        'chkIsTaxInclusive
        '
        Me.chkIsTaxInclusive.AutoSize = True
        Me.chkIsTaxInclusive.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkIsTaxInclusive.Location = New System.Drawing.Point(925, 282)
        Me.chkIsTaxInclusive.Name = "chkIsTaxInclusive"
        Me.chkIsTaxInclusive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsTaxInclusive.TabIndex = 18
        Me.chkIsTaxInclusive.UseVisualStyleBackColor = True
        '
        'txtCity
        '
        Me.txtCity.Enabled = False
        Me.txtCity.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCity.Location = New System.Drawing.Point(673, 120)
        Me.txtCity.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(109, 28)
        Me.txtCity.TabIndex = 0
        '
        'txtPhone
        '
        Me.txtPhone.Enabled = False
        Me.txtPhone.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPhone.Location = New System.Drawing.Point(514, 161)
        Me.txtPhone.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(267, 28)
        Me.txtPhone.TabIndex = 0
        '
        'txtVATRegistrationNumber
        '
        Me.txtVATRegistrationNumber.Enabled = False
        Me.txtVATRegistrationNumber.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtVATRegistrationNumber.Location = New System.Drawing.Point(514, 203)
        Me.txtVATRegistrationNumber.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATRegistrationNumber.Name = "txtVATRegistrationNumber"
        Me.txtVATRegistrationNumber.Size = New System.Drawing.Size(267, 28)
        Me.txtVATRegistrationNumber.TabIndex = 0
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(1259, 36)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(90, 21)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "رقم الفاتورة"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(1249, 76)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(103, 21)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = "تاريخ الفاتورة "
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(1229, 193)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(126, 21)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "تاريخ الاستحقاق"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(1257, 153)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(91, 21)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "طبيعة الدفع"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(1228, 239)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(126, 21)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "مصدر المشتريات"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(1253, 105)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(97, 21)
        Me.Label16.TabIndex = 1
        Me.Label16.Text = "طريقة الدفع "
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(1283, 297)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(60, 21)
        Me.Label18.TabIndex = 1
        Me.Label18.Text = "الضريبة"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(1216, 325)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(141, 21)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "الية فلترة المنتجات"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(957, 280)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(53, 21)
        Me.Label19.TabIndex = 1
        Me.Label19.Text = "شامل"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(819, 82)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 21)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "إسم المورد"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(821, 40)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(80, 21)
        Me.Label15.TabIndex = 1
        Me.Label15.Text = "كود المورد"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(301, 224)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(147, 21)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "اجمالي قبل الضريبة"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(350, 67)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(89, 21)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "حالة الطلب"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(307, 106)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(145, 21)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "المستودع المستلم"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel1.Controls.Add(Me.tab1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1458, 1055)
        Me.Panel1.TabIndex = 2
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
        'colDelete
        '
        Me.colDelete.HeaderText = "حذف"
        Me.colDelete.MinimumWidth = 6
        Me.colDelete.Name = "colDelete"
        '
        'frmPurchases
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 1055)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmPurchases"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "انشاء فاتورة مشتريات"
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.tab1.ResumeLayout(False)
        Me.TabInvoice.ResumeLayout(False)
        Me.TabInvoice.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
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
    Friend WithEvents txtExchangeRate As TextBox
    Friend WithEvents txtCurrencyID As TextBox
    Friend WithEvents btnSaveDraft As Button
    Friend WithEvents txtStatusName As TextBox
    Friend WithEvents cboProductFilter As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents btnDeletePostedPurchase As Button
    Friend WithEvents btnEditPostedPurchase As Button
    Friend WithEvents txtDocumentID As TextBox
    Friend WithEvents btnEnableEditing As Button
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
    Friend WithEvents colDelete As DataGridViewButtonColumn
End Class
