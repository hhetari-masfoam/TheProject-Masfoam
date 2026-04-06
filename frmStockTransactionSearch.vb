Imports System.Data
Imports System.Data.SqlClient

Public Class frmStockTransactionSearch
    Inherits frmBaseSearch
    Public Property CurrentMode As Integer
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

        ' =========================
        ' تحديد نوع العملية حسب المود
        ' =========================
        Dim operationTypeID As Integer = 0

        Select Case CurrentMode
            Case 1 ' TransferSend
                operationTypeID = 10

            Case 2 ' TransferReceive
                operationTypeID = 10

            Case 3 ' PurchaseReceive
                operationTypeID = 7

            Case 4 ' ProductionReceive
                operationTypeID = 6

            Case 5 ' CuttingReceive
                operationTypeID = 11

            Case 6 ' SalesReturnReceive
                operationTypeID = 12

            Case 7 ' PurchaseReturnSend
                operationTypeID = 14

            Case 8 ' CuttingWasteReceive
                operationTypeID = 13

            Case 9 ' PostSales
                operationTypeID = 4   ' فقط للعرض حالياً
        End Select


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
WHERE h.OperationTypeID = @OperationTypeID
ORDER BY h.TransactionDate DESC
", con)

                cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

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