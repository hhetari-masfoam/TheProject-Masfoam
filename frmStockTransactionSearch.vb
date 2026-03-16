Imports System.Data
Imports System.Data.SqlClient

Public Class frmStockTransactionSearch
    Inherits frmBaseSearch

    Public Property SelectedTransactionID As Integer = 0

    Private Sub frmStockTransactionSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler dgvSearch.CellMouseDoubleClick, AddressOf dgvSearch_CellMouseDoubleClick
        LoadAllTransactions()
    End Sub

    ' =========================
    ' تحميل كل الترانسكشنز
    ' =========================
    Private Sub LoadAllTransactions()
        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    h.TransactionID,
    h.TransactionDate,
    h.SourceDocumentID,
    ot.OperationCode AS OperationName,
    h.PeriodID,
    h.StatusID,
    h.IsInventoryPosted,
    h.CreatedBy,
    h.CreatedAt,
    h.SentAt,
    h.SentBy,
    h.ReceivedAt,
    h.ReceivedBy
FROM dbo.Inventory_TransactionHeader h
LEFT JOIN dbo.Workflow_OperationType ot
    ON ot.OperationTypeID = h.OperationTypeID
ORDER BY h.TransactionDate DESC
", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        dgvSearch.AutoGenerateColumns = True
        dgvSearch.DataSource = dt
    End Sub

    Private Sub dgvSearch_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        If e.RowIndex < 0 Then Exit Sub

        SelectedTransactionID = CInt(dgvSearch.Rows(e.RowIndex).Cells("TransactionID").Value)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

End Class