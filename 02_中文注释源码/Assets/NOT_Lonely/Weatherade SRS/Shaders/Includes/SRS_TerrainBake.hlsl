// ================================================================================
// 【Codex 中文研读注释｜学习副本，不是原作者注释】
// 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\Includes\SRS_TerrainBake.hlsl
// 分类/优先级：Terrain 雪支持 / B-支持
// 文件职责：修改后的 URP Terrain 输入、splat、各渲染 Pass 与 Bake。
// 数据流位置：TerrainData/TerrainLayer -> 底表面 -> SnowCoverage。
// 主要 Unity 技术：URP Terrain Lit、GPU Instancing、Splat Map、Terrain holes
// 建议关注：与同名 Shader Pass 配对读。
// 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
// ================================================================================

#ifndef SRS_TERRAIN_BAKE_INCLUDED
#define SRS_TERRAIN_BAKE_INCLUDED

//SelectMap(half4(albedo.rgb, smoothness), inputData.normalWS, snowMask)

half3 SelectMap(float4 mixedAlbedo, float3 worldNormal, float finalMask)
{
    half3 map = float3(0, 0, 0);
	half3 nrm = worldNormal * 0.5 + 0.5;

	map = _MapID == 3.0 ? half3(finalMask, finalMask, finalMask) : map; //coverage mask
	map = _MapID == 2.0 ? half3(mixedAlbedo.a, mixedAlbedo.a, mixedAlbedo.a) : map; //smoothness
    map = _MapID == 1.0 ? nrm.xzy : map; //normals
	map = _MapID == 0.0 ? mixedAlbedo.rgb : map; //albedo

    return map;
}

#endif
