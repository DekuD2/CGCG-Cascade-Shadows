﻿using FR.CascadeShadows.Rendering;
using FR.CascadeShadows.Resources.Loaders;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using System.Runtime.InteropServices;
using System.Windows.Markup;

namespace FR.CascadeShadows.Resources.Shaders;

public static class Fullscreen
{
    static readonly VertexShaderInfo VertexShader = ResourceCache.Get<VertexShaderInfo>(@"Shaders\Vs\fullscreen.hlsl");

    public static void Prepare(DeviceContext1 context)
    {
        context.VertexShader.Set(VertexShader);
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
    }

    public static void Draw(DeviceContext1 context)
    {
        context.Draw(4, 0);
    }
}

public static class PointLightProgram
{
    static readonly PixelShader PixelShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\pointLight.hlsl");
    public static readonly Buffer LightBuffer = new Buffer(Devices.Device3D, 48, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

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
        public Vector3 Position;
        private float _0;
        public Color3 Color;
        public float C1;
        public float C2;
        public float C3;

        public LightParameters(Vector3 Position, Color? color = null, float c1 = 0.3f, float c2 = 0.2f, float c3 = 0.1f)
        {
            this.Position = Position;
            this._0 = 0;
            this.Color = color?.ToColor3() ?? Color3.White;
            this.C1 = c1;
            this.C2 = c2;
            this.C3 = c3;
        }

        public void Set(DeviceContext1 context)
        {
            context.MapSubresource(LightBuffer, MapMode.WriteDiscard, MapFlags.None, out var stream);
            stream.Write(this);
            context.UnmapSubresource(LightBuffer, 0);
        }
    }
}

public static class DirectionalLightProgram
{
    static readonly PixelShader PixelShader = ResourceCache.Get<PixelShader>(@"Shaders\Ps\directionalLight.hlsl");
    public static readonly Buffer LightBuffer = new Buffer(Devices.Device3D, 32, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

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
