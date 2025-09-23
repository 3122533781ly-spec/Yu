Shader "Effect/Effect_Amplify_Shader_01"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode("ZTest Mode", Float) = 2
		[Enum(OFF,0,ON,1)]_ZWriteMode("ZWrite Mode", Float) = 0
		_Depth("Depth", Float) = 0
		_power("power", Float) = 1
		[HDR]_TintColor("TintColor", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
		[Toggle(_OPACITY_MASK_ON)] _Opacity_Mask("Opacity_Mask", Float) = 0
		[Toggle(_TUREKEYFALSE_ON)] _TureKeyFalse("TureKeyFalse", Float) = 0
		_FrontColor("FrontColor", Color) = (1,1,1,1)
		_TureColor("TureColor", Color) = (1,1,1,1)
		[Toggle(_KEY_MAINTEX_A_ON)] _Key_MainTex_A("Key_MainTex_A", Float) = 0
		[Toggle(_KEY_MAIN_CUSTOM1_ON)] _Key_Main_Custom1("Key_Main_Custom", Float) = 0
		_MainTex("MainTex", 2D) = "white" {}
		_Msak("Msak", 2D) = "white" {}
		_M_offset_ver("M_offset_ver", Vector) = (0,0,0,0)
		[Toggle(_KEY_MASK01_ON)] _Key_Mask01("Key_Mask01", Float) = 0
		_Msak01("Msak01", 2D) = "white" {}
		[Toggle(_KEY_NOMRAL_ON)] _Key_Nomral("Key_Nomral", Float) = 0
		_Normal_qingdu("Normal_qingdu", Float) = 0.1
		_Normal("Normal", 2D) = "white" {}
		_ND_offset_ver("ND_offset_ver", Vector) = (0,0,0,0)
		[Toggle(_KEY_DISSOLVE_ON)] _Key_Dissolve("Key_Dissolve", Float) = 0
		[Toggle(_KEY_DISSOLVE_R_ON)] _Key_Dissolve_R("Key_Dissolve_R", Float) = 0
		[HDR]_DE_Color("DE_Color", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
		_DistorEdge("DistorEdge", Range( 0 , 0.3)) = 0
		_Distor_intensity("Distor_intensity", Range( 0 , 1)) = 0
		_Dissolve("Dissolve", 2D) = "white" {}
		_Vertex_Offset("Vertex_Offset", 2D) = "white" {}
		_Vertex_Offset_Vector("Vertex_Offset_Vector", Vector) = (0,0,0,0)
		_Distortion("Distortion", 2D) = "white" {}
		_Distortion_XY_Z("Distortion_XY_Z", Vector) = (0,0,0,0)
		[Toggle(_KEY_FRESNEL_ON)] _Key_Fresnel("Key_Fresnel", Float) = 0
		[HDR]_FresnelColor("FresnelColor", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
		_Fresnel("Fresnel", Vector) = (1,5,1,0)
		_Mask_Clip_Value("Mask_Clip_Value", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite [_ZWriteMode]
		ZTest [_ZTestMode]
		Blend [_Src] [_Dst]
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#pragma multi_compile_instancing
		#pragma shader_feature_local _TUREKEYFALSE_ON
		#pragma shader_feature_local _KEY_FRESNEL_ON
		#pragma shader_feature_local _KEY_MASK01_ON
		#pragma shader_feature_local _KEY_MAIN_CUSTOM1_ON
		#pragma shader_feature_local _KEY_NOMRAL_ON
		#pragma shader_feature_local _KEY_DISSOLVE_ON
		#pragma shader_feature_local _KEY_DISSOLVE_R_ON
		#pragma shader_feature_local _KEY_MAINTEX_A_ON
		#pragma shader_feature_local _OPACITY_MASK_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
			float4 screenPos;
			float3 viewDir;
		};

		uniform sampler2D _Vertex_Offset;
		SamplerState sampler_Vertex_Offset;
		uniform float4 _Vertex_Offset_Vector;
		uniform float4 _Vertex_Offset_ST;
		uniform float4 _FrontColor;
		uniform float _CullMode;
		uniform float _Src;
		uniform float _Dst;
		uniform float _ZTestMode;
		uniform float _ZWriteMode;
		uniform float _Mask_Clip_Value;
		uniform float4 _TintColor;
		uniform float4 _Fresnel;
		uniform float4 _FresnelColor;
		uniform sampler2D _Msak01;
		SamplerState sampler_Msak01;
		uniform float4 _Msak01_ST;
		uniform sampler2D _Msak;
		SamplerState sampler_Msak;
		uniform float4 _Msak_ST;
		uniform float4 _M_offset_ver;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Normal;
		uniform float4 _ND_offset_ver;
		uniform float4 _Normal_ST;
		uniform float _Normal_qingdu;
		uniform sampler2D _Dissolve;
		uniform float4 _Dissolve_ST;
		uniform float _Distor_intensity;
		uniform float _DistorEdge;
		uniform float4 _DE_Color;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _Distortion;
		uniform float4 _Distortion_XY_Z;
		uniform float4 _Distortion_ST;
		uniform float4 _TureColor;
		SamplerState sampler_MainTex;
		uniform float _power;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float Ver_Time36 = _Time.y;
			float2 appendResult170 = (float2(_Vertex_Offset_Vector.x , _Vertex_Offset_Vector.y));
			float2 uv_Vertex_Offset = v.texcoord.xy * _Vertex_Offset_ST.xy + _Vertex_Offset_ST.zw;
			float3 ase_vertexNormal = v.normal.xyz;
			float clampResult176 = clamp( _Vertex_Offset_Vector.z , 0.0 , 5.0 );
			float3 VertexOffset147 = ( ( tex2Dlod( _Vertex_Offset, float4( ( ( Ver_Time36 * appendResult170 ) + uv_Vertex_Offset ), 0, 1.0) ).r - 0.5 ) * ( ase_vertexNormal * clampResult176 ) );
			v.vertex.xyz += VertexOffset147;
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float kongzhi241 = ( ( _CullMode + _Src + _Dst + _ZTestMode + _ZWriteMode + _Mask_Clip_Value ) * 0.0 );
			float3 appendResult293 = (float3(_TintColor.r , _TintColor.g , _TintColor.b));
			float Ver_Color_A116 = i.vertexColor.a;
			float temp_output_283_0 = ( _TintColor.a * 2.0 * Ver_Color_A116 );
			float4 temp_cast_1 = (1.0).xxxx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float clampResult208 = clamp( abs( _Fresnel.y ) , 0.0 , 60.0 );
			float fresnelNdotV180 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode180 = ( 0.0 + abs( _Fresnel.x ) * pow( 1.0 - fresnelNdotV180, clampResult208 ) );
			float clampResult184 = clamp( fresnelNode180 , 0.0 , 1.0 );
			float clampResult203 = clamp( abs( _Fresnel.z ) , 0.0 , 1.0 );
			float lerpResult206 = lerp( ( 1.0 - clampResult184 ) , clampResult184 , trunc( clampResult203 ));
			#ifdef _KEY_FRESNEL_ON
				float4 staticSwitch214 = ( lerpResult206 * ( _FresnelColor * 2.0 ) );
			#else
				float4 staticSwitch214 = temp_cast_1;
			#endif
			float2 uv_Msak01 = i.uv_texcoord * _Msak01_ST.xy + _Msak01_ST.zw;
			float4 tex2DNode93 = tex2D( _Msak01, uv_Msak01 );
			#ifdef _KEY_MASK01_ON
				float staticSwitch125 = ( tex2DNode93.r * tex2DNode93.a );
			#else
				float staticSwitch125 = 1.0;
			#endif
			float4 Ver_Color_RGBA114 = i.vertexColor;
			float2 uv_Msak = i.uv_texcoord * _Msak_ST.xy + _Msak_ST.zw;
			float Ver_Time36 = _Time.y;
			float2 appendResult50 = (float2(_M_offset_ver.z , _M_offset_ver.w));
			float2 Ver_Z67 = ( Ver_Time36 * appendResult50 );
			float4 tex2DNode83 = tex2D( _Msak, ( uv_Msak + Ver_Z67 ) );
			float3 appendResult297 = (float3(tex2DNode83.r , tex2DNode83.g , tex2DNode83.b));
			float3 temp_output_299_0 = ( appendResult297 * tex2DNode83.a );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 appendResult68 = (float2(i.uv2_tex4coord2.x , i.uv2_tex4coord2.y));
			#ifdef _KEY_MAIN_CUSTOM1_ON
				float2 staticSwitch352 = ( uv_MainTex + appendResult68 );
			#else
				float2 staticSwitch352 = uv_MainTex;
			#endif
			float2 appendResult57 = (float2(_M_offset_ver.x , _M_offset_ver.y));
			float2 appendResult39 = (float2(_ND_offset_ver.x , _ND_offset_ver.y));
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			#ifdef _KEY_NOMRAL_ON
				float staticSwitch69 = ( (tex2D( _Normal, ( ( Ver_Time36 * appendResult39 ) + uv_Normal ) )).r * _Normal_qingdu );
			#else
				float staticSwitch69 = 0.0;
			#endif
			float4 tex2DNode80 = tex2D( _MainTex, ( staticSwitch352 + ( _Time.y * appendResult57 ) + staticSwitch69 ) );
			float4 temp_cast_3 = (1.0).xxxx;
			float2 appendResult40 = (float2(_ND_offset_ver.z , _ND_offset_ver.w));
			float2 uv_Dissolve = i.uv_texcoord * _Dissolve_ST.xy + _Dissolve_ST.zw;
			float temp_output_64_0 = (tex2D( _Dissolve, ( ( Ver_Time36 * appendResult40 ) + uv_Dissolve ) )).r;
			float temp_output_341_0 = ( i.uv2_tex4coord2.z + _Distor_intensity );
			float2 _Vector0 = float2(1,0);
			float ifLocalVar66 = 0;
			if( temp_output_64_0 <= temp_output_341_0 )
				ifLocalVar66 = _Vector0.y;
			else
				ifLocalVar66 = _Vector0.x;
			float ifLocalVar65 = 0;
			if( temp_output_64_0 <= ( _DistorEdge + temp_output_341_0 ) )
				ifLocalVar65 = _Vector0.y;
			else
				ifLocalVar65 = _Vector0.x;
			float clampResult81 = clamp( ( temp_output_64_0 - temp_output_341_0 ) , 0.0 , 1.0 );
			float4 temp_cast_4 = (clampResult81).xxxx;
			#ifdef _KEY_DISSOLVE_R_ON
				float4 staticSwitch86 = temp_cast_4;
			#else
				float4 staticSwitch86 = ( ifLocalVar66 + ( ( ifLocalVar66 - ifLocalVar65 ) * _DE_Color ) );
			#endif
			#ifdef _KEY_DISSOLVE_ON
				float4 staticSwitch90 = staticSwitch86;
			#else
				float4 staticSwitch90 = temp_cast_3;
			#endif
			float4 temp_output_202_0 = ( staticSwitch214 * ( staticSwitch125 * ( Ver_Color_RGBA114 * ( ( float4( temp_output_299_0 , 0.0 ) * tex2DNode80 ) * staticSwitch90 ) ) ) );
			float4 temp_output_246_0 = ( kongzhi241 + ( float4( ( appendResult293 * 2.0 * temp_output_283_0 ) , 0.0 ) * temp_output_202_0 ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult108 = (float2(_Distortion_XY_Z.x , _Distortion_XY_Z.y));
			float2 uv_Distortion = i.uv_texcoord * _Distortion_ST.xy + _Distortion_ST.zw;
			float clampResult239 = clamp( _Distortion_XY_Z.z , 0.0 , 1.0 );
			float4 screenColor105 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + (( tex2D( _Distortion, ( ( Ver_Time36 * appendResult108 ) + uv_Distortion ) ) * clampResult239 * Ver_Color_A116 )).rga ).xy);
			float4 Brushed120 = screenColor105;
			float clampResult136 = clamp( _Distortion_XY_Z.w , 0.0 , 1.0 );
			float BMlN_XY_Z_A133 = clampResult136;
			float4 lerpResult128 = lerp( ( _FrontColor * temp_output_246_0 ) , Brushed120 , BMlN_XY_Z_A133);
			float4 Color248 = lerpResult128;
			float4 Color_F249 = ( temp_output_246_0 * _TureColor );
			float dotResult258 = dot( ase_worldNormal , i.viewDir );
			float4 lerpResult260 = lerp( Color248 , Color_F249 , (1.0 + (sign( dotResult258 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
			#ifdef _TUREKEYFALSE_ON
				float4 staticSwitch252 = lerpResult260;
			#else
				float4 staticSwitch252 = Color248;
			#endif
			o.Emission = staticSwitch252.rgb;
			float4 temp_cast_10 = (0.0).xxxx;
			float4 clampResult304 = clamp( temp_output_202_0 , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 MainTex_A308 = ( tex2DNode83.a * tex2DNode80.a * staticSwitch90 );
			float Mask01_A316 = staticSwitch125;
			#ifdef _KEY_MAINTEX_A_ON
				float staticSwitch15 = 0.0;
			#else
				float staticSwitch15 = 1.0;
			#endif
			float4 lerpResult262 = lerp( ( clampResult304 * temp_output_283_0 ) , ( temp_output_283_0 * MainTex_A308 * Mask01_A316 ) , staticSwitch15);
			float4 clampResult351 = clamp( lerpResult262 , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 temp_cast_12 = (_power).xxxx;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth21 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth21 = abs( ( screenDepth21 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float clampResult24 = clamp( distanceDepth21 , 0.0 , 1.0 );
			float4 lerpResult31 = lerp( temp_cast_10 , pow( clampResult351 , temp_cast_12 ) , clampResult24);
			float4 Alpha153 = lerpResult31;
			float4 temp_cast_13 = (1.0).xxxx;
			#ifdef _OPACITY_MASK_ON
				float staticSwitch331 = 1.0;
			#else
				float staticSwitch331 = 0.0;
			#endif
			float4 lerpResult327 = lerp( Alpha153 , temp_cast_13 , staticSwitch331);
			o.Alpha = lerpResult327.r;
			float4 temp_cast_15 = (1.0).xxxx;
			float4 lerpResult333 = lerp( temp_cast_15 , Alpha153 , staticSwitch331);
			clip( lerpResult333.r - _Mask_Clip_Value );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
