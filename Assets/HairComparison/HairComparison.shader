Shader "Custom/HairComparisonShader"
{
    Properties
    {
        // Texture inputs
        [NoScaleOffset]_PlayerHairTex ("Player Hair Texture", 2D) = "white" {}
        [NoScaleOffset]_TargetHairTex ("Target Hair Texture", 2D) = "white" {}
        
        // Comparison Parameters
        _ColorSimilarityThreshold ("Color Similarity Threshold", Range(0, 1)) = 0.1
        _ShapeSimilarityThreshold ("Shape Similarity Threshold", Range(0, 1)) = 0.1
        
        // Difficulty Scaling
        _ResolutionFactor ("Resolution Difficulty Factor", Range(0, 1)) = 0.5
        
        // Scoring Outputs
        [HideInInspector]_OverallScore ("Overall Score", Float) = 0
        [HideInInspector]_ColorScore ("Color Score", Float) = 0
        [HideInInspector]_ShapeScore ("Shape Score", Float) = 0
        
        // Mismatch Visualization
        _MismatchIntensity ("Mismatch Visualization Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }
        
        Pass 
        {
            Name "HairComparison"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // Texture samplers
            TEXTURE2D(_PlayerHairTex);
            SAMPLER(sampler_PlayerHairTex);
            TEXTURE2D(_TargetHairTex);
            SAMPLER(sampler_TargetHairTex);
            
            // Properties
            CBUFFER_START(UnityPerMaterial)
                float _ColorSimilarityThreshold;
                float _ShapeSimilarityThreshold;
                float _ResolutionFactor;
                float _MismatchIntensity;
            CBUFFER_END
            
            // Vertex input structure
            struct Attributes 
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            // Vertex output structure
            struct Varyings 
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            // Function to calculate color difference
            float CalculateColorDifference(float4 color1, float4 color2) 
            {
                return distance(color1.rgb, color2.rgb);
            }
            
            // Sobel edge detection function
            float DetectEdges(Texture2D tex, float2 uv, float2 texelSize) 
            {
                // Sample surrounding pixels
                float4 tl = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(-texelSize.x, -texelSize.y));
                float4 t  = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(0, -texelSize.y));
                float4 tr = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(texelSize.x, -texelSize.y));
                float4 l  = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(-texelSize.x, 0));
                float4 r  = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(texelSize.x, 0));
                float4 bl = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(-texelSize.x, texelSize.y));
                float4 b  = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(0, texelSize.y));
                float4 br = SAMPLE_TEXTURE2D(tex, sampler_PlayerHairTex, uv + float2(texelSize.x, texelSize.y));
                
                // Sobel X and Y gradients
                float sobelX = length(tr + 2*r + br - (tl + 2*l + bl));
                float sobelY = length(bl + 2*b + br - (tl + 2*t + tr));
                
                return sqrt(sobelX * sobelX + sobelY * sobelY);
            }
            
            // Vertex shader
            Varyings vert(Attributes input) 
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            // Fragment shader
            float4 frag(Varyings input) : SV_Target 
            {
                // Sample player and target hair textures
                float4 playerColor = SAMPLE_TEXTURE2D(_PlayerHairTex, sampler_PlayerHairTex, input.uv);
                float4 targetColor = SAMPLE_TEXTURE2D(_TargetHairTex, sampler_TargetHairTex, input.uv);
                
                // Calculate color difference
                float colorDifference = CalculateColorDifference(playerColor, targetColor);
                
                // Calculate edge differences
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float playerEdges = DetectEdges(_PlayerHairTex, input.uv, texelSize);
                float targetEdges = DetectEdges(_TargetHairTex, input.uv, texelSize);
                float edgeDifference = abs(playerEdges - targetEdges);
                
                // Apply resolution factor for difficulty scaling
                float adjustedColorThreshold = _ColorSimilarityThreshold * (1 - _ResolutionFactor);
                float adjustedShapeThreshold = _ShapeSimilarityThreshold * (1 - _ResolutionFactor);
                
                // Determine match status
                float colorMatch = colorDifference < adjustedColorThreshold ? 1 : 0;
                float shapeMatch = edgeDifference < adjustedShapeThreshold ? 1 : 0;
                
                // Calculate overall match score
                float overallScore = (colorMatch + shapeMatch) / 2.0;
                
                // Mismatch visualization
                float4 mismatchColor = float4(1, 0, 0, _MismatchIntensity);
                float4 finalColor = lerp(playerColor, mismatchColor, 
                    (1 - colorMatch) * _MismatchIntensity + 
                    (1 - shapeMatch) * _MismatchIntensity
                );
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    // Custom editor to expose shader properties
    CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
}