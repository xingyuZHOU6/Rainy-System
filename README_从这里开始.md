# 可交互雪源码研读包

这是一份从当前项目重新扫描、按引用链整理出的学习副本。原项目文件没有被修改。

项目基线：Unity 2022.3.62f2c1、Universal Render Pipeline 14.0.12、脚本宏 `WEATHERADE_INCLUDED;USING_URP`。

## 目录怎么用

- `01_原始源码`：原样复制的源码与对应 `.meta`，保留 `Assets/...` 目录结构，适合与项目逐字对照。
- `02_中文注释源码`：同一批源码的学习副本。每个文件都有中文职责说明，核心文件还加入了关键流程的行内注释；没有修改执行逻辑。
- `03_场景与配置参考`：样例场景、材质、Prefab、URP Renderer、ProjectSettings、Amplify Shader Function 等可读配置。它们不是核心代码，但能解释 Inspector 参数和 GUID 引用。
- `04_索引与校验`：完整 CSV/Markdown 文件清单、源文件 SHA-256 与复制统计。
- 根目录的几份中文文档：建议先读 `技术原理详解.md`，再按 `快速阅读路线.md` 进入源码。

## 先记住一句话

它不是“把脚印画到雪材质”这么简单，而是：

1. 用顶视正交相机记录环境深度，得到哪里能积雪；
2. 用一上一下两台正交相机分别记录雪面和交互物代理的深度；
3. 比较深度并与上一帧痕迹合成，形成有历史的压痕高度场；
4. 从高度场重建法线并打包成 `_SRS_TraceTex`；
5. 雪 Shader 在像素阶段用它改变颜色和法线，在顶点/曲面细分阶段用它压低几何。

## 最小必读集合

按这个顺序读，约 10 个文件就能抓住主线：

1. `CoverageBase.cs`
2. `SRS_RenderDepthWithReplacement.cs`
3. `TexturePacking.shader`
4. `SRS_CoverageCommon.hlsl`
5. `SRS_Tracer.cs`
6. `SRS_TraceMaskGenerator.cs`
7. `TraceMaskGen.shader`
8. `SnowCoverage.cs`
9. `SRS_SnowCoverage.hlsl`
10. `SnowCoverage (Tessellation).shader` 或 Terrain 对应版本

## “相关源码”的边界

为了不漏掉真实依赖，本包包含三层：

- 核心：积雪遮挡、交互痕迹、位移、雪材质、URP 深度 Renderer。
- 直接支持：Terrain Pass、光照、三平面采样、Inspector/ShaderGUI、工具类、示例交互脚本。
- 邻接可选：GPU 飘雪粒子、TotalBrush 可绘制覆盖、BatchShaderSwapper 材质迁移工具、Built-in 管线兼容实现。它们不负责运行时脚印生成，但与完整雪效果、遮罩创作或已有材质接入直接相关。

纯雨水、体积灯和启动页代码已排除。Unity/URP 包本身的官方源码没有重复复制，版本固定在 `Packages/manifest.json` 中。

共享 Terrain/Pass 文件里能看到对 `SRS_RainCoverage.hlsl/.cginc` 的条件 Include；它只在 `SRS_RAIN_COVERAGE_SHADER` 分支生效，雪 Shader 定义的是 `SRS_SNOW_COVERAGE_SHADER`，因此两份纯雨水实现未计入本雪源码包。

## 重要说明

- `02_中文注释源码` 主要用于阅读；若要回灌 Unity，请优先使用 `01_原始源码`，避免和原项目中的同名类、Shader、GUID 冲突。
- Shader 文件尾部常有 Amplify Shader Editor 的节点序列化数据。真正执行的代码通常在文件前半段；节点数据用于图形化编辑器恢复图表。
- 纹理、模型和音频等大体积二进制资源没有整包复制；本包保留其导入 `.meta`、材质/场景引用和通道说明，源码研究不受影响。
- 原资产及源码版权仍归原作者；本包只是用户本机项目的研读副本。
