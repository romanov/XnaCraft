float4x4 World;
float4x4 View;
float4x4 Projection;

Texture BlockTexture;
sampler BlockTextureSampler = sampler_state
{
	texture = <BlockTexture>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;	
	float2 TexCoords0 : TEXCOORD0;
	float2 TexCoords1 : TEXCOORD1;
	float2 TexCoords2 : TEXCOORD2;
	float2 TexCoords3 : TEXCOORD3;
	float2 TexCoords4 : TEXCOORD4;
	float2 TexCoords5 : TEXCOORD5;
	float2 TexCoords6 : TEXCOORD6;
	float2 TexCoords7 : TEXCOORD7;
	float2 TexCoords8 : TEXCOORD8;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoords0 : TEXCOORD0;
	float2 TexCoords1 : TEXCOORD1;
	float2 TexCoords2 : TEXCOORD2;
	float2 TexCoords3 : TEXCOORD3;
	float2 TexCoords4 : TEXCOORD4;
	float2 TexCoords5 : TEXCOORD5;
	float2 TexCoords6 : TEXCOORD6;
	float2 TexCoords7 : TEXCOORD7;
	float2 TexCoords8 : TEXCOORD8;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoords0 = input.TexCoords0;
    output.TexCoords1 = input.TexCoords1;
    output.TexCoords2 = input.TexCoords2;
    output.TexCoords3 = input.TexCoords3;
    output.TexCoords4 = input.TexCoords4;
    output.TexCoords5 = input.TexCoords5;
    output.TexCoords6 = input.TexCoords6;
    output.TexCoords7 = input.TexCoords7;
    output.TexCoords8 = input.TexCoords8;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 texColor = { 0, 0, 0, 1 };

    texColor += tex2D(BlockTextureSampler, input.TexCoords0);
    texColor += tex2D(BlockTextureSampler, input.TexCoords1);
    texColor += tex2D(BlockTextureSampler, input.TexCoords2);
    texColor += tex2D(BlockTextureSampler, input.TexCoords3);
    texColor += tex2D(BlockTextureSampler, input.TexCoords4);
    texColor += tex2D(BlockTextureSampler, input.TexCoords5);
    texColor += tex2D(BlockTextureSampler, input.TexCoords6);
    texColor += tex2D(BlockTextureSampler, input.TexCoords7);
    texColor += tex2D(BlockTextureSampler, input.TexCoords8);

	return texColor;
}

technique BlockTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
