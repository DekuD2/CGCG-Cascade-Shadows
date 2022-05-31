using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;

using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public class MeshObject
{
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;
    private readonly TransitionGate Gate = new();
    public ref bool Show => ref Gate.ShowNode;

    public Matrix Transform => Matrix.Scaling(Scale) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Position);

    RenderingInstructions instructions;
    RenderingInstructions shadowInstruction;

    public MeshObject(Mesh mesh, IMaterial material)
    {
        instructions = DeferredPipeline.SurfacePass
            .Set(material.ProgramStep)
            .Then(material.MaterialStep)
            .Then(GeometryData.Set(mesh))
            .Then(Gate)
            .ThenDraw(Draw);

        if (material is IDepthMaterial depthMat)
            shadowInstruction = DeferredPipeline.ShadowCastPass
                .Set(depthMat.DepthStep)
                .Then(GeometryData.Set(mesh))
                .Then(Gate)
                .ThenDraw(Draw);
        else
            shadowInstruction = DeferredPipeline.ShadowCastPass
                .Set(Resources.Shaders.Vs.Depth.Set)
                .Then(GeometryData.Set(mesh))
                .Then(Gate)
                .ThenDraw(Draw);
    }

    void Draw(DeviceContext1 context)
    {
        ConstantBuffers.UpdateTransform(context, Transform);
        context.DrawLastGeometry();
    }
}
