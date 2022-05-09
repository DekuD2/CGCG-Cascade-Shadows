using SharpDX;

using System.Collections.Generic;

namespace FR.CascadeShadows.Rendering;

public static class CameraUtils
{
    public static readonly Vector3[] Corners = new Vector3[8];

    static CameraUtils()
    {
        for (int i = 0; i < 8; i++)
            Corners[i] = new Vector3((i % 2) * 2 - 1, ((i >> 1) % 2) * 2 - 1, (i >> 2) % 2);
    }

    public static IEnumerable<Vector3> GetCorners(Matrix view, Matrix projection)
    {
        Matrix viewProjInv = Matrix.Invert(view * projection);
        foreach (var c in Corners)
            yield return Vector3.TransformCoordinate(c, viewProjInv);
    }
}