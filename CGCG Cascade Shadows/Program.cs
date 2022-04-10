using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FR.CascadeShadows;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        // TODO: I commented a lot of complex vertex shader and pixel shader to figure out the
        // D3D11 ERROR: ID3D11DeviceContext::Draw: Vertex Shader - Pixel Shader linkage error: Signatures between stages are incompatible. The input stage requires Semantic/Index (TEXCOORD,1) as input, but it is not provided by the output stage. [ EXECUTION ERROR #342: DEVICE_SHADER_LINKAGE_SEMANTICNAME_NOT_FOUND]

        Directory.SetCurrentDirectory("Resources");

        MainViewModel viewModel = new();
        MainWindow window = new(viewModel);

        window.Show();

        var presenter = viewModel.GetDirectXPresenter().Result;
        var renderer = new Renderer(presenter.Output);
        var ship = ResourceCache.Get<Mesh>(@"Models\ship_02.obj");
        // presenter.Output
        //var i = renderer.ForwardPass.AttachInstructions();
        //Console.WriteLine(Resources.Shaders.Ps.Color.Hi);

        Mesh mesh = new();
        mesh.Positions = new Vector3[] { new Vector3(-1, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 1, 1) };
        mesh.Normals = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
        mesh.Indices = new uint[] { 0, 1, 2 };
        var geometry = mesh.GeometryData;

        var shipInstr2 = DeferredPipeline.SurfacePass
            .Set(Resources.Shaders.ComplexProgram.Set)
            .Then(new Resources.Shaders.ComplexProgram.Material()
            {
                Diffuse = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange.png"),
                Normal = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_normal.png"),
                Emission = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange_emission.png"),
                Specular = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_ior.png"),
                Glossy = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_metallic2.png"),
            })
            .Then(GeometryData.Set(ship))
            .ThenDraw(c =>
            {
                ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.05f));
                c.DrawLastGeometry();
            });

        var ambientInstr = DeferredPipeline.LightPass
            .Set(Resources.Shaders.AmbientProgram.Set)
            .Then(new Resources.Shaders.AmbientProgram.Parameters(new Color(new Vector4(0.3f))))
            .ThenDraw(Resources.Shaders.AmbientProgram.Draw);

        var pointLightInstr = DeferredPipeline.LightPass
            .Set(Resources.Shaders.PointLightProgram.Set)
            .Then(new Resources.Shaders.PointLightProgram.LightParameters(new Vector3(4, 2, 1), Color.Orange, c3: 0.05f).Set)
            .ThenDraw(Resources.Shaders.PointLightProgram.Draw);

        //var dirLightInstr = DeferredPipeline.LightPass
        //    .Set(Resources.Shaders.DirectionalLightProgram.Set)
        //    .Then(new Resources.Shaders.DirectionalLightProgram.LightParameters(new Vector3(0.1f, -1, -0.1f), Color.Cyan, 0.4f).Set)
        //    .ThenDraw(Resources.Shaders.DirectionalLightProgram.Draw);

        DirectionalLight light = new(512, 512, new(new Vector3(0.01f, -1, -0.01f), Color.Cyan, 0.4f))
        {
            Position = new(0, 100, 0),
            Size = new(30, 30)
        };

        renderer.Lights.Add(light);

        //data.Position = Transform.World.TranslationVector;W
        //PointLightProgram.SetParameters(context, data);
        //PointLightProgram.Draw(context, data.Position, data.CalculateArea());

        int outputIdx = 0;

        renderer.Camera.Position = new(1, 4, 10);
        viewModel.MoveCamera += x => renderer.Camera.Move(x);
        viewModel.RotateCamera += x => renderer.Camera.Rotate(x);
        viewModel.OutputChanged += i => outputIdx = i;

        Queue<TimeSpan> snapshots = new(Enumerable.Repeat(TimeSpan.Zero, 60));
        Stopwatch stopwatch = new();
        //int framesInASecond

        while (true)
        {
            renderer.Render();

            DeferredPipeline.DEBUG_onlySurface = false;
            if (outputIdx is > 0 and <= 7)
            {
                DeferredPipeline.DEBUG_onlySurface = true;
                RenderUtils.CopyTexture(Devices.Context3D,
                    outputIdx == 7
                    ? ((DeferredPipeline)renderer.Pipeline).DepthBuffer.ShaderResourceView
                    : ((DeferredPipeline)renderer.Pipeline).Gbuffer.ShaderResourceViews.ElementAt(outputIdx - 1),
                    renderer.output.RenderTargetView!,
                    renderer.output.Description.Width,
                    renderer.output.Description.Height,
                    exponent: outputIdx == 7 ? 512f : 1f);
            }
            else if (outputIdx is 8)
                RenderUtils.CopyTexture(Devices.Context3D,
                    light.ShaderResourceView,
                    renderer.output.RenderTargetView!,
                    renderer.output.Description.Width,
                    renderer.output.Description.Height);

            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}

public abstract class Light
{
    public abstract void Setup(DeviceContext1 context);
}

public partial class DirectionalLight : Light
{
    readonly RenderingInstructions lightInstructions;
    readonly Viewport viewport;

    readonly RenderingTexture lightTexture = new(ShaderUsage.RenderTarget | ShaderUsage.ShaderResource);
    readonly Resources.Shaders.DirectionalLightProgram.LightParameters lightParams;

    public DirectionalLight(int width, int height, Resources.Shaders.DirectionalLightProgram.LightParameters lightParams)
    {
        viewport = new(0, 0, width, height, 0f, 1f);
        this.lightParams = lightParams;

        lightInstructions = DeferredPipeline.LightPass
            .Set(Resources.Shaders.DirectionalLightProgram.Set)
            .Then(lightParams.Set)
            .ThenDraw(Resources.Shaders.DirectionalLightProgram.Draw);

        lightTexture.RenderTargetViewDesc = new()
        {
            Format = Format.R32G32B32A32_Float,
            Dimension = RenderTargetViewDimension.Texture2D,
            Texture2D = new RenderTargetViewDescription.Texture2DResource()
            {
                MipSlice = 0
            }
        };

        lightTexture.ShaderResourceViewDesc = new()
        {
            Format = Format.R32G32B32A32_Float,
            Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
            Texture2D = new ShaderResourceViewDescription.Texture2DResource()
            {
                MipLevels = 1,
                MostDetailedMip = 0
            }
        };

        lightTexture.ReplaceTexture(new Texture2D(Devices.Device3D, new()
        {
            Width = width,
            Height = height,
            Format = Format.R32G32B32A32_Float,
            SampleDescription = new SampleDescription(count: 1, quality: 0),
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        }));
    }

    public Vector3 Position { get; set; }
    public Vector2 Size { get; set; } = new(250, 250);
    public ShaderResourceView ShaderResourceView => lightTexture.ShaderResourceView!;

    public override void Setup(DeviceContext1 context)
    {
        ConstantBuffers.UpdateCamera(context, this, Size.X / Size.Y, PassType.Shadows);
        context.ClearRenderTargetView(lightTexture.RenderTargetView, new Color(0, 1, 0, 0));
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(lightTexture.RenderTargetView);
    }
}

public partial class DirectionalLight : ICamera
{
    public bool Active => true;

    public float Order => 1;

    public Matrix View => Matrix.LookAtRH(
                Position,
                Position + lightParams.Direction,
                lightParams.Direction.X == 0 && lightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up);

    public Color Background => new(0f, 0f, 0f, 0f);

    public RectangleF ViewportRectangle => new(0, 0, 1, 1);

    public Matrix Projection(float aspect)
        => Matrix.OrthoRH(Size.X, Size.Y, 0.01f, 100f);

}