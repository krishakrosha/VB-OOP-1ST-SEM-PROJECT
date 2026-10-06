' =====================================================================
'  3 SCIENTISTS AND 3 GOO  -  "Lava Canyon Crossing"
'
'  STORY : A lab sits on one side of a lava canyon. Three scientists and
'          three blobs of friendly (but hungry) Goo must reach the safe
'          bank on the other side using a hover platform.
'
'  RULES : - the platform holds at most 2 and needs at least 1 driver
'          - if the Goo OUTNUMBER the scientists on a bank (and at least
'            one scientist is there) the Goo dissolve the scientists
'          - get all 6 to the goal bank to win
'
'  THE THREE LESSON IDEAS USED HERE
'   - PictureBox : the lava, both banks, the hover platform and all six
'                  characters are PictureBoxes. There are NO image files
'                  and no drawing code - each PictureBox is just a solid
'                  BackColor (a letter label tells the characters apart).
'   - Timer      : tmrAnim (20 ms) is the animation loop: it slides the
'                  platform, walks the characters, makes the platform hover
'                  and makes the lava glow. tmrClock (1 s) is the stopwatch.
'   - Location   : everything moves by changing .Location a few pixels at
'                  a time toward a goal Location.
'
'  PUZZLE STATE
'   Each Piece has a Zone: 0 = start bank (top), 1 = goal bank (bottom),
'   2 = riding the platform. The platform has a side too (0 top, 1 bottom).
'   Danger is checked each time the platform finishes a trip.
' =====================================================================
Public Class ScientistsAndGoo

    ' the arena fills the whole window, so no title band is added
    Protected Overrides ReadOnly Property ShowHeader As Boolean
        Get
            Return False
        End Get
    End Property

    ' One scientist or one blob of goo
    Private Class Piece
        Public IsScientist As Boolean
        Public Zone As Integer          ' 0 = start bank, 1 = goal bank, 2 = on platform
        Public Seat As Integer = -1     ' 0 or 1 while riding
        Public Box As PictureBox        ' the on-screen block
        Public Goal As Point            ' where the block is heading
    End Class

    ' ---------- layout (the canyon is crossed top -> bottom) ----------
    Private Const ArenaW As Integer = 680
    Private Const ArenaH As Integer = 600
    Private Const StartBankH As Integer = 130
    Private Const GoalBankY As Integer = 470
    Private Const PlatW As Integer = 170
    Private Const PlatH As Integer = 70
    Private Const PlatX As Integer = 420
    Private Const TopDockY As Integer = 140
    Private Const BottomDockY As Integer = 390
    Private Const BlockSize As Integer = 50
    Private Const BestTrips As Integer = 11

    ' ---------- controls (all built in code) ----------
    Private pbArena As PictureBox       ' the lava
    Private pbStartBank As PictureBox
    Private pbGoalBank As PictureBox
    Private pbPlatform As PictureBox
    Private pnlSide As Panel
    Private lblMsg As Label
    Private lblStats As Label
    Private btnLaunch As Button
    Private btnReset As Button
    Private btnClose As Button

    ' ---------- timers ----------
    Private WithEvents tmrAnim As New System.Windows.Forms.Timer With {.Interval = 20}
    Private WithEvents tmrClock As New System.Windows.Forms.Timer With {.Interval = 1000}

    ' ---------- game state ----------
    Private pieces As New List(Of Piece)
    Private platSide As Integer = 0
    Private platMoving As Boolean = False
    Private platTargetY As Integer = TopDockY
    Private gameOver As Boolean = False
    Private trips As Integer = 0
    Private seconds As Integer = 0
    Private hoverTick As Double = 0

    ' =================================================================
    '  SET-UP
    ' =================================================================
    Private Sub ScientistsAndGoo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "3 Scientists and 3 Goo"
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        StartPosition = FormStartPosition.CenterScreen
        ClientSize = New Size(ArenaW + 220, ArenaH)
        DoubleBuffered = True

        ' --- the lava canyon (everything in the game sits on it) ---
        pbArena = New PictureBox With {
            .Location = New Point(0, 0), .Size = New Size(ArenaW, ArenaH),
            .BackColor = Color.FromArgb(200, 60, 10)}
        Controls.Add(pbArena)

        ' --- the two banks ---
        pbStartBank = New PictureBox With {
            .Location = New Point(0, 0), .Size = New Size(ArenaW, StartBankH),
            .BackColor = Color.FromArgb(66, 72, 94)}
        pbGoalBank = New PictureBox With {
            .Location = New Point(0, GoalBankY), .Size = New Size(ArenaW, ArenaH - GoalBankY),
            .BackColor = Color.FromArgb(52, 110, 84)}
        pbArena.Controls.Add(pbStartBank)
        pbArena.Controls.Add(pbGoalBank)
        NewLabel(pbStartBank, "START BANK" & vbCrLf & "(the lab)", 420, 30, 240, 70, 14, Color.White).TextAlign = ContentAlignment.MiddleRight
        NewLabel(pbGoalBank, "GOAL BANK" & vbCrLf & "(safe zone)", 420, 30, 240, 70, 14, Color.White).TextAlign = ContentAlignment.MiddleRight

        ' --- the hover platform ---
        pbPlatform = New PictureBox With {
            .Location = New Point(PlatX, TopDockY), .Size = New Size(PlatW, PlatH),
            .BackColor = Color.FromArgb(170, 175, 190), .BorderStyle = BorderStyle.FixedSingle,
            .Cursor = Cursors.Hand}
        Dim lblPlat As Label = NewLabel(pbPlatform, "HOVER PLATFORM", 0, 56, PlatW, 14, 7, Color.FromArgb(50, 50, 60))
        lblPlat.TextAlign = ContentAlignment.MiddleCenter
        AddHandler pbPlatform.Click, AddressOf LaunchPlatform
        AddHandler lblPlat.Click, AddressOf LaunchPlatform
        pbArena.Controls.Add(pbPlatform)

        ' --- 3 scientists and 3 goo: each one is a PictureBox ---
        For i As Integer = 0 To 5
            Dim p As New Piece With {.IsScientist = (i < 3), .Zone = 0}
            p.Box = New PictureBox With {
                .Size = New Size(BlockSize, BlockSize), .Tag = p,
                .BorderStyle = BorderStyle.FixedSingle, .Cursor = Cursors.Hand,
                .BackColor = If(p.IsScientist, Color.FromArgb(60, 170, 235), Color.FromArgb(120, 215, 70))}
            Dim tag As Label = NewLabel(p.Box, If(p.IsScientist, "S", "G"), 0, 0, BlockSize, BlockSize, 18,
                                        If(p.IsScientist, Color.White, Color.FromArgb(25, 70, 10)))
            tag.TextAlign = ContentAlignment.MiddleCenter
            tag.Cursor = Cursors.Hand
            AddHandler p.Box.Click, AddressOf PieceClicked
            AddHandler tag.Click, AddressOf PieceClicked
            pbArena.Controls.Add(p.Box)
            pieces.Add(p)
        Next

        ' z-order: characters on top, then the platform, then the banks
        pbPlatform.SendToBack()
        pbStartBank.SendToBack()
        pbGoalBank.SendToBack()

        BuildSidePanel()

        AddHandler FormClosed, AddressOf OnFormClosed
        ResetGame()
        tmrAnim.Start()
        tmrClock.Start()
    End Sub

    Private Sub BuildSidePanel()
        pnlSide = New Panel With {
            .Location = New Point(ArenaW, 0), .Size = New Size(220, ArenaH),
            .BackColor = Color.FromArgb(28, 30, 44)}
        Controls.Add(pnlSide)

        NewLabel(pnlSide, "3 SCIENTISTS" & vbCrLf & "& 3 GOO", 10, 10, 200, 60, 16, Color.White)

        ' small colour key
        Dim keyS As New PictureBox With {.Location = New Point(14, 82), .Size = New Size(22, 22),
                                         .BackColor = Color.FromArgb(60, 170, 235)}
        Dim keyG As New PictureBox With {.Location = New Point(14, 110), .Size = New Size(22, 22),
                                         .BackColor = Color.FromArgb(120, 215, 70)}
        pnlSide.Controls.Add(keyS)
        pnlSide.Controls.Add(keyG)
        NewLabel(pnlSide, "Scientist (S)", 44, 82, 160, 22, 10, Color.White)
        NewLabel(pnlSide, "Goo (G)", 44, 110, 160, 22, 10, Color.White)

        NewLabel(pnlSide, "Get everyone over the lava." & vbCrLf &
                          "- The platform carries 2." & vbCrLf &
                          "- It needs 1 driver." & vbCrLf &
                          "- Goo outnumbering the scientists on a bank dissolves them!",
                 12, 145, 200, 150, 10, Color.FromArgb(205, 210, 230), False)

        lblMsg = NewLabel(pnlSide, "", 12, 300, 200, 110, 10, Color.White)
        lblStats = NewLabel(pnlSide, "", 12, 418, 200, 40, 10, Color.FromArgb(255, 220, 120))

        btnLaunch = NewButton("Launch Platform", 10, 470, AddressOf LaunchPlatform)
        btnReset = NewButton("Reset", 10, 512, AddressOf ResetClicked)
        btnClose = NewButton("Close", 10, 554, AddressOf CloseClicked)
    End Sub

    Private Function NewLabel(parent As Control, txt As String, x As Integer, y As Integer,
                              w As Integer, h As Integer, size As Single, c As Color,
                              Optional bold As Boolean = True) As Label
        Dim lb As New Label With {
            .Text = txt, .Left = x, .Top = y, .Width = w, .Height = h,
            .ForeColor = c, .BackColor = Color.Transparent,
            .Font = New Font(AppTheme.TextFontName, size, If(bold, FontStyle.Bold, FontStyle.Regular))}
        parent.Controls.Add(lb)
        Return lb
    End Function

    Private Function NewButton(txt As String, x As Integer, y As Integer, handler As EventHandler) As Button
        Dim b As New Button With {
            .Text = txt, .Left = x, .Top = y, .Width = 200, .Height = 36,
            .FlatStyle = FlatStyle.Flat, .BackColor = AppTheme.PrimaryColor,
            .ForeColor = Color.White, .Font = AppTheme.TextFont(10, FontStyle.Bold),
            .Cursor = Cursors.Hand}
        b.FlatAppearance.BorderColor = AppTheme.AccentColor
        AddHandler b.Click, handler
        pnlSide.Controls.Add(b)
        Return b
    End Function

    ' Everybody back to the start bank, score cleared
    Private Sub ResetGame()
        platSide = 0
        platMoving = False
        platTargetY = TopDockY
        gameOver = False
        trips = 0
        seconds = 0
        pbPlatform.Location = New Point(PlatX, TopDockY)
        For Each p As Piece In pieces
            p.Zone = 0
            p.Seat = -1
            p.Box.Visible = True
        Next
        PlanGoals()
        For Each p As Piece In pieces
            p.Box.Location = p.Goal
        Next
        btnLaunch.Enabled = True
        tmrClock.Start()
        ShowMessage("Click a character to load the platform, then press Launch Platform.")
        UpdateStats()
    End Sub

    ' =================================================================
    '  PLAYER ACTIONS
    ' =================================================================
    Private Sub PieceClicked(sender As Object, e As EventArgs)
        If platMoving OrElse gameOver Then Return

        ' the click may come from the block itself or from its letter label
        Dim ctl As Control = DirectCast(sender, Control)
        Dim box As PictureBox
        If TypeOf ctl Is PictureBox Then
            box = DirectCast(ctl, PictureBox)
        Else
            box = DirectCast(ctl.Parent, PictureBox)
        End If
        Dim p As Piece = DirectCast(box.Tag, Piece)

        If p.Zone = 2 Then
            p.Zone = platSide                   ' step off onto the bank we are docked at
            p.Seat = -1
            ShowMessage("Load or unload, then press Launch Platform.")
        ElseIf p.Zone = platSide Then
            Dim seat As Integer = OpenSeat()
            If seat = -1 Then
                ShowMessage("The platform is full - it carries only 2.")
                Return
            End If
            p.Zone = 2
            p.Seat = seat
            ShowMessage("Load or unload, then press Launch Platform.")
        Else
            ShowMessage("The platform is docked on the other bank!")
        End If
    End Sub

    Private Sub LaunchPlatform(sender As Object, e As EventArgs)
        If platMoving OrElse gameOver Then Return
        If RiderCount() = 0 Then
            ShowMessage("Somebody has to drive! Put at least 1 character on the platform.")
            Return
        End If
        platTargetY = If(platSide = 0, BottomDockY, TopDockY)
        platMoving = True
        ShowMessage("Hovering across the lava...")
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
    '  RULES
    ' =================================================================
    Private Function RiderCount() As Integer
        Dim n As Integer = 0
        For Each p As Piece In pieces
            If p.Zone = 2 Then n += 1
        Next
        Return n
    End Function

    Private Function OpenSeat() As Integer
        For s As Integer = 0 To 1
            Dim used As Boolean = False
            For Each p As Piece In pieces
                If p.Zone = 2 AndAlso p.Seat = s Then used = True
            Next
            If Not used Then Return s
        Next
        Return -1
    End Function

    ' Called when the platform finishes a trip
    Private Sub Docked()
        platSide = If(platTargetY = BottomDockY, 1, 0)
        trips += 1
        UpdateStats()

        Dim sciStart, gooStart, sciGoal, gooGoal As Integer
        For Each p As Piece In pieces
            ' riders count as standing on the bank the platform docked at
            Dim bank As Integer = If(p.Zone = 2, platSide, p.Zone)
            If bank = 0 Then
                If p.IsScientist Then sciStart += 1 Else gooStart += 1
            Else
                If p.IsScientist Then sciGoal += 1 Else gooGoal += 1
            End If
        Next

        If sciStart > 0 AndAlso gooStart > sciStart Then
            Dissolved(0)
        ElseIf sciGoal > 0 AndAlso gooGoal > sciGoal Then
            Dissolved(1)
        ElseIf sciGoal = 3 AndAlso gooGoal = 3 Then
            Won()
        Else
            ShowMessage("All safe. Unload, load the next pair, and keep going.")
        End If
    End Sub

    Private Sub Dissolved(bank As Integer)
        gameOver = True
        btnLaunch.Enabled = False
        tmrClock.Stop()
        For Each p As Piece In pieces
            Dim here As Integer = If(p.Zone = 2, platSide, p.Zone)
            If p.IsScientist AndAlso here = bank Then p.Box.Visible = False
        Next
        Dim name As String = If(bank = 0, "start", "goal")
        ShowMessage("Too much Goo on the " & name & " bank - the scientists dissolved! Press Reset.")
        lblMsg.ForeColor = Color.FromArgb(255, 130, 130)
    End Sub

    Private Sub Won()
        gameOver = True
        btnLaunch.Enabled = False
        tmrClock.Stop()
        Dim grade As String
        If trips <= BestTrips Then
            grade = "Perfect run!"
        ElseIf trips <= BestTrips + 4 Then
            grade = "Nice work!"
        Else
            grade = "Done! Can you beat " & trips & "?"
        End If
        ShowMessage("Everyone crossed in " & trips & " trips. " & grade)
        lblMsg.ForeColor = Color.FromArgb(150, 255, 150)
    End Sub

    Private Sub ShowMessage(txt As String)
        lblMsg.ForeColor = Color.White
        lblMsg.Text = txt
    End Sub

    Private Sub UpdateStats()
        lblStats.Text = "Trips: " & trips & vbCrLf &
                        "Time: " & (seconds \ 60).ToString("00") & ":" & (seconds Mod 60).ToString("00")
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
        hoverTick += 0.1

        ' the lava slowly glows brighter and darker
        Dim glow As Double = (Math.Sin(hoverTick * 0.6) + 1) / 2
        pbArena.BackColor = Color.FromArgb(175 + CInt(60 * glow), 45 + CInt(40 * glow), 10)

        ' the platform: slide toward the other bank, and hover side to side
        Dim y As Integer = pbPlatform.Top
        If platMoving Then
            Dim left As Integer = platTargetY - y
            If Math.Abs(left) <= 3 Then
                y = platTargetY
                platMoving = False
                pbPlatform.Top = y
                Docked()
            Else
                y += Math.Sign(left) * 3
                pbPlatform.Top = y
            End If
        End If
        pbPlatform.Left = PlatX + CInt(Math.Sin(hoverTick) * 4)

        ' the characters: decide each goal, then walk toward it
        PlanGoals()
        For Each p As Piece In pieces
            If p.Zone = 2 AndAlso platMoving Then
                p.Box.Location = p.Goal          ' riders travel with the platform
            Else
                WalkToward(p, 8)
            End If
        Next
    End Sub

    ' Sets every character's goal Location
    Private Sub PlanGoals()
        Dim startSlot As Integer = 0
        Dim goalSlot As Integer = 0
        For Each p As Piece In pieces
            Select Case p.Zone
                Case 0      ' start bank (top): a row from the left
                    p.Goal = New Point(20 + startSlot * 56, 40)
                    startSlot += 1
                Case 1      ' goal bank (bottom)
                    p.Goal = New Point(20 + goalSlot * 56, GoalBankY + 40)
                    goalSlot += 1
                Case Else   ' on the platform
                    p.Goal = New Point(pbPlatform.Left + 10 + p.Seat * 80, pbPlatform.Top + 3)
            End Select
        Next
    End Sub

    ' Moves a PictureBox's Location up to `speed` pixels toward its goal
    Private Sub WalkToward(p As Piece, speed As Integer)
        Dim here As Point = p.Box.Location
        Dim dx As Integer = p.Goal.X - here.X
        Dim dy As Integer = p.Goal.Y - here.Y
        Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)
        If dist <= speed Then
            p.Box.Location = p.Goal
        Else
            p.Box.Location = New Point(here.X + CInt(dx / dist * speed), here.Y + CInt(dy / dist * speed))
        End If
    End Sub

End Class
