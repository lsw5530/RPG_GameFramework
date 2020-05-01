// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mogo/FakeLight" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_RimWidth ("Width",float) = 1
		_RimPower("Power",float) = 3
		_FinalPower("FinalPower",float) = 1.3
    }

    SubShader {

        Pass {
		
		Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
                #pragma vertex vert

                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {

                    float4 vertex : POSITION;

                    float3 normal : NORMAL;

                    float2 texcoord : TEXCOORD0;

                };

                struct v2f {

                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 color : COLOR;
					float3 factor : COLOR1;
                };

                uniform float4 _MainTex_ST;
                uniform float4 _RimColor;
				uniform float _RimWidth;
				uniform float _RimPower;
				uniform float _FinalPower;

                v2f vert (appdata_base v) {

                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);

                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    float dotProduct = 1.0 - saturate(dot(v.normal,viewDir ));

					o.color  = smoothstep(1.0 - _RimWidth, 1.0, pow(dotProduct,_RimPower));
                    o.factor = o.color;
					//o.color = pow(dotProduct,2) ;

                    o.color *= _RimColor;

                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;

                }

                uniform sampler2D _MainTex;
                uniform float4 _Color;

                float4 frag(v2f i) : COLOR {

                    float4 texcol = tex2D(_MainTex, i.uv);

                    texcol *= _Color;

                    //texcol.rgb += i.color;
					//texcol.rgb = texcol.rgb * ( 1 - i.factor.r) + i.color;
//					texcol.rgb = 1 - (1 - texcol.rgb * ( 1 - i.factor)) * ( 1 - i.color);

				texcol.rgb = lerp(texcol.rgb,_RimColor,i.factor);


                    return texcol * _FinalPower;

                }
            ENDCG

        }

    }

}