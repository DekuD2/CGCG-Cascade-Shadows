struct PsIn
{
	float4 position : SV_Position;
	float2 uv : TEXCOORD; // uv relative to the viewport
};

//static const float3 toLight = normalize(float3(30, 30, 30)); // Test with directional light
static const float di = 0.8; // diffuse intensity
static const float si = 0.6; // specular intensity
static const float ai = 0.2; // ambient intensity

static const int VISUALISE = 0;
static const int PCF_MODE = 3;
static const float DEPTH_BIAS = 0.0001;

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
	float inverseResolutions[3];
};

cbuffer SettingsBuffer : register(b2)
{
	int s_visualise; // 0 = no visualisation; 1 = mult visualisation; 2 = override visualisation
	int s_pcf_mode; // 0 = no pcf; 1 = no pcf 4x4; 2 = pcf; 3 = pcf 4x4; 4 = pcf 3x3 gaussian
	float s_depth_bias;
	bool s_blend_between_cascades;
}

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

float SamplePCF(int index, float2 uv, float compare)
{
	if (index == 0)
		return shadowTexture.SampleCmpLevelZero(texCompSampler, uv, compare, int2(0, 0));
	else if (index == 1)
		return shadowTexture1.SampleCmpLevelZero(texCompSampler, uv, compare, int2(0, 0));
	else
		return shadowTexture2.SampleCmpLevelZero(texCompSampler, uv, compare, int2(0, 0));
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

float SampleShadowOffsetCompare(int index, float2 uv, int2 offset, float compare)
{
	return sign(saturate(SampleShadowOffset(index, uv, offset) + compare));
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
	float texScale;

	// Find the currect cascade index
	int i;
	for (i = 0; i < 3; i++)
	{
		// world position -> position from the light space
		lightSpacePos = mul(float4(position, 1), projections[i]);

		// from [-1,1] space to [0,1] space (+ divide by w)
		shadowMapCoords = float2(0.5 + (lightSpacePos.x / lightSpacePos.w * 0.5),
								 0.5 - (lightSpacePos.y / lightSpacePos.w * 0.5));


		// If we are outside of the cascade, try the next one
		if (shadowMapCoords.x != saturate(shadowMapCoords.x)
			|| shadowMapCoords.y != saturate(shadowMapCoords.y))
		{
			// Unless it's the last cascade in which case we assume shadow
			if (i == 3)
			{
				return float4(0, 0, 0, 0);
			}

			continue;
		}

		// If we got here, we found our cascade

		if (VISUALISE == 2)
		{
			if (i == 0)
				return float4(0.5, 0, 0, 1);
			else if (i == 1)
				return float4(0, 0.5, 0, 1);
			else
				return float4(0, 0, 0.5, 1);
		}

		// calculate the depth from the POV of light
		lightSpaceDepth = (lightSpacePos.z / lightSpacePos.w);
		// get our textureScale for PCF kernels
		texScale = inverseResolutions[i];

		// TODO: here calculate blend% and stuff

		// I don't remember this, I think it might be unnecessary
		if (lightSpaceDepth < 0)
		{
			return float4(0, 0, 0, 0);
		}

		break;
	}

	float depthPercentage = 0.0;
	float compValue = lightSpaceDepth - DEPTH_BIAS;

	// Calculate depth percentage using our selected PCF mode
	if (PCF_MODE == 0)
	{
		depthPercentage = SampleShadowOffsetCompare(i, shadowMapCoords, int2(0,0), compValue);
	}
	else if (PCF_MODE == 1)
	{
		for (float x = -1.5; x <= 1.55; x += 1.0)
		{
			for (float y = -1.5; y <= 1.55; y += 1.0)
			{
				depthPercentage += SampleShadowOffsetCompare(i, shadowMapCoords + float2(x, y) * texScale, int2(0,0), compValue);
			}
		}
		depthPercentage = 1 - (depthPercentage / 16.0);
	}
	else if (PCF_MODE == 2)
	{
		depthPercentage = SamplePCF(i, shadowMapCoords, compValue);
	}
	else if (PCF_MODE == 3)
	{
		for (float x = -1.5; x <= 1.55; x += 1.0)
		{
			for (float y = -1.5; y <= 1.55; y += 1.0)
			{
				depthPercentage += SamplePCF(i, shadowMapCoords + float2(x, y) * inverseResolutions[i], compValue);
				// TODO: use depth ddx/ddy here
			}
		}
		depthPercentage /= 16.0;
	}
	else if (PCF_MODE == 4)
	{

	}

	if (!castShadow || depthPercentage > 0) // or not in shadow
	{
		// Calculate light normally
		normal = normalize(normal);

		float3 toLight = -normalize(lightDir);
		float distance = length(toLight);
		toLight = normalize(toLight);

		float attenuation = saturate(intensity);

		float3 toCamera = normalize(camera - position);
		float3 reflection = -normalize(reflect(toLight, normal));
		float refDot = saturate(dot(reflection, toCamera));

		float fresnel = Shlick(gloss, toLight, normalize(toLight + toCamera));

		float3 color = saturate(dot(normal, toLight)) * albedo * (1.0 - fresnel) * lightColor * attenuation; // diffuse // I think it should also use fresnel and gloss maybe? (I am really not sure)
		color += pow(refDot, specular) * gloss * /*replaced with fresnel*/ lightColor * attenuation * fresnel; // specular

		if (VISUALISE == 1)
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
}