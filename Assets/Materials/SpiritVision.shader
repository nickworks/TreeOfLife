Shader "Hidden/SpiritVision"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			fixed _value;

			fixed4 frag (v2f_img i) : SV_Target{
				fixed4 original = tex2D(_MainTex, i.uv);

				half4 input = original.rgba;
				fixed4 output = float4(0.191, -0.054, -0.221, 0.0) + input;
				output.a = original.a;
				output = lerp(output, original, _value);

				return output;
			} 

			ENDCG
		}
	}
	Fallback Off
}
