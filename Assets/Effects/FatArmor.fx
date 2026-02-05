sampler uImage0 : register(s0); // UV texture
sampler uImage1 : register(s1); // Armor texture

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
	float4 tex = tex2D(uImage0, coords);
	if (tex.a < 0.5f)
		return float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 col = tex2D(uImage1, tex.xy);
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
