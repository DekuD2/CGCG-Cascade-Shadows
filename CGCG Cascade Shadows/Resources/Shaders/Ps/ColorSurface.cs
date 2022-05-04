using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class ColorSurface
{
    static readonly PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\colorSurface.hlsl");

    static readonly Buffer ColorBuffer = new(Devices.Device3D,
        sizeof(float) * 8,
        ResourceUsage.Dynamic,
        BindFlags.ConstantBuffer,
        CpuAccessFlags.Write,
        ResourceOptionFlags.None,
        0);

    public static void Set(DeviceContext1 context)
    {
        if (context.IsShadowMode()) return;

        context.PixelShader.Set(Shader);
        context.PixelShader.SetConstantBuffer(0, ColorBuffer);
    }

    public static void SetParameters(DeviceContext1 context, Color3 diffuse, Color3 emission, float gloss, float specular)
    {
        if (context.IsShadowMode()) return;

        context.MapSubresource(ColorBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(diffuse);
        stream.Write(gloss);
        stream.Write(emission);
        stream.Write(specular);
        context.UnmapSubresource(ColorBuffer, 0);
    }
}