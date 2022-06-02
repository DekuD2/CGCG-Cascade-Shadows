struct PsIn
{
	float4 position : SV_Position;
	float4 worldPosition : WORLDPOS;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;
	float3 tbnEye : EYE;
	float3 tbnPos : EYE2;
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

cbuffer CameraBuffer : register(b1)
{
	float3 camera;
	int passType; // = { 0:normal, 1:shadows }
};

Texture2D diffuseTexture : register(t0);
Texture2D normalTexture : register(t1);
Texture2D glossyTexture : register(t2);
Texture2D emissionTexture : register(t3);
Texture2D specularTexture : register(t4);
Texture2D depthTexture : register(t5);

static const int layers = 20;
static const float heightScale = 0.05;

SamplerState texSampler
{
	Filter = ANISOTROPIC;
	AddressU = Wrap;
	AddressV = Wrap;
};

float3 ParallaxMap(float2 uv, float3 viewDir)
{
	float layerDepth = 1.0 / layers;
	float currL = 0.0;

	float2 delta = (viewDir.yx / abs(viewDir.z)) * heightScale * layerDepth;
	float deltaT = length(delta);
	float T = 0;

	float currH = depthTexture.Sample(texSampler, uv).r;
	float2 currUV = uv;

	float2 prevUV = currUV;
	float prevH = currH;
	float prevL = currL;

	for (int i = 0; i < layers; i++)
	{
		if (currL > currH)
			break;

		prevUV = currUV;
		prevH = currH;
		prevL = currL;

		currUV += delta;
		currH = depthTexture.Sample(texSampler, currUV).r;
		currL += layerDepth;
		T += deltaT;
	}

	float w = (prevH - prevL) / (prevH - currH + layerDepth);
	return float3(prevUV + w * delta, T - (1 - w) * deltaT);
}

PsOut Main(PsIn input)
{
	PsOut output = (PsOut)0;

	output.specular = 0;
	output.emission = 0;
	output.gloss = 0;

	output.position = input.worldPosition.xyz; // this should also change to calculate shadows properly
	output.alpha = 1;

	float3 ray = normalize(input.tbnPos - input.tbnEye);

	float3x3 tbn = float3x3(input.tangent, input.binormal, input.normal);

	float3 parallax = ParallaxMap(input.uv, ray);
	float2 uv = parallax.xy;
	//float3 delta = float3(uv - input.uv, parallax.z);

	float3 worldRay = normalize(output.position - camera);

	output.position += worldRay * parallax.z / heightScale;
	//output.emission = float3(0, parallax.z, 0);
	//output.emission = worldRay;
	//output.emission = camera * 20;

	if (uv.x != saturate(uv.x) || uv.y != saturate(uv.y))
		discard;
	//uv = input.uv;

	output.albedo = diffuseTexture.Sample(texSampler, uv).rgb; // * matDiffuse;

	float3 n = normalTexture.Sample(texSampler, uv).rgb;
	n.xy = n.yx;
	n.xy = n.xy * 2.0 - 1.0;
	output.normal = normalize(mul(n, tbn));

	// output.specular = 1 - specularTexture.Sample(texSampler, uv).r;
	// output.emission = emissionTexture.Sample(texSampler, uv).rgb;
	// output.gloss = glossyTexture.Sample(texSampler, uv).r;

	// TESTING
	//float d = depthTexture.Sample(texSampler, input.uv).r;
	//output.albedo = float3(d, d, d);
	//output.normal = float3(0, 1, 0);

	return output;
}