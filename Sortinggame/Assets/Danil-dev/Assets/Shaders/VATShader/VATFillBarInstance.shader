Shader "D-Dev/VAT/FillBarInstance"
{
    Properties
    {
        [Header(Fill Colors)]
        _MainColorTop   ("Fill Color Top",    Color) = (0.24, 0.85, 0.48, 1)
        _MainColorBot   ("Fill Color Bottom", Color) = (0.15, 0.77, 0.39, 1)
        _DamageColor    ("Damage Color",      Color) = (0.96, 0.78, 0.26, 1)
        _BackColor      ("Background Color",  Color) = (0.10, 0.10, 0.18, 1)

        [Header(Border)]
        _BorderColor    ("Border Color",      Color) = (0.18, 1.0, 0.54, 1)
        // In HEIGHT-FRACTIONS (0 = sharp, 0.5 = full semicircle at the ends)
        _BorderSize     ("Border Thickness",  Range(0.0, 0.25)) = 0.08
        // Corner radius also in HEIGHT-FRACTIONS, independent of bar width
        _CornerRadius   ("Corner Radius",     Range(0.0, 0.5))  = 0.18

        [Header(Shine)]
        _ShineStrength  ("Shine Strength",    Range(0.0, 1.0))  = 0.55
        _ShineHeight    ("Shine Height",      Range(0.0, 1.0))  = 0.45
        _ShineAngle     ("Shine Streak X",    Range(0.0, 1.0))  = 0.38

        [Header(Edge Glow)]
        _GlowColor      ("Glow Color",        Color) = (0.18, 1.0, 0.54, 1)
        _GlowWidth      ("Glow Width",        Range(0.0, 0.5))  = 0.12
        _GlowStrength   ("Glow Strength",     Range(0.0, 1.0))  = 0.85
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float  aspect     : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Fill)
                UNITY_DEFINE_INSTANCED_PROP(float, _DelayedFill)
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 _MainColorTop;
            float4 _MainColorBot;
            float4 _DamageColor;
            float4 _BackColor;

            float4 _BorderColor;
            float  _BorderSize;
            float  _CornerRadius;

            float  _ShineStrength;
            float  _ShineHeight;
            float  _ShineAngle;

            float4 _GlowColor;
            float  _GlowWidth;
            float  _GlowStrength;

            // Rounded-rect SDF. All inputs in the same unit space.
            float SdRoundBox(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 worldPos = GetObjectToWorldMatrix()._m03_m13_m23;
                float3 viewPos  = TransformWorldToView(worldPos);

                float scaleX = length(GetObjectToWorldMatrix()._m00_m10_m20);
                float scaleY = length(GetObjectToWorldMatrix()._m01_m11_m21);

                float3 pos        = viewPos + float3(input.positionOS.xy * float2(scaleX, scaleY), 0);
                output.positionCS = TransformWViewToHClip(pos);
                output.uv         = input.uv;
                output.aspect     = (scaleY > 0.0001) ? scaleX / scaleY : 1.0;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float fill  = UNITY_ACCESS_INSTANCED_PROP(Props, _Fill);
                float dFill = UNITY_ACCESS_INSTANCED_PROP(Props, _DelayedFill);

                float2 uv     = input.uv;
                float  aspect = input.aspect;

                // Work in "height-unit" space so radius/border are aspect-independent.
                // uv is [0,1]x[0,1]; remap to [-0.5*aspect, 0.5*aspect] x [-0.5, 0.5]
                float2 p = (uv - 0.5) * float2(aspect, 1.0);

                float  r       = _CornerRadius;                          // height-units
                float  brd     = _BorderSize;                            // height-units
                float2 outerHE = float2(aspect * 0.5, 0.5);
                float2 innerHE = float2(aspect * 0.5 - brd, 0.5 - brd);
                float  innerR  = max(0.0, r - brd);

                float outerDist = SdRoundBox(p, outerHE, r);
                float innerDist = SdRoundBox(p, innerHE, innerR);

                float outerAlpha = 1.0 - smoothstep(-0.01, 0.01, outerDist);
                if (outerAlpha < 0.001) discard;

                bool inContent = innerDist < 0.0;
                bool inBorder  = (outerDist < 0.0) && !inContent;

                // Remap UV into the inner content rect (same shrink as innerHE)
                // innerHE in height-units → convert back to UV fractions
                float2 innerUVHE   = innerHE / float2(aspect, 1.0);   // [0..0.5] in UV space
                float2 contentMin  = 0.5 - innerUVHE;
                float2 contentSize = innerUVHE * 2.0;
                float2 uvC = saturate((uv - contentMin) / contentSize);

                // ── Base content color ─────────────────────────────────────
                half4 col = _BackColor;

                if (inContent)
                {
                    half4 fillCol = lerp(_MainColorBot, _MainColorTop, uvC.y);

                    if (uvC.x < fill)
                        col = fillCol;
                    else if (uvC.x < dFill)
                        col = _DamageColor;
                    else
                        col = _BackColor;

                    // ── Edge glow ──────────────────────────────────────────
                    if (_GlowWidth > 0.001 && fill > 0.01)
                    {
                        float distToEdge = abs(uvC.x - fill);
                        float glow = pow(1.0 - smoothstep(0.0, _GlowWidth, distToEdge), 1.8);
                        glow *= _GlowStrength;
                        float side = smoothstep(fill + 0.01, fill - 0.01, uvC.x) * 0.6
                                   + smoothstep(fill - 0.01, fill + 0.04, uvC.x) * 0.4;
                        glow *= lerp(0.25, 1.0, side);
                        col.rgb = lerp(col.rgb, _GlowColor.rgb, glow * _GlowColor.a);
                    }

                    // ── Shine ──────────────────────────────────────────────
                    if (_ShineStrength > 0.001 && uvC.x < fill)
                    {
                        float shineV = 1.0 - smoothstep(0.0, _ShineHeight, 1.0 - uvC.y);
                        shineV = shineV * shineV;
                        float streak = abs(uvC.x - _ShineAngle) / 0.22;
                        float shineH = pow(saturate(1.0 - streak), 2.0) * 0.5;
                        float shine  = saturate(shineV + shineH) * _ShineStrength;
                        col.rgb += shine * 0.9;
                    }
                }

                // ── Border ─────────────────────────────────────────────────
                if (inBorder)
                    col = _BorderColor;

                col.a *= outerAlpha;
                return col;
            }
            ENDHLSL
        }
    }
}
