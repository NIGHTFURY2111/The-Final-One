


using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader Bloom;
    [SerializeField] private Shader Composite;
    private Material BloomMat;
    private Material CompositeMat;
    CustomPass pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) { return; }
        renderer.EnqueuePass(pass);
    }

    public override void Create()
    {
       
        BloomMat = CoreUtils.CreateEngineMaterial(Bloom);
        CompositeMat = CoreUtils.CreateEngineMaterial(Composite);
        pass = new CustomPass(BloomMat,CompositeMat);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {

        if (renderingData.cameraData.cameraType != CameraType.Game) { return; }
        pass.ConfigureInput(ScriptableRenderPassInput.Depth);
        pass.ConfigureInput(ScriptableRenderPassInput.Color);
        pass.setTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        
       
    }
    protected override void Dispose(bool Disposing)
    {
        CoreUtils.Destroy(BloomMat);
        CoreUtils.Destroy(CompositeMat);
        
    }
    
}
