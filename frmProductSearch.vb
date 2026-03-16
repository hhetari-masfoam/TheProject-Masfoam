Imports System.Data.SqlClient

Public Class frmProductSearch
    Inherits frmBaseSearch

    Public Property SelectedProductID As Integer = 0
    Public Property SelectedProductCode As String = ""
    Public Property SelectedProductTypeID As Integer = 0
    Public Property SelectedUnitID As Integer = 0
    Public Property SearchFilter As ProductSearchFilter = ProductSearchFilter.None
    Public Property FilterValueID As Integer = 0
    Public Property TargetStoreID As Integer = 0
    Public Property CategoryID As Integer = 0
    Public Property SourceStoreID As Integer = 0
    Public Property OnlyWithBalance As Boolean = False
    Public Property ProductTypeID As Integer = 0
    Public Property ProductCategoryID As Integer = 0

    Public Enum ProductSearchFilter
        None
        HasBOM
        ByType
        ByGroup
        ByCategory
        ProductionOnly
        InSourceStoreWithBalance
    End Enum

    Private Sub frmProductSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' اربط الحدث هنا حتى لا يتدخل الـ Designer
        AddHandler dgvSearch.CellMouseDoubleClick, AddressOf dgvSearch_CellMouseDoubleClick

        LoadProducts()
    End Sub

    Public Sub LoadProducts()
        Dim dt As New DataTable()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
SELECT
    p.ProductID,
    p.ProductCode,
    p.ProductName,
    pt.TypeName,
    p.ProductTypeID,
    p.ProductCategoryID,
    p.StorageUnitID
FROM Master_Product p
INNER JOIN Master_ProductType pt
    ON pt.ProductTypeID = p.ProductTypeID
WHERE p.IsActive = 1
AND (
    @OnlyWithBalance = 0
 OR EXISTS (
        SELECT 1
        FROM Inventory_Balance ib
        WHERE ib.ProductID = p.ProductID
          AND ib.StoreID = @SourceStoreID
          AND ib.QtyOnHand > 0
    )
)
AND (
    @ProductTypeID = 0
 OR p.ProductTypeID = @ProductTypeID
)
AND (
    @ProductCategoryID = 0
 OR p.ProductCategoryID = @ProductCategoryID
)
ORDER BY p.ProductCode
", con)
                cmd.Parameters.Add("@SourceStoreID", SqlDbType.Int).Value = SourceStoreID
                cmd.Parameters.Add("@OnlyWithBalance", SqlDbType.Bit).Value = OnlyWithBalance
                cmd.Parameters.Add("@ProductTypeID", SqlDbType.Int).Value = ProductTypeID
                cmd.Parameters.Add("@ProductCategoryID", SqlDbType.Int).Value = ProductCategoryID

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        dgvSearch.DataSource = dt
        dgvSearch.Columns("ProductID").Visible = False
        dgvSearch.Columns("ProductTypeID").Visible = False
        dgvSearch.Columns("ProductCategoryID").Visible = False
        dgvSearch.Columns("StorageUnitID").Visible = False
    End Sub

    Private Sub dgvSearch_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        If e.RowIndex < 0 Then Exit Sub

        SelectedProductID = CInt(dgvSearch.Rows(e.RowIndex).Cells("ProductID").Value)
        SelectedProductCode = dgvSearch.Rows(e.RowIndex).Cells("ProductCode").Value.ToString()
        SelectedProductTypeID = CInt(dgvSearch.Rows(e.RowIndex).Cells("ProductTypeID").Value)
        SelectedUnitID = CInt(dgvSearch.Rows(e.RowIndex).Cells("StorageUnitID").Value)

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

End Class