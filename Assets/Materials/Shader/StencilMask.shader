Shader "Custom/StencilMask"
{
    Properties
    {
        [PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
        [HideInInspector]  _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector]  _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector]  _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData]  _AlphaTex ("External Alpha", 2D) = "white" { }
        [PerRendererData]  _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-100" }
        ColorMask 0
        ZWrite off
        LOD 200

        Stencil {
            Ref 1
            Pass replace
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
