struct PsIn
{
	float4 position : SV_Position;
};

cbuffer ColorBuffer
{
	float4 color;
};

float4 Main(PsIn input) : SV_Target
{
	//return float4(1, 0, 1, 1);
	return color;
}