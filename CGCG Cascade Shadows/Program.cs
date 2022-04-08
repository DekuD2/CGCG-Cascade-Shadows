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

        //var shipInstr = DeferredPipeline.ForwardPass.AttachInstructions(
        //    new TransitionMethod(Resources.Shaders.Vs.Simple.Set),
        //    new TransitionMethod(Resources.Shaders.Ps.Color.Set),
        //    GeometryData.Set(ship),
        //    new DrawMethod(Draw)); 

        var shipInstr = DeferredPipeline.SurfacePass.AttachInstructions(
            new TransitionMethod(Resources.Shaders.ComplexProgram.Set),
            new Resources.Shaders.ComplexProgram.Material()
            {
                Diffuse = ResourceCache.Get<ShaderResourceView>(@"Models\Ship\ship_orange.png")
            },
            GeometryData.Set(ship),
            new DrawMethod(c =>
            {
                ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.1f));
                c.DrawLastGeometry();
            }));

        var lightInstr = DeferredPipeline.LightPass.AttachInstructions(
            new TransitionMethod(Resources.Shaders.AmbientProgram.Set),
            new Resources.Shaders.AmbientProgram.Parameters(Color.Red),
            new DrawMethod(Resources.Shaders.AmbientProgram.Draw));

        //var shipInstr = DeferredPipeline.SurfacePass.AttachInstructions(
        //    new TransitionMethod(Resources.Shaders.Vs.Complex.Set),
        //    new TransitionMethod(Resources.Shaders.Ps.ColorSurface.Set),
        //    GeometryData.Set(ship),
        //    new DrawMethod(c =>
        //    {
        //        Resources.Shaders.Ps.ColorSurface.SetParameters(c, Color.Cyan);
        //        ConstantBuffers.UpdateTransform(c, Matrix.Scaling(0.1f));
        //        c.DrawLastGeometry();
        //    }));

        //var surfaceTest = DeferredPipeline.SurfacePass.AttachInstructions(
        //    new TransitionMethod(Resources.Shaders.FullscreenProgram.Set),
        //    new TransitionMethod(Resources.Shaders.Ps.ColorSurface.Set),
        //    new DrawMethod(c =>
        //    {
        //        Resources.Shaders.Ps.ColorSurface.SetParameters(c, Color.DarkBlue);
        //        Resources.Shaders.FullscreenProgram.Draw(c);
        //    }));

        //var lightInstr2 = DeferredPipeline.LightPass.AttachInstructions(
        //    new TransitionMethod(Resources.Shaders.FullscreenProgram.Set),
        //    new TransitionMethod(Resources.Shaders.Ps.Color.Set),
        //    new DrawMethod(Resources.Shaders.FullscreenProgram.Draw));

        //var lightInstr3 = DeferredPipeline.LightPass.AttachInstructions(
        //    new TransitionMethod(Resources.Shaders.FullscreenProgram.Set),
        //    new TransitionMethod(Resources.Shaders.Ps.Color.Set),
        //    new DrawMethod(Resources.Shaders.FullscreenProgram.Draw));

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
