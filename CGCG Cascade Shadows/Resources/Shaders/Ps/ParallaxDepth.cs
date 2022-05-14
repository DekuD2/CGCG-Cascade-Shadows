using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Runtime.InteropServices;

using SRV = SharpDX.Direct3D11.ShaderResourceView;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class ParallaxDepth
{
    public static PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\parallaxDepth.hlsl");

    public static void Set(DeviceContext1 context)
    {
        context.PixelShader.Set(Shader);
        //context.PixelShader.SetConstantBuffer(0, ConstantBuffers.Camera);
    }

    public static void SetParameters(
        DeviceContext1 context,
        SRV? depth)
    {
        context.PixelShader.SetShaderResource(5, depth); //Texture2D depthTexture : register(t5);

        context.PixelShader.SetSampler(0, SamplerStates.Default);
    }
}
