// ================================================================================
// 【Codex 中文研读注释｜学习副本，不是原作者注释】
// 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\CGIncludes\SRS_TerrainBake.cginc
// 分类/优先级：Built-in 管线兼容 / D-参考
// 文件职责：Built-in 管线的覆盖、雪、光照、Terrain 和细分实现。
// 数据流位置：Built-in Forward/Deferred 路径。
// 主要 Unity 技术：CGInclude、UnityCG、Built-in rendering
// 建议关注：当前项目运行 URP；CoverageCommon.cginc 例外，痕迹 Blit 直接依赖。
// 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
// ================================================================================

#ifndef SRS_TERRAIN_BAKE_SHADER
#define SRS_TERRAIN_BAKE_SHADER
uniform float _MapID;

float3 CalculateDistantNorm(float3 distantNormal, float3 geomN)
{
		float3 geomTangent = normalize(cross(geomN, float3(0, 0, 1)));
	    float3 geomBitangent = normalize(cross(geomTangent, geomN));
		distantNormal = distantNormal.x * geomTangent + distantNormal.y * geomBitangent + distantNormal.z * geomN;  
		distantNormal = distantNormal.xzy;
	return distantNormal;
}
void SelectMap(float4 mixedAlbedo, float3 worldNormal, float3 geomN, float finalMask, out float3 map)
{
	float3 nrm = worldNormal * 0.5 + 0.5;
	map = float3(0, 0, 0);
	map = _MapID == 3.0 ? float3(finalMask, finalMask, finalMask) : map;
	map = _MapID == 2.0 ? float3(mixedAlbedo.a, mixedAlbedo.a, mixedAlbedo.a) : map;
	map = _MapID == 1.0 ? float3(nrm.xz, 1) : map;
	map = _MapID == 0.0 ? mixedAlbedo.rgb : map;
}
#endif