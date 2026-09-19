/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Editor\NL_PaintableObject_UI.cs
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

    [CustomEditor(typeof(NL_PaintableObject))]
    public class NL_PaintableObject_UI : Editor
    {
        private NL_PaintableObject paintableObject;

        public override void OnInspectorGUI()
        {
            paintableObject = target as NL_PaintableObject;

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Total Brush")) NL_TotalBrush.OpenWindow();

            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Additional Vertex Streams data", "Additional Vertex Streams on the current Mesh Renderer."));

            EditorGUI.BeginDisabledGroup(paintableObject.cachedStreams == null);
            if (GUILayout.Button(new GUIContent("Restore", "Delete current painted data and restore previous vertex streams if it exists.")))
            {
                bool option = EditorUtility.DisplayDialog("Restore Previous Vertex Streams?", "You are about to DELETE CURRENT painted data and RESTORE PREVIOUS vertex streams. \nCurrent painted data will be lost. Are you sure?", "Yes, restore", "Cancel");
                switch (option)
                {
                    case true:
                        paintableObject.DeletePaintedData(true);
                        break;
                    case false:
                        break;
                }
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Clear", "Delete Vertex Streams completely including current painted data.")))
            {
                bool option = EditorUtility.DisplayDialog("Delete Vertex Streams including painted data?", "You are about to delete the Additional Vertex Streams from the Mesh Renderer including painted data. \nAre you sure?", "Yes, delete completely", "Cancel");
                switch (option)
                {
                    case true:
                        paintableObject.DeletePaintedData(false);
                        break;
                    case false:
                        break;
                }
            }

            if(GUILayout.Button("Check Streams"))
            {
                paintableObject.CheckStreams();
            }

            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (paintableObject != null) serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
