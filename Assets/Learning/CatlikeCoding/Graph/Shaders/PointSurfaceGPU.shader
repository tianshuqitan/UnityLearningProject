Shader "Custom/Point Surface GPU"
{
    Properties
    {
        _Smoothness ("_Smoothness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma editor_sync_compilation // force unity to compile shader before used
        #pragma target 4.5 // OpenGL ES 3.1
        
        float _Smoothness;

        struct Input
        {
            float3 worldPos;
        };

        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            surface.Albedo.rg = saturate(input.worldPos.xy * 0.5 + 0.5);
            surface.Smoothness = _Smoothness;
        }

        #include "PointGPU.hlsl"
        
        ENDCG
    }

    FallBack "Diffuse"
}