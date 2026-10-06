' =====================================================================
'  ThemedForm  -  the base window for every lesson
'
'  When a lesson window opens this class:
'    1. paints the page background,
'    2. adds a dark title band at the top (using the form's Text),
'    3. asks AppTheme to restyle every control on the form.
'  The lesson forms only have to say  "Inherits ThemedForm".
' =====================================================================
Public Class ThemedForm
    Inherits Form

    Private Const HeaderHeight As Integer = 54

    ' The window font is set here, before the designer code runs, so the
    ' form scales all of its controls to fit the new font automatically.
    Public Sub New()
        MyBase.New()
        Font = AppTheme.TextFont(10.0F)
    End Sub

    ' A window that draws its own scenery (the two animations) turns this off.
    Protected Overridable ReadOnly Property ShowHeader As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)

        If Not DesignMode Then
            BackColor = AppTheme.PageColor
            If ShowHeader Then AddHeaderBand()
            AppTheme.StyleControls(Me)
        End If

        MyBase.OnLoad(e)

    End Sub

    Private Sub AddHeaderBand()

        Dim band As New Panel With {
            .Dock = DockStyle.Top,
            .Height = CInt(HeaderHeight * DeviceDpi / 96.0),
            .BackColor = AppTheme.HeaderColor
        }

        Dim stripe As New Panel With {
            .Dock = DockStyle.Left,
            .Width = CInt(8 * DeviceDpi / 96.0),
            .BackColor = AppTheme.AccentColor
        }

        Dim title As New Label With {
            .Text = Text,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(18, 0, 0, 0),
            .ForeColor = Color.White,
            .BackColor = Color.Transparent,
            .Font = AppTheme.TextFont(15.0F, FontStyle.Bold)
        }

        band.Controls.Add(title)
        band.Controls.Add(stripe)

        ' added last = docked first, so the band sits above everything else
        Controls.Add(band)

    End Sub

End Class
