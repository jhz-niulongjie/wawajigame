using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public sealed class Loading : MonoBehaviour {

    private enum SelectEnterGame
    {
        Loading=0,
        LuckyBoy,
        LuckyTurnTable,
        LuckyBigBom,
        LuckySendPhone,
    }

    private int loadTime = 6;//网络监测时间

    private void Awake()
    {
        Debug.Log("游戏启动");
        Android_Call.UnityCallAndroid(AndroidMethod.HideSplash);//隐藏splash图片
        if (AppConst.test) loadTime = 1;
        StartNetCheck();
    }

    /// <summary>
    /// 检测网络 进入游戏
    /// </summary>
    private void StartNetCheck()
    {

        Debug.Log("=====开始网络监测=====");
        if (AppConst.test)
            UnityPing.CreatePing("www.baidu.com", loadTime, EnterGame, EnterGame);
        else
            UnityPing.CreatePing("www.baidu.com", loadTime, EnterGame, AppQuit);
    }
    /// <summary>
    /// 根据选择进入游戏
    /// </summary>
    private void EnterGame()
    {
        if (!AppConst.test)
        {
            ////0 是抓娃娃  1转转转 3大炮筒
            string select_ = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.SelectGame);
            Debug.Log("选择的游戏：：" + select_);
            select_ = (Convert.ToInt32(select_) + 1).ToString();//为了对应枚举 还有场景值
            if (select_ == ((int)SelectEnterGame.LuckyBoy).ToString())
            {
                SceneManager.LoadScene((int)SelectEnterGame.LuckyBoy);
            }
            else if (select_ == ((int)SelectEnterGame.LuckyTurnTable).ToString())
            {
                SceneManager.LoadScene((int)SelectEnterGame.LuckyTurnTable);
            }
            else if (select_ == ((int)SelectEnterGame.LuckyBigBom).ToString())
            {
                SceneManager.LoadScene((int)SelectEnterGame.LuckyBigBom);
            }

            //SceneManager.LoadScene((int)SelectEnterGame.LuckySendPhone);
        }
        else
        {
            SceneManager.LoadScene((int)SelectEnterGame.LuckyBoy);
        }
    }
    /// <summary>
    /// 游戏退出
    /// </summary>
    private void AppQuit()
    {
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "小胖检测到网络不好，请联网后重试");//隐藏splash图片
        DOVirtual.DelayedCall(5,()=> 
        {
            Debug.Log("游戏退出");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            Application.Quit();
        });
    }
}
