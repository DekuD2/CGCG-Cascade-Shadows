using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Linq;

namespace FR.CascadeShadows;

/// <param name="Near">Set to float.0 to use camera.Near</param>
/// <param name="Far">Set to float.PositiveInfinity to use camera.Far</param>
public record CascadeDescription(float Near, float Far, int TexWidth, int TexHeight);

record Cascade(Viewport Viewport, RenderingTexture Texture, CascadeDescription Description)
{
    public float Near => Description.Near;
    public float Far => Description.Far;
    public DepthStencilView? DepthStencilView => Texture.DepthStencilView;
    public ShaderResourceView? ShaderResourceView => Texture.ShaderResourceView;
}

public partial class DirectionalLight : Light
{
    public Resources.Shaders.DirectionalLightProgram.LightParameters LightParams;

    readonly RenderingInstructions lightInstructions;

    readonly Cascade[] cascades = new Cascade[3];

    public Vector3 Source { get; set; } = new(0, 100, 0);

    public static bool TexelSnapping = true;

    public DirectionalLight(
        Resources.Shaders.DirectionalLightProgram.LightParameters lightParams,
        params CascadeDescription[] descs)
    {
        Debug.Assert(descs.Length <= 3);

        for (int i = 0; i < descs.Length; i++)
        {
            var (lightTexture, viewport) = CreateDepthTexture(descs[i].TexWidth, descs[i].TexHeight);
            this.cascades[i] = new Cascade(viewport, lightTexture, descs[i]);
        }

        LightParams = lightParams;

        lightInstructions = DeferredPipeline.LightPass
            .Set(Resources.Shaders.DirectionalLightProgram.Set)
            .Then(c =>
            {
                LightParams.Set(c);
                c.PixelShader.SetShaderResource(10, cascades[0].ShaderResourceView);
                c.PixelShader.SetShaderResource(11, cascades[1].ShaderResourceView);
                c.PixelShader.SetShaderResource(12, cascades[2].ShaderResourceView);
            })
            .Then(Gate)
            .ThenDraw(Resources.Shaders.DirectionalLightProgram.Draw);
    }

    public ShaderResourceView ShaderResourceView => cascades[0].Texture.ShaderResourceView!;
    public ShaderResourceView ShaderResourceView1 => cascades[1].Texture.ShaderResourceView!;
    public ShaderResourceView ShaderResourceView2 => cascades[2].Texture.ShaderResourceView!;
    public TransitionGate Gate { get; } = new();

    int n = 0;

    public override void Render(DeviceContext1 context, ICamera camera)
    {
        DumbCamera lightCamera = new()
        {
            Position = Vector3.Zero,
            View = Matrix.LookAtRH(
                Source + Vector3.Zero,
                Source + LightParams.Direction,
                LightParams.Direction.X == 0 && LightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up),
        };

        int index = 0;
        foreach (var c in cascades)
        {
            lightCamera.SetTarget(c.Viewport);

            var corners = CameraUtils.GetCorners(
                    camera.View,
                    camera.ProjectionSubfrustum(
                        c.Near == 0 ? camera.Near : c.Near,
                        c.Far == float.PositiveInfinity ? camera.Far : c.Far)).ToArray();

            if (!TexelSnapping)
            {
                Vector2 min = new(float.MaxValue);
                Vector2 max = new(float.MinValue);

                // Find min and max
                foreach (var corner in corners)
                {
                    var lightSpaceCorner = Vector3.TransformCoordinate(corner, lightCamera.View);
                    min = Vector2.Min(min, new(lightSpaceCorner.X, lightSpaceCorner.Y));
                    max = Vector2.Max(max, new(lightSpaceCorner.X, lightSpaceCorner.Y));
                }

                // Set projection
                lightCamera.Projection = Matrix.OrthoOffCenterRH(min.X, max.X, min.Y, max.Y, 0.01f, 250f);
            }
            else // yes TexelSnapping
            {
                Vector3 firstCorner = Vector3.TransformCoordinate(corners[0], lightCamera.View);
                Vector3 secondCorner = Vector3.TransformCoordinate(corners[7], lightCamera.View);

                float diameter = (firstCorner - secondCorner).Length();
                float texelSize = c.Description.TexWidth / diameter;
                Vector3 center = (firstCorner + secondCorner) / 2f;
                float radius = diameter / 2f;

                center *= texelSize;
                center.X = (float)Math.Round(center.X);
                center.Y = (float)Math.Round(center.Y);
                center /= texelSize;

                lightCamera.Projection = Matrix.OrthoOffCenterRH(center.X - radius, center.X + radius, center.Y - radius, center.Y + radius, 0.01f, 250f);
            }

            LightParams.SetProjection(ref LightParams, lightCamera.View * lightCamera.Projection, index++);

            context.ClearState();

            ConstantBuffers.UpdateCamera(context, lightCamera, PassType.Shadows);

            context.ClearDepthStencilView(c.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
            context.Rasterizer.SetViewport(c.Viewport);
            context.OutputMerger.SetRenderTargets(c.DepthStencilView);

            DeferredPipeline.ShadowCastPass.Render(context); // HACK: depends on specific pipeline :c
        }
    }
}