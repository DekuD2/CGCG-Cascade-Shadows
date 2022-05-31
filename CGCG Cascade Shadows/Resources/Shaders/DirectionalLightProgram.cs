using FR.CascadeShadows.Rendering;

using SharpDX;
using SharpDX.Direct3D11;

using System.Runtime.InteropServices;

namespace FR.CascadeShadows.Resources.Shaders;

public static class DirectionalLightProgram
{
    static PixelShader PixelShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl");
    public static readonly Buffer LightBuffer = new(Devices.Device3D, 272, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

    public static void Recompile()
    {
        var newShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl", true);
        if (newShader != null)
            PixelShader = newShader;
    }

    public static SamplerState Sampler = SamplerStates.Shadow;

    public static void Set(DeviceContext1 context)
    {
        Fullscreen.Prepare(context);

        context.PixelShader.Set(PixelShader);
        context.PixelShader.SetConstantBuffer(0, ConstantBuffers.Camera);
        context.PixelShader.SetConstantBuffer(1, LightBuffer);
        context.PixelShader.SetConstantBuffer(2, Settings.CBuffer);
        context.PixelShader.SetSampler(0, Sampler);
        context.PixelShader.SetSampler(1, SamplerStates.ShadowComp);
    }

    public static void Draw(DeviceContext1 context)
    {
        Fullscreen.Draw(context);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LightParameters
    {
        // First rule - pack 4 byters
        // Second rule - never cross 16 bytes
        public Vector3 Direction;
        public bool CastShadow = false;
        private byte _p0 = 0;
        private byte _p1 = 0;
        private byte _p2 = 0; // 16B
        public Color3 Color;
        public float Intensity; // 16B
        private Matrix projection1 = Matrix.Identity; // 16B
        private Matrix projection2 = Matrix.Identity; // 16B
        private Matrix projection3 = Matrix.Identity; // 16B
        private float invRes1; // inverse resolutions
        private Color3 _p4 = default;
        private float invRes2;
        private Color3 _p5 = default;
        private float invRes3;
        private Color3 _p6 = default;
        // NEW
        //public float Offset1; // the limiters
        //public float Offset2; // I CAN'T MAN WTF IS WRONG WITH MY BRAIN
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        //private Matrix[] projections = new Matrix[2];

        public LightParameters(
            Vector3 direction,
            int res1,
            int res2,
            int res3,
            Color? color = null,
            float intensity = 0.3f)
        {
            this.Direction = direction;
            this.Color = color?.ToColor3() ?? Color3.White;
            this.Intensity = intensity;
            this.invRes1 = 1 / (float)res1;
            this.invRes2 = 1 / (float)res2;
            this.invRes3 = 1 / (float)res3;
        }

        public void SetProjection(ref LightParameters @this, Matrix projection, int index = 0)
        {
            @this.CastShadow = true;
            if (index == 0)
                @this.projection1 = Matrix.Transpose(projection);
            if (index == 1)
                @this.projection2 = Matrix.Transpose(projection);
            if (index == 2)
                @this.projection3 = Matrix.Transpose(projection);

            int size = Utilities.SizeOf<LightParameters>();
        }

        public void UpdateResolution(ref LightParameters @this, int res, int index)
        {
            @this.CastShadow = true;
            if (index == 0)
                @this.invRes1 = 1 / (float)res;
            if (index == 1)
                @this.invRes2 = 1 / (float)res;
            if (index == 2)
                @this.invRes3 = 1 / (float)res;
        }

        public void Set(DeviceContext1 context)
        {
            context.MapSubresource(LightBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
            stream.Write(this);
            context.UnmapSubresource(LightBuffer, 0);
        }
    }
}
