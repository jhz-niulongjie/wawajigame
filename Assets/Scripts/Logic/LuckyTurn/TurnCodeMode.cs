using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public sealed class TurnCodeMode : GameMode {

    public TurnCodeMode(GameCtr _sdk) : base(_sdk,GameKind.LuckyTurn)
    {
        Debug.Log("////////支付模式\\\\\\\\\\");
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "欢迎进入幸运转转转游戏");
        UIManager.Instance.ShowUI(UITurnSplashPage.NAME, true);

        if (AppConst.test)
        {
            sdk.gameStatus.SetOpenId("123");
            sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);//在游戏中  测试用
            sdk.handleSqlite.DelOverTimeUserFromDataBase();//删除超过时间的礼品碎片
        }
    }
    //未支付进入游戏
    public override void NoPay()
    {
        StartEnterGame();
    }
    public override void GameStart()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIManager.Instance.ShowUI(UITurnSplashPage.NAME, false);
        UIManager.Instance.ShowUI(UITurnCodePage.NAME, false);
        UIManager.Instance.ShowUI(UITurnTablePage.NAME, true);
    }

    public override void EnterGameByStatus()
    {
        base.EnterGameByStatus();
        if (sdk.gameStatus.runStatus == GameRunStatus.GameEnd || sdk.gameStatus.runStatus == GameRunStatus.QRCode)
            StartEnterGame();
        else if (sdk.gameStatus.runStatus == GameRunStatus.NoPay)
            //查询是否支付
            Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, sdk.gameStatus.applyRechargeId, true);
        else if (sdk.gameStatus.runStatus == GameRunStatus.InGame)
        {
            //在游戏中 直接进入游戏
            sdk.ChangeType<LuckyTurnMgr>().GetOnSaleValue();
            GameStart();
        }
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

    //开始进入游戏
    private void StartEnterGame()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.QRCode);
        sdk.gameStatus.SetRemainRound(sdk.selectRound - 1);//剩余局数
        DOVirtual.DelayedCall(2.5f, () => 
        {
            UIManager.Instance.ShowUI(UITurnSplashPage.NAME, false);
            UIManager.Instance.ShowUI(UITurnCodePage.NAME, true, GetPayVoiceContent());
        });
    }
}
