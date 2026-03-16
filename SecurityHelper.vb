Imports System.Data.SqlClient

Public Module SecurityHelper

    Public Function HashPassword(password As String) As String
        Using sha As Security.Cryptography.SHA256 = Security.Cryptography.SHA256.Create()
            Dim bytes = Text.Encoding.UTF8.GetBytes(password)
            Dim hash = sha.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function

    Public Sub ResetPassword(employeeID As Integer, newPassword As String)
        Using con As New SqlConnection(AppConfig.MainConnectionString)
            Using cmd As New SqlCommand("
                UPDATE Employees
                SET PasswordHash = @Hash,
                    MustChangePassword = 1
                WHERE EmployeeID = @ID
            ", con)

                cmd.Parameters.AddWithValue("@Hash", HashPassword(newPassword))
                cmd.Parameters.AddWithValue("@ID", employeeID)

                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Module
