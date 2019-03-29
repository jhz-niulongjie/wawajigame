using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using System.IO;
using System.Text;
using LitJson;

public sealed class NetMrg : Singleton<NetMrg>
{
    private BestHttpImpl httpImpl = null;
    private bool isText = false;
    public string robotId { get; private set; }

    private NetMrg()
    {
        httpImpl = new BestHttpImpl();
        httpImpl.SetHttpParams(false);
        httpImpl.AddHead("Content-Type", "application/json");
        isText = Android_Call.UnityCallAndroidHasReturn<bool>(AndroidMethod.IsText);
        robotId = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.getRobotId);
        Debug.Log("小胖ID***" + robotId + "***是否测试环境***" + isText);
        //robotId = "100000000009";
        //isText = true;
    }


    /// <summary>
    /// 发送请求
    /// </summary>
    ///  <param name="method">请求内容</param>
    /// <param name="requestParams">参数</param>
    public void SendRequest(AndroidMethod method, JsonData requestParams = null)
    {
        string httpUrl = GetUrl(method);
        Debug.Log("HTTP:::" + method.GetEnumContent() + "***" + httpUrl);
        //Dictionary<string, string> requestParams = new Dictionary<string, string>
        //{
        //    //{ "name","nlj"},
        //    //{ "sex","boy"}
        //};
        if (requestParams == null)
            requestParams = new JsonData();
        requestParams["robotId"] = robotId;

        // 是否打开多次回调模式
        bool isOpenStram = false;
        httpImpl.Post(httpUrl, requestParams, isOpenStram, rsp =>
        {
            if (rsp != null)
            {
                Debug.Log("响应成功***" + method.GetEnumContent()+ "******"+ rsp.DataAsText);
                JsonData jsonData = JsonMapper.ToObject(rsp.DataAsText);
                string resultCode = jsonData["resultCode"].ToString();
                if (resultCode == "SUCCESS")
                {
                    switch (method)
                    {
                        case AndroidMethod.GetProbabilityValue:
                            AndroidCallUnity.Instance.GetProbabilityCall(jsonData["data"]);
                            break;
                        case AndroidMethod.GetDrawQrCode:
                            AndroidCallUnity.Instance.QRCodeCall(jsonData);
                            break;
                        case AndroidMethod.GetPayStatus:
                        case AndroidMethod.GetPayStatusSendPhone:
                            string status = jsonData["status"].ToString();
                            if (status == "1")//支付成功
                                AndroidCallUnity.Instance.PaySuccess(jsonData);
                            break;
                        case AndroidMethod.ResPhoneCode:
                            AndroidCallUnity.Instance.Question_Wing(jsonData["code"].ToString());
                            break;
                        case AndroidMethod.SendCatchRecordList:
                            AndroidCallUnity.Instance.AndroidCall(CallParameter.UpRecordListSuccess);
                            break;
                        case AndroidMethod.SendCatchRecord:
                        case AndroidMethod.Q_UpRecord:
                            break;
                        default:
                            Debug.Log("响应类型不匹配");
                            break;
                    }
                }
                else if (resultCode == "NO_DOLL_ROBOT" || resultCode == "ACTIVE_ROBOT")//娃娃机未绑定
                {
                    AndroidCallUnity.Instance.AndroidCall(CallParameter.NoBind);
                }
            }
        },
        fail =>
        {
            Debug.Log("响应失败***" + method.GetEnumContent());
            switch (method)
            {
                case AndroidMethod.SendCatchRecord:
                case AndroidMethod.Q_UpRecord:
                    AndroidCallUnity.Instance.AndroidCall(CallParameter.UpRecordFail);
                    break;
            }
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
            case AndroidMethod.ResPhoneCode:
                httpUrl = ip + AppConst.TestPhone_GetPhoneCode;
                break;
            case AndroidMethod.GetPayStatusSendPhone:
                httpUrl = ip + AppConst.TestPhone_PayStatus;
                break;
        }
        return httpUrl;
    }
}


