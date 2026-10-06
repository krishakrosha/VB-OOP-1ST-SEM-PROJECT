Public Class CodingWithVariablesNameConstantsAndCalculationsForm

    Private Sub btnExecution_Click(sender As Object, e As EventArgs) Handles btnExecution.Click

        Dim price As Double = CDbl(txtPrice.Text)
        Dim quantity As Integer = CInt(txtQuantity.Text)
        Const taxRate As Double = 0.12

        Dim subtotal As Double = price * quantity
        Dim tax As Double = subtotal * taxRate
        Dim total As Double = subtotal + tax

        lblResult.Text = $"Subtotal: {subtotal:0.00}" & vbCrLf &
                         $"Tax: {tax:0.00}" & vbCrLf &
                         $"Total: {total:0.00}"

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

End Class
