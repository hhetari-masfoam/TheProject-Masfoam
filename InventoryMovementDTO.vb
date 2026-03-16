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

    ' Old values
    Public Property OldQty As Decimal
    Public Property OldAvgCost As Decimal

    ' Incoming values
    Public Property InQty As Decimal
    Public Property InAvgCost As Decimal

    ' Result
    Public Property NewAvgCost As Decimal

End Class