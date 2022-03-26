using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System;

namespace FR.CascadeShadows;
public class ShaderAnalysis : IDisposable
{
    public int HighestConstantBufferIdx { get; private set; } = -1;
    public int HighestTextueIdx { get; private set; } = -1;
    public int HighestSamplerIdx { get; private set; } = -1;

    //readonly ShaderBytecode byteCode;
    readonly ShaderReflection reflection;
    ShaderDescription Description => reflection.Description;

    public ShaderAnalysis(ShaderBytecode byteCode)
    {
        //this.byteCode = byteCode;
        reflection = new ShaderReflection(byteCode);

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

    public InputElement[] GetInputElements()
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
                i);
        }
        return inputElements;
    }

    const RegisterComponentMaskFlags flags_r = RegisterComponentMaskFlags.ComponentX;
    const RegisterComponentMaskFlags flags_rg = flags_r | RegisterComponentMaskFlags.ComponentY;
    const RegisterComponentMaskFlags flags_rgb = flags_rg | RegisterComponentMaskFlags.ComponentZ;
    const RegisterComponentMaskFlags flags_rgba = flags_rgb | RegisterComponentMaskFlags.ComponentW;

    public Format GetFormat(ShaderParameterDescription desc)
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
                    _ => throw new FormatException("Unrecognized shader input format components!")
                },
            RegisterComponentType.SInt32 =>
                desc.UsageMask switch
                {
                    flags_r => Format.R32_SInt,
                    flags_rg => Format.R32G32_SInt,
                    flags_rgb => Format.R32G32B32_SInt,
                    flags_rgba => Format.R32G32B32A32_SInt,
                    _ => throw new FormatException("Unrecognized shader input format components!")
                },
            RegisterComponentType.Float32 =>
                desc.UsageMask switch
                {
                    flags_r => Format.R32_Float,
                    flags_rg => Format.R32G32_Float,
                    flags_rgb => Format.R32G32B32_Float,
                    flags_rgba => Format.R32G32B32A32_Float,
                    _ => throw new FormatException("Unrecognized shader input format components!")
                },
            _ => throw new FormatException("Unrecognized shader input format!")
        };

    public (string name, int index)[] GetSemantics()
    {
        var inParams = Description.InputParameters;
        (string, int)[] semantics = new (string, int)[inParams];

        for (int i = 0; i < inParams; i++)
        {
            var paramDesc = reflection.GetInputParameterDescription(i);
            semantics[i] = (paramDesc.SemanticName, paramDesc.SemanticIndex);
        }

        return semantics;
    }

    public PrimitiveTopology GeometryShaderInputPrimitive
    {
        get
        {
            var inputPrim = reflection.GeometryShaderSInputPrimitive;
            switch (inputPrim)
            {
                case InputPrimitive.Undefined:
                    return PrimitiveTopology.Undefined;

                case InputPrimitive.Point:
                    return PrimitiveTopology.PointList;

                case InputPrimitive.Line:
                    return PrimitiveTopology.LineList;

                case InputPrimitive.Triangle:
                    return PrimitiveTopology.TriangleList;

                case InputPrimitive.LineWithAdjacency:
                    return PrimitiveTopology.LineListWithAdjacency;

                case InputPrimitive.TriangleWithAdjacency:
                    return PrimitiveTopology.TriangleListWithAdjacency;

                default:
                    return inputPrim
                        - InputPrimitive.PatchWith1ControlPoints
                        + PrimitiveTopology.PatchListWith1ControlPoints;
            }
        }
    }

    public void Dispose()
    {
        reflection?.Dispose();
    }
}
