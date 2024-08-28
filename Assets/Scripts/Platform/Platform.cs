using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public LightState currentPlatformState;
    private SpriteRenderer sr;
    private BoxCollider2D coll;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player Light")
        {
            var currentState = collision.GetComponent<FlashLight>().currrentLightState;
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
