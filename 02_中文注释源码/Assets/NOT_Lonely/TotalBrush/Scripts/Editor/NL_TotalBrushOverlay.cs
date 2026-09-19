/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Editor\NL_TotalBrushOverlay.cs
 * 分类/优先级：可选可绘制覆盖工具 / C-扩展
 * 文件职责：TotalBrush 的运行时数据、SceneView 编辑器、Compute 和绘制 Shader。
 * 数据流位置：美术笔刷 -> 顶点色/PaintedMask -> PaintMaskRGBA。
 * 主要 Unity 技术：EditorTool、SceneView、ComputeShader、Undo、顶点色
 * 建议关注：不保存实时脚印；BasicSetupSnow 的 paintableCoverage 为 0。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using NOT_Lonely.TotalBrush;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), id_ovrl, "")]
public class NL_TotalBrushOverlay : ToolbarOverlay
{
    NL_TotalBrushOverlay() : base(NL_TotalBrushOverlayBtn.id)
    {

    }

    private const string id_ovrl = "TotalBrush-overlay";
    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new VisualElement();
        root.style.width = new StyleLength(new Length(32, LengthUnit.Pixel));
        root.style.height = new StyleLength(new Length(20, LengthUnit.Pixel));

        Button openTB = new Button(() => OpenTotalBrush());
        openTB.style.flexGrow = 1;
        openTB.style.marginBottom = 0;
        openTB.style.marginTop = 0;
        openTB.style.marginLeft = 0;
        openTB.style.marginRight = 0;
        openTB.text = "TB";

        root.Add(openTB);

        return root;
    }

    public static void OpenTotalBrush()
    {
        NL_TotalBrush.Mode mode = NL_TotalBrush.Mode.Mesh;

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Terrain>() != null)
            mode = NL_TotalBrush.Mode.Terrain;

        NL_TotalBrush.OpenWindowExternal(mode);
    }
}
