using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Video;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using LitJson;

public class UIMovieQRCodePage : UIDataBase
{
    public const string NAME = "UIMovieQRCodePage";
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
        get { return AssetFolder.LuckyBoy; }
    }
    public GameObject qrCode;
    public RawImage raw;
    public VideoPlayer vplayers;
    public GameObject loadings;
    public GameObject text_Pay;
    public Animator animator;
    public Image xiaoP;
    public Text moneyText;
    public Text gameTimes;
    public GameObject tryTran;
    public CanvasGroup tryWanCanvas;
    public List<VoiceContent> vc_lists;
    public IEnumerator currentIE = null;
    public GameMisson gamePlay;
    public GameCtr sdk;
    public override void Init()
    {
        base.Init();
        currentIE = PlayVoiceIe();
        sdk = LuckyBoyMgr.Instance;
        qrCode = CommTool.FindObjForName(gameObject, "QR-code");
        raw = CommTool.GetCompentCustom<RawImage>(qrCode, "RawImage");
        loadings = CommTool.FindObjForName(qrCode, "loading");

        xiaoP = CommTool.GetCompentCustom<Image>(qrCode, "xiaopang");
        animator = CommTool.GetCompentCustom<Animator>(qrCode, "xiaopang");
        animator.enabled = false;

        tryTran = CommTool.FindObjForName(gameObject, "tryPlay");
        tryWanCanvas = tryTran.GetComponent<CanvasGroup>();
        tryTran.SetActive(false);
        if (sdk.isGame)
        {
            gamePlay = sdk.gameMode.gameMisson;
            gamePlay.UpdateSpecialVoice(VoiceType.Loading);
            vplayers = CommTool.GetCompentCustom<VideoPlayer>(gameObject, "movie");
            text_Pay = CommTool.FindObjForName(gameObject, "text_game");
            moneyText = CommTool.GetCompentCustom<Text>(text_Pay, "shiyuan_game");
            gameTimes = CommTool.GetCompentCustom<Text>(text_Pay, "wuci_game");
        }
        else
        {
            vplayers = CommTool.GetCompentCustom<VideoPlayer>(gameObject, "movie_nogame");//不游戏
            text_Pay = CommTool.FindObjForName(raw.gameObject, "text_nogame");
            moneyText = CommTool.GetCompentCustom<Text>(text_Pay, "shiyuan_nogame");
            EventHandler.RegisterEvnet(EventHandlerType.StopCoroutine, o => StopCoroutine(currentIE));
        }

        EventHandler.RegisterEvnet(EventHandlerType.QRCodeSuccess, QRCodeSuccess);

        if (AppConst.test)
        {
            //试玩
            UIEventLisener.Get(tryTran).OnClick += o => AndroidCallUnity.Instance.Question_Wing("1");
        }

    }

    public override void OnShow(object data)
    {
        vc_lists = data as List<VoiceContent>;
        if (vc_lists == null) vc_lists = new List<VoiceContent>();
        qrCode.SetActive(false);
        vplayers.gameObject.SetActive(true);
        sdk.raw = raw;
        if (sdk.isFirstGame)//开始显示试玩界面
        {
            PlayMovies();
        }
        else  //试玩结束
        {
            MovieOvers(null);
        }
    }
    public override void OnHide()
    {
        base.OnHide();
        // EventHandler.UnRegisterEvent(EventHandlerType.QRCodeSuccess, QRCodeSuccess);
    }

    public void PlayMovies()
    {
        vplayers.loopPointReached += MovieOvers;
        vplayers.Play();
    }
    public virtual void MovieOvers(VideoPlayer p)
    {
        Debug.Log("视频播放完毕");
        if (sdk.isGame)
        {
            tryTran.SetActive(true);
            sdk.gameTryStatus = 1;//视频播放完成  可以试玩
        }
        moneyText.text = sdk.money + "元";
        vplayers.gameObject.SetActive(false);
        qrCode.SetActive(true);
        loadings.SetActive(true);
        raw.gameObject.SetActive(false);
        TryPlayAlpha();
        GetCodeData();
    }
    //获得二维码
    public override void GetCodeData()
    {
        JsonData jsondata = new JsonData();
        jsondata["flag"] = GameCtr.Instance.isNoDied ? 0 : 1;//0是 没有时间限制  1是有一分钟限制
        bool isCanPlay = Android_Call.UnityCallAndroidHasReturn<bool>(AndroidMethod.isCanPlay);
        if (isCanPlay)
        {
            #region 获取二维码
            bool flagQuit = false;
            StartCoroutine(CommTool.TimeFun(60, 5, (ref float t) =>
            {
                if (!sdk.isGetCode)
                {
                    // Android_Call.UnityCallAndroid(AndroidMethod.GetDrawQrCode);
                    NetMrg.Instance.SendRequest(AndroidMethod.GetDrawQrCode, jsondata);
                    if (t == 0 && !flagQuit)
                    {
                        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                            "小胖发现没有网络哦，请联网后再来玩吧");
                        t = 8;
                        if (t < 10) t = 10;
                        flagQuit = true;
                    }
                    else if (t == 0)// 没有网络时间到退出
                    {
                        sdk.AppQuit();//游戏推出
                        return true;
                    }
                    return false;
                }
                else
                    return true;

            }));
            #endregion
        }
        else
        {
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
               "没有礼品不能开始游戏");
            DOVirtual.DelayedCall(3, GameCtr.Instance.AppQuit);
        }
    }

    public override IEnumerator PlayVoiceIe()
    {
        int index = 0;
        float voiceTime = 0;//语音时间
        float total_time = 0;
        int[] timeArray = { 0, 12, 30, 43, 57 };
        while (index < vc_lists.Count)
        {
            if (index < vc_lists.Count)
            {
                yield return new WaitForSeconds(timeArray[index] - total_time);//开始播放下一个
                animator.enabled = true;
                vc_lists[index].Content = CommTool.TransformPayVoice("#", vc_lists[index].Content, sdk.money.ToString());
                vc_lists[index].Content = CommTool.TransformPayVoice("*", vc_lists[index].Content, sdk.selectRound.ToString());
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, vc_lists[index].Content);
                voiceTime = vc_lists[index].Content.Length * 0.265f;
                yield return new WaitForSeconds(voiceTime);//停止嘴巴动
                animator.enabled = false;
                xiaoP.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIQRCode, "1");
                total_time = timeArray[index] + voiceTime;
                index++;
            }
        }
        yield return new WaitForSeconds(10);//避免临界点 退出   加10秒
        sdk.AppQuit();
    }

    //二维码返回成功
    public void QRCodeSuccess(object data)
    {
        loadings.SetActive(false);
        raw.gameObject.SetActive(true);
        text_Pay.SetActive(true);
        if (moneyText)
            moneyText.text = sdk.money + "元";
        if (gameTimes)
            gameTimes.text = sdk.selectRound + "次";
        StartCoroutine(currentIE);//二维码界面计时
    }
    //试玩胡明户型
    void TryPlayAlpha()
    {
        tryWanCanvas.DOFade(1, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        EventHandler.UnRegisterEvent(EventHandlerType.QRCodeSuccess, QRCodeSuccess);
    }
}
