' =====================================================================
'  AppTheme  -  the look of the whole application in one place
'
'  Change a colour or the font here and every lesson window follows.
'  Every lesson form inherits from ThemedForm (see ThemedForm.vb), which
'  calls AppTheme.StyleControls() when the window opens.
' =====================================================================
Public Module AppTheme

    ' ---------- colours (slate + teal + amber) ----------
    Public ReadOnly PageColor As Color = Color.FromArgb(241, 245, 249)      ' window background
    Public ReadOnly CardColor As Color = Color.White                        ' left-hand "lesson" panel
    Public ReadOnly TintColor As Color = Color.FromArgb(236, 247, 246)      ' right-hand "try it" panel
    Public ReadOnly InkColor As Color = Color.FromArgb(30, 41, 59)          ' normal text
    Public ReadOnly SoftInkColor As Color = Color.FromArgb(100, 116, 139)   ' hints / grey text
    Public ReadOnly LineColor As Color = Color.FromArgb(203, 213, 225)      ' borders and dividers

    Public ReadOnly PrimaryColor As Color = Color.FromArgb(13, 148, 136)    ' teal  - main buttons
    Public ReadOnly PrimaryDark As Color = Color.FromArgb(15, 118, 110)     ' hover
    Public ReadOnly HeaderColor As Color = Color.FromArgb(15, 23, 42)       ' header band / menu bar
    Public ReadOnly AccentColor As Color = Color.FromArgb(245, 158, 11)     ' amber - small highlights
    Public ReadOnly DangerColor As Color = Color.FromArgb(100, 116, 139)    ' Close / Exit buttons (calm slate)
    Public ReadOnly DangerDark As Color = Color.FromArgb(71, 85, 105)

    Public ReadOnly CodeBack As Color = Color.FromArgb(15, 23, 42)          ' code boxes look like an editor
    Public ReadOnly CodeText As Color = Color.FromArgb(167, 243, 208)

    ' ---------- fonts ----------
    Public ReadOnly TextFontName As String = PickFont("Corbel", "Segoe UI")
    Public ReadOnly CodeFontName As String = PickFont("Consolas", "Courier New")

    Private Function PickFont(preferred As String, fallback As String) As String
        For Each family As FontFamily In FontFamily.Families
            If family.Name = preferred Then Return preferred
        Next
        Return fallback
    End Function

    Public Function TextFont(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font(TextFontName, size, style)
    End Function

    Public Function CodeFont(size As Single) As Font
        Return New Font(CodeFontName, size, FontStyle.Regular)
    End Function

    ' =================================================================
    '  STYLE EVERY CONTROL ON A FORM
    ' =================================================================
    Public Sub StyleControls(parent As Control)

        For Each c As Control In parent.Controls

            If TypeOf c Is Button Then
                StyleButton(DirectCast(c, Button))

            ElseIf TypeOf c Is RichTextBox Then
                StyleRichTextBox(DirectCast(c, RichTextBox))

            ElseIf TypeOf c Is TextBox Then
                Dim box As TextBox = DirectCast(c, TextBox)
                box.BorderStyle = BorderStyle.FixedSingle
                If box.Name.ToLower().Contains("code") Then
                    box.BackColor = CodeBack
                    box.ForeColor = CodeText
                    box.Font = CodeFont(10F)
                Else
                    box.BackColor = Color.White
                    box.ForeColor = InkColor
                    box.Font = TextFont(10F)
                End If

            ElseIf TypeOf c Is ComboBox Then
                c.Font = TextFont(10F)
                c.BackColor = Color.White
                c.ForeColor = InkColor

            ElseIf TypeOf c Is NumericUpDown Then
                c.Font = TextFont(10F)
                c.BackColor = Color.White

            ElseIf TypeOf c Is ListBox Then
                Dim lb As ListBox = DirectCast(c, ListBox)
                lb.Font = TextFont(10F)
                lb.BorderStyle = BorderStyle.FixedSingle
                lb.ForeColor = InkColor

            ElseIf TypeOf c Is ListView Then
                c.Font = TextFont(11F)
                DirectCast(c, ListView).BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is DataGridView Then
                StyleGrid(DirectCast(c, DataGridView))

            ElseIf TypeOf c Is RadioButton OrElse TypeOf c Is CheckBox Then
                c.Font = TextFont(10F)
                c.ForeColor = InkColor
                c.Cursor = Cursors.Hand

            ElseIf TypeOf c Is GroupBox Then
                c.Font = TextFont(10F, FontStyle.Bold)
                c.ForeColor = PrimaryDark
                StyleControls(c)

            ElseIf TypeOf c Is Label Then
                StyleLabel(DirectCast(c, Label))

            ElseIf TypeOf c Is SplitContainer Then
                StyleSplitContainer(DirectCast(c, SplitContainer))

            ElseIf TypeOf c Is StatusStrip Then
                StyleStatusStrip(DirectCast(c, StatusStrip))

            Else
                ' TableLayoutPanel, Panel, ... : keep their children styled too
                If c.Name = "tlpLeft" Then
                    c.BackColor = CardColor
                ElseIf c.Name = "tlpRight" Then
                    c.BackColor = TintColor
                ElseIf TypeOf c Is TableLayoutPanel OrElse TypeOf c Is Panel Then
                    c.BackColor = Color.Transparent
                End If
                StyleControls(c)
            End If

        Next

    End Sub

    ' ---------- individual controls ----------
    Private Sub StyleButton(b As Button)

        Dim isQuiet As Boolean = (b.Name = "btnClose" OrElse b.Name = "btnExit" OrElse b.Name = "btnClear")
        Dim normal As Color = If(isQuiet, DangerColor, PrimaryColor)
        Dim hover As Color = If(isQuiet, DangerDark, PrimaryDark)

        b.FlatStyle = FlatStyle.Flat
        b.FlatAppearance.BorderSize = 0
        b.FlatAppearance.MouseOverBackColor = hover
        b.FlatAppearance.MouseDownBackColor = hover
        b.UseVisualStyleBackColor = False
        b.BackColor = normal
        b.ForeColor = Color.White
        b.Font = TextFont(10F, FontStyle.Bold)
        b.Cursor = Cursors.Hand

        ' a little taller than the old 23-pixel buttons
        If b.Dock <> DockStyle.Fill AndAlso b.Height < 32 Then b.Height = 32

    End Sub

    Private Sub StyleLabel(lb As Label)

        Dim n As String = lb.Name.ToLower()

        ' the preview label is the thing the lesson changes - leave it alone
        If n.Contains("preview") Then Return

        If n.EndsWith("title") Then
            ' section headings
            lb.Font = TextFont(12F, FontStyle.Bold)
            lb.ForeColor = PrimaryDark
        ElseIf n = "lblresult" Then
            ' the answer line stands out
            lb.Font = TextFont(11F, FontStyle.Bold)
            If lb.ForeColor = SystemColors.ControlText Then lb.ForeColor = HeaderColor
        Else
            lb.Font = TextFont(10F)
            If lb.ForeColor = SystemColors.ControlText Then lb.ForeColor = InkColor
        End If

        If lb.BackColor <> Color.Transparent AndAlso lb.BackColor = SystemColors.Control Then
            lb.BackColor = Color.Transparent
        End If

    End Sub

    Private Sub StyleRichTextBox(r As RichTextBox)

        r.BorderStyle = BorderStyle.None

        If r.Name.ToLower().Contains("code") Then
            ' sample code looks like a small code editor
            r.BackColor = CodeBack
            r.ForeColor = CodeText
            r.Font = CodeFont(10F)
        Else
            r.BackColor = Color.FromArgb(248, 250, 252)
            r.ForeColor = InkColor
            r.Font = TextFont(10F)
        End If

    End Sub

    Private Sub StyleGrid(g As DataGridView)

        g.EnableHeadersVisualStyles = False
        g.BorderStyle = BorderStyle.None
        g.BackgroundColor = Color.White
        g.GridColor = LineColor
        g.Font = TextFont(10F)

        g.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        g.ColumnHeadersDefaultCellStyle.Font = TextFont(10F, FontStyle.Bold)
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White

        g.DefaultCellStyle.ForeColor = InkColor
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241)
        g.DefaultCellStyle.SelectionForeColor = InkColor
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252)

    End Sub

    Private Sub StyleSplitContainer(s As SplitContainer)

        s.BackColor = LineColor          ' shows as a thin divider
        s.SplitterWidth = 2
        s.Panel1.BackColor = CardColor
        s.Panel2.BackColor = TintColor

        ' give both panels the same comfortable margin
        AddMargin(s.Panel1)
        AddMargin(s.Panel2)

        StyleControls(s.Panel1)
        StyleControls(s.Panel2)

    End Sub

    Private Sub StyleStatusStrip(s As StatusStrip)

        s.BackColor = HeaderColor
        s.ForeColor = Color.White
        s.Font = TextFont(10F)
        For Each item As ToolStripItem In s.Items
            item.ForeColor = Color.White
            item.Font = TextFont(10F)
        Next

    End Sub

    ' Moves everything in a panel so that the content starts 20 px from the
    ' left and 16 px from the top, and lines the headings up with the rest.
    Private Sub AddMargin(p As Panel)

        Dim loose As New List(Of Control)
        For Each c As Control In p.Controls
            If c.Dock = DockStyle.None Then loose.Add(c)
        Next
        If loose.Count = 0 Then Return

        Dim minLeft As Integer = Integer.MaxValue
        Dim minTop As Integer = Integer.MaxValue
        Dim minLeftBody As Integer = Integer.MaxValue

        For Each c As Control In loose
            minLeft = Math.Min(minLeft, c.Left)
            minTop = Math.Min(minTop, c.Top)
            If Not c.Name.ToLower().EndsWith("title") Then minLeftBody = Math.Min(minLeftBody, c.Left)
        Next

        Dim k As Double = p.DeviceDpi / 96.0       ' 1.0 at 100% screen scaling
        Dim dx As Integer = CInt(20 * k) - minLeft
        Dim dy As Integer = CInt(16 * k) - minTop

        For Each c As Control In loose
            c.Left += dx
            c.Top += dy
        Next

        ' headings share the left edge of the body controls
        If minLeftBody <> Integer.MaxValue Then
            For Each c As Control In loose
                If c.Name.ToLower().EndsWith("title") Then c.Left = minLeftBody + dx
            Next
        End If

    End Sub

    ' =================================================================
    '  MENU BAR COLOURS  (used by Form1)
    ' =================================================================
    Public Sub StyleMenu(menu As MenuStrip)

        menu.Renderer = New ToolStripProfessionalRenderer(New MenuColors())
        menu.BackColor = HeaderColor
        menu.ForeColor = Color.White
        menu.Font = TextFont(11F)
        menu.Padding = New Padding(10, 6, 0, 6)

        For Each item As ToolStripMenuItem In menu.Items
            StyleMenuItem(item, True)
        Next

    End Sub

    Private Sub StyleMenuItem(item As ToolStripMenuItem, isTopLevel As Boolean)

        item.Font = TextFont(11F)
        item.BackColor = If(isTopLevel, HeaderColor, Color.White)
        item.ForeColor = If(isTopLevel, Color.White, InkColor)
        item.Margin = If(isTopLevel, New Padding(0, 0, 6, 0), item.Margin)

        For Each child As ToolStripItem In item.DropDownItems
            If TypeOf child Is ToolStripMenuItem Then
                StyleMenuItem(DirectCast(child, ToolStripMenuItem), False)
            End If
        Next

    End Sub

    Private Class MenuColors
        Inherits ProfessionalColorTable

        Public Overrides ReadOnly Property MenuItemSelected As Color
            Get
                Return Color.FromArgb(204, 251, 241)
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
            Get
                Return Color.FromArgb(204, 251, 241)
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
            Get
                Return Color.FromArgb(204, 251, 241)
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
            Get
                Return PrimaryDark
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
            Get
                Return PrimaryDark
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemBorder As Color
            Get
                Return PrimaryColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuBorder As Color
            Get
                Return LineColor
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
            Get
                Return Color.White
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
            Get
                Return Color.White
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
            Get
                Return Color.White
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
            Get
                Return Color.White
            End Get
        End Property
        Public Overrides ReadOnly Property MenuStripGradientBegin As Color
            Get
                Return HeaderColor
            End Get
        End Property
        Public Overrides ReadOnly Property MenuStripGradientEnd As Color
            Get
                Return HeaderColor
            End Get
        End Property
    End Class

End Module
