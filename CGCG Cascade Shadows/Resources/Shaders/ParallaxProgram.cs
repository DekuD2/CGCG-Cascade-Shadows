using FR.CascadeShadows.Rendering;

using SharpDX.Direct3D11;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.CascadeShadows.Resources.Shaders;

public static class ParallaxProgram
{
    public static void Set(DeviceContext1 context)
    {
        Vs.Parallax.Set(context);
        Ps.Parallax.Set(context);
    }

    public class Material : TransitionStep, IMaterial, IDepthMaterial
    {
        public ShaderResourceView? Diffuse { get; init; }
        public ShaderResourceView? Normal { get; init; }
        public ShaderResourceView? Glossy { get; init; }
        public ShaderResourceView? Emission { get; init; }
        public ShaderResourceView? Specular { get; init; }
        public ShaderResourceView? Displacement { get; init; }

        public TransitionStep ProgramStep => new TransitionMethod(Set);
        public TransitionStep MaterialStep => new TransitionMethod(Enter);
        public TransitionStep DepthStep { get; }

        public Material()
            => DepthStep = new ParallaxDepthStep(this);

        public override void Enter(DeviceContext1 context)
            => Ps.Parallax.SetParameters(context, Diffuse, Normal, Glossy, Emission, Specular, Displacement);

        class ParallaxDepthStep : TransitionStep
        {
            public Material Material { get; }

            public ParallaxDepthStep(Material material) 
                => Material = material;

            public override void Enter(DeviceContext1 context)
            {
                Vs.ParallaxDepth.Set(context);
                Ps.ParallaxDepth.Set(context);
                Ps.ParallaxDepth.SetParameters(context, Material.Displacement);
            }

            public override void Exit(DeviceContext1 context)
            {
                context.PixelShader.Set(null);
                base.Exit(context);
            }
        }
    }
}
