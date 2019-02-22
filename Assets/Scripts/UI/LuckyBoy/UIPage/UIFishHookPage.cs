using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIFishHookPage : UIDataBase {

    public const string NAME = "UIFishHookPage";
    public override UIShowPos ShowPos { get { return UIShowPos.Normal; } }
    public override HidePage hidePage { get { return HidePage.Destory; } }
    public override AssetFolder assetFolder { get { return AssetFolder.LuckyBoy; } }

    public override void Init()
    {


    }


}
