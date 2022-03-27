using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows;

public static class DirectXExtensions
{
    static VertexShaderInfo? currentVertexShaderInfo;
    public static void Set(this VertexShaderStage vertexShaderStage, VertexShaderInfo vertexShaderInfo)
    {
        vertexShaderStage.Set(vertexShaderInfo.Shader);
        currentVertexShaderInfo = vertexShaderInfo;
    }

    public static void InferVertexBuffers(this InputAssemblerStage inputAssemblerStage, GeometryData geometry)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(InferVertexBuffers)} called before {nameof(VertexShaderInfo)} context was set.");

        inputAssemblerStage.SetVertexBuffers(0, currentVertexShaderInfo.GetVertexBuffersOrEmpty(geometry));
    }

    public static void InferInputLayout(this InputAssemblerStage inputAssemblerStage, bool separateChannels = true)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(InferInputLayout)} called before {nameof(VertexShaderInfo)} context was set.");

        Devices.Context3D.InputAssembler.InputLayout = separateChannels
            ? currentVertexShaderInfo.InputLayoutSeparate.Value
            : currentVertexShaderInfo.InputLayoutAggregated.Value;
    }

    public static VertexBufferBinding[] FindVertexBuffers(this VertexShaderStage inputAssemblerStage, GeometryData geometry)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(FindVertexBuffers)} called before {nameof(VertexShaderInfo)} context was set.");

        return currentVertexShaderInfo.GetVertexBuffersOrEmpty(geometry);
    }
}
