Imports System.Data.SqlClient

Public Class frmSRDistripution
    Inherits AABaseOperationForm
    ' ============================================
    ' FORM WORKFLOW CONTEXT
    ' ============================================
    Private Const FORM_SCOPE As String = "LOD"
    Private Const FORM_OPERATION_TYPE_ID As Integer = 4   ' LOD من جدول OperationType
    Private ExcludeLinkedToLoadingOrder As Boolean = False
    Private RequireOpenLoadingOrder As Boolean = False
    Private RequiredLoadingStatusID As Integer? = Nothing
    Private FilterBusinessStatus_In As List(Of Integer) = Nothing
    Private FilterBusinessStatus_NotIn As List(Of Integer) = Nothing

    Private FilterLoadingStatus_In As List(Of Integer) = Nothing
    Private FilterLoadingStatus_NotIn As List(Of Integer) = Nothing
    Protected Overrides ReadOnly Property FormScopeCode As String
        Get
            Return "LOD"
        End Get
    End Property
    ' ============================================
    ' RELATED ENTITY (BUS) CONTEXT
    ' ============================================
    Private ReadOnly BUS_OPERATION_TYPE_ID As Integer =
    Workflow_OperationPolicyHelper.GetOperationTypeIDByCode("BUS")

    ' ============================================
    ' READ CURRENT EDIT POLICY FOR THIS FORM
    ' ============================================
    Private Function GetCurrentEditPolicy(
    entityID As Integer
) As EditPolicy

        Dim statusID As Integer

        ' 1) جلب الحالة الحالية حسب Scope الفورم
        Workflow_OperationPolicyHelper.GetEntityStatusByScope(
        FORM_SCOPE,
        entityID,
        statusID
    )

        ' 2) جلب سياسة التعديل
        Return Workflow_OperationPolicyHelper.GetEditPolicy(
        FORM_OPERATION_TYPE_ID,
        statusID
    )

    End Function


    Private IsLoading As Boolean = False
    ' =========================
    ' الحالة التجارية الحالية المعروضة في الفورم
    ' =========================
    ' =========================
    ' هل فلترة التاريخ مفعلة؟
    ' =========================
    Private IsDateFilterEnabled As Boolean = False
    Private CurrentLOID As Integer = 0
    ' يمنع إعادة الدخول أثناء تحرير الجريد
    Private IsReloadingLoadingSRD As Boolean = False
    ' يحدد هل المستخدم في وضع إضافة SR إلى LO قديم
    Private IsAddSRToExistingLO As Boolean = False
    ' يسمح بالتعديل الإداري حتى لو كان CLOSED (تشطيبات لاحقًا)
    Private CurrentBusinessFilterIDs As List(Of Integer) = Nothing
    Private PendingSRIDForLoading As Integer = 0
    Private CurrentSRID As Integer = 0
    Private IsTempLO As Boolean = False
    Private IsDraftLO As Boolean = False
    Private CurrentCaseName As String = "New_Orders"
    Private CurrentStatusFilterIDs As New List(Of Integer)


    Private Enum LoadingDirtyState
        None = 0
        HeaderChanged = 1
        QuantitiesChanged = 2
    End Enum
    ' ============================================
    ' TEMP UI REFRESH ENUM
    ' Purpose: eliminate compile errors and run forms
    ' ============================================
    Public Enum RefreshReason
        None = 0
        AfterSave = 1
        AfterPost = 2
        TabChanged = 3
        SelectionChanged = 4
        InitialLoad = 6
        LOSelected = 7
    End Enum

    Private CurrentDirtyState As LoadingDirtyState = LoadingDirtyState.None
    Private Enum DistributionMode
        SalesRequest = 0
        Invoice = 1
    End Enum

    Private CurrentMode As DistributionMode = DistributionMode.SalesRequest




    ' =====================================================
    ' تحديث جميع الجريدات حسب الحالة الحالية
    ' تُستدعى بعد أي عملية نقل / حفظ / إلغاء
    ' =====================================================
    Private Sub LoadStoresForSRD()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Using da As New SqlClient.SqlDataAdapter("
            SELECT
                StoreID AS StoreID,
                StoreName
            FROM Master_Store
            WHERE IsActive = 1
            ORDER BY StoreName
        ", con)

                Dim dt As New DataTable
                da.Fill(dt)


            End Using
        End Using

    End Sub


    ' =====================================================
    ' تطبيق قواعد تفعيل / تعطيل الأزرار حسب الحالة الحالية
    ' =====================================================


    Private Sub dptDate_ValueChanged(
    sender As Object,
    e As EventArgs
)

        If IsLoading Then Exit Sub

        IsDateFilterEnabled = True
        LoadSRList()
        dgvSR.ClearSelection()

    End Sub



    Private Sub frmSRDistripution_Load(
    sender As Object,
    e As EventArgs
) Handles MyBase.Load
        Me.AutoScaleMode = AutoScaleMode.None
        Me.StartPosition = FormStartPosition.Manual

        Dim r As Rectangle = Screen.FromControl(Me).WorkingArea

        Me.Width = CInt(r.Width * 0.95)
        Me.Height = CInt(r.Height * 0.95)

        Me.Left = r.Left + (r.Width - Me.Width) \ 2
        Me.Top = r.Top + (r.Height - Me.Height) \ 2
        IsLoading = True

        dgvSR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        dgvSRD.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        dgvSR.RowHeadersVisible = False
        dgvSR.AllowUserToResizeRows = False
        dgvSR.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSR.MultiSelect = False
        dgvSR.MultiSelect = True
        dgvSR.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        ' =========================
        ' تحميل البيانات الأساسية
        ' =========================
        LoadStoresForSRD()          ' ✅ إجباري
        LoadBusinessStatus_NewOnly()
        ApplyCentralDistributionCase(tbcSRDistribution.SelectedTab.Name)
        LoadSRList()


        dgvSR.ClearSelection()
        dgvSR.CurrentCell = Nothing
        dgvSRD.Rows.Clear()
        CurrentSRID = 0

        IsLoading = False

        ' =========================
        ' تحميل شاشة التوزيع
        ' =========================
        CurrentSRID = 0


        Debug.WriteLine("AFTER LOAD - CurrentLOID = " & CurrentLOID)

        ' =========================
        ' تحميل أوامر التحميل المفتوحة
        ' =========================

        ' =========================
        ' اختيار أول LO مفتوح (إن وجد)
        ' ⚠️ لا نعيّن CurrentLOID يدويًا
        ' =========================

        ' =========================
        ' تحميل Combo حالة الإنجاز
        ' =========================

        ' =========================
        ' تحديد مخزن افتراضي مرة واحدة
        ' =========================

    End Sub
    Private Sub LoadBusinessStatus_NewOnly()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
            SELECT
                StatusID,
                StatusCode,
                StatusName
            FROM Workflow_Status
            ORDER BY StatusID
        ", con)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    If dt.Rows.Count = 0 Then
                        MessageBox.Show(
                        "لا توجد حالات مناسبة لعرضها في شاشة التوزيع.",
                        "خطأ في الإعداد",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                        Me.Close()
                        Exit Sub
                    End If

                End Using
            End Using
        End Using

    End Sub
    Private Sub btnSRDistrClose_Click(
    sender As Object,
    e As EventArgs
) Handles btnCloseDistribution.Click

        Me.Close()

    End Sub
    ' فلاج لمنع الاستدعاءات المتداخلة
    Private IsReloadingSRD As Boolean = False



    Private Sub dgvLoadingSRD_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
)

        If IsLoading Then Exit Sub


        ' لا نعتبره تعديل إلا إذا المستخدم أدخل قيمة حقيقية

        CurrentDirtyState = CurrentDirtyState Or LoadingDirtyState.QuantitiesChanged


    End Sub
    ' =====================================================
    ' تحميل كمبوهات أمر التحميل (سائق - مشرف - مخزن - سيارة)
    ' =====================================================

    ' =====================================================
    ' تحميل مصادر بيانات ComboBox Columns داخل dgvLOs
    ' =====================================================
    ' =====================================================
    ' تحميل مصادر DataGridViewComboBoxColumn حسب الجداول الفعلية
    ' - DriverName و LoadingSupervisor و Store و VehicleInfo كلها نصوص في LoadingOrder
    ' =====================================================
    ' =====================================================
    ' حفظ تعديلات أمر التحميل (Header فقط)
    ' بدون تغيير الحالة (تبقى OPEN)
    ' =====================================================





    Private Sub dgvLOs_CellValueChanged(
    sender As Object,
    e As DataGridViewCellEventArgs
)


        CurrentDirtyState = CurrentDirtyState Or LoadingDirtyState.HeaderChanged

    End Sub
    '-------------------------------------------------------------------------------------------------------------

    Private Sub Execute_MoveSR_ToNextState(
    srID As Integer,
    targetBusinessState As String
)
        ' -------------------------
        ' (1) تحقق مبدئي من المدخلات
        ' -------------------------
        If srID <= 0 Then
            Throw New ArgumentException("SRID غير صالح.")
        End If

        If String.IsNullOrWhiteSpace(targetBusinessState) Then
            Throw New ArgumentException("الحالة المستهدفة غير محددة.")
        End If

        ' -------------------------
        RefreshDistributionContext(RefreshReason.AfterSave)

    End Sub
    Public Sub Execute_StartLoadingForSR(
    srID As Integer,
    Optional existingLOID As Integer = 0,
    Optional executedByEmployeeID As Integer = 0
)

        ' -------------------------
        ' (1) تحقق مبدئي
        ' -------------------------
        If srID <= 0 Then
            Throw New ArgumentException("SRID غير صالح.")
        End If

        If existingLOID < 0 Then
            Throw New ArgumentException("LOID غير صالح.")
        End If

        ' -------------------------
        ' (2) استدعاء طبقة التطبيق (غير موجودة بعد)
        ' -------------------------
        ' مثال (سيُبنى لاحقًا):
        '
        ' If existingLOID = 0 Then
        '     LoadingApplicationService.CreateNewLOForSR(
        '         srID,
        '         AppSession.LoggedEmployeeID
        '     )
        ' Else
        '     LoadingApplicationService.AddSRToExistingLO(
        '         srID,
        '         existingLOID,
        '         AppSession.LoggedEmployeeID
        '     )
        ' End If
        '

        ' -------------------------
        ' (3) تحديث السياق
        ' -------------------------
        RefreshDistributionContext(RefreshReason.AfterSave)

    End Sub
    Private Sub Execute_UpdateLoadingOrderHeader(
    loID As Integer,
    driverEmployeeID As Integer?,
    supervisorEmployeeID As Integer?,
    vehicleID As Integer?,
    sourceStoreID As Integer?,
    notes As String
)

        ' =====================================================
        ' Use Case: Update Loading Order Header
        '
        ' هذه الدالة مسؤولة فقط عن:
        ' - تحديث بيانات الهيدر لأمر التحميل
        ' - طالما أن أمر التحميل ما زال قابلًا للتعديل
        '
        ' ملاحظات مهمة:
        ' - لا تحتوي SQL
        ' - لا تتحقق من الحالات هنا
        ' - لا تغير FulfillmentStatus أو BusinessStatus
        ' - لا تتعامل مع الجريدات
        '
        ' أي تحقق أو منع تعديل سيتم داخل Application Service
        ' =====================================================

        ' -------------------------
        ' (1) تحقق مبدئي
        ' -------------------------
        If loID <= 0 Then
            Throw New ArgumentException("LOID غير صالح.")
        End If

        ' القيم Nullable مسموحة (قد لا يحدد المستخدم كل شيء)
        ' لا نتحقق من وجودها هنا

        ' -------------------------
        ' (2) استدعاء طبقة التطبيق (غير موجودة بعد)
        ' -------------------------
        ' مثال (سيُبنى لاحقًا):
        '
        ' LoadingApplicationService.UpdateHeader(
        '     loID,
        '     driverEmployeeID,
        '     supervisorEmployeeID,
        '     vehicleID,
        '     sourceStoreID,
        '     notes,
        '     AppSession.LoggedEmployeeID
        ' )
        '

        ' -------------------------
        ' (3) تحديث السياق (عرض فقط)
        ' -------------------------
        RefreshDistributionContext(RefreshReason.AfterSave)

    End Sub
    Private Sub Execute_SaveLoadingQuantities(
    loID As Integer,
    loadingLines As IEnumerable(Of LoadingLineInput)
)

        ' =====================================================
        ' Use Case: Save Loading Quantities (Details)
        '
        ' هذه الدالة:
        ' - تحفظ كميات التحميل المدخلة من المستخدم
        ' - لا تخصم مخزون
        ' - لا ترحّل
        ' - لا تغيّر حالات LO أو SR
        '
        ' المسموح:
        ' - إدراج / تحديث كميات مؤقتة أثناء العمل
        ' - إعادة الحفظ عدة مرات
        '
        ' الممنوع:
        ' - أي قرار أعمال
        ' - أي SQL
        ' - أي تعامل مباشر مع الجريدات
        ' =====================================================

        ' -------------------------
        ' (1) تحقق مبدئي
        ' -------------------------
        If loID <= 0 Then
            Throw New ArgumentException("LOID غير صالح.")
        End If

        If loadingLines Is Nothing Then
            Throw New ArgumentException("لا توجد بنود تحميل.")
        End If

        ' -------------------------
        ' (2) تحقق شكلي على المدخلات
        ' -------------------------
        For Each line In loadingLines

            If line Is Nothing Then
                Throw New ArgumentException("سطر تحميل غير صالح.")
            End If

            If line.SRDID <= 0 Then
                Throw New ArgumentException("SRDID غير صالح.")
            End If

            If line.LoadedQty < 0D Then
                Throw New ArgumentException("الكمية المحملة لا يمكن أن تكون سالبة.")
            End If

            ' الكمية = 0 مسموحة (تعني تجاهل السطر)
        Next

        ' -------------------------
        ' (3) استدعاء طبقة التطبيق (غير موجودة بعد)
        ' -------------------------
        ' مثال (سيُبنى لاحقًا):
        '
        ' LoadingApplicationService.SaveLoadingDetails(
        '     loID,
        '     loadingLines,
        '     AppSession.LoggedEmployeeID
        ' )
        '
        CurrentDirtyState = LoadingDirtyState.None

        ' -------------------------
        ' (4) تحديث السياق (عرض فقط)
        ' -------------------------
        RefreshDistributionContext(RefreshReason.AfterSave)

    End Sub
    Private Sub Execute_ReverseLoadingOrder(
    loID As Integer,
    reason As String
)

        ' =====================================================
        ' Use Case: Reverse Loading Order
        '
        ' هذا الاستخدام:
        ' - يعكس أمر تحميل تم ترحيله أو تجاوزه
        ' - يعيد المخزون
        ' - يعيد فتح الحجوزات أو يلغيها
        ' - يعيد الحالات إلى ما قبل الترحيل (حسب السياسة)
        '
        ' ⚠️ ملاحظات حرجة:
        ' - لا يوجد SQL هنا
        ' - لا يوجد منطق حالات
        ' - لا يوجد خصم / إضافة مخزون يدوي
        '
        ' هذا استخدام إداري عالي الخطورة
        ' =====================================================

        ' -------------------------
        ' (1) تحقق مبدئي
        ' -------------------------
        If loID <= 0 Then
            Throw New ArgumentException("LOID غير صالح.")
        End If

        If String.IsNullOrWhiteSpace(reason) Then
            Throw New ArgumentException("سبب عكس أمر التحميل مطلوب.")
        End If

        ' -------------------------
        ' (2) استدعاء طبقة التطبيق (غير موجودة بعد)
        ' -------------------------
        ' مثال (سيُبنى لاحقًا):
        '
        ' LoadingApplicationService.ReverseLoadingOrder(
        '     loID,
        '     reason,
        '     AppSession.LoggedEmployeeID
        ' )
        '
        ' ملاحظة:
        ' - الخدمة هي من:
        '   * تتحقق هل العكس مسموح
        '   * تمنع العكس إن وُجدت فواتير
        '   * تعكس الخصم
        '   * تعيد الحالة الصحيحة
        '

        ' -------------------------
        ' (3) تحديث السياق بعد العكس
        ' -------------------------
        RefreshDistributionContext(RefreshReason.AfterPost)
    End Sub
    Private Sub btnMoveSR_Click(sender As Object, e As EventArgs) Handles btnMoveSR.Click

        If dgvSR.SelectedRows.Count = 0 Then
            MessageBox.Show("يجب اختيار طلب واحد على الأقل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If


        Using frm As New frmMoveSR()
            frm.ShowDialog(Me)
        End Using

    End Sub
    Public Sub MoveSelectedSR(newStatusID As Integer)
        UpdateBusinessStatusForSelectedSR(newStatusID)
        LoadSRList()
    End Sub
    Private Sub UpdateBusinessStatusForSelectedSR(newStatusID As Integer)

        If dgvSR.SelectedRows.Count = 0 Then Exit Sub

        Dim srIDs As New List(Of Integer)

        For Each r As DataGridViewRow In dgvSR.SelectedRows
            If r.IsNewRow Then Continue For
            srIDs.Add(CInt(r.Cells("colSRID").Value))
        Next

        If srIDs.Count = 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String = "
UPDATE dbo.Business_SRD
SET BusinessStatusID = @NewStatusID
WHERE SRID IN (" & String.Join(",", srIDs) & ")
  AND BusinessStatusID IN (2,3,4)
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@NewStatusID", newStatusID)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

    Private Function GetOpenLOForSR(srID As Integer) As Integer

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT TOP 1 LOS.LOID
FROM Logistics_LoadingOrderSR LOS
INNER JOIN Logistics_LoadingOrder LO
    ON LO.LOID = LOS.LOID
WHERE LOS.SRID = @SRID
  AND LO.LoadingStatusID NOT IN (
        SELECT StatusID
        FROM Workflow_Status
  )
ORDER BY LO.InitiatedDateTime DESC
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then Return 0

                Return CInt(obj)

            End Using
        End Using

    End Function
    Public Enum MoveLOResult
        Cancel
        UseExisting
        CreateNew
    End Enum

    Public Property Result As MoveLOResult = MoveLOResult.Cancel
    Public Property SelectedLOID As Integer = 0

    Private Sub btnStartLoading_Click(
    sender As Object,
    e As EventArgs
) Handles btnStartLoading.Click

        If dgvSR.SelectedRows.Count <> 1 Then
            MessageBox.Show("يجب اختيار سجل واحد فقط", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =====================================================
            ' وضع الفاتورة
            ' =====================================================


            ' =====================================================
            ' وضع الطلب (SR)
            ' =====================================================

            Dim srID As Integer =
            CInt(dgvSR.SelectedRows(0).Cells("colSRID").Value)


            ' =====================================================
            ' 1️⃣ تحقق من حالة الطلب
            ' =====================================================
            Using cmdCheckSR As New SqlCommand("
IF EXISTS (
    SELECT 1
    FROM dbo.Business_SRD
    WHERE SRID = @SRID
      AND BusinessStatusID IN (4,12)
)
    SELECT 0
ELSE
    SELECT 1
", con)

                cmdCheckSR.Parameters.AddWithValue("@SRID", srID)

                If CInt(cmdCheckSR.ExecuteScalar()) = 1 Then
                    MessageBox.Show("لا يوجد أي بند بحالة تسمح بالتحميل",
                                "تنبيه",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End Using


            ' =====================================================
            ' 2️⃣ هل يوجد LO مفتوح ؟
            ' =====================================================
            Dim existingLOID As Integer = 0
            Dim sameSRInOpenLO As Boolean = False

            Using cmdCheckLO As New SqlCommand("
SELECT TOP 1 LO.LOID,
       CASE WHEN EXISTS (
            SELECT 1
            FROM dbo.Logistics_LoadingOrderSR LOS
            WHERE LOS.LOID = LO.LOID
              AND LOS.SRID = @SRID
       )
       THEN 1 ELSE 0 END AS HasSameSR
FROM dbo.Logistics_LoadingOrder LO
WHERE LO.LoadingStatusID IN (1,2,14)
ORDER BY LO.LOID DESC
", con)

                cmdCheckLO.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmdCheckLO.ExecuteReader()
                    If rd.Read() Then
                        existingLOID = CInt(rd("LOID"))
                        sameSRInOpenLO = CInt(rd("HasSameSR")) = 1
                    End If
                End Using
            End Using


            ' =====================================================
            ' 3️⃣ إذا وجد LO مفتوح
            ' =====================================================
            If existingLOID > 0 Then

                If sameSRInOpenLO Then
                    Using frm As New frmLoadingBoard()
                        frm.FocusLOID = existingLOID
                        frm.IsNewLO = False
                        frm.ShowDialog(Me)
                    End Using
                Else
                    Using frm As New frmMoveLO()
                        frm.CurrentSRID = srID
                        frm.ExistingLOID = existingLOID
                        frm.ShowDialog(Me)
                    End Using
                End If

                Exit Sub
            End If


            ' =====================================================
            ' 4️⃣ إنشاء LO جديد
            ' =====================================================

            Dim newLOID As Integer

            Using tran = con.BeginTransaction()
                Try

                    ' 4.1 توليد كود LO
                    Dim nextLOCode As String

                    Using cmdCode As New SqlCommand("cfg.GetNextCode", con, tran)
                        cmdCode.CommandType = CommandType.StoredProcedure
                        cmdCode.Parameters.AddWithValue("@CodeType", "LOD")

                        Dim pNextCode As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                        pNextCode.Direction = ParameterDirection.Output
                        cmdCode.Parameters.Add(pNextCode)

                        cmdCode.ExecuteNonQuery()
                        nextLOCode = pNextCode.Value.ToString()
                    End Using


                    ' 4.2 جلب OperationTypeID للطلب (BUS)
                    Dim operationTypeID As Integer

                    Using cmdType As New SqlCommand("
SELECT TOP 1 OperationTypeID
FROM dbo.Workflow_OperationType
WHERE OperationCode = @Code
  AND IsActive = 1
", con, tran)

                        cmdType.Parameters.AddWithValue("@Code", "LOD")

                        Dim obj = cmdType.ExecuteScalar()

                        If obj Is Nothing OrElse IsDBNull(obj) Then
                            Throw New ApplicationException("OperationType (LOD) غير موجود أو غير مفعل في Workflow_OperationType.")
                        End If

                        operationTypeID = CInt(obj)
                    End Using



                    ' 4.3 إنشاء Header
                    Using cmdLO As New SqlCommand("
INSERT INTO dbo.Logistics_LoadingOrder
(LOCode, InitiatedDateTime, LoadingStatusID, CreatedAt, CreatedBy, OperationTypeID)
OUTPUT INSERTED.LOID
VALUES
(@LOCode, GETDATE(), 1, GETDATE(), @UserID, @OperationTypeID)
", con, tran)

                        cmdLO.Parameters.AddWithValue("@LOCode", nextLOCode)
                        cmdLO.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                        cmdLO.Parameters.AddWithValue("@OperationTypeID", operationTypeID)

                        newLOID = CInt(cmdLO.ExecuteScalar())
                    End Using


                    ' 4.4 ربط LO مع SR
                    Using cmdLOS As New SqlCommand("
INSERT INTO dbo.Logistics_LoadingOrderSR (LOID, SRID)
VALUES (@LOID, @SRID)
", con, tran)

                        cmdLOS.Parameters.AddWithValue("@LOID", newLOID)
                        cmdLOS.Parameters.AddWithValue("@SRID", srID)
                        cmdLOS.ExecuteNonQuery()
                    End Using


                    ' 4.5 نسخ التفاصيل
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
  AND SRD.BusinessStatusID <> 13
", con, tran)

                        cmdLOD.Parameters.AddWithValue("@LOID", newLOID)
                        cmdLOD.Parameters.AddWithValue("@SRID", srID)
                        cmdLOD.ExecuteNonQuery()
                    End Using


                    tran.Commit()

                Catch
                    tran.Rollback()
                    Throw
                End Try
            End Using


            ' =====================================================
            ' فتح لوحة التحميل
            ' =====================================================
            Using frm As New frmLoadingBoard()
                frm.FocusLOID = newLOID
                frm.IsNewLO = True
                frm.ShowDialog(Me)
            End Using

        End Using

    End Sub

    Private Function HasOpenLoadingOrder() As Boolean

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT CASE 
    WHEN EXISTS (
        SELECT 1
        FROM Logistics_LoadingOrder LO
        WHERE LO.LoadingStatusID NOT IN (
            SELECT StatusID
            FROM Workflow_Status
        )
    )
    THEN 1 ELSE 0
END
", con)

                Return (CInt(cmd.ExecuteScalar()) = 1)

            End Using
        End Using

    End Function



    Private Sub btnSaveLoading_Click(
    sender As Object,
    e As EventArgs
)

        If CurrentLOID <= 0 Then Exit Sub

        Dim lines As New List(Of LoadingLineInput)


        Execute_SaveLoadingQuantities(CurrentLOID, lines)

    End Sub

    Private Function GetNullableInt(value As Object) As Integer?
        If value Is Nothing OrElse IsDBNull(value) Then Return Nothing
        Return CInt(value)
    End Function


    Private Function CellIntOrDBNull(value As Object) As Object
        If value Is Nothing OrElse IsDBNull(value) Then
            Return DBNull.Value
        End If

        If Not IsNumeric(value) Then
            Return DBNull.Value
        End If

        Return CInt(value)
    End Function

    Private Sub RefreshDistributionContext(reason As RefreshReason)

        If IsLoading Then Exit Sub
        IsLoading = True

        Try
            Select Case reason

                Case RefreshReason.InitialLoad,
                 RefreshReason.TabChanged,
                 RefreshReason.AfterSave,
                 RefreshReason.AfterPost

                    ' إعادة تطبيق حالة العرض الحالية (تعتمد على StatusID من جدول Status)
                    ApplyCentralDistributionCase(CurrentCaseName)

                    ' تحميل القائمة الرئيسية حسب الفلترة الجديدة
                    LoadSRList()

                    ' تهيئة الواجهة
                    If tbcSRDistribution.SelectedTab IsNot Nothing AndAlso
                   tbcSRDistribution.SelectedTab.Name = "tabpSRNew" Then
                        ' ApplyUI_NewTab()
                    End If

                    ' إعادة ضبط الاختيارات
                    CurrentSRID = 0
                    dgvSR.ClearSelection()
                    dgvSR.CurrentCell = Nothing
                    dgvSRD.Rows.Clear()

                Case RefreshReason.LOSelected
                    ' لا يوجد إجراء حالياً

            End Select

        Finally
            IsLoading = False
        End Try

    End Sub



    Private Sub Execute_MoveSR_ToStatus(
    srID As Integer,
    targetStatusCode As String
)

        ' =====================================================
        ' Use Case: Move Sales Request To Specific Status
        '
        ' - UI Entry Point
        ' - No SQL
        ' - No business logic
        ' - Uses StatusCode only
        ' =====================================================

        If srID <= 0 Then
            Throw New ArgumentException("SRID غير صالح.")
        End If

        If String.IsNullOrWhiteSpace(targetStatusCode) Then
            Throw New ArgumentException("StatusCode غير صالح.")
        End If

        ' التنفيذ الفعلي (لاحقًا)
        ' SalesRequestApplicationService.MoveToStatus(
        '     srID,
        '     targetStatusCode,
        '     AppSession.LoggedEmployeeID
        ' )

        ' تحديث الواجهة
        RefreshDistributionContext(RefreshReason.AfterSave)

    End Sub

    Private Sub dgvSR_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSR.CellClick

        ' تأكد أن الضغط على صف فعلي
        If e.RowIndex < 0 Then Exit Sub
        If dgvSR.SelectedRows.Count > 1 Then Exit Sub

        Dim srID As Integer =
        CInt(dgvSR.Rows(e.RowIndex).Cells("colSRID").Value)

        If srID <= 0 Then Exit Sub

        ' لا نمنع التحميل إذا كان التحديد جاء من تغيير Tab
        '       If srID = CurrentSRID AndAlso dgvSR.SelectedRows.Count > 0 Then Exit Sub

        CurrentSRID = srID

        ' تحميل التفاصيل فقط عند ضغط المستخدم
        If CurrentMode = DistributionMode.Invoice Then

            LoadInvoiceDetails(CurrentSRID)

        Else

            LoadSRDetails(CurrentSRID)
            FillLoadedQtyForSRD(CurrentSRID)

        End If


    End Sub

    Private Sub frmSRDistripution_Shown(
    sender As Object,
    e As EventArgs
) Handles Me.Shown

        SetFormCentered95Percent()

    End Sub
    ' =========================================================
    ' الدالة المركزية الجديدة
    ' بديلة عن ApplyDistributionContext القديمة
    ' =========================================================
    Private Function GetStatusIDs(statusCodes As List(Of String)) As List(Of Integer)

        Dim result As New List(Of Integer)

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT StatusID
            FROM Workflow_Status
            WHERE StatusCode IN (" & String.Join(",", statusCodes.Select(Function(s) "'" & s & "'")) & ")
        ", con)

                con.Open()
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        result.Add(CInt(rd("StatusID")))
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function
    Private Sub LoadSRList()
        If CurrentMode = DistributionMode.Invoice Then Exit Sub

        dgvSR.Rows.Clear()
        dgvSRD.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =====================================================
            ' 1) Build Business Filter (SRD)
            ' =====================================================
            Dim businessClause As String = ""

            If FilterBusinessStatus_In IsNot Nothing AndAlso
       FilterBusinessStatus_In.Count > 0 Then

                businessClause &= "
AND SRD.BusinessStatusID IN (" &
        String.Join(",", FilterBusinessStatus_In) & ")"
            End If


            If FilterBusinessStatus_NotIn IsNot Nothing AndAlso
       FilterBusinessStatus_NotIn.Count > 0 Then

                businessClause &= "
AND SRD.BusinessStatusID NOT IN (" &
        String.Join(",", FilterBusinessStatus_NotIn) & ")"
            End If


            ' =====================================================
            ' 2) Build Loading Filter (مصحح)
            ' =====================================================
            Dim loadingClause As String = ""

            ' 🔴 إذا المطلوب فقط التي ليس لها LO
            If FilterLoadingStatus_In IsNot Nothing AndAlso
       FilterLoadingStatus_In.Count = 1 AndAlso
       FilterLoadingStatus_In(0) = -999 Then

                loadingClause &= "
AND LO.LOID IS NULL"

            Else

                If FilterLoadingStatus_In IsNot Nothing AndAlso
           FilterLoadingStatus_In.Count > 0 Then

                    loadingClause &= "
AND LO.LoadingStatusID IN (" &
            String.Join(",", FilterLoadingStatus_In) & ")"
                End If


                If FilterLoadingStatus_NotIn IsNot Nothing AndAlso
           FilterLoadingStatus_NotIn.Count > 0 Then

                    loadingClause &= "
AND (
        LO.LOID IS NULL
        OR LO.LoadingStatusID NOT IN (" &
            String.Join(",", FilterLoadingStatus_NotIn) & ")
    )"
                End If

            End If



            ' =====================================================
            ' 3) FINAL QUERY (SRD × LO)
            ' =====================================================
            Dim sql As String = "
SELECT
    SR.SRID,
    SR.SRCode,
    CAST(SR.SRDate AS date) AS SRDateOnly,
    P.PartnerName,
    E.EmpName,

    SRD.SRDID,
    SRD.ProductCode,
    SRD.ProductType,
    SRD.Quantity,
    SRD.BusinessStatusID,

    LO.LOID,
    LO.LOCode,
    LO.LoadingStatusID

FROM Business_SRD SRD

INNER JOIN Business_SR SR
    ON SR.SRID = SRD.SRID

LEFT JOIN Master_Partner P
    ON P.PartnerID = SR.PartnerID

LEFT JOIN Security_Employee E
    ON E.EmpCode = SR.SalesRepCode

LEFT JOIN Logistics_LoadingOrderDetail LOD
    ON LOD.SourceDetailID = SRD.SRDID

LEFT JOIN Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID

WHERE 1 = 1
" & businessClause & "
" & loadingClause & "

ORDER BY SR.SRDate DESC, SRD.SRDID
"

            Using cmd As New SqlCommand(sql, con)
                Using rd = cmd.ExecuteReader()

                    Dim addedSR As New HashSet(Of Integer)

                    While rd.Read()

                        Dim srID As Integer = CInt(rd("SRID"))

                        ' ============================
                        ' الجريد الرئيسي (SR)
                        ' ============================
                        If Not addedSR.Contains(srID) Then

                            dgvSR.Rows.Add(
                            srID,
                            rd("SRCode"),
                            rd("SRDateOnly"),
                            rd("PartnerName"),
                            rd("EmpName")
                        )

                            addedSR.Add(srID)
                        End If


                        ' ============================
                        ' جريد التفاصيل (SRD × LO)
                        ' ============================
                        ' ============================
                        ' جريد التفاصيل (SRD × LO)
                        ' ============================
                        ' ============================
                        ' جريد التفاصيل (SRD × LO)
                        ' ============================
                        Dim r As Integer = dgvSRD.Rows.Add()

                        dgvSRD.Rows(r).Cells("colSRDID").Value = rd("SRDID")
                        dgvSRD.Rows(r).Cells("colSRDProductCode").Value = rd("ProductCode")
                        dgvSRD.Rows(r).Cells("colSRDProductType").Value = rd("ProductType")
                        dgvSRD.Rows(r).Cells("colSRDQTY").Value = rd("Quantity")

                        dgvSRD.Rows(r).Cells("colLOCode").Value = If(IsDBNull(rd("LOCode")), "", rd("LOCode").ToString())
                        dgvSRD.Rows(r).Cells("colLoadingStatusID").Value = If(IsDBNull(rd("LoadingStatusID")), DBNull.Value, rd("LoadingStatusID"))

                        ' إذا تحب تعبئة الأسماء أيضاً (لو متاحة من الاستعلام) وإلا اتركها
                        ' dgvSRD.Rows(r).Cells("colSRDBusinessStatusID").Value = rd("BusinessStatusID")
                        ' dgvSRD.Rows(r).Cells("colSRDBusinessStatusName").Value = ...
                        ' dgvSRD.Rows(r).Cells("colLoadingStatusName").Value = ...

                    End While

                End Using
            End Using

        End Using

    End Sub

    Private Sub dgvSRD_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSRD.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub
        If tbcSRDistribution.SelectedTab Is Nothing Then Exit Sub
        If tbcSRDistribution.SelectedTab.Name <> "tabpSRWaitingInvoicesSRs" Then Exit Sub

        Dim row As DataGridViewRow = dgvSRD.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Exit Sub

        Dim loCode As String = ""
        If row.Cells("colLOCode").Value IsNot Nothing Then
            loCode = row.Cells("colLOCode").Value.ToString().Trim()
        End If

        If String.IsNullOrWhiteSpace(loCode) Then
            MessageBox.Show("هذا السطر لا يحتوي على أمر تحميل (LOCode فارغ).", "تنبيه")
            Exit Sub
        End If

        Dim loID As Integer = 0
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using cmd As New SqlCommand("
SELECT TOP 1 LOID
FROM dbo.Logistics_LoadingOrder
WHERE LOCode = @LOCode
", con)
                cmd.Parameters.AddWithValue("@LOCode", loCode)
                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    loID = CInt(obj)
                End If
            End Using
        End Using

        If loID <= 0 Then
            MessageBox.Show("لم يتم العثور على LOID لهذا الكود: " & loCode, "تنبيه")
            Exit Sub
        End If

        Using frm As New frmLoadingBoard()
            frm.FocusLOID = loID
            frm.CurrentMode = frmLoadingBoard.LoadingBoardMode.ViewOnly
            frm.ShowDialog(Me)
        End Using

        LoadSRList()

    End Sub
    Private Sub LoadSRDetails(srID As Integer)
        If CurrentMode = DistributionMode.Invoice Then Exit Sub

        dgvSRD.Rows.Clear()
        If srID <= 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =====================================================
            ' 1) Build Business Filter
            ' =====================================================
            Dim businessClause As String = ""

            If FilterBusinessStatus_In IsNot Nothing AndAlso
           FilterBusinessStatus_In.Count > 0 Then

                businessClause &= "
AND SRD.BusinessStatusID IN (" &
            String.Join(",", FilterBusinessStatus_In) & ")"
            End If

            If FilterBusinessStatus_NotIn IsNot Nothing AndAlso
           FilterBusinessStatus_NotIn.Count > 0 Then

                businessClause &= "
AND SRD.BusinessStatusID NOT IN (" &
            String.Join(",", FilterBusinessStatus_NotIn) & ")"
            End If


            ' =====================================================
            ' 2) Build Loading Filter (LEFT JOIN Safe)
            ' =====================================================
            Dim loadingClause As String = ""

            If FilterLoadingStatus_In IsNot Nothing AndAlso
           FilterLoadingStatus_In.Count > 0 Then

                loadingClause &= "
AND (
    LO.LOID IS NULL
    OR LO.LoadingStatusID IN (" &
    String.Join(",", FilterLoadingStatus_In) & ")
)"
            End If

            If FilterLoadingStatus_NotIn IsNot Nothing AndAlso
           FilterLoadingStatus_NotIn.Count > 0 Then

                loadingClause &= "
AND (
    LO.LOID IS NULL
    OR LO.LoadingStatusID NOT IN (" &
    String.Join(",", FilterLoadingStatus_NotIn) & ")
)"
            End If


            ' =====================================================
            ' 3) Final Query
            ' =====================================================
            Dim sql As String = "
SELECT
    SRD.SRDID,
    SRD.BusinessStatusID,
    BS.StatusName AS BusinessStatusName,
    SRD.ProductCode,
    SRD.ProductType,
    SRD.Quantity,
    SRD.Notes,

    LO.LOCode,
    LO.LoadingStatusID,
    LS.StatusName AS LoadingStatusName

FROM dbo.Business_SRD SRD

INNER JOIN dbo.Workflow_Status BS
    ON BS.StatusID = SRD.BusinessStatusID

LEFT JOIN dbo.Logistics_LoadingOrderDetail LOD
    ON LOD.SourceDetailID = SRD.SRDID

LEFT JOIN dbo.Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID

LEFT JOIN dbo.Workflow_Status LS
    ON LS.StatusID = LO.LoadingStatusID

WHERE SRD.SRID = @SRID
" & businessClause & "
" & loadingClause & "

ORDER BY SRD.SRDID, LO.LOCode
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()

                    While rd.Read()

                        Dim r As Integer = dgvSRD.Rows.Add()

                        dgvSRD.Rows(r).Cells("colSRDID").Value = rd("SRDID")
                        dgvSRD.Rows(r).Cells("colSRDBusinessStatusID").Value = rd("BusinessStatusID")
                        dgvSRD.Rows(r).Cells("colSRDBusinessStatusName").Value = rd("BusinessStatusName")
                        dgvSRD.Rows(r).Cells("colSRDProductCode").Value = rd("ProductCode")
                        dgvSRD.Rows(r).Cells("colSRDProductType").Value = rd("ProductType")
                        dgvSRD.Rows(r).Cells("colSRDQTY").Value = rd("Quantity")
                        dgvSRD.Rows(r).Cells("colSRDAvailableQTY").Value = 0D
                        dgvSRD.Rows(r).Cells("colSRDLoadedQty").Value = 0D
                        dgvSRD.Rows(r).Cells("colSRDNotes").Value =
                        If(rd("Notes") Is DBNull.Value, "", rd("Notes").ToString())

                        ' ============================
                        ' Loading Info (Safe NULL Handling)
                        ' ============================
                        If rd("LOCode") Is DBNull.Value Then

                            dgvSRD.Rows(r).Cells("colLOCode").Value = ""
                            dgvSRD.Rows(r).Cells("colLoadingStatusID").Value = DBNull.Value
                            dgvSRD.Rows(r).Cells("colLoadingStatusName").Value = ""

                        Else

                            dgvSRD.Rows(r).Cells("colLOCode").Value = rd("LOCode")
                            dgvSRD.Rows(r).Cells("colLoadingStatusID").Value = rd("LoadingStatusID")
                            dgvSRD.Rows(r).Cells("colLoadingStatusName").Value = rd("LoadingStatusName")

                        End If

                    End While

                End Using
            End Using

        End Using

        FillLoadedQtyForSRD(srID)

    End Sub
    Private Sub LoadInvoiceList()

        dgvSR.Rows.Clear()
        dgvSRD.Rows.Clear()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String = "
SELECT
    H.DocumentID,
    H.DocumentNo,
    CAST(H.DocumentDate AS date) AS DocDate,
    P.PartnerName,
    ST. StatusName
FROM dbo.Inventory_DocumentHeader H
LEFT JOIN Master_Partner P
    ON P.PartnerID = H.PartnerID
LEFT JOIN Workflow_Status ST
    ON ST.StatusID = H.StatusID
WHERE H.DocumentType = 'SAL'
ORDER BY H.DocumentDate DESC
"

            Using cmd As New SqlCommand(sql, con)
                Using rd = cmd.ExecuteReader()

                    While rd.Read()

                        Dim r As Integer = dgvSR.Rows.Add()

                        dgvSR.Rows(r).Cells("colSRID").Value = rd("DocumentID")
                        dgvSR.Rows(r).Cells("colSRCode").Value = rd("DocumentNo")
                        dgvSR.Rows(r).Cells("colSRDate").Value = rd("DocDate")
                        dgvSR.Rows(r).Cells("colSRPartnerID").Value = rd("PartnerName")
                        dgvSR.Rows(r).Cells("colSRNote").Value = rd("StatusName")

                    End While

                End Using
            End Using

        End Using

    End Sub
    Private Sub LoadInvoiceDetails(documentID As Integer)

        dgvSRD.Rows.Clear()
        If documentID <= 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Dim sql As String = "
SELECT
    D.DetailID,
    P.ProductCode,
    P.ProductName,
    D.Quantity,
    D.UnitID,
    PT.TypeName,
    U.UnitName
FROM dbo.Inventory_DocumentDetails D
INNER JOIN Master_Product P
    ON P. ProductID = D.ProductID
LEFT JOIN Master_ProductType PT
    ON PT.ProductTypeID = P.ProductTypeID
LEFT JOIN Master_Unit U
    ON  U.UnitID = D.UnitID
WHERE D.DocumentID = @DocumentID
ORDER BY D.DetailID
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@DocumentID", documentID)

                Using rd = cmd.ExecuteReader()

                    While rd.Read()

                        Dim r As Integer = dgvSRD.Rows.Add()

                        dgvSRD.Rows(r).Cells("colSRDID").Value = rd("DetailID")
                        dgvSRD.Rows(r).Cells("colSRDProductCode").Value = rd("ProductCode")
                        dgvSRD.Rows(r).Cells("colSRDProductType").Value = rd("TypeName")
                        dgvSRD.Rows(r).Cells("colSRDQTY").Value = rd("Quantity")
                        dgvSRD.Rows(r).Cells("colSRDNotes").Value = rd("UnitName")

                    End While

                End Using
            End Using

        End Using

    End Sub

    Private Sub ApplyCentralDistributionCase(caseName As String)
        FilterBusinessStatus_In = Nothing
        FilterBusinessStatus_NotIn = Nothing
        FilterLoadingStatus_In = Nothing
        FilterLoadingStatus_NotIn = Nothing
        Select Case caseName

            Case "tabpSRNew"
                FilterBusinessStatus_In = New List(Of Integer) From {2}
                FilterBusinessStatus_NotIn = Nothing
                FilterLoadingStatus_In = Nothing
                FilterLoadingStatus_NotIn = Nothing

            Case "tabpSRInCutting"
                FilterBusinessStatus_In = New List(Of Integer) From {3}
                FilterBusinessStatus_NotIn = Nothing
                FilterLoadingStatus_In = Nothing
                FilterLoadingStatus_NotIn = Nothing

            Case "tabpSRInLoading"
                FilterBusinessStatus_In = New List(Of Integer) From {4}
                FilterBusinessStatus_NotIn = Nothing
                FilterLoadingStatus_In = Nothing
                FilterLoadingStatus_NotIn = New List(Of Integer) From {10, 11, 14, 1, 2, 15}


            Case "tabpSROutLoading"
                FilterBusinessStatus_In = Nothing
                FilterBusinessStatus_NotIn = New List(Of Integer) From {10, 11}
                FilterLoadingStatus_In = New List(Of Integer) From {1, 2, 14}
                FilterLoadingStatus_NotIn = Nothing

            Case "tabPartialSRs"
                FilterBusinessStatus_In = New List(Of Integer) From {12}
                FilterBusinessStatus_NotIn = Nothing
                FilterLoadingStatus_In = Nothing
                FilterLoadingStatus_NotIn = New List(Of Integer) From {10, 11, 14}
            Case "tabpSRWaitingInvoicesSRs"
                FilterBusinessStatus_In = Nothing
                FilterBusinessStatus_NotIn = New List(Of Integer) From {1, 2, 3, 10, 11, 4}
                FilterLoadingStatus_In = New List(Of Integer) From {15}
                FilterLoadingStatus_NotIn = Nothing
            Case "tabInvoicedSRs"
                FilterBusinessStatus_In = New List(Of Integer) From {11}
                FilterBusinessStatus_NotIn = Nothing
                FilterLoadingStatus_In = Nothing
                FilterLoadingStatus_NotIn = Nothing

            Case "tabReturnLO"
                FilterBusinessStatus_In = Nothing
                FilterBusinessStatus_NotIn = New List(Of Integer) From {10, 11}
                FilterLoadingStatus_In = New List(Of Integer) From {9}
                FilterLoadingStatus_NotIn = Nothing


            Case Else

        End Select



    End Sub
    Private Sub tbcSRDistribution_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles tbcSRDistribution.SelectedIndexChanged

        If IsLoading Then Exit Sub
        If tbcSRDistribution.SelectedTab.Name = "tabNewInvoice" Then
            CurrentMode = DistributionMode.Invoice
        Else
            CurrentMode = DistributionMode.SalesRequest
        End If

        ApplyCentralDistributionCase(tbcSRDistribution.SelectedTab.Name)

        If CurrentMode = DistributionMode.Invoice Then
            LoadInvoiceList()
        Else
            LoadSRList()
        End If

        dgvSRD.Rows.Clear()

        ApplyCentralUI_ByTab()
    End Sub

    Private Sub btnOpenLoadingBoard_Click(
    sender As Object,
    e As EventArgs
) Handles btnOpenLoadingBoard.Click

        Dim f As New frmLoadingBoard()
        f.ShowDialog()

    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------
    'تحديث الجريدات عند التفعيل 
    '----------------------------------------------------------------------------------------------------------------------------------
    Private Sub frmMain_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        '   RefreshAllGrids()
    End Sub
    Private Sub RefreshAllGrids()
        LoadSRList()


    End Sub

    Private Sub SetColumnWidth(
    dgv As DataGridView,
    columnName As String,
    width As Integer,
    Optional visible As Boolean = True
)
        If Not dgv.Columns.Contains(columnName) Then Exit Sub
        With dgv.Columns(columnName)
            .Visible = visible
            If visible Then .Width = width
        End With
    End Sub
    Private Sub ApplyCentralUI_ByTab()

        ' =================================================
        ' (1) الأزرار – الافتراضي: كلها مفعلة
        ' =================================================
        btnMoveSR.Enabled = True
        btnStartLoading.Enabled = True
        btnOpenLoadingBoard.Enabled = True
        btnCloseSR.Enabled = True
        btnSRPrint.Enabled = True

        ' =================================================
        ' (2) الجريد الرئيسي dgvSR
        ' =================================================
        dgvSR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        SetColumnWidth(dgvSR, "colSRID", 70)
        SetColumnWidth(dgvSR, "colSRCode", 140)
        SetColumnWidth(dgvSR, "colSRDate", 110)
        SetColumnWidth(dgvSR, "colSRPartnerName", 200)
        SetColumnWidth(dgvSR, "colSREmpName", 160)
        SetColumnWidth(dgvSR, "colSRNotes", 300)

        ' =================================================
        ' (3) جريد التفاصيل dgvSRD
        ' =================================================
        dgvSRD.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

        SetColumnWidth(dgvSRD, "colSRDID", 70)
        SetColumnWidth(dgvSRD, "colSRDProductCode", 150)
        SetColumnWidth(dgvSRD, "colSRDProductType", 140)
        SetColumnWidth(dgvSRD, "colSRDQTY", 90)
        SetColumnWidth(dgvSRD, "colSRDAvailableQTY", 110)
        SetColumnWidth(dgvSRD, "colSRDLoadedQty", 110)
        SetColumnWidth(dgvSRD, "colSRDBusinessStatusName", 150)
        SetColumnWidth(dgvSRD, "colSRDFulfillmentStatusName", 160)
        SetColumnWidth(dgvSRD, "colSRDNotes", 260)

        ' =================================================
        ' (4) تخصيص حسب التاب (حالياً الكل True)
        ' عدّل لاحقاً أي زر أو عمود إلى False
        ' =================================================
        Select Case tbcSRDistribution.SelectedTab.Name

            Case "tabpSRNew"
            ' مثال مستقبلي:
            ' SetColumnWidth(dgvSRD, "colSRDLoadedQty", 0, False)
            ' btnStartLoading.Enabled = False

            Case "tabpSRInCutting"

            Case "tabpSRInLoading"

            Case "tabpSROutLoading"

            Case "tabPartialSRs"

            Case "tabWaitingInvoicesSRS"

            Case "tabClosedSRS"


            Case "tabNewInvoice"
                SetColumnWidth(dgvSR, "colSRSaleRepCode", 0, False)
                SetColumnWidth(dgvSRD, "colSRDBusinessStatusName", 0, False)
                SetColumnWidth(dgvSRD, "colSRDAvailableQTY", 0, False)

        End Select

    End Sub
    Private Sub SetFormCentered95Percent()

        ' لازم نرجع Normal وإلا الـ Maximized يلغي المقاسات
        '     Me.WindowState = FormWindowState.Normal
        ' Me.StartPosition = FormStartPosition.Manual

        ' Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea

        '  Me.Width = CInt(wa.Width * 0.95)
        ' Me.Height = CInt(wa.Height * 0.95)

        '  Me.Left = wa.Left + (wa.Width - Me.Width) \ 2
        '  Me.Top = wa.Top + (wa.Height - Me.Height) \ 2

    End Sub
    Private Sub FillLoadedQtyForSRD(srID As Integer)

        If srID <= 0 Then Exit Sub
        If dgvSRD.Rows.Count = 0 Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            con.Open()

            ' =====================================================
            ' Build Loading Filter (Safe for LEFT JOIN logic)
            ' =====================================================
            Dim loadingClause As String = ""

            If FilterLoadingStatus_In IsNot Nothing AndAlso
           FilterLoadingStatus_In.Count > 0 Then

                loadingClause &= "
AND LO.LoadingStatusID IN (" &
            String.Join(",", FilterLoadingStatus_In) & ")"
            End If

            If FilterLoadingStatus_NotIn IsNot Nothing AndAlso
           FilterLoadingStatus_NotIn.Count > 0 Then

                loadingClause &= "
AND LO.LoadingStatusID NOT IN (" &
            String.Join(",", FilterLoadingStatus_NotIn) & ")"
            End If


            Dim sql As String = "
SELECT
    LOD.SourceDetailID,
    SUM(LOD.LoadedQty) AS LoadedQty
FROM dbo.Logistics_LoadingOrderDetail LOD

INNER JOIN dbo.Business_SRD SRD
    ON SRD.SRDID = LOD.SourceDetailID

INNER JOIN dbo.Logistics_LoadingOrder LO
    ON LO.LOID = LOD.LOID

WHERE SRD.SRID = @SRID
" & loadingClause & "
GROUP BY LOD.SourceDetailID
"

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@SRID", srID)

                Dim dict As New Dictionary(Of Integer, Decimal)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        dict(CInt(rd("SourceDetailID"))) = CDec(rd("LoadedQty"))
                    End While
                End Using

                For Each row As DataGridViewRow In dgvSRD.Rows

                    If row.IsNewRow Then Continue For

                    Dim srdID As Integer =
                    CInt(row.Cells("colSRDID").Value)

                    If dict.ContainsKey(srdID) Then
                        row.Cells("colSRDLoadedQty").Value = dict(srdID)
                    Else
                        row.Cells("colSRDLoadedQty").Value = 0D
                    End If

                Next

            End Using
        End Using

    End Sub

    Private Sub btnCloseSR_Click(sender As Object, e As EventArgs) Handles btnCloseSR.Click

        If dgvSR.SelectedRows.Count <> 1 Then
            MessageBox.Show("يجب اختيار طلب واحد فقط", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim srID As Integer =
            CInt(dgvSR.SelectedRows(0).Cells("colSRID").Value)

        ' ==========================================
        ' تأكيد المستخدم
        ' ==========================================
        If MessageBox.Show(
            "هل أنت متأكد من إغلاق الطلب؟ لا يمكن التراجع.",
            "تأكيد الإغلاق",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        ) <> DialogResult.Yes Then
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try

                    ' ==========================================
                    ' 1) قراءة الحالة الحالية
                    ' ==========================================
                    Dim oldStatusID As Integer

                    Using cmdGetStatus As New SqlCommand("
    SELECT TOP 1 BusinessStatusID
    FROM dbo.Business_SRD
    WHERE SRID = @SRID
    ", con, tran)

                        cmdGetStatus.Parameters.AddWithValue("@SRID", srID)

                        Dim v = cmdGetStatus.ExecuteScalar()

                        If v Is Nothing OrElse IsDBNull(v) Then
                            Throw New ApplicationException("لم يتم العثور على حالة الطلب.")
                        End If

                        oldStatusID = CInt(v)
                    End Using


                    ' ==========================================
                    ' 2) منع الإغلاق إن وجد LO نشط
                    ' ==========================================
                    Using cmdCheck As New SqlCommand("
    IF EXISTS (
        SELECT 1
        FROM dbo.Business_SRD SRD
        INNER JOIN dbo.Logistics_LoadingOrderDetail LOD
            ON LOD.SourceDetailID = SRD.SRDID
        INNER JOIN dbo.Logistics_LoadingOrder LO
            ON LO.LOID = LOD.LOID
        WHERE SRD.SRID = @SRID
          AND LO.LoadingStatusID IN (1,2,14,15)
    )
        SELECT 1
    ELSE
        SELECT 0
    ", con, tran)

                        cmdCheck.Parameters.AddWithValue("@SRID", srID)

                        If CInt(cmdCheck.ExecuteScalar()) = 1 Then
                            tran.Rollback()

                            MessageBox.Show(
                                "لا يمكن إغلاق الطلب لوجود أمر تحميل نشط.",
                                "تنبيه",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)

                            Exit Sub
                        End If
                    End Using


                    ' ==========================================
                    ' 3) تنفيذ الإغلاق
                    ' ==========================================
                    Using cmdUpdate As New SqlCommand("
    UPDATE dbo.Business_SRD
    SET BusinessStatusID = 11
    WHERE SRID = @SRID
    ", con, tran)

                        cmdUpdate.Parameters.AddWithValue("@SRID", srID)
                        cmdUpdate.ExecuteNonQuery()
                    End Using


                    ' ==========================================
                    ' 4) تسجيل Audit (BUS وليس LOD)
                    ' ==========================================
                    AuditHelper.Write(
                        con,
                        tran,
                        operationTypeID:=BUS_OPERATION_TYPE_ID,
                        entityID:=srID,
                        actionCode:="CLOSE",
                        oldStatusID:=oldStatusID,
                        newStatusID:=11,
                        userID:=CurrentUser.EmployeeID,
                        reason:="إغلاق طلب مبيعات من شاشة التحميل"
                    )

                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()

                    MessageBox.Show(
                        "حدث خطأ أثناء الإغلاق: " & ex.Message,
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)

                    Exit Sub
                End Try
            End Using
        End Using

        MessageBox.Show("تم إغلاق الطلب بنجاح.",
                        "تم",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

        LoadSRList()

    End Sub

End Class

