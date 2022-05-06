struct VsIn
{
	float3 position : POSITION;
};

struct PsIn
{
	float4 position : SV_Position;
};

cbuffer TransformBuffer
{
	float4x4 world;
	float4x4 viewProjection;
};

PsIn Main(VsIn input) // : SV_Position
{
	PsIn output = (PsIn)0;

    output.position = mul(mul(float4(input.position, 1), world), viewProjection);

	return output;
}