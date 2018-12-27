using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UILoadingPage : UIDataBase
{
    public const string NAME = "UILoadingPage";

    public override UIShowPos ShowPos
    {
        get
        {
            return UIShowPos.TipTop;
        }
    }

    public override HidePage hidePage
    {
        get
        {
            return HidePage.Destory;
        }
    }
    public override AssetFolder assetFolder
    {
        get
        {
            return AssetFolder.LuckyBoy;
        }
    }

    public void Start()
    {
        Android_Call.UnityCallAndroid(AndroidMethod.HideSplash);
    }
}
