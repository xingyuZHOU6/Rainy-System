/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\URP_RendererSetter.cs
 * 分类/优先级：URP 深度接入 / A-必读
 * 文件职责：把 SRS 专用 RendererData 追加到当前 URP Asset，并返回 Renderer Index。
 * 数据流位置：SRS_DepthRenderer.asset -> URP Asset m_RendererDataList -> 相机 SetRenderer(index)。
 * 主要 Unity 技术：C# 反射、UniversalRenderPipelineAsset、多 Renderer
 * 建议关注：使用私有字段反射且未显式持久化，升级或构建前重点验证。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class URP_RendererSetter
{
    // 【中文注释】反射修改 URP 私有 Renderer 列表；版本敏感，且要核对是否被持久化到资产。
    public static int SetWeatheradeRenderer(ScriptableRendererData rendererAsset)
    {
        int srsRendererId = -1;

        if (UniversalRenderPipeline.asset)
        {
            Type urpAsset = typeof(UniversalRenderPipelineAsset);
            FieldInfo renderersListField = urpAsset.GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])renderersListField.GetValue(UniversalRenderPipeline.asset);

            for (int i = 0; i < rendererDataList.Length; i++)
            {
                if (rendererDataList[i] == rendererAsset)
                {
                    //Debug.Log("Renderer is set already. Skip.");
                    srsRendererId = i;
                    return srsRendererId;
                }
            }

            List<ScriptableRendererData> rendDataList = rendererDataList.ToList();
            rendDataList.Add(rendererAsset);

            rendererDataList = rendDataList.ToArray();

            //Set renderers list back
            renderersListField.SetValue(UniversalRenderPipeline.asset, rendererDataList);

            srsRendererId = rendererDataList.Length - 1;
        }
        else
        {
            Debug.LogError("No Universal Render Pipeline is currently active.");
        }

        return srsRendererId;
    }
}
