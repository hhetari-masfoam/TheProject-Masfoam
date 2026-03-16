Imports System.Data.SqlClient

Public Class frmSaleRequest
    Inherits AABaseOperationForm

    Private Const FORM_SCOPE As String = "BUS"
    Private Const FORM_OPERATION_TYPE_ID As Integer = 1   '  من جدول OperationType

    ' =========================
    ' Edit State
    ' =========================
    Private IsEditing As Boolean = False
    Private EditingOriginalValues As Dictionary(Of String, Object) = Nothing
    Private EditingOriginalProductCode As String = ""
    ' =========================
    ' ProductTypeCode Cache
    ' =========================
    Private ProductTypeCodeCache As New Dictionary(Of Integer, String)

    ' =========================
    ' TaxRate Cache (تحسين بسيط)
    ' =========================
    Private TaxRateCache As New Dictionary(Of Integer, Decimal)
    ' =========================
    ' Save State
    ' =========================
    Private IsSaved As Boolean = False
    Private IsFormLoading As Boolean = False
    Private CurrentBusinessStatusID As Integer = 0
    Private CurrentSRID As Integer
    Private CurrentStatusID As Integer = 0
    ' =========================
    ' Fulfillment / Physical Status
    ' =========================
    Private CurrentPolicy As EditPolicy




    Private Function GetTaxRate(taxTypeID As Integer) As Decimal

        ' حماية: لا توجد ضريبة
        If taxTypeID <= 0 Then
            Return 0D
        End If

        ' Cache
        If TaxRateCache.ContainsKey(taxTypeID) Then
            Return TaxRateCache(taxTypeID)
        End If

        Dim rate As Decimal

        Using con As New SqlClient.SqlConnection(ConnStr)
            Using cmd As New SqlClient.SqlCommand(
            "SELECT TaxRate
             FROM Master_TaxType
             WHERE TaxTypeID = @TaxTypeID
               AND IsActive = 1", con)

                cmd.Parameters.Add("@TaxTypeID", SqlDbType.Int).Value = taxTypeID

                con.Open()

                Dim obj = cmd.ExecuteScalar()

                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Throw New Exception(
                    "نوع الضريبة غير موجود أو غير مفعل. TaxTypeID=" & taxTypeID
                )
                End If

                rate = CDec(obj)
            End Using
        End Using

        ' تخزين في الكاش
        TaxRateCache(taxTypeID) = rate

        Return rate

    End Function

    Private Function PieceVolM3_FromCM(l As Decimal, w As Decimal, h As Decimal) As Decimal
        If l <= 0 OrElse w <= 0 OrElse h <= 0 Then Return 0D
        Return (l * w * h) / 1000000D
    End Function
    Private Function GetProductTypeCode(productTypeID As Integer) As String

        If productTypeID <= 0 Then
            Throw New Exception("ProductTypeID غير صالح (قيمة صفر أو سالبة).")
        End If

        ' 🔹 كاش أولاً
        If ProductTypeCodeCache.ContainsKey(productTypeID) Then
            Return ProductTypeCodeCache(productTypeID)
        End If

        Dim code As String = Nothing

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlClient.SqlCommand(
            "SELECT TypeCode FROM Master_ProductType WHERE ProductTypeID = @ID", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = productTypeID

                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    code = obj.ToString()
                End If
            End Using
        End Using

        If String.IsNullOrWhiteSpace(code) Then
            Throw New Exception("TypeCode غير موجود للنوع ProductTypeID=" & productTypeID)
        End If

        ProductTypeCodeCache(productTypeID) = code
        Return code

    End Function
    Private Structure LineCalcResult
        Public TotalBeforeDiscount As Decimal
        Public DiscountAmount As Decimal
        Public AmountAfterDiscount As Decimal
        Public TaxAmount As Decimal
        Public TotalAfterVAT As Decimal
        Public VolumePerUnit As Decimal
        Public TotalVolume As Decimal
        Public PricePerM3 As Decimal
    End Structure
    Private Function CalculateSaleLine(
    Quantity As Decimal,
    piecePrice As Decimal,
    DiscountRate As Decimal,
    taxRatePercent As Decimal,
    l As Decimal,
    w As Decimal,
    h As Decimal
) As LineCalcResult

        Dim res As New LineCalcResult

        ' =========================
        ' الحجم
        ' =========================
        res.VolumePerUnit = PieceVolM3_FromCM(l, w, h)
        res.TotalVolume = Math.Round(res.VolumePerUnit * Quantity, 6)

        If res.VolumePerUnit > 0D AndAlso piecePrice > 0D Then
            res.PricePerM3 = Math.Round(piecePrice / res.VolumePerUnit, 2)
        End If

        ' =========================
        ' الحسابات المالية
        ' =========================
        res.TotalBeforeDiscount = Math.Round(Quantity * piecePrice, 2)

        If DiscountRate > 0D Then
            res.DiscountAmount =
            Math.Round(res.TotalBeforeDiscount * (DiscountRate / 100D), 2)
        End If

        res.AmountAfterDiscount =
        Math.Round(res.TotalBeforeDiscount - res.DiscountAmount, 2)

        If taxRatePercent > 0D Then
            res.TaxAmount =
            Math.Round(res.AmountAfterDiscount * (taxRatePercent / 100D), 2)
        End If

        res.TotalAfterVAT =
        Math.Round(res.AmountAfterDiscount + res.TaxAmount, 2)

        Return res

    End Function


    Private Sub dgvSRDetails_CellFormatting(
    sender As Object,
    e As DataGridViewCellFormattingEventArgs
) Handles dgvSRDetails.CellFormatting

        If dgvSRDetails.Columns(e.ColumnIndex).Name = "colDetDiscount" Then

            If e.Value IsNot Nothing AndAlso IsNumeric(e.Value) Then
                e.Value = e.Value.ToString() & "%"
                e.FormattingApplied = True
            End If

        End If

    End Sub
    Private Sub dgvSRDetails_CellParsing(
    sender As Object,
    e As DataGridViewCellParsingEventArgs
) Handles dgvSRDetails.CellParsing

        If dgvSRDetails.Columns(e.ColumnIndex).Name = "colDetDiscount" Then

            If e.Value IsNot Nothing Then
                Dim s As String = e.Value.ToString().Replace("%", "").Trim()

                Dim d As Decimal
                If Decimal.TryParse(s, d) Then
                    e.Value = d          ' 👈 تخزين 15
                    e.ParsingApplied = True
                End If
            End If

        End If

    End Sub
    Private Sub CalcLineAmounts(
    Quantity As Decimal,
    piecePrice As Decimal,
    DiscountRate As Decimal,
    taxRatePercent As Decimal,
    ByRef totalBeforeDiscount As Decimal,
    ByRef discountAmount As Decimal,
    ByRef amountAfterDiscount As Decimal,
    ByRef TaxAmount As Decimal,
    ByRef totalAfterVAT As Decimal
)

        ' =========================
        ' الإجمالي قبل الخصم
        ' =========================
        totalBeforeDiscount = Math.Round(Quantity * piecePrice, 2)

        ' =========================
        ' الخصم
        ' =========================
        discountAmount = 0D
        If DiscountRate > 0D Then
            discountAmount =
            Math.Round(totalBeforeDiscount * (DiscountRate / 100D), 2)
        End If

        ' =========================
        ' الصافي بعد الخصم
        ' =========================
        amountAfterDiscount =
        Math.Round(totalBeforeDiscount - discountAmount, 2)

        ' =========================
        ' VAT
        ' =========================
        TaxAmount = 0D
        If taxRatePercent > 0D Then
            TaxAmount =
            Math.Round(amountAfterDiscount * (taxRatePercent / 100D), 2)
        End If

        ' =========================
        ' الإجمالي بعد الضريبة
        ' =========================
        totalAfterVAT =
        Math.Round(amountAfterDiscount + TaxAmount, 2)

    End Sub

    Private Function CaptureDetailRowValues(r As DataGridViewRow) As Dictionary(Of String, Object)

        Dim d As New Dictionary(Of String, Object)

        ' نحفظ فقط أعمدة التفاصيل التي نحتاجها للرجوع بدون تعديل
        d("colDetVAT") = r.Cells("colDetVAT").Value
        d("colDetProductCode") = r.Cells("colDetProductCode").Value
        d("colDetBaseProductCode") = r.Cells("colDetBaseProductCode").Value
        d("colDetType") = r.Cells("colDetType").Value
        d("colDetSellUnit") = r.Cells("colDetSellUnit").Value
        d("colDetLength") = r.Cells("colDetLength").Value
        d("colDetWidth") = r.Cells("colDetWidth").Value
        d("colDetHeight") = r.Cells("colDetHeight").Value
        d("colDetQty") = r.Cells("colDetQty").Value
        d("colDetPieceSellPrice") = r.Cells("colDetPieceSellPrice").Value
        d("colDetDiscount") = r.Cells("colDetDiscount").Value
        d("colDetNote") = r.Cells("colDetNote").Value

        Return d

    End Function
    Private Sub CalcAmounts(
    Quantity As Decimal,
    piecePrice As Decimal,
    DiscountRate As Decimal,
    ByRef totalBeforeDiscount As Decimal,
    ByRef discountAmount As Decimal,
    ByRef amountAfterDiscount As Decimal
)

        totalBeforeDiscount = Math.Round(Quantity * piecePrice, 2)

        discountAmount = 0D
        If DiscountRate > 0D Then
            discountAmount =
            Math.Round(totalBeforeDiscount * (DiscountRate / 100D), 2)
        End If

        amountAfterDiscount =
        Math.Round(totalBeforeDiscount - discountAmount, 2)

    End Sub
    Private Sub RecalcInputRow(row As DataGridViewRow)

        If row Is Nothing Then Exit Sub

        Dim l As Decimal = GetDec(row.Cells("colLength").Value)
        Dim w As Decimal = GetDec(row.Cells("colWidth").Value)
        Dim h As Decimal = GetDec(row.Cells("colHeight").Value)

        Dim Quantity As Decimal = GetDec(row.Cells("colQty").Value)
        Dim piecePrice As Decimal = GetDec(row.Cells("colPieceSellPrice").Value)
        Dim DiscountRate As Decimal = GetDec(row.Cells("colDiscount").Value)



        Dim taxTypeID As Integer = CInt(row.Cells("colInputTaxID").Value)
        Dim TaxRate As Decimal = GetTaxRate(taxTypeID)

        Dim calc As LineCalcResult =
        CalculateSaleLine(
            Quantity,
            piecePrice,
            DiscountRate,
            TaxRate,
            l, w, h
        )

        row.Cells("colTotalAmount").Value = calc.TotalBeforeDiscount
        row.Cells("colDiscountAmount").Value = calc.DiscountAmount
        row.Cells("colAmountAfterDiscount").Value = calc.AmountAfterDiscount
        row.Cells("colInputVATAmount").Value = calc.TaxAmount
        row.Cells("colInputTotalAfterVAT").Value = calc.TotalAfterVAT

        If calc.VolumePerUnit > 0D Then
            row.Cells("colM3SellPrice").Value = calc.PricePerM3
        End If

    End Sub

    Private Sub RecalcDetailRow(r As DataGridViewRow)

        If r Is Nothing Then Exit Sub

        Dim l As Decimal = GetDec(r.Cells("colDetLength").Value)
        Dim w As Decimal = GetDec(r.Cells("colDetWidth").Value)
        Dim h As Decimal = GetDec(r.Cells("colDetHeight").Value)

        Dim Quantity As Decimal = GetDec(r.Cells("colDetQty").Value)
        Dim piecePrice As Decimal = GetDec(r.Cells("colDetPieceSellPrice").Value)
        Dim DiscountRate As Decimal = GetDec(r.Cells("colDetDiscount").Value)

        Dim taxTypeID As Integer = 0
        If r.Cells("colDetVAT").Value IsNot Nothing Then
            taxTypeID = CInt(r.Cells("colDetVAT").Value)
        End If

        Dim TaxRate As Decimal = GetTaxRate(taxTypeID)

        Dim calc As LineCalcResult =
        CalculateSaleLine(
            Quantity,
            piecePrice,
            DiscountRate,
            TaxRate,
            l, w, h
        )

        r.Cells("colDetVolumePerUnit").Value = calc.VolumePerUnit
        r.Cells("colDetTotalVolume").Value = calc.TotalVolume
        r.Cells("colDetM3SellPrice").Value = calc.PricePerM3

        r.Cells("colDetDiscountAmount").Value = calc.DiscountAmount
        r.Cells("colDetAmountBeforeDiscount").Value = calc.TotalBeforeDiscount
        r.Cells("colDetTotalAmountBFVAT").Value = calc.AmountAfterDiscount
        r.Cells("colDetVATAmount").Value = calc.TaxAmount
        r.Cells("colDetTotalAmount").Value = calc.TotalAfterVAT

    End Sub
    Private Function ValidateDetailRow(
    r As DataGridViewRow,
    ByRef errMsg As String
) As Boolean

        errMsg = ""

        If r Is Nothing OrElse r.IsNewRow Then
            errMsg = "سطر تفاصيل غير صالح."
            Return False
        End If

        ' =========================
        ' الكمية
        ' =========================
        Dim Quantity As Decimal = GetDec(r.Cells("colDetQty").Value)
        If Quantity <= 0D Then
            errMsg = "الكمية يجب أن تكون أكبر من صفر."
            Return False
        End If

        ' =========================
        ' سعر الحبة
        ' =========================
        Dim unitPrice As Decimal = GetDec(r.Cells("colDetPieceSellPrice").Value)
        If unitPrice < 0D Then
            errMsg = "سعر الحبة لا يمكن أن يكون سالبًا."
            Return False
        End If

        ' =========================
        ' الخصم
        ' =========================
        Dim discPct As Decimal = GetDec(r.Cells("colDetDiscount").Value)
        If discPct < 0D OrElse discPct > 100D Then
            errMsg = "نسبة الخصم يجب أن تكون بين 0 و 100."
            Return False
        End If

        ' =========================
        ' الأبعاد (لو السعر موجود)
        ' =========================
        Dim l As Decimal = GetDec(r.Cells("colDetLength").Value)
        Dim w As Decimal = GetDec(r.Cells("colDetWidth").Value)
        Dim h As Decimal = GetDec(r.Cells("colDetHeight").Value)

        If unitPrice > 0D AndAlso (l <= 0D OrElse w <= 0D OrElse h <= 0D) Then
            errMsg = "الأبعاد (طول / عرض / ارتفاع) يجب أن تكون أكبر من صفر."
            Return False
        End If

        ' =========================
        ' ProductType
        ' =========================
        If r.Cells("colDetType").Value Is Nothing _
       OrElse IsDBNull(r.Cells("colDetType").Value) _
       OrElse CInt(r.Cells("colDetType").Value) <= 0 Then

            errMsg = "نوع المنتج غير محدد."
            Return False
        End If

        ' =========================
        ' وحدة البيع
        ' =========================
        If r.Cells("colDetSellUnit").Value Is Nothing _
       OrElse IsDBNull(r.Cells("colDetSellUnit").Value) _
       OrElse CInt(r.Cells("colDetSellUnit").Value) <= 0 Then

            errMsg = "وحدة البيع غير محددة."
            Return False
        End If

        ' =========================
        ' الضريبة
        ' =========================
        If r.Cells("colDetVAT").Value Is Nothing _
       OrElse IsDBNull(r.Cells("colDetVAT").Value) _
       OrElse CInt(r.Cells("colDetVAT").Value) <= 0 Then

            errMsg = "يجب اختيار نوع الضريبة."
            Return False
        End If

        Return True

    End Function
    Private Function ValidateSaleRequestBeforeSave(ByRef errMsg As String) As Boolean

        errMsg = ""

        ' تحقق من الهيدر
        If cboPartnerCode.SelectedValue Is Nothing Then
            errMsg = "العميل غير محدد."
            Return False
        End If

        If cboSRepCode.SelectedValue Is Nothing Then
            errMsg = "مندوب المبيعات غير محدد."
            Return False
        End If

        If cboStoreCode.SelectedValue Is Nothing Then
            errMsg = "المستودع غير محدد."
            Return False
        End If

        ' تحقق من التفاصيل
        Dim hasValidDetail As Boolean = False

        For Each r As DataGridViewRow In dgvSRDetails.Rows
            If r.IsNewRow Then Continue For

            Dim lineErr As String = ""
            If Not ValidateDetailRow(r, lineErr) Then
                errMsg = "خطأ في سطر التفاصيل (" &
                     r.Cells("colDetProductCode").Value.ToString() &
                     "): " & lineErr
                Return False
            End If

            hasValidDetail = True
        Next

        If Not hasValidDetail Then
            errMsg = "لا يمكن حفظ الطلب بدون سطور تفاصيل."
            Return False
        End If

        Return True

    End Function

    Private Sub ResetInputToAddMode()

        If dgvSRInputs.Rows.Count = 0 Then dgvSRInputs.Rows.Add()
        Dim inputRow As DataGridViewRow = dgvSRInputs.Rows(0)

        inputRow.Cells("colAdd").Value = "Add"

        ClearInputRow(inputRow)

        ' =========================
        ' تعيين ضريبة افتراضية (VAT 15%)
        ' =========================
        inputRow.Cells("colInputTaxID").Value = 1

    End Sub


    Private Function DetailProductCodeExists(productCode As String, Optional excludeProductCode As String = "") As Boolean

        For Each r As DataGridViewRow In dgvSRDetails.Rows
            If r.IsNewRow Then Continue For
            Dim v As String = If(r.Cells("colDetProductCode").Value, "").ToString()
            If v = "" Then Continue For

            If v = productCode AndAlso v <> excludeProductCode Then
                Return True
            End If
        Next

        Return False

    End Function

    Private Function IsInputRowValid(row As DataGridViewRow) As Boolean

        ' الصنف الأساسي
        If row.Cells("colBaseProductCode").Value Is Nothing Then Return False

        ' النوع
        If row.Cells("colType").Value Is Nothing Then Return False

        ' الكمية
        If GetDec(row.Cells("colQty").Value) <= 0 Then Return False

        ' سعر الحبة
        If GetDec(row.Cells("colPieceSellPrice").Value) <= 0 Then Return False

        Return True

    End Function

    Private EditingDetailRowIndex As Integer = -1

    Private Function GenerateSRCode() As String

        Dim yy As String = Date.Now.ToString("yy")
        Dim nextSeq As Integer = 1

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
            "SELECT MAX(SRCode) 
             FROM Business_SR 
             WHERE SRCode LIKE 'SR-" & yy & "-%'"

            Using cmd As New SqlClient.SqlCommand(sql, con)
                Dim obj = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    Dim lastCode As String = obj.ToString()
                    Dim parts() As String = lastCode.Split("-"c)
                    If parts.Length = 3 Then
                        Integer.TryParse(parts(2), nextSeq)
                        nextSeq += 1
                    End If
                End If
            End Using
        End Using

        Return $"SR-{yy}-{nextSeq.ToString("000000")}"

    End Function
    Private Sub RefreshUI()

        btnSaveSR.Enabled = CurrentPolicy.AllowEditData
        btnSRDelete.Enabled = CurrentPolicy.IsCancelable

        dgvSRInputs.ReadOnly = Not CurrentPolicy.AllowEditData
        dgvSRDetails.ReadOnly = Not CurrentPolicy.AllowEditQuantity

        dgvSRDetails.Columns("colDetQty").ReadOnly =
        Not CurrentPolicy.AllowEditQuantity

        dgvSRDetails.Columns("colDetPieceSellPrice").ReadOnly =
        Not CurrentPolicy.AllowEditCost

    End Sub

    Private Sub frmSaleRequest_Load(
    sender As Object,
    e As EventArgs
) Handles MyBase.Load

        IsFormLoading = True

        ' ==================================================
        ' 1) تحميل البيانات المرجعية (Lookups)
        ' ==================================================
        LoadPartners()
        LoadSalesReps()
        LoadStores()
        LoadSellUnits()
        LoadProductTypes()
        LoadBaseProductsForSR()
        LoadTaxTypesForDetails()
        LoadTaxTypesForInputs()
        btnSaveSR.Text = "حفظ"

        ' ==================================================
        ' 2) حجم الفورم 95%
        ' ==================================================
        Dim screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea

        Me.Width = CInt(screenRect.Width * 0.95)
        Me.Height = CInt(screenRect.Height * 0.95)

        Me.StartPosition = FormStartPosition.Manual
        Me.Left = screenRect.Left + (screenRect.Width - Me.Width) \ 2
        Me.Top = screenRect.Top + (screenRect.Height - Me.Height) \ 2

        ' ==================================================
        ' 3) إعداد الجريدات (Details)
        ' ==================================================
        With dgvSRDetails
            .AllowUserToAddRows = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .ScrollBars = ScrollBars.Vertical
        End With

        ' ----- نوع المنتج -----
        Dim colType As DataGridViewComboBoxColumn =
        CType(dgvSRDetails.Columns("colDetType"), DataGridViewComboBoxColumn)

        colType.DataSource = GetProductTypesTable()
        colType.DisplayMember = "TypeName"
        colType.ValueMember = "ProductTypeID"
        colType.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing

        ' ----- وحدة البيع -----
        Dim colUnit As DataGridViewComboBoxColumn =
        CType(dgvSRDetails.Columns("colDetSellUnit"), DataGridViewComboBoxColumn)

        colUnit.DataSource = GetUnitsTable()
        colUnit.DisplayMember = "UnitName"
        colUnit.ValueMember = "UNITID"
        colUnit.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing

        ' ==================================================
        ' 4) تنسيقات العرض فقط (Details)
        ' ==================================================
        dgvSRDetails.Columns("colDetDiscount").ValueType = GetType(Decimal)
        dgvSRDetails.Columns("colDetDiscount").DefaultCellStyle.Format = "0"
        dgvSRDetails.Columns("colDetDiscount").DefaultCellStyle.Alignment =
        DataGridViewContentAlignment.MiddleRight

        dgvSRDetails.Columns("colDetLength").DefaultCellStyle.Format = "0.0"
        dgvSRDetails.Columns("colDetWidth").DefaultCellStyle.Format = "0.0"
        dgvSRDetails.Columns("colDetHeight").DefaultCellStyle.Format = "0.0"

        dgvSRDetails.Columns("colDetPieceSellPrice").DefaultCellStyle.Format = "0.0"
        dgvSRDetails.Columns("colDetM3SellPrice").DefaultCellStyle.Format = "0.0"

        dgvSRDetails.Columns("colDetVolumePerUnit").DefaultCellStyle.Format = "0.000"
        dgvSRDetails.Columns("colDetTotalVolume").DefaultCellStyle.Format = "0.000"

        dgvSRDetails.Columns("colDetDiscountAmount").DefaultCellStyle.Format = "0.000"
        dgvSRDetails.Columns("colDetAmountBeforeDiscount").DefaultCellStyle.Format = "0.000"
        dgvSRDetails.Columns("colDetTotalAmountBFVAT").DefaultCellStyle.Format = "0.000"
        dgvSRDetails.Columns("colDetVATAmount").DefaultCellStyle.Format = "0.000"
        dgvSRDetails.Columns("colDetTotalAmount").DefaultCellStyle.Format = "0.000"

        ' ==================================================
        ' 5) توزيع عرض الأعمدة (FillWeight)
        ' ==================================================
        dgvSRDetails.Columns("colDetProductCode").FillWeight = 100
        dgvSRDetails.Columns("colDetType").FillWeight = 50
        dgvSRDetails.Columns("colDetLength").FillWeight = 50
        dgvSRDetails.Columns("colDetWidth").FillWeight = 50
        dgvSRDetails.Columns("colDetHeight").FillWeight = 50
        dgvSRDetails.Columns("colDetQty").FillWeight = 50
        dgvSRDetails.Columns("colDetSellUnit").FillWeight = 50
        dgvSRDetails.Columns("colDetTotalAmount").FillWeight = 50
        dgvSRDetails.Columns("colDetPieceSellPrice").FillWeight = 50
        dgvSRDetails.Columns("colDetM3SellPrice").FillWeight = 50
        dgvSRDetails.Columns("colDetDiscountAmount").FillWeight = 50
        dgvSRDetails.Columns("colDetAmountBeforeDiscount").FillWeight = 50
        dgvSRDetails.Columns("colDetVolumePerUnit").FillWeight = 50
        dgvSRDetails.Columns("colDetTotalVolume").FillWeight = 50

        ' ==================================================
        ' 6) إعداد جريد الإدخال (Inputs)
        ' ==================================================
        With dgvSRInputs
            .AllowUserToAddRows = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .ScrollBars = ScrollBars.Vertical
        End With

        dgvSRInputs.Columns("colLength").DefaultCellStyle.Format = "0.0"
        dgvSRInputs.Columns("colWidth").DefaultCellStyle.Format = "0.0"
        dgvSRInputs.Columns("colHeight").DefaultCellStyle.Format = "0.0"

        dgvSRInputs.Columns("colPieceSellPrice").DefaultCellStyle.Format = "0.0"
        dgvSRInputs.Columns("colM3SellPrice").DefaultCellStyle.Format = "0.0"

        dgvSRInputs.Columns("colBaseProductCode").FillWeight = 100
        dgvSRInputs.Columns("colType").FillWeight = 80
        dgvSRInputs.Columns("colLength").FillWeight = 50
        dgvSRInputs.Columns("colWidth").FillWeight = 50
        dgvSRInputs.Columns("colHeight").FillWeight = 50
        dgvSRInputs.Columns("colQty").FillWeight = 50
        dgvSRInputs.Columns("colSellUnit").FillWeight = 50

        ' ==================================================
        ' 7) صف الإدخال الافتراضي
        ' ==================================================
        dgvSRInputs.Rows.Clear()
        dgvSRInputs.Rows.Add()

        Dim inputRow As DataGridViewRow = dgvSRInputs.Rows(0)
        inputRow.Cells("colType").Value = 2
        inputRow.Cells("colSellUnit").Value = 12
        inputRow.Cells("colInputTaxID").Value = 1

        ' ==================================================
        ' 8) قيم الهيدر الافتراضية
        ' ==================================================
        txtSRCode.Text = GenerateSRCode()
        dtpSRDate.Value = Date.Today
        dtpSRDeliveryDate.Value = Date.Today
        chkIsActive.Checked = True

        cboPartnerCode.SelectedIndex = -1
        txtPartnerName.Clear()
        txtPartnerPhone.Clear()

        cboSRepCode.SelectedIndex = -1
        txtSRepName.Clear()

        If cboStoreCode.Items.Count > 0 Then
            cboStoreCode.SelectedValue = 4
        End If

        ' ==================================================
        ' 9) تحديد الحالة الابتدائية + قراءة Policy
        ' ==================================================
        CurrentStatusID =
        Workflow_OperationPolicyHelper.GetInitialStatusByScope(FORM_SCOPE)

        CurrentPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            FORM_OPERATION_TYPE_ID,
            CurrentStatusID
        )
        txtSRStatus.Text =
    Workflow_OperationPolicyHelper.GetStatusName(CurrentStatusID)

        ' ==================================================
        ' 10) عكس الـ Policy على الواجهة
        ' ==================================================
        RefreshUI()
        IsFormLoading = False

    End Sub

    Private Sub LoadStores()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT 
            StoreID,
            StoreName
         FROM Master_Store
         WHERE IsActive = 1
         ORDER BY StoreName"

            Using da As New SqlClient.SqlDataAdapter(sql, con)

                Dim dt As New DataTable
                da.Fill(dt)

                cboStoreCode.DisplayMember = "StoreName"  ' المعروض للمستخدم
                cboStoreCode.ValueMember = "StoreID"           ' المخزن فعليًا
                cboStoreCode.DataSource = dt

            End Using
        End Using

    End Sub

    Private Sub LoadPartners()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT p.PartnerID, p.PartnerCode, p.PartnerName, p.Phone
         FROM Master_Partner p
         INNER JOIN Master_PartnerRole pr ON pr.PartnerID = p.PartnerID
         WHERE pr.RoleCode = 'CUSTOMER'
           AND p.IsActive = 1
         ORDER BY p.PartnerCode"

            Using da As New SqlClient.SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)

                cboPartnerCode.DisplayMember = "PartnerCode"
                cboPartnerCode.ValueMember = "PartnerID"
                cboPartnerCode.DataSource = dt   ' ✅ هذا هو المهم
                cboPartnerCode.SelectedIndex = -1
            End Using
        End Using

    End Sub

    Private Sub LoadTaxTypesForInputs()

        Using con As New SqlConnection(ConnStr)
            Dim dt As New DataTable

            Dim sql As String =
        "SELECT TaxTypeID, TaxName
         FROM Master_TaxType
         WHERE IsActive = 1
         ORDER BY TaxTypeID"

            Using da As New SqlDataAdapter(sql, con)
                da.Fill(dt)
            End Using

            Dim col As DataGridViewComboBoxColumn =
            CType(dgvSRInputs.Columns("colInputTaxID"), DataGridViewComboBoxColumn)

            col.DataSource = dt
            col.DisplayMember = "TaxName"
            col.ValueMember = "TaxTypeID"
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        End Using

        ' 🔹 القيمة الافتراضية = VAT 15%
        If dgvSRInputs.Rows.Count > 0 Then
            dgvSRInputs.Rows(0).Cells("colInputTaxID").Value = 1
        End If

    End Sub

    Private Sub LoadTaxTypesForDetails()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT TaxTypeID, TaxName
         FROM Master_TaxType
         WHERE IsActive = 1
         ORDER BY TaxName"

            Using da As New SqlClient.SqlDataAdapter(sql, con)

                Dim dt As New DataTable
                da.Fill(dt)

                ' =========================
                ' التحقق من العمود
                ' =========================
                If Not dgvSRDetails.Columns.Contains("colDetVAT") Then
                    MessageBox.Show(
                    "العمود colDetVAT غير موجود في dgvSRDetails",
                    "خطأ تصميم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Exit Sub
                End If

                Dim col As DataGridViewComboBoxColumn =
                TryCast(dgvSRDetails.Columns("colDetVAT"),
                        DataGridViewComboBoxColumn)

                If col Is Nothing Then
                    MessageBox.Show(
                    "العمود colDetVAT ليس ComboBoxColumn",
                    "خطأ تصميم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Exit Sub
                End If

                ' =========================
                ' الربط
                ' =========================
                col.DataSource = dt
                col.DisplayMember = "TaxName" ' المعروض
                col.ValueMember = "TaxTypeID"        ' المخزن
                col.DisplayStyle =
                DataGridViewComboBoxDisplayStyle.DropDownButton

            End Using
        End Using

    End Sub

    Private Sub cboPartnerCode_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboPartnerCode.SelectedIndexChanged

        If IsFormLoading Then Exit Sub
        If cboPartnerCode.SelectedIndex = -1 Then
            txtPartnerName.Clear()
            txtPartnerPhone.Clear()
            Exit Sub
        End If

        Dim drv As DataRowView =
        TryCast(cboPartnerCode.SelectedItem, DataRowView)

        If drv Is Nothing Then Exit Sub

        txtPartnerName.Text = drv("PartnerName").ToString()
        txtPartnerPhone.Text = drv("Phone").ToString()

    End Sub
    Private Sub LoadSalesReps()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT EmpCode, EmpName
         FROM Security_Employee
         WHERE IsSalesRep = 1
           AND IsActive = 1
         ORDER BY EmpCode"

            Using da As New SqlClient.SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)

                cboSRepCode.DisplayMember = "EmpCode"
                cboSRepCode.ValueMember = "EmpCode"
                cboSRepCode.DataSource = dt   ' ✅ هذا هو المهم
                cboSRepCode.SelectedIndex = -1
            End Using
        End Using

    End Sub
    Private Sub cboSRepCode_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cboSRepCode.SelectedIndexChanged

        If cboSRepCode.SelectedValue Is Nothing Then Exit Sub
        If TypeOf cboSRepCode.SelectedValue Is DataRowView Then Exit Sub

        Dim drv As DataRowView =
        TryCast(cboSRepCode.SelectedItem, DataRowView)

        If drv Is Nothing Then Exit Sub

        txtSRepName.Text = drv("EmpName").ToString()

    End Sub
    Private Sub btnPatrnerFind_Click(
    sender As Object,
    e As EventArgs
) Handles btnPatrnerFind.Click

        Using f As New frmPartnerSearch
            f.ShowDialog()

            If f.SelectedPartnerID > 0 Then
                cboPartnerCode.SelectedValue = f.SelectedPartnerID
            End If
        End Using

    End Sub
    Private Sub btnFindSRep_Click(
    sender As Object,
    e As EventArgs
) Handles btnFindSRep.Click

        Using f As New frmEmployeeSearch
            f.ShowDialog()

            If Not String.IsNullOrEmpty(f.SelectedEmpCode) Then
                cboSRepCode.SelectedValue = f.SelectedEmpCode
            End If
        End Using

    End Sub
    Private Sub LoadBaseProductsForSR()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT ProductCode
         FROM Master_Product
         WHERE IsActive = 1
        And ProductCategoryID=2
         ORDER BY ProductCode"

            Using da As New SqlClient.SqlDataAdapter(sql, con)

                Dim dt As New DataTable
                da.Fill(dt)

                ' =========================
                ' التحقق من وجود العمود
                ' =========================
                If Not dgvSRInputs.Columns.Contains("colBaseProductCode") Then
                    MessageBox.Show(
                    "العمود colBaseProductCode غير موجود في dgvSRInputs",
                    "خطأ تصميم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Exit Sub
                End If

                ' =========================
                ' التحقق من نوع العمود
                ' =========================
                Dim col As DataGridViewComboBoxColumn =
                TryCast(
                    dgvSRInputs.Columns("colBaseProductCode"),
                    DataGridViewComboBoxColumn
                )

                If col Is Nothing Then
                    MessageBox.Show(
                    "العمود colBaseProductCode ليس ComboBoxColumn",
                    "خطأ تصميم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                    Exit Sub
                End If

                ' =========================
                ' الربط
                ' =========================
                col.DataSource = dt
                col.DisplayMember = "ProductCode"
                col.ValueMember = "ProductCode" ' 👈 نخزن الكود نفسه

                col.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                col.FlatStyle = FlatStyle.Standard



            End Using
        End Using

    End Sub
    Private Sub dgvSRInputs_EditingControlShowing(
    sender As Object,
    e As DataGridViewEditingControlShowingEventArgs
) Handles dgvSRInputs.EditingControlShowing

        If dgvSRInputs.CurrentCell Is Nothing Then Exit Sub

        ' فقط لعمود الصنف الأساسي
        If dgvSRInputs.Columns(dgvSRInputs.CurrentCell.ColumnIndex).Name <> "colBaseProductCode" Then
            Exit Sub
        End If
        If dgvSRInputs.Columns(dgvSRInputs.CurrentCell.ColumnIndex).Name <> "colDiscount" Then Exit Sub

        Dim tb = TryCast(e.Control, TextBox)
        If tb Is Nothing Then Exit Sub

        tb.Text = tb.Text.Replace("%", "").Trim()
        tb.SelectAll()

        Dim cb As ComboBox = TryCast(e.Control, ComboBox)
        If cb Is Nothing Then Exit Sub

        ' =========================
        ' تفعيل الكتابة + التوقع
        ' =========================
        cb.DropDownStyle = ComboBoxStyle.DropDown
        cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cb.AutoCompleteSource = AutoCompleteSource.ListItems
    End Sub

    Private Sub LoadSellUnits()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT UnitID, UnitName
         FROM Master_Unit
         WHERE IsActive = 1
         ORDER BY UnitName"

            Using da As New SqlClient.SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)

                Dim col As DataGridViewComboBoxColumn =
                CType(dgvSRInputs.Columns("colSellUnit"), DataGridViewComboBoxColumn)

                col.DataSource = dt
                col.DisplayMember = "UnitName"   ' 👈 المعروض للمستخدم
                col.ValueMember = "UnitID"           ' 👈 المخزن فعليًا
                col.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            End Using
        End Using

        ' ✅ تعيين القيمة الافتراضية = 7
        If dgvSRInputs.Rows.Count > 0 Then
            dgvSRInputs.Rows(0).Cells("colSellUnit").Value = 7
        End If

    End Sub
    Private Sub LoadProductTypes()

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Dim sql As String =
        "SELECT 
    ProductTypeID,
    TypeName
FROM Master_ProductType
WHERE IsActive = 1
ORDER BY TypeName
"

            Using da As New SqlClient.SqlDataAdapter(sql, con)
                Dim dt As New DataTable
                da.Fill(dt)

                Dim col As DataGridViewComboBoxColumn =
            CType(dgvSRInputs.Columns("colType"), DataGridViewComboBoxColumn)

                col.DataSource = dt
                col.DisplayMember = "TypeName"      ' 👈 المعروض
                col.ValueMember = "ProductTypeID"  ' 👈 المخزن
                col.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                If dgvSRInputs.Rows.Count > 0 Then
                    dgvSRInputs.Rows(0).Cells("colType").Value = 2
                End If


            End Using
        End Using

    End Sub
    Private Sub dgvSRInputs_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSRInputs.CellEndEdit

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = dgvSRInputs.Rows(e.RowIndex)
        Dim colName As String = dgvSRInputs.Columns(e.ColumnIndex).Name

        ' ===== تحويل السعر =====
        If colName = "colM3SellPrice" Then

            Dim l As Decimal = GetDec(row.Cells("colLength").Value)
            Dim w As Decimal = GetDec(row.Cells("colWidth").Value)
            Dim h As Decimal = GetDec(row.Cells("colHeight").Value)

            Dim pieceM3 As Decimal = PieceVolM3_FromCM(l, w, h)
            If pieceM3 > 0D Then
                Dim priceM3 As Decimal = GetDec(row.Cells("colM3SellPrice").Value)
                If priceM3 > 0D Then
                    row.Cells("colPieceSellPrice").Value =
                    Math.Round(priceM3 * pieceM3, 2)
                End If
            End If

        ElseIf colName = "colPieceSellPrice" Then

            Dim l As Decimal = GetDec(row.Cells("colLength").Value)
            Dim w As Decimal = GetDec(row.Cells("colWidth").Value)
            Dim h As Decimal = GetDec(row.Cells("colHeight").Value)

            Dim pieceM3 As Decimal = PieceVolM3_FromCM(l, w, h)
            If pieceM3 > 0D Then
                Dim piecePrice As Decimal = GetDec(row.Cells("colPieceSellPrice").Value)
                If piecePrice > 0D Then
                    row.Cells("colM3SellPrice").Value =
                    Math.Round(piecePrice / pieceM3, 2)
                End If
            End If

        End If

        ' ===== الحساب النهائي =====
        RecalcInputRow(row)
        IsSaved = False
        btnSaveSR.Enabled = True

    End Sub
    Private Function GetDec(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D

        Dim s As String = v.ToString().Trim()
        If s = "" Then Return 0D

        Dim d As Decimal
        If Decimal.TryParse(s, d) Then Return d

        Return 0D
    End Function
    Private Sub dgvSRInputs_CellContentClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSRInputs.CellContentClick
        ' ==========================================
        ' التحقق من الصلاحية حسب الـ Policy
        ' ==========================================
        If Not CurrentPolicy.AllowEditData Then
            MessageBox.Show("لا يمكن تعديل الطلب في هذه الحالة.", "تنبيه")
            Exit Sub
        End If

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim clickedCol As String = dgvSRInputs.Columns(e.ColumnIndex).Name
        Dim srcRow As DataGridViewRow = dgvSRInputs.Rows(e.RowIndex)

        ' =========================
        ' DELETE (في الجريد العلوي)
        ' =========================
        If clickedCol = "colDelete" Then

            If IsEditing Then
                IsEditing = False
                EditingOriginalValues = Nothing
                EditingOriginalProductCode = ""
                ResetInputToAddMode()
                Exit Sub
            End If

            ClearInputRow(srcRow)
            Exit Sub

        End If

        ' =========================
        ' فقط زر Add / Edit
        ' =========================
        If clickedCol <> "colAdd" Then Exit Sub

        Dim modeText As String = If(srcRow.Cells("colAdd").Value, "Add").ToString().Trim()
        If modeText = "" Then modeText = "Add"

        RecalcSaleRequestTotals()

        ' =========================
        ' تحقق أساسي
        ' =========================
        If srcRow.Cells("colBaseProductCode").Value Is Nothing Then
            MessageBox.Show("اختر الصنف الأساسي أولاً", "تنبيه")
            Exit Sub
        End If

        Dim l As Decimal = GetDec(srcRow.Cells("colLength").Value)
        Dim w As Decimal = GetDec(srcRow.Cells("colWidth").Value)
        Dim h As Decimal = GetDec(srcRow.Cells("colHeight").Value)

        If l <= 0 OrElse w <= 0 OrElse h <= 0 Then
            MessageBox.Show("أدخل الطول والعرض والارتفاع", "تنبيه")
            Exit Sub
        End If

        If Not IsInputRowValid(srcRow) Then
            MessageBox.Show("أكمل البيانات المطلوبة: الصنف/النوع/الكمية/سعر الحبة", "تنبيه")
            Exit Sub
        End If

        Dim baseCode As String = srcRow.Cells("colBaseProductCode").Value.ToString()
        Dim productTypeID As Integer = CInt(srcRow.Cells("colType").Value)

        Dim productCode As String =
    GenerateProductCode(baseCode, l, w, h)

        RecalcSaleRequestTotals()

        ' =========================
        ' منع التكرار
        ' =========================
        Dim excludeCode As String = ""
        If IsEditing Then excludeCode = EditingOriginalProductCode

        If DetailProductCodeExists(productCode, excludeCode) Then
            MessageBox.Show("هذا الصنف مضاف مسبقًا ولا يمكن تكراره", "تنبيه")
            Exit Sub
        End If

        ' =========================
        ' إنشاء صف تفاصيل
        ' =========================
        Dim idx As Integer = dgvSRDetails.Rows.Add()
        Dim detRow As DataGridViewRow = dgvSRDetails.Rows(idx)

        ' ✅ الضريبة من الجريد العلوي
        Dim taxID As Integer = 0
        If srcRow.Cells("colInputTaxID").Value IsNot Nothing Then
            taxID = CInt(srcRow.Cells("colInputTaxID").Value)
        End If
        detRow.Cells("colDetVAT").Value = taxID

        detRow.Cells("colDetProductCode").Value = productCode
        detRow.Cells("colDetBaseProductCode").Value = baseCode
        detRow.Cells("colDetType").Value = srcRow.Cells("colType").Value
        detRow.Cells("colDetSellUnit").Value = srcRow.Cells("colSellUnit").Value
        detRow.Cells("colDetLength").Value = srcRow.Cells("colLength").Value
        detRow.Cells("colDetWidth").Value = srcRow.Cells("colWidth").Value
        detRow.Cells("colDetHeight").Value = srcRow.Cells("colHeight").Value
        detRow.Cells("colDetQty").Value = srcRow.Cells("colQty").Value
        detRow.Cells("colDetPieceSellPrice").Value = srcRow.Cells("colPieceSellPrice").Value

        ' الخصم
        detRow.Cells("colDetDiscount").Value = srcRow.Cells("colDiscount").Value

        ' الملاحظة
        detRow.Cells("colDetNote").Value = srcRow.Cells("colNote").Value
        detRow.Cells("colDetProductID").Value = DBNull.Value
        ' =========================
        ' حساب كل القيم التابعة
        ' =========================
        RecalcDetailRow(detRow)

        ' =========================
        ' إنهاء وضع التعديل
        ' =========================
        If modeText.Equals("Edit", StringComparison.OrdinalIgnoreCase) Then
            IsEditing = False
            EditingOriginalValues = Nothing
            EditingOriginalProductCode = ""
        End If

        ResetInputToAddMode()
        RecalcSaleRequestTotals()
        IsSaved = False
        btnSaveSR.Enabled = True

    End Sub
    Private Sub dgvSRDetails_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSRDetails.CellClick
        If Not CurrentPolicy.AllowEditData Then
            MessageBox.Show("لا يمكن تعديل الطلب في هذه الحالة.", "تنبيه")
            Exit Sub
        End If

        ' =========================
        ' تحقق أساسي
        ' =========================
        If e.RowIndex < 0 Then Exit Sub

        ' =========================
        ' منع الدخول إذا هناك تعديل قائم
        ' =========================
        If IsEditing Then
            MessageBox.Show(
            "قم بإنهاء تعديل السطر الحالي أولاً",
            "تنبيه",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
            Exit Sub
        End If

        Dim detRow As DataGridViewRow = dgvSRDetails.Rows(e.RowIndex)
        If detRow Is Nothing OrElse detRow.IsNewRow Then Exit Sub

        ' =========================
        ' تحقق من وجود BaseProductCode
        ' =========================
        If detRow.Cells("colDetBaseProductCode").Value Is Nothing _
       OrElse detRow.Cells("colDetBaseProductCode").Value.ToString() = "" Then
            Exit Sub
        End If

        ' =========================
        ' حفظ الصف للتعديل
        ' =========================
        IsEditing = True
        EditingOriginalValues = CaptureDetailRowValues(detRow)
        EditingOriginalProductCode = detRow.Cells("colDetProductCode").Value.ToString()
        EditingDetailRowIndex = e.RowIndex

        ' =========================
        ' تعبئة صف الإدخال العلوي
        ' =========================
        If dgvSRInputs.Rows.Count = 0 Then dgvSRInputs.Rows.Add()
        Dim inputRow As DataGridViewRow = dgvSRInputs.Rows(0)

        inputRow.Cells("colBaseProductCode").Value = detRow.Cells("colDetBaseProductCode").Value
        inputRow.Cells("colType").Value = detRow.Cells("colDetType").Value
        inputRow.Cells("colSellUnit").Value = detRow.Cells("colDetSellUnit").Value
        inputRow.Cells("colLength").Value = detRow.Cells("colDetLength").Value
        inputRow.Cells("colWidth").Value = detRow.Cells("colDetWidth").Value
        inputRow.Cells("colHeight").Value = detRow.Cells("colDetHeight").Value
        inputRow.Cells("colQty").Value = detRow.Cells("colDetQty").Value
        inputRow.Cells("colPieceSellPrice").Value = detRow.Cells("colDetPieceSellPrice").Value
        inputRow.Cells("colDiscount").Value = detRow.Cells("colDetDiscount").Value
        inputRow.Cells("colNote").Value = detRow.Cells("colDetNote").Value

        ' الضريبة
        inputRow.Cells("colInputTaxID").Value = detRow.Cells("colDetVAT").Value

        RecalcInputRow(inputRow)

        inputRow.Cells("colAdd").Value = "Edit"

        ' =========================
        ' حذف الصف من التفاصيل
        ' =========================
        dgvSRDetails.Rows.RemoveAt(e.RowIndex)

    End Sub
    Private Sub ClearInputRow(row As DataGridViewRow)

        For Each col As DataGridViewColumn In dgvSRInputs.Columns
            If col.Name.StartsWith("col") _
           AndAlso col.Name <> "colAdd" _
           AndAlso col.Name <> "colInputTaxID" _
           AndAlso col.Name <> "colType" _
           AndAlso col.Name <> "colSellUnit" Then

                row.Cells(col.Name).Value = Nothing
            End If
        Next

        ' القيم الافتراضية
        row.Cells("colInputTaxID").Value = 1
        row.Cells("colType").Value = 2
        row.Cells("colSellUnit").Value = 12

    End Sub
    Private Sub dgvSRInputs_CellClick(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSRInputs.CellClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim col = dgvSRInputs.Columns(e.ColumnIndex)

        ' ✅ فقط عند النقر بالماوس
        If TypeOf col Is DataGridViewComboBoxColumn Then
            dgvSRInputs.CurrentCell = dgvSRInputs.Rows(e.RowIndex).Cells(e.ColumnIndex)
            dgvSRInputs.BeginEdit(True)

            Dim cb As ComboBox =
            TryCast(dgvSRInputs.EditingControl, ComboBox)

            If cb IsNot Nothing Then
                cb.DroppedDown = True
            End If
        End If

    End Sub

    Private Sub dgvSRDetails_CellEndEdit(
    sender As Object,
    e As DataGridViewCellEventArgs
) Handles dgvSRDetails.CellEndEdit

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim row As DataGridViewRow = dgvSRDetails.Rows(e.RowIndex)
        Dim colName As String = dgvSRDetails.Columns(e.ColumnIndex).Name

        ' الأعمدة التي لو تغيرت لازم نعيد حساب الصف
        Dim affectsCalc As Boolean =
        (colName = "colDetVAT") OrElse
        (colName = "colDetQty") OrElse
        (colName = "colDetPieceSellPrice") OrElse
        (colName = "colDetDiscount") OrElse
        (colName = "colDetLength") OrElse
        (colName = "colDetWidth") OrElse
        (colName = "colDetHeight")

        If Not affectsCalc Then Exit Sub

        RecalcDetailRow(row)
        RecalcSaleRequestTotals()
        IsSaved = False
        btnSaveSR.Enabled = True

    End Sub

    Private Sub AddNewInputRow()

        ' لو يوجد صف إدخال لا نعيد إنشائه
        If dgvSRInputs.Rows.Count = 0 Then
            dgvSRInputs.Rows.Add()
        End If

        ' وضع المؤشر على أول خلية
        dgvSRInputs.CurrentCell =
            dgvSRInputs.Rows(0).Cells(0)

    End Sub
    Private Function GenerateProductCode(
    baseProductCode As String,
       l As Decimal,
    w As Decimal,
    h As Decimal
) As String



        ' =========================
        ' توليد الكود الأساسي
        ' =========================
        Dim lStr As String = CInt(l).ToString("000")
        Dim wStr As String = CInt(w).ToString("000")
        Dim hStr As String = CInt(h).ToString("000")

        Dim baseCode As String =
$"{baseProductCode}-{lStr}{wStr}{hStr}"

        ' =========================
        ' لو غير موجود → نرجعه مباشرة
        ' =========================
        If Not ProductExistsInProductTable(baseCode) Then
            Return baseCode
        End If

        ' =========================
        ' لو موجود → نرجعه كما هو (بدون أي تعديل)
        ' =========================
        If ProductExistsInProductTable(baseCode) Then
            Return baseCode
        End If

        ' غير موجود → الفورم الآخر يتكفل بالترميز
        Return baseCode


    End Function

    Private Sub RecalcSaleRequestTotals()

        Dim totalVolume As Decimal = 0D
        Dim totalBFVAT As Decimal = 0D
        Dim totalVAT As Decimal = 0D
        Dim LineTotal As Decimal = 0D

        For Each r As DataGridViewRow In dgvSRDetails.Rows

            If r.IsNewRow Then Continue For

            totalVolume += GetDec(r.Cells("colDetTotalVolume").Value)
            totalBFVAT += GetDec(r.Cells("colDetTotalAmountBFVAT").Value)
            totalVAT += GetDec(r.Cells("colDetVATAmount").Value)
            LineTotal += GetDec(r.Cells("colDetTotalAmount").Value)

        Next

        txtTotalSRVolume.Text = Math.Round(totalVolume, 6).ToString("0.000") & ("    ") & " م³"
        txtTotalSRAmountBFVAT.Text = Math.Round(totalBFVAT, 2).ToString("0.00") & ("    ") & " ر.س"
        txtTotalVAT.Text = Math.Round(totalVAT, 2).ToString("0.00") & ("    ") & " ر.س"
        txtTotalSRAmount.Text = Math.Round(LineTotal, 2).ToString("0.00") & ("    ") & " ر.س"

    End Sub
    Private Sub ResetToInitialState()
        IsLoading = True

        ' ========= Context =========
        CurrentSRID = 0

        ' ========= Header =========
        cboPartnerCode.SelectedIndex = -1
        txtPartnerName.Clear()
        txtPartnerPhone.Clear()
        txtPartnerDept.Clear()

        cboSRepCode.SelectedIndex = -1
        txtSRepName.Clear()

        cboStoreCode.SelectedIndex = -1

        dtpSRDate.Value = Date.Today
        dtpSRDeliveryDate.Value = Date.Today

        txtSRNote.Clear()
        txtSRCode.Clear()
        chkIsActive.Checked = True

        ' ========= Grids =========
        dgvSRInputs.Rows.Clear()
        dgvSRDetails.Rows.Clear()

        ' ========= Totals =========
        txtTotalSRVolume.Text = "0"
        txtTotalSRAmount.Text = "0"
        txtTotalSRAmountBFVAT.Text = "0"
        txtTotalVAT.Text = "0"

        ' ==================================================
        ' Initial Status + Policy (مصدر الحقيقة الوحيد)
        ' ==================================================
        CurrentStatusID =
        Workflow_OperationPolicyHelper.GetInitialStatusByScope(FORM_SCOPE)

        CurrentPolicy =
        Workflow_OperationPolicyHelper.GetEditPolicy(
            FORM_OPERATION_TYPE_ID,
            CurrentStatusID
        )

        txtSRStatus.Text =
        Workflow_OperationPolicyHelper.GetStatusName(CurrentStatusID)

        IsLoading = False
        btnSaveSR.Text = "حفظ"

    End Sub

    Private Sub btnNewSR_Click(sender As Object, e As EventArgs) Handles btnNewSR.Click

        If HasUnsavedData() Then
            Dim res = MessageBox.Show(
            "يوجد بيانات غير محفوظة، هل تريد حفظ الطلب قبل إنشاء طلب جديد؟",
            "تأكيد",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question
        )

            Select Case res
                Case DialogResult.Yes
                    SaveSaleRequest()
                    If HasUnsavedData() Then Exit Sub ' لو فشل الحفظ

                Case DialogResult.Cancel
                    Exit Sub

                Case DialogResult.No
                    ' كمل بدون حفظ
            End Select
        End If

        ResetToInitialState()
        ' ==================================================
        ' 7) صف الإدخال الافتراضي
        ' ==================================================
        dgvSRInputs.Rows.Clear()
        dgvSRInputs.Rows.Add()

        Dim inputRow As DataGridViewRow = dgvSRInputs.Rows(0)
        inputRow.Cells("colType").Value = 2
        inputRow.Cells("colSellUnit").Value = 12
        inputRow.Cells("colInputTaxID").Value = 1

        ' ==================================================
        ' 8) قيم الهيدر الافتراضية
        ' ==================================================
        txtSRCode.Text = GenerateSRCode()
        dtpSRDate.Value = Date.Today
        dtpSRDeliveryDate.Value = Date.Today
        chkIsActive.Checked = True

        cboPartnerCode.SelectedIndex = -1
        txtPartnerName.Clear()
        txtPartnerPhone.Clear()

        cboSRepCode.SelectedIndex = -1
        txtSRepName.Clear()

        If cboStoreCode.Items.Count > 0 Then
            cboStoreCode.SelectedValue = 4
        End If

    End Sub

    Private Sub CreateNewSaleRequest()


        ' =========================
        ' تنظيف الكاش
        ' =========================
        ProductTypeCodeCache.Clear()
        TaxRateCache.Clear()

        ' =========================
        ' حالة الحفظ
        ' =========================
        IsSaved = False
        btnSaveSR.Enabled = True
        btnSRDelete.Enabled = True

        ' =========================
        ' فصل الجريد
        ' =========================
        dgvSRDetails.EndEdit()
        dgvSRDetails.CurrentCell = Nothing

        ' =========================
        ' مسح البيانات
        ' =========================
        txtSRCode.Text = GenerateSRCode()

        dtpSRDate.Value = Date.Today
        dtpSRDeliveryDate.Value = Date.Today
        txtSRNote.Clear()

        txtTotalSRVolume.Clear()
        txtTotalSRAmountBFVAT.Clear()
        txtTotalVAT.Clear()
        txtTotalSRAmount.Clear()

        dgvSRDetails.Rows.Clear()
        dgvSRInputs.Rows.Clear()
        dgvSRInputs.Rows.Add()

        cboStoreCode.Enabled = False

        CreateNewSaleRequest()
    End Sub

    Private Sub btnCloseSR_Click(
    sender As Object,
    e As EventArgs
) Handles btnCloseSR.Click

        ' =========================
        ' لا يوجد بيانات → اغلق مباشرة
        ' =========================
        If Not HasUnsavedData() Then
            Me.Close()
            Exit Sub
        End If
        If IsSaved Then
            Me.Close()
            Exit Sub
        End If

        ' =========================
        ' تحذير حفظ
        ' =========================
        Dim res = MessageBox.Show(
        "يوجد بيانات غير محفوظة، هل تريد حفظ الطلب قبل الإغلاق؟",
        "تأكيد",
        MessageBoxButtons.YesNoCancel,
        MessageBoxIcon.Question
    )

        Select Case res

            Case DialogResult.Yes
                ' 🔴 مكان الحفظ (عند جاهزية كود الحفظ)
                ' SaveSaleRequest()
                MessageBox.Show("الحفظ غير مفعّل بعد", "تنبيه")
                ' لا نغلق
                IsSaved = True
                btnSaveSR.Enabled = False

            Case DialogResult.No
                Me.Close()

            Case DialogResult.Cancel
                Exit Sub

        End Select

    End Sub
    Private Function HasUnsavedData() As Boolean

        ' لو محفوظ → لا يوجد بيانات غير محفوظة
        If IsSaved Then Return False

        ' وجود تفاصيل
        For Each r As DataGridViewRow In dgvSRDetails.Rows
            If Not r.IsNewRow Then Return True
        Next

        ' بيانات رأس حقيقية
        If cboPartnerCode.SelectedValue IsNot Nothing Then Return True
        If cboSRepCode.SelectedValue IsNot Nothing Then Return True
        If cboStoreCode.SelectedValue IsNot Nothing Then Return True
        If txtSRNote.Text.Trim() <> "" Then Return True

        Return False

    End Function

    Private Function CalcWeightedDiscountPercent() As Decimal

        Dim totalBFDisc As Decimal = 0D
        Dim totalDiscAmount As Decimal = 0D

        For Each r As DataGridViewRow In dgvSRDetails.Rows
            If r.IsNewRow Then Continue For

            Dim Quantity As Decimal = GetDec(r.Cells("colDetQty").Value)
            Dim price As Decimal = GetDec(r.Cells("colDetPieceSellPrice").Value)
            Dim discPct As Decimal = GetDec(r.Cells("colDetDiscount").Value)

            Dim lineTotal As Decimal = Quantity * price
            Dim lineDisc As Decimal = lineTotal * (discPct / 100D)

            totalBFDisc += lineTotal
            totalDiscAmount += lineDisc
        Next

        If totalBFDisc = 0D Then Return 0D

        Return Math.Round((totalDiscAmount / totalBFDisc) * 100D, 5)

    End Function

    ' =========================================================
    ' هل الكود موجود في جدول المنتجات؟
    ' =========================================================
    Private Function ProductExistsInProductTable(productCode As String) As Boolean

        If String.IsNullOrWhiteSpace(productCode) Then Return False

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlClient.SqlCommand(
            "SELECT TOP 1 1 FROM Master_Product WHERE ProductCode = @Code", con)

                cmd.Parameters.AddWithValue("@Code", productCode.Trim())

                Dim obj = cmd.ExecuteScalar()
                Return (obj IsNot Nothing AndAlso Not IsDBNull(obj))
            End Using
        End Using

    End Function


    ' =========================================================
    ' قبل الحفظ: تأكد أن كل منتجات التفاصيل موجودة
    ' - لو منتج ناقص: افتح فورم الترميز (سننفذه بالخطوة 2)
    ' - إذا المستخدم أغلق/ألغى: نرجع False ونوقف الحفظ
    ' =========================================================

    ' =========================================================
    ' (مؤقت) سيتم تنفيذه في الخطوة 2
    ' =========================================================





    Private Sub SaveSaleRequest()

        Using con As New SqlConnection(ConnStr)
            con.Open()
            If dgvSRDetails.Rows.Count = 0 Then Exit Sub

            ' =========================
            ' 1) حساب المجاميع (كما كان)
            ' =========================
            Dim totalUnitPrice As Decimal = 0D
            Dim totalPriceM3 As Decimal = 0D

            For Each r As DataGridViewRow In dgvSRDetails.Rows
                If r.IsNewRow Then Continue For
                totalUnitPrice += GetDec(r.Cells("colDetPieceSellPrice").Value)
                totalPriceM3 += GetDec(r.Cells("colDetM3SellPrice").Value)
            Next

            ' =========================
            ' 2) تجهيز تفاصيل الطلب (TVP)
            '    نفس بيانات INSERT القديمة
            ' =========================
            Dim dtDetails As New DataTable

            dtDetails.Columns.Add("ProductID", GetType(Integer))
            dtDetails.Columns.Add("ProductCode", GetType(String))
            dtDetails.Columns.Add("Quantity", GetType(Decimal))
            dtDetails.Columns.Add("LengthCM", GetType(Decimal))
            dtDetails.Columns.Add("WidthCM", GetType(Decimal))
            dtDetails.Columns.Add("HeightCM", GetType(Decimal))
            dtDetails.Columns.Add("UnitPrice", GetType(Decimal))
            dtDetails.Columns.Add("PricePerM3", GetType(Decimal))
            dtDetails.Columns.Add("DiscountRate", GetType(Decimal))
            dtDetails.Columns.Add("Notes", GetType(String))
            dtDetails.Columns.Add("ProductTypeID", GetType(Integer)) ' الايدي
            dtDetails.Columns.Add("SellUnitID", GetType(Integer))
            dtDetails.Columns.Add("TaxTypeID", GetType(Integer))
            dtDetails.Columns.Add("BaseProductCode", GetType(String))

            For Each r As DataGridViewRow In dgvSRDetails.Rows
                If r.IsNewRow Then Continue For

                dtDetails.Rows.Add(
                If(IsDBNull(r.Cells("colDetProductID").Value), DBNull.Value, r.Cells("colDetProductID").Value),
                r.Cells("colDetProductCode").Value.ToString(),
                GetDec(r.Cells("colDetQty").Value),
                GetDec(r.Cells("colDetLength").Value),
                GetDec(r.Cells("colDetWidth").Value),
                GetDec(r.Cells("colDetHeight").Value),
                GetDec(r.Cells("colDetPieceSellPrice").Value),
                GetDec(r.Cells("colDetM3SellPrice").Value),
                GetDec(r.Cells("colDetDiscount").Value),
                If(r.Cells("colDetNote").Value Is Nothing, DBNull.Value, r.Cells("colDetNote").Value.ToString()),
CInt(r.Cells("colDetType").Value),                     ' ProductTypeID (رقم)
                If(r.Cells("colDetSellUnit").Value Is Nothing, DBNull.Value, r.Cells("colDetSellUnit").Value),
                If(r.Cells("colDetVAT").Value Is Nothing, DBNull.Value, r.Cells("colDetVAT").Value),
                If(r.Cells("colDetBaseProductCode").Value Is Nothing, DBNull.Value, r.Cells("colDetBaseProductCode").Value)
            )
                Dim msg As String = "سيتم حفظ السطر بالقيم التالية:" & vbCrLf & vbCrLf &
                    "ProductCode : " & r.Cells("colDetProductCode").Value.ToString() & vbCrLf &
                    "ProductType : " & GetProductTypeCode(CInt(r.Cells("colDetType").Value)) & vbCrLf &
                    "ProductType : " & r.Cells("colDetType").Value.ToString() & vbCrLf &
                    "ProductID : " & If(
                        IsDBNull(r.Cells("colDetProductID").Value) OrElse r.Cells("colDetProductID").Value Is Nothing,
                        "NULL",
                        r.Cells("colDetProductID").Value.ToString()
                    )

            Next

            ' =========================
            ' 3) استدعاء الإجراء (بدل INSERT)
            ' =========================
            Using cmd As New SqlCommand("sal.SaveRequest", con)
                cmd.CommandType = CommandType.StoredProcedure

                Dim pSRID As New SqlParameter("@SRID", SqlDbType.Int)
                pSRID.Direction = ParameterDirection.InputOutput
                pSRID.Value = CurrentSRID
                cmd.Parameters.Add(pSRID)

                cmd.Parameters.AddWithValue("@SRCode", txtSRCode.Text)
                cmd.Parameters.AddWithValue("@SRDate", dtpSRDate.Value.Date)
                cmd.Parameters.AddWithValue("@DeliveryDate", dtpSRDeliveryDate.Value.Date)
                cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked)
                cmd.Parameters.AddWithValue("@PartnerID", CInt(cboPartnerCode.SelectedValue))
                cmd.Parameters.AddWithValue("@SalesRepCode", cboSRepCode.SelectedValue.ToString())
                cmd.Parameters.AddWithValue("@StoreID", CInt(cboStoreCode.SelectedValue))
                cmd.Parameters.AddWithValue("@UnitPrice", totalUnitPrice)
                cmd.Parameters.AddWithValue("@PricePerM3", totalPriceM3)
                cmd.Parameters.AddWithValue("@DiscountRate", CalcWeightedDiscountPercent())
                cmd.Parameters.AddWithValue("@Notes", txtSRNote.Text)
                cmd.Parameters.AddWithValue("@CreatedBy", 1)

                Dim pDetails As New SqlParameter("@Details", SqlDbType.Structured)
                pDetails.TypeName = "sal.SRDetails_TVP"
                pDetails.Value = dtDetails
                cmd.Parameters.Add(pDetails)

                cmd.ExecuteNonQuery()

                CurrentSRID = CInt(pSRID.Value)
            End Using

        End Using

    End Sub
    Public Sub LoadSaleRequest(srID As Integer)

        CurrentSRID = srID

        ProductTypeCodeCache.Clear()
        TaxRateCache.Clear()
        IsSaved = True

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            ' =====================================
            ' 1) تحميل الهيدر من Business_SR
            ' =====================================
            Using cmd As New SqlCommand("
SELECT
    SRCode,
    SRDate,
    DeliveryDate,
    IsActive,
    PartnerID,
    SalesRepCode,
    StoreID,
    UnitPrice,
    PricePerM3,
    DiscountRate,
    Notes
FROM dbo.Business_SR
WHERE SRID = @SRID
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then

                        txtSRCode.Text = rd("SRCode").ToString()
                        dtpSRDate.Value = CDate(rd("SRDate"))

                        If Not IsDBNull(rd("DeliveryDate")) Then
                            dtpSRDeliveryDate.Value = CDate(rd("DeliveryDate"))
                        End If

                        chkIsActive.Checked = CBool(rd("IsActive"))
                        cboPartnerCode.SelectedValue = rd("PartnerID")
                        cboSRepCode.SelectedValue = rd("SalesRepCode")
                        cboStoreCode.SelectedValue = rd("StoreID")

                        txtSRNote.Text = rd("Notes").ToString()

                    End If
                End Using
            End Using


            ' =====================================
            ' 2) تحميل الحالة من Business_SRD
            ' =====================================
            Using cmd As New SqlCommand("
SELECT TOP 1 BusinessStatusID
FROM dbo.Business_SRD
WHERE SRID = @SRID
ORDER BY CreatedAt DESC
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Dim statusObj = cmd.ExecuteScalar()

                If statusObj IsNot Nothing AndAlso Not IsDBNull(statusObj) Then
                    CurrentBusinessStatusID = CInt(statusObj)
                Else
                    CurrentBusinessStatusID = 0
                End If

                txtSRStatus.Text = GetStatusNameByID(CurrentBusinessStatusID)

                CurrentPolicy =
                Workflow_OperationPolicyHelper.GetEditPolicy(
                    FORM_OPERATION_TYPE_ID,
                    CurrentBusinessStatusID
                )

                RefreshUI()

            End Using


            ' =====================================
            ' 3) تحميل التفاصيل من Business_SRD
            ' =====================================
            dgvSRDetails.Rows.Clear()

            Using cmd As New SqlCommand("
SELECT
    ProductID,
    ProductCode,
    Quantity,
    LengthCM,
    WidthCM,
    HeightCM,
    UnitPrice,
    PricePerM3,
    DiscountRate,
    ProductTypeID,
    Notes,
    SellUnitID,
    BaseProductCode,
    TaxTypeID
FROM dbo.Business_SRD
WHERE SRID = @SRID
", con)

                cmd.Parameters.AddWithValue("@SRID", srID)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()

                        Dim idx As Integer = dgvSRDetails.Rows.Add()
                        Dim r As DataGridViewRow = dgvSRDetails.Rows(idx)

                        r.Cells("colDetProductID").Value = rd("ProductID")
                        r.Cells("colDetProductCode").Value = rd("ProductCode")
                        r.Cells("colDetQty").Value = rd("Quantity")
                        r.Cells("colDetLength").Value = rd("LengthCM")
                        r.Cells("colDetWidth").Value = rd("WidthCM")
                        r.Cells("colDetHeight").Value = rd("HeightCM")
                        r.Cells("colDetPieceSellPrice").Value = rd("UnitPrice")
                        r.Cells("colDetM3SellPrice").Value = rd("PricePerM3")
                        r.Cells("colDetDiscount").Value = rd("DiscountRate")
                        r.Cells("colDetType").Value = rd("ProductTypeID")
                        r.Cells("colDetNote").Value = rd("Notes")
                        r.Cells("colDetSellUnit").Value = rd("SellUnitID")
                        r.Cells("colDetBaseProductCode").Value = rd("BaseProductCode")
                        r.Cells("colDetVAT").Value = rd("TaxTypeID")

                        RecalcDetailRow(r)

                    End While
                End Using
            End Using

        End Using


        ' =====================================
        ' 4) إعادة الحسابات
        ' =====================================
        RecalcSaleRequestTotals()

        IsSaved = False
        btnSaveSR.Enabled = True
        btnSaveSR.Text = "تعديل"

    End Sub



    Private Function GetBaseProductCodeFromProduct(
    productCode As String
) As String

        ' =========================
        ' إرجاع كود الصنف الأساسي
        ' اعتمادًا على جدول Product
        ' =========================
        If String.IsNullOrWhiteSpace(productCode) Then
            Return ""
        End If

        Using con As New SqlClient.SqlConnection(ConnStr)
            con.Open()

            Using cmd As New SqlClient.SqlCommand(
            "SELECT BaseProductID 
             FROM Master_Product 
             WHERE ProductCode = @Code", con)

                cmd.Parameters.AddWithValue("@Code", productCode)

                Dim obj = cmd.ExecuteScalar()

                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Return ""
                End If

                ' =========================
                ' حسب تصميمك:
                ' BaseProductID يخزن كود الصنف الأساسي
                ' =========================
                Return obj.ToString()

            End Using
        End Using

    End Function

    Private Sub btnSRSearch_Click(
    sender As Object,
    e As EventArgs
) Handles btnSRSearch.Click

        Using f As New frmSRSearch

            ' فتح شاشة البحث
            If f.ShowDialog() <> DialogResult.OK Then Exit Sub

            ' التأكد من اختيار طلب
            If f.SelectedSRID <= 0 Then Exit Sub

            ' تحميل الطلب في نفس الفورم
            LoadSaleRequest(f.SelectedSRID)

        End Using

    End Sub
    Private Sub dgvSRInputs_CellValidating(
    sender As Object,
    e As DataGridViewCellValidatingEventArgs
) Handles dgvSRInputs.CellValidating

        If dgvSRInputs.Columns(e.ColumnIndex).Name <> "colInputTaxID" Then Exit Sub

        Dim cb As ComboBox = TryCast(dgvSRInputs.EditingControl, ComboBox)
        If cb Is Nothing Then Exit Sub

        Dim typedText As String = cb.Text.Trim()
        If typedText = "" Then Exit Sub

        ' حاول تثبيت القيمة المكتوبة
        For i As Integer = 0 To cb.Items.Count - 1
            Dim drv As DataRowView = TryCast(cb.Items(i), DataRowView)
            If drv Is Nothing Then Continue For

            ' يطابق ID أو الاسم
            If drv("TaxTypeID").ToString() = typedText _
           OrElse drv("TaxName").ToString().Equals(typedText, StringComparison.OrdinalIgnoreCase) Then

                cb.SelectedIndex = i
                Exit Sub
            End If
        Next

        ' لو لم يتم التطابق → رجّع الافتراضي 15%
        cb.SelectedValue = 1

    End Sub
    Private Function GetProductTypesTable() As DataTable
        Using con As New SqlClient.SqlConnection(ConnStr)
            Using da As New SqlClient.SqlDataAdapter(
            "SELECT ProductTypeID, TypeName FROM Master_ProductType WHERE IsActive=1", con)
                Dim dt As New DataTable
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    Private Function GetUnitsTable() As DataTable
        Using con As New SqlClient.SqlConnection(ConnStr)
            Using da As New SqlClient.SqlDataAdapter(
            "SELECT UnitID, UnitName FROM Master_Unit WHERE IsActive=1", con)
                Dim dt As New DataTable
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function
    Private Sub dgvSRInputs_KeyDown(
    sender As Object,
    e As KeyEventArgs
) Handles dgvSRInputs.KeyDown

        If e.KeyCode <> Keys.Enter Then Exit Sub
        If dgvSRInputs.CurrentCell Is Nothing Then Exit Sub

        Dim colName As String =
        dgvSRInputs.Columns(dgvSRInputs.CurrentCell.ColumnIndex).Name

        ' فقط عمود Add
        If colName <> "colAdd" Then Exit Sub

        ' منع السلوك الافتراضي (الانتقال لسطر جديد)
        e.Handled = True
        e.SuppressKeyPress = True

        ' تنفيذ نفس كود الضغط بالماوس
        dgvSRInputs_CellContentClick(
        dgvSRInputs,
        New DataGridViewCellEventArgs(
            dgvSRInputs.CurrentCell.ColumnIndex,
            dgvSRInputs.CurrentCell.RowIndex
        )
    )

    End Sub
    Private Sub dgvSRInputs_CellFormatting(
    sender As Object,
    e As DataGridViewCellFormattingEventArgs
) Handles dgvSRInputs.CellFormatting

        If dgvSRInputs.Columns(e.ColumnIndex).Name <> "colDiscount" Then Exit Sub
        If e.Value Is Nothing OrElse IsDBNull(e.Value) Then Exit Sub

        Dim d As Decimal
        If Decimal.TryParse(e.Value.ToString(), d) Then
            e.Value = d.ToString("0.#") & "%"
            e.FormattingApplied = True
        End If

    End Sub

    Private Sub dgvSRInputs_CellParsing(
       sender As Object,
       e As DataGridViewCellParsingEventArgs
   ) Handles dgvSRInputs.CellParsing

        ' فقط عمود الخصم في الجريد العلوي
        If dgvSRInputs.Columns(e.ColumnIndex).Name = "colDiscount" Then

            If e.Value IsNot Nothing Then
                Dim s As String = e.Value.ToString().Replace("%", "").Trim()

                Dim d As Decimal
                If Decimal.TryParse(s, d) Then
                    e.Value = d          ' 👈 نخزن الرقم فقط (مثلاً 15)
                    e.ParsingApplied = True
                End If
            End If

        End If

    End Sub
    Private Sub CancelCurrentEditIfAny()

        If Not IsEditing OrElse EditingOriginalValues Is Nothing Then Exit Sub

        Dim idx As Integer = dgvSRDetails.Rows.Add()
        Dim r As DataGridViewRow = dgvSRDetails.Rows(idx)

        For Each kv In EditingOriginalValues
            r.Cells(kv.Key).Value = kv.Value
        Next

        RecalcDetailRow(r)

        IsEditing = False
        EditingOriginalValues = Nothing
        EditingOriginalProductCode = ""
        EditingDetailRowIndex = -1

    End Sub


    Private Sub btnSRDelete_Click(sender As Object, e As EventArgs) Handles btnSRDelete.Click

        If CurrentSRID = 0 Then
            MessageBox.Show("لا يوجد طلب محدد للحذف")
            Exit Sub
        End If



        If MessageBox.Show(
        "هل أنت متأكد من حذف الطلب؟",
        "تأكيد الحذف",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    ) = DialogResult.No Then
            Exit Sub
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "UPDATE Business_SR
             SET IsActive = 0
             WHERE SRID = @ID", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CurrentSRID
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        MessageBox.Show("تم حذف الطلب بنجاح")
        CreateNewSaleRequest()

    End Sub
    Private Function IsSRNewStatus(statusID As Integer) As Boolean

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT StatusCode
             FROM Workflow_Status
             WHERE StatusID = @ID", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = statusID
                con.Open()

                Dim codeObj = cmd.ExecuteScalar()
                If codeObj Is Nothing OrElse IsDBNull(codeObj) Then
                    Return False
                End If

                Return codeObj.ToString() = "SR_NEW"
            End Using
        End Using

    End Function


    Private Sub InsertSaleRequest()

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
INSERT INTO Business_SR
(
    SRCode,
    SRDate,
    DeliveryDate,
    IsActive,
    PartnerID,
    StoreID,
    SalesRepCode,
    Notes,
    BusinessStatusID
)
VALUES
(
    @SRCode,
    @SRDate,
    @DeliveryDate,
    @IsActive,
    @PartnerID,
    @StoreID,
    @SalesRepCode,
    @Notes,
    @BusinessStatusID
);
SELECT SCOPE_IDENTITY();
", con)

                cmd.Parameters.Add("@SRCode", SqlDbType.NVarChar, 50).Value = txtSRCode.Text.Trim()
                cmd.Parameters.Add("@SRDate", SqlDbType.Date).Value = dtpSRDate.Value.Date
                cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = dtpSRDeliveryDate.Value.Date
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked

                cmd.Parameters.Add("@PartnerID", SqlDbType.Int).Value = CInt(cboPartnerCode.SelectedValue)
                cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = 4
                cmd.Parameters.Add("@SalesRepCode", SqlDbType.NVarChar, 50).Value = cboSRepCode.Text

                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value =
                If(txtSRNote.Text.Trim() = "", DBNull.Value, txtSRNote.Text.Trim())

                ' =========================
                ' Statuses
                ' =========================
                cmd.Parameters.Add("@BusinessStatusID", SqlDbType.Int).Value = CurrentBusinessStatusID

                con.Open()

                Dim newID = cmd.ExecuteScalar()
                If newID Is Nothing OrElse IsDBNull(newID) Then
                    Throw New Exception("فشل حفظ طلب المبيعات")
                End If

                CurrentSRID = CInt(newID)

            End Using
        End Using

    End Sub
    Private Function GetStatusNameByID(statusID As Integer) As String

        If statusID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT StatusName
             FROM Workflow_Status
             WHERE StatusID = @ID
               AND IsActive = 1", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = statusID
                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Return ""
                End If

                Return obj.ToString()
            End Using
        End Using

    End Function
    Private Function GetStatusCodeByID(statusID As Integer) As String

        If statusID <= 0 Then Return ""

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT StatusCode
             FROM Workflow_Status
             WHERE StatusID = @ID", con)

                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = statusID
                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Return ""
                End If

                Return obj.ToString()
            End Using
        End Using

    End Function
    Private Function GetProductIDByCode(productCode As String) As Integer

        If String.IsNullOrWhiteSpace(productCode) Then Return 0

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
            "SELECT ProductID FROM Master_Product WHERE ProductCode = @Code", con)

                cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = productCode
                con.Open()

                Dim obj = cmd.ExecuteScalar()
                If obj Is Nothing OrElse IsDBNull(obj) Then
                    Return 0
                End If

                Return CInt(obj)
            End Using
        End Using

    End Function


    Private Sub btnSaveSR_Click(
    sender As Object,
    e As EventArgs
) Handles btnSaveSR.Click
        Dim errMsg As String = ""
        If Not CurrentPolicy.AllowEditData Then
            MessageBox.Show("لا يمكن تعديل الطلب في هذه الحالة.", "تنبيه")
            Exit Sub
        End If

        If Not ValidateSaleRequestBeforeSave(errMsg) Then
            MessageBox.Show(errMsg, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' =========================
        ' تأكد من وجود المنتجات (كما عندك)
        ' =========================
        If Not EnsureAllDetailProductsExistBeforeSave() Then Exit Sub

        ' =========================
        ' تحقق أساسي قبل الحفظ
        ' =========================
        If cboPartnerCode.SelectedValue Is Nothing Then Exit Sub
        If cboSRepCode.SelectedValue Is Nothing Then Exit Sub
        If cboStoreCode.SelectedValue Is Nothing Then Exit Sub

        ' لا يوجد تفاصيل
        If dgvSRDetails.Rows.Count = 0 Then Exit Sub

        Dim hasDetail As Boolean = False
        For Each r As DataGridViewRow In dgvSRDetails.Rows
            If Not r.IsNewRow Then
                hasDetail = True
                Exit For
            End If
        Next

        If Not hasDetail Then Exit Sub


        ' =========================
        ' الحفظ
        ' =========================
        ' =========================
        ' الحفظ
        ' =========================
        SaveSaleRequest()

        IsSaved = True
        btnSaveSR.Enabled = False
        btnSaveSR.Text = "حفظ"

        MessageBox.Show(
    "تم حفظ طلب المبيعات بنجاح",
    "نجاح",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
)

    End Sub
    Private Function EnsureAllDetailProductsExistBeforeSave() As Boolean

        For Each r As DataGridViewRow In dgvSRDetails.Rows

            If r.IsNewRow Then Continue For

            ' =========================
            ' قراءة القيم من الجريد
            ' =========================
            Dim productCode As String =
            If(r.Cells("colDetProductCode").Value, "").ToString().Trim()

            If productCode = "" Then Continue For

            Dim productTypeID As Integer = 0
            If r.Cells("colDetType").Value IsNot Nothing AndAlso
           Not IsDBNull(r.Cells("colDetType").Value) Then
                productTypeID = CInt(r.Cells("colDetType").Value)
            End If

            If productTypeID <= 0 Then
                MessageBox.Show("نوع المنتج غير صالح.")
                Return False
            End If

            Dim foundProductID As Integer = 0

            ' =========================
            ' البحث بالكود + النوع
            ' =========================
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT ProductID
                FROM Master_Product
                WHERE ProductCode = @Code
                  AND ProductTypeID = @TypeID
            ", con)

                    cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = productCode
                    cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = productTypeID

                    con.Open()

                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        foundProductID = CInt(obj)
                    End If
                End Using
            End Using

            ' =========================
            ' إذا وجد → اربطه
            ' =========================
            If foundProductID > 0 Then
                r.Cells("colDetProductID").Value = foundProductID
                Continue For
            End If

            ' =========================
            ' إذا لم يوجد → افتح الترميز
            ' =========================
            Dim baseCode As String =
            If(r.Cells("colDetBaseProductCode").Value, "").ToString().Trim()

            Dim l As Decimal = GetDec(r.Cells("colDetLength").Value)
            Dim w As Decimal = GetDec(r.Cells("colDetWidth").Value)
            Dim h As Decimal = GetDec(r.Cells("colDetHeight").Value)

            Dim sellUnitID As Integer = 0
            If r.Cells("colDetSellUnit").Value IsNot Nothing AndAlso
           Not IsDBNull(r.Cells("colDetSellUnit").Value) Then
                sellUnitID = CInt(r.Cells("colDetSellUnit").Value)
            End If

            Dim ok As Boolean =
            OpenCodingFormForMissingProduct_FromSR(
                productCode,
                baseCode,
                productTypeID,
                sellUnitID,
                l, w, h
            )

            If Not ok Then
                ' المستخدم ألغى الترميز
                Return False
            End If

            ' =========================
            ' إعادة البحث بعد الترميز
            ' =========================
            Using con As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("
                SELECT ProductID
                FROM Master_Product
                WHERE ProductCode = @Code
                  AND ProductTypeID = @TypeID
            ", con)

                    cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = productCode
                    cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = productTypeID

                    con.Open()

                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        foundProductID = CInt(obj)
                    End If
                End Using
            End Using

            If foundProductID <= 0 Then
                MessageBox.Show("فشل إنشاء الصنف.")
                Return False
            End If

            r.Cells("colDetProductID").Value = foundProductID

        Next

        Return True

    End Function



    Private Function OpenCodingFormForMissingProduct_FromSR(
    productCode As String,
    baseProductCode As String,
    productTypeID As Integer,
    sellUnitID As Integer,
    l As Decimal, w As Decimal, h As Decimal
) As Boolean

        Using f As New frmProduct

            ' تمرير البيانات
            f.PrefillFromSaleRequest(
            productCode,
            baseProductCode,
            productTypeID,
            sellUnitID,
            l, w, h
        )

            ' فتح الفورم
            If f.ShowDialog() <> DialogResult.OK Then
                Return False   ' المستخدم ألغى
            End If

            ' تحقق أن الصنف تم حفظه
            If f.SavedProductID <= 0 Then
                Return False
            End If

        End Using

        Return True

    End Function

End Class