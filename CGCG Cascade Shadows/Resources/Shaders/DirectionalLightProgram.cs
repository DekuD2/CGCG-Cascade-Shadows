using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System.Runtime.InteropServices;

namespace FR.CascadeShadows.Resources.Shaders;

public static class DirectionalLightProgram
{
    static readonly PixelShader PixelShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl");
    public static readonly Buffer LightBuffer = new(Devices.Device3D, 32, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

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
        private float _0;
        public Color3 Color;
        public float Intensity;

        public LightParameters(Vector3 direction, Color? color = null, float intensity = 0.3f)
        {
            this.Direction = direction;
            this._0 = 0;
            this.Color = color?.ToColor3() ?? Color3.White;
            this.Intensity = intensity;
        }

        public void Set(DeviceContext1 context)
        {
            context.MapSubresource(LightBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
            stream.Write(this);
            context.UnmapSubresource(LightBuffer, 0);
        }
    }
}
