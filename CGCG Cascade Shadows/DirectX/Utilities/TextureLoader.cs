using SharpDX;
using SharpDX.Direct3D11;
using Format = SharpDX.DXGI.Format;
using SharpDX.WIC;
using System.IO;

namespace FR.CascadeShadows;
public static class TextureLoader
{
    public static ShaderResourceView LoadShaderResourceView(string filename)
    {
        var tex = LoadTexture(Devices.WicFactory, Devices.Device3D, filename);
        var srv = new ShaderResourceView(Devices.Device3D, tex);
        tex.Dispose();
        return srv;
    }

    private static BitmapSource LoadBitmap(ImagingFactory wicFactory, Stream stream)
    {
        var bitmapDecoder = new BitmapDecoder(
            wicFactory,
            stream,
            DecodeOptions.CacheOnDemand);

        var formatConverter = new FormatConverter(wicFactory);

        formatConverter.Initialize(bitmapDecoder.GetFrame(0),
            //PixelFormat.Format32bppPRGBA,
            PixelFormat.Format32bppRGBA,
            BitmapDitherType.None,
            null,
            0.0,
            BitmapPaletteType.Custom);

        return formatConverter;
    }

    public static Texture2D LoadTexture(ImagingFactory wicFactory, Device device, Stream stream)
    {
        BitmapSource bitmapSource = LoadBitmap(wicFactory, stream);
        int stride = bitmapSource.Size.Width * 4;
        using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
        {
            bitmapSource.CopyPixels(stride, buffer);
            Texture2DDescription desc = DefaultTexture2DDescription;
            desc.Width = bitmapSource.Size.Width;
            desc.Height = bitmapSource.Size.Height;
            return new Texture2D(device, desc, new DataRectangle(buffer.DataPointer, stride));
        }
    }

    private static BitmapSource LoadBitmap(ImagingFactory wicFactory, string file)
    {
        var bitmapDecoder = new BitmapDecoder(
            wicFactory,
            file,
            DecodeOptions.CacheOnDemand);

        var formatConverter = new FormatConverter(wicFactory);

        formatConverter.Initialize(bitmapDecoder.GetFrame(0),
            //PixelFormat.Format32bppPRGBA,
            PixelFormat.Format32bppRGBA,
            BitmapDitherType.None,
            null,
            0.0,
            BitmapPaletteType.Custom);

        return formatConverter;
    }

    public static Texture2D LoadTexture(ImagingFactory wicFactory, Device device, string file)
    {
        BitmapSource bitmapSource = LoadBitmap(wicFactory, file);
        int stride = bitmapSource.Size.Width * 4;
        using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
        {
            bitmapSource.CopyPixels(stride, buffer);
            Texture2DDescription desc = DefaultTexture2DDescription;
            desc.Width = bitmapSource.Size.Width;
            desc.Height = bitmapSource.Size.Height;
            return new Texture2D(device, desc, new DataRectangle(buffer.DataPointer, stride));
        }
    }

    public static Texture2DDescription DefaultTexture2DDescription
        => new Texture2DDescription()
        {
            ArraySize = 1,
            BindFlags = BindFlags.ShaderResource,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = Format.R8G8B8A8_UNorm,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
        };
}
