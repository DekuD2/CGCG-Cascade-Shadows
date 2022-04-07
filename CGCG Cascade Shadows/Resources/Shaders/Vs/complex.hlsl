struct VsIn
{
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;
};

struct PsIn
{
	float4 position : SV_Position;
	float4 worldPosition : WORLDPOS;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float3 tangent : TANGENT;
	float3 binormal : BINORMAL;
};

cbuffer TransformBuffer
{
	float4x4 world;
	float4x4 viewProjection;
};

PsIn Main(VsIn input)
{
	PsIn output = (PsIn)0;
	
	output.worldPosition = mul(float4(input.position, 1), world);
	output.position = mul(output.worldPosition, viewProjection);
	output.normal = normalize(mul(float4(input.normal, 0), world).xyz);
	output.tangent = normalize(mul(float4(input.tangent, 0), world).xyz);
	output.binormal = normalize(mul(float4(input.binormal, 0), world).xyz);
	//output.normal = normalize(mul(float4(0, 1, 0, 0), world).xyz); // so that it's in objects space
	//output.tangent = normalize(mul(float4(1, 0, 0, 0), world).xyz); // rather then in tangent space
	//output.binormal = normalize(mul(float4(0, 0, 1, 0), world).xyz);
	output.uv = input.uv;

	return output;
}