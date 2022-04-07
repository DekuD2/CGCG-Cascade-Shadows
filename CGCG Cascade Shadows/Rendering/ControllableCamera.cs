using SharpDX;

namespace FR.CascadeShadows.Rendering;

public class ControllableCamera : ICamera
{
    float rotX = 0;
    float rotY = 0;

    public float Order { get; set; } = 0;
    public bool Active { get; set; } = true;

    public Color Background { get; set; } = Color.MediumPurple;
    public RectangleF ViewportRectangle { get; set; } = new RectangleF(0, 0, 1, 1);
    //public IRenderingPipeline RenderingPipeline { get; set; } = new EditorPipeline();

    public float FieldOfView { get; set; } = 85;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.RotationYawPitchRoll(0, 0, 0);

    public Matrix View => Matrix.LookAtRH(
                Position,
                Position + Vector3.Transform(Vector3.ForwardRH, Rotation),
                Vector3.Up);

    public Matrix Projection(float aspect) => Matrix.PerspectiveFovRH(
                MathUtil.DegreesToRadians(FieldOfView) / aspect,
                aspect,
                0.01f,
                100f);

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
}
