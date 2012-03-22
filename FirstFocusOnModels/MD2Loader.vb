Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Timers.Timer
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text

Public Class MD2Loader

    <StructLayout(LayoutKind.Sequential)>
    Structure MD2Header
        Dim magicNumber As Integer
        Dim version As Integer
        Dim skinWidth As Integer
        Dim skinHeight As Integer
        Dim frameSize As Integer
        Dim numSkins As Integer
        Dim numVertices As Integer
        Dim numTexCoords As Integer
        Dim numTriangles As Integer
        Dim numGLCommands As Integer
        Dim numFrames As Integer
        Dim offsetSkins As Integer
        Dim offsetTexCoords As Integer
        Dim offsetTriangles As Integer
        Dim offsetFrames As Integer
        Dim offsetGLCommands As Integer
        Dim offsetEnd As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MD2TexCoord
        Dim s As Short
        Dim t As Short
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MD2Frame
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3, ArraySubType:=UnmanagedType.R4)>
        Dim scale() As Single
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3, ArraySubType:=UnmanagedType.R4)>
        Dim translate() As Single
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)>
        Dim name() As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MD2Triangle
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)>
        Dim vertIndices() As Short
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)>
        Dim texIndices() As Short
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MD2Vertex
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)>
        Dim vertices() As Byte
        Dim lightNormalIndex As Byte
    End Structure

    Structure Frame
        Dim vertices() As Vector3
        Dim name As String
    End Structure

    Structure MD2Model
        Dim frames() As Frame
        Dim texCoords() As MD2TexCoord
        Dim triangles() As MD2Triangle
    End Structure

    Public Const MD2_MAGIC_NUMBER = 844121161
    Public Const MD2_VERSION = 8

    Protected filename As String
    Protected skinFileName As String
    Protected scale As Single
    Protected header As MD2Header
    Protected texCoords(,) As Single
    Protected triangles() As MD2Triangle
    'Protected vertices() As Vector3
    Protected textures(1) As Integer
    Protected model As MD2Model

    Protected startFrame As Long
    Protected endFrame As Long
    Protected curFrame As Long
    Protected nextFrame As Long
    Protected timePerFrame As Long
    Protected lastTime As Long

    Public Sub New(ByVal modelFileName As String, ByVal skinFileName As String, ByVal scale As Single)
        Me.filename = modelFileName
        Me.skinFileName = skinFileName
        Me.scale = scale
        LoadModel()
        LoadTextures()
    End Sub

    Protected Sub BytesToStructure(ByVal bytes() As Byte, ByRef t As ValueType)
        Dim gch As GCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned)
        t = Marshal.PtrToStructure(gch.AddrOfPinnedObject, t.GetType())
        gch.Free()
    End Sub


    Protected Sub LoadModel()
        Dim fs As New FileStream(filename, FileMode.Open, FileAccess.Read)
        Dim br As New BinaryReader(fs)
        Dim bytes() As Byte

        bytes = br.ReadBytes(Marshal.SizeOf(GetType(MD2Header)))
        BytesToStructure(bytes, header)

        If (header.magicNumber <> MD2_MAGIC_NUMBER) Or (header.version <> MD2_VERSION) Then
            Throw New ArgumentException("Error: Invalid MD2 Model!")
        End If

        ReDim texCoords(header.numTexCoords, 2)
        br.BaseStream.Seek(header.offsetTexCoords, SeekOrigin.Begin)
        For i = 0 To header.numTexCoords - 1
            Dim stCoord As MD2TexCoord
            bytes = br.ReadBytes(Marshal.SizeOf(GetType(MD2TexCoord)))
            BytesToStructure(bytes, stCoord)
            texCoords(i, 0) = stCoord.s / header.skinWidth
            texCoords(i, 1) = stCoord.t / header.skinHeight
        Next

        ReDim triangles(header.numTriangles)
        br.BaseStream.Seek(header.offsetTriangles, SeekOrigin.Begin)
        For i = 0 To header.numTriangles - 1
            bytes = br.ReadBytes(Marshal.SizeOf(GetType(MD2Triangle)))
            BytesToStructure(bytes, triangles(i))
        Next

        ReDim model.frames(header.numFrames)
        br.BaseStream.Seek(header.offsetFrames, SeekOrigin.Begin)
        For i = 0 To header.numFrames - 1
            Dim frame As New MD2Frame
            ReDim model.frames(i).vertices(header.numVertices)

            bytes = br.ReadBytes(Marshal.SizeOf(GetType(MD2Frame)))
            BytesToStructure(bytes, frame)

            'Console.WriteLine(Encoding.ASCII.GetString(frame.name))

            Dim vertex As MD2Vertex

            For j = 0 To header.numVertices - 1
                ' Carrega os vertices do frame actual
                bytes = br.ReadBytes(Marshal.SizeOf(GetType(MD2Vertex)))
                BytesToStructure(bytes, vertex)
                model.frames(i).vertices(j) = New Vector3(
                    vertex.vertices(0) * frame.scale(0) + frame.translate(0),
                    vertex.vertices(1) * frame.scale(1) + frame.translate(1),
                    vertex.vertices(2) * frame.scale(2) + frame.translate(2)
                )
            Next
        Next

        SetAnimation(0, header.numFrames - 1, 100, 0)
        'SetAnimation(0, 39, 111, 0)
        'Console.ReadLine()
    End Sub

    Public Sub SetAnimation(ByVal startFrame As Long, ByVal endFrame As Long, ByVal timePerFrame As Long, ByVal currentTime As Long)
        Me.lastTime = currentTime
        Me.startFrame = startFrame
        Me.endFrame = endFrame
        Me.curFrame = startFrame
        Me.nextFrame = IIf(endFrame > startFrame, startFrame + 1, startFrame)
        Me.timePerFrame = timePerFrame
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
        LoadTexture(textures(0), skinFileName)
    End Sub

    Public Sub Render(ByVal time As Long)
        Dim delta As Long = time - lastTime

        If delta > timePerFrame Then
            lastTime = time
            curFrame = nextFrame
            nextFrame += 1
            If nextFrame > endFrame Then nextFrame = startFrame
        End If

        Dim blend As Single = CSng(delta) / CSng(timePerFrame)

        Dim vertices(header.numVertices) As Vector3

        For i = 0 To header.numVertices - 1
            vertices(i) = Vector3.Lerp(model.frames(curFrame).vertices(i), model.frames(nextFrame).vertices(i), blend) * scale
        Next

        GL.PushMatrix()
        GL.Rotate(-90, 1, 0, 0)
        GL.Rotate(-90, 0, 0, 1)
        'GL.Scale(0.2, 0.2, 0.2)
        'GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line)

        GL.BindTexture(TextureTarget.Texture2D, textures(0))

        GL.Begin(BeginMode.Triangles)
        For i = 0 To header.numTriangles - 1
            GL.TexCoord2(texCoords(triangles(i).texIndices(0), 0), texCoords(triangles(i).texIndices(0), 1))
            GL.Vertex3(
                vertices(triangles(i).vertIndices(0)).X,
                vertices(triangles(i).vertIndices(0)).Y,
                vertices(triangles(i).vertIndices(0)).Z
            )
            GL.TexCoord2(texCoords(triangles(i).texIndices(1), 0), texCoords(triangles(i).texIndices(1), 1))
            GL.Vertex3(
                vertices(triangles(i).vertIndices(1)).X,
                vertices(triangles(i).vertIndices(1)).Y,
                vertices(triangles(i).vertIndices(1)).Z
            )
            GL.TexCoord2(texCoords(triangles(i).texIndices(2), 0), texCoords(triangles(i).texIndices(2), 1))
            GL.Vertex3(
                vertices(triangles(i).vertIndices(2)).X,
                vertices(triangles(i).vertIndices(2)).Y,
                vertices(triangles(i).vertIndices(2)).Z
            )
        Next
        GL.End()
        GL.PopMatrix()
    End Sub
End Class
