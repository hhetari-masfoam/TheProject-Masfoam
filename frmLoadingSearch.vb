Imports System.Data.SqlClient

Public Class frmLoadingSearch
    Inherits frmBaseSearch
    Public Property SelectedLOID As Integer = 0

    ' =========================
    ' تحميل البيانات
    ' =========================
    Protected Overrides Sub LoadData()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    LO.LOID,
    LO.LOCode,
    LO.InitiatedDateTime,
    LO.SourceStoreID,
    LO.DriverEmployeeID,
    LO.LoadingSupervisorID,
    LO.VehicleID,
    LO.LoadingStatusID,
    S.StatusName
FROM dbo.Logistics_LoadingOrder LO
LEFT JOIN dbo.Workflow_Status S
    ON S.StatusID = LO.LoadingStatusID
ORDER BY LO.LOID DESC
", con)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvSearch.DataSource = dt
                End Using

            End Using
        End Using

        ' إخفاء المفتاح
        dgvSearch.Columns("LOID").Visible = False

        ' عناوين الأعمدة
        dgvSearch.Columns("LOCode").HeaderText = "رقم أمر التحميل"
        dgvSearch.Columns("InitiatedDateTime").HeaderText = "التاريخ"
        dgvSearch.Columns("SourceStoreID").HeaderText = "المخزن"
        dgvSearch.Columns("DriverEmployeeID").HeaderText = "السائق"
        dgvSearch.Columns("LoadingSupervisorID").HeaderText = "المشرف"
        dgvSearch.Columns("VehicleID").HeaderText = "السيارة"
        dgvSearch.Columns("LoadingStatusID").HeaderText = "الحالة"
        dgvSearch.Columns("StatusName").HeaderText = "اسم الحالة"

    End Sub
    ' =========================
    ' تجهيز الجريد (لا شيء إضافي)
    ' =========================
    Protected Overrides Sub PrepareGrid()
        MyBase.PrepareGrid()
    End Sub

    ' =========================
    ' عند اختيار صف
    ' =========================
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        If rowIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = dgvSearch.Rows(rowIndex)

        SelectedLOID = CInt(row.Cells("LOID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub
End Class
