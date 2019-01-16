using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPhoneResultPage : UIDataBase {

    public const string NAME = "UIPhoneResultPage";
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.Normal; }
    }
    public override HidePage hidePage
    {
        get { return HidePage.Destory; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.LuckySendPhone; }
    }
    private GameObject success;
    private GameObject fail;
    private GameObject gameEnd;
    private GameObject gameEnd_tryplay;
    private GameObject gameOverOne;
    private GameObject gameOverTwo;
    private GameObject gameOverThree;
    private Text phoneCode;


    public override void Init()
    {
        success=CommTool.FindObjForName(gameObject, "Success");
        fail = CommTool.FindObjForName(gameObject, "Fail");
        gameEnd = CommTool.FindObjForName(gameObject, "GameEnd");
        gameOverOne = CommTool.FindObjForName(gameObject, "GameEnd_One");
        gameOverTwo = CommTool.FindObjForName(gameObject, "GameEnd_Two");
        gameOverThree = CommTool.FindObjForName(gameObject, "GameEnd_Phone");
        gameEnd_tryplay = CommTool.FindObjForName(gameObject, "GameEnd_TryPlay");
        phoneCode = CommTool.GetCompentCustom<Text>(gameObject, "phoneCode");
    }


    public override void OnShow(object data)
    {
        success.SetActive(false);
        fail.SetActive(false);
        gameEnd.SetActive(false);
        gameOverOne.SetActive(false);
        gameOverTwo.SetActive(false);
        gameOverThree.SetActive(false);
        gameEnd_tryplay.SetActive(false);
        CatchTy catchs= (CatchTy)data;
        switch (catchs)
        {
            case CatchTy.Catch:success.SetActive(true); break;
            case CatchTy.Drop:
            case CatchTy.CatchErrorPos:
            case CatchTy.NoCatch: fail.SetActive(true); break;
            case CatchTy.GameEnd: gameEnd.SetActive(true); break;
            case CatchTy.GameOverOne: gameOverOne.SetActive(true); break;
            case CatchTy.GameOverTwo: gameOverTwo.SetActive(true); break;
            case CatchTy.GameOverThree: gameOverThree.SetActive(true);
                phoneCode.text = GameCtr.Instance.ChangeType<LuckySendPhoneMgr>().phoneCode; break;
            case CatchTy.GameOverTryPlay: gameEnd_tryplay.SetActive(true); break;
        }
    }


}
