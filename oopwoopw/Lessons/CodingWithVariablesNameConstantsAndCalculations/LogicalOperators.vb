Public Class LogicalOperators

    Private Sub LogicalOperators_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        rbAnd.Checked = True
        ShowTruthTable("AND")

    End Sub

    ' The Text of each radio button is the operator name: AND, OR, NOT or XOR.
    Private Sub Operator_CheckedChanged(sender As Object, e As EventArgs) Handles rbAnd.CheckedChanged,
                                                                                  rbOr.CheckedChanged,
                                                                                  rbNot.CheckedChanged,
                                                                                  rbXor.CheckedChanged

        Dim picked As RadioButton = DirectCast(sender, RadioButton)

        If picked.Checked Then
            ShowTruthTable(picked.Text)
        End If

    End Sub

    ' Fills the grid with every True/False combination for the chosen operator.
    Private Sub ShowTruthTable(operatorName As String)

        dgvTruthTable.Columns.Clear()
        dgvTruthTable.Rows.Clear()

        dgvTruthTable.Columns.Add("A", "A")
        If operatorName <> "NOT" Then
            dgvTruthTable.Columns.Add("B", "B")
        End If
        dgvTruthTable.Columns.Add("Result", "Result")

        For Each a As Boolean In {True, False}

            If operatorName = "NOT" Then
                dgvTruthTable.Rows.Add(a, Not a)
            Else
                For Each b As Boolean In {True, False}
                    dgvTruthTable.Rows.Add(a, b, Calculate(operatorName, a, b))
                Next
            End If

        Next

    End Sub

    Private Function Calculate(operatorName As String, a As Boolean, b As Boolean) As Boolean

        Select Case operatorName
            Case "AND"
                Return a And b
            Case "OR"
                Return a Or b
            Case Else
                Return a Xor b
        End Select

    End Function

End Class
