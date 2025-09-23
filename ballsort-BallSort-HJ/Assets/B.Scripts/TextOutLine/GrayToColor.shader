Shader "Custom/GrayToColor"
{
	Properties
	{
		// ��ͼ
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	// ��ֵ
	_Lerp("Color Lerp",Range(0,1.0)) = 0
	}
		SubShader
	{
		// ��ǩ
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}

		ZWrite On
		 Blend SrcAlpha OneMinusSrcAlpha
		 Cull Off

		 LOD 100

		 pass {

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata {


		float4 vertex: POSITION;
		float2 uv:TEXCOORD0;
		fixed4 color : COLOR;

		};

		struct v2f {


		float2 uv:TEXCOORD0;
		float4 vertex:SV_POSITION;
		fixed4 color : COLOR;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _Lerp;

		v2f vert(appdata v) {


		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv,_MainTex);
		o.color = v.color;

		return o;
		}


		fixed4 frag(v2f i) :SV_Target{

			// ��ͼ��ɫ
			fixed4 renderTex = tex2D(_MainTex,i.uv);

		// �Ҷ� ����Ĳο��Ҷ� ���Աȶ�Ч��
		//fixed gray = 0.2125*renderTex.r + 0.7154*renderTex.g + 0.0721*renderTex.b;
		fixed gray = 0.30 * renderTex.r + 0.59 * renderTex.g + 0.11 * renderTex.b;

		// �Ҷ���ɫ
		fixed3 grayLerp = fixed3(gray, gray, gray);
		// ��ֵ�Ҷȱ��ɫ
		fixed3 finalColor = lerp(renderTex.rgb, grayLerp, _Lerp);

		// ������ɫ
		return fixed4(finalColor, renderTex.a);
		}

		ENDCG
		}
	}
		FallBack "Diffuse"
}