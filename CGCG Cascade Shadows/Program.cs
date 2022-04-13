using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using Microsoft.Win32;

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
    static TransitionGate PointLightGate = new();
    static TransitionGate AmbientLightGate = new();

    //private const string keyBase = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";

    //static string? GetProgramPath(string filename)
    //{
    //    using RegistryKey? fileKey = Registry.LocalMachine.OpenSubKey(string.Format(@"{0}\{1}", keyBase, filename));
    //    return fileKey?.GetValue(string.Empty)?.ToString();
    //}

    [STAThread]
    public static void Main()
    {
        Directory.SetCurrentDirectory("Resources");

        MainViewModel viewModel = new();
        MainWindow window = new(viewModel);

        window.Show();

        var presenter = viewModel.GetDirectXPresenter().Result;
        var renderer = new Renderer(presenter.Output);
        var ship = ResourceCache.Get<Mesh>(@"Models\ship_02.obj");
        var ball = MeshGenerator.GenerateSphere(20, 20);
        var quad = MeshGenerator.GenerateQuad();

        MeshObject shipMO = new(ship, new Resources.Shaders.ComplexProgram.Material()
        {
            Diffuse = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange.png"),
            Normal = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_normal.png"),
            Emission = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange_emission.png"),
            Specular = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_ior.png"),
            Glossy = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_metallic2.png"),
        })
        {
            Scale = new(0.05f)
        };

        var ballMO = new MeshObject(ball, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.Green,
            Gloss = 0.7f,
            SpecularPower = 128f
        })
        {
            Scale = new(0.4f),
            Position = new(0, 4, 0)
        };

        var ballMO2 = new MeshObject(ball, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.Blue,
            Gloss = 0.3f,
            SpecularPower = 1f
        })
        {
            Scale = new(0.4f),
            Position = new(3, 2, 1)
        };

        var quadMO = new MeshObject(quad, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.White,
            Gloss = 0.2f,
            SpecularPower = 0.5f
        })
        {
            Scale = new(10f),
            Position = new(0, -5, 0),
            Rotation = Quaternion.RotationYawPitchRoll(0f, -MathF.PI * 0.5f, 0f)
        };

        Vector3 ballPos = new(0, 4, 0);
        float quadHeight = -10;

        #region old scene setup (maybe better because it's more obvoius whats going on?)
        //var shipInstr2 = DeferredPipeline.SurfacePass
        //    .Set(Resources.Shaders.ComplexProgram.Set)
        //    .Then(new Resources.Shaders.ComplexProgram.Material()
        //    {
        //        Diffuse = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange.png"),
        //        Normal = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_normal.png"),
        //        Emission = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange_emission.png"),
        //        Specular = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_ior.png"),
        //        Glossy = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_metallic2.png"),
        //    })
        //    .Then(GeometryData.Set(ship))
        //    .ThenDraw(c =>
        //    {
        //        ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.05f));
        //        c.DrawLastGeometry();
        //    });


        //var ballInstr = DeferredPipeline.SurfacePass
        //    .Set(Resources.Shaders.SimpleProgram.Set)
        //    .Then(GeometryData.Set(ball))
        //    .Then(new Resources.Shaders.SimpleProgram.Material()
        //    {
        //        Diffuse = Color.Green,
        //        Gloss = 0.7f,
        //        SpecularPower = 128f
        //    })
        //    .ThenDraw(c =>
        //    {
        //        ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.4f) * Matrix.Translation(ballPos));
        //        c.DrawLastGeometry();
        //    });

        //var quadInstr = DeferredPipeline.SurfacePass
        //    .Set(Resources.Shaders.SimpleProgram.Set)
        //    .Then(GeometryData.Set(quad))
        //    .Then(new Resources.Shaders.SimpleProgram.Material()
        //    {
        //        Diffuse = Color.White,
        //        Gloss = 0.2f,
        //        SpecularPower = 0.5f
        //    })
        //    .ThenDraw(c =>
        //    {
        //        ConstantBuffers.UpdateTransform(c, Matrix.RotationX(-MathF.PI * 0.5f) * Matrix.Scaling(10f) * Matrix.Translation(0, quadHeight, 0));
        //        c.DrawLastGeometry();
        //    });

        //var ballInstr2 = DeferredPipeline.SurfacePass
        //    .Set(Resources.Shaders.SimpleProgram.Set)
        //    .Then(GeometryData.Set(ball))
        //    .Then(new Resources.Shaders.SimpleProgram.Material()
        //    {
        //        Diffuse = Color.Blue,
        //        Gloss = 0.3f,
        //        SpecularPower = 1f
        //    })
        //    .ThenDraw(c =>
        //    {
        //        ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.4f) * Matrix.Translation(new(3, 2, 1)));
        //        c.DrawLastGeometry();
        //});
        #endregion

        var ambientInstr = DeferredPipeline.LightPass
            .Set(Resources.Shaders.AmbientProgram.Set)
            .Then(new Resources.Shaders.AmbientProgram.Parameters(new Color(new Vector4(0.1f))))
            .Then(AmbientLightGate)
            .ThenDraw(Resources.Shaders.AmbientProgram.Draw);

        var pointLightInstr = DeferredPipeline.LightPass
            .Set(Resources.Shaders.PointLightProgram.Set)
            .Then(new Resources.Shaders.PointLightProgram.LightParameters(new Vector3(4, 2, 1), Color.Orange, c3: 0.05f).Set)
            .Then(PointLightGate)
            .ThenDraw(Resources.Shaders.PointLightProgram.Draw);

        DirectionalLight light = new(512, 512, new(new Vector3(0.01f, -1, -0.01f), Color.White, 0.4f))
        {
            Position = new(0, 100, 0),
            Range = new(30, 30)
        };

        renderer.Lights.Add(light);

        //data.Position = Transform.World.TranslationVector;
        //PointLightProgram.SetParameters(context, data);
        //PointLightProgram.Draw(context, data.Position, data.CalculateArea());

        int outputIdx = 0;

        #region setup window interactions
        renderer.Camera.Position = new(1, 4, 10);
        viewModel.MoveCamera += x => renderer.Camera.Move(x);
        viewModel.RotateCamera += x => renderer.Camera.Rotate(x);
        viewModel.OutputChanged += i => outputIdx = i;
        viewModel.ReloadShader += s =>
        {
            try
            {
                Resources.Shaders.DirectionalLightProgram.Recompile();
            }
            catch (Exception e)
            {
                viewModel.ShowError(e.Message);
            }
        };
        viewModel.Toggle += (name, show) =>
        {
            var gate = name switch
            {
                "ambient light" => AmbientLightGate,
                "point light" => PointLightGate,
                "directional light" => light.Gate,
                _ => throw new Exception($"Unknown toggle ({name})")
            };
            gate.ShowNode = show;
        };
        #endregion

        Queue<TimeSpan> snapshots = new(Enumerable.Repeat(TimeSpan.Zero, 60));
        Stopwatch stopwatch = new();
        //int framesInASecond

        var timer = Stopwatch.StartNew();

        while (true)
        {
            // Scene dynamics
            light.LightParams.Direction.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1000f) * 0.02f;
            light.LightParams.Direction.X = (float)Math.Cos(timer.ElapsedMilliseconds / 1000f) * 0.02f;

            ballPos.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1400f) * 2f;
            ballMO.Position.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1400f) * 2f;

            quadHeight = -5 + (float)Math.Cos(timer.ElapsedMilliseconds / 1800f) * 5f;
            quadMO.Position.Y = -5 + (float)Math.Cos(timer.ElapsedMilliseconds / 1800f) * 5f;

            // Render
            renderer.Render();

            // Output filter
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
                    renderer.output.Description.Height,
                    exponent: 1f);

            // Other
            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}

public class MeshObject
{
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;

    public Matrix Transform => Matrix.Scaling(Scale) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Position);

    RenderingInstructions instructions;

    public MeshObject(Mesh mesh, IMaterial material)
    {
        instructions = DeferredPipeline.SurfacePass
            .Set(material.ProgramStep)
            .Then(material.MaterialStep)
            .Then(GeometryData.Set(mesh))
            .ThenDraw(Draw);
    }

    void Draw(DeviceContext1 context)
    {
        ConstantBuffers.UpdateTransform(context, Transform);
        context.DrawLastGeometry();
    }
}

public abstract class Light
{
    public abstract void Setup(DeviceContext1 context);
    public abstract float Aspect { get; }
    public abstract ICamera Camera { get; }
}

public abstract partial class BaseLight : Light
{
    static DepthStencilViewDescription DepthStencilViewDescription = new()
    {
        Format = Format.D24_UNorm_S8_UInt,
        Dimension = DepthStencilViewDimension.Texture2D,
        Texture2D = new DepthStencilViewDescription.Texture2DResource()
        {
            MipSlice = 0
        }
    };

    static ShaderResourceViewDescription ShaderResourceViewDescription = new()
    {
        Format = Format.R24_UNorm_X8_Typeless,
        Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
        Texture2D = new ShaderResourceViewDescription.Texture2DResource()
        {
            MipLevels = 1,
            MostDetailedMip = 0
        }
    };

    public static (RenderingTexture, Viewport) CreateDepthTexture(int width, int height)
    {
        Viewport viewport = new(0, 0, width, height, 0f, 1f);
        RenderingTexture renderingTexture = new(ShaderUsage.DepthStencil | ShaderUsage.ShaderResource)
        {
            DepthStencilViewDesc = DepthStencilViewDescription,
            ShaderResourceViewDesc = ShaderResourceViewDescription
        };

        renderingTexture.ReplaceTexture(new Texture2D(Devices.Device3D, new()
        {
            Width = width,
            Height = height,
            Format = Format.R24G8_Typeless,
            SampleDescription = new SampleDescription(count: 1, quality: 0),
            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        }));

        return (renderingTexture, viewport);
    }
    public override ICamera Camera => this;

    public Vector3 Position { get; set; }
}
public abstract partial class BaseLight : ICamera
{
    public bool Active => true;
    public float Order => 1;
    public Color Background => new(0f, 0f, 0f, 0f);
    public RectangleF ViewportRectangle => new(0, 0, 1, 1);

    public abstract Matrix View { get; }
    public abstract Matrix Projection(float aspect);
}

public partial class DirectionalLight : BaseLight
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

    public override void Setup(DeviceContext1 context)
    {
        // TEMP??
        LightParams.SetProjection(ref LightParams, View * Projection(Aspect));

        context.ClearDepthStencilView(lightTexture.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(lightTexture.DepthStencilView);
    }

    public Vector2 Range { get; set; } = new(250, 250);
    public override float Aspect => Range.X / Range.Y;

    public override Matrix View => Matrix.LookAtRH(
                Position,
                Position + LightParams.Direction,
                LightParams.Direction.X == 0 && LightParams.Direction.Z == 0 ? Vector3.ForwardRH : Vector3.Up);
    public override Matrix Projection(float aspect)
        => Matrix.OrthoRH(Range.X, Range.Y, 0.01f, 250f);
}