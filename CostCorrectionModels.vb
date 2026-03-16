Public Class RevaluationDocumentHeaderDto


    Public Property DocumentID As Integer
    Public Property DocumentNo As String
    Public Property DocumentDate As DateTime

    Public Property PartnerID As Integer?

    Public Property TotalAmount As Decimal
    Public Property TotalTax As Decimal
    Public Property TotalNetAmount As Decimal

    Public Property PaymentMethodID As Integer?
    Public Property PaymentTermID As Integer?

    Public Property Notes As String
    Public Property StatusID As Integer?

    Public Property IsInventoryPosted As Boolean
    Public Property IsTaxInclusive As Boolean


End Class
