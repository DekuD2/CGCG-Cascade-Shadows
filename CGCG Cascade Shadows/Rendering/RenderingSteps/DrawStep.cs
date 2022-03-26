using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract class DrawStep : RenderingStep
{
    public abstract void Draw(DeviceContext1 context);
    public virtual bool Alive => true;
}
