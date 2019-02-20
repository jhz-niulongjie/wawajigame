using DG.Tweening;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BigBomCodeMode : GameMode {

    public BigBomCodeMode(GameCtr _sdk) : base(_sdk,GameKind.LuckyBigBom)
    {
        
    }

    public override void GameStart()
    {
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIMgr.Instance.ShowUI(UIBigBomCodePage.NAME, false);
        UIMgr.Instance.ShowUI(UIBigBomPage.NAME, true,false);
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

    public override void EnterGameByStatus()
    {
        base.EnterGameByStatus();
        sdk.gameStatus.SetRemainRound(sdk.selectRound - 1);//剩余局数
        UIMgr.Instance.ShowUI(UIBigBomCodePage.NAME, true, GetPayVoiceContent());
    }
}
