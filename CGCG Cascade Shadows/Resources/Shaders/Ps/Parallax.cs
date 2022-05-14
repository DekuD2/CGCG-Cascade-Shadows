using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Runtime.InteropServices;

using SRV = SharpDX.Direct3D11.ShaderResourceView;

namespace FR.CascadeShadows.Resources.Shaders.Ps;

public static class Parallax
{
    public static PixelShader Shader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\parallax.hlsl");
    //public static Buffer MaterialBuffer = new(Devices.Device3D, 32, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

    public static void Set(DeviceContext1 context)
    {
        context.PixelShader.Set(Shader);
        context.PixelShader.SetConstantBuffer(1, ConstantBuffers.Camera);
        //context.PixelShader.SetConstantBuffer(0, MaterialBuffer);
    }

    public static void Recompile()
    {
        var newShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\parallax.hlsl", true);
        if (newShader != null)
            Shader = newShader;
    }

    public static void SetParameters(
        DeviceContext1 context,
        SRV? diffuse,
        SRV? normal,
        SRV? glossy,
        SRV? emission,
        SRV? specular,
        SRV? depth)
    {
        context.PixelShader.SetShaderResource(0, diffuse);  //Texture2D diffuseTexture : register(t0);
        context.PixelShader.SetShaderResource(1, normal);   //Texture2D normalTexture : register(t1);
        context.PixelShader.SetShaderResource(2, glossy);   //Texture2D glossyTexture : register(t2);
        context.PixelShader.SetShaderResource(3, emission); //Texture2D emissionTexture : register(t3);
        context.PixelShader.SetShaderResource(4, specular); //Texture2D specularTexture : register(t4);
        context.PixelShader.SetShaderResource(5, depth); //Texture2D depthTexture : register(t5);

        context.PixelShader.SetSampler(0, SamplerStates.Default);
    }
}
