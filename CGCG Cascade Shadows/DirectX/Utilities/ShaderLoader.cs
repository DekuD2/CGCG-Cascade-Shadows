//using SharpDX.D3DCompiler;

//using System.Diagnostics;

//namespace FR.CascadeShadows;
//public static class ShaderLoader
//{
//    public static CompilationResult CompileFile(string file, string func, string profile)
//    {
//        var result = ShaderBytecode.CompileFromFile(file, func, profile);

//        if (result.Bytecode == null)
//        {
//            Log.Warning("Shader compiler error:");
//            Log.Indent();
//            Log.WriteLine(result.Message);
//            Log.Unindent();
//            return result;
//        }
//        else if (!string.IsNullOrEmpty(result.Message))
//        {
//            Log.Warning("PShader message:");
//            Log.Indent();
//            Log.WriteLine(result.Message);
//            Log.Unindent();
//        }

//        return result;
//    }

//    public static CompilationResult Compile(string code, string func, string profile)
//    {
//        var result = ShaderBytecode.Compile(code, func, profile);

//        if (result.Bytecode == null)
//        {
//            Log.Warning("Shader compile error:");
//            Log.Indent();
//            Log.WriteLine(result.Message);
//            Log.Unindent();
//            return result;
//        }
//        else if (!string.IsNullOrEmpty(result.Message))
//        {
//            Log.Warning("Shader message:");
//            Log.Indent();
//            Log.WriteLine(result.Message);
//            Log.Unindent();
//        }

//        return result;
//    }
//}
