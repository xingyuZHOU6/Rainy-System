/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\Editor\SRS_TerrainUtility_editor.cs
 * 分类/优先级：编辑器与 Inspector / C-扩展
 * 文件职责：自定义 Inspector/ShaderGUI，处理分组、Undo、keyword 和 Override UI。
 * 数据流位置：SerializedProperty/MaterialProperty -> UI -> 材质状态。
 * 主要 Unity 技术：UnityEditor、ShaderGUI、Undo、Scene GUI
 * 建议关注：不参与 Player 每帧算法。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

#if UNITY_EDITOR
namespace NOT_Lonely.Weatherade
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SRS_TerrainUtility))]
    public class SRS_TerrainUtility_UI : Editor
    {
        private SRS_TerrainUtility srs_terrainUtility;
        private SerializedProperty basemapResolution;
        private SerializedProperty boundsMultiplier;
        private SerializedProperty tilingMultiplier;
        GUIStyle background;
        private float lastBoundsMultiplier;

        private void OnEnable()
        {
            basemapResolution = serializedObject.FindProperty("basemapResolution");
            boundsMultiplier = serializedObject.FindProperty("boundsMultiplier");
            tilingMultiplier = serializedObject.FindProperty("tilingMultiplier");

            background = EditorGUIUtility.isProSkin ? NL_Utilities.GetBackgroundStyle(new Color(1, 1, 1, 0.05f), "background") : NL_Utilities.GetBackgroundStyle(new Color(1, 1, 1, 0.2f), "background");
        }

        public override void OnInspectorGUI()
        {
            srs_terrainUtility = target as SRS_TerrainUtility;

            EditorGUI.BeginChangeCheck();

            NL_Utilities.BeginUICategory("BAKING", background);

            EditorGUILayout.PropertyField(tilingMultiplier, new GUIContent("Tiling Multiplier", "Terrain textures tiling multiplier. Use values lower than 1 to scale up  all textures of the terrain layers, so less tiling will be noticable on far distances."));
            EditorGUILayout.PropertyField(basemapResolution, new GUIContent("Texture Resolution", "Resolution of the LOD textures."));

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PrefixLabel("LOD Albedo (RGB) Smoothness (A)");
            EditorGUILayout.ObjectField(srs_terrainUtility.albedoLOD, typeof(Texture2D), false);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(new GUIContent("Re-assing", "Re-assign the texture to the main terrain material and the Base Map material."))) srs_terrainUtility.ReAssignTexture();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PrefixLabel("LOD Normal");
            EditorGUILayout.ObjectField(srs_terrainUtility.normalLOD, typeof(Texture2D), false);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(new GUIContent("Re-assing", "Re-assign the texture to the main terrain material and the Base Map material."))) srs_terrainUtility.ReAssignTexture();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Bake")) srs_terrainUtility.BakeMaps();

            NL_Utilities.EndUICategory();

            NL_Utilities.BeginUICategory("CLIPPING FIX", background);
            EditorGUILayout.PropertyField(boundsMultiplier, new GUIContent("Bounds Multiplier", "Terrain patch bounds multiplier. Increase this value if you are using the Displacement feature in terrain material to prevent unwanted culling of terrain patches."));
            if (srs_terrainUtility.boundsMultiplier != lastBoundsMultiplier)
            {
                srs_terrainUtility.UpdateBoundsMultiplier(srs_terrainUtility.boundsMultiplier);
                lastBoundsMultiplier = srs_terrainUtility.boundsMultiplier;

            }

            NL_Utilities.EndUICategory();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                srs_terrainUtility.ValicateValues();
            }
        }
    }
}
#endif