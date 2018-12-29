using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public sealed class UIBigBomCodePage : UIViewBase {

    public const string NAME = "UIBigBomCodePage";
    public override UIShowPos _showPos { get { return UIShowPos.Normal; } }
    public override HidePage _hidePage { get { return HidePage.Destory; } }
    public override AssetFolder _assetFolder { get { return AssetFolder.LuckyBigBom; } }


    public override void OnEnter()
    {
        base.OnEnter();

        vc_list = _Data as List<VoiceContent>;
        anim.SetActive(true);
        code.SetActive(false);
        GameCtr.Instance.raw = rawImage;
        PlayMovie();

        //测试
        MovieOver(null);
    }


    public override void MovieOver(VideoPlayer p)
    {
        anim.SetActive(false);
        code.SetActive(true);
        EventDispatcher.AddListener<string>(EventHandlerType.Question_Wing, Question_Wing);
        Invoke("NormalEnter",2);
    }

    public void Question_Wing(string data)
    {
        HideSelf();
        // UIManager.Instance.ShowUI(UIBigBomPage.NAME, true, true);//进入试玩
        UIMgr.Instance.ShowUI(UIBigBomPage.NAME, true, true);
    }

    void NormalEnter()
    {
        HideSelf();
        //UIManager.Instance.ShowUI(UIBigBomPage.NAME, true, false);//正常进入
        UIMgr.Instance.ShowUI(UIBigBomPage.NAME, true, false);
    }
}
