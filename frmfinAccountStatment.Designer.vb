<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmfinAccountStatment
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
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnSearchAccount = New System.Windows.Forms.Button()
        Me.cboAccountNameAr = New System.Windows.Forms.ComboBox()
        Me.txtAccountCode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnLoadData = New System.Windows.Forms.Button()
        Me.dtpFromDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpToDate = New System.Windows.Forms.DateTimePicker()
        Me.dgvAccountDetails = New System.Windows.Forms.DataGridView()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtTotalCredit = New System.Windows.Forms.TextBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTotalDebit = New System.Windows.Forms.TextBox()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.lblDifference = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtDifference = New System.Windows.Forms.TextBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2.SuspendLayout()
        CType(Me.dgvAccountDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnSearchAccount)
        Me.Panel2.Controls.Add(Me.cboAccountNameAr)
        Me.Panel2.Controls.Add(Me.txtAccountCode)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.btnLoadData)
        Me.Panel2.Controls.Add(Me.dtpFromDate)
        Me.Panel2.Controls.Add(Me.dtpToDate)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1384, 100)
        Me.Panel2.TabIndex = 1
        '
        'btnSearchAccount
        '
        Me.btnSearchAccount.Location = New System.Drawing.Point(1190, 12)
        Me.btnSearchAccount.Name = "btnSearchAccount"
        Me.btnSearchAccount.Size = New System.Drawing.Size(99, 57)
        Me.btnSearchAccount.TabIndex = 6
        Me.btnSearchAccount.Text = "بحث"
        Me.btnSearchAccount.UseVisualStyleBackColor = True
        '
        'cboAccountNameAr
        '
        Me.cboAccountNameAr.FormattingEnabled = True
        Me.cboAccountNameAr.Location = New System.Drawing.Point(912, 40)
        Me.cboAccountNameAr.Name = "cboAccountNameAr"
        Me.cboAccountNameAr.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.cboAccountNameAr.Size = New System.Drawing.Size(272, 25)
        Me.cboAccountNameAr.TabIndex = 5
        '
        'txtAccountCode
        '
        Me.txtAccountCode.Location = New System.Drawing.Point(912, 12)
        Me.txtAccountCode.Name = "txtAccountCode"
        Me.txtAccountCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtAccountCode.Size = New System.Drawing.Size(272, 24)
        Me.txtAccountCode.TabIndex = 3
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(835, 45)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(29, 17)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "الى"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(835, 17)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(26, 17)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "من"
        '
        'btnLoadData
        '
        Me.btnLoadData.Location = New System.Drawing.Point(470, 12)
        Me.btnLoadData.Name = "btnLoadData"
        Me.btnLoadData.Size = New System.Drawing.Size(153, 57)
        Me.btnLoadData.TabIndex = 1
        Me.btnLoadData.Text = "عرض كشف الحساب"
        Me.btnLoadData.UseVisualStyleBackColor = True
        '
        'dtpFromDate
        '
        Me.dtpFromDate.Location = New System.Drawing.Point(629, 12)
        Me.dtpFromDate.Name = "dtpFromDate"
        Me.dtpFromDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dtpFromDate.Size = New System.Drawing.Size(200, 24)
        Me.dtpFromDate.TabIndex = 0
        '
        'dtpToDate
        '
        Me.dtpToDate.Location = New System.Drawing.Point(629, 45)
        Me.dtpToDate.Name = "dtpToDate"
        Me.dtpToDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dtpToDate.Size = New System.Drawing.Size(200, 24)
        Me.dtpToDate.TabIndex = 0
        '
        'dgvAccountDetails
        '
        Me.dgvAccountDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAccountDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvAccountDetails.Location = New System.Drawing.Point(0, 100)
        Me.dgvAccountDetails.Name = "dgvAccountDetails"
        Me.dgvAccountDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.dgvAccountDetails.RowHeadersWidth = 51
        Me.dgvAccountDetails.RowTemplate.Height = 26
        Me.dgvAccountDetails.Size = New System.Drawing.Size(1384, 462)
        Me.dgvAccountDetails.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Location = New System.Drawing.Point(296, 18)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Panel3.Size = New System.Drawing.Size(125, 33)
        Me.Panel3.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(22, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 17)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "اجمالي مدين"
        '
        'txtTotalCredit
        '
        Me.txtTotalCredit.Location = New System.Drawing.Point(296, 57)
        Me.txtTotalCredit.Name = "txtTotalCredit"
        Me.txtTotalCredit.Size = New System.Drawing.Size(125, 24)
        Me.txtTotalCredit.TabIndex = 4
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Label2)
        Me.Panel4.Location = New System.Drawing.Point(165, 18)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Panel4.Size = New System.Drawing.Size(125, 33)
        Me.Panel4.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 6)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 17)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "اجمالي دائن"
        '
        'txtTotalDebit
        '
        Me.txtTotalDebit.Location = New System.Drawing.Point(165, 57)
        Me.txtTotalDebit.Name = "txtTotalDebit"
        Me.txtTotalDebit.Size = New System.Drawing.Size(125, 24)
        Me.txtTotalDebit.TabIndex = 4
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.lblDifference)
        Me.Panel5.Controls.Add(Me.Label3)
        Me.Panel5.Location = New System.Drawing.Point(34, 18)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Panel5.Size = New System.Drawing.Size(125, 33)
        Me.Panel5.TabIndex = 7
        '
        'lblDifference
        '
        Me.lblDifference.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDifference.AutoSize = True
        Me.lblDifference.Location = New System.Drawing.Point(6, 6)
        Me.lblDifference.Name = "lblDifference"
        Me.lblDifference.Size = New System.Drawing.Size(0, 17)
        Me.lblDifference.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(68, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 17)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "الرصيد"
        '
        'txtDifference
        '
        Me.txtDifference.Location = New System.Drawing.Point(34, 57)
        Me.txtDifference.Name = "txtDifference"
        Me.txtDifference.Size = New System.Drawing.Size(125, 24)
        Me.txtDifference.TabIndex = 4
        '
        'Button4
        '
        Me.Button4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button4.Location = New System.Drawing.Point(1052, 24)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(136, 58)
        Me.Button4.TabIndex = 0
        Me.Button4.Text = "طباعة"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button5.Location = New System.Drawing.Point(910, 24)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(136, 58)
        Me.Button5.TabIndex = 0
        Me.Button5.Text = "إغلاق"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button6.Location = New System.Drawing.Point(1194, 24)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(136, 58)
        Me.Button6.TabIndex = 0
        Me.Button6.Text = "اكسل"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel5)
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Button6)
        Me.Panel1.Controls.Add(Me.Panel4)
        Me.Panel1.Controls.Add(Me.Button5)
        Me.Panel1.Controls.Add(Me.Button4)
        Me.Panel1.Controls.Add(Me.txtDifference)
        Me.Panel1.Controls.Add(Me.txtTotalCredit)
        Me.Panel1.Controls.Add(Me.txtTotalDebit)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 562)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1384, 99)
        Me.Panel1.TabIndex = 7
        '
        'frmfinAccountStatment
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1384, 661)
        Me.Controls.Add(Me.dgvAccountDetails)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Font = New System.Drawing.Font("Tahoma", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmfinAccountStatment"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "كشف الحساب"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.dgvAccountDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As Panel
    Friend WithEvents dgvAccountDetails As DataGridView
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents txtTotalCredit As TextBox
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents txtTotalDebit As TextBox
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents txtDifference As TextBox
    Friend WithEvents lblDifference As Label
    Friend WithEvents cboAccountNameAr As ComboBox
    Friend WithEvents txtAccountCode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnLoadData As Button
    Friend WithEvents dtpFromDate As DateTimePicker
    Friend WithEvents dtpToDate As DateTimePicker
    Friend WithEvents btnSearchAccount As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Button6 As Button
    Friend WithEvents Panel1 As Panel
End Class
