Imports System.Data.SqlClient
Imports System.Runtime.Remoting.Messaging

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

    Private IsLoading As Boolean = False
    Private IsDirty As Boolean = False

    Public Property FocusLOID As Integer
    Public Property PendingSRID As Integer = 0
    Private _isGridUpdating As Boolean = False   ' لمنع CellValueChanged/EndEdit أثناء التحديث البرمجي
    Private _isSavingGrid As Boolean = False
    Private CurrentLOID As Integer = 0
    Private CurrentSRID As Integer = 0
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
    End Enum
    Public Property CurrentMode As LoadingBoardMode = LoadingBoardMode.Normal
    Private Sub ApplyModeUI()

        ' Visible by mode
        btnCloseBoard.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection)
        btnAddSelectedSRToLO.Visible = (CurrentMode = LoadingBoardMode.Normal)
        btnSaveLO.Visible = (CurrentMode <> LoadingBoardMode.InvoiceSelection)
        btnPostLoading.Visible = (CurrentMode = LoadingBoardMode.Normal)
        btnExportToInvoice.Visible = (CurrentMode = LoadingBoardMode.InvoiceSelection)

        If CurrentMode = LoadingBoardMode.InvoiceSelection Then

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
            FROM Security_Employee
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
            FROM Security_Employee
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
            FROM Master_Store
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
            FROM Master_Vehicle
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

                If CurrentMode = LoadingBoardMode.InvoiceSelection Then
                    ' ✅ في وضع الفاتورة: اعرض LO سواء 15 أو 8
                    statusFilter = "15,8"
                Else
                    ' الوضع القديم كما هو
                    statusFilter = "0,1,2,14"
                End If

                ' ✅ فلترة "متاح للسحب" تختلف فقط في وضع الفاتورة
                Dim sqlAvailabilityFilter As String

                If CurrentMode = LoadingBoardMode.InvoiceSelection Then
                    ' متاح للسحب إذا يوجد على الأقل LOD واحد LoadedQty>0 وغير مرتبط بفاتورة SAL غير ملغاة
                    sqlAvailabilityFilter = "
AND EXISTS (
    SELECT 1
    FROM Logistics_LoadingOrderDetail LOD
    WHERE LOD.LOID = LO.LOID
      AND ISNULL(LOD.LoadedQty,0) > 0
      AND NOT EXISTS (
            SELECT 1
            FROM Inventory_DocumentDetails IDD
            INNER JOIN Inventory_DocumentHeader H
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
    FROM Logistics_LoadingOrderDetail LOD
    WHERE LOD.LOID = LO.LOID
      AND NOT EXISTS (
            SELECT 1
            FROM Inventory_DocumentDetails IDD
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
FROM dbo.Logistics_LoadingOrder LO
INNER JOIN dbo.Workflow_Status S
    ON S.StatusID = LO.LoadingStatusID
LEFT JOIN dbo.Master_Vehicle V
    ON V.VehicleID = LO.VehicleID
LEFT JOIN dbo.Security_Employee E
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
            FROM Business_SR
            LEFT JOIN Master_Partner P ON P.PartnerID = SR.PartnerID
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
                ApplyUIMode() ' keeps ViewOnly restrictions
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
        If CurrentMode = LoadingBoardMode.InvoiceSelection Then
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
            FROM Logistics_LoadingOrder
            WHERE LOID = @LOID
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        Dim r As Integer = dgvLOs.Rows.Add()
                        Dim row As DataGridViewRow = dgvLOs.Rows(r)

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

    Private Sub btnCloseBoard_Click(
    sender As Object,
    e As EventArgs
) Handles btnCloseBoard.Click


        ' لاحقًا: لو LO محفوظ
        Me.Close()

    End Sub
    Private Sub btnSaveLO_Click(sender As Object, e As EventArgs) Handles btnSaveLO.Click
        If CurrentLOID <= 0 Then
            MessageBox.Show("لم يتم اختيار أمر تحميل", "تنبيه")
            Exit Sub
        End If

        ' الحالات المسموح بها حسب جدولك الجديد (LOD Scope):
        ' 0,1,2  : يسمح حفظ كامل
        ' 14     : يسمح حفظ كامل
        ' 15     : يسمح تعديل الهيدر فقط
        ' غير ذلك: لا يسمح

        Dim statusID As Integer = GetLoadingStatusID(CurrentLOID)

        ' ✅ Header-only في WAITING_INVOICE (15)
        If statusID = 15 Then
            Using con As New SqlConnection(ConnStr)
                con.Open()
                Using cmdHdr As New SqlCommand("
UPDATE dbo.Logistics_LoadingOrder
SET
    DriverEmployeeID    = @DriverEmployeeID,
    LoadingSupervisorID = @LoadingSupervisorID,
    VehicleID           = @VehicleID,
    Notes               = @Notes,
    ModifiedAt          = SYSDATETIME(),
    ModifiedBy          = @UserID
WHERE LOID = @LOID
", con)

                    Dim row = dgvLOs.Rows(0)

                    cmdHdr.Parameters.AddWithValue("@LOID", CurrentLOID)
                    cmdHdr.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                    cmdHdr.Parameters.AddWithValue("@DriverEmployeeID", If(row.Cells("colLOsDriverCode").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@LoadingSupervisorID", If(row.Cells("colLOsSupervisor").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@VehicleID", If(row.Cells("colLOsVehicale").Value, DBNull.Value))
                    cmdHdr.Parameters.AddWithValue("@Notes", If(row.Cells("colLOsNote").Value, DBNull.Value))

                    cmdHdr.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("تم حفظ بيانات الهيدر فقط (الحالة: WAITING_INVOICE).", "تم")
            IsSaved = True
            IsDirty = False
            Return
        End If

        ' ✅ السماح بالحفظ الكامل فقط في: 0,1,2,14
        Dim allowFullSave As Boolean = (statusID = 0 OrElse statusID = 1 OrElse statusID = 2 OrElse statusID = 14)

        If Not allowFullSave Then
            MessageBox.Show("لا يمكن الحفظ في هذه الحالة.", "تنبيه")
            Exit Sub
        End If


        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    '=========================================================
                    ' (1) حفظ بيانات الهيدر
                    '=========================================================
                    Using cmdHdr As New SqlCommand("
UPDATE dbo.Logistics_LoadingOrder
SET
    DriverEmployeeID    = @DriverEmployeeID,
    LoadingSupervisorID = @LoadingSupervisorID,
    VehicleID           = @VehicleID,
    SourceStoreID       = @SourceStoreID,
    Notes               = @Notes,
    ModifiedAt          = SYSDATETIME(),
    ModifiedBy          = @UserID
WHERE LOID = @LOID
", con, tran)

                        Dim row = dgvLOs.Rows(0)

                        cmdHdr.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmdHdr.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)

                        cmdHdr.Parameters.AddWithValue("@DriverEmployeeID", If(row.Cells("colLOsDriverCode").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@LoadingSupervisorID", If(row.Cells("colLOsSupervisor").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@VehicleID", If(row.Cells("colLOsVehicale").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@SourceStoreID", If(row.Cells("colLOsStoreID").Value, DBNull.Value))
                        cmdHdr.Parameters.AddWithValue("@Notes", If(row.Cells("colLOsNote").Value, DBNull.Value))

                        cmdHdr.ExecuteNonQuery()
                    End Using


                    If dgvLoadingSRD.IsCurrentCellDirty Then
                        dgvLoadingSRD.CommitEdit(DataGridViewDataErrorContexts.Commit)
                    End If
                    dgvLoadingSRD.EndEdit()


                    '=========================================================
                    ' (2) حفظ LoadedQty في تفاصيل التحميل
                    '=========================================================
                    _isSavingGrid = True
                    Try
                        Using cmdDet As New SqlCommand("
UPDATE dbo.Logistics_LoadingOrderDetail
SET LoadedQty = @LoadedQty
WHERE LoadingOrderDetailID = @LODID
", con, tran)

                            For Each row As DataGridViewRow In dgvLoadingSRD.Rows
                                If row.IsNewRow Then Continue For
                                If row.Cells("colLoadingOrderDetailID").Value Is Nothing Then Continue For
                                If IsDBNull(row.Cells("colLoadingOrderDetailID").Value) Then Continue For

                                Dim lodID As Integer = CInt(row.Cells("colLoadingOrderDetailID").Value)

                                Dim loadedSaved As Decimal = CDec(If(row.Cells("colLoadingSRDLoadedQTY").Value, 0D))
                                Dim loadedSession As Decimal = CDec(If(row.Cells("colLoadingSRDLoadedInThisLO").Value, 0D))
                                Dim loadedQty As Decimal = loadedSaved + loadedSession

                                cmdDet.Parameters.Clear()
                                cmdDet.Parameters.Add("@LODID", SqlDbType.Int).Value = lodID

                                Dim p = cmdDet.Parameters.Add("@LoadedQty", SqlDbType.Decimal)
                                p.Precision = 18
                                p.Scale = 3
                                p.Value = loadedQty

                                cmdDet.ExecuteNonQuery()

                                row.Cells("colLoadingSRDLoadedQTY").Value = loadedQty
                                row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D
                            Next
                        End Using
                    Finally
                        _isSavingGrid = False
                    End Try

                    '=========================================================
                    ' (3) منطق الحفظ المركزي (بدون SP) - نسخة من log.SaveLoadingOrder
                    '=========================================================

                    ' (3.1) قراءة OperationTypeID + SourceStoreID (مهم للحجز)
                    Dim operationTypeID As Integer
                        Dim sourceStoreIDObj As Object

                        Using cmdOp As New SqlCommand("
SELECT OperationTypeID, SourceStoreID
FROM dbo.Logistics_LoadingOrder
WHERE LOID = @LOID
", con, tran)
                            cmdOp.Parameters.AddWithValue("@LOID", CurrentLOID)

                            Using rd = cmdOp.ExecuteReader()
                                If Not rd.Read() Then Throw New Exception("LO غير موجود")
                                If IsDBNull(rd("OperationTypeID")) Then Throw New Exception("OperationTypeID غير موجود لأمر التحميل")
                                operationTypeID = CInt(rd("OperationTypeID"))
                                sourceStoreIDObj = rd("SourceStoreID")
                            End Using
                        End Using

                        If sourceStoreIDObj Is Nothing OrElse IsDBNull(sourceStoreIDObj) Then
                            Throw New Exception("SourceStoreID غير محدد في أمر التحميل - لا يمكن إنشاء حجز")
                        End If


                        ' (3.2) تحديث Volume_m3
                        Using cmdVol As New SqlCommand("
UPDATE LOD
SET Volume_m3 =
    (ISNULL(Length_cm,0) * ISNULL(Width_cm,0) * ISNULL(Height_cm,0)) / 1000000.0
FROM dbo.Logistics_LoadingOrderDetail LOD
WHERE LOD.LOID = @LOID
", con, tran)
                            cmdVol.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdVol.ExecuteNonQuery()
                        End Using


                        ' (3.3) تحديث الحجز الموجود (LoadedQty > 0)
                        Using cmdUpdRes As New SqlCommand("
UPDATE IR
SET
    IR.ReservedQty = LOD.LoadedQty,
    IR.ReservedAt  = SYSDATETIME(),
    IR.ReleasedAt  = NULL,
    IR.CreatedBy   = @UserID
FROM dbo.Inventory_Reservation IR
INNER JOIN dbo.Logistics_LoadingOrderDetail LOD
    ON IR.SourceDetailID        = LOD.SourceDetailID
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
", con, tran)
                            cmdUpdRes.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                            cmdUpdRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                            cmdUpdRes.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdUpdRes.ExecuteNonQuery()
                        End Using


                        ' (3.4) حذف الحجز إذا LoadedQty = 0
                        Using cmdDelRes As New SqlCommand("
DELETE IR
FROM dbo.Inventory_Reservation IR
INNER JOIN dbo.Logistics_LoadingOrderDetail LOD
    ON IR.SourceDetailID        = LOD.SourceDetailID
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty = 0
", con, tran)
                            cmdDelRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                            cmdDelRes.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdDelRes.ExecuteNonQuery()
                        End Using


                        ' (3.5) إنشاء حجز جديد (للأسطر بدون حجز)
                        Using cmdInsRes As New SqlCommand("
INSERT INTO dbo.Inventory_Reservation
(
    ProductID,
    SourceStoreID,
    ReservedQty,
    SourceOperationTypeID,
    SourceID,
    CostAtReserve,
    ReservedAt,
    CreatedBy,
    ReservationStatusID,
    SourceDetailID
)
SELECT
    LOD.ProductID,
    LO.SourceStoreID,
    LOD.LoadedQty,
    @OperationTypeID,
    LOD.LoadingOrderDetailID,
    0,
    SYSDATETIME(),
    @UserID,
    1,
    LOD.SourceDetailID
FROM dbo.Logistics_LoadingOrderDetail LOD
INNER JOIN dbo.Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID
LEFT JOIN dbo.Inventory_Reservation IR
    ON IR.SourceDetailID        = LOD.SourceDetailID
   AND IR.SourceOperationTypeID = @OperationTypeID
   AND IR.ProductID             = LOD.ProductID
WHERE LOD.LOID = @LOID
  AND LOD.LoadedQty > 0
  AND LOD.SourceDetailID IS NOT NULL
  AND IR.ReservationID IS NULL
", con, tran)
                            cmdInsRes.Parameters.AddWithValue("@OperationTypeID", operationTypeID)
                            cmdInsRes.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                            cmdInsRes.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdInsRes.ExecuteNonQuery()
                        End Using


                        ' (3.6) تحديث حالة SRD حسب إجمالي التحميل
                        Using cmdUpdSRD As New SqlCommand("
;WITH TotalLoaded AS
(
    SELECT
        SourceDetailID,
        SUM(LoadedQty) AS TotalLoadedQty
    FROM dbo.Logistics_LoadingOrderDetail
    GROUP BY SourceDetailID
)
UPDATE SRD
SET SRD.BusinessStatusID =
    CASE
        WHEN ISNULL(T.TotalLoadedQty, 0) >= SRD.Quantity THEN 13
        WHEN ISNULL(T.TotalLoadedQty, 0) > 0        THEN 12
        ELSE SRD.BusinessStatusID
    END
FROM dbo.Business_SRD SRD
LEFT JOIN TotalLoaded T
    ON T.SourceDetailID = SRD.SRDID
WHERE EXISTS
(
    SELECT 1
    FROM dbo.Logistics_LoadingOrderDetail X
    WHERE X.SourceDetailID = SRD.SRDID
      AND X.LOID = @LOID
)
", con, tran)
                            cmdUpdSRD.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdUpdSRD.ExecuteNonQuery()
                        End Using


                        ' (3.7) تحديث حالة أمر التحميل
                        ' (3.7) تحديث حالة أمر التحميل - فقط في الحالات المسموحة
                        Using cmdUpdLO As New SqlCommand("
UPDATE dbo.Logistics_LoadingOrder
SET
    LoadingStatusID = 14,
    ModifiedAt = SYSDATETIME(),
    ModifiedBy = @UserID
WHERE LOID = @LOID
", con, tran)
                            cmdUpdLO.Parameters.AddWithValue("@LOID", CurrentLOID)
                            cmdUpdLO.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                            cmdUpdLO.ExecuteNonQuery()
                        End Using
                        tran.Commit()
                        IsSaved = True
                        IsDirty = False

                    Catch ex As Exception
                        Try : tran.Rollback() : Catch : End Try
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            End Using
        End Using

        IsSaved = True
        MessageBox.Show("تم حفظ أمر التحميل بنجاح")
        IsDirty = False

        IsLoading = True
        Try
            Dim savedLOID As Integer = CurrentLOID
            LoadOpenedLoadingOrders()
            CurrentLOID = savedLOID
        Finally
            IsLoading = False
        End Try

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
        If Not (st = 2 OrElse st = 14) Then
            e.Cancel = True
        End If

    End Sub

    Private Sub dgvOpenedLOs_CurrentCellChanged(
    sender As Object,
    e As EventArgs
) Handles dgvOpenedLOs.CurrentCellChanged

        If IsLoading Then Exit Sub
        If dgvOpenedLOs.CurrentCell Is Nothing Then Exit Sub
        If dgvOpenedLOs.CurrentCell.RowIndex < 0 Then Exit Sub

        Dim loID As Integer =
        CInt(dgvOpenedLOs.Rows(dgvOpenedLOs.CurrentCell.RowIndex).Cells("colOpenLOsID").Value)

        If loID <= 0 Then Exit Sub

        IsLoading = True
        Try
            CurrentLOID = loID

            ' 1) هيدر LO
            LoadLOHeader(loID)
            ' 🔑 تثبيت المخزن المحفوظ كالمخزن الحالي
            If dgvLOs.Rows.Count > 0 Then
                CurrentSelectedStoreID =
        If(dgvLOs.Rows(0).Cells("colLOsStoreID").Value Is DBNull.Value,
           0,
           CInt(dgvLOs.Rows(0).Cells("colLOsStoreID").Value))
            End If

            ' 2) SRs المرتبطة
            LoadSRsForLO(loID)

            ' 3) تفاصيل التحميل
            IsSaved = True
            LoadSRDDetailsForLO(loID)
            ApplyEditPolicyByLoadingStatus(loID)
        Finally
            IsLoading = False
        End Try
    End Sub

    Private Sub LoadSRsForLO(loID As Integer)

        dgvLoadingSR.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sqlExtraFilterForLOD As String = ""

            If CurrentMode = LoadingBoardMode.InvoiceSelection Then
                sqlExtraFilterForLOD = "
      AND ISNULL(LOD.LoadedQty,0) > 0
      AND NOT EXISTS (
            SELECT 1
            FROM Inventory_DocumentDetails IDD
            INNER JOIN Inventory_DocumentHeader H
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
            FROM Inventory_DocumentDetails IDD
            WHERE IDD.SourceLoadingOrderDetailID = LOD.LoadingOrderDetailID
      )"
            End If

            Dim sql As String = "
SELECT
    SR.SRID,
    SR.SRCode,
    P.PartnerName,
    CAST(SR.SRDate AS date) AS SRDateOnly
FROM dbo.Business_SR SR
LEFT JOIN dbo.Master_Partner P
    ON P.PartnerID = SR.PartnerID
WHERE EXISTS (
    SELECT 1
    FROM dbo.Logistics_LoadingOrderDetail LOD
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

            If CurrentMode = LoadingBoardMode.InvoiceSelection Then
                sqlNotExistsForThisLOD = "
AND ISNULL(LOD.LoadedQty,0) > 0
AND NOT EXISTS (
    SELECT 1
    FROM Inventory_DocumentDetails IDD
    INNER JOIN Inventory_DocumentHeader H
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
    FROM Inventory_DocumentDetails IDD
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
        FROM dbo.Logistics_LoadingOrderDetail d2
        WHERE d2.SourceDetailID = LOD.SourceDetailID
          AND d2.LOID <> @LOID
    ),0) AS LoadedBefore,

    ISNULL((
        SELECT
            IB.QtyOnHand - ISNULL(SUM(IR.ReservedQty),0)
        FROM dbo.Inventory_Balance IB
        LEFT JOIN dbo.Inventory_Reservation IR
            ON IR.ProductID = IB.ProductID
           AND IR.SourceStoreID = IB.StoreID
           AND IR.ReservationStatusID = 1
        WHERE IB.StoreID = @StoreID
          AND IB.ProductID = LOD.ProductID
        GROUP BY IB.QtyOnHand
    ),0) AS AvailableQty

FROM dbo.Logistics_LoadingOrderDetail LOD
INNER JOIN dbo.Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID
INNER JOIN dbo.Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
INNER JOIN dbo.Business_SR SR
    ON SR.SRID = LOD.SourceHeaderID
LEFT JOIN dbo.Workflow_Status BS
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
FROM dbo.Logistics_LoadingOrderSR
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
INSERT INTO dbo.Logistics_LoadingOrderSR (LOID, SRID)
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
INSERT INTO dbo.Logistics_LoadingOrderDetail
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
FROM dbo.Business_SRD SRD
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
        '    LoadLOList()

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
    Private Sub dgvLoadingSR_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvLoadingSR.CellContentClick
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        If dgvLoadingSR.Columns(e.ColumnIndex).Name <> "colLoadingSRtoInvoice" Then Exit Sub


        ' جرّب نجبر الحفظ
        dgvLoadingSR.CommitEdit(DataGridViewDataErrorContexts.Commit)
        If CurrentMode <> LoadingBoardMode.InvoiceSelection Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If dgvLoadingSR.Columns(e.ColumnIndex).Name <> "colLoadingSRtoInvoice" Then Exit Sub
        dgvLoadingSR.CommitEdit(DataGridViewDataErrorContexts.Commit)

        Dim clickedRow = dgvLoadingSR.Rows(e.RowIndex)

        Dim isChecked As Boolean =
        CBool(If(clickedRow.Cells("colLoadingSRtoInvoice").Value, False))

        ' إذا المستخدم يحاول التحديد
        If isChecked Then

            ' 🔒 إلغاء أي تحديد سابق فورًا
            For Each r As DataGridViewRow In dgvLoadingSR.Rows
                If r Is clickedRow Then Continue For
                r.Cells("colLoadingSRtoInvoice").Value = False
            Next

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
                    ' 1) إعادة حالة الطلبات إلى NOT_LOADED (0)
                    ' 1) إعادة حالة الطلبات إلى الحالة الابتدائية (BusinessStatusID = 4)
                    Using cmd0 As New SqlCommand("
UPDATE SRD
SET BusinessStatusID = 4
FROM dbo.Business_SRD SRD
INNER JOIN dbo.Logistics_LoadingOrderDetail LOD
    ON LOD.SourceDetailID = SRD.SRDID
WHERE LOD.LOID = @LOID
", con, tran)
                        cmd0.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd0.ExecuteNonQuery()
                    End Using

                    ' 2) حذف تفاصيل التحميل
                    Using cmd1 As New SqlCommand("
DELETE FROM dbo.Logistics_LoadingOrderDetail
WHERE LOID = @LOID
", con, tran)
                        cmd1.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd1.ExecuteNonQuery()
                    End Using

                    ' 3) حذف ربط الطلبات
                    Using cmd2 As New SqlCommand("
DELETE FROM dbo.Logistics_LoadingOrderSR
WHERE LOID = @LOID
", con, tran)
                        cmd2.Parameters.AddWithValue("@LOID", CurrentLOID)
                        cmd2.ExecuteNonQuery()
                    End Using

                    ' 4) حذف هيدر أمر التحميل (بدون شرط حالة)
                    Using cmd3 As New SqlCommand("
DELETE FROM dbo.Logistics_LoadingOrder
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
        ' ✅ مهم جداً: الحدث قد يُستدعى أثناء InitializeComponent أو تغيير عناوين الأعمدة
        If IsLoading Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If e.RowIndex >= dgvLoadingSRD.Rows.Count Then Exit Sub

        Dim row = dgvLoadingSRD.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim colName = dgvLoadingSRD.Columns(e.ColumnIndex).Name
        If colName <> "colLoadingSRDLoadedInThisLO" Then Exit Sub

        ' ✅ منع تعديل الكمية إلا في NEW أو IN_LOADEDING
        Dim st As Integer = GetLoadingStatusID(CurrentLOID)
        If Not (st = 2 OrElse st = 14) Then
            IsLoading = True
            Try
                row.Cells("colLoadingSRDLoadedInThisLO").Value = 0D
            Finally
                IsLoading = False
            End Try
            Exit Sub
        End If

        ' ... ثم كمل حساباتك الحالية (requiredQty, remaining, IsDirty...)
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
            FROM dbo.Inventory_Balance IB
            LEFT JOIN dbo.Inventory_Reservation IR
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



    Private Sub btnPostLoading_Click(sender As Object, e As EventArgs) Handles btnPostLoading.Click

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
        BuildPostConfirmationData(CurrentLOID)

        If frm.ShowDialog() <> DialogResult.OK Then Exit Sub


        '====================================================
        ' 5) تنفيذ الترحيل
        '====================================================
        Try

            Dim repo As New InventoryRepository(ConnStr)
            repo.PostLoadingOrder(CurrentLOID, CurrentUser.EmployeeID)


            MessageBox.Show("تم الترحيل بنجاح",
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
    Private Function BuildPostConfirmationData(loID As Integer) As DataTable

        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String = "
     SELECT
    SRD.ProductCode,
    SUM(LOD.LoadedQty) AS TotalQty,
    P.PartnerName,
    E.EmpName AS DriverName
FROM Logistics_LoadingOrderDetail LOD
INNER JOIN Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID
INNER JOIN Business_SR SR
    ON SR.SRID = SRD.SRID
INNER JOIN Master_Partner P
    ON P.PartnerID = SR.PartnerID
INNER JOIN Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID
INNER JOIN Security_Employee E
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
            FROM Logistics_LoadingOrderDetail
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
            FROM Logistics_LoadingOrderDetail LOD
            INNER JOIN Business_SR SR
                ON SR.SRID = LOD.SourceHeaderID
            INNER JOIN Master_Partner P
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
            FROM Logistics_LoadingOrder LO
            INNER JOIN Security_Employee E
                ON E.EmployeeID = LO.DriverEmployeeID
            WHERE LO.LOID = @LOID
        ", con)

                cmd.Parameters.AddWithValue("@LOID", loID)

                Dim result = cmd.ExecuteScalar()
                Return If(result Is Nothing, "", result.ToString())
            End Using
        End Using

    End Function

    Private Sub btnExportToInvoice_Click(
    sender As Object,
    e As EventArgs
) Handles btnExportToInvoice.Click

        Dim selectedSRID As Integer = 0

        For Each r As DataGridViewRow In dgvLoadingSR.Rows
            If CBool(If(r.Cells("colLoadingSRtoInvoice").Value, False)) Then
                selectedSRID = CInt(r.Cells("colLoadingSRID").Value)
                Exit For
            End If
        Next

        If selectedSRID <= 0 Then
            MessageBox.Show("يرجى اختيار طلب واحد فقط للفوترة.", "تنبيه")
            Exit Sub
        End If

        If CurrentLOID <= 0 Then
            MessageBox.Show("لم يتم تحديد أمر التحميل.", "تنبيه")
            Exit Sub
        End If

        ' ✅ Snapshot: ModifiedAt لحظة التصدير
        Dim modAtObj As Object = Nothing
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
SELECT ModifiedAt
FROM dbo.Logistics_LoadingOrder
WHERE LOID = @LOID
", con)
                cmd.Parameters.AddWithValue("@LOID", CurrentLOID)
                modAtObj = cmd.ExecuteScalar()
            End Using
        End Using

        If modAtObj Is Nothing OrElse IsDBNull(modAtObj) Then
            SelectedLOModifiedAt = Nothing
        Else
            SelectedLOModifiedAt = CDate(modAtObj)
        End If

        Me.SelectedLOID = CurrentLOID
        Me.SelectedSRID = selectedSRID

        Me.Close()

    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

    End Sub


    Private Sub ApplyUIMode()


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
FROM dbo.Logistics_LoadingOrder LO
INNER JOIN dbo.Workflow_Status S
    ON S.StatusID = LO.LoadingStatusID
LEFT JOIN dbo.Master_Vehicle V
    ON V.VehicleID = LO.VehicleID
LEFT JOIN dbo.Security_Employee E
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
FROM dbo.Logistics_LoadingOrder
WHERE LOID = @LOID
", con)
                cmd.Parameters.AddWithValue("@LOID", loID)
                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0
                Return CInt(obj)
            End Using
        End Using
    End Function

    Private Sub ApplyEditPolicyByLoadingStatus(loID As Integer)

        If loID <= 0 Then Exit Sub
        If IsLoading Then Exit Sub

        ' 1) Mode override
        If CurrentMode = LoadingBoardMode.ViewOnly Then
            dgvLOs.ReadOnly = True
            dgvLoadingSR.ReadOnly = True
            colLoadingSRtoInvoice.ReadOnly = False
            dgvLoadingSRD.ReadOnly = True

            btnSaveLO.Enabled = False
            btnPostLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnRemoveSR.Enabled = False
            btnExportToInvoice.Enabled = False
            Exit Sub
        End If

        If CurrentMode = LoadingBoardMode.InvoiceSelection Then

            dgvLOs.ReadOnly = True
            dgvLoadingSRD.ReadOnly = True

            ' ✅ مهم: الجريد ليس ReadOnly (حتى يعمل التشيك)
            dgvLoadingSR.ReadOnly = False

            ' ✅ اقفل كل الأعمدة ما عدا عمود التشيك فقط
            For Each col As DataGridViewColumn In dgvLoadingSR.Columns
                col.ReadOnly = (col.Name <> "colLoadingSRtoInvoice")
            Next

            ' ✅ اجعل النقر على التشيك يشتغل مباشرة
            dgvLoadingSR.EditMode = DataGridViewEditMode.EditOnEnter

            btnSaveLO.Enabled = False
            btnPostLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnRemoveSR.Enabled = False

            Exit Sub
        End If

        ' 2) Status-based policy (Normal mode)
        Dim statusID As Integer = GetLoadingStatusID(loID)

        Dim fullEdit As Boolean = (statusID = 2 OrElse statusID = 14)  ' NEW, IN_LOADEDING
        Dim headerOnly As Boolean = (statusID = 15)                    ' WAITING_INVOICE

        If fullEdit Then
            dgvLOs.ReadOnly = False
            dgvLoadingSR.ReadOnly = False
            dgvLoadingSRD.ReadOnly = False

            ' اقفل كل أعمدة SRD عدا الكمية
            For Each col As DataGridViewColumn In dgvLoadingSRD.Columns
                col.ReadOnly = (col.Name <> "colLoadingSRDLoadedInThisLO")
            Next

            btnSaveLO.Enabled = True
            btnPostLoading.Enabled = True
            btnAddSelectedSRToLO.Enabled = True
            btnRemoveSR.Enabled = True
            Exit Sub
        End If

        If headerOnly Then
            dgvLoadingSR.ReadOnly = True
            colLoadingSRtoInvoice.ReadOnly = False

            dgvLoadingSRD.ReadOnly = True

            dgvLOs.ReadOnly = False
            For Each c As DataGridViewColumn In dgvLOs.Columns
                c.ReadOnly = True
            Next

            dgvLOs.Columns("colLOsDriverCode").ReadOnly = False
            dgvLOs.Columns("colLOsSupervisor").ReadOnly = False
            dgvLOs.Columns("colLOsVehicale").ReadOnly = False
            dgvLOs.Columns("colLOsNote").ReadOnly = False
            If dgvLOs.Columns.Contains("colLOsStoreID") Then dgvLOs.Columns("colLOsStoreID").ReadOnly = True

            btnSaveLO.Enabled = True
            btnPostLoading.Enabled = False
            btnAddSelectedSRToLO.Enabled = False
            btnRemoveSR.Enabled = False
            Exit Sub
        End If

        ' No edit
        dgvLOs.ReadOnly = True
        dgvLoadingSR.ReadOnly = True
        colLoadingSRtoInvoice.ReadOnly = False

        dgvLoadingSRD.ReadOnly = True

        btnSaveLO.Enabled = False
        btnPostLoading.Enabled = False
        btnAddSelectedSRToLO.Enabled = False
        btnRemoveSR.Enabled = False
        If CurrentMode = LoadingBoardMode.InvoiceSelection Then

            ' ... (التعديلات التي تعملها على ReadOnly للأعمدة)


            Exit Sub
        End If

    End Sub

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

End Class
