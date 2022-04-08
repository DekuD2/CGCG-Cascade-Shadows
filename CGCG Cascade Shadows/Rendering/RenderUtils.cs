using SharpDX;
using SharpDX.Direct3D11;

using System.Linq;

namespace FR.CascadeShadows.Rendering;

public static class RenderUtils
{
    public static void CopyTexture(DeviceContext1 context,
        ShaderResourceView source,
        RenderTargetView target,
        int width,
        int height,
        Color? multiplyer = null)
    {
        var viewport = new Viewport(0, 0, width, height, minDepth: 0, maxDepth: 1);

        context.ClearState();

        // Target
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(null, target);

        // Rasterizer
        context.Rasterizer.State = RasterizerStates.Default;

        // Blending and DepthStencil settings
        context.OutputMerger.SetBlendState(BlendStates.Default);
        context.OutputMerger.SetDepthStencilState(DepthStencilStates.Ignore);

        // Set shaders
        Resources.Shaders.Fullscreen.Prepare(context);
        Resources.Shaders.Ps.Copy.Set(context);
        context.PixelShader.SetShaderResources(0, source);

        // Set params
        Resources.Shaders.Ps.Copy.SetParameters(context, multiplyer);

        // Draw
        Resources.Shaders.Fullscreen.Draw(context);
    }
}