Imports System.Data.SqlClient

Public Class AuditHelper

    Public Shared Sub Write(
        con As SqlConnection,
        tran As SqlTransaction,
        operationTypeID As Integer,
        entityID As Integer,
        actionCode As String,
        oldStatusID As Integer?,
        newStatusID As Integer?,
        userID As Integer,
        Optional referenceNumber As String = Nothing,
        Optional reason As String = Nothing,
        Optional extraData As String = Nothing
    )

        Using cmd As New SqlCommand("cfg.WriteAudit", con, tran)

            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
            cmd.Parameters.AddWithValue("@EntityID", entityID)
            cmd.Parameters.AddWithValue("@ActionCode", actionCode)

            cmd.Parameters.AddWithValue("@OldStatusID",
                If(oldStatusID.HasValue, oldStatusID.Value, DBNull.Value))

            cmd.Parameters.AddWithValue("@NewStatusID",
                If(newStatusID.HasValue, newStatusID.Value, DBNull.Value))

            cmd.Parameters.AddWithValue("@ChangedBy", userID)

            cmd.Parameters.AddWithValue("@ReferenceNumber",
                If(String.IsNullOrWhiteSpace(referenceNumber), DBNull.Value, referenceNumber))

            cmd.Parameters.AddWithValue("@Reason",
                If(String.IsNullOrWhiteSpace(reason), DBNull.Value, reason))

            cmd.Parameters.AddWithValue("@ExtraData",
                If(String.IsNullOrWhiteSpace(extraData), DBNull.Value, extraData))

            cmd.ExecuteNonQuery()

        End Using

    End Sub

End Class
