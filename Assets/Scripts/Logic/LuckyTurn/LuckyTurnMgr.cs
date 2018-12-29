using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using LitJson;

public sealed class LuckyTurnMgr : GameCtr
{
    //优惠券列表
    public List<VoiceContent> listOnSaleNumber { get; private set; }
    //是否支付进入
    public bool codeEnter { get; private set; }//是true支付进入  false答题进入
    //进入游戏
    protected override void EnterGame()
    {
        Debug.Log("进入幸运转转转");
        if (test)
        {
            Debug.Log("自己测试");
            GetOnSaleValue();
            codeEnter = true;
            gameMode = new TurnCodeMode(this, 3);
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
                        codeEnter = true;
                        gameMode = new TurnCodeMode(this, selectRound);
                    }
                    else
                    {
                        codeEnter = false;
                        GetOnSaleValue();
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
        handleSqlite.DelOverTimeUserFromDataBase();//删除超过时间的礼品碎片
        if (gameMode.isPlayGame != SelectGameMode.NoGame)//不玩游戏不要优惠券数据
        {
            GetOnSaleValue();
        }
        gameMode.GameStart();
    }


    //获得优惠券数据
    public void GetOnSaleValue()
    {
        string onsaleJson = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.GetOnSaleNumberData);
        if (test)
        {
            listOnSaleNumber = new List<VoiceContent> { new VoiceContent { Content="我我我我我我我我", Type="4" },
            new VoiceContent { Content = "我我我我我我我我", Type = "1" },
            new VoiceContent { Content = "我我我我我我我我", Type = "2" },
            new VoiceContent { Content = "我我我我我我我我", Type = "3" }, };
        }
        Debug.Log("---onsaleJson===" + onsaleJson);
        if (!string.IsNullOrEmpty(onsaleJson))
        {
            JsonData j_data = JsonMapper.ToObject(onsaleJson);
            //type的值 及是id值
            listOnSaleNumber = JsonMapper.ToObject<List<VoiceContent>>(j_data["plist"].ToJson());
        }
    }
}
