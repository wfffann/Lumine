using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public LightState currentPlatformState;
    private SpriteRenderer sr;
    private BoxCollider2D coll;
    private ItemFader itemFader;

    public LayerMask lightLayer;
    public float checkRadius;
    private float currentTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        itemFader = GetComponent<ItemFader>();
    }

    private void Start()
    {
        if (currentPlatformState == LightState.LightUp)
        {
            itemFader.FadeOut();
            coll.enabled = false;
        }
        else if (currentPlatformState == LightState.LightDown)
        {
            itemFader.FadeIn();
            coll.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, checkRadius, new Vector2(0, 0), lightLayer);

        //检测光显暗显平台的显示，如果都没有检测到则正常显示
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player Light")
            {
                var currentState = hit.collider.GetComponent<LightController>().currrentLightState;
                if (currentState == currentPlatformState)
                {
                    itemFader.FadeIn();
                    coll.enabled = true;
                }
                else
                {
                    itemFader.FadeOut();
                    coll.enabled = false;
                }
                //全局光照
                if (currentState == LightState.GlobalLight)
                {
                    if (currentPlatformState == LightState.LightUp)
                    {
                        itemFader.FadeIn();
                        coll.enabled = true;
                    }
                    else
                    {
                        itemFader.FadeOut();
                        coll.enabled = false;
                    }
                }
                return;
            }
        }

        //碰撞体中没有灯光则按正常显示
        if (currentPlatformState == LightState.LightUp)
        {
            itemFader.FadeOut();
            coll.enabled = false;
        }
        else
        {
            itemFader.FadeIn();
            coll.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
