Shader "Unlit/TilemapGridOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridEnabled ("Grid Enabled (0/1)", Float) = 1
        _GridColor ("Grid Color", Color) = (1,0.8,0,1)
        _CellSize ("Cell Size (world units)", Vector) = (1,1,0,0)
        _Thickness ("Thickness (world units)", Float) = 0.02
        _Opacity ("Grid Opacity", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _GridEnabled;
            float4 _GridColor;
            float4 _CellSize;
            float _Thickness;
            float _Opacity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPosXY : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPosXY = worldPos.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);

                if (_GridEnabled < 0.5)
                    return baseCol;

                float2 worldPos = i.worldPosXY;
                float2 cell = max(_CellSize.xy, float2(1e-6, 1e-6));

                // position inside each cell (0–1)
                float2 cellUV = frac(worldPos / cell);

                // distance (in world units) to nearest vertical & horizontal edges
                float distToVerticalEdge = min(cellUV.x, 1.0 - cellUV.x) * cell.x;
                float distToHorizontalEdge = min(cellUV.y, 1.0 - cellUV.y) * cell.y;

                // Anti-aliasing width
                float pixelWidth = length(fwidth(worldPos));
                float edgeAA = max(_Thickness * 0.5, pixelWidth);

                // Compute smooth edges (1 at line center, 0 in between)
                float vLine = smoothstep(edgeAA, 0.0, distToVerticalEdge);
                float hLine = smoothstep(edgeAA, 0.0, distToHorizontalEdge);

                // Combine both lines (max of both)
                float lineMask = saturate(max(vLine, hLine));

                fixed4 gridCol = fixed4(_GridColor.rgb, _Opacity * lineMask);

                // Blend grid over base texture
                return lerp(baseCol, gridCol, gridCol.a);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Texture"
}
