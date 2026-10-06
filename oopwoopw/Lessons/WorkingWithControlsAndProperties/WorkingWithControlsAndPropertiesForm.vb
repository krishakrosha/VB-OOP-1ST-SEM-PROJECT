Public Class WorkingWithControlsAndPropertiesForm

    ' Copies the typed text into the label and changes the label's Font property.
    Private Sub btnExecution_Click(sender As Object, e As EventArgs) Handles btnExecution.Click

        lblResult.Text = txtInput.Text
        lblResult.Font = New Font("Arial", 16, FontStyle.Bold)

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

End Class
