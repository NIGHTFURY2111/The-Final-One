
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class CustomPass : ScriptableRenderPass
{
    private Material BloomMaterial;
    private Material CompositeMaterial;
    private RTHandle ColorTarget;
  
    private RenderTextureDescriptor CamDesc;
    const int k_MaxPyramidSize = 16;
    private int[] _BloomMipUp;
    private int[] _BloomMipDown;
    private RTHandle[] m_BloomMipUp;
    private RTHandle[] m_BloomMipDown;
    private GraphicsFormat hdrFormat;
    private BenDayBloomEffectComponent Bloom;
    public CustomPass(Material Bloom,Material Composite)
    {
        BloomMaterial = Bloom;
        CompositeMaterial = Composite;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
  
        _BloomMipUp = new int[k_MaxPyramidSize];
        _BloomMipDown = new int[k_MaxPyramidSize];
        m_BloomMipUp = new RTHandle[k_MaxPyramidSize];
        m_BloomMipDown = new RTHandle[k_MaxPyramidSize];
        for (int i = 0; i < k_MaxPyramidSize; i++)
        {
            _BloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
            _BloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);
            // Get name, will get Allocated with descriptor later
            m_BloomMipUp[i] = RTHandles.Alloc(_BloomMipUp[i], name: "_BloomMipUp" + i);
            m_BloomMipDown[i] = RTHandles.Alloc(_BloomMipDown[i], name: "_BloomMipDown" + i);
            const FormatUsage usage = FormatUsage.Linear | FormatUsage.Render;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, usage))// HDR fallbackT 
            {
                hdrFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                
            }
            else
            {
                hdrFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
                
            }

        }
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        Bloom = VolumeManager.instance.stack.GetComponent<BenDayBloomEffectComponent>();
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Custom Post Process Effects")))
        {
            Texture ssaotexx = Shader.GetGlobalTexture("_ScreenSpaceOcclusionTexture");
            CompositeMaterial.SetTexture("_SSAO", ssaotexx);
            //Do the bloom pass here first  
            SetupBloom(cmd, ColorTarget);
            CompositeMaterial.SetFloat("_Density", Bloom.dostDensity.value);
            CompositeMaterial.SetFloat("_Cutoff", Bloom.dotsCutoff.value);

            RenderTextureDescriptor d = ColorTarget.rt.descriptor;
            RTHandle tempRT = RTHandles.Alloc(d, name: "_TempRT");

            Blitter.BlitCameraTexture(cmd, ColorTarget, tempRT);

           
            Blitter.BlitCameraTexture(cmd, tempRT, ColorTarget,CompositeMaterial, 0);
            
          
            RTHandles.Release(tempRT);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CamDesc = renderingData.cameraData.cameraTargetDescriptor;
    }
    public void setTarget(RTHandle Color) 
    {
        ColorTarget = Color;
        
    }

    RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
    {
        var desc = CamDesc; // Make a copy
        desc.depthBufferBits = (int)depthBufferBits;
        desc.msaaSamples = 1;
        desc.width = width;
        desc.height = height;
        desc.graphicsFormat = format;
        return desc;
    }


    private void SetupBloom(CommandBuffer cmd,RTHandle source)
    {
        // Start at half-res
        int downres = 1;
        int tw = CamDesc.width >> downres;
        int th = CamDesc.height >> downres;

        // Determine the iteration count
        int maxSize = Mathf.Max(tw, th);
        int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
        int mipCount = Mathf.Clamp(iterations, 1, Bloom.maxItters.value);

        // Pre-filtering parameters
        float clamp = Bloom.clamp.value;
        float threshold = Mathf.GammaToLinearSpace(Bloom.threshold.value);
        float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee

        // Material setup
        float scatter = Mathf.Lerp(0.05f, 0.95f, Bloom.scatter.value);
       
        BloomMaterial.SetVector("_Params", new Vector4(scatter, clamp, threshold, thresholdKnee));

        // Prefilter
        
        var desc = GetCompatibleDescriptor(tw, th, hdrFormat);
        for (int i = 0; i < mipCount; i++)
        {
            RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipUp[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipUp[i].name);
            RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipDown[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipDown[i].name);
            desc.width = Mathf.Max(1, desc.width >> 1);
            desc.height = Mathf.Max(1, desc.height >> 1);
        }
        Blitter.BlitCameraTexture(cmd, source, m_BloomMipDown[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, BloomMaterial, 0);

        // Downsample - gaussian pyramid
        var lastDown = m_BloomMipDown[0];
        for (int i = 1; i < mipCount; i++)
        {
            // Classic two pass gaussian blur - use mipUp as a temporary target
            //   First pass does 2x downsampling + 9-tap gaussian
            //   Second pass does 9-tap gaussian using a 5-tap filter + bilinear filtering
            Blitter.BlitCameraTexture(cmd, lastDown, m_BloomMipUp[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, BloomMaterial, 1);
            Blitter.BlitCameraTexture(cmd, m_BloomMipUp[i], m_BloomMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, BloomMaterial, 2);

            lastDown = m_BloomMipDown[i];
        }
        // Upsample (bilinear by default, HQ filtering does bicubic instead
        for (int i = mipCount - 2; i >= 0; i--)
        {
            var lowMip = (i == mipCount - 2) ? m_BloomMipDown[i + 1] : m_BloomMipUp[i + 1];
            var highMip = m_BloomMipDown[i];
            var dst = m_BloomMipUp[i];

            cmd.SetGlobalTexture("_SourceTexLowMip", lowMip);
            Blitter.BlitCameraTexture(cmd, highMip, dst, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, BloomMaterial, 3);
            
        }
        CompositeMaterial.SetTexture("_Bloom", m_BloomMipUp[0]);
        CompositeMaterial.SetFloat("_BloomIntensity", Bloom.intensity.value);
    }

   
    
}
