using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System.Runtime.InteropServices;

namespace FR.CascadeShadows.Resources.Shaders;

public static class DirectionalLightProgram
{
    static PixelShader PixelShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl");
    public static readonly Buffer LightBuffer = new(Devices.Device3D, 64, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

    public static void Recompile()
    {
        var newShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl", true);
        if (newShader != null)
            PixelShader = newShader;
    }

    public static void Set(DeviceContext1 context)
    {
        Fullscreen.Prepare(context);

        context.PixelShader.Set(PixelShader);
        context.PixelShader.SetConstantBuffer(0, ConstantBuffers.Camera);
        context.PixelShader.SetConstantBuffer(1, LightBuffer);
        context.PixelShader.SetSampler(0, SamplerStates.Default);
    }

    public static void Draw(DeviceContext1 context)
    {
        Fullscreen.Draw(context);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LightParameters
    {
        public Vector3 Direction;
        public bool CastShadow = false;
        private byte _p0 = 0;
        private byte _p1 = 0;
        private byte _p2 = 0;
        public Color3 Color;
        public float Intensity;
        private Matrix LightViewProjection = Matrix.Identity;

        public LightParameters(
            Vector3 direction,
            Color? color = null,
            float intensity = 0.3f)
        {
            this.Direction = direction;
            this.Color = color?.ToColor3() ?? Color3.White;
            this.Intensity = intensity;
        }

        public void SetShadowCast(ref LightParameters @this, Matrix lightViewProjection)
        {
            @this.CastShadow = true;
            @this.LightViewProjection = Matrix.Transpose(lightViewProjection);
        }

        public void Set(DeviceContext1 context)
        {
            context.MapSubresource(LightBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
            stream.Write(this);
            context.UnmapSubresource(LightBuffer, 0);
        }
    }
}
