using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public static class SamplerStates
{
    public static SamplerState Default { get; private set; } = new SamplerState(Devices.Device3D, DefaultDesc);
    public static SamplerState Anisotropic { get; private set; } = new SamplerState(Devices.Device3D, AnisotropicDesc);

    static SamplerStateDescription DefaultDesc
        => new()
        {
            Filter = Filter.MinMagMipLinear,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap
        };

    static SamplerStateDescription AnisotropicDesc
        => new()
        {
            Filter = Filter.Anisotropic,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap
        };
}