Shader "Custom/ConveyorBeltProcedural"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.2, 0.2, 0.2, 1)
        _StripeColor ("Stripe Color", Color) = (0.25, 0.25, 0.25, 1)
        _StripeCount ("Stripe Count", Float) = 20
        _Speed ("Scroll Speed", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            float4 _BaseColor;
            float4 _StripeColor;
            float _StripeCount;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Scroll UVs in X over time
                o.uv = v.uv;
                o.uv.x += _Time.y * _Speed;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Create repeating stripes
                float stripe = frac(i.uv.x * _StripeCount);
                float mask = step(0.5, stripe);

                // Mix base and stripe color
                return lerp(_BaseColor, _StripeColor, mask);
            }
            ENDCG
        }
    }
}