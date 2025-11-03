Shader "Unlit/TilemapGridOverlay"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _CellSize ("Cell Size", Vector) = (1,1,0,0)
        _Thickness ("Line Thickness", Range(0.001, 0.1)) = 0.02
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _GridEnabled ("Grid Enabled", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        LOD 100
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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            float4 _GridColor;
            float4 _CellSize;
            float _Thickness;
            float _Opacity;
            float _GridEnabled;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // If disabled, make transparent
                if (_GridEnabled < 0.5)
                    return half4(0,0,0,0);

                // World position, scaled to cell size
                float2 gridUV = IN.positionWS.xy / _CellSize.xy;

                // Fractional position within each grid cell
                float2 cellPos = frac(gridUV);

                // Distance to the nearest horizontal and vertical grid lines
                float2 edgeDist = min(cellPos, 1.0 - cellPos);

                // Determine if this fragment is part of a grid line
                float lineMask = step(edgeDist.x, _Thickness) + step(edgeDist.y, _Thickness);
                lineMask = saturate(lineMask); // clamp between 0–1

                // Apply opacity
                float alpha = lineMask * _GridColor.a * _Opacity;

                // Transparent tiles, visible grid lines
                return half4(_GridColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
