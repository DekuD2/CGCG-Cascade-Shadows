using SharpDX;

namespace FR.CascadeShadows.Rendering;

public interface ICamera
{
    bool Active { get; }
    float Order { get; }
    Matrix View { get; }
    Matrix Projection { get; }
    Vector3 Position { get; }
    Color Background { get; }

    Viewport Viewport { get; }
    void SetTarget(Viewport rectangle);
}
