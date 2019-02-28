using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LitJson;
using DG.Tweening;

public sealed class LuckyBoyMgr : GameCtr
{
    //是否添加约束 只对三局制有效
    public bool isAddConstraint { get; private set; }
    //已支付的次数
    public int payCount { get; private set; }
    protected override void EnterGame()
    {
        Debug.Log("进入幸运礼品机");
        if (test)
        {
            Debug.Log("自己测试");
            PlayerPrefs.DeleteAll();
            gameMode = new CodeMode(this);
            gameMode.EnterGame();
            EventHandler.RegisterEvnet(EventHandlerType.StartTryPlay, StartTryPlay);
        }
        else
        {
            base.EnterGame();
            if (isGame)
            {
                Debug.Log("开启游戏 模式");
                if (selectMode == SelectGameMode.Pay)//codeMode  选择模式
                {
                    gameMode = new CodeMode(this);
                }
                else
                {
                    gameMode = new QuestionMode(this);
                }
            }
            else
            {
                Debug.Log("不玩游戏 模式");
                gameMode = new GiveUpOnGameMode(this);  //不游戏模式
            }
            gameMode.EnterGame();
        }

    }

    //二维码获得成功
    protected override void QRCodeCall(string result)
    {
        if (isGetCode) return;
        isGetCode = true;
        string[] msgs = result.Split('|');
        QRCode.ShowCode(raw, msgs[0]);
        gameStatus.SetOrderNoRobotId(msgs[1], msgs[2]);
        gameStatus.SetRunStatus(GameRunStatus.NoPay);
        EventHandler.ExcuteEvent(EventHandlerType.QRCodeSuccess, null);
        StartCoroutine(CommTool.TimeFun(2, 2, (ref float t) =>
        {
            if (gameTryStatus == 2)//处于试玩状态
            {
                return true;//结束查询是否支付
            }
            if (!isPaySucess)
            {
                //检测是否支付
                Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, msgs[1], false);
            }
            if (t == 0) t = 2;
            return isPaySucess;
        }));
    }


    //支付成功
    protected override void PaySuccess(string result)
    {
        if (gameTryStatus == 2) return;//此时在试玩
        isPaySucess = true;
        isAddConstraint = false;
        string[] res = result.Split('|');
        pay = Convert.ToInt32(res[0]);
        pay++;
        gameStatus.SetOpenId(res[1]);
        Debug.Log("支付成功--openId::" + res[1] + "-玩家第几次支付:" + pay);
        JsonData j_data = JsonMapper.ToObject(res[2]);
        List<CatchSuccessData> paylist = JsonMapper.ToObject<List<CatchSuccessData>>(j_data.ToJson());
        payCount = paylist.Count;
        Debug.Log("玩家已支付次数.Count---" + payCount);
        int autoPayTime = 1;//自定义支付次数  默认第一次支付
        if (payCount > 0)
        {
            if (payCount >= 5)//支付大于5次
            {
                int winNum = 0;
                for (int i = 0; i < paylist.Count; i++)
                {
                    if (paylist[i].cnum > 0) winNum++;
                }
                Debug.Log("已抓中礼品次数--" + winNum);
                if (winNum >= 3) isAddConstraint = true;  //中奖次数大于等于3次 受限
            }
            // 最后一次是第一个  上一次支付是否抓中 等于0 上次支付没抓中 进入中等难度
            if (paylist[0].cnum == 0) autoPayTime = 2;
        }
        if (gameMode.gameMisson != null)
        {
            if (isAddConstraint)
                gameMode.gameMisson.IntiPayTimes(1);//最高难度
            else
            {
                if (autoSendGift)
                    gameMode.gameMisson.IntiPayTimes(pay);
                else
                    gameMode.gameMisson.IntiPayTimes(autoPayTime);
            }
        }
        gameTryStatus = 100;
        gameMode.GameStart();
    }

    protected override void Question_Wing(string result)
    {
        if (gameTryStatus == 1) //可以试玩
        {
            isGetCode = false;
            gameTryStatus = 2;
            gameStatus.SetRemainRound(3);
            gameMode.EnterTryPlay();
        }
        if (test)
        {
            //string aaa = "[{\"cnum\":0,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
            //    "{\"cnum\":0,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
            //    "{\"cnum\":1,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
            //    "{\"cnum\":2,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
            //    "{\"cnum\":1,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}]";

            string aaa = "[{\"cnum\":0,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
                "{\"cnum\":0,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}]";


            string res = "0|dfsfdfsdsdfdsfdsfdsfsdf|" + aaa;
            PaySuccess(res);
        }
    }


    private void StartTryPlay(object obj)
    {
        Question_Wing(null);
    }

}
