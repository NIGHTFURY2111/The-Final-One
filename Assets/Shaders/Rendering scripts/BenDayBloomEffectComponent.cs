using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable,VolumeComponentMenuForRenderPipeline("Custom/BenDayBloomEffect", typeof(UniversalRenderPipeline))]
public class BenDayBloomEffectComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => intensity.value > 0f;
    public bool IsTileCompatible() => false;

    [Header("Bloom Settings")]

    public FloatParameter threshold = new FloatParameter(0.9f,true);
    public FloatParameter intensity = new FloatParameter(1f,true);
    public ClampedFloatParameter scatter = new ClampedFloatParameter(0.7f,0,1,true);
    public IntParameter clamp = new IntParameter(65472,true);
    public ClampedIntParameter maxItters = new ClampedIntParameter(6, 0, 10);


    [Header("Benday")]

    public IntParameter dostDensity = new IntParameter(10, true);
    public ClampedFloatParameter dotsCutoff = new ClampedFloatParameter(0.4f, 0, 1, true);
   
}

   

