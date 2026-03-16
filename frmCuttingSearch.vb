Imports System.Data
Imports System.Data.SqlClient

Public Class frmCuttingSearch
    Inherits frmBaseSearch

    ' =========================
    ' المعرّف المختار
    ' =========================
    Public Property SelectedCuttingID As Integer = 0

    ' =========================
    ' تحميل البيانات (كل الهيدر)
    ' =========================
    Protected Overrides Sub LoadData()

        Dim dt As New DataTable

        Dim sql As String =
            "SELECT *
             FROM Production_CuttingHeader
             ORDER BY CuttingID DESC"

        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dt)
            End Using
        End Using

        dgvSearch.DataSource = dt

        ' إخفاء المفتاح الداخلي
        If dgvSearch.Columns.Contains("CuttingID") Then
            dgvSearch.Columns("CuttingID").Visible = False
        End If

    End Sub

    ' =========================
    ' اختيار صف (نقرة واحدة)
    ' =========================
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        If dgvSearch.Rows.Count = 0 Then Exit Sub
        If rowIndex < 0 OrElse rowIndex >= dgvSearch.Rows.Count Then Exit Sub

        SelectedCuttingID =
            CInt(dgvSearch.Rows(rowIndex).Cells("CuttingID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
