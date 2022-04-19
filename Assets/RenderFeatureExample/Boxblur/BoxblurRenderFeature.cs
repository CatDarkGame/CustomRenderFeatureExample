using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BoxblurRenderFeature : ScriptableRendererFeature
{
    private static readonly string SHADER_NAME = "Hidden/CustomRenderFeature/Boxblur";

    public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public bool isTwoPass = false;  // TwoPass 모드 옵션
    [Range(1, 9)]
    public int blurSamples = 1;
    
    private BoxblurRenderPass _renderPass;  // 일반 BoxBlur
    private BoxblurTwoPassRenderPass _renderPass_TwoPass; // 2Pass BoxBlur
    private Material _material = null;
    
    
    public override void Create()
    {
        if (!_material)
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = CoreUtils.CreateEngineMaterial(shader);
        }
        _renderPass = new BoxblurRenderPass(passEvent, _material);
        _renderPass_TwoPass = new BoxblurTwoPassRenderPass(passEvent, _material);
    }
    
    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (_material)
        {
            CoreUtils.Destroy(_material);
            _material = null;
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!isTwoPass)
        {
            _renderPass.blurSamples = blurSamples;
            _renderPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(_renderPass);
        }
        else
        {
            _renderPass_TwoPass.blurSamples = blurSamples;
            _renderPass_TwoPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(_renderPass_TwoPass);
        }
    }
}
