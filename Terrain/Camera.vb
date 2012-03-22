Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Class Camera
    Public pos As Vector3

    ' Orientação no eixo X
    Public pitch As Single

    ' Orientação no eixo Y
    Public yaw As Single

    ' Orientação no eixo Z
    Public roll As Single

    Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        Me.pos = New Vector3(x, y, z)
    End Sub

    Public Sub Render()
        GL.Rotate(-pitch, 1, 0, 0)
        GL.Rotate(-yaw, 0, 1, 0)
        GL.Rotate(-roll, 0, 0, 1)
        GL.Translate(-pos.X, -pos.Y, -pos.Z)
    End Sub

    Public Sub Walk(ByVal delta As Single)
        pos.Z -= delta * Math.Cos(MathHelper.DegreesToRadians(yaw))
        pos.X -= delta * Math.Sin(MathHelper.DegreesToRadians(yaw))
    End Sub
End Class
