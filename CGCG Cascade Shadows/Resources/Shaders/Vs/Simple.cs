using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;

public static class Simple
{
    public static VertexShaderInfo ShaderInfo = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\simple.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.VertexShader.Set(ShaderInfo);
        context.VertexShader.SetConstantBuffer(0, ConstantBuffers.Transform);
    }
}