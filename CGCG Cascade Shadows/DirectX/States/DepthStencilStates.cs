using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public static class DepthStencilStates
{
    public static readonly DepthStencilState Default = new(Devices.Device3D, DefaultDesc);
    public static readonly DepthStencilState Ignore = new(Devices.Device3D, IgnoreDesc);
    public static readonly DepthStencilState Override = new(Devices.Device3D, OverrideDesc);
    public static readonly DepthStencilState LightVolume = new(Devices.Device3D, LightVolumeDesc);
    public static readonly DepthStencilState Clear = new(Devices.Device3D, ClearDesc);
    public static readonly DepthStencilState NoWrite = new(Devices.Device3D, NoWriteDesc);

    static DepthStencilStateDescription DefaultDesc
        => new ()
        {
            IsDepthEnabled = true,
            DepthComparison = Comparison.LessEqual,
            DepthWriteMask = DepthWriteMask.All,
            IsStencilEnabled = false,
            StencilReadMask = 0xff,
            StencilWriteMask = 0xff,
            FrontFace =
            {
                Comparison = Comparison.Always,
                FailOperation = StencilOperation.Keep,
                PassOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Increment
            },
            BackFace =
            {
                Comparison = Comparison.Always,
                FailOperation = StencilOperation.Keep,
                PassOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Decrement
            }
        };

    static DepthStencilStateDescription IgnoreDesc
        => DefaultDesc with
        {
            IsDepthEnabled = false,
            DepthComparison = Comparison.Always,
            DepthWriteMask = DepthWriteMask.Zero
        };

    static DepthStencilStateDescription OverrideDesc
        => DefaultDesc with
        {
            DepthComparison = Comparison.Always,
        };

    static DepthStencilStateDescription LightVolumeDesc
        => DefaultDesc with
        {
            IsStencilEnabled = true,
            FrontFace = new DepthStencilOperationDescription()
            {
                Comparison = Comparison.Greater,
                FailOperation = StencilOperation.Keep,
                PassOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Keep
            },
            BackFace = new DepthStencilOperationDescription()
            {
                Comparison = Comparison.Always,
                FailOperation = StencilOperation.Keep,
                PassOperation = StencilOperation.Keep,
                DepthFailOperation = StencilOperation.Keep
            }
        };

    static DepthStencilStateDescription ClearDesc
        => DefaultDesc with
        {
            DepthComparison = Comparison.Always,
            IsStencilEnabled = true,
            FrontFace = new DepthStencilOperationDescription()
            {
                Comparison = Comparison.Always,
                FailOperation = StencilOperation.Zero,
                PassOperation = StencilOperation.Zero,
                DepthFailOperation = StencilOperation.Zero
            },
            BackFace = new DepthStencilOperationDescription()
            {
                Comparison = Comparison.Always,
                FailOperation = StencilOperation.Zero,
                PassOperation = StencilOperation.Zero,
                DepthFailOperation = StencilOperation.Zero
            }
        };

    static DepthStencilStateDescription NoWriteDesc
        => DefaultDesc with
        {
            DepthWriteMask = DepthWriteMask.Zero
        };
}