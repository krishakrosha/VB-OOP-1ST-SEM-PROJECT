Public Class ExcessiveControls

    ' stays False until the form has finished loading, so the radio buttons
    ' do not fire their events while the form is still being set up
    Private isReady As Boolean = False

    ' =====================================================
    '  EVENTS
    ' =====================================================

    Private Sub ExcessiveControls_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        isReady = True
        RefreshScreen()
        tssMessage.Text = "Choose an operator, set A and B, then press SUBMIT."

    End Sub

    ' An operator radio button was picked
    Private Sub Operator_CheckedChanged(sender As Object, e As EventArgs) Handles rdoAnd.CheckedChanged,
                                                                                  rdoOr.CheckedChanged,
                                                                                  rdoNot.CheckedChanged,
                                                                                  rdoXor.CheckedChanged

        If Not isReady Then Return
        If Not DirectCast(sender, RadioButton).Checked Then Return

        RefreshScreen()
        tssMessage.Text = "Operator selected: " & OperatorName()

    End Sub

    ' A or B was ticked / unticked: show the new value on the check box
    Private Sub Input_CheckedChanged(sender As Object, e As EventArgs) Handles chkA.CheckedChanged,
                                                                               chkB.CheckedChanged

        chkA.Text = "A = " & chkA.Checked.ToString()
        chkB.Text = "B = " & chkB.Checked.ToString()

    End Sub

    ' SUBMIT: work out the answer and show it
    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        Dim a As Boolean = chkA.Checked
        Dim b As Boolean = chkB.Checked
        Dim result As Boolean = Evaluate(a, b)

        Dim expression As String
        If rdoNot.Checked Then
            expression = "NOT " & a.ToString()
        Else
            expression = a.ToString() & " " & OperatorName() & " " & b.ToString()
        End If

        lblResult.Text = expression & "   =   " & result.ToString()
        lblResult.ForeColor = ResultColor(result)

        HighlightRow(a, b)
        tssMessage.Text = "Submitted: " & expression & " = " & result.ToString()

    End Sub

    ' CLEAR: back to the starting state
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click

        rdoAnd.Checked = True
        chkA.Checked = False
        chkB.Checked = False

        RefreshScreen()
        tssMessage.Text = "Cleared. Choose an operator to begin."

    End Sub

    ' EXIT: close the form (FormClosing below asks for confirmation)
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Close()
    End Sub

    ' Runs for both the Exit button and the window's X button
    Private Sub ExcessiveControls_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim answer As DialogResult = MessageBox.Show("Are you sure you want to exit?",
                                                     "Exit",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question)

        If answer = DialogResult.No Then
            e.Cancel = True            ' stay open
        End If

    End Sub

    ' =====================================================
    '  HELPERS
    ' =====================================================

    ' Brings the screen in line with the chosen operator
    Private Sub RefreshScreen()

        ' NOT only uses A, so B is switched off for it
        chkB.Enabled = Not rdoNot.Checked
        If rdoNot.Checked Then chkB.Checked = False

        lblResult.Text = "Result will appear here"
        lblResult.ForeColor = Color.Gray

        BuildTruthTable()

    End Sub

    Private Function OperatorName() As String

        If rdoAnd.Checked Then Return "AND"
        If rdoOr.Checked Then Return "OR"
        If rdoNot.Checked Then Return "NOT"
        Return "XOR"

    End Function

    ' The logical operators themselves
    Private Function Evaluate(a As Boolean, b As Boolean) As Boolean

        Select Case OperatorName()
            Case "AND"
                Return a And b
            Case "OR"
                Return a Or b
            Case "NOT"
                Return Not a
            Case Else
                Return a Xor b
        End Select

    End Function

    Private Function ResultColor(result As Boolean) As Color
        If result Then Return Color.DarkGreen
        Return Color.Firebrick
    End Function

    ' Lists every possible combination for the chosen operator
    Private Sub BuildTruthTable()

        lvTruthTable.Items.Clear()

        For Each a As Boolean In {True, False}
            If rdoNot.Checked Then
                AddTruthRow(a, False)
            Else
                For Each b As Boolean In {True, False}
                    AddTruthRow(a, b)
                Next
            End If
        Next

    End Sub

    Private Sub AddTruthRow(a As Boolean, b As Boolean)

        Dim result As Boolean = Evaluate(a, b)

        Dim row As New ListViewItem(a.ToString())
        If rdoNot.Checked Then
            row.SubItems.Add("-")               ' NOT has no B
        Else
            row.SubItems.Add(b.ToString())
        End If
        row.SubItems.Add(result.ToString())
        row.ForeColor = ResultColor(result)

        lvTruthTable.Items.Add(row)

    End Sub

    ' Colours the row that matches the values you submitted
    Private Sub HighlightRow(a As Boolean, b As Boolean)

        For Each row As ListViewItem In lvTruthTable.Items

            row.BackColor = SystemColors.Window

            Dim sameA As Boolean = (row.Text = a.ToString())
            Dim sameB As Boolean = rdoNot.Checked OrElse (row.SubItems(1).Text = b.ToString())

            If sameA AndAlso sameB Then
                row.BackColor = Color.Khaki
                row.EnsureVisible()
            End If

        Next

    End Sub

End Class
