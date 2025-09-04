Shader "Custom/BurnCard_Wavy"
{
    Properties
    {
        _MainTex ("Card Texture", 2D) = "white" {}
        _BurnTex ("Burn Mask (noise)", 2D) = "white" {}
        _BurnColor ("Burn Edge Color", Color) = (1,0.5,0,1)
        _BurnAmount ("Burn Amount", Range(0,1)) = 0
        _EdgeWidth ("Edge Width", Range(0.001,0.2)) = 0.05
        _WaveAmplitude ("Wave Amplitude", Range(0,0.2)) = 0.08
        _WaveFrequency ("Wave Frequency", Range(0,10)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BurnTex;
            float4 _MainTex_ST;
            float4 _BurnColor;
            float _BurnAmount;
            float _EdgeWidth;
            float _WaveAmplitude;
            float _WaveFrequency;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // основна текстура карти
                fixed4 col = tex2D(_MainTex, i.uv);

                // хвиляста крива вигорання
                float wave = sin(i.uv.x * _WaveFrequency * 3.1415) * _WaveAmplitude;

                // шум з текстури для нерівності краю
                float noise = tex2D(_BurnTex, i.uv * 2).r * 0.1;

                // маска вигорання: нижній лівий кут = 0
float burnMask = (i.uv.x + i.uv.y) / 2.0 + wave + noise;

                // поріг вигорання
                float burnThreshold = _BurnAmount;

                // наскільки піксель вигорів
                float alpha = step(burnThreshold, burnMask);

                // край згорання
                float edge = smoothstep(burnThreshold, burnThreshold + _EdgeWidth, burnMask);

                // застосування альфи та кольору краю
                col.a *= alpha;
                col.rgb = lerp(_BurnColor.rgb, col.rgb, edge);

                return col;
            }

            ENDCG
        }
    }
}
