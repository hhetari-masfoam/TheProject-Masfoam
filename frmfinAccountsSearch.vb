Imports System.Data.SqlClient

Public Class frmfinAccountsSearch
    Inherits frmBaseSearch
    Public Property OperationTypeID As Integer = 0

    Public Property SelectedTransactionNo As String = ""
    Public Property SelectedAccountID As Integer = 0
    Private Sub frmfinPaymentVouchersSearch_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        ' لا تستدعي شيء هنا
        ' BaseSearch سيستدعي PrepareGrid + LoadData

    End Sub

    ' =========================
    ' تحميل البيانات
    ' =========================

    Protected Overrides Sub LoadData()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable()

            Dim sql As String = "
SELECT 
    a.AccountID,
    a.AccountNameAr
FROM gl.Accounts a
JOIN cfg.AccountOperationRules r
    ON r.AccountID = a.AccountID
WHERE r.OperationTypeID = @OperationTypeID
  AND r.IsAllowed = 1
  AND a.IsActive = 1
ORDER BY a.AccountNameAr
"

            Using da As New SqlDataAdapter(sql, con)

                da.SelectCommand.Parameters.AddWithValue("@OperationTypeID", OperationTypeID)

                da.Fill(dt) ' 🔥 هذا السطر كان ناقص

            End Using

            dgvSearch.DataSource = dt

        End Using

    End Sub


    ' =========================
    ' عند اختيار صف
    ' =========================
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        SelectedAccountID = CInt(dgvSearch.Rows(rowIndex).Cells("AccountID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub
End Class