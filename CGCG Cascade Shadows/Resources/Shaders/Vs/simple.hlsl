struct VsIn
{
	float3 position : POSITION;
	float3 normal : NORMAL;
};

struct PsIn
{
	float4 position : SV_Position;
    float4 worldPos : WORLDPOS;
	float3 normal : NORMAL;
};

cbuffer TransformBuffer
{
	float4x4 world;
	float4x4 viewProjection;
};

PsIn Main(VsIn input)
{
	PsIn output = (PsIn)0;

    output.worldPos = mul(float4(input.position, 1), world);
    output.position = mul(output.worldPos, viewProjection);
    output.normal = normalize(mul(float4(input.normal, 0), world).xyz);

	return output;
}