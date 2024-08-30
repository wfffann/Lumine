using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkButton : MonoBehaviour
{
    [Header("组件获取")]
    public GameObject button;
    public GameObject talkUI;

    private DialogSystem dialogSystem;

    [Header("基本设置")]
    private string targetCharacterName;

    private void Awake()
    {
        dialogSystem = GetComponent<DialogSystem>();

        targetCharacterName = this.gameObject.name;
    }

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
            //获取当前人物的名称
            dialogSystem.targetCharacterName = targetCharacterName;

            //展示对话框
            talkUI.SetActive(true);
        }
    }
}
