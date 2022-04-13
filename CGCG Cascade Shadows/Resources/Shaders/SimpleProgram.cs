using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.CascadeShadows.Resources.Shaders;

public static class SimpleProgram
{
    public static void Set(DeviceContext1 context)
    {
        Vs.Simple.Set(context);
        Ps.ColorSurface.Set(context);
    }

    public class Material : TransitionStep, IMaterial
    {
        public Color Diffuse { get; init; } = Color.White;
        public Color Emission { get; init; } = Color.Black;
        public float Gloss { get; init; } = 0f;
        public float SpecularPower { get; init; } = 1f;

        public TransitionStep ProgramStep => new TransitionMethod(Set);
        public TransitionStep MaterialStep => new TransitionMethod(Enter);

        public override void Enter(DeviceContext1 context)
            => Ps.ColorSurface.SetParameters(context, Diffuse.ToColor3(), Emission.ToColor3(), Gloss, SpecularPower);
    }
}
