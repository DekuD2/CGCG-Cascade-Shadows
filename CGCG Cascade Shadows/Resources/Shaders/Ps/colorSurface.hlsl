struct PsIn
{
    float4 position : SV_Position;
    float4 worldPos : WORLDPOS;
    float3 normal : NORMAL;
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

cbuffer ColorBuffer
{
    float3 diffuse;
    float gloss;
    float3 emission;
    float specular;
};

PsOut Main(PsIn input)
{
    PsOut output = (PsOut)0;

    output.position = input.worldPos.xyz;
    output.albedo = diffuse.rgb;
    output.normal = input.normal;
    output.emission = emission;
    output.gloss = gloss;
    output.specular = specular;
    output.alpha = 1;

    return output;
}