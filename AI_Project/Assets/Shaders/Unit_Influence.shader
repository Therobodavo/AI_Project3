﻿Shader "Unlit/Unit_Influence"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "blue" {}
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Transparency("Transparency", Range(0.0, 0.5)) = 0.25
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Stencil {
				Ref 0
				Comp Equal
				Pass IncrSat
				Fail IncrSat
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _TintColor;
			float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
				col.a = _Transparency;
                return col;
            }
            ENDCG

			
			}

			Pass
			{
				Stencil{
					Ref 1
					Comp Less
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
				float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				float4 _TintColor;
				float _Transparency;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = _TintColor + _TintColor + _TintColor;
					col.a = _Transparency ;
					return col;
				}
				ENDCG
        }
    }
}
