using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace FR.CascadeShadows;

public abstract partial class Light
{
    static DepthStencilViewDescription DepthStencilViewDescription = new()
    {
        Format = Format.D24_UNorm_S8_UInt,
        Dimension = DepthStencilViewDimension.Texture2D,
        Texture2D = new DepthStencilViewDescription.Texture2DResource()
        {
            MipSlice = 0
        }
    };

    static ShaderResourceViewDescription ShaderResourceViewDescription = new()
    {
        Format = Format.R24_UNorm_X8_Typeless,
        Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
        Texture2D = new ShaderResourceViewDescription.Texture2DResource()
        {
            MipLevels = 1,
            MostDetailedMip = 0
        }
    };

    public static (RenderingTexture, Viewport) CreateDepthTexture(int width, int height)
    {
        Viewport viewport = new(0, 0, width, height, 0f, 1f);
        RenderingTexture renderingTexture = new(ShaderUsage.DepthStencil | ShaderUsage.ShaderResource)
        {
            DepthStencilViewDesc = DepthStencilViewDescription,
            ShaderResourceViewDesc = ShaderResourceViewDescription
        };

        renderingTexture.ReplaceTexture(new Texture2D(Devices.Device3D, new()
        {
            Width = width,
            Height = height,
            Format = Format.R24G8_Typeless,
            SampleDescription = new SampleDescription(count: 1, quality: 0),
            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        }));

        return (renderingTexture, viewport);
    }
}
