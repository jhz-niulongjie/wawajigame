using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Phone_ThreeRoundPlay : GameMisson
{
    Dictionary<SendPhoneStatusType, Dictionary<SendPhoneOperateType, List<VoiceContent>>> sendPhoneOperDic;
    Dictionary<SendPhoneOperateType, List<VoiceContent>> operDic;
    public Phone_ThreeRoundPlay(GameCtr _sdk) : base(_sdk)
    {
        //注册警察移动
        EventDispatcher.AddListener(EventHandlerType.RoundOver, playSelfAction);
        LoadVoice();
    }

    public override void StartGame(GameObject police, Transform catchMove)
    {
        KillTween();
        NormalPaly(police, catchMove);
        _round = GetRound();
        UIManager.Instance.ShowUI(UIRoundEnterPage.NAME, true, _round);
    }

    public override List<VoiceContent> GetVoiceContentBy(int statueType, int operType)
    {
        SendPhoneStatusType statusType = (SendPhoneStatusType)statueType;
        operDic = sendPhoneOperDic[statusType];
        SendPhoneOperateType oper = (SendPhoneOperateType)operType;
        if (operDic.ContainsKey(oper))
        {
            return operDic[oper];
        }
        return null;
    }
    //当前局数
    public override int GetRound()
    {
        return 3 - sdk.gameStatus.remainGameRound;
    }

    public override void NoZhuaZhong(CatchTy cat, ExtendContent voiceContent, out float delytime, out string[] contents)
    {
        delytime = 0;
        contents = null;
        if (voiceContent == null)
            operDic = sendPhoneOperDic[SendPhoneStatusType.TryPlay];
        else
            operDic = sendPhoneOperDic[SendPhoneStatusType.Common];
        if (cat == CatchTy.Drop)//掉落
        {
            if (operDic.ContainsKey(SendPhoneOperateType.Drop))
            {
                var list = operDic[SendPhoneOperateType.Drop];
                string speak = list[UnityEngine.Random.Range(0, list.Count - 1)].Content;
                contents = new string[] { speak };
                delytime = speak.Length * AppConst.speakTime;
            }
        }
        else
        {
            if (operDic.ContainsKey(SendPhoneOperateType.NoCatch))
            {
                var list = operDic[SendPhoneOperateType.NoCatch];
                string speak = list[UnityEngine.Random.Range(0, list.Count - 1)].Content;
                contents = new string[] { speak };
                delytime = speak.Length * AppConst.speakTime;
            }
        }
    }

    public override void SetDropProbability(bool isReach, ref CatchTy catchty)
    {
        //是否掉
        bool flag = RandomProbli.GetSupriseGiftPro();
        if (!flag)//  必掉
        {
            catchty = CatchTy.Drop;
        }
    }

    private void LoadVoice()
    {
        LuckyTurn sendPhone = VoiceMrg<LuckyTurn, ExtendContent>.GetVoiceFromAsset("sendphone");
        sendPhoneOperDic = sendPhone.luckyTurnList.GroupBy(v => (SendPhoneStatusType)Convert.ToInt32(v.Id)).
            ToDictionary(g => g.Key, g => g.GroupBy(v => (SendPhoneOperateType)Enum.Parse(typeof(SendPhoneOperateType), v.Type)).ToDictionary(n => n.Key, n => n.ToList()));
        operDic = sendPhoneOperDic[SendPhoneStatusType.TryPlay];
    }
    //警察移动
    private void playSelfAction()
    {
        if (playAction != null) playAction();
    }


}
