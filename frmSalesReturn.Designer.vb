<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSalesReturn
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
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.pnlControl = New System.Windows.Forms.Panel()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.cboStatusID = New System.Windows.Forms.ComboBox()
        Me.btnPostReturnInvoice = New System.Windows.Forms.Button()
        Me.btnNewInvoice = New System.Windows.Forms.Button()
        Me.btnSaveDraft = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnGetOriginalInvoice = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.tabDraftInvoices = New System.Windows.Forms.TabPage()
        Me.cboPaymentMethodID = New System.Windows.Forms.ComboBox()
        Me.btnAddPartner = New System.Windows.Forms.Button()
        Me.btnFindPartner = New System.Windows.Forms.Button()
        Me.cboSourceStoreID = New System.Windows.Forms.ComboBox()
        Me.cboPaymentTerm = New System.Windows.Forms.ComboBox()
        Me.dtpDocumentDate = New System.Windows.Forms.DateTimePicker()
        Me.txtGrandTotal = New System.Windows.Forms.TextBox()
        Me.txtTotalTax = New System.Windows.Forms.TextBox()
        Me.txtTotalAmount = New System.Windows.Forms.TextBox()
        Me.cboPartnerID = New System.Windows.Forms.ComboBox()
        Me.pnlInvoice = New System.Windows.Forms.Panel()
        Me.tabCurrent = New System.Windows.Forms.TabControl()
        Me.TabInvoice = New System.Windows.Forms.TabPage()
        Me.dgvInvoiceDetails = New System.Windows.Forms.DataGridView()
        Me.colSourceLODDetailID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetSourceDocumentDetailID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetUnitID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetSRDID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetSRID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetBaseProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDetQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetReturnQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetSellUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetPieceSellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetM3SellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetLineAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetDiscountPercent = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetDiscountAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetTaxableAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetTaxPercent = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDetTaxAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTotal = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.pnlInvoiceSupport = New System.Windows.Forms.Panel()
        Me.cboTaxReason = New System.Windows.Forms.ComboBox()
        Me.txtInvoiceNote = New System.Windows.Forms.TextBox()
        Me.chkIsIncludeVAT = New System.Windows.Forms.CheckBox()
        Me.chkIsExport = New System.Windows.Forms.CheckBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.lblTotalVolume = New System.Windows.Forms.Label()
        Me.pnlUp = New System.Windows.Forms.Panel()
        Me.pnlTotals = New System.Windows.Forms.Panel()
        Me.txtTotalDiscount = New System.Windows.Forms.TextBox()
        Me.txtTotalTaxableAmount = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.pnlPayment = New System.Windows.Forms.Panel()
        Me.txtVehicleCode = New System.Windows.Forms.TextBox()
        Me.txtDriverName = New System.Windows.Forms.TextBox()
        Me.txtSRCode = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.pnlPartner1 = New System.Windows.Forms.Panel()
        Me.txtPartnerName = New System.Windows.Forms.TextBox()
        Me.txtVATRegistrationNumber = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.txtInvoicCode = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.pnlControl.SuspendLayout()
        Me.pnlInvoice.SuspendLayout()
        Me.tabCurrent.SuspendLayout()
        Me.TabInvoice.SuspendLayout()
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlInvoiceSupport.SuspendLayout()
        Me.pnlUp.SuspendLayout()
        Me.pnlTotals.SuspendLayout()
        Me.pnlPayment.SuspendLayout()
        Me.pnlPartner1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Location = New System.Drawing.Point(7, 583)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(112, 41)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'pnlControl
        '
        Me.pnlControl.BackColor = System.Drawing.SystemColors.Info
        Me.pnlControl.Controls.Add(Me.btnSearch)
        Me.pnlControl.Controls.Add(Me.cboStatusID)
        Me.pnlControl.Controls.Add(Me.btnPostReturnInvoice)
        Me.pnlControl.Controls.Add(Me.btnNewInvoice)
        Me.pnlControl.Controls.Add(Me.btnSaveDraft)
        Me.pnlControl.Controls.Add(Me.btnCancel)
        Me.pnlControl.Controls.Add(Me.btnGetOriginalInvoice)
        Me.pnlControl.Controls.Add(Me.btnPrint)
        Me.pnlControl.Controls.Add(Me.btnClose)
        Me.pnlControl.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlControl.Location = New System.Drawing.Point(1200, 0)
        Me.pnlControl.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlControl.Name = "pnlControl"
        Me.pnlControl.Size = New System.Drawing.Size(124, 760)
        Me.pnlControl.TabIndex = 3
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Location = New System.Drawing.Point(4, 351)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(112, 41)
        Me.btnSearch.TabIndex = 17
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'cboStatusID
        '
        Me.cboStatusID.Enabled = False
        Me.cboStatusID.FormattingEnabled = True
        Me.cboStatusID.Location = New System.Drawing.Point(7, 3)
        Me.cboStatusID.Name = "cboStatusID"
        Me.cboStatusID.Size = New System.Drawing.Size(105, 24)
        Me.cboStatusID.TabIndex = 16
        '
        'btnPostReturnInvoice
        '
        Me.btnPostReturnInvoice.BackColor = System.Drawing.SystemColors.Control
        Me.btnPostReturnInvoice.Location = New System.Drawing.Point(6, 668)
        Me.btnPostReturnInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPostReturnInvoice.Name = "btnPostReturnInvoice"
        Me.btnPostReturnInvoice.Size = New System.Drawing.Size(112, 50)
        Me.btnPostReturnInvoice.TabIndex = 0
        Me.btnPostReturnInvoice.Text = "ارسال المرتجع للمستودع"
        Me.btnPostReturnInvoice.UseVisualStyleBackColor = False
        '
        'btnNewInvoice
        '
        Me.btnNewInvoice.BackColor = System.Drawing.SystemColors.Control
        Me.btnNewInvoice.Location = New System.Drawing.Point(7, 419)
        Me.btnNewInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNewInvoice.Name = "btnNewInvoice"
        Me.btnNewInvoice.Size = New System.Drawing.Size(112, 41)
        Me.btnNewInvoice.TabIndex = 0
        Me.btnNewInvoice.Text = "جديد"
        Me.btnNewInvoice.UseVisualStyleBackColor = False
        '
        'btnSaveDraft
        '
        Me.btnSaveDraft.BackColor = System.Drawing.SystemColors.Control
        Me.btnSaveDraft.Location = New System.Drawing.Point(7, 460)
        Me.btnSaveDraft.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSaveDraft.Name = "btnSaveDraft"
        Me.btnSaveDraft.Size = New System.Drawing.Size(112, 41)
        Me.btnSaveDraft.TabIndex = 0
        Me.btnSaveDraft.Text = "حفظ كمسودة"
        Me.btnSaveDraft.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Location = New System.Drawing.Point(7, 501)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(112, 41)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnGetOriginalInvoice
        '
        Me.btnGetOriginalInvoice.BackColor = System.Drawing.SystemColors.Control
        Me.btnGetOriginalInvoice.Location = New System.Drawing.Point(6, 35)
        Me.btnGetOriginalInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGetOriginalInvoice.Name = "btnGetOriginalInvoice"
        Me.btnGetOriginalInvoice.Size = New System.Drawing.Size(112, 88)
        Me.btnGetOriginalInvoice.TabIndex = 0
        Me.btnGetOriginalInvoice.Text = "جلب اصل الفاتورة"
        Me.btnGetOriginalInvoice.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Location = New System.Drawing.Point(7, 624)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(112, 41)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "غلق"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'tabDraftInvoices
        '
        Me.tabDraftInvoices.Location = New System.Drawing.Point(4, 34)
        Me.tabDraftInvoices.Margin = New System.Windows.Forms.Padding(4)
        Me.tabDraftInvoices.Name = "tabDraftInvoices"
        Me.tabDraftInvoices.Padding = New System.Windows.Forms.Padding(4)
        Me.tabDraftInvoices.Size = New System.Drawing.Size(1192, 722)
        Me.tabDraftInvoices.TabIndex = 1
        Me.tabDraftInvoices.Text = "فواتير مسودة"
        Me.tabDraftInvoices.UseVisualStyleBackColor = True
        '
        'cboPaymentMethodID
        '
        Me.cboPaymentMethodID.Enabled = False
        Me.cboPaymentMethodID.FormattingEnabled = True
        Me.cboPaymentMethodID.Location = New System.Drawing.Point(12, 125)
        Me.cboPaymentMethodID.Name = "cboPaymentMethodID"
        Me.cboPaymentMethodID.Size = New System.Drawing.Size(165, 24)
        Me.cboPaymentMethodID.TabIndex = 15
        '
        'btnAddPartner
        '
        Me.btnAddPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnAddPartner.Enabled = False
        Me.btnAddPartner.Location = New System.Drawing.Point(56, 7)
        Me.btnAddPartner.Name = "btnAddPartner"
        Me.btnAddPartner.Size = New System.Drawing.Size(48, 28)
        Me.btnAddPartner.TabIndex = 14
        Me.btnAddPartner.Text = "Add"
        Me.btnAddPartner.UseVisualStyleBackColor = False
        '
        'btnFindPartner
        '
        Me.btnFindPartner.BackColor = System.Drawing.SystemColors.Control
        Me.btnFindPartner.Enabled = False
        Me.btnFindPartner.Location = New System.Drawing.Point(11, 7)
        Me.btnFindPartner.Name = "btnFindPartner"
        Me.btnFindPartner.Size = New System.Drawing.Size(45, 28)
        Me.btnFindPartner.TabIndex = 14
        Me.btnFindPartner.Text = "Find"
        Me.btnFindPartner.UseVisualStyleBackColor = False
        '
        'cboSourceStoreID
        '
        Me.cboSourceStoreID.Enabled = False
        Me.cboSourceStoreID.FormattingEnabled = True
        Me.cboSourceStoreID.Location = New System.Drawing.Point(17, 37)
        Me.cboSourceStoreID.Name = "cboSourceStoreID"
        Me.cboSourceStoreID.Size = New System.Drawing.Size(165, 24)
        Me.cboSourceStoreID.TabIndex = 13
        '
        'cboPaymentTerm
        '
        Me.cboPaymentTerm.Enabled = False
        Me.cboPaymentTerm.FormattingEnabled = True
        Me.cboPaymentTerm.Location = New System.Drawing.Point(12, 96)
        Me.cboPaymentTerm.Name = "cboPaymentTerm"
        Me.cboPaymentTerm.Size = New System.Drawing.Size(165, 24)
        Me.cboPaymentTerm.TabIndex = 11
        '
        'dtpDocumentDate
        '
        Me.dtpDocumentDate.Location = New System.Drawing.Point(12, 38)
        Me.dtpDocumentDate.Name = "dtpDocumentDate"
        Me.dtpDocumentDate.Size = New System.Drawing.Size(165, 24)
        Me.dtpDocumentDate.TabIndex = 9
        '
        'txtGrandTotal
        '
        Me.txtGrandTotal.Enabled = False
        Me.txtGrandTotal.Location = New System.Drawing.Point(10, 121)
        Me.txtGrandTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGrandTotal.Name = "txtGrandTotal"
        Me.txtGrandTotal.Size = New System.Drawing.Size(164, 24)
        Me.txtGrandTotal.TabIndex = 7
        '
        'txtTotalTax
        '
        Me.txtTotalTax.Enabled = False
        Me.txtTotalTax.Location = New System.Drawing.Point(10, 92)
        Me.txtTotalTax.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTotalTax.Name = "txtTotalTax"
        Me.txtTotalTax.Size = New System.Drawing.Size(164, 24)
        Me.txtTotalTax.TabIndex = 7
        '
        'txtTotalAmount
        '
        Me.txtTotalAmount.Enabled = False
        Me.txtTotalAmount.Location = New System.Drawing.Point(10, 5)
        Me.txtTotalAmount.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTotalAmount.Name = "txtTotalAmount"
        Me.txtTotalAmount.Size = New System.Drawing.Size(164, 24)
        Me.txtTotalAmount.TabIndex = 7
        '
        'cboPartnerID
        '
        Me.cboPartnerID.Enabled = False
        Me.cboPartnerID.FormattingEnabled = True
        Me.cboPartnerID.Location = New System.Drawing.Point(104, 9)
        Me.cboPartnerID.Margin = New System.Windows.Forms.Padding(4)
        Me.cboPartnerID.Name = "cboPartnerID"
        Me.cboPartnerID.Size = New System.Drawing.Size(73, 24)
        Me.cboPartnerID.TabIndex = 6
        '
        'pnlInvoice
        '
        Me.pnlInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.pnlInvoice.Controls.Add(Me.tabCurrent)
        Me.pnlInvoice.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlInvoice.Location = New System.Drawing.Point(0, 0)
        Me.pnlInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlInvoice.Name = "pnlInvoice"
        Me.pnlInvoice.Size = New System.Drawing.Size(1200, 760)
        Me.pnlInvoice.TabIndex = 2
        '
        'tabCurrent
        '
        Me.tabCurrent.Controls.Add(Me.TabInvoice)
        Me.tabCurrent.Controls.Add(Me.tabDraftInvoices)
        Me.tabCurrent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabCurrent.ItemSize = New System.Drawing.Size(150, 30)
        Me.tabCurrent.Location = New System.Drawing.Point(0, 0)
        Me.tabCurrent.Margin = New System.Windows.Forms.Padding(4)
        Me.tabCurrent.Name = "tabCurrent"
        Me.tabCurrent.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tabCurrent.RightToLeftLayout = True
        Me.tabCurrent.SelectedIndex = 0
        Me.tabCurrent.Size = New System.Drawing.Size(1200, 760)
        Me.tabCurrent.TabIndex = 0
        Me.tabCurrent.Tag = ""
        '
        'TabInvoice
        '
        Me.TabInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TabInvoice.Controls.Add(Me.dgvInvoiceDetails)
        Me.TabInvoice.Controls.Add(Me.pnlInvoiceSupport)
        Me.TabInvoice.Controls.Add(Me.lblTotalVolume)
        Me.TabInvoice.Controls.Add(Me.pnlUp)
        Me.TabInvoice.Location = New System.Drawing.Point(4, 34)
        Me.TabInvoice.Margin = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.Name = "TabInvoice"
        Me.TabInvoice.Padding = New System.Windows.Forms.Padding(4)
        Me.TabInvoice.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.TabInvoice.Size = New System.Drawing.Size(1192, 722)
        Me.TabInvoice.TabIndex = 0
        Me.TabInvoice.Text = "الفاتورة"
        '
        'dgvInvoiceDetails
        '
        Me.dgvInvoiceDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvInvoiceDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInvoiceDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colSourceLODDetailID, Me.colDetSourceDocumentDetailID, Me.colDetUnitID, Me.colDetSRDID, Me.colDetSRID, Me.colDetBaseProductCode, Me.colDetProductID, Me.colDetProductCode, Me.colProductName, Me.colDetProductType, Me.colDetQty, Me.colDetReturnQty, Me.colDetSellUnit, Me.colDetPieceSellPrice, Me.colDetM3SellPrice, Me.colDetLineAmount, Me.colDetDiscountPercent, Me.colDetDiscountAmount, Me.colDetTaxableAmount, Me.colDetTaxPercent, Me.colDetTaxAmount, Me.colTotal, Me.colDetNote, Me.colDetDelete})
        Me.dgvInvoiceDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvInvoiceDetails.Location = New System.Drawing.Point(4, 299)
        Me.dgvInvoiceDetails.Name = "dgvInvoiceDetails"
        Me.dgvInvoiceDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvInvoiceDetails.RowHeadersVisible = False
        Me.dgvInvoiceDetails.RowHeadersWidth = 51
        Me.dgvInvoiceDetails.RowTemplate.Height = 26
        Me.dgvInvoiceDetails.Size = New System.Drawing.Size(1184, 402)
        Me.dgvInvoiceDetails.TabIndex = 20
        '
        'colSourceLODDetailID
        '
        Me.colSourceLODDetailID.DataPropertyName = "SourceLoadingOrderDetailID"
        Me.colSourceLODDetailID.HeaderText = "رقم صنف التحميل"
        Me.colSourceLODDetailID.MinimumWidth = 6
        Me.colSourceLODDetailID.Name = "colSourceLODDetailID"
        Me.colSourceLODDetailID.Visible = False
        '
        'colDetSourceDocumentDetailID
        '
        Me.colDetSourceDocumentDetailID.HeaderText = "Column1"
        Me.colDetSourceDocumentDetailID.MinimumWidth = 6
        Me.colDetSourceDocumentDetailID.Name = "colDetSourceDocumentDetailID"
        Me.colDetSourceDocumentDetailID.Visible = False
        '
        'colDetUnitID
        '
        Me.colDetUnitID.HeaderText = "Column1"
        Me.colDetUnitID.MinimumWidth = 6
        Me.colDetUnitID.Name = "colDetUnitID"
        Me.colDetUnitID.Visible = False
        '
        'colDetSRDID
        '
        Me.colDetSRDID.HeaderText = "Column1"
        Me.colDetSRDID.MinimumWidth = 6
        Me.colDetSRDID.Name = "colDetSRDID"
        Me.colDetSRDID.Visible = False
        '
        'colDetSRID
        '
        Me.colDetSRID.HeaderText = "Column1"
        Me.colDetSRID.MinimumWidth = 6
        Me.colDetSRID.Name = "colDetSRID"
        Me.colDetSRID.Visible = False
        '
        'colDetBaseProductCode
        '
        Me.colDetBaseProductCode.HeaderText = ""
        Me.colDetBaseProductCode.MinimumWidth = 6
        Me.colDetBaseProductCode.Name = "colDetBaseProductCode"
        Me.colDetBaseProductCode.Visible = False
        '
        'colDetProductID
        '
        Me.colDetProductID.HeaderText = "رقم الصنف"
        Me.colDetProductID.MinimumWidth = 6
        Me.colDetProductID.Name = "colDetProductID"
        Me.colDetProductID.Visible = False
        '
        'colDetProductCode
        '
        Me.colDetProductCode.HeaderText = "كود الصنف"
        Me.colDetProductCode.MinimumWidth = 6
        Me.colDetProductCode.Name = "colDetProductCode"
        Me.colDetProductCode.ReadOnly = True
        Me.colDetProductCode.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colDetProductCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colProductName
        '
        Me.colProductName.HeaderText = "اسم الصنف"
        Me.colProductName.MinimumWidth = 6
        Me.colProductName.Name = "colProductName"
        Me.colProductName.ReadOnly = True
        '
        'colDetProductType
        '
        Me.colDetProductType.HeaderText = "النوع"
        Me.colDetProductType.MinimumWidth = 6
        Me.colDetProductType.Name = "colDetProductType"
        Me.colDetProductType.ReadOnly = True
        '
        'colDetQty
        '
        Me.colDetQty.HeaderText = "الكمية"
        Me.colDetQty.MinimumWidth = 6
        Me.colDetQty.Name = "colDetQty"
        Me.colDetQty.ReadOnly = True
        '
        'colDetReturnQty
        '
        Me.colDetReturnQty.HeaderText = "الكمية المرتجعة"
        Me.colDetReturnQty.MinimumWidth = 6
        Me.colDetReturnQty.Name = "colDetReturnQty"
        '
        'colDetSellUnit
        '
        Me.colDetSellUnit.HeaderText = "الوحدة"
        Me.colDetSellUnit.MinimumWidth = 6
        Me.colDetSellUnit.Name = "colDetSellUnit"
        Me.colDetSellUnit.ReadOnly = True
        Me.colDetSellUnit.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colDetSellUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'colDetPieceSellPrice
        '
        Me.colDetPieceSellPrice.HeaderText = "سعر الوحدة"
        Me.colDetPieceSellPrice.MinimumWidth = 6
        Me.colDetPieceSellPrice.Name = "colDetPieceSellPrice"
        '
        'colDetM3SellPrice
        '
        Me.colDetM3SellPrice.HeaderText = "سعر المتر المكعب"
        Me.colDetM3SellPrice.MinimumWidth = 6
        Me.colDetM3SellPrice.Name = "colDetM3SellPrice"
        Me.colDetM3SellPrice.ReadOnly = True
        Me.colDetM3SellPrice.Visible = False
        '
        'colDetLineAmount
        '
        Me.colDetLineAmount.HeaderText = "المبلغ الاجمالي"
        Me.colDetLineAmount.MinimumWidth = 6
        Me.colDetLineAmount.Name = "colDetLineAmount"
        Me.colDetLineAmount.ReadOnly = True
        '
        'colDetDiscountPercent
        '
        Me.colDetDiscountPercent.HeaderText = "الخصم%"
        Me.colDetDiscountPercent.MinimumWidth = 6
        Me.colDetDiscountPercent.Name = "colDetDiscountPercent"
        '
        'colDetDiscountAmount
        '
        Me.colDetDiscountAmount.HeaderText = "مبلغ الخصم"
        Me.colDetDiscountAmount.MinimumWidth = 6
        Me.colDetDiscountAmount.Name = "colDetDiscountAmount"
        Me.colDetDiscountAmount.ReadOnly = True
        '
        'colDetTaxableAmount
        '
        Me.colDetTaxableAmount.HeaderText = "الاجمالي بعد الخصم"
        Me.colDetTaxableAmount.MinimumWidth = 6
        Me.colDetTaxableAmount.Name = "colDetTaxableAmount"
        Me.colDetTaxableAmount.ReadOnly = True
        '
        'colDetTaxPercent
        '
        Me.colDetTaxPercent.HeaderText = "الضريبة"
        Me.colDetTaxPercent.MinimumWidth = 6
        Me.colDetTaxPercent.Name = "colDetTaxPercent"
        Me.colDetTaxPercent.ReadOnly = True
        Me.colDetTaxPercent.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'colDetTaxAmount
        '
        Me.colDetTaxAmount.HeaderText = "مبلغ الضريبة"
        Me.colDetTaxAmount.MinimumWidth = 6
        Me.colDetTaxAmount.Name = "colDetTaxAmount"
        Me.colDetTaxAmount.ReadOnly = True
        '
        'colTotal
        '
        Me.colTotal.HeaderText = "اجمالي المبلغ"
        Me.colTotal.MinimumWidth = 6
        Me.colTotal.Name = "colTotal"
        Me.colTotal.ReadOnly = True
        '
        'colDetNote
        '
        Me.colDetNote.HeaderText = "ملاحظات"
        Me.colDetNote.MinimumWidth = 6
        Me.colDetNote.Name = "colDetNote"
        Me.colDetNote.ReadOnly = True
        Me.colDetNote.Visible = False
        '
        'colDetDelete
        '
        Me.colDetDelete.HeaderText = "حذف"
        Me.colDetDelete.MinimumWidth = 6
        Me.colDetDelete.Name = "colDetDelete"
        Me.colDetDelete.Visible = False
        '
        'pnlInvoiceSupport
        '
        Me.pnlInvoiceSupport.Controls.Add(Me.cboTaxReason)
        Me.pnlInvoiceSupport.Controls.Add(Me.txtInvoiceNote)
        Me.pnlInvoiceSupport.Controls.Add(Me.chkIsIncludeVAT)
        Me.pnlInvoiceSupport.Controls.Add(Me.chkIsExport)
        Me.pnlInvoiceSupport.Controls.Add(Me.Label23)
        Me.pnlInvoiceSupport.Controls.Add(Me.Label5)
        Me.pnlInvoiceSupport.Controls.Add(Me.Label4)
        Me.pnlInvoiceSupport.Controls.Add(Me.Label20)
        Me.pnlInvoiceSupport.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlInvoiceSupport.Location = New System.Drawing.Point(4, 186)
        Me.pnlInvoiceSupport.Name = "pnlInvoiceSupport"
        Me.pnlInvoiceSupport.Size = New System.Drawing.Size(1184, 113)
        Me.pnlInvoiceSupport.TabIndex = 22
        '
        'cboTaxReason
        '
        Me.cboTaxReason.Enabled = False
        Me.cboTaxReason.FormattingEnabled = True
        Me.cboTaxReason.Location = New System.Drawing.Point(14, 87)
        Me.cboTaxReason.Name = "cboTaxReason"
        Me.cboTaxReason.Size = New System.Drawing.Size(263, 24)
        Me.cboTaxReason.TabIndex = 21
        '
        'txtInvoiceNote
        '
        Me.txtInvoiceNote.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtInvoiceNote.Location = New System.Drawing.Point(617, 6)
        Me.txtInvoiceNote.Multiline = True
        Me.txtInvoiceNote.Name = "txtInvoiceNote"
        Me.txtInvoiceNote.Size = New System.Drawing.Size(458, 103)
        Me.txtInvoiceNote.TabIndex = 20
        '
        'chkIsIncludeVAT
        '
        Me.chkIsIncludeVAT.AutoSize = True
        Me.chkIsIncludeVAT.Enabled = False
        Me.chkIsIncludeVAT.Location = New System.Drawing.Point(259, 42)
        Me.chkIsIncludeVAT.Name = "chkIsIncludeVAT"
        Me.chkIsIncludeVAT.Size = New System.Drawing.Size(18, 17)
        Me.chkIsIncludeVAT.TabIndex = 18
        Me.chkIsIncludeVAT.UseVisualStyleBackColor = True
        '
        'chkIsExport
        '
        Me.chkIsExport.AutoSize = True
        Me.chkIsExport.Enabled = False
        Me.chkIsExport.Location = New System.Drawing.Point(259, 12)
        Me.chkIsExport.Name = "chkIsExport"
        Me.chkIsExport.Size = New System.Drawing.Size(18, 17)
        Me.chkIsExport.TabIndex = 17
        Me.chkIsExport.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(108, 65)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(131, 17)
        Me.Label23.TabIndex = 1
        Me.Label23.Text = "سبب الإعفاء الضريبي"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(108, 41)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(137, 17)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "الاسعار شاملة الضريبة"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(155, 11)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(90, 17)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "بضاعة للتصدير"
        '
        'Label20
        '
        Me.Label20.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(1109, 12)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(59, 17)
        Me.Label20.TabIndex = 1
        Me.Label20.Text = "ملاحظات"
        '
        'lblTotalVolume
        '
        Me.lblTotalVolume.AutoSize = True
        Me.lblTotalVolume.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblTotalVolume.Location = New System.Drawing.Point(4, 701)
        Me.lblTotalVolume.Name = "lblTotalVolume"
        Me.lblTotalVolume.Size = New System.Drawing.Size(104, 17)
        Me.lblTotalVolume.TabIndex = 23
        Me.lblTotalVolume.Text = "0.000 متر مكعب"
        '
        'pnlUp
        '
        Me.pnlUp.Controls.Add(Me.pnlTotals)
        Me.pnlUp.Controls.Add(Me.pnlPayment)
        Me.pnlUp.Controls.Add(Me.pnlPartner1)
        Me.pnlUp.Controls.Add(Me.Panel3)
        Me.pnlUp.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlUp.Location = New System.Drawing.Point(4, 4)
        Me.pnlUp.Name = "pnlUp"
        Me.pnlUp.Size = New System.Drawing.Size(1184, 182)
        Me.pnlUp.TabIndex = 21
        '
        'pnlTotals
        '
        Me.pnlTotals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlTotals.BackColor = System.Drawing.Color.Transparent
        Me.pnlTotals.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlTotals.Controls.Add(Me.txtTotalDiscount)
        Me.pnlTotals.Controls.Add(Me.txtTotalTaxableAmount)
        Me.pnlTotals.Controls.Add(Me.txtTotalAmount)
        Me.pnlTotals.Controls.Add(Me.txtTotalTax)
        Me.pnlTotals.Controls.Add(Me.txtGrandTotal)
        Me.pnlTotals.Controls.Add(Me.Label19)
        Me.pnlTotals.Controls.Add(Me.Label18)
        Me.pnlTotals.Controls.Add(Me.Label3)
        Me.pnlTotals.Controls.Add(Me.Label13)
        Me.pnlTotals.Controls.Add(Me.Label14)
        Me.pnlTotals.Location = New System.Drawing.Point(3, 8)
        Me.pnlTotals.Name = "pnlTotals"
        Me.pnlTotals.Size = New System.Drawing.Size(283, 159)
        Me.pnlTotals.TabIndex = 19
        '
        'txtTotalDiscount
        '
        Me.txtTotalDiscount.Enabled = False
        Me.txtTotalDiscount.Location = New System.Drawing.Point(10, 34)
        Me.txtTotalDiscount.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTotalDiscount.Name = "txtTotalDiscount"
        Me.txtTotalDiscount.Size = New System.Drawing.Size(164, 24)
        Me.txtTotalDiscount.TabIndex = 7
        '
        'txtTotalTaxableAmount
        '
        Me.txtTotalTaxableAmount.Enabled = False
        Me.txtTotalTaxableAmount.Location = New System.Drawing.Point(10, 63)
        Me.txtTotalTaxableAmount.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTotalTaxableAmount.Name = "txtTotalTaxableAmount"
        Me.txtTotalTaxableAmount.Size = New System.Drawing.Size(164, 24)
        Me.txtTotalTaxableAmount.TabIndex = 7
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(225, 16)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(51, 17)
        Me.Label19.TabIndex = 1
        Me.Label19.Text = "اجمالي"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(197, 44)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(79, 17)
        Me.Label18.TabIndex = 1
        Me.Label18.Text = "مبلغ  الخصم"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(189, 72)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 17)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "اجمالي ق.ض"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(198, 100)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(78, 17)
        Me.Label13.TabIndex = 1
        Me.Label13.Text = "مبلغ الضريبة"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(178, 128)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(98, 17)
        Me.Label14.TabIndex = 1
        Me.Label14.Text = "المبلغ الاجمالي"
        '
        'pnlPayment
        '
        Me.pnlPayment.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlPayment.BackColor = System.Drawing.Color.Transparent
        Me.pnlPayment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlPayment.Controls.Add(Me.txtVehicleCode)
        Me.pnlPayment.Controls.Add(Me.txtDriverName)
        Me.pnlPayment.Controls.Add(Me.txtSRCode)
        Me.pnlPayment.Controls.Add(Me.Label22)
        Me.pnlPayment.Controls.Add(Me.Label21)
        Me.pnlPayment.Controls.Add(Me.cboSourceStoreID)
        Me.pnlPayment.Controls.Add(Me.Label12)
        Me.pnlPayment.Controls.Add(Me.Label11)
        Me.pnlPayment.Location = New System.Drawing.Point(599, 8)
        Me.pnlPayment.Name = "pnlPayment"
        Me.pnlPayment.Size = New System.Drawing.Size(283, 159)
        Me.pnlPayment.TabIndex = 19
        '
        'txtVehicleCode
        '
        Me.txtVehicleCode.Enabled = False
        Me.txtVehicleCode.Location = New System.Drawing.Point(17, 95)
        Me.txtVehicleCode.Name = "txtVehicleCode"
        Me.txtVehicleCode.Size = New System.Drawing.Size(165, 24)
        Me.txtVehicleCode.TabIndex = 15
        '
        'txtDriverName
        '
        Me.txtDriverName.Enabled = False
        Me.txtDriverName.Location = New System.Drawing.Point(17, 66)
        Me.txtDriverName.Name = "txtDriverName"
        Me.txtDriverName.Size = New System.Drawing.Size(165, 24)
        Me.txtDriverName.TabIndex = 15
        '
        'txtSRCode
        '
        Me.txtSRCode.Enabled = False
        Me.txtSRCode.Location = New System.Drawing.Point(17, 8)
        Me.txtSRCode.Name = "txtSRCode"
        Me.txtSRCode.Size = New System.Drawing.Size(165, 24)
        Me.txtSRCode.TabIndex = 14
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(228, 72)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(50, 17)
        Me.Label22.TabIndex = 1
        Me.Label22.Text = "السائق"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(227, 99)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(51, 17)
        Me.Label21.TabIndex = 1
        Me.Label21.Text = "السيارة"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(191, 15)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(87, 17)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "طلب المبيعات"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(213, 44)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(65, 17)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "المستودع"
        '
        'pnlPartner1
        '
        Me.pnlPartner1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlPartner1.BackColor = System.Drawing.Color.Transparent
        Me.pnlPartner1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlPartner1.Controls.Add(Me.txtPartnerName)
        Me.pnlPartner1.Controls.Add(Me.cboPartnerID)
        Me.pnlPartner1.Controls.Add(Me.txtVATRegistrationNumber)
        Me.pnlPartner1.Controls.Add(Me.btnAddPartner)
        Me.pnlPartner1.Controls.Add(Me.btnFindPartner)
        Me.pnlPartner1.Controls.Add(Me.Label2)
        Me.pnlPartner1.Controls.Add(Me.Label15)
        Me.pnlPartner1.Controls.Add(Me.Label6)
        Me.pnlPartner1.Location = New System.Drawing.Point(301, 8)
        Me.pnlPartner1.Name = "pnlPartner1"
        Me.pnlPartner1.Size = New System.Drawing.Size(283, 159)
        Me.pnlPartner1.TabIndex = 19
        '
        'txtPartnerName
        '
        Me.txtPartnerName.Enabled = False
        Me.txtPartnerName.Location = New System.Drawing.Point(11, 40)
        Me.txtPartnerName.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPartnerName.Name = "txtPartnerName"
        Me.txtPartnerName.Size = New System.Drawing.Size(165, 24)
        Me.txtPartnerName.TabIndex = 0
        '
        'txtVATRegistrationNumber
        '
        Me.txtVATRegistrationNumber.Enabled = False
        Me.txtVATRegistrationNumber.Location = New System.Drawing.Point(11, 70)
        Me.txtVATRegistrationNumber.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATRegistrationNumber.Name = "txtVATRegistrationNumber"
        Me.txtVATRegistrationNumber.Size = New System.Drawing.Size(165, 24)
        Me.txtVATRegistrationNumber.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Enabled = False
        Me.Label2.Location = New System.Drawing.Point(196, 46)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 17)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "إسم العميل"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Enabled = False
        Me.Label15.Location = New System.Drawing.Point(203, 17)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(70, 17)
        Me.Label15.TabIndex = 1
        Me.Label15.Text = "كود العميل"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Enabled = False
        Me.Label6.Location = New System.Drawing.Point(185, 75)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 17)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "الرقم الضريبي"
        '
        'Panel3
        '
        Me.Panel3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel3.BackColor = System.Drawing.Color.Transparent
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.txtInvoicCode)
        Me.Panel3.Controls.Add(Me.cboPaymentMethodID)
        Me.Panel3.Controls.Add(Me.cboPaymentTerm)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.Label16)
        Me.Panel3.Controls.Add(Me.dtpDocumentDate)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Location = New System.Drawing.Point(897, 8)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(283, 159)
        Me.Panel3.TabIndex = 18
        '
        'txtInvoicCode
        '
        Me.txtInvoicCode.Enabled = False
        Me.txtInvoicCode.Location = New System.Drawing.Point(12, 4)
        Me.txtInvoicCode.Name = "txtInvoicCode"
        Me.txtInvoicCode.Size = New System.Drawing.Size(165, 24)
        Me.txtInvoicCode.TabIndex = 16
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(195, 97)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(75, 17)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "طبيعة الدفع"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(191, 126)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(79, 17)
        Me.Label16.TabIndex = 1
        Me.Label16.Text = "طريقة الدفع "
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(198, 10)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "كود الفاتورة"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(187, 39)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(83, 17)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = "تاريخ الفاتورة "
        '
        'frmSalesReturn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1324, 760)
        Me.Controls.Add(Me.pnlInvoice)
        Me.Controls.Add(Me.pnlControl)
        Me.Name = "frmSalesReturn"
        Me.Text = "frmInvoice"
        Me.pnlControl.ResumeLayout(False)
        Me.pnlInvoice.ResumeLayout(False)
        Me.tabCurrent.ResumeLayout(False)
        Me.TabInvoice.ResumeLayout(False)
        Me.TabInvoice.PerformLayout()
        CType(Me.dgvInvoiceDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlInvoiceSupport.ResumeLayout(False)
        Me.pnlInvoiceSupport.PerformLayout()
        Me.pnlUp.ResumeLayout(False)
        Me.pnlTotals.ResumeLayout(False)
        Me.pnlTotals.PerformLayout()
        Me.pnlPayment.ResumeLayout(False)
        Me.pnlPayment.PerformLayout()
        Me.pnlPartner1.ResumeLayout(False)
        Me.pnlPartner1.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnPrint As Button
    Friend WithEvents pnlControl As Panel
    Friend WithEvents btnNewInvoice As Button
    Friend WithEvents btnSaveDraft As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnGetOriginalInvoice As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents tabDraftInvoices As TabPage
    Friend WithEvents cboStatusID As ComboBox
    Friend WithEvents cboPaymentMethodID As ComboBox
    Friend WithEvents btnAddPartner As Button
    Friend WithEvents btnFindPartner As Button
    Friend WithEvents cboSourceStoreID As ComboBox
    Friend WithEvents cboPaymentTerm As ComboBox
    Friend WithEvents dtpDocumentDate As DateTimePicker
    Friend WithEvents txtGrandTotal As TextBox
    Friend WithEvents txtTotalTax As TextBox
    Friend WithEvents txtTotalAmount As TextBox
    Friend WithEvents cboPartnerID As ComboBox
    Friend WithEvents pnlInvoice As Panel
    Friend WithEvents tabCurrent As TabControl
    Friend WithEvents TabInvoice As TabPage
    Friend WithEvents Label6 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtVATRegistrationNumber As TextBox
    Friend WithEvents txtPartnerName As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents pnlPartner1 As Panel
    Friend WithEvents pnlTotals As Panel
    Friend WithEvents txtTotalTaxableAmount As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents txtTotalDiscount As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents pnlPayment As Panel
    Friend WithEvents pnlUp As Panel
    Friend WithEvents dgvInvoiceDetails As DataGridView
    Friend WithEvents Label21 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents chkIsExport As CheckBox
    Friend WithEvents Label4 As Label
    Friend WithEvents pnlInvoiceSupport As Panel
    Friend WithEvents chkIsIncludeVAT As CheckBox
    Friend WithEvents Label5 As Label
    Friend WithEvents lblTotalVolume As Label
    Friend WithEvents txtSRCode As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents txtInvoiceNote As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents txtDriverName As TextBox
    Friend WithEvents txtVehicleCode As TextBox
    Friend WithEvents txtInvoicCode As TextBox
    Friend WithEvents cboTaxReason As ComboBox
    Friend WithEvents Label23 As Label
    Friend WithEvents btnPostReturnInvoice As Button
    Friend WithEvents colSourceLODDetailID As DataGridViewTextBoxColumn
    Friend WithEvents colDetSourceDocumentDetailID As DataGridViewTextBoxColumn
    Friend WithEvents colDetUnitID As DataGridViewTextBoxColumn
    Friend WithEvents colDetSRDID As DataGridViewTextBoxColumn
    Friend WithEvents colDetSRID As DataGridViewTextBoxColumn
    Friend WithEvents colDetBaseProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductID As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colProductName As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductType As DataGridViewComboBoxColumn
    Friend WithEvents colDetQty As DataGridViewTextBoxColumn
    Friend WithEvents colDetReturnQty As DataGridViewTextBoxColumn
    Friend WithEvents colDetSellUnit As DataGridViewTextBoxColumn
    Friend WithEvents colDetPieceSellPrice As DataGridViewTextBoxColumn
    Friend WithEvents colDetM3SellPrice As DataGridViewTextBoxColumn
    Friend WithEvents colDetLineAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetDiscountPercent As DataGridViewTextBoxColumn
    Friend WithEvents colDetDiscountAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetTaxableAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetTaxPercent As DataGridViewComboBoxColumn
    Friend WithEvents colDetTaxAmount As DataGridViewTextBoxColumn
    Friend WithEvents colTotal As DataGridViewTextBoxColumn
    Friend WithEvents colDetNote As DataGridViewTextBoxColumn
    Friend WithEvents colDetDelete As DataGridViewButtonColumn
    Friend WithEvents btnSearch As Button
End Class
