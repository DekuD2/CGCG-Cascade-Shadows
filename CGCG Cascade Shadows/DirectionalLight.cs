using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows;

public partial class DirectionalLight : Light
{
    public Resources.Shaders.DirectionalLightProgram.LightParameters LightParams;

    readonly RenderingInstructions lightInstructions;
    readonly Viewport viewport;
    readonly RenderingTexture lightTexture;

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

    public override void Render(DeviceContext1 context)
    {
        Vector3[] cornerns = new Vector3[8];
        for (int i = 0; i < 8; i++)
            cornerns[i] = new Vector3(i % 2, (i << 1) % 2, (i << 2) % 2);

        float[] stops = new float[] { 0.01f, 10f, 35f, 80f, 250f }; // 5 numbers define 4 regions [x,(x+1)]

        Matrix[] perspectives = new Matrix[stops.Length - 1];
        //for(int i = 0; i < stops.Length; i++)
        // This is supposed to be on the camera actually

        Camera camera = new()
        {
            Position = Position,
            Projection = Matrix.LookAtRH(
                Position,
                Position + LightParams.Direction,
                LightParams.Direction.X == 0 && LightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up),
            View = Matrix.LookAtRH(
                Position,
                Position + LightParams.Direction,
                LightParams.Direction.X == 0 && LightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up)
        };

        context.ClearState();
        camera.SetTarget(viewport);
        ConstantBuffers.UpdateCamera(context, camera, PassType.Shadows);

        LightParams.SetProjection(ref LightParams, camera.View * camera.Projection);

        context.ClearDepthStencilView(lightTexture.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(lightTexture.DepthStencilView);

        DeferredPipeline.SurfacePass.Render(context); // HACK: depends on specific pipeline :c
    }

    public Vector2 Range { get; set; } = new(250, 250);
    public float Aspect => Range.X / Range.Y;
    public Vector3 Position { get; set; }
}