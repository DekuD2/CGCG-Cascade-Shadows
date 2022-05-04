using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows;

public abstract partial class Light
{
    //public Vector3 Position { get; set; }
    //public abstract void Setup(DeviceContext1 context);
    public abstract void Render(DeviceContext1 context);
    //public abstract float Aspect { get; }
    //public abstract ICamera Camera { get; }
}
