using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Rendering.Meshes;
using FR.CascadeShadows.Resources;

using SharpDX;

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
        var renderer = new Renderer();
        var mesh = ResourceCache.Get<Mesh>(@"Models\ship.obj");
        // presenter.Output
        //var i = renderer.ForwardPass.AttachInstructions();
        //Console.WriteLine(Resources.Shaders.Ps.Color.Hi);

        while (true)
        {
            Devices.Context3D.ClearRenderTargetView(presenter.Output.RenderTargetView, Color.Red);
            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}
