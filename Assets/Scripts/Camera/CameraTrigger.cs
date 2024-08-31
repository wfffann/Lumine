using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Header("组件获取")]
    public CameraController cameraController;

    [Header("基本设置")]
    public TurnDirctionTwoDir tiggerDir;
    public TurnDirctionFourDir cameraDir;

    public Vector3 smoothVelocity = Vector3.zero;
    public float smoothTime;

    private Vector3 beforePosition;
    private Vector3 afterPosition;
    public Vector3 targetPosition;

    private IEnumerator cameraMoveCoroutine;

    private void Awake()
    {
        cameraMoveCoroutine = CameraMoveCoroutinue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            beforePosition = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            afterPosition = collision.transform.position;

            TurnDirctionFourDir tmp_Dir = GetCameraTurnDirction();

            switch (tmp_Dir)
            {
                case TurnDirctionFourDir.TurnTop:
                    targetPosition = new Vector3(Camera.main.transform.position.x,
                        Camera.main.transform.position.y + cameraController.cameraHeight, Camera.main.transform.position.z);

                    BeginCoroutine();
                    break;

                case TurnDirctionFourDir.TurnBottom:
                    targetPosition = new Vector3(Camera.main.transform.position.x,
                        Camera.main.transform.position.y - cameraController.cameraHeight, Camera.main.transform.position.z);

                    BeginCoroutine();
                    break;

                case TurnDirctionFourDir.TurnLeft:
                    targetPosition = new Vector3(Camera.main.transform.position.x - cameraController.cameraWidth,
                        Camera.main.transform.position.y, Camera.main.transform.position.z);

                    BeginCoroutine();
                    break;

                case TurnDirctionFourDir.TurnRight:
                    targetPosition = new Vector3(Camera.main.transform.position.x + cameraController.cameraWidth,
                        Camera.main.transform.position.y, Camera.main.transform.position.z);

                    BeginCoroutine();
                    break;

                case TurnDirctionFourDir.None:
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 获取摄像机转向方向
    /// </summary>
    /// <returns></returns>
    public TurnDirctionFourDir GetCameraTurnDirction()
    {
        //根据当前的trigger设置
        switch (tiggerDir)
        {
            case TurnDirctionTwoDir.Horizontal:
                if(beforePosition.x < afterPosition.x)
                {
                    return TurnDirctionFourDir.TurnRight;
                }
                else
                {
                    return TurnDirctionFourDir.TurnLeft;
                }

            case TurnDirctionTwoDir.Vertical:
                if(beforePosition.y < afterPosition.y)
                {
                    return TurnDirctionFourDir.TurnTop;
                }
                else
                {
                    return TurnDirctionFourDir.TurnBottom;
                }

            default:
                return TurnDirctionFourDir.None;
        }
    }

    /// <summary>
    /// 摄像机移动的协程
    /// </summary>
    /// <param name="_turnDirction"></param>
    /// <param name="_value"></param>
    /// <returns></returns>
    public IEnumerator CameraMoveCoroutinue()
    {
        while (Mathf.Abs(Camera.main.transform.position.magnitude - targetPosition.magnitude) >= 0.001f)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPosition, ref smoothVelocity, smoothTime);
            yield return null;
        }
    }

    /// <summary>
    /// 开始协程
    /// </summary>
    public void BeginCoroutine()
    {
        if(cameraMoveCoroutine == null)
        {
            cameraMoveCoroutine = CameraMoveCoroutinue();
            StartCoroutine(cameraMoveCoroutine);
        }
        else
        {
            StopCoroutine(cameraMoveCoroutine);
            cameraMoveCoroutine = null;
            cameraMoveCoroutine = CameraMoveCoroutinue();
            StartCoroutine(cameraMoveCoroutine);
        }
    }
}


