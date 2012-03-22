'
' Créditos para:
' - http://www.codeproject.com/KB/openGL/OPENGLTG.aspx
' - http://www.lighthouse3d.com/opengl/terrain/index.php?heightmap
'
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class Terrain
    Inherits GameWindow

    Protected heightMap(,) As Single
    Protected terrain(,,) As Single

    Protected terrainWidth As Integer
    Protected terrainHeight As Integer
    Protected mapWidth As Integer
    Protected mapHeight As Integer

    Protected terrainGL As Integer

    Protected textures(2) As Integer

    Public Const HEIGHT_SCALE = 32.0
    Public Const MAP_SCALE_X = 2
    Public Const MAP_SCALE_Y = 2

    Protected camera As New Camera(0, 0, 0)

    Public Sub New()
        MyBase.New(480, 360, GraphicsMode.Default, "Terrain Rendering with OpenGL")
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
        LoadTexture(textures(0), "burnt_sand.jpg")
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        GL.ClearColor(0.1, 0.4, 0.4, 1.0)
        GL.Enable(EnableCap.DepthTest)

        ' Set front face in clockwise direction and enable backface culling.
        GL.FrontFace(FrontFaceDirection.Cw)
        GL.CullFace(CullFaceMode.Back)
        GL.Enable(EnableCap.CullFace)

        GL.Enable(EnableCap.Texture2D)

        GL.Enable(EnableCap.Lighting)
        GL.Enable(EnableCap.Light1)
        GL.Light(LightName.Light1, LightParameter.Ambient, New Single() {0.3, 0.3, 0.3, 1.0})
        GL.Light(LightName.Light1, LightParameter.Diffuse, New Single() {1.0, 1.0, 1.0, 1.0})
        GL.Light(LightName.Light1, LightParameter.Position, New Single() {0.0, 0.0, 1.0, 0.0})
        GL.LightModel(LightModelParameter.LightModelAmbient, New Single() {0.2, 0.2, 0.2, 1.0})

        GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, New Single() {0.9, 0.8, 0.7, 1.0})

        LoadTextures()
        LoadHeightMap("heightmap1.png")
        GenerateTerrain(0, 0, 0)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        GL.Viewport(0, 0, Me.Width, Me.Height)

        Dim aspect As Single = Me.Width / CSng(Me.Height)
        Dim mat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 350.0)

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(mat)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    Protected Sub LoadHeightMap(ByVal filename As String)
        Dim bmp As New Bitmap(filename)

        If bmp.Width < 2048 AndAlso bmp.Height < 2048 Then
            ReDim heightMap(bmp.Height, bmp.Width)
        Else
            Throw New ArgumentException("The image width and height should not be greater that 2048.")
        End If

        mapWidth = bmp.Width
        mapHeight = bmp.Height

        For i As Integer = 0 To mapHeight - 1
            For j As Integer = 0 To mapWidth - 1
                Dim color As Color = bmp.GetPixel(j, i)
                ' Em princípio as imagens usadas já estarão em greyscale. Caso não, a instrução abaixo
                ' converte a cor para greyscale. A fórmula foi aprendida no artigo em http://lodev.org/cgtutor/color.html
                Dim greyScale As Single = (CInt(color.R) + color.G + color.B) / 3.0

                ' Faz clamp do valor da altura para a escala [0,1]
                heightMap(i, j) = greyScale / 255.0
            Next
        Next
    End Sub

    Protected Function GetHeight(ByVal xPos As Integer, ByVal zPos As Integer) As Single
        Dim xMap As Integer = (xPos + (terrainWidth / 2.0)) / MAP_SCALE_X
        Dim yMap As Integer = (zPos + (terrainHeight / 2.0)) / MAP_SCALE_Y
        If xMap >= 0 AndAlso xMap < mapWidth AndAlso yMap >= 0 AndAlso yMap < mapHeight Then
            Return heightMap(yMap, xMap) * HEIGHT_SCALE
            'Dim avg As Single = (heightMap(yMap, xMap) + heightMap(yMap + 1, xMap) + heightMap(yMap, xMap + 1) + heightMap(yMap + 1, xMap + 1)) / 4.0
            'Return avg * HEIGHT_SCALE
        End If
        Return 0
    End Function

    Public Sub GenerateTerrain(ByVal xOfs As Single, ByVal yOfs As Single, ByVal zOfs As Single)
        terrainWidth = mapWidth * MAP_SCALE_X
        terrainHeight = mapHeight * MAP_SCALE_Y

        ReDim terrain(mapHeight, mapWidth, 3)
        For i = 0 To mapHeight - 1
            For j = 0 To mapWidth - 1
                terrain(i, j, 0) = j * MAP_SCALE_X - terrainWidth / 2 + xOfs
                terrain(i, j, 1) = heightMap(i, j) * HEIGHT_SCALE + yOfs
                terrain(i, j, 2) = i * MAP_SCALE_Y - terrainHeight / 2 + zOfs
            Next
        Next

        Dim normals(mapHeight, mapWidth) As Vector3
        For i = 0 To mapHeight - 1
            For j = 0 To mapWidth - 1
                normals(i, j) = New Vector3(0, 0, 0)
            Next
        Next

        For i = 0 To mapHeight - 2
            For j = 0 To mapWidth - 2
                Dim v1 As Vector3, v2 As Vector3, v3 As Vector3, v4 As Vector3, n As Vector3

                v1 = New Vector3(terrain(i, j, 0), terrain(i, j, 1), terrain(i, j, 2))
                v2 = New Vector3(terrain(i, j + 1, 0), terrain(i, j + 1, 1), terrain(i, j + 1, 2))
                v3 = New Vector3(terrain(i + 1, j, 0), terrain(i + 1, j, 1), terrain(i + 1, j, 2))
                v4 = New Vector3(terrain(i + 1, j + 1, 0), terrain(i + 1, j + 1, 1), terrain(i + 1, j + 1, 2))

                ' Normais para triangulo 1
                n = Vector3.Cross(v3 - v1, v2 - v1)
                'n.Normalize()
                normals(i, j) += n
                normals(i, j).Normalize()
                normals(i, j + 1) += n
                normals(i, j + 1).Normalize()
                normals(i + 1, j) += n
                normals(i + 1, j).Normalize()

                ' Normais para triangulo 2
                n = Vector3.Cross(v3 - v2, v4 - v2)
                'n.Normalize()
                normals(i, j + 1) += n
                normals(i, j + 1).Normalize()
                normals(i + 1, j + 1) += n
                normals(i + 1, j + 1).Normalize()
                normals(i + 1, j) += n
                normals(i + 1, j).Normalize()
            Next
        Next

        terrainGL = GL.GenLists(1)

        GL.NewList(terrainGL, ListMode.Compile)

        'GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse)
        GL.BindTexture(TextureTarget.Texture2D, textures(0))

        For i = 0 To mapHeight - 2
            GL.Begin(BeginMode.TriangleStrip)
            For j = 0 To mapWidth - 2
                'GL.Color3(1.0, 1.0, 1.0)
                ' Vértice 1
                'GL.Color3(heightMap(i, j), heightMap(i, j), heightMap(i, j))
                GL.Normal3(normals(i, j))
                GL.TexCoord2(0.0, 1.0)
                GL.Vertex3(terrain(i, j, 0), terrain(i, j, 1), terrain(i, j, 2))

                ' Vértice 2
                'GL.Color3(heightMap(i, j + 1), heightMap(i, j + 1), heightMap(i, j + 1))
                GL.Normal3(normals(i, j + 1))
                GL.TexCoord2(1.0, 1.0)
                GL.Vertex3(terrain(i, j + 1, 0), terrain(i, j + 1, 1), terrain(i, j + 1, 2))

                ' Vértice 3
                'GL.Color3(heightMap(i + 1, j), heightMap(i + 1, j), heightMap(i + 1, j))
                GL.Normal3(normals(i + 1, j))
                GL.TexCoord2(0.0, 0.0)
                GL.Vertex3(terrain(i + 1, j, 0), terrain(i + 1, j, 1), terrain(i + 1, j, 2))

                ' Vértice 4
                'GL.Color3(heightMap(i + 1, j + 1), heightMap(i + 1, j + 1), heightMap(i + 1, j + 1))
                GL.Normal3(normals(i + 1, j + 1))
                GL.TexCoord2(1.0, 0.0)
                GL.Vertex3(terrain(i + 1, j + 1, 0), terrain(i + 1, j + 1, 1), terrain(i + 1, j + 1, 2))
            Next
            GL.End()
        Next
        GL.EndList()
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
        MyBase.OnRenderFrame(e)

        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        If Keyboard.Item(Input.Key.Up) Then
            camera.Walk(0.2)
        End If
        If Keyboard.Item(Input.Key.Down) Then
            camera.Walk(-0.2)
        End If
        If Keyboard.Item(Input.Key.Left) Then
            camera.yaw += 1
        End If
        If Keyboard.Item(Input.Key.Right) Then
            camera.yaw -= 1
        End If

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        camera.pos.Y = GetHeight(camera.pos.X, camera.pos.Z) + 1.75
        camera.Render()

        GL.CallList(terrainGL)

        SwapBuffers()
    End Sub

    Public Shared Sub Main()
        Dim app As New Terrain
        app.Run(60, 60)
    End Sub
End Class
