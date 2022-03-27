using SharpDX.Direct3D11;

using System;

namespace FR.CascadeShadows.Rendering;

public abstract class DrawStep : IRenderingStep
{
    public abstract void Draw(DeviceContext1 context);
    public virtual bool Alive => true;
}

