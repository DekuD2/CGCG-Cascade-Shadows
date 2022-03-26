using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Loaders;

public record VertexShaderInfo(VertexShader Shader, ShaderBytecode Bytecode)
{
    public class Loader : FileLoader<VertexShaderInfo>
    {
        public override VertexShaderInfo? Load(string uri)
        {
            var result = ShaderLoader.CompileFile(uri, "Main", "vs_5_0");

            if (result.HasErrors)
                return null;

            return new VertexShaderInfo(new VertexShader(Devices.Device3D, result.Bytecode), result.Bytecode);
        }
    }
}