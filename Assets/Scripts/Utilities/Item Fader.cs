using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//限制一定要求有渲染器组件，才能改变透明度
[RequireComponent(typeof(SpriteRenderer))]

public class ItemFader : MonoBehaviour
{
    private SpriteRenderer sr;
    public Color originColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originColor = sr.color;
    }

    /// <summary>
    /// 逐渐恢复颜色
    /// </summary>
    public bool FadeIn()
    {
        Color targetColor = originColor;
        sr.DOColor(targetColor, Settings.itemFadeDuration);
        return true;
    }

    /// <summary>
    /// 逐渐完全透明
    /// </summary>
    public bool FadeOut()
    {
        Color targetColor = originColor;
        targetColor.a = Settings.targetAlpha;
        sr.DOColor(targetColor, Settings.itemFadeDuration);
        return true;
    }
}
