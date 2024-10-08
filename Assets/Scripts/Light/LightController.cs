using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    [Header("组件获取")]
    private Light2D flashLight;//SpotLight
    private CircleCollider2D coll;
    public ShadowTargetData shadowTargetData;
    private Transform playerTransform;
    public Player player;

    [Header("状态检测")]
    public LightState currrentLightState = LightState.LightDown; //灯光状态

    [Header("基本设置")]
    public float originalSpotLightOuterAngle; //原始聚光状态的外圈角度
    private float currentPointLightOuterAngle; //当前外圈角度
    private float currentPointLightInnerAngle; //当前内圈角度
    private float originalCollRaius;

    [Header("全局光照设置")]
    public float globalMaxRadius;
    public float radiusIncreaseSpeed;
    [SerializeField] private float switchTimeCounter;
    public float switchTime;
    private List<ShadowCaster2D> allShadowCaster = new List<ShadowCaster2D>();

    [Header("影子设置")]
    public float raduisInner;
    public float raduisOuter;

    public LayerMask shadow; //给影子添加图层，避免检测到其它图层

    private float timer;
    public float triggerTime;

    private List<GameObject> currentSceneShadowList = new List<GameObject>();//当前场景的阴影集合


    private void Awake()
    {
        flashLight = GetComponent<Light2D>();
        coll = GetComponent<CircleCollider2D>();

        currentPointLightOuterAngle = originalSpotLightOuterAngle;//赋值聚光灯的初始角度

        raduisInner = flashLight.pointLightInnerRadius;
        raduisOuter = flashLight.pointLightOuterRadius;

        //关闭所有影子
        CloseAllShadow();
        flashLight.enabled = false;
        originalCollRaius = coll.radius;
    }

    private void OnEnable()
    {
        EventHandler.BaseGlobal += PlacedLightOnBase;
        EventHandler.GetupLightEvent += GetupLightFromBase;
    }

    private void OnDisable()
    {
        EventHandler.BaseGlobal -= PlacedLightOnBase;
        EventHandler.GetupLightEvent -= GetupLightFromBase;
    }

    private void Update()
    {
        //绘制辅助线
        DrawHelpLine();

        //检查灯光状态；
        SwitchLightState();
        
        //聚光灯的焦距改变
        ChangeSpotLight();
        
        //聚光状态的射线检测
        RaycastCheck();

        //普通光
        ChangeLightUp();

        //全局光
        ChangeGlobalLight();

        //灯光跟随
        LightFollow();
    }

    private void LightFollow()
    {
        if (currrentLightState == LightState.LightDown || currrentLightState == LightState.LightUp || currrentLightState == LightState.SpotLight)
        {
            transform.position = player.lightPos.position;
        }
    }

    /// <summary>
    /// 绘制辅助线
    /// </summary>
    private void DrawHelpLine()
    {
        if (player.facingDir == 1)
        {
            Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_1, Color.red);

            Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_2, Color.red);
        }
        else
        {
            Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_1, Color.red);

            Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            Debug.DrawRay(this.transform.position, tmp_Dir_Bottom_2, Color.red);
        }
    }

    /// <summary>
    /// 检查具体的灯光状态
    /// </summary>
    public void SwitchLightState()
    {
        //开关聚光状态
        if (Input.GetKeyDown(KeyCode.U))
        {
            switch (currrentLightState)
            {
                case LightState.LightDown:
                    currrentLightState = LightState.SpotLight;
                    flashLight.enabled = true;
                    coll.enabled = false;
                    break;

                case LightState.LightUp:
                    currrentLightState = LightState.SpotLight;
                    flashLight.enabled = true;
                    coll.enabled = false;
                    break;

                case LightState.SpotLight:
                    currrentLightState = LightState.LightDown;
                    //关闭所有影子
                    CloseAllShadow();
                    flashLight.enabled = false;
                    coll.enabled = true;

                    break;

                case LightState.GlobalLight:
                    break;

                default:
                    break;
            }
        }
        //开开起正常灯状态
        else if (Input.GetKeyDown(KeyCode.V))
        {
            switch (currrentLightState)
            {
                case LightState.LightDown:
                    currrentLightState = LightState.LightUp;
                    flashLight.enabled = true;
                    coll.enabled = true;
                    break;

                case LightState.LightUp:
                    currrentLightState = LightState.LightDown;
                    //关闭所有影子
                    CloseAllShadow();
                    flashLight.enabled = false;
                    coll.enabled = true;
                    break;

                case LightState.SpotLight:
                    currrentLightState = LightState.LightUp;
                    flashLight.enabled = true;
                    coll.enabled = true;
                    break;

                case LightState.GlobalLight:
                    break;

                default:
                    break;
            }
        }
    }

    #region 灯光聚焦状态
    /// <summary>
    /// 改变手电筒的焦距
    /// </summary>
    public void ChangeSpotLight()
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

        //中心射出的射线
        RaycastHit2D hit_Center = Physics2D.Raycast(this.transform.position, this.transform.up,
            flashLight.pointLightOuterRadius, shadow);

        //向下的两条射线
        RaycastHit2D hit_Bottom_1;
        RaycastHit2D hit_Bottom_2;
        if (player.facingDir == 1)
        {
            Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            hit_Bottom_1 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_1.normalized,
                flashLight.pointLightOuterRadius, shadow);

            Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(-flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            hit_Bottom_2 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_2.normalized,
                flashLight.pointLightOuterRadius, shadow);
        }
        else
        {
            Quaternion quaternion_Bottom_1 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 4, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_1 = quaternion_Bottom_1 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            hit_Bottom_1 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_1.normalized,
                flashLight.pointLightOuterRadius, shadow);

            Quaternion quaternion_Bottom_2 = Quaternion.AngleAxis(flashLight.pointLightOuterAngle / 8, new Vector3(0, 0, 1));
            Vector2 tmp_Dir_Bottom_2 = quaternion_Bottom_2 * this.transform.up * flashLight.pointLightOuterRadius;//旋转后的射线
            hit_Bottom_2 = Physics2D.Raycast(this.transform.position, tmp_Dir_Bottom_2.normalized,
                flashLight.pointLightOuterRadius, shadow);
        }

        //检测机关
        if (hit_Center.collider != null && hit_Center.collider.CompareTag("Organ") && currrentLightState == LightState.SpotLight)
        {
            timer += Time.deltaTime;
            if (timer >= triggerTime)
            {
                //触发机关(TODO:
                Debug.Log("organs");
            }
        }

        //如果有一条射线检测到
        if (hit_Center.collider != null && hit_Center.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            ShadowActivities(hit_Center);
        }
        else if (hit_Bottom_1.collider != null && hit_Bottom_1.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            ShadowActivities(hit_Bottom_1);
        }
        else if (hit_Bottom_2.collider != null && hit_Bottom_2.collider.CompareTag("ShadowTarget") && currrentLightState == LightState.SpotLight)
        {
            ShadowActivities(hit_Bottom_2);
        }
        //没有射线检测到
        else
        {
            //关闭所有阴影
            CloseAllShadow();
        }
    }

    /// <summary>
    /// 关于阴影的显示或者隐藏
    /// </summary>
    /// <param name="hit"></param>
    public void ShadowActivities(RaycastHit2D hit)
    {
        Vector3 tmp_Dir_1;
        Vector3 tmp_Dir_2;
        if (player.facingDir == 1)
        {
            tmp_Dir_1 = new Vector3(hit.collider.gameObject.transform.GetChild(0).position.x, this.transform.position.y)
                - this.transform.position;

            tmp_Dir_2 = hit.collider.gameObject.transform.GetChild(0).position - this.transform.position;
        }
        else
        {
            tmp_Dir_1 = new Vector3(hit.collider.gameObject.transform.GetChild(1).position.x, this.transform.position.y)
                - this.transform.position;

            tmp_Dir_2 = hit.collider.gameObject.transform.GetChild(1).position - this.transform.position;
        }

        if (Mathf.Cos(flashLight.pointLightOuterAngle / 2 * Mathf.Deg2Rad) <= tmp_Dir_1.magnitude / tmp_Dir_2.magnitude)
        {
            //显示阴影
            if (hit.collider.gameObject.transform.childCount == 4 &&
                hit.collider.gameObject.transform.GetChild(3).GetComponent<Shadow>() != null)
            {

                hit.collider.transform.GetChild(3).transform.GetComponent<Shadow>().ChangeShadowPosition();
                hit.collider.transform.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                //创建阴影
                CreateShadow(hit.collider.gameObject);
            }
        }
        //不在生成影子范围内
        else
        {
            if (hit.collider.gameObject.transform.childCount == 4 &&
                hit.collider.gameObject.transform.GetChild(3).GetComponent<Shadow>() != null)
            {
                hit.collider.transform.GetChild(3).gameObject.SetActive(false);
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

                //找到Player
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

                //计算Player与物体的距离
                float distance = (this.transform.parent.transform.GetChild(1).transform.position - playerTransform.position).magnitude;

                Vector2 tmp_ShadowBornPosition;
                //Shadow初始位置
                if (playerTransform.position.x <= this.transform.position.x)
                {
                    tmp_ShadowBornPosition = new Vector2(Mathf.Abs(tmp_Distance), -this.transform.position.y);
                }
                else
                {
                    tmp_ShadowBornPosition = new Vector2(-Mathf.Abs(tmp_Distance), -this.transform.position.y);
                }

                //实例化生成Shadow
                GameObject tmp_Shadow = GameObject.Instantiate(shadowTargetData.shadowTargetsList[i].shadowTargetPrefab,
                    _shadowTargetGameObject.transform);

                //添加进阴影的集合
                currentSceneShadowList.Add(tmp_Shadow);

                tmp_Shadow.transform.localPosition = tmp_ShadowBornPosition;

                //添加Shadow脚本
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
            flashLight.pointLightOuterRadius = 6;
            flashLight.pointLightOuterAngle = 360;
            flashLight.pointLightInnerAngle = 360;

            //将碰撞体也设置为正常状态
            coll.radius = originalCollRaius;

            //将场景内所有的地形的阴影遮挡开启
            if (allShadowCaster.Count > 0)
            {
                foreach (var sc in allShadowCaster)
                {
                    if (sc != null && sc.enabled == false)
                        sc.enabled = true;
                }
            }
        }
    }
    #endregion

    #region 全局光照状态

    /// <summary>
    /// 灯光放到基座上进行全局光照
    /// </summary>
    /// <param name="vector"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void PlacedLightOnBase(Vector3 basePos)
    {
        if (currrentLightState == LightState.GlobalLight || currrentLightState == LightState.LightDown || currrentLightState == LightState.SpotLight)
        {
            currrentLightState = LightState.LightUp;
            return;
        }
        currrentLightState = LightState.GlobalLight;
        transform.position = basePos + new Vector3(0, 0.7f, 0);
        flashLight.enabled = true;

        transform.SetParent(GameObject.Find("PlayerController").transform, true);
    }
    public void ChangeGlobalLight()
    {
        if(currrentLightState == LightState.GlobalLight)
        {
            if(flashLight.pointLightOuterRadius < globalMaxRadius)
            {
                flashLight.pointLightOuterRadius += radiusIncreaseSpeed * Time.deltaTime;
                coll.radius += radiusIncreaseSpeed * Time.deltaTime * 3f;
                return;
            }

            //放大到全局光，开始闪烁
            switchTimeCounter += Time.deltaTime;
            if(switchTimeCounter > switchTime)
            {
                switchTimeCounter = 0;
                flashLight.enabled = !flashLight.enabled;
                coll.enabled = !coll.enabled;
            }
        }
    }

    /// <summary>
    /// 从基座上捡起灯
    /// </summary>
    public void GetupLightFromBase(Vector3 _Pos)
    {
        transform.SetParent(player.transform, true);
        transform.position = _Pos;
        currrentLightState = LightState.LightUp;
        flashLight.enabled = true;
        coll.enabled = true;
    }

    /// <summary>
    /// 全局光照下状态下，关闭物体产生阴影的组件
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Plane" && currrentLightState == LightState.GlobalLight)
        {
            var sc = collision.GetComponent<ShadowCaster2D>();
            allShadowCaster.Add(sc);
            sc.enabled = false;
        }
    }
    #endregion 
}
