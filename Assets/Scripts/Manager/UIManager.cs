using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("组件获取")]
    public Transform itemsCollectTransform;//物品的收集栏

    //[Header("基本设置")]

    
    /// <summary>
    /// 设置
    /// </summary>
    public void SettingUI()
    {
        //停止游戏
        Time.timeScale = 0f;

        //弹出SettingUI
    }

    /// <summary>
    /// 关系设置
    /// </summary>
    public void CloseSettingUI()
    {
        //恢复游戏
        Time.timeScale = 1f;
    }
}
