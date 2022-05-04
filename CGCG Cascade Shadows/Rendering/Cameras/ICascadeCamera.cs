using SharpDX;

namespace FR.CascadeShadows.Rendering;

public interface ICascadeCamera
{
    int Cascades { get; }
    Matrix GetProjectionCascade(int cascade);

    public static readonly Vector3[] Corners = new Vector3[8];

    static ICascadeCamera()
    {
        for (int i = 0; i < 8; i++)
            Corners[i] = new Vector3(i % 2, (i << 1) % 2, (i << 2) % 2);
    }
}