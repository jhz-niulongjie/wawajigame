using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public sealed class UIBigBomResultPage : UIViewBase {

    public const string NAME = "UIBigBomResultPage";
    public override UIShowPos _showPos { get { return UIShowPos.TipTop; } }
    public override HidePage _hidePage { get { return HidePage.Hide; } }
    public override AssetFolder _assetFolder { get { return AssetFolder.LuckyBigBom; } }

    private Text isSuccess;
    private Transform image;

    protected override void OnInit()
    {
        isSuccess = CommTool.GetCompentCustom<Text>(gameObject,"isSuccess");
        image = CommTool.FindObjForName(gameObject, "Image").transform;
    }

    protected override void DoTweenAnimEnter()
    {
        image.transform.localScale = Vector3.zero;
        image.DOLocalRotate(new Vector3(0, 0, -720), 0.5f,RotateMode.LocalAxisAdd);
        image.DOScale(Vector3.one, 0.5f);
    }

    protected override void DoTweenAnimExit(Tweener tweener = null, TweenCallback callback = null)
    {
        image.DOLocalRotate(new Vector3(0, 0, 720), 0.5f, RotateMode.LocalAxisAdd);
        var tw = image.DOScale(Vector3.zero, 0.5f);

        base.DoTweenAnimExit(tw);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        object[] dts = _Data as object[];
        if (dts.Length > 1)
        {
            int level = Convert.ToInt32(dts[0]);
            bool success = (bool)dts[1];
            bool trypaly = (bool)dts[2];
            LuckyTurnVoiceType ltv = (LuckyTurnVoiceType)dts[3];
            ShowUITable(level, success, trypaly,ltv);
            DOVirtual.DelayedCall(3.5f, () =>
            {
                EventDispatcher.Dispatch(EventHandlerType.GameEndStart);
                HideSelf();
            });
        }
    }
   

    private void ShowUITable(int level,bool success,bool tryplay, LuckyTurnVoiceType lvt)
    {
        if (!tryplay)
        {
            if (level != 3)
            {
                isSuccess.text = success ? "闯关成功" : "闯关失败";
            }
            else
            {
                Debug.Log("获得的概率类型--"+lvt);
                success = lvt == LuckyTurnVoiceType.SupriseGift ? true : false;
                isSuccess.text = success ? "金库充盈，获得神秘礼品一份" : "金库亏空，没有有价值的礼品";
            }
        }
        else //试玩
        {
            isSuccess.text = "试玩结束";
        }
    }
}
