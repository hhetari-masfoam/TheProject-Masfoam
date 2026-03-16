Public Class frmPostLoadingConfirmation
    Private Sub frmPostLoadingConfirmation_Load(
    sender As Object,
    e As EventArgs
) Handles MyBase.Load

        dgvPostingOrderConfirmation.ReadOnly = True
        dgvPostingOrderConfirmation.AllowUserToAddRows = False
        dgvPostingOrderConfirmation.AllowUserToDeleteRows = False
        dgvPostingOrderConfirmation.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPostingOrderConfirmation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        dgvPostingOrderConfirmation.RightToLeft = RightToLeft.Yes

        ' تعريب رؤوس الأعمدة
        dgvPostingOrderConfirmation.Columns("ProductCode").HeaderText = "كود الصنف"
        dgvPostingOrderConfirmation.Columns("PartnerName").HeaderText = "اسم العميل"
        dgvPostingOrderConfirmation.Columns("DriverName").HeaderText = "اسم السائق"
        dgvPostingOrderConfirmation.Columns("TotalQty").HeaderText = "الكمية"

        dgvPostingOrderConfirmation.ColumnHeadersDefaultCellStyle.Font =
    New Font("Tahoma", 10, FontStyle.Bold)

        dgvPostingOrderConfirmation.DefaultCellStyle.Font =
    New Font("Tahoma", 10)

        lblTotalLoadedVolume.Font =
    New Font("Tahoma", 16, FontStyle.Bold)
        dgvPostingOrderConfirmation.ColumnHeadersDefaultCellStyle.Font =
    New Font("Tahoma", 10, FontStyle.Bold)

        dgvPostingOrderConfirmation.DefaultCellStyle.Font =
    New Font("Tahoma", 10)

        lblTotalLoadedVolume.Font =
    New Font("Tahoma", 16, FontStyle.Bold)

    End Sub

    Private Sub btnOK_Click(
    sender As Object,
    e As EventArgs
) Handles btnOK.Click

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnCancel_Click(
    sender As Object,
    e As EventArgs
) Handles btnCancel.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub
End Class