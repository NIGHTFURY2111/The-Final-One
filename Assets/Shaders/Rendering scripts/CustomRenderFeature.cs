


using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderFeature : ScriptableRendererFeature
{

    [SerializeField] private Shader Bloom;
    [SerializeField] private Shader Composite;
    [SerializeField] private Shader Normal;
    [SerializeField] private Shader Occlude;
    [SerializeField] private Shader Outline;
    [SerializeField] private LayerMask NormalMask;
    [SerializeField] private LayerMask OccludeMask;
    Material BloomMat;
    Material CompositeMat;
    Material NormalMat;
    Material OccludeMat;
    Material OutlineMat;
    CustomPass pass;
    NormalPass normalPass;
    OutlinePass outlinePass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) { return; }
        renderer.EnqueuePass(pass);
        renderer.EnqueuePass(normalPass);
        renderer.EnqueuePass(outlinePass);
    }

    public override void Create()
    {
       
        BloomMat = CoreUtils.CreateEngineMaterial(Bloom);
        CompositeMat = CoreUtils.CreateEngineMaterial(Composite);
        NormalMat = CoreUtils.CreateEngineMaterial(Normal);
        OccludeMat = CoreUtils.CreateEngineMaterial(Occlude);
        OutlineMat = CoreUtils.CreateEngineMaterial(Outline);
        pass = new CustomPass(BloomMat,CompositeMat);
        normalPass = new NormalPass(NormalMat,OccludeMat,NormalMask,OccludeMask);
        outlinePass = new OutlinePass(OutlineMat);
 
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {

        if (renderingData.cameraData.cameraType != CameraType.Game) { return; }
       
        pass.ConfigureInput(ScriptableRenderPassInput.Color);
        outlinePass.ConfigureInput(ScriptableRenderPassInput.Color);
        pass.setTarget(renderer.cameraColorTargetHandle);
        outlinePass.setTarget(renderer.cameraColorTargetHandle);
       


    }
    protected override void Dispose(bool Disposing)
    {
        CoreUtils.Destroy(BloomMat);
        CoreUtils.Destroy(CompositeMat);
        CoreUtils.Destroy(NormalMat);
        CoreUtils.Destroy(OccludeMat);
        CoreUtils.Destroy(OutlineMat);
        
    }
    
}
