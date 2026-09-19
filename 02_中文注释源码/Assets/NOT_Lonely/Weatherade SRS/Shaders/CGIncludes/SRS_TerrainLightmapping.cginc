// ================================================================================
// 【Codex 中文研读注释｜学习副本，不是原作者注释】
// 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\CGIncludes\SRS_TerrainLightmapping.cginc
// 分类/优先级：Built-in 管线兼容 / D-参考
// 文件职责：Built-in 管线的覆盖、雪、光照、Terrain 和细分实现。
// 数据流位置：Built-in Forward/Deferred 路径。
// 主要 Unity 技术：CGInclude、UnityCG、Built-in rendering
// 建议关注：当前项目运行 URP；CoverageCommon.cginc 例外，痕迹 Blit 直接依赖。
// 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
// ================================================================================

#ifndef SRS_TERRAIN_LIGHTMAPPING_INCLUDED
#define SRS_TERRAIN_LIGHTMAPPING_INCLUDED

#include "UnityPBSLighting.cginc"
#include "UnityMetaPass.cginc"
#include"UnityStandardMeta.cginc"

#if defined(TERRAIN_BASE_PASS)
    sampler2D _MainTex;
    float4 _MainTex_ST;
#endif

struct VertexData {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
};

v2f VertMeta(VertexData v)
{
    v2f i;

    UNITY_INITIALIZE_OUTPUT(VertexData, i);
    /*
    float2 lmUV = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
    v.vertex.xy = lmUV;
	v.vertex.z = 0.5;
    v.vertex.w = 1;
    */
    
#ifdef TERRAIN_BASE_PASS
    i.pos = UnityObjectToClipPos(v.vertex);
#else
    i.pos = v.vertex;
#endif
    //i.pos = float4(lmUV * 2 - 1, 0, 1);
    i.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);

    return i;
}

float4 FragMeta(v2f i) : SV_TARGET
{
    UnityMetaInput surfaceData;
	surfaceData.Emission = 0;

    float oneMinusReflectivity;
	surfaceData.Albedo = DiffuseAndSpecularFromMetallic(float4(1, 1, 1, 1), 0, surfaceData.SpecularColor, oneMinusReflectivity);
    surfaceData.SpecularColor = float4(0, 0, 0, 1);

	return UnityMetaFragment(surfaceData);
}

#endif