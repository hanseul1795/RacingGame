Shader "Unlit/PortalView"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_PortalTex ("Portal TargetTexture", 2D) = "white" {}
		_TurbulenceMask("Turbulence Mask", 2D) = "white" {}
		_NoiseScale("Noize Scale (XYZ) Height (W)", Vector) = (1, 1, 1, 0.2)
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
				//Cull Off
				Lighting Off
				ZWrite Off

		LOD 100
 
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4    _MainTex_ST;

			sampler2D _PortalTex;
			float4    _PortalTex_ST;

			float4 _NoiseScale;
			sampler2D _TurbulenceMask;

			struct appdata
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD2;
				float4 pos : SV_POSITION;
			};	

			v2f vert(appdata v)
			{
				v2f o;

				float3 wpos = mul(unity_ObjectToWorld, v.pos).xyz;
				float4 coordNoise = float4(wpos * _NoiseScale.xyz, 0);
				float4 tex1 = tex2Dlod(_TurbulenceMask, coordNoise + float4(_Time.x * 3, _Time.x * 5, _Time.x * 2.5, 0));
				v.pos.xyz += v.normal* 0.005 + tex1.rgb * _NoiseScale.w - _NoiseScale.w / 2;

				o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = UnityObjectToClipPos (v.pos);
				o.screenPos = ComputeScreenPos (o.pos);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * tex2Dproj(_PortalTex, UNITY_PROJ_COORD(i.screenPos));
			}
			ENDCG
	    }
	}
}
