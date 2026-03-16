Imports System.Data.SqlClient

Public Class frmBOMSearch
    Inherits frmBaseSearch


    Public Property SelectedBOMID As Integer = 0

    Private Sub LoadBOM(Optional searchText As String = "")
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT " &
            "BH.BOMID, " &
            "BH.BOMCode AS [كود البوم], " &
            "BH.VersionNo AS [الإصدار], " &
            "P.ProductName AS [الصنف], " &
            "BH.IsActive AS [نشط] " &
            "FROM Production_BOMHeader BH " &
            "INNER JOIN Master_Product P ON P.ProductID = BH.ProductID " &
            "WHERE (@t = '' OR BH.BOMCode LIKE @like OR P.ProductName LIKE @like) " &
            "ORDER BY P.ProductName, BH.VersionNo", con)

                cmd.Parameters.AddWithValue("@t", searchText)
                cmd.Parameters.AddWithValue("@like", "%" & searchText & "%")

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        dgvSearch.DataSource = dt
        dgvSearch.Columns("BOMID").Visible = False
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.Text = "بحث عن BOM"
        LoadBOM()
    End Sub
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        If dgvSearch.Rows.Count = 0 Then Exit Sub

        SelectedBOMID = CInt(dgvSearch.Rows(rowIndex).Cells("BOMID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub


End Class
