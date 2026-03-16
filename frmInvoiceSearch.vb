Imports System.Data.SqlClient

Public Class frmInvoiceSearch
    Inherits frmBaseSearch

    Public Property SelectedDocumentID As Integer = 0

    ' =========================
    ' تحميل البيانات
    ' =========================
    Protected Overrides Sub LoadData()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    H.DocumentID,
    H.DocumentNo AS InvoiceCode,
    H.DocumentDate,
    P.PartnerName,
    H.TotalAmount,
    S.StatusName
FROM Inventory_DocumentHeader H
INNER JOIN Master_Partner P
    ON P.PartnerID = H.PartnerID
INNER JOIN Workflow_Status S
    ON S.StatusID = H.StatusID
WHERE H.DocumentType = 'SAL'
  AND H.IsOutbound = 1
ORDER BY H.DocumentID DESC
", con)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    dgvSearch.DataSource = dt
                End Using

            End Using
        End Using

        ' =========================
        ' تنسيق الأعمدة
        ' =========================
        If dgvSearch.Columns.Contains("DocumentID") Then
            dgvSearch.Columns("DocumentID").Visible = False
        End If

        If dgvSearch.Columns.Contains("InvoiceCode") Then
            dgvSearch.Columns("InvoiceCode").HeaderText = "رقم الفاتورة"
        End If

        If dgvSearch.Columns.Contains("DocumentDate") Then
            dgvSearch.Columns("DocumentDate").HeaderText = "التاريخ"
        End If

        If dgvSearch.Columns.Contains("PartnerName") Then
            dgvSearch.Columns("PartnerName").HeaderText = "العميل"
        End If

        If dgvSearch.Columns.Contains("TotalAmount") Then
            dgvSearch.Columns("TotalAmount").HeaderText = "الصافي"
            dgvSearch.Columns("TotalAmount").DefaultCellStyle.Format = "N2"
        End If

        If dgvSearch.Columns.Contains("StatusName") Then
            dgvSearch.Columns("StatusName").HeaderText = "الحالة"
        End If

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

        SelectedDocumentID =
        CInt(row.Cells("DocumentID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
