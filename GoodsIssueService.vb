Imports System.Data.SqlClient

Public Class GoodsIssueService

    Private ReadOnly _connStr As String

    Public Sub New(connStr As String)
        _connStr = connStr
    End Sub

    Public Sub Issue(loID As Integer, losrID As Integer, userID As Integer)

        Using con As New SqlConnection(_connStr)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    ' ============================================
                    ' 1️⃣ جلب TransactionID
                    ' ============================================
                    Dim transactionID As Integer

                    Using cmd As New SqlCommand("
SELECT TOP 1 TransactionID
FROM Inventory_TransactionHeader
WHERE SourceDocumentID = @LOID
  AND OperationTypeID = 4
ORDER BY TransactionID DESC
", con, tran)

                        cmd.Parameters.AddWithValue("@LOID", loID)
                        transactionID = CInt(cmd.ExecuteScalar())

                    End Using

                    If transactionID <= 0 Then
                        Throw New Exception("لم يتم العثور على ترانزكشن التحميل")
                    End If

                    ' ============================================
                    ' 2️⃣ توليد كود الفسح
                    ' ============================================
                    Dim clearanceCode As String

                    Using cmd As New SqlCommand("cfg.GenerateInvoiceNumber", con, tran)

                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@InvoiceType", "GIS")

                        Dim outParam As New SqlParameter("@NewNumber", SqlDbType.NVarChar, 50)
                        outParam.Direction = ParameterDirection.Output
                        cmd.Parameters.Add(outParam)

                        cmd.ExecuteNonQuery()

                        clearanceCode = outParam.Value.ToString()

                    End Using

                    ' ============================================
                    ' 3️⃣ تنفيذ الترحيل (داخل نفس الترانزكشن)
                    ' ============================================
                    Dim engine As New TransactionEngine(_connStr)
                    engine.Receive(transactionID, userID, con, tran)
                    ' ============================================
                    ' 🔥 جلب LOSRID الصحيح من DB
                    ' ============================================
                    Dim realLOSRID As Integer

                    Using cmd As New SqlCommand("
SELECT TOP 1 LOSRID
FROM Logistics_LoadingOrderSR
WHERE LOID = @LOID AND SRID = @SRID
", con, tran)

                        cmd.Parameters.AddWithValue("@LOID", loID)
                        cmd.Parameters.AddWithValue("@SRID", losrID) ' هنا losrID هو SRID القادم من الفورم

                        Dim obj = cmd.ExecuteScalar()

                        If obj Is Nothing OrElse IsDBNull(obj) Then
                            Throw New Exception("لم يتم العثور على ربط LO مع SR (LOSRID)")
                        End If

                        realLOSRID = CInt(obj)

                    End Using
                    ' ============================================
                    ' 4️⃣ إدخال الفسح
                    ' ============================================

                    Using cmd As New SqlCommand("
INSERT INTO Logistics_GoodsClearance
(
    LOSRID,
    ClearanceCode,
    ClearanceDate,
    StatusID,
    Notes,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @LOSRID,
    @Code,
    SYSDATETIME(),
    6,
    NULL,
    SYSDATETIME(),
    @UserID
)
", con, tran)

                        cmd.Parameters.AddWithValue("@LOSRID", realLOSRID)
                        cmd.Parameters.AddWithValue("@Code", clearanceCode)
                        cmd.Parameters.AddWithValue("@UserID", userID)

                        cmd.ExecuteNonQuery()

                    End Using

                    ' ============================================
                    ' 5️⃣ تحديث حالة LO
                    ' ============================================
                    Using cmd As New SqlCommand("
UPDATE Logistics_LoadingOrder
SET LoadingStatusID = 15
WHERE LOID = @LOID
", con, tran)

                        cmd.Parameters.AddWithValue("@LOID", loID)
                        cmd.ExecuteNonQuery()

                    End Using

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using

        End Using

    End Sub

End Class