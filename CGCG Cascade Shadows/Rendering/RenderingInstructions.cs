
using System;

namespace FR.CascadeShadows.Rendering;

public class RenderingInstructions : IDisposable
{
    InnerNode rootNode;
    DrawableNode leafNode;

    public RenderingStep[]? Steps { get; private set; }

    internal RenderingInstructions(InnerNode root, DrawableNode leaf)
    {
        this.rootNode = root;
        this.leafNode = leaf;
    }

    public void Change(params RenderingStep[] newSteps)
    {
        leafNode.Outdate();
        leafNode = rootNode.Add(newSteps);
    }

    public void Detach()
    {
        leafNode.Outdate();
    }

    public void Dispose()
    {
        Detach();
    }
}
