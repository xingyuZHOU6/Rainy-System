/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\Extra\VectorLabelsAttribute.cs
 * 分类/优先级：共享支持 / C-扩展
 * 文件职责：被雪系统或其编辑器/示例间接使用的支持源码。
 * 数据流位置：由相邻子系统按需调用。
 * 主要 Unity 技术：Unity C#/Shader 支持代码
 * 建议关注：先看文件头与调用者。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using UnityEngine;
public class VectorLabelsAttribute : PropertyAttribute
{
    public readonly string[] Labels;

    public VectorLabelsAttribute(params string[] labels)
    {
        Labels = labels;
    }
}