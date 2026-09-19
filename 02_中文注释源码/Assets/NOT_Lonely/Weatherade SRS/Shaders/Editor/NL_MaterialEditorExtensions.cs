/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Shaders\Editor\NL_MaterialEditorExtensions.cs
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

public static class MaterialEditorExtension
{
    public static void MinMaxSliderWithFloats(this MaterialEditor editor, MaterialProperty remapProp, float minLimit, float maxLimit, GUIContent label)
    {
        MaterialEditor.BeginProperty(remapProp);

        Vector2 remap = remapProp.vectorValue;

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PrefixLabel(label);

        //remap.x = EditorGUILayout.FloatField(remap.x, GUILayout.MaxWidth(50));
        EditorGUILayout.MinMaxSlider(GUIContent.none, ref remap.x, ref remap.y, minLimit, maxLimit);
        //remap.y = EditorGUILayout.FloatField(remap.y, GUILayout.MaxWidth(50));

        if (EditorGUI.EndChangeCheck())
            remapProp.vectorValue = remap;

        EditorGUILayout.EndHorizontal();
        MaterialEditor.EndProperty();
    }
}
