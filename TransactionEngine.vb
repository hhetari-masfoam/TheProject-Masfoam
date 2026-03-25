Imports System.Data.SqlClient
Imports System.Text

Public Class TransactionEngine

    Private ReadOnly _connectionString As String
    Private ReadOnly _transactionRepo As TransactionRepository
    Private ReadOnly _workflowEngine As WorkflowEngine

    Private ReadOnly _inventoryRepo As InventoryRepository
    Private ReadOnly _validationEngine As ValidationEngine

    Public Sub New(connectionString As String)

        _connectionString = connectionString
        _transactionRepo = New TransactionRepository()
        _inventoryRepo = New InventoryRepository(_connectionString)
        _workflowEngine = New WorkflowEngine()
        _validationEngine = New ValidationEngine()

    End Sub

    Public Sub Receive(transactionID As Integer, userID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()
                Dim opTypeID As Integer = -1
                Dim operationGroupID As Guid = Guid.NewGuid()
                Try

                    ' =====================================================
                    ' 1️⃣ Validation + بيانات عامة
                    ' =====================================================

                    _validationEngine.ValidateReceive(transactionID, con, tran)

                    Dim m3UnitID As Integer
                    Using cmd As New SqlCommand("SELECT UnitID FROM dbo.Master_Unit WHERE UnitCode = 'M3'", con, tran)
                        m3UnitID = CInt(cmd.ExecuteScalar())
                    End Using

                    Using cmd As New SqlCommand("SELECT OperationTypeID FROM dbo.Inventory_TransactionHeader WHERE TransactionID = @ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", transactionID)
                        opTypeID = CInt(cmd.ExecuteScalar())
                    End Using
                    Using cmd As New SqlCommand("UPDATE Inventory_TransactionHeader SET PostingDate = SYSDATETIME() WHERE TransactionID = @ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", transactionID)
                        cmd.ExecuteNonQuery()
                    End Using
                    Dim oldQtyDict As Dictionary(Of Integer, Decimal)

                    ' =====================================================
                    ' 2️⃣ Routing — كل نوع لوحده بالكامل
                    ' =====================================================

                    Select Case opTypeID

' =====================================================
' 6️⃣ PRODUCTION
' =====================================================


                        Case 6

                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)
                            Dim dt = _inventoryRepo.GetProductsByTargetStore(transactionID, con, tran)

                            Dim hasM3 As Boolean = False
                            Dim hasNonM3 As Boolean = False
                            Dim productIdsM3 As New List(Of Integer)
                            Dim productIdsNonM3 As New List(Of Integer)
                            Dim seq As Integer = 1

                            For Each row As DataRow In dt.Rows
                                Dim pid As Integer = CInt(row("ProductID"))
                                Dim su As Integer = CInt(row("StorageUnitID"))

                                If su = m3UnitID Then
                                    hasM3 = True
                                    productIdsM3.Add(pid)
                                Else
                                    hasNonM3 = True
                                    productIdsNonM3.Add(pid)
                                End If
                            Next

                            ' Ledger
                            _inventoryRepo.InsertCostLedger_OUT(transactionID, operationGroupID, seq, oldQtyDict, con, tran)

                            If hasM3 Then
                                _inventoryRepo.InsertCostLedger_M3(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)
                            End If

                            Dim ledgerID =
_inventoryRepo.InsertCostLedger_Regular_Production(
transactionID,
userID,
m3UnitID,
operationGroupID,
seq,
oldQtyDict,
con,
tran)
                            ' Links
                            _inventoryRepo.InsertProductionLedgerLinks(transactionID, operationGroupID, userID, con, tran)

                            ' Inventory
                            _inventoryRepo.ApplyInventoryOut(transactionID, con, tran)
                            _inventoryRepo.ApplyInventoryIn(transactionID, m3UnitID, ledgerID, con, tran)

                            ' Cost
                            Dim allIds As New List(Of Integer)
                            allIds.AddRange(productIdsM3)
                            allIds.AddRange(productIdsNonM3)

                            _inventoryRepo.RecalculateAverage_PUR_PRO_BySnapshot(transactionID, m3UnitID, allIds, con, tran)


' =====================================================
' 7️⃣ PURCHASE
' =====================================================
                        Case 7
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

                            Dim dt = _inventoryRepo.GetProductsByTargetStore(transactionID, con, tran)

                            Dim hasM3 As Boolean = False
                            Dim hasNonM3 As Boolean = False
                            Dim productIdsM3 As New List(Of Integer)
                            Dim productIdsNonM3 As New List(Of Integer)
                            Dim seq As Integer = 1

                            For Each row As DataRow In dt.Rows

                                Dim pid As Integer = CInt(row("ProductID"))
                                Dim storageUnitId As Integer = CInt(row("StorageUnitID"))
                                If storageUnitId = m3UnitID Then
                                    hasM3 = True
                                    productIdsM3.Add(pid)
                                Else
                                    hasNonM3 = True
                                    productIdsNonM3.Add(pid)
                                End If

                            Next

                            ' Ledger
                            If hasM3 Then
                                _inventoryRepo.InsertCostLedger_M3_Purchase(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)
                            End If

                            Dim ledgerID =
                                _inventoryRepo.InsertCostLedger_RegularPurchase(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)
                            _inventoryRepo.InsertLedgerLinks_PUR(transactionID, operationGroupID, userID, con, tran)

                            ' Inventory
                            _inventoryRepo.ApplyInventoryIn(transactionID, m3UnitID, ledgerID, con, tran)

                            ' Cost
                            If hasM3 Then
                                _inventoryRepo.RecalculateAverage_PUR_PRO_BySnapshot(transactionID, m3UnitID, productIdsM3, con, tran)
                            End If

                            If hasNonM3 Then
                                _inventoryRepo.RecalculateAverage_PUR_PRO_BySnapshot(transactionID, m3UnitID, productIdsNonM3, con, tran)
                            End If


' =====================================================
' 🔟 TRANSFER
' =====================================================
                        Case 10

                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

                            Dim seq As Integer = 1

                            ' Ledger
                            _inventoryRepo.InsertCostLedger_OUTtransfer(transactionID, operationGroupID, seq, oldQtyDict, con, tran)
                            _inventoryRepo.InsertCostLedger_IN_TRN(transactionID, operationGroupID, seq, oldQtyDict, con, tran)

                            ' Links
                            _inventoryRepo.InsertTransferLedgerLinks_TRN(transactionID, operationGroupID, userID, con, tran)

                            ' Inventory
                            _inventoryRepo.ApplyInventoryOut(transactionID, con, tran)
                            _inventoryRepo.ApplyInventoryIn(transactionID, m3UnitID, 0, con, tran)


' =====================================================
' 1️⃣1️⃣ CUT
' =====================================================
                        Case 11
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores_cut(transactionID, operationGroupID, con, tran)
                            Dim seq As Integer = 1

                            ' Ledger
                            _inventoryRepo.InsertCostLedger_OUTCut(transactionID, operationGroupID, seq, oldQtyDict, con, tran)
                            _inventoryRepo.InsertCostLedger_CUT(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)

                            ' Links
                            _inventoryRepo.InsertCuttingLedgerLinks_M3Only(transactionID, operationGroupID, userID, con, tran)

                            ' Inventory
                            _inventoryRepo.ApplyInventoryOutCut(transactionID, con, tran)
                            _inventoryRepo.ApplyInventoryInCut(transactionID, m3UnitID, 0, con, tran)

                            ' Cost
                            _inventoryRepo.UpdateFinalProductAvgCostForFG_FromCutLedger(transactionID, con, tran)


' =====================================================
' 1️⃣2️⃣ SALES RETURN
' =====================================================
                        Case 12
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStoresReturn(transactionID, operationGroupID, con, tran)

                            Dim dt = _inventoryRepo.GetProductsByTargetStoreReturn(transactionID, con, tran)
                            Dim hasM3 As Boolean = False
                            Dim hasNonM3 As Boolean = False
                            Dim productIdsM3 As New List(Of Integer)
                            Dim productIdsNonM3 As New List(Of Integer)
                            Dim seq As Integer = 1

                            For Each dr As DataRow In dt.Rows

                                Dim pid As Integer = Convert.ToInt32(dr("ProductID"))

                                Dim productUnitId As Integer = Convert.ToInt32(dr("StorageUnitID"))

                                Dim baseUnitId As Integer = 0
                                If dt.Columns.Contains("BaseUnitID") AndAlso Not IsDBNull(dr("BaseUnitID")) Then
                                    baseUnitId = Convert.ToInt32(dr("BaseUnitID"))
                                End If

                                Dim isM3 As Boolean = (productUnitId = m3UnitID) OrElse (baseUnitId = m3UnitID)

                                If isM3 Then
                                    hasM3 = True
                                    productIdsM3.Add(pid)
                                Else
                                    hasNonM3 = True
                                    productIdsNonM3.Add(pid)
                                End If

                            Next

                            ' Ledger

                            Dim testM3Count As Integer = 0
                            Using cmd As New SqlCommand("
SELECT COUNT(*)
FROM Inventory_TransactionDetails d
JOIN Master_Product p ON p.ProductID = d.ProductID
LEFT JOIN Master_Product bp ON bp.ProductID = p.BaseProductID
WHERE d.TransactionID = @T
  AND d.TargetStoreID IS NOT NULL
  AND (p.StorageUnitID = @M3 OR bp.StorageUnitID = @M3)
", con, tran)
                                cmd.Parameters.AddWithValue("@T", transactionID)
                                cmd.Parameters.AddWithValue("@M3", m3UnitID)
                                testM3Count = CInt(cmd.ExecuteScalar())
                            End Using
                            If hasM3 Then
                                _inventoryRepo.InsertCostLedger_M3Return(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)

                            End If

                            Dim ledgerID =
        _inventoryRepo.InsertCostLedger_RegularReturn(transactionID, userID, m3UnitID, operationGroupID, seq, oldQtyDict, con, tran)

                            _inventoryRepo.InsertLedgerLinks_SRT(transactionID, operationGroupID, userID, con, tran)

                            Dim cnt As Integer = 0
                            Using cmd As New SqlCommand("
SELECT COUNT(*) 
FROM Inventory_CostLedger
WHERE TransactionID=@T AND OperationGroupID=@G
", con, tran)
                                cmd.Parameters.AddWithValue("@T", transactionID)
                                cmd.Parameters.AddWithValue("@G", operationGroupID)
                                cnt = CInt(cmd.ExecuteScalar())
                            End Using



                            ' Inventory
                            _inventoryRepo.ApplyInventoryInReturn(transactionID, m3UnitID, ledgerID, con, tran)

                            ' Cost
                            If hasM3 Then
                                _inventoryRepo.RecalculateAverage_PUR_PRO_BySnapshotReturn(transactionID, m3UnitID, productIdsM3, con, tran)

                            End If

                            If hasNonM3 Then
                                _inventoryRepo.RecalculateAverage_PUR_PRO_BySnapshotReturn(transactionID, m3UnitID, productIdsNonM3, con, tran)

                            End If



' =====================================================
' 1️⃣3️⃣ LOAD
' =====================================================
                        Case 4
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

                            Dim seq As Integer = 1

                            ' Ledger
                            _inventoryRepo.InsertCostLedger_OUT(transactionID, operationGroupID, seq, oldQtyDict, con, tran)

                            _inventoryRepo.InsertLoadingLinks(transactionID, operationGroupID, userID, con, tran)

                            ' Inventory
                            _inventoryRepo.ApplyInventoryOut(transactionID, con, tran)


' =====================================================
' PRT
' =====================================================
                        Case 14
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

                            Dim seq As Integer = 1
                            _inventoryRepo.InsertCostLedger_OUT_PRT(transactionID, operationGroupID, seq, oldQtyDict, con, tran)
                            _inventoryRepo.InsertPRTLinks(transactionID, operationGroupID, userID, con, tran)

                            _inventoryRepo.ApplyInventoryOut(transactionID, con, tran)


                            _inventoryRepo.FinalizeLedgerMetadata(operationGroupID, con, tran)



' =====================================================
' 1️⃣4️⃣ SCRAP
' =====================================================


                        Case 13
                            oldQtyDict =
                                _inventoryRepo.GetOldQtyAllStores(transactionID, operationGroupID, con, tran)

                            Dim seq As Integer = 1

                            _inventoryRepo.Receive_ScrapInsideTransaction(
        transactionID,
        userID,
        con,
        tran
    )

                    End Select


                    ' =====================================================
                    ' 3️⃣ Final Status
                    ' =====================================================
                    _inventoryRepo.FinalizeLedgerMetadata(operationGroupID, con, tran)
                    UpdateSourceDocumentStatus(transactionID, userID, con, tran)
                    _inventoryRepo.UpdateFinalStatuses(transactionID, con, tran)
                    tran.Commit()



                Catch ex As Exception

                    tran.Rollback()

                    Dim msg As New Text.StringBuilder()

                    msg.AppendLine("==========================================")
                    msg.AppendLine("        RECEIVE ENGINE CRITICAL ERROR     ")
                    msg.AppendLine("==========================================")
                    msg.AppendLine("Time: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    msg.AppendLine("Server: " & con.DataSource)
                    msg.AppendLine("Database: " & con.Database)
                    msg.AppendLine("TransactionID: " & transactionID)
                    msg.AppendLine("UserID: " & userID)
                    msg.AppendLine("OperationTypeID: " & opTypeID)
                    msg.AppendLine("OperationGroupID: " & operationGroupID.ToString())
                    msg.AppendLine("------------------------------------------")

                    msg.AppendLine("Exception Type:")
                    msg.AppendLine(ex.GetType().FullName)

                    msg.AppendLine("------------------------------------------")
                    msg.AppendLine("Message:")
                    msg.AppendLine(ex.Message)

                    msg.AppendLine("------------------------------------------")
                    msg.AppendLine("StackTrace:")
                    msg.AppendLine(ex.StackTrace)

                    msg.AppendLine("------------------------------------------")
                    msg.AppendLine("TargetSite:")
                    msg.AppendLine(ex.TargetSite?.ToString())

                    msg.AppendLine("------------------------------------------")

                    ' ===== SQL Exception تفاصيل دقيقة =====

                    If TypeOf ex Is SqlClient.SqlException Then

                        Dim sqlEx As SqlClient.SqlException = DirectCast(ex, SqlClient.SqlException)

                        msg.AppendLine("SQL ERROR DETAILS")
                        msg.AppendLine("------------------------------------------")

                        For Each err As SqlClient.SqlError In sqlEx.Errors

                            msg.AppendLine("SQL Error Number : " & err.Number)
                            msg.AppendLine("Line Number      : " & err.LineNumber)
                            msg.AppendLine("Procedure        : " & err.Procedure)
                            msg.AppendLine("State            : " & err.State)
                            msg.AppendLine("Class            : " & err.Class)
                            msg.AppendLine("Message          : " & err.Message)

                            msg.AppendLine("------------------------------------------")

                        Next

                    End If


                    ' ===== Inner Exceptions =====

                    Dim inner = ex.InnerException
                    Dim depth As Integer = 1

                    While inner IsNot Nothing

                        msg.AppendLine("INNER EXCEPTION LEVEL " & depth)

                        msg.AppendLine(inner.GetType().FullName)
                        msg.AppendLine(inner.Message)
                        msg.AppendLine(inner.StackTrace)

                        msg.AppendLine("------------------------------------------")

                        inner = inner.InnerException
                        depth += 1

                    End While


                    ' ===== حفظ في ملف =====

                    Dim logPath As String =
        IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReceiveErrors.txt")

                    IO.File.AppendAllText(logPath, msg.ToString() & Environment.NewLine)

                    ' ===== عرض رسالة مفيدة للمستخدم =====

                    MsgBox(
        "حدث خطأ أثناء تنفيذ الاستلام." & vbCrLf &
        "TransactionID: " & transactionID & vbCrLf &
        "راجع الملف ReceiveErrors.txt للتفاصيل.",
        MsgBoxStyle.Critical
    )

                    Throw New Exception(msg.ToString(), ex)

                End Try

            End Using
        End Using

    End Sub







    Public Function CreateTransfer(
    transactionDate As DateTime,
    userId As Integer,
    details As List(Of TransactionDetailDTO)
) As Integer

        If details Is Nothing OrElse details.Count = 0 Then
            Throw New Exception("لا توجد أصناف في التحويل")
        End If

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' 1️⃣ التحقق من الأرصدة قبل البدء
                    Dim insufficientProducts As New List(Of String)()

                    For Each dto In details
                        Dim balance As Decimal = GetProductBalance(dto.ProductID, dto.SourceStoreID, con, tran)

                        If balance < dto.Quantity Then
                            Dim productName = GetProductName(dto.ProductID, con, tran)
                            insufficientProducts.Add($"{productName} (المتوفر: {balance:N2}, المطلوب: {dto.Quantity:N2})")
                        End If
                    Next

                    If insufficientProducts.Count > 0 Then
                        Dim msg As String = "لا يمكن إتمام التحويل بسبب نقص الكمية في المخزن:" & vbCrLf & vbCrLf
                        msg &= String.Join(vbCrLf, insufficientProducts)
                        Throw New Exception(msg)
                    End If

                    ' 2️⃣ Validation
                    _validationEngine.ValidateTransfer(details)

                    ' 3️⃣ Get OperationTypeID
                    Dim operationTypeID = _workflowEngine.GetOperationTypeID("TRN", con, tran)

                    ' 4️⃣ Generate Code
                    Dim transactionCode = _transactionRepo.GetNextTransactionCode("TRN", con, tran)

                    ' 5️⃣ Insert Header (✅ الآن يعبّي SentAt/SentBy)
                    Dim newID = _transactionRepo.InsertTransferHeader(
                    transactionDate,
                    transactionCode,
                    operationTypeID,
                    userId,
                    con,
                    tran
                )

                    ' 6️⃣ Insert Details (UnitCost قد يكون 0 هنا؛ سيتم تعبئته وقت الترحيل)
                    For Each dto In details
                        dto.CostAmount = dto.Quantity * dto.UnitCost
                    Next

                    _transactionRepo.InsertTransferDetails(newID, details, con, tran)

                    ' 7️⃣ Self Reference
                    _transactionRepo.UpdateSelfReference(newID, con, tran)

                    ' ✅ 8️⃣ Post داخل نفس الـ Transaction
                    _transactionRepo.ExecutePostTransaction(newID, userId, con, tran)

                    tran.Commit()
                    Return newID

                Catch ex As Exception
                    tran.Rollback()

                    If ex.Message.Contains("نقص الكمية") Then
                        Throw New Exception(ex.Message)
                    ElseIf ex.Message.Contains("FK") OrElse ex.Message.Contains("foreign key") Then
                        Throw New Exception("خطأ في البيانات المدخلة: تأكد من صحة المنتجات والمخازن")
                    Else
                        Throw
                    End If
                End Try
            End Using
        End Using

    End Function




    Private Function GetProductBalance(
        productID As Integer,
        storeID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As Decimal
        Dim sql = "SELECT ISNULL(QtyOnHand, 0) FROM Inventory_Balance WHERE ProductID = @ProductID AND StoreID = @StoreID"
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            cmd.Parameters.AddWithValue("@StoreID", storeID)
            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then
                Return 0
            End If
            Return Convert.ToDecimal(result)
        End Using
    End Function

    Private Function GetProductName(
        productID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    ) As String
        Dim sql = "SELECT ProductName FROM Master_Product WHERE ProductID = @ProductID"
        Using cmd As New SqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@ProductID", productID)
            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then
                Return "منتج غير معروف"
            End If
            Return result.ToString()
        End Using
    End Function
    Public Sub PostTransaction(transactionID As Integer, userID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    _transactionRepo.ExecutePostTransaction(transactionID, userID, con, tran)

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub

    Public Function CalculateWeightedAverage_PUR_PRO(
            oldQty As Decimal,
            oldAvgCost As Decimal,
            inQty As Decimal,
            inAvg As Decimal
        ) As Decimal

        Dim totalQty As Decimal = oldQty + inQty

        ' CASE 1: total = 0 → keep old cost
        If totalQty = 0D Then
            Return oldAvgCost
        End If

        ' CASE 2: no previous stock
        If oldQty = 0D Then
            Return inAvg
        End If

        ' CASE 3: weighted average (نفس SQL بالضبط)
        Return ((oldQty * oldAvgCost) + (inQty * inAvg)) / totalQty

    End Function

    Public Sub ApplyAverageCost_PUR_PRO(
        products As List(Of ProductAvgDTO)
    )

        For Each item In products

            Dim newAvg As Decimal = CalculateWeightedAverage_PUR_PRO(
                item.OldQty,
                item.OldAvgCost,
                item.InQty,
                item.InAvg
            )

            item.NewAvgCost = newAvg

        Next

    End Sub
    Public Function CalculateWeightedAverage(
    oldQty As Decimal,
    oldAvgCost As Decimal,
    inQty As Decimal,
    inAvgCost As Decimal
) As Decimal

        Dim totalQty As Decimal = oldQty + inQty

        If totalQty = 0D Then
            Return oldAvgCost
        End If

        If oldQty = 0D Then
            Return inAvgCost
        End If

        Return ((oldQty * oldAvgCost) + (inQty * inAvgCost)) / totalQty

    End Function
    Public Function CalculateM3Quantity(
        quantity As Decimal,
        length As Decimal,
        width As Decimal,
        height As Decimal
    ) As Decimal

        Dim volumePerUnit As Decimal = (length * width * height) / 1000000D
        Return quantity * volumePerUnit

    End Function
    Public Sub ApplyAverageCost(
        items As List(Of ProductAverageDTO)
    )

        For Each item In items

            item.NewAvgCost = CalculateWeightedAverage(
                item.OldQty,
                item.OldAvgCost,
                item.InQty,
                item.InAvgCost
            )

        Next

    End Sub
    Private Sub UpdateSourceDocumentStatus(
        transactionID As Integer,
        userID As Integer,
        con As SqlConnection,
        tran As SqlTransaction
    )

        Dim operationCode As String = ""
        Dim sourceDocumentID As Integer = 0

        ' 1️⃣ قراءة نوع العملية + المصدر
        Using cmd As New SqlCommand("
        SELECT h.SourceDocumentID, ot.OperationCode
        FROM Inventory_TransactionHeader h
        INNER JOIN Workflow_OperationType ot
            ON ot.OperationTypeID = h.OperationTypeID
        WHERE h.TransactionID = @ID
    ", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)

            Using rd = cmd.ExecuteReader()
                If rd.Read() Then
                    sourceDocumentID = If(IsDBNull(rd("SourceDocumentID")), 0, CInt(rd("SourceDocumentID")))
                    operationCode = rd("OperationCode").ToString()
                End If
            End Using
        End Using

        ' 2️⃣ تحديث الترانسكشن نفسه
        Using cmd As New SqlCommand("
        UPDATE Inventory_TransactionHeader
        SET IsInventoryPosted = 1,
            StatusID = 6,
            ReceivedAt = SYSDATETIME(),
            ReceivedBy = @UserID
        WHERE TransactionID = @ID
    ", con, tran)

            cmd.Parameters.AddWithValue("@ID", transactionID)
            cmd.Parameters.AddWithValue("@UserID", userID)
            cmd.ExecuteNonQuery()
        End Using

        ' 3️⃣ تحديث المستند المصدر حسب نوع العملية
        If sourceDocumentID = 0 Then Exit Sub

        Select Case operationCode

            Case "PRO"
                Using cmd As New SqlCommand("
                UPDATE Production_Header
                SET IsInventoryPosted = 1,
                    StatusID = 6
                WHERE ProductionID = @ID
            ", con, tran)

                    cmd.Parameters.AddWithValue("@ID", sourceDocumentID)
                    cmd.ExecuteNonQuery()
                End Using

            Case "PUR"
                Using cmd As New SqlCommand("
                UPDATE Inventory_DocumentHeader
                SET IsInventoryPosted = 1,
                    StatusID = 6
                WHERE DocumentID = @ID
            ", con, tran)

                    cmd.Parameters.AddWithValue("@ID", sourceDocumentID)
                    cmd.ExecuteNonQuery()
                End Using

            Case "CUT"
                Using cmd As New SqlCommand("
                UPDATE Production_CuttingHeader
                SET IsInventoryPosted = 1,
                    StatusID = 6
                WHERE CuttingID = @ID
            ", con, tran)

                    cmd.Parameters.AddWithValue("@ID", sourceDocumentID)
                    cmd.ExecuteNonQuery()
                End Using

            Case "SRT"
                Using cmd As New SqlCommand("
                UPDATE Inventory_DocumentHeader
                SET IsInventoryPosted = 1,
                    StatusID = 6
                WHERE DocumentID = @ID
            ", con, tran)

                    cmd.Parameters.AddWithValue("@ID", sourceDocumentID)
                    cmd.ExecuteNonQuery()
                End Using
            Case "SCR"
                Using cmd As New SqlCommand("
        UPDATE dbo.Inventory_WasteHeader
        SET StatusID = 6,
            ReceivedAt = SYSDATETIME(),
            ReceivedBy = @UserID
        WHERE WasteID = @ID
    ", con, tran)

                    cmd.Parameters.AddWithValue("@ID", sourceDocumentID)
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    cmd.ExecuteNonQuery()
                End Using
        End Select

    End Sub
    Public Function CreateTransaction(
        transactionDate As Date,
        userId As Integer,
        details As DataTable
    ) As Integer

        Using con As New SqlConnection(_connectionString)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    ' 1️⃣ إدخال الهيدر
                    ' في TransactionEngine.vb - داخل CreateTransaction

                    ' أولاً: جلب PeriodID النشط
                    Dim periodID As Integer

                    Dim periodSQL As String = "
SELECT TOP 1 PeriodID
FROM cfg.FiscalPeriod
WHERE @TransactionDate BETWEEN StartDate AND EndDate
  AND IsOpen = 1
ORDER BY PeriodID"

                    Using cmdPeriod As New SqlCommand(periodSQL, con, tran)
                        cmdPeriod.Parameters.AddWithValue("@TransactionDate", transactionDate)
                        Dim result = cmdPeriod.ExecuteScalar()

                        If result Is Nothing OrElse IsDBNull(result) Then
                            Throw New Exception("لا توجد فترة محاسبية مفتوحة للتاريخ " & transactionDate.ToShortDateString())
                        End If

                        periodID = CInt(result)
                    End Using

                    ' ثم استخدم periodID في INSERT
                    Dim headerSQL As String = "
INSERT INTO Inventory_TransactionHeader
(
    TransactionDate,
    SourceDocumentID,
    OperationTypeID,
    PeriodID,
    StatusID,
    IsFinancialPosted,
    IsInventoryPosted,
    CreatedBy,
    CreatedAt
)
VALUES
(
    @TransactionDate,
    0,
    10,
    @PeriodID,
    5,
    0,
    0,
    @UserID,
    SYSDATETIME()
);
SELECT SCOPE_IDENTITY();"
                    Dim newID As Integer = 0

                    Using cmd As New SqlCommand(headerSQL, con, tran)
                        cmd.Parameters.AddWithValue("@TransactionDate", transactionDate)
                        cmd.Parameters.AddWithValue("@PeriodID", periodID)
                        cmd.Parameters.AddWithValue("@UserID", userId)

                        newID = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    ' 2️⃣ إدخال التفاصيل مع حساب التكلفة
                    For Each row As DataRow In details.Rows
                        Dim productID As Integer = CInt(row("ProductID"))
                        Dim qty As Decimal = CDec(row("Qty"))
                        Dim unitID As Integer = CInt(row("UnitID"))
                        Dim sourceStoreID As Integer = CInt(row("SourceStoreID"))
                        Dim targetStoreID As Integer = CInt(row("TargetStoreID"))

                        ' جلب معلومات المنتج وحساب التكلفة
                        Dim unitCost As Decimal = 0D
                        Dim costAmount As Decimal = 0D

                        ' استعلام جلب بيانات المنتج
                        Dim productSQL As String = "
                    SELECT 
                        p.StorageUnitID,
                        p.BaseProductID,
                        p.Length,
                        p.Width,
                        p.Height,
                        p.AvgCost,
                        u.UnitCode,
                        u.UnitName,
                        bp.StorageUnitID AS BaseStorageUnitID,
                        bp.Length AS BaseLength,
                        bp.Width AS BaseWidth,
                        bp.Height AS BaseHeight
                    FROM Master_Product p
                    INNER JOIN Master_Unit u ON u.UnitID = p.StorageUnitID
                    LEFT JOIN Master_Product bp ON bp.ProductID = p.BaseProductID
                    WHERE p.ProductID = @ProductID"

                        Dim productInfo As DataTable = New DataTable()
                        Using cmd As New SqlCommand(productSQL, con, tran)
                            cmd.Parameters.AddWithValue("@ProductID", productID)
                            Using da As New SqlDataAdapter(cmd)
                                da.Fill(productInfo)
                            End Using
                        End Using

                        If productInfo.Rows.Count > 0 Then
                            Dim dr As DataRow = productInfo.Rows(0)
                            Dim storageUnitID As Integer = CInt(dr("StorageUnitID"))
                            Dim unitCode As String = dr("UnitCode").ToString()
                            Dim baseProductID As Object = dr("BaseProductID")
                            Dim avgCost As Decimal = CDec(dr("AvgCost"))

                            ' الحالة 1: وحدة التخزين = متر مكعب (M3)
                            If unitCode = "M3" Then
                                ' جلب التكلفة من FinalProductAvgCost
                                Dim costSQL As String = "
                            SELECT TOP 1 AvgCostPerM3forFG
                            FROM Master_FinalProductAvgCost
                            WHERE BaseProductID = @BaseProductID
                            ORDER BY CreatedAt DESC"

                                Using cmd As New SqlCommand(costSQL, con, tran)
                                    cmd.Parameters.AddWithValue("@BaseProductID", If(baseProductID Is DBNull.Value, productID, CInt(baseProductID)))
                                    Dim result = cmd.ExecuteScalar()
                                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                                        unitCost = CDec(result)
                                    Else
                                        unitCost = avgCost
                                    End If
                                End Using

                                ' الحالة 2: المنتج له BaseProduct و BaseProduct وحدته M3
                            ElseIf baseProductID IsNot Nothing AndAlso Not IsDBNull(baseProductID) Then
                                Dim baseStorageUnitID As Object = dr("BaseStorageUnitID")

                                ' التحقق من أن وحدة الـ BaseProduct هي M3
                                Dim checkBaseUnitSQL = "SELECT UnitCode FROM Master_Unit WHERE UnitID = @UnitID"
                                Dim baseUnitCode As String = ""
                                Using cmd As New SqlCommand(checkBaseUnitSQL, con, tran)
                                    cmd.Parameters.AddWithValue("@UnitID", baseStorageUnitID)
                                    Dim result = cmd.ExecuteScalar()
                                    If result IsNot Nothing Then
                                        baseUnitCode = result.ToString()
                                    End If
                                End Using

                                If baseUnitCode = "M3" Then
                                    ' جلب AvgCostPerM3forFG للـ BaseProduct
                                    Dim costSQL As String = "
                                SELECT TOP 1 AvgCostPerM3forFG
                                FROM Master_FinalProductAvgCost
                                WHERE BaseProductID = @BaseProductID
                                ORDER BY CreatedAt DESC"

                                    Dim costPerM3 As Decimal = 0D
                                    Using cmd As New SqlCommand(costSQL, con, tran)
                                        cmd.Parameters.AddWithValue("@BaseProductID", CInt(baseProductID))
                                        Dim result = cmd.ExecuteScalar()
                                        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                                            costPerM3 = CDec(result)
                                        Else
                                            costPerM3 = avgCost
                                        End If
                                    End Using

                                    ' حساب حجم القطعة بالمتر المكعب
                                    Dim length As Decimal = If(dr("Length") Is DBNull.Value, 0D, CDec(dr("Length")))
                                    Dim width As Decimal = If(dr("Width") Is DBNull.Value, 0D, CDec(dr("Width")))
                                    Dim height As Decimal = If(dr("Height") Is DBNull.Value, 0D, CDec(dr("Height")))

                                    Dim volumePerUnit As Decimal = (length * width * height) / 1000000D
                                    unitCost = costPerM3 * volumePerUnit
                                Else
                                    unitCost = avgCost
                                End If

                                ' الحالة 3: باقي الحالات
                            Else
                                unitCost = avgCost
                            End If
                        End If

                        costAmount = qty * unitCost

                        ' إدخال التفاصيل مع التكلفة المحسوبة
                        Dim detailSQL As String = "
                    INSERT INTO Inventory_TransactionDetails
                    (
                        TransactionID,
                        ProductID,
                        Quantity,
                        UnitID,
                        UnitCost,
                        CostAmount,
                        SourceStoreID,
                        TargetStoreID,
                        SourceDocumentDetailID,
                        CreatedAt,
                        CreatedBy
                    )
                    VALUES
                    (
                        @TransactionID,
                        @ProductID,
                        @Quantity,
                        @UnitID,
                        @UnitCost,
                        @CostAmount,
                        @SourceStoreID,
                        @TargetStoreID,
                        0,
                        SYSDATETIME(),
                        @UserID
                    )"

                        Using cmd As New SqlCommand(detailSQL, con, tran)
                            cmd.Parameters.AddWithValue("@TransactionID", newID)
                            cmd.Parameters.AddWithValue("@ProductID", productID)
                            cmd.Parameters.AddWithValue("@Quantity", qty)
                            cmd.Parameters.AddWithValue("@UnitID", unitID)
                            cmd.Parameters.AddWithValue("@UnitCost", unitCost)
                            cmd.Parameters.AddWithValue("@CostAmount", costAmount)
                            cmd.Parameters.AddWithValue("@SourceStoreID", sourceStoreID)
                            cmd.Parameters.AddWithValue("@TargetStoreID", targetStoreID)
                            cmd.Parameters.AddWithValue("@UserID", userId)

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()
                    Return newID

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Function

    Public Sub CancelTransaction(transactionID As Integer, userID As Integer)

        Using con As New SqlConnection(_connectionString)
            con.Open()

            Using tran = con.BeginTransaction()

                Try
                    ' 1️⃣ التحقق من وجود السند وصلاحيته للإلغاء
                    Dim currentStatusID As Integer = 0
                    Dim isPosted As Boolean = False

                    Using cmd As New SqlCommand("
                    SELECT 
                        h.StatusID,
                        h.IsInventoryPosted
                    FROM dbo.Inventory_TransactionHeader h
                    WHERE h.TransactionID = @TransactionID
                ", con, tran)

                        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

                        Using reader = cmd.ExecuteReader()
                            If Not reader.Read() Then
                                Throw New Exception("السند غير موجود.")
                            End If

                            currentStatusID = reader.GetInt32(0)
                            isPosted = reader.GetBoolean(1)
                        End Using
                    End Using

                    ' 2️⃣ التحقق من عدم الترحيل المخزني
                    If isPosted Then
                        Throw New Exception("لا يمكن إلغاء سند مرحّل مخزنيًا.")
                    End If

                    ' 3️⃣ التحقق من أن الحالة مسموح بإلغائها (2 أو 5 فقط)
                    If currentStatusID <> 2 AndAlso currentStatusID <> 5 Then
                        Throw New Exception("لا يمكن إلغاء السند في حالته الحالية. الحالات المسموح بها: 2 و 5 فقط.")
                    End If

                    ' 4️⃣ تنفيذ الإلغاء (تغيير الحالة إلى 10)
                    Using cmd As New SqlCommand("
                    UPDATE dbo.Inventory_TransactionHeader
                    SET StatusID = 10  -- حالة ملغي
                    WHERE TransactionID = @TransactionID
                ", con, tran)

                        cmd.Parameters.AddWithValue("@TransactionID", transactionID)

                        Dim rowsAffected = cmd.ExecuteNonQuery()

                        If rowsAffected = 0 Then
                            Throw New Exception("فشل تحديث السند.")
                        End If
                    End Using

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try

            End Using
        End Using

    End Sub
    Public Sub UpdateTransaction(transactionID As Integer, userID As Integer, details As DataTable)
        Using con As New SqlConnection(_connectionString)
            con.Open()
            Using tran As SqlTransaction = con.BeginTransaction()
                Try
                    ' ✅ تحديث الهيدر - الأعمدة الصحيحة
                    Using cmd As New SqlCommand("
                    UPDATE Inventory_TransactionHeader 
                    SET 
                        CreatedBy = @UserID,        -- آخر من عدل
                        CreatedAt = @CurrentDate    -- تاريخ التعديل
                    WHERE TransactionID = @ID", con, tran)

                        cmd.Parameters.AddWithValue("@ID", transactionID)
                        cmd.Parameters.AddWithValue("@UserID", userID)
                        cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now)

                        cmd.ExecuteNonQuery()
                    End Using

                    ' حذف التفاصيل القديمة
                    Using cmd As New SqlCommand("DELETE FROM Inventory_TransactionDetails WHERE TransactionID = @ID", con, tran)
                        cmd.Parameters.AddWithValue("@ID", transactionID)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' ✅ إضافة التفاصيل الجديدة - مع الأعمدة الصحيحة
                    For Each row As DataRow In details.Rows
                        Using cmd As New SqlCommand("
                        INSERT INTO Inventory_TransactionDetails
                        (
                            TransactionID, 
                            ProductID, 
                            Quantity, 
                            UnitID, 
                            UnitCost,           -- ✅ اسم العمود الصحيح (ليس UnitPrice)
                            CostAmount,         -- ✅ يجب حسابه
                            SourceStoreID, 
                            TargetStoreID,
                            SourceDocumentDetailID, -- ✅ قيمة افتراضية
                            CreatedAt,               -- ✅ تاريخ الإنشاء
                            CreatedBy                 -- ✅ منشئ التفاصيل
                        )
                        VALUES
                        (
                            @TransactionID, 
                            @ProductID, 
                            @Qty, 
                            @UnitID, 
                            @UnitCost,          -- ✅ UnitCost
                            @CostAmount,        -- ✅ CostAmount
                            @SourceStoreID, 
                            @TargetStoreID,
                            0,                   -- SourceDocumentDetailID افتراضي
                            @CreatedAt,          -- تاريخ الآن
                            @CreatedBy            -- userID
                        )", con, tran)

                            ' قراءة الكمية
                            Dim qty As Decimal = Convert.ToDecimal(row("Qty"))

                            ' حساب UnitCost و CostAmount
                            Dim unitCost As Decimal = 0D
                            If row.Table.Columns.Contains("UnitPrice") AndAlso Not IsDBNull(row("UnitPrice")) Then
                                unitCost = Convert.ToDecimal(row("UnitPrice"))
                            End If

                            Dim costAmount As Decimal = qty * unitCost

                            cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                            cmd.Parameters.AddWithValue("@ProductID", row("ProductID"))
                            cmd.Parameters.AddWithValue("@Qty", qty)
                            cmd.Parameters.AddWithValue("@UnitID", row("UnitID"))
                            cmd.Parameters.AddWithValue("@UnitCost", unitCost)
                            cmd.Parameters.AddWithValue("@CostAmount", costAmount)
                            cmd.Parameters.AddWithValue("@SourceStoreID",
                                If(IsDBNull(row("SourceStoreID")), DBNull.Value, row("SourceStoreID")))
                            cmd.Parameters.AddWithValue("@TargetStoreID",
                                If(IsDBNull(row("TargetStoreID")), DBNull.Value, row("TargetStoreID")))
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now)
                            cmd.Parameters.AddWithValue("@CreatedBy", userID)

                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Public Sub ReverseReceiveTransaction(
        transactionID As Integer,
        userID As Integer,
        reason As String
    )
        Using con As New SqlConnection(_connectionString)
            con.Open()
            Using tran = con.BeginTransaction()
                Try
                    ' 1. التحقق من إمكانية الإلغاء
                    Dim validationEngine As New ValidationEngine()
                    validationEngine.ValidateReversePossibility(transactionID, con, tran)
                    _inventoryRepo.ValidateNoLaterAverageUsage(
    transactionID,
    con,
    tran
)
                    ' 2. الحصول على تفاصيل الحركة الأصلية
                    Dim details = _transactionRepo.GetTransactionDetails(transactionID, con, tran)

                    ' 3. الحصول على كود التصحيح الجديد
                    Dim correctionCode = _workflowEngine.GetNextCorrectionCode(con, tran)

                    ' 4. معالجة كل صنف على حدة
                    For Each detail In details
                        ' 4.1 الحصول على المتوسط القديم من CostLedger
                        Dim oldAvgData = _inventoryRepo.GetPreviousAvgCostFromCostLedger(
                            detail.ProductID,
                            detail.StoreID,
                            transactionID,
                            con,
                            tran
                        )

                        ' 4.2 عكس حركة المخزون (إنقاص الكمية)
                        _inventoryRepo.ReverseInventoryMovement(detail, con, tran)

                        ' 4.3 استعادة المتوسط القديم
                        Dim unitID = _inventoryRepo.GetProductUnitID(detail.ProductID, con, tran)

                        If unitID = 8 Then  ' إذا كانت الوحدة متر مكعب (رقم 3 افتراضي)
                            _inventoryRepo.UpdateFinalProductAvgCost(
                                detail.BaseProductID,
                                oldAvgData.OldAvgCostPerM3,
                                con,
                                tran
                            )
                        Else
                            _inventoryRepo.UpdateProductAvgCost(
                                detail.ProductID,
                                oldAvgData.OldAvgCost,
                                con,
                                tran
                            )
                        End If

                        ' 4.4 تسجيل في CostLedger
                        Dim costLedgerID = _inventoryRepo.LogReverseInCostLedger(
                            detail,
                            userID,
                            oldAvgData,
                            con,
                            tran
                        )

                        ' 4.5 تسجيل في Correction Table
                        _inventoryRepo.InsertCorrectionRecord(
                            correctionCode,
                            detail,
                            oldAvgData,
                            costLedgerID,
                            userID,
                            reason,
                            con,
                            tran
                        )
                    Next

                    ' 5. تحديث حالة المستندات
                    _inventoryRepo.UpdateDocumentStatus(transactionID, con, tran)
                    _workflowEngine.UpdateTransactionStatus(transactionID, 9, userID, con, tran) ' 9 = ملغي

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub




End Class