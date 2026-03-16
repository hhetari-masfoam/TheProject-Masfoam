Imports System.Data.SqlClient

Public Class frmMoveLO
    Inherits AABaseOperationForm
    Public ExistingLOID As Integer

    Public Property CurrentSRID As Integer = 0

    Private Sub frmMoveLO_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If CurrentSRID = 0 Then
            MessageBox.Show("تم فتح الشاشة بدون طلب مبيعات", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
        End If
    End Sub

    Private Sub btnNewLO_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnNewLO.Click

        If CurrentSRID = 0 Then
            MessageBox.Show("لا يوجد طلب مبيعات محدد", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim newLOID As Integer = 0

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using tran = con.BeginTransaction()
                Try
                    ' 1- توليد كود LO
                    Dim nextLOCode As String

                    Using cmdCode As New SqlCommand("cfg.GetNextCode", con, tran)
                        cmdCode.CommandType = CommandType.StoredProcedure
                        cmdCode.Parameters.AddWithValue("@CodeType", "LO")

                        Dim pNextCode As New SqlParameter("@NextCode", SqlDbType.NVarChar, 50)
                        pNextCode.Direction = ParameterDirection.Output
                        cmdCode.Parameters.Add(pNextCode)

                        cmdCode.ExecuteNonQuery()
                        nextLOCode = pNextCode.Value.ToString()
                    End Using

                    ' 2- إنشاء Header (Status=1)
                    Using cmdLO As New SqlCommand("
INSERT INTO dbo.Logistics_LoadingOrder
(LOCode, InitiatedDateTime, LoadingStatusID, OperationTypeID, CreatedAt, CreatedBy)
VALUES
(@LOCode, GETDATE(), 1, @OperationTypeID, GETDATE(), @UserID);

SELECT SCOPE_IDENTITY();
", con, tran)

                        cmdLO.Parameters.AddWithValue("@LOCode", nextLOCode)
                        cmdLO.Parameters.AddWithValue("@UserID", CurrentUser.EmployeeID)
                        cmdLO.Parameters.AddWithValue("@OperationTypeID", 1)

                        newLOID = CInt(cmdLO.ExecuteScalar())
                    End Using

                    ' 3- ربط SR
                    Using cmdLOS As New SqlCommand("
INSERT INTO dbo.Logistics_LoadingOrderSR (LOID, SRID)
VALUES (@LOID, @SRID)
", con, tran)

                        cmdLOS.Parameters.AddWithValue("@LOID", newLOID)
                        cmdLOS.Parameters.AddWithValue("@SRID", CurrentSRID)
                        cmdLOS.ExecuteNonQuery()
                    End Using

                    ' 4- نسخ التفاصيل (LoadedQty=0)
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

                        cmdLOD.Parameters.AddWithValue("@LOID", newLOID)
                        cmdLOD.Parameters.AddWithValue("@SRID", CurrentSRID)
                        cmdLOD.ExecuteNonQuery()
                    End Using


                    tran.Commit()

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            End Using
        End Using

        ' فتح لوحة التحميل على LO الجديد
        Using frm As New frmLoadingBoard()
            frm.FocusLOID = newLOID
            frm.IsNewLO = True
            frm.ShowDialog(Me)
        End Using

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnOpendLO_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnOpendLO.Click

        If CurrentSRID = 0 Then
            MessageBox.Show("لا يوجد طلب مبيعات محدد", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' فتح البورد فقط، والإضافة تتم من داخل البورد بزر btnAddSelectedSRToLO
        Using frm As New frmLoadingBoard()
            frm.FocusLOID = 0
            frm.PendingSRID = CurrentSRID
            frm.ShowDialog(Me)
        End Using

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

End Class
