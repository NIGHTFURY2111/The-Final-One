Shader "Custom/CelShader"
{

   
    Properties
    {
        _Thickness ("Thickness", Float) = 1
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _ColorStrength("Albedo Strength",Range (0, 1)) = 0.8
        _DifuseStrength("Diffuse Strength",Range (0, 1)) = 0.3
        _DifuseStep1("Diffuse Step 1",Range (0, 1)) = 0.3
        _DifuseStep2("Diffuse Step 2",Range (0, 1)) = 0.7
        [Toggle(Specular_Highlights)]_SpecularHighlights("Specular Highlights", Float) = 0
        _SpecularStrength("specular Strength",Range (0, 1)) = 1
        _SpecularPower("specular Power",float) = 300
        _SsaoStrength("SSAO Strength",Range (0, 1)) = 1
        [Toggle(USE_PRECALCULATED_OUTLINE_NORMALS)]_PrecalculateNormals("Use Optimised normals", Float) = 0
     
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"   "UniversalMaterialType" = "Lit" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward"} 
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN 
            #pragma shader_feature Specular_Highlights
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
           


            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoords : TEXCOORD3;
            };

            float4 _Color;
            float _ColorStrength;
            float _DifuseStrength;
            float _SpecularStrength;
            float _SpecularPower;
            float _DifuseStep1;
            float _DifuseStep2;


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = positions.positionCS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = positions.positionWS;
                float4 shadowCoordinates = GetShadowCoord(positions);
                OUT.shadowCoords = shadowCoordinates;
                
                
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {   
                
                Light mainLight = GetMainLight(IN.shadowCoords);
                float shadowAmount = mainLight.shadowAttenuation;    
                shadowAmount = step(0.5, shadowAmount); 
             
                
                float3 N = normalize(IN.normalWS);
                float3 L = normalize(normalize(mainLight.direction));
                    
                float diffuseLight = saturate(dot(L,N));
                if(diffuseLight < _DifuseStep1)
                {
                    diffuseLight = 0;
                }
                else if(diffuseLight < _DifuseStep2)
                {
                    diffuseLight = 0.7;
                }
                else
                {
                    diffuseLight = 1.0;
                }
                
               
                float3 finalColor = diffuseLight * _DifuseStrength * mainLight.color * shadowAmount ;
                
                
                #ifdef Specular_Highlights
                    float3 V = normalize(_WorldSpaceCameraPos - IN.positionWS);
                    float3 H = normalize(L+V);
                    float specularLight = pow(saturate(dot(H,N))  ,_SpecularPower );
                    specularLight = step(0.5,specularLight) ;
                    finalColor += specularLight   * _SpecularStrength * (1-step(diffuseLight,_DifuseStep2)) *mainLight.color * shadowAmount ; 
                #endif
                
               
                finalColor += _Color.xyz * _ColorStrength ;
                
                return float4(saturate(finalColor),1);
            }
           
               
            ENDHLSL
        }
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit"}
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_PRECALCULATED_OUTLINE_NORMALS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                #ifdef USE_PRECALCULATED_OUTLINE_NORMALS
                    float3 smoothNormalOS   : TEXCOORD1; 
                #endif

            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float _Thickness;
            float4 _OutlineColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 normalOS;
                #ifdef USE_PRECALCULATED_OUTLINE_NORMALS
                    normalOS = IN.smoothNormalOS;
                #else
                    normalOS = IN.normalOS;
                #endif
                float3 posOS = IN.positionOS.xyz + normalOS * _Thickness;
                OUT.positionCS = TransformObjectToHClip(posOS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                
                return _OutlineColor;
            }
            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            tags { "lightmode" = "depthnormals" }
            ZWrite On
           
        
            HLSLPROGRAM
            #pragma vertex vertDepthNormals
            #pragma fragment fragDepthNormals
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
        
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            float _SsaoStrength;
        
            Varyings vertDepthNormals(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }
        
            float4 fragDepthNormals(Varyings IN) : SV_Target
            {
                
                return float4(NormalizeNormalPerPixel(IN.normalWS) * 0.5 + _SsaoStrength , 1.0);
            }
            ENDHLSL
        }
        Pass
        {
            
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
          
            ColorMask 0 

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes 
            {
            	float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings 
            {
            	float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };
            
            
            float3 _LightDirection;
            float4 GetShadowCasterPositionCS(float3 positionWS, float3 normalWS) 
            {
	            float3 lightDirectionWS = _LightDirection;
	            
	            float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
	            
                #if UNITY_REVERSED_Z
                	positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                	positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                return positionCS;
            }
           
            
            
            Varyings vert(Attributes IN)   
            {
            	Varyings OUT;
            
            	
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = GetShadowCasterPositionCS(positionWS, OUT.normalWS);
            	return OUT;
            }
            
            float4 frag(Varyings IN) : SV_TARGET {
            	return 0;
            }
            ENDHLSL
        }
    }

}
