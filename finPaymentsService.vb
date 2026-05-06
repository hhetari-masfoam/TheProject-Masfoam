Imports System.Data.SqlClient

Public Class finPaymentsService

    ' =========================================
    ' 🔵 حفظ السند (Header + Details)
    ' =========================================
    Public Function SavePaymentVoucher(model As finPaymentModel, userID As Integer) As Integer

        Using con As New SqlConnection(AppConfig.MainConnectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    Dim transactionID As Integer

                    ' =========================================
                    ' 🔴 HEADER
                    ' =========================================
                    Using cmd As New SqlCommand("
INSERT INTO fin.CashTransactionHeader
(
    TransactionNo,
    TransactionDate,
    OperationTypeID,
    Direction,
    CashAccountID,
    PartnerID,
    PartnerTypeID,
    CurrencyID,
    ExchangeRate,
    TotalAmount,
    PaymentMethodID,
    Description,
    ReferenceNo,
    StatusID,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @No,
    @Date,
    @Op,
    @Dir,
    @CashAcc,
    @Partner,
    @PartnerType,
    @Currency,
    @Rate,
    @Amount,
    @PayMethod,
    @Desc,
    @Ref,
    1,
    SYSDATETIME(),
    @User
);

SELECT SCOPE_IDENTITY();
", con, tran)

                        cmd.Parameters.AddWithValue("@No", model.No)
                        cmd.Parameters.AddWithValue("@Date", model.TransactionDate)
                        cmd.Parameters.AddWithValue("@Op", model.OperationTypeID)
                        cmd.Parameters.AddWithValue("@Dir", model.Direction)
                        cmd.Parameters.AddWithValue("@CashAcc", model.CashAccountID)
                        cmd.Parameters.AddWithValue("@Partner", ToDb(model.PartnerID))
                        '                        cmd.Parameters.AddWithValue("@PartnerType", ToDb(model.PartnerTypeID))
                        '                        cmd.Parameters.AddWithValue("@Currency", model.CurrencyID)
                        '                       cmd.Parameters.AddWithValue("@Rate", model.ExchangeRate)
                        cmd.Parameters.AddWithValue("@Amount", model.Amount)
                        cmd.Parameters.AddWithValue("@PayMethod", model.PaymentMethodID)
                        cmd.Parameters.AddWithValue("@Desc", ToDb(model.Description))
                        '                        cmd.Parameters.AddWithValue("@Ref", ToDb(model.ReferenceNo))
                        cmd.Parameters.AddWithValue("@User", userID)

                        transactionID = CInt(cmd.ExecuteScalar())
                    End Using

                    ' =========================================
                    ' 🔴 DETAILS
                    ' =========================================
                    InsertDetailsFromGrid(
                        transactionID,
                        model.CashAccountID,
                        model.Direction,
                        model.Grid,
                        con,
                        tran
                    )

                    tran.Commit()
                    Return transactionID

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Function


    ' =========================================
    ' 🔵 إدخال التفاصيل
    ' =========================================
    Private Sub InsertDetailsFromGrid(
        transactionID As Integer,
        cashAccountID As Integer,
        direction As Integer,
        dgv As DataGridView,
        con As SqlConnection,
        tran As SqlTransaction
    )

        Dim total As Decimal = 0
        Dim lineNo As Integer = 1

        For Each row As DataGridViewRow In dgv.Rows

            If row.IsNewRow Then Continue For

            Dim acc = ToInt(row.Cells("AccountID").Value)
            Dim amount = ToDec(row.Cells("Amount").Value)
            Dim net = ToDec(row.Cells("NetAmount").Value)
            Dim tax = ToDec(row.Cells("TaxAmount").Value)
            Dim rate = ToDec(row.Cells("TaxRate").Value)
            Dim isTaxable = ToBool(row.Cells("IsTaxable").Value)
            Dim isIncluded = ToBool(row.Cells("IsTaxIncluded").Value)
            Dim desc = ToStr(row.Cells("Description").Value)

            If acc <= 0 OrElse amount <= 0 Then Continue For

            ' 🔴 السطر الأساسي
            Using cmd As New SqlCommand("
INSERT INTO fin.CashTransactionDetails
(
    CashTransactionID,
    LineNumber,
    AccountID,
    PartnerID,
    DebitAmount,
    CreditAmount,
    Amount,
    IsTaxable,
    IsTaxIncluded,
    TaxRate,
    TaxAmount,
    NetAmount,
    Description
)
VALUES
(
    @TID,
    @Line,
    @Acc,
    NULL,
    @Dr,
    0,
    @Amount,
    @IsTaxable,
    @IsTaxIncluded,
    @Rate,
    @Tax,
    @Net,
    @Desc
)
", con, tran)

                cmd.Parameters.AddWithValue("@TID", transactionID)
                cmd.Parameters.AddWithValue("@Line", lineNo)
                cmd.Parameters.AddWithValue("@Acc", acc)
                cmd.Parameters.AddWithValue("@Dr", net)
                cmd.Parameters.AddWithValue("@Amount", amount)
                cmd.Parameters.AddWithValue("@IsTaxable", isTaxable)
                cmd.Parameters.AddWithValue("@IsTaxIncluded", isIncluded)
                cmd.Parameters.AddWithValue("@Rate", rate)
                cmd.Parameters.AddWithValue("@Tax", tax)
                cmd.Parameters.AddWithValue("@Net", net)
                cmd.Parameters.AddWithValue("@Desc", desc)

                cmd.ExecuteNonQuery()
            End Using

            lineNo += 1
            total += net + tax

            ' 🔵 سطر الضريبة
            If tax > 0 Then

                Dim taxAcc = GetTaxAccountID(direction, con, tran)

                Using cmd As New SqlCommand("
INSERT INTO fin.CashTransactionDetails
(
    CashTransactionID,
    LineNumber,
    AccountID,
    DebitAmount,
    CreditAmount,
    Amount,
    TaxAmount,
    NetAmount,
    Description
)
VALUES
(
    @TID,
    @Line,
    @Acc,
    @Dr,
    0,
    @Tax,
    @Tax,
    0,
    'ضريبة'
)
", con, tran)

                    cmd.Parameters.AddWithValue("@TID", transactionID)
                    cmd.Parameters.AddWithValue("@Line", lineNo)
                    cmd.Parameters.AddWithValue("@Acc", taxAcc)
                    cmd.Parameters.AddWithValue("@Dr", tax)

                    cmd.ExecuteNonQuery()
                End Using

                lineNo += 1
            End If

        Next

        ' 🔵 سطر الكاش
        Using cmd As New SqlCommand("
INSERT INTO fin.CashTransactionDetails
(
    CashTransactionID,
    LineNumber,
    AccountID,
    DebitAmount,
    CreditAmount,
    Amount,
    NetAmount
)
VALUES
(
    @TID,
    @Line,
    @Acc,
    0,
    @Cr,
    @Cr,
    @Cr
)
", con, tran)

            cmd.Parameters.AddWithValue("@TID", transactionID)
            cmd.Parameters.AddWithValue("@Line", lineNo)
            cmd.Parameters.AddWithValue("@Acc", cashAccountID)
            cmd.Parameters.AddWithValue("@Cr", total)

            cmd.ExecuteNonQuery()
        End Using

    End Sub


    ' =========================================
    ' 🔵 أدوات مساعدة
    ' =========================================
    Private Function ToDb(v As Object) As Object
        If v Is Nothing Then Return DBNull.Value
        Return v
    End Function

    Private Function ToDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal = 0D
        Decimal.TryParse(v.ToString(), d)
        Return d
    End Function

    Private Function ToInt(v As Object) As Integer
        If v Is Nothing OrElse IsDBNull(v) Then Return 0
        Return Convert.ToInt32(v)
    End Function

    Private Function ToBool(v As Object) As Boolean
        If v Is Nothing OrElse IsDBNull(v) Then Return False
        Return Convert.ToBoolean(v)
    End Function

    Private Function ToStr(v As Object) As String
        If v Is Nothing OrElse IsDBNull(v) Then Return ""
        Return v.ToString()
    End Function


    ' =========================================
    ' 🔵 حساب الضريبة
    ' =========================================
    Private Function GetTaxAccountID(
        direction As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As Integer

        Using cmd As New SqlCommand("
SELECT TOP 1 TaxAccountID
FROM md.TaxType
WHERE IsActive = 1
  AND TaxDirection = @Dir
  AND TaxAccountID IS NOT NULL
", con, tran)

            cmd.Parameters.AddWithValue("@Dir", direction)

            Dim obj = cmd.ExecuteScalar()

            If obj Is Nothing OrElse IsDBNull(obj) Then
                Throw New Exception("حساب الضريبة غير معرف")
            End If

            Return CInt(obj)

        End Using

    End Function

End Class