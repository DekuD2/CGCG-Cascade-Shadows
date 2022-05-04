struct PsIn
{
    float4 position : SV_Position;
    float2 uv : TEXCOORD;
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
    float3 position = positionTexture.Sample(texSampler, input.uv).rgb;
    float3 albedo = albedoTexture.Sample(texSampler, input.uv).rgb;
    float3 normal = normalTexture.Sample(texSampler, input.uv).rgb;
    float3 emission = emissionTexture.Sample(texSampler, input.uv).rgb;
	//float specular = specularTexture.Sample(texSampler, uv).r;
    float gloss = glossTexture.Sample(texSampler, input.uv).r;
	float alpha = alphaTexture.Sample(texSampler, input.uv).r;

    //return float4(albedo, 1);
    //ret   urn float4(0.1, 0.1, 0.1, 1);
	
    //return float4(albedo, 1);
    // 
    //return float4(albedo * ambientColor + emission, alpha);
    return float4(ambientColor * albedo + emission, alpha);
}