<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMoveLO
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnNewLO = New System.Windows.Forms.Button()
        Me.btnOpendLO = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(260, 62)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(319, 34)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "كيف تريد تحميل الطلب "
        '
        'btnNewLO
        '
        Me.btnNewLO.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNewLO.Location = New System.Drawing.Point(499, 239)
        Me.btnNewLO.Name = "btnNewLO"
        Me.btnNewLO.Size = New System.Drawing.Size(241, 64)
        Me.btnNewLO.TabIndex = 4
        Me.btnNewLO.Text = "تحميل جديد"
        Me.btnNewLO.UseVisualStyleBackColor = True
        '
        'btnOpendLO
        '
        Me.btnOpendLO.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOpendLO.Location = New System.Drawing.Point(102, 239)
        Me.btnOpendLO.Name = "btnOpendLO"
        Me.btnOpendLO.Size = New System.Drawing.Size(241, 64)
        Me.btnOpendLO.TabIndex = 6
        Me.btnOpendLO.Text = "اضاف الى تحميل قديم"
        Me.btnOpendLO.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(300, 342)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(241, 64)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "تراجع"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmMoveLO
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnNewLO)
        Me.Controls.Add(Me.btnOpendLO)
        Me.Controls.Add(Me.Button1)
        Me.Name = "frmMoveLO"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmMoveLO"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents btnNewLO As Button
    Friend WithEvents btnOpendLO As Button
    Friend WithEvents Button1 As Button
End Class
