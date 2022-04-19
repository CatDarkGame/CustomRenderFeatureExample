// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CustomRenderFeature/Boxblur"
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
        float2 _MainTex_TexelSize;
        
        float _blurSamples; // 블러 샘플링 갯수

        // 8방향 전부 계산하는 일반 BoxBlur
        float4 BoxblurPassFragment (Varyings i) : SV_TARGET 
        {
            float4 col = 0.0f; 
            int samples = (2 * _blurSamples) + 1;
            
            for(float x=0; x<samples; x++)
            {
                for(float y=0; y<samples; y++)
                {
                    float2 offset = float2(x - _blurSamples, y - _blurSamples);
                    col += tex2D(_MainTex, i.uv + (offset * _MainTex_TexelSize));
                }
            }
            
            return float4(col.rgb / (samples * samples), 1);
        }
        
        
        // 가로세로만 계산하는 BoxBlur - 가로
        float4 Boxblur_HorizontalPassFragment (Varyings i) : SV_TARGET 
        {
            float4 col = 0.0f; 
            int samples = (2 * _blurSamples) + 1;
            
            for(float x=0; x<samples; x++)
            {
                float2 offset = float2(x - _blurSamples, 0.0f);
                col += tex2D(_MainTex, i.uv + (offset * _MainTex_TexelSize));
            }
            
            return float4(col.rgb / samples, 1);
        }
        
        // 가로세로만 계산하는 BoxBlur - 세로
        float4 Boxblur_VerticalPassFragment (Varyings i) : SV_TARGET 
        {
            float4 col = 0.0f; 
            int samples = (2 * _blurSamples) + 1;
            
            for(float y=0; y<samples; y++)
            {
                float2 offset = float2(0.0f, y - _blurSamples);
                col += tex2D(_MainTex, i.uv + (offset * _MainTex_TexelSize));
            }
            
            return float4(col.rgb / samples, 1);
        }

    ENDHLSL
    
    SubShader
    {
	    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
		ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "BoxBlur"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment BoxblurPassFragment

            ENDHLSL
        }
        
        Pass
        {
            Name "BoxBlur_Horizontal"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment Boxblur_HorizontalPassFragment

            ENDHLSL
        }
        
        Pass
        {
            Name "BoxBlur_Vertical"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment Boxblur_VerticalPassFragment

            ENDHLSL
        }
       
    }
}