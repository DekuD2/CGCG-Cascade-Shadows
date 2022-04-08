struct PsIn
{
    float4 position : SV_Position;
    float2 uv : TEXCOORD;
};

Texture2D tex : register(t0);

SamplerState texSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

cbuffer copyBuffer : register(b0)
{
	float3 multiplyBy;
    int textureIdx;
};

float4 Main(PsIn input) : SV_Target
{
    return float4(tex.Sample(texSampler, input.uv).rgb * multiplyBy, 1);
}