using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public sealed class UIMessagePage : UIDataBase
{
    public const string NAME = "UIMessagePage";
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
            return HidePage.Hide;
        }
    }
    public override AssetFolder assetFolder
    {
        get
        {
            return AssetFolder.LuckyBoy;
        }
    }
    private Text msg;
    private GameObject startUI;
    private Transform level;
    private Image round;
    private Image _level;
    private Image yi;
    private Image zhong;
    private Image gao;

    private const float timeLen = 0.3f;
    public override void Init()
    {
        base.Init();
        msg = CommTool.GetCompentCustom<Text>(gameObject, "msg");
        startUI = CommTool.FindObjForName(gameObject, "startui");
        round = CommTool.GetCompentCustom<Image>(startUI, "round");
        level = CommTool.FindObjForName(startUI, "level").transform;
        _level = CommTool.GetCompentCustom<Image>(startUI, "_level");
        yi = CommTool.GetCompentCustom<Image>(level.gameObject, "yi");
        zhong = CommTool.GetCompentCustom<Image>(level.gameObject, "zhong");
        gao = CommTool.GetCompentCustom<Image>(level.gameObject, "gao");
    }



    public override void OnShow(object data)
    {
        base.OnShow(data);
        msg.gameObject.SetActive(false);
        startUI.SetActive(false);
        if (data is string)
        {
            msg.gameObject.SetActive(true);
            string content = data.ToString();
            msg.text = content;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, content);
            DOVirtual.DelayedCall(content.Length * timeLen, GameCtr.Instance.AppQuit);
        }
        else if (data is Action)
        {
            Action ac = data as Action;
            round.transform.localPosition = new Vector3(-800, 98, 0);
            level.transform.localPosition = new Vector3(800, -140, 0);
            startUI.SetActive(true);
            SetRoundImageShow();
            round.transform.DOLocalMoveX(0, 0.5f);
            level.transform.DOLocalMoveX(0, 0.5f);
            Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
            Android_Call.UnityCallAndroidHasParameter<int>(AndroidMethod.ShakeWave, 5000);
            Action act = () => Hide(ac);
            EventHandler.ExcuteEvent(EventHandlerType.RoundStart, act);
        }

    }
    void Hide(Action ac)
    {
        round.transform.DOLocalMoveX(-800, 0.5f);
        level.transform.DOLocalMoveX(800, 0.5f).OnComplete(() =>
        {
            UIManager.Instance.ShowUI(NAME, false);
            if (ac != null)
                ac();
            ac = null;
        }); ;
    }


    string GetLevel()
    {
        string content = "易";
        GameLevel glevel = LuckyBoyMgr.Instance.gameMode.gameMisson._gameLevel;
        if (glevel == GameLevel.Nan)
            content = "高";
        else if (glevel == GameLevel.Zhong)
            content = "中";
        return content;
    }
    //设置局数图片
    void SetRoundImageShow()
    {
        string _str = LuckyBoyMgr.Instance.gameMode.gameMisson._round + "round";
        round.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, _str);
        GameLevel gameLevel = LuckyBoyMgr.Instance.gameMode.gameMisson._gameLevel;
        _level.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, gameLevel.ToString());
        yi.sprite= UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain,"xing");
        zhong.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "xing");
        gao.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "xing");
        if (gameLevel == GameLevel.Zhong)
            gao.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "hui");
        else if (gameLevel == GameLevel.Yi)
        {
            zhong.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "hui");
            gao.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "hui");
        }
    }
}
