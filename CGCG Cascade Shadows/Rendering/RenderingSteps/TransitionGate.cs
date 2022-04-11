using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public class TransitionGate : TransitionStep
{
    public bool ShowNode = true;
    public override bool Show => ShowNode;

    public override void Enter(DeviceContext1 context) { }
}
