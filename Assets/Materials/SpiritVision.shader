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
				//The original screen image
				fixed4 original = tex2D(_MainTex, i.uv);
				//This is original that will be added to output to make the screen effect
				half4 input = original.rgba;

				fixed4 output = float4(0.191, -0.054, -0.221, 0.0) + input;
				//Allow for alpha control if needed
				output.a = original.a;
				//This allows for the output to fade in and out based on a value. Could be used if vision effect should wear off
				output = lerp(output, original, _value);

				return output;
			} 

			ENDCG
		}
	}
	Fallback Off
}
