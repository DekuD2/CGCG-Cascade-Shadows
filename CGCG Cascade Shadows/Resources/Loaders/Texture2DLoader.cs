using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Loaders;

public class Texture2DLoader : FileLoader<Texture2D>
{
    public override Texture2D? Load(string uri)
        => TextureLoader.LoadTexture(Devices.WicFactory, Devices.Device3D, uri);
}