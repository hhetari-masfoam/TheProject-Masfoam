' ===============================================
' ملف: TransactionDetailDTO.vb
' ===============================================

Public Class TransactionDetailDTO
    ' الخصائص الأساسية
    Public Property DetailID As Integer
    Public Property TransactionID As Integer
    Public Property ProductID As Integer
    Public Property CostAmount As Decimal
    Public Property Quantity As Decimal
    Public Property UnitID As Integer
    Public Property UnitCost As Decimal
    Public Property SourceStoreID As Integer
    Public Property TargetStoreID As Integer

    ' الخصائص الإضافية المطلوبة
    Public Property StoreID As Integer          ' المخزن الفعلي المستخدم
    Public Property BaseProductID As Integer    ' للمنتج الأساسي (المتر المكعب)
    Public Property StorageUnitID As Integer    ' وحدة قياس المنتج
    Public Property SourceDocumentID As Integer ' رقم المستند المصدر
    Public Property OperationTypeID As Integer  ' نوع العملية

    ' Constructor
    Public Sub New()
        DetailID = 0
        TransactionID = 0
        ProductID = 0
        Quantity = 0D
        UnitID = 0
        UnitCost = 0D
        SourceStoreID = 0
        TargetStoreID = 0
        StoreID = 0
        BaseProductID = 0
        StorageUnitID = 0
        SourceDocumentID = 0
        OperationTypeID = 0
    End Sub

    ' للتسهيل في التصحيح
    Public Overrides Function ToString() As String
        Return $"ProductID: {ProductID}, Quantity: {Quantity}, StoreID: {StoreID}"
    End Function
End Class