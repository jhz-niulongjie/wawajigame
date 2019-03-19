using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class AndroidCallUnity : MonoSingleton<AndroidCallUnity> {

    private bool isGetProbalility = false;//是否获得概率值

    private Action<string> androidCallAction;

    private Action<string> androidQRCodeAction;
    private Action<string> androidGetProbabilityAction;
    private Action<string> androidPaySuccessAction;
    private Action<string> androidQuestion_WingAction;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_androidCallAction">Android调用unity 简单方法</param>
    /// <param name="_androidQRCodeAction">二维码获得成功</param>
    /// <param name="_androidGetProbabilityAction">获得概率值成功</param>
    /// <param name="_androidPaySuccessAction">支付成功</param>
    /// <param name="_androidQuestion_WingAction">摇动翅膀</param>
    public void Init(Action<string> _androidCallAction, Action<string> _androidQRCodeAction, 
        Action<string> _androidGetProbabilityAction, Action<string> _androidPaySuccessAction,
        Action<string> _androidQuestion_WingAction)
    {
        androidCallAction = _androidCallAction;
        androidQRCodeAction = _androidQRCodeAction;
        androidGetProbabilityAction = _androidGetProbabilityAction;
        androidPaySuccessAction = _androidPaySuccessAction;
        androidQuestion_WingAction = _androidQuestion_WingAction;
    }

    public void AndroidCall(string result)
    {
        Debug.Log("拍头啦");
        if (androidCallAction != null)
        {
            androidCallAction(result);
        }
    }


    //二维码获得成功
    public void QRCodeCall(string result)
    {
        Debug.Log("二维码获得成功---"+ "--isGetProbalility::"+ isGetProbalility);
        if (!isGetProbalility) return;//金钱获得成功才显示二维码
        if (androidQRCodeAction != null)
        {
            androidQRCodeAction(result);
           // androidQRCodeAction = null;
        }
    }
    //获得概率值
    public void GetProbabilityCall(string result)
    {
        if (androidGetProbabilityAction != null)
        {
            androidGetProbabilityAction(result);
            isGetProbalility = true;
        }
    }

    //支付成功
    public void PaySuccess(string result)
    {
        if (androidPaySuccessAction != null)
        {
            Debug.Log("--支付成功啦- androidPaySuccessAction--");
            androidPaySuccessAction(result);
            androidPaySuccessAction = null;
        }
    }

    //摆动翅膀回答问题
    public void Question_Wing(string result)
    {
        if (androidQuestion_WingAction != null)
        {
            Debug.Log("--摇动了翅膀--"+result);
            androidQuestion_WingAction(result);
        }
    }

    public override void Dispose()
    {
        androidCallAction = null;
        androidQRCodeAction = null;
        androidGetProbabilityAction = null;
        androidPaySuccessAction = null;
        androidQuestion_WingAction = null;
        base.Dispose();
    }
}
