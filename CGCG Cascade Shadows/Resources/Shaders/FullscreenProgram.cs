using FR.CascadeShadows.Resources.Loaders;

using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders;

public static class Fullscreen
{
    static readonly VertexShaderInfo VertexShader = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\fullscreen.hlsl");

    public static void Prepare(DeviceContext1 context)
    {
        context.VertexShader.Set(VertexShader);
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
    }

    public static void Draw(DeviceContext1 context)
    {
        context.Draw(4, 0);
    }
}
