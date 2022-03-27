using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows.Rendering.Meshes;

public partial class GeometryData
{
    public static TransitionStep Set(GeometryData geometry) => new SetGeometry(geometry);
    public static TransitionStep Set(Mesh mesh) => new SetGeometry(mesh.GeometryData);

    class SetGeometry : TransitionStep
    {
        GeometryData geometry;
        VertexBufferBinding[] cachedBuffers = Array.Empty<VertexBufferBinding>();
        object? lastVertexShaderInfo = null;

        public SetGeometry(GeometryData geometry)
            => this.geometry = geometry;

        public override void Enter(DeviceContext1 context)
        {
            if (DirectXExtensions.CurrentVertexShaderInfo != lastVertexShaderInfo)
            {
                lastVertexShaderInfo = DirectXExtensions.CurrentVertexShaderInfo;
                cachedBuffers = context.VertexShader.FindVertexBuffers(geometry);
            }

            //context.InputAssembler.SetIndexBuffer(geometry.Indices, SharpDX.DXGI.Format.R32_UInt, 0);
            context.InputAssembler.SetIndexBuffer(geometry.Indices, SharpDX.DXGI.Format.R32_UInt, 0);
            context.InputAssembler.SetVertexBuffers(0, cachedBuffers);
            context.InputAssembler.InferInputLayout();
            context.InputAssembler.PrimitiveTopology = geometry.PrimitiveTopology;

            DirectXExtensions.SetActiveGeometry(geometry);
        }

        public override bool Equals(object? obj)
            => obj switch
            {
                SetGeometry other => other.geometry == geometry,
                _ => false
            };

        public override int GetHashCode()
            => geometry.GetHashCode() + 17;
    }
}
