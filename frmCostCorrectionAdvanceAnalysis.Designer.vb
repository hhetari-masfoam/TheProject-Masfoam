<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCostCorrectionAdvanceAnalysis
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
        Me.dgvFlowWarnings = New System.Windows.Forms.DataGridView()
        Me.dgvFlowRatios = New System.Windows.Forms.DataGridView()
        Me.dgvSimulationLinks = New System.Windows.Forms.DataGridView()
        Me.dgvSimulationLedger = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        CType(Me.dgvFlowWarnings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvFlowRatios, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvSimulationLinks, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvSimulationLedger, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvFlowWarnings
        '
        Me.dgvFlowWarnings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFlowWarnings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvFlowWarnings.Location = New System.Drawing.Point(0, 0)
        Me.dgvFlowWarnings.Name = "dgvFlowWarnings"
        Me.dgvFlowWarnings.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvFlowWarnings.RowHeadersWidth = 51
        Me.dgvFlowWarnings.RowTemplate.Height = 26
        Me.dgvFlowWarnings.Size = New System.Drawing.Size(1447, 312)
        Me.dgvFlowWarnings.TabIndex = 7
        '
        'dgvFlowRatios
        '
        Me.dgvFlowRatios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFlowRatios.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvFlowRatios.Location = New System.Drawing.Point(0, 0)
        Me.dgvFlowRatios.Name = "dgvFlowRatios"
        Me.dgvFlowRatios.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvFlowRatios.RowHeadersWidth = 51
        Me.dgvFlowRatios.RowTemplate.Height = 26
        Me.dgvFlowRatios.Size = New System.Drawing.Size(1447, 166)
        Me.dgvFlowRatios.TabIndex = 6
        '
        'dgvSimulationLinks
        '
        Me.dgvSimulationLinks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSimulationLinks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSimulationLinks.Location = New System.Drawing.Point(0, 0)
        Me.dgvSimulationLinks.Name = "dgvSimulationLinks"
        Me.dgvSimulationLinks.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSimulationLinks.RowHeadersWidth = 51
        Me.dgvSimulationLinks.RowTemplate.Height = 26
        Me.dgvSimulationLinks.Size = New System.Drawing.Size(1447, 315)
        Me.dgvSimulationLinks.TabIndex = 5
        '
        'dgvSimulationLedger
        '
        Me.dgvSimulationLedger.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSimulationLedger.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSimulationLedger.Location = New System.Drawing.Point(0, 0)
        Me.dgvSimulationLedger.Name = "dgvSimulationLedger"
        Me.dgvSimulationLedger.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvSimulationLedger.RowHeadersWidth = 51
        Me.dgvSimulationLedger.RowTemplate.Height = 26
        Me.dgvSimulationLedger.Size = New System.Drawing.Size(1447, 250)
        Me.dgvSimulationLedger.TabIndex = 4
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnClose)
        Me.Panel1.Controls.Add(Me.Button3)
        Me.Panel1.Controls.Add(Me.Button2)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel1.Location = New System.Drawing.Point(1447, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(153, 1055)
        Me.Panel1.TabIndex = 8
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(38, 400)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "إغلاق"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(38, 354)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 0
        Me.Button3.Text = "Button1"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(38, 325)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 0
        Me.Button2.Text = "Button1"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(38, 296)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvSimulationLedger)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer4)
        Me.SplitContainer1.Size = New System.Drawing.Size(1447, 1055)
        Me.SplitContainer1.SplitterDistance = 250
        Me.SplitContainer1.TabIndex = 9
        '
        'SplitContainer4
        '
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.dgvFlowRatios)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer4.Size = New System.Drawing.Size(1447, 801)
        Me.SplitContainer4.SplitterDistance = 166
        Me.SplitContainer4.TabIndex = 9
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
        Me.SplitContainer3.Panel1.Controls.Add(Me.dgvSimulationLinks)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.dgvFlowWarnings)
        Me.SplitContainer3.Size = New System.Drawing.Size(1447, 631)
        Me.SplitContainer3.SplitterDistance = 315
        Me.SplitContainer3.TabIndex = 9
        '
        'frmCostCorrectionAdvanceAnalysis
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1600, 1055)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frmCostCorrectionAdvanceAnalysis"
        Me.Text = "frmCostCorrectionAnalysis"
        CType(Me.dgvFlowWarnings, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvFlowRatios, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvSimulationLinks, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvSimulationLedger, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents dgvFlowWarnings As DataGridView
    Friend WithEvents dgvFlowRatios As DataGridView
    Friend WithEvents dgvSimulationLinks As DataGridView
    Friend WithEvents dgvSimulationLedger As DataGridView
    Friend WithEvents btnClose As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents Panel1 As Panel
End Class
