using FR.CascadeShadows;
using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using Microsoft.Win32;

using SharpDX;
using SharpDX.Direct3D11;

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
        // Create dual graph
        // 1) Add event (XXX added)
        // 2) Ignore all nodes except for geometry node
        // 3) Replace geometry node with a new geometry node that has 

        Directory.SetCurrentDirectory("Resources");

        MainViewModel viewModel = new();
        MainWindow window = new(viewModel);

        window.Show();

        var presenter = viewModel.GetDirectXPresenter().Result;
        var renderer = new Renderer(presenter.Output);
        var ship = ResourceCache.Get<Mesh>(@"Models\ship_02.obj");
        var ball = MeshGenerator.GenerateSphere(40, 40);
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
            Gloss = 0.0f,
            SpecularPower = 128f
        })
        {
            Scale = new(0.4f),
            Position = new(0, 4, 0)
        };

        var ballMO2 = new MeshObject(ball, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.Blue,
            Gloss = 0.5f,
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

        DirectionalLight light = new(2048, 2048, new(new Vector3(0.01f, -1, -0.01f), Color.White, 0.4f))
        {
            //Position = new(0, 100, 0),
            Range = new(50, 50)
        };

        renderer.Lights.Add(light);

        //data.Position = Transform.World.TranslationVector;
        //PointLightProgram.SetParameters(context, data);
        //PointLightProgram.Draw(context, data.Position, data.CalculateArea());

        int outputIdx = 0;

        #region setup window interactions
        renderer.Camera.Position = new(1, 4, 10);
        renderer.Camera.CascadeStops = new[] { 0.01f, 5f, 20f, 250f };
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
