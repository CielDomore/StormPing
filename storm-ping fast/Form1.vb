Imports System.Drawing.Drawing2D

Public Class Form1

    ' ===== CONTROLES =====
    Dim PanelLogin As Panel
    Dim PanelHeader As Panel
    Dim PanelGradient As Panel
    Dim lblTitulo As Label
    Dim lblSubtitulo As Label
    Dim lblUser As Label
    Dim lblPass As Label
    Dim txtUser As TextBox
    Dim txtPass As TextBox
    Dim btnLogin As Button
    Dim btnFechar As Button
    Dim btnMinimizar As Button
    Dim lblMsg As Label
    Dim lblIcon As Label
    Dim PanelUserBox As Panel
    Dim PanelPassBox As Panel
    Dim lblUserIcon As Label
    Dim lblPassIcon As Label
    Dim chkLembrar As CheckBox
    Dim lblEsqueci As Label

    ' ===== TIMERS =====
    Dim TimerSlide As New Timer
    Dim TimerFade As New Timer
    Dim TimerPulse As New Timer
    Dim TimerEntrada As New Timer
    Dim TimerGlow As New Timer

    Dim fadeOut As Boolean = False
    Dim pulseUp As Boolean = True
    Dim pulseValue As Integer = 0
    Dim entradaStep As Integer = 0
    Dim glowAngle As Integer = 0

    ' ===== CORES DO TEMA =====
    Dim CorFundo As Color = Color.FromArgb(15, 15, 25)
    Dim CorPainel As Color = Color.FromArgb(25, 25, 40)
    Dim CorPainelClaro As Color = Color.FromArgb(35, 35, 55)
    Dim CorPrimaria As Color = Color.FromArgb(99, 102, 241)
    Dim CorPrimariaHover As Color = Color.FromArgb(129, 132, 255)
    Dim CorSecundaria As Color = Color.FromArgb(236, 72, 153)
    Dim CorTexto As Color = Color.FromArgb(248, 250, 252)
    Dim CorTextoSec As Color = Color.FromArgb(148, 163, 184)
    Dim CorInput As Color = Color.FromArgb(30, 30, 50)
    Dim CorBorda As Color = Color.FromArgb(55, 55, 80)
    Dim CorSucesso As Color = Color.FromArgb(34, 197, 94)
    Dim CorErro As Color = Color.FromArgb(239, 68, 68)

    Private isDragging As Boolean = False
    Private dragStart As Point

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        ' ===== FORM PRINCIPAL =====
        Me.Size = New Size(1000, 600)
        Me.BackColor = CorFundo
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.DoubleBuffered = True
        Me.Opacity = 0

        ' ===== PAINEL GRADIENTE LATERAL =====
        PanelGradient = New Panel
        PanelGradient.Size = New Size(420, Me.Height)
        PanelGradient.Left = 0
        PanelGradient.Top = 0
        Me.Controls.Add(PanelGradient)
        AddHandler PanelGradient.Paint, AddressOf PanelGradient_Paint
        AddHandler PanelGradient.MouseDown, AddressOf Form_MouseDown
        AddHandler PanelGradient.MouseMove, AddressOf Form_MouseMove
        AddHandler PanelGradient.MouseUp, AddressOf Form_MouseUp

        ' ===== ICONE GRANDE =====
        lblIcon = New Label
        lblIcon.Text = "⚡"
        lblIcon.Font = New Font("Segoe UI", 72, FontStyle.Regular)
        lblIcon.ForeColor = Color.White
        lblIcon.AutoSize = True
        lblIcon.BackColor = Color.Transparent
        lblIcon.Left = 150
        lblIcon.Top = 180
        PanelGradient.Controls.Add(lblIcon)

        ' ===== TEXTO LATERAL =====
        Dim lblWelcome As New Label
        lblWelcome.Text = "STORM PING"
        lblWelcome.Font = New Font("Segoe UI", 28, FontStyle.Bold)
        lblWelcome.ForeColor = Color.White
        lblWelcome.AutoSize = True
        lblWelcome.BackColor = Color.Transparent
        lblWelcome.Left = 85
        lblWelcome.Top = 320
        PanelGradient.Controls.Add(lblWelcome)

        Dim lblDesc As New Label
        lblDesc.Text = "Sistema de Monitoramento" & vbCrLf & "Rápido e Eficiente"
        lblDesc.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        lblDesc.ForeColor = Color.FromArgb(200, 255, 255, 255)
        lblDesc.AutoSize = True
        lblDesc.BackColor = Color.Transparent
        lblDesc.Left = 110
        lblDesc.Top = 370
        lblDesc.TextAlign = ContentAlignment.MiddleCenter
        PanelGradient.Controls.Add(lblDesc)

        ' ===== PAINEL LOGIN PRINCIPAL =====
        PanelLogin = New Panel
        PanelLogin.Size = New Size(480, Me.Height)
        PanelLogin.BackColor = CorPainel
        PanelLogin.Left = 420
        PanelLogin.Top = 0
        Me.Controls.Add(PanelLogin)
        AddHandler PanelLogin.MouseDown, AddressOf Form_MouseDown
        AddHandler PanelLogin.MouseMove, AddressOf Form_MouseMove
        AddHandler PanelLogin.MouseUp, AddressOf Form_MouseUp

        ' ===== BOTOES CONTROLE =====
        btnFechar = New Button
        btnFechar.Text = "✕"
        btnFechar.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        btnFechar.Size = New Size(45, 35)
        btnFechar.Left = PanelLogin.Width - 50
        btnFechar.Top = 5
        btnFechar.FlatStyle = FlatStyle.Flat
        btnFechar.FlatAppearance.BorderSize = 0
        btnFechar.BackColor = Color.Transparent
        btnFechar.ForeColor = CorTextoSec
        btnFechar.Cursor = Cursors.Hand
        PanelLogin.Controls.Add(btnFechar)
        AddHandler btnFechar.Click, AddressOf BtnFechar_Click
        AddHandler btnFechar.MouseEnter, AddressOf BtnFechar_Enter
        AddHandler btnFechar.MouseLeave, AddressOf BtnFechar_Leave

        btnMinimizar = New Button
        btnMinimizar.Text = "─"
        btnMinimizar.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        btnMinimizar.Size = New Size(45, 35)
        btnMinimizar.Left = PanelLogin.Width - 95
        btnMinimizar.Top = 5
        btnMinimizar.FlatStyle = FlatStyle.Flat
        btnMinimizar.FlatAppearance.BorderSize = 0
        btnMinimizar.BackColor = Color.Transparent
        btnMinimizar.ForeColor = CorTextoSec
        btnMinimizar.Cursor = Cursors.Hand
        PanelLogin.Controls.Add(btnMinimizar)
        AddHandler btnMinimizar.Click, AddressOf BtnMinimizar_Click
        AddHandler btnMinimizar.MouseEnter, AddressOf BtnMin_Enter
        AddHandler btnMinimizar.MouseLeave, AddressOf BtnMin_Leave

        ' ===== TITULO LOGIN =====
        lblTitulo = New Label
        lblTitulo.Text = "Bem-vindo!"
        lblTitulo.Font = New Font("Segoe UI", 26, FontStyle.Bold)
        lblTitulo.ForeColor = CorTexto
        lblTitulo.AutoSize = True
        lblTitulo.Left = 60
        lblTitulo.Top = 100
        PanelLogin.Controls.Add(lblTitulo)

        lblSubtitulo = New Label
        lblSubtitulo.Text = "Faça login para continuar"
        lblSubtitulo.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        lblSubtitulo.ForeColor = CorTextoSec
        lblSubtitulo.AutoSize = True
        lblSubtitulo.Left = 60
        lblSubtitulo.Top = 145
        PanelLogin.Controls.Add(lblSubtitulo)

        ' ===== CAMPO USUARIO =====
        lblUser = New Label
        lblUser.Text = "USUÁRIO"
        lblUser.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        lblUser.ForeColor = CorTextoSec
        lblUser.AutoSize = True
        lblUser.Left = 60
        lblUser.Top = 200
        PanelLogin.Controls.Add(lblUser)

        PanelUserBox = New Panel
        PanelUserBox.Size = New Size(360, 50)
        PanelUserBox.Left = 60
        PanelUserBox.Top = 225
        PanelUserBox.BackColor = CorInput
        PanelLogin.Controls.Add(PanelUserBox)
        AddHandler PanelUserBox.Paint, AddressOf PanelBox_Paint

        lblUserIcon = New Label
        lblUserIcon.Text = "👤"
        lblUserIcon.Font = New Font("Segoe UI", 14, FontStyle.Regular)
        lblUserIcon.ForeColor = CorTextoSec
        lblUserIcon.AutoSize = True
        lblUserIcon.Left = 15
        lblUserIcon.Top = 12
        PanelUserBox.Controls.Add(lblUserIcon)

        txtUser = New TextBox
        txtUser.Left = 50
        txtUser.Top = 14
        txtUser.Width = 290
        txtUser.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        txtUser.BackColor = CorInput
        txtUser.ForeColor = CorTexto
        txtUser.BorderStyle = BorderStyle.None
        PanelUserBox.Controls.Add(txtUser)
        AddHandler txtUser.Enter, AddressOf TxtUser_Enter
        AddHandler txtUser.Leave, AddressOf TxtUser_Leave

        ' ===== CAMPO SENHA =====
        lblPass = New Label
        lblPass.Text = "SENHA"
        lblPass.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        lblPass.ForeColor = CorTextoSec
        lblPass.AutoSize = True
        lblPass.Left = 60
        lblPass.Top = 295
        PanelLogin.Controls.Add(lblPass)

        PanelPassBox = New Panel
        PanelPassBox.Size = New Size(360, 50)
        PanelPassBox.Left = 60
        PanelPassBox.Top = 320
        PanelPassBox.BackColor = CorInput
        PanelLogin.Controls.Add(PanelPassBox)
        AddHandler PanelPassBox.Paint, AddressOf PanelBox_Paint

        lblPassIcon = New Label
        lblPassIcon.Text = "🔒"
        lblPassIcon.Font = New Font("Segoe UI", 14, FontStyle.Regular)
        lblPassIcon.ForeColor = CorTextoSec
        lblPassIcon.AutoSize = True
        lblPassIcon.Left = 15
        lblPassIcon.Top = 12
        PanelPassBox.Controls.Add(lblPassIcon)

        txtPass = New TextBox
        txtPass.Left = 50
        txtPass.Top = 14
        txtPass.Width = 290
        txtPass.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        txtPass.BackColor = CorInput
        txtPass.ForeColor = CorTexto
        txtPass.BorderStyle = BorderStyle.None
        txtPass.PasswordChar = "●"c
        PanelPassBox.Controls.Add(txtPass)
        AddHandler txtPass.Enter, AddressOf TxtPass_Enter
        AddHandler txtPass.Leave, AddressOf TxtPass_Leave

        ' ===== OPCOES =====
        chkLembrar = New CheckBox
        chkLembrar.Text = "Lembrar-me"
        chkLembrar.Font = New Font("Segoe UI", 9, FontStyle.Regular)
        chkLembrar.ForeColor = CorTextoSec
        chkLembrar.Left = 60
        chkLembrar.Top = 390
        chkLembrar.AutoSize = True
        PanelLogin.Controls.Add(chkLembrar)

        lblEsqueci = New Label
        lblEsqueci.Text = "Esqueceu a senha?"
        lblEsqueci.Font = New Font("Segoe UI", 9, FontStyle.Underline)
        lblEsqueci.ForeColor = CorPrimaria
        lblEsqueci.AutoSize = True
        lblEsqueci.Left = 305
        lblEsqueci.Top = 392
        lblEsqueci.Cursor = Cursors.Hand
        PanelLogin.Controls.Add(lblEsqueci)
        AddHandler lblEsqueci.MouseEnter, AddressOf LblEsqueci_Enter
        AddHandler lblEsqueci.MouseLeave, AddressOf LblEsqueci_Leave

        ' ===== BOTAO LOGIN =====
        btnLogin = New Button
        btnLogin.Text = "ENTRAR"
        btnLogin.Left = 60
        btnLogin.Top = 440
        btnLogin.Width = 360
        btnLogin.Height = 55
        btnLogin.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.BackColor = CorPrimaria
        btnLogin.ForeColor = Color.White
        btnLogin.Cursor = Cursors.Hand
        PanelLogin.Controls.Add(btnLogin)
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click
        AddHandler btnLogin.MouseEnter, AddressOf BtnLogin_Enter
        AddHandler btnLogin.MouseLeave, AddressOf BtnLogin_Leave
        AddHandler btnLogin.Paint, AddressOf BtnLogin_Paint

        ' ===== MENSAGEM =====
        lblMsg = New Label
        lblMsg.AutoSize = True
        lblMsg.Font = New Font("Segoe UI", 10, FontStyle.Regular)
        lblMsg.ForeColor = CorSucesso
        lblMsg.Visible = False
        lblMsg.Left = 60
        lblMsg.Top = 510
        PanelLogin.Controls.Add(lblMsg)

        ' ===== RODAPE =====
        Dim lblRodape As New Label
        lblRodape.Text = "© 2025 Storm Ping - Todos os direitos reservados"
        lblRodape.Font = New Font("Segoe UI", 8, FontStyle.Regular)
        lblRodape.ForeColor = CorTextoSec
        lblRodape.AutoSize = True
        lblRodape.Left = 115
        lblRodape.Top = 560
        PanelLogin.Controls.Add(lblRodape)

        ' ===== CONFIGURAR TIMERS =====
        TimerSlide.Interval = 12
        AddHandler TimerSlide.Tick, AddressOf TimerSlide_Tick

        TimerFade.Interval = 40
        AddHandler TimerFade.Tick, AddressOf TimerFade_Tick

        TimerPulse.Interval = 50
        AddHandler TimerPulse.Tick, AddressOf TimerPulse_Tick

        TimerEntrada.Interval = 20
        AddHandler TimerEntrada.Tick, AddressOf TimerEntrada_Tick

        TimerGlow.Interval = 50
        AddHandler TimerGlow.Tick, AddressOf TimerGlow_Tick

        ' ===== INICIAR ANIMACOES =====
        TimerEntrada.Start()
        ' TimerGlow removido para evitar piscamento

    End Sub

    ' ===== PAINT GRADIENTE =====
    Private Sub PanelGradient_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim rect As New Rectangle(0, 0, PanelGradient.Width, PanelGradient.Height)
        Dim brush As New LinearGradientBrush(rect, CorPrimaria, CorSecundaria, 45)
        e.Graphics.FillRectangle(brush, rect)
        brush.Dispose()
    End Sub

    ' ===== PAINT BOX INPUT =====
    Private Sub PanelBox_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim pnl As Panel = DirectCast(sender, Panel)
        Dim rect As New Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1)
        Dim pen As New Pen(CorBorda, 2)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim path As GraphicsPath = RoundedRect(rect, 8)
        e.Graphics.DrawPath(pen, path)
        pen.Dispose()
        path.Dispose()
    End Sub

    ' ===== PAINT BOTAO LOGIN =====
    Private Sub BtnLogin_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim btn As Button = DirectCast(sender, Button)
        Dim rect As New Rectangle(0, 0, btn.Width, btn.Height)
        Dim path As GraphicsPath = RoundedRect(rect, 12)
        btn.Region = New Region(path)
        path.Dispose()
    End Sub

    ' ===== FUNCAO RETANGULO ARREDONDADO =====
    Function RoundedRect(ByVal bounds As Rectangle, ByVal radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath
        Dim diameter As Integer = radius * 2
        Dim arc As New Rectangle(bounds.Location, New Size(diameter, diameter))

        path.AddArc(arc, 180, 90)
        arc.X = bounds.Right - diameter
        path.AddArc(arc, 270, 90)
        arc.Y = bounds.Bottom - diameter
        path.AddArc(arc, 0, 90)
        arc.X = bounds.Left
        path.AddArc(arc, 90, 90)
        path.CloseFigure()

        Return path
    End Function

    ' ===== EVENTOS HOVER =====
    Private Sub BtnLogin_Enter(ByVal sender As Object, ByVal e As EventArgs)
        btnLogin.BackColor = CorPrimariaHover
        TimerPulse.Start()
    End Sub

    Private Sub BtnLogin_Leave(ByVal sender As Object, ByVal e As EventArgs)
        btnLogin.BackColor = CorPrimaria
        TimerPulse.Stop()
    End Sub

    Private Sub BtnFechar_Enter(ByVal sender As Object, ByVal e As EventArgs)
        btnFechar.BackColor = CorErro
        btnFechar.ForeColor = Color.White
    End Sub

    Private Sub BtnFechar_Leave(ByVal sender As Object, ByVal e As EventArgs)
        btnFechar.BackColor = Color.Transparent
        btnFechar.ForeColor = CorTextoSec
    End Sub

    Private Sub BtnMin_Enter(ByVal sender As Object, ByVal e As EventArgs)
        btnMinimizar.BackColor = CorPainelClaro
    End Sub

    Private Sub BtnMin_Leave(ByVal sender As Object, ByVal e As EventArgs)
        btnMinimizar.BackColor = Color.Transparent
    End Sub

    Private Sub LblEsqueci_Enter(ByVal sender As Object, ByVal e As EventArgs)
        lblEsqueci.ForeColor = CorPrimariaHover
    End Sub

    Private Sub LblEsqueci_Leave(ByVal sender As Object, ByVal e As EventArgs)
        lblEsqueci.ForeColor = CorPrimaria
    End Sub

    Private Sub TxtUser_Enter(ByVal sender As Object, ByVal e As EventArgs)
        PanelUserBox.BackColor = Color.FromArgb(40, 40, 65)
        PanelUserBox.Invalidate()
    End Sub

    Private Sub TxtUser_Leave(ByVal sender As Object, ByVal e As EventArgs)
        PanelUserBox.BackColor = CorInput
        txtUser.BackColor = CorInput
        PanelUserBox.Invalidate()
    End Sub

    Private Sub TxtPass_Enter(ByVal sender As Object, ByVal e As EventArgs)
        PanelPassBox.BackColor = Color.FromArgb(40, 40, 65)
        PanelPassBox.Invalidate()
    End Sub

    Private Sub TxtPass_Leave(ByVal sender As Object, ByVal e As EventArgs)
        PanelPassBox.BackColor = CorInput
        txtPass.BackColor = CorInput
        PanelPassBox.Invalidate()
    End Sub

    ' ===== DRAG FORM =====
    Private Sub Form_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = True
            dragStart = e.Location
        End If
    End Sub

    Private Sub Form_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
        If isDragging Then
            Me.Left += e.X - dragStart.X
            Me.Top += e.Y - dragStart.Y
        End If
    End Sub

    Private Sub Form_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        isDragging = False
    End Sub

    ' ===== BOTOES CONTROLE =====
    Private Sub BtnFechar_Click(ByVal sender As Object, ByVal e As EventArgs)
        Application.Exit()
    End Sub

    Private Sub BtnMinimizar_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    ' ===== CLICK LOGIN =====
    Private Sub BtnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        If txtUser.Text = "storm" And txtPass.Text = "central" Then
            ShowMsg("✓ Login realizado com sucesso!", True)
            ' Abre Form2 após login bem sucedido
            Dim f2 As New Form2
            f2.Show()
            Me.Hide()
        Else
            ShowMsg("✗ Usuário ou senha inválidos!", False)
            ' Efeito shake
            ShakeForm()
        End If
    End Sub

    ' ===== VARIAVEIS SHAKE =====
    Dim TimerShake As New Timer
    Dim shakeCount As Integer = 0
    Dim shakeOriginal As Point

    ' ===== EFEITO SHAKE =====
    Sub ShakeForm()
        shakeOriginal = Me.Location
        shakeCount = 0
        TimerShake.Interval = 30
        AddHandler TimerShake.Tick, AddressOf TimerShake_Tick
        TimerShake.Start()
    End Sub

    Private Sub TimerShake_Tick(ByVal sender As Object, ByVal e As EventArgs)
        shakeCount += 1
        If shakeCount Mod 2 = 0 Then
            Me.Left = shakeOriginal.X - 8
        Else
            Me.Left = shakeOriginal.X + 8
        End If
        If shakeCount >= 8 Then
            Me.Location = shakeOriginal
            TimerShake.Stop()
            RemoveHandler TimerShake.Tick, AddressOf TimerShake_Tick
        End If
    End Sub

    ' ===== MOSTRAR MENSAGEM =====
    Sub ShowMsg(ByVal texto As String, ByVal sucesso As Boolean)
        lblMsg.Text = texto
        lblMsg.ForeColor = If(sucesso, CorSucesso, CorErro)
        lblMsg.Visible = True
        fadeOut = False
        TimerFade.Start()
    End Sub

    ' ===== ANIMAÇÃO SLIDE =====
    Private Sub TimerSlide_Tick(ByVal sender As Object, ByVal e As EventArgs)
        lblMsg.Top -= 3
        If lblMsg.Top <= 510 Then
            TimerSlide.Stop()
            TimerFade.Start()
        End If
    End Sub

    ' ===== FADE =====
    Private Sub TimerFade_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Static fadeCount As Integer = 0
        fadeCount += 1
        If fadeCount > 50 Then
            lblMsg.Visible = False
            TimerFade.Stop()
            fadeCount = 0
        End If
    End Sub

    ' ===== PULSE BUTTON =====
    Private Sub TimerPulse_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If pulseUp Then
            pulseValue += 3
            If pulseValue >= 15 Then pulseUp = False
        Else
            pulseValue -= 3
            If pulseValue <= 0 Then pulseUp = True
        End If
        ' Garantir que os valores RGB não ultrapassem 255
        Dim r As Integer = Math.Min(255, CorPrimaria.R + pulseValue)
        Dim g As Integer = Math.Min(255, CorPrimaria.G + pulseValue)
        Dim b As Integer = Math.Min(255, CorPrimaria.B + pulseValue)
        btnLogin.BackColor = Color.FromArgb(r, g, b)
    End Sub

    ' ===== ANIMACAO ENTRADA =====
    Private Sub TimerEntrada_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Me.Opacity += 0.05
        If Me.Opacity >= 1 Then
            Me.Opacity = 1
            TimerEntrada.Stop()
        End If
    End Sub

    ' ===== GLOW ANIMATION =====
    Private Sub TimerGlow_Tick(ByVal sender As Object, ByVal e As EventArgs)
        ' Desabilitado para evitar piscamento
        TimerGlow.Stop()
    End Sub

End Class
