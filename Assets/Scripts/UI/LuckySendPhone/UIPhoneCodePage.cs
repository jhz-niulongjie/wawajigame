using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPhoneCodePage : UIDataBase {

    public const string NAME = "UIPhoneCodePage";
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

    public override void OnShow(object data)
    {
        vc_list = data as List<VoiceContent>;
        if (vc_list == null) vc_list = new List<VoiceContent>();
        GameCtr.Instance.raw = rawImage;
        GetCodeData();
    }
}
