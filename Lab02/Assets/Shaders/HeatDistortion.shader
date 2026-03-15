Shader "Unlit/HeatDistortion"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Strength ("Distortion Strength", Range(0,0.1)) = 0.02
        _Speed ("Distortion Speed", Float) = 0.5
        _Scale ("Noise Scale", Float) = 10
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }

        GrabPass { "_GrabTexture" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            sampler2D _NoiseTex;

            float _Strength;
            float _Speed;
            float _Scale;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 noiseUV = i.uv * _Scale;
                noiseUV.y += _Time.y * _Speed;

                float2 noise = tex2D(_NoiseTex, noiseUV).rg;
                float2 offset = (noise - 0.5) * _Strength;

                float4 col = tex2Dproj(_GrabTexture, i.grabPos + float4(offset,0,0));

                return col;
            }

            ENDCG
        }
    }
}