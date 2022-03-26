using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public abstract partial class TransitionStateStep : RenderingStep
{
    public static TransitionStateStep Empty { get; } = new NoTransition();

    class NoTransition : TransitionStateStep
    {
        public override void Enter(DeviceContext1 context) { }
    }
}
