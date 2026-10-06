Public Class Form1

    ' =====================================================
    '  WINDOW SET-UP
    ' =====================================================

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        BackColor = AppTheme.PageColor
        AppTheme.StyleMenu(MenuStrip1)
        CenterWelcomeText()

    End Sub

    ' Keeps the welcome text in the middle when the window is resized
    Private Sub pnlHome_Resize(sender As Object, e As EventArgs) Handles pnlHome.Resize
        CenterWelcomeText()
    End Sub

    Private Sub CenterWelcomeText()

        Dim items() As Control = {lblHomeTitle, pnlHomeBar, lblHomeCourse, lblHomeHint}
        Dim gap As Integer = 16

        Dim totalHeight As Integer = gap * (items.Length - 1)
        For Each item As Control In items
            totalHeight += item.Height
        Next

        Dim y As Integer = (pnlHome.ClientSize.Height - totalHeight) \ 2

        For Each item As Control In items
            item.Left = (pnlHome.ClientSize.Width - item.Width) \ 2
            item.Top = y
            y += item.Height + gap
        Next

    End Sub

    ' Every lesson opens the same way, so one helper does the work
    Private Sub OpenLesson(lesson As Form)
        lesson.Show()
    End Sub

    ' =====================================================
    '  MENU: LESSONS
    ' =====================================================

    Private Sub IntroductionToOOPToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntroductionToOOPToolStripMenuItem.Click
        OpenLesson(New Orientation())
    End Sub

    Private Sub ClassesAndObjectsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClassesAndObjectsToolStripMenuItem.Click
        OpenLesson(New ClassesAndObjectsForm())
    End Sub

    Private Sub EncapsulationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EncapsulationToolStripMenuItem.Click
        OpenLesson(New EncapsulationForm())
    End Sub

    Private Sub InheritanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InheritanceToolStripMenuItem.Click
        OpenLesson(New InheritanceForm())
    End Sub

    Private Sub PolymorphismToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PolymorphismToolStripMenuItem.Click
        OpenLesson(New PolymorphismForm())
    End Sub

    Private Sub GettingStartedWithMicrosoftVisualBasicNETToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GettingStartedWithMicrosoftVisualBasicNETToolStripMenuItem.Click
        OpenLesson(New GettingStartedWithVBNetForm())
    End Sub

    Private Sub PlanningApplicationsAndDesigningInterfacesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlanningApplicationsAndDesigningInterfacesToolStripMenuItem.Click
        OpenLesson(New PlanningApplicationsAndDesigningInterfacesForm())
    End Sub

    Private Sub DataHandlingToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles DataHandlingToolStripMenuItem1.Click
        OpenLesson(New DataHandlingForm())
    End Sub

    Private Sub DifferentDataToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DifferentDataToolStripMenuItem.Click
        OpenLesson(New DataTypesAndArithmeticOperations())
    End Sub

    Private Sub VariableNamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VariableNamesToolStripMenuItem.Click
        OpenLesson(New CodingWithVariablesNameConstantsAndCalculationsForm())
    End Sub

    Private Sub LogicalOperatorsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogicalOperatorsToolStripMenuItem.Click
        OpenLesson(New LogicalOperators())
    End Sub

    Private Sub ArrayExampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ArrayExampleToolStripMenuItem.Click
        OpenLesson(New ArraysForm())
    End Sub

    Private Sub MonthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MonthToolStripMenuItem.Click
        OpenLesson(New MonthsListbox())
    End Sub

    Private Sub DebuggingAndTracingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DebuggingAndTracingToolStripMenuItem.Click
        OpenLesson(New DebuggingAndTracingForm())
    End Sub

    Private Sub ThreePriestsAndDevilsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ThreePriestsAndDevilsToolStripMenuItem.Click
        OpenLesson(New ThreePriestsAndDevils())
    End Sub

    Private Sub ScientistsAndGooToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScientistsAndGooToolStripMenuItem.Click
        OpenLesson(New ScientistsAndGoo())
    End Sub

    Private Sub ControlSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControlSampleToolStripMenuItem.Click
        OpenLesson(New WorkingWithControlsAndPropertiesForm())
    End Sub

    Private Sub TextPropertiesManipulatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TextPropertiesManipulatorToolStripMenuItem.Click
        OpenLesson(New TextPropertiesManipulator())
    End Sub

    Private Sub ExcessiveControlsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcessiveControlsToolStripMenuItem.Click
        OpenLesson(New ExcessiveControls())
    End Sub

    ' =====================================================
    '  MENU: EXIT
    ' =====================================================

    Private Sub ExitToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem1.Click
        Close()
    End Sub

End Class
