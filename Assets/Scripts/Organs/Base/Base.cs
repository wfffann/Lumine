using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("组件获取")]
    public Player player;

    [Header("状态检测")]
    private bool canPlaceGlobalLight = false;
    public bool isPlaced;//是否放置灯

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPlaceGlobalLight && !isPlaced)
        {
            isPlaced = true;
            EventHandler.CallBaseGlobal(transform.position);
        }
        else if(Input.GetKeyDown(KeyCode.F) && canPlaceGlobalLight && isPlaced)
        {
            isPlaced = false;
            EventHandler.RaisedGetupLightEvent(player.lightPos.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            player = collision.GetComponent<Player>();
            canPlaceGlobalLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            player = collision.GetComponent<Player>();
            canPlaceGlobalLight = false;
        }
    }
}
