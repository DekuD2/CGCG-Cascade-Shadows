using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Wpf.Interop.DirectX;
using System.Windows.Threading;
using System.Windows;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Windows.Media;
using System.Diagnostics;
using SharpDX;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace FR.CascadeShadows;
public class DirectXPresenter
{
    // Target
    Window window;
    ContentPresenter target;

    // DirectX
    D3D11Image dxImageSource = new();
    Texture2D? wpfSurface;
    Image dxImage;

    public DirectXPresenter(ContentPresenter target)
    {
        // UI
        this.window = Window.GetWindow(target);
        this.target = target;

        target.SizeChanged += (s, e) => WindowSizeChanged();
        dxImageSource.WindowOwner = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        dxImageSource.OnRender = Sync;

        WindowSizeChanged();

        dxImage = new Image()
        {
            Source = dxImageSource,
            Stretch = Stretch.Fill,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        target.Content = dxImage;
    }

    // Events
    public static event Action? OutputResized;

    // Target
    public ContentPresenter Target => target;

    // UI
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Aspect { get; private set; }

    // DirectX
    public RenderingTexture Output { get; } = new RenderingTexture(ShaderUsage.ShaderResource | ShaderUsage.RenderTarget);


    void WindowSizeChanged()
    {
        int width = Target.ActualWidth < 1 ? 1 : (int)Math.Ceiling(Target.ActualWidth);
        int height = Target.ActualHeight < 1 ? 1 : (int)Math.Ceiling(Target.ActualHeight);
        dxImageSource.SetPixelSize(width, height);
    }

    public void Present()
    {
        dxImageSource.Lock();

        // Synchronize surfaces
        Sync(IntPtr.Zero, false);
        // Redraw
        dxImageSource.AddDirtyRect(new Int32Rect(0, 0, dxImageSource.PixelWidth, dxImageSource.PixelHeight));

        dxImageSource.Unlock();
    }

    public void Sync(IntPtr surface, bool isNewSurface)
    {
        Texture2D SurfaceToTexture2D(IntPtr surfacePtr)
        {
            var dxSurface = new Surface(surfacePtr);
            var dxgiResource = dxSurface.QueryInterface<SharpDX.DXGI.Resource>();
            var sharedHandle = dxgiResource.SharedHandle;

            var dx11Resource = Devices.Device3D.OpenSharedResource<SharpDX.Direct3D11.Resource>(sharedHandle);
            var outputResource = dx11Resource.QueryInterface<Texture2D>();

            dxgiResource.Dispose();
            dx11Resource.Dispose();

            return outputResource;
        }

        Devices.Context3D.Flush(); // Force the rendering to finish

        if (isNewSurface || wpfSurface == null) // Create a new surface
        {
            // TODO: Render with output as source texture and new buffer as output. (rescale)
            wpfSurface?.Dispose();
            wpfSurface = SurfaceToTexture2D(surface);

            if (wpfSurface.Description.Width == 0 || wpfSurface.Description.Height == 0)
                return;

            Width = wpfSurface.Description.Width;
            Height = wpfSurface.Description.Height;
            Aspect = Width / (float)Height;

            Output.ResetTexture(new Texture2D(Devices.Device3D, wpfSurface.Description));

            OutputResized?.Invoke();
        }
        else // Present
        {
            // I kinda don't like that I don't have a swapchain now
            Devices.Context3D.CopyResource(Output.Texture2D, wpfSurface);
            // dxImageSource.SetBackBuffer()?
            // I think it is done automatically but I have to make a back buffer or sth?
        }
    }
}
