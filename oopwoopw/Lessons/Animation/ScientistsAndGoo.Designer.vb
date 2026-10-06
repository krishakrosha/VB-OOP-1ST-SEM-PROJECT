<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScientistsAndGoo
    Inherits ThemedForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        SuspendLayout()
        ' 
        ' ScientistsAndGoo
        ' 
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(900, 600)
        Name = "ScientistsAndGoo"
        Text = "3 Scientists and 3 Goo"
        ResumeLayout(False)
    End Sub
End Class
