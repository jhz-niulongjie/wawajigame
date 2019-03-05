using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class QuestionMode : GameMode
{
    Action startGame = null;
    Action startQuestion = null;
    List<Q_Question> q_question = null;
    public QuestionMode(GameCtr _sdk,GameKind _gameKind=GameKind.LuckyBoy) : base(_sdk,_gameKind)
    {
        Debug.Log("//////答题模式\\\\\\\\");
        sdk.gameTryStatus = -100;//答题模式
        sdk.Q_startCarwTime = CommTool.GetTimeStamp();
        //开始游戏
        startGame = () =>
        {
            UIManager.Instance.ShowUI(UIQuestionPage.NAME, false);
            SetMissonValue();
            GameStart();
        };
        //开始答题
        startQuestion = () =>
        {
            //sdk.RegHeadAction(() => EventHandler.ExcuteEvent(EventHandlerType.HeadPress, false));
            sdk.gameStatus.SetRunStatus(GameRunStatus.Question);
            object[] obs = { q_question, startGame, true };
            UIManager.Instance.ShowUI(UIQuestionPage.NAME, true, obs);
        };
    }
    #region 重写方法

    public override void SetMissonValue()
    {
        if (sdk.selectRound == 3)
            gameMisson = new Q_ThreeRoundPlay(sdk);
        else
            gameMisson = new Q_FiveRoundPlay(sdk);
    }


    public override void EnterGame()
    {
        q_question = Get_QuestionDefault();
        if (q_question != null)
        {
            base.EnterGame();
        }
    }

    public override void GameStart()
    {
        gameMisson.IntiPayTimes(1);//答题模式始终显示 第一次玩
        sdk.gameStatus.SetRunStatus(GameRunStatus.InGame);
        UIManager.Instance.ShowUI(UIFishHookPage.NAME, true);
        UIManager.Instance.ShowUI(UIMovePage.NAME, true);
        UIManager.Instance.ShowUI(UITimePage.NAME, true);
    }

    public override void UpRecord(bool isSuccess)
    {
        base.UpRecord(isSuccess);
        LuckyBoyMgr.Instance.startCarwTime = CommTool.GetTimeStamp();
        Android_Call.UnityCallAndroidHasParameter<bool, string, string>(AndroidMethod.Q_UpRecord,
              isSuccess, LuckyBoyMgr.Instance.startCarwTime, LuckyBoyMgr.Instance.Q_startCarwTime);
    }

    public override void ShowEndUI(GameMisson gamePlay)
    {
        VoiceContent tVC = gamePlay.GetVoiceContent(gamePlay._Count - 2).Content;
        int time = Convert.ToInt32(tVC.Time);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, tVC.Content);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
        bool isEnd = false;
        sdk.RegHeadAction(() => isEnd = true);
        sdk.StartCoroutine(CommTool.TimeFun(time, 0.5f, (ref float t) =>
        {
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
        if (sdk.gameStatus.runStatus == GameRunStatus.GameEnd || sdk.gameStatus.runStatus == GameRunStatus.Question)
        {
            if (startQuestion != null)
            {
                startQuestion();
                startQuestion = null;
            }
        }
        else if (sdk.gameStatus.runStatus == GameRunStatus.InGame)
        {
            if (startGame != null)
            {
                startGame();
                startGame = null;
            }
        }
    }
    /// <summary>
    /// 批量上报记录
    /// </summary>
    public override void StartUpRecordList()
    {
        List<C_RecordData> list = sdk.handleSqlite.Q_ReadData();
        if (list == null || list.Count == 0)
        {
            Debug.Log("record表为空不需上报");
            return;
        }
        string json = JsonMapper.ToJson(list);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SendCatchRecordList, json);
    }
    public override void Clear()
    {
        base.Clear();
        startGame = null;
        startQuestion = null;
        if (q_question != null) q_question.Clear();
    }
    #endregion
}
