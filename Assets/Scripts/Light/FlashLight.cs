using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashLight : MonoBehaviour
{
    [Header("组件获取")]
    private Light2D flashLight;//SpotLight

    [Header("状态检测")]
    public bool isSpotLight;//是否聚光

    [Header("基本设置")]
    private float originalPointLightOuterAngle;
    private float currentPointLightOuterAngle;

    public float raduisInner;
    public float raduisOuter;

    public LayerMask layermask;

    private float timer;
    public float triggerTime;

    private void Awake()
    {
        flashLight = GetComponent<Light2D>();

        originalPointLightOuterAngle = flashLight.pointLightOuterAngle;
        currentPointLightOuterAngle = originalPointLightOuterAngle;

        raduisInner = flashLight.pointLightInnerRadius;
        raduisOuter = flashLight.pointLightOuterRadius;
    }

    private void Update()
    {
        Debug.DrawRay(this.transform.position, this.transform.up * flashLight.pointLightOuterRadius, Color.red);
        SpotLight();
        ChangeFlashLight();
        RaycastCheck();
    }

    /// <summary>
    /// 是否为聚光状态
    /// </summary>
    public void SpotLight()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            isSpotLight = !isSpotLight;
            flashLight.enabled = isSpotLight;
        }
    }

    /// <summary>
    /// 改变手电筒的焦距
    /// </summary>
    public void ChangeFlashLight()
    {
        if (!isSpotLight) return;//不是聚光状态

        flashLight.pointLightInnerAngle = 0;

        if (Input.GetKey(KeyCode.Y))
        {
            //增加扇形范围
            if(currentPointLightOuterAngle < 80f)
            {
                currentPointLightOuterAngle += 1f;

                //调整Raduis
                raduisInner -= 0.05f;
                raduisOuter -= 0.05f;
            }
        }

        if (Input.GetKey(KeyCode.I))
        {
            //缩小扇形范围
            if(currentPointLightOuterAngle > 2f)
            {
                currentPointLightOuterAngle -= 1f;

                //调整Raduis
                raduisInner += 0.05f;
                raduisOuter += 0.05f;
            }
        }

        //更新范围
        flashLight.pointLightOuterAngle = Mathf.Clamp(currentPointLightOuterAngle, 2f, 80f);
        flashLight.pointLightInnerRadius = Mathf.Clamp(raduisInner, 0f, 4f);
        flashLight.pointLightOuterRadius = Mathf.Clamp(raduisOuter, 6f, 10f);
    }

    /// <summary>
    /// 射线检测
    /// </summary>
    public void RaycastCheck()
    {
        if (!isSpotLight) return;

        //TODO:根据人物的朝向修改方向

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.up, 
            flashLight.pointLightOuterRadius);

        //检测机关
        if (hit.collider != null && hit.collider.CompareTag("Organ"))
        {
            timer += Time.deltaTime;
            if(timer >= triggerTime)
            {
                //触发机关
                Debug.Log("organs");
            }
        }

        //检测目标物体是否在聚光灯内
        if(hit.collider != null && hit.collider.CompareTag("ShadowTarget"))
        {
            Debug.Log("检测到了Target");

            //邻边向量
            Vector3 tmp_Dir_1 = new Vector3(hit.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y) 
                - this.transform.position;
            //斜边向量
            Vector3 tmp_Dir_2 = hit.collider.gameObject.transform.GetChild(0).position - this.transform.position;

            Debug.Log("计算了向量");

            if (Mathf.Cos(flashLight.pointLightOuterAngle / 2 * Mathf.Deg2Rad) <= tmp_Dir_1.magnitude / tmp_Dir_2.magnitude)
            {
                //显示阴影
                Debug.Log("显示阴影");
            }
        }

    }
}
