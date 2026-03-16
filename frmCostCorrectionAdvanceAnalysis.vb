Imports System.Security.Cryptography

Public Class frmCostCorrectionAdvanceAnalysis
    Inherits AABaseOperationForm
    Public Sub LoadSimulation(sim As DataTable, links As DataTable)

        FillLedgerAnalysis(sim)

        FillLinksAnalysis(links)

        FillFlowRatios(links)

    End Sub
    Private Function D(val As Object) As Decimal
        If IsDBNull(val) OrElse val Is Nothing Then
            Return 0D
        End If
        Return CDec(val)
    End Function

    Private Sub InitializeAdvancedAnalysisGrids()

        InitializeSimulationLedgerGrid()

        '=============================
        ' Simulation Links Grid
        '=============================

        Dim dtLinks As New DataTable()

        dtLinks.Columns.Add("LinkID", GetType(Long))
        dtLinks.Columns.Add("SourceLedgerID", GetType(Long))
        dtLinks.Columns.Add("TargetLedgerID", GetType(Long))
        dtLinks.Columns.Add("LinkType", GetType(Integer))

        dtLinks.Columns.Add("OriginalQty", GetType(Decimal))
        dtLinks.Columns.Add("SimQty", GetType(Decimal))

        dtLinks.Columns.Add("OriginalUnitCost", GetType(Decimal))
        dtLinks.Columns.Add("SimUnitCost", GetType(Decimal))

        dtLinks.Columns.Add("SimTotalCost", GetType(Decimal))

        dgvSimulationLinks.DataSource = dtLinks


        '=============================
        ' Flow Ratios Grid
        '=============================

        Dim dtRatios As New DataTable()

        dtRatios.Columns.Add("TargetLedgerID", GetType(Long))
        dtRatios.Columns.Add("SourceLedgerID", GetType(Long))
        dtRatios.Columns.Add("Qty", GetType(Decimal))
        dtRatios.Columns.Add("Ratio", GetType(Decimal))

        dgvFlowRatios.DataSource = dtRatios


        '=============================
        ' Warning Grid
        '=============================

        Dim dtWarn As New DataTable()

        dtWarn.Columns.Add("LedgerID", GetType(Long))
        dtWarn.Columns.Add("Issue", GetType(String))

        dgvFlowWarnings.DataSource = dtWarn

    End Sub
    Private Sub InitializeSimulationLedgerGrid()

        Dim dt As New DataTable()

        dt.Columns.Add("LedgerID", GetType(Long))
        dt.Columns.Add("TransactionID", GetType(Integer))
        dt.Columns.Add("ProductID", GetType(Integer))
        dt.Columns.Add("OperationTypeID", GetType(Integer))
        dt.Columns.Add("PostingDate", GetType(Date))

        dt.Columns.Add("OldQty", GetType(Decimal))
        dt.Columns.Add("NewQty", GetType(Decimal))

        dt.Columns.Add("OldAvgCost", GetType(Decimal))
        dt.Columns.Add("NewAvgCost", GetType(Decimal))

        'Simulation columns
        dt.Columns.Add("SimQty", GetType(Decimal))
        dt.Columns.Add("SimAvgCost", GetType(Decimal))

        dgvSimulationLedger.DataSource = dt

        dgvSimulationLedger.ReadOnly = True
        dgvSimulationLedger.AllowUserToAddRows = False
        dgvSimulationLedger.AllowUserToDeleteRows = False

        For Each col As DataGridViewColumn In dgvSimulationLedger.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

            If col.ValueType Is GetType(Decimal) Then
                col.DefaultCellStyle.Format = "N4"
            End If
        Next

    End Sub

    Private Sub FillSimulationLedgerGrid(sim As DataTable)

        Dim dt As DataTable =
    CType(dgvSimulationLedger.DataSource, DataTable)

        dt.Rows.Clear()

        For Each r As DataRow In sim.Rows

            Dim row = dt.NewRow()

            row("LedgerID") = r("LedgerID")
            row("TransactionID") = r("TransactionID")
            row("ProductID") = r("ProductID")
            row("OperationTypeID") = r("OperationTypeID")
            row("PostingDate") = r("PostingDate")

            row("OldQty") = r("OldQty")
            row("NewQty") = r("NewQty")

            row("OldAvgCost") = r("OldAvgCost")
            row("NewAvgCost") = r("NewAvgCost")

            row("SimQty") = r("SimQty")
            row("SimAvgCost") = r("SimAvgCost")

            dt.Rows.Add(row)

        Next

    End Sub


    Private Sub FillLedgerAnalysis(sim As DataTable)
        Dim dt As DataTable = TryCast(dgvSimulationLedger.DataSource, DataTable)

        If dt Is Nothing Then
            dt = CreateLedgerAnalysisTable()
            dgvSimulationLedger.DataSource = dt
        End If

        dt.Rows.Clear()


    End Sub

    Private Function CreateLedgerAnalysisTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("رقم القيد", GetType(Long))
        dt.Columns.Add("المنتج", GetType(Integer))
        dt.Columns.Add("المستودع", GetType(Integer))

        dt.Columns.Add("الكمية القديمة", GetType(Decimal))
        dt.Columns.Add("الكمية بعد المحاكاة", GetType(Decimal))
        dt.Columns.Add("فرق الكمية", GetType(Decimal))

        dt.Columns.Add("الكمية المحلية القديمة", GetType(Decimal))
        dt.Columns.Add("الكمية المحلية بعد المحاكاة", GetType(Decimal))
        dt.Columns.Add("فرق الكمية المحلية", GetType(Decimal))

        dt.Columns.Add("متوسط التكلفة القديم", GetType(Decimal))
        dt.Columns.Add("متوسط التكلفة بعد المحاكاة", GetType(Decimal))
        dt.Columns.Add("فرق المتوسط", GetType(Decimal))

        dt.Columns.Add("كمية الدخول", GetType(Decimal))
        dt.Columns.Add("كمية الخروج", GetType(Decimal))

        dt.Columns.Add("تكلفة الدخول", GetType(Decimal))
        dt.Columns.Add("تكلفة الخروج", GetType(Decimal))

        dt.Columns.Add("إجمالي الدخول بعد المحاكاة", GetType(Decimal))
        dt.Columns.Add("إجمالي الخروج بعد المحاكاة", GetType(Decimal))

        Return dt

    End Function

    Private Sub FillLinksAnalysis(links As DataTable)

        Dim dt As DataTable = TryCast(dgvSimulationLinks.DataSource, DataTable)

        If dt Is Nothing Then
            dt = CreateLinksAnalysisTable()
            dgvSimulationLinks.DataSource = dt
        End If

        dt.Rows.Clear()

        For Each r As DataRow In links.Rows

            Dim row = dt.NewRow()

            row("رقم الرابط") = r("LinkID")
            row("قيد المصدر") = r("SourceLedgerID")
            row("قيد الهدف") = r("TargetLedgerID")

            row("نوع الرابط") = r("LinkType")

            row("الكمية الأصلية") = r("FlowQty")
            row("الكمية بعد المحاكاة") = r("FlowQty")

            row("تكلفة الوحدة الأصلية") = r("FlowUnitCost")
            row("تكلفة الوحدة بعد المحاكاة") = r("FlowUnitCost")

            row("إجمالي التكلفة بعد المحاكاة") =
                CDec(r("FlowQty")) * CDec(r("FlowUnitCost"))

            dt.Rows.Add(row)

        Next

    End Sub

    Private Function CreateLinksAnalysisTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("رقم الرابط", GetType(Long))
        dt.Columns.Add("قيد المصدر", GetType(Long))
        dt.Columns.Add("قيد الهدف", GetType(Long))

        dt.Columns.Add("نوع الرابط", GetType(String))

        dt.Columns.Add("الكمية الأصلية", GetType(Decimal))
        dt.Columns.Add("الكمية بعد المحاكاة", GetType(Decimal))

        dt.Columns.Add("تكلفة الوحدة الأصلية", GetType(Decimal))
        dt.Columns.Add("تكلفة الوحدة بعد المحاكاة", GetType(Decimal))

        dt.Columns.Add("إجمالي التكلفة بعد المحاكاة", GetType(Decimal))

        Return dt

    End Function
    Private Sub FillFlowRatios(links As DataTable)

        Dim dt As DataTable = TryCast(dgvFlowRatios.DataSource, DataTable)

        If dt Is Nothing Then
            dt = CreateFlowRatioTable()
            dgvFlowRatios.DataSource = dt
        End If

        dt.Rows.Clear()

        Dim groups =
        links.AsEnumerable().
        GroupBy(Function(r) CLng(r("TargetLedgerID")))

        For Each g In groups

            Dim total As Decimal =
            g.Sum(Function(r) D(r("FlowQty")))

            For Each r In g

                Dim row = dt.NewRow()

                Dim qty As Decimal = D(r("FlowQty"))
                Dim ratio As Decimal = If(total = 0, 0, qty / total)

                row("القيد الهدف") = r("TargetLedgerID")
                row("قيد المصدر") = r("SourceLedgerID")

                row("الكمية") = qty
                row("النسبة") = ratio

                dt.Rows.Add(row)

            Next

        Next

    End Sub

    Private Function CreateFlowRatioTable() As DataTable

        Dim dt As New DataTable()

        dt.Columns.Add("القيد الهدف", GetType(Long))
        dt.Columns.Add("قيد المصدر", GetType(Long))
        dt.Columns.Add("الكمية", GetType(Decimal))
        dt.Columns.Add("النسبة", GetType(Decimal))

        Return dt

    End Function

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()

    End Sub
End Class