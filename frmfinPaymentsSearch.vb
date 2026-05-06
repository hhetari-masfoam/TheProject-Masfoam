
Imports System.Data.SqlClient

    Public Class frmFinPaymentsSearch
        Inherits frmBaseSearch
        Public Property SelectedTransactionID As Integer = 0
        Public Property OperationTypeID As Integer = 1
        Public Property SelectedTransactionNo As String = ""
        Protected Overrides Sub LoadData()

            Using con As New SqlConnection(AppConfig.MainConnectionString)
                con.Open()

                Dim dt As New DataTable

                Dim da As New SqlDataAdapter("
SELECT 
    t.CashTransactionID,  -- مهم جداً
    t.TransactionNo,
    t.TransactionDate,
    t.TotalAmount,
    t.OperationTypeID,
    s.statusName
FROM fin.CashTransactionHeader t
LEFT JOIN wf.status s on t.statusID=s.statusID
WHERE t.OperationTypeID = @Type
", con)

                da.SelectCommand.Parameters.AddWithValue("@Type", OperationTypeID)

                da.Fill(dt)

                dgvSearch.DataSource = dt

            End Using

        End Sub

        Protected Overrides Sub OnRowSelected(rowIndex As Integer)

            SelectedTransactionID =
            CInt(dgvSearch.Rows(rowIndex).Cells("CashTransactionID").Value)

            SelectedTransactionNo =
            dgvSearch.Rows(rowIndex).Cells("TransactionNo").Value.ToString()

            Me.DialogResult = DialogResult.OK
            Me.Close()

        End Sub

End Class
