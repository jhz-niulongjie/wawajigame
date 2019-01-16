using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class CatchSuccessData
{
    //抓取成功的数量
    public int cnum;
    public string openId;
    public string applyRechargeid;
}
public sealed class LuckySendPhoneMgr : GameCtr {

    // 今天支付次数 抓取成功次数
    public List<CatchSuccessData> catchlist { get; private set; }
    //兑换码
    public string phoneCode { get; private set; }
    protected override void EnterGame()
    {
        Debug.Log("进入幸运送平板游戏");
        gameMode = new SendPhoneMode(this, 3);
        gameStatus.SetRemainRound(3);
        gameMode.EnterGame();
    }

    protected override void QRCodeCall(string result)
    {
        if (isGetCode) return;
        isGetCode = true;
        string[] msgs = result.Split('|');
        QRCode.ShowCode(raw, msgs[0]);
        gameStatus.SetOrderNoRobotId(msgs[1], msgs[2]);
        gameStatus.SetRunStatus(GameRunStatus.NoPay);
        EventHandler.ExcuteEvent(EventHandlerType.QRCodeSuccess, null);
        StartCoroutine(CommTool.TimeFun(2, 2, (ref float t) =>
        {
            if (!isPaySucess)
            {
                //检测是否支付
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.GetPayStatusSendPhone, msgs[1]);
            }
            if (t == 0) t = 2;
            return isPaySucess;
        }));
    }

    protected override void PaySuccess(string result)
    {
        if (!string.IsNullOrEmpty(result))
        {
            isPaySucess = true;
            string[] datas = result.Split('|');
            Debug.Log("支付成功--openId::" + datas[0]);
            gameStatus.SetOpenId(datas[0]);
            JsonData j_data = JsonMapper.ToObject(datas[1]);
            catchlist = JsonMapper.ToObject<List<CatchSuccessData>>(j_data.ToJson());
            Debug.Log("catchlist.Count---" + catchlist.Count);
            for (int i = 0; i < catchlist.Count; i++)
            {
                Debug.Log("cnum---" +catchlist[i].cnum+"--openId--"+ catchlist[i].openId+"--applyId--"+ catchlist[i].applyRechargeid);
            }
            gameMode.GameStart();
        }
    }

    //获得兑换码
    protected override void Question_Wing(string result)
    {
        phoneCode = result;
    }
}
