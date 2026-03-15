Shader "Custom/GoldCube"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Range(0,1)) = 0.5
        _RoughnessMap ("Roughness Map (G)", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "white" {}
        _HeightScale ("Height Scale", Range(0, 0.1)) = 0.02
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TwoSided ("Enable Two-Sided Rendering", Float) = 2
        _ToggleTextures ("Toggle Texture", Range(0,1)) = 0.5

        // 🔥 EMISSION
        _EmissionColor ("Emission Color", Color) = (1,0.8,0,1)
        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 1
        _EmissionMap ("Emission Map", 2D) = "white" {}
        _UseEmissionMap ("Use Emission Map", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull [_TwoSided]

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _RoughnessMap;
        sampler2D _HeightMap;
        sampler2D _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_RoughnessMap;
            float2 uv_HeightMap;
            float2 uv_EmissionMap;
            float3 viewDir;
        };

        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        float _BumpScale;
        float _HeightScale;
        float _ToggleTextures;

        fixed4 _EmissionColor;
        float _EmissionIntensity;
        float _UseEmissionMap;

        float2 ParallaxOffset(float2 uv, float height, float3 viewDir)
        {
            float depth = height * _HeightScale;
            float3 viewTangent = normalize(viewDir);
            float parallax = depth * -viewTangent.z;
            return uv + parallax * viewTangent.xy;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c;

            if (_ToggleTextures >= 0.5)
            {
                float height = tex2D(_HeightMap, IN.uv_HeightMap).r;
                float2 parallaxUV = ParallaxOffset(IN.uv_MainTex, height, IN.viewDir);

                c = tex2D(_MainTex, parallaxUV) * _Color;
                o.Albedo = c.rgb;

                float roughness = tex2D(_RoughnessMap, IN.uv_RoughnessMap).g;
                o.Smoothness = lerp(_Glossiness, roughness, roughness);

                o.Normal = UnpackNormal(tex2D(_BumpMap, parallaxUV)) * _BumpScale;
                o.Alpha = c.a;
            }
            else
            {
                c = _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                o.Smoothness = _Glossiness;
            }

            o.Metallic = _Metallic;

            // 🔥 EMISSION LOGIC
            if (_UseEmissionMap >= 0.5)
            {
                fixed3 emissionTex = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb;
                o.Emission = emissionTex * _EmissionColor.rgb * _EmissionIntensity;
            }
            else
            {
                o.Emission = _EmissionColor.rgb * _EmissionIntensity;
            }
        }
        ENDCG
    }

    FallBack "Standard"
}