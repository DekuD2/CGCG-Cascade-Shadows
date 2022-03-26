using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;
public static class Direct
{
    public static VertexShader Shader = ResourceCache.Get<VertexShader>(@"Shaders\Vs\direct.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.InputAssembler.InputLayout = Shader.InputLayout;
        context.InputAssembler.SetDescriptor(Shader.Descriptor);

        context.VertexShader.Set(Shader.Shader);
    }

    // Mesh->Buffers method that will be useful?
}
