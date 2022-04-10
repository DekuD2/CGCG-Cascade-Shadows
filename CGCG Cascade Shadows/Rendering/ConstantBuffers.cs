using SharpDX.Direct3D11;
using SharpDX;

namespace FR.CascadeShadows.Rendering;

public enum PassType
{
    Normal = 0,
    Shadows = 1
}

public static class ConstantBuffers
{
    static ICamera? currentCamera;
    static Matrix currentViewProjection = Matrix.Identity;

    public static void UpdateCamera(DeviceContext1 context, ICamera camera, float aspect, PassType pass)
    {
        currentCamera = camera;
        currentViewProjection = currentCamera.View * currentCamera.Projection(aspect);

        context.MapSubresource(Camera, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(camera.Position);
        stream.Write((int)pass);
        context.UnmapSubresource(Camera, 0);
    }

    public static readonly Buffer Transform = new Buffer(
        Devices.Device3D,
        Utilities.SizeOf<Matrix>() * 2,
        ResourceUsage.Dynamic,
        BindFlags.ConstantBuffer,
        CpuAccessFlags.Write,
        ResourceOptionFlags.None,
        0);

    public static readonly Buffer Camera = new Buffer(
        Devices.Device3D,
        Utilities.SizeOf<Matrix>() * 2,
        ResourceUsage.Dynamic,
        BindFlags.ConstantBuffer,
        CpuAccessFlags.Write,
        ResourceOptionFlags.None,
        0);

    public static void UpdateTransform(DeviceContext1 context, Matrix world)
    {
        context.MapSubresource(Transform, MapMode.WriteDiscard, MapFlags.None, out var stream);
        stream.Write(Matrix.Transpose(world));
        stream.Write(Matrix.Transpose(currentViewProjection));
        context.UnmapSubresource(Transform, 0);
    }
}
