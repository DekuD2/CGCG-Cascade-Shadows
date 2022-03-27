using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract partial class TransitionStep : IRenderingStep
{
    public static implicit operator TransitionStep(TransitionAction transition)
        => new TransitionMethod(transition);

    public abstract void Enter(DeviceContext1 context);
    public virtual void Exit(DeviceContext1 context) { }
}
