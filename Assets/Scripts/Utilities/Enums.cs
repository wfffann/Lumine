using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightState
{
    LightDown, //灯关闭
    LightUp, //灯正常状态
    SpotLight, //灯聚焦状态
    GlobalLight,
    //待添加
}

public enum PlatformState
{
    noMove, //不动
    horizontalMove, //水平运动
    verticalMove, //竖直运动
}

public enum PlatformMoveDir
{
    Up, Down, Left, Right, No
}
