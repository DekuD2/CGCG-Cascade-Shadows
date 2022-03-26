using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public delegate void DrawAction(DeviceContext1 context);

public class DrawMethod : DrawStep
{
    //SceneNode node;
    DrawAction draw;

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
}
