using SharpDX.Direct3D11;

namespace FR.CascadeShadows;
public static class SamplerStates
{
    public static SamplerState Default { get; private set; } = new SamplerState(Devices.Device3D, defaultDesc);
    public static SamplerState Anisotropic { get; private set; } = new SamplerState(Devices.Device3D, anisotropicDesc);

    static SamplerStateDescription defaultDesc
        => new SamplerStateDescription()
        {
            Filter = Filter.MinMagMipLinear,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap
        };

    static SamplerStateDescription anisotropicDesc
        => new SamplerStateDescription()
        {
            Filter = Filter.Anisotropic,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap
        };
}