﻿struct PsIn
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
	float3 lightDir;
	bool castShadow;
	float3 lightColor;
	float intensity;

	matrix lightViewProjection;
};

Texture2D positionTexture : register(t0);
Texture2D albedoTexture : register(t1);
Texture2D normalTexture : register(t2);
Texture2D emissionTexture : register(t3);
Texture2D specularTexture : register(t4);
Texture2D glossTexture : register(t5);
Texture2D alphaTexture : register(t6);

Texture2D shadowTexture : register(t10);

SamplerState texSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Main(PsIn input) : SV_Target
{
	// Get surface data
	float3 position = positionTexture.Sample(texSampler, input.uv).rgb;
	float3 albedo = albedoTexture.Sample(texSampler, input.uv).rgb;
	float3 normal = normalTexture.Sample(texSampler, input.uv).rgb;
	float specular = specularTexture.Sample(texSampler, input.uv).r;
	float gloss = glossTexture.Sample(texSampler, input.uv).r;
	float alpha = alphaTexture.Sample(texSampler, input.uv).r;

	float4 lightSpacePos = mul(float4(position, 1), lightViewProjection);
	if (lightSpacePos.w == 0)
		lightSpacePos.w = 1.0;
	float2 shadowMapCoords = 0.5 * lightSpacePos.xy / lightSpacePos.w + float2(-0.5, 0.5);

	shadowMapCoords.x = 0.5 + (lightSpacePos.x / lightSpacePos.w * 0.5);
	shadowMapCoords.y = 0.5 - (lightSpacePos.y / lightSpacePos.w * 0.5);
	float depth = lightSpacePos.z / lightSpacePos.w;

	// if (shadowMapCoords.x > 1 || shadowMapCoords.x < 0 || shadowMapCoords.y > 1 || shadowMapCoords.y < 0)
	// 	return float4(0, 0, 0, 0);

	// shadowMapCoords = float2(0.5, 0.5);

	float lightMapDepth = lightSpacePos.z / lightSpacePos.w;

	float depthSample = shadowTexture.Sample(texSampler, shadowMapCoords).r;
	float depthDiff = depth - depthSample;

	// return float4(pow(depthDiff, 1), 0, 0, 1);
	// float cmp = shadowTexture.SampleCmpLevelZero(texSampler, shadowMapCoords, depth + 0.01);

	if (!castShadow || depthDiff < 0.001) // or not in shadow
	{
		//return float4(shadowMapCoords, 0, 1);
		//return float4(shadowUV.x * 255, shadowUV.y * 255, 0, 1);
		//return float4(shadowWorld, 1);

		normal = normalize(normal);

		float3 toLight = -normalize(lightDir);
		float distance = length(toLight);
		toLight = normalize(toLight);

		float attenuation = saturate(intensity);

		float3 color = saturate(dot(normal, toLight)) * albedo * lightColor * attenuation; // diffuse

		float3 toCamera = normalize(camera - position);
		float3 reflection = -normalize(reflect(toLight, normal));
		float refDot = saturate(dot(reflection, toCamera));
		color += pow(refDot, specular) * gloss * lightColor * attenuation; // specular

		return float4(color, alpha);
	}
	else
		//return float4(right * 1, up * 1, 0, 0);
		return float4(0, 0, 0, 0);
}