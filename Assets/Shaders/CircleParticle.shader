Shader "Unlit/CircleParticle"
{
    Properties
    {
        _InnerSize ("Inner size", float) = 0.0
        _OuterSize ("Outer size", float) = 0.0
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _FlowTex ("Flow texture", 2D) = "black" {}
        _FlowAmount ("Flow amount", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
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
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed _InnerSize;
            fixed _OuterSize;
            fixed4 _Color;
            sampler2D _FlowTex;
            fixed _FlowAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed flow = tex2D(_FlowTex, i.uv);
                fixed2 shift = (flow - fixed2(0.5, 0.5)) * _FlowAmount;

                fixed2 centeredUV = (i.uv + shift - fixed2(0.5, 0.5)) * 2;
                fixed sqrDist = dot(centeredUV, centeredUV);

                fixed2 d = ddx(i.uv.x);
                fixed fadeOut = clamp((_OuterSize * _OuterSize - sqrDist) / d * 0.5, 0, 1);
                fixed fadeIn = clamp((sqrDist - _InnerSize * _InnerSize) / d * 0.5, 0, 1);


                return fixed4(_Color.r, _Color.g, _Color.b, _Color.a * fadeIn * fadeOut);
            }
            ENDCG
        }
    }
}
