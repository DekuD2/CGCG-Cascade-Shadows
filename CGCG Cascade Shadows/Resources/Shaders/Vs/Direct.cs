using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders.Vs;
public static class Direct
{
    public static readonly VertexShader Shader = ResourceCache.Get<VertexShader>(@"Shaders\Vs\direct.hlsl");
    //public static readonly InputLayout InputLayout = new(Devices.Device3D, );

    public static readonly InputLayout InputLayout = new(Devices.Device3D,
        ResourceCache.Get<ShaderBytecode>(@"Shaders\Vs\direct.hlsl"),
        new InputElement[]
        {
            new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0)
        });

    public static void Set(DeviceContext1 context)
    {
        //context.InputAssembler.InputLayout = Shader.InputLayout;
        //context.InputAssembler.SetDescriptor(Shader.Descriptor);

        //context.VertexShader.Set(Shader.Shader);

        context.InputAssembler.InputLayout = InputLayout;
        context.VertexShader.Set(Shader);
    }

    // Mesh->Buffers method that will be useful?
}
