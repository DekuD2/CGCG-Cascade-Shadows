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
	float3 lightDir;
	bool castShadow;
	float3 lightColor;
	float intensity;

	float3 shadowCastPos;
	float shadowCastWidth;
	float shadowCastHeight;
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

	//float3 shadow = shadowTexture.Sample(texSampler, input.uv).rgb;

	// we need to find a ray (world position with the direction of light)
	// and intersect a plane of the light source
	// if we get intersection, we can get relative distance
	// project that with dot product or sum? (we should know the camera up TODO: exception when facing directly downward)
	// so project light direction onto up and normalize it

	// plane: a*x + b*y + c*z = d
	// ray:
	//  x = px + rx*k
	//  y = py + ry*k
	//  z = rz + rz*k
	// a*px + a*rx*k + b*py + b*ry*k + c*py + c*ry*k - d = 0
	// a.p + a.r * k = d

	// WAIT WE DON'T NEED THIS
	// We just need to project it onto a plane

	float3 rightDir = normalize(cross(float3(0, 1, 0), lightDir));
	// Idk if I need to normalize the second cross product
	float3 upDir = normalize(dot(rightDir, lightDir));

	float3 relative = position - shadowCastPos;
	float right = dot(rightDir, relative);
	float up = dot(upDir, relative);

	float2 shadowUV = float2(right / shadowCastWidth + 0.5, up / shadowCastHeight + 0.5);
	float3 shadowWorld = shadowTexture.Sample(texSampler, shadowUV).rgb;

	return float4(shadowUV.x, shadowUV.y, 0, 1);
	return float4(shadowWorld, 1);

	if (!castShadow || length(shadowWorld - position) > 2.0) // or not in shadow
	{
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