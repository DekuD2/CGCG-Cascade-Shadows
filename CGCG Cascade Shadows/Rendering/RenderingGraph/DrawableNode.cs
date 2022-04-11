using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public sealed class DrawableNode : RenderingNode
{
    readonly DrawStep step;

    public DrawableNode(DrawStep obj)
        => this.step = obj;

    public DrawableNode(DrawAction action)
        => this.step = new DrawMethod(action);

    public override bool Alive => step.Alive && !Outdated;

    public override bool Show => step.Show;

    internal bool Outdated { get; private set; } = false;

    public override void Render(DeviceContext1 context)
    {
        if (Show)
            step.Draw(context);
    }

    internal void Outdate()
        => Outdated = true;
}
