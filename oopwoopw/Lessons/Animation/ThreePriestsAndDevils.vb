Imports System.Drawing.Drawing2D

' =====================================================================
'  3 PRIESTS AND 3 DEVILS  (Cannibals & Missionaries)
'
'  GOAL : carry all 3 priests and all 3 devils across the river.
'  RULES: - the boat holds at most 2 passengers and needs at least 1 rower
'         - if devils OUTNUMBER the priests on a bank (and at least one
'           priest is there), the devils eat the priests -> game over
'
'  HOW IT USES THE LESSON TOOLS
'   - PictureBox : every priest, every devil, the boat and the clouds are
'                  PictureBoxes sitting on top of the scenery PictureBox.
'   - Timer      : tmrAnim (20 ms) is the animation loop. Every tick it
'                  moves the boat, slides the characters, drifts the
'                  clouds and bobs the boat on the water.
'                  tmrClock (1 s) is the stopwatch.
'   - Location   : movement is done by changing a PictureBox's Location
'                  a few pixels at a time toward its target Location.
'   - All pictures are drawn in code (MakePriest, MakeDevil, MakeBoat...)
'     so no image files are needed.
'
'  HOW THE PUZZLE STATE WORKS
'   Each Figure has a Side: 0 = left bank, 1 = right bank, 2 = on boat.
'   The boat has a side too (0 or 1). Safety is checked every time the
'   boat finishes crossing.
' =====================================================================
Public Class ThreePriestsAndDevils

    ' the scenery fills the whole window, so no title band is added
    Protected Overrides ReadOnly Property ShowHeader As Boolean
        Get
            Return False
        End Get
    End Property

    ' One priest or one devil
    Private Class Figure
        Public IsPriest As Boolean
        Public Side As Integer          ' 0 = left bank, 1 = right bank, 2 = on boat
        Public Seat As Integer = -1     ' 0 or 1 while on the boat
        Public Box As PictureBox        ' the on-screen sprite
        Public Target As Point          ' where the sprite is heading (Location)
    End Class

    ' ---------- layout constants ----------
    Private Const SceneW As Integer = 900
    Private Const SceneH As Integer = 560
    Private Const GroundY As Integer = 340          ' top of the river banks
    Private Const BankW As Integer = 250
    Private Const BoatW As Integer = 140
    Private Const BoatH As Integer = 52
    Private Const FigW As Integer = 44
    Private Const FigH As Integer = 76
    Private Const LeftDockX As Integer = BankW + 6
    Private Const RightDockX As Integer = SceneW - BankW - BoatW - 6
    Private Const BoatY As Integer = GroundY + 6
    Private Const OptimalMoves As Integer = 11

    ' ---------- controls (all created in code) ----------
    Private pbScene As SceneBox
    Private pbBoat As PictureBox
    Private pbCloud1 As PictureBox
    Private pbCloud2 As PictureBox
    Private lblTitle As Label
    Private lblRules As Label
    Private lblMsg As Label
    Private lblStats As Label
    Private btnRow As Button
    Private btnReset As Button
    Private btnMenu As Button

    ' ---------- timers ----------
    Private WithEvents tmrAnim As New System.Windows.Forms.Timer With {.Interval = 20}
    Private WithEvents tmrClock As New System.Windows.Forms.Timer With {.Interval = 1000}

    ' ---------- pictures ----------
    Private imgPriest As Bitmap
    Private imgDevil As Bitmap

    ' ---------- game state ----------
    Private people As New List(Of Figure)
    Private boatSide As Integer = 0
    Private boatMoving As Boolean = False
    Private boatTargetX As Integer = LeftDockX
    Private gameOver As Boolean = False
    Private moves As Integer = 0
    Private seconds As Integer = 0
    Private bobTick As Single = 0

    ' =================================================================
    '  SET-UP
    ' =================================================================
    Private Sub ThreePriestsAndDevils_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "3 Priests and 3 Devils"
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        StartPosition = FormStartPosition.CenterScreen
        ClientSize = New Size(SceneW, SceneH)

        imgPriest = MakePriest()
        imgDevil = MakeDevil()

        ' the scenery is one big PictureBox; everything else sits on it
        pbScene = New SceneBox With {.Dock = DockStyle.Fill, .Image = MakeScenery()}
        Controls.Add(pbScene)

        pbCloud1 = MakeCloud(40, 150, 55)
        pbCloud2 = MakeCloud(560, 110, 150)
        pbCloud1.SendToBack()
        pbCloud2.SendToBack()

        lblTitle = MakeLabel("3 PRIESTS  &  3 DEVILS", 0, 8, SceneW, 44, AppTheme.TextFont(26, FontStyle.Bold), Color.White)
        lblRules = MakeLabel("Take everyone across the river. The boat holds 2 and needs a rower." & vbCrLf &
                             "If devils outnumber the priests on a bank, the priests get eaten!",
                             0, 54, SceneW, 44, AppTheme.TextFont(11, FontStyle.Bold), Color.FromArgb(255, 238, 200))
        lblMsg = MakeLabel("", 100, 112, 700, 32, AppTheme.TextFont(12, FontStyle.Bold), Color.White)
        lblMsg.BackColor = Color.FromArgb(150, 0, 0, 0)
        lblStats = MakeLabel("", 12, 520, 330, 28, AppTheme.TextFont(11, FontStyle.Bold), Color.White)
        lblStats.TextAlign = ContentAlignment.MiddleLeft

        ' the boat
        pbBoat = New PictureBox With {
            .Image = MakeBoat(), .Size = New Size(BoatW, BoatH),
            .BackColor = Color.Transparent, .Cursor = Cursors.Hand,
            .Location = New Point(LeftDockX, BoatY)}
        AddHandler pbBoat.Click, AddressOf RowBoat
        pbScene.Controls.Add(pbBoat)

        ' 3 priests, then 3 devils - each one is a PictureBox
        For i As Integer = 0 To 5
            Dim f As New Figure With {.IsPriest = (i < 3), .Side = 0}
            f.Box = New PictureBox With {
                .Image = If(f.IsPriest, imgPriest, imgDevil),
                .Size = New Size(FigW, FigH),
                .BackColor = Color.Transparent, .Cursor = Cursors.Hand,
                .Tag = f}
            AddHandler f.Box.Click, AddressOf FigureClicked
            pbScene.Controls.Add(f.Box)
            people.Add(f)
        Next

        btnRow = MakeButton("Row Boat", 290, 495, 140, AddressOf RowBoat)
        btnReset = MakeButton("Reset", 440, 495, 140, AddressOf ResetClicked)
        btnMenu = MakeButton("Close", 590, 495, 140, AddressOf CloseClicked)

        AddHandler FormClosed, AddressOf OnFormClosed

        ResetGame()
        tmrAnim.Start()
        tmrClock.Start()
    End Sub

    Private Function MakeLabel(txt As String, x As Integer, y As Integer, w As Integer, h As Integer,
                               f As Font, c As Color) As Label
        Dim lb As New Label With {
            .Text = txt, .Left = x, .Top = y, .Width = w, .Height = h,
            .Font = f, .ForeColor = c, .BackColor = Color.Transparent,
            .TextAlign = ContentAlignment.MiddleCenter}
        pbScene.Controls.Add(lb)
        Return lb
    End Function

    Private Function MakeButton(txt As String, x As Integer, y As Integer, w As Integer,
                                handler As EventHandler) As Button
        Dim b As New Button With {
            .Text = txt, .Left = x, .Top = y, .Width = w, .Height = 40,
            .FlatStyle = FlatStyle.Flat, .BackColor = AppTheme.PrimaryColor,
            .ForeColor = Color.White, .Font = AppTheme.TextFont(12, FontStyle.Bold),
            .Cursor = Cursors.Hand}
        b.FlatAppearance.BorderColor = AppTheme.AccentColor
        AddHandler b.Click, handler
        pbScene.Controls.Add(b)
        Return b
    End Function

    Private Function MakeCloud(x As Integer, y As Integer, seed As Integer) As PictureBox
        Dim pb As New PictureBox With {
            .Image = MakeCloudImage(), .Size = New Size(130, 56),
            .BackColor = Color.Transparent, .Location = New Point(x, y)}
        pbScene.Controls.Add(pb)
        Return pb
    End Function

    ' Puts everyone back on the left bank and clears the score
    Private Sub ResetGame()
        boatSide = 0
        boatMoving = False
        boatTargetX = LeftDockX
        gameOver = False
        moves = 0
        seconds = 0
        pbBoat.Location = New Point(LeftDockX, BoatY)
        For Each f As Figure In people
            f.Side = 0
            f.Seat = -1
            f.Box.Visible = True
        Next
        Relayout()
        For Each f As Figure In people
            f.Box.Location = f.Target
        Next
        btnRow.Enabled = True
        tmrClock.Start()
        SetMessage("Click a priest or devil to put him in the boat, then press Row Boat.")
        UpdateStats()
    End Sub

    ' =================================================================
    '  PLAYER ACTIONS
    ' =================================================================
    Private Sub FigureClicked(sender As Object, e As EventArgs)
        If boatMoving OrElse gameOver Then Return
        Dim f As Figure = DirectCast(DirectCast(sender, PictureBox).Tag, Figure)

        If f.Side = 2 Then
            ' get off the boat onto the bank where the boat is docked
            f.Side = boatSide
            f.Seat = -1
            SetMessage("Click passengers to load or unload, then press Row Boat.")
        ElseIf f.Side = boatSide Then
            Dim seat As Integer = FreeSeat()
            If seat = -1 Then
                SetMessage("The boat only holds two passengers!")
                Return
            End If
            f.Side = 2
            f.Seat = seat
            SetMessage("Click passengers to load or unload, then press Row Boat.")
        Else
            SetMessage("The boat is on the other bank - he can't reach it.")
        End If
    End Sub

    Private Sub RowBoat(sender As Object, e As EventArgs)
        If boatMoving OrElse gameOver Then Return
        If RiderCount() = 0 Then
            SetMessage("Somebody has to row! Put at least one passenger in the boat.")
            Return
        End If
        boatTargetX = If(boatSide = 0, RightDockX, LeftDockX)
        boatMoving = True
        SetMessage("Rowing across the river...")
    End Sub

    Private Sub ResetClicked(sender As Object, e As EventArgs)
        ResetGame()
    End Sub

    Private Sub CloseClicked(sender As Object, e As EventArgs)
        Close()
    End Sub

    Private Sub OnFormClosed(sender As Object, e As FormClosedEventArgs)
        tmrAnim.Stop()
        tmrClock.Stop()
    End Sub

    ' =================================================================
    '  GAME RULES
    ' =================================================================
    Private Function RiderCount() As Integer
        Dim n As Integer = 0
        For Each f As Figure In people
            If f.Side = 2 Then n += 1
        Next
        Return n
    End Function

    Private Function FreeSeat() As Integer
        For s As Integer = 0 To 1
            Dim taken As Boolean = False
            For Each f As Figure In people
                If f.Side = 2 AndAlso f.Seat = s Then taken = True
            Next
            If Not taken Then Return s
        Next
        Return -1
    End Function

    ' The boat just docked: did anybody get eaten? Did we win?
    Private Sub Arrived()
        boatSide = If(boatTargetX = RightDockX, 1, 0)
        moves += 1
        UpdateStats()

        Dim priestsL, devilsL, priestsR, devilsR As Integer
        For Each f As Figure In people
            ' passengers count as being on the bank where the boat docked
            Dim bank As Integer = If(f.Side = 2, boatSide, f.Side)
            If bank = 0 Then
                If f.IsPriest Then priestsL += 1 Else devilsL += 1
            Else
                If f.IsPriest Then priestsR += 1 Else devilsR += 1
            End If
        Next

        If priestsL > 0 AndAlso devilsL > priestsL Then
            Eaten(0)
        ElseIf priestsR > 0 AndAlso devilsR > priestsR Then
            Eaten(1)
        ElseIf priestsR = 3 AndAlso devilsR = 3 Then
            Win()
        Else
            SetMessage("Safe for now. Unload, load the next passengers, and keep going.")
        End If
    End Sub

    Private Sub Eaten(bank As Integer)
        gameOver = True
        btnRow.Enabled = False
        tmrClock.Stop()
        For Each f As Figure In people
            Dim bankOf As Integer = If(f.Side = 2, boatSide, f.Side)
            If f.IsPriest AndAlso bankOf = bank Then f.Box.Visible = False
        Next
        Dim sideName As String = If(bank = 0, "left", "right")
        SetMessage("The devils outnumbered the priests on the " & sideName & " bank and ate them! Press Reset.")
        lblMsg.ForeColor = Color.FromArgb(255, 120, 120)
    End Sub

    Private Sub Win()
        gameOver = True
        btnRow.Enabled = False
        tmrClock.Stop()
        Dim rating As String
        If moves <= OptimalMoves Then
            rating = "A perfect crossing!"
        ElseIf moves <= OptimalMoves + 4 Then
            rating = "Very good!"
        Else
            rating = "Well done - can you do it in " & OptimalMoves & "?"
        End If
        SetMessage("Everyone made it across in " & moves & " crossings. " & rating)
        lblMsg.ForeColor = Color.FromArgb(150, 255, 150)
    End Sub

    Private Sub SetMessage(txt As String)
        lblMsg.ForeColor = Color.White
        lblMsg.Text = txt
    End Sub

    Private Sub UpdateStats()
        lblStats.Text = "Crossings: " & moves & "     Time: " & (seconds \ 60).ToString("00") & ":" & (seconds Mod 60).ToString("00")
    End Sub

    ' =================================================================
    '  ANIMATION  (Timer + Location)
    ' =================================================================
    Private Sub tmrClock_Tick(sender As Object, e As EventArgs) Handles tmrClock.Tick
        If Not gameOver Then
            seconds += 1
            UpdateStats()
        End If
    End Sub

    Private Sub tmrAnim_Tick(sender As Object, e As EventArgs) Handles tmrAnim.Tick
        ' clouds drift to the right and wrap around
        pbCloud1.Left += 1
        If pbCloud1.Left > SceneW Then pbCloud1.Left = -pbCloud1.Width
        pbCloud2.Left += 2
        If pbCloud2.Left > SceneW Then pbCloud2.Left = -pbCloud2.Width

        ' boat: slide toward the far dock, otherwise just bob on the water
        bobTick += 0.12F
        If boatMoving Then
            Dim remaining As Integer = boatTargetX - pbBoat.Left
            If Math.Abs(remaining) <= 4 Then
                pbBoat.Left = boatTargetX
                boatMoving = False
                Arrived()
            Else
                pbBoat.Left += Math.Sign(remaining) * 4
            End If
        End If
        pbBoat.Top = BoatY + CInt(Math.Sin(bobTick) * 3)

        ' characters: work out where each should be, then walk toward it
        Relayout()
        For Each f As Figure In people
            If f.Side = 2 AndAlso boatMoving Then
                f.Box.Location = f.Target        ' riders travel with the boat
            Else
                StepToward(f, 8)
            End If
        Next
    End Sub

    ' Decides every figure's target Location
    Private Sub Relayout()
        Dim leftSlot As Integer = 0
        Dim rightSlot As Integer = 0
        For Each f As Figure In people
            Select Case f.Side
                Case 0      ' left bank - fill from the river edge outward
                    f.Target = New Point(BankW - FigW - 6 - leftSlot * 40, GroundY - FigH + 8)
                    leftSlot += 1
                Case 1      ' right bank
                    f.Target = New Point(SceneW - BankW + 6 + rightSlot * 40, GroundY - FigH + 8)
                    rightSlot += 1
                Case Else   ' on the boat
                    f.Target = New Point(pbBoat.Left + 20 + f.Seat * 56, pbBoat.Top + 14 - FigH)
            End Select
        Next
    End Sub

    ' Moves a PictureBox's Location up to `speed` pixels toward its target
    Private Sub StepToward(f As Figure, speed As Integer)
        Dim p As Point = f.Box.Location
        Dim dx As Integer = f.Target.X - p.X
        Dim dy As Integer = f.Target.Y - p.Y
        Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)
        If dist <= speed Then
            f.Box.Location = f.Target
        Else
            f.Box.Location = New Point(p.X + CInt(dx / dist * speed), p.Y + CInt(dy / dist * speed))
        End If
    End Sub

    ' =================================================================
    '  PICTURES  (everything is drawn with GDI+, no image files needed)
    ' =================================================================
    Private Function MakeScenery() As Bitmap
        Dim bmp As New Bitmap(SceneW, SceneH)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias

            ' dusk sky
            Using br As New LinearGradientBrush(New Rectangle(0, 0, SceneW, GroundY + 12),
                                                Color.FromArgb(88, 58, 140), Color.FromArgb(252, 168, 130), 90.0F)
                g.FillRectangle(br, 0, 0, SceneW, GroundY + 12)
            End Using

            ' setting sun
            Using br As New SolidBrush(Color.FromArgb(255, 236, 180))
                g.FillEllipse(br, 395, 250, 110, 110)
            End Using

            ' far hills
            Using br As New SolidBrush(Color.FromArgb(78, 48, 110))
                g.FillPolygon(br, New Point() {New Point(0, 352), New Point(110, 276), New Point(250, 352)})
                g.FillPolygon(br, New Point() {New Point(180, 352), New Point(320, 296), New Point(460, 352)})
                g.FillPolygon(br, New Point() {New Point(430, 352), New Point(580, 300), New Point(720, 352)})
                g.FillPolygon(br, New Point() {New Point(650, 352), New Point(790, 270), New Point(900, 352)})
            End Using

            ' river
            Dim waterTop As Integer = GroundY + 10
            Using br As New LinearGradientBrush(New Rectangle(0, waterTop, SceneW, SceneH - waterTop),
                                                Color.FromArgb(86, 120, 200), Color.FromArgb(28, 38, 100), 90.0F)
                g.FillRectangle(br, 0, waterTop, SceneW, SceneH - waterTop)
            End Using
            Using pn As New Pen(Color.FromArgb(70, 255, 255, 255), 2)
                For y0 As Integer = waterTop + 30 To SceneH - 20 Step 24
                    Dim pts As New List(Of PointF)
                    For x As Integer = 0 To SceneW Step 10
                        pts.Add(New PointF(x, y0 + CSng(Math.Sin((x + y0) / 22.0) * 3)))
                    Next
                    g.DrawLines(pn, pts.ToArray())
                Next
            End Using

            ' the two banks (sloped toward the river)
            Dim earth As Color = Color.FromArgb(112, 76, 42)
            Using br As New SolidBrush(earth)
                g.FillPolygon(br, New Point() {New Point(0, GroundY), New Point(BankW, GroundY),
                                               New Point(BankW - 24, SceneH), New Point(0, SceneH)})
                g.FillPolygon(br, New Point() {New Point(SceneW - BankW, GroundY), New Point(SceneW, GroundY),
                                               New Point(SceneW, SceneH), New Point(SceneW - BankW + 24, SceneH)})
            End Using
            Using br As New SolidBrush(Color.FromArgb(150, 94, 52))
                Dim r As New Random(7)
                For i As Integer = 0 To 45
                    Dim px As Integer = r.Next(0, BankW - 40)
                    Dim py As Integer = r.Next(GroundY + 20, SceneH - 10)
                    g.FillEllipse(br, px, py, 7, 4)
                    g.FillEllipse(br, SceneW - BankW + 30 + px, py, 7, 4)
                Next
            End Using
            Using br As New SolidBrush(Color.FromArgb(72, 124, 56))
                g.FillRectangle(br, 0, GroundY, BankW, 12)
                g.FillRectangle(br, SceneW - BankW, GroundY, BankW, 12)
            End Using
        End Using
        Return bmp
    End Function

    Private Function MakeCloudImage() As Bitmap
        Dim bmp As New Bitmap(130, 56)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Using br As New SolidBrush(Color.FromArgb(170, 255, 235, 240))
                g.FillEllipse(br, 4, 22, 60, 30)
                g.FillEllipse(br, 34, 6, 56, 38)
                g.FillEllipse(br, 66, 20, 58, 30)
                g.FillEllipse(br, 20, 28, 90, 24)
            End Using
        End Using
        Return bmp
    End Function

    Private Function MakeBoat() As Bitmap
        Dim bmp As New Bitmap(BoatW, BoatH)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            ' hull
            Using br As New SolidBrush(Color.FromArgb(120, 72, 34))
                g.FillPolygon(br, New Point() {New Point(0, 12), New Point(BoatW, 12),
                                               New Point(BoatW - 20, 44), New Point(20, 44)})
            End Using
            ' deck rim
            Using br As New SolidBrush(Color.FromArgb(176, 120, 64))
                g.FillRectangle(br, 0, 10, BoatW, 6)
            End Using
            ' planks
            Using pn As New Pen(Color.FromArgb(80, 46, 20), 2)
                g.DrawLine(pn, 14, 24, BoatW - 14, 24)
                g.DrawLine(pn, 20, 34, BoatW - 20, 34)
            End Using
            ' ripples around the hull
            Using pn As New Pen(Color.FromArgb(150, 255, 255, 255), 2)
                g.DrawArc(pn, 8, 38, BoatW - 16, 12, 0, 180)
            End Using
        End Using
        Return bmp
    End Function

    Private Function MakePriest() As Bitmap
        Dim bmp As New Bitmap(FigW, FigH)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            ' robe
            Using br As New SolidBrush(Color.FromArgb(40, 32, 52))
                g.FillPolygon(br, New Point() {New Point(15, 30), New Point(29, 30),
                                               New Point(40, 72), New Point(4, 72)})
            End Using
            ' arms
            Using br As New SolidBrush(Color.FromArgb(58, 48, 74))
                g.FillEllipse(br, 6, 32, 10, 24)
                g.FillEllipse(br, 28, 32, 10, 24)
            End Using
            ' white collar
            Using br As New SolidBrush(Color.White)
                g.FillRectangle(br, 18, 28, 8, 5)
            End Using
            ' rope belt
            Using pn As New Pen(Color.FromArgb(220, 190, 110), 2)
                g.DrawLine(pn, 12, 50, 32, 50)
            End Using
            ' golden cross
            Using br As New SolidBrush(Color.Gold)
                g.FillRectangle(br, 20, 36, 4, 15)
                g.FillRectangle(br, 16, 40, 12, 4)
            End Using
            ' head
            Using br As New SolidBrush(Color.FromArgb(244, 214, 178))
                g.FillEllipse(br, 13, 8, 18, 21)
            End Using
            ' hair
            Using br As New SolidBrush(Color.FromArgb(90, 60, 40))
                g.FillPie(br, 13, 7, 18, 14, 180, 180)
            End Using
            ' eyes + smile
            Using br As New SolidBrush(Color.Black)
                g.FillEllipse(br, 17, 17, 3, 3)
                g.FillEllipse(br, 24, 17, 3, 3)
            End Using
            Using pn As New Pen(Color.FromArgb(150, 70, 60), 1)
                g.DrawArc(pn, 18, 19, 8, 6, 20, 140)
            End Using
            ' halo
            Using pn As New Pen(Color.Gold, 2)
                g.DrawEllipse(pn, 12, 0, 20, 7)
            End Using
            ' shoes
            Using br As New SolidBrush(Color.FromArgb(30, 24, 30))
                g.FillEllipse(br, 6, 70, 14, 6)
                g.FillEllipse(br, 24, 70, 14, 6)
            End Using
        End Using
        Return bmp
    End Function

    Private Function MakeDevil() As Bitmap
        Dim bmp As New Bitmap(FigW, FigH)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Dim red As Color = Color.FromArgb(200, 36, 36)
            Dim dark As Color = Color.FromArgb(120, 16, 16)

            ' pitchfork
            Using pn As New Pen(Color.FromArgb(70, 70, 70), 2)
                g.DrawLine(pn, 5, 26, 5, 72)
                g.DrawLine(pn, 1, 26, 9, 26)
                g.DrawLine(pn, 1, 20, 1, 26)
                g.DrawLine(pn, 5, 18, 5, 26)
                g.DrawLine(pn, 9, 20, 9, 26)
            End Using
            ' tail
            Using pn As New Pen(dark, 3)
                g.DrawBezier(pn, New Point(32, 62), New Point(46, 62), New Point(44, 46), New Point(40, 44))
            End Using
            Using br As New SolidBrush(dark)
                g.FillPolygon(br, New Point() {New Point(36, 46), New Point(44, 40), New Point(44, 50)})
            End Using
            ' body
            Using br As New SolidBrush(red)
                g.FillPolygon(br, New Point() {New Point(13, 32), New Point(31, 32),
                                               New Point(36, 68), New Point(8, 68)})
                ' arms
                g.FillEllipse(br, 7, 34, 9, 22)
                g.FillEllipse(br, 28, 34, 9, 22)
                ' head
                g.FillEllipse(br, 12, 10, 20, 22)
            End Using
            ' horns
            Using br As New SolidBrush(dark)
                g.FillPolygon(br, New Point() {New Point(13, 16), New Point(9, 2), New Point(20, 12)})
                g.FillPolygon(br, New Point() {New Point(31, 16), New Point(35, 2), New Point(24, 12)})
            End Using
            ' angry eyes
            Using br As New SolidBrush(Color.Yellow)
                g.FillEllipse(br, 16, 18, 5, 4)
                g.FillEllipse(br, 23, 18, 5, 4)
            End Using
            Using pn As New Pen(Color.Black, 2)
                g.DrawLine(pn, 15, 16, 21, 19)
                g.DrawLine(pn, 29, 16, 23, 19)
            End Using
            ' grin
            Using br As New SolidBrush(Color.FromArgb(60, 0, 0))
                g.FillPie(br, 16, 22, 12, 8, 0, 180)
            End Using
            ' feet
            Using br As New SolidBrush(dark)
                g.FillEllipse(br, 8, 69, 12, 7)
                g.FillEllipse(br, 24, 69, 12, 7)
            End Using
        End Using
        Return bmp
    End Function

End Class

' A PictureBox that paints without flicker (important when many
' transparent PictureBoxes move over it)
Friend Class SceneBox
    Inherits PictureBox

    Public Sub New()
        MyBase.New()
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
        Me.DoubleBuffered = True
    End Sub
End Class
