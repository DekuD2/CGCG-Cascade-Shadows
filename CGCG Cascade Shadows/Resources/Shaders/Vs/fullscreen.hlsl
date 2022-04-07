struct PsIn
{
	float4 position : SV_Position;
	float2 uv : TEXCOORD;
};

PsIn Main(uint id : SV_VertexID)
{
	PsIn output;

	output.uv = float2((id >> 1) & 1, id & 1);
	output.position = float4(output.uv * float2(2, -2) + float2(-1, 1), 0, 1);

	return output;
}