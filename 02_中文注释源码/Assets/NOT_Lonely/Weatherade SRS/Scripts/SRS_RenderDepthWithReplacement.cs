/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\SRS_RenderDepthWithReplacement.cs
 * 分类/优先级：URP 深度接入 / A-必读
 * 文件职责：URP Renderer Feature：用替换 Shader 绘制指定 Layer/Queue，Unity 6 分支同时支持 RenderGraph。
 * 数据流位置：SRS 专用相机剔除结果 -> overrideShader -> 目标深度附件。
 * 主要 Unity 技术：ScriptableRendererFeature、ScriptableRenderPass、DrawingSettings、RendererList、RTHandle、RenderGraph
 * 建议关注：对照 Unity 2022 Execute 路径与 Unity 6 RecordRenderGraph 路径。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using System.Collections.Generic;
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// 【中文注释】URP 版 Replacement Shader：专用相机统一用 DepthRenderer 绘制指定 Layer/Queue。
public class SRS_RenderDepthWithReplacement : ScriptableRendererFeature
{
    public static Dictionary<Camera, RTHandle> rtHandles;

    class CustomRenderPass : ScriptableRenderPass
    {
        private Settings settings;
        private FilteringSettings filteringSettings;
        private ProfilingSampler _profilingSampler;
        private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();
#if UNITY_6000_0_OR_NEWER
        private TextureHandle texHandle;
#endif
        private string renderPassName;

#if !UNITY_6000_0_OR_NEWER
        private RTHandle rtCustomColor, rtCameraDepth;
#endif

        public CustomRenderPass(Settings settings, string name)
        {
            this.settings = settings;

            filteringSettings = new FilteringSettings(settings.queue, settings.layerMask);

            // Use default tags
            shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
            shaderTagsList.Add(new ShaderTagId("UniversalForward"));
            shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));

            renderPassName = name;
            _profilingSampler = new ProfilingSampler(name);
        }

#if UNITY_6000_0_OR_NEWER
        private class PassData
        {
            public RendererListHandle rendererListHandle;
        }

        private void InitRendererLists(ContextContainer frameData, ref PassData passData, RenderGraph renderGraph)
        {
            UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();

            var sortFlags = cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagsList, universalRenderingData, cameraData, lightData, sortFlags);

            drawSettings.overrideShader = settings.replacementShader;
            drawSettings.overrideShaderPassIndex = 0;

            var param = new RendererListParams(universalRenderingData.cullResults, drawSettings, filteringSettings);
            passData.rendererListHandle = renderGraph.CreateRendererList(param);
        }

        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            context.cmd.DrawRendererList(data.rendererListHandle);
        }

        // 【中文注释】Unity 6：导入外部 RTHandle 为深度附件，执行 overrideShader RendererList。
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(renderPassName, out var passData, _profilingSampler))
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                InitRendererLists(frameData, ref passData, renderGraph);

                if (!passData.rendererListHandle.IsValid())
                    return;

                if (rtHandles != null)
                {
                    if (rtHandles.TryGetValue(cameraData.camera, out RTHandle RTh))
                    {
                        texHandle = renderGraph.ImportTexture(RTh);
                    }
                }

                if (texHandle.IsValid())
                    builder.SetRenderAttachmentDepth(texHandle, AccessFlags.Write);

                builder.UseRendererList(passData.rendererListHandle);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }
#else
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            rtCustomColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RTHandle rtCameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;

            ConfigureTarget(rtCustomColor, rtCameraDepth);
            ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
        }

        // 【中文注释】Unity 2022/URP14：用当前 cullResults 和 overrideShader 直接 DrawRenderers。
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagsList, ref renderingData, sortingCriteria);

                drawingSettings.overrideShader = settings.replacementShader;
                drawingSettings.overrideShaderPassIndex = 0;

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
#endif
    }

    // Exposed Settings
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;

        public LayerMask layerMask = ~0;
        public RenderQueueRange queue = RenderQueueRange.opaque;
        public Shader replacementShader; //use DepthRenderer shader
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings, name);
        m_ScriptablePass.renderPassEvent = settings._event;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        CameraType cameraType = renderingData.cameraData.cameraType;
        if (cameraType == CameraType.Preview) return; // Ignore feature for editor/inspector previews & asset thumbnails
        renderer.EnqueuePass(m_ScriptablePass);
    }
}