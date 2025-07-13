Shader "MY/SH_NeonTetra"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UpColor ("UpColor", Color) = (1,1,1,1)
        _DownColor ("DownColor", Color) = (1,1,1,1)
        _BlendRange ("Blend Range", Range(0.01, 0.1)) = 0.5
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
                float4 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _UpColor;
            fixed4 _DownColor;
            float _BlendRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = (0.0,0.0,0.0,0.0);

                // 上下で色を変える
                float halfRange = _BlendRange * 0.5;
                float upFactor = smoothstep(-halfRange, halfRange, i.localPos.y);
                float downFactor = 1.0 - upFactor;

                // 各色を発光色として加算（ぼかして重ねる表現）
                col.rgb += _UpColor.rgb * _UpColor.a * upFactor;
                col.rgb += _DownColor.rgb * _DownColor.a * downFactor;

                fixed4 emissiveColor = lerp(_DownColor, _UpColor, upFactor);
                // ベースカラーに発光色を加算
                col.rgb += emissiveColor.rgb;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
