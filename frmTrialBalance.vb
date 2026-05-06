Imports System.Data.SqlClient

Public Class frmTrialBalance
    Private Function GetPeriodID(docDate As Date, con As SqlConnection) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE @DocDate >= StartDate
AND @DocDate < DATEADD(DAY,1,EndDate)
AND IsOpen = 1
ORDER BY StartDate
", con)

            cmd.Parameters.AddWithValue("@DocDate", docDate)

            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                Throw New Exception("لا توجد فترة مالية مفتوحة لهذا التاريخ")
            End If

            Return CInt(result)

        End Using

    End Function
    Private Sub btnLoadData_Click(sender As Object, e As EventArgs) Handles btnLoadData.Click

        Try

            Using con As New SqlConnection(AppConfig.MainConnectionString)
                con.Open()

                Dim fromPeriod = GetPeriodID(dtpFromDate.Value, con)
                Dim toPeriod = GetPeriodID(dtpToDate.Value, con)

                Dim dt As New DataTable
                Dim level As Object = DBNull.Value
                If cboAccountLevel.SelectedIndex > 0 Then
                    level = CInt(cboAccountLevel.SelectedItem)
                Else
                    level = DBNull.Value
                End If

                Dim onlyWithBalance As Integer = If(chkShowOnlyWithBalance.Checked, 1, 0)
                Using cmd As New SqlCommand("
SELECT 
    parent.AccountCode,

    REPLICATE('    ', parent.AccountLevel - 1) 
    + ISNULL(p2.AccountNameAr + ' - ', '') 
    + parent.AccountNameAr AS AccountNameAr,

    SUM(ISNULL(d.DebitAmount,0)) AS TotalDebit,
    SUM(ISNULL(d.CreditAmount,0)) AS TotalCredit

FROM gl.Accounts parent

LEFT JOIN gl.Accounts child
    ON child.FullPath LIKE parent.FullPath + '%'

LEFT JOIN gl.JournalDetails d
    ON d.AccountID = child.AccountID

LEFT JOIN gl.JournalHeader j
    ON j.JournalID = d.JournalID
   AND j.JournalDate >= @FromDate
   AND j.JournalDate < DATEADD(DAY,1,@ToDate)

LEFT JOIN gl.Accounts p2
    ON p2.AccountID = parent.ParentAccountID

WHERE 
    (@Level IS NULL OR parent.AccountLevel <= @Level)

GROUP BY 
    parent.AccountCode,
    parent.AccountNameAr,
    parent.AccountLevel,
    p2.AccountNameAr

HAVING 
(
    @OnlyWithBalance = 0
    OR
    SUM(ISNULL(d.DebitAmount,0)) <> 0
    OR
    SUM(ISNULL(d.CreditAmount,0)) <> 0
)

ORDER BY parent.AccountCode
", con)

                    cmd.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date)
                    cmd.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date)
                    cmd.Parameters.AddWithValue("@Level", level)
                    cmd.Parameters.AddWithValue("@OnlyWithBalance", onlyWithBalance)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using

                End Using

                dgvTrialBalance.DataSource = dt

                ApplyTrialBalanceStyle()

                LoadTrialBalanceTotals(con)
                If dt.Rows.Count = 0 Then
                    MessageBox.Show("لا توجد بيانات للفترة المحددة")
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub LoadTrialBalanceTotals(con As SqlConnection)

        Using cmd As New SqlCommand("
SELECT
    ISNULL(SUM(d.DebitAmount), 0)  AS TotalDebit,
    ISNULL(SUM(d.CreditAmount), 0) AS TotalCredit
FROM gl.JournalHeader h
INNER JOIN gl.JournalDetails d 
    ON d.JournalID = h.JournalID
WHERE h.JournalDate >= @FromDate
  AND h.JournalDate < DATEADD(DAY, 1, @ToDate)
  AND h.IsPosted = 1
", con)

            cmd.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date)
            cmd.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date)

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then

                    Dim totalDebit As Decimal = CDec(rd("TotalDebit"))
                    Dim totalCredit As Decimal = CDec(rd("TotalCredit"))
                    Dim diff As Decimal = totalDebit - totalCredit

                    txtTotalDebit.Text = totalDebit.ToString("N2")
                    txtTotalCredit.Text = totalCredit.ToString("N2")
                    txtDifference.Text = diff.ToString("N2")

                    txtDifference.ForeColor =
                    If(Math.Round(diff, 2) = 0D, Color.Green, Color.Red)

                End If
            End Using

        End Using

    End Sub
    Private Sub ApplyTrialBalanceStyle()

        With dgvTrialBalance

            .RightToLeft = RightToLeft.Yes
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            .Columns("AccountCode").HeaderText = "كود الحساب"
            .Columns("AccountNameAr").HeaderText = "اسم الحساب"
            .Columns("TotalDebit").HeaderText = "مدين"
            .Columns("TotalCredit").HeaderText = "دائن"

            .Columns("AccountNameAr").FillWeight = 50
            .Columns("AccountCode").FillWeight = 20
            .Columns("TotalDebit").FillWeight = 15
            .Columns("TotalCredit").FillWeight = 15

            .Columns("TotalDebit").DefaultCellStyle.Format = "N2"
            .Columns("TotalCredit").DefaultCellStyle.Format = "N2"

            .Columns("TotalDebit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("TotalCredit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowHeadersVisible = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .GridColor = Color.LightGray
            .DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue
        End With

    End Sub
    Private Sub CalculateTotals(dt As DataTable)

        Dim totalDebit As Decimal = 0
        Dim totalCredit As Decimal = 0

        For Each row As DataRow In dt.Rows

            Dim accountCode As String = row("AccountCode").ToString()

            ' 🔥 إذا الحساب له أبناء → تجاهله
            Dim hasChildren = dt.AsEnumerable().
        Any(Function(r) r("AccountCode").ToString().StartsWith(accountCode) _
        AndAlso r("AccountCode").ToString() <> accountCode)

            If Not hasChildren Then
                totalDebit += CDec(row("TotalDebit"))
                totalCredit += CDec(row("TotalCredit"))
            End If

        Next

        txtTotalDebit.Text = totalDebit.ToString("N2")
        txtTotalCredit.Text = totalCredit.ToString("N2")

        Dim diff = totalDebit - totalCredit
        txtDifference.Text = diff.ToString("N2")

        ' 🔥 تلوين احترافي
        If diff = 0 Then
            txtDifference.ForeColor = Color.Green
        Else
            txtDifference.ForeColor = Color.Red
        End If

    End Sub
    Private Sub finTrialBalance_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' 🔷 تحميل المستويات أولاً
        LoadAccountLevels()

        ' 🔷 تاريخ افتراضي
        dtpFromDate.Value = New Date(Now.Year, Now.Month, 1)
        dtpToDate.Value = Now.Date

        ' 🔷 تحميل البيانات
        btnLoadData.PerformClick()
        dtpFromDate.Format = DateTimePickerFormat.Custom
        dtpFromDate.CustomFormat = "yyyy-MM-dd"

        dtpToDate.Format = DateTimePickerFormat.Custom
        dtpToDate.CustomFormat = "yyyy-MM-dd"
    End Sub
    Private Sub LoadAccountLevels()

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim maxLevel As Integer = 1

            Using cmd As New SqlCommand("
SELECT ISNULL(MAX(AccountLevel),1)
FROM gl.Accounts
", con)

                Dim result = cmd.ExecuteScalar()

                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    maxLevel = CInt(result)
                End If

            End Using

            ' 🔷 تعبئة الكبوا
            cboAccountLevel.Items.Clear()

            ' أول خيار = الكل
            cboAccountLevel.Items.Add("الكل")

            For i As Integer = 1 To maxLevel
                cboAccountLevel.Items.Add(i)
            Next

            ' 🔷 اختيار افتراضي
            cboAccountLevel.SelectedIndex = 0

        End Using

    End Sub
End Class