Shader "WaterCaustics/Caustics" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
		_DistTex ("Distort Texture", 2d) = ""{}
		_tileNum ("Tile Num", float) = 10
		//_Wave_Spd ("Wave Spd", float) =0.45

	}
	
	Subshader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend One One
			//Blend DstColor One
			//Blend SrcAlpha One
			//Blend OneMinusDstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#pragma target 3.0
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float2 uv_DistTex : TEXCOORD2;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 _Projector;
			float4x4 _ProjectorClip;
			float4 _DistTex_ST;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul (_Projector, vertex);
				o.uvFalloff = mul (_ProjectorClip, vertex);
				o.uv_DistTex = TRANSFORM_TEX( o.uvFalloff.xyz, _DistTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Color;
			fixed _tileNum;
			sampler2D _ShadowTex;
			sampler2D _DistTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed _WaveSpd = 0.45;
				fixed dSpd = 1.0;
			
				fixed4 uvF = fixed4(UNITY_PROJ_COORD(i.uvShadow));
				fixed4 uvF2 = uvF;
				fixed d = tex2D(_DistTex, uvF*20 + fixed2(_Time.x*-5.5*dSpd, 0)).x*0.2 +
						  tex2D(_DistTex, uvF*5  + fixed2(_Time.x* -1*dSpd, 0)).y + 
						  tex2D(_DistTex, uvF*5  + fixed2(_Time.x* 1*dSpd, 0)).z*2;
				d = d*0.6;
				uvF.xy += d*0.01;
				d = saturate(d);
				d = d*d;
				uvF.xy *= fixed2(_tileNum, _tileNum);
				uvF.x += _Time.x * _WaveSpd;
				
				
				fixed l = tex2D(_DistTex, uvF2*5  + fixed2(_Time.x*-2*dSpd, 0)).w;
				l = l*l;
				
				fixed d2 = tex2D(_DistTex, uvF2*20 + fixed2(_Time.x*5.2*dSpd + 0.5, 0)).x*0.2 +
						   tex2D(_DistTex, uvF2*5 + fixed2(_Time.x* 0.5*dSpd + 0.5, 0)).y + 
						   tex2D(_DistTex, uvF2*5  + fixed2(_Time.x*-0.5*dSpd + 0.5, 0)).z*2;
				d2 = d2*0.6;
				uvF2.xy += d2*0.01;
				d2 = saturate(d2);
				d2 = d2*d2;
				uvF2.xy *= fixed2(_tileNum, _tileNum);
				uvF2.y += _Time.x * _WaveSpd;
				
				uvF2 = uvF2 + fixed4(0.4, 0, 0, 0);
				
				
				fixed4 texS = tex2Dproj (_ShadowTex, uvF)  * min(d*1.1, 1);
				fixed4 texS2 = tex2Dproj (_ShadowTex, uvF2)* min(d2*1.1, 1);
				
				fixed4 res = lerp(texS, texS2, l);
				
				return res * _Color;
			}
			ENDCG
		}
	}
}
