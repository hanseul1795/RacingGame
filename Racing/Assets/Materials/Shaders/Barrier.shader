Shader "William/Barrier"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color ("Color", Color) = (0,0,0,0)
		_Multiplier("Multiplier", Range (0.5, 10)) = 0.5
		_SliceAmount("Slice Amount", Range(-1, 1.5)) = -1
		_NorthShine("NorthShine", Range(0.0,20)) = 0
	}

	SubShader
	{
		Blend One One
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1)/2;
				o.screenuv.y = 1 - o.screenuv.y;
				o.depth = -(UnityObjectToViewPos(v.vertex)).z *_ProjectionParams.w;

				o.objectPos = v.vertex.xyz;		
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}
			
			sampler2D _CameraDepthNormalsTexture;
			fixed4 _Color;

			float _SliceAmount;

			float _Multiplier;
			float _NorthShine;

			float triWave(float t, float offset, float yOffset)
			{
				return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			}

			fixed4 texColor(v2f i, float rim)
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				mainTex.r *= triWave(_Time.x * 5, abs(i.objectPos.y) * 2, -0.7) * 6;
				mainTex.g *= saturate(rim) * (sin(_Time.z + mainTex.b * 10) + 1);
				return mainTex.r * _Color + mainTex.g * _Color;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;
				
				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * _Multiplier, diff);

				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 1.1;
				float northPole = (i.objectPos.y - 0.5) * _NorthShine;
				float glow = max(max(intersect, rim), northPole);

				fixed4 glowColor = fixed4(lerp(_Color.rgb, fixed3(1, 1, 1), pow(glow, 4)), 1);
				
				fixed4 hexes = texColor(i, rim);

				fixed4 col = _Color * _Color.a + glowColor * glow + hexes;

				 clip(tex2D (_MainTex, i.uv).b - (_SliceAmount - i.objectPos.y) );

				return col;
			}
			ENDCG
		}
	}
}
