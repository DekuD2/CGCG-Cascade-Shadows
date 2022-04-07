using SharpDX;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Point = System.Windows.Point;

namespace FR.CascadeShadows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    MainViewModel viewModel;

    // Camera
    private Point? lastMousePos = null;
    private float cameraForwardSpeed = -0.01f;
    private float cameraSidewaySpeed = 0.03f;
    private float cameraRotateSpeed = 0.0044f;

    public MainWindow(MainViewModel dataContext)
    {
        DataContext = dataContext;
        viewModel = dataContext;

        InitializeComponent();
    }

    private void renderingTarget_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(renderingTarget);
            if (lastMousePos != null)
            {
                var delta = pos - lastMousePos;
                viewModel.MoveCameraCommand.Execute(
                    new Vector3(
                        (float)delta.Value.X * cameraSidewaySpeed,
                        (float)delta.Value.Y * cameraSidewaySpeed,
                        0));
            }
            lastMousePos = pos;
        }
        else if (e.RightButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(renderingTarget);
            if (lastMousePos != null)
            {
                var delta = pos - lastMousePos;
                viewModel.RotateCameraCommand.Execute(
                    new Vector2(
                        (float)delta.Value.X * cameraRotateSpeed,
                        (float)delta.Value.Y * cameraRotateSpeed));
            }
            lastMousePos = pos;
        }
        else
        {
            lastMousePos = null;
        }
    }

    private void renderingTarget_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        viewModel.MoveCameraCommand.Execute(new Vector3(0, 0, e.Delta * cameraForwardSpeed));
    }
}
