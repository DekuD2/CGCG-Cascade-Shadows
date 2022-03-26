using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Loaders;

public class PixelShaderLoader : FileLoader<PixelShader>
{
    public override PixelShader? Load(string uri)
    {
        var result = ShaderLoader.CompileFile(uri, "Main", "ps_5_0");

        if (result.HasErrors)
            return null;

        return new PixelShader(Devices.Device3D, result.Bytecode);
    }
}
