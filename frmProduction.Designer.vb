<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmProduction
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
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.cboTargetStore = New System.Windows.Forms.ComboBox()
        Me.cboSourceStore = New System.Windows.Forms.ComboBox()
        Me.txtProductionAmount = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.cboBOMVersion = New System.Windows.Forms.ComboBox()
        Me.txtBOMID = New System.Windows.Forms.TextBox()
        Me.txtCleaningChemicalQTY = New System.Windows.Forms.TextBox()
        Me.btnSearchBOM = New System.Windows.Forms.Button()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dgvProductionCalculations = New System.Windows.Forms.DataGridView()
        Me.colCalProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalProductUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalBOMQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalAvailableStock = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalActualQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colCalCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtDeviation = New System.Windows.Forms.TextBox()
        Me.txtPastProductAverageCost = New System.Windows.Forms.TextBox()
        Me.txtProductUnitCost = New System.Windows.Forms.TextBox()
        Me.txtTotalProductionQTY = New System.Windows.Forms.TextBox()
        Me.txtTotalProductionCost = New System.Windows.Forms.TextBox()
        Me.txtTotalProductionValue = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cboCleaningChemical = New System.Windows.Forms.ComboBox()
        Me.dtpProductionDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpCreatedDate = New System.Windows.Forms.DateTimePicker()
        Me.cboProductID = New System.Windows.Forms.ComboBox()
        Me.btnProductSearch = New System.Windows.Forms.Button()
        Me.pnlBtns = New System.Windows.Forms.Panel()
        Me.btnCancelProduction = New System.Windows.Forms.Button()
        Me.btnExecuteProduction = New System.Windows.Forms.Button()
        Me.cboProductionStatus = New System.Windows.Forms.ComboBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.lblBOMStatus = New System.Windows.Forms.Label()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.txtProductGroupID = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.txtProductionCode = New System.Windows.Forms.TextBox()
        Me.txtProductName = New System.Windows.Forms.TextBox()
        Me.txtBaseUnitID = New System.Windows.Forms.TextBox()
        Me.txtCategoryID = New System.Windows.Forms.TextBox()
        Me.txtCustomerID = New System.Windows.Forms.TextBox()
        Me.txtTotalChemicalAmount = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.pnlTotals = New System.Windows.Forms.Panel()
        Me.sctDetails = New System.Windows.Forms.SplitContainer()
        Me.dgvProduced = New System.Windows.Forms.DataGridView()
        Me.colManOutputID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManlength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManWidth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManHeight = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManProductVolume = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colManTotalProductVolume = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ChkIsCleaningUsed = New System.Windows.Forms.CheckBox()
        Me.txtProductionUnit = New System.Windows.Forms.TextBox()
        Me.txtCleaningChemicalUnit = New System.Windows.Forms.TextBox()
        Me.txtSubCategory = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ChkBOMIsActive = New System.Windows.Forms.CheckBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        CType(Me.dgvProductionCalculations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBtns.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.pnlTotals.SuspendLayout()
        CType(Me.sctDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.sctDetails.Panel1.SuspendLayout()
        Me.sctDetails.Panel2.SuspendLayout()
        Me.sctDetails.SuspendLayout()
        CType(Me.dgvProduced, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(951, 12)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(101, 17)
        Me.Label14.TabIndex = 84
        Me.Label14.Text = "الحجم الاجمالي"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(272, 12)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(55, 17)
        Me.Label21.TabIndex = 85
        Me.Label21.Text = "الانحراف"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(356, 12)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(109, 17)
        Me.Label20.TabIndex = 86
        Me.Label20.Text = "متوسط س.سابق"
        '
        'cboTargetStore
        '
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(15, 146)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(147, 24)
        Me.cboTargetStore.TabIndex = 87
        '
        'cboSourceStore
        '
        Me.cboSourceStore.FormattingEnabled = True
        Me.cboSourceStore.Location = New System.Drawing.Point(15, 117)
        Me.cboSourceStore.Name = "cboSourceStore"
        Me.cboSourceStore.Size = New System.Drawing.Size(147, 24)
        Me.cboSourceStore.TabIndex = 87
        '
        'txtProductionAmount
        '
        Me.txtProductionAmount.Location = New System.Drawing.Point(85, 454)
        Me.txtProductionAmount.Name = "txtProductionAmount"
        Me.txtProductionAmount.Size = New System.Drawing.Size(77, 24)
        Me.txtProductionAmount.TabIndex = 2
        '
        'Label22
        '
        Me.Label22.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(112, 430)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(47, 17)
        Me.Label22.TabIndex = 82
        Me.Label22.Text = "الاصدار"
        '
        'Label23
        '
        Me.Label23.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(45, 383)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(114, 17)
        Me.Label23.TabIndex = 83
        Me.Label23.Text = "كود معادلة التصنيع"
        '
        'cboBOMVersion
        '
        Me.cboBOMVersion.FormattingEnabled = True
        Me.cboBOMVersion.Location = New System.Drawing.Point(15, 424)
        Me.cboBOMVersion.Name = "cboBOMVersion"
        Me.cboBOMVersion.Size = New System.Drawing.Size(83, 24)
        Me.cboBOMVersion.TabIndex = 81
        '
        'txtBOMID
        '
        Me.txtBOMID.Enabled = False
        Me.txtBOMID.Location = New System.Drawing.Point(15, 379)
        Me.txtBOMID.Name = "txtBOMID"
        Me.txtBOMID.Size = New System.Drawing.Size(147, 24)
        Me.txtBOMID.TabIndex = 80
        '
        'txtCleaningChemicalQTY
        '
        Me.txtCleaningChemicalQTY.Location = New System.Drawing.Point(85, 520)
        Me.txtCleaningChemicalQTY.Name = "txtCleaningChemicalQTY"
        Me.txtCleaningChemicalQTY.Size = New System.Drawing.Size(77, 24)
        Me.txtCleaningChemicalQTY.TabIndex = 79
        '
        'btnSearchBOM
        '
        Me.btnSearchBOM.Location = New System.Drawing.Point(104, 424)
        Me.btnSearchBOM.Name = "btnSearchBOM"
        Me.btnSearchBOM.Size = New System.Drawing.Size(58, 24)
        Me.btnSearchBOM.TabIndex = 51
        Me.btnSearchBOM.TabStop = False
        Me.btnSearchBOM.UseVisualStyleBackColor = True
        '
        'Label25
        '
        Me.Label25.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(77, 460)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(82, 17)
        Me.Label25.TabIndex = 65
        Me.Label25.Text = "كمية التصنيع"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(77, 491)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(82, 17)
        Me.Label4.TabIndex = 65
        Me.Label4.Text = "مادة التنظيف"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(86, 68)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(73, 17)
        Me.Label5.TabIndex = 65
        Me.Label5.Text = "تاريخ الانتاج"
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(80, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 17)
        Me.Label3.TabIndex = 66
        Me.Label3.Text = "تاريخ الانشاء"
        '
        'dgvProductionCalculations
        '
        Me.dgvProductionCalculations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProductionCalculations.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colCalProductCode, Me.colCalProductID, Me.colCalProductName, Me.colCalProductUnit, Me.colCalBOMQTY, Me.colCalAvailableStock, Me.colCalActualQTY, Me.colCalCost})
        Me.dgvProductionCalculations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvProductionCalculations.Location = New System.Drawing.Point(0, 0)
        Me.dgvProductionCalculations.Name = "dgvProductionCalculations"
        Me.dgvProductionCalculations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvProductionCalculations.RowHeadersWidth = 51
        Me.dgvProductionCalculations.RowTemplate.Height = 26
        Me.dgvProductionCalculations.Size = New System.Drawing.Size(1077, 404)
        Me.dgvProductionCalculations.TabIndex = 93
        Me.dgvProductionCalculations.TabStop = False
        '
        'colCalProductCode
        '
        Me.colCalProductCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.colCalProductCode.HeaderText = "كود الصنف"
        Me.colCalProductCode.MinimumWidth = 6
        Me.colCalProductCode.Name = "colCalProductCode"
        Me.colCalProductCode.ReadOnly = True
        '
        'colCalProductID
        '
        Me.colCalProductID.HeaderText = "رقم الصنف"
        Me.colCalProductID.MinimumWidth = 6
        Me.colCalProductID.Name = "colCalProductID"
        Me.colCalProductID.ReadOnly = True
        Me.colCalProductID.Visible = False
        Me.colCalProductID.Width = 125
        '
        'colCalProductName
        '
        Me.colCalProductName.HeaderText = "اسم الصنف"
        Me.colCalProductName.MinimumWidth = 6
        Me.colCalProductName.Name = "colCalProductName"
        Me.colCalProductName.ReadOnly = True
        Me.colCalProductName.Width = 125
        '
        'colCalProductUnit
        '
        Me.colCalProductUnit.HeaderText = "الوحدة"
        Me.colCalProductUnit.MinimumWidth = 6
        Me.colCalProductUnit.Name = "colCalProductUnit"
        Me.colCalProductUnit.ReadOnly = True
        Me.colCalProductUnit.Width = 125
        '
        'colCalBOMQTY
        '
        Me.colCalBOMQTY.HeaderText = "الكمية"
        Me.colCalBOMQTY.MinimumWidth = 6
        Me.colCalBOMQTY.Name = "colCalBOMQTY"
        Me.colCalBOMQTY.ReadOnly = True
        Me.colCalBOMQTY.Width = 125
        '
        'colCalAvailableStock
        '
        Me.colCalAvailableStock.HeaderText = "المخزون"
        Me.colCalAvailableStock.MinimumWidth = 6
        Me.colCalAvailableStock.Name = "colCalAvailableStock"
        Me.colCalAvailableStock.ReadOnly = True
        Me.colCalAvailableStock.Width = 125
        '
        'colCalActualQTY
        '
        Me.colCalActualQTY.HeaderText = "الكمية الفعلية"
        Me.colCalActualQTY.MinimumWidth = 6
        Me.colCalActualQTY.Name = "colCalActualQTY"
        Me.colCalActualQTY.Width = 125
        '
        'colCalCost
        '
        Me.colCalCost.HeaderText = "التكلفة"
        Me.colCalCost.MinimumWidth = 6
        Me.colCalCost.Name = "colCalCost"
        Me.colCalCost.ReadOnly = True
        Me.colCalCost.Width = 125
        '
        'Label13
        '
        Me.Label13.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(100, 553)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(59, 17)
        Me.Label13.TabIndex = 68
        Me.Label13.Text = "ملاحظات"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(507, 12)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(76, 17)
        Me.Label19.TabIndex = 87
        Me.Label19.Text = "سعر الوحدة"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(610, 12)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(91, 17)
        Me.Label18.TabIndex = 88
        Me.Label18.Text = "اجمالي المنتج"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(725, 12)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(94, 17)
        Me.Label17.TabIndex = 89
        Me.Label17.Text = "اجمالي التكلفة"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(850, 12)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(87, 17)
        Me.Label16.TabIndex = 90
        Me.Label16.Text = "اجمالي المواد"
        '
        'txtDeviation
        '
        Me.txtDeviation.Location = New System.Drawing.Point(235, 35)
        Me.txtDeviation.Name = "txtDeviation"
        Me.txtDeviation.Size = New System.Drawing.Size(112, 24)
        Me.txtDeviation.TabIndex = 77
        '
        'txtPastProductAverageCost
        '
        Me.txtPastProductAverageCost.Location = New System.Drawing.Point(353, 35)
        Me.txtPastProductAverageCost.Name = "txtPastProductAverageCost"
        Me.txtPastProductAverageCost.Size = New System.Drawing.Size(112, 24)
        Me.txtPastProductAverageCost.TabIndex = 78
        '
        'txtProductUnitCost
        '
        Me.txtProductUnitCost.Location = New System.Drawing.Point(471, 35)
        Me.txtProductUnitCost.Name = "txtProductUnitCost"
        Me.txtProductUnitCost.Size = New System.Drawing.Size(112, 24)
        Me.txtProductUnitCost.TabIndex = 79
        '
        'txtTotalProductionQTY
        '
        Me.txtTotalProductionQTY.Location = New System.Drawing.Point(589, 35)
        Me.txtTotalProductionQTY.Name = "txtTotalProductionQTY"
        Me.txtTotalProductionQTY.Size = New System.Drawing.Size(112, 24)
        Me.txtTotalProductionQTY.TabIndex = 80
        '
        'txtTotalProductionCost
        '
        Me.txtTotalProductionCost.Location = New System.Drawing.Point(707, 35)
        Me.txtTotalProductionCost.Name = "txtTotalProductionCost"
        Me.txtTotalProductionCost.Size = New System.Drawing.Size(112, 24)
        Me.txtTotalProductionCost.TabIndex = 81
        '
        'txtTotalProductionValue
        '
        Me.txtTotalProductionValue.Location = New System.Drawing.Point(943, 35)
        Me.txtTotalProductionValue.Name = "txtTotalProductionValue"
        Me.txtTotalProductionValue.Size = New System.Drawing.Size(112, 24)
        Me.txtTotalProductionValue.TabIndex = 82
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(50, 119)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(109, 17)
        Me.Label11.TabIndex = 69
        Me.Label11.Text = "مخزن المواد الخام"
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(48, 288)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(111, 17)
        Me.Label8.TabIndex = 72
        Me.Label8.Text = "فئة الصنف المصنع"
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(35, 147)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(124, 17)
        Me.Label9.TabIndex = 73
        Me.Label9.Tag = ""
        Me.Label9.Text = "مخزن المنتج المصنع"
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(37, 258)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(122, 17)
        Me.Label7.TabIndex = 74
        Me.Label7.Text = "وحدة الصنف المصنع"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(93, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 17)
        Me.Label1.TabIndex = 75
        Me.Label1.Text = "كود الانتاج"
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(40, 228)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(119, 17)
        Me.Label6.TabIndex = 76
        Me.Label6.Text = "اسم الصنف المصنع"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(47, 198)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(112, 17)
        Me.Label2.TabIndex = 77
        Me.Label2.Text = "كود الصنف المصنع"
        '
        'cboCleaningChemical
        '
        Me.cboCleaningChemical.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboCleaningChemical.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboCleaningChemical.FormattingEnabled = True
        Me.cboCleaningChemical.Location = New System.Drawing.Point(15, 484)
        Me.cboCleaningChemical.Name = "cboCleaningChemical"
        Me.cboCleaningChemical.Size = New System.Drawing.Size(123, 24)
        Me.cboCleaningChemical.TabIndex = 3
        '
        'dtpProductionDate
        '
        Me.dtpProductionDate.Location = New System.Drawing.Point(15, 68)
        Me.dtpProductionDate.Name = "dtpProductionDate"
        Me.dtpProductionDate.Size = New System.Drawing.Size(147, 24)
        Me.dtpProductionDate.TabIndex = 0
        '
        'dtpCreatedDate
        '
        Me.dtpCreatedDate.Enabled = False
        Me.dtpCreatedDate.Location = New System.Drawing.Point(15, 8)
        Me.dtpCreatedDate.Name = "dtpCreatedDate"
        Me.dtpCreatedDate.Size = New System.Drawing.Size(147, 24)
        Me.dtpCreatedDate.TabIndex = 63
        '
        'cboProductID
        '
        Me.cboProductID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProductID.FormattingEnabled = True
        Me.cboProductID.Location = New System.Drawing.Point(15, 197)
        Me.cboProductID.Name = "cboProductID"
        Me.cboProductID.Size = New System.Drawing.Size(97, 24)
        Me.cboProductID.TabIndex = 1
        '
        'btnProductSearch
        '
        Me.btnProductSearch.Location = New System.Drawing.Point(118, 197)
        Me.btnProductSearch.Name = "btnProductSearch"
        Me.btnProductSearch.Size = New System.Drawing.Size(44, 24)
        Me.btnProductSearch.TabIndex = 61
        Me.btnProductSearch.Text = "Find"
        Me.btnProductSearch.UseVisualStyleBackColor = True
        '
        'pnlBtns
        '
        Me.pnlBtns.Controls.Add(Me.btnCancelProduction)
        Me.pnlBtns.Controls.Add(Me.btnExecuteProduction)
        Me.pnlBtns.Controls.Add(Me.cboProductionStatus)
        Me.pnlBtns.Controls.Add(Me.btnSearch)
        Me.pnlBtns.Controls.Add(Me.lblBOMStatus)
        Me.pnlBtns.Controls.Add(Me.chkIsActive)
        Me.pnlBtns.Controls.Add(Me.btnNew)
        Me.pnlBtns.Controls.Add(Me.btnSave)
        Me.pnlBtns.Controls.Add(Me.btnCancel)
        Me.pnlBtns.Controls.Add(Me.btnClose)
        Me.pnlBtns.Controls.Add(Me.btnPrint)
        Me.pnlBtns.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlBtns.Location = New System.Drawing.Point(1425, 0)
        Me.pnlBtns.Name = "pnlBtns"
        Me.pnlBtns.Size = New System.Drawing.Size(107, 808)
        Me.pnlBtns.TabIndex = 65
        '
        'btnCancelProduction
        '
        Me.btnCancelProduction.Location = New System.Drawing.Point(4, 761)
        Me.btnCancelProduction.Name = "btnCancelProduction"
        Me.btnCancelProduction.Size = New System.Drawing.Size(100, 41)
        Me.btnCancelProduction.TabIndex = 89
        Me.btnCancelProduction.Text = "الغاء الانتاج"
        Me.btnCancelProduction.UseVisualStyleBackColor = True
        '
        'btnExecuteProduction
        '
        Me.btnExecuteProduction.Location = New System.Drawing.Point(3, 621)
        Me.btnExecuteProduction.Name = "btnExecuteProduction"
        Me.btnExecuteProduction.Size = New System.Drawing.Size(100, 41)
        Me.btnExecuteProduction.TabIndex = 64
        Me.btnExecuteProduction.Text = "تنفيذ الانتاج"
        Me.btnExecuteProduction.UseVisualStyleBackColor = True
        '
        'cboProductionStatus
        '
        Me.cboProductionStatus.Enabled = False
        Me.cboProductionStatus.FormattingEnabled = True
        Me.cboProductionStatus.Location = New System.Drawing.Point(3, 6)
        Me.cboProductionStatus.Name = "cboProductionStatus"
        Me.cboProductionStatus.Size = New System.Drawing.Size(101, 24)
        Me.cboProductionStatus.TabIndex = 88
        '
        'btnSearch
        '
        Me.btnSearch.Location = New System.Drawing.Point(4, 493)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(100, 37)
        Me.btnSearch.TabIndex = 63
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'lblBOMStatus
        '
        Me.lblBOMStatus.AutoSize = True
        Me.lblBOMStatus.Location = New System.Drawing.Point(202, 15)
        Me.lblBOMStatus.Name = "lblBOMStatus"
        Me.lblBOMStatus.Size = New System.Drawing.Size(45, 17)
        Me.lblBOMStatus.TabIndex = 62
        Me.lblBOMStatus.Text = "نشطة"
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Enabled = False
        Me.chkIsActive.Location = New System.Drawing.Point(178, 14)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsActive.TabIndex = 55
        Me.chkIsActive.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Location = New System.Drawing.Point(4, 349)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(100, 37)
        Me.btnNew.TabIndex = 49
        Me.btnNew.TabStop = False
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(4, 394)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 37)
        Me.btnSave.TabIndex = 47
        Me.btnSave.Text = "حفظ"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(4, 437)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 37)
        Me.btnCancel.TabIndex = 50
        Me.btnCancel.TabStop = False
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(3, 718)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 37)
        Me.btnClose.TabIndex = 48
        Me.btnClose.Text = "اغلاق"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar
        Me.btnPrint.Location = New System.Drawing.Point(3, 671)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 41)
        Me.btnPrint.TabIndex = 52
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'txtProductGroupID
        '
        Me.txtProductGroupID.Enabled = False
        Me.txtProductGroupID.Location = New System.Drawing.Point(15, 317)
        Me.txtProductGroupID.Name = "txtProductGroupID"
        Me.txtProductGroupID.Size = New System.Drawing.Size(147, 24)
        Me.txtProductGroupID.TabIndex = 53
        Me.txtProductGroupID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label24
        '
        Me.Label24.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(63, 320)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(96, 17)
        Me.Label24.TabIndex = 71
        Me.Label24.Text = "مجموعة الصنف"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(15, 550)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(147, 205)
        Me.txtNotes.TabIndex = 4
        Me.txtNotes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtProductionCode
        '
        Me.txtProductionCode.Enabled = False
        Me.txtProductionCode.Location = New System.Drawing.Point(15, 38)
        Me.txtProductionCode.Name = "txtProductionCode"
        Me.txtProductionCode.Size = New System.Drawing.Size(147, 24)
        Me.txtProductionCode.TabIndex = 53
        '
        'txtProductName
        '
        Me.txtProductName.Enabled = False
        Me.txtProductName.Location = New System.Drawing.Point(15, 227)
        Me.txtProductName.Name = "txtProductName"
        Me.txtProductName.Size = New System.Drawing.Size(147, 24)
        Me.txtProductName.TabIndex = 53
        '
        'txtBaseUnitID
        '
        Me.txtBaseUnitID.Enabled = False
        Me.txtBaseUnitID.Location = New System.Drawing.Point(15, 257)
        Me.txtBaseUnitID.Name = "txtBaseUnitID"
        Me.txtBaseUnitID.Size = New System.Drawing.Size(147, 24)
        Me.txtBaseUnitID.TabIndex = 53
        Me.txtBaseUnitID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtCategoryID
        '
        Me.txtCategoryID.Enabled = False
        Me.txtCategoryID.Location = New System.Drawing.Point(15, 287)
        Me.txtCategoryID.Name = "txtCategoryID"
        Me.txtCategoryID.Size = New System.Drawing.Size(147, 24)
        Me.txtCategoryID.TabIndex = 53
        Me.txtCategoryID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtCustomerID
        '
        Me.txtCustomerID.Enabled = False
        Me.txtCustomerID.Location = New System.Drawing.Point(15, 781)
        Me.txtCustomerID.Name = "txtCustomerID"
        Me.txtCustomerID.Size = New System.Drawing.Size(147, 24)
        Me.txtCustomerID.TabIndex = 53
        Me.txtCustomerID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtTotalChemicalAmount
        '
        Me.txtTotalChemicalAmount.Location = New System.Drawing.Point(825, 35)
        Me.txtTotalChemicalAmount.Name = "txtTotalChemicalAmount"
        Me.txtTotalChemicalAmount.Size = New System.Drawing.Size(112, 24)
        Me.txtTotalChemicalAmount.TabIndex = 83
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.pnlTotals)
        Me.Panel3.Controls.Add(Me.sctDetails)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1077, 808)
        Me.Panel3.TabIndex = 67
        '
        'pnlTotals
        '
        Me.pnlTotals.Controls.Add(Me.Label20)
        Me.pnlTotals.Controls.Add(Me.txtTotalChemicalAmount)
        Me.pnlTotals.Controls.Add(Me.txtTotalProductionValue)
        Me.pnlTotals.Controls.Add(Me.Label14)
        Me.pnlTotals.Controls.Add(Me.txtTotalProductionCost)
        Me.pnlTotals.Controls.Add(Me.Label21)
        Me.pnlTotals.Controls.Add(Me.txtTotalProductionQTY)
        Me.pnlTotals.Controls.Add(Me.txtProductUnitCost)
        Me.pnlTotals.Controls.Add(Me.Label19)
        Me.pnlTotals.Controls.Add(Me.txtPastProductAverageCost)
        Me.pnlTotals.Controls.Add(Me.Label18)
        Me.pnlTotals.Controls.Add(Me.txtDeviation)
        Me.pnlTotals.Controls.Add(Me.Label17)
        Me.pnlTotals.Controls.Add(Me.Label16)
        Me.pnlTotals.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlTotals.Location = New System.Drawing.Point(0, 747)
        Me.pnlTotals.Name = "pnlTotals"
        Me.pnlTotals.Size = New System.Drawing.Size(1077, 61)
        Me.pnlTotals.TabIndex = 95
        '
        'sctDetails
        '
        Me.sctDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sctDetails.Location = New System.Drawing.Point(0, 0)
        Me.sctDetails.Name = "sctDetails"
        Me.sctDetails.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'sctDetails.Panel1
        '
        Me.sctDetails.Panel1.Controls.Add(Me.dgvProductionCalculations)
        '
        'sctDetails.Panel2
        '
        Me.sctDetails.Panel2.Controls.Add(Me.dgvProduced)
        Me.sctDetails.Size = New System.Drawing.Size(1077, 808)
        Me.sctDetails.SplitterDistance = 404
        Me.sctDetails.TabIndex = 96
        '
        'dgvProduced
        '
        Me.dgvProduced.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvProduced.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProduced.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colManOutputID, Me.colManlength, Me.colManWidth, Me.colManHeight, Me.colManQTY, Me.colManProductVolume, Me.colManTotalProductVolume})
        Me.dgvProduced.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvProduced.Location = New System.Drawing.Point(0, 0)
        Me.dgvProduced.Name = "dgvProduced"
        Me.dgvProduced.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvProduced.RowHeadersWidth = 51
        Me.dgvProduced.RowTemplate.Height = 26
        Me.dgvProduced.Size = New System.Drawing.Size(1077, 400)
        Me.dgvProduced.TabIndex = 0
        '
        'colManOutputID
        '
        Me.colManOutputID.HeaderText = "Column1"
        Me.colManOutputID.MinimumWidth = 6
        Me.colManOutputID.Name = "colManOutputID"
        Me.colManOutputID.Visible = False
        '
        'colManlength
        '
        Me.colManlength.HeaderText = "الطول"
        Me.colManlength.MinimumWidth = 6
        Me.colManlength.Name = "colManlength"
        '
        'colManWidth
        '
        Me.colManWidth.HeaderText = "العرض"
        Me.colManWidth.MinimumWidth = 6
        Me.colManWidth.Name = "colManWidth"
        '
        'colManHeight
        '
        Me.colManHeight.HeaderText = "الارتفاع"
        Me.colManHeight.MinimumWidth = 6
        Me.colManHeight.Name = "colManHeight"
        '
        'colManQTY
        '
        Me.colManQTY.HeaderText = "الكمية"
        Me.colManQTY.MinimumWidth = 6
        Me.colManQTY.Name = "colManQTY"
        '
        'colManProductVolume
        '
        Me.colManProductVolume.HeaderText = "حجم الوحدة"
        Me.colManProductVolume.MinimumWidth = 6
        Me.colManProductVolume.Name = "colManProductVolume"
        Me.colManProductVolume.ReadOnly = True
        '
        'colManTotalProductVolume
        '
        Me.colManTotalProductVolume.HeaderText = "اجمالي الحجم"
        Me.colManTotalProductVolume.MinimumWidth = 6
        Me.colManTotalProductVolume.Name = "colManTotalProductVolume"
        Me.colManTotalProductVolume.ReadOnly = True
        '
        'ChkIsCleaningUsed
        '
        Me.ChkIsCleaningUsed.AutoSize = True
        Me.ChkIsCleaningUsed.Location = New System.Drawing.Point(144, 488)
        Me.ChkIsCleaningUsed.Name = "ChkIsCleaningUsed"
        Me.ChkIsCleaningUsed.Size = New System.Drawing.Size(18, 17)
        Me.ChkIsCleaningUsed.TabIndex = 90
        Me.ChkIsCleaningUsed.UseVisualStyleBackColor = True
        '
        'txtProductionUnit
        '
        Me.txtProductionUnit.Location = New System.Drawing.Point(15, 454)
        Me.txtProductionUnit.Name = "txtProductionUnit"
        Me.txtProductionUnit.Size = New System.Drawing.Size(64, 24)
        Me.txtProductionUnit.TabIndex = 89
        '
        'txtCleaningChemicalUnit
        '
        Me.txtCleaningChemicalUnit.Enabled = False
        Me.txtCleaningChemicalUnit.Location = New System.Drawing.Point(15, 520)
        Me.txtCleaningChemicalUnit.Name = "txtCleaningChemicalUnit"
        Me.txtCleaningChemicalUnit.Size = New System.Drawing.Size(64, 24)
        Me.txtCleaningChemicalUnit.TabIndex = 86
        '
        'txtSubCategory
        '
        Me.txtSubCategory.Enabled = False
        Me.txtSubCategory.Location = New System.Drawing.Point(15, 349)
        Me.txtSubCategory.Name = "txtSubCategory"
        Me.txtSubCategory.Size = New System.Drawing.Size(147, 24)
        Me.txtSubCategory.TabIndex = 53
        Me.txtSubCategory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label26
        '
        Me.Label26.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(44, 527)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(115, 17)
        Me.Label26.TabIndex = 65
        Me.Label26.Text = "كمية مادة التنظيف"
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(49, 351)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(110, 17)
        Me.Label10.TabIndex = 71
        Me.Label10.Text = "المجموعة الفرعية"
        '
        'ChkBOMIsActive
        '
        Me.ChkBOMIsActive.AutoSize = True
        Me.ChkBOMIsActive.Location = New System.Drawing.Point(16, 384)
        Me.ChkBOMIsActive.Name = "ChkBOMIsActive"
        Me.ChkBOMIsActive.Size = New System.Drawing.Size(18, 17)
        Me.ChkBOMIsActive.TabIndex = 88
        Me.ChkBOMIsActive.UseVisualStyleBackColor = True
        Me.ChkBOMIsActive.Visible = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(1077, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ChkIsCleaningUsed)
        Me.SplitContainer1.Panel1.Controls.Add(Me.dtpCreatedDate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtProductionUnit)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboTargetStore)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCustomerID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboSourceStore)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCategoryID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCleaningChemicalUnit)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtBaseUnitID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtProductionAmount)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtProductName)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtProductionCode)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtNotes)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboBOMVersion)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnProductSearch)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtSubCategory)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboProductID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtProductGroupID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.dtpProductionDate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtBOMID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboCleaningChemical)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCleaningChemicalQTY)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnSearchBOM)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ChkBOMIsActive)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label22)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label23)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label6)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label26)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label25)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label7)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label9)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label8)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label24)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label13)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label10)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label11)
        Me.SplitContainer1.Size = New System.Drawing.Size(348, 808)
        Me.SplitContainer1.SplitterDistance = 172
        Me.SplitContainer1.TabIndex = 68
        '
        'frmProduction
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1532, 808)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlBtns)
        Me.Controls.Add(Me.Panel3)
        Me.Name = "frmProduction"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmProduction"
        CType(Me.dgvProductionCalculations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBtns.ResumeLayout(False)
        Me.pnlBtns.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.pnlTotals.ResumeLayout(False)
        Me.pnlTotals.PerformLayout()
        Me.sctDetails.Panel1.ResumeLayout(False)
        Me.sctDetails.Panel2.ResumeLayout(False)
        CType(Me.sctDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.sctDetails.ResumeLayout(False)
        CType(Me.dgvProduced, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Label14 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents cboSourceStore As ComboBox
    Friend WithEvents txtProductionAmount As TextBox
    Friend WithEvents Label22 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents cboBOMVersion As ComboBox
    Friend WithEvents txtBOMID As TextBox
    Friend WithEvents txtCleaningChemicalQTY As TextBox
    Friend WithEvents btnSearchBOM As Button
    Friend WithEvents Label25 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents dgvProductionCalculations As DataGridView
    Friend WithEvents Label13 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents txtDeviation As TextBox
    Friend WithEvents txtPastProductAverageCost As TextBox
    Friend WithEvents txtProductUnitCost As TextBox
    Friend WithEvents txtTotalProductionQTY As TextBox
    Friend WithEvents txtTotalProductionCost As TextBox
    Friend WithEvents txtTotalProductionValue As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents cboCleaningChemical As ComboBox
    Friend WithEvents dtpProductionDate As DateTimePicker
    Friend WithEvents dtpCreatedDate As DateTimePicker
    Friend WithEvents cboProductID As ComboBox
    Friend WithEvents btnProductSearch As Button
    Friend WithEvents pnlBtns As Panel
    Friend WithEvents lblBOMStatus As Label
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents txtProductionCode As TextBox
    Friend WithEvents txtProductName As TextBox
    Friend WithEvents txtBaseUnitID As TextBox
    Friend WithEvents txtCategoryID As TextBox
    Friend WithEvents txtCustomerID As TextBox
    Friend WithEvents txtTotalChemicalAmount As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents dgvProduced As DataGridView
    Friend WithEvents txtProductGroupID As TextBox
    Friend WithEvents Label24 As Label
    Friend WithEvents cboProductionStatus As ComboBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnExecuteProduction As Button
    Friend WithEvents pnlTotals As Panel
    Friend WithEvents sctDetails As SplitContainer
    Friend WithEvents ChkBOMIsActive As CheckBox
    Friend WithEvents txtProductionUnit As TextBox
    Friend WithEvents txtCleaningChemicalUnit As TextBox
    Friend WithEvents txtSubCategory As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label26 As Label
    Friend WithEvents colCalProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colCalProductID As DataGridViewTextBoxColumn
    Friend WithEvents colCalProductName As DataGridViewTextBoxColumn
    Friend WithEvents colCalProductUnit As DataGridViewTextBoxColumn
    Friend WithEvents colCalBOMQTY As DataGridViewTextBoxColumn
    Friend WithEvents colCalAvailableStock As DataGridViewTextBoxColumn
    Friend WithEvents colCalActualQTY As DataGridViewTextBoxColumn
    Friend WithEvents colCalCost As DataGridViewTextBoxColumn
    Friend WithEvents colManOutputID As DataGridViewTextBoxColumn
    Friend WithEvents colManlength As DataGridViewTextBoxColumn
    Friend WithEvents colManWidth As DataGridViewTextBoxColumn
    Friend WithEvents colManHeight As DataGridViewTextBoxColumn
    Friend WithEvents colManQTY As DataGridViewTextBoxColumn
    Friend WithEvents colManProductVolume As DataGridViewTextBoxColumn
    Friend WithEvents colManTotalProductVolume As DataGridViewTextBoxColumn
    Friend WithEvents ChkIsCleaningUsed As CheckBox
    Friend WithEvents btnCancelProduction As Button
    Friend WithEvents SplitContainer1 As SplitContainer
End Class
