// ================================================================================
// 【Codex 中文研读注释｜学习副本，不是原作者注释】
// 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\Includes\SRS_TerrainShadowCasterPass.hlsl
// 分类/优先级：Terrain 雪支持 / B-支持
// 文件职责：修改后的 URP Terrain 输入、splat、各渲染 Pass 与 Bake。
// 数据流位置：TerrainData/TerrainLayer -> 底表面 -> SnowCoverage。
// 主要 Unity 技术：URP Terrain Lit、GPU Instancing、Splat Map、Terrain holes
// 建议关注：与同名 Shader Pass 配对读。
// 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
// ================================================================================


#ifndef SRS_TERRAIN_SHADOW_CASTER_INCLUDED
#define SRS_TERRAIN_SHADOW_CASTER_INCLUDED

#include "SRS_CoverageCommon.hlsl" //SRS: common functions
#include "SRS_Lighting.hlsl" //SRS: use this include instead of standard

// Shadow pass

// Shadow Casting Light geometric parameters. These variables are used when applying the shadow Normal Bias and are set by UnityEngine.Rendering.Universal.ShadowUtils.SetupShadowCasterConstantBuffer in com.unity.render-pipelines.universal/Runtime/ShadowUtils.cs
// For Directional lights, _LightDirection is used when applying shadow Normal Bias.
// For Spot lights and Point lights, _LightPosition is used to compute the actual light direction because it is different at each shadow caster geometry vertex.
float3 _LightDirection;
float3 _LightPosition;

struct Attributes
{
    float4 positionOS     : POSITION;
    float3 normalOS       : NORMAL;
    //SRS: use float3 to store the tess mask in Z
    #if defined (_TESSELLATION_ON)
        float3 texcoord     : TEXCOORD0;
    #else
        float2 texcoord     : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 clipPos      : SV_POSITION;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

//SRS includes

#if defined(SRS_SNOW_COVERAGE_SHADER)
    #include "SRS_SnowCoverage.hlsl"
#else
    #include "../CGIncludes/SRS_RainCoverage.cginc"
#endif

Varyings ShadowPassVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    #if !defined(_TESSELLATION_ON)
        TerrainInstancing(v.positionOS, v.normalOS, v.texcoord);
    #endif

    //SRS: do displacement in world space and then convert from world to clip space (default URP shader converts from local to clip directly)
    float3 posWS = (float3)0;
    #if defined(_COVERAGE_ON) && defined(_DISPLACEMENT_ON)
        posWS = Displace(v.normalOS, v.positionOS.xyz, v.texcoord.xy, 0);
    #else
        posWS = TransformObjectToWorld(v.positionOS.xyz);
    #endif
    //
    float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

#if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - posWS);
#else
    float3 lightDirectionWS = _LightDirection;
#endif

    float4 clipPos = TransformWorldToHClip(ApplyShadowBias(posWS, normalWS, lightDirectionWS));

#if UNITY_REVERSED_Z
    clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
#else
    clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
#endif

    o.clipPos = clipPos;

    o.texcoord = v.texcoord.xy;

    return o;
}

half4 ShadowPassFragment(Varyings IN) : SV_TARGET
{
#ifdef _ALPHATEST_ON
    ClipHoles(IN.texcoord);
#endif
    return 0;
}

#endif
