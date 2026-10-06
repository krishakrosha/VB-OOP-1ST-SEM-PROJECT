Public Class PolymorphismForm

    Private Sub PolymorphismForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        cmbAnimal.Items.AddRange({"Dog", "Cat"})
        cmbAnimal.SelectedIndex = 0

    End Sub

    ' Both Dog and Cat are Animals, so one variable can hold either of them.
    Private Sub btnExecution_Click(sender As Object, e As EventArgs) Handles btnExecution.Click

        Dim animal As Animal

        If cmbAnimal.SelectedItem.ToString() = "Dog" Then
            animal = New Dog()
        Else
            animal = New Cat()
        End If

        lblResult.Text = animal.Speak()

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

End Class
