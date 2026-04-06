<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCostCorrection
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
        Me.pnlControls = New System.Windows.Forms.Panel()
        Me.txtStatusName = New System.Windows.Forms.TextBox()
        Me.btnAdvanceAnalysis = New System.Windows.Forms.Button()
        Me.btnExcute = New System.Windows.Forms.Button()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnAffectedOperations = New System.Windows.Forms.Button()
        Me.btnPreveiw = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.dgvAffectedOperations = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.dgvCorrectionQueue = New System.Windows.Forms.DataGridView()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.dgvSimulation = New System.Windows.Forms.DataGridView()
        Me.pnlControls.SuspendLayout()
        CType(Me.dgvAffectedOperations, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.dgvCorrectionQueue, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.dgvSimulation, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlControls
        '
        Me.pnlControls.BackColor = System.Drawing.SystemColors.Info
        Me.pnlControls.Controls.Add(Me.txtStatusName)
        Me.pnlControls.Controls.Add(Me.btnAdvanceAnalysis)
        Me.pnlControls.Controls.Add(Me.btnExcute)
        Me.pnlControls.Controls.Add(Me.btnNew)
        Me.pnlControls.Controls.Add(Me.btnAffectedOperations)
        Me.pnlControls.Controls.Add(Me.btnPreveiw)
        Me.pnlControls.Controls.Add(Me.btnCancel)
        Me.pnlControls.Controls.Add(Me.btnSearch)
        Me.pnlControls.Controls.Add(Me.btnPrint)
        Me.pnlControls.Controls.Add(Me.btnClose)
        Me.pnlControls.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlControls.Location = New System.Drawing.Point(1448, 0)
        Me.pnlControls.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlControls.Name = "pnlControls"
        Me.pnlControls.Size = New System.Drawing.Size(134, 1055)
        Me.pnlControls.TabIndex = 5
        '
        'txtStatusName
        '
        Me.txtStatusName.Enabled = False
        Me.txtStatusName.Location = New System.Drawing.Point(5, 12)
        Me.txtStatusName.Name = "txtStatusName"
        Me.txtStatusName.Size = New System.Drawing.Size(114, 24)
        Me.txtStatusName.TabIndex = 22
        '
        'btnAdvanceAnalysis
        '
        Me.btnAdvanceAnalysis.Location = New System.Drawing.Point(7, 536)
        Me.btnAdvanceAnalysis.Name = "btnAdvanceAnalysis"
        Me.btnAdvanceAnalysis.Size = New System.Drawing.Size(115, 58)
        Me.btnAdvanceAnalysis.TabIndex = 1
        Me.btnAdvanceAnalysis.Text = "تحليل متقدم"
        Me.btnAdvanceAnalysis.UseVisualStyleBackColor = True
        '
        'btnExcute
        '
        Me.btnExcute.Location = New System.Drawing.Point(7, 472)
        Me.btnExcute.Name = "btnExcute"
        Me.btnExcute.Size = New System.Drawing.Size(115, 58)
        Me.btnExcute.TabIndex = 1
        Me.btnExcute.Text = "تنفيذ"
        Me.btnExcute.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.SystemColors.Control
        Me.btnNew.Location = New System.Drawing.Point(7, 693)
        Me.btnNew.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(115, 58)
        Me.btnNew.TabIndex = 0
        Me.btnNew.Text = "جديد"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnAffectedOperations
        '
        Me.btnAffectedOperations.BackColor = System.Drawing.SystemColors.Control
        Me.btnAffectedOperations.Location = New System.Drawing.Point(7, 341)
        Me.btnAffectedOperations.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAffectedOperations.Name = "btnAffectedOperations"
        Me.btnAffectedOperations.Size = New System.Drawing.Size(115, 58)
        Me.btnAffectedOperations.TabIndex = 0
        Me.btnAffectedOperations.Text = "العمليات المتاثرة"
        Me.btnAffectedOperations.UseVisualStyleBackColor = False
        '
        'btnPreveiw
        '
        Me.btnPreveiw.BackColor = System.Drawing.SystemColors.Control
        Me.btnPreveiw.Location = New System.Drawing.Point(7, 407)
        Me.btnPreveiw.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPreveiw.Name = "btnPreveiw"
        Me.btnPreveiw.Size = New System.Drawing.Size(115, 58)
        Me.btnPreveiw.TabIndex = 0
        Me.btnPreveiw.Text = "محاكاة"
        Me.btnPreveiw.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Location = New System.Drawing.Point(7, 984)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(115, 58)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Control
        Me.btnSearch.Location = New System.Drawing.Point(7, 759)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(115, 58)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "بحث"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.Control
        Me.btnPrint.Location = New System.Drawing.Point(7, 825)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(115, 58)
        Me.btnPrint.TabIndex = 0
        Me.btnPrint.TabStop = False
        Me.btnPrint.Text = "طباعة"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.SystemColors.Control
        Me.btnClose.Location = New System.Drawing.Point(7, 891)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(115, 58)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'dgvAffectedOperations
        '
        Me.dgvAffectedOperations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAffectedOperations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvAffectedOperations.Location = New System.Drawing.Point(0, 0)
        Me.dgvAffectedOperations.Name = "dgvAffectedOperations"
        Me.dgvAffectedOperations.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvAffectedOperations.RowHeadersWidth = 51
        Me.dgvAffectedOperations.RowTemplate.Height = 26
        Me.dgvAffectedOperations.Size = New System.Drawing.Size(1448, 328)
        Me.dgvAffectedOperations.TabIndex = 0
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer3)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(1448, 1055)
        Me.SplitContainer1.SplitterDistance = 320
        Me.SplitContainer1.TabIndex = 9
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.dgvCorrectionQueue)
        Me.SplitContainer3.Size = New System.Drawing.Size(1448, 320)
        Me.SplitContainer3.SplitterDistance = 291
        Me.SplitContainer3.TabIndex = 8
        '
        'dgvCorrectionQueue
        '
        Me.dgvCorrectionQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCorrectionQueue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCorrectionQueue.Location = New System.Drawing.Point(0, 0)
        Me.dgvCorrectionQueue.Name = "dgvCorrectionQueue"
        Me.dgvCorrectionQueue.ReadOnly = True
        Me.dgvCorrectionQueue.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvCorrectionQueue.RowHeadersWidth = 51
        Me.dgvCorrectionQueue.RowTemplate.Height = 26
        Me.dgvCorrectionQueue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvCorrectionQueue.Size = New System.Drawing.Size(1448, 291)
        Me.dgvCorrectionQueue.TabIndex = 7
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.dgvAffectedOperations)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.dgvSimulation)
        Me.SplitContainer2.Size = New System.Drawing.Size(1448, 731)
        Me.SplitContainer2.SplitterDistance = 328
        Me.SplitContainer2.TabIndex = 1
        '
        'dgvSimulation
        '
        Me.dgvSimulation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSimulation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSimulation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSimulation.Location = New System.Drawing.Point(0, 0)
        Me.dgvSimulation.Name = "dgvSimulation"
        Me.dgvSimulation.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSimulation.RowHeadersWidth = 51
        Me.dgvSimulation.RowTemplate.Height = 26
        Me.dgvSimulation.Size = New System.Drawing.Size(1448, 399)
        Me.dgvSimulation.TabIndex = 0
        '
        'frmCostCorrection
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1582, 1055)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlControls)
        Me.Name = "frmCostCorrection"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmRevaluation"
        Me.pnlControls.ResumeLayout(False)
        Me.pnlControls.PerformLayout()
        CType(Me.dgvAffectedOperations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.dgvCorrectionQueue, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.dgvSimulation, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlControls As Panel
    Friend WithEvents txtStatusName As TextBox
    Friend WithEvents btnExcute As Button
    Friend WithEvents btnPreveiw As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnClose As Button
    Private WithEvents btnNew As Button
    Friend WithEvents dgvAffectedOperations As DataGridView
    Friend WithEvents btnAffectedOperations As Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvSimulation As DataGridView
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents btnAdvanceAnalysis As Button
    Friend WithEvents dgvCorrectionQueue As DataGridView
    Friend WithEvents SplitContainer3 As SplitContainer
End Class
