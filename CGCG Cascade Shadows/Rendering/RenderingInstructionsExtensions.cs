using System;
using System.Linq;

namespace FR.CascadeShadows.Rendering;

public static class RenderingInstructionsExtensions
{
    public static RenderingInstructions AttachInstructions(this InnerNode root, params IRenderingStep[] steps)
        => new(root, root.Add(steps));

    //public static RenderingInstructions AttachInstructions2(this InnerNode root, DrawStep draw, params TransitionStep[] transitions)
    //{
    //    IRenderingStep[] steps = new IRenderingStep[transitions.Length + 1];
    //    transitions.CopyTo(steps, 0);
    //    steps[^1] = draw;
    //    return new(root, root.Add(steps));
    //}
}
