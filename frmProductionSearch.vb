Imports System.Data.SqlClient

Public Class frmProductionSearch
    Inherits frmBaseSearch

    Public Property SelectedProductionID As Integer = 0

    Protected Overrides Sub LoadData()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT
                    ProductionID,
                    ProductionCode,
                    ProductionDate
                FROM Production_Header
                ORDER BY ProductionDate DESC
            ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        dgvSearch.DataSource = dt

    End Sub

    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        SelectedProductionID =
            CInt(dgvSearch.Rows(rowIndex).Cells("ProductionID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
