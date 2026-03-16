' =========================
' frmProducts.vb
' =========================

Imports System.Data
Imports System.Data.SqlClient

Public Class frmProduct
    Inherits AABaseOperationForm
    ' =========================
    ' Prefill from Sale Request
    ' =========================
    Private HasSaleRequestPrefill As Boolean = False
    Private IsFormInitializing As Boolean = False

    Private SR_ProductCode As String = ""
    Private SR_BaseProductCode As String = ""
    Private SR_ProductTypeID As Integer = 0
    Private SR_SellUnitID As Integer = 0
    Private SR_L As Decimal = 0
    Private SR_W As Decimal = 0
    Private SR_H As Decimal = 0

    ' =========================
    ' ProductID الناتج بعد الحفظ
    ' =========================
    Private OpenedFromCutting As Boolean = False

    Public Property SavedProductID As Integer = 0
    ' =========================
    ' بيانات مؤقتة قادمة من فورم القص
    ' =========================
    Private hasPrefill As Boolean = False
    Private preCode As String = ""
    Private preTypeID As Integer = 0
    Private preGroupName As String = ""
    Private preColorName As String = ""
    Private preL As Decimal = 0
    Private preW As Decimal = 0
    Private preH As Decimal = 0
    Private preBaseProductCode As String = ""
    ' =========================
    ' كثافة قادمة من فورم القص
    ' =========================
    Private preDensity As Decimal = 0D

    ' =========================================================
    ' بيانات قادمة من فورم القص (للترميز التلقائي)
    ' =========================================================
    Private Structure CuttingProductPrefill
        Public ProductCode As String
        Public ProductTypeID As Integer
        Public ProductGroupName As String
        Public ColorName As String
        Public LengthCM As Decimal
        Public WidthCM As Decimal
        Public HeightCM As Decimal
    End Structure

    Private _cuttingPrefill As CuttingProductPrefill
    Private _hasCuttingPrefill As Boolean = False


    ' =========================
    ' Opened From Sale Request
    ' =========================
    Private OpenedFromSaleRequest As Boolean = False

    ' =========================================================
    ' اختيار المنتج الأساسي بناءً على الكود القادم من فورم القص
    ' =========================================================
    Private Sub SelectBaseProductByCode(baseProductCode As String)

        If String.IsNullOrWhiteSpace(baseProductCode) Then Exit Sub
        If cboBaseProduct.DataSource Is Nothing Then Exit Sub

        For Each drv As DataRowView In CType(cboBaseProduct.DataSource, DataTable).DefaultView
            If drv("ProductCode").ToString().Trim() = baseProductCode.Trim() Then
                cboBaseProduct.SelectedValue = CInt(drv("ProductID"))
                Exit Sub
            End If
        Next

    End Sub

    ' =========================
    ' استقبال البيانات من فورم القص
    ' =========================
    Public Sub PrefillFromCutting(
    productCode As String,
    productTypeID As Integer,
    productGroupName As String,
    colorName As String,
    baseProductCode As String,
    lengthCM As Decimal,
    widthCM As Decimal,
    heightCM As Decimal,
    targetStoreID As Integer
)

        preCode = productCode
        preTypeID = productTypeID
        preGroupName = productGroupName
        preColorName = colorName
        preBaseProductCode = baseProductCode

        preL = lengthCM
        preW = widthCM
        preH = heightCM


        hasPrefill = True
        OpenedFromCutting = True

    End Sub
    ' =========================================================
    ' استقبال البيانات من طلب المبيعات (SR)
    ' - الكود جاهز ولا يتم توليده
    ' =========================================================

    ' =========================
    ' Connection
    ' =========================

    ' =========================
    ' State Variables
    ' =========================
    Private CurrentProductID As Integer = 0
    Private IsLoadingProduct As Boolean = False
    Private BaseProductCode As String = ""
    ' =========================
    ' دالة موحدة لحسم وحدات الصنف
    ' =========================
    Private Sub ResolveProductUnits(
    ByRef purchaseUnitID As Integer,
    ByRef stockUnitID As Integer
)

        purchaseUnitID = 0
        stockUnitID = 0

    End Sub

    ' =========================
    ' Form Load
    ' =========================
    Private Sub frmProducts_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        IsFormInitializing = True

        LoadCategory()
        LoadProductGroup()
        LoadUnits()
        LoadTaxtype()
        LoadProductColor()
        LoadSubCategories()
        LoadProductType()
        LoadMixType()

        ' 🔒 اضبط الحالة الابتدائية صراحة
        chkHasDimensions.Checked = False
        ToggleDimensionsControls(False)

        ' ===== Prefill logic =====
        If HasSaleRequestPrefill Then

            ApplySaleRequestDefaultsFromBaseProduct()
        End If
        ' ===== Prefill logic =====

        IsFormInitializing = False

        ' ===== Sales Request Prefill =====
        If OpenedFromSaleRequest AndAlso HasSaleRequestPrefill Then


            ' افتح الأبعاد مرة واحدة فقط
            chkHasDimensions.Checked = True
            ToggleDimensionsControls(True)

            ' عبّي القيم
            txtLength.Text = SR_L.ToString("0.###")
            txtWidth.Text = SR_W.ToString("0.###")
            txtHeight.Text = SR_H.ToString("0.###")

            ' طبق خصائص البيز برودكت
            ApplySaleRequestDefaultsFromBaseProduct()

            ' اختَر البيز برودكت بعد تحميل الكمبو
            If Not String.IsNullOrWhiteSpace(SR_BaseProductCode) Then
                SelectBaseProductByCode(SR_BaseProductCode)
            End If

        End If


        ' ✅ بعد انتهاء التحميل فقط
        If hasPrefill AndAlso OpenedFromCutting Then
            chkHasDimensions.Checked = True
            ToggleDimensionsControls(True)

            ' ===== من الجريد =====
            txtProductCode.Text = preCode
            txtLength.Text = preL.ToString("0.###")
            txtWidth.Text = preW.ToString("0.###")
            txtHeight.Text = preH.ToString("0.###")
            cboProductTypeID.SelectedValue = preTypeID

            ' ===== قيم ثابتة =====

            chkProductStatus.Checked = True

            ' ===== من الأب =====
            If preBaseProductCode <> "" Then

                SelectBaseProductByCode(preBaseProductCode)
                CopyDefaultsFromBaseProduct(preBaseProductCode)
            End If

        End If


    End Sub

    Private Sub SetDefaultsForNewProduct()

        ' تأكد أن الكمبوهات محملة
        If cboCategory.DataSource Is Nothing Then Exit Sub

        cboCategory.SelectedValue = 2
        cboSubCategory.SelectedValue = 9
        cboProductGroup.SelectedValue = 13
        cboProductTypeID.SelectedValue = 1
        cboProductColor.SelectedValue = 8
        cboPricingUnitID.SelectedValue = 8
        cboMixTypeID.SelectedValue = 2
        cboStorageUnitID.SelectedValue = 8
        chkHasDimensions.Checked = False
        btnSave.Enabled = True
    End Sub

    ' =========================
    ' Load Product Color
    ' =========================
    Private Sub LoadProductColor()
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT ColorID, ColorName FROM Master_ProductColor ORDER BY ColorName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductColor.DataSource = dt
        cboProductColor.DisplayMember = "ColorName"
        cboProductColor.ValueMember = "ColorID"
        cboProductColor.SelectedIndex = -1
        ' =========================
        ' Prefill from Sale Request (AFTER LOAD)
        ' =========================
        If HasSaleRequestPrefill Then

            ' الكود جاهز
            txtProductCode.Text = SR_ProductCode
            txtProductCode.ReadOnly = True

            ' النوع
            If SR_ProductTypeID > 0 Then
                cboProductTypeID.SelectedValue = SR_ProductTypeID
            End If

            ' المقاسات
            chkHasDimensions.Checked = True
            ToggleDimensionsControls(True)

            txtLength.Text = SR_L.ToString("0.###")
            txtWidth.Text = SR_W.ToString("0.###")
            txtHeight.Text = SR_H.ToString("0.###")

            ' Base Product (بعد تحميله)
            If SR_BaseProductCode <> "" Then
                cboBaseProduct.Text = SR_BaseProductCode
            End If

            ' وحدة التسعير
            If SR_SellUnitID > 0 Then
                cboPricingUnitID.SelectedValue = SR_SellUnitID
            End If

            chkProductStatus.Checked = True

        End If

    End Sub
    Private Sub LoadMixType()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductID, ProductCode
             FROM Master_Product
             WHERE IsActive = 1
               AND ProductID <> @CurrentProductID
             ORDER BY ProductCode", con)

                ' 🟢 مهم: منع اختيار المنتج نفسه
                cmd.Parameters.Add("@CurrentProductID", SqlDbType.Int).Value = CurrentProductID

                con.Open()
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    dt.Load(rdr)
                End Using

            End Using
        End Using

        ' 🟢 إعادة ربط نظيفة
        cboMixTypeID.DataSource = Nothing
        cboMixTypeID.Items.Clear()

        cboMixTypeID.DisplayMember = "ProductCode"
        cboMixTypeID.ValueMember = "ProductID"
        cboMixTypeID.DataSource = dt

        ' فقط في الإضافة الجديدة
        If CurrentProductID = 0 Then
            cboMixTypeID.SelectedIndex = -1
        End If


    End Sub

    ' =========================
    ' Load Category
    ' =========================
    Private Sub LoadCategory()
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT CategoryID, CategoryName FROM Master_ProductCategory ORDER BY CategoryName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboCategory.DataSource = dt
        cboCategory.DisplayMember = "CategoryName"
        cboCategory.ValueMember = "CategoryID"
        cboCategory.SelectedIndex = -1
    End Sub

    ' =========================
    ' Load SubCategory by Category
    ' =========================
    Private Sub LoadSubCategories()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT SubCategoryID, SubCategoryName, HasDimensions
             FROM Master_SubCategory
             WHERE IsActive = 1
             ORDER BY SubCategoryName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using
        End Using

        cboSubCategory.DataSource = dt
        cboSubCategory.DisplayMember = "SubCategoryName"
        cboSubCategory.ValueMember = "SubCategoryID"
        cboSubCategory.SelectedIndex = -1

    End Sub

    ' =========================
    ' Load Product Group
    ' =========================
    Private Sub LoadProductGroup()
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT ProductGroupID, GroupName FROM Master_ProductGroup ORDER BY GroupName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductGroup.DataSource = dt
        cboProductGroup.DisplayMember = "GroupName"
        cboProductGroup.ValueMember = "ProductGroupID"
        cboProductGroup.SelectedIndex = -1
    End Sub

    ' =========================
    ' Load Product Type
    ' =========================
    Private Sub LoadProductType()
        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT ProductTypeID, TypeName FROM Master_ProductType ORDER BY TypeName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboProductTypeID.DataSource = dt
        cboProductTypeID.DisplayMember = "TypeName"
        cboProductTypeID.ValueMember = "ProductTypeID"
        cboProductTypeID.SelectedIndex = -1
    End Sub

    Private Sub chkHasDimensions_CheckedChanged(
    sender As Object, e As EventArgs
) Handles chkHasDimensions.CheckedChanged

        If IsFormInitializing Then Exit Sub

        ToggleDimensionsControls(chkHasDimensions.Checked)

    End Sub

    Private Sub LoadUnits()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT UnitID, UnitName FROM Master_Unit WHERE IsActive = 1 ORDER BY UnitName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

            End Using
        End Using

        ' تحميل نفس المصدر للجميع
        cboStorageUnitID.DataSource = dt.Copy()
        cboStorageUnitID.DisplayMember = "UnitName"
        cboStorageUnitID.ValueMember = "UnitID"

        cboPricingUnitID.DataSource = dt.Copy()
        cboPricingUnitID.DisplayMember = "UnitName"
        cboPricingUnitID.ValueMember = "UnitID"

        ' =========================
        ' Production Unit (NEW)
        ' =========================
        Dim dtProduction As New DataTable
        dtProduction = dt.Copy()

        cboProductionUnit.DataSource = dtProduction
        cboProductionUnit.DisplayMember = "UnitName"
        cboProductionUnit.ValueMember = "UnitID"
        cboProductionUnit.SelectedIndex = -1
        cboProductionUnit.SelectedValue = 12 ' PCS


    End Sub

    ' =========================
    ' Load Store
    ' =========================

    ' =========================
    ' Load Tax Type
    ' =========================
    ' =========================
    ' Load Tax Type
    ' =========================
    Private Sub LoadTaxtype()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT TaxTypeID, TaxName FROM Master_Taxtype ORDER BY TaxName", con)

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        cboTaxType.DataSource = dt
        cboTaxType.DisplayMember = "TaxName"
        cboTaxType.ValueMember = "TaxTypeID"

        ' ✅ تثبيت الضريبة الافتراضية (بعد الربط فقط)
        If dt.Rows.Count > 0 Then
            cboTaxType.SelectedValue = 1   ' أو أول ضريبة
            If cboTaxType.SelectedIndex = -1 Then
                cboTaxType.SelectedIndex = 0
            End If
        End If

    End Sub
    ' =========================
    ' Load Base Products (for dimensions)
    ' =========================
    Private Sub LoadBaseProducts()

        Dim dt As New DataTable

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "SELECT ProductID, ProductCode, ProductName
FROM Master_Product
WHERE IsActive = 1
  AND (Length IS NULL OR Length = 0)
  AND ProductID <> @CurrentProductID
ORDER BY ProductName
", con)

                cmd.Parameters.Add("@CurrentProductID", SqlDbType.Int).Value = CurrentProductID

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        ' فك أي ربط سابق (مهم جدًا)
        cboBaseProduct.DataSource = Nothing
        cboBaseProduct.Items.Clear()

        ' تأكد أن dt يحتوي ProductID و ProductCode
        cboBaseProduct.DisplayMember = "ProductCode"
        cboBaseProduct.ValueMember = "ProductID"

        ' الآن اربط المصدر
        cboBaseProduct.DataSource = dt

        ' بدون اختيار افتراضي
        cboBaseProduct.SelectedIndex = -1


    End Sub

    ' =========================
    ' Generate next product code (NO dimensions)
    ' Format: P-00001
    ' =========================
    Private Function GetNextProductCode() As String

        Dim maxNumber As Integer = 0

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductCode FROM Master_Product WHERE ProductCode LIKE 'P-%'", con)

                con.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()

                        Dim code As String = dr("ProductCode").ToString()

                        ' يقبل فقط الشكل P-00001
                        If code.Length = 7 AndAlso code.StartsWith("P-") Then
                            Dim numPart As Integer
                            If Integer.TryParse(code.Substring(2), numPart) Then
                                If numPart > maxNumber Then
                                    maxNumber = numPart
                                End If
                            End If
                        End If

                    End While
                End Using
            End Using
        End Using

        Return "P-" & (maxNumber + 1).ToString("00000")

    End Function
    ' =========================
    ' Validate Product
    ' =========================
    Private Function ValidateProduct() As Boolean

        If cboCategory.SelectedIndex = -1 OrElse
   cboSubCategory.SelectedIndex = -1 OrElse
   cboStorageUnitID.SelectedIndex = -1 OrElse
   cboTaxType.SelectedIndex = -1 OrElse
   cboProductColor.SelectedIndex = -1 Then

            MessageBox.Show("يرجى تعبئة جميع الحقول المطلوبة")
            Return False
        End If

        If chkHasDimensions.Checked Then
            If txtLength.Text.Trim() = "" OrElse
       txtWidth.Text.Trim() = "" OrElse
       txtHeight.Text.Trim() = "" Then

                MessageBox.Show("يرجى إدخال الطول والعرض والارتفاع")
                Return False
            End If
        End If



        ' الكود
        If txtProductCode.Text.Trim() = "" Then
            MessageBox.Show("كود الصنف مطلوب")
            Return False
        End If

        ' التكرار
        If IsProductCodeExists(
    txtProductCode.Text.Trim(),
    CInt(cboProductTypeID.SelectedValue)
) Then

            MessageBox.Show("كود الصنف موجود مسبقًا لهذا النوع")
            Return False

        End If

        If cboProductTypeID.SelectedIndex = -1 _
   OrElse cboProductTypeID.SelectedValue Is Nothing _
   OrElse IsDBNull(cboProductTypeID.SelectedValue) Then

            MessageBox.Show("يرجى اختيار نوع الصنف (Product Type)")
            cboProductTypeID.Focus()
            Return False
        End If

        Return True

    End Function

    ' =========================
    ' Generate Product Code By Dimensions
    ' Format: X-098009099
    ' =========================
    Private Function GenerateProductCodeByDimensions() As String

        Dim baseCode As String = ""

        ' =========================
        ' تحديد كود الصنف الأساسي
        ' =========================
        If cboBaseProduct.SelectedIndex = -1 AndAlso BaseProductCode = "" Then
            MessageBox.Show("يرجى اختيار الصنف الأساسي")
            Return ""
        End If

        ' عند التعديل
        If Not String.IsNullOrWhiteSpace(BaseProductCode) Then
            baseCode = BaseProductCode.Trim()

            ' عند الإدخال الجديد
        ElseIf cboBaseProduct.SelectedIndex <> -1 Then

            Dim drv As DataRowView =
            TryCast(cboBaseProduct.SelectedItem, DataRowView)

            If drv Is Nothing Then Return ""

            baseCode = drv("ProductCode").ToString().Trim()
            BaseProductCode = baseCode

        Else
            Return ""
        End If

        ' =========================
        ' قراءة الأبعاد
        ' =========================
        Dim lDec, wDec, hDec As Decimal

        If Not Decimal.TryParse(txtLength.Text.Trim(), lDec) Then Return ""
        If Not Decimal.TryParse(txtWidth.Text.Trim(), wDec) Then Return ""
        If Not Decimal.TryParse(txtHeight.Text.Trim(), hDec) Then Return ""

        Dim l As Integer = CInt(Math.Round(lDec))
        Dim w As Integer = CInt(Math.Round(wDec))
        Dim h As Integer = CInt(Math.Round(hDec))

        ' =========================
        ' توليد الكود
        ' مثال: FOAM-080200050
        ' =========================
        Return $"{baseCode}-{l:000}{w:000}{h:000}"

    End Function

    Private Sub InsertProduct(purchaseUnitID As Integer, stockUnitID As Integer)

        Using con As New SqlConnection(ConnStr)

            Dim productTypeID As Integer = CInt(cboProductTypeID.SelectedValue)

            Dim sql As String =
        "INSERT INTO Master_Product (" &
        "ProductCode, ProductName, ProductEnglishName, Barcode, " &
        "ProductCategoryID, ProductSubCategoryID, ProductTypeID, ProductGroupID, " &
        "StorageUnitID, PricingUnitID, ProductionUnitID, " &
        "DefaultTaxTypeID, IsActive,  Description, " &
        "Length, Width, Height, Density, BaseProductID, HasDimensions, " &
        "ProductColorID, MixTypeID" &
        ") VALUES (" &
        "@ProductCode, @ProductName, @ProductEnglishName, @Barcode, " &
        "@ProductCategoryID, @ProductSubCategoryID, @ProductTypeID, @ProductGroupID, " &
        "@StorageUnitID, @PricingUnitID, @ProductionUnitID, " &
        " @DefaultTaxTypeID, @IsActive,  @Description, " &
        "@Length, @Width, @Height, @Density, @BaseProductID, @HasDimensions, " &
        "@ProductColorID, @MixTypeID" &
        "); SELECT SCOPE_IDENTITY();"

            Using cmd As New SqlCommand(sql, con)

                ' =========================
                ' البيانات الأساسية
                ' =========================
                cmd.Parameters.Add("@ProductCode", SqlDbType.NVarChar, 50).Value = txtProductCode.Text.Trim()
                cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200).Value = txtProductName.Text.Trim()
                cmd.Parameters.Add("@ProductEnglishName", SqlDbType.NVarChar, 200).Value =
                If(txtProductEnglishName.Text.Trim() = "", DBNull.Value, txtProductEnglishName.Text.Trim())
                cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar, 100).Value =
                If(txtBarcode.Text.Trim() = "", DBNull.Value, txtBarcode.Text.Trim())
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
                If(txtDescription.Text.Trim() = "", DBNull.Value, txtDescription.Text.Trim())

                ' =========================
                ' العلاقات
                ' =========================
                cmd.Parameters.Add("@ProductCategoryID", SqlDbType.Int).Value = CInt(cboCategory.SelectedValue)
                cmd.Parameters.Add("@ProductSubCategoryID", SqlDbType.Int).Value = CInt(cboSubCategory.SelectedValue)
                cmd.Parameters.Add("@ProductTypeID", SqlDbType.Int).Value = productTypeID

                If cboProductGroup.SelectedIndex = -1 Then
                    cmd.Parameters.Add("@ProductGroupID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@ProductGroupID", SqlDbType.Int).Value = CInt(cboProductGroup.SelectedValue)
                End If

                ' =========================
                ' Production Unit (اختياري)
                ' =========================
                If cboProductionUnit.SelectedValue Is Nothing _
   OrElse IsDBNull(cboProductionUnit.SelectedValue) _
   OrElse Not TypeOf cboProductionUnit.SelectedValue Is Integer Then

                    cmd.Parameters.Add("@ProductionUnitID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@ProductionUnitID", SqlDbType.Int).Value =
        CInt(cboProductionUnit.SelectedValue)
                End If

                ' =========================
                ' الضريبة
                ' =========================
                Dim taxID As Integer = If(cboTaxType.SelectedIndex > -1, CInt(cboTaxType.SelectedValue), 1)
                cmd.Parameters.Add("@DefaultTaxTypeID", SqlDbType.Int).Value = taxID

                cmd.Parameters.Add("@ProductColorID", SqlDbType.Int).Value = CInt(cboProductColor.SelectedValue)

                If cboMixTypeID.SelectedIndex = -1 Then
                    cmd.Parameters.Add("@MixTypeID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@MixTypeID", SqlDbType.Int).Value = CInt(cboMixTypeID.SelectedValue)
                End If

                ' =========================
                ' Base Product
                ' =========================
                If cboBaseProduct.SelectedValue Is Nothing _
                OrElse IsDBNull(cboBaseProduct.SelectedValue) _
                OrElse TypeOf cboBaseProduct.SelectedValue Is DataRowView Then

                    cmd.Parameters.Add("@BaseProductID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@BaseProductID", SqlDbType.Int).Value =
                    CInt(cboBaseProduct.SelectedValue)
                End If

                ' =========================
                ' الوحدات
                ' =========================
                cmd.Parameters.Add("@StorageUnitID", SqlDbType.Int).Value = CInt(cboStorageUnitID.SelectedValue)
                cmd.Parameters.Add("@PricingUnitID", SqlDbType.Int).Value = CInt(cboPricingUnitID.SelectedValue)


                ' =========================
                ' الحالة
                ' =========================
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkProductStatus.Checked

                ' =========================
                ' الأبعاد
                ' =========================
                cmd.Parameters.Add("@HasDimensions", SqlDbType.Bit).Value = chkHasDimensions.Checked
                cmd.Parameters.Add("@Length", SqlDbType.Decimal).Value =
                If(txtLength.Text.Trim() = "", DBNull.Value, Decimal.Parse(txtLength.Text.Trim()))
                cmd.Parameters.Add("@Width", SqlDbType.Decimal).Value =
                If(txtWidth.Text.Trim() = "", DBNull.Value, Decimal.Parse(txtWidth.Text.Trim()))
                cmd.Parameters.Add("@Height", SqlDbType.Decimal).Value =
                If(txtHeight.Text.Trim() = "", DBNull.Value, Decimal.Parse(txtHeight.Text.Trim()))
                cmd.Parameters.Add("@Density", SqlDbType.Decimal).Value =
                If(txtDensity.Text.Trim() = "", DBNull.Value, Decimal.Parse(txtDensity.Text.Trim()))

                con.Open()

                Dim newIDObj As Object = cmd.ExecuteScalar()

                If newIDObj Is Nothing OrElse IsDBNull(newIDObj) Then
                    Throw New Exception("فشل حفظ الصنف")
                End If

                CurrentProductID = CInt(newIDObj)
                SavedProductID = CurrentProductID   ' ⭐ النقطة الحاسمة

            End Using
        End Using

    End Sub
    ' =========================================================
    ' نسخ الخصائص من Base Product
    ' =========================================================
    Private Sub CopyDefaultsFromBaseProduct(baseProductCode As String)

        If String.IsNullOrWhiteSpace(baseProductCode) Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT *
             FROM Master_Product
             WHERE ProductCode = @Code", con)

                cmd.Parameters.AddWithValue("@Code", baseProductCode.Trim())
                con.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If Not dr.Read() Then Exit Sub

                    ' =========================
                    ' التصنيفات
                    ' =========================
                    cboCategory.SelectedValue = 3
                    cboSubCategory.SelectedValue = dr("ProductSubCategoryID")

                    txtProductName.Text = dr("ProductName").ToString()

                    txtProductEnglishName.Text = dr("ProductEnglishName").ToString()
                    ' =========================
                    ' المجموعة / اللون
                    ' =========================
                    If Not IsDBNull(dr("ProductGroupID")) Then
                        cboProductGroup.SelectedValue = dr("ProductGroupID")
                    End If

                    If Not IsDBNull(dr("ProductColorID")) Then
                        cboProductColor.SelectedValue = dr("ProductColorID")
                    End If

                    ' =========================
                    ' الوحدات
                    ' =========================
                    cboStorageUnitID.SelectedValue = 12
                    cboPricingUnitID.SelectedValue = dr("PricingUnitID")


                    ' =========================
                    ' الضريبة والمخزن
                    ' =========================
                    cboTaxType.SelectedValue = 1

                    ' =========================
                    ' الكثافة (اختياري)
                    ' =========================
                    If Not IsDBNull(dr("Density")) Then
                        txtDensity.Text = dr("Density").ToString()
                    End If

                End Using
            End Using
        End Using

    End Sub

    ' =========================
    ' Update Product
    ' =========================
    Private Sub UpdateProduct(purchaseUnitID As Integer, stockUnitID As Integer)

        If CurrentProductID = 0 Then
            MessageBox.Show("لا يوجد صنف للتعديل")
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)

            Dim sql As String =
            "UPDATE Master_Product SET " &
            "ProductCode=@ProductCode, " &
            "ProductName=@ProductName, " &
            "ProductEnglishName=@ProductEnglishName, " &
            "Barcode=@Barcode, " &
            "Description=@Description, " &
            "ProductCategoryID=@ProductCategoryID, " &
            "ProductSubCategoryID=@ProductSubCategoryID, " &
            "ProductTypeID=@ProductTypeID, " &
            "ProductGroupID=@ProductGroupID, " &
            "PricingUnitID=@PricingUnitID, " &
            "StorageUnitID=@StorageUnitID, " &
            "DefaultTaxTypeID=@DefaultTaxTypeID, " &
            "ProductColorID=@ProductColorID, " &
            "MixTypeID=@MixTypeID, " &
            "BaseProductID=@BaseProductID, " &
            "HasDimensions=@HasDimensions, " &
            "Length=@Length, " &
            "Width=@Width, " &
            "Height=@Height, " &
            "Density=@Density, " &
            "ProductionUnitID=@ProductionUnitID, " &
            "IsActive=@IsActive " &
            "WHERE ProductID=@ProductID"

            Using cmd As New SqlCommand(sql, con)

                ' =========================
                ' المفتاح
                ' =========================
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = CurrentProductID

                ' =========================
                ' بيانات أساسية
                ' =========================
                cmd.Parameters.Add("@ProductCode", SqlDbType.NVarChar, 50).Value = txtProductCode.Text.Trim()
                cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200).Value = txtProductName.Text.Trim()
                cmd.Parameters.Add("@ProductEnglishName", SqlDbType.NVarChar, 200).Value =
                If(txtProductEnglishName.Text.Trim() = "", DBNull.Value, txtProductEnglishName.Text.Trim())
                cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar, 100).Value =
                If(txtBarcode.Text.Trim() = "", DBNull.Value, txtBarcode.Text.Trim())
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
                If(txtDescription.Text.Trim() = "", DBNull.Value, txtDescription.Text.Trim())

                ' =========================
                ' العلاقات
                ' =========================
                cmd.Parameters.Add("@ProductCategoryID", SqlDbType.Int).Value = CInt(cboCategory.SelectedValue)
                cmd.Parameters.Add("@ProductSubCategoryID", SqlDbType.Int).Value = CInt(cboSubCategory.SelectedValue)
                cmd.Parameters.Add("@ProductTypeID", SqlDbType.Int).Value = CInt(cboProductTypeID.SelectedValue)

                If cboProductGroup.SelectedIndex = -1 Then
                    cmd.Parameters.Add("@ProductGroupID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@ProductGroupID", SqlDbType.Int).Value = CInt(cboProductGroup.SelectedValue)
                End If

                cmd.Parameters.Add("@ProductColorID", SqlDbType.Int).Value = CInt(cboProductColor.SelectedValue)
                ' =========================
                ' Production Unit (اختياري)
                ' =========================
                If cboProductionUnit.SelectedValue Is Nothing _
   OrElse IsDBNull(cboProductionUnit.SelectedValue) _
   OrElse Not TypeOf cboProductionUnit.SelectedValue Is Integer Then

                    cmd.Parameters.Add("@ProductionUnitID", SqlDbType.Int).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@ProductionUnitID", SqlDbType.Int).Value =
        CInt(cboProductionUnit.SelectedValue)
                End If

                ' =========================
                ' الوحدات
                ' =========================
                cmd.Parameters.Add("@PricingUnitID", SqlDbType.Int).Value = CInt(cboPricingUnitID.SelectedValue)
                cmd.Parameters.Add("@StorageUnitID", SqlDbType.Int).Value = CInt(cboStorageUnitID.SelectedValue)

                ' =========================
                ' الأسعار
                ' =========================


                ' =========================
                ' الضريبة (Default = 1)
                ' =========================
                Dim taxID As Integer = 1
                If cboTaxType.SelectedIndex > -1 Then
                    taxID = CInt(cboTaxType.SelectedValue)
                End If
                cmd.Parameters.Add("@DefaultTaxTypeID", SqlDbType.Int).Value = taxID

                ' =========================
                ' Mix Type (اختياري)
                ' =========================
                If cboMixTypeID.SelectedValue Is Nothing _
   OrElse IsDBNull(cboMixTypeID.SelectedValue) _
   OrElse TypeOf cboMixTypeID.SelectedValue Is DataRowView Then

                    cmd.Parameters.Add("@MixTypeID", SqlDbType.Int).Value = DBNull.Value

                Else
                    cmd.Parameters.Add("@MixTypeID", SqlDbType.Int).Value =
        CInt(cboMixTypeID.SelectedValue)
                End If
                ' =========================
                ' Base Product (اختياري)
                ' =========================
                If cboBaseProduct.SelectedValue Is Nothing _
   OrElse IsDBNull(cboBaseProduct.SelectedValue) _
   OrElse TypeOf cboBaseProduct.SelectedValue Is DataRowView Then

                    cmd.Parameters.Add("@BaseProductID", SqlDbType.Int).Value = DBNull.Value

                Else
                    cmd.Parameters.Add("@BaseProductID", SqlDbType.Int).Value =
        CInt(cboBaseProduct.SelectedValue)
                End If


                ' =========================
                ' الأبعاد (اختيارية)
                ' =========================
                cmd.Parameters.Add("@HasDimensions", SqlDbType.Bit).Value = chkHasDimensions.Checked

                If txtLength.Text.Trim() = "" Then
                    cmd.Parameters.Add("@Length", SqlDbType.Decimal).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@Length", SqlDbType.Decimal).Value = Decimal.Parse(txtLength.Text.Trim())
                End If

                If txtWidth.Text.Trim() = "" Then
                    cmd.Parameters.Add("@Width", SqlDbType.Decimal).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@Width", SqlDbType.Decimal).Value = Decimal.Parse(txtWidth.Text.Trim())
                End If

                If txtHeight.Text.Trim() = "" Then
                    cmd.Parameters.Add("@Height", SqlDbType.Decimal).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@Height", SqlDbType.Decimal).Value = Decimal.Parse(txtHeight.Text.Trim())
                End If

                If txtDensity.Text.Trim() = "" Then
                    cmd.Parameters.Add("@Density", SqlDbType.Decimal).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@Density", SqlDbType.Decimal).Value = Decimal.Parse(txtDensity.Text.Trim())
                End If


                ' =========================
                ' الحالة
                ' =========================
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkProductStatus.Checked

                con.Open()
                cmd.ExecuteNonQuery()

            End Using
        End Using


    End Sub
    ' =========================
    ' Prepare New Product
    ' =========================
    Private Sub PrepareNewProduct()

        txtProductCode.Clear()
        txtProductName.Clear()
        txtProductEnglishName.Clear()
        txtBarcode.Clear()
        txtDescription.Clear()
        txtLength.Clear()
        txtWidth.Clear()
        txtHeight.Clear()
        txtDensity.Clear()

        cboCategory.SelectedIndex = -1
        cboSubCategory.SelectedIndex = -1

        cboProductGroup.SelectedIndex = -1
        cboBaseProduct.SelectedIndex = -1
        cboProductColor.SelectedIndex = -1
        cboPricingUnitID.SelectedIndex = -1
        cboProductTypeID.SelectedIndex = -1
        cboTaxType.SelectedValue = 1

        chkProductStatus.Checked = True
        btnSave.Enabled = True

        ToggleDimensionsControls(False)

        txtProductName.Focus()
        ' =========================
        ' Reset Base Product Logic
        ' =========================
        BaseProductCode = ""
        cboMixTypeID.SelectedIndex = -1
        cboStorageUnitID.SelectedIndex = -1
    End Sub
    Private Sub RefreshSaveButtonText()

        If CurrentProductID = 0 Then
            btnSave.Text = "حفظ"
        Else
            btnSave.Text = "تعديل"
        End If

    End Sub

    ' =========================
    ' Load Product By ID (for edit/search)
    ' =========================
    Private Sub LoadProductByID(productID As Integer)

        IsLoadingProduct = True

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT *
             FROM Master_Product
             WHERE ProductID = @ID", con)

                cmd.Parameters.AddWithValue("@ID", productID)

                con.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then

                        txtProductCode.Text = dr("ProductCode").ToString()
                        txtProductName.Text = dr("ProductName").ToString()
                        txtProductEnglishName.Text = dr("ProductEnglishName").ToString()
                        txtDescription.Text = dr("Description").ToString()
                        txtDensity.Text = dr("density").ToString

                        cboCategory.SelectedValue = dr("ProductCategoryID")
                        cboSubCategory.SelectedValue = dr("ProductSubCategoryID")
                        cboProductTypeID.SelectedValue = dr("ProductTypeID")
                        cboProductColor.SelectedValue = dr("ProductColorID")
                        cboProductGroup.SelectedValue = dr("ProductGroupID")
                        cboPricingUnitID.SelectedValue = dr("PricingUnitID")
                        cboTaxType.SelectedValue = dr("DefaultTaxTypeID")
                        cboStorageUnitID.SelectedValue = dr("StorageUnitID")
                        cboMixTypeID.SelectedValue = dr("MixTypeID")
                        ' =========================
                        ' Base Product (آمن)
                        ' =========================
                        If IsDBNull(dr("BaseProductID")) Then
                            cboBaseProduct.SelectedIndex = -1
                        Else
                            cboBaseProduct.SelectedValue = CInt(dr("BaseProductID"))
                        End If
                        ' =========================
                        ' Production Unit
                        ' =========================
                        If IsDBNull(dr("ProductionUnitID")) Then
                            cboProductionUnit.SelectedIndex = -1
                        Else
                            cboProductionUnit.SelectedValue = CInt(dr("ProductionUnitID"))
                        End If

                        ' =========================
                        ' Density (من Product فقط)
                        ' =========================
                        If IsDBNull(dr("Density")) Then
                            txtDensity.Text = ""
                        Else
                            txtDensity.Text = dr("Density").ToString()
                        End If

                        chkHasDimensions.Checked =
                        Not IsDBNull(dr("HasDimensions")) AndAlso CBool(dr("HasDimensions"))

                        chkProductStatus.Checked = CBool(dr("IsActive"))

                    End If
                End Using
            End Using
        End Using

        IsLoadingProduct = False

    End Sub
    ' =========================
    ' Buttons
    ' =========================
    Private Function ValidateProductCodingBeforeSave(
    productID As Integer,            ' 0 أو -1 إذا جديد (حسب نظامك)
    storageUnitID As Integer,
    baseProductID As Integer?,        ' Nothing إذا ما فيه أب
    m3UnitID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Boolean

        ' 1) المنتج M3 لا يكون له أب
        If storageUnitID = m3UnitID AndAlso baseProductID.HasValue AndAlso baseProductID.Value > 0 Then
            MessageBox.Show("لا يمكن ربط صنف وحدته متر مكعب (M3) بأب. يجب أن يكون الأب فارغًا.")
            Return False
        End If

        ' لا يوجد أب أصلاً => انتهينا
        If Not baseProductID.HasValue OrElse baseProductID.Value <= 0 Then
            Return True
        End If

        ' 2) منع M3 تحت أب M3
        Dim parentStorageUnitID As Integer = GetProductStorageUnitID(baseProductID.Value, con, tran)

        If parentStorageUnitID = m3UnitID AndAlso storageUnitID = m3UnitID Then
            MessageBox.Show("غير مسموح: الأب وحدته M3 والابن وحدته M3. هذا يسبب حلقة غير صحيحة.")
            Return False
        End If

        Return True
    End Function

    Private Function GetProductStorageUnitID(
    productID As Integer,
    con As SqlConnection,
    tran As SqlTransaction
) As Integer
        Using cmd As New SqlCommand("
        SELECT ISNULL(StorageUnitID, 0)
        FROM dbo.Master_Product
        WHERE ProductID = @ProductID;", con, tran)

            cmd.Parameters.AddWithValue("@ProductID", productID)

            Dim obj = cmd.ExecuteScalar()
            If obj Is Nothing OrElse obj Is DBNull.Value Then Return 0
            Return CInt(obj)
        End Using
    End Function
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        Dim isNew As Boolean = (CurrentProductID = 0)

        ' =========================
        ' حسم الوحدات
        ' =========================
        Dim purchaseUnitID As Integer
        Dim stockUnitID As Integer
        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using tran As SqlTransaction = con.BeginTransaction()

                Dim m3UnitID As Integer = 8 ' أو من إعداداتك

                Dim storageUnitID As Integer = CInt(cboStorageUnitID.SelectedValue)

                Dim baseProductID As Integer? = Nothing
                If cboBaseProduct.SelectedValue IsNot Nothing AndAlso CInt(cboBaseProduct.SelectedValue) > 0 Then
                    baseProductID = CInt(cboBaseProduct.SelectedValue)
                End If

                If Not ValidateProductCodingBeforeSave(
            productID:=CurrentProductID,
            storageUnitID:=storageUnitID,
            baseProductID:=baseProductID,
            m3UnitID:=m3UnitID,
            con:=con,
            tran:=tran
        ) Then
                    tran.Rollback()
                    Exit Sub
                End If

                ' ... كمل الحفظ ...
                tran.Commit()
            End Using
        End Using
        Try
            ResolveProductUnits(purchaseUnitID, stockUnitID)
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
            Exit Sub
        End Try

        ' =========================
        ' حسم كود الصنف
        ' =========================
        If chkHasDimensions.Checked Then

            txtProductCode.ReadOnly = True

            ' توليد الكود فقط في الإدخال اليدوي
            If Not OpenedFromCutting AndAlso Not OpenedFromSaleRequest Then
                txtProductCode.Text = GenerateProductCodeByDimensions()
            End If

            If txtProductCode.Text.Trim() = "" Then
                MessageBox.Show("كود الصنف غير موجود")
                Exit Sub
            End If

        Else
            txtProductCode.ReadOnly = False

            If txtProductCode.Text.Trim() = "" Then
                txtProductCode.Text = GetNextProductCode()
            End If
        End If

        ' =========================
        ' Validation
        ' =========================
        If Not ValidateProduct() Then Exit Sub

        ' =========================
        ' منع التكرار (خصوصًا من SR)
        ' =========================
        If isNew AndAlso IsProductCodeExists(
    txtProductCode.Text.Trim(),
    CInt(cboProductTypeID.SelectedValue)
) Then

            If OpenedFromSaleRequest OrElse OpenedFromCutting Then
                SavedProductID = GetProductIDByCode(txtProductCode.Text.Trim())
                Me.DialogResult = DialogResult.OK
                Me.Close()
                Exit Sub
            Else
                MessageBox.Show("كود الصنف موجود مسبقًا")
                Exit Sub
            End If

        End If

        ' =========================
        ' الحفظ الفعلي
        ' =========================
        If isNew Then
            InsertProduct(purchaseUnitID, stockUnitID)
            MessageBox.Show("تم إنشاء الصنف بنجاح", "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            UpdateProduct(purchaseUnitID, stockUnitID)
            MessageBox.Show("تم تعديل الصنف بنجاح", "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        btnSave.Enabled = False
        RefreshSaveButtonText()

        ' =========================
        ' إغلاق منظم بعد نجاح الحفظ
        ' =========================
        Me.DialogResult = DialogResult.OK


    End Sub
    Private Function GetProductIDByCode(productCode As String) As Integer

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductID FROM Master_Product WHERE ProductCode=@Code", con)

                cmd.Parameters.AddWithValue("@Code", productCode.Trim())
                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    Return CInt(obj)
                End If
            End Using
        End Using

        Return 0

    End Function

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        CurrentProductID = 0
        PrepareNewProduct()
        SetDefaultsForNewProduct()
        RefreshSaveButtonText()

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Using f As New frmProductSearch()
            If f.ShowDialog() = DialogResult.OK Then
                CurrentProductID = f.SelectedProductID
                '               LoadMixType()              ' ✅ هنا فقط
                LoadProductByID(CurrentProductID)
                RefreshSaveButtonText()

            End If
        End Using
        btnSave.Enabled = True
    End Sub


    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If CurrentProductID = 0 Then
            MessageBox.Show("لا يوجد صنف محدد للحذف")
            Exit Sub
        End If

        If MessageBox.Show("هل أنت متأكد من حذف الصنف؟",
                           "تأكيد الحذف",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = DialogResult.No Then
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "DELETE FROM Master_Product WHERE ProductID=@ID", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CurrentProductID

                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        MessageBox.Show("تم حذف الصنف بنجاح")
        CurrentProductID = 0
        PrepareNewProduct()

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    ' =========================
    ' Load HasDimensions from SubCategory
    ' =========================


    ' =========================
    ' Toggle Dimensions Controls
    ' =========================
    ' =========================
    ' Toggle Dimensions Controls
    ' =========================
    Private Sub ToggleDimensionsControls(showDimensions As Boolean)

        txtLength.Visible = showDimensions
        txtWidth.Visible = showDimensions
        txtHeight.Visible = showDimensions

        lblLength.Visible = showDimensions
        lblWidth.Visible = showDimensions
        lblHeight.Visible = showDimensions

        cboBaseProduct.Visible = showDimensions
        lblBaseProduct.Visible = showDimensions

        txtDensity.Visible = True
        lblDensity.Visible = True

        txtProductCode.ReadOnly = showDimensions

        If showDimensions Then
            LoadBaseProducts()
        Else
            txtLength.Clear()
            txtWidth.Clear()
            txtHeight.Clear()
            cboBaseProduct.SelectedIndex = -1
            BaseProductCode = ""
        End If

    End Sub
    ' =========================
    ' Check Product Code Exists
    ' =========================
    Private Function IsProductCodeExists(
    productCode As String,
    productTypeID As Integer
) As Boolean

        If String.IsNullOrWhiteSpace(productCode) Then Return False
        If productTypeID <= 0 Then Return False

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
        "SELECT COUNT(*)
         FROM Master_Product
         WHERE ProductCode = @Code
           AND ProductTypeID = @TypeID
           AND ProductID <> @ID", con)

                cmd.Parameters.AddWithValue("@Code", productCode.Trim())
                cmd.Parameters.AddWithValue("@TypeID", productTypeID)
                cmd.Parameters.AddWithValue("@ID", CurrentProductID)

                con.Open()
                Return CInt(cmd.ExecuteScalar()) > 0

            End Using
        End Using

    End Function

    ' =========================
    ' Numbers Only (with dot)
    ' =========================
    Private Sub NumbersOnly_KeyPress(sender As Object, e As KeyPressEventArgs)

        If Char.IsControl(e.KeyChar) Then Exit Sub

        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True
        End If

    End Sub

    ' =========================
    ' English + Numbers + Space
    ' =========================
    Private Sub EnglishOnly_KeyPress(sender As Object, e As KeyPressEventArgs)

        If Char.IsControl(e.KeyChar) Then Exit Sub

        ' حروف + أرقام + مسافة + رموز شائعة
        If Char.IsLetterOrDigit(e.KeyChar) OrElse
       e.KeyChar = " "c OrElse
       e.KeyChar = "-"c OrElse
       e.KeyChar = "_"c OrElse
       e.KeyChar = "."c OrElse
       e.KeyChar = "/"c OrElse
       e.KeyChar = "("c OrElse
       e.KeyChar = ")"c OrElse
       e.KeyChar = "+"c Then
            Exit Sub
        End If

        e.Handled = True
    End Sub
    Public Sub PrefillFromSaleRequest(
    productCode As String,
    baseProductCode As String,
    productTypeID As Integer,
    sellUnitID As Integer,
    lengthCM As Decimal,
    widthCM As Decimal,
    heightCM As Decimal
)

        OpenedFromSaleRequest = True
        HasSaleRequestPrefill = True

        SR_ProductCode = productCode
        SR_BaseProductCode = baseProductCode
        SR_ProductTypeID = productTypeID
        SR_SellUnitID = sellUnitID
        SR_L = lengthCM
        SR_W = widthCM
        SR_H = heightCM
        ' 🔥 النقطة الناقصة
        If Not String.IsNullOrWhiteSpace(SR_BaseProductCode) Then

            ApplySaleRequestDefaultsFromBaseProduct()
            chkHasDimensions.Checked = True

        End If

    End Sub


    Private Sub ApplySaleRequestDefaultsFromBaseProduct()

        If String.IsNullOrWhiteSpace(SR_BaseProductCode) Then Exit Sub

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT *
         FROM Master_Product
         WHERE ProductCode = @Code", con)

                cmd.Parameters.AddWithValue("@Code", SR_BaseProductCode.Trim())
                con.Open()

                Using dr As SqlDataReader = cmd.ExecuteReader()
                    If Not dr.Read() Then Exit Sub

                    ' =========================
                    ' الأسماء
                    ' =========================
                    txtProductName.Text = dr("ProductName").ToString()
                    txtProductEnglishName.Text =
                        If(IsDBNull(dr("ProductEnglishName")),
                           "",
                           dr("ProductEnglishName").ToString())

                    ' =========================
                    ' التصنيفات
                    ' =========================
                    cboCategory.SelectedValue = 3   ' FG

                    If Not IsDBNull(dr("ProductSubCategoryID")) Then
                        cboSubCategory.SelectedValue = dr("ProductSubCategoryID")
                    End If

                    cboProductTypeID.SelectedValue = SR_ProductTypeID

                    If Not IsDBNull(dr("ProductGroupID")) Then
                        cboProductGroup.SelectedValue = dr("ProductGroupID")
                    End If

                    If Not IsDBNull(dr("ProductColorID")) Then
                        cboProductColor.SelectedValue = dr("ProductColorID")
                    End If

                    ' =========================
                    ' الكثافة
                    ' =========================
                    If Not IsDBNull(dr("Density")) Then
                        txtDensity.Text = dr("Density").ToString()
                    End If

                    ' =========================
                    ' الوحدات
                    ' =========================
                    cboStorageUnitID.SelectedValue = 12   ' PCS

                    If Not IsDBNull(dr("PricingUnitID")) Then
                        cboPricingUnitID.SelectedValue = dr("PricingUnitID")
                    End If

                    ' =========================
                    ' المخزن
                    ' =========================
                    ' =========================
                    ' MixType = NULL
                    ' =========================
                    cboMixTypeID.SelectedIndex = -1

                End Using
            End Using
        End Using

    End Sub



    Private Sub ApplyCuttingPrefill()

        If Not OpenedFromCutting OrElse Not hasPrefill Then Exit Sub

        ' =========================
        ' 1) قيم من الجريد (الممررة)
        ' =========================
        txtProductCode.Text = preCode

        chkHasDimensions.Checked = True
        txtLength.Text = preL.ToString()
        txtWidth.Text = preW.ToString()
        txtHeight.Text = preH.ToString()

        ' =========================
        ' 2) قيم ثابتة (IDs)
        ' =========================
        cboProductTypeID.SelectedValue = 3
        cboProductGroup.SelectedValue = 1
        cboStorageUnitID.SelectedValue = 2
        cboTaxType.SelectedValue = 1
        cboPricingUnitID.SelectedValue = 3

        ' =========================
        ' 3) من الأب (اعتماداً على baseProductCode الذي تمرّره الآن)
        '    أنت تمرر cboProductCode.Text (اسم/كود الأب) كـ baseProductCode
        '    سنجلب منه بيانات الاسم/التصنيف/الكثافة/اللون/الـ BaseProduct
        ' =========================
        If Not String.IsNullOrWhiteSpace(preBaseProductCode) Then
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT TOP 1 *
                FROM Master_Product
                WHERE ProductCode = @Code
            ", con)

                    cmd.Parameters.AddWithValue("@Code", preBaseProductCode.Trim())

                    con.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            ' الأسماء
                            txtProductName.Text = dr("ProductName").ToString()
                            txtProductEnglishName.Text =
                                If(IsDBNull(dr("ProductEnglishName")), "", dr("ProductEnglishName").ToString())

                            ' التصنيف
                            If Not IsDBNull(dr("CategoryID")) Then
                                cboCategory.SelectedValue = dr("CategoryID")
                            End If

                            ' الكثافة
                            If Not IsDBNull(dr("Density")) Then
                                txtDensity.Text = dr("Density").ToString()
                            End If

                            ' اللون
                            If Not IsDBNull(dr("ProductColorID")) Then
                                cboProductColor.SelectedValue = dr("ProductColorID")
                            End If

                            ' الـ BaseProduct (يعني المنتج الأب نفسه)
                            ' إذا عندك Combo للـ BaseProduct مربوط بـ ProductID:
                            If Not IsDBNull(dr("ProductID")) Then
                                cboBaseProduct.SelectedValue = dr("ProductID")
                            End If

                        End If
                    End Using
                End Using
            End Using
        End If

        ' مخزن افتراضي (إذا عندك Combo خاص بالمخزن داخل frmProduct)
        '        If targetStoreID > 0 Then
        ' غيّر اسم الكمبو حسب مشروعك إن وجد:
        ' cboDefaultStore.SelectedValue = targetStoreID
        '       End If

    End Sub
End Class
