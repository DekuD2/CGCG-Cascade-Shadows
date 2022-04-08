﻿using SharpDX;
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

    public readonly ControllableCamera Camera = new();

    public readonly RenderingTexture output;
    public readonly IRenderingPipeline Pipeline;

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
        ConstantBuffers.UpdateCamera(Context3D, Camera, Aspect);

        var viewport = new Viewport(0, 0, Width, Height, minDepth: 0, maxDepth: 1);
        Pipeline.Clear(Context3D, viewport);
        Pipeline.Render(Context3D, viewport, Color.Blue);
    }
}
