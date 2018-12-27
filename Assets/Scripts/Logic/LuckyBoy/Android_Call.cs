using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Android_Call
{
    private static AndroidJavaObject _androidjava;
    private static AndroidJavaObject androidjava
    {
        get
        {

            if (Application.platform == RuntimePlatform.Android && _androidjava == null)
                _androidjava = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            return _androidjava;
        }

    }
   #region   之前逻辑

    ////获得游戏模式
    //public static string GetGameMode()
    //{
    //    if (androidjava != null)
    //        return androidjava.Call<string>("GetGameModeData");
    //    return null;
    //}

    ////能否开始游戏
    //public static bool IsCanPlay()
    //{
    //    if (androidjava != null)
    //        LuckyBoyManager.Instance.isHas = androidjava.Call<bool>("isCanPlay");
    //    return LuckyBoyManager.Instance.isHas;
    //}
    //是否取走
    //public bool isTabke()
    //{
    //    if (androidjava != null)
    //        isTakeAway = androidjava.Call<bool>("isTakeAway");
    //    return isTakeAway;
    //}

    //public static void GetPayStatus(string orderNo, bool isFirst)
    //{
    //    Debug.Log("查询是否支付");
    //    if (androidjava != null)
    //        androidjava.Call("GetPayStatus", orderNo, isFirst);
    //}
    //public static void GetQR_Code(RawImage r)//二维码载入
    //{
    //    Debug.Log("请求二维码");
    //    if (LuckyBoyManager.Instance.raw == null) LuckyBoyManager.Instance.raw = r;
    //    if (androidjava != null)
    //        androidjava.Call("GetDrawQrCode");
    //}
    /// <summary>
    /// 上传抓中记录  code
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isSuccess"></param>
    //public static void C_UpRecord(bool isSuccess)
    //{
    //    Debug.Log("抓中向服务器传输记录-----C:" + isSuccess);
    //    if (androidjava != null)
    //    {
    //        if (string.IsNullOrEmpty(LuckyBoyManager.Instance.startCarwTime))
    //            LuckyBoyManager.Instance.startCarwTime = CommTool.GetTimeStamp();
    //        androidjava.Call("SendCatchRecord", isSuccess, LuckyBoyManager.Instance.startCarwTime);
    //    }
    //}
    /// <summary>
    /// 上传抓中记录  question
    /// </summary>
    /// <param name="isSuccess"></param>
    //public static void Q_UpRecord(bool isSuccess)
    //{
    //    Debug.Log("抓中向服务器传输记录-----Q:" + isSuccess);
    //    if (androidjava != null)
    //    {
    //        if (string.IsNullOrEmpty(LuckyBoyManager.Instance.startCarwTime))
    //            LuckyBoyManager.Instance.startCarwTime = CommTool.GetTimeStamp();
    //        androidjava.Call("Q_UpRecord", isSuccess, LuckyBoyManager.Instance.startCarwTime, LuckyBoyManager.Instance.Q_startCarwTime);
    //    }
    //}

    //public static void UpRecordList(string list)
    //{
    //    Debug.Log("批量上传记录-----");
    //    if (androidjava != null)
    //        androidjava.Call("SendCatchRecordList", list);
    //}

    //public static void Speak(string msg)
    //{
    //    Debug.Log("播放语音-----" + msg);
    //    if (androidjava != null)
    //        androidjava.Call("uspeak", msg);
    //}
    //public static void Wave(int num)
    //{
    //    Debug.Log("摆动翅膀");
    //    if (androidjava != null)
    //        androidjava.Call("uwave", num);
    //}
    //public static void WonDoll(bool state)
    //{
    //    Debug.Log("抓中摆动翅膀闪灯带--" + state);
    //    if (androidjava != null)
    //        androidjava.Call("wonDoll", state);
    //}
    //public static void GetProbability()
    //{
    //    Debug.Log("获得抓中概率值");
    //    if (androidjava != null)
    //        androidjava.Call("GetProbabilityValue");
    //}
    //public static void CustomQuit()
    //{
    //    Debug.Log("自定义退出");
    //    if (androidjava != null)
    //        androidjava.Call("CustomQuit");
    //}

    //public static void Light(bool n, int num)
    //{
    //    Debug.Log("灯光闪烁啊");
    //    if (androidjava != null)
    //        androidjava.Call("ulight", n, num);
    //}

    //public static void AutoSendPresent()
    //{
    //    Debug.Log("自动送礼物");
    //    //isTakeAway = false;
    //    if (androidjava != null)
    //        androidjava.Call("autoPresent");
    //}

    //public static string GetQuestionAnswer()
    //{
    //    Debug.Log("-获得考题-");
    //    if (androidjava != null)
    //        return androidjava.Call<string>("GetQuestionAnswer");
    //    return null;
    //}

    //public static string GetPayVoice()
    //{
    //    Debug.Log("-获得支付页面语音-");
    //    if (androidjava != null)
    //        return androidjava.Call<string>("GetPayVoice");
    //    return null;
    //}

    //public static void AnswerStartOrEnd(bool state)
    //{
    //    Debug.Log("-开始结束答题：" + state);
    //    if (androidjava != null)
    //        androidjava.Call("AnswerStartOrEnd", state);
    //}

    //public static void HideSplash()
    //{
    //    Debug.Log("-hide Splash");
    //    if (androidjava != null)
    //        androidjava.Call("HideSplash");
    //}
#endregion


   #region 新逻辑
      
    public static T UnityCallAndroidHasReturn<T>(AndroidMethod  _method)
    {
        Debug.Log(_method.GetEnumContent());
        if (androidjava != null)
            return androidjava.Call<T>(_method.ToString());
        return default(T);
    }
     
    public static void UnityCallAndroidHasParameter<T>(AndroidMethod _method, T t)
    {
        Debug.Log(_method.GetEnumContent()+"--"+t);
        if (androidjava != null)
            androidjava.Call(_method.ToString(),t);
    }

    public static void UnityCallAndroidHasParameter<S,B>(AndroidMethod _method,S s,B b)
    {
        Debug.Log(_method.GetEnumContent()+"--"+b+"---"+s);
        if (androidjava != null)
            androidjava.Call(_method.ToString(), s,b);
    }
    public static void UnityCallAndroidHasParameter<S,T,B>(AndroidMethod _method, S s,T t, B b)
    {
        Debug.Log(_method.GetEnumContent() + "--" + t+"---"+s);
        if (androidjava != null)
            androidjava.Call(_method.ToString(), s,t,b);
    }
    public static void UnityCallAndroid(AndroidMethod _method)
    {
        Debug.Log(_method.GetEnumContent());
        if (androidjava != null)
            androidjava.Call(_method.ToString());
    }
#endregion
}
