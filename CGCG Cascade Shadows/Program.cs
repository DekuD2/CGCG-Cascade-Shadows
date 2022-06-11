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
        var astronaut = ResourceCache.Get<Mesh>(@"Models\11070_astronaut_v4.obj");
        var ball = MeshGenerator.GenerateSphere(40, 40);
        var quad = MeshGenerator.GenerateQuad();
        Settings settings = new();
        settings.Set(Devices.Context3D);

        MeshObject shipMO = new(ship, new Resources.Shaders.ComplexProgram.Material()
        {
            Diffuse = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange.png"),
            Normal = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_normal_l.png"),
            Emission = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange_emission.png"),
            Specular = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_ior_l.png"),
            Glossy = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_metallic2_l.png"),
        })
        {
            Scale = new(0.05f)
        };

        var astronautMO = new MeshObject(astronaut, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.LightGray,
            Gloss = 0.01f,
            SpecularPower = 512f
        })
        {
            Scale = new(0.1f),
            Rotation = Quaternion.RotationYawPitchRoll(0, -MathF.PI * 0.5f, 0),
            Position = new(-5, -5, 0)
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

        var ball2MO = new MeshObject(ball, new Resources.Shaders.SimpleProgram.Material()
        {
            Diffuse = Color.Blue,
            Gloss = 0.5f,
            SpecularPower = 1f
        })
        {
            Scale = new(0.4f),
            Position = new(3, 2, 1)
        };

        var quadBasicMO = new MeshObject(quad, new Resources.Shaders.SimpleProgram.Material()
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
        quadBasicMO.Show = false;

        var quadParallaxMO = new MeshObject(quad, new Resources.Shaders.ParallaxProgram.Material()
        {
            Diffuse = ResourceCache.Get<ShaderResourceView>(@"Textures\bricks2.jpg"),
            Displacement = ResourceCache.Get<ShaderResourceView>(@"Textures\bricks2_disp_l.jpg"),
            Normal = ResourceCache.Get<ShaderResourceView>(@"Textures\bricks2_normal_l.jpg"),
        })
        {
            Scale = new(10f),
            Position = new(0, -5, 0),
            Rotation = Quaternion.RotationYawPitchRoll(0f, -MathF.PI * 0.5f, 0f)
        };

        //Vector3 ballPos = new(0, 4, 0);
        //float quadHeight = -10;

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
            .Then(new Resources.Shaders.PointLightProgram.LightParameters(new Vector3(8, 3, 2), Color.Orange, c3: 0.25f).Set)
            .Then(PointLightGate)
            .ThenDraw(Resources.Shaders.PointLightProgram.Draw);

        DirectionalLight light = new(
            new Vector3(0.05f, -1, -0.01f),
            Color.White,
            0.4f,
            new CascadeDescription(0, 5, 2048, 2048),
            new CascadeDescription(4.8f, 16, 1024, 1024),
            new CascadeDescription(15.8f, float.PositiveInfinity, 4096, 4096)
            );

        renderer.Lights.Add(light);

        //data.Position = Transform.World.TranslationVector;
        //PointLightProgram.SetParameters(context, data);
        //PointLightProgram.Draw(context, data.Position, data.CalculateArea());

        int outputIdx = 0;

        var timer = Stopwatch.StartNew();

        #region setup window interactions
        renderer.Camera.Position = new(1, 4, 10);
        viewModel.MoveCamera += x => renderer.Camera.Move(x);
        viewModel.RotateCamera += x => renderer.Camera.Rotate(x);
        viewModel.OutputChanged += i => outputIdx = i;
        viewModel.SamplerChanged += ViewModel_SamplerChanged;
        viewModel.ReloadShader += s =>
        {
            try
            {
                if (s == "directional light")
                    Resources.Shaders.DirectionalLightProgram.Recompile();
                else if (s == "parallax")
                    Resources.Shaders.Ps.Parallax.Recompile();
            }
            catch (Exception e)
            {
                viewModel.ShowError(e.Message);
            }
        };
        viewModel.SetValue += (name, value) =>
        {
            if (name == "pcf")
            {
                if (value is int i)
                {
                    settings.PcfMode = i;
                    settings.Set(Devices.Context3D);
                }
            }
            else if (name == "depthBias")
            {
                if (value is float f)
                {
                    settings.DepthBias = f;
                    settings.Set(Devices.Context3D);
                }
            }
            else if (name == "visualise")
            {
                if (value is int i)
                {
                    settings.Visualise = i;
                    settings.Set(Devices.Context3D);
                }
            }
            else if (name == "cascade1")
            {
                if (value is int i)
                    light.UpdateResolution(128 * (int)Math.Pow(2, i), 0);
            }
            else if (name == "cascade2")
            {
                if (value is int i)
                    light.UpdateResolution(128 * (int)Math.Pow(2, i), 1);
            }
            else if (name == "cascade3")
            {
                if (value is int i)
                    light.UpdateResolution(128 * (int)Math.Pow(2, i), 2);
            }
        };
        viewModel.Toggle += (name, show) =>
        {
            if (name == "play")
            {
                if (show)
                    timer.Start();
                else
                    timer.Stop();
            }
            else if (name == "cascadeBlend")
            {
                settings.BlendCascades = show;
                settings.Set(Devices.Context3D);
            }
            else if (name == "derivative")
            {
                settings.Derivative = show;
                settings.Set(Devices.Context3D);
            }
            else if (name == "texelSnap")
            {
                DirectionalLight.TexelSnapping = show;
            }
            else if (name == "fitToScene")
            {
                // 0 5 20 infinity
                //float[] steps = new float[] { 0, 5, 16, float.PositiveInfinity };
                if (show)
                {
                    light.UpdateBoundaries(0, 5, 0);
                    light.UpdateBoundaries(0, 16, 1);
                    light.UpdateBoundaries(0, float.PositiveInfinity, 2);
                }
                else
                {
                    light.UpdateBoundaries(0, 5, 0);
                    light.UpdateBoundaries(4.8f, 16, 1);
                    light.UpdateBoundaries(15.8f, float.PositiveInfinity, 2);
                }
            }
            else if (name == "parallax")
            {
                quadParallaxMO.Show = show;
                quadBasicMO.Show = !show;
            }
            else
            {
                var gate = name switch
                {
                    "ambient light" => AmbientLightGate,
                    "point light" => PointLightGate,
                    "directional light" => light.Gate,
                    _ => throw new Exception($"Unknown toggle ({name})")
                };
                gate.ShowNode = show;
            }
        };
        #endregion

        int frames = 0;
        var stopwatch = Stopwatch.StartNew();
        //int framesInASecond

        while (true)
        {
            // Scene dynamics
            light.LightParams.Direction.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1000f) * 0.45f;
            //light.LightParams.Direction.X = (float)Math.Cos(timer.ElapsedMilliseconds / 1000f) * 0.02f;

            //ballPos.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1400f) * 2f;
            ballMO.Position.Z = (float)Math.Sin(timer.ElapsedMilliseconds / 1400f) * 2f;

            //quadHeight = -5 + (float)Math.Cos(timer.ElapsedMilliseconds / 1800f) * 5f;
            //quadParallaxMO.Position.Y = -5 + (float)Math.Cos(timer.ElapsedMilliseconds / 1800f) * 5f;
            //quadBasicMO.Position.Y = -5 + (float)Math.Cos(timer.ElapsedMilliseconds / 1800f) * 5f;
            shipMO.Position.Y = -1 + (float)Math.Cos(timer.ElapsedMilliseconds / 800f) * 2.8f;

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
            else if (outputIdx is 9)
                RenderUtils.CopyTexture(Devices.Context3D,
                    light.ShaderResourceView1,
                    renderer.output.RenderTargetView!,
                    renderer.output.Description.Width,
                    renderer.output.Description.Height,
                    exponent: 1f);
            else if (outputIdx is 10)
                RenderUtils.CopyTexture(Devices.Context3D,
                    light.ShaderResourceView2,
                    renderer.output.RenderTargetView!,
                    renderer.output.Description.Width,
                    renderer.output.Description.Height,
                    exponent: 1f);

            // Other
            WpfDispatcher.ProcessMessages();
            presenter.Present();

            frames++;
            if(stopwatch.ElapsedMilliseconds >= 1000)
            {
                System.Diagnostics.Debug.WriteLine(frames);
                frames = 0;
                stopwatch.Restart();
            }
        }
    }

    private static void ViewModel_SamplerChanged(string sampler)
    {
        Debug.WriteLine(sampler);
        Resources.Shaders.DirectionalLightProgram.Sampler = sampler switch
        {
            "MinMagMipLinear" => SamplerStates.Shadow,
            "MinMagMipPoint" => SamplerStates.Point,
            "Anisotropic" => SamplerStates.Anisotropic,
            _ => SamplerStates.Default
        };
    }
}
