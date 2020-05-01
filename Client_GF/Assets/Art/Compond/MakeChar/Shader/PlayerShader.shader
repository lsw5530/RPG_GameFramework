// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mogo/PlayerShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Layer1Tex("Layer1",2D) = "white"{}
		_Color ("Main Color", Color) = (1,1,1,1)
		_CtrlColor("CtrlColor",Color) = (1,1,1,1)
		_HitColor("Hit Color",Color) = (0,0,0,0)
		_BRDFTex ("NdotL NdotH (RGBA)", 2D) = "white" {}
		_HighLight("High Light",Float) = 1
	}
	
	SubShader { 
	Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
	LOD 210
		Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma surface surf PseudoBRDF exclude_path:prepass vertex:separateSH nolightmap noforwardadd noambient approxview
		
struct MySurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 OcclusionAndAmbientLight;
};

sampler2D _BRDFTex;
float4 _Color;

inline fixed4 LightingPseudoBRDF (MySurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
	// Half vector
	fixed3 halfDir = normalize (lightDir + viewDir);
	
	// N.L
	fixed NdotL = dot (s.Normal, lightDir);
	// N.H
	fixed NdotH = dot (s.Normal, halfDir);
	
	// remap N.L from [-1..1] to [0..1]
	// this way we can shade pixels facing away from the light - helps to simulate bounce lights
	fixed biasNdotL = NdotL * 0.5 + 0.5;
	
	// lookup light texture
	//  rgb = diffuse term
	//    a = specular term
	fixed4 l = tex2D (_BRDFTex, fixed2(biasNdotL, NdotH));

	fixed4 c;
	// mask specular term by Gloss factor
	// modulate specular with Albedo to allow metalic-ish look
	//
	// Shadowgun style: instead of adding LightProbes, treat them as both occlusion for MainLight and Ambient bounce
	// that is not physically correct, but it
	// 1) provides way to occlude MainLight without using runtime shadows
	// 2) allows bounce light to be affected by per-pixel normals
	// note that bounce lights becomes much weaker!
	c.rgb = s.OcclusionAndAmbientLight * s.Albedo * (l.rgb + s.Gloss * l.a) * 2;
	c.a = _Color.a;
	return c;
}


sampler2D _MainTex;
sampler2D _Layer1Tex;

struct Input {
	float2 uv_MainTex;
	float3 normal;
	fixed3 shOcclusionAndAmbient;
};

void separateSH (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	float3 worldN = mul ((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
	//o.normal = v.normal;
	o.shOcclusionAndAmbient = ShadeSH9 (float4(worldN,1.0));
}

void surf (Input IN, inout MySurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb;
//o.Albedo = (tex.r+tex.g+tex.b)*0.3*tex2D(_Layer1Tex,IN.uv_MainTex)*tex2D(_Layer1Tex,IN.uv_MainTex)*0.8;
	o.Gloss = tex.a;
	o.Alpha = tex.a;
	//o.Normal = IN.normal;
	o.OcclusionAndAmbientLight = IN.shOcclusionAndAmbient;
}
ENDCG

	}

	SubShader {
		LOD 205
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		

		 Pass
		 {
		
	Fog{Mode Off}
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Layer1Tex;
			fixed4 _Color;
			fixed4 _CtrlColor;
			fixed4 _HitColor;
			float _HighLight;

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float2	uv2 : TEXCOORD1;
				float2	uvStaticAlpha : TEXCOORD2;
			};

			float4 _MainTex_ST;
			float4 _Layer1Tex_ST;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uvStaticAlpha = v.texcoord;
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord,_Layer1Tex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				float a = tex2D(_MainTex,i.uvStaticAlpha).a;
				fixed4 col0 = tex2D(_MainTex,i.uv);
				fixed4 col1 = tex2D(_Layer1Tex,i.uv2);

				fixed4 result = col0 * _Color + col1 * a *_CtrlColor;
				result.a = col1.a * _CtrlColor.a * _Color.a;

				return fixed4(result.rgb*_HighLight,result.a);
				//return fixed4(1,1,1,1);
			}
			ENDCG
		 }
	} 
	SubShader {
		LOD 200
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		

		 Pass
		 {
		
	Fog{Mode Off}
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _HitColor;

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 result = tex2D(_MainTex,i.uv);
				return result;
			}
			ENDCG
		 }
	} 

	
}
