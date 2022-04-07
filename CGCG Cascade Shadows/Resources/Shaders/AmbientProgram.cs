using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Resources.Shaders;

public static class AmbientProgram
{
    public static void Set(DeviceContext1 context)
    {
        FullscreenProgram.Set(context);
        Ps.Ambient.Set(context);
    }

    public static void Draw(DeviceContext1 context)
    {
        FullscreenProgram.Draw(context);
    }

    public class Parameters : TransitionStep
    {
        Color color;

        public Parameters(Color color) 
            => this.color = color;

        public override void Enter(DeviceContext1 context) 
            => Ps.Ambient.SetParameters(context, ref color);
    }
}
