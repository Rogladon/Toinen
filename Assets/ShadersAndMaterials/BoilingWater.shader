Shader "Toinen/Boiling Water" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// // make fog work
			// #pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				// UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

#define M_PI 3.1417

			v2f vert(appdata v) {
				v2f o;
				o.vertex = v.vertex;
				if (o.vertex.z > 1) {
					float timeOffset = o.vertex.x / 4.0;
					o.vertex.z += cos(((_Time * 10 + 10.0 * timeOffset) % 1) * M_PI * 10.0) * 0.25 - 1;
				}
				o.vertex = UnityObjectToClipPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// // apply fog
				// UNITY_APPLY_FOG(i.fogCoord, col);
				// col.rgb *= col.a;
				return col;
			}
			ENDCG
		}
	}
}
