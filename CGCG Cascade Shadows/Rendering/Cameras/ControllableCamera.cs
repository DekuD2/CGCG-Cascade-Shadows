
using SharpDX;

namespace FR.CascadeShadows.Rendering.Cameras;

public partial class ControllableCamera : ICamera
{
    float rotX = 0;
    float rotY = 0;

    public ControllableCamera(float near, float far)
    {
        Near = near;
        Far = far;
    }

    public float Order { get; set; } = 0;
    public bool Active { get; set; } = true;

    public Color Background { get; set; } = Color.MediumPurple;
    public Viewport Viewport { get; private set; }
    float Aspect => (float)Viewport.Width / Viewport.Height;

    public float FieldOfView { get; set; } = 85;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.RotationYawPitchRoll(0, 0, 0);

    public Matrix View => Matrix.LookAtRH(
                Position,
                Position + Vector3.Transform(Vector3.ForwardRH, Rotation),
                Vector3.Up);

    public Matrix Projection => ProjectionSubfrustum(Near, Far);

    public float Near { get; set; }
    public float Far { get; set; }

    public Matrix ProjectionSubfrustum(float near, float far)
        => Matrix.PerspectiveFovRH(
            MathUtil.DegreesToRadians(FieldOfView) / Aspect,
            Aspect,
            near,
            far);

    public void Rotate(Vector2 amount)
    {
        rotX -= amount.X;
        rotY -= amount.Y;
        rotY = MathUtil.Clamp(rotY, -MathUtil.PiOverTwo + 0.05f, +MathUtil.PiOverTwo - 0.05f);
        Rotation = Quaternion.RotationYawPitchRoll(rotX, rotY, 0);
    }

    public void Move(Vector3 amount)
    {
        amount.X = -amount.X;
        Position += Vector3.Transform(amount, Rotation);
    }

    public void Reset()
    {
        rotX = 0;
        rotY = 0;
        Position = new Vector3(0, 1, 8);
        Rotation = Quaternion.RotationYawPitchRoll(0, 0, 0);
    }

    public void SetTarget(Viewport viewport)
        => Viewport = viewport;
}
