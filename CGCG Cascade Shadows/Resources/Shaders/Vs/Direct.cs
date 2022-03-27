using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;

public static class Direct
{
    public static readonly VertexShaderInfo Shader = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\direct.hlsl");

    public static void Set(DeviceContext1 context)
    {
        //context.InputAssembler.InputLayout = Shader.InputLayout;
        //context.InputAssembler.SetDescriptor(Shader.Descriptor);

        //context.VertexShader.Set(Shader.Shader);

        //context.InputAssembler.InputLayout = InputLayout;
        context.VertexShader.Set(Shader);
    }

    // Mesh->Buffers method that will be useful?
}
