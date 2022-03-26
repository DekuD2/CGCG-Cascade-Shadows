using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public static class BlendStates
{
    public static BlendState Default { get; private set; } = new BlendState(Devices.Device3D, defaultDesc);
    public static BlendState Additive { get; private set; } = new BlendState(Devices.Device3D, additiveDesc); // For light
    public static BlendState Transparency { get; private set; } = new BlendState(Devices.Device3D, transparencyDesc);
    public static BlendState InvertedTransparency { get; private set; } = new BlendState(Devices.Device3D, transparencyImageDesc);
    public static BlendState Background { get; private set; } = new BlendState(Devices.Device3D, backgroundDesc);

    static BlendStateDescription defaultDesc
    {
        get
        {
            var desc = new BlendStateDescription();

            for (int i = 0; i < desc.RenderTarget.Length; i++)
            {
                desc.RenderTarget[i].IsBlendEnabled = false;
                desc.RenderTarget[i].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            }

            //for (int i = 0; i < desc.RenderTarget.Length; i++)
            //{
            //    desc.RenderTarget[i].IsBlendEnabled = false;
            //    desc.RenderTarget[i].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            //    desc.RenderTarget[i].SourceBlend = BlendOption.One; // == FACTOR (multiplier) of that color
            //    desc.RenderTarget[i].DestinationBlend = BlendOption.One;
            //    desc.RenderTarget[i].BlendOperation = BlendOperation.Add;

            //    desc.RenderTarget[i].SourceAlphaBlend = BlendOption.Zero;
            //    desc.RenderTarget[i].DestinationAlphaBlend = BlendOption.One;
            //    desc.RenderTarget[i].AlphaBlendOperation = BlendOperation.Add;
            //}
            //desc.IndependentBlendEnable = false;
            //desc.AlphaToCoverageEnable = false;

            return desc;
        }
    }

    static BlendStateDescription additiveDesc
    {
        get
        {
            var desc = new BlendStateDescription();

            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            desc.RenderTarget[0].SourceBlend = BlendOption.One; // == FACTOR (multiplier) of that color
            desc.RenderTarget[0].DestinationBlend = BlendOption.One;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Maximum;

            return desc;
        }
    }

    static BlendStateDescription transparencyDesc
    {
        get
        {
            var desc = new BlendStateDescription();

            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha; // == FACTOR (multiplier) of that color
            desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            desc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
            desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            return desc;
        }
    }

    static BlendStateDescription transparencyImageDesc
    {
        get
        {
            var desc = new BlendStateDescription();

            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha; // == FACTOR (multiplier) of that color
            desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            return desc;
        }
    }

    static BlendStateDescription backgroundDesc
    {
        get
        {
            var desc = new BlendStateDescription();

            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            desc.RenderTarget[0].SourceBlend = BlendOption.InverseDestinationAlpha; // == FACTOR (multiplier) of that color
            desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationAlpha;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            desc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
            desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Maximum;

            return desc;
        }
    }
}