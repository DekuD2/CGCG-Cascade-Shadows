using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public static class SamplerStates
{
    public static SamplerState Default { get; private set; } = new SamplerState(Devices.Device3D, DefaultDesc);
    public static SamplerState Shadow { get; private set; } = new SamplerState(Devices.Device3D, ShadowDesc);
    public static SamplerState Point { get; private set; } = new SamplerState(Devices.Device3D, PointDesc);
    public static SamplerState ShadowComp { get; private set; } = new SamplerState(Devices.Device3D, ShadowCmpDesc);
    public static SamplerState Anisotropic { get; private set; } = new SamplerState(Devices.Device3D, AnisotropicDesc);

    static SamplerStateDescription DefaultDesc
        => new()
        {
            Filter = Filter.MinMagMipLinear,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
        };

    static SamplerStateDescription AnisotropicDesc
        => new()
        {
            Filter = Filter.Anisotropic,
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap
        };

    static SamplerStateDescription ShadowDesc
        => new()
        {
            Filter = Filter.MinMagMipLinear,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
        };

    static SamplerStateDescription PointDesc
        => new()
        {
            Filter = Filter.MinMagMipPoint,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
        };

    static SamplerStateDescription ShadowCmpDesc
        => new()
        {
            Filter = Filter.MinMagLinearMipPoint,
            AddressU = TextureAddressMode.Border,
            AddressV = TextureAddressMode.Border,
            AddressW = TextureAddressMode.Border,
            ComparisonFunction = Comparison.Less,
            BorderColor = new(0, 0, 0, 1),
            MaximumAnisotropy = 0,
            MipLodBias = 0,
            MinimumLod = 0,
            MaximumLod = 0
        };
}