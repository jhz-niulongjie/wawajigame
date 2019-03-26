using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public sealed class UICodePageNoDied : UIMovieQRCodePage {

    public override HidePage hidePage
    {
        get { return HidePage.Hide; }
    }

    private bool isFirstEnter = true;

    public override void OnShow(object data)
    {
        Debug.Log("进入支付页面不退出模式");
        vc_lists = data as List<VoiceContent>;
        if (vc_lists == null) vc_lists = new List<VoiceContent>();
        qrCode.SetActive(false);
        vplayers.gameObject.SetActive(true);
        sdk.raw = raw;
        if (isFirstEnter)
        {
            isFirstEnter = false;
            PlayMovies();
        }
        else
        {
            qrCode.SetActive(true);
            vplayers.gameObject.SetActive(false);
            QRCodeSuccess(null);
            GameCtr.Instance.gameTryStatus = 1;//继续试玩
        }
    }

    public override IEnumerator PlayVoiceIe()
    {
        yield break;
    }

}
