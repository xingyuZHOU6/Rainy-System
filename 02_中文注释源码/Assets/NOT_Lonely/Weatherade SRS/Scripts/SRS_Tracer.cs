/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\SRS_Tracer.cs
 * 分类/优先级：动态雪痕核心 / A-必读
 * 文件职责：为 MeshRenderer/SkinnedMeshRenderer 创建只写深度的代理物体，并放入痕迹相机专用 Layer。
 * 数据流位置：动态物原网格/骨骼 -> DepthOccluder 代理 -> traceObjsCam 深度图。
 * 主要 Unity 技术：运行时网格代理、SkinnedMesh bones、LayerMask、材质实例化
 * 建议关注：关注 parent、vertexPush、Renderer 类型分支和代理清理风险。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRS_Tracer : MonoBehaviour
{
    public string tracerLayerName = "Dynamic";
    public bool parent = true;
    public float vertexPush = 0;
    public Renderer[] renderers;

    private List<Transform> tracers = new List<Transform>();

    void OnEnable()
    {
        SetupTracers();
    }

    private void Update()
    {
        if (parent) return;

        for (int i = 0; i < tracers.Count; i++)
        {
            tracers[i].transform.SetPositionAndRotation(renderers[i].transform.position, renderers[i].transform.rotation);
            tracers[i].transform.localScale = renderers[i].transform.lossyScale;
        }
    }

    // 【中文注释】复制只供痕迹相机观察的代理，让正常材质与痕迹深度职责分离。
    private void SetupTracers()
    {
        tracers = new List<Transform>();

        // 【中文注释】vertexPush 沿法线膨胀代理，帮助薄鞋底/低模稳定地与雪面深度相交。
        Material tracerMtl = new Material(Shader.Find("NOT_Lonely/Weatherade/Extra/NL_DepthOccluder"));
        tracerMtl.name = "TracerMaterial";
        tracerMtl.SetFloat("_VertexPush", vertexPush);

        if (renderers == null || renderers.Length == 0)
        {
            renderers = new Renderer[1];
            renderers[0] = GetComponent<Renderer>();
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            GameObject tracerObj = new GameObject($"{renderers[i].name}_SRS Tracer");

            tracerObj.layer = LayerMask.NameToLayer(tracerLayerName);

            if (parent) tracerObj.transform.parent = renderers[i].transform;
            else tracers.Add(tracerObj.transform);

            tracerObj.transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;

            Material[] mtls = renderers[i].materials;
            for (int m = 0; m < mtls.Length; m++)
            {
                mtls[m] = tracerMtl;
            }

            if (renderers[i] is MeshRenderer)
            {
                MeshFilter mFilter = tracerObj.AddComponent<MeshFilter>();
                MeshRenderer mRnd = tracerObj.AddComponent<MeshRenderer>();
                mFilter.sharedMesh = renderers[i].GetComponent<MeshFilter>().sharedMesh;
                SetupRenderer(mRnd, mtls);
            }
            // 【中文注释】复用原 bones/rootBone，因此动画形变也能写入痕迹深度。
            else if(renderers[i] is SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer srcSRnd = renderers[i] as SkinnedMeshRenderer;
                SkinnedMeshRenderer sRnd = tracerObj.AddComponent<SkinnedMeshRenderer>();
                sRnd.sharedMesh = srcSRnd.sharedMesh;
                sRnd.localBounds = srcSRnd.localBounds;
                sRnd.quality = srcSRnd.quality;
                sRnd.updateWhenOffscreen = srcSRnd.updateWhenOffscreen;
                sRnd.bones = srcSRnd.bones;
                sRnd.rootBone = srcSRnd.rootBone;
                sRnd.skinnedMotionVectors = false;
                SetupRenderer(sRnd, mtls);
            }
            
            /*
            
            mRnd.materials = mtls;
            mRnd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mRnd.receiveGI = ReceiveGI.Lightmaps;
            mRnd.receiveShadows = false;
            mRnd.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            */
        }
    }

    private void SetupRenderer(Renderer rnd, Material[] mtls)
    {
        rnd.materials = mtls;
        rnd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rnd.receiveShadows = false;
        rnd.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        rnd.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
    }
}
