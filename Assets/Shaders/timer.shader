Shader "Unlit/Circle"
{
    Properties
    {
        _MainTex ("MainTex (unused)", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorB ("Color B", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorC ("Color C", Color) = (1.0, 1.0, 1.0, 1.0)
        _Angle ("Angle", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
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

            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _ColorC;
            fixed _Angle;

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
                float2 centeredUV = i.uv - fixed2(0.5, 0.5);
                float dist = length(centeredUV);
                if (dist > 0.5) discard;

                float angle = -atan(centeredUV.x / centeredUV.y);
                if (angle < 0) angle += 3.14159265359;
                if (centeredUV.x > 0) angle += 3.14159265359;

                if (angle > _Angle + 2 * 3.14159265359) {
                    return _ColorC;
                }
                else if (angle > _Angle) {
                    return _ColorA;
                }
                else {
                    return _ColorB;
                }
            }
            ENDCG
        }
    }
}
