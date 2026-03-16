<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCuttingWasteCalculator
    Inherits AABaseOperationForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.dgvCalculations = New System.Windows.Forms.DataGridView()
        Me.colDetailID = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colConsumption = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWastePercent = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWasteQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWasteValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductAvailableQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductDensity = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTotalWaste = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtStatusCode = New System.Windows.Forms.TextBox()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnCalculate = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.txtWasteReason = New System.Windows.Forms.TextBox()
        Me.txtWastePercent = New System.Windows.Forms.TextBox()
        Me.txtCurrentScrapStock = New System.Windows.Forms.TextBox()
        Me.txtWasteAvgCost = New System.Windows.Forms.TextBox()
        Me.txtCurrentScrapAvgCost = New System.Windows.Forms.TextBox()
        Me.cboCalculationType = New System.Windows.Forms.ComboBox()
        Me.cboWasteType = New System.Windows.Forms.ComboBox()
        Me.cboTargetStore = New System.Windows.Forms.ComboBox()
        Me.cboSourceStore = New System.Windows.Forms.ComboBox()
        Me.cboScrapCode = New System.Windows.Forms.ComboBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblWastePercent = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.dtpPeriodEnd = New System.Windows.Forms.DateTimePicker()
        Me.dtpPeriodStart = New System.Windows.Forms.DateTimePicker()
        Me.txtScrapAvgCost = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtTotalScrapValue = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtTotalWasteWeight = New System.Windows.Forms.TextBox()
        Me.lblWasteWeight = New System.Windows.Forms.Label()
        Me.txtTotalWasteVolume = New System.Windows.Forms.TextBox()
        Me.lblTotalWasteVolume = New System.Windows.Forms.Label()
        Me.txtTotalWasteValue = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        CType(Me.dgvCalculations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvCalculations
        '
        Me.dgvCalculations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCalculations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCalculations.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetailID, Me.colProductID, Me.colProductCode, Me.colConsumption, Me.colProductUnit, Me.colWastePercent, Me.colWasteQTY, Me.colWasteValue, Me.colProductAvailableQty, Me.colProductDensity, Me.colTotalWaste, Me.colNotes, Me.colDelete})
        Me.dgvCalculations.Dock = System.Windows.Forms.DockStyle.Left
        Me.dgvCalculations.Location = New System.Drawing.Point(0, 0)
        Me.dgvCalculations.Name = "dgvCalculations"
        Me.dgvCalculations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvCalculations.RowHeadersWidth = 51
        Me.dgvCalculations.RowTemplate.Height = 26
        Me.dgvCalculations.Size = New System.Drawing.Size(945, 752)
        Me.dgvCalculations.TabIndex = 0
        '
        'colDetailID
        '
        Me.colDetailID.HeaderText = "رقم التفصيلة"
        Me.colDetailID.MinimumWidth = 6
        Me.colDetailID.Name = "colDetailID"
        Me.colDetailID.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colDetailID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.colDetailID.Visible = False
        '
        'colProductID
        '
        Me.colProductID.HeaderText = "رقم الصنف"
        Me.colProductID.MinimumWidth = 6
        Me.colProductID.Name = "colProductID"
        Me.colProductID.Visible = False
        '
        'colProductCode
        '
        Me.colProductCode.HeaderText = "كود الصنف"
        Me.colProductCode.MinimumWidth = 6
        Me.colProductCode.Name = "colProductCode"
        '
        'colConsumption
        '
        Me.colConsumption.HeaderText = "استهلاك الفترة"
        Me.colConsumption.MinimumWidth = 6
        Me.colConsumption.Name = "colConsumption"
        '
        'colProductUnit
        '
        Me.colProductUnit.HeaderText = "الوحدة"
        Me.colProductUnit.MinimumWidth = 6
        Me.colProductUnit.Name = "colProductUnit"
        Me.colProductUnit.Visible = False
        '
        'colWastePercent
        '
        Me.colWastePercent.HeaderText = "نسبة الهالك"
        Me.colWastePercent.MinimumWidth = 6
        Me.colWastePercent.Name = "colWastePercent"
        '
        'colWasteQTY
        '
        Me.colWasteQTY.HeaderText = "كمية الهالك"
        Me.colWasteQTY.MinimumWidth = 6
        Me.colWasteQTY.Name = "colWasteQTY"
        '
        'colWasteValue
        '
        Me.colWasteValue.HeaderText = "قيمة الهالك"
        Me.colWasteValue.MinimumWidth = 6
        Me.colWasteValue.Name = "colWasteValue"
        '
        'colProductAvailableQty
        '
        Me.colProductAvailableQty.HeaderText = "الكمية المتاحة"
        Me.colProductAvailableQty.MinimumWidth = 6
        Me.colProductAvailableQty.Name = "colProductAvailableQty"
        '
        'colProductDensity
        '
        Me.colProductDensity.HeaderText = "كثافة المنتج"
        Me.colProductDensity.MinimumWidth = 6
        Me.colProductDensity.Name = "colProductDensity"
        '
        'colTotalWaste
        '
        Me.colTotalWaste.HeaderText = "اجمالي وزن الهالك"
        Me.colTotalWaste.MinimumWidth = 6
        Me.colTotalWaste.Name = "colTotalWaste"
        '
        'colNotes
        '
        Me.colNotes.HeaderText = "ملاحظات"
        Me.colNotes.MinimumWidth = 6
        Me.colNotes.Name = "colNotes"
        Me.colNotes.Visible = False
        '
        'colDelete
        '
        Me.colDelete.HeaderText = "حذف"
        Me.colDelete.MinimumWidth = 6
        Me.colDelete.Name = "colDelete"
        Me.colDelete.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colDelete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.txtStatusCode)
        Me.Panel1.Controls.Add(Me.btnClose)
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnCalculate)
        Me.Panel1.Controls.Add(Me.btnSave)
        Me.Panel1.Controls.Add(Me.btnNew)
        Me.Panel1.Controls.Add(Me.btnSend)
        Me.Panel1.Controls.Add(Me.btnSearch)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel1.Location = New System.Drawing.Point(1298, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(137, 752)
        Me.Panel1.TabIndex = 1
        '
        'txtStatusCode
        '
        Me.txtStatusCode.Location = New System.Drawing.Point(4, 4)
        Me.txtStatusCode.Name = "txtStatusCode"
        Me.txtStatusCode.Size = New System.Drawing.Size(128, 24)
        Me.txtStatusCode.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(6, 556)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(126, 42)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "إغلاق"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(6, 508)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(126, 42)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnCalculate
        '
        Me.btnCalculate.Location = New System.Drawing.Point(6, 206)
        Me.btnCalculate.Name = "btnCalculate"
        Me.btnCalculate.Size = New System.Drawing.Size(126, 42)
        Me.btnCalculate.TabIndex = 0
        Me.btnCalculate.Text = "اجراء الحساب"
        Me.btnCalculate.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(6, 364)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(126, 42)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "حفظ"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Location = New System.Drawing.Point(6, 307)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(126, 42)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = True
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(6, 412)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(126, 42)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "ارسال للمخزن"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'btnSearch
        '
        Me.btnSearch.Location = New System.Drawing.Point(6, 460)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(126, 42)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(945, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtWasteReason)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtWastePercent)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCurrentScrapStock)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtWasteAvgCost)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCurrentScrapAvgCost)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboCalculationType)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboWasteType)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboTargetStore)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboSourceStore)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cboScrapCode)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label16)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label6)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label9)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblWastePercent)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label17)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label8)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label7)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.dtpPeriodEnd)
        Me.SplitContainer1.Panel1.Controls.Add(Me.dtpPeriodStart)
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtScrapAvgCost)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label13)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtTotalScrapValue)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label11)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtTotalWasteWeight)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblWasteWeight)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtTotalWasteVolume)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblTotalWasteVolume)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtTotalWasteValue)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label10)
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.SplitContainer1.Size = New System.Drawing.Size(353, 752)
        Me.SplitContainer1.SplitterDistance = 501
        Me.SplitContainer1.TabIndex = 2
        '
        'txtWasteReason
        '
        Me.txtWasteReason.Location = New System.Drawing.Point(6, 422)
        Me.txtWasteReason.Multiline = True
        Me.txtWasteReason.Name = "txtWasteReason"
        Me.txtWasteReason.Size = New System.Drawing.Size(161, 76)
        Me.txtWasteReason.TabIndex = 4
        '
        'txtWastePercent
        '
        Me.txtWastePercent.Location = New System.Drawing.Point(7, 355)
        Me.txtWastePercent.Name = "txtWastePercent"
        Me.txtWastePercent.Size = New System.Drawing.Size(161, 24)
        Me.txtWastePercent.TabIndex = 4
        '
        'txtCurrentScrapStock
        '
        Me.txtCurrentScrapStock.Enabled = False
        Me.txtCurrentScrapStock.Location = New System.Drawing.Point(7, 126)
        Me.txtCurrentScrapStock.Name = "txtCurrentScrapStock"
        Me.txtCurrentScrapStock.ReadOnly = True
        Me.txtCurrentScrapStock.Size = New System.Drawing.Size(161, 24)
        Me.txtCurrentScrapStock.TabIndex = 4
        '
        'txtWasteAvgCost
        '
        Me.txtWasteAvgCost.Location = New System.Drawing.Point(6, 206)
        Me.txtWasteAvgCost.Name = "txtWasteAvgCost"
        Me.txtWasteAvgCost.Size = New System.Drawing.Size(161, 24)
        Me.txtWasteAvgCost.TabIndex = 4
        '
        'txtCurrentScrapAvgCost
        '
        Me.txtCurrentScrapAvgCost.Enabled = False
        Me.txtCurrentScrapAvgCost.Location = New System.Drawing.Point(7, 96)
        Me.txtCurrentScrapAvgCost.Name = "txtCurrentScrapAvgCost"
        Me.txtCurrentScrapAvgCost.ReadOnly = True
        Me.txtCurrentScrapAvgCost.Size = New System.Drawing.Size(161, 24)
        Me.txtCurrentScrapAvgCost.TabIndex = 4
        '
        'cboCalculationType
        '
        Me.cboCalculationType.Enabled = False
        Me.cboCalculationType.FormattingEnabled = True
        Me.cboCalculationType.Location = New System.Drawing.Point(7, 325)
        Me.cboCalculationType.Name = "cboCalculationType"
        Me.cboCalculationType.Size = New System.Drawing.Size(161, 24)
        Me.cboCalculationType.TabIndex = 3
        '
        'cboWasteType
        '
        Me.cboWasteType.Enabled = False
        Me.cboWasteType.FormattingEnabled = True
        Me.cboWasteType.Location = New System.Drawing.Point(6, 295)
        Me.cboWasteType.Name = "cboWasteType"
        Me.cboWasteType.Size = New System.Drawing.Size(161, 24)
        Me.cboWasteType.TabIndex = 3
        '
        'cboTargetStore
        '
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(7, 265)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.Size = New System.Drawing.Size(161, 24)
        Me.cboTargetStore.TabIndex = 3
        '
        'cboSourceStore
        '
        Me.cboSourceStore.FormattingEnabled = True
        Me.cboSourceStore.Location = New System.Drawing.Point(6, 236)
        Me.cboSourceStore.Name = "cboSourceStore"
        Me.cboSourceStore.Size = New System.Drawing.Size(161, 24)
        Me.cboSourceStore.TabIndex = 3
        '
        'cboScrapCode
        '
        Me.cboScrapCode.FormattingEnabled = True
        Me.cboScrapCode.Location = New System.Drawing.Point(6, 176)
        Me.cboScrapCode.Name = "cboScrapCode"
        Me.cboScrapCode.Size = New System.Drawing.Size(161, 24)
        Me.cboScrapCode.TabIndex = 3
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(235, 325)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(97, 17)
        Me.Label16.TabIndex = 2
        Me.Label16.Text = "طريقة الحساب "
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(216, 209)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(116, 17)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "سعر الوحدة للهالك"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(268, 295)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(64, 17)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "نوع الهالك"
        '
        'lblWastePercent
        '
        Me.lblWastePercent.AutoSize = True
        Me.lblWastePercent.Location = New System.Drawing.Point(239, 358)
        Me.lblWastePercent.Name = "lblWastePercent"
        Me.lblWastePercent.Size = New System.Drawing.Size(91, 17)
        Me.lblWastePercent.TabIndex = 2
        Me.lblWastePercent.Text = "النسبة المئوية"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(251, 425)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(78, 17)
        Me.Label17.TabIndex = 2
        Me.Label17.Text = "سبب الهالك"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(197, 133)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(148, 17)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "المخزون المتاح للسكراب"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(239, 265)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(93, 17)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "مخزن الاستلام"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(252, 236)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(80, 17)
        Me.Label7.TabIndex = 2
        Me.Label7.Text = "مخزن الصرف"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(188, 99)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(157, 17)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "المتوسط الحالي للسكراب"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(251, 176)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 17)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "صنف السكراب"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(260, 42)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(72, 17)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "نهاية الفترة"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(254, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(75, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "بداية الفترة "
        '
        'dtpPeriodEnd
        '
        Me.dtpPeriodEnd.Location = New System.Drawing.Point(3, 42)
        Me.dtpPeriodEnd.Name = "dtpPeriodEnd"
        Me.dtpPeriodEnd.Size = New System.Drawing.Size(161, 24)
        Me.dtpPeriodEnd.TabIndex = 1
        '
        'dtpPeriodStart
        '
        Me.dtpPeriodStart.Location = New System.Drawing.Point(3, 12)
        Me.dtpPeriodStart.Name = "dtpPeriodStart"
        Me.dtpPeriodStart.Size = New System.Drawing.Size(161, 24)
        Me.dtpPeriodStart.TabIndex = 0
        '
        'txtScrapAvgCost
        '
        Me.txtScrapAvgCost.Location = New System.Drawing.Point(7, 164)
        Me.txtScrapAvgCost.Name = "txtScrapAvgCost"
        Me.txtScrapAvgCost.Size = New System.Drawing.Size(161, 24)
        Me.txtScrapAvgCost.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(177, 171)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(155, 17)
        Me.Label13.TabIndex = 2
        Me.Label13.Text = "المتوسط الجديد للسكراب"
        '
        'txtTotalScrapValue
        '
        Me.txtTotalScrapValue.Location = New System.Drawing.Point(7, 102)
        Me.txtTotalScrapValue.Name = "txtTotalScrapValue"
        Me.txtTotalScrapValue.Size = New System.Drawing.Size(161, 24)
        Me.txtTotalScrapValue.TabIndex = 0
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(215, 109)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(117, 17)
        Me.Label11.TabIndex = 2
        Me.Label11.Text = "اجمالي قيمة الناتج"
        '
        'txtTotalWasteWeight
        '
        Me.txtTotalWasteWeight.Location = New System.Drawing.Point(7, 42)
        Me.txtTotalWasteWeight.Name = "txtTotalWasteWeight"
        Me.txtTotalWasteWeight.Size = New System.Drawing.Size(161, 24)
        Me.txtTotalWasteWeight.TabIndex = 0
        '
        'lblWasteWeight
        '
        Me.lblWasteWeight.AutoSize = True
        Me.lblWasteWeight.Location = New System.Drawing.Point(219, 49)
        Me.lblWasteWeight.Name = "lblWasteWeight"
        Me.lblWasteWeight.Size = New System.Drawing.Size(113, 17)
        Me.lblWasteWeight.TabIndex = 2
        Me.lblWasteWeight.Text = "اجمالي وزن الهالك"
        '
        'txtTotalWasteVolume
        '
        Me.txtTotalWasteVolume.Location = New System.Drawing.Point(6, 12)
        Me.txtTotalWasteVolume.Name = "txtTotalWasteVolume"
        Me.txtTotalWasteVolume.Size = New System.Drawing.Size(161, 24)
        Me.txtTotalWasteVolume.TabIndex = 0
        '
        'lblTotalWasteVolume
        '
        Me.lblTotalWasteVolume.AutoSize = True
        Me.lblTotalWasteVolume.Location = New System.Drawing.Point(210, 19)
        Me.lblTotalWasteVolume.Name = "lblTotalWasteVolume"
        Me.lblTotalWasteVolume.Size = New System.Drawing.Size(122, 17)
        Me.lblTotalWasteVolume.TabIndex = 2
        Me.lblTotalWasteVolume.Text = "اجمالي حجم الهالك"
        '
        'txtTotalWasteValue
        '
        Me.txtTotalWasteValue.Location = New System.Drawing.Point(7, 72)
        Me.txtTotalWasteValue.Name = "txtTotalWasteValue"
        Me.txtTotalWasteValue.Size = New System.Drawing.Size(161, 24)
        Me.txtTotalWasteValue.TabIndex = 0
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(211, 79)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(121, 17)
        Me.Label10.TabIndex = 2
        Me.Label10.Text = "اجمالي قيمة الهالك"
        '
        'frmCuttingWasteCalculator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1435, 752)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.dgvCalculations)
        Me.Name = "frmCuttingWasteCalculator"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmCuttingScrabCalculator"
        CType(Me.dgvCalculations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents dgvCalculations As DataGridView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents Label1 As Label
    Friend WithEvents dtpPeriodEnd As DateTimePicker
    Friend WithEvents dtpPeriodStart As DateTimePicker
    Friend WithEvents Label2 As Label
    Friend WithEvents txtCurrentScrapStock As TextBox
    Friend WithEvents txtCurrentScrapAvgCost As TextBox
    Friend WithEvents cboScrapCode As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtWasteAvgCost As TextBox
    Friend WithEvents cboWasteType As ComboBox
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents cboSourceStore As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents txtScrapAvgCost As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtTotalScrapValue As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txtTotalWasteWeight As TextBox
    Friend WithEvents lblWasteWeight As Label
    Friend WithEvents txtTotalWasteVolume As TextBox
    Friend WithEvents lblTotalWasteVolume As Label
    Friend WithEvents txtTotalWasteValue As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents txtStatusCode As TextBox
    Friend WithEvents btnClose As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSend As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents cboCalculationType As ComboBox
    Friend WithEvents Label16 As Label
    Friend WithEvents btnSave As Button
    Friend WithEvents txtWasteReason As TextBox
    Friend WithEvents Label17 As Label
    Friend WithEvents txtWastePercent As TextBox
    Friend WithEvents lblWastePercent As Label
    Friend WithEvents btnCalculate As Button
    Friend WithEvents colDetailID As DataGridViewButtonColumn
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colConsumption As DataGridViewTextBoxColumn
    Friend WithEvents colProductUnit As DataGridViewTextBoxColumn
    Friend WithEvents colWastePercent As DataGridViewTextBoxColumn
    Friend WithEvents colWasteQTY As DataGridViewTextBoxColumn
    Friend WithEvents colWasteValue As DataGridViewTextBoxColumn
    Friend WithEvents colProductAvailableQty As DataGridViewTextBoxColumn
    Friend WithEvents colProductDensity As DataGridViewTextBoxColumn
    Friend WithEvents colTotalWaste As DataGridViewTextBoxColumn
    Friend WithEvents colNotes As DataGridViewTextBoxColumn
    Friend WithEvents colDelete As DataGridViewButtonColumn
End Class
