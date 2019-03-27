﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

public sealed class AndroidCallUnity : MonoSingleton<AndroidCallUnity> {

    private bool isGetProbalility = false;//是否获得概率值

    private Action androidHeadDownAction;
    private Action<CallParameter> androidCallAction;
    private Action<JsonData> androidQRCodeAction;
    private Action<JsonData> androidGetProbabilityAction;
    private Action<JsonData> androidPaySuccessAction;
    private Action<string> androidQuestion_WingAction;
    private Action<string> androidImageInfoAction;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_androidHeadDownAction">头部按下</param>
    /// <param name="_androidCallAction">Android调用unity 简单方法</param>
    /// <param name="_androidQRCodeAction">二维码获得成功</param>
    /// <param name="_androidGetProbabilityAction">获得概率值成功</param>
    /// <param name="_androidPaySuccessAction">支付成功</param>
    /// <param name="_androidQuestion_WingAction">摇动翅膀</param>
    /// <param name="_androidImageInfoAction">结束页面图片信息</param>
    public void Init(Action _androidHeadDownAction, Action<CallParameter> _androidCallAction, Action<JsonData> _androidQRCodeAction, 
        Action<JsonData> _androidGetProbabilityAction, Action<JsonData> _androidPaySuccessAction,
        Action<string> _androidQuestion_WingAction, Action<string> _androidImageInfoAction)
    {
        androidHeadDownAction = _androidHeadDownAction;
        androidCallAction = _androidCallAction;
        androidQRCodeAction = _androidQRCodeAction;
        androidGetProbabilityAction = _androidGetProbabilityAction;
        androidPaySuccessAction = _androidPaySuccessAction;
        androidQuestion_WingAction = _androidQuestion_WingAction;
        androidImageInfoAction = _androidImageInfoAction;
    }

    public void RestData()
    {
        isGetProbalility = false;
    }

    //头部按下
    public void HeadDown()
    {
        if (androidHeadDownAction!=null)
        {
            androidHeadDownAction();
        }
    }

    public void AndroidCall(CallParameter result)
    {
        if (androidCallAction != null)
        {
            androidCallAction(result);
        }
    }


    //二维码获得成功
    public void QRCodeCall(JsonData result)
    {
        Debug.Log("二维码获得成功---"+ "--isGetProbalility::"+ isGetProbalility);
        if (!isGetProbalility) return;//金钱获得成功才显示二维码
        if (androidQRCodeAction != null)
        {
            androidQRCodeAction(result);
        }
    }
    //获得概率值
    public void GetProbabilityCall(JsonData result)
    {
        if (androidGetProbabilityAction != null)
        {
            androidGetProbabilityAction(result);
            isGetProbalility = true;
        }
    }

    //支付成功
    public void PaySuccess(JsonData result)
    {
        if (androidPaySuccessAction != null)
        {
            androidPaySuccessAction(result);
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
    //结束页面图片name
    public void GameOverImageInfo(string result)
    {
        if (androidImageInfoAction != null)
        {
            Debug.Log("--结束页面显示图片name****语音--" + result);
            androidImageInfoAction(result);
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
