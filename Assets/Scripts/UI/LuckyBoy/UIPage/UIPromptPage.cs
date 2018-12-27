using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public sealed class UIPromptPage : UIDataBase
{
    public const string NAME = "UIPromptPage";
    public override UIShowPos ShowPos
    {
        get
        {
            return UIShowPos.TipTop;
        }
    }
    public override HidePage hidePage
    {
        get
        {
            return HidePage.Hide;
        }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.LuckyBoy; }
    }
    private GameObject success;
    private GameObject fail;
    private GameObject failDrop;
    private GameObject dropPrompt;
    private GameObject gameEnd;
    private GameObject gameEnd_Game;
    private GameObject gameEnd_Present;
    private GameObject hasboy;
    public override void Init()
    {
        base.Init();
        success = CommTool.FindObjForName(gameObject, "Success");
        fail = CommTool.FindObjForName(gameObject, "Fail");
        failDrop = CommTool.FindObjForName(gameObject, "FailDrop");
        dropPrompt = CommTool.FindObjForName(failDrop, "Prompt");
        gameEnd =CommTool.FindObjForName(gameObject, "GameEnd");
        gameEnd_Game = CommTool.FindObjForName(gameObject, "GameEnd_Game");
        gameEnd_Present = CommTool.FindObjForName(gameObject, "GameEnd_Present");
        hasboy = CommTool.FindObjForName(gameObject, "HasBoy");
        EventHandler.RegisterEvnet(EventHandlerType.ClosePage, ClosePage);
    }

    public override void OnShow(object data)
    {
        base.OnShow(data);
        SuccessFuc(data);
    }

    private void ClosePage(object data)
    {
        UIManager.Instance.ShowUI(NAME, false);
        HideUI();
    }
    private void SuccessFuc(object o)
    {
        HideUI();
        CatchTy cath = (CatchTy)o;
        switch (cath)
        {
            case CatchTy.Catch:
                success.SetActive(true);
                break;
            case CatchTy.CatchErrorPos:
                fail.SetActive(true);
                break;
            case CatchTy.NoCatch:
            case CatchTy.Drop:
                LuckyBoyMgr.Instance.gameMode.gameMisson.DropShowPrompt(dropPrompt,failDrop);
                break;
            case CatchTy.HasBoy:
                hasboy.SetActive(true);
                break;
            case CatchTy.GameEnd:
                gameEnd.SetActive(true);
                break;
            case CatchTy.GameEndGame:
                gameEnd_Game.SetActive(true);
                break;
            case CatchTy.GameEndGift:
                gameEnd_Present.SetActive(true);
                break;
        }
    }

    void HideUI()
    {
        success.SetActive(false);
        fail.SetActive(false);
        failDrop.SetActive(false);
        hasboy.SetActive(false);
        gameEnd.SetActive(false);
        gameEnd_Game.SetActive(false);
        gameEnd_Present.SetActive(false);
    }

    private void OnDestroy()
    {
        EventHandler.UnRegisterEvent(EventHandlerType.ClosePage, ClosePage);
    }
    
}
