Imports System.Data.SqlClient

Public Module PermissionService

    Private ReadOnly _permissions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    Private _isLoaded As Boolean = False

    ' ======================================
    ' تحميل الصلاحيات مرة واحدة بعد Login
    ' ======================================
    Public Sub LoadForUser(employeeID As Integer)

        _permissions.Clear()

        Using con As New SqlConnection(AppConfig.MainConnectionString)

            Using cmd As New SqlCommand("
            SELECT DISTINCT p.PermissionCode
            FROM Security_EmployeeRole er
            INNER JOIN Security_RolePermission rp ON rp.RoleID = er.RoleID
            INNER JOIN Security_Permission p ON p.PermissionID = rp.PermissionID
            WHERE er.EmployeeID = @EmployeeID
              AND er.IsActive = 1
              AND rp.IsAllowed = 1
              AND p.IsActive = 1
        ", con)

                cmd.Parameters.AddWithValue("@EmployeeID", employeeID)

                con.Open()
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        _permissions.Add(r("PermissionCode").ToString())
                    End While
                End Using

            End Using
        End Using

        _isLoaded = True
    End Sub

    ' ======================================
    ' تحقق ناعم (True / False)
    ' ======================================
    Public Function Can(permissionCode As String) As Boolean
        If Not _isLoaded Then
            Throw New InvalidOperationException("Permissions not loaded.")
        End If

        Return _permissions.Contains(permissionCode)
    End Function

    ' ======================================
    ' تحقق صارم (يوقف التنفيذ)
    ' ======================================
    Public Sub Require(permissionCode As String)

        If Not Can(permissionCode) Then
            ShowPermissionMessage(permissionCode)
            Throw New UnauthorizedAccessException(permissionCode)
        End If

    End Sub
    Private Sub ShowPermissionMessage(permissionCode As String)

        Dim msg As String = "لا تملك الصلاحية لتنفيذ هذا الإجراء."

        Select Case permissionCode
            Case "PURCHASE_OPEN"
                msg = "لا تملك صلاحية فتح شاشة المشتريات."
            Case "PURCHASE_SAVE"
                msg = "لا تملك صلاحية حفظ فاتورة المشتريات."
            Case "PURCHASE_CANCEL"
                msg = "لا تملك صلاحية إلغاء فاتورة المشتريات."
        End Select

        MessageBox.Show(msg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)

    End Sub

End Module
