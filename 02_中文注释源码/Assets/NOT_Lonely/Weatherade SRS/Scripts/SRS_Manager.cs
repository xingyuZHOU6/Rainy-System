/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\SRS_Manager.cs
 * 分类/优先级：共享运行时状态 / B-支持
 * 文件职责：保存 DataTransfer、覆盖材质和灯光列表的静态入口。
 * 数据流位置：各子系统 -> 共享引用。
 * 主要 Unity 技术：静态服务定位、运行时 GameObject 创建
 * 建议关注：理解它如何按需创建 SRS_DataTransfer 即可。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using NOT_Lonely.Weatherade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRS_Manager
{
    public static SRS_DataTransfer srs_dataTransfer;
    public static List<Material> coverageMaterials = new List<Material>();
    public static List<Light> pointLights = new List<Light>();
    public static List<Light> spotLights = new List<Light>();

    public static void InitDataTransfer()
    {
        if (srs_dataTransfer == null) srs_dataTransfer = NL_Utilities.FindObjectOfType<SRS_DataTransfer>(true);
        if (srs_dataTransfer == null) srs_dataTransfer = new GameObject("SRS_Data Transfer").AddComponent<SRS_DataTransfer>();
    }
}
