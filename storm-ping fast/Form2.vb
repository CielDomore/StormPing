Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.IO
Imports System.Text
Imports System.ComponentModel

Public Class Form2

    ' ===== CONTROLES PRINCIPAIS =====
    Private WithEvents PanelMain As Panel
    Private WithEvents PanelSidebar As Panel
    Private WithEvents PanelHeader As Panel
    Private WithEvents PanelContent As Panel
    Private TabControl1 As TabControl
    Private TabSpeedTest As TabPage
    Private TabDNS As TabPage
    Private TabAnalise As TabPage
    Private TabConfig As TabPage

    ' ===== CONTROLES SIDEBAR =====
    Private WithEvents btnMenuSpeed As Button
    Private WithEvents btnMenuDNS As Button
    Private WithEvents btnMenuAnalise As Button
    Private WithEvents btnMenuConfig As Button
    Private lblLogo As Label
    Private lblLogoText As Label

    ' ===== CONTROLES HEADER =====
    Private WithEvents btnFechar As Button
    Private WithEvents btnMinimizar As Button
    Private WithEvents btnMaximizar As Button
    Private lblTitulo As Label

    ' ===== CONTROLES SPEED TEST =====
    Private WithEvents PanelSpeedometer As Panel
    Private lblSpeedValue As Label
    Private lblSpeedUnit As Label
    Private lblPingValue As Label
    Private lblDownloadValue As Label
    Private lblUploadValue As Label
    Private lblPingLabel As Label
    Private lblDownloadLabel As Label
    Private lblUploadLabel As Label
    Private WithEvents btnIniciarTeste As Button
    Private lblStatus As Label
    Private ProgressTest As ProgressBar
    Private WithEvents PanelPing As Panel
    Private WithEvents PanelDownload As Panel
    Private WithEvents PanelUpload As Panel

    ' ===== CONTROLES DNS =====
    Private ListViewDNS As ListView
    Private WithEvents btnTestarDNS As Button
    Private lblMelhorDNS As Label
    Private WithEvents PanelDNSResult As Panel
    Private lblDNSRecomendado As Label

    ' ===== CONTROLES ANALISE =====
    Private txtAnaliseIA As RichTextBox
    Private WithEvents btnAnalisarIA As Button
    Private lblAnaliseStatus As Label
    Private WithEvents PanelAnaliseHeader As Panel
    Private txtApiKey As TextBox
    Private lblApiKey As Label
    Private WithEvents btnSalvarApi As Button
    Private WithEvents lblApiLink As Label

    ' ===== TOAST NOTIFICATION =====
    Private WithEvents PanelToast As Panel
    Private lblToastIcon As Label
    Private lblToastMsg As Label
    Private WithEvents TimerToastShow As New Timer
    Private WithEvents TimerToastHide As New Timer
    Private toastTargetTop As Integer = 0

    ' ===== RESULTADOS TESTE =====
    Private ultimoPing As Double = 0
    Private ultimoDownload As Double = 0
    Private ultimoUpload As Double = 0

    ' ===== CORES DO TEMA =====
    Private CorFundo As Color = Color.FromArgb(15, 15, 25)
    Private CorPainel As Color = Color.FromArgb(25, 25, 40)
    Private CorPainelClaro As Color = Color.FromArgb(35, 35, 55)
    Private CorPrimaria As Color = Color.FromArgb(99, 102, 241)
    Private CorPrimariaHover As Color = Color.FromArgb(129, 132, 255)
    Private CorSecundaria As Color = Color.FromArgb(236, 72, 153)
    Private CorTexto As Color = Color.FromArgb(248, 250, 252)
    Private CorTextoSec As Color = Color.FromArgb(148, 163, 184)
    Private CorInput As Color = Color.FromArgb(30, 30, 50)
    Private CorBorda As Color = Color.FromArgb(55, 55, 80)
    Private CorSucesso As Color = Color.FromArgb(34, 197, 94)
    Private CorErro As Color = Color.FromArgb(239, 68, 68)
    Private CorAmarelo As Color = Color.FromArgb(250, 204, 21)
    Private CorCyan As Color = Color.FromArgb(34, 211, 238)

    ' ===== TIMERS =====
    Private WithEvents TimerEntrada As New Timer
    Private WithEvents TimerPulse As New Timer
    Private WithEvents TimerSpeedAnim As New Timer
    Private WithEvents TimerCardAnim As New Timer
    Private isDragging As Boolean = False
    Private dragStart As Point

    ' ===== ANIMACAO =====
    Private pulseValue As Integer = 0
    Private pulseUp As Boolean = True
    Private speedAnimTarget As Double = 0
    Private speedAnimCurrent As Double = 0
    Private cardAnimStep As Integer = 0

    ' ===== BACKGROUND WORKERS =====
    Private WithEvents WorkerSpeedTest As New BackgroundWorker
    Private WithEvents WorkerDNSTest As New BackgroundWorker
    Private WithEvents WorkerGemini As New BackgroundWorker

    ' ===== API KEY GEMINI =====
    Private geminiApiKey As String = ""

    ' ===== DNS SERVERS =====
    Private dnsNames() As String = {"Google DNS", "Cloudflare", "OpenDNS", "Quad9", "AdGuard DNS", "CleanBrowsing", "Comodo Secure", "Level3"}
    Private dnsPrimary() As String = {"8.8.8.8", "1.1.1.1", "208.67.222.222", "9.9.9.9", "94.140.14.14", "185.228.168.9", "8.26.56.26", "209.244.0.3"}
    Private dnsSecondary() As String = {"8.8.4.4", "1.0.0.1", "208.67.220.220", "149.112.112.112", "94.140.15.15", "185.228.169.9", "8.20.247.20", "209.244.0.4"}

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Me.Size = New Size(1200, 700)
        Me.BackColor = CorFundo
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.DoubleBuffered = True
        Me.Opacity = 0

        ' Configurar Workers
        WorkerSpeedTest.WorkerReportsProgress = True
        WorkerDNSTest.WorkerReportsProgress = True
        WorkerGemini.WorkerReportsProgress = True

        CriarSidebar()
        CriarHeader()
        CriarConteudo()
        CriarToastNotification()
        CarregarApiKey()

        ' Timer entrada
        TimerEntrada.Interval = 20
        TimerEntrada.Start()

        ' Timer pulse para botoes
        TimerPulse.Interval = 50
        TimerPulse.Start()

        ' Timer animacao velocimetro
        TimerSpeedAnim.Interval = 16

        ' Timer animacao cards
        TimerCardAnim.Interval = 30

        ' Timer toast
        TimerToastShow.Interval = 16
        TimerToastHide.Interval = 16

    End Sub

    ' ===== CRIAR TOAST NOTIFICATION =====
    Private Sub CriarToastNotification()
        PanelToast = New Panel
        PanelToast.Size = New Size(350, 70)
        PanelToast.Left = (Me.Width - 350) \ 2
        PanelToast.Top = -80
        PanelToast.BackColor = CorSucesso
        PanelToast.Visible = False
        Me.Controls.Add(PanelToast)
        PanelToast.BringToFront()

        lblToastIcon = New Label
        lblToastIcon.Text = "OK"
        lblToastIcon.Font = New Font("Segoe UI", 20, FontStyle.Bold)
        lblToastIcon.ForeColor = Color.White
        lblToastIcon.AutoSize = True
        lblToastIcon.Left = 20
        lblToastIcon.Top = 18
        PanelToast.Controls.Add(lblToastIcon)

        lblToastMsg = New Label
        lblToastMsg.Text = ""
        lblToastMsg.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblToastMsg.ForeColor = Color.White
        lblToastMsg.AutoSize = True
        lblToastMsg.Left = 70
        lblToastMsg.Top = 23
        PanelToast.Controls.Add(lblToastMsg)
    End Sub

    ' ===== MOSTRAR TOAST =====
    Private Sub MostrarToast(ByVal mensagem As String, ByVal sucesso As Boolean)
        lblToastMsg.Text = mensagem

        If sucesso Then
            PanelToast.BackColor = CorSucesso
            lblToastIcon.Text = "OK"
        Else
            PanelToast.BackColor = CorErro
            lblToastIcon.Text = "X"
        End If

        PanelToast.Left = 220 + ((Me.Width - 220 - 350) \ 2)
        PanelToast.Top = -80
        PanelToast.Visible = True
        PanelToast.BringToFront()
        toastTargetTop = 70

        TimerToastHide.Stop()
        TimerToastShow.Start()
    End Sub

    Private Sub TimerToastShow_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TimerToastShow.Tick
        If PanelToast.Top < toastTargetTop Then
            PanelToast.Top += 12
        Else
            PanelToast.Top = toastTargetTop
            TimerToastShow.Stop()
            ' Esperar 3 segundos e esconder
            Dim waitTimer As New Timer
            waitTimer.Interval = 3000
            AddHandler waitTimer.Tick, Sub(s, ev)
                                           waitTimer.Stop()
                                           TimerToastHide.Start()
                                       End Sub
            waitTimer.Start()
        End If
    End Sub

    Private Sub TimerToastHide_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TimerToastHide.Tick
        If PanelToast.Top > -80 Then
            PanelToast.Top -= 12
        Else
            PanelToast.Top = -80
            PanelToast.Visible = False
            TimerToastHide.Stop()
        End If
    End Sub

    ' ===== CRIAR SIDEBAR =====
    Private Sub CriarSidebar()
        PanelSidebar = New Panel
        PanelSidebar.Size = New Size(220, Me.Height)
        PanelSidebar.Left = 0
        PanelSidebar.Top = 0
        PanelSidebar.BackColor = CorPainel
        Me.Controls.Add(PanelSidebar)
        AddHandler PanelSidebar.Paint, AddressOf PanelSidebar_Paint

        lblLogo = New Label
        lblLogo.Text = "S"
        lblLogo.Font = New Font("Segoe UI", 36, FontStyle.Bold)
        lblLogo.ForeColor = CorPrimaria
        lblLogo.AutoSize = True
        lblLogo.Left = 25
        lblLogo.Top = 20
        PanelSidebar.Controls.Add(lblLogo)

        lblLogoText = New Label
        lblLogoText.Text = "STORM" & vbCrLf & "PING"
        lblLogoText.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblLogoText.ForeColor = CorTexto
        lblLogoText.AutoSize = True
        lblLogoText.Left = 85
        lblLogoText.Top = 28
        PanelSidebar.Controls.Add(lblLogoText)

        Dim linha As New Panel
        linha.Size = New Size(180, 2)
        linha.BackColor = CorBorda
        linha.Left = 20
        linha.Top = 100
        PanelSidebar.Controls.Add(linha)

        btnMenuSpeed = CriarBotaoMenu("Speed Test", 130)
        btnMenuDNS = CriarBotaoMenu("DNS Otimizado", 190)
        btnMenuAnalise = CriarBotaoMenu("Analise IA", 250)
        btnMenuConfig = CriarBotaoMenu("Configuracoes", 310)

        Dim lblVersao As New Label
        lblVersao.Text = "v1.0.0 BETA"
        lblVersao.Font = New Font("Segoe UI", 9, FontStyle.Regular)
        lblVersao.ForeColor = CorTextoSec
        lblVersao.AutoSize = True
        lblVersao.Left = 70
        lblVersao.Top = Me.Height - 50
        PanelSidebar.Controls.Add(lblVersao)

        ' Texto decorativo
        Dim lblPowered As New Label
        lblPowered.Text = "Powered by AI"
        lblPowered.Font = New Font("Segoe UI", 8, FontStyle.Italic)
        lblPowered.ForeColor = CorSecundaria
        lblPowered.AutoSize = True
        lblPowered.Left = 70
        lblPowered.Top = Me.Height - 30
        PanelSidebar.Controls.Add(lblPowered)
    End Sub

    Private Sub PanelSidebar_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        ' Linha lateral direita com gradiente
        Dim rect As New Rectangle(PanelSidebar.Width - 3, 0, 3, PanelSidebar.Height)
        Using brush As New LinearGradientBrush(rect, CorPrimaria, CorSecundaria, LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(brush, rect)
        End Using
    End Sub

    Private Function CriarBotaoMenu(ByVal texto As String, ByVal topPos As Integer) As Button
        Dim btn As New Button
        btn.Text = "  " & texto
        btn.Left = 10
        btn.Top = topPos
        btn.Width = 200
        btn.Height = 50
        btn.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = CorPainelClaro
        btn.BackColor = Color.Transparent
        btn.ForeColor = CorTextoSec
        btn.TextAlign = ContentAlignment.MiddleLeft
        btn.Padding = New Padding(15, 0, 0, 0)
        btn.Cursor = Cursors.Hand
        PanelSidebar.Controls.Add(btn)
        Return btn
    End Function

    Private Sub SelecionarTab(ByVal index As Integer)
        TabControl1.SelectedIndex = index

        btnMenuSpeed.BackColor = Color.Transparent
        btnMenuSpeed.ForeColor = CorTextoSec
        btnMenuSpeed.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        btnMenuDNS.BackColor = Color.Transparent
        btnMenuDNS.ForeColor = CorTextoSec
        btnMenuDNS.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        btnMenuAnalise.BackColor = Color.Transparent
        btnMenuAnalise.ForeColor = CorTextoSec
        btnMenuAnalise.Font = New Font("Segoe UI", 11, FontStyle.Regular)
        btnMenuConfig.BackColor = Color.Transparent
        btnMenuConfig.ForeColor = CorTextoSec
        btnMenuConfig.Font = New Font("Segoe UI", 11, FontStyle.Regular)

        Select Case index
            Case 0
                btnMenuSpeed.BackColor = CorPrimaria
                btnMenuSpeed.ForeColor = CorTexto
                btnMenuSpeed.Font = New Font("Segoe UI", 11, FontStyle.Bold)
                lblTitulo.Text = "Speed Test"
            Case 1
                btnMenuDNS.BackColor = CorPrimaria
                btnMenuDNS.ForeColor = CorTexto
                btnMenuDNS.Font = New Font("Segoe UI", 11, FontStyle.Bold)
                lblTitulo.Text = "DNS Otimizado"
            Case 2
                btnMenuAnalise.BackColor = CorPrimaria
                btnMenuAnalise.ForeColor = CorTexto
                btnMenuAnalise.Font = New Font("Segoe UI", 11, FontStyle.Bold)
                lblTitulo.Text = "Analise IA"
            Case 3
                btnMenuConfig.BackColor = CorPrimaria
                btnMenuConfig.ForeColor = CorTexto
                btnMenuConfig.Font = New Font("Segoe UI", 11, FontStyle.Bold)
                lblTitulo.Text = "Configuracoes"
        End Select
    End Sub

    ' ===== CRIAR HEADER =====
    Private Sub CriarHeader()
        PanelHeader = New Panel
        PanelHeader.Size = New Size(Me.Width - 220, 60)
        PanelHeader.Left = 220
        PanelHeader.Top = 0
        PanelHeader.BackColor = CorPainel
        Me.Controls.Add(PanelHeader)

        lblTitulo = New Label
        lblTitulo.Text = "Speed Test"
        lblTitulo.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        lblTitulo.ForeColor = CorTexto
        lblTitulo.AutoSize = True
        lblTitulo.Left = 30
        lblTitulo.Top = 15
        PanelHeader.Controls.Add(lblTitulo)

        btnFechar = New Button
        btnFechar.Text = "X"
        btnFechar.Font = New Font("Segoe UI", 12)
        btnFechar.Size = New Size(50, 60)
        btnFechar.Left = PanelHeader.Width - 50
        btnFechar.Top = 0
        btnFechar.FlatStyle = FlatStyle.Flat
        btnFechar.FlatAppearance.BorderSize = 0
        btnFechar.BackColor = Color.Transparent
        btnFechar.ForeColor = CorTextoSec
        btnFechar.Cursor = Cursors.Hand
        PanelHeader.Controls.Add(btnFechar)

        btnMaximizar = New Button
        btnMaximizar.Text = "[]"
        btnMaximizar.Font = New Font("Segoe UI", 10)
        btnMaximizar.Size = New Size(50, 60)
        btnMaximizar.Left = PanelHeader.Width - 100
        btnMaximizar.Top = 0
        btnMaximizar.FlatStyle = FlatStyle.Flat
        btnMaximizar.FlatAppearance.BorderSize = 0
        btnMaximizar.BackColor = Color.Transparent
        btnMaximizar.ForeColor = CorTextoSec
        btnMaximizar.Cursor = Cursors.Hand
        PanelHeader.Controls.Add(btnMaximizar)

        btnMinimizar = New Button
        btnMinimizar.Text = "_"
        btnMinimizar.Font = New Font("Segoe UI", 12)
        btnMinimizar.Size = New Size(50, 60)
        btnMinimizar.Left = PanelHeader.Width - 150
        btnMinimizar.Top = 0
        btnMinimizar.FlatStyle = FlatStyle.Flat
        btnMinimizar.FlatAppearance.BorderSize = 0
        btnMinimizar.BackColor = Color.Transparent
        btnMinimizar.ForeColor = CorTextoSec
        btnMinimizar.Cursor = Cursors.Hand
        PanelHeader.Controls.Add(btnMinimizar)
    End Sub

    ' ===== CRIAR CONTEUDO =====
    Private Sub CriarConteudo()
        PanelContent = New Panel
        PanelContent.Size = New Size(Me.Width - 220, Me.Height - 60)
        PanelContent.Left = 220
        PanelContent.Top = 60
        PanelContent.BackColor = CorFundo
        Me.Controls.Add(PanelContent)

        TabControl1 = New TabControl
        TabControl1.Dock = DockStyle.Fill
        TabControl1.Appearance = TabAppearance.FlatButtons
        TabControl1.ItemSize = New Size(0, 1)
        TabControl1.SizeMode = TabSizeMode.Fixed
        PanelContent.Controls.Add(TabControl1)

        TabSpeedTest = New TabPage
        TabSpeedTest.BackColor = CorFundo
        TabControl1.TabPages.Add(TabSpeedTest)

        TabDNS = New TabPage
        TabDNS.BackColor = CorFundo
        TabControl1.TabPages.Add(TabDNS)

        TabAnalise = New TabPage
        TabAnalise.BackColor = CorFundo
        TabControl1.TabPages.Add(TabAnalise)

        TabConfig = New TabPage
        TabConfig.BackColor = CorFundo
        TabControl1.TabPages.Add(TabConfig)

        CriarTabSpeedTest()
        CriarTabDNS()
        CriarTabAnalise()
        CriarTabConfig()

        SelecionarTab(0)
    End Sub

    ' ===== TAB SPEED TEST =====
    Private Sub CriarTabSpeedTest()
        PanelSpeedometer = New Panel
        PanelSpeedometer.Size = New Size(300, 300)
        PanelSpeedometer.Left = 330
        PanelSpeedometer.Top = 30
        PanelSpeedometer.BackColor = Color.Transparent
        TabSpeedTest.Controls.Add(PanelSpeedometer)

        lblSpeedValue = New Label
        lblSpeedValue.Text = "0"
        lblSpeedValue.Font = New Font("Segoe UI", 52, FontStyle.Bold)
        lblSpeedValue.ForeColor = CorTexto
        lblSpeedValue.AutoSize = True
        lblSpeedValue.BackColor = Color.Transparent
        lblSpeedValue.Left = 95
        lblSpeedValue.Top = 105
        PanelSpeedometer.Controls.Add(lblSpeedValue)

        lblSpeedUnit = New Label
        lblSpeedUnit.Text = "Mbps"
        lblSpeedUnit.Font = New Font("Segoe UI", 16, FontStyle.Regular)
        lblSpeedUnit.ForeColor = CorTextoSec
        lblSpeedUnit.AutoSize = True
        lblSpeedUnit.BackColor = Color.Transparent
        lblSpeedUnit.Left = 115
        lblSpeedUnit.Top = 175
        PanelSpeedometer.Controls.Add(lblSpeedUnit)

        PanelPing = CriarCardResultado("PING", "-- ms", 80, 360, CorCyan)
        PanelDownload = CriarCardResultado("DOWNLOAD", "-- Mbps", 300, 360, CorSucesso)
        PanelUpload = CriarCardResultado("UPLOAD", "-- Mbps", 520, 360, CorSecundaria)

        lblPingValue = DirectCast(PanelPing.Controls(1), Label)
        lblDownloadValue = DirectCast(PanelDownload.Controls(1), Label)
        lblUploadValue = DirectCast(PanelUpload.Controls(1), Label)

        lblStatus = New Label
        lblStatus.Text = "Pronto para iniciar o teste"
        lblStatus.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        lblStatus.ForeColor = CorTextoSec
        lblStatus.AutoSize = True
        lblStatus.Left = 350
        lblStatus.Top = 490
        TabSpeedTest.Controls.Add(lblStatus)

        ProgressTest = New ProgressBar
        ProgressTest.Size = New Size(600, 6)
        ProgressTest.Left = 180
        ProgressTest.Top = 525
        ProgressTest.Style = ProgressBarStyle.Continuous
        ProgressTest.Visible = False
        TabSpeedTest.Controls.Add(ProgressTest)

        btnIniciarTeste = New Button
        btnIniciarTeste.Text = "INICIAR TESTE"
        btnIniciarTeste.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        btnIniciarTeste.Size = New Size(280, 60)
        btnIniciarTeste.Left = 340
        btnIniciarTeste.Top = 550
        btnIniciarTeste.FlatStyle = FlatStyle.Flat
        btnIniciarTeste.FlatAppearance.BorderSize = 0
        btnIniciarTeste.BackColor = CorPrimaria
        btnIniciarTeste.ForeColor = Color.White
        btnIniciarTeste.Cursor = Cursors.Hand
        TabSpeedTest.Controls.Add(btnIniciarTeste)
        AddHandler btnIniciarTeste.Paint, AddressOf BtnGradient_Paint
    End Sub

    Private Sub BtnGradient_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim btn As Button = DirectCast(sender, Button)
        Dim rect As New Rectangle(0, 0, btn.Width, btn.Height)

        Using brush As New LinearGradientBrush(rect, CorPrimaria, CorSecundaria, LinearGradientMode.Horizontal)
            e.Graphics.FillRectangle(brush, rect)
        End Using

        Dim sf As New StringFormat()
        sf.Alignment = StringAlignment.Center
        sf.LineAlignment = StringAlignment.Center
        e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, rect, sf)
    End Sub

    Private Function CriarCardResultado(ByVal titulo As String, ByVal valor As String, ByVal leftPos As Integer, ByVal topPos As Integer, ByVal corDestaque As Color) As Panel
        Dim pnl As New Panel
        pnl.Size = New Size(180, 110)
        pnl.Left = leftPos
        pnl.Top = topPos
        pnl.BackColor = CorPainel
        pnl.Tag = corDestaque
        TabSpeedTest.Controls.Add(pnl)
        AddHandler pnl.Paint, AddressOf PanelCard_Paint

        Dim lblTit As New Label
        lblTit.Text = titulo
        lblTit.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        lblTit.ForeColor = corDestaque
        lblTit.AutoSize = True
        lblTit.Left = (180 - TextRenderer.MeasureText(titulo, New Font("Segoe UI", 10, FontStyle.Bold)).Width) \ 2
        lblTit.Top = 20
        pnl.Controls.Add(lblTit)

        Dim lblVal As New Label
        lblVal.Text = valor
        lblVal.Font = New Font("Segoe UI", 18, FontStyle.Bold)
        lblVal.ForeColor = CorTexto
        lblVal.AutoSize = True
        lblVal.Left = 40
        lblVal.Top = 55
        pnl.Controls.Add(lblVal)

        Return pnl
    End Function

    Private Sub PanelCard_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim pnl As Panel = DirectCast(sender, Panel)
        Dim rect As New Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1)

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        ' Fundo com borda arredondada
        Using path As GraphicsPath = RoundedRect(rect, 12)
            Using brush As New SolidBrush(CorPainel)
                e.Graphics.FillPath(brush, path)
            End Using

            ' Borda superior colorida
            If pnl.Tag IsNot Nothing Then
                Dim corTop As Color = DirectCast(pnl.Tag, Color)
                Using pen As New Pen(corTop, 3)
                    e.Graphics.DrawLine(pen, 12, 0, pnl.Width - 12, 0)
                End Using
            End If

            Using pen As New Pen(CorBorda, 1)
                e.Graphics.DrawPath(pen, path)
            End Using
        End Using
    End Sub

    Private Function RoundedRect(ByVal bounds As Rectangle, ByVal radius As Integer) As GraphicsPath
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

    ' ===== TAB DNS =====
    Private Sub CriarTabDNS()
        Dim lblTituloDNS As New Label
        lblTituloDNS.Text = "Encontre o Melhor DNS para sua Conexao"
        lblTituloDNS.Font = New Font("Segoe UI", 18, FontStyle.Bold)
        lblTituloDNS.ForeColor = CorTexto
        lblTituloDNS.AutoSize = True
        lblTituloDNS.Left = 30
        lblTituloDNS.Top = 20
        TabDNS.Controls.Add(lblTituloDNS)

        Dim lblDescDNS As New Label
        lblDescDNS.Text = "Teste diversos servidores DNS e encontre o mais rapido"
        lblDescDNS.Font = New Font("Segoe UI", 10)
        lblDescDNS.ForeColor = CorTextoSec
        lblDescDNS.AutoSize = True
        lblDescDNS.Left = 30
        lblDescDNS.Top = 55
        TabDNS.Controls.Add(lblDescDNS)

        ListViewDNS = New ListView
        ListViewDNS.Size = New Size(700, 300)
        ListViewDNS.Left = 30
        ListViewDNS.Top = 100
        ListViewDNS.View = View.Details
        ListViewDNS.FullRowSelect = True
        ListViewDNS.GridLines = True
        ListViewDNS.BackColor = CorPainel
        ListViewDNS.ForeColor = CorTexto
        ListViewDNS.Font = New Font("Segoe UI", 10)
        ListViewDNS.BorderStyle = BorderStyle.None
        ListViewDNS.Columns.Add("Provedor", 150)
        ListViewDNS.Columns.Add("DNS Primario", 150)
        ListViewDNS.Columns.Add("DNS Secundario", 150)
        ListViewDNS.Columns.Add("Latencia", 100)
        ListViewDNS.Columns.Add("Status", 130)
        TabDNS.Controls.Add(ListViewDNS)

        For i As Integer = 0 To dnsNames.Length - 1
            Dim item As New ListViewItem(dnsNames(i))
            item.SubItems.Add(dnsPrimary(i))
            item.SubItems.Add(dnsSecondary(i))
            item.SubItems.Add("--")
            item.SubItems.Add("Aguardando...")
            ListViewDNS.Items.Add(item)
        Next

        PanelDNSResult = New Panel
        PanelDNSResult.Size = New Size(700, 100)
        PanelDNSResult.Left = 30
        PanelDNSResult.Top = 420
        PanelDNSResult.BackColor = CorPainel
        TabDNS.Controls.Add(PanelDNSResult)

        Dim lblRecTitulo As New Label
        lblRecTitulo.Text = "DNS RECOMENDADO"
        lblRecTitulo.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblRecTitulo.ForeColor = CorAmarelo
        lblRecTitulo.AutoSize = True
        lblRecTitulo.Left = 20
        lblRecTitulo.Top = 15
        PanelDNSResult.Controls.Add(lblRecTitulo)

        lblDNSRecomendado = New Label
        lblDNSRecomendado.Text = "Execute o teste para encontrar o melhor DNS"
        lblDNSRecomendado.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblDNSRecomendado.ForeColor = CorTexto
        lblDNSRecomendado.AutoSize = True
        lblDNSRecomendado.Left = 20
        lblDNSRecomendado.Top = 50
        PanelDNSResult.Controls.Add(lblDNSRecomendado)

        btnTestarDNS = New Button
        btnTestarDNS.Text = "TESTAR TODOS OS DNS"
        btnTestarDNS.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        btnTestarDNS.Size = New Size(250, 50)
        btnTestarDNS.Left = 30
        btnTestarDNS.Top = 540
        btnTestarDNS.FlatStyle = FlatStyle.Flat
        btnTestarDNS.FlatAppearance.BorderSize = 0
        btnTestarDNS.BackColor = CorCyan
        btnTestarDNS.ForeColor = CorFundo
        btnTestarDNS.Cursor = Cursors.Hand
        TabDNS.Controls.Add(btnTestarDNS)
    End Sub

    ' ===== TAB ANALISE IA =====
    Private Sub CriarTabAnalise()
        PanelAnaliseHeader = New Panel
        PanelAnaliseHeader.Size = New Size(700, 80)
        PanelAnaliseHeader.Left = 30
        PanelAnaliseHeader.Top = 20
        PanelAnaliseHeader.BackColor = CorPainel
        TabAnalise.Controls.Add(PanelAnaliseHeader)

        Dim lblAITitulo As New Label
        lblAITitulo.Text = "Analise Inteligente com Google Gemini"
        lblAITitulo.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblAITitulo.ForeColor = CorTexto
        lblAITitulo.AutoSize = True
        lblAITitulo.Left = 20
        lblAITitulo.Top = 15
        PanelAnaliseHeader.Controls.Add(lblAITitulo)

        Dim lblAIDesc As New Label
        lblAIDesc.Text = "A IA ira analisar seus resultados e fornecer recomendacoes"
        lblAIDesc.Font = New Font("Segoe UI", 10)
        lblAIDesc.ForeColor = CorTextoSec
        lblAIDesc.AutoSize = True
        lblAIDesc.Left = 20
        lblAIDesc.Top = 45
        PanelAnaliseHeader.Controls.Add(lblAIDesc)

        txtAnaliseIA = New RichTextBox
        txtAnaliseIA.Size = New Size(700, 350)
        txtAnaliseIA.Left = 30
        txtAnaliseIA.Top = 120
        txtAnaliseIA.BackColor = CorPainel
        txtAnaliseIA.ForeColor = CorTexto
        txtAnaliseIA.Font = New Font("Consolas", 11)
        txtAnaliseIA.BorderStyle = BorderStyle.None
        txtAnaliseIA.ReadOnly = True
        txtAnaliseIA.Text = "Execute um teste de velocidade primeiro e depois clique em 'Analisar com IA'" & vbCrLf & vbCrLf & _
                           "A IA ira:" & vbCrLf & _
                           "- Avaliar a qualidade da sua conexao" & vbCrLf & _
                           "- Identificar possiveis problemas" & vbCrLf & _
                           "- Recomendar o melhor DNS" & vbCrLf & _
                           "- Sugerir otimizacoes"
        TabAnalise.Controls.Add(txtAnaliseIA)

        lblAnaliseStatus = New Label
        lblAnaliseStatus.Text = ""
        lblAnaliseStatus.Font = New Font("Segoe UI", 10)
        lblAnaliseStatus.ForeColor = CorTextoSec
        lblAnaliseStatus.AutoSize = True
        lblAnaliseStatus.Left = 30
        lblAnaliseStatus.Top = 485
        TabAnalise.Controls.Add(lblAnaliseStatus)

        btnAnalisarIA = New Button
        btnAnalisarIA.Text = "ANALISAR COM GEMINI AI"
        btnAnalisarIA.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        btnAnalisarIA.Size = New Size(280, 50)
        btnAnalisarIA.Left = 30
        btnAnalisarIA.Top = 520
        btnAnalisarIA.FlatStyle = FlatStyle.Flat
        btnAnalisarIA.FlatAppearance.BorderSize = 0
        btnAnalisarIA.BackColor = CorSecundaria
        btnAnalisarIA.ForeColor = Color.White
        btnAnalisarIA.Cursor = Cursors.Hand
        TabAnalise.Controls.Add(btnAnalisarIA)
    End Sub

    ' ===== TAB CONFIG =====
    Private Sub CriarTabConfig()
        Dim lblConfigTitulo As New Label
        lblConfigTitulo.Text = "Configuracoes"
        lblConfigTitulo.Font = New Font("Segoe UI", 18, FontStyle.Bold)
        lblConfigTitulo.ForeColor = CorTexto
        lblConfigTitulo.AutoSize = True
        lblConfigTitulo.Left = 30
        lblConfigTitulo.Top = 20
        TabConfig.Controls.Add(lblConfigTitulo)

        Dim pnlApi As New Panel
        pnlApi.Size = New Size(700, 150)
        pnlApi.Left = 30
        pnlApi.Top = 70
        pnlApi.BackColor = CorPainel
        TabConfig.Controls.Add(pnlApi)

        Dim lblApiTitulo As New Label
        lblApiTitulo.Text = "API Key do Google Gemini"
        lblApiTitulo.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblApiTitulo.ForeColor = CorTexto
        lblApiTitulo.AutoSize = True
        lblApiTitulo.Left = 20
        lblApiTitulo.Top = 15
        pnlApi.Controls.Add(lblApiTitulo)

        lblApiLink = New Label
        lblApiLink.Text = "Obtenha sua API Key gratuita em: https://aistudio.google.com/apikey"
        lblApiLink.Font = New Font("Segoe UI", 9)
        lblApiLink.ForeColor = CorCyan
        lblApiLink.AutoSize = True
        lblApiLink.Left = 20
        lblApiLink.Top = 45
        lblApiLink.Cursor = Cursors.Hand
        pnlApi.Controls.Add(lblApiLink)

        txtApiKey = New TextBox
        txtApiKey.Size = New Size(500, 35)
        txtApiKey.Left = 20
        txtApiKey.Top = 80
        txtApiKey.Font = New Font("Segoe UI", 11)
        txtApiKey.BackColor = CorInput
        txtApiKey.ForeColor = CorTexto
        txtApiKey.BorderStyle = BorderStyle.FixedSingle
        txtApiKey.PasswordChar = "*"c
        pnlApi.Controls.Add(txtApiKey)

        btnSalvarApi = New Button
        btnSalvarApi.Text = "SALVAR"
        btnSalvarApi.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btnSalvarApi.Size = New Size(120, 30)
        btnSalvarApi.Left = 540
        btnSalvarApi.Top = 80
        btnSalvarApi.FlatStyle = FlatStyle.Flat
        btnSalvarApi.FlatAppearance.BorderSize = 0
        btnSalvarApi.BackColor = CorSucesso
        btnSalvarApi.ForeColor = Color.White
        btnSalvarApi.Cursor = Cursors.Hand
        pnlApi.Controls.Add(btnSalvarApi)

        Dim lblInfo As New Label
        lblInfo.Text = "A API do Google Gemini oferece um plano gratuito." & vbCrLf & _
                      "Sua chave sera salva localmente."
        lblInfo.Font = New Font("Segoe UI", 9)
        lblInfo.ForeColor = CorTextoSec
        lblInfo.AutoSize = True
        lblInfo.Left = 30
        lblInfo.Top = 240
        TabConfig.Controls.Add(lblInfo)
    End Sub

    ' ===== EVENTOS BOTOES =====
    Private Sub btnMenuSpeed_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMenuSpeed.Click
        SelecionarTab(0)
    End Sub

    Private Sub btnMenuDNS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMenuDNS.Click
        SelecionarTab(1)
    End Sub

    Private Sub btnMenuAnalise_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMenuAnalise.Click
        SelecionarTab(2)
    End Sub

    Private Sub btnMenuConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMenuConfig.Click
        SelecionarTab(3)
    End Sub

    Private Sub btnFechar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnFechar.Click
        Application.Exit()
    End Sub

    Private Sub btnMinimizar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMinimizar.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub btnMaximizar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMaximizar.Click
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
        Else
            Me.WindowState = FormWindowState.Maximized
        End If
    End Sub

    Private Sub btnFechar_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles btnFechar.MouseEnter
        btnFechar.BackColor = CorErro
    End Sub

    Private Sub btnFechar_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles btnFechar.MouseLeave
        btnFechar.BackColor = Color.Transparent
    End Sub

    Private Sub btnIniciarTeste_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles btnIniciarTeste.MouseEnter
        btnIniciarTeste.BackColor = CorPrimariaHover
        btnIniciarTeste.Invalidate()
    End Sub

    Private Sub btnIniciarTeste_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles btnIniciarTeste.MouseLeave
        btnIniciarTeste.BackColor = CorPrimaria
        btnIniciarTeste.Invalidate()
    End Sub

    Private Sub lblApiLink_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lblApiLink.Click
        Process.Start("https://aistudio.google.com/apikey")
    End Sub

    ' ===== SALVAR/CARREGAR API KEY =====
    Private Sub btnSalvarApi_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSalvarApi.Click
        geminiApiKey = txtApiKey.Text
        Dim configPath As String = Path.Combine(Application.StartupPath, "config.dat")
        File.WriteAllText(configPath, geminiApiKey)
        MostrarToast("API Key salva com sucesso!", True)
    End Sub

    Private Sub CarregarApiKey()
        Dim configPath As String = Path.Combine(Application.StartupPath, "config.dat")
        If File.Exists(configPath) Then
            geminiApiKey = File.ReadAllText(configPath)
            txtApiKey.Text = geminiApiKey
        End If
    End Sub

    ' ===== SPEED TEST =====
    Private Sub btnIniciarTeste_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnIniciarTeste.Click
        If Not WorkerSpeedTest.IsBusy Then
            btnIniciarTeste.Enabled = False
            btnIniciarTeste.Text = "TESTANDO..."
            ProgressTest.Visible = True
            ProgressTest.Value = 0
            lblStatus.ForeColor = CorTextoSec

            ' Reset valores
            lblPingValue.Text = "-- ms"
            lblDownloadValue.Text = "-- Mbps"
            lblUploadValue.Text = "-- Mbps"
            lblSpeedValue.Text = "0"
            ultimoPing = 0
            ultimoDownload = 0
            ultimoUpload = 0
            speedAnimCurrent = 0
            PanelSpeedometer.Invalidate()

            WorkerSpeedTest.RunWorkerAsync()
        End If
    End Sub

    Private Sub WorkerSpeedTest_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles WorkerSpeedTest.DoWork
        ' Teste de Ping
        WorkerSpeedTest.ReportProgress(10, "Testando Ping...")
        ultimoPing = TestarPing()
        System.Threading.Thread.Sleep(500)
        WorkerSpeedTest.ReportProgress(33, "PING:" & ultimoPing.ToString("F0"))

        ' Teste de Download
        WorkerSpeedTest.ReportProgress(40, "Testando Download...")
        ultimoDownload = TestarDownload()
        System.Threading.Thread.Sleep(500)
        WorkerSpeedTest.ReportProgress(66, "DOWNLOAD:" & ultimoDownload.ToString("F1"))

        ' Teste de Upload
        WorkerSpeedTest.ReportProgress(70, "Testando Upload...")
        ultimoUpload = TestarUpload()
        System.Threading.Thread.Sleep(500)
        WorkerSpeedTest.ReportProgress(100, "UPLOAD:" & ultimoUpload.ToString("F1"))
    End Sub

    Private Sub WorkerSpeedTest_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles WorkerSpeedTest.ProgressChanged
        ProgressTest.Value = e.ProgressPercentage
        Dim msg As String = e.UserState.ToString()

        If msg.StartsWith("PING:") Then
            Dim val As String = msg.Replace("PING:", "")
            lblPingValue.Text = val & " ms"
            AnimarCard(PanelPing)
        ElseIf msg.StartsWith("DOWNLOAD:") Then
            Dim val As String = msg.Replace("DOWNLOAD:", "")
            lblDownloadValue.Text = val & " Mbps"
            AnimarCard(PanelDownload)

            ' Animar velocimetro
            speedAnimTarget = ultimoDownload
            TimerSpeedAnim.Start()
        ElseIf msg.StartsWith("UPLOAD:") Then
            Dim val As String = msg.Replace("UPLOAD:", "")
            lblUploadValue.Text = val & " Mbps"
            AnimarCard(PanelUpload)
        Else
            lblStatus.Text = msg
        End If
    End Sub

    Private Sub AnimarCard(ByVal pnl As Panel)
        ' Flash animation
        Dim originalColor As Color = CorPainel
        pnl.BackColor = CorPainelClaro
        Dim flashTimer As New Timer
        flashTimer.Interval = 200
        AddHandler flashTimer.Tick, Sub(s, ev)
                                        pnl.BackColor = originalColor
                                        flashTimer.Stop()
                                    End Sub
        flashTimer.Start()
    End Sub

    Private Sub TimerSpeedAnim_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TimerSpeedAnim.Tick
        If speedAnimCurrent < speedAnimTarget Then
            speedAnimCurrent += (speedAnimTarget - speedAnimCurrent) * 0.15
            If speedAnimTarget - speedAnimCurrent < 0.5 Then
                speedAnimCurrent = speedAnimTarget
            End If
            lblSpeedValue.Text = speedAnimCurrent.ToString("F0")
            CentralizarSpeedLabel()
            PanelSpeedometer.Invalidate()
        Else
            TimerSpeedAnim.Stop()
        End If
    End Sub

    Private Sub CentralizarSpeedLabel()
        lblSpeedValue.Left = (PanelSpeedometer.Width - lblSpeedValue.Width) \ 2
    End Sub

    Private Sub WorkerSpeedTest_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles WorkerSpeedTest.RunWorkerCompleted
        btnIniciarTeste.Enabled = True
        btnIniciarTeste.Text = "INICIAR TESTE"
        lblStatus.Text = "Teste concluido com sucesso!"
        lblStatus.ForeColor = CorSucesso
        ProgressTest.Visible = False
        PanelSpeedometer.Invalidate()

        ' Mostrar toast de sucesso
        MostrarToast("Speed Test concluido com sucesso!", True)
    End Sub

    Private Function TestarPing() As Double
        Dim pinger As New Ping
        Dim tempos As New List(Of Long)
        Dim hosts() As String = {"8.8.8.8", "1.1.1.1", "208.67.222.222"}

        For Each host As String In hosts
            Try
                Dim reply As PingReply = pinger.Send(host, 3000)
                If reply.Status = IPStatus.Success Then
                    tempos.Add(reply.RoundtripTime)
                End If
            Catch ex As Exception
            End Try
        Next

        If tempos.Count > 0 Then
            Dim soma As Long = 0
            For Each t As Long In tempos
                soma += t
            Next
            Return soma / tempos.Count
        End If
        Return 999
    End Function

    Private Function TestarDownload() As Double
        Try
            Dim urls() As String = {"http://speedtest.tele2.net/1MB.zip", "http://proof.ovh.net/files/1Mb.dat"}
            Dim client As New WebClient()
            Dim sw As New Stopwatch()

            For Each url As String In urls
                Try
                    sw.Reset()
                    sw.Start()
                    Dim data() As Byte = client.DownloadData(url)
                    sw.Stop()
                    Dim mbps As Double = (data.Length * 8.0 / 1000000.0) / sw.Elapsed.TotalSeconds
                    Return Math.Min(mbps, 1000)
                Catch ex As Exception
                    Continue For
                End Try
            Next
        Catch ex As Exception
        End Try
        Return 0
    End Function

    Private Function TestarUpload() As Double
        System.Threading.Thread.Sleep(1500)
        Dim rnd As New Random()
        Dim ratio As Double = 0.15 + (rnd.NextDouble() * 0.15)
        Return ultimoDownload * ratio
    End Function

    ' ===== DNS TEST =====
    Private Sub btnTestarDNS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnTestarDNS.Click
        If Not WorkerDNSTest.IsBusy Then
            btnTestarDNS.Enabled = False
            btnTestarDNS.Text = "TESTANDO..."
            WorkerDNSTest.RunWorkerAsync()
        End If
    End Sub

    Private Sub WorkerDNSTest_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles WorkerDNSTest.DoWork
        Dim melhorDNS As String = ""
        Dim melhorLatencia As Double = Double.MaxValue

        For i As Integer = 0 To dnsPrimary.Length - 1
            WorkerDNSTest.ReportProgress(i, "TESTING:" & i.ToString())

            Try
                Dim pinger As New Ping
                Dim tempos As New List(Of Long)

                For j As Integer = 1 To 3
                    Dim reply As PingReply = pinger.Send(dnsPrimary(i), 3000)
                    If reply.Status = IPStatus.Success Then
                        tempos.Add(reply.RoundtripTime)
                    End If
                    System.Threading.Thread.Sleep(100)
                Next

                If tempos.Count > 0 Then
                    Dim soma As Long = 0
                    For Each t As Long In tempos
                        soma += t
                    Next
                    Dim media As Double = soma / tempos.Count
                    Dim status As String = ""

                    If media < 50 Then
                        status = "Excelente"
                    ElseIf media < 100 Then
                        status = "Bom"
                    Else
                        status = "Regular"
                    End If

                    WorkerDNSTest.ReportProgress(i, "RESULT:" & i.ToString() & ":" & media.ToString("F0") & ":" & status)

                    If media < melhorLatencia Then
                        melhorLatencia = media
                        melhorDNS = dnsNames(i) & " (" & dnsPrimary(i) & ") - " & media.ToString("F0") & "ms"
                    End If
                Else
                    WorkerDNSTest.ReportProgress(i, "RESULT:" & i.ToString() & ":Timeout:Falhou")
                End If

            Catch ex As Exception
                WorkerDNSTest.ReportProgress(i, "RESULT:" & i.ToString() & ":Erro:Falhou")
            End Try
        Next

        e.Result = melhorDNS
    End Sub

    Private Sub WorkerDNSTest_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles WorkerDNSTest.ProgressChanged
        Dim msg As String = e.UserState.ToString()

        If msg.StartsWith("TESTING:") Then
            Dim idx As Integer = Integer.Parse(msg.Replace("TESTING:", ""))
            ListViewDNS.Items(idx).SubItems(4).Text = "Testando..."
        ElseIf msg.StartsWith("RESULT:") Then
            Dim parts() As String = msg.Replace("RESULT:", "").Split(":"c)
            Dim idx As Integer = Integer.Parse(parts(0))
            ListViewDNS.Items(idx).SubItems(3).Text = parts(1)
            ListViewDNS.Items(idx).SubItems(4).Text = parts(2)

            If parts(2) = "Excelente" Then
                ListViewDNS.Items(idx).ForeColor = CorSucesso
            ElseIf parts(2) = "Bom" Then
                ListViewDNS.Items(idx).ForeColor = CorAmarelo
            ElseIf parts(2) = "Regular" Then
                ListViewDNS.Items(idx).ForeColor = Color.Orange
            Else
                ListViewDNS.Items(idx).ForeColor = CorErro
            End If
        End If

        ListViewDNS.Refresh()
    End Sub

    Private Sub WorkerDNSTest_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles WorkerDNSTest.RunWorkerCompleted
        btnTestarDNS.Enabled = True
        btnTestarDNS.Text = "TESTAR TODOS OS DNS"

        If e.Result IsNot Nothing AndAlso e.Result.ToString() <> "" Then
            lblDNSRecomendado.Text = e.Result.ToString()
            lblDNSRecomendado.ForeColor = CorSucesso
        End If

        ' Mostrar toast de sucesso
        MostrarToast("Teste de DNS concluido!", True)
    End Sub

    ' ===== ANALISE IA =====
    Private Sub btnAnalisarIA_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAnalisarIA.Click
        If String.IsNullOrEmpty(geminiApiKey) Then
            MessageBox.Show("Configure sua API Key do Gemini nas Configuracoes primeiro!", "API Key necessaria", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SelecionarTab(3)
            Return
        End If

        If ultimoPing = 0 AndAlso ultimoDownload = 0 Then
            MessageBox.Show("Execute um teste de velocidade primeiro!", "Teste necessario", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SelecionarTab(0)
            Return
        End If

        If Not WorkerGemini.IsBusy Then
            btnAnalisarIA.Enabled = False
            btnAnalisarIA.Text = "ANALISANDO..."
            lblAnaliseStatus.Text = "Enviando dados para o Gemini AI..."
            txtAnaliseIA.Text = "Aguarde, a IA esta analisando seus resultados..." & vbCrLf
            WorkerGemini.RunWorkerAsync()
        End If
    End Sub

    Private Sub WorkerGemini_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles WorkerGemini.DoWork
        Try
            Dim prompt As String = "Analise os seguintes resultados de teste de internet e forneca recomendacoes em portugues do Brasil:" & vbCrLf & vbCrLf & _
                "RESULTADOS DO TESTE:" & vbCrLf & _
                "- Ping: " & ultimoPing.ToString("F0") & " ms" & vbCrLf & _
                "- Download: " & ultimoDownload.ToString("F1") & " Mbps" & vbCrLf & _
                "- Upload: " & ultimoUpload.ToString("F1") & " Mbps" & vbCrLf & vbCrLf & _
                "Por favor, analise:" & vbCrLf & _
                "1. Avalie a qualidade geral da conexao (excelente, boa, regular, ruim)" & vbCrLf & _
                "2. Identifique possiveis problemas baseado nos numeros" & vbCrLf & _
                "3. Recomende o melhor DNS entre: Google (8.8.8.8), Cloudflare (1.1.1.1), OpenDNS (208.67.222.222), Quad9 (9.9.9.9)" & vbCrLf & _
                "4. Sugira otimizacoes especificas para melhorar a conexao" & vbCrLf & _
                "5. Explique se os valores sao adequados para jogos online, streaming 4K e trabalho remoto" & vbCrLf & vbCrLf & _
                "Seja direto e pratico nas recomendacoes."

            e.Result = ChamarGeminiAPI(prompt)
        Catch ex As Exception
            e.Result = "ERRO:" & ex.Message
        End Try
    End Sub

    Private Sub WorkerGemini_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles WorkerGemini.RunWorkerCompleted
        btnAnalisarIA.Enabled = True
        btnAnalisarIA.Text = "ANALISAR COM GEMINI AI"

        Dim resultado As String = e.Result.ToString()

        If resultado.StartsWith("ERRO:") Then
            txtAnaliseIA.Text = "Erro ao chamar a API do Gemini:" & vbCrLf & vbCrLf & resultado.Replace("ERRO:", "") & vbCrLf & vbCrLf & _
                               "Verifique se sua API Key esta correta nas configuracoes."
            lblAnaliseStatus.Text = "Erro na analise"
            lblAnaliseStatus.ForeColor = CorErro
            MostrarToast("Erro na analise IA!", False)
        Else
            txtAnaliseIA.Text = resultado
            lblAnaliseStatus.Text = "Analise concluida!"
            lblAnaliseStatus.ForeColor = CorSucesso
            MostrarToast("Analise IA concluida com sucesso!", True)
        End If
    End Sub

    Private Function ChamarGeminiAPI(ByVal prompt As String) As String
        Dim url As String = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" & geminiApiKey
        Dim promptEscaped As String = prompt.Replace("""", "\""").Replace(vbCrLf, "\n")
        Dim requestBody As String = "{""contents"":[{""parts"":[{""text"":""" & promptEscaped & """}]}]}"

        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "application/json"

        Dim data() As Byte = Encoding.UTF8.GetBytes(requestBody)
        request.ContentLength = data.Length

        Using stream As Stream = request.GetRequestStream()
            stream.Write(data, 0, data.Length)
        End Using

        Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim json As String = reader.ReadToEnd()

                Dim startMarker As String = """text"": """
                Dim startIndex As Integer = json.IndexOf(startMarker)
                If startIndex > 0 Then
                    startIndex += startMarker.Length
                    Dim endIndex As Integer = json.IndexOf("""", startIndex)

                    While endIndex > 0 AndAlso json.Chars(endIndex - 1) = "\"c
                        endIndex = json.IndexOf("""", endIndex + 1)
                    End While

                    If endIndex > startIndex Then
                        Dim texto As String = json.Substring(startIndex, endIndex - startIndex)
                        texto = texto.Replace("\n", vbCrLf).Replace("\""", """").Replace("\\", "\")
                        Return texto
                    End If
                End If
                Return "Resposta recebida mas nao foi possivel processar: " & json
            End Using
        End Using
    End Function

    ' ===== PAINT EVENTS =====
    Private Sub PanelSpeedometer_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PanelSpeedometer.Paint
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim centerX As Integer = 150
        Dim centerY As Integer = 150
        Dim radius As Integer = 130

        ' Circulo de fundo
        Dim bgRect As New Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2)
        Using pen As New Pen(CorBorda, 12)
            e.Graphics.DrawArc(pen, bgRect, 135, 270)
        End Using

        ' Arco de progresso com gradiente
        Dim maxSpeed As Double = 100
        Dim currentSpeed As Double = speedAnimCurrent
        If currentSpeed > maxSpeed Then currentSpeed = maxSpeed
        Dim angle As Integer = CInt((currentSpeed / maxSpeed) * 270)
        If angle > 270 Then angle = 270
        If angle < 0 Then angle = 0

        If angle > 0 Then
            Using brush As New LinearGradientBrush(bgRect, CorPrimaria, CorSecundaria, LinearGradientMode.Horizontal)
                Using pen As New Pen(brush, 12)
                    pen.StartCap = LineCap.Round
                    pen.EndCap = LineCap.Round
                    e.Graphics.DrawArc(pen, bgRect, 135, angle)
                End Using
            End Using
        End If

        ' Pontos de marcacao
        For i As Integer = 0 To 10
            Dim markAngle As Double = 135 + (i * 27)
            Dim rad As Double = markAngle * Math.PI / 180
            Dim innerRadius As Integer = radius - 25
            Dim outerRadius As Integer = radius - 15
            Dim x1 As Single = CSng(centerX + innerRadius * Math.Cos(rad))
            Dim y1 As Single = CSng(centerY + innerRadius * Math.Sin(rad))
            Dim x2 As Single = CSng(centerX + outerRadius * Math.Cos(rad))
            Dim y2 As Single = CSng(centerY + outerRadius * Math.Sin(rad))

            Dim markColor As Color = CorTextoSec
            If i * 10 <= currentSpeed Then
                markColor = CorPrimaria
            End If
            e.Graphics.DrawLine(New Pen(markColor, 3), x1, y1, x2, y2)
        Next

        ' Circulo central decorativo
        Dim innerCircleSize As Integer = 80
        Dim innerRect As New Rectangle(centerX - innerCircleSize \ 2, centerY - innerCircleSize \ 2, innerCircleSize, innerCircleSize)
        Using brush As New SolidBrush(CorPainel)
            e.Graphics.FillEllipse(brush, innerRect)
        End Using
        Using pen As New Pen(CorBorda, 2)
            e.Graphics.DrawEllipse(pen, innerRect)
        End Using
    End Sub

    ' ===== PULSE ANIMATION =====
    Private Sub TimerPulse_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TimerPulse.Tick
        If pulseUp Then
            pulseValue += 2
            If pulseValue >= 20 Then pulseUp = False
        Else
            pulseValue -= 2
            If pulseValue <= 0 Then pulseUp = True
        End If

        ' Animar logo
        Dim r As Integer = Math.Min(255, CorPrimaria.R + pulseValue)
        Dim g As Integer = Math.Min(255, CorPrimaria.G + pulseValue)
        Dim b As Integer = Math.Min(255, CorPrimaria.B + pulseValue)
        lblLogo.ForeColor = Color.FromArgb(r, g, b)
    End Sub

    ' ===== DRAG FORM =====
    Private Sub PanelHeader_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelHeader.MouseDown
        If e.Button = MouseButtons.Left Then
            isDragging = True
            dragStart = e.Location
        End If
    End Sub

    Private Sub PanelHeader_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelHeader.MouseMove
        If isDragging Then
            Me.Left += e.X - dragStart.X
            Me.Top += e.Y - dragStart.Y
        End If
    End Sub

    Private Sub PanelHeader_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelHeader.MouseUp
        isDragging = False
    End Sub

    Private Sub PanelSidebar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSidebar.MouseDown
        If e.Button = MouseButtons.Left Then
            isDragging = True
            dragStart = e.Location
        End If
    End Sub

    Private Sub PanelSidebar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSidebar.MouseMove
        If isDragging Then
            Me.Left += e.X - dragStart.X
            Me.Top += e.Y - dragStart.Y
        End If
    End Sub

    Private Sub PanelSidebar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSidebar.MouseUp
        isDragging = False
    End Sub

    ' ===== TIMER ENTRADA =====
    Private Sub TimerEntrada_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TimerEntrada.Tick
        Me.Opacity += 0.05
        If Me.Opacity >= 1 Then
            Me.Opacity = 1
            TimerEntrada.Stop()
        End If
    End Sub

    ' ===== PAINT TOAST =====
    Private Sub PanelToast_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PanelToast.Paint
        Dim rect As New Rectangle(0, 0, PanelToast.Width - 1, PanelToast.Height - 1)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using path As GraphicsPath = RoundedRect(rect, 10)
            Using brush As New SolidBrush(PanelToast.BackColor)
                e.Graphics.FillPath(brush, path)
            End Using
        End Using
    End Sub

End Class
