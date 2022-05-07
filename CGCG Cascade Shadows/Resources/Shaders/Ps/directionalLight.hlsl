struct PsIn
{
	float4 position : SV_Position;
	float2 uv : TEXCOORD; // uv relative to the viewport
};

//static const float3 toLight = normalize(float3(30, 30, 30)); // Test with directional light
static const float di = 0.8; // diffuse intensity
static const float si = 0.6; // specular intensity
static const float ai = 0.2; // ambient intensity

const bool VISUALISE = false;

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

	matrix projections[3];
};

Texture2D positionTexture : register(t0);
Texture2D albedoTexture : register(t1);
Texture2D normalTexture : register(t2);
Texture2D emissionTexture : register(t3);
Texture2D specularTexture : register(t4);
Texture2D glossTexture : register(t5);
Texture2D alphaTexture : register(t6);

Texture2D shadowTexture : register(t10);
Texture2D shadowTexture1 : register(t11);
Texture2D shadowTexture2 : register(t12);

SamplerState texSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};


float Shlick(float c, float3 l, float3 h)
{
	return c + (1 - c) * pow(1 - dot(l, h), 5);
}

float4 Main(PsIn input) : SV_Target
{
	// Get surface data
	float3 position = positionTexture.Sample(texSampler, input.uv).rgb;
	float3 albedo = albedoTexture.Sample(texSampler, input.uv).rgb;
	float3 normal = normalTexture.Sample(texSampler, input.uv).rgb;
	float specular = specularTexture.Sample(texSampler, input.uv).r;
	float gloss = glossTexture.Sample(texSampler, input.uv).r;
	float alpha = alphaTexture.Sample(texSampler, input.uv).r;

	//float4 lightSpacePos = mul(float4(position, 1), projections[0]);
	float4 lightSpacePos;
	float2 shadowMapCoords;
	float lightSpaceDepth;

	int i;
	for (i = 0; i < 3; i++)
	{
		lightSpacePos = mul(float4(position, 1), projections[i]);

		shadowMapCoords = float2(0.5 + (lightSpacePos.x / lightSpacePos.w * 0.5),
								 0.5 - (lightSpacePos.y / lightSpacePos.w * 0.5));

		if (shadowMapCoords.x != saturate(shadowMapCoords.x)
			|| shadowMapCoords.y != saturate(shadowMapCoords.y))
		{
			if (i == 3)
			{
				return float4(0, 0, 0, 0);
			}
			continue;
		}

		lightSpaceDepth = lightSpacePos.z / lightSpacePos.w;
		if (lightSpaceDepth < 0)
		{
			// TODO: UNCOMMENT
			//return float4(0, 0, 0, 0);
		}

		break;
	}

	//float2 shadowMapCoords = float2(0.5 + (lightSpacePos.x / lightSpacePos.w * 0.5),
	//								0.5 - (lightSpacePos.y / lightSpacePos.w * 0.5));

	//float lightSpaceDepth = lightSpacePos.z / lightSpacePos.w;

	// Check if within the light
	/*if (shadowMapCoords.x != saturate(shadowMapCoords.x)
		|| shadowMapCoords.y != saturate(shadowMapCoords.y)
		|| lightSpaceDepth < 0)
		return float4(0, 0, 0, 0);*/

	float lightMapDepth = lightSpacePos.z / lightSpacePos.w;

	float depthSample;
	if (i == 0)
		depthSample = shadowTexture.Sample(texSampler, shadowMapCoords).r;
	else if (i == 1)
		depthSample = shadowTexture1.Sample(texSampler, shadowMapCoords).r;
	else
		depthSample = shadowTexture2.Sample(texSampler, shadowMapCoords).r;

	float depthDiff = lightSpaceDepth - depthSample;

	if (VISUALISE)
	{
		if (i == 0)
			return float4(0.5, 0, 0, 1);
		else if (i == 1)
			return float4(0, 0.5, 0, 1);
		else
			return float4(0, 0, 0.5, 1);
	}

	// return float4(pow(depthDiff, 1), 0, 0, 1);
	// float cmp = shadowTexture.SampleCmpLevelZero(texSampler, shadowMapCoords, depth + 0.01);

	if (!castShadow || depthDiff < 0.001) // or not in shadow
	{
		// Calculate light normally
		normal = normalize(normal);

		float3 toLight = -normalize(lightDir);
		float distance = length(toLight);
		toLight = normalize(toLight);

		float attenuation = saturate(intensity);

		float3 color = saturate(dot(normal, toLight)) * albedo * lightColor * attenuation; // diffuse // I think it should also use fresnel and gloss maybe? (I am really not sure)

		float3 toCamera = normalize(camera - position);
		float3 reflection = -normalize(reflect(toLight, normal));
		float refDot = saturate(dot(reflection, toCamera));

		float fresnel = Shlick(gloss, toLight, normalize(toLight + toCamera));

		color += pow(refDot, specular) * /*gloss * //replaced with fresnel*/ lightColor * attenuation * fresnel; // specular

		return float4(color, alpha);
	}
	else
		return float4(0, 0, 0, 0);
	//return float4(right * 1, up * 1, 0, 0);
}