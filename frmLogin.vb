Imports System.Data.SqlClient

Public Class frmLogin
    Inherits AABaseOperationForm
    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadLoginUsers()
        txtPassword.Clear()
        txtPassword.Focus()
    End Sub

    Private Sub LoadLoginUsers()
        Try
            cboLoginName.DropDownStyle = ComboBoxStyle.DropDownList

            Using con As New SqlConnection(AppConfig.MainConnectionString)
                Using cmd As New SqlCommand("
                    SELECT EmployeeID, EmpName
                    FROM Security_Employee
                    WHERE IsSystemUser = 1
                      AND IsActive = 1
                    ORDER BY EmpName
                ", con)

                    Dim dt As New DataTable
                    con.Open()
                    dt.Load(cmd.ExecuteReader())

                    cboLoginName.DataSource = dt
                    cboLoginName.DisplayMember = "EmpName"
                    cboLoginName.ValueMember = "EmployeeID"
                    cboLoginName.SelectedValue = -1
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("خطأ تحميل المستخدمين" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

        If cboLoginName.SelectedValue Is Nothing _
        OrElse String.IsNullOrWhiteSpace(txtPassword.Text) Then
            MessageBox.Show("اختر المستخدم وأدخل كلمة المرور")
            Exit Sub
        End If

        Dim employeeID As Integer = CInt(cboLoginName.SelectedValue)
        Dim passwordHash As String = HashPassword(txtPassword.Text)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
                SELECT EmployeeID, EmpName, LoginName, RoleID, IsAdmin
                FROM Security_Employee
                WHERE EmployeeID = @ID
                  AND PasswordHash = @Pwd
                  AND IsSystemUser = 1
                  AND IsActive = 1
            ", con)

                cmd.Parameters.AddWithValue("@ID", employeeID)
                cmd.Parameters.AddWithValue("@Pwd", passwordHash)

                con.Open()
                Using r = cmd.ExecuteReader()
                    If Not r.Read() Then
                        MessageBox.Show("كلمة المرور غير صحيحة")
                        Exit Sub
                    End If
                    CurrentUser.EmployeeID = r("EmployeeID")
                    CurrentUser.EmpName = r("EmpName").ToString()
                    CurrentUser.LoginName = r("LoginName").ToString()
                    CurrentUser.IsAdmin = CBool(r("IsAdmin"))

                    ' RoleID قد يكون NULL
                    CurrentUser.RoleID =
                        If(IsDBNull(r("RoleID")), 0, CInt(r("RoleID")))
                End Using
            End Using
        End Using

        ' 🔐 تحميل الصلاحيات مرة واحدة فقط
        PermissionService.LoadForUser(CurrentUser.EmployeeID)

        ' 🔑 الخطوة الحاسمة
        Me.DialogResult = DialogResult.OK
        ' افتح الماستر
        Dim master As New frmMasterForm()
        master.Show()

        ' أخفِ اللوجن (لا Close)
        Me.Hide()

    End Sub
    Private Sub btnReSetPassword_Click(sender As Object, e As EventArgs) Handles btnReSetPassword.Click

        If cboLoginName.SelectedValue Is Nothing Then
            MessageBox.Show("اختر المستخدم أولاً")
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(txtPassword.Text) Then
            MessageBox.Show("أدخل كلمة المرور الجديدة")
            Exit Sub
        End If

        Dim employeeID As Integer = CInt(cboLoginName.SelectedValue)
        Dim newHash As String = HashPassword(txtPassword.Text.Trim())

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
            UPDATE Security_Employee
            SET PasswordHash = @Hash,
                MustChangePassword = 0
            WHERE EmployeeID = @ID
              AND IsSystemUser = 1
              AND IsActive = 1
        ", con)

                cmd.Parameters.AddWithValue("@Hash", newHash)
                cmd.Parameters.AddWithValue("@ID", employeeID)

                con.Open()
                Dim rows = cmd.ExecuteNonQuery()

                If rows = 0 Then
                    MessageBox.Show("لم يتم تحديث كلمة المرور")
                    Exit Sub
                End If
            End Using
        End Using

        MessageBox.Show("تم تغيير كلمة المرور بنجاح")
        txtPassword.Clear()
        txtPassword.Focus()

    End Sub

End Class
