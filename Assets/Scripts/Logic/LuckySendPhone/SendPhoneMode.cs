using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SendPhoneMode : GameMode
{
    bool isTryPlay = false;
    public SendPhoneMode(GameCtr _sdk) : base(_sdk,GameKind.LuckySendPhone)
    {
        Debug.Log("开始试玩");
        //注册试玩结束
        EventDispatcher.AddListener(EventHandlerType.TryPlayOver, TryPlayOver);
        //注册抓娃娃动画播放完成
        EventDispatcher.AddListener(EventHandlerType.MoviePlayOver, MoviePlayOver);
    }

    public override void SetMissonValue()
    {
        gameMisson = new Phone_ThreeRoundPlay(sdk);
    }

    public override void EnterGame()
    {
        //显示抓娃娃动画
        UIManager.Instance.ShowUI(UIPhoneAnimPage.NAME, true);
    }

    //进入游戏
    public override void EnterGameByStatus()
    {
        if (sdk.gameStatus.runStatus == GameRunStatus.GameEnd || sdk.gameStatus.runStatus == GameRunStatus.QRCode)
            StartEnterGame();
        else if (sdk.gameStatus.runStatus == GameRunStatus.NoPay)
        {
            //查询是否支付
            // Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, sdk.gameStatus.applyRechargeId, true);
            JsonData jsondata = new JsonData();
            jsondata["orderNo"] = sdk.gameStatus.applyRechargeId;
            NetMrg.Instance.SendRequest(AndroidMethod.GetPayStatus, jsondata);
        }
        else if (sdk.gameStatus.runStatus == GameRunStatus.InGame)
        {
            GameStart();
        }
    }
    public override void GameStart()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIManager.Instance.ShowUI(UIPhoneCodePage.NAME, false);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, true);
        UIManager.Instance.ShowUI(UIMovePage.NAME, true);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, true, isTryPlay);
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
       // Android_Call.UnityCallAndroidHasParameter<bool, string>(AndroidMethod.SendCatchRecord,
              //  isSuccess, LuckyBoyMgr.Instance.startCarwTime);

        JsonData jsondata = new JsonData();
        jsondata["status"] = isSuccess ? 1 : 0;
        jsondata["reportTime"] = LuckyBoyMgr.Instance.startCarwTime;
        jsondata["openId"] = GameCtr.Instance.openId;
        jsondata["applyRechargeId"] = GameCtr.Instance.orderNumber;
        NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecord, jsondata);

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
    public override List<VoiceContent> GetPayVoiceContent()
    {
        List<VoiceContent> vctlist = this.gameMisson.GetVoiceContentBy((int)SendPhoneStatusType.Code, (int)SendPhoneOperateType.Code);
        return vctlist;
    }

    public override void ShowEndUI(GameMisson gamePlay)
    {
        List<VoiceContent> tVC = null;
        float time_ = 0;
        if (gamePlay._Count == 3)//这次抓中三次
        {
            if (gamePlay.signTimes == 0)//标记0次
            {
                UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.GameOverOne);
                tVC = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.OnePayEnter, (int)SendPhoneOperateType.GameOver);
                time_ = tVC[0].Content.Length * AppConst.speakTime;
            }
            else if (gamePlay.signTimes == 1)
            {
                UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.GameOverTwo);
                tVC = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.TwoPayEnter, (int)SendPhoneOperateType.GameOver);
                time_ = tVC[0].Content.Length * AppConst.speakTime;
            }
            else if (gamePlay.signTimes == 2)
            {
                UIManager.Instance.ShowUI(UIPhoneResultPage.NAME, true, CatchTy.GameOverThree);
                tVC = gamePlay.GetVoiceContentBy((int)SendPhoneStatusType.ThreePayEnter, (int)SendPhoneOperateType.GameOver);
                time_ = 60 * 10;
            }
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC[0].Content);
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
            bool isEnd = false;
            sdk.RegHeadAction(() => isEnd = true);
            sdk.StartCoroutine(CommTool.TimeFun(time_, 0.5f, (ref float t) =>
            {
                if (isEnd)
                {
                    sdk.AppQuit();
                }
                return isEnd;
            }, sdk.AppQuit));//游戏推出
        }
        else
            sdk.AppQuit();
    }

    #region 私有方法

    //开始进入游戏
    private void StartEnterGame()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.QRCode);
        UIManager.Instance.ShowUI(UIPhoneCodePage.NAME, true, GetPayVoiceContent());
    }

    //进入试玩
    private void EnterTryPlay()
    {
        isTryPlay = true;
        sdk.gameStatus.SetRunStatus(GameRunStatus.GameEnd);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, true);
        UIManager.Instance.ShowUI(UIMovePage.NAME, true);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, true, true);
    }
    //试玩结束
    private void TryPlayOver()
    {
        isTryPlay = false;
        sdk.gameStatus.SetRemainRound(2);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, false);
        UIManager.Instance.ShowUI(UIMovePage.NAME, false);
        UIManager.Instance.ShowUI(UIPhoneTimePage.NAME, false);
        base.EnterGame();
    }
    //视频播放完成
    private void MoviePlayOver()
    {
        SetMissonValue();
        EnterTryPlay();
    }
    #endregion
}
