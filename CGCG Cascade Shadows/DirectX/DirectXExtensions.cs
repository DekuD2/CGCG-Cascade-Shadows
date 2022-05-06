using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows;

public static class DirectXExtensions
{
    static VertexShaderInfo? currentVertexShaderInfo;
    static GeometryData? currentGeometryData;

    // Returns object because it's only supposed to be used for comparing whether VertexBuffers are up to date by SetGeometry
    // Otherwise this should never be used
    public static object? CurrentVertexShaderInfo => currentVertexShaderInfo;

    public static void Set(this VertexShaderStage vertexShaderStage, VertexShaderInfo vertexShaderInfo)
    {
        vertexShaderStage.Set(vertexShaderInfo.Shader);
        currentVertexShaderInfo = vertexShaderInfo;
    }

    /// <summary>
    /// Sets vertex and index buffers from the <paramref name="geometry"/> based on the current <see cref="VertexShaderInfo"/>.
    /// </summary>
    /// <param name="inputAssemblerStage"></param>
    /// <param name="geometry"></param>
    /// <exception cref="Exception"></exception>
    public static void InferBuffers(this InputAssemblerStage inputAssemblerStage, GeometryData geometry)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(InferBuffers)} called before {nameof(VertexShaderInfo)} context was set.");

        inputAssemblerStage.SetVertexBuffers(0, currentVertexShaderInfo.GetVertexBuffersOrEmpty(geometry));
        inputAssemblerStage.SetIndexBuffer(geometry.Indices, SharpDX.DXGI.Format.R32_UInt, 0);
        SetActiveGeometry(geometry);
    }

    /// <param name="separateChannels">True if each input parameter uses a separate buffer slot (True for <see cref="Mesh"/>). False otherwise.</param>
    public static void InferInputLayout(this InputAssemblerStage inputAssemblerStage, bool separateChannels = true)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(InferInputLayout)} called before {nameof(VertexShaderInfo)} context was set.");

        Devices.Context3D.InputAssembler.InputLayout = separateChannels
            ? currentVertexShaderInfo.InputLayoutSeparate.Value
            : currentVertexShaderInfo.InputLayoutAggregated.Value;
    }

    public static VertexBufferBinding[] FindVertexBuffers(this VertexShaderStage vertexShaderStage, GeometryData geometry)
    {
        if (currentVertexShaderInfo == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(FindVertexBuffers)} called before {nameof(VertexShaderInfo)} context was set.");

        return currentVertexShaderInfo.GetVertexBuffersOrEmpty(geometry);
    }

    /// <summary>
    /// Draws the last geometry set by <see cref="InferBuffers(InputAssemblerStage, GeometryData)"/> or <see cref="GeometryData.Set(GeometryData)"/>.
    /// </summary>
    public static void DrawLastGeometry(this DeviceContext1 context)
    {
        if (currentGeometryData == null)
            throw new Exception($"{nameof(DirectXExtensions)}: {nameof(DrawLastGeometry)} called before {nameof(GeometryData)} context was set.");

        if (currentGeometryData.IndexCount != 0)
            context.DrawIndexed(currentGeometryData.IndexCount, 0, 0);
        else
            context.Draw(currentGeometryData.VertexCount, 0);
    }

    internal static void SetActiveGeometry(GeometryData geometry)
        => currentGeometryData = geometry;
    }
