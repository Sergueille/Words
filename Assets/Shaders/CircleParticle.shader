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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _InnerSize;
            float _OuterSize;
            float4 _Color;
            sampler2D _FlowTex;
            float _FlowAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float flow = tex2D(_FlowTex, i.uv);
                float2 shift = (flow - float2(0.5, 0.5)) * _FlowAmount;

                float2 centeredUV = (i.uv + shift - float2(0.5, 0.5)) * 2;
                float sqrDist = dot(centeredUV, centeredUV);

                float2 d = ddx(i.uv.x);
                float fadeOut = clamp((_OuterSize * _OuterSize - sqrDist) / d * 0.5, 0, 1);
                float fadeIn = clamp((sqrDist - _InnerSize * _InnerSize) / d * 0.5, 0, 1);


                return float4(_Color.r, _Color.g, _Color.b, _Color.a * fadeIn * fadeOut);
            }
            ENDCG
        }
    }
}
