using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class UIGameOverImagePage : UIDataBase {

    public const string NAME = "UIGameOverImagePage";
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.TipTop; }
    }
    public override HidePage hidePage
    {
        get { return HidePage.Destory; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.Common; }
    }

    private RawImage rawIamge;

    public override void Init()
    {
        rawIamge = CommTool.GetCompentCustom<RawImage>(gameObject, "Image");
    }

    public override void OnShow(object data)
    {
        rawIamge.texture = GameCtr.Instance.overTexture;
        rawIamge.SetNativeSize();
    }

}
