
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePass : ScriptableRenderPass
{
    private RTHandle ColorTarget;
    private Material OutlineMaterial;
    public OutlinePass(Material outlineMaterial)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        OutlineMaterial = outlineMaterial;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Effect")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            RenderTextureDescriptor d = ColorTarget.rt.descriptor;
            RTHandle tempRT = RTHandles.Alloc(d, name: "_TempRT");

            Blitter.BlitCameraTexture(cmd, ColorTarget, tempRT);


            Blitter.BlitCameraTexture(cmd, tempRT, ColorTarget, OutlineMaterial, 0);


            RTHandles.Release(tempRT);

        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public void setTarget(RTHandle Color)
    {
        ColorTarget = Color;
      
    }


}
