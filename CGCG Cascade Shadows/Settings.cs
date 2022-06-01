using SharpDX;
using SharpDX.Direct3D11;

using System.Runtime.InteropServices;

namespace FR.CascadeShadows;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Settings
{
    public static readonly Buffer CBuffer =
        new(Devices.Device3D,
            32,
            ResourceUsage.Dynamic,
            BindFlags.ConstantBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None,
            0);

    public int Visualise = 0;
    public int PcfMode = 4;
    public float DepthBias = 0.001f;
    public bool BlendCascades = true;
    private byte _p0 = 0;
    private byte _p1 = 0;
    private byte _p2 = 0;
    public bool Derivative = true;  

    public Settings() { }

    public void Set(DeviceContext1 context)
    {
        context.MapSubresource(CBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(this);
        context.UnmapSubresource(CBuffer, 0);
    }
}
