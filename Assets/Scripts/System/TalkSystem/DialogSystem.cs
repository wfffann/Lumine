using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("组件获取")]
    public Text textLable;
    public Image faceImage;

    public TextAsset textFile;//文本文件

    public Sprite face01, face02;//TODO:怎么寻找头像

    [Header("状态检测")]
    private bool textFinished;
    private bool cancelTyping;

    [Header("基本设置")]
    public int index;
    public float textSpeed;//文本输出速度

    List<string> textList = new List<string>();//切割的文本

    private void Awake()
    {
        GetTextFormFile(textFile);//读取文本内容
    }

    private void OnEnable()
    {
        textFinished = true;
        //读取第一行内容
        StartCoroutine(SetTextUI());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (textFinished)
            {
                StartCoroutine(SetTextUI());

                if (index == textList.Count)
                {
                    gameObject.SetActive(false);
                    index = 0;
                }
            }
            else if (!textFinished && !cancelTyping)
            {
                cancelTyping = true;
            }
        }
    }

    /// <summary>
    /// 获取Text文件的内容
    /// </summary>
    /// <param name="file"></param>
    private void GetTextFormFile(TextAsset file)
    {
        textList.Clear();//清空
        index = 0;

        var lineData = file.text.Split('\n');//按换行符切割的数组

        foreach(var line in lineData)
        {
            textList.Add(line);
        }
    }

    /// <summary>
    /// 文本逐字输出
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetTextUI()
    {
        textFinished = false;
        textLable.text = "";//清空文本框

        switch (textList[index].Trim())
        {
            case "A":
                //faceImage.sprite = face01;
                index++;
                break;

            case "B":
                //faceImage.sprite = face02;
                index++;
                break;

            default:
                break;
        }

        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length -1)
        {
            textLable.text += textList[index][letter];
            letter++;

            yield return new WaitForSeconds(textSpeed);
        }

        textLable.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }
}
