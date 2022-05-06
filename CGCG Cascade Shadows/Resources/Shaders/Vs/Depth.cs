using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;

public static class Depth
{
    static readonly VertexShaderInfo ShaderInfo = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\depth.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.VertexShader.Set(ShaderInfo);
        context.VertexShader.SetConstantBuffer(0, ConstantBuffers.Transform);
    }
}