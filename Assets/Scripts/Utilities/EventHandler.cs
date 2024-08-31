using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{
    //通知玩家全局光照
    public static event Action<Vector3> BaseGlobal;

    //拿起灯事件
    public static event Action<Vector3> GetupLightEvent;

    /// <summary>
    /// 激活全局光照
    /// </summary>
    /// <param name="basePos"></param>
    public static void CallBaseGlobal(Vector3 basePos)
    {
        BaseGlobal?.Invoke(basePos);
    }

    /// <summary>
    /// 拿起灯
    /// </summary>
    /// <param name="_Pos"></param>
    public static void RaisedGetupLightEvent(Vector3 _Pos)
    {
        GetupLightEvent?.Invoke(_Pos);
    }
}
