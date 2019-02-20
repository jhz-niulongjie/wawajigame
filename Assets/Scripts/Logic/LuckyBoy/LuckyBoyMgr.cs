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
    //一个账号一共支付了几次
    public List<CatchSuccessData> catchlist { get; private set; }
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
        string[] res = result.Split('|');
        pay = Convert.ToInt32(res[0]);
        gameStatus.SetOpenId(res[1]);
        Debug.Log("支付成功--openId::" + res[1]);
        pay++;
        Debug.Log("玩家第几次支付:" + pay);
        JsonData j_data = JsonMapper.ToObject(res[2]);
        catchlist = JsonMapper.ToObject<List<CatchSuccessData>>(j_data.ToJson());
        Debug.Log("玩家已支付次数.Count---" + catchlist.Count);
        if (gameMode.gameMisson != null)
        {
            gameMode.gameMisson.IntiPayTimes(pay);
        }
        gameMode.GameStart();
    }
}
