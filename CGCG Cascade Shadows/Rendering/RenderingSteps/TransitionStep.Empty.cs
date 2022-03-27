using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract partial class TransitionStep : IRenderingStep
{
    public static TransitionStep Empty { get; } = new NoTransition();

    class NoTransition : TransitionStep
    {
        public override void Enter(DeviceContext1 context) { }
    }
}
