struct PsIn
{
	float4 position : SV_Position;
	float2 uv : TEXCOORD; // uv relative to the viewport
};

//static const float3 toLight = normalize(float3(30, 30, 30)); // Test with directional light
static const float di = 0.8; // diffuse intensity
static const float si = 0.6; // specular intensity
static const float ai = 0.2; // ambient intensity

cbuffer CameraBuffer : register(b0)
{
	float3 camera;
	int passType; // = { 0:normal, 1:shadows }
};

cbuffer LightBuffer : register(b1)
{
	float3 lightPos;
	float3 lightColor;
	float c1;
	float c2;
	float c3;
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

float4 Main(PsIn input) : SV_Target
{
	float3 position = positionTexture.Sample(texSampler, input.uv).rgb;
	float3 albedo = albedoTexture.Sample(texSampler, input.uv).rgb;
	float3 normal = normalTexture.Sample(texSampler, input.uv).rgb;
	//float3 emission = emissionTexture.Sample(texSampler, input.abs_uv).rgb;
	float specular = specularTexture.Sample(texSampler, input.uv).r;
	float gloss = glossTexture.Sample(texSampler, input.uv).r;
	float alpha = alphaTexture.Sample(texSampler, input.uv).r;

	normal = normalize(normal);

	float3 toLight = (lightPos - position);
	float distance = length(toLight);
	toLight = normalize(toLight);

	float attenuation = saturate(1.0 / (c1 + c2 * distance + c3 * distance * distance));

	float3 color = saturate(dot(normal, toLight)) * albedo * lightColor * attenuation; // diffuse

	float3 toCamera = normalize(camera - position);
	float3 reflection = -normalize(reflect(toLight, normal));
	float refDot = saturate(dot(reflection, toCamera));
	color += pow(refDot, specular) * gloss * lightColor * attenuation; // specular

	return float4(color, alpha);
}