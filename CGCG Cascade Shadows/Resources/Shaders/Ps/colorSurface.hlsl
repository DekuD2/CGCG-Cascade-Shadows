struct PsIn
{
	float4 position : SV_Position;
};

cbuffer ColorBuffer
{
	float4 color;
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

PsOut Main(PsIn input)
{
	PsOut output = (PsOut)0;

	output.position = color.rgb;
	output.albedo = color.rgb;
	output.normal = color.rgb;
	output.emission = color.rgb;
	output.gloss = 1;
	output.alpha = 1;

	return output;
}