// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CustomRenderFeature/Gaussianblur"
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

        float4 BlurHorizontalPassFragment (Varyings i) : SV_TARGET 
        {
            float4 col = 0.0f; 
        	float offsets[] = {
        		-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0
        	};
        	float weights[] = {
        		0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
        		0.19459459, 0.12162162, 0.05405405, 0.01621622
        	};
        	for (int j = 0; j < 9; j++) {
        		float offset = offsets[j] * 2.0 * _MainTex_TexelSize.x;
        		col += tex2D(_MainTex, i.uv + float2(offset, 0.0f)) * weights[j];
        	}
        	
        	return float4(col.rgb, 1);
        }

        float4 BlurVerticalPassFragment (Varyings i) : SV_TARGET 
        {
        	float4 col = 0.0f; 
        	col=0.0f;
        	float offsets[] = {
        		-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0
        	}; 
        	float weights[] = {
        		0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
        		0.19459459, 0.12162162, 0.05405405, 0.01621622
        	};
        	for (int j = 0; j < 9; j++) 
        	{
        		float offset = offsets[j] * _MainTex_TexelSize.y;
        		col += tex2D(_MainTex, i.uv + float2(0.0f, offset)) * weights[j];
        	}
        	return float4(col.rgb, 1.0);
        }
        
     

    ENDHLSL
    
    SubShader
    {
	    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
		ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "Gaussian Blur Horizontal"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment BlurHorizontalPassFragment

            ENDHLSL
        }

        Pass
        {
            Name "Gaussian Blur Vertical"
            HLSLPROGRAM

                #pragma vertex FullscreenVert
                #pragma fragment BlurVerticalPassFragment

            ENDHLSL
        }
        
       
    }
}