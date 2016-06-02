Shader "Custom/PaperShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	_Texture2("Texture 2", 2D) = ""
		_BumpMap("Bumpmap", 2D) = "bump" {}
	_Detail("Detail", 2D) = "gray" {}

	_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0

		_Blend("Blend", Range(0, 1)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off


		CGPROGRAM
#pragma surface surf Lambert

	struct Input {
		float2 uv_MainTex;
		float2 uv_Texture2;
		float2 uv_BumpMap;
		float3 viewDir;
		float2 uv_Detail;

	};
	sampler2D _MainTex;
	sampler2D _Texture2;
	sampler2D _BumpMap;
	sampler2D _Detail;
	float _Blend;

	float4 _RimColor;
	float _RimPower;
	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 t1 = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 t2 = tex2D(_Texture2, IN.uv_Texture2);

		o.Albedo = lerp(t1, t2, _Blend)*tex2D(_Detail, IN.uv_Detail).rgb;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow(rim, _RimPower)*tex2D(_Detail, IN.uv_Detail).rgb;


	}
	ENDCG
	}
		//Fallback "Diffuse"
}
