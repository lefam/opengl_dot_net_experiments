Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Class Camera

    Protected m_pos As Vector3
    Protected m_at As Vector3
    Protected m_up As Vector3
    Protected m_right As Vector3
    Protected m_pitch As Single
    Protected m_yaw As Single

    Public Sub New(ByVal pos As Vector3, ByVal at As Vector3)
        Me.m_pos = pos
        Me.m_at = At
    End Sub

    Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        Me.New(New Vector3(x, y, z), New Vector3(0.0, 0.0, -1.0))
    End Sub

    Public Sub Pitch(ByVal amount As Single)
        m_pitch += amount
        Dim mat As Matrix4 = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(m_pitch))
        m_at = Vector3.TransformVector(m_at, mat)
    End Sub

    Public Sub Walk(ByVal delta As Single)
        'm_at.Normalize()
        m_pos.X += m_at.X * delta
        m_pos.Y += 0.0
        m_pos.Z += m_at.Z * delta
    End Sub

    Public ReadOnly Property At() As Vector3
        Get
            Return m_at
        End Get
    End Property

    Public ReadOnly Property Position() As Vector3
        Get
            Return m_pos
        End Get
    End Property

    Public Sub Render()
        GL.Translate(-m_pos.X, -m_pos.Y, -m_pos.Z)
        GL.Rotate(-m_pitch, 0, 1, 0)
    End Sub
End Class
