Imports System.Data.SqlClient

Public Class ValidationEngine
    Public Sub ValidateTransfer(details As List(Of TransactionDetailDTO))

        If details Is Nothing OrElse details.Count = 0 Then
            Throw New Exception("لا توجد تفاصيل للتحويل")
        End If

        For Each d In details
            If d.Quantity <= 0 Then
                Throw New Exception("الكمية يجب أن تكون أكبر من صفر")
            End If

            If d.SourceStoreID = d.TargetStoreID Then
                Throw New Exception("لا يمكن التحويل لنفس المخزن")
            End If
        Next

    End Sub
    Public Sub ValidateReceive(
       transactionID As Integer,
       con As SqlConnection,
       tran As SqlTransaction
   )

        If transactionID <= 0 Then
            Throw New Exception("رقم العملية غير صحيح")
        End If

        ' يمكنك إضافة تحقق الحالة هنا إذا أردت

    End Sub

    ' ===============================================
    ' مكان الإضافة: داخل Class ValidationEngine
    ' ===============================================

    ''' <summary>
    ''' التحقق من إمكانية إلغاء استلام سند
    ''' </summary>
    Public Sub ValidateReversePossibility(transactionID As Integer, con As SqlConnection, tran As SqlTransaction)
        ' 1. التحقق من وجود السند
        Dim sql = "
        SELECT 
            h.IsInventoryPosted,
            h.StatusID,
            h.OperationTypeID,
            h.SourceDocumentID
        FROM Inventory_TransactionHeader h
        WHERE h.TransactionID = @TransactionID"

        Dim cmd As New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

        Dim reader = cmd.ExecuteReader()
        If Not reader.Read() Then
            reader.Close()
            Throw New Exception("السند غير موجود")
        End If

        Dim isPosted = reader.GetBoolean(0)
        Dim statusID = reader.GetInt32(1)
        Dim operationTypeID = reader.GetInt32(2)
        Dim SourceDocumentID = reader.GetInt32(3)
        reader.Close()

        ' 2. التحقق من أن السند مرحل
        If Not isPosted Then
            Throw New Exception("لا يمكن إلغاء سند غير مرحل")
        End If

        ' 3. التحقق من أن السند من نوع استلام (PUR, PRO, CUT, SRT)
        If operationTypeID <> 7 AndAlso operationTypeID <> 6 AndAlso
           operationTypeID <> 11 AndAlso operationTypeID <> 12 Then
            Throw New Exception("هذا السند ليس من نوع استلام")
        End If


        ' 5. التحقق من توفر الكمية للعكس
        sql = "
SELECT 
    d.ProductID,
    ISNULL(d.TargetStoreID, d.SourceStoreID) as StoreID,
    d.Quantity,
    b.QtyOnHand
FROM Inventory_TransactionDetails d
JOIN Master_Product p
    ON p.ProductID = d.ProductID
INNER JOIN Inventory_Balance b 
    ON b.ProductID = d.ProductID 
    AND b.BaseProductID = ISNULL(p.BaseProductID, p.ProductID)
    AND b.StoreID = ISNULL(d.TargetStoreID, d.SourceStoreID)
WHERE d.TransactionID = @TransactionID
  AND b.QtyOnHand < d.Quantity"

        cmd = New SqlCommand(sql, con, tran)
        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

        Dim insufficientReader = cmd.ExecuteReader()
        If insufficientReader.HasRows Then
            insufficientReader.Close()
            Throw New Exception("الكمية المتوفرة في المخزن لا تسمح بعكس الاستلام")
        End If
        insufficientReader.Close()

    End Sub
End Class
