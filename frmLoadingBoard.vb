Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Drawing2D
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Messaging
Imports System.Security.Cryptography

Public Class frmLoadingBoard
    Inherits AABaseOperationForm
    Public IsOpenedFromInvoice As Boolean = False
    Public Property ParentInvoice As frmInvoice
    Public SelectedLOID As Integer = 0
    Public SelectedSRID As Integer = 0
    ' داخل كلاس frmLoadingBoard
    Public Property SelectedLOModifiedAt As DateTime?
    Public Property IsNewLO As Boolean = False
    Private CurrentSelectedStoreID As Integer = 0

    '    Private IsLoading As Boolean = False
    Private IsDirty As Boolean = False

    Public Property FocusLOID As Integer
    Public Property PendingSRID As Integer = 0
    Private _isGridUpdating As Boolean = False   ' لمنع CellValueChanged/EndEdit أثناء التحديث البرمجي
    Private _isSavingGrid As Boolean = False
    Private CurrentLOID As Integer = 0
    Private CurrentSRID As Integer = 0
    Private service As LoadingApplicationService
    Private isPostedEditMode As Boolean = False
    Private OriginalOutputTable As DataTable = Nothing
    ' =========================
    ' Draft Loading Order State
    ' =========================
    Private IsSaved As Boolean = False

    '    Public Enum LoadingBoardMode
    '       Normal
    '      InvoiceSelection
    ' End Enum

    Public Enum LoadingBoardMode
        Normal = 0
        InvoiceSelection = 1
        ViewOnly = 2
        GoodsIssueSelection = 4
    End Enum
    Public Property CurrentMode As LoadingBoardMode = LoadingBoardMode.Normal
    Private Sub ApplyEditPolicyByLoadingStatus(loID As Integer)

        If loID <= 0 Then Exit Sub
        If IsLoading Then Exit Sub

        ' =========================================
        ' 1) ViewOnly Mode
        ' =========================================
        If CurrentMode = LoadingBoardMode.ViewOnly Then
            MessageBox.Show("viewonly")
            dgvLOs.ReadOnly = True
            dgvLoadingSR.ReadOnly = True
            dgvLoadingSRD.ReadOnly = True

            btnSaveLO.Enabled = False
            btnSendLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnExportToInvoice.Enabled = False

            Exit Sub
        End If

        ' =========================================
        ' 2) Invoice / GoodsIssue Selection Mode
        ' =========================================
        If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
       CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
            MessageBox.Show("selection")

            dgvLOs.ReadOnly = True
            dgvLoadingSRD.ReadOnly = True

            dgvLoadingSR.ReadOnly = False
            For Each col As DataGridViewColumn In dgvLoadingSR.Columns
                col.ReadOnly = (col.Name <> "colLoadingSRtoInvoice")
            Next
            dgvLoadingSR.EditMode = DataGridViewEditMode.EditOnEnter

            btnSaveLO.Enabled = False
            btnSendLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnExportToInvoice.Enabled = True

            Exit Sub
        End If

        ' =========================================
        ' 3) Normal Mode فقط
        ' =========================================
        Dim statusID As Integer = GetLoadingStatusID(loID)

        Dim fullEdit As Boolean = (statusID = 0 OrElse statusID = 1 OrElse statusID = 2 OrElse statusID = 5 OrElse statusID = 14)
        Dim postedEdit As Boolean = (statusID = 15) AndAlso isPostedEditMode = True
        Dim headerOnly As Boolean = (statusID = 15)
        ' =========================================
        ' 3.0) posted Edit  

        ' =========================================

        If postedEdit Then
            dgvLoadingSR.ReadOnly = True
            dgvLoadingSRD.ReadOnly = True

            dgvLOs.ReadOnly = False
            For Each c As DataGridViewColumn In dgvLOs.Columns
                c.ReadOnly = True
            Next

            dgvLOs.Columns("colLOsDriverCode").ReadOnly = False
            dgvLOs.Columns("colLOsSupervisor").ReadOnly = False
            dgvLOs.Columns("colLOsVehicale").ReadOnly = False
            dgvLOs.Columns("colLOsNote").ReadOnly = False
            dgvLOs.Columns("colLOsNote").ReadOnly = False


            If dgvLOs.Columns.Contains("colLOsStoreID") Then
                dgvLOs.Columns("colLOsStoreID").ReadOnly = True
            End If
            dgvLoadingSRD.ReadOnly = False
            For Each col As DataGridViewColumn In dgvLoadingSRD.Columns
                col.ReadOnly = True
            Next

            dgvLoadingSRD.Columns("colLoadingSRDLoadedQTY").ReadOnly = False
            dgvLoadingSRD.EditMode = DataGridViewEditMode.EditOnEnter

            btnSaveLO.Enabled = True
            btnSendLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnExportToInvoice.Enabled = False

            Exit Sub
        End If


        ' =========================================
        ' 3.1) Full Edit    

        ' =========================================
        If fullEdit Then
            ' Header
            dgvLOs.ReadOnly = False

            ' SR
            dgvLoadingSR.ReadOnly = True

            ' SRD: افتح فقط عمود الكمية
            dgvLoadingSRD.ReadOnly = False
            For Each col As DataGridViewColumn In dgvLoadingSRD.Columns
                col.ReadOnly = True
            Next

            dgvLoadingSRD.Columns("colLoadingSRDLoadedInThisLO").ReadOnly = False
            dgvLoadingSRD.Columns("colLoadingSRDDeleteFromThisLoadingOrder").ReadOnly = False
            dgvLoadingSRD.EditMode = DataGridViewEditMode.EditOnEnter

            btnSaveLO.Enabled = True
            btnSendLoading.Enabled = (statusID <> 5)
            btnAddSelectedSRToLO.Enabled = True
            btnExportToInvoice.Enabled = False

            Exit Sub
        End If

        ' =========================================
        ' 3.2) Header Only
        ' =========================================
        If headerOnly Then

            dgvLoadingSR.ReadOnly = True
            dgvLoadingSRD.ReadOnly = True

            dgvLOs.ReadOnly = False
            For Each c As DataGridViewColumn In dgvLOs.Columns
                c.ReadOnly = True
            Next

            dgvLOs.Columns("colLOsDriverCode").ReadOnly = False
            dgvLOs.Columns("colLOsSupervisor").ReadOnly = False
            dgvLOs.Columns("colLOsVehicale").ReadOnly = False
            dgvLOs.Columns("colLOsNote").ReadOnly = False

            If dgvLOs.Columns.Contains("colLOsStoreID") Then
                dgvLOs.Columns("colLOsStoreID").ReadOnly = True
            End If

            btnSaveLO.Enabled = True
            btnSendLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnExportToInvoice.Enabled = False

            Exit Sub
        End If


        ' =========================================
        ' 3.3) No Edit
        ' =========================================
        dgvLOs.ReadOnly = True
        dgvLoadingSR.ReadOnly = True
        dgvLoadingSRD.ReadOnly = True

        btnSaveLO.Enabled = False
        btnSendLoading.Enabled = False
        btnAddSelectedSRToLO.Enabled = False
        btnExportToInvoice.Enabled = False

    End Sub
    Private Sub ApplyModeUI()

        ' Visible by mode
        btnCloseBoard.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection)
        btnAddSelectedSRToLO.Visible = (CurrentMode = LoadingBoardMode.Normal)
        btnSaveLO.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection)
        btnSendLoading.Visible = (CurrentMode = LoadingBoardMode.Normal)
        btnExportToInvoice.Visible = (CurrentMode <> LoadingBoardMode.Normal)
        btnSearch.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection OrElse CurrentMode = LoadingBoardMode.GoodsIssueSelection)
        btnCancel.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection OrElse CurrentMode = LoadingBoardMode.GoodsIssueSelection)
        btnPrint.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection OrElse CurrentMode = LoadingBoardMode.GoodsIssueSelection)

        If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
   CurrentMode = LoadingBoardMode.GoodsIssueSelection Then

            colLoadingSRtoInvoice.Visible = True
            colLoadingSRDCodes.Visible = True

            colLoadingSRDLoadedBefore.Visible = False
            colLoadingSRDLoadedQTY.Visible = False
            colLoadingSRDRemainingQTY.Visible = False
            colLoadingSRDAvailableQTY.Visible = False
            colLoadingSRDFulfillmentStatusName.Visible = False
            colLoadingSRDBusinessStatusName.Visible = False
            colOpenLOsVehicleInfo.Visible = False
            colOpenLOsSupervisor.Visible = False

            ' فقط: اجعل أعمدة SR ReadOnly باستثناء التشك
            For Each col As DataGridViewColumn In dgvLoadingSR.Columns
                col.ReadOnly = (col.Name <> "colLoadingSRtoInvoice")
            Next

            Exit Sub
        End If

        ' Normal / ViewOnly: إظهار أعمدة التحميل
        colLoadingSRtoInvoice.Visible = False

        colLoadingSRDLoadedBefore.Visible = True
        colLoadingSRDLoadedQTY.Visible = True
        colLoadingSRDRemainingQTY.Visible = True
        colLoadingSRDAvailableQTY.Visible = True
        colLoadingSRDFulfillmentStatusName.Visible = False
        colLoadingSRDBusinessStatusName.Visible = True
        colLoadingSRID.Visible = True
        colLoadingSRDLoadedInThisLO.Visible = True
        colLoadingSRDCodes.Visible = True

        ' Formats/Widths (كما عندك)
        dgvLOs.Columns("colLOsNote").Width = 100

        With dgvLoadingSRD
            .Columns("colLoadingSRDDeleteFromThisLoadingOrder").Width = 50

            .Columns("colLoadingSRDBusinessStatusName").Width = 100
            .Columns("colLoadingSRDFulfillmentStatusName").Width = 100
            .Columns("colLoadingSRDLoadedInThisLO").Width = 50
            .Columns("colLoadingSRDCodes").Width = 100
            .Columns("colLoadingSRDQTY").Width = 80
            .Columns("colLoadingSRDAvailableQTY").Width = 80
            .Columns("colLoadingSRDLoadedBefore").Width = 80
            .Columns("colLoadingSRDRemainingQTY").Width = 80
            .Columns("colLoadingSRDLoadedQTY").Width = 80

            .Columns("colLoadingSRDQTY").DefaultCellStyle.Format = "N1"
            .Columns("colLoadingSRDAvailableQTY").DefaultCellStyle.Format = "N1"
            .Columns("colLoadingSRDLoadedBefore").DefaultCellStyle.Format = "N1"
            .Columns("colLoadingSRDRemainingQTY").DefaultCellStyle.Format = "N1"
            .Columns("colLoadingSRDLoadedQTY").DefaultCellStyle.Format = "N1"
        End With

    End Sub
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub New(srID As Integer)
        InitializeComponent()
        CurrentSRID = srID
    End Sub
    Private Sub LoadLoadingGridCombos()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =========================
            ' السائق
            ' =========================
            Using da As New SqlDataAdapter("
            SELECT EmployeeID, EmpName
            FROM sec.Employee
            WHERE IsActive = 1
            ORDER BY EmpName
        ", con)

                Dim dt As New DataTable()
                da.Fill(dt)

                With CType(dgvLOs.Columns("colLOsDriverCode"), DataGridViewComboBoxColumn)
                    .DataSource = dt
                    .DisplayMember = "EmpName"
                    .ValueMember = "EmployeeID"   ' ✅ ID
                    .DefaultCellStyle.NullValue = Nothing
                End With
            End Using

            ' =========================
            ' المشرف
            ' =========================
            Using da As New SqlDataAdapter("
            SELECT EmployeeID, EmpName
            FROM sec.Employee
            WHERE IsActive = 1
            ORDER BY EmpName
        ", con)

                Dim dt As New DataTable()
                da.Fill(dt)

                With CType(dgvLOs.Columns("colLOsSupervisor"), DataGridViewComboBoxColumn)
                    .DataSource = dt
                    .DisplayMember = "EmpName"
                    .ValueMember = "EmployeeID"   ' ✅ ID
                    .DefaultCellStyle.NullValue = Nothing
                End With
            End Using

            ' =========================
            ' المخزن (SourceStoreID)
            ' =========================
            Using da As New SqlDataAdapter("
            SELECT StoreID AS StoreID, StoreName
            FROM md.Store
            WHERE IsActive = 1
            ORDER BY StoreName
        ", con)

                Dim dt As New DataTable()
                da.Fill(dt)

                With CType(dgvLOs.Columns("colLOsStoreID"), DataGridViewComboBoxColumn)
                    .DataSource = dt
                    .DisplayMember = "StoreName"
                    .ValueMember = "StoreID"   ' ✅ ID
                    .DefaultCellStyle.NullValue = Nothing
                End With
            End Using

            ' =========================
            ' السيارة
            ' =========================
            Using da As New SqlDataAdapter("
            SELECT VehicleID, VehicleCode
            FROM md.Vehicle
            ORDER BY VehicleCode
        ", con)

                Dim dt As New DataTable()
                da.Fill(dt)

                With CType(dgvLOs.Columns("colLOsVehicale"), DataGridViewComboBoxColumn)
                    .DataSource = dt
                    .DisplayMember = "VehicleCode"
                    .ValueMember = "VehicleID"   ' ✅ ID
                    .DefaultCellStyle.NullValue = Nothing
                End With
            End Using

        End Using

    End Sub
    Private Sub dgvLOs_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvLOs.CellClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim grid = dgvLOs
        Dim col = grid.Columns(e.ColumnIndex)

        ' فقط أعمدة الكمبو
        If TypeOf col Is DataGridViewComboBoxColumn Then

            grid.CurrentCell = grid.Rows(e.RowIndex).Cells(e.ColumnIndex)

            ' ادخل وضع التعديل مباشرة
            grid.BeginEdit(True)

            ' افتح الكمبو فورًا
            Dim ctl = TryCast(grid.EditingControl, ComboBox)
            If ctl IsNot Nothing Then
                ctl.DroppedDown = True
            End If

        End If

    End Sub

    Private Sub LoadOpenedLoadingOrders()

        IsLoading = True
        Try
            dgvOpenedLOs.Rows.Clear()

            Using con As New SqlConnection(ConnStr)
                con.Open()

                Dim statusFilter As String

                If CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
                    statusFilter = "5"

                ElseIf CurrentMode = LoadingBoardMode.InvoiceSelection Then
                    statusFilter = "15,8"

                Else
                    statusFilter = "0,1,2,14"
                End If
                ' ✅ فلترة "متاح للسحب" تختلف فقط في وضع الفاتورة
                Dim sqlAvailabilityFilter As String

                If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
   CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
                    ' متاح للسحب إذا يوجد على الأقل LOD واحد LoadedQty>0 وغير مرتبط بفاتورة SAL غير ملغاة
                    sqlAvailabilityFilter = "
AND EXISTS (
    SELECT 1
    FROM log.LoadingOrderDetail LOD
    WHERE LOD.LOID = LO.LOID
      AND ISNULL(LOD.LoadedQty,0) > 0
      AND NOT EXISTS (
            SELECT 1
            FROM inv.DocumentDetails IDD
            INNER JOIN inv.DocumentHeader H
                ON H.DocumentID = IDD.DocumentID
            WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
              AND H.DocumentType = 'SAL'
              AND H.StatusID <> 10
      )
)"
                Else
                    ' الوضع القديم كما هو (بدون ربط الهيدر)
                    sqlAvailabilityFilter = "
AND EXISTS (
    SELECT 1
    FROM log.LoadingOrderDetail LOD
    WHERE LOD.LOID = LO.LOID
      AND NOT EXISTS (
            SELECT 1
            FROM inv.DocumentDetails IDD
            WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
      )
)"
                End If

                Dim sql As String = "
SELECT
    LO.LOID,
    LO.LOCode,
    LO.InitiatedDateTime,
    LO.LoadingStatusID,
    S.StatusName AS LoadingStatusName,
    V.VehicleCode,
    E.EmpName AS SupervisorName
FROM log.LoadingOrder LO
INNER JOIN wf.Status S
    ON S.StatusID = LO.LoadingStatusID
LEFT JOIN md.Vehicle V
    ON V.VehicleID = LO.VehicleID
LEFT JOIN sec.Employee E
    ON E.EmployeeID = LO.LoadingSupervisorID
WHERE LO.LoadingStatusID IN (" & statusFilter & ")
" & sqlAvailabilityFilter & "
ORDER BY LO.InitiatedDateTime DESC
"

                Using cmd As New SqlCommand(sql, con)
                    Using rd = cmd.ExecuteReader()
                        While rd.Read()
                            Dim r As Integer = dgvOpenedLOs.Rows.Add()

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsID").Value =
                            CInt(rd("LOID"))

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsCode").Value =
                            rd("LOCode").ToString()

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsInitiatedDateTime").Value =
                            CDate(rd("InitiatedDateTime"))

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsStatusID").Value =
                            CInt(rd("LoadingStatusID"))

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsStatus").Value =
                            rd("LoadingStatusName").ToString()

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsVehicleInfo").Value =
                            If(rd("VehicleCode") Is DBNull.Value, "", rd("VehicleCode").ToString())

                            dgvOpenedLOs.Rows(r).Cells("colOpenLOsSupervisor").Value =
                            If(rd("SupervisorName") Is DBNull.Value, "", rd("SupervisorName").ToString())
                        End While
                    End Using
                End Using
            End Using

            dgvOpenedLOs.ClearSelection()
            dgvOpenedLOs.CurrentCell = Nothing
            CurrentLOID = 0

            ' تنظيف واجهة إذا لا يوجد أوامر تحميل متاحة
            If dgvOpenedLOs.Rows.Count = 0 Then
                dgvLoadingSR.Rows.Clear()
                dgvLoadingSRD.Rows.Clear()
            End If

        Finally
            IsLoading = False
        End Try

    End Sub
    Private Sub LoadSelectedSRIntoLoadingBoard(srID As Integer)

        dgvLoadingSR.Rows.Clear()
        dgvLoadingSRD.Rows.Clear()

        If srID <= 0 Then Exit Sub

        ' 🔑 دخول سياق التحميل (SR جديد أو مسودة)

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT
                SR.SRID,
                SR.SRCode,
                P.PartnerName,
                CAST(SR.SRDate AS date) AS SRDateOnly
            FROM inv.SR
            LEFT JOIN md.Partner P ON P.PartnerID = SR.PartnerID
            WHERE SR.SRID = @SRID
        ", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        Dim r As Integer = dgvLoadingSR.Rows.Add()

                        dgvLoadingSR.Rows(r).Cells("colLoadingSRID").Value = CInt(rd("SRID"))
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRCodes").Value = rd("SRCode").ToString()
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRPartners").Value =
                        If(rd("PartnerName") Is DBNull.Value, "", rd("PartnerName").ToString())
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRDates").Value = rd("SRDateOnly")

                    End If
                End Using
            End Using
        End Using


    End Sub
    Private Sub frmLoadingBoard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' -------------------------
        ' Window/layout
        ' -------------------------
        Me.StartPosition = FormStartPosition.Manual
        Dim r As Rectangle = Screen.FromControl(Me).WorkingArea
        Me.Width = CInt(r.Width * 0.95)
        Me.Height = CInt(r.Height * 0.95)
        Me.Left = r.Left + (r.Width - Me.Width) \ 2
        Me.Top = r.Top + (r.Height - Me.Height) \ 2
        service = New LoadingApplicationService(ConnStr)

        IsLoading = True
        Try
            ' -------------------------
            ' Grid basic setup
            ' -------------------------
            dgvLoadingSRD.StandardTab = False
            dgvOpenedLOs.TabStop = False
            dgvLoadingSR.TabStop = False
            dgvLOs.TabStop = False
            dgvLoadingSRD.TabStop = True

            dgvOpenedLOs.MultiSelect = False
            dgvOpenedLOs.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' Lookups/combos
            LoadLoadingGridCombos()

            ' Apply UI for the current mode (Normal / InvoiceSelection / ViewOnly)
            ApplyModeUI()

            ' =========================
            ' 1) Open a specific LO directly (FocusLOID)
            ' =========================
            If FocusLOID > 0 Then
                CurrentLOID = FocusLOID

                ' Load header first (also sets CurrentSelectedStoreID inside LoadLOHeader in your code)
                LoadLOHeader(CurrentLOID)

                ' Ensure CurrentSelectedStoreID is ready for SRD availability calculations
                If dgvLOs.Rows.Count > 0 AndAlso dgvLOs.Rows(0).Cells("colLOsStoreID").Value IsNot DBNull.Value Then
                    CurrentSelectedStoreID = CInt(dgvLOs.Rows(0).Cells("colLOsStoreID").Value)
                Else
                    CurrentSelectedStoreID = 4
                End If

                ' Load related grids
                LoadSRsForLO(CurrentLOID)
                LoadSRDDetailsForLO(CurrentLOID)

                ' Fill dgvOpenedLOs with the focused LO (even if it doesn't match list filters)
                AddFocusLOToOpenedGrid(CurrentLOID)

                ' Apply permissions/edit policy last
                ApplyEditPolicyByLoadingStatus(CurrentLOID)

                Exit Sub
            End If

            ' =========================
            ' 2) Normal start: load opened LOs list
            ' =========================
            LoadOpenedLoadingOrders()

            ' Optional: open SR directly (your existing behavior)
            If CurrentSRID > 0 Then
                LoadSelectedSRIntoLoadingBoard(CurrentSRID)
            End If

            ' Do NOT clear selection here if you want CurrentCellChanged to fire when user clicks.
            ' If you prefer a clean UI on load, keep it:
            dgvOpenedLOs.ClearSelection()
            dgvOpenedLOs.CurrentCell = Nothing

            ' No LO selected yet => leave CurrentLOID = 0; avoid loading SRD here.

        Finally
            IsLoading = False
        End Try

    End Sub
    Private Sub frmLoadingBoard_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' ✅ طبّق سياسة وضع الفاتورة حتى قبل اختيار LO
        If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
   CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
            ApplyInvoiceSelectionPolicyForSRGrid()
        End If
    End Sub
    Private Sub ApplyInvoiceSelectionPolicyForSRGrid()

        ' افتح الجريد حتى يعمل التشيك
        dgvLoadingSR.ReadOnly = False

        ' اقفل كل الأعمدة عدا التشيك
        For Each col As DataGridViewColumn In dgvLoadingSR.Columns
            col.ReadOnly = (col.Name <> "colLoadingSRtoInvoice")
        Next

        dgvLoadingSR.EditMode = DataGridViewEditMode.EditOnEnter

    End Sub

    Private Sub LoadLOHeader(loID As Integer)

        dgvLOs.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT
                LOID,
                LOCode,
                InitiatedDateTime,
                DriverEmployeeID,
                LoadingSupervisorID,
                VehicleID,
                SourceStoreID,
                Notes
            FROM log.LoadingOrder
            WHERE LOID = @LOID
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        Dim row As DataGridViewRow

                        If dgvLOs.Rows.Count = 0 Then
                            Dim r As Integer = dgvLOs.Rows.Add()
                            row = dgvLOs.Rows(r)
                        Else
                            row = dgvLOs.Rows(0)
                        End If
                        ' أعمدة عادية
                        row.Cells("colLOsID").Value =
                        CInt(rd("LOID"))

                        row.Cells("colLOsCode").Value =
                        rd("LOCode").ToString()

                        row.Cells("colLOsDate").Value =
                        CDate(rd("InitiatedDateTime"))

                        ' ComboBox Columns (الإصلاح الحقيقي)
                        SetComboCellValueSafely(
                        row,
                        "colLOsDriverCode",
                        rd("DriverEmployeeID")
                    )

                        SetComboCellValueSafely(
                        row,
                        "colLOsSupervisor",
                        rd("LoadingSupervisorID")
                    )

                        SetComboCellValueSafely(
                        row,
                        "colLOsVehicale",
                        rd("VehicleID")
                    )

                        ' 🔒 فرض المستودع = 4 دائماً
                        SetComboCellValueSafely(
                        row,
                        "colLOsStoreID",
                        4
                    )

                        CurrentSelectedStoreID = 4

                        ' Notes
                        row.Cells("colLOsNote").Value =
                        If(rd("Notes") Is DBNull.Value, "", rd("Notes").ToString())

                    End If
                End Using

            End Using
        End Using

    End Sub

    Private Sub dgvLoadingSRD_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvLoadingSRD.CellBeginEdit

        If IsLoading Then
            e.Cancel = True
            Exit Sub
        End If

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            e.Cancel = True
            Exit Sub
        End If

        If dgvLoadingSRD.Columns(e.ColumnIndex).Name <> "colLoadingSRDLoadedInThisLO" Then Exit Sub

        Dim st As Integer = GetLoadingStatusID(CurrentLOID)
        If Not (st = 2 OrElse st = 5 OrElse st = 14 OrElse isPostedEditMode) Then
            e.Cancel = True
        End If

    End Sub

    Private Sub dgvOpenedLOs_CurrentCellChanged(
    sender As Object,
    e As EventArgs
) Handles dgvOpenedLOs.CurrentCellChanged

        If IsLoading Then Exit Sub

        If dgvOpenedLOs.CurrentRow Is Nothing Then Exit Sub
        If dgvOpenedLOs.CurrentRow.Index < 0 Then Exit Sub

        Dim loID As Integer = 0

        If dgvOpenedLOs.CurrentRow.Cells("colOpenLOsID").Value IsNot Nothing Then
            loID = CInt(dgvOpenedLOs.CurrentRow.Cells("colOpenLOsID").Value)
        End If

        If loID <= 0 Then Exit Sub

        SetCurrentLO(loID)

    End Sub

    Private Sub LoadSRsForLO(loID As Integer)

        dgvLoadingSR.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sqlExtraFilterForLOD As String = ""

            If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
   CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
                sqlExtraFilterForLOD = "
      AND ISNULL(LOD.LoadedQty,0) > 0
      AND NOT EXISTS (
            SELECT 1
            FROM inv.DocumentDetails IDD
            INNER JOIN inv.DocumentHeader H
                ON H.DocumentID = IDD.DocumentID
            WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
              AND H.DocumentType = 'SAL'
              AND H.StatusID <> 10
      )"
            ElseIf CurrentMode = LoadingBoardMode.ViewOnly Then
                ' ✅ اعرض كل SR المرتبطة بالـ LO بدون شروط إضافية
                sqlExtraFilterForLOD = ""
            Else
                sqlExtraFilterForLOD = "
      AND NOT EXISTS (
            SELECT 1
            FROM inv.DocumentDetails IDD
            WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
      )"
            End If

            Dim sql As String = "
SELECT
    SR.SRID,
    SR.SRCode,
    P.PartnerName,
    CAST(SR.SRDate AS date) AS SRDateOnly
FROM inv.SR SR
LEFT JOIN md.Partner P
    ON P.PartnerID = SR.PartnerID
WHERE EXISTS (
    SELECT 1
    FROM log.LoadingOrderDetail LOD
    WHERE LOD.LOID = @LOID
      AND LOD.SourceHeaderID = SR.SRID
" & sqlExtraFilterForLOD & "
)
ORDER BY SR.SRID
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim r As Integer = dgvLoadingSR.Rows.Add()

                        dgvLoadingSR.Rows(r).Cells("colLoadingSRID").Value = CInt(rd("SRID"))
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRCodes").Value = rd("SRCode").ToString()
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRPartners").Value =
                        If(rd("PartnerName") Is DBNull.Value, "", rd("PartnerName").ToString())
                        dgvLoadingSR.Rows(r).Cells("colLoadingSRDates").Value =
                        If(rd("SRDateOnly") Is DBNull.Value, DBNull.Value, rd("SRDateOnly"))
                    End While
                End Using
            End Using
        End Using

        dgvLoadingSR.ClearSelection()
        dgvLoadingSR.CurrentCell = Nothing

        If dgvLoadingSR.Rows.Count = 0 Then
            dgvLoadingSRD.Rows.Clear()
        End If

    End Sub
    Private Sub LoadSRDDetailsForLO(loID As Integer)

        dgvLoadingSRD.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' ✅ التعديل فقط في وضع الفاتورة:
            ' اخفِ LOD إذا كان مرتبط بفاتورة SAL ليست ملغاة (StatusID <> 10)
            ' + احتياط: في وضع الفاتورة فقط، لا نعرض LoadedQty = 0
            Dim sqlNotExistsForThisLOD As String = ""

            If CurrentMode = LoadingBoardMode.InvoiceSelection OrElse
   CurrentMode = LoadingBoardMode.GoodsIssueSelection Then
                sqlNotExistsForThisLOD = "
AND ISNULL(LOD.LoadedQty,0) > 0
AND NOT EXISTS (
    SELECT 1
    FROM inv.DocumentDetails IDD
    INNER JOIN inv.DocumentHeader H
        ON H.DocumentID = IDD.DocumentID
    WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
      AND H.DocumentType = 'SAL'
      AND H.StatusID <> 10
)"
            ElseIf CurrentMode = LoadingBoardMode.ViewOnly Then
                ' ✅ اعرض كل شيء (لا فلترة فواتير)
                sqlNotExistsForThisLOD = ""
            Else
                ' الوضع العادي كما هو
                sqlNotExistsForThisLOD = "
AND NOT EXISTS (
    SELECT 1
    FROM inv.DocumentDetails IDD
    WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
)"
            End If

            Dim sql As String = "
SELECT
    LOD.LoadingOrderDetailID,
    LOD.LOID,
    LOD.SourceHeaderID,
    LOD.SourceDetailID,
    LOD.ProductID,
    ISNULL(LOD.LoadedQty,0) AS LoadedQty,

    SR.SRCode,
    SRD.ProductCode,
    SRD.ProductType,
    ISNULL(SRD.Quantity,0) AS RequiredQty,

    SRD.BusinessStatusID,
    BS.StatusName AS BusinessStatusName,

    ISNULL((
        SELECT SUM(d2.LoadedQty)
        FROM log.LoadingOrderDetail d2
        WHERE d2.SourceDetailID = LOD.SourceDetailID
          AND d2.LOID <> @LOID
    ),0) AS LoadedBefore,

    ISNULL((
        SELECT
            IB.QtyOnHand - ISNULL(SUM(IR.ReservedQty),0)
        FROM inv.Balance IB
        LEFT JOIN inv.Reservation IR
            ON IR.ProductID = IB.ProductID
           AND IR.SourceStoreID = IB.StoreID
           AND IR.ReservationStatusID = 1
        WHERE IB.StoreID = @StoreID
          AND IB.ProductID = LOD.ProductID
        GROUP BY IB.QtyOnHand
    ),0) AS AvailableQty

FROM log.LoadingOrderDetail LOD
INNER JOIN log.LoadingOrder LO
    ON LO.LOID = LOD.LOID
INNER JOIN inv.SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
INNER JOIN inv.SR SR
    ON SR.SRID = LOD.SourceHeaderID
LEFT JOIN wf.Status BS
    ON BS.StatusID = SRD.BusinessStatusID

WHERE LOD.LOID = @LOID
" & sqlNotExistsForThisLOD & "
"

            If IsOpenedFromInvoice AndAlso CurrentSRID > 0 Then
                sql &= " AND LOD.SourceHeaderID = @SRID "
            End If
            sql &= " ORDER BY SR.SRCode, SRD.ProductCode "

            Using cmd As New SqlCommand(sql, con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                If IsOpenedFromInvoice AndAlso CurrentSRID > 0 Then
                    cmd.Parameters.AddWithValue("@SRID", CurrentSRID)
                End If

                cmd.Parameters.AddWithValue(
                "@StoreID",
                If(CurrentSelectedStoreID > 0, CurrentSelectedStoreID, DBNull.Value)
            )

                Using rd = cmd.ExecuteReader()
                    While rd.Read()

                        Dim requiredQty As Decimal = CDec(rd("RequiredQty"))
                        Dim loadedBefore As Decimal = CDec(rd("LoadedBefore"))
                        Dim loadedInThisLO_DB As Decimal = CDec(rd("LoadedQty"))
                        Dim loadedInSession As Decimal = 0D

                        Dim remaining As Decimal =
                        requiredQty -
                        loadedBefore -
                        loadedInThisLO_DB -
                        loadedInSession

                        Dim r As Integer = dgvLoadingSRD.Rows.Add()

                        dgvLoadingSRD.Rows(r).Cells("colLoadingOrderDetailID").Value =
                        CInt(rd("LoadingOrderDetailID"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDCodes").Value =
                        rd("SRCode").ToString()

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDProductCode").Value =
                        rd("ProductCode").ToString()

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDProductType").Value =
                        If(rd("ProductType") Is DBNull.Value, "", rd("ProductType").ToString())

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDSRID").Value =
                        CInt(rd("SourceHeaderID"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDSRDID").Value =
                        CInt(rd("SourceDetailID"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDProductID").Value =
                        CInt(rd("ProductID"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDBusinessStatusID").Value =
                        CInt(rd("BusinessStatusID"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDBusinessStatusName").Value =
                        If(rd("BusinessStatusName") Is DBNull.Value, "", rd("BusinessStatusName").ToString())

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDQTY").Value =
                        requiredQty

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDAvailableQTY").Value =
                        CDec(rd("AvailableQty"))

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDAvailableQTY").Tag =
                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDAvailableQTY").Value

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDLoadedBefore").Value =
                        loadedBefore

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDLoadedQTY").Value =
                        loadedInThisLO_DB

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDLoadedInThisLO").Value =
                        0D

                        dgvLoadingSRD.Rows(r).Cells("colLoadingSRDRemainingQTY").Value =
                        remaining

                    End While
                End Using
            End Using
        End Using

        dgvLoadingSRD.ClearSelection()
        dgvLoadingSRD.CurrentCell = Nothing

    End Sub


    Private Sub btnCloseBoard_Click(
    sender As Object,
    e As EventArgs
) Handles btnCloseBoard.Click


        ' لاحقًا: لو LO محفوظ
        Me.Close()

    End Sub


    Private Sub btnAddSelectedSRToLO_Click(
    sender As Object,
    e As EventArgs
) Handles btnAddSelectedSRToLO.Click

        ' =========================
        ' 1) تحقق أساسي
        ' =========================
        If CurrentLOID = 0 Then
            MessageBox.Show("لم يتم اختيار أمر تحميل", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If PendingSRID = 0 Then
            MessageBox.Show("لا يوجد طلب مبيعات للإضافة", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' =========================
                    ' 2) حماية: منع تكرار ربط نفس الطلب
                    ' =========================
                    Using cmdChk As New SqlCommand("
SELECT COUNT(*)
FROM log.LoadingOrderSR
WHERE LOID = @LOID
  AND SRID = @SRID
", con, tran)

                        cmdChk.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmdChk.Parameters.AddWithValue("@SRID", PendingSRID)

                        If CInt(cmdChk.ExecuteScalar()) > 0 Then
                            MessageBox.Show("الطلب مضاف مسبقاً إلى أمر التحميل", "تنبيه",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                            tran.Rollback()
                            Exit Sub
                        End If
                    End Using

                    ' =========================
                    ' 3) ربط SR مع LO
                    ' =========================
                    Using cmdLOS As New SqlCommand("
INSERT INTO log.LoadingOrderSR (LOID, SRID)
VALUES (@LOID, @SRID)
", con, tran)

                        cmdLOS.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmdLOS.Parameters.AddWithValue("@SRID", PendingSRID)
                        cmdLOS.ExecuteNonQuery()
                    End Using

                    ' =========================
                    ' 4) نسخ تفاصيل الطلب إلى تفاصيل التحميل
                    ' =========================
                    Using cmdLOD As New SqlCommand("
INSERT INTO log.LoadingOrderDetail
(LOID, SourceHeaderID, SourceDetailID, ProductID, LoadedQty,
 Length_cm, Width_cm, Height_cm,
 ProductTypeID, CreatedAt)
SELECT
    @LOID,
    SRD.SRID,
    SRD.SRDID,
    SRD.ProductID,
    0,
    SRD.LengthCM,
    SRD.WidthCM,
    SRD.HeightCM,
    SRD.ProductTypeID,
    GETDATE()
FROM inv.SRD SRD
WHERE SRD.SRID = @SRID
", con, tran)

                        cmdLOD.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmdLOD.Parameters.AddWithValue("@SRID", PendingSRID)
                        cmdLOD.ExecuteNonQuery()
                    End Using

                    ' =========================
                    ' 5) تحديث حالة Fulfillment
                    ' =========================

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            End Using
        End Using
        ' =========================
        ' 6) إعادة تحميل البورد والتركيز
        ' =========================
        Dim targetLOID As Integer = CurrentLOID

        IsLoading = True
        Try
            LoadOpenedLoadingOrders()

            ' إعادة تحديد نفس أمر التحميل
            For Each r As DataGridViewRow In dgvOpenedLOs.Rows
                If r.IsNewRow Then Continue For
                If r.Cells("colOpenLOsID").Value Is Nothing Then Continue For
                If CInt(r.Cells("colOpenLOsID").Value) = targetLOID Then
                    dgvOpenedLOs.ClearSelection()
                    r.Selected = True

                    If dgvOpenedLOs.Columns.Contains("colOpenLOsCode") Then
                        dgvOpenedLOs.CurrentCell = r.Cells("colOpenLOsCode")
                    Else
                        dgvOpenedLOs.CurrentCell = r.Cells(0)
                    End If

                    Exit For
                End If
            Next

            CurrentLOID = targetLOID

            ' حمّل البيانات مباشرة
            LoadLOHeader(CurrentLOID)

            If dgvLOs.Rows.Count > 0 AndAlso
       dgvLOs.Rows(0).Cells("colLOsStoreID").Value IsNot DBNull.Value AndAlso
       dgvLOs.Rows(0).Cells("colLOsStoreID").Value IsNot Nothing Then

                CurrentSelectedStoreID = CInt(dgvLOs.Rows(0).Cells("colLOsStoreID").Value)
            End If

            LoadSRsForLO(CurrentLOID)
            LoadSRDDetailsForLO(CurrentLOID)
            ApplyEditPolicyByLoadingStatus(CurrentLOID)

        Finally
            IsLoading = False
        End Try

        PendingSRID = 0

        MessageBox.Show("تمت إضافة الطلب إلى أمر التحميل بنجاح",
            "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub



    Private Sub SetDefaultStoreSafely(rowIndex As Integer, storeID As Integer)

        Dim col =
        CType(dgvLOs.Columns("colLOsStoreID"), DataGridViewComboBoxColumn)

        Dim dt As DataTable =
        CType(col.DataSource, DataTable)

        ' تأكد أن المستودع موجود في الـ DataSource
        Dim found() As DataRow = dt.Select("StoreID = " & storeID)

        If found.Length = 0 Then
            ' المستودع غير موجود → لا تضع قيمة
            dgvLOs.Rows(rowIndex).Cells("colLOsStoreID").Value = DBNull.Value
            Exit Sub
        End If

        dgvLOs.Rows(rowIndex).Cells("colLOsStoreID").Value = storeID

    End Sub
    Private Sub SetComboCellValueSafely(
    row As DataGridViewRow,
    columnName As String,
    value As Object
)

        If value Is Nothing OrElse IsDBNull(value) Then
            row.Cells(columnName).Value = DBNull.Value
            Exit Sub
        End If

        Dim col =
        CType(dgvLOs.Columns(columnName), DataGridViewComboBoxColumn)

        If col.DataSource Is Nothing Then
            row.Cells(columnName).Value = DBNull.Value
            Exit Sub
        End If

        Dim dt As DataTable = CType(col.DataSource, DataTable)
        Dim valueMember As String = col.ValueMember

        Dim found = dt.Select($"{valueMember} = {value}")

        If found.Length = 0 Then
            ' القيمة غير موجودة في Lookup
            row.Cells(columnName).Value = DBNull.Value
            Exit Sub
        End If

        row.Cells(columnName).Value = value

    End Sub
    Private Sub dgvLoadingSRD_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvLoadingSRD.CurrentCellDirtyStateChanged
        If IsLoading OrElse _isSavingGrid Then Exit Sub
        If dgvLoadingSRD.IsCurrentCellDirty Then
            dgvLoadingSRD.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub


    Private Sub dgvLoadingSR_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvLoadingSR.CellValueChanged

        If CurrentMode <> LoadingBoardMode.InvoiceSelection Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If dgvLoadingSR.Columns(e.ColumnIndex).Name <> "colLoadingSRtoInvoice" Then Exit Sub

        Dim currentRow = dgvLoadingSR.Rows(e.RowIndex)
        Dim isChecked As Boolean =
        CBool(If(currentRow.Cells("colLoadingSRtoInvoice").Value, False))

        If Not isChecked Then Exit Sub

        ' منع اختيار أكثر من طلب
        For Each r As DataGridViewRow In dgvLoadingSR.Rows
            If r Is currentRow Then Continue For
            r.Cells("colLoadingSRtoInvoice").Value = False
        Next

        ' ✅ تحديث SRID
        CurrentSRID = CInt(currentRow.Cells("colLoadingSRID").Value)

        ' ✅ فلترة التفاصيل
        LoadSRDDetailsForLO(CurrentLOID)

        IsSaved = False
        IsDirty = True

    End Sub


    Private Sub frmLoadingBoard_FormClosing(
    sender As Object,
    e As FormClosingEventArgs
) Handles Me.FormClosing

        ' لا يوجد LO → لا شيء
        If CurrentLOID = 0 Then Exit Sub

        ' تم حفظ فعلي → لا تنظيف
        ' LO قديم → لا تنظيف أبداً
        If Not IsNewLO Then Exit Sub

        ' LO جديد وتم حفظه → لا تنظيف
        If IsSaved Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    Using cmd1 As New SqlCommand("
DELETE FROM log.LoadingOrderDetail
WHERE LOID = @LOID
", con, tran)
                        cmd1.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd1.ExecuteNonQuery()
                    End Using

                    ' 3) حذف ربط الطلبات
                    Using cmd2 As New SqlCommand("
DELETE FROM log.LoadingOrderSR
WHERE LOID = @LOID
", con, tran)
                        cmd2.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd2.ExecuteNonQuery()
                    End Using

                    ' 4) حذف هيدر أمر التحميل (بدون شرط حالة)
                    Using cmd3 As New SqlCommand("
DELETE FROM log.LoadingOrder
WHERE LOID = @LOID
", con, tran)
                        cmd3.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd3.ExecuteNonQuery()
                    End Using

                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub
    Private Sub dgvLoadingSRD_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLoadingSRD.CellValueChanged

        If IsLoading OrElse _isSavingGrid Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If e.RowIndex >= dgvLoadingSRD.Rows.Count Then Exit Sub

        Dim row = dgvLoadingSRD.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim colName = dgvLoadingSRD.Columns(e.ColumnIndex).Name

        ' =====================================
        ' 🟥 1) حذف الصف (CheckBox)
        ' =====================================
        If colName = "colLoadingSRDDeleteFromThisLoadingOrder" Then

            Dim isDeleted As Boolean =
            CBool(If(row.Cells("colLoadingSRDDeleteFromThisLoadingOrder").Value, False))

            If isDeleted Then
                row.DefaultCellStyle.BackColor = Color.LightCoral
            Else
                row.DefaultCellStyle.BackColor = Color.White
            End If

            Exit Sub
        End If

        ' =====================================
        ' 🟩 2) تعديل الكمية
        ' =====================================
        If colName <> "colLoadingSRDLoadedInThisLO" Then Exit Sub

        Dim st As Integer = GetLoadingStatusID(CurrentLOID)

        If Not (st = 2 OrElse st = 14 OrElse st = 5) Then
            IsLoading = True
            Try
                row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D
            Finally
                IsLoading = False
            End Try
            Exit Sub
        End If

        ' 👇 هنا تكمل حساباتك (remaining / available / IsDirty)

    End Sub
    Private Sub dgvLoadingSRD_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvLoadingSRD.CellEndEdit
        If IsLoading OrElse _isSavingGrid Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        Dim colName = dgvLoadingSRD.Columns(e.ColumnIndex).Name
        If colName <> "colLoadingSRDLoadedInThisLO" Then Exit Sub

        Dim row = dgvLoadingSRD.Rows(e.RowIndex)

        Dim enteredQty As Decimal =
        CDec(If(row.Cells("colLoadingSRDLoadedInThisLO").Value, 0D))

        Dim availableQty As Decimal =
        CDec(If(row.Cells("colLoadingSRDAvailableQTY").Value, 0D))

        ' 🔒 منع إدخال أكبر من المتاح
        If enteredQty > availableQty Then

            MessageBox.Show(
        "الكمية المدخلة أكبر من المتاح.",
        "تنبيه",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    )

            row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D

            Me.BeginInvoke(New MethodInvoker(Sub()
                                                 dgvLoadingSRD.CurrentCell = row.Cells("colLoadingSRDLoadedInThisLO")
                                                 dgvLoadingSRD.BeginEdit(True)
                                             End Sub))

            Exit Sub
        End If

        ' إذا الإدخال صحيح → أعد الحساب
        Dim productID As Integer =
        CInt(row.Cells("colLoadingSRDProductID").Value)

        RecalculateAvailableQtyForProduct(productID)

    End Sub
    Private Sub RecalculateAvailableQtyForProduct(productID As Integer)

        Dim storeID As Object = Nothing

        If dgvLOs.Rows.Count > 0 Then
            storeID = dgvLOs.Rows(0).Cells("colLOsStoreID").Value
        End If

        If storeID Is Nothing OrElse IsDBNull(storeID) Then Exit Sub

        Dim qtyOnHand As Decimal = 0D
        Dim reservedQty As Decimal = 0D

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT 
                IB.QtyOnHand,
                ISNULL(SUM(IR.ReservedQty),0) AS ReservedQty
            FROM inv.Balance IB
            LEFT JOIN inv.Reservation IR
                ON IR.ProductID = IB.ProductID
               AND IR.SourceStoreID = IB.StoreID
               AND IR.ReservationStatusID = 1
            WHERE IB.ProductID = @ProductID
              AND IB.StoreID = @StoreID
            GROUP BY IB.QtyOnHand
        ", con)

                cmd.Parameters.AddWithValue("@ProductID", productID)
                cmd.Parameters.AddWithValue("@StoreID", storeID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        qtyOnHand = CDec(rd("QtyOnHand"))
                        reservedQty = CDec(rd("ReservedQty"))
                    End If
                End Using
            End Using
        End Using

        Dim realAvailable As Decimal = qtyOnHand - reservedQty
        If realAvailable < 0 Then realAvailable = 0

        For Each r As DataGridViewRow In dgvLoadingSRD.Rows

            If r.IsNewRow Then Continue For
            If CInt(r.Cells("colLoadingSRDProductID").Value) <> productID Then Continue For

            Dim loadedBefore As Decimal =
            CDec(If(r.Cells("colLoadingSRDLoadedBefore").Value, 0))

            Dim loadedSaved As Decimal =
            CDec(If(r.Cells("colLoadingSRDLoadedQTY").Value, 0))

            Dim loadedSession As Decimal =
            CDec(If(r.Cells("colLoadingSRDLoadedInThisLO").Value, 0))

            Dim availableDisplayed As Decimal =
            realAvailable -
            loadedBefore -
            loadedSaved -
            loadedSession

            If availableDisplayed < 0 Then availableDisplayed = 0

            r.Cells("colLoadingSRDAvailableQTY").Value = availableDisplayed

            Dim requiredQty As Decimal =
            CDec(If(r.Cells("colLoadingSRDQTY").Value, 0))

            r.Cells("colLoadingSRDRemainingQTY").Value =
            requiredQty -
            loadedBefore -
            loadedSaved -
            loadedSession

        Next

    End Sub
    Private Sub dgvLOs_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvLOs.CellValueChanged

        If IsLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        ' فقط عند تغيير المخزن
        If dgvLOs.Columns(e.ColumnIndex).Name <> "colLOsStoreID" Then Exit Sub

        Dim cellValue = dgvLOs.Rows(e.RowIndex).Cells("colLOsStoreID").Value

        If cellValue Is Nothing OrElse IsDBNull(cellValue) Then
            CurrentSelectedStoreID = 0
            Exit Sub
        End If

        CurrentSelectedStoreID = CInt(cellValue)

        ' إعادة تحميل التفاصيل لإعادة حساب الكميات فورًا
        If CurrentLOID > 0 Then
            LoadSRDDetailsForLO(CurrentLOID)
        End If

    End Sub
    Private Sub dgvLOs_CurrentCellDirtyStateChanged(
    sender As Object,
    e As EventArgs
) Handles dgvLOs.CurrentCellDirtyStateChanged

        If dgvLOs.IsCurrentCellDirty Then
            dgvLOs.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub



    Private Sub btnSendLoading_Click(sender As Object, e As EventArgs) Handles btnSendLoading.Click

        If CurrentLOID = 0 Then Exit Sub

        '====================================================
        ' 1) تحقق من الحفظ
        '====================================================
        If Not IsSaved Then
            MessageBox.Show("يجب حفظ أمر التحميل أولاً.")
            Exit Sub
        End If

        Dim st As Integer = GetLoadingStatusID(CurrentLOID)

        If Not (st = 2 OrElse st = 14) Then
            MessageBox.Show("لا يمكن ترحيل أمر التحميل في هذه الحالة.", "تنبيه")
            Exit Sub
        End If


        '====================================================
        ' 2) تحقق من وجود كميات
        '====================================================
        Dim hasQty As Boolean = False

        For Each r As DataGridViewRow In dgvLoadingSRD.Rows

            If r.IsNewRow Then Continue For

            If CDec(If(r.Cells("colLoadingSRDLoadedQTY").Value, 0)) > 0 Then
                hasQty = True
                Exit For
            End If

        Next

        If Not hasQty Then
            MessageBox.Show("لم يتم تحميل أي كميات.")
            Exit Sub
        End If


        If IsDirty Then
            MessageBox.Show("يوجد تعديلات غير محفوظة. يرجى الحفظ أولاً.")
            Exit Sub
        End If


        '====================================================
        ' 3) تحقق من بيانات الهيدر
        '====================================================
        If dgvLOs.CurrentRow Is Nothing Then
            MessageBox.Show("لا يوجد أمر تحميل محدد.")
            Exit Sub
        End If

        Dim row As DataGridViewRow = dgvLOs.CurrentRow

        If row.Cells("colLOsDriverCode").Value Is Nothing _
    OrElse row.Cells("colLOsDriverCode").Value.ToString() = "" Then
            MessageBox.Show("يجب اختيار السائق.")
            Exit Sub
        End If

        If row.Cells("colLOsSupervisor").Value Is Nothing _
    OrElse row.Cells("colLOsSupervisor").Value.ToString() = "" Then
            MessageBox.Show("يجب اختيار المشرف.")
            Exit Sub
        End If

        If row.Cells("colLOsVehicale").Value Is Nothing _
    OrElse row.Cells("colLOsVehicale").Value.ToString() = "" Then
            MessageBox.Show("يجب اختيار السيارة.")
            Exit Sub
        End If

        If row.Cells("colLOsStoreID").Value Is Nothing _
    OrElse CInt(If(row.Cells("colLOsStoreID").Value, 0)) = 0 Then
            MessageBox.Show("يجب اختيار المستودع.")
            Exit Sub
        End If


        '====================================================
        ' 4) شاشة التأكيد
        '====================================================
        Dim frm As New frmPostLoadingConfirmation

        Dim totalVolume As Decimal = GetTotalLoadedVolume(CurrentLOID)

        frm.lblTotalLoadedVolume.Text =
        totalVolume.ToString("N3") & "   متر مكعب"

        frm.dgvPostingOrderConfirmation.DataSource =
        BuildSendConfirmationData(CurrentLOID)

        If frm.ShowDialog() <> DialogResult.OK Then Exit Sub


        '====================================================
        ' 5) تنفيذ الترحيل
        '====================================================
        Try

            service.SendLoadingOrder(CurrentLOID, CurrentUser.EmployeeID)


            MessageBox.Show("تم الارسال وبانتظار فسح البضائع",
                        "نجاح",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

        Catch ex As Exception

            If ex.Message.Contains("Insufficient stock") Then

                MessageBox.Show("الكمية غير كافية في المخزون",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)

            Else

                MessageBox.Show(ex.Message,
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)

            End If

        End Try

    End Sub
    Private Function BuildSendConfirmationData(loID As Integer) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String = "
     SELECT
    SRD.ProductCode,
    SUM(LOD.LoadedQty) AS TotalQty,
    P.PartnerName,
    E.EmpName AS DriverName
FROM log.LoadingOrderDetail LOD
INNER JOIN inv.SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
INNER JOIN inv.SR SR
    ON SR.SRID = SRD.SRID
INNER JOIN md.Partner P
    ON P.PartnerID = SR.PartnerID
INNER JOIN log.LoadingOrder LO
    ON LO.LOID = LOD.LOID
INNER JOIN sec.Employee E
    ON E.EmployeeID = LO.DriverEmployeeID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
GROUP BY SRD.ProductCode, P.PartnerName, E.EmpName
ORDER BY SRD.ProductCode

        "

            Using da As New SqlDataAdapter(sql, con)
                da.SelectCommand.Parameters.AddWithValue("@LOID", loID)
                da.Fill(dt)
            End Using
        End Using

        Return dt

    End Function
    Private Function GetTotalLoadedVolume(loID As Integer) As Decimal

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT ISNULL(SUM(
                (ISNULL(Length_cm,0) *
                 ISNULL(Width_cm,0) *
                 ISNULL(Height_cm,0)) 
                 / 1000000.0
                 * LoadedQty
            ),0)
            FROM log.LoadingOrderDetail
            WHERE LOID = @LOID
              AND LoadedQty > 0
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Return CDec(cmd.ExecuteScalar())
            End Using
        End Using

    End Function
    Private Function GetPartnerNames(loID As Integer) As String

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT DISTINCT P.PartnerName
            FROM log.LoadingOrderDetail LOD
            INNER JOIN inv.SR SR
                ON SR.SRID = LOD.SourceHeaderID
            INNER JOIN md.Partner P
                ON P.PartnerID = SR.PartnerID
            WHERE LOD.LOID = @LOID
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    Dim names As New List(Of String)
                    While rd.Read()
                        names.Add(rd(0).ToString())
                    End While
                    Return String.Join(", ", names)
                End Using
            End Using
        End Using

    End Function
    Private Function GetDriverName(loID As Integer) As String

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT E.EmpName
            FROM log.LoadingOrder LO
            INNER JOIN sec.Employee E
                ON E.EmployeeID = LO.DriverEmployeeID
            WHERE LO.LOID = @LOID
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Dim result = cmd.ExecuteScalar()
                Return If(result Is Nothing, "", result.ToString())
            End Using
        End Using

    End Function
    Private Sub dgvLoadingSR_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLoadingSR.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        If dgvLoadingSR.Columns(e.ColumnIndex).Name = "colLoadingSRtoInvoice" Then

            For Each row As DataGridViewRow In dgvLoadingSR.Rows
                row.Cells("colLoadingSRtoInvoice").Value = False
            Next

            dgvLoadingSR.Rows(e.RowIndex).Cells("colLoadingSRtoInvoice").Value = True

        End If

    End Sub
    Private Sub btnExportToInvoice_Click(sender As Object, e As EventArgs) Handles btnExportToInvoice.Click

        ' 1️⃣ تأكد اختيار أمر تحميل
        If dgvOpenedLOs.CurrentRow Is Nothing Then
            MessageBox.Show("اختر أمر تحميل أولاً", "تنبيه")
            Exit Sub
        End If

        ' 2️⃣ استخراج LOID
        SelectedLOID = CInt(dgvOpenedLOs.CurrentRow.Cells("colOpenLOsID").Value)

        ' 3️⃣ البحث عن SR المحدد
        Dim found As Boolean = False
        If dgvLoadingSR.Rows.Count = 0 Then
            MessageBox.Show("لا توجد طلبات في هذا التحميل", "تنبيه")
            Exit Sub
        End If
        For Each row As DataGridViewRow In dgvLoadingSR.Rows

            If row.Cells("colLoadingSRtoInvoice").Value IsNot Nothing AndAlso
           CBool(row.Cells("colLoadingSRtoInvoice").Value) = True Then

                SelectedSRID = CInt(row.Cells("colLoadingSRID").Value)
                found = True
                Exit For

            End If

        Next

        ' 4️⃣ تحقق
        If Not found Then
            MessageBox.Show("اختر طلب مبيعات واحد", "تنبيه")
            Exit Sub
        End If

        ' 5️⃣ إرجاع النتيجة
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub


    Private Sub AddFocusLOToOpenedGrid(loID As Integer)

        dgvOpenedLOs.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    LO.LOID,
    LO.LOCode,
    LO.InitiatedDateTime,
    LO.LoadingStatusID,
    S.StatusName AS LoadingStatusName,
    V.VehicleCode,
    E.EmpName AS SupervisorName
FROM log.LoadingOrder LO
INNER JOIN wf.Status S
    ON S.StatusID = LO.LoadingStatusID
LEFT JOIN md.Vehicle V
    ON V.VehicleID = LO.VehicleID
LEFT JOIN sec.Employee E
    ON E.EmployeeID = LO.LoadingSupervisorID
WHERE LO.LOID = @LOID
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim r As Integer = dgvOpenedLOs.Rows.Add()

                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsID").Value = CInt(rd("LOID"))
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsCode").Value = rd("LOCode").ToString()
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsInitiatedDateTime").Value = CDate(rd("InitiatedDateTime"))
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsStatusID").Value = CInt(rd("LoadingStatusID"))
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsStatus").Value = rd("LoadingStatusName").ToString()
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsVehicleInfo").Value =
                            If(rd("VehicleCode") Is DBNull.Value, "", rd("VehicleCode").ToString())
                        dgvOpenedLOs.Rows(r).Cells("colOpenLOsSupervisor").Value =
                            If(rd("SupervisorName") Is DBNull.Value, "", rd("SupervisorName").ToString())
                    End If
                End Using
            End Using
        End Using

        dgvOpenedLOs.ClearSelection()

        If dgvOpenedLOs.Rows.Count > 0 Then
            dgvOpenedLOs.Rows(0).Selected = True

            For Each c As DataGridViewColumn In dgvOpenedLOs.Columns
                If c.Visible Then
                    dgvOpenedLOs.CurrentCell = dgvOpenedLOs.Rows(0).Cells(c.Index)
                    Exit For
                End If
            Next
        Else
            dgvOpenedLOs.CurrentCell = Nothing
        End If

    End Sub
    Private Function GetLoadingStatusID(loID As Integer) As Integer
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
SELECT LoadingStatusID
FROM log.LoadingOrder
WHERE LOID = @LOID
", con)
                cmd.Parameters.AddWithValue("@LOID", loID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0
                Return CInt(obj)
            End Using
        End Using
    End Function


    Private Sub dgvLoadingSR_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvLoadingSR.CellBeginEdit

        ' نسمح بالتعديل فقط في وضع الفاتورة
        If CurrentMode <> LoadingBoardMode.InvoiceSelection Then
            e.Cancel = True
            Exit Sub
        End If

        ' نسمح بالتعديل فقط لعمود التشيك
        If dgvLoadingSR.Columns(e.ColumnIndex).Name <> "colLoadingSRtoInvoice" Then
            e.Cancel = True
            Exit Sub
        End If

    End Sub
    Private Sub dgvLoadingSR_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvLoadingSR.CurrentCellDirtyStateChanged
        If CurrentMode <> LoadingBoardMode.InvoiceSelection Then Exit Sub
        If dgvLoadingSR.IsCurrentCellDirty Then
            dgvLoadingSR.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub
    Private Sub SetCurrentLO(loID As Integer)
        If CurrentLOID = loID Then Exit Sub
        If loID <= 0 Then Exit Sub

        ' 🔥 إيقاف الأحداث
        IsLoading = True

        Try
            ' 🔥 أهم شيء: Reset الحالة
            isPostedEditMode = False

            ' 🔥 تحديد LO في الجريد
            For Each r As DataGridViewRow In dgvOpenedLOs.Rows
                If r.IsNewRow Then Continue For
                If r.Cells("colOpenLOsID").Value Is Nothing Then Continue For

                If CInt(r.Cells("colOpenLOsID").Value) = loID Then
                    dgvOpenedLOs.ClearSelection()
                    r.Selected = True

                    ' اختيار خلية مرئية فقط
                    ' 🔥 لا تغيّر CurrentCell هنا أبداً
                    ' فقط حدّث CurrentLOID
                    CurrentLOID = loID
                    Exit For
                End If
            Next

            ' 🔥 تحديث المتغير
            CurrentLOID = loID

            ' 🔥 تحميل البيانات
            LoadLOHeader(loID)
            LoadSRsForLO(loID)
            LoadSRDDetailsForLO(loID)

            ' 🔥 تطبيق السياسة
            ApplyEditPolicyByLoadingStatus(loID)

        Finally
            IsLoading = False
        End Try

    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        Dim frm As New frmLoadingSearch()

        If frm.ShowDialog() <> DialogResult.OK Then Exit Sub
        If frm.SelectedLOID <= 0 Then Exit Sub

        ' 🔑 تحديد الـ LO المختار
        Me.FocusLOID = frm.SelectedLOID
        isPostedEditMode = False

        ' 🔥 الطريقة المثالية: تحميل LO واحد فقط + تحديده مباشرة
        AddFocusLOToOpenedGrid(Me.FocusLOID)

        ' 🔄 تحميل باقي البيانات
        SetCurrentLO(Me.FocusLOID)
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If CurrentLOID = 0 Then
            MessageBox.Show("لم يتم اختيار أمر تحميل")
            Exit Sub
        End If
        Dim statusID As Integer = GetLoadingStatusID(CurrentLOID)

        If statusID <> 2 OrElse statusID <> 5 OrElse statusID <> 14 Then
            MessageBox.Show("لا يمكن الغاء سند مرحل  ")
            Exit Sub
        End If

        If MessageBox.Show("هل تريد إلغاء أمر التحميل؟", "تأكيد",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Warning) <> DialogResult.Yes Then Exit Sub

        Try


            service.CancelLoadingOrder(CurrentLOID, CurrentUser.EmployeeID)

            MessageBox.Show("تم إلغاء أمر التحميل")

            ' إعادة تحميل
            LoadOpenedLoadingOrders()

            dgvLoadingSR.Rows.Clear()
            dgvLoadingSRD.Rows.Clear()
            isPostedEditMode = False

            CurrentLOID = 0

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnSaveLO_Click(sender As Object, e As EventArgs) Handles btnSaveLO.Click
        Try

            Dim service As New LoadingApplicationService(ConnStr)

            service.SaveLoadingOrder(
    CurrentLOID,
    CurrentUser.EmployeeID,
    dgvLOs,
    dgvLoadingSRD,
    IsSaved,
    IsDirty,
    IsLoading,
    _isSavingGrid,
    CurrentSelectedStoreID,
    isPostedEditMode,
    OriginalOutputTable   ' 🔥 مهم جدًا
)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Exit Sub
        End Try

        ' =========================
        ' UI Reload فقط
        ' =========================
        Dim savedLOID As Integer = CurrentLOID

        IsLoading = True
        Try
            LoadOpenedLoadingOrders()

            CurrentLOID = savedLOID

            For Each r As DataGridViewRow In dgvOpenedLOs.Rows
                If r.IsNewRow Then Continue For
                If r.Cells("colOpenLOsID").Value Is Nothing Then Continue For
                If CInt(r.Cells("colOpenLOsID").Value) = savedLOID Then
                    dgvOpenedLOs.ClearSelection()
                    r.Selected = True
                    For Each c As DataGridViewCell In r.Cells
                        If c.Visible Then
                            dgvOpenedLOs.CurrentCell = c
                            Exit For
                        End If
                    Next
                    Exit For
                End If
            Next
            isPostedEditMode = False

            LoadLOHeader(CurrentLOID)
            LoadSRsForLO(CurrentLOID)
            LoadSRDDetailsForLO(CurrentLOID)
            ApplyEditPolicyByLoadingStatus(CurrentLOID)

        Finally
            IsLoading = False
        End Try

    End Sub

    Private Sub btnEditPostedProduction_Click(sender As Object, e As EventArgs) Handles btnEditPostedProduction.Click

        If CurrentLOID <= 0 Then
            MessageBox.Show("لا يوجد سند مفتوح")
            Exit Sub
        End If
        ApplyEditPolicyByLoadingStatus(CurrentLOID)
        Application.DoEvents()
        Dim statusID As Integer = GetLoadingStatusID(CurrentLOID)

        If statusID <> 15 Then
            MessageBox.Show("هذا التعديل مخصص للسندات المرحلة فقط ")
            Exit Sub
        End If

        If service.IsLOInCorrectionQueue(CurrentLOID) Then
            MessageBox.Show("لا يمكن تعديل السند لأنه موجود في قائمة التصحيح")
            Exit Sub
        End If        ' 🔥 تفعيل وضع التعديل
        isPostedEditMode = True

        ' 🔥 نحفظ نسخة من الجريد الحالي
        OriginalOutputTable = GetSRDLoadedSnapshotWithLinks(CurrentLOID)
        ' 🔥 فك القفل
        ApplyEditPolicyByLoadingStatus(CurrentLOID)
        MessageBox.Show("تم تفعيل وضع تعديل سند تحميل")

    End Sub

    Private Sub btnDeletePostedLoading_Click(sender As Object, e As EventArgs) Handles btnDeletePostedLoading.Click
        Dim statusID As Integer = GetLoadingStatusID(CurrentLOID)

        If CurrentLOID <= 0 Then
            MessageBox.Show("لا يوجد سند مفتوح")
            Exit Sub
        End If
        ApplyEditPolicyByLoadingStatus(CurrentLOID)
        Application.DoEvents()
        If statusID <> 15 Then
            MessageBox.Show("هذا التعديل مخصص للسندات المرحلة فقط ")
            Exit Sub
        End If

        If service.IsLOInCorrectionQueue(CurrentLOID) Then
            MessageBox.Show("لا يمكن الغاء السند لأنه موجود في قائمة التصحيح")
            Exit Sub
        End If        ' 🔥 تفعيل وضع التعديل
        Dim result = MessageBox.Show(
            " سيتم إلغاء عملية التحميل بالكامل، هل أنت متأكد؟ يجب الحفظ بعد التاكيد لتثبيت الالغاء",
            "تأكيد",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

        If result <> DialogResult.Yes Then Exit Sub

        ' 🔥 وضع التعديل
        btnEditPostedProduction.PerformClick()

        dgvLoadingSRD.EndEdit()
        isPostedEditMode = True

        ' 🔥 تصفير + إعادة حساب
        For Each row As DataGridViewRow In dgvLoadingSRD.Rows
            If row.IsNewRow Then Continue For

            row.Cells("colLoadingSRDLoadedQTY").Value = 0D

            Dim productID As Integer =
        CInt(row.Cells("colLoadingSRDProductID").Value)

            RecalculateAvailableQtyForProduct(productID)
        Next
        dgvLoadingSRD.Refresh()


    End Sub
    Private Function GetDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal
        Decimal.TryParse(v.ToString(), d)
        Return d
    End Function
    Private Function GetSRDLoadedSnapshotWithLinks(loID As Integer) As DataTable

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    LOD.LoadingOrderDetailID   AS DocumentDetailID,
    TD.DetailID               AS TransactionDetailID,
    CL.LedgerID               AS LedgerID,
    TD.ProductID,
    CL.OutQty                 AS Qty
FROM log.LoadingOrderDetail LOD
INNER JOIN inv.TransactionDetails TD
    ON TD.SourceDocumentDetailID = LOD.LoadingOrderDetailID
INNER JOIN inv.TransactionHeader TH
    ON TH.TransactionID = TD.TransactionID
INNER JOIN inv.CostLedger CL
    ON CL.SourceDetailID = TD.DetailID
WHERE LOD.LOID = @LOID
  AND TH.SourceDocumentID = @LOID
  AND TH.OperationTypeID = 4
ORDER BY LOD.LoadingOrderDetailID
", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt

    End Function




End Class
