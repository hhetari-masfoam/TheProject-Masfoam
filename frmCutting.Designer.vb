<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCutting
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtCuttingStatus = New System.Windows.Forms.TextBox()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.pnlProductSpecs = New System.Windows.Forms.Panel()
        Me.cboTargetedStore = New System.Windows.Forms.ComboBox()
        Me.cboSourceStore = New System.Windows.Forms.ComboBox()
        Me.txtProductNote = New System.Windows.Forms.TextBox()
        Me.cboCuttingCode = New System.Windows.Forms.ComboBox()
        Me.dtpCuttingDate = New System.Windows.Forms.DateTimePicker()
        Me.txtProductType = New System.Windows.Forms.TextBox()
        Me.txtAvailableQTY = New System.Windows.Forms.TextBox()
        Me.txtProductMixType = New System.Windows.Forms.TextBox()
        Me.txtProductColor = New System.Windows.Forms.TextBox()
        Me.txtProductGroup = New System.Windows.Forms.TextBox()
        Me.txtProductCategory = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtEnglishName = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.lbl2 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.btnFind = New System.Windows.Forms.Button()
        Me.txtStoreID = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cboProductCode = New System.Windows.Forms.ComboBox()
        Me.dgvOutPut = New System.Windows.Forms.DataGridView()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutProductTypeID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutWidth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutHeight = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutProductType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutPieceVolume = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOutTotalVolume = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.pnlOutPutsCalculations = New System.Windows.Forms.Panel()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.TotalPcsOutPut = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtTotalVolumeOutPut = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.tabCutting = New System.Windows.Forms.TabControl()
        Me.TabCuttingProcess = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvInPut = New System.Windows.Forms.DataGridView()
        Me.colLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWidth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colHeight = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colAdd = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.tabCuttingFollow = New System.Windows.Forms.TabPage()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.pnlProductSpecs.SuspendLayout()
        CType(Me.dgvOutPut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlOutPutsCalculations.SuspendLayout()
        Me.tabCutting.SuspendLayout()
        Me.TabCuttingProcess.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvInPut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabCuttingFollow.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.txtCuttingStatus)
        Me.Panel1.Controls.Add(Me.btnSend)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Controls.Add(Me.Button5)
        Me.Panel1.Controls.Add(Me.btnSearch)
        Me.Panel1.Controls.Add(Me.btnClose)
        Me.Panel1.Controls.Add(Me.btnSave)
        Me.Panel1.Controls.Add(Me.btnNew)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel1.Location = New System.Drawing.Point(1442, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(140, 753)
        Me.Panel1.TabIndex = 0
        '
        'txtCuttingStatus
        '
        Me.txtCuttingStatus.Enabled = False
        Me.txtCuttingStatus.Location = New System.Drawing.Point(6, 4)
        Me.txtCuttingStatus.Name = "txtCuttingStatus"
        Me.txtCuttingStatus.Size = New System.Drawing.Size(129, 24)
        Me.txtCuttingStatus.TabIndex = 10
        '
        'btnSend
        '
        Me.btnSend.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSend.Location = New System.Drawing.Point(8, 319)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(129, 55)
        Me.btnSend.TabIndex = 1
        Me.btnSend.Text = "ارسال الناتج"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(5, 629)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(129, 55)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "طباعة"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button5.Location = New System.Drawing.Point(5, 690)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(129, 55)
        Me.Button5.TabIndex = 0
        Me.Button5.Text = "حذف"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'btnSearch
        '
        Me.btnSearch.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearch.Location = New System.Drawing.Point(8, 381)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(129, 55)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(8, 514)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(129, 55)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "اغلاق"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.Location = New System.Drawing.Point(8, 257)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(129, 55)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "حفظ"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNew.Location = New System.Drawing.Point(8, 195)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(129, 55)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = True
        '
        'pnlProductSpecs
        '
        Me.pnlProductSpecs.Controls.Add(Me.cboTargetedStore)
        Me.pnlProductSpecs.Controls.Add(Me.cboSourceStore)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductNote)
        Me.pnlProductSpecs.Controls.Add(Me.cboCuttingCode)
        Me.pnlProductSpecs.Controls.Add(Me.dtpCuttingDate)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductType)
        Me.pnlProductSpecs.Controls.Add(Me.txtAvailableQTY)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductMixType)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductColor)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductGroup)
        Me.pnlProductSpecs.Controls.Add(Me.txtProductCategory)
        Me.pnlProductSpecs.Controls.Add(Me.Label12)
        Me.pnlProductSpecs.Controls.Add(Me.Label15)
        Me.pnlProductSpecs.Controls.Add(Me.Label16)
        Me.pnlProductSpecs.Controls.Add(Me.Label14)
        Me.pnlProductSpecs.Controls.Add(Me.Label13)
        Me.pnlProductSpecs.Controls.Add(Me.Label10)
        Me.pnlProductSpecs.Controls.Add(Me.txtEnglishName)
        Me.pnlProductSpecs.Controls.Add(Me.Label8)
        Me.pnlProductSpecs.Controls.Add(Me.lbl2)
        Me.pnlProductSpecs.Controls.Add(Me.Label25)
        Me.pnlProductSpecs.Controls.Add(Me.btnFind)
        Me.pnlProductSpecs.Controls.Add(Me.txtStoreID)
        Me.pnlProductSpecs.Controls.Add(Me.Label11)
        Me.pnlProductSpecs.Controls.Add(Me.Label7)
        Me.pnlProductSpecs.Controls.Add(Me.Label5)
        Me.pnlProductSpecs.Controls.Add(Me.cboProductCode)
        Me.pnlProductSpecs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlProductSpecs.Location = New System.Drawing.Point(1038, 3)
        Me.pnlProductSpecs.Name = "pnlProductSpecs"
        Me.pnlProductSpecs.Size = New System.Drawing.Size(393, 673)
        Me.pnlProductSpecs.TabIndex = 0
        '
        'cboTargetedStore
        '
        Me.cboTargetedStore.FormattingEnabled = True
        Me.cboTargetedStore.Location = New System.Drawing.Point(5, 155)
        Me.cboTargetedStore.Name = "cboTargetedStore"
        Me.cboTargetedStore.Size = New System.Drawing.Size(181, 24)
        Me.cboTargetedStore.TabIndex = 8
        '
        'cboSourceStore
        '
        Me.cboSourceStore.FormattingEnabled = True
        Me.cboSourceStore.Location = New System.Drawing.Point(5, 125)
        Me.cboSourceStore.Name = "cboSourceStore"
        Me.cboSourceStore.Size = New System.Drawing.Size(181, 24)
        Me.cboSourceStore.TabIndex = 8
        '
        'txtProductNote
        '
        Me.txtProductNote.Location = New System.Drawing.Point(4, 537)
        Me.txtProductNote.Multiline = True
        Me.txtProductNote.Name = "txtProductNote"
        Me.txtProductNote.Size = New System.Drawing.Size(182, 130)
        Me.txtProductNote.TabIndex = 6
        '
        'cboCuttingCode
        '
        Me.cboCuttingCode.Enabled = False
        Me.cboCuttingCode.FormattingEnabled = True
        Me.cboCuttingCode.Location = New System.Drawing.Point(3, 61)
        Me.cboCuttingCode.Name = "cboCuttingCode"
        Me.cboCuttingCode.Size = New System.Drawing.Size(183, 24)
        Me.cboCuttingCode.TabIndex = 5
        '
        'dtpCuttingDate
        '
        Me.dtpCuttingDate.Enabled = False
        Me.dtpCuttingDate.Location = New System.Drawing.Point(3, 31)
        Me.dtpCuttingDate.Name = "dtpCuttingDate"
        Me.dtpCuttingDate.Size = New System.Drawing.Size(183, 24)
        Me.dtpCuttingDate.TabIndex = 4
        '
        'txtProductType
        '
        Me.txtProductType.Location = New System.Drawing.Point(3, 447)
        Me.txtProductType.Name = "txtProductType"
        Me.txtProductType.Size = New System.Drawing.Size(183, 24)
        Me.txtProductType.TabIndex = 3
        '
        'txtAvailableQTY
        '
        Me.txtAvailableQTY.Location = New System.Drawing.Point(3, 507)
        Me.txtAvailableQTY.Name = "txtAvailableQTY"
        Me.txtAvailableQTY.Size = New System.Drawing.Size(183, 24)
        Me.txtAvailableQTY.TabIndex = 3
        '
        'txtProductMixType
        '
        Me.txtProductMixType.Location = New System.Drawing.Point(3, 477)
        Me.txtProductMixType.Name = "txtProductMixType"
        Me.txtProductMixType.Size = New System.Drawing.Size(183, 24)
        Me.txtProductMixType.TabIndex = 3
        '
        'txtProductColor
        '
        Me.txtProductColor.Location = New System.Drawing.Point(3, 417)
        Me.txtProductColor.Name = "txtProductColor"
        Me.txtProductColor.Size = New System.Drawing.Size(183, 24)
        Me.txtProductColor.TabIndex = 3
        '
        'txtProductGroup
        '
        Me.txtProductGroup.Location = New System.Drawing.Point(3, 387)
        Me.txtProductGroup.Name = "txtProductGroup"
        Me.txtProductGroup.Size = New System.Drawing.Size(183, 24)
        Me.txtProductGroup.TabIndex = 3
        '
        'txtProductCategory
        '
        Me.txtProductCategory.Location = New System.Drawing.Point(3, 357)
        Me.txtProductCategory.Name = "txtProductCategory"
        Me.txtProductCategory.Size = New System.Drawing.Size(183, 24)
        Me.txtProductCategory.TabIndex = 3
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(218, 549)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(111, 17)
        Me.Label12.TabIndex = 0
        Me.Label12.Text = "ملاحظات المبيعات"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(295, 447)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(34, 17)
        Me.Label15.TabIndex = 0
        Me.Label15.Text = "النوع"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(235, 507)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(94, 17)
        Me.Label16.TabIndex = 0
        Me.Label16.Text = "الكمية المتوفرة"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(254, 477)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(75, 17)
        Me.Label14.TabIndex = 0
        Me.Label14.Text = "نوع المكس "
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(294, 417)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(35, 17)
        Me.Label13.TabIndex = 0
        Me.Label13.Text = "اللون"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(233, 387)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(96, 17)
        Me.Label10.TabIndex = 0
        Me.Label10.Text = "مجموعة المنتج"
        '
        'txtEnglishName
        '
        Me.txtEnglishName.Location = New System.Drawing.Point(3, 327)
        Me.txtEnglishName.Name = "txtEnglishName"
        Me.txtEnglishName.Size = New System.Drawing.Size(183, 24)
        Me.txtEnglishName.TabIndex = 3
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(262, 357)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(67, 17)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "فئة المنتج"
        '
        'lbl2
        '
        Me.lbl2.AutoSize = True
        Me.lbl2.Location = New System.Drawing.Point(236, 327)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(93, 17)
        Me.lbl2.TabIndex = 0
        Me.lbl2.Text = "الاسم انجليزي"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(251, 155)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(78, 17)
        Me.Label25.TabIndex = 0
        Me.Label25.Text = "مخزن الانتاج"
        '
        'btnFind
        '
        Me.btnFind.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFind.Location = New System.Drawing.Point(3, 215)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(183, 54)
        Me.btnFind.TabIndex = 2
        Me.btnFind.Text = "اختيار المنتح"
        Me.btnFind.UseVisualStyleBackColor = True
        '
        'txtStoreID
        '
        Me.txtStoreID.AutoSize = True
        Me.txtStoreID.Location = New System.Drawing.Point(255, 125)
        Me.txtStoreID.Name = "txtStoreID"
        Me.txtStoreID.Size = New System.Drawing.Size(74, 17)
        Me.txtStoreID.TabIndex = 0
        Me.txtStoreID.Text = "مخزن القص"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(228, 61)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(101, 17)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "كود عملية القص"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(286, 38)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(43, 17)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "التاريخ"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(261, 185)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(68, 17)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "كود الصنف"
        '
        'cboProductCode
        '
        Me.cboProductCode.FormattingEnabled = True
        Me.cboProductCode.Location = New System.Drawing.Point(5, 185)
        Me.cboProductCode.Name = "cboProductCode"
        Me.cboProductCode.Size = New System.Drawing.Size(181, 24)
        Me.cboProductCode.TabIndex = 1
        '
        'dgvOutPut
        '
        Me.dgvOutPut.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvOutPut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvOutPut.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductID, Me.colOutProductTypeID, Me.colOutLength, Me.colOutWidth, Me.colOutHeight, Me.colOutQTY, Me.colOutProductCode, Me.colOutProductType, Me.colOutPieceVolume, Me.colOutTotalVolume, Me.colDelete})
        Me.dgvOutPut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOutPut.Location = New System.Drawing.Point(0, 0)
        Me.dgvOutPut.Name = "dgvOutPut"
        Me.dgvOutPut.RowHeadersWidth = 51
        Me.dgvOutPut.RowTemplate.Height = 26
        Me.dgvOutPut.Size = New System.Drawing.Size(1035, 604)
        Me.dgvOutPut.TabIndex = 2
        '
        'colProductID
        '
        Me.colProductID.HeaderText = "Column1"
        Me.colProductID.MinimumWidth = 6
        Me.colProductID.Name = "colProductID"
        Me.colProductID.Visible = False
        '
        'colOutProductTypeID
        '
        Me.colOutProductTypeID.HeaderText = "Column1"
        Me.colOutProductTypeID.MinimumWidth = 6
        Me.colOutProductTypeID.Name = "colOutProductTypeID"
        Me.colOutProductTypeID.Visible = False
        '
        'colOutLength
        '
        Me.colOutLength.HeaderText = "الطول"
        Me.colOutLength.MinimumWidth = 6
        Me.colOutLength.Name = "colOutLength"
        '
        'colOutWidth
        '
        Me.colOutWidth.HeaderText = "العرض"
        Me.colOutWidth.MinimumWidth = 6
        Me.colOutWidth.Name = "colOutWidth"
        '
        'colOutHeight
        '
        Me.colOutHeight.HeaderText = "الارتفاع"
        Me.colOutHeight.MinimumWidth = 6
        Me.colOutHeight.Name = "colOutHeight"
        '
        'colOutQTY
        '
        Me.colOutQTY.HeaderText = "العدد"
        Me.colOutQTY.MinimumWidth = 6
        Me.colOutQTY.Name = "colOutQTY"
        '
        'colOutProductCode
        '
        Me.colOutProductCode.HeaderText = "كود الصنف"
        Me.colOutProductCode.MinimumWidth = 6
        Me.colOutProductCode.Name = "colOutProductCode"
        '
        'colOutProductType
        '
        Me.colOutProductType.HeaderText = "النوع"
        Me.colOutProductType.MinimumWidth = 6
        Me.colOutProductType.Name = "colOutProductType"
        Me.colOutProductType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colOutProductType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'colOutPieceVolume
        '
        Me.colOutPieceVolume.HeaderText = "حجم القطعة"
        Me.colOutPieceVolume.MinimumWidth = 6
        Me.colOutPieceVolume.Name = "colOutPieceVolume"
        '
        'colOutTotalVolume
        '
        Me.colOutTotalVolume.HeaderText = "اجمالي الحجم"
        Me.colOutTotalVolume.MinimumWidth = 6
        Me.colOutTotalVolume.Name = "colOutTotalVolume"
        '
        'colDelete
        '
        Me.colDelete.HeaderText = ""
        Me.colDelete.MinimumWidth = 6
        Me.colDelete.Name = "colDelete"
        Me.colDelete.Text = "حذف"
        '
        'pnlOutPutsCalculations
        '
        Me.pnlOutPutsCalculations.Controls.Add(Me.Label22)
        Me.pnlOutPutsCalculations.Controls.Add(Me.txtNotes)
        Me.pnlOutPutsCalculations.Controls.Add(Me.TotalPcsOutPut)
        Me.pnlOutPutsCalculations.Controls.Add(Me.Label18)
        Me.pnlOutPutsCalculations.Controls.Add(Me.txtTotalVolumeOutPut)
        Me.pnlOutPutsCalculations.Controls.Add(Me.Label17)
        Me.pnlOutPutsCalculations.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlOutPutsCalculations.Location = New System.Drawing.Point(3, 676)
        Me.pnlOutPutsCalculations.Name = "pnlOutPutsCalculations"
        Me.pnlOutPutsCalculations.Size = New System.Drawing.Size(1428, 45)
        Me.pnlOutPutsCalculations.TabIndex = 0
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(1305, 15)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(59, 17)
        Me.Label22.TabIndex = 7
        Me.Label22.Text = "ملاحظات"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(760, 11)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(461, 24)
        Me.txtNotes.TabIndex = 6
        '
        'TotalPcsOutPut
        '
        Me.TotalPcsOutPut.Location = New System.Drawing.Point(100, 11)
        Me.TotalPcsOutPut.Name = "TotalPcsOutPut"
        Me.TotalPcsOutPut.Size = New System.Drawing.Size(106, 24)
        Me.TotalPcsOutPut.TabIndex = 1
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(222, 15)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(156, 17)
        Me.Label18.TabIndex = 0
        Me.Label18.Text = "اجمالي عدد القطع الناتجة"
        '
        'txtTotalVolumeOutPut
        '
        Me.txtTotalVolumeOutPut.Location = New System.Drawing.Point(476, 11)
        Me.txtTotalVolumeOutPut.Name = "txtTotalVolumeOutPut"
        Me.txtTotalVolumeOutPut.Size = New System.Drawing.Size(106, 24)
        Me.txtTotalVolumeOutPut.TabIndex = 1
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(628, 15)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(126, 17)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "اجمالي الحجم الناتج"
        '
        'tabCutting
        '
        Me.tabCutting.Controls.Add(Me.TabCuttingProcess)
        Me.tabCutting.Controls.Add(Me.tabCuttingFollow)
        Me.tabCutting.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabCutting.Location = New System.Drawing.Point(0, 0)
        Me.tabCutting.Name = "tabCutting"
        Me.tabCutting.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tabCutting.RightToLeftLayout = True
        Me.tabCutting.SelectedIndex = 0
        Me.tabCutting.Size = New System.Drawing.Size(1442, 753)
        Me.tabCutting.TabIndex = 1
        '
        'TabCuttingProcess
        '
        Me.TabCuttingProcess.Controls.Add(Me.pnlProductSpecs)
        Me.TabCuttingProcess.Controls.Add(Me.SplitContainer1)
        Me.TabCuttingProcess.Controls.Add(Me.pnlOutPutsCalculations)
        Me.TabCuttingProcess.Location = New System.Drawing.Point(4, 25)
        Me.TabCuttingProcess.Name = "TabCuttingProcess"
        Me.TabCuttingProcess.Padding = New System.Windows.Forms.Padding(3)
        Me.TabCuttingProcess.Size = New System.Drawing.Size(1434, 724)
        Me.TabCuttingProcess.TabIndex = 0
        Me.TabCuttingProcess.Text = "قص المنتجات"
        Me.TabCuttingProcess.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Left
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvInPut)
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.dgvOutPut)
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.SplitContainer1.Size = New System.Drawing.Size(1035, 673)
        Me.SplitContainer1.SplitterDistance = 65
        Me.SplitContainer1.TabIndex = 1
        '
        'dgvInPut
        '
        Me.dgvInPut.AllowUserToDeleteRows = False
        Me.dgvInPut.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvInPut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInPut.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colLength, Me.colWidth, Me.colHeight, Me.colQTY, Me.colProductType, Me.colAdd})
        Me.dgvInPut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvInPut.Location = New System.Drawing.Point(0, 0)
        Me.dgvInPut.Name = "dgvInPut"
        Me.dgvInPut.RowHeadersWidth = 51
        Me.dgvInPut.RowTemplate.Height = 26
        Me.dgvInPut.Size = New System.Drawing.Size(1035, 65)
        Me.dgvInPut.TabIndex = 2
        '
        'colLength
        '
        Me.colLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colLength.HeaderText = "الطول"
        Me.colLength.MinimumWidth = 6
        Me.colLength.Name = "colLength"
        Me.colLength.Width = 125
        '
        'colWidth
        '
        Me.colWidth.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colWidth.HeaderText = "العرض"
        Me.colWidth.MinimumWidth = 6
        Me.colWidth.Name = "colWidth"
        Me.colWidth.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colWidth.Width = 125
        '
        'colHeight
        '
        Me.colHeight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colHeight.HeaderText = "الارتفاع"
        Me.colHeight.MinimumWidth = 6
        Me.colHeight.Name = "colHeight"
        Me.colHeight.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colHeight.Width = 125
        '
        'colQTY
        '
        Me.colQTY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colQTY.HeaderText = "العدد"
        Me.colQTY.MinimumWidth = 6
        Me.colQTY.Name = "colQTY"
        Me.colQTY.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colQTY.Width = 125
        '
        'colProductType
        '
        Me.colProductType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colProductType.HeaderText = "النوع"
        Me.colProductType.MinimumWidth = 6
        Me.colProductType.Name = "colProductType"
        Me.colProductType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.colProductType.Width = 125
        '
        'colAdd
        '
        Me.colAdd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.colAdd.HeaderText = ""
        Me.colAdd.MinimumWidth = 6
        Me.colAdd.Name = "colAdd"
        Me.colAdd.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colAdd.Text = "إضافة"
        Me.colAdd.UseColumnTextForButtonValue = True
        Me.colAdd.Width = 125
        '
        'tabCuttingFollow
        '
        Me.tabCuttingFollow.Controls.Add(Me.DataGridView1)
        Me.tabCuttingFollow.Location = New System.Drawing.Point(4, 25)
        Me.tabCuttingFollow.Name = "tabCuttingFollow"
        Me.tabCuttingFollow.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCuttingFollow.Size = New System.Drawing.Size(1434, 724)
        Me.tabCuttingFollow.TabIndex = 1
        Me.tabCuttingFollow.Text = "القص غير المحول"
        Me.tabCuttingFollow.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(7, 4)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 51
        Me.DataGridView1.RowTemplate.Height = 26
        Me.DataGridView1.Size = New System.Drawing.Size(896, 275)
        Me.DataGridView1.TabIndex = 0
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(8, 445)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(129, 55)
        Me.btnCancel.TabIndex = 11
        Me.btnCancel.Text = "إلغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmCutting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 753)
        Me.Controls.Add(Me.tabCutting)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmCutting"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmCutting"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.pnlProductSpecs.ResumeLayout(False)
        Me.pnlProductSpecs.PerformLayout()
        CType(Me.dgvOutPut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOutPutsCalculations.ResumeLayout(False)
        Me.pnlOutPutsCalculations.PerformLayout()
        Me.tabCutting.ResumeLayout(False)
        Me.TabCuttingProcess.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvInPut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabCuttingFollow.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents pnlProductSpecs As Panel
    Friend WithEvents Button5 As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnNew As Button
    Friend WithEvents pnlOutPutsCalculations As Panel
    Friend WithEvents tabCutting As TabControl
    Friend WithEvents TabCuttingProcess As TabPage
    Friend WithEvents tabCuttingFollow As TabPage
    Friend WithEvents txtEnglishName As TextBox
    Friend WithEvents lbl2 As Label
    Friend WithEvents btnFind As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents cboProductCode As ComboBox
    Friend WithEvents txtProductCategory As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtProductGroup As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents txtProductNote As TextBox
    Friend WithEvents cboCuttingCode As ComboBox
    Friend WithEvents dtpCuttingDate As DateTimePicker
    Friend WithEvents txtProductColor As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents txtProductMixType As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents txtProductType As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents dgvOutPut As DataGridView
    Friend WithEvents txtAvailableQTY As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents TotalPcsOutPut As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents txtTotalVolumeOutPut As TextBox
    Friend WithEvents Label17 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label22 As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents cboSourceStore As ComboBox
    Friend WithEvents txtStoreID As Label
    Friend WithEvents btnSend As Button
    Friend WithEvents cboTargetedStore As ComboBox
    Friend WithEvents Label25 As Label
    Friend WithEvents txtCuttingStatus As TextBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvInPut As DataGridView
    Friend WithEvents Button1 As Button
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colOutProductTypeID As DataGridViewTextBoxColumn
    Friend WithEvents colOutLength As DataGridViewTextBoxColumn
    Friend WithEvents colOutWidth As DataGridViewTextBoxColumn
    Friend WithEvents colOutHeight As DataGridViewTextBoxColumn
    Friend WithEvents colOutQTY As DataGridViewTextBoxColumn
    Friend WithEvents colOutProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colOutProductType As DataGridViewTextBoxColumn
    Friend WithEvents colOutPieceVolume As DataGridViewTextBoxColumn
    Friend WithEvents colOutTotalVolume As DataGridViewTextBoxColumn
    Friend WithEvents colDelete As DataGridViewButtonColumn
    Friend WithEvents colLength As DataGridViewTextBoxColumn
    Friend WithEvents colWidth As DataGridViewTextBoxColumn
    Friend WithEvents colHeight As DataGridViewTextBoxColumn
    Friend WithEvents colQTY As DataGridViewTextBoxColumn
    Friend WithEvents colProductType As DataGridViewComboBoxColumn
    Friend WithEvents colAdd As DataGridViewButtonColumn
    Friend WithEvents btnCancel As Button
End Class
