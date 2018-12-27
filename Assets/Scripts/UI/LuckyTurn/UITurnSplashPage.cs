using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public sealed class UITurnSplashPage : UIDataBase {

    public const string NAME = "UITurnSplashPage";
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
        get { return AssetFolder.LuckyTurn; }
    }

    public override void Init()
    {
        base.Init();
        GameObject littlePan = CommTool.FindObjForName(gameObject, "littlePan");
        littlePan.transform.DOLocalRotate(new Vector3(0, 0, -360), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        //littlePan = CommTool.FindObjForName(gameObject, "zhuan");
        //littlePan.transform.DOLocalRotate(new Vector3(0, 0, -360), 4,RotateMode.FastBeyond360).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear); ;
    }

    public override void OnShow(object data)
    {
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "欢迎进入幸运转转转游戏");
    }



}


