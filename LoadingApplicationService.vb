Imports System.Data.SqlClient

Public Class LoadingApplicationService

    Private ReadOnly _connStr As String

    Public Sub New(connectionString As String)
        _connStr = connectionString
    End Sub


    Private Sub TouchLoadingOrderModifiedAt(
    con As SqlConnection,
    tran As SqlTransaction,
    loID As Integer,
    userID As Integer
)
        Using cmd As New SqlCommand("
UPDATE dbo.Logistics_LoadingOrder
SET ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)
            cmd.Parameters.AddWithValue("@LOID", loID)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

End Class