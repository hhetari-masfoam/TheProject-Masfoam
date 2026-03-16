<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLoadingBoard
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
        Me.pnlOverlayLoading = New System.Windows.Forms.Panel()
        Me.scLoadingMain = New System.Windows.Forms.SplitContainer()
        Me.dgvLOs = New System.Windows.Forms.DataGridView()
        Me.colLOsID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLOsCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLOsDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLOsDriverCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colLOsVehicale = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colLOsSupervisor = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colLOsNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLOsStoreID = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.dgvLoadingSR = New System.Windows.Forms.DataGridView()
        Me.colLoadingSRCodes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRtoInvoice = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colLoadingSRID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRPartners = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDates = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvLoadingSRD = New System.Windows.Forms.DataGridView()
        Me.colLoadingSRDCodes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDSRDID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingOrderDetailID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDSRID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDProductType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDBusinessStatusID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDBusinessStatusName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDFulfillmentStatusID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDFulfillmentStatusName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDAvailableQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDLoadedBefore = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDLoadedQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDRemainingQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLoadingSRDLoadedInThisLO = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlTotals = New System.Windows.Forms.Panel()
        Me.lblTotalLOQTY = New System.Windows.Forms.Label()
        Me.lblTotalLOVolume = New System.Windows.Forms.Label()
        Me.pnlLoadingActions = New System.Windows.Forms.Panel()
        Me.dgvOpenedLOs = New System.Windows.Forms.DataGridView()
        Me.colOpenLOsID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsInitiatedDateTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsStatusID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsVehicleInfo = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colOpenLOsSupervisor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlOpenLOs = New System.Windows.Forms.Panel()
        Me.btnRemoveSR = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnExportToInvoice = New System.Windows.Forms.Button()
        Me.btnCloseBoard = New System.Windows.Forms.Button()
        Me.btnAddSelectedSRToLO = New System.Windows.Forms.Button()
        Me.btnSaveLO = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnPostLoading = New System.Windows.Forms.Button()
        Me.pnlOverlayLoading.SuspendLayout()
        CType(Me.scLoadingMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scLoadingMain.Panel1.SuspendLayout()
        Me.scLoadingMain.Panel2.SuspendLayout()
        Me.scLoadingMain.SuspendLayout()
        CType(Me.dgvLOs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvLoadingSR, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvLoadingSRD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTotals.SuspendLayout()
        Me.pnlLoadingActions.SuspendLayout()
        CType(Me.dgvOpenedLOs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlOpenLOs.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlOverlayLoading
        '
        Me.pnlOverlayLoading.BackColor = System.Drawing.Color.PapayaWhip
        Me.pnlOverlayLoading.Controls.Add(Me.scLoadingMain)
        Me.pnlOverlayLoading.Controls.Add(Me.pnlLoadingActions)
        Me.pnlOverlayLoading.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOverlayLoading.Location = New System.Drawing.Point(0, 0)
        Me.pnlOverlayLoading.Name = "pnlOverlayLoading"
        Me.pnlOverlayLoading.Size = New System.Drawing.Size(1782, 753)
        Me.pnlOverlayLoading.TabIndex = 9
        '
        'scLoadingMain
        '
        Me.scLoadingMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scLoadingMain.Location = New System.Drawing.Point(0, 0)
        Me.scLoadingMain.Name = "scLoadingMain"
        Me.scLoadingMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'scLoadingMain.Panel1
        '
        Me.scLoadingMain.Panel1.Controls.Add(Me.dgvLOs)
        Me.scLoadingMain.Panel1.Controls.Add(Me.dgvLoadingSR)
        '
        'scLoadingMain.Panel2
        '
        Me.scLoadingMain.Panel2.Controls.Add(Me.dgvLoadingSRD)
        Me.scLoadingMain.Panel2.Controls.Add(Me.pnlTotals)
        Me.scLoadingMain.Size = New System.Drawing.Size(1185, 753)
        Me.scLoadingMain.SplitterDistance = 259
        Me.scLoadingMain.TabIndex = 2
        '
        'dgvLOs
        '
        Me.dgvLOs.AllowUserToAddRows = False
        Me.dgvLOs.AllowUserToDeleteRows = False
        Me.dgvLOs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvLOs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLOs.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colLOsID, Me.colLOsCode, Me.colLOsDate, Me.colLOsDriverCode, Me.colLOsVehicale, Me.colLOsSupervisor, Me.colLOsNote, Me.colLOsStoreID})
        Me.dgvLOs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvLOs.Location = New System.Drawing.Point(0, 0)
        Me.dgvLOs.MultiSelect = False
        Me.dgvLOs.Name = "dgvLOs"
        Me.dgvLOs.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvLOs.RowHeadersWidth = 51
        Me.dgvLOs.RowTemplate.Height = 26
        Me.dgvLOs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvLOs.Size = New System.Drawing.Size(1185, 111)
        Me.dgvLOs.TabIndex = 1
        '
        'colLOsID
        '
        Me.colLOsID.HeaderText = "Column1"
        Me.colLOsID.MinimumWidth = 6
        Me.colLOsID.Name = "colLOsID"
        Me.colLOsID.Visible = False
        '
        'colLOsCode
        '
        Me.colLOsCode.HeaderText = "الكود"
        Me.colLOsCode.MinimumWidth = 6
        Me.colLOsCode.Name = "colLOsCode"
        '
        'colLOsDate
        '
        Me.colLOsDate.HeaderText = "التاريخ"
        Me.colLOsDate.MinimumWidth = 6
        Me.colLOsDate.Name = "colLOsDate"
        '
        'colLOsDriverCode
        '
        Me.colLOsDriverCode.HeaderText = "السائق"
        Me.colLOsDriverCode.MinimumWidth = 6
        Me.colLOsDriverCode.Name = "colLOsDriverCode"
        '
        'colLOsVehicale
        '
        Me.colLOsVehicale.HeaderText = "السيارة"
        Me.colLOsVehicale.MinimumWidth = 6
        Me.colLOsVehicale.Name = "colLOsVehicale"
        '
        'colLOsSupervisor
        '
        Me.colLOsSupervisor.HeaderText = "المشرف"
        Me.colLOsSupervisor.MinimumWidth = 6
        Me.colLOsSupervisor.Name = "colLOsSupervisor"
        '
        'colLOsNote
        '
        Me.colLOsNote.HeaderText = "ملاحظات"
        Me.colLOsNote.MinimumWidth = 6
        Me.colLOsNote.Name = "colLOsNote"
        Me.colLOsNote.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colLOsNote.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'colLOsStoreID
        '
        Me.colLOsStoreID.HeaderText = "المستودع"
        Me.colLOsStoreID.MinimumWidth = 6
        Me.colLOsStoreID.Name = "colLOsStoreID"
        Me.colLOsStoreID.ReadOnly = True
        '
        'dgvLoadingSR
        '
        Me.dgvLoadingSR.AllowUserToAddRows = False
        Me.dgvLoadingSR.AllowUserToDeleteRows = False
        Me.dgvLoadingSR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLoadingSR.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colLoadingSRCodes, Me.colLoadingSRtoInvoice, Me.colLoadingSRID, Me.colLoadingSRPartners, Me.colLoadingSRDates})
        Me.dgvLoadingSR.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.dgvLoadingSR.Location = New System.Drawing.Point(0, 111)
        Me.dgvLoadingSR.MultiSelect = False
        Me.dgvLoadingSR.Name = "dgvLoadingSR"
        Me.dgvLoadingSR.ReadOnly = True
        Me.dgvLoadingSR.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvLoadingSR.RowHeadersWidth = 51
        Me.dgvLoadingSR.RowTemplate.Height = 26
        Me.dgvLoadingSR.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvLoadingSR.Size = New System.Drawing.Size(1185, 148)
        Me.dgvLoadingSR.TabIndex = 0
        '
        'colLoadingSRCodes
        '
        Me.colLoadingSRCodes.HeaderText = "كود الطلبات"
        Me.colLoadingSRCodes.MinimumWidth = 6
        Me.colLoadingSRCodes.Name = "colLoadingSRCodes"
        Me.colLoadingSRCodes.ReadOnly = True
        Me.colLoadingSRCodes.Width = 125
        '
        'colLoadingSRtoInvoice
        '
        Me.colLoadingSRtoInvoice.HeaderText = "تحميل للفاتورة"
        Me.colLoadingSRtoInvoice.MinimumWidth = 6
        Me.colLoadingSRtoInvoice.Name = "colLoadingSRtoInvoice"
        Me.colLoadingSRtoInvoice.ReadOnly = True
        Me.colLoadingSRtoInvoice.Visible = False
        Me.colLoadingSRtoInvoice.Width = 125
        '
        'colLoadingSRID
        '
        Me.colLoadingSRID.HeaderText = "Column1"
        Me.colLoadingSRID.MinimumWidth = 6
        Me.colLoadingSRID.Name = "colLoadingSRID"
        Me.colLoadingSRID.ReadOnly = True
        Me.colLoadingSRID.Width = 125
        '
        'colLoadingSRPartners
        '
        Me.colLoadingSRPartners.HeaderText = "العميل"
        Me.colLoadingSRPartners.MinimumWidth = 6
        Me.colLoadingSRPartners.Name = "colLoadingSRPartners"
        Me.colLoadingSRPartners.ReadOnly = True
        Me.colLoadingSRPartners.Width = 125
        '
        'colLoadingSRDates
        '
        Me.colLoadingSRDates.HeaderText = "التاريخ"
        Me.colLoadingSRDates.MinimumWidth = 6
        Me.colLoadingSRDates.Name = "colLoadingSRDates"
        Me.colLoadingSRDates.ReadOnly = True
        Me.colLoadingSRDates.Width = 125
        '
        'dgvLoadingSRD
        '
        Me.dgvLoadingSRD.AllowUserToAddRows = False
        Me.dgvLoadingSRD.AllowUserToDeleteRows = False
        Me.dgvLoadingSRD.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvLoadingSRD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLoadingSRD.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colLoadingSRDCodes, Me.colLoadingSRDSRDID, Me.colLoadingOrderDetailID, Me.colLoadingSRDSRID, Me.colLoadingSRDProductID, Me.colLoadingSRDID, Me.colLoadingSRDProductCode, Me.colLoadingSRDProductType, Me.colLoadingSRDBusinessStatusID, Me.colLoadingSRDBusinessStatusName, Me.colLoadingSRDFulfillmentStatusID, Me.colLoadingSRDFulfillmentStatusName, Me.colLoadingSRDQTY, Me.colLoadingSRDAvailableQTY, Me.colLoadingSRDLoadedBefore, Me.colLoadingSRDLoadedQTY, Me.colLoadingSRDRemainingQTY, Me.colLoadingSRDLoadedInThisLO})
        Me.dgvLoadingSRD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvLoadingSRD.Location = New System.Drawing.Point(0, 0)
        Me.dgvLoadingSRD.Name = "dgvLoadingSRD"
        Me.dgvLoadingSRD.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvLoadingSRD.RowHeadersWidth = 51
        Me.dgvLoadingSRD.RowTemplate.Height = 26
        Me.dgvLoadingSRD.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvLoadingSRD.Size = New System.Drawing.Size(1185, 490)
        Me.dgvLoadingSRD.TabIndex = 0
        '
        'colLoadingSRDCodes
        '
        Me.colLoadingSRDCodes.HeaderText = "كود طلب المبيعات"
        Me.colLoadingSRDCodes.MinimumWidth = 6
        Me.colLoadingSRDCodes.Name = "colLoadingSRDCodes"
        '
        'colLoadingSRDSRDID
        '
        Me.colLoadingSRDSRDID.HeaderText = "كودتفاصيل التحميل"
        Me.colLoadingSRDSRDID.MinimumWidth = 6
        Me.colLoadingSRDSRDID.Name = "colLoadingSRDSRDID"
        Me.colLoadingSRDSRDID.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.colLoadingSRDSRDID.Visible = False
        '
        'colLoadingOrderDetailID
        '
        Me.colLoadingOrderDetailID.DataPropertyName = "LoadingOrderDetailID"
        Me.colLoadingOrderDetailID.HeaderText = "كود تفاصيل التحميل 2"
        Me.colLoadingOrderDetailID.MinimumWidth = 6
        Me.colLoadingOrderDetailID.Name = "colLoadingOrderDetailID"
        Me.colLoadingOrderDetailID.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.colLoadingOrderDetailID.Visible = False
        '
        'colLoadingSRDSRID
        '
        Me.colLoadingSRDSRID.HeaderText = "رقم تفاصيل الطلب"
        Me.colLoadingSRDSRID.MinimumWidth = 6
        Me.colLoadingSRDSRID.Name = "colLoadingSRDSRID"
        Me.colLoadingSRDSRID.Visible = False
        '
        'colLoadingSRDProductID
        '
        Me.colLoadingSRDProductID.HeaderText = "رقم الصنف"
        Me.colLoadingSRDProductID.MinimumWidth = 6
        Me.colLoadingSRDProductID.Name = "colLoadingSRDProductID"
        Me.colLoadingSRDProductID.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.colLoadingSRDProductID.Visible = False
        '
        'colLoadingSRDID
        '
        Me.colLoadingSRDID.HeaderText = "رقم تفاصيل التحميل"
        Me.colLoadingSRDID.MinimumWidth = 6
        Me.colLoadingSRDID.Name = "colLoadingSRDID"
        Me.colLoadingSRDID.Visible = False
        '
        'colLoadingSRDProductCode
        '
        Me.colLoadingSRDProductCode.HeaderText = "كود الاصناف"
        Me.colLoadingSRDProductCode.MinimumWidth = 6
        Me.colLoadingSRDProductCode.Name = "colLoadingSRDProductCode"
        '
        'colLoadingSRDProductType
        '
        Me.colLoadingSRDProductType.HeaderText = "النوع"
        Me.colLoadingSRDProductType.MinimumWidth = 6
        Me.colLoadingSRDProductType.Name = "colLoadingSRDProductType"
        '
        'colLoadingSRDBusinessStatusID
        '
        Me.colLoadingSRDBusinessStatusID.HeaderText = "BusinessStatusID"
        Me.colLoadingSRDBusinessStatusID.MinimumWidth = 6
        Me.colLoadingSRDBusinessStatusID.Name = "colLoadingSRDBusinessStatusID"
        Me.colLoadingSRDBusinessStatusID.Visible = False
        '
        'colLoadingSRDBusinessStatusName
        '
        Me.colLoadingSRDBusinessStatusName.HeaderText = "حالة الطلب"
        Me.colLoadingSRDBusinessStatusName.MinimumWidth = 6
        Me.colLoadingSRDBusinessStatusName.Name = "colLoadingSRDBusinessStatusName"
        Me.colLoadingSRDBusinessStatusName.Visible = False
        '
        'colLoadingSRDFulfillmentStatusID
        '
        Me.colLoadingSRDFulfillmentStatusID.HeaderText = "FulfillmentStatusID"
        Me.colLoadingSRDFulfillmentStatusID.MinimumWidth = 6
        Me.colLoadingSRDFulfillmentStatusID.Name = "colLoadingSRDFulfillmentStatusID"
        Me.colLoadingSRDFulfillmentStatusID.Visible = False
        '
        'colLoadingSRDFulfillmentStatusName
        '
        Me.colLoadingSRDFulfillmentStatusName.HeaderText = "الانجاز"
        Me.colLoadingSRDFulfillmentStatusName.MinimumWidth = 6
        Me.colLoadingSRDFulfillmentStatusName.Name = "colLoadingSRDFulfillmentStatusName"
        Me.colLoadingSRDFulfillmentStatusName.Visible = False
        '
        'colLoadingSRDQTY
        '
        Me.colLoadingSRDQTY.HeaderText = "الكمية الطلوبة"
        Me.colLoadingSRDQTY.MinimumWidth = 6
        Me.colLoadingSRDQTY.Name = "colLoadingSRDQTY"
        '
        'colLoadingSRDAvailableQTY
        '
        Me.colLoadingSRDAvailableQTY.HeaderText = "المتوفر في المخزن"
        Me.colLoadingSRDAvailableQTY.MinimumWidth = 6
        Me.colLoadingSRDAvailableQTY.Name = "colLoadingSRDAvailableQTY"
        '
        'colLoadingSRDLoadedBefore
        '
        Me.colLoadingSRDLoadedBefore.HeaderText = "محمل سابقا"
        Me.colLoadingSRDLoadedBefore.MinimumWidth = 6
        Me.colLoadingSRDLoadedBefore.Name = "colLoadingSRDLoadedBefore"
        '
        'colLoadingSRDLoadedQTY
        '
        Me.colLoadingSRDLoadedQTY.DataPropertyName = "LoadedQty"
        Me.colLoadingSRDLoadedQTY.HeaderText = "محمل في امر التحميل هذا"
        Me.colLoadingSRDLoadedQTY.MinimumWidth = 6
        Me.colLoadingSRDLoadedQTY.Name = "colLoadingSRDLoadedQTY"
        '
        'colLoadingSRDRemainingQTY
        '
        Me.colLoadingSRDRemainingQTY.HeaderText = "العدد المتبقي للطلبية"
        Me.colLoadingSRDRemainingQTY.MinimumWidth = 6
        Me.colLoadingSRDRemainingQTY.Name = "colLoadingSRDRemainingQTY"
        '
        'colLoadingSRDLoadedInThisLO
        '
        Me.colLoadingSRDLoadedInThisLO.HeaderText = "تحميل الان"
        Me.colLoadingSRDLoadedInThisLO.MinimumWidth = 6
        Me.colLoadingSRDLoadedInThisLO.Name = "colLoadingSRDLoadedInThisLO"
        '
        'pnlTotals
        '
        Me.pnlTotals.BackColor = System.Drawing.Color.Transparent
        Me.pnlTotals.Controls.Add(Me.lblTotalLOQTY)
        Me.pnlTotals.Controls.Add(Me.lblTotalLOVolume)
        Me.pnlTotals.Location = New System.Drawing.Point(3, 436)
        Me.pnlTotals.Name = "pnlTotals"
        Me.pnlTotals.Size = New System.Drawing.Size(269, 51)
        Me.pnlTotals.TabIndex = 3
        '
        'lblTotalLOQTY
        '
        Me.lblTotalLOQTY.AutoSize = True
        Me.lblTotalLOQTY.Location = New System.Drawing.Point(0, 0)
        Me.lblTotalLOQTY.Name = "lblTotalLOQTY"
        Me.lblTotalLOQTY.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblTotalLOQTY.Size = New System.Drawing.Size(0, 17)
        Me.lblTotalLOQTY.TabIndex = 3
        '
        'lblTotalLOVolume
        '
        Me.lblTotalLOVolume.AutoSize = True
        Me.lblTotalLOVolume.Location = New System.Drawing.Point(0, 0)
        Me.lblTotalLOVolume.Name = "lblTotalLOVolume"
        Me.lblTotalLOVolume.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblTotalLOVolume.Size = New System.Drawing.Size(0, 17)
        Me.lblTotalLOVolume.TabIndex = 3
        '
        'pnlLoadingActions
        '
        Me.pnlLoadingActions.Controls.Add(Me.dgvOpenedLOs)
        Me.pnlLoadingActions.Controls.Add(Me.pnlOpenLOs)
        Me.pnlLoadingActions.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlLoadingActions.Location = New System.Drawing.Point(1185, 0)
        Me.pnlLoadingActions.Name = "pnlLoadingActions"
        Me.pnlLoadingActions.Size = New System.Drawing.Size(597, 753)
        Me.pnlLoadingActions.TabIndex = 1
        '
        'dgvOpenedLOs
        '
        Me.dgvOpenedLOs.AllowUserToAddRows = False
        Me.dgvOpenedLOs.AllowUserToDeleteRows = False
        Me.dgvOpenedLOs.AllowUserToResizeColumns = False
        Me.dgvOpenedLOs.AllowUserToResizeRows = False
        Me.dgvOpenedLOs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvOpenedLOs.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colOpenLOsID, Me.colOpenLOsCode, Me.colOpenLOsInitiatedDateTime, Me.colOpenLOsStatus, Me.colOpenLOsStatusID, Me.colOpenLOsVehicleInfo, Me.colOpenLOsSupervisor})
        Me.dgvOpenedLOs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOpenedLOs.Location = New System.Drawing.Point(0, 0)
        Me.dgvOpenedLOs.MultiSelect = False
        Me.dgvOpenedLOs.Name = "dgvOpenedLOs"
        Me.dgvOpenedLOs.ReadOnly = True
        Me.dgvOpenedLOs.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvOpenedLOs.RowHeadersVisible = False
        Me.dgvOpenedLOs.RowHeadersWidth = 51
        Me.dgvOpenedLOs.RowTemplate.Height = 26
        Me.dgvOpenedLOs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvOpenedLOs.Size = New System.Drawing.Size(597, 608)
        Me.dgvOpenedLOs.TabIndex = 2
        '
        'colOpenLOsID
        '
        Me.colOpenLOsID.HeaderText = "Column1"
        Me.colOpenLOsID.MinimumWidth = 6
        Me.colOpenLOsID.Name = "colOpenLOsID"
        Me.colOpenLOsID.ReadOnly = True
        Me.colOpenLOsID.Visible = False
        Me.colOpenLOsID.Width = 125
        '
        'colOpenLOsCode
        '
        Me.colOpenLOsCode.HeaderText = "كود امر التحميل"
        Me.colOpenLOsCode.MinimumWidth = 6
        Me.colOpenLOsCode.Name = "colOpenLOsCode"
        Me.colOpenLOsCode.ReadOnly = True
        Me.colOpenLOsCode.Width = 125
        '
        'colOpenLOsInitiatedDateTime
        '
        Me.colOpenLOsInitiatedDateTime.HeaderText = "التاريخ"
        Me.colOpenLOsInitiatedDateTime.MinimumWidth = 6
        Me.colOpenLOsInitiatedDateTime.Name = "colOpenLOsInitiatedDateTime"
        Me.colOpenLOsInitiatedDateTime.ReadOnly = True
        Me.colOpenLOsInitiatedDateTime.Width = 125
        '
        'colOpenLOsStatus
        '
        Me.colOpenLOsStatus.HeaderText = "الحالة"
        Me.colOpenLOsStatus.MinimumWidth = 6
        Me.colOpenLOsStatus.Name = "colOpenLOsStatus"
        Me.colOpenLOsStatus.ReadOnly = True
        Me.colOpenLOsStatus.Width = 125
        '
        'colOpenLOsStatusID
        '
        Me.colOpenLOsStatusID.HeaderText = "الحالة"
        Me.colOpenLOsStatusID.MinimumWidth = 6
        Me.colOpenLOsStatusID.Name = "colOpenLOsStatusID"
        Me.colOpenLOsStatusID.ReadOnly = True
        Me.colOpenLOsStatusID.Width = 125
        '
        'colOpenLOsVehicleInfo
        '
        Me.colOpenLOsVehicleInfo.HeaderText = "السيارة"
        Me.colOpenLOsVehicleInfo.MinimumWidth = 6
        Me.colOpenLOsVehicleInfo.Name = "colOpenLOsVehicleInfo"
        Me.colOpenLOsVehicleInfo.ReadOnly = True
        Me.colOpenLOsVehicleInfo.Width = 125
        '
        'colOpenLOsSupervisor
        '
        Me.colOpenLOsSupervisor.HeaderText = "مشرف التحميل"
        Me.colOpenLOsSupervisor.MinimumWidth = 6
        Me.colOpenLOsSupervisor.Name = "colOpenLOsSupervisor"
        Me.colOpenLOsSupervisor.ReadOnly = True
        Me.colOpenLOsSupervisor.Width = 125
        '
        'pnlOpenLOs
        '
        Me.pnlOpenLOs.Controls.Add(Me.btnRemoveSR)
        Me.pnlOpenLOs.Controls.Add(Me.btnCancel)
        Me.pnlOpenLOs.Controls.Add(Me.btnExportToInvoice)
        Me.pnlOpenLOs.Controls.Add(Me.btnCloseBoard)
        Me.pnlOpenLOs.Controls.Add(Me.btnAddSelectedSRToLO)
        Me.pnlOpenLOs.Controls.Add(Me.btnSaveLO)
        Me.pnlOpenLOs.Controls.Add(Me.Button2)
        Me.pnlOpenLOs.Controls.Add(Me.btnPostLoading)
        Me.pnlOpenLOs.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlOpenLOs.Location = New System.Drawing.Point(0, 608)
        Me.pnlOpenLOs.Name = "pnlOpenLOs"
        Me.pnlOpenLOs.Size = New System.Drawing.Size(597, 145)
        Me.pnlOpenLOs.TabIndex = 3
        '
        'btnRemoveSR
        '
        Me.btnRemoveSR.Location = New System.Drawing.Point(265, 6)
        Me.btnRemoveSR.Name = "btnRemoveSR"
        Me.btnRemoveSR.Size = New System.Drawing.Size(124, 34)
        Me.btnRemoveSR.TabIndex = 7
        Me.btnRemoveSR.Text = "حذف طلب"
        Me.btnRemoveSR.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(135, 6)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(124, 34)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnExportToInvoice
        '
        Me.btnExportToInvoice.BackColor = System.Drawing.Color.Gainsboro
        Me.btnExportToInvoice.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExportToInvoice.Location = New System.Drawing.Point(406, 6)
        Me.btnExportToInvoice.Name = "btnExportToInvoice"
        Me.btnExportToInvoice.Size = New System.Drawing.Size(179, 136)
        Me.btnExportToInvoice.TabIndex = 5
        Me.btnExportToInvoice.Text = "تصدير للفاتورة"
        Me.btnExportToInvoice.UseVisualStyleBackColor = False
        '
        'btnCloseBoard
        '
        Me.btnCloseBoard.BackColor = System.Drawing.Color.Gainsboro
        Me.btnCloseBoard.Location = New System.Drawing.Point(135, 83)
        Me.btnCloseBoard.Name = "btnCloseBoard"
        Me.btnCloseBoard.Size = New System.Drawing.Size(124, 34)
        Me.btnCloseBoard.TabIndex = 4
        Me.btnCloseBoard.Text = "اغلاق"
        Me.btnCloseBoard.UseVisualStyleBackColor = False
        '
        'btnAddSelectedSRToLO
        '
        Me.btnAddSelectedSRToLO.BackColor = System.Drawing.Color.Gainsboro
        Me.btnAddSelectedSRToLO.Location = New System.Drawing.Point(3, 83)
        Me.btnAddSelectedSRToLO.Name = "btnAddSelectedSRToLO"
        Me.btnAddSelectedSRToLO.Size = New System.Drawing.Size(124, 34)
        Me.btnAddSelectedSRToLO.TabIndex = 3
        Me.btnAddSelectedSRToLO.Text = "اضافة الى قديم"
        Me.btnAddSelectedSRToLO.UseVisualStyleBackColor = False
        '
        'btnSaveLO
        '
        Me.btnSaveLO.BackColor = System.Drawing.Color.Gainsboro
        Me.btnSaveLO.Location = New System.Drawing.Point(3, 3)
        Me.btnSaveLO.Name = "btnSaveLO"
        Me.btnSaveLO.Size = New System.Drawing.Size(124, 34)
        Me.btnSaveLO.TabIndex = 0
        Me.btnSaveLO.Text = "حفظ التحميل"
        Me.btnSaveLO.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Gainsboro
        Me.Button2.Location = New System.Drawing.Point(135, 43)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(124, 34)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "طباعة"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'btnPostLoading
        '
        Me.btnPostLoading.BackColor = System.Drawing.Color.Gainsboro
        Me.btnPostLoading.Location = New System.Drawing.Point(3, 43)
        Me.btnPostLoading.Name = "btnPostLoading"
        Me.btnPostLoading.Size = New System.Drawing.Size(124, 34)
        Me.btnPostLoading.TabIndex = 0
        Me.btnPostLoading.Text = "إرسال للفوترة"
        Me.btnPostLoading.UseVisualStyleBackColor = False
        '
        'frmLoadingBoard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1782, 753)
        Me.Controls.Add(Me.pnlOverlayLoading)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frmLoadingBoard"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmLoadingBoard"
        Me.pnlOverlayLoading.ResumeLayout(False)
        Me.scLoadingMain.Panel1.ResumeLayout(False)
        Me.scLoadingMain.Panel2.ResumeLayout(False)
        CType(Me.scLoadingMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scLoadingMain.ResumeLayout(False)
        CType(Me.dgvLOs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvLoadingSR, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvLoadingSRD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTotals.ResumeLayout(False)
        Me.pnlTotals.PerformLayout()
        Me.pnlLoadingActions.ResumeLayout(False)
        CType(Me.dgvOpenedLOs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOpenLOs.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlOverlayLoading As Panel
    Friend WithEvents pnlLoadingActions As Panel
    Friend WithEvents dgvOpenedLOs As DataGridView
    Friend WithEvents pnlOpenLOs As Panel
    Friend WithEvents btnCloseBoard As Button
    Friend WithEvents btnAddSelectedSRToLO As Button
    Friend WithEvents btnSaveLO As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents btnPostLoading As Button
    Friend WithEvents scLoadingMain As SplitContainer
    Friend WithEvents dgvLOs As DataGridView
    Friend WithEvents dgvLoadingSR As DataGridView
    Friend WithEvents dgvLoadingSRD As DataGridView
    Friend WithEvents pnlTotals As Panel
    Friend WithEvents lblTotalLOQTY As Label
    Friend WithEvents lblTotalLOVolume As Label
    Friend WithEvents colLoadingSRCodes As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRtoInvoice As DataGridViewCheckBoxColumn
    Friend WithEvents colLoadingSRID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRPartners As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDates As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsID As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsCode As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsInitiatedDateTime As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsStatus As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsStatusID As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsVehicleInfo As DataGridViewTextBoxColumn
    Friend WithEvents colOpenLOsSupervisor As DataGridViewTextBoxColumn
    Friend WithEvents btnExportToInvoice As Button
    Friend WithEvents colLoadingSRDCodes As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDSRDID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingOrderDetailID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDSRID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDProductID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDProductType As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDBusinessStatusID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDBusinessStatusName As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDFulfillmentStatusID As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDFulfillmentStatusName As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDQTY As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDAvailableQTY As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDLoadedBefore As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDLoadedQTY As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDRemainingQTY As DataGridViewTextBoxColumn
    Friend WithEvents colLoadingSRDLoadedInThisLO As DataGridViewTextBoxColumn
    Friend WithEvents colLOsID As DataGridViewTextBoxColumn
    Friend WithEvents colLOsCode As DataGridViewTextBoxColumn
    Friend WithEvents colLOsDate As DataGridViewTextBoxColumn
    Friend WithEvents colLOsDriverCode As DataGridViewComboBoxColumn
    Friend WithEvents colLOsVehicale As DataGridViewComboBoxColumn
    Friend WithEvents colLOsSupervisor As DataGridViewComboBoxColumn
    Friend WithEvents colLOsNote As DataGridViewTextBoxColumn
    Friend WithEvents colLOsStoreID As DataGridViewComboBoxColumn
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnRemoveSR As Button
End Class
