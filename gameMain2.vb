Public Class gameMain2
#Region "Variables"
    Private Const brickWidth As Integer = 30
    Private Const brickHeight As Integer = 23
    Private Const brickRows As Integer = 8 - 1
    Private Const brickColumns As Integer = 12 - 1
    Dim life As Integer = 3
    Dim score(20) As Integer
    Dim arraycount As Integer = 0

    Private brickArray(brickRows, brickColumns) As Rectangle
    Private isBrickEnabled(brickRows, brickColumns) As Boolean

    Private gamePaddle As Rectangle = New Rectangle(300, 430, 50, 10)

    Private gameBall As Rectangle = New Rectangle(gamePaddle.X + 25 / 2 _
    - (16 / 2), 430 - 16, 16, 16)
    Private isBallGlued As Boolean = True

    Dim speed As Single = 14
    Dim xVel As Single = Math.Cos(speed) * speed
    Dim yVel As Single = Math.Sin(speed) * speed
#End Region

#Region "Load Game"
    Private Sub gameMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Refresh()
        loadBricks()
    End Sub

#End Region

#Region "Paint Event"
    Private Sub gameMain_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        ' Loop through all enabled bricks and display them.
        For row As Integer = 0 To brickRows
            For column As Integer = 0 To brickColumns
                If isBrickEnabled(row, column) Then _
                    e.Graphics.FillRectangle(Brushes.LightBlue, brickArray(row, column))
            Next
        Next

        ' Show the ball and the paddle.
        e.Graphics.FillEllipse(Brushes.White, gameBall)
        e.Graphics.FillRectangle(Brushes.Black, gamePaddle)
    End Sub
#End Region

#Region "Game Timer"
    Private Sub tmrGame_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGame.Tick
        If Not isBallGlued Then _
        gameBall.Location = New Point(gameBall.X + xVel, gameBall.Y + yVel)

        ' Check for top wall.
        If gameBall.Location.Y < 0 Then
            gameBall.Location = New Point(gameBall.Location.X, 0)
            yVel = -yVel
        End If

        ' Check for bottom wall (restart)
        If gameBall.Location.Y - gameBall.Height > Me.Height Then

            isBallGlued = True
            life = -1
            gameBall.Location = New Point(gamePaddle.X + 72 / 2 _
            - (gameBall.Width / 2), 432 - 16)

            If life = -1 Then MsgBox("You Lost")
            Me.Close()
        End If


        ' Check for left wall.
        If gameBall.Location.X < 0 Then
            gameBall.Location = New Point(0, gameBall.Location.Y)
            xVel = -xVel
        End If

        ' Check for right wall.
        If gameBall.Location.X + gameBall.Width > Me.Width Then
            gameBall.Location = New Point(Me.Width - gameBall.Width, _
            gameBall.Location.Y)
            xVel = -xVel
        End If

        ' Check for paddle.
        If gameBall.IntersectsWith(gamePaddle) Then
            gameBall.Location = New Point(gameBall.X, gamePaddle.Y - gameBall.Height)
            yVel = -yVel
        End If


        ' Check for blocks
        For rows As Integer = 0 To brickRows
            For columns As Integer = 0 To brickColumns
                If Not isBrickEnabled(rows, columns) Then Continue For
                If gameBall.IntersectsWith(brickArray(rows, columns)) Then
                    isBrickEnabled(rows, columns) = False
                    score(arraycount) += 100
                    TextBox1.Text = 2400 + score(arraycount)
                    If gameBall.X + 10 < brickArray(rows, columns).X Or _
                    gameBall.X > brickArray(rows, columns).X + brickArray(rows, columns).Width _
                     Then
                        xVel = -xVel
                    Else
                        yVel = -yVel
                    End If
                End If
            Next
        Next

        ' Check for end of game.
        If getBrickCount() = 0 Then
            tmrGame.Stop()
            Windows.Forms.Cursor.Show()
            MessageBox.Show("Well Done, You Won")
            Me.Close()
        End If
        Me.Refresh()


    End Sub

#End Region

#Region "Set Up Bricks"
    Sub loadBricks()
        Dim xOffset As Integer = 75, yOffset As Integer = 100
        For row As Integer = 0 To brickRows
            For column As Integer = 0 To brickColumns

                brickArray(row, column) = New Rectangle( _
                xOffset, yOffset, brickWidth, brickHeight)

                xOffset += brickWidth + 10
                isBrickEnabled(row, column) = True
            Next
            yOffset += brickHeight + 10
            xOffset = 75
        Next
    End Sub
#End Region

#Region "Get Amount of Bricks"
    Function getBrickCount() As Integer
        Dim Count As Integer = 0
        For Each brick As Boolean In isBrickEnabled
            If brick = True Then Count += 1
        Next
        Return Count
    End Function
#End Region

#Region "Move Paddle According to Mouse Position"
    Private Sub gameMain_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If e.X > 0 And e.X < Me.Width - 72 Then _
        gamePaddle.Location = New Point(e.X, gamePaddle.Y)

        If isBallGlued Then
            gameBall.Location = New Point(gamePaddle.X + 72 / 2 - (gameBall.Width / 2), _
            432 - 16)
        End If
    End Sub
#End Region

#Region "Launch Ball"

    Private Sub gameMain_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ' Launch the ball.
            If isBallGlued Then
                isBallGlued = False
            End If
        End If
    End Sub
#End Region

#Region "Quit on Escape Key Press and Pause on P Key Press"
    Private Sub gameMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        ' Exit
        If e.KeyCode = Keys.Escape Then Application.Exit()

        ' Toggle Paused
        If e.KeyCode = Keys.P Then _
            If tmrGame.Enabled Then tmrGame.Stop() _
            Else tmrGame.Start()
    End Sub
#End Region

End Class
