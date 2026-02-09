sampler uImage0 : register(s0);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
	float4 col = tex2D(uImage0, coords);
    col.rgb = lerp(col.rgb, 1.0f, 0.5f);
	return col;
}

technique Technique1
{
	pass MainPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
