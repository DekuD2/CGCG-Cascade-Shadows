using FR.CascadeShadows.Rendering;

using SharpDX;

namespace FR.CascadeShadows;

public class DumbCamera : ICamera
{
    public bool Active => true;
    public float Order => 1;
    public Color Background => new(0f, 0f, 0f, 0f);
    public Viewport Viewport { get; private set; }

    public Matrix View { get; set; }
    public Matrix Projection { get; set; }

    public Vector3 Position { get; set; }

    public float Near => throw new System.NotImplementedException();

    public float Far => throw new System.NotImplementedException();

    public Matrix ProjectionSubfrustum(float near, float far)
    {
        throw new System.NotImplementedException();
    }

    public void SetTarget(Viewport viewport)
        => this.Viewport = viewport;
}
