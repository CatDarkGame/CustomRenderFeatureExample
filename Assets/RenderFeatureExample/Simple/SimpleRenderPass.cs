using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SimpleRenderPass : ScriptableRenderPass
{
    private static readonly string PASS_TAG = "SimpleRenderPass";
    private static readonly int PROPERTY_TEMPBUFFER = Shader.PropertyToID("_TempBuffer"); // 임시렌더텍스처 변수명
    
    private Material _material;
    private RenderTargetIdentifier _destination;  // 화면렌더텍스처(카메라)
    private RenderTargetIdentifier _tempBuffer = new RenderTargetIdentifier(PROPERTY_TEMPBUFFER); // 임시렌더텍스처
    
    
    public SimpleRenderPass(RenderPassEvent renderPassEvent, Material material)
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
        cmd.GetTemporaryRT(PROPERTY_TEMPBUFFER, descriptor, FilterMode.Bilinear);
       
        // 현재 화면 데이터를 Material 효과를 적용하며 임시렌더텍스처에 복사
        cmd.Blit(_destination, _tempBuffer, _material);
        
        // 임시렌더텍스처를 화면렌더텍스처에 복사
        cmd.Blit(_tempBuffer, _destination);
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(PROPERTY_TEMPBUFFER);
    }
    
 
    
  

}
