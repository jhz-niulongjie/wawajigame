using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using vMrg_3 = VoiceMrg<ExcelScriptObj_3, VoiceContentType_3>;

public class ThreeRoundPlay : GameMisson
{
    List<VoiceContentType_3> list;
    int douDongNum = 0;//抖动次数
    public ThreeRoundPlay(GameCtr sdk) : base(sdk)
    {
        Debug.Log("/////三局模式\\\\\\");
        vMrg_3.InitVoiceData("voiceThreeNames");
    }

    public override void StartGame(GameObject police, Transform catchMove)
    {
        _isWin = sdk.gameStatus.status == 1 ? true : false;//是否抓中过
        bool isDouDong = sdk.gameStatus.isDouDong;
        _round = GetRound();
        string msg = string.Format("第-{0}-次抓，第-{1}-局，上局是否抓中-{2},是否抖动-{3}", _timesPay, _round, _isWin, isDouDong);
        Debug.Log(msg);
        KillTween();
        if (GameCtr.Instance.ChangeType<LuckyBoyMgr>().isAddConstraint)
        {
            //受限 执行最高难度  且降低难度逻辑变化   
            //前两局都是最高难度  前两局都碰到第三局才降低为中等难度 
            if (_round < 3)//第一局  第二局
            {
                NormalPaly(police, catchMove);
                if (_round == 2 && !_isWin && isDouDong) douDongNum++;
            }
            else
            {
                if (_isWin)//抓中过
                    NormalPaly(police, catchMove);
                else
                {
                    if (!_isWin && isDouDong) douDongNum++;
                    if (douDongNum == 2)//抖动两次
                        NoPolicePlay(police, catchMove);
                    else
                        NormalPaly(police, catchMove);
                }
            }
            UIManager.Instance.ShowUI(UIMessagePage.NAME, true, playAction);
        }
        else   //没达到条件  走之前的逻辑
        {
            if (_timesPay == 1)//第一次玩
            {
                if (_round == 1)//第一局
                    NormalPaly(police, catchMove);
                else if (_round == 2)//第二局
                {
                    if (_isWin)
                        NormalPaly(police, catchMove);
                    else//没抓中
                    {
                        //在抖动范围内
                        if (isDouDong)
                            NoPolicePlay(police, catchMove);
                        else
                            NormalPaly(police, catchMove);
                    }
                }
                else
                {
                    if (_isWin)//抓中国
                        NormalPaly(police, catchMove);
                    else
                    {
                        //在抖动范围内
                        if (isDouDong)
                        {
                            if (_gameLevel == GameLevel.Nan)
                                NoPolicePlay(police, catchMove);
                            else
                                EasyPlay(police, catchMove);
                        }
                        else
                        {
                            if (_gameLevel == GameLevel.Nan)
                                NormalPaly(police, catchMove);
                            else
                                NoPolicePlay(police, catchMove);
                        }
                    }
                }
                UIManager.Instance.ShowUI(UIMessagePage.NAME, true, playAction);
            }
            else if (_timesPay == 2)//第二次玩
            {
                if (_round == 1)//第一局
                    EasyPlay(police, catchMove);
                else//第二局 第三局
                {
                    if (_isWin)//抓中国
                        NormalPaly(police, catchMove);
                    else
                        EasyPlay(police, catchMove);
                }
                UIManager.Instance.ShowUI(UIMessagePage.NAME, true, playAction);
            }
            else if (_timesPay == 3)//第三次玩
            {
                //直接出玩娃娃
                EventHandler.ExcuteEvent(EventHandlerType.Success, CatchTy.Catch);
            }
        }
    }

    public override void NoZhuaZhong(CatchTy cat, ExtendContent voiceContent, out float delytime, out string[] contents)
    {

        if (GameCtr.Instance.ChangeType<LuckyBoyMgr>().isAddConstraint)
        {
            if (_isWin)
            {
                if (cat == CatchTy.Drop)
                {
                    contents = voiceContent.ShootDropWin.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.ShootDropWin.Time);
                }
                else
                {
                    contents = voiceContent.NoDouDong.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                }
            }
            else
            {
                //抖动了两次  且是第二局 下局难度降低
                if (_round == 2 && douDongNum == 1)
                {
                    if (cat == CatchTy.Drop)
                    {
                        contents = voiceContent.ShootDrop.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.ShootDrop.Time);
                    }
                    else
                    {
                        contents = voiceContent.DouDong.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.DouDong.Time);
                    }
                }
                else
                {
                    contents = voiceContent.NoDouDong.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                }
            }
        }
        else
        {
            if (cat == CatchTy.NoCatch)//抖动
            {
                Debug.Log("...抖动啦。。");
                if (_isWin)
                {
                    contents = voiceContent.NoDouDong.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                }
                else
                {
                    //不送礼品  大于等于第二次支付 不说送礼品的话 
                    if (!sdk.autoSendGift && _timesPay >= 2 && _round == 3)
                    {
                        contents = voiceContent.NoDouDong.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                    }
                    else
                    {
                        contents = voiceContent.DouDong.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.DouDong.Time);
                    }
                }
            }
            else if (cat == CatchTy.Drop)
            {
                Debug.Log("...打掉啦。。");
                if (_isWin)//打落决对是已抓中过
                {
                    contents = voiceContent.ShootDropWin.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.ShootDropWin.Time);
                }
                else
                {
                    //不送礼品  大于等于第二次支付 不说送礼品的话 
                    if (!sdk.autoSendGift && _timesPay >= 2 && _round == 3)
                    {
                        contents = voiceContent.ShootDropWin.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.ShootDropWin.Time);
                    }
                    else
                    {
                        contents = voiceContent.ShootDrop.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.ShootDrop.Time);
                    }
                }
            }
            else //太偏
            {
                Debug.Log("...太偏啦。。");
                if (_round == 3)
                {
                    if (_isWin)
                    {
                        contents = voiceContent.NoDouDong.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                    }
                    else
                    {
                        if (!sdk.autoSendGift && _timesPay >= 2)
                        {
                            contents = voiceContent.NoDouDong.Content.Split('|');
                            delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                        }
                        else
                        {
                            contents = voiceContent.DouDong.Content.Split('|');
                            delytime = Convert.ToInt32(voiceContent.DouDong.Time);
                        }
                    }
                }
                else
                {
                    contents = voiceContent.NoDouDong.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                }
            }
        }
    }

    public override void UpdateRoundVoice()
    {
        VoiceType vt = (VoiceType)sdk.gameMode.gameMisson._timesPay;
        list = vMrg_3.GetRoundVoice(vt, GetRound());
        _Count = list.Count;
    }

    public override void UpdateSpecialVoice(VoiceType vtype)
    {
        list = vMrg_3.GetVoiceForType(vtype);
        _Count = list.Count;
    }

    public override ExtendContent GetVoiceContent(int index)
    {
        if (list != null && list.Count > index)
        {
            return list[index];
        }
        return null;
    }


    public override ExtendContent GetSpecialVoice(VoiceType vtype, int type)
    {
        List<VoiceContentType_3> list = vMrg_3.GetRoundVoice(vtype, type);
        if (list != null)
            return list[0];
        return null;
    }
    //当前局数
    public override int GetRound()
    {
        return 3 - sdk.gameStatus.remainGameRound;
    }

    public override void DropShowPrompt(GameObject prompt, GameObject drop)
    {
        drop.SetActive(true);
        prompt.SetActive(false);
        drop.transform.localPosition = Vector3.zero;
        if (!_isWin && _round < 3 && _gameLevel != GameLevel.Yi || douDongNum == 1)
        {
            prompt.SetActive(true);
            drop.transform.localPosition = new Vector3(0, 45, 0);
        }
    }
    public override void Clear()
    {
        base.Clear();
        if (list != null)
            list.Clear();
        vMrg_3.Clear();
    }
}
