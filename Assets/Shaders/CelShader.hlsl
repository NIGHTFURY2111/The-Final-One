#ifndef CEL_SHADED_INCLUDE
#define CEL_SHADED_INCLUDE
#ifndef SHADERGRAPH_PREVIEW
float3 celShadedLight(Light l, float3 n, float3 v, float diffuseStep1, float diffuseStep2, float diffuseStrength, float specularStrength, float specularPower)
{
    float3 L = normalize(normalize(l.direction));
        
    float diffuseLight = saturate(dot(L, n));

   

    if (diffuseLight < diffuseStep1)
    {
        diffuseLight = 0;
    }
    else if (diffuseLight < diffuseStep2)
    {
        diffuseLight = 0.7;
    }
    else
    {
        diffuseLight = 1.0;
    }
    
   
    float3 finalColor = diffuseLight * diffuseStrength * l.color * l.shadowAttenuation;
    
    
#ifdef SPECULAR_HIGHLIGHTS
       
        float3 H = normalize(L+v);
        float specularLight = pow(saturate(dot(H,n))  ,specularPower );
        specularLight = step(0.5,specularLight) ;
        finalColor += specularLight   * specularStrength * (1-step(diffuseLight,diffuseStep2)) * l.color * l.shadowAttenuation ; 
#endif
    return finalColor;

}
#endif
void LightingCelShaded_float(float fresnelPower, float colorStrength, float diffuseStep1, float diffuseStep2, float diffuseStrength, float specularStrength, float specularPower, float3 albedoColor, float3 fresnelColor, float3 positionWS, float3 normalWS, float3 V, out float3 color)
{


#if defined(SHADERGRAPH_PREVIEW)
    color = float3(0.0, 0.0, 1.0);
#else
#if SHADOWS_SCREEN
    float4 clipPos = GetClipSpacePosition(positionWS);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
#endif
    Light mainLight = GetMainLight(shadowCoord);
                
                
    float3 N = normalize(normalWS);
    
    float3 finalColor = celShadedLight(mainLight, N, V, diffuseStep1, diffuseStep2, diffuseStrength, specularStrength ,specularPower);
    uint addLightcount = GetAdditionalLightsCount();
    for (uint i = 0; i < addLightcount; i++)
    {
                    
        finalColor += celShadedLight(GetAdditionalLight(i, positionWS), N, V, diffuseStep1, diffuseStep2, diffuseStrength, specularStrength, specularPower);
    }
#ifdef FRESNEL_HIGHLIGHTS
                    float3 fresnel = saturate(1- dot(V,N));
                    finalColor +=  smoothstep(0.2,0.7,pow(fresnel,fresnelPower))* fresnelColor;
#endif
   
    finalColor += albedoColor * colorStrength;
                
    color =  finalColor;
    
#endif
}
#endif