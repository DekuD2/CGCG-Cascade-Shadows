using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using FR.CascadeShadows.Rendering.Meshes;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FR.CascadeShadows.Resources.Loaders;

public delegate VertexBufferBinding VbbGetter(GeometryData geometry);
public delegate VertexBufferBinding[] MeshInputGetter(GeometryData geometry);

public record VertexShaderInfo(VertexShader Shader,
    ShaderBytecode Bytecode,
    Lazy<InputLayout> InputLayoutSeparate,
    Lazy<InputLayout> InputLayoutAggregated,
    MeshInputGetter MeshInputGetter)
{
    public VertexBufferBinding[] GetVertexBuffersOrEmpty(GeometryData geometry)
    {
        try
        {
            return MeshInputGetter(geometry);
        }
        catch
        {
            Debug.WriteLine($"Mesh '{geometry.Name}' is missing some vertex information required by the material. Returning null.");
            return Array.Empty<VertexBufferBinding>();
        }
    }

    public class Loader : FileLoader<VertexShaderInfo>
    {
        public override VertexShaderInfo? Load(string uri)
        {
            var result = ShaderLoader.CompileFile(uri, "Main", "vs_5_0");

            if (result.HasErrors)
                return null;

            using var analysis = new ShaderAnalysis(result);

            var elements = analysis.GetInputElements();

            VbbGetter[] getters = GetIndividualGetters(elements);

            VertexBufferBinding[] GetVertexBuffers(GeometryData geometry)
                => getters.Select(g => g(geometry)).ToArray();

            return new VertexShaderInfo(new VertexShader(Devices.Device3D, result.Bytecode),
                result.Bytecode,
                new(() => new InputLayout(Devices.Device3D, result.Bytecode, elements)),
                new(() => new InputLayout(Devices.Device3D, result.Bytecode, elements.Select(x => x with { Slot = 0 }).ToArray())),
                GetVertexBuffers);
        }

        static VbbGetter[] GetIndividualGetters(InputElement[] elements)
        {
            var getters = new VbbGetter[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].SemanticName.StartsWith("SV_")) // Skip system values
                    continue;

                getters[i] = elements[i].SemanticName switch
                {
                    "POSITION" => m => m.Positions ?? throw new FormatException("Mesh does not contain: " + elements[i].SemanticName),
                    "NORMAL" => m => m.Normals ?? throw new FormatException("Mesh does not contain: " + elements[i].SemanticName),
                    "TEXCOORD" => m => m.TexCoordChannels[0],
                    "TANGENT" => m => m.Tangents ?? throw new FormatException("Mesh does not contain: " + elements[i].SemanticName),
                    "BINORMAL" => m => m.Binormals ?? throw new FormatException("Mesh does not contain: " + elements[i].SemanticName),
                    "COLOR" => m => m.ColorChannelCount >= 1 ? m.ColorChannels[0] : throw new FormatException("Mesh does not contain: " + elements[i].SemanticName),
                    _ => throw new FormatException("Incorrect (or not implemented yet) format: " + elements[i].SemanticName)
                };
            }

            return getters;
        }
    }
}