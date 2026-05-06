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
        Me.cboPaymentTerm = New System.Windows.Forms.ComboBox()
        Me.dtpDueDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpDocumentDate = New System.Windows.Forms.DateTimePicker()
        Me.cboPartnerCode = New System.Windows.Forms.ComboBox()
        Me.txtNote = New System.Windows.Forms.TextBox()
        Me.dgvInvoiceDetails = New System.Windows.Forms.DataGridView()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnDeletePostedPurchase = New System.Windows.Forms.Button()
        Me.btnEditPostedPurchase = New System.Windows.Forms.Button()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.btnSaveDraft = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.tab1 = New System.Windows.Forms.TabControl()
        Me.TabInvoice = New System.Windows.Forms.TabPage()
        Me.txtExchangeRate = New System.Windows.Forms.TextBox()
        Me.txtCurrencyID = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.txtDocumentID = New System.Windows.Forms.TextBox()
        Me.cboProductFilter = New System.Windows.Forms.ComboBox()
        Me.txtPartnerCode = New System.Windows.Forms.TextBox()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.txtVATRegistrationNumber = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.cboVATRate = New System.Windows.Forms.ComboBox()
        Me.chkIsTaxInclusive = New System.Windows.Forms.CheckBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtGrandTotal = New System.Windows.Forms.TextBox()
        Me.txtVATTotal = New System.Windows.Forms.TextBox()
        Me.txtSubTotal = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.colProductSearch = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
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
        Me.Panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabOldInvoices
        '
        Me.tabOldInvoices.Location = New System.Drawing.Point(4, 34)
        Me.tabOldInvoices.Margin = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Name = "tabOldInvoices"
        Me.tabOldInvoices.Padding = New System.Windows.Forms.Padding(4)
        Me.tabOldInvoices.Size = New System.Drawing.Size(1309, 1017)
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
        Me.cboTargetStore.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboTargetStore.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(2, 5)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(267, 29)
        Me.cboTargetStore.TabIndex = 17
        '
        'cboPaymentMethod
        '
        Me.cboPaymentMethod.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboPaymentMethod.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPaymentMethod.FormattingEnabled = True
        Me.cboPaymentMethod.Location = New System.Drawing.Point(910, 77)
        Me.cboPaymentMethod.Name = "cboPaymentMethod"
        Me.cboPaymentMethod.Size = New System.Drawing.Size(198, 29)
        Me.cboPaymentMethod.TabIndex = 15
        '
        'btnAddPartner
        '
        Me.btnAddPartner.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnAddPartner.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddPartner.Location = New System.Drawing.Point(468, 5)
        Me.btnAddPartner.Name = "btnAddPartner"
        Me.btnAddPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnAddPartner.TabIndex = 14
        Me.btnAddPartner.Text = "Add"
        Me.btnAddPartner.UseVisualStyleBackColor = False
        '
        'btnFindPartner
        '
        Me.btnFindPartner.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFindPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnFindPartner.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFindPartner.Location = New System.Drawing.Point(522, 5)
        Me.btnFindPartner.Name = "btnFindPartner"
        Me.btnFindPartner.Size = New System.Drawing.Size(49, 28)
        Me.btnFindPartner.TabIndex = 14
        Me.btnFindPartner.Text = "Find"
        Me.btnFindPartner.UseVisualStyleBackColor = False
        '
        'cboPaymentTerm
        '
        Me.cboPaymentTerm.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboPaymentTerm.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPaymentTerm.FormattingEnabled = True
        Me.cboPaymentTerm.Location = New System.Drawing.Point(910, 114)
        Me.cboPaymentTerm.Name = "cboPaymentTerm"
        Me.cboPaymentTerm.Size = New System.Drawing.Size(198, 29)
        Me.cboPaymentTerm.TabIndex = 11
        '
        'dtpDueDate
        '
        Me.dtpDueDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpDueDate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpDueDate.Location = New System.Drawing.Point(910, 151)
        Me.dtpDueDate.Name = "dtpDueDate"
        Me.dtpDueDate.Size = New System.Drawing.Size(198, 28)
        Me.dtpDueDate.TabIndex = 10
        '
        'dtpDocumentDate
        '
        Me.dtpDocumentDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpDocumentDate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpDocumentDate.Location = New System.Drawing.Point(910, 41)
        Me.dtpDocumentDate.Name = "dtpDocumentDate"
        Me.dtpDocumentDate.Size = New System.Drawing.Size(198, 28)
        Me.dtpDocumentDate.TabIndex = 9
        '
        'cboPartnerCode
        '
        Me.cboPartnerCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboPartnerCode.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPartnerCode.FormattingEnabled = True
        Me.cboPartnerCode.Location = New System.Drawing.Point(578, 5)
        Me.cboPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.cboPartnerCode.Name = "cboPartnerCode"
        Me.cboPartnerCode.Size = New System.Drawing.Size(157, 29)
        Me.cboPartnerCode.TabIndex = 6
        '
        'txtNote
        '
        Me.txtNote.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNote.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNote.Location = New System.Drawing.Point(2, 81)
        Me.txtNote.Margin = New System.Windows.Forms.Padding(4)
        Me.txtNote.Multiline = True
        Me.txtNote.Name = "txtNote"
        Me.txtNote.Size = New System.Drawing.Size(267, 97)
        Me.txtNote.TabIndex = 5
        '
        'dgvInvoiceDetails
        '
        Me.dgvInvoiceDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvInvoiceDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInvoiceDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductSearch, Me.colProductCode, Me.colProductType, Me.colProductID, Me.colProductName, Me.colQty, Me.colUnitID, Me.colUnitPrice, Me.colTaxableAmount, Me.colVATRate, Me.colVATAmount, Me.colTotalAmount, Me.colDelete})
        Me.dgvInvoiceDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvInvoiceDetails.Location = New System.Drawing.Point(4, 202)
        Me.dgvInvoiceDetails.Margin = New System.Windows.Forms.Padding(4)
        Me.dgvInvoiceDetails.Name = "dgvInvoiceDetails"
        Me.dgvInvoiceDetails.RowHeadersWidth = 51
        Me.dgvInvoiceDetails.RowTemplate.Height = 26
        Me.dgvInvoiceDetails.Size = New System.Drawing.Size(1301, 811)
        Me.dgvInvoiceDetails.TabIndex = 3
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(771, 153)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(107, 21)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "الرقم الضريبي"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(795, 117)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(83, 21)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "رقم الهاتف"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(822, 81)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(56, 21)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "العنوان"
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
        Me.Panel2.Location = New System.Drawing.Point(1317, 0)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(124, 1055)
        Me.Panel2.TabIndex = 3
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
        Me.tab1.Size = New System.Drawing.Size(1317, 1055)
        Me.tab1.TabIndex = 0
        Me.tab1.Tag = ""
        '
        'TabInvoice
        '
        Me.TabInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
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
        Me.TabInvoice.Size = New System.Drawing.Size(1309, 1017)
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
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel3.Controls.Add(Me.txtDocumentID)
        Me.Panel3.Controls.Add(Me.cboProductFilter)
        Me.Panel3.Controls.Add(Me.txtPartnerCode)
        Me.Panel3.Controls.Add(Me.txtAddress)
        Me.Panel3.Controls.Add(Me.txtCity)
        Me.Panel3.Controls.Add(Me.cboTargetStore)
        Me.Panel3.Controls.Add(Me.txtPhone)
        Me.Panel3.Controls.Add(Me.cboPaymentMethod)
        Me.Panel3.Controls.Add(Me.txtVATRegistrationNumber)
        Me.Panel3.Controls.Add(Me.btnAddPartner)
        Me.Panel3.Controls.Add(Me.Label7)
        Me.Panel3.Controls.Add(Me.btnFindPartner)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Controls.Add(Me.cboPaymentTerm)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.dtpDueDate)
        Me.Panel3.Controls.Add(Me.dtpDocumentDate)
        Me.Panel3.Controls.Add(Me.Label16)
        Me.Panel3.Controls.Add(Me.Label11)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.cboPartnerCode)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.txtNote)
        Me.Panel3.Controls.Add(Me.Label15)
        Me.Panel3.Controls.Add(Me.Label6)
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.Label17)
        Me.Panel3.Controls.Add(Me.Label4)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(4, 4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1301, 198)
        Me.Panel3.TabIndex = 23
        '
        'txtDocumentID
        '
        Me.txtDocumentID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDocumentID.Location = New System.Drawing.Point(910, 5)
        Me.txtDocumentID.Name = "txtDocumentID"
        Me.txtDocumentID.Size = New System.Drawing.Size(198, 28)
        Me.txtDocumentID.TabIndex = 23
        '
        'cboProductFilter
        '
        Me.cboProductFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboProductFilter.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboProductFilter.FormattingEnabled = True
        Me.cboProductFilter.Location = New System.Drawing.Point(2, 41)
        Me.cboProductFilter.Name = "cboProductFilter"
        Me.cboProductFilter.Size = New System.Drawing.Size(267, 29)
        Me.cboProductFilter.TabIndex = 22
        '
        'txtPartnerCode
        '
        Me.txtPartnerCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPartnerCode.Enabled = False
        Me.txtPartnerCode.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPartnerCode.Location = New System.Drawing.Point(469, 42)
        Me.txtPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPartnerCode.Name = "txtPartnerCode"
        Me.txtPartnerCode.Size = New System.Drawing.Size(267, 28)
        Me.txtPartnerCode.TabIndex = 0
        '
        'txtAddress
        '
        Me.txtAddress.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAddress.Enabled = False
        Me.txtAddress.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddress.Location = New System.Drawing.Point(468, 78)
        Me.txtAddress.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(151, 28)
        Me.txtAddress.TabIndex = 0
        '
        'txtCity
        '
        Me.txtCity.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCity.Enabled = False
        Me.txtCity.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCity.Location = New System.Drawing.Point(627, 78)
        Me.txtCity.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(109, 28)
        Me.txtCity.TabIndex = 0
        '
        'txtPhone
        '
        Me.txtPhone.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPhone.Enabled = False
        Me.txtPhone.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPhone.Location = New System.Drawing.Point(468, 114)
        Me.txtPhone.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(267, 28)
        Me.txtPhone.TabIndex = 0
        '
        'txtVATRegistrationNumber
        '
        Me.txtVATRegistrationNumber.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtVATRegistrationNumber.Enabled = False
        Me.txtVATRegistrationNumber.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtVATRegistrationNumber.Location = New System.Drawing.Point(468, 150)
        Me.txtVATRegistrationNumber.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATRegistrationNumber.Name = "txtVATRegistrationNumber"
        Me.txtVATRegistrationNumber.Size = New System.Drawing.Size(267, 28)
        Me.txtVATRegistrationNumber.TabIndex = 0
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(1196, 9)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(90, 21)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "رقم الفاتورة"
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(1183, 45)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(103, 21)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = "تاريخ الفاتورة "
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(1160, 153)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(126, 21)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "تاريخ الاستحقاق"
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(1195, 117)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(91, 21)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "طبيعة الدفع"
        '
        'Label16
        '
        Me.Label16.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(1189, 81)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(97, 21)
        Me.Label16.TabIndex = 1
        Me.Label16.Text = "طريقة الدفع "
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(329, 81)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(97, 21)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "ملاحظــــــات"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(285, 45)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(141, 21)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "الية فلترة المنتجات"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(789, 45)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 21)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "إسم المورد"
        '
        'Label15
        '
        Me.Label15.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(798, 9)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(80, 21)
        Me.Label15.TabIndex = 1
        Me.Label15.Text = "كود المورد"
        '
        'Label17
        '
        Me.Label17.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(281, 9)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(145, 21)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "المستودع المستلم"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel1.Controls.Add(Me.tab1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1317, 1055)
        Me.Panel1.TabIndex = 2
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel4.Controls.Add(Me.cboVATRate)
        Me.Panel4.Controls.Add(Me.chkIsTaxInclusive)
        Me.Panel4.Controls.Add(Me.Label18)
        Me.Panel4.Controls.Add(Me.Label19)
        Me.Panel4.Controls.Add(Me.txtGrandTotal)
        Me.Panel4.Controls.Add(Me.txtVATTotal)
        Me.Panel4.Controls.Add(Me.txtSubTotal)
        Me.Panel4.Controls.Add(Me.Label3)
        Me.Panel4.Controls.Add(Me.Label13)
        Me.Panel4.Controls.Add(Me.Label14)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 910)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1317, 145)
        Me.Panel4.TabIndex = 4
        '
        'cboVATRate
        '
        Me.cboVATRate.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboVATRate.FormattingEnabled = True
        Me.cboVATRate.Location = New System.Drawing.Point(92, 11)
        Me.cboVATRate.Name = "cboVATRate"
        Me.cboVATRate.Size = New System.Drawing.Size(110, 29)
        Me.cboVATRate.TabIndex = 23
        '
        'chkIsTaxInclusive
        '
        Me.chkIsTaxInclusive.AutoSize = True
        Me.chkIsTaxInclusive.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkIsTaxInclusive.Location = New System.Drawing.Point(13, 17)
        Me.chkIsTaxInclusive.Name = "chkIsTaxInclusive"
        Me.chkIsTaxInclusive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsTaxInclusive.TabIndex = 22
        Me.chkIsTaxInclusive.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(310, 14)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(60, 21)
        Me.Label18.TabIndex = 20
        Me.Label18.Text = "الضريبة"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(36, 15)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(53, 21)
        Me.Label19.TabIndex = 21
        Me.Label19.Text = "شامل"
        '
        'txtGrandTotal
        '
        Me.txtGrandTotal.Enabled = False
        Me.txtGrandTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGrandTotal.Location = New System.Drawing.Point(13, 103)
        Me.txtGrandTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGrandTotal.Name = "txtGrandTotal"
        Me.txtGrandTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtGrandTotal.TabIndex = 11
        '
        'txtVATTotal
        '
        Me.txtVATTotal.Enabled = False
        Me.txtVATTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtVATTotal.Location = New System.Drawing.Point(13, 75)
        Me.txtVATTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATTotal.Name = "txtVATTotal"
        Me.txtVATTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtVATTotal.TabIndex = 12
        '
        'txtSubTotal
        '
        Me.txtSubTotal.Enabled = False
        Me.txtSubTotal.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSubTotal.Location = New System.Drawing.Point(13, 47)
        Me.txtSubTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSubTotal.Name = "txtSubTotal"
        Me.txtSubTotal.Size = New System.Drawing.Size(189, 28)
        Me.txtSubTotal.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(223, 51)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(147, 21)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "اجمالي قبل الضريبة"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(253, 79)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(117, 21)
        Me.Label13.TabIndex = 9
        Me.Label13.Text = "اجمالي الضريبة"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(252, 107)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(118, 21)
        Me.Label14.TabIndex = 10
        Me.Label14.Text = "المبلغ الاجمالي"
        '
        'colProductSearch
        '
        Me.colProductSearch.HeaderText = "بحث"
        Me.colProductSearch.MinimumWidth = 6
        Me.colProductSearch.Name = "colProductSearch"
        Me.colProductSearch.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductSearch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.colProductSearch.Text = "بحث"
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
        Me.colUnitID.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colUnitID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'colUnitPrice
        '
        Me.colUnitPrice.HeaderText = "السعر"
        Me.colUnitPrice.MinimumWidth = 6
        Me.colUnitPrice.Name = "colUnitPrice"
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
        Me.colDelete.Text = "حذف"
        Me.colDelete.UseColumnTextForButtonValue = True
        '
        'frmPurchases
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1441, 1055)
        Me.Controls.Add(Me.Panel4)
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
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabOldInvoices As TabPage
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSend As Button
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents cboPaymentMethod As ComboBox
    Friend WithEvents btnAddPartner As Button
    Friend WithEvents btnFindPartner As Button
    Friend WithEvents cboPaymentTerm As ComboBox
    Friend WithEvents dtpDueDate As DateTimePicker
    Friend WithEvents dtpDocumentDate As DateTimePicker
    Friend WithEvents cboPartnerCode As ComboBox
    Friend WithEvents txtNote As TextBox
    Friend WithEvents dgvInvoiceDetails As DataGridView
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents tab1 As TabControl
    Friend WithEvents TabInvoice As TabPage
    Friend WithEvents Label17 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label16 As Label
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
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Label11 As Label
    Friend WithEvents cboVATRate As ComboBox
    Friend WithEvents chkIsTaxInclusive As CheckBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents txtGrandTotal As TextBox
    Friend WithEvents txtVATTotal As TextBox
    Friend WithEvents txtSubTotal As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents colProductSearch As DataGridViewButtonColumn
    Friend WithEvents colProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colProductType As DataGridViewComboBoxColumn
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colProductName As DataGridViewTextBoxColumn
    Friend WithEvents colQty As DataGridViewTextBoxColumn
    Friend WithEvents colUnitID As DataGridViewTextBoxColumn
    Friend WithEvents colUnitPrice As DataGridViewTextBoxColumn
    Friend WithEvents colTaxableAmount As DataGridViewTextBoxColumn
    Friend WithEvents colVATRate As DataGridViewTextBoxColumn
    Friend WithEvents colVATAmount As DataGridViewTextBoxColumn
    Friend WithEvents colTotalAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDelete As DataGridViewButtonColumn
End Class
