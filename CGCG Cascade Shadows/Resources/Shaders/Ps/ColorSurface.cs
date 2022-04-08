using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class ColorSurface
{
    static readonly PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\colorSurface.hlsl");

    static readonly Buffer ColorBuffer = new(Devices.Device3D,
        sizeof(float) * 4,
        ResourceUsage.Dynamic,
        BindFlags.ConstantBuffer,
        CpuAccessFlags.Write,
        ResourceOptionFlags.None,
        0);

    public static void Set(DeviceContext1 context)
    {
        context.PixelShader.Set(Shader);
        context.PixelShader.SetConstantBuffer(0, ColorBuffer);
    }

    public static void SetParameters(DeviceContext1 context, Color4 color)
    {
        context.MapSubresource(ColorBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(color);
        context.UnmapSubresource(ColorBuffer, 0);
    }
}
