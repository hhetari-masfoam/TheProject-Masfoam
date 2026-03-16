<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMoveSR
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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnMoveToWH = New System.Windows.Forms.Button()
        Me.btnMoveToCutting = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnBackToNew = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(292, 372)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(200, 47)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "تراجع"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnMoveToWH
        '
        Me.btnMoveToWH.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveToWH.Location = New System.Drawing.Point(292, 286)
        Me.btnMoveToWH.Name = "btnMoveToWH"
        Me.btnMoveToWH.Size = New System.Drawing.Size(200, 47)
        Me.btnMoveToWH.TabIndex = 1
        Me.btnMoveToWH.Text = "الى المستودع"
        Me.btnMoveToWH.UseVisualStyleBackColor = True
        '
        'btnMoveToCutting
        '
        Me.btnMoveToCutting.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveToCutting.Location = New System.Drawing.Point(553, 286)
        Me.btnMoveToCutting.Name = "btnMoveToCutting"
        Me.btnMoveToCutting.Size = New System.Drawing.Size(200, 47)
        Me.btnMoveToCutting.TabIndex = 1
        Me.btnMoveToCutting.Text = "الى القص"
        Me.btnMoveToCutting.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(252, 92)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(292, 34)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "اين تريد توجيه الطلب"
        '
        'btnBackToNew
        '
        Me.btnBackToNew.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBackToNew.Location = New System.Drawing.Point(32, 286)
        Me.btnBackToNew.Name = "btnBackToNew"
        Me.btnBackToNew.Size = New System.Drawing.Size(200, 47)
        Me.btnBackToNew.TabIndex = 1
        Me.btnBackToNew.Text = "الى جديد"
        Me.btnBackToNew.UseVisualStyleBackColor = True
        '
        'frmMoveSR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnMoveToCutting)
        Me.Controls.Add(Me.btnBackToNew)
        Me.Controls.Add(Me.btnMoveToWH)
        Me.Controls.Add(Me.Button1)
        Me.Name = "frmMoveSR"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmMoveSR"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents btnMoveToWH As Button
    Friend WithEvents btnMoveToCutting As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnBackToNew As Button
End Class
