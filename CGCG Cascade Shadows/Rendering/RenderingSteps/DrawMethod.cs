using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public delegate void DrawAction(DeviceContext1 context);

public class DrawMethod : DrawStep
{
    public static implicit operator DrawMethod(DrawAction draw)
        => new(draw);

    //SceneNode node;
    readonly DrawAction draw;

    // DrawAction must be method of a component.
    public DrawMethod(DrawAction draw)
    {
        //node = draw.Target as SceneNode;
        this.draw = draw;
    }

    //public DrawMethod(SceneNode node, DrawAction draw)
    //{
    //    this.node = node;
    //    this.draw = draw;
    //}

    public override bool Alive => true; //node?.Alive;

    public override void Draw(DeviceContext1 context)
        => draw.Invoke(context);

    public override bool Equals(object? obj)
        => obj switch
        {
            DrawMethod other => other.draw == draw,
            _ => false
        };

    public override int GetHashCode()
        => draw.GetHashCode() + 37;
}
