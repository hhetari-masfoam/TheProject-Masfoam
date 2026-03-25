Imports System.Data
Imports System.Data.SqlClient

' ✅ بحث فواتير المشتريات (PURCHASE) - فورم مستقل
Public Class frmPurchaseReturnsearch
    Inherits frmBaseSearch   ' 👈 نفس فكرة Product Base Search
    Public Property SelectedDocumentID As Integer = 0

    ' =========================
    ' تجهيز الجريد (أعمدة فقط)
    ' =========================
    ' =========================
    ' تجهيز الجريد
    ' =========================
    Protected Overrides Sub PrepareGrid()

        With dgvSearch
            .Columns.Clear()
            .AutoGenerateColumns = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
        End With

        ' =========================
        ' DocumentID (مخفي)
        ' =========================
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "DocumentID",
        .DataPropertyName = "DocumentID",
        .Visible = False
    })

        ' =========================
        ' رقم الفاتورة
        ' =========================
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "DocumentNo",
        .HeaderText = "رقم الفاتورة",
        .DataPropertyName = "DocumentNo"
    })

        ' =========================
        ' تاريخ الفاتورة
        ' =========================
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "DocumentDate",
        .HeaderText = "التاريخ",
        .DataPropertyName = "DocumentDate"
    })

        ' =========================
        ' المورد
        ' =========================
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PartnerName",
        .HeaderText = "المورد",
        .DataPropertyName = "PartnerName"
    })

        ' =========================
        ' الحالة
        ' =========================
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "StatusName",
        .HeaderText = "الحالة",
        .DataPropertyName = "StatusName"
    })
        dgvSearch.Columns.Add(New DataGridViewTextBoxColumn() With {
    .Name = "OriginalInvoiceNo",
    .HeaderText = "الفاتورة الأصلية",
    .DataPropertyName = "OriginalInvoiceNo"
})


    End Sub

    ' =========================
    ' تحميل بيانات البحث
    ' =========================
    ' =========================
    ' تحميل بيانات البحث
    ' =========================
    Protected Overrides Sub LoadData()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
       SELECT TOP 200
    prt.DocumentID,
    prt.DocumentNo,
    prt.DocumentDate,
    p.PartnerName,
    ws.StatusName,
    (
        SELECT TOP 1 pur.DocumentNo
        FROM Document_Link l
        INNER JOIN Inventory_DocumentHeader pur
            ON pur.DocumentID = l.SourceDocumentID
        WHERE l.TargetDocumentID = prt.DocumentID
    ) AS OriginalInvoiceNo
FROM Inventory_DocumentHeader prt
INNER JOIN Master_Partner p
    ON p.PartnerID = prt.PartnerID
INNER JOIN Workflow_Status ws
    ON ws.StatusID = prt.StatusID
WHERE prt.DocumentType = 'PRT'
ORDER BY prt.DocumentDate DESC

        ", con)

                con.Open()
                dt.Load(cmd.ExecuteReader())

            End Using
        End Using

        dgvSearch.DataSource = dt

    End Sub

    ' =========================
    ' عند اختيار صف (Double Click)
    ' =========================
    ' =========================
    ' اختيار صف
    ' =========================
    Protected Overrides Sub OnRowSelected(rowIndex As Integer)

        SelectedDocumentID =
        CInt(dgvSearch.Rows(rowIndex).Cells("DocumentID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
