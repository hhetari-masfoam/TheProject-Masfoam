<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProduct
    Inherits AABaseOperationForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtProductCode = New System.Windows.Forms.TextBox()
        Me.lblProductCode = New System.Windows.Forms.Label()
        Me.lblBarCode = New System.Windows.Forms.Label()
        Me.txtBarcode = New System.Windows.Forms.TextBox()
        Me.lblProductName = New System.Windows.Forms.Label()
        Me.txtProductName = New System.Windows.Forms.TextBox()
        Me.lblProductEngName = New System.Windows.Forms.Label()
        Me.txtProductEnglishName = New System.Windows.Forms.TextBox()
        Me.lblCategory = New System.Windows.Forms.Label()
        Me.lblSubCategory = New System.Windows.Forms.Label()
        Me.lblProductGroup = New System.Windows.Forms.Label()
        Me.lblStandardQTY = New System.Windows.Forms.Label()
        Me.chkProductStatus = New System.Windows.Forms.CheckBox()
        Me.btnNew = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btndelete = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.cboCategory = New System.Windows.Forms.ComboBox()
        Me.cboProductGroup = New System.Windows.Forms.ComboBox()
        Me.cboSubCategory = New System.Windows.Forms.ComboBox()
        Me.txtMandatory = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblTaxType = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.cboTaxType = New System.Windows.Forms.ComboBox()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.lblSizes = New System.Windows.Forms.Label()
        Me.txtLength = New System.Windows.Forms.TextBox()
        Me.txtWidth = New System.Windows.Forms.TextBox()
        Me.txtHeight = New System.Windows.Forms.TextBox()
        Me.lblLength = New System.Windows.Forms.Label()
        Me.lblWidth = New System.Windows.Forms.Label()
        Me.lblHeight = New System.Windows.Forms.Label()
        Me.txtDensity = New System.Windows.Forms.TextBox()
        Me.lblDensity = New System.Windows.Forms.Label()
        Me.lblColor = New System.Windows.Forms.Label()
        Me.cboProductColor = New System.Windows.Forms.ComboBox()
        Me.lblBaseProduct = New System.Windows.Forms.Label()
        Me.cboBaseProduct = New System.Windows.Forms.ComboBox()
        Me.cboStorageUnitID = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.chkHasDimensions = New System.Windows.Forms.CheckBox()
        Me.cboProductTypeID = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cboPricingUnitID = New System.Windows.Forms.ComboBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.cboMixTypeID = New System.Windows.Forms.ComboBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.cboProductionUnit = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtProductCode
        '
        Me.txtProductCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtProductCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtProductCode.Location = New System.Drawing.Point(2, 1)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(158, 24)
        Me.txtProductCode.TabIndex = 0
        Me.txtProductCode.TabStop = False
        Me.txtProductCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblProductCode
        '
        Me.lblProductCode.AutoSize = True
        Me.lblProductCode.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProductCode.Location = New System.Drawing.Point(891, 47)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(74, 18)
        Me.lblProductCode.TabIndex = 1
        Me.lblProductCode.Text = "كود الصنف"
        '
        'lblBarCode
        '
        Me.lblBarCode.AutoSize = True
        Me.lblBarCode.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBarCode.Location = New System.Drawing.Point(913, 427)
        Me.lblBarCode.Name = "lblBarCode"
        Me.lblBarCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblBarCode.Size = New System.Drawing.Size(51, 18)
        Me.lblBarCode.TabIndex = 3
        Me.lblBarCode.Text = "الباركود"
        '
        'txtBarcode
        '
        Me.txtBarcode.Location = New System.Drawing.Point(645, 421)
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.ReadOnly = True
        Me.txtBarcode.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtBarcode.Size = New System.Drawing.Size(165, 24)
        Me.txtBarcode.TabIndex = 12
        Me.txtBarcode.TabStop = False
        '
        'lblProductName
        '
        Me.lblProductName.AutoSize = True
        Me.lblProductName.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProductName.Location = New System.Drawing.Point(838, 89)
        Me.lblProductName.Name = "lblProductName"
        Me.lblProductName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblProductName.Size = New System.Drawing.Size(127, 18)
        Me.lblProductName.TabIndex = 5
        Me.lblProductName.Text = "اسم الصنف العربي"
        '
        'txtProductName
        '
        Me.txtProductName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtProductName.Location = New System.Drawing.Point(646, 83)
        Me.txtProductName.Name = "txtProductName"
        Me.txtProductName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtProductName.Size = New System.Drawing.Size(165, 24)
        Me.txtProductName.TabIndex = 0
        Me.txtProductName.Tag = ""
        '
        'lblProductEngName
        '
        Me.lblProductEngName.AutoSize = True
        Me.lblProductEngName.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProductEngName.Location = New System.Drawing.Point(878, 131)
        Me.lblProductEngName.Name = "lblProductEngName"
        Me.lblProductEngName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblProductEngName.Size = New System.Drawing.Size(89, 18)
        Me.lblProductEngName.TabIndex = 7
        Me.lblProductEngName.Text = "اسم انجليزي"
        '
        'txtProductEnglishName
        '
        Me.txtProductEnglishName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtProductEnglishName.Location = New System.Drawing.Point(646, 125)
        Me.txtProductEnglishName.Name = "txtProductEnglishName"
        Me.txtProductEnglishName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtProductEnglishName.Size = New System.Drawing.Size(165, 24)
        Me.txtProductEnglishName.TabIndex = 1
        '
        'lblCategory
        '
        Me.lblCategory.AutoSize = True
        Me.lblCategory.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCategory.Location = New System.Drawing.Point(927, 173)
        Me.lblCategory.Name = "lblCategory"
        Me.lblCategory.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblCategory.Size = New System.Drawing.Size(38, 18)
        Me.lblCategory.TabIndex = 9
        Me.lblCategory.Text = "الفئة"
        '
        'lblSubCategory
        '
        Me.lblSubCategory.AutoSize = True
        Me.lblSubCategory.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSubCategory.Location = New System.Drawing.Point(879, 215)
        Me.lblSubCategory.Name = "lblSubCategory"
        Me.lblSubCategory.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblSubCategory.Size = New System.Drawing.Size(88, 18)
        Me.lblSubCategory.TabIndex = 11
        Me.lblSubCategory.Text = "الفئة الفرعية"
        '
        'lblProductGroup
        '
        Me.lblProductGroup.AutoSize = True
        Me.lblProductGroup.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProductGroup.Location = New System.Drawing.Point(446, 257)
        Me.lblProductGroup.Name = "lblProductGroup"
        Me.lblProductGroup.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblProductGroup.Size = New System.Drawing.Size(105, 18)
        Me.lblProductGroup.TabIndex = 15
        Me.lblProductGroup.Text = "مجموعة الصنف"
        '
        'lblStandardQTY
        '
        Me.lblStandardQTY.AutoSize = True
        Me.lblStandardQTY.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStandardQTY.Location = New System.Drawing.Point(467, 427)
        Me.lblStandardQTY.Name = "lblStandardQTY"
        Me.lblStandardQTY.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblStandardQTY.Size = New System.Drawing.Size(79, 18)
        Me.lblStandardQTY.TabIndex = 29
        Me.lblStandardQTY.Text = "وحدة الانتاج"
        '
        'chkProductStatus
        '
        Me.chkProductStatus.AutoSize = True
        Me.chkProductStatus.Checked = True
        Me.chkProductStatus.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkProductStatus.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkProductStatus.Location = New System.Drawing.Point(3, 3)
        Me.chkProductStatus.Name = "chkProductStatus"
        Me.chkProductStatus.Size = New System.Drawing.Size(61, 22)
        Me.chkProductStatus.TabIndex = 30
        Me.chkProductStatus.TabStop = False
        Me.chkProductStatus.Text = "نشط"
        Me.chkProductStatus.UseVisualStyleBackColor = True
        '
        'btnNew
        '
        Me.btnNew.BackColor = System.Drawing.Color.Gainsboro
        Me.btnNew.Location = New System.Drawing.Point(3, 375)
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(134, 40)
        Me.btnNew.TabIndex = 1
        Me.btnNew.TabStop = False
        Me.btnNew.Text = "New"
        Me.btnNew.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.Gainsboro
        Me.btnSave.Location = New System.Drawing.Point(3, 421)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(134, 40)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btndelete
        '
        Me.btndelete.BackColor = System.Drawing.Color.Gainsboro
        Me.btndelete.Location = New System.Drawing.Point(3, 467)
        Me.btndelete.Name = "btndelete"
        Me.btndelete.Size = New System.Drawing.Size(134, 40)
        Me.btndelete.TabIndex = 35
        Me.btndelete.TabStop = False
        Me.btndelete.Text = "Delete"
        Me.btndelete.UseVisualStyleBackColor = False
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.Gainsboro
        Me.btnSearch.Location = New System.Drawing.Point(3, 513)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(134, 40)
        Me.btnSearch.TabIndex = 36
        Me.btnSearch.TabStop = False
        Me.btnSearch.Text = "Search"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.Gainsboro
        Me.btnClose.Location = New System.Drawing.Point(3, 605)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(134, 40)
        Me.btnClose.TabIndex = 2
        Me.btnClose.TabStop = False
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'cboCategory
        '
        Me.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCategory.FormattingEnabled = True
        Me.cboCategory.ItemHeight = 16
        Me.cboCategory.Location = New System.Drawing.Point(646, 167)
        Me.cboCategory.Name = "cboCategory"
        Me.cboCategory.Size = New System.Drawing.Size(165, 24)
        Me.cboCategory.TabIndex = 2
        '
        'cboProductGroup
        '
        Me.cboProductGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProductGroup.FormattingEnabled = True
        Me.cboProductGroup.Location = New System.Drawing.Point(218, 251)
        Me.cboProductGroup.Name = "cboProductGroup"
        Me.cboProductGroup.Size = New System.Drawing.Size(165, 24)
        Me.cboProductGroup.TabIndex = 6
        '
        'cboSubCategory
        '
        Me.cboSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSubCategory.FormattingEnabled = True
        Me.cboSubCategory.Location = New System.Drawing.Point(646, 209)
        Me.cboSubCategory.Name = "cboSubCategory"
        Me.cboSubCategory.Size = New System.Drawing.Size(165, 24)
        Me.cboSubCategory.TabIndex = 3
        '
        'txtMandatory
        '
        Me.txtMandatory.AutoSize = True
        Me.txtMandatory.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMandatory.ForeColor = System.Drawing.Color.Red
        Me.txtMandatory.Location = New System.Drawing.Point(973, 47)
        Me.txtMandatory.Name = "txtMandatory"
        Me.txtMandatory.Size = New System.Drawing.Size(16, 18)
        Me.txtMandatory.TabIndex = 38
        Me.txtMandatory.Text = "*"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(973, 173)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(16, 18)
        Me.Label2.TabIndex = 39
        Me.Label2.Text = "*"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Red
        Me.Label3.Location = New System.Drawing.Point(973, 215)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(16, 18)
        Me.Label3.TabIndex = 39
        Me.Label3.Text = "*"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.Red
        Me.Label7.Location = New System.Drawing.Point(557, 257)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(16, 18)
        Me.Label7.TabIndex = 39
        Me.Label7.Text = "*"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Red
        Me.Label9.Location = New System.Drawing.Point(973, 299)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(16, 18)
        Me.Label9.TabIndex = 39
        Me.Label9.Text = "*"
        '
        'lblTaxType
        '
        Me.lblTaxType.AutoSize = True
        Me.lblTaxType.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTaxType.Location = New System.Drawing.Point(459, 384)
        Me.lblTaxType.Name = "lblTaxType"
        Me.lblTaxType.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblTaxType.Size = New System.Drawing.Size(92, 18)
        Me.lblTaxType.TabIndex = 41
        Me.lblTaxType.Text = "وحدة للتسعير"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.ForeColor = System.Drawing.Color.Red
        Me.Label17.Location = New System.Drawing.Point(557, 384)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(16, 18)
        Me.Label17.TabIndex = 39
        Me.Label17.Text = "*"
        '
        'cboTaxType
        '
        Me.cboTaxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTaxType.FormattingEnabled = True
        Me.cboTaxType.Location = New System.Drawing.Point(218, 335)
        Me.cboTaxType.Name = "cboTaxType"
        Me.cboTaxType.Size = New System.Drawing.Size(165, 24)
        Me.cboTaxType.TabIndex = 10
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(218, 479)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(589, 190)
        Me.txtDescription.TabIndex = 14
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.Gainsboro
        Me.btnPrint.Location = New System.Drawing.Point(3, 559)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(134, 40)
        Me.btnPrint.TabIndex = 46
        Me.btnPrint.TabStop = False
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'lblSizes
        '
        Me.lblSizes.AutoSize = True
        Me.lblSizes.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSizes.Location = New System.Drawing.Point(256, 75)
        Me.lblSizes.Name = "lblSizes"
        Me.lblSizes.Size = New System.Drawing.Size(68, 18)
        Me.lblSizes.TabIndex = 47
        Me.lblSizes.Text = "المقاسات"
        '
        'txtLength
        '
        Me.txtLength.Location = New System.Drawing.Point(4, 69)
        Me.txtLength.Name = "txtLength"
        Me.txtLength.Size = New System.Drawing.Size(53, 24)
        Me.txtLength.TabIndex = 10
        '
        'txtWidth
        '
        Me.txtWidth.Location = New System.Drawing.Point(4, 94)
        Me.txtWidth.Name = "txtWidth"
        Me.txtWidth.Size = New System.Drawing.Size(53, 24)
        Me.txtWidth.TabIndex = 11
        '
        'txtHeight
        '
        Me.txtHeight.Location = New System.Drawing.Point(4, 119)
        Me.txtHeight.Name = "txtHeight"
        Me.txtHeight.Size = New System.Drawing.Size(53, 24)
        Me.txtHeight.TabIndex = 12
        '
        'lblLength
        '
        Me.lblLength.AutoSize = True
        Me.lblLength.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLength.Location = New System.Drawing.Point(73, 72)
        Me.lblLength.Name = "lblLength"
        Me.lblLength.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblLength.Size = New System.Drawing.Size(96, 18)
        Me.lblLength.TabIndex = 49
        Me.lblLength.Text = "الطول   (سم)"
        '
        'lblWidth
        '
        Me.lblWidth.AutoSize = True
        Me.lblWidth.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWidth.Location = New System.Drawing.Point(71, 97)
        Me.lblWidth.Name = "lblWidth"
        Me.lblWidth.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblWidth.Size = New System.Drawing.Size(98, 18)
        Me.lblWidth.TabIndex = 49
        Me.lblWidth.Text = "العرض   (سم)"
        '
        'lblHeight
        '
        Me.lblHeight.AutoSize = True
        Me.lblHeight.Location = New System.Drawing.Point(71, 123)
        Me.lblHeight.Name = "lblHeight"
        Me.lblHeight.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblHeight.Size = New System.Drawing.Size(98, 17)
        Me.lblHeight.TabIndex = 49
        Me.lblHeight.Text = "الارتفاع    (سم)"
        '
        'txtDensity
        '
        Me.txtDensity.Location = New System.Drawing.Point(646, 378)
        Me.txtDensity.Name = "txtDensity"
        Me.txtDensity.Size = New System.Drawing.Size(165, 24)
        Me.txtDensity.TabIndex = 11
        '
        'lblDensity
        '
        Me.lblDensity.AutoSize = True
        Me.lblDensity.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDensity.Location = New System.Drawing.Point(851, 384)
        Me.lblDensity.Name = "lblDensity"
        Me.lblDensity.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblDensity.Size = New System.Drawing.Size(114, 18)
        Me.lblDensity.TabIndex = 49
        Me.lblDensity.Text = "الكثافة (كجم\م³)"
        '
        'lblColor
        '
        Me.lblColor.AutoSize = True
        Me.lblColor.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColor.Location = New System.Drawing.Point(927, 299)
        Me.lblColor.Name = "lblColor"
        Me.lblColor.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblColor.Size = New System.Drawing.Size(38, 18)
        Me.lblColor.TabIndex = 41
        Me.lblColor.Text = "اللون"
        '
        'cboProductColor
        '
        Me.cboProductColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProductColor.FormattingEnabled = True
        Me.cboProductColor.Location = New System.Drawing.Point(646, 293)
        Me.cboProductColor.Name = "cboProductColor"
        Me.cboProductColor.Size = New System.Drawing.Size(165, 24)
        Me.cboProductColor.TabIndex = 7
        '
        'lblBaseProduct
        '
        Me.lblBaseProduct.AutoSize = True
        Me.lblBaseProduct.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBaseProduct.Location = New System.Drawing.Point(211, 44)
        Me.lblBaseProduct.Name = "lblBaseProduct"
        Me.lblBaseProduct.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblBaseProduct.Size = New System.Drawing.Size(113, 18)
        Me.lblBaseProduct.TabIndex = 41
        Me.lblBaseProduct.Text = "المنتج الاساسي"
        '
        'cboBaseProduct
        '
        Me.cboBaseProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBaseProduct.FormattingEnabled = True
        Me.cboBaseProduct.Location = New System.Drawing.Point(4, 38)
        Me.cboBaseProduct.Name = "cboBaseProduct"
        Me.cboBaseProduct.Size = New System.Drawing.Size(165, 24)
        Me.cboBaseProduct.TabIndex = 8
        Me.cboBaseProduct.Visible = False
        '
        'cboStorageUnitID
        '
        Me.cboStorageUnitID.FormattingEnabled = True
        Me.cboStorageUnitID.Location = New System.Drawing.Point(218, 293)
        Me.cboStorageUnitID.Name = "cboStorageUnitID"
        Me.cboStorageUnitID.Size = New System.Drawing.Size(165, 24)
        Me.cboStorageUnitID.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Red
        Me.Label4.Location = New System.Drawing.Point(973, 257)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(16, 18)
        Me.Label4.TabIndex = 39
        Me.Label4.Text = "*"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(437, 299)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label5.Size = New System.Drawing.Size(114, 18)
        Me.Label5.TabIndex = 41
        Me.Label5.Text = "الوحدة الأساسية"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Red
        Me.Label6.Location = New System.Drawing.Point(557, 299)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(16, 18)
        Me.Label6.TabIndex = 39
        Me.Label6.Text = "*"
        '
        'chkHasDimensions
        '
        Me.chkHasDimensions.AutoSize = True
        Me.chkHasDimensions.Location = New System.Drawing.Point(151, 12)
        Me.chkHasDimensions.Name = "chkHasDimensions"
        Me.chkHasDimensions.Size = New System.Drawing.Size(18, 17)
        Me.chkHasDimensions.TabIndex = 52
        Me.chkHasDimensions.UseVisualStyleBackColor = True
        '
        'cboProductTypeID
        '
        Me.cboProductTypeID.FormattingEnabled = True
        Me.cboProductTypeID.Location = New System.Drawing.Point(646, 251)
        Me.cboProductTypeID.Name = "cboProductTypeID"
        Me.cboProductTypeID.Size = New System.Drawing.Size(165, 24)
        Me.cboProductTypeID.TabIndex = 5
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(930, 257)
        Me.Label8.Name = "Label8"
        Me.Label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label8.Size = New System.Drawing.Size(37, 18)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "النوع"
        '
        'cboPricingUnitID
        '
        Me.cboPricingUnitID.FormattingEnabled = True
        Me.cboPricingUnitID.Location = New System.Drawing.Point(218, 378)
        Me.cboPricingUnitID.Name = "cboPricingUnitID"
        Me.cboPricingUnitID.Size = New System.Drawing.Size(165, 24)
        Me.cboPricingUnitID.TabIndex = 12
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(498, 341)
        Me.Label18.Name = "Label18"
        Me.Label18.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label18.Size = New System.Drawing.Size(53, 18)
        Me.Label18.TabIndex = 41
        Me.Label18.Text = "الضريبة"
        '
        'cboMixTypeID
        '
        Me.cboMixTypeID.FormattingEnabled = True
        Me.cboMixTypeID.Location = New System.Drawing.Point(646, 335)
        Me.cboMixTypeID.Name = "cboMixTypeID"
        Me.cboMixTypeID.Size = New System.Drawing.Size(165, 24)
        Me.cboMixTypeID.TabIndex = 9
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(853, 341)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(114, 18)
        Me.Label19.TabIndex = 47
        Me.Label19.Text = "نوع المكس الناتج"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.BackColor = System.Drawing.Color.Transparent
        Me.Label20.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.ForeColor = System.Drawing.Color.Red
        Me.Label20.Location = New System.Drawing.Point(258, 19)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(257, 18)
        Me.Label20.TabIndex = 53
        Me.Label20.Text = "الصنف في حالة له مقاسات يكود اليا"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.IndianRed
        Me.Panel1.Controls.Add(Me.btnNew)
        Me.Panel1.Controls.Add(Me.btnSave)
        Me.Panel1.Controls.Add(Me.btndelete)
        Me.Panel1.Controls.Add(Me.btnSearch)
        Me.Panel1.Controls.Add(Me.btnClose)
        Me.Panel1.Controls.Add(Me.btnPrint)
        Me.Panel1.Controls.Add(Me.chkProductStatus)
        Me.Panel1.Location = New System.Drawing.Point(1030, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(140, 657)
        Me.Panel1.TabIndex = 55
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(901, 480)
        Me.Label10.Name = "Label10"
        Me.Label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label10.Size = New System.Drawing.Size(64, 18)
        Me.Label10.TabIndex = 41
        Me.Label10.Text = "ملاجظات"
        Me.Label10.Visible = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(202, 15)
        Me.Label13.Name = "Label13"
        Me.Label13.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label13.Size = New System.Drawing.Size(122, 18)
        Me.Label13.TabIndex = 49
        Me.Label13.Text = "الصنف له مقاسات"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.Info
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.cboBaseProduct)
        Me.Panel2.Controls.Add(Me.lblBaseProduct)
        Me.Panel2.Controls.Add(Me.lblSizes)
        Me.Panel2.Controls.Add(Me.txtLength)
        Me.Panel2.Controls.Add(Me.txtWidth)
        Me.Panel2.Controls.Add(Me.txtHeight)
        Me.Panel2.Controls.Add(Me.lblLength)
        Me.Panel2.Controls.Add(Me.chkHasDimensions)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.lblWidth)
        Me.Panel2.Controls.Add(Me.lblHeight)
        Me.Panel2.Location = New System.Drawing.Point(218, 41)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(334, 150)
        Me.Panel2.TabIndex = 56
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 19.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(546, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label1.Size = New System.Drawing.Size(37, 40)
        Me.Label1.TabIndex = 41
        Me.Label1.Text = "⮞"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Tahoma", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.ForeColor = System.Drawing.Color.Red
        Me.Label16.Location = New System.Drawing.Point(574, 38)
        Me.Label16.Name = "Label16"
        Me.Label16.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label16.Size = New System.Drawing.Size(32, 34)
        Me.Label16.TabIndex = 41
        Me.Label16.Text = "⮞"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Tahoma", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.ForeColor = System.Drawing.Color.Red
        Me.Label21.Location = New System.Drawing.Point(598, 41)
        Me.Label21.Name = "Label21"
        Me.Label21.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label21.Size = New System.Drawing.Size(26, 28)
        Me.Label21.TabIndex = 41
        Me.Label21.Text = "⮞"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Tahoma", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.ForeColor = System.Drawing.Color.Red
        Me.Label22.Location = New System.Drawing.Point(618, 44)
        Me.Label22.Name = "Label22"
        Me.Label22.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label22.Size = New System.Drawing.Size(21, 22)
        Me.Label22.TabIndex = 41
        Me.Label22.Text = "⮞"
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.Red
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.txtProductCode)
        Me.Panel3.ForeColor = System.Drawing.Color.Red
        Me.Panel3.Location = New System.Drawing.Point(645, 42)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(166, 30)
        Me.Panel3.TabIndex = 57
        '
        'cboProductionUnit
        '
        Me.cboProductionUnit.FormattingEnabled = True
        Me.cboProductionUnit.Location = New System.Drawing.Point(218, 421)
        Me.cboProductionUnit.Name = "cboProductionUnit"
        Me.cboProductionUnit.Size = New System.Drawing.Size(165, 24)
        Me.cboProductionUnit.TabIndex = 58
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.Red
        Me.Label11.Location = New System.Drawing.Point(557, 433)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(16, 18)
        Me.Label11.TabIndex = 39
        Me.Label11.Text = "*"
        '
        'frmProduct
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1182, 687)
        Me.ControlBox = False
        Me.Controls.Add(Me.cboProductionUnit)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.cboMixTypeID)
        Me.Controls.Add(Me.cboPricingUnitID)
        Me.Controls.Add(Me.cboProductTypeID)
        Me.Controls.Add(Me.cboStorageUnitID)
        Me.Controls.Add(Me.lblDensity)
        Me.Controls.Add(Me.txtDensity)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.cboProductColor)
        Me.Controls.Add(Me.cboTaxType)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.lblColor)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.lblTaxType)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtMandatory)
        Me.Controls.Add(Me.cboSubCategory)
        Me.Controls.Add(Me.cboProductGroup)
        Me.Controls.Add(Me.cboCategory)
        Me.Controls.Add(Me.lblStandardQTY)
        Me.Controls.Add(Me.lblProductGroup)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.lblSubCategory)
        Me.Controls.Add(Me.lblCategory)
        Me.Controls.Add(Me.lblProductEngName)
        Me.Controls.Add(Me.txtProductEnglishName)
        Me.Controls.Add(Me.lblProductName)
        Me.Controls.Add(Me.txtProductName)
        Me.Controls.Add(Me.lblBarCode)
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtDescription)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(50, 50)
        Me.Name = "frmProduct"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ترميز الصنف"
        Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtProductCode As TextBox
    Friend WithEvents lblProductCode As Label
    Friend WithEvents lblBarCode As Label
    Friend WithEvents txtBarcode As TextBox
    Friend WithEvents lblProductName As Label
    Friend WithEvents txtProductName As TextBox
    Friend WithEvents lblProductEngName As Label
    Friend WithEvents txtProductEnglishName As TextBox
    Friend WithEvents lblCategory As Label
    Friend WithEvents lblSubCategory As Label
    Friend WithEvents lblProductGroup As Label
    Friend WithEvents lblStandardQTY As Label
    Friend WithEvents chkProductStatus As CheckBox
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btndelete As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents cboCategory As ComboBox
    Friend WithEvents cboProductGroup As ComboBox
    Friend WithEvents cboSubCategory As ComboBox
    Friend WithEvents txtMandatory As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents lblTaxType As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents cboTaxType As ComboBox
    Friend WithEvents txtDescription As TextBox
    Friend WithEvents btnPrint As Button
    Friend WithEvents lblSizes As Label
    Friend WithEvents txtLength As TextBox
    Friend WithEvents txtWidth As TextBox
    Friend WithEvents txtHeight As TextBox
    Friend WithEvents lblLength As Label
    Friend WithEvents lblWidth As Label
    Friend WithEvents lblHeight As Label
    Friend WithEvents txtDensity As TextBox
    Friend WithEvents lblDensity As Label
    Friend WithEvents lblColor As Label
    Friend WithEvents cboProductColor As ComboBox
    Friend WithEvents lblBaseProduct As Label
    Friend WithEvents cboBaseProduct As ComboBox
    Friend WithEvents cboStorageUnitID As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents chkHasDimensions As CheckBox
    Friend WithEvents cboProductTypeID As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents cboPricingUnitID As ComboBox
    Friend WithEvents Label18 As Label
    Friend WithEvents cboMixTypeID As ComboBox
    Friend WithEvents Label19 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label10 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents cboProductionUnit As ComboBox
    Friend WithEvents Label11 As Label
End Class
