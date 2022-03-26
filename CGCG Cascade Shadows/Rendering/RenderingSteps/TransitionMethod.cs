using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public delegate void TransitionAction(DeviceContext1 context);

public class TransitionMethod : TransitionStateStep
{
    readonly TransitionAction transition;

    public TransitionMethod(TransitionAction transition)
        => this.transition = transition;

    public override void Enter(DeviceContext1 context)
        => transition.Invoke(context);
}