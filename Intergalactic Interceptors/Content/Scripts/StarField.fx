#ifndef CommonFX
#include "Common.fx"
#endif

float4 iMouse;

#define iterations 20
#define formuparam 0.53

#define volsteps 3
#define stepsize 0.1

float zoom = 1.0f;
float tile = 1.0f;
float speed = 0.001f;

float brightness = 0.0015f;
float darkmatter = 0.300f;
float distfading = 0.730f;
float saturation = 0.850f;

void StarShader
(
	in   float2 In_TexCoord : TEXCOORD,
	out   float4 Out_Color : COLOR
)
{
	float d = tex2D(CameraDepthSampler, In_TexCoord).r;
	Out_Color = tex2D(CameraBaseSampler, In_TexCoord);
	if (d != 1.0f)
	{
		return;
	}

	//get coords and direction
	float2 uv=In_TexCoord-.5;
	uv.y*=iViewport.y/iViewport.x;
	float3 dir=float3(uv*zoom,1.);
	float time=iTime*speed;

	//mouse rotation
	float a1=.5+iMouse.x/iViewport.x*2.;
	float a2=.5+iMouse.y/iViewport.y*2.;
	float2x2 rot1=float2x2(cos(a1),sin(a1),-sin(a1),cos(a1));
	float2x2 rot2=float2x2(cos(a2),sin(a2),-sin(a2),cos(a2));
	dir.xz=mul(dir.xz,rot1);
	dir.xy=mul(dir.xy,rot2);
	float3 from=float3(1.0,2.0,3.0);
	from+=float3(time*0,-time*1,3.);
	from.xz=mul(from.xz,rot1);
	from.xy=mul(from.xy,rot2);
	
	//volumetric rendering
	float s=0.1,fade=1.;
	float3 v= float3(0.0,0.0,0.0);
	for (int r=0; r<volsteps; r++) {
		float3 p=from+s*dir*.5;
		p = abs((float3)(tile)-fmod(p,(float3)(tile*2.))); // tiling fold
		float pa,a=pa=0.;
		for (int i=0; i<iterations; i++) { 
			p=abs(p)/dot(p,p)-formuparam; // the magic formula
			a+=abs(length(p)-pa); // absolute sum of average change
			pa=length(p);
		}
		float dm=max(0.,darkmatter-a*a*.001); //dark matter
		a*=a*a; // add contrast
		if (r>6) fade*=1.-dm; // dark matter, don't render near
		//v+=vec3(dm,dm*.5,0.);
		v+=fade;
		v+=float3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
		fade*=distfading; // distance fading
		s+=stepsize;
	}
	v=lerp((float3)(length(v)),v,saturation); //color adjust
	Out_Color += float4(v*.01,0.);
}

texture2D StarPass;
sampler2D StarSamp = sampler_state
{
	texture = StarPass;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

technique StarTech
{
	pass StarPass
	<
		string Format = "A8R8G8B8";
		float WidthScale = 1.0f;
		float HeightScale = 1.0f;
	>
	{
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		AlphaTestEnable = false;
		CullMode = ccw;
		FillMode = solid;
		vertexshader = compile vs_3_0 ScreenQuad();
		pixelshader = compile ps_3_0 StarShader();
	}
}