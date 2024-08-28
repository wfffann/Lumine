using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;

public class FlashLight : MonoBehaviour
{
    [Header("组件获取")]
    private Light2D flashLight;//SpotLight
    private CircleCollider2D coll;
    public ShadowTargetData shadowTargetData;

    [Header("状态检测")]
    public LightState currrentLightState = LightState.LightDown; //是否聚光

    [Header("基本设置")]
    private float originalPointLightOuterAngle; //原始外圈角度
    private float currentPointLightOuterAngle; //当前外圈角度
    private float originalPointLightInnerAngle; //原始内圈角度
    private float currentPointLightInnerAngle; //当前内圈角度


    public float raduisInner;
    public float raduisOuter;

    public LayerMask shadow;

    private float timer;
    public float triggerTime;

    private List<GameObject> currentSceneShadowList = new List<GameObject>();//当前场景的阴影集合


    private void Awake()
    {
        flashLight = GetComponent<Light2D>();
        coll = GetComponent<CircleCollider2D>();

        originalPointLightOuterAngle = flashLight.pointLightOuterAngle;
        currentPointLightOuterAngle = originalPointLightOuterAngle;

        raduisInner = flashLight.pointLightInnerRadius;
        raduisOuter = flashLight.pointLightOuterRadius;

        //关闭所有影子
        CloseAllShadow();
        flashLight.enabled = false;
        coll.enabled = false;
    }

    private void Update()
    {
        //Debug.DrawRay(this.transform.position, this.transform.up * flashLight.pointLightOuterRadius, Color.red);

        //Quaternion quaternion_Top1 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_1 = quaternion_Top1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //Debug.DrawRay(this.transform.position, tmp_Dir_Top_1, Color.green);

        //Quaternion quaternion_Top2 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_2 = quaternion_Top2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //Debug.DrawRay(this.transform.position, tmp_Dir_Top_2, Color.green);

        //Quaternion quaternion_Top3 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8 * 3 , new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_3 = quaternion_Top3 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //Debug.DrawRay(this.transform.position, tmp_Dir_Top_3, Color.green);


        Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
        Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_1, Color.red);

        Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
        Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_2, Color.red);

        //Quaternion quaternion_Bottom_3 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8 * 3, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Bottom_3 = quaternion_Bottom_3 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_3, Color.red);

        CheckLightState();
        ChangeFlashLight();
        RaycastCheck();
        ChangeLightUp(); 
    }

    /// <summary>
    /// 检查具体的灯光状态
    /// </summary>
    public void CheckLightState()
    {
        //开关聚光状态
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (currrentLightState == LightState.LightDown)
                currrentLightState = LightState.SpotLight;
            else if (currrentLightState == LightState.SpotLight)
                currrentLightState = LightState.LightDown;

            if (currrentLightState == LightState.LightDown)
            {
                //关闭所有影子
                CloseAllShadow();
                flashLight.enabled = false;
                coll.enabled = true;
            }
            else
            {
                flashLight.enabled = true;
                coll.enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (currrentLightState == LightState.LightDown)
                currrentLightState = LightState.LightUp;
            else if (currrentLightState == LightState.LightUp)
                currrentLightState = LightState.LightDown;

            if (currrentLightState == LightState.LightDown)
            {
                //关闭所有影子
                CloseAllShadow();
                flashLight.enabled = false;
                coll.enabled = true;
            }
            else
            {
                flashLight.enabled = true;
                coll.enabled = true;
            }
        }
    }

    #region 灯光聚焦状态
    /// <summary>
    /// 改变手电筒的焦距
    /// </summary>
    public void ChangeFlashLight()
    {
        if (currrentLightState != LightState.SpotLight) return;//不是聚光状态

        flashLight.pointLightInnerAngle = 0;

        if (Input.GetKey(KeyCode.Y))
        {
            //增加扇形范围
            if (currentPointLightOuterAngle < 80f)
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
            if (currentPointLightOuterAngle > 2f)
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
        if (currrentLightState != LightState.SpotLight) return;

        //TODO:根据人物的朝向修改方向

        RaycastHit2D hit_Center = Physics2D.Raycast(this.transform.position, this.transform.up,
            flashLight.pointLightOuterRadius, shadow);

        //Quaternion quaternion_Top_1 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_1 = quaternion_Top_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //RaycastHit2D hit_Top_1 = Physics2D.Raycast(this.transform.position, tmp_Dir_Top_1.normalized, 
        //    flashLight.pointLightOuterRadius);

        //Quaternion quaternion_Top_2 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_2 = quaternion_Top_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //RaycastHit2D hit_Top_2 = Physics2D.Raycast(this.transform.position, tmp_Dir_Top_2.normalized, 
        //    flashLight.pointLightOuterRadius);

        //Quaternion quaternion_Top_3 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8 * 3, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Top_3 = quaternion_Top_3 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //RaycastHit2D hit_Top_3 = Physics2D.Raycast(this.transform.position, tmp_Dir_Top_3.normalized, 
        //    flashLight.pointLightOuterRadius);

        Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
        Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        RaycastHit2D hit_Bottom_1 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_1.normalized,
            flashLight.pointLightOuterRadius, shadow);

        Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
        Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        RaycastHit2D hit_Bottom_2 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_2.normalized,
            flashLight.pointLightOuterRadius, shadow);

        //Quaternion quaternion_Bottom_3 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8 * 3, new Vector3(0, 0, 1));
        //Vector2 tmp_Dir_Bottom_3 = quaternion_Bottom_3 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
        //RaycastHit2D hit_Bottom_3 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_3.normalized, 
        //    flashLight.pointLightOuterRadius);

        //检测机关
        if (hit_Center.collider != null && hit_Center.collider.CompareTag("Organ") && currrentLightState == LightState.SpotLight)
        {
            timer += Time.deltaTime;
            if (timer >= triggerTime)
            {
                //触发机关
                Debug.Log("organs");
            }
        }

        //检测目标物体是否在聚光灯内(生成阴影Shadow
        //if(hit_Center.collider != null && hit_Center.collider.CompareTag("ShadowTarget"))

        //if (hit_Center.collider != null && hit_Center.collider.CompareTag("ShadowTarget") ||
        //   hit_Bottom_1.collider != null && hit_Bottom_1.collider.CompareTag("ShadowTarget") ||
        //   hit_Bottom_2.collider != null && hit_Bottom_2.collider.CompareTag("ShadowTarget") ||
        //   hit_Bottom_3.collider != null && hit_Bottom_3.collider.CompareTag("ShadowTarget"))
        //{
        //    //邻边距离
        //    //float tmp_Distance = (new Vector3(hit.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y)
        //    //    - this.transform.position).magnitude;

        //    //Debug.Log("tmp_Distance: " + tmp_Distance);

        //    //邻边向量
        //    Vector3 tmp_Dir_1 = new Vector3(hit_Center.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y) 
        //        - this.transform.position;
        //    //斜边向量
        //    Vector3 tmp_Dir_2 = hit_Center.collider.gameObject.transform.GetChild(0).position - this.transform.position;

        //    if (Mathf.Cos(flashLight.pointLightOuterAngle / 2 * Mathf.Deg2Rad) <= tmp_Dir_1.magnitude / tmp_Dir_2.magnitude)
        //    {
        //        //显示阴影
        //        if(hit_Center.collider.gameObject.transform.childCount == 3 && 
        //            hit_Center.collider.gameObject.transform.GetChild(2).GetComponent<Shadow>() != null)
        //        {

        //            hit_Center.collider.transform.GetChild(2).gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            //创建
        //            CreateShadow(hit_Center.collider.gameObject);
        //        }
        //    }
        //    //不在生成影子范围内
        //    else
        //    {
        //        //Debug.Log("隐藏阴影");
        //        if(hit_Center.collider.gameObject.transform.childCount == 3 && 
        //            hit_Center.collider.gameObject.transform.GetChild(2).GetComponent<Shadow>() != null)
        //        {
        //            Debug.Log("隐藏阴影");

        //            hit_Center.collider.transform.GetChild(2).gameObject.SetActive(false);
        //        }
        //    }
        //}
        //else
        //{
        //    for(int i = 0; i < currentSceneShadowList.Count; i++)
        //    {
        //        currentSceneShadowList[i].gameObject.SetActive(false);
        //    }
        //}
        //}

        if (hit_Center.collider != null && hit_Center.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            AboutShadow(hit_Center);
        }
        else if (hit_Bottom_1.collider != null && hit_Bottom_1.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            AboutShadow(hit_Bottom_1);
        }
        else if (hit_Bottom_2.collider != null && hit_Bottom_2.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            AboutShadow(hit_Bottom_2);
        }
        //else if (hit_Bottom_3.collider != null && hit_Bottom_3.collider.CompareTag("ShadowTarget"))
        //{
        //    AboutShadow(hit_Bottom_3);
        //}
        else
        {
            //关闭所有阴影
            CloseAllShadow();
        }
    }

    public void AboutShadow(RaycastHit2D hit)
    {
        //邻边距离
        //float tmp_Distance = (new Vector3(hit.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y)
        //    - this.transform.position).magnitude;

        //Debug.Log("tmp_Distance: " + tmp_Distance);

        //邻边向量
        Vector3 tmp_Dir_1 = new Vector3(hit.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y)
            - this.transform.position;
        //斜边向量
        Vector3 tmp_Dir_2 = hit.collider.gameObject.transform.GetChild(0).position - this.transform.position;

        if (Mathf.Cos(flashLight.pointLightOuterAngle / 2 * Mathf.Deg2Rad) <= tmp_Dir_1.magnitude / tmp_Dir_2.magnitude)
        {
            //显示阴影
            if (hit.collider.gameObject.transform.childCount == 3 &&
                hit.collider.gameObject.transform.GetChild(2).GetComponent<Shadow>() != null)
            {

                hit.collider.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                //创建
                CreateShadow(hit.collider.gameObject);
            }
        }
        //不在生成影子范围内
        else
        {
            //Debug.Log("隐藏阴影");
            if (hit.collider.gameObject.transform.childCount == 3 &&
                hit.collider.gameObject.transform.GetChild(2).GetComponent<Shadow>() != null)
            {
                Debug.Log("隐藏阴影");

                hit.collider.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 生成当前物体的影子
    /// </summary>
    public void CreateShadow(GameObject _shadowTargetGameObject)
    {
        for (int i = 0; i < shadowTargetData.shadowTargetsList.Count; i++)
        {
            //匹配名称
            if (_shadowTargetGameObject.name == shadowTargetData.shadowTargetsList[i].shadowTargetName)
            {
                //计算光源与ShadowTarget的距离
                float tmp_Distance = Vector2.Distance(this.transform.position,
                    _shadowTargetGameObject.transform.GetChild(1).transform.position);

                //Shadow初始位置(TODO:这个位置还需要修改
                Vector2 tmp_ShadowBornPosition = new Vector2(Mathf.Abs(tmp_Distance), -this.transform.position.y);

                //实例化生成Shadow
                GameObject tmp_Shadow = GameObject.Instantiate(shadowTargetData.shadowTargetsList[i].shadowTargetPrefab,
                    _shadowTargetGameObject.transform);

                //添加进阴影的集合
                currentSceneShadowList.Add(tmp_Shadow);

                tmp_Shadow.transform.localPosition = tmp_ShadowBornPosition;

                //添加Shadow脚本
                //Shadow tmp_shadow = tmp_Obj.AddComponent<Shadow>(); 
                tmp_Shadow.AddComponent<Shadow>();
            }
        }
    }

    /// <summary>
    /// 关闭所有的阴影
    /// </summary>
    public void CloseAllShadow()
    {
        for (int i = 0; i < currentSceneShadowList.Count; i++)
        {
            currentSceneShadowList[i].gameObject.SetActive(false);
        }
    }
    #endregion

    #region 普通灯光状态
    public void ChangeLightUp()
    {
        if (currrentLightState == LightState.LightUp)
        {
            flashLight.pointLightOuterAngle = 360;
            flashLight.pointLightInnerAngle = 360;
        }
    }
    #endregion
}
