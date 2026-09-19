/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Editor\NL_TotalBrushOverlayBtn.cs
 * 分类/优先级：可选可绘制覆盖工具 / C-扩展
 * 文件职责：TotalBrush 的运行时数据、SceneView 编辑器、Compute 和绘制 Shader。
 * 数据流位置：美术笔刷 -> 顶点色/PaintedMask -> PaintMaskRGBA。
 * 主要 Unity 技术：EditorTool、SceneView、ComputeShader、Undo、顶点色
 * 建议关注：不保存实时脚印；BasicSetupSnow 的 paintableCoverage 为 0。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using NOT_Lonely.TotalBrush;

[EditorToolbarElement(id, typeof(SceneView))]
public class NL_TotalBrushOverlayBtn : EditorToolbarButton
{
    public const string id = "OpenTotalBrush-btn";

    NL_TotalBrushOverlayBtn()
    {
        //text = "Open Total Brush";
        tooltip = "Open the Total Brush window";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/NOT_Lonely/TotalBrush/UI/ToolbarIcon_openTB.png");
        clicked += OnOpenTotalBrush;
    }

    void OnOpenTotalBrush()
    {
        NL_TotalBrushOverlay.OpenTotalBrush();
    }
}
