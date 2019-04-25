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
    //自定义支付次数
    public int autoPayTime { get; private set; }
    //支付结果
    public JsonData payResult;
    protected override void EnterGame()
    {
        Debug.Log("进入幸运礼品机");
        if (AppConst.test)
        {
            Debug.Log("自己测试");
            isGame = false;
            gameMode = new GiveUpOnGameMode(this);  //不游戏模式
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
    protected override void QRCodeCall(JsonData result)
    {
        if (isGetCode) return;
        isGetCode = true;
        QRCode.ShowCode(raw, result["qrUrl"].ToString());
        orderNumber = result["orderNo"].ToString();
        gameStatus.SetOrderNoRobotId(orderNumber,NetMrg.Instance.robotId);
        gameStatus.SetRunStatus(GameRunStatus.NoPay);
        EventHandler.ExcuteEvent(EventHandlerType.QRCodeSuccess, null);
        JsonData jsondata = new JsonData();
        jsondata["orderNo"] = orderNumber;
        StartCoroutine(CommTool.TimeFun(2, 2, (ref float t) =>
        {
            if (!isPaySucess)
            {
                //检测是否支付
                // Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, orderNumber, false);
                NetMrg.Instance.SendRequest(AndroidMethod.GetPayStatus, jsondata);
            }
            if (t == 0) t = 2;
            return isPaySucess;
        }));
    }


    //支付成功
    protected override void PaySuccess(JsonData result)
    {
        if (gameTryStatus == 2)
        {
            Debug.Log("****试玩中支付成功****");
            payResult = result;
            isPaySucess = true;
            return;//此时在试玩
        }
        isPaySucess = true;
        isAddConstraint = false;
        pay = Convert.ToInt32(result["winningLevel"].ToString());
        pay++;
        openId = result["openId"].ToString();
        gameStatus.SetOpenId(openId);
        Debug.Log("支付成功--openId::" + openId + "-玩家第几次支付:" + pay);
        JsonData j_data = result["data"];
        List<CatchSuccessData> paylist = JsonMapper.ToObject<List<CatchSuccessData>>(j_data.ToJson());
        paylist.Reverse();
        payCount = paylist.Count;
        Debug.Log("玩家已支付次数.Count---" + payCount);
        autoPayTime = 1;//自定义支付次数  默认第一次支付
        if (payCount > 0)
        {
            if (payCount >= 5)//从第6次支付开始 计算是否受限
            {
                int winNum = 0;
                for (int i = 0; i < 5; i++) //只计算前五次
                {
                    if (paylist[i].cnum > 0) winNum++;
                }
                if (winNum >= 3)
                {
                    Debug.Log("****达到受限条件***");
                    isAddConstraint = true;  //中奖次数大于等于3次  从第6次支付开始受限
                }
            }
            if (paylist[payCount - 1].cnum == 0) autoPayTime = 2;//上次支付没有抓中
        }

        if (gameMode.gameMisson != null)
        {
            if (autoSendGift)
                gameMode.gameMisson.IntiPayTimes(pay);
            else
                gameMode.gameMisson.IntiPayTimes(autoPayTime);
        }
        gameTryStatus = 100;//进入游戏
        gameMode.GameStart();
    }

    protected override void Question_Wing(string result)
    {
        if (gameTryStatus == 1) //可以试玩
        {
            isGetCode = false;
            gameStatus.SetRemainRound(3);
            gameMode.EnterTryPlay();
        }
        else if (gameTryStatus == -100)//答题模式
        {
            base.Question_Wing(result);
        }
        if (AppConst.test)
        {
            DOVirtual.DelayedCall(3, () =>
            {
                string aaa = "[{\"cnum\":1,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
                    "{\"cnum\":1,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
                    "{\"cnum\":1,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}," +
                    "{\"cnum\":2,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}]";


                //string aaa = "[{\"cnum\":0,\"openId\":\"ofWtHv8hnh8UFLcqeio9Tb4rVoPU\",\"applyRechargeid\":\"GD1000000000151547446443994\"}]";



                string res = "0|dfsfdfsdsdfdsfdsfdsfsdf|" + aaa;
               // PaySuccess(res);
            });
        }
    }


    private void StartTryPlay(object obj)
    {
        Question_Wing(null);
    }

    //试玩结束是否进入游戏
    public void TryOverEnterGame(List<VoiceContent> listVC)
    {
        if (isPaySucess && payResult!=null)
            PaySuccess(payResult);
        else
        {
            ResetGame();
            ChangeSpeechMode();
            UIManager.Instance.ShowUI(UIMovieQRCodePage.NAME, true, listVC);
        }
    }


}
