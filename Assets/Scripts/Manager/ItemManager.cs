using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [Header("组件获取")]
    public Animator animator;

    [Header("基本设置")]
    public List<Item> currentItemsList = new List<Item>();//已收集的道具集合
    public List<Item> allItemsList = new List<Item>();//总道具的集合

    private int currentIndex;
    private Item currentItem;//当前装备的道具

    /// <summary>
    /// 切换道具
    /// </summary>
    public void SwitchItems()
    {
        //切换道具按键检测
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //索引的判断

            //下一个道具

            //隐藏当前道具 

            //赋值当前索引的道具
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //索引的判断

            //上一个道具

            //隐藏当前道具 

            //赋值当前索引的道具
        }

        //装备当前的道具
        //EquippedItem();
    }

    /// <summary>
    /// 获取道具
    /// </summary>
    public void GetItem(Item _item)
    {
        currentItemsList.Add(_item);

        //修改道具栏UI

        //装配获取的道具

    }

    /// <summary>
    /// 装备当前的道具
    /// </summary>
    public void EquippedItem(Item _item)
    {
        //装备当前道具的动画机
    }

}
