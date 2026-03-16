Imports System.Data
Imports System.Data.SqlClient

' ✅ بحث فواتير المشتريات (PURCHASE) - فورم مستقل
Public Class frmPurchaseSearch
    Inherits frmBaseSearch   ' 👈 نفس فكرة Product Base Search
    Public Property SelectedDocumentID As Integer = 0
    Public Property SelectedDocumentNo As String
    Public Property OnlyPosted As Boolean = False
    Public Property DocumentTypeFilter As String = "PUR"    ' =========================
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

    End Sub

    ' =========================
    ' تحميل بيانات البحث
    ' =========================
    Protected Overrides Sub LoadData()

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)

            Dim sql As String = "
        SELECT TOP 200
            d.DocumentID,
            d.DocumentNo,
            d.DocumentDate,
            p.PartnerName,
            s.StatusName,
            d.IsInventoryPosted
        FROM Inventory_DocumentHeader d
        INNER JOIN Master_Partner p
            ON p.PartnerID = d.PartnerID
        INNER JOIN Workflow_Status s
            ON s.StatusID = d.StatusID
        WHERE d.DocumentType = @DocType
        "

            ' 👇 إذا كان المطلوب فقط المرحل
            If OnlyPosted Then
                sql &= " AND d.IsInventoryPosted = 1 "
            End If

            sql &= " ORDER BY d.DocumentDate DESC "

            Using cmd As New SqlCommand(sql, con)

                cmd.Parameters.AddWithValue("@DocType", DocumentTypeFilter)

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
