Shader "Custom/MRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Id ("Id", float) = 0.5
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
                float depth : TEXCOORD1;
            };

            struct buffer {
                float4 buf1 : SV_Target0;
                float4 buf2 : SV_Target1;
                float4 buf3 : SV_Target2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Id;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                COMPUTE_EYEDEPTH(o.depth.x);
                o.depth *= _ProjectionParams.w;
                //for debug visualizing
                o.depth *= -1;
                o.depth += 1;
                //end debug
                return o;
            }

            buffer frag (v2f i)
            {
                buffer o;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                o.buf1 = col;
                o.buf2 = float4(i.depth.x, 0, 0, 1);
                o.buf3 = float4(0, _Id, 0, 1);

                return o;
            }
            ENDCG
        }
    }
}
