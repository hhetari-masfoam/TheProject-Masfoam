Public Class frmMasterForm
    Inherits AABaseOperationForm


    Private Sub btnfrmPurchases_Click(sender As Object, e As EventArgs) Handles btnfrmPurchases.Click

        ' 🔐 تحقق صلاحية (اختياري)
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح المشتريات")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmPurchases Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmPurchases()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmMasterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        ' 🔐 إجبار تسجيل الدخول عند فتح النظام


        ReloadUIAfterUserChange()

    End Sub

    Private Sub btnChangeUser_Click(sender As Object, e As EventArgs) Handles btnChangeUser.Click

        CurrentUser.Clear()

        ' إغلاق الماستر
        Me.Close()

        ' إعادة إظهار نفس اللوجن (Startup Instance)
        Application.OpenForms.OfType(Of frmLogin)().First().Show()


    End Sub
    Private Sub ReloadUIAfterUserChange()


        ' تحديث اسم المستخدم الحالي
        lblCurrentUser.Text = CurrentUser.EmpName

        ' إعادة تحميل الصلاحيات
        LoadUserPermissions(CurrentUser.RoleID, CurrentUser.IsAdmin)

        ' إغلاق أي فورمات فرعية مفتوحة
        CloseAllChildForms()

    End Sub
    Private Sub CloseAllChildForms()
        For Each f As Form In Me.MdiChildren
            f.Close()
        Next
    End Sub
    Private Sub LoadUserPermissions(roleID As Integer, isAdmin As Boolean)

        ' نفس منطق تحميل الصلاحيات الحالي لديك
    End Sub

    Private Sub btnfrmStockTransaction_Click(sender As Object, e As EventArgs) Handles btnfrmStockTransaction.Click

        ' 🔐 تحقق صلاحية (اختياري)
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح المشتريات")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmStockTransaction Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmStockTransaction()
        frm.Show()   ' 👈 هذا كل المطلوب


    End Sub

    Private Sub frmBOM_Click(sender As Object, e As EventArgs) Handles frmBOM.Click

        ' 🔐 تحقق صلاحية (اختياري)
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmBOM Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmBOM()
        frm.Show()   ' 👈 هذا كل المطلوب


    End Sub

    Private Sub frmProduction_Click(sender As Object, e As EventArgs) Handles frmProduction.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmProduction Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmProduction()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub btnfrmProducts_Click(sender As Object, e As EventArgs) Handles btnfrmProducts.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmProduct Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmProduct()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmCutting_Click(sender As Object, e As EventArgs) Handles frmCutting.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmCutting Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmCutting()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmSaleRequest_Click(sender As Object, e As EventArgs) Handles frmSaleRequest.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmSaleRequest Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmSaleRequest()
        frm.Show()   ' 👈 هذا كل المطلوب
    End Sub

    Private Sub frmSRDistripution_Click(sender As Object, e As EventArgs) Handles frmSRDistripution.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmSRDistripution Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmSRDistripution()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmInvoice_Click(sender As Object, e As EventArgs) Handles frmInvoice.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmInvoice Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmInvoice()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub btnfrmSalesReturn_Click(sender As Object, e As EventArgs) Handles btnfrmSalesReturn.Click

        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmSalesReturn Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmSalesReturn()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmCuttingScrabCalculator_Click(sender As Object, e As EventArgs) Handles frmCuttingScrabCalculator.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmCuttingWasteCalculator Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmCuttingWasteCalculator()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub



    Private Sub frmCostCorrection_Click(sender As Object, e As EventArgs) Handles frmCostCorrection.Click
        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmCostCorrection Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmCostCorrection()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub

    Private Sub frmPurchaseReturn_Click(sender As Object, e As EventArgs) Handles frmPurchaseReturn.Click

        If Not PermissionService.Can("PURCHASE_OPEN") Then
            MessageBox.Show("لا تملك صلاحية فتح قوائم الانتاج")
            Exit Sub
        End If

        ' منع فتح أكثر من نسخة (اختياري)
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmPurchaseReturn Then
                f.Activate()
                Exit Sub
            End If
        Next

        ' فتح الفورم بشكل عادي
        Dim frm As New frmPurchaseReturn()
        frm.Show()   ' 👈 هذا كل المطلوب

    End Sub
End Class

