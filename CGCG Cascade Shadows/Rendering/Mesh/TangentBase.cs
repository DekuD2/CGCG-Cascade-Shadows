using SharpDX;

using System.Runtime.InteropServices;

namespace FR.CascadeShadows.Rendering.Meshes;

[StructLayout(LayoutKind.Sequential)]
public struct TangentBase
{
    public Vector3 Tangent;
    public Vector3 BiTangent;

    public TangentBase(Vector3 tangent, Vector3 bitangent)
    {
        Tangent = tangent;
        BiTangent = bitangent;
    }
}
