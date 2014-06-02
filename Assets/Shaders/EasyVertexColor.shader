Shader "Mobile/EasyVertexColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd



		struct Input {
			float3 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color.rgb;
		}
		ENDCG
	} 
	FallBack "Mobile/VertexLit"
}