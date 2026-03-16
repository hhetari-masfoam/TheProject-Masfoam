Imports System.Data.SqlClient

Public Class WorkflowEngine
    Public Function GetOperationTypeID(
    operationCode As String,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer

        Using cmd As New SqlCommand("
        SELECT OperationTypeID
        FROM Workflow_OperationType
        WHERE OperationCode = @OperationCode
          AND IsActive = 1", con, tran)

            cmd.Parameters.AddWithValue("@OperationCode", operationCode)

            Dim result = cmd.ExecuteScalar()

            If result Is Nothing OrElse IsDBNull(result) Then
                Throw New Exception("OperationType غير معرف")
            End If

            Return CInt(result)
        End Using

    End Function

    ' ===============================================
    ' مكان الإضافة: داخل Class WorkflowEngine
    ' ===============================================

    ''' <summary>
    ''' الحصول على كود التصحيح التالي
    ''' </summary>
    Public Function GetNextCorrectionCode(con As SqlConnection, tran As SqlTransaction) As String
        Dim cmd As New SqlCommand("cfg.GetNextCode", con, tran)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@CodeType", "COR")

        Dim nextCodeParam = New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
        nextCodeParam.Direction = ParameterDirection.Output
        cmd.Parameters.Add(nextCodeParam)

        cmd.ExecuteNonQuery()

        Return nextCodeParam.Value.ToString()
    End Function

    ''' <summary>
    ''' تحديث حالة الحركة
    ''' </summary>
    Public Sub UpdateTransactionStatus(transactionID As Integer, statusID As Integer, userID As Integer, con As SqlConnection, tran As SqlTransaction)
        Dim sql = "
        UPDATE Inventory_TransactionHeader 
        SET StatusID = @StatusID,
            IsInventoryPosted = 0,
            CreatedBy = @UserID,
            CreatedAt = GETDATE()
        WHERE TransactionID = @TransactionID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@TransactionID", transactionID)
        cmd.Parameters.AddWithValue("@StatusID", statusID)
        cmd.Parameters.AddWithValue("@UserID", userID)

        cmd.ExecuteNonQuery()
    End Sub

End Class
