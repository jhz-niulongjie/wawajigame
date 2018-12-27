using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UIBgPage: UIDataBase
{
    public const string NAME = "UIBgPage";
    public override UIShowPos ShowPos
    {
        get
        {
            return UIShowPos.Normal;
        }
    }
    public override HidePage hidePage
    {
        get
        {
            return HidePage.Destory;
        }
    }
    public override AssetFolder assetFolder
    {
        get
        {
            return AssetFolder.LuckyBoy;
        }
    }
}
