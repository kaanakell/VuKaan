Shader "CustomRenderTexture/ScreenColorReveal"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "ScreenColorReveal"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float2 _CursorScreenPos;
            float _RevealRadius;
            float _EdgeSoftness;
            float _AspectRatio;

            half4 Frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                half lum = dot(col.rgb, half3(0.2126h, 0.7152h, 0.0722h));
                half grey = half3(lum, lum, lum);

                float2 diff = input.texcoord - _CursorScreenPos;
                diff.x *= _AspectRatio;
                float dist = length(diff);

                float reveal = 1.0 - smoothstep(_RevealRadius - _EdgeSoftness, _RevealRadius + _EdgeSoftness, dist);

                return half4(lerp(grey, col.rgb, reveal), col.a);
            }
            ENDHLSL
        }
    }
}
