using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public sealed class DrawableNode : RenderingNode
{
    DrawStep obj;

    public DrawableNode(DrawStep obj)
        => this.obj = obj;

    public override bool Alive => obj.Alive && !Outdated;

    internal bool Outdated { get; private set; } = false;

    public override void Render(DeviceContext1 context)
        => obj.Draw(context);

    internal void Outdate()
        => Outdated = true;
}
