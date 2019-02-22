using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMode
{
    public GameCtr sdk { get; private set; }
    //选择的游戏局数对象
    public GameMisson gameMisson { get; protected set; }
    //选择的游戏种类
    public GameKind selectgameKind { get; private set; }
    //语音数据
    public Q_Voice_ScriptObj qvs { get; private set; }
    //上局是否抓中
    public bool lastRoundIsSuccess { get; private set; }

    protected GameMode(GameCtr _sdk,GameKind _gameKind=GameKind.LuckyBoy)
    {
        sdk = _sdk;
        selectgameKind = _gameKind;
        qvs = VoiceMrg<Q_Voice_ScriptObj, ExtendContent>.GetVoiceFromAsset("QuestionVoice");
        GetGameStatusData();
    }
    #region 虚函数
    /// <summary>
    /// 设置几局游戏
    /// </summary>
    public virtual void SetMissonValue() { }
    /// <summary>
    ///  游戏入口
    /// </summary>
    public virtual void EnterGame()
    {
        StartUpRecordList();
        EnterGameByStatus();
    }
    /// <summary>
    /// 通过存在状态进入游戏
    /// </summary>
    public virtual void EnterGameByStatus()
    {
        if (sdk.gameStatus == null)
        {
            Debug.Log("--sdk.gameStatus--是 null");
            return;
        }
        Debug.Log("游戏类型--" + sdk.gameStatus.gameKind + "--游戏处于状态---" + sdk.gameStatus.runStatus +
           "----剩余局数---" + sdk.gameStatus.remainGameRound + "---已出礼品=="
           + sdk.gameStatus.status + "---上次是第几次支付--" + sdk.gameStatus.payTime);
    }
    /// <summary>
    /// 游戏正式开始
    /// </summary>
    public virtual void GameStart() { }
    
    /// <summary>
    /// 未支付
    /// </summary>
    public virtual void NoPay() { }

    /// <summary>
    /// 上传抓取记录
    /// </summary>
    ///<param name="isSuccess"></param>
    public virtual void UpRecord(bool isSuccess)
    {
        lastRoundIsSuccess = isSuccess;
    }
    /// <summary>
    /// 上传记录集合
    /// </summary>
    public virtual void StartUpRecordList() { }

    /// <summary>
    ///  显示游戏结束面板
    /// </summary>
    /// <param name="gamePlay"></param>
    public virtual void ShowEndUI(GameMisson gamePlay) { }

    /// <summary>
    /// 进入游戏试玩
    /// </summary>
    public virtual void EnterTryPlay() { }

    /// <summary>
    /// 获得支付页面语音
    /// </summary>
    /// <returns></returns>
    public virtual List<VoiceContent> GetPayVoiceContent()
    {
        string q_result= Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.GetPayPageVoice);
        if (!string.IsNullOrEmpty(q_result))
        {
            JsonData j_data = JsonMapper.ToObject(q_result);
            return JsonMapper.ToObject<List<VoiceContent>>(j_data["plist"].ToJson());
        }
        return null;
    }

    /// <summary>
    /// 清空
    /// </summary>
    public virtual void Clear()
    {
        if (qvs != null)
            qvs.q_v_list.Clear();
        if (gameMisson != null)
            gameMisson.Clear();
    }



    #endregion




    #region 实例成员
    /// <summary>
    /// 获得当前游戏状态
    public void GetGameStatusData()
    {
        sdk.gameStatus = CommTool.LoadClass<GameStatus>(GameCtr.statusKey);
        Debug.Log("sdk.gameStatus---0:" + sdk.gameStatus);
        if (sdk.gameStatus != null)
        {
            if (sdk.gameStatus.gameRound != sdk.selectRound || sdk.gameStatus.gameMode != sdk.selectMode
                || sdk.gameStatus.gameKind!=selectgameKind)//上次选择的模式不同
            {
                sdk.gameStatus.ClearData();
                if (sdk.gameStatus.gameMode != sdk.selectMode)//游戏模式不同 清空表数据
                    sdk.handleSqlite.DeleteData(HandleSqliteData.recordTable);
                sdk.gameStatus = null;
            }
        }
        if(sdk.gameStatus==null)
            sdk.gameStatus = new GameStatus(sdk.selectMode, sdk.selectRound,selectgameKind);
        Debug.Log("sdk.gameStatus---1:"+ sdk.gameStatus);
    }

    //请求考题
    protected List<Q_Question> Get_QuestionDefault()
    {
        string q_result= Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.GetQuestionAnswer);
        if (string.IsNullOrEmpty(q_result))
        {
            Q_Library_ScriptObj qs = VoiceMrg<Q_Library_ScriptObj, ExtendContent>.GetVoiceFromAsset("QuestionLibrary");
            return qs.question_list;
        }
        else
        {
            JsonData j_data = JsonMapper.ToObject(q_result);
            Debug.Log("code-" + j_data["code"]);
            int code = (int)j_data["code"];
            if (code == 0)//表示没有库  
            {
                AppQuit(QuestionVoiceType.Library_Empty);
                return null;
            }
            else if (code == 1)//表示有库但是空的 需要读默认
            {
                Q_Library_ScriptObj qs = VoiceMrg<Q_Library_ScriptObj, ExtendContent>.GetVoiceFromAsset("QuestionLibrary");
                return qs.question_list;
            }
            else  //有数据
            {
                return JsonMapper.ToObject<List<Q_Question>>(j_data["gameAnswer"].ToJson());
            }
        }

    }
    /// <summary>
    /// 得到
    /// </summary>
    /// <param name="qvt"></param>
    /// <returns></returns>
    protected VoiceContent GetVoiceContent(QuestionVoiceType qvt)
    {
        return qvs.q_v_list.Find(c => c.Type == qvt.ToString());
    }
    /// <summary>
    /// 游戏 无网  无库 退出
    /// </summary>
    /// <param name="qvt"></param>
    protected void AppQuit(QuestionVoiceType qvt)
    {
        VoiceContent vc = GetVoiceContent(qvt);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, vc.Content);
        int quit_time = Convert.ToInt32(vc.Time);
        sdk.StartCoroutine(CommTool.TimeFun(quit_time, quit_time, null, sdk.Q_AppQuit));
    }
    #endregion
}
