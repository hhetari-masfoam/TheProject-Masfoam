Public Class frmMoveSR
    Inherits AABaseOperationForm

    Private Sub btnMoveToCutting_Click(
    sender As Object,
    e As EventArgs
) Handles btnMoveToCutting.Click

        DirectCast(Me.Owner, frmSRDistripution).MoveSelectedSR(3)
        Me.Close()

    End Sub


    Private Sub btnMoveToWH_Click(
    sender As Object,
    e As EventArgs
) Handles btnMoveToWH.Click

        DirectCast(Me.Owner, frmSRDistripution).MoveSelectedSR(4)
        Me.Close()

    End Sub


    Private Sub btnBackToNew_Click(
    sender As Object,
    e As EventArgs
) Handles btnBackToNew.Click

        DirectCast(Me.Owner, frmSRDistripution).MoveSelectedSR(2)
        Me.Close()

    End Sub

End Class