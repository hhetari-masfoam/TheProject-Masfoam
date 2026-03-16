Imports System.Data.SqlClient

Public Class frmEmployeeSearch
    Inherits frmBaseSearch

    ' =========================
    ' القيمة الراجعة للفورم الأب
    ' =========================
    Public Property SelectedEmpCode As String

    ' =========================
    ' تحميل البيانات (Override صحيح)
    ' =========================
    Protected Overrides Sub LoadData()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)

            Dim sql As String =
"SELECT
    e.EmpCode,
    e.EmpName,
    e.Mobile
FROM Security_Employee e
WHERE e.IsSalesRep = 1
  AND e.IsActive = 1
ORDER BY e.EmpName"

            Using cmd As New SqlCommand(sql, con)
                con.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        dgvSearch.DataSource = dt

    End Sub

    ' =========================
    ' عند اختيار صف (نقرة واحدة)
    ' =========================
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        If rowIndex < 0 Then Exit Sub

        Dim drv As DataRowView =
            TryCast(dgvSearch.Rows(rowIndex).DataBoundItem, DataRowView)

        If drv Is Nothing Then Exit Sub

        SelectedEmpCode = drv("EmpCode").ToString()

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
