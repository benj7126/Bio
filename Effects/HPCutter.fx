#define PI 3.14159265

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float uTime;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, uv);

    uv -= 0.5;
    uv *= 2;

	float r = atan2(uv.y, uv.x);

	if (r > fmod(uTime, PI))
		discard;

	return color;
}

technique Technique1
{
	pass GlowingDustPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}