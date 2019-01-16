using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public sealed class UIPhoneAnimPage : UIDataBase {

    public const string NAME = "UIPhoneAnimPage";
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

    private VideoPlayer vplayers;

    public override void Init()
    {
        base.Init();
        vplayers = CommTool.GetCompentCustom<VideoPlayer>(gameObject, "movie");
    }

    public override void OnShow(object data)
    {
        //Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "欢迎进入幸运赢平板游戏");
        vplayers.loopPointReached += MovieOvers;
        vplayers.Play();
    }

    private void MovieOvers(VideoPlayer p)
    {
        EventDispatcher.Dispatch(EventHandlerType.MoviePlayOver);
        HideSelf();
    }
}
