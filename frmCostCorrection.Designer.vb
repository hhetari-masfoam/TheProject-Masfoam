<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCostCorrection
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
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabPUR = New System.Windows.Forms.TabPage()
        Me.dgvMain = New System.Windows.Forms.DataGridView()
        Me.colProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnitID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTaxableAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVATRate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colVATAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTotalAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.txtPartnerName = New System.Windows.Forms.TextBox()
        Me.cboVATRate = New System.Windows.Forms.ComboBox()
        Me.cboDocumentID = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.dtpDocumentDate = New System.Windows.Forms.DateTimePicker()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cboPaymentTerm = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtVATTotal = New System.Windows.Forms.TextBox()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.cboTargetStore = New System.Windows.Forms.ComboBox()
        Me.chkIsTaxInclusive = New System.Windows.Forms.CheckBox()
        Me.txtSubTotal = New System.Windows.Forms.TextBox()
        Me.cboPartnerCode = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtGrandTotal = New System.Windows.Forms.TextBox()
        Me.txtExchangeRate = New System.Windows.Forms.TextBox()
        Me.txtCurrencyID = New System.Windows.Forms.TextBox()
        Me.tabPRO = New System.Windows.Forms.TabPage()
        Me.tabCUT = New System.Windows.Forms.TabPage()
        Me.tabSCR = New System.Windows.Forms.TabPage()
        Me.tabCOR = New System.Windows.Forms.TabPage()
        Me.tabAdvancedAnalysis = New System.Windows.Forms.TabPage()
        Me.pnlControls = New System.Windows.Forms.Panel()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.pnlMode = New System.Windows.Forms.Panel()
        Me.rbtnCancelMode = New System.Windows.Forms.RadioButton()
        Me.rbtnEditMode = New System.Windows.Forms.RadioButton()
        Me.btnAdvanceAnalysis = New System.Windows.Forms.Button()
        Me.btnExcute = New System.Windows.Forms.Button()
        Me.btnLoadOriginalDocument = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnAffectedOperations = New System.Windows.Forms.Button()
        Me.btnPreveiw = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.dgvAffectedOperations = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvAdjustResult = New System.Windows.Forms.DataGridView()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.dgvSimulation = New System.Windows.Forms.DataGridView()
        Me.tabMain.SuspendLayout()
        Me.tabPUR.SuspendLayout()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel3.SuspendLayout()
        Me.pnlControls.SuspendLayout()
        Me.pnlMode.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.dgvAffectedOperations, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvAdjustResult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.dgvSimulation, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabPUR)
        Me.tabMain.Controls.Add(Me.tabPRO)
        Me.tabMain.Controls.Add(Me.tabCUT)
        Me.tabMain.Controls.Add(Me.tabSCR)
        Me.tabMain.Controls.Add(Me.tabCOR)
        Me.tabMain.Controls.Add(Me.tabAdvancedAnalysis)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tabMain.RightToLeftLayout = True
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1448, 285)
        Me.tabMain.TabIndex = 0
        '
        'tabPUR
        '
        Me.tabPUR.Controls.Add(Me.dgvMain)
        Me.tabPUR.Controls.Add(Me.Panel3)
        Me.tabPUR.Controls.Add(Me.txtExchangeRate)
        Me.tabPUR.Controls.Add(Me.txtCurrencyID)
        Me.tabPUR.Location = New System.Drawing.Point(4, 25)
        Me.tabPUR.Name = "tabPUR"
        Me.tabPUR.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPUR.Size = New System.Drawing.Size(1440, 256)
        Me.tabPUR.TabIndex = 0
        Me.tabPUR.Text = "المشتريات"
        Me.tabPUR.UseVisualStyleBackColor = True
        '
        'dgvMain
        '
        Me.dgvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductCode, Me.colProductType, Me.colProductID, Me.colProductName, Me.colUnitPrice, Me.colQty, Me.colUnitID, Me.colTaxableAmount, Me.colVATRate, Me.colVATAmount, Me.colTotalAmount})
        Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvMain.Location = New System.Drawing.Point(3, 85)
        Me.dgvMain.Margin = New System.Windows.Forms.Padding(4)
        Me.dgvMain.Name = "dgvMain"
        Me.dgvMain.RowHeadersWidth = 51
        Me.dgvMain.RowTemplate.Height = 26
        Me.dgvMain.Size = New System.Drawing.Size(1434, 168)
        Me.dgvMain.TabIndex = 3
        '
        'colProductCode
        '
        Me.colProductCode.HeaderText = "كود الصنف"
        Me.colProductCode.MinimumWidth = 6
        Me.colProductCode.Name = "colProductCode"
        Me.colProductCode.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'colProductType
        '
        Me.colProductType.HeaderText = "النوع"
        Me.colProductType.MinimumWidth = 6
        Me.colProductType.Name = "colProductType"
        Me.colProductType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
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
        Me.colUnitID.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colUnitID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
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
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel3.Controls.Add(Me.txtPartnerName)
        Me.Panel3.Controls.Add(Me.cboVATRate)
        Me.Panel3.Controls.Add(Me.cboDocumentID)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.dtpDocumentDate)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.Label14)
        Me.Panel3.Controls.Add(Me.cboPaymentTerm)
        Me.Panel3.Controls.Add(Me.Label7)
        Me.Panel3.Controls.Add(Me.txtVATTotal)
        Me.Panel3.Controls.Add(Me.txtPhone)
        Me.Panel3.Controls.Add(Me.cboTargetStore)
        Me.Panel3.Controls.Add(Me.chkIsTaxInclusive)
        Me.Panel3.Controls.Add(Me.txtSubTotal)
        Me.Panel3.Controls.Add(Me.cboPartnerCode)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.Label3)
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.Label18)
        Me.Panel3.Controls.Add(Me.Label17)
        Me.Panel3.Controls.Add(Me.Label15)
        Me.Panel3.Controls.Add(Me.Label13)
        Me.Panel3.Controls.Add(Me.txtGrandTotal)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(3, 3)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1434, 82)
        Me.Panel3.TabIndex = 4
        '
        'txtPartnerName
        '
        Me.txtPartnerName.Location = New System.Drawing.Point(711, 27)
        Me.txtPartnerName.Name = "txtPartnerName"
        Me.txtPartnerName.Size = New System.Drawing.Size(267, 24)
        Me.txtPartnerName.TabIndex = 27
        '
        'cboVATRate
        '
        Me.cboVATRate.FormattingEnabled = True
        Me.cboVATRate.Location = New System.Drawing.Point(360, 27)
        Me.cboVATRate.Name = "cboVATRate"
        Me.cboVATRate.Size = New System.Drawing.Size(189, 24)
        Me.cboVATRate.TabIndex = 26
        '
        'cboDocumentID
        '
        Me.cboDocumentID.FormattingEnabled = True
        Me.cboDocumentID.Location = New System.Drawing.Point(1122, 3)
        Me.cboDocumentID.Name = "cboDocumentID"
        Me.cboDocumentID.Size = New System.Drawing.Size(172, 24)
        Me.cboDocumentID.TabIndex = 24
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(1336, 58)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(75, 17)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "طبيعة الدفع"
        '
        'dtpDocumentDate
        '
        Me.dtpDocumentDate.Location = New System.Drawing.Point(1122, 27)
        Me.dtpDocumentDate.Name = "dtpDocumentDate"
        Me.dtpDocumentDate.Size = New System.Drawing.Size(172, 24)
        Me.dtpDocumentDate.TabIndex = 9
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(1328, 34)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(83, 17)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = "تاريخ الفاتورة "
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(237, 58)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(98, 17)
        Me.Label14.TabIndex = 1
        Me.Label14.Text = "المبلغ الاجمالي"
        '
        'cboPaymentTerm
        '
        Me.cboPaymentTerm.FormattingEnabled = True
        Me.cboPaymentTerm.Location = New System.Drawing.Point(1122, 51)
        Me.cboPaymentTerm.Name = "cboPaymentTerm"
        Me.cboPaymentTerm.Size = New System.Drawing.Size(172, 24)
        Me.cboPaymentTerm.TabIndex = 11
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(1338, 10)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(73, 17)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "رقم الفاتورة"
        '
        'txtVATTotal
        '
        Me.txtVATTotal.Enabled = False
        Me.txtVATTotal.Location = New System.Drawing.Point(6, 27)
        Me.txtVATTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtVATTotal.Name = "txtVATTotal"
        Me.txtVATTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtVATTotal.TabIndex = 7
        '
        'txtPhone
        '
        Me.txtPhone.Enabled = False
        Me.txtPhone.Location = New System.Drawing.Point(711, 51)
        Me.txtPhone.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(267, 24)
        Me.txtPhone.TabIndex = 0
        '
        'cboTargetStore
        '
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(360, 3)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(189, 24)
        Me.cboTargetStore.TabIndex = 17
        '
        'chkIsTaxInclusive
        '
        Me.chkIsTaxInclusive.AutoSize = True
        Me.chkIsTaxInclusive.Location = New System.Drawing.Point(569, 36)
        Me.chkIsTaxInclusive.Name = "chkIsTaxInclusive"
        Me.chkIsTaxInclusive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsTaxInclusive.TabIndex = 18
        Me.chkIsTaxInclusive.UseVisualStyleBackColor = True
        '
        'txtSubTotal
        '
        Me.txtSubTotal.Enabled = False
        Me.txtSubTotal.Location = New System.Drawing.Point(6, 3)
        Me.txtSubTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSubTotal.Name = "txtSubTotal"
        Me.txtSubTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtSubTotal.TabIndex = 7
        '
        'cboPartnerCode
        '
        Me.cboPartnerCode.FormattingEnabled = True
        Me.cboPartnerCode.Location = New System.Drawing.Point(711, 3)
        Me.cboPartnerCode.Margin = New System.Windows.Forms.Padding(4)
        Me.cboPartnerCode.Name = "cboPartnerCode"
        Me.cboPartnerCode.Size = New System.Drawing.Size(267, 24)
        Me.cboPartnerCode.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1014, 34)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 17)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "إسم المورد"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(214, 10)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 17)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "اجمالي قبل الضريبة"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(1018, 58)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 17)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "رقم الهاتف"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(601, 35)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(88, 17)
        Me.Label18.TabIndex = 1
        Me.Label18.Text = "شامل الضريبة"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(569, 10)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(120, 17)
        Me.Label17.TabIndex = 1
        Me.Label17.Text = "المستودع المستلم"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(1018, 10)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(66, 17)
        Me.Label15.TabIndex = 1
        Me.Label15.Text = "كود المورد"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(239, 34)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(96, 17)
        Me.Label13.TabIndex = 1
        Me.Label13.Text = "اجمالي الضريبة"
        '
        'txtGrandTotal
        '
        Me.txtGrandTotal.Enabled = False
        Me.txtGrandTotal.Location = New System.Drawing.Point(6, 51)
        Me.txtGrandTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGrandTotal.Name = "txtGrandTotal"
        Me.txtGrandTotal.Size = New System.Drawing.Size(189, 24)
        Me.txtGrandTotal.TabIndex = 7
        '
        'txtExchangeRate
        '
        Me.txtExchangeRate.Location = New System.Drawing.Point(3, 194)
        Me.txtExchangeRate.Name = "txtExchangeRate"
        Me.txtExchangeRate.Size = New System.Drawing.Size(23, 24)
        Me.txtExchangeRate.TabIndex = 21
        Me.txtExchangeRate.Visible = False
        '
        'txtCurrencyID
        '
        Me.txtCurrencyID.Location = New System.Drawing.Point(3, 187)
        Me.txtCurrencyID.Name = "txtCurrencyID"
        Me.txtCurrencyID.Size = New System.Drawing.Size(23, 24)
        Me.txtCurrencyID.TabIndex = 21
        Me.txtCurrencyID.Visible = False
        '
        'tabPRO
        '
        Me.tabPRO.Location = New System.Drawing.Point(4, 25)
        Me.tabPRO.Name = "tabPRO"
        Me.tabPRO.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPRO.Size = New System.Drawing.Size(1440, 256)
        Me.tabPRO.TabIndex = 1
        Me.tabPRO.Text = "الانتاج"
        Me.tabPRO.UseVisualStyleBackColor = True
        '
        'tabCUT
        '
        Me.tabCUT.Location = New System.Drawing.Point(4, 25)
        Me.tabCUT.Name = "tabCUT"
        Me.tabCUT.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCUT.Size = New System.Drawing.Size(1440, 256)
        Me.tabCUT.TabIndex = 2
        Me.tabCUT.Text = "القص"
        Me.tabCUT.UseVisualStyleBackColor = True
        '
        'tabSCR
        '
        Me.tabSCR.Location = New System.Drawing.Point(4, 25)
        Me.tabSCR.Name = "tabSCR"
        Me.tabSCR.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSCR.Size = New System.Drawing.Size(1440, 256)
        Me.tabSCR.TabIndex = 3
        Me.tabSCR.Text = "السكراب"
        Me.tabSCR.UseVisualStyleBackColor = True
        '
        'tabCOR
        '
        Me.tabCOR.Location = New System.Drawing.Point(4, 25)
        Me.tabCOR.Name = "tabCOR"
        Me.tabCOR.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCOR.Size = New System.Drawing.Size(1440, 256)
        Me.tabCOR.TabIndex = 4
        Me.tabCOR.Text = "تصحيح التكلفة"
        Me.tabCOR.UseVisualStyleBackColor = True
        '
        'tabAdvancedAnalysis
        '
        Me.tabAdvancedAnalysis.Location = New System.Drawing.Point(4, 25)
        Me.tabAdvancedAnalysis.Name = "tabAdvancedAnalysis"
        Me.tabAdvancedAnalysis.Padding = New System.Windows.Forms.Padding(3)
        Me.tabAdvancedAnalysis.Size = New System.Drawing.Size(1440, 256)
        Me.tabAdvancedAnalysis.TabIndex = 5
        Me.tabAdvancedAnalysis.Text = "تحليل متقدم"
        Me.tabAdvancedAnalysis.UseVisualStyleBackColor = True
        '
        'pnlControls
        '
        Me.pnlControls.BackColor = System.Drawing.SystemColors.Info
        Me.pnlControls.Controls.Add(Me.txtStatusName)
        Me.pnlControls.Controls.Add(Me.pnlMode)
        Me.pnlControls.Controls.Add(Me.btnAdvanceAnalysis)
        Me.pnlControls.Controls.Add(Me.btnExcute)
        Me.pnlControls.Controls.Add(Me.btnLoadOriginalDocument)
        Me.pnlControls.Controls.Add(Me.btnNew)
        Me.pnlControls.Controls.Add(Me.btnAffectedOperations)
        Me.pnlControls.Controls.Add(Me.btnPreveiw)
        Me.pnlControls.Controls.Add(Me.btnCancel)
        Me.pnlControls.Controls.Add(Me.btnSearch)
        Me.pnlControls.Controls.Add(Me.btnPrint)
        Me.pnlControls.Controls.Add(Me.btnClose)
        Me.pnlControls.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlControls.Location = New System.Drawing.Point(1448, 0)
        Me.pnlControls.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlControls.Name = "pnlControls"
        Me.pnlControls.Size = New System.Drawing.Size(134, 1055)
        Me.pnlControls.TabIndex = 5
        '
        'txtStatusName
        '
        Me.txtStatusName.Enabled = False
        Me.txtStatusName.Location = New System.Drawing.Point(5, 12)
        Me.txtStatusName.Name = "txtStatusName"
        Me.txtStatusName.Size = New System.Drawing.Size(114, 24)
        Me.txtStatusName.TabIndex = 22
        '
        'pnlMode
        '
        Me.pnlMode.Controls.Add(Me.rbtnCancelMode)
        Me.pnlMode.Controls.Add(Me.rbtnEditMode)
        Me.pnlMode.Location = New System.Drawing.Point(5, 51)
        Me.pnlMode.Name = "pnlMode"
        Me.pnlMode.Size = New System.Drawing.Size(114, 73)
        Me.pnlMode.TabIndex = 1
        '
        'rbtnCancelMode
        '
        Me.rbtnCancelMode.AutoSize = True
        Me.rbtnCancelMode.Location = New System.Drawing.Point(25, 28)
        Me.rbtnCancelMode.Name = "rbtnCancelMode"
        Me.rbtnCancelMode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.rbtnCancelMode.Size = New System.Drawing.Size(58, 21)
        Me.rbtnCancelMode.TabIndex = 0
        Me.rbtnCancelMode.TabStop = True
        Me.rbtnCancelMode.Text = "حذف"
        Me.rbtnCancelMode.UseVisualStyleBackColor = True
        '
        'rbtnEditMode
        '
        Me.rbtnEditMode.AutoSize = True
        Me.rbtnEditMode.Location = New System.Drawing.Point(21, 3)
        Me.rbtnEditMode.Name = "rbtnEditMode"
        Me.rbtnEditMode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.rbtnEditMode.Size = New System.Drawing.Size(62, 21)
        Me.rbtnEditMode.TabIndex = 0
        Me.rbtnEditMode.TabStop = True
        Me.rbtnEditMode.Text = "تعديل"
        Me.rbtnEditMode.UseVisualStyleBackColor = True
        '
        'btnAdvanceAnalysis
        '
        Me.btnAdvanceAnalysis.Location = New System.Drawing.Point(7, 536)
        Me.btnAdvanceAnalysis.Name = "btnAdvanceAnalysis"
        Me.btnAdvanceAnalysis.Size = New System.Drawing.Size(115, 58)
        Me.btnAdvanceAnalysis.TabIndex = 1
        Me.btnAdvanceAnalysis.Text = "تحليل متقدم"
        Me.btnAdvanceAnalysis.UseVisualStyleBackColor = True
        '
        'btnExcute
        '
        Me.btnExcute.Location = New System.Drawing.Point(7, 472)
        Me.btnExcute.Name = "btnExcute"
        Me.btnExcute.Size = New System.Drawing.Size(115, 58)
        Me.btnExcute.TabIndex = 1
        Me.btnExcute.Text = "تنفيذ"
        Me.btnExcute.UseVisualStyleBackColor = True
        '
        'btnLoadOriginalDocument
        '
        Me.btnLoadOriginalDocument.BackColor = System.Drawing.SystemColors.Control
        Me.btnLoadOriginalDocument.Location = New System.Drawing.Point(7, 148)
        Me.btnLoadOriginalDocument.Margin = New System.Windows.Forms.Padding(4)
        Me.btnLoadOriginalDocument.Name = "btnLoadOriginalDocument"
        Me.btnLoadOriginalDocument.Size = New System.Drawing.Size(115, 120)
        Me.btnLoadOriginalDocument.TabIndex = 0
        Me.btnLoadOriginalDocument.Text = "جلب السند الخاطئ"
        Me.btnLoadOriginalDocument.UseVisualStyleBackColor = False
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.SystemColors.Control
        Me.btnNew.Location = New System.Drawing.Point(7, 693)
        Me.btnNew.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(115, 58)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnAffectedOperations
        '
        Me.btnAffectedOperations.BackColor = System.Drawing.SystemColors.Control
        Me.btnAffectedOperations.Location = New System.Drawing.Point(7, 341)
        Me.btnAffectedOperations.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAffectedOperations.Name = "btnAffectedOperations"
        Me.btnAffectedOperations.Size = New System.Drawing.Size(115, 58)
        Me.btnAffectedOperations.TabIndex = 0
        Me.btnAffectedOperations.Text = "العمليات المتاثرة"
        Me.btnAffectedOperations.UseVisualStyleBackColor = False
        '
        'btnPreveiw
        '
        Me.btnPreveiw.BackColor = System.Drawing.SystemColors.Control
        Me.btnPreveiw.Location = New System.Drawing.Point(7, 407)
        Me.btnPreveiw.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPreveiw.Name = "btnPreveiw"
        Me.btnPreveiw.Size = New System.Drawing.Size(115, 58)
        Me.btnPreveiw.TabIndex = 0
        Me.btnPreveiw.Text = "محاكاة"
        Me.btnPreveiw.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Location = New System.Drawing.Point(7, 984)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(115, 58)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Location = New System.Drawing.Point(7, 759)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(115, 58)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Location = New System.Drawing.Point(7, 825)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(115, 58)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.TabStop = False
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Location = New System.Drawing.Point(7, 891)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(115, 58)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tabMain)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1448, 285)
        Me.Panel1.TabIndex = 8
        '
        'dgvAffectedOperations
        '
        Me.dgvAffectedOperations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvAffectedOperations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAffectedOperations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvAffectedOperations.Location = New System.Drawing.Point(0, 0)
        Me.dgvAffectedOperations.Name = "dgvAffectedOperations"
        Me.dgvAffectedOperations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvAffectedOperations.RowHeadersWidth = 51
        Me.dgvAffectedOperations.RowTemplate.Height = 26
        Me.dgvAffectedOperations.Size = New System.Drawing.Size(1448, 197)
        Me.dgvAffectedOperations.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 285)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvAdjustResult)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(1448, 770)
        Me.SplitContainer1.SplitterDistance = 173
        Me.SplitContainer1.TabIndex = 9
        '
        'dgvAdjustResult
        '
        Me.dgvAdjustResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvAdjustResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAdjustResult.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvAdjustResult.Location = New System.Drawing.Point(0, 0)
        Me.dgvAdjustResult.Name = "dgvAdjustResult"
        Me.dgvAdjustResult.ReadOnly = True
        Me.dgvAdjustResult.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvAdjustResult.RowHeadersWidth = 51
        Me.dgvAdjustResult.RowTemplate.Height = 26
        Me.dgvAdjustResult.Size = New System.Drawing.Size(1448, 173)
        Me.dgvAdjustResult.TabIndex = 6
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.dgvAffectedOperations)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.dgvSimulation)
        Me.SplitContainer2.Size = New System.Drawing.Size(1448, 593)
        Me.SplitContainer2.SplitterDistance = 197
        Me.SplitContainer2.TabIndex = 1
        '
        'dgvSimulation
        '
        Me.dgvSimulation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSimulation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSimulation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSimulation.Location = New System.Drawing.Point(0, 0)
        Me.dgvSimulation.Name = "dgvSimulation"
        Me.dgvSimulation.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSimulation.RowHeadersWidth = 51
        Me.dgvSimulation.RowTemplate.Height = 26
        Me.dgvSimulation.Size = New System.Drawing.Size(1448, 392)
        Me.dgvSimulation.TabIndex = 0
        '
        'frmCostCorrection
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 1055)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.pnlControls)
        Me.Name = "frmCostCorrection"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmRevaluation"
        Me.tabMain.ResumeLayout(False)
        Me.tabPUR.ResumeLayout(False)
        Me.tabPUR.PerformLayout()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.pnlControls.ResumeLayout(False)
        Me.pnlControls.PerformLayout()
        Me.pnlMode.ResumeLayout(False)
        Me.pnlMode.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.dgvAffectedOperations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvAdjustResult, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.dgvSimulation, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabMain As TabControl
    Friend WithEvents tabPUR As TabPage
    Friend WithEvents pnlMode As Panel
    Friend WithEvents tabPRO As TabPage
    Friend WithEvents rbtnCancelMode As RadioButton
    Friend WithEvents rbtnEditMode As RadioButton
    Friend WithEvents pnlControls As Panel
    Friend WithEvents txtStatusName As TextBox
    Friend WithEvents btnExcute As Button
    Friend WithEvents btnPreveiw As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnClose As Button
    Private WithEvents btnLoadOriginalDocument As Button
    Private WithEvents btnNew As Button
    Friend WithEvents Label14 As Label
    Friend WithEvents txtGrandTotal As TextBox
    Friend WithEvents txtExchangeRate As TextBox
    Friend WithEvents dgvMain As DataGridView
    Friend WithEvents Panel3 As Panel
    Friend WithEvents txtCurrencyID As TextBox
    Friend WithEvents chkIsTaxInclusive As CheckBox
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents txtPhone As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents cboPaymentTerm As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents dtpDocumentDate As DateTimePicker
    Friend WithEvents Label10 As Label
    Friend WithEvents txtVATTotal As TextBox
    Friend WithEvents txtSubTotal As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents cboDocumentID As ComboBox
    Friend WithEvents cboVATRate As ComboBox
    Friend WithEvents cboPartnerCode As ComboBox
    Friend WithEvents txtPartnerName As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents dgvAffectedOperations As DataGridView
    Friend WithEvents btnAffectedOperations As Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvSimulation As DataGridView
    Friend WithEvents dgvAdjustResult As DataGridView
    Friend WithEvents colProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colProductType As DataGridViewTextBoxColumn
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colProductName As DataGridViewTextBoxColumn
    Friend WithEvents colUnitPrice As DataGridViewTextBoxColumn
    Friend WithEvents colQty As DataGridViewTextBoxColumn
    Friend WithEvents colUnitID As DataGridViewTextBoxColumn
    Friend WithEvents colTaxableAmount As DataGridViewTextBoxColumn
    Friend WithEvents colVATRate As DataGridViewTextBoxColumn
    Friend WithEvents colVATAmount As DataGridViewTextBoxColumn
    Friend WithEvents colTotalAmount As DataGridViewTextBoxColumn
    Friend WithEvents tabCUT As TabPage
    Friend WithEvents tabSCR As TabPage
    Friend WithEvents tabCOR As TabPage
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents tabAdvancedAnalysis As TabPage
    Friend WithEvents btnAdvanceAnalysis As Button
End Class
