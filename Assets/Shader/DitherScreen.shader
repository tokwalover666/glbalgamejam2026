Shader "Custom/DitherScreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // Dither
        _DitherStrength ("Dither Strength", Range(0,1)) = 1
        _ColorSteps ("Color Steps", Range(2, 64)) = 8
        _PatternScale ("Pattern Scale", Range(0.25, 16)) = 1

        // Color shaping
        _Saturation ("Saturation", Range(0, 3)) = 1
        _Contrast ("Contrast", Range(0, 3)) = 1
        _Brightness ("Brightness", Range(-1, 1)) = 0
        _PosterizeStrength ("Posterize Strength", Range(0,1)) = 0

        // Style toggles
        _LumaDither ("Luma Dither (0/1)", Range(0,1)) = 0

        // Optional pixelation
        _Pixelate ("Pixelate (0=off)", Range(0, 512)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float _DitherStrength;
            float _ColorSteps;
            float _PatternScale;

            float _Saturation;
            float _Contrast;
            float _Brightness;
            float _PosterizeStrength;

            float _LumaDither;
            float _Pixelate;

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

            float Bayer4x4(int x, int y)
            {
                int bayer[4][4] = {
                    {  0,  8,  2, 10 },
                    { 12,  4, 14,  6 },
                    {  3, 11,  1,  9 },
                    { 15,  7, 13,  5 }
                };
                return bayer[y & 3][x & 3] / 16.0;
            }

            float3 ApplyColorControls(float3 c)
            {
                // Brightness
                c += _Brightness;

                // Contrast around 0.5
                c = (c - 0.5) * _Contrast + 0.5;

                // Saturation
                float l = dot(c, float3(0.2126, 0.7152, 0.0722));
                c = lerp(float3(l, l, l), c, _Saturation);

                return saturate(c);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Optional pixelate (nice with dither)
                if (_Pixelate > 0.5)
                {
                    float2 pix = _Pixelate.xx;
                    uv = (floor(uv * pix) + 0.5) / pix;
                }

                fixed4 col = tex2D(_MainTex, uv);
                float3 baseCol = ApplyColorControls(col.rgb);

                // Scale the bayer pattern in screen space
                float2 screenXY = i.uv * _ScreenParams.xy;
                screenXY /= max(_PatternScale, 0.0001);

                float threshold = Bayer4x4((int)screenXY.x, (int)screenXY.y);

                float steps = max(_ColorSteps, 2.0);

                float3 quantizedRGB;
                quantizedRGB.r = floor(baseCol.r * steps + threshold) / steps;
                quantizedRGB.g = floor(baseCol.g * steps + threshold) / steps;
                quantizedRGB.b = floor(baseCol.b * steps + threshold) / steps;

                // Luma-only option (more “classic” look)
                float luma = dot(baseCol, float3(0.2126, 0.7152, 0.0722));
                float qLuma = floor(luma * steps + threshold) / steps;
                float3 quantizedLuma = baseCol * (qLuma / max(luma, 1e-4));

                float3 quantized = lerp(quantizedRGB, quantizedLuma, step(0.5, _LumaDither));

                // Optional “posterize strength” blend (extra punch)
                float3 posterized = floor(baseCol * steps) / steps;
                float3 withPosterize = lerp(quantized, posterized, _PosterizeStrength);

                float3 result = lerp(baseCol, withPosterize, _DitherStrength);
                return float4(result, 1);
            }
            ENDCG
        }
    }
    FallBack Off
}
