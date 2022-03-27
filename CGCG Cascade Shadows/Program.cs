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
        var ship = ResourceCache.Get<Mesh>(@"Models\ship.obj");
        // presenter.Output
        //var i = renderer.ForwardPass.AttachInstructions();
        //Console.WriteLine(Resources.Shaders.Ps.Color.Hi);

        Mesh mesh = new();
        mesh.Positions = new Vector3[] { new Vector3(-1, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 1, 1) };
        mesh.Indices = new uint[] { 0, 1, 2 };
        var geometry = mesh.GeometryData;

        void Draw(DeviceContext1 context)
        {
            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            context.InputAssembler.InferVertexBuffers(geometry);
            context.InputAssembler.InferInputLayout();

            context.Draw(3, 0);
        }

        var shipInstr = renderer.ForwardPass.AttachInstructions(
            new TransitionMethod(Resources.Shaders.Vs.Direct.Set),
            new TransitionMethod(Resources.Shaders.Ps.Color.Set),
            new DrawMethod(Draw));


        //var shipInstr2 = renderer.ForwardPass.AttachInstructions2(
        //    Draw,
        //    Resources.Shaders.Vs.Direct.Set,
        //    Resources.Shaders.Ps.Color.Set);


        while (true)
        {
            Devices.Context3D.ClearRenderTargetView(presenter.Output.RenderTargetView, Color.LightSteelBlue);

            renderer.Render();
            //renderer.ForwardPass.Render(Devices.Context3D);

            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}
