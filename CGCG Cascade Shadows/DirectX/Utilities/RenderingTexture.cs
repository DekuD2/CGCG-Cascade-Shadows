using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows;

[Flags]
public enum ShaderUsage
{
    None = 0,
    DepthStencil = 1,
    ShaderResource = 2,
    RenderTarget = 4,
}

/// <summary>
/// Texture that autogenerates RTV and SRV.
/// It's purpose is to be a RTV/SRV wrapper, so that changes in the surface, which
/// change the textures, will not mess up rendering targets/sources.
/// Owns its DirectX interfaces and will dispose of them.
/// </summary>
public class RenderingTexture : IDisposable
{
    // Changes to descriptions are only visible after ResetTexture() is called!
    public RenderTargetViewDescription? RenderTargetViewDesc;
    public DepthStencilViewDescription? DepthStencilViewDesc;
    public ShaderResourceViewDescription? ShaderResourceViewDesc;

    public RenderingTexture(ShaderUsage GenerateViewsFlags)
        => ShaderInputFlags = GenerateViewsFlags;

    public event Action? Resized;

    public Texture2D Texture2D { get; private set; }
    public Texture2DDescription Description => Texture2D?.Description ?? new Texture2DDescription();
    public RenderTargetView? RenderTargetView { get; private set; }
    public DepthStencilView? DepthStencilView { get; private set; }
    public ShaderResourceView? ShaderResourceView { get; private set; }
    public ShaderUsage ShaderInputFlags { get; private set; }

    public void ReplaceTexture(Texture2D newTexture)
    {
        Dispose();

        // TODO: Maybe don't keep the Texture2D?
        // (WPF DX11 example disposed it after creating the RTV)
        Texture2D = newTexture;

        // TODO: Try providing empty desciptions instead. Maybe that's the default behavior?
        // In case it works, you can also remove flags and put descs in constructor instead
        if ((ShaderInputFlags & ShaderUsage.RenderTarget) > 0)
            RenderTargetView = RenderTargetViewDesc == null ?
                new RenderTargetView(Devices.Device3D, Texture2D) :
                new RenderTargetView(Devices.Device3D, Texture2D, RenderTargetViewDesc.Value);

        if ((ShaderInputFlags & ShaderUsage.DepthStencil) > 0)
            DepthStencilView = DepthStencilViewDesc == null ?
                 new DepthStencilView(Devices.Device3D, Texture2D) :
                 new DepthStencilView(Devices.Device3D, Texture2D, DepthStencilViewDesc.Value);

        if ((ShaderInputFlags & ShaderUsage.ShaderResource) > 0)
            ShaderResourceView = ShaderResourceViewDesc == null ?
                new ShaderResourceView(Devices.Device3D, Texture2D) :
                new ShaderResourceView(Devices.Device3D, Texture2D, ShaderResourceViewDesc.Value);

        Resized?.Invoke();
    }

    public void Dispose()
    {
        Texture2D?.Dispose();
        RenderTargetView?.Dispose();
        DepthStencilView?.Dispose();
        ShaderResourceView?.Dispose();
    }

    public static explicit operator Texture2D(RenderingTexture renderingTexture)
        => renderingTexture.Texture2D;

    public static explicit operator RenderTargetView?(RenderingTexture renderingTexture)
        => renderingTexture.RenderTargetView;

    public static explicit operator DepthStencilView?(RenderingTexture renderingTexture)
        => renderingTexture.DepthStencilView;

    public static explicit operator ShaderResourceView?(RenderingTexture renderingTexture)
        => renderingTexture.ShaderResourceView;
}
