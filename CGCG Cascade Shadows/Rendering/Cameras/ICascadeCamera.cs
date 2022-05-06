using SharpDX;

using System.Collections.Generic;

namespace FR.CascadeShadows.Rendering;

public interface ICascadeCamera : ICamera
{
    int Cascades { get; }
    Matrix GetProjectionCascade(int cascade);

    public static readonly Vector3[] Corners = new Vector3[8];

    static ICascadeCamera()
    {
        for (int i = 0; i < 8; i++)
            Corners[i] = new Vector3((i % 2) * 2 - 1, ((i >> 1) % 2) * 2 - 1, (i >> 2) % 2);
    }

    IEnumerable<Vector3> GetCorners(int cascade)
    {
        if (cascade >= Cascades || cascade < 0)
            throw new System.ArgumentException("Cascade out of bounds", nameof(cascade));

        Matrix viewProjInv = Matrix.Invert(View * GetProjectionCascade(cascade));
        foreach (var c in Corners)
            yield return Vector3.TransformCoordinate(c, viewProjInv);
    }
}