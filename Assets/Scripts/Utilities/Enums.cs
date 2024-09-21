using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightState
{
    LightDown, //�ƹر�
    LightUp, //������״̬
    SpotLight, //�ƾ۽�״̬
    GlobalLight,
    //�����
}

public enum PlatformState
{
    noMove, //����
    horizontalMove, //ˮƽ�˶�
    verticalMove, //��ֱ�˶�
}

public enum PlatformMoveDir
{
    Up, Down, Left, Right, No
}
