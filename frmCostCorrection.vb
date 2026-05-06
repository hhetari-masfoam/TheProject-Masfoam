Imports System.Data.SqlClient


Public Class frmCostCorrection
    Private CurrentTransactionDetailID As Integer = 0
    '  Private _costCorrectionService As CostCorrectionService
    Private coscorrectionengin As CostCorrectionEngine

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
        dgvAffectedOperations.Columns("RootTransactionID").DisplayIndex = 20

        dgvAffectedOperations.Columns("PostingDate").DisplayIndex = 19

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
        dt.Columns.Add("RootTransactionID", GetType(Long))

        dt.Columns.Add("PostingDate", GetType(Date))

        dgvSimulation.AutoGenerateColumns = True
        dgvSimulation.DataSource = dt

        dgvSimulation.ReadOnly = True
        dgvSimulation.AllowUserToAddRows = False
        dgvSimulation.AllowUserToDeleteRows = False

        Dim cols = {
            "LocalOldQty", "OldQty", "CorrectInQty", "CorrectOutQty",
            "NewQty", "LocalNewQty",
            "OldAvgCost", "CorrectInUnitCost", "CorrectOutUnitCost", "NewAvgCost"
        }

        For Each LocalOldQty In cols
            If dgvSimulation.Columns.Contains(LocalOldQty) Then
                dgvSimulation.Columns(LocalOldQty).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each OldQty In cols
            If dgvSimulation.Columns.Contains(OldQty) Then
                dgvSimulation.Columns(OldQty).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each CorrectOutQty In cols
            If dgvSimulation.Columns.Contains(CorrectOutQty) Then
                dgvSimulation.Columns(CorrectOutQty).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each NewQty In cols
            If dgvSimulation.Columns.Contains(NewQty) Then
                dgvSimulation.Columns(NewQty).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each LocalNewQty In cols
            If dgvSimulation.Columns.Contains(LocalNewQty) Then
                dgvSimulation.Columns(LocalNewQty).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each OldAvgCost In cols
            If dgvSimulation.Columns.Contains(OldAvgCost) Then
                dgvSimulation.Columns(OldAvgCost).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each CorrectInUnitCost In cols
            If dgvSimulation.Columns.Contains(CorrectInUnitCost) Then
                dgvSimulation.Columns(CorrectInUnitCost).DefaultCellStyle.Format = "N2"
            End If
        Next
        For Each CorrectOutUnitCost In cols
            If dgvSimulation.Columns.Contains(CorrectOutUnitCost) Then
                dgvSimulation.Columns(CorrectOutUnitCost).DefaultCellStyle.Format = "N2"
            End If
        Next

        For Each NewAvgCost In cols
            If dgvSimulation.Columns.Contains(NewAvgCost) Then
                dgvSimulation.Columns(NewAvgCost).DefaultCellStyle.Format = "N2"
            End If
        Next

    End Sub

    Private Sub HideEmptyColumns(grid As DataGridView)

        For Each col As DataGridViewColumn In grid.Columns

            Dim hasValue As Boolean = False

            For Each row As DataGridViewRow In grid.Rows

                If row.IsNewRow Then Continue For

                Dim val = row.Cells(col.Index).Value

                If val IsNot Nothing AndAlso Not IsDBNull(val) AndAlso val.ToString().Trim() <> "" Then
                    hasValue = True
                    Exit For
                End If

            Next

            col.Visible = hasValue

        Next

    End Sub


    Private Sub FormatCorrectionQuiueGrid(grid As DataGridView)

        ' =========================
        ' 1) إخفاء أعمدة (أمثلة)
        ' =========================

        If grid.Columns.Contains("ID") Then
            grid.Columns("ID").Visible = False
        End If

        If grid.Columns.Contains("StatusID") Then
            grid.Columns("StatusID").Visible = False
        End If
        If grid.Columns.Contains("CostGroupID") Then
            grid.Columns("CostGroupID").Visible = False
        End If
        If grid.Columns.Contains("DocumentProductID") Then
            grid.Columns("DocumentProductID").Visible = False
        End If
        If grid.Columns.Contains("DocumentQuantity") Then
            grid.Columns("DocumentQuantity").Visible = False
        End If
        If grid.Columns.Contains("DocumentUnitPrice") Then
            grid.Columns("DocumentUnitPrice").Visible = False
        End If
        If grid.Columns.Contains("DocumentLineTotal") Then
            grid.Columns("DocumentLineTotal").Visible = False
        End If
        If grid.Columns.Contains("QueueProductID") Then
            grid.Columns("QueueProductID").Visible = False
        End If
        If grid.Columns.Contains("DocumentUnitID") Then
            grid.Columns("DocumentUnitID").Visible = False
        End If
        If grid.Columns.Contains("OperationTypeID") Then
            grid.Columns("OperationTypeID").Visible = False
        End If
        If grid.Columns.Contains("SourceDocumentDetailID") Then
            grid.Columns("SourceDocumentDetailID").Visible = False
        End If
        If grid.Columns.Contains("DetailID") Then
            grid.Columns("DetailID").Visible = False
        End If


        If grid.Columns.Contains("TransactionID") Then
            grid.Columns("TransactionID").Visible = False
        End If
        If grid.Columns.Contains("LedgerSequence") Then
            grid.Columns("LedgerSequence").Visible = False
        End If
        If grid.Columns.Contains("OperationGroupID") Then
            grid.Columns("OperationGroupID").Visible = False
        End If
        If grid.Columns.Contains("GroupSeq") Then
            grid.Columns("GroupSeq").Visible = False
        End If
        If grid.Columns.Contains("DocumentDetailID") Then
            grid.Columns("DocumentDetailID").Visible = False
        End If

        ' 👉 أضف هنا ما تريد إخفاءه بنفس الطريقة


        ' =========================
        ' 2) ترتيب الأعمدة (أمثلة)
        ' =========================

        Dim order As New Dictionary(Of String, Integer) From {
        {"TransactionID", 1},
        {"TransactionDetailID", 2},
        {"DocumentID", 3},
        {"DocumentDetailID", 4},
        {"ProductCode", 5},
        {"OldQuantity", 6},
        {"NewQuantity", 7},
        {"OldCost", 8},
        {"NewUnitCost", 9},
        {"UnitName", 10},
        {"ChangeType", 11},
        {"StatusName", 12}
    }

        For Each kvp In order
            If grid.Columns.Contains(kvp.Key) Then
                grid.Columns(kvp.Key).DisplayIndex = kvp.Value
            End If
        Next


        ' =========================
        ' 3) (اختياري) تغيير أسماء الأعمدة
        ' =========================

        If grid.Columns.Contains("NewQuantity") Then
            grid.Columns("NewQuantity").HeaderText = "الكمية الجديدة"
        End If

        If grid.Columns.Contains("NewUnitCost") Then
            grid.Columns("NewUnitCost").HeaderText = "التكلفة الجديدة"
        End If

        If grid.Columns.Contains("StatusName") Then
            grid.Columns("StatusName").HeaderText = "الحالة"
        End If
        If grid.Columns.Contains("ChangeType") Then
            grid.Columns("ChangeType").HeaderText = "نوع التعديل"
        End If
        If grid.Columns.Contains("CreatedAt") Then
            grid.Columns("CreatedAt").HeaderText = "تاريخ الانشاء"
        End If
        If grid.Columns.Contains("ScopeCode") Then
            grid.Columns("ScopeCode").HeaderText = "العملية"
        End If
        If grid.Columns.Contains("ProductCode") Then
            grid.Columns("ProductCode").HeaderText = "كودالصنف"
        End If
        If grid.Columns.Contains("UnitName") Then
            grid.Columns("UnitName").HeaderText = "الوحدة"
        End If
        If grid.Columns.Contains("DocumentID") Then
            grid.Columns("DocumentID").HeaderText = "رقم الفاتورة"
        End If
        If grid.Columns.Contains("DocumentDetailID") Then
            grid.Columns("DocumentDetailID").HeaderText = "رقم تفصيل الفاتورة"
        End If
        If grid.Columns.Contains("ErrorMessage") Then
            grid.Columns("ErrorMessage").HeaderText = "رسالة الخطاء"
        End If
        If grid.Columns.Contains("OldQuantity") Then
            grid.Columns("OldQuantity").HeaderText = "الكمية القديمة"
        End If
        If grid.Columns.Contains("OldCost") Then
            grid.Columns("OldCost").HeaderText = "التكلفة القديمة"
        End If
        If grid.Columns.Contains("TransactionID") Then
            grid.Columns("TransactionID").HeaderText = "رقم الترحيل  "
        End If
        If grid.Columns.Contains("TransactionDetailID") Then
            grid.Columns("TransactionDetailID").HeaderText = " رقم تفصيل الترحيل "
        End If


        ' =========================
        ' 4) تنسيق الأرقام
        ' =========================

        For Each col As DataGridViewColumn In grid.Columns
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next

    End Sub

    Private Sub LoadCorrectionQueue()

        Try

            Dim dt = coscorrectionengin.GetCorrectionQueue()

            dgvCorrectionQueue.AutoGenerateColumns = True
            dgvCorrectionQueue.DataSource = dt
            '          HideEmptyColumns(dgvCorrectionQueue)
            FormatCorrectionQuiueGrid(dgvCorrectionQueue)
            dgvCorrectionQueue.ReadOnly = True
            dgvCorrectionQueue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvCorrectionQueue.AllowUserToAddRows = False
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

        If row.IsNewRow Then Exit Sub

        ' =========================
        ' 1) قراءة TransactionDetailID بأمان
        ' =========================
        Dim detailID As Integer = 0

        If Not IsDBNull(row.Cells("TransactionDetailID").Value) Then
            detailID = CInt(row.Cells("TransactionDetailID").Value)
        End If

        If detailID <= 0 Then
            dgvAffectedOperations.DataSource = Nothing
            Exit Sub
        End If

        ' =========================
        ' 2) جلب اللدجرات المتأثرة
        ' =========================
        Dim dt = coscorrectionengin.GetAffectedCostDependencies(
            New List(Of Integer) From {detailID}
        )

        ' =========================
        ' 3) تعبئة الجريد
        ' =========================
        FillAffectedOperations(dt)

    End Sub

    Private Function GetQueueDetailIDs() As List(Of Integer)

        Dim list As New List(Of Integer)

        For Each row As DataGridViewRow In dgvCorrectionQueue.Rows

            If row.IsNewRow Then Continue For

            If IsDBNull(row.Cells("TransactionDetailID").Value) Then Continue For

            list.Add(CInt(row.Cells("TransactionDetailID").Value))

        Next

        Return list

    End Function
    Private Sub frmCostCorrection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        coscorrectionengin = New CostCorrectionEngine(ConnStr)

        InitializeAffectedOperationsGrid() ' 🔥 مهم جدًا

        LoadCorrectionQueue()
        LoadRebuildGraph()
    End Sub

    Private Sub LoadRebuildGraph()

        Dim detailIDs = GetQueueDetailIDs()

        Dim dt = coscorrectionengin.GetUnifiedAffectedGraph(detailIDs)

        dgvRebuildGraph.AutoGenerateColumns = True
        dgvRebuildGraph.DataSource = dt

        ' 👇 هذا صحيح مكانه
        FormatGridDecimals(dgvRebuildGraph)

    End Sub
    Private Sub FormatGridDecimals(dgv As DataGridView)

        Dim dt As DataTable = TryCast(dgv.DataSource, DataTable)
        If dt Is Nothing Then Exit Sub

        For Each col As DataGridViewColumn In dgv.Columns

            Dim t = dt.Columns(col.Name).DataType

            If t Is GetType(Decimal) _
        OrElse t Is GetType(Double) _
        OrElse t Is GetType(Single) Then

                col.DefaultCellStyle.Format = "N2"

            End If

        Next

    End Sub
    Private Sub btnPreveiw_Click(sender As Object, e As EventArgs) Handles btnPreveiw.Click

        Dim detailIDs = GetQueueDetailIDs()

        Dim startLedgerIds = GetStartLedgerIdsFromQueue()

        Dim dtSim = coscorrectionengin.SimulateByStartLedgerIds(startLedgerIds)

        dgvSimulation.AutoGenerateColumns = True
        dgvSimulation.DataSource = dtSim

        FormatGridDecimals(dgvSimulation)
        FormatSimulationGrid(dgvSimulation)

    End Sub

    Private Function GetStartLedgerIdsFromQueue() As List(Of Long)

        Dim list As New List(Of Long)

        For Each row As DataGridViewRow In dgvCorrectionQueue.Rows

            If row.IsNewRow Then Continue For

            If IsDBNull(row.Cells("StartLedgerID").Value) Then Continue For

            list.Add(CLng(row.Cells("StartLedgerID").Value))

        Next

        Return list

    End Function
    Private Sub FormatSimulationGrid(grid As DataGridView)

        ' =========================
        ' 1) إخفاء أعمدة (أمثلة)
        ' =========================



        If grid.Columns.Contains("TransactionID") Then
            grid.Columns("TransactionID").Visible = False
        End If
        If grid.Columns.Contains("LedgerSequence") Then
            grid.Columns("LedgerSequence").Visible = False
        End If
        If grid.Columns.Contains("OperationGroupID") Then
            grid.Columns("OperationGroupID").Visible = False
        End If
        If grid.Columns.Contains("GroupSeq") Then
            grid.Columns("GroupSeq").Visible = False
        End If
        If grid.Columns.Contains("DocumentDetailID") Then
            grid.Columns("DocumentDetailID").Visible = False
        End If
        If grid.Columns.Contains("PostingDate") Then
            grid.Columns("PostingDate").Visible = False
        End If

        ' 👉 أضف هنا ما تريد إخفاءه بنفس الطريقة


        ' =========================
        ' 2) ترتيب الأعمدة (أمثلة)
        ' =========================

        Dim order As New Dictionary(Of String, Integer) From {
        {"LedgerID", 1},
        {"ProductID", 2},
        {"StoreID", 3},
        {"LocalOldQty", 4},
        {"OldQty", 5},
        {"InQty", 6},
        {"OutQty", 7},
        {"NewQty", 8},
        {"LocalNewQty", 9},
        {"OldAvgCost", 10},
        {"InUnitCost", 11},
        {"OutUnitCost", 12},
        {"NewAvgCost", 13},
        {"PrevLedgerID", 14}
    }

        For Each kvp In order
            If grid.Columns.Contains(kvp.Key) Then
                grid.Columns(kvp.Key).DisplayIndex = kvp.Value
            End If
        Next


        ' =========================
        ' 3) (اختياري) تغيير أسماء الأعمدة
        ' =========================

        If grid.Columns.Contains("NewQuantity") Then
            grid.Columns("NewQuantity").HeaderText = "الكمية الجديدة"
        End If

        If grid.Columns.Contains("NewUnitCost") Then
            grid.Columns("NewUnitCost").HeaderText = "التكلفة الجديدة"
        End If

        If grid.Columns.Contains("StatusName") Then
            grid.Columns("StatusName").HeaderText = "الحالة"
        End If
        If grid.Columns.Contains("ChangeType") Then
            grid.Columns("ChangeType").HeaderText = "نوع التعديل"
        End If
        If grid.Columns.Contains("CreatedAt") Then
            grid.Columns("CreatedAt").HeaderText = "تاريخ الانشاء"
        End If
        If grid.Columns.Contains("ScopeCode") Then
            grid.Columns("ScopeCode").HeaderText = "العملية"
        End If
        If grid.Columns.Contains("ProductCode") Then
            grid.Columns("ProductCode").HeaderText = "كودالصنف"
        End If
        If grid.Columns.Contains("UnitName") Then
            grid.Columns("UnitName").HeaderText = "الوحدة"
        End If
        If grid.Columns.Contains("DocumentID") Then
            grid.Columns("DocumentID").HeaderText = "رقم الفاتورة"
        End If
        If grid.Columns.Contains("DocumentDetailID") Then
            grid.Columns("DocumentDetailID").HeaderText = "رقم تفصيل الفاتورة"
        End If
        If grid.Columns.Contains("ErrorMessage") Then
            grid.Columns("ErrorMessage").HeaderText = "رسالة الخطاء"
        End If
        If grid.Columns.Contains("OldQuantity") Then
            grid.Columns("OldQuantity").HeaderText = "الكمية القديمة"
        End If
        If grid.Columns.Contains("OldCost") Then
            grid.Columns("OldCost").HeaderText = "التكلفة القديمة"
        End If
        If grid.Columns.Contains("TransactionID") Then
            grid.Columns("TransactionID").HeaderText = "رقم الترحيل  "
        End If
        If grid.Columns.Contains("TransactionDetailID") Then
            grid.Columns("TransactionDetailID").HeaderText = " رقم تفصيل الترحيل "
        End If


        ' =========================
        ' 4) تنسيق الأرقام
        ' =========================

        For Each col As DataGridViewColumn In grid.Columns
            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N2"
            End If
        Next

    End Sub

    Private Sub btnExcute_Click(sender As Object, e As EventArgs) Handles btnExcute.Click

        Try
            ' =========================
            ' 1) جمع StartLedgerIDs من الجريد
            ' =========================
            Dim startLedgerIds = GetStartLedgerIdsFromQueue()

            If startLedgerIds Is Nothing OrElse startLedgerIds.Count = 0 Then
                MessageBox.Show("لا يوجد بيانات للتنفيذ", "تنبيه")
                Exit Sub
            End If

            ' =========================
            ' 2) تأكيد المستخدم
            ' =========================
            Dim result = MessageBox.Show(
                "سيتم تنفيذ التصحيح على البيانات الفعلية، هل أنت متأكد؟",
                "تأكيد",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            )

            If result <> DialogResult.Yes Then Exit Sub

            ' =========================
            ' 3) تنفيذ المحرك
            ' =========================
            Me.Cursor = Cursors.WaitCursor

            coscorrectionengin.ExecuteFullCorrection(startLedgerIds, CurrentUser.EmployeeID)

            Me.Cursor = Cursors.Default

            ' =========================
            ' 4) إشعار النجاح
            ' =========================
            MessageBox.Show("تم تنفيذ التصحيح بنجاح", "نجاح")

            ' =========================
            ' 5) إعادة تحميل البيانات
            ' =========================
            LoadCorrectionQueue()
            LoadRebuildGraph()


        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub

    Private Sub btnShowTransactionCorrection_Click(sender As Object, e As EventArgs) Handles btnShowTransactionCorrection.Click

        Try
            ' =========================
            ' 1) جمع StartLedgerIDs
            ' =========================
            Dim startLedgerIds = GetStartLedgerIdsFromQueue()

            If startLedgerIds Is Nothing OrElse startLedgerIds.Count = 0 Then
                MessageBox.Show("لا يوجد بيانات", "تنبيه")
                Exit Sub
            End If

            ' =========================
            ' 2) تشغيل Preview فقط (بدون أي تعديل)
            ' =========================
            Me.Cursor = Cursors.WaitCursor

            Dim dtPreview As DataTable =
                coscorrectionengin.GetTransactionCorrectionPreview(startLedgerIds)

            Me.Cursor = Cursors.Default

            ' =========================
            ' 3) عرض الجريد
            ' =========================
            dgvTransactionPreview.DataSource = dtPreview

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show(ex.Message, "خطأ")
        End Try

    End Sub
End Class

