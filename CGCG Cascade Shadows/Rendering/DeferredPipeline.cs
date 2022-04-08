using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace FR.CascadeShadows.Rendering;

//public class ShadowPipeline : IRenderingPipeline
//{

//}

public class DeferredPipeline : IRenderingPipeline
{
    public static readonly Color4 ClearColor = new(0, 0, 0, 1);

    public static readonly InnerNode SurfacePass = new(c =>
    {
        // Rasterizer
        Devices.Context3D.Rasterizer.State = RasterizerStates.Default;

        // Blending and DepthStencil settings
        Devices.Context3D.OutputMerger.SetBlendState(BlendStates.Default);
        Devices.Context3D.OutputMerger.SetDepthStencilState(DepthStencilStates.Default);
    });

    public static readonly InnerNode LightPass = new(c =>
    {
        // Rasterizer
        Devices.Context3D.Rasterizer.State = RasterizerStates.Default;

        // Blending and DepthStencil settings
        Devices.Context3D.OutputMerger.SetBlendState(BlendStates.Additive);
        Devices.Context3D.OutputMerger.SetDepthStencilState(DepthStencilStates.Ignore);
    });

    public static readonly InnerNode ForwardPass = new(c =>
    {
        // Rasterizer
        c.Rasterizer.State = RasterizerStates.Default;

        // Blending and DepthStencil settings
        c.OutputMerger.SetBlendState(BlendStates.Transparency);
        c.OutputMerger.SetDepthStencilState(DepthStencilStates.Default);
    });

    public DeferredPipeline(RenderingTexture output)
    {
        Output = output;
        Gbuffer = new(output);
        DepthBuffer = new(output);
    }

    public RenderingTexture Output { get; }
    public DepthBuffer DepthBuffer { get; }
    public GBuffer Gbuffer { get; }

    public void Clear(DeviceContext1 context, Viewport viewport)
    {
        // Clear GBuffer
        foreach (var rtv in Gbuffer.RenderTargetViews)
            context.ClearRenderTargetView(rtv, ClearColor);

        // Clear depth buffer
        context.ClearDepthStencilView(DepthBuffer.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);

        // Clear render target
        context.ClearRenderTargetView(Output.RenderTargetView, /*Color.BlanchedAlmond*/ClearColor);
    }

    public void Render(DeviceContext1 context, Viewport viewport, Color background)
    {
        // Surface pass
        context.ClearState();
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(DepthBuffer.DepthStencilView, Gbuffer.RenderTargetViews.ToArray());
        SurfacePass.Render(Devices.Context3D);

        // Light pass
        context.ClearState();
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(DepthBuffer.DepthStencilView, Output.RenderTargetView);
        context.PixelShader.SetShaderResources(0, Gbuffer.ShaderResourceViews.ToArray());
        LightPass.Render(Devices.Context3D);

        // Background would be here

        // Forward pass
        context.ClearState();
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(DepthBuffer.DepthStencilView, Output.RenderTargetView);
        ForwardPass.Render(Devices.Context3D);

    }
}