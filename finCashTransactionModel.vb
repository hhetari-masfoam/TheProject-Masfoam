Public Class finCashTransactionModel


    Public Property StatusID As Integer

    Public Property CreatedBy As Integer

    Public Property ID As Integer

    Public Property TransactionNo As String
    Public Property TransactionDate As Date

    Public Property OperationTypeID As Integer
    Public Property TransactionID As Integer
    Public Property Direction As Integer

    Public Property CashAccountID As Integer

    Public Property PartnerID As Integer?   ' optional snapshot
    Public Property PartnerTypeID As Integer?

    Public Property CurrencyID As Integer
    Public Property ExchangeRate As Decimal

    Public Property TotalAmount As Decimal

    Public Property PaymentMethodID As Integer

    Public Property Description As String
    Public Property ReferenceNo As String

    Public Property Details As List(Of finCashTransactionDetailModel)

End Class