using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;

public static class ParallaxDepth
{
    static readonly VertexShaderInfo Shader = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\parallaxDepth.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.VertexShader.Set(Shader);
        context.VertexShader.SetConstantBuffer(0, ConstantBuffers.Transform);
        context.VertexShader.SetConstantBuffer(1, ConstantBuffers.Camera);
    }
}
