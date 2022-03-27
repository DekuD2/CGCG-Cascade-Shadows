
using System;

namespace FR.CascadeShadows.Rendering;

public class RenderingInstructions : IDisposable
{
    readonly InnerNode rootNode;
    DrawableNode leafNode;

    internal RenderingInstructions(InnerNode root, DrawableNode leaf)
    {
        this.rootNode = root;
        this.leafNode = leaf;
    }

    public void Change(params IRenderingStep[] newSteps)
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
