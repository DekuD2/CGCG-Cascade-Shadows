using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.IO;
using System.Linq;

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
                Specular = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_metallic2.png"),
                Glossy = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_ior.png"),
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

        //var pointLightInstr = DeferredPipeline.LightPass
        //    .Set(Resources.Shaders.PointLightProgram.Set)
        //    .Then(new Resources.Shaders.PointLightProgram.LightParameters(new Vector3(1, 10, 1), Color.Cyan, c3: 0.05f).Set)
        //    .ThenDraw(Resources.Shaders.PointLightProgram.Draw);

        var dirLightInstr = DeferredPipeline.LightPass
            .Set(Resources.Shaders.DirectionalLightProgram.Set)
            .Then(new Resources.Shaders.DirectionalLightProgram.LightParameters(new Vector3(0.1f, -1, -0.1f), Color.Cyan, 0.4f).Set)
            .ThenDraw(Resources.Shaders.DirectionalLightProgram.Draw);

        //data.Position = Transform.World.TranslationVector;
        //PointLightProgram.SetParameters(context, data);
        //PointLightProgram.Draw(context, data.Position, data.CalculateArea());

        renderer.Camera.Position = new(1, 4, 10);
        viewModel.MoveCamera += x => renderer.Camera.Move(x);
        viewModel.RotateCamera += x => renderer.Camera.Rotate(x);

        while (true)
        {
            renderer.Render();

            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}
