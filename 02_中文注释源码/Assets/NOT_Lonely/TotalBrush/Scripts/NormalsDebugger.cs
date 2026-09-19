/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\TotalBrush\Scripts\NormalsDebugger.cs
 * 分类/优先级：可选可绘制覆盖工具 / C-扩展
 * 文件职责：TotalBrush 的运行时数据、SceneView 编辑器、Compute 和绘制 Shader。
 * 数据流位置：美术笔刷 -> 顶点色/PaintedMask -> PaintMaskRGBA。
 * 主要 Unity 技术：EditorTool、SceneView、ComputeShader、Undo、顶点色
 * 建议关注：不保存实时脚印；BasicSetupSnow 的 paintableCoverage 为 0。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class NormalsDebugger : MonoBehaviour
{
    [SerializeField] private bool showOriginalNormals;
    [SerializeField] private bool showUnifiedNormals;
    [SerializeField] private bool showVertices;
    [SerializeField] private float length = 0.5f;
    [SerializeField] private float verticesSize = 0.005f;

    [SerializeField] private Vector3[] vPos;
    [SerializeField] private Vector3[] originalNormals;
    [SerializeField] private List<Vector3> unifiedNormals = new List<Vector3>();

    private void OnEnable()
    {
        originalNormals = gameObject.GetComponent<MeshFilter>().sharedMesh.normals;
        vPos = gameObject.GetComponent<MeshFilter>().sharedMesh.vertices;

        Mesh streams = gameObject.GetComponent<MeshRenderer>().additionalVertexStreams;
        unifiedNormals = new List<Vector3>();
        if (streams != null)
        {
            streams.GetUVs(3, unifiedNormals);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Handles.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < vPos.Length; i++)
        {
            if (showOriginalNormals)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(vPos[i], originalNormals[i] * length);
            }

            if (showVertices)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(vPos[i], Vector3.one * verticesSize);
            }

            if (showUnifiedNormals)
            {
                if (unifiedNormals.Count > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(vPos[i], unifiedNormals[i] * length);
                }
            }
            //Handles.Label(vPos[i] + unifiedNormals[i] * length, i.ToString());
        }
    }
}
#endif
