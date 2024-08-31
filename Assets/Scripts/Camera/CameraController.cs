using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TurnDirctionTwoDir
{
    Horizontal,
    Vertical
}

public enum TurnDirctionFourDir
{
    TurnTop,
    TurnBottom,
    TurnLeft,
    TurnRight,
    None
}

public class CameraController : MonoBehaviour
{
    [Header("组件获取")]
    public Camera camera;
    private BoxCollider2D coll;

    [Header("基本设置")]
    public TurnDirctionFourDir turnDir;
    public float cameraHeight;
    public float cameraWidth;

    private void Start()
    {
        cameraHeight = 2f * camera.orthographicSize;
        //cameraWidth = Screen.width / Screen.height * cameraHeight * 2f;//TODO:长度不对
        cameraWidth = camera.orthographicSize * Screen.width / Screen.height * 2;
        //Debug.Log("cameraWidth: + " + cameraWidth);
    }
 }
