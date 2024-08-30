using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkButton : MonoBehaviour
{
    public GameObject button;
    public GameObject talkUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        button.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        button.SetActive(false);
    }

    private void Update()
    {
        if(button.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            //展示对话框
            talkUI.SetActive(true);
        }
    }
}
