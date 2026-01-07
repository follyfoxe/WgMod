sampler uImage0 : register(s0); // The actual armor texture
sampler uImage1 : register(s1); // Belly uv map
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
	float2 bellyCoords = (floor(coords * uImageSize0) + 0.5f) / uImageSize1;
	float4 tex = tex2D(uImage1, bellyCoords);
	if (tex.a < 0.5f)
		return float4(0.0f, 0.0f, 0.0f, 0.0f);
	float2 uv = (floor(tex.xy * uImageSize0) + 0.5f) / uImageSize0;
	float4 col = tex2D(uImage0, uv);
	col.rgb *= 1.0f - tex.b;
	return col * color;
}

technique Technique1
{
	pass MainPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
