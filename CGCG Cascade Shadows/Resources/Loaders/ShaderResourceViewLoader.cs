using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Loaders;

public class ShaderResourceViewLoader : FileLoader<ShaderResourceView>
{
    public override ShaderResourceView? Load(string uri)
    {
        var texture = ResourceCache.Get<Texture2D>(uri);
        return new ShaderResourceView(Devices.Device3D, texture);
    }
}
