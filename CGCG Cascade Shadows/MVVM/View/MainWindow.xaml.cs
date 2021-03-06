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
        dataContext.Error += MainViewModel_Error;

        //PresentationSource.FromVisual(this).CompositionTarget.

        InitializeComponent();
    }

    private void renderingTarget_MouseMove(object sender, MouseEventArgs e)
    {
        if (zoomCb.IsChecked == true && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(renderingTarget);
            if (lastMousePos != null)
            {
                var delta = pos - lastMousePos;
                //var dist = (float)Math.Sqrt((float)delta.Value.X * (float)delta.Value.X + (float)delta.Value.Y * (float)delta.Value.Y);
                var dist = (float)delta.Value.X - (float)delta.Value.Y;

                viewModel.MoveCameraCommand.Execute(new Vector3(0, 0, dist * 2f * cameraForwardSpeed));
            }
            lastMousePos = pos;
        }
        if (e.LeftButton == MouseButtonState.Pressed && rotateCb.IsChecked != true)
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
        else if (e.RightButton == MouseButtonState.Pressed || (e.LeftButton == MouseButtonState.Pressed && rotateCb.IsChecked == true))
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

    private void MainViewModel_Error(string error)
    {
        //errorPopupMessage.Text = error;
        //errorPopup.IsOpen = true;

        string messageBoxText = error;
        string caption = "Error";
        MessageBoxButton button = MessageBoxButton.OK;
        MessageBoxImage icon = MessageBoxImage.Error;
        MessageBoxResult result;

        result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        errorPopup.IsOpen = false;
    }

    private void zoomCb_Checked(object sender, RoutedEventArgs e)
    {
        rotateCb.IsChecked = false;
    }

    private void rotateCb_Checked(object sender, RoutedEventArgs e)
    {
        zoomCb.IsChecked = false;
    }
}
