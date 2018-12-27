using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class Q_ThreeRoundPlay : ThreeRoundPlay
{

    public Q_ThreeRoundPlay(GameCtr _sdk) : base(_sdk)
    {

    }

    public override void NoZhuaZhong(CatchTy cat, ExtendContent voiceContent, out int delytime, out string[] contents)
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
                if (_round == 3)
                {
                    contents = voiceContent.ShootDropWin.Content.Split('|');
                    delytime = Convert.ToInt32(voiceContent.ShootDropWin.Time);
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
                if (_round == 3)
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
            contents = voiceContent.NoDouDong.Content.Split('|');
            delytime = Convert.ToInt32(voiceContent.NoDouDong.Time);
        }
    }

   
}
