Public Class frmBaseSearch
    Inherits AABaseOperationForm

    ' =========================
    ' اتصال قاعدة البيانات (مشترك بين جميع فورمات البحث)
    ' =========================

    ' =========================
    ' نقطة تمديد للفورمات الوريثة عند اختيار صف
    ' =========================
    Protected Overridable Sub OnRowPicked()
        ' افتراضيًا لا شيء
    End Sub
    Public Property SourceStoreID As Integer = 0

    ' =========================
    ' تحميل الفورم
    ' =========================

    Private Sub frmBaseSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PrepareGrid()   ' تجهيز الأعمدة
        LoadData()      ' تحميل البيانات
    End Sub

    ' =========================
    ' إعداد الجريد (سلوك عام)
    ' =========================
    Protected Overridable Sub PrepareGrid()
        With dgvSearch
            .AutoGenerateColumns = True
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AllowUserToAddRows = False
        End With
    End Sub

    ' =========================
    ' دبل كليك (اختيار)
    ' =========================
    Protected Overridable Sub dgvSearch_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs) _
    Handles dgvSearch.CellClick

        If e.RowIndex < 0 Then Exit Sub
        OnRowSelected(e.RowIndex)
    End Sub

    ' =========================
    ' نقطة التوسعة للفورم الابن
    ' =========================
    Protected Overridable Sub OnRowSelected(rowIndex As Integer)
        ' سيتم تنفيذها في الفورم الابن
    End Sub

    ' =========================
    ' زر الإغلاق
    ' =========================
    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
    ' =========================
    ' تحميل البيانات (سلوك عام - سيُستبدل في الابن)
    ' =========================
    Protected Overridable Sub LoadData()
        ' تلميح: الفورم الابن هو من يكتب الاستعلام ويملأ dgvSearch
    End Sub
    ' =========================
    ' زر البحث (سلوك عام)
    ' =========================



End Class
