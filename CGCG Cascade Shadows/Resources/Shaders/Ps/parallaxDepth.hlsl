struct PsIn
{
	float4 position : SV_Position;
	float4 worldPosition : WORLDPOS;
	float2 uv : TEXCOORD;
	float3 tbnEye : EYE;
	float3 tbnPos : EYE2;
};

struct PsOut
{
	float depth : SV_Depth;
};

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

	float3 ray = normalize(input.tbnPos - input.tbnEye);
	float3 parallax = ParallaxMap(input.uv, ray);
	if (parallax.x != saturate(parallax.x) || parallax.y != saturate(parallax.y))
		discard;

	output.depth = input.position.z + abs(parallax.z);

	return output;
}