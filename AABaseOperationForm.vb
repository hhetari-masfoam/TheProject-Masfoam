Imports System.Data.SqlClient
Imports System.ComponentModel

Public Class AABaseOperationForm
    Inherits Form
    Public Sub New()
        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            InitializeComponent()
            Return
        End If

        InitializeComponent()
    End Sub

    ' =========================
    ' Connection
    ' =========================
    Protected ReadOnly ConnStr As String = AppConfig.MainConnectionString
    ' =========================
    ' Central Operation State
    ' =========================
    Protected FormOperationTypeID As Integer = 0
    Protected FormStatusID As Integer = 0
    Protected CurrentDocumentID As Integer = 0

    ' =========================
    ' Loading / UI Guard
    ' =========================
    Protected IsLoading As Boolean = False
    Private _uiGuardCounter As Integer = 0
    Protected CurrentPolicy As EditPolicy
    Protected CurrentSubEntityID As Integer = 0

    Protected Property FormStatusName As String = ""
    Protected Sub LoadWorkflowPolicy(Optional entityID As Integer = 0)

        ' 1) Resolve OperationType
        ResolveFormOperationType()

        ' 2) Resolve Status
        RefreshFormStatus(entityID)

        ' 3) Get Policy
        CurrentPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            FormOperationTypeID,
            FormStatusID
        )

        ' 4) Apply Policy (Hook)
        ApplyPolicyToForm(CurrentPolicy)

    End Sub
    Protected Overridable Sub ApplyPolicyToForm(policy As EditPolicy)
        ' يتم تنفيذها في الفورم الوارث فقط
    End Sub

    Protected Sub EnterUIGuard()
        _uiGuardCounter += 1
    End Sub

    Protected Sub ExitUIGuard()
        If _uiGuardCounter > 0 Then _uiGuardCounter -= 1
    End Sub

    Protected ReadOnly Property IsUIGuarded As Boolean
        Get
            Return _uiGuardCounter > 0
        End Get
    End Property

    ' =========================
    ' Abstract Identity
    ' =========================
    ' كل فورم يرث هذا الفورم يجب أن يحدد Scope فقط
    Protected Overridable ReadOnly Property FormScopeCode As String
        Get
            Return ""
        End Get
    End Property

    ' =========================
    ' Form Load (Central Init)
    ' =========================
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then Exit Sub
    End Sub
    ' =========================
    ' Resolve Operation Type
    ' =========================
    Protected Sub ResolveFormOperationType()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT OperationTypeID
                FROM Workflow_OperationType
                WHERE OperationCode = @Scope
                  AND IsActive = 1
            ", con)

                cmd.Parameters.AddWithValue("@Scope", FormScopeCode)

                con.Open()
                Dim result = cmd.ExecuteScalar()

                If result Is Nothing Then
                    Throw New ApplicationException(
                        $"OperationType غير معرف للـ Scope ({FormScopeCode})"
                    )
                End If

                FormOperationTypeID = CInt(result)
            End Using
        End Using

    End Sub

    ' =========================
    ' Resolve Initial Status
    ' =========================
    Protected Sub ResolveInitialStatus()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT TOP 1 
                s.StatusID,
                s.StatusName
            FROM Workflow_StatusScope ss
            INNER JOIN Workflow_Status s
                ON s.StatusID = ss.StatusID
            WHERE ss.ScopeCode = @Scope
              AND ss.IsInitial = 1
              AND ss.IsActive = 1
        ", con)

                cmd.Parameters.AddWithValue("@Scope", FormScopeCode)

                con.Open()
                Using r = cmd.ExecuteReader()

                    If Not r.Read() Then
                        Throw New ApplicationException(
                        $"لا توجد حالة ابتدائية معرفة للـ Scope ({FormScopeCode})"
                    )
                    End If

                    FormStatusID = CInt(r("StatusID"))
                    FormStatusName = r("StatusName").ToString()

                End Using
            End Using
        End Using

    End Sub

    ' =========================
    ' Refresh Status (Central)
    ' =========================
    Protected Sub RefreshFormStatus(Optional entityID As Integer = 0)

        Dim statusID As Integer = 0

        If entityID > 0 Then
            ' استخدام استعلام مباشر بدون الحاجة إلى FormScopeCode
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT StatusID
                FROM Inventory_TransactionHeader
                WHERE TransactionID = @EntityID
            ", con)

                    cmd.Parameters.AddWithValue("@EntityID", entityID)
                    con.Open()

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        statusID = CInt(result)
                    End If
                End Using
            End Using
        Else
            ' إنشاء جديد → الحالة الابتدائية
            ResolveInitialStatus()
            statusID = FormStatusID
        End If

        ' تثبيت الحالة في الفورم
        FormStatusID = statusID

    End Sub
    Protected Function GetStatusNameByID(statusID As Integer) As String

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT StatusName
            FROM Workflow_Status
            WHERE StatusID = @ID
        ", con)

                cmd.Parameters.AddWithValue("@ID", statusID)
                con.Open()

                Dim result = cmd.ExecuteScalar()
                If result Is Nothing OrElse IsDBNull(result) Then
                    Return ""
                End If

                Return result.ToString()
            End Using
        End Using

    End Function


    Protected Sub SafeResetGrid(dgv As DataGridView)

        If dgv.DataSource Is Nothing Then Exit Sub

        Dim cm As CurrencyManager =
        TryCast(Me.BindingContext(dgv.DataSource), CurrencyManager)

        If cm IsNot Nothing Then
            cm.EndCurrentEdit()
            cm.SuspendBinding()
        End If

        dgv.DataSource = Nothing

        If cm IsNot Nothing Then
            cm.ResumeBinding()
        End If

    End Sub

    Private Sub AABaseOperationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
