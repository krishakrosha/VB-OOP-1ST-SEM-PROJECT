Public Class DataTypesAndArithmeticOperations

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click

        ' 1. both boxes must contain something
        If String.IsNullOrWhiteSpace(txtFirstNumber.Text) OrElse
           String.IsNullOrWhiteSpace(txtSecondNumber.Text) Then

            ShowWarning("Please enter both numbers.", "Missing Input")
            Return
        End If

        ' 2. the text must be a valid number of the chosen data type
        Dim firstNumber As Double
        Dim secondNumber As Double

        If Not ReadNumbers(firstNumber, secondNumber) Then
            ShowWarning("Please enter valid numbers for the selected data type.", "Invalid Input")
            Return
        End If

        ' 3. dividing by zero is not allowed
        If rbDivide.Checked AndAlso secondNumber = 0 Then
            ShowWarning("Cannot divide by zero.", "Math Error")
            Return
        End If

        ' 4. do the arithmetic and show the answer
        lblResult.Text = Calculate(firstNumber, secondNumber).ToString()

    End Sub

    ' Converts both text boxes using the data type picked with the radio buttons.
    ' Returns False when the text is not a valid number for that type.
    Private Function ReadNumbers(ByRef first As Double, ByRef second As Double) As Boolean

        If rbInteger.Checked Then
            Dim a, b As Integer
            If Integer.TryParse(txtFirstNumber.Text, a) AndAlso Integer.TryParse(txtSecondNumber.Text, b) Then
                first = a
                second = b
                Return True
            End If

        ElseIf rbDouble.Checked Then
            Return Double.TryParse(txtFirstNumber.Text, first) AndAlso
                   Double.TryParse(txtSecondNumber.Text, second)

        ElseIf rbSingle.Checked Then
            Dim a, b As Single
            If Single.TryParse(txtFirstNumber.Text, a) AndAlso Single.TryParse(txtSecondNumber.Text, b) Then
                first = a
                second = b
                Return True
            End If

        ElseIf rbDecimal.Checked Then
            Dim a, b As Decimal
            If Decimal.TryParse(txtFirstNumber.Text, a) AndAlso Decimal.TryParse(txtSecondNumber.Text, b) Then
                first = CDbl(a)
                second = CDbl(b)
                Return True
            End If
        End If

        Return False

    End Function

    ' Applies the operation picked with the radio buttons.
    Private Function Calculate(first As Double, second As Double) As Double

        If rbAdd.Checked Then
            Return first + second
        ElseIf rbSubtract.Checked Then
            Return first - second
        ElseIf rbMultiply.Checked Then
            Return first * second
        ElseIf rbDivide.Checked Then
            ' whole-number division when the Integer type is chosen
            If rbInteger.Checked Then
                Return first \ second
            End If
            Return first / second
        End If

        Return 0

    End Function

    Private Sub ShowWarning(message As String, title As String)
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

End Class
