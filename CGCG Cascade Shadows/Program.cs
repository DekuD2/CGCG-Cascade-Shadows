using SharpDX;

using System;

namespace FR.CascadeShadows;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        MainViewModel viewModel = new MainViewModel();
        MainWindow window = new MainWindow(viewModel);

        window.Show();

        var presenter = viewModel.GetDirectXPresenter().Result;

        // presenter.Output

        while (true)
        {
            Devices.Context3D.ClearRenderTargetView(presenter.Output.RenderTargetView, Color.Red);
            WpfDispatcher.ProcessMessages();
            presenter.Present();
        }
    }
}