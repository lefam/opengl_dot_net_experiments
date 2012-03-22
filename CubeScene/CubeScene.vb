Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class CubeScene
    Inherits GameWindow

    Protected textures(2) As Integer

    ' Rotação da camera sobre o eixo Y.
    Protected yaw As Single

    ' Rotação da camera sobre o eixo X.
    Protected pitch As Single

    ' Posição da camera.
    Protected camPos As New Vector3(0.0, 1.0, 8.0)

    ' Direcção da camera.
    Protected camAt As New Vector3(0.0, 0.0, -1.0)

    Protected angle As Integer

    Public Sub New()
        MyBase.New(480, 360, GraphicsMode.Default, "CubeScene (OpenGL in OpenTK)")
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        GL.ClearColor(0.1, 0.3, 0.2, 0.0)
        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Texture2D)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)

        LoadTextures()
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        Dim aspect As Single = Me.Width / CSng(Me.Height)
        Dim matProj As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 100.0)

        GL.Viewport(0, 0, Me.Width, Me.Height)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(matProj)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
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
        LoadTexture(textures(0), "stone_floor.png")
        LoadTexture(textures(1), "cube_texture.png")
    End Sub

    Protected Sub DrawFloor(ByVal texIndex As Integer)
        GL.BindTexture(TextureTarget.Texture2D, textures(texIndex))
        GL.Begin(BeginMode.Quads)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1.0, 0.0, -1.0)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1.0, 0.0, -1.0)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1.0, 0.0, 1.0)
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1.0, 0.0, 1.0)
        GL.End()
    End Sub

    Protected Sub DrawCube(ByVal texIndex As Integer)
        GL.BindTexture(TextureTarget.Texture2D, textures(texIndex))
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
    End Sub

    Protected Sub ApplyCamera()
        GL.Rotate(-Me.pitch, 1, 0, 0)
        GL.Rotate(-Me.yaw, 0, 1, 0)
        GL.Translate(-camPos.X, -camPos.Y, -camPos.Z)
    End Sub

    Protected Sub DrawRoom()
        Dim zPos As Single = -6.0
        Dim xPos As Single
        For i As Integer = 1 To 10
            xPos = -6.0
            For j As Integer = 1 To 10
                GL.LoadIdentity()
                ApplyCamera()
                GL.Translate(xPos, 0, zPos)

                DrawFloor(0)

                xPos += 2
            Next
            zPos += 2
        Next

        ' Cubo 1
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(0, 1, 0)
        DrawCube(1)

        ' Cubo 2
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(-4.0, 0.5, 5)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        ' Cubo 3
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(7.0, 0.5, 5)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        ' Cubo 4
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(12.0, 0.5, 10)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        ' Cubo 5
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(2.0, 0.5, 12)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        ' Cubo 6
        GL.LoadIdentity()
        ApplyCamera()
        GL.Translate(-6.0, 0.5, 12)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        ' Cubo 7 em rotação
        GL.LoadIdentity()
        ApplyCamera()

        GL.Translate(6, 1, -5)
        GL.Rotate(angle, 0, 1, 0)
        GL.Rotate(angle + 25, 0, 0, 1)
        GL.Rotate(angle + 50, 1, 0, 0)
        GL.Scale(0.5, 0.5, 0.5)
        DrawCube(1)

        angle += 1
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
        MyBase.OnRenderFrame(e)

        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()


        If Keyboard.Item(Input.Key.Left) Then
            yaw += 0.7
        End If

        If Keyboard.Item(Input.Key.Right) Then
            yaw -= 0.7
        End If

        If Keyboard.Item(Input.Key.PageDown) Then
            pitch += 0.7
        End If

        If Keyboard.Item(Input.Key.PageUp) Then
            pitch -= 0.7
        End If

        If Keyboard.Item(Input.Key.Up) Then
            camPos.Z -= 0.04 * Math.Cos(MathHelper.DegreesToRadians(yaw))
            camPos.X -= 0.04 * Math.Sin(MathHelper.DegreesToRadians(yaw))
        End If

        If Keyboard.Item(Input.Key.Down) Then
            camPos.Z += 0.04 * Math.Cos(MathHelper.DegreesToRadians(yaw))
            camPos.X += 0.04 * Math.Sin(MathHelper.DegreesToRadians(yaw))
        End If

        DrawRoom()
        SwapBuffers()
    End Sub

    Public Shared Sub Main()
        Dim app As New CubeScene
        app.Run(60, 60)
    End Sub
End Class