/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\Editor\URP_LitDetailGUI.cs
 * 分类/优先级：编辑器与 Inspector / C-扩展
 * 文件职责：自定义 Inspector/ShaderGUI，处理分组、Undo、keyword 和 Override UI。
 * 数据流位置：SerializedProperty/MaterialProperty -> UI -> 材质状态。
 * 主要 Unity 技术：UnityEditor、ShaderGUI、Undo、Scene GUI
 * 建议关注：不参与 Player 每帧算法。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class URP_LitDetailGUI
{
    internal static class Styles
    {
        public static readonly GUIContent detailInputs = EditorGUIUtility.TrTextContent("Detail Inputs",
            "These settings define the surface details by tiling and overlaying additional maps on the surface.");

        public static readonly GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Mask",
            "Select a mask for the Detail map. The mask uses the alpha channel of the selected texture. The Tiling and Offset settings have no effect on the mask.");

        public static readonly GUIContent detailAlbedoMapText = EditorGUIUtility.TrTextContent("Base Map",
            "Select the surface detail texture.The alpha of your texture determines surface hue and intensity.");

        public static readonly GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map",
            "Designates a Normal Map to create the illusion of bumps and dents in the details of this Material's surface.");

        public static readonly GUIContent detailAlbedoMapScaleInfo = EditorGUIUtility.TrTextContent("Setting the scaling factor to a value other than 1 results in a less performant shader variant.");
        public static readonly GUIContent detailAlbedoMapFormatError = EditorGUIUtility.TrTextContent("This texture is not in linear space.");
    }

    public struct LitProperties
    {
        public MaterialProperty detailMask;
        public MaterialProperty detailAlbedoMapScale;
        public MaterialProperty detailAlbedoMap;
        public MaterialProperty detailNormalMapScale;
        public MaterialProperty detailNormalMap;

        public LitProperties(MaterialProperty[] properties)
        {
            detailMask = BaseShaderGUI.FindProperty("_DetailMask", properties, false);
            detailAlbedoMapScale = BaseShaderGUI.FindProperty("_DetailAlbedoMapScale", properties, false);
            detailAlbedoMap = BaseShaderGUI.FindProperty("_DetailAlbedoMap", properties, false);
            detailNormalMapScale = BaseShaderGUI.FindProperty("_DetailNormalMapScale", properties, false);
            detailNormalMap = BaseShaderGUI.FindProperty("_DetailNormalMap", properties, false);
        }
    }

    public static void DoDetailArea(LitProperties properties, MaterialEditor materialEditor)
    {
        materialEditor.TexturePropertySingleLine(Styles.detailMaskText, properties.detailMask);
        materialEditor.TexturePropertySingleLine(Styles.detailAlbedoMapText, properties.detailAlbedoMap,
            properties.detailAlbedoMap.textureValue != null ? properties.detailAlbedoMapScale : null);
        if (properties.detailAlbedoMapScale.floatValue != 1.0f)
        {
            EditorGUILayout.HelpBox(Styles.detailAlbedoMapScaleInfo.text, MessageType.Info, true);
        }
        var detailAlbedoTexture = properties.detailAlbedoMap.textureValue as Texture2D;
        if (detailAlbedoTexture != null && GraphicsFormatUtility.IsSRGBFormat(detailAlbedoTexture.graphicsFormat))
        {
            EditorGUILayout.HelpBox(Styles.detailAlbedoMapFormatError.text, MessageType.Warning, true);
        }
        materialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, properties.detailNormalMap,
            properties.detailNormalMap.textureValue != null ? properties.detailNormalMapScale : null);
        materialEditor.TextureScaleOffsetProperty(properties.detailAlbedoMap);
    }

    public static void SetMaterialKeywords(Material material)
    {
        if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailNormalMap") && material.HasProperty("_DetailAlbedoMapScale"))
        {
            bool isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
            bool hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
            CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
            CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
        }
    }
}
