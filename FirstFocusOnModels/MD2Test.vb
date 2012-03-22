Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Timers.Timer
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text

Public Class MD2Test
    Inherits GameWindow

    Protected timer As New System.Diagnostics.Stopwatch
    Protected ogros As MD2Loader
    Protected gunner_head As MD2Loader
    Protected gunner_body As MD2Loader
    Protected gunner_weapon As MD2Loader
    Protected weapon As MD2Loader
    Protected model As MD2Loader
    Protected yaw As Single

    Public Sub New()
        MyBase.New(480, 360, GraphicsMode.Default, "OpenGL MD2 Animation")
        Me.VSync = VSyncMode.Off
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        GL.Enable(EnableCap.DepthTest)
        GL.ClearColor(0.2, 0.2, 0.3, 1.0)
        'GL.ClearColor(0.0, 0.0, 0.0, 1.0)
        GL.Enable(EnableCap.Texture2D)

        ogros = New MD2Loader("models\ogros.md2", "models\igdosh.png", 0.22)
        weapon = New MD2Loader("models\weapon.md2", "models\weapon.png", 0.22)
        model = New MD2Loader("models\goblin.md2", "models\goblin.jpg", 0.2)

        ogros.SetAnimation(0, 39, 100, 0)
        weapon.SetAnimation(0, 39, 100, 0)

        timer.Start()
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        Dim aspect As Single = Me.Width / CSng(Me.Height)
        Dim mat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000)
        GL.Viewport(0, 0, Me.Width, Me.Height)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(mat)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
        MyBase.OnRenderFrame(e)

        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
        GL.Translate(0, 0, -20)
        GL.Rotate(25, 1, 0, 0)
        GL.Rotate(yaw, 0, 1, 0)

        Dim time As Long = timer.ElapsedMilliseconds

        yaw += 0.05

        GL.PushMatrix()
        GL.Translate(3, 0, 0)
        ogros.Render(time)
        weapon.Render(time)
        GL.PopMatrix()

        GL.PushMatrix()
        GL.Translate(-4, 0, 0)
        model.Render(time)
        GL.PopMatrix()
        'gunner_body.Render(time)
        'gunner_head.Render(time)
        'gunner_weapon.Render(time)

        SwapBuffers()
    End Sub

    Public Shared Sub Main()
        Dim app As New MD2Test
        app.Run()
    End Sub
End Class
