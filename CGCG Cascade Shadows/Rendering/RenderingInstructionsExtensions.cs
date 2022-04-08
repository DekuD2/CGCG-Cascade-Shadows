using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FR.CascadeShadows.Rendering;

public record PartialSetup(InnerNode root, List<IRenderingStep> steps);

public static class RenderingInstructionsExtensions
{
    public static RenderingInstructions AttachInstructions(this InnerNode root, params IRenderingStep[] steps)
        => new(root, root.Add(steps));

    public static PartialSetup Set(this InnerNode root, TransitionStep transition)
        => new(root, new(5) { transition });

    public static PartialSetup Set(this InnerNode root, TransitionAction transitionAction)
        => new(root, new(5) { new TransitionMethod(transitionAction) });

    public static PartialSetup Then(this PartialSetup setup, TransitionStep transition)
    {
        setup.steps.Add(transition);
        return setup;
    }

    public static PartialSetup Then(this PartialSetup setup, TransitionAction transitionAction)
    {
        setup.steps.Add(new TransitionMethod(transitionAction));
        return setup;
    }

    public static RenderingInstructions ThenDraw(this PartialSetup setup, DrawStep draw)
    {
        setup.steps.Add(draw);
        return new(setup.root, setup.root.Add(CollectionsMarshal.AsSpan(setup.steps)));
    }

    public static RenderingInstructions ThenDraw(this PartialSetup setup, DrawAction drawAction)
    {
        setup.steps.Add(new DrawMethod(drawAction));
        return new(setup.root, setup.root.Add(CollectionsMarshal.AsSpan(setup.steps)));
    }

    public static RenderingInstructions Draw(this InnerNode root, DrawStep draw)
        => new(root, root.Add(new[] { draw }));

    public static RenderingInstructions Draw(this InnerNode root, DrawAction drawAction)
        => new(root, root.Add(new[] { new DrawMethod(drawAction) }));

    //public static RenderingInstructions AttachInstructions2(this InnerNode root, DrawStep draw, params TransitionStep[] transitions)
    //{
    //    IRenderingStep[] steps = new IRenderingStep[transitions.Length + 1];
    //    transitions.CopyTo(steps, 0);
    //    steps[^1] = draw;
    //    return new(root, root.Add(steps));
    //}
}
