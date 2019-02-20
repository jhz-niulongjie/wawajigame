using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TurnQuestionMode : QuestionMode {

    public TurnQuestionMode(GameCtr _sdk) : base(_sdk,GameKind.LuckyTurn)
    {
        sdk.gameStatus.ClearData();//答题进入 转转游戏 不用缓存
        sdk.gameStatus.SetRemainRound(sdk.selectRound - 1);//设置剩余局数
        ////测试数据
        if (GameCtr.test)
        {
            GameCtr.Instance.gameStatus.SetOpenId("123");
            GameCtr.Instance.gameStatus.SetRunStatus(GameRunStatus.InGame);//在游戏中  测试用
        }
    }
    //不进行操作
    public override void SetMissonValue()
    {
    }
    public override void GameStart()
    {
       UIManager.Instance.ShowUI(UITurnTablePage.NAME, true);
    }
}
