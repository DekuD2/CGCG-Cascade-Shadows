using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class Copy
{
    static readonly PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\copy.hlsl");

    static readonly Buffer CopyBuffer = new(Devices.Device3D, 16, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

    public static void Set(DeviceContext1 context)
    {
        context.PixelShader.Set(Shader);
        context.PixelShader.SetConstantBuffer(0, CopyBuffer);
        context.PixelShader.SetSampler(0, SamplerStates.Default);
    }

    public static void SetParameters(DeviceContext1 context, SharpDX.Color? multiplier = null)
    {
        context.MapSubresource(CopyBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(multiplier?.ToColor3() ?? Color3.White);
        stream.Write(0);
        context.UnmapSubresource(CopyBuffer, 0);
    }
}
