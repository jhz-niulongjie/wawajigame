using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SendPhoneMode : GameMode
{
    public SendPhoneMode(GameCtr _sdk, int misson) : base(_sdk, SelectGameMode.Pay, misson)
    {
        Debug.Log("开始试玩");
    }

    public override void SetMissonValue()
    {
        gameMisson = new Phone_ThreeRoundPlay(sdk);
    }

    public override void EnterGame()
    {
        SetMissonValue();
        EnterTryPlay();
        //base.EnterGame();
    }




    //进入游戏
    public override void EnterGameByStatus()
    {
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
    public override void GameStart()
    {
        if (sdk.mainObj) sdk.mainObj.SetActive(true);
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIManager.Instance.ShowUI(UIPhoneCodePage.NAME, false);
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
            tVC = gamePlay.GetVoiceContent(gamePlay._Count - 1).Content;
            time = Convert.ToInt32(tVC.Time);
        }
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
        int spaceTime = time - 2;
        bool isEnd = false;
        sdk.RegHeadAction(() => isEnd = true);
        sdk.StartCoroutine(CommTool.TimeFun(time, 0.5f, (ref float t) =>
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
            if (isEnd)
            {
                sdk.AppQuit();
            }
            return isEnd;
        }, sdk.AppQuit));//游戏推出
    }

    #region 私有方法

    //开始进入游戏
    private void StartEnterGame()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.QRCode);
#if UNITY_ANDROID
        UIManager.Instance.ShowUI(UIPhoneCodePage.NAME, true, GetPayVoiceContent(), o =>
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

    //进入试玩
    private void EnterTryPlay()
    {
        GameStart();
    }

    #endregion
}
