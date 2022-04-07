struct PsIn
{
    float4 position : SV_Position;
    float2 rel_uv : TEXCOORD; // uv relative to the viewport
    float2 abs_uv : TEXCOORD1; // uv relative to whole screen and not just viewport
};

Texture2D positionTexture : register(t0);
Texture2D albedoTexture : register(t1);
Texture2D normalTexture : register(t2);
Texture2D emissionTexture : register(t3);
Texture2D specularTexture : register(t4);
Texture2D glossTexture : register(t5);
Texture2D alphaTexture : register(t6);

SamplerState texSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

cbuffer colorBuffer : register(b0)
{
	float3 ambientColor;
};

float4 Main(PsIn input) : SV_Target
{
    float3 position = positionTexture.Sample(texSampler, input.abs_uv).rgb;
    float3 albedo = albedoTexture.Sample(texSampler, input.abs_uv).rgb;
    float3 normal = normalTexture.Sample(texSampler, input.abs_uv).rgb;
    float3 emission = emissionTexture.Sample(texSampler, input.abs_uv).rgb;
	//float specular = specularTexture.Sample(texSampler, input.uv).r;
    float gloss = glossTexture.Sample(texSampler, input.abs_uv).r;
	float alpha = alphaTexture.Sample(texSampler, input.abs_uv).r;

    //return float4(albedo, 1);
    //return float4(0.1, 0.1, 0.1, 1);
	
    //return float4(normal, 1);
    // 
    //return float4(albedo * ambientColor + emission, alpha);

    return float4(albedo, 1);
}