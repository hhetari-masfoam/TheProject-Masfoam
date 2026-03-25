Public Class ProductAvgDTO

    Public Property ProductID As Integer

    Public Property OldQty As Decimal
    Public Property OldAvgCost As Decimal

    Public Property InQty As Decimal
    Public Property InAvg As Decimal

    Public Property NewAvgCost As Decimal

End Class
Public Class ProductAverageDTO

    Public Property ProductID As Integer
    Public Property BaseProductID As Integer
    Public Property TotalChemicalCost As Decimal

    Public Property ProductCode As String
    ' Old values
    Public Property OldQty As Decimal
    Public Property OldAvgCost As Decimal

    ' Incoming values
    Public Property InQty As Decimal
    Public Property InAvgCost As Decimal

    ' Result
    Public Property NewAvgCost As Decimal
End Class


Public Class ProductionRevaluationDto
        Public Property ProductionBaseValue As Decimal
        Public Property TransactionID As Integer
        Public Property ProductID As Integer
        Public Property PostingDate As DateTime

        Public Property Inputs As DataTable
        Public Property Outputs As DataTable
        Public Property TotalChemicalCost As Decimal
        Public Property ProductionCode As String
        Public Property TotalProductionVolume As Decimal
        Public Property TotalProductionQty As Decimal
        Public Property TotalChemicalQty As Decimal
        Public Property ChemicalQty As Decimal
        Public Property TotalCost As Decimal
        Public Property UnitCost As Decimal
        Public Property PastAvgCost As Decimal
        Public Property ProductCode As String
    Public Property UnitID As String
    Public Property UnitName As String
    Public Property SubCategoryID As Integer
End Class

    Public Class RevalDetailDTO
        Public Property DetailID As Integer
        Public Property ProductID As Integer

        Public Property OldQty As Decimal
        Public Property NewQty As Decimal

        Public Property OldUnitPrice As Decimal
        Public Property NewUnitPrice As Decimal
    End Class

Public Class RevalAffectedInputDto

    Public Property DetailID As Integer
    Public Property ProductID As Integer

    Public Property ProductCode As String
    Public Property ProductName As String
    Public Property ProductTypeID As String
    Public Property UnitID As String

    Public Property SourceStoreID As Integer?
    Public Property TargetStoreID As Integer?

    Public Property OldQty As Decimal
    Public Property NewQty As Decimal

    Public Property OldUnitPrice As Decimal
    Public Property NewUnitPrice As Decimal

    Public Property GrossAmount As Decimal
    Public Property DiscountRate As Decimal
    Public Property DiscountAmount As Decimal

    Public Property IncludeTax As Boolean
    Public Property TaxRate As Decimal
    Public Property TaxTypeID As Integer?

    Public Property TaxableAmount As Decimal
    Public Property TaxAmount As Decimal

    Public Property NetAmount As Decimal
    Public Property LineTotal As Decimal

End Class
Public Class CuttingRevaluationDto
    Public Property Outputs As DataTable
    Public Property CuttingCode As String
    Public Property BaseProductCode As String
    Public Property AvailableQty As Decimal
End Class