using SharpDX.Direct3D11;

using System;
using System.Collections.Generic;
using FR.Core;

namespace FR.CascadeShadows.Rendering;

public class InnerNode : RenderingNode
{
    readonly List<RenderingNode> children = new();
    readonly TransitionStateStep transition;

    public InnerNode(TransitionStateStep transition)
        => this.transition = transition;

    public InnerNode(TransitionAction method)
        => this.transition = new TransitionMethod(method);

    public override bool Alive => children.Count > 0;

    public override void Render(DeviceContext1 context)
    {
        transition.Enter(context);

        foreach (var child in children.FastFilterAndRemove(x => x.Alive))
            child.Render(context);

        transition.Exit(context);
    }

    internal DrawableNode Add(Span<RenderingStep> steps)
    {
        if (steps.Length == 1)
        {
            if (steps[0] is not DrawStep obj)
                throw new ArgumentException($"Last rendering step must inherit from {nameof(DrawStep)}.");

            // Last step -> add a leaf node
            var leafNode = new DrawableNode(obj);
            children.Add(leafNode);
            return leafNode;
        }
        else if (steps.Length > 0)
        {
            if (steps[0] is not TransitionStateStep state)
                throw new ArgumentException($"Non-last rendering steps must inherit from {nameof(TransitionStateStep)}.");

            // Existing step -> no need to add a new node, just recursively "browse"
            foreach (var child in children)
                if (child is InnerNode innerNode && innerNode.transition == state)
                    return innerNode.Add(steps[1..]);

            // New step -> create a new node
            InnerNode node = new(state);
            children.Add(node);
            return node.Add(steps[1..]);
        }
        else
            throw new ArgumentException("The argument can't be an empty array.");
    }

    public static InnerNode Empty => new(TransitionStateStep.Empty);
}
