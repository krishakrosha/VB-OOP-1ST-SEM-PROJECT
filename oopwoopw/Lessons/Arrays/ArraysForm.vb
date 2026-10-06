Public Class ArraysForm

    ' Reads the three names into an array, then lists them with a loop.
    Private Sub btnExecution_Click(sender As Object, e As EventArgs) Handles btnExecution.Click

        Dim students() As String = {txtName1.Text, txtName2.Text, txtName3.Text}

        Dim list As String = "Student List"
        For i As Integer = 0 To students.Length - 1
            list &= vbCrLf & $"{i + 1}. {students(i)}"
        Next

        lblResult.Text = list

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

End Class
