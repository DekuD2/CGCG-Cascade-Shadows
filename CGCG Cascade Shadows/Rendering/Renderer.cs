using FR.CascadeShadows.Rendering.Cameras;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System.Collections.Generic;

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

    public readonly ControllableCamera Camera = new(0.01f, 250f);

    public readonly RenderingTexture output;
    public readonly IRenderingPipeline Pipeline;

    public readonly List<Light> Lights = new();

    public Renderer(RenderingTexture target)
    {
        output = target;
        Pipeline = new DeferredPipeline(target);
    }

    int Width => output.Description.Width;
    int Height => output.Description.Height;
    float Aspect => Width / Height;

    public void Render()
    {
        //Context3D.ClearRenderTargetView(target.RenderTargetView, new(0, 0, 0, 1));
        var viewport = new Viewport(0, 0, Width, Height, minDepth: 0, maxDepth: 1);
        Camera.SetTarget(viewport);

        // Cast shadows
        foreach (var light in Lights)
            light.Render(Context3D, Camera);


        ConstantBuffers.UpdateCamera(Context3D, Camera, PassType.Normal);
        Pipeline.Clear(Context3D, viewport);
        Pipeline.Render(Context3D, viewport, Color.Blue);
    }
}

public interface IRenderPass
{
    void Render(DeviceContext1 context, Viewport viewport);
}

public class PipelinePass : IRenderPass
{
    private readonly IRenderingPipeline pipeline;

    public PipelinePass(IRenderingPipeline pipeline)
        => this.pipeline = pipeline;

    public void Render(DeviceContext1 context, Viewport viewport)
    {
        pipeline.Clear(Context3D, viewport);
        pipeline.Render(Context3D, viewport, Color.Blue);
    }
}
