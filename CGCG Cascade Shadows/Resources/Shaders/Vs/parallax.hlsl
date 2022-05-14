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
	float3 tbnEye : EYE;
	float3 tbnPos : EYE2;
};

cbuffer TransformBuffer : register(b0)
{
	float4x4 world;
	float4x4 viewProjection;
};

cbuffer CameraBuffer : register(b1)
{
	float3 camera;
	int passType; // = { 0:normal, 1:shadows }
};

PsIn Main(VsIn input)
{
	PsIn output = (PsIn)0;

	output.worldPosition = mul(float4(input.position, 1), world);
	output.position = mul(output.worldPosition, viewProjection);

	output.normal = normalize(mul(float4(input.normal, 0), world).xyz);
	output.tangent = normalize(mul(float4(input.tangent, 0), world).xyz);
	output.binormal = normalize(mul(float4(input.binormal, 0), world).xyz);

	// Calculate TBN space stuff
	float3x3 TBN = transpose(float3x3(output.tangent, output.binormal, output.normal));
	// This is actually not exactly correct (it's not the eye position in TBN space),
	// because we lose the translation.
	// But that doesn't matter because we are taking the difference between those two vectors, so the translations cancel out. 
	output.tbnEye = mul(camera, TBN);
	output.tbnPos = mul(output.worldPosition, TBN);

	output.uv = input.uv;

	return output;
}