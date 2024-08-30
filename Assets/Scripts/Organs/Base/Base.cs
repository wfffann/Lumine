using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Base : MonoBehaviour
{
    private bool canPlaceGlobalLight = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPlaceGlobalLight)
        {
            EventHandler.CallBaseGlobal(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            canPlaceGlobalLight = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            canPlaceGlobalLight = false;
    }
}
