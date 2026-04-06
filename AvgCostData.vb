' ===============================================
' مكان الإضافة: ملف جديد باسم AvgCostData.vb
' ===============================================

Public Class AvgCostData
    Public Property OldAvgCostPerM3 As Decimal
    Public Property OriginalLedgerID As Integer
    Public Property LedgerID As Long
    Public Property TransactionID As Integer
    Public Property SourceDetailID As Integer
    Public Property LocalOldQty As Decimal
    Public Property LocalNewQty As Decimal
    Public Property ProductID As Integer
    Public Property BaseProductID As Integer
    Public Property StoreID As Integer

    Public Property OperationTypeID As Integer

    Public Property PrevLedgerID As Nullable(Of Long)
    Public Property SourceLedgerID As Nullable(Of Long)

    Public Property InQty As Decimal
    Public Property OutQty As Decimal
    Public Property NewQty As Decimal
    Public Property OldQty As Decimal

    Public Property InUnitCost As Decimal
    Public Property OutUnitCost As Decimal
    Public Property NewAvgCost As Decimal
    Public Property OldAvgCost As Decimal

    Public Property PostingDate As DateTime
    Public Property LedgerSequence As Integer
End Class