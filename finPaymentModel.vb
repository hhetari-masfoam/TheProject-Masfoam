Public Class finPaymentModel

    Public Property ID As Integer              ' 🔥 مهم للتعديل
    Public Property No As String
    Public Property TransactionDate As Date

    Public PartnerID As Integer
    Public Property SupplierAccountID As Integer   ' 🔥 حساب المورد
    Public Property CustomerAccountID As Integer   ' 🔥 حساب المورد

    Public Property CashAccountID As Integer       ' 🔥 الصندوق / البنك

    Public Property OperationTypeID As Integer     ' 🔥 نوع العملية
    Public Property Direction As Byte              ' 🔥 1=IN / 2=OUT

    Public Property Amount As Decimal
    Public Property Description As String
    Public PaymentMethodID As Integer
    Public Property IsTaxable As Boolean
    Public Property IsTaxIncluded As Boolean
    Public Property TaxTypeID As Integer
    Public Property TaxRate As Decimal
    Public Property TaxAmount As Decimal
    Public Property NetAmount As Decimal
    Public Property PartnerAccountID As Integer?
    Public Property BeneficiaryAccountID As Integer?
    Public Property Grid As DataGridView


End Class