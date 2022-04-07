using SharpDX;

namespace FR.CascadeShadows.Rendering;

public interface ICamera
{
    bool Active { get; }
    float Order { get; }
    Matrix View { get; }
    Matrix Projection(float aspect);
    Vector3 Position { get; }
    Color Background { get; }
    RectangleF ViewportRectangle { get; }

}
