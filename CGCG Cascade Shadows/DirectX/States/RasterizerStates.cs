using SharpDX.Direct3D11;

namespace FR.CascadeShadows;
public static class RasterizerStates
{
    /// <summary>
    /// Draws only front facing triangles.
    /// </summary>
    public static RasterizerState1 Default = new RasterizerState1(Devices.Device3D, defaultDesc);
    /// <summary>
    /// Draws only back facing triangles.
    /// </summary>
    public static RasterizerState1 Inverted = new RasterizerState1(Devices.Device3D, inversedDesc);
    public static RasterizerState1 NoCulling = new RasterizerState1(Devices.Device3D, noCullingDesc);
    public static RasterizerState1 WireFrame = new RasterizerState1(Devices.Device3D, wireFrameDesc);

    static RasterizerStateDescription1 defaultDesc =>
        new RasterizerStateDescription1()
        {
            CullMode = CullMode.Back,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 inversedDesc =>
        new RasterizerStateDescription1()
        {
            CullMode = CullMode.Front,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 noCullingDesc =>
        new RasterizerStateDescription1()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 wireFrameDesc =>
        new RasterizerStateDescription1()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Wireframe,
            IsFrontCounterClockwise = true
        };
}