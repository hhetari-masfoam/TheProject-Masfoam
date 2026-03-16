Public Class InvoiceHeaderInput

    Public Property DocumentID As Integer
    Public Property DocumentType As String
    Public Property DocumentNo As String
    Public Property DocumentDate As DateTime
    Public Property DueDate As DateTime?

    Public Property PartnerID As Integer?
    Public Property CurrencyID As Integer
    Public Property ExchangeRate As Decimal

    Public Property TaxReasonID As Integer?
    Public Property TaxTypeID As Integer

    Public Property LineTotal As Decimal
    Public Property TotalDiscount As Decimal
    Public Property TotalTax As Decimal
    Public Property TotalNetAmount As Decimal

    Public Property PaidAmount As Decimal
    Public Property RemainingAmount As Decimal

    Public Property DeliveryDate As Date?
    Public Property DriverName As String
    Public Property VehicleNo As String

    Public Property Notes As String
    Public Property CreatedAt As DateTime

    Public Property PaymentMethodID As Integer
    Public Property PaymentTermID As Integer?

    Public Property StatusID As Integer

    Public Property IsInventoryPosted As Boolean
    Public Property IsZatcaReported As Boolean
    Public Property IsTaxInclusive As Boolean
    Public Property IsOutbound As Boolean
    ' =========================
    ' Document Linking
    ' =========================
    Public Property SourceDocumentID As Integer?
    Public Property SourceDocumentType As String
    Public Property TotalAmount As Decimal
    Public Property GrandTotal As Decimal
    Public Property TotalTaxableAmount As Decimal

End Class
