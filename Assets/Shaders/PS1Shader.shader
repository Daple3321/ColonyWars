Shader "Custom/PS1Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (0, 0.5, 1, 1) // голубой
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _ShakeIntensity ("Shake Intensity", Float) = 2.0 // чем меньше, тем сильнее эффект
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0; // стандартна€, перспективна€ интерпол€ци€
                float3 light : COLOR;
            };

            sampler2D _MainTex;
            SamplerState _MainTex_sampler
            {
                Filter = MIN_MAG_MIP_POINT; // point filtering дл€ пикселизации
                AddressU = Wrap;
                AddressV = Wrap;
            };

            fixed4 _Color;
            fixed4 _LightColor;
            float _ShakeIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Ёффект "дрожащих" полигонов с настраиваемой интенсивностью:
                o.pos.xy = floor(o.pos.xy * _ShakeIntensity) / _ShakeIntensity;
                o.uv = v.uv;

                // ѕростейшее вершинное освещение (Ћамбертово):
                float3 lightDir = normalize(float3(0.5, 1, 0.5));
                float diff = max(0, dot(v.normal, lightDir));
                o.light = diff * _LightColor.rgb;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 col = texColor * _Color;
                col.rgb *= i.light;
                //  вантование цвета дл€ имитации 8-битной глубины:
                col.rgb = floor(col.rgb * 255.0) / 255.0;
                return col;
            }
            ENDCG
        }
    }
}
