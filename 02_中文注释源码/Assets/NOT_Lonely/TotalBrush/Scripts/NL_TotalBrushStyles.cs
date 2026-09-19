/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\NL_TotalBrushStyles.cs
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
    using UnityEditor;
    using UnityEngine;

    public class NL_PainterStyles
    {
        public static GUIStyle GetBackgroundStyle(Color color)
        {
            GUIStyle style = new GUIStyle();
            Texture2D texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, color);
            texture.Apply();

            style.normal.background = texture;
            return style;
        }

        public static GUIStyle GetCenteredHeader()
        {
            GUIStyle style = new GUIStyle();

            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.765f, 0.765f, 0.765f, 1) : new Color(0.0351f, 0.0351f, 0.0351f, 1);

            return style;
        }

        public static GUIStyle GetPreviewBtnStyle()
        {
            GUIStyle style = new GUIStyle();

            style.padding = new RectOffset(1, 1, 1, 1);
            style.stretchHeight = false;
            style.stretchWidth = false;
            style.fixedWidth = 33;
            style.fixedHeight = 33;

            Texture2D bg = new Texture2D(1, 1);

            bg.SetPixel(0, 0, new Color(0.242f, 0.371f, 0.585f, 1));
            bg.Apply();

            style.active.background = bg;

            return style;
        }

        public static GUIStyle GetPreviewStyle()
        {
            GUIStyle style = new GUIStyle();

            style.padding = new RectOffset(1, 1, 1, 1);
            style.margin = new RectOffset(5, 0, 0, 0);
            style.stretchHeight = false;
            style.stretchWidth = false;
            style.fixedWidth = 128;
            style.fixedHeight = 128;

            return style;
        }

        public static GUIStyle GetCenteredItalicLabel()
        {
            GUIStyle style = new GUIStyle();

            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Italic;
            style.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.765f, 0.765f, 0.765f, 1) : new Color(0.0351f, 0.0351f, 0.0351f, 1);

            return style;
        }

        public static GUIStyle GetSceneLabel(Color textColor, Color backgroundColor)
        {
            GUIStyle style = new GUIStyle();

            Texture2D texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, backgroundColor);
            texture.Apply();

            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Italic;
            style.normal.textColor = textColor;
            style.normal.background = texture;
            style.fontSize = 15;
            style.padding = new RectOffset(2, 2, 2, 2);

            return style;
        }

        public static GUIStyle GetCustomCheckbox()
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniButton);

            style.alignment = TextAnchor.MiddleCenter;
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }
    }
}
#endif
