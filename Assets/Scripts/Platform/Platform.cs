using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public LightState currentPlatformState;
    private SpriteRenderer sr;
    private BoxCollider2D coll;
    public float checkRadius;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (currentPlatformState == LightState.LightUp)
        {
            sr.enabled = false;
            coll.enabled = false;
        }
        else if (currentPlatformState == LightState.LightDown)
        {
            sr.enabled = true;
            coll.enabled = true;    
        }
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, checkRadius, new Vector2(1f, 0));
        foreach (var hit in hits)
        {
            if (hit.collider == null)
            {
                if (currentPlatformState == LightState.LightUp)
                {
                    sr.enabled = false;
                    coll.enabled = false;
                }
                else
                {
                    sr.enabled = true;
                    coll.enabled = true;
                }
                return;
            }
            if (hit.collider.tag == "Player Light")
            {
                var currentState = hit.collider.GetComponent<FlashLight>().currrentLightState;
                if (currentState == currentPlatformState)
                {
                    sr.enabled = true;
                    coll.enabled = true;
                }
                else
                {
                    sr.enabled = false;
                    coll.enabled = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
