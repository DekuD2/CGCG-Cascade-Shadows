struct PsIn
{
	float4 position : SV_Position;
	float2 uv : TEXCOORD; // uv relative to the viewport
};

//static const float3 toLight = normalize(float3(30, 30, 30)); // Test with directional light
static const float di = 0.8; // diffuse intensity
static const float si = 0.6; // specular intensity
static const float ai = 0.2; // ambient intensity

static const bool VISUALISE = false;
static const bool VISUALISE_AFTER = true;
static const float DEPTH_BIAS = 0.001;

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

	matrix proyections[3];
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

SamplerState texSampler : register(s0);
SamplerComparisonState texCompSampler : register(s1);


float Shlick(float c, float3 l, float3 h)
{
	return c + (1 - c) * pow(1 - dot(l, h), 5);
}

float SampleShadow(int index, float2 uv)
{
	if (index == 0)
		return shadowTexture.Sample(texSampler, uv).r;
	else if (index == 1)
		return shadowTexture1.Sample(texSampler, uv).r;
	else
		return shadowTexture2.Sample(texSampler, uv).r;
}

float SampleShadowOffset(int index, float2 uv, int2 offset)
{
	if (index == 0)
		return shadowTexture.Sample(texSampler, uv, offset).r;
	else if (index == 1)
		return shadowTexture1.Sample(texSampler, uv, offset).r;
	else
		return shadowTexture2.Sample(texSampler, uv, offset).r;
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

	float4 lightSpacePos;
	float2 shadowMapCoords;
	float lightSpaceDepth;

	// Find the currect cascade
	int i;
	for (i = 0; i < 3; i++)
	{
		lightSpacePos = mul(float4(position, 1), proyections[i]);

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

		lightSpaceDepth = (lightSpacePos.z / lightSpacePos.w);
		if (lightSpaceDepth < 0)
		{
			// TODO: UNCOMMENT
			return float4(0, 0, 0, 0);
		}

		break;
	}

	// Sample depth
	float depthSample = SampleShadow(i, shadowMapCoords);
	float depthDiff = lightSpaceDepth - depthSample - DEPTH_BIAS;

	depthDiff = shadowTexture1.SampleCmpLevelZero(texCompSampler, shadowMapCoords, lightSpaceDepth - 10000);
	// return float4(shadowTexture1.Sample(texSampler, shadowMapCoords).r, 0, 0, 1);
	// return float4(depthDiff, depthDiff, depthDiff, 1);

	// Calculate depth percentage
	float depthPercentage = 0;
	for (int x = -1.5; x <= 1.5; x += 1.0)
	{
		for (int y = -1.5; y <= 1.5; y += 1.0)
		{
			depthPercentage += sign(saturate(lightSpaceDepth - SampleShadowOffset(i, shadowMapCoords, int2(x, y)) - DEPTH_BIAS));
			// TODO: use depth ddx/ddy here
		}
	}
	depthPercentage = 1 - (depthPercentage / 16.0);

	// UNCOMMENTING THIS DISABLES PCF
	//depthPercentage = 1 - sign(depthDiff);

	// return float4(depthPercentage, depthPercentage, depthPercentage, 1);

	if (VISUALISE)
	{
		if (i == 0)
			return float4(0.5, 0, 0, 1);
		else if (i == 1)
			return float4(0, 0.5, 0, 1);
		else
			return float4(0, 0, 0.5, 1);
	}

	if (!castShadow || depthPercentage > 0) // or not in shadow
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

		if (VISUALISE_AFTER)
		{
			if (i == 0)
				color *= float3(0.7, 0.3, 0.3);
			else if (i == 1)
				color *= float3(0.3, 0.7, 0.3);
			else
				color *= float3(0.3, 0.3, 0.7);
		}
		return float4(color * depthPercentage, alpha);
	}
	else
		return float4(0, 0, 0, 0);
	//return float4(right * 1, up * 1, 0, 0);
}