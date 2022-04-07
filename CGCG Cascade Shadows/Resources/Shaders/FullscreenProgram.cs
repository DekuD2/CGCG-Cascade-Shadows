using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders;

public static class FullscreenProgram
{
    static readonly VertexShaderInfo ShaderInfo = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\fullscreen.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.VertexShader.Set(ShaderInfo);
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
    }

    public static void Draw(DeviceContext1 context)
    {
        context.Draw(4, 0);
    }
}
