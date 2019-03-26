using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public sealed class GiveUpOnGameMode : GameMode
{
    List<Q_Question> q_question = null;
    private Action outPresent = null;
    public GiveUpOnGameMode(GameCtr sdk) : base(sdk)
    {
        outPresent = OutPresent;
    }

    public override void EnterGame()
    {
        if (sdk.selectMode == SelectGameMode.Pay)
            StartEnterPay();
        else
            EnterQuestion();
    }
    public override void GameStart()
    {
        //支付成功出礼品
        EventHandler.ExcuteEvent(EventHandlerType.StopCoroutine, null);
        OutPresent();
    }

    public override List<VoiceContent> GetPayVoiceContent()
    {
        List<VoiceContent> vctlist = base.GetPayVoiceContent();
        if (vctlist != null)
            return vctlist.FindAll(v => v.Type == "2");//不游戏
        return null;
    }

    public override void Clear()
    {
        base.Clear();
        outPresent = null;
        if (q_question != null) q_question.Clear();
    }



    //进入答题
    public void EnterQuestion()
    {
        StartUpRecordList_Question();
        sdk.Q_startCarwTime = CommTool.GetTimeStamp();
        q_question = Get_QuestionDefault();
        if (q_question != null)
        {
            sdk.RegHeadAction(() => EventHandler.ExcuteEvent(EventHandlerType.HeadPress, false));
            if (sdk.gameStatus != null)
                sdk.gameStatus.SetRunStatus(GameRunStatus.Question);
            object[] obs = { q_question, outPresent, false };
            UIManager.Instance.ShowUI(UIQuestionPage.NAME, true, obs);
        }
        else
            Debug.LogError("问题库没有数据");
    }

    //进入支付
    public void StartEnterPay()
    {
        StartUpRecordList_Pay();
        EnterPay();
    }



    /// <summary>
    /// 批量上报记录
    /// </summary>
    private void StartUpRecordList_Pay()
    {
        List<C_RecordData> list = sdk.handleSqlite.C_ReadData();
        if (list == null || list.Count == 0)
        {
            Debug.Log("record表为空不需上报");
            return;
        }
        //string json = JsonMapper.ToJson(list);
        //Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SendCatchRecordList, json);
        JsonData jsondata = new JsonData();
        jsondata["list"] = new JsonData(list);
        NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecordList, jsondata);
    }


    /// <summary>
    /// 批量上报记录
    /// </summary>
    private void StartUpRecordList_Question()
    {
        List<C_RecordData> list = sdk.handleSqlite.Q_ReadData();
        if (list == null || list.Count == 0)
        {
            Debug.Log("record表为空不需上报");
            return;
        }
        //string json = JsonMapper.ToJson(list);
        //Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SendCatchRecordList, json);
        JsonData jsondata = new JsonData();
        jsondata["list"] = new JsonData(list);
        NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecordList, jsondata);
    }

    //开始进入游戏
    private void EnterPay()
    {
#if UNITY_ANDROID
        UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, true, GetPayVoiceContent(), o =>
        {
           //播放载入语音
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                             "欢迎您来到扫码赢礼品游戏");
            Android_Call.UnityCallAndroidHasParameter<int>(AndroidMethod.ShakeWave, 5000);
            Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, 5000);
        });
#endif
    }

    //出礼品
    private void OutPresent()
    {
        sdk.gameStatus.SetIsCatch(1);
         LuckyBoyMgr.Instance.startCarwTime = CommTool.GetTimeStamp();
        if (sdk.selectMode == SelectGameMode.Pay)
        {
            // Android_Call.UnityCallAndroidHasParameter<bool, string>(AndroidMethod.SendCatchRecord,
            //  true,LuckyBoyMgr.Instance.startCarwTime);
            JsonData jsondata = new JsonData();
            jsondata["status"] = 1;
            jsondata["reportTime"] = LuckyBoyMgr.Instance.startCarwTime;
            jsondata["openId"] = GameCtr.Instance.openId;
            jsondata["applyRechargeId"] = GameCtr.Instance.orderNumber;
            NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecord, jsondata);
        }
        else
        {
           // Android_Call.UnityCallAndroidHasParameter<bool, string, string>(AndroidMethod.Q_UpRecord,
             // true, LuckyBoyMgr.Instance.startCarwTime, LuckyBoyMgr.Instance.Q_startCarwTime);

            JsonData jsondata = new JsonData();
            jsondata["status"] = 1;
            jsondata["reportTime"] = LuckyBoyMgr.Instance.startCarwTime;
            jsondata["openId"] = "ANS" + LuckyBoyMgr.Instance.Q_startCarwTime;
            jsondata["applyRechargeId"] ="ANS"+ LuckyBoyMgr.Instance.Q_startCarwTime;
            NetMrg.Instance.SendRequest(AndroidMethod.SendCatchRecord, jsondata);

        }
        UIManager.Instance.ShowUI(UIPromptPage.NAME, true, CatchTy.Catch);
        Android_Call.UnityCallAndroid(AndroidMethod.AutoPresent);//自动出礼物
        AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.shengli, false);//播放胜利音效
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
        VoiceContent vc = GetVoiceContent(QuestionVoiceType.Get_Present);
        string[] contents = vc.Content.Split('|');
        int rangeIndex = UnityEngine.Random.Range(0, contents.Length);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, contents[rangeIndex]);
        EffectMrg.ShowEffect(); //播放特效
        float winTime = Convert.ToSingle(vc.Time) + 2f;
        Debug.Log("winTime::"+winTime);
        sdk.StartCoroutine(CommTool.TimeFun(winTime, winTime, null, () =>
        {
            Debug.Log("退出啊::");
            UIManager.Instance.ShowUI(UIPromptPage.NAME, false);
            sdk.AppQuit();
        }));
    }
}
