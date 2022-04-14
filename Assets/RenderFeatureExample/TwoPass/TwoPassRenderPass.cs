using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TwoPassRenderPass : ScriptableRenderPass
{
    private static readonly string PASS_TAG = "TwoPassRenderPass";
    
    private static readonly int PROPERTY_BUFFER1_TEX = Shader.PropertyToID("_Buffer1Tex"); // 버퍼1 sampler2D 변수명
    private static readonly int PROPERTY_TEMPBUFFER_1 = Shader.PropertyToID("_TempBuffer_1"); // 임시렌더텍스처 변수명
    private static readonly int PROPERTY_TEMPBUFFER_2 = Shader.PropertyToID("_TempBuffer_2"); 
    
    private Material _material;
    private RenderTargetIdentifier _destination;  // 화면렌더텍스처(카메라)
    private RenderTargetIdentifier _tempBuffer_1 = new RenderTargetIdentifier(PROPERTY_TEMPBUFFER_1); // 임시렌더텍스처
    private RenderTargetIdentifier _tempBuffer_2 = new RenderTargetIdentifier(PROPERTY_TEMPBUFFER_2); // 임시렌더텍스처
    
    
    public TwoPassRenderPass(RenderPassEvent renderPassEvent, Material material)
    {
        this.renderPassEvent = renderPassEvent;
        _material = material;
    }
    
    public void Setup(RenderTargetIdentifier renderTargetDestination)
    {
        _destination = renderTargetDestination;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(PASS_TAG);
       
        // 임시렌더텍스처 생성
        CameraData cameraData = renderingData.cameraData;
        RenderTextureDescriptor descriptor = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
        cmd.GetTemporaryRT(PROPERTY_TEMPBUFFER_1, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(PROPERTY_TEMPBUFFER_2, descriptor, FilterMode.Bilinear);
        
        // 현재 화면 데이터를 Material 효과를 적용하며 임시렌더텍스처에 복사 (Pass 0)
        cmd.Blit(_destination, _tempBuffer_1, _material, 0);
        // 버퍼1을 별도 sampler2D Property로 접근 가능하세 세팅
        cmd.SetGlobalTexture(PROPERTY_BUFFER1_TEX, _tempBuffer_1);
        
        // 현재 화면 데이터를 Material 효과를 적용하며 임시렌더텍스처에 복사 (Pass 1)
        cmd.Blit(_destination, _tempBuffer_2, _material, 1);
        
        // 임시렌더텍스처를 화면렌더텍스처에 복사
        cmd.Blit(_tempBuffer_2, _destination);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(PROPERTY_TEMPBUFFER_1);
        cmd.ReleaseTemporaryRT(PROPERTY_TEMPBUFFER_2);
    }


}
