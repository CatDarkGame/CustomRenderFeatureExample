// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CustomRenderFeature/TwoPass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
    HLSLINCLUDE
    
        #pragma exclude_renderers gles
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
        
        sampler2D _MainTex;
        sampler2D _Buffer1Tex;
    
        float4 TwoPassFragment_1 (Varyings i) : SV_TARGET 
        {
            float4 col = tex2D(_MainTex, i.uv) * float4(0, 0, 1, 1);
            return col;
        }
        
        float4 TwoPassFragment_2 (Varyings i) : SV_TARGET 
        {
            float4 col = tex2D(_MainTex, i.uv); 
            float4 buffer1 = tex2D(_Buffer1Tex, i.uv); 
            
            return col + buffer1;
        }
    
    ENDHLSL
        
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "TwoPass_1"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment TwoPassFragment_1

            ENDHLSL
        }
        
        Pass
        {
            Name "TwoPass_2"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment TwoPassFragment_2

            ENDHLSL
        }
    }
}