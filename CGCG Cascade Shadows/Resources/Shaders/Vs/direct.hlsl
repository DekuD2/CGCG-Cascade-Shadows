struct VsIn
{
	float3 position : POSITION;
};

struct PsIn
{
	float4 position : SV_Position;
};

PsIn Main(VsIn input)
{
	PsIn output = (PsIn)0;

	output.position = float4(input.position, 1);

	return output;
}