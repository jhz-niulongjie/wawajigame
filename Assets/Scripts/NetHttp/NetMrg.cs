using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using System.IO;
using System.Text;

public sealed class NetMrg : Singleton<NetMrg>
{
    private BestHttpImpl httpImpl = null;
    private bool isText = false;
    private string robotId = "100000056918";

    private NetMrg()
    {
        httpImpl = new BestHttpImpl();
        httpImpl.SetHttpParams(false);
        //httpImpl.AddHead("content-type","application/json");
        httpImpl.AddHead("Content-Type","application/json");
        isText = Android_Call.UnityCallAndroidHasReturn<bool>(AndroidMethod.IsText);
        robotId = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.getRobotId);
        Debug.Log("小胖ID***" + robotId + "***是否测试环境***" + isText);
        robotId = "100000056918";
    }



    /// <summary>
    /// 发送请求
    /// </summary>
    ///  <param name="method">请求内容</param>
    /// <param name="requestParams">参数</param>
    public void SendRequest(AndroidMethod method, Dictionary<string, string> requestParams = null)
    {
        string httpUrl = GetUrl(method);
        //string httpUrl = "http://192.168.15.12:8096/v1/wxluckDraw/getLuckDrawQrCode";
        Debug.Log(method.GetEnumContent() + "***" + httpUrl);
        //Dictionary<string, string> requestParams = new Dictionary<string, string>
        //{
        //    //{ "name","nlj"},
        //    //{ "sex","boy"}
        //};
        if (requestParams == null)
            requestParams = new Dictionary<string, string>();
        requestParams.Add("robotId", robotId);

        // 是否打开多次回调模式
        bool isOpenStram = false;
        httpImpl.Post(httpUrl, requestParams, isOpenStram, rsp =>
        {
            if (rsp != null)
            {
                Debug.Log("收到响应***" + rsp.DataAsText);

                switch (method)
                {
                    case AndroidMethod.GetProbabilityValue:
                        AndroidCallUnity.Instance.GetProbabilityCall(rsp.DataAsText);
                        break;
                    case AndroidMethod.GetDrawQrCode:
                        AndroidCallUnity.Instance.QRCodeCall(rsp.DataAsText);
                        break;
                    case AndroidMethod.GetPayStatus:
                    case AndroidMethod.GetPayStatusSendPhone:
                        AndroidCallUnity.Instance.PaySuccess(rsp.DataAsText);
                        break;
                    case AndroidMethod.ResPhoneCode:
                        AndroidCallUnity.Instance.Question_Wing(rsp.DataAsText);
                        break;
                    case AndroidMethod.SendCatchRecordList:
                        AndroidCallUnity.Instance.PaySuccess(rsp.DataAsText);
                        break;
                    default:
                        Debug.Log("响应类型不匹配");
                        break;
                }

            }
        },
        fail =>
        {


        });
    }

    //获得IP
    private string GetUrl(AndroidMethod method)
    {
        string httpUrl = "";
        string ip = isText ? AppConst.Test_IP : AppConst.IP;
        switch (method)
        {
            case AndroidMethod.GetProbabilityValue:
                httpUrl = ip + AppConst.LuckCatchProbability;
                break;
            case AndroidMethod.GetDrawQrCode:
                httpUrl = ip + AppConst.LuckDrawQrCode;
                break;
            case AndroidMethod.GetPayStatus:
                httpUrl = ip + AppConst.LuckPayStatus;
                break;
            case AndroidMethod.SendCatchRecord:
                httpUrl = ip + AppConst.LuckCatchRecord;
                break;
            case AndroidMethod.SendCatchRecordList:
                httpUrl = ip + AppConst.LuckCatchRecordList;
                break;
        }
        return httpUrl;
    }
}


