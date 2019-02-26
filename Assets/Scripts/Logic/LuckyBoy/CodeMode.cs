using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

public sealed class CodeMode : GameMode
{
    public CodeMode(GameCtr _sdk) : base(_sdk)
    {
        Debug.Log("////////支付模式\\\\\\\\\\");
    }

    #region 重写方法
    public override void SetMissonValue()
    {
        sdk.gameStatus.SetRemainRound(sdk.selectRound - 1);
        if (sdk.selectRound == 3)
            gameMisson = new ThreeRoundPlay(sdk);
        else
            gameMisson = new FiveRoundPlay(sdk);
    }

    public override void EnterGame()
    {
        SetMissonValue();
        base.EnterGame();
    }

    public override void GameStart()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIManager.Instance.ShowUI(UIBgPage.NAME, false);
        UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, false);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, true);
        UIManager.Instance.ShowUI(UIMovePage.NAME, true);
        UIManager.Instance.ShowUI(UITimePage.NAME, true);
    }

    public override void NoPay()
    {
        UIManager.Instance.ShowUI(UIMessagePage.NAME, false);
        UIManager.Instance.ShowUI(UIBgPage.NAME, false);
        StartEnterGame();
    }
    public override void UpRecord(bool isSuccess)
    {
        base.UpRecord(isSuccess);
        LuckyBoyMgr.Instance.startCarwTime = CommTool.GetTimeStamp();
        Android_Call.UnityCallAndroidHasParameter<bool, string>(AndroidMethod.SendCatchRecord,
                isSuccess, LuckyBoyMgr.Instance.startCarwTime);
    }

    public override List<VoiceContent> GetPayVoiceContent()
    {
        List<VoiceContent> vctlist = base.GetPayVoiceContent();
        if (vctlist != null)
            return vctlist.FindAll(v => v.Type == "1");//玩游戏
        return null;
    }

    public override void ShowEndUI(GameMisson gamePlay)
    {
        int time = 0;
        VoiceContent tVC = null;
        if (sdk.gameStatus.status == 1)//抓中过 
        {
            tVC = gamePlay.GetVoiceContent(gamePlay._Count - 2).Content;
            time = Convert.ToInt32(tVC.Time);
        }
        else
        {
            if (sdk.autoSendGift)//自动送礼品  还是之前的逻辑
            {
                tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;
            }
            else
            {
                if (sdk.ChangeType<LuckyBoyMgr>().payCount == 0)//首次进入
                    tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;
                else
                    tVC = gamePlay.GetSpecialVoice(VoiceType.GameEnd_NoGift, 0).Content;
            }
            time = Convert.ToInt32(tVC.Time);
        }
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
        int spaceTime = time - 2;//2秒后显示
        bool isEnd = false;
        sdk.RegHeadAction(() => isEnd = true);
        sdk.StartCoroutine(CommTool.TimeFun(time, 0.5f, (ref float t) =>
        {
            if (sdk.autoSendGift) //自动送礼品 才会显示第二结束界面
            {
                if (spaceTime == t)
                {
                    if (sdk.gameStatus.status != 1)
                    {
                        if (sdk.gameMode.gameMisson._timesPay == 1)
                            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.GameEndGame);
                        else if (sdk.gameMode.gameMisson._timesPay == 2)
                            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.GameEndGift);
                    }
                }
            }
            if (isEnd)
            {
                sdk.AppQuit();
            }
            return isEnd;
        }, sdk.AppQuit));//游戏推出
    }


    //进入游戏
    public override void EnterGameByStatus()
    {
        base.EnterGameByStatus();
        if (sdk.gameStatus.runStatus == GameRunStatus.GameEnd || sdk.gameStatus.runStatus == GameRunStatus.QRCode)
            StartEnterGame();
        else if (sdk.gameStatus.runStatus == GameRunStatus.NoPay)
        {
            //UIManager.Instance.ShowUI(UIBgPage.NAME, true);
            //查询是否支付
            Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, sdk.gameStatus.applyRechargeId, true);
        }
        else if (sdk.gameStatus.runStatus == GameRunStatus.InGame)
        {
            gameMisson.IntiPayTimes(sdk.gameStatus.payTime);//在游戏中 直接进入游戏
            GameStart();
        }
    }

    /// <summary>
    /// 批量上报记录
    /// </summary>
    public override void StartUpRecordList()
    {
        List<C_RecordData> list = sdk.handleSqlite.C_ReadData();
        if (list == null || list.Count == 0)
        {
            Debug.Log("record表为空不需上报");
            return;
        }
        string json = JsonMapper.ToJson(list);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SendCatchRecordList, json);
    }


    //进入试玩
    public override  void EnterTryPlay()
    {
        gameMisson = new Phone_ThreeRoundPlay(sdk);
        sdk.gameStatus.SetRunStatus(GameRunStatus.GameEnd);
        UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, false);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, true);
        UIManager.Instance.ShowUI(UIMovePage.NAME, true);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, true, true);
        //注册试玩结束
        EventDispatcher.AddListener(EventHandlerType.TryPlayOver, TryPlayOver);
    }

    #endregion


    #region 私有方法

    //开始进入游戏
    private void StartEnterGame()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.QRCode);
#if UNITY_ANDROID
        UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, true, GetPayVoiceContent(), o =>
        {
            ExtendContent EC = gameMisson.GetVoiceContent(0);
            if (EC != null)
            {
                //播放载入语音
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, EC.Content.Content);
                EC = null;
            }
            Android_Call.UnityCallAndroidHasParameter<int>(AndroidMethod.ShakeWave, 4000);
            Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 4000);
        });
#endif
    }

    //试玩结束
    private void TryPlayOver()
    {
        SetMissonValue();
        sdk.gameTryStatus = 3;
        sdk.gameStatus.SetRunStatus(GameRunStatus.GameEnd);
        UIManager.Instance.ShowUI(UIMovePage.NAME, false);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, false);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, false);
        UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, true, GetPayVoiceContent());
    }

    #endregion
}
