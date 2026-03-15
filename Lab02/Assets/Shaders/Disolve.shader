Shader "Unlit/Disolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Noise", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
        _EdgeWidth ("Edge Width", Range(0,0.1)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _MainTex;
        sampler2D _DissolveTex;

        float _DissolveAmount;
        float _EdgeWidth;
        fixed4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DissolveTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            float dissolve = tex2D(_DissolveTex, IN.uv_DissolveTex).r;

            float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, dissolve);

            if (dissolve < _DissolveAmount)
                clip(-1);

            o.Albedo = col.rgb;

            if (dissolve < _DissolveAmount + _EdgeWidth)
                o.Emission = _EdgeColor.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}