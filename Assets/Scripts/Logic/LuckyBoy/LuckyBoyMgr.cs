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
    protected override void EnterGame()
    {
        Debug.Log("进入幸运礼品机");
        if (test)
        {
            Debug.Log("自己测试");
            gameMode = new QuestionMode(this, 5);
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
            string _mode = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.GetGameModeData);
            Debug.Log("----_moldeData----" + _mode);
            if (!string.IsNullOrEmpty(_mode))
            {
                string[] contents = _mode.Split('|');
                if (contents.Length < 5)
                {
                    Debug.LogError("请求模式数量不符");
                    return;
                }
                Debug.Log("contents[0]---" + contents[0]);
                SelectGameMode selectMode = (SelectGameMode)Convert.ToInt32(contents[0]); //选择模式
                Debug.Log("---selectMode------" + selectMode);
                int selectRound = Convert.ToInt32(contents[1]);//选择局数
                question = Convert.ToInt32(contents[2]);//几道题
                pass = Convert.ToInt32(contents[3]);//通过数量
                int isGame = Convert.ToInt32(contents[4]);//是否进行游戏 0是进行 1不进行
                if (isGame == 0)
                {
                    Debug.Log("开启游戏 模式");
                    if (selectMode == SelectGameMode.Pay)//codeMode  选择模式
                    {
                        gameMode = new CodeMode(this, selectRound);
                    }
                    else
                    {
                        gameMode = new QuestionMode(this, selectRound);
                    }
                }
                else
                {
                    Debug.Log("不玩游戏 模式");
                    gameMode = new GiveUpOnGameMode(this, selectMode);  //不游戏模式
                }
                gameMode.EnterGame();
            }
            else
            {
                Debug.Log("模式数据为空");
                Q_AppQuit();
            }
        }

    }
 
    //支付成功
    protected override void PaySuccess(string result)
    {
        base.PaySuccess(result);
        pay++;
        Debug.Log("玩家第几次支付:" + pay);
        if (gameMode.gameMisson != null)
        {
            gameMode.gameMisson.IntiPayTimes(pay);
        }
        gameMode.GameStart();
    }
}
