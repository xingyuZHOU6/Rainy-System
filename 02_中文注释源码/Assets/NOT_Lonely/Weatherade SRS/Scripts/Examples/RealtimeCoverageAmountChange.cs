/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Scripts\Examples\RealtimeCoverageAmountChange.cs
 * 分类/优先级：样例控制 / C-扩展
 * 文件职责：运行时改变积雪量的最小示例。
 * 数据流位置：输入/时间 -> SnowCoverage 参数。
 * 主要 Unity 技术：MonoBehaviour、运行时参数驱动
 * 建议关注：查看公开 API 用法。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

using NOT_Lonely.Weatherade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeCoverageAmountChange : MonoBehaviour
{
    [SerializeField] private float startAmount = 0;
    [SerializeField] private float endAmount = 0.75f;

    [SerializeField] private bool changeOnStart = true;
    [SerializeField] private float changeTime = 15;

    private float changeSpeed => 1 / changeTime;

    private SnowCoverage snowCoverage;


    void Start()
    {
        snowCoverage = (SnowCoverage)CoverageBase.instance;

        if (changeOnStart)
            StartCoroutine(ChangeCoverageAmountGradually());
    }

    private IEnumerator ChangeCoverageAmountGradually()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime * changeSpeed;
            SetCoverageAmount(Mathf.Lerp(startAmount, endAmount, t));
            yield return null;
        }

        SetCoverageAmount(endAmount);
    }

    /// <summary>
    /// Set the amount of snow coverage.
    /// </summary>
    /// <param name="amount">Representation of the 'Amount' value of the Snow Coverage Instance. 0-1 range is used.</param>
    public void SetCoverageAmount(float amount)
    {
        snowCoverage.coverageAmount = amount;
        snowCoverage.UpdateCoverageMaterials();
    }
}
