struct PsIn
{
	float4 position : SV_Position;
	float4 worldPosition : WORLDPOS;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;
};

struct PsOut
{
	float3 position : SV_Target0;
	float3 albedo : SV_Target1;
	float3 normal : SV_Target2;
	float3 emission : SV_Target3;
	float specular : SV_Target4;
	float gloss : SV_Target5;
	float alpha : SV_Target6;
};

Texture2D diffuseTexture : register(t0);
Texture2D normalTexture : register(t1);
Texture2D glossyTexture : register(t2);
Texture2D emissionTexture : register(t3);
Texture2D specularTexture : register(t4);

SamplerState texSampler
{
	Filter = ANISOTROPIC;
	AddressU = Wrap;
	AddressV = Wrap;
};

PsOut Main(PsIn input)
{
	PsOut output = (PsOut)0;

	output.position = input.worldPosition.xyz;
	output.alpha = 1;
	output.albedo = diffuseTexture.Sample(texSampler, input.uv).rgb; // * matDiffuse;

	float3 n = normalTexture.Sample(texSampler, input.uv).rgb;
	n.xy = n.xy * 2.0 - 1.0;
	output.normal = normalize(mul(n, float3x3(input.tangent, input.binormal, input.normal)));
	//output.normal = normalize((n.x * input.tangent) + (n.y * input.binormal) + (n.z * input.normal));

	output.specular = 1 - specularTexture.Sample(texSampler, input.uv).r;
	output.emission = emissionTexture.Sample(texSampler, input.uv).rgb;
	output.gloss = glossyTexture.Sample(texSampler, input.uv).r;

	return output;
	//return diffuseTexture.Sample(texSampler, input.uv);
}