// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mogo/MonsterShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		//_HitColor("Hit Color",Color) = (0,0,0,0)
		_FinalPower("FinalPower",Float) = 1
	}
	SubShader {
		LOD 200
		 Pass
		 {
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			//fixed4 _HitColor;
			uniform float _FinalPower;

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
				//o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.uv = v.texcoord;

				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texcol = tex2D (_MainTex, i.uv);
			//	fixed4 result = texcol * _Color + _HitColor;
				fixed4 result = texcol * _Color;
				return result * _FinalPower;
			}
			ENDCG
		 }
	} 

	Fallback "Diffuse"
}
