#ifndef CommonFX
#define CommonFX
#endif

#define MOD3 float3(0.1031f, 0.11369f, 0.13787f)
static const float Math_PI =3.1416f;// atan(-1);

float iTime;
float4 iViewport = float4(800, 600, 1000, 10);	// Width, Height, FarClip, NearClip
matrix iViewProjectionInverseMatrix, iViewInverseMatrix;

texture2D iCameraBaseTexture;
sampler2D CameraBaseSampler = sampler_state
{
	texture = iCameraBaseTexture;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

texture2D iCameraDepthTexture;
sampler2D CameraDepthSampler = sampler_state
{
	texture = iCameraDepthTexture;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

texture2D iCameraMetallicTexture;
sampler2D CameraMetallicSampler = sampler_state
{
	texture = iCameraMetallicTexture;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

texture2D iCameraNormalTexture;
sampler2D CameraNormalSampler = sampler_state
{
	texture = iCameraNormalTexture;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

texture2D iCameraRoughnessTexture;
sampler2D CameraRoughnessSampler = sampler_state
{
	texture = iCameraRoughnessTexture;
	AddressU = clamp;
	AddressV = clamp;
	AddressW = clamp;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
};

/*texture3D NoiseBoolTexture;
sampler3D NoiseBoolSampler = sampler_state
{
	texture = NoiseBoolTexture;
	AddressU = Wrap;
	AddressV = Wrap;
	AddressW = Wrap;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};*/

// Vertex shader for rendering the full-screen quad
void ScreenQuad
(
	inout float4 IO_Position: POSITION0,
	inout float2 IO_TexCoord: TEXCOORD0
)
{
	IO_TexCoord += 1.0f / (iViewport.xy * 2.0f);
}

float4 ScreenSpacePosition(in float2 In_TexCoord, in float In_Depth)
{
	float4 Out_Position;
	Out_Position.x = In_TexCoord.x * 2.0f - 1.0f;
	Out_Position.y = -(In_TexCoord.y * 2.0f - 1.0f);
	Out_Position.z = 0.0f;
	Out_Position.w = 1.0f;
	Out_Position = mul(Out_Position, iViewProjectionInverseMatrix);
	Out_Position *= In_Depth;
	Out_Position += (1.0f - Out_Position.w) * iViewInverseMatrix[3];
	return Out_Position;
}

float4 ScreenSpacePosition2(in float2 In_TexCoord, in float In_Depth)
{
	float4 Out_Position = 1.0f;
	Out_Position.x = In_TexCoord.x * 2.0f - 1.0f;
	Out_Position.y = -(In_TexCoord.y * 2.0f - 1.0f);
	Out_Position.z = ((In_Depth - iViewport.w) / (iViewport.z - iViewport.w));
	float a = ((((Out_Position.x * iViewProjectionInverseMatrix._m03) + (Out_Position.y * iViewProjectionInverseMatrix._m13)) + (Out_Position.y * iViewProjectionInverseMatrix._m23)) + iViewProjectionInverseMatrix._m33); 
	Out_Position = mul(iViewProjectionInverseMatrix, Out_Position);
	return Out_Position / a;
}

float rand(in float2 co){
	return frac(sin(dot(co.xy, float2(12.9898f, 78.233f))) * 43758.5453f);
}

float hash(float p)
{
	float3 p3  = frac((float3)p * MOD3.x);
	p3 += dot(p3, p3.yzx + 19.19f);
	return frac((p3.x + p3.y) * p3.z);
}

float hash12(in float2 p)
{
	float3 p3  = frac(float3(p.xyx) * MOD3);
	p3 += dot(p3, p3.yzx + 19.19f);
	return frac((p3.x + p3.y) * p3.z);
}

float2 hash22(in float2 p)
{
	float3 p3 = frac(float3(p.xyx) * MOD3);
	p3 += dot(p3, p3.yzx + 19.19f);
	return frac(float2((p3.x + p3.y) * p3.z, (p3.x + p3.z) * p3.y));
}

float3 hash33(in float3 p3)
{
	float3 h3 = frac(p3 * float3(443.897f, 441.423f, 437.195f));
	h3 += dot(h3, h3.yxz + 19.19f);
	return frac((h3.xxy + h3.yxx) * h3.zyx);
}