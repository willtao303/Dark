Shader "Custom/StencilObject"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" { }
        _Color ("Tint", Color) = (1,1,1,1)
        _RendererColor ("RendererColor", Color) = (1,1,1,1)
        _Flip ("Flip", Vector) = (1,1,1,1)
        [MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
        [PerRendererData]  _AlphaTex ("External Alpha", 2D) = "white" { }
        [PerRendererData]  _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderQueue"="Geometry+1100" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" "PreviewType"="Plane"}
        LOD 200
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Stencil{
            Ref 1
            Comp equal
        }


        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
        ENDCG
        }
        

    }
    FallBack "Diffuse"
}
