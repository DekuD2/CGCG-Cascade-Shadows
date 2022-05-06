using Microsoft.Wpf.Interop.DirectX;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

//using SwapChainPanel = Windows.UI.Xaml.Controls.SwapChainPanel;

namespace FR.CascadeShadows;

public class DirectXPresenter
{
    // Target
    readonly Window window;
    readonly ContentPresenter target;

    // DirectX
    readonly D3D11Image dxImageSource = new();
    readonly Image dxImage;
    //readonly SwapChainPanel swapChainPanel = new();
    Texture2D? wpfSurface;

    TimeSpan lastRender = TimeSpan.Zero;

    public DirectXPresenter(ContentPresenter target)
    {
        // UI
        this.window = Window.GetWindow(target);
        this.target = target;

        target.SizeChanged += (s, e) => WindowSizeChanged();
        dxImageSource.WindowOwner = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        dxImageSource.OnRender = Sync;

        CompositionTarget.Rendering += (s, e) => dxImageSource.RequestRender();

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
    public RenderingTexture Output { get; } = new RenderingTexture(ShaderUsage.RenderTarget);


    void WindowSizeChanged()
    {
        int width = Target.ActualWidth < 1 ? 1 : (int)Math.Ceiling(Target.ActualWidth);
        int height = Target.ActualHeight < 1 ? 1 : (int)Math.Ceiling(Target.ActualHeight);
        dxImageSource.SetPixelSize(width, height);
    }

    public void Present()
    {
        // Synchronize surfaces
        // Is this necessary
        dxImageSource.Lock();
        //dxImageSource.RequestRender();
        //Sync(IntPtr.Zero, false);
        // Redraw
        dxImageSource.AddDirtyRect(new Int32Rect(0, 0, dxImageSource.PixelWidth, dxImageSource.PixelHeight));
        dxImageSource.Unlock();
    }

    SwapChain2 swapChain;
    public void Sync(IntPtr surface, bool isNewSurface)
    {
        Texture2D SurfaceToTexture2D(IntPtr surfacePtr)
        {
            var dxSurface = new Surface(surfacePtr);
            var dxgiResource = dxSurface.QueryInterface<SharpDX.DXGI.Resource>();
            var sharedHandle = dxgiResource.SharedHandle;

            var dx11Resource = Devices.Device3D.OpenSharedResource<SharpDX.Direct3D11.Resource>(sharedHandle);
            var outputResource = dx11Resource.QueryInterface<Texture2D>();

            //SwapChainDescription1 desc = new SwapChainDescription1()
            //{
            //    AlphaMode = AlphaMode.Ignore,
            //    BufferCount = 2,
            //    Usage = Usage.RenderTargetOutput,
            //    Format = outputResource.Description.Format,
            //    Height = outputResource.Description.Height,
            //    Width = outputResource.Description.Width,
            //    SampleDescription =
            //    {
            //        Count = 1
            //    },
            //    SwapEffect = SwapEffect.FlipSequential,
            //};
            //var swapChain1 = new SwapChain1(Devices.DxgiFactory, Devices.Device3D, ref desc/*, restrictToOutput: output*/);
            //swapChain = swapChain1.QueryInterface<SwapChain2>();

            //using (ISwapChainPanelNative native = ComObject.As<ISwapChainPanelNative>(null))
            //    native.SwapChain = swapChain;

            //var backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

            dxgiResource.Dispose();
            dx11Resource.Dispose();

            return outputResource;
        }

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

            Debug.WriteLine("Resizing");
            // Try to make a swapchain? (for debug)

            Output.ReplaceTexture(new Texture2D(Devices.Device3D, wpfSurface.Description));
            //dxImageSource.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, );
            //Output.ReplaceTexture(swapChain.GetBackBuffer<Texture2D>(0));

            OutputResized?.Invoke();
        }
        else // Present
        {
            Devices.Context3D.Flush(); // Force the rendering to finish
                                       // I kinda don't like that I don't have a swapchain now
                                       //swapChain.Present(1, PresentFlags.None);
            Devices.Context3D.CopyResource(Output.Texture2D, wpfSurface);
            //dxImageSource.
            // dxImageSource.SetBackBuffer()?
            // I think it is done automatically but I have to make a back buffer or sth?
        }
    }
}
