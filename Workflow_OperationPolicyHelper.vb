Imports System.Data.SqlClient
Imports System.Configuration

'====================================================
' Edit Mode Enum
'====================================================
Public Enum EditMode
    None = 0
    EditInPlace = 1
    EditAsNew = 2
End Enum

'====================================================
' Edit Kind (TECHNICAL ONLY)
'====================================================
Public Enum EditKind
    Quantity
    Cost
    Data
End Enum

'====================================================
' Full Workflow Policy Model
'====================================================
Public Structure EditPolicy

    ' --- Posting ---
    Public IsPostable As Boolean
    Public IsUnpostable As Boolean
    Public IsCancelable As Boolean

    ' --- Editing ---
    Public AllowEditQuantity As Boolean
    Public AllowEditCost As Boolean
    Public AllowEditData As Boolean

    ' --- Mode ---
    Public Mode As EditMode

End Structure

'====================================================
' Workflow Operation Policy Helper
'====================================================
Public Module Workflow_OperationPolicyHelper

    '====================================================
    ' Connection String
    '====================================================
    Private ReadOnly Property ConnStr As String
        Get
            Dim cs = ConfigurationManager.ConnectionStrings("MainDB")
            If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                Throw New ApplicationException("Workflow_OperationPolicyHelper: ConnectionString غير مضبوط")
            End If
            Return cs.ConnectionString
        End Get
    End Property

    '====================================================
    ' CENTRAL POLICY READER (DB → MODEL)
    '====================================================
    Public Function GetEditPolicy(
        operationTypeID As Integer,
        statusID As Integer
    ) As EditPolicy

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT
                    IsPostable,
                    IsUnpostable,
                    IsCancelable,
                    AllowEditQuantity,
                    AllowEditCost,
                    AllowEditData,
                    EditMode
                FROM Workflow_OperationStatusPolicy
                WHERE OperationTypeID = @OP
                  AND StatusID = @ST
                  AND IsActive = 1
            ", con)

                cmd.Parameters.AddWithValue("@OP", operationTypeID)
                cmd.Parameters.AddWithValue("@ST", statusID)

                con.Open()
                Using r = cmd.ExecuteReader()

                    If Not r.Read() Then
                        Return New EditPolicy With {
                            .IsPostable = False,
                            .IsUnpostable = False,
                            .IsCancelable = False,
                            .AllowEditQuantity = False,
                            .AllowEditCost = False,
                            .AllowEditData = False,
                            .Mode = EditMode.None
                        }
                    End If

                    Return New EditPolicy With {
                        .IsPostable = CBool(r("IsPostable")),
                        .IsUnpostable = CBool(r("IsUnpostable")),
                        .IsCancelable = CBool(r("IsCancelable")),
                        .AllowEditQuantity = CBool(r("AllowEditQuantity")),
                        .AllowEditCost = CBool(r("AllowEditCost")),
                        .AllowEditData = CBool(r("AllowEditData")),
                        .Mode = CType(CInt(r("EditMode")), EditMode)
                    }

                End Using
            End Using
        End Using

    End Function

    '====================================================
    ' Helper: Is Edit Allowed (NO DECISION)
    '====================================================
    Public Function IsEditAllowed(
    policy As EditPolicy,
    kind As EditKind
) As Boolean

        Select Case kind
            Case EditKind.Quantity
                Return policy.AllowEditQuantity

            Case EditKind.Cost
                Return policy.AllowEditCost

            Case EditKind.Data
                Return policy.AllowEditData

            Case Else
                Return False
        End Select

    End Function


    '====================================================
    ' Document Status Reader (SOURCE OF STATUS)
    '====================================================
    Public Sub GetDocumentStatus(
        documentID As Integer,
        ByRef statusID As Integer
    )

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT StatusID
                FROM Inventory_DocumentHeader
                WHERE DocumentID = @ID
            ", con)

                cmd.Parameters.AddWithValue("@ID", documentID)
                con.Open()

                Dim v = cmd.ExecuteScalar()
                If v Is Nothing OrElse IsDBNull(v) Then
                    Throw New ApplicationException("المستند غير موجود")
                End If

                statusID = CInt(v)
            End Using
        End Using

    End Sub
    Public Sub GetEntityStatusByScope(
    scopeCode As String,
    entityID As Integer,
    ByRef statusID As Integer
)

        Using con As New SqlConnection(ConnStr)

            Dim sql As String = ""

            Select Case scopeCode

                Case "PUR"
                    sql = "
SELECT StatusID
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
"

                Case "TRN"
                    sql = "
SELECT StatusID
FROM Inventory_TransactionHeader
WHERE TransactionID = @ID
"

                Case "PRO"
                    sql = "
SELECT StatusID
FROM ProductionHeader
WHERE ProductionID = @ID
"

                    ' ⬅️ أضف أي Scope جديد هنا فقط
                Case "CUT"
                    sql = "
SELECT StatusID
FROM Production_CuttingHeader
WHERE CuttingID = @ID
"
                Case "LOD"
                    sql = "
SELECT StatusID
FROM Production_CuttingHeader
WHERE CuttingID = @ID
"
                Case "SAL"
                    sql = "
SELECT StatusID
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
"


                Case "BUS"
                    sql = "
SELECT StatusID
FROM Business_SR
WHERE SRID = @ID
"

                Case "SRT"
                    sql = "
SELECT StatusID
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
"
                Case "PRT"
                    sql = "
SELECT StatusID
FROM Inventory_DocumentHeader
WHERE DocumentID = @ID
"


                Case Else
                    Throw New ApplicationException(
                    $"Scope غير مدعوم: {scopeCode}"
                )
            End Select

            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@ID", entityID)
                con.Open()

                Dim v = cmd.ExecuteScalar()

                If v Is Nothing OrElse IsDBNull(v) Then
                    Throw New ApplicationException("السجل غير موجود")
                End If

                statusID = CInt(v)
            End Using

        End Using

    End Sub
    Public Function GetInitialStatusByScope(scopeCode As String) As Integer

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT TOP (1) StatusID
            FROM Workflow_StatusScope
            WHERE ScopeCode = @Scope
              AND IsInitial = 1
              AND IsActive = 1
        ", con)

                cmd.Parameters.AddWithValue("@Scope", scopeCode)
                con.Open()

                Dim v = cmd.ExecuteScalar()
                If v Is Nothing OrElse IsDBNull(v) Then
                    Throw New ApplicationException(
                    $"لا توجد حالة ابتدائية معرفة لـ Scope = {scopeCode}"
                )
                End If

                Return CInt(v)
            End Using
        End Using

    End Function
    Public Function GetStatusName(statusID As Integer) As String

        Using con As New SqlClient.SqlConnection(ConnStr)
            Using cmd As New SqlClient.SqlCommand(
            "SELECT StatusName
             FROM Workflow_Status
             WHERE StatusID = @ID
               AND IsActive = 1", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = statusID
                con.Open()

                Dim obj = cmd.ExecuteScalar()

                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Return "غير معروف"
                End If

                Return obj.ToString()
            End Using
        End Using

    End Function
    '====================================================
    ' OperationType Reader (By Code)
    '====================================================
    Public Function GetOperationTypeIDByCode(
    operationCode As String
) As Integer

        If String.IsNullOrWhiteSpace(operationCode) Then
            Throw New ArgumentException("OperationCode غير صالح.")
        End If

        Using con As New SqlConnection(ConnStr)

            Using cmd As New SqlCommand("
            SELECT OperationTypeID
            FROM Workflow_OperationType
            WHERE OperationCode = @Code
              AND IsActive = 1
        ", con)

                cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = operationCode

                con.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse IsDBNull(result) Then
                    Throw New ApplicationException(
                    $"لم يتم العثور على OperationType بالكود: {operationCode}"
                )
                End If

                Return CInt(result)

            End Using
        End Using

    End Function

End Module
