namespace FR.CascadeShadows.Rendering;

public static class RenderingInstructionsExtensions
{
    public static RenderingInstructions AttachInstructions(this InnerNode root, params RenderingStep[] steps)
        => new RenderingInstructions(root, root.Add(steps));
}
