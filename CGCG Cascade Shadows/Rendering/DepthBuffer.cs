using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace FR.CascadeShadows.Rendering;

public class DepthBuffer
{
    readonly RenderingTexture output;
    readonly RenderingTexture depthBuffer = new(ShaderUsage.DepthStencil | ShaderUsage.ShaderResource);

    public DepthBuffer(RenderingTexture output)
    {
        this.output = output;

        depthBuffer.DepthStencilViewDesc = new()
        {
            Dimension = DepthStencilViewDimension.Texture2D, // Should depend on SwapChain.Description.SampleDescription.Count (> 1 => multisampled)
            Format = Format.D24_UNorm_S8_UInt,
        };

        depthBuffer.ShaderResourceViewDesc = new()
        {
            Dimension = ShaderResourceViewDimension.Texture2D,
            Format = Format.R24_UNorm_X8_Typeless,
            Texture2D =
            {
                MipLevels = -1,
                MostDetailedMip = 0
            }
        };

        output.Resized += Resize;
        Resize();
    }

    public Texture2D Texture => depthBuffer.Texture2D;
    public DepthStencilView DepthStencilView => depthBuffer.DepthStencilView!;
    public ShaderResourceView ShaderResourceView => depthBuffer.ShaderResourceView!;

    private void Resize()
    {
        depthBuffer.ReplaceTexture(new Texture2D(
            Devices.Device3D,
            new Texture2DDescription()
            {
                //Format = Format.D32_Float_S8X24_UInt,
                Format = Format.R24G8_Typeless,
                ArraySize = 1,
                MipLevels = 1,
                Width = output.Description.Width,
                Height = output.Description.Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default
            }));
    }
}