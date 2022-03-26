using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public static class RasterizerStates
{
    /// <summary>
    /// Draws only front facing triangles.
    /// </summary>
    public static readonly RasterizerState1 Default = new(Devices.Device3D, DefaultDesc);
    /// <summary>
    /// Draws only back facing triangles.
    /// </summary>
    public static readonly RasterizerState1 Inverted = new(Devices.Device3D, InversedDesc);
    public static readonly RasterizerState1 NoCulling = new(Devices.Device3D, NoCullingDesc);
    public static readonly RasterizerState1 WireFrame = new(Devices.Device3D, WireFrameDesc);

    static RasterizerStateDescription1 DefaultDesc =>
        new()
        {
            CullMode = CullMode.Back,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 InversedDesc =>
        new()
        {
            CullMode = CullMode.Front,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 NoCullingDesc =>
        new()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Solid,
            IsFrontCounterClockwise = true
        };

    static RasterizerStateDescription1 WireFrameDesc =>
        new()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Wireframe,
            IsFrontCounterClockwise = true
        };
}