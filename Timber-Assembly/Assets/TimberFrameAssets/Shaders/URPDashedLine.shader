Shader "Custom/URPDashedLine"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [DashLength] _DashLength ("Dash Length", Float) = 1.0
        [GapLength] _GapLength ("Gap Length", Float) = 1.0
        [Tiling] _Tiling ("Tiling Factor", Float) = 5.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; 
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _DashLength;
                float _GapLength;
                float _Tiling;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float totalLength = _DashLength + _GapLength;
                
                // LineRenderer projects distance along the U (x) axis when set to Tile mode.
                float currentPos = fmod(IN.uv.x * _Tiling, totalLength);

                if (currentPos > _DashLength)
                {
                    discard;
                }

                return IN.color * _Color;
            }
            ENDHLSL
        }
    }
}
