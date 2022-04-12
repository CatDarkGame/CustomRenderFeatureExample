// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CustomRenderFeature/Simple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
	    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
		ZTest Always ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

            #pragma prefer_hlslcc gles   
            #pragma exclude_renderers d3d11_9x  
            
            #pragma vertex FullscreenVert
            #pragma fragment frag

            sampler2D _MainTex;
 
            float4 frag(Varyings i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float3 grayscale = (col.r + col.g + col.b) * 0.3333f;
    
                return float4(grayscale.rgb, 1);
            }
            
            ENDHLSL
        }
    }
}