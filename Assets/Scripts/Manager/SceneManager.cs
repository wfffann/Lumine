using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    private void Start()
    {
        EnterMenu();
    }

    /// <summary>
    /// 进入游戏菜单
    /// </summary>
    public void EnterMenu()
    {
        //加载菜单场景
    }

    /// <summary>
    /// 加载下一个场景
    /// </summary>
    public void LoadNextScene()
    {

    }

    /// <summary>
    /// 正在加载场景中
    /// </summary>
    public void OnLoadNextScene()
    {

    }

    /// <summary>
    /// 场景加载结束
    /// </summary>
    public void AfterLoadScene()
    {

    }
}
