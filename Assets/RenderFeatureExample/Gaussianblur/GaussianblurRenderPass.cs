using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianblurRenderPass : ScriptableRenderPass
{
    private static readonly string PASS_TAG = "GaussianblurRenderPass";
    
    private static readonly int PROPERTY_TEMPBUFFER_1 = Shader.PropertyToID("_TempBuffer_1"); // 임시렌더텍스처 변수명
    private static readonly int PROPERTY_TEMPBUFFER_2 = Shader.PropertyToID("_TempBuffer_2");

    public int blurStep = 8; // 가우시안 블러 샘플링 횟수
    
    private Material _material;
    private RenderTargetIdentifier _destination;  // 화면렌더텍스처(카메라)
    private RenderTargetIdentifier _tempBuffer_1 = new RenderTargetIdentifier(PROPERTY_TEMPBUFFER_1); // 임시렌더텍스처
    private RenderTargetIdentifier _tempBuffer_2 = new RenderTargetIdentifier(PROPERTY_TEMPBUFFER_2); // 임시렌더텍스처
    
    
    public GaussianblurRenderPass(RenderPassEvent renderPassEvent, Material material)
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
        
        // 2 Pass 가우스안블러 렌더링
        cmd.Blit(_destination, _tempBuffer_1, _material, 0);    // Horizontal
        cmd.Blit(_tempBuffer_1, _tempBuffer_2, _material, 1);   // Vertical
        for (int i = 1; i < blurStep; i++)
        {
            cmd.Blit(_tempBuffer_2, _tempBuffer_1, _material, 0);
            cmd.Blit(_tempBuffer_1, _tempBuffer_2, _material, 1);
        }

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
