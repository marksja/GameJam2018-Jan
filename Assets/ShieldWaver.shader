Shader "Unlit/ShieldWaver"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_MainAlpha("Base Alpha", float) = 0.1
		_RippleAlpha("Ripple Alpha", float) = 0.3
		_Ripple1Start("Ripple 1 Start Value", float) = 0.1
		_Ripple1End("Ripple 1 End Value", float) = 0.1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
        	Lighting Off
        	ZWrite Off
        	Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile DUMMY PIXELSNAP_ON
			
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
			sampler2D _NoiseTex;
			float4 _MainTex_ST;
			float _MainAlpha;
			float _RippleAlpha;
			float _Ripple1Start;
			float _Ripple1End;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				#ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap (o.vertex);
                #endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float cutout = tex2D(_NoiseTex, i.uv).r;
				fixed4 col = tex2D(_MainTex, i.uv);

				col.a = _MainAlpha;
				if (cutout >= _Ripple1Start && cutout <= _Ripple1End)
					col.a += _RippleAlpha;


				return col;
			}
			ENDCG
		}
	}
}
