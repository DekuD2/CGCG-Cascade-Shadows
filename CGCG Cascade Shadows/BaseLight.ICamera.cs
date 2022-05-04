using FR.CascadeShadows.Rendering;

using SharpDX;

namespace FR.CascadeShadows;

//public abstract partial class BaseLight : ICamera
//{
//    public bool Active => true;
//    public float Order => 1;
//    public Color Background => new(0f, 0f, 0f, 0f);
//    public RectangleF ViewportRectangle => new(0, 0, 1, 1);

//    public abstract Matrix View { get; }
//    public abstract Matrix Projection(float aspect);
//}

public class Camera : ICamera
{
    public bool Active => true;
    public float Order => 1;
    public Color Background => new(0f, 0f, 0f, 0f);
    public Viewport Viewport { get; private set; }

    public Matrix View { get; set; }
    public Matrix Projection { get; set; }

    public Vector3 Position { get; set; }

    public void SetTarget(Viewport viewport)
        => this.Viewport = viewport;
}
