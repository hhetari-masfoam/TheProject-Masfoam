Imports System.Data.SqlClient

Public Class finCashTransactionService

    Public Function SaveTransaction(model As finCashTransactionModel) As Integer

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    ' =========================
                    ' 🔵 1. إدخال الهيدر
                    ' =========================

                    Dim cmd As New SqlCommand("
INSERT INTO fin.CashTransactionHeader
(
TransactionNo, TransactionDate, OperationTypeID, Direction,
CashAccountID, PartnerID, PartnerTypeID,
CurrencyID, ExchangeRate, TotalAmount,
PaymentMethodID, Description, ReferenceNo,
StatusID, IsPosted, CreatedAt, CreatedBy
)
VALUES
(
@TransactionNo, @TransactionDate, @OperationTypeID, @Direction,
@CashAccountID, @PartnerID, @PartnerTypeID,
@CurrencyID, @ExchangeRate, @TotalAmount,
@PaymentMethodID, @Description, @ReferenceNo,
@StatusID, 0, SYSDATETIME(), @CreatedBy
);
SELECT SCOPE_IDENTITY();
", con, tran)

                    cmd.Parameters.AddWithValue("@TransactionNo", model.TransactionNo)
                    cmd.Parameters.AddWithValue("@TransactionDate", model.TransactionDate)
                    cmd.Parameters.AddWithValue("@OperationTypeID", model.OperationTypeID)
                    cmd.Parameters.AddWithValue("@Direction", model.Direction)

                    cmd.Parameters.AddWithValue("@CashAccountID", model.CashAccountID)

                    cmd.Parameters.AddWithValue("@PartnerID", If(model.PartnerID, DBNull.Value))
                    cmd.Parameters.AddWithValue("@PartnerTypeID", If(model.PartnerTypeID, DBNull.Value))

                    cmd.Parameters.AddWithValue("@CurrencyID", model.CurrencyID)
                    cmd.Parameters.AddWithValue("@ExchangeRate", model.ExchangeRate)
                    cmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount)

                    cmd.Parameters.AddWithValue("@PaymentMethodID", model.PaymentMethodID)

                    cmd.Parameters.AddWithValue("@Description", If(model.Description, ""))
                    cmd.Parameters.AddWithValue("@ReferenceNo", If(model.ReferenceNo, ""))

                    cmd.Parameters.AddWithValue("@StatusID", model.StatusID)
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy)

                    Dim transactionID = Convert.ToInt32(cmd.ExecuteScalar())

                    ' =========================
                    ' 🟢 2. إدخال التفاصيل
                    ' =========================

                    Dim lineNo As Integer = 1

                    For Each d In model.Details

                        Dim cmdDet As New SqlCommand("
INSERT INTO fin.CashTransactionDetails
(
CashTransactionID, LineNumber,
AccountID, PartnerID,
DebitAmount, CreditAmount, Amount,
IsTaxable, IsTaxIncluded, TaxRate, TaxAmount, NetAmount,
Description
)
VALUES
(
@CashTransactionID, @LineNumber,
@AccountID, @PartnerID,
@DebitAmount, @CreditAmount, @Amount,
@IsTaxable, @IsTaxIncluded, @TaxRate, @TaxAmount, @NetAmount,
@Description
)
", con, tran)

                        cmdDet.Parameters.AddWithValue("@CashTransactionID", transactionID)
                        cmdDet.Parameters.AddWithValue("@LineNumber", lineNo)

                        cmdDet.Parameters.AddWithValue("@AccountID", d.AccountID)
                        cmdDet.Parameters.AddWithValue("@PartnerID", If(d.PartnerID, DBNull.Value))

                        cmdDet.Parameters.AddWithValue("@DebitAmount", d.DebitAmount)
                        cmdDet.Parameters.AddWithValue("@CreditAmount", d.CreditAmount)
                        cmdDet.Parameters.AddWithValue("@Amount", d.Amount)

                        cmdDet.Parameters.AddWithValue("@IsTaxable", d.IsTaxable)
                        cmdDet.Parameters.AddWithValue("@IsTaxIncluded", d.IsTaxIncluded)
                        cmdDet.Parameters.AddWithValue("@TaxRate", d.TaxRate)
                        cmdDet.Parameters.AddWithValue("@TaxAmount", d.TaxAmount)
                        cmdDet.Parameters.AddWithValue("@NetAmount", d.NetAmount)

                        cmdDet.Parameters.AddWithValue("@Description", If(d.Description, ""))

                        cmdDet.ExecuteNonQuery()

                        lineNo += 1

                    Next

                    tran.Commit()
                    Return transactionID

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function

    Public Function UpdateTransaction(model As finCashTransactionModel) As Integer

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try

                    ' =========================
                    ' 🔵 1. تحديث الهيدر
                    ' =========================

                    Dim cmd As New SqlCommand("
UPDATE fin.CashTransactionHeader
SET
    TransactionDate = @TransactionDate,
    OperationTypeID = @OperationTypeID,
    Direction = @Direction,
    CashAccountID = @CashAccountID,
    PartnerID = @PartnerID,
    PartnerTypeID = @PartnerTypeID,
    CurrencyID = @CurrencyID,
    ExchangeRate = @ExchangeRate,
    TotalAmount = @TotalAmount,
    PaymentMethodID = @PaymentMethodID,
    Description = @Description,
    ReferenceNo = @ReferenceNo,
    StatusID = @StatusID,
    LastUpdatedAt = SYSDATETIME(),
    LastUpdatedBy = @UpdatedBy
WHERE CashTransactionID = @ID
", con, tran)

                    cmd.Parameters.AddWithValue("@ID", model.TransactionID)
                    cmd.Parameters.AddWithValue("@TransactionDate", model.TransactionDate)
                    cmd.Parameters.AddWithValue("@OperationTypeID", model.OperationTypeID)
                    cmd.Parameters.AddWithValue("@Direction", model.Direction)

                    cmd.Parameters.AddWithValue("@CashAccountID", model.CashAccountID)

                    cmd.Parameters.AddWithValue("@PartnerID", If(model.PartnerID, DBNull.Value))
                    cmd.Parameters.AddWithValue("@PartnerTypeID", If(model.PartnerTypeID, DBNull.Value))

                    cmd.Parameters.AddWithValue("@CurrencyID", model.CurrencyID)
                    cmd.Parameters.AddWithValue("@ExchangeRate", model.ExchangeRate)
                    cmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount)

                    cmd.Parameters.AddWithValue("@PaymentMethodID", model.PaymentMethodID)

                    cmd.Parameters.AddWithValue("@Description", If(model.Description, ""))
                    cmd.Parameters.AddWithValue("@ReferenceNo", If(model.ReferenceNo, ""))

                    cmd.Parameters.AddWithValue("@StatusID", model.StatusID)
                    cmd.Parameters.AddWithValue("@UpdatedBy", model.CreatedBy) ' أو CurrentUser

                    cmd.ExecuteNonQuery()

                    ' =========================
                    ' 🔴 2. حذف التفاصيل القديمة
                    ' =========================

                    Dim cmdDel As New SqlCommand("
DELETE FROM fin.CashTransactionDetails
WHERE CashTransactionID = @ID
", con, tran)

                    cmdDel.Parameters.AddWithValue("@ID", model.TransactionID)
                    cmdDel.ExecuteNonQuery()

                    ' =========================
                    ' 🟢 3. إعادة إدخال التفاصيل
                    ' =========================

                    Dim lineNo As Integer = 1

                    For Each d In model.Details

                        Dim cmdDet As New SqlCommand("
INSERT INTO fin.CashTransactionDetails
(
CashTransactionID, LineNumber,
AccountID, PartnerID,
DebitAmount, CreditAmount, Amount,
IsTaxable, IsTaxIncluded, TaxRate, TaxAmount, NetAmount,
Description
)
VALUES
(
@CashTransactionID, @LineNumber,
@AccountID, @PartnerID,
@DebitAmount, @CreditAmount, @Amount,
@IsTaxable, @IsTaxIncluded, @TaxRate, @TaxAmount, @NetAmount,
@Description
)
", con, tran)

                        cmdDet.Parameters.AddWithValue("@CashTransactionID", model.TransactionID)
                        cmdDet.Parameters.AddWithValue("@LineNumber", lineNo)

                        cmdDet.Parameters.AddWithValue("@AccountID", d.AccountID)
                        cmdDet.Parameters.AddWithValue("@PartnerID", If(d.PartnerID, DBNull.Value))

                        cmdDet.Parameters.AddWithValue("@DebitAmount", d.DebitAmount)
                        cmdDet.Parameters.AddWithValue("@CreditAmount", d.CreditAmount)
                        cmdDet.Parameters.AddWithValue("@Amount", d.Amount)

                        cmdDet.Parameters.AddWithValue("@IsTaxable", d.IsTaxable)
                        cmdDet.Parameters.AddWithValue("@IsTaxIncluded", d.IsTaxIncluded)
                        cmdDet.Parameters.AddWithValue("@TaxRate", d.TaxRate)
                        cmdDet.Parameters.AddWithValue("@TaxAmount", d.TaxAmount)
                        cmdDet.Parameters.AddWithValue("@NetAmount", d.NetAmount)

                        cmdDet.Parameters.AddWithValue("@Description", If(d.Description, ""))

                        cmdDet.ExecuteNonQuery()

                        lineNo += 1

                    Next

                    tran.Commit()

                    Return model.TransactionID

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function
    Public Sub UpdateStatus(transactionID As Integer, newStatus As Integer)

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using cmd As New SqlCommand("
UPDATE fin.CashTransactionHeader
SET StatusID = @S
WHERE CashTransactionID = @ID
", con)

                cmd.Parameters.AddWithValue("@S", newStatus)
                cmd.Parameters.AddWithValue("@ID", transactionID)

                cmd.ExecuteNonQuery()

            End Using
        End Using

    End Sub

    Public Function GetTransactionByID(id As Integer) As DataTable

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable()

            Using da As New SqlDataAdapter("
SELECT *
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con)

                da.SelectCommand.Parameters.AddWithValue("@ID", id)
                da.Fill(dt)

            End Using

            Return dt

        End Using

    End Function

    Public Function GetTransactionHeader(id As Integer) As DataRow

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable()

            Using da As New SqlDataAdapter("
SELECT *
FROM fin.CashTransactionHeader
WHERE CashTransactionID = @ID
", con)

                da.SelectCommand.Parameters.AddWithValue("@ID", id)
                da.Fill(dt)

            End Using

            If dt.Rows.Count = 0 Then Return Nothing

            Return dt.Rows(0)

        End Using

    End Function

    Public Function GetTransactionDetails(id As Integer) As DataTable

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Dim dt As New DataTable()

            Using da As New SqlDataAdapter("
SELECT 
    d.*,
    a.AccountNameAr
FROM fin.CashTransactionDetails d
LEFT JOIN gl.Accounts a ON a.AccountID = d.AccountID
WHERE d.CashTransactionID = @ID
ORDER BY d.LineNumber
", con)

                da.SelectCommand.Parameters.AddWithValue("@ID", id)
                da.Fill(dt)

            End Using

            Return dt

        End Using

    End Function

End Class