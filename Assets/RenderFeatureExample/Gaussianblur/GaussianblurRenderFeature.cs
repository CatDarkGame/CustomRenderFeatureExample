using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianblurRenderFeature : ScriptableRendererFeature
{
    private static readonly string SHADER_NAME = "Hidden/CustomRenderFeature/Gaussianblur";

    public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    [Range(1, 8)]
    public int blurStep = 8;
    
    private GaussianblurRenderPass _renderPass;
    private Material _material = null;
    
    
    public override void Create()
    {
        if (!_material)
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = CoreUtils.CreateEngineMaterial(shader);
        }
        _renderPass = new GaussianblurRenderPass(passEvent, _material);
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
        _renderPass.blurStep = blurStep;
        _renderPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_renderPass);
    }
}
