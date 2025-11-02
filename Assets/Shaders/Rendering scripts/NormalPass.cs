
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NormalPass : ScriptableRenderPass
{
    Material NormalMaterial;
    Material OccludeMaterial;
    Material OutlineMaterial;
    LayerMask NormalMask;
    LayerMask OccludeMask;
    RTHandle normalView;
    List<ShaderTagId> tags;
    public NormalPass( Material normalMatrieal,Material occludeMaterial,LayerMask normalMask,LayerMask occludeMask )
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        tags = new List<ShaderTagId>
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightWeightForward")
        };
        NormalMaterial = normalMatrieal;
        OccludeMaterial = occludeMaterial;
  
        NormalMask = normalMask;
        OccludeMask = occludeMask;
    

    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        RenderTextureDescriptor desc = cameraTextureDescriptor;
        desc.depthBufferBits = 0;
        normalView = RTHandles.Alloc(desc, name: "Normal View");
        
        ConfigureTarget(normalView);
        ConfigureClear(ClearFlag.All, Color.clear);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

       
        DrawingSettings ds = CreateDrawingSettings(tags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
        
        using (new ProfilingScope(cmd, new ProfilingSampler("View Space Normals")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            FilteringSettings fs = new FilteringSettings(RenderQueueRange.opaque, NormalMask);
            ds.overrideMaterial = NormalMaterial;
            context.DrawRenderers(renderingData.cullResults, ref ds, ref fs);
            ds.overrideMaterial = OccludeMaterial;
            fs = new FilteringSettings(RenderQueueRange.opaque, OccludeMask);
            context.DrawRenderers(renderingData.cullResults, ref ds, ref fs);
            cmd.SetGlobalTexture("_NormalTex", normalView);

        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
       
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        RTHandles.Release(normalView);
    }
}
