using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract class RenderingNode
{
    public abstract void Render(DeviceContext1 context);
    public abstract bool Alive { get; }
}
