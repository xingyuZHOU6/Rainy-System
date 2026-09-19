/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Editor\NL_IsolationModeWindow.cs
 * 分类/优先级：可选可绘制覆盖工具 / C-扩展
 * 文件职责：TotalBrush 的运行时数据、SceneView 编辑器、Compute 和绘制 Shader。
 * 数据流位置：美术笔刷 -> 顶点色/PaintedMask -> PaintMaskRGBA。
 * 主要 Unity 技术：EditorTool、SceneView、ComputeShader、Undo、顶点色
 * 建议关注：不保存实时脚印；BasicSetupSnow 的 paintableCoverage 为 0。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

#if UNITY_EDITOR
namespace NOT_Lonely.TotalBrush
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    [EditorWindowTitle(title = "Isolation Window", useTypeNameAsIconName = false)]
    public class NL_IsolationModeWindow : SceneView
    {
        public static NL_IsolationStage stage;
        public static NL_IsolationModeWindow window;

        public static void SwitchIsolationMode(bool state, List<NL_PaintableObject> pObjects = null)
        {
            if (state)
            {
                window = GetWindow<NL_IsolationModeWindow>(typeof(SceneView));

                window.sceneLighting = false;
                window.drawGizmos = false;
                window.SetupWindow(pObjects);
                window.Repaint();
            }
            else
            {
                StageUtility.GoToMainStage();
                if (stage != null) DestroyImmediate(stage);
                if (window != null) window.Close();
            }
        }

        private void SetupWindow(List<NL_PaintableObject> pObjects = null)
        {
            titleContent = new GUIContent("Isolation Mode");

            stage = ScriptableObject.CreateInstance<NL_IsolationStage>();
            NL_IsolationStage.ownerWindow = this;
            stage.titleContent = titleContent;

            StageUtility.GoToStage(stage, true);

            stage.Setup(pObjects);
        }
    }
}
#endif
