using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System;

namespace FR.CascadeShadows.Resources.Loaders;

public class ShaderAnalysis : IDisposable
{
    public int HighestConstantBufferIdx { get; private set; } = -1;
    public int HighestTextueIdx { get; private set; } = -1;
    public int HighestSamplerIdx { get; private set; } = -1;

    readonly ShaderReflection reflection;
    ShaderDescription Description => reflection.Description;

    public ShaderAnalysis(ShaderBytecode byteCode)
    {
        this.reflection = new ShaderReflection(byteCode);

        for (int i = 0; i < Description.BoundResources; i++)
        {
            var res = reflection.GetResourceBindingDescription(i);
            switch (res.Type)
            {
                //case ShaderInputType.TextureBuffer:
                //    break;
                case ShaderInputType.ConstantBuffer:
                    if (HighestConstantBufferIdx < res.BindPoint)
                        HighestConstantBufferIdx = res.BindPoint;
                    break;
                case ShaderInputType.Texture:
                    if (HighestTextueIdx < res.BindPoint)
                        HighestTextueIdx = res.BindPoint;
                    break;
                case ShaderInputType.Sampler:
                    if (HighestSamplerIdx < res.BindPoint)
                        HighestSamplerIdx = res.BindPoint;
                    break;
            }
        }
    }

    public InputElement[] GetInputElements(bool separateSlots = true)
    {
        int inParams = Description.InputParameters;
        InputElement[] inputElements = new InputElement[inParams];
        for (int i = 0; i < inParams; i++)
        {
            var paramDesc = reflection.GetInputParameterDescription(i);
            inputElements[i] = new InputElement(
                paramDesc.SemanticName,
                paramDesc.SemanticIndex,
                GetFormat(paramDesc),
                separateSlots ? i : 0);
        }
        return inputElements;
    }

    const RegisterComponentMaskFlags flags_r = RegisterComponentMaskFlags.ComponentX;
    const RegisterComponentMaskFlags flags_rg = flags_r | RegisterComponentMaskFlags.ComponentY;
    const RegisterComponentMaskFlags flags_rgb = flags_rg | RegisterComponentMaskFlags.ComponentZ;
    const RegisterComponentMaskFlags flags_rgba = flags_rgb | RegisterComponentMaskFlags.ComponentW;

    public static Format GetFormat(ShaderParameterDescription desc)
        => desc.ComponentType switch
        {
            RegisterComponentType.Unknown => Format.Unknown,
            RegisterComponentType.UInt32 =>
                desc.UsageMask switch
                {
                    flags_r => Format.R32_UInt,
                    flags_rg => Format.R32G32_UInt,
                    flags_rgb => Format.R32G32B32_UInt,
                    flags_rgba => Format.R32G32B32A32_UInt,
                    _ => throw new FormatException("Unrecognized shader input format components.")
                },
            RegisterComponentType.SInt32 =>
                desc.UsageMask switch
                {
                    flags_r => Format.R32_SInt,
                    flags_rg => Format.R32G32_SInt,
                    flags_rgb => Format.R32G32B32_SInt,
                    flags_rgba => Format.R32G32B32A32_SInt,
                    _ => throw new FormatException("Unrecognized shader input format components.")
                },
            RegisterComponentType.Float32 =>
                desc.UsageMask switch
                {
                    flags_r => Format.R32_Float,
                    flags_rg => Format.R32G32_Float,
                    flags_rgb => Format.R32G32B32_Float,
                    flags_rgba => Format.R32G32B32A32_Float,
                    _ => throw new FormatException("Unrecognized shader input format components.")
                },
            _ => throw new FormatException("Unrecognized shader input format.")
        };

    public PrimitiveTopology GeometryShaderInputPrimitive
        => reflection.GeometryShaderSInputPrimitive switch
            {
                InputPrimitive.Undefined => PrimitiveTopology.Undefined,
                InputPrimitive.Point => PrimitiveTopology.PointList,
                InputPrimitive.Line => PrimitiveTopology.LineList,
                InputPrimitive.Triangle => PrimitiveTopology.TriangleList,
                InputPrimitive.LineWithAdjacency => PrimitiveTopology.LineListWithAdjacency,
                InputPrimitive.TriangleWithAdjacency => PrimitiveTopology.TriangleListWithAdjacency,
                var inputPrim => inputPrim - InputPrimitive.PatchWith1ControlPoints + PrimitiveTopology.PatchListWith1ControlPoints,
            };

    public void Dispose() => reflection?.Dispose();
}
