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
        if (sdk.isFirstGame)
            sdk.gameStatus.SetRunStatus(GameRunStatus.GameEnd);//不使用 每次进入重新开始
        else
            sdk.gameStatus.SetRunStatus(GameRunStatus.NoPay);
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
        base.GameStart();
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
        //Android_Call.UnityCallAndroidHasParameter<bool, string>(AndroidMethod.SendCatchRecord,
        // isSuccess, LuckyBoyMgr.Instance.startCarwTime);
        JsonData jsondata = new JsonData();
        jsondata["status"] = isSuccess ? 1 : 0;
        jsondata["reportTime"] = LuckyBoyMgr.Instance.startCarwTime;
        jsondata["openId"] = GameCtr.Instance.openId;
        jsondata["applyRechargeId"] = GameCtr.Instance.orderNumber;
        NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecord, jsondata);

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
        if (sdk.gameStatus.status == 1)
            tVC = gamePlay.GetVoiceContent(gamePlay._Count - 2).Content;
        else if (sdk.ChangeType<LuckyBoyMgr>().isAddConstraint && sdk.selectRound == 3)// 条件受限
        {
            if (sdk.autoSendGift && sdk.gameMode.gameMisson._timesPay == 2)//自动送礼品  还是之前的逻辑
                tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;  //说送礼物语音
            else
                tVC = gamePlay.GetVoiceContent(gamePlay._Count - 2).Content;
        }
        else
        {
            if (sdk.autoSendGift)//自动送礼品  还是之前的逻辑
                tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;
            else
            {
                if (sdk.gameMode.gameMisson._timesPay == 1)//首次进入
                    tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;
                else
                    tVC = gamePlay.GetSpecialVoice(VoiceType.GameEnd_NoGift, 0).Content;
            }
        }
        time = Convert.ToInt32(tVC.Time);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
        int spaceTime = time - 2;//2秒后显示
        bool isEnd = false;
        sdk.RegHeadAction(() => isEnd = true);
        sdk.StartCoroutine(CommTool.TimeFun(time, 0.5f, (ref float t) =>
        {
            if (sdk.gameStatus.status != 1)
            {
                if (spaceTime == t)
                {
                    if (sdk.gameMode.gameMisson._timesPay == 1)
                    {
                        if (sdk.selectRound == 3 && !sdk.ChangeType<LuckyBoyMgr>().isAddConstraint || sdk.selectRound == 5)  //五局制不受影响
                            UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.GameEndGame);
                    }
                    else if (sdk.gameMode.gameMisson._timesPay == 2)
                    {
                        if (sdk.autoSendGift)
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
            //JsonData jsondata = new JsonData();
            //jsondata["orderNo"] = sdk.gameStatus.applyRechargeId;
            //NetMrg.Instance.SendRequest(AndroidMethod.GetPayStatus, jsondata);
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
        //Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SendCatchRecordList, json);
        JsonData jsondata = new JsonData();
        jsondata["list"] = new JsonData(json);
        NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecordList, jsondata);

    }


    //进入试玩
    public override void EnterTryPlay()
    {
        base.EnterTryPlay();
        sdk.gameTryStatus = 2;//进入试玩
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
            Android_Call.UnityCallAndroidHasParameter<int>(AndroidMethod.ShakeWave, 5000);
            Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
        });
#endif
    }

    //试玩结束
    private void TryPlayOver()
    {
        SetMissonValue();
        sdk.gameTryStatus = 3;//试玩结束
        sdk.gameStatus.SetRunStatus(GameRunStatus.GameEnd);
        UIManager.Instance.ShowUI(UIMovePage.NAME, false);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, false);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, false);
        sdk.ChangeType<LuckyBoyMgr>().TryOverEnterGame(GetPayVoiceContent());
    }

    #endregion
}
