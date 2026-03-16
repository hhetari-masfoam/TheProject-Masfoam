Imports System.Data.SqlClient

Public Class frmPartnerSearch
    Inherits frmBaseSearch

    ' فلترة كود الشريك (مثال: SUP-)
    ' =========================
    Public Property PartnerCodePrefix As String = ""
    ' الكود المختار
    Public Property SelectedPartnerCode As String = ""
    Public Property SelectedPartnerID As Integer

    Private Sub frmPartnerSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PrepareGrid()
        LoadData()

    End Sub

    Protected Overrides Sub PrepareGrid()

        With dgvSearch
            .Columns.Clear()
            .AutoGenerateColumns = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        End With

        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PartnerCode",
        .HeaderText = "كود الشريك",
        .DataPropertyName = "PartnerCode"
    })

        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PartnerName",
        .HeaderText = "اسم الشريك",
        .DataPropertyName = "PartnerName"
    })

        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "Phone",
        .HeaderText = "الهاتف",
        .DataPropertyName = "Phone"
    })

        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "City",
        .HeaderText = "المدينة",
        .DataPropertyName = "City"
    })

    End Sub

    Protected Overrides Sub LoadData()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)

            Dim sql As String =
"SELECT
    p.PartnerID,          -- مهم (حتى لو مخفي)
    p.PartnerCode,
    p.PartnerName,
    p.Phone,
    a.City
FROM Master_Partner p
INNER JOIN Master_PartnerRole pr
    ON pr.PartnerID = p.PartnerID
    AND pr.RoleCode = 'CUSTOMER'
LEFT JOIN Master_PartnerAddress a
    ON a.PartnerID = p.PartnerID
    AND a.IsDefault = 1
WHERE p.IsActive = 1"

            ' 🔎 فلترة حسب Prefix إن وُجد
            If Not String.IsNullOrWhiteSpace(PartnerCodePrefix) Then
                sql &= " AND p.PartnerCode LIKE @Prefix"
            End If

            sql &= " ORDER BY p.PartnerName"

            Using cmd As New SqlCommand(sql, con)

                If Not String.IsNullOrWhiteSpace(PartnerCodePrefix) Then
                    cmd.Parameters.AddWithValue("@Prefix", PartnerCodePrefix & "%")
                End If

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using
        End Using

        dgvSearch.DataSource = dt

        ' إخفاء العمود التقني (اختياري لكن أنصح به)
        If dgvSearch.Columns.Contains("PartnerID") Then
            dgvSearch.Columns("PartnerID").Visible = False
        End If

    End Sub

    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        If rowIndex < 0 Then Exit Sub

        ' الحصول على DataRowView المرتبط بالسطر
        Dim drv As DataRowView =
            TryCast(dgvSearch.Rows(rowIndex).DataBoundItem, DataRowView)

        If drv Is Nothing Then Exit Sub

        ' إرجاع PartnerID من الـ DataRow (حتى لو لم يظهر في الجريد)
        SelectedPartnerID = CInt(drv("PartnerID"))

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub


End Class