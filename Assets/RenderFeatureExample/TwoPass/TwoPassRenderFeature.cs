using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TwoPassRenderFeature : ScriptableRendererFeature
{
    private static readonly string SHADER_NAME = "Hidden/CustomRenderFeature/TwoPass";

    public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    
    private TwoPassRenderPass _renderPass;
    private Material _material = null;
    
    
    public override void Create()
    {
        if (!_material)
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = CoreUtils.CreateEngineMaterial(shader);
        }
        _renderPass = new TwoPassRenderPass(passEvent, _material);
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
        _renderPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_renderPass);
    }
}
