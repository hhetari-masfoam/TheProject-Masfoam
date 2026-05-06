Imports System.Data.SqlClient

Public Class frmfinAccountStatment
    Private IsLoading As Boolean = False
    Private Sub frmAccountStatement_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadAccounts()

        dtpFromDate.Checked = False
        dtpToDate.Checked = False

    End Sub

    Private Sub LoadAccounts()
        IsLoading = True
        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable()

            Using cmd As New SqlCommand("
SELECT AccountID, AccountCode, AccountNameAr
FROM gl.Accounts
WHERE IsActive = 1
ORDER BY AccountCode
", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using

            cboAccountNameAr.DataSource = dt
            cboAccountNameAr.DisplayMember = "AccountNameAr"
            cboAccountNameAr.ValueMember = "AccountID"

        End Using
        IsLoading = False
    End Sub
    Private Sub cboAccountNameAr_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAccountNameAr.SelectedIndexChanged

        If IsLoading Then Exit Sub

        If cboAccountNameAr.SelectedValue Is Nothing Then Exit Sub

        Dim drv As DataRowView = TryCast(cboAccountNameAr.SelectedItem, DataRowView)
        If drv Is Nothing Then Exit Sub

        txtAccountCode.Text = drv("AccountCode").ToString()

    End Sub
    Private Sub txtAccountCode_TextChanged(sender As Object, e As EventArgs) Handles txtAccountCode.TextChanged

        If IsLoading Then Exit Sub

        If cboAccountNameAr.DataSource Is Nothing Then Exit Sub
        If String.IsNullOrEmpty(cboAccountNameAr.ValueMember) Then Exit Sub

        Dim dt As DataTable = CType(cboAccountNameAr.DataSource, DataTable)

        Dim rows = dt.Select("AccountCode LIKE '" & txtAccountCode.Text & "%'")

        If rows.Length = 0 Then Exit Sub

        cboAccountNameAr.SelectedValue = rows(0)("AccountID")

    End Sub
    Private Sub btnLoadData_Click(sender As Object, e As EventArgs) Handles btnLoadData.Click

        If cboAccountNameAr.SelectedValue Is Nothing Then
            MessageBox.Show("اختر الحساب")
            Exit Sub
        End If

        LoadStatement()

    End Sub
    Private Sub LoadStatement()

        Dim accountId As Integer = CInt(cboAccountNameAr.SelectedValue)

        Dim fromDate As Object = DBNull.Value
        Dim toDate As Object = DBNull.Value

        If dtpFromDate.Checked Then
            fromDate = dtpFromDate.Value.Date
        End If

        If dtpToDate.Checked Then
            toDate = dtpToDate.Value.Date.AddDays(1) ' 🔴 مهم
        End If

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            ' =========================================
            ' 🔴 1) الرصيد الافتتاحي (قبل الفترة)
            ' =========================================
            Dim openingDebit As Decimal = 0D
            Dim openingCredit As Decimal = 0D
            Dim openingBalance As Decimal = 0D

            If dtpFromDate.Checked Then

                Using cmd As New SqlCommand("
SELECT 
    ISNULL(SUM(d.DebitAmount),0),
    ISNULL(SUM(d.CreditAmount),0)
FROM gl.JournalDetails d
JOIN gl.JournalHeader h ON d.JournalID = h.JournalID
WHERE d.AccountID = @A
AND h.JournalDate < @FromDate
", con)

                    cmd.Parameters.Add("@A", SqlDbType.Int).Value = accountId
                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = dtpFromDate.Value.Date

                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            openingDebit = CDec(rdr(0))
                            openingCredit = CDec(rdr(1))
                        End If
                    End Using

                End Using

            End If

            openingBalance = openingDebit - openingCredit

            ' =========================================
            ' 🔴 2) الحركات داخل الفترة
            ' =========================================
            Dim dt As New DataTable()

            Using cmd As New SqlCommand("
SELECT
    h.JournalDate,
    h.JournalID,
    op.OperationName,
    h.Description,
    d.DebitAmount,
    d.CreditAmount,
    (d.DebitAmount - d.CreditAmount) AS Movement
FROM gl.JournalDetails d
JOIN gl.JournalHeader h ON d.JournalID = h.JournalID
LEFT JOIN wf.OperationType op ON h.OperationTypeID = op.OperationTypeID
WHERE d.AccountID = @A
AND (@FromDate IS NULL OR h.JournalDate >= @FromDate)
AND (@ToDate IS NULL OR h.JournalDate < @ToDate)
ORDER BY h.JournalDate, h.JournalID
", con)

                cmd.Parameters.Add("@A", SqlDbType.Int).Value = accountId
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using

            ' =========================================
            ' 🔴 3) إضافة عمود الرصيد
            ' =========================================
            dt.Columns.Add("Balance", GetType(Decimal))

            Dim runningBalance As Decimal = openingBalance

            ' =========================================
            ' 🔴 4) صف الرصيد الافتتاحي (مهم جداً)
            ' =========================================
            Dim rowOpen = dt.NewRow()

            rowOpen("JournalDate") = If(dtpFromDate.Checked, dtpFromDate.Value.Date, DBNull.Value)
            rowOpen("Description") = "رصيد افتتاحي"
            rowOpen("DebitAmount") = openingDebit
            rowOpen("CreditAmount") = openingCredit
            rowOpen("Movement") = openingBalance
            rowOpen("Balance") = openingBalance

            dt.Rows.InsertAt(rowOpen, 0)

            ' =========================================
            ' 🔴 5) حساب الرصيد التراكمي
            ' =========================================
            For i As Integer = 1 To dt.Rows.Count - 1

                Dim movement As Decimal = If(IsDBNull(dt.Rows(i)("Movement")), 0D, CDec(dt.Rows(i)("Movement")))

                runningBalance += movement

                dt.Rows(i)("Balance") = runningBalance

            Next

            dgvAccountDetails.DataSource = dt

            ' =========================================
            ' 🔴 6) المجاميع (الآن صحيحة 100%)
            ' =========================================
            Dim totalDebit As Decimal = dt.AsEnumerable().
            Sum(Function(r) If(IsDBNull(r("DebitAmount")), 0D, CDec(r("DebitAmount"))))

            Dim totalCredit As Decimal = dt.AsEnumerable().
            Sum(Function(r) If(IsDBNull(r("CreditAmount")), 0D, CDec(r("CreditAmount"))))

            txtTotalDebit.Text = totalDebit.ToString("N2")
            txtTotalCredit.Text = totalCredit.ToString("N2")

            ' =========================================
            ' 🔴 7) الفرق
            ' =========================================
            Dim diff As Decimal = totalDebit - totalCredit

            txtDifference.Text = Math.Abs(diff).ToString("N2")

            If diff > 0 Then
                lblDifference.Text = "له"
            ElseIf diff < 0 Then
                lblDifference.Text = "عليه"
            Else
                lblDifference.Text = "متوازن"
            End If

        End Using

    End Sub
    Private Sub dgvAccountDetails_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAccountDetails.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub

        If dgvAccountDetails.Rows(e.RowIndex).Cells("JournalID").Value Is Nothing Then Exit Sub

        Dim journalId As Integer = CInt(dgvAccountDetails.Rows(e.RowIndex).Cells("JournalID").Value)

        '   OpenTransactionForm(journalId)

    End Sub
End Class