/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\Extra\SRS_DataTransfer.cs
 * 分类/优先级：URP 深度接入 / B-支持
 * 文件职责：传递 Camera->RTHandle 映射给 Unity 6 RenderGraph 分支，并向 Shader 提供主相机方向。
 * 数据流位置：外部 RenderTexture/Camera -> Renderer Feature；主相机 up/right -> 全局 Shader 向量。
 * 主要 Unity 技术：RenderPipelineManager 回调、RTHandle、执行顺序
 * 建议关注：Unity 2022 主要使用相机方向，Unity 6 才需要 RTHandle 字典。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif
[DefaultExecutionOrder(-99)]
public class SRS_DataTransfer : MonoBehaviour
{
#if USING_URP
    private Dictionary<Camera, RTHandle> rtHandles;
    private List<Camera> tempCams = new List<Camera>();
    private List<RTHandle> tempHandles = new List<RTHandle>();

    private void Awake()
    {
        ResetTempLists();
    }
#endif
    private void OnEnable()
    {
#if USING_URP
        RenderPipelineManager.beginCameraRendering += OnCamPreRender;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
#endif
#else
        Camera.onPreRender += OnCamPreRender;
#endif
    }

    private void OnDisable()
    {
#if USING_URP
        RenderPipelineManager.beginCameraRendering -= OnCamPreRender;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
#endif
#else
        Camera.onPreRender -= OnCamPreRender;
#endif
    }

#if USING_URP
#if UNITY_EDITOR
    private void OnPlaymodeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
            UpdateCameraRTHandlePairs();
        else
            ResetTempLists();
    }
#endif

    private void ResetTempLists()
    {
        tempCams = new List<Camera>();
        tempHandles = new List<RTHandle>();
    }

    // 【中文注释】Unity 6 RenderGraph 通过显式 Camera->RTHandle 映射导入外部目标。
    public void UpdateCameraRTHandlePairs()
    {
        SRS_RenderDepthWithReplacement.rtHandles = new Dictionary<Camera, RTHandle>();
        rtHandles = new Dictionary<Camera, RTHandle>();

        for (int i = 0; i < tempCams.Count; i++)
        {
            rtHandles.Add(tempCams[i], tempHandles[i]);
        }

        SRS_RenderDepthWithReplacement.rtHandles = rtHandles;
    }

    public void AddCameraRTHandlePair(Camera cam, RTHandle rtHandle)
    {
        if (tempCams.Count != tempHandles.Count) ResetTempLists(); //reset lists if their counts are different

        if (!tempCams.Contains(cam))
        {
            tempCams.Add(cam);
            tempHandles.Add(rtHandle);
        }
    }

    private void OnCamPreRender(ScriptableRenderContext context, Camera cam)
#else
    private void OnCamPreRender(Camera cam)
#endif
    {
        if ((Application.isPlaying && cam.tag == "MainCamera") || !Application.isPlaying)
        {
            Shader.SetGlobalVector("_viewCamUp", cam.transform.up);
            Shader.SetGlobalVector("_viewCamRight", cam.transform.right);
        }
    }
}
