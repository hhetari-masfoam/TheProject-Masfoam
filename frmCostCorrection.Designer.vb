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
        Me.tabPRO = New System.Windows.Forms.TabPage()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.dgvProduced = New System.Windows.Forms.DataGridView()
        Me.dgvProductionCalculations = New System.Windows.Forms.DataGridView()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.txtProductionCode = New System.Windows.Forms.TextBox()
        Me.txtProductID = New System.Windows.Forms.TextBox()
        Me.txtProductionAmount = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.txtProductionUnit = New System.Windows.Forms.TextBox()
        Me.txtTotalChemicalQty = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.txtTotalProductionVolume = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtTotalProductionCost = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtTotalProductionQTY = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtProductUnitCost = New System.Windows.Forms.TextBox()
        Me.txtPastProductAverageCost = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
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
        Me.tabCUT = New System.Windows.Forms.TabPage()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.dgvOutPut = New System.Windows.Forms.DataGridView()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.txtSourceStore = New System.Windows.Forms.TextBox()
        Me.txtStoreID = New System.Windows.Forms.Label()
        Me.txtAvailableQTY = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.dtpDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpCuttingDate = New System.Windows.Forms.DateTimePicker()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.txtProductCode = New System.Windows.Forms.TextBox()
        Me.txtTotalVolumeOutPut = New System.Windows.Forms.TextBox()
        Me.txtCuttingCode = New System.Windows.Forms.TextBox()
        Me.TotalPcsOutPut = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.tabSRT = New System.Windows.Forms.TabPage()
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
        Me.pnlOperations = New System.Windows.Forms.Panel()
        Me.dgvAffectedOperations = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvAdjustResult = New System.Windows.Forms.DataGridView()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.dgvSimulation = New System.Windows.Forms.DataGridView()
        Me.tabMain.SuspendLayout()
        Me.tabPRO.SuspendLayout()
        Me.Panel4.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.dgvProduced, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvProductionCalculations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.tabPUR.SuspendLayout()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel3.SuspendLayout()
        Me.tabCUT.SuspendLayout()
        Me.Panel6.SuspendLayout()
        CType(Me.dgvOutPut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        Me.pnlControls.SuspendLayout()
        Me.pnlMode.SuspendLayout()
        Me.pnlOperations.SuspendLayout()
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
        Me.tabMain.Controls.Add(Me.tabPRO)
        Me.tabMain.Controls.Add(Me.tabPUR)
        Me.tabMain.Controls.Add(Me.tabCUT)
        Me.tabMain.Controls.Add(Me.tabSRT)
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
        'tabPRO
        '
        Me.tabPRO.Controls.Add(Me.Panel4)
        Me.tabPRO.Controls.Add(Me.Panel2)
        Me.tabPRO.Location = New System.Drawing.Point(4, 25)
        Me.tabPRO.Name = "tabPRO"
        Me.tabPRO.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPRO.Size = New System.Drawing.Size(1440, 256)
        Me.tabPRO.TabIndex = 1
        Me.tabPRO.Text = "الانتاج"
        Me.tabPRO.UseVisualStyleBackColor = True
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.SplitContainer3)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(3, 3)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1049, 250)
        Me.Panel4.TabIndex = 117
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.dgvProduced)
        Me.SplitContainer3.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.dgvProductionCalculations)
        Me.SplitContainer3.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.SplitContainer3.Size = New System.Drawing.Size(1049, 250)
        Me.SplitContainer3.SplitterDistance = 522
        Me.SplitContainer3.TabIndex = 95
        '
        'dgvProduced
        '
        Me.dgvProduced.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvProduced.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProduced.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvProduced.Location = New System.Drawing.Point(0, 0)
        Me.dgvProduced.Name = "dgvProduced"
        Me.dgvProduced.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvProduced.RowHeadersWidth = 51
        Me.dgvProduced.RowTemplate.Height = 26
        Me.dgvProduced.Size = New System.Drawing.Size(522, 250)
        Me.dgvProduced.TabIndex = 1
        '
        'dgvProductionCalculations
        '
        Me.dgvProductionCalculations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvProductionCalculations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProductionCalculations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvProductionCalculations.Location = New System.Drawing.Point(0, 0)
        Me.dgvProductionCalculations.Name = "dgvProductionCalculations"
        Me.dgvProductionCalculations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvProductionCalculations.RowHeadersWidth = 51
        Me.dgvProductionCalculations.RowTemplate.Height = 26
        Me.dgvProductionCalculations.Size = New System.Drawing.Size(523, 250)
        Me.dgvProductionCalculations.TabIndex = 94
        Me.dgvProductionCalculations.TabStop = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.txtProductionCode)
        Me.Panel2.Controls.Add(Me.txtProductID)
        Me.Panel2.Controls.Add(Me.txtProductionAmount)
        Me.Panel2.Controls.Add(Me.Label20)
        Me.Panel2.Controls.Add(Me.txtProductionUnit)
        Me.Panel2.Controls.Add(Me.txtTotalChemicalQty)
        Me.Panel2.Controls.Add(Me.Label25)
        Me.Panel2.Controls.Add(Me.txtTotalProductionVolume)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.Label16)
        Me.Panel2.Controls.Add(Me.txtTotalProductionCost)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtTotalProductionQTY)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.txtProductUnitCost)
        Me.Panel2.Controls.Add(Me.txtPastProductAverageCost)
        Me.Panel2.Controls.Add(Me.Label19)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(1052, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(385, 250)
        Me.Panel2.TabIndex = 116
        '
        'txtProductionCode
        '
        Me.txtProductionCode.Location = New System.Drawing.Point(9, 5)
        Me.txtProductionCode.Name = "txtProductionCode"
        Me.txtProductionCode.Size = New System.Drawing.Size(170, 24)
        Me.txtProductionCode.TabIndex = 116
        '
        'txtProductID
        '
        Me.txtProductID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProductID.Enabled = False
        Me.txtProductID.Location = New System.Drawing.Point(9, 32)
        Me.txtProductID.Name = "txtProductID"
        Me.txtProductID.Size = New System.Drawing.Size(170, 24)
        Me.txtProductID.TabIndex = 115
        '
        'txtProductionAmount
        '
        Me.txtProductionAmount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProductionAmount.Location = New System.Drawing.Point(93, 59)
        Me.txtProductionAmount.Name = "txtProductionAmount"
        Me.txtProductionAmount.Size = New System.Drawing.Size(86, 24)
        Me.txtProductionAmount.TabIndex = 97
        '
        'Label20
        '
        Me.Label20.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(257, 225)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(109, 17)
        Me.Label20.TabIndex = 110
        Me.Label20.Text = "متوسط س.سابق"
        '
        'txtProductionUnit
        '
        Me.txtProductionUnit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProductionUnit.Enabled = False
        Me.txtProductionUnit.Location = New System.Drawing.Point(9, 59)
        Me.txtProductionUnit.Name = "txtProductionUnit"
        Me.txtProductionUnit.Size = New System.Drawing.Size(78, 24)
        Me.txtProductionUnit.TabIndex = 102
        '
        'txtTotalChemicalQty
        '
        Me.txtTotalChemicalQty.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTotalChemicalQty.Enabled = False
        Me.txtTotalChemicalQty.Location = New System.Drawing.Point(9, 113)
        Me.txtTotalChemicalQty.Name = "txtTotalChemicalQty"
        Me.txtTotalChemicalQty.Size = New System.Drawing.Size(170, 24)
        Me.txtTotalChemicalQty.TabIndex = 108
        '
        'Label25
        '
        Me.Label25.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(284, 63)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(82, 17)
        Me.Label25.TabIndex = 98
        Me.Label25.Text = "كمية التصنيع"
        '
        'txtTotalProductionVolume
        '
        Me.txtTotalProductionVolume.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTotalProductionVolume.Enabled = False
        Me.txtTotalProductionVolume.Location = New System.Drawing.Point(9, 86)
        Me.txtTotalProductionVolume.Name = "txtTotalProductionVolume"
        Me.txtTotalProductionVolume.Size = New System.Drawing.Size(170, 24)
        Me.txtTotalProductionVolume.TabIndex = 107
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(300, 9)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(66, 17)
        Me.Label11.TabIndex = 99
        Me.Label11.Text = "كود الانتاج"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(254, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(112, 17)
        Me.Label1.TabIndex = 99
        Me.Label1.Text = "كود الصنف المصنع"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(265, 90)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(101, 17)
        Me.Label4.TabIndex = 109
        Me.Label4.Text = "الحجم الاجمالي"
        '
        'Label16
        '
        Me.Label16.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(279, 117)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(87, 17)
        Me.Label16.TabIndex = 114
        Me.Label16.Text = "اجمالي المواد"
        '
        'txtTotalProductionCost
        '
        Me.txtTotalProductionCost.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTotalProductionCost.Enabled = False
        Me.txtTotalProductionCost.Location = New System.Drawing.Point(9, 140)
        Me.txtTotalProductionCost.Name = "txtTotalProductionCost"
        Me.txtTotalProductionCost.Size = New System.Drawing.Size(170, 24)
        Me.txtTotalProductionCost.TabIndex = 106
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(272, 144)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(94, 17)
        Me.Label9.TabIndex = 113
        Me.Label9.Text = "اجمالي التكلفة"
        '
        'txtTotalProductionQTY
        '
        Me.txtTotalProductionQTY.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTotalProductionQTY.Enabled = False
        Me.txtTotalProductionQTY.Location = New System.Drawing.Point(9, 167)
        Me.txtTotalProductionQTY.Name = "txtTotalProductionQTY"
        Me.txtTotalProductionQTY.Size = New System.Drawing.Size(170, 24)
        Me.txtTotalProductionQTY.TabIndex = 105
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(275, 171)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(91, 17)
        Me.Label6.TabIndex = 112
        Me.Label6.Text = "اجمالي المنتج"
        '
        'txtProductUnitCost
        '
        Me.txtProductUnitCost.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProductUnitCost.Enabled = False
        Me.txtProductUnitCost.Location = New System.Drawing.Point(9, 194)
        Me.txtProductUnitCost.Name = "txtProductUnitCost"
        Me.txtProductUnitCost.Size = New System.Drawing.Size(170, 24)
        Me.txtProductUnitCost.TabIndex = 104
        '
        'txtPastProductAverageCost
        '
        Me.txtPastProductAverageCost.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPastProductAverageCost.Enabled = False
        Me.txtPastProductAverageCost.Location = New System.Drawing.Point(9, 221)
        Me.txtPastProductAverageCost.Name = "txtPastProductAverageCost"
        Me.txtPastProductAverageCost.Size = New System.Drawing.Size(170, 24)
        Me.txtPastProductAverageCost.TabIndex = 103
        '
        'Label19
        '
        Me.Label19.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(290, 198)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(76, 17)
        Me.Label19.TabIndex = 111
        Me.Label19.Text = "سعر الوحدة"
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
        'tabCUT
        '
        Me.tabCUT.Controls.Add(Me.Panel6)
        Me.tabCUT.Controls.Add(Me.Panel5)
        Me.tabCUT.Location = New System.Drawing.Point(4, 25)
        Me.tabCUT.Name = "tabCUT"
        Me.tabCUT.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCUT.Size = New System.Drawing.Size(1440, 256)
        Me.tabCUT.TabIndex = 2
        Me.tabCUT.Text = "القص"
        Me.tabCUT.UseVisualStyleBackColor = True
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.dgvOutPut)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel6.Location = New System.Drawing.Point(3, 3)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(1085, 250)
        Me.Panel6.TabIndex = 1
        '
        'dgvOutPut
        '
        Me.dgvOutPut.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvOutPut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvOutPut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOutPut.Location = New System.Drawing.Point(0, 0)
        Me.dgvOutPut.Name = "dgvOutPut"
        Me.dgvOutPut.RowHeadersWidth = 51
        Me.dgvOutPut.RowTemplate.Height = 26
        Me.dgvOutPut.Size = New System.Drawing.Size(1085, 250)
        Me.dgvOutPut.TabIndex = 3
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.txtSourceStore)
        Me.Panel5.Controls.Add(Me.txtStoreID)
        Me.Panel5.Controls.Add(Me.txtAvailableQTY)
        Me.Panel5.Controls.Add(Me.Label26)
        Me.Panel5.Controls.Add(Me.dtpDate)
        Me.Panel5.Controls.Add(Me.dtpCuttingDate)
        Me.Panel5.Controls.Add(Me.Label27)
        Me.Panel5.Controls.Add(Me.Label24)
        Me.Panel5.Controls.Add(Me.Label23)
        Me.Panel5.Controls.Add(Me.txtProductCode)
        Me.Panel5.Controls.Add(Me.txtTotalVolumeOutPut)
        Me.Panel5.Controls.Add(Me.txtCuttingCode)
        Me.Panel5.Controls.Add(Me.TotalPcsOutPut)
        Me.Panel5.Controls.Add(Me.Label12)
        Me.Panel5.Controls.Add(Me.Label22)
        Me.Panel5.Controls.Add(Me.Label21)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel5.Location = New System.Drawing.Point(1088, 3)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(349, 250)
        Me.Panel5.TabIndex = 0
        '
        'txtSourceStore
        '
        Me.txtSourceStore.Enabled = False
        Me.txtSourceStore.Location = New System.Drawing.Point(5, 95)
        Me.txtSourceStore.Name = "txtSourceStore"
        Me.txtSourceStore.Size = New System.Drawing.Size(155, 24)
        Me.txtSourceStore.TabIndex = 21
        '
        'txtStoreID
        '
        Me.txtStoreID.AutoSize = True
        Me.txtStoreID.Location = New System.Drawing.Point(261, 98)
        Me.txtStoreID.Name = "txtStoreID"
        Me.txtStoreID.Size = New System.Drawing.Size(74, 17)
        Me.txtStoreID.TabIndex = 19
        Me.txtStoreID.Text = "مخزن القص"
        '
        'txtAvailableQTY
        '
        Me.txtAvailableQTY.Enabled = False
        Me.txtAvailableQTY.Location = New System.Drawing.Point(5, 211)
        Me.txtAvailableQTY.Name = "txtAvailableQTY"
        Me.txtAvailableQTY.Size = New System.Drawing.Size(155, 24)
        Me.txtAvailableQTY.TabIndex = 18
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(241, 210)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(94, 17)
        Me.Label26.TabIndex = 17
        Me.Label26.Text = "الكمية المتوفرة"
        '
        'dtpDate
        '
        Me.dtpDate.Enabled = False
        Me.dtpDate.Location = New System.Drawing.Point(5, 8)
        Me.dtpDate.Name = "dtpDate"
        Me.dtpDate.Size = New System.Drawing.Size(155, 24)
        Me.dtpDate.TabIndex = 16
        '
        'dtpCuttingDate
        '
        Me.dtpCuttingDate.Enabled = False
        Me.dtpCuttingDate.Location = New System.Drawing.Point(5, 37)
        Me.dtpCuttingDate.Name = "dtpCuttingDate"
        Me.dtpCuttingDate.Size = New System.Drawing.Size(155, 24)
        Me.dtpCuttingDate.TabIndex = 16
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(227, 42)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(108, 17)
        Me.Label27.TabIndex = 15
        Me.Label27.Text = "تاريخ عملية القص"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(292, 14)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(43, 17)
        Me.Label24.TabIndex = 15
        Me.Label24.Text = "التاريخ"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(209, 182)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(126, 17)
        Me.Label23.TabIndex = 9
        Me.Label23.Text = "اجمالي الحجم الناتج"
        '
        'txtProductCode
        '
        Me.txtProductCode.Enabled = False
        Me.txtProductCode.Location = New System.Drawing.Point(5, 124)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(155, 24)
        Me.txtProductCode.TabIndex = 14
        '
        'txtTotalVolumeOutPut
        '
        Me.txtTotalVolumeOutPut.Enabled = False
        Me.txtTotalVolumeOutPut.Location = New System.Drawing.Point(5, 182)
        Me.txtTotalVolumeOutPut.Name = "txtTotalVolumeOutPut"
        Me.txtTotalVolumeOutPut.Size = New System.Drawing.Size(155, 24)
        Me.txtTotalVolumeOutPut.TabIndex = 12
        '
        'txtCuttingCode
        '
        Me.txtCuttingCode.Enabled = False
        Me.txtCuttingCode.Location = New System.Drawing.Point(5, 66)
        Me.txtCuttingCode.Name = "txtCuttingCode"
        Me.txtCuttingCode.Size = New System.Drawing.Size(155, 24)
        Me.txtCuttingCode.TabIndex = 14
        '
        'TotalPcsOutPut
        '
        Me.TotalPcsOutPut.Enabled = False
        Me.TotalPcsOutPut.Location = New System.Drawing.Point(5, 153)
        Me.TotalPcsOutPut.Name = "TotalPcsOutPut"
        Me.TotalPcsOutPut.Size = New System.Drawing.Size(155, 24)
        Me.TotalPcsOutPut.TabIndex = 11
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(234, 70)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(101, 17)
        Me.Label12.TabIndex = 6
        Me.Label12.Text = "كود عملية القص"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(179, 154)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(156, 17)
        Me.Label22.TabIndex = 8
        Me.Label22.Text = "اجمالي عدد القطع الناتجة"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(267, 126)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(68, 17)
        Me.Label21.TabIndex = 7
        Me.Label21.Text = "كود الصنف"
        '
        'tabSRT
        '
        Me.tabSRT.Location = New System.Drawing.Point(4, 25)
        Me.tabSRT.Name = "tabSRT"
        Me.tabSRT.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSRT.Size = New System.Drawing.Size(1440, 256)
        Me.tabSRT.TabIndex = 7
        Me.tabSRT.Text = "المرتجعات"
        Me.tabSRT.UseVisualStyleBackColor = True
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
        'pnlOperations
        '
        Me.pnlOperations.Controls.Add(Me.tabMain)
        Me.pnlOperations.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlOperations.Location = New System.Drawing.Point(0, 0)
        Me.pnlOperations.Name = "pnlOperations"
        Me.pnlOperations.Size = New System.Drawing.Size(1448, 285)
        Me.pnlOperations.TabIndex = 8
        '
        'dgvAffectedOperations
        '
        Me.dgvAffectedOperations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAffectedOperations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvAffectedOperations.Location = New System.Drawing.Point(0, 0)
        Me.dgvAffectedOperations.Name = "dgvAffectedOperations"
        Me.dgvAffectedOperations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvAffectedOperations.RowHeadersWidth = 51
        Me.dgvAffectedOperations.RowTemplate.Height = 26
        Me.dgvAffectedOperations.Size = New System.Drawing.Size(1448, 239)
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
        Me.SplitContainer1.SplitterDistance = 234
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
        Me.dgvAdjustResult.Size = New System.Drawing.Size(1448, 234)
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
        Me.SplitContainer2.Size = New System.Drawing.Size(1448, 532)
        Me.SplitContainer2.SplitterDistance = 239
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
        Me.dgvSimulation.Size = New System.Drawing.Size(1448, 289)
        Me.dgvSimulation.TabIndex = 0
        '
        'frmCostCorrection
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 1055)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlOperations)
        Me.Controls.Add(Me.pnlControls)
        Me.Name = "frmCostCorrection"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmRevaluation"
        Me.tabMain.ResumeLayout(False)
        Me.tabPRO.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.dgvProduced, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvProductionCalculations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.tabPUR.ResumeLayout(False)
        Me.tabPUR.PerformLayout()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.tabCUT.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        CType(Me.dgvOutPut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.pnlControls.ResumeLayout(False)
        Me.pnlControls.PerformLayout()
        Me.pnlMode.ResumeLayout(False)
        Me.pnlMode.PerformLayout()
        Me.pnlOperations.ResumeLayout(False)
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
    Friend WithEvents dgvProductionCalculations As DataGridView
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents dgvProduced As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label25 As Label
    Friend WithEvents txtProductionUnit As TextBox
    Friend WithEvents txtProductionAmount As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents txtTotalChemicalQty As TextBox
    Friend WithEvents txtTotalProductionVolume As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtTotalProductionCost As TextBox
    Friend WithEvents txtTotalProductionQTY As TextBox
    Friend WithEvents txtProductUnitCost As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents txtPastProductAverageCost As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents txtProductID As TextBox
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Panel5 As Panel
    Friend WithEvents txtProductionCode As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Panel6 As Panel
    Friend WithEvents Label23 As Label
    Friend WithEvents txtProductCode As TextBox
    Friend WithEvents txtTotalVolumeOutPut As TextBox
    Friend WithEvents txtCuttingCode As TextBox
    Friend WithEvents TotalPcsOutPut As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents dgvOutPut As DataGridView
    Friend WithEvents dtpCuttingDate As DateTimePicker
    Friend WithEvents Label24 As Label
    Friend WithEvents txtAvailableQTY As TextBox
    Friend WithEvents Label26 As Label
    Friend WithEvents txtSourceStore As TextBox
    Friend WithEvents txtStoreID As Label
    Friend WithEvents dtpDate As DateTimePicker
    Friend WithEvents Label27 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents tabSRT As TabPage
    Friend WithEvents pnlOperations As Panel
End Class
