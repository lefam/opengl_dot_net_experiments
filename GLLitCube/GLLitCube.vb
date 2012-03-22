Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class GLLitCube
    Inherits GameWindow

    Protected angle As Single
    Protected textures(1) As Integer

    Public Sub New()
        MyBase.New(468, 360, GraphicsMode.Default, "Textured Cube with OpenTK")
    End Sub

    Protected Sub LoadTexture(ByVal textureId As Integer, ByVal filename As String)
        Dim bmp As New Bitmap(filename)

        Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        GL.BindTexture(TextureTarget.Texture2D, textureId)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                      bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra,
                      PixelType.UnsignedByte, data.Scan0)

        bmp.UnlockBits(data)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
    End Sub

    Protected Sub LoadTextures()
        GL.GenTextures(textures.Length, textures)
        LoadTexture(textures(0), "texture.png")
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        GL.ClearColor(0.5, 0.5, 0.6, 0)
        GL.ClearDepth(1.0)
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Texture2D)
        GL.Enable(EnableCap.Lighting)

        GL.Light(LightName.Light1, LightParameter.Ambient, New Single() {0.1, 0.1, 0.1, 1.0})
        GL.Light(LightName.Light1, LightParameter.Diffuse, New Single() {1.0, 1.0, 1.0, 1.0})
        GL.Light(LightName.Light1, LightParameter.Position, New Single() {0.0, 0.0, 6.0, 1.0})
        GL.Enable(EnableCap.Light1)

        LoadTextures()
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        ' TODO: Perceber melhor sobre o glViewport()
        GL.Viewport(0, 0, Me.Width, Me.Height)

        Dim aspect As Single = CSng(Me.Width) / Me.Height
        Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 100.0)

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(projMat)

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
        MyBase.OnRenderFrame(e)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        GL.LoadIdentity()
        GL.Translate(0, 0, -5)
        GL.Rotate(angle, 0, 1, 1)
        'GL.Rotate(angle, 0, 1, 0)
        GL.Rotate(angle, 1, 0, 0)
        GL.Rotate(angle, 0, 1, 0)
        GL.Rotate(angle, 0, 0, 1)

        angle += 0.5

        GL.BindTexture(TextureTarget.Texture2D, textures(0))

        GL.Begin(BeginMode.Quads)
        ' Face frontal
        GL.Normal3(0.0, 0.0, 1.0)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, 1)

        ' Face traseira
        GL.Normal3(0.0, 0.0, -1.0)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, -1, -1)

        ' Face lateral direita
        GL.Normal3(1.0, 0.0, 0.0)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, -1, -1)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, -1, 1)

        ' Face lateral esquerda
        GL.Normal3(-1.0, 0.0, 0.0)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, 1, 1)

        ' Face de cima
        GL.Normal3(0.0, 1.0, 0.0)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, 1)

        ' Face de baixo
        GL.Normal3(0.0, -1.0, 0.0)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, -1, -1)
        GL.End()

        SwapBuffers()
    End Sub

    Public Shared Sub Main()
        Dim app As New GLLitCube
        app.Run(60, 60)
    End Sub
End Class