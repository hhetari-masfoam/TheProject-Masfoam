Imports System.Data.SqlClient


Public Class frmCostCorrection
    Private CurrentTransactionDetailID As Integer = 0
    Private _costCorrectionService As CostCorrectionService
    Private Class SimulationCorrection
        Public Property LedgerID As Long
        Public Property ProductID As Integer
        Public Property StoreID As Integer?
        Public Property NewQty As Decimal
        Public Property NewUnitCost As Decimal
    End Class


    ' GENERAL OPERATIONS
    Private Sub InitializeAffectedOperationsGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("InQty", GetType(Decimal))
        dt.Columns.Add("OutQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("InUnitCost", GetType(Decimal))
        dt.Columns.Add("OutUnitCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))


        dgvAffectedOperations.Columns.Clear()
        dgvAffectedOperations.AutoGenerateColumns = True
        dgvAffectedOperations.DataSource = dt
        dgvAffectedOperations.Columns("LedgerID").DisplayIndex = 0
        dgvAffectedOperations.Columns("TransactionID").DisplayIndex = 1
        dgvAffectedOperations.Columns("SourceDetailID").DisplayIndex = 2
        dgvAffectedOperations.Columns("ProductID").DisplayIndex = 3
        dgvAffectedOperations.Columns("BaseProductID").DisplayIndex = 4
        dgvAffectedOperations.Columns("StoreID").DisplayIndex = 5
        dgvAffectedOperations.Columns("OperationTypeID").DisplayIndex = 6

        dgvAffectedOperations.Columns("LocalOldQty").DisplayIndex = 7
        dgvAffectedOperations.Columns("OldQty").DisplayIndex = 8
        dgvAffectedOperations.Columns("InQty").DisplayIndex = 9
        dgvAffectedOperations.Columns("OutQty").DisplayIndex = 10
        dgvAffectedOperations.Columns("NewQty").DisplayIndex = 11
        dgvAffectedOperations.Columns("LocalNewQty").DisplayIndex = 12

        dgvAffectedOperations.Columns("OldAvgCost").DisplayIndex = 13
        dgvAffectedOperations.Columns("InUnitCost").DisplayIndex = 14
        dgvAffectedOperations.Columns("OutUnitCost").DisplayIndex = 15
        dgvAffectedOperations.Columns("NewAvgCost").DisplayIndex = 16

        dgvAffectedOperations.Columns("LedgerSequence").DisplayIndex = 17
        dgvAffectedOperations.Columns("SourceLedgerID").DisplayIndex = 18
        dgvAffectedOperations.Columns("SupersededByLedgerID").DisplayIndex = 19
        dgvAffectedOperations.Columns("RootTransactionID").DisplayIndex = 20

        dgvAffectedOperations.Columns("PostingDate").DisplayIndex = 21

        For Each col As DataGridViewColumn In dgvAffectedOperations.Columns
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next
    End Sub
    Private Sub FillAffectedOperations(source As DataTable)

        '========================================
        ' 1) تأكد أن DataSource موجود
        '========================================
        Dim dt As DataTable = TryCast(dgvAffectedOperations.DataSource, DataTable)

        If dt Is Nothing Then
            dt = CreateAffectedOperationsTable() ' 🔥 ننشئه من جديد
            dgvAffectedOperations.DataSource = dt
        Else
            dt.Rows.Clear()
        End If

        If source Is Nothing Then Exit Sub

        '========================================
        ' 2) تعبئة البيانات
        '========================================
        For Each r As DataRow In source.Rows

            Dim row = dt.NewRow()

            row("LedgerID") = r("LedgerID")
            row("TransactionID") = r("TransactionID")
            row("SourceDetailID") = If(IsDBNull(r("SourceDetailID")), DBNull.Value, r("SourceDetailID"))

            row("ProductID") = r("ProductID")
            row("BaseProductID") = If(source.Columns.Contains("BaseProductID"), r("BaseProductID"), r("ProductID"))
            row("StoreID") = r("StoreID")
            row("OperationTypeID") = r("OperationTypeID")

            row("LocalOldQty") = r("LocalOldQty")
            row("OldQty") = r("OldQty")

            row("InQty") = r("InQty")
            row("OutQty") = r("OutQty")

            row("NewQty") = r("NewQty")
            row("LocalNewQty") = r("LocalNewQty")

            row("OldAvgCost") = r("OldAvgCost")

            row("InUnitCost") = If(source.Columns.Contains("InUnitCost"), r("InUnitCost"), 0D)
            row("OutUnitCost") = If(source.Columns.Contains("OutUnitCost"), r("OutUnitCost"), 0D)

            row("NewAvgCost") = r("NewAvgCost")

            row("LedgerSequence") = r("LedgerSequence")
            row("SourceLedgerID") = r("SourceLedgerID")

            row("SupersededByLedgerID") = If(source.Columns.Contains("SupersededByLedgerID") AndAlso Not IsDBNull(r("SupersededByLedgerID")), r("SupersededByLedgerID"), DBNull.Value)
            row("RootTransactionID") = If(source.Columns.Contains("RootTransactionID") AndAlso Not IsDBNull(r("RootTransactionID")), r("RootTransactionID"), DBNull.Value)

            row("PostingDate") = r("PostingDate")

            dt.Rows.Add(row)

        Next

    End Sub
    Private Function CreateAffectedOperationsTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("InQty", GetType(Decimal))
        dt.Columns.Add("OutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("InUnitCost", GetType(Decimal))
        dt.Columns.Add("OutUnitCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Long))
        dt.Columns.Add("SourceLedgerID", GetType(Long))

        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(DateTime))

        Return dt

    End Function




    Private Function CreateSimulationTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("OperationGroupID", GetType(String))
        dt.Columns.Add("GroupSeq", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("CorrectInQty", GetType(Decimal))
        dt.Columns.Add("CorrectOutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))

        dt.Columns.Add("CorrectInUnitCost", GetType(Decimal))
        dt.Columns.Add("CorrectOutUnitCost", GetType(Decimal))

        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))

        Return dt

    End Function
    Private Sub InitializeSimulationGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Long))
        dt.Columns.Add("SourceDetailID", GetType(Long))

        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("BaseProductID", GetType(Integer))
        dt.Columns.Add("StoreID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("OperationGroupID", GetType(String))
        dt.Columns.Add("GroupSeq", GetType(Integer))

        dt.Columns.Add("LocalOldQty", GetType(Decimal))
        dt.Columns.Add("OldQty", GetType(Decimal))

        dt.Columns.Add("CorrectInQty", GetType(Decimal))
        dt.Columns.Add("CorrectOutQty", GetType(Decimal))

        dt.Columns.Add("NewQty", GetType(Decimal))
        dt.Columns.Add("LocalNewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))

        dt.Columns.Add("CorrectInUnitCost", GetType(Decimal))
        dt.Columns.Add("CorrectOutUnitCost", GetType(Decimal))

        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        dt.Columns.Add("LedgerSequence", GetType(Integer))
        dt.Columns.Add("SourceLedgerID", GetType(Long))
        dt.Columns.Add("SupersededByLedgerID", GetType(Long))
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))

        dgvSimulation.AutoGenerateColumns = True
        dgvSimulation.DataSource = dt

        dgvSimulation.ReadOnly = True
        dgvSimulation.AllowUserToAddRows = False
        dgvSimulation.AllowUserToDeleteRows = False

        For Each col As DataGridViewColumn In dgvSimulation.Columns

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If

        Next

    End Sub

    Private Sub frmCostCorrection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _costCorrectionService = New CostCorrectionService(ConnStr)

        InitializeAffectedOperationsGrid() ' 🔥 مهم جدًا

        LoadCorrectionQueue()

    End Sub


    Private Sub LoadCorrectionQueue()

        Try

            Dim dt = _costCorrectionService.GetCorrectionQueue()

            dgvCorrectionQueue.AutoGenerateColumns = True
            dgvCorrectionQueue.DataSource = dt

            dgvCorrectionQueue.ReadOnly = True
            dgvCorrectionQueue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Catch ex As Exception
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
    Private Sub dgvCorrectionQueue_SelectionChanged(
        sender As Object,
        e As EventArgs
    ) Handles dgvCorrectionQueue.SelectionChanged

        If dgvCorrectionQueue.CurrentRow Is Nothing Then Exit Sub

        Dim row = dgvCorrectionQueue.CurrentRow

        Dim detailID As Integer

        Dim newDetailID As Integer =
    CInt(row.Cells("TransactionDetailID").Value)

        Dim oldDetailIDObj = row.Cells("CorrectionReferenceDetailID").Value

        If Not IsDBNull(oldDetailIDObj) Then
            ' 🟡 تعديل → نرجع للقديم
            detailID = CInt(oldDetailIDObj)
        Else
            ' 🟢 إلغاء أو بدون تعديل
            detailID = newDetailID
        End If
        If detailID <= 0 Then
            dgvAffectedOperations.DataSource = Nothing
            Exit Sub
        End If

        ' =========================
        ' جلب اللدجر والتأثيرات
        ' =========================

        Dim dt = _costCorrectionService.GetAffectedCostDependencies(
            New List(Of Integer) From {detailID}
        )

        FillAffectedOperations(dt)

    End Sub
End Class

