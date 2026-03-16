<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSRDistripution
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.dgvSR = New System.Windows.Forms.DataGridView()
        Me.colSRID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRPartnerID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRSaleRepCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnMoveSR = New System.Windows.Forms.Button()
        Me.btnSRPrint = New System.Windows.Forms.Button()
        Me.btnCloseDistribution = New System.Windows.Forms.Button()
        Me.dgvSRD = New System.Windows.Forms.DataGridView()
        Me.colSRDID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLOCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDBusinessStatusID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingStatusID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDBusinessStatusName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingStatusName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDProductType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDAvailableQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDLoadedQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSRDNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn26 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn27 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn28 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn29 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn30 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn31 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn32 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn33 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn34 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn35 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewComboBoxColumn1 = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.DataGridViewComboBoxColumn2 = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.DataGridViewComboBoxColumn3 = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.DataGridViewComboBoxColumn4 = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.DataGridViewTextBoxColumn36 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnOpenLoadingBoard = New System.Windows.Forms.Button()
        Me.btnCloseSR = New System.Windows.Forms.Button()
        Me.btnStartLoading = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lblPartener = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblSRCode = New System.Windows.Forms.Label()
        Me.tbcSRDistribution = New System.Windows.Forms.TabControl()
        Me.tabpSRNew = New System.Windows.Forms.TabPage()
        Me.tabpSRInCutting = New System.Windows.Forms.TabPage()
        Me.tabpSRInLoading = New System.Windows.Forms.TabPage()
        Me.tabpSROutLoading = New System.Windows.Forms.TabPage()
        Me.tabPartialSRs = New System.Windows.Forms.TabPage()
        Me.tabpSRWaitingInvoicesSRs = New System.Windows.Forms.TabPage()
        Me.tabInvoicedSRs = New System.Windows.Forms.TabPage()
        Me.tabReturnLO = New System.Windows.Forms.TabPage()
        Me.tabNewInvoice = New System.Windows.Forms.TabPage()
        Me.pnlFilters = New System.Windows.Forms.Panel()
        CType(Me.dgvSR, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvSRD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel3.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.tbcSRDistribution.SuspendLayout()
        Me.pnlFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvSR
        '
        Me.dgvSR.AllowUserToAddRows = False
        Me.dgvSR.AllowUserToDeleteRows = False
        Me.dgvSR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSR.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colSRID, Me.colSRCode, Me.colSRDate, Me.colSRStatus, Me.colSRPartnerID, Me.colSRSaleRepCode, Me.colSRNote})
        Me.dgvSR.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSR.Location = New System.Drawing.Point(0, 0)
        Me.dgvSR.MultiSelect = False
        Me.dgvSR.Name = "dgvSR"
        Me.dgvSR.ReadOnly = True
        Me.dgvSR.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSR.RowHeadersVisible = False
        Me.dgvSR.RowHeadersWidth = 51
        Me.dgvSR.RowTemplate.Height = 26
        Me.dgvSR.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSR.Size = New System.Drawing.Size(628, 635)
        Me.dgvSR.TabIndex = 0
        '
        'colSRID
        '
        Me.colSRID.HeaderText = "Column1"
        Me.colSRID.MinimumWidth = 6
        Me.colSRID.Name = "colSRID"
        Me.colSRID.ReadOnly = True
        Me.colSRID.Visible = False
        Me.colSRID.Width = 125
        '
        'colSRCode
        '
        Me.colSRCode.HeaderText = "كود طلب المبيعات"
        Me.colSRCode.MinimumWidth = 6
        Me.colSRCode.Name = "colSRCode"
        Me.colSRCode.ReadOnly = True
        Me.colSRCode.Width = 125
        '
        'colSRDate
        '
        DataGridViewCellStyle1.Format = "dd-MM-yyyy"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.colSRDate.DefaultCellStyle = DataGridViewCellStyle1
        Me.colSRDate.HeaderText = "تاريخ الانشاء"
        Me.colSRDate.MinimumWidth = 6
        Me.colSRDate.Name = "colSRDate"
        Me.colSRDate.ReadOnly = True
        Me.colSRDate.Width = 125
        '
        'colSRStatus
        '
        Me.colSRStatus.HeaderText = "الحالة"
        Me.colSRStatus.MinimumWidth = 6
        Me.colSRStatus.Name = "colSRStatus"
        Me.colSRStatus.ReadOnly = True
        Me.colSRStatus.Visible = False
        Me.colSRStatus.Width = 125
        '
        'colSRPartnerID
        '
        Me.colSRPartnerID.HeaderText = "العميل"
        Me.colSRPartnerID.MinimumWidth = 6
        Me.colSRPartnerID.Name = "colSRPartnerID"
        Me.colSRPartnerID.ReadOnly = True
        Me.colSRPartnerID.Width = 125
        '
        'colSRSaleRepCode
        '
        Me.colSRSaleRepCode.HeaderText = "المندوب"
        Me.colSRSaleRepCode.MinimumWidth = 6
        Me.colSRSaleRepCode.Name = "colSRSaleRepCode"
        Me.colSRSaleRepCode.ReadOnly = True
        Me.colSRSaleRepCode.Width = 125
        '
        'colSRNote
        '
        Me.colSRNote.HeaderText = "ملاحظات"
        Me.colSRNote.MinimumWidth = 6
        Me.colSRNote.Name = "colSRNote"
        Me.colSRNote.ReadOnly = True
        Me.colSRNote.Width = 125
        '
        'btnMoveSR
        '
        Me.btnMoveSR.Location = New System.Drawing.Point(792, 6)
        Me.btnMoveSR.Name = "btnMoveSR"
        Me.btnMoveSR.Size = New System.Drawing.Size(184, 75)
        Me.btnMoveSR.TabIndex = 0
        Me.btnMoveSR.Text = "تحريك الطلب"
        Me.btnMoveSR.UseVisualStyleBackColor = True
        '
        'btnSRPrint
        '
        Me.btnSRPrint.Location = New System.Drawing.Point(245, 46)
        Me.btnSRPrint.Name = "btnSRPrint"
        Me.btnSRPrint.Size = New System.Drawing.Size(110, 35)
        Me.btnSRPrint.TabIndex = 0
        Me.btnSRPrint.Text = "طباعة"
        Me.btnSRPrint.UseVisualStyleBackColor = True
        '
        'btnCloseDistribution
        '
        Me.btnCloseDistribution.Location = New System.Drawing.Point(134, 46)
        Me.btnCloseDistribution.Name = "btnCloseDistribution"
        Me.btnCloseDistribution.Size = New System.Drawing.Size(105, 35)
        Me.btnCloseDistribution.TabIndex = 0
        Me.btnCloseDistribution.Text = "اغلاق"
        Me.btnCloseDistribution.UseVisualStyleBackColor = True
        '
        'dgvSRD
        '
        Me.dgvSRD.AllowUserToAddRows = False
        Me.dgvSRD.AllowUserToDeleteRows = False
        Me.dgvSRD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSRD.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colSRDID, Me.colLOCode, Me.colSRDBusinessStatusID, Me.colLoadingStatusID, Me.colSRDProductCode, Me.colSRDBusinessStatusName, Me.colLoadingStatusName, Me.colSRDProductType, Me.colSRDQTY, Me.colSRDAvailableQTY, Me.colSRDLoadedQty, Me.colSRDNotes})
        Me.dgvSRD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSRD.Location = New System.Drawing.Point(0, 0)
        Me.dgvSRD.MultiSelect = False
        Me.dgvSRD.Name = "dgvSRD"
        Me.dgvSRD.ReadOnly = True
        Me.dgvSRD.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSRD.RowHeadersVisible = False
        Me.dgvSRD.RowHeadersWidth = 51
        Me.dgvSRD.RowTemplate.Height = 26
        Me.dgvSRD.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSRD.Size = New System.Drawing.Size(979, 548)
        Me.dgvSRD.TabIndex = 0
        '
        'colSRDID
        '
        Me.colSRDID.HeaderText = "Column1"
        Me.colSRDID.MinimumWidth = 6
        Me.colSRDID.Name = "colSRDID"
        Me.colSRDID.ReadOnly = True
        Me.colSRDID.Visible = False
        Me.colSRDID.Width = 125
        '
        'colLOCode
        '
        Me.colLOCode.HeaderText = "كود امر التحميل"
        Me.colLOCode.MinimumWidth = 6
        Me.colLOCode.Name = "colLOCode"
        Me.colLOCode.ReadOnly = True
        Me.colLOCode.Width = 125
        '
        'colSRDBusinessStatusID
        '
        Me.colSRDBusinessStatusID.HeaderText = "BusinessStatusID  "
        Me.colSRDBusinessStatusID.MinimumWidth = 6
        Me.colSRDBusinessStatusID.Name = "colSRDBusinessStatusID"
        Me.colSRDBusinessStatusID.ReadOnly = True
        Me.colSRDBusinessStatusID.Visible = False
        Me.colSRDBusinessStatusID.Width = 125
        '
        'colLoadingStatusID
        '
        Me.colLoadingStatusID.HeaderText = "حالة امر التحميل"
        Me.colLoadingStatusID.MinimumWidth = 6
        Me.colLoadingStatusID.Name = "colLoadingStatusID"
        Me.colLoadingStatusID.ReadOnly = True
        Me.colLoadingStatusID.Visible = False
        Me.colLoadingStatusID.Width = 125
        '
        'colSRDProductCode
        '
        Me.colSRDProductCode.HeaderText = "كود الصنف"
        Me.colSRDProductCode.MinimumWidth = 6
        Me.colSRDProductCode.Name = "colSRDProductCode"
        Me.colSRDProductCode.ReadOnly = True
        Me.colSRDProductCode.Width = 125
        '
        'colSRDBusinessStatusName
        '
        Me.colSRDBusinessStatusName.HeaderText = "حالة الطلب"
        Me.colSRDBusinessStatusName.MinimumWidth = 6
        Me.colSRDBusinessStatusName.Name = "colSRDBusinessStatusName"
        Me.colSRDBusinessStatusName.ReadOnly = True
        Me.colSRDBusinessStatusName.Width = 125
        '
        'colLoadingStatusName
        '
        Me.colLoadingStatusName.HeaderText = "حالة امر التحميل"
        Me.colLoadingStatusName.MinimumWidth = 6
        Me.colLoadingStatusName.Name = "colLoadingStatusName"
        Me.colLoadingStatusName.ReadOnly = True
        Me.colLoadingStatusName.Width = 125
        '
        'colSRDProductType
        '
        Me.colSRDProductType.HeaderText = "النوع"
        Me.colSRDProductType.MinimumWidth = 6
        Me.colSRDProductType.Name = "colSRDProductType"
        Me.colSRDProductType.ReadOnly = True
        Me.colSRDProductType.Width = 125
        '
        'colSRDQTY
        '
        Me.colSRDQTY.HeaderText = "الكمية المطلوبة"
        Me.colSRDQTY.MinimumWidth = 6
        Me.colSRDQTY.Name = "colSRDQTY"
        Me.colSRDQTY.ReadOnly = True
        Me.colSRDQTY.Width = 125
        '
        'colSRDAvailableQTY
        '
        Me.colSRDAvailableQTY.HeaderText = "الكمية المتوفرة"
        Me.colSRDAvailableQTY.MinimumWidth = 6
        Me.colSRDAvailableQTY.Name = "colSRDAvailableQTY"
        Me.colSRDAvailableQTY.ReadOnly = True
        Me.colSRDAvailableQTY.Width = 125
        '
        'colSRDLoadedQty
        '
        Me.colSRDLoadedQty.HeaderText = "الكمية المحملة"
        Me.colSRDLoadedQty.MinimumWidth = 6
        Me.colSRDLoadedQty.Name = "colSRDLoadedQty"
        Me.colSRDLoadedQty.ReadOnly = True
        Me.colSRDLoadedQty.Width = 125
        '
        'colSRDNotes
        '
        Me.colSRDNotes.HeaderText = "ملاحظات"
        Me.colSRDNotes.MinimumWidth = 6
        Me.colSRDNotes.Name = "colSRDNotes"
        Me.colSRDNotes.ReadOnly = True
        Me.colSRDNotes.Width = 125
        '
        'DataGridViewTextBoxColumn26
        '
        Me.DataGridViewTextBoxColumn26.HeaderText = "الكمية المحملة"
        Me.DataGridViewTextBoxColumn26.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn26.Name = "DataGridViewTextBoxColumn26"
        Me.DataGridViewTextBoxColumn26.Width = 125
        '
        'DataGridViewTextBoxColumn27
        '
        Me.DataGridViewTextBoxColumn27.HeaderText = "الوحدة"
        Me.DataGridViewTextBoxColumn27.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn27.Name = "DataGridViewTextBoxColumn27"
        Me.DataGridViewTextBoxColumn27.Width = 125
        '
        'DataGridViewTextBoxColumn28
        '
        Me.DataGridViewTextBoxColumn28.HeaderText = "الكمية المتوفرة"
        Me.DataGridViewTextBoxColumn28.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn28.Name = "DataGridViewTextBoxColumn28"
        Me.DataGridViewTextBoxColumn28.Width = 125
        '
        'DataGridViewTextBoxColumn29
        '
        Me.DataGridViewTextBoxColumn29.HeaderText = "الكمية المحملة مسبقا"
        Me.DataGridViewTextBoxColumn29.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn29.Name = "DataGridViewTextBoxColumn29"
        Me.DataGridViewTextBoxColumn29.Width = 125
        '
        'DataGridViewTextBoxColumn30
        '
        Me.DataGridViewTextBoxColumn30.HeaderText = "ملاحظات طلب المبيعات"
        Me.DataGridViewTextBoxColumn30.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn30.Name = "DataGridViewTextBoxColumn30"
        Me.DataGridViewTextBoxColumn30.Width = 125
        '
        'DataGridViewTextBoxColumn31
        '
        Me.DataGridViewTextBoxColumn31.HeaderText = "ملاحظات"
        Me.DataGridViewTextBoxColumn31.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn31.Name = "DataGridViewTextBoxColumn31"
        Me.DataGridViewTextBoxColumn31.Width = 125
        '
        'DataGridViewTextBoxColumn32
        '
        Me.DataGridViewTextBoxColumn32.HeaderText = "كود اذن التحميل"
        Me.DataGridViewTextBoxColumn32.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn32.Name = "DataGridViewTextBoxColumn32"
        Me.DataGridViewTextBoxColumn32.Width = 125
        '
        'DataGridViewTextBoxColumn33
        '
        Me.DataGridViewTextBoxColumn33.HeaderText = "كود طلب المبيعات"
        Me.DataGridViewTextBoxColumn33.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn33.Name = "DataGridViewTextBoxColumn33"
        Me.DataGridViewTextBoxColumn33.Width = 125
        '
        'DataGridViewTextBoxColumn34
        '
        Me.DataGridViewTextBoxColumn34.HeaderText = "تاريخ الانشاء اذن التحميل"
        Me.DataGridViewTextBoxColumn34.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn34.Name = "DataGridViewTextBoxColumn34"
        Me.DataGridViewTextBoxColumn34.Width = 125
        '
        'DataGridViewTextBoxColumn35
        '
        Me.DataGridViewTextBoxColumn35.HeaderText = "العميل"
        Me.DataGridViewTextBoxColumn35.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn35.Name = "DataGridViewTextBoxColumn35"
        Me.DataGridViewTextBoxColumn35.Width = 125
        '
        'DataGridViewComboBoxColumn1
        '
        Me.DataGridViewComboBoxColumn1.HeaderText = "السائق"
        Me.DataGridViewComboBoxColumn1.MinimumWidth = 6
        Me.DataGridViewComboBoxColumn1.Name = "DataGridViewComboBoxColumn1"
        Me.DataGridViewComboBoxColumn1.Width = 125
        '
        'DataGridViewComboBoxColumn2
        '
        Me.DataGridViewComboBoxColumn2.HeaderText = "رقم السيارة"
        Me.DataGridViewComboBoxColumn2.MinimumWidth = 6
        Me.DataGridViewComboBoxColumn2.Name = "DataGridViewComboBoxColumn2"
        Me.DataGridViewComboBoxColumn2.Width = 125
        '
        'DataGridViewComboBoxColumn3
        '
        Me.DataGridViewComboBoxColumn3.HeaderText = "مسؤول التحميل"
        Me.DataGridViewComboBoxColumn3.MinimumWidth = 6
        Me.DataGridViewComboBoxColumn3.Name = "DataGridViewComboBoxColumn3"
        Me.DataGridViewComboBoxColumn3.Width = 125
        '
        'DataGridViewComboBoxColumn4
        '
        Me.DataGridViewComboBoxColumn4.HeaderText = "المخزن"
        Me.DataGridViewComboBoxColumn4.MinimumWidth = 6
        Me.DataGridViewComboBoxColumn4.Name = "DataGridViewComboBoxColumn4"
        Me.DataGridViewComboBoxColumn4.Width = 125
        '
        'DataGridViewTextBoxColumn36
        '
        Me.DataGridViewTextBoxColumn36.HeaderText = "ملاحظات"
        Me.DataGridViewTextBoxColumn36.MinimumWidth = 6
        Me.DataGridViewTextBoxColumn36.Name = "DataGridViewTextBoxColumn36"
        Me.DataGridViewTextBoxColumn36.Width = 125
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.btnOpenLoadingBoard)
        Me.Panel3.Controls.Add(Me.btnCloseSR)
        Me.Panel3.Controls.Add(Me.btnStartLoading)
        Me.Panel3.Controls.Add(Me.btnCloseDistribution)
        Me.Panel3.Controls.Add(Me.btnSRPrint)
        Me.Panel3.Controls.Add(Me.btnMoveSR)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel3.Location = New System.Drawing.Point(0, 548)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(979, 87)
        Me.Panel3.TabIndex = 6
        '
        'btnOpenLoadingBoard
        '
        Me.btnOpenLoadingBoard.Location = New System.Drawing.Point(562, 47)
        Me.btnOpenLoadingBoard.Name = "btnOpenLoadingBoard"
        Me.btnOpenLoadingBoard.Size = New System.Drawing.Size(184, 35)
        Me.btnOpenLoadingBoard.TabIndex = 3
        Me.btnOpenLoadingBoard.Text = "فتح لوحة التحميل"
        Me.btnOpenLoadingBoard.UseVisualStyleBackColor = True
        '
        'btnCloseSR
        '
        Me.btnCloseSR.Location = New System.Drawing.Point(13, 46)
        Me.btnCloseSR.Name = "btnCloseSR"
        Me.btnCloseSR.Size = New System.Drawing.Size(115, 35)
        Me.btnCloseSR.TabIndex = 3
        Me.btnCloseSR.Text = "قفل الطلبات"
        Me.btnCloseSR.UseVisualStyleBackColor = True
        '
        'btnStartLoading
        '
        Me.btnStartLoading.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btnStartLoading.Location = New System.Drawing.Point(562, 6)
        Me.btnStartLoading.Name = "btnStartLoading"
        Me.btnStartLoading.Size = New System.Drawing.Size(184, 35)
        Me.btnStartLoading.TabIndex = 1
        Me.btnStartLoading.Text = "بدء تحميل الطلب"
        Me.btnStartLoading.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 111)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvSRD)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel3)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.dgvSR)
        Me.SplitContainer1.Size = New System.Drawing.Size(1611, 635)
        Me.SplitContainer1.SplitterDistance = 979
        Me.SplitContainer1.TabIndex = 7
        '
        'lblPartener
        '
        Me.lblPartener.AutoSize = True
        Me.lblPartener.Location = New System.Drawing.Point(1304, 5)
        Me.lblPartener.Name = "lblPartener"
        Me.lblPartener.Size = New System.Drawing.Size(46, 17)
        Me.lblPartener.TabIndex = 4
        Me.lblPartener.Text = "العميل"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(569, 5)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(72, 17)
        Me.lblStatus.TabIndex = 4
        Me.lblStatus.Text = "حالة الطلب"
        '
        'lblSRCode
        '
        Me.lblSRCode.AutoSize = True
        Me.lblSRCode.Location = New System.Drawing.Point(1099, 5)
        Me.lblSRCode.Name = "lblSRCode"
        Me.lblSRCode.Size = New System.Drawing.Size(67, 17)
        Me.lblSRCode.TabIndex = 4
        Me.lblSRCode.Text = "كود الطلب"
        '
        'tbcSRDistribution
        '
        Me.tbcSRDistribution.Controls.Add(Me.tabpSRNew)
        Me.tbcSRDistribution.Controls.Add(Me.tabpSRInCutting)
        Me.tbcSRDistribution.Controls.Add(Me.tabpSRInLoading)
        Me.tbcSRDistribution.Controls.Add(Me.tabpSROutLoading)
        Me.tbcSRDistribution.Controls.Add(Me.tabPartialSRs)
        Me.tbcSRDistribution.Controls.Add(Me.tabpSRWaitingInvoicesSRs)
        Me.tbcSRDistribution.Controls.Add(Me.tabInvoicedSRs)
        Me.tbcSRDistribution.Controls.Add(Me.tabReturnLO)
        Me.tbcSRDistribution.Controls.Add(Me.tabNewInvoice)
        Me.tbcSRDistribution.Dock = System.Windows.Forms.DockStyle.Top
        Me.tbcSRDistribution.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbcSRDistribution.Location = New System.Drawing.Point(0, 0)
        Me.tbcSRDistribution.Name = "tbcSRDistribution"
        Me.tbcSRDistribution.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tbcSRDistribution.RightToLeftLayout = True
        Me.tbcSRDistribution.SelectedIndex = 0
        Me.tbcSRDistribution.ShowToolTips = True
        Me.tbcSRDistribution.Size = New System.Drawing.Size(1611, 56)
        Me.tbcSRDistribution.TabIndex = 6
        '
        'tabpSRNew
        '
        Me.tabpSRNew.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.tabpSRNew.Location = New System.Drawing.Point(4, 30)
        Me.tabpSRNew.Name = "tabpSRNew"
        Me.tabpSRNew.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpSRNew.Size = New System.Drawing.Size(1603, 22)
        Me.tabpSRNew.TabIndex = 0
        Me.tabpSRNew.Text = "طلبات المبيعات الجديدة         "
        Me.tabpSRNew.UseVisualStyleBackColor = True
        '
        'tabpSRInCutting
        '
        Me.tabpSRInCutting.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.tabpSRInCutting.Location = New System.Drawing.Point(4, 30)
        Me.tabpSRInCutting.Name = "tabpSRInCutting"
        Me.tabpSRInCutting.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpSRInCutting.Size = New System.Drawing.Size(1603, 22)
        Me.tabpSRInCutting.TabIndex = 1
        Me.tabpSRInCutting.Text = "طلبات في القص     "
        '
        'tabpSRInLoading
        '
        Me.tabpSRInLoading.Location = New System.Drawing.Point(4, 30)
        Me.tabpSRInLoading.Name = "tabpSRInLoading"
        Me.tabpSRInLoading.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpSRInLoading.Size = New System.Drawing.Size(1603, 22)
        Me.tabpSRInLoading.TabIndex = 2
        Me.tabpSRInLoading.Text = "طلبات في المستودع      "
        Me.tabpSRInLoading.UseVisualStyleBackColor = True
        '
        'tabpSROutLoading
        '
        Me.tabpSROutLoading.Location = New System.Drawing.Point(4, 30)
        Me.tabpSROutLoading.Name = "tabpSROutLoading"
        Me.tabpSROutLoading.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpSROutLoading.Size = New System.Drawing.Size(1603, 22)
        Me.tabpSROutLoading.TabIndex = 3
        Me.tabpSROutLoading.Text = "طلبات في التحميل      "
        Me.tabpSROutLoading.UseVisualStyleBackColor = True
        '
        'tabPartialSRs
        '
        Me.tabPartialSRs.Location = New System.Drawing.Point(4, 30)
        Me.tabPartialSRs.Name = "tabPartialSRs"
        Me.tabPartialSRs.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPartialSRs.Size = New System.Drawing.Size(1603, 22)
        Me.tabPartialSRs.TabIndex = 4
        Me.tabPartialSRs.Text = "طلبات محملة جزئيا     "
        Me.tabPartialSRs.UseVisualStyleBackColor = True
        '
        'tabpSRWaitingInvoicesSRs
        '
        Me.tabpSRWaitingInvoicesSRs.Location = New System.Drawing.Point(4, 30)
        Me.tabpSRWaitingInvoicesSRs.Name = "tabpSRWaitingInvoicesSRs"
        Me.tabpSRWaitingInvoicesSRs.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpSRWaitingInvoicesSRs.Size = New System.Drawing.Size(1603, 22)
        Me.tabpSRWaitingInvoicesSRs.TabIndex = 5
        Me.tabpSRWaitingInvoicesSRs.Text = "طلبات منتظرة الفاتورة      "
        Me.tabpSRWaitingInvoicesSRs.UseVisualStyleBackColor = True
        '
        'tabInvoicedSRs
        '
        Me.tabInvoicedSRs.Location = New System.Drawing.Point(4, 30)
        Me.tabInvoicedSRs.Name = "tabInvoicedSRs"
        Me.tabInvoicedSRs.Padding = New System.Windows.Forms.Padding(3)
        Me.tabInvoicedSRs.Size = New System.Drawing.Size(1603, 22)
        Me.tabInvoicedSRs.TabIndex = 6
        Me.tabInvoicedSRs.Text = "طلبات مقفلة      "
        Me.tabInvoicedSRs.UseVisualStyleBackColor = True
        '
        'tabReturnLO
        '
        Me.tabReturnLO.Location = New System.Drawing.Point(4, 30)
        Me.tabReturnLO.Name = "tabReturnLO"
        Me.tabReturnLO.Padding = New System.Windows.Forms.Padding(3)
        Me.tabReturnLO.Size = New System.Drawing.Size(1603, 22)
        Me.tabReturnLO.TabIndex = 7
        Me.tabReturnLO.Text = "مرتجعات"
        Me.tabReturnLO.UseVisualStyleBackColor = True
        '
        'tabNewInvoice
        '
        Me.tabNewInvoice.Location = New System.Drawing.Point(4, 30)
        Me.tabNewInvoice.Name = "tabNewInvoice"
        Me.tabNewInvoice.Padding = New System.Windows.Forms.Padding(3)
        Me.tabNewInvoice.Size = New System.Drawing.Size(1603, 22)
        Me.tabNewInvoice.TabIndex = 8
        Me.tabNewInvoice.Text = "فواتيرمبيعات"
        Me.tabNewInvoice.UseVisualStyleBackColor = True
        '
        'pnlFilters
        '
        Me.pnlFilters.Controls.Add(Me.tbcSRDistribution)
        Me.pnlFilters.Controls.Add(Me.lblSRCode)
        Me.pnlFilters.Controls.Add(Me.lblStatus)
        Me.pnlFilters.Controls.Add(Me.lblPartener)
        Me.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilters.Location = New System.Drawing.Point(0, 0)
        Me.pnlFilters.Name = "pnlFilters"
        Me.pnlFilters.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.pnlFilters.Size = New System.Drawing.Size(1611, 105)
        Me.pnlFilters.TabIndex = 0
        '
        'frmSRDistripution
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1611, 746)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlFilters)
        Me.Name = "frmSRDistripution"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "frmSRDistripution"
        CType(Me.dgvSR, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvSRD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel3.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.tbcSRDistribution.ResumeLayout(False)
        Me.pnlFilters.ResumeLayout(False)
        Me.pnlFilters.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvSR As DataGridView
    Friend WithEvents dgvSRD As DataGridView
    Friend WithEvents btnSRPrint As Button
    Friend WithEvents btnMoveSR As Button
    Friend WithEvents DataGridViewTextBoxColumn26 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn27 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn28 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn29 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn30 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn31 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn32 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn33 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn34 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn35 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewComboBoxColumn1 As DataGridViewComboBoxColumn
    Friend WithEvents DataGridViewComboBoxColumn2 As DataGridViewComboBoxColumn
    Friend WithEvents DataGridViewComboBoxColumn3 As DataGridViewComboBoxColumn
    Friend WithEvents DataGridViewComboBoxColumn4 As DataGridViewComboBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn36 As DataGridViewTextBoxColumn
    Friend WithEvents btnCloseDistribution As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents btnStartLoading As Button
    Friend WithEvents btnCloseSR As Button
    Friend WithEvents btnOpenLoadingBoard As Button
    Friend WithEvents colSRID As DataGridViewTextBoxColumn
    Friend WithEvents colSRCode As DataGridViewTextBoxColumn
    Friend WithEvents colSRDate As DataGridViewTextBoxColumn
    Friend WithEvents colSRStatus As DataGridViewTextBoxColumn
    Friend WithEvents colSRPartnerID As DataGridViewTextBoxColumn
    Friend WithEvents colSRSaleRepCode As DataGridViewTextBoxColumn
    Friend WithEvents colSRNote As DataGridViewTextBoxColumn
    Friend WithEvents colSRDID As DataGridViewTextBoxColumn
    Friend WithEvents colLOCode As DataGridViewTextBoxColumn
    Friend WithEvents colSRDBusinessStatusID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingStatusID As DataGridViewTextBoxColumn
    Friend WithEvents colSRDProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colSRDBusinessStatusName As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingStatusName As DataGridViewTextBoxColumn
    Friend WithEvents colSRDProductType As DataGridViewTextBoxColumn
    Friend WithEvents colSRDQTY As DataGridViewTextBoxColumn
    Friend WithEvents colSRDAvailableQTY As DataGridViewTextBoxColumn
    Friend WithEvents colSRDLoadedQty As DataGridViewTextBoxColumn
    Friend WithEvents colSRDNotes As DataGridViewTextBoxColumn
    Friend WithEvents lblPartener As Label
    Friend WithEvents lblStatus As Label
    Friend WithEvents lblSRCode As Label
    Friend WithEvents tbcSRDistribution As TabControl
    Friend WithEvents tabpSRNew As TabPage
    Friend WithEvents tabpSRInCutting As TabPage
    Friend WithEvents tabpSRInLoading As TabPage
    Friend WithEvents tabpSROutLoading As TabPage
    Friend WithEvents tabPartialSRs As TabPage
    Friend WithEvents tabpSRWaitingInvoicesSRs As TabPage
    Friend WithEvents tabInvoicedSRs As TabPage
    Friend WithEvents tabReturnLO As TabPage
    Friend WithEvents tabNewInvoice As TabPage
    Friend WithEvents pnlFilters As Panel
End Class
