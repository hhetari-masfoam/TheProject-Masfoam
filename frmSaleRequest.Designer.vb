<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSaleRequest
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
        Me.pnlgrv = New System.Windows.Forms.Panel()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvSRInputs = New System.Windows.Forms.DataGridView()
        Me.colBaseProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWidth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colHeight = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSellUnit = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colM3SellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colPieceSellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.coltotalAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDiscount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDiscountAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colAmountAfterDiscount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colInputTaxID = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colInputVATAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colInputTotalAfterVAT = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colAdd = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.dgvSRDetails = New System.Windows.Forms.DataGridView()
        Me.colDetBaseProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDetLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetWidth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetHeight = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetSellUnit = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDetVolumePerUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetTotalVolume = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetPieceSellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetM3SellPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetAmountBeforeDiscount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetTotalAmountBFVAT = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetDiscount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetDiscountAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetVAT = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDetVATAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetTotalAmount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlBottons = New System.Windows.Forms.Panel()
        Me.btnCloseSR = New System.Windows.Forms.Button()
        Me.btnSRPrint = New System.Windows.Forms.Button()
        Me.btnSRDelete = New System.Windows.Forms.Button()
        Me.btnSRSearch = New System.Windows.Forms.Button()
        Me.btnSaveSR = New System.Windows.Forms.Button()
        Me.btnNewSR = New System.Windows.Forms.Button()
        Me.lblIsActive = New System.Windows.Forms.Label()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.txtFulfillmentStatusID = New System.Windows.Forms.TextBox()
        Me.lblNote = New System.Windows.Forms.Label()
        Me.txtSRNote = New System.Windows.Forms.TextBox()
        Me.btnPatrnerFind = New System.Windows.Forms.Button()
        Me.cboStoreCode = New System.Windows.Forms.ComboBox()
        Me.txtPartnerDept = New System.Windows.Forms.TextBox()
        Me.txtPartnerPhone = New System.Windows.Forms.TextBox()
        Me.btnFindSRep = New System.Windows.Forms.Button()
        Me.txtPartnerName = New System.Windows.Forms.TextBox()
        Me.lblStoreCode = New System.Windows.Forms.Label()
        Me.lblPartnerDept = New System.Windows.Forms.Label()
        Me.lblSPInformation = New System.Windows.Forms.Label()
        Me.lblPartnerPhone = New System.Windows.Forms.Label()
        Me.lblPartnerName = New System.Windows.Forms.Label()
        Me.txtSRepName = New System.Windows.Forms.TextBox()
        Me.lblPartnerCode = New System.Windows.Forms.Label()
        Me.lblEDT = New System.Windows.Forms.Label()
        Me.cboPartnerCode = New System.Windows.Forms.ComboBox()
        Me.cboSRepCode = New System.Windows.Forms.ComboBox()
        Me.lblPartnerInformation = New System.Windows.Forms.Label()
        Me.lblSRStatus = New System.Windows.Forms.Label()
        Me.lblSPcode = New System.Windows.Forms.Label()
        Me.lblSRDate = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblSPName = New System.Windows.Forms.Label()
        Me.lblSRCode = New System.Windows.Forms.Label()
        Me.dtpSRDeliveryDate = New System.Windows.Forms.DateTimePicker()
        Me.lblSRInformation = New System.Windows.Forms.Label()
        Me.txtSRCode = New System.Windows.Forms.TextBox()
        Me.txtSRStatus = New System.Windows.Forms.TextBox()
        Me.dtpSRDate = New System.Windows.Forms.DateTimePicker()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.txtTotalVAT = New System.Windows.Forms.TextBox()
        Me.txtTotalSRAmountBFVAT = New System.Windows.Forms.TextBox()
        Me.txtTotalSRVolume = New System.Windows.Forms.TextBox()
        Me.txtTotalSRAmount = New System.Windows.Forms.TextBox()
        Me.lblTotalVat = New System.Windows.Forms.Label()
        Me.lblTotalBVat = New System.Windows.Forms.Label()
        Me.lblTotalVolume = New System.Windows.Forms.Label()
        Me.lblTotalAmount = New System.Windows.Forms.Label()
        Me.pnlMainSR = New System.Windows.Forms.Panel()
        Me.pnlgrv.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvSRInputs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvSRDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBottons.SuspendLayout()
        Me.pnlHeader.SuspendLayout()
        Me.pnlBottom.SuspendLayout()
        Me.pnlMainSR.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlgrv
        '
        Me.pnlgrv.Controls.Add(Me.SplitContainer1)
        Me.pnlgrv.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlgrv.Location = New System.Drawing.Point(0, 237)
        Me.pnlgrv.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlgrv.Name = "pnlgrv"
        Me.pnlgrv.Size = New System.Drawing.Size(1472, 634)
        Me.pnlgrv.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvSRInputs)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.dgvSRDetails)
        Me.SplitContainer1.Size = New System.Drawing.Size(1472, 634)
        Me.SplitContainer1.SplitterDistance = 119
        Me.SplitContainer1.TabIndex = 0
        '
        'dgvSRInputs
        '
        Me.dgvSRInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSRInputs.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colBaseProductCode, Me.colType, Me.colLength, Me.colWidth, Me.colHeight, Me.colQty, Me.colSellUnit, Me.colM3SellPrice, Me.colPieceSellPrice, Me.coltotalAmount, Me.colDiscount, Me.colDiscountAmount, Me.colAmountAfterDiscount, Me.colInputTaxID, Me.colInputVATAmount, Me.colInputTotalAfterVAT, Me.colNote, Me.colAdd, Me.colDelete})
        Me.dgvSRInputs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSRInputs.Location = New System.Drawing.Point(0, 0)
        Me.dgvSRInputs.Name = "dgvSRInputs"
        Me.dgvSRInputs.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSRInputs.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvSRInputs.RowHeadersVisible = False
        Me.dgvSRInputs.RowHeadersWidth = 51
        Me.dgvSRInputs.RowTemplate.Height = 26
        Me.dgvSRInputs.Size = New System.Drawing.Size(1472, 119)
        Me.dgvSRInputs.TabIndex = 2
        '
        'colBaseProductCode
        '
        Me.colBaseProductCode.DataPropertyName = "BaseProductID"
        Me.colBaseProductCode.HeaderText = "الصنف الاساس"
        Me.colBaseProductCode.MinimumWidth = 6
        Me.colBaseProductCode.Name = "colBaseProductCode"
        Me.colBaseProductCode.Width = 125
        '
        'colType
        '
        Me.colType.HeaderText = "النوع"
        Me.colType.MinimumWidth = 6
        Me.colType.Name = "colType"
        Me.colType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.colType.Width = 125
        '
        'colLength
        '
        Me.colLength.HeaderText = "طول"
        Me.colLength.MinimumWidth = 6
        Me.colLength.Name = "colLength"
        Me.colLength.Width = 125
        '
        'colWidth
        '
        Me.colWidth.HeaderText = "عرض"
        Me.colWidth.MinimumWidth = 6
        Me.colWidth.Name = "colWidth"
        Me.colWidth.Width = 125
        '
        'colHeight
        '
        Me.colHeight.HeaderText = "ارتفاع"
        Me.colHeight.MinimumWidth = 6
        Me.colHeight.Name = "colHeight"
        Me.colHeight.Width = 125
        '
        'colQty
        '
        Me.colQty.HeaderText = "العدد"
        Me.colQty.MinimumWidth = 6
        Me.colQty.Name = "colQty"
        Me.colQty.Width = 125
        '
        'colSellUnit
        '
        Me.colSellUnit.HeaderText = "وحدة البيع"
        Me.colSellUnit.MinimumWidth = 6
        Me.colSellUnit.Name = "colSellUnit"
        Me.colSellUnit.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colSellUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.colSellUnit.Width = 125
        '
        'colM3SellPrice
        '
        Me.colM3SellPrice.HeaderText = "سعر المتر المكعب"
        Me.colM3SellPrice.MinimumWidth = 6
        Me.colM3SellPrice.Name = "colM3SellPrice"
        Me.colM3SellPrice.Width = 125
        '
        'colPieceSellPrice
        '
        Me.colPieceSellPrice.HeaderText = "سعر الوحدة"
        Me.colPieceSellPrice.MinimumWidth = 6
        Me.colPieceSellPrice.Name = "colPieceSellPrice"
        Me.colPieceSellPrice.Width = 125
        '
        'coltotalAmount
        '
        Me.coltotalAmount.HeaderText = "اجمالي المبلغ "
        Me.coltotalAmount.MinimumWidth = 6
        Me.coltotalAmount.Name = "coltotalAmount"
        Me.coltotalAmount.Width = 125
        '
        'colDiscount
        '
        Me.colDiscount.HeaderText = "الخصم"
        Me.colDiscount.MinimumWidth = 6
        Me.colDiscount.Name = "colDiscount"
        Me.colDiscount.Width = 125
        '
        'colDiscountAmount
        '
        Me.colDiscountAmount.HeaderText = "مبلغ الخصم"
        Me.colDiscountAmount.MinimumWidth = 6
        Me.colDiscountAmount.Name = "colDiscountAmount"
        Me.colDiscountAmount.Width = 125
        '
        'colAmountAfterDiscount
        '
        Me.colAmountAfterDiscount.HeaderText = "اجمالي بعد الخصم"
        Me.colAmountAfterDiscount.MinimumWidth = 6
        Me.colAmountAfterDiscount.Name = "colAmountAfterDiscount"
        Me.colAmountAfterDiscount.Width = 125
        '
        'colInputTaxID
        '
        Me.colInputTaxID.HeaderText = "الضريبة"
        Me.colInputTaxID.MinimumWidth = 6
        Me.colInputTaxID.Name = "colInputTaxID"
        Me.colInputTaxID.Width = 125
        '
        'colInputVATAmount
        '
        Me.colInputVATAmount.HeaderText = "مبلغ الضريبة"
        Me.colInputVATAmount.MinimumWidth = 6
        Me.colInputVATAmount.Name = "colInputVATAmount"
        Me.colInputVATAmount.Width = 125
        '
        'colInputTotalAfterVAT
        '
        Me.colInputTotalAfterVAT.HeaderText = "الاجمالي"
        Me.colInputTotalAfterVAT.MinimumWidth = 6
        Me.colInputTotalAfterVAT.Name = "colInputTotalAfterVAT"
        Me.colInputTotalAfterVAT.Width = 125
        '
        'colNote
        '
        Me.colNote.HeaderText = "ملاحظات"
        Me.colNote.MinimumWidth = 6
        Me.colNote.Name = "colNote"
        Me.colNote.Width = 125
        '
        'colAdd
        '
        Me.colAdd.HeaderText = ""
        Me.colAdd.MinimumWidth = 6
        Me.colAdd.Name = "colAdd"
        Me.colAdd.Text = "Add"
        Me.colAdd.UseColumnTextForButtonValue = True
        Me.colAdd.Width = 125
        '
        'colDelete
        '
        Me.colDelete.HeaderText = ""
        Me.colDelete.MinimumWidth = 6
        Me.colDelete.Name = "colDelete"
        Me.colDelete.Text = "Delete"
        Me.colDelete.UseColumnTextForButtonValue = True
        Me.colDelete.Width = 125
        '
        'dgvSRDetails
        '
        Me.dgvSRDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSRDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetBaseProductCode, Me.colDetProductID, Me.colDetProductCode, Me.colDetType, Me.colDetLength, Me.colDetWidth, Me.colDetHeight, Me.colDetQty, Me.colDetSellUnit, Me.colDetVolumePerUnit, Me.colDetTotalVolume, Me.colDetPieceSellPrice, Me.colDetM3SellPrice, Me.colDetAmountBeforeDiscount, Me.colDetTotalAmountBFVAT, Me.colDetDiscount, Me.colDetDiscountAmount, Me.colDetVAT, Me.colDetVATAmount, Me.colDetTotalAmount, Me.colDetNote})
        Me.dgvSRDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSRDetails.Location = New System.Drawing.Point(0, 0)
        Me.dgvSRDetails.Name = "dgvSRDetails"
        Me.dgvSRDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSRDetails.RowHeadersVisible = False
        Me.dgvSRDetails.RowHeadersWidth = 51
        Me.dgvSRDetails.RowTemplate.Height = 26
        Me.dgvSRDetails.Size = New System.Drawing.Size(1472, 511)
        Me.dgvSRDetails.TabIndex = 2
        '
        'colDetBaseProductCode
        '
        Me.colDetBaseProductCode.HeaderText = ""
        Me.colDetBaseProductCode.MinimumWidth = 6
        Me.colDetBaseProductCode.Name = "colDetBaseProductCode"
        Me.colDetBaseProductCode.Visible = False
        Me.colDetBaseProductCode.Width = 125
        '
        'colDetProductID
        '
        Me.colDetProductID.HeaderText = "رقم الصنف"
        Me.colDetProductID.MinimumWidth = 6
        Me.colDetProductID.Name = "colDetProductID"
        Me.colDetProductID.Visible = False
        Me.colDetProductID.Width = 125
        '
        'colDetProductCode
        '
        Me.colDetProductCode.HeaderText = "كود الصنف"
        Me.colDetProductCode.MinimumWidth = 6
        Me.colDetProductCode.Name = "colDetProductCode"
        Me.colDetProductCode.Width = 125
        '
        'colDetType
        '
        Me.colDetType.HeaderText = "النوع"
        Me.colDetType.MinimumWidth = 6
        Me.colDetType.Name = "colDetType"
        Me.colDetType.Width = 125
        '
        'colDetLength
        '
        Me.colDetLength.HeaderText = "الطول"
        Me.colDetLength.MinimumWidth = 6
        Me.colDetLength.Name = "colDetLength"
        Me.colDetLength.Width = 125
        '
        'colDetWidth
        '
        Me.colDetWidth.HeaderText = "العرض"
        Me.colDetWidth.MinimumWidth = 6
        Me.colDetWidth.Name = "colDetWidth"
        Me.colDetWidth.Width = 125
        '
        'colDetHeight
        '
        Me.colDetHeight.HeaderText = "الارتفاع"
        Me.colDetHeight.MinimumWidth = 6
        Me.colDetHeight.Name = "colDetHeight"
        Me.colDetHeight.Width = 125
        '
        'colDetQty
        '
        Me.colDetQty.HeaderText = "الكمية"
        Me.colDetQty.MinimumWidth = 6
        Me.colDetQty.Name = "colDetQty"
        Me.colDetQty.Width = 125
        '
        'colDetSellUnit
        '
        Me.colDetSellUnit.HeaderText = "الوحدة"
        Me.colDetSellUnit.MinimumWidth = 6
        Me.colDetSellUnit.Name = "colDetSellUnit"
        Me.colDetSellUnit.Width = 125
        '
        'colDetVolumePerUnit
        '
        Me.colDetVolumePerUnit.HeaderText = "حجم الوحدة"
        Me.colDetVolumePerUnit.MinimumWidth = 6
        Me.colDetVolumePerUnit.Name = "colDetVolumePerUnit"
        Me.colDetVolumePerUnit.Width = 125
        '
        'colDetTotalVolume
        '
        Me.colDetTotalVolume.HeaderText = "الحجم الاجمالي"
        Me.colDetTotalVolume.MinimumWidth = 6
        Me.colDetTotalVolume.Name = "colDetTotalVolume"
        Me.colDetTotalVolume.Width = 125
        '
        'colDetPieceSellPrice
        '
        Me.colDetPieceSellPrice.HeaderText = "سعر الوحدة"
        Me.colDetPieceSellPrice.MinimumWidth = 6
        Me.colDetPieceSellPrice.Name = "colDetPieceSellPrice"
        Me.colDetPieceSellPrice.Width = 125
        '
        'colDetM3SellPrice
        '
        Me.colDetM3SellPrice.HeaderText = "سعر المتر المكعب"
        Me.colDetM3SellPrice.MinimumWidth = 6
        Me.colDetM3SellPrice.Name = "colDetM3SellPrice"
        Me.colDetM3SellPrice.Width = 125
        '
        'colDetAmountBeforeDiscount
        '
        Me.colDetAmountBeforeDiscount.HeaderText = "المبلغ الاجمالي"
        Me.colDetAmountBeforeDiscount.MinimumWidth = 6
        Me.colDetAmountBeforeDiscount.Name = "colDetAmountBeforeDiscount"
        Me.colDetAmountBeforeDiscount.Width = 125
        '
        'colDetTotalAmountBFVAT
        '
        Me.colDetTotalAmountBFVAT.HeaderText = "الاجمالي بعد الخصم"
        Me.colDetTotalAmountBFVAT.MinimumWidth = 6
        Me.colDetTotalAmountBFVAT.Name = "colDetTotalAmountBFVAT"
        Me.colDetTotalAmountBFVAT.Width = 125
        '
        'colDetDiscount
        '
        Me.colDetDiscount.HeaderText = "الخصم%"
        Me.colDetDiscount.MinimumWidth = 6
        Me.colDetDiscount.Name = "colDetDiscount"
        Me.colDetDiscount.Width = 125
        '
        'colDetDiscountAmount
        '
        Me.colDetDiscountAmount.HeaderText = "مبلغ الخصم"
        Me.colDetDiscountAmount.MinimumWidth = 6
        Me.colDetDiscountAmount.Name = "colDetDiscountAmount"
        Me.colDetDiscountAmount.Width = 125
        '
        'colDetVAT
        '
        Me.colDetVAT.HeaderText = "الضريبة"
        Me.colDetVAT.MinimumWidth = 6
        Me.colDetVAT.Name = "colDetVAT"
        Me.colDetVAT.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colDetVAT.Width = 125
        '
        'colDetVATAmount
        '
        Me.colDetVATAmount.HeaderText = "مبلغ الضريبة"
        Me.colDetVATAmount.MinimumWidth = 6
        Me.colDetVATAmount.Name = "colDetVATAmount"
        Me.colDetVATAmount.Width = 125
        '
        'colDetTotalAmount
        '
        Me.colDetTotalAmount.HeaderText = "اجمالي المبلغ"
        Me.colDetTotalAmount.MinimumWidth = 6
        Me.colDetTotalAmount.Name = "colDetTotalAmount"
        Me.colDetTotalAmount.Width = 125
        '
        'colDetNote
        '
        Me.colDetNote.HeaderText = "ملاحظات"
        Me.colDetNote.MinimumWidth = 6
        Me.colDetNote.Name = "colDetNote"
        Me.colDetNote.Width = 125
        '
        'pnlBottons
        '
        Me.pnlBottons.BackColor = System.Drawing.Color.Transparent
        Me.pnlBottons.Controls.Add(Me.btnCloseSR)
        Me.pnlBottons.Controls.Add(Me.btnSRPrint)
        Me.pnlBottons.Controls.Add(Me.btnSRDelete)
        Me.pnlBottons.Controls.Add(Me.btnSRSearch)
        Me.pnlBottons.Controls.Add(Me.btnSaveSR)
        Me.pnlBottons.Controls.Add(Me.btnNewSR)
        Me.pnlBottons.Controls.Add(Me.lblIsActive)
        Me.pnlBottons.Controls.Add(Me.chkIsActive)
        Me.pnlBottons.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlBottons.Location = New System.Drawing.Point(1472, 0)
        Me.pnlBottons.Name = "pnlBottons"
        Me.pnlBottons.Size = New System.Drawing.Size(121, 871)
        Me.pnlBottons.TabIndex = 0
        '
        'btnCloseSR
        '
        Me.btnCloseSR.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCloseSR.BackColor = System.Drawing.SystemColors.Control
        Me.btnCloseSR.Location = New System.Drawing.Point(4, 807)
        Me.btnCloseSR.Name = "btnCloseSR"
        Me.btnCloseSR.Size = New System.Drawing.Size(112, 43)
        Me.btnCloseSR.TabIndex = 0
        Me.btnCloseSR.Text = "Close"
        Me.btnCloseSR.UseVisualStyleBackColor = False
        '
        'btnSRPrint
        '
        Me.btnSRPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSRPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnSRPrint.Location = New System.Drawing.Point(4, 758)
        Me.btnSRPrint.Name = "btnSRPrint"
        Me.btnSRPrint.Size = New System.Drawing.Size(112, 43)
        Me.btnSRPrint.TabIndex = 0
        Me.btnSRPrint.Text = "Print"
        Me.btnSRPrint.UseVisualStyleBackColor = False
        '
        'btnSRDelete
        '
        Me.btnSRDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSRDelete.BackColor = System.Drawing.SystemColors.Control
        Me.btnSRDelete.Location = New System.Drawing.Point(4, 709)
        Me.btnSRDelete.Name = "btnSRDelete"
        Me.btnSRDelete.Size = New System.Drawing.Size(112, 43)
        Me.btnSRDelete.TabIndex = 0
        Me.btnSRDelete.Text = "Delete"
        Me.btnSRDelete.UseVisualStyleBackColor = False
        '
        'btnSRSearch
        '
        Me.btnSRSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSRSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSRSearch.Location = New System.Drawing.Point(4, 660)
        Me.btnSRSearch.Name = "btnSRSearch"
        Me.btnSRSearch.Size = New System.Drawing.Size(112, 43)
        Me.btnSRSearch.TabIndex = 0
        Me.btnSRSearch.Text = "Search"
        Me.btnSRSearch.UseVisualStyleBackColor = False
        '
        'btnSaveSR
        '
        Me.btnSaveSR.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSaveSR.BackColor = System.Drawing.SystemColors.Control
        Me.btnSaveSR.Location = New System.Drawing.Point(4, 611)
        Me.btnSaveSR.Name = "btnSaveSR"
        Me.btnSaveSR.Size = New System.Drawing.Size(112, 43)
        Me.btnSaveSR.TabIndex = 0
        Me.btnSaveSR.Text = "Save"
        Me.btnSaveSR.UseVisualStyleBackColor = False
        '
        'btnNewSR
        '
        Me.btnNewSR.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnNewSR.BackColor = System.Drawing.SystemColors.Control
        Me.btnNewSR.Location = New System.Drawing.Point(4, 562)
        Me.btnNewSR.Name = "btnNewSR"
        Me.btnNewSR.Size = New System.Drawing.Size(112, 43)
        Me.btnNewSR.TabIndex = 0
        Me.btnNewSR.Text = "New"
        Me.btnNewSR.UseVisualStyleBackColor = False
        '
        'lblIsActive
        '
        Me.lblIsActive.AutoSize = True
        Me.lblIsActive.Location = New System.Drawing.Point(24, 24)
        Me.lblIsActive.Name = "lblIsActive"
        Me.lblIsActive.Size = New System.Drawing.Size(82, 18)
        Me.lblIsActive.TabIndex = 0
        Me.lblIsActive.Text = "الطلب نشط"
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Location = New System.Drawing.Point(64, 73)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsActive.TabIndex = 5
        Me.chkIsActive.UseVisualStyleBackColor = True
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.Transparent
        Me.pnlHeader.Controls.Add(Me.txtFulfillmentStatusID)
        Me.pnlHeader.Controls.Add(Me.lblNote)
        Me.pnlHeader.Controls.Add(Me.txtSRNote)
        Me.pnlHeader.Controls.Add(Me.btnPatrnerFind)
        Me.pnlHeader.Controls.Add(Me.cboStoreCode)
        Me.pnlHeader.Controls.Add(Me.txtPartnerDept)
        Me.pnlHeader.Controls.Add(Me.txtPartnerPhone)
        Me.pnlHeader.Controls.Add(Me.btnFindSRep)
        Me.pnlHeader.Controls.Add(Me.txtPartnerName)
        Me.pnlHeader.Controls.Add(Me.lblStoreCode)
        Me.pnlHeader.Controls.Add(Me.lblPartnerDept)
        Me.pnlHeader.Controls.Add(Me.lblSPInformation)
        Me.pnlHeader.Controls.Add(Me.lblPartnerPhone)
        Me.pnlHeader.Controls.Add(Me.lblPartnerName)
        Me.pnlHeader.Controls.Add(Me.txtSRepName)
        Me.pnlHeader.Controls.Add(Me.lblPartnerCode)
        Me.pnlHeader.Controls.Add(Me.lblEDT)
        Me.pnlHeader.Controls.Add(Me.cboPartnerCode)
        Me.pnlHeader.Controls.Add(Me.cboSRepCode)
        Me.pnlHeader.Controls.Add(Me.lblPartnerInformation)
        Me.pnlHeader.Controls.Add(Me.lblSRStatus)
        Me.pnlHeader.Controls.Add(Me.lblSPcode)
        Me.pnlHeader.Controls.Add(Me.lblSRDate)
        Me.pnlHeader.Controls.Add(Me.Label1)
        Me.pnlHeader.Controls.Add(Me.lblSPName)
        Me.pnlHeader.Controls.Add(Me.lblSRCode)
        Me.pnlHeader.Controls.Add(Me.dtpSRDeliveryDate)
        Me.pnlHeader.Controls.Add(Me.lblSRInformation)
        Me.pnlHeader.Controls.Add(Me.txtSRCode)
        Me.pnlHeader.Controls.Add(Me.txtSRStatus)
        Me.pnlHeader.Controls.Add(Me.dtpSRDate)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(1472, 237)
        Me.pnlHeader.TabIndex = 1
        '
        'txtFulfillmentStatusID
        '
        Me.txtFulfillmentStatusID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFulfillmentStatusID.Enabled = False
        Me.txtFulfillmentStatusID.Location = New System.Drawing.Point(1165, 195)
        Me.txtFulfillmentStatusID.Name = "txtFulfillmentStatusID"
        Me.txtFulfillmentStatusID.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtFulfillmentStatusID.Size = New System.Drawing.Size(184, 26)
        Me.txtFulfillmentStatusID.TabIndex = 7
        '
        'lblNote
        '
        Me.lblNote.AutoSize = True
        Me.lblNote.Location = New System.Drawing.Point(222, 9)
        Me.lblNote.Name = "lblNote"
        Me.lblNote.Size = New System.Drawing.Size(64, 18)
        Me.lblNote.TabIndex = 0
        Me.lblNote.Text = "ملاحظات"
        '
        'txtSRNote
        '
        Me.txtSRNote.Location = New System.Drawing.Point(14, 37)
        Me.txtSRNote.Multiline = True
        Me.txtSRNote.Name = "txtSRNote"
        Me.txtSRNote.Size = New System.Drawing.Size(278, 184)
        Me.txtSRNote.TabIndex = 1
        '
        'btnPatrnerFind
        '
        Me.btnPatrnerFind.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPatrnerFind.BackColor = System.Drawing.SystemColors.Control
        Me.btnPatrnerFind.Location = New System.Drawing.Point(851, 46)
        Me.btnPatrnerFind.Name = "btnPatrnerFind"
        Me.btnPatrnerFind.Size = New System.Drawing.Size(58, 30)
        Me.btnPatrnerFind.TabIndex = 6
        Me.btnPatrnerFind.Text = "Find"
        Me.btnPatrnerFind.UseVisualStyleBackColor = False
        '
        'cboStoreCode
        '
        Me.cboStoreCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboStoreCode.Enabled = False
        Me.cboStoreCode.FormattingEnabled = True
        Me.cboStoreCode.Location = New System.Drawing.Point(546, 195)
        Me.cboStoreCode.Name = "cboStoreCode"
        Me.cboStoreCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboStoreCode.Size = New System.Drawing.Size(171, 26)
        Me.cboStoreCode.TabIndex = 6
        '
        'txtPartnerDept
        '
        Me.txtPartnerDept.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPartnerDept.Enabled = False
        Me.txtPartnerDept.Location = New System.Drawing.Point(851, 158)
        Me.txtPartnerDept.Name = "txtPartnerDept"
        Me.txtPartnerDept.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtPartnerDept.Size = New System.Drawing.Size(187, 26)
        Me.txtPartnerDept.TabIndex = 5
        '
        'txtPartnerPhone
        '
        Me.txtPartnerPhone.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPartnerPhone.Enabled = False
        Me.txtPartnerPhone.Location = New System.Drawing.Point(851, 126)
        Me.txtPartnerPhone.Name = "txtPartnerPhone"
        Me.txtPartnerPhone.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtPartnerPhone.Size = New System.Drawing.Size(187, 26)
        Me.txtPartnerPhone.TabIndex = 4
        '
        'btnFindSRep
        '
        Me.btnFindSRep.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFindSRep.BackColor = System.Drawing.SystemColors.Control
        Me.btnFindSRep.Location = New System.Drawing.Point(546, 45)
        Me.btnFindSRep.Name = "btnFindSRep"
        Me.btnFindSRep.Size = New System.Drawing.Size(58, 30)
        Me.btnFindSRep.TabIndex = 6
        Me.btnFindSRep.Text = "Find"
        Me.btnFindSRep.UseVisualStyleBackColor = False
        '
        'txtPartnerName
        '
        Me.txtPartnerName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPartnerName.Enabled = False
        Me.txtPartnerName.Location = New System.Drawing.Point(851, 84)
        Me.txtPartnerName.Name = "txtPartnerName"
        Me.txtPartnerName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtPartnerName.Size = New System.Drawing.Size(187, 26)
        Me.txtPartnerName.TabIndex = 3
        '
        'lblStoreCode
        '
        Me.lblStoreCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStoreCode.AutoSize = True
        Me.lblStoreCode.Location = New System.Drawing.Point(739, 203)
        Me.lblStoreCode.Name = "lblStoreCode"
        Me.lblStoreCode.Size = New System.Drawing.Size(95, 18)
        Me.lblStoreCode.TabIndex = 0
        Me.lblStoreCode.Text = "كود المستودع"
        '
        'lblPartnerDept
        '
        Me.lblPartnerDept.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPartnerDept.AutoSize = True
        Me.lblPartnerDept.Location = New System.Drawing.Point(1060, 167)
        Me.lblPartnerDept.Name = "lblPartnerDept"
        Me.lblPartnerDept.Size = New System.Drawing.Size(83, 18)
        Me.lblPartnerDept.TabIndex = 0
        Me.lblPartnerDept.Text = "رصيد العميل"
        '
        'lblSPInformation
        '
        Me.lblSPInformation.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSPInformation.AutoSize = True
        Me.lblSPInformation.Location = New System.Drawing.Point(722, 19)
        Me.lblSPInformation.Name = "lblSPInformation"
        Me.lblSPInformation.Size = New System.Drawing.Size(112, 18)
        Me.lblSPInformation.TabIndex = 0
        Me.lblSPInformation.Text = "معلومات المندوب"
        '
        'lblPartnerPhone
        '
        Me.lblPartnerPhone.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPartnerPhone.AutoSize = True
        Me.lblPartnerPhone.Location = New System.Drawing.Point(1069, 130)
        Me.lblPartnerPhone.Name = "lblPartnerPhone"
        Me.lblPartnerPhone.Size = New System.Drawing.Size(74, 18)
        Me.lblPartnerPhone.TabIndex = 0
        Me.lblPartnerPhone.Text = "رقم العميل"
        '
        'lblPartnerName
        '
        Me.lblPartnerName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPartnerName.AutoSize = True
        Me.lblPartnerName.Location = New System.Drawing.Point(1061, 93)
        Me.lblPartnerName.Name = "lblPartnerName"
        Me.lblPartnerName.Size = New System.Drawing.Size(82, 18)
        Me.lblPartnerName.TabIndex = 0
        Me.lblPartnerName.Text = "إسم العميل"
        '
        'txtSRepName
        '
        Me.txtSRepName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSRepName.Enabled = False
        Me.txtSRepName.Location = New System.Drawing.Point(546, 83)
        Me.txtSRepName.Name = "txtSRepName"
        Me.txtSRepName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtSRepName.Size = New System.Drawing.Size(171, 26)
        Me.txtSRepName.TabIndex = 3
        '
        'lblPartnerCode
        '
        Me.lblPartnerCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPartnerCode.AutoSize = True
        Me.lblPartnerCode.Location = New System.Drawing.Point(1069, 56)
        Me.lblPartnerCode.Name = "lblPartnerCode"
        Me.lblPartnerCode.Size = New System.Drawing.Size(74, 18)
        Me.lblPartnerCode.TabIndex = 0
        Me.lblPartnerCode.Text = "كود العميل"
        '
        'lblEDT
        '
        Me.lblEDT.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblEDT.AutoSize = True
        Me.lblEDT.Location = New System.Drawing.Point(1377, 166)
        Me.lblEDT.Name = "lblEDT"
        Me.lblEDT.Size = New System.Drawing.Size(90, 18)
        Me.lblEDT.TabIndex = 0
        Me.lblEDT.Text = "تاريخ التسليم"
        '
        'cboPartnerCode
        '
        Me.cboPartnerCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboPartnerCode.FormattingEnabled = True
        Me.cboPartnerCode.Location = New System.Drawing.Point(916, 47)
        Me.cboPartnerCode.Name = "cboPartnerCode"
        Me.cboPartnerCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboPartnerCode.Size = New System.Drawing.Size(122, 26)
        Me.cboPartnerCode.TabIndex = 2
        '
        'cboSRepCode
        '
        Me.cboSRepCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboSRepCode.FormattingEnabled = True
        Me.cboSRepCode.Location = New System.Drawing.Point(611, 46)
        Me.cboSRepCode.Name = "cboSRepCode"
        Me.cboSRepCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboSRepCode.Size = New System.Drawing.Size(106, 26)
        Me.cboSRepCode.TabIndex = 2
        '
        'lblPartnerInformation
        '
        Me.lblPartnerInformation.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPartnerInformation.AutoSize = True
        Me.lblPartnerInformation.Location = New System.Drawing.Point(1039, 23)
        Me.lblPartnerInformation.Name = "lblPartnerInformation"
        Me.lblPartnerInformation.Size = New System.Drawing.Size(104, 18)
        Me.lblPartnerInformation.TabIndex = 0
        Me.lblPartnerInformation.Text = "معلومات العميل"
        '
        'lblSRStatus
        '
        Me.lblSRStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSRStatus.AutoSize = True
        Me.lblSRStatus.Location = New System.Drawing.Point(1389, 129)
        Me.lblSRStatus.Name = "lblSRStatus"
        Me.lblSRStatus.Size = New System.Drawing.Size(78, 18)
        Me.lblSRStatus.TabIndex = 0
        Me.lblSRStatus.Text = "حالة الطلب"
        '
        'lblSPcode
        '
        Me.lblSPcode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSPcode.AutoSize = True
        Me.lblSPcode.Location = New System.Drawing.Point(752, 52)
        Me.lblSPcode.Name = "lblSPcode"
        Me.lblSPcode.Size = New System.Drawing.Size(82, 18)
        Me.lblSPcode.TabIndex = 0
        Me.lblSPcode.Text = "كود المندوب"
        '
        'lblSRDate
        '
        Me.lblSRDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSRDate.AutoSize = True
        Me.lblSRDate.Location = New System.Drawing.Point(1383, 92)
        Me.lblSRDate.Name = "lblSRDate"
        Me.lblSRDate.Size = New System.Drawing.Size(84, 18)
        Me.lblSRDate.TabIndex = 0
        Me.lblSRDate.Text = "تاريخ الانشاء"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1387, 203)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 18)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "حالة التنفيذ"
        '
        'lblSPName
        '
        Me.lblSPName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSPName.AutoSize = True
        Me.lblSPName.Location = New System.Drawing.Point(744, 89)
        Me.lblSPName.Name = "lblSPName"
        Me.lblSPName.Size = New System.Drawing.Size(90, 18)
        Me.lblSPName.TabIndex = 0
        Me.lblSPName.Text = "إسم المندوب"
        '
        'lblSRCode
        '
        Me.lblSRCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSRCode.AutoSize = True
        Me.lblSRCode.Location = New System.Drawing.Point(1395, 55)
        Me.lblSRCode.Name = "lblSRCode"
        Me.lblSRCode.Size = New System.Drawing.Size(72, 18)
        Me.lblSRCode.TabIndex = 0
        Me.lblSRCode.Text = "كود الطلب"
        '
        'dtpSRDeliveryDate
        '
        Me.dtpSRDeliveryDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpSRDeliveryDate.CustomFormat = "dd-MM-yyyy"
        Me.dtpSRDeliveryDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpSRDeliveryDate.Location = New System.Drawing.Point(1165, 157)
        Me.dtpSRDeliveryDate.Name = "dtpSRDeliveryDate"
        Me.dtpSRDeliveryDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dtpSRDeliveryDate.RightToLeftLayout = True
        Me.dtpSRDeliveryDate.Size = New System.Drawing.Size(184, 26)
        Me.dtpSRDeliveryDate.TabIndex = 4
        '
        'lblSRInformation
        '
        Me.lblSRInformation.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSRInformation.AutoSize = True
        Me.lblSRInformation.Location = New System.Drawing.Point(1365, 22)
        Me.lblSRInformation.Name = "lblSRInformation"
        Me.lblSRInformation.Size = New System.Drawing.Size(102, 18)
        Me.lblSRInformation.TabIndex = 0
        Me.lblSRInformation.Text = "معلومات الطلب"
        '
        'txtSRCode
        '
        Me.txtSRCode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSRCode.Enabled = False
        Me.txtSRCode.Location = New System.Drawing.Point(1164, 46)
        Me.txtSRCode.Name = "txtSRCode"
        Me.txtSRCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtSRCode.Size = New System.Drawing.Size(185, 26)
        Me.txtSRCode.TabIndex = 1
        '
        'txtSRStatus
        '
        Me.txtSRStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSRStatus.Enabled = False
        Me.txtSRStatus.Location = New System.Drawing.Point(1165, 125)
        Me.txtSRStatus.Name = "txtSRStatus"
        Me.txtSRStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtSRStatus.Size = New System.Drawing.Size(184, 26)
        Me.txtSRStatus.TabIndex = 3
        '
        'dtpSRDate
        '
        Me.dtpSRDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpSRDate.CustomFormat = "dd-MM-yyyy"
        Me.dtpSRDate.Enabled = False
        Me.dtpSRDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpSRDate.Location = New System.Drawing.Point(1165, 83)
        Me.dtpSRDate.Name = "dtpSRDate"
        Me.dtpSRDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dtpSRDate.RightToLeftLayout = True
        Me.dtpSRDate.Size = New System.Drawing.Size(184, 26)
        Me.dtpSRDate.TabIndex = 2
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.txtTotalVAT)
        Me.pnlBottom.Controls.Add(Me.txtTotalSRAmountBFVAT)
        Me.pnlBottom.Controls.Add(Me.txtTotalSRVolume)
        Me.pnlBottom.Controls.Add(Me.txtTotalSRAmount)
        Me.pnlBottom.Controls.Add(Me.lblTotalVat)
        Me.pnlBottom.Controls.Add(Me.lblTotalBVat)
        Me.pnlBottom.Controls.Add(Me.lblTotalVolume)
        Me.pnlBottom.Controls.Add(Me.lblTotalAmount)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 871)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(1593, 121)
        Me.pnlBottom.TabIndex = 3
        '
        'txtTotalVAT
        '
        Me.txtTotalVAT.Location = New System.Drawing.Point(158, 65)
        Me.txtTotalVAT.Name = "txtTotalVAT"
        Me.txtTotalVAT.ReadOnly = True
        Me.txtTotalVAT.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtTotalVAT.Size = New System.Drawing.Size(151, 26)
        Me.txtTotalVAT.TabIndex = 4
        '
        'txtTotalSRAmountBFVAT
        '
        Me.txtTotalSRAmountBFVAT.Location = New System.Drawing.Point(158, 39)
        Me.txtTotalSRAmountBFVAT.Name = "txtTotalSRAmountBFVAT"
        Me.txtTotalSRAmountBFVAT.ReadOnly = True
        Me.txtTotalSRAmountBFVAT.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtTotalSRAmountBFVAT.Size = New System.Drawing.Size(151, 26)
        Me.txtTotalSRAmountBFVAT.TabIndex = 4
        '
        'txtTotalSRVolume
        '
        Me.txtTotalSRVolume.Location = New System.Drawing.Point(158, 13)
        Me.txtTotalSRVolume.Name = "txtTotalSRVolume"
        Me.txtTotalSRVolume.ReadOnly = True
        Me.txtTotalSRVolume.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtTotalSRVolume.Size = New System.Drawing.Size(151, 26)
        Me.txtTotalSRVolume.TabIndex = 4
        '
        'txtTotalSRAmount
        '
        Me.txtTotalSRAmount.Location = New System.Drawing.Point(158, 91)
        Me.txtTotalSRAmount.Name = "txtTotalSRAmount"
        Me.txtTotalSRAmount.ReadOnly = True
        Me.txtTotalSRAmount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtTotalSRAmount.Size = New System.Drawing.Size(151, 26)
        Me.txtTotalSRAmount.TabIndex = 4
        '
        'lblTotalVat
        '
        Me.lblTotalVat.AutoSize = True
        Me.lblTotalVat.Location = New System.Drawing.Point(373, 69)
        Me.lblTotalVat.Name = "lblTotalVat"
        Me.lblTotalVat.Size = New System.Drawing.Size(104, 18)
        Me.lblTotalVat.TabIndex = 5
        Me.lblTotalVat.Text = "اجمالي الضريبة"
        '
        'lblTotalBVat
        '
        Me.lblTotalBVat.AutoSize = True
        Me.lblTotalBVat.Location = New System.Drawing.Point(337, 43)
        Me.lblTotalBVat.Name = "lblTotalBVat"
        Me.lblTotalBVat.Size = New System.Drawing.Size(140, 18)
        Me.lblTotalBVat.TabIndex = 5
        Me.lblTotalBVat.Text = "الاجمالي قبل الضريبة"
        '
        'lblTotalVolume
        '
        Me.lblTotalVolume.AutoSize = True
        Me.lblTotalVolume.Location = New System.Drawing.Point(371, 17)
        Me.lblTotalVolume.Name = "lblTotalVolume"
        Me.lblTotalVolume.Size = New System.Drawing.Size(106, 18)
        Me.lblTotalVolume.TabIndex = 5
        Me.lblTotalVolume.Text = "الحجم الاجمالي"
        '
        'lblTotalAmount
        '
        Me.lblTotalAmount.AutoSize = True
        Me.lblTotalAmount.Location = New System.Drawing.Point(343, 95)
        Me.lblTotalAmount.Name = "lblTotalAmount"
        Me.lblTotalAmount.Size = New System.Drawing.Size(134, 18)
        Me.lblTotalAmount.TabIndex = 5
        Me.lblTotalAmount.Text = "اجمالي مبلغ الفاتورة"
        '
        'pnlMainSR
        '
        Me.pnlMainSR.Controls.Add(Me.pnlgrv)
        Me.pnlMainSR.Controls.Add(Me.pnlHeader)
        Me.pnlMainSR.Controls.Add(Me.pnlBottons)
        Me.pnlMainSR.Controls.Add(Me.pnlBottom)
        Me.pnlMainSR.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMainSR.Location = New System.Drawing.Point(0, 0)
        Me.pnlMainSR.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlMainSR.Name = "pnlMainSR"
        Me.pnlMainSR.Size = New System.Drawing.Size(1593, 992)
        Me.pnlMainSR.TabIndex = 7
        '
        'frmSaleRequest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1593, 992)
        Me.Controls.Add(Me.pnlMainSR)
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmSaleRequest"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "طلب المبيعات"
        Me.pnlgrv.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvSRInputs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvSRDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBottons.ResumeLayout(False)
        Me.pnlBottons.PerformLayout()
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.pnlBottom.ResumeLayout(False)
        Me.pnlBottom.PerformLayout()
        Me.pnlMainSR.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlgrv As Panel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvSRInputs As DataGridView
    Friend WithEvents dgvSRDetails As DataGridView
    Friend WithEvents pnlBottons As Panel
    Friend WithEvents btnCloseSR As Button
    Friend WithEvents btnSRPrint As Button
    Friend WithEvents btnSRDelete As Button
    Friend WithEvents btnSRSearch As Button
    Friend WithEvents btnSaveSR As Button
    Friend WithEvents btnNewSR As Button
    Friend WithEvents lblIsActive As Label
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents lblNote As Label
    Friend WithEvents txtSRNote As TextBox
    Friend WithEvents btnPatrnerFind As Button
    Friend WithEvents cboStoreCode As ComboBox
    Friend WithEvents txtPartnerDept As TextBox
    Friend WithEvents txtPartnerPhone As TextBox
    Friend WithEvents btnFindSRep As Button
    Friend WithEvents txtPartnerName As TextBox
    Friend WithEvents lblStoreCode As Label
    Friend WithEvents lblPartnerDept As Label
    Friend WithEvents lblSPInformation As Label
    Friend WithEvents lblPartnerPhone As Label
    Friend WithEvents lblPartnerName As Label
    Friend WithEvents txtSRepName As TextBox
    Friend WithEvents lblPartnerCode As Label
    Friend WithEvents lblEDT As Label
    Friend WithEvents cboPartnerCode As ComboBox
    Friend WithEvents cboSRepCode As ComboBox
    Friend WithEvents lblPartnerInformation As Label
    Friend WithEvents lblSRStatus As Label
    Friend WithEvents lblSPcode As Label
    Friend WithEvents lblSRDate As Label
    Friend WithEvents lblSPName As Label
    Friend WithEvents lblSRCode As Label
    Friend WithEvents dtpSRDeliveryDate As DateTimePicker
    Friend WithEvents lblSRInformation As Label
    Friend WithEvents txtSRCode As TextBox
    Friend WithEvents txtSRStatus As TextBox
    Friend WithEvents dtpSRDate As DateTimePicker
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents lblTotalAmount As Label
    Friend WithEvents txtTotalVAT As TextBox
    Friend WithEvents txtTotalSRAmountBFVAT As TextBox
    Friend WithEvents txtTotalSRVolume As TextBox
    Friend WithEvents txtTotalSRAmount As TextBox
    Friend WithEvents lblTotalVat As Label
    Friend WithEvents lblTotalBVat As Label
    Friend WithEvents lblTotalVolume As Label
    Friend WithEvents pnlMainSR As Panel
    Friend WithEvents colBaseProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colType As DataGridViewComboBoxColumn
    Friend WithEvents colLength As DataGridViewTextBoxColumn
    Friend WithEvents colWidth As DataGridViewTextBoxColumn
    Friend WithEvents colHeight As DataGridViewTextBoxColumn
    Friend WithEvents colQty As DataGridViewTextBoxColumn
    Friend WithEvents colSellUnit As DataGridViewComboBoxColumn
    Friend WithEvents colM3SellPrice As DataGridViewTextBoxColumn
    Friend WithEvents colPieceSellPrice As DataGridViewTextBoxColumn
    Friend WithEvents coltotalAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDiscount As DataGridViewTextBoxColumn
    Friend WithEvents colDiscountAmount As DataGridViewTextBoxColumn
    Friend WithEvents colAmountAfterDiscount As DataGridViewTextBoxColumn
    Friend WithEvents colInputTaxID As DataGridViewComboBoxColumn
    Friend WithEvents colInputVATAmount As DataGridViewTextBoxColumn
    Friend WithEvents colInputTotalAfterVAT As DataGridViewTextBoxColumn
    Friend WithEvents colNote As DataGridViewTextBoxColumn
    Friend WithEvents colAdd As DataGridViewButtonColumn
    Friend WithEvents colDelete As DataGridViewButtonColumn
    Friend WithEvents txtFulfillmentStatusID As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents colDetBaseProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductID As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colDetType As DataGridViewComboBoxColumn
    Friend WithEvents colDetLength As DataGridViewTextBoxColumn
    Friend WithEvents colDetWidth As DataGridViewTextBoxColumn
    Friend WithEvents colDetHeight As DataGridViewTextBoxColumn
    Friend WithEvents colDetQty As DataGridViewTextBoxColumn
    Friend WithEvents colDetSellUnit As DataGridViewComboBoxColumn
    Friend WithEvents colDetVolumePerUnit As DataGridViewTextBoxColumn
    Friend WithEvents colDetTotalVolume As DataGridViewTextBoxColumn
    Friend WithEvents colDetPieceSellPrice As DataGridViewTextBoxColumn
    Friend WithEvents colDetM3SellPrice As DataGridViewTextBoxColumn
    Friend WithEvents colDetAmountBeforeDiscount As DataGridViewTextBoxColumn
    Friend WithEvents colDetTotalAmountBFVAT As DataGridViewTextBoxColumn
    Friend WithEvents colDetDiscount As DataGridViewTextBoxColumn
    Friend WithEvents colDetDiscountAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetVAT As DataGridViewComboBoxColumn
    Friend WithEvents colDetVATAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetTotalAmount As DataGridViewTextBoxColumn
    Friend WithEvents colDetNote As DataGridViewTextBoxColumn
End Class
