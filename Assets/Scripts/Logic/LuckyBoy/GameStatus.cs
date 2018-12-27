using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class GameStatus
{
    public GameRunStatus runStatus { get; private set; }
    public int remainGameRound { get; private set; }
    public bool isDouDong { get; private set; }//是否抖动
    public int payTime { get; private set; }
    public int status { get; private set; }
    public string applyRechargeId { get; private set; }//账单号
    public string openId { get; private set; }//openId
    public string robotId { get; private set; }//小胖Id
    public string reportTime { get; private set; }//抓取时间戳

    public int gameRound { get; private set; }//上次选择的是几局模式

    public SelectGameMode gameMode { get; private set; }//是支付模式 还是答题模式  0是支付模式 1是答题模式

    public GameKind gameKind { get; private set; }//游戏种类

    public GameStatus() { }
    public GameStatus(SelectGameMode _mode, int _roundModel,GameKind _kind)
    {
        SetProDefulat(_mode, _roundModel);
        SetGameKind(_kind);
    }
    public void SetRunStatus(GameRunStatus status)
    {
        runStatus = status;
        SaveData();
    }
    public void SetRemainRound(int num)
    {
        if (num >= 0)
        {
            remainGameRound = num;
            SaveData();
        }

    }
    public void SetIsCatch(int flag)
    {
        status = flag;
        SaveData();
    }
    public void SetIsDouDong(bool flag)
    {
        isDouDong = flag;
        SaveData();
    }
    public void SetOrderNoRobotId(string order, string robotID)
    {
        applyRechargeId = order;
        robotId = robotID;
        SaveData();
    }
    public void SetOpenId(string opId)
    {
        openId = opId;
        SaveData();
    }
    public void SetPayTime(int pay)
    {
        payTime = pay;
        SaveData();
    }
    public void SetRecordTime(string time)
    {
        reportTime = time;
        SaveData();
    }

    public void SetGameKind(GameKind _gameKind)
    {
        gameKind = _gameKind;
        SaveData();
    }
    public void SetProDefulat(SelectGameMode mode = SelectGameMode.Pay, int round = 0)
    {
        runStatus = GameRunStatus.GameEnd;
        status = 0;
        isDouDong = false;
        gameMode = mode;
        gameRound = round;
        remainGameRound = round - 1;
        applyRechargeId = null;
        openId = null;
        robotId = null;
        reportTime = null;
        SaveData();
    }

    public void ClearData()
    {
        if (PlayerPrefs.HasKey(GameCtr.statusKey))
            PlayerPrefs.DeleteKey(GameCtr.statusKey);
    }

    private void SaveData()
    {
        CommTool.SaveClass<GameStatus>(GameCtr.statusKey, this);
    }


}
