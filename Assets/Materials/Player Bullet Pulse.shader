Shader "Unlit/Player Bullet Pulse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (0, 0, 1, 1) // Default Blue
        _PulseSpeed ("Pulse Speed", Float) = 10.0
        _Opacity ("Opacity", Range(0, 1)) = 1.0 //Default Opacity

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
               
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _PulseSpeed;
            float _Opacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float rawPulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float sharpPulse = pow(rawPulse, 8.0);
                fixed4 baseCol = col * _Color;
                fixed4 finalCol = lerp(baseCol, fixed4(1,1,1,1), sharpPulse);
                finalCol.a = _Opacity;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalCol);
                return finalCol;
            }
            ENDCG
        }
    }
}
