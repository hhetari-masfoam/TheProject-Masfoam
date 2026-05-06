Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class frmfinChartOfAccounts
    Public ParentAccountID As Integer
    Private CurrentAccountTypeID As Integer
    Private CurrentAccountNatureID As Integer
    Private IsNewMode As Boolean = False
    Private CurrentAccountID As Integer = 0
    Private _selectedAccountID As Integer = 0
    Private _isLoadingHeaders As Boolean = False
    Private Sub LoadAccountsTree()

        tvChartOfAccounts.Nodes.Clear()

        Dim dt As New DataTable

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
            SELECT AccountID, AccountCode, AccountNameAr, ParentAccountID
            FROM gl.Accounts
            ORDER BY FullPath
        ", con)

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

            End Using
        End Using

        Dim dict As New Dictionary(Of Integer, TreeNode)

        For Each row As DataRow In dt.Rows

            Dim id As Integer = CInt(row("AccountID"))
            Dim code As String = row("AccountCode").ToString()
            Dim name As String = row("AccountNameAr").ToString()

            Dim node As New TreeNode(code & " - " & name)
            node.Tag = id

            dict.Add(id, node)

            If IsDBNull(row("ParentAccountID")) Then
                tvChartOfAccounts.Nodes.Add(node)
            Else
                Dim parentID As Integer = CInt(row("ParentAccountID"))
                dict(parentID).Nodes.Add(node)
            End If

        Next

        'tvChartOfAccounts.ExpandAll()

    End Sub
    Private Sub TreeView1_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvChartOfAccounts.NodeMouseClick

        tvChartOfAccounts.SelectedNode = e.Node

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        Try
            Dim childDigitsValue As Object

            If chkPostable.Checked Then
                childDigitsValue = DBNull.Value
            Else
                If cboChildDigits.SelectedIndex <= 0 Then
                    MessageBox.Show("يجب تحديد عدد الخانات")
                    Exit Sub
                End If

                childDigitsValue = CInt(cboChildDigits.SelectedItem)
            End If
            If Not chkPostable.Checked Then
                If HasTransactions(CurrentAccountID) Then
                    MessageBox.Show("لا يمكن تحويل حساب مستخدم إلى حساب أب")
                    Exit Sub
                End If
            End If
            Using con As New SqlConnection(AppConfig.MainConnectionString)
                con.Open()

                If IsNewMode Then

                    Using cmd As New SqlCommand("gl.AddAccount", con)

                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@AccountCode", txtAccountCode.Text)
                        cmd.Parameters.AddWithValue("@AccountNameAr", txtAccountName.Text)
                        cmd.Parameters.AddWithValue("@AccountTypeID", CurrentAccountTypeID)
                        cmd.Parameters.AddWithValue("@AccountNatureID", CurrentAccountNatureID)
                        cmd.Parameters.AddWithValue("@ParentAccountID", ParentAccountID)
                        cmd.Parameters.AddWithValue("@IsPostable", chkPostable.Checked)
                        cmd.Parameters.AddWithValue("@IsControlAccount", chkControl.Checked)
                        cmd.Parameters.AddWithValue("@AllowCostCenter", chkCostCenter.Checked)
                        cmd.Parameters.AddWithValue("@IsSystem", 0)
                        cmd.Parameters.AddWithValue("@SortOrder", 1)
                        cmd.Parameters.AddWithValue("@ChildDigits", childDigitsValue)
                        ' 🔥 مهم
                        Dim outParam As New SqlParameter("@NewAccountID", SqlDbType.Int)
                        outParam.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outParam)

                        cmd.ExecuteNonQuery()

                    End Using

                Else

                    Using cmd As New SqlCommand("
UPDATE gl.Accounts
SET 
    AccountNameAr = @Name,
    IsPostable = @Postable,
    IsControlAccount = @Control,
    AllowCostCenter = @Cost,
    IsActive = @Active,
    Notes = @Notes,
    ChildDigits = @ChildDigits   -- 🔥 هذا الناقص
WHERE AccountID = @ID
", con)

                        cmd.Parameters.AddWithValue("@Name", txtAccountName.Text)
                        cmd.Parameters.AddWithValue("@Postable", chkPostable.Checked)
                        cmd.Parameters.AddWithValue("@Control", chkControl.Checked)
                        cmd.Parameters.AddWithValue("@Cost", chkCostCenter.Checked)
                        cmd.Parameters.AddWithValue("@Active", chkActive.Checked)
                        cmd.Parameters.AddWithValue("@Notes", txtNotes.Text)
                        cmd.Parameters.AddWithValue("@ID", CurrentAccountID)
                        cmd.Parameters.AddWithValue("@ChildDigits", childDigitsValue)
                        cmd.ExecuteNonQuery()

                    End Using

                End If

            End Using

            LoadAccountsTree()
            MessageBox.Show("تم الحفظ بنجاح")

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles mnuAddChildAccount.Click
        If tvChartOfAccounts.SelectedNode Is Nothing Then Exit Sub

        Dim parentID As Integer = CInt(tvChartOfAccounts.SelectedNode.Tag)

        '  Dim frm As New frmAddAccount
        ' frm.ParentAccountID = parentID

        ' If frm.ShowDialog() = DialogResult.OK Then
        'LoadAccountsTree()
        ' End If
    End Sub
    Private Sub frmChartOfAccounts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadAccountsTree()
        cboChildDigits.Items.Clear()

        cboChildDigits.Items.Add("")   ' يمثل NULL
        cboChildDigits.Items.Add(1)
        cboChildDigits.Items.Add(2)
        cboChildDigits.Items.Add(3)
        cboChildDigits.Items.Add(4)
        cboChildDigits.Items.Add(5)

        cboChildDigits.DropDownStyle = ComboBoxStyle.DropDownList
    End Sub
    Private Sub tvChartOfAccounts_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvChartOfAccounts.AfterSelect

        If tvChartOfAccounts.SelectedNode Is Nothing Then Exit Sub

        Dim accountID As Integer = CInt(tvChartOfAccounts.SelectedNode.Tag)
        _selectedAccountID = accountID
        LoadAccountHeader(accountID)

    End Sub
    Private Sub LoadAccountHeader(accountID As Integer)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
SELECT 
    a.*,
    t.AccountTypeNameAr,
    n.AccountNatureNameAr,
    p.AccountCode AS ParentCode,
    p.AccountNameAr AS ParentName
FROM gl.Accounts a
LEFT JOIN gl.AccountTypes t ON a.AccountTypeID = t.AccountTypeID
LEFT JOIN gl.AccountNatures n ON a.AccountNatureID = n.AccountNatureID
LEFT JOIN gl.Accounts p ON a.ParentAccountID = p.AccountID
WHERE a.AccountID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", accountID)

                con.Open()

                Using rd = cmd.ExecuteReader()

                    If rd.Read() Then

                        CurrentAccountID = accountID
                        IsNewMode = False

                        txtAccountCode.Text = rd("AccountCode").ToString()
                        txtAccountName.Text = rd("AccountNameAr").ToString()

                        If IsDBNull(rd("ParentCode")) Then
                            txtAccountParent.Text = ""
                        Else
                            txtAccountParent.Text = rd("ParentCode").ToString() & " - " & rd("ParentName").ToString()
                        End If
                        txtAccountLevel.Text = rd("AccountLevel").ToString()
                        txtAccountLevel.Text = rd("AccountLevel").ToString()
                        txtAccountLevel.Text = rd("AccountLevel").ToString()

                        txtAccountType.Text = rd("AccountTypeNameAr").ToString()
                        txtAccountNature.Text = rd("AccountNatureNameAr").ToString()

                        CurrentAccountTypeID = CInt(rd("AccountTypeID"))
                        CurrentAccountNatureID = CInt(rd("AccountNatureID"))

                        chkPostable.Checked = CBool(rd("IsPostable"))
                        chkControl.Checked = CBool(rd("IsControlAccount"))
                        chkCostCenter.Checked = CBool(rd("AllowCostCenter"))
                        chkActive.Checked = CBool(rd("IsActive"))

                        txtNotes.Text = rd("Notes").ToString()
                        If IsDBNull(rd("ChildDigits")) Then
                            cboChildDigits.SelectedIndex = 0   ' فارغ = NULL
                        Else
                            cboChildDigits.SelectedItem = CInt(rd("ChildDigits"))
                        End If
                        ' 🔥 تطبيق وضع التعديل
                        SetFormMode(False, False)

                    End If

                End Using
            End Using
        End Using

    End Sub
    Private Sub mnuEditAccount_Click(sender As Object, e As EventArgs) Handles mnuEditAccount.Click

        If CurrentAccountID = 0 Then Exit Sub

        IsNewMode = False

        ' 🔥 فتح التعديل
        SetFormMode(False, True)

    End Sub
    Private Sub mnuAddChildAccount_Click(sender As Object, e As EventArgs) Handles mnuAddChildAccount.Click

        If tvChartOfAccounts.SelectedNode Is Nothing Then Exit Sub

        Dim parentID As Integer = CInt(tvChartOfAccounts.SelectedNode.Tag)

        Dim msg As String = ""
        If Not CanAddChildAccount(parentID, msg) Then
            MessageBox.Show(msg)
            Exit Sub
        End If
        ' ❌ منع Postable

        ' 🔥 وضع الإضافة
        IsNewMode = True
        CurrentAccountID = 0
        ParentAccountID = parentID

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
SELECT 
    a.*,
    t.AccountTypeNameAr,
    n.AccountNatureNameAr
FROM gl.Accounts a
LEFT JOIN gl.AccountTypes t ON a.AccountTypeID = t.AccountTypeID
LEFT JOIN gl.AccountNatures n ON a.AccountNatureID = n.AccountNatureID
WHERE a.AccountID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", parentID)
                con.Open()

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        txtAccountParent.Text = parentID.ToString()
                        txtAccountLevel.Text = (CInt(rd("AccountLevel")) + 1).ToString()

                        txtAccountType.Text = rd("AccountTypeNameAr").ToString()
                        txtAccountNature.Text = rd("AccountNatureNameAr").ToString()

                        CurrentAccountTypeID = CInt(rd("AccountTypeID"))
                        CurrentAccountNatureID = CInt(rd("AccountNatureID"))

                        Try
                            txtAccountCode.Text = GenerateAccountCode(parentID)
                        Catch ex As Exception
                            MessageBox.Show("خطأ في توليد كود الحساب: " & ex.Message)
                            Exit Sub
                        End Try

                        chkPostable.Checked = True
                        chkControl.Checked = False

                        txtAccountName.Clear()
                        txtNotes.Clear()

                        ' 🔥 تطبيق وضع الإضافة
                        SetFormMode(True, True)

                    End If
                End Using
            End Using
        End Using

    End Sub
    Private Sub mnuDeleteAccount_Click(sender As Object, e As EventArgs) Handles mnuDeleteAccount.Click

        If CurrentAccountID = 0 Then Exit Sub

        If MessageBox.Show("هل تريد حذف الحساب؟", "تأكيد", MessageBoxButtons.YesNo) = DialogResult.No Then Exit Sub

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            ' 🔴 منع حذف System
            Using cmd As New SqlCommand("SELECT IsSystem FROM gl.Accounts WHERE AccountID=@ID", con)
                cmd.Parameters.AddWithValue("@ID", CurrentAccountID)

                If CBool(cmd.ExecuteScalar()) Then
                    MessageBox.Show("لا يمكن حذف حساب نظام")
                    Exit Sub
                End If
            End Using

            ' 🔴 منع إذا فيه قيود
            Using cmd As New SqlCommand("
SELECT TOP 1 1 
FROM gl.JournalDetails 
WHERE AccountID=@ID
", con)

                cmd.Parameters.AddWithValue("@ID", CurrentAccountID)

                If cmd.ExecuteScalar() IsNot Nothing Then
                    MessageBox.Show("لا يمكن حذف الحساب لوجود قيود")
                    Exit Sub
                End If
            End Using

            ' 🔴 منع إذا عنده أبناء
            Using cmd As New SqlCommand("
SELECT TOP 1 1 
FROM gl.Accounts 
WHERE ParentAccountID=@ID
", con)

                cmd.Parameters.AddWithValue("@ID", CurrentAccountID)

                If cmd.ExecuteScalar() IsNot Nothing Then
                    MessageBox.Show("لا يمكن حذف الحساب لوجود حسابات فرعية")
                    Exit Sub
                End If
            End Using

            ' 🟢 الحذف
            Using cmd As New SqlCommand("DELETE FROM gl.Accounts WHERE AccountID=@ID", con)
                cmd.Parameters.AddWithValue("@ID", CurrentAccountID)
                cmd.ExecuteNonQuery()
            End Using

        End Using

        LoadAccountsTree()
        MessageBox.Show("تم الحذف")

    End Sub
    Private Sub mnuRefresh_Click(sender As Object, e As EventArgs) Handles mnuRefresh.Click
        LoadAccountsTree()
    End Sub
    Private Function GenerateAccountCode(parentID As Integer) As String

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim parentCode As String = ""
            Dim childDigits As Integer = 0

            ' 1) جلب كود الأب وعدد خانات الأبناء
            Using cmd As New SqlCommand("
SELECT AccountCode, ChildDigits
FROM gl.Accounts
WHERE AccountID = @ID
", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = parentID

                Using rd = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        Throw New Exception("لم يتم العثور على الحساب الأب")
                    End If

                    parentCode = rd("AccountCode").ToString()

                    If IsDBNull(rd("ChildDigits")) Then
                        Throw New Exception("هذا الحساب لا يسمح بإضافة أبناء لأن عدد خانات الأبناء غير محدد")
                    End If

                    childDigits = CInt(rd("ChildDigits"))

                    If childDigits <= 0 Then
                        Throw New Exception("عدد خانات الأبناء غير صحيح")
                    End If
                End Using
            End Using

            ' 2) جلب أكبر رقم ابن فعلي
            Dim lastChildNumber As Integer = 0

            Using cmd As New SqlCommand("
SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(AccountCode, LEN(@ParentCode) + 1, @ChildDigits) AS INT)), 0)
FROM gl.Accounts
WHERE ParentAccountID = @ParentID
  AND AccountCode LIKE @ParentCode + '%'
  AND LEN(AccountCode) = LEN(@ParentCode) + @ChildDigits
", con)

                cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentID
                cmd.Parameters.Add("@ParentCode", SqlDbType.NVarChar, 50).Value = parentCode
                cmd.Parameters.Add("@ChildDigits", SqlDbType.Int).Value = childDigits

                Dim obj = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    Integer.TryParse(obj.ToString(), lastChildNumber)
                Else
                    lastChildNumber = 0
                End If
            End Using

            Dim nextNum As Integer = lastChildNumber + 1

            Return parentCode & nextNum.ToString("D" & childDigits)

        End Using

    End Function
    Private Function IsControlAccount(accountID As Integer) As Boolean

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
SELECT IsControlAccount 
FROM gl.Accounts 
WHERE AccountID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", accountID)

                con.Open()

                Return CBool(cmd.ExecuteScalar())

            End Using
        End Using

    End Function
    Private Sub SetFormMode(isNew As Boolean, isEdit As Boolean)

        ' 🔴 حقول ثابتة دائماً
        txtAccountCode.ReadOnly = True
        txtAccountParent.ReadOnly = True
        txtAccountLevel.ReadOnly = True
        txtAccountType.ReadOnly = True
        txtAccountNature.ReadOnly = True

        ' 🔵 وضع عرض فقط
        If Not isNew AndAlso Not isEdit Then

            txtAccountName.ReadOnly = True
            txtNotes.ReadOnly = True

            chkPostable.Enabled = False
            chkControl.Enabled = False
            chkActive.Enabled = False
            chkCostCenter.Enabled = False
            cboChildDigits.Enabled = False

            btnSave.Enabled = False

            Exit Sub
        End If

        ' 🟢 وضع تعديل أو إضافة
        txtAccountName.ReadOnly = False
        txtNotes.ReadOnly = False

        chkPostable.Enabled = True
        chkControl.Enabled = True
        chkActive.Enabled = True
        chkCostCenter.Enabled = True
        cboChildDigits.Enabled = Not chkPostable.Checked

        btnSave.Enabled = True

        If isNew Then
            btnSave.Text = "حفظ"
        Else
            btnSave.Text = "تعديل"
        End If

    End Sub

    Private Sub chkControl_CheckedChanged(sender As Object, e As EventArgs) Handles chkControl.CheckedChanged

    End Sub

    Private Sub chkPostable_CheckedChanged(sender As Object, e As EventArgs) Handles chkPostable.CheckedChanged
        If chkPostable.Checked Then
            cboChildDigits.SelectedIndex = 0
            cboChildDigits.Enabled = False
        Else
            cboChildDigits.Enabled = True
        End If
    End Sub

    Private Function HasTransactions(accountID As Integer) As Boolean
        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
SELECT TOP 1 1
FROM gl.JournalDetails
WHERE AccountID = @ID
", con)

                cmd.Parameters.AddWithValue("@ID", accountID)
                con.Open()

                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        End Using
    End Function

    Private Function CanAddChildAccount(parentID As Integer, ByRef message As String) As Boolean

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
SELECT IsPostable, ChildDigits
FROM gl.Accounts
WHERE AccountID = @ID
", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = parentID
                con.Open()

                Using rd = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        message = "لم يتم العثور على الحساب الأب"
                        Return False
                    End If

                    If CBool(rd("IsPostable")) Then
                        message = "لا يمكن إضافة فرع تحت حساب نهائي"
                        Return False
                    End If

                    If IsDBNull(rd("ChildDigits")) Then
                        message = "لا يمكن إضافة فرع لأن عدد خانات الأبناء غير محدد"
                        Return False
                    End If
                End Using
            End Using
        End Using

        Return True

    End Function

    Private Sub btnViewJournalEntries_Click(sender As Object, e As EventArgs) Handles btnViewJournalEntries.Click
        If _selectedAccountID = 0 Then
            MessageBox.Show("اختر حساب أولاً")
            Exit Sub
        End If

        LoadJournalHeaders(_selectedAccountID)
        If dgvJournalHeaders.Rows.Count > 0 Then
            dgvJournalHeaders.Rows(0).Selected = True
        End If

    End Sub
    Private Sub dgvJournalHeaders_SelectionChanged(sender As Object, e As EventArgs) Handles dgvJournalHeaders.SelectionChanged

        If _isLoadingHeaders Then Exit Sub   ' 🔥 يمنع التشغيل أثناء التحميل

        If dgvJournalHeaders.CurrentRow Is Nothing Then Exit Sub
        If dgvJournalHeaders.CurrentRow.Cells("JournalID").Value Is Nothing Then Exit Sub

        Dim journalID As Integer = CInt(dgvJournalHeaders.CurrentRow.Cells("JournalID").Value)

        LoadJournalDetails(journalID)

    End Sub

    Private Sub LoadJournalHeaders(accountID As Integer)

        _isLoadingHeaders = True

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            ' 🔴 1) جلب FullPath
            Dim fullPath As String = ""

            Using cmdPath As New SqlCommand("
SELECT FullPath 
FROM gl.Accounts 
WHERE AccountID = @ID
", con)

                cmdPath.Parameters.AddWithValue("@ID", accountID)
                fullPath = cmdPath.ExecuteScalar().ToString()

            End Using

            Dim dt As New DataTable

            ' 🔴 2) جلب كل القيود للأب + الأبناء
            Using cmd As New SqlCommand("
SELECT DISTINCT
    j.JournalID,
    j.JournalNo,
    j.JournalDate,
    j.Description,
    j.TotalDebit,
    j.TotalCredit,
    jt.JournalTypeNameAr
FROM gl.JournalHeader j
INNER JOIN gl.JournalDetails d 
    ON d.JournalID = j.JournalID
INNER JOIN gl.Accounts a 
    ON a.AccountID = d.AccountID
INNER JOIN gl.JournalTypes jt 
    ON jt.JournalTypeID = j.JournalTypeID
WHERE a.FullPath LIKE @FullPath + '%'
ORDER BY j.JournalDate DESC
", con)

                cmd.Parameters.AddWithValue("@FullPath", fullPath)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using

            dgvJournalHeaders.DataSource = dt
            With dgvJournalHeaders
                dgvJournalHeaders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .RightToLeft = RightToLeft.Yes
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

                .Columns("JournalTypeNameAr").HeaderText = "نوع القيد"
                .Columns("TotalCredit").HeaderText = "دائن"
                .Columns("TotalDebit").HeaderText = "مدين"
                .Columns("Description").HeaderText = "البيان"
                .Columns("JournalDate").HeaderText = "التاريخ"
                .Columns("JournalNo").HeaderText = "رقم القيد"
                .Columns("JournalID").HeaderText = "رقم"

                ' 🔷 توزيع
                .Columns("JournalTypeNameAr").FillWeight = 15
                .Columns("TotalCredit").FillWeight = 15
                .Columns("TotalDebit").FillWeight = 15
                .Columns("Description").FillWeight = 30
                .Columns("JournalDate").FillWeight = 10
                .Columns("JournalNo").FillWeight = 10
                .Columns("JournalID").FillWeight = 5

                ' 🔷 تنسيق
                .Columns("TotalDebit").DefaultCellStyle.Format = "N2"
                .Columns("TotalCredit").DefaultCellStyle.Format = "N2"
                dgvJournalHeaders.Columns("TotalDebit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                dgvJournalHeaders.Columns("TotalCredit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
        End Using

        _isLoadingHeaders = False

    End Sub

    Private Sub LoadJournalDetails(journalID As Integer)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable

            ' 🔷 SQL منظم
            Using cmd As New SqlCommand("
SELECT 
    d.LineNumber,
    a.AccountCode,
    ISNULL(p.AccountNameAr + ' - ', '') + a.AccountNameAr AS AccountDisplayName,
    d.DebitAmount,
    d.CreditAmount,
    d.ProductID
FROM gl.JournalDetails d
INNER JOIN gl.Accounts a 
    ON a.AccountID = d.AccountID
LEFT JOIN gl.Accounts p 
    ON p.AccountID = a.ParentAccountID
WHERE d.JournalID = @JournalID
ORDER BY d.LineNumber
", con)

                cmd.Parameters.AddWithValue("@JournalID", journalID)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using

            ' 🔷 تنظيف الجريد
            dgvJournalDetails.AutoGenerateColumns = True
            dgvJournalDetails.Columns.Clear()
            dgvJournalDetails.DataSource = dt

            With dgvJournalDetails
                dgvJournalDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .RightToLeft = RightToLeft.Yes
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .ScrollBars = ScrollBars.Vertical

                ' 🔷 توزيع المساحات
                .Columns("LineNumber").FillWeight = 10
                .Columns("AccountCode").FillWeight = 20
                .Columns("AccountDisplayName").FillWeight = 50
                .Columns("DebitAmount").FillWeight = 10
                .Columns("CreditAmount").FillWeight = 10


                .Columns("LineNumber").HeaderText = "السطر"
                .Columns("AccountCode").HeaderText = "كود الحساب"
                .Columns("AccountDisplayName").HeaderText = "اسم الحساب"
                .Columns("DebitAmount").HeaderText = "مدين"
                .Columns("CreditAmount").HeaderText = "دائن"


                ' 🔷 الحد الأدنى (مهم جدًا لمنع الاختفاء)
                .Columns("AccountDisplayName").MinimumWidth = 200
                .Columns("AccountCode").MinimumWidth = 100
                .Columns("DebitAmount").MinimumWidth = 90
                .Columns("CreditAmount").MinimumWidth = 90

                ' 🔷 محاذاة
                dgvJournalDetails.Columns("DebitAmount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                dgvJournalDetails.Columns("CreditAmount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                ' 🔷 تنسيق
                .Columns("DebitAmount").DefaultCellStyle.Format = "N2"
                .Columns("CreditAmount").DefaultCellStyle.Format = "N2"

            End With

            ' 🔷 تلوين مدين/دائن
            For Each row As DataGridViewRow In dgvJournalDetails.Rows

                If Not IsDBNull(row.Cells("DebitAmount").Value) AndAlso CDec(row.Cells("DebitAmount").Value) > 0 Then
                    row.Cells("DebitAmount").Style.ForeColor = Color.Blue
                End If

                If Not IsDBNull(row.Cells("CreditAmount").Value) AndAlso CDec(row.Cells("CreditAmount").Value) > 0 Then
                    row.Cells("CreditAmount").Style.ForeColor = Color.Red
                End If

            Next

            ' 🔷 إخفاء الأعمدة غير المهمة
            If dgvJournalDetails.Columns.Contains("ProductID") Then
                dgvJournalDetails.Columns("ProductID").Visible = False
            End If

        End Using

    End Sub
End Class