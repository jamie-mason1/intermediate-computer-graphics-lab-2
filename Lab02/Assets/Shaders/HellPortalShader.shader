Shader "Unlit/HellPortalShader"
{
    Properties
    {
        _CenterOpacity ("Center Opacity", Range(0,1)) = 0.8
        _EdgeColor ("Edge Color", Color) = (1,0,0,1)
        _EdgePower ("Edge Size", Range(1,10)) = 5
        _EdgeIntensity ("Edge Glow", Range(0,5)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float _CenterOpacity;
            float4 _EdgeColor;
            float _EdgePower;
            float _EdgeIntensity;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.normal = UnityObjectToWorldNormal(v.normal);

                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnel = pow(1 - dot(normalize(i.normal), i.viewDir), _EdgePower);

                float edgeGlow = fresnel * _EdgeIntensity;

                float3 color = edgeGlow * _EdgeColor.rgb;

                float alpha = _CenterOpacity + edgeGlow;

                return float4(color, alpha);
            }

            ENDCG
        }
    }
}