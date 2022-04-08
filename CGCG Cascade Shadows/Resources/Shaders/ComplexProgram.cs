using FR.CascadeShadows.Rendering;

using SharpDX.Direct3D11;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.CascadeShadows.Resources.Shaders;

public static class ComplexProgram
{
    public static void Set(DeviceContext1 context)
    {
        Vs.Complex.Set(context);
        Ps.Complex.Set(context);
    }

    public class Material : TransitionStep
    {
        public ShaderResourceView? Diffuse { get; init; }
        public ShaderResourceView? Normal { get; init; }
        public ShaderResourceView? Glossy { get; init; }
        public ShaderResourceView? Emission { get; init; }
        public ShaderResourceView? Specular { get; init; }

        public override void Enter(DeviceContext1 context)
            => Ps.Complex.SetParameters(context, Diffuse, Normal, Glossy, Emission, Specular);
    }
}
