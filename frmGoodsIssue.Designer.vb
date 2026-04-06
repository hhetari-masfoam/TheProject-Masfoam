<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGoodsIssue
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
        Me.pnlControl = New System.Windows.Forms.Panel()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnIssue = New System.Windows.Forms.Button()
        Me.btnImportSRWaitingIssue = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.dgvSRD = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.dtpIssueDate = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.txtIssueCode = New System.Windows.Forms.TextBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvLO = New System.Windows.Forms.DataGridView()
        Me.pnlControl.SuspendLayout()
        CType(Me.dgvSRD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvLO, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlControl
        '
        Me.pnlControl.BackColor = System.Drawing.SystemColors.Info
        Me.pnlControl.Controls.Add(Me.txtStatusName)
        Me.pnlControl.Controls.Add(Me.btnNew)
        Me.pnlControl.Controls.Add(Me.btnIssue)
        Me.pnlControl.Controls.Add(Me.btnImportSRWaitingIssue)
        Me.pnlControl.Controls.Add(Me.btnCancel)
        Me.pnlControl.Controls.Add(Me.btnSearch)
        Me.pnlControl.Controls.Add(Me.btnPrint)
        Me.pnlControl.Controls.Add(Me.btnClose)
        Me.pnlControl.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlControl.Location = New System.Drawing.Point(1200, 0)
        Me.pnlControl.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlControl.Name = "pnlControl"
        Me.pnlControl.Size = New System.Drawing.Size(124, 760)
        Me.pnlControl.TabIndex = 4
        '
        'txtStatusName
        '
        Me.txtStatusName.Location = New System.Drawing.Point(7, 3)
        Me.txtStatusName.Name = "txtStatusName"
        Me.txtStatusName.Size = New System.Drawing.Size(109, 24)
        Me.txtStatusName.TabIndex = 1
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.SystemColors.Control
        Me.btnNew.Location = New System.Drawing.Point(7, 419)
        Me.btnNew.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(112, 41)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnIssue
        '
        Me.btnIssue.BackColor = System.Drawing.SystemColors.Control
        Me.btnIssue.Location = New System.Drawing.Point(7, 460)
        Me.btnIssue.Margin = New System.Windows.Forms.Padding(4)
        Me.btnIssue.Name = "btnIssue"
        Me.btnIssue.Size = New System.Drawing.Size(112, 41)
        Me.btnIssue.TabIndex = 0
        Me.btnIssue.Text = "فسح البضاعة"
        Me.btnIssue.UseVisualStyleBackColor = False
        '
        'btnImportSRWaitingIssue
        '
        Me.btnImportSRWaitingIssue.BackColor = System.Drawing.SystemColors.Control
        Me.btnImportSRWaitingIssue.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnImportSRWaitingIssue.ForeColor = System.Drawing.Color.IndianRed
        Me.btnImportSRWaitingIssue.Location = New System.Drawing.Point(4, 261)
        Me.btnImportSRWaitingIssue.Margin = New System.Windows.Forms.Padding(4)
        Me.btnImportSRWaitingIssue.Name = "btnImportSRWaitingIssue"
        Me.btnImportSRWaitingIssue.Size = New System.Drawing.Size(112, 86)
        Me.btnImportSRWaitingIssue.TabIndex = 0
        Me.btnImportSRWaitingIssue.Text = "اوامر التحميل"
        Me.btnImportSRWaitingIssue.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Location = New System.Drawing.Point(7, 501)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(112, 41)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Location = New System.Drawing.Point(7, 542)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(112, 41)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Location = New System.Drawing.Point(7, 583)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(112, 41)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Location = New System.Drawing.Point(7, 624)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(112, 41)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "غلق"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'dgvSRD
        '
        Me.dgvSRD.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSRD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSRD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSRD.Location = New System.Drawing.Point(0, 0)
        Me.dgvSRD.Name = "dgvSRD"
        Me.dgvSRD.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSRD.RowHeadersWidth = 51
        Me.dgvSRD.RowTemplate.Height = 26
        Me.dgvSRD.Size = New System.Drawing.Size(1200, 572)
        Me.dgvSRD.TabIndex = 5
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.dtpIssueDate)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.txtNotes)
        Me.Panel1.Controls.Add(Me.txtIssueCode)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1200, 100)
        Me.Panel1.TabIndex = 6
        '
        'dtpIssueDate
        '
        Me.dtpIssueDate.Location = New System.Drawing.Point(864, 48)
        Me.dtpIssueDate.Name = "dtpIssueDate"
        Me.dtpIssueDate.Size = New System.Drawing.Size(200, 24)
        Me.dtpIssueDate.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(539, 22)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 17)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "ملاحظات"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1097, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(78, 17)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "تاريخ الفسح"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1104, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "كود الفسح"
        '
        'txtNotes
        '
        Me.txtNotes.Location = New System.Drawing.Point(40, 15)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(474, 57)
        Me.txtNotes.TabIndex = 0
        '
        'txtIssueCode
        '
        Me.txtIssueCode.Location = New System.Drawing.Point(864, 15)
        Me.txtIssueCode.Name = "txtIssueCode"
        Me.txtIssueCode.Size = New System.Drawing.Size(200, 24)
        Me.txtIssueCode.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 100)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvLO)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.dgvSRD)
        Me.SplitContainer1.Size = New System.Drawing.Size(1200, 660)
        Me.SplitContainer1.SplitterDistance = 84
        Me.SplitContainer1.TabIndex = 7
        '
        'dgvLO
        '
        Me.dgvLO.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvLO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLO.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvLO.Location = New System.Drawing.Point(0, 0)
        Me.dgvLO.Name = "dgvLO"
        Me.dgvLO.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvLO.RowHeadersWidth = 51
        Me.dgvLO.RowTemplate.Height = 26
        Me.dgvLO.Size = New System.Drawing.Size(1200, 84)
        Me.dgvLO.TabIndex = 0
        '
        'frmGoodsIssue
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1324, 760)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.pnlControl)
        Me.Name = "frmGoodsIssue"
        Me.Text = "frmGoodsIssue"
        Me.pnlControl.ResumeLayout(False)
        Me.pnlControl.PerformLayout()
        CType(Me.dgvSRD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvLO, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlControl As Panel
    Friend WithEvents btnNew As Button
    Friend WithEvents btnIssue As Button
    Friend WithEvents btnImportSRWaitingIssue As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents txtStatusName As TextBox
    Friend WithEvents dgvSRD As DataGridView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents txtIssueCode As TextBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvLO As DataGridView
    Friend WithEvents dtpIssueDate As DateTimePicker
End Class
