using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LuckySendPhoneMgr : GameCtr {

    protected override void EnterGame()
    {
        Debug.Log("进入幸运送平板游戏");
        gameMode = new SendPhoneMode(this, 3);
        gameStatus.SetRemainRound(3);
        gameMode.EnterGame();
    }
}
