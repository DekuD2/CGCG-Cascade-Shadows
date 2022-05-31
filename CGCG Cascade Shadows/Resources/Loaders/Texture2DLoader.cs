using SharpDX.Direct3D11;

using System.IO;

namespace FR.CascadeShadows.Resources.Loaders;

public class Texture2DLoader : FileLoader<Texture2D>
{
    public override Texture2D? Load(string uri)
    {
        bool linear = Path.GetFileNameWithoutExtension(uri).EndsWith("_l");
        linear = true;
        // I am actually confused but the output surface doesn't use sRGB space
        return TextureLoader.LoadTexture(Devices.WicFactory, Devices.Device3D, uri, linear);
    }
}