'
' Rotating Gouraud Shaded Cube in OpenGL using the OpenTK library.
' Developed by leonelmachava <leonelmachava@gmail.com>
' http://codentronix.com
'
' Copyright (c) 2011 Leonel Machava
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this 
' software and associated documentation files (the "Software"), to deal in the Software 
' without restriction, including without limitation the rights to use, copy, modify, 
' merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
' permit persons to whom the Software is furnished to do so, subject to the following 
' conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies 
' or substantial portions of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
' OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class GLShadedCube
    Inherits GameWindow

    Protected angle As Single

    Public Sub New()
        MyBase.New(320, 240, GraphicsMode.Default, "First touch with OpenTK")
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        GL.ClearColor(0.5, 0.5, 0.6, 0)
        GL.Enable(EnableCap.DepthTest)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

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

        angle += 1

        GL.Begin(BeginMode.Quads)

        ' Front face 
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(-1, 1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, 1, 1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(1, -1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, -1, 1)

        ' Back face
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(1, 1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, 1, -1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(-1, -1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, -1, -1)

        ' Right face
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(1, 1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, 1, 1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(1, -1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, -1, -1)

        ' Left face
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(-1, 1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, 1, 1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(-1, -1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, -1, -1)

        ' Top face
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(-1, 1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, 1, -1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(1, 1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, 1, 1)

        ' Bottom face
        GL.Color3(1.0, 0.0, 0.0)
        GL.Vertex3(-1, -1, 1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(1, -1, 1)
        GL.Color3(0.0, 0.0, 0.7)
        GL.Vertex3(1, -1, -1)
        GL.Color3(0.0, 1.0, 0.0)
        GL.Vertex3(-1, -1, -1)

        GL.End()

        SwapBuffers()
    End Sub

    Public Shared Sub Main()
        Dim app As New GLShadedCube
        app.Run(60, 60)
    End Sub
End Class