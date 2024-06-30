Shader "Unlit/Circle"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Tansparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = fixed4(GammaToLinearSpace(v.color.rgb), v.color.a);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed dist = length(i.uv - fixed2(0.5, 0.5));
                fixed pxdist = sqrt(ddx(dist) * ddx(dist) + ddy(dist) * ddy(dist));
                fixed a = 1.0 - clamp((dist - 0.5 + pxdist) / pxdist, 0, 1);

                return fixed4(_Color.r * i.color.r, _Color.g * i.color.g, _Color.b * i.color.b, _Color.a * a * i.color.a);
            }
            ENDCG
        }
    }
}
