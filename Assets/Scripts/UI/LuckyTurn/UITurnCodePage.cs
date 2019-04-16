using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public sealed class UITurnCodePage : UIDataBase
{
    public const string NAME = "UITurnCodePage";
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.Normal; }
    }

    public override HidePage hidePage
    {
        get { return HidePage.Destory; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.LuckyTurn; }
    }

    private Image payImg;

    public override void InitCodeData()
    {
        base.InitCodeData();
        payImg = CommTool.GetCompentCustom<Image>(gameObject,"payImg");
    }
    public override void OnShow(object data)
    {
        vc_list = data as List<VoiceContent>;
        if (vc_list == null) vc_list = new List<VoiceContent>();
        GameCtr.Instance.raw = rawImage;
        payImg.sprite= UIAtlasManager.Instance.LoadSprite(UIAtlasName.UILuckyTurn, "payText");
        GetCodeData();
    }

   
}
