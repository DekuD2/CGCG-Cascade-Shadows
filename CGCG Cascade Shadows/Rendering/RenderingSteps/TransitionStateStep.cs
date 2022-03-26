using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract partial class TransitionStateStep : RenderingStep
{
    public abstract void Enter(DeviceContext1 context);
    public virtual void Exit(DeviceContext1 context) { }
}
