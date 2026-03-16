Imports System.Data.SqlClient

Public Class frmInvoicePrint
    Inherits AABaseOperationForm

    Private _transactionID As Integer
    Private _zatcaDT As DataTable

    Public Sub LoadInvoice(
        transactionID As Integer,
        zatcaData As DataTable
    )
        _transactionID = transactionID
        _zatcaDT = zatcaData
    End Sub

    Private Sub frmInvoicePrint_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        If _transactionID <= 0 Then
            MessageBox.Show("فاتورة غير صالحة")
            Me.Close()
            Exit Sub
        End If

        LoadHeader()
        LoadDetails()
        LoadZATCAInfo()

    End Sub

    Private Sub LoadHeader()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    TransactionCode,
    PartnerName,
    PartnerVATNumber,
    IssueDateTime
FROM Inventory_TransactionHeader
WHERE TransactionID = @TransactionID
", con)

                cmd.Parameters.AddWithValue("@TransactionID", _transactionID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        lblInvoiceNumber.Text = rd("TransactionCode").ToString()
                        lblPartnerName.Text = rd("PartnerName").ToString()
                        lblVATNumber.Text = rd("PartnerVATNumber").ToString()
                        lblIssueDate.Text =
                            CDate(rd("IssueDateTime")).ToString("yyyy-MM-dd HH:mm")
                    End If
                End Using
            End Using
        End Using

    End Sub

    Private Sub LoadDetails()

        Using con As New SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlCommand("
SELECT
    P.ProductCode,
    P.ProductName,
    D.Quantity,
    D.UnitPrice,
    D.LineTotalWithVAT
FROM Inventory_TransactionDetails D
INNER JOIN Master_Product P ON P.ProductID = D.ProductID
WHERE D.TransactionID = @TransactionID
", con)

                cmd.Parameters.AddWithValue("@TransactionID", _transactionID)

                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)
                    dgvDetails.DataSource = dt
                End Using
            End Using
        End Using

    End Sub

    Private Sub LoadZATCAInfo()

        If _zatcaDT Is Nothing OrElse _zatcaDT.Rows.Count = 0 Then Exit Sub

        lblInvoiceHash.Text = _zatcaDT.Rows(0)("InvoiceHash").ToString()
        lblQRCode.Text = _zatcaDT.Rows(0)("QRCode").ToString()

    End Sub


End Class
