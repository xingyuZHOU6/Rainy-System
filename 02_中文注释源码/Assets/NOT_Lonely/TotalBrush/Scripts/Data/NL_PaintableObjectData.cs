/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Data\NL_PaintableObjectData.cs
 * 分类/优先级：可选可绘制覆盖工具 / C-扩展
 * 文件职责：TotalBrush 的运行时数据、SceneView 编辑器、Compute 和绘制 Shader。
 * 数据流位置：美术笔刷 -> 顶点色/PaintedMask -> PaintMaskRGBA。
 * 主要 Unity 技术：EditorTool、SceneView、ComputeShader、Undo、顶点色
 * 建议关注：不保存实时脚印；BasicSetupSnow 的 paintableCoverage 为 0。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

namespace NOT_Lonely.TotalBrush
{
    using UnityEngine;

    public class NL_PaintableObjectData : ScriptableObject
    {
        public Vector3[] vPositionsWS;
        public Vector3[] vPositionsSS;
        public Vector3[] normals;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct SourceVertex
        {
            public Vector3 pos;
            public Vector3 posSS;
            public Vector3 normal;
            public Vector4 color;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct CalculatedVertex
        {
            public Vector3 pos;
            public Vector3 posSS;
            public Vector3 normal;
            public Vector4 color;
        }

        public SourceVertex[] vertSource;
        public CalculatedVertex[] vertCalculated;

        public GraphicsBuffer vertBufferSource;
        public GraphicsBuffer vertBufferCalculated;
        public ComputeBuffer c_vPosBufferWS;
        public ComputeBuffer c_vPosBufferSS;
    }
}
