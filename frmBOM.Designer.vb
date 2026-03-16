<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBOM
    Inherits System.Windows.Forms.Form

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
        Me.txtBOMCode = New System.Windows.Forms.TextBox()
        Me.txtProductName = New System.Windows.Forms.TextBox()
        Me.txtBaseUnitID = New System.Windows.Forms.TextBox()
        Me.txtCategoryID = New System.Windows.Forms.TextBox()
        Me.txtSubCategoryID = New System.Windows.Forms.TextBox()
        Me.txtProductGroupID = New System.Windows.Forms.TextBox()
        Me.txtCustomerID = New System.Windows.Forms.TextBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.الكثافة = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cboBOMVersion = New System.Windows.Forms.ComboBox()
        Me.dtpCreatedDate = New System.Windows.Forms.DateTimePicker()
        Me.cboProductionUnit = New System.Windows.Forms.ComboBox()
        Me.cboProductID = New System.Windows.Forms.ComboBox()
        Me.btnProductSearch = New System.Windows.Forms.Button()
        Me.txtCreatedBy = New System.Windows.Forms.TextBox()
        Me.txtDensity = New System.Windows.Forms.TextBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnCloseBOM = New System.Windows.Forms.Button()
        Me.btnSearchBOM = New System.Windows.Forms.Button()
        Me.btndelete = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnNewBOM = New System.Windows.Forms.Button()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblBOMStatus = New System.Windows.Forms.Label()
        Me.btnDeactivateBOM = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblTotalAvgCost = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.dgvBOMDetails = New System.Windows.Forms.DataGridView()
        Me.colDetProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetProductQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetBaseUnitID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetBaseUnitName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetAvgCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDetIsActive = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvProductSelect = New System.Windows.Forms.DataGridView()
        Me.colSelSearch = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colSelDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colSelAdd = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colSelProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colSelProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelProductQTY = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelBaseUnitID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelBaseUnitName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelAvgCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSelIsActive = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.txtStoreID = New System.Windows.Forms.TextBox()
        Me.txtProductionBaseQTY = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.dgvBOMDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvProductSelect, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtBOMCode
        '
        Me.txtBOMCode.Enabled = False
        Me.txtBOMCode.Location = New System.Drawing.Point(10, 8)
        Me.txtBOMCode.Name = "txtBOMCode"
        Me.txtBOMCode.Size = New System.Drawing.Size(147, 24)
        Me.txtBOMCode.TabIndex = 53
        '
        'txtProductName
        '
        Me.txtProductName.Enabled = False
        Me.txtProductName.Location = New System.Drawing.Point(10, 128)
        Me.txtProductName.Name = "txtProductName"
        Me.txtProductName.Size = New System.Drawing.Size(147, 24)
        Me.txtProductName.TabIndex = 53
        '
        'txtBaseUnitID
        '
        Me.txtBaseUnitID.Enabled = False
        Me.txtBaseUnitID.Location = New System.Drawing.Point(10, 158)
        Me.txtBaseUnitID.Name = "txtBaseUnitID"
        Me.txtBaseUnitID.Size = New System.Drawing.Size(147, 24)
        Me.txtBaseUnitID.TabIndex = 53
        Me.txtBaseUnitID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtCategoryID
        '
        Me.txtCategoryID.Enabled = False
        Me.txtCategoryID.Location = New System.Drawing.Point(10, 248)
        Me.txtCategoryID.Name = "txtCategoryID"
        Me.txtCategoryID.Size = New System.Drawing.Size(147, 24)
        Me.txtCategoryID.TabIndex = 53
        Me.txtCategoryID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtSubCategoryID
        '
        Me.txtSubCategoryID.Enabled = False
        Me.txtSubCategoryID.Location = New System.Drawing.Point(10, 278)
        Me.txtSubCategoryID.Name = "txtSubCategoryID"
        Me.txtSubCategoryID.Size = New System.Drawing.Size(147, 24)
        Me.txtSubCategoryID.TabIndex = 53
        Me.txtSubCategoryID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtProductGroupID
        '
        Me.txtProductGroupID.Enabled = False
        Me.txtProductGroupID.Location = New System.Drawing.Point(10, 308)
        Me.txtProductGroupID.Name = "txtProductGroupID"
        Me.txtProductGroupID.Size = New System.Drawing.Size(147, 24)
        Me.txtProductGroupID.TabIndex = 53
        Me.txtProductGroupID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtCustomerID
        '
        Me.txtCustomerID.Location = New System.Drawing.Point(10, 375)
        Me.txtCustomerID.Name = "txtCustomerID"
        Me.txtCustomerID.Size = New System.Drawing.Size(147, 24)
        Me.txtCustomerID.TabIndex = 53
        Me.txtCustomerID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(10, 405)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(147, 145)
        Me.txtNotes.TabIndex = 3
        Me.txtNotes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Label15)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.الكثافة)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.cboBOMVersion)
        Me.Panel2.Controls.Add(Me.dtpCreatedDate)
        Me.Panel2.Controls.Add(Me.cboProductionUnit)
        Me.Panel2.Controls.Add(Me.cboProductID)
        Me.Panel2.Controls.Add(Me.btnProductSearch)
        Me.Panel2.Controls.Add(Me.txtCreatedBy)
        Me.Panel2.Controls.Add(Me.txtNotes)
        Me.Panel2.Controls.Add(Me.txtBOMCode)
        Me.Panel2.Controls.Add(Me.txtProductName)
        Me.Panel2.Controls.Add(Me.txtBaseUnitID)
        Me.Panel2.Controls.Add(Me.txtCategoryID)
        Me.Panel2.Controls.Add(Me.txtDensity)
        Me.Panel2.Controls.Add(Me.txtSubCategoryID)
        Me.Panel2.Controls.Add(Me.txtCustomerID)
        Me.Panel2.Controls.Add(Me.txtProductGroupID)
        Me.Panel2.Location = New System.Drawing.Point(809, 6)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(305, 646)
        Me.Panel2.TabIndex = 55
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(206, 560)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(94, 17)
        Me.Label15.TabIndex = 78
        Me.Label15.Text = "انشأت بواسطة"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(255, 72)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(47, 17)
        Me.Label5.TabIndex = 65
        Me.Label5.Text = "الاصدار"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(223, 43)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 17)
        Me.Label3.TabIndex = 66
        Me.Label3.Text = "تاريخ الانشاء"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(214, 223)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 17)
        Me.Label4.TabIndex = 67
        Me.Label4.Text = "وحدة التصنيع"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(243, 403)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(59, 17)
        Me.Label13.TabIndex = 68
        Me.Label13.Text = "ملاحظات"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(162, 313)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(140, 17)
        Me.Label11.TabIndex = 69
        Me.Label11.Text = "مجموعة الصنف المصنع"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(222, 283)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(80, 17)
        Me.Label10.TabIndex = 70
        Me.Label10.Text = "الفئة الفرعية"
        '
        'الكثافة
        '
        Me.الكثافة.AutoSize = True
        Me.الكثافة.Location = New System.Drawing.Point(257, 351)
        Me.الكثافة.Name = "الكثافة"
        Me.الكثافة.Size = New System.Drawing.Size(45, 17)
        Me.الكثافة.TabIndex = 71
        Me.الكثافة.Text = "الكثافة"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(205, 380)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(97, 17)
        Me.Label12.TabIndex = 71
        Me.Label12.Text = "مصنعة لمصلحة"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(191, 253)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(111, 17)
        Me.Label8.TabIndex = 72
        Me.Label8.Text = "فئة الصنف المصنع"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(180, 163)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(122, 17)
        Me.Label7.TabIndex = 74
        Me.Label7.Text = "وحدة الصنف المصنع"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(188, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(114, 17)
        Me.Label1.TabIndex = 75
        Me.Label1.Text = "كود معادلة التصنيع"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(183, 133)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(119, 17)
        Me.Label6.TabIndex = 76
        Me.Label6.Text = "اسم الصنف المصنع"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(190, 103)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(112, 17)
        Me.Label2.TabIndex = 77
        Me.Label2.Text = "كود الصنف المصنع"
        '
        'cboBOMVersion
        '
        Me.cboBOMVersion.FormattingEnabled = True
        Me.cboBOMVersion.Location = New System.Drawing.Point(10, 67)
        Me.cboBOMVersion.Name = "cboBOMVersion"
        Me.cboBOMVersion.Size = New System.Drawing.Size(147, 24)
        Me.cboBOMVersion.TabIndex = 64
        '
        'dtpCreatedDate
        '
        Me.dtpCreatedDate.Enabled = False
        Me.dtpCreatedDate.Location = New System.Drawing.Point(10, 38)
        Me.dtpCreatedDate.Name = "dtpCreatedDate"
        Me.dtpCreatedDate.Size = New System.Drawing.Size(147, 24)
        Me.dtpCreatedDate.TabIndex = 63
        '
        'cboProductionUnit
        '
        Me.cboProductionUnit.Enabled = False
        Me.cboProductionUnit.FormattingEnabled = True
        Me.cboProductionUnit.Location = New System.Drawing.Point(10, 218)
        Me.cboProductionUnit.Name = "cboProductionUnit"
        Me.cboProductionUnit.Size = New System.Drawing.Size(147, 24)
        Me.cboProductionUnit.TabIndex = 2
        '
        'cboProductID
        '
        Me.cboProductID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProductID.FormattingEnabled = True
        Me.cboProductID.Location = New System.Drawing.Point(10, 98)
        Me.cboProductID.Name = "cboProductID"
        Me.cboProductID.Size = New System.Drawing.Size(97, 24)
        Me.cboProductID.TabIndex = 0
        '
        'btnProductSearch
        '
        Me.btnProductSearch.Location = New System.Drawing.Point(113, 98)
        Me.btnProductSearch.Name = "btnProductSearch"
        Me.btnProductSearch.Size = New System.Drawing.Size(44, 24)
        Me.btnProductSearch.TabIndex = 61
        Me.btnProductSearch.Text = "Find"
        Me.btnProductSearch.UseVisualStyleBackColor = True
        '
        'txtCreatedBy
        '
        Me.txtCreatedBy.Location = New System.Drawing.Point(10, 556)
        Me.txtCreatedBy.Name = "txtCreatedBy"
        Me.txtCreatedBy.Size = New System.Drawing.Size(147, 24)
        Me.txtCreatedBy.TabIndex = 53
        '
        'txtDensity
        '
        Me.txtDensity.Enabled = False
        Me.txtDensity.Location = New System.Drawing.Point(10, 346)
        Me.txtDensity.Name = "txtDensity"
        Me.txtDensity.Size = New System.Drawing.Size(147, 24)
        Me.txtDensity.TabIndex = 53
        Me.txtDensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.LightGray
        Me.btnPrint.Location = New System.Drawing.Point(3, 553)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 41)
        Me.btnPrint.TabIndex = 52
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnCloseBOM
        '
        Me.btnCloseBOM.BackColor = System.Drawing.Color.LightGray
        Me.btnCloseBOM.Location = New System.Drawing.Point(3, 600)
        Me.btnCloseBOM.Name = "btnCloseBOM"
        Me.btnCloseBOM.Size = New System.Drawing.Size(100, 37)
        Me.btnCloseBOM.TabIndex = 48
        Me.btnCloseBOM.Text = "Close"
        Me.btnCloseBOM.UseVisualStyleBackColor = False
        '
        'btnSearchBOM
        '
        Me.btnSearchBOM.BackColor = System.Drawing.Color.LightGray
        Me.btnSearchBOM.Location = New System.Drawing.Point(3, 510)
        Me.btnSearchBOM.Name = "btnSearchBOM"
        Me.btnSearchBOM.Size = New System.Drawing.Size(100, 37)
        Me.btnSearchBOM.TabIndex = 51
        Me.btnSearchBOM.TabStop = False
        Me.btnSearchBOM.Text = "Search"
        Me.btnSearchBOM.UseVisualStyleBackColor = False
        '
        'btndelete
        '
        Me.btndelete.BackColor = System.Drawing.Color.LightGray
        Me.btndelete.Location = New System.Drawing.Point(3, 418)
        Me.btndelete.Name = "btndelete"
        Me.btndelete.Size = New System.Drawing.Size(100, 37)
        Me.btndelete.TabIndex = 50
        Me.btndelete.TabStop = False
        Me.btndelete.Text = "Delete"
        Me.btndelete.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.LightGray
        Me.btnSave.Location = New System.Drawing.Point(3, 375)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 37)
        Me.btnSave.TabIndex = 47
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btnNewBOM
        '
        Me.btnNewBOM.BackColor = System.Drawing.Color.LightGray
        Me.btnNewBOM.Location = New System.Drawing.Point(3, 330)
        Me.btnNewBOM.Name = "btnNewBOM"
        Me.btnNewBOM.Size = New System.Drawing.Size(100, 37)
        Me.btnNewBOM.TabIndex = 49
        Me.btnNewBOM.TabStop = False
        Me.btnNewBOM.Text = "New"
        Me.btnNewBOM.UseVisualStyleBackColor = False
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Enabled = False
        Me.chkIsActive.Location = New System.Drawing.Point(34, 5)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(18, 17)
        Me.chkIsActive.TabIndex = 55
        Me.chkIsActive.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblBOMStatus)
        Me.Panel1.Controls.Add(Me.chkIsActive)
        Me.Panel1.Controls.Add(Me.btnDeactivateBOM)
        Me.Panel1.Controls.Add(Me.btnNewBOM)
        Me.Panel1.Controls.Add(Me.btnSave)
        Me.Panel1.Controls.Add(Me.btndelete)
        Me.Panel1.Controls.Add(Me.btnSearchBOM)
        Me.Panel1.Controls.Add(Me.btnCloseBOM)
        Me.Panel1.Controls.Add(Me.btnPrint)
        Me.Panel1.Location = New System.Drawing.Point(1120, 6)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(112, 645)
        Me.Panel1.TabIndex = 55
        '
        'lblBOMStatus
        '
        Me.lblBOMStatus.AutoSize = True
        Me.lblBOMStatus.Location = New System.Drawing.Point(58, 5)
        Me.lblBOMStatus.Name = "lblBOMStatus"
        Me.lblBOMStatus.Size = New System.Drawing.Size(45, 17)
        Me.lblBOMStatus.TabIndex = 62
        Me.lblBOMStatus.Text = "نشطة"
        '
        'btnDeactivateBOM
        '
        Me.btnDeactivateBOM.BackColor = System.Drawing.Color.LightGray
        Me.btnDeactivateBOM.Location = New System.Drawing.Point(3, 287)
        Me.btnDeactivateBOM.Name = "btnDeactivateBOM"
        Me.btnDeactivateBOM.Size = New System.Drawing.Size(100, 37)
        Me.btnDeactivateBOM.TabIndex = 49
        Me.btnDeactivateBOM.TabStop = False
        Me.btnDeactivateBOM.Text = "تعطيل BOM"
        Me.btnDeactivateBOM.UseVisualStyleBackColor = False
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.lblTotalAvgCost)
        Me.Panel3.Controls.Add(Me.Label14)
        Me.Panel3.Controls.Add(Me.dgvBOMDetails)
        Me.Panel3.Controls.Add(Me.dgvProductSelect)
        Me.Panel3.Controls.Add(Me.txtStoreID)
        Me.Panel3.Controls.Add(Me.txtProductionBaseQTY)
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Location = New System.Drawing.Point(6, 6)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(797, 646)
        Me.Panel3.TabIndex = 65
        '
        'lblTotalAvgCost
        '
        Me.lblTotalAvgCost.AutoSize = True
        Me.lblTotalAvgCost.Location = New System.Drawing.Point(196, 565)
        Me.lblTotalAvgCost.Name = "lblTotalAvgCost"
        Me.lblTotalAvgCost.Size = New System.Drawing.Size(0, 17)
        Me.lblTotalAvgCost.TabIndex = 3
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(309, 562)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(331, 17)
        Me.Label14.TabIndex = 3
        Me.Label14.Text = "متوسط تكلفة متر مكعب من المتنج حسب قائمة الاصناف"
        '
        'dgvBOMDetails
        '
        Me.dgvBOMDetails.AllowUserToAddRows = False
        Me.dgvBOMDetails.AllowUserToDeleteRows = False
        Me.dgvBOMDetails.AllowUserToOrderColumns = True
        Me.dgvBOMDetails.AllowUserToResizeRows = False
        Me.dgvBOMDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvBOMDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBOMDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetProductID, Me.colDetProductCode, Me.colDetProductName, Me.colDetProductQTY, Me.colDetBaseUnitID, Me.colDetBaseUnitName, Me.colDetAvgCost, Me.colDetIsActive})
        Me.dgvBOMDetails.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke
        Me.dgvBOMDetails.Location = New System.Drawing.Point(9, 97)
        Me.dgvBOMDetails.MultiSelect = False
        Me.dgvBOMDetails.Name = "dgvBOMDetails"
        Me.dgvBOMDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvBOMDetails.RowHeadersVisible = False
        Me.dgvBOMDetails.RowHeadersWidth = 51
        Me.dgvBOMDetails.RowTemplate.Height = 26
        Me.dgvBOMDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvBOMDetails.Size = New System.Drawing.Size(778, 450)
        Me.dgvBOMDetails.TabIndex = 1
        '
        'colDetProductID
        '
        Me.colDetProductID.HeaderText = "رقم الصنف"
        Me.colDetProductID.MinimumWidth = 6
        Me.colDetProductID.Name = "colDetProductID"
        Me.colDetProductID.ReadOnly = True
        Me.colDetProductID.Visible = False
        '
        'colDetProductCode
        '
        Me.colDetProductCode.HeaderText = "كود الصنف"
        Me.colDetProductCode.MinimumWidth = 6
        Me.colDetProductCode.Name = "colDetProductCode"
        '
        'colDetProductName
        '
        Me.colDetProductName.HeaderText = "اسم الصنف"
        Me.colDetProductName.MinimumWidth = 6
        Me.colDetProductName.Name = "colDetProductName"
        '
        'colDetProductQTY
        '
        Me.colDetProductQTY.HeaderText = "الكمية"
        Me.colDetProductQTY.MinimumWidth = 6
        Me.colDetProductQTY.Name = "colDetProductQTY"
        '
        'colDetBaseUnitID
        '
        Me.colDetBaseUnitID.HeaderText = "الوحدة"
        Me.colDetBaseUnitID.MinimumWidth = 6
        Me.colDetBaseUnitID.Name = "colDetBaseUnitID"
        Me.colDetBaseUnitID.Visible = False
        '
        'colDetBaseUnitName
        '
        Me.colDetBaseUnitName.HeaderText = "الوحدة"
        Me.colDetBaseUnitName.MinimumWidth = 6
        Me.colDetBaseUnitName.Name = "colDetBaseUnitName"
        '
        'colDetAvgCost
        '
        Me.colDetAvgCost.HeaderText = "متوسط التكلفة"
        Me.colDetAvgCost.MinimumWidth = 6
        Me.colDetAvgCost.Name = "colDetAvgCost"
        '
        'colDetIsActive
        '
        Me.colDetIsActive.DataPropertyName = "IsActive"
        Me.colDetIsActive.HeaderText = "نشط"
        Me.colDetIsActive.MinimumWidth = 6
        Me.colDetIsActive.Name = "colDetIsActive"
        '
        'dgvProductSelect
        '
        Me.dgvProductSelect.AllowUserToAddRows = False
        Me.dgvProductSelect.AllowUserToDeleteRows = False
        Me.dgvProductSelect.AllowUserToOrderColumns = True
        Me.dgvProductSelect.AllowUserToResizeRows = False
        Me.dgvProductSelect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvProductSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProductSelect.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colSelSearch, Me.colSelDelete, Me.colSelAdd, Me.colSelProductID, Me.colSelProductCode, Me.colSelProductName, Me.colSelProductQTY, Me.colSelBaseUnitID, Me.colSelBaseUnitName, Me.colSelAvgCost, Me.colSelIsActive})
        Me.dgvProductSelect.Location = New System.Drawing.Point(9, 8)
        Me.dgvProductSelect.MultiSelect = False
        Me.dgvProductSelect.Name = "dgvProductSelect"
        Me.dgvProductSelect.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvProductSelect.RowHeadersVisible = False
        Me.dgvProductSelect.RowHeadersWidth = 51
        Me.dgvProductSelect.RowTemplate.Height = 26
        Me.dgvProductSelect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvProductSelect.Size = New System.Drawing.Size(778, 83)
        Me.dgvProductSelect.TabIndex = 0
        '
        'colSelSearch
        '
        Me.colSelSearch.HeaderText = ""
        Me.colSelSearch.MinimumWidth = 6
        Me.colSelSearch.Name = "colSelSearch"
        Me.colSelSearch.Text = "Search"
        Me.colSelSearch.UseColumnTextForButtonValue = True
        '
        'colSelDelete
        '
        Me.colSelDelete.DataPropertyName = "IsActive"
        Me.colSelDelete.HeaderText = ""
        Me.colSelDelete.MinimumWidth = 6
        Me.colSelDelete.Name = "colSelDelete"
        Me.colSelDelete.Text = "Delete"
        Me.colSelDelete.UseColumnTextForButtonValue = True
        '
        'colSelAdd
        '
        Me.colSelAdd.HeaderText = ""
        Me.colSelAdd.MinimumWidth = 6
        Me.colSelAdd.Name = "colSelAdd"
        Me.colSelAdd.Text = "Add"
        Me.colSelAdd.UseColumnTextForButtonValue = True
        '
        'colSelProductID
        '
        Me.colSelProductID.HeaderText = "Product ID"
        Me.colSelProductID.MinimumWidth = 6
        Me.colSelProductID.Name = "colSelProductID"
        Me.colSelProductID.Visible = False
        '
        'colSelProductCode
        '
        Me.colSelProductCode.HeaderText = "كود الصنف"
        Me.colSelProductCode.MinimumWidth = 6
        Me.colSelProductCode.Name = "colSelProductCode"
        Me.colSelProductCode.ReadOnly = True
        Me.colSelProductCode.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colSelProductCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colSelProductName
        '
        Me.colSelProductName.HeaderText = "اسم الصنف"
        Me.colSelProductName.MinimumWidth = 6
        Me.colSelProductName.Name = "colSelProductName"
        Me.colSelProductName.ReadOnly = True
        '
        'colSelProductQTY
        '
        Me.colSelProductQTY.HeaderText = "الكمية"
        Me.colSelProductQTY.MinimumWidth = 6
        Me.colSelProductQTY.Name = "colSelProductQTY"
        '
        'colSelBaseUnitID
        '
        Me.colSelBaseUnitID.HeaderText = "الوحدة"
        Me.colSelBaseUnitID.MinimumWidth = 6
        Me.colSelBaseUnitID.Name = "colSelBaseUnitID"
        Me.colSelBaseUnitID.ReadOnly = True
        Me.colSelBaseUnitID.Visible = False
        '
        'colSelBaseUnitName
        '
        Me.colSelBaseUnitName.HeaderText = "الوحدة"
        Me.colSelBaseUnitName.MinimumWidth = 6
        Me.colSelBaseUnitName.Name = "colSelBaseUnitName"
        '
        'colSelAvgCost
        '
        Me.colSelAvgCost.HeaderText = "متوسط التكلفة"
        Me.colSelAvgCost.MinimumWidth = 6
        Me.colSelAvgCost.Name = "colSelAvgCost"
        Me.colSelAvgCost.ReadOnly = True
        '
        'colSelIsActive
        '
        Me.colSelIsActive.HeaderText = "نشط"
        Me.colSelIsActive.MinimumWidth = 6
        Me.colSelIsActive.Name = "colSelIsActive"
        Me.colSelIsActive.ReadOnly = True
        '
        'txtStoreID
        '
        Me.txtStoreID.Enabled = False
        Me.txtStoreID.Location = New System.Drawing.Point(604, 186)
        Me.txtStoreID.Name = "txtStoreID"
        Me.txtStoreID.Size = New System.Drawing.Size(147, 24)
        Me.txtStoreID.TabIndex = 53
        Me.txtStoreID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtStoreID.Visible = False
        '
        'txtProductionBaseQTY
        '
        Me.txtProductionBaseQTY.Enabled = False
        Me.txtProductionBaseQTY.Location = New System.Drawing.Point(632, 220)
        Me.txtProductionBaseQTY.Name = "txtProductionBaseQTY"
        Me.txtProductionBaseQTY.Size = New System.Drawing.Size(44, 24)
        Me.txtProductionBaseQTY.TabIndex = 1
        Me.txtProductionBaseQTY.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(601, 193)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(124, 17)
        Me.Label9.TabIndex = 73
        Me.Label9.Tag = ""
        Me.Label9.Text = "مخزن المنتج المصنع"
        Me.Label9.Visible = False
        '
        'frmBOM
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1237, 653)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.MinimumSize = New System.Drawing.Size(1200, 700)
        Me.Name = "frmBOM"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmBOM"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.dgvBOMDetails, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvProductSelect, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtBOMCode As TextBox
    Friend WithEvents txtProductName As TextBox
    Friend WithEvents txtBaseUnitID As TextBox
    Friend WithEvents txtCategoryID As TextBox
    Friend WithEvents txtSubCategoryID As TextBox
    Friend WithEvents txtProductGroupID As TextBox
    Friend WithEvents txtCustomerID As TextBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents cboProductID As ComboBox
    Friend WithEvents btnProductSearch As Button
    Friend WithEvents txtCreatedBy As TextBox
    Friend WithEvents cboProductionUnit As ComboBox
    Friend WithEvents dtpCreatedDate As DateTimePicker
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnCloseBOM As Button
    Friend WithEvents btnSearchBOM As Button
    Friend WithEvents btndelete As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnNewBOM As Button
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents cboBOMVersion As ComboBox
    Friend WithEvents btnDeactivateBOM As Button
    Friend WithEvents lblBOMStatus As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents dgvBOMDetails As DataGridView
    Friend WithEvents dgvProductSelect As DataGridView
    Friend WithEvents Label15 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents الكثافة As Label
    Friend WithEvents txtDensity As TextBox
    Friend WithEvents lblTotalAvgCost As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents colDetProductID As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductCode As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductName As DataGridViewTextBoxColumn
    Friend WithEvents colDetProductQTY As DataGridViewTextBoxColumn
    Friend WithEvents colDetBaseUnitID As DataGridViewTextBoxColumn
    Friend WithEvents colDetBaseUnitName As DataGridViewTextBoxColumn
    Friend WithEvents colDetAvgCost As DataGridViewTextBoxColumn
    Friend WithEvents colDetIsActive As DataGridViewTextBoxColumn
    Friend WithEvents colSelSearch As DataGridViewButtonColumn
    Friend WithEvents colSelDelete As DataGridViewButtonColumn
    Friend WithEvents colSelAdd As DataGridViewButtonColumn
    Friend WithEvents colSelProductID As DataGridViewTextBoxColumn
    Friend WithEvents colSelProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colSelProductName As DataGridViewTextBoxColumn
    Friend WithEvents colSelProductQTY As DataGridViewTextBoxColumn
    Friend WithEvents colSelBaseUnitID As DataGridViewTextBoxColumn
    Friend WithEvents colSelBaseUnitName As DataGridViewTextBoxColumn
    Friend WithEvents colSelAvgCost As DataGridViewTextBoxColumn
    Friend WithEvents colSelIsActive As DataGridViewCheckBoxColumn
    Friend WithEvents Label9 As Label
    Friend WithEvents txtProductionBaseQTY As TextBox
    Friend WithEvents txtStoreID As TextBox
End Class
