<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInvoicePrint
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
        Me.dgvDetails = New System.Windows.Forms.DataGridView()
        Me.lblInvoiceNumber = New System.Windows.Forms.Label()
        Me.lblPartnerName = New System.Windows.Forms.Label()
        Me.lblVATNumber = New System.Windows.Forms.Label()
        Me.lblIssueDate = New System.Windows.Forms.Label()
        Me.lblInvoiceHash = New System.Windows.Forms.Label()
        Me.lblQRCode = New System.Windows.Forms.Label()
        CType(Me.dgvDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvDetails
        '
        Me.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvDetails.Location = New System.Drawing.Point(174, 215)
        Me.dgvDetails.Name = "dgvDetails"
        Me.dgvDetails.RowHeadersWidth = 51
        Me.dgvDetails.RowTemplate.Height = 26
        Me.dgvDetails.Size = New System.Drawing.Size(240, 150)
        Me.dgvDetails.TabIndex = 0
        '
        'lblInvoiceNumber
        '
        Me.lblInvoiceNumber.AutoSize = True
        Me.lblInvoiceNumber.Location = New System.Drawing.Point(651, 55)
        Me.lblInvoiceNumber.Name = "lblInvoiceNumber"
        Me.lblInvoiceNumber.Size = New System.Drawing.Size(113, 17)
        Me.lblInvoiceNumber.TabIndex = 1
        Me.lblInvoiceNumber.Text = "lblInvoiceNumber"
        '
        'lblPartnerName
        '
        Me.lblPartnerName.AutoSize = True
        Me.lblPartnerName.Location = New System.Drawing.Point(651, 85)
        Me.lblPartnerName.Name = "lblPartnerName"
        Me.lblPartnerName.Size = New System.Drawing.Size(100, 17)
        Me.lblPartnerName.TabIndex = 1
        Me.lblPartnerName.Text = "lblPartnerName"
        '
        'lblVATNumber
        '
        Me.lblVATNumber.AutoSize = True
        Me.lblVATNumber.Location = New System.Drawing.Point(651, 114)
        Me.lblVATNumber.Name = "lblVATNumber"
        Me.lblVATNumber.Size = New System.Drawing.Size(93, 17)
        Me.lblVATNumber.TabIndex = 1
        Me.lblVATNumber.Text = "lblVATNumber"
        '
        'lblIssueDate
        '
        Me.lblIssueDate.AutoSize = True
        Me.lblIssueDate.Location = New System.Drawing.Point(651, 131)
        Me.lblIssueDate.Name = "lblIssueDate"
        Me.lblIssueDate.Size = New System.Drawing.Size(80, 17)
        Me.lblIssueDate.TabIndex = 1
        Me.lblIssueDate.Text = "lblIssueDate"
        '
        'lblInvoiceHash
        '
        Me.lblInvoiceHash.AutoSize = True
        Me.lblInvoiceHash.Location = New System.Drawing.Point(651, 164)
        Me.lblInvoiceHash.Name = "lblInvoiceHash"
        Me.lblInvoiceHash.Size = New System.Drawing.Size(94, 17)
        Me.lblInvoiceHash.TabIndex = 1
        Me.lblInvoiceHash.Text = "lblInvoiceHash"
        '
        'lblQRCode
        '
        Me.lblQRCode.AutoSize = True
        Me.lblQRCode.Location = New System.Drawing.Point(651, 181)
        Me.lblQRCode.Name = "lblQRCode"
        Me.lblQRCode.Size = New System.Drawing.Size(71, 17)
        Me.lblQRCode.TabIndex = 1
        Me.lblQRCode.Text = "lblQRCode"
        '
        'frmInvoicePrint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.lblQRCode)
        Me.Controls.Add(Me.lblInvoiceHash)
        Me.Controls.Add(Me.lblIssueDate)
        Me.Controls.Add(Me.lblVATNumber)
        Me.Controls.Add(Me.lblPartnerName)
        Me.Controls.Add(Me.lblInvoiceNumber)
        Me.Controls.Add(Me.dgvDetails)
        Me.Name = "frmInvoicePrint"
        Me.Text = "frmInvoicePrint"
        CType(Me.dgvDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvDetails As DataGridView
    Friend WithEvents lblInvoiceNumber As Label
    Friend WithEvents lblPartnerName As Label
    Friend WithEvents lblVATNumber As Label
    Friend WithEvents lblIssueDate As Label
    Friend WithEvents lblInvoiceHash As Label
    Friend WithEvents lblQRCode As Label
End Class
