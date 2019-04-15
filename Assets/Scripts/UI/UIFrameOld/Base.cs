using DG.Tweening;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Base : MonoBehaviour, I_Code
{

    #region 支付数据
    public GameObject anim { get; set; }

    public GameObject code { get; set; }

    public VideoPlayer vplayer { get; set; }

    public GameObject loading { get; set; }

    public RawImage rawImage { get; set; }

    public Text money_text { get; set; }

    public Text times_text { get; set; }

    public List<VoiceContent> vc_list { get; set; }

    public virtual void InitCodeData()
    {
        anim = CommTool.FindObjForName(gameObject, "animation");
        code = CommTool.FindObjForName(gameObject, "code");
        vplayer = CommTool.GetCompentCustom<VideoPlayer>(gameObject, "movie");
        loading = CommTool.FindObjForName(gameObject, "loading");
        rawImage = CommTool.GetCompentCustom<RawImage>(gameObject, "RawImage");
        money_text = CommTool.GetCompentCustom<Text>(gameObject, "money");
        times_text = CommTool.GetCompentCustom<Text>(gameObject, "times");
    }

    public virtual void PlayMovie()
    {
        vplayer.loopPointReached += MovieOver;
        vplayer.Play();
    }

    public virtual void MovieOver(VideoPlayer p)
    {
        anim.SetActive(false);
        code.SetActive(true);
        GetCodeData();
    }
    //获得二维码数据
    public virtual void GetCodeData()
    {
        JsonData jsondata = new JsonData();
        jsondata["flag"] = GameCtr.Instance.isNoDied ? 0 : 1;//0是 没有时间限制  1是有一分钟限制
        loading.SetActive(true);
        rawImage.gameObject.SetActive(false);
        bool isCanPlay = Android_Call.UnityCallAndroidHasReturn<bool>(AndroidMethod.isCanPlay);

        if (isCanPlay)
        {
            #region 获取二维码
            StartCoroutine(CommTool.TimeFun(60, 2, (ref float t) =>
            {
                if (!GameCtr.Instance.isGetCode)
                {
                    if (t == 0)
                        GameCtr.Instance.AppQuit();
                    else
                        NetMrg.Instance.SendRequest(AndroidMethod.GetDrawQrCode, jsondata);
                    return false;
                }
                else
                {
                    InitMoneyData();
                    GetQRCodeSuccess();
                    return true;
                }
            }));
            #endregion
        }
        else
        {
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                  "没有礼品不能开始游戏");
            DOVirtual.DelayedCall(3, GameCtr.Instance.AppQuit);
        }
    }

    public virtual void InitMoneyData()
    {
        money_text.text = GameCtr.Instance.money.ToString();
        times_text.text = GameCtr.Instance.selectRound.ToString();
    }

    public virtual void GetQRCodeSuccess()
    {
        loading.SetActive(false);
        rawImage.gameObject.SetActive(true);
        if (!GameCtr.Instance.isNoDied)
            StartCoroutine(PlayVoiceIe());
    }

    public virtual IEnumerator PlayVoiceIe()
    {
        int index = 0;
        float total_time = 0;
        int[] timeArray = { 0, 12, 30, 43, 57 };
        while (total_time <= 63)
        {
            if (index < vc_list.Count && index < timeArray.Length && total_time == timeArray[index])
            {
                vc_list[index].Content = CommTool.TransformPayVoice("#", vc_list[index].Content, GameCtr.Instance.money.ToString());
                vc_list[index].Content = CommTool.TransformPayVoice("*", vc_list[index].Content, GameCtr.Instance.selectRound.ToString());
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, vc_list[index].Content);
                index++;
            }
            yield return new WaitForSeconds(1);
            total_time++;
        }
        GameCtr.Instance.AppQuit();
    }
    #endregion

}
