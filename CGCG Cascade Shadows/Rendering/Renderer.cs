using SharpDX;
using SharpDX.Direct3D11;

using static FR.CascadeShadows.Devices;

namespace FR.CascadeShadows.Rendering;
public class Renderer
{
    // Renderer
    //  - camera
    //  - scene
    //  + Render()
    //      clear ViewPort
    //      setup ViewPort
    //      setup camera properties
    //      render scene

    // scene
    //   - 3D scenegraph (for matrices)

    // pass
    //   - graph of state changes and draw calls
    //   - the overall algorithm
    //   - buffer setup used for rendering
    //   * e.g. SurfacePass -> LightPass -> ForwardPass -> GizmoPass

    static Viewport viewport;
    static DepthStencilView dsv;
    static RenderTargetView rtv;

    public InnerNode ForwardPass = new(c => ForwardPass2.Enter(viewport, dsv, rtv));
}

public static class ForwardPass2
{
    public static void Enter(Viewport viewport, DepthStencilView dsv, RenderTargetView rtv)
    {
        Context3D.ClearState();

        // Target
        Context3D.Rasterizer.SetViewport(viewport);
        Context3D.OutputMerger.SetRenderTargets(dsv, rtv);

        // Rasterizer
        Context3D.Rasterizer.State = RasterizerStates.Default;

        // Blending and DepthStencil settings
        Context3D.OutputMerger.SetBlendState(BlendStates.Transparency);
        Context3D.OutputMerger.SetDepthStencilState(DepthStencilStates.Default);
    }
}