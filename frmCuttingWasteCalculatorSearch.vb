Imports System.Data
Imports System.Data.SqlClient

Public Class frmCuttingWasteCalculatorSearch
    Inherits frmBaseSearch

    Public Property SelectedWasteID As Integer = 0

    Protected Overrides Sub LoadData()
        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT TOP (1000)
    h.WasteID,
    h.WasteCode,
    h.PeriodStartDate,
    h.PeriodEndDate,
    ws.StatusName AS StatusName,
    ws.StatusCode AS StatusCode,
    h.SourceStoreID,
    h.TargetStoreID,
    h.ScrapProductID,
    h.CalculatedAt
FROM dbo.Inventory_WasteHeader AS h
LEFT JOIN dbo.Workflow_Status AS ws
    ON ws.StatusID = h.StatusID
WHERE h.OperationTypeID = 13
ORDER BY h.WasteID DESC;
", con)
                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        dgvSearch.DataSource = dt
    End Sub

    Protected Overrides Sub OnRowSelected(rowIndex As Integer)
        SelectedWasteID = CInt(dgvSearch.Rows(rowIndex).Cells("WasteID").Value)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class