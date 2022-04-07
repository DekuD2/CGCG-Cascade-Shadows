using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.IO;

namespace FR.CascadeShadows;

public static class Program
{
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
        // presenter.Output
        //var i = renderer.ForwardPass.AttachInstructions();
        //Console.WriteLine(Resources.Shaders.Ps.Color.Hi);

        Mesh mesh = new();
        mesh.Positions = new Vector3[] { new Vector3(-1, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 1, 1) };
        mesh.Normals = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
        mesh.Indices = new uint[] { 0, 1, 2 };
        var geometry = mesh.GeometryData;

        void Draw(DeviceContext1 context)
        {
            ConstantBuffers.UpdateTransform(context, Matrix.Scaling(0.1f));
            context.DrawLastGeometry();
        }

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
            new DrawMethod(Draw));

        var lightInstr = DeferredPipeline.LightPass.AttachInstructions(
            new TransitionMethod(Resources.Shaders.AmbientProgram.Set),
            new Resources.Shaders.AmbientProgram.Parameters(Color.Green),
            new DrawMethod(Resources.Shaders.AmbientProgram.Draw));

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
            //Devices.Context3D.ClearRenderTargetView(presenter.Output.RenderTargetView, Color.LightSteelBlue);

            renderer.Render();
            //renderer.ForwardPass.Render(Devices.Context3D);

            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}
