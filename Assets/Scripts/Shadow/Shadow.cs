using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [Header("组件获取")]
    private Transform playerTransform;

    [Header("基本设置")]
    private float distance;
    private float changeAmount = 0.05f;

    private void Update()
    {
        //寻找Player
        if(this.gameObject.activeInHierarchy == true)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            //改变阴影的位置
            ChangeShadowPosition();
        }
    }

    /// <summary>
    /// 根据Player的位置改变阴影的位置
    /// </summary>
    public void ChangeShadowPosition()
    {
        float tmp_Distance = (this.transform.parent.transform.GetChild(1).transform.position - playerTransform.position).magnitude;
        this.transform.localPosition = new Vector2(Mathf.Abs(tmp_Distance), -playerTransform.position.y);
    }

    /// <summary>
    /// 改变Shadow阴影的大小
    /// </summary>
    public void ChangeShadowScale()
    {

    }
}
