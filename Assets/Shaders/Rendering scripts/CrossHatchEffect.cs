using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/CrossHatchEffect", typeof(UniversalRenderPipeline))]
public class CrosshatchEffectComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => Density.value > 0f;
    public bool IsTileCompatible() => false;

    [Header("Line Settings")]

   
    public FloatParameter Density = new FloatParameter(100f, true);
    public ClampedFloatParameter Width = new ClampedFloatParameter(0.7f, 0, 1, true);
   



}
