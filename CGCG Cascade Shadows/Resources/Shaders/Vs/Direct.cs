using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;

public static class Direct
{
    public static readonly VertexShaderInfo ShaderInfo = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\direct.hlsl");

    public static void Set(DeviceContext1 context) 
        => context.VertexShader.Set(ShaderInfo);
}
