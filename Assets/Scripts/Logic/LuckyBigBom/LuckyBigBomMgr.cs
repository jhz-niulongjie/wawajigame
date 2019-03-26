using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

public sealed class LuckyBigBomMgr : GameCtr
{
    //进入游戏
    protected override void EnterGame()
    {
        Debug.Log("进入闯关赢礼品");
        if (AppConst.test)
        {
            gameMode = new BigBomCodeMode(this);
            gameMode.EnterGame();
            pass = 3;
        }
        else
        {
            base.EnterGame();
            if (isGame)
            {
                Debug.Log("开启游戏 模式");
                if (selectMode == SelectGameMode.Pay)//codeMode  选择模式
                {
                    gameMode = new BigBomCodeMode(this);
                }
                else
                {
                    gameMode = new TurnQuestionMode(this);
                }
            }
            else
            {
                Debug.Log("不玩游戏 模式");//扫码 或答题直接出娃娃
                gameMode = new GiveUpOnGameMode(this);  //不游戏模式
            }
            gameMode.EnterGame();
        }
    }

    protected override void PaySuccess(JsonData result)
    {
        base.PaySuccess(result);
        gameMode.GameStart();
    }
}
