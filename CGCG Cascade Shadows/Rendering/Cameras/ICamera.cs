using SharpDX;

namespace FR.CascadeShadows.Rendering;

public interface ICamera
{
    bool Active { get; }
    float Order { get; }
    Matrix View { get; }
    Matrix Projection { get; }
    Matrix ProjectionSubfrustum(float near, float far);
    Vector3 Position { get; }
    float Near { get; }
    float Far { get; }
    Color Background { get; }

    Viewport Viewport { get; }
    void SetTarget(Viewport rectangle);
}
