using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public sealed class UITurnResultPage : UIDataBase
{
    public const string NAME = "UITurnResultPage";

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
        get { return AssetFolder.LuckyTurn; }
    }

    #region 礼品碎片
    private GameObject gift_part;
    private GameObject giftPart_1;
    private Transform giftpart_part;
    private Transform part_1;
    private Transform part_2;
    private Transform part_3;
    private Transform part_4;
    private GameObject congratulations;
    #endregion

    private GameObject onceAgain;
    private GameObject thankyouJoin;
    private GameObject surpriceGift;
    private GameObject youngWay;
    private GameObject young;
    private GameObject joke;
    private Transform gridPanel;
    private Text jokeContent;


    private GameObject healthyWay;
    private GameObject healthy;
    private GameObject music;
    private GameObject onSale;
    private Text saleCode;
    private GameObject gameOver;

    private bool isSccess = false;
    private const float wordtime = 0.27f;
    private  float jokeEndY = 300;
    private TransferData transferData = null;
    private Dictionary<LuckyTurnVoiceType, Action> dic = null;

    public override void Init()
    {
        //礼品碎片
        gift_part = CommTool.FindObjForName(gameObject, "giftpart");
        giftPart_1 = CommTool.FindObjForName(gift_part, "giftPart_1");
        giftpart_part = CommTool.FindObjForName(gift_part, "part_").transform;
        part_1 = CommTool.FindObjForName(gift_part, "part_1").transform;
        part_2 = CommTool.FindObjForName(gift_part, "part_2").transform;
        part_3 = CommTool.FindObjForName(gift_part, "part_3").transform;
        part_4 = CommTool.FindObjForName(gift_part, "part_4").transform;
        congratulations = CommTool.FindObjForName(gift_part, "congratulations");
        //再来一次
        onceAgain = CommTool.FindObjForName(gameObject, "onceAgain");
        //谢谢参与
        thankyouJoin = CommTool.FindObjForName(gameObject, "thankyouJoin");
        //神秘礼物
        surpriceGift = CommTool.FindObjForName(gameObject, "surpricegift");
        //年轻秘诀
        youngWay = CommTool.FindObjForName(gameObject, "youngway");
        young = CommTool.FindObjForName(youngWay, "young");
        joke = CommTool.FindObjForName(youngWay, "joke");
        gridPanel = CommTool.FindObjForName(joke, "GridLayoutPanel").transform;
        jokeContent = CommTool.GetCompentCustom<Text>(joke, "jokeContent");
        //健康秘笈
        healthyWay = CommTool.FindObjForName(gameObject, "healthyway");
        healthy = CommTool.FindObjForName(healthyWay, "healthy");
        music = CommTool.FindObjForName(healthyWay, "music");

        //优惠券
        onSale = CommTool.FindObjForName(gameObject, "onsale");
        saleCode = CommTool.GetCompentCustom<Text>(onSale, "salecode");
        //游戏结束
        gameOver = CommTool.FindObjForName(gameObject, "gameover");

    }

    public override void OnOpen()
    {
        Reg();
        InitAction();
    }


    public override void OnShow(object data)
    {
        if (data != null)
        {
            HideAllUI();
            isSccess = false;
            transferData = (TransferData)data;
            if (dic.ContainsKey(transferData.uiType))
                dic[transferData.uiType]();
        }
    }

    public override void HideSelf()
    {
        base.HideSelf();
        GameCtr.Instance.gameStatus.SetIsCatch(isSccess?1:0);
        GameCtr.Instance.gameMode.UpRecord(isSccess);//上报抓取记录
        gridPanel.DOLocalMoveY(-78.4f, 1);
        AudioManager.Instance.StopPlayAds(AudioType.Fixed);//停止播放
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);//停止摆动翅膀闪光带
        if (transferData.contiunAction != null)
            transferData.contiunAction(); //继续下一局
    }
    void Reg()
    {
        EventHandler.RegisterEvnet(EventHandlerType.GameOver, GameOver);
    }

    void InitAction()
    {
        dic = new Dictionary<LuckyTurnVoiceType, Action>
        {
            {LuckyTurnVoiceType.GiftPart, GiftPart},
            {LuckyTurnVoiceType.HealthyWay, HealthyWay},
            {LuckyTurnVoiceType.OnceAgain, OnceAgain},
            {LuckyTurnVoiceType.YoungWay, YoungWay},
            {LuckyTurnVoiceType.SupriseGift, SupriseGift},
            {LuckyTurnVoiceType.ThankYouJoin, ThankYouJoin},
            {LuckyTurnVoiceType.OnSale, OnSale},
        };
    }

    #region 礼品碎片
    void GiftPart()
    {
        InitGiftPartPos();
        gift_part.SetActive(true);
        giftPart_1.SetActive(true);
        giftpart_part.gameObject.SetActive(true);
        string speak = transferData.voice[0].Content;
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnElseGift, false);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
        DOVirtual.DelayedCall(speak.Length * wordtime, GiftPart_Fly);
    }

    void GiftPart_Fly()
    {
        giftPart_1.SetActive(false);
        giftpart_part.DOLocalMove(transferData.pos, 0.6f);
        giftpart_part.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.6f).OnComplete(() =>
           {
               Debug.Log("礼品碎片动画完成");
               giftpart_part.gameObject.SetActive(false);
               giftpart_part.localScale = Vector3.one;
               giftpart_part.localPosition = new Vector3(3.2f, -23.8f, 0);
               EventHandler.ExcuteEvent(EventHandlerType.ShowGiftPart, (Action<string>)MakeUp_Gift);
           });

    }
    //合成礼物
    void MakeUp_Gift(string speak)
    {
        float _time = 0.8f;
        Ease _ease = Ease.OutBounce;
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
        part_1.DOLocalMove(new Vector3(-50.4f, 55.7f, 0), _time).SetEase(_ease);
        part_2.DOLocalMove(new Vector3(37, 57, 0), _time).SetEase(_ease);
        part_3.DOLocalMove(new Vector3(-50.6f, -33, 0), _time).SetEase(_ease);
        part_4.DOLocalMove(new Vector3(37.1f, -30, 0), _time).SetEase(_ease).OnComplete(() =>
          {
              DOVirtual.DelayedCall(0.3f, () =>
              {
                  congratulations.SetActive(true);
                  OutPresent();
              });
          });
    }
    //初始化碎片坐标
    void InitGiftPartPos()
    {
        part_1.localPosition = new Vector3(-692, 445, 0);
        part_2.localPosition = new Vector3(692, 445, 0);
        part_3.localPosition = new Vector3(-692, -445, 0);
        part_4.localPosition = new Vector3(692, -445, 0);
        congratulations.SetActive(false);
    }
    #endregion

    void OnceAgain()
    {
        onceAgain.SetActive(true);
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnElseGift, false);
        DOVirtual.DelayedCall(SpeakAndLight(), HideSelf);
    }
    void HealthyWay()
    {
        healthyWay.SetActive(true);
        healthy.SetActive(true);
        music.SetActive(false);
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnElseGift, false);
        DOVirtual.DelayedCall(SpeakAndLight(), () =>
        {
            //播放随机音频
            GameCtr.Instance.RegHeadAction(HideSelf);//注册拍头退出
            healthy.SetActive(false);
            music.SetActive(true);
            int mp3 = UnityEngine.Random.Range(10, 18);
            AudioManager.Instance.StopPlayAds(AudioType.BackGround);
            AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, (AudioNams)mp3, false);
            float clipLength = AudioManager.Instance.GetClipLength(AssetFolder.LuckyTurn, (AudioNams)mp3);
            float mucicTime = clipLength + 0.5f;
            StartCoroutine(CommTool.TimeFun(mucicTime, mucicTime, null, ()=> 
            {
                string[] musicss = transferData.voice[1].Content.Split('|');
                int speakIndex = UnityEngine.Random.Range(0, musicss.Length);
                string speak2 = musicss[speakIndex];
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak2);
                StartCoroutine(CommTool.TimeFun(speak2.Length * wordtime, speak2.Length * wordtime, null, HideSelf));
            }));
        });
    }
    //音符跳动
    void MusicDance()
    {
        Transform musicParent = music.transform;
        for (int i = 0; i < musicParent.childCount; i++)
        {
            musicParent.GetChild(i);
        }
    }



    void YoungWay()
    {
        youngWay.SetActive(true);
        young.SetActive(true);
        joke.SetActive(false);
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnElseGift, false);
        DOVirtual.DelayedCall(SpeakAndLight(), () =>
        {
            young.SetActive(false);
            joke.SetActive(true);
            GameCtr.Instance.RegHeadAction(HideSelf);//注册拍头退出
            int indexJoke= UnityEngine.Random.Range(0, transferData.joke.Count);
            string speak =  transferData.joke[indexJoke].Type;
            jokeContent.text = speak;
            int row = JokeRows(speak);
            if (row > 6)
                gridPanel.DOLocalMoveY(78.4f, 14).SetLoops(2,LoopType.Restart).SetEase(Ease.Linear);
            else
                gridPanel.DOLocalMoveY(1f, 0.01f);
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
            transferData.joke.RemoveAt(indexJoke);
            Debug.Log("jokeLeng--" + speak.Length * wordtime);
            StartCoroutine(CommTool.TimeFun(speak.Length * wordtime, speak.Length * wordtime, null, () =>
              {
                //播音效
                  AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.JokeOver, false);
                  string[] jokess = transferData.voice[1].Content.Split('|');
                  int speakIndex = UnityEngine.Random.Range(0, jokess.Length);
                  string speak2 = jokess[speakIndex];
                  Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak2);
                  StartCoroutine(CommTool.TimeFun(speak2.Length * wordtime, speak2.Length * wordtime, null, HideSelf));
              }));
        });
    }
    //笑话行数
    int JokeRows(string speak)
    {
       string[] sps= speak.Split(("@\n").ToCharArray());
       int nums = sps.Length;
        if (nums > 0)
        {
            for (int i = 0; i < sps.Length; i++)
            {
                if (sps[i].Length >= 25) nums++;
            }
        }
        else
            nums = speak.Length / 25;
       return nums;  //240 height 最多放6行
    }

    void ThankYouJoin()
    {
        thankyouJoin.SetActive(true);
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnThankyouJoin, false);
        DOVirtual.DelayedCall(SpeakAndLight(), HideSelf);
    }
    void SupriseGift()
    {
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnSurpriceGift, false);
        surpriceGift.SetActive(true);
        SpeakAndLight();
        OutPresent();
    }

    void OnSale()
    {
        var salelist = GameCtr.Instance.ChangeType<LuckyTurnMgr>().listOnSaleNumber;
        if (salelist != null && salelist.Count > 0)
        {
            GameCtr.Instance.RegHeadAction(HideSelf);//注册拍头退出
            onSale.SetActive(true);
            saleCode.text = salelist[0].Content;
            //操作数据库 更新优惠券状态
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.UpdateOnSaleValue, salelist[0].Type);//type 对应id
            salelist.RemoveAt(0);
            Debug.Log("还有优惠劵：：" + salelist.Count);
            SpeakAndLight();
            StartCoroutine(CommTool.TimeFun(30, 30, null, HideSelf, null));
        }

    }
    //说话并要翅膀
    float SpeakAndLight()
    {
        string speak = transferData.voice[0].Content;
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀
        return speak.Length * wordtime;
    }

    //出礼品
    void OutPresent()
    {
        Debug.Log("****出礼品****");
        isSccess = true;
        Android_Call.UnityCallAndroid(AndroidMethod.AutoPresent);//自动出礼物
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
        //5秒后 继续游戏
        DOVirtual.DelayedCall(8, HideSelf);
    }
    
    //隐藏所有UI
    void HideAllUI()
    {
        gift_part.SetActive(false);
        onceAgain.SetActive(false);
        thankyouJoin.SetActive(false);
        surpriceGift.SetActive(false);
        youngWay.SetActive(false);
        healthyWay.SetActive(false);
        onSale.SetActive(false);
        gameOver.SetActive(false);
    }
    //游戏结束退出
    void GameOver(object data)
    {
        HideAllUI();
        gameOver.SetActive(true);
        //游戏推出  
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
            "游戏结束喽，如果还想玩，再来找小胖吧");
        DOVirtual.DelayedCall(4, GameCtr.Instance.AppQuit);
    }
}
