using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public sealed class UITimePage : UIDataBase
{
    public const string NAME = "UITimePage";
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
    private Image time_1;
    private Image time_2;
    private Text text_time;
    private Image quxian;
    private int num1 = 3;//时间
    private int num2 = 0;
    private int remainRound = 0;//剩余局数
    private int succNum = 0;
    private bool isUpFinish = true;
    private Action aciton = null;
    private object elist_Round;//List<VoiceContentType_5>
    //根据时间变化的index
    private int indexTime = 1;
    private const int indexVoice = 0;
    GameCtr sdk;
    GameMisson gamePlay;
    ExtendContent ECT = null;


    public override void Init()
    {
        base.Init();
        sdk = LuckyBoyMgr.Instance;
        gamePlay = sdk.gameMode.gameMisson;
        time_1 = CommTool.GetCompentCustom<Image>(gameObject, "time_1");
        time_2 = CommTool.GetCompentCustom<Image>(gameObject, "time_2");
        // quxian = CommTool.GetCompentCustom<Image>(gameObject, "quxian");
        //text_time = CommTool.GetCompentCustom<Text>(gameObject, "time");
        EventHandler.RegisterEvnet(EventHandlerType.Success, Success);
        EventHandler.RegisterEvnet(EventHandlerType.HeadPress, HeadPress);
        EventHandler.RegisterEvnet(EventHandlerType.GameEndStart, HeadPress);
        EventHandler.RegisterEvnet(EventHandlerType.RoundStart, RoundStart);
    }


    public override void OnShow(object data)
    {
        base.OnShow(data);
        aciton = NormalUpdate;
        OnInit();
        StartCoroutine(TimeUpdate());
    }
    //根据本地记录重置数据
    private void OnInit()
    {
        remainRound = sdk.gameStatus.remainGameRound;
        gamePlay.UpdateRoundVoice();
        AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.BackGround, AudioNams.backGround, true);
    }

    IEnumerator TimeUpdate()
    {
        while (num1 > 0 || (num1 >= 0 && num2 >= 0))
        {
            yield return new WaitForSeconds(1);
            if (isUpFinish)
                continue;
            if (aciton != null)
                aciton();
            if (num1 < 0 || num2 < 0)
            {
                num1 = 0;
                num2 = 0;
                aciton = null;
                //时间到自动抓
                sdk.RegHeadAction(null);
                AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.downing2, false);
                EventHandler.ExcuteEvent(EventHandlerType.HeadPress, true);
            }
        }
    }

    private void NormalUpdate()
    {
        int number = 30 - Convert.ToInt32(num1 + "" + num2);
        ECT = gamePlay.GetVoiceContent(indexTime);
        //播放语音
        if (indexTime < gamePlay._Count && number.ToString() == ECT.Time)
        {
              //不送礼品 支付次数大于2次 第一局 改变语音
            if (!sdk.autoSendGift&& number == 0 && sdk.ChangeType<LuckyBoyMgr>().payCount > 1 && 
                (remainRound == 4&&sdk.selectRound==5|| remainRound == 2 && sdk.selectRound == 3))
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "这次要看准时机哦，一定能抓到的，加油吧");
            else
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, ECT.Content.Content);
            indexTime++;
        }
        if (num2 == 0)
        {
            num2 = 9;
            num1--;
        }
        else
            num2--;
        if (num1 >= 0 && num2 >= 0)
        {
            time_1.overrideSprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, num1.ToString());
            time_2.overrideSprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, num2.ToString());
        }
    }

    private void RestStartUpdate()
    {
        num1 = 3;
        num2 = 0;
        time_1.overrideSprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, num1.ToString());
        time_2.overrideSprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, num2.ToString());
        aciton = null;
        aciton = NormalUpdate;
    }
    private void RestStart()
    {
        remainRound--;
        if (remainRound >= 0 && sdk.gameMode.gameMisson._timesPay < 3)//不是第三次支付
        {
            indexTime = 1;
            gamePlay.UpdateRoundVoice();
            EventHandler.ExcuteEvent(EventHandlerType.RestStart, null);
            aciton = null;
            aciton = RestStartUpdate;
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.BackGround, AudioNams.backGround, true);
        }
        else//游戏结束
        {
            isUpFinish = true;
            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.GameEnd);
            EventHandler.ExcuteEvent(EventHandlerType.GameEndStart, true);
            StopCoroutine(TimeUpdate());
            sdk.gameMode.ShowEndUI(gamePlay);
        }
    }
    /// <summary>
    /// 是否抓中
    /// </summary>
    /// <param name="data"></param>
    private void Success(object data)
    {
        isUpFinish = true;
        AudioManager.Instance.StopPlayAds(AudioType.BackGround);
        StopCoroutine(WinPlay((CatchTy)data));
        StartCoroutine(WinPlay((CatchTy)data));
    }

    IEnumerator WinPlay(CatchTy cat)
    {
        sdk.gameStatus.SetRemainRound(remainRound - 1);
        //MyFuncPerSecond func = null;
        VoiceContent tVC = null;
        if (cat == CatchTy.Catch)
        {
            #region 有异物
            //if (!sdk.isTabke())//出口有异物
            //{
            //    UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.HasBoy);
            //    ExtendContent etable = gamePlay.GetSpecialVoice(VoiceType.Special, 1);
            //    func = (ref float t) =>
            //    {
            //        if (t == 20 && !sdk.isTakeAway)
            //        {
            //            sdk.Speak(etable.Content.Content);//播放音效
            //        }
            //        if (t == 0) t = 21;
            //        return sdk.isTakeAway;
            //    };
            //    yield return sdk.TimeFun(20, 1, func);
            //}
            #endregion

            #region 胜利等待取走礼物
            succNum++;
            sdk.gameStatus.SetIsCatch(1);
            sdk.gameMode.UpRecord(true);
            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.Catch);
            Android_Call.UnityCallAndroid(AndroidMethod.AutoPresent);//自动出礼物
            CommTool.SaveIntData(CatchTimes.Catch.ToString());
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shengli, false);//播放胜利音效
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
            tVC = gamePlay.GetVoiceContent(indexVoice).Winning;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
            EffectMrg.ShowEffectNormal(); //播放特效
            float winTime = Convert.ToInt32(tVC.Time) + 2f;
            yield return CommTool.TimeFun(winTime, winTime);
            EffectMrg.HideEffectNoraml();
            UIManager.Instance.ShowUI(UIPromptPage.NAME, false);
            #region  等待取礼物
            //int winafter = Convert.ToInt32(gamePlay.GetVoiceContent(indexVoice).Winafter.Time) + 2;//时间间隔两秒
            //func = null;
            //func = (ref float t) =>
            //{
            //    if (sdk.isTakeAway)//已取走
            //    {
            //        Debug.Log("已取走");
            //        EffectMrg.StopPlayEffect();
            //        sdk.WonDoll(false);
            //        EventHandler.ExcuteEvent(EventHandlerType.ClosePage, null);
            //        if (sdk.gamePlay._timesPay == 3)
            //            time_ci = -10;
            //        return true;
            //    }
            //    if (t == 0)
            //    {
            //        sdk.Speak(gamePlay.GetVoiceContent(indexVoice).Winafter.Content);//播放音效
            //        t = winafter;
            //    }
            //    return false;
            //};
            //yield return sdk.TimeFun(winafter, 1, func);
            #endregion
            #endregion
        }
        else
        {
            string[] contents = null;
            float delytime = 0;
            gamePlay.NoZhuaZhong(cat, gamePlay.GetVoiceContent(indexVoice), out delytime, out contents);
            sdk.gameMode.UpRecord(false);
            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, cat);//失败显示
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shibai, false);
            int idx = UnityEngine.Random.Range(0, contents.Length);
            //随机语音
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, contents[idx]);
            if (remainRound > 0)
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
            else
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, ((int)delytime - 1) * 1000);
            bool isEnd = false;
            sdk.RegHeadAction(() => isEnd = true);
            yield return CommTool.TimeFun(delytime, 0.5f, (ref float t) => isEnd, null);
            EventHandler.ExcuteEvent(EventHandlerType.ClosePage, null);
        }
        RestStart();
    }

    //设置曲线值
    private void SetQuXImg()
    {
        Sprite sp = quxian.sprite;
        int random = 0;
        while (sp == quxian.sprite)
        {
            random = UnityEngine.Random.Range(1, 4);
            sp = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, "Bo-0" + random);
        }
        quxian.sprite = sp;
        sdk.randomQuXian = random;//就一种速度
    }
    private void RoundStart(object o)
    {
        VoiceContent tVC = gamePlay.GetVoiceContent(indexVoice).Content;
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
        int time = Convert.ToInt32(tVC.Time);
        Action ac = o as Action;
        StartCoroutine(HideRound(time, ac));
        tVC = null;
    }
    IEnumerator HideRound(int time, Action ac)
    {
        yield return new WaitForSeconds(time);
        if (ac != null)
            ac();
    }
    //拍头停止计时
    private void HeadPress(object o)
    {
        isUpFinish = (bool)o;
    }


    private void OnDestroy()
    {
        aciton = null;
        EventHandler.UnRegisterEvent(EventHandlerType.Success, Success);
        EventHandler.UnRegisterEvent(EventHandlerType.HeadPress, HeadPress);
        EventHandler.UnRegisterEvent(EventHandlerType.GameEndStart, HeadPress);
        EventHandler.UnRegisterEvent(EventHandlerType.RoundStart, RoundStart);
    }

}
