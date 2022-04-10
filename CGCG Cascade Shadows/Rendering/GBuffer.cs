using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System.Collections.Generic;
using System.Linq;

namespace FR.CascadeShadows.Rendering;

public class GBuffer
{
    public static readonly Format[] Formats = new Format[]
    {
            Format.R32G32B32A32_Float, // float3 position : SV_Target0;
            Format.R32G32B32A32_Float, // float3 albedo : SV_Target1;
            Format.R32G32B32A32_Float, // float3 normal : SV_Target2;
            Format.R32G32B32A32_Float, // float3 emission: SV_Target3;
            Format.R32_Float,          // float specular : SV_Target4;
            Format.R32_Float,          // float gloss : SV_Target5;
            Format.R32_Float           // float alpha : SV_Target6;
    };

    readonly RenderingTexture output;
    readonly RenderingTexture[] renderingTextures;

    public GBuffer(RenderingTexture output)
    {
        this.output = output;

        renderingTextures = new RenderingTexture[Formats.Length];
        for (int i = 0; i < Formats.Length; i++)
        {
            renderingTextures[i] = new(ShaderUsage.ShaderResource | ShaderUsage.RenderTarget)
            {
                RenderTargetViewDesc = new()
                {
                    Format = Formats[i],
                    Dimension = RenderTargetViewDimension.Texture2D,
                    Texture2D = new RenderTargetViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                },

                ShaderResourceViewDesc = new()
                {
                    Format = Formats[i],
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0
                    }
                }
            };
        }

        output.Resized += Resize;
        Resize();
    }

    public IEnumerable<Texture2D> Textures => renderingTextures.Select(x => x.Texture2D);
    public IEnumerable<ShaderResourceView> ShaderResourceViews => renderingTextures.Select(rt => rt.ShaderResourceView!);
    public IEnumerable<RenderTargetView> RenderTargetViews => renderingTextures.Select(rt => rt.RenderTargetView!);

    void Resize()
    {
        for (int i = 0; i < Formats.Length; i++)
            renderingTextures[i].ReplaceTexture(new Texture2D(Devices.Device3D, new Texture2DDescription()
            {
                Width = output.Description.Width,
                Height = output.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Formats[i],
                SampleDescription = new SampleDescription(count: 1, quality: 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }));
    }
}
