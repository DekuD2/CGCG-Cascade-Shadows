using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

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

    RenderingTexture target;
    RenderingTexture depthBuffer = new(ShaderUsage.DepthStencil | ShaderUsage.ShaderResource);
    public ControllableCamera Camera = new();

    int Width => target.Description.Width;
    int Height => target.Description.Height;
    float Aspect => Width / Height;

    public Renderer(RenderingTexture target)
    {
        this.target = target;

        var depthDsvDesc = new DepthStencilViewDescription()
        {
            Dimension = DepthStencilViewDimension.Texture2D,
            Format = Format.D24_UNorm_S8_UInt,
        };

        var depthSrvDesc = new ShaderResourceViewDescription()
        {
            Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
            Format = Format.R24_UNorm_X8_Typeless
        };

        depthSrvDesc.Texture2D.MipLevels = -1;

        depthBuffer.DepthStencilViewDesc = depthDsvDesc;
        depthBuffer.ShaderResourceViewDesc = depthSrvDesc;

        Resized();

        ForwardPass = new(c => ForwardPass2.Enter(c, viewport, /*depthBuffer.DepthStencilView!*/null, target.RenderTargetView!));
    }

    public void Resized()
    {
        var depthBufferTexture = new Texture2D(
            Device3D,
            new Texture2DDescription()
            {
                //Format = Format.D32_Float_S8X24_UInt,
                Format = Format.R24G8_Typeless,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default
            });

        depthBuffer.ReplaceTexture(depthBufferTexture);
    }

    public void Render()
    {
        //Context3D.ClearRenderTargetView(target.RenderTargetView, new(0, 0, 0, 1));
        ConstantBuffers.UpdateCamera(Context3D, Camera, Aspect);

        viewport = new Viewport(0, 0, Width, Height, minDepth: 0, maxDepth: 1);
        ForwardPass.Render(Context3D);
    }

    //public InnerNode ForwardPass = new(c => ForwardPass2.Enter(c, viewport, dsv, rtv));
    public InnerNode ForwardPass = new(c =>
    {
        c.ClearState();

        // Target
        c.Rasterizer.SetViewport(viewport);
        c.OutputMerger.SetRenderTargets(dsv, rtv);

        // Rasterizer
        //context.Rasterizer.State = RasterizerStates.Default;
        c.Rasterizer.State = RasterizerStates.NoCulling;

        // Blending and DepthStencil settings
        c.OutputMerger.SetBlendState(BlendStates.Transparency);
        c.OutputMerger.SetDepthStencilState(DepthStencilStates.Default);
    });
}

public static class ForwardPass2
{
    public static void Enter(DeviceContext1 context, Viewport viewport, DepthStencilView dsv, RenderTargetView rtv)
    {
        context.ClearState();

        // Target
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(dsv, rtv);

        // Rasterizer
        //context.Rasterizer.State = RasterizerStates.Default;
        context.Rasterizer.State = RasterizerStates.NoCulling;

        // Blending and DepthStencil settings
        context.OutputMerger.SetBlendState(BlendStates.Transparency);
        context.OutputMerger.SetDepthStencilState(DepthStencilStates.Default);
    }
}