using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UIDragCheckPage : UIDataBase
{
    public const string NAME = "UIDragCheckPage";
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.TipTop; }
    }
    public override HidePage hidePage
    {
        get { return HidePage.Hide; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.Common; }
    }

    private float left_X;
    private float right_X;
    private float up_Y;
    private float down_Y;

    private int tag1 = 0;
    private int tag2 = 0;

    private GameObject check;

    public override void Init()
    {
        check = CommTool.FindObjForName(gameObject, "check");
        left_X = Screen.width / 15;
        right_X = Screen.width - left_X;

        down_Y = Screen.height / 12;
        up_Y = Screen.height - down_Y;
    }

    public override void OnShow(object data)
    {
        UIEventLisener.Get(check).OnDragBegin += OnDragBegin;
        UIEventLisener.Get(check).OnDragEnd += OnDragEnd;
        tag1 = 0;
        tag2 = 0;
    }

    public override void OnHide()
    {
        UIEventLisener.Get(check).OnDragBegin -= OnDragBegin;
        UIEventLisener.Get(check).OnDragEnd -= OnDragEnd;
    }

    private void OnDragBegin(GameObject go, PointerEventData eventData)
    {
        if (tag1 == 2) tag1 = 0;
        if (tag2 == 2) tag2 = 0;
        Debug.Log("OnDragBegin***********" + eventData.position + "***Screen.width***" + Screen.width + "***Screen.Height**" + Screen.height);
        if (eventData.position.x < left_X && eventData.position.y > up_Y)//left
        {
            tag1 = 1;
        }
        if (eventData.position.x > right_X && eventData.position.y > up_Y)//right
        {
            tag2 =1;
        }
    }

    private void OnDragEnd(GameObject go, PointerEventData eventData)
    {
        Debug.Log("OnDragEnd***********" + eventData.position + "***Screen.width***" + Screen.width + "***Screen.Height**" + Screen.height);
        if (eventData.position.x < left_X && eventData.position.y < down_Y)//left
        {
            tag1 = 2;
        }
        if (eventData.position.x > right_X && eventData.position.y < down_Y)//right
        {
            tag2 = 2;
        }
        //同时符合
        if (tag1 == 2 && tag2 == 2)
        {
            Debug.Log("***下拉条件同时符合***结束游戏");
            GameCtr.Instance.CodePageGameQuit();
        }

    }
}

