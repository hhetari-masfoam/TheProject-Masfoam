Public Class frmSRSearch
    Inherits frmBaseSearch

    Public Property SelectedSRID As Integer = 0
    Protected Overrides Sub LoadData()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
"SELECT
    sr.SRID,
    sr.SRCode,
    sr.SRDate,
    sr.DeliveryDate,

    bs.StatusName AS SRStatus,
    fs.StatusName AS FulfillmentStatus,

    sr.PartnerID,
    sr.StoreID,
    sr.SalesRepCode
FROM dbo.Business_SR sr

OUTER APPLY
(
    SELECT TOP 1 srd.BusinessStatusID
    FROM dbo.Business_SRD srd
    WHERE srd.SRID = sr.SRID
    ORDER BY srd.CreatedAt DESC
) last_srd
LEFT JOIN dbo.Workflow_Status bs
    ON bs.StatusID = last_srd.BusinessStatusID

OUTER APPLY
(
    SELECT TOP 1 lo.LoadingStatusID
    FROM dbo.Logistics_LoadingOrder lo
    INNER JOIN dbo.Logistics_LoadingOrderDetail lod
        ON lod.LOID = lo.LOID
    WHERE lod.SourceHeaderID = sr.SRID
    ORDER BY lo.CreatedAt DESC
) last_lo
LEFT JOIN dbo.Workflow_Status fs
    ON fs.StatusID = last_lo.LoadingStatusID

WHERE sr.IsActive = 1
ORDER BY sr.SRDate DESC"

            Using da As New SqlClient.SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)
                dgvSearch.DataSource = dt
            End Using
        End Using

    End Sub

    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        ' =========================
        ' قراءة الصف المختار
        ' =========================
        Dim r As DataGridViewRow = dgvSearch.Rows(rowIndex)

        ' =========================
        ' التأكد من وجود رقم الطلب
        ' =========================
        If r.Cells("SRID").Value Is Nothing Then Exit Sub

        ' =========================
        ' حفظ رقم طلب المبيعات المختار
        ' =========================
        SelectedSRID = CInt(r.Cells("SRID").Value)

        ' =========================
        ' إغلاق شاشة البحث
        ' =========================
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub frmSRSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load


    End Sub
End Class

