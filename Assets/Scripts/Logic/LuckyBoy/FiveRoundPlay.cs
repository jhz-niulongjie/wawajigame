using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vMrg_5 = VoiceMrg<ExcelScriptObj_5, VoiceContentType_5>;

public class FiveRoundPlay : GameMisson
{
    //抖动次数
    protected int douDongNum = 0;
    //抖动三次改变游戏难度
    protected const int changePlayNum = 3;

    List<VoiceContentType_5> list;
    //  mode  默认支付模式
    public FiveRoundPlay(GameCtr sdk) : base(sdk)
    {
        Debug.Log("////五局模式\\\\");
        vMrg_5.InitVoiceData("voiceFiveNames");
    }

    public override void StartGame(GameObject police, Transform catchMove)
    {
        _isWin = sdk.gameStatus.status == 1 ? true : false;//是否抓中过
        bool isDouDong = sdk.gameStatus.isDouDong;
        if (_isWin) douDongNum = -10;
        _round = GetRound();
        string msg = string.Format("第-{0}-次抓，第-{1}-局，上局是否抓中-{2}、是否抖动-{3}", _timesPay, _round, _isWin, isDouDong);
        Debug.Log(msg);
        KillTween();
        if (_timesPay == 1)//第一次玩
        {
            switch (_round)
            {
                case 1:
                case 2:
                case 3:
                    NormalPaly(police, catchMove);
                    break;
                case 4:
                    if (douDongNum == changePlayNum)
                        NoPolicePlay(police, catchMove);
                    else
                        NormalPaly(police, catchMove);
                    break;
                case 5:
                    if (douDongNum == changePlayNum)
                        NoPolicePlay(police, catchMove);
                    else if (douDongNum > changePlayNum)
                        EasyPlay(police, catchMove);
                    else
                        NormalPaly(police, catchMove);
                    break;
            }
            UIManager.Instance.ShowUI(UIMessagePage.NAME, true, playAction);
        }
        else if (_timesPay == 2)//第二次玩
        {
            if (_isWin)//抓中国
                NormalPaly(police, catchMove);
            else
            {
                if (_round == 1)//第一局
                    NoPolicePlay(police, catchMove);
                else//第二局 第三局
                {
                    if (isDouDong)
                        EasyPlay(police, catchMove);
                    else
                    {
                        if (_gameLevel == GameLevel.Nan || _gameLevel == GameLevel.Zhong)
                            NoPolicePlay(police, catchMove);
                    }
                }
            }
            UIManager.Instance.ShowUI(UIMessagePage.NAME, true, playAction);
        }
        else if (_timesPay == 3)//第三次玩
        {
            //直接出玩娃娃
            EventHandler.ExcuteEvent(EventHandlerType.Success, CatchTy.Catch);
        }
    }

    public override void NoZhuaZhong(CatchTy cat, ExtendContent voiceContent, out float delytime, out string[] contents)
    {
        delytime = 0;
        contents = null;
        Debug.Log("没抓中类型---" + cat);
        if (_round == 5 || _isWin)
        {
            if (cat == CatchTy.Drop)
            {
                contents = voiceContent.ShootDrop.Content.Split('|');
                delytime = Convert.ToInt32(voiceContent.ShootDrop.Time);
            }
            else if (cat == CatchTy.NoCatch)
            {
                contents = voiceContent.DouDong.Content.Split('|');
                delytime = Convert.ToInt32(voiceContent.DouDong.Time);
            }
            else
            {
                contents = voiceContent.NoDouDong.Content.Split('|');
                delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
            }
        }
        else
        {
            if (_timesPay == 1)
            {
                if (cat == CatchTy.Drop || cat == CatchTy.NoCatch)
                {
                    douDongNum++;
                    if (douDongNum > 3)
                    {
                        if (_round == 4)
                        {
                            contents = voiceContent.DouDong_4DD.Content.Split('|');
                            delytime = Convert.ToInt32(voiceContent.DouDong_4DD.Time);
                        }
                    }
                    else
                    {
                        if (douDongNum == 3)
                        {
                            contents = voiceContent.ShootDrop_3DD.Content.Split('|');
                            delytime = Convert.ToInt32(voiceContent.ShootDrop_3DD.Time);
                        }
                        else
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
                    }
                }
                else //太偏啦
                {
                    contents = voiceContent.NoDouDong.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
                }
            }
            else if (_timesPay == 2)
            {
                if (cat == CatchTy.Drop || cat == CatchTy.NoCatch)
                {
                    if (_gameLevel != GameLevel.Yi)
                    {
                        contents = voiceContent.ShootDrop_3DD.Content.Split('|');
                        delytime = Convert.ToInt32(voiceContent.ShootDrop_3DD.Time);
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
    }

    public override void UpdateRoundVoice()
    {
        VoiceType vt = (VoiceType)sdk.gameMode.gameMisson._timesPay;
        list = vMrg_5.GetRoundVoice(vt, GetRound());
        _Count = list.Count;
    }
    public override void UpdateSpecialVoice(VoiceType vtype)
    {
        list = vMrg_5.GetVoiceForType(vtype);
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
        List<VoiceContentType_5> list = vMrg_5.GetRoundVoice(vtype, type);
        if (list != null)
            return list[0];
        return null;
    }
    //当前局数
    public override int GetRound()
    {
        return 5 - sdk.gameStatus.remainGameRound;
    }

    public override void DropShowPrompt(GameObject prompt, GameObject drop)
    {
        drop.SetActive(true);
        prompt.SetActive(false);
        drop.transform.localPosition = Vector3.zero;
        if (!_isWin && _round < 5 && _gameLevel != GameLevel.Yi)
        {
            if (douDongNum >= changePlayNum || _timesPay == 2)
            {
                prompt.SetActive(true);
                drop.transform.localPosition = new Vector3(0, 45, 0);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        if (list != null)
            list.Clear();
        vMrg_5.Clear();
    }
}
