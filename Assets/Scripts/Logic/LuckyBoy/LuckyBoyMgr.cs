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
    
    protected override void EnterGame()
    {
        Debug.Log("进入幸运礼品机");
        if (test)
        {
            Debug.Log("自己测试");
            gameMode = new QuestionMode(this);
            gameMode.EnterGame();
            pass = 3;
            //DOVirtual.DelayedCall(2, () => 
            //{
            //    string result = "0|dfsfdfsdsdfdsfdsfdsfsdf";
            //    PaySuccess(result);
            //});
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

    //支付成功
    protected override void PaySuccess(string result)
    {
        isPaySucess = true;
        isAddConstraint = false;
        string[] res = result.Split('|');
        pay = Convert.ToInt32(res[0]);
        pay++;
        gameStatus.SetOpenId(res[1]);
        Debug.Log("支付成功--openId::"+ res[1] + "玩家第几次支付:" + pay);
        JsonData j_data = JsonMapper.ToObject(res[2]);
        List<CatchSuccessData> paylist = JsonMapper.ToObject<List<CatchSuccessData>>(j_data.ToJson());
        Debug.Log("玩家已支付次数.Count---" + paylist.Count);
        int autoPayTime = 1;//自定义支付次数  默认第一次支付
        if (paylist.Count > 0)
        {
            if (paylist.Count >= 5)
            {
                int winNum = 0;
                for (int i = 0; i < paylist.Count; i++)
                {
                    if (paylist[i].cnum > 0) winNum++;
                }
                Debug.Log("已抓中礼品次数--" + winNum);
                if (winNum >= 3) isAddConstraint = true;
            }
             //上一次支付是否抓中
            if (paylist[paylist.Count - 1].cnum==0) autoPayTime = 2;
        }
        if (gameMode.gameMisson != null)
        {
            if(autoSendGift)
              gameMode.gameMisson.IntiPayTimes(pay);
            else
              gameMode.gameMisson.IntiPayTimes(autoPayTime);
        }
        gameMode.GameStart();
    }
}
