using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class UIPhoneTimePage : UIDataBase
{
    public const string NAME = "UIPhoneTimePage";
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
        get { return AssetFolder.LuckySendPhone; }
    }
    private Image time_1;
    private Image time_2;
    private Text text_time;
    private int num1 = 3;//时间
    private int num2 = 0;
    private int remainRound = 0;//剩余局数
    private int succNum = 0;
    private bool isUpFinish = true;
    private Action aciton = null;
    private GameCtr sdk;
    private GameMisson gamePlay;

    private GameObject tryPlay;
    private GameObject showTime;
    private bool IsTryPlay = false;
    private List<VoiceContent> operlist;
    private List<VoiceContent> operEnterlist;
    private List<CatchSuccessData> catchList;


    public override void Init()
    {
        base.Init();
        sdk = LuckyBoyMgr.Instance;
        gamePlay = sdk.gameMode.gameMisson;
        catchList = GameCtr.Instance.ChangeType<LuckySendPhoneMgr>().catchlist;
        time_1 = CommTool.GetCompentCustom<Image>(gameObject, "time_1");
        time_2 = CommTool.GetCompentCustom<Image>(gameObject, "time_2");
        tryPlay = CommTool.FindObjForName(gameObject, "tryPlay");
        showTime = CommTool.FindObjForName(gameObject, "Times");
        text_time = CommTool.GetCompentCustom<Text>(gameObject, "time");
        EventHandler.RegisterEvnet(EventHandlerType.Success, Success);
        EventHandler.RegisterEvnet(EventHandlerType.HeadPress, HeadPress);
        EventHandler.RegisterEvnet(EventHandlerType.GameEndStart, HeadPress);
    }

    public override void OnShow(object data)
    {
        IsTryPlay = (bool)data;
        tryPlay.SetActive(IsTryPlay);
        showTime.SetActive(!IsTryPlay);
        operlist = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.Common, (int)SendPhoneOperateType.NoOperate);
        operEnterlist= gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.Common, (int)SendPhoneOperateType.NoOperateEnter);
        aciton = NormalUpdate;
        OnInit();
        StartCoroutine(TimeUpdate());
    }
    //根据本地记录重置数据
    private void OnInit()
    {
        remainRound = 2;
        gamePlay.signTimes = GetCatchSuccessTime();
        text_time.text = remainRound.ToString();
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
        SpeakOperation(number);
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

    //无操作说话
    private void SpeakOperation(int time)
    {
        string speak = "";
        if (time == 3 && remainRound == 2)
        {
            if (IsTryPlay || gamePlay.signTimes == 0)
                speak = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.OnePayEnter, (int)SendPhoneOperateType.NoOperateEnter)[0].Content;
            else if (gamePlay.signTimes == 1)
                speak = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.TwoPayEnter, (int)SendPhoneOperateType.NoOperateEnter)[0].Content;
            else
                speak = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.ThreePayEnter, (int)SendPhoneOperateType.NoOperateEnter)[0].Content;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);

        }
        else
        {
            if (time == 3)
            {
                if (remainRound == 1)
                    speak = operEnterlist[0].Content;
                else
                    speak = operEnterlist[1].Content;
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
            }
            else
            {
                for (int i = 0; i < operlist.Count; i++)
                {
                    if (operlist[i].Time == time.ToString())
                        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, operlist[i].Content);
                }
            }
        }
    }

    //获得抓中次数
    private int GetCatchSuccessTime()
    {
        if (catchList==null||catchList.Count == 0)
            return 0;
        if (catchList.Count == 1)
        {
            if (catchList[0].cnum< 3)
                return 0;
            else
                return 1;
        }
        else if (catchList.Count == 2)
        {
            if (catchList[0].cnum < 3)//最新的
                return 0;
            else
            {
                if (catchList[1].cnum < 3)
                    return 1;
                else
                    return 2;
            }
        }
        return 0;
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
        if (remainRound >= 0)
        {
            text_time.text = remainRound.ToString();
            EventHandler.ExcuteEvent(EventHandlerType.RestStart, null);
            aciton = null;
            aciton = RestStartUpdate;
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.BackGround, AudioNams.backGround, true);
        }
        else//游戏结束
        {
            isUpFinish = true;
            Debug.Log("标记次数：："+ gamePlay.signTimes+ "---本次抓中：："+succNum);
            if (gamePlay.signTimes == 2 && succNum == 3)
            {
                //请求兑换码
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.ResPhoneCode, CombinApplyId());
            }
            gamePlay._Count = succNum;
            var list = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.Common, (int)SendPhoneOperateType.GameEnd);
            string speak = list[succNum].Content;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
            Android_Call.UnityCallAndroid(AndroidMethod.AutoPresent);//自动出礼物
            UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.GameEnd);
            EventHandler.ExcuteEvent(EventHandlerType.GameEndStart, true);
            DOVirtual.DelayedCall(speak.Length * GameCtr.speakTime, () => sdk.gameMode.ShowEndUI(gamePlay));
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
        if (IsTryPlay)
            StartCoroutine(WinTryPlay((CatchTy)data));
        else
            StartCoroutine(WinPlay((CatchTy)data));
    }

    IEnumerator WinPlay(CatchTy cat)
    {
        sdk.gameStatus.SetRemainRound(remainRound - 1);
        if (cat == CatchTy.Catch)
        {
            #region 胜利等待取走礼物
            succNum++;
            //sdk.gameStatus.SetIsCatch(1);
            sdk.gameMode.UpRecord(true);
            UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.Catch);
            CommTool.SaveIntData(CatchTimes.Catch.ToString());
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shengli, false);//播放胜利音效
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
            var tVC = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.Common, (int)SendPhoneOperateType.Catch);
            string speak = tVC[UnityEngine.Random.Range(0, tVC.Count - 1)].Content;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
            EffectMrg.ShowEffectNormal(); //播放特效
            float winTime = speak.Length * GameCtr.speakTime + 2f;
            yield return CommTool.TimeFun(winTime, winTime);
            EffectMrg.HideEffectNoraml();
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);//停止摆动翅膀闪光带
            #endregion
        }
        else
        {
            string[] contents = null;
            float delytime = 0;
            sdk.gameMode.gameMisson.NoZhuaZhong(cat, new ExtendContent(), out delytime, out contents);
            sdk.gameMode.UpRecord(false);
            UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, cat);//失败显示
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shibai, false);
            //随机语音
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, contents[0]);
            if (remainRound > 0)
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
            else
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, ((int)delytime - 1) * 1000);
            bool isEnd = false;
            sdk.RegHeadAction(() => isEnd = true);
            yield return CommTool.TimeFun(delytime, 0.5f, (ref float t) => isEnd, null);
        }
        UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, false);
        RestStart();
    }


    IEnumerator WinTryPlay(CatchTy cat)
    {
        if (cat == CatchTy.Catch)
        {
            #region 胜利等待取走礼物
            UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.Catch);
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shengli, false);//播放胜利音效
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
            var tVC = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.TryPlay, (int)SendPhoneOperateType.Catch);
            string speak = tVC[UnityEngine.Random.Range(0, tVC.Count - 1)].Content;
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, speak);
            EffectMrg.ShowEffectNormal(); //播放特效
            float winTime = speak.Length * GameCtr.speakTime + 2f;
            yield return CommTool.TimeFun(winTime, winTime);
            EffectMrg.HideEffectNoraml();
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);//停止摆动翅膀闪光带
            #endregion
        }
        else
        {
            string[] contents = null;
            float delytime = 0;
            sdk.gameMode.gameMisson.NoZhuaZhong(cat, null, out delytime, out contents);
            UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, cat);//失败显示
            AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shibai, false);
            //随机语音
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, contents[0]);
            if (remainRound > 0)
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
            else
                Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, ((int)delytime - 1) * 1000);
            yield return CommTool.TimeFun(delytime, delytime);
        }
        UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.GameOverTryPlay);
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, false);
        EventDispatcher.Dispatch(EventHandlerType.TryPlayOver);
    }
    //拍头停止计时
    private void HeadPress(object o)
    {
        isUpFinish = (bool)o;
    }

    //组合账单号
    private string CombinApplyId()
    {
        string applyId = sdk.gameStatus.applyRechargeId; ;
        for (int i = 0; i < catchList.Count; i++)
        {
            applyId += ","+ catchList[i].applyRechargeid;
        }
        Debug.Log("三个账单号::"+applyId);
        return applyId;
    }


    private void OnDestroy()
    {
        aciton = null;
        EventHandler.UnRegisterEvent(EventHandlerType.Success, Success);
        EventHandler.UnRegisterEvent(EventHandlerType.HeadPress, HeadPress);
        EventHandler.UnRegisterEvent(EventHandlerType.GameEndStart, HeadPress);
    }
}
