using SharpDX.D3DCompiler;

using System.Diagnostics;

namespace FR.CascadeShadows;
public static class ShaderLoader
{
    public static CompilationResult CompileFile(string file, string func, string profile)
    {
        var result = ShaderBytecode.CompileFromFile(file, func, profile);

        if (result.Bytecode == null)
        {
            Debug.WriteLine("[Shader compiler error]");
            Debug.Indent();
            Debug.WriteLine(result.Message);
            Debug.Unindent();
            return result;
        }
        else if (!string.IsNullOrEmpty(result.Message))
        {
            Debug.WriteLine("[Shader not loaded]");
            Debug.Indent();
            Debug.WriteLine(result.Message);
            Debug.Unindent();
        }

        return result;
    }

    public static CompilationResult Compile(string code, string func, string profile)
    {
        var result = ShaderBytecode.Compile(code, func, profile);

        if (result.Bytecode == null)
        {
            Debug.WriteLine("[Shader compiler error]");
            Debug.Indent();
            Debug.WriteLine(result.Message);
            Debug.Unindent();
            return result;
        }
        else if (!string.IsNullOrEmpty(result.Message))
        {
            Debug.WriteLine("[Shader not loaded]");
            Debug.Indent();
            Debug.WriteLine(result.Message);
            Debug.Unindent();
        }

        return result;
    }
}
