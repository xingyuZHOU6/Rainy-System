/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\Tools\NL_GradientFillTool.cs
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
    using UnityEngine;

    public class NL_GradientFillTool
    {
        public static void ApplyGradient(ComputeShader c_shader, Vector3 gradientStartPoint, Vector3 gradientEndPoint, bool clampGradientStart, bool clampGradientEnd, Texture2D gradient, Vector2 targetValMinMax, float opacity, RenderTexture depth, Matrix4x4 depthCamMatrix, float cullingBias, float nDevThreshold, bool[] channelMask, List<NL_PaintableObject> paintableObjects)
        {
            gradientStartPoint.y = SceneView.lastActiveSceneView.camera.pixelHeight - gradientStartPoint.y;
            gradientEndPoint.y = SceneView.lastActiveSceneView.camera.pixelHeight - gradientEndPoint.y;

            Vector3 gradientAxis = (gradientEndPoint - gradientStartPoint).normalized;

            int[] cm = NL_TotalBrushUtilities.ConvertChannelMask(channelMask);

            for (int i = 0; i < paintableObjects.Count; i++)
            {
                NL_PaintableObject obj = paintableObjects[i];

                if (obj.data.vPositionsSS == null) return;

                NL_TotalBrushUtilities.SetSourceVetData(obj, c_shader, "GradientFillVertexColors", obj.data.vPositionsWS, obj.data.vPositionsSS, obj.data.normals, obj.tempColors);

                c_shader.SetVector("gradientStartPoint", gradientStartPoint);
                c_shader.SetVector("gradientEndPoint", gradientEndPoint);
                c_shader.SetBool("clampGradientStart", clampGradientStart);
                c_shader.SetBool("clampGradientEnd", clampGradientEnd);
                c_shader.SetVector("gradientAxis", gradientAxis);
                c_shader.SetVector("camForward", SceneView.lastActiveSceneView.camera.transform.forward);
                c_shader.SetVector("targetValMinMax", targetValMinMax);
                c_shader.SetFloat("opacity", opacity);
                c_shader.SetFloat("cullingBias", cullingBias);
                c_shader.SetFloat("nDevThreshold", nDevThreshold);
                c_shader.SetTexture(obj.c_kernelId, "gradientTex", gradient);
                c_shader.SetMatrix("depthCamMatrix", depthCamMatrix);
                c_shader.SetTexture(obj.c_kernelId, "depth", depth);

                c_shader.SetInt("maskR", cm[0]);
                c_shader.SetInt("maskG", cm[1]);
                c_shader.SetInt("maskB", cm[2]);
                c_shader.SetInt("maskA", cm[3]);

                NL_TotalBrushUtilities.CalculateAndApplyVertColors(obj, c_shader);
            }
        }
    }
}
#endif