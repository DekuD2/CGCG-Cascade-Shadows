using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public delegate void TransitionAction(DeviceContext1 context);

public class TransitionMethod : TransitionStep
{
    public static implicit operator TransitionMethod(TransitionAction transition)
        => new(transition);

    readonly TransitionAction transition;

    public TransitionMethod(TransitionAction transition)
        => this.transition = transition;

    public override void Enter(DeviceContext1 context)
        => transition.Invoke(context);

    public override bool Equals(object? obj)
        => obj switch
        {
            TransitionMethod other => other.transition == transition,
            _ => false
        };

    public override int GetHashCode()
        => transition.GetHashCode() + 117;
}