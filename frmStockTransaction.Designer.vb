<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmStockTransaction
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.dgvTransfersList = New System.Windows.Forms.DataGridView()
        Me.txtTransactionCode = New System.Windows.Forms.TextBox()
        Me.dtpTransactionDate = New System.Windows.Forms.DateTimePicker()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblSourceStore = New System.Windows.Forms.Label()
        Me.lblTargetStore = New System.Windows.Forms.Label()
        Me.cboTargetStore = New System.Windows.Forms.ComboBox()
        Me.cboSourceStore = New System.Windows.Forms.ComboBox()
        Me.dgvTransferDetails = New System.Windows.Forms.DataGridView()
        Me.colProductSearch = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.colProductID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colProductCode = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colProductName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSourceOnHand = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTransferQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colUnit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colEmp2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTargetAfter = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colLineNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colDelete = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnCancelReceive = New System.Windows.Forms.Button()
        Me.btnTransactionSearch = New System.Windows.Forms.Button()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.grpReceiveMode = New System.Windows.Forms.GroupBox()
        Me.rdoScrapRecieve = New System.Windows.Forms.RadioButton()
        Me.rdoPostSales = New System.Windows.Forms.RadioButton()
        Me.rdoPRTSend = New System.Windows.Forms.RadioButton()
        Me.rdoReceiveTransaction = New System.Windows.Forms.RadioButton()
        Me.rdoSendTransaction = New System.Windows.Forms.RadioButton()
        Me.rdoReturnReceive = New System.Windows.Forms.RadioButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.rdoCuttingReceive = New System.Windows.Forms.RadioButton()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.rdoProductionReceive = New System.Windows.Forms.RadioButton()
        Me.rdoPurchaseReceive = New System.Windows.Forms.RadioButton()
        Me.Panel1.SuspendLayout()
        CType(Me.dgvTransfersList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.dgvTransferDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.grpReceiveMode.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.dgvTransfersList)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1191, 331)
        Me.Panel1.TabIndex = 0
        '
        'dgvTransfersList
        '
        Me.dgvTransfersList.AllowUserToAddRows = False
        Me.dgvTransfersList.AllowUserToDeleteRows = False
        Me.dgvTransfersList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvTransfersList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTransfersList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvTransfersList.Location = New System.Drawing.Point(0, 0)
        Me.dgvTransfersList.Name = "dgvTransfersList"
        Me.dgvTransfersList.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvTransfersList.RowHeadersWidth = 51
        Me.dgvTransfersList.RowTemplate.Height = 26
        Me.dgvTransfersList.Size = New System.Drawing.Size(1191, 331)
        Me.dgvTransfersList.TabIndex = 1
        '
        'txtTransactionCode
        '
        Me.txtTransactionCode.Location = New System.Drawing.Point(5, 34)
        Me.txtTransactionCode.Name = "txtTransactionCode"
        Me.txtTransactionCode.Size = New System.Drawing.Size(169, 24)
        Me.txtTransactionCode.TabIndex = 8
        '
        'dtpTransactionDate
        '
        Me.dtpTransactionDate.Location = New System.Drawing.Point(5, 8)
        Me.dtpTransactionDate.Name = "dtpTransactionDate"
        Me.dtpTransactionDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dtpTransactionDate.Size = New System.Drawing.Size(169, 24)
        Me.dtpTransactionDate.TabIndex = 3
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblSourceStore)
        Me.Panel2.Controls.Add(Me.lblTargetStore)
        Me.Panel2.Controls.Add(Me.cboTargetStore)
        Me.Panel2.Controls.Add(Me.cboSourceStore)
        Me.Panel2.Controls.Add(Me.dgvTransferDetails)
        Me.Panel2.Controls.Add(Me.Panel5)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 331)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1191, 422)
        Me.Panel2.TabIndex = 1
        '
        'lblSourceStore
        '
        Me.lblSourceStore.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblSourceStore.AutoSize = True
        Me.lblSourceStore.Location = New System.Drawing.Point(657, 58)
        Me.lblSourceStore.Name = "lblSourceStore"
        Me.lblSourceStore.Size = New System.Drawing.Size(54, 17)
        Me.lblSourceStore.TabIndex = 6
        Me.lblSourceStore.Text = "المرسل"
        '
        'lblTargetStore
        '
        Me.lblTargetStore.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblTargetStore.AutoSize = True
        Me.lblTargetStore.Location = New System.Drawing.Point(206, 58)
        Me.lblTargetStore.Name = "lblTargetStore"
        Me.lblTargetStore.Size = New System.Drawing.Size(59, 17)
        Me.lblTargetStore.TabIndex = 4
        Me.lblTargetStore.Text = "المستلم"
        '
        'cboTargetStore
        '
        Me.cboTargetStore.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.cboTargetStore.FormattingEnabled = True
        Me.cboTargetStore.Location = New System.Drawing.Point(640, 74)
        Me.cboTargetStore.Name = "cboTargetStore"
        Me.cboTargetStore.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboTargetStore.Size = New System.Drawing.Size(92, 24)
        Me.cboTargetStore.TabIndex = 5
        '
        'cboSourceStore
        '
        Me.cboSourceStore.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.cboSourceStore.FormattingEnabled = True
        Me.cboSourceStore.Location = New System.Drawing.Point(190, 74)
        Me.cboSourceStore.Name = "cboSourceStore"
        Me.cboSourceStore.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboSourceStore.Size = New System.Drawing.Size(92, 24)
        Me.cboSourceStore.TabIndex = 7
        '
        'dgvTransferDetails
        '
        Me.dgvTransferDetails.AllowUserToAddRows = False
        Me.dgvTransferDetails.AllowUserToDeleteRows = False
        Me.dgvTransferDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvTransferDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTransferDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colProductSearch, Me.colProductID, Me.colProductCode, Me.colProductType, Me.colProductName, Me.colSourceOnHand, Me.colemp, Me.colTransferQty, Me.colUnit, Me.colEmp2, Me.colTargetAfter, Me.colLineNotes, Me.colDelete})
        Me.dgvTransferDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvTransferDetails.Location = New System.Drawing.Point(0, 54)
        Me.dgvTransferDetails.Name = "dgvTransferDetails"
        Me.dgvTransferDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvTransferDetails.RowHeadersVisible = False
        Me.dgvTransferDetails.RowHeadersWidth = 51
        Me.dgvTransferDetails.RowTemplate.Height = 26
        Me.dgvTransferDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTransferDetails.Size = New System.Drawing.Size(1191, 368)
        Me.dgvTransferDetails.TabIndex = 0
        '
        'colProductSearch
        '
        Me.colProductSearch.HeaderText = "بحث"
        Me.colProductSearch.MinimumWidth = 6
        Me.colProductSearch.Name = "colProductSearch"
        Me.colProductSearch.UseColumnTextForButtonValue = True
        '
        'colProductID
        '
        Me.colProductID.HeaderText = "Product ID"
        Me.colProductID.MinimumWidth = 6
        Me.colProductID.Name = "colProductID"
        Me.colProductID.Visible = False
        '
        'colProductCode
        '
        Me.colProductCode.DataPropertyName = "ProductCode"
        Me.colProductCode.HeaderText = "كود الصنف"
        Me.colProductCode.MinimumWidth = 6
        Me.colProductCode.Name = "colProductCode"
        Me.colProductCode.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colProductCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colProductType
        '
        Me.colProductType.HeaderText = "النوع"
        Me.colProductType.MinimumWidth = 6
        Me.colProductType.Name = "colProductType"
        '
        'colProductName
        '
        Me.colProductName.DataPropertyName = "ProductName"
        Me.colProductName.HeaderText = "اسم الصنف"
        Me.colProductName.MinimumWidth = 6
        Me.colProductName.Name = "colProductName"
        Me.colProductName.ReadOnly = True
        '
        'colSourceOnHand
        '
        Me.colSourceOnHand.DataPropertyName = "SourceOnHand"
        Me.colSourceOnHand.HeaderText = "الكمية المتاحة"
        Me.colSourceOnHand.MinimumWidth = 6
        Me.colSourceOnHand.Name = "colSourceOnHand"
        Me.colSourceOnHand.ReadOnly = True
        '
        'colemp
        '
        Me.colemp.HeaderText = ""
        Me.colemp.MinimumWidth = 6
        Me.colemp.Name = "colemp"
        Me.colemp.ReadOnly = True
        '
        'colTransferQty
        '
        Me.colTransferQty.DataPropertyName = "TransferQty"
        Me.colTransferQty.HeaderText = "الكمية المحولة"
        Me.colTransferQty.MinimumWidth = 6
        Me.colTransferQty.Name = "colTransferQty"
        '
        'colUnit
        '
        Me.colUnit.DataPropertyName = "UnitID"
        Me.colUnit.HeaderText = "الوحدة"
        Me.colUnit.MinimumWidth = 6
        Me.colUnit.Name = "colUnit"
        Me.colUnit.ReadOnly = True
        '
        'colEmp2
        '
        Me.colEmp2.HeaderText = ""
        Me.colEmp2.MinimumWidth = 6
        Me.colEmp2.Name = "colEmp2"
        Me.colEmp2.ReadOnly = True
        '
        'colTargetAfter
        '
        Me.colTargetAfter.DataPropertyName = "TargetAfter"
        Me.colTargetAfter.HeaderText = "الكمية المتاحة"
        Me.colTargetAfter.MinimumWidth = 6
        Me.colTargetAfter.Name = "colTargetAfter"
        Me.colTargetAfter.ReadOnly = True
        '
        'colLineNotes
        '
        Me.colLineNotes.DataPropertyName = "LineNotes"
        Me.colLineNotes.HeaderText = "ملاحظات سطرية"
        Me.colLineNotes.MinimumWidth = 6
        Me.colLineNotes.Name = "colLineNotes"
        '
        'colDelete
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.colDelete.DefaultCellStyle = DataGridViewCellStyle1
        Me.colDelete.HeaderText = "حذف"
        Me.colDelete.MinimumWidth = 6
        Me.colDelete.Name = "colDelete"
        Me.colDelete.Text = "➖"
        Me.colDelete.UseColumnTextForButtonValue = True
        '
        'Panel5
        '
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1191, 54)
        Me.Panel5.TabIndex = 8
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.txtTransactionCode)
        Me.Panel4.Controls.Add(Me.dtpTransactionDate)
        Me.Panel4.Controls.Add(Me.btnCancel)
        Me.Panel4.Controls.Add(Me.btnCancelReceive)
        Me.Panel4.Controls.Add(Me.btnTransactionSearch)
        Me.Panel4.Controls.Add(Me.btnSend)
        Me.Panel4.Controls.Add(Me.btnNew)
        Me.Panel4.Controls.Add(Me.btnClose)
        Me.Panel4.Controls.Add(Me.grpReceiveMode)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel4.Location = New System.Drawing.Point(1191, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(191, 753)
        Me.Panel4.TabIndex = 9
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(39, 633)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(128, 53)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnCancelReceive
        '
        Me.btnCancelReceive.Location = New System.Drawing.Point(39, 695)
        Me.btnCancelReceive.Name = "btnCancelReceive"
        Me.btnCancelReceive.Size = New System.Drawing.Size(128, 53)
        Me.btnCancelReceive.TabIndex = 17
        Me.btnCancelReceive.Text = "الغاء الاستلام"
        Me.btnCancelReceive.UseVisualStyleBackColor = True
        '
        'btnTransactionSearch
        '
        Me.btnTransactionSearch.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTransactionSearch.Location = New System.Drawing.Point(39, 385)
        Me.btnTransactionSearch.Name = "btnTransactionSearch"
        Me.btnTransactionSearch.Size = New System.Drawing.Size(128, 53)
        Me.btnTransactionSearch.TabIndex = 12
        Me.btnTransactionSearch.Text = "بحث"
        Me.btnTransactionSearch.UseVisualStyleBackColor = True
        '
        'btnSend
        '
        Me.btnSend.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSend.Location = New System.Drawing.Point(39, 509)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(128, 53)
        Me.btnSend.TabIndex = 10
        Me.btnSend.Text = "ارسال"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNew.Location = New System.Drawing.Point(39, 447)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(128, 53)
        Me.btnNew.TabIndex = 9
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(39, 571)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(128, 53)
        Me.btnClose.TabIndex = 8
        Me.btnClose.Text = "اغلاق"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'grpReceiveMode
        '
        Me.grpReceiveMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpReceiveMode.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.grpReceiveMode.Controls.Add(Me.rdoScrapRecieve)
        Me.grpReceiveMode.Controls.Add(Me.rdoPostSales)
        Me.grpReceiveMode.Controls.Add(Me.rdoPRTSend)
        Me.grpReceiveMode.Controls.Add(Me.rdoReceiveTransaction)
        Me.grpReceiveMode.Controls.Add(Me.rdoSendTransaction)
        Me.grpReceiveMode.Controls.Add(Me.rdoReturnReceive)
        Me.grpReceiveMode.Controls.Add(Me.Label4)
        Me.grpReceiveMode.Controls.Add(Me.Label9)
        Me.grpReceiveMode.Controls.Add(Me.Label1)
        Me.grpReceiveMode.Controls.Add(Me.rdoCuttingReceive)
        Me.grpReceiveMode.Controls.Add(Me.Label11)
        Me.grpReceiveMode.Controls.Add(Me.Label5)
        Me.grpReceiveMode.Controls.Add(Me.Label3)
        Me.grpReceiveMode.Controls.Add(Me.Label10)
        Me.grpReceiveMode.Controls.Add(Me.Label8)
        Me.grpReceiveMode.Controls.Add(Me.Label2)
        Me.grpReceiveMode.Controls.Add(Me.rdoProductionReceive)
        Me.grpReceiveMode.Controls.Add(Me.rdoPurchaseReceive)
        Me.grpReceiveMode.Location = New System.Drawing.Point(4, 64)
        Me.grpReceiveMode.Name = "grpReceiveMode"
        Me.grpReceiveMode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.grpReceiveMode.Size = New System.Drawing.Size(181, 306)
        Me.grpReceiveMode.TabIndex = 20
        Me.grpReceiveMode.TabStop = False
        '
        'rdoScrapRecieve
        '
        Me.rdoScrapRecieve.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoScrapRecieve.AutoSize = True
        Me.rdoScrapRecieve.Location = New System.Drawing.Point(162, 248)
        Me.rdoScrapRecieve.Name = "rdoScrapRecieve"
        Me.rdoScrapRecieve.Size = New System.Drawing.Size(17, 16)
        Me.rdoScrapRecieve.TabIndex = 22
        Me.rdoScrapRecieve.TabStop = True
        Me.rdoScrapRecieve.UseVisualStyleBackColor = True
        '
        'rdoPostSales
        '
        Me.rdoPostSales.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoPostSales.AutoSize = True
        Me.rdoPostSales.Location = New System.Drawing.Point(162, 69)
        Me.rdoPostSales.Name = "rdoPostSales"
        Me.rdoPostSales.Size = New System.Drawing.Size(17, 16)
        Me.rdoPostSales.TabIndex = 21
        Me.rdoPostSales.TabStop = True
        Me.rdoPostSales.UseVisualStyleBackColor = True
        '
        'rdoPRTSend
        '
        Me.rdoPRTSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoPRTSend.AutoSize = True
        Me.rdoPRTSend.Location = New System.Drawing.Point(162, 45)
        Me.rdoPRTSend.Name = "rdoPRTSend"
        Me.rdoPRTSend.Size = New System.Drawing.Size(17, 16)
        Me.rdoPRTSend.TabIndex = 21
        Me.rdoPRTSend.TabStop = True
        Me.rdoPRTSend.UseVisualStyleBackColor = True
        '
        'rdoReceiveTransaction
        '
        Me.rdoReceiveTransaction.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoReceiveTransaction.AutoSize = True
        Me.rdoReceiveTransaction.Location = New System.Drawing.Point(162, 121)
        Me.rdoReceiveTransaction.Name = "rdoReceiveTransaction"
        Me.rdoReceiveTransaction.Size = New System.Drawing.Size(17, 16)
        Me.rdoReceiveTransaction.TabIndex = 20
        Me.rdoReceiveTransaction.TabStop = True
        Me.rdoReceiveTransaction.UseVisualStyleBackColor = True
        '
        'rdoSendTransaction
        '
        Me.rdoSendTransaction.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoSendTransaction.AutoSize = True
        Me.rdoSendTransaction.Location = New System.Drawing.Point(162, 13)
        Me.rdoSendTransaction.Name = "rdoSendTransaction"
        Me.rdoSendTransaction.Size = New System.Drawing.Size(17, 16)
        Me.rdoSendTransaction.TabIndex = 20
        Me.rdoSendTransaction.TabStop = True
        Me.rdoSendTransaction.UseVisualStyleBackColor = True
        '
        'rdoReturnReceive
        '
        Me.rdoReturnReceive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoReturnReceive.AutoSize = True
        Me.rdoReturnReceive.Location = New System.Drawing.Point(162, 225)
        Me.rdoReturnReceive.Name = "rdoReturnReceive"
        Me.rdoReturnReceive.Size = New System.Drawing.Size(17, 16)
        Me.rdoReturnReceive.TabIndex = 19
        Me.rdoReturnReceive.TabStop = True
        Me.rdoReturnReceive.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Firebrick
        Me.Label4.Location = New System.Drawing.Point(24, 247)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(148, 18)
        Me.Label4.TabIndex = 18
        Me.Label4.Text = "استلام  سكراب قص"
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Firebrick
        Me.Label9.Location = New System.Drawing.Point(17, 223)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(155, 18)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "استلام مرتجع مبيعات"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Firebrick
        Me.Label1.Location = New System.Drawing.Point(83, 197)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 18)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "استلام قص"
        '
        'rdoCuttingReceive
        '
        Me.rdoCuttingReceive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoCuttingReceive.AutoSize = True
        Me.rdoCuttingReceive.Location = New System.Drawing.Point(162, 199)
        Me.rdoCuttingReceive.Name = "rdoCuttingReceive"
        Me.rdoCuttingReceive.Size = New System.Drawing.Size(17, 16)
        Me.rdoCuttingReceive.TabIndex = 19
        Me.rdoCuttingReceive.TabStop = True
        Me.rdoCuttingReceive.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.Firebrick
        Me.Label11.Location = New System.Drawing.Point(63, 119)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(109, 18)
        Me.Label11.TabIndex = 18
        Me.Label11.Text = "استلام مخزني"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Firebrick
        Me.Label5.Location = New System.Drawing.Point(66, 64)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(98, 18)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "فسح مبيعات"
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Firebrick
        Me.Label3.Location = New System.Drawing.Point(5, 40)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(167, 18)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "ارسال مرتجع مشتريات"
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.Firebrick
        Me.Label10.Location = New System.Drawing.Point(66, 11)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(106, 18)
        Me.Label10.TabIndex = 18
        Me.Label10.Text = "ارسال مخزني"
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Firebrick
        Me.Label8.Location = New System.Drawing.Point(49, 145)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(123, 18)
        Me.Label8.TabIndex = 18
        Me.Label8.Text = "استلام مشتريات"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Firebrick
        Me.Label2.Location = New System.Drawing.Point(83, 171)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 18)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "استلام انتاج"
        '
        'rdoProductionReceive
        '
        Me.rdoProductionReceive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoProductionReceive.AutoSize = True
        Me.rdoProductionReceive.Location = New System.Drawing.Point(162, 173)
        Me.rdoProductionReceive.Name = "rdoProductionReceive"
        Me.rdoProductionReceive.Size = New System.Drawing.Size(17, 16)
        Me.rdoProductionReceive.TabIndex = 19
        Me.rdoProductionReceive.TabStop = True
        Me.rdoProductionReceive.UseVisualStyleBackColor = True
        '
        'rdoPurchaseReceive
        '
        Me.rdoPurchaseReceive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rdoPurchaseReceive.AutoSize = True
        Me.rdoPurchaseReceive.Location = New System.Drawing.Point(162, 147)
        Me.rdoPurchaseReceive.Name = "rdoPurchaseReceive"
        Me.rdoPurchaseReceive.Size = New System.Drawing.Size(17, 16)
        Me.rdoPurchaseReceive.TabIndex = 19
        Me.rdoPurchaseReceive.TabStop = True
        Me.rdoPurchaseReceive.UseVisualStyleBackColor = True
        '
        'frmStockTransaction
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1382, 753)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel4)
        Me.MinimumSize = New System.Drawing.Size(1200, 400)
        Me.Name = "frmStockTransaction"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmStockTransaction"
        Me.Panel1.ResumeLayout(False)
        CType(Me.dgvTransfersList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.dgvTransferDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.grpReceiveMode.ResumeLayout(False)
        Me.grpReceiveMode.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents dgvTransfersList As DataGridView
    Friend WithEvents Panel2 As Panel
    Friend WithEvents dgvTransferDetails As DataGridView
    Friend WithEvents lblTargetStore As Label
    Friend WithEvents cboTargetStore As ComboBox
    Friend WithEvents lblSourceStore As Label
    Friend WithEvents cboSourceStore As ComboBox
    Friend WithEvents txtTransactionCode As TextBox
    Friend WithEvents dtpTransactionDate As DateTimePicker
    Friend WithEvents Panel4 As Panel
    Friend WithEvents btnTransactionSearch As Button
    Friend WithEvents btnSend As Button
    Friend WithEvents btnNew As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnCancelReceive As Button
    Friend WithEvents Panel5 As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents rdoPurchaseReceive As RadioButton
    Friend WithEvents Label1 As Label
    Friend WithEvents grpReceiveMode As GroupBox
    Friend WithEvents rdoReturnReceive As RadioButton
    Friend WithEvents rdoCuttingReceive As RadioButton
    Friend WithEvents rdoProductionReceive As RadioButton
    Friend WithEvents Label9 As Label
    Friend WithEvents rdoSendTransaction As RadioButton
    Friend WithEvents rdoReceiveTransaction As RadioButton
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents rdoPRTSend As RadioButton
    Friend WithEvents Label3 As Label
    Friend WithEvents rdoScrapRecieve As RadioButton
    Friend WithEvents Label4 As Label
    Friend WithEvents rdoPostSales As RadioButton
    Friend WithEvents Label5 As Label
    Friend WithEvents colProductSearch As DataGridViewButtonColumn
    Friend WithEvents colProductID As DataGridViewTextBoxColumn
    Friend WithEvents colProductCode As DataGridViewComboBoxColumn
    Friend WithEvents colProductType As DataGridViewComboBoxColumn
    Friend WithEvents colProductName As DataGridViewTextBoxColumn
    Friend WithEvents colSourceOnHand As DataGridViewTextBoxColumn
    Friend WithEvents colemp As DataGridViewTextBoxColumn
    Friend WithEvents colTransferQty As DataGridViewTextBoxColumn
    Friend WithEvents colUnit As DataGridViewTextBoxColumn
    Friend WithEvents colEmp2 As DataGridViewTextBoxColumn
    Friend WithEvents colTargetAfter As DataGridViewTextBoxColumn
    Friend WithEvents colLineNotes As DataGridViewTextBoxColumn
    Friend WithEvents colDelete As DataGridViewButtonColumn
End Class
