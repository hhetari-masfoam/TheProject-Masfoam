Public Class finCashTransactionDetailModel
    Public Property LineNumber As Integer

    Public Property DebitAmount As Decimal
    Public Property CreditAmount As Decimal

    Public Property AccountID As Integer
    Public Property PartnerTypeID As Integer?
    Public Property PartnerID As Integer?

    Public Property Amount As Decimal

    Public Property IsTaxable As Boolean
    Public Property IsTaxIncluded As Boolean

    Public Property TaxRate As Decimal
    Public Property TaxAmount As Decimal
    Public Property NetAmount As Decimal
    Public Property TotalAmount As Decimal

    Public Property Description As String
    Public Property TaxTypeID As Integer

End Class