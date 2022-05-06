using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Linq;

namespace FR.CascadeShadows;

public partial class DirectionalLight : Light
{
    public Resources.Shaders.DirectionalLightProgram.LightParameters LightParams;

    readonly RenderingInstructions lightInstructions;
    readonly Viewport viewport;
    readonly RenderingTexture lightTexture;

    public Vector3 Source { get; set; } = new(0, 100, 0);

    public DirectionalLight(
        int texWidth, int texHeight,
        Resources.Shaders.DirectionalLightProgram.LightParameters lightParams)
    {
        (lightTexture, viewport) = CreateDepthTexture(texWidth, texHeight);

        LightParams = lightParams;

        lightInstructions = DeferredPipeline.LightPass
            .Set(Resources.Shaders.DirectionalLightProgram.Set)
            .Then(c =>
            {
                LightParams.Set(c);
                c.PixelShader.SetShaderResource(10, lightTexture.ShaderResourceView);
            })
            .Then(Gate)
            .ThenDraw(Resources.Shaders.DirectionalLightProgram.Draw);
    }

    public ShaderResourceView ShaderResourceView => lightTexture.ShaderResourceView!;
    public TransitionGate Gate { get; } = new();

    //public override void Setup(DeviceContext1 context)
    //{
    //    // TEMP??
    //    LightParams.SetProjection(ref LightParams, View * Projection(Aspect));

    //    context.ClearDepthStencilView(lightTexture.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
    //    context.Rasterizer.SetViewport(viewport);
    //    context.OutputMerger.SetRenderTargets(lightTexture.DepthStencilView);
    //}

    public override void Render(DeviceContext1 context, ICamera camera)
    {
        Camera lightCamera = new()
        {
            Position = Vector3.Zero,
            View = Matrix.LookAtRH(
                Source + Vector3.Zero,
                Source + LightParams.Direction,
                LightParams.Direction.X == 0 && LightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up),
            //Projection = Matrix.OrthoRH(
            //    Range.X,
            //    Range.Y,
            //    0.01f,
            //    250f)
        };

        //context.ClearState();
        lightCamera.SetTarget(viewport);
        //ConstantBuffers.UpdateCamera(context, lightCamera, PassType.Shadows);

        if (camera is ICascadeCamera cascadedCamera)
        {
            foreach (var c in Enumerable.Range(0, 1/*cascadedCamera.Cascades*/))
            {
                Vector2 min = new(float.MaxValue);
                Vector2 max = new(float.MinValue);

                var corners = cascadedCamera.GetCorners(c).ToArray();

                // Find min and max
                foreach (var corner in cascadedCamera.GetCorners(c))
                {
                    var lightSpaceCorner = Vector3.TransformCoordinate(corner, lightCamera.View);
                    min = Vector2.Min(min, new(lightSpaceCorner.X, lightSpaceCorner.Y));
                    max = Vector2.Max(max, new(lightSpaceCorner.X, lightSpaceCorner.Y));
                }

                // Set projection
                lightCamera.Projection = Matrix.OrthoOffCenterRH(min.X, max.X, min.Y, max.Y, 0.01f, 250f);

                //lightCamera.Projection = Matrix.OrthoRH(
                //    Range.X,
                //    Range.Y,
                //    0.01f,
                //    250f);

                LightParams.SetProjection(ref LightParams, lightCamera.View * lightCamera.Projection);

                context.ClearState();

                ConstantBuffers.UpdateCamera(context, lightCamera, PassType.Shadows);

                context.ClearDepthStencilView(lightTexture.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
                context.Rasterizer.SetViewport(viewport);
                context.OutputMerger.SetRenderTargets(lightTexture.DepthStencilView);

                DeferredPipeline.ShadowCastPass.Render(context); // HACK: depends on specific pipeline :c
            }
        }

        //LightParams.SetProjection(ref LightParams, lightCamera.View * lightCamera.Projection);

        //context.ClearDepthStencilView(lightTexture.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
        //context.Rasterizer.SetViewport(viewport);
        //context.OutputMerger.SetRenderTargets(lightTexture.DepthStencilView);

        //DeferredPipeline.SurfacePass.Render(context); // HACK: depends on specific pipeline :c
    }

    public Vector2 Range { get; set; } = new(250, 250);
    public float Aspect => Range.X / Range.Y;
}