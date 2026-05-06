<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmfinChartOfAccounts
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
        Me.components = New System.ComponentModel.Container()
        Me.tvChartOfAccounts = New System.Windows.Forms.TreeView()
        Me.cmsChartOfAccounts = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuAddChildAccount = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditAccount = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuDeleteAccount = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuRefresh = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlDetails = New System.Windows.Forms.Panel()
        Me.sctAccountDetails = New System.Windows.Forms.SplitContainer()
        Me.dgvJournalHeaders = New System.Windows.Forms.DataGridView()
        Me.dgvJournalDetails = New System.Windows.Forms.DataGridView()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.chkCostCenter = New System.Windows.Forms.CheckBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.chkControl = New System.Windows.Forms.CheckBox()
        Me.chkPostable = New System.Windows.Forms.CheckBox()
        Me.txtAccountLevel = New System.Windows.Forms.TextBox()
        Me.txtAccountName = New System.Windows.Forms.TextBox()
        Me.txtAccountCode = New System.Windows.Forms.TextBox()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.cboChildDigits = New System.Windows.Forms.ComboBox()
        Me.btnViewJournalEntries = New System.Windows.Forms.Button()
        Me.chkSystem = New System.Windows.Forms.CheckBox()
        Me.chkActive = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ملاحظات = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtAccountBalance = New System.Windows.Forms.TextBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.txtAccountNature = New System.Windows.Forms.TextBox()
        Me.txtAccountType = New System.Windows.Forms.TextBox()
        Me.txtAccountParent = New System.Windows.Forms.TextBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cmsChartOfAccounts.SuspendLayout()
        Me.pnlDetails.SuspendLayout()
        CType(Me.sctAccountDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.sctAccountDetails.Panel1.SuspendLayout()
        Me.sctAccountDetails.Panel2.SuspendLayout()
        Me.sctAccountDetails.SuspendLayout()
        CType(Me.dgvJournalHeaders, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvJournalDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlHeader.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tvChartOfAccounts
        '
        Me.tvChartOfAccounts.ContextMenuStrip = Me.cmsChartOfAccounts
        Me.tvChartOfAccounts.Dock = System.Windows.Forms.DockStyle.Left
        Me.tvChartOfAccounts.Location = New System.Drawing.Point(0, 0)
        Me.tvChartOfAccounts.Name = "tvChartOfAccounts"
        Me.tvChartOfAccounts.RightToLeftLayout = True
        Me.tvChartOfAccounts.Size = New System.Drawing.Size(350, 1055)
        Me.tvChartOfAccounts.TabIndex = 0
        '
        'cmsChartOfAccounts
        '
        Me.cmsChartOfAccounts.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.cmsChartOfAccounts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAddChildAccount, Me.mnuEditAccount, Me.mnuDeleteAccount, Me.mnuRefresh})
        Me.cmsChartOfAccounts.Name = "ContextMenuStrip1"
        Me.cmsChartOfAccounts.Size = New System.Drawing.Size(145, 100)
        '
        'mnuAddChildAccount
        '
        Me.mnuAddChildAccount.Name = "mnuAddChildAccount"
        Me.mnuAddChildAccount.Size = New System.Drawing.Size(144, 24)
        Me.mnuAddChildAccount.Text = "اضافة فرع"
        '
        'mnuEditAccount
        '
        Me.mnuEditAccount.Name = "mnuEditAccount"
        Me.mnuEditAccount.Size = New System.Drawing.Size(144, 24)
        Me.mnuEditAccount.Text = "تعديل"
        '
        'mnuDeleteAccount
        '
        Me.mnuDeleteAccount.Name = "mnuDeleteAccount"
        Me.mnuDeleteAccount.Size = New System.Drawing.Size(144, 24)
        Me.mnuDeleteAccount.Text = "حذف"
        '
        'mnuRefresh
        '
        Me.mnuRefresh.Name = "mnuRefresh"
        Me.mnuRefresh.Size = New System.Drawing.Size(144, 24)
        Me.mnuRefresh.Text = "تحديث"
        '
        'pnlDetails
        '
        Me.pnlDetails.Controls.Add(Me.sctAccountDetails)
        Me.pnlDetails.Controls.Add(Me.Button3)
        Me.pnlDetails.Controls.Add(Me.Button2)
        Me.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDetails.Location = New System.Drawing.Point(0, 0)
        Me.pnlDetails.Name = "pnlDetails"
        Me.pnlDetails.Size = New System.Drawing.Size(1232, 895)
        Me.pnlDetails.TabIndex = 1
        '
        'sctAccountDetails
        '
        Me.sctAccountDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sctAccountDetails.Location = New System.Drawing.Point(0, 0)
        Me.sctAccountDetails.Name = "sctAccountDetails"
        Me.sctAccountDetails.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'sctAccountDetails.Panel1
        '
        Me.sctAccountDetails.Panel1.Controls.Add(Me.dgvJournalHeaders)
        Me.sctAccountDetails.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'sctAccountDetails.Panel2
        '
        Me.sctAccountDetails.Panel2.Controls.Add(Me.dgvJournalDetails)
        Me.sctAccountDetails.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.sctAccountDetails.Size = New System.Drawing.Size(1232, 895)
        Me.sctAccountDetails.SplitterDistance = 278
        Me.sctAccountDetails.TabIndex = 8
        '
        'dgvJournalHeaders
        '
        Me.dgvJournalHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvJournalHeaders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvJournalHeaders.Location = New System.Drawing.Point(0, 0)
        Me.dgvJournalHeaders.Name = "dgvJournalHeaders"
        Me.dgvJournalHeaders.RowHeadersWidth = 51
        Me.dgvJournalHeaders.RowTemplate.Height = 26
        Me.dgvJournalHeaders.Size = New System.Drawing.Size(1232, 278)
        Me.dgvJournalHeaders.TabIndex = 7
        '
        'dgvJournalDetails
        '
        Me.dgvJournalDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvJournalDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvJournalDetails.Location = New System.Drawing.Point(0, 0)
        Me.dgvJournalDetails.Name = "dgvJournalDetails"
        Me.dgvJournalDetails.RowHeadersWidth = 51
        Me.dgvJournalDetails.RowTemplate.Height = 26
        Me.dgvJournalDetails.Size = New System.Drawing.Size(1232, 613)
        Me.dgvJournalDetails.TabIndex = 0
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(989, 757)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 6
        Me.Button3.Text = "Button1"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(989, 715)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 6
        Me.Button2.Text = "Button1"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'chkCostCenter
        '
        Me.chkCostCenter.AutoSize = True
        Me.chkCostCenter.Location = New System.Drawing.Point(270, 56)
        Me.chkCostCenter.Name = "chkCostCenter"
        Me.chkCostCenter.Size = New System.Drawing.Size(91, 21)
        Me.chkCostCenter.TabIndex = 7
        Me.chkCostCenter.Text = "مركز تكلفة"
        Me.chkCostCenter.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(3, 9)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(136, 39)
        Me.btnSave.TabIndex = 6
        Me.btnSave.Text = "حفظ"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'chkControl
        '
        Me.chkControl.AutoSize = True
        Me.chkControl.Location = New System.Drawing.Point(239, 30)
        Me.chkControl.Name = "chkControl"
        Me.chkControl.Size = New System.Drawing.Size(122, 21)
        Me.chkControl.TabIndex = 5
        Me.chkControl.Text = "حساب تجميعي"
        Me.chkControl.UseVisualStyleBackColor = True
        '
        'chkPostable
        '
        Me.chkPostable.AutoSize = True
        Me.chkPostable.Location = New System.Drawing.Point(254, 3)
        Me.chkPostable.Name = "chkPostable"
        Me.chkPostable.Size = New System.Drawing.Size(107, 21)
        Me.chkPostable.TabIndex = 5
        Me.chkPostable.Text = "حساب نهائي"
        Me.chkPostable.UseVisualStyleBackColor = True
        '
        'txtAccountLevel
        '
        Me.txtAccountLevel.Location = New System.Drawing.Point(674, 84)
        Me.txtAccountLevel.Name = "txtAccountLevel"
        Me.txtAccountLevel.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountLevel.TabIndex = 1
        '
        'txtAccountName
        '
        Me.txtAccountName.Location = New System.Drawing.Point(905, 44)
        Me.txtAccountName.Name = "txtAccountName"
        Me.txtAccountName.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountName.TabIndex = 1
        '
        'txtAccountCode
        '
        Me.txtAccountCode.Location = New System.Drawing.Point(905, 14)
        Me.txtAccountCode.Name = "txtAccountCode"
        Me.txtAccountCode.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountCode.TabIndex = 0
        '
        'pnlHeader
        '
        Me.pnlHeader.Controls.Add(Me.cboChildDigits)
        Me.pnlHeader.Controls.Add(Me.btnSave)
        Me.pnlHeader.Controls.Add(Me.btnViewJournalEntries)
        Me.pnlHeader.Controls.Add(Me.chkSystem)
        Me.pnlHeader.Controls.Add(Me.chkActive)
        Me.pnlHeader.Controls.Add(Me.Label7)
        Me.pnlHeader.Controls.Add(Me.Label8)
        Me.pnlHeader.Controls.Add(Me.Label6)
        Me.pnlHeader.Controls.Add(Me.ملاحظات)
        Me.pnlHeader.Controls.Add(Me.Label5)
        Me.pnlHeader.Controls.Add(Me.Label4)
        Me.pnlHeader.Controls.Add(Me.Label3)
        Me.pnlHeader.Controls.Add(Me.Label2)
        Me.pnlHeader.Controls.Add(Me.Label1)
        Me.pnlHeader.Controls.Add(Me.txtAccountBalance)
        Me.pnlHeader.Controls.Add(Me.txtNotes)
        Me.pnlHeader.Controls.Add(Me.txtAccountNature)
        Me.pnlHeader.Controls.Add(Me.txtAccountType)
        Me.pnlHeader.Controls.Add(Me.txtAccountParent)
        Me.pnlHeader.Controls.Add(Me.chkCostCenter)
        Me.pnlHeader.Controls.Add(Me.txtAccountCode)
        Me.pnlHeader.Controls.Add(Me.txtAccountName)
        Me.pnlHeader.Controls.Add(Me.chkControl)
        Me.pnlHeader.Controls.Add(Me.chkPostable)
        Me.pnlHeader.Controls.Add(Me.txtAccountLevel)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(1232, 156)
        Me.pnlHeader.TabIndex = 2
        '
        'cboChildDigits
        '
        Me.cboChildDigits.FormattingEnabled = True
        Me.cboChildDigits.Location = New System.Drawing.Point(400, 92)
        Me.cboChildDigits.Name = "cboChildDigits"
        Me.cboChildDigits.Size = New System.Drawing.Size(121, 24)
        Me.cboChildDigits.TabIndex = 16
        '
        'btnViewJournalEntries
        '
        Me.btnViewJournalEntries.Location = New System.Drawing.Point(3, 101)
        Me.btnViewJournalEntries.Name = "btnViewJournalEntries"
        Me.btnViewJournalEntries.Size = New System.Drawing.Size(136, 39)
        Me.btnViewJournalEntries.TabIndex = 15
        Me.btnViewJournalEntries.Text = "عرض القيود"
        Me.btnViewJournalEntries.UseVisualStyleBackColor = True
        '
        'chkSystem
        '
        Me.chkSystem.AutoSize = True
        Me.chkSystem.Location = New System.Drawing.Point(305, 101)
        Me.chkSystem.Name = "chkSystem"
        Me.chkSystem.Size = New System.Drawing.Size(56, 21)
        Me.chkSystem.TabIndex = 14
        Me.chkSystem.Text = "نظام"
        Me.chkSystem.UseVisualStyleBackColor = True
        '
        'chkActive
        '
        Me.chkActive.AutoSize = True
        Me.chkActive.Location = New System.Drawing.Point(302, 80)
        Me.chkActive.Name = "chkActive"
        Me.chkActive.Size = New System.Drawing.Size(59, 21)
        Me.chkActive.TabIndex = 13
        Me.chkActive.Text = "نشط"
        Me.chkActive.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(550, 95)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(105, 17)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "منازل تكويد الابناء"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(550, 56)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 17)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "رصيد الحساب"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(798, 84)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(62, 17)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "المستوى"
        '
        'ملاحظات
        '
        Me.ملاحظات.AutoSize = True
        Me.ملاحظات.Location = New System.Drawing.Point(550, 24)
        Me.ملاحظات.Name = "ملاحظات"
        Me.ملاحظات.Size = New System.Drawing.Size(59, 17)
        Me.ملاحظات.TabIndex = 12
        Me.ملاحظات.Text = "ملاحظات"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(798, 52)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 17)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "طبيعة الحساب"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(798, 14)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 17)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "نوع الحساب"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(1036, 76)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(82, 17)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "الحساب الاب"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1036, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 17)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "اسم الحساب"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1036, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 17)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "كود الحساب"
        '
        'txtAccountBalance
        '
        Me.txtAccountBalance.Location = New System.Drawing.Point(421, 53)
        Me.txtAccountBalance.Name = "txtAccountBalance"
        Me.txtAccountBalance.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountBalance.TabIndex = 11
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(421, 20)
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(100, 24)
        Me.txtNotes.TabIndex = 11
        '
        'txtAccountNature
        '
        Me.txtAccountNature.Location = New System.Drawing.Point(674, 52)
        Me.txtAccountNature.Name = "txtAccountNature"
        Me.txtAccountNature.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountNature.TabIndex = 10
        '
        'txtAccountType
        '
        Me.txtAccountType.Location = New System.Drawing.Point(674, 14)
        Me.txtAccountType.Name = "txtAccountType"
        Me.txtAccountType.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountType.TabIndex = 9
        '
        'txtAccountParent
        '
        Me.txtAccountParent.Location = New System.Drawing.Point(905, 73)
        Me.txtAccountParent.Name = "txtAccountParent"
        Me.txtAccountParent.Size = New System.Drawing.Size(100, 24)
        Me.txtAccountParent.TabIndex = 8
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(350, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlHeader)
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlDetails)
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.SplitContainer1.Size = New System.Drawing.Size(1232, 1055)
        Me.SplitContainer1.SplitterDistance = 156
        Me.SplitContainer1.TabIndex = 3
        '
        'frmChartOfAccounts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 1055)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.tvChartOfAccounts)
        Me.Name = "frmChartOfAccounts"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.RightToLeftLayout = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmChartOfAccounts"
        Me.cmsChartOfAccounts.ResumeLayout(False)
        Me.pnlDetails.ResumeLayout(False)
        Me.sctAccountDetails.Panel1.ResumeLayout(False)
        Me.sctAccountDetails.Panel2.ResumeLayout(False)
        CType(Me.sctAccountDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.sctAccountDetails.ResumeLayout(False)
        CType(Me.dgvJournalHeaders, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvJournalDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tvChartOfAccounts As TreeView
    Friend WithEvents pnlDetails As Panel
    Friend WithEvents txtAccountName As TextBox
    Friend WithEvents txtAccountCode As TextBox
    Friend WithEvents Button3 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents chkControl As CheckBox
    Friend WithEvents chkPostable As CheckBox
    Friend WithEvents txtAccountLevel As TextBox
    Friend WithEvents cmsChartOfAccounts As ContextMenuStrip
    Friend WithEvents chkCostCenter As CheckBox
    Friend WithEvents mnuAddChildAccount As ToolStripMenuItem
    Friend WithEvents mnuEditAccount As ToolStripMenuItem
    Friend WithEvents mnuDeleteAccount As ToolStripMenuItem
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents txtAccountParent As TextBox
    Friend WithEvents txtAccountBalance As TextBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents txtAccountNature As TextBox
    Friend WithEvents txtAccountType As TextBox
    Friend WithEvents dgvJournalHeaders As DataGridView
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents ملاحظات As Label
    Friend WithEvents chkSystem As CheckBox
    Friend WithEvents chkActive As CheckBox
    Friend WithEvents mnuRefresh As ToolStripMenuItem
    Friend WithEvents btnViewJournalEntries As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents cboChildDigits As ComboBox
    Friend WithEvents sctAccountDetails As SplitContainer
    Friend WithEvents dgvJournalDetails As DataGridView
    Friend WithEvents SplitContainer1 As SplitContainer
End Class
