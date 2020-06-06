Shader "PostProcess/Transition"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Cutoff("Cutoff", float) = 0
	}
		SubShader
		{
			Pass
			{

			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _TransitionTex;
			uniform float4 _Color;
			uniform float _Cutoff;

			float4 frag(v2f_img i) : COLOR
			{
				return float4(1,1,1,1);
			}

			ENDCG
		}
		}
}