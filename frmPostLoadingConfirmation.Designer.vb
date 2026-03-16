<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPostLoadingConfirmation
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
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.dgvPostingOrderConfirmation = New System.Windows.Forms.DataGridView()
        Me.lblTotalLoadedVolume = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.dgvPostingOrderConfirmation, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Tahoma", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(189, 348)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(159, 55)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "الغاء"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Font = New System.Drawing.Font("Tahoma", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOK.Location = New System.Drawing.Point(462, 348)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(159, 55)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "ترحيل"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'dgvPostingOrderConfirmation
        '
        Me.dgvPostingOrderConfirmation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPostingOrderConfirmation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPostingOrderConfirmation.Location = New System.Drawing.Point(7, 192)
        Me.dgvPostingOrderConfirmation.Name = "dgvPostingOrderConfirmation"
        Me.dgvPostingOrderConfirmation.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvPostingOrderConfirmation.RowHeadersWidth = 51
        Me.dgvPostingOrderConfirmation.RowTemplate.Height = 26
        Me.dgvPostingOrderConfirmation.Size = New System.Drawing.Size(781, 150)
        Me.dgvPostingOrderConfirmation.TabIndex = 2
        '
        'lblTotalLoadedVolume
        '
        Me.lblTotalLoadedVolume.AutoSize = True
        Me.lblTotalLoadedVolume.Font = New System.Drawing.Font("Tahoma", 19.8!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalLoadedVolume.ForeColor = System.Drawing.Color.IndianRed
        Me.lblTotalLoadedVolume.Location = New System.Drawing.Point(208, 122)
        Me.lblTotalLoadedVolume.Name = "lblTotalLoadedVolume"
        Me.lblTotalLoadedVolume.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblTotalLoadedVolume.Size = New System.Drawing.Size(378, 41)
        Me.lblTotalLoadedVolume.TabIndex = 3
        Me.lblTotalLoadedVolume.Text = "اجمالي الحجم المحمل"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 19.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(30, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(724, 41)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "هل انت متاكد من ارسال التحميل الى الفوترة"
        '
        'frmPostLoadingConfirmation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTotalLoadedVolume)
        Me.Controls.Add(Me.dgvPostingOrderConfirmation)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Name = "frmPostLoadingConfirmation"
        Me.Text = "frmPostLoadingConfirmation"
        CType(Me.dgvPostingOrderConfirmation, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnCancel As Button
    Friend WithEvents btnOK As Button
    Friend WithEvents dgvPostingOrderConfirmation As DataGridView
    Friend WithEvents lblTotalLoadedVolume As Label
    Friend WithEvents Label1 As Label
End Class
