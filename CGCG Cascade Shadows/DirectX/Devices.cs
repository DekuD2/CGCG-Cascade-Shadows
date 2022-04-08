using SharpDX;
using SharpDX.Diagnostics;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using System;

using D2D = SharpDX.Direct2D1;
using D3D11 = SharpDX.Direct3D11;
using DW = SharpDX.DirectWrite;
using DXGI = SharpDX.DXGI;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using WIC = SharpDX.WIC;

namespace FR.CascadeShadows;

/// <summary>
/// Helper class to manage DirectX objects.
/// </summary>
public static class Devices
{
    // Stage shortcuts (using static Devices;)
    public static InputAssemblerStage IA => Context3D.InputAssembler;
    public static VertexShaderStage VS => Context3D.VertexShader;
    public static HullShaderStage HS => Context3D.HullShader;
    public static DomainShaderStage DS => Context3D.DomainShader;
    public static GeometryShaderStage GS => Context3D.GeometryShader;
    public static PixelShaderStage PS => Context3D.PixelShader;
    public static OutputMergerStage OM => Context3D.OutputMerger;
    public static ComputeShaderStage CS => Context3D.ComputeShader;
    public static RasterizerStage RS => Context3D.Rasterizer;

    // Direct3D
    public static D3D11.Device1 Device3D { get; private set; }
    public static D3D11.DeviceContext4 Context3D { get; private set; }

    // Direct2D
    public static D2D.Factory1 Factory2D { get; private set; }
    public static D2D.Device Device2D { get; private set; }
    public static D2D.DeviceContext Context2D { get; private set; }

    // Other
    public static DW.Factory DwFactory { get; private set; }
    public static WIC.ImagingFactory WicFactory { get; private set; }

    static readonly FeatureLevel[] direct3DFeatureLevels = new FeatureLevel[]
    {
            FeatureLevel.Level_11_1
    };

    static Devices()
    {
#if DEBUG
        // To solve bug causing spam of this spam being spammed in output:
        // "Exception thrown: 'SharpDX.Diagnostics.ObjectTracker.GetStackTraceException' in SharpDX.dll"
        // https://github.com/sharpdx/SharpDX/issues/698
        ObjectTracker.StackTraceProvider = () => Environment.StackTrace;
        Configuration.EnableObjectTracking = true;
#endif

        // Create 3D device and context
        var creationFlags = DeviceCreationFlags.BgraSupport;
#if DEBUG
        creationFlags |= DeviceCreationFlags.Debug;
#endif
        using (var device = new D3D11.Device(DriverType.Hardware, creationFlags, direct3DFeatureLevels))
            Device3D = device.QueryInterfaceOrNull<D3D11.Device1>();
        if (Device3D == null)
            throw new NotSupportedException("DirectX 11.1 Device1 is not supported!");
        //Context3D = Device3D.ImmediateContext.QueryInterface<D3D11.DeviceContext1>(); // Try d3dDevice.ImmediateContext1
        Context3D = Device3D.ImmediateContext1.QueryInterface<D3D11.DeviceContext4>();

        // Create factories
        var debugLevel = DebugLevel.None;
#if DEBUG
        debugLevel = DebugLevel.Information;
#endif
        Factory2D = new D2D.Factory1(FactoryType.SingleThreaded, debugLevel);
        DwFactory = new DW.Factory(DW.FactoryType.Shared);
        WicFactory = new WIC.ImagingFactory();

        // Create 2D device and context
        using (var dxgiDevice = Device3D.QueryInterface<DXGI.Device>())
            Device2D = new D2D.Device(Factory2D, dxgiDevice);
        Context2D = new D2D.DeviceContext(Device2D, DeviceContextOptions.None);

#if DEBUG
        Device3D.DebugName = "Main Device3D";
        Context3D.DebugName = "Main Context3D";
#endif
    }

    public static void Dispose()
    {
        Context3D?.Rasterizer.State?.Dispose();

        Device3D?.Dispose();
        Context3D?.Dispose();
        Factory2D?.Dispose();
        Device2D?.Dispose();
        Context2D?.Dispose();
        DwFactory?.Dispose();
        WicFactory?.Dispose();
    }
}