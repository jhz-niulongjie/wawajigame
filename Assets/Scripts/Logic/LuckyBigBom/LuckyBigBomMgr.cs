using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class LuckyBigBomMgr : GameCtr
{
    //进入游戏
    protected override void EnterGame()
    {
        Debug.Log("进入闯关赢礼品");
        if (test)
        {
            gameMode = new BigBomCodeMode(this, 3);
            gameMode.EnterGame();
            pass = 3;
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
                        gameMode = new BigBomCodeMode(this, selectRound);
                    }
                    else
                    {
                        gameMode = new TurnQuestionMode(this, selectRound);
                    }
                }
                else
                {
                    Debug.Log("不玩游戏 模式");//扫码 或答题直接出娃娃
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

    protected override void PaySuccess(string result)
    {
        base.PaySuccess(result);
        gameMode.GameStart();
    }
}
