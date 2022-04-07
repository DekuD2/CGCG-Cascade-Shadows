using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class Ambient
{
    static readonly PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\ambient.hlsl");

    static readonly Buffer ColorBuffer = new Buffer(Devices.Device3D, 16, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

    public static void Set(DeviceContext1 context)
    {
        context.PixelShader.Set(Shader);
        context.PixelShader.SetConstantBuffer(0, ColorBuffer);
        context.PixelShader.SetSampler(0, SamplerStates.Default);
    }

    public static void SetParameters(DeviceContext1 context, ref SharpDX.Color color)
    {
        var c3 = color.ToColor3();

        context.MapSubresource(ColorBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(c3);
        context.UnmapSubresource(ColorBuffer, 0);
    }
}
