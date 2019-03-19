using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public sealed class UIRoundEnterPage : UIDataBase
{
    public const string NAME = "UIRoundEnterPage";

    public override UIShowPos ShowPos { get { return UIShowPos.TipTop; } }

    public override HidePage hidePage { get { return HidePage.Hide; } }

    public override AssetFolder assetFolder { get { return AssetFolder.LuckySendPhone; } }

    private Image round;
    private Transform tryPaly;
    string contentSpeak;

    public override void Init()
    {
        base.Init();
        round = CommTool.GetCompentCustom<Image>(gameObject, "round");
        tryPaly = CommTool.FindObjForName(gameObject, "tryPlay").transform;
    }

    public override void OnShow(object data)
    {
        int reulst = (int)data;
        Transform tempTrans = null;
        if (reulst == 0)
        {   //试玩模式
            round.gameObject.SetActive(false);
            tempTrans = tryPaly;
            List<VoiceContent> list = GameCtr.Instance.gameMode.gameMisson.GetVoiceContentBy((int)SendPhoneStatusType.TryPlay, (int)SendPhoneOperateType.TryPlayEnter);
            contentSpeak = list[0].Content;
        }
        else
        {
            tryPaly.gameObject.SetActive(false);
            tempTrans = round.transform;
            round.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UIMain, reulst+"round");
            List<VoiceContent> list = GameCtr.Instance.gameMode.gameMisson.GetVoiceContentBy((int)SendPhoneStatusType.Common, (int)SendPhoneOperateType.RoundEnter);
            if (reulst > 0)// 1 2 3  局
                contentSpeak = list[reulst - 1].Content;
        }
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, contentSpeak);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
        tempTrans.localScale = Vector3.zero;
        tempTrans.gameObject.SetActive(true);
        tempTrans.DOScale(Vector3.one, 2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            EventDispatcher.Dispatch(EventHandlerType.RoundOver);
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
            HideSelf();
        });
    }
}
